"""WIP ONLY: combined B flow hypothesis + required named locals. No game Emit."""
import hashlib,json,importlib,importlib.util,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file());sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from round9_part_b_fixtures import compile_smcs,find,body_difference
SCRIPT=Path(__file__).resolve().parent;WORK=ROOT/'build/round9/part-d-joint'
# Reuse the source-only semantic host; all cache/output files stay in this WIP.
spec=importlib.util.spec_from_file_location('joint_source_only_host_base',ROOT/'tools/round9_float_identity_fixtures.py')
ff=importlib.util.module_from_spec(spec);spec.loader.exec_module(ff)
ff.WORK=WORK;ff.RULE=SCRIPT/'Round9JointNamedSelectorRules.cs';ff.HOST=ff.HOST.replace('AddRound9FloatingIdentityRules','AddRound9JointNamedSelectorRules')
def main():
 base=ROOT/'build/round9/cycles/b10/after/Assembly-CSharp';manifest=json.loads((base/'build-manifest.json').read_text(encoding='utf-8-sig'))
 for source in manifest['sources']:source['path']=str(base/source['path']);source['input_path']=source['path']
 mp=WORK/'frozen-manifest.json';mp.write_text(json.dumps(manifest,indent=2)+'\n',encoding='utf-8')
 cfg=json.loads((ROOT/'config/round9-protocol.json').read_text())['models']['Assembly-CSharp']
 bpatches=json.loads((ROOT/'reports/round9-part-b-source-patches.json').read_text())['patches'];types=json.loads((base/'plain-local-types.json').read_text())
 proofs=[];patches=[]
 with importlib.import_module(cfg['module']).SlotModelClient(mp) as predictor:
  for identity,logical,brule in [('form.GameForm::_update(System.Int32)->System.Boolean','src/Assembly-CSharp/form/GameForm.cs','b9-event-loop-split-guard'),('form.SubForm::Init()->System.Boolean','src/Assembly-CSharp/form/SubForm.cs','b9-empty-arm-common-return')]:
   path=base/logical;source=path.read_bytes().decode('utf-8-sig');bp=next(p for p in bpatches if p['method_id']==identity);assert source.count(bp['before'])==1
   position=source.index(bp['before']);start=len(source[:position].encode('utf-16-le'))//2;end=start+len(bp['before'].encode('utf-16-le'))//2
   candidate=ff.apply(source,dict(bp,start=start,end=end))
   source_steps=[dict(rule=brule,before=bp['before'],after=bp['after'])]
   if 'GameForm' in identity:
    needle='if (udat_.GetMatchInfoJoin() == 0 && !apdat_.CheckAlreadyEvent(176))';assert candidate.count(needle)==1
    after='int testedMatchInfoJoin = udat_.GetMatchInfoJoin();\r\n\t\t\t\t\t\tif (testedMatchInfoJoin == 0 && !apdat_.CheckAlreadyEvent(176))'
    candidate=candidate.replace(needle,after);source_steps.append(dict(before=needle,after=after,rule='named-tested-Int32-local'))
   else:
    # Host must read the already flow-corrected text while retaining the logical source path.
    temp=WORK/'SubForm-candidate.cs';temp.write_bytes(candidate.encode('utf-8'));cm=json.loads(json.dumps(manifest))
    for row in cm['sources']:
     if Path(row['path'])==path:row['path']=str(temp);row['input_path']=str(temp)
    cmp=WORK/'sub-source-only-manifest.json';cmp.write_text(json.dumps(cm,indent=2)+'\n',encoding='utf-8')
    rows=ff.query(temp,cmp,dict(round7_cycle_rule='d9-joint-flow-local-stores',target_method='Init'));small=[p for r in rows for p in r['patches']];assert len(small)==2,len(small)
    for patch in sorted(small,key=lambda p:p['start'],reverse=True):candidate=ff.apply(candidate,patch)
    source_steps.extend(small)
   after_model=predictor.predict(path,candidate,identity);before_model=predictor.predict(path,source,identity)
   # Only extract the new method via the exact unchanged suffix delimiter.
   delta=len(candidate.encode('utf-16-le'))//2-len(source.encode('utf-16-le'))//2;after_end=end+delta
   after=candidate.encode('utf-16-le')[start*2:after_end*2].decode('utf-16-le')
   patch=dict(start=start,end=end,before=bp['before'],after=after,rule='d9-joint-flow-local-stores',method_id=identity,path=str(ROOT/logical),logical_path=logical,
    model_source_sha256=sha(ROOT/cfg['source']),model_client_sha256=sha(ROOT/cfg['client']),client_sha256=sha(ROOT/cfg['client']))
   patches.append(patch);proofs.append(dict(method_id=identity,source_sha256=sha(path),source_steps=source_steps,before_model=before_model,after_model=after_model,
    original_types=types[identity],after_types_equal=after_model['predicted_types']==types[identity],unique_raw_UTF16_rebase=source.count(bp['before'])==1,game_compilations=0,body_match='UNKNOWN'))
   print(identity,'complete',after_model['complete'],'types',len(after_model['predicted_types']),'orig',len(types[identity]),'equal',proofs[-1]['after_types_equal'])
 files=[cfg['source'],cfg['client'],'tools/Round9PartBRules.cs','tools/round9_part_b_fixtures.py','tools/round9_float_identity_fixtures.py','tools/round9_float_identity_hints.py']
 if SCRIPT==ROOT/'tools':files.extend(['tools/Round9JointNamedSelectorRules.cs','tools/round9_joint_flow_local_calculate.py','tools/round9_joint_flow_local_fixtures.py'])
 dependencies={p:sha(ROOT/p) for p in files}
 for patch in patches:patch.update(dependency_sha256=dependencies,syntax_source_sha256=sha(ff.RULE),calculator_sha256=sha(Path(__file__)))
 result=dict(operation='joint-B-flow-plus-local-allocation',baseline=str(base.relative_to(ROOT)),patches=patches,proofs=proofs,game_compilations=0,adopted=False,
  syntax_source_sha256=sha(ff.RULE),calculator_sha256=sha(Path(__file__)),dependency_sha256=dependencies,
  baseline_receipt_sha256=sha(base/'verification.json'),original_dump_sha256=sha(base/'original-methods.json'),
  reused_B_bundle_sha256=sha(ROOT/'reports/round9-part-b-source-patches.json'),original_types_sha256=sha(base/'plain-local-types.json'))
 (WORK/'patches-and-proof.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
if __name__=='__main__':main()
