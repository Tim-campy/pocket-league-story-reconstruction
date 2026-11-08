"""Two independent, finite normal-C# templates. No I/O at import or calls."""
import re

TOKEN=re.compile(r'//[^\r\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_][A-Za-z_0-9]*|\d+|\?\?|!=|==|&&|\|\||=>|[^\s]',re.S)
def _tokens(text):
 return [(m.group(),m.start(),m.end()) for m in TOKEN.finditer(text) if not m.group().startswith(('//','/*'))]
def _replace_once(text,old,new):
 tokens=_tokens(text);expected=[t[0] for t in _tokens(old)];n=len(expected);hits=[i for i in range(len(tokens)-n+1) if [t[0] for t in tokens[i:i+n]]==expected]
 if len(hits)!=1:raise ValueError('expected one exact token template')
 a,b=tokens[hits[0]][1],tokens[hits[0]+n-1][2]
 if '//' in text[a:b] or '/*' in text[a:b]:raise ValueError('comments inside replaced expression')
 return text[:a]+new+text[b:]
def transform_version_char_concat(text):
 """Caller binds Application.unityVersion[i] as System.Char string indexer."""
 if any(t[0] in ('delegate','=>','unsafe','yield','await') for t in _tokens(text)):raise ValueError('unsupported callable')
 return _replace_once(text,'Application.unityVersion[i].ToString() ?? ""','(Application.unityVersion[i] + "")')
def transform_pictograph_local_inside(text):
 """Caller binds pictograph to local Pictograph and dictionary/key as recorded."""
 ts=[t[0] for t in _tokens(text)]
 prefix=[t[0] for t in _tokens('Pictograph pictograph = GetPictograph(key);')]
 if not any(ts[i:i+len(prefix)]==prefix for i in range(len(ts)-len(prefix)+1)):raise ValueError('local declaration missing')
 if any(t in ('delegate','=>','yield','await','ref','out') for t in ts):raise ValueError('unsupported local lifetime')
 return _replace_once(text,'pictograph = (pictographList_[key] = new Pictograph(key, seb, frame, wait, x, y, margin));','pictographList_[key] = (pictograph = new Pictograph(key, seb, frame, wait, x, y, margin));')
