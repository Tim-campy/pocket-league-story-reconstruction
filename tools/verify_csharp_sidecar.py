"""Verify whole Assembly-CSharp without changing the shared method ledger.

The root verifier can integrate this machine-generated sidecar. Whole type and
assembly metadata identity remain separate, unimplemented claims.
"""
from collections import Counter
import argparse
import csv
import json
from pathlib import Path
import re
import subprocess
from build_csharp import sha
from compare_kairo import differences,patterns
from generated_groups import prove_groups

ROOT=Path(__file__).resolve().parents[1]


def inspect(path,output):
    with output.open('wb') as file:
        subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(path),'--structure'],stdout=file,check=True)
    return json.loads(output.read_text(encoding='utf-8-sig'))


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--label',default='mono-smcs')
    args=parser.parse_args()
    folder=(ROOT/'build'/('Assembly-CSharp-'+args.label)).resolve()
    if not folder.is_relative_to((ROOT/'build').resolve()):raise ValueError('Build folder outside workspace')
    manifest=json.loads((folder/'build-manifest.json').read_text(encoding='utf-8'))
    artifact=folder/'Assembly-CSharp.dll'
    if manifest['exit_code'] or sha(artifact)!=manifest['output_sha256']:raise ValueError('Build missing or changed')
    if sha(Path(manifest['compiler']))!=manifest['compiler_sha256']:raise ValueError('Compiler changed since build')
    if sha(Path(manifest['runtime']))!=manifest['runtime_sha256']:raise ValueError('Compiler runtime changed since build')
    for source in manifest['sources']:
        if sha(ROOT/source['path'])!=source['sha256']:raise ValueError('Source changed')
    for ref in manifest['references']:
        if sha(Path(ref['path']))!=ref['sha256']:raise ValueError('Reference changed')
    oracle=ROOT/'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll'
    if sha(oracle)!='3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949':
        raise ValueError('Original Assembly-CSharp identity changed')
    original=inspect(oracle,folder/'original-methods.json');rebuilt=inspect(artifact,folder/'rebuilt-methods.json')
    with (ROOT/'config/methods.csv').open(encoding='utf-8',newline='') as file:
        ledger={row['token']:row for row in csv.DictReader(file) if row['assembly']=='Assembly-CSharp'}
    originals=original['selected_methods'];candidates={m['stable_id']:m for m in rebuilt['selected_methods']}
    if len(candidates)!=len(rebuilt['selected_methods']):raise ValueError('Ambiguous candidate method IDs')
    token=lambda m:'0x'+format(0x06000000|int(m['token'].split('0x')[1].rstrip(']'),16),'08x')
    generated_ids={m['stable_id'] for m in originals if ledger[token(m)]['gerado_por']}
    equal_ids={m['stable_id'] for m in originals if 'raw_body_hex' in m and not differences(m,candidates.get(m['stable_id']))[0]}
    ownership,groups=prove_groups(original,rebuilt,generated_ids,equal_ids)
    generated_signatures={m['signature'] for m in originals if m['stable_id'] in generated_ids}
    compiled={s['path'] for s in manifest['sources']}
    counts=Counter();facets=Counter();traits=Counter();results={}
    for method in originals:
        row=ledger[token(method)];candidate=candidates.get(method['stable_id']);delta,percent=differences(method,candidate)
        has_body='raw_body_hex' in method;body_equal=has_body and not delta
        references=[]
        for instruction in method.get('instructions',[]):
            if isinstance(instruction['operand'],dict):references.extend(instruction['operand'].values())
        pending=bool(row['gerado_por']) or bool(re.search(r'::[<]|/[<]|<PrivateImplementationDetails>',json.dumps(references))) or any(
            r in generated_signatures for r in references if isinstance(r,str))
        proof=ownership.get(method['stable_id'])
        if pending and proof and proof['proved']:pending=False
        available=row['fonte'] in compiled
        state='matched' if body_equal and not pending and available else 'decompiled' if available else 'todo'
        labels=patterns(method,candidate) if delta else []
        counts[state]+=1;counts['body_equal']+=body_equal;counts['declarations_without_body']+=not has_body
        counts['equal_body_held_for_generated_group']+=bool(body_equal and pending)
        counts['missing_method']+=candidate is None
        if delta and has_body:facets.update(delta);traits.update(labels)
        results[row['id']]={'state':state,'percent':percent,'differences':delta,'body_present':has_body,
            'body_equal':body_equal,'generated_group_pending':pending,'patterns':labels,'ownership_proof':proof,
            'stable_id':method['stable_id']}
    summary={'schema_version':1,'assembly':'Assembly-CSharp','compiler_candidate':manifest['compiler'],
        'historical_compiler_identity':'UNKNOWN','original_sha256':sha(oracle),'build_sha256':sha(artifact),
        'build_artifact':artifact.relative_to(ROOT).as_posix(),'counts':dict(counts),'difference_facets':dict(facets),
        'patterns':dict(traits),'extra_method_ids':len(set(candidates)-{m['stable_id'] for m in originals}),
        'type_metadata_identity':'NOT_VERIFIED','whole_assembly_byte_identity':False,'normalized_assembly_fields':[],
        'ledger_updated':False,'inspector_source_sha256':sha(ROOT/'tools/ProbeInspector.cs'),
        'verifier_sha256':sha(Path(__file__)),'sources':manifest['sources'],'build_label':args.label,
        'helper_source_hashes':{name:sha(ROOT/name) for name in ('tools/compare_kairo.py','tools/generated_groups.py')},
        'compiler_sha256':manifest['compiler_sha256'],'runtime_sha256':manifest['runtime_sha256']}
    (folder/'verification-sidecar.json').write_text(json.dumps({**summary,'methods':results,'generated_groups':groups},indent=2)+'\n',encoding='utf-8')
    (ROOT/'reports/Assembly-CSharp-verification.json').write_text(json.dumps({k:v for k,v in summary.items() if k!='sources'},indent=2)+'\n',encoding='utf-8')
    with (ROOT/'reports/Assembly-CSharp-verification.csv').open('w',encoding='utf-8',newline='') as file:
        writer=csv.DictWriter(file,fieldnames=['id','stable_id','state','percent','body_equal','generated_group_pending','differences'],lineterminator='\n')
        writer.writeheader()
        for key,row in results.items():writer.writerow({k:key if k=='id' else ';'.join(row[k]) if k=='differences' else row[k] for k in writer.fieldnames})
    print(json.dumps({k:v for k,v in summary.items() if k not in ('sources','patterns','difference_facets')}))


if __name__=='__main__':main()
