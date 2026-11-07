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
from round11_controller import measured_zero_streak,stop_reason,validate_cycle,current_measurement,selection_mode,official_identity_map
from compare_kairo import differences

ROOT=Path(__file__).resolve().parents[1]


def run(arguments):
    command=[sys.executable,*map(str,arguments)]
    result=subprocess.run(command,cwd=ROOT)
    if result.returncode:raise RuntimeError('Cycle step failed: '+str(arguments))


def validate_candidate_budgets(label,probes,assembly,cfg):
    counts={}
    labels=[p['label'] for p in probes]+[label]
    for run_label in labels:
        report=json.loads((ROOT/('reports/round11-'+run_label+'-'+assembly+'-engine.json')).read_text(encoding='utf-8'))
        for method in report['methods']:
            used=method.get('incomplete_variants_compiled',0)
            if used>cfg['incomplete_candidate_variants_per_method_per_pass']:raise ValueError('Incomplete per-pass budget exceeded')
            counts[method['id']]=counts.get(method['id'],0)+used
    if any(n>cfg['incomplete_candidate_variants_per_method_per_cycle'] for n in counts.values()):raise ValueError('Incomplete per-cycle budget exceeded')


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--number',type=int,required=True)
    p.add_argument('--phase',choices=['C','D'],required=True)
    p.add_argument('--front',choices=['C1','C2','C3','C4','C5','C6'])
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
    journal_path=ROOT/'reports/round11-cycle-journal.json'
    journal=json.loads(journal_path.read_text(encoding='utf-8')) if journal_path.exists() else {'cycles':[],'consecutive_zero_cycles':0}
    autonomous=[r for r in journal['cycles'] if r.get('phase')=='D']
    cfg=json.loads((ROOT/'config/round11-protocol.json').read_text(encoding='utf-8'))
    if journal.get('consecutive_zero_cycles',0)!=measured_zero_streak(journal):raise ValueError('Stale D-only empty-queue zero counter')
    if stop_reason(cfg,journal):raise ValueError('Measured user stop condition prevents new reconstruction work')
    if args.phase!='D' and autonomous:raise ValueError('ABC cannot bypass already-started D round')
    if args.phase=='D' and args.target_substring:raise ValueError('D main must cover all pending bodies')
    queue_empty=not any(r['status']=='pending' for r in cfg['prepared_queue'])
    if args.phase=='D' and any(cfg.get('required_parts',{}).get(part)!='complete' for part in 'ABC'):
        raise ValueError('Parts A, B and C must finish before autonomous work')
    if args.phase=='D' and ((queue_empty and journal['consecutive_zero_cycles']>=cfg['consecutive_zero_stop']) or len(autonomous)>=cfg['maximum_cycles']):raise ValueError('A user stop condition prevents autonomous cycle')
    if args.number!=len(journal['cycles'])+1:raise ValueError('Cycle number is not next')
    identities=official_identity_map(json.loads((ROOT/('build/verification-'+args.assembly+'.json')).read_text(encoding='utf-8-sig')))
    primary_ids=[mid for mid in identities if args.probe_substring[0] in mid]
    if len(primary_ids)!=1:raise ValueError('Primary target is absent or ambiguous in official receipt')
    primary=primary_ids[0]
    request={'number':args.number,'phase':args.phase,'front':args.front,'assembly':args.assembly,
             'primary_target':primary,'target_substring':args.target_substring,'queue_ids':args.queue_id}
    measured=current_measurement(cfg,journal) if args.phase=='D' else None
    label=validate_cycle(cfg,journal,request,measured)
    mode=selection_mode(cfg,request,measured) if args.phase=='D' else 'C_front'
    baseline=Path('build/round11/cycles')/label/'baseline'
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
        run(['tools/round11_freeze.py','--destination',baseline])
    run(['tools/round11_measure.py','--snapshot',baseline,'--label',label+'-before'])
    original=json.loads((ROOT/baseline/args.assembly/'original-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']
    primary_ids=[m['stable_id'] for m in original if args.probe_substring[0] in m['stable_id']]
    if len(primary_ids)!=1:raise ValueError('Primary target is ambiguous')
    primary=primary_ids[0]
    validate_cycle(cfg,journal,request,measured)
    if args.phase=='D' and len(autonomous)>=2 and primary==autonomous[-1].get('primary_target')==autonomous[-2].get('primary_target'):
        raise ValueError('Same primary target cannot head three consecutive autonomous cycles')
    common=['--assembly',args.assembly,'--baseline-dir',baseline,'--cycle-rule',args.rule,
        '--cache-symbol-mapping','--aligned-search-progress','--fast-screen',
        '--limit-iterations','24','--max-candidates','64','--max-model-candidates','192']
    common+=['--max-incomplete-candidates',str(json.loads((ROOT/'config/round11-protocol.json').read_text(encoding='utf-8'))['incomplete_candidate_variants_per_method_per_pass'])]
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
        run(['tools/round11_engine.py','--run-label',probe_label,'--method-ids',ids,*common])
        document=json.loads((ROOT/('reports/round11-'+probe_label+'-'+args.assembly+'-engine.json')).read_text(encoding='utf-8'))
        method=next((m for m in document['methods'] if m['id']==selected[0]),None)
        probes.append({'label':probe_label,'method_id':selected[0],
            'body_equal':bool(method and method['body_equal']),
            'stop_reason':method['stop_reason'] if method else 'already_equal_at_baseline',
            'accepted_steps':len(method['steps']) if method else 0})
        if probes[-1]['body_equal']:break
    (ROOT/('reports/round11-'+label+'-probe-summary.json')).write_text(json.dumps({'probes':probes,
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
    run(['tools/round11_engine.py','--run-label',label,*common,*full_args])
    validate_candidate_budgets(label,probes,args.assembly,cfg)
    run(['tools/round11_adopt.py','--run-label',label,'--baseline-dir',baseline,'--assemblies',args.assembly])
    cleanup=Path('reports/round11-'+label+'-whitespace.json')
    run(['tools/round11_clean_added_whitespace.py','--report',cleanup])
    cleaned=json.loads((ROOT/cleanup).read_text(encoding='utf-8'))
    if cleaned['blank_lines'] or cleaned.get('nonblank_lines_edited',0):
        run(['tools/round11_adopt.py','--run-label',label,'--baseline-dir',baseline,'--assemblies',args.assembly,'--refresh-official-proof'])
    after_snapshot=baseline.parent/'after'
    run(['tools/round11_freeze.py','--destination',after_snapshot])
    run(['tools/round11_measure.py','--snapshot',after_snapshot,'--label',label+'-after'])
    engine=json.loads((ROOT/('reports/round11-'+label+'-'+args.assembly+'-engine.json')).read_text(encoding='utf-8'))
    new_steps=sum(s['rule'].startswith(args.rule) for m in engine['methods'] for s in m['steps'])
    new_trials=sum(t['rule'].startswith(args.rule) for m in engine['methods'] for t in m['trials'])
    result=('Novos corpos aprovados' if engine['body_gains'] else
            'Avanco parcial; nenhum novo matched' if new_steps else
            'Nenhum candidato compilado avancou' if new_trials else
            'INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento')
    result+='; '+str(new_trials)+' tentativas da regra, '+str(new_steps)+' avancos'
    for item in cfg['prepared_queue']:
        if item['id'] in args.queue_id:
            original_index={m['stable_id']:m for m in original}
            rebuilt_index={m['stable_id']:m for m in json.loads((ROOT/baseline/args.assembly/'rebuilt-methods.json').read_text(encoding='utf-8-sig'))['selected_methods']}
            audited={m['id'] for m in engine['methods']}
            equal=[mid for mid in item['method_ids'] if mid in original_index and not differences(original_index[mid],rebuilt_index.get(mid))[0]]
            missing=set(item['method_ids'])-audited-set(equal)
            if missing:raise ValueError('Initial queue targets not audited: '+repr(sorted(missing)))
            item.update(status='tested',report='reports/round11-'+label+'-'+args.assembly+'-engine.json',
                        tested_ids=sorted(set(item['method_ids'])&audited),baseline_equal_ids=equal)
    if args.phase=='C':
        front=cfg['c_fronts'][args.front]
        front['attempts'].append({'label':label,'engine':'reports/round11-'+label+'-'+args.assembly+'-engine.json',
                                  'adoption':'reports/round11-'+label+'-adoption.json','result':result,'assembly':args.assembly,
                                  'evaluation_status':'INFRA_NOT_EVALUATED_NO_PDB_MAPPING' if engine['methods'] and all(m.get('stop_reason')=='source_mapping_UNKNOWN' for m in engine['methods']) else 'evaluated'})
        front['status']='verified' if set(front['required_assemblies'])<={a['assembly'] for a in front['attempts'] if not a.get('evaluation_status','').startswith('INFRA_')} else 'partial'
        if all(f['status']=='verified' for f in cfg['c_fronts'].values()):cfg['required_parts']['C']='complete'
    cfg.update(phase='autonomous_D' if args.phase=='D' else 'part_C',autonomous_started=args.phase=='D')
    (ROOT/'config/round11-protocol.json').write_text(json.dumps(cfg,indent=2)+'\n',encoding='utf-8',newline='\n')
    run(['tools/round11_raw_gate.py','--snapshot',after_snapshot,'--measurement','reports/round11-'+label+'-after-measurement.json','--label',label])
    run(['tools/round11_journal.py','--label',label,'--rule',args.rule,'--rule-steps',new_steps,'--assembly',args.assembly,
        '--phase',args.phase,'--front',args.front or '', '--selection-mode',mode,'--primary-target',primary,'--queue-empty-before',str(queue_empty).lower(),
        '--pair',args.pair,'--hypothesis',args.hypothesis,'--result',result,
        '--before','reports/round11-'+label+'-before-measurement.json',
        '--after','reports/round11-'+label+'-after-measurement.json'])
    run(['tools/round11_hypotheses.py'])
    commit=['tools/round11_commit.py','--run-label',label,'--message','match: round 11 '+label+' '+args.rule]
    if args.push:commit+=['--push']
    run(commit)
    print(json.dumps({'cycle':label,'completed':True,'result':result}),flush=True)


if __name__=='__main__':main()
