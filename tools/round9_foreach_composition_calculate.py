"""Bind source compositions to current source, original slots and NoEmit model."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round7_slot_model import SlotModelClient
from round9_foreach_composition_source import TARGETS,method_span
FAMILY='d9-calculated-foreach-slot-control-composition'
WORK=ROOT/'build/round9/foreach-composition-calculated'
def main():
 WORK.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle12/after/Assembly-CSharp'
 manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for source in manifest['sources']:
  source['path']=str(base/source['path']);source['input_path']=source['path']
 mp=WORK/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
 original={m['stable_id']:m for m in json.loads((base/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']}
 expected=json.loads((base/'plain-local-types.json').read_text(encoding='utf-8-sig'))
 dependencies={p:sha(ROOT/p) for p in ('tools/round9_foreach_composition_source.py','tools/round9_foreach_composition_calculate.py','tools/round9_foreach_composition_fixtures.py','tools/Round7SlotModel.cs','tools/round7_slot_model.py')}
 evidence={'reports/round9-foreach-composition-fixtures.json':sha(ROOT/'reports/round9-foreach-composition-fixtures.json')}
 patches=[];proofs=[]
 with SlotModelClient(mp) as slots:
  for key,logical,signature,transform,subrule,existing in TARGETS:
   path=base/logical;assert sha(path)==sha(ROOT/logical),('source-changed',logical)
   source=path.read_bytes().decode('utf-8-sig');start,end=method_span(source,signature);before=source[start:end];after=transform(before)
   candidate=source[:start]+after+source[end:];prediction=slots.predict(path,candidate,key);old=slots.predict(path,source,key)
   complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==expected[key];allowed=not complete or equal
   proof={'method_id':key,'logical_path':logical,'source_file_sha256':sha(path),'original_types':expected[key],'before_prediction':old,'after_prediction':prediction,
    'complete':complete,'types_equal':equal,'eligible_under_user_model_policy':allowed,'existing_primitive_not_novel':existing,'game_compilations':0,
    'original_il_bytes':original[key]['header']['code_size'],'source_transform':subrule,'body_oracle':'UNKNOWN'}
   proofs.append(proof);print(json.dumps({k:proof[k] for k in ('method_id','complete','types_equal','eligible_under_user_model_policy','existing_primitive_not_novel')},ensure_ascii=True),flush=True)
   if not allowed:continue
   patch={'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,
    'rule':FAMILY+'-'+subrule,'method_id':key,'logical_path':logical,'source_file_sha256':sha(path),'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),
    'model_source_sha256':dependencies['tools/Round7SlotModel.cs'],'client_sha256':dependencies['tools/round7_slot_model.py'],
    'calculator_sha256':sha(Path(__file__)),'dependency_sha256':dependencies,'evidence_sha256':evidence,
    'complete':complete,'predicted_types':prediction.get('predicted_types'),'existing_primitive_not_novel':existing,'source_only':True,
    'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json'),
    'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),'rebase_policy':'unique exact UTF16 declaration and fresh model/compiled-signature gate'}
   patches.append(patch);(WORK/(subrule+'-candidate.cs')).write_bytes(candidate.encode())
 report={'operation':'source-only-foreach-slot-control-compositions','family':FAMILY,'baseline':str(base.relative_to(ROOT)).replace('\\','/'),
  'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','calculator_sha256':sha(Path(__file__)),'dependency_sha256':dependencies,
  'evidence_sha256':evidence,'patches':patches,'proofs':proofs,'potential_methods':len(patches),
  'potential_original_IL_bytes':sum(original[p['method_id']]['header']['code_size'] for p in patches),
  'policy':'Complete wrong signatures rejected. Incomplete predictions remain bounded candidates, never assumed correct.',
  'novelty':'Three joint compositions; Process is explicitly a previously known primitive with no compiled trial proved in historical aggregate records.'}
 (ROOT/'reports/round9-foreach-composition-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
