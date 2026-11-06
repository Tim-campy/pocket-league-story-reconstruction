"""Source-only repair of local identities proved by unique store contexts."""
import json,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from apply_il_rules import receipt_is_fresh
from round7_slot_model import SlotModelClient
from round7_slot_solver import local_access,lifetimes,_edit,_patch
from round7_coalesce_calculate import rebase_unique_patch
FOLDER=ROOT/'build/round7/store-anchor-experiment'
KEY='form.SubForm::_draw3(kairo.unity.ui.Graphics)->System.Void'
LOGICAL='src/Assembly-CSharp/form/SubForm.cs'


def read(p):return json.loads(Path(p).read_text(encoding='utf-8-sig'))


def token(instruction):return json.dumps({k:instruction.get(k) for k in ('opcode','operand')},sort_keys=True,separators=(',',':'))


def unique_store_anchor(original,rebuilt,offset,min_size=1):
 old,new=original['instructions'],rebuilt['instructions']
 index=next(i for i,x in enumerate(new) if x['offset']==offset)
 if not local_access(new[index]) or local_access(new[index])[0]!='stloc':raise ValueError('anchor_not_store')
 for size in range(min_size,min(12,index)+1):
  window=new[index-size:index]
  if any(isinstance(x.get('operand'),dict) and any(k in x['operand'] for k in ('branch','switch')) for x in window):break
  wanted=[token(x) for x in window]
  def matches(items):
   return [i for i,x in enumerate(items) if i>=size and local_access(x) and local_access(x)[0]=='stloc' and [token(a) for a in items[i-size:i]]==wanted]
  left,right=matches(old),matches(new)
  if len(left)==len(right)==1:
   return {'original_slot':local_access(old[left[0]])[1],'rebuilt_slot':local_access(new[index])[1],
           'original_store_offset':old[left[0]]['offset'],'rebuilt_store_offset':offset,
           'context':window,'context_size':size,'original_occurrences':1,'rebuilt_occurrences':1}
 raise ValueError('store_context_not_unique')


def prepare_snapshot():
 FOLDER.mkdir(parents=True,exist_ok=True)
 base=ROOT/'build/round7/cycles/cycle05/baseline/Assembly-CSharp'
 accepted=ROOT/'build/round7/runs/cycle05/engine/Assembly-CSharp'
 receipt_path=ROOT/'build/verification-Assembly-CSharp.json';receipt=read(receipt_path)
 if not receipt_is_fresh(ROOT,receipt):raise ValueError('Current official source proof is stale')
 artifact=ROOT/receipt['build_artifact'];path=ROOT/LOGICAL
 manifest=read(artifact.parent/'build-manifest.json')
 for item in manifest['sources']:item['input_path']=str(ROOT/item['path'])
 mp=FOLDER/'manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
 source=path.read_bytes().decode('utf-8-sig');(FOLDER/'accepted-SubForm.cs').write_bytes(source.encode('utf-8'))
 original_path=base/'original-methods.json';rebuilt_path=artifact.parent/'rebuilt-methods.json'
 original=next(m for m in read(original_path)['selected_methods'] if m['stable_id']==KEY)
 rebuilt=next(m for m in read(rebuilt_path)['selected_methods'] if m['stable_id']==KEY)
 checkpoint_path=accepted/'adoption-rechecked-final-methods.json'
 checkpoint=next(m for m in read(checkpoint_path)['selected_methods'] if m['stable_id']==KEY)
 if differences(checkpoint,rebuilt)[0]:raise ValueError('Current method body differs from analyzed cycle05 checkpoint')
 with SlotModelClient(mp) as client:model=client.predict(ROOT/LOGICAL,source,KEY)
 if not model['complete'] or len(model['predicted_types'])!=119:raise ValueError('Actual Main model is not the accepted119-local source')
 for name,value in [('original',original),('accepted',rebuilt),('model',model)]:
  (FOLDER/(name+'.json')).write_text(json.dumps(value,indent=2)+'\n',encoding='utf-8')
 (FOLDER/'evidence.json').write_text(json.dumps({'original_types':read(base/'plain-local-types.json')[KEY]},indent=2)+'\n',encoding='utf-8')
 return {'actual_main_source_sha256':sha(path),'current_official_receipt_sha256':sha(receipt_path),
         'current_official_receipt_fresh':True,'current_method_body_equals_cycle05_checkpoint':True,
         'current_descriptor_sha256':sha(rebuilt_path),'cycle05_checkpoint_descriptor_sha256':sha(checkpoint_path),
         'original_descriptor_sha256':sha(original_path)}


