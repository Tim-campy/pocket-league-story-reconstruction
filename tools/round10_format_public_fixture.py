"""Remove three public fixture trailing spaces; retain the private proof."""
import json
from pathlib import Path
from compare_kairo import sha
from round10_clean_added_whitespace import clean_source_lines
from round10_smcs_empty_arms_source import vals

ROOT = Path(__file__).resolve().parents[1]


def main():
    bundle_path = ROOT / 'reports/round10-smcs-guard-slot-compositions-patches.json'
    document = json.loads(bundle_path.read_text(encoding='utf-8'))
    name = 'tools/fixtures/round10-smcs-guard-slot-compositions/toy-validation/sp/candidate/Fixture.cs'
    path = ROOT / name
    before_hash = sha(path)
    assert before_hash == document['own_fixture_source_sha256'][name]
    before = path.read_bytes().decode('utf-8')
    after, changed, blank, nonblank, refused = clean_source_lines(before, [17, 21, 28])
    assert changed == [17, 21, 28] and blank == 0 and nonblank == 3 and not refused
    assert vals(before) == vals(after)
    path.write_bytes(after.encode('utf-8'))
    proof = dict(operation='public-own-fixture-trivia-only', source=name,
                 before_sha256=before_hash, after_sha256=sha(path),
                 trailing_space_lines_trimmed=changed, nonblank_lines_edited=3,
                 lexical_tokens_unchanged=True,
                 literal_or_interpolation_edits=0, line_endings_preserved=True,
                 private_fixture_evidence_unchanged=True, target_source_writes=False,
                 target_compilations=0, method_state_writes=False,
                 cleaner_sha256=sha(ROOT / 'tools/round10_clean_added_whitespace.py'),
                 producer_sha256=sha(Path(__file__)))
    report = ROOT / 'reports/round10-smcs-guard-slot-fixture-format.json'
    assert not report.exists()
    report.write_text(json.dumps(proof, indent=2) + '\n', encoding='utf-8')
    document['own_fixture_source_sha256'][name] = sha(path)
    document['evidence_sha256'][report.relative_to(ROOT).as_posix()] = sha(report)
    document['dependency_sha256'][Path(__file__).relative_to(ROOT).as_posix()] = sha(Path(__file__))
    document['publication_format_adjustment'] = report.relative_to(ROOT).as_posix()
    bundle_path.write_text(json.dumps(document, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(proof))


if __name__ == '__main__':
    main()
