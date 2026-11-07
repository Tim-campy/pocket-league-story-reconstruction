"""Bind the completed private guard fixture/evidence set without changing it."""
import json
from pathlib import Path
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    folder = ROOT / 'build/round10/autonomous-smcs-guards'
    upstream = folder / 'source-patches.json'
    document = json.loads(upstream.read_text(encoding='utf-8'))
    assert sha(upstream) == 'e5b8ccbc6171a70a56d6a4dce3dc65bd357c6092186dc675e57637483c9a0c95'
    for name, expected in document['deps_sha256'].items():
        assert sha(folder / name) == expected, name
    assert sha(folder / document['fixture_report']) == document['fixture_report_sha256']
    assert sha(folder / 'proposal.py') == document['calculator_sha256']
    frozen = {path.relative_to(ROOT).as_posix(): sha(path)
              for path in folder.rglob('*') if path.is_file() and '__pycache__' not in path.parts}
    document['frozen_evidence_sha256'] = frozen
    document['dependency_sha256'] = {Path(__file__).relative_to(ROOT).as_posix(): sha(Path(__file__))}
    target = ROOT / 'build/round10/publication/smcs-guards-engine.json'
    target.parent.mkdir(parents=True, exist_ok=True)
    assert not target.exists()
    target.write_text(json.dumps(document, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'private_files_bound': len(frozen), 'game_compilations': 0,
                      'private_frozen_inputs_modified': False}))


if __name__ == '__main__':
    main()
