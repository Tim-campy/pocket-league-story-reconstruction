"""Reviewable prepared-D queue runner. Default is read-only; --execute is explicit.

Never starts D until ABC are complete. The existing round9_cycle tool remains
responsible for builds, oracle adoption, the journal, commits and optional push.
This planner never writes method ledger states. It handles only eight prepared
families, then returns control for discovery of new families.
"""
from __future__ import annotations
import argparse,ast,csv,hashlib,json,os,re,subprocess,sys,time
from dataclasses import dataclass
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'AGENTS.md').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences
OUT=ROOT/'build/round9/d-planner'
REGISTRY=(
 {'id':'d-roslyn-float-identity','assembly':'KairoLibrary','rule':'d9-explicit-float-identity-parameter','ready':'reports/round9-d-roslyn-ready.json','bundle':'reports/round9-d-float-identity-patches.json','doc':'docs/notes/round9-part-d-roslyn.md','external':False,'entry':'AddRound9FloatingIdentityRules','helper':['tools/Round9FloatingIdentityRules.cs'],'hint_module':'round9_float_identity_hints','hint_function':'float_identity_conversion_hints','hypothesis':'Conversao explicita Single para Single na ocorrencia comprovada preserva conv.r4 apos carga de parametro Single.'},
 {'id':'d-roslyn-per-arm-stores','assembly':'KairoLibrary','rule':'d9-calculated-per-arm-local-stores','ready':'reports/round9-d-roslyn-ready.json','bundle':'reports/round9-d-per-arm-store-patches.json','doc':'docs/notes/round9-part-d-roslyn.md','external':False,'entry':'AddRound9PerArmStoreRules','helper':['tools/Round9PerArmStoreRules.cs'],'hint_module':'round9_per_arm_store_hints','hint_function':'per_arm_local_store_hints','hypothesis':'If/else com armazenamento do local em cada braco, em vez do join de valor ternario, reproduz stores por braco comprovados.'},
 {'id':'d-smcs-joint-flow-local-stores','assembly':'Assembly-CSharp','rule':'d9-joint-flow-local-stores','ready':'reports/round9-d-joint-flow-local-ready.json','bundle':'reports/round9-d-joint-flow-local-patches.json','doc':'docs/notes/round9-part-d-joint-flow-locals.md','external':True,'hypothesis':'Compor fluxo B com locais nomeados comprovados: guarda de continue e alias GetMatchInfoJoin em GameForm; seletores nomeados e if-chains em SubForm, preservando retornos.'},
 {'id':'d-smcs-terminal-if-return','assembly':'Assembly-CSharp','rule':'terminal-if-arm-return-flatten-else','ready':'reports/round9-d-scoped-exit-bundle.json','bundle':'reports/round9-d-terminal-if-return-patches.json','doc':'docs/notes/round9-scoped-exits.md','external':False,'entry':'AddRound9ScopedExitRules','helper':['tools/Round9ScopedExitCandidates.cs','tools/Round9ScopedExitAdapter.cs'],'hint_module':'round9_scoped_exit_hints','hint_function':'scoped_exit_hints','hypothesis':'Ret original contra br sintetico de if/else terminal: inserir return no fim do then e retirar else, preservando bloco lexical, condicao e trabalho.'},
 {'id':'d-smcs-scoped-common-join','assembly':'Assembly-CSharp','rule':'scoped-terminal-arm-common-join','ready':'reports/round9-d-scoped-exit-bundle.json','bundle':'reports/round9-d-scoped-common-join-patches.json','doc':'docs/notes/round9-scoped-exits.md','external':False,'entry':'AddRound9ScopedExitRules','helper':['tools/Round9ScopedExitCandidates.cs','tools/Round9ScopedExitAdapter.cs'],'hint_module':'round9_scoped_exit_hints','hint_function':'scoped_exit_hints','hypothesis':'Br original para join provado contra return recuperado: conservar operador e acoes e estruturar else ate o destino, aceitando somente join local ou braco nao linear novo.'},

 {'id':'d-smcs-multiple-selector-splitting','assembly':'Assembly-CSharp','rule':'d9-calculated-multiple-selector-local-splitting','ready':'reports/round9-multi-selector-index.json','bundle':'reports/round9-multi-selector-patches.json','doc':'docs/notes/round9-multi-selector.md','external':True,'hypothesis':'Nomear conjuntamente os tres seletores Int32 nos slots originais e converter seus switches em if-chains, avaliando cada seletor uma vez e preservando donos de breaks.'},
 {'id':'d-roslyn-named-eh-result','assembly':'KairoLibrary','rule':'d9-named-eh-result-rejoin','ready':'reports/round9-d-eh-dispatch-ready.json','bundle':'reports/round9-d-eh-dispatch-patches.json','doc':'docs/notes/round9-part-d-eh-dispatch.md','external':True,'hypothesis':'Manter resultado Byte[] inicializado antes do try, atribuir ToByteArray ao mesmo local e retorna-lo depois do finally, evitando o temporario de retorno protegido separado.'},
 {'id':'d-roslyn-state-dispatch-index','assembly':'KairoLibrary','rule':'d9-calculated-state-dispatch-index','ready':'reports/round9-d-eh-dispatch-ready.json','bundle':'reports/round9-d-eh-dispatch-patches.json','doc':'docs/notes/round9-part-d-eh-dispatch.md','external':True,'hypothesis':'Declarar indice e estado no bloco pai e recuperar dispatch if(estado==0)/else if(estado==1), preservando a leitura do estado e somente retirando os breaks finais comprovados do switch.'},
)
class GuardError(RuntimeError):pass

