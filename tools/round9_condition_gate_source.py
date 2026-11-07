"""Pure D27 source condition compositions. UNKNOWN; no I/O in APIs.
No models, compilers, AST helpers, fixtures or verifier are invoked.
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


def billing_purchase_loop(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if ((items[j] == null) || (items[j].GetState() != 1))')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('continue;'): raise ValueError('billing-first-guard-only-continue-required')
    parent = frames(t, start)[-1]
    if parent + 1 != start or t[parent - 1].value != ')': raise ValueError('billing-loop-parent-not-proven')
    parens = [i for i in range(parent - 1) if t[i].value == '(' and matching(t, i) == parent - 1]
    if len(parens) != 1 or t[parens[0] - 1].value != 'for' or [z.value for z in t[parens[0] + 1:parent - 1]] != vals('int j = 0; j < items.Length; j++'):
        raise ValueError('billing-array-index-loop-not-proven')
    end = matching(t, parent)
    second = b + 1
    header = vals('if (items[j].GetPurchase() == null)')
    if [z.value for z in t[second:second + len(header)]] != header: raise ValueError('billing-second-guard-not-proven')
    sa, sb = body(t, second)
    if [z.value for z in t[sa + 1:sb]] != vals('continue;'): raise ValueError('billing-purchase-guard-only-continue-required')
    tail_header = vals('if (text2.Length() > 0)')
    tail_end = vals('num5++;')
    if [z.value for z in t[sb + 1:sb + 1 + len(tail_header)]] != tail_header or [z.value for z in t[end - len(tail_end):end]] != tail_end:
        raise ValueError('billing-tail-leaf-boundaries-not-proven')
    if any(z.value in ('break', 'continue', 'return', 'goto', 'delegate', '=>', 'yield') for z in t[sb + 1:end]):
        raise ValueError('billing-tail-nonlocal-transfer-unsupported')
    payload = text[t[sb].end:t[end].start]
    replacement = ('if (items[j] == null) { continue; }\r\n'
                   'if (items[j].GetState() == 1 && items[j].GetPurchase() != null) {' + payload + '}')
    return edit(text, t[start].start, t[end].start, replacement,
                'separate-null-continue-and-positive-state-purchase-tail', steps)


def billing_empty_type4(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'else if (type_ != 4 && type_ == 5)')
    a, b = body(t, start + 1)
    if [z.value for z in t[b + 1:]] != vals('return true; }'):
        raise ValueError('billing-final-common-true-return-not-proven')
    payload = text[t[a].start:t[b].end]
    return edit(text, t[start].start, t[b].end, 'else if (type_ == 4) { } else if (type_ == 5) ' + payload,
                'billing-empty-type4-before-preserved-type5-body', steps)


def _billing(text):
    steps = []
    text = billing_purchase_loop(text, steps)
    return billing_empty_type4(text, steps), steps


def transform_billing(method_source):
    return _billing(method_source)[0]


def collision_sequential_guards(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (num == 0 || p1.address_[0] - p2.address_[0] < -1 || p1.address_[0] - p2.address_[0] > 1 || p1.address_[1] - p2.address_[1] < -1 || p1.address_[1] - p2.address_[1] > 1)')
    a, b = body(t, start)
    if len(frames(t, start)) != 1 or [z.value for z in t[a + 1:b]] != vals('return;'):
        raise ValueError('collision-root-combined-return-guard-not-proven')
    replacement = ('if (num == 0) { return; }\r\n'
                   'if (p1.address_[0] - p2.address_[0] < -1 || p1.address_[0] - p2.address_[0] > 1) { return; }\r\n'
                   'if (p1.address_[1] - p2.address_[1] < -1 || p1.address_[1] - p2.address_[1] > 1) { return; }')
    return edit(text, t[start].start, t[b].end, replacement, 'three-sequential-root-int32-return-guards', steps)


def collision_float_stores(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, count = find(t, 'num6 = ((!(num5 > num4)) ? 1 : 0);')
    return edit(text, t[start].start, t[start + count - 1].end,
                'if (num5 > num4) { num6 = 0; } else { num6 = 1; }',
                'positive-float-gt-per-arm-store-preserve-unordered-false', steps)


def collision_positive_state_tail(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (state_ != 0 || (num != 2 && num != 3 && num != 4))')
    a, b = body(t, start)
    roots = frames(t, start)
    if len(roots) != 1 or [z.value for z in t[a + 1:b]] != vals('return;'):
        raise ValueError('collision-state-root-guard-not-proven')
    end = matching(t, roots[0])
    if t[b + 1].value != 'if' or [z.value for z in t[b + 1:b + 1 + len(vals('if (Player.ChBodyHitBody(p1, p2))'))]] != vals('if (Player.ChBodyHitBody(p1, p2))'):
        raise ValueError('collision-second-phase-body-not-proven')
    payload = text[t[b].end:t[end].start]
    return edit(text, t[start].start, t[end].start,
                'if (state_ == 0 && (num == 2 || num == 3 || num == 4)) {' + payload + '}',
                'positive-state-and-kind-gate-second-phase-to-common-return', steps)


def collision_positive_rectangle_tail(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    hits = []
    for i in range(len(t) - 2):
        if [z.value for z in t[i:i + len(vals('if (!GameUtil.ChRectHitRect('))]] == vals('if (!GameUtil.ChRectHitRect('): hits.append(i)
    if len(hits) != 1: raise ValueError('one-negative-rectangle-call-guard-required')
    start = hits[0]
    a, b = body(t, start)
    roots = frames(t, start)
    if len(roots) != 1 or [z.value for z in t[a + 1:b]] != vals('return;'):
        raise ValueError('collision-rectangle-root-guard-not-proven')
    end = matching(t, roots[0])
    condition_end = matching(t, start + 1)
    # Remove only ! before a Boolean call; never complement a floating comparison.
    positive = text[t[start + 3].start:t[condition_end].start]
    payload = text[t[b].end:t[end].start]
    return edit(text, t[start].start, t[end].start, 'if (' + positive + ') {' + payload + '}',
                'positive-rectangle-call-gate-rest-to-common-return', steps)


def _collision(text):
    steps = []
    for fn in [collision_sequential_guards, collision_float_stores,
               collision_positive_state_tail, collision_positive_rectangle_tail]:
        text = fn(text, steps)
    return text, steps


def transform_collision(method_source):
    return _collision(method_source)[0]


def transform_common_query(declaration_source):
    """Only the field/local initializer expression. Never synthesizes a .cctor."""
    t = lex(declaration_source)
    start, count = find(t, '(Config.APPLI_TYPE != 0) ? "uid=1&sid=PL91" : "uid=NULLGWDOCOMO"')
    replacement = '(Config.APPLI_TYPE == 0) ? "uid=NULLGWDOCOMO" : "uid=1&sid=PL91"'
    return declaration_source[:t[start].start] + replacement + declaration_source[t[start + count - 1].end:]


