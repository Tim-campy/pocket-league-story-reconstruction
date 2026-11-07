"""Pure source proposals only: no model, compiler, fixture or verifier invocation.

The lexical scanner preserves string/comment contents and edits raw source text.
All emitted records remain UNKNOWN and are not READY for adoption.
"""
import hashlib
import json
import re
from dataclasses import dataclass
from pathlib import Path

ROOT = next(p for p in Path(__file__).resolve().parents if (p / 'AGENTS.md').exists())
OUT = ROOT / 'build/round9/foreach-composition-source-proposals'


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


def inline_foreach_alias(body, alias_type, alias, expression):
    tokens = lex(body)
    target = lex(alias_type + ' ' + alias + ' = ' + expression + ';')
    sequence = values(target)
    hits = [i for i in range(len(tokens) - len(target) + 1)
            if values(tokens[i:i + len(target)]) == sequence]
    if len(hits) != 1:
        raise ValueError('alias-declaration-not-unique')
    first = hits[0]
    after = first + len(target)
    if tokens[after].value != 'foreach' or tokens[after + 1].value != '(':
        raise ValueError('alias-not-immediately-followed-by-foreach')
    close = matching(tokens, after + 1)
    in_at = next(i for i in range(after + 2, close) if tokens[i].value == 'in')
    if values(tokens[in_at + 1:close]) != [alias]:
        raise ValueError('foreach-does-not-use-alias-directly')
    # These three current aliases are used solely by the foreach expression.
    if sum(t.value == alias for t in tokens) != 2:
        raise ValueError('alias-has-other-uses')
    expression_token = tokens[in_at + 1]
    return edits(body, [(tokens[first].start, tokens[after - 1].end, ''),
                        (expression_token.start, expression_token.end, expression)])


def language_reverse_inline(body):
    tokens = lex(body)
    declaration = values(lex('string[][] systemLanguagePackFiles = Language.GetSystemLanguagePackFiles();'))
    hits = [i for i in range(len(tokens) - len(declaration) + 1)
            if values(tokens[i:i + len(declaration)]) == declaration]
    if len(hits) != 1:
        raise ValueError('language-array-declaration-not-unique')
    first = hits[0]
    for_at = first + len(declaration)
    if tokens[for_at].value != 'for':
        raise ValueError('expected-adopted-indexed-for')
    close = matching(tokens, for_at + 1)
    expected_header = values(lex('(int spriteIndex = 0; spriteIndex < systemLanguagePackFiles.Length; spriteIndex++)'))
    if values(tokens[for_at + 1:close + 1]) != expected_header:
        raise ValueError('unexpected-for-header')
    opening = close + 1
    end = matching(tokens, opening)
    first_end = statement_end(tokens, opening + 1)
    if values(tokens[opening + 1:first_end + 1]) != values(lex('string[] item = systemLanguagePackFiles[spriteIndex];')):
        raise ValueError('element-is-not-the-first-indexed-local')
    if sum(t.value == 'systemLanguagePackFiles' for t in tokens) != 3:
        raise ValueError('named-array-used-outside-loop')
    if sum(t.value == 'spriteIndex' for t in tokens) != 4:
        raise ValueError('index-used-outside-canonical-loop-controls')
    remainder = body[tokens[first_end].end:tokens[end].start]
    replacement = 'foreach (string[] item in Language.GetSystemLanguagePackFiles()) {' + remainder + '}'
    return edits(body, [(tokens[first].start, tokens[end].end, replacement)])


def language_default_bool(section):
    tokens = lex(section)
    wanted = values(lex('Language.GetLanguagePackFile() != text2 && Language.SetLanguagePack(text2) && callback_ != null'))
    hits = []
    for i, token in enumerate(tokens):
        if token.value != 'if' or tokens[i + 1].value != '(':
            continue
        close = matching(tokens, i + 1)
        if values(tokens[i + 2:close]) == wanted:
            hits.append((i, close))
    if len(hits) != 1:
        raise ValueError('default-condition-not-unique')
    at, close = hits[0]
    name = 'recoveredLanguagePackChanged'
    if any(t.value == name for t in tokens):
        raise ValueError('fresh-boolean-name-collision')
    end = statement_end(tokens, close + 1)
    arm = section[tokens[close + 1].start:tokens[end].end]
    after = ('if (Language.GetLanguagePackFile() != text2) {\r\n'
             'bool ' + name + ' = Language.SetLanguagePack(text2);\r\n'
             'if (' + name + ' && callback_ != null) ' + arm + '\r\n}')
    return edits(section, [(tokens[at].start, tokens[end].end, after)])


