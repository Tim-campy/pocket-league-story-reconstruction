"""One typed Int32 shift-mask source correction; no I/O or IL manipulation."""
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_]\w*|\d+|=>|!=|==|<=|>=|<<|>>|\|=|\+\+|--|&&|\|\||[^\s]',re.S)
def tokens(s):return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(s) if not m.group().startswith(('//','/*'))]
def span(s,pat):
 t=tokens(s);p=[x[0] for x in tokens(pat)];hits=[i for i in range(len(t)-len(p)+1) if [x[0] for x in t[i:i+len(p)]]==p]
 if len(hits)!=1:raise ValueError('unique reviewed template required:'+pat)
 i=hits[0];return t[i][1],t[i+len(p)-1][2]
def transform_init(s):
 if any(x[0] in ('goto','delegate','=>','yield','await') for x in tokens(s)):raise ValueError('unsupported flow/capture')
 for p in ['string[] array=data[spriteIndex];','int num=0;','for(int j=0;j<array[1].Length();j++)','char c=array[1].CharAt(j);',"if(c=='1'){num |= 1 << (j & 0x1F);}"]:span(s,p)
 a,b=span(s,'num |= 1 << (j & 0x1F);');return s[:a]+'num |= 1 << j;'+s[b:]
