"""Run the existing read-only raw comparer against a fresh frozen round-11 build."""
import argparse
import json
from pathlib import Path
import re
import subprocess
import sys
from compare_kairo import sha
from apply_il_rules import receipt_is_fresh

ROOT=Path(__file__).resolve().parents[1]


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--assembly',required=True,choices=('KairoLibrary','Assembly-CSharp'))
    parser.add_argument('--label',required=True)
    args=parser.parse_args()
    if not re.fullmatch('[A-Za-z0-9][A-Za-z0-9_.-]*',args.label):raise ValueError('Unsafe measurement label')
    receipt_path=ROOT/('build/verification-'+args.assembly+'.json')
    receipt=json.loads(receipt_path.read_text(encoding='utf-8-sig'))
    if not receipt_is_fresh(ROOT,receipt) or any(receipt.get(k) for k in ('regressions','body_regressions','type_regressions')):
        raise ValueError('Raw trigger requires a fresh nonregressive verifier receipt')
    snapshot=ROOT/'build/round11/raw-inputs'/args.label/args.assembly
    if snapshot.exists():raise FileExistsError('Never replace raw input evidence; choose a new label')
    subprocess.run([sys.executable,'-X','utf8','tools/round11_freeze.py','--destination',str(snapshot)],cwd=ROOT,check=True)
    subprocess.run([sys.executable,'-X','utf8','tools/round10_raw_measurement.py',
        '--baseline',str(snapshot),'--assembly',args.assembly,'--label','round11-'+args.label],cwd=ROOT,check=True)
    manifest=ROOT/'reports'/('round11-'+args.label+'-'+args.assembly+'-raw-manifest.json')
    print(json.dumps({'assembly':args.assembly,'input_build_sha256':receipt['build_sha256'],
        'manifest':str(manifest.relative_to(ROOT)),'manifest_sha256':sha(manifest),
        'snapshot':str(snapshot.relative_to(ROOT)),'assembly_bytes_modified':False,'method_states_edited':False}))


if __name__=='__main__':main()
