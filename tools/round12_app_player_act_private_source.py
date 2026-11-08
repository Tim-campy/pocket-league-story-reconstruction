"""One pure missing-empty-arm source proposal; no I/O or code emission."""
import re
METHOD_ID='game.Player::Act(System.Int32)->System.Boolean'
RULE='r12-player-act-preserve-empty-action-five-arm'

def closing(source,start):
    depth=0
    for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]):
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced source block')

def transform_player_act(source):
    if re.search(r'\bact_id\s*==\s*5\b',source):raise ValueError('action5 already present')
    previous='else if ((act_id == 10)) {'
    marker='} else if ((act_id == 6)) {'
    following='else if ((act_id == 11)) {'
    if source.count(previous)!=1 or source.count(marker)!=1 or source.count(following)!=1:
        raise ValueError('one exact action10/6/11 chain required')
    ten=source.index(previous);m=source.index(marker);eleven=source.index(following)
    if not ten<m<eleven:raise ValueError('original section order required')
    if closing(source,source.index('{',ten))!=m:raise ValueError('action6 must directly follow completed action10 body')
    nl='\r\n'if'\r\n'in source else'\n'
    return source[:m]+'} else if ((act_id == 5)) {'+nl+marker+source[m+len(marker):]
