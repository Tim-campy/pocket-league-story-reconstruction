"""Bind journal and raw gate to a new immutable PE snapshot after full verification.

No target compilation or method-state edit occurs here. Never replaces a frozen
snapshot/report; measured gains must stay exactly the same.
"""
import argparse
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import sha
from round12_controller import load, semantic_closed, verify_raw_gate

ROOT = Path(__file__).resolve().parents[1]
METRICS = ('matched_methods', 'matched_original_il_bytes', 'divergent_bodies',
           'divergent_original_il_bytes', 'equal_body_held_methods')


def reconcile(row, before, old_after, new_after, path, digest):
    for assembly in old_after['assemblies']:
        old = old_after['assemblies'][assembly]
        new = new_after['assemblies'][assembly]
        if any(old[k] != new[k] for k in METRICS):
            raise ValueError('Post-verification semantic measurement changed: ' + assembly)
        gain = dict(methods_gained=new['matched_methods'] - before['assemblies'][assembly]['matched_methods'],
                    il_bytes_gained=new['matched_original_il_bytes'] - before['assemblies'][assembly]['matched_original_il_bytes'])
        if gain != row['gains'][assembly]:
            raise ValueError('Post-verification changes measured cycle gains')
    result = dict(row)
    result['postverify_provenance'] = [*row.get('postverify_provenance', []),
        dict(previous_after_measurement=row['after_measurement'], previous_after_sha256=row['after_sha256'],
             new_after_measurement=path, new_after_sha256=digest, gains_unchanged=True)]
    result.update(after_measurement=path, after_sha256=digest)
    return result


def run(args):
    subprocess.run([sys.executable, '-X', 'utf8', *map(str, args)], cwd=ROOT, check=True)


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--label', required=True)
    args = p.parse_args()
    if (ROOT / 'build/round12/ROOT_RAW_WRITE_HOLD').exists():
        raise RuntimeError('Root raw files are being edited; preserve journal and retry commit only after freeze confirmation')
    journal_path = ROOT / 'reports/round12-cycle-journal.json'
    journal = load(journal_path)
    if not journal['cycles'] or (args.label != journal['cycles'][-1]['label'] and args.label != 'final-report'):
        raise ValueError('Only the current measured cycle/final reporting may refresh provenance')
    row = journal['cycles'][-1]
    before = load(ROOT / row['before_measurement'])
    old = load(ROOT / row['after_measurement'])
    if sha(ROOT / row['after_measurement']) != row['after_sha256']:
        raise ValueError('Existing journal measurement changed')
    number = 1
    while True:
        label = args.label + '-postverify%02d' % number
        snapshot = Path('build/round12/runs') / args.label / 'commit' / ('postverify%02d' % number)
        measurement = Path('reports/round12-' + label + '-measurement.json')
        if not (ROOT / snapshot).exists() and not (ROOT / measurement).exists():
            break
        number += 1
    run(['tools/round12_freeze.py', '--destination', snapshot])
    run(['tools/round12_measure.py', '--snapshot', snapshot, '--label', label])
    current = load(ROOT / measurement)
    updated = reconcile(row, before, old, current, measurement.as_posix(), sha(ROOT / measurement))
    cfg_path = ROOT / 'config/round12-protocol.json'
    cfg = load(cfg_path)
    gate = cfg.get('part_a_raw_gate', {})
    if gate.get('status') == 'PASS':
        previous = load(ROOT / gate['report'])
        if sha(ROOT / gate['report']) != gate['sha256']:
            raise ValueError('Previously approved raw report changed')
        producer = ROOT / 'tools/round12_raw_compare.py'
        if sha(producer) != previous['producer_sha256']:
            raise ValueError('Approved raw comparator changed; root must revalidate it')
        run(['tools/round12_raw_compare.py', '--snapshot', snapshot, '--assembly', 'KairoLibrary', '--label', label])
        report_path = Path('reports/round12-' + label + '-KairoLibrary-raw.json')
        report = load(ROOT / report_path)
        new_gate = dict(gate, status=report['status'], report=report_path.as_posix(),
            sha256=sha(ROOT / report_path), snapshot=snapshot.as_posix(),
            recompiled_sha256=report['recompiled_sha256'],
            initial_root_pass=gate.get('initial_root_pass', dict(report=gate['report'], sha256=gate['sha256'])))
        candidate_cfg = dict(cfg, part_a_raw_gate=new_gate)
        verify_raw_gate(candidate_cfg, current)  # Literal SHA and every semantic/raw count; no normalization.
        cfg['part_a_raw_gate'] = new_gate
        cfg_path.write_text(json.dumps(cfg, indent=2) + '\n', encoding='utf-8')
    elif semantic_closed(current):
        run(['tools/round12_raw_gate.py', '--snapshot', snapshot, '--measurement', measurement, '--label', label])
    from round12_app_raw_trigger import measure_if_due
    app_raw = measure_if_due(current, snapshot, label)
    if app_raw is not None:
        cfg.setdefault('app_raw_measurements', []).append(app_raw)
        cfg_path.write_text(json.dumps(cfg, indent=2) + '\n', encoding='utf-8')
    journal['cycles'][-1] = updated
    journal_path.write_text(json.dumps(journal, indent=2, ensure_ascii=False) + '\n', encoding='utf-8')
    print(json.dumps(dict(postverify_snapshot=str(snapshot), measurement=str(measurement),
                         cycle_gains_unchanged=True, target_compilations=0)))


if __name__ == '__main__':
    main()
