"""Pure source calculations against FROZEN D25 only; UNKNOWN/not READY.
No production source binding and no model/compiler/AST/fixture/verifier.
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


def nearest_for(t, position, variable):
    for opening in reversed(frames(t, position)):
        if t[opening - 1].value != ')': continue
        parens = [i for i in range(opening - 1) if t[i].value == '(' and matching(t, i) == opening - 1]
        if len(parens) != 1: raise ValueError('ambiguous-control-parentheses')
        p = parens[0]
        if t[p - 1].value in ('for', 'while', 'foreach', 'lock', 'using'):
            if t[p - 1].value != 'for' or [z.value for z in t[p + 1:p + 4]] != vals('int ' + variable + ' ='):
                raise ValueError('nearest-loop-owner-not-proven')
            return opening, matching(t, opening)
    raise ValueError('loop-owner-not-found')


def switch_sections(t, selector):
    start, count = find(t, 'switch (' + selector + ')')
    opening = start + count
    if t[opening].value != '{': raise ValueError('switch-block-required')
    closing = matching(t, opening)
    labels = []
    i = opening + 1
    while i < closing:
        if t[i].value == '{': i = matching(t, i) + 1; continue
        if t[i].value in ('case', 'default'):
            if t[i].value == 'default': labels.append(('default', i, i + 2)); i += 2; continue
            if not t[i + 1].value.isdigit() or t[i + 2].value != ':': raise ValueError('literal-case-required')
            labels.append((int(t[i + 1].value), i, i + 3)); i += 3; continue
        i += 1
    result = {}
    for index, (value, label, first) in enumerate(labels):
        end = labels[index + 1][1] if index + 1 < len(labels) else closing
        if value in result: raise ValueError('duplicate-case')
        result[value] = (first, end)
    return opening, closing, labels, result


def execute_money_section(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    _, _, _, sections = switch_sections(t, 'num')
    first, end = sections[0]
    if [z.value for z in t[first:first + len(vals('if (scr[1] < 0)'))]] != vals('if (scr[1] < 0)'):
        raise ValueError('money-negative-first-guard-not-proven')
    a, b = body(t, first)
    if [z.value for z in t[a + 1:b]] != vals('SubMoney(-scr[1], 4); break;'):
        raise ValueError('money-true-body-not-proven')
    # Localized literal is preserved from source; match calls/arguments around it without inventing text.
    tt = t[b + 1:end]
    if [z.value for z in tt[:len(vals('AddMoney(scr[1], 4);'))]] != vals('AddMoney(scr[1], 4);'):
        raise ValueError('money-false-add-call-not-proven')
    if [z.value for z in tt[-2:]] != ['break', ';'] or [z.value for z in tt[len(vals('AddMoney(scr[1], 4);')):len(vals('AddMoney(scr[1], 4);')) + 2]] != ['AddPopupMessage', '(']:
        raise ValueError('money-false-popup-and-terminal-break-not-proven')
    if sum(z.value == ';' for z in tt) != 3: raise ValueError('money-false-tail-has-extra-statements')
    # Remove only the owned early break; retain the section's final break and its label.
    true_payload = text[t[a].end:t[b - 2].start]
    false_payload = text[t[b].end:t[end - 2].start]
    replacement = 'if (scr[1] < 0) {' + true_payload + '} else {' + false_payload + '}'
    return edit(text, t[first].start, t[end - 2].start, replacement,
                'case0-if-else-local-join-before-preserved-section-break', steps)


def execute_trade_section(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    _, _, _, sections = switch_sections(t, 'num')
    first, end = sections[17]
    if t[first].value != '{' or matching(t, first) != end - 1:
        raise ValueError('trade-section-explicit-block-required')
    start = first + 1
    head = vals('if (scr[1] == -1)')
    if [z.value for z in t[start:start + len(head)]] != head: raise ValueError('trade-negative-id-guard-not-proven')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('break;'): raise ValueError('trade-guard-must-only-break')
    final = end - 3
    if [z.value for z in t[final:end - 1]] != ['break', ';']: raise ValueError('trade-final-owned-break-required')
    decl = vals('TradeData tradeData2 = tradeData_[scr[1]];')
    if [z.value for z in t[b + 1:b + 1 + len(decl)]] != decl:
        raise ValueError('trade-local-leaf-not-proven')
    payload = text[t[b].end:t[final].start]
    return edit(text, t[start].start, t[final].start,
                'if (scr[1] != -1) {' + payload + '}',
                'case17-positive-trade-body-before-preserved-section-break', steps)


def _execute(text):
    steps = []
    text = execute_money_section(text, steps)
    return execute_trade_section(text, steps), steps


def transform_execute_script(method_source):
    return _execute(method_source)[0]


def _if_call_hits(t, call, negative):
    hits = []
    for i in range(len(t) - 3):
        if t[i].value != 'if' or t[i + 1].value != '(': continue
        end = matching(t, i + 1)
        expr = [z.value for z in t[i + 2:end]]
        wanted = vals(('!(' if negative else '') + call + '(')
        if expr[:len(wanted)] == wanted: hits.append(i)
    return hits


def coat_first_intermediate_join(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    hits = _if_call_hits(t, 'GameUtil.ChRectHitRect', True)
    if len(hits) != 1: raise ValueError('one-negative-empty-rectangle-arm-required')
    start = hits[0]
    a, b = body(t, start)
    nearest_for(t, start, 'j')
    if b != a + 1 or [z.value for z in t[b + 1:b + 3]] != ['else', '{']:
        raise ValueError('rectangle-empty-else-shape-required')
    other = b + 2
    other_end = matching(t, other)
    if [z.value for z in t[other_end + 1:other_end + 3]] != ['continue', ';']:
        raise ValueError('existing-rectangle-tail-continue-required')
    if [z.value for z in t[other + 1:other + 3]] != ['apdat_', '.']:
        raise ValueError('rectangle-insert-leaf-not-proven')
    condition = text[t[start].start:t[a].start]
    payload = text[t[other].start:t[other_end].end]
    return edit(text, t[start].start, t[other_end].end, condition + '{ continue; }' + payload,
                'rectangle-direct-continue-bypass-intermediate-case-join', steps)


def coat_positive_guards(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    hits = _if_call_hits(t, 'GameUtil.ChRectHitRect', False)
    if len(hits) != 2: raise ValueError('exact-two-positive-rectangle-guards-required')
    changes = []
    for start in hits:
        a, b = body(t, start)
        nearest_for(t, start, 'j')
        condition = text[t[start + 1].end:t[a - 1].start]
        payload = text[t[a].start:t[b].end]
        changes.append((t[start].start, t[b].end, 'if (!(' + condition + ')) { continue; }' + payload))
    for a, b, replacement in reversed(changes):
        text = edit(text, a, b, replacement, 'rectangle-explicit-continue-preserve-payload-block', steps)
    return text


def _coat(text):
    steps = []
    text = coat_first_intermediate_join(text, steps)
    return coat_positive_guards(text, steps), steps


def transform_coat(method_source):
    return _coat(method_source)[0]


def _check_zsort_owner(t, at):
    opening, closing, labels, sections = switch_sections(t, 'num2')
    if [x[0] for x in labels] != [1, 2, 3, 8, 9, 4, 5, 6, 7, 10]: raise ValueError('zsort-ordered-case-inventory-required')
    scope = frames(t, at)
    if scope[-1] != opening: raise ValueError('zsort-bit-guard-must-be-direct-section-command')
    _, loop_end = nearest_for(t, opening, 'j')
    if loop_end != closing + 1: raise ValueError('zsort-switch-must-exhaust-inner-loop-tail')
    return sections


def zsort_first_intermediate_join(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (!(BitUtil.Check(typeBits, 1 << num2)))')
    a, b = body(t, start)
    sections = _check_zsort_owner(t, start)
    if sections[1][0] != start: raise ValueError('zsort-negative-empty-arm-not-first-case1-command')
    if b != a + 1 or [z.value for z in t[b + 1:b + 3]] != ['else', '{']:
        raise ValueError('zsort-empty-else-shape-required')
    other = b + 2
    other_end = matching(t, other)
    if [z.value for z in t[other_end + 1:sections[1][1]]] != ['break', ';']:
        raise ValueError('zsort-preserved-case1-terminal-break-required')
    condition = text[t[start].start:t[a].start]
    payload = text[t[other].start:t[other_end].end]
    return edit(text, t[start].start, t[other_end].end, condition + '{ continue; }' + payload,
                'zsort-case1-direct-loop-continue-bypass-intermediate-section-break', steps)


def zsort_positive_guards(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    needle = vals('if (BitUtil.Check(typeBits, 1 << num2))')
    hits = [i for i in range(len(t) - len(needle) + 1) if [z.value for z in t[i:i + len(needle)]] == needle]
    if len(hits) != 8: raise ValueError('exact-eight-zsort-positive-guards-required')
    changes = []
    for start in hits:
        a, b = body(t, start)
        sections = _check_zsort_owner(t, start)
        section = next((k for k, (first, end) in sections.items() if first == start), None)
        if section not in [2, 3, 8, 4, 5, 6, 7, 10]: raise ValueError('unexpected-zsort-positive-case')
        if [z.value for z in t[b + 1:sections[section][1]]] != ['break', ';']:
            raise ValueError('zsort-positive-payload-must-precede-terminal-break')
        payload = text[t[a].start:t[b].end]
        changes.append((t[start].start, t[b].end, 'if (!BitUtil.Check(typeBits, 1 << num2)) { continue; }' + payload))
    for a, b, replacement in reversed(changes):
        text = edit(text, a, b, replacement, 'zsort-explicit-continue-preserve-child-declaration-block', steps)
    return text


def _zsort(text):
    steps = []
    text = zsort_first_intermediate_join(text, steps)
    return zsort_positive_guards(text, steps), steps


def transform_zsort(method_source):
    return _zsort(method_source)[0]


