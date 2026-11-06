import json,sys,inspect
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists());sys.path.insert(0,str(ROOT/'tools'))
import round7_coalesce_calculate as frozen
from round7_slot_model import SlotModelClient
from round7_slot_solver import _patch,_edit
from round7_coalesce_calculate import rebase_unique_patch
from compare_kairo import sha
from round8_slot_anchor_calculate import disjoint
WIP=ROOT/'build/round8/smcs-slot-anchors/next';BASE=ROOT/'build/round8/baseline/Assembly-CSharp';SOURCE=Path(__file__).with_name('round8_coordinate_slot_syntax.cs.txt')
read=lambda p:json.loads(Path(p).read_text(encoding='utf-8-sig'))
namespace=dict(vars(frozen));namespace.update(FOLDER=WIP,SYNTAX_SOURCE=SOURCE);exec(inspect.getsource(frozen.Syntax).replace("(ROOT/'tools/round7_coalesce_syntax.cs')","SYNTAX_SOURCE"),namespace);Syntax=namespace['Syntax']
TARGETS=[('form.DebugForm::Draw_coatEva(kairo.unity.ui.Graphics)->System.Void','src/Assembly-CSharp/form/DebugForm.cs',['num7','num8'],['passer','num10']),('game.Ball::SetSavePos(kairo.unity.m3d4.util3d.Vector3D,kairo.unity.m3d4.util3d.Vector3D,kairo.unity.m3d4.util3d.Vector3D)->System.Void','src/Assembly-CSharp/game/Ball.cs',['num8','num9'],[]),('form.DebugForm::Draw_moveArea(kairo.unity.ui.Graphics,System.Int32,System.Int32)->System.Void','src/Assembly-CSharp/form/DebugForm.cs',['num6','num7'],['player','player2'])]
def main():
 manifest=read(BASE/'build-manifest.json')
 for item in manifest['sources']:item['input_path']=str(BASE/item['path'])
 mp=WIP/'frozen-manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
 syntax=Syntax(mp);types=read(BASE/'plain-local-types.json');patches=[];rows=[]
 original={m['stable_id']:m for m in read(BASE/'original-methods.json')['selected_methods']};rebuilt={m['stable_id']:m for m in read(BASE/'rebuilt-methods.json')['selected_methods']}
 ep=ROOT/'reports/round8-coordinate-slot-evidence.json';ep=ep if ep.exists() else WIP/'anchors.json';evidence={r['method_id']:r for r in read(ep)['methods']}
 try:
  with SlotModelClient(mp) as c:
   for n,(key,logical,names,sinks) in enumerate(TARGETS):
    f=WIP/str(n);source=(BASE/logical).read_bytes().decode('utf-8-sig');initial=c.predict(logical,source,key);model=initial;changed=source;proofs=[];mapping={}
    for anchor in evidence[key]['anchors']:
     slot,target=anchor['rebuilt_slot'],anchor['original_slot']
     if slot in mapping and mapping[slot]!=target:raise ValueError('conflicting_anchor')
     mapping[slot]=target
    for old in sinks:
     v=next(v for v in model['variables'] if v['name']==old);initial_v=next(v for v in initial['variables'] if v['name']==old);target=mapping[initial_v['slot']];changed=syntax.change(logical,changed,model,'sink-first-assignment',group_starts=[v['start']]);model=c.predict(logical,changed,key);proofs.append({'operation':'sink-uninitialized-declaration-to-first-simple-assignment','name':old,'original_slot':target,'rebuilt_slot':initial_v['slot'],'capture_rejected':True,'rhs_self_reference_rejected':True,'all_uses_in_target_block_required':True})
    before_name={0:'num4',1:'flag',2:'num4'}[n]
    for index,old in enumerate(names):
     vs=[v for v in model['variables'] if v['name']==old];before=next(v for v in model['variables'] if v['name']==before_name);initial_slots=[v['slot'] for v in initial['variables'] if v['name']==old];targets={mapping[slot] for slot in initial_slots};assert len(targets)==1;target=next(iter(targets));ranges=disjoint(rebuilt[key],initial_slots);insert=before['statement_start'] if index==0 else next(v for v in model['variables'] if v['name']=='coordinateX')['statement_end'];changed=syntax.change(logical,changed,model,'anchored-hoist',group_starts=[v['start'] for v in vs],insert_at=insert,name=['coordinateX','coordinateY'][index]);model=c.predict(logical,changed,key);proofs.append({'operation':'coalesce-disjoint-same-identity-local-declarations-before-parent-control-flow','merged_name':old,'lifetimes':ranges,'original_slot':target,'rebuilt_slots':initial_slots})
    print(n,model['complete'],len(model['predicted_types']),len(types[key]),model['predicted_types']==types[key],model['unsupported_reasons']);print([(v['name'],v['slot']) for v in model['variables'] if v['name'] in ['coordinateX','coordinateY']+sinks]);(f/'candidate.cs').write_bytes(changed.encode('utf-8'));(f/'prediction.json').write_text(json.dumps(model,indent=2),encoding='utf-8')
    if model['complete'] and model['predicted_types']==types[key]:
     patch=_patch(source,initial,changed,'calculated-coordinate-local-coalescence');special={v['name']:v['slot'] for v in model['variables'] if v['name'] in ['coordinateX','coordinateY']+sinks}
     expected=[{'coordinateX':6,'coordinateY':7,'passer':14,'num10':15},{'coordinateX':8,'coordinateY':9},{'coordinateX':4,'coordinateY':5,'player':11,'player2':17}][n];assert special==expected
     patch.update(method_id=key,logical_path=logical,complete=True,predicted_types=model['predicted_types'],proofs=proofs,anchors=evidence[key]['anchors'],special_variables=special,source_only=True,candidate_compilations=0,baseline_source_sha256=sha(BASE/logical),model_source_sha256=sha(ROOT/'tools/Round7SlotModel.cs'),client_sha256=sha(ROOT/'tools/round7_slot_model.py'))
     current=(ROOT/logical).read_bytes().decode('utf-8-sig');bound=rebase_unique_patch(current,patch);fresh=_edit(current,[(bound['start'],bound['end'],bound['after'])]);fm=c.predict(logical,fresh,key);assert fm['complete'] and fm['predicted_types']==types[key];patch['current_unique_rebase_verified']=True;patches.append(patch)
     rows.append({'method_id':key,'original_il_bytes':original[key]['header']['code_size'],'model_complete':True,'signature_exact':True,'local_count':len(model['predicted_types']),'special_variables':special,'game_body_match':'UNKNOWN','current_unique_rebase_verified':True})
    else:raise ValueError('independent_model_rejected')
 finally:syntax.close()
 dependencies={str(p.relative_to(ROOT)):sha(p) for p in [ROOT/'tools/round7_coalesce_calculate.py',ROOT/'tools/round7_slot_solver.py',ROOT/'tools/round8_slot_anchor_calculate.py',ROOT/'tools/compare_kairo.py']}
 for patch in patches:patch.update(calculator_sha256=sha(Path(__file__)),syntax_source_sha256=sha(SOURCE),dependency_sha256=dependencies)
 fp=ROOT/'reports/round8-coordinate-slot-fixture.json';fp=fp if fp.exists() else WIP/'fixture-result.json'
 result={'operation':'source-only-coordinate-identity-coalescence-and-first-assignment-sink','family':'calculated-coordinate-local-coalescence','candidate_game_compilations':0,'frozen_baseline':str(BASE.relative_to(ROOT)),'patches':patches,'methods':rows,'potential_il_bytes':sum(r['original_il_bytes'] for r in rows),'fixture':read(fp),'calculator_sha256':sha(Path(__file__)),'syntax_source_sha256':sha(SOURCE),'dependency_sha256':dependencies,'model_source_sha256':sha(ROOT/'tools/Round7SlotModel.cs'),'client_sha256':sha(ROOT/'tools/round7_slot_model.py'),'slot_anchor_evidence_sha256':sha(ep),'baseline_descriptor_sha256':{name:sha(BASE/name) for name in ['original-methods.json','rebuilt-methods.json','plain-local-types.json']},'prefix_after_compilation':'UNKNOWN'}
 (WIP/'round8-coordinate-slot-patches.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items() if k not in {'patches','fixture','baseline_descriptor_sha256','dependency_sha256'}},indent=2))
if __name__=='__main__':main()
