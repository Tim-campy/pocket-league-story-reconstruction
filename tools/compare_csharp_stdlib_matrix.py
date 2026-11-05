"""Recheck frontend and optimize candidates after fixing core-library binding.

Uses the isolated AppData source with the original two-argument IndexOf call.
Does not change the selected baseline, sources, method ledger or receipts.
"""
from collections import Counter
import json
from pathlib import Path
from compare_csharp_stdlib import prepare_inspector,validate,inspect,compare
from compare_kairo import differences
from build_csharp import sha

ROOT=Path(__file__).resolve().parents[1]


def main():
    baseline_label='profile-default-indexof';baseline_folder=ROOT/'build'/('Assembly-CSharp-'+baseline_label)
    baseline_manifest=validate(baseline_folder)
    baseline=json.loads((baseline_folder/'profile-methods.json').read_text(encoding='utf-8-sig'))
    oracle_path=ROOT/'original/extracted/assets/bin/Data/Managed/Assembly-CSharp.dll'
    if sha(oracle_path)!='3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949':raise ValueError('Original changed')
    original=json.loads((ROOT/'build/Assembly-CSharp-mono-smcs/original-methods.json').read_text(encoding='utf-8-sig'))
    baseline_equal,baseline_counts,_=compare(original,baseline)
    inspector=prepare_inspector();results={}
    previous={m['stable_id']:m for m in baseline['selected_methods']}
    for label in ('profile-default-gmcs','profile-default-smcs-noopt','profile-default-gmcs-noopt','profile-default-mcs34'):
        folder=ROOT/'build'/('Assembly-CSharp-'+label)
        manifest=json.loads((folder/'build-manifest.json').read_text(encoding='utf-8'))
        if manifest['sources']!=baseline_manifest['sources'] or manifest['references']!=baseline_manifest['references']:
            raise ValueError('Matrix needs identical sources, order and explicit references')
        flags=lambda item:[f for f in item['flags'] if not f.startswith(('-optimize','-out:'))]
        if flags(manifest)!=flags(baseline_manifest):raise ValueError('Other compiler flags differ')
        item={key:manifest[key] for key in ('compiler_sha256','runtime_sha256','flags','stdlib_binding','exit_code')}
        if manifest['exit_code']:
            log=(folder/'compiler.log').read_text(encoding='utf-8')
            errors=[line for line in log.splitlines() if 'error CS' in line]
            item.update({'status':'BUILD_FAILED_NOT_COMPARED','error_count':len(errors),
                'error_codes':dict(Counter(line.split('error ')[1].split(':')[0] for line in errors))})
        else:
            validate(folder)
            built=inspect(inspector,folder/'Assembly-CSharp.dll',folder/'profile-methods.json')
            equal,counts,delta=compare(original,built);candidate={m['stable_id']:m for m in built['selected_methods']}
            changed=[key for key,method in previous.items() if differences(method,candidate.get(key))[0]]
            gained=sorted(equal-baseline_equal);lost=sorted(baseline_equal-equal)
            item.update({'status':'FULL_BUILD_COMPARED','counts':counts,'gain_count':len(gained),'loss_count':len(lost),
                'gained_body_ids':gained,'lost_body_ids':lost,'method_outputs_changed_from_smcs_optimize_plus':len(changed),
                'changed_method_ids':changed,'assembly_references':built['references'],
                'assembly_reference_identities_match_original':set(built['references'])==set(original['references']),
                'assembly_reference_order_matches_original':built['references']==original['references'],
                'build_sha256':manifest['output_sha256']})
        item['target_profile_qualified']=item.get('assembly_reference_identities_match_original')
        item['interpretation']='build failed before body comparison' if manifest['exit_code'] else (
            'same target framework profile; method-body comparison applicable' if item['target_profile_qualified'] else
            'different core-library identity; this is a profile mismatch, not evidence against this compiler generation')
        results[label]=item
    report={'scope':'four isolated whole-assembly controls with stdlib default and identical IndexOf-restored C# sources',
        'baseline_label':baseline_label,'baseline_counts':baseline_counts,'baseline_sha256':baseline_manifest['output_sha256'],
        'candidates':results,'compiler_historical_identity':'UNKNOWN','normalized_assembly_fields':[],
        'applied_to_main':False,'ledger_updated':False,'baseline_receipts_updated':False,
        'inspector_source_sha256':sha(ROOT/'tools/ProbeInspector.cs')}
    (ROOT/'reports/Assembly-CSharp-stdlib-matrix.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({label:{key:item[key] for key in ('status','error_count','counts','gain_count','loss_count',
        'method_outputs_changed_from_smcs_optimize_plus','assembly_reference_identities_match_original',
        'assembly_reference_order_matches_original') if key in item} for label,item in results.items()}))


if __name__=='__main__':main()
