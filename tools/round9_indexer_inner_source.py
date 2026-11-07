"""NOT_TESTED indexer/local assignment compositions; frozen D26 ONLY.
API calls are pure text, no I/O or C# tools. No current Main binding.
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



def transform_material_indexer_local_inner(method_source):
 s=method_source;no_capture(s)
 assert not re.search(r'\b(?:try|catch|finally|using|lock)\b',mask(s)),'local store observation on protected exits is unproved'
 assert re.search(r'\bDictionary\s*<\s*string\s*,\s*Material\s*>\s+value\s*;',mask(s))
 assert re.search(r'\bMaterial\s+value2\s*=\s*null\s*;',mask(s))
 pairs=[('value = (materlas_[key] = new Dictionary<string, Material>());','materlas_[key] = (value = new Dictionary<string, Material>());'),('value2 = (value[text] = new Material(shader, image));','value[text] = (value2 = new Material(shader, image));')]
 result=s
 for old,new in pairs:
  at=once(result,old);result=result[:at]+new+result[at+len(old):]
 return finish(s,result)
