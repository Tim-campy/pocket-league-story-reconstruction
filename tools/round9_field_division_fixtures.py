"""Independent smcs controls for stable field division; no game compilation."""
import json,sys,hashlib
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import round9_float_identity_fixtures as host
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round6_alignment import align_methods
from round9_field_division_hints import field_division_hints
WORK=ROOT/'build/round9/field-division';WORK.mkdir(parents=True,exist_ok=True)
host.WORK=WORK;host.RULE=ROOT/'tools/Round9FieldDivisionRules.cs';host.HOST=host.HOST.replace('AddRound9FloatingIdentityRules','AddRound9FieldDivisionRules');compiler.HERE=WORK
SOURCE='''public class Box{public int I;public float F;public double D;public long L;public byte B;public decimal M;public volatile int V;}
public struct ValueBox{public int I;}
public class Fixture {
 public int Field;public Box Data=new Box();public ValueBox Value;public int Calls;
 public Box Get(){Calls++;return Data;}public Box Property{get{Calls++;return Data;}}
 public void ExpectedImplicit(){Field=Field/2;}public void Implicit(){Field/=2;}
 public void ExpectedLocal(){Box a=Data;a.I=a.I/2;}public void Local(){Box a=Data;a.I/=2;}
 public void ExpectedFloat(float scale){Data.F=Data.F/scale;}public void Float(float scale){Data.F/=scale;}
 public void ExpectedDouble(double scale){Data.D=Data.D/scale;}public void Double(double scale){Data.D/=scale;}
 public void ExpectedLong(long scale){Data.L=Data.L/scale;}public void Long(long scale){Data.L/=scale;}
 public void ExpectedTwice(){Field=Field/2;Field=Field/4;}public void Twice(){Field/=2;Field/=4;}
 public void RejectMethod(){Get().I/=2;}public void RejectProperty(){Property.I/=2;}
 public void RejectVolatile(){Data.V/=2;}public void RejectByte(){Data.B/=2;}
 public void RejectDecimal(){Data.M/=2m;}public void RejectConvert(){Data.F/=2;}
 public void RejectValue(){Value.I/=2;}
}
'''
def main():
 manifest=json.loads((ROOT/'build/round9/cycles/cycle09/after/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
 path=WORK/'Input.cs';mp=WORK/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[{'path':str(path),'input_path':str(path)}])))
 baseline=compiler.compile_toy(SOURCE,'baseline');rows=[];compilations=1
 for name in ('Implicit','Local','Float','Double','Long','Twice'):
  source=SOURCE;current=baseline[name];control=baseline['Expected'+name];steps=[]
  for iteration in range(2 if name=='Twice' else 1):
   aligned=align_methods(control,current);hints=field_division_hints(control,current,aligned['original_offset'],aligned['rebuilt_offset']);assert hints,(name,aligned)
   path.write_bytes(source.encode());start=source.index(' '+name+'(');point=source.index('/=',start)
   request=dict(hints,compiler='smcs',round7_cycle_rule='d9-explicit-field-division',target_method=name,point=point)
   patches=[p for r in host.query(path,mp,request) for p in r['patches']];assert patches,(name,request)
   patch=min(patches,key=lambda x:x['distance']);source=host.apply(source,patch);current=compiler.compile_toy(source,name+str(iteration))[name];compilations+=1;steps.append(patch)
  delta=[x for x in differences(control,current)[0] if x!='signature'];assert not delta,(name,delta)
  rows.append({'method':name,'full_body_equal':True,'steps':steps,'locals_equal':control['locals']==current['locals']})
 path.write_bytes(SOURCE.encode());negative=[]
 for name in ('RejectMethod','RejectProperty','RejectVolatile','RejectByte','RejectDecimal','RejectConvert','RejectValue'):
  request=dict(compiler='smcs',round7_cycle_rule='d9-explicit-field-division',round9_division_repeated_receiver=True,target_method=name)
  assert not any(r['patches'] for r in host.query(path,mp,request)),name;negative.append(name)
 assert not any(r['patches'] for r in host.query(path,mp,dict(compiler='smcs',round7_cycle_rule='d9-explicit-field-division')));negative.append('no-original-evidence')
 report={'operation':'smcs-stable-field-division-fixtures','compiler_sha256':sha(compiler.SMCS),'source_sha256':hashlib.sha256(SOURCE.encode()).hexdigest(),
  'helper_sha256':sha(host.RULE),'hints_sha256':sha(ROOT/'tools/round9_field_division_hints.py'),'driver_sha256':sha(Path(__file__)),
  'game_compilations':0,'toy_compilations':compilations,'positive_full_bodies':len(rows),'negative_controls':len(negative),
  'fixtures':rows,'negatives':negative,'no_new_locals':True,'checked_context_preserved':True,
  'limitations':'Only equal primitive field/RHS types and repeatable field/local/parameter/this receivers; division only, old multiplication family unchanged.'}
 (ROOT/'reports/round9-field-division-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
if __name__=='__main__':main()
