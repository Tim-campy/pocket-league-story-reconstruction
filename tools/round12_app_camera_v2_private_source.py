"""Pure, bounded source producer for Camera's control shape; no compilation."""
import re

RULE = 'r12-camera-runtime-if-selector-and-explicit-guards'
METHOD_ID = 'game.Camera::Update_pos()->System.Void'

def close(source, start):
    depth = 0
    for match in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]', source[start:]):
        if match[0] == '{': depth += 1
        elif match[0] == '}':
            depth -= 1
            if depth == 0: return start + match.start()
    raise ValueError('Unbalanced source')

def once(source, before, after):
    if source.count(before) != 1: raise ValueError('Unique source anchor required: '+before[:100])
    return source.replace(before, after, 1)

def restore_selector_chain(source):
    if source.count('switch (0)') != 1: raise ValueError('One folded switch required')
    start = source.index('switch (0)')
    opening = source.index('{', start)
    ending = close(source, opening)
    switch_body = source[opening+1:ending]
    match = re.fullmatch(r'\s*case 0:(?P<zero>[\s\S]*?)\s*break;\s*case 1:(?P<one>[\s\S]*?)\s*break;\s*', switch_body)
    if not match: raise ValueError('Exactly two bounded constant-switch cases required')
    replacement = 'float recoveredUnusedSingleA;\nint recoveredSelector = 0;\nfloat recoveredUnusedSingleB;\nif (recoveredSelector == 0)\n{' + match['zero'] + '\n}\nelse if (recoveredSelector == 1)\n{' + match['one'] + '\n}'
    return source[:start] + replacement + source[ending+1:]

def transform_v1(source):
    if source.count('public void Update_pos()') != 1: raise ValueError('One method required')
    marker = '\t\t\telse if (udat_.gm_.state_ == 3 && '
    if source.count(marker) != 1: raise ValueError('Unique throw-in branch required')
    start = source.index(marker)
    opening = source.index('{', start)
    ending = close(source, opening)
    head = source[start:opening].replace('else if', 'if', 1)
    source = source[:start] + '\t\t\telse\n\t\t\t{\n' + head + source[opening:ending] + '\treturn;\n\t\t\t}' + source[ending+1:]
    source = once(source, '\t\t\telse if (mode_ == 0)', '\t\t\tif (mode_ == 0)')
    # Close the new enclosing else immediately before the method's own close.
    method_open = source.index('{', source.index('public void Update_pos()'))
    # The source has one extra open brace temporarily, so identify the final
    # method close from the already bounded method text, not by guessing nesting.
    last = source.rfind('}')
    source = source[:last] + '\t}\n\t\t' + source[last:]
    source = restore_selector_chain(source)
    old = '''if (flg_disp)
\t\t\t\t\t{
\t\t\t\t\t\tCollision((int)cursor_pos.x_, (int)cursor_pos.y_);
\t\t\t\t\t}'''
    source = once(source, old, '''if (!flg_disp) { return; }
Collision((int)cursor_pos.x_, (int)cursor_pos.y_);''')
    old = '''else
\t\t\t{
\t\t\t\tif (mode_ != 2 || apdat_.frmSoccer_.state_ == 8)'''
    source = once(source, old, '''else if (mode_ == 2)
\t\t\t{
\t\t\t\tif (apdat_.frmSoccer_.state_ == 8)''')
    call = 'GameUtil.ChCrossLine(chPos[0], chPos[1], chPos[2], chPos[3])'
    source = once(source, 'if ('+call+')', 'bool recoveredIntersection = '+call+';\nif (recoveredIntersection)')
    assert close(source, method_open) == source.rfind('}')
    return source

def extract(source):
    start = source.index('public void Update_pos()')
    end = close(source, source.index('{', start)) + 1
    return start, end, source[start:end]


RULE='r12-camera-runtime-selector-and-common-goal-else'
CONDITION='if (udat_.gm_.state_ == 8 && udat_.gm_.stateCnt_ >= GameManager.WAIT_GOAL[0])'

def enclosing_open(source,index):
 stack=[]
 for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[:index]):
  if m[0]=='{':stack.append(m.start())
  elif m[0]=='}':stack.pop()
 if not stack:raise ValueError('No containing block')
 return stack[-1]

def common_goal_else(source):
 matches=list(re.finditer(re.escape(CONDITION),source))
 if len(matches)!=2:raise ValueError('Exactly two existing goal conditions required')
 start=matches[1].start();opening=source.index('{',start);ending=close(source,opening);parent=enclosing_open(source,start);parent_end=close(source,parent)
 body=source[opening+1:ending]
 pattern=r'\s*pos\.x_ \+= num3;\s*pos\.y_ \+= num4;\s*return;\s*'
 if not re.fullmatch(pattern,body):raise ValueError('Only the two position additions and return may form this arm')
 positive=re.sub(r'return;\s*$','',body)
 tail=source[ending+1:parent_end]
 if not tail.strip():raise ValueError('A following sibling body is required')
 return source[:opening+1]+positive+'}\nelse\n{'+tail+'\n}'+source[parent_end:]

def transform(source):
    # The bounded method may arrive with CRLF. Line endings are trivia, and the
    # external patch still authenticates the exact original UTF16 span.
    source=source.replace('\r\n','\n')
    return common_goal_else(transform_v1(source))
