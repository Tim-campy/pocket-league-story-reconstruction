"""Control-only V2 over immutable V1, ordinary C#; no I/O at calls."""
import re
from v1_frozen import transform as v1_transform
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+(?:\.\d+)?f?|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('unique exact template missing:'+pat)
 i=hits[0];return t[i][1],t[i+len(p)-1][2]
def replace(s,pat,new):a,b=span(s,pat);return s[:a]+new+s[b:]
def matching(ts,i,left,right):
 depth=0
 for j in range(i,len(ts)):
  if ts[j][0]==left:depth+=1
  elif ts[j][0]==right:
   depth-=1
   if not depth:return j
 raise ValueError('unclosed delimiter')
def split_loop_guards(s):
 s=replace(s,'if (j == 0 || (i == team_id_ && j == p_id_)) {continue;}','if (j == 0) {continue;}\nif (i == team_id_ && j == p_id_) {continue;}')
 return replace(s,'if (JMath.Abs(address_[0] - player.address_[0]) >= 2 || JMath.Abs(address_[1] - player.address_[1]) >= 2 || BitUtil.Check(player.flag_, 16408)) {continue;}','if (JMath.Abs(address_[0] - player.address_[0]) >= 2) {continue;}\nif (JMath.Abs(address_[1] - player.address_[1]) >= 2) {continue;}\nif (BitUtil.Check(player.flag_, 16408)) {continue;}')
def split_float_guards_and_stores(s):
 s=replace(s,'if (num3 < 0.001f || num4 < 0.001f) {continue;}','if (num3 < 0.001f) {continue;}\nif (num4 < 0.001f) {continue;}')
 return replace(s,'num5 = ((!(num4 > num3)) ? 1 : 0);','if (num4 > num3) {num5 = 0;} else {num5 = 1;}')
def wrap_guard_tail(s,prefix,transfer,positive=None):
 ts=tokens(s);a,_=span(s,prefix);idx=next(i for i,t in enumerate(ts) if t[1]==a);assert ts[idx][0]=='if' and ts[idx+1][0]=='('
 endcond=matching(ts,idx+1,'(',')');assert ts[endcond+1][0]=='{';endguard=matching(ts,endcond+1,'{','}')
 if [x[0] for x in ts[endcond+2:endguard]]!=[transfer,';']:raise ValueError('guard transfer changed')
 stack=[]
 for i in range(idx):
  if ts[i][0]=='{':stack.append(i)
  elif ts[i][0]=='}':stack.pop()
 owner=stack[-1];ownerend=matching(ts,owner,'{','}')
 condition=s[ts[idx+1][2]:ts[endcond][1]]
 if positive is None:
  if not condition.lstrip().startswith('!GameUtil.ChRectHitRect('):raise ValueError('only direct Boolean call guard')
  condition=condition.lstrip()[1:]
 else:condition=positive
 tail=s[ts[endguard][2]:ts[ownerend][1]]
 if not tail.strip():raise ValueError('empty tail')
 return s[:a]+'if ('+condition+') {'+tail+'}\n'+s[ts[ownerend][1]:]
def positive_collision_regions(s):
 # The first negative rectangle guard is loop-owned; the second is method-return-owned.
 s=wrap_guard_tail(s,'if (!GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0]','continue')
 s=wrap_guard_tail(s,'if (udat_.gm_.state_ != 0 || udat_.gm_.gameState_ != 2 || !BitUtil.Check(udat_.ball_.flag_, 8))','return','udat_.gm_.state_ == 0 && udat_.gm_.gameState_ == 2 && BitUtil.Check(udat_.ball_.flag_, 8)')
 s=wrap_guard_tail(s,'if (!(udat_.ball_.pos_.y_ < 20f))','return','udat_.ball_.pos_.y_ < 20f')
 return wrap_guard_tail(s,'if (!GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData3[0]','return')
def transform_controls(s):
 debug='if (SoccerForm.DEBUG_GK_MODE == 1 && team_id_ == 0) {return;}';span(s,debug)
 if any(t[0] in ('goto','delegate','=>','yield','await') for t in tokens(s)):raise ValueError('unsupported callable flow')
 out=positive_collision_regions(split_float_guards_and_stores(split_loop_guards(s)));span(out,debug);return out
def transform_collision_v2(s):return transform_controls(v1_transform(s))