def read(path):return json.loads(Path(path).read_bytes().decode('utf-8-sig'))
def sha(path):return hashlib.sha256(Path(path).read_bytes()).hexdigest()
def emit(event,**values):print(json.dumps({'event':event,**values},ensure_ascii=True),flush=True)
def atomic_json(path,data):
 path=Path(path);path.parent.mkdir(parents=True,exist_ok=True);tmp=path.with_name(path.name+'.planner-tmp-'+str(os.getpid()))
 if tmp.exists():raise GuardError('Temporary output already exists: '+str(tmp))
 tmp.write_text(json.dumps(data,indent=2,ensure_ascii=False)+'\n',encoding='utf-8',newline='\n');os.replace(tmp,path)
def qualified(assembly,mid):return assembly+'::'+mid

def stop_reason(cfg,journal):
 cycles=[c for c in journal.get('cycles',[]) if c.get('phase')=='D']
 if any(cfg.get('required_parts',{}).get(p)!='complete' for p in 'ABC'):return 'ABC_not_complete'
 if len(cycles)>=int(cfg.get('maximum_cycles',40)):return 'maximum_D_cycles_reached'
 empty=not any(q.get('status')=='pending' for q in cfg.get('prepared_queue',[]))
 if empty and journal.get('consecutive_zero_cycles',0)>=int(cfg.get('consecutive_zero_stop',5)):return 'user_consecutive_zero_stop'
 return None

def choose_family(families,journal):
 """Greedy current IL weight, then largest permitted primary; no lookahead guesses."""
 history=[r for r in journal.get('cycles',[]) if r.get('phase')=='D']
 quota_due=len(history)>=4 and not any(r['assembly']=='KairoLibrary' for r in history[-4:])
 forbidden=history[-1].get('primary_target') if len(history)>=2 and history[-1].get('primary_target')==history[-2].get('primary_target') else None
 choices=[]
 for family in families:
  if not family['eligible']:continue
  if quota_due and family['assembly']!='KairoLibrary':continue
  allowed=[r for r in family['eligible'] if r['id']!=forbidden]
  if not allowed:continue
  primary=sorted(allowed,key=lambda r:(-r['il_bytes'],r['id']))[0]
  choices.append((family,primary))
 if not choices:return None,{'quota_due':quota_due,'forbidden_primary':forbidden,'reason':'no_untested_prepared_family_satisfies_quota_or_primary_guard'}
 choices.sort(key=lambda v:(-v[0]['weight_il'],-v[1]['il_bytes'],v[0]['id']))
 family,primary=choices[0]
 return dict(family,primary=primary),{'quota_due':quota_due,'forbidden_primary':forbidden,'reason':'largest_current_divergent_original_IL_weight'}

