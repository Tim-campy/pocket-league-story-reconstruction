"""Report stable method ID additions/removals from existing inspector dumps.

This tool does not change recovered sources, binaries, ledgers or ownership rules.
"""
import argparse
import json
from pathlib import Path
from compare_kairo import body_cells, differences, sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--folder', type=Path, default=ROOT / 'build/KairoLibrary-roslyn-1.3.2')
    parser.add_argument('--output', type=Path, default=ROOT / 'reports/KairoLibrary-structural-baseline.json')
    args = parser.parse_args()
    left = args.folder / 'original-methods.json'
    right = args.folder / 'rebuilt-methods.json'
    original = {m['stable_id']: m for m in json.loads(left.read_text(encoding='utf-8-sig'))['selected_methods']}
    rebuilt = {m['stable_id']: m for m in json.loads(right.read_text(encoding='utf-8-sig'))['selected_methods']}
    missing, extra = sorted(set(original) - set(rebuilt)), sorted(set(rebuilt) - set(original))
    shifted = []
    for old_id in missing:
        for new_id in extra:
            a, b = original[old_id], rebuilt[new_id]
            if body_cells(a) is not None and body_cells(a) == body_cells(b):
                facets, _ = differences(a, b)
                shifted.append({'original_id': old_id, 'rebuilt_id': new_id,
                                'resolved_body_cells_equal': True, 'remaining_differences': facets,
                                'diagnosis': 'Compiler-generated lambda ordinal differs; source declaration order is the candidate cause.',
                                'cause_status': 'HYPOTHESIS pending source correction and full verifier'})
    hash_callers = []
    for method in rebuilt.values():
        for instruction in method.get('instructions', []):
            if '::ComputeStringHash' in str(instruction.get('operand')):
                original_method = original.get(method['stable_id'])
                hash_callers.append({'rebuilt_caller': method['stable_id'], 'call_offset': instruction['offset'],
                                     'original_same_id_present': original_method is not None,
                                     'original_has_hash_call': any('::ComputeStringHash' in str(i.get('operand'))
                                         for i in original_method.get('instructions', [])) if original_method else None,
                                     'diagnosis': 'Recovered string-switch generated hash helper absent from original.',
                                     'cause_status': 'Observed generated helper; original C# form UNKNOWN until source probe'})
    report = {'schema_version': 1, 'input_dumps': {str(left): sha(left), str(right): sha(right)},
              'missing_ids': missing, 'extra_ids': extra, 'same_body_different_identity': shifted,
              'hash_helper_callers': hash_callers, 'source_or_ledger_modified': False}
    args.output.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(report, indent=2))


if __name__ == '__main__':
    main()
