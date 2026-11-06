"""Toy-only Roslyn 1.1 full-body oracle for per-arm local assignments."""
import hashlib,json,shutil,subprocess,sys,importlib.util
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file())
sys.path.insert(0,str(ROOT/'tools'))
from compiler_probes import ROSLYN
from round7_roslyn_for_declaration_fixture import compile_fixture
from compare_kairo import differences,sha
from round6_alignment import align_methods
from round9_per_arm_store_hints import per_arm_local_store_hints
SCRIPT=Path(__file__).resolve().parent;WORK=ROOT/'build/round9/part-d-stores';RULE=SCRIPT/'Round9PerArmStoreRules.cs'
if SCRIPT==ROOT/'tools':
 import round9_float_identity_fixtures as ff
else:
 spec=importlib.util.spec_from_file_location('float_fixture_source',WORK.parent/'part-d-float/fixtures.py')
 saved=list(sys.path);sys.path.insert(0,str(WORK.parent/'part-d-float'));ff=importlib.util.module_from_spec(spec);spec.loader.exec_module(ff);sys.path[:]=saved
 sys.modules.pop('hints',None) # The imported float driver has a sibling module of the same name.
HOST=ff.HOST.replace('AddRound9FloatingIdentityRules','AddRound9PerArmStoreRules').replace('NoEmitFloatOccurrence','NoEmitPerArmStores')
SOURCE='''// UTF-16 control: 🔎
public class Fixture {
 public class Node { public Node left_,right_;public int value_; }
 int ReadBit(){return 1;}void Consume(int value){}
 public void Expand(Node root,int count){for(int i=0;i<count;i++){Node node=root;while(node.left_!=null){node=(ReadBit()!=0)?node.right_:node.left_;}Consume(node.value_);}}
 public void ExpectedExpand(Node root,int count){for(int i=0;i<count;i++){Node node=root;while(node.left_!=null){if(!(ReadBit()!=0)){node=node.left_;}else{node=node.right_;}}Consume(node.value_);}}
 float width_;
 public float Align(float cursor,float extent,int anchor){if(anchor==4){cursor=((width_!=-1f)?(width_-extent):(cursor-extent));}return cursor;}
 public float ExpectedAlign(float cursor,float extent,int anchor){if(anchor==4){if(!(width_!=-1f)){cursor=(cursor-extent);}else{cursor=(width_-extent);}}return cursor;}
 public float AlignLocal(float cursor,float extent,int anchor){float x=cursor;if(anchor==4){x=((width_!=-1f)?(width_-extent):(x-extent));}return x;}
 public float ExpectedAlignLocal(float cursor,float extent,int anchor){float x=cursor;if(anchor==4){if(!(width_!=-1f)){x=(x-extent);}else{x=(width_-extent);}}return x;}
 public void Field(Node root){root.left_=(ReadBit()!=0)?root.right_:root.left_;}
 public void Indexed(Node[] data,int i,Node root){data[i++]=(ReadBit()!=0)?root.right_:root.left_;}
 public void NoTernary(Node root){Node node=root;node=node.left_;Consume(node.value_);}
}
'''.replace('\n','\r\n')