def main():
 provenance=prepare_snapshot()
 original,rebuilt,model=read(FOLDER/'original.json'),read(FOLDER/'accepted.json'),read(FOLDER/'model.json')
 source=(FOLDER/'accepted-SubForm.cs').read_bytes().decode('utf-8-sig')
 # Four distinct evaluations establish the permutation independently of the
 # global slot votes: RateConvert(...23,75...), width/2-120, height-240, and 14.
 anchors=[unique_store_anchor(original,rebuilt,offset,min_size=size) for offset,size in [(3518,7),(3932,5),(3942,3),(3946,1)]]
 assert [(a['rebuilt_slot'],a['original_slot']) for a in anchors]==[(36,32),(37,36),(38,37),(32,38)]
 variables={v['name']:v for v in model['variables'] if v['name'] in {'r','num22','num20','num21'}}
 assert variables['r']['slot']==36 and variables['num22']['slot']==32
 def at(start,end):return source.encode('utf-16-le')[2*start:2*end].decode('utf-16-le')
 r,num22=variables['r'],variables['num22']
 assert at(num22['statement_start'],num22['statement_end']).strip()=='int num22;'
 assert r['initializer']=='GameUtil.RateConvert(num16, 0, 140, 23, 75, true)'
 method_start,method_end=model['declaration_start'],model['declaration_end']
 method=at(method_start,method_end)
 def unique(needle):
  if method.count(needle)!=1:raise ValueError('source_occurrence_not_unique:'+needle)
  before=method[:method.index(needle)]
  start=method_start+len(before.encode('utf-16-le'))//2
  return start,start+len(needle.encode('utf-16-le'))//2
 zero_start,zero_end=unique('num22 = num17 = num18 = 0;')
 late_start,late_end=unique('num22 = 14;')
 edits=[(num22['statement_start'],num22['statement_end'],'int r;'),
        (zero_start,zero_end,'r = num17 = num18 = 0;'),
        (r['statement_start'],r['statement_end'],'r = '+r['initializer']+';'),
        (late_start,late_end,'int num22 = 14;')]
 candidate=_edit(source,edits)
 with SlotModelClient(FOLDER/'manifest.json') as client:prediction=client.predict(ROOT/LOGICAL,candidate,KEY)
 expected=read(FOLDER/'evidence.json')['original_types']
 special={v['name']:v['slot'] for v in prediction['variables'] if v['name'] in variables}
 assert prediction['complete'] and prediction['predicted_types']==expected
 assert special=={'r':32,'num20':36,'num21':37,'num22':38},special
 (FOLDER/'candidate-SubForm.cs').write_bytes(candidate.encode('utf-8'))
 patch=_patch(source,model,candidate,'calculated-store-anchored-declarations')
 patch.update(method_id=KEY,logical_path=LOGICAL,source_only=True,candidate_compilations=0,
              complete=True,predicted_types=prediction['predicted_types'],
              baseline_source_sha256=sha(FOLDER/'accepted-SubForm.cs'),model_source_sha256=sha(ROOT/'tools/Round7SlotModel.cs'),
              client_sha256=sha(ROOT/'tools/round7_slot_model.py'),anchors=anchors,special_variables=special)
 result={'operation':'source-only-store-anchored-local-identity-repair','candidate_game_compilations':0,
         'accepted_checkpoint':'build/round7/runs/cycle05/engine/Assembly-CSharp/adoption-rechecked-final-methods.json',
         'method_id':KEY,'original_il_bytes':original['header']['code_size'],'patches':[patch],
         'original_slot_lifetimes':[r for r in lifetimes(original) if r['slot'] in {32,36,37,38}],
         'rebuilt_slot_lifetimes':[r for r in lifetimes(rebuilt) if r['slot'] in {32,36,37,38}],
         'model_complete':prediction['complete'],'signature_equal':prediction['predicted_types']==expected,
         'special_variables':special,'prefix_after_compilation':'UNKNOWN: no game candidate compiled','provenance':provenance,'calculator_sha256':sha(Path(__file__))}
 (ROOT/'reports/round7-store-anchor-patches.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
 print(json.dumps({k:v for k,v in result.items() if k not in {'patches','original_slot_lifetimes','rebuilt_slot_lifetimes'}}))
 print(json.dumps(anchors))


if __name__=='__main__':main()
