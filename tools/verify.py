"""Reference preflight. Full compiler-backed matching remains unimplemented.

Exit 0 only for explicit --reference-only success. Exit 1: reference corruption.
Exit 2: reconstruction verification blocked. Never sets matched or normalizes
fields, and never treats original DLL copies as a reconstructed build.
"""
import argparse
import csv
import hashlib
import json
from pathlib import Path
import sys
from verify_apk import compare_files
ROOT=Path(__file__).resolve().parents[1]


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    scope=parser.add_mutually_exclusive_group(required=True)
    scope.add_argument('--all',action='store_true'); scope.add_argument('--method')
    parser.add_argument('--reference-only',action='store_true'); args=parser.parse_args()
    project=json.loads((ROOT/'config/project.json').read_text(encoding='utf-8'))
    refs=json.loads((ROOT/'config/method-reference.json').read_text(encoding='utf-8'))
    with (ROOT/'config/methods.csv').open(encoding='utf-8',newline='') as file: methods=list(csv.DictReader(file))
    if len(refs)!=len(methods) or set(refs)!={m['id'] for m in methods}: raise ValueError('Method ledger differs from reference')
    if any(m['estado']=='matched' for m in methods): raise ValueError('Unsupported matched state: no compiler-backed receipts')
    if args.method and args.method not in refs: parser.error('Unknown full method signature')
    with (ROOT/project['original_apk']).open('rb') as file: sha=hashlib.file_digest(file,'sha256').hexdigest()
    if sha!=project['apk_sha256']: raise ValueError('Original APK changed')
    baseline=compare_files(ROOT/project['original_apk'],ROOT/project['extracted']) if args.all else None
    if baseline and not baseline['equal']: raise ValueError('Extracted original differs from APK')
    cache={}; checked=0
    for method in methods:
        if args.method and method['id']!=args.method: continue
        ref=refs[method['id']]; relative=ref['artifact_path']
        if relative not in cache:
            data=(ROOT/project['extracted']/relative).read_bytes()
            if hashlib.sha256(data).hexdigest()!=ref['artifact_sha256']: raise ValueError('Assembly reference hash changed')
            cache[relative]=data
        body=ref['body']
        if int(method['tamanho_il'])!=ref['il_size'] or method['token']!=ref['token']:
            raise ValueError('Method identity or IL size changed')
        if body:
            payload=cache[relative][body['file_offset']:body['file_offset']+body['size']]
            if hashlib.sha256(payload).hexdigest()!=body['sha256']: raise ValueError('Original method body changed')
            if not (ROOT/ref['reference_il']).is_file(): raise ValueError('Missing IL reference')
            checked+=1
    result={'stage':'reference-preflight','reference':'PASS','reference_bodies':checked,
        'reference_files':baseline['files_total'] if baseline else None,
        'reconstruction':'NOT_EVALUATED' if args.reference_only else 'BLOCKED','matched':0,
        'blockers':['Compiler/flags not yet validated for each DLL','Whole-assembly C# build pending',
                    'Resolved-token method/type/assembly matcher pending']}
    (ROOT/'build').mkdir(exist_ok=True)
    (ROOT/'build/verification-preflight.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(result)); return 0 if args.reference_only else 2


def cli():
    try: return main()
    except (OSError,ValueError) as error:
        print(str(error),file=sys.stderr); return 1


if __name__=='__main__': sys.exit(cli())
