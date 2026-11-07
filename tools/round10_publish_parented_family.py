"""Publish a child source composition with its explicitly pinned parent API.

Private producer/evidence paths become frozen evidence, never relaxed engine
dependencies. No target source, method state, compiler or assembly is changed.
"""
import argparse
import json
from pathlib import Path
import shutil
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--folder', required=True)
    parser.add_argument('--family', required=True)
    parser.add_argument('--bundle', default='source-patches.json')
    parser.add_argument('--calculator', required=True)
    parser.add_argument('--parent-calculator', required=True)
    parser.add_argument('--fixture', required=True)
    parser.add_argument('--expected-bundle-sha256', required=True)
    parser.add_argument('--expected-parent-sha256', required=True)
    parser.add_argument('--rule-prefix', required=True)
    parser.add_argument('--extra-producer', action='append', default=[])
    parser.add_argument('--expected-extra-sha256', action='append', default=[])
    parser.add_argument('--coverage', required=True)
    args = parser.parse_args()
    folder = (ROOT / args.folder).resolve()
    assert folder.is_relative_to(ROOT / 'build/round10')
    upstream = folder / args.bundle
    assert sha(upstream) == args.expected_bundle_sha256
    document = json.loads(upstream.read_text(encoding='utf-8'))
    parent = (folder / args.parent_calculator).resolve()
    assert parent.is_relative_to(ROOT / 'build/round10') and sha(parent) == args.expected_parent_sha256
    calculator = folder / args.calculator
    assert sha(calculator) == document['calculator_sha256']
    dependencies = {Path(__file__).relative_to(ROOT).as_posix(): sha(Path(__file__))}
    frozen = {upstream.relative_to(ROOT).as_posix(): sha(upstream)}
    mappings = {}

    def resolve(name, expected):
        candidates = [Path(name), ROOT / name, folder / name]
        paths = {p.resolve() for p in candidates if p.is_file() and sha(p) == expected}
        assert len(paths) == 1, (name, paths)
        return paths.pop()

    def pin(name, expected):
        path = resolve(name, expected)
        if path.is_relative_to(ROOT / 'tools'):
            dependencies[path.relative_to(ROOT).as_posix()] = expected
        else:
            assert path.is_relative_to(ROOT / 'build/round10'), name
            frozen[path.relative_to(ROOT).as_posix()] = expected

    producers = [(parent, 'parent_source.py'), (calculator, 'source.py')]
    for index, (name, expected) in enumerate(zip(args.extra_producer, args.expected_extra_sha256, strict=True), 1):
        source = (folder / name).resolve()
        assert source.is_relative_to(ROOT / 'build/round10') and sha(source) == expected
        producers.append((source, 'dependency%d_source.py' % index))
    for source, suffix in producers:
        public = ROOT / 'tools' / ('round10_' + args.family.replace('-', '_') + '_' + suffix)
        assert not public.exists()
        shutil.copyfile(source, public)
        assert sha(public) == sha(source)
        dependencies[public.relative_to(ROOT).as_posix()] = sha(public)
        frozen[source.relative_to(ROOT).as_posix()] = sha(source)
        mappings[source.relative_to(ROOT).as_posix()] = public.relative_to(ROOT).as_posix()
    for key in ('dependency_sha256', 'evidence_sha256', 'frozen_evidence_sha256'):
        for name, expected in document.get(key, {}).items():
            pin(name, expected)
    own = ROOT / 'tools/fixtures' / ('round10-' + args.family)
    own.mkdir(parents=True, exist_ok=False)
    fixture_sources = {}
    for source in sorted(folder.rglob('Fixture*.cs')):
        assert source.resolve().is_relative_to(folder)
        target = own / source.relative_to(folder)
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(source, target)
        fixture_sources[target.relative_to(ROOT).as_posix()] = sha(target)
        frozen[source.relative_to(ROOT).as_posix()] = sha(source)
    fixture = folder / args.fixture
    fixture_public = ROOT / 'reports' / ('round10-' + args.family + '-fixtures.json')
    assert not fixture_public.exists()
    shutil.copyfile(fixture, fixture_public)
    frozen[fixture.relative_to(ROOT).as_posix()] = sha(fixture)
    evidence = {fixture_public.relative_to(ROOT).as_posix(): sha(fixture_public)}
    for patch in document['patches']:
        for key in ('dependency_sha256', 'evidence_sha256', 'frozen_evidence_sha256'):
            for name, expected in patch.pop(key, {}).items():
                pin(name, expected)
        patch['upstream_rule'] = patch['rule']
        patch['rule'] = args.rule_prefix + '-' + patch['rule']
    target = ROOT / 'reports' / ('round10-' + args.family + '-patches.json')
    assert not target.exists()
    output = dict(operation='publish-parented-source-composition', source_only=True,
                  game_compilations=0, target_source_writes=False, method_state_writes=False,
                  patches=document['patches'], calculator_sha256=sha(calculator),
                  dependency_sha256=dependencies, evidence_sha256=evidence,
                  frozen_evidence_sha256=frozen, own_fixture_source_sha256=fixture_sources,
                  producer_module_mapping=mappings, body_status='UNKNOWN',
                  diagnostic_control_coverage=args.coverage)
    target.write_text(json.dumps(output, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'proposals': len(output['patches']), 'private_paths_pinned': len(frozen),
                      'parent_api_published': True, 'game_compilations': 0}))


if __name__ == '__main__':
    main()
