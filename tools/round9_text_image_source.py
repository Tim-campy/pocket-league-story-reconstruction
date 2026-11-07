"""Pure source-only compositions for three bounded post-D26 residuals.

No I/O on import/API calls. No C# fixture/compiler, model, AST or verifier.
main() alone reads unchanged current/frozen targets and writes this WIP folder.
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

def transform_make_alignment_tail(method_source):
 s=method_source
 a=once(s,'if ((num2 == 32))');end=once(s,'if (height_ != -1f && num4 < 0f)');i=indent(s,a)
 old=s[a:end]
 no_capture(old)
 for expression in ['num4 = ((height_ != -1f) ? ((height_ - num4) / 2f) : ((-(num4)) / 2f));','num4 = ((height_ != -1f) ? (height_ - num4) : (0f - num4));','num4 = 0f;']:assert old.count(expression)==1
 assert old.count('?')==2 and words(old).count('num4')==7,'only exact final alignment expressions supported'
 assert words(old)==words('if ((num2 == 32)) { num4 = ((height_ != -1f) ? ((height_ - num4) / 2f) : ((-(num4)) / 2f)); } else if ((num2 == 64)) { num4 = ((height_ != -1f) ? (height_ - num4) : (0f - num4)); } else { num4 = 0f; }'),'exact complete alignment tail token template required'
 after='if (num2 == 32) { if (height_ == -1f) { num4 = -num4 / 2f; } else { num4 = (height_ - num4) / 2f; } } else if (num2 == 64) { if (height_ == -1f) { num4 = -num4; } else { num4 = height_ - num4; } } else { num4 = 0f; }\r\n'+i
 return finish(s,s[:a]+after+s[end:])

def transform_textblock_frame_arms(s):
 old='num3 = ((!(pictographParam_[1] == "∞")) ? int.Parse(pictographParam_[1]) : (-1));';a=once(s,old)
 new='if (pictographParam_[1] == "∞") { num3 = -1; } else { num3 = int.Parse(pictographParam_[1]); }'
 return finish(s,s[:a]+new+s[a+len(old):])

def transform_textblock_tail_scope(s):
 no_capture(s);_,try_end=outer_try(s)
 positive=once(s,'if (isPictograph_)');opening=s.index('{',positive);end=close(s,opening)
 true_body=s[opening:end];assert words(true_body)[-4:]==['return','result',';','}']
 late=once(s,'int num11 = len;');assert not s[end:late].strip()
 a=line_start(s,late);b=line_start(s,try_end-1);tail=s[a:b]
 assert 'num11'not in words(s[:a]+s[b:]),'length result crosses proposed child scope'
 assert words(tail)[-3:]==['return','num11',';']
 p=indent(s,late);replacement=p+'{\r\n'+tail+p+'}\r\n'
 return finish(s,s[:a]+replacement+s[b:])

def transform_textblock_draw_composition(method_source):
 return transform_textblock_tail_scope(transform_textblock_frame_arms(method_source))

def optimize_bounds(s):
 a=once(s,'if (image.optimizePass_ != 0)');opening=s.index('{',a);return opening,close(s,opening)

def transform_drawimage_float_casts(s):
 for name,target in [('width','num17'),('height','num18')]:
  assert re.search(r'\bfloat\s+'+name+r'\b',mask(s)),'Single parameter/local type proof needed'
  divisor='swidth'if name=='width'else'sheight';old='float '+target+' = '+name+' / '+divisor+';';a=once(s,old)
  new='float '+target+' = (float)'+name+' / '+divisor+';';s=s[:a]+new+s[a+len(old):]
 return s

def transform_drawimage_optimize_arms(s):
 old='array = ((image.optimizeAccess_ == null) ? image.GetOptimize(image.optimizeAccessL_[num28]) : image.GetOptimize(image.optimizeAccess_[num28]));';a=once(s,old)
 new='if (image.optimizeAccess_ == null) { array = image.GetOptimize(image.optimizeAccessL_[num28]); } else { array = image.GetOptimize(image.optimizeAccess_[num28]); }'
 return finish(s,s[:a]+new+s[a+len(old):])

def transform_drawimage_positive_lookup(s):
 _,end=optimize_bounds(s);tail_end=line_start(s,end-1)
 a=once(s,'if (num25 == -1)');opening=s.index('{',a);guard_end=close(s,opening)
 assert words(s[opening:guard_end])==['{','return',';','}']
 tail=s[guard_end:tail_end];assert tail.strip().startswith('image.GetOptimizeSeb(num25);')and'return'not in words(tail)
 p=indent(s,a);replacement='if (num25 != -1) {'+tail+p+'}\r\n'
 return finish(s,s[:a]+replacement+s[tail_end:])

def transform_drawimage_lookup_scope(s):
 _,outer_end=optimize_bounds(s)
 branch=once(s,'if (image.optimizeSeb_ == null)');opening=s.index('{',branch);branch_end=close(s,opening)
 assert words(s[opening:branch_end])[-3:]==['return',';','}'],'first path must terminate before late lookup'
 late=once(s,'int num25 = -1;');assert not s[branch_end:late].strip()
 assert s.count('int num26 = image.optimizeSeb_.Length / 4;')==1
 a=line_start(s,late);b=line_start(s,outer_end-1);tail=s[a:b]
 no_capture(tail)
 assert not re.search(r'(?m)^\s*[A-Za-z_]\w*\s*:',mask(tail)),'labels could permit cross-scope entry'
 assert all(n not in words(s[:a]+s[b:])for n in ['num25','num26']),'lookup state used outside new shared scope'
 p=indent(s,late);replacement=p+'{\r\n'+tail+p+'}\r\n'
 return finish(s,s[:a]+replacement+s[b:])

def transform_drawimage_composition(method_source):
 s=transform_drawimage_float_casts(method_source)
 s=transform_drawimage_optimize_arms(s)
 s=transform_drawimage_positive_lookup(s)
 s=transform_drawimage_lookup_scope(s)
 return finish(method_source,s)

