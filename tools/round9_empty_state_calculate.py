"""Fresh source-only binding and models for empty state arms and intermediate lexical joins."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round7_slot_model import SlotModelClient
from round9_empty_state_source import extract,transform_set_pos,transform_change_state,transform_team_update,inventory
FAMILY='d9-calculated-empty-state-compositions'
def main():
 work=ROOT/'build/round9/empty-state-calculated';work.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle32/after/Assembly-CSharp';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for row in manifest['sources']:row['path']=str(base/row['path']);row['input_path']=row['path']
 mp=work/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8');expected_all=json.loads((base/'plain-local-types.json').read_text(encoding='utf-8-sig'));original=json.loads((base/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']
 deps={p:sha(ROOT/p)for p in ('tools/round9_empty_state_source.py', 'tools/round9_empty_state_fixtures.py', 'tools/round9_empty_state_calculate.py', 'tools/Round7SlotModel.cs', 'tools/round7_slot_model.py', 'tools/fixtures/round9-empty-state/setpos-before.cs.txt', 'tools/fixtures/round9-empty-state/setpos-control.cs.txt', 'tools/fixtures/round9-empty-state/change-before.cs.txt', 'tools/fixtures/round9-empty-state/change-control.cs.txt', 'tools/fixtures/round9-empty-state/team-before.cs.txt', 'tools/fixtures/round9-empty-state/team-control.cs.txt')};evidence={'reports/round9-empty-state-fixtures.json':sha(ROOT/'reports/round9-empty-state-fixtures.json')};patches=[];proofs=[];potential=0
 targets=[('game.TGroupArea::SetPosData()->System.Void','src/Assembly-CSharp/game/TGroupArea.cs','public void SetPosData()',transform_set_pos),('game.TGroupArea::ChangeState(System.Int32)->System.Void','src/Assembly-CSharp/game/TGroupArea.cs','public void ChangeState(int next_state)',transform_change_state),('game.Team::Update()->System.Void','src/Assembly-CSharp/game/Team.cs','public void Update()',transform_team_update)]
 with SlotModelClient(mp)as model:
  for key,logical,header,transform in targets:
   path=base/logical;assert sha(path)==sha(ROOT/logical);source=path.read_bytes().decode('utf-8-sig');start,end,before=extract(source,header);after=transform(before);candidate=source[:start]+after+source[end:];expected=expected_all[key];prediction=model.predict(path,candidate,key);complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==expected;allowed=not complete or equal
   patch={'rule':FAMILY,'method_id':key,'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'model_source_sha256':deps['tools/Round7SlotModel.cs'],'client_sha256':deps['tools/round7_slot_model.py'],'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'source_file_sha256':sha(path),'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),'complete':complete,'predicted_types':prediction.get('predicted_types'),'source_only':True,'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json')}
   if allowed:patches.append(patch);potential+=next(m for m in original if m['stable_id']==key)['header']['code_size']
   proofs.append({'method_id':key,'original_types':expected,'prediction':prediction,'types_equal':equal,'eligible_under_user_model_policy':allowed,'before_inventory':inventory(before),'after_inventory':inventory(after)})
   print(json.dumps({'method_id':key,'complete':complete,'types_equal':equal,'eligible':allowed,'predicted_count':len(prediction.get('predicted_types',[]))}),flush=True)
 report={'operation':'calculated-empty-state-compositions','family':FAMILY,'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'baseline':str(base.relative_to(ROOT)).replace('\\','/'),'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','patches':patches,'proofs':proofs,'potential_methods':len(patches),'potential_original_IL_bytes':potential,'novelty':'SetPos positive mode3/direct index tests/neg; ChangeState empty mode3 without extra else and state14/local TenantIn join; Team two empty gameState arms and final7/6/8 chain. Known isolated negatives retained.'}
 (ROOT/'reports/round9-empty-state-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
