"""Full frozen-source comparison of requested revisions against Roslyn1.1.0.

No main source, method/type ledger, build output or verification receipt is changed.
Run only after the parent finishes and verifies its source changes.
"""
import argparse,hashlib,json,re,shutil,subprocess,sys,zipfile
from itertools import combinations
from pathlib import Path
from compare_kairo import body_cells,differences,sha
from fetch_roslyn_revisions import PINNED as REVISION_PINS,COMPONENTS
from metadata_compare import compare_paths

ROOT=Path(__file__).resolve().parents[1]
BASELINE='1.1.0'
VERSIONS=('1.1.0','1.1.1','1.2.1','1.2.2')
PINS={BASELINE:('cd84fb2c71cafe03cdae0add609fa12c75f61c89ea87b1422fd108ac7e570745','53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3'),**REVISION_PINS}
FLAGS=['/nologo','/noconfig','/nostdlib+','/target:library','/platform:anycpu','/optimize+',
       '/debug:pdbonly','/deterministic-','/langversion:6','/unsafe-','/checked-','/codepage:65001','/utf8output']
def write(p,x):p.write_text(json.dumps(x,indent=2)+'\n',encoding='utf-8')
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def index(doc):
    result={m['stable_id']:m for m in doc['selected_methods']}
    if len(result)!=len(doc['selected_methods']):raise ValueError('Ambiguous method identities')
    return result
def delta(base,other):return {'gain_ids':sorted(other-base),'loss_ids':sorted(base-other),'same_match_set':other==base}
def controls():
    swapped=delta({'a','b'},{'a','c'})
    assert swapped=={'gain_ids':['c'],'loss_ids':['b'],'same_match_set':False}
    assert delta({'a'},{'a'})['same_match_set']
    assert not delta({'a'},{'b'})['same_match_set']
    return {'same_count_different_ids_rejected':True,'same_sets_recognized':True,'different_sets_distinguished':True}
def require_compiler(version):
    package=ROOT/f'analysis/downloads/microsoft.net.compilers.{version}.nupkg'
    compiler=ROOT/f'analysis/tools/roslyn-{version}/tools/csc.exe'
    if sha(package)!=PINS[version][0] or sha(compiler)!=PINS[version][1]:raise ValueError('Changed pinned compiler/package '+version)
    components={}
    with zipfile.ZipFile(package) as z:
        for name in COMPONENTS:
            data=z.read('tools/'+name);path=compiler.parent/name
            if path.read_bytes()!=data:raise ValueError('Compiler component differs from pinned package: '+version+'/'+name)
            components[name]=hashlib.sha256(data).hexdigest()
    return compiler,components
def frozen_copy(source,target,expected):
    data=source.read_bytes()
    if hashlib.sha256(data).hexdigest()!=expected:raise ValueError('Stale input '+str(source))
    target.parent.mkdir(parents=True,exist_ok=True);target.write_bytes(data)
def freeze(snapshot):
    main=ROOT/'build/KairoLibrary-roslyn-1.1.0';manifest=read(main/'build-manifest.json')
    if manifest['exit_code'] or sha(main/'KairoLibrary.dll')!=manifest['output_sha256']:raise ValueError('Main baseline is not compiled')
    if manifest['compiler_sha256']!=PINS[BASELINE][1]:raise ValueError('Main baseline compiler differs from1.1.0')
    if [f for f in manifest['flags'] if not f.startswith('/out:')]!=FLAGS:raise ValueError('Unexpected main flags')
    order=[x for x in (ROOT/'config/compile-order/KairoLibrary.txt').read_text(encoding='utf-8').splitlines() if x and not x.startswith('#')]
    if 'src/KairoLibrary/AssemblyInfo.cs'not in order:order.append('src/KairoLibrary/AssemblyInfo.cs')
    if order!=[s['path'] for s in manifest['sources']]:raise ValueError('Current compile order differs from verified main')
    sources=[];references=[]
    for s in manifest['sources']:
        target=snapshot/'sources'/s['path'];frozen_copy(ROOT/s['path'],target,s['sha256'])
        sources.append({'logical_path':s['path'],'original_path':str(ROOT/s['path']),'frozen_path':str(target),'sha256':s['sha256']})
    for n,r in enumerate(manifest['references']):
        target=snapshot/'references'/str(n)/Path(r['path']).name;frozen_copy(Path(r['path']),target,r['sha256'])
        references.append({'ordinal':n,'original_path':r['path'],'frozen_path':str(target),'sha256':r['sha256']})
    frozen_copy(main/'rebuilt-methods.json',snapshot/'baseline-main-methods.json',sha(main/'rebuilt-methods.json'))
    write(snapshot/'input-receipt.json',{'main_manifest_sha256':sha(main/'build-manifest.json'),'main_output_sha256':manifest['output_sha256'],'flags':FLAGS,'sources':sources,'references':references})
    return sources,references
