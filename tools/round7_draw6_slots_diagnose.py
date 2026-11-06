"""Read-only draw6 local calculus and source-only existing-transform probes."""
import json,sys
from collections import Counter,defaultdict
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round7_slot_model import SlotModelClient
from round7_slot_solver import correspondences,lifetimes
from round7_transform import TransformClient
FOLDER=ROOT/'build/round7/draw6-slot-calculus';FOLDER.mkdir(parents=True,exist_ok=True)
KEY='form.SubForm::_draw6(kairo.unity.ui.Graphics)->System.Void';LOGICAL='src/Assembly-CSharp/form/SubForm.cs'


def read(p):return json.loads(Path(p).read_text(encoding='utf-8-sig'))


def main():
 base=ROOT/'build/round7/cycles/cycle11/baseline/Assembly-CSharp';manifest=read(base/'build-manifest.json')
 for item in manifest['sources']:item['input_path']=str(base/item['path'])
 mp=FOLDER/'manifest.json';mp.write_text(json.dumps(manifest),encoding='utf-8')
 source=(base/LOGICAL).read_bytes().decode('utf-8-sig');(FOLDER/'snapshot-SubForm.cs').write_bytes(source.encode('utf-8'))
 original=next(m for m in read(base/'original-methods.json')['selected_methods'] if m['stable_id']==KEY)
 rebuilt=next(m for m in read(base/'rebuilt-methods.json')['selected_methods'] if m['stable_id']==KEY)
 oldtypes=read(base/'plain-local-types.json')[KEY];newtypes=read(base/'plain-local-types-rebuilt.json')[KEY]
 with SlotModelClient(mp) as client:
  model=client.predict(ROOT/LOGICAL,source,KEY)
  mapping,votes=correspondences(original,rebuilt,oldtypes,newtypes)
  groups=defaultdict(list)
  for current,wanted in mapping.items():groups[wanted].append(current)
  merged=[{'original_slot':slot,'rebuilt_slots':slots,'variables':[v for v in model['variables'] if v['slot'] in slots]}
          for slot,slots in groups.items() if len(slots)>1]
  with TransformClient(mp) as syntax:
   point=source.index('mapchipMain = ((',source.index('void _draw6('))
   response=syntax.request(ROOT/LOGICAL,source=source,offset=len(source[:point].encode('utf-16-le'))//2,compiler='smcs',max_candidates=2048)
  candidates=[]
  for patch in response['patches']:
   if not patch['rule'].startswith('assignment-ternary-if'):continue
   raw=source.encode('utf-16-le');candidate=(raw[:2*patch['start']]+patch['after'].encode('utf-16-le')+raw[2*patch['end']:]).decode('utf-16-le')
   prediction=client.predict(ROOT/LOGICAL,candidate,KEY)
   candidates.append({'rule':patch['rule'],'before':patch['before'],'after':patch['after'],'complete':prediction['complete'],'predicted_types':prediction['predicted_types'],'signature_equal':prediction['predicted_types']==oldtypes})
 used={l['slot'] for l in lifetimes(original)}
 result={'operation':'read-only-draw6-local-calculus','candidate_game_compilations':0,'baseline':str(base.relative_to(ROOT)),
         'method_id':KEY,'source_sha256':sha(base/LOGICAL),'original_count':len(oldtypes),'rebuilt_count':len(newtypes),
         'original_types':oldtypes,'rebuilt_types':newtypes,'original_type_counts':dict(Counter(oldtypes)),'rebuilt_type_counts':dict(Counter(newtypes)),
         'model_complete':model['complete'],'model_predicts_rebuilt':model['predicted_types']==newtypes,
         'type_mismatches':[{'slot':i,'original':a,'rebuilt':b} for i,(a,b) in enumerate(zip(oldtypes,newtypes)) if a!=b],
         'unused_original_slots':[{'slot':i,'type':t} for i,t in enumerate(oldtypes) if i not in used],
         'slot_correspondence':votes,'many_to_one':merged,'original_lifetimes':lifetimes(original),'rebuilt_lifetimes':lifetimes(rebuilt),
         'existing_first_ternary_candidates':candidates,'model':model}
 (ROOT/'reports/round7-draw6-slot-diagnosis.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
 for name,data in [('original',original),('rebuilt',rebuilt)]: (FOLDER/(name+'.json')).write_text(json.dumps(data,indent=2)+'\n',encoding='utf-8')
 print(json.dumps({k:v for k,v in result.items() if k in ['original_count','rebuilt_count','model_complete','model_predicts_rebuilt','original_type_counts','rebuilt_type_counts','type_mismatches','unused_original_slots']}))
 print('manytoone',[(g['original_slot'],g['rebuilt_slots'],[(v['name'],v['slot']) for v in g['variables']]) for g in merged])
 print('firstternary',[(c['rule'],c['complete'],len(c['predicted_types']),c['signature_equal']) for c in candidates])


if __name__=='__main__':main()
