"""Evaluate isolated complete C# variant builds before selecting any method.

Does not edit src/, the method ledger or compiled assemblies. Full body matches
are measured independently of source ownership and metadata/type completion.
"""
from collections import Counter
import argparse
import csv
import hashlib
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import differences
from compiler_probes import ROSLYN,UNITY

ROOT=Path(__file__).resolve().parents[1]
VARIANTS={
    'control_flow':('ilspy-control_flow-ordered','variant-control_flow-ordered'),
    'conservative':('ilspy-conservative_safe-ordered','variant-conservative_safe-ordered'),
    'expressions':('ilspy-expressions_safe2-ordered','variant-expressions_safe2-ordered'),
    'codemerx':('codemerx-compatible-ordered','variant-codemerx_compatible-ordered'),
}


def digest(path):return hashlib.sha256(path.read_bytes()).hexdigest()


def read(path):return json.loads(path.read_text(encoding='utf-8-sig'))


def parse_sources(selected,folder,label,require_valid=True):
    folder.mkdir(parents=True,exist_ok=True)
    vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir())
    profile=UNITY/'Editor/Data/Mono/lib/mono/2.0'
    references=[profile/'mscorlib.dll',profile/'System.Core.dll',
        ROOT/'original/extracted/assets/bin/Data/Managed/UnityEngine.dll',profile/'System.dll']
    manifest=folder/(label+'-parse-input.json');output=folder/(label+'-members.json')
    manifest.write_text(json.dumps({'sources':[str(p) for p in selected.values()],
        'references':[str(p) for p in references]},indent=2)+'\n',encoding='utf-8')
    with output.open('wb') as file:
        result=subprocess.run([str(vs/'dotnet/net10.0/runtime/dotnet.exe'),str(ROOT/'build/source-recovery/SourceRecovery.dll'),
            'members',str(manifest)],stdout=file,stderr=subprocess.PIPE)
    if result.returncode:raise RuntimeError(result.stderr.decode('utf-8',errors='replace')[-3000:])
    document=read(output)
    if require_valid and document['errors']:raise ValueError('Source semantic errors: '+json.dumps(document['errors']))
    return document


def inspect(binary,folder):
    output=folder/'methods.json';cache=folder/'methods-cache.json'
    identity={'artifact_sha256':digest(binary),'inspector_sha256':digest(ROOT/'build/tools/ProbeInspector.exe')}
    if not (output.is_file() and cache.is_file() and read(cache)==identity):
        with output.open('wb') as file:
            result=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--structure'],stdout=file,stderr=subprocess.PIPE)
        if result.returncode:raise RuntimeError(result.stderr.decode('utf-8',errors='replace'))
        cache.write_text(json.dumps(identity,indent=2)+'\n',encoding='utf-8')
    return read(output)


def selected_sources(mapping):
    order=[p for p in (ROOT/'config/compile-order/KairoLibrary.txt').read_text(encoding='utf-8').splitlines() if p and not p.startswith('#')]
    if 'src/KairoLibrary/AssemblyInfo.cs' not in order:order.append('src/KairoLibrary/AssemblyInfo.cs')
    return {logical:Path(mapping[logical]).resolve() if logical in mapping else ROOT/logical for logical in order}