def validate(sources,references):
    for s in sources+references:
        if sha(Path(s['frozen_path']))!=s['sha256']:raise ValueError('Frozen input changed')
def inspect(inspector,binary,out):
    with out.open('wb') as f:subprocess.run([str(inspector),str(binary),'--structure'],check=True,stdout=f,stderr=subprocess.PIPE)
    return index(read(out))
def compile_version(version,folder,sources,references):
    compiler,components=require_compiler(version);out=folder/'KairoLibrary.dll'
    rsp=folder/'compile.rsp';args=[x for x in FLAGS if x!='/noconfig']+['/out:'+str(out)]
    args+=['/reference:'+s['frozen_path'] for s in references]+[s['frozen_path'] for s in sources]
    rsp.write_text('\n'.join('"'+x+'"' for x in args)+'\n',encoding='utf-8')
    p=subprocess.run([str(compiler),'/noconfig','@'+str(rsp)],capture_output=True,text=True,encoding='utf-8',errors='replace')
    (folder/'compiler.log').write_text(p.stdout+p.stderr,encoding='utf-8')
    result={'package_version':version,'compiler':str(compiler),'compiler_sha256':PINS[version][1],
            'package_sha256':PINS[version][0],'components_sha256':components,'flags':FLAGS,'sources':sources,'references':references,
            'exit_code':p.returncode,'output_sha256':sha(out) if p.returncode==0 else None,
            'errors':[l for l in (p.stdout+p.stderr).splitlines() if re.search(r'\berror\s+CS\d+',l)]}
    write(folder/'build-manifest.json',result);return result
