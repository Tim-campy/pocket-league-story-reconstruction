"""Two independent pure declaration/reuse compositions; no binary I/O."""
import re

SCORE_ID='form.SoccerForm::UpdateOffscreenTopBar(kairo.unity.ui.Graphics,System.Boolean)->System.Void'
GRAPH_ID='game.UserData::DrawGraph_menuElement(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32[],System.Int32[],System.Int32)->System.Void'
SCORE_RULE='r12-soccer-topbar-root-anchors-and-shared-score-xy'
GRAPH_RULE='r12-menu-graph-root-shared-four-coordinates'

def closing(source,start):
    depth=0
    for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]):
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced block')

def transform_score(source):
    group=re.compile(r'global::data.TeamData teamData2;\s*global::surface.GameView instance;\s*int topBarOffN;\s*int num;\s*TeamData teamData =')
    matches=list(group.finditer(source))
    if len(matches)!=1:raise ValueError('unique prior hoisted TeamData/GameView/int declaration group required')
    if re.search(r'\b(delegate|yield|goto)\b|=>',source):raise ValueError('captured/escaping declarations unsupported')
    expected=['teamData2 = apdat_.tData_[udat_.gm_.teamId_[1]];',
              'instance = GameView.GetInstance();','topBarOffN = GetTopBarOffN();',
              'num = AppData.ClampMin((instance.GetGameWidth() - (240 + topBarOffN * 2)) / 2, 0);']
    for text in expected:
        if source.count(text)!=1:raise ValueError('exact initialized receiver/offset/cursor required')
    for name in ['num2','num3','num4','num5']:
        if source.count('int '+name+' = num;')!=1:raise ValueError('exact four original bar anchors required')
    if source.count('int num6 = 0;')!=1:raise ValueError('exact score-y declaration required')
    for text in ['int x = instance.GetGameWidth() / 2 - 13;','int x = instance.GetGameWidth() / 2 + 13;']:
        if source.count(text)!=1:raise ValueError('both original score-x assignments required')
    m=matches[0]
    nl='\r\n'if'\r\n'in source else'\n'
    source=source[:m.start()]+'int num2, num5, num3, num4;'+nl+'TeamData teamData ='+source[m.end():]
    for name,typ in [('teamData2','TeamData'),('instance','GameView'),('topBarOffN','int'),('num','int')]:
        # Initialized forms retain the exact old evaluation points.
        match=next(t for t in expected if t.startswith(name+' ='))
        source=source.replace(match,typ+' '+match,1)
    for name in ['num2','num3','num4','num5']:
        source=source.replace('int '+name+' = num;',name+' = num;',1)
    source=source.replace('int num6 = 0;','int x, num6;'+nl+'x = num6 = 0;',1)
    source=source.replace('int x = instance.GetGameWidth() / 2 - 13;','x = instance.GetGameWidth() / 2 - 13;',1)
    return source.replace('int x = instance.GetGameWidth() / 2 + 13;','x = instance.GetGameWidth() / 2 + 13;',1)

def transform_graph(source):
    if re.search(r'\b(delegate|yield|goto)\b|=>',source):raise ValueError('captured/escaping declaration unsupported')
    first='for (int i = 0; i < MENUGRAPH_POINTPOS.Length; i++)'
    if source.count(first)!=1:raise ValueError('unique first empty-loop anchor required')
    for name in ['num','num2','num3','num4']:
        if len(re.findall(r'\bint '+name+r'\s*=',source))!=3:raise ValueError('three original coordinate declaration groups required')
    # New parent declaration precedes all child loop indices. Every initializer stays put.
    nl='\r\n'if'\r\n'in source else'\n'
    source=source.replace(first,'int num, num2, num3, num4;'+nl+first,1)
    for name in ['num','num2','num3','num4']:
        source=re.sub(r'\bint '+name+r'\s*=',name+' =',source)
    # All types are Int32: bind the later outputs ahead of the limit/index too.
    if source.count('int c1max = 25;')!=1 or source.count('int x = GameUtil.RateConvert(')!=1 or source.count('int x2 = GameUtil.RateConvert(')!=1:
        raise ValueError('one existing conversion-limit and two original output declarations required')
    source=source.replace('int c1max = 25;','int x, x2;'+nl+'int c1max = 25;',1)
    source=source.replace('int x = GameUtil.RateConvert(','x = GameUtil.RateConvert(',1)
    source=source.replace('int x2 = GameUtil.RateConvert(','x2 = GameUtil.RateConvert(',1)
    return source
