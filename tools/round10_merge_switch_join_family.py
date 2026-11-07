"""Group four fixture-tested proposals sharing the same measured first pair."""
import json
from pathlib import Path
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    measurement_path = ROOT / 'reports/round10-cycle13-after-measurement.json'
    measurement = json.loads(measurement_path.read_text(encoding='utf-8'))
    group = next(g for g in measurement['assemblies']['Assembly-CSharp']['alignment']['opcode_histogram']
                 if g['pair'] == 'brtrue => stloc.s')
    dependencies = {Path(__file__).relative_to(ROOT).as_posix(): sha(Path(__file__))}
    evidence = {measurement_path.relative_to(ROOT).as_posix(): sha(measurement_path)}
    frozen = {}
    patches = []
    for name in ('smcs-switch-selector-compositions', 'smcs-switch-layout-compositions'):
        path = ROOT / 'reports' / ('round10-' + name + '-patches.json')
        document = json.loads(path.read_text(encoding='utf-8'))
        dependencies.update(document.get('dependency_sha256', {}))
        evidence.update(document.get('evidence_sha256', {}))
        evidence[path.relative_to(ROOT).as_posix()] = sha(path)
        frozen.update(document.get('frozen_evidence_sha256', {}))
        for original in document['patches']:
            patch = dict(original)
            patch['upstream_rule'] = patch['rule']
            patch['rule'] = 'd10-smcs-switch-join-' + patch['rule']
            assert (patch['before'], patch['after']) == (original['before'], original['after'])
            patches.append(patch)
    assert len(patches) == group['methods'] == 4
    assert {p['method_id'] for p in patches} == set(group['method_ids'])
    assert group['original_il_bytes'] == 8527
    for name, expected in dependencies.items():
        assert (ROOT / name).resolve().is_relative_to(ROOT / 'tools') and sha(ROOT / name) == expected
    for name, expected in evidence.items():
        assert (ROOT / name).resolve().is_relative_to(ROOT / 'reports') and sha(ROOT / name) == expected
    for name, expected in frozen.items():
        assert (ROOT / name).resolve().is_relative_to(ROOT / 'build/round10') and sha(ROOT / name) == expected
    output = dict(operation='register-same-first-pair-fixture-tested-switch-family',
                  source_only=True, target_source_writes=False, game_compilations=0,
                  source_text_changed=False, method_state_writes=False,
                  pair=group['pair'], selected_methods=4, selected_original_il_bytes=8527,
                  rationale='Original numeric dispatch uses named selectors and local joins; recovered switches add selector stores. Each composition has independent fixtures and a fresh engine model remains mandatory.',
                  patches=patches, dependency_sha256=dependencies, evidence_sha256=evidence,
                  frozen_evidence_sha256=frozen)
    target = ROOT / 'reports/round10-smcs-switch-join-family-patches.json'
    assert not target.exists()
    target.write_text(json.dumps(output, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'same_pair': group['pair'], 'proposals': 4, 'potential_original_il_bytes': 8527,
                      'source_text_changed': False, 'target_compilations': 0}))


if __name__ == '__main__':
    main()
