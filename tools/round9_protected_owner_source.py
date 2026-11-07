"""NOT_TESTED pure protected-exit compositions, frozen D33 only.
No import/API I/O, compiler/model/AST/verifier or Main source binding.
"""
from collections import Counter
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|\x27(?:\\.|[^\x27\\])*\x27|[A-Za-z_][A-Za-z_0-9]*|[^\s]',re.M)
def tokens(s):return[(m.group(),m.start(),m.end())for m in TOKEN.finditer(s)if not m.group().startswith(('//','/*'))]
def words(s):return[t for t,_,_ in tokens(s)]
def mask(s):
 out=list(s)
 for m in TOKEN.finditer(s):
  if m.group().startswith(('//','/*','"','@"',"'")):
   for i in range(m.start(),m.end()):
    if out[i]not in '\r\n':out[i]=' '
 return ''.join(out)
def once(s,text):
 assert s.count(text)==1,('unique anchor required',text[:100],s.count(text))
 return s.index(text)
def close(s,opening):
 assert s[opening]=='{';m=mask(s);depth=0
 for i in range(opening,len(s)):
  if m[i]=='{':depth+=1
  elif m[i]=='}':
   depth-=1
   if not depth:return i+1
 raise AssertionError('unclosed lexical block')
def line_start(s,i):return s.rfind('\n',0,i)+1
def indent(s,i):
 p=s[line_start(s,i):i];assert not p.strip();return p
def no_capture(s):assert not re.search(r'\b(?:goto|yield|await|delegate)\b|=>',mask(s)),'unproven transfer/capture unsupported'
def inventories(s):
 ts=words(s)
 return Counter(t for t in ts if t.startswith(('"','@"',"'"))),Counter(t for i,t in enumerate(ts[:-1])if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)and ts[i+1]=='('and t not in {'if','for','foreach','while','switch','catch','using','lock','typeof','sizeof','default','checked','unchecked'})
def outer_try(s):
 m=re.search(r'\btry\s*{',mask(s));assert m;opening=s.index('{',m.start());return opening,close(s,opening)
def finish(before,after):
 assert inventories(before)==inventories(after),'string/character literals or named calls changed'
 assert close(after,after.index('{'))==len(after)
 return after



def no_closure(s):assert not re.search(r'\b(?:yield|await|delegate)\b|=>',mask(s))
def parent_block(s,at):
 m=mask(s);stack=[]
 for i,c in enumerate(m[:at]):
  if c=='{':stack.append(i)
  elif c=='}':assert stack;stack.pop()
 assert stack
 return stack[-1],close(s,stack[-1])

def transform_payment_retry_else(method_source):
 s=method_source;no_closure(s)
 assert re.findall(r'\bgoto\s+(\w+)\s*;',mask(s))==['IL_0304','IL_0304']
 a=once(s,'if (array == null || array.Length == 0)');go=s.index('{',a);ge=close(s,go)
 assert words(s[go:ge])==words('{ result = -1; goto IL_0304; }')
 tp,te=parent_block(s,a);assert re.search(r'\btry\s*$',mask(s[:tp]))
 fs=once(s,'for (int j = 0; j < array.Length; j++)');fo=s.index('{',fs);fe=close(s,fo)
 assert parent_block(s,fs)[0]==tp and not s[ge:fs].strip()and not s[fe:te-1].strip()
 assert 'break'not in words(s[fs:fe]),'array scan must not supply another break owner'
 assert words(s).count('do')==1 and words(s).count('while')==1 and words(s).count('break')==2
 # guard/array loop are inside try inside the existing retry do; adding else
 # creates no new loop owner. The moved terminal break still exits that do.
 guard=s[a:ge].replace('goto IL_0304;','');p=indent(s,a)
 joined=guard+'\r\n'+p+'else {\r\n'+s[fs:fe]+'\r\n'+p+'break;\r\n'+p+'}'
 result=s[:a]+joined+s[fe:]
 catch_goto=once(result,'goto IL_0304;');catch_parent,catch_end=parent_block(result,catch_goto)
 assert re.search(r'\bcatch\s*\(Exception t\)\s*$',mask(result[:catch_parent]))
 tail_label=once(result,'IL_0304:');between=result[catch_end:tail_label]
 assert words(between)==['break',';'],'only the old outer-do break may lie before retry label'
 result=result[:catch_end]+result[tail_label+len('IL_0304:'):]
 catch_goto=once(result,'goto IL_0304;');result=result[:catch_goto]+result[catch_goto+len('goto IL_0304;'):]
 assert words(result).count('break')==words(s).count('break')and'goto'not in words(result)
 return finish(s,result)

def transform_http_retry_continue(method_source):
 s=method_source;no_closure(s)
 assert words(s).count('while')==1 and re.search(r'\bwhile\s*\(true\)',mask(s))
 assert not any(x in words(s)for x in ['for','foreach','do','switch'])
 old='goto end_IL_0014;';a=once(s,old);label='end_IL_0014:;';b=once(s,label)
 assert b>a and re.search(r'num\+\+;\s*$',s[:a])
 cp,ce=parent_block(s,a);assert re.search(r'\bcatch\s*\(Exception ex\)\s*$',mask(s[:cp]))
 assert b<ce and not s[b+len(label):ce-1].strip()
 assert words(s[a+len(old):b])==['IL_00ab',':','throw','ex',';']
 result=s[:a]+'continue;'+s[a+len(old):b]+s[b+len(label):]
 assert words(result).count('continue')==words(s).count('continue')+1
 assert result.count('goto IL_00ab;')==s.count('goto IL_00ab;')==2
 return finish(s,result)

def transform_storage_heap_common_return(method_source):
 s=method_source;no_capture(s);a=once(s,'if (num != -1)');opening=s.index('{',a);end=close(s,opening)
 assert words(s[opening:end])==words('{heapData_.SetElementAt(data,num);return;}')
 pp,pe=parent_block(s,a);assert 'Config.USE_HEAP_STORAGE' in s[max(0,pp-100):pp]
 tail=s[end:pe-1];assert words(tail)==words('heapName_.AddElement(filename); heapData_.AddElement(data);')
 p=indent(s,a);new='if (num != -1) { heapData_.SetElementAt(data, num); } else {'+tail+p+'}\r\n'+p+'return;\r\n'
 return finish(s,s[:a]+new+s[pe-1:])

def transform_storage_media_explicit_return(method_source):
 s=method_source;anchor='else if (media_ != 3 && (media_ == 1 || media_ == 4))';a=once(s,anchor);opening=s.index('{',a);end=close(s,opening)
 body=s[opening:end];p=indent(s,a)
 new='else { if (media_ == 3) { return; } if (media_ == 1 || media_ == 4) '+body+'\r\n'+p+'}'
 return finish(s,s[:a]+new+s[end:])

def transform_storage_write_protected_exits(method_source):
 return transform_storage_media_explicit_return(transform_storage_heap_common_return(method_source))
