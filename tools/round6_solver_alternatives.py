"""Retain all calculated baseline alternatives across greedy source advances.

This module does not compile game candidates or adopt source. Rebasing only
constructs a source hypothesis; the engine must predict its signature again and
apply the ordinary strict-prefix, full-body and full-metadata checks.
"""
import hashlib
import json
from pathlib import Path

from compare_kairo import sha
from round6_slot_model import SlotModelClient

ROOT=Path(__file__).resolve().parents[1]
BASELINE_COMMIT='66eea9ddeb8e02a9f357f97f8e7c9dc60f54d6e9'


def priority(variant):
    rule=variant['rule']
    if 'original-zero-chain' in rule:return 0
    if 'all-calculated-blocks' in rule:return 1
    return 2


def rebase_variant(source, declaration_start, declaration_end, variant, *, method_id):
    """Replace only the caller's independently mapped matching method span.

    A stable ID must be supplied explicitly, not inferred from declaration text.
    Nothing here claims that the baseline variant preserves current progress.
    """
    if not method_id or variant.get('method_id')!=method_id:
        raise ValueError('Baseline alternative belongs to a different method')
    if not isinstance(declaration_start,int) or not isinstance(declaration_end,int):
        raise TypeError('Declaration coordinates must be integer UTF-16 offsets')
    raw=source.encode('utf-16-le')
    if not 0<=declaration_start<declaration_end<=len(raw)//2:
        raise ValueError('Declaration span is outside current source')
    try:
        before=raw[declaration_start*2:declaration_end*2].decode('utf-16-le')
        raw[:declaration_start*2].decode('utf-16-le')
        raw[declaration_end*2:].decode('utf-16-le')
    except UnicodeDecodeError as error:
        raise ValueError('Declaration span splits a UTF-16 surrogate pair') from error
    after=variant.get('after')
    if not isinstance(after,str) or not after.strip():
        raise ValueError('Missing calculated alternative source')
    return {'start':declaration_start,'end':declaration_end,'before':before,'after':after,
            'rule':variant['rule'],'distance':0,'method_id':method_id,
            'baseline_after_sha256':hashlib.sha256(after.encode('utf-8')).hexdigest(),
            'rebased_on_current_source_sha256':hashlib.sha256(source.encode('utf-8')).hexdigest(),
            'requires_fresh_model_prediction':True,'requires_strict_prefix_and_full_verification':True}


def main():
    read=lambda p:json.loads(Path(p).read_text(encoding='utf-8-sig'))
    base=ROOT/'build/round6/baseline/Assembly-CSharp'
    inventory=read(ROOT/'reports/round6-slot-solver-inventory.json')
    model_hash=sha(ROOT/'tools/Round6SlotModel.cs')
    client_hash=sha(ROOT/'tools/round6_slot_model.py')
    solver_hash=sha(ROOT/'tools/round6_slot_solver.py')
    if (inventory['model_source_sha256'],inventory['client_sha256'],inventory['solver_sha256'])!=(model_hash,client_hash,solver_hash):
        raise ValueError('Frozen solver inputs changed since their inventory')
    original={m['stable_id']:m for m in read(base/'original-methods.json')['selected_methods']}
    rebuilt={m['stable_id']:m for m in read(base/'rebuilt-methods.json')['selected_methods']}
    types=read(base/'plain-local-types.json')
    selected=[r for r in inventory['methods'] if r['approved_patch_rules']]
    variants=[]
    methods=[]
    with SlotModelClient(base/'build-manifest.json') as client:
        for number,row in enumerate(selected,1):
            identity=row['method_id'];logical=row['source'];source_path=base/logical
            source=source_path.read_text(encoding='utf-8-sig')
            result=client.solve(ROOT/logical,source,identity,original[identity],types[identity],rebuilt_method=rebuilt[identity])
            if len(result['patches'])!=len(row['approved_patch_rules']):
                raise ValueError('Calculated alternatives differ from frozen inventory: '+identity)
            for patch in result['patches']:
                variants.append({'method_id':identity,'logical_path':logical,
                    'sourcepath':str(source_path.relative_to(ROOT)).replace('\\','/'),
                    'baseline_source_sha256':sha(source_path),
                    'baseline_before':patch['before'],'after':patch['after'],
                    'rule':'solver-baseline-'+patch['rule'].removeprefix('solver-'),
                    'calculated_rule':patch['rule'],'baseline_start':patch['start'],'baseline_end':patch['end'],
                    'original_signature':types[identity],
                    'after_sha256':hashlib.sha256(patch['after'].encode('utf-8')).hexdigest()})
            methods.append({'method_id':identity,'alternatives':len(result['patches'])})
            print(json.dumps({'processed':number,'total':len(selected),'method_id':identity,'alternatives':len(result['patches'])}),flush=True)
    variants.sort(key=lambda row:(row['method_id'],priority(row),row['after_sha256']))
    result={'operation':'all-model-approved-frozen-baseline-source-alternatives',
        'baseline_commit':BASELINE_COMMIT,'candidate_compilations':0,'source_adoptions':0,'completed':True,
        'model_source_sha256':model_hash,'client_sha256':client_hash,'solver_sha256':solver_hash,
        'inventory_sha256':sha(ROOT/'reports/round6-slot-solver-inventory.json'),
        'methods':len(methods),'alternatives':len(variants),'method_inventory':methods,'variants':variants,
        'runtime_contract':'Rebase only after matching stable ID, then re-predict complete original signature; strict raw prefix, whole-body, no-regression and metadata checks remain mandatory.'}
    output=ROOT/'reports/round6-slot-baseline-alternatives.json'
    output.write_text(json.dumps(result,indent=2,ensure_ascii=True)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({k:v for k,v in result.items() if k not in ('variants','method_inventory')}))


if __name__=='__main__':main()
