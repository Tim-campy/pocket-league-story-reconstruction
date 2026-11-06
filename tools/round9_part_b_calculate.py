"""Five Part-B source hypotheses from A5; no generic C extractor and no game build."""
import importlib,json,sys,hashlib
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round6_alignment import align_methods
from round9_part_b_fixtures import CASES,query,apply,RULE
WORK=ROOT/'build/round9/part-b-calculated';WORK.mkdir(parents=True,exist_ok=True)
BASE=ROOT/'build/round9/cycles/a05/after'
TARGETS={
 'b9-event-loop-split-guard':('Assembly-CSharp','form.GameForm::_update(System.Int32)->System.Boolean','src/Assembly-CSharp/form/GameForm.cs'),
 'b9-empty-arm-common-return':('Assembly-CSharp','form.SubForm::Init()->System.Boolean','src/Assembly-CSharp/form/SubForm.cs'),
 'b9-guard-return-empty-positive-arm':('Assembly-CSharp','form.SoccerForm::Draw_newPlayerPopular(kairo.unity.ui.Graphics)->System.Void','src/Assembly-CSharp/form/SoccerForm.cs'),
 'b9-positive-render-with-gui-else':('KairoLibrary','kairo.unity.ui.Balloon::Draw(kairo.unity.ui.Graphics,System.Single,System.Single,UnityEngine.Matrix4x4)->System.Void','src/KairoLibrary/kairo/unity/ui/Balloon.cs'),
 'b9-boolean-value-before-if':('KairoLibrary','kairo.unity.ui.Graphics::_drawBitmap(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void','src/KairoLibrary/kairo/unity/ui/Graphics.cs')}

def main():
    cfg=json.loads((ROOT/'config/round9-protocol.json').read_text());patches=[];proofs=[]
    for assembly in ['Assembly-CSharp','KairoLibrary']:
        manifest=json.loads((BASE/assembly/'build-manifest.json').read_text(encoding='utf-8-sig'))
        for source in manifest['sources']:
            frozen=str(BASE/assembly/source['path']);source['input_path']=frozen;source['path']=frozen
        mp=WORK/(assembly+'-source-only-manifest.json');mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
        model_cfg=cfg['models'][assembly];Client=importlib.import_module(model_cfg['module']).SlotModelClient
        original={m['stable_id']:m for m in json.loads((BASE/assembly/'original-methods.json').read_text())['selected_methods']}
        rebuilt={m['stable_id']:m for m in json.loads((BASE/assembly/'rebuilt-methods.json').read_text())['selected_methods']}
        original_types=json.loads((BASE/assembly/'plain-local-types.json').read_text())
        with Client(mp) as predictor:
            for family,rule,owner,name,expected in CASES:
                target_assembly,identity,logical=TARGETS[rule]
                if target_assembly!=assembly:continue
                source=(BASE/assembly/logical).read_bytes().decode('utf-8-sig')
                # Every tree is read from the frozen A5 snapshot, independent of later Main adoptions.
                rows=query(BASE/assembly/logical,mp,rule)
                found=[(row,patch) for row in rows for patch in row['patches']]
                assert len(found)==1,(rule,[(r['owner'],r['method'],len(r['patches'])) for r in rows if r['patches']])
                row,small=found[0];assert source.encode('utf-16-le')[small['start']*2:small['end']*2].decode('utf-16-le')==small['before']
                candidate=apply(source,small)
                before_model=predictor.predict(BASE/assembly/logical,source,identity);after_model=predictor.predict(BASE/assembly/logical,candidate,identity)
                start,end=row['declaration_start'],row['declaration_end'];raw=source.encode('utf-16-le')
                after_end=end+len(small['after'].encode('utf-16-le'))//2-(small['end']-small['start'])
                before=raw[start*2:end*2].decode('utf-16-le');after=candidate.encode('utf-16-le')[start*2:after_end*2].decode('utf-16-le')
                # Prior accepted patches can leave mixed newlines. Preserve the
                # literal raw source; never normalize it merely for transport.
                assert source.count(before)==1
                patch=dict(start=start,end=end,before=before,after=after,rule=rule,distance=0,method_id=identity,path=str(ROOT/logical),logical_path=logical,
                    model_source_sha256=sha(ROOT/model_cfg['source']),model_client_sha256=sha(ROOT/model_cfg['client']),client_sha256=sha(ROOT/model_cfg['client']))
                patches.append(patch);alignment=align_methods(original[identity],rebuilt[identity])
                proof=dict(assembly=assembly,method_id=identity,rule=rule,logical_path=logical,source_sha256=sha(BASE/assembly/logical),
                    before_model=before_model,after_model=after_model,original_types=original_types[identity],
                    candidate_predicted_types_equal=after_model['predicted_types']==original_types[identity],
                    model_complete_wrong_signature=after_model['complete'] and after_model['predicted_types']!=original_types[identity],
                    before_CRLF=before.count('\r\n'),before_LF_only=before.count('\n')-before.count('\r\n'),after_CRLF=after.count('\r\n'),after_LF_only=after.count('\n')-after.count('\r\n'),
                    AST_scope_proofs=row['scope_proofs'],single_atomic_source_occurrence=True,
                    primary_IL_alignment=alignment,original_header=original[identity]['header'],rebuilt_header=rebuilt[identity]['header'],
                    original_exception_regions=original[identity]['exception_regions'],game_compilations=0,body_match='UNKNOWN')
                proofs.append(proof)
                print(rule,'patches=1','types',len(after_model['predicted_types']),'complete',after_model['complete'],'originalTypesEqual',proof['candidate_predicted_types_equal'],'reasons',after_model['unsupported_reasons'])
    result=dict(operation='round9-PartB-five-explicit-source-occurrences',baseline='build/round9/cycles/a05/after',
        helper_sha256=sha(RULE),game_compilations=0,adopted=False,patches=patches,proofs=proofs)
    files=['tools/Round9PartBRules.cs','tools/round9_part_b_calculate.py','tools/round9_part_b_fixtures.py','tools/compiler_probes.py']
    for model in cfg['models'].values():files.extend([model['source'],model['client']])
    dependencies={name:sha(ROOT/name) for name in dict.fromkeys(files)}
    result.update(dependency_sha256=dependencies,syntax_source_sha256=sha(RULE),calculator_sha256=sha(Path(__file__)))
    for patch in result['patches']:patch.update(dependency_sha256=dependencies,syntax_source_sha256=sha(RULE),calculator_sha256=sha(Path(__file__)))
    (WORK/'patches-and-proof.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')

if __name__=='__main__':main()
