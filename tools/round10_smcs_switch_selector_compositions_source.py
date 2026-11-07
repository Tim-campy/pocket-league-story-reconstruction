"""Pure private Int32 switch/guard compositions; never compiles or reads files."""
import re


def masked(text):
    pattern = r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\''
    return re.sub(pattern, lambda m: ' ' * len(m[0]), text)


def block(text, start):
    clean, depth = masked(text), 0
    if clean[start] != '{': raise ValueError('expected block')
    for index in range(start, len(text)):
        depth += (clean[index] == '{') - (clean[index] == '}')
        if depth == 0: return start, index + 1
    raise ValueError('unclosed block')


def terminal_break(text):
    match = re.search(r'\bbreak\s*;\s*$', masked(text))
    if not match: raise ValueError('expected switch-owned terminal break')
    return text[:match.start()]


def unwrap_block(text):
    clean = masked(text)
    start = len(clean) - len(clean.lstrip())
    if clean[start:start + 1] == '{':
        _, end = block(text, start)
        if not clean[end:].strip(): return text[start + 1:end - 1]
    return text


def switches(text, selector):
    clean = masked(text)
    return [(m.start(), block(text, m.end())[1], text[m.start():block(text, m.end())[1]])
            for m in re.finditer(r'\bswitch\s*\(\s*' + re.escape(selector) + r'\s*\)\s*', clean)]


def sections(statement, selector):
    clean = masked(statement)
    head = re.match(r'\s*switch\s*\(\s*' + re.escape(selector) + r'\s*\)\s*', clean)
    if not head: raise ValueError('unproven selector expression')
    begin, end = block(statement, head.end())
    if clean[end:].strip(): raise ValueError('unexpected switch sibling')
    labels, depth, index = [], 0, begin + 1
    pattern = re.compile(r'case\s+(-?\d+)\s*:|default\s*:')
    while index < end - 1:
        match = pattern.match(clean, index) if depth == 0 else None
        if match:
            if match[1] is None: raise ValueError('default ownership not implemented')
            labels.append((match.start(), match.end(), int(match[1])))
            index = match.end()
            continue
        depth += (clean[index] == '{') - (clean[index] == '}')
        index += 1
    result, pending = [], []
    for index, (_, start, key) in enumerate(labels):
        stop = labels[index + 1][0] if index + 1 < len(labels) else end - 1
        content = statement[start:stop]
        pending.append(key)
        if masked(content).strip():
            result.append((tuple(pending), content))
            pending = []
    if pending: raise ValueError('empty final case')
    return result


def chain(selector, groups):
    result = []
    for keys, body in groups:
        condition = ' || '.join(selector + ' == ' + str(key) for key in keys)
        result.append(('if' if not result else 'else if') + ' (' + condition + ') {\n' + body + '\n}')
    return '\n'.join(result)


def transform_boot(method_source):
    matches = switches(method_source, 'i')
    if len(matches) != 2: raise ValueError('expected both selector sites, not an isolated old trial')
    result = method_source
    for start, end, statement in reversed(matches):
        groups = sections(statement, 'i')
        if [keys for keys, _ in groups] != [(0,), (1,)]:
            raise ValueError('unproven case inventory/order')
        groups = [(keys, terminal_break(body)) for keys, body in groups]
        result = result[:start] + chain('i', groups) + result[end:]
    return result


def scoped_guard_to_else(content):
    body = terminal_break(unwrap_block(content))
    clean = masked(body)
    head = re.match(r'\s*if\s*\(w\s*<=\s*TBAR_W\[type\]\)\s*', clean)
    if not head: raise ValueError('unexpected width guard')
    begin, end = block(body, head.end())
    then = terminal_break(body[begin + 1:end - 1])
    if re.search(r'\b(break|continue|return|goto)\b', masked(then)):
        raise ValueError('unproven transfer in width arm')
    tail = body[end:]
    if not masked(tail).strip() or re.match(r'\s*else\b', masked(tail)):
        raise ValueError('missing independent positive tail')
    return body[:begin + 1] + then + '\n} else {\n' + tail + '\n}'


def transform_topbar(method_source):
    matches = switches(method_source, 'type')
    if len(matches) != 2: raise ValueError('expected outer and inner dispatch')
    outer_start, outer_end, outer = matches[0]
    groups = sections(outer, 'type')
    if [keys for keys, _ in groups] != [(0,), (1, 3, 5), (2, 4), (6,)]:
        raise ValueError('unproven outer section inventory')
    output = []
    for keys, content in groups:
        if keys == (1, 3, 5):
            content = terminal_break(content)
            inner_matches = switches(content, 'type')
            if len(inner_matches) != 1: raise ValueError('inner dispatch not unique')
            start, end, inner = inner_matches[0]
            inner_groups = sections(inner, 'type')
            if [ks for ks, _ in inner_groups] != [(1,), (5,)]: raise ValueError('inner case evidence differs')
            inner_chain = chain('type', [(ks, terminal_break(b)) for ks, b in inner_groups])
            content = content[:start] + inner_chain + content[end:]
            # Original branches compare1, then5, then3. Preserve that order.
            output.append(((1, 5, 3), content))
        else:
            output.append((keys, scoped_guard_to_else(content)))
    return method_source[:outer_start] + chain('type', output) + method_source[outer_end:]


def extract_method(source, name, parameter_count=None):
    clean = masked(source)
    matches = list(re.finditer(r'(?m)^[ \t]*(?:public|private|protected|internal)[^\r\n;{}]*\b'
                               + re.escape(name) + r'\s*\((?P<params>[^\r\n]*)\)\s*(?=\{)', clean))
    if parameter_count is not None:
        matches = [m for m in matches if (m['params'].count(',') + 1 if m['params'].strip() else 0) == parameter_count]
    if len(matches) != 1: raise ValueError('method signature must be unique')
    match = matches[0]
    _, end = block(source, match.end())
    return match.start(), end, source[match.start():end]
