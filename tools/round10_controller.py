"""REVIEW-ONLY Round10 command planner. Never executes a tool or writes state."""
import argparse,hashlib,json,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if(p/'AGENTS.md').is_file())
class GuardError(ValueError):pass

def d_rows(journal):return[r for r in journal.get('cycles',[])if r.get('phase')=='D']
def queue_empty(cfg):return not any(q.get('status')=='pending'for q in cfg.get('prepared_queue',[]))
def measured_zero_streak(journal):
 count=0
 for row in reversed(d_rows(journal)):
  if row.get('queue_empty_before')is not True or row.get('queue_empty')is not True:break
  gains=row.get('methods_gained')
  if not isinstance(gains,int)or gains<0:raise GuardError('Invalid measured gain')
  if gains:break
  count+=1
 return count

def stop_reason(cfg,journal):
 if len(d_rows(journal))>=cfg['maximum_cycles']:return'maximum_'+str(cfg['maximum_cycles'])+'_D_cycles'
 if queue_empty(cfg)and measured_zero_streak(journal)>=5:return'five_zero_D_cycles_with_queue_empty_before_and_after'
 return None

def official_identity_map(receipt):
 result={}
 for ledger_key,proof in receipt['methods'].items():
  stable=proof.get('stable_id')
  if not stable or stable in result:raise GuardError('Missing or ambiguous official stable identity')
  result[stable]=(ledger_key,proof)
 return result

def validate_cycle(cfg,journal,request):
 cap=cfg.get('maximum_cycles')
 override=cfg.get('user_stop_limit_override',{})
 if cap not in (35,40) or (cap==35 and (override.get('previous_maximum_cycles')!=40 or override.get('instruction')!='Pare essa rodada no ciclo35' or override.get('reason')!='stop_limit_user35')) or cfg.get('consecutive_zero_stop')!=5 or cfg.get('maximum_consecutive_primary_same_method')!=2:raise GuardError('Protocol constants changed without the recorded user override')
 rows=d_rows(journal);phase=request['phase'];reason=stop_reason(cfg,journal)
 if reason:raise GuardError('Reconstruction stopped: '+reason)
 if cfg.get('phase')=='stopped':raise GuardError('Round already stopped')
 if request['number']!=len(journal.get('cycles',[]))+1:raise GuardError('Global journal number is not next')
 if phase=='D':
  if any(cfg.get('required_parts',{}).get(p)!='complete'for p in 'ABC'):raise GuardError('ABC not complete')
  if request.get('target_substring')or request.get('method_ids'):raise GuardError('D main must cover every pending body of selected assembly')
  if len(rows)>=4 and request['assembly']!='KairoLibrary'and not any(r['assembly']=='KairoLibrary'for r in rows[-4:]):raise GuardError('Fifth rolling D must be KairoLibrary')
  if len(rows)>=2 and rows[-1].get('primary_target')==rows[-2].get('primary_target')==request['primary_target']:raise GuardError('Same primary would head three D cycles')
 elif rows:raise GuardError('ABC work cannot be used to bypass an already-started D round')
 return 'cycle%02d'%(len(rows)+1)if phase=='D'else phase.lower()+'%02d'%request['number']

def file_hash(path):return hashlib.sha256(path.read_bytes()).hexdigest()
def load(path):return json.loads(path.read_text(encoding='utf-8-sig'))
def main():
 p=argparse.ArgumentParser(description=__doc__);p.add_argument('--config',type=Path,default=ROOT/'config/round10-protocol.json');p.add_argument('--journal',type=Path,default=ROOT/'reports/round10-cycle-journal.json')
 sub=p.add_subparsers(dest='mode',required=True);sub.add_parser('check');q=sub.add_parser('plan-cycle');q.add_argument('--phase',choices=list('ABCD'),required=True);q.add_argument('--assembly',choices=['KairoLibrary','Assembly-CSharp'],required=True);q.add_argument('--primary-id',required=True);q.add_argument('--probe-id',action='append',default=[]);q.add_argument('--queue-id',action='append',default=[]);q.add_argument('--rule',required=True);q.add_argument('--pair',required=True);q.add_argument('--hypothesis',required=True);q.add_argument('--external-proposals');q.add_argument('--push',action='store_true')
 q=sub.add_parser('queue-command');q.add_argument('--execute',action='store_true');q.add_argument('--push',action='store_true');q=sub.add_parser('finish-command');q.add_argument('--recommendations',required=True);q.add_argument('--push',action='store_true')
 args=p.parse_args();cfg=load(args.config);journal=load(args.journal)if args.journal.exists()else{'cycles':[],'consecutive_zero_cycles':0};inputs={str(args.config):file_hash(args.config)}
 if args.journal.exists():inputs[str(args.journal)]=file_hash(args.journal)
 result={'review_only':True,'executes_commands':False,'writes_state':False,'D_count':len(d_rows(journal)),'zero_streak':measured_zero_streak(journal),'prepared_queue_empty':queue_empty(cfg),'stop_reason':stop_reason(cfg,journal),'input_sha256':inputs}
 if args.mode=='plan-cycle':
  receipt_path=ROOT/('build/verification-'+args.assembly+'.json');index=official_identity_map(load(receipt_path));inputs[str(receipt_path)]=file_hash(receipt_path)
  probes=[args.primary_id,*args.probe_id]
  if len(set(probes))!=len(probes)or any(mid not in index for mid in probes):raise GuardError('Probe identities absent/duplicated in official receipt')
  request={'phase':args.phase,'assembly':args.assembly,'primary_target':args.primary_id,'number':len(journal['cycles'])+1};label=validate_cycle(cfg,journal,request)
  command=[sys.executable,'-X','utf8','tools/round10_cycle.py','--number',str(request['number']),'--phase',args.phase,'--assembly',args.assembly,'--rule',args.rule,'--pair',args.pair,'--hypothesis',args.hypothesis]
  for mid in probes:command+=['--probe-substring',mid]
  for item in args.queue_id:command+=['--queue-id',item]
  if args.external_proposals:command+=['--external-proposals',args.external_proposals]
  if args.push:command+=['--push']
  result.update(label=label,command=command,primary_ledger_key=index[args.primary_id][0])
 elif args.mode=='queue-command':
  if any(cfg.get('required_parts',{}).get(x)!='complete'for x in 'ABC'):raise GuardError('ABC not complete')
  if result['stop_reason']:raise GuardError('Stop condition reached')
  result['command']=[sys.executable,'-X','utf8','tools/round10_prepared_d_queue.py']+(['--execute']if args.execute else[])+(['--push']if args.push else[])
 elif args.mode=='finish-command':
  if not result['stop_reason']:raise GuardError('No measured normal stop')
  result['command']=[sys.executable,'-X','utf8','tools/round10_finish.py','--recommendations',args.recommendations]+(['--push']if args.push else[])
 print(json.dumps(result,indent=2,ensure_ascii=True))
if __name__=='__main__':main()
