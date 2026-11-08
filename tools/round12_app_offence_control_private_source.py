"""Pure one-method removal of an artificial bool plus two original exit shapes."""
import re
METHOD_ID='game.Team::Update_coatEvaNewOffence()->System.Void'
RULE='r12-offence-direct-team-test-and-two-empty-positive-exits'

def closing(source,start):
    depth=0
    for m in re.finditer(r'//[^\r\n]*|/\*[\s\S]*?\*/|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[{}]',source[start:]):
        if m[0]=='{':depth+=1
        elif m[0]=='}':
            depth-=1
            if depth==0:return start+m.start()
    raise ValueError('unbalanced block')

def remove_artificial_bool(source):
    name='recoveredCondition_27548'
    pattern=re.compile(r'\{\s*bool '+name+r' = team_id_ == 0;\s*if \('+name+r'\)')
    matches=list(pattern.finditer(source))
    if len(matches)!=1 or len(re.findall(r'\b'+name+r'\b',source))!=2:
        raise ValueError('one exact artificial bool wrapper with no escaping use required')
    m=matches[0];end=closing(source,m.start());a=source.index('{',m.end());b=closing(source,a)
    otherwise=re.match(r'\s*else\s*\{',source[b+1:end])
    if not otherwise:raise ValueError('original team true/false branches required')
    false_open=b+1+otherwise.end()-1;false_end=closing(source,false_open)
    if source[false_end+1:end].strip():raise ValueError('bool wrapper must contain only this if/else')
    return source[:m.start()]+'if (team_id_ == 0)'+source[m.end():end]+source[end+1:]

def restore_area_guards(source):
    anchor='if (ChPAreaLineOver())'
    if source.count(anchor)!=2:raise ValueError('two exact post-fill area guards required')
    edits=[]
    for m in re.finditer(re.escape(anchor),source):
        op=source.index('{',m.end());end=closing(source,op)
        if re.match(r'\s*else\b',source[end+1:]):raise ValueError('guard already has an else')
        body=source[op:end+1]
        if body.count('for (int ')!=2 or 'DEAFULT_FORMATION_EVA_PENALTYAREA'not in body or 'coatEva_new_offence['not in body:
            raise ValueError('exact two-loop penalty overwrite body required')
        if re.search(r'\b(break|continue|goto|return)\b',body):raise ValueError('escaping transfer in overwrite body unsupported')
        edits.append((m.start(),op,'if (!ChPAreaLineOver()) { } else '))
    for start,end,replacement in reversed(edits):source=source[:start]+replacement+source[end:]
    return source

def transform_offence(source):
    return restore_area_guards(remove_artificial_bool(source))
