"""Pure lexical block-exit compositions; UNKNOWN, not READY.
No I/O in import/APIs; no compiler/model/AST/fixture/verifier.
"""
from collections import Counter
from dataclasses import dataclass
import hashlib
import json
from pathlib import Path
import re
import subprocess

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


def frames_before(t, position):
    result = []
    for i in range(position):
        if t[i].value == '{': result.append(i)
        elif t[i].value == '}': result.pop()
    return result


def condition_owner(t, opening, keyword, expected):
    if t[opening - 1].value != ')': raise ValueError('control-owner-parentheses-required')
    pairs = [i for i in range(opening - 1) if t[i].value == '(' and matching(t, i) == opening - 1]
    if len(pairs) != 1 or t[pairs[0] - 1].value != keyword:
        raise ValueError('lexical-control-owner-not-proven')
    if [z.value for z in t[pairs[0] + 1:opening - 1]] != vals(expected):
        raise ValueError('lexical-control-condition-not-proven')


def expression_statements(t, first, end, expected_names):
    """Only exact count of call statements, no declarations/control or labels."""
    names = []
    cursor = first
    while cursor < end:
        start = cursor
        # Accept bare Cal(...) or apdat_.DrawSeb(...), never a declaration/assignment.
        while cursor < end and t[cursor].value != '(': cursor += 1
        if cursor == end: raise ValueError('tail-call-parentheses-missing')
        prefix = ''.join(z.value for z in t[start:cursor])
        closing = matching(t, cursor)
        if closing + 1 >= end or t[closing + 1].value != ';':
            raise ValueError('tail-not-only-call-statements')
        names.append(prefix)
        cursor = closing + 2
    if names != expected_names: raise ValueError('tail-call-identities-not-proven')


def scoped_highlight_tail(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if ((Config.TRIAL_VERSION) && (Language.English()))')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('return;'): raise ValueError('highlight-guard-must-be-void-return-only')
    frames = frames_before(t, start)
    if len(frames) < 2: raise ValueError('highlight-block-and-loop-required')
    parent, loop = frames[-1], frames[-2]
    condition_owner(t, parent, 'if', 'i == selMenu_')
    # A for(i=0;...) is the immediately enclosing control of this highlight block.
    if t[loop - 1].value != ')': raise ValueError('highlight-loop-not-proven')
    pairs = [i for i in range(loop - 1) if t[i].value == '(' and matching(t, i) == loop - 1]
    if len(pairs) != 1 or t[pairs[0] - 1].value != 'for': raise ValueError('highlight-owner-not-for')
    if [z.value for z in t[pairs[0] + 1:pairs[0] + 5]] != vals('int i = 0'):
        raise ValueError('highlight-loop-variable-not-proven')
    end = matching(t, parent)
    next_header = vals('if (i == selMenu_)')
    if [z.value for z in t[end + 1:end + 1 + len(next_header)]] != next_header:
        raise ValueError('highlight-next-sibling-not-proven')
    expression_statements(t, b + 1, end, ['apdat_.DrawSeb', 'apdat_.DrawSeb'])
    payload = text[t[b].end:t[end].start]
    condition = text[t[start].start:t[a].start]
    return edit(text, t[start].start, t[end].start, condition + '{ } else {' + payload + '}',
                'highlight-local-empty-arm-else-two-calls-preserve-next-loop-sibling', steps)


def integer_slot_ternaries(text, steps=None):
    steps = [] if steps is None else steps
    for yes, no in [(6, 11), (2, 6), (4, 8)]:
        t = lex(text)
        start, count = find(t, '(j != 0) ? ' + str(yes) + ' : ' + str(no))
        text = edit(text, t[start].start, t[start + count - 1].end,
                    '(j == 0) ? ' + str(no) + ' : ' + str(yes),
                    'int32-zero-first-ternary-' + str(no) + '-' + str(yes), steps)
    return text


def scoped_passer_tail(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (udat_.gm_.state_ == 5)')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('return;'): raise ValueError('passer-guard-must-be-void-return-only')
    frames = frames_before(t, start)
    if len(frames) != 2: raise ValueError('passer-parent-must-be-root-if-section')
    parent = frames[-1]
    condition_owner(t, parent, 'if', 'ch_ceva[7] == 1')
    end = matching(t, parent)
    next_header = vals('if (ch_ceva[8] == 1)')
    if [z.value for z in t[end + 1:end + 1 + len(next_header)]] != next_header:
        raise ValueError('passer-next-section-not-proven')
    expression_statements(t, b + 1, end, ['Cal_evaCoatMulti9block'])
    expected = vals('Cal_evaCoatMulti9block(coatEva_new_deffence, cx, cy, CEVA_PASSER_MP2_DIV);')
    if [z.value for z in t[b + 1:end]] != expected: raise ValueError('passer-tail-call-arguments-not-proven')
    payload = text[t[b].end:t[end].start]
    condition = text[t[start].start:t[a].start]
    return edit(text, t[start].start, t[end].start, condition + '{ } else {' + payload + '}',
                'passer-section-local-empty-arm-else-call-preserve-ch8-sibling', steps)


def _title(text):
    steps = []
    text = scoped_highlight_tail(text, steps)
    return integer_slot_ternaries(text, steps), steps


def transform_title(method_source):
    return _title(method_source)[0]


def _team(text):
    steps = []
    return scoped_passer_tail(text, steps), steps


def transform_team(method_source):
    return _team(method_source)[0]


