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
def merge_x_and_zero_chain(text):
 clean=masked(text)
 if re.search(r'\b(delegate|yield)\b|=>',clean):raise ValueError('captures/iterators unsupported')
 pattern=r'int\s+num2\s*;\s*int\s+num3\s*;\s*int\s+num\s*=\s*\(num2\s*=\s*\(num3\s*=\s*0\)\)\s*;'
 match=unique(pattern,clean)
 # Four initial locals represent basex/basey/outx/outy and original stloc4,3,2,1.
 text=text[:match.start()]+'int num9, num, num2, num3;\nnum9 = num = num2 = num3 = 0;'+text[match.end():]
 clean=masked(text);rows=list(re.finditer(r'\bint\s+num9\s*(?=[=;])',clean))
 if len(rows)!=5:raise ValueError('expected five child basex declarations')
 for row in reversed(rows):
  after=text[row.end():]
  if after.lstrip().startswith(';'):
   end=row.end()+len(after)-len(after.lstrip())+1
   text=text[:row.start()]+text[end:]
  else:text=text[:row.start()]+'num9'+text[row.end():]
 return text

def condition_local_money(text):
 clean=masked(text)
 name='recoveredMoneyStyle'
 if re.search(r'\b'+name+r'\b',clean):raise ValueError('synthetic name collision')
 match=unique(r'else\s+if\s*\(\s*AppData\.GetInstance\(\)\.sysSave_\.ints\[14\]\s*==\s*1\s*\)',clean)
 # Preserve the whole existing suffix, bounded by its actual lexical parent.
 stack=[]
 for i,c in enumerate(clean[:match.start()]):
  if c=='{':stack.append(i)
  elif c=='}':stack.pop()
 if not stack:raise ValueError('condition has no lexical parent')
 _,end=block(text,stack[-1]);tail=text[match.end():end-1]
 if not re.search(r'^\s*\{',masked(tail))or'DrawMoney2'not in masked(tail):raise ValueError('unexpected money suffix')
 after='else { bool '+name+' = AppData.GetInstance().sysSave_.ints[14] == 1;\nif ('+name+')'+tail+'\n}'
 return text[:match.start()]+after+text[end-1:]

def transform_layout(method_source):
 return condition_local_money(merge_x_and_zero_chain(method_source))
