"""Restore an ordinary false Boolean guard so smcs retains the original tail."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|=>|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
NAME='useCustomFriendList'
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('unique exact guard required:'+pat)
 i=hits[0];return t[i][1],t[i+len(p)-1][2]
def transform_friend_list(s):
 t=tokens(s)
 if any(x[0] in (NAME,'goto','delegate','=>','yield','await') for x in t):raise ValueError('collision/unsupported flow')
 a,b=span(s,'if(0==0){return base.CgiGetFriendList();}');i=next(i for i,x in enumerate(t) if x[1]==a);op=next(j for j,x in enumerate(t) if x[0]=='{')
 if i!=op+1:raise ValueError('guard must be first method-root statement')
 span(s[b:],'Vector vector=new Vector();');span(s[b:],'Vector vector2=new Vector();')
 return s[:a]+'bool useCustomFriendList = false;\nif (!useCustomFriendList) {return base.CgiGetFriendList();}'+s[b:]
