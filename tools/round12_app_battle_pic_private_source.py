"""Pure declaration/zero-store and stable Int32 dispatch templates, finite B4 scope."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pattern):
 t=tokens(s);p=[x[0] for x in tokens(pattern)];h=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(h)!=1:raise ValueError('unique template missing: '+pattern)
 i=h[0];return t[i][1],t[i+len(p)-1][2]
def replace(s,pattern,new):a,b=span(s,pattern);return s[:a]+new+s[b:]
def match_block(s,start):
 ts=tokens(s);a=next(i for i,t in enumerate(ts) if t[1]>=start and t[0]=='{');depth=0
 for t in ts[a:]:
  if t[0]=='{':depth+=1
  elif t[0]=='}':
   depth-=1
   if not depth:return ts[a][1],t[1],t[2]
 raise ValueError('unclosed block')
def zero_layout(s):
 s=replace(s,'int num3; int num2; int num4; int num = (num2 = (num3 = (num4 = 0)));','int num4, num5, num, num2, num3;\nnum5 = num = num2 = num3 = num4 = 0;')
 return replace(s,'int num5 = 4;','num5 = 4;')
def two_arm_if(s,selector):
 a,b=span(s,'switch ('+selector+')');op,cl,end=match_block(s,b);body=s[op+1:cl];ts=tokens(body);labels=[];depth=0
 for i,t in enumerate(ts):
  if t[0]=='{':depth+=1
  elif t[0]=='}':depth-=1
  elif t[0] in ('case','default') and depth==0:labels.append(i)
 if len(labels)!=2 or any([ts[i+j][0] for j in range(3)]!=['case',str(n),':'] for n,i in enumerate(labels)):raise ValueError('only cases0/1, no default')
 parts=[]
 for n,i in enumerate(labels):
  stop=ts[labels[n+1]][1] if n+1<len(labels) else len(body);part=body[ts[i+2][2]:stop];pt=tokens(part);breaks=[x for x in pt if x[0]=='break']
  if len(breaks)!=1:raise ValueError('nonterminal or nested break owner')
  bi=pt.index(breaks[0]);suffix=[x[0] for x in pt[bi:]]
  if suffix not in (['break',';'],['break',';','}']):raise ValueError('break is not section-terminal')
  part=part[:breaks[0][1]]+part[pt[bi+1][2]:];parts.append(part)
 return s[:a]+'if ('+selector+' == 0) {'+parts[0]+'} else if ('+selector+' == 1) {'+parts[1]+'}'+s[end:]
def transform_battle_pic(s):
 if any(t[0] in ('delegate','=>','yield','await','goto') for t in tokens(s)):raise ValueError('unsupported flow/capture')
 span(s,'int matchInfoJoin = instance3.GetMatchInfoJoin();');span(s,'for (int l = 0; l < 2; l++)')
 return two_arm_if(two_arm_if(zero_layout(s),'matchInfoJoin'),'l')