def cycle_command(family,journal,push=False):
 command=[sys.executable,'-X','utf8','tools/round9_cycle.py','--number',str(len(journal.get('cycles',[]))+1),'--phase','D','--queue-id',family['id'],'--assembly',family['assembly'],'--rule',family['rule'],'--pair',family['primary']['pair'],'--hypothesis',family['hypothesis'],'--probe-substring',family['primary']['id']]
 if family.get('external'):command+=['--external-proposals',family['bundle']]
 if push:command+=['--push']
 if any(s in command for s in ('--target-substring','--method-ids')):raise AssertionError('Main pass must reach all pending methods')
 return command

def integration_check(family):
 if family.get('external'):return {'ready':True,'mode':'external-proposals'}
 from prepare_round9_transform import source_inputs
 paths,main=source_inputs();names={p.relative_to(ROOT).as_posix() for p in paths};missing=set(family['helper'])-names
 if missing:return {'ready':False,'reason':'helper_sources_not_integrated','missing':sorted(missing)}
 texts=[main]+[p.read_bytes().decode('utf-8-sig') for p in paths[1:]]
 entry=family['entry'];count=sum(len(re.findall(r'\b'+re.escape(entry)+r'\s*\(',t)) for t in texts)
 if count<2:return {'ready':False,'reason':'entry_declaration_without_dispatch_call','entry':entry}
 engine=ast.parse((ROOT/'tools/round9_engine.py').read_bytes().decode('utf-8-sig'));aliases=set();module_aliases=set()
 for node in ast.walk(engine):
  if isinstance(node,ast.ImportFrom) and node.module==family['hint_module']:
   aliases.update(a.asname or a.name for a in node.names if a.name==family['hint_function'])
  if isinstance(node,ast.Import):module_aliases.update(a.asname or a.name for a in node.names if a.name==family['hint_module'])
 called=any(isinstance(n,ast.Call) and ((isinstance(n.func,ast.Name) and n.func.id in aliases) or (isinstance(n.func,ast.Attribute) and isinstance(n.func.value,ast.Name) and n.func.value.id in module_aliases and n.func.attr==family['hint_function'])) for n in ast.walk(engine))
 return {'ready':called,'mode':'dynamic-hints','entry':entry,'hint_call_present':called,'reason':'integrated' if called else 'fresh_hint_not_called_by_engine'}

def _verified_file(relative,expected,inputs):
 path=(ROOT/relative).resolve()
 if not path.is_relative_to(ROOT):raise GuardError('Proof escapes workspace: '+relative)
 if sha(path)!=expected:raise GuardError('Proof hash changed: '+relative)
 inputs[relative.replace('\\','/')]=expected

def family_patches(bundle,rule):
    """Shared external bundles may contain different reviewed families."""
    return [p for p in bundle['patches'] if p.get('rule','').startswith(rule)]

def declared_hashes(document):
    hashes={}
    for name in ('file_sha256','files_sha256'):
        value=document.get(name,{})
        if not isinstance(value,dict):raise GuardError('Unsupported ready hash map: '+name)
        hashes.update(value)
    files=document.get('files',[])
    if isinstance(files,dict):hashes.update(files)
    elif isinstance(files,list):
        for row in files:hashes[row['path']]=row['sha256']
    else:raise GuardError('Unsupported ready files hash layout')
    return hashes


