"""Private source-only Init2 composition. Import and APIs perform no I/O."""
import re

FAMILY = 'r11-initial-init2-control-composition'
METHOD_ID = 'form.SubForm::Init2()->System.Boolean'


def _once(source, old, new):
    if source.count(old) != 1:
        raise ValueError('unique source anchor required: ' + old)
    return source.replace(old, new, 1)


def _close(source, start):
    """Balanced braces, ignoring C# comments and regular/verbatim literals."""
    if source[start] != '{':
        raise ValueError('opening block expected')
    depth, i, state = 0, start, 'code'
    while i < len(source):
        c = source[i]
        n = source[i:i+2]
        if state == 'line':
            if c in '\r\n': state = 'code'
        elif state == 'comment':
            if n == '*/': state = 'code'; i += 1
        elif state == 'verbatim':
            if n == '""': i += 1
            elif c == '"': state = 'code'
        elif state in ('string', 'char'):
            if c == '\\': i += 1
            elif c == ('"' if state == 'string' else "'"): state = 'code'
        elif n == '//': state = 'line'; i += 1
        elif n == '/*': state = 'comment'; i += 1
        elif n == '@"': state = 'verbatim'; i += 1
        elif c == '"': state = 'string'
        elif c == "'": state = 'char'
        elif c == '{': depth += 1
        elif c == '}':
            depth -= 1
            if depth == 0: return i
        i += 1
    raise ValueError('unbalanced block')


def restore_first_match_and_empty_else(source):
    old = r'if \(TrainingData\.TRA_PLAYER\[i\] != playerData\.id_\)\s*\{\s*continue;\s*\}\s*flag = true;\s*break;'
    matches = list(re.finditer(old, source))
    if len(matches) != 1:
        raise ValueError('unique first inner-loop guard required')
    m = matches[0]
    replacement = 'if (TrainingData.TRA_PLAYER[i] == playerData.id_) { flag = true; break; }'
    source = source[:m.start()] + replacement + source[m.end():]
    anchor = 'if (!flag)'
    if source.count(anchor) != 1: raise ValueError('unique flag guard required')
    start = source.index('{', source.index(anchor) + len(anchor))
    end = _close(source, start)
    if source[start:end+1].strip('{} \t\r\n') != 'TrainingData.TRA_PLAYER[i] = -1;':
        raise ValueError('flag assignment leaf changed')
    if source[end+1:].lstrip().startswith('else'):
        raise ValueError('flag guard already has else')
    return source[:end+1] + ' else { }' + source[end+1:]


EMPTY_GUARDS = (
    ('if (TrainingData.TRA_PLAYER[num3] != -1 && value1_ != num3)',
     'if (TrainingData.TRA_PLAYER[num3] == -1) { } else if (value1_ == num3) { } else'),
    ('if (TrainingData.TRA_PLAYER[num7] != -1)',
     'if (TrainingData.TRA_PLAYER[num7] == -1) { } else'),
    ('else if (type_ != 52)', 'else if (type_ == 52) { } else'),
    ('else if (type_ != 54)', 'else if (type_ == 54) { } else'),
    ('else if (type_ != 56 && type_ != 57)',
     'else if (type_ == 56) { } else if (type_ == 57) { } else'),
    ('if (num13 != 0)', 'if (num13 == 0) { } else'),
    ('if (num18 != 0)', 'if (num18 == 0) { } else'),
    ('if (questMatchData3.state_ != 0)', 'if (questMatchData3.state_ == 0) { } else'),
    ('else if (type_ != 66)', 'else if (type_ == 66) { } else'),
    ('else if (type_ != 69 && type_ != 71 && type_ != 72 && type_ != 74 && type_ == 77)',
     'else if (type_ == 69) { } else if (type_ == 71) { } else if (type_ == 72) { } else if (type_ == 74) { } else if (type_ == 77)'),
    ('if (BitUtil.Check(apdat_.fanBonus_[num28].flag_, 4))',
     'if (!BitUtil.Check(apdat_.fanBonus_[num28].flag_, 4)) { } else'),
)


def restore_empty_guard_arms(source):
    for old, new in EMPTY_GUARDS:
        # All compared values are integral or Boolean. No floating-point
        # relation or .un-bearing floating-point test is inverted here.
        source = _once(source, old, new)
    return source


def restore_named_match_selector(source):
    anchor = 'switch (udat_.GetMatchInfoJoin())'
    if source.count(anchor) != 1: raise ValueError('unique switch selector required')
    name = 'init2MatchJoin'
    if re.search(r'\b' + name + r'\b', source): raise ValueError('fresh selector name required')
    start = source.index(anchor)
    block = source.index('{', start + len(anchor))
    end = _close(source, block)
    content = source[block+1:end]
    case_re = re.compile(r'\s*case ([01]):\s*\{')
    cases, cursor = [], 0
    for expected in (0, 1):
        m = case_re.match(content, cursor)
        if m is None or int(m[1]) != expected:
            raise ValueError('two ordered explicit case blocks required')
        opening = m.end()-1
        closing = _close(content, opening)
        body = content[opening:closing+1]
        if not re.search(r'return Pop\(\);\s*\}$', body):
            raise ValueError('case must end with existing return Pop leaf')
        if re.search(r'\b(?:break|continue|goto)\b', body):
            raise ValueError('non-return switch transfer unsupported')
        cases.append(body)
        cursor = closing+1
    if content[cursor:].strip(): raise ValueError('extra switch section unsupported')
    replacement = ('int ' + name + ' = udat_.GetMatchInfoJoin();\n'
                   'if (' + name + ' == 0) ' + cases[0] + '\n'
                   'else if (' + name + ' == 1) ' + cases[1])
    return source[:start] + replacement + source[end+1:]


def transform_init2_control_composition(method_source):
    """Header agnostic; returns complete source, never touches any assembly."""
    source = restore_first_match_and_empty_else(method_source)
    source = restore_empty_guard_arms(source)
    return restore_named_match_selector(source)
