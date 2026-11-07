"""Authenticate and publish the frozen C hypotheses; never execute their main()."""
import argparse
import hashlib
import importlib.util
import json
from pathlib import Path
import shutil
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def module(path, name):
    spec = importlib.util.spec_from_file_location(name, path)
    value = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(value)
    return value


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--family', choices=['roslyn', 'smcs'], required=True)
    p.add_argument('--bundle-sha256', required=True)
    p.add_argument('--index-sha256', required=True)
    args = p.parse_args()
    folder = ROOT / 'build/round11' / (args.family + '-hypotheses')
    bundle_path = folder / ('ready-for-game-probe.json' if args.family == 'roslyn' else 'final-probe-bundle.json')
    index_path = folder / ('ready-freeze.json' if args.family == 'roslyn' else 'frozen-index.json')
    assert sha(bundle_path) == args.bundle_sha256 and sha(index_path) == args.index_sha256
    index = json.loads(index_path.read_text(encoding='utf-8'))['files']
    files = index if isinstance(index, dict) else {str((folder / r['path']).relative_to(ROOT)).replace('\\', '/'): r['sha256'] for r in index}
    for name, digest in files.items():
        assert sha(ROOT / name) == digest, name
    original = json.loads(bundle_path.read_text(encoding='utf-8'))
    frozen = dict(original.get('frozen_evidence_sha256', {}))
    frozen.update(files)
    frozen.update({str(p.relative_to(ROOT)).replace('\\', '/'): sha(p) for p in (bundle_path, index_path)})
    deps = dict(original.get('dependency_sha256', {}))
    for name in list(frozen):
        if (ROOT / name).resolve().is_relative_to(ROOT / 'tools'):
            deps[name] = frozen.pop(name)
    historical = {p: frozen.pop(p) for p in list(frozen) if p.startswith('build/round10/')}
    assert all((ROOT / p).suffix == '.json' for p in historical)
    assert all(p.startswith('build/round11/') for p in frozen)
    if args.family == 'roslyn':
        producers = [('c123_calculate.py', 'round11_roslyn_c123_source.py'), ('c6_calculate.py', 'round11_roslyn_c6_source.py')]
        fixture_dirs = [folder / 'c123-fixtures', folder / 'c6-fixture']
        reports = ['c123-fixture-report.json', 'c6-fixture-report.json', 'c1-initial-fixture-report.json', 'c123-initial-contrast-report.json']
        note = 'NOTE.md'
    else:
        producers = [('calculator.py', 'round11_smcs_c45_source.py'), ('composition.py', 'round11_smcs_c4_composition_source.py')]
        fixture_dirs = []
        reports = ['fixture-results.json', 'composition-fixture-results.json', 'hypotheses-final.json', 'history-sha-gates.json']
        note = 'README.md'
    for source, name in producers:
        target = ROOT / 'tools' / name
        assert not target.exists()
        shutil.copyfile(folder / source, target)
        deps[target.relative_to(ROOT).as_posix()] = sha(target)
    evidence = {}
    for name in reports:
        target = ROOT / 'reports' / ('round11-c-' + args.family + '-' + name)
        assert not target.exists()
        shutil.copyfile(folder / name, target)
        evidence[target.relative_to(ROOT).as_posix()] = sha(target)
    own = ROOT / 'tools/fixtures' / ('round11-c-' + args.family)
    assert not own.exists()
    fixtures = [p for directory in fixture_dirs for p in directory.rglob('*.cs')]
    if args.family == 'smcs':
        fixtures = list(folder.glob('Fixture*.cs')) + [folder / 'Runner.cs']
    own_hashes = {}
    for source in fixtures:
        target = own / source.relative_to(folder)
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(source, target)
        own_hashes[target.relative_to(ROOT).as_posix()] = sha(target)
    target_note = ROOT / 'docs/notes' / ('round11-c-' + args.family + '-hypotheses.md')
    assert not target_note.exists()
    shutil.copyfile(folder / note, target_note)
    # Reproduce all pure APIs before admitting before/after payloads.
    if args.family == 'roslyn':
        c123 = module(ROOT / 'tools/round11_roslyn_c123_source.py', 'published_c123')
        c6 = module(ROOT / 'tools/round11_roslyn_c6_source.py', 'published_c6')
        apis = {'r11-c1': c123.transform_sound_protected_returns,
                'r11-c2': c123.transform_shader_array_alias_and_ordered_selector,
                'r11-c3': c123.transform_save_cancel_returns_and_log_scope,
                'r11-c6': c6.transform_seb_nonconstant_true}
    else:
        from round10_parented_source import load_with_parent
        compound = load_with_parent(ROOT / 'tools/round11_smcs_c4_composition_source.py',
                                    ROOT / 'tools/round11_smcs_c45_source.py', 'calculator')
        calculator = module(ROOT / 'tools/round11_smcs_c45_source.py', 'published_c45')
        apis = {'r11-c4': compound.transform_training_composed, 'r11-c5': calculator.restore_map_zero_subtractions}
    deps[Path(__file__).relative_to(ROOT).as_posix()] = sha(Path(__file__))
    for patch in original['patches']:
        front = patch['rule'][:6]
        assert apis[front](patch['before']) == patch['after'], front
        item = dict(patch)
        old_frozen = item.pop('frozen_evidence_sha256', {})
        item['historical_frozen_evidence_sha256'] = {k: v for k, v in old_frozen.items() if k.startswith('build/round10/')}
        item['frozen_evidence_sha256'] = {k: v for k, v in old_frozen.items() if k.startswith('build/round11/')}
        item.update(rebase_unique=True, source_span_units='UTF16', game_body_status='UNKNOWN')
        doc = dict(source_only=True, patches=[item], dependency_sha256=deps,
                   evidence_sha256=evidence, frozen_evidence_sha256=frozen,
                   historical_frozen_evidence_sha256=historical,
                   own_fixture_source_sha256=own_hashes, target_compilations=0,
                   public_pure_api_reproduction_exact=True)
        target = ROOT / 'reports' / ('round11-' + front[4:] + '-patches.json')
        assert not target.exists()
        target.write_text(json.dumps(doc, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'family': args.family, 'proposals': len(original['patches']),
                      'frozen_files_checked': len(files), 'target_compilations': 0}))


if __name__ == '__main__':
    main()
