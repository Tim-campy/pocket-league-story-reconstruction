"""Compose draw6 declarations from measured slots; no game compilation."""
import inspect,json,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'))
import round7_coalesce_calculate as frozen
from round7_slot_model import SlotModelClient
from round7_slot_solver import lifetimes,_edit,_patch
from round7_transform import TransformClient
from compare_kairo import sha
from round7_coalesce_calculate import rebase_unique_patch
from round7_draw6_slots_diagnose import main as diagnose_snapshot
FOLDER=ROOT/'build/round7/draw6-slot-calculus';FOLDER.mkdir(parents=True,exist_ok=True)
KEY='form.SubForm::_draw6(kairo.unity.ui.Graphics)->System.Void';LOGICAL='src/Assembly-CSharp/form/SubForm.cs'


def read(p):return json.loads(Path(p).read_text(encoding='utf-8-sig'))


def main():
 diagnose_snapshot()
 # Derive an isolated bridge from the frozen utility without editing it.
 namespace=dict(vars(frozen));namespace['FOLDER']=FOLDER
 code=inspect.getsource(frozen.Syntax).replace("(ROOT/'tools/round7_coalesce_syntax.cs')","(ROOT/'tools/round7_draw6_slot_syntax.cs.txt')")
 exec(code,namespace);syntax=namespace['Syntax'](FOLDER/'manifest.json')
 data=read(ROOT/'reports/round7-draw6-slot-diagnosis.json');original=read(FOLDER/'original.json');rebuilt=read(FOLDER/'rebuilt.json')
 source=(FOLDER/'snapshot-SubForm.cs').read_bytes().decode('utf-8-sig');initial=data['model'];expected=data['original_types']
 group=next(g for g in data['many_to_one'] if g['original_slot']==61);widths=group['variables']
 ranges=sorted((l['first_use'],l['last_use']) for l in lifetimes(rebuilt) if l['slot'] in group['rebuilt_slots'])
 if any(right[0]<=left[1] for left,right in zip(ranges,ranges[1:])):raise ValueError('interleaved_width_variables')
 height=next(v for v in initial['variables'] if v['name']=='num28')
 if not all(height['lexical_scope_start']<=v['start']<height['lexical_scope_end'] for v in widths):raise ValueError('width_common_scope_unknown')
 if any(i['opcode']=='switch' for i in original['instructions']):raise ValueError('original_switch_exists')
 try:
  with SlotModelClient(FOLDER/'manifest.json') as model:
   candidate=syntax.change(LOGICAL,source,initial,'merge-width-chain',group_starts=[v['start'] for v in widths],height_start=height['start'])
   prediction=model.predict(ROOT/LOGICAL,candidate,KEY)
   stages=[{'stage':'merge-three-widths-and-zerochain','complete':prediction['complete'],'count':len(prediction['predicted_types']),'signature_equal':prediction['predicted_types']==expected}]
   candidate=syntax.change(LOGICAL,candidate,prediction,'switches-to-if',allow_call_selector='udat_.GetMatchInfoJoin()')
   prediction=model.predict(ROOT/LOGICAL,candidate,KEY)
   stages.append({'stage':'named-selector49-two-switches-to-if','complete':prediction['complete'],'count':len(prediction['predicted_types']),'signature_equal':prediction['predicted_types']==expected})
   with TransformClient(FOLDER/'manifest.json') as transform:
    point=candidate.index('mapchipMain = ((',candidate.index('void _draw6('))
    result=transform.request(ROOT/LOGICAL,source=candidate,offset=len(candidate[:point].encode('utf-16-le'))//2,compiler='smcs',max_candidates=2048)
   choices=[]
   ternary_base=candidate
   for patch in result['patches']:
    if not patch['rule'].startswith('assignment-ternary-if'):continue
    changed=_edit(ternary_base,[(patch['start'],patch['end'],patch['after'])]);proposed=model.predict(ROOT/LOGICAL,changed,KEY)
    choices.append({'rule':patch['rule'],'complete':proposed['complete'],'signature_equal':proposed['predicted_types']==expected,'after':patch['after']})
    if patch['rule']=='assignment-ternary-if-reverse':candidate=changed;prediction=proposed
   stages.append({'stage':'existing-assignment-ternary-if-reverse','complete':prediction['complete'],'count':len(prediction['predicted_types']),'signature_equal':prediction['predicted_types']==expected})
 finally:syntax.close()
 approved=prediction['complete'] and prediction['predicted_types']==expected
 special={v['name']:v['slot'] for v in prediction['variables'] if v['name'] in ['switchValue','coalescedWidth','num28','mapchipMain']}
 if approved:assert special['switchValue']==49 and special['coalescedWidth']==61 and special['num28']==62 and special['mapchipMain']==7
 patch=_patch(source,initial,candidate,'calculated-named-switch-selector-coalescence')
 patch.update(method_id=KEY,logical_path=LOGICAL,source_only=True,candidate_compilations=0,
              complete=prediction['complete'],predicted_types=prediction['predicted_types'],
              baseline_source_sha256=sha(FOLDER/'snapshot-SubForm.cs'),model_source_sha256=sha(ROOT/'tools/Round7SlotModel.cs'),
              client_sha256=sha(ROOT/'tools/round7_slot_model.py'),stages=stages,special_variables=special)
 instructions=original['instructions']
 selector_index=next(i for i,x in enumerate(instructions) if x['offset']==5362)
 chain_index=next(i for i,x in enumerate(instructions) if x['offset']==7573)
 output={'operation':'source-only-draw6-named-selector-width-calculus','candidate_game_compilations':0,
         'method_id':KEY,'original_il_bytes':original['header']['code_size'],'conclusion':'MODEL_APPROVED' if approved else 'INCONCLUSIVA',
         'stages':stages,'special_variables':special,'first_ternary_choices':choices,'width_lifetimes':ranges,
         'width_source_declarations':widths,'height_source_declaration':height,
         'original_key_lifetimes':[v for v in lifetimes(original) if v['slot'] in [7,49,61,62]],
         'original_selector_anchor':instructions[selector_index:selector_index+4],
         'original_zerochain_anchor':instructions[chain_index:chain_index+4],
         'original_first_ternary_anchor':[x for x in instructions if 287<=x['offset']<=350],
         'original_switch_opcodes':sum(x['opcode']=='switch' for x in instructions),
         'rebuilt_switch_opcodes':sum(x['opcode']=='switch' for x in rebuilt['instructions']),
         'calculator_sha256':sha(Path(__file__)),'syntax_source_sha256':sha(ROOT/'tools/round7_draw6_slot_syntax.cs.txt'),
         'dependency_sha256':{'tools/'+name:sha(ROOT/'tools'/name) for name in ['round7_coalesce_calculate.py','round7_slot_solver.py','round7_draw6_slots_diagnose.py']},
         'original_unused_slots':data['unused_original_slots'],'signature_equal':approved,
         'patches':[patch] if approved else [],'predicted_types':prediction['predicted_types'],'expected_types':expected}
 (FOLDER/'candidate-SubForm.cs').write_bytes(candidate.encode('utf-8'))
 (ROOT/'reports/round7-draw6-calculated-patches.json').write_text(json.dumps(output,indent=2)+'\n',encoding='utf-8')
 print(json.dumps({k:v for k,v in output.items() if k in ['conclusion','stages','special_variables','width_lifetimes','signature_equal']}))


if __name__=='__main__':main()
