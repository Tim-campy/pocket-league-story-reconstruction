"""Pure composition of four C18 residual reasons, never source/model approval.

Import/function calls do no I/O. main() alone binds frozen/current target text
and writes this WIP directory; no compiler, fixture, model or verifier.
"""
from collections import Counter
import re

TOKEN=re.compile(r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|\x27(?:\\.|[^\x27\\])*\x27|[A-Za-z_][A-Za-z_0-9]*|[^\s]',re.M)
def tokens(s):return [(m.group(),m.start(),m.end())for m in TOKEN.finditer(s)if not m.group().startswith(('//','/*'))]
def mask(s):
 result=list(s)
 for m in TOKEN.finditer(s):
  if m.group().startswith(('//','/*','"','@"',"'")):
   for i in range(m.start(),m.end()):
    if result[i]not in '\r\n':result[i]=' '
 return ''.join(result)
def once(s,text):
 assert s.count(text)==1,('require unique anchor',text[:100],s.count(text))
 return s.index(text)
def close(s,opening):
 assert s[opening]=='{';masked=mask(s);depth=0
 for i in range(opening,len(s)):
  if masked[i]=='{':depth+=1
  elif masked[i]=='}':
   depth-=1
   if not depth:return i+1
 raise AssertionError('unclosed lexical block')
def literal_inventory(s):return Counter(t for t,_,_ in tokens(s)if t.startswith(('"','@"',"'")))
def call_inventory(s):
 ts=[t for t,_,_ in tokens(s)]
 return Counter(t for i,t in enumerate(ts[:-1])if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)and ts[i+1]=='('and t not in {'if','for','foreach','while','switch','catch','using','lock','typeof','sizeof','default','checked','unchecked'})
def apply(s,changes):
 for a,b,text in sorted(changes,reverse=True):s=s[:a]+text+s[b:]
 return s

def calculate_execute_residual(method_source):
 source=method_source;masked=mask(source);changes=[];reasons=[]
 assert len(re.findall(r'\bbool\s+flag\s*=',masked))==1,'flag must be an explicit bool local, not property/unknown type'
 assert len(re.findall(r'\bSaveRecorder\s+instance4\s*=',masked))==1,'instance4 must be a stable declared SaveRecorder local'
 marker='if (instance4 != null && flag != lastKeyguard_) { if (flag) '
 a=once(source,marker);outer=source.index('{',a);inner=a+len(marker)
 assert source[inner]=='{'
 end_inner=close(source,inner);end_outer=close(source,outer)
 assert not source[end_inner:end_outer-1].strip(),'no other outer-if effects allowed'
 assert [t for t,_,_ in tokens(source[inner:end_inner])]==['{','ProcessEvent','(','AppEventType','.','Sleep',')',';','}'],'exact ProcessEvent leaf only'
 body=source[inner:end_inner]
 changes.append((a,end_outer,'if (instance4 != null && ((flag != lastKeyguard_) & flag)) '+body))
 reasons.append(dict(edit='outer-reference-check-short-circuit-with-eager-Boolean-tail',original_offsets=[101,103,105,108,113,115,116,118,120,121],source='replace actual adopted nested-if shape, not obsolete flat pre-D18 source',evaluation='instance4 null first; lastKeyguard_ loaded once; flag is builtin local; no floating comparison changed',evidence='original ceq;ldc0;ceq;ldloc5;and vs current beq then brfalse; proposed emission size effect UNKNOWN (+4 expected)'))
 for old,call,original_offset in [('instance4?.Lock();','instance4.Lock();',295),('instance4?.Unlock(true);','instance4.Unlock(true);',855)]:
  a=once(source,old)
  assert masked[a:a+len(old)]==old
  changes.append((a,a+len(old),'if ((object)instance4 != null) { '+call+' }'))
  reasons.append(dict(edit='explicit-reference-null-guard-'+('Lock'if original_offset==295 else'Unlock'),original_offset=original_offset,receiver='same stable local instance4, no reevaluation of GetInstance/property',argument_text_preserved=True,evidence='original callvirt and rebuilt call have same SaveRecorder member signature; explicit guard is hypothesis until emission fixture'))
 marker='switch (reserveSave)';a=once(source,marker);opening=source.index('{',a);end=close(source,opening)
 expected=['{','case','1',':','instance','.','Save','(',')',';','break',';','case','2',':','instance','.','SaveAll','(',')',';','break',';','}']
 assert [t for t,_,_ in tokens(source[opening:end])]==expected,'only two simple switch-owned breaks/call leaves supported'
 assert len(re.findall(r'\bint\s+reserveSave\s*=',masked))==1,'selector must be explicit stable Int32 local; enum/property selector unsupported'
 loop=once(source,'for (int j = 0; j < 3; j++)');loop_open=source.index('{',loop);loop_end=close(source,loop_open)
 assert loop_open<a<end<loop_end,'switch must be inside existing retry for'
 tail=source[end:loop_end]
 assert re.search(r'\bbreak\s*;\s*}\s*$',mask(tail)),'existing outer-loop break must remain after switch/catch'
 after='if (reserveSave == 1) { instance.Save(); } else if (reserveSave == 2) { instance.SaveAll(); }'
 changes.append((a,end,after))
 reasons.append(dict(edit='reserveSave-if-elseif-with-owned-switch-breaks-only',original_offsets=[1028,1031,1033,1034,1039,1041,1044,1046,1047,1052],selector_type='Int32 from explicit int reserveSave; no selector call/property duplicated',removed_break_owner='two breaks in this exact two-section switch',retained_break_owner='existing retry-for break outside switch/catch; suffix copied exactly',evidence='original tests1 thenSave or tests2 thenSaveAll; current switch dispatches both tests first; proposed size effect UNKNOWN (-2 expected)'))
 ordered=sorted(changes);assert all(a[1]<=b[0]for a,b in zip(ordered,ordered[1:]))
 result=apply(source,changes)
 assert literal_inventory(source)==literal_inventory(result),'string/character literals changed'
 assert call_inventory(source)==call_inventory(result),'call-name inventory changed'
 assert source[end:]==result[result.index(after)+len(after):],'outer retry-loop/catch suffix must be exact'
 notify='NotifyManager recoveredNotifyManager = NotifyManager.GetInstance();'
 # Optional in small fixtures; mandatory for the actual bound target in main.
 if notify in source:assert notify in result and result.count(notify)==source.count(notify)
 assert close(result,result.index('{'))==len(result)
 return dict(after=result,changes=changes,reasons=reasons,checks=dict(no_new_named_locals=True,flag_declared_Boolean=True,instance4_stable_declared_local=True,reserveSave_declared_Int32=True,ProcessEvent_leaf_copied_exact=True,call_name_multiset_equal=True,string_character_literal_multiset_equal=True,switch_breaks_removed=2,outer_retry_loop_break_and_suffix_unchanged=True,Notify_declaration_untouched=True,no_float_relation_rewrite=True,lexical_braces_balanced=True),ready=False,complete=None)

def transform_execute_residual(method_source):
 """Pure method-text API; no declaring class/header identity dependency."""
 return calculate_execute_residual(method_source)['after']

