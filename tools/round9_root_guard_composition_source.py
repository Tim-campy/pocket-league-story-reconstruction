"""Pure root-guard composition proposals; no model, compiler, fixture or verifier.

The lexical scanner preserves string/comment contents and edits raw source text.
All emitted records remain UNKNOWN and are not READY for adoption.
"""
import hashlib
import json
import re
from dataclasses import dataclass
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / 'build/round9/root-guard-composition-source-proposals'


@dataclass(frozen=True)
class Token:
    value: str
    start: int
    end: int


def lex(text):
    out = []
    i = 0
    while i < len(text):
        c = text[i]
        if c.isspace():
            i += 1
            continue
        if text.startswith('//', i):
            end = text.find('\n', i + 2)
            i = len(text) if end < 0 else end
            continue
        if text.startswith('/*', i):
            end = text.find('*/', i + 2)
            if end < 0:
                raise ValueError('unterminated-comment')
            i = end + 2
            continue
        start = i
        verbatim = text.startswith('@"', i)
        if verbatim or c in ('"', "'"):
            quote = '"' if verbatim else c
            i += 2 if verbatim else 1
            while i < len(text):
                if text[i] == quote:
                    if verbatim and text.startswith('""', i):
                        i += 2
                        continue
                    i += 1
                    break
                if not verbatim and text[i] == '\\':
                    i += 2
                else:
                    i += 1
            else:
                raise ValueError('unterminated-string')
        else:
            match = re.match(r'[A-Za-z_@][A-Za-z_0-9]*|[0-9]+|==|!=|&&|\|\||\+\+|--|=>', text[i:])
            i += len(match.group()) if match else 1
        out.append(Token(text[start:i], start, i))
    return out


def matching(tokens, at):
    pairs = {'(': ')', '[': ']', '{': '}'}
    stack = [pairs[tokens[at].value]]
    for i in range(at + 1, len(tokens)):
        value = tokens[i].value
        if value in pairs:
            stack.append(pairs[value])
        elif value in (')', ']', '}'):
            if not stack or value != stack.pop():
                raise ValueError('unbalanced-delimiters')
            if not stack:
                return i
    raise ValueError('unclosed-delimiter')


def values(tokens):
    return [t.value for t in tokens]


def edits(text, changes):
    changes = sorted(changes)
    for (_, end, _), (start, _, _) in zip(changes, changes[1:]):
        if end > start:
            raise ValueError('overlapping-source-edits')
    for start, end, replacement in reversed(changes):
        text = text[:start] + replacement + text[end:]
    return text


def unique(text, old, new):
    if text.count(old) != 1:
        raise ValueError('before-is-not-unique:' + old[:70])
    at = text.index(old)
    return text[:at] + new + text[at + len(old):]


def method_span(text, signature):
    if text.count(signature) != 1:
        raise ValueError('method-signature-not-unique')
    start = text.index(signature)
    tokens = lex(text)
    opening = next(i for i, t in enumerate(tokens) if t.start >= start and t.value == '{')
    close = matching(tokens, opening)
    return start, tokens[close].end


def statement_end(tokens, at):
    if tokens[at].value == '{':
        return matching(tokens, at)
    return next(i for i in range(at, len(tokens)) if tokens[i].value == ';')


IDENTIFIER = re.compile(r'@?[A-Za-z_][A-Za-z_0-9]*\Z')
FORBIDDEN = {'delegate', '=>', 'yield', 'await', 'async', 'fixed', 'using',
             'unsafe', 'stackalloc', 'ref', 'out', 'const', 'goto'}


