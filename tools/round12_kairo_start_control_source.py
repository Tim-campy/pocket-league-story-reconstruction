"""Pure parent-source rewrite for the existing Start lambda; no I/O."""
import re
RULE='r12-start-dialog-positive-retry-arm'
GENERATED_ID='kairo.unity.system.KairoBase/<>c::<Start>b__55_0()->System.Object'
PARENT_ID='kairo.unity.system.KairoBase::Start()->System.Void'

def closing(source,start):
    depth=0;state='code';i=start
    if source[start]!='{':raise ValueError('opening source block required')
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

def prove_retry_loop_owner(source,guard_start,guard_end):
    loops=list(re.finditer(r'while\s*\(true\)',source))
    if len(loops)!=1:raise ValueError('one original while(true) owner required')
    lo=source.index('{',loops[0].end());le=closing(source,lo)
    if not(lo<guard_start<guard_end<le):raise ValueError('retry guard must belong to original loop')
    tr=re.search(r'\btry\s*\{',source[lo+1:guard_start])
    if tr is None:raise ValueError('enclosing original try required')
    to=lo+1+tr.end()-1;te=closing(source,to)
    if not(to<guard_start<guard_end<te):raise ValueError('retry guard must stay inside original try')
    if re.search(r'\b(?:for|foreach|while|switch)\s*\(',source[to+1:guard_start]):
        raise ValueError('intervening breakable owner unsupported')
    return {'while_open':lo,'while_close':le,'try_open':to,'try_close':te,'continue_owner':'original while(true)','EH_effect':'leave original try to existing while header'}

def transform_start_retry_positive(parent_method_source):
    pattern=re.compile(r'if\s*\(dialog.Show\(\) != 4\)\s*\{\s*return false;\s*\}\s*num\+\+;')
    matches=list(pattern.finditer(parent_method_source))
    if len(matches)!=1:raise ValueError('one existing Show guard and retry increment required')
    if len(re.findall(r'\bdialog.Show\(\)',parent_method_source))!=1:raise ValueError('Show must be evaluated once')
    m=matches[0]
    if 'while (true)'not in parent_method_source or 'catch'not in parent_method_source:
        raise ValueError('original loop and exception boundary required')
    prove_retry_loop_owner(parent_method_source,m.start(),m.end())
    # Explicit continue leaves the try to the existing while header. A plain
    # positive if/else is insufficient: it emits a different joining branch.
    replacement='if (dialog.Show() == 4) { num++; continue; } return false;'
    return parent_method_source[:m.start()]+replacement+parent_method_source[m.end():]
