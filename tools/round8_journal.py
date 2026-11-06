"""Append a measured cycle result. Never changes reconstruction ledger states."""
import argparse
import json
from pathlib import Path
from compare_kairo import sha

ROOT=Path(__file__).resolve().parents[1]


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--label',required=True)
    parser.add_argument('--rule',required=True)
    parser.add_argument('--rule-steps',type=int,default=0)
    parser.add_argument('--pair',required=True)
    parser.add_argument('--hypothesis',required=True)
    parser.add_argument('--result',required=True)
    parser.add_argument('--assembly',required=True,choices=['Assembly-CSharp','KairoLibrary'])
    parser.add_argument('--before',required=True,type=Path)
    parser.add_argument('--after',required=True,type=Path)
    args=parser.parse_args()
    path=ROOT/'reports/round8-cycle-journal.json'
    journal=json.loads(path.read_text(encoding='utf-8')) if path.exists() else {'baseline_commit':'25b8108711a4a8920a2600cf9ffea5b710b3a481','cycles':[]}
    if any(c['label']==args.label for c in journal['cycles']):raise ValueError('Cycle already recorded')
    before=json.loads(args.before.read_text(encoding='utf-8'))['assemblies']
    after=json.loads(args.after.read_text(encoding='utf-8'))['assemblies']
    changes={a:{'methods_gained':after[a]['matched_methods']-before[a]['matched_methods'],
                'il_bytes_gained':after[a]['matched_original_il_bytes']-before[a]['matched_original_il_bytes']} for a in before}
    if any(v['methods_gained']<0 or v['il_bytes_gained']<0 for v in changes.values()):raise ValueError('Negative cycle gain')
    journal['cycles'].append({'cycle':len(journal['cycles'])+1,'label':args.label,'rule':args.rule,'assembly':args.assembly,'pair':args.pair,
        'hypothesis':args.hypothesis,'result':args.result,'gains':changes,'rule_accepted_steps':args.rule_steps,
        'methods_gained':sum(v['methods_gained'] for v in changes.values()),
        'il_bytes_gained':sum(v['il_bytes_gained'] for v in changes.values()),
        'before_measurement':str(args.before),'before_sha256':sha(args.before),
        'after_measurement':str(args.after),'after_sha256':sha(args.after)})
    journal['consecutive_zero_cycles']=0
    for row in reversed(journal['cycles']):
        if row['methods_gained']:break
        journal['consecutive_zero_cycles']+=1
    path.write_text(json.dumps(journal,indent=2,ensure_ascii=False)+'\n',encoding='utf-8',newline='\n')
    lines=['# Diario da rodada 8','',
        '| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |',
        '| --- | --- | --- | --- | --- | --- |']
    for row in journal['cycles']:
        values=[row['cycle'],row['assembly']+' / '+row['pair'],row['hypothesis'],row['result'],row['methods_gained'],row['il_bytes_gained']]
        lines.append('| '+' | '.join(str(v).replace('|','\\|').replace('\n',' ') for v in values)+' |')
    (ROOT/'docs/notes/round8-cycle-journal.md').write_text('\n'.join(lines)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps(journal['cycles'][-1],ensure_ascii=True))


if __name__=='__main__':main()
