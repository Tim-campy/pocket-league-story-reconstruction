"""Pure source-only compositions. No emit/model/AST/verifier/production writes."""
from pathlib import Path
import hashlib
import importlib.util
import json
import sys

import round9_terminal_empty_arms_source as pure
lex, matching, inventory = pure.lex, pure.matching, pure.inventory


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


def splice(text, start, end, replacement, label, steps):
    before = text[start:end]
    if '//' in before or '/*' in before:
        raise ValueError('comment-in-edited-control-span:' + label)
    steps.append({'label': label,
                  'start_utf16_in_step_input': len(text[:start].encode('utf-16-le')) // 2,
                  'end_utf16_in_step_input': len(text[:end].encode('utf-16-le')) // 2,
                  'before': before, 'after': replacement})
    return text[:start] + replacement + text[end:]


def _subform(text):
    steps = []
    t = lex(text)
    needle = 'if ((type_ != 61 || (tenantData.id_ != TenantData.TENANT_ID_STADIUM && tenantData.id_ != TenantData.TENANT_ID_TRAININGAREA && tenantData.id_ != TenantData.TENANT_ID_TRAININGAREA_SPRED)) && tenantData.state_ != 0)'
    start, _ = find(t, needle)
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('ary1.AddElement(tenantData);'):
        raise ValueError('tenant-leaf-not-proven')
    # The nearest lexical block must be a for body, ending immediately after this if.
    frames = []
    for i in range(start):
        if t[i].value == '{': frames.append(i)
        elif t[i].value == '}': frames.pop()
    parent = frames[-1]
    if t[parent - 1].value != ')':
        raise ValueError('tenant-parent-not-loop')
    opens = [i for i in range(parent - 1) if t[i].value == '(' and matching(t, i) == parent - 1]
    if len(opens) != 1 or t[opens[0] - 1].value != 'for':
        raise ValueError('tenant-parent-not-for')
    if matching(t, parent) != b + 1:
        raise ValueError('tenant-tail-ownership-unsupported')
    then = text[t[a].start:t[b].end]
    replacement = ('if (type_ == 61) {\r\n'
        '\t\t\t\t\t\t\t\t\t\t\t\t\tif (tenantData.id_ == TenantData.TENANT_ID_STADIUM) { continue; }\r\n'
        '\t\t\t\t\t\t\t\t\t\t\t\t\tif (tenantData.id_ == TenantData.TENANT_ID_TRAININGAREA) { continue; }\r\n'
        '\t\t\t\t\t\t\t\t\t\t\t\t\tif (tenantData.id_ == TenantData.TENANT_ID_TRAININGAREA_SPRED) { continue; }\r\n'
        '\t\t\t\t\t\t\t\t\t\t\t\t}\r\n'
        '\t\t\t\t\t\t\t\t\t\t\t\tif (tenantData.state_ != 0) ' + then)
    text = splice(text, t[start].start, t[b].end, replacement, 'three-explicit-tenant-continues-preserve-state-positive-body', steps)

    t = lex(text)
    start, _ = find(t, 'else if (type_ != 62 && type_ != 63)')
    outer_a, outer_b = body(t, start + 1)
    inner = outer_a + 1
    guard, _ = find(t[inner:outer_b], 'if (type_ != 64)')
    if guard != 0:
        raise ValueError('type64-guard-must-be-first')
    guard_a, guard_b = body(t, inner)
    if [z.value for z in t[guard_a + 1:guard_b]] != vals('return Init2();'):
        raise ValueError('type64-guard-leaf-not-proven')
    remaining = t[guard_b + 1:outer_b]
    expected_prefix = vals('TenantData tenantData3 = apdat_.tenantData_[tenantDataId_]; int[] lvUpAddParam = tenantData3.GetLvUpAddParam();')
    if [z.value for z in remaining[:len(expected_prefix)]] != expected_prefix:
        raise ValueError('type64-declarations-not-proven')
    rest = [z.value for z in t[outer_b + 1:]]
    first_not_close = next((i for i, v in enumerate(rest) if v != '}'), None)
    if first_not_close is None or rest[first_not_close:] != ['return', 'true', ';', '}']:
        raise ValueError('type64-tail-not-method-common-true-return')
    preserved_body = text[t[guard_b].end:t[outer_b].start]
    return_leaf = text[t[guard_a].end:t[guard_b].start]
    replacement = ('else if (type_ == 62) { }\r\n'
        '\t\t\t\t\t\t\t\t\t\telse if (type_ == 63) { }\r\n'
        '\t\t\t\t\t\t\t\t\t\telse if (type_ == 64) {' + preserved_body + '}\r\n'
        '\t\t\t\t\t\t\t\t\t\telse {' + return_leaf + '}')
    text = splice(text, t[start].start, t[outer_b].end, replacement, 'empty62-empty63-positive64-else-init2-preserve-local-block', steps)
    return text, steps


