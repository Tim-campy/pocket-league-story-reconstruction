"""Source-only declaration calculation for every frozen pending smcs body."""
import json
from collections import Counter
from pathlib import Path
from round6_slot_model import SlotModelClient
from round6_slot_solver import lifetimes
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]


def main():
    base=ROOT/'build/round6/baseline/Assembly-CSharp'
    read=lambda p:json.loads(Path(p).read_text(encoding='utf-8-sig'))
    validation=read(ROOT/'reports/round6-slot-model-validation.json')
    original={m['stable_id']:m for m in read(base/'original-methods.json')['selected_methods']}
    rebuilt={m['stable_id']:m for m in read(base/'rebuilt-methods.json')['selected_methods']}
    types=read(base/'plain-local-types.json')
    pending=[r for r in validation['methods'] if not r['body_equal']]
    rows=[]
    with SlotModelClient(base/'build-manifest.json') as client:
        for number,row in enumerate(pending,1):
            identity=row['method_id']
            if 'source' not in row:
                result={'constraints':['generated_or_unmapped_source_method'],'patches':[],
                        'original_lifetimes':lifetimes(original[identity])}
            else:
                path=ROOT/row['source'];source=(base/row['source']).read_text(encoding='utf-8-sig')
                result=client.solve(path,source,identity,original[identity],types[identity],rebuilt_method=rebuilt[identity])
            result['approved_patch_rules']=[p['rule'] for p in result.pop('patches',[])]
            result.update(method_id=identity,source=row.get('source'),
                          model_complete=row['complete'],original_signature_equal=row['original_signature_equal'])
            rows.append(result)
            print(json.dumps({'processed':number,'total':len(pending),'method_id':identity,
                              'approved_patches':len(result['approved_patch_rules'])}),flush=True)
    result={'operation':'all-pending-source-only-declaration-calculation','candidate_compilations':0,
            'model_source_sha256':sha(ROOT/'tools/Round6SlotModel.cs'),
            'client_sha256':sha(ROOT/'tools/round6_slot_model.py'),
            'solver_sha256':sha(ROOT/'tools/round6_slot_solver.py'),
            'pending_methods':len(rows),'methods_with_original_lifetimes':sum('original_lifetimes' in r for r in rows),
            'methods_with_calculated_positions':sum(bool(r.get('calculated_declarations')) for r in rows),
            'methods_with_model_approved_patches':sum(bool(r['approved_patch_rules']) for r in rows),
            'model_approved_patches':sum(len(r['approved_patch_rules']) for r in rows),
            'constraints':dict(Counter(c for r in rows for c in r.get('constraints',[]))),
            'methods':rows}
    (ROOT/'reports/round6-slot-solver-inventory.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in result.items() if k!='methods'}))


if __name__=='__main__':main()
