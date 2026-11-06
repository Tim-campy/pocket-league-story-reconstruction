"""NoEmit joint splitting of recycled switch selectors into calculated named aliases."""
import json,re,sys,hashlib
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if(p/'tools/round7_slot_model.py').exists());sys.path.insert(0,str(ROOT/'tools'))
from round7_slot_solver import local_access,_edit,_patch
from round7_slot_model import SlotModelClient
from round9_control_ast import Client
from round9_control_il import _effect,OPCODES

def walk(n):
 yield n
 for c in n.get('children',[]):yield from walk(c)
def source_slice(source,span):return source.encode('utf-16-le')[span['start']*2:span['end']*2].decode('utf-16-le')
def int_constant(v):
 op=v['opcode']
 if op=='ldc.i4.m1':return -1
 if op in ('ldc.i4','ldc.i4.s'):return v['operand']
 if re.fullmatch(r'ldc.i4.[0-8]',op):return int(op[-1])
 return None

def expression_before_store(instructions,store_index):
 # Backward stack dependency, rejecting control/dup rather than taking a fixed
 # textual window containing a preceding unrelated assignment.
 needed=1
 for i in range(store_index-1,-1,-1):
  ins=instructions[i];effect=_effect(ins,None)
  if effect is None or ins['opcode']=='dup'or OPCODES.get(ins['opcode'],{}).get('flow')in ('Branch','Cond_Branch','Return','Throw')or ins['opcode'].startswith(('stloc','stfld','stsfld')):return None
  pop,push,_=effect
  if push>needed:return None
  needed+=pop-push
  if needed==0:return instructions[i:store_index]
 return None

def original_aliases(method):
 instr=method['instructions'];out=[]
 for slot in range(len(method['locals'])):
  hits=[i for i,v in enumerate(instr)if local_access(v)and local_access(v)[1]==slot];stores=[i for i in hits if instr[i]['opcode'].startswith('stloc')]
  if len(stores)!=1:continue
  labels=[];ok=True
  for i in hits:
   if i==stores[0]:continue
   if not instr[i]['opcode'].startswith('ldloc')or i+1>=len(instr):ok=False;break
   if instr[i+1]['opcode']=='brtrue':labels.append(0)
   elif i+2<len(instr)and int_constant(instr[i+1])is not None and instr[i+2]['opcode']=='bne.un':labels.append(int_constant(instr[i+1]))
   else:ok=False;break
  if ok and len(labels)>=2:out.append({'slot':slot,'labels':labels,'store_offset':instr[stores[0]]['offset'],'first_use':instr[hits[0]]['offset'],'last_use':instr[hits[-1]]['offset'],'store_expression_instructions':expression_before_store(instr,stores[0]),'each_use_is_dispatch_test':True})
 return out

def calculate(source_ir,source,original,model,expected_types,compiled_method):
 switches=[n for n in walk(source_ir['tree'])if n['kind']=='switch'];available=original_aliases(original);edits=[];proof=[]
 if any(v['opcode']=='switch'for v in original['instructions']):return {'eligible':False,'reason':'original-switch-opcode-present'}
 if len(switches)<2:return {'eligible':False,'reason':'not-a-multiple-selector-composition'}
 temporary_slots={v['slot']for v in model.get('variables',[])if v.get('kind')=='emit-temporary'and v['name']=='$switch'and v['type']=='System.Int32'}
 own_instructions=compiled_method['instructions'];own_stores=[i for i,v in enumerate(own_instructions)if local_access(v)and local_access(v)[1]in temporary_slots and v['opcode'].startswith('stloc')]
 if len(own_stores)!=len(switches):return {'eligible':False,'reason':'actual-own-switch-store-count-not-bijective'}

 assigned=set()
 for ordinal,node in enumerate(switches):
  if node['data']['selector_type']!='System.Int32':return {'eligible':False,'reason':'selector-not-int32'}
  selector=node['data']['selector'];sections=node['children'][1:];labels=[];parts=[];name='recoveredSelector'+str(ordinal+1)
  if re.search(r'\b'+name+r'\b',source):return {'eligible':False,'reason':'synthetic-name-collision'}
  for section in sections:
   if not section['children']or section['children'][-1]['kind']!='break':return {'eligible':False,'reason':'nonterminal-switch-break'}
   statements=section['children'][:-1]
   if any(n['kind']in ('break','continue','switch')for st in statements for n in walk(st)):return {'eligible':False,'reason':'nested-transfer-or-switch-ownership'}
   case_labels=section['data']['case_labels']
   if len(case_labels)!=1:return {'eligible':False,'reason':'multiple-or-default-label'}
   match=re.fullmatch(r'case (-?\d+):',case_labels[0])
   if not match:return {'eligible':False,'reason':'nonliteral-case-label'}
   label=int(match[1]);labels.append(label);parts.append('if ('+name+' == '+str(label)+') {\r\n'+'\r\n'.join(source_slice(source,st['span'])for st in statements)+'\r\n}')
  matches=[a for a in available if a['labels']==labels and a['slot']not in assigned]
  if len(matches)!=1:return {'eligible':False,'reason':'original-alias-dispatch-not-unique','labels':labels,'matches':len(matches)}
  alias=matches[0]
  if expected_types[alias['slot']]!='System.Int32':return {'eligible':False,'reason':'original-selector-local-is-not-int32'}
  assigned.add(alias['slot']);own_store=own_stores[ordinal];own_expression=expression_before_store(own_instructions,own_store);fingerprint=lambda xs:[(v['opcode'],v['operand'])for v in xs]
  if own_expression is None or alias['store_expression_instructions']is None or fingerprint(own_expression)!=fingerprint(alias['store_expression_instructions']):return {'eligible':False,'reason':'original-own-selector-expression-anchor-differs'}
  alias=dict(alias,own_store_offset=own_instructions[own_store]['offset'],own_switch_reused_slot=local_access(own_instructions[own_store])[1],original_own_expression_anchor_exact=True,own_expression_instructions=own_expression);replacement='int '+name+' = '+selector+';\r\n'+' else '.join(parts);edits.append((node['span']['start'],node['span']['end'],replacement));proof.append(dict(alias,name=name,selector=selector,source_span=node['span'],terminal_breaks_removed=len(sections),selector_evaluations=1))
 candidate=_edit(source,edits);patch=_patch(source,model,candidate,'calculated-multi-switch-selector-splitting')
 return {'eligible':True,'patch':patch,'proofs':proof,'candidate_source':candidate,'original_switch_opcode_count':0,'game_compilations':0,'body_oracle':'UNKNOWN'}