def load_state(*,check_integration=True):
 inputs={}
 def tracked(relative):inputs[relative]=sha(ROOT/relative);return read(ROOT/relative)
 cfg=tracked('config/round9-protocol.json');journal=tracked('reports/round9-cycle-journal.json')
 for rel in ('tools/round9_cycle.py','tools/round9_engine.py','tools/round9_adopt.py','tools/round9_commit.py','tools/prepare_round9_transform.py','tools/round9_journal.py','tools/round9_freeze.py','tools/round9_measure.py','tools/verify','tools/verify.py'):inputs[rel]=sha(ROOT/rel)
 ledger_path=ROOT/'config/methods.csv';inputs['config/methods.csv']=sha(ledger_path)
 with ledger_path.open(encoding='utf-8-sig',newline='') as stream:ledger={r['id']:r for r in csv.DictReader(stream)}
 registry={q['id']:q for q in cfg.get('prepared_queue',[])};missing=[f['id'] for f in REGISTRY if f['id'] not in registry]
 assemblies={}
 for assembly in ('KairoLibrary','Assembly-CSharp'):
  relative='build/verification-'+assembly+'.json';receipt=tracked(relative)
  if any(receipt.get(k) for k in ('regressions','body_regressions','type_regressions')):raise GuardError('Current oracle reports a regression: '+assembly)
  _verified_file(receipt['build_artifact'],receipt['build_sha256'],inputs)
  for source in receipt['sources']:_verified_file(source['path'],source['sha256'],inputs)
  for helper,digest in receipt.get('helper_source_hashes',{}).items():_verified_file(helper,digest,inputs)
  _verified_file('tools/'+('verify_kairo.py' if assembly=='KairoLibrary' else 'verify_csharp.py'),receipt['verifier_sha256'],inputs)
  _verified_file('tools/ProbeInspector.cs',receipt['inspector_source_sha256'],inputs)
  _verified_file('original/extracted/assets/bin/Data/Managed/'+assembly+'.dll',receipt['original_sha256'],inputs)
  folder=(ROOT/receipt['build_artifact']).parent
  dump={}
  for label in ('original','rebuilt'):
   path=folder/(label+'-methods.json');relative=path.relative_to(ROOT).as_posix();data=tracked(relative);dump[label]={m['stable_id']:m for m in data['selected_methods']}
  assemblies[assembly]={'receipt':receipt,**dump}
 families=[]
 for specification in REGISTRY:
  family=dict(specification);ready=tracked(family['ready']);bundle=tracked(family['bundle']);tracked_doc=ROOT/family['doc'];inputs[family['doc']]=sha(tracked_doc)
  if ready.get('state') not in (None,'READY'):raise GuardError('Family is not READY: '+family['id'])
  proof_hashes=declared_hashes(ready)
  for rel,digest in proof_hashes.items():_verified_file(rel,digest,inputs)
  dependencies=dict(bundle.get('dependency_sha256',{}))
  for proposal in bundle['patches']:dependencies.update(proposal.get('dependency_sha256',{}))
  for rel,digest in dependencies.items():_verified_file(rel,digest,inputs)
  proposals=family_patches(bundle,family['rule'])
  ids=sorted({p['method_id'] for p in proposals})
  if not ids:raise GuardError('READY bundle has no reviewed family method IDs: '+family['id'])
  data=assemblies[family['assembly']];eligible=[];equal=[]
  for mid in ids:
   qid=qualified(family['assembly'],mid);entry=ledger.get(qid);original=data['original'].get(mid);candidate=data['rebuilt'].get(mid);status=data['receipt']['methods'].get(qid)
   if entry is None or original is None or candidate is None or status is None:raise GuardError('Current method identity missing: '+qid)
   if not original.get('instructions'):raise GuardError('Prepared body target has no original body: '+qid)
   size=original['header']['code_size']
   if int(entry['tamanho_il'])!=size:raise GuardError('Ledger/original IL size differs: '+qid)
   body_equal=not differences(original,candidate)[0]
   if status.get('body_equal') is not body_equal:raise GuardError('Current receipt/body dump disagree: '+qid)
   if entry['estado']=='matched' and not body_equal:raise GuardError('Matched method currently differs: '+qid)
   item={'id':mid,'il_bytes':size,'ledger_state':entry['estado'],'body_equal':body_equal}
   if body_equal:equal.append(item);continue
   from round6_alignment import align_methods
   alignment=align_methods(original,candidate);item['pair']=alignment['opcode_pair'];item['first_aligned_original_offset']=alignment['original_offset'];item['first_aligned_rebuilt_offset']=alignment['rebuilt_offset'];eligible.append(item)
  queue_item=registry.get(family['id'])
  if queue_item:
   status=queue_item.get('status')
   if status not in ('pending','tested','superseded'):raise GuardError('Unrecognized prepared queue status: '+family['id']+' '+str(status))
   recorded=[r for r in journal.get('cycles',[]) if r.get('phase')=='D' and r.get('rule')==family['rule']]
   if status=='pending' and recorded:raise GuardError('Pending family already has a measured D cycle; no automatic repeat: '+family['id'])
   if status=='tested':
    if not recorded:raise GuardError('Tested family has no measured D journal entry: '+family['id'])
    report_path=queue_item.get('report');inputs[report_path]=sha(ROOT/report_path)
    for event in recorded:
     for key in ('before','after'):_verified_file(event[key+'_measurement'],event[key+'_sha256'],inputs)
   if status=='superseded':
    proof_path=queue_item.get('report');proof=tracked(proof_path)
    if proof.get('operation')!='verified-prepared-family-supersession' or proof.get('queue_id')!=family['id'] or proof.get('all_target_bodies_equal') is not True or proof.get('new_cycle_recorded') is not False:raise GuardError('Superseded family lacks a recognized official-verifier proof: '+family['id'])
    if eligible:raise GuardError('Superseded family has a newly divergent body: '+family['id'])
  family.update(known_ids=ids,eligible=eligible,already_equal=equal,weight_il=sum(m['il_bytes'] for m in eligible),queue=registry.get(family['id']),integration=integration_check(family) if check_integration else {'ready':True,'test_only':True})
  if family.get('external') and eligible:
   from round7_coalesce_calculate import rebase_unique_patch
   failures=[]
   for mid in [m['id'] for m in eligible]:
    options=[p for p in proposals if p['method_id']==mid];valid=False
    for proposal in options:
     path=ROOT/proposal['logical_path'];text=path.read_bytes().decode('utf-8-sig')
     try:rebase_unique_patch(text,proposal);valid=True
     except ValueError:pass
    if not valid:failures.append(mid)
   if failures:family['external_rebase_required']=failures
  families.append(family)
 for relative,digest in inputs.items():
  if sha(ROOT/relative)!=digest:raise GuardError('Input changed during planning: '+relative)
 return {'cfg':cfg,'journal':journal,'ledger':ledger,'assemblies':assemblies,'families':families,'missing_queue_ids':missing,'input_sha256':inputs,'planner_sha256':sha(Path(__file__))}

