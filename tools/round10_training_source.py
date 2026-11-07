"""Source-only training composition from the retained D33 prototype."""
from collections import Counter
from dataclasses import dataclass
import hashlib
import json
from pathlib import Path
import re

@dataclass(frozen=True)
class Token:
    value: str
    start: int
    end: int


def lex(text):
    result = []
    i = 0
    while i < len(text):
        if text[i].isspace():
            i += 1
            continue
        if text.startswith('//', i):
            n = text.find('\n', i + 2)
            i = len(text) if n < 0 else n
            continue
        if text.startswith('/*', i):
            n = text.find('*/', i + 2)
            if n < 0:
                raise ValueError('unterminated-comment')
            i = n + 2
            continue
        start = i
        verbatim = text.startswith('@"', i)
        if verbatim or text[i] in ('"', "'"):
            quote = '"' if verbatim else text[i]
            i += 2 if verbatim else 1
            while i < len(text):
                if text[i] == quote:
                    if verbatim and text.startswith('""', i):
                        i += 2
                        continue
                    i += 1
                    break
                i += 2 if not verbatim and text[i] == '\\' else 1
            else:
                raise ValueError('unterminated-literal')
        else:
            m = re.match(r'[A-Za-z_@][A-Za-z_0-9]*|[0-9]+|==|!=|&&|\|\||=>', text[i:])
            i += len(m.group()) if m else 1
        result.append(Token(text[start:i], start, i))
    return result


def matching(tokens, start):
    pairs = {'(': ')', '[': ']', '{': '}'}
    stack = [pairs[tokens[start].value]]
    for i in range(start + 1, len(tokens)):
        v = tokens[i].value
        if v in pairs:
            stack.append(pairs[v])
        elif v in (')', ']', '}'):
            if not stack or stack.pop() != v:
                raise ValueError('unbalanced-delimiter')
            if not stack:
                return i
    raise ValueError('unclosed-delimiter')


def vals(text):
    return [z.value for z in lex(text)]


def find(tokens, needle):
    values = [z.value for z in tokens]
    wanted = vals(needle)
    hits = [i for i in range(len(values) - len(wanted) + 1) if values[i:i + len(wanted)] == wanted]
    if len(hits) != 1:
        raise ValueError('token-pattern-not-unique:' + needle[:80])
    return hits[0], len(wanted)


def body(tokens, if_start):
    if tokens[if_start].value != 'if' or tokens[if_start + 1].value != '(':
        raise ValueError('if-required')
    cond_end = matching(tokens, if_start + 1)
    opening = cond_end + 1
    if tokens[opening].value != '{':
        raise ValueError('explicit-body-required')
    return opening, matching(tokens, opening)


def inventory(text):
    tokens = lex(text)
    literals = [z.value for z in tokens if z.value.startswith(('"', '@"', "'"))]
    excluded = {'if', 'for', 'while', 'switch', 'catch', 'using', 'lock', 'typeof', 'sizeof', 'checked', 'unchecked', 'default'}
    calls = [tokens[i - 1].value for i, z in enumerate(tokens) if z.value == '(' and i and re.fullmatch(r'[A-Za-z_@][A-Za-z_0-9]*', tokens[i - 1].value) and tokens[i - 1].value not in excluded]
    return {'lexical_call_like_names': dict(sorted(Counter(calls).items())),
            'literal_tokens': literals,
            'return_count': sum(z.value == 'return' for z in tokens),
            'anonymous_function_count': sum(z.value in ('delegate', '=>') for z in tokens),
            'note': 'Lexical inventory only; not semantic resolution or an AST/model.'}


def sha(data):
    return hashlib.sha256(data).hexdigest()


def anonymous_body_hashes(text):
    t = lex(text)
    result = []
    for i, token in enumerate(t):
        if token.value != 'delegate':
            continue
        opening = next(j for j in range(i + 1, len(t)) if t[j].value == '{')
        closing = matching(t, opening)
        result.append(sha(text[t[opening].start:t[closing].end].encode('utf-8')))
    return result


def extract(source, marker):
    if source.count(marker) != 1:
        raise ValueError('method-marker-not-unique')
    start = source.index(marker)
    t = lex(source[start:])
    root = next(i for i, z in enumerate(t) if z.value == '{')
    end = start + t[matching(t, root)].end
    return start, end, source[start:end]


def edit(text, start, end, replacement, label, steps):
    steps.append({'component': label, 'start_utf16_in_step_input': len(text[:start].encode('utf-16-le')) // 2,
                  'end_utf16_in_step_input': len(text[:end].encode('utf-16-le')) // 2,
                  'before': text[start:end], 'after': replacement})
    return text[:start] + replacement + text[end:]


def frames(t, position):
    result = []
    for i in range(position):
        if t[i].value == '{': result.append(i)
        elif t[i].value == '}': result.pop()
    return result


def training_switch_to_if(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, size = find(t, 'switch (tMode)')
    opening = start + size
    if t[opening].value != '{':
        raise ValueError('training-switch-explicit-body-required')
    end = matching(t, opening)
    sections = []
    i = opening + 1
    while i < end:
        if t[i].value != 'case' or t[i + 2].value != ':':
            raise ValueError('training-single-constant-label-required')
        number = t[i + 1].value
        first = i + 3
        j = first
        while j < end:
            if t[j].value in ('case', 'default'):
                break
            if t[j].value == '{':
                j = matching(t, j) + 1
            else:
                j += 1
        if [z.value for z in t[j - 2:j]] != vals('break;'):
            raise ValueError('training-case-must-end-in-break')
        if any(z.value in ('return', 'break', 'continue', 'goto', 'case', 'default') for z in t[first:j - 2]):
            raise ValueError('training-payload-has-other-control-transfer')
        payload = text[t[first].start:t[j - 2].start]
        sections.append((number, payload))
        i = j
    if [n for n, _ in sections] != ['0', '1', '3']:
        raise ValueError('training-exact-ordered-cases-0-1-3-required')
    replacement = ' '.join(('if' if i == 0 else 'else if') + ' (tMode == ' + number + ') {' + payload + '}'
                           for i, (number, payload) in enumerate(sections))
    return edit(text, t[start].start, t[end].end, replacement,
                'training-direct-argument-tests-0-1-3-without-switch-cache-or-final-break', steps)


def training_common_exit_else(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (tMode == 2)')
    a, b = body(t, start)
    stack = frames(t, start)
    if len(stack) != 1 or stack[0] + 1 != start:
        raise ValueError('training-mode2-must-be-first-root-statement')
    root_end = matching(t, stack[0])
    if root_end != len(t) - 1:
        raise ValueError('training-input-must-be-one-complete-method')
    if [z.value for z in t[b - 2:b]] != vals('return;'):
        raise ValueError('training-mode2-must-end-in-return-only-transfer')
    if sum(z.value == 'return' for z in t) != 1:
        raise ValueError('training-other-returns-unsupported')
    tail = text[t[b].end:t[root_end].start]
    tail_prefix = vals('WIN_CHA[1] = 0; int num5 = 0; if (tMode == 0)')
    if vals(tail)[:len(tail_prefix)] != tail_prefix:
        raise ValueError('training-original-tail-declaration-and-direct-dispatch-required')
    true = text[t[start].start:t[b - 2].start] + '}'
    return edit(text, t[start].start, t[root_end].start,
                true + ' else {' + tail + '}',
                'training-mode2-common-exit-join-else-tail-retain-named-local-order', steps)


def _training(text):
    steps = []
    text = training_switch_to_if(text, steps)
    return training_common_exit_else(text, steps), steps


def transform_training(method_source):
    return _training(method_source)[0]

