"""Read-only structural/body comparison of external UnityEngine variants.

No decompilation or target execution. Differences cannot alone establish that
an APK DLL was modified: legitimate player/platform variants can add APIs too.
"""
from collections import Counter
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import differences,sha
from compiler_probes import UNITY

ROOT=Path(__file__).resolve().parents[1]


def inspect(path,label):
    folder=ROOT/'analysis/unity-structure';folder.mkdir(parents=True,exist_ok=True)
    output=folder/(label+'.json')
    with output.open('wb') as file:
        subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(path),'--structure'],stdout=file,check=True)
    return json.loads(output.read_text(encoding='utf-8-sig'))


def compare(original,candidate):
    old={m['stable_id']:m for m in original['selected_methods']};new={m['stable_id']:m for m in candidate['selected_methods']}
    ot={t['name']:t for t in original['types']};nt={t['name']:t for t in candidate['types']}
    counts=Counter();method_rows=[];facets=Counter()
    for key in sorted(old.keys() & new.keys()):
        delta,_=differences(old[key],new[key]);present='raw_body_hex' in old[key]
        counts['common_bodies' if present else 'common_declarations']+=1
        if not delta: counts['equal_bodies' if present else 'equal_declarations']+=1
        else: counts['different_bodies' if present else 'different_declarations']+=1;facets.update(delta)
        if delta: method_rows.append({'id':key,'body_present':present,'differences':delta})
    type_rows=[];all_subset=True
    for name in sorted(ot.keys() & nt.keys()):
        a,b=ot[name],nt[name];delta=[];subset=True
        for key in ('attributes','base_type','interfaces','custom_attributes','packing_size','class_size','generic_parameters'):
            if a[key]!=b[key]:delta.append(key);subset=False
        for key,identity in (('fields','id'),('properties','name'),('events','name')):
            ax={x[identity]:x for x in a[key]};bx={x[identity]:x for x in b[key]}
            if ax!=bx:delta.append(key)
            if not ax.keys()<=bx.keys() or any(bx.get(k)!=v for k,v in ax.items()):subset=False
        for key in ('methods','nested_types'):
            if a[key]!=b[key]:delta.append(key)
            if not set(a[key])<=set(b[key]):subset=False
        all_subset &= subset
        if delta:type_rows.append({'type':name,'differences':delta,'apk_members_are_subset_with_equal_shared_metadata':subset})
    apk_extra=sorted(old.keys()-new.keys());apk_extra_types=sorted(ot.keys()-nt.keys())
    subset=not apk_extra and not apk_extra_types and not method_rows and all_subset
    return {'counts':dict(counts),'common_types':len(ot.keys()&nt.keys()),'apk_type_count':len(ot),'installed_type_count':len(nt),
        'apk_method_count':len(old),'installed_method_count':len(new),'apk_only_types':apk_extra_types,
        'installed_only_types':sorted(nt.keys()-ot.keys()),'apk_only_methods':apk_extra,
        'installed_only_methods':sorted(new.keys()-old.keys()),'method_differences':method_rows,
        'method_difference_facets':dict(facets),'type_differences':type_rows,
        'apk_is_structural_subset_with_identical_shared_bodies':subset,
        'interpretation':'consistent with stripping of this candidate; build provenance still required' if subset else
            'not explained by pure stripping of this installed candidate; platform/revision/modification remains unresolved'}


def main():
    subprocess.run([sys.executable,str(ROOT/'tools/prepare_probe_inspector.py')],check=True,capture_output=True)
    source=ROOT/'original/extracted/assets/bin/Data/Managed/UnityEngine.dll'
    original=inspect(source,'apk')
    candidates={'Editor':UNITY/'Editor/Data/Managed/UnityEngine.dll',
        'WebPlayer':UNITY/'Editor/Data/PlaybackEngines/WebPlayer/Managed/UnityEngine.dll'}
    results={}
    for label,path in candidates.items():
        result=compare(original,inspect(path,label));result['installed_sha256']=sha(path);results[label]=result
    report={'apk_sha256':sha(source),'unity_version':'5.3.4f1','status':'EXTERNAL_UNVERIFIED',
        'comparison_scope':'type/member shapes and all common full method bodies with resolved metadata tokens; no decompilation',
        'variants':results,'normalized_assembly_fields':[],
        'limitation':'Only installed Editor and WebPlayer variants tested; official Android player variant not available'}
    (ROOT/'reports/UnityEngine-structural-comparison.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({name:{'counts':r['counts'],'apk_only_types':len(r['apk_only_types']),
        'apk_only_methods':len(r['apk_only_methods']),'installed_only_methods':len(r['installed_only_methods']),
        'pure_stripping_supported':r['apk_is_structural_subset_with_identical_shared_bodies']} for name,r in results.items()}))


if __name__=='__main__': main()