def main():
    p=argparse.ArgumentParser();p.add_argument('--label',default='round3');args=p.parse_args()
    if not re.fullmatch('[A-Za-z0-9_-]+',args.label):raise ValueError('Invalid run label')
    snapshot=ROOT/f'analysis/variants/roslyn-revisions-{args.label}';folder=ROOT/f'build/roslyn-revisions-{args.label}'
    report=ROOT/f'reports/KairoLibrary-roslyn-revisions-{args.label}.json'
    if snapshot.exists()or folder.exists()or report.exists():raise ValueError('Run already exists; preserve evidence with a new label')
    tests=controls()
    for version in VERSIONS:require_compiler(version)
    snapshot.mkdir(parents=True);folder.mkdir(parents=True);sources,references=freeze(snapshot)
    own=snapshot/'inspector';own.mkdir()
    for name in ['ProbeInspector.exe','Mono.Cecil.dll']:shutil.copy2(ROOT/'build/tools'/name,own/name)
    inspector=own/'ProbeInspector.exe';original=ROOT/'original/extracted/assets/bin/Data/Managed/KairoLibrary.dll'
    original_sha=sha(original)
    if original_sha!='bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee':raise ValueError('Original hash changed')
    oracle=inspect(inspector,original,folder/'original-methods.json');body_ids={k for k,m in oracle.items() if body_cells(m)is not None}
    matches={};outputs={};per={};metadata={}
    for version in VERSIONS:
        validate(sources,references);dest=folder/version;dest.mkdir();item=compile_version(version,dest,sources,references);per[version]=item
        if item['exit_code']:continue
        methods=inspect(inspector,dest/'KairoLibrary.dll',dest/'rebuilt-methods.json');outputs[version]=methods
        matches[version]={k for k in body_ids if not differences(oracle[k],methods.get(k))[0]}
        item.update(body_equal=len(matches[version]),matched_original_ids=sorted(matches[version]),missing_ids=sorted(set(oracle)-set(methods)),extra_ids=sorted(set(methods)-set(oracle)))
        meta=compare_paths(original,dest/'KairoLibrary.dll',dest/'metadata-comparison.json');metadata[version]=meta
        item['metadata']={k:meta[k] for k in ['types_equal','types_total','type_order_equal','declarations_proved','global_differences','type_difference_counts_nonexclusive']}
        print(json.dumps({'version':version,'body_equal':len(matches[version]),'types_equal':meta['types_equal'],'missing':len(item['missing_ids']),'extra':len(item['extra_ids'])}),flush=True)
    validate(sources,references)
    if sha(original)!=original_sha:raise ValueError('Original changed')
    complete=len(matches)==len(VERSIONS);main=index(read(snapshot/'baseline-main-methods.json'));base=outputs.get(BASELINE,{})
    main_differences=[k for k in sorted(set(main)|set(base)) if k not in main or differences(main[k],base.get(k))[0]]
    complete=complete and not main_differences
    for version,m in matches.items():
        item=per[version];item.update(delta(matches[BASELINE],m));item['gain_count']=len(item['gain_ids']);item['loss_count']=len(item['loss_ids'])
        item['exclusive_vs_all_tested']=sorted(m-set().union(*(s for v,s in matches.items()if v!=version)))
        diff=[{'id':k,'facets':['only_candidate'] if k not in base else differences(base[k],outputs[version].get(k))[0]} for k in sorted(set(base)|set(outputs[version])) if k not in base or differences(base[k],outputs[version].get(k))[0]]
        item['outputs_different_vs_1_1_0']=diff;item['output_difference_count']=len(diff)
        bt={k for k,t in metadata[BASELINE]['types'].items()if t['equal']};ct={k for k,t in metadata[version]['types'].items()if t['equal']}
        item['type_gain_ids']=sorted(ct-bt);item['type_loss_ids']=sorted(bt-ct)
    same_sets=complete and all(x==matches[BASELINE] for x in matches.values())
    same_outputs=complete and all(not x['output_difference_count'] for x in per.values())
    pairs=[]
    for left,right in combinations(outputs,2):
        ids=set(outputs[left])|set(outputs[right])
        changed=[k for k in sorted(ids) if k not in outputs[left] or differences(outputs[left][k],outputs[right].get(k))[0]]
        pairs.append({'left':left,'right':right,'method_output_difference_count':len(changed),'different_method_ids':changed,
                      'same_exact_match_set':matches[left]==matches[right]})
    result={'schema_version':1,'baseline_version':BASELINE,'requested_versions':VERSIONS,'scope':'complete KairoLibrary, frozen identical sources/references/order/options',
        'original_sha256':original_sha,'original_method_count':len(oracle),'original_body_count':len(body_ids),
        'baseline_body_equal':len(matches[BASELINE]) if BASELINE in matches else None,'per_version':per,
        'complete_matrix':complete,'frozen_baseline_reproduces_main':not main_differences,'baseline_freeze_differences':main_differences,
        'match_sets_identical':same_sets,'all_method_outputs_identical':same_outputs,'pairwise_comparisons':pairs,'negative_controls':tests,
        'snapshot_receipt':str((snapshot/'input-receipt.json').relative_to(ROOT)),'snapshot_receipt_sha256':sha(snapshot/'input-receipt.json'),
        'inspector_sha256':sha(inspector),'inspector_source_sha256':sha(ROOT/'tools/ProbeInspector.cs'),
        'historical_compiler_identity':'UNKNOWN','main_sources_modified':False,'method_or_type_states_written':False,
        'normalized_assembly_fields':[],'post_compile_binary_edits':False,
        'conclusion':'INDISTINGUISHABLE on inspected method definitions and bodies' if same_outputs else 'Match sets identical but outputs distinguishable' if same_sets else 'DISTINGUISHABLE by exact-match identities' if complete else 'INCONCLUSIVE matrix failure'}
    write(report,result);print(json.dumps({'report':str(report.relative_to(ROOT)),'conclusion':result['conclusion'],'counts':{v:x.get('body_equal') for v,x in per.items()}}),flush=True)
    return 0 if complete else 1
if __name__=='__main__':raise SystemExit(main())
