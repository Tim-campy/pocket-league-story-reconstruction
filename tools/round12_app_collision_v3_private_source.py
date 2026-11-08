"""Finite B06-parent control composition; ordinary C# only, no I/O."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+(?:\.\d+)?f?|=>|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('unique exact template missing:'+pat)
 i=hits[0];return t[i][1],t[i+len(p)-1][2]
def matching(ts,i,left,right):
 n=0
 for j in range(i,len(ts)):
  if ts[j][0]==left:n+=1
  elif ts[j][0]==right:
   n-=1
   if not n:return j
 raise ValueError('unclosed delimiter')
def positive_height(s):
 a,b=span(s,'if (!(udat_.ball_.pos_.y_ < 20f)) {return;}');ts=tokens(s);i=next(i for i,x in enumerate(ts) if x[1]==a);stack=[]
 for j in range(i):
  if ts[j][0]=='{':stack.append(j)
  elif ts[j][0]=='}':stack.pop()
 op=stack[-1];cl=matching(ts,op,'{','}')
 # Prove the tail owner is the role0 branch, not an arbitrary ancestor.
 if op<6 or [x[0] for x in ts[op-6:op]]!=['if','(','role_','==','0',')']:raise ValueError('role0 block owner required')
 tail=s[b:ts[cl][1]]
 if not tail.strip():raise ValueError('empty positive tail')
 return s[:a]+'if (udat_.ball_.pos_.y_ < 20f) {'+tail+'}\n'+s[ts[cl][1]:]
OLD='else if ((!BitUtil.Check(udat_.ball_.flag_, 16) || !BitUtil.Check(udat_.ball_.flag_, 32) || udat_.ball_.boundNum_ != 0) && ChTrapOrHeading()) {ChRepetition();}'
NEW='else {\nif (BitUtil.Check(udat_.ball_.flag_, 16) && BitUtil.Check(udat_.ball_.flag_, 32) && udat_.ball_.boundNum_ == 0) {return;}\nif (ChTrapOrHeading()) {ChRepetition();}\n}'
def restore_tail_return(s):
 a,b=span(s,OLD);return s[:a]+NEW+s[b:]
def transform_collision_v3(s):
 if any(x[0] in ('goto','delegate','=>','yield','await') for x in tokens(s)):raise ValueError('unsupported callable flow')
 span(s,'if (SoccerForm.DEBUG_GK_MODE == 1 && team_id_ == 0) {return;}')
 out=restore_tail_return(positive_height(s));span(out,'if (SoccerForm.DEBUG_GK_MODE == 1 && team_id_ == 0) {return;}');return out
