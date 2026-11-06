"""Calculate source declaration moves from authenticated slot anchors; never Emit game IL."""
import json,sys,inspect,hashlib
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'))
import round7_coalesce_calculate as frozen
from round7_slot_model import SlotModelClient
from round7_slot_solver import _edit,_patch,lifetimes
from round7_coalesce_calculate import rebase_unique_patch
from compare_kairo import sha
WIP=ROOT/'build/round8/smcs-slot-anchors';BASE=ROOT/'build/round8/baseline/Assembly-CSharp'
SYNTAX_SOURCE=Path(__file__).with_name('round8_slot_anchor_syntax.cs.txt')
TARGETS=[('game.Ball::Update()->System.Boolean','src/Assembly-CSharp/game/Ball.cs',1),('form.BootForm::DivServerData(System.String)->System.Byte[][]','src/Assembly-CSharp/form/BootForm.cs',0)]
read=lambda p:json.loads(Path(p).read_text(encoding='utf-8-sig'))
def syntax_class():
 namespace=dict(vars(frozen));namespace.update(FOLDER=WIP,SYNTAX_SOURCE=SYNTAX_SOURCE)
 text=inspect.getsource(frozen.Syntax).replace("(ROOT/'tools/round7_coalesce_syntax.cs')","SYNTAX_SOURCE")
 exec(text,namespace);return namespace['Syntax']
def disjoint(method,slots):
 ranges=sorted((x['first_use'],x['last_use']) for x in lifetimes(method) if x['slot'] in set(slots))
 if len(ranges)!=len(set(slots)) or any(b[0]<=a[1] for a,b in zip(ranges,ranges[1:])):raise ValueError('interleaved_or_missing_local_lifetimes')
 return ranges
def common_explicit(model,variables):
 blocks={b['id']:b for b in model['blocks']};chains=[]
 for v in variables:
  chain=[];block=blocks[v['block']]
  while True:
   if block['kind'] in ('explicit','method'):chain.append(block)
   if block['parent']<0:break
   block=blocks[block['parent']]
  chains.append(chain)
 return next(b for b in chains[0] if all(any(x['id']==b['id'] for x in chain) for chain in chains))
