"""Pure two-expression GamePad source proposal, no I/O or emitted IL."""
import re
METHOD_ID='surface.GamePad::OnTouchEvent(kairo.unity.surface.TouchEvent)->System.Void'
RULE='r12-gamepad-boolean-and-id-conditional-values'

def closing(source,start):
    depth=0
    for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]):
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced source block')

def transform_gamepad(source):
    toggle='visibleTouchRange_ = !visibleTouchRange_;'
    numeric='int num = ((id != 21) ? 1 : 0);'
    if source.count(toggle)!=1 or source.count(numeric)!=1:
        raise ValueError('one exact visible flag toggle and one existing mode conditional required')
    a=source.find('case 14:');b=source.find('case 15:',a)
    if a<0 or b<0 or not a<source.index(toggle)<b:
        raise ValueError('flag occurrence must belong to original case14 section')
    a=source.find('case 21:');b=source.find('case 5:',a)
    if a<0 or b<0 or not a<source.index(numeric)<b:
        raise ValueError('mode occurrence must belong to original case21/22 section')
    source=source.replace(toggle,'visibleTouchRange_ = (visibleTouchRange_ ? false : true);',1)
    return source.replace(numeric,'int num = ((id == 21) ? 0 : 1);',1)
