"""Pure one-method unary-negation plus final-empty-else composition."""
import re
METHOD_ID='game.Team::SetPositionKickOff()->System.Void'
RULE='r12-kickoff-final-negation-and-explicit-empty-else'

def brace_tokens(source):
    return re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source)

def closing(source,start):
    depth=0
    for m in brace_tokens(source[start:]):
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced source block')

def transform_kickoff(source):
    expr='0f - FORMATION_KICKOFF[1].z_'
    guard='if (udat_.gm_.gameState_ == team_id_)'
    if source.count(expr)!=1 or source.count(guard)!=1:
        raise ValueError('one exact final z subtraction and game-state guard required')
    at=source.index(guard)
    depth=0
    for m in brace_tokens(source[:at]):
        if m[0]=='{':depth+=1
        elif m[0]=='}':depth-=1
    if depth!=1:raise ValueError('guard must belong directly to method body')
    op=source.index('{',at);end=closing(source,op)
    method_end=closing(source,source.index('{'))
    if source[end+1:method_end].strip():raise ValueError('guard must be the last command with no existing else')
    if not op<source.index(expr)<end:raise ValueError('subtraction must be inside this final guard')
    tail='udat_.ball_.ReceiveBall(players_[players_.Length - 1]);'
    if source.count(tail)!=1 or not source[op+1:end].rstrip().endswith(tail):
        raise ValueError('original final receive leaf must be preserved')
    # Apply the later edit first; neither replacement touches another command.
    source=source[:end+1]+' else { }'+source[end+1:]
    return source.replace(expr,'-(FORMATION_KICKOFF[1].z_)',1)
