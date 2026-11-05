"""Check the oracle, then compile and compare the recovered assembly.

Exit 0: requested checks passed; 1: differences/failures; 2: missing scope.
Type/whole-assembly identity is reported separately and remains pending.
"""
import argparse
import csv
import hashlib
import json
from pathlib import Path
import sys
import subprocess
from verification_state import validate_matched
from verify_apk import compare_files
ROOT=Path(__file__).resolve().parents[1]


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    scope=parser.add_mutually_exclusive_group(required=True)
    scope.add_argument('--all',action='store_true'); scope.add_argument('--method')
    parser.add_argument('--reference-only',action='store_true')
    parser.add_argument('--assembly',choices=['KairoLibrary','Assembly-CSharp']); args=parser.parse_args()
    project=json.loads((ROOT/'config/project.json').read_text(encoding='utf-8'))
    refs=json.loads((ROOT/'config/method-reference.json').read_text(encoding='utf-8'))
    with (ROOT/'config/methods.csv').open(encoding='utf-8',newline='') as file: methods=list(csv.DictReader(file))
    if len(refs)!=len(methods) or set(refs)!={m['id'] for m in methods}: raise ValueError('Method ledger differs from reference')
    if args.method and args.method not in refs: parser.error('Unknown full method signature')
    if args.method:
        selected=next(m['assembly'] for m in methods if m['id']==args.method)
        if args.assembly and selected!=args.assembly: parser.error('Method belongs to another assembly')
        args.assembly=selected
    with (ROOT/project['original_apk']).open('rb') as file: sha=hashlib.file_digest(file,'sha256').hexdigest()
    if sha!=project['apk_sha256']: raise ValueError('Original APK changed')
    baseline=compare_files(ROOT/project['original_apk'],ROOT/project['extracted']) if args.all else None
    if baseline and not baseline['equal']: raise ValueError('Extracted original differs from APK')
    cache={}; checked=0
    for method in methods:
        if args.method and method['id']!=args.method: continue
        if args.assembly and method['assembly']!=args.assembly: continue
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
        'reconstruction':'NOT_EVALUATED' if args.reference_only else 'PARTIAL',
        'matched':sum(m['estado']=='matched' for m in methods),
        'blockers':['Remaining body/metadata differences and raw whole-assembly identity']}
    (ROOT/'build').mkdir(exist_ok=True)
    (ROOT/'build/verification-preflight.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(result),flush=True)
    if args.reference_only:
        validate_matched(ROOT,methods)
        return 0
    status=0
    verifiers={'KairoLibrary':'verify_kairo.py','Assembly-CSharp':'verify_csharp.py'}
    for assembly in verifiers:
        if args.assembly and assembly!=args.assembly:continue
        code=subprocess.run([sys.executable,str(ROOT/'tools'/verifiers[assembly])],cwd=ROOT).returncode
        if code not in (0,1):return code
        status=max(status,code)
        if args.method:
            receipt=json.loads((ROOT/f'build/verification-{assembly}.json').read_text(encoding='utf-8'))
            print(json.dumps({'method':args.method,'result':receipt['methods'][args.method]}))
    return status or 2


def cli():
    try: return main()
    except (OSError,ValueError) as error:
        print(str(error),file=sys.stderr); return 1


if __name__=='__main__': sys.exit(cli())
