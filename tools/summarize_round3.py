"""Assemble round-3 measurements from verifier receipts, without editing states."""
from collections import Counter,defaultdict
import csv,json,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
BASELINE='88519635c00d72a7d1dd31f55901d73a643cf226'
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def git_csv(name):
    data=subprocess.run(['git','show',BASELINE+':config/'+name],cwd=ROOT,capture_output=True,check=True).stdout.decode('utf-8')
    return list(csv.DictReader(data.splitlines()))
def main():
    previous=git_csv('methods.csv');previous_types=git_csv('types.csv')
    now=list(csv.DictReader((ROOT/'config/methods.csv').open(encoding='utf-8',newline='')))
    groups=defaultdict(Counter);examples=defaultdict(list);assemblies={};regressions=[];held=[]
    for a in ['KairoLibrary','Assembly-CSharp']:
        receipt=read(ROOT/'build'/('verification-'+a+'.json'));folder=(ROOT/receipt['build_artifact']).parent
        original={m['stable_id']:m for m in read(folder/'original-methods.json')['selected_methods']}
        built={m['stable_id']:m for m in read(folder/'rebuilt-methods.json')['selected_methods']}
        old={r['id']:r for r in previous if r['assembly']==a};current={r['id']:r for r in now if r['assembly']==a}
        regressions.extend(k for k,r in old.items() if r['estado']=='matched' and current[k]['estado']!='matched')
        metadata=read(ROOT/'reports'/(a+'-metadata.json'))
        assemblies[a]={'total':len(current),'previous_matched':sum(r['estado']=='matched' for r in old.values()),
            'matched':receipt['counts']['matched'],'counts':receipt['counts'],
            'previous_types_matched':sum(t['assembly']==a and t['metadados']=='matched' for t in previous_types),
            'types_matched':metadata['types_equal'],'types_total':metadata['types_total'],
            'structural_metadata_equal':metadata['whole_structural_metadata_equal'],
            'type_differences':metadata['type_difference_counts_nonexclusive'],'global_metadata_differences':metadata['global_differences'],
            'type_regressions':metadata['type_state_regressions'],'build_sha256':receipt['build_sha256'],
            'missing_ids':sorted(set(original)-set(built)),'extra_ids':sorted(set(built)-set(original))}
        for identifier,result in receipt['methods'].items():
            if result['body_equal'] and result['state']!='matched':held.append(identifier)
            if not result['body_present'] or result['body_equal']:continue
            facets=set(result['differences']);dims=[]
            for name,keys in [('H',{'header'}),('L',{'locals'}),('I',{'instructions'}),('E',{'exception_regions','extra_sections'})]:
                if keys&facets:dims.append(name)
            group='+'.join(dims) or 'OTHER_RAW_BODY';groups[group][a]+=1
            sid=result['stable_id'];examples[group].append({'id':identifier,'original':original[sid],'rebuilt':built.get(sid)})
    ordered=sorted(groups,key=lambda g:(-sum(groups[g].values()),g));chosen={}
    for group in ordered[:3]:
        pool=examples[group]
        with_eh=[x for x in pool if x['original'].get('exception_regions')]
        if 'E'in group and with_eh:pool=with_eh
        chosen[group]=min(pool,key=lambda x:max(len(x['original'].get('instructions',[])),len((x['rebuilt']or{}).get('instructions',[]))))
    data={'baseline_commit':BASELINE,'assemblies':assemblies,
        'previous_matched':sum(r['estado']=='matched' for r in previous),'matched':sum(r['estado']=='matched' for r in now),
        'previous_matched_IL_bytes':sum(int(r['tamanho_il']) for r in previous if r['estado']=='matched'),
        'matched_IL_bytes':sum(int(r['tamanho_il']) for r in now if r['estado']=='matched'),
        'IL_bytes_total':sum(int(r['tamanho_il']) for r in now),
        'method_state_regressions':regressions,'held_equal_methods':held,
        'groups':[{'group':g,'counts':dict(groups[g]),'total':sum(groups[g].values())} for g in ordered],
        'top_three_examples':chosen}
    (ROOT/'reports/round3-final-summary.json').write_text(json.dumps(data,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:v for k,v in data.items() if k!='top_three_examples'}))
    for group,item in chosen.items():
        print(group,item['id'])
        for side in ['original','rebuilt']:
            m=item[side];print(side,'header',m['header'],'locals',m['locals'],'EH',m['exception_regions']);print(json.dumps(m['instructions']))
if __name__=='__main__':main()
