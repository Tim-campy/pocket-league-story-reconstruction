"""Finite branch-layout source templates. Parent C# only; no file I/O."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+(?:\.\d+)?f?|=>|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('unique template required:'+pat)
 i=hits[0];return t[i][1],t[i+len(p)-1][2]
def matching(t,i):
 n=0
 for j in range(i,len(t)):
  if t[j][0]=='{':n+=1
  elif t[j][0]=='}':
   n-=1
   if n==0:return j
 raise ValueError('unclosed block')
def reject(s):
 if any(x[0] in ('goto','delegate','=>','yield','await') for x in tokens(s)):raise ValueError('unsupported preexisting flow/capture')
def replace(s,old,new):a,b=span(s,old);return s[:a]+new+s[b:]
def transform_get_touch(s):
 reject(s);a,b=span(s,'switch(type){');t=tokens(s);op=next(i for i,x in enumerate(t) if x[2]==b);cl=matching(t,op)
 if [x[0] for x in t[cl+1:]]!=['}']:raise ValueError('switch must be method terminal')
 depth=0;defaults=[]
 for i in range(op+1,cl):
  if t[i][0]=='{':depth+=1
  elif t[i][0]=='}':depth-=1
  elif t[i][0]=='default' and not depth:defaults.append(i)
 if len(defaults)!=1:raise ValueError('unique direct default required')
 i=defaults[0];p=['default',':','if','(','Config','.','PRINT',')','{']
 if [x[0] for x in t[i:i+len(p)]]!=p:raise ValueError('default PRINT guard required')
 end=matching(t,i+len(p)-1)
 if [x[0] for x in t[end+1:cl]]!=['return','0',';']:raise ValueError('default must end in return0')
 if [x[0] for x in t[i+len(p):end]].count('throw')!=1:raise ValueError('single default throw required')
 rs=t[end+1][1];re_=t[cl-1][2]
 return s[:rs]+'break;'+s[re_:t[cl][2]]+'\nreturn 0;'+s[t[cl][2]:]
def transform_set_touch(s):
 reject(s);span(s,'type = (int)(type & 0xFFFF0000u);')
 old='int num=type; if(num!=131072){base.SetTouchValue(type,value);}else{select_=value;}'
 return replace(s,old,'switch (type) {case 131072: select_ = value; break; default: base.SetTouchValue(type,value); break;}')
def transform_trackbar(s):
 reject(s);old='int typeAlias=type; if((typeAlias==0)||(typeAlias!=9)){}else{Set(v,e.dx,e.component);}'
 return replace(s,old,'switch (type) {case 0: break; case 9: Set(v,e.dx,e.component); break;}')
def transform_billing_touch(s):
 reject(s);a,b=span(s,'int type=e.type; if((type!=1)&&(type!=2)){}else{');t=tokens(s);op=next(i for i,x in enumerate(t) if x[2]==b);cl=matching(t,op);payload=s[b:t[cl][1]]
 expected='select2_=component.value; if(e.type==1){canvas_.KeyClick(2048);}'
 if [x[0] for x in tokens(payload)]!=[x[0] for x in tokens(expected)]:raise ValueError('reviewed billing payload required')
 return s[:a]+'switch (e.type) {case 2: goto case 1; case 1: {'+payload+'break;}}'+s[t[cl][2]:]
def transform_goal_depth(s):
 reject(s);a,b=span(s,'if((parts_id==3)){');t=tokens(s);op=next(i for i,x in enumerate(t) if x[2]==b);cl=matching(t,op)
 expected='if(udat_.ball_.pos_.z_<GOAL1_GLOBAL_POS[2].z_){return udat_.ball_.camera_pos_[1]-1;}'
 if [x[0] for x in tokens(s[b:t[cl][1]])]!=[x[0] for x in tokens(expected)]:raise ValueError('reviewed goal payload required')
 if [x[0] for x in t[cl+1:]]!=['return','0',';','}']:raise ValueError('root return0 suffix required')
 return s[:t[cl][2]]+' else if (parts_id == 4) {} else if (parts_id == 5) {} else if (parts_id == 6) {} else if (parts_id == 7) {}'+s[t[cl][2]:]
