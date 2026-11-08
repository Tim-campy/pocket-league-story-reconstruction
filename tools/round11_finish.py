"""Finish reporting only after an explicit user-defined autonomous stop.

Runs no reconstruction transformations. Source, IL and metadata proof remain
with the ordinary verifier; final models/extractor measurements are read-only.
"""
import argparse
import hashlib
from concurrent.futures import ThreadPoolExecutor
import json
from pathlib import Path
import subprocess
import sys

ROOT=Path(__file__).resolve().parents[1]


def read(path):return json.loads((ROOT/path).read_text(encoding='utf-8-sig'))

def selected_smcs_validator(cfg):
    validators={'round7_slot_model':'tools/round7_slot_validate.py',
                'round11_slot_model':'tools/round11_slot_validate.py'}
    module=cfg['models']['Assembly-CSharp']['module']
    if module not in validators:raise ValueError('No final validator for production source model '+module)
    return validators[module]
def run(arguments,label,accepted=(0,)):
    folder=ROOT/'build/round11/final-report';folder.mkdir(parents=True,exist_ok=True)
    result=subprocess.run([sys.executable,'-X','utf8',*arguments],cwd=ROOT,capture_output=True)
    (folder/(label+'.log')).write_bytes(result.stdout+result.stderr)
    if result.returncode not in accepted:
        raise RuntimeError(label+' failed: '+(result.stdout+result.stderr).decode('utf-8',errors='replace')[-5000:])
    return result


def validate_final_control(path,corpus,expected_matched):
    document=read(path)
    if document.get('valid_frozen_input_measurement') is not True or document.get('inputs_changed_during_run'):
        raise ValueError('Final control extraction changed inputs or is invalid')
    if document.get('own_IL_or_MDB_used_to_infer_AST_control') is not False:
        raise ValueError('Final AST control independence is not proved')
    if (ROOT/document['snapshot']).resolve()!=(ROOT/corpus).resolve():
        raise ValueError('Final control extraction describes another snapshot')
    checks={str(Path(corpus)/'methods.csv'):document['ledger_sha256'],
            document['method_irs']:document['method_irs_sha256'],
            str(Path(document['method_irs']).parent/'source-manifest.json'):document['source_manifest_sha256'],
            **document['extractor_inputs']}
    for name,digest in checks.items():
        if hashlib.sha256((ROOT/name).read_bytes()).hexdigest()!=digest:
            raise ValueError('Stale final control evidence: '+name)
    if document['matched']['methods']!=expected_matched:
        raise ValueError('Final control and model matched denominators disagree')
    return document


def main():
    p=argparse.ArgumentParser(description=__doc__)
    p.add_argument('--recommendations',required=True,type=Path)
    p.add_argument('--push',action='store_true')
    p.add_argument('--user-requested-report',action='store_true',
                   help='Record the current explicit user request for the final report; run no new reconstruction cycle')
    args=p.parse_args()
    cfg=read('config/round11-protocol.json');journal=read('reports/round11-cycle-journal.json')
    if cfg.get('report_schema_reviewed') is not True:raise ValueError('Review Round10 ABC report adapter before finalization')
    autonomous=[r for r in journal['cycles'] if r.get('phase')=='D']
    pending=[q['id'] for q in cfg['prepared_queue'] if q['status']=='pending']
    if args.user_requested_report:
        cfg['requested_stop']={'source':'user','request':'me envie o relatorio final',
            'operation':'final_report_requested',
            'after_autonomous_cycle':len(autonomous),
            'checkpoint_commit':subprocess.check_output(['git','rev-parse','HEAD'],cwd=ROOT).decode().strip()}
        reason='Solicitação explícita do usuário: me envie o relatorio final; parada após o ciclo D06'
    elif len(autonomous)>=cfg['maximum_cycles']:
        reason=f"Limite de {cfg['maximum_cycles']} ciclos autônomos concluídos"
    elif not pending and journal['consecutive_zero_cycles']>=cfg['consecutive_zero_stop']:
        reason=f"{journal['consecutive_zero_cycles']} ciclos D consecutivos sem novo matched, com a fila preparada vazia"
    else:
        raise ValueError('No measured normal stop condition; continue the authorized round')
    recommendations=json.loads(args.recommendations.read_text(encoding='utf-8-sig'))
    if not isinstance(recommendations,list) or not 2<=len(recommendations)<=3:
        raise ValueError('Provide two or three evidence-based recommendations')
    run(['tools/verify','--all'],'full-verification',accepted=(0,1))
    corpus='build/round11/final-corpus'
    run(['tools/round11_freeze.py','--destination',corpus],'freeze-final')
    jobs=[([selected_smcs_validator(cfg),'--baseline',corpus+'/Assembly-CSharp',
            '--output','reports/round11-final-smcs-model-validation.json'],'smcs-final'),
          (['tools/round11_roslyn_validate.py','--baseline',corpus+'/KairoLibrary',
            '--output','reports/round11-final-roslyn-model-validation.json'],'roslyn-final'),
          (['tools/round11_control_validate.py','--snapshot',corpus,'--label','final','--stage','4'],'control-final')]
    with ThreadPoolExecutor(max_workers=3) as pool:
        futures=[pool.submit(run,arguments,label) for arguments,label in jobs]
        for future in futures:future.result()
    smcs_validation=read('reports/round11-final-smcs-model-validation.json')
    validate_final_control('reports/round11-control-validation-final.json',corpus,smcs_validation['matched_bodies'])
    cfg['final_model_measurements']={
        'Assembly-CSharp':{'validation':'reports/round11-final-smcs-model-validation.json','corpus_baseline':corpus+'/Assembly-CSharp'},
        'KairoLibrary':{'validation':'reports/round11-final-roslyn-model-validation.json','corpus_baseline':corpus+'/KairoLibrary'}}
    cfg['control_extractor_validation']='reports/round11-control-validation-final.json'
    cfg.update(phase='stopped',autonomous_started=True)
    (ROOT/'config/round11-protocol.json').write_text(json.dumps(cfg,indent=2)+'\n',encoding='utf-8',newline='\n')
    stop={'operation':'measured-round11-stop','new_reconstruction_work_stopped':True,
        'reason':reason,'cycles_completed':len(journal['cycles']),
        'autonomous_cycles_completed':len(autonomous),
        'consecutive_zero_D_cycles':journal['consecutive_zero_cycles'],
        'prepared_pending_at_stop':pending,
        'blockers':['Nenhuma instalação ou arquivo solicitado ao usuário. A rodada foi encerrada pelo pedido de relatório final, antes dos limites normais; há quatro propostas privadas não compiladas no jogo e um defeito conhecido do modelo Roslyn em um método pendente.'],
        'recommendations':recommendations,
        'preparation_notes':cfg.get('preparation_notes',[])}
    (ROOT/'reports/round11-stop.json').write_text(json.dumps(stop,indent=2)+'\n',encoding='utf-8',newline='\n')
    command=['tools/round11_commit.py','--run-label','final-report','--message','docs: round 11 measured results and stop','--final-report']
    if args.push:command+=['--push']
    result=run(command,'final-commit')
    print(result.stdout.decode('utf-8',errors='replace'))


if __name__=='__main__':main()
