"""Observe two identical smcs builds of frozen sources; no normalization/state edits.

The output path is isolated; shared main sources and verification state may be
changed concurrently by the parent, so they are never compilation inputs here.
"""
import json,subprocess,sys,time
from pathlib import Path
from check_kairo_repeatability_1_1 import observe
from check_kairo_repeatability import volatile_ranges
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]

def main():
    frozen=ROOT/'analysis/variants/round3-baseline-Assembly-CSharp/source-map.json'
    base=json.loads((ROOT/'build/round3/Assembly-CSharp-build-manifest.json').read_text(encoding='utf-8'))
    mapping=json.loads(frozen.read_text(encoding='utf-8-sig'))
    if set(mapping)!={s['path'] for s in base['sources']} or len(mapping)!=97:
        raise ValueError('Expected all 97 frozen compilation units')
    identity={s['path']:s['sha256'] for s in base['sources']}
    def validate_inputs():
        for logical,path in mapping.items():
            p=Path(path).resolve()
            if not p.is_relative_to((ROOT/'analysis/variants/round3-baseline-Assembly-CSharp').resolve()):
                raise ValueError('Source map escapes the frozen snapshot')
            if sha(p)!=identity[logical]:raise ValueError('Frozen source changed: '+logical)
        for ref in base['references']:
            if sha(Path(ref['path']))!=ref['sha256']:raise ValueError('Original reference changed')
        for key in ('compiler','runtime'):
            if sha(Path(base[key]))!=base[key+'_sha256']:raise ValueError('Toolchain binary changed')
    validate_inputs()
    folder=ROOT/'build/Assembly-CSharp-round3-repeatability';folder.mkdir(parents=True,exist_ok=True)
    order=folder/'compile-order.txt'
    order.write_text('\n'.join(s['path'] for s in base['sources'] if s['path']!='src/Assembly-CSharp/AssemblyInfo.cs')+'\n',encoding='utf-8')
    command=[sys.executable,str(ROOT/'tools/build_csharp.py'),'--label','round3-repeatability',
             '--source-map',str(frozen),'--compile-order',str(order)]
    rea_root=Path(json.loads((ROOT/'config/toolchain.json').read_text(encoding='utf-8'))['rea_package_root'])
    target=folder/'Assembly-CSharp.dll'
    subprocess.run(command,cwd=ROOT,check=True,capture_output=True)
    first,a=observe(target,rea_root);manifest=json.loads((folder/'build-manifest.json').read_text(encoding='utf-8'))
    (folder/'first.dll').write_bytes(first)
    (folder/'first-manifest.json').write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
    validate_inputs();time.sleep(1.1)
    subprocess.run(command,cwd=ROOT,check=True,capture_output=True)
    second,b=observe(target,rea_root);other=json.loads((folder/'build-manifest.json').read_text(encoding='utf-8'))
    validate_inputs()
    for key in ('compiler','compiler_sha256','runtime','runtime_sha256','flags','stdlib_binding','scoped_environment','sources','references'):
        if manifest[key]!=other[key]:raise ValueError('Build inputs changed: '+key)
    if len(first)!=len(second):raise ValueError('Repeated output lengths differ')
    ranges=volatile_ranges(first,a)
    if ranges!=volatile_ranges(second,b):raise ValueError('Field layout changed')
    changed=[i for i,(x,y) in enumerate(zip(first,second)) if x!=y]
    covered={i for field in ranges for i in range(field['offset'],field['offset']+field['size'])}
    for field in ranges:
        start=field['offset'];end=start+field['size']
        field.update(changed_bytes=sum(i in changed for i in range(start,end)),
                     first_hex=first[start:end].hex(),second_hex=second[start:end].hex())
    result={'schema_version':1,'assembly':'Assembly-CSharp',
        'scope':'two identical complete smcs2.6.5.0 builds, frozen97sources, defaultstdlib, debug-, unsafe-',
        'command':command,'output':str(target.relative_to(ROOT)),'source_map':str(frozen.relative_to(ROOT)),
        'source_map_sha256':sha(frozen),'source_count':len(mapping),'flags':manifest['flags'],
        'compiler_sha256':manifest['compiler_sha256'],'runtime_sha256':manifest['runtime_sha256'],
        'scoped_environment':manifest['scoped_environment'],'sources':manifest['sources'],'references':manifest['references'],
        'first':a,'second':b,'output_length':len(first),'changed_byte_count':len(changed),'changed_offsets':changed,
        'identified_ranges':ranges,'fields_observed_changing':[f['field'] for f in ranges if f['changed_bytes']],
        'identified_fields_not_observed_changing':[f['field'] for f in ranges if not f['changed_bytes']],
        'unexplained_changed_offsets':[i for i in changed if i not in covered],
        'comparison_exceptions_applied':False,'post_compile_binary_edits':False,'main_output_written':False,
        'method_or_type_state_written':False,'original_build_determinism':'UNKNOWN',
        'main_concurrency':'Shared main/state can change under the parent; this experiment writes only its own isolated output/report.'}
    out=ROOT/'reports/round3-Assembly-CSharp-repeatability.json'
    out.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:result[k] for k in ['source_count','output_length','changed_byte_count','fields_observed_changing','identified_fields_not_observed_changing','unexplained_changed_offsets']}),flush=True)
    if result['unexplained_changed_offsets']:raise ValueError('Some changed bytes remain unidentified')
if __name__=='__main__':main()
