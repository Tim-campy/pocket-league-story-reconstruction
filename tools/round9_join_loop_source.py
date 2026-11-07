"""Pure D30 join and loop composition calculations, UNKNOWN, no I/O during APIs.
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


def anonymous_body_hashes(text):
    t = lex(text)
    result = []
    for i, token in enumerate(t):
        if token.value != 'delegate':
            continue
        opening = next(j for j in range(i + 1, len(t)) if t[j].value == '{')
        closing = matching(t, opening)
        result.append(sha(text[t[opening].start:t[closing].end].encode('utf-8')))
    return result


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


def find_in(t, start, end, needle):
    wanted = vals(needle)
    hits = [i for i in range(start, end - len(wanted) + 1)
            if [z.value for z in t[i:i + len(wanted)]] == wanted]
    if len(hits) != 1:
        raise ValueError('bounded-pattern-not-unique:' + needle)
    return hits[0], len(wanted)


def type_branch(t, number):
    start, _ = find(t, 'else if (type_ == ' + str(number) + ')')
    return body(t, start + 1)


def friend_mail_if_else(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    parent, parent_end = type_branch(t, 5)
    start, _ = find_in(t, parent, parent_end, 'if (select_ == 0)')
    a, b = body(t, start)
    if b != a + 1 or [z.value for z in t[b + 1:b + 3]] != ['else', '{']:
        raise ValueError('mail-positive-arm-not-empty-with-else')
    false_open = b + 2
    false_end = matching(t, false_open)
    if [z.value for z in t[false_open + 1:false_end]] != vals('return Pop();'):
        raise ValueError('mail-else-must-only-return-Pop')
    mail = vals('FriendData.ActionSendMail(friendData_.INTRODUCTION_APPLY_SUBJECT, friendData_.INTRODUCTION_APPLY_TEXT);')
    leaf_start = false_end + 1
    if [z.value for z in t[leaf_start:leaf_start + len(mail)]] != mail:
        raise ValueError('mail-not-immediately-after-empty-then-else-return')
    leaf_end = leaf_start + len(mail) - 1
    owner = frames(t, start)[-1]
    owner_predicate = vals('if (canvas_.CheckKeyPulse(1048576))')
    if [z.value for z in t[owner - len(owner_predicate):owner]] != owner_predicate:
        raise ValueError('mail-owner-is-not-the-existing-keypulse-if')
    if leaf_end + 1 != matching(t, owner):
        raise ValueError('mail-owner-has-additional-unmapped-tail')
    literal_mail = text[t[leaf_start].start:t[leaf_end].end]
    return edit(text, t[start].start, t[leaf_end].end,
                'if (select_ == 0) {' + literal_mail + '} else { return Pop(); }',
                'friend-type5-mail-then-Pop-else-preserve-owned-keypulse-join', steps)


def friend_apply_if_else(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    parent, parent_end = type_branch(t, 6)
    start, _ = find_in(t, parent, parent_end, 'if (friendData_.applyList_.Size() <= 0)')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals('return Pop();'):
        raise ValueError('apply-guard-must-only-return-Pop')
    owner = frames(t, start)[-1]
    end = matching(t, owner)
    tail = text[t[b].end:t[end].start]
    if vals(tail) != vals('friend_ = (Friend)friendData_.applyList_.FirstElement();'):
        raise ValueError('apply-owned-tail-not-single-friend-assignment')
    owner_predicate = vals('if (next_.type_ == 8)')
    if [z.value for z in t[owner - len(owner_predicate):owner]] != owner_predicate:
        raise ValueError('apply-guard-owner-is-not-next-type8')
    return edit(text, t[start].start, t[end].start,
                'if (friendData_.applyList_.Size() > 0) {' + tail + '} else { return Pop(); }',
                'friend-type6-positive-Int32-apply-size-then-else-Pop-before-next-type-join', steps)


def friend_state3_if_else(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    parent, parent_end = type_branch(t, 8)
    start, _ = find_in(t, parent, parent_end, 'if (friend_.state_.Get() != 3)')
    a, b = body(t, start)
    original_thread_body = text[t[a].start:t[b].end]
    if [z.value for z in t[b - 3:b]] != vals('return true;'):
        raise ValueError('thread-branch-does-not-end-in-return-true')
    if sum(z.value == 'delegate' for z in t[a + 1:b]) != 1:
        raise ValueError('thread-branch-must-contain-one-existing-anonymous-function')
    owner = frames(t, start)[-1]
    end = matching(t, owner)
    tail = text[t[b].end:t[end].start]
    if vals(tail) != vals('FriendForm friendForm3 = new FriendForm(9); friendForm3.friend_ = friend_; Push(friendForm3);'):
        raise ValueError('state3-positive-tail-not-original-three-statements')
    owner_predicate = vals('if (select_ == 0)')
    if [z.value for z in t[owner - len(owner_predicate):owner]] != owner_predicate:
        raise ValueError('state3-owner-is-not-existing-select-zero-if')
    if [z.value for z in t[end + 1:end + 6]] != vals('return Pop();'):
        raise ValueError('state3-shared-Pop-return-not-next-sibling')
    return edit(text, t[start].start, t[end].start,
                'if (friend_.state_.Get() == 3) {' + tail + '} else ' + original_thread_body,
                'friend-type8-state3-dialog-first-else-preserved-thread-return-true', steps)


def _friend(text):
    steps = []
    for fn in [friend_mail_if_else, friend_apply_if_else, friend_state3_if_else]:
        text = fn(text, steps)
    return text, steps


def transform_friend_update(method_source):
    result=_friend(method_source)[0]
    if anonymous_body_hashes(method_source) != anonymous_body_hashes(result): raise ValueError('anonymous-body-order-or-text-changed')
    return result


def area_for_flag_condition(text, variable, flag, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    header = 'for (int ' + variable + ' = 0; ' + variable + ' < tGroupAreas_.Length; ' + variable + '++)'
    start, size = find(t, header)
    opening = start + size
    if t[opening].value != '{':
        raise ValueError('area-for-explicit-block-required')
    guard = opening + 1
    expected = vals('if (!' + flag + ')')
    if [z.value for z in t[guard:guard + len(expected)]] != expected:
        raise ValueError('area-flag-guard-not-first-for-command')
    a, b = body(t, guard)
    if [z.value for z in t[a + 1:b]] != vals('break;'):
        raise ValueError('area-flag-guard-not-break-only')
    replacement = 'for (int ' + variable + ' = 0; ' + variable + ' < tGroupAreas_.Length && ' + flag + '; ' + variable + '++) {'
    return edit(text, t[start].start, t[b].end, replacement,
                'area-' + variable + '-range-first-flag-second-for-condition-not-top-body-break', steps)


def area_hit_continue(text, inner_variable, flag, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    loop, size = find(t, 'for (int ' + inner_variable + ' = 0;')
    opening = matching(t, loop + 1) + 1
    if t[opening].value != '{': raise ValueError('area-inner-explicit-block-required')
    end = matching(t, opening)
    start, _ = find_in(t, opening, end, 'if (!GameUtil.ChRectHitRect(')
    a, b = body(t, start)
    if [z.value for z in t[a + 1:b]] != vals(flag + ' = false; break;'):
        raise ValueError('area-hit-false-body-not-flag-assignment-and-owned-break')
    if b + 1 != end or frames(t, start)[-1] != opening:
        raise ValueError('area-hit-guard-not-terminal-in-own-inner-loop')
    closing_condition = matching(t, start + 1)
    if t[start + 2].value != '!': raise ValueError('area-hit-explicit-not-required')
    call = text[t[start + 3].start:t[closing_condition].start]
    return edit(text, t[start].start, t[b].end,
                'if (' + call + ') { continue; } ' + flag + ' = false; break;',
                'area-' + inner_variable + '-positive-hit-continue-before-flag-false-break-same-owner', steps)


def area_positive_state14(text, steps=None):
    steps = [] if steps is None else steps
    t = lex(text)
    start, _ = find(t, 'if (state_ != 14)')
    a, b = body(t, start)
    stack = frames(t, start)
    if len(stack) != 2 or t[stack[-1] - 1].value != 'else' or stack[-1] + 1 != start:
        raise ValueError('area-final-root-else-owner-not-proven')
    if [z.value for z in t[a + 1:b]] != vals('return;'):
        raise ValueError('area-final-state-guard-must-only-return')
    end = matching(t, stack[-1])
    if [z.value for z in t[end + 1:]] != ['}']:
        raise ValueError('area-final-else-not-terminal-in-method')
    return edit(text, t[stack[-1] - 1].start, t[b].end, 'else if (state_ == 14) {',
                'area-final-positive-state14-preserve-existing-tail-block', steps)


def _area(text):
    steps = []
    text = area_for_flag_condition(text, 'k', 'flag', steps)
    text = area_hit_continue(text, 'l', 'flag', steps)
    text = area_for_flag_condition(text, 'num6', 'flag2', steps)
    text = area_hit_continue(text, 'num7', 'flag2', steps)
    return area_positive_state14(text, steps), steps


def transform_area_update(method_source):
    return _area(method_source)[0]



