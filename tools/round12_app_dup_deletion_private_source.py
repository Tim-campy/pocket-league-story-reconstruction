"""Finite local-initialization-chain templates; no I/O, no IL patching."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|=>|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];h=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(h)!=1:raise ValueError('unique template missing:'+pat)
 i=h[0];return t[i][1],t[i+len(p)-1][2]
def replace(s,pat,new):a,b=span(s,pat);return s[:a]+new+s[b:]
def block(s,start):
 t=tokens(s);a=next(i for i,x in enumerate(t) if x[1]>=start and x[0]=='{');depth=0
 for x in t[a:]:
  if x[0]=='{':depth+=1
  elif x[0]=='}':
   depth-=1
   if depth==0:return t[a][1],x[1],x[2]
 raise ValueError('unclosed block')
def direct_cases(s,selector,values):
 a,b=span(s,'switch ('+selector+')');op,cl,end=block(s,b);body=s[op+1:cl];t=tokens(body);labels=[];depth=0
 for i,x in enumerate(t):
  if x[0]=='{':depth+=1
  elif x[0]=='}':depth-=1
  elif x[0] in ('case','default') and depth==0:labels.append(i)
 if len(labels)!=len(values) or any([t[i+j][0] for j in range(3)]!=['case',str(n),':'] for n,i in zip(values,labels)):raise ValueError('exact cases required')
 parts=[]
 for n,i in enumerate(labels):
  part=body[t[i+2][2]:t[labels[n+1]][1] if n+1<len(labels) else len(body)];pt=tokens(part);bs=[j for j,x in enumerate(pt) if x[0]=='break']
  if len(bs)!=1:raise ValueError('ambiguous break ownership')
  j=bs[0]
  if [x[0] for x in pt[j:]] not in (['break',';'],['break',';','}']):raise ValueError('break not section terminal')
  parts.append(part[:pt[j][1]]+part[pt[j+1][2]:])
 new=' else '.join('if ('+selector+' == '+str(n)+') {'+p+'}' for n,p in zip(values,parts));return s[:a]+new+s[end:]
def transform_menu(s):
 if any(x[0] in ('goto','delegate','=>','yield','await') for x in tokens(s)):raise ValueError('unsupported control/capture')
 s=replace(s,'int num = 0; int num2 = 0;','int num3, num;\nnum3 = num = 0;\nint num7, num2;\nnum7 = num2 = 0;')
 s=replace(s,'int num3 = instance.GetSideSoftWidth() + 1;','num3 = instance.GetSideSoftWidth() + 1;')
 s=replace(s,'int num7 = instance.GetGameWidth() - 123 - instance.GetSideSoftWidth();','num7 = instance.GetGameWidth() - 123 - instance.GetSideSoftWidth();')
 span(s,'int matchInfoJoin = udat_.GetMatchInfoJoin();');span(s,'for (int i = 0; i < 3; i++)')
 return direct_cases(direct_cases(s,'matchInfoJoin',[0,1]),'i',[0,1,2])
def transform_tenant(s):
 if any(x[0] in ('goto','delegate','=>','yield','await') for x in tokens(s)):raise ValueError('unsupported control/capture')
 s=replace(s,'int num = 0; draw_cnt = 0;','int x, num;\nx = num = 0;\ndraw_cnt = 0;')
 return replace(s,'int x = camera_pos[0] + (num3 - 1) * 8;','x = camera_pos[0] + (num3 - 1) * 8;')
