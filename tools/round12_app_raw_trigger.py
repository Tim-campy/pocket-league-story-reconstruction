"""Measure App PE literally when ten or fewer methods remain; never adopt it."""
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import sha

ROOT = Path(__file__).resolve().parents[1]


def due(measurement):
    app = measurement['assemblies']['Assembly-CSharp']
    return app['divergent_bodies'] + app['equal_body_held_methods'] <= 10


def measure_if_due(measurement, snapshot, label):
    if not due(measurement):
        return None
    raw_label = label + '-app-threshold'
    subprocess.run([sys.executable, '-X', 'utf8', 'tools/round12_raw_compare.py',
                    '--snapshot', str(snapshot), '--assembly', 'Assembly-CSharp',
                    '--label', raw_label, '--allow-pending-measurement'], cwd=ROOT, check=True)
    path = Path('reports/round12-' + raw_label + '-Assembly-CSharp-raw.json')
    report = json.loads((ROOT / path).read_text(encoding='utf-8'))
    return dict(report=path.as_posix(), sha256=sha(ROOT / path),
                snapshot=str(snapshot), recompiled_sha256=report['recompiled_sha256'],
                status=report['status'], measurement_only=True,
                outside_exceptions_differing_bytes=report['outside_exceptions_differing_bytes'],
                target_compilations=0, states_modified=False)
