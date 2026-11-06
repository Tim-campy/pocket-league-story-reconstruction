"""Run one authorized, bounded cycle; original artifacts and ledger are read-only here.

Only the official verifier called by adoption may promote states. A successful
cycle commits the full verified evidence, including a zero-gain cycle.
"""
import argparse
import json
from pathlib import Path
import subprocess
import sys

ROOT=Path(__file__).resolve().parents[1]


def run(arguments):
    command=[sys.executable,*map(str,arguments)]
    result=subprocess.run(command,cwd=ROOT)
    if result.returncode:raise RuntimeError('Cycle step failed: '+str(arguments))


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--number',type=int,required=True)
    p.add_argument('--assembly',choices=['KairoLibrary','Assembly-CSharp'],required=True)
    p.add_argument('--rule',required=True)
    p.add_argument('--pair',required=True)
    p.add_argument('--hypothesis',required=True)
    p.add_argument('--probe-substring',required=True,action='append')
    p.add_argument('--push',action='store_true')
    args=p.parse_args()
    journal_path=ROOT/'reports/round7-cycle-journal.json'
    journal=json.loads(journal_path.read_text(encoding='utf-8')) if journal_path.exists() else {'cycles':[],'consecutive_zero_cycles':0}
    if journal['consecutive_zero_cycles']>=3 or len(journal['cycles'])>=20:raise ValueError('A user stop condition already holds')
    if args.number!=len(journal['cycles'])+1:raise ValueError('Cycle number is not next')
    label='cycle%02d'%args.number
    baseline=Path('build/round7/cycles')/label/'baseline'
    run(['tools/round7_freeze.py','--destination',baseline])
    run(['tools/round7_measure.py','--snapshot',baseline,'--label',label+'-before'])
    original=json.loads((ROOT/baseline/args.assembly/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']
    common=['--assembly',args.assembly,'--baseline-dir',baseline,'--cycle-rule',args.rule,
        '--cache-symbol-mapping','--aligned-search-progress','--fast-screen',
        '--limit-iterations','24','--max-candidates','64','--max-model-candidates','192']
    probes=[]
    for number,needle in enumerate(args.probe_substring,1):
        selected=[m['stable_id'] for m in original if needle in m['stable_id']]
        if len(selected)!=1:raise ValueError('Isolated probe must select one original method: '+str(selected))
        ids=ROOT/baseline.parent/('probe%d-ids.json'%number);ids.write_text(json.dumps(selected),encoding='utf-8')
        probe_label=label+('-probe' if number==1 else '-probe%d'%number)
        run(['tools/round7_engine.py','--run-label',probe_label,'--method-ids',ids,*common])
        document=json.loads((ROOT/('reports/round7-'+probe_label+'-'+args.assembly+'-engine.json')).read_text(encoding='utf-8'))
        method=next((m for m in document['methods'] if m['id']==selected[0]),None)
        probes.append({'label':probe_label,'method_id':selected[0],
            'body_equal':bool(method and method['body_equal']),
            'stop_reason':method['stop_reason'] if method else 'already_equal_at_baseline',
            'accepted_steps':len(method['steps']) if method else 0})
        if probes[-1]['body_equal']:break
    (ROOT/('reports/round7-'+label+'-probe-summary.json')).write_text(json.dumps({'probes':probes,
        'isolated_complete_match':any(p['body_equal'] for p in probes)},indent=2)+'\n',encoding='utf-8',newline='\n')
    run(['tools/round7_engine.py','--run-label',label,*common])
    run(['tools/round7_adopt.py','--run-label',label,'--baseline-dir',baseline,'--assemblies',args.assembly])
    cleanup=Path('reports/round7-'+label+'-whitespace.json')
    run(['tools/round7_clean_added_whitespace.py','--report',cleanup])
    if json.loads((ROOT/cleanup).read_text(encoding='utf-8'))['blank_lines']:
        run(['tools/round7_adopt.py','--run-label',label,'--baseline-dir',baseline,'--assemblies',args.assembly,'--refresh-official-proof'])
    after_snapshot=baseline.parent/'after'
    run(['tools/round7_freeze.py','--destination',after_snapshot])
    run(['tools/round7_measure.py','--snapshot',after_snapshot,'--label',label+'-after'])
    engine=json.loads((ROOT/('reports/round7-'+label+'-'+args.assembly+'-engine.json')).read_text(encoding='utf-8'))
    new_steps=sum(s['rule'].startswith(args.rule) for m in engine['methods'] for s in m['steps'])
    new_trials=sum(t['rule'].startswith(args.rule) for m in engine['methods'] for t in m['trials'])
    result=('Novos corpos aprovados' if engine['body_gains'] else
            'Avanco parcial; nenhum novo matched' if new_steps else
            'Nenhum candidato compilado avancou' if new_trials else
            'INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento')
    result+='; '+str(new_trials)+' tentativas da regra, '+str(new_steps)+' avancos'
    run(['tools/round7_journal.py','--label',label,'--rule',args.rule,'--rule-steps',new_steps,'--assembly',args.assembly,
        '--pair',args.pair,'--hypothesis',args.hypothesis,'--result',result,
        '--before','reports/round7-'+label+'-before-measurement.json',
        '--after','reports/round7-'+label+'-after-measurement.json'])
    run(['tools/round7_hypotheses.py'])
    commit=['tools/round7_commit.py','--run-label',label,'--message','match: round 7 '+label+' '+args.rule]
    if args.push:commit+=['--push']
    run(commit)
    print(json.dumps({'cycle':label,'completed':True,'result':result}),flush=True)


if __name__=='__main__':main()
