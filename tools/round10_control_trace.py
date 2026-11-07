"""Tree projection with provenance. Never constructs source structure from IL/plan."""
import copy

def sequence(children):
 flat=[]
 for child in children:
  if child['kind']=='sequence':flat.extend(child['children'])
  else:flat.append(child)
 out=[]
 for child in flat:
  if child['kind']=='leaf'and out and out[-1]['kind']=='leaf':out[-1]['_origins'].extend(child.get('_origins',[]));continue
  out.append(child)
 return {'kind':'sequence','children':out}
def stripped(value):
 if isinstance(value,dict):return {k:stripped(v)for k,v in value.items()if not k.startswith('_')}
 if isinstance(value,list):return [stripped(v)for v in value]
 return value

def project(ir,canonical_tree):
 if not ir.get('tree_complete',ir.get('complete',False)):return None
 def visit(node,ancestors):
  kind=node['kind'];data=node.get('data',{});lineage=ancestors+[node];children=[visit(c,lineage)for c in node.get('children',[])]
  if any(c is None for c in children):return None
  origin={'node':node,'ancestors':ancestors}
  if kind=='sequence':return sequence(children)
  if kind=='leaf':return {'kind':'leaf','_origins':[origin]}
  if kind in ('return','throw'):return sequence(children+[{'kind':kind,'opcode':data.get('opcode','ret'if kind=='return'else'throw'),'_origin':origin}])
  if kind in ('break','continue'):
   transfer=canonical_tree({'tree_complete':True,'tree':node})
   if transfer is None:return None
   return dict(transfer,_origin=origin)
  if kind=='logical':
   if data.get('condition_chain') or data.get('r10_common_loop_target_chain'):return sequence(children)
   if(data.get('value_mode')or(data.get('operator')in('&&','||')and'branch_on_true'not in data))and len(children)==3:return {'kind':'logical-value','condition':sequence([children[0]]),'right':sequence([children[1]]),'short_result':sequence([children[2]]),'join_opcode':data.get('join_opcode','br.s'),'_origin':origin}
   if data.get('opcode'):return sequence(children+[{'kind':'test','opcode':data['opcode'],'_origin':origin}])
   if len(children)==1:return children[0]
   return None
  if kind=='switch':
   if 'dense_table' not in data:return None
   return {'kind':'switch','dispatch':{'normalization_opcode':data.get('dense_normalization_opcode'),'normalization_value':data.get('dense_normalization_value',0),'table':data['dense_table'],'default_branch_opcode':data.get('dense_default_branch')},'sections':[{'keys':raw.get('data',{}).get('case_keys',[]),'body':sequence([value]),'_origin':{'node':raw,'ancestors':lineage}} for raw,value in zip(node.get('children',[]),children)],'_origin':origin}
  if kind=='try':
   if 'eh_kind' not in data:return None
   return {'kind':'try','handler_kind':data['eh_kind'],'catch_types':data.get('eh_catch_types',[]),'try_epilogue_opcode':data.get('eh_try_epilogue'),'handler_epilogue_opcodes':data.get('eh_handler_epilogues',[]),'body':sequence([children[0]]),'handlers':[sequence([c]) for c in children[1:]],'_origin':origin}
  if kind in ('if','ternary'):
   if data.get('constant_elided'):return sequence(children)
   if len(children)not in(2,3):return None
   join=data.get('join_opcode','br'if kind=='ternary'else None)
   if'join_opcode'not in data and data.get('has_else')and data.get('then_can_fallthrough'):join='br'
   condition=sequence([children[0]]);then=sequence([children[1]]);otherwise=sequence([children[2]])if len(children)==3 else None
   result={'kind':kind,'condition':condition,'then':then,'else':otherwise,'join_opcode':join,'_origin':origin}
   if kind=='if'and otherwise is not None and data.get('then_can_fallthrough')is False and join is None:result['else']=None;return sequence([result,otherwise])
   return result
  if kind=='loop':
   if not data.get('emission_form')or len(children)!=2:return None
   def common(n):return n.get('data',{}).get('condition_chain') or n.get('data',{}).get('r10_common_loop_target_chain') or any(common(c) for c in n.get('children',[]))
   condition={'kind':'test-chain','taken_target':'loop-body','predicates':children[0]} if ir.get('control_stage',3)>=3 and common(node['children'][0]) else children[0]
   return {'kind':'loop','emission_form':data['emission_form'],'entry_opcode':data.get('entry_opcode'),'condition':condition,'body':children[1],'tail_opcode':data.get('tail_opcode'),'_origin':origin}
  return None
 result=visit(ir['tree'],[])
 expected=canonical_tree(ir)
 if result is None or stripped(result)!=expected:raise ValueError('provenance-projection-disagrees-with-shared-canonical-tree')
 return result

