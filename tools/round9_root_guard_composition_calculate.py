"""Fresh NoEmit gate for the joint root-declaration/guard calculation."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round7_slot_model import SlotModelClient
from round9_root_guard_composition_source import TARGETS,method_span,calculate_root_guard
WORK=ROOT/'build/round9/root-guard-composition-calculated';FAMILY='d9-calculated-root-guard-declaration-composition'
def main():
 WORK.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle14/after/Assembly-CSharp';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for row in manifest['sources']:row['path']=str(base/row['path']);row['input_path']=row['path']
 mp=WORK/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8');types=json.loads((base/'plain-local-types.json').read_text())
 deps={p:sha(ROOT/p) for p in ('tools/round9_root_guard_composition_source.py','tools/round9_root_guard_composition_fixtures.py','tools/round9_root_guard_composition_calculate.py','tools/Round7SlotModel.cs','tools/round7_slot_model.py')}
 evidence={'reports/round9-root-guard-composition-fixtures.json':sha(ROOT/'reports/round9-root-guard-composition-fixtures.json')};proofs=[];patches=[]
 with SlotModelClient(mp) as model:
  for key,logical,header,size in TARGETS:
   path=base/logical;assert sha(path)==sha(ROOT/logical),logical
   source=path.read_bytes().decode('utf-8-sig');start,end=method_span(source,header);before=source[start:end];calculated=calculate_root_guard(before);after=calculated['after'];candidate=source[:start]+after+source[end:]
   old=model.predict(path,source,key);prediction=model.predict(path,candidate,key);complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==types[key];allowed=not complete or equal
   proof={'method_id':key,'logical_path':logical,'source_file_sha256':sha(path),'before_prediction':old,'after_prediction':prediction,'original_types':types[key],
    'complete':complete,'types_equal':equal,'eligible_under_user_model_policy':allowed,'lexical_evidence':{k:v for k,v in calculated.items() if k!='after'},'body_oracle':'UNKNOWN','original_il_bytes':size,'game_compilations':0}
   proofs.append(proof);print(json.dumps({k:proof[k] for k in ('method_id','complete','types_equal','eligible_under_user_model_policy')}),flush=True)
   if not allowed:continue
   patches.append({'rule':FAMILY,'method_id':key,'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,
    'model_source_sha256':deps['tools/Round7SlotModel.cs'],'client_sha256':deps['tools/round7_slot_model.py'],'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,
    'source_file_sha256':sha(path),'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),'complete':complete,'predicted_types':prediction.get('predicted_types'),
    'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json'),
    'source_only':True,'rebase_policy':'unique exact UTF16 method; fresh complete/incomplete model policy and actual signature gate'})
   (WORK/(path.stem+'-candidate.cs')).write_bytes(candidate.encode())
 report={'operation':'calculated-root-guard-and-declaration-source-composition','family':FAMILY,'baseline':str(base.relative_to(ROOT)).replace('\\','/'),'calculator_sha256':sha(Path(__file__)),
  'dependency_sha256':deps,'evidence_sha256':evidence,'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','patches':patches,'proofs':proofs,'potential_methods':len(patches),'potential_original_IL_bytes':sum(r['original_il_bytes'] for r in proofs if r['eligible_under_user_model_policy']),
  'novelty':'Joint bare declarations retained in root plus positive tail control; isolated declaration-hoist/control candidates previously failed.'}
 (ROOT/'reports/round9-root-guard-composition-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
