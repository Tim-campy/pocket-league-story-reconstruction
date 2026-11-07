"""Pure C# text compositions, UNKNOWN until independent model/compiler/oracle.

No production writes, semantic model, AST helper, compiler or verifier invocation.
The component APIs accept isolated method declarations with arbitrary headers.
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



def replace(text, begin, end, value, label, steps):
    old = text[begin:end]
    # Comments are conservatively unsupported; literals in untouched leaves stay raw.
    steps.append({'component': label, 'start_utf16_in_step_input': len(text[:begin].encode('utf-16-le')) // 2,
                  'end_utf16_in_step_input': len(text[:end].encode('utf-16-le')) // 2,
                  'before': old, 'after': value})
    return text[:begin] + value + text[end:]


def statements_end(t, start):
    depth = []
    pairs = {'(': ')', '[': ']', '{': '}'}
    for i in range(start, len(t)):
        v = t[i].value
        if v in pairs: depth.append(pairs[v])
        elif v in (')', ']', '}'):
            if not depth or depth.pop() != v: raise ValueError('unbalanced-statement')
        elif v == ';' and not depth: return i
    raise ValueError('statement-end-not-found')


def _dispatch(text, expected_groups, which, steps):
    t = lex(text)
    hits = []
    needle = vals('switch (pass_mode)')
    for i in range(len(t) - len(needle) + 1):
        if [z.value for z in t[i:i + len(needle)]] == needle: hits.append(i)
    if which >= len(hits): raise ValueError('dispatch-not-found')
    at = hits[which]
    opening = matching(t, at + 1) + 1
    if t[opening].value != '{': raise ValueError('switch-explicit-body-required')
    closing = matching(t, opening)
    # Find only labels owned by this switch, ignoring all nested bodies.
    labels = []
    i = opening + 1
    while i < closing:
        if t[i].value == '{': i = matching(t, i) + 1; continue
        if t[i].value == 'case':
            if not t[i + 1].value.isdigit() or t[i + 2].value != ':': raise ValueError('noninteger-case')
            labels.append((int(t[i + 1].value), i, i + 3))
            i += 3; continue
        if t[i].value in ('default', 'goto'): raise ValueError('default-or-goto-unsupported')
        i += 1
    groups = []
    index = 0
    while index < len(labels):
        numbers = [labels[index][0]]
        code_start = labels[index][2]
        while index + 1 < len(labels) and code_start == labels[index + 1][1]:
            index += 1; numbers.append(labels[index][0]); code_start = labels[index][2]
        code_stop = labels[index + 1][1] if index + 1 < len(labels) else closing
        # Some sections use an explicit block; retain exactly that block.
        braced = t[code_start].value == '{'
        if braced:
            end = matching(t, code_start)
            if end != code_stop - 1: raise ValueError('section-has-extra-tail')
            first, last = code_start + 1, end
        else:
            first, last = code_start, code_stop
        if [z.value for z in t[last - 2:last]] != ['break', ';']:
            raise ValueError('section-terminal-owned-break-required')
        # Terminal section break is outside every child loop/if/try.
        j = first
        while j < last - 2:
            if t[j].value == '{': j = matching(t, j) + 1; continue
            if t[j].value in ('break', 'continue', 'goto', 'return', 'throw'):
                raise ValueError('section-direct-transfer-before-terminal-break')
            j += 1
        if braced:
            original = text[t[code_start].start:t[code_stop - 1].end]
            rel_a = t[last - 2].start - t[code_start].start
            rel_b = t[last - 1].end - t[code_start].start
            code = original[:rel_a] + original[rel_b:]
        else:
            code = '{' + text[t[first].start:t[last - 2].start] + '}'
        groups.append((numbers, code))
        index += 1
    if [g[0] for g in groups] != expected_groups: raise ValueError('dispatch-case-order-not-proven')
    rendered = []
    for i, (numbers, code) in enumerate(groups):
        condition = ' || '.join(f'pass_mode == {value}' for value in numbers)
        rendered.append(('if' if i == 0 else 'else if') + ' (' + condition + ') ' + code)
    return replace(text, t[at].start, t[closing].end, '\r\n\t\t\t'.join(rendered),
                   'dispatch-' + '-'.join(','.join(map(str, g)) for g in expected_groups), steps)


def switch_dispatches(method_source, steps=None):
    steps = [] if steps is None else steps
    text = _dispatch(method_source, [[0, 5], [1], [2], [3], [4]], 0, steps)
    return _dispatch(text, [[0, 5], [1], [3], [4]], 0, steps)


def loop_continue(method_source, steps=None):
    steps = [] if steps is None else steps
    t = lex(method_source)
    start, _ = find(t, 'if (i != player.p_id_)')
    opening, closing = body(t, start)
    frames = []
    for j in range(start):
        if t[j].value == '{': frames.append(j)
        elif t[j].value == '}': frames.pop()
    parent = frames[-1]
    if matching(t, parent) != closing + 1 or t[parent - 1].value != ')':
        raise ValueError('positive-if-must-exhaust-for-body')
    parens = [j for j in range(parent - 1) if t[j].value == '(' and matching(t, j) == parent - 1]
    if len(parens) != 1 or t[parens[0] - 1].value != 'for': raise ValueError('continue-owner-not-for')
    if [z.value for z in t[opening + 1:opening + 4]] != vals('Player player3 ='):
        raise ValueError('loop-declaration-anchor-not-proven')
    payload = method_source[t[opening].end:t[closing].start]
    return replace(method_source, t[start].start, t[closing].end,
                   'if (i == player.p_id_) { continue; }' + payload,
                   'owner-proven-loop-continue-unwrapping-positive-arm', steps)


def ternary_stores(method_source, steps=None):
    steps = [] if steps is None else steps
    t = lex(method_source)
    begin, _ = find(t, 'num3 = ((player3.team_id_ != 0) ?')
    end = statements_end(t, begin)
    questions = [i for i in range(begin, end) if t[i].value == '?']
    colons = [i for i in range(begin, end) if t[i].value == ':']
    if len(questions) != 1 or len(colons) != 1: raise ValueError('nested-ternary-unsupported')
    question, colon = questions[0], colons[0]
    true_open = question + 1
    false_open = colon + 1
    if t[true_open].value != '(' or matching(t, true_open) != colon - 1:
        raise ValueError('ternary-true-parenthesized-expression-required')
    false_close = matching(t, false_open)
    if t[false_open].value != '(' or [z.value for z in t[false_close + 1:end]] != [')']:
        raise ValueError('ternary-false-parenthesized-expression-required')
    yes = method_source[t[true_open].start:t[colon - 1].end]
    no = method_source[t[false_open].start:t[false_close].end]
    # Integer team comparison only; no floating relational operator is negated.
    replacement = 'if (player3.team_id_ == 0) { num3 = ' + no + '; } else { num3 = ' + yes + '; }'
    return replace(method_source, t[begin].start, t[end].end, replacement,
                   'integer-team-zero-first-and-store-each-ternary-arm', steps)


def root_declarations(method_source, steps=None):
    steps = [] if steps is None else steps
    t = lex(method_source)
    begin, count = find(t, 'int num4 = 2; float num5 = 1f; float num6 = 1f; float num7 =')
    rhs = begin + count
    end = statements_end(t, rhs)
    expr = method_source[t[rhs].start:t[end].start]
    replacement = 'float num7;\r\n\t\t\tint num4 = 2;\r\n\t\t\tfloat num5 = 1f;\r\n\t\t\tfloat num6 = 1f;\r\n\t\t\tnum7 = ' + expr + ';'
    return replace(method_source, t[begin].start, t[end].end, replacement,
                   'bare-num7-before-num4-5-6-preserve-initialization-order', steps)


def split_field_initializers(method_source, steps=None):
    steps = [] if steps is None else steps
    text = method_source
    for name in ['num15', 'num16', 'num14']:
        t = lex(text)
        begin, count = find(t, 'int ' + name + ' = (targetPId =')
        rhs = begin + count
        end = statements_end(t, begin)
        if t[end - 1].value != ')': raise ValueError('assignment-value-shape-not-proven')
        expression = text[t[rhs].start:t[end - 1].start]
        replacement = 'int ' + name + ' = ' + expression + '; targetPId = ' + name + ';'
        text = replace(text, t[begin].start, t[end].end, replacement,
                       'local-initialization-before-field-' + name, steps)
    return text


def known_divisions(method_source, steps=None):
    steps = [] if steps is None else steps
    text = method_source
    for field in ['x_', 'z_']:
        t = lex(text)
        begin, count = find(t, 'v_.' + field + ' /= num8;')
        text = replace(text, t[begin].start, t[begin + count - 1].end,
                       'v_.' + field + ' = v_.' + field + ' / num8;',
                       'existing-d10-explicit-field-division-' + field, steps)
    return text


def _setpass(text):
    steps = []
    text = switch_dispatches(text, steps)
    text = loop_continue(text, steps)
    text = ternary_stores(text, steps)
    text = root_declarations(text, steps)
    text = split_field_initializers(text, steps)
    text = known_divisions(text, steps)
    return text, steps


def transform_setpass(method_source):
    return _setpass(method_source)[0]


def shared_training_floats(method_source, steps=None):
    steps = [] if steps is None else steps
    text = method_source
    t = lex(text)
    at, _ = find(t, 'if (a_.y_ == 0f || v_.y_ == 0f || pos_.y_ <= 0f)')
    first_open, first_end = body(t, at)
    second_header = vals('else if (!flag)')
    if [z.value for z in t[first_end + 1:first_end + 1 + len(second_header)]] != second_header:
        raise ValueError('exclusive-training-second-arm-not-proven')
    second_open, second_end = body(t, first_end + 2)
    # Both arms must be in a direct else-block; no intermediate owner is moved.
    frames = []
    for i in range(at):
        if t[i].value == '{': frames.append(i)
        elif t[i].value == '}': frames.pop()
    owner = frames[-1]
    if owner + 1 != at or t[owner - 1].value != 'else': raise ValueError('training-shared-else-owner-not-proven')
    changes = []
    # Change initialized declarations into assignments in arm1.
    first = t[first_open + 1:first_end]
    for name in ['num', 'num2']:
        index, _ = find(first, 'float ' + name + ' =')
        token = first[index]
        changes.append((token.start, first[index + 1].start, ''))
    # Delete only the bare declarations at the beginning of arm2.
    expected = vals('float num; float num2;')
    if [z.value for z in t[second_open + 1:second_open + 1 + len(expected)]] != expected:
        raise ValueError('second-arm-bare-pair-not-proven')
    changes.append((t[second_open + 1].start, t[second_open + len(expected)].end, ''))
    changes.append((t[owner].end, t[owner].end, '\r\n\t\t\t\tfloat num;\r\n\t\t\t\tfloat num2;'))
    for a, b, replacement in sorted(changes, reverse=True):
        text = replace(text, a, b, replacement, 'shared-training-float-declarations-same-exclusive-arms', steps)
    return text


def unused_training_float(method_source, steps=None):
    steps = [] if steps is None else steps
    t = lex(method_source)
    begin, count = find(t, 'bool flag2 = false; int[] collisionData =')
    bool_end = next(i for i in range(begin, begin + count) if t[i].value == ';')
    insertion = t[bool_end].end
    if 'recoveredUnusedSingle' in [z.value for z in t]: raise ValueError('synthetic-name-not-fresh')
    return replace(method_source, insertion, insertion, '\r\n\t\t\t\t\tfloat recoveredUnusedSingle;',
                   'original-unused-single-slot4-after-flag2-before-collisiondata', steps)


def training_arithmetic(method_source, steps=None):
    steps = [] if steps is None else steps
    text = method_source
    patterns = [('x_', 'num', 2), ('z_', 'num2', 2), ('y_', '0.995f', 1), ('x_', '0.9f', 1), ('z_', '0.9f', 1)]
    for field, rhs, expected_count in patterns:
        needle = 'v_.' + field + ' *= ' + rhs + ';'
        t = lex(text); wanted = vals(needle)
        hits = [i for i in range(len(t) - len(wanted) + 1) if [z.value for z in t[i:i + len(wanted)]] == wanted]
        if len(hits) != expected_count: raise ValueError('known-multiply-site-count-not-proven:' + needle)
        for at in reversed(hits):
            text = replace(text, t[at].start, t[at + len(wanted) - 1].end,
                           'v_.' + field + ' = v_.' + field + ' * ' + rhs + ';',
                           'existing-rule8-explicit-multiply-' + field + '-' + rhs, steps)
    t = lex(text)
    at, count = find(t, 'v_.y_ = (0f - v_.y_) / 2f;')
    return replace(text, t[at].start, t[at + count - 1].end, 'v_.y_ = (-v_.y_) / 2f;',
                   'original-unary-neg-instead-of-zero-subtraction', steps)


def _training(text):
    steps = []
    text = shared_training_floats(text, steps)
    text = unused_training_float(text, steps)
    text = training_arithmetic(text, steps)
    return text, steps


def transform_training(method_source):
    return _training(method_source)[0]


