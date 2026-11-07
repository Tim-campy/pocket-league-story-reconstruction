"""Authenticate a closed private family by its complete frozen file index."""
import argparse
import json
from pathlib import Path
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--folder', required=True)
    parser.add_argument('--family', required=True)
    parser.add_argument('--calculator', required=True)
    parser.add_argument('--bundle', default='probe-bundle.json')
    parser.add_argument('--index', default='frozen-index.json')
    parser.add_argument('--expected-index-sha256', required=True)
    parser.add_argument('--expected-bundle-sha256', required=True)
    args = parser.parse_args()
    folder = (ROOT / args.folder).resolve()
    assert folder.is_relative_to(ROOT / 'build/round10')
    upstream = folder / args.bundle
    index_path = folder / args.index
    assert sha(upstream) == args.expected_bundle_sha256
    assert sha(index_path) == args.expected_index_sha256
    document = json.loads(upstream.read_text(encoding='utf-8'))
    index = json.loads(index_path.read_text(encoding='utf-8'))
    files = index['files_sha256']
    frozen = dict(document.get('frozen_evidence_sha256', {}))
    for name, expected in files.items():
        path = (folder / name).resolve()
        assert path.is_relative_to(folder) and sha(path) == expected, name
        frozen[path.relative_to(ROOT).as_posix()] = expected
    for path in (upstream, index_path):
        frozen[path.relative_to(ROOT).as_posix()] = sha(path)
    expected = document.get('calculator_sha256', document.get('source_calculator_sha256'))
    assert expected and sha(folder / args.calculator) == expected
    document['calculator_sha256'] = expected
    document['frozen_evidence_sha256'] = frozen
    document.setdefault('dependency_sha256', {})[Path(__file__).relative_to(ROOT).as_posix()] = sha(Path(__file__))
    target = ROOT / 'build/round10/publication' / (args.family + '-indexed-engine.json')
    target.parent.mkdir(parents=True, exist_ok=True)
    assert not target.exists()
    target.write_text(json.dumps(document, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'files_checked': len(files), 'private_sources_modified': False,
                      'target_compilations': 0, 'target': target.relative_to(ROOT).as_posix()}))


if __name__ == '__main__':
    main()
