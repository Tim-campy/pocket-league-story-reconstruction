"""Independent historical Roslyn controls for value/branch Boolean contexts."""
import json,sys,hashlib
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import round9_float_identity_fixtures as host
from round7_roslyn_for_declaration_fixture import compile_fixture
from compare_kairo import differences,sha
from round6_alignment import align_methods
from round9_boolean_context_hints import boolean_context_hints
WORK=ROOT/'build/round9/boolean-context';WORK.mkdir(parents=True,exist_ok=True)
host.WORK=WORK;host.RULE=ROOT/'tools/Round9BooleanContextRules.cs';host.HOST=host.HOST.replace('AddRound9FloatingIdentityRules','AddRound9BooleanContextRules')
SOURCE='''public class Flag{public static bool operator true(Flag f){return true;}public static bool operator false(Flag f){return false;}public static Flag operator &(Flag a,Flag b){return a;}}
public class Compared{public static int Calls;public static bool operator ==(Compared a,Compared b){Calls++;return (object)a==(object)b;}public static bool operator !=(Compared a,Compared b){Calls++;return (object)a!=(object)b;}public override bool Equals(object o){return (object)this==o;}public override int GetHashCode(){return 0;}}
public class Fixture{
 public int Count;public bool Guard(){Count++;return true;}
 public void ExpectedValue(bool a,bool b,bool tail){if(!(a&&b)?false:!tail)Count++;}
 public void Value(bool a,bool b,bool tail){if(a&&b&&!tail)Count++;}
 public void ExpectedFloat(float value,bool tail){if(!(Guard()&&value>0f)?false:!tail)Count++;}
 public void Float(float value,bool tail){if(Guard()&&value>0f&&!tail)Count++;}
 public void ExpectedEquality(Compared x,bool a,bool tail){if(!((x==null)&&a)?false:!tail)Count++;}
 public void Equality(Compared x,bool a,bool tail){if((x==null)&&a&&!tail)Count++;}
 public void ExpectedInline(int mask){if((mask&2)!=0){Count++;}else{Count--;}}
 public void Inline(int mask){bool alias=(mask&2)!=0;if(alias){Count++;}else{Count--;}}
 public void ExpectedInlineZero(int mask){if((mask&2)==0){Count++;}else{Count--;}}
 public void InlineZero(int mask){bool alias=(mask&2)==0;if(alias){Count++;}else{Count--;}}
 public void RejectMultiple(int mask){bool alias=(mask&2)!=0;if(alias)Count++;if(alias)Count--;}
 public void RejectSeparated(int mask){bool alias=(mask&2)!=0;Count++;if(alias)Count++;}
 public void RejectCustom(Flag a,Flag b){if(a&&b)Count++;}
 public void RejectNoAnd(bool a){if(a)Count++;}
}
'''
def method(rows,name):return next(v for k,v in rows.items() if k.startswith('Fixture::'+name+'('))
def main():
 manifest=json.loads((ROOT/'build/round9/cycles/cycle10/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'))
 path=WORK/'Input.cs';mp=WORK/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[{'path':str(path),'input_path':str(path)}])))
 baseline,_=compile_fixture(WORK/'baseline',SOURCE,manifest);rows=[]
 for name in ('Value','Float','Equality','Inline','InlineZero'):
  original=method(baseline,'Expected'+name);rebuilt=method(baseline,name);aligned=align_methods(original,rebuilt)
  hints=boolean_context_hints(original,rebuilt,aligned['original_offset'],aligned['rebuilt_offset']);assert hints,(name,aligned)
  path.write_bytes(SOURCE.encode());start=SOURCE.index(' '+name+'(');point=SOURCE.index('if(',start)
  request=dict(hints,compiler='roslyn',round7_cycle_rule='d9-boolean-value-branch-context',target_method=name,point=point)
  patches=[p for r in host.query(path,mp,request) for p in r['patches']];assert len(patches)==1,(name,patches,hints)
  if name.startswith('Inline'):
   declared=dict(request,point=SOURCE.index('bool alias',start));from_declaration=[p for r in host.query(path,mp,declared) for p in r['patches']]
   assert len(from_declaration)==1 and all(from_declaration[0][k]==patches[0][k] for k in ('start','end','before','after')),(name,from_declaration)
  candidate=host.apply(SOURCE,patches[0]);actuals,_=compile_fixture(WORK/name,candidate,manifest);actual=method(actuals,name)
  delta=[x for x in differences(original,actual)[0] if x!='signature'];assert not delta,(name,delta)
  rows.append({'method':name,'full_body_equal':True,'locals_equal':original['locals']==actual['locals'],'hint':hints,'patch':patches[0],
   'calls_original':[x.get('operand') for x in original['instructions'] if x['opcode'] in ('call','callvirt')],
   'calls_candidate':[x.get('operand') for x in actual['instructions'] if x['opcode'] in ('call','callvirt')]})
  assert rows[-1]['calls_original']==rows[-1]['calls_candidate']
 path.write_bytes(SOURCE.encode());negative=[]
 for name,mode in [('RejectMultiple','inline-adjacent-alias'),('RejectSeparated','inline-adjacent-alias'),('RejectCustom','materialize-tail'),('RejectNoAnd','materialize-tail')]:
  request=dict(compiler='roslyn',round7_cycle_rule='d9-boolean-value-branch-context',round9_boolean_context_mode=mode,target_method=name)
  assert not any(r['patches'] for r in host.query(path,mp,request)),name;negative.append(name)
 assert not any(r['patches'] for r in host.query(path,mp,dict(compiler='roslyn',round7_cycle_rule='d9-boolean-value-branch-context')));negative.append('no-original-evidence')
 report={'operation':'historical-Roslyn-Boolean-value-branch-context-fixtures','compiler_sha256':manifest['compiler_sha256'],
  'source_sha256':hashlib.sha256(SOURCE.encode()).hexdigest(),'helper_sha256':sha(host.RULE),'hints_sha256':sha(ROOT/'tools/round9_boolean_context_hints.py'),'driver_sha256':sha(Path(__file__)),
  'toy_compilations':6,'compilation_count_scope':'final successful run; previous implementation runs compiled2+5+6+6 toy assemblies','game_compilations':0,'positive_full_bodies':len(rows),'negative_controls':len(negative),'fixtures':rows,'negatives':negative,
  'alias_declaration_and_if_mapping_same_patch':True,
  'float_unordered_semantics_preserved_by_explicit_not':True,'no_new_named_locals':True,
  'initial_harness_failures':['Overload guard initially rejected builtin Single comparison symbols as though user-defined; fixed before final controls','Static review found generic nested-overload guard excessive; restricted to outer logical operator and added Equality fixture. Actual renderForceMaterial_ is kairo.unity.graphics.Material without overloaded equality, so no real-target veto was demonstrated','Static review found mapped alias declaration needs adjacent sibling If; added precise sibling selection and equal-patch controls at both source positions before game compilation'],
  'limitations':'Only original value-join or redundant zero-normalization shapes; adjacent unique-use alias only. Every game candidate needs independent model and complete body oracle.'}
 (ROOT/'reports/round9-boolean-context-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
if __name__=='__main__':main()
