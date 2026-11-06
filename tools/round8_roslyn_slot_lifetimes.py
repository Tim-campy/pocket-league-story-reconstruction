"""Calculate three source hypotheses from pinned IL slot uses; no game build."""
from pathlib import Path
import hashlib,json,re,sys
root=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file())
sys.path.insert(0,str(root/'tools'))
from round8_roslyn_model import SlotModelClient

work=root/'build/round8/roslyn-slot-lifetimes-calculated';work.mkdir(parents=True,exist_ok=True)
base=root/'build/round8/cycles/cycle03/after/KairoLibrary'
diagnosis=json.loads((root/'reports/round8-roslyn-slot-lifetime-analysis.json').read_text())
for row in diagnosis['methods']:
    row['stable_id']=row['method_id'];row['source']=row['logical_path']
manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
for row in manifest['sources']:row['input_path']=str(base/row['path'])
(work/'manifest.json').write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
patches=[];calculations=[]
prefix='roslyn-store-use-lifetime-scope'

def sha_bytes(raw):return hashlib.sha256(raw).hexdigest()
def variants(name,before):
    if name=='GetCode':
        anchor='\t\t\tint num = bytes.Length;\r\n'
        assert before.count(anchor)==1
        after=before.replace(anchor,anchor+'\t\t\tint num2, num3, num4, num5;\r\n',1)
        after,count=re.subn(r'(?m)^(\t*)int (num[2-5]) = ',r'\1\2 = ',after)
        assert count==14,count
        return [('shared-scratch-locals',after,dict(shared_variables=['num2','num3','num4','num5'],removed_declaration_count=count))]
    if name=='DrawArc':
        beforeloop='int num15 = 0;\r\n\t\t\t\t\t\twhile (num7 < num6)'
        afterloop='for (int num15 = 0; num7 < num6; )'
        assert before.count(beforeloop)==1
        after=before.replace(beforeloop,afterloop,1)
        alias='RenderPass[] renderPass = GetRenderPass(guiStyle_.normal.textColor);\r\n\t\t\t\tforeach (RenderPass renderPass2 in renderPass)'
        direct='foreach (RenderPass renderPass2 in GetRenderPass(guiStyle_.normal.textColor))'
        assert after.count(alias)==1
        return [('while-counter-scope',after,dict(counter='num15',rebuilt_slot=13,original_slot=22)),
                ('while-counter-scope-inline-array',after.replace(alias,direct,1),dict(counter='num15',rebuilt_slot=13,original_slot=22,foreach_array_alias_inlined=True))]
    if name=='OnPostRender':
        opening='\t\t\t\tList<Image.TextRenderTask> textRenderTasks_ = image_.textRenderTasks_;'
        ending='\t\t\t\t}\r\n\t\t\t}\r\n\t\t\tcatch (Exception t)'
        assert before.count(opening)==before.count(ending)==1
        after=before.replace(opening,'\t\t\t\t{\r\n'+opening,1).replace(ending,'\t\t\t\t}\r\n'+ending,1)
        return [('late-child-block',after,dict(moved_to_child_scope=['textRenderTasks_','balloonRenderTasks_','touchTasks_','flag2'],graphics_stays_outer=True))]
    raise ValueError(name)

with SlotModelClient(work/'manifest.json') as model:
    for record in diagnosis['methods']:
        identity=record['stable_id'];logical=record['source'];source=(base/logical).read_bytes().decode('utf-8-sig')
        prediction=model.predict(root/logical,source,identity)
        start,end=prediction['declaration_start'],prediction['declaration_end'];raw=source.encode('utf-16-le')
        before=raw[start*2:end*2].decode('utf-16-le');name=identity.split('::')[1].split('(')[0]
        for rule,after,reason in variants(name,before):
            assert before.count('\n')==before.count('\r\n') and after.count('\n')==after.count('\r\n')
            candidate=(raw[:start*2]+after.encode('utf-16-le')+raw[end*2:]).decode('utf-16-le')
            after_model=model.predict(root/logical,candidate,identity)
            patch=dict(start=start,end=end,before=before,after=after,rule=prefix+'-'+rule,distance=0,
                method_id=identity,path=str(root/logical),logical_path=logical)
            patches.append(patch)
            calculations.append(dict(method_id=identity,rule=patch['rule'],reason=reason,
                baseline_source_sha256=sha_bytes((base/logical).read_bytes()),before_utf8_sha256=sha_bytes(before.encode('utf-8')),
                original_types=record['original_types'],rebuilt_types=record['rebuilt_types'],candidate_prediction=after_model,
                predicted_original_signature_equal=after_model['predicted_types']==record['original_types'],
                original_il_bytes=record['original_il_bytes'],body_match='UNKNOWN',game_compilations=0,
                before_CRLF_count=before.count('\r\n'),before_LF_count=before.count('\n'),start_utf16=start,end_utf16=end))
result=dict(operation='source-only-Roslyn-slot-lifetime-calculated-patches',measurement=diagnosis['measurement'],
    game_compilations=0,adopted=False,rule_prefix=prefix,patches=patches,calculations=calculations,
    model_source_sha256=sha_bytes((root/'tools/Round8RoslynModel.cs').read_bytes()),
    model_client_sha256=sha_bytes((root/'tools/round8_roslyn_model.py').read_bytes()),
    model_incompleteness_does_not_reject=True)
dependency_files=['tools/round8_roslyn_slot_lifetimes.py','tools/Round8RoslynModel.cs','tools/round8_roslyn_model.py','tools/compiler_probes.py']
dependencies={name:sha_bytes((root/name).read_bytes()) for name in dependency_files}
result.update(dependency_sha256=dependencies,calculator_sha256=dependencies['tools/round8_roslyn_slot_lifetimes.py'],client_sha256=result['model_client_sha256'])
for patch in result['patches']:
    patch.update(dependency_sha256=dependencies,model_source_sha256=result['model_source_sha256'],model_client_sha256=result['model_client_sha256'],client_sha256=result['model_client_sha256'],calculator_sha256=result['calculator_sha256'])
(root/'reports/round8-roslyn-slot-lifetime-patches.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
for row in calculations:print(row['rule'],len(row['candidate_prediction']['predicted_types']),row['predicted_original_signature_equal'],row['candidate_prediction']['complete'],row['candidate_prediction']['unsupported_reasons'])
