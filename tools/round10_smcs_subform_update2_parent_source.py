"""Private pure source calculus; existing delegates/storeys are never authored."""
import re

def masked(text):
 pattern=r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\''
 return re.sub(pattern,lambda m:' '*len(m[0]),text)
def block(text,start):
 clean=masked(text);depth=0
 if clean[start]!='{':raise ValueError('expected lexical block')
 for i in range(start,len(text)):
  depth+=(clean[i]=='{')-(clean[i]=='}')
  if depth==0:return start,i+1
 raise ValueError('unclosed block')
def unique(pattern,text):
 rows=list(re.finditer(pattern,text))
 if len(rows)!=1:raise ValueError('expected unique anchor: '+pattern)
 return rows[0]
def remove_key_alias(text):
 match=unique(r'\{\s*bool\s+(?P<alias>\w+)\s*=\s*canvas_\.CheckKeyRepeat\(\s*131072\s*\)\s*;',masked(text))
 alias=match['alias'];_,end=block(text,match.start());region=text[match.end():end-1]
 if len(re.findall(r'\b'+re.escape(alias)+r'\b',masked(text)))!=2:raise ValueError('alias has additional uses/capture')
 test=re.match(r'\s*if\s*\(\s*'+re.escape(alias)+r'\s*\)\s*',masked(region))
 if not test:raise ValueError('unexpected alias use')
 begin,stop=block(region,test.end())
 if masked(region[stop:]).strip():raise ValueError('aliased guard has else or sibling statements')
 after='if (canvas_.CheckKeyRepeat(131072)) '+region[begin:stop]
 return text[:match.start()]+after+text[end:]
def add_named_join(text):
 name='recoveredMatchInfoJoin'
 if re.search(r'\b'+name+r'\b',masked(text)):raise ValueError('synthetic Join name collision')
 match=unique(r'\bif\s*\(\s*udat_\.GetMatchInfoJoin\(\)\s*==\s*0\s*&&',masked(text))
 after='int '+name+' = udat_.GetMatchInfoJoin();\nif ('+name+' == 0 &&'
 return text[:match.start()]+after+text[match.end():]
def transform_layout(method_source):
 return add_named_join(remove_key_alias(method_source))
