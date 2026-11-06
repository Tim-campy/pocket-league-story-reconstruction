"""Audit every existing smcs pending body against complete-IR/source-only gates."""
import collections,json,sys,hashlib
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if(p/'tools/round7_slot_model.py').exists());sys.path[:0]=[str(ROOT/'tools'),str(Path(__file__).resolve().parent)]
from round9_control_ast import Client
from round7_slot_model import SlotModelClient
from round7_slot_solver import _edit
from round9_control_il import canonical_tree,canonical_plan
from round9_control_leaf_runs import bind_control_leaves
from round9_control_trace import project
from round9_control_rewrite import rewrite_control
WIP=ROOT/'build/round9/control-rewrite';BASE=ROOT/'build/round9/cycles/b10/after/Assembly-CSharp'
read=lambda p:json.loads(Path(p).read_text(encoding='utf-8-sig'))
def sha(path):return hashlib.sha256(Path(path).read_bytes()).hexdigest()
def export(out):
 dependencies=['tools/Round9ControlAst.cs','tools/round9_control_ast.py','tools/round9_control_trace.py','tools/round9_control_render_fragments.py','tools/round9_control_rewrite.py','tools/round9_control_il.py','tools/round9_control_cfg.py','tools/round9_control_leaf_runs.py','tools/round7_slot_model.py','tools/Round7SlotModel.cs','tools/round7_slot_solver.py','tools/compare_kairo.py','reports/round9-control-validation-public.json','reports/round9-control-leaf-bindings.json','reports/round9-control-unused-suffix-patches.json','tools/round9_unused_suffix_calculate.py','tools/test_round9_control_rewrite.py','reports/round9-control-rewrite-fixture.json','reports/round9-control-binding-suffix-fixture.json']
 all_hashes={name:sha(ROOT/name)for name in dependencies};hashes={name:value for name,value in all_hashes.items()if name.startswith('tools/')};evidence={name:value for name,value in all_hashes.items()if name.startswith('reports/')};out['dependency_sha256']=hashes;out['evidence_sha256']=evidence;out['calculator_sha256']=sha(Path(__file__));out['notes']=['Potential methods and IL bytes only; game oracle has not tested these candidates.','All 164 pending bodies audited; no CIL bytes copied into result.','Unused-suffix calculator is a separately frozen dependency.']
 selected=[]
 for patch in out['patches']:
  p=dict(patch);p['original_rule']=p['rule'];p['rule']='c9-control-skeleton-'+p['rule'];logical=p['logical_path'];p.update(calculator_sha256=out['calculator_sha256'],syntax_source_sha256=hashes['tools/Round9ControlAst.cs'],model_source_sha256=hashes['tools/Round7SlotModel.cs'],client_sha256=hashes['tools/round7_slot_model.py'],dependency_sha256=hashes,evidence_sha256=evidence,source_file_sha256=sha(BASE/logical),source_snapshot='build/round9/cycles/b10/after/Assembly-CSharp',rebase_policy='unique-exact-UTF16-before-only; fresh-NoEmit-model-required',own_compilation_receipt_sha256=sha(BASE/'verification.json'),own_dump_sha256=sha(BASE/'rebuilt-methods.json'),original_dump_sha256=sha(BASE/'original-methods.json'));selected.append(p)
 suffix=read(ROOT/'reports/round9-control-unused-suffix-patches.json')
 for patch in suffix['patches']:
  p=dict(patch);p['upstream_calculator_sha256']=suffix['calculator_sha256'];p['upstream_dependency_sha256']=suffix.get('dependency_sha256',{});p.update(calculator_sha256=out['calculator_sha256'],syntax_source_sha256=hashes['tools/Round9ControlAst.cs'],dependency_sha256=hashes,evidence_sha256=evidence,source_snapshot='build/round9/cycles/b10/after/Assembly-CSharp',source_file_sha256=sha(BASE/p['logical_path']),rebase_policy='unique-exact-UTF16-before-only; fresh-NoEmit-model-required',source_only=True,candidate_compilations=0);selected.append(p)
 for patch in selected:
  text=(BASE/patch['logical_path']).read_bytes().decode('utf-8-sig');patch.update(source_text_sha256=hashlib.sha256(text.encode('utf-8')).hexdigest(),own_body_source_bound=True,own_compilation_receipt_sha256=sha(BASE/'verification.json'),own_dump_sha256=sha(BASE/'rebuilt-methods.json'),original_dump_sha256=sha(BASE/'original-methods.json'))
 out['additional_calculated_suffix_proposals']=[{'method_id':p['method_id'],'rule':p['rule'],'source_only':True,'evidence_report':'reports/round9-control-binding-suffix-fixture.json'}for p in suffix['patches']]
 for row in out['methods']:
  extras=[p for p in suffix['patches']if p['method_id']==row['method_id']]
  if extras:row['additional_calculated_suffix_proposals']=len(extras);row['suffix_note']='Frozen independent suffix calculus, separate from generic Solver7 failure above; fresh model and game oracle still required.'
 out['combined_calculated_candidates']=len(selected);ids={p['method_id']for p in selected};originals={m['stable_id']:m for m in read(BASE/'original-methods.json')['selected_methods']};potential=sum(originals[key]['header']['code_size']for key in ids);bundle={'operation':'complete-control-IR-bound-existing-leaf-source-proposals','family':'c9-control-skeleton','source_only':True,'game_compilations':0,'adopted':False,'potential_methods':len(ids),'potential_original_IL_bytes':potential,'calculator_sha256':out['calculator_sha256'],'dependency_sha256':hashes,'evidence_sha256':evidence,'patches':selected}
 (ROOT/'reports/round9-control-rewrite-audit.json').write_text(json.dumps(out,indent=2)+'\n',encoding='utf-8');(ROOT/'reports/round9-control-rewrite-patches.json').write_text(json.dumps(bundle,indent=2)+'\n',encoding='utf-8');print(json.dumps({'audited':len(out['methods']),'control_proposals':out['methods_with_proposals'],'merged_methods':len(ids),'potential_IL_bytes':potential,'game_compilations':0},indent=2))

