"""Private source-only declaration/dispatch calculus; no compiler/model/IO."""
import re

PLANS={
 'num4':([0,1,2,3,4,5],[]),'num9':([21,22,25],[]),
 'num14':([6,7,8,9,10],[7,10]),'num18':([14,12,15,16],[]),
 'num21':([17,13,18,19],[]),'num23':([20,21,22,23,24,29],[]),
 'num25':([26,27,28],[]),'num27':([0,1],[]),
}

def masked(text):
 pattern=r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\''
 return re.sub(pattern,lambda m:' '*len(m[0]),text)

def block(text,start):
 clean=masked(text);depth=0
 if clean[start]!='{':raise ValueError('expected block')
 for i in range(start,len(text)):
  depth+=(clean[i]=='{')-(clean[i]=='}')
  if depth==0:return start,i+1
 raise ValueError('unclosed block')

def unique(pattern,text):
 matches=list(re.finditer(pattern,text))
 if len(matches)!=1:raise ValueError('anchor must be unique: '+pattern)
 return matches[0]

def unwrap(text):
 clean=masked(text);start=len(clean)-len(clean.lstrip())
 if clean[start:start+1]=='{':
  _,end=block(text,start)
  if not clean[end:].strip():return text[start+1:end-1]
 return text

def remove_break(text):
 match=re.search(r'\bbreak\s*;\s*$',masked(text))
 if not match:raise ValueError('expected switch-owned terminal break')
 return text[:match.start()]

def case_body(content):
 body=unwrap(content);clean=masked(body)
 if re.search(r'\bcontinue\b',clean):raise ValueError('continue ownership not implemented for these case templates')
 if re.search(r'\bbreak\s*;\s*$',clean):
  body=remove_break(body)
 elif re.search(r'\bbreak\b',clean):
  # Proven cases0/2/23: an if arm breaks the switch, followed by a return.
  head=re.match(r'\s*if\s*\(',clean)
  if not head:raise ValueError('unproven nonterminal switch break')
  opened=clean.find('{',head.end());begin,end=block(body,opened)
  then=remove_break(body[begin+1:end-1]);tail=body[end:]
  if not re.match(r'\s*return\b',masked(tail)):
   raise ValueError('switch guard tail must be the original return')
  body=body[:begin+1]+then+'\n} else {\n'+tail+'\n}'
 if re.search(r'\bbreak\b',masked(body)):
  raise ValueError('break ownership is not proven for this case')
 return body

def parse_sections(statement,selector):
 clean=masked(statement);head=re.match(r'\s*switch\s*\(\s*'+re.escape(selector)+r'\s*\)\s*',clean)
 if not head:raise ValueError('selector must be the proven named Int32')
 begin,end=block(statement,head.end());depth=0;labels=[];i=begin+1
 pattern=re.compile(r'case\s+(-?\d+)\s*:|default\s*:')
 while i<end-1:
  match=pattern.match(clean,i)if depth==0 else None
  if match:
   if match[1]is None:raise ValueError('unexpected default section')
   labels.append((match.start(),match.end(),int(match[1])));i=match.end();continue
  depth+=(clean[i]=='{')-(clean[i]=='}');i+=1
 result={}
 for j,(_,start,key)in enumerate(labels):
  stop=labels[j+1][0]if j+1<len(labels)else end-1
  content=statement[start:stop]
  if not masked(content).strip():raise ValueError('grouped case not in this proven inventory')
  if key in result:raise ValueError('duplicate case')
  result[key]=case_body(content)
 return result

def transform_dispatches(method_source):
 text=method_source
 if re.search(r'\bdelegate\b|=>|\bgoto\b',masked(text)):raise ValueError('unproven capture/goto scope')
 if len(re.findall(r'\bswitch\b',masked(text)))!=8:raise ValueError('expected all eight proven dispatches')
 edits=[]
 for selector,(keys,empty)in PLANS.items():
  match=unique(r'\bswitch\s*\(\s*'+selector+r'\s*\)\s*',masked(text));_,end=block(text,match.end())
  cases=parse_sections(text[match.start():end],selector)
  if list(cases)!=[key for key in keys if key not in empty]:raise ValueError('case order/inventory differs: '+selector)
  for key in empty:cases[key]=''
  clauses=[]
  for key in keys:clauses.append(('if'if not clauses else'else if')+' ('+selector+' == '+str(key)+') {\n'+cases[key]+'\n}')
  edits.append((match.start(),end,'\n'.join(clauses)))
 for start,end,after in sorted(edits,reverse=True):text=text[:start]+after+text[end:]
 return text

def transform_layout(method_source):
 text=transform_dispatches(method_source)
 call=unique(r'if\s*\(\s*udat_\.GetMatchInfoJoin\(\)\s*==\s*0\s*\)',masked(text))
 name='recoveredMatchInfoJoin'
 if re.search(r'\b'+name+r'\b',masked(text)):raise ValueError('named Join local collision')
 text=text[:call.start()]+'int '+name+' = udat_.GetMatchInfoJoin();\nif ('+name+' == 0)'+text[call.end():]
 unused='recoveredUnusedLocal65'
 if re.search(r'\b'+unused+r'\b',masked(text)):raise ValueError('unused local collision')
 anchor=unique(r'\bint\s+num38\s*=\s*9\s*;',masked(text))
 text=text[:anchor.start()]+'int '+unused+';\n'+text[anchor.start():]
 return text

def transform_assignments(method_source):
 text=method_source
 plans=[
  ('num3 = ((!MyFormBase.Jp()) ? 124 : 104);','if (MyFormBase.Jp()) { num3 = 104; } else { num3 = 124; }'),
  ('num28 = ((num28 != -1) ? 1 : 0);','if (num28 == -1) { num28 = 0; } else { num28 = 1; }'),
  ('num28 = ((num28 != 0) ? app_.GetOrientation() : (-1));','if (num28 == 0) { num28 = -1; } else { num28 = app_.GetOrientation(); }'),
  ('num30 = ((num30 != -1) ? 1 : 0);','if (num30 == -1) { num30 = 0; } else { num30 = 1; }'),
  ('num30 = ((num30 != 0) ? app_.GetOrientation() : (-1));','if (num30 == 0) { num30 = -1; } else { num30 = app_.GetOrientation(); }'),
 ]
 for before,after in plans:
  if text.count(before)!=1:raise ValueError('unproven typed assignment: '+before)
  text=text.replace(before,after)
 return text

def transform_layout_assignments(method_source):
 return transform_assignments(transform_layout(method_source))
