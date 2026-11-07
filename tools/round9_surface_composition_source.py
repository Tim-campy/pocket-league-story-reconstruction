"""Pure Surface composition; all repository/file I/O is guarded in main().

No compiler, AST transformer, slot model or verifier. Inputs outside these
narrow textual/type-declaration preconditions are rejected, never guessed.
"""
from collections import Counter
import re

TOKEN=re.compile(r'//[^\r\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|"(?:\\.|[^"\\])*"|\x27(?:\\.|[^\x27\\])*\x27|[A-Za-z_][A-Za-z_0-9]*|[^\s]',re.M)
def tokens(source):
 return [(m.group(),m.start(),m.end())for m in TOKEN.finditer(source)if not m.group().startswith(('//','/*'))]
def mask(source):
 result=list(source)
 for m in TOKEN.finditer(source):
  if m.group().startswith(('//','/*','"','@"',"'")):
   for n in range(m.start(),m.end()):
    if result[n]not in '\r\n':result[n]=' '
 return ''.join(result)
def once(source,text):
 assert source.count(text)==1,('required unique anchor',text[:100],source.count(text))
 return source.index(text)
def close(source,start):
 assert source[start]=='{'
 masked=mask(source);depth=0
 for i in range(start,len(source)):
  if masked[i]=='{':depth+=1
  elif masked[i]=='}':
   depth-=1
   if not depth:return i+1
 raise AssertionError('unclosed lexical block')
def indent_at(source,start):
 prefix=source[source.rfind('\n',0,start)+1:start]
 assert not prefix.strip(),'statement anchor must begin after indentation only'
 return prefix
def apply(source,changes):
 for start,end,text in sorted(changes,reverse=True):source=source[:start]+text+source[end:]
 return source
def literal_inventory(source):return Counter(t for t,_,_ in tokens(source)if t.startswith(('"','@"',"'")))
def call_inventory(source):
 words=[t for t,_,_ in tokens(source)]
 return Counter(t for i,t in enumerate(words[:-1])if re.fullmatch('[A-Za-z_][A-Za-z_0-9]*',t)and words[i+1]=='('and t not in {'if','for','foreach','while','switch','catch','using','lock','typeof','sizeof','default','checked','unchecked'})

