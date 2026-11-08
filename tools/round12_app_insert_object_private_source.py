"""One explicit retry/continue and insertion-tail ownership restoration."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|=>|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('unique reviewed structure required:'+pat)
 i=hits[0];return t[i][1],t[i+len(p)-1][2]
def match(t,i):
 depth=0
 for j in range(i,len(t)):
  if t[j][0]=='{':depth+=1
  elif t[j][0]=='}':
   depth-=1
   if depth==0:return j
 raise ValueError('unclosed block')
LOOP='num=depth+360;if(num<0||zsortObjectCount_.Length<=num){return;}if(zsortObjectCount_[num]>=30){}else{break;}depth++;'
NEW_LOOP='num = depth + 360;\nif (num < 0 || zsortObjectCount_.Length <= num) {return;}\nif (zsortObjectCount_[num] >= 30) {depth++; continue;}\n'
def transform_insert(s):
 t=tokens(s)
 if any(x[0] in ('goto','delegate','=>','yield','await','try','finally') for x in t):raise ValueError('unsupported transfer/capture/EH')
 span(s,'if(gameObjectPoolIndex_>=gameObjectPool_.Length){return;}');span(s,'int num;');a,b=span(s,'while(true){');op=next(i for i,x in enumerate(t) if x[2]==b);cl=match(t,op);root=next(i for i,x in enumerate(t) if x[0]=='{')
 if match(t,root)!=len(t)-1:raise ValueError('normal entire method required')
 if sum((1 if x[0]=='{' else -1 if x[0]=='}' else 0) for x in t[:op])!=1:raise ValueError('retry loop must be method-root owned')
 if [x[0] for x in t[op+1:cl]]!=[x[0] for x in tokens(LOOP)]:raise ValueError('reviewed retry loop body required')
 tail=s[t[cl][2]:t[-1][1]];tt=tokens(tail)
 if not tt or any(x[0] in ('break','continue','return','while','for','switch','try','catch','finally','lock','using') for x in tt):raise ValueError('terminal straight insertion tail required')
 prefix='zsortObjects_[num][zsortObjectCount_[num]]=gameObjectPoolIndex_;zsortObjectCount_[num]++;'
 if [x[0] for x in tt[:len(tokens(prefix))]]!=[x[0] for x in tokens(prefix)]:raise ValueError('reviewed insertion prefix required')
 span(tail,'int[] array=gameObjectPool_[gameObjectPoolIndex_];')
 if [x[0] for x in tt[-3:]]!=['gameObjectPoolIndex_','++',';']:raise ValueError('reviewed pool increment required')
 return s[:b]+NEW_LOOP+tail+'\nbreak;\n'+s[t[cl][1]:t[cl][2]]+s[t[-1][1]:]
