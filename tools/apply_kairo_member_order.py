"""Verify the isolated member-order candidate before touching project sources.

The permutation/explicit-constructor restoration is user-authorized. Every
previously equal body must remain equal; final states come only from verify.
"""
import csv
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import differences,sha

ROOT=Path(__file__).resolve().parents[1]


def main():
    folder=ROOT/'build/KairoLibrary-trial-order-all'
    dump=folder/'rebuilt-methods.json'
    with dump.open('wb') as file:
        subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(folder/'KairoLibrary.dll'),'--structure'],stdout=file,check=True)
    original=json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']
    rebuilt={m['stable_id']:m for m in json.loads(dump.read_text(encoding='utf-8-sig'))['selected_methods']}
    baseline={m['stable_id']:m for m in json.loads((ROOT/'build/KairoLibrary-roslyn-1.3.2/rebuilt-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']}
    equal=[];gains=[];losses=[]
    for method in original:
        if 'raw_body_hex' not in method:continue
        key=method['stable_id'];same=not differences(method,rebuilt.get(key))[0]
        previous=not differences(method,baseline.get(key))[0]
        if same:equal.append(key)
        if same and not previous:gains.append(key)
        if previous and not same:losses.append(key)
    report={'original_bodies':len(equal)+sum('raw_body_hex' in m and m['stable_id'] not in equal for m in original),
        'body_equal':len(equal),'gains':gains,'losses':losses,'applied':False}
    output=ROOT/'reports/KairoLibrary-member-order-application.json'
    if losses:
        output.write_text(json.dumps(report,indent=2)+'\n');raise ValueError('Candidate regresses previously equal method bodies')
    manifest=json.loads((folder/'build-manifest.json').read_text())
    mapping=json.loads((ROOT/'analysis/kairo-order-combined-source-map.json').read_text())
    for source in manifest['sources']:
        path=Path(source.get('input_path',ROOT/source['path']))
        if sha(path)!=source['sha256']:raise ValueError('Trial input changed: '+source['path'])
        if source['path'] not in mapping and sha(ROOT/source['path'])!=source['sha256']:
            raise ValueError('Project source changed since trial')
    backup=ROOT/'analysis/member-order-backup';backup.mkdir(exist_ok=True)
    for logical,candidate in mapping.items():
        path=ROOT/logical;saved=backup/logical;saved.parent.mkdir(parents=True,exist_ok=True)
        if saved.exists():raise ValueError('Refusing overwrite of restoration backup')
        saved.write_bytes(path.read_bytes());path.write_bytes(Path(candidate).read_bytes())
    result=subprocess.run([sys.executable,str(ROOT/'tools/verify_kairo.py')],cwd=ROOT,capture_output=True,text=True)
    print(result.stdout.strip())
    receipt=json.loads((ROOT/'build/verification-KairoLibrary.json').read_text())
    if result.returncode not in (0,1) or receipt['regressions']:
        for logical in mapping:(ROOT/logical).write_bytes((backup/logical).read_bytes())
        subprocess.run([sys.executable,str(ROOT/'tools/verify_kairo.py')],cwd=ROOT,check=False,capture_output=True)
        report['failure']='restored because full verifier failed or regressed'
        output.write_text(json.dumps(report,indent=2)+'\n');raise ValueError(report['failure'])
    report.update({'applied':True,'source_files':len(mapping),'verified_matched':receipt['counts']['matched'],
        'compiler_package':'Microsoft.Net.Compilers 1.3.2','whole_assembly_equal':False})
    output.write_text(json.dumps(report,indent=2)+'\n')
    print(json.dumps({k:v if not isinstance(v,list) else len(v) for k,v in report.items()}))


if __name__=='__main__':main()
