"""Fresh source-only binding and models for three loop target compositions."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round7_slot_model import SlotModelClient
from round9_loop_target_source import extract,transform_execute_script,transform_coat,transform_zsort,inventory
FAMILY='d9-calculated-section-and-loop-target-compositions'
def main():
 work=ROOT/'build/round9/loop-target-calculated';work.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle27/after/Assembly-CSharp';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for row in manifest['sources']:row['path']=str(base/row['path']);row['input_path']=row['path']
 mp=work/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8');expected_all=json.loads((base/'plain-local-types.json').read_text(encoding='utf-8-sig'));original=json.loads((base/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']
 deps={p:sha(ROOT/p)for p in ('tools/round9_loop_target_source.py','tools/round9_loop_target_fixtures.py','tools/round9_loop_target_calculate.py','tools/Round7SlotModel.cs','tools/round7_slot_model.py')};evidence={'reports/round9-loop-target-fixtures.json':sha(ROOT/'reports/round9-loop-target-fixtures.json')};patches=[];proofs=[];potential=0
 targets=[('main.AppData::ExecuteScript(System.Int32[],System.Boolean,System.String,System.Int32[],System.Int32[][])->form.SubForm','src/Assembly-CSharp/main/AppData.cs','public SubForm ExecuteScript(int[] scr, bool popup, string args, int[] argi, int[][] script)',transform_execute_script),('game.Map::Draw_coat(kairo.unity.ui.Graphics,System.Boolean)->System.Void','src/Assembly-CSharp/game/Map.cs','public void Draw_coat(Graphics g, bool offscreen)',transform_coat),('main.AppData::DrawZSortObjects(kairo.unity.ui.Graphics,System.Boolean,System.Int32)->System.Void','src/Assembly-CSharp/main/AppData.cs','public void DrawZSortObjects(Graphics g, bool offscreen, int typeBits)',transform_zsort)]
 with SlotModelClient(mp)as model:
  for key,logical,header,transform in targets:
   path=base/logical;assert sha(path)==sha(ROOT/logical);source=path.read_bytes().decode('utf-8-sig');start,end,before=extract(source,header);after=transform(before);candidate=source[:start]+after+source[end:];expected=expected_all[key];prediction=model.predict(path,candidate,key);complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==expected;allowed=not complete or equal
   patch={'rule':FAMILY,'method_id':key,'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'model_source_sha256':deps['tools/Round7SlotModel.cs'],'client_sha256':deps['tools/round7_slot_model.py'],'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'source_file_sha256':sha(path),'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),'complete':complete,'predicted_types':prediction.get('predicted_types'),'source_only':True,'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json')}
   if allowed:patches.append(patch);potential+=next(m for m in original if m['stable_id']==key)['header']['code_size']
   proofs.append({'method_id':key,'original_types':expected,'prediction':prediction,'types_equal':equal,'eligible_under_user_model_policy':allowed,'before_inventory':inventory(before),'after_inventory':inventory(after)})
   print(json.dumps({'method_id':key,'complete':complete,'types_equal':equal,'eligible':allowed,'predicted_count':len(prediction.get('predicted_types',[]))}),flush=True)
 report={'operation':'calculated-section-and-loop-target-compositions','family':FAMILY,'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'baseline':str(base.relative_to(ROOT)).replace('\\','/'),'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','patches':patches,'proofs':proofs,'potential_methods':len(patches),'potential_original_IL_bytes':potential,'novelty':'ExecuteScript case-local joins retain common section breaks; Map direct continue targets preserve child blocks; Zsort eight positive guards plus first empty guard become direct loop continues while case9 remains distinct. Bodies/signatures only accepted by unchanged oracle.'}
 (ROOT/'reports/round9-loop-target-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