def require_committed_journal(state):
    """A failed cycle may have written a journal before its commit failed."""
    result=subprocess.run(['git','show','HEAD:reports/round9-cycle-journal.json'],cwd=ROOT,capture_output=True)
    if result.returncode:raise GuardError('Cannot prove current journal committed at HEAD')
    if json.loads(result.stdout.decode('utf-8-sig'))!=state['journal']:raise GuardError('Current measured journal is not committed at HEAD; root must finish or audit the previous cycle first')


def run_logged(command,log):
 """Only --execute reaches this function; root never ran it during preparation."""
 log=Path(log);log.parent.mkdir(parents=True,exist_ok=True);env=os.environ.copy();env['PYTHONUTF8']='1'
 with log.open('wb') as stream:
  proc=subprocess.Popen(command,cwd=ROOT,stdout=subprocess.PIPE,stderr=subprocess.STDOUT,env=env)
  for line in iter(proc.stdout.readline,b''):
   stream.write(line);stream.flush();sys.stdout.buffer.write(line);sys.stdout.buffer.flush()
  return proc.wait()

def resolve_superseded(state):
 """Re-run official verification; update queue bookkeeping only, never methods."""
 candidates=[f for f in state['families'] if f['queue'] and f['queue']['status']=='pending' and not f['eligible']]
 if not candidates:return state,[]
 journal_sha=sha(ROOT/'reports/round9-cycle-journal.json');ledger_before=sha(ROOT/'config/methods.csv');stamp=str(time.time_ns());log=OUT/'superseded'/stamp/'official-full-verifier.log'
 command=[sys.executable,'-X','utf8','tools/verify','--all'];emit('supersession_verifier_start',families=[f['id'] for f in candidates],command=command,log=str(log))
 code=run_logged(command,log)
 if code not in (0,1):raise GuardError('Official supersession verifier failed, return '+str(code))
 checked=load_state();resolved=[];cfg=checked['cfg']
 if sha(ROOT/'reports/round9-cycle-journal.json')!=journal_sha:raise GuardError('Journal changed during supersession verification')
 for before in candidates:
  family=next(f for f in checked['families'] if f['id']==before['id'])
  if family['eligible']:raise GuardError('Family still has divergent bodies after verification: '+family['id'])
  proof={'operation':'verified-prepared-family-supersession','queue_id':family['id'],'family':family['rule'],'assembly':family['assembly'],'known_method_ids':family['known_ids'],'all_target_bodies_equal':True,'body_proofs':family['already_equal'],'official_verifier_command':command,'official_verifier_returncode':code,'official_verifier_log':str(log.relative_to(ROOT)),'official_verifier_log_sha256':sha(log),'current_receipt_sha256':sha(ROOT/('build/verification-'+family['assembly']+'.json')),'ledger_before_sha256':ledger_before,'ledger_after_sha256':sha(ROOT/'config/methods.csv'),'journal_sha256_unchanged':journal_sha,'new_cycle_recorded':False,'zero_cycle_invented':False,'method_state_writes':'official verifier only','input_sha256':checked['input_sha256'],'planner_sha256':sha(Path(__file__))}
  relative='reports/round9-d-superseded-'+family['id']+'-'+stamp+'.json';atomic_json(ROOT/relative,proof)
  item=next(q for q in cfg['prepared_queue'] if q['id']==family['id']);item.update(status='superseded',report=relative,resolution='all prepared target bodies equal under fresh official verifier')
  resolved.append({'queue_id':family['id'],'proof':relative});emit('family_superseded',**resolved[-1],cycle_added=False)
 atomic_json(ROOT/'config/round9-protocol.json',cfg)
 return load_state(),resolved

