"""Classify extractor disagreements in a frozen validation (not game failures)."""
from collections import defaultdict
import argparse
import importlib
import json
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]


def first(left, right, path='tree'):
    if type(left) is not type(right):
        return path, left, right
    if isinstance(left, dict):
        if left.get('kind') != right.get('kind'):
            return path + '.kind', left.get('kind'), right.get('kind')
        if set(left) != set(right):
            return path + '.keys', sorted(left), sorted(right)
        for key in left:
            found = first(left[key], right[key], path + '.' + key)
            if found:
                return found
    elif isinstance(left, list):
        if len(left) != len(right):
            return path + '.length', len(left), len(right)
        for n, (a, b) in enumerate(zip(left, right)):
            found = first(a, b, path + '[]')
            if found:
                return found
    elif left != right:
        return path, left, right
    return None


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--label', required=True)
    p.add_argument('--il-module-dir', default='build/round9/control-il')
    args = p.parse_args()
    sys.path.insert(0, str(ROOT / args.il_module_dir))
    cil = importlib.import_module('smcs_control_il')
    proof = json.loads((ROOT / f'reports/round9-control-validation-{args.label}.json').read_text(encoding='utf-8'))
    data = json.loads((ROOT / proof['method_irs']).read_text(encoding='utf-8'))
    groups = defaultdict(list)
    for row in proof['methods']:
        if row['state'] != 'matched' or not row.get('paired_tree_complete'):
            continue
        pair = data[row['id']]
        left, right = cil.canonical_tree(pair['original_il_ir']), cil.canonical_tree(pair['source_ast_ir'])
        if left is None or right is None:
            key = 'unsupported-projection IL=' + str(left is None) + ' AST=' + str(right is None)
            detail = None
        else:
            detail = first(left, right)
            if detail is None:
                continue
            key = json.dumps(detail, sort_keys=True)
        groups[key].append({'id': row['id'], 'first_difference': detail,
                            'original_tree': left, 'source_tree': right})
    rows = [{'group': key, 'methods': len(values), 'examples': values[:3]}
            for key, values in sorted(groups.items(), key=lambda pair: -len(pair[1]))]
    target = ROOT / f'build/round9/control-validation/{args.label}/tree-errors.json'
    target.write_text(json.dumps(rows, indent=2) + '\n', encoding='utf-8')
    print(json.dumps([{'group': r['group'], 'methods': r['methods'],
                       'ids': [e['id'] for e in r['examples']]} for r in rows[:15]]))


if __name__ == '__main__':
    main()
