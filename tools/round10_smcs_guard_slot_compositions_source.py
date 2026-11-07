"""Private pure source compositions. No IO, compiler, model, or target writes."""
import re


def masked(text):
    # Preserve positions while hiding literals/comments from ownership checks.
    pattern = r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\''
    return re.sub(pattern, lambda m: ' ' * len(m[0]), text)


def block(text, start):
    clean = masked(text)
    if clean[start] != '{':
        raise ValueError('expected lexical block')
    depth = 0
    for index in range(start, len(text)):
        depth += (clean[index] == '{') - (clean[index] == '}')
        if depth == 0:
            return start, index + 1
    raise ValueError('unclosed block')


def one(pattern, text, flags=0):
    found = list(re.finditer(pattern, text, flags))
    if len(found) != 1:
        raise ValueError('expected unique anchor: ' + pattern)
    return found[0]


def replace_one(text, before, after):
    if text.count(before) != 1:
        raise ValueError('missing or duplicate exact anchor: ' + before)
    return text.replace(before, after)


def remove_terminal(text, keyword):
    match = re.search(r'\b' + keyword + r'\s*;\s*$', masked(text))
    if not match:
        raise ValueError('required terminal ' + keyword)
    return text[:match.start()]


def switch_to_parameter_chain(statement, parameter='id'):
    clean = masked(statement)
    head = re.match(r'\s*switch\s*\(\s*' + re.escape(parameter) + r'\s*\)\s*', clean)
    if not head:
        raise ValueError('switch selector must be the proven Int32 parameter')
    begin, end = block(statement, head.end())
    if clean[end:].strip():
        raise ValueError('unexpected sibling after switch')
    depth = 0
    labels = []
    label_pattern = re.compile(r'case\s+(-?[0-9]+)\s*:|default\s*:')
    index = begin + 1
    while index < end - 1:
        if depth == 0:
            match = label_pattern.match(clean, index)
            if match:
                labels.append((match.start(), match.end(), match[1]))
                index = match.end()
                continue
        depth += (clean[index] == '{') - (clean[index] == '}')
        index += 1
    if [label[2] for label in labels] != ['-1', '-2', None]:
        raise ValueError('case order/value evidence not satisfied')
    result = []
    for index, (_, start, key) in enumerate(labels):
        stop = labels[index + 1][0] if index + 1 < len(labels) else end - 1
        body = remove_terminal(statement[start:stop], 'break')
        head = ('if (' + parameter + ' == ' + key + ')') if index == 0 else (
            'else if (' + parameter + ' == ' + key + ')' if key is not None else 'else')
        result.append(head + ' {' + body + '\n}')
    return '\n'.join(result)


def transform_sp(method_source):
    text = method_source
    for low, high in ((2, 6), (4, 8), (6, 11), (1, 3)):
        before = '(!flag) ? ' + str(low) + ' : ' + str(high)
        after = 'flag ? ' + str(high) + ' : ' + str(low)
        text = replace_one(text, before, after)
    outer = one(r'\bif\s*\(\s*id\s*>=\s*0\s*\)\s*', masked(text))
    begin, end = block(text, outer.end())
    body = remove_terminal(text[begin + 1:end - 1], 'return')
    inner = one(r'\bif\s*\(\s*num3\s*==\s*-1\s*\)\s*', masked(body))
    ib, ie = block(body, inner.end())
    then = remove_terminal(body[ib + 1:ie - 1], 'return')
    tail = body[ie:]
    if not tail.strip() or re.match(r'\s*else\b', masked(tail)):
        raise ValueError('missing independent else tail')
    body = body[:ib + 1] + then + '\n} else {' + tail + '\n}'
    suffix = text[end:]
    switch = re.match(r'\s*switch\s*\(', masked(suffix))
    if not switch:
        raise ValueError('outer else switch not adjacent')
    switch_begin = suffix.index('{', switch.start())
    _, switch_end = block(suffix, switch_begin)
    chain = switch_to_parameter_chain(suffix[:switch_end])
    # Outer-positive path now joins method exit; inner branches join first.
    return text[:begin + 1] + body + '\n} else {\n' + chain + '\n}' + suffix[switch_end:]


def transform_billing(method_source):
    # Slot12 is an existing Boolean in the original, not a guessed new variable.
    pattern = (r'if\s*\(\s*(?P<flag>\w+)\s*\|\|\s*\(\s*!\s*(?P<item>\w+)\.recovery_'
               r'\s*&&\s*!\s*Consume\(\s*(?P=item)\s*\)\s*\)\s*\)\s*\{\s*continue\s*;\s*\}')
    match = one(pattern, method_source)
    local = 'recoveredConsumeResult'
    if re.search(r'\b' + local + r'\b', masked(method_source)):
        raise ValueError('synthetic local name collision')
    item, flag = match['item'], match['flag']
    after = ('if (' + flag + ') { continue; }\nif (!' + item + '.recovery_) {\n'
             'bool ' + local + ' = Consume(' + item + ');\n'
             'if (!' + local + ') { continue; }\n}')
    return method_source[:match.start()] + after + method_source[match.end():]


def transform_banner(method_source):
    pattern = (r'\{\s*bool\s+(?P<local>\w+)\s*=\s*\(image_\s*==\s*null\s*&&\s*i\s*!=\s*0\)'
               r'\s*\|\|\s*i\s*==\s*current_\s*\|\|\s*jarInflater\.GetSize\(FILENAMES\[i\]\)\s*==\s*-1\s*;'
               r'\s*if\s*\(\s*(?P=local)\s*\)\s*\{\s*continue\s*;\s*\}\s*\}')
    match = one(pattern, method_source)
    if len(re.findall(r'\b' + re.escape(match['local']) + r'\b', masked(method_source))) != 2:
        raise ValueError('condition local has additional uses')
    after = ('if (image_ == null && i != 0) { continue; }\n'
             'if (i == current_) { continue; }\n'
             'if (jarInflater.GetSize(FILENAMES[i]) == -1) { continue; }')
    return method_source[:match.start()] + after + method_source[match.end():]


def extract_method(source, name):
    clean = masked(source)
    match = one(r'(?m)^[ \t]*(?:public|private|protected|internal)[^\r\n;{}]*\b' + re.escape(name) + r'\s*\([^\r\n]*\)\s*(?=\{)', clean)
    begin, end = block(source, match.end())
    return match.start(), end, source[match.start():end]