def plan(state,push=False):
 reason=stop_reason(state['cfg'],state['journal']);pending=[f for f in state['families'] if f['queue'] and f['queue'].get('status')=='pending'];summary=[{'id':f['id'],'assembly':f['assembly'],'rule':f['rule'],'weight_il':f['weight_il'],'eligible':f['eligible'],'already_equal_ids':[m['id'] for m in f['already_equal']],'integration':f['integration'],'external_rebase_required':f.get('external_rebase_required',[])} for f in pending]
 choice,selection=choose_family(pending,state['journal']);blocked=[]
 if reason:blocked.append(reason)
 if state['missing_queue_ids']:blocked.append('missing_prepared_queue_ids')
 if choice and not choice['integration']['ready']:blocked.append('selected_dynamic_family_not_integrated')
 if choice and choice.get('external_rebase_required'):blocked.append('selected_external_proposals_need_recalculation')
 return {'operation':'prepared-eight-family-plan','read_only':True,'stop_reason':reason,'blocked':blocked,'missing_queue_ids':state['missing_queue_ids'],'pending':summary,'selection':selection,'selected_queue_id':choice['id'] if choice else None,'primary_target':choice['primary'] if choice else None,'command':cycle_command(choice,state['journal'],push) if choice else None,'supersession_requires_official_verifier':[f['id'] for f in pending if not f['eligible']],'all_prepared_resolved':not pending and not state['missing_queue_ids'],'input_sha256':state['input_sha256'],'planner_sha256':state['planner_sha256']}