def transform_subform(method_source):
    return _subform(method_source)[0]


def _ball(text):
    steps = []
    # First edit the inner body, so the later whole-arm swap cannot overlap edits.
    t = lex(text)
    start, _ = find(t, 'if (!(lineOutPos_.z_ > 900f))')
    a, b = body(t, start)
    if b != a + 1 or [z.value for z in t[b + 1:b + 3]] != ['else', '{']:
        raise ValueError('ball-inner-arm-not-empty-else')
    else_a, else_b = b + 2, matching(t, b + 2)
    if [z.value for z in t[else_a + 1:else_b]] != vals('num3 = 1;'):
        raise ValueError('ball-inner-positive-leaf-not-proven')
    positive = text[t[else_a].start:t[else_b].end]
    text = splice(text, t[start].start, t[else_b].end,
                  'if (lineOutPos_.z_ > 900f) ' + positive,
                  'remove-empty-negated-float-arm-preserve-positive-gt', steps)

    t = lex(text)
    start, _ = find(t, 'else if ((!(lineOutPos_.z_ < -900f)) && (!(lineOutPos_.z_ > 900f)))')
    a, b = body(t, start + 1)
    if [z.value for z in t[a + 1:b]] != vals('udat_.gm_.ChangeState(3);'):
        raise ValueError('ball-in-range-leaf-not-proven')
    if [z.value for z in t[b + 1:b + 3]] != ['else', '{']:
        raise ValueError('ball-out-of-range-arm-missing')
    other_a, other_b = b + 2, matching(t, b + 2)
    outside = text[t[other_a].start:t[other_b].end]
    inside = text[t[a].start:t[b].end]
    text = splice(text, t[start].start, t[other_b].end,
                  'else if (lineOutPos_.z_ < -900f || lineOutPos_.z_ > 900f) ' + outside + ' else ' + inside,
                  'positive-float-outside-arm-before-in-range-else-no-operator-negation', steps)

    t = lex(text)
    start, _ = find(t, 'if (flag13 && !flag8 && SoccerForm.DEBUG_BALLTEST_MODE != 1)')
    a, b = body(t, start)
    if t[a + 1].value != 'if':
        raise ValueError('ball-wall-if-missing')
    wall_start = a + 1
    if [z.value for z in t[wall_start:wall_start + len(vals('if (SoccerForm.DEBUG_WALLREFRECT == 1)'))]] != vals('if (SoccerForm.DEBUG_WALLREFRECT == 1)'):
        raise ValueError('ball-wall-first-arm-not-proven')
    if pure._statement_end(t, wall_start) != b:
        raise ValueError('ball-wall-chain-does-not-exhaust-debug-body')
    # This block is not a loop; its first statement is the same preserved wall chain.
    body_text = text[t[a].end:t[b].start]
    replacement = ('if (flag13 && !flag8) {\r\n'
                   '\t\t\t\t\tif (SoccerForm.DEBUG_BALLTEST_MODE == 1) { }\r\n'
                   '\t\t\t\t\telse ' + body_text.lstrip() + '}')
    text = splice(text, t[start].start, t[b].end, replacement,
                  'explicit-empty-debug-arm-only-suffix-preserve-flag13-notflag8-prefix', steps)
    return text, steps


def transform_ball(method_source):
    return _ball(method_source)[0]


def extract(source, marker):
    if source.count(marker) != 1:
        raise ValueError('method-marker-not-unique')
    start = source.index(marker)
    t = lex(source[start:])
    root = next(i for i, z in enumerate(t) if z.value == '{')
    end = start + t[matching(t, root)].end
    return start, end, source[start:end]


