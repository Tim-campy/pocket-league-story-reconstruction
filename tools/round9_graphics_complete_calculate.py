"""Fresh bounded NoEmit gate for the full four-component Graphics source."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round8_roslyn_model import SlotModelClient
from round9_boolean_composition_calculate import TARGETS
from round9_boolean_composition_source import once,close
from round9_graphics_complete_source import transform_graphics_complete
FAMILY='d9-calculated-graphics-Boolean-block-negation-composition'
def main():
 work=ROOT/'build/round9/graphics-complete-calculated';work.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle15/after/KairoLibrary'
 manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for s in manifest['sources']:s['path']=str(base/s['path']);s['input_path']=s['path']
 mp=work/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
 key,logical,header,_,_=TARGETS[0];path=base/logical;assert sha(path)==sha(ROOT/logical)
 source=path.read_bytes().decode('utf-8-sig');start=once(source,header);end=close(source,source.index('{',start));before=source[start:end];after=transform_graphics_complete(before);candidate=source[:start]+after+source[end:];expected=json.loads((base/'plain-local-types.json').read_text())[key]
 with SlotModelClient(mp) as slots:prediction=slots.predict(path,candidate,key)
 complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==expected;allowed=not complete or equal
 deps={p:sha(ROOT/p) for p in ('tools/round9_graphics_complete_source.py','tools/round9_graphics_complete_fixtures.py','tools/round9_graphics_complete_calculate.py','tools/round9_boolean_composition_source.py','tools/round9_boolean_composition_fixtures.py','tools/round9_boolean_composition_calculate.py','tools/Round8RoslynModel.cs','tools/round8_roslyn_model.py')}
 evidence={p:sha(ROOT/p) for p in ('reports/round9-graphics-complete-fixture.json','reports/round9-cycle14-probe-KairoLibrary-engine.json')}
 patch={'rule':FAMILY,'method_id':key,'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,
  'model_source_sha256':deps['tools/Round8RoslynModel.cs'],'client_sha256':deps['tools/round8_roslyn_model.py'],'dependency_sha256':deps,'evidence_sha256':evidence,'calculator_sha256':sha(Path(__file__)),
  'source_file_sha256':sha(path),'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),'complete':complete,'predicted_types':prediction.get('predicted_types'),'source_only':True,
  'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json')}
 report={'operation':'whole-Graphics-source-plus-unary-negation','family':FAMILY,'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,
  'baseline':str(base.relative_to(ROOT)).replace('\\','/'),'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','proofs':[{'method_id':key,'original_types':expected,'prediction':prediction,'types_equal':equal,'eligible_under_user_model_policy':allowed}],
  'patches':[patch] if allowed else [],'potential_methods':int(allowed),'potential_original_IL_bytes':3513 if allowed else 0,'novelty':'Rejected C14 whole composition plus separately proved negation delta; no adoption of the intermediate candidate.'}
 (ROOT/'reports/round9-graphics-complete-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({'complete':complete,'types_equal':equal,'eligible':allowed}))
if __name__=='__main__':main()