def calculate_surface(method_source):
 """Return after/changes/reasons/checks. No method/header/class identity needed.

 Deliberately supports only the actual explicitly int-typed action selector.
 An enum/unknown selector is rejected instead of assuming case3 is typed.
 """
 source=method_source;masked=mask(source);changes=[];reasons=[]
 assert len(re.findall(r'\bswitch\s*\(',masked))==1,'exactly one switch owner required'
 selector_declarations=list(re.finditer(r'\bint\s+action\s*=\s*[^;]+;',masked))
 assert len(selector_declarations)==1,'selector must be explicitly int action; enum/unknown is unsupported'
 # Reject shadowed/ambiguous declaration forms even if an int declaration exists.
 assert not re.search(r'\b(?:var|\w+(?:\.\w+)*)\s+action\s*(?:;|=)',masked.replace('int action','ACTION_DECL_REMOVED')),'ambiguous/shadowed selector declaration'
 sw=once(source,'switch (action)');opening=source.index('{',sw);switch_end=close(source,opening)
 declaration=selector_declarations[0]
 depth=lambda offset:masked[:offset].count('{')-masked[:offset].count('}')
 assert declaration.start()<sw and depth(declaration.start())==depth(sw)==1,'selector must be declared in method top block before switch'
 labels=[int(x)for x in re.findall(r'^\s*case\s+(\d+)\s*:',masked[opening:switch_end],re.M)]
 assert labels==[0,1,2]and not re.search(r'\bdefault\s*:',masked[opening:switch_end]),'require only existing cases0,1,2 without default'
 label2=re.search(r'(?m)^(\s*)case 2:',source[opening:switch_end]);assert label2
 label_indent=label2.group(1).splitlines()[-1]
 closing_line=source.rfind('\n',0,switch_end-1)+1
 assert not source[closing_line:switch_end-1].strip()
 changes.append((closing_line,closing_line,label_indent+'case 3:\r\n'+label_indent+'\tbreak;\r\n'))
 reasons.append(dict(edit='add-typed-terminal-empty-case3',original_offsets=[335,337,358,1743],selector='action',selector_type='System.Int32 from unique explicit int declaration; enum/unknown rejected',target_owner='only enclosing switch; break reaches post-switch join',evidence='original0..3table fourth target1743 equals out-of-range target1743; source currently0..2'))

 expression='num8 * num8 + num9 * num9 < 225f'
 marker='if (!('+expression+'))'
 a=once(source,marker);i=indent_at(source,a);block=source.index('{',a);end=close(source,block)
 case1_pos=once(source,'case 1:');case2_pos=once(source,'case 2:')
 assert case1_pos<a<end<case2_pos,'distance guard must belong to case1'
 assert len(re.findall(r'\bfloat\s+num8\s*=',masked))==1 and len(re.findall(r'\bfloat\s+num9\s*=',masked))==1,'float operands must be explicit builtin locals'
 fallback=source[block:end]
 assert [t for t,_,_ in tokens(fallback)]==['{','touchEvent','=','CreateTouchEvent','(','1',',','c',')',';','goto','case','2',';','}'],'fallback effects/target must be exact'
 add='taskBuffer_.Add(CreateTouchEvent(5, c));'
 rest=source[end:];space=re.match(r'\s*',rest).end();add_start=end+space
 assert source.startswith(add,add_start)and add_start+len(add)<case2_pos
 after='if ('+expression+')\r\n'+i+'{\r\n'+i+'\t'+add+'\r\n'+i+'}\r\n'+i+'else\r\n'+i+fallback
 changes.append((a,add_start+len(add),after))
 reasons.append(dict(edit='restore-case1-positive-float-if-and-arm-order',original_offsets=[535,537,556,558,566,568,573],expression_preserved=expression,comparison='keep <225f; no opposite relational substitution; NaN takes else as before',target_owner='goto case2 within same sole switch',evidence='originalbge.un.s558;Add5;br.s573;Create1/store11/goto788 versus reversed guard without2-byte br.s'))

 loop='for (i = num7; i < taskBuffer_.Size() && taskBuffer_.ElementAt(i).IsPositive(); i++)'
 a=once(source,loop);i=indent_at(source,a);block=source.index('{',a);end=close(source,block)
 assert not source[block+1:end-1].strip(),'loop body must be strictly empty'
 declaration=re.search(r'\bint\s+i\s*;\s*$',source[:a]);assert declaration,'preserve existing immediately preceding int i declaration'
 declaration_text=source[declaration.start():declaration.end()]
 after='for (i = num7; i < taskBuffer_.Size(); i++)\r\n'+i+'{\r\n'+i+'\tif (!taskBuffer_.ElementAt(i).IsPositive())\r\n'+i+'\t\tbreak;\r\n'+i+'}'
 changes.append((a,end,after))
 reasons.append(dict(edit='move-IsPositive-predicate-to-loop-body-break-guard',original_offsets=[1940,1944,1946,1964,1966,1972,1985,1987],existing_index_declaration_preserved=declaration_text,target_owner='break exits the immediately enclosing for; no new switch/loop in body',evaluation_order='Size before ElementAt/IsPositive; false breaks before increment; initialize/increment unchanged',evidence='original bodyPositive1946/falseexit1987,inc1966,testSize1972/blt1946; current empty loop puts both predicates at bottom'))
 spans=sorted(changes)
 assert all(a[1]<=b[0]for a,b in zip(spans,spans[1:])),'overlapping edits'
 result=apply(source,changes)
 assert literal_inventory(source)==literal_inventory(result),'literal leaf inventory changed'
 assert call_inventory(source)==call_inventory(result),'call-name inventory changed'
 assert close(result,result.index('{'))==len(result),'method lexical braces invalid'
 return dict(after=result,changes=changes,reasons=reasons,checks=dict(selector_type='System.Int32',empty_case_label=3,selector_enum_not_assumed=True,literal_token_multiset_unchanged=True,call_name_token_multiset_unchanged=True,existing_index_declaration_preserved=True,fallback_block_copied_exact=True,comparison_expression_copied_exact=True,new_named_variables=0,lexical_braces_balanced=True,semantic_AST_oracle=False),ready=False,complete=None)

def transform_surface(method_source):
 """Return transformed method text, independent of declaring type/name/signature."""
 return calculate_surface(method_source)['after']

