"""Pure complete ten-hunk SubForm draw composition; UNKNOWN, not READY.
No file I/O during import or API calls; no compiler/model/AST/verifier.
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


def edit(text, start, end, replacement, component, steps):
    steps.append({'component': component,
                  'start_utf16_in_step_input': len(text[:start].encode('utf-16-le')) // 2,
                  'end_utf16_in_step_input': len(text[:end].encode('utf-16-le')) // 2,
                  'before': text[start:end], 'after': replacement})
    return text[:start] + replacement + text[end:]


def frames_before(t, position):
    frames = []
    for i in range(position):
        if t[i].value == '{': frames.append(i)
        elif t[i].value == '}': frames.pop()
    return frames


def if_chain_end(t, start):
    if t[start].value == '{': return matching(t, start) + 1
    if t[start].value != 'if': raise ValueError('only-braced-if-chain-supported')
    a, b = body(t, start)
    end = b + 1
    if end < len(t) and t[end].value == 'else': end = if_chain_end(t, end + 1)
    return end


def terminal_else_ancestry(t, position):
    frames = frames_before(t, position)
    if not frames: raise ValueError('method-body-required')
    # Every ancestor below the method body is an else-block, never a loop/EH.
    if any(t[frame - 1].value != 'else' for frame in frames[1:]):
        raise ValueError('non-else-ancestor-ownership-unsupported')
    parent = frames[-1]
    end = matching(t, parent)
    if any(z.value != '}' for z in t[end + 1:]):
        raise ValueError('ancestor-not-terminal-to-method')
    return parent, end


def window_empty_arm(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (!IsDrawWindow())')
    a, b = body(t, start)
    if len(frames_before(t, start)) != 1: raise ValueError('window-guard-must-be-root-command')
    payload = text[t[a].start:t[b].end]
    return edit(text, t[start].start, t[b].end,
                'if (IsDrawWindow()) { } else ' + payload,
                'root-window-positive-empty-arm-preserve-legitimate-early-return-leaves', steps)


def null_button_continues(text, steps=None):
    steps = [] if steps is None else steps
    for variable in ['i', 'j']:
        t = lex(text)
        start, _ = find(t, 'if (selectButtons_[' + variable + '] != null)')
        a, b = body(t, start)
        frames = frames_before(t, start)
        parent = frames[-1]
        if parent + 1 != start or matching(t, parent) != b + 1 or t[parent - 1].value != ')':
            raise ValueError('button-if-must-be-only-for-command')
        parens = [i for i in range(parent - 1) if t[i].value == '(' and matching(t, i) == parent - 1]
        if len(parens) != 1 or t[parens[0] - 1].value != 'for': raise ValueError('button-continue-owner-not-for')
        if [z.value for z in t[parens[0] + 1:parens[0] + 5]] != vals('int ' + variable + ' = 0'):
            raise ValueError('button-for-identity-not-proven')
        if any(z.value in ('delegate', '=>', 'goto', 'yield') for z in t[a + 1:b]):
            raise ValueError('captured-or-nonlocal-control-body-unsupported')
        payload = text[t[a].end:t[b].start]
        replacement = 'if (selectButtons_[' + variable + '] == null) { continue; }' + payload
        text = edit(text, t[start].start, t[b].end, replacement,
                    'owner-proven-null-button-continue-' + variable, steps)
    return text


def empty_type_arms(text, steps=None):
    steps = [] if steps is None else steps
    for empty, following in [(2, 3), (18, 19), (20, 23)]:
        t = lex(text)
        start, _ = find(t, 'if (type_ == ' + str(empty) + ')')
        a, b = body(t, start)
        if [z.value for z in t[a + 1:b]] != vals('return;'): raise ValueError('empty-type-guard-not-only-return')
        parent, parent_end = terminal_else_ancestry(t, start)
        if start != parent + 1 or t[parent - 1].value != 'else': raise ValueError('empty-type-not-first-else-command')
        next_start = b + 1
        header = vals('if (type_ == ' + str(following) + ')')
        if [z.value for z in t[next_start:next_start + len(header)]] != header:
            raise ValueError('empty-type-following-arm-not-proven')
        if if_chain_end(t, next_start) != parent_end: raise ValueError('following-chain-not-exhaustive-parent-tail')
        text = edit(text, t[start].start, t[next_start].start,
                    'if (type_ == ' + str(empty) + ') { } else ',
                    'terminal-type-empty-arm-' + str(empty) + '-join-next-' + str(following), steps)
    return text


def two_case_dispatch(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, length = find(t, 'switch (num10)')
    opening = start + length
    if t[opening].value != '{': raise ValueError('dispatch-block-required')
    closing = matching(t, opening)
    labels = []
    i = opening + 1
    while i < closing:
        if t[i].value == '{': i = matching(t, i) + 1; continue
        if t[i].value == 'case':
            if not t[i + 1].value.isdigit() or t[i + 2].value != ':': raise ValueError('integer-case-required')
            labels.append((int(t[i + 1].value), i, i + 3)); i += 3; continue
        if t[i].value in ('default', 'goto'): raise ValueError('dispatch-default-or-goto-unsupported')
        i += 1
    if [label[0] for label in labels] != [0, 1]: raise ValueError('exact-ordered-two-cases-required')
    arms = []
    for index, (_, _, begin) in enumerate(labels):
        end = labels[index + 1][1] if index == 0 else closing
        if [z.value for z in t[end - 2:end]] != ['break', ';']: raise ValueError('owned-terminal-case-break-required')
        # Target has no case-local declaration or nested transfer: keep this restriction explicit.
        if any(z.value in ('break', 'continue', 'return', 'goto', 'delegate', '=>', 'yield') for z in t[begin:end - 2]):
            raise ValueError('two-case-internal-control-transfer-unsupported')
        payload = text[t[begin].start:t[end - 2].start]
        arms.append('{' + payload + '}')
    replacement = 'if (num10 == 0) ' + arms[0] + ' else if (num10 == 1) ' + arms[1]
    return edit(text, t[start].start, t[closing].end, replacement,
                'named-num10-direct-zero-one-tests-without-switch-cache', steps)


def final_type29_empty_arm(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'else if (type_ != 29)')
    a, b = body(t, start + 1)
    terminal_else_ancestry(t, start)
    if any(z.value != '}' for z in t[b + 1:]): raise ValueError('type29-not-final-arm-to-method')
    if [z.value for z in t[a + 1:b]] != vals('_draw2(g);'): raise ValueError('final-draw2-leaf-not-proven')
    payload = text[t[a].start:t[b].end]
    return edit(text, t[start].start, t[b].end, 'else if (type_ == 29) { } else ' + payload,
                'final-type29-empty-arm-before-preserved-draw2-else', steps)


def _transform(text):
    if any(z.value in ('delegate', '=>', 'yield') for z in lex(text)):
        raise ValueError('capturing-or-state-machine-method-unsupported')
    steps = []
    for fn in [window_empty_arm, null_button_continues, empty_type_arms, two_case_dispatch, final_type29_empty_arm]:
        text = fn(text, steps)
    return text, steps


def transform_draw(method_source):
    """Pure header-independent whole-method composition; no model or I/O."""
    return _transform(method_source)[0]


