"""Reproduce the missed Receipt hypothesis without changing project sources/state."""
import json,subprocess,sys
from pathlib import Path
from apply_il_rules import dump,read
from compare_kairo import differences,sha
ROOT=Path(__file__).resolve().parents[1]
def main():
    a='Assembly-CSharp';key='system.billing.ui.Receipt::GetItemType(UnityEngine.Purchasing.ProductType)->System.String'
    original={m['stable_id']:m for m in read(ROOT/'build/round3/Assembly-CSharp-original-methods.json')['selected_methods']}
    before={m['stable_id']:m for m in read(ROOT/'build/round3/Assembly-CSharp-rebuilt-methods.json')['selected_methods']}
    mapping_baseline=read(ROOT/'analysis/variants/round3-baseline-Assembly-CSharp/source-map.json')
    logical='src/Assembly-CSharp/system/billing/ui/Receipt.cs'
    source=Path(mapping_baseline[logical]);text=source.read_bytes().decode('utf-8-sig')
    old='return (type != ProductType.Subscription) ? "inapp" : "subs";'
    new='return (type == ProductType.Subscription) ? "subs" : "inapp";'
    if text.count(old)!=1:raise ValueError('Expected baseline Receipt expression')
    folder=ROOT/'analysis/variants/round3-diagnostic-a';folder.mkdir(parents=True,exist_ok=True)
    candidate=folder/'Receipt.cs';candidate.write_bytes(text.replace(old,new).encode('utf-8'))
    selected=dict(mapping_baseline);selected[logical]=str(candidate)
    mapping=folder/'source-map.json';mapping.write_text(json.dumps(selected))
    result=subprocess.run([sys.executable,str(ROOT/'tools/build_csharp.py'),'--label','round3-diagnostic-a','--source-map',str(mapping)],capture_output=True,cwd=ROOT)
    (folder/'build.log').write_bytes(result.stdout+result.stderr)
    if result.returncode:raise ValueError('Isolated Receipt compile failed')
    build=ROOT/'build/Assembly-CSharp-round3-diagnostic-a'
    built=dump(build/'Assembly-CSharp.dll',build/'methods.json')
    already={k for k,m in original.items() if 'raw_body_hex'in m and not differences(m,before.get(k))[0]}
    losses=[k for k in sorted(already) if differences(original[k],built.get(k))[0]]
    delta,percent=differences(original[key],built[key])
    report={'method':a+'::'+key,'baseline_differences':differences(original[key],before[key])[0],
        'old_scope_filter_excludes_pure_instruction_group':True,
        'old_branch_filter_original':[i['opcode'] for i in original[key]['instructions'] if i['opcode'].startswith(('brtrue','brfalse'))],
        'old_branch_filter_rebuilt':[i['opcode'] for i in before[key]['instructions'] if i['opcode'].startswith(('brtrue','brfalse'))],
        'old_Negate_already_rewrites_not_equals_to_equals':True,
        'hypothesis':{'before':old,'after':new},'candidate_body_equal':not delta,'candidate_differences':delta,
        'previous_equal_bodies_lost':losses,'baseline_equal_bodies':len(already),
        'source_sha256':sha(source),'candidate_source_sha256':sha(candidate),
        'original_instructions':original[key]['instructions'],'baseline_instructions':before[key]['instructions'],
        'candidate_instructions':built[key]['instructions'],'main_sources_modified':False,'ledger_modified':False}
    (ROOT/'reports/round3-diagnostic-A.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({'method':key,'candidate_body_equal':not delta,'losses':len(losses),'cause':'Two target-selection filters excluded the method before compilation; equality negation was already implemented.'}))
if __name__=='__main__':main()
