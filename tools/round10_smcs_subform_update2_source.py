"""Private pure C# control composition, no AST/IL or production side effects."""
import re
from proposal import masked,block,unique,transform_layout

def enclosing(text,position):
 clean=masked(text);stack=[]
 for i,c in enumerate(clean[:position]):
  if c=='{':stack.append(i)
  elif c=='}':stack.pop()
 if not stack:raise ValueError('guard lacks lexical owner')
 return block(text,stack[-1])

def terminal_tail(text,match,positive,ending='return',tail_anchor=None):
 """Move only suffix in same lexical block; retain every prefix declaration."""
 clean=masked(text);pbegin,pend=enclosing(text,match.start())
 body_start=match.end()
 while body_start<len(text)and clean[body_start].isspace():body_start+=1
 begin,end=block(text,body_start);body=text[begin+1:end-1];tail=text[end:pend-1]
 bodyclean=masked(body)
 if ending=='return':
  if not re.search(r'\breturn\s+[^;{}]+;\s*$',bodyclean):raise ValueError('guard is not terminal value return')
  if re.search(r'\b(if|for|while|do|try|using|lock|switch|delegate|goto|break|continue)\b|=>',bodyclean):raise ValueError('guard body has nested control/capture')
 elif ending=='continue':
  if not re.fullmatch(r'\s*continue\s*;\s*',bodyclean):raise ValueError('expected lone continue')
 else:raise ValueError('unsupported terminator')
 if not masked(tail).strip()or re.match(r'\s*else\b',masked(tail)):raise ValueError('no same-block suffix')
 if tail_anchor and tail_anchor not in masked(tail):raise ValueError('wrong suffix owner')
 if ending=='continue':
  prefix=clean[:pbegin].rstrip()
  # Deliberately demand actual for-l block header, not a coincident IL target.
  if not re.search(r'for\s*\(\s*int\s+l\s*=\s*0\s*;[^{};]+;[^{}]+\)\s*$',prefix):raise ValueError('continue is not owned by intended outer for-l')
  result='if ('+positive+') {'+tail+'\n}'
 else:result='if ('+positive+') {'+tail+'\n} else {'+body+'\n}'
 return text[:match.start()]+result+text[pend-1:]

def guards(text,pattern,positive,count,ending='return',tail_anchor=None):
 rows=list(re.finditer(pattern,masked(text)))
 if len(rows)!=count:raise ValueError('guard count mismatch '+str((pattern,len(rows),count)))
 for match in reversed(rows):
  predicate=positive(match)if callable(positive)else positive
  text=terminal_tail(text,match,predicate,ending,tail_anchor)
 return text

def restore_inner_strength(text):
 rows=list(re.finditer(r'\bif\s*\(\s*!tenantData4\.IsStreangth\(\)\s*\)',masked(text)))
 selected=[]
 for match in rows:
  pb,pe=enclosing(text,match.start());prefix=masked(text[:pb]).rstrip()
  if prefix.endswith('else')and not masked(text[pb+1:match.start()]).strip()and'TenantData.ProcStrenSpecialTenant' in masked(text[match.end():pe]):selected.append(match)
 if len(selected)!=1:raise ValueError('inner strength owner is not unique')
 return terminal_tail(text,selected[0],'tenantData4.IsStreangth()',tail_anchor='TenantData.ProcStrenSpecialTenant')

def restore_pay(text):
 return guards(text,r'\bif\s*\(\s*!apdat_\.ChPayMoney\(\s*(tenantData[34]\.(?:buildCost_|paramStrengthCost_))\s*\)\s*\)',lambda m:'apdat_.ChPayMoney('+m[1]+')',2)

def restore_select(text,count=5):
 return guards(text,r'\bif\s*\(\s*select_\s*!=\s*0\s*\)','select_ == 0',count)

def restore_page(text):
 return guards(text,r'\bif\s*\(\s*page_\s*!=\s*0\s*\)','page_ == 0',1)

def restore_states(text):
 text=guards(text,r'\bif\s*\(\s*sponsorData\.state_\s*!=\s*2\s*\)','sponsorData.state_ == 2',1)
 return guards(text,r'\bif\s*\(\s*fanBonusData\.state_\s*!=\s*0\s*\)','fanBonusData.state_ == 0',1)

def restore_event_loop(text):
 return guards(text,r'\bif\s*\(\s*apdat_\.CheckAlreadyEvent\(189\)\s*\)','!apdat_.CheckAlreadyEvent(189)',1,ending='continue',tail_anchor='for (int m')

def restore_empty_type48(text):
 m=unique(r'\belse\s+if\s*\(\s*type_\s*!=\s*48\s*\)',masked(text))
 return text[:m.start()]+'else if (type_ == 48) { } else'+text[m.end():]

def restore_empty_select2(text):
 m=unique(r'\belse\s+if\s*\(\s*select_\s*!=\s*2\s*&&\s*select_\s*==\s*3\s*\)',masked(text))
 return text[:m.start()]+'else if (select_ == 2) { } else if (select_ == 3)'+text[m.end():]

STAGES=[('layout',transform_layout),('inner_strength',restore_inner_strength),('pay',restore_pay),('select',restore_select),('page',restore_page),('states',restore_states),('event_loop',restore_event_loop),('type48',restore_empty_type48),('select2',restore_empty_select2)]
def transform_integral(method_source):
 for _,fn in STAGES:method_source=fn(method_source)
 return method_source
