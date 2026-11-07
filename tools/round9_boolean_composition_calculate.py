"""Fresh source-only model gate for two composed Roslyn source proposals."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round8_roslyn_model import SlotModelClient
from round9_boolean_composition_source import calculate_graphics,calculate_shader,once,close
WORK=ROOT/'build/round9/boolean-composition-calculated';FAMILY='d9-calculated-boolean-block-storage-composition'
TARGETS=[('kairo.unity.ui.Graphics::_drawBitmap(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void','src/KairoLibrary/kairo/unity/ui/Graphics.cs','protected void _drawBitmap(Image image, float dx, float dy, float width, float height, float sx, float sy, float swidth, float sheight)',calculate_graphics,'graphics'),
 ('kairo.unity.graphics.ShaderBuilder::GetCode(System.Int32)->System.String','src/KairoLibrary/kairo/unity/graphics/ShaderBuilder.cs','private static string GetCode(int nameId)',calculate_shader,'shader')]
def main():
 WORK.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle13/after/KairoLibrary';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for row in manifest['sources']:row['path']=str(base/row['path']);row['input_path']=row['path']
 mp=WORK/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
 original={m['stable_id']:m for m in json.loads((base/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']};types=json.loads((base/'plain-local-types.json').read_text())
 deps={p:sha(ROOT/p) for p in ('tools/round9_boolean_composition_source.py','tools/round9_boolean_composition_calculate.py','tools/round9_boolean_composition_fixtures.py','tools/Round8RoslynModel.cs','tools/round8_roslyn_model.py')}
 evidence={p:sha(ROOT/p) for p in ('reports/round9-boolean-composition-fixtures.json','reports/round9-cycle11-KairoLibrary-engine.json')};proofs=[];patches=[]
 with SlotModelClient(mp) as model:
  for key,logical,header,transform,label in TARGETS:
   path=base/logical;assert sha(path)==sha(ROOT/logical),logical
   source=path.read_bytes().decode('utf-8-sig');start=once(source,header);end=close(source,source.index('{',start));before=source[start:end];result=transform(before);after=result['after'];candidate=source[:start]+after+source[end:]
   prediction=model.predict(path,candidate,key);old=model.predict(path,source,key);complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==types[key];allowed=not complete or equal
   record={'method_id':key,'logical_path':logical,'source_file_sha256':sha(path),'before_prediction':old,'after_prediction':prediction,'original_types':types[key],
    'complete':complete,'types_equal':equal,'eligible_under_user_model_policy':allowed,'reasons':result['reasons'],'source_invariants':result['proofs'],'game_compilations':0,'body_oracle':'UNKNOWN'}
   proofs.append(record);print(json.dumps({k:record[k] for k in ('method_id','complete','types_equal','eligible_under_user_model_policy')}),flush=True)
   if not allowed:continue
   patches.append({'rule':FAMILY+'-'+label,'method_id':key,'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,
    'model_source_sha256':deps['tools/Round8RoslynModel.cs'],'client_sha256':deps['tools/round8_roslyn_model.py'],'dependency_sha256':deps,'evidence_sha256':evidence,'calculator_sha256':sha(Path(__file__)),
    'source_file_sha256':sha(path),'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),'complete':complete,'predicted_types':prediction.get('predicted_types'),
    'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json'),
    'source_only':True,'rebase_policy':'unique exact UTF16 method; fresh bounded model and compiled original signature required'})
   (WORK/(label+'-candidate.cs')).write_bytes(candidate.encode())
 report={'operation':'calculated-Boolean-plus-block-storage-source-compositions','family':FAMILY,'baseline':str(base.relative_to(ROOT)).replace('\\','/'),'calculator_sha256':sha(Path(__file__)),
  'dependency_sha256':deps,'evidence_sha256':evidence,'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','patches':patches,'proofs':proofs,'potential_methods':len(patches),'potential_original_IL_bytes':sum(original[p['method_id']]['header']['code_size'] for p in patches),
  'novelty':'D11 isolated shapes plus new jointly required block order/eager Boolean and/storage layout; no isolated rejected candidate repeated.'}
 (ROOT/'reports/round9-boolean-composition-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