def _type_end(tokens, at):
    """Lexical recognition only, not a semantic type claim."""
    if at >= len(tokens) or not IDENTIFIER.fullmatch(tokens[at].value):
        return None
    i = at + 1
    while i + 1 < len(tokens) and tokens[i].value in ('.', ':'):
        if tokens[i].value == ':' and tokens[i + 1].value == ':':
            i += 2
        else:
            i += 1
        if i >= len(tokens) or not IDENTIFIER.fullmatch(tokens[i].value):
            return None
        i += 1
    if i < len(tokens) and tokens[i].value == '<':
        depth = 1
        i += 1
        while i < len(tokens) and depth:
            depth += (tokens[i].value == '<') - (tokens[i].value == '>')
            i += 1
        if depth:
            return None
    while i < len(tokens) and tokens[i].value == '[':
        close = matching(tokens, i)
        if any(t.value != ',' for t in tokens[i + 1:close]):
            break
        i = close + 1
    if i < len(tokens) and tokens[i].value == '?':
        i += 1
    return i


def _declaration(tokens, at, text):
    type_end = _type_end(tokens, at)
    if type_end is None or type_end + 1 >= len(tokens):
        return None
    if not IDENTIFIER.fullmatch(tokens[type_end].value) or tokens[type_end + 1].value not in ('=', ';', ','):
        return None
    type_text = text[tokens[at].start:tokens[type_end - 1].end]
    name = tokens[type_end].value
    if tokens[type_end + 1].value != '=':
        raise ValueError('root-tail-declaration-must-have-one-initializer')
    i = type_end + 2
    initializer_start = i
    while i < len(tokens):
        if tokens[i].value in ('(', '[', '{'):
            i = matching(tokens, i) + 1
            continue
        if tokens[i].value == ',':
            raise ValueError('multiple-declarators-unsupported')
        if tokens[i].value == ';':
            break
        i += 1
    if i >= len(tokens) or i == initializer_start:
        raise ValueError('root-tail-initializer-not-terminated')
    initializer = text[tokens[initializer_start].start:tokens[i - 1].end]
    return {'type': type_text, 'name': name, 'initializer': initializer,
            'start': tokens[at].start, 'end': tokens[i].end, 'next_token': i + 1}


def _root_indices(tokens, opening, close):
    indices = []
    curly = 0
    paren = 0
    square = 0
    for i in range(opening, close + 1):
        value = tokens[i].value
        if curly == 1 and paren == 0 and square == 0:
            indices.append(i)
        if value == '{':
            curly += 1
        elif value == '}':
            curly -= 1
        elif value == '(':
            paren += 1
        elif value == ')':
            paren -= 1
        elif value == '[':
            square += 1
        elif value == ']':
            square -= 1
    return indices


