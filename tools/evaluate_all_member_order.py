"""Measure isolated ordering candidates against the oracle and a frozen baseline.

Writes reports only; never changes sources, generated states or compiler output.
"""
import argparse
import json
from pathlib import Path
import subprocess
from compare_kairo import differences, sha
from metadata_compare import compare_paths
from plan_all_member_order import LABELS

ROOT=Path(__file__).resolve().parents[1]


def dump(path,output):
    with output.open('wb') as file:
        subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(path),'--structure'],stdout=file,check=True)
    return json.loads(output.read_text(encoding='utf-8-sig'))


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--assembly',choices=list(LABELS),required=True)
    parser.add_argument('--label',required=True)
    parser.add_argument('--baseline-label')
    args=parser.parse_args()
    baseline=ROOT/'build'/f'{args.assembly}-{args.baseline_label or LABELS[args.assembly]}'
    candidate=ROOT/'build'/f'{args.assembly}-{args.label}'
    oracle_path=ROOT/'original/extracted/assets/bin/Data/Managed'/f'{args.assembly}.dll'
    oracle=dump(oracle_path,candidate/'original-methods.json')
    old=dump(baseline/f'{args.assembly}.dll',candidate/'baseline-methods.json')
    new=dump(candidate/f'{args.assembly}.dll',candidate/'rebuilt-methods.json')
    old_index={m['stable_id']:m for m in old['selected_methods']}
    new_index={m['stable_id']:m for m in new['selected_methods']}
    original_index={m['stable_id']:m for m in oracle['selected_methods']}
    old_equal=set();new_equal=set()
    for key,method in original_index.items():
        if 'raw_body_hex' not in method:
            continue
        if not differences(method,old_index.get(key))[0]:
            old_equal.add(key)
        if not differences(method,new_index.get(key))[0]:
            new_equal.add(key)
    old_metadata=compare_paths(oracle_path,baseline/f'{args.assembly}.dll',candidate/'baseline-metadata-comparison.json')
    new_metadata=compare_paths(oracle_path,candidate/f'{args.assembly}.dll',candidate/'metadata-comparison.json')
    original_types=[t['name'] for t in oracle['types']]
    new_types=[t['name'] for t in new['types']]
    old_types=[t['name'] for t in old['types']]
    summary_keys=('types_equal','declarations_proved','type_order_equal','type_difference_counts_nonexclusive')
    report={'assembly':args.assembly,'candidate_label':args.label,'baseline_label':args.baseline_label or LABELS[args.assembly],
        'baseline_body_equal':len(old_equal),'candidate_body_equal':len(new_equal),
        'body_gains':sorted(new_equal-old_equal),'body_losses':sorted(old_equal-new_equal),
        'original_body_count':sum('raw_body_hex' in m for m in oracle['selected_methods']),
        'original_method_count':len(original_index),'candidate_method_count':len(new_index),
        'missing_original_methods':sorted(set(original_index)-set(new_index)),
        'extra_candidate_methods':sorted(set(new_index)-set(original_index)),
        'baseline_metadata':{key:old_metadata[key] for key in summary_keys},
        'candidate_metadata':{key:new_metadata[key] for key in summary_keys},
        'baseline_positionally_equal_type_count':sum(a==b for a,b in zip(original_types,old_types)),
        'candidate_positionally_equal_type_count':sum(a==b for a,b in zip(original_types,new_types)),
        'original_type_order':original_types,'candidate_type_order':new_types,
        'candidate_sha256':sha(candidate/f'{args.assembly}.dll'),'baseline_sha256':sha(baseline/f'{args.assembly}.dll'),
        'applied_to_main':False,'ledger_updated':False}
    output=ROOT/'reports'/f'{args.assembly}-{args.label}-evaluation.json'
    output.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({key:(len(value) if isinstance(value,list) else value) for key,value in report.items()
                     if key not in ('original_type_order','candidate_type_order')}))


if __name__=='__main__':
    main()
