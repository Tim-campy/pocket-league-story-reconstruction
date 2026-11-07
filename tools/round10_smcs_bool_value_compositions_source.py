"""Private source-only composition. No files, model or compiler accessed."""
import re

def masked(text):
 pattern=r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\''
 return re.sub(pattern,lambda m:' '*len(m[0]),text)

def block(text,start):
 clean=masked(text);depth=0
 if clean[start]!='{':raise ValueError('expected block')
 for index in range(start,len(text)):
  depth+=(clean[index]=='{')-(clean[index]=='}')
  if depth==0:return start,index+1
 raise ValueError('unclosed block')

def one(pattern,text):
 matches=list(re.finditer(pattern,text))
 if len(matches)!=1:raise ValueError('anchor must be unique: '+pattern)
 return matches[0]

def transform_team(method_source):
 clean=masked(method_source)
 if re.search(r'\bdelegate\b|=>',clean):raise ValueError('capture/anonymous scope unsupported')
 pattern=r'\{\s*bool\s+(?P<alias>\w+)\s*=\s*players_\[i\]\.role_\s*!=\s*0\s*;'
 match=one(pattern,clean);alias=match['alias'];start,end=block(method_source,match.start())
 if len(re.findall(r'\b'+re.escape(alias)+r'\b',clean))!=2:raise ValueError('condition alias has additional uses')
 suffix=method_source[match.end():end-1]
 head=re.match(r'\s*if\s*\(\s*'+re.escape(alias)+r'\s*\)\s*',masked(suffix))
 if not head:raise ValueError('condition alias is not the sole guarded region')
 begin,stop=block(suffix,head.end())
 if masked(suffix[stop:]).strip():raise ValueError('else or sibling changes ownership')
 positive=suffix[begin+1:stop-1];positive_clean=masked(positive)
 declaration=re.match(r'\s*(?P<type>Player)\s+(?P<name>player)\s*=\s*players_\[i\]\s*;',positive_clean)
 if not declaration:raise ValueError('expected unique Player local after role check')
 remaining=positive[declaration.end():]
 second=re.match(r'\s*if\s*\(\s*player\.seb_\s*!=\s*12\s*\)\s*',masked(remaining))
 if not second:raise ValueError('expected second positive region')
 sb,se=block(remaining,second.end())
 if masked(remaining[se:]).strip():raise ValueError('second guard has else or extra tail')
 body=remaining[sb+1:se-1]
 # The declaration joins the enclosing for block, matching original Player slot4.
 after=('if (players_[i].role_ == 0) { continue; }\n'
        +positive[:declaration.end()]+'\nif (player.seb_ == 12) { continue; }\n'+body)
 return method_source[:start]+after+method_source[end:]

def transform_edit(method_source):
 clean=masked(method_source)
 if re.search(r'\bdelegate\b|=>',clean):raise ValueError('capture/anonymous scope unsupported')
 pattern=r'\{\s*bool\s+(?P<alias>\w+)\s*=\s*et_cur\[0\]\s*%\s*3\s*==\s*0\s*;'
 match=one(pattern,clean);alias=match['alias'];start,end=block(method_source,match.start())
 if len(re.findall(r'\b'+re.escape(alias)+r'\b',clean))!=2:raise ValueError('condition alias has additional uses')
 region=method_source[match.end():end-1]
 head=re.match(r'\s*if\s*\(\s*'+re.escape(alias)+r'\s*\)\s*',masked(region))
 if not head:raise ValueError('unexpected aliased test')
 # Keep both existing branches and every leaf; no inferred local names.
 region=region[:head.start()]+re.sub(r'\b'+re.escape(alias)+r'\b','et_cur[0] % 3 == 0',region[head.start():head.end()])+region[head.end():]
 text=method_source[:start]+region+method_source[end:]
 first=one(r'\bif\s*\(\s*et_cur\[0\]\s*!=\s*5\s*\)\s*',masked(text))
 begin,stop=block(text,first.end())
 if re.match(r'\s*else\b',masked(text[stop:])):raise ValueError('already has an else')
 text=text[:first.start()]+'if (et_cur[0] == 5) { } else '+text[begin:stop]+text[stop:]
 last=one(r'\belse\s+if\s*\(\s*array4\s*!=\s*null\s*\)\s*',masked(text))
 _,stop=block(text,last.end())
 if re.match(r'\s*else\b',masked(text[stop:])):raise ValueError('terminal empty else already present')
 # This terminal empty arm preserves original br-to-next@2939, not a return.
 return text[:stop]+' else { }'+text[stop:]

def extract_method(source,name,parameter_count=None):
 clean=masked(source)
 matches=list(re.finditer(r'(?m)^[ \t]*(?:public|private|protected|internal)[^\r\n;{}]*\b'+re.escape(name)+r'\s*\((?P<params>[^\r\n]*)\)\s*(?=\{)',clean))
 if parameter_count is not None:matches=[m for m in matches if(m['params'].count(',')+1 if m['params'].strip()else 0)==parameter_count]
 if len(matches)!=1:raise ValueError('method signature not unique')
 m=matches[0];_,end=block(source,m.end());return m.start(),end,source[m.start():end]
