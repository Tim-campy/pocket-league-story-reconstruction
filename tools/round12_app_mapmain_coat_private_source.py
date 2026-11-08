"""Finite MapMain dispatch/continue composition; no I/O or IL manipulation."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|=>|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('unique reviewed template required:'+pat)
 i=hits[0];return t[i][1],t[i+len(p)-1][2]
def match(t,i,left='{',right='}'):
 n=0
 for j in range(i,len(t)):
  if t[j][0]==left:n+=1
  elif t[j][0]==right:
   n-=1
   if n==0:return j
 raise ValueError('unclosed delimiter')
def direct_dispatch(s,guarded=True):
 a,b=span(s,'switch(num2){');t=tokens(s);op=next(i for i,x in enumerate(t) if x[2]==b);cl=match(t,op);depth=0;labels=[]
 _,ae=span(s,'if(udat_.tAreaLv_<2){');areaop=next(i for i,x in enumerate(t) if x[2]==ae)
 if areaop+1!=op-4 or match(t,areaop)!=cl+1:raise ValueError('switch-only area guard owner required')
 for i in range(op+1,cl):
  if t[i][0]=='{':depth+=1
  elif t[i][0]=='}':depth-=1
  elif t[i][0] in ('case','default') and not depth:labels.append(i)
 if len(labels)!=3 or any([x[0] for x in t[i:i+3]]!=['case',str(value),':'] for i,value in zip(labels,(0,3,6))):raise ValueError('exact0/3/6 sections required')
 parts=[]
 for n,i in enumerate(labels):
  end=labels[n+1] if n+1<len(labels) else cl;part=t[i+3:end]
  if [x[0] for x in part[-2:]]!=['break',';'] or sum(x[0]=='break' for x in part)!=1:raise ValueError('terminal case break required')
  words=[x[0] for x in part[:-2]]
  if n<2 and words[:2]!=['mapchipMain2','=']:raise ValueError('reviewed chip assignment required')
  if n==2 and words!=['num2','=','5',';']:raise ValueError('reviewed fence assignment required')
  parts.append(s[t[i+2][2]:part[-2][1]])
 chain=' else '.join('if (num2 == '+str(value)+') {'+part+'}' for value,part in zip((0,3,6),parts))
 if guarded:chain='if (num2 >= 0) {'+chain+'}'
 return s[:a]+chain+s[t[cl][2]:]
def fence_continue(s):
 a,b=span(s,'if(num2>=5){');t=tokens(s);op=next(i for i,x in enumerate(t) if x[2]==b);cl=match(t,op);payload=s[b:t[cl][1]];pt=tokens(payload)
 ha,_=span(s,'if(GameUtil.ChRectHitRect(camera_pos[0]');hi=next(i for i,x in enumerate(t) if x[1]==ha);hitop=match(t,hi+1,'(',')')+1
 if cl+1!=match(t,hitop):raise ValueError('fence must be terminal in reviewed hit body')
 if not pt or [x[0] for x in pt[:5]]!=['apdat_','.','InsertGameObject','(','6'] or sum(x[0]==';' for x in pt)!=1:raise ValueError('reviewed single fence insert required')
 return s[:a]+'if (num2 < 5) {continue;}\n'+payload+s[t[cl][2]:]
def hit_continue(s):
 a,_=span(s,'if(GameUtil.ChRectHitRect(camera_pos[0]');t=tokens(s);i=next(i for i,x in enumerate(t) if x[1]==a);endcond=match(t,i+1,'(',')')
 if t[endcond+1][0]!='{':raise ValueError('hit body required')
 op=endcond+1;cl=match(t,op);condition=s[t[i+1][2]:t[endcond][1]];payload=s[t[op][2]:t[cl][1]]
 stack=[]
 for j in range(i):
  if t[j][0]=='{':stack.append(j)
  elif t[j][0]=='}':stack.pop()
 owner=stack[-1];ownerend=match(t,owner);_,e=span(s,'for(int i=0;i<MAPCHIP_FIELD_WNUM;i++)');headend=next(j for j,x in enumerate(t) if x[2]==e)
 if owner!=headend+1 or t[owner][0]!='{' or cl+1!=ownerend:raise ValueError('hit must be terminal in reviewed inner i loop')
 return s[:a]+'if (!('+condition+')) {continue;}\n'+payload+s[t[cl][2]:]
def transform_coat(s):
 if any(x[0] in ('goto','delegate','=>','yield','await') for x in tokens(s)):raise ValueError('unsupported capture/flow')
 span(s,'int num2;');span(s,'num2=-1;');span(s,'if(udat_.tAreaLv_<2){')
 return hit_continue(fence_continue(direct_dispatch(s)))
