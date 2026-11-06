"""Actual smcs fixture for switch lowering and unused for-declarator slots."""
import json,os,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'));sys.path.insert(0,str(Path(__file__).resolve().parent))
from round7_coalesce_calculate import Syntax,read
from round7_slot_model import SlotModelClient
from compare_kairo import body_cells,sha
FOLDER=ROOT/'build/round7/coalesce-experiment/extra';FOLDER.mkdir(parents=True,exist_ok=True)
SOURCE=r'''public static class ExtraFixture {
 static void Sink(int x){}
 public static void OriginalIf(int value){int k=value;if(k==0){Sink(1);}else if(k==1){for(int i=0;i<3;i++){if(i==2)break;Sink(i);}}else{Sink(3);}}
 public static void RecoveredSwitch(int value){int k=value;switch(k){case 0:Sink(1);break;case 1:{for(int i=0;i<3;i++){if(i==2)break;Sink(i);}break;}default:Sink(3);break;}}
 public static void OriginalUnused(){int unused,i;for(i=0;i<3;i++)Sink(i);}
 public static void RecoveredUnused(){for(int i=0;i<3;i++)Sink(i);}
}'''


def main():
 manifest=read(ROOT/'build/round7/cycles/cycle02/baseline/Assembly-CSharp/build-manifest.json')
 source=FOLDER/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n')
 mp=FOLDER/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8')
 syntax=Syntax(mp)
 try:
  with SlotModelClient(mp) as client:
   model=client.predict(source,SOURCE,'ExtraFixture::RecoveredSwitch(System.Int32)->System.Void')
   changed=syntax.change(source,SOURCE,model,'switches-to-if')
   model=client.predict(source,changed,'ExtraFixture::RecoveredUnused()->System.Void')
   variable=next(v for v in model['variables'] if v['name']=='i')
   changed=syntax.change(source,changed,model,'reserve-variable-before',variable_start=variable['start'],original_slot=0,type_fullname='System.Int32')
   model=client.predict(source,changed,'ExtraFixture::RecoveredUnused()->System.Void')
   reserved=next(v for v in model['variables'] if v['name']=='unusedOriginalSlot0')
   assert model['complete'] and reserved['slot']==0
 finally:syntax.close()
 source.write_text(changed,encoding='utf-8',newline='\n')
 binary=FOLDER/'Fixture.dll';args=[f for f in manifest['flags'] if not f.startswith('-out:') and f!='-noconfig']
 args+=['-out:'+str(binary)]+['-r:'+r['path'] for r in manifest['references']]+[str(source)]
 rsp=FOLDER/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8')
 env=os.environ.copy();env.update(manifest['scoped_environment'])
 p=subprocess.run([manifest['runtime'],manifest['compiler'],'-noconfig','@'+str(rsp)],cwd=ROOT,env=env,capture_output=True)
 (FOLDER/'compiler.log').write_bytes(p.stdout+p.stderr)
 if p.returncode:raise RuntimeError(p.stdout+p.stderr)
 raw=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all'],capture_output=True,check=True)
 methods={m['stable_id']:m for m in json.loads(raw.stdout.decode('utf-8-sig'))['selected_methods']}
 def same(first,last):
  a,b=methods[first],methods[last]
  return body_cells(a)==body_cells(b) and a['locals']==b['locals'] and a['exception_regions']==b['exception_regions']
 switch=same('ExtraFixture::OriginalIf(System.Int32)->System.Void','ExtraFixture::RecoveredSwitch(System.Int32)->System.Void')
 unused=same('ExtraFixture::OriginalUnused()->System.Void','ExtraFixture::RecoveredUnused()->System.Void')
 result={'operation':'independent-smcs-composed-coalescence-auxiliary-fixtures','candidate_game_compilations':0,'fixture_assembly_compilations':1,'switch_to_if_body_exact':switch,'unused_for_declarator_body_exact':unused,'unused_slot_model_position':reserved['slot'],'source_sha256':sha(source),'compiler_sha256':manifest['compiler_sha256']}
 (ROOT/'reports/round7-coalesce-extra-fixture.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
 print(json.dumps(result));assert switch and unused


if __name__=='__main__':main()
