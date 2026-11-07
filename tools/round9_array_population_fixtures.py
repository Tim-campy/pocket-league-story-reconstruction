"""Independent smcs array population fixtures; no game source is compiled."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import round9_float_identity_fixtures as host
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round6_alignment import align_methods
from round9_array_population_hints import array_population_hints
WORK=ROOT/'build/round9/array-population';WORK.mkdir(parents=True,exist_ok=True)
host.WORK=WORK;host.RULE=ROOT/'tools/Round9ArrayPopulationRules.cs'
host.HOST=host.HOST.replace('AddRound9FloatingIdentityRules','AddRound9ArrayPopulationRules')
compiler.HERE=WORK
SOURCE='''using System;
public class Fixture {
 public static int Calls; public static int Mark(int n){Calls=Calls*10+n;return n;}
 public static object Make(int n){Calls=Calls*10+n;return new object();}
 public int ExpectedDeclare(){int[] a=new int[2];a[0]=Mark(7);a[1]=Mark(8);return a[0]+a[1];}
 public int Declare(){int[] a=new int[2]{Mark(7),Mark(8)};return a[0]+a[1];}
 public int ExpectedAssign(bool flag){int[] a=null;if(flag){a=new int[1];a[0]=Mark(7);}return a[0];}
 public int Assign(bool flag){int[] a=null;if(flag){a=new int[1]{Mark(7)};}return a[0];}
 public object ExpectedReferences(){object[] a=new object[2];a[0]=Make(1);a[1]=Make(2);return a[0];}
 public object References(){object[] a=new object[2]{Make(1),Make(2)};return a[0];}
 public int ExpectedInferred(){var a=new int[2];a[0]=Mark(1);a[1]=Mark(2);return a[1];}
 public int Inferred(){var a=new int[]{Mark(1),Mark(2)};return a[1];}
 public static int Use(int[] a){return a[0];}
 public int RejectArgument(){return Use(new int[1]{Mark(1)});}
 public int[] Field;public int RejectField(){Field=new int[1]{Mark(1)};return Field[0];}
 public int RejectMulti(){int[,] a=new int[1,1]{{Mark(1)}};return a[0,0];}
 public int RejectSelf(){int[] a=null;a=new int[1]{a==null?1:2};return a[0];}
 public object RejectCapture(){System.Threading.ThreadStart[] a=new System.Threading.ThreadStart[1]{delegate{Calls++;}};return a[0];}
 public int RejectEmbedded(bool f){int[] a=null;if(f)a=new int[1]{Mark(1)};return a[0];}
 public int RejectPlain(){int[] a=new int[1];a[0]=Mark(1);return a[0];}
}
'''

def main():
 manifest=json.loads((ROOT/'build/round9/cycles/cycle08/after/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
 path=WORK/'Input.cs';mp=WORK/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[{'path':str(path),'input_path':str(path)}])))
 baseline=compiler.compile_toy(SOURCE,'baseline');rows=[]
 for name in ('Declare','Assign','References','Inferred'):
  original=baseline['Expected'+name];rebuilt=baseline[name];aligned=align_methods(original,rebuilt)
  hints=array_population_hints(original,rebuilt,aligned['original_offset'],aligned['rebuilt_offset']);assert hints,(name,aligned)
  path.write_bytes(SOURCE.encode('utf-8'));start=SOURCE.index(' '+name+'(');point=SOURCE.index('new ',start)
  request=dict(hints,compiler='smcs',round7_cycle_rule='d9-array-local-before-population',target_method=name,point=point)
  patches=[p for r in host.query(path,mp,request) for p in r['patches']];assert len(patches)==1,(name,patches)
  candidate=host.apply(SOURCE,patches[0]);actual=compiler.compile_toy(candidate,name)[name]
  delta=[x for x in differences(original,actual)[0] if x!='signature'];assert not delta,(name,delta)
  rows.append({'method':name,'full_body_equal':True,'hints':hints,'patch':patches[0],
   'original_call_order':[i.get('operand') for i in original['instructions'] if i['opcode'] in ('call','callvirt')],
   'candidate_call_order':[i.get('operand') for i in actual['instructions'] if i['opcode'] in ('call','callvirt')]})
  assert rows[-1]['original_call_order']==rows[-1]['candidate_call_order']
 path.write_bytes(SOURCE.encode('utf-8'));negative=[]
 for name in ('RejectArgument','RejectField','RejectMulti','RejectSelf','RejectCapture','RejectEmbedded','RejectPlain'):
  request=dict(compiler='smcs',round7_cycle_rule='d9-array-local-before-population',round9_array_population_valid=True,target_method=name)
  assert not any(r['patches'] for r in host.query(path,mp,request)),name;negative.append(name)
 assert not any(r['patches'] for r in host.query(path,mp,dict(compiler='smcs',round7_cycle_rule='d9-array-local-before-population')));negative.append('no-original-evidence')
 # Fresh hint refuses an ordinary unchanged body and a different allocation type.
 assert not array_population_hints(baseline['ExpectedDeclare'],baseline['ExpectedDeclare'],0,0)
 original=baseline['ExpectedDeclare'];rebuilt=json.loads(json.dumps(baseline['Declare']));a=align_methods(original,rebuilt)
 at=next(i for i in rebuilt['instructions'] if i['opcode']=='newarr');at['operand']={'type':'different'}
 assert not array_population_hints(original,rebuilt,a['original_offset'],a['rebuilt_offset']);negative.extend(['no-frontier-evidence','different-newarr-type'])
 report={'operation':'smcs-local-before-array-population-fixtures','compiler_sha256':sha(compiler.SMCS),
  'source_sha256':__import__('hashlib').sha256(SOURCE.encode()).hexdigest(),'helper_sha256':sha(host.RULE),
  'hints_sha256':sha(ROOT/'tools/round9_array_population_hints.py'),'driver_sha256':sha(Path(__file__)),
  'game_compilations':0,'toy_compilations':5,'positive_full_bodies':len(rows),'negative_controls':len(negative),
  'fixtures':rows,'negatives':negative,'evaluation_order_preserved':True,'original_bytes_copied':False,
  'initial_harness_failures':['write_text translated fixture LF to CRLF, invalidating requested offsets; fixed with exact UTF-8 write_bytes before oracle tests'],
  'limitations':'Single-dimension local declarations/assignments only; model and game-body verification remain required.'}
 target=ROOT/'reports/round9-array-population-fixtures.json';target.write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8')
 print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
if __name__=='__main__':main()
