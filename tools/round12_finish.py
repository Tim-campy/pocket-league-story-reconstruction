"""Final evidence and commit after a measured Round12 stop, never a new cycle.

Run only after the production operator hands over exclusive target ownership.
"""
import argparse
from concurrent.futures import ThreadPoolExecutor
import json
from pathlib import Path
import subprocess
import sys

ROOT=next(p for p in Path(__file__).resolve().parents if(p/'AGENTS.md').exists())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from apply_il_rules import receipt_is_fresh
from round12_controller import stop_reason,validate_config,current_measurement,verify_raw_gate

def read(path):return json.loads((ROOT/path).read_text(encoding='utf-8-sig'))
def write(path,value):(ROOT/path).write_text(json.dumps(value,indent=2,ensure_ascii=False)+'\n',encoding='utf-8',newline='\n')
def run(args,label,accepted=(0,)):
 folder=ROOT/'build/round12/final-report';folder.mkdir(parents=True,exist_ok=True)
 result=subprocess.run([sys.executable,'-X','utf8',*map(str,args)],cwd=ROOT,capture_output=True)
 (folder/(label+'.log')).write_bytes(result.stdout+result.stderr)
 if result.returncode not in accepted:raise RuntimeError(label+'\n'+(result.stdout+result.stderr).decode('utf-8',errors='replace')[-4500:])
 return result

def main():
 p=argparse.ArgumentParser(description=__doc__);p.add_argument('--recommendations',required=True,type=Path);p.add_argument('--push',action='store_true');args=p.parse_args()
 cfg=read('config/round12-protocol.json');validate_config(cfg);journal=read('reports/round12-cycle-journal.json');reason=stop_reason(cfg,journal)
 if not reason:raise ValueError('No authorized measured stop; continue reconstruction')
 recommendations=json.loads(args.recommendations.read_text(encoding='utf-8-sig'))
 if not isinstance(recommendations,list)or not 2<=len(recommendations)<=3:raise ValueError('Two or three measured recommendations required')
 for a in('KairoLibrary','Assembly-CSharp'):
  receipt=read('build/verification-'+a+'.json')
  if not receipt_is_fresh(ROOT,receipt)or any(receipt.get(k)for k in('regressions','body_regressions','type_regressions')):raise ValueError('Fresh zero-regression verifier proof required')
  for name,digest in receipt.get('helper_source_hashes',{}).items():
   if sha(ROOT/name)!=digest:raise ValueError('Verifier helper changed: '+name)
 # Reuse the last immutable, fully verified corpus. No reconstruction source
 # changes occur during reporting. The final commit below performs the one
 # required full verification and renews literal raw acceptance of that PE.
 measurement=current_measurement(cfg,journal);verify_raw_gate(cfg,measurement);corpus=measurement['snapshot']
 jobs=[(['tools/round12_roslyn_validate.py','--baseline',corpus+'/KairoLibrary','--output','reports/round12-final-roslyn-model-validation.json'],'roslyn-model'),(['tools/round11_slot_validate.py','--baseline',corpus+'/Assembly-CSharp','--output','reports/round12-final-smcs-model-validation.json'],'smcs-model'),(['tools/round11_control_validate.py','--snapshot',corpus,'--label','round12-final','--stage','4'],'control')]
 with ThreadPoolExecutor(max_workers=3)as pool:
  futures=[pool.submit(run,*job)for job in jobs]
  for future in futures:future.result()
 for assembly,filename in [('KairoLibrary','round12-final-roslyn-model-validation.json'),('Assembly-CSharp','round12-final-smcs-model-validation.json')]:
  model=read('reports/'+filename)
  if model['matched_complete_mismatches']:raise ValueError('Incorrect complete model prediction; investigate before reporting')
 control=read('reports/round11-control-validation-round12-final.json')
 if not control['valid_frozen_input_measurement']or control['inputs_changed_during_run']or control['own_IL_or_MDB_used_to_infer_AST_control']:raise ValueError('Control measurement is not independent and frozen')
 if control['matched']['methods']!=read('reports/round12-final-smcs-model-validation.json')['matched_bodies']:raise ValueError('Model and control corpus mismatch')
 control['legacy_measurement_file']='reports/round11-control-validation-round12-final.json';control['legacy_measurement_sha256']=sha(ROOT/control['legacy_measurement_file']);write('reports/round12-control-validation-final.json',control)
 cfg['final_model_measurements']={a:{'validation':'reports/'+filename,'corpus_baseline':corpus+'/'+a}for a,filename in [('KairoLibrary','round12-final-roslyn-model-validation.json'),('Assembly-CSharp','round12-final-smcs-model-validation.json')]}
 cfg['control_extractor_validation']='reports/round12-control-validation-final.json';cfg['phase']='stopped';cfg['required_parts']['B']='complete' if reason=='all_method_and_structural_metadata_targets_closed' else 'stopped_by_protocol';write('config/round12-protocol.json',cfg)
 reasons={'maximum_40_Part_B_cycles':'Limite de quarenta ciclos da Parte B concluído.','five_zero_Part_B_cycles_with_queue_empty_before_and_after':'Cinco ciclos consecutivos da Parte B sem novo matched, com a fila vazia antes e depois de cada um.','explicit_user_stop':'Parada solicitada explicitamente pelo usuário.'}
 reasons['all_method_and_structural_metadata_targets_closed']='Todos os métodos e metadados estruturais dos dois assemblies foram aprovados pelo verificador, com fontes e PEs atuais. O ciclo de correções de corpos foi encerrado por conclusão, antes do limite de quarenta ciclos.'
 stop={'operation':'measured-round12-stop','new_reconstruction_work_stopped':True,'reason_code':reason,'reason':reasons[reason],'cycles_A':sum(r['phase']=='A'for r in journal['cycles']),'cycles_B':sum(r['phase']=='B'for r in journal['cycles']),'consecutive_zero_B_cycles':journal['consecutive_zero_cycles'],'prepared_pending_at_stop':[q['id']for q in cfg['prepared_queue']if q['status']=='pending'],'blockers':['Nenhuma instalação, arquivo adicional ou decisão de escopo foi solicitada ao usuário nesta rodada. O motivo da parada é o critério registrado acima.'],'recommendations':recommendations,'no_new_apk':True}
 write('reports/round12-stop.json',stop)
 cmd=['tools/round12_commit.py','--run-label','final-report','--message','docs: round 12 measured final report and stop','--final-report']
 if args.push:cmd+=['--push']
 result=run(cmd,'final-commit');print(result.stdout.decode('utf-8',errors='replace'))

if __name__=='__main__':main()
