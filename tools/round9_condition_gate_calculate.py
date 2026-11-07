"""Fresh source-only binding and models for billing and collision condition gates."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round7_slot_model import SlotModelClient
from round9_condition_gate_source import extract,transform_billing,transform_collision,inventory
FAMILY='d9-calculated-condition-gate-compositions'
def main():
 work=ROOT/'build/round9/condition-gate-calculated';work.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle29/after/Assembly-CSharp';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for row in manifest['sources']:row['path']=str(base/row['path']);row['input_path']=row['path']
 mp=work/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8');expected_all=json.loads((base/'plain-local-types.json').read_text(encoding='utf-8-sig'));original=json.loads((base/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']
 deps={p:sha(ROOT/p)for p in ('tools/round9_condition_gate_source.py','tools/round9_condition_gate_fixtures.py','tools/round9_condition_gate_calculate.py','tools/Round7SlotModel.cs','tools/round7_slot_model.py','tools/fixtures/round9-condition-gates/billing-fixture-template.cs.txt','tools/fixtures/round9-condition-gates/billing-manual-control-template.cs.txt','tools/fixtures/round9-condition-gates/collision-fixture-template.cs.txt','tools/fixtures/round9-condition-gates/collision-manual-control-template.cs.txt')};evidence={'reports/round9-condition-gate-fixtures.json':sha(ROOT/'reports/round9-condition-gate-fixtures.json')};patches=[];proofs=[];potential=0
 targets=[('system.form.BillingForm::_update()->System.Boolean','src/Assembly-CSharp/system/form/BillingForm.cs','private bool _update()',transform_billing),('game.GameManager::Collision_proc(game.Player,game.Player)->System.Void','src/Assembly-CSharp/game/GameManager.cs','public void Collision_proc(Player p1, Player p2)',transform_collision)]
 with SlotModelClient(mp)as model:
  for key,logical,header,transform in targets:
   path=base/logical;assert sha(path)==sha(ROOT/logical);source=path.read_bytes().decode('utf-8-sig');start,end,before=extract(source,header);after=transform(before);candidate=source[:start]+after+source[end:];expected=expected_all[key];prediction=model.predict(path,candidate,key);complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==expected;allowed=not complete or equal
   patch={'rule':FAMILY,'method_id':key,'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'model_source_sha256':deps['tools/Round7SlotModel.cs'],'client_sha256':deps['tools/round7_slot_model.py'],'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'source_file_sha256':sha(path),'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),'complete':complete,'predicted_types':prediction.get('predicted_types'),'source_only':True,'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json')}
   if allowed:patches.append(patch);potential+=next(m for m in original if m['stable_id']==key)['header']['code_size']
   proofs.append({'method_id':key,'original_types':expected,'prediction':prediction,'types_equal':equal,'eligible_under_user_model_policy':allowed,'before_inventory':inventory(before),'after_inventory':inventory(after)})
   print(json.dumps({'method_id':key,'complete':complete,'types_equal':equal,'eligible':allowed,'predicted_count':len(prediction.get('predicted_types',[]))}),flush=True)
 report={'operation':'calculated-condition-gate-compositions','family':FAMILY,'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'baseline':str(base.relative_to(ROOT)).replace('\\','/'),'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','patches':patches,'proofs':proofs,'potential_methods':len(patches),'potential_original_IL_bytes':potential,'novelty':'Billing null continue plus positive state/purchase and emptytype4; collision three sequential integer guards plus positive phase gates and ordered float per-arm stores. CGI initializer has unsupported mapping and is not a candidate.'}
 (ROOT/'reports/round9-condition-gate-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
