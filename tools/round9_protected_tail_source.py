"""NOT_TESTED pure source proposals bound exclusively to frozen D26.
Import/API calls perform no I/O and invoke no C# tools, model, AST or oracle.
main() reads only frozen snapshot/evidence and writes this NEW WIP directory.
"""
from collections import Counter
import re
TOKEN=re.compile(r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|\x27(?:\\.|[^\x27\\])*\x27|[A-Za-z_][A-Za-z_0-9]*|[^\s]',re.M)
def tokens(s):return[(m.group(),m.start(),m.end())for m in TOKEN.finditer(s)if not m.group().startswith(('//','/*'))]
def words(s):return[t for t,_,_ in tokens(s)]
def mask(s):
 out=list(s)
 for m in TOKEN.finditer(s):
  if m.group().startswith(('//','/*','"','@"',"'")):
   for i in range(m.start(),m.end()):
    if out[i]not in '\r\n':out[i]=' '
 return ''.join(out)
def once(s,text):
 assert s.count(text)==1,('unique anchor required',text[:100],s.count(text))
 return s.index(text)
def close(s,opening):
 assert s[opening]=='{';m=mask(s);depth=0
 for i in range(opening,len(s)):
  if m[i]=='{':depth+=1
  elif m[i]=='}':
   depth-=1
   if not depth:return i+1
 raise AssertionError('unclosed lexical block')
def line_start(s,i):return s.rfind('\n',0,i)+1
def indent(s,i):
 p=s[line_start(s,i):i];assert not p.strip();return p
def no_capture(s):assert not re.search(r'\b(?:goto|yield|await|delegate)\b|=>',mask(s)),'unproven transfer/capture unsupported'
def inventories(s):
 ts=words(s)
 return Counter(t for t in ts if t.startswith(('"','@"',"'"))),Counter(t for i,t in enumerate(ts[:-1])if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)and ts[i+1]=='('and t not in {'if','for','foreach','while','switch','catch','using','lock','typeof','sizeof','default','checked','unchecked'})
def outer_try(s):
 m=re.search(r'\btry\s*{',mask(s));assert m;opening=s.index('{',m.start());return opening,close(s,opening)
def finish(before,after):
 assert inventories(before)==inventories(after),'string/character literals or named calls changed'
 assert close(after,after.index('{'))==len(after)
 return after


def transform_stream_writearray_common_exit(method_source):
 s=method_source; no_capture(s)
 anchor='Type elementType = array.GetType().GetElementType();'
 pos=once(s,anchor)+len(anchor);m=mask(s);changes=[];branches=[]
 expected=['elementType.IsArray']+['elementType == typeof('+x+')' for x in ['bool','byte','sbyte','short','ushort','int','uint','long','ulong','float','double','string','SecureByte','SecureShort','SecureInt','SecureLong','SecureString']]+['elementType.GetInterface(typeof(ISerializable).Name) != null']
 for n,condition in enumerate(expected):
  match=re.match(r'\s*if\s*\(',m[pos:]);assert match,('expected own type branch',n)
  at=pos+match.start()+len(match.group())-1
  depth=1;end=at+1
  while depth:
   assert end<len(s)
   depth+=int(m[end]=='(')-int(m[end]==')');end+=1
  assert words(s[at+1:end-1])==words(condition),('unexpected branch condition',n)
  opening=end+len(m[end:])-len(m[end:].lstrip());assert m[opening]=='{'
  closing=close(s,opening)
  block_words=tokens(s[opening:closing]);assert block_words[-3][0]=='return'and block_words[-2][0]==';'
  return_at=opening+block_words[-3][1];return_end=opening+block_words[-2][2]
  content_start=opening+1;foreach=re.match(r'\s*foreach\s*\(',m[content_start:]);assert foreach
  foreach_open=m.index('{',content_start+foreach.end());foreach_end=close(s,foreach_open)
  assert not m[foreach_end:return_at].strip(),'only one foreach plus terminal return permitted'
  assert words(s[return_at:closing])==['return',';','}']
  if_start=pos+match.start()+len(match.group())-len(match.group().lstrip())
  # Locate keyword rather than whitespace; preserve all existing body text.
  if_start=m.index('if',pos,at)
  if n:changes.append((if_start,if_start,'else '))
  changes.append((return_at,return_end,''));branches.append((if_start,closing));pos=closing
 tail=s[pos:];tail_words=words(tail)
 assert tail_words==words('throw new ArgumentException("StreamUtil.WriteArray : unsupported by " + elementType.FullName); }'),'unexpected terminal effects or statements'
 at=m.index('throw',pos);semicolon=m.index(';',at)+1
 changes.append((at,at,'else { '));changes.append((semicolon,semicolon,' }'))
 assert words(s[:branches[0][0]]).count('return')==1,'initial null-array return must remain'
 result=s
 for a,b,text in sorted(changes,reverse=True):result=result[:a]+text+result[b:]
 assert words(result).count('return')==1
 assert result.count('else if (elementType')==18
 return finish(s,result)

def transform_lineup_start_positive_array(method_source):
 s=method_source; no_capture(s);opening,end=outer_try(s)
 anchor='byte[] array = Storage.Read(3, false, "lineup.dat");';a=once(s,anchor);assert opening<a<end
 guard=once(s,'if (array == null)');gopen=s.index('{',guard);gend=close(s,gopen)
 assert words(s[gopen:gend])==['{','return',';','}'];assert not s[a+len(anchor):guard].strip()
 tail=s[gend:end-1];assert words(tail)[:6]==words('jarInflater = new JarInflater ( array')
 assert 'goto'not in words(tail)
 # The whole original try tail is copied exactly, including later returns,
 # loops, nested catch handlers and assignments. No local receiver is added.
 result=s[:guard]+'if ((object)array != null) {'+tail+'}'+s[end-1:]
 assert words(s).count('return')==words(result).count('return')+1
 return finish(s,result)

def transform_lineup_start_close(method_source):
 s=method_source;old='jarInflater?.Close();';at=once(s,old)
 assert re.search(r'\bJarInflater\s+jarInflater\s*=\s*null\s*;',mask(s))
 assert re.search(r'finally\s*{\s*jarInflater\?\.Close\(\);\s*jarInflater = null;\s*}',s),'only exact outer finalizer shape permitted'
 new='if ((object)jarInflater != null) { jarInflater.Close(); }'
 return finish(s,s[:at]+new+s[at+len(old):])

def transform_lineup_start_composition(method_source):
 return transform_lineup_start_close(transform_lineup_start_positive_array(method_source))
