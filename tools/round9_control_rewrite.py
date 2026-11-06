"""Conservative source renderer from complete original control IR and bound own-source leaves.

No IL is emitted, patched, copied into C#, or fabricated for a candidate.
"""
import copy,json,re,sys,hashlib
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if(p/'tools/round7_slot_model.py').exists());sys.path.insert(0,str(ROOT/'tools'))
from round9_control_il import canonical_tree,canonical_plan
from round7_slot_solver import _edit,_patch,correspondences,_group_candidates
from round9_control_trace import project,leaves,stripped
from round9_control_render_fragments import source_fragments

class Refused(ValueError):pass

def _tests(tree):
 result=[]
 if tree['kind']=='test':result.append(tree)
 for key,value in tree.items():
  if key.startswith('_'):continue
  if isinstance(value,dict)and'kind'in value:result.extend(_tests(value))
  elif isinstance(value,list):
   for item in value:
    if isinstance(item,dict)and'kind'in item:result.extend(_tests(item))
 return result

def _opposite(opcode,floating=False):
 short=opcode.endswith('.s');op=opcode[:-2]if short else opcode
 table={'brtrue':'brfalse','brfalse':'brtrue','beq':'bne.un','bne.un':'beq','blt':'bge','bge':'blt','bgt':'ble','ble':'bgt','blt.un':'bge.un','bge.un':'blt.un','bgt.un':'ble.un','ble.un':'bgt.un'}
 if floating:table.update(blt='bge.un',bge='blt.un',bgt='ble.un',ble='bgt.un',**{'blt.un':'bge','bge.un':'blt','bgt.un':'ble','ble.un':'bgt'})
 value=table.get(op)
 return value+('.s'if short else'')if value else None

def _floating(node):return node.get('data',{}).get('floating',False)or any(_floating(c)for c in node.get('children',[]))

class Renderer:
 def __init__(self,original,source_ir,source,bindings):
  self.target=project(original,canonical_tree);self.source_tree=project(source_ir,canonical_tree);self.source=source;self.by_source_path=dict(leaves(self.source_tree));self.by_goal_path={tuple(row['original_path']):row for row in bindings['bindings']};self.used=set();self.return_void=source_ir['method_id'].endswith('->System.Void');self.audit=[]
 def fragments(self,path):
  row=self.by_goal_path.get(path)
  if row is None:raise Refused('original-leaf-path-is-unbound:'+repr(path))
  source_path=tuple(row['source_path'])
  if source_path not in self.by_source_path:raise Refused('source-leaf-path-is-unmapped')
  self.used.add(path)
  try:return source_fragments(self.by_source_path[source_path],self.source)
  except ValueError as e:raise Refused(str(e))from e
 def condition(self,tree,path,on_true):
  leafs=leaves(tree,path);tests=_tests(tree)
  if len(leafs)!=1 or len(tests)!=1:raise Refused('condition-is-not-one-bound-value-run-and-one-test')
  frags=self.fragments(leafs[0][0])
  if len(frags)!=1 or frags[0]['kind']!='condition':raise Refused('condition-leaf-is-not-one-existing-condition-expression')
  fragment=frags[0];node=fragment['condition_node'];source_test=_tests(canonical_tree({'tree_complete':True,'tree':node}))
  if len(source_test)!=1:raise Refused('source-condition-predicate-is-ambiguous')
  source_opcode=source_test[0]['opcode'];source_sense=node['data'].get('branch_on_true');target_opcode=tests[0]['opcode'];floating=_floating(node)
  at_target=source_opcode if source_sense==on_true else _opposite(source_opcode,floating)
  if at_target==target_opcode:result=fragment['text'];orientation='unchanged-expression'
  elif _opposite(at_target,floating)==target_opcode:result='!('+fragment['text']+')';orientation='explicit-logical-negation'
  else:raise Refused('target-test-has-no-proved-existing-expression-polarity:'+str((source_opcode,source_sense,target_opcode,on_true)))
  self.audit.append({'condition_source_span':[fragment['start'],fragment['end']],'source_opcode':source_opcode,'target_opcode':target_opcode,'floating':floating,'orientation':orientation})
  return result
 def value(self,tree,path):
  units=self.sequence_units(tree,path)
  if len(units)!=1 or units[0][0]not in('value','return_value'):raise Refused('value-region-is-not-one-existing-expression')
  return units[0][1]
 def sequence_units(self,tree,path):
  kind=tree['kind']
  if kind=='sequence':
   units=[]
   for index,child in enumerate(tree['children']):
    child_path=path+('children',index)
    if child['kind']=='return':
     if child.get('opcode')!='ret':raise Refused('non-ret-return-region')
     if units and units[-1][0]in('value','return_value'):expr=units.pop()[1];units.append(('statement','return '+expr+';'))
     elif self.return_void:units.append(('statement','return;'))
     else:raise Refused('nonvoid-return-has-no-bound-value')
    else:units.extend(self.sequence_units(child,child_path))
   return units
  if kind=='leaf':return[(f['kind'],f['text'])for f in self.fragments(path)]
  if kind=='if':
   condition=self.condition(tree['condition'],path+('condition',),False);then=self.statements(tree['then'],path+('then',));code='if ('+condition+') {\n'+then+'\n}'
   if tree.get('else')is not None:code+=' else {\n'+self.statements(tree['else'],path+('else',))+'\n}'
   return[('statement',code)]
  if kind=='ternary':
   if tree.get('join_opcode')!='br' or tree.get('else')is None:raise Refused('ternary-join-not-supported')
   condition=self.condition(tree['condition'],path+('condition',),False);left=self.value(tree['then'],path+('then',));right=self.value(tree['else'],path+('else',));return[('value','('+condition+') ? ('+left+') : ('+right+')')]
  if kind=='logical-value':
   short=self.value(tree['short_result'],path+('short_result',))
   if short not in('true','false')or tree.get('join_opcode')!='br.s':raise Refused('logical-value-short-result-not-source-proven')
   is_or=short=='true';condition=self.condition(tree['condition'],path+('condition',),is_or);right=self.value(tree['right'],path+('right',));return[('value','('+condition+') '+('||'if is_or else'&&')+' ('+right+')')]
  if kind=='loop':
   form=tree['emission_form'];body=self.statements(tree['body'],path+('body',))
   if form=='pretested'and tree.get('entry_opcode')=='br'and tree.get('tail_opcode')is None:condition=self.condition(tree['condition'],path+('condition',),True);code='while ('+condition+') {\n'+body+'\n}'
   elif form=='posttested'and tree.get('entry_opcode')is None and tree.get('tail_opcode')is None:condition=self.condition(tree['condition'],path+('condition',),True);code='do {\n'+body+'\n} while ('+condition+');'
   elif form=='infinite'and tree.get('tail_opcode')=='br'and not _tests(tree['condition']):code=('for (;;)'if tree.get('entry_opcode')=='br'else'while (true)')+' {\n'+body+'\n}'
   else:raise Refused('loop-emission-template-not-supported')
   return[('statement',code)]
  if kind in('break','continue'):
   if tree.get('opcode')!='br'or tree.get('target_depth')!=0:raise Refused('nonlocal-or-protected-loop-transfer-not-renderable')
   return[('statement',kind+';')]
  if kind=='throw':raise Refused('throw-expression-value-ownership-not-supported')
  raise Refused('control-region-not-renderable:'+kind)
 def statements(self,tree,path=()):
  units=self.sequence_units(tree,path)
  if any(kind!='statement'for kind,text in units):raise Refused('unconsumed-expression-or-condition-in-statement-region')
  return'\n'.join(text for kind,text in units)
 def render(self):
  code=self.statements(self.target)
  if len(self.used)!=len(self.by_goal_path):raise Refused('not-all-bound-original-leaves-consumed')
  return'{\n'+code+'\n}'

