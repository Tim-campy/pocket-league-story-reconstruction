"""Pure D27 empty-arm/state calculations, UNKNOWN, no I/O during APIs.
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


def terminal_root_else_gate(text, predicate, positive, label, steps):
    t = lex(text)
    start, _ = find(t, 'if (' + predicate + ')')
    a, b = body(t, start)
    stack = frames(t, start)
    if len(stack) != 2 or t[stack[-1] - 1].value != 'else' or stack[-1] + 1 != start:
        raise ValueError('root-final-else-guard-owner-not-proven')
    if [z.value for z in t[a + 1:b]] != vals('return;'):
        raise ValueError('root-final-guard-must-only-return')
    end = matching(t, stack[-1])
    if [z.value for z in t[end + 1:]] != ['}']:
        raise ValueError('final-else-not-terminal-method-command')
    return edit(text, t[stack[-1] - 1].start, t[b].end,
                'else if (' + positive + ') {', label, steps)


def setpos_positive_mode3(text, steps=None):
    steps = [] if steps is None else steps
    return terminal_root_else_gate(text, 'tArea_.tMode_ != 3', 'tArea_.tMode_ == 3',
                                   'setpos-final-positive-mode3-retain-existing-body-block', steps)


def setpos_direct_index_tests(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, count = find(t, 'switch (j)')
    opening = start + count
    end = matching(t, opening)
    expected = vals('{ case 1: num9 = 70; break; case 2: num9 = 15; break; }')
    if [z.value for z in t[opening:end + 1]] != expected:
        raise ValueError('setpos-exact-two-index-assignments-required')
    return edit(text, t[start].start, t[end].end,
                'if (j == 1) { num9 = 70; } else if (j == 2) { num9 = 15; }',
                'setpos-direct-index-tests-without-extra-int32-switch-cache', steps)


def setpos_original_neg(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, count = find(t, 'num13 = 0f - num13;')
    return edit(text, t[start].start, t[start + count - 1].end, 'num13 = -num13;',
                'setpos-unary-neg-as-original-preserve-floating-negation', steps)


def _setpos(text):
    steps = []
    for fn in [setpos_positive_mode3, setpos_direct_index_tests, setpos_original_neg]:
        text = fn(text, steps)
    return text, steps


def transform_set_pos(method_source):
    return _setpos(method_source)[0]


def change_empty_mode3(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'else if ((tArea_.tMode_ != 3))')
    a, b = body(t, start + 1)
    if b != a + 1: raise ValueError('change-mode3-arm-must-be-empty')
    return edit(text, t[start].start, t[b].end, 'else if (tArea_.tMode_ == 3) { }',
                'change-empty-mode3-bne-to-next-without-adding-empty-else', steps)


def change_positive_state14(text, steps=None):
    steps = [] if steps is None else steps
    return terminal_root_else_gate(text, 'state_ != 14', 'state_ == 14',
                                   'change-final-positive-state14-retain-local-declaration-block', steps)


def change_tenant_local_join(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    old, _ = find(t, 'if (tPlayer9.oldTrainingId_[1] == 0)')
    parent, parent_end = body(t, old)
    start = parent + 1
    head = vals('if (tPlayer9.trainingId_[1] == 0)')
    if [z.value for z in t[start:start + len(head)]] != head:
        raise ValueError('training-inner-guard-not-first-parent-command')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('tPlayer9.AddCommandTenantIn(); continue;'):
        raise ValueError('training-true-command-and-owned-continue-not-proven')
    ending = vals('num4 += 6;')
    if [z.value for z in t[parent_end - len(ending):parent_end]] != ending:
        raise ValueError('training-false-tail-boundary-not-proven')
    # Ensure the enclosing owner is precisely the existing num5 loop, not another loop.
    loop_found = False
    for opening in reversed(frames(t, old)):
        if t[opening - 1].value != ')': continue
        pairs = [i for i in range(opening - 1) if t[i].value == '(' and matching(t, i) == opening - 1]
        if len(pairs) != 1: raise ValueError('training-ambiguous-owner')
        p = pairs[0]
        if t[p - 1].value in ('for', 'while', 'foreach'):
            if t[p - 1].value != 'for' or [z.value for z in t[p + 1:p + 5]] != vals('int num5 = 0'):
                raise ValueError('training-nearest-loop-must-be-num5')
            loop_found = True
            break
    if not loop_found: raise ValueError('training-loop-owner-not-found')
    true = text[t[a].end:t[b - 2].start]
    false = text[t[b].end:t[parent_end].start]
    return edit(text, t[start].start, t[parent_end].start,
                'if (tPlayer9.trainingId_[1] == 0) {' + true + '} else {' + false + '}',
                'training-tenant-if-else-local-join-before-outer-loop-join', steps)


def _change(text):
    steps = []
    for fn in [change_empty_mode3, change_positive_state14, change_tenant_local_join]:
        text = fn(text, steps)
    return text, steps


def transform_change_state(method_source):
    return _change(method_source)[0]


def team_two_empty_game_states(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'else if ((udat_.gm_.gameState_ == team_id_) || (udat_.gm_.gameState_ == 1 - team_id_))')
    a, b = body(t, start + 1)
    if b != a + 1 or [z.value for z in t[b + 1:b + 3]] != ['else', '{']:
        raise ValueError('team-current-or-arm-must-have-two-empty-branches')
    second = b + 2
    second_end = matching(t, second)
    if second_end != second + 1: raise ValueError('team-final-else-must-be-empty')
    next_loop = vals('for (int l = 0;')
    if [z.value for z in t[second_end + 1:second_end + 1 + len(next_loop)]] != next_loop:
        raise ValueError('team-game-state-join-not-followed-by-existing-player-loop')
    replacement = ('else if (udat_.gm_.gameState_ == team_id_) { } '
                   'else if (udat_.gm_.gameState_ == 1 - team_id_) { }')
    return edit(text, t[start].start, t[second_end].end, replacement,
                'team-two-empty-game-state-arms-without-extra-empty-else-join', steps)


def team_final_empty_states(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'else if (udat_.gm_.state_ != 7 && udat_.gm_.state_ != 6 && udat_.gm_.state_ == 8)')
    a, b = body(t, start + 1)
    if [z.value for z in t[b + 1:]] != ['}']:
        raise ValueError('team-final-state-arm-not-terminal-to-method')
    payload = text[t[a].start:t[b].end]
    return edit(text, t[start].start, t[b].end,
                'else if (udat_.gm_.state_ == 7) { } else if (udat_.gm_.state_ == 6) { } else if (udat_.gm_.state_ == 8) ' + payload,
                'team-final-empty7-empty6-positive8-retain-player-loop-body', steps)


def _team(text):
    steps = []
    text = team_two_empty_game_states(text, steps)
    return team_final_empty_states(text, steps), steps


def transform_team_update(method_source):
    return _team(method_source)[0]

