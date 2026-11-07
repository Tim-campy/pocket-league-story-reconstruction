"""Preserve a rejected publication attempt before selecting the real R2 inputs."""
import json
from pathlib import Path
import subprocess
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def main():
    source = ROOT / 'tools/round10_roslyn_late_tail_stack_alias_source.py'
    old = ROOT / 'build/round10/autonomous-roslyn-slots-next/calculate.py'
    selected = ROOT / 'build/round10/autonomous-roslyn-slots-next/variant2/calculate.py'
    assert sha(source) == sha(old)
    assert sha(selected) == '82edf98998675df71f65449a1428a6c4766a0ab50605f7c92d5882d743439f0b'
    assert not subprocess.check_output(['git', 'ls-files', '--', str(source.relative_to(ROOT))], cwd=ROOT)
    target = ROOT / 'build/round10/publication/roslyn770-rejected-producer.py'
    assert source.resolve().is_relative_to(ROOT / 'tools')
    assert target.resolve().is_relative_to(ROOT / 'build/round10') and not target.exists()
    target.parent.mkdir(parents=True, exist_ok=True)
    source.replace(target)
    fixture = ROOT / 'build/round10/autonomous-roslyn-slots-next/variant2/validation-v1/fixture-report.json'
    assert fixture.exists()
    report = dict(operation='recover-publication-with-wrong-relative-fixture-path',
                  initial_failure='FileNotFoundError before fixture/bundle publication; wrong R1 calculator copy already existed',
                  rejected_producer=target.relative_to(ROOT).as_posix(), rejected_producer_sha256=sha(target),
                  selected_producer=selected.relative_to(ROOT).as_posix(), selected_producer_sha256=sha(selected),
                  selected_fixture=fixture.relative_to(ROOT).as_posix(), selected_fixture_sha256=sha(fixture),
                  private_frozen_inputs_modified=False, game_compilations=0, target_source_writes=False,
                  method_state_writes=False, producer_sha256=sha(Path(__file__)))
    output = ROOT / 'reports/round10-roslyn770-publication-recovery.json'
    assert not output.exists()
    output.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'rejected_copy_preserved': True, 'selected': selected.relative_to(ROOT).as_posix(),
                      'target_compilations': 0}))


if __name__ == '__main__':
    main()