def main():
 WIP.mkdir(parents=True,exist_ok=True)
 manifest=read(BASE/'build-manifest.json')
 for x in manifest['sources']:x['input_path']=str(BASE/x['path'])
 mp=WIP/'frozen-manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
 types=read(BASE/'plain-local-types.json');evidence=read(WIP/'anchors.json');anchors={m['method_id']:m for m in evidence['methods']}
 baseline_original={m['stable_id']:m for m in read(BASE/'original-methods.json')['selected_methods']};baseline_rebuilt={m['stable_id']:m for m in read(BASE/'rebuilt-methods.json')['selected_methods']}
 syntax=syntax_class()(mp);patches=[];rows=[]
 try:
  with SlotModelClient(mp) as c:
   for key,logical,n in TARGETS:
    folder=WIP/str(n);source=(BASE/logical).read_bytes().decode('utf-8-sig');initial=c.predict(ROOT/logical,source,key);original=read(folder/'original.json');rebuilt=read(folder/'rebuilt.json')
    if source!=(folder/'source.cs').read_bytes().decode('utf-8-sig'):raise ValueError('snapshot_source_drift')
    if original!=baseline_original[key] or rebuilt!=baseline_rebuilt[key]:raise ValueError('baseline_method_descriptor_drift')
    mapping={}
    for anchor in anchors[key]['anchors']:
     slot=anchor['rebuilt_slot'];target=anchor['original_slot']
     if slot in mapping and mapping[slot]!=target:raise ValueError('conflicting_store_anchor')
     mapping[slot]=target
    stages=[];model=initial
    if n==0:
     assert mapping=={8:7,7:8}
     variable=next(v for v in model['variables'] if v['slot']==8);before=next(v for v in model['variables'] if v['slot']==7)
     changed=syntax.change(ROOT/logical,source,model,'anchored-hoist',group_starts=[variable['start']],insert_at=before['statement_start'],name='chunkLength')
     stages.append({'operation':'hoist declaration before outer array declaration; preserve initializer inside for','desired_original_slot':7})
    else:
     assert mapping=={7:8,8:9,9:8,10:9}
     unused=[i for i in range(len(types[key])) if i not in {x['slot'] for x in lifetimes(original)}];assert unused==[7,12] and all(types[key][i]=='System.Single' for i in unused)
     changed=source
     for target,name in [(8,'dragX'),(9,'dragZ')]:
      initial_slots=sorted(slot for slot,dest in mapping.items() if dest==target);ranges=disjoint(rebuilt,initial_slots)
      selected_initial=[v for v in initial['variables'] if v['slot'] in initial_slots];names={v['name'] for v in selected_initial};assert len(names)==1
      variables=[v for v in model['variables'] if v['name'] in names]
      ancestor=common_explicit(model,variables);insert=ancestor['start']+1
      if target==9:insert=next(v for v in model['variables'] if v['name']=='dragX')['statement_end']
      changed=syntax.change(ROOT/logical,changed,model,'anchored-hoist',group_starts=[v['start'] for v in variables],insert_at=insert,name=name,prefix_declarations=('float unusedOriginalSlot7;\r\n' if target==8 else ''))
      model=c.predict(ROOT/logical,changed,key)
      stages.append({'operation':'merge same-identity locals at explicit least common block; all initializer positions retained','original_slot':target,'rebuilt_slots':initial_slots,'disjoint_rebuilt_lifetimes':ranges,'common_block':ancestor})
     variable=next(v for v in model['variables'] if v['name']=='flag9')
     changed=_edit(changed,[(variable['statement_end'],variable['statement_end'],'\r\nfloat unusedOriginalSlot12;')])
     stages.append({'operation':'reserve proven unused original slots without initializer','original_slots':unused,'type':'System.Single','all_original_local_accesses_scanned':True})
    final=c.predict(ROOT/logical,changed,key);equal=final['complete'] and final['predicted_types']==types[key]
    if not equal:raise ValueError('independent_model_rejected_calculated_candidate:'+key)
    special={v['name']:v['slot'] for v in final['variables'] if v['name'] in {'chunkLength','array3','dragX','dragZ','unusedOriginalSlot7','unusedOriginalSlot12'}}
    if n==0:assert special=={'chunkLength':7,'array3':8}
    else:assert special=={'unusedOriginalSlot7':7,'dragX':8,'dragZ':9,'unusedOriginalSlot12':12}
    patch=_patch(source,initial,changed,'calculated-store-anchored-local-lca')
    patch.update(method_id=key,logical_path=logical,baseline_source_sha256=sha(BASE/logical),source_only=True,candidate_compilations=0,model_source_sha256=sha(ROOT/'tools/Round7SlotModel.cs'),client_sha256=sha(ROOT/'tools/round7_slot_model.py'),predicted_types=final['predicted_types'],complete=True,stages=stages,anchors=anchors[key]['anchors'],special_variables=special)
    # Current Main can move independently in other methods. Rebase only an exact unique declaration.
    current=(ROOT/logical).read_bytes().decode('utf-8-sig');rebased=rebase_unique_patch(current,patch);fresh=_edit(current,[(rebased['start'],rebased['end'],rebased['after'])]);fresh_model=c.predict(ROOT/logical,fresh,key)
    assert fresh_model['complete'] and fresh_model['predicted_types']==types[key]
    patch['current_unique_rebase_verified']=True;patches.append(patch)
    (folder/'candidate.cs').write_bytes(changed.encode('utf-8'));(folder/'prediction.json').write_text(json.dumps(final,indent=2),encoding='utf-8')
    rows.append({'method_id':key,'original_il_bytes':original['header']['code_size'],'model_complete':final['complete'],'signature_exact':equal,'local_count':len(final['predicted_types']),'special_variables':special,'game_body_match':'UNKNOWN: no target compiled','current_unique_rebase_verified':True})
 finally:syntax.close()
 dependencies={str(p.relative_to(ROOT)):sha(p) for p in [ROOT/'tools/round7_coalesce_calculate.py',ROOT/'tools/round7_slot_solver.py',ROOT/'tools/compare_kairo.py',ROOT/'tools/round7_store_anchor_calculate.py']}
 for patch in patches:patch.update(calculator_sha256=sha(Path(__file__)),syntax_source_sha256=sha(SYNTAX_SOURCE),dependency_sha256=dependencies)
 result={'operation':'source-only-unique-store-anchors-local-lca','family':'calculated-store-anchored-local-lca','candidate_game_compilations':0,'fixture':read(WIP/'fixture-result.json'),'frozen_baseline':str(BASE.relative_to(ROOT)),'patches':patches,'methods':rows,'potential_il_bytes':sum(r['original_il_bytes'] for r in rows),'calculator_sha256':sha(Path(__file__)),'syntax_source_sha256':sha(SYNTAX_SOURCE),'dependency_sha256':dependencies,'model_source_sha256':sha(ROOT/'tools/Round7SlotModel.cs'),'client_sha256':sha(ROOT/'tools/round7_slot_model.py'),'baseline_descriptor_sha256':{name:sha(BASE/name) for name in ['original-methods.json','rebuilt-methods.json','plain-local-types.json']},'slot_anchor_evidence_sha256':sha(WIP/'anchors.json'),'prefix_after_compilation':'UNKNOWN'}
 (WIP/'round8-slot-anchor-patches.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items() if k not in {'patches','fixture','baseline_descriptor_sha256','dependency_sha256'}},indent=2))
if __name__=='__main__':main()