def method_index(document,logical_of):
    result={}
    for source in document['sources']:
        path=Path(source['path']);logical=logical_of[str(path)]
        for type_record in source['types']:
            for member in type_record['members']:
                for key in member['method_ids']:
                    if key in result:raise ValueError('Ambiguous source method '+key)
                    result[key]={'input_path':str(path.relative_to(ROOT)),'logical_path':logical,
                        'input_sha256':digest(path),'type':type_record['name'],'member':member}
    return result


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--variant',choices=list(VARIANTS),action='append')
    parser.add_argument('--build',action='store_true');args=parser.parse_args()
    original=read(ROOT/'build/KairoLibrary-roslyn-1.3.2/original-methods.json')['selected_methods']
    receipt=read(ROOT/'build/verification-KairoLibrary.json')
    with (ROOT/'config/methods.csv').open(encoding='utf-8',newline='') as file:
        rows={r['token']:r for r in csv.DictReader(file) if r['assembly']=='KairoLibrary'}
    token=lambda m:'0x'+format(0x06000000|int(m['token'].split('0x')[1].rstrip(']'),16),'08x')
    original_ids={m['stable_id']:rows[token(m)]['id'] for m in original}
    baseline_equal={key for key,identifier in original_ids.items() if receipt['methods'][identifier]['body_equal']}
    baseline_matched={key for key,identifier in original_ids.items() if receipt['methods'][identifier]['state']=='matched'}
    reports={};union=set()
    for label in args.variant or list(VARIANTS):
        source_folder,build_label=VARIANTS[label]
        map_path=ROOT/'analysis/variants'/source_folder/'source-map.json';folder=ROOT/'build'/('KairoLibrary-'+build_label)
        if not map_path.is_file():reports[label]={'status':'MISSING_SOURCE_MAP'};continue
        if args.build:
            result=subprocess.run([sys.executable,str(ROOT/'tools/build_kairo.py'),'--compiler',str(ROOT/'analysis/tools/roslyn-1.3.2/tools/csc.exe'),
                '--label',build_label,'--source-map',str(map_path)],cwd=ROOT,capture_output=True,text=True,encoding='utf-8')
            if result.returncode:reports[label]={'status':'BUILD_FAILED','log':(folder/'compiler.log').relative_to(ROOT).as_posix()};continue
        manifest=read(folder/'build-manifest.json')
        if manifest['exit_code']:reports[label]={'status':'BUILD_FAILED'};continue
        if digest(folder/'KairoLibrary.dll')!=manifest['output_sha256']:raise ValueError('Variant artifact changed')
        for source in manifest['sources']:
            if digest(Path(source.get('input_path',str(ROOT/source['path']))))!=source['sha256']:raise ValueError('Variant input changed')
        document=inspect(folder/'KairoLibrary.dll',folder);built={m['stable_id']:m for m in document['selected_methods']}
        mapping=read(map_path);selected=selected_sources(mapping)
        semantic=parse_sources(selected,folder,'source');source_members=method_index(semantic,{str(path):logical for logical,path in selected.items()})
        counts=Counter();candidates=[];all_equal=[];main_losses=[]
        for method in original:
            key=method['stable_id'];delta,_=differences(method,built.get(key))
            if 'raw_body_hex' not in method:continue
            if delta:
                counts['different_or_missing']+=1
                if key in baseline_equal:main_losses.append(key)
                continue
            counts['body_equal']+=1;all_equal.append(key)
            if key not in baseline_equal:
                counts['new_equal_bodies']+=1;union.add(key)
                if key in source_members:
                    counts['new_equal_with_source_member']+=1
                    candidates.append(dict({'id':original_ids[key],'stable_id':key},**source_members[key]))
        reports[label]={'status':'PASS','counts':dict(counts),'source_map':map_path.relative_to(ROOT).as_posix(),
            'build_manifest':(folder/'build-manifest.json').relative_to(ROOT).as_posix(),
            'artifact_sha256':manifest['output_sha256'],'full_variant_would_lose_body_matches':len(main_losses),
            'body_equal_ids':all_equal,'candidates':candidates,
            'scope':'isolated whole-build evaluation; no main-source adoption or ledger promotion'}
        print(json.dumps({'variant':label,'counts':dict(counts),'losses_if_whole_variant_adopted':len(main_losses),
            'first_candidates':[c['stable_id'] for c in candidates[:5]]}),flush=True)
    report={'baseline_body_equal':len(baseline_equal),'baseline_matched':len(baseline_matched),
        'baseline_build_sha256':receipt['build_sha256'],'union_new_equal_bodies':len(union),
        'variants':reports,'applied_to_main':False,'promoted_states':False}
    (ROOT/'reports/KairoLibrary-variant-evaluation.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')


if __name__=='__main__':main()
