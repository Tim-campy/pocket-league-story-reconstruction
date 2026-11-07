"""Pure text proposal: terminal OR-return list -> empty else-if arms.

No compiler, semantic model, AST helper, verifier, production write, or READY.
transform_friend accepts an isolated method declaration; its name/header is free.
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


def _statement_end(tokens, start):
    """Only block bodies and if/else chains are accepted for the preserved tail."""
    if tokens[start].value == '{':
        return matching(tokens, start) + 1
    if tokens[start].value != 'if' or tokens[start + 1].value != '(':
        raise ValueError('tail-must-be-if-chain-with-block-bodies')
    condition_end = matching(tokens, start + 1)
    if tokens[condition_end + 1].value != '{':
        raise ValueError('tail-arm-must-have-explicit-block')
    end = _statement_end(tokens, condition_end + 1)
    if end < len(tokens) and tokens[end].value == 'else':
        end = _statement_end(tokens, end + 1)
    return end


def _proposal(method_source):
    t = lex(method_source)
    root = next((i for i, token in enumerate(t) if token.value == '{'), None)
    if root is None or matching(t, root) != len(t) - 1:
        raise ValueError('input-must-be-one-method-with-explicit-body')
    candidates = []
    for i in range(root + 1, len(t) - 3):
        if t[i].value != 'if' or t[i + 1].value != '(':
            continue
        close = matching(t, i + 1)
        terms = []
        expression = t[i + 2:close]
        for j in range(0, len(expression), 4):
            group = expression[j:j + 3]
            if len(group) != 3 or not re.fullmatch(r'[A-Za-z_@][A-Za-z_0-9]*', group[0].value) or group[1].value != '==' or not group[2].value.isdigit():
                break
            terms.append((group[0].value, int(group[2].value), group[0].start, group[2].end))
            if j + 3 < len(expression) and expression[j + 3].value != '||':
                break
        else:
            if len(terms) >= 2 and len(expression) == 4 * len(terms) - 1 and len({term[0] for term in terms}) == 1:
                candidates.append((i, close, terms))
    if len(candidates) != 1:
        raise ValueError('expected-one-flat-same-selector-or-list')
    start, condition_end, terms = candidates[0]
    body = condition_end + 1
    if [z.value for z in t[body:body + 4]] != ['{', 'return', ';', '}']:
        raise ValueError('guard-must-contain-only-void-return')
    following = body + 4
    if following >= len(t) or t[following].value != 'if':
        raise ValueError('guard-must-be-followed-by-if-chain')

    # Prove the guard parent is method root or its final immediate else-block.
    braces = []
    for j in range(root, start):
        if t[j].value == '{':
            braces.append(j)
        elif t[j].value == '}':
            braces.pop()
    if not braces or len(braces) > 2:
        raise ValueError('nested-ownership-unsupported')
    parent = braces[-1]
    if len(braces) == 2 and t[parent - 1].value != 'else':
        raise ValueError('nonterminal-or-loop-protected-parent-unsupported')
    if start != parent + 1:
        raise ValueError('guard-must-be-first-parent-statement')
    parent_end = matching(t, parent)
    if _statement_end(t, following) != parent_end:
        raise ValueError('preserved-if-chain-must-exhaust-parent-tail')
    if any(z.value != '}' for z in t[parent_end + 1:]):
        raise ValueError('parent-is-not-terminal-to-method')
    if len({term[1] for term in terms}) != len(terms):
        raise ValueError('duplicate-selector-constant')
    # Read the next test only to reject a duplicate condition, never to synthesize its body.
    next_close = matching(t, following + 1)
    next_expression = [z.value for z in t[following + 2:next_close]]
    if len(next_expression) != 3 or next_expression[:2] != [terms[0][0], '=='] or not next_expression[2].isdigit() or int(next_expression[2]) in {term[1] for term in terms}:
        raise ValueError('following-chain-selector-not-proven')

    a, b = t[start].start, t[following].start
    newline = '\r\n' if '\r\n' in method_source else '\n'
    line_start = method_source.rfind('\n', 0, a) + 1
    indent = method_source[line_start:a]
    if indent.strip():
        indent = ''
    conditions = [method_source[term[2]:term[3]] for term in terms]
    arms = [f"{'if' if i == 0 else 'else if'} ({condition}) {{ }}" for i, condition in enumerate(conditions)]
    replacement = (newline + indent).join(arms) + newline + indent + 'else '
    after = method_source[:a] + replacement + method_source[b:]
    return after, {
        'python_start': a, 'python_end': b,
        'start': len(method_source[:a].encode('utf-16-le')) // 2,
        'end': len(method_source[:b].encode('utf-16-le')) // 2,
        'before': method_source[a:b], 'after': replacement,
        'selector': terms[0][0], 'constants': [term[1] for term in terms],
        'following_constant': int(next_expression[2]),
        'terminal_parent_kind': 'method-root' if len(braces) == 1 else 'final-immediate-else-block',
        'preserved_tail_if_chain': True,
        'comments_in_replaced_span': '//' in method_source[a:b] or '/*' in method_source[a:b],
    }


def transform_friend(method_source):
    """Header-independent pure text transform; unsupported source raises ValueError."""
    after, proof = _proposal(method_source)
    if proof['comments_in_replaced_span']:
        raise ValueError('comments-in-control-span-require-manual-review')
    return after


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


