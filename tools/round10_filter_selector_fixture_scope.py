"""Keep publication bound to the frozen selector family, excluding a live study."""
import json
from pathlib import Path
import subprocess
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    private = ROOT / 'build/round10/autonomous-smcs-slots-next'
    index_path = private / 'frozen-index.json'
    assert sha(index_path) == '69898eca9f2a37f7c5b87455f53d9867734fbefc38cfca35862bfac3563b3908'
    index = json.loads(index_path.read_text(encoding='utf-8'))
    allowed = {Path(name).as_posix(): expected for name, expected in index['files_sha256'].items()}
    for name, expected in allowed.items():
        assert sha(private / name) == expected, name
    bundle_path = ROOT / 'reports/round10-smcs-named-selector-compositions-patches.json'
    bundle = json.loads(bundle_path.read_text(encoding='utf-8'))
    own = ROOT / 'tools/fixtures/round10-smcs-named-selector-compositions'
    quarantine = ROOT / 'build/round10/publication/quarantined-selector-fixture-copies'
    assert not quarantine.exists()
    excluded = []
    for name, expected in list(bundle['own_fixture_source_sha256'].items()):
        path = ROOT / name
        relative = path.relative_to(own)
        original = private / relative
        assert sha(path) == expected
        if relative.as_posix() in allowed:
            assert sha(original) == allowed[relative.as_posix()]
            continue
        assert relative.parts[0] == 'tplayer-goto-study'
        assert path.resolve().is_relative_to(own.resolve())
        tracked = subprocess.check_output(['git', 'ls-files', '--', name], cwd=ROOT)
        assert not tracked, 'Refuse moving a previously tracked user file'
        target = quarantine / relative
        assert target.resolve().is_relative_to(quarantine.resolve()) and not target.exists()
        target.parent.mkdir(parents=True, exist_ok=True)
        path.replace(target)
        assert sha(target) == expected
        excluded.append(dict(public_copy=name, preserved_copy=target.relative_to(ROOT).as_posix(), sha256=expected))
        del bundle['own_fixture_source_sha256'][name]
        for key in list(bundle['frozen_evidence_sha256']):
            if (ROOT / key).resolve() == original.resolve():
                del bundle['frozen_evidence_sha256'][key]
    assert len(excluded) == 8
    assert not any('tplayer-goto-study' in name for name in bundle['frozen_evidence_sha256'])
    report_path = ROOT / 'reports/round10-smcs-named-selector-frozen-scope.json'
    assert not report_path.exists()
    proof = dict(operation='exclude-unfrozen-own-study-from-selector-family-publication',
                 frozen_index=index_path.relative_to(ROOT).as_posix(), frozen_index_sha256=sha(index_path),
                 frozen_family_files_checked=len(allowed), excluded_live_study_copies=excluded,
                 private_originals_changed=False, target_source_writes=False, game_compilations=0,
                 method_state_writes=False, producer_sha256=sha(Path(__file__)))
    report_path.write_text(json.dumps(proof, indent=2) + '\n', encoding='utf-8')
    bundle['dependency_sha256'][Path(__file__).relative_to(ROOT).as_posix()] = sha(Path(__file__))
    bundle['evidence_sha256'][report_path.relative_to(ROOT).as_posix()] = sha(report_path)
    bundle['publication_scope_filter'] = report_path.relative_to(ROOT).as_posix()
    bundle_path.write_text(json.dumps(bundle, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'frozen_files_checked': len(allowed), 'unfrozen_study_copies_excluded': len(excluded),
                      'private_study_unchanged': True, 'target_source_writes': False}))


if __name__ == '__main__':
    main()
