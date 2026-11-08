"""Finite original terminal-empty-test template, ordinary parent C#. No I/O."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|=>|!=|==|<=|>=|\+\+|--|&&|\|\||[^\s]',re.S)
OLD='if ((!(Config.DEBUG_CMD)) || (SHOW_DRAWNUM == 1)) {return;}'
NEW='if (Config.DEBUG_CMD && SHOW_DRAWNUM == 1) { }'
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def transform_draw_main(s):
 t=tokens(s);p=[x[0] for x in tokens(OLD)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('unique exact terminal condition required')
 if any(x[0] in ('goto','delegate','=>','yield','await') for x in t):raise ValueError('unsupported capture/flow')
 i=hits[0];last=i+len(p);depth=0
 for x in t[:i]:
  if x[0]=='{':depth+=1
  elif x[0]=='}':depth-=1
 if depth!=1 or [x[0] for x in t[last:]]!=['}']:raise ValueError('method-root terminal guard required')
 return s[:t[i][1]]+NEW+s[t[last-1][2]:]