def language_dispatch(body):
    tokens = lex(body)
    hits = []
    for i, token in enumerate(tokens):
        if token.value == 'switch' and tokens[i + 1].value == '(':
            close = matching(tokens, i + 1)
            if values(tokens[i + 2:close]) == ['num']:
                hits.append((i, close))
    if len(hits) != 1:
        raise ValueError('language-switch-not-unique')
    at, close = hits[0]
    opening, end = close + 1, matching(tokens, close + 1)
    labels = []
    i = opening + 1
    while i < end:
        if tokens[i].value == '{':
            i = matching(tokens, i) + 1
            continue
        if tokens[i].value not in ('case', 'default'):
            raise ValueError('unexpected-top-level-switch-content')
        colon = next(j for j in range(i + 1, end) if tokens[j].value == ':')
        label = ''.join(values(tokens[i + 1:colon])) if tokens[i].value == 'case' else 'default'
        start = colon + 1
        j = start
        while j < end and tokens[j].value not in ('case', 'default'):
            j = matching(tokens, j) + 1 if tokens[j].value in ('{', '(', '[') else j + 1
        labels.append((label, start, j))
        i = j
    if [x[0] for x in labels] != ['-3', '-2', '-1', 'default']:
        raise ValueError('case-order-does-not-match-original-evidence')
    output = []
    for ordinal, (label, first, last) in enumerate(labels):
        block = tokens[first].value == '{'
        terminal = last - 2 if block else last - 1
        if tokens[terminal].value != ';' or tokens[terminal - 1].value != 'break':
            raise ValueError('section-does-not-end-in-switch-break')
        if sum(t.value == 'break' for t in tokens[first:last]) != 1:
            raise ValueError('additional-break-owner-is-not-proved')
        start_pos = tokens[first].start
        end_pos = tokens[last - 1].end
        section = body[start_pos:tokens[terminal - 1].start] + body[tokens[terminal].end:end_pos]
        if not block:
            section = '{\r\n' + section + '\r\n}'
        if label == 'default':
            section = language_default_bool(section)
        prefix = 'else' if label == 'default' else ('if' if ordinal == 0 else 'else if') + ' (num == ' + label + ')'
        output.append(prefix + ' ' + section)
    return edits(body, [(tokens[at].start, tokens[end].end, '\r\n'.join(output))])


def language(body):
    return language_dispatch(language_reverse_inline(body))


def billing(body):
    after = inline_foreach_alias(body, 'string[][]', 'array2', 'array')
    tokens = lex(after)
    wanted = values(lex('array3[0] != productId'))
    matches = []
    for i, token in enumerate(tokens):
        if token.value != 'foreach' or tokens[i + 1].value != '(':
            continue
        close = matching(tokens, i + 1)
        if values(tokens[i + 2:close]) != values(lex('string[] array3 in array')):
            continue
        opening = close + 1
        if tokens[opening].value != '{' or tokens[opening + 1].value != 'if':
            raise ValueError('billing-guard-not-first-in-foreach-body')
        guard = opening + 1
        condition_end = matching(tokens, guard + 1)
        condition = tokens[guard + 2:condition_end]
        while condition and condition[0].value == '(' and matching(condition, 0) == len(condition) - 1:
            condition = condition[1:-1]
        if values(condition) != wanted:
            raise ValueError('billing-guard-is-not-the-proved-string-inequality')
        arm = condition_end + 1
        end = statement_end(tokens, arm)
        true_tokens = tokens[arm:end + 1]
        if true_tokens[0].value == '{':
            true_tokens = true_tokens[1:-1]
        if values(true_tokens) != ['return', ';']:
            raise ValueError('billing-guard-arm-is-not-one-void-return')
        if tokens[end + 1].value == 'else':
            raise ValueError('billing-guard-has-else')
        matches.append(true_tokens[0])
    if len(matches) != 1:
        raise ValueError('billing-guard-transfer-not-unique')
    transfer = matches[0]
    return edits(after, [(transfer.start, transfer.end, 'continue')])


