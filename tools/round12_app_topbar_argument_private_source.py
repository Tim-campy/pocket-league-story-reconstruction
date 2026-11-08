"""Pure source patch for one measured final argument; retains the receiver alias."""
import re
METHOD_ID='form.SoccerForm::Draw_topBar(kairo.unity.ui.Graphics)->System.Void'
RULE='r12-topbar-final-call-original-graphics-argument'

def transform_topbar(source):
    declaration='global::kairo.unity.ui.Graphics gAlias = g;'
    anchor='UpdateOffscreenTopBar(gAlias, false);'
    if source.count(declaration)!=1 or source.count(anchor)!=1:
        raise ValueError('unique alias/call anchors required')
    if len(re.findall(r'\bgAlias\s*=',source))!=1 or re.search(r'\b(?:ref|out)\s+(?:gAlias|g)\b',source):
        raise ValueError('alias/argument may be mutated by this source')
    if re.search(r'(?<![\w.])g\s*=',source):raise ValueError('argument reassigned')
    return source.replace(anchor,'UpdateOffscreenTopBar(g, false);')

def closing(source,start):
    depth=0
    for i in range(start,len(source)):
        if source[i]=='{':depth+=1
        elif source[i]=='}':
            depth-=1
            if depth==0:return i
    raise ValueError('unbalanced braces')
