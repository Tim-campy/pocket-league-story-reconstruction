"""Independent smcs proof of disjoint declarations versus one common local."""
import json
import os
from pathlib import Path
import subprocess
from compare_kairo import body_cells, differences, sha
from round5_local_cases import local_inspector
from round7_slot_model import SlotModelClient

ROOT=Path(__file__).resolve().parents[1]
SOURCE=r'''public static class CoalesceFixture {
 static void Sink(int x,int a,int b,int c){}
 public static void Original(bool first){
  int x,a,b,c; x=a=b=c=0;
  if(first){x=7;Sink(x,a,b,c);}else{x=9;Sink(x,a,b,c);}
 }
 public static void Split(bool first){
  int c;int b;int a=(b=(c=0));
  if(first){int dx=7;Sink(dx,a,b,c);}else{int dx=9;Sink(dx,a,b,c);}
 }
 public static void CoalescedDeclarationsOnly(bool first){
  int x;int c;int b;int a=(b=(c=0));
  if(first){x=7;Sink(x,a,b,c);}else{x=9;Sink(x,a,b,c);}
 }
 public static void CoalescedAndChain(bool first){
  int x;int a;int b;int c;x=a=b=c=0;
  if(first){x=7;Sink(x,a,b,c);}else{x=9;Sink(x,a,b,c);}
 }
}'''


def main():
    baseline=ROOT/'build/round7/cycles/cycle02/baseline/Assembly-CSharp'
    manifest=json.loads((baseline/'build-manifest.json').read_text(encoding='utf-8-sig'))
    folder=ROOT/'build/round7/slot-coalesce-fixture';folder.mkdir(parents=True,exist_ok=True)
    source=folder/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n')
    binary=folder/'Fixture.dll'
    args=[f for f in manifest['flags'] if not f.startswith('-out:') and f!='-noconfig']
    args+=['-out:'+str(binary)]+['-r:'+r['path'] for r in manifest['references']]+[str(source)]
    rsp=folder/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8')
    env=os.environ.copy();env.update(manifest['scoped_environment'])
    result=subprocess.run([manifest['runtime'],manifest['compiler'],'-noconfig','@'+str(rsp)],env=env,cwd=ROOT,capture_output=True)
    (folder/'compiler.log').write_bytes(result.stdout+result.stderr)
    if result.returncode:raise RuntimeError(result.stdout+result.stderr)
    raw=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all'],capture_output=True,check=True)
    actual={m['stable_id']:m for m in json.loads(raw.stdout.decode('utf-8-sig'))['selected_methods']}
    local=subprocess.run([str(local_inspector()),str(binary),'--locals-only'],capture_output=True,check=True)
    types={m['stable_id']:m['plain_local_types'] for m in json.loads(local.stdout.decode('utf-8-sig'))['methods']}
    mp=folder/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8')
    with SlotModelClient(mp) as client:models=client.predict_all(source,SOURCE)
    original=actual['CoalesceFixture::Original(System.Boolean)->System.Void']
    rows=[]
    for model in models:
        identity=model['method_id'];method=actual[identity]
        rows.append({'method_id':identity,'local_types':types[identity],'model_complete':model['complete'],
                     'model_exact':model['predicted_types']==types[identity],
                     'original_body_equal':body_cells(original)==body_cells(method) and original.get('locals')==method.get('locals') and original.get('exception_regions')==method.get('exception_regions'),
                     'first_instructions':method['instructions'][:14],
                     'original_difference_reasons':differences(original,method)[0]})
    report={'operation':'independent-smcs-coalesce-fixture','candidate_game_compilations':0,
            'fixture_assembly_compilations':1,'compiler_sha256':manifest['compiler_sha256'],
            'source_sha256':sha(source),'source':SOURCE,'methods':rows,
            'fixture_body_comparison':'Complete canonical body cells, locals and EH; fixture method names differ, so signature identity is reported separately. No game states are affected.'}
    (ROOT/'reports/round7-coalesce-fixture.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8',newline='\n')
    print(json.dumps({r['method_id']:{'locals':len(r['local_types']),'model_exact':r['model_exact'],'body_equal':r['original_body_equal']} for r in rows}))
    assert all(r['model_complete'] and r['model_exact'] for r in rows)
    assert next(r for r in rows if '::CoalescedAndChain(' in r['method_id'])['original_body_equal']
    assert not next(r for r in rows if '::CoalescedDeclarationsOnly(' in r['method_id'])['original_body_equal']


if __name__=='__main__':main()
