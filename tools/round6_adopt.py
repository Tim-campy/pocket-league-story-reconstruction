"""Adopt source candidates after complete production proofs of both targets.

Only tools/verify writes ledger states. Importing this module has no side effects.
Both engine results are protected by their frozen source and artifact hashes.
"""
import argparse
import csv
import json
from pathlib import Path
import subprocess
import sys

from apply_il_rules import dump, read, receipt_is_fresh
from compare_kairo import differences, sha
from kairo_candidate import COMPILER_PATH
from metadata_compare import compare_paths

ROOT = Path(__file__).resolve().parents[1]
ASSEMBLIES = ('KairoLibrary', 'Assembly-CSharp')
BASE = ROOT / 'build/round6/baseline'
FOLDER = ROOT / 'build/round6/adoption'
OUTPUT = ROOT / 'reports/round6-adoption.json'


def project_path(path):
    value = Path(path)
    return value if value.is_absolute() else ROOT / value


def expected_methods(assembly):
    path = ROOT / 'build/round6/engine' / assembly / 'final-methods.json'
    return {m['stable_id']: m for m in read(path)['selected_methods']}


def assert_same_bodies(expected, actual, purpose):
    changes = [key for key, method in expected.items() if differences(method, actual.get(key))[0]]
    if changes or set(actual) != set(expected):
        raise ValueError(purpose + ': changed/missing bodies ' + repr(changes)
                         + '; additional ' + repr(sorted(set(actual) - set(expected))))
    return changes


def assert_production_selection(assembly, manifest):
    selected = read(ROOT / 'config/toolchain.json')['assemblies'][assembly]
    flags = [flag for flag in manifest['flags'] if not flag.startswith(('/out:', '-out:'))]
    refs = lambda items: [(str(project_path(r['path']).resolve()).lower(), r['sha256']) for r in items]
    if flags != selected['flags'] or refs(manifest['references']) != refs(selected['reference_files']):
        raise ValueError('Production flags/references differ from selection: ' + assembly)
    if manifest['compiler_sha256'] != read(BASE / assembly / 'build-manifest.json')['compiler_sha256']:
        raise ValueError('Production compiler differs from frozen baseline: ' + assembly)


def load_engine(assembly):
    engine = read(ROOT / 'reports' / ('round6-' + assembly + '-engine.json'))
    if not engine.get('completed') or not engine.get('adoption_eligible') or engine.get('body_losses'):
        raise ValueError(assembly + ' engine incomplete, ineligible or regressive')
    if sha(project_path(engine['final_binary'])) != engine['final_sha256']:
        raise ValueError('App engine binary changed')
    folder = (ROOT / 'build/round6/engine' / assembly).resolve()
    mapping = read(project_path(engine['source_map']))
    for logical, value in mapping.items():
        path = project_path(value).resolve()
        if not logical.startswith('src/' + assembly + '/') or not path.is_relative_to(folder / 'sources'):
            raise ValueError('Candidate source outside App engine source tree: ' + logical)
        if sha(path) != engine['source_hashes'][logical]:
            raise ValueError('App candidate source changed: ' + logical)
    return engine, mapping


def baseline_sources_unchanged(assembly):
    manifest = read(BASE / assembly / 'build-manifest.json')
    for row in manifest['sources']:
        logical = row['path']
        frozen = BASE / assembly / logical
        if sha(frozen) != row['sha256'] or sha(ROOT / logical) != row['sha256']:
            raise ValueError('Source changed after frozen baseline: ' + logical)


def run_official(log_name):
    run = subprocess.run([sys.executable, str(ROOT / 'tools/verify'), '--all'], cwd=ROOT, capture_output=True)
    (FOLDER / log_name).write_bytes(run.stdout + run.stderr)
    if run.returncode not in (0, 1):
        raise RuntimeError('Official verifier failed; inspect ' + log_name)


