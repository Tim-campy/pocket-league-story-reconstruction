"""Adopt measured source candidates only after fresh production-compiler proofs.

The official full verifier, never this script, writes method/type ledger states.
Original artifacts are read-only. No DLL bytes are changed or copied as output.
"""
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


def refresh_official_proof():
    """Reverify a documented whitespace cleanup; never recopy engine sources."""
    folder = ROOT / 'build/round4/adoption'
    output = ROOT / 'reports/round4-adoption.json'
    report = read(output)
    run = subprocess.run([sys.executable, str(ROOT / 'tools/verify'), '--all'], cwd=ROOT, capture_output=True)
    (folder / 'official-full-verify-after-cleanup.log').write_bytes(run.stdout + run.stderr)
    if run.returncode not in (0, 1):
        raise RuntimeError('Cleanup verification failed')
    checked = {}
    for a in ASSEMBLIES:
        receipt = read(ROOT / 'build' / ('verification-' + a + '.json'))
        if not receipt_is_fresh(ROOT, receipt) or receipt['regressions'] or receipt.get('type_regressions'):
            raise RuntimeError('Cleanup receipt is stale or regressive: ' + a)
        e = read(ROOT / 'reports' / ('round4-' + a + '-engine.json'))
        expected = {m['stable_id']: m for m in read(ROOT / 'build/round4/engine' / a / 'final-methods.json')['selected_methods']}
        binary = ROOT / receipt['build_artifact']
        actual = dump(binary, folder / (a + '-after-cleanup-methods.json'))
        changes = [k for k, m in expected.items() if differences(m, actual.get(k))[0]]
        if changes or set(expected) != set(actual):
            raise RuntimeError('Whitespace cleanup changed emitted bodies: ' + repr(changes))
        report['assemblies'][a].update(official_sha256=receipt['build_sha256'], counts=receipt['counts'], types_equal=receipt['types_equal'])
        checked[a] = {'body_changes_vs_engine': changes, 'official_sha256': receipt['build_sha256'], 'sources': receipt['sources']}
    report['post_adoption_cleanup'] = {'purpose': 'Remove trailing indentation from five generated blank lines',
                                      'official_full_verification': True, 'assemblies': checked}
    report['preflight_tool_failure'] = {'stage': 'before source adoption', 'exception': 'AttributeError: source-map string was not converted to Path',
                                      'fixed': True, 'source_or_ledger_changes_in_failed_attempt': False,
                                      'log': 'build/round4/adoption-first-attempt.log'}
    output.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'official_verified': True, 'matched_gains': report['matched_gains'], 'matched_losses': report['matched_losses'], 'cleanup_changed_bodies': 0}))


