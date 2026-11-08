"""Round-7 read-only method/IL measurements, reusing the strict body oracle."""
import argparse
import csv
import json
from pathlib import Path

from compare_kairo import sha
from round4_measure import LABELS, measure
from round6_alignment import make_histogram

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--snapshot', type=Path)
    parser.add_argument('--label', default='baseline')
    args = parser.parse_args()
    snapshot = args.snapshot.resolve() if args.snapshot else None
    ledger_path = snapshot / 'methods.csv' if snapshot else ROOT / 'config/methods.csv'
    with ledger_path.open(encoding='utf-8-sig', newline='') as stream:
        ledger = {r['id']: r for r in csv.DictReader(stream)}
    result = {'schema_version': 1, 'operation': 'read-only-first-IL-divergence-measurement',
              'baseline_commit': 'a075dcc8ca7b08e2cb9fcdd0cbcb5ce1fe8cd236',
              'metric': 'Only metadata-token identities resolved. All branch targets, operand bytes, instruction widths, constants and nops retained. Whole-body acceptance also requires header, locals and EH equality.',
              'snapshot': str(snapshot.relative_to(ROOT)) if snapshot else None,
              'pair_grouping': 'Exact opcode and resolved operand; opcode projection is diagnostic only.',
              'ledger_sha256': sha(ledger_path), 'assemblies': {}}
    for assembly, label in LABELS.items():
        folder = snapshot / assembly if snapshot else ROOT / 'build' / (assembly + '-' + label)
        receipt = folder / 'verification.json' if snapshot else ROOT / 'build' / ('verification-' + assembly + '.json')
        measured = measure(assembly, folder, receipt, ledger)
        left = json.loads((folder / 'original-methods.json').read_text(encoding='utf-8-sig'))
        right = json.loads((folder / 'rebuilt-methods.json').read_text(encoding='utf-8-sig'))
        pending_ids = [m['stable_id'] for m in measured['methods']]
        measured['alignment'] = make_histogram(left, right, pending_ids)
        for field in ('exact_histogram', 'opcode_histogram'):
            measured['alignment'][field].sort(key=lambda r: (-r['original_il_bytes'], -r['methods'], r['pair']))
        originals = {m['stable_id']: m for m in left['selected_methods']}
        rebuilt = {m['stable_id']: m for m in right['selected_methods']}
        measured['pending_local_signatures_equal'] = sum(
            originals[k].get('locals') == rebuilt.get(k, {}).get('locals') for k in pending_ids)
        measured['pending_local_signatures_different'] = len(pending_ids) - measured['pending_local_signatures_equal']
        from compare_kairo import differences
        receipt_data=json.loads(receipt.read_text(encoding='utf-8-sig'))
        pending_stable={v['stable_id'] for k,v in receipt_data['methods'].items() if ledger[k]['estado']!='matched' and v.get('body_present')}
        held=[mid for mid in pending_stable if mid in originals and mid in rebuilt and not differences(originals[mid],rebuilt[mid])[0]]
        measured['equal_body_held_methods']=len(held)
        measured['equal_body_held_method_ids']=sorted(held)
        result['assemblies'][assembly] = measured
    target = ROOT / 'reports' / ('round12-' + args.label + '-measurement.json')
    target.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8', newline='\n')
    fields = ('assembly', 'id', 'original_il_bytes', 'rebuilt_il_bytes', 'first_il_difference_offset', 'first_pair_opcodes', 'first_pair_exact', 'body_differences')
    with target.with_suffix('.csv').open('w', encoding='utf-8', newline='') as stream:
        writer = csv.DictWriter(stream, fieldnames=fields, lineterminator='\n'); writer.writeheader()
        for a, data in result['assemblies'].items():
            for row in data['methods']:
                item = {k: row[k] for k in fields if k != 'assembly'}
                item['assembly'] = a
                item['body_differences'] = ';'.join(item['body_differences'])
                writer.writerow(item)
    roslyn = result['assemblies']['KairoLibrary']
    top = {'scope': 'Roslyn/KairoLibrary only', 'measurement': target.relative_to(ROOT).as_posix(),
           'measurement_sha256': sha(target), 'pending_bodies': roslyn['divergent_bodies'],
           'pending_original_il_bytes': roslyn['divergent_original_il_bytes'],
           'exact_top10': roslyn['first_pair_exact_histogram'][:10],
           'opcode_top10': roslyn['first_pair_opcode_histogram'][:10]}
    (ROOT / 'reports' / ('round12-' + args.label + '-Roslyn-top10.json')).write_text(json.dumps(top, indent=2) + '\n', encoding='utf-8', newline='\n')
    print(json.dumps({a: {k: v[k] for k in ('matched_methods', 'matched_original_il_bytes', 'divergent_bodies', 'divergent_original_il_bytes', 'size_distribution')} for a, v in result['assemblies'].items()}))


if __name__ == '__main__':
    main()