def collect_official(report, before, suffix=''):
    released = []
    for assembly in ASSEMBLIES:
        receipt_path = ROOT / 'build' / ('verification-' + assembly + '.json')
        receipt = read(receipt_path)
        if (not receipt_is_fresh(ROOT, receipt) or receipt.get('regressions')
                or receipt.get('body_regressions') or receipt.get('type_regressions')):
            raise RuntimeError('Official receipt stale or regressive: ' + assembly)
        actual = dump(ROOT / receipt['build_artifact'], FOLDER / (assembly + suffix + '-official-methods.json'))
        assert_same_bodies(expected_methods(assembly), actual, 'Official vs expected ' + assembly)
        gains = [key for key, proof in receipt['methods'].items()
                 if proof['state'] == 'matched' and before[key]['estado'] != 'matched']
        losses = [key for key, proof in receipt['methods'].items()
                  if proof['state'] != 'matched' and before[key]['estado'] == 'matched']
        if losses:
            raise RuntimeError('Official matched regressions: ' + repr(losses))
        for key in report['assemblies'][assembly]['body_gains']:
            proof = next((p for p in receipt['methods'].values() if p['stable_id'] == key), None)
            if not proof or not proof['body_equal']:
                raise RuntimeError('Engine body gain absent from official proof: ' + key)
        baseline_receipt = read(BASE / assembly / 'verification.json')
        released += [key for key in gains if baseline_receipt['methods'][key]['body_equal']]
        if receipt['types_equal'] != receipt['types_total']:
            raise RuntimeError('Official structural metadata differs: ' + assembly)
        report['assemblies'][assembly].update(
            matched_gains=gains, matched_losses=losses, counts=receipt['counts'],
            official_sha256=receipt['build_sha256'], official_receipt_sha256=sha(receipt_path),
            types_equal=receipt['types_equal'], types_total=receipt['types_total'])
    report.update(official_verified=True, released_held=len(released), released_held_ids=released,
                  matched_gains=sum(len(v['matched_gains']) for v in report['assemblies'].values()),
                  matched_losses=sum(len(v['matched_losses']) for v in report['assemblies'].values()))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--refresh-official-proof', action='store_true',
                        help='Recheck post-adoption source cleanup; never recopy engine sources')
    parser.add_argument('--incremental', action='store_true',
                        help='Adopt a later engine pass after checking the previous official source proof')
    args = parser.parse_args()
    if args.incremental and args.refresh_official_proof:
        raise ValueError('Incremental adoption and proof-only refresh are separate operations')
    FOLDER.mkdir(parents=True, exist_ok=True)
    before = {row['id']: row for row in csv.DictReader((BASE / 'methods.csv').open(encoding='utf-8-sig', newline=''))}
    engines, mapping = {}, {}
    for assembly in ASSEMBLIES:
        engines[assembly], candidates = load_engine(assembly)
        mapping.update(candidates)
    if args.refresh_official_proof:
        report = read(OUTPUT)
        run_official('official-full-verify-after-cleanup.log')
        collect_official(report, before, '-after-cleanup')
        report['post_adoption_cleanup'] = {'official_full_verification': True, 'body_changes_vs_expected': 0}
    else:
        report = {'operation': 'source-only-production-proof-and-official-verification',
                  'baseline_commit': '66eea9ddeb8e02a9f357f97f8e7c9dc60f54d6e9',
                  'official_verified': False, 'assemblies': {}, 'changed_sources': []}
        if args.incremental:
            previous = read(OUTPUT)
            if not previous.get('official_verified'):
                raise ValueError('No previous official adoption proof')
            for assembly in ASSEMBLIES:
                receipt = read(ROOT / ('build/verification-' + assembly + '.json'))
                if (not receipt_is_fresh(ROOT, receipt)
                        or receipt['build_sha256'] != previous['assemblies'][assembly]['official_sha256']
                        or any(receipt.get(k) for k in ('regressions', 'body_regressions', 'type_regressions'))):
                    raise ValueError('Main sources changed outside the previous official proof: ' + assembly)
            report['previous_adoption'] = previous
        for assembly in ASSEMBLIES:
            engine = engines[assembly]
            if not args.incremental:
                baseline_sources_unchanged(assembly)
            label = 'round6-production-proof'
            command = [sys.executable, str(ROOT / 'tools' / ('build_kairo.py' if assembly == 'KairoLibrary' else 'build_csharp.py')),
                       '--label', label]
            if assembly == 'KairoLibrary':
                command += ['--compiler', str(ROOT / COMPILER_PATH)]
            command += ['--source-map', str(project_path(engine['source_map']))]
            run = subprocess.run(command, cwd=ROOT, capture_output=True)
            (FOLDER / (assembly + '-production-build.log')).write_bytes(run.stdout + run.stderr)
            if run.returncode:
                raise RuntimeError('Production proof failed to compile: ' + assembly)
            output = ROOT / 'build' / (assembly + '-' + label)
            manifest = read(output / 'build-manifest.json')
            assert_production_selection(assembly, manifest)
            binary = output / (assembly + '.dll')
            actual = dump(binary, FOLDER / (assembly + '-production-methods.json'))
            assert_same_bodies(expected_methods(assembly), actual, 'Production proof ' + assembly)
            metadata = compare_paths(ROOT / 'original/extracted/assets/bin/Data/Managed' / (assembly + '.dll'),
                                     binary, FOLDER / (assembly + '-production-metadata.json'))
            if not metadata['whole_structural_metadata_equal']:
                raise ValueError('Production proof metadata differs: ' + assembly)
            report['assemblies'][assembly] = {
                'engine_executed': True,
                'production_body_changes_vs_expected': [], 'production_sha256': sha(binary),
                'production_manifest': str((output / 'build-manifest.json').relative_to(ROOT)),
                'production_manifest_sha256': sha(output / 'build-manifest.json'),
                'source_map': engine['source_map'],
                'body_gains': engine['body_gains'],
                'types_equal': metadata['types_equal']}
        # Only source is adopted, after full proofs of BOTH production assemblies.
        for logical, candidate in mapping.items():
            source = project_path(candidate)
            current = ROOT / logical
            if source.read_bytes() == current.read_bytes():
                continue
            backup = FOLDER / ('pre-incremental-adoption' if args.incremental else 'pre-adoption') / logical
            backup.parent.mkdir(parents=True, exist_ok=True)
            backup.write_bytes(current.read_bytes())
            current.write_bytes(source.read_bytes())
            report['changed_sources'].append(logical)
        run_official('official-full-verify.log')
        collect_official(report, before)
    OUTPUT.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'official_verified': report['official_verified'], 'matched_gains': report['matched_gains'],
                      'matched_losses': report['matched_losses']}))


if __name__ == '__main__':
    main()
