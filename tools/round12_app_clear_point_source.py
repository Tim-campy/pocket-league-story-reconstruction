"""Two pure, separately gated source proposals; no I/O/IL emission."""
import re

CLEAR_ID='form.SubForm::Draw_clearPoint(kairo.unity.ui.Graphics)->System.Void'
GOAL_ID='game.Map::Draw_goal(kairo.unity.ui.Graphics)->System.Void'
CLEAR_RULE='r12-clear-point-unused-int-and-inner-counter-scope'
GOAL_RULE='r12-map-goal-shared-xyz-before-loop-scopes'

def closing(source,start):
    depth=0
    for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]):
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced block')

def transform_clear_point(source):
    group=re.compile(r'int num2;(?P<gap>\s*)int num3;(?P<gap2>\s*)int num = 105;')
    rows=list(group.finditer(source))
    if len(rows)!=1:raise ValueError('one existing num2/num3/num declaration group required')
    if re.search(r'\brecoveredUnusedLayout\b',source):raise ValueError('unused-local name must be fresh')
    marker=re.compile(r'if \(clearPoint_mode == 4\)\s*\{\s*(?P<assignment>num3 = 0;)')
    scopes=list(marker.finditer(source))
    if len(scopes)!=1:raise ValueError('one mode4 counter-first block required')
    m=scopes[0];bodyend=closing(source,source.index('{',m.start()))
    uses=list(re.finditer(r'\bnum3\b',source))
    if any(not(rows[0].start()<=u.start()<rows[0].end() or m.start()<=u.start()<bodyend)for u in uses):
        raise ValueError('counter may not escape its mode4 block')
    # Same-length name change is not assumed; perform edits from right to left.
    source=source[:m.start('assignment')]+'int '+source[m.start('assignment'):]
    g=rows[0]
    return source[:g.start()]+'int num2;'+g['gap']+'int recoveredUnusedLayout;'+g['gap2']+'int num = 105;'+source[g.end():]

def transform_map_goal(source):
    root='int num2 = 17;'
    if source.count(root)!=1:raise ValueError('one existing num2 initialization required')
    if re.search(r'\b(delegate|yield|goto)\b|=>',source):raise ValueError('captured/escaping declaration scope unsupported')
    for name in ('x','y','depth'):
        if len(re.findall(r'\bint '+name+r'\s*=',source))!=2:
            raise ValueError('each shared scalar must be declared in exactly two original bodies')
    # Bind both declaration groups to the two explicitly indexed goal loops.
    for goal,index in [('GOAL1','j'),('GOAL2','l')]:
        marker='for (int '+index+' = 0; '+index+' < '+goal+'_PARTS_POS.Length; '+index+'++)'
        if source.count(marker)!=1:raise ValueError('one exact goal parts loop required')
        a=source.index(marker);op=source.index('{',a);end=closing(source,op);body=source[op+1:end]
        expected=[
            'int x = camera_pos[0] + '+goal+'_PARTS_POS['+index+'][0] + num;',
            'int y = camera_pos[1] + '+goal+'_PARTS_POS['+index+'][1] + num2;',
            'int depth = camera_pos[1] + '+goal+'_PARTS_POS['+index+'][1] + num2 + '+goal+'_PARTS_GRA['+index+'][2];']
        if any(body.count(text)!=1 for text in expected):raise ValueError('exact original coordinate/depth leaves required')
        if len(re.findall(r'\b(for|while|switch|try)\s*[({]',body)):
            raise ValueError('nested owner or protected region unsupported')
    newline='\r\n'if'\r\n'in source else'\n'
    match=re.search(r'(?P<indent>[\t ]*)int num2 = 17;',source)
    source=source.replace(root,root+newline+match['indent']+'int x, y, depth;',1)
    for name in ('x','y','depth'):
        source=re.sub(r'\bint '+name+r'\s*=',name+' =',source)
    return source
