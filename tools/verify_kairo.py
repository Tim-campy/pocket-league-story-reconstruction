"""Compile the selected Kairo candidate and verify complete method bodies.

Methods with unresolved compiler-generated ownership/dependencies and declarations
without CIL remain decompiled. Type/assembly equality is reported separately.
"""
from collections import Counter
import csv
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys
from compare_kairo import assert_build_provenance, differences, patterns, sha
from generated_groups import prove_groups

ROOT=Path(__file__).resolve().parents[1]


def dump_methods(binary,output):
    temporary=output.with_suffix('.tmp')
    with temporary.open('wb') as file:
        result=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--structure'],stdout=file,stderr=subprocess.PIPE)
    if result.returncode: raise RuntimeError(result.stderr.decode('utf-8',errors='replace'))
    temporary.replace(output)
    return json.loads(output.read_text(encoding='utf-8-sig'))['selected_methods']


def main():
    compiler=ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'
    if not compiler.is_file(): raise SystemExit('Selected compiler package missing; acquire the documented Microsoft.Net.Compilers 1.3.2 package')
    if sha(compiler)!='d14ba119edd71bdcb82b12659647427aa46d819caf6e72ac6d4ae86c81eb36c9':
        raise ValueError('Selected compiler SHA-256 does not match the pinned candidate')
    subprocess.run([sys.executable,str(ROOT/'tools/prepare_probe_inspector.py')],check=True,cwd=ROOT,capture_output=True)
    subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',str(compiler),'--label','roslyn-1.3.2'],check=True,cwd=ROOT,capture_output=True)
    folder=ROOT/'build/KairoLibrary-roslyn-1.3.2'
    manifest=assert_build_provenance('build/KairoLibrary-roslyn-1.3.2')
    original_path=ROOT/'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'
    source_hash=sha(ROOT/'tools/ProbeInspector.cs')
    cache_path=folder/'original-cache.json';dump_path=folder/'original-methods.json'
    cache={'artifact_sha256':sha(original_path),'inspector_source_sha256':source_hash,'cecil_sha256':sha(ROOT/'build/tools/Mono.Cecil.dll')}
    if cache_path.is_file() and dump_path.is_file() and json.loads(cache_path.read_text())==cache:
        original=json.loads(dump_path.read_text(encoding='utf-8-sig'))['selected_methods']
    else:
        original=dump_methods(original_path,dump_path)
        cache_path.write_text(json.dumps(cache,indent=2)+'\n',encoding='utf-8')
    rebuilt=dump_methods(folder/'KairoLibrary.dll',folder/'rebuilt-methods.json')
    indices={m['stable_id']:m for m in rebuilt}
    if len(indices)!=len(rebuilt): raise ValueError('Ambiguous rebuilt method IDs')
    with (ROOT/'config/methods.csv').open(encoding='utf-8',newline='') as file:
        reader=csv.DictReader(file);columns=reader.fieldnames;ledger=list(reader)
    target={row['token']:row for row in ledger if row['assembly']=='KairoLibrary'}
    reference=json.loads((ROOT/'config/method-reference.json').read_text(encoding='utf-8'))
    for row in target.values():
        if reference[row['id']]['artifact_sha256']!=cache['artifact_sha256']: raise ValueError('Oracle identity mismatch')
    token_of=lambda m:'0x'+format(0x06000000|int(m['token'].split('0x')[1].rstrip(']'),16),'08x')
    generated_signatures={m['signature'] for m in original if target[token_of(m)]['gerado_por']}
    original_document=json.loads(dump_path.read_text(encoding='utf-8-sig'))
    rebuilt_document=json.loads((folder/'rebuilt-methods.json').read_text(encoding='utf-8-sig'))
    generated_ids={m['stable_id'] for m in original if target[token_of(m)]['gerado_por']}
    equal_ids={m['stable_id'] for m in original if 'raw_body_hex' in m and not differences(m,indices.get(m['stable_id']))[0]}
    ownership,groups=prove_groups(original_document,rebuilt_document,generated_ids,equal_ids)
    original_rows={m['stable_id']:target[token_of(m)] for m in original}
    compiled_sources={s['path'] for s in manifest['sources']}
    results={};counts=Counter();regressions=[];body_regressions=[]
    for method in original:
        row=target[token_of(method)];built=indices.get(method['stable_id'])
        delta,percent=differences(method,built)
        has_body='raw_body_hex' in method
        body_equal=has_body and not delta
        refs=[]
        for instruction in method.get('instructions',[]):
            operand=instruction['operand']
            if isinstance(operand,dict): refs.extend(operand.values())
        generated_dependency=bool(re.search(r'::[<]|/[<]|<PrivateImplementationDetails>',json.dumps(refs))) or any(
            v in generated_signatures for v in refs if isinstance(v,str))
        group_pending=bool(row['gerado_por']) or generated_dependency
        ownership_proof=ownership.get(method['stable_id'])
        if group_pending and ownership_proof and ownership_proof['proved']: group_pending=False
        if row['gerado_por'] and ownership_proof and ownership_proof['proved'] and len(ownership_proof['origins'])==1:
            row['gerado_por']=original_rows[ownership_proof['origins'][0]]['id']
        source_available=row['fonte'] in compiled_sources
        state='matched' if body_equal and not group_pending and source_available else ('decompiled' if source_available else 'todo')
        if row['estado']=='matched' and state!='matched': regressions.append(row['id'])
        if row['estado']=='matched' and not body_equal:body_regressions.append(row['id'])
        result={'state':state,'body_equal':body_equal,'generated_group_pending':group_pending,
            'percent':percent,'differences':delta,'body_present':has_body,
            'patterns':patterns(method,built) if delta else [],'ownership_proof':ownership_proof}
        results[row['id']]=result;counts[state]+=1
        counts['body_equal']+=body_equal
        counts['declarations_without_body']+=not has_body
        if body_equal and group_pending: counts['equal_body_held_for_generated_group']+=1
        row['estado']=state
        row['percentual']=f'{percent:.4f}' if percent is not None else 'UNKNOWN'
        if state=='matched': row['percentual']='100.0000'
        row['notas']='; '.join(filter(None,[
            'verificado por build integral Roslyn pacote 1.3.2',
            'vinculo gerado comprovado por nomes e IL' if ownership_proof and ownership_proof['proved'] else '',
            'grupo gerado ainda UNKNOWN' if group_pending else '',
            'declaracao sem CIL; metadados completos pendentes' if not has_body else '',
            ','.join(delta)]))
    receipt={'schema_version':1,'assembly':'KairoLibrary','build_artifact':'build/KairoLibrary-roslyn-1.3.2/KairoLibrary.dll',
        'build_sha256':manifest['output_sha256'],'original_sha256':cache['artifact_sha256'],
        'verifier_sha256':sha(Path(__file__)),'inspector_source_sha256':source_hash,'sources':manifest['sources'],
        'helper_source_hashes':{p:sha(ROOT/p) for p in ('tools/generated_groups.py','tools/compare_kairo.py')},
        'methods':results,'counts':dict(counts),'regressions':regressions,'body_regressions':body_regressions,'type_metadata_identity':'NOT_VERIFIED',
        'whole_assembly_byte_identity':False,'normalized_assembly_fields':[]}
    (ROOT/'build/verification-KairoLibrary.json').write_text(json.dumps(receipt,indent=2)+'\n',encoding='utf-8')
    with (ROOT/'config/methods.csv').open('w',encoding='utf-8',newline='') as file:
        writer=csv.DictWriter(file,fieldnames=columns,lineterminator='\n');writer.writeheader();writer.writerows(ledger)
    (ROOT/'reports').mkdir(exist_ok=True)
    group_summary={'groups':groups,'ownership':ownership,
        'groups_proved':sum(g['proved'] for g in groups.values()),'groups_total':len(groups),
        'methods_with_ownership_proof':sum(p['proved'] for p in ownership.values()),
        'body_equal_still_held':[key for key,r in results.items() if r['body_equal'] and r['generated_group_pending']]}
    (ROOT/'reports/KairoLibrary-generated-groups.json').write_text(json.dumps(group_summary,indent=2)+'\n',encoding='utf-8')
    summary={key:value for key,value in receipt.items() if key not in ('sources','methods')}
    (ROOT/'reports/KairoLibrary-verification.json').write_text(json.dumps(summary,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'assembly':'KairoLibrary','counts':dict(counts),'regressions':len(regressions),
        'whole_assembly_equal':False,'type_metadata':'NOT_VERIFIED'}))
    return 1 if any(result['state']!='matched' for result in results.values()) or regressions else 0


if __name__=='__main__': raise SystemExit(main())
