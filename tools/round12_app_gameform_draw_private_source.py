"""One pure calculated GameForm._draw composition; no I/O or emitted IL."""
import re

METHOD_ID = 'form.GameForm::_draw(kairo.unity.ui.Graphics)->System.Void'
RULE = 'r12-gameform-draw-root-zeros-existing-selector-ifchain'

def closing(source, start):
    depth = 0
    for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]', source[start:]):
        if m[0] == '{': depth += 1
        elif m[0] == '}':
            depth -= 1
            if depth == 0: return start + m.start()
    raise ValueError('unbalanced block')

def root_zeros(source):
    pattern = re.compile(r'int num2;(?P<gap>\s*)int num3;\s*int num = \(num2 = \(num3 = 0\)\);')
    rows = list(pattern.finditer(source))
    if len(rows) != 1: raise ValueError('unique existing three-local initial chain required')
    m = rows[0]
    if len(re.findall(r'int num13 = 0;', source)) != 1:
        raise ValueError('unique later num13 declaration required')
    result = source[:m.start()] + 'int num13, num, num2, num3;' + m['gap'] + 'num13 = num = num2 = num3 = 0;' + source[m.end():]
    return result.replace('int num13 = 0;', 'num13 = 0;', 1)

def existing_selector_ifchain(source):
    anchor = 'switch (matchInfoJoin)'
    if source.count(anchor) != 1: raise ValueError('one matchInfoJoin switch required')
    start = source.index(anchor)
    op = source.index('{', start)
    end = closing(source, op)
    inner = source[op+1:end]
    c0 = re.search(r'\s*case 0:\s*\{', inner)
    if not c0 or inner[:c0.start()].strip(): raise ValueError('case0 must be first')
    b0 = inner.index('{', c0.start()); e0 = closing(inner, b0)
    c1 = re.search(r'\s*case 1:\s*\{', inner[e0+1:])
    if not c1: raise ValueError('case1 required')
    offset = e0+1
    if inner[offset:offset+c1.start()].strip(): raise ValueError('intervening member')
    b1 = inner.index('{', offset+c1.start()); e1 = closing(inner, b1)
    if inner[e1+1:].strip(): raise ValueError('only these two switch sections supported')
    bodies = [inner[b0+1:e0], inner[b1+1:e1]]
    for body in bodies:
        # These exact case leaves contain no nested transfer owner.
        if len(re.findall(r'\bbreak\s*;', body)) != 1 or any(word in body for word in ('for (','while (','switch (','continue;','goto ')):
            raise ValueError('exact case-owned terminal breaks required')
        if not re.search(r'break;\s*$', body): raise ValueError('case break must terminate section')
    bodies = [re.sub(r'break;\s*$', '', body) for body in bodies]
    replacement = 'if (matchInfoJoin == 0) {' + bodies[0] + '} else if (matchInfoJoin == 1) {' + bodies[1] + '}'
    return source[:start] + replacement + source[end+1:]

def transform_gameform_draw(source):
    return existing_selector_ifchain(root_zeros(source))
