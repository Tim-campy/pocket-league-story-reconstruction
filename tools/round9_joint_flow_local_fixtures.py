"""Joint source-shape toy controls only; no target compilation."""
import json,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if (p/'tools/compiler_probes.py').is_file());sys.path.insert(0,str(ROOT/'tools'))
from round9_part_b_fixtures import compile_smcs,query as bquery,apply,find,body_difference
from compare_kairo import sha
try:import round9_joint_flow_local_calculate as joint
except ImportError:import calculate as joint
WORK=ROOT/'build/round9/part-d-joint'
SOURCE=r'''namespace form {
 public class U {public int GetMatchInfoJoin(){return 1;}}
 public class A {public bool CheckAlreadyEvent(int n){return false;}public void ExecEvent(int n,object a,object b){}}
 public class GameForm {
 U udat_=new U();A apdat_=new A();object[] eventData_=new object[3];bool CheckAlreadyEvent(object e){return e==this;}bool CheckTermScript(object e){return e!=this;}void ExecEvent(object e){}void SetBattleData(){}void Use(int n){}
 public bool _update(int count){for(int i=0;i<eventData_.Length;i++){if(((eventData_[i]==null)||(CheckAlreadyEvent(eventData_[i])))||(!(CheckTermScript(eventData_[i])))){continue;}ExecEvent(eventData_[i]);return false;}if(count>0){SetBattleData();if (udat_.GetMatchInfoJoin() == 0 && !apdat_.CheckAlreadyEvent(176)){apdat_.ExecEvent(176,null,null);}}int add=count;Use(add);return false;}
 public bool Expected(int count){for(int i=0;i<eventData_.Length;i++){if((eventData_[i]==null)||(CheckAlreadyEvent(eventData_[i]))){continue;}if(CheckTermScript(eventData_[i])){ExecEvent(eventData_[i]);return false;}}if(count>0){SetBattleData();int testedMatchInfoJoin=udat_.GetMatchInfoJoin();if(testedMatchInfoJoin==0&&!apdat_.CheckAlreadyEvent(176)){apdat_.ExecEvent(176,null,null);}}int add=count;Use(add);return false;}
 }
 public class SubForm {
 U udat_=new U();int type_;void Use(int n){}void Use(object n){}object Get(){return this;}
 public bool Init(){if(type_==3){Use(3);}else if(type_!=6){if(type_==33){switch(udat_.GetMatchInfoJoin()){case 1:{object item=Get();Use(item);return false;}case 2:Use(2);return false;}object tail=Get();Use(tail);}else if(type_==36){switch(udat_.GetMatchInfoJoin()){case 0:{object item=Get();Use(item);return false;}case 1:Use(1);return false;default:Use(5);return false;}}}return true;}
 public bool Expected(){if(type_==3){Use(3);}else if(type_==6){}else{if(type_==33){int testedMatchInfoJoin1=udat_.GetMatchInfoJoin();if(testedMatchInfoJoin1==1){{object item=Get();Use(item);return false;}}else if(testedMatchInfoJoin1==2){Use(2);return false;}object tail=Get();Use(tail);}else if(type_==36){int testedMatchInfoJoin2=udat_.GetMatchInfoJoin();if(testedMatchInfoJoin2==0){{object item=Get();Use(item);return false;}}else if(testedMatchInfoJoin2==1){Use(1);return false;}else{Use(5);return false;}}}return true;}
 }
}'''.replace('\n','\r\n')

def main():
 manifest=json.loads((ROOT/'build/round9/cycles/b10/after/Assembly-CSharp/build-manifest.json').read_text(encoding='utf-8-sig'))
 folder=WORK/'fixtures';folder.mkdir(parents=True,exist_ok=True);path=folder/'Input.cs';mp=folder/'manifest.json';mp.write_text(json.dumps(dict(manifest,sources=[dict(path=str(path),input_path=str(path))])))
 path.write_bytes(SOURCE.encode('utf-8'));before=compile_smcs(folder/'baseline',SOURCE,manifest);current=SOURCE
 for rule in ['b9-event-loop-split-guard','b9-empty-arm-common-return']:
  path.write_bytes(current.encode('utf-8'));patches=[p for r in bquery(path,mp,rule) for p in r['patches']];assert len(patches)==1,(rule,patches);current=apply(current,patches[0])
 flow=compile_smcs(folder/'flow-only',current,manifest)
 needle='if (udat_.GetMatchInfoJoin() == 0 && !apdat_.CheckAlreadyEvent(176))';assert current.count(needle)==1
 current=current.replace(needle,'int testedMatchInfoJoin=udat_.GetMatchInfoJoin();if(testedMatchInfoJoin==0&&!apdat_.CheckAlreadyEvent(176))')
 path.write_bytes(current.encode('utf-8'));patches=[p for r in joint.ff.query(path,mp,dict(round7_cycle_rule='d9-joint-flow-local-stores',target_method='Init')) for p in r['patches']];assert len(patches)==2
 for patch in sorted(patches,key=lambda p:p['start'],reverse=True):current=apply(current,patch)
 after=compile_smcs(folder/'joint',current,manifest);checks=[]
 for owner,name in [('form.GameForm','_update'),('form.SubForm','Init')]:
  actual=find(after,owner,name);expected=find(after,owner,'Expected');delta=body_difference(expected,actual);assert not delta,(owner,delta)
  flowdelta=body_difference(expected,find(flow,owner,name));assert flowdelta,(owner,'flow-only unexpectedly equal')
  checks.append(dict(method=owner+'::'+name,full_body_differences=delta,flow_only_differences=flowdelta,original=expected,actual=actual,flow_only=find(flow,owner,name)))
 # A switch-owned break is not silently converted to a return/loop jump.
 negative_source=SOURCE.replace('case 2:Use(2);return false;','case 2:Use(2);break;')
 path.write_bytes(negative_source.encode('utf-8'));assert not any(r['patches'] for r in joint.ff.query(path,mp,dict(round7_cycle_rule='d9-joint-flow-local-stores',target_method='Init')))
 negative_second=SOURCE.replace('default:Use(5);return false;','default:Use(5);break;')
 path.write_bytes(negative_second.encode('utf-8'));assert not any(r['patches'] for r in joint.ff.query(path,mp,dict(round7_cycle_rule='d9-joint-flow-local-stores',target_method='Init')))
 result=dict(operation='WIP-joint-B-flow-and-named-local-smcs-fixtures',helper_sha256=sha(joint.ff.RULE),driver_sha256=sha(Path(__file__)),compiler_sha256=manifest['compiler_sha256'],
  historical_fixture_compilations=3,game_compilations=0,full_body_controls_equal=2,fixtures=checks,negative='switch-owned break refused; no jump destination reclassified')
 (WORK/'fixture-results.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in result.items() if k!='fixtures'}))
if __name__=='__main__':main()
