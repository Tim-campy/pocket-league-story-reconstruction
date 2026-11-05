"""Isolate Microsoft v2 mscorlib using frozen current C#; no main/state writes."""
import hashlib,json,shutil,struct,subprocess
from pathlib import Path
from compare_kairo import differences,sha
from metadata_compare import compare_paths
from kairo_candidate import BUILD_FOLDER,COMPILER_PATH
ROOT=Path(__file__).resolve().parents[1]
def read(p):return json.loads(Path(p).read_text(encoding='utf-8-sig'))
def pe(path):
    data=path.read_bytes();offset=struct.unpack_from('<I',data,0x3c)[0]
    return {'machine_hex':hex(struct.unpack_from('<H',data,offset+4)[0]),'optional_magic_hex':hex(struct.unpack_from('<H',data,offset+24)[0])}
def dump(binary,out):
    with out.open('wb') as file:subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--structure'],stdout=file,check=True)
    return read(out)
def main():
    production=ROOT/BUILD_FOLDER;manifest=read(production/'build-manifest.json');receipt=read(ROOT/'build/verification-KairoLibrary.json')
    if manifest['output_sha256']!=sha(production/'KairoLibrary.dll') or receipt['build_sha256']!=manifest['output_sha256']:raise ValueError('Main artifact is not verified')
    snapshot=ROOT/'analysis/variants/round3-mscorlib-current';snapshot.mkdir(parents=True,exist_ok=True)
    sources=[]
    for item in manifest['sources']:
        source=ROOT/item['path']
        if sha(source)!=item['sha256']:raise ValueError('Source changed while freezing')
        out=snapshot/item['path'];out.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(source,out)
        sources.append({**item,'input_path':str(out)})
    for name in ['original-methods.json','rebuilt-methods.json']:shutil.copy2(production/name,snapshot/name)
    original=read(snapshot/'original-methods.json');current=read(snapshot/'rebuilt-methods.json');oi={m['stable_id']:m for m in original['selected_methods']};ci={m['stable_id']:m for m in current['selected_methods']}
    core32=Path('C:/Windows/Microsoft.NET/Framework/v2.0.50727/mscorlib.dll');core64=Path('C:/Windows/Microsoft.NET/Framework64/v2.0.50727/mscorlib.dll')
    references=manifest['references'];core_report=[{'path':str(p),'sha256':sha(p),'pe':pe(p)} for p in [Path(references[0]['path']),core32,core64]]
    results={}
    baseline_equal={k for k,m in oi.items() if 'raw_body_hex'in m and not differences(m,ci.get(k))[0]}
    if len(baseline_equal)!=receipt['counts']['body_equal']:raise ValueError('Current body count differs from receipt')
    original_path=ROOT/'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'
    oldmeta=compare_paths(original_path,production/'KairoLibrary.dll')
    for label,new_core in [('unity-frozen',None),('microsoft',core32)]:
        folder=ROOT/'build'/('KairoLibrary-round3-mscorlib-'+label);folder.mkdir(parents=True,exist_ok=True)
        refs=[dict(r) for r in references]
        if new_core:refs[0]={'path':str(new_core),'sha256':sha(new_core)}
        output=folder/'KairoLibrary.dll';flags=[f for f in manifest['flags'] if not f.startswith('/out:')]+['/out:'+str(output)]
        args=[f for f in flags if f!='/noconfig']+['/reference:'+r['path'] for r in refs]+[s['input_path'] for s in sources]
        rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+x+'"' for x in args)+'\n',encoding='utf-8')
        result=subprocess.run([str(ROOT/COMPILER_PATH),'/noconfig','@'+str(rsp)],capture_output=True);(folder/'compiler.log').write_bytes(result.stdout+result.stderr)
        item={'exit_code':result.returncode,'references':refs,'flags':flags}
        if result.returncode:
            item['errors']=(result.stdout+result.stderr).decode('utf-8',errors='replace')[-10000:]
        else:
            document=dump(output,folder/'rebuilt-methods.json');built={m['stable_id']:m for m in document['selected_methods']}
            equal={k for k,m in oi.items() if 'raw_body_hex'in m and not differences(m,built.get(k))[0]}
            meta=compare_paths(original_path,output,folder/'metadata-comparison.json')
            bt={k for k,t in oldmeta['types'].items() if t['equal']};nt={k for k,t in meta['types'].items() if t['equal']}
            item.update(body_equal=len(equal),body_gains=sorted(equal-baseline_equal),body_losses=sorted(baseline_equal-equal),
                all_method_output_differences=[k for k,m in ci.items() if differences(m,built.get(k))[0]],type_equal=meta['types_equal'],type_gains=sorted(nt-bt),type_losses=sorted(bt-nt),
                declarations_proved=meta['declarations_proved'],global_differences=meta['global_differences'],candidate_sha256=sha(output),missing_methods=sorted(set(oi)-set(built)),extra_methods=sorted(set(built)-set(oi)))
        (folder/'build-manifest.json').write_text(json.dumps({'compiler':str(ROOT/COMPILER_PATH),'compiler_sha256':sha(ROOT/COMPILER_PATH),'sources':sources,'references':refs,'flags':flags,'exit_code':result.returncode,'output_sha256':sha(output) if result.returncode==0 else None},indent=2)+'\n')
        results[label]=item;print(json.dumps({'variant':label,'body_equal':item.get('body_equal'),'body_gains':item.get('body_gains'),'body_losses':item.get('body_losses'),'type_equal':item.get('type_equal'),'type_gains':item.get('type_gains'),'type_losses':item.get('type_losses')}),flush=True)
    if results['unity-frozen'].get('all_method_output_differences'):raise ValueError('Frozen source baseline does not reproduce main methods')
    report={'scope':'current verified source frozen before reference experiment; only core library changes','main_output_sha256':manifest['output_sha256'],'baseline_body_equal':len(baseline_equal),'baseline_type_equal':oldmeta['types_equal'],'source_snapshot':str(snapshot.relative_to(ROOT)),'sources':sources,'core_libraries':core_report,'microsoft_32_64_hash_equal':sha(core32)==sha(core64),'results':results,'main_sources_modified':False,'main_references_modified':False,'ledger_modified':False}
    (ROOT/'reports/round3-mscorlib-probe.json').write_text(json.dumps(report,indent=2)+'\n')
if __name__=='__main__':main()