def query(body):
    after = inline_foreach_alias(body, 'List<string>[]', 'array2', 'array')
    tokens = lex(after)
    close = len(tokens) - 1
    if tokens[close].value != '}':
        raise ValueError('method-close-not-found')
    final = values(lex('return new Inventory();'))
    first = close - len(final)
    if values(tokens[first:close]) != final:
        raise ValueError('final-return-is-not-exact-current-constructor')
    name = 'recoveredInventoryResult'
    if any(t.value == name for t in tokens):
        raise ValueError('fresh-result-name-collision')
    child = '{\r\nInventory ' + name + ' = new Inventory();\r\nreturn ' + name + ';\r\n}'
    result = edits(after, [(tokens[first].start, tokens[close - 1].end, child)])
    # The early direct return is deliberately retained, not converted.
    if result.count('return new Inventory();') != 1:
        raise ValueError('early-direct-return-was-not-preserved')
    return result


def process(body):
    return inline_foreach_alias(body, 'BillingItem[]', 'array', 'items')


# Standalone public-in-WIP APIs for independent Fixture.Run declarations. These
# functions do not inspect a class/method name, manifest, metadata or any IL.
transform_language = language
transform_billing = billing
transform_query = query
transform_process = process


TARGETS = [
    ('system.form.LanguageForm::Update()->System.Boolean', 'src/Assembly-CSharp/system/form/LanguageForm.cs',
     'public override bool Update()', language, 'joint-smcs-indexed-loop-inline-dispatch-bool-local', False),
    ('system.billing.ui.BillingManager::UpdateBillingItems(System.Boolean)->System.Void', 'src/Assembly-CSharp/system/billing/ui/BillingManager.cs',
     'public virtual void UpdateBillingItems(bool detail)', billing, 'joint-foreach-copy-and-loop-continue', False),
    ('system.billing.ui.StoreBase::QueryInventory(System.Boolean,System.Collections.Generic.List`1<System.String>,System.Collections.Generic.List`1<System.String>)->system.billing.util.Inventory', 'src/Assembly-CSharp/system/billing/ui/StoreBase.cs',
     'public Inventory QueryInventory(bool querySkuDetails, List<string> moreItemSkus, List<string> moreSubsSkus = null)', query, 'joint-foreach-copy-and-final-child-result-local', False),
    ('system.billing.ui.StoreBase::ProcessIncompletePayments()->System.Void', 'src/Assembly-CSharp/system/billing/ui/StoreBase.cs',
     'protected void ProcessIncompletePayments()', process, 'existing-foreach-direct-array-expression', True),
]


def main():
    rows = []
    for key, logical, signature, transform, family, existing in TARGETS:
        raw = (ROOT / logical).read_bytes()
        source = raw.decode('utf-8-sig')
        start, end = method_span(source, signature)
        before = source[start:end]
        after = transform(before)
        if after == before:
            raise ValueError('proposal-is-unchanged')
        row = {'method_id': key, 'logical_path': logical,
               'source_file_sha256': hashlib.sha256(raw).hexdigest(),
               'source_text_sha256': hashlib.sha256(source.encode('utf-8')).hexdigest(),
               'start': len(source[:start].encode('utf-16-le')) // 2,
               'end': len(source[:end].encode('utf-16-le')) // 2,
               'before': before, 'after': after, 'rule': family,
               'source_only': True, 'existing_primitive_not_novel': existing,
               'status': 'PROPOSAL_ONLY_NOT_READY', 'model_proof': 'UNKNOWN',
               'body_oracle': 'UNKNOWN', 'game_compilations': 0,
               'notes': ['No generated class is written or altered by name.',
                         'Whole declaration spans use UTF16 and preserve raw CRLF content.',
                         'Historical zero approved candidates are aggregate model rejections, not compiled rejection of this proposal.']}
        rows.append(row)
    out = {'operation': 'pure-source-foreach-joint-proposals-without-model-or-compilation',
           'status': 'PROPOSAL_ONLY_NOT_READY', 'model_invocations': 0,
           'compiler_invocations': 0, 'fixture_invocations': 0,
           'verifier_invocations': 0, 'source_writes': 0,
           'calculator_sha256': hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
           'patches': rows}
    (OUT / 'proposals.json').write_text(json.dumps(out, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'proposals': len(rows), 'status': out['status'],
                      'source_writes': 0, 'model_invocations': 0, 'compiler_invocations': 0}))


if __name__ == '__main__':
    main()
