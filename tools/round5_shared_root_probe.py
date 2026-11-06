"""Measure atomic shared-root recovery in the two largest SubForm methods."""
import json
from pathlib import Path
from compare_kairo import differences
from metadata_compare import compare_paths
from round3_metadata_candidates import compile_frozen,dump
from round4_engine import apply_patch,prefix,il_cells
from round5_transform import TransformClient

ROOT=Path(__file__).resolve().parents[1]
def read(path):return json.loads(Path(path).read_text(encoding='utf-8-sig'))

def main():
    base=ROOT/'build/round5/baseline/Assembly-CSharp';manifest=read(base/'build-manifest.json')
    original={m['stable_id']:m for m in read(base/'original-methods.json')['selected_methods']}
    before={m['stable_id']:m for m in read(base/'rebuilt-methods.json')['selected_methods']}
    equal={k for k,m in original.items() if 'raw_body_hex'in m and not differences(m,before.get(k))[0]}
    logical='src/Assembly-CSharp/form/SubForm.cs';source=(base/logical).read_bytes().decode('utf-8-sig');records=[]
    with TransformClient(base/'build-manifest.json') as client:
      for name in ['_draw6','_draw4']:
        key='form.SubForm::'+name+'(kairo.unity.ui.Graphics)->System.Void'
        start=source.index('int num = (num2 = (num3 = 0));',source.index('private void '+name+'('))
        response=client.request(ROOT/logical,source=source,offset=len(source[:start].encode('utf-16-le'))//2,max_candidates=4096,
          original_zero_chain_stores=4,expected_alias_types=['System.Int32'],original_local_count=len(original[key]['locals']),rebuilt_local_count=len(before[key]['locals']))
        patches=[p for p in response.get('patches',[]) if p['rule']=='restore-shared-root-zero' and p['after'].startswith('int num4;')]
        if not patches:raise ValueError((name,response))
        patch=patches[0];dest=ROOT/'analysis/variants'/('round5-shared-root-'+name);dest.mkdir(parents=True,exist_ok=True)
        candidate=dest/'SubForm.cs';candidate.write_text(apply_patch(source,patch),encoding='utf-8',newline='')
        mapping={s['path']:str(base/s['path']) for s in manifest['sources']};mapping[logical]=str(candidate)
        label='round5-shared-root-'+name;p=compile_frozen('Assembly-CSharp',label,manifest,mapping)
        record={'method_id':key,'rule':'restore-shared-root-zero','compile_pass':p.returncode==0,'patch':patch,'main_sources_modified':False,'states_promoted':False}
        folder=ROOT/'build'/('Assembly-CSharp-'+label)
        if p.returncode==0:
          actual={m['stable_id']:m for m in dump(folder/'Assembly-CSharp.dll',folder/'probe.json')['selected_methods']}
          eq={k for k,m in original.items() if 'raw_body_hex'in m and not differences(m,actual.get(k))[0]}
          meta=compare_paths(ROOT/'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll',folder/'Assembly-CSharp.dll',folder/'metadata.json')
          old=prefix(original[key],before[key]);new=prefix(original[key],actual[key])
          record.update(body_equal=key in eq,old_prefix=old,new_prefix=new,prefix_preserved=il_cells(original[key])[:old]==il_cells(actual[key])[:old],
            body_gains=sorted(eq-equal),body_losses=sorted(equal-eq),types_equal=meta['types_equal'],types_total=meta['types_total'],whole_structural_metadata_equal=meta['whole_structural_metadata_equal'],
            original_local_count=len(original[key]['locals']),before_local_count=len(before[key]['locals']),candidate_local_count=len(actual[key]['locals']),facets=differences(original[key],actual[key])[0])
        else:record['compiler_error']=(p.stdout+p.stderr).decode('utf-8',errors='replace')
        records.append(record)
        (ROOT/'reports/round5-shared-root-probes.json').write_text(json.dumps({'cases':records},indent=2)+'\n',encoding='utf-8',newline='\n')
        print(json.dumps({k:v for k,v in record.items() if k not in ('patch','compiler_error')}),flush=True)

if __name__=='__main__':main()
