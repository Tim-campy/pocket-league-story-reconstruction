"""Pure source calculation, limited to the measured normal method."""
import re
METHOD_ID='main.AppData::DrawTrophyEffect(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32,System.Int32)->System.Void'
RULE='r12-trophy-origin-parameter-aliases'

def transform_trophy(source):
    if 'trophyOriginX' in source or 'trophyOriginY' in source:
        raise ValueError('new alias collision')
    frame='int num = (subform_frame - 20) % 66;'
    if source.count(frame)!=1 or source.count('int num7 = px +')!=1 or source.count('int num8 = py +')!=1:
        raise ValueError('unique measured frame/coordinate anchors required')
    null_decl=r'int\[\] array = null;\s*array = resCom2_\.seb\[106\]\.GetSprite\(trophy_id, 0\);'
    if len(re.findall(null_decl,source))!=1:raise ValueError('unique null-then-sprite declaration required')
    source=source.replace(frame,'int trophyOriginX = px;\r\nint trophyOriginY = py;\r\n'+frame)
    return source.replace('int num7 = px +','int num7 = trophyOriginX +').replace('int num8 = py +','int num8 = trophyOriginY +')

def closing(source,start):
    depth=0
    for i in range(start,len(source)):
        if source[i]=='{':depth+=1
        elif source[i]=='}':
            depth-=1
            if depth==0:return i
    raise ValueError('unbalanced braces')
