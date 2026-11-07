"""Publish frozen, fixture-tested source proposals; never compile or adopt them."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil

ROOT = Path(__file__).resolve().parents[1]


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def read(path):
    return json.loads(path.read_text(encoding='utf-8-sig'))


def write(path, value):
    assert not path.exists(), path
    path.write_text(json.dumps(value, indent=2) + '\n', encoding='utf-8')


def publish(family):
    settings = {
        'smcs-empty-arms': ('autonomous-smcs-bne', 'fixture-validation/probe-bundle-engine.json', 'calculate.py', 'fixture-validation/fixture-results.json'),
        'roslyn-lifetimes': ('autonomous-roslyn-lifetimes', 'validation-v1/ready-for-game-probe.json', 'calculate.py', 'validation-v1/fixture-report.json'),
        'static-bool': ('static-bool-followup', 'ready-bundle.json', 'proposal.py', 'toy-validation/report.json'),
    }
    folder_name, bundle_name, calculator_name, fixture_name = settings[family]
    folder = ROOT / 'build/round10' / folder_name
    upstream = folder / bundle_name
    document = read(upstream)
    calculator = folder / calculator_name
    public_calculator = ROOT / 'tools' / ('round10_' + family.replace('-', '_') + '_source.py')
    assert not public_calculator.exists()
    shutil.copyfile(calculator, public_calculator)
    assert digest(public_calculator) == digest(calculator)
    dependencies = {public_calculator.relative_to(ROOT).as_posix(): digest(public_calculator),
                    Path(__file__).relative_to(ROOT).as_posix(): digest(Path(__file__))}
    dependencies.update(document.get('dependency_sha256', {}))
    frozen = dict(document.get('frozen_evidence_sha256', {}))
    frozen[upstream.relative_to(ROOT).as_posix()] = digest(upstream)
    frozen[calculator.relative_to(ROOT).as_posix()] = digest(calculator)
    fixture = folder / fixture_name
    frozen[fixture.relative_to(ROOT).as_posix()] = digest(fixture)
    public_fixture = ROOT / 'reports' / ('round10-' + family + '-fixtures.json')
    write(public_fixture, read(fixture))
    evidence = {public_fixture.relative_to(ROOT).as_posix(): digest(public_fixture)}
    # Own fixture sources are distributable and distinct from game/original artifacts.
    public_own = ROOT / 'tools/fixtures' / ('round10-' + family)
    public_own.mkdir(parents=True, exist_ok=False)
    own_sources = sorted(folder.glob('Fixture*.cs'))
    if not own_sources:
        own_sources = sorted((folder / 'validation-v1').glob('*/before/Fixture.cs'))
        own_sources += sorted((folder / 'validation-v1').glob('*/control/Fixture.cs'))
    copied = {}
    for index, source in enumerate(own_sources):
        target = public_own / (str(index) + '-' + source.name)
        shutil.copyfile(source, target)
        copied[target.relative_to(ROOT).as_posix()] = digest(target)
        frozen[source.relative_to(ROOT).as_posix()] = digest(source)
    if family == 'static-bool':
        before = (folder / 'before.cs.txt').read_bytes().decode('utf-8-sig')
        after = (folder / 'after.cs.txt').read_bytes().decode('utf-8-sig')
        assert hashlib.sha256(before.encode('utf-8')).hexdigest() == document['before_sha256']
        assert hashlib.sha256(after.encode('utf-8')).hexdigest() == document['after_sha256']
        patches = [dict(method_id=document['method_id'], logical_path=document['logical_source'],
                        start=0, end=len(before.encode('utf-16-le')) // 2,
                        before=before, after=after, rule='d10-static-bool-composition',
                        model_complete=None, source_only=True)]
        for name in ('before.cs.txt', 'after.cs.txt'):
            frozen[(folder / name).relative_to(ROOT).as_posix()] = digest(folder / name)
    else:
        patches = document['patches']
    for patch in patches:
        frozen.update(patch.get('frozen_evidence_sha256', {}))
        patch.pop('frozen_evidence_sha256', None)
        if family == 'roslyn-lifetimes':
            patch['upstream_rule'] = patch['rule']
            patch['rule'] = 'd10-roslyn-composition-' + patch['rule']
    for name, expected in dependencies.items():
        path = (ROOT / name).resolve()
        assert path.is_relative_to(ROOT / 'tools') and digest(path) == expected, name
    for name, expected in frozen.items():
        path = (ROOT / name).resolve()
        assert path.is_relative_to(ROOT / 'build/round10') and digest(path) == expected, name
    target = ROOT / 'reports' / ('round10-' + family + '-patches.json')
    output = dict(operation='publish-frozen-fixture-tested-source-proposals',
                  family=family, source_only=True, target_compilations=0,
                  target_source_writes=False, method_state_writes=False,
                  body_status='UNKNOWN', patches=patches,
                  calculator_sha256=digest(public_calculator), dependency_sha256=dependencies,
                  evidence_sha256=evidence, frozen_evidence_sha256=frozen,
                  own_fixture_source_sha256=copied,
                  fresh_model_and_real_compiler_required=True)
    write(target, output)
    print(json.dumps({'family': family, 'patches': len(patches), 'target': str(target.relative_to(ROOT)),
                      'game_compilations': 0, 'frozen_inputs_checked': len(frozen)}))


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('family', choices=('smcs-empty-arms', 'roslyn-lifetimes', 'static-bool'))
    publish(parser.parse_args().family)
