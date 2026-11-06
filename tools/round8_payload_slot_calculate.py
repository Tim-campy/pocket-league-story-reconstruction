import json,sys,subprocess,inspect
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists());sys.path.insert(0,str(ROOT/'tools'))
import round7_coalesce_calculate as frozen
def make_syntax(folder,source):
 folder.mkdir(exist_ok=True);ns=dict(vars(frozen));ns.update(FOLDER=folder,SYNTAX_SOURCE=source);exec(inspect.getsource(frozen.Syntax).replace("(ROOT/'tools/round7_coalesce_syntax.cs')","SYNTAX_SOURCE"),ns);return ns['Syntax']
from round7_coalesce_calculate import Syntax as FrozenSyntax
from round7_slot_model import SlotModelClient
from round7_slot_solver import _edit,_patch,lifetimes
from round8_slot_anchor_calculate import disjoint
from compiler_probes import ROSLYN
from prepare_round7_transform import ensure_helper as ensure_frozen_transform
from round7_coalesce_calculate import rebase_unique_patch
from compare_kairo import sha
WIP=ROOT/'build/round8/smcs-slot-anchors/spform';BASE=ROOT/'build/round8/baseline/Assembly-CSharp';LOGICAL='src/Assembly-CSharp/form/SpForm.cs';KEY='form.SpForm::Update()->System.Boolean'
read=lambda p:json.loads(Path(p).read_text(encoding='utf-8-sig'))
def transformer(manifest):
 return subprocess.Popen(ensure_frozen_transform()+[str(manifest)],stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True,encoding='utf-8')

def request(process,path,source,needle,rule):
 at=source.index(needle);offset=len(source[:at].encode('utf-16-le'))//2;process.stdin.write(json.dumps({'path':str(path),'source':source,'offset':offset,'compiler':'smcs','round7_cycle_rule':rule,'max_candidates':2048})+'\n');process.stdin.flush();result=json.loads(process.stdout.readline());patches=[p for p in result.get('patches',[]) if p['rule']==rule]
 if len(patches)!=1:raise ValueError({'rule':rule,'patch_count':len(patches),'response':result})
 patch=patches[0];return _edit(source,[(patch['start'],patch['end'],patch['after'])]),patch
