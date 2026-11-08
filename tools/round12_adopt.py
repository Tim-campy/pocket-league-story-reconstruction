"""Adopt source candidates only after production proofs of BOTH target assemblies.

A selected assembly needs its real completed engine. An untouched assembly is
proved against its fresh official preflight receipt, never a fabricated engine.
Only tools/verify writes ledger states. Importing this module has no side effects.
"""
import argparse
import csv
import json
from pathlib import Path
import re
import subprocess
import sys

from apply_il_rules import dump, read, receipt_is_fresh
from compare_kairo import differences, sha
from kairo_candidate import COMPILER_PATH
from metadata_compare import compare_paths

ROOT = Path(__file__).resolve().parents[1]
ASSEMBLIES = ('KairoLibrary', 'Assembly-CSharp')


def project_path(path):
    value = Path(path)
    return value if value.is_absolute() else ROOT / value


def assert_same_bodies(expected, actual, purpose):
    changes = [key for key, method in expected.items() if differences(method, actual.get(key))[0]]
    if changes or set(actual) != set(expected):
        raise ValueError(purpose + ': changed/missing bodies ' + repr(changes)
                         + '; additional ' + repr(sorted(set(actual) - set(expected))))


def fresh_official(assembly):
    receipt = read(ROOT / 'build' / ('verification-' + assembly + '.json'))
    if not receipt_is_fresh(ROOT, receipt) or any(receipt.get(k) for k in ('regressions','body_regressions','type_regressions')):
        raise ValueError('Official receipt stale or regressive: ' + assembly)
    if receipt['types_equal'] != receipt['types_total']:
        raise ValueError('Official structural metadata differs: ' + assembly)
    return receipt


def assert_production_selection(assembly, manifest, baseline):
    selected = read(ROOT / 'config/toolchain.json')['assemblies'][assembly]
    flags = [flag for flag in manifest['flags'] if not flag.startswith(('/out:', '-out:'))]
    refs = lambda items: [(str(project_path(r['path']).resolve()).lower(), r['sha256']) for r in items]
    if flags != selected['flags'] or refs(manifest['references']) != refs(selected['reference_files']):
        raise ValueError('Production flags/references differ from selection: ' + assembly)
    if manifest['compiler_sha256'] != read(baseline / assembly / 'build-manifest.json')['compiler_sha256']:
        raise ValueError('Production compiler differs from frozen baseline: ' + assembly)


def load_engine(assembly, run_label, run_folder):
    engine = read(ROOT / 'reports' / ('round12-' + run_label + '-' + assembly + '-engine.json'))
    if not engine.get('completed') or not engine.get('adoption_eligible') or engine.get('body_losses'):
        raise ValueError(assembly + ' engine incomplete, ineligible or regressive')
    if sha(project_path(engine['final_binary'])) != engine['final_sha256']:
        raise ValueError('Engine binary changed: ' + assembly)
    folder = (run_folder / 'engine' / assembly).resolve()
    mapping = read(project_path(engine['source_map']))
    for logical, value in mapping.items():
        path = project_path(value).resolve()
        if not logical.startswith('src/' + assembly + '/') or not path.is_relative_to(folder / 'sources'):
            raise ValueError('Candidate source outside engine source tree: ' + logical)
        if sha(path) != engine['source_hashes'][logical]:
            raise ValueError('Candidate source changed: ' + logical)
    expected = {m['stable_id']:m for m in read(folder / 'final-methods.json')['selected_methods']}
    # The sidecar must describe the actual hash-checked final engine artifact.
    actual = dump(project_path(engine['final_binary']), folder / 'adoption-rechecked-final-methods.json')
    assert_same_bodies(expected, actual, 'Engine final sidecar vs binary ' + assembly)
    return engine, mapping, expected


def baseline_sources_unchanged(assembly, baseline):
    manifest = read(baseline / assembly / 'build-manifest.json')
    for row in manifest['sources']:
        logical = row['path']
        if sha(baseline / assembly / logical) != row['sha256'] or sha(ROOT / logical) != row['sha256']:
            raise ValueError('Source changed after frozen baseline: ' + logical)


