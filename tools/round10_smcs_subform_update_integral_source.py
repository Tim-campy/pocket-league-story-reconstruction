"""Private pure typed-control composition. No IO, model or compilation."""
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

def unique(pattern,text):
 matches=list(re.finditer(pattern,text))
 if len(matches)!=1:raise ValueError('required unique anchor: '+pattern)
 return matches[0]

def parent_block(text,position):
 stack=[]
 for index,char in enumerate(masked(text)[:position]):
  if char=='{':stack.append(index)
  elif char=='}':
   if not stack:raise ValueError('unbalanced source')
   stack.pop()
 if not stack:raise ValueError('missing lexical parent')
 return block(text,stack[-1])

def require_int32(proven):
 if proven is not True:raise ValueError('caller must prove primitive Int32 field/array types; no float complement')

def transform_empty_tests(text):
 match=unique(r'\belse\s+if\s*\(\s*!\s*flag3\s*\)\s*',masked(text))
 begin,end=block(text,match.end())
 if re.match(r'\s*else\b',masked(text[end:])):raise ValueError('flag3 already has a false arm')
 text=text[:match.start()]+'else if (flag3) { } else '+text[begin:end]+text[end:]
 match=unique(r'\bif\s*\(\s*!\s*udat_\.League_update\(\)\s*\)\s*',masked(text))
 begin,end=block(text,match.end())
 if masked(text[begin+1:end-1]).strip():raise ValueError('League_update test must have empty body')
 if re.match(r'\s*else\b',masked(text[end:])):raise ValueError('League_update empty test must not have else')
 return text[:match.start()]+'if (udat_.League_update()) '+text[begin:]

def guard_tail(text,match,positive,expected_return,allowed_prefix=None):
 begin,end=block(text,match.end());body=text[begin+1:end-1]
 if not re.fullmatch(r'\s*return\s+'+expected_return+r'\s*;\s*',masked(body)):
  raise ValueError('guard return is not the proven expression')
 parent,parent_end=parent_block(text,match.start())
 prefix=masked(text[parent+1:match.start()])
 if prefix.strip()and(allowed_prefix is None or not re.fullmatch(allowed_prefix,prefix)):
  raise ValueError('parent prefix is not the explicitly preserved declaration')
 tail=text[end:parent_end-1]
 if not masked(tail).strip()or re.match(r'\s*else\b',masked(tail)):raise ValueError('no independent sibling tail')
 if re.search(r'\bdelegate\b|=>',masked(tail)):raise ValueError('captured tail ownership unsupported')
 result='if ('+positive+') {\n'+tail+'\n} else {\n'+body+'\n}'
 return text[:match.start()]+result+text[parent_end-1:]

def transform_select_tails(text,*,primitive_int32_proven=False):
 require_int32(primitive_int32_proven)
 matches=list(re.finditer(r'\bif\s*\(\s*select_\s*!=\s*0\s*\)\s*',masked(text)))
 if len(matches)!=2:raise ValueError('both proven select0 sites are required')
 prefix=r'\s*FanActData\s+fanActData2\s*=\s*fanActData_\s*;\s*'
 for match in reversed(matches):text=guard_tail(text,match,'select_ == 0',r'Pop\(\s*\)',prefix)
 return text

def transform_for_m(text,*,primitive_int32_proven=False):
 require_int32(primitive_int32_proven)
 match=unique(r'\bif\s*\(\s*fw_cnt\[m\s*%\s*fw_cnt\.Length\]\s*<=\s*0\s*\)\s*',masked(text))
 begin,end=block(text,match.end());parent,parent_end=parent_block(text,match.start())
 clean=masked(text)
 prefix=clean[:parent]
 if not re.search(r'\bfor\s*\(\s*int\s+m\s*=\s*0\s*;\s*m\s*<\s*fw_cnt\.Length\s*\*\s*2\s*;\s*m\+\+\s*\)\s*$',prefix):
  raise ValueError('continue must target the proven for-m increment')
 if clean[parent+1:match.start()].strip()or clean[end:parent_end-1].strip():
  raise ValueError('positive wrapper must be the sole for-m statement')
 body=text[begin+1:end-1]
 if re.search(r'\bgoto\b|\bdelegate\b|=>',masked(body)):raise ValueError('unproven transfer/capture in loop tail')
 after='if (fw_cnt[m % fw_cnt.Length] > 0) { continue; }\n'+body
 return text[:match.start()]+after+text[end:]

def transform_frame6(text,*,primitive_int32_proven=False):
 require_int32(primitive_int32_proven)
 match=unique(r'\bif\s*\(\s*frame_\s*>=\s*FA2_ANIME_TIME\[6\]\s*\)\s*',masked(text))
 begin,end=block(text,match.end());parent,parent_end=parent_block(text,match.start())
 if masked(text[parent+1:match.start()]).strip():raise ValueError('frame6 guard must start its parent else')
 body=text[begin+1:end-1];tail=text[end:parent_end-1]
 if not re.search(r'\breturn\s+Pop\(\s*\)\s*;\s*$',masked(body)):raise ValueError('rank tail must end at the proven Pop return')
 if re.search(r'\btry\b|\bcatch\b|\bfinally\b|\bdelegate\b|=>',masked(body)):
  raise ValueError('EH/capture movement is unsupported')
 if not re.fullmatch(r'\s*frame_\s*=\s*FA2_ANIME_TIME\[6\]\s*-\s*1\s*;\s*',masked(tail)):
  raise ValueError('frame6 tail must be only the proven limit assignment')
 after='if (frame_ < FA2_ANIME_TIME[6]) {\n'+tail+'\n} else {\n'+body+'\n}'
 return text[:match.start()]+after+text[parent_end-1:]

def transform_type41(text,*,primitive_int32_proven=False):
 require_int32(primitive_int32_proven)
 match=unique(r'\bif\s*\(\s*type_\s*!=\s*41\s*\)\s*',masked(text))
 return guard_tail(text,match,'type_ == 41',r'Update2\(\s*\)')

def transform_residual(method_source,*,primitive_int32_proven=False):
 require_int32(primitive_int32_proven)
 text=transform_empty_tests(method_source)
 text=transform_select_tails(text,primitive_int32_proven=True)
 text=transform_for_m(text,primitive_int32_proven=True)
 text=transform_frame6(text,primitive_int32_proven=True)
 return transform_type41(text,primitive_int32_proven=True)
