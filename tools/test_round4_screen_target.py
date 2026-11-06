"""Prove three target-only screens against real frozen smcs method bodies."""
import json
from pathlib import Path
import time

from compare_kairo import differences
from prepare_round4_screen import prepare_skeletons
from round3_metadata_candidates import compile_frozen, dump
from round4_transform import TransformClient

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
    with TransformClient(base / 'build-manifest.json') as client:
        for name, logical, signature in [
                ('GetIncompletePayments', 'src/Assembly-CSharp/system/billing/ui/StoreBase.cs',
                 'public List<Receipt> GetIncompletePayments()'),
                ('GetSum', 'src/Assembly-CSharp/game/Team.cs', 'public int GetSum('),
                ('ChMoveDirect', 'src/Assembly-CSharp/game/Player.cs', 'public bool ChMoveDirect(')]:
            source = (base / logical).read_bytes().decode('utf-8-sig')
            offset = len(source[:source.index(signature)].encode('utf-16-le')) // 2
            response = client.request(ROOT / logical, source=source, offset=offset, max_candidates=0)
            start = response['declaration_start']
            screened_source = client.screen_source(source, start)
            source_file = ROOT / 'build/round4/screen-target-sources' / logical
            source_file.parent.mkdir(parents=True, exist_ok=True)
            source_file.write_text(screened_source, encoding='utf-8', newline='')
            mapping = dict(skeletons)
            mapping[logical] = str(source_file)
            label = 'round4-screen-target-test-' + name
            started = time.perf_counter()
            compiled = compile_frozen(assembly, label, manifest, mapping)
            seconds = time.perf_counter() - started
            record = {'method_name': name, 'logical_source': logical,
                      'kept_declaration_start': start, 'compile_exit_code': compiled.returncode,
                      'compile_seconds': seconds, 'screening_only': True}
            if compiled.returncode == 0:
                folder = ROOT / 'build' / (assembly + '-' + label)
                rebuilt = {m['stable_id']: m for m in dump(folder / (assembly + '.dll'), folder / 'probe.json')['selected_methods']}
                ids = [key for key in baseline if '::' + name + '(' in key]
                assert ids
                record['methods'] = {key: {'body_equal': not differences(baseline[key], rebuilt.get(key))[0],
                                          'differences': differences(baseline[key], rebuilt.get(key))[0]} for key in ids}
                assert all(item['body_equal'] for item in record['methods'].values()), record
            else:
                record['compiler_log'] = (compiled.stdout + compiled.stderr).decode('utf-8', errors='replace')[-5000:]
            checks.append(record)
            report = {'main_sources_modified': False, 'ledger_states_adopted': False,
                      'policy': 'Target-only screening is eligible only after canonical target equality proof. Every advancing screened candidate requires a real full build and full non-regression/metadata checks.',
                      'checks': checks}
            (ROOT / 'reports/round4-screen-target-validation.json').write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
            print(json.dumps(record), flush=True)


if __name__ == '__main__':
    main()
