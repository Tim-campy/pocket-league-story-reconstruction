"""Read measured receipts and produce the round report data; never edit states."""
from collections import Counter,defaultdict
import csv,json,subprocess
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def main():
    previous=subprocess.run(['git','show','2ab24ee6f9b8d8f204c587b878d38ed120151467:config/methods.csv'],cwd=ROOT,capture_output=True,check=True).stdout.decode('utf-8')
    prior=list(csv.DictReader(previous.splitlines()))
    current=list(csv.DictReader((ROOT/'config/methods.csv').open(encoding='utf-8',newline='')))
    groups=defaultdict(Counter);examples=defaultdict(list);assemblies={};regressions=[];metadata={}
    for a in ['KairoLibrary','Assembly-CSharp']:
        receipt=read(ROOT/'build'/('verification-'+a+'.json'));folder=(ROOT/receipt['build_artifact']).parent
        original={m['stable_id']:m for m in read(folder/'original-methods.json')['selected_methods']}
        built={m['stable_id']:m for m in read(folder/'rebuilt-methods.json')['selected_methods']}
        before={r['id']:r for r in prior if r['assembly']==a};now={r['id']:r for r in current if r['assembly']==a}
        regressions.extend(k for k,r in before.items() if r['estado']=='matched' and now[k]['estado']!='matched')
        assemblies[a]={'total':len(now),'previous_matched':sum(r['estado']=='matched' for r in before.values()),'matched':receipt['counts']['matched'],'counts':receipt['counts'],'missing_ids':sorted(set(original)-set(built)),'extra_ids':sorted(set(built)-set(original)),'build_sha256':receipt['build_sha256']}
        metadata[a]=read(ROOT/'reports'/(a+'-metadata.json'))
        for key,r in receipt['methods'].items():
            if not r['body_present'] or r['body_equal']:continue
            facets=set(r['differences']);dims=[]
            for label,keys in [('H',{'header'}),('L',{'locals'}),('I',{'instructions'}),('E',{'exception_regions','extra_sections'})]:
                if facets&keys:dims.append(label)
            group='+'.join(dims) if dims else '+'.join(sorted(facets-{'resolved_body_bytes'}))
            groups[group][a]+=1
            sid=r['stable_id'];o=original[sid];b=built.get(sid)
            examples[group].append({'id':key,'stable_id':sid,'original':o,'rebuilt':b})
    ordered=sorted(groups,key=lambda k:-sum(groups[k].values()))
    chosen={}
    for group in ordered[:3]:
        chosen[group]=min(examples[group],key=lambda x:max(len(x['original'].get('instructions',[])),len((x['rebuilt']or{}).get('instructions',[]))))
    result={'baseline_commit':'2ab24ee6f9b8d8f204c587b878d38ed120151467','assemblies':assemblies,'matched':sum(a['matched'] for a in assemblies.values()),'previous_matched':sum(a['previous_matched'] for a in assemblies.values()),'method_state_regressions':regressions,
        'groups':[{'group':g,'counts':dict(groups[g]),'total':sum(groups[g].values())} for g in ordered],
        'top_three_examples':chosen,'metadata':{a:{k:m[k] for k in ['types_total','types_equal','type_difference_counts_nonexclusive','global_differences','declarations_proved','resources_equal','original_resource_count','rebuilt_resource_count','type_state_regressions']} for a,m in metadata.items()}}
    out=ROOT/'reports/round2-final-summary.json';out.write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:v for k,v in result.items() if k!='top_three_examples'}))
    for group,x in chosen.items():
        print(group,x['id'])
        for side in ['original','rebuilt']:
            m=x[side];print(side,'HEADER',m['header'],'LOCALS',m['locals']);print(json.dumps(m['instructions']))
if __name__=='__main__':main()
