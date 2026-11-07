"""Bind a frozen source-only proposal to its published calculator and fixtures."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil

ROOT = Path(__file__).resolve().parents[1]


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def publish(args):
    folder = (ROOT / args.folder).resolve()
    assert folder.is_relative_to(ROOT / 'build/round10')
    source_bundle = folder / args.bundle
    document = json.loads(source_bundle.read_text(encoding='utf-8-sig'))
    calculator = folder / args.calculator
    public_calculator = ROOT / 'tools' / ('round10_' + args.family.replace('-', '_') + '_source.py')
    assert not public_calculator.exists()
    shutil.copyfile(calculator, public_calculator)
    assert sha(calculator) == sha(public_calculator)
    dependencies = dict(document.get('dependency_sha256', {}))
    dependencies.update({public_calculator.relative_to(ROOT).as_posix(): sha(public_calculator),
                         Path(__file__).relative_to(ROOT).as_posix(): sha(Path(__file__))})
    frozen = dict(document.get('frozen_evidence_sha256', {}))
    frozen[source_bundle.relative_to(ROOT).as_posix()] = sha(source_bundle)
    frozen[calculator.relative_to(ROOT).as_posix()] = sha(calculator)
    evidence = dict(document.get('evidence_sha256', {}))
    fixture = folder / args.fixture
    fixture_output = ROOT / 'reports' / ('round10-' + args.family + '-fixtures.json')
    assert not fixture_output.exists()
    shutil.copyfile(fixture, fixture_output)
    evidence[fixture_output.relative_to(ROOT).as_posix()] = sha(fixture_output)
    frozen[fixture.relative_to(ROOT).as_posix()] = sha(fixture)
    own_folder = ROOT / 'tools/fixtures' / ('round10-' + args.family)
    own_folder.mkdir(parents=True, exist_ok=False)
    copied = {}
    # Authored fixture source only: no game snapshots or binary artifacts.
    for source in sorted(folder.rglob('Fixture*.cs')):
        target = own_folder / source.relative_to(folder)
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(source, target)
        copied[target.relative_to(ROOT).as_posix()] = sha(target)
        frozen[source.relative_to(ROOT).as_posix()] = sha(source)
    assert document['patches']
    for patch in document['patches']:
        assert patch.get('before') and patch.get('after') and patch.get('method_id') and patch.get('logical_path')
        dependencies.update(patch.get('dependency_sha256', {}))
        frozen.update(patch.pop('frozen_evidence_sha256', {}))
        if args.rule_prefix:
            patch['upstream_rule'] = patch['rule']
            patch['rule'] = args.rule_prefix + '-' + patch['rule']
    for name, expected in dependencies.items():
        path = (ROOT / name).resolve()
        assert path.is_relative_to(ROOT / 'tools') and sha(path) == expected, name
    for name, expected in evidence.items():
        path = (ROOT / name).resolve()
        assert path.is_relative_to(ROOT / 'reports') and path.suffix == '.json' and sha(path) == expected, name
    for name, expected in frozen.items():
        path = (ROOT / name).resolve()
        assert path.is_relative_to(ROOT / 'build/round10') and sha(path) == expected, name
    output = dict(operation='publish-frozen-source-only-fixture-tested-proposals',
                  source_only=True, target_compilations=0, target_source_writes=False,
                  body_status='UNKNOWN', method_state_writes=False,
                  patches=document['patches'], calculator_sha256=sha(public_calculator),
                  dependency_sha256=dependencies, evidence_sha256=evidence,
                  frozen_evidence_sha256=frozen, own_fixture_source_sha256=copied,
                  fresh_model_and_real_compiler_required=True)
    target = ROOT / 'reports' / ('round10-' + args.family + '-patches.json')
    assert not target.exists()
    target.write_text(json.dumps(output, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'bundle': target.relative_to(ROOT).as_posix(),
                      'proposals': len(output['patches']), 'game_compilations': 0,
                      'frozen_inputs_checked': len(frozen)}))


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--folder', required=True)
    parser.add_argument('--family', required=True)
    parser.add_argument('--bundle', required=True)
    parser.add_argument('--calculator', required=True)
    parser.add_argument('--fixture', required=True)
    parser.add_argument('--rule-prefix', default='')
    publish(parser.parse_args())
