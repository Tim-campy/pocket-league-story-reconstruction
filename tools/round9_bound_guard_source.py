"""Pure D27 calculations. No I/O in API/import; UNKNOWN until independent tests.
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


def frames(t, position):
    result = []
    for i in range(position):
        if t[i].value == '{': result.append(i)
        elif t[i].value == '}': result.pop()
    return result


def owner_condition(t, opening, keyword, expression):
    if t[opening - 1].value != ')': raise ValueError('control-parentheses-required')
    pairs = [i for i in range(opening - 1) if t[i].value == '(' and matching(t, i) == opening - 1]
    if len(pairs) != 1 or t[pairs[0] - 1].value != keyword or [z.value for z in t[pairs[0] + 1:opening - 1]] != vals(expression):
        raise ValueError('lexical-control-owner-not-proven')


def nearest_for_i(t, at):
    for opening in reversed(frames(t, at)):
        if t[opening - 1].value != ')': continue
        pairs = [i for i in range(opening - 1) if t[i].value == '(' and matching(t, i) == opening - 1]
        if len(pairs) != 1: raise ValueError('ambiguous-control-owner')
        p = pairs[0]
        if t[p - 1].value in ('for', 'foreach', 'while'):
            if t[p - 1].value != 'for' or [z.value for z in t[p + 1:p + 5]] != vals('int i = 0'):
                raise ValueError('nearest-for-i-owner-not-proven')
            return opening, matching(t, opening)
    raise ValueError('loop-owner-missing')


def _map(text):
    steps = []
    t = lex(text)
    start, _ = find(t, 'if (num < 0 || num > MAPCHIP_COAT_WNUM || num2 < -1 || num2 > MAPCHIP_COAT_HNUM)')
    a, b = body(t, start)
    if len(frames(t, start)) != 1 or [z.value for z in t[a + 1:b]] != vals('return;'):
        raise ValueError('map-root-return-guard-not-proven')
    replacement = ('if (num < 0) { return; }\r\n'
                   'if (num > MAPCHIP_COAT_WNUM) { return; }\r\n'
                   'if (num2 < -1) { return; }\r\n'
                   'if (num2 > MAPCHIP_COAT_HNUM) { return; }')
    return edit(text, t[start].start, t[b].end, replacement,
                'four-signed-int32-bounds-with-four-original-return-sites', steps), steps


def transform_coat_line(method_source):
    return _map(method_source)[0]


def effect_type3_continue(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (array[1] >= 0)')
    a, b = body(t, start)
    parent = frames(t, start)[-1]
    owner_condition(t, parent, 'if', 'array[0] == 3')
    nearest_for_i(t, start)
    if parent + 1 != start or matching(t, parent) != b + 1:
        raise ValueError('effect-type3-positive-block-not-only-tail')
    if [z.value for z in t[a + 1:a + 4]] != vals('TPlayer tPlayer2 ='):
        raise ValueError('effect-local-declaration-anchor-not-proven')
    # Retain the original body as a standalone block: preserve its child locals.
    payload = text[t[a].start:t[b].end]
    return edit(text, t[start].start, t[b].end, 'if (array[1] < 0) { continue; }' + payload,
                'type3-negative-frame-continue-preserve-child-local-block', steps)


def effect_type6_branch(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (array[0] != 6)')
    a, b = body(t, start)
    parent = frames(t, start)[-1]
    if parent + 1 != start or t[parent - 1].value != 'else' or [z.value for z in t[a + 1:b]] != vals('continue;'):
        raise ValueError('effect-type6-negative-first-else-guard-not-proven')
    _, loop_end = nearest_for_i(t, start)
    if matching(t, parent) + 1 != loop_end:
        raise ValueError('effect-type6-block-not-terminal-loop-branch')
    next_head = vals('if (array[1] < 0)')
    if [z.value for z in t[b + 1:b + 1 + len(next_head)]] != next_head:
        raise ValueError('effect-type6-frame-guard-not-preserved')
    # Keep the existing else-body block; replace its prefix and delete only its first guard.
    return edit(text, t[parent - 1].start, t[b].end, 'else if (array[0] == 6) {',
                'type6-positive-else-if-preserve-negative-frame-guard-and-leaves', steps)


def _effect(text):
    steps = []
    text = effect_type3_continue(text, steps)
    return effect_type6_branch(text, steps), steps


def transform_effect(method_source):
    return _effect(method_source)[0]


def _lineup(text):
    steps = []
    t = lex(text)
    start, _ = find(t, 'if (select_ >= linups_.Length)')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('return Pop();'): raise ValueError('lineup-inner-pop-guard-required')
    outer = frames(t, start)[-1]
    owner_condition(t, outer, 'if', '(canvas_.CheckKeyPulse(1048576))')
    outer_end = matching(t, outer)
    if outer + 1 != start or [z.value for z in t[b + 1:outer_end]] != vals('Canvas.OpenBrowser(linups_[select_].GetURL());'):
        raise ValueError('lineup-inner-tail-call-not-proven')
    if [z.value for z in t[outer_end + 1:outer_end + 3]] != ['else', '{']:
        raise ValueError('lineup-old-external-empty-else-required')
    empty_open = outer_end + 2
    empty_end = matching(t, empty_open)
    if empty_end != empty_open + 1: raise ValueError('lineup-external-else-is-not-empty')
    # Remove only the outer empty else first. The old single reversal kept it and failed.
    text = edit(text, t[outer_end].end, t[empty_end].end, '',
                'remove-proven-empty-outer-keypulse-else-not-the-inner-return', steps)
    t = lex(text)
    start, _ = find(t, 'if (select_ >= linups_.Length)')
    a, b = body(t, start)
    outer = frames(t, start)[-1]
    outer_end = matching(t, outer)
    call = text[t[b].end:t[outer_end].start]
    ret = text[t[a].start:t[b].end]
    return edit(text, t[start].start, t[outer_end].start,
                'if (select_ < linups_.Length) {' + call + '} else ' + ret,
                'inner-positive-browser-arm-else-pop-with-external-join-removed', steps), steps


def transform_lineup_composed(method_source):
    """New TWO-edit composition; never offer the R8C02 single-edit variant."""
    return _lineup(method_source)[0]