def main():
 manifest=read(BASE/'build-manifest.json')
 for item in manifest['sources']:item['input_path']=str(BASE/item['path'])
 mp=WIP/'frozen-manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8');syntax=make_syntax(WIP/'merge-helper',ROOT/'tools/round8_coordinate_slot_syntax.cs.txt')(mp);switch_syntax=make_syntax(WIP/'switch-helper',Path(__file__).with_name('round8_payload_switch_syntax.cs.txt'))(mp);original=next(m for m in read(BASE/'original-methods.json')['selected_methods'] if m['stable_id']==KEY);rebuilt=next(m for m in read(BASE/'rebuilt-methods.json')['selected_methods'] if m['stable_id']==KEY);expected=read(BASE/'plain-local-types.json')[KEY];source=(BASE/LOGICAL).read_bytes().decode('utf-8-sig');changed=source;stages=[];process=transformer(mp)
 try:
  with SlotModelClient(mp) as c:
   ep=ROOT/'reports/round8-payload-slot-evidence.json';ep=ep if ep.exists() else WIP/'anchors.json';evidence=read(ep);mapping={}
   for anchor in evidence['anchors']:
    slot,target=anchor['rebuilt_slot'],anchor['original_slot']
    if slot in mapping and mapping[slot]!=target:raise ValueError('contradictory_anchor')
    mapping[slot]=target
   assert mapping=={11:11,13:11,17:11,21:11}
   assert not any(i['opcode']=='switch' for i in original['instructions']) and sum(i['opcode']=='switch' for i in rebuilt['instructions'])==1
   initial=c.predict(ROOT/LOGICAL,source,KEY);model=initial;selected=[v for v in model['variables'] if v['name']=='data'];assert {v['slot'] for v in selected}=={11,13,17,21};intervals=disjoint(rebuilt,[v['slot'] for v in selected]);at=source.index('if (num5 >= 0)');insert=len(source[:at].encode('utf-16-le'))//2
   changed=syntax.change(LOGICAL,changed,model,'anchored-hoist',group_starts=[v['start'] for v in selected],insert_at=insert,name='saveDataBytes');model=c.predict(ROOT/LOGICAL,changed,KEY);stages.append({'stage':'coalesce-4-byte-arrays','count':len(model['predicted_types']),'complete':model['complete'],'types':model['predicted_types']})
   changed=switch_syntax.change(LOGICAL,changed,model,'switches-to-if');model=c.predict(ROOT/LOGICAL,changed,KEY);stages.append({'stage':'switch-if-terminal-break-return-throw','count':len(model['predicted_types']),'complete':model['complete'],'types':model['predicted_types']})
   assert 13 not in {l['slot'] for l in lifetimes(original)} and expected[13]=='kairo.unity.ui.Dialog'
   record=next(v for v in model['variables'] if v['name']=='recordStore');assert record['slot']==13
   # Calculated emission slot, with no claim about the unobservable original local name/scope.
   changed=_edit(changed,[(record['statement_start'],record['statement_start'],'global::kairo.unity.ui.Dialog unusedOriginalDialog13;\r\n')]);model=c.predict(ROOT/LOGICAL,changed,KEY);stages.append({'stage':'reserve-unused-dialog13-before-recordStore','count':len(model['predicted_types']),'complete':model['complete'],'types':model['predicted_types']});assert next(v['slot'] for v in model['variables'] if v['name']=='unusedOriginalDialog13')==13
   assert model['complete'] and model['predicted_types']==expected
   changed,guard=request(process,ROOT/LOGICAL,changed,'if (num2 != -3)','smcs-value-return-guard-reverse');model=c.predict(ROOT/LOGICAL,changed,KEY);stages.append({'stage':'guard-reverse-after-signature-repair','count':len(model['predicted_types']),'complete':model['complete'],'types':model['predicted_types']})
   assert model['complete'] and model['predicted_types']==expected
   (WIP/'candidate.cs').write_bytes(changed.encode('utf-8'));(WIP/'prediction.json').write_text(json.dumps(model,indent=2),encoding='utf-8')
   dependencies={str(p.relative_to(ROOT)):sha(p) for p in [ROOT/'tools/round7_coalesce_calculate.py',ROOT/'tools/round7_slot_solver.py',ROOT/'tools/round8_slot_anchor_calculate.py',ROOT/'tools/round8_coordinate_slot_syntax.cs.txt',ROOT/'tools/prepare_round7_transform.py',ROOT/'tools/compare_kairo.py']}
   # Retained dispatcher and rule sources are immutable, but pin them explicitly.
   LEGACY=('Round7TransformSyntax.cs','Round7NewRules.cs','Round7AliasRules.cs','Round7SmcsRules.cs','Round7RoslynRules.cs','round7_smcs_cycle_rules.cs','Round7RoslynCycleRules.cs','Round7SmcsExitRules.cs','Round7SmcsGuardRules.cs','Round7StringConcatRules.cs','Round7LoopConditionRules.cs','Round7SmcsSwitchRules.cs','Round7RoslynDeclarationRules.cs','Round7TerminalArmRules.cs','Round7BalancedBranchRules.cs','Round7InfiniteTailGuardRules.cs')
   dependencies.update({str((ROOT/'tools'/name).relative_to(ROOT)):sha(ROOT/'tools'/name) for name in LEGACY})
   special={v['name']:v['slot'] for v in model['variables'] if v['name'] in ['saveDataBytes','unusedOriginalDialog13','recordStore']};assert special=={'saveDataBytes':11,'unusedOriginalDialog13':13,'recordStore':14}
   patch=_patch(source,initial,changed,'calculated-shared-payload-switch-guard');patch.update(method_id=KEY,logical_path=LOGICAL,complete=True,predicted_types=model['predicted_types'],stages=stages,intervals=intervals,source_only=True,candidate_compilations=0,anchors=evidence['anchors'],special_variables=special,baseline_source_sha256=sha(BASE/LOGICAL),model_source_sha256=sha(ROOT/'tools/Round7SlotModel.cs'),client_sha256=sha(ROOT/'tools/round7_slot_model.py'),calculator_sha256=sha(Path(__file__)),syntax_source_sha256=sha(Path(__file__).with_name('round8_payload_switch_syntax.cs.txt')),dependency_sha256=dependencies)
   current=(ROOT/LOGICAL).read_bytes().decode('utf-8-sig');rebased=rebase_unique_patch(current,patch);fresh=_edit(current,[(rebased['start'],rebased['end'],rebased['after'])]);prediction=c.predict(ROOT/LOGICAL,fresh,KEY);assert prediction['complete'] and prediction['predicted_types']==expected;patch['current_unique_rebase_verified']=True
   fp=ROOT/'reports/round8-payload-slot-fixture.json';fp=fp if fp.exists() else WIP/'fixture-result.json'
   result={'operation':'source-only-shared-payload-and-unused-dialog-switch-guard-composition','family':'calculated-shared-payload-switch-guard','candidate_game_compilations':0,'frozen_baseline':str(BASE.relative_to(ROOT)),'patches':[patch],'methods':[{'method_id':KEY,'original_il_bytes':original['header']['code_size'],'model_complete':True,'signature_exact':True,'local_count':len(model['predicted_types']),'special_variables':special,'game_body_match':'UNKNOWN','current_unique_rebase_verified':True}],'potential_il_bytes':original['header']['code_size'],'fixture':read(fp),'calculator_sha256':sha(Path(__file__)),'syntax_source_sha256':patch['syntax_source_sha256'],'dependency_sha256':dependencies,'model_source_sha256':patch['model_source_sha256'],'client_sha256':patch['client_sha256'],'slot_anchor_evidence_sha256':sha(ep),'prefix_after_compilation':'UNKNOWN','baseline_descriptor_sha256':{name:sha(BASE/name) for name in ['original-methods.json','rebuilt-methods.json','plain-local-types.json']}}
   (WIP/'round8-payload-slot-patches.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items() if k not in {'patches','fixture','dependency_sha256','baseline_descriptor_sha256'}},indent=2))

 finally:syntax.close();switch_syntax.close();process.stdin.close();process.wait(timeout=10)
if __name__=='__main__':main()
