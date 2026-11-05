"""Compare isolated full App builds with and without -nostdlib.

Read-only body comparison. Does not change source, ledgers or baseline receipts.
Uses a private inspector executable so concurrent Kairo verification can rebuild
its own shared inspector without executable-file locks.
"""
from collections import Counter
import json
import os
from pathlib import Path
import shutil
import subprocess
from build_csharp import sha
from compare_kairo import differences
from compiler_probes import UNITY

ROOT=Path(__file__).resolve().parents[1]


def prepare_inspector():
    folder=ROOT/'build/Assembly-CSharp-profile-tools';folder.mkdir(parents=True,exist_ok=True)
    compiler=Path(os.environ.get('WINDIR','C:/Windows'))/'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
    cecil=UNITY/'Editor/Data/Managed/Mono.Cecil.dll'
    subprocess.run([str(compiler),'/nologo','/target:exe','/out:'+str(folder/'ProbeInspector.exe'),
        '/r:'+str(cecil),'/r:System.Web.Extensions.dll',str(ROOT/'tools/ProbeInspector.cs')],check=True,capture_output=True)
    shutil.copy2(cecil,folder/'Mono.Cecil.dll')
    return folder/'ProbeInspector.exe'


def inspect(inspector,path,output):
    with output.open('wb') as file:
        subprocess.run([str(inspector),str(path),'--structure'],check=True,stdout=file,stderr=subprocess.PIPE)
    return json.loads(output.read_text(encoding='utf-8-sig'))


def validate(folder):
    manifest=json.loads((folder/'build-manifest.json').read_text(encoding='utf-8'))
    if manifest['exit_code'] or sha(folder/'Assembly-CSharp.dll')!=manifest['output_sha256']:raise ValueError('Build failed or artifact changed')
    if sha(Path(manifest['compiler']))!=manifest['compiler_sha256']:raise ValueError('Compiler changed')
    if sha(Path(manifest['runtime']))!=manifest['runtime_sha256']:raise ValueError('Runtime changed')
    for source in manifest['sources']:
        path=Path(source['input_path']) if 'input_path' in source else ROOT/source['path']
        if sha(path)!=source['sha256']:raise ValueError('Source changed: '+source['path'])
    for reference in manifest['references']:
        if sha(Path(reference['path']))!=reference['sha256']:raise ValueError('Reference changed')
    return manifest


def compare(original,built):
    candidate={m['stable_id']:m for m in built['selected_methods']}
    if len(candidate)!=len(built['selected_methods']):raise ValueError('Ambiguous method IDs')
    equal=set();counts=Counter();differences_by_id={}
    for method in original['selected_methods']:
        delta,_=differences(method,candidate.get(method['stable_id']))
        if 'raw_body_hex' not in method:
            counts['declarations_without_body']+=1;counts['declaration_equal']+=not delta
            continue
        counts['bodies']+=1
        if not delta:equal.add(method['stable_id'])
        else:differences_by_id[method['stable_id']]=delta
    counts['body_equal']=len(equal);counts['body_different_or_missing']=len(differences_by_id)
    counts['missing_methods']=len({m['stable_id'] for m in original['selected_methods']}-set(candidate))
    counts['extra_methods']=len(set(candidate)-{m['stable_id'] for m in original['selected_methods']})
    return equal,dict(counts),differences_by_id


def main():
    baseline_folder=ROOT/'build/Assembly-CSharp-mono-smcs'
    baseline_manifest=validate(baseline_folder)
    oracle_path=ROOT/'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll'
    if sha(oracle_path)!='3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949':raise ValueError('Original changed')
    original=json.loads((baseline_folder/'original-methods.json').read_text(encoding='utf-8-sig'))
    baseline=json.loads((baseline_folder/'rebuilt-methods.json').read_text(encoding='utf-8-sig'))
    baseline_equal,baseline_counts,_=compare(original,baseline)
    inspector=prepare_inspector();results={}
    for label in ('profile-default','profile-default-indexof'):
        folder=ROOT/'build'/('Assembly-CSharp-'+label);manifest=validate(folder)
        if manifest['references']!=baseline_manifest['references']:raise ValueError('Explicit references changed')
        if manifest['compiler_sha256']!=baseline_manifest['compiler_sha256'] or manifest['runtime_sha256']!=baseline_manifest['runtime_sha256']:
            raise ValueError('Compiler/runtime changed')
        flags=lambda item:[f for f in item['flags'] if f!='-nostdlib' and not f.startswith('-out:')]
        if flags(manifest)!=flags(baseline_manifest):raise ValueError('Other compiler flags changed')
        built=inspect(inspector,folder/'Assembly-CSharp.dll',folder/'profile-methods.json')
        equal,counts,delta=compare(original,built)
        gained=sorted(equal-baseline_equal);lost=sorted(baseline_equal-equal)
        previous={m['stable_id']:m for m in baseline['selected_methods']}
        current={m['stable_id']:m for m in built['selected_methods']}
        changed=[key for key,method in previous.items() if differences(method,current.get(key))[0]]
        source_changes=[s['path'] for s,b in zip(manifest['sources'],baseline_manifest['sources']) if s['path']!=b['path'] or s['sha256']!=b['sha256']]
        results[label]={'counts':counts,'gained_body_ids':gained,'lost_body_ids':lost,'gain_count':len(gained),'loss_count':len(lost),
            'source_changes':source_changes,'method_outputs_changed_from_baseline':len(changed),'changed_method_ids':changed,
            'assembly_references':built['references'],'assembly_reference_order_matches_original':built['references']==original['references'],
            'assembly_reference_identities_match_original':set(built['references'])==set(original['references']),
            'build_sha256':manifest['output_sha256'],'compiler_sha256':manifest['compiler_sha256'],
            'runtime_sha256':manifest['runtime_sha256'],'flags':manifest['flags'],'stdlib_binding':manifest['stdlib_binding'],
            'remaining_difference_facets':dict(Counter(facet for facets in delta.values() for facet in facets))}
    report={'scope':'complete Assembly-CSharp isolated profile-option comparison; no target execution',
        'original_sha256':sha(oracle_path),'baseline_sha256':baseline_manifest['output_sha256'],
        'baseline_counts':baseline_counts,'baseline_flags':baseline_manifest['flags'],'candidates':results,
        'inspector_source_sha256':sha(ROOT/'tools/ProbeInspector.cs'),
        'cecil_sha256':sha(UNITY/'Editor/Data/Managed/Mono.Cecil.dll'),
        'profile_mscorlib_sha256':sha(UNITY/'Editor/Data/Mono/lib/mono/unity/mscorlib.dll'),
        'explicit_mscorlib_sha256':baseline_manifest['references'][0]['sha256'],
        'applied_to_main':False,'ledger_updated':False,'baseline_receipts_updated':False,'normalized_assembly_fields':[]}
    (ROOT/'reports/Assembly-CSharp-stdlib-comparison.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({name:{k:item[k] for k in ('counts','gain_count','loss_count','source_changes',
        'method_outputs_changed_from_baseline','assembly_reference_identities_match_original','assembly_reference_order_matches_original')} for name,item in results.items()}))


if __name__=='__main__':main()
