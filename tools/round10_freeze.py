"""Freeze verified round-ten inputs, without touching original artifacts."""
import argparse
import csv
import json
from pathlib import Path
import shutil
import subprocess

from compare_kairo import sha
from apply_il_rules import receipt_is_fresh
from round5_local_cases import local_inspector

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'build/round10/baseline'
BASE_COMMIT = 'aecb02b8a7b0965508e223335e44ae36a95a0939'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--destination', type=Path)
    args = parser.parse_args()
    destination = args.destination.resolve() if args.destination else BASE
    head = subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=ROOT).decode().strip()
    if not args.destination and head != BASE_COMMIT:
        raise ValueError('The round-ten baseline requires commit c51d204')
    if destination.exists():
        raise FileExistsError('Never replace a frozen baseline')
    receipts = {}
    for a in ('KairoLibrary', 'Assembly-CSharp'):
        receipt_path = ROOT / ('build/verification-' + a + '.json')
        receipt = json.loads(receipt_path.read_text(encoding='utf-8-sig'))
        if not receipt_is_fresh(ROOT, receipt) or any(receipt.get(k) for k in ('regressions', 'body_regressions', 'type_regressions')):
            raise ValueError('The baseline proof is stale or regressive: ' + a)
        receipts[a] = receipt
    destination.mkdir(parents=True)
    for name in ('methods.csv', 'types.csv', 'toolchain.json'):
        shutil.copyfile(ROOT / 'config' / name, destination / name)
    for a, receipt in receipts.items():
        target = destination / a
        target.mkdir()
        artifact = ROOT / receipt['build_artifact']
        for name in ('original-methods.json', 'rebuilt-methods.json', 'build-manifest.json'):
            shutil.copyfile(artifact.parent / name, target / name)
        shutil.copyfile(ROOT / ('build/verification-' + a + '.json'), target / 'verification.json')
        manifest = json.loads((target / 'build-manifest.json').read_text(encoding='utf-8-sig'))
        for source in manifest['sources']:
            out = target / source['path']
            out.parent.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(ROOT / source['path'], out)
        for name, binary in [('plain-local-types.json', ROOT / 'original/extracted/assets/bin/Data/Managed' / (a + '.dll')),
                             ('plain-local-types-rebuilt.json', artifact)]:
            result = subprocess.run([str(local_inspector()), str(binary), '--locals-only'], capture_output=True, check=True)
            data = json.loads(result.stdout.decode('utf-8-sig'))
            plain = {m['stable_id']: m.get('plain_local_types', []) for m in data['methods']}
            (target / name).write_text(json.dumps(plain, indent=2) + '\n', encoding='utf-8', newline='\n')
    (destination / 'snapshot.json').write_text(json.dumps({'commit': head, 'ledger_sha256': sha(destination / 'methods.csv')}, indent=2)+'\n', encoding='utf-8')
    print(json.dumps({'baseline_commit': head, 'assemblies': list(receipts), 'ledger_sha256': sha(destination / 'methods.csv')}))


if __name__ == '__main__':
    main()