def calculate_named_slot_variants(source,old_source,old_model,new_model,original,compiled,expected_types,compiled_types):
 """Reuse Solver7 declaration calculus with named provenance, never fake candidate IL."""
 slots,evidence=correspondences(original,compiled,expected_types,compiled_types)
 old_by={};new_by={}
 for v in old_model.get('variables',[]):old_by.setdefault((v['name'],v['type']),[]).append(v)
 for v in new_model.get('variables',[]):new_by.setdefault((v['name'],v['type']),[]).append(v)
 current={};names=[]
 for key,values in new_by.items():
  before=old_by.get(key,[])
  if len(values)==len(before)==1 and before[0]['slot']in slots:
   current[values[0]['slot']]=slots[before[0]['slot']];names.append({'name':key[0],'type':key[1],'real_own_slot':before[0]['slot'],'new_source_predicted_slot':values[0]['slot'],'desired_original_slot':slots[before[0]['slot']]})
 declared=[v for v in new_model.get('variables',[])if v['kind']=='declared'and v.get('statement_start',-1)>=0];scopes={}
 for variable in declared:
  if variable['slot']in current:scopes.setdefault(variable['lexical_scope_start'],[]).append(variable)
 patches=[]
 for scope,variables in scopes.items():
  ordered=sorted(variables,key=lambda v:current[v['slot']]);runs=[];run=[]
  for variable in ordered:
   if run and current[variable['slot']]!=current[run[-1]['slot']]+1:
    if len(run)>1:runs.append(run)
    run=[]
   run.append(variable)
  if len(run)>1:runs.append(run)
  for run in runs:
   edits,proposals=_group_candidates(source,new_model,original,expected_types,current,declared,run);patches.extend(proposals)
 return{'patches':patches,'named_slot_map':names,'real_DTO_correspondence':evidence,'candidate_IL_fabricated':False,'calculus':'frozen Solver7._group_candidates with unique name/type provenance from real own DTO'}

