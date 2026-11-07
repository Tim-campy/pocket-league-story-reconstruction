"""Prove exact staged C# bytes in isolated builds and record Git EOL conversions.

Official DLLs and ledgers are untouched. A skipped assembly needs staged source
bytes identical to its fresh verified working tree; EOL changes require a build.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys

from apply_il_rules import dump, read
from compare_kairo import differences, sha
from kairo_candidate import COMPILER_PATH
from metadata_compare import compare_paths
from round10_adopt import assert_production_selection, fresh_official

ROOT=Path(__file__).resolve().parents[1]
ASSEMBLIES=('KairoLibrary','Assembly-CSharp')


def staged(path):
    return subprocess.check_output(['git','show',':'+path],cwd=ROOT)


def digest(data):
    return hashlib.sha256(data).hexdigest()


def write_json(path,value):
    path.write_text(json.dumps(value,indent=2,ensure_ascii=True)+'\n',encoding='utf-8',newline='\n')


def source_proof(args):
    folder=ROOT / 'build/round10/runs' / args.run_label / 'git-source-proof'
    output=ROOT / 'reports' / ('round10-'+args.run_label+'-git-source-proof.json')
    folder.mkdir(parents=True,exist_ok=True)
    baseline=args.baseline_dir if args.baseline_dir.is_absolute() else ROOT / args.baseline_dir
    preflight={a:fresh_official(a) for a in ASSEMBLIES}
    result={'operation':'compile-exact-staged-CSharp-source-bytes','current_builds_modified':False,
        'method_or_type_states_modified':False,'run_label':args.run_label,'baseline_dir':str(baseline.resolve()),
        'selected_assemblies':args.assemblies,'assemblies':{}}
    for assembly in ASSEMBLIES:
        receipt=preflight[assembly]
        manifest=read((ROOT / receipt['build_artifact']).parent / 'build-manifest.json')
        mapping,sources={},[]
        for source in manifest['sources']:
            logical=source['path'];data=staged(logical)
            out=folder / 'sources' / logical
            out.parent.mkdir(parents=True,exist_ok=True);out.write_bytes(data)
            mapping[logical]=str(out)
            working=(ROOT / logical).read_bytes()
            sources.append({'path':logical,'working_tree_sha256':digest(working),'staged_sha256':digest(data),
                'identical_bytes':working==data,'only_CRLF_to_LF_conversion':working!=data and working.replace(b'\r\n',b'\n')==data})
        mp=folder / (assembly+'-source-map.json');write_json(mp,mapping)
        if assembly not in args.assemblies:
            if not all(s['identical_bytes'] for s in sources):
                raise ValueError('Skipped assembly has differing staged bytes; include it in --assemblies: '+assembly)
            result['assemblies'][assembly]={'source_files':sources,'compiled':False,
                'proof_kind':'exact staged bytes already proved by fresh official receipt',
                'body_changes_vs_verified_working_tree':[],'method_inventory_equal':True,
                'types_equal':receipt['types_equal'],'types_total':receipt['types_total'],
                'whole_structural_metadata_equal':True,'official_build_sha256':receipt['build_sha256']}
            continue
        label='round10-'+args.run_label+'-git-source-proof'
        command=[sys.executable,ROOT / 'tools' / ('build_kairo.py' if assembly=='KairoLibrary' else 'build_csharp.py'),
            '--label',label,'--source-map',mp]
        if assembly=='KairoLibrary':
            command += ['--compiler',ROOT / COMPILER_PATH]
        run=subprocess.run([str(x) for x in command],cwd=ROOT,capture_output=True)
        (folder / (assembly+'-build.log')).write_bytes(run.stdout+run.stderr)
        if run.returncode:
            raise RuntimeError('Staged source did not compile: '+assembly)
        build=ROOT / 'build' / (assembly+'-'+label)
        assert_production_selection(assembly,read(build / 'build-manifest.json'),baseline)
        binary=build / (assembly+'.dll')
        actual=dump(binary,folder / (assembly+'-methods.json'))
        expected={m['stable_id']:m for m in read((ROOT / receipt['build_artifact']).parent / 'rebuilt-methods.json')['selected_methods']}
        changed=[key for key,method in expected.items() if differences(method,actual.get(key))[0]]
        metadata=compare_paths(ROOT / 'original/extracted/assets/bin/Data/Managed' / (assembly+'.dll'),binary,folder / (assembly+'-metadata.json'))
        proof={'source_files':sources,'compiled':True,'proof_kind':'isolated compilation of exact staged source bytes',
            'body_changes_vs_verified_working_tree':changed,'method_inventory_equal':set(actual)==set(expected),
            'types_equal':metadata['types_equal'],'types_total':metadata['types_total'],
            'whole_structural_metadata_equal':metadata['whole_structural_metadata_equal'],
            'official_build_sha256':receipt['build_sha256'],'isolated_staged_build_sha256':sha(binary)}
        result['assemblies'][assembly]=proof
        if changed or not proof['method_inventory_equal'] or not metadata['whole_structural_metadata_equal']:
            write_json(output,result)
            raise ValueError('Git normalization changed bodies or definitions: '+assembly)
    for assembly in ASSEMBLIES:
        receipt=fresh_official(assembly)
        if receipt['build_sha256']!=preflight[assembly]['build_sha256']:
            raise ValueError('Official assembly changed during staged proof: '+assembly)
        for source in result['assemblies'][assembly]['source_files']:
            if digest(staged(source['path']))!=source['staged_sha256'] or sha(ROOT / source['path'])!=source['working_tree_sha256']:
                raise ValueError('Source changed during staged proof: '+source['path'])
    result['both_official_receipts_fresh_before_and_after']=True
    write_json(output,result)
    print(json.dumps({'staged_source_proof':True,'changed_bodies':0,
        'types_equal':sum(v['types_equal'] for v in result['assemblies'].values())}))


def content_manifest(args):
    name='reports/round10-'+args.run_label+'-git-content-provenance.json'
    files=subprocess.check_output(['git','diff','--cached','--name-only','-z'],cwd=ROOT).decode('utf-8').split('\0')
    rows=[]
    for path in sorted(p for p in files if p and p!=name):
        disk,index=(ROOT / path).read_bytes(),staged(path)
        conversion=disk!=index and disk.replace(b'\r\n',b'\n')==index
        if disk!=index and not conversion:
            raise ValueError('Unstaged content differs beyond Git EOL conversion: '+path)
        rows.append({'path':path,'measured_working_tree_sha256':digest(disk),'committed_content_sha256':digest(index),
            'conversion':'CRLF-to-LF by .gitattributes' if conversion else 'identical bytes'})
    proof=read(ROOT / 'reports' / ('round10-'+args.run_label+'-git-source-proof.json'))
    for assembly,data in proof['assemblies'].items():
        receipt=fresh_official(assembly)
        if receipt['build_sha256']!=data['official_build_sha256']:
            raise ValueError('Official assembly changed after staged proof: '+assembly)
        for source in data['source_files']:
            if digest(staged(source['path']))!=source['staged_sha256']:
                raise ValueError('Staged C# changed after build proof: '+source['path'])
    write_json(ROOT / name,{'operation':'Git-index-content-provenance','run_label':args.run_label,
        'note':'Evidence hashes identify measured working-tree bytes. This manifest identifies committed bytes when Git converts CRLF to LF; logical JSON data is unchanged.',
        'self_excluded_to_avoid_circular_hash':name,'files':rows})
    print(json.dumps({'manifest_files':len(rows),'CRLF_to_LF_conversions':sum(r['conversion']!='identical bytes' for r in rows)}))


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--run-label',default='part-a')
    parser.add_argument('--baseline-dir',type=Path,default=Path('build/round10/baseline'))
    parser.add_argument('--assemblies',nargs='+',choices=ASSEMBLIES,default=list(ASSEMBLIES))
    parser.add_argument('--manifest-only',action='store_true')
    args=parser.parse_args()
    if not re.fullmatch(r'[A-Za-z0-9][A-Za-z0-9._-]*',args.run_label):
        raise ValueError('Invalid run label')
    content_manifest(args) if args.manifest_only else source_proof(args)


if __name__=='__main__':
    main()
