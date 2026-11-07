"""Register evidence-only A/B phases with zero measured method and IL gains."""
import argparse
import json
from pathlib import Path
from compare_kairo import sha
from round11_controller import BASE_COMMIT, d_rows, load, validate_config

ROOT = Path(__file__).resolve().parents[1]


def validate_zero_gain(before, after):
    for assembly in ('KairoLibrary', 'Assembly-CSharp'):
        a, b = before['assemblies'][assembly], after['assemblies'][assembly]
        for field in ('matched_methods', 'matched_original_il_bytes', 'total_methods',
                      'original_total_il_bytes', 'divergent_bodies'):
            if a[field] != b[field]:
                raise ValueError('A/B must have zero body/method gains: ' + assembly + '/' + field)


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--part', choices=['A', 'B'], required=True)
    p.add_argument('--evidence', required=True, type=Path)
    p.add_argument('--before', required=True, type=Path)
    p.add_argument('--after', required=True, type=Path)
    p.add_argument('--result', required=True)
    args = p.parse_args()
    config_path = ROOT / 'config/round11-protocol.json'
    cfg = load(config_path)
    validate_config(cfg)
    journal_path = ROOT / 'reports/round11-cycle-journal.json'
    journal = load(journal_path) if journal_path.exists() else {'cycles': []}
    if d_rows(journal):
        raise ValueError('A/B cannot be registered after D starts')
    before, after = load(args.before), load(args.after)
    if before['baseline_commit'] != BASE_COMMIT or after['baseline_commit'] != BASE_COMMIT:
        raise ValueError('Evidence measurement belongs to another round')
    validate_zero_gain(before, after)
    evidence = args.evidence.resolve()
    if not evidence.is_relative_to(ROOT) or evidence.suffix != '.json':
        raise ValueError('Evidence must be a local JSON receipt')
    load(evidence)
    record = dict(report=evidence.relative_to(ROOT).as_posix(), report_sha256=sha(evidence),
                  result=args.result, status='evaluated', methods_gained=0, il_bytes_gained=0,
                  before_measurement=str(args.before), before_sha256=sha(args.before),
                  after_measurement=str(args.after), after_sha256=sha(args.after))
    previous = cfg['part_evidence'].get(args.part)
    if previous and previous != record:
        raise ValueError('Existing A/B evidence differs; retain history before superseding')
    cfg['part_evidence'][args.part] = record
    cfg['required_parts'][args.part] = 'complete'
    baseline_path = ROOT / cfg['baseline_measurement']
    baseline = load(baseline_path)
    if baseline['baseline_commit'] != BASE_COMMIT:
        raise ValueError('Wrong baseline measurement')
    cfg['baseline_measurement_sha256'] = sha(baseline_path)
    cfg['phase'] = 'part_C' if all(cfg['required_parts'][p] == 'complete' for p in ('A', 'B')) else 'preparation'
    config_path.write_text(json.dumps(cfg, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(record))


if __name__ == '__main__':
    main()