def main():
 p=argparse.ArgumentParser(description=__doc__);p.add_argument('--execute',action='store_true');p.add_argument('--push',action='store_true');args=p.parse_args();OUT.mkdir(parents=True,exist_ok=True)
 state=load_state();preview=plan(state,args.push)
 if not args.execute:
  destination=OUT/'plan-preview.json';atomic_json(destination,preview);emit('plan',blocked=preview['blocked'],selected_queue_id=preview['selected_queue_id'],primary=preview['primary_target'],families=[{'id':r['id'],'weight_il':r['weight_il'],'eligible_methods':len(r['eligible'])} for r in preview['pending']],proof=str(destination.relative_to(ROOT)),command=preview['command']);return 0
 if preview['stop_reason']:raise GuardError('Cannot execute: '+preview['stop_reason'])
 if state['missing_queue_ids']:raise GuardError('Root must register prepared queue IDs: '+str(state['missing_queue_ids']))
 require_committed_journal(state)
 lock=OUT/'runner.lock';descriptor=os.open(lock,os.O_CREAT|os.O_EXCL|os.O_WRONLY);os.write(descriptor,str(os.getpid()).encode());os.close(descriptor)
 try:
  while True:
   state=load_state();guard=stop_reason(state['cfg'],state['journal'])
   if guard:emit('stop',reason=guard);return 0
   state,resolved=resolve_superseded(state);current=plan(state,args.push)
   if current['all_prepared_resolved']:emit('handoff',reason='eight_prepared_families_processed',new_family_discovery_required=True,resolved=resolved,journal_cycles=len(state['journal']['cycles']));return 0
   if current['blocked']:raise GuardError('Planner guard: '+str(current['blocked']))
   require_committed_journal(state)
   if not current['command']:raise GuardError('Need root/new family to satisfy quota or primary guard: '+str(current['selection']))
   history=[r for r in state['journal']['cycles'] if r.get('phase')=='D'];label='cycle%02d'%(len(history)+1);folder=OUT/'runs'/label
   if folder.exists():raise GuardError('Never overwrite planner cycle logs: '+str(folder))
   folder.mkdir(parents=True);atomic_json(folder/'plan.json',current)
   emit('start',label=label,queue_id=current['selected_queue_id'],primary=current['primary_target']['id'],weight_il=next(f['weight_il'] for f in current['pending'] if f['id']==current['selected_queue_id']),command=current['command'])
   before_count=len(state['journal']['cycles']);code=run_logged(current['command'],folder/'cycle.stdout.log')
   if code:raise GuardError('Cycle failed; do not repeat automatically. See '+str(folder/'cycle.stdout.log'))
   after=load_state();rows=after['journal']['cycles']
   if len(rows)!=before_count+1:raise GuardError('Cycle did not append exactly one measured journal entry')
   entry=rows[-1]
   if entry.get('phase')!='D' or entry.get('label')!=label or entry.get('rule')!=next(f['rule'] for f in state['families'] if f['id']==current['selected_queue_id']):raise GuardError('Unexpected measured cycle identity')
   if entry.get('methods_gained',0)<0 or entry.get('il_bytes_gained',0)<0:raise GuardError('Regression in actual measured journal')
   status=next(q for q in after['cfg']['prepared_queue'] if q['id']==current['selected_queue_id'])
   if status.get('status')!='tested':raise GuardError('Cycle did not resolve its prepared queue item')
   atomic_json(folder/'finish.json',{'journal_entry':entry,'post_input_sha256':after['input_sha256']});emit('finish',label=label,queue_id=current['selected_queue_id'],methods_gained=entry['methods_gained'],il_bytes_gained=entry['il_bytes_gained'],result=entry['result'])
 finally:
  if lock.exists() and lock.read_text()==str(os.getpid()):lock.unlink()
if __name__=='__main__':
 try:raise SystemExit(main())
 except (GuardError,OSError,ValueError,KeyError,TypeError) as error:emit('failed',reason=str(error),automatic_retry=False);raise SystemExit(1)
