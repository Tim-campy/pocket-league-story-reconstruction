"""Pure, bounded two-method source producer; no IO or compiler invocation."""
import re
FAMILY='r12-script-named-dispatch-if-chain-and-loop-exits'
IDS={'game':'form.GameForm::Script()->System.Void','soccer':'form.SoccerForm::Script()->System.Void'}

def closing(text,start):
    depth=0
    for i in range(start,len(text)):
        if text[i]=='{':depth+=1
        elif text[i]=='}':
            depth-=1
            if depth==0:return i
    raise ValueError('unbalanced block')

def pieces(source,kind):
    if any(x in source for x in ['"',"'",'//','/*','recoveredScriptKind']):
        raise ValueError('unsupported literal/comment/name collision')
    anchor='switch (firstScript[0])'
    if source.count(anchor)!=1 or source.count('while (script_.Size() > 0)')!=1:
        raise ValueError('unique measured dispatch/loop required')
    st=source.index(anchor);op=source.index('{',st);end=closing(source,op)+1
    labels=list(re.finditer(r'(?m)^\s*(?:case ([0-9]+)|(?P<default>default))\s*:',source[op+1:end-1]))
    keys=[int(m[1])if m[1]else'default'for m in labels]
    expected=list(range(6))if kind=='soccer'else list(range(4))
    if keys!=[1,0]+expected[2:]+['default']:
        raise ValueError('case order/inventory differs')
    region=source[op+1:end-1];body={}
    for i,m in enumerate(labels):body[keys[i]]=region[m.end():labels[i+1].start()if i+1<len(labels)else len(region)].strip()
    if body['default']!='return;':raise ValueError('unexpected default transfer')
    if kind=='game':
        tail=source[end:source.rfind('}')]
        if not re.fullmatch(r'\s*script_\.RemoveElementAt\(0\);\s*}\s*',tail):raise ValueError('unexpected loop tail')
    else:
        if not re.fullmatch(r'\s*}\s*',source[end:source.rfind('}')]):raise ValueError('unexpected loop tail')
    return st,end,body

def terminal(body,old,new=''):
    if not body.endswith(old):raise ValueError('unexpected terminal transfer')
    return body[:-len(old)].rstrip()+new

def transform(source,kind,dispatch_only=False):
    st,end,b=pieces(source,kind)
    branches=[]
    for key in ([1,0,2,3,4,5]if kind=='soccer'else[1,0,2,3]):
        body=b[key]
        if key in[1,0,3]:body=terminal(body,'return;' if not dispatch_only else 'return;','return;'if dispatch_only else '')
        elif key==2:
            body=terminal(body,'break;','continue;')
            if kind=='game':
                if body.strip()!='continue;':raise ValueError('unexpected case2 body')
                body='script_.RemoveElementAt(0);\r\ncontinue;'
        elif key==4:
            if body.count('break;')!=1 or body.count('return;')!=1:raise ValueError('case4 owners unsupported')
            body=body.replace('break;','continue;')
            if not dispatch_only:body=terminal(body,'return;')
        elif key==5:body=terminal(body,'break;','continue;')
        branches.append(('if'if not branches else'else if')+' (recoveredScriptKind == '+str(key)+') {\r\n'+body+'\r\n}')
    if dispatch_only:branches.append('else { return; }')
    else:branches.append('break;')
    replacement='int recoveredScriptKind = firstScript[0];\r\n'+'\r\n'.join(branches)
    # All original common leaves are represented in the matching case2 body.
    close_loop=source.rfind('}',0,source.rfind('}'))
    return source[:st]+replacement+'\r\n'+source[close_loop:]

def transform_game_script(source):return transform(source,'game')
def transform_soccer_script(source):return transform(source,'soccer')
