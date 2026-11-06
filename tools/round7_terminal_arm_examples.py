"""Read-only source probes for the opt-in terminal-action rule."""
import json,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'));sys.path.insert(0,str(Path(__file__).resolve().parent))
from test_round7_terminal_arm_rules import Client,RULE
from round7_slot_model import SlotModelClient
from compare_kairo import sha
FOLDER=ROOT/'build/round7/terminal-arm-experiment';FOLDER.mkdir(parents=True,exist_ok=True)


def read(p):return json.loads(Path(p).read_text(encoding='utf-8-sig'))


def main():
 base=ROOT/'build/round7/cycles/cycle09/baseline/Assembly-CSharp'
 manifest=read(base/'build-manifest.json')
 for item in manifest['sources']:item['input_path']=str(ROOT/item['path'])
 mp=FOLDER/'game-source-only-manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
 measured=read(ROOT/'reports/round7-cycle08-after-measurement.json')['assemblies']['Assembly-CSharp']
 pairs={m['stable_id']:m for m in measured['alignment']['methods']}
 original=read(base/'plain-local-types.json')
 original_methods={m['stable_id']:m for m in read(base/'original-methods.json')['selected_methods']}
 cases=[('form.SoccerForm::Draw_newPlayerLv(kairo.unity.ui.Graphics)->System.Void','src/Assembly-CSharp/form/SoccerForm.cs','if (stateCnt_ < PBONUS_NP_LV_TIME[3])',None),
        ('game.DelayEvent::Serialize(java.io.OutputStream)->System.Void','src/Assembly-CSharp/game/DelayEvent.cs','if (argi_ == null)',None),
        ('form.SubForm::_draw5(kairo.unity.ui.Graphics)->System.Void','src/Assembly-CSharp/form/SubForm.cs','if (udArrowCnt == 0)','num = 70;')]
 syntax=Client(mp);rows=[]
 try:
  with SlotModelClient(mp) as model:
   for key,logical,needle,previous in cases:
    path=ROOT/logical
    source=path.read_bytes().decode('utf-8-sig')
    name=key.split('::')[1].split('(')[0];method_start=source.index('void '+name+'(')
    point=source.index(needle,method_start)
    if previous:
     while previous not in source[max(method_start,point-200):point]:point=source.index(needle,point+len(needle))
    patches=syntax.request(ROOT/logical,source,point)
    checks=[]
    for patch in patches:
     raw=source.encode('utf-16-le');candidate=(raw[:2*patch['start']]+patch['after'].encode('utf-16-le')+raw[2*patch['end']:]).decode('utf-16-le')
     prediction=model.predict(ROOT/logical,candidate,key)
     checks.append({'rule':patch['rule'],'complete':prediction['complete'],'predicted_count':len(prediction['predicted_types']),
                    'original_signature_equal':prediction['predicted_types']==original[key],
                    'unsupported_reasons':prediction.get('unsupported_reasons',[]),'before':patch['before'],'after':patch['after']})
    pair=pairs.get(key,{})
    branch_offset=9734 if '::_draw5(' in key else pair['original_offset']
    instructions={i['offset']:i for i in original_methods[key]['instructions']}
    chain=[];target=branch_offset
    while target in instructions and len(chain)<8:
     instruction=instructions[target];chain.append(instruction)
     if instruction['opcode'] not in ('br','br.s'):break
     target=instruction.get('operand',{}).get('branch')
    ancestry=syntax.last_response.get('mapped',{}).get('terminal_arm_ancestry',[])
    assert ancestry[-1].endswith(':method-body') and all(node.split('@')[0] in {'Block','IfStatement','ElseClause'} for node in ancestry)
    row={'method_id':key,'source':logical,'source_snapshot':str(path.relative_to(ROOT)),'source_sha256':sha(path),
         'source_line':source[:point].count('\n')+1,'hypothesis_count':len(checks),'patch_checks':checks,
         'terminal_parent_path':ancestry,'terminal_path_contains_no_loop_or_EH':True,
         'original_branch_chain':chain,
         'original_offset_cycle08':pair.get('original_offset','NOT_IN_CYCLE08_FIRST_PAIR'),
         'original_pair_cycle08':pair.get('pair','NOT_IN_CYCLE08_FIRST_PAIR')}
    rows.append(row);print(json.dumps({k:v for k,v in row.items() if k!='patch_checks'}));print(json.dumps([{k:v for k,v in c.items() if k not in {'before','after'}} for c in checks]))
 finally:syntax.close()
 result={'operation':'source-only-terminal-actions-real-examples','candidate_game_compilations':0,'rule':RULE,
         'rule_source_sha256':sha(ROOT/'tools/Round7TerminalArmRules.cs'),'cycle08_br_to_ret_methods':14,'cycle08_br_to_ret_il_bytes':37050,'cycle09_br_to_ret_methods':15,'cycle09_br_to_ret_il_bytes':49112,'source_examples':'Current Main snapshots; historical IL offsets are explicitly labeled','methods':rows}
 (ROOT/'reports/round7-terminal-arm-examples.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')


if __name__=='__main__':main()
