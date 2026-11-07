"""Measure independent source/IL control extraction; never promote method states.

Only source text enters the AST reader. Original CIL enters the CIL reader.
Plans and structured trees have separate denominators: lossless instruction
coverage is not a claim that structured recovery succeeded.
"""
import argparse
from collections import Counter
import csv
import hashlib
import importlib
import json
from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest()


def summarize(rows):
    stats = {'methods': len(rows)}
    for key in ('source_mapped', 'il_plan_complete', 'source_plan_complete',
                'paired_plan_complete', 'plans_equal', 'il_tree_complete',
                'source_tree_complete', 'paired_tree_complete', 'trees_equal',
                'tree_comparison_available', 'exact_instruction_coverage'):
        stats[key] = sum(bool(row.get(key)) for row in rows)
    for side in ('il', 'source'):
        reasons = Counter()
        for row in rows:
            reasons.update(set(r.get('code', str(r)) if isinstance(r, dict) else str(r)
                               for r in row.get(side + '_reasons', [])))
        stats[side + '_incomplete_reasons'] = dict(reasons.most_common())
    stats['plan_mismatch_groups'] = dict(Counter(
        row.get('first_plan_pair', 'UNKNOWN') for row in rows
        if row.get('paired_plan_complete') and not row.get('plans_equal')).most_common())
    return stats


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--snapshot', default='build/round11/baseline')
    parser.add_argument('--il-module-dir', default='tools')
    parser.add_argument('--ast-module-dir', default='tools')
    parser.add_argument('--label', default='stage01')
    parser.add_argument('--stage', type=int, default=1)
    args = parser.parse_args()
    base = ROOT / args.snapshot
    il_dir, ast_dir = ROOT / args.il_module_dir, ROOT / args.ast_module_dir
    sys.path[:0] = [str(il_dir), str(ast_dir)]
    public_il=(il_dir/'round9_control_il.py').exists()
    public_ast=(ast_dir/'round9_control_ast.py').exists()
    cil = importlib.import_module('round10_control_il')
    ast = importlib.import_module('round10_control_ast')
    cil.CONTROL_STAGE=args.stage
    inputs = [il_dir / name for name in (('round10_control_il.py','round9_control_cfg.py','round9_control_opcodes.json')
              if public_il else ('smcs_control_il.py', 'cfg_core.py', 'opcodes.json'))]
    inputs += [ast_dir / name for name in (('round10_control_ast.py', 'Round10ControlAst.cs')
               if public_ast else ('control_ast.py', 'Round9ControlAst.cs'))]
    if args.stage>=4:inputs += [il_dir/'round10_control_owners.py']
    inputs += [Path(__file__)]
    input_hashes = {str(p.relative_to(ROOT)): sha(p) for p in inputs}
    folder = ROOT / 'build/round11/control-validation' / args.label
    folder.mkdir(parents=True, exist_ok=True)
    manifest = read(base / 'Assembly-CSharp/build-manifest.json')
    # Frozen source paths, not live src, prevent concurrent source acceptance
    # from changing this corpus. Semantic references come from its manifest.
    for source in manifest['sources']:
        source['input_path'] = str((base / 'Assembly-CSharp' / source['path']).resolve())
    manifest_path = folder / 'source-manifest.json'
    manifest_path.write_text(json.dumps(manifest, indent=2) + '\n', encoding='utf-8')
    sources = {}
    with ast.Client(manifest_path, stage=args.stage) as client:
        for source in manifest['sources']:
            for ir in client.extract(ROOT / source['path']):
                key = ir['method_id']
                if key in sources:
                    raise ValueError('Ambiguous AST identity: ' + key)
                sources[key] = dict(ir, source=source['path'])
    (folder / 'source-irs.json').write_text(json.dumps(sources, indent=2) + '\n', encoding='utf-8')
    originals = read(base / 'Assembly-CSharp/original-methods.json')['selected_methods']
    with (base / 'methods.csv').open(encoding='utf-8-sig', newline='') as stream:
        ledger = {r['id']: r for r in csv.DictReader(stream)}
    verified = read(base / 'Assembly-CSharp/verification.json')['methods']
    states = {v['stable_id']: ledger[k]['estado'] for k, v in verified.items()}
    rows = []
    details = {}
    for method in originals:
        if not method.get('instructions'):
            continue
        key = method['stable_id']
        state = states[key]
        left = cil.extract_il(method)
        right = sources.get(key)
        row = {'id': key, 'state': state,
               'original_il_bytes': method['header']['code_size'],
               'source_mapped': right is not None,
               'il_reasons': left.get('reasons', []),
               'source_reasons': right.get('reasons', []) if right else [{'code': 'source-method-unmapped'}],
               'exact_instruction_coverage': left.get('coverage', {}).get('exact_instruction_coverage', False)}
        for side, ir in (('il', left), ('source', right or {})):
            row[side + '_plan_complete'] = bool(ir.get('emission_plan_complete', False))
            row[side + '_tree_complete'] = bool(ir.get('tree_complete', False))
        row['paired_plan_complete'] = row['il_plan_complete'] and row['source_plan_complete']
        row['paired_tree_complete'] = row['il_tree_complete'] and row['source_tree_complete']
        if row['paired_plan_complete']:
            lp, rp = cil.canonical_plan(left), cil.canonical_plan(right)
            row['plans_equal'] = lp == rp
            if lp != rp:
                n = next((n for n, pair in enumerate(zip(lp, rp)) if pair[0] != pair[1]), min(len(lp), len(rp)))
                row['first_plan_index'] = n
                row['first_plan_pair'] = json.dumps([lp[n] if n < len(lp) else None,
                                                     rp[n] if n < len(rp) else None], sort_keys=True)
        if row['paired_tree_complete']:
            if not hasattr(cil, 'canonical_tree'):
                row['tree_comparison'] = 'UNKNOWN: shared structured-tree signature unavailable'
            else:
                lt, rt = cil.canonical_tree(left), cil.canonical_tree(right)
                row['tree_comparison_available'] = lt is not None and rt is not None
                row['trees_equal'] = row['tree_comparison_available'] and lt == rt
                if not row['tree_comparison_available']:
                    row['tree_comparison'] = 'UNKNOWN: complete-tree claim has unsupported canonical projection'
                elif lt != rt:
                    row['tree_comparison'] = 'different'
        rows.append(row)
        details[key] = {'original_il_ir': left, 'source_ast_ir': right}
    details_path = folder / 'method-irs.json'
    details_path.write_text(json.dumps(details, indent=2) + '\n', encoding='utf-8')
    changed = [path for path, digest in input_hashes.items() if sha(ROOT / path) != digest]
    report = {'operation': 'independent-source-vs-original-CIL-control-validation',
              'stage': args.stage, 'snapshot': args.snapshot, 'ledger_sha256': sha(base / 'methods.csv'),
              'source_manifest_sha256': sha(manifest_path), 'extractor_inputs': input_hashes,
              'inputs_changed_during_run': changed,
              'valid_frozen_input_measurement': not changed,
              'own_IL_or_MDB_used_to_infer_AST_control': False,
              'method_irs': str(details_path.relative_to(ROOT)),
              'method_irs_sha256': sha(details_path),
              'matched': summarize([r for r in rows if r['state'] == 'matched']),
              'pending': summarize([r for r in rows if r['state'] != 'matched']),
              'all_bodies': summarize(rows), 'methods': rows}
    target = ROOT / 'reports' / ('round11-control-validation-' + args.label + '.json')
    target.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    summary={}
    for group in ('matched','pending'):
        summary[group]={k:v for k,v in report[group].items() if k not in ('plan_mismatch_groups','il_incomplete_reasons','source_incomplete_reasons')}
    print(json.dumps(dict(summary,valid_frozen_input_measurement=report['valid_frozen_input_measurement'])))


if __name__ == '__main__':
    main()
