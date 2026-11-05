"""Compile and verify the complete Assembly-CSharp candidate, updating its ledger.

Only compiler-produced C# output is compared. Generated ownership proofs and
full bodies are independent requirements; declarations without CIL and the
unowned compiler data constructor cannot be promoted. Type metadata is pending.
"""
import csv
import json
from pathlib import Path
import subprocess
import sys
from build_csharp import sha

ROOT=Path(__file__).resolve().parents[1]
COMPILER_SHA256='26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0'


def main():
    subprocess.run([sys.executable,str(ROOT/'tools/prepare_probe_inspector.py')],cwd=ROOT,capture_output=True,check=True)
    subprocess.run([sys.executable,str(ROOT/'tools/build_csharp.py'),'--compiler','smcs',
        '--optimize','+','--debug','-','--stdlib','default','--label','mono-smcs'],cwd=ROOT,capture_output=True,check=True)
    subprocess.run([sys.executable,str(ROOT/'tools/verify_csharp_sidecar.py'),'--label','mono-smcs'],
        cwd=ROOT,capture_output=True,check=True)
    folder=ROOT/'build/Assembly-CSharp-mono-smcs'
    manifest=json.loads((folder/'build-manifest.json').read_text(encoding='utf-8'))
    sidecar=json.loads((folder/'verification-sidecar.json').read_text(encoding='utf-8'))
    if manifest['compiler_sha256']!=COMPILER_SHA256:raise ValueError('Compiler differs from the validated smcs candidate')
    if sidecar['build_sha256']!=manifest['output_sha256'] or sha(folder/'Assembly-CSharp.dll')!=manifest['output_sha256']:
        raise ValueError('Build changed during verification')
    with (ROOT/'config/methods.csv').open(encoding='utf-8',newline='') as file:
        reader=csv.DictReader(file);columns=reader.fieldnames;ledger=list(reader)
    targets={row['id']:row for row in ledger if row['assembly']=='Assembly-CSharp'}
    if set(targets)!=set(sidecar['methods']):raise ValueError('Sidecar method identities differ from the ledger')
    reference=json.loads((ROOT/'config/method-reference.json').read_text(encoding='utf-8'))
    for row in targets.values():
        if reference[row['id']]['artifact_sha256']!=sidecar['original_sha256']:raise ValueError('Original reference identity mismatch')
    original_ids={result['stable_id']:key for key,result in sidecar['methods'].items()}
    results=sidecar['methods'];regressions=[]
    for key,result in results.items():
        row=targets[key]
        if row['estado']=='matched' and result['state']!='matched':regressions.append(key)
        proof=result.get('ownership_proof')
        if row['gerado_por'] and proof and proof['proved'] and len(proof['origins'])==1:
            row['gerado_por']=original_ids[proof['origins'][0]]
        row['estado']=result['state']
        percent=result['percent']
        row['percentual']='100.0000' if result['state']=='matched' else f'{percent:.4f}' if percent is not None else 'UNKNOWN'
        notes=['verificado por build integral Mono smcs 2.6.5.0']
        if proof and proof['proved']:notes.append('vinculo gerado comprovado por nomes e IL')
        if result['generated_group_pending']:notes.append('vinculo gerado ainda pendente')
        if not result['body_present']:notes.append('declaracao sem CIL; metadados completos pendentes')
        if result['differences']:notes.append(','.join(result['differences']))
        if result['stable_id'].startswith('main.AppData::Draw_helpMessage('):
            notes.append('IndexOf(string,int) restaurado; stdlib default; demais diferencas registradas pelo verificador')
        row['notas']='; '.join(notes)
    helpers=('tools/build_csharp.py','tools/verify_csharp_sidecar.py','tools/compare_kairo.py','tools/generated_groups.py')
    receipt={'schema_version':1,'assembly':'Assembly-CSharp',
        'build_artifact':'build/Assembly-CSharp-mono-smcs/Assembly-CSharp.dll',
        'build_sha256':manifest['output_sha256'],'original_sha256':sidecar['original_sha256'],
        'verifier_sha256':sha(Path(__file__)),'inspector_source_sha256':sha(ROOT/'tools/ProbeInspector.cs'),
        'helper_source_hashes':{name:sha(ROOT/name) for name in helpers},'sources':manifest['sources'],
        'methods':results,'counts':sidecar['counts'],'regressions':regressions,
        'type_metadata_identity':'NOT_VERIFIED','whole_assembly_byte_identity':False,
        'normalized_assembly_fields':[],'historical_compiler_identity':'UNKNOWN',
        'compiler_candidate':'Unity smcs 2.6.5.0','compiler_sha256':manifest['compiler_sha256'],
        'runtime_sha256':manifest['runtime_sha256']}
    (ROOT/'build/verification-Assembly-CSharp.json').write_text(json.dumps(receipt,indent=2)+'\n',encoding='utf-8')
    with (ROOT/'config/methods.csv').open('w',encoding='utf-8',newline='') as file:
        writer=csv.DictWriter(file,fieldnames=columns,lineterminator='\n');writer.writeheader();writer.writerows(ledger)
    summary={key:value for key,value in receipt.items() if key not in ('methods','sources')}
    (ROOT/'reports/Assembly-CSharp-verification.json').write_text(json.dumps(summary,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'assembly':'Assembly-CSharp','counts':receipt['counts'],'regressions':len(regressions),
        'type_metadata':'NOT_VERIFIED','whole_assembly_equal':False}))
    return 1 if regressions or any(r['state']!='matched' for r in results.values()) else 0


if __name__=='__main__':raise SystemExit(main())
