"""Measure screened/full target identity on frozen smcs sources; never adopt it."""
import json
from pathlib import Path
import subprocess
import time

from compare_kairo import differences
from prepare_round4_screen import prepare_skeletons
from round3_metadata_candidates import compile_frozen, dump

ROOT = Path(__file__).resolve().parents[1]


def read(path):
    return json.loads(Path(path).read_text(encoding='utf-8-sig'))


def main():
    assembly = 'Assembly-CSharp'
    base = ROOT / 'build/round4/baseline' / assembly
    manifest = read(base / 'build-manifest.json')
    skeletons = prepare_skeletons(base / 'build-manifest.json', ROOT / 'build/round4/screen-sources' / assembly)
    baseline = {m['stable_id']: m for m in read(base / 'rebuilt-methods.json')['selected_methods']}
    checks = []
    for name, logical in [
            ('StoreBase', 'src/Assembly-CSharp/system/billing/ui/StoreBase.cs'),
            ('Team', 'src/Assembly-CSharp/game/Team.cs'),
            ('Player', 'src/Assembly-CSharp/game/Player.cs')]:
        mapping = dict(skeletons)
        mapping[logical] = str(base / logical)
        label = 'round4-screen-test-' + name
        started = time.perf_counter()
        compiled = compile_frozen(assembly, label, manifest, mapping)
        seconds = time.perf_counter() - started
        record = {'real_source_file': logical, 'compile_exit_code': compiled.returncode,
                  'compile_seconds': seconds, 'screening_only': True}
        if compiled.returncode == 0:
            folder = ROOT / 'build' / (assembly + '-' + label)
            rebuilt = {m['stable_id']: m for m in dump(folder / (assembly + '.dll'), folder / 'probe.json')['selected_methods']}
            # Source ledger includes nested/compiler-generated bodies in this file.
            import csv
            with (ROOT / 'config/methods.csv').open(encoding='utf-8-sig', newline='') as stream:
                ids = [row['id'].split('::', 1)[1].replace('+', '/') for row in csv.DictReader(stream)
                       if row['assembly'] == assembly and row['fonte'] == logical]
            # Stable IDs preserve pointer-plus type syntax; match the ledger's
            # known identities first, then include declaring nested target types.
            target_types = {key.split('::', 1)[0] for key in ids}
            selected = [key for key, method in baseline.items()
                        if 'raw_body_hex' in method and key.split('::', 1)[0] in target_types]
            changed = {key: differences(baseline[key], rebuilt.get(key))[0]
                       for key in selected if differences(baseline[key], rebuilt.get(key))[0]}
            intended_name = {'StoreBase': 'GetIncompletePayments', 'Team': 'GetSum',
                             'Player': 'ChMoveDirect'}[name]
            intended = [key for key in selected if '::' + intended_name + '(' in key]
            record.update(target_bodies=len(selected), target_bodies_equal=len(selected) - len(changed),
                          target_bodies_changed=changed,
                          intended_methods={key: {'body_equal': key not in changed,
                                                 'differences': changed.get(key, [])} for key in intended},
                          eligibility='TARGET_SPECIFIC_PROOF_REQUIRED' if changed else 'ALL_TARGET_FILE_BODIES_PROVED_FOR_THIS_BASELINE')
        else:
            record['compiler_log'] = (compiled.stdout + compiled.stderr).decode('utf-8', errors='replace')[-5000:]
        checks.append(record)
        report = {'main_sources_modified': False, 'ledger_states_adopted': False,
                  'compiler': manifest['compiler'], 'checks': checks,
                  'policy': 'Each method needs target canonical equality against a real full build before screening; every advancing screened candidate requires a complete real build and full non-regression verification before acceptance.'}
        (ROOT / 'reports/round4-screen-validation.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
        print(json.dumps(record), flush=True)


if __name__ == '__main__':
    main()
