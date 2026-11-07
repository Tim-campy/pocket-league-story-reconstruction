"""Pure source prototypes for three role-proven late-local scope mismatches.

No compiler/fixture/AST/model/verifier. No I/O at import/function-call time.
This never changes or normalizes an assembly, and supplies no oracle approval.
"""
from collections import Counter
import re

TOKEN=re.compile(r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|\x27(?:\\.|[^\x27\\])*\x27|[A-Za-z_][A-Za-z_0-9]*|[^\s]',re.M)
def tokens(s):return [(m.group(),m.start(),m.end())for m in TOKEN.finditer(s)if not m.group().startswith(('//','/*'))]
def words(s):return[t for t,_,_ in tokens(s)]
def mask(s):
 out=list(s)
 for m in TOKEN.finditer(s):
  if m.group().startswith(('//','/*','"','@"',"'")):
   for i in range(m.start(),m.end()):
    if out[i]not in '\r\n':out[i]=' '
 return ''.join(out)
def once(s,text):
 assert s.count(text)==1,('require unique anchor',text[:80],s.count(text))
 return s.index(text)
def apply(s,changes):
 for a,b,text in sorted(changes,reverse=True):s=s[:a]+text+s[b:]
 return s
def close(s,opening):
 assert s[opening]=='{';m=mask(s);depth=0
 for i in range(opening,len(s)):
  if m[i]=='{':depth+=1
  elif m[i]=='}':
   depth-=1
   if not depth:return i+1
 raise AssertionError('unclosed lexical block')
def no_cross_scope_constructs(s):
 assert not re.search(r'\b(?:goto|yield|await|delegate)\b|=>',mask(s)),'goto/capture/iterator/async unsupported'
def inventories(s):
 ts=words(s)
 strings=Counter(t for t in ts if t.startswith(('"','@"',"'")))
 calls=Counter(t for i,t in enumerate(ts[:-1])if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)and ts[i+1]=='('and t not in {'if','for','foreach','while','switch','catch','using','lock','typeof','sizeof','default','checked','unchecked'})
 return strings,calls
def finish(source,after,changes,component,checks):
 assert inventories(source)==inventories(after),'string/character literals or call names changed'
 assert close(after,after.index('{'))==len(after)
 return dict(after=after,changes=changes,component=component,checks=checks,ready=False,complete=None)

def calculate_balloon_tail_scope(method_source):
 s=method_source;no_cross_scope_constructs(s)
 loop=once(s,'foreach (Rectangle item2 in maskList_)');opening=s.index('{',loop);end=close(s,opening)
 hit=once(s,'if (KairoMath.CheckHitRect(');assert opening<hit<end
 hit_open=s.index('{',hit);hit_end=close(s,hit_open)
 true_body=s[hit_open:hit_end];wt=words(true_body)
 assert wt[-3:]==['continue',';','}']and wt.count('continue')==1
 assert not any(t in wt for t in ['for','foreach','while','do','switch']),'continue must belong to mask foreach'
 late=once(s,'bool flag6 = draw_tmp2.right < num18;')
 assert not s[hit_end:late].strip(),'tail must immediately follow the true-arm continue block'
 tail_start=s.rfind('\n',0,late)+1;tail_end=s.rfind('\n',0,end-1)+1
 assert opening<tail_start<tail_end<end
 tail=s[tail_start:tail_end];prefix=s[tail_start:late]
 assert not prefix.strip()
 names={'flag6','flag7','flag8','flag9','num27'}
 for name in names:
  assert name not in words(s[:tail_start]+s[tail_end:]),'tail local used outside new scope: '+name
 assert len(re.findall(r'\bbool\s+flag[6-9]\s*=',mask(tail)))==4
 assert len(re.findall(r'\bint\s+num27\s*=',mask(tail)))==1
 newline='\r\n'if'\r\n'in tail else'\n'
 replacement=prefix+'{'+newline+tail+prefix+'}'+newline
 after=s[:tail_start]+replacement+s[tail_end:]
 assert true_body in after and after.count(true_body)==s.count(true_body),'continue arm must remain exact'
 assert tail in replacement,'late statements must be copied byte-text exactly'
 return finish(s,after,[(tail_start,tail_end,replacement)],'balloon-post-continue-tail-child-block',dict(no_variable_rename=True,late_variables=sorted(names),tail_text_copied_exact=True,continue_arm_copied_exact=True,continue_owner='same mask foreach',late_names_not_used_outside_child=True,no_capture_goto_yield_await=True,new_control_statement_count=0,original_foreach_header_and_body_order_preserved=True))

def transform_balloon_tail_scope(method_source):
 return calculate_balloon_tail_scope(method_source)['after']

def calculate_seb_scan_loop_scope(method_source):
 s=method_source;no_cross_scope_constructs(s)
 pattern=re.compile(r'int (?P<index>num11|num12) = (?P<lower>num8|num9);(?P<gap1>\s*)int (?P<stride>num12|num13) = (?P=lower) \* 8;(?P<gap2>\s*)while \((?P=index) <= (?P<upper>num9|num10)\)')
 matches=list(pattern.finditer(s));assert len(matches)==1,'require paired initializers and scan while'
 m=matches[0];names=(m['index'],m['stride'],m['lower'],m['upper'])
 assert names in [('num11','num12','num8','num9'),('num12','num13','num9','num10')],'unknown scan-role tuple'
 opening=s.index('{',m.end());end=close(s,opening);body=s[opening:end]
 index,stride,lower,upper=names
 outside=s[:m.start()]+s[end:]
 assert index not in words(outside)and stride not in words(outside),'scan counters used outside loop scope'
 assert index+'++;'in body and stride+' += 8;'in body,'leave both original increments in body'
 assert not any(t in words(body)for t in ['for','foreach','while','do','switch']),'nested break/continue owners unsupported'
 continue_count=words(body).count('continue')
 header='for (int '+index+' = '+lower+', '+stride+' = '+lower+' * 8; '+index+' <= '+upper+';)'
 after=s[:m.start()]+header+s[m.end():]
 assert body in after and after.count(body)==s.count(body)
 return finish(s,after,[(m.start(),m.end(),header)],'seb-paired-counters-in-for-initializer-scope',dict(no_variable_rename=True,counters=[index,stride],initializer_order_unchanged=True,initializer_expressions_unchanged=True,body_copied_exact=True,increments_remain_in_body=True,for_increment_section_empty=True,existing_continue_count=continue_count,continue_destination='condition; body increments remain skipped exactly as with old while',break_destination='same scan-loop exit',counter_names_not_used_outside_new_scope=True,no_capture_goto_yield_await=True))

def transform_seb_scan_loop_scope(method_source):
 return calculate_seb_scan_loop_scope(method_source)['after']

