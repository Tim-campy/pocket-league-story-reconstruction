"""Preserve the failed formatting gate before a fresh full commit retry."""
import json
from pathlib import Path
import shutil
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    source = ROOT / 'build/round10/runs/cycle07/commit'
    target = ROOT / 'build/round10/recovery/cycle07-format-rejected'
    assert source.resolve().is_relative_to(ROOT / 'build/round10')
    assert target.resolve().is_relative_to(ROOT / 'build/round10') and not target.exists()
    shutil.copytree(source, target)
    captured = {p.relative_to(ROOT).as_posix(): sha(p) for p in target.rglob('*') if p.is_file()}
    cfg_path = ROOT / 'config/round10-protocol.json'
    cfg = json.loads(cfg_path.read_text(encoding='utf-8'))
    cfg.setdefault('preparation_notes', []).append(
        'Cycle07 adopted HttpObject._connect with zero regressions, then its commit stopped at Git whitespace '
        'on four blank fixture lines. Failed commit logs were frozen before a trivia-only public-copy repair. '
        'The retry reruns full official verification and source/index proof; it does not rerun the cycle or add a journal gain. '
        'An earlier publication formatter preflight also rejected an incorrect blank-line assumption before writing; '
        'the subsequent literal-protected, token-identical cleanup is recorded separately.')
    cfg_path.write_text(json.dumps(cfg, indent=2) + '\n', encoding='utf-8')
    public = ROOT / 'reports/round10-cycle07-format-recovery.json'
    assert not public.exists()
    proof = dict(operation='preserve-cycle07-format-failure-before-commit-retry',
                 failed_stage='Git staged whitespace gate; four blank fixture lines',
                 failed_commit_logs_sha256=captured,
                 publication_repair='reports/round10-smcs-switch-selector-compositions-format-review.json',
                 publication_repair_sha256=sha(ROOT / 'reports/round10-smcs-switch-selector-compositions-format-review.json'),
                 full_verification_required_again=True, engine_cycle_repeated=False,
                 journal_gain_duplicated=False, target_source_writes=False,
                 method_state_writes=False, producer_sha256=sha(Path(__file__)))
    public.write_text(json.dumps(proof, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'failed_logs_preserved': len(captured), 'method_states_modified': False}))


if __name__ == '__main__':
    main()