def rewrite_control(original_ir,source_ir,source_text,leaf_bindings,*,source_path,method_id,expected_types,slot_model_client,ast_client,original_method,compiled_method,compiled_types):
 result={'eligible':False,'reasons':[],'patches':[],'audit':{},'candidate_game_compilations':0}
 if leaf_bindings.get('source_text_sha256')!=hashlib.sha256(source_text.encode('utf-8')).hexdigest()or not leaf_bindings.get('own_body_source_bound')or not re.fullmatch(r'[0-9a-f]{64}',leaf_bindings.get('own_compilation_receipt_sha256',leaf_bindings.get('own_body_receipt_sha256',''))):
  result['reasons'].append('source-text-is-not-authenticated-by-own-build-receipt');return result
 if source_ir.get('method_id')!=method_id or original_ir.get('method_id')!=method_id:
  result['reasons'].append('control-method-identities-differ');return result
 if not all(ir.get('tree_complete')and ir.get('emission_plan_complete')for ir in(original_ir,source_ir)):
  result['reasons'].append('original-or-source-control-IR-incomplete');return result
 if not leaf_bindings.get('complete'):
  result['reasons'].append('nonbijective-or-unproved-value-leaf-bindings');return result
 old_model=slot_model_client.predict(source_path,source_text,method_id)
 silent=[v for v in old_model.get('variables',[])if v.get('kind')=='declared'and v.get('initializer')is None and v.get('statement_start',-1)>=0]
 if silent:
  result['reasons'].append('zero-IL-local-declarations-have-no-proved-render-anchor');result['audit']['silent_declarations']=[{'name':v['name'],'type':v['type'],'slot':v['slot'],'statement_start':v['statement_start'],'lexical_scope_start':v['lexical_scope_start']}for v in silent];return result
 try:
  renderer=Renderer(original_ir,source_ir,source_text,leaf_bindings);body=renderer.render();coverage=source_ir['coverage'];start,end=coverage['source_start'],coverage['source_end'];nl='\r\n'if'\r\n'in source_text else'\n';body=body.replace('\n',nl);candidate=_edit(source_text,[(start,end,body)]);result['audit']['render']=renderer.audit
 except(Refused,ValueError,KeyError)as error:result['reasons'].append(str(error));return result
 if candidate==source_text:result['reasons'].append('rendered-source-is-unchanged');return result
 new_model=slot_model_client.predict(source_path,candidate,method_id);variants=[('control-only',candidate,{'kind':'no-slot-edits'})]
 if new_model.get('complete')and new_model.get('predicted_types')!=expected_types:
  calculated=calculate_named_slot_variants(candidate,source_text,old_model,new_model,original_method,compiled_method,expected_types,compiled_types);result['audit']['slot_composition']=calculated;variants=[]
  for p in calculated['patches']:variants.append((p['rule'],_edit(candidate,[(p['start'],p['end'],p['after'])]),{'kind':'control-plus-calculated-Solver7-declarations','named_slot_map':calculated['named_slot_map']}))
  if not variants:result['reasons'].append('complete-wrong-slot-signature-and-no-calculated-declaration-variant')
 result['audit']['variant_checks']=[]
 for label,text,composition in variants[:24]:
  check={'variant':label};result['audit']['variant_checks'].append(check)
  prediction=slot_model_client.predict(source_path,text,method_id)
  check.update(slot_model_complete=prediction.get('complete'),slot_types_exact=prediction.get('predicted_types')==expected_types,slot_count=len(prediction.get('predicted_types',[])))
  if prediction.get('complete')and prediction.get('predicted_types')!=expected_types:check['reason']='complete-wrong-slot-signature';continue
  extracted=ast_client.extract(source_path,text,method_id)
  if len(extracted)!=1:check['reason']='candidate-control-method-unmapped';continue
  ir=extracted[0]
  check.update(tree_complete=ir.get('tree_complete'),plan_complete=ir.get('emission_plan_complete'),control_reasons=ir.get('reasons',[]))
  if not ir.get('tree_complete')or not ir.get('emission_plan_complete'):check['reason']='candidate-control-IR-incomplete';continue
  check.update(tree_equals_original=canonical_tree(ir)==canonical_tree(original_ir),plan_equals_original=canonical_plan(ir)==canonical_plan(original_ir))
  if not check['tree_equals_original']or not check['plan_equals_original']:check['reason']='candidate-control-differs-original';continue
  patch=_patch(source_text,old_model,text,'control-tree-reconstruction');patch.update(method_id=method_id,logical_path=str(source_path),complete=prediction.get('complete',False),predicted_types=prediction.get('predicted_types',[]),slot_composition=composition,source_only=True,candidate_compilations=0,control_IR_complete=True,control_plan_equals_original=True,control_tree_equals_original=True,variant=label)
  result['patches'].append(patch)
 result['eligible']=bool(result['patches'])
 if not result['patches']and not result['reasons']:result['reasons'].append('rendered-variants-fail-independent-control-or-slot-gate')
 return result