def main():
 WIP.mkdir(parents=True,exist_ok=True)
 validation=read(ROOT/'reports/round9-control-validation-public.json');details=read(ROOT/validation['method_irs']);binding_report=read(ROOT/'reports/round9-control-leaf-bindings.json');bindings={x['id']:x for x in binding_report['methods']};originals={m['stable_id']:m for m in read(BASE/'original-methods.json')['selected_methods']};own={m['stable_id']:m for m in read(BASE/'rebuilt-methods.json')['selected_methods']};types=read(BASE/'plain-local-types.json');own_types=read(BASE/'plain-local-types-rebuilt.json');manifest=read(BASE/'build-manifest.json')
 for item in manifest['sources']:item['input_path']=str(BASE/item['path'])
 mp=WIP/'rewrite-manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8');rows=[];patches=[]
 with Client(mp)as ast,SlotModelClient(mp)as slots:
  for m in validation['methods']:
   if m['state']=='matched':continue
   key=m['id'];ir=details[key];goal,source=ir['original_il_ir'],ir.get('source_ast_ir');row={'method_id':key,'state':m['state'],'original_il_bytes':m['original_il_bytes'],'patches':0,'game_compilations':0};rows.append(row)
   if not source:row['reasons']=['source-method-unmapped'];continue
   if not goal.get('tree_complete')or not goal.get('emission_plan_complete')or not source.get('tree_complete')or not source.get('emission_plan_complete'):row['reasons']=['original-or-source-IR-incomplete'];row['original_reasons']=goal.get('reasons');row['source_reasons']=source.get('reasons');continue
   logical=source['source'];text=(BASE/logical).read_bytes().decode('utf-8-sig');source_row=next(x for x in manifest['sources']if x['path']==logical);receipt=read(BASE/'verification.json');receipt_source=next(x for x in receipt['sources']if x['path']==logical);assert sha(BASE/logical)==source_row['sha256']==receipt_source['sha256'];same=canonical_tree(goal)==canonical_tree(source);row['skeletons_equal']=same
   if same:
    model=slots.predict(ROOT/logical,text,key)
    if model.get('complete')and model['predicted_types']==types[key]:row['reasons']=['source_skeleton_equal_and_slot_signature_already_exact'];continue
    calculated=slots.solve(ROOT/logical,text,key,originals[key],types[key],own[key]);row['calculated_slot_candidates']=calculated.get('calculated_candidates',0);row['solver_constraints']=calculated.get('constraints',[])
    for patch in calculated['patches']:
     candidate=_edit(text,[(patch['start'],patch['end'],patch['after'])]);fresh=ast.extract(ROOT/logical,candidate,key)
     if len(fresh)!=1 or not fresh[0].get('tree_complete')or not fresh[0].get('emission_plan_complete')or canonical_tree(fresh[0])!=canonical_tree(goal)or canonical_plan(fresh[0])!=canonical_plan(goal):continue
     pred=slots.predict(ROOT/logical,candidate,key)
     if not pred['complete']or pred['predicted_types']!=types[key]:continue
     patch.update(method_id=key,logical_path=logical,complete=True,predicted_types=pred['predicted_types'],source_only=True,candidate_compilations=0,rule='source_skeleton_equal_solver_only',slot_composition={'kind':'Solver7 using genuine original and own DTOs','candidate_DTO_fabricated':False});patches.append(patch);row['patches']+=1
    row['reasons']=[]if row['patches']else['source_skeleton_equal_solver_has_no_approved_control_preserving_candidate'];continue
   binding=bindings.get(key)
   if binding:binding=dict(binding,source_text_sha256=hashlib.sha256(text.encode('utf-8')).hexdigest(),own_body_source_bound=True,own_body_receipt_sha256=sha(BASE/'verification.json'))
   if not binding or not binding['complete']:row['reasons']=['leaf-binding-not-complete'];row['binding_reasons']=binding.get('reasons',[])if binding else['no-binding-record'];continue
   result=rewrite_control(goal,source,text,binding,source_path=ROOT/logical,method_id=key,expected_types=types[key],slot_model_client=slots,ast_client=ast,original_method=originals[key],compiled_method=own[key],compiled_types=own_types[key]);row['reasons']=result['reasons'];row['rewrite_audit']=result['audit'];row['patches']=len(result['patches'])
   for patch in result['patches']:patch['logical_path']=logical;patches.append(patch)
   print(key,'patches',row['patches'],'reasons',row['reasons'],flush=True)
 counts=collections.Counter(reason for r in rows for reason in r.get('reasons',[]));out={'operation':'all-pending-control-reconstruction-source-only-audit','snapshot':str(BASE.parent.relative_to(ROOT)),'pending_bodies':len(rows),'source_only':True,'game_compilations':0,'new_hypotheses':False,'methods':rows,'patches':patches,'methods_with_proposals':sum(r['patches']>0 for r in rows),'proposal_variants':len(patches),'groups':dict(counts),'validation_sha256':sha(ROOT/'reports/round9-control-validation-public.json'),'bindings_sha256':sha(ROOT/'reports/round9-control-leaf-bindings.json'),'AST_helper_sha256':sha(ROOT/'tools/Round9ControlAst.cs'),'slot_model_sha256':sha(ROOT/'tools/Round7SlotModel.cs'),'slot_client_sha256':sha(ROOT/'tools/round7_slot_model.py')}
 export(out)

if __name__=='__main__':main()