def leaves(tree,path=()):
 if not isinstance(tree,dict):return []
 if tree.get('kind')=='leaf':return [(path,tree)]
 out=[]
 for key,value in tree.items():
  if key.startswith('_'):continue
  if isinstance(value,dict):out.extend(leaves(value,path+(key,)))
  elif isinstance(value,list):
   for i,item in enumerate(value):
    if isinstance(item,dict):out.extend(leaves(item,path+(key,i)))
 return out

def source_slice(source,start,end):return source.encode('utf-16-le')[start*2:end*2].decode('utf-16-le')

def source_fragments(projected_leaf,source):
 """Use only source-tree lineage/span to identify whole existing fragments."""
 fragments=[]
 for origin in projected_leaf.get('_origins',[]):
  node=origin['node'];ancestors=origin['ancestors'];lineage=ancestors+[node];logical=[n for n in ancestors if n['kind']=='logical'and'branch_on_true'in n.get('data',{})]
  if logical:
   owner=logical[0];span=owner['span'];fragment={'kind':'condition','start':span['start'],'end':span['end'],'text':source_slice(source,span['start'],span['end']),'condition_node':owner}
  elif node.get('data',{}).get('role')=='logical-constant-result':
   owner=next((n for n in reversed(ancestors)if n['kind']=='logical'and n.get('data',{}).get('value_mode')),None)
   if owner is None:raise ValueError('logical-constant-origin-missing')
   fragment={'kind':'value','start':node['span']['start'],'end':node['span']['end'],'text':'true'if owner['data'].get('operator')=='||'else'false','derived_literal_from_source_operator':True}
  else:
   # An arm of a value expression has priority over its enclosing return.
   arm=None
   for parent,child in zip(lineage,lineage[1:]):
    if parent['kind']=='ternary'and child in parent.get('children',[])[1:]:arm=child
    if parent['kind']=='logical'and parent.get('data',{}).get('value_mode')and child in parent.get('children',[])[1:]:arm=child
   if arm is not None:
    span=arm['span'];fragment={'kind':'value','start':span['start'],'end':span['end'],'text':source_slice(source,span['start'],span['end'])}
   else:
    ret=next((n for n in reversed(ancestors)if n['kind']=='return'and not n.get('data',{}).get('implicit')),None)
    if ret is not None:
     span=ret['span'];text=source_slice(source,span['start'],span['end']).strip()
     if not text.startswith('return')or not text.endswith(';'):raise ValueError('return-source-span-is-not-a-return-statement')
     fragment={'kind':'return_value','start':span['start'],'end':span['end'],'text':text[6:-1].strip()}
    else:
     command=None
     for candidate in reversed(lineage):
      if candidate['kind']!='sequence':continue
      span=candidate['span'];text=source_slice(source,span['start'],span['end']).strip()
      if text.endswith(';')and not text.startswith('{')and not candidate.get('data',{}).get('role'):
       def plain(n):return n['kind']in('sequence','leaf')and all(plain(c)for c in n.get('children',[]))
       if plain(candidate):command=(candidate,text);break
     if command is None:
      text=source_slice(source,node['span']['start'],node['span']['end']).strip()
      import re
      if re.fullmatch(r'(?:\+\+|--)?[A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*(?:\+\+|--)',text):fragment={'kind':'statement','start':node['span']['start'],'end':node['span']['end'],'text':text+';','for_iterator_as_statement':True}
      else:raise ValueError('source-fragment-not-whole-command-or-bound-value:'+node['id'])
     else:
      owner,text=command;span=owner['span'];fragment={'kind':'statement','start':span['start'],'end':span['end'],'text':text}
  key=(fragment['kind'],fragment['start'],fragment['end'],fragment['text'])
  if not any((f['kind'],f['start'],f['end'],f['text'])==key for f in fragments):fragments.append(fragment)
 return sorted(fragments,key=lambda f:(f['start'],f['end']))
