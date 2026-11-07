"""Bind local producer/fixture evidence to an engine-compatible source bundle."""
import argparse
import json
from pathlib import Path
from compare_kairo import sha
from round10_controller import stop_reason

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--folder', required=True)
    parser.add_argument('--family', required=True)
    parser.add_argument('--expected-bundle-sha256', required=True)
    parser.add_argument('--bundle', default='source-patches.json')
    parser.add_argument('--calculator', default='proposal.py')
    args = parser.parse_args()
    cfg = json.loads((ROOT / 'config/round10-protocol.json').read_text(encoding='utf-8'))
    journal = json.loads((ROOT / 'reports/round10-cycle-journal.json').read_text(encoding='utf-8'))
    assert cfg['phase'] != 'stopped' and stop_reason(cfg, journal) is None
    folder = (ROOT / args.folder).resolve()
    assert folder.is_relative_to(ROOT / 'build/round10')
    upstream = folder / args.bundle
    assert sha(upstream) == args.expected_bundle_sha256
    document = json.loads(upstream.read_text(encoding='utf-8'))
    for name, expected in document.get('deps_sha256', {}).items():
        path = folder / name
        assert path.resolve().is_relative_to(folder) and sha(path) == expected, name
    if document.get('fixture_report'):
        assert sha(folder / document['fixture_report']) == document['fixture_report_sha256']
    assert sha(folder / args.calculator) == document['calculator_sha256']
    frozen = dict(document.get('frozen_evidence_sha256', {}))
    for path in folder.rglob('*'):
        if path.is_file() and '__pycache__' not in path.parts:
            assert path.resolve().is_relative_to(folder)
            frozen[path.relative_to(ROOT).as_posix()] = sha(path)
    document['frozen_evidence_sha256'] = frozen
    document.setdefault('dependency_sha256', {})[Path(__file__).relative_to(ROOT).as_posix()] = sha(Path(__file__))
    target = ROOT / 'build/round10/publication' / (args.family + '-engine.json')
    target.parent.mkdir(parents=True, exist_ok=True)
    assert not target.exists()
    target.write_text(json.dumps(document, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'private_files_bound': len(frozen), 'game_compilations': 0,
                      'private_frozen_inputs_modified': False, 'target': target.relative_to(ROOT).as_posix()}))


if __name__ == '__main__':
    main()
