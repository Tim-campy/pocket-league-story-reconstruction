"""Recover omitted implicit static constructors in two isolated C# candidates.

Original BeforeFieldInit and field order are checked first. Only a default RHS
on an existing static field is restored; no explicit static constructor or new
generated helper class is written. Nothing in src/ or the ledger is changed.
"""
from collections import Counter
import json
from pathlib import Path
import subprocess
import sys
from compare_csharp_stdlib import prepare_inspector,validate,inspect,compare
from compare_kairo import differences
from build_csharp import sha

ROOT=Path(__file__).resolve().parents[1]
TARGETS={'form.FormManager':('src/Assembly-CSharp/form/FormManager.cs',
    'private static FormManager instance_;','private static FormManager instance_ = null;'),
    'system.billing.ui.StoreBase':('src/Assembly-CSharp/system/billing/ui/StoreBase.cs',
    'private static KairoService ks_;','private static KairoService ks_ = null;')}


def main():
    oracle_path=ROOT/'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll'
    if sha(oracle_path)!='3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949':
        raise ValueError('Original Assembly-CSharp identity changed')
    baseline_folder=ROOT/'build/Assembly-CSharp-profile-default-indexof'
    baseline_manifest=validate(baseline_folder)
    original=json.loads((ROOT/'build/Assembly-CSharp-mono-smcs/original-methods.json').read_text(encoding='utf-8-sig'))
    baseline=json.loads((baseline_folder/'profile-methods.json').read_text(encoding='utf-8-sig'))
    old_types={t['name']:t for t in original['types']};before_types={t['name']:t for t in baseline['types']}
    old_methods={m['stable_id']:m for m in original['selected_methods']}
    baseline_equal,baseline_counts,_=compare(original,baseline)
    folder=ROOT/'analysis/variants/app-static-initializers';folder.mkdir(parents=True,exist_ok=True)
    root_map=json.loads((ROOT/'analysis/variants/app-stdlib-indexof/source-map.json').read_text(encoding='utf-8'))
    replacements={};observations={}
    for target,(logical,old,new) in TARGETS.items():
        before=old_types[target];current=before_types[target]
        if not (before['attributes']&0x100000) or current['attributes']!=before['attributes']:
            raise ValueError('This field-initializer test requires matching BeforeFieldInit types')
        constructor=old_methods[target+'::.cctor()->System.Void']
        if constructor['instructions']!=[{'offset':0,'opcode':'ret','operand':None}]:raise ValueError('Original cctor is not an empty implicit initializer')
        source=(ROOT/logical).read_bytes().decode('utf-8-sig')
        if source.count(old)!=1:raise ValueError('Unexpected field source shape')
        destination=folder/logical;destination.parent.mkdir(parents=True,exist_ok=True)
        destination.write_bytes(source.replace(old,new).encode('utf-8'))
        replacements[target]=(logical,str(destination))
        observations[target]={'original_attributes':before['attributes'],'before_attributes':current['attributes'],
            'original_before_field_init':True,'candidate_edit':{'old':old,'new':new},
            'original_cctor_header':constructor['header'],'original_cctor_method_attributes':constructor['method_attributes'],
            'explicit_static_constructor_added':False}
    inspector=prepare_inspector();results={}
    for label,targets in [('profile-cctor-formmanager',['form.FormManager']),
        ('profile-cctor-storebase',['system.billing.ui.StoreBase']),('profile-cctor-combined',list(TARGETS))]:
        mapping=dict(root_map);mapping.update(dict(replacements[t] for t in targets))
        map_path=folder/(label+'-source-map.json');map_path.write_text(json.dumps(mapping,indent=2)+'\n',encoding='utf-8')
        command=[sys.executable,str(ROOT/'tools/build_csharp.py'),'--compiler','smcs','--stdlib','default',
            '--optimize','+','--debug','-','--label',label,'--source-map',str(map_path)]
        completed=subprocess.run(command,cwd=ROOT,capture_output=True,text=True,encoding='utf-8',errors='replace')
        if completed.returncode:raise RuntimeError(completed.stdout+completed.stderr)
        build_folder=ROOT/'build'/('Assembly-CSharp-'+label);manifest=validate(build_folder)
        if manifest['references']!=baseline_manifest['references']:raise ValueError('Reference inputs changed')
        flags=lambda item:[f for f in item['flags'] if not f.startswith('-out:')]
        if flags(manifest)!=flags(baseline_manifest):raise ValueError('Compiler options changed')
        built=inspect(inspector,build_folder/'Assembly-CSharp.dll',build_folder/'profile-methods.json')
        equal,counts,_=compare(original,built);types={t['name']:t for t in built['types']}
        methods={m['stable_id']:m for m in built['selected_methods']}
        target_results={}
        for target in targets:
            key=target+'::.cctor()->System.Void';delta,percent=differences(old_methods[key],methods.get(key))
            target_results[target]={'cctor_equal':not delta,'differences':delta,'percent':percent,
                'candidate_attributes':types[target]['attributes'],
                'attributes_equal':types[target]['attributes']==old_types[target]['attributes'],
                'before_field_init':bool(types[target]['attributes']&0x100000),
                'type_shape_equal':types[target]==old_types[target],
                'method_order_equal':types[target]['methods']==old_types[target]['methods'],
                'field_order_equal':[f['name'] for f in types[target]['fields']]==[f['name'] for f in old_types[target]['fields']]}
        gained=sorted(equal-baseline_equal);lost=sorted(baseline_equal-equal)
        results[label]={'counts':counts,'targets':target_results,'source_map':str(map_path),
            'build_sha256':manifest['output_sha256'],'compiler_sha256':manifest['compiler_sha256'],
            'runtime_sha256':manifest['runtime_sha256'],'gain_count':len(gained),'loss_count':len(lost),
            'flags':manifest['flags'],'sources':manifest['sources'],'references':manifest['references'],
            'gained_body_ids':gained,'lost_body_ids':lost,
            'assembly_reference_identities_match_original':set(built['references'])==set(original['references']),
            'assembly_reference_order_matches_original':built['references']==original['references']}
    report={'scope':'two missing App static constructors, default field initializer candidates only',
        'baseline_counts':baseline_counts,'baseline_sha256':baseline_manifest['output_sha256'],
        'original_sha256':sha(ROOT/'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll'),
        'observations':observations,'candidates':results,'applied_to_main':False,'ledger_updated':False,
        'inspector_source_sha256':sha(ROOT/'tools/ProbeInspector.cs'),'normalized_assembly_fields':[]}
    (ROOT/'reports/Assembly-CSharp-static-initializers.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({name:{k:value[k] for k in ('counts','gain_count','loss_count','targets')} for name,value in results.items()}))


if __name__=='__main__':main()
