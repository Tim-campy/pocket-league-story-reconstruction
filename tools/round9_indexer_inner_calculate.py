"""Fresh source-only binding and models for Dictionary/local inner assignment chains."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round8_roslyn_model import SlotModelClient
from round9_indexer_inner_source import close,transform_material_indexer_local_inner,inventories as inventory
def extract(source,header):
 assert source.count(header)==1;start=source.index(header);end=close(source,source.index('{',start));return start,end,source[start:end]
FAMILY='d9-calculated-indexer-local-inner-compositions'
def main():
 work=ROOT/'build/round9/indexer-inner-calculated';work.mkdir(parents=True,exist_ok=True);base=ROOT/'build/round9/cycles/cycle38/after/KairoLibrary';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for row in manifest['sources']:row['path']=str(base/row['path']);row['input_path']=row['path']
 mp=work/'source-only-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8');expected_all=json.loads((base/'plain-local-types.json').read_text(encoding='utf-8-sig'));original=json.loads((base/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']
 deps={p:sha(ROOT/p)for p in ('tools/round9_indexer_inner_source.py', 'tools/round9_indexer_inner_fixtures.py', 'tools/round9_indexer_inner_calculate.py', 'tools/Round8RoslynModel.cs', 'tools/round8_roslyn_model.py', 'tools/fixtures/round9-indexer-inner/FixtureIndexer.cs')};evidence={'reports/round9-indexer-inner-fixtures.json':sha(ROOT/'reports/round9-indexer-inner-fixtures.json')};patches=[];proofs=[];potential=0
 targets=[('kairo.unity.graphics.MaterialManager::GetMaterial(kairo.unity.ui.Graphics,kairo.unity.ui.Image,UnityEngine.Color[],System.Single,System.Boolean,System.Int32,System.Boolean,System.Boolean,System.Boolean)->kairo.unity.graphics.Material','src/KairoLibrary/kairo/unity/graphics/MaterialManager.cs','public Material GetMaterial(kairo.unity.ui.Graphics g, Image image, Color[] colors, float alpha, bool clipRegion, int opaTransOrder, bool clearDepth, bool clearColor, bool clearStencil)',transform_material_indexer_local_inner),('kairo.unity.graphics.MaterialManager::GetMaterial(UnityEngine.Shader,kairo.unity.ui.Image,System.Object[])->kairo.unity.graphics.Material','src/KairoLibrary/kairo/unity/graphics/MaterialManager.cs','public Material GetMaterial(Shader shader, Image image, params object[] args)',transform_material_indexer_local_inner)]
 with SlotModelClient(mp)as model:
  for key,logical,header,transform in targets:
   path=base/logical;assert sha(path)==sha(ROOT/logical);source=path.read_bytes().decode('utf-8-sig');start,end,before=extract(source,header);after=transform(before);candidate=source[:start]+after+source[end:];expected=expected_all[key];prediction=model.predict(path,candidate,key);complete=bool(prediction.get('complete'));equal=prediction.get('predicted_types')==expected;allowed=not complete or equal
   patch={'rule':FAMILY,'method_id':key,'logical_path':logical,'start':len(source[:start].encode('utf-16-le'))//2,'end':len(source[:end].encode('utf-16-le'))//2,'before':before,'after':after,'model_source_sha256':deps['tools/Round8RoslynModel.cs'],'client_sha256':deps['tools/round8_roslyn_model.py'],'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'source_file_sha256':sha(path),'source_text_sha256':hashlib.sha256(source.encode()).hexdigest(),'complete':complete,'predicted_types':prediction.get('predicted_types'),'source_only':True,'source_snapshot':str(base.relative_to(ROOT)).replace('\\','/'),'original_dump_sha256':sha(base/'original-methods.json'),'own_dump_sha256':sha(base/'rebuilt-methods.json'),'own_compilation_receipt_sha256':sha(base/'verification.json')}
   if allowed:patches.append(patch);potential+=next(m for m in original if m['stable_id']==key)['header']['code_size']
   proofs.append({'method_id':key,'original_types':expected,'prediction':prediction,'types_equal':equal,'eligible_under_user_model_policy':allowed,'before_inventory':inventory(before),'after_inventory':inventory(after)})
   print(json.dumps({'method_id':key,'complete':complete,'types_equal':equal,'eligible':allowed,'predicted_count':len(prediction.get('predicted_types',[]))}),flush=True)
 report={'operation':'calculated-indexer-local-inner-compositions','family':FAMILY,'calculator_sha256':sha(Path(__file__)),'dependency_sha256':deps,'evidence_sha256':evidence,'baseline':str(base.relative_to(ROOT)).replace('\\','/'),'game_compilations':0,'adopted':False,'body_oracle':'UNKNOWN','patches':patches,'proofs':proofs,'potential_methods':len(patches),'potential_original_IL_bytes':potential,'novelty':'Two overloads put named Dictionary and Material local stores inside indexer assignments, eliminating used-assignment spill temps. Old field-only chain rule did not cover indexers; no arbitrary setter/capture generalization.'}
 (ROOT/'reports/round9-indexer-inner-patches.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
