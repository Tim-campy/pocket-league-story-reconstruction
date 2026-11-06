"""Independent smcs fixture for hoisting declarations while keeping evaluations."""
import os,json,subprocess,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/round7_slot_model.py').exists())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import body_cells,sha
FOLDER=ROOT/'build/round7/store-anchor-experiment/fixture';FOLDER.mkdir(parents=True,exist_ok=True)
ORIGINAL='''int num16=0;int r;int num17;int num18;r=num17=num18=0;int num19=0;
while(num16<2){r=RateConvert(num16,0,140,23,75,true);num17=RateConvert(num16,0,140,32,123,true);num18=RateConvert(num16,0,140,58,168,true);Sink(r,num17,num18);num16++;if(num19>3)break;}
int num20=width/2-120;int num21=height-240;int num22=14;Sink(num20,num21,num22);'''
RECOVERED=ORIGINAL.replace('int r;','int num22;').replace('r=num17=num18=0;','num22=num17=num18=0;').replace('while(num16<2){r=RateConvert','while(num16<2){int r=RateConvert').replace('int num22=14;','num22=14;')
CORRECTED=RECOVERED.replace('int num22;','int r;').replace('num22=num17=num18=0;','r=num17=num18=0;').replace('int r=RateConvert','r=RateConvert').replace('num22=14;','int num22=14;')
SOURCE='''public static class StoreAnchorFixture {
static int RateConvert(int a,int b,int c,int d,int e,bool f){return a+b+c+d+e;}
static void Sink(int a,int b,int c){}
'''+''.join('public static void '+name+'(int width,int height){'+body+'}\n' for name,body in [('Original',ORIGINAL),('Recovered',RECOVERED),('Corrected',CORRECTED)])+'}'


def main():
 manifest=json.loads((ROOT/'build/round7/cycles/cycle05/baseline/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
 source=FOLDER/'Fixture.cs';source.write_text(SOURCE,encoding='utf-8',newline='\n')
 binary=FOLDER/'Fixture.dll';args=[f for f in manifest['flags'] if not f.startswith('-out:') and f!='-noconfig']
 args+=['-out:'+str(binary)]+['-r:'+r['path'] for r in manifest['references']]+[str(source)]
 rsp=FOLDER/'compile.rsp';rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8')
 env=os.environ.copy();env.update(manifest['scoped_environment'])
 p=subprocess.run([manifest['runtime'],manifest['compiler'],'-noconfig','@'+str(rsp)],cwd=ROOT,env=env,capture_output=True)
 (FOLDER/'compiler.log').write_bytes(p.stdout+p.stderr)
 if p.returncode:raise RuntimeError(p.stdout+p.stderr)
 raw=subprocess.run([str(ROOT/'build/tools/ProbeInspector.exe'),str(binary),'--all'],capture_output=True,check=True)
 methods={m['stable_id']:m for m in json.loads(raw.stdout.decode('utf-8-sig'))['selected_methods']}
 original=methods['StoreAnchorFixture::Original(System.Int32,System.Int32)->System.Void']
 def equal(name):
  other=methods['StoreAnchorFixture::'+name+'(System.Int32,System.Int32)->System.Void']
  return body_cells(original)==body_cells(other) and original['locals']==other['locals'] and original['exception_regions']==other['exception_regions']
 result={'operation':'independent-smcs-store-anchored-declaration-fixture','candidate_game_compilations':0,'fixture_assembly_compilations':1,
         'recovered_body_equal':equal('Recovered'),'corrected_body_equal':equal('Corrected'),
         'original_local_count':len(original['locals']),'source_sha256':sha(source),'compiler_sha256':manifest['compiler_sha256'],
         'fixture_identity_note':'Fixture method names differ. Equality is of complete canonical body, local signature and EH; no game states are affected.'}
 (ROOT/'reports/round7-store-anchor-fixture.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8')
 print(json.dumps(result));assert not result['recovered_body_equal'] and result['corrected_body_equal']


if __name__=='__main__':main()
