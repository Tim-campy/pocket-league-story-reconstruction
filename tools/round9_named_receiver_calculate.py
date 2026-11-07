"""Current-source binding and bounded NoEmit gate for the named receiver."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round8_roslyn_model import SlotModelClient
from round9_named_receiver_source import calculate_notify_receiver,close
FAMILY='d9-calculated-named-receiver-protected-return-order'
def main():
 work=ROOT/'build/round9/named-receiver-calculated';work.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle17/after/KairoLibrary';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for row in manifest['sources']:row['path']=str(base/row['path']);row['input_path']=row['path']
 mp=work/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
 key='kairo.unity.form.FormManagerBase::_execute()->System.Boolean';logical='src/KairoLibrary/kairo/unity/form/FormManagerBase.cs';path=base/logical;assert sha(path)==sha(ROOT/logical)
 source=path.read_bytes().decode('utf-8-sig');header='private bool _execute()';assert source.count(header)==1;start=source.index(header);end=close(source,source.index('{',start));before=source[start:end];result=calculate_notify_receiver(before);after=result['after'];candidate=source[:start]+after+source[end:];expected=json.loads((base/'plain-local-types.json').read_text())[key]
 with SlotModelClient(mp) as model:prediction=model.predict(path,candidate,key)
 complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==expected;allowed=not complete or equal
 deps={p:sha(ROOT/p) for p in ('tools/round9_named_receiver_source.py','tools/round9_named_receiver_fixtures.py','tools/round9_named_receiver_calculate.py','tools/Round8RoslynModel.cs','tools/round8_roslyn_model.py')};evidence={'reports/round9-named-receiver-fixtures.json':sha(ROOT/'reports/round9-named-receiver-fixtures.json')}
 patch={'rule':FAMILY,'method_id':key,'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,
  'model_source_sha256':deps['tools/Round8RoslynModel.cs'],'client_sha256':deps['tools/round8_roslyn_model.py'],'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,
  'source_file_sha256':sha(path),'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),'complete':complete,'predicted_types':prediction.get('predicted_types'),'source_only':True,
  'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json')}
 report={'operation':'named-receiver-same-try-before-lazy-return-local','family':FAMILY,'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'baseline':str(base.relative_to(ROOT)).replace('\\','/'),
  'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','patches':[patch] if allowed else [],'proofs':[{'method_id':key,'original_types':expected,'prediction':prediction,'types_equal':equal,'eligible_under_user_model_policy':allowed,'source_checks':result['checks'],'name':result['fresh_name']}],
  'potential_methods':int(allowed),'potential_original_IL_bytes':1088 if allowed else 0,'novelty':'Invocation receiver named in same parent try; stable ?. expansion did not cover GetInstance or original local8 reservation.'}
 (ROOT/'reports/round9-named-receiver-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({'complete':complete,'types_equal':equal,'eligible':allowed,'types':len(prediction.get('predicted_types',[]))}))
if __name__=='__main__':main()
