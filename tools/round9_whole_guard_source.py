"""NOT_TESTED pure reference/integer guard compositions; frozen D29 ONLY.
No I/O on import/API calls, no model/compiler/AST/verifier invocation.
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



def positive_tail(s,anchor,condition,required_tail):
 no_capture(s);a=once(s,anchor);opening=s.index('{',a);end=close(s,opening)
 assert words(s[opening:end])==['{','return',';','}'],'guard body must be only return'
 m=mask(s);depth=0
 for c in m[:a]:depth+=int(c=='{')-int(c=='}')
 assert depth==1,'must be direct method-body guard'
 method_end=close(s,s.index('{'));assert method_end==len(s)
 tail=s[end:method_end-1];expected=words(required_tail)
 assert words(tail)[:len(expected)]==expected,'unexpected tail entry'
 assert not re.search(r'\b(?:goto|yield|await|delegate)\b|=>',mask(tail))
 result=s[:a]+'if ('+condition+') {'+tail+indent(s,a)+'}\r\n'+s[method_end-1:]
 assert words(result).count('return')==words(s).count('return')-1
 return finish(s,result)

def transform_surface_debug_positive_tail(method_source):
 return positive_tail(method_source,'if (!Config.DEBUG || surfaceList_.Size() <= 0 || paintRects_.Count <= 0)','Config.DEBUG && surfaceList_.Size() > 0 && paintRects_.Count > 0','SurfaceBase surfaceBase = surfaceList_.FirstElement();')

def transform_save_unlock_positive_tail(method_source):
 return positive_tail(method_source,'if (!forced)','forced','for (int i = 0; i < saveReserve_.Length; i++)')

def transform_offscreen_two_guards(method_source):
 s=method_source;no_capture(s);anchor='if (image_ != null && !dispose_ && GetLayout != null)';a=once(s,anchor);opening=s.index('{',a);end=close(s,opening)
 method_open=s.index('{');method_end=close(s,method_open)
 assert not s[method_open+1:a].strip() and not s[end:method_end-1].strip(),'whole positive branch must be the sole method statement'
 expected=words('int width; int height; Image.Format format; Image.Filter filterMode; GetLayout(out width, out height, out format, out filterMode); Resize(width, height, format, filterMode);')
 assert words(s[opening+1:end-1])==expected,'only captured declaration/call payload permitted'
 p=indent(s,a);new='if (image_ == null || dispose_) { return; }\r\n'+p+'if (GetLayout == null) { return; }'+s[opening+1:end-1]
 result=s[:a]+new+s[end:]
 assert words(result).count('return')==words(s).count('return')+2
 return finish(s,result)
