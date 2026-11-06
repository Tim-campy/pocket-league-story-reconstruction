"""Read-only frozen-source calculations; no Emit and no game compilation."""
import importlib,json,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round6_alignment import align_methods
try:
 from round9_float_identity_fixtures import query,apply,RULE
 from round9_float_identity_hints import float_identity_conversion_hints
except ImportError:
 from fixtures import query,apply,RULE
 from hints import float_identity_conversion_hints
WORK=ROOT/'build/round9/part-d-float'
TARGETS=[
 ('kairo.unity.graph.Graph::Easing(System.Single,System.Single,System.Single,System.Single,System.Single)->System.Single','src/KairoLibrary/kairo/unity/graph/Graph.cs','Easing','frame / length',17,17),
 ('kairo.unity.ui.Graphics::_drawImage(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Int32,System.Int32,System.Single,System.Single)->System.Void','src/KairoLibrary/kairo/unity/ui/Graphics.cs','_drawImage','width / swidth',679,679),
 ('kairo.unity.ui.Graphics::_drawImage(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Int32,System.Int32,System.Single,System.Single)->System.Void','src/KairoLibrary/kairo/unity/ui/Graphics.cs','_drawImage','height / sheight',687,686)]

def main():
 base=ROOT/(sys.argv[1] if len(sys.argv)>1 else 'build/round9/cycles/b10/after')/'KairoLibrary'
 assert (base/'build-manifest.json').exists(),base
 manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for source in manifest['sources']:
  source['path']=str(base/source['path']);source['input_path']=source['path']
 mp=WORK/'frozen-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
 cfg=json.loads((ROOT/'config/round9-protocol.json').read_text())['models']['KairoLibrary']
 original={m['stable_id']:m for m in json.loads((base/'original-methods.json').read_text())['selected_methods']}
 rebuilt={m['stable_id']:m for m in json.loads((base/'rebuilt-methods.json').read_text())['selected_methods']}
 types=json.loads((base/'plain-local-types.json').read_text())
 proofs=[];patches=[]
 with importlib.import_module(cfg['module']).SlotModelClient(mp) as predictor:
  for identity,logical,name,needle,oo,ro in TARGETS:
   path=base/logical;source=path.read_bytes().decode('utf-8-sig')
   hints=float_identity_conversion_hints(original[identity],rebuilt[identity],oo,ro)
   assert hints,(identity,oo,ro)
   # The body span comes from Roslyn syntax; the raw file stays CRLF/mixed unchanged.
   position=source.index(needle,source.index(' '+name+'('));point=len(source[:position].encode('utf-16-le'))//2
   request=dict(hints,round7_cycle_rule='d9-explicit-float-identity-parameter',target_method=name,point=point)
   found=[(r,p) for r in query(path,mp,request) for p in r['patches']];assert len(found)==1,(identity,found)
   row,small=found[0];candidate=apply(source,small)
   before_model=predictor.predict(path,source,identity);after_model=predictor.predict(path,candidate,identity)
   start,end=row['declaration_start'],row['declaration_end'];after_end=end+len(small['after'].encode('utf-16-le'))//2-(small['end']-small['start'])
   before=source.encode('utf-16-le')[start*2:end*2].decode('utf-16-le');after=candidate.encode('utf-16-le')[start*2:after_end*2].decode('utf-16-le')
   assert source.count(before)==1
   patch=dict(start=start,end=end,before=before,after=after,rule='d9-explicit-float-identity-parameter',distance=0,method_id=identity,path=str(ROOT/logical),logical_path=logical,
    model_source_sha256=sha(ROOT/cfg['source']),model_client_sha256=sha(ROOT/cfg['client']),client_sha256=sha(ROOT/cfg['client']))
   patches.append(patch)
   proofs.append(dict(method_id=identity,logical_path=logical,parameter_ordinal=hints['round9_identity_parameter_ordinal'],single_source_edit=small,
    original_conversion_evidence=hints['round9_identity_conversion_evidence'],source_sha256=sha(path),before_model=before_model,after_model=after_model,
    original_types=types[identity],after_types_equal=after_model['predicted_types']==types[identity],
    complete_wrong_signature=after_model['complete'] and after_model['predicted_types']!=types[identity],
    unique_raw_UTF16_rebase=True,before_CRLF=before.count('\r\n'),before_LF_only=before.count('\n')-before.count('\r\n'),
    full_IL_match='UNKNOWN',game_compilations=0,first_alignment=align_methods(original[identity],rebuilt[identity])))
   print(name,needle,'NoEmit complete',after_model['complete'],'types',len(after_model['predicted_types']),'equal',proofs[-1]['after_types_equal'])
 files=[cfg['source'],cfg['client'],'tools/compiler_probes.py']
 if Path(__file__).parent==ROOT/'tools':files.extend(['tools/Round9FloatingIdentityRules.cs','tools/round9_float_identity_hints.py','tools/round9_float_identity_fixtures.py','tools/round9_float_identity_calculate.py'])
 result=dict(operation='round9-explicit-Single-identity-source-only-calculation',baseline=str(base.relative_to(ROOT)),
  model_source_sha256=sha(ROOT/cfg['source']),model_client_sha256=sha(ROOT/cfg['client']),original_dump_sha256=sha(base/'original-methods.json'),
  helper_sha256=sha(RULE),hints_sha256=sha(Path(__file__).parent/('round9_float_identity_hints.py' if Path(__file__).parent==ROOT/'tools' else 'hints.py')),calculator_sha256=sha(Path(__file__)),
  dependency_sha256={p:sha(ROOT/p) for p in files},patches=patches,proofs=proofs,game_compilations=0,adopted=False,
  syntax_source_sha256=sha(RULE))
 for patch in patches:patch.update(dependency_sha256=result['dependency_sha256'],syntax_source_sha256=sha(RULE),calculator_sha256=sha(Path(__file__)))
 (WORK/'patches-and-proof.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')

if __name__=='__main__':main()
