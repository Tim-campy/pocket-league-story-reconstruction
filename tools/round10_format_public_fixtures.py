"""Trim public own-fixture trivia, preserving compiled private evidence."""
import argparse
import json
from pathlib import Path
from compare_kairo import sha
from round10_clean_added_whitespace import clean_source_lines
from round10_smcs_empty_arms_source import vals

ROOT = Path(__file__).resolve().parents[1]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--family', required=True)
    args = parser.parse_args()
    bundle_path = ROOT / 'reports' / ('round10-' + args.family + '-patches.json')
    bundle = json.loads(bundle_path.read_text(encoding='utf-8'))
    root = (ROOT / 'tools/fixtures' / ('round10-' + args.family)).resolve()
    planned = []
    for name, expected in bundle['own_fixture_source_sha256'].items():
        path = (ROOT / name).resolve()
        assert path.is_relative_to(root) and path.suffix == '.cs' and sha(path) == expected
        before = path.read_bytes().decode('utf-8')
        lines = [n for n, line in enumerate(before.splitlines(), 1) if line.rstrip(' \t') != line]
        if not lines:
            continue
        after, changed, blank, nonblank, refused = clean_source_lines(before, lines)
        assert not refused and changed == lines and vals(before) == vals(after)
        planned.append((path, after, dict(path=name, before_sha256=expected,
                       lines=changed, blank_lines=blank, nonblank_trivia_lines=nonblank,
                       lexical_tokens_unchanged=True, literals_unchanged=True,
                       line_endings_preserved=True)))
    assert planned, 'No whitespace adjustment is needed'
    proof_path = ROOT / 'reports' / ('round10-' + args.family + '-format-review.json')
    assert not proof_path.exists()
    rows = []
    for path, after, row in planned:
        path.write_bytes(after.encode('utf-8'))
        row['after_sha256'] = sha(path)
        bundle['own_fixture_source_sha256'][row['path']] = sha(path)
        rows.append(row)
    producer = Path(__file__).relative_to(ROOT).as_posix()
    dependencies = {name: sha(ROOT / name) for name in (
        producer, 'tools/round10_clean_added_whitespace.py', 'tools/round10_smcs_empty_arms_source.py')}
    proof = dict(operation='public-own-fixture-trivia-only', rows=rows,
                 private_compiled_fixture_evidence_unchanged=True,
                 target_source_writes=False, game_compilations=0,
                 method_state_writes=False, dependency_sha256=dependencies)
    proof_path.write_text(json.dumps(proof, indent=2) + '\n', encoding='utf-8')
    bundle['evidence_sha256'][proof_path.relative_to(ROOT).as_posix()] = sha(proof_path)
    bundle['dependency_sha256'].update(dependencies)
    bundle['publication_format_adjustment'] = proof_path.relative_to(ROOT).as_posix()
    bundle_path.write_text(json.dumps(bundle, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(proof))


if __name__ == '__main__':
    main()
