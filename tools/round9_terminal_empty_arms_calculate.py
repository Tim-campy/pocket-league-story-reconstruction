"""Fresh source/model evidence for the joint eight-arm terminal chain."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round7_slot_model import SlotModelClient
from round9_terminal_empty_arms_source import lex,matching,transform_friend,inventory
FAMILY='d9-calculated-terminal-empty-arm-chain-composition'
def main():
 work=ROOT/'build/round9/terminal-empty-arms-calculated';work.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle18/after/Assembly-CSharp';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for row in manifest['sources']:row['path']=str(base/row['path']);row['input_path']=row['path']
 mp=work/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
 key='system.form.FriendForm::_draw(kairo.unity.ui.Graphics)->System.Void';logical='src/Assembly-CSharp/system/form/FriendForm.cs';path=base/logical;assert sha(path)==sha(ROOT/logical)
 source=path.read_bytes().decode('utf-8-sig');header='public void _draw(Graphics g)';assert source.count(header)==1;start=source.index(header);ts=lex(source[start:]);opening=next(i for i,t in enumerate(ts) if t.value=='{');end=start+ts[matching(ts,opening)].end;before=source[start:end];after=transform_friend(before);candidate=source[:start]+after+source[end:];expected=json.loads((base/'plain-local-types.json').read_text())[key]
 with SlotModelClient(mp) as model:prediction=model.predict(path,candidate,key)
 complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==expected;allowed=not complete or equal
 deps={p:sha(ROOT/p) for p in ('tools/round9_terminal_empty_arms_source.py','tools/round9_terminal_empty_arms_fixtures.py','tools/round9_terminal_empty_arms_calculate.py','tools/Round7SlotModel.cs','tools/round7_slot_model.py')};evidence={'reports/round9-terminal-empty-arms-fixtures.json':sha(ROOT/'reports/round9-terminal-empty-arms-fixtures.json')}
 patch={'rule':FAMILY,'method_id':key,'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,
  'model_source_sha256':deps['tools/Round7SlotModel.cs'],'client_sha256':deps['tools/round7_slot_model.py'],'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'source_file_sha256':sha(path),
  'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),'complete':complete,'predicted_types':prediction.get('predicted_types'),'source_only':True,'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),
  'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json')}
 report={'operation':'whole-terminal-empty-arm-chain-calculation','family':FAMILY,'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'baseline':str(base.relative_to(ROOT)).replace('\\','/'),
  'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','patches':[patch] if allowed else [],'proofs':[{'method_id':key,'original_types':expected,'prediction':prediction,'types_equal':equal,'eligible_under_user_model_policy':allowed,'before_inventory':inventory(before),'after_inventory':inventory(after)}],
  'potential_methods':int(allowed),'potential_original_IL_bytes':6994 if allowed else 0,'novelty':'Joint eight empty arms and complete retained type12 tail; not D01 single-arm/return transformation.'}
 (ROOT/'reports/round9-terminal-empty-arms-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({'complete':complete,'types_equal':equal,'eligible':allowed,'predicted_count':len(prediction.get('predicted_types',[]))}))
if __name__=='__main__':main()
