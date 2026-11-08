"""Record a handoff request for root's exhaustive raw comparison. No target build."""
import argparse
import json
from pathlib import Path
from compare_kairo import sha
from round12_controller import load, semantic_closed, validate_config

ROOT = Path(__file__).resolve().parents[1]


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--snapshot', required=True, type=Path)
    p.add_argument('--measurement', required=True, type=Path)
    p.add_argument('--label', required=True)
    args = p.parse_args()
    cfg = load(ROOT / 'config/round12-protocol.json')
    validate_config(cfg)
    measurement = load(args.measurement)
    due = semantic_closed(measurement)
    if due and cfg.get('part_a_raw_gate', {}).get('status') != 'PASS':
        artifacts_path = args.snapshot / 'assemblies/artifacts.json'
        artifacts = load(artifacts_path)
        dll = args.snapshot / artifacts['KairoLibrary']['path']
        if sha(dll) != artifacts['KairoLibrary']['sha256']:
            raise ValueError('Own frozen Kairo PE changed')
        request = dict(status='AWAITING_ROOT_RAW', snapshot=str(args.snapshot),
            snapshot_sha256=sha(args.snapshot / 'snapshot.json'),
            measurement=str(args.measurement), measurement_sha256=sha(args.measurement),
            artifacts_sha256=sha(artifacts_path), recompiled_sha256=sha(dll),
            semantic_divergent_bodies=0, held_methods=0,
            target_compilations=0, normalization_performed=False,
            owner='root', required_result='Every byte outside four proven nondeterministic fields equals original')
        path = ROOT / ('reports/round12-' + args.label + '-raw-handoff.json')
        path.write_text(json.dumps(request, indent=2) + '\n', encoding='utf-8')
        cfg['phase'] = 'awaiting_root_raw'
        cfg['part_a_raw_gate'] = dict(status='AWAITING_ROOT_RAW', request=path.relative_to(ROOT).as_posix(), sha256=sha(path))
        (ROOT / 'config/round12-protocol.json').write_text(json.dumps(cfg, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(dict(raw_handoff_due=due, target_compilations=0, raw_comparison_executed=False)))


if __name__ == '__main__':
    main()