def main():
    if '--refresh-official-proof' in sys.argv:
        return refresh_official_proof()
    folder = ROOT / 'build/round4/adoption'
    folder.mkdir(parents=True, exist_ok=True)
    baseline_rows = list(csv.DictReader((ROOT / 'build/round4/baseline/methods.csv').open(encoding='utf-8-sig', newline='')))
    before = {r['id']: r for r in baseline_rows}
    report = {'operation': 'source-only-production-proof-and-official-verification', 'assemblies': {}, 'official_verified': False}
    candidates = {}
    for a in ASSEMBLIES:
        e = read(ROOT / 'reports' / ('round4-' + a + '-engine.json'))
        if not e.get('completed') or not e.get('adoption_eligible'):
            raise ValueError('Engine did not finish with zero regressions: ' + a)
        if sha(Path(e['final_binary'])) != e['final_sha256']:
            raise ValueError('Engine artifact changed: ' + a)
        engine_folder = ROOT / 'build/round4/engine' / a
        for logical, expected in e['source_hashes'].items():
            if sha(engine_folder / 'sources' / logical) != expected:
                raise ValueError('Candidate source changed: ' + logical)
            if (ROOT / logical).read_bytes() != (ROOT / 'build/round4/baseline' / a / logical).read_bytes():
                raise ValueError('Main source changed after frozen baseline: ' + logical)
        mapping = read(Path(e['source_map']))
        for logical, path in mapping.items():
            if not logical.startswith('src/' + a + '/') or not Path(path).resolve().is_relative_to(engine_folder.resolve()):
                raise ValueError('Source override outside target: ' + logical)
        candidates.update(mapping)
        label = 'round4-production-proof'
        cmd = [sys.executable, ROOT / 'tools' / ('build_kairo.py' if a == 'KairoLibrary' else 'build_csharp.py'),
               '--label', label, '--source-map', e['source_map']]
        if a == 'KairoLibrary':
            cmd += ['--compiler', ROOT / COMPILER_PATH]
        result = subprocess.run([str(p) for p in cmd], cwd=ROOT, capture_output=True)
        (folder / (a + '-production-build.log')).write_bytes(result.stdout + result.stderr)
        if result.returncode:
            raise RuntimeError('Production candidate did not compile: ' + a)
        binary = ROOT / 'build' / (a + '-' + label) / (a + '.dll')
        actual = dump(binary, folder / (a + '-production-methods.json'))
        expected = {m['stable_id']: m for m in read(engine_folder / 'final-methods.json')['selected_methods']}
        changed = [k for k, v in expected.items() if differences(v, actual.get(k))[0]]
        if changed or set(actual) != set(expected):
            raise ValueError('Production flags changed engine bodies: ' + repr(changed))
        metadata = compare_paths(ROOT / 'original/extracted/assets/bin/Data/Managed' / (a + '.dll'), binary,
                                 folder / (a + '-production-metadata.json'))
        if not metadata['whole_structural_metadata_equal']:
            raise ValueError('Production candidate metadata mismatch: ' + a)
        report['assemblies'][a] = {'production_body_changes_vs_engine': changed, 'source_map': e['source_map'],
                                  'source_files': len(mapping), 'production_sha256': sha(binary),
                                  'types_equal': metadata['types_equal'], 'body_gains': e['body_gains']}
    # Both assemblies have fresh independent proofs before any main source edit.
    for logical, source in candidates.items():
        backup = folder / 'pre-adoption' / logical
        backup.parent.mkdir(parents=True, exist_ok=True)
        backup.write_bytes((ROOT / logical).read_bytes())
        (ROOT / logical).write_bytes(Path(source).read_bytes())
    result = subprocess.run([sys.executable, str(ROOT / 'tools/verify'), '--all'], cwd=ROOT, capture_output=True)
    (folder / 'official-full-verify.log').write_bytes(result.stdout + result.stderr)
    if result.returncode not in (0, 1):
        raise RuntimeError('Official verifier failed unexpectedly; inspect official-full-verify.log')
    held_released = []
    for a in ASSEMBLIES:
        receipt = read(ROOT / 'build' / ('verification-' + a + '.json'))
        if not receipt_is_fresh(ROOT, receipt) or receipt['regressions'] or receipt.get('type_regressions'):
            raise RuntimeError('Official receipt is stale or has regressions: ' + a)
        for key in report['assemblies'][a]['body_gains']:
            proof = next((v for v in receipt['methods'].values() if v['stable_id'] == key), None)
            if not proof or not proof['body_equal']:
                raise RuntimeError('Engine gain did not survive official verifier: ' + key)
        gained = [k for k, v in receipt['methods'].items() if v['state'] == 'matched' and before[k]['estado'] != 'matched']
        losses = [k for k, v in receipt['methods'].items() if v['state'] != 'matched' and before[k]['estado'] == 'matched']
        base_receipt = read(ROOT / 'build/round4/baseline' / a / 'verification.json')
        held_released += [k for k in gained if base_receipt['methods'][k]['body_equal']]
        report['assemblies'][a].update(matched_gains=gained, matched_losses=losses, counts=receipt['counts'],
                                      official_sha256=receipt['build_sha256'], types_equal=receipt['types_equal'])
        if losses:
            raise RuntimeError('Official state regression: ' + repr(losses))
    report.update(official_verified=True, released_held=len(held_released), released_held_ids=held_released,
                  matched_gains=sum(len(v['matched_gains']) for v in report['assemblies'].values()),
                  matched_losses=sum(len(v['matched_losses']) for v in report['assemblies'].values()))
    output = ROOT / 'reports/round4-adoption.json'
    output.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'official_verified': True, 'matched_gains': report['matched_gains'], 'matched_losses': report['matched_losses']}))


if __name__ == '__main__':
    main()
