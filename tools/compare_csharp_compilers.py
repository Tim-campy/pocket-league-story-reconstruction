"""Summarize full Assembly-CSharp candidate builds from identical source bytes."""
from collections import Counter
import json
from pathlib import Path
from build_csharp import sha
from compare_kairo import differences

ROOT=Path(__file__).resolve().parents[1]


def main():
    candidates={};sources=None;sidecars={}
    for name in ('mono-smcs','mono-smcs-noopt','mono-gmcs','mono-mcs34'):
        folder=ROOT/'build'/('Assembly-CSharp-'+name)
        manifest=json.loads((folder/'build-manifest.json').read_text(encoding='utf-8'))
        if sources is None:sources=manifest['sources']
        if sources!=manifest['sources']:raise ValueError('Candidate source/order mismatch')
        for source in sources:
            if sha(ROOT/source['path'])!=source['sha256']:raise ValueError('Source changed')
        item={k:manifest[k] for k in ('compiler_sha256','runtime_sha256','flags','exit_code')}
        if manifest['exit_code']==0:
            if sha(folder/'Assembly-CSharp.dll')!=manifest['output_sha256']:raise ValueError('Candidate artifact changed')
            sidecar=json.loads((folder/'verification-sidecar.json').read_text(encoding='utf-8'))
            if sidecar['build_sha256']!=manifest['output_sha256']:raise ValueError('Stale candidate verification')
            sidecars[name]=sidecar
            item.update({'counts':sidecar['counts'],'extra_method_ids':sidecar['extra_method_ids']})
        else:
            item['compiler_error_codes']=sorted({line.split('error ')[1].split(':')[0] for line in (folder/'compiler.log').read_text(encoding='utf-8').splitlines() if 'error CS' in line})
        candidates[name]=item
    counts=Counter();modern_only=[]
    for key,method in sidecars['mono-smcs']['methods'].items():
        if not method['body_present']:continue
        a=method['body_equal'];b=sidecars['mono-mcs34']['methods'][key]['body_equal']
        counts['both_match' if a and b else 'only_smcs_matches' if a else 'only_mcs34_matches' if b else 'neither_matches']+=1
        if b and not a:modern_only.append(key)
    def methods(label):
        return {m['stable_id']:m for m in json.loads((ROOT/'build'/('Assembly-CSharp-'+label)/'rebuilt-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']}
    opt,nonopt=methods('mono-smcs'),methods('mono-smcs-noopt')
    changed=[key for key,method in opt.items() if differences(method,nonopt.get(key))[0]]
    report={'scope':'complete Assembly-CSharp candidate builds from identical ordered recovered C# sources',
        'source_files':len(sources),'candidates':candidates,'smcs_vs_mcs34_original_body_matches':dict(counts),
        'mcs34_only_original_bodies':modern_only,
        'smcs_optimize_flag_method_output_differences':len(changed),'smcs_optimize_flag_changed_methods':changed,
        'selected_candidate':'mono-smcs','historical_compiler_identity':'UNKNOWN',
        'known_source_workaround':'AppData.Draw_helpMessage uses IndexOf(string), original calls IndexOf(string,int); this method remains divergent',
        'ledger_updated':False,'normalized_assembly_fields':[]}
    (ROOT/'reports/Assembly-CSharp-compiler-comparison.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'candidate_body_counts':{k:v.get('counts',{}).get('body_equal') for k,v in candidates.items()},
        'smcs_vs_mcs34':dict(counts),'optimize_method_differences':len(changed)}))


if __name__=='__main__':main()
