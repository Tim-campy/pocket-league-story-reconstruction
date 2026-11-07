"""NOT_TESTED pure field-outer indexer-chain composition; frozen D32 only.
API has no I/O; no C# tool/model/AST/verifier invocation, no Main binding.
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



def transform_renderpass_resultcolors_chain(method_source):
 s=method_source;no_capture(s)
 assert not re.search(r'\b(?:try|catch|finally|using|lock)\b',mask(s)),'protected observation unproved'
 old='Color[] array = (ResultColors[Colors.Length] = new Color[Colors.Length]);';a=once(s,old)
 following='resultColors_ = array;';b=once(s,following)
 assert b>a and not s[a+len(old):b].strip(),'must be two adjacent statements'
 assert words(s).count('array')==2,'intermediate named array is observed elsewhere'
 new='resultColors_ = (ResultColors[Colors.Length] = new Color[Colors.Length]);'
 result=s[:a]+new+s[b+len(following):]
 assert 'array'not in words(result)
 return finish(s,result)
