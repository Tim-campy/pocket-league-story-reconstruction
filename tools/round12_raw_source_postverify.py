"""Fresh PE/source provenance for a consolidated metadata-source commit, without changing A11."""
import argparse
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import sha
from round12_raw_source_trial import read, write, invariants, snapshot_provenance

ROOT = Path(__file__).resolve().parents[1]


def run(args):
    subprocess.run([sys.executable, '-X', 'utf8', *map(str, args)], cwd=ROOT, check=True)


def main():
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument('--label', required=True)
    args = p.parse_args()
    journal_path = ROOT/'reports/round12-raw-source-journal.json'
    journal = read(journal_path)
    if [r['label'] for r in journal['steps']] != ['r%02d'%i for i in range(1,7)]:
        raise ValueError('All six measured source experiments must precede consolidation')
    cfg_path = ROOT/'config/round12-protocol.json'
    cfg = read(cfg_path)
    approval = cfg['root_raw_acceptance']
    if sha(ROOT/approval['report']) != approval['sha256']:
        raise ValueError('Root independent raw acceptance changed')
    root_acceptance = read(ROOT/approval['report'])
    if root_acceptance['status'] != 'PASS' or sha(ROOT/'tools/round12_raw_compare.py') != root_acceptance['producer_sha256']:
        raise ValueError('Only the root-approved literal comparator may renew acceptance')
    invariants()
    number = len(journal.get('commit_provenance', [])) + 1
    label = args.label+'-postverify%02d'%number
    snapshot = Path('build/round12/raw-source-trials')/args.label/('postverify%02d'%number)
    run(['tools/round12_freeze.py','--destination',snapshot])
    run(['tools/round12_measure.py','--snapshot',snapshot,'--label',label])
    measurement = Path('reports/round12-'+label+'-measurement.json')
    measured = read(ROOT/measurement)['assemblies']['KairoLibrary']
    if (measured['matched_methods'] != 3287 or measured['matched_original_il_bytes'] != 269894
            or measured['divergent_bodies'] or measured['equal_body_held_methods']):
        raise ValueError('Metadata-source commit changed semantic closure')
    run(['tools/round12_raw_compare.py','--snapshot',snapshot,'--assembly','KairoLibrary','--label',label])
    raw_path = Path('reports/round12-'+label+'-KairoLibrary-raw.json')
    raw = read(ROOT/raw_path)
    if journal['steps'][-1]['raw_status'] == 'PASS' and raw['status'] != 'PASS':
        raise ValueError('Literal raw result regressed on the fresh verified PE')
    row = dict(label=label,snapshot=snapshot.as_posix(),measurement=measurement.as_posix(),
        measurement_sha256=sha(ROOT/measurement),raw_report=raw_path.as_posix(),
        raw_report_sha256=sha(ROOT/raw_path),raw_status=raw['status'],
        outside_exceptions_differing_bytes=raw['outside_exceptions_differing_bytes'],
        recompiled_sha256=raw['recompiled_sha256'],provenance=snapshot_provenance(ROOT/snapshot),
        methods_gained=0,IL_bytes_gained=0,body_cycle_journal_unchanged=True,
        root_initial_acceptance=approval,Root_review_required=False)
    journal.setdefault('commit_provenance',[]).append(row)
    write(journal_path,journal)
    if raw['status'] != 'PASS': raise ValueError('Cannot consolidate Part A as complete without renewed literal PASS')
    cfg['phase'] = 'part_A_complete_B_ready'
    cfg['required_parts']['A'] = 'complete'
    cfg['part_a_raw_gate'] = dict(status='PASS',
        candidate_status=raw['status'],report=raw_path.as_posix(),sha256=sha(ROOT/raw_path),
        snapshot=snapshot.as_posix(),recompiled_sha256=raw['recompiled_sha256'],
        raw_source_journal_sha256=sha(journal_path),root_initial_acceptance=approval,
        initial_root_pass=dict(report=root_acceptance['report'],sha256=root_acceptance['report_sha256']))
    cfg['raw_source_current_measurement'] = dict(path=measurement.as_posix(),sha256=sha(ROOT/measurement),
        snapshot=snapshot.as_posix(),recompiled_sha256=raw['recompiled_sha256'],
        raw_source_journal_sha256=sha(journal_path))
    write(cfg_path,cfg)
    print(json.dumps({k:row[k] for k in ['label','snapshot','raw_status','outside_exceptions_differing_bytes','recompiled_sha256']}))


if __name__ == '__main__': main()
