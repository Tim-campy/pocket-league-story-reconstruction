"""Typed source-only composition. Parser dependency is pinned separately."""
import re
from round10_smcs_branch_joins_source import lex,vals,matching,find

def patch_tokens(text,needle,replacement):
 t=lex(text);i,n=find(t,needle);return text[:t[i].start]+replacement+text[t[i+n-1].end:]

def convert_dispatches(text):
 if sum(t.value=='switch'for t in lex(text))!=4:raise ValueError('expected exactly four dispatches')
 for iteration in range(4):
  t=lex(text);starts=[i for i,x in enumerate(t)if x.value=='switch'];i=starts[-1]
  if [x.value for x in t[i:i+4]]!=['switch','(','num3',')']:raise ValueError('wrong selector')
  opening=i+4;closing=matching(t,opening);sections=[];q=opening+1
  while q<closing:
   labels=[]
   while q<closing and t[q].value=='case':
    if t[q+2].value!=':'or not t[q+1].value.isdigit():raise ValueError('noninteger case')
    labels.append(int(t[q+1].value));q+=3
   if not labels:raise ValueError('unrecognized/default section')
   first=q
   while q<closing and t[q].value!='case':
    if t[q].value in('(','[','{'):q=matching(t,q)+1
    else:q+=1
   a,b=first,q
   while a<b and t[a].value=='{'and matching(t,a)==b-1:a+=1;b-=1
   if [x.value for x in t[b-2:b]]!=['break',';']:raise ValueError('section lacks terminal break')
   if any(x.value in('break','continue','return','goto','switch','for','while','foreach','try','delegate')for x in t[a:b-2]):raise ValueError('inner transfer or control unsupported')
   payload=text[t[first].start:t[q-1].end];cut=t[b-2].start-t[first].start;stop=t[b-1].end-t[first].start;payload=payload[:cut]+payload[stop:]
   if labels==[0,1]:labels=[1,0]
   sections.append((labels,payload))
  expected=[[2],[1,0]]if iteration==3 else[[1],[0]]
  if [x[0]for x in sections]!=expected:raise ValueError('unexpected original dispatch order')
  replacement='\n'.join(('if'if n==0 else'else if')+' ('+' || '.join('(num3 == '+str(k)+')'for k in labels)+') {\n'+payload+'\n}'for n,(labels,payload)in enumerate(sections))
  text=text[:t[i].start]+replacement+text[t[closing].end:]
 return text

def restore_guards(text):
 t=lex(text);condition='player.coliBallState_ != 4 && player.address_[0] - ball_.address_[0] >= -1 && player.address_[0] - ball_.address_[0] <= 1 && player.address_[1] - ball_.address_[1] >= -1 && player.address_[1] - ball_.address_[1] <= 1'
 i,n=find(t,'if('+condition+')');opening=i+n;closing=matching(t,opening)
 stack=[]
 for q in range(i):
  if t[q].value=='{':stack.append(q)
  elif t[q].value=='}':stack.pop()
 loops=[q for q in range(i)if t[q].value=='for'and q+1<i and t[q+1].value=='('and matching(t,q+1)+1==stack[-1]]
 if len(loops)!=1 or [z.value for z in t[loops[0]:loops[0]+5]]!=['for','(','int','j','=']:raise ValueError('guard continue owner is not lexical for-j')
 if t[opening].value!='{':raise ValueError('explicit positive body required')
 body=text[t[opening].end:t[closing].start]
 prefix='if (player.coliBallState_ == 4) { continue; }\nif (player.address_[0]-ball_.address_[0] < -1 || player.address_[0]-ball_.address_[0] > 1) { continue; }\nif (player.address_[1]-ball_.address_[1] < -1 || player.address_[1]-ball_.address_[1] > 1) { continue; }\n'
 # The original false path for float >= is blt.un. Retain >= literally.
 bt=lex(body);j,k=find(bt,'if (!(ball_.pos_.y_ >= num) &&');cond_open=j+1;cond_close=matching(bt,cond_open);guard=body[bt[j+k-1].end:bt[cond_close].start]
 body=body[:bt[j].start]+'if (ball_.pos_.y_ >= num) { continue; }\nif ('+guard+')'+body[bt[cond_close].end:]
 return text[:t[i].start]+prefix+body+text[t[closing].end:]

def restore_angles(text):
 text=patch_tokens(text,'angle = ((player3.direct_ != 0) ? ((player3.direct_ != 1) ? (190 + GameUtil.Random(70)) : (280 + GameUtil.Random(70))) : (190 + GameUtil.Random(160)));',
  'if (player3.direct_ == 0) { angle = 190 + GameUtil.Random(160); } else if (player3.direct_ == 1) { angle = 280 + GameUtil.Random(70); } else { angle = 190 + GameUtil.Random(70); }')
 return patch_tokens(text,'angle = ((player3.direct_ != 2) ? ((player3.direct_ != 1) ? (100 + GameUtil.Random(70)) : (10 + GameUtil.Random(70))) : (10 + GameUtil.Random(160)));',
  'if (player3.direct_ == 2) { angle = 10 + GameUtil.Random(160); } else if (player3.direct_ == 1) { angle = 10 + GameUtil.Random(70); } else { angle = 100 + GameUtil.Random(70); }')

def transform_collision(text):
 text=restore_guards(text)
 # Convert innermost before parents so removed break always has one owner.
 text=convert_dispatches(text)
 return restore_angles(text)
