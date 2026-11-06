"""Frozen B10 source-only patches, each justified by original same-slot arm stores."""
import importlib,json,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round6_alignment import align_methods
try:
 from round9_per_arm_store_fixtures import query,apply,RULE
 from round9_per_arm_store_hints import per_arm_store_evidence
except ImportError:
 from fixtures import query,apply,RULE
 from hints import per_arm_store_evidence
WORK=ROOT/'build/round9/part-d-stores'
TARGETS=[
 ('kairo.unity.util.Compressor::Expand(System.Byte[])->System.Byte[]','src/KairoLibrary/kairo/unity/util/Compressor.cs','Expand','node2 = ',0,80,'node2'),
 ('kairo.unity.ui.TextLayout::_makeTextBlocks()->System.Void','src/KairoLibrary/kairo/unity/ui/TextLayout.cs','_makeTextBlocks','num3 = ((width_ != -1f) ? (width_ - num5) : (num3 - num5));',0,2063,'num3'),
 ('kairo.unity.ui.TextLayout::_makeTextBlocks()->System.Void','src/KairoLibrary/kairo/unity/ui/TextLayout.cs','_makeTextBlocks','num3 = ((width_ != -1f) ? (num3 + (width_ - num3) / 2f - num5 / 2f) : (num3 - num5 / 2f));',0,2545,'num3'),
 ('kairo.unity.ui.TextLayout::_makeTextBlocks()->System.Void','src/KairoLibrary/kairo/unity/ui/TextLayout.cs','_makeTextBlocks','num3 = ((width_ != -1f) ? (width_ - num5) : (num3 - num5));',1,2601,'num3')]

def occurrence(text,value,n,start):
 position=start
 for _ in range(n+1):position=text.index(value,position)+len(value)
 return position-len(value)

def main():
 base=ROOT/'build/round9/cycles/b10/after/KairoLibrary';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for source in manifest['sources']:source['path']=str(base/source['path']);source['input_path']=source['path']
 mp=WORK/'frozen-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
 cfg=json.loads((ROOT/'config/round9-protocol.json').read_text())['models']['KairoLibrary']
 original={m['stable_id']:m for m in json.loads((base/'original-methods.json').read_text())['selected_methods']};rebuilt={m['stable_id']:m for m in json.loads((base/'rebuilt-methods.json').read_text())['selected_methods']}
 types=json.loads((base/'plain-local-types.json').read_text());proofs=[];patches=[]
 with importlib.import_module(cfg['module']).SlotModelClient(mp) as predictor:
  for identity,logical,name,needle,n,offset,local in TARGETS:
   path=base/logical;source=path.read_bytes().decode('utf-8-sig');hints=per_arm_store_evidence(original[identity],offset);assert hints,(identity,offset)
   # Source orientation is separately read from the unchanged predicate and the arm operands.
   # Original first arm: node.left_ / cursor-extent / cursor-extent/2;
   # original second arm: node.right_ / width-extent / centered cursor expression.
   # This is not inferred from branch polarity alone.
   hints['round9_per_arm_store_order']='false-first'
   start_of_method=source.index(' '+name+'(');position=occurrence(source,needle,n,start_of_method)
   if name=='Expand':position=source.index('node2 = (',start_of_method)
   point=len(source[:position].encode('utf-16-le'))//2
   found=[(r,p) for r in query(path,mp,dict(hints,round7_cycle_rule='d9-calculated-per-arm-local-stores',target_method=name,point=point)) for p in r['patches']]
   assert len(found)==1,(identity,offset,found)
   row,small=found[0];assert small['before'].startswith(local+' = ')
   candidate=apply(source,small);before_model=predictor.predict(path,source,identity);after_model=predictor.predict(path,candidate,identity)
   original_slot=hints['round9_per_arm_store_slot'];variables=[v for v in before_model['variables'] if v['name']==local]
   assert len(variables)==1 and variables[0]['slot']==original_slot,(identity,variables,original_slot)
   assert variables[0]['type']==types[identity][original_slot]
   start,end=row['declaration_start'],row['declaration_end'];after_end=end+len(small['after'].encode('utf-16-le'))//2-(small['end']-small['start'])
   before=source.encode('utf-16-le')[start*2:end*2].decode('utf-16-le');after=candidate.encode('utf-16-le')[start*2:after_end*2].decode('utf-16-le');assert source.count(before)==1
   patches.append(dict(start=start,end=end,before=before,after=after,rule='d9-calculated-per-arm-local-stores',distance=0,method_id=identity,path=str(ROOT/logical),logical_path=logical,
    model_source_sha256=sha(ROOT/cfg['source']),model_client_sha256=sha(ROOT/cfg['client']),client_sha256=sha(ROOT/cfg['client'])))
   proofs.append(dict(method_id=identity,logical_path=logical,source_sha256=sha(path),single_source_edit=small,original_arm_store_evidence=hints['evidence'],
    source_orientation='false-first verified independently by unchanged predicate and field/arithmetic operands',source_local_slot_and_type_proof=variables[0],
    before_model=before_model,after_model=after_model,original_types=types[identity],after_types_equal=after_model['predicted_types']==types[identity],
    complete_wrong_signature=after_model['complete'] and after_model['predicted_types']!=types[identity],original_first_store=offset,
    before_CRLF=before.count('\r\n'),before_LF_only=before.count('\n')-before.count('\r\n'),unique_raw_UTF16_rebase=True,
    original_alignment=align_methods(original[identity],rebuilt[identity]),full_IL_match='UNKNOWN',game_compilations=0))
   print(name,offset,'NoEmit',after_model['complete'],len(after_model['predicted_types']),'equal',proofs[-1]['after_types_equal'])
 files=[cfg['source'],cfg['client'],'tools/compiler_probes.py']
 if Path(__file__).parent==ROOT/'tools':files.extend(['tools/Round9PerArmStoreRules.cs','tools/round9_per_arm_store_hints.py','tools/round9_per_arm_store_fixtures.py','tools/round9_per_arm_store_calculate.py','tools/round9_float_identity_fixtures.py'])
 result=dict(operation='round9-per-arm-local-store-source-only-calculation',baseline=str(base.relative_to(ROOT)),original_dump_sha256=sha(base/'original-methods.json'),
  helper_sha256=sha(RULE),hints_sha256=sha(Path(__file__).parent/('round9_per_arm_store_hints.py' if Path(__file__).parent==ROOT/'tools' else 'hints.py')),calculator_sha256=sha(Path(__file__)),
  dependency_sha256={p:sha(ROOT/p) for p in files},syntax_source_sha256=sha(RULE),patches=patches,proofs=proofs,adopted=False,game_compilations=0)
 for patch in patches:patch.update(dependency_sha256=result['dependency_sha256'],syntax_source_sha256=sha(RULE),calculator_sha256=sha(Path(__file__)))
 (WORK/'patches-and-proof.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')

if __name__=='__main__':main()
