"""Standalone toy-only checks for semantic coalescence and rejections."""
import json,sys,subprocess,os
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'))
sys.path.insert(0,str(Path(__file__).resolve().parent))
from round7_coalesce_calculate import Syntax,read
from round7_slot_model import SlotModelClient
from round7_slot_solver import lifetimes
from compare_kairo import body_cells,sha
FOLDER=ROOT/'build/round7/coalesce-experiment';FOLDER.mkdir(parents=True,exist_ok=True)


def main():
 base=ROOT/'build/round7/slot-coalesce-fixture';manifest=read(base/'manifest.json')
 logical=Path(manifest['sources'][0]['path']);source=logical.read_bytes().decode('utf-8-sig')
 syntax=Syntax(base/'manifest.json')
 try:
  with SlotModelClient(base/'manifest.json') as client:
   model=client.predict(logical,source,'CoalesceFixture::Split(System.Boolean)->System.Void')
   group=[v for v in model['variables'] if v['name']=='dx']
   candidate=syntax.change(logical,source,model,'merge-root-chain',group_starts=[v['start'] for v in group],root_names_by_slot=['$coalesced','a','b','c'])
   prediction=client.predict(logical,candidate,model['method_id'])
   assert prediction['complete'] and prediction['predicted_types']==['System.Int32']*4
   captured=source.replace('Sink(dx,a,b,c);','System.Threading.ThreadStart f = delegate { Sink(dx,a,b,c); }; f();')
   captured_model=client.predict(logical,captured,model['method_id'])
   # Spans are found from source syntax spelling; captured declarations no longer
   # have ordinary slots and must not be borrowed from the allocator's output.
   starts=[];at=0
   while True:
    at=captured.find('dx=',at)
    if at<0:break
    starts.append(len(captured[:at].encode('utf-16-le'))//2);at+=3
   try:syntax.change(logical,captured,captured_model,'merge-root-chain',group_starts=starts,root_names_by_slot=['$coalesced','a','b','c'])
   except RuntimeError as e:
    assert 'captured_group_rejected' in str(e);capture_rejected=True
   else:raise AssertionError('captured group authorized')
 finally:syntax.close()
 output_source=FOLDER/'CoalescedFixture.cs';output_source.write_bytes(candidate.encode('utf-8'))
 binary=FOLDER/'Fixture.dll'
 args=[f for f in manifest['flags'] if not f.startswith('-out:') and f!='-noconfig']
 args+=['-out:'+str(binary)]+['-r:'+r['path'] for r in manifest['references']]+[str(output_source)]
 rsp=FOLDER/'fixture.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8')
 env=os.environ.copy();env.update(manifest['scoped_environment'])
 p=subprocess.run([manifest['runtime'],manifest['compiler'],'-noconfig','@'+str(rsp)],env=env,cwd=ROOT,capture_output=True)
 (FOLDER/'fixture-compiler.log').write_bytes(p.stdout+p.stderr)
 if p.returncode:raise RuntimeError(p.stdout+p.stderr)
 def dump(path):
  raw=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(path),'--all'],capture_output=True,check=True)
  return {m['stable_id']:m for m in json.loads(raw.stdout.decode('utf-8-sig'))['selected_methods']}
 original=dump(base/'Fixture.dll');rebuilt=dump(binary)
 o=original['CoalesceFixture::Original(System.Boolean)->System.Void'];r=rebuilt['CoalesceFixture::Split(System.Boolean)->System.Void']
 body_equal=body_cells(o)==body_cells(r) and o['locals']==r['locals'] and o['exception_regions']==r['exception_regions']
 assert body_equal
 spans=lifetimes(o)
 first=next(v for v in spans if v['slot']==1);second=next(v for v in spans if v['slot']==2)
 interleaving_rejected=not(first['last_use']<second['first_use'] or second['last_use']<first['first_use'])
 assert interleaving_rejected
 result={'operation':'independent-smcs-semantic-coalescence-fixture','candidate_game_compilations':0,'fixture_assembly_compilations':1,
         'semantic_LCA_transformation_complete_body_exact':body_equal,'capture_rejected_source_only':capture_rejected,
         'interleaving_rejected_using_real_fixture_IL':interleaving_rejected,'interleaved_lifetimes':[first,second],
         'model_complete':prediction['complete'],'model_types':prediction['predicted_types'],'source_sha256':sha(output_source),
         'compiler_sha256':manifest['compiler_sha256'],'fixture_identity_note':'Fixture method names differ; comparison uses complete canonical body cells, locals and EH only. No game state is promoted.'}
 (ROOT/'reports/round7-coalesce-semantic-fixture.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
 print(json.dumps(result))


if __name__=='__main__':main()
