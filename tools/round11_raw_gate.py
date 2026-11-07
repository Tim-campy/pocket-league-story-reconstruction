"""Trigger read-only raw measurements at body-closure thresholds; never normalize."""
import argparse
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import sha
from round11_controller import load, validate_config

ROOT = Path(__file__).resolve().parents[1]


def due_events(cfg, measurement):
    recorded = cfg.get('raw_evaluations', {})
    result = []
    for assembly, limit in [('KairoLibrary', 0), ('Assembly-CSharp', 10)]:
        pending = measurement['assemblies'][assembly]['divergent_bodies']
        if pending <= limit and assembly not in recorded:
            result.append((assembly, pending))
    return result


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--snapshot', required=True, type=Path)
    p.add_argument('--measurement', required=True, type=Path)
    p.add_argument('--label', required=True)
    args = p.parse_args()
    config_path = ROOT / 'config/round11-protocol.json'
    cfg = load(config_path)
    validate_config(cfg)
    measured = load(args.measurement)
    events = due_events(cfg, measured)
    artifacts = load(args.snapshot / 'assemblies/artifacts.json')
    for assembly, pending in events:
        artifact = args.snapshot / artifacts[assembly]['path']
        if sha(artifact) != artifacts[assembly]['sha256']:
            raise ValueError('Frozen raw input changed: ' + assembly)
        folder = ROOT / 'build/round11/runs' / args.label / 'raw-trigger'
        folder.mkdir(parents=True, exist_ok=True)
        label = args.label + '-trigger'
        command = [sys.executable, '-X', 'utf8', 'tools/round11_raw_measurement.py',
                   '--assembly', assembly, '--label', label]
        result = subprocess.run(command, cwd=ROOT, capture_output=True)
        (folder / (assembly + '.log')).write_bytes(result.stdout + result.stderr)
        if result.returncode:
            raise RuntimeError('Raw trigger failed; retain adoption and repair this receipt before committing')
        prefix = 'round11-' + label + '-' + assembly + '-raw-'
        reports = sorted((ROOT / 'reports').glob(prefix + '*.json'))
        if not reports:
            raise ValueError('Raw measurement emitted no assembly-bound JSON receipt')
        cfg.setdefault('raw_evaluations', {})[assembly] = dict(
            status='measured', pending_bodies=pending, trigger_label=label,
            measurement=str(args.measurement), measurement_sha256=sha(args.measurement),
            snapshot=str(args.snapshot), artifacts_sha256=sha(args.snapshot / 'assemblies/artifacts.json'),
            frozen_recompiled_sha256=artifacts[assembly]['sha256'],
            reports={r.relative_to(ROOT).as_posix(): sha(r) for r in reports},
            original_modified=False, normalization_performed=False)
    config_path.write_text(json.dumps(cfg, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({'raw_triggers_evaluated': [x[0] for x in events], 'target_compilations': 0}))


if __name__ == '__main__':
    main()
