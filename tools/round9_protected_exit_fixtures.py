"""Independent smcs EH transfer ownership controls; never compile game source."""
import json,sys,hashlib
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import round9_float_identity_fixtures as host
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round6_alignment import align_methods
from round9_protected_exit_hints import protected_exit_hints
WORK=ROOT/'build/round9/protected-exit';WORK.mkdir(parents=True,exist_ok=True)
host.WORK=WORK;host.RULE=ROOT/'tools/Round9ProtectedExitRules.cs';host.HOST=host.HOST.replace('AddRound9FloatingIdentityRules','AddRound9ProtectedExitRules');compiler.HERE=WORK
SOURCE='''using System;public class Fixture{public int Count;public void Work(bool fail){if(fail)throw new Exception();Count++;}
 public void ExpectedLoop(bool fail){for(int i=0;i<3;i++){try{Work(fail);break;}catch(Exception){Count+=i;}}Count+=7;}
 public void Loop(bool fail){for(int i=0;i<3;i++){try{Work(fail);}catch(Exception){Count+=i;continue;}break;}Count+=7;}
 public void ExpectedOuter(bool fail){for(int j=0;j<2;j++){for(int i=0;i<3;i++){try{Work(fail);break;}catch(Exception){Count+=i;}}}Count+=7;}
 public void Outer(bool fail){for(int j=0;j<2;j++){for(int i=0;i<3;i++){try{Work(fail);}catch(Exception){Count+=i;continue;}break;}}Count+=7;}
 public void ExpectedFinally(bool fail){for(int i=0;i<3;i++){try{Work(fail);break;}catch(Exception){Count+=i;}finally{Count+=17;}}Count+=7;}
 public void Finally(bool fail){for(int i=0;i<3;i++){try{Work(fail);}catch(Exception){Count+=i;continue;}finally{Count+=17;}break;}Count+=7;}
 public void ExpectedGuard(bool save,bool fail){if(save){try{Work(fail);}catch(Exception e){Count+=e.Message.Length;}}else{Count+=9;}}
 public void Guard(bool save,bool fail){if(save){try{Work(fail);return;}catch(Exception e){Count+=e.Message.Length;return;}}Count+=9;}
 public void ExpectedGuardPrefix(bool save,bool fail){Count+=3;if(save){try{Work(fail);}catch(Exception){Count++;}}else{Count+=9;}}
 public void GuardPrefix(bool save,bool fail){Count+=3;if(save){try{Work(fail);return;}catch(Exception){Count++;return;}}Count+=9;}
 public void RejectNoBreak(bool fail){for(int i=0;i<3;i++){try{Work(fail);}catch(Exception){Count++;continue;}Count+=4;}}
 public void RejectNoContinue(bool fail){for(int i=0;i<3;i++){try{Work(fail);}catch(Exception){Count++;}break;}}
 public void RejectWhile(bool fail){while(Count<3){try{Work(fail);}catch(Exception){Count++;continue;}break;}}
 public int RejectValue(bool save,bool fail){if(save){try{Work(fail);return 1;}catch(Exception){return 2;}}Count+=9;return 3;}
 public void RejectInnerReturn(bool save,bool fail){if(save){try{if(fail)return;Work(fail);return;}catch(Exception){return;}}Count+=9;}
 public void RejectFinally(bool save,bool fail){if(save){try{Work(fail);return;}finally{Count++;}}Count+=9;}
}
'''
def main():
 manifest=json.loads((ROOT/'build/round9/cycles/cycle11/after/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
 path=WORK/'Input.cs';mp=WORK/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[{'path':str(path),'input_path':str(path)}])))
 baseline=compiler.compile_toy(SOURCE,'baseline');rows=[]
 for name in ('Loop','Outer','Finally','Guard','GuardPrefix'):
  original=baseline['Expected'+name];rebuilt=baseline[name];aligned=align_methods(original,rebuilt)
  hints=protected_exit_hints(original,rebuilt,aligned['original_offset'],aligned['rebuilt_offset']);assert hints,(name,aligned)
  path.write_bytes(SOURCE.encode());start=SOURCE.index(' '+name+'(');point=SOURCE.index('try{',start)
  request=dict(hints,compiler='smcs',round7_cycle_rule='d9-protected-exit-owner',target_method=name,point=point)
  patches=[p for r in host.query(path,mp,request) for p in r['patches']];assert len(patches)==1,(name,patches)
  candidate=host.apply(SOURCE,patches[0]);actual=compiler.compile_toy(candidate,name)[name]
  delta=[x for x in differences(original,actual)[0] if x!='signature'];assert not delta,(name,delta)
  rows.append({'method':name,'full_body_equal':True,'locals_equal':original['locals']==actual['locals'],'hint':hints,'patch':patches[0],'EH_equal':original['exception_regions']==actual['exception_regions']})
 path.write_bytes(SOURCE.encode());negative=[]
 for name,mode in [('RejectNoBreak','loop-try-break'),('RejectNoContinue','loop-try-break'),('RejectWhile','loop-try-break'),('RejectValue','guard-try-returns'),('RejectInnerReturn','guard-try-returns'),('RejectFinally','guard-try-returns')]:
  request=dict(compiler='smcs',round7_cycle_rule='d9-protected-exit-owner',round9_protected_exit_mode=mode,target_method=name)
  assert not any(r['patches'] for r in host.query(path,mp,request)),name;negative.append(name)
 assert not any(r['patches'] for r in host.query(path,mp,dict(compiler='smcs',round7_cycle_rule='d9-protected-exit-owner')));negative.append('no-EH-evidence')
 report={'operation':'smcs-protected-transfer-owner-fixtures','compiler_sha256':sha(compiler.SMCS),'source_sha256':hashlib.sha256(SOURCE.encode()).hexdigest(),
  'helper_sha256':sha(host.RULE),'hints_sha256':sha(ROOT/'tools/round9_protected_exit_hints.py'),'driver_sha256':sha(Path(__file__)),
  'toy_compilations':6,'compilation_count_scope':'final successful run; earlier syntax-failed attempt plus4 compiled toy assemblies in initial hint-shape run','game_compilations':0,'positive_full_bodies':len(rows),'negative_controls':len(negative),'fixtures':rows,'negatives':negative,
  'no_new_locals':True,'EH_compared_in_body_oracle':True,'loop_ownership_and_finally_preserved':True,
  'initial_harness_failures':['Initial manual Finally fixture had one extra closing brace; compilation failed before any control comparison. Fixed before final fixture run.','Sequence aligner associated the original normal leave with first versus second rebuilt leave in short Guard fixture; hint now checks both placements against exact EH handler boundaries without normalizing instructions.'],
  'limitations':'Specific direct For/Try/terminal CatchContinue/outer Break, or root void If/TryReturns plus tail; no general EH tree inference.'}
 (ROOT/'reports/round9-protected-exit-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
if __name__=='__main__':main()