def run_official(folder, log_name):
    run = subprocess.run([sys.executable, str(ROOT / 'tools/verify'), '--all'], cwd=ROOT, capture_output=True)
    (folder / log_name).write_bytes(run.stdout + run.stderr)
    if run.returncode not in (0, 1):
        raise RuntimeError('Official verifier failed; inspect ' + str(folder / log_name))


def collect_official(report, before, expected, baseline, folder, suffix=''):
    released=[]
    for assembly in ASSEMBLIES:
        receipt=fresh_official(assembly)
        receipt_path=ROOT / 'build' / ('verification-' + assembly + '.json')
        actual=dump(ROOT / receipt['build_artifact'], folder / (assembly + suffix + '-official-methods.json'))
        assert_same_bodies(expected[assembly], actual, 'Official vs expected ' + assembly)
        gains=[key for key,proof in receipt['methods'].items() if proof['state']=='matched' and before[key]['estado']!='matched']
        losses=[key for key,proof in receipt['methods'].items() if proof['state']!='matched' and before[key]['estado']=='matched']
        if losses:
            raise RuntimeError('Official matched regressions: ' + repr(losses))
        for key in report['assemblies'][assembly]['body_gains']:
            proof=next((p for p in receipt['methods'].values() if p['stable_id']==key), None)
            if not proof or not proof['body_equal']:
                raise RuntimeError('Engine body gain absent from official proof: ' + key)
        frozen=read(baseline / assembly / 'verification.json')
        released += [key for key in gains if frozen['methods'][key]['body_equal']]
        report['assemblies'][assembly].update(matched_gains=gains,matched_losses=losses,counts=receipt['counts'],
            official_sha256=receipt['build_sha256'],official_receipt_sha256=sha(receipt_path),
            types_equal=receipt['types_equal'],types_total=receipt['types_total'])
    report.update(official_verified=True,released_held=len(released),released_held_ids=released,
        matched_gains=sum(len(v['matched_gains']) for v in report['assemblies'].values()),
        matched_losses=sum(len(v['matched_losses']) for v in report['assemblies'].values()))


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--run-label',default='part-a')
    parser.add_argument('--baseline-dir',type=Path,default=Path('build/round12/baseline'))
    parser.add_argument('--assemblies',nargs='+',choices=ASSEMBLIES,default=list(ASSEMBLIES))
    parser.add_argument('--refresh-official-proof',action='store_true',help='Recheck cleanup; never recopy engine sources')
    parser.add_argument('--incremental',action='store_true',help='Later engine pass after previous official source proof')
    args=parser.parse_args()
    if not re.fullmatch(r'[A-Za-z0-9][A-Za-z0-9._-]*',args.run_label):
        raise ValueError('Invalid run label')
    if args.incremental and args.refresh_official_proof:
        raise ValueError('Incremental adoption and proof refresh are separate operations')
    baseline=project_path(args.baseline_dir).resolve()
    run_folder=ROOT / 'build/round12/runs' / args.run_label
    folder=run_folder / 'adoption'
    output=ROOT / 'reports' / ('round12-' + args.run_label + '-adoption.json')
    folder.mkdir(parents=True,exist_ok=True)
    before={row['id']:row for row in csv.DictReader((baseline / 'methods.csv').open(encoding='utf-8-sig',newline=''))}
    # Both mains must already have valid evidence before candidates are examined.
    selected=set(args.assemblies)
    # Cleanup intentionally invalidates selected source hashes. Its refresh is
    # proved against the immutable engine inventory by a new official build.
    preflight={a:fresh_official(a) for a in ASSEMBLIES
               if not args.refresh_official_proof or a not in selected}
    engines,mapping,expected={},{},{}
    for assembly in ASSEMBLIES:
        if assembly in selected:
            engine,candidates,methods=load_engine(assembly,args.run_label,run_folder)
            engines[assembly]=engine;mapping.update(candidates);expected[assembly]=methods
        else:
            expected[assembly]=dump(ROOT / preflight[assembly]['build_artifact'],folder / (assembly + '-untouched-preflight-methods.json'))
    if args.refresh_official_proof:
        report=read(output)
        if set(report['selected_assemblies'])!=selected:
            raise ValueError('Refresh selection differs from adoption proof')
        run_official(folder,'official-full-verify-after-cleanup.log')
        collect_official(report,before,expected,baseline,folder,'-after-cleanup')
        report['post_adoption_cleanup']={'official_full_verification':True,'body_changes_vs_expected':0}
    else:
        report={'operation':'source-only-production-proof-and-official-verification',
            'run_label':args.run_label,'baseline_dir':str(baseline),
            'baseline_commit':read(baseline/'snapshot.json')['commit'],
            'selected_assemblies':sorted(selected),'official_verified':False,'assemblies':{},'changed_sources':[]}
        if args.incremental:
            previous=read(output)
            if not previous.get('official_verified'):
                raise ValueError('No previous official adoption proof')
            for assembly in ASSEMBLIES:
                if preflight[assembly]['build_sha256']!=previous['assemblies'][assembly]['official_sha256']:
                    raise ValueError('Main changed outside previous official proof: ' + assembly)
            report['previous_adoption']=previous
        for assembly in ASSEMBLIES:
            if not args.incremental:
                baseline_sources_unchanged(assembly,baseline)
            label='round12-' + args.run_label + '-production-proof'
            command=[sys.executable,str(ROOT / 'tools' / ('build_kairo.py' if assembly=='KairoLibrary' else 'build_csharp.py')),'--label',label]
            if assembly=='KairoLibrary':
                command += ['--compiler',str(ROOT / COMPILER_PATH)]
            engine=engines.get(assembly)
            if engine:
                command += ['--source-map',str(project_path(engine['source_map']))]
            run=subprocess.run(command,cwd=ROOT,capture_output=True)
            (folder / (assembly + '-production-build.log')).write_bytes(run.stdout+run.stderr)
            if run.returncode:
                raise RuntimeError('Production proof failed to compile: ' + assembly)
            build=ROOT / 'build' / (assembly + '-' + label)
            manifest=read(build / 'build-manifest.json')
            assert_production_selection(assembly,manifest,baseline)
            binary=build / (assembly + '.dll')
            actual=dump(binary,folder / (assembly + '-production-methods.json'))
            assert_same_bodies(expected[assembly],actual,'Production proof ' + assembly)
            metadata=compare_paths(ROOT / 'original/extracted/assets/bin/Data/Managed' / (assembly + '.dll'),binary,folder / (assembly + '-production-metadata.json'))
            if not metadata['whole_structural_metadata_equal']:
                raise ValueError('Production metadata differs: ' + assembly)
            report['assemblies'][assembly]={'engine_executed':bool(engine),'production_body_changes_vs_expected':[],
                'production_sha256':sha(binary),'production_manifest':str((build / 'build-manifest.json').relative_to(ROOT)),
                'production_manifest_sha256':sha(build / 'build-manifest.json'),
                'source_map':engine['source_map'] if engine else None,'body_gains':engine['body_gains'] if engine else [],
                'untouched_preflight_sha256':preflight[assembly]['build_sha256'] if not engine else None,
                'types_equal':metadata['types_equal']}
        # Both real production builds have passed before the first main-source edit.
        for logical,candidate in mapping.items():
            source=project_path(candidate);current=ROOT / logical
            if source.read_bytes()==current.read_bytes():
                continue
            backup=folder / ('pre-incremental-adoption' if args.incremental else 'pre-adoption') / logical
            backup.parent.mkdir(parents=True,exist_ok=True);backup.write_bytes(current.read_bytes())
            current.write_bytes(source.read_bytes());report['changed_sources'].append(logical)
        run_official(folder,'official-full-verify.log')
        collect_official(report,before,expected,baseline,folder)
    output.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'official_verified':report['official_verified'],'matched_gains':report['matched_gains'],'matched_losses':report['matched_losses']}))


if __name__=='__main__':
    main()
