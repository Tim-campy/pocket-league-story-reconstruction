"""Pure dialog99 guard/tail composition; the frozen parent module is untouched."""
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

def enclosing_block(text,position):
 clean=masked(text);stack=[]
 for index,char in enumerate(clean[:position]):
  if char=='{':stack.append(index)
  elif char=='}':
   if not stack:raise ValueError('unbalanced source')
   stack.pop()
 if not stack:raise ValueError('no parent block')
 return block(text,stack[-1])

def transform_dialog99(method_source):
 clean=masked(method_source)
 pattern=r'\bif\s*\(\s*dialog_\.dialogId_\s*!=\s*99\s*\)\s*'
 matches=list(re.finditer(pattern,clean))
 if len(matches)!=1:raise ValueError('dialog99 guard must be unique')
 guard=matches[0];begin,end=block(method_source,guard.end())
 returned=method_source[begin+1:end-1]
 if not re.fullmatch(r'\s*return\s+formMan_\.RemoveAllForms\(\s*\)\s*;\s*',masked(returned)):
  raise ValueError('guard must contain only the proven value return')
 parent,parent_end=enclosing_block(method_source,guard.start())
 tail=method_source[end:parent_end-1]
 if not re.fullmatch(r'\s*dialog_\s*=\s*null\s*;\s*[A-Za-z_][\w.]*\s*\([^;{}]*\)\s*;\s*',masked(tail)):
  raise ValueError('expected only dialog clear and browser call in this sibling tail')
 after='if (dialog_.dialogId_ == 99) {\n'+tail+'\n} else {\n'+returned+'\n}'
 return method_source[:guard.start()]+after+method_source[parent_end-1:]

def compose_primary(method_source,base_transform):
 """base_transform must be the hash-checked frozen transform_layout_assignments."""
 return transform_dialog99(base_transform(method_source))
