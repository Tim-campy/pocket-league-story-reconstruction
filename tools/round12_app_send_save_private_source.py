"""Finite normal-C# control composition for one reviewed parent declaration. No I/O."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pattern):
 t=tokens(s);p=[x[0] for x in tokens(pattern)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('exact unique template absent: '+pattern)
 i=hits[0];return t[i][1],t[i+len(p)-1][2]
def block(s,start):
 t=tokens(s);i=next(i for i,x in enumerate(t) if x[1]>=start and x[0]=='{');depth=0
 for x in t[i:]:
  if x[0]=='{':depth+=1
  elif x[0]=='}':
   depth-=1
   if not depth:return t[i][1],x[1],x[2]
 raise ValueError('unclosed block')
def replace(s,pattern,new):a,b=span(s,pattern);return s[:a]+new+s[b:]
def positive_outer_dispatch(s):
 return replace(s,'goto IL_0817; } if (sendState_ != 1) { goto IL_0817; }','} else if (sendState_ == 1) {')
def positive_save_ternaries(s):
 old='((save_ == null) ? string.Empty : save_.strs[0])';new='((save_ != null) ? save_.strs[0] : string.Empty)'
 if s.count(old)!=2:raise ValueError('two save ternary occurrences required')
 return s.replace(old,new)
def retry_for_and_positive_catches(s):
 if any(x[0] in ('recoveredSendRetry','recoveredSendOuterEnd','yield','delegate','=>') for x in tokens(s)):raise ValueError('unsupported ownership/collision')
 a,b=span(s,'int num7 = 0;');w0,w1=span(s[b:],'while (true) { IL_07cd:');w0+=b;w1+=b;op,cl,end=block(s,w0);body=s[op+1:cl]
 ia,ib=span(body,'if (num7 < array4.Length)');iop,icl,iend=block(body,ib);ibod=body[iop+1:icl];ra,rb=span(ibod,'while (true)');rop,rcl,rend=block(ibod,rb);retry=ibod[rop+1:rcl];assert [x[0] for x in tokens(ibod[rend:])]==['break',';']
 ta,tb=span(retry,'try { Post (');top,tcl,tend=block(retry,ta);draw=retry[:ta];post=retry[top+1:tcl];assert [x[0] for x in tokens(draw)][:4]==['Draw','(','text8',')'];assert post.strip().startswith('Post(')
 ca,cb=span(retry,'catch (Exception t)');cop,ccl,cend=block(retry,cb);assert [x[0] for x in tokens(retry[cend:])]==['num7','++',';','goto','IL_07cd',';'];catch=retry[cop+1:ccl];ga,gb=span(catch,'if (dialog.Show() != 8) { continue; }');prefix=catch[:ga];cancel=catch[gb:];assert [x[0] for x in tokens(cancel)][-2:]==['break',';'];last=span(cancel,'break;');cancel=cancel[:last[0]]+'goto recoveredSendOuterEnd;'+cancel[last[1]:]
 completion=body[iend:];assert [x[0] for x in tokens(completion)][-2:]==['return',';']
 new='for (int num7 = 0; num7 < array4.Length; num7++) {\n recoveredSendRetry:\n'+draw+'try {'+post+'continue;\n} catch (Exception t) {'+prefix+'if (dialog.Show() == 8) {'+cancel+'}\n}\n goto recoveredSendRetry;\n}\n'+completion
 s=s[:a]+new+s[end:];s=replace(s,'goto end_IL_038e; IL_0817: JThread.Sleep(100L); end_IL_038e:;','}\n JThread.Sleep(100L);')
 ca,cb=span(s,'catch (Exception t2)');cop,ccl,cend=block(s,cb);body=s[cop+1:ccl];ga,gb=span(body,'if (dialog3.Show() != 8) { continue; }');new=body[:ga]+'if (dialog3.Show() == 8) {'+body[gb:]+'}\n';s=s[:cop+1]+new+s[ccl:cend]+'\n recoveredSendOuterEnd:;'+s[cend:]
 return s
def transform_send_save(s):
 s=positive_outer_dispatch(s);s=positive_save_ternaries(s);return retry_for_and_positive_catches(s)
