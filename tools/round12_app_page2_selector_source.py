"""Pure finite three-method declaration/control proposals; no I/O/IL emission."""
import re

SAVING_ID='game.TPlayer::CollisionSaving(game.TBall)->System.Void'
PAGE_ID='surface.GameView::OnTouchDlgPage(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void'
PAGE2_ID='surface.GameView::OnTouchDlgPage2(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void'
SAVING_RULE='r12-saving-named-selector-unused-float-layout'
PAGE_RULE='r12-dialog-page-shared-effect-direct-inner-dispatch'
PAGE2_RULE='r12-dialog-page2-shared-effect-direct-inner-dispatch-empty-holes'

def closing(source,start):
    depth=0
    for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]):
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced source block')

def strip_owned_break(text):
    text=text.strip()
    if text.startswith('{'):
        end=closing(text,0)
        if text[end+1:].strip():raise ValueError('unexpected leaves after case block')
        text=text[1:end]
    if len(re.findall(r'\bbreak\s*;',text))!=1 or not re.search(r'break;\s*$',text):
        raise ValueError('one terminal case-owned break required')
    if re.search(r'\b(for|while|switch|try|goto|continue)\b',text):
        raise ValueError('nested transfer owner unsupported')
    return re.sub(r'break;\s*$','',text)

def parts(source,anchor,expected):
    if source.count(anchor)!=1:raise ValueError('one exact switch required')
    start=source.index(anchor);op=source.index('{',start);end=closing(source,op)
    inner=source[op+1:end]
    labels=list(re.finditer(r'\b(?:case\s+(\d+)|(?P<default>default))\s*:',inner))
    actual=[m[1]if not m['default']else'default'for m in labels]
    if actual!=expected or inner[:labels[0].start()].strip():raise ValueError('exact section order required')
    bodies=[strip_owned_break(inner[m.end():labels[i+1].start()if i+1<len(labels)else len(inner)])for i,m in enumerate(labels)]
    return start,end+1,bodies

def page_common(source,second=False):
    outer='switch (type)';inner='switch (v)'
    if source.count(outer)!=1:raise ValueError('one original outer switch required')
    if re.search(r'\b(delegate|yield|goto)\b|=>',source):raise ValueError('escaping effect declaration unsupported')
    start,end,bodies=parts(source,inner,['16','18','17','19'])
    if len(re.findall(r'TouchEffect touchEffect\s*=',source))!=4:
        raise ValueError('four original effect declarations required')
    axes=[('2','vx_','-10f'),('3','vx_','10f'),('14','vy_','-10f'),('15','vy_','10f')]
    for body,(kind,axis,speed)in zip(bodies,axes):
        if 'TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, '+kind+', component);'not in body:
            raise ValueError('effect initializer leaf mismatch')
        if 'touchEffect.'+axis+' = '+speed+';'not in body:raise ValueError('velocity leaf mismatch')
    bodies=[body.replace('TouchEffect touchEffect =','touchEffect =',1)for body in bodies]
    chain='if (v == 16) {'+bodies[0]+'} else if (v == 18) {'+bodies[1]+'} else if (v == 17) {'+bodies[2]+'} else if (v == 19) {'+bodies[3]+'}'
    source=source[:start]+chain+source[end:]
    nl='\r\n'if'\r\n'in source else'\n'
    source=source.replace(outer,'TouchEffect touchEffect;'+nl+outer,1)
    if second:
        holes=re.compile(r'case 3:\s*case 4:\s*case 7:\s*case 8:\s*case 9:\s*break;')
        matches=list(holes.finditer(source))
        if len(matches)!=1:raise ValueError('one exact empty-hole section required')
        source=source[:matches[0].start()]+source[matches[0].end():]
    return source

def transform_page(source):return page_common(source,False)
def transform_page2(source):return page_common(source,True)

def transform_saving(source):
    if any(name in source for name in['recoveredSavingChoice','recoveredUnusedX2','recoveredUnusedY2','recoveredUnusedX','recoveredUnusedY']):
        raise ValueError('new diagnostic local names must be fresh')
    start,end,bodies=parts(source,'switch (GameUtil.Random(3))',['2','1','default'])
    if bodies[1].count('float num2 =')!=1 or bodies[2].count('float num =')!=1:
        raise ValueError('two exact speed initializer anchors required')
    nl='\r\n'if'\r\n'in source else'\n'
    bodies[1]=bodies[1].replace('float num2 =','float recoveredUnusedX2, recoveredUnusedY2;'+nl+'float num2 =',1)
    bodies[2]=bodies[2].replace('float num =','float recoveredUnusedX, recoveredUnusedY;'+nl+'float num =',1)
    chain='int recoveredSavingChoice = GameUtil.Random(3);'+nl+'if (recoveredSavingChoice == 2) {'+bodies[0]+'} else if (recoveredSavingChoice == 1) {'+bodies[1]+'} else {'+bodies[2]+'}'
    return source[:start]+chain+source[end:]