def main():
 import argparse
 from round7_coalesce_calculate import rebase_unique_patch
 parser=argparse.ArgumentParser();parser.add_argument('--snapshot',default='build/round9/cycles/c11/after');args=parser.parse_args();base=ROOT/args.snapshot/'Assembly-CSharp';work=ROOT/'build/round9/multi-selector';work.mkdir(parents=True,exist_ok=True);read=lambda p:json.loads(p.read_text(encoding='utf-8-sig'));sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest();manifest=read(base/'build-manifest.json');receipt=read(base/'verification.json')
 for entry in manifest['sources']:entry['input_path']=str(base/entry['path'])
 mp=work/'manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8');key='form.SubForm::DrawMenu(kairo.unity.ui.Graphics,System.Int32)->System.Void';logical='src/Assembly-CSharp/form/SubForm.cs';text=(base/logical).read_bytes().decode('utf-8-sig');expected=read(base/'plain-local-types.json')[key];original=next(v for v in read(base/'original-methods.json')['selected_methods']if v['stable_id']==key);own=next(v for v in read(base/'rebuilt-methods.json')['selected_methods']if v['stable_id']==key);file_sha=sha(base/logical);assert file_sha==next(v['sha256']for v in receipt['sources']if v['path']==logical)==next(v['sha256']for v in manifest['sources']if v['path']==logical)
 with Client(mp)as ast,SlotModelClient(mp)as slots:
  source_ir=ast.extract(ROOT/logical,text,key)[0];source_model=slots.predict(ROOT/logical,text,key);calculated=calculate(source_ir,text,original,source_model,expected,own);assert calculated['eligible'],calculated;prediction=slots.predict(ROOT/logical,calculated.pop('candidate_source'),key);assert prediction['complete']and prediction['predicted_types']==expected
  patch=calculated['patch'];live_text=(ROOT/logical).read_bytes().decode('utf-8-sig');rebased=rebase_unique_patch(live_text,patch);live_candidate=_edit(live_text,[(rebased['start'],rebased['end'],rebased['after'])]);live_model=slots.predict(ROOT/logical,live_candidate,key);assert live_model['complete']and live_model['predicted_types']==expected
 names=['tools/round9_multi_selector_calculate.py','tools/test_round9_multi_selector.py','tools/Round7SlotModel.cs','tools/round7_slot_model.py','tools/round7_slot_solver.py','tools/round7_coalesce_calculate.py','tools/round9_control_ast.py','tools/Round9ControlAst.cs','tools/round9_control_il.py','tools/round9_control_cfg.py'];dependencies={name:sha(ROOT/name)for name in names};evidence={name:sha(ROOT/name)for name in ['reports/round9-multi-selector-fixture.json','reports/round9-multi-selector-diagnosis.json']};family='d9-calculated-multiple-selector-local-splitting';patch.update(rule=family,method_id=key,logical_path=logical,model_source_sha256=dependencies['tools/Round7SlotModel.cs'],client_sha256=dependencies['tools/round7_slot_model.py'],calculator_sha256=sha(Path(__file__)),syntax_source_sha256=dependencies['tools/Round9ControlAst.cs'],dependency_sha256=dependencies,evidence_sha256=evidence,complete=True,predicted_types=prediction['predicted_types'],source_only=True,candidate_compilations=0,source_file_sha256=file_sha,source_text_sha256=hashlib.sha256(text.encode('utf-8')).hexdigest(),own_body_source_bound=True,own_compilation_receipt_sha256=sha(base/'verification.json'),original_dump_sha256=sha(base/'original-methods.json'),own_dump_sha256=sha(base/'rebuilt-methods.json'),snapshot_sha256=sha(base.parent/'snapshot.json'),source_snapshot=str(base.relative_to(ROOT)).replace('\\','/'),rebase_policy='unique exact UTF16 declaration; fresh complete exact NoEmit model required')
 out={'operation':'joint-selector-local-splitting-source-only-proposal','family':family,'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','potential_methods':1,'potential_original_IL_bytes':original['header']['code_size'],'calculator_sha256':sha(Path(__file__)),'dependency_sha256':dependencies,'evidence_sha256':evidence,'snapshot':str(base.parent.relative_to(ROOT)).replace('\\','/'),'snapshot_sha256':sha(base.parent/'snapshot.json'),'patches':[patch],'proofs':calculated['proofs'],'fresh_current_unique_rebase':True,'fresh_current_model_complete':True,'fresh_current_model_types_exact':True};(ROOT/'reports/round9-multi-selector-patches.json').write_text(json.dumps(out,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in out.items()if k not in ['patches','proofs','dependency_sha256','evidence_sha256']},indent=2))
if __name__=='__main__':main()