def ensure_host():
 digest=hashlib.sha256(RULE.read_bytes()+HOST.encode()).hexdigest();folder=WORK/'helper'/digest[:16];folder.mkdir(parents=True,exist_ok=True);target=folder/'StoreHost.dll'
 vs=next(p for p in ROSLYN.parents if (p/'dotnet').is_dir());runtime=vs/'dotnet/net10.0/runtime';framework=runtime/'shared/Microsoft.NETCore.App/10.0.10'
 refs=[framework/(n+'.dll') for n in ['System.Private.CoreLib','System.Runtime','System.Console','System.Linq','System.Collections','System.Text.Json','System.Collections.Immutable','System.Memory','netstandard']]+[ROSLYN.parent/(n+'.dll') for n in ['Microsoft.CodeAnalysis','Microsoft.CodeAnalysis.CSharp']]
 if not target.exists():
  host=folder/'Host.cs';host.write_text(HOST,encoding='utf-8');rsp=folder/'compile.rsp';args=['/nologo','/nostdlib+','/target:exe','/out:'+str(target)]+['/r:'+str(p) for p in refs]+[str(host),str(RULE)];rsp.write_text('\n'.join('"'+a+'"' for a in args)+'\n',encoding='utf-8');result=subprocess.run([str(ROSLYN),'/noconfig','@'+str(rsp)],capture_output=True)
  if result.returncode:raise RuntimeError((result.stdout+result.stderr).decode(errors='replace'))
  for p in refs[-2:]:shutil.copy2(p,folder/p.name)
  target.with_suffix('.runtimeconfig.json').write_text(json.dumps({'runtimeOptions':{'tfm':'net10.0','framework':{'name':'Microsoft.NETCore.App','version':'10.0.10'}}}))
 return [str(runtime/'dotnet.exe'),str(target)]

def query(path,manifest,request):return json.loads(subprocess.run(ensure_host()+[str(path),str(manifest),json.dumps(request)],capture_output=True,check=True).stdout)
apply=ff.apply
def method(rows,name):return next(v for k,v in rows.items() if k.startswith('Fixture::'+name+'('))
def body_diff(a,b):return [d for d in differences(a,b)[0] if d!='signature']

def main():
 manifest=json.loads((ROOT/'build/round9/cycles/a05/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'))
 folder=WORK/'fixtures';folder.mkdir(parents=True,exist_ok=True);path=folder/'Input.cs';mp=folder/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[dict(path=str(path),input_path=str(path))])))
 base,_=compile_fixture(folder/'baseline',SOURCE,manifest);current=SOURCE;checks=[]
 for name,control in [('Expand','ExpectedExpand'),('AlignLocal','ExpectedAlignLocal')]:
  alignment=align_methods(method(base,control),method(base,name));hints=per_arm_local_store_hints(method(base,control),method(base,name),alignment['original_offset'],alignment['rebuilt_offset']);assert hints,(name,alignment)
  path.write_bytes(current.encode('utf-8'));request=dict(hints,round7_cycle_rule='d9-calculated-per-arm-local-stores',target_method=name)
  patches=[p for r in query(path,mp,request) for p in r['patches']];assert len(patches)==1,(name,patches)
  current=apply(current,patches[0]);actual,_=compile_fixture(folder/name,current,manifest);delta=body_diff(method(actual,name),method(base,control));assert not delta,(name,delta)
  checks.append(dict(method=name,control=control,hints=hints,patch=patches[0],body_differences=delta,actual=method(actual,name),expected=method(base,control)))
 path.write_bytes(current.encode('utf-8'))
 default=dict(round7_cycle_rule='d9-calculated-per-arm-local-stores',round9_original_same_slot_arm_stores=True,round9_per_arm_store_slot=0,round9_per_arm_store_order='false-first')
 for name in ['Align','Field','Indexed','NoTernary']:
  assert not any(r['patches'] for r in query(path,mp,dict(default,target_method=name))),name
 for bad in [dict(round9_original_same_slot_arm_stores=False),dict(round9_per_arm_store_order='unknown'),dict(round9_per_arm_store_slot=-1)]:
  assert not any(r['patches'] for r in query(path,mp,dict(default,**bad)))
 result=dict(operation='historical-csc-per-arm-local-store-fixtures',compiler_sha256=manifest['compiler_sha256'],helper_sha256=sha(RULE),hints_sha256=sha(SCRIPT/'round9_per_arm_store_hints.py'),fixture_driver_sha256=sha(Path(__file__)),historical_fixture_compilations=3,
  game_compilations=0,full_body_controls_equal=2,fixtures=checks,negatives=['parameter target','field target','indexed target with side effect','no ternary','missing/invalid original evidence'],preserves_condition_expression_and_unordered_float_branches=True)
 (WORK/'fixture-results.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items() if k!='fixtures'}))

if __name__=='__main__':main()
