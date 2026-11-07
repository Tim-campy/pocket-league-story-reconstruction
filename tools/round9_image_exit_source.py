"""NOT_TESTED pure residual composition, frozen after-D29 ONLY.
No compiler/model/AST/verifier or current Main binding. API calls have no I/O.
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



def transform_drawimage_access_order(method_source):
 s=method_source
 old='if (image.optimizeAccess_ == null) { array = image.GetOptimize(image.optimizeAccessL_[num28]); } else { array = image.GetOptimize(image.optimizeAccess_[num28]); }'
 new='if (image.optimizeAccess_ != null) { array = image.GetOptimize(image.optimizeAccess_[num28]); } else { array = image.GetOptimize(image.optimizeAccessL_[num28]); }'
 a=once(s,old)
 return finish(s,s[:a]+new+s[a+len(old):])

def transform_drawimage_terminal_return(method_source):
 s=method_source;no_capture(s);m=mask(s);a=once(s,'if (num25 != -1)');opening=s.index('{',a);end=close(s,opening)
 parents=[]
 for i,c in enumerate(m[:a]):
  if c=='{':parents.append(i)
  elif c=='}':assert parents;parents.pop()
 assert parents
 parent=parents[-1];parent_end=close(s,parent)
 assert not s[end:parent_end-1].strip(),'positive guard must be final statement of lookup child block'
 prefix=s[parent:a]
 expected_prefix=words('{ int num25 = -1;');assert words(prefix)[:len(expected_prefix)]==expected_prefix
 assert prefix.count('int num26 = image.optimizeSeb_.Length / 4;')==1
 expected_body=words('image.GetOptimizeSeb(num25); for (');assert words(s[opening:end])[1:1+len(expected_body)]==expected_body,'unexpected optimized body entry'
 assert '\n' in s[end:parent_end-1]
 # Add a return at the existing lookup block's tail. This is deliberately
 # AFTER the positive guard, so its false edge and normal loop exit meet the
 # same explicit protected return, before the non-optimized else fallback.
 p=indent(s,a);result=s[:end]+'\r\n'+p+'return;'+s[end:]
 assert words(result).count('return')==words(s).count('return')+1
 return finish(s,result)

def transform_drawimage_d29_residual(method_source):
 return transform_drawimage_terminal_return(transform_drawimage_access_order(method_source))
