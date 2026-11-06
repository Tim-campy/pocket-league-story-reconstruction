"""Run one authorized, bounded cycle; original artifacts and ledger are read-only here.

Only the official verifier called by adoption may promote states. A successful
cycle commits the full verified evidence, including a zero-gain cycle.
"""
import argparse
import json
from pathlib import Path
import subprocess
import sys
from compare_kairo import sha
from apply_il_rules import receipt_is_fresh

ROOT=Path(__file__).resolve().parents[1]


def run(arguments):
    command=[sys.executable,*map(str,arguments)]
    result=subprocess.run(command,cwd=ROOT)
    if result.returncode:raise RuntimeError('Cycle step failed: '+str(arguments))


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--number',type=int,required=True)
    p.add_argument('--phase',choices=list('ABCD'),required=True)
    p.add_argument('--queue-id',action='append',default=[])
    p.add_argument('--target-substring',action='append',default=[])
    p.add_argument('--isolate-family',action='store_true')
    p.add_argument('--single-step',action='store_true')
    p.add_argument('--assembly',choices=['KairoLibrary','Assembly-CSharp'],required=True)
    p.add_argument('--rule',required=True)
    p.add_argument('--pair',required=True)
    p.add_argument('--hypothesis',required=True)
    p.add_argument('--probe-substring',required=True,action='append')
    p.add_argument('--push',action='store_true')
    p.add_argument('--external-proposals',type=Path)
    p.add_argument('--use-prepared-baseline',action='store_true',help='Resume preparation only with unchanged HEAD, ledgers, sources and fresh receipts')
    args=p.parse_args()
    journal_path=ROOT/'reports/round9-cycle-journal.json'
    journal=json.loads(journal_path.read_text(encoding='utf-8')) if journal_path.exists() else {'cycles':[],'consecutive_zero_cycles':0}
    autonomous=[r for r in journal['cycles'] if r.get('phase')=='D']
    cfg=json.loads((ROOT/'config/round9-protocol.json').read_text(encoding='utf-8'))
    queue_empty=not any(r['status']=='pending' for r in cfg['prepared_queue'])
    if args.phase=='D' and any(cfg.get('required_parts',{}).get(part)!='complete' for part in 'ABC'):
        raise ValueError('Parts A, B and C must finish before autonomous work')
    if args.phase=='D' and ((queue_empty and journal['consecutive_zero_cycles']>=5) or len(autonomous)>=40):raise ValueError('A user stop condition prevents autonomous cycle')
    if args.number!=len(journal['cycles'])+1:raise ValueError('Cycle number is not next')
    if args.phase=='D' and len(autonomous)>=4 and args.assembly!='KairoLibrary' and not any(r['assembly']=='KairoLibrary' for r in autonomous[-4:]):
        raise ValueError('The fifth cycle in a rolling block must examine KairoLibrary')
    label='cycle%02d'%(len(autonomous)+1) if args.phase=='D' else args.phase.lower()+'%02d'%args.number
    baseline=Path('build/round9/cycles')/label/'baseline'
    if args.use_prepared_baseline:
        snapshot=json.loads((ROOT/baseline/'snapshot.json').read_text(encoding='utf-8'))
        head=subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT).decode().strip()
        if snapshot['commit']!=head or snapshot['ledger_sha256']!=sha(ROOT/'config/methods.csv'):
            raise ValueError('Prepared baseline no longer describes HEAD/ledger')
        for name in ('methods.csv','types.csv','toolchain.json'):
            if sha(ROOT/baseline/name)!=sha(ROOT/'config'/name):
                raise ValueError('Prepared configuration changed: '+name)
        for assembly in ('KairoLibrary','Assembly-CSharp'):
            receipt=json.loads((ROOT/('build/verification-'+assembly+'.json')).read_text(encoding='utf-8-sig'))
            if not receipt_is_fresh(ROOT,receipt) or any(receipt.get(k) for k in ('regressions','body_regressions','type_regressions')):
                raise ValueError('Prepared receipt is stale/regressive: '+assembly)
            manifest=json.loads((ROOT/baseline/assembly/'build-manifest.json').read_text(encoding='utf-8-sig'))
            for source in manifest['sources']:
                if sha(ROOT/baseline/assembly/source['path'])!=sha(ROOT/source['path']):
                    raise ValueError('Prepared source changed: '+source['path'])
        print(json.dumps({'prepared_baseline_reused':str(baseline),'immutable_snapshot_preserved':True}),flush=True)
    else:
        run(['tools/round9_freeze.py','--destination',baseline])
    run(['tools/round9_measure.py','--snapshot',baseline,'--label',label+'-before'])
    original=json.loads((ROOT/baseline/args.assembly/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']
    primary_ids=[m['stable_id'] for m in original if args.probe_substring[0] in m['stable_id']]
    if len(primary_ids)!=1:raise ValueError('Primary target is ambiguous')
    primary=primary_ids[0]
    if args.phase=='D' and len(autonomous)>=2 and primary==autonomous[-1].get('primary_target')==autonomous[-2].get('primary_target'):
        raise ValueError('Same primary target cannot head three consecutive autonomous cycles')
    common=['--assembly',args.assembly,'--baseline-dir',baseline,'--cycle-rule',args.rule,
        '--cache-symbol-mapping','--aligned-search-progress','--fast-screen',
        '--limit-iterations','24','--max-candidates','64','--max-model-candidates','192']
    common+=['--max-incomplete-candidates',str(json.loads((ROOT/'config/round9-protocol.json').read_text())['incomplete_candidate_variants_per_method_per_pass'])]
    if args.external_proposals:common+=['--external-proposals',str(args.external_proposals)]
    if args.isolate_family:common+=['--isolate-family']
    if args.single_step:common[common.index('--limit-iterations')+1]='1'
    probes=[]
    probed_ids=set()
    for number,needle in enumerate(args.probe_substring,1):
        selected=[m['stable_id'] for m in original if needle in m['stable_id']]
        if len(selected)!=1:raise ValueError('Isolated probe must select one original method: '+str(selected))
        if selected[0] in probed_ids:raise ValueError('Only one isolated probe per method per cycle')
        probed_ids.add(selected[0])
        ids=ROOT/baseline.parent/('probe%d-ids.json'%number);ids.write_text(json.dumps(selected),encoding='utf-8')
        probe_label=label+('-probe' if number==1 else '-probe%d'%number)
        run(['tools/round9_engine.py','--run-label',probe_label,'--method-ids',ids,*common])
        document=json.loads((ROOT/('reports/round9-'+probe_label+'-'+args.assembly+'-engine.json')).read_text(encoding='utf-8'))
        method=next((m for m in document['methods'] if m['id']==selected[0]),None)
        probes.append({'label':probe_label,'method_id':selected[0],
            'body_equal':bool(method and method['body_equal']),
            'stop_reason':method['stop_reason'] if method else 'already_equal_at_baseline',
            'accepted_steps':len(method['steps']) if method else 0})
        if probes[-1]['body_equal']:break
    (ROOT/('reports/round9-'+label+'-probe-summary.json')).write_text(json.dumps({'probes':probes,
        'isolated_complete_match':any(p['body_equal'] for p in probes)},indent=2)+'\n',encoding='utf-8',newline='\n')
    full_args=[]
    if args.target_substring:
        chosen=[]
        for needle in args.target_substring:
            ids=[m['stable_id'] for m in original if needle in m['stable_id']]
            if len(ids)!=1:raise ValueError('Target must select exactly one method: '+str(ids))
            chosen+=ids
        scope=ROOT/baseline.parent/'main-ids.json';scope.write_text(json.dumps(sorted(set(chosen))),encoding='utf-8')
        full_args=['--method-ids',scope]
    run(['tools/round9_engine.py','--run-label',label,*common,*full_args])
    run(['tools/round9_adopt.py','--run-label',label,'--baseline-dir',baseline,'--assemblies',args.assembly])
    cleanup=Path('reports/round9-'+label+'-whitespace.json')
    run(['tools/round9_clean_added_whitespace.py','--report',cleanup])
    if json.loads((ROOT/cleanup).read_text(encoding='utf-8'))['blank_lines']:
        run(['tools/round9_adopt.py','--run-label',label,'--baseline-dir',baseline,'--assemblies',args.assembly,'--refresh-official-proof'])
    after_snapshot=baseline.parent/'after'
    run(['tools/round9_freeze.py','--destination',after_snapshot])
    run(['tools/round9_measure.py','--snapshot',after_snapshot,'--label',label+'-after'])
    engine=json.loads((ROOT/('reports/round9-'+label+'-'+args.assembly+'-engine.json')).read_text(encoding='utf-8'))
    new_steps=sum(s['rule'].startswith(args.rule) for m in engine['methods'] for s in m['steps'])
    new_trials=sum(t['rule'].startswith(args.rule) for m in engine['methods'] for t in m['trials'])
    result=('Novos corpos aprovados' if engine['body_gains'] else
            'Avanco parcial; nenhum novo matched' if new_steps else
            'Nenhum candidato compilado avancou' if new_trials else
            'INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento')
    result+='; '+str(new_trials)+' tentativas da regra, '+str(new_steps)+' avancos'
    for item in cfg['prepared_queue']:
        if item['id'] in args.queue_id:item.update(status='tested',report='reports/round9-'+label+'-'+args.assembly+'-engine.json')
    (ROOT/'config/round9-protocol.json').write_text(json.dumps(cfg,indent=2)+'\n',encoding='utf-8',newline='\n')
    run(['tools/round9_journal.py','--label',label,'--rule',args.rule,'--rule-steps',new_steps,'--assembly',args.assembly,
        '--phase',args.phase,'--primary-target',primary,'--queue-empty-before',str(queue_empty).lower(),
        '--pair',args.pair,'--hypothesis',args.hypothesis,'--result',result,
        '--before','reports/round9-'+label+'-before-measurement.json',
        '--after','reports/round9-'+label+'-after-measurement.json'])
    run(['tools/round9_hypotheses.py'])
    commit=['tools/round9_commit.py','--run-label',label,'--message','match: round 9 '+label+' '+args.rule]
    if args.push:commit+=['--push']
    run(commit)
    print(json.dumps({'cycle':label,'completed':True,'result':result}),flush=True)


if __name__=='__main__':main()
