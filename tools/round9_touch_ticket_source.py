"""Pure D29 brfalse composition calculations, UNKNOWN, no I/O during APIs.
"""
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


def enclosing_for(t, position, variable):
    for opening in reversed(frames(t, position)):
        if t[opening - 1].value != ')':
            continue
        candidates = [i for i in range(opening - 1) if t[i].value == '(' and matching(t, i) == opening - 1]
        if len(candidates) != 1:
            raise ValueError('ambiguous-parenthesized-owner')
        p = candidates[0]
        if t[p - 1].value in ('for', 'while', 'foreach'):
            if t[p - 1].value != 'for' or [z.value for z in t[p + 1:p + 5]] != vals('int ' + variable + ' = 0'):
                raise ValueError('unexpected-nearest-loop')
            return opening, matching(t, opening)
    raise ValueError('required-for-owner-absent')


def touch_array_positive_tail(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (array == null)')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('goto dispatchExit;'):
        raise ValueError('array-guard-goto-only-required')
    owner = frames(t, start)[-1]
    owner_end = matching(t, owner)
    expected = vals('if ((id == 12) || (id == 14) || (id == 15))')
    if [z.value for z in t[owner - len(expected):owner]] != expected:
        raise ValueError('array-guard-must-belong-to-scroll-id-arm')
    if [z.value for z in t[owner_end + 1:owner_end + 4]] != vals('dispatchExit:;'):
        raise ValueError('dispatch-label-not-next-sibling-after-scroll-arm')
    tail = text[t[b].end:t[owner_end].start]
    last = vals('h = array[1]; }')
    if not vals(tail) or vals(tail)[-len(last):] != last:
        raise ValueError('array-tail-last-false-branch-not-proven')
    return edit(text, t[start].start, t[owner_end].start,
                'if (array != null) {' + tail + '}',
                'touch-array-positive-tail-before-owned-dispatch-exit', steps)


def touch_component_continue(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (component.id == 2 || component.id == 3)')
    a, b = body(t, start)
    loop, loop_end = enclosing_for(t, start, 'i')
    if b + 1 != loop_end:
        raise ValueError('component-payload-not-terminal-in-for-i')
    declaration = vals('int num26 = value & 0xFFFF;')
    if vals(text[t[a].end:t[b].start])[:len(declaration)] != declaration:
        raise ValueError('component-payload-local-anchor-missing')
    payload = text[t[a].start:t[b].end]
    return edit(text, t[start].start, t[b].end,
                'if (component.id != 2 && component.id != 3) { continue; } ' + payload,
                'touch-component-two-exclusion-tests-owned-continue-preserve-payload-block', steps)


def touch_scale_ternary(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, size = find(t, '(g == null) ? GetScaleRatio() : g.GetScale()')
    return edit(text, t[start].start, t[start + size - 1].end,
                '(g != null) ? g.GetScale() : GetScaleRatio()',
                'touch-scale-reference-polarity-preserve-call-arms', steps)


def touch_implicit_shift_masks(text, steps=None):
    steps = [] if steps is None else steps
    for before, after in [('1 << (id & 0x1F)', '1 << id'),
                          ('1 << (component2.id & 0x1F)', '1 << component2.id')]:
        t = lex(text)
        start, size = find(t, before)
        text = edit(text, t[start].start, t[start + size - 1].end, after,
                    'touch-remove-explicit-Int32-shift-mask-already-inserted-by-smcs', steps)
    return text


def touch_separate_collision_guard(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    bits = '((num32 & (1 << touchComponent.id)) == 0 && (num31 & (1 << component2.id)) == 0)'
    priority = 'touchComponent.GetPriority() != component2.GetPriority()'
    hit = 'CheckHitRect(touchComponent.x, touchComponent.y, touchComponent.width, touchComponent.height, component2.x, component2.y, component2.width, component2.height)'
    start, _ = find(t, 'if (' + bits + ' || ' + priority + ' || !' + hit + ')')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('continue;'):
        raise ValueError('collision-guard-must-only-continue')
    enclosing_for(t, start, 'j')
    if [z.value for z in t[b + 1:b + 5]] != vals('int num33 = touchComponent'):
        raise ValueError('collision-tail-local-anchor-missing')
    replacement = ('if (' + bits + ') { continue; } '
                   'if (' + priority + ' || !' + hit + ') { continue; }')
    return edit(text, t[start].start, t[b].end, replacement,
                'touch-separate-collision-bits-continue-from-priority-hit-continue-same-for-j', steps)


def _touch(text):
    steps = []
    for fn in [touch_array_positive_tail, touch_component_continue, touch_scale_ternary,
               touch_implicit_shift_masks, touch_separate_collision_guard]:
        text = fn(text, steps)
    return text, steps


def transform_touch(method_source):
    return _touch(method_source)[0]


def _billing_empty(text):
    t = lex(text)
    start, _ = find(t, 'else if (!text.Equals("ticket"))')
    a, b = body(t, start + 1)
    if b != a + 1:
        raise ValueError('ticket-arm-must-be-empty')
    if t[b + 1].value == 'else':
        raise ValueError('ticket-arm-already-has-else')
    suffix = [z.value for z in t[b + 1:]]
    expected = [vals('base.Provisioning(task); }'), vals('base.Cancel(task); }')]
    if suffix not in expected:
        raise ValueError('ticket-arm-not-followed-by-unchanged-base-call-and-method-end')
    steps = []
    result = edit(text, t[start].start, t[b].end, 'else if (text.Equals("ticket")) { }',
                  'billing-positive-empty-ticket-test-without-extra-empty-else', steps)
    return result, steps


def transform_billing_empty_ticket(method_source):
    return _billing_empty(method_source)[0]

