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
def matched(text,start,left,right):
 clean=masked(text);depth=0
 for i in range(start,len(clean)):
  depth+=(clean[i]==left)-(clean[i]==right)
  if depth==0:return i+1
 raise ValueError('unbalanced syntax')

def if_node(text,condition):
 clean=masked(text);rows=[]
 for m in re.finditer(r'\bif\s*\(',clean):
  stop=matched(text,m.end()-1,'(',')');test=re.sub(r'\s+','',clean[m.end():stop-1])
  if test==condition:
   begin=stop
   while clean[begin].isspace():begin+=1
   _,end=block(text,begin);rows.append((m.start(),stop,begin,end))
 if len(rows)!=1:raise ValueError('condition anchor not unique '+condition)
 return rows[0]

def owner(text,start,loopvar):
 clean=masked(text);stack=[]
 for i,c in enumerate(clean[:start]):
  if c=='{':stack.append(i)
  elif c=='}':stack.pop()
 if not stack:raise ValueError('no lexical owner')
 begin,end=block(text,stack[-1]);prefix=clean[:begin].rstrip()
 if not re.search(r'for\s*\(\s*int\s+'+loopvar+r'\s*=[^{}]+\)\s*$',prefix):raise ValueError('wrong loop owner '+loopvar)
 return begin,end

def guard_plus_positive_tail(text,condition,first,positive,loopvar):
 start,_,begin,end=if_node(text,condition);pb,pe=owner(text,start,loopvar)
 if re.sub(r'\s+','',masked(text[begin+1:end-1]))!='continue;':raise ValueError('guard has actions or noncontinue')
 tail=text[end:pe-1]
 if not masked(tail).strip()or re.match(r'\s*else\b',masked(tail)):raise ValueError('missing same-loop suffix')
 if re.search(r'\b(delegate|yield|goto)\b|=>',masked(tail)):raise ValueError('capture/unstructured tail unsupported')
 return text[:start]+'if ('+first+') { continue; }\nif ('+positive+') {'+tail+'\n}'+text[pe-1:]

def transform_move(text):
 return guard_plus_positive_tail(text,'(moveDirectBox[i]==-1)||(ch_direct==moveDirectBox[i])','moveDirectBox[i] == -1','ch_direct != moveDirectBox[i]','i')

def transform_pid(text):
 condition='((address_[0]==player2.address_[0])&&(address_[1]==player2.address_[1]))||(!(GameUtil.ChOnLine(screen_pos_,player.screen_pos_,6,player2.screen_pos_)))'
 text=guard_plus_positive_tail(text,condition,'address_[0] == player2.address_[0] && address_[1] == player2.address_[1]','GameUtil.ChOnLine(screen_pos_,player.screen_pos_,6,player2.screen_pos_)','j')
 start,_,begin,end=if_node(text,'l!=p_id_');pb,pe=owner(text,start,'l')
 if masked(text[end:pe-1]).strip():raise ValueError('not terminal loop if')
 body=text[begin+1:end-1]
 if 'player3'not in masked(body)or'num7'not in masked(body):raise ValueError('unexpected distance suffix')
 return text[:start]+'if (l == p_id_) { continue; }'+body+text[end:]

def empty_else(text,condition):
 start,_,begin,end=if_node(text,condition);clean=masked(text)
 if clean[begin+1:end-1].strip():raise ValueError('then arm is not empty')
 m=re.match(r'\s*else\s*',clean[end:])
 if not m:raise ValueError('empty if lacks else')
 ob=end+m.end();_,oe=block(text,ob)
 return start,oe,text[ob+1:oe-1]

def transform_touch(text):
 start,end,body=empty_else(text,'(component.value!=20160614)')
 innerstart,innerend,work=empty_else(body,'type!=1')
 decl=unique(r'\bint\s+type\s*=\s*e\.type\s*;',masked(body))
 if decl.end()>innerstart or masked(body[decl.end():innerstart]).strip():raise ValueError('type and dispatch not adjacent')
 if masked(body[:decl.start()]).strip():raise ValueError('unexpected prefix inside value arm')
 tail=body[innerend:]
 if re.sub(r'\s+','',masked(tail))!='returnfalse;':raise ValueError('unexpected value-arm suffix')
 if not re.fullmatch(r'\s*instance\.Push\(new\s+LanguageForm\(\)\)\s*;\s*',masked(work)):raise ValueError('unexpected case leaf')
 replacement='if (component.value == 20160614) { switch (e.type) { case 1: '+work+' break; }'+tail+'\n}'
 return text[:start]+replacement+text[end:]
