"""Pure calculated six-hunk dribble composition; UNKNOWN, never READY.
No I/O in import/API calls; driver only reads sources and writes ignored WIP.
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


def owner_for(t, at, variable, initial=0):
    frames = []
    for i in range(at):
        if t[i].value == '{': frames.append(i)
        elif t[i].value == '}': frames.pop()
    parent = frames[-1]
    if t[parent - 1].value != ')': raise ValueError('nearest-block-not-for')
    pairs = [i for i in range(parent - 1) if t[i].value == '(' and matching(t, i) == parent - 1]
    if len(pairs) != 1 or t[pairs[0] - 1].value != 'for': raise ValueError('nearest-owner-not-for')
    if [z.value for z in t[pairs[0] + 1:pairs[0] + 5]] != vals('int ' + variable + ' = ' + str(initial)):
        raise ValueError('for-variable-identity-not-proven:' + variable)
    return parent, matching(t, parent)


def positive_collision_tail(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (!(GameUtil.ChOnLine(screen_pos_, player2.screen_pos_, 12, player3.screen_pos_)))')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('continue;'): raise ValueError('negative-collision-guard-not-only-continue')
    _, loop_end = owner_for(t, start, 'l')
    tail = t[b + 1:loop_end]
    if [z.value for z in tail] != vals('flag2 = true; eva_pass_info_onEnemy[k][0] = 1; break;'):
        raise ValueError('positive-collision-tail-not-proven')
    payload = text[t[b].end:t[loop_end].start]
    return edit(text, t[start].start, t[loop_end].start,
                'if (GameUtil.ChOnLine(screen_pos_, player2.screen_pos_, 12, player3.screen_pos_)) {' + payload + '}',
                'positive-collision-tail-with-loop-break-whole-composition-only', steps)


def nested_empty_pass_guard(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (flag2 && eva_pass_info[k][0] != 1)')
    a, b = body(t, start)
    owner_for(t, start, 'k', 1)
    payload = text[t[a].start:t[b].end]
    return edit(text, t[start].start, t[b].end,
                'if (flag2) { if (eva_pass_info[k][0] == 1) { } else ' + payload + ' }',
                'flag2-nested-empty-eva-one-arm-with-local-join-not-continue', steps)


def explicit_fallback_continue(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (n != p_id_)')
    a, b = body(t, start)
    _, loop_end = owner_for(t, start, 'n')
    if b + 1 != loop_end or [z.value for z in t[a + 1:b]] != vals('eva_pass[n][0] = 1f; break;'):
        raise ValueError('fallback-loop-tail-not-proven')
    return edit(text, t[start].start, t[b].end,
                'if (n == p_id_) { continue; }' + text[t[a].end:t[b].start],
                'fallback-loop-explicit-self-continue-before-assignment-and-break', steps)


def separate_team_float_continues(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if ((team_id_ == 0 && pos_.z_ > player4.pos_.z_) || (team_id_ == 1 && pos_.z_ < player4.pos_.z_) || (player4.address_[0] == player4.mostEvaAddres_[0] && player4.address_[1] == player4.mostEvaAddres_[1]))')
    a, b = body(t, start)
    owner_for(t, start, 'num9', 1)
    if [z.value for z in t[a + 1:b]] != vals('continue;'): raise ValueError('team-guard-body-not-only-continue')
    replacement = ('if (team_id_ == 0) { if (pos_.z_ > player4.pos_.z_) { continue; } }\r\n'
                   '\t\t\t\tif (team_id_ == 1) { if (pos_.z_ < player4.pos_.z_) { continue; } }\r\n'
                   '\t\t\t\tif (player4.address_[0] == player4.mostEvaAddres_[0] && player4.address_[1] == player4.mostEvaAddres_[1]) { continue; }')
    return edit(text, t[start].start, t[b].end, replacement,
                'two-owner-proven-team-continues-preserve-positive-float-gt-lt', steps)


def separate_address_continue(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if ((address_[0] != player5.address_[0] || address_[1] != player5.address_[1]) && GameUtil.ChOnLine(screen_pos_, temp_afr_mostEvaSPos, 12, player5.screen_pos_))')
    a, b = body(t, start)
    _, loop_end = owner_for(t, start, 'num12')
    if b + 1 != loop_end or [z.value for z in t[a + 1:b]] != vals('flag4 = true; eva_pass_info_onEnemy[num9][1] = 1; break;'):
        raise ValueError('address-loop-payload-tail-not-proven')
    payload = text[t[a].start:t[b].end]
    replacement = ('if (address_[0] == player5.address_[0] && address_[1] == player5.address_[1]) { continue; }\r\n'
                   '\t\t\t\t\tif (GameUtil.ChOnLine(screen_pos_, temp_afr_mostEvaSPos, 12, player5.screen_pos_)) ' + payload)
    return edit(text, t[start].start, t[b].end, replacement,
                'two-int-coordinate-equality-continue-before-positive-collision-if', steps)


def _transform(text):
    steps = []
    for fn in [positive_collision_tail, nested_empty_pass_guard, explicit_fallback_continue,
               separate_team_float_continues, separate_address_continue]:
        text = fn(text, steps)
    return text, steps


def transform_dribble(method_source):
    """Header-independent, no I/O. Only whole composition is a proposed candidate."""
    return _transform(method_source)[0]


