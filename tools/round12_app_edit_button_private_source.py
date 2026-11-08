"""Finite integer dispatch plus four parent-scope coordinate aliases. No I/O."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|=>|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
NAMES=('touchDx','touchDy','touchWidth','touchHeight')
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def hits(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];return [(t[i][1],t[i+len(p)-1][2]) for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
def span(s,pat):
 found=hits(s,pat)
 if len(found)!=1:raise ValueError('unique exact template required:'+pat)
 return found[0]
def match(t,i):
 n=0
 for j in range(i,len(t)):
  if t[j][0]=='{':n+=1
  elif t[j][0]=='}':
   n-=1
   if n==0:return j
 raise ValueError('unclosed block')
def direct_dispatch(s):
 a,b=span(s,'switch(type){');t=tokens(s);op=next(i for i,x in enumerate(t) if x[2]==b);cl=match(t,op);_,te=span(s,'if(isTouch){');top=next(i for i,x in enumerate(t) if x[2]==te);_,me=span(s,'if(isMarker){');mop=next(i for i,x in enumerate(t) if x[2]==me)
 if [x[0] for x in t[top+1:op-4]]!=['int','num','=','786432',';'] or cl+1!=mop-4:raise ValueError('dispatch must follow num initializer before marker')
 mcl=match(t,mop)
 if [x[0] for x in t[mcl+1:mcl+3]]!=['else','{'] or match(t,mcl+2)+1!=match(t,top):raise ValueError('marker must be terminal in touch block')
 labels=[];depth=0
 for i in range(op+1,cl):
  if t[i][0]=='{':depth+=1
  elif t[i][0]=='}':depth-=1
  elif t[i][0] in ('case','default') and not depth:labels.append(i)
 if len(labels)!=7:raise ValueError('exact seven cases required')
 parts=[]
 for n,i in enumerate(labels):
  end=labels[n+1] if n+1<len(labels) else cl
  if [x[0] for x in t[i:end]]!=['case',str(n),':','num','=',str(786432+65536*n),';','break',';']:raise ValueError('reviewed value assignment and owned break required')
  parts.append(s[t[i+2][2]:t[end-2][1]])
 chain=' else '.join('if (type == '+str(n)+') {'+p+'}' for n,p in enumerate(parts));return s[:a]+chain+s[t[cl][2]:]
def add_coordinate_aliases(s):
 if any(x[0] in NAMES for x in tokens(s)):raise ValueError('coordinate alias name collision')
 a,_=span(s,'if(isMarker){');prefix='int touchDx = dx;\nint touchDy = dy;\nint touchWidth = W;\nint touchHeight = H;\n';found=hits(s,'GameView.AddTouch(g,23,dx,dy,W,H,num|index,')
 if len(found)!=2 or any(x<a for x,_ in found):raise ValueError('two reviewed marker-arm coordinate calls required')
 for x,y in reversed(found):s=s[:x]+'GameView.AddTouch(g,23,touchDx,touchDy,touchWidth,touchHeight,num|index,'+s[y:]
 return s[:a]+prefix+s[a:]
def transform_edit_button(s):
 if any(x[0] in ('goto','delegate','=>','yield','await') for x in tokens(s)):raise ValueError('unsupported flow/capture')
 return add_coordinate_aliases(direct_dispatch(s))