def calculate_root_guard(method_source, allowed_types=('LineupForm', 'SupervisorData')):
    """Return source + lexical evidence only; never claim model/body approval."""
    tokens = lex(method_source)
    opening = next((i for i, t in enumerate(tokens) if t.value == '{'), None)
    if opening is None or 'void' not in values(tokens[:opening]):
        raise ValueError('only-explicit-void-method-declarations-supported')
    close = matching(tokens, opening)
    if close != len(tokens) - 1:
        raise ValueError('input-must-be-one-complete-method-declaration')
    if any(t.value in FORBIDDEN for t in tokens):
        raise ValueError('captures-or-unsupported-control-modifier')
    # Local functions are unsupported even when block-bodied (and therefore
    # contain no => token). The historical game compiler cannot express them.
    for i in range(opening + 2, close):
        if tokens[i].value != '(':
            continue
        end = matching(tokens, i)
        if end + 1 < close and tokens[end + 1].value == '{' and IDENTIFIER.fullmatch(tokens[i - 1].value):
            if tokens[i - 1].value in ('if', 'for', 'foreach', 'while', 'switch', 'catch', 'using', 'lock'):
                continue
            previous = tokens[i - 2].value
            if IDENTIFIER.fullmatch(previous) and previous not in ('new', 'if', 'for', 'while', 'switch', 'catch', 'using', 'lock'):
                raise ValueError('local-function-unsupported')
    roots = _root_indices(tokens, opening, close)
    matches = []
    for i in roots:
        if tokens[i].value != 'if' or i + 1 >= close or tokens[i + 1].value != '(':
            continue
        condition_end = matching(tokens, i + 1)
        then_open = condition_end + 1
        if tokens[then_open].value != '{':
            continue
        then_end = matching(tokens, then_open)
        if values(tokens[then_open + 1:then_end]):
            continue
        if tokens[then_end + 1].value != 'else' or tokens[then_end + 2].value != '{':
            continue
        else_open = then_end + 2
        else_end = matching(tokens, else_open)
        if values(tokens[else_open + 1:else_end]) != ['return', ';']:
            continue
        at = else_end + 1
        declarations = []
        while at < close:
            declaration = _declaration(tokens, at, method_source)
            if declaration is None:
                break
            if declaration['type'] not in allowed_types or declaration['type'] in ('var', 'dynamic'):
                raise ValueError('root-tail-type-not-explicitly-supported:' + declaration['type'])
            declarations.append(declaration)
            at = declaration['next_token']
        if not declarations:
            continue
        if any(t.value in ('return', 'break', 'continue', 'throw') for t in tokens[else_end + 1:close]):
            raise ValueError('tail-transfer-ownership-unsupported')
        # A second root declaration later in the tail would change its owner if
        # moved silently. This narrow family supports consecutive leading ones.
        for j in roots:
            if j < at or j >= close or tokens[j].value in ('if', 'for', 'foreach', 'while', 'do', 'try', 'catch', 'finally', 'switch'):
                continue
            if j > 0 and tokens[j - 1].value not in (';', '}', '{'):
                continue
            if _declaration(tokens, j, method_source) is not None:
                raise ValueError('nonleading-root-tail-declaration-unsupported')
        condition_tokens = tokens[i + 2:condition_end]
        names = [d['name'] for d in declarations]
        if len(names) != len(set(names)):
            raise ValueError('duplicate-root-local')
        if any(t.value in names for t in condition_tokens):
            raise ValueError('guard-refers-to-hoisted-local')
        # No fixture or semantic binder is run here. The single callable is
        # limited to existing plain declaration statements, never for variables.
        condition = method_source[tokens[i + 1].end:tokens[condition_end].start]
        tail_start = tokens[else_end].end
        tail_end = tokens[close].start
        tail = method_source[tail_start:tail_end]
        replacements = [(d['start'] - tail_start, d['end'] - tail_start,
                         d['name'] + ' = ' + d['initializer'] + ';') for d in declarations]
        assigned_tail = edits(tail, replacements)
        nl = '\r\n' if '\r\n' in method_source else '\n'
        bare = nl.join(d['type'] + ' ' + d['name'] + ';' for d in declarations)
        replacement = bare + nl + 'if (' + condition + ') {' + assigned_tail + nl + '}' + nl
        after = edits(method_source, [(tokens[i].start, tail_end, replacement)])
        to_utf16 = lambda at: len(method_source[:at].encode('utf-16-le')) // 2
        matches.append({'after': after, 'guard_source_span': [tokens[i].start, tokens[else_end].end],
                        'guard_source_span_utf16': [to_utf16(tokens[i].start), to_utf16(tokens[else_end].end)],
                        'lexical_offsets': 'Python-codepoint offsets relative to method declaration; explicit *_utf16 fields use UTF16',
                        'condition_preserved': condition,
                        'root_declarations': [{k: v for k, v in d.items() if k != 'next_token'} for d in declarations],
                        'nested_declarations_moved': False, 'initializer_order_preserved': True,
                        'model_proof': 'UNKNOWN', 'body_oracle': 'UNKNOWN'})
    if len(matches) != 1:
        raise ValueError('root-guard-composition-not-unique:' + str(len(matches)))
    return matches[0]


def transform_root_guard(method_source, allowed_types=('LineupForm', 'SupervisorData')):
    """Fixture.Run-compatible API: source declaration string -> source string."""
    return calculate_root_guard(method_source, allowed_types)['after']


TARGETS = [
    ('system.KairoService::OnStart()->System.Void', 'src/Assembly-CSharp/system/KairoService.cs',
     'public override void OnStart()', 600),
    ('form.GameForm::ProgressTime(System.Int32,System.Int32)->System.Void', 'src/Assembly-CSharp/form/GameForm.cs',
     'public void ProgressTime(int add, int frameCount)', 2349),
]


