"""Private header-agnostic C# producers. No I/O or compiler/model calls."""
import re
PAUSE_RULE='r11-pause-notify-receivers-and-branch-scopes'
TRANSPOSE_RULE='r11-array-initializer-last-use-stack-consumption'

def _once(source,old,new):
    if source.count(old)!=1:raise ValueError('unique source anchor required: '+old)
    return source.replace(old,new,1)

def closing(source,start):
    depth=0;state='code';i=start
    if source[start]!='{':raise ValueError('opening block expected')
    while i<len(source):
        c=source[i];n=source[i:i+2]
        if state=='line':
            if c in '\r\n':state='code'
        elif state=='comment':
            if n=='*/':state='code';i+=1
        elif state in ['string','char']:
            if c=='\\':i+=1
            elif c==('"'if state=='string'else"'"):state='code'
        elif n=='//':state='line';i+=1
        elif n=='/*':state='comment';i+=1
        elif c=='"':state='string'
        elif c=="'":state='char'
        elif c=='{':depth+=1
        elif c=='}':
            depth-=1
            if not depth:return i
        i+=1
    raise ValueError('unbalanced source block')

def transform_pause_notify_and_else(method_source):
    for name in ['pauseNotify','resumeNotify']:
        if re.search(r'\b'+name+r'\b',method_source):raise ValueError('fresh notification local required')
    anchor='if (pauseStatus)'
    if method_source.count(anchor)!=1:raise ValueError('one pause branch required')
    start=method_source.index(anchor);opening=method_source.index('{',start+len(anchor));end=closing(method_source,opening)
    root=method_source.index('{');last=closing(method_source,root)
    truebody=method_source[opening+1:end];tail=method_source[end+1:last]
    if not re.search(r'OnSuspend\(\);\s*return;\s*$',truebody):raise ValueError('existing suspend/return leaf required')
    if not tail.lstrip().startswith('NotifyManager.GetInstance()?.DoCancel();')or not re.search(r'OnResume\(\);\s*$',tail):
        raise ValueError('original unwrapped resume tail required')
    if len(re.findall(r'\bGetFormManager\(\)',method_source))!=2 or method_source.count('catch (Exception)')!=2:
        raise ValueError('two original manager loops and catches required')
    source=method_source[:end+1]+' else {'+tail+'\n}'+method_source[last:]
    source=_once(source,'NotifyManager.GetInstance()?.DoNotify();',
        'NotifyManager pauseNotify = NotifyManager.GetInstance();\nif ((object)pauseNotify != null) { pauseNotify.DoNotify(); }')
    source=_once(source,'NotifyManager.GetInstance()?.DoCancel();',
        'NotifyManager resumeNotify = NotifyManager.GetInstance();\nif ((object)resumeNotify != null) { resumeNotify.DoCancel(); }')
    return source

TRANSPOSE_ORDER=(0,4,8,12,1,5,9,13,2,6,10,14,3,7,11,15)
def transform_transpose_last_array_read(method_source):
    matches=list(re.finditer(r'float\[\] array = new float\[16\]\s*\{',method_source))
    if len(matches)!=1:raise ValueError('one complete array initializer required')
    m=matches[0];end=closing(method_source,m.end()-1)
    semi=end+1
    while semi<len(method_source)and method_source[semi].isspace():semi+=1
    if semi>=len(method_source)or method_source[semi]!=';':raise ValueError('declaration terminator required')
    leaves=[s.strip()for s in method_source[m.end():end].split(',')]
    if leaves!=['mtx_['+str(i)+']'for i in TRANSPOSE_ORDER]:raise ValueError('exact existing transposition leaves required')
    root=method_source.index('{');last=closing(method_source,root)
    if method_source[root+1:m.start()].strip()or method_source[semi+1:last].strip():raise ValueError('unexpected prior or following source statement')
    nl='\r\n'if'\r\n'in method_source else'\n'
    replacement='float[] array = new float[16];'+nl+nl.join('array['+str(n)+'] = '+leaf+';'for n,leaf in enumerate(leaves))
    return method_source[:m.start()]+replacement+method_source[semi+1:]
