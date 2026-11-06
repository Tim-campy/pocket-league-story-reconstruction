import json,sys
from pathlib import Path
ROOT=next(p for p in Path(__file__).resolve().parents if(p/'tools/test_round9_control_rewrite.py').exists());WIP=ROOT/'build/round9/multi-selector';WIP.mkdir(parents=True,exist_ok=True);sys.path[:0]=[str(ROOT/'tools')]
from test_round9_control_rewrite import compile_file,BASE,plain_types
from compare_kairo import body_cells
from round9_control_ast import Client
from round7_slot_model import SlotModelClient
from round9_multi_selector_calculate import calculate
BODY_OWN="""int prefix=11;if(type==3){switch(Selector(value)){case 0:Use(1);break;case 1:Use(2);break;}}else if(type==5){int n=-12;switch(Selector(value)){case 6:Use(n);break;case 8:Use(4);break;}}else if(type==7){int m=-12;switch(Selector(value)){case 12:Use(m);break;case 14:Use(6);break;}}Use(prefix);"""
BODY_GOAL="""int prefix=11;if(type==3){int first=Selector(value);if(first==0){Use(1);}else if(first==1){Use(2);}}else if(type==5){int n=-12;int second=Selector(value);if(second==6){Use(n);}else if(second==8){Use(4);}}else if(type==7){int m=-12;int third=Selector(value);if(third==12){Use(m);}else if(third==14){Use(6);}}Use(prefix);"""
def text(body):return 'public static class Fixture{static int state;static int Selector(int n){state++;return n;}static void Use(int n){state+=n;}public static void Test(int type,int value){'+body+'}}'
def main():
 w=WIP/'fixture';w.mkdir(exist_ok=True);source=w/'Fixture.cs';old=text(BODY_OWN);source.write_text(old,encoding='utf-8');mp=w/'manifest.json';mp.write_text(json.dumps(dict(BASE,sources=[{'path':str(source),'input_path':str(source)}])),encoding='utf-8');key='Fixture::Test(System.Int32,System.Int32)->System.Void'
 with Client(mp)as ast,SlotModelClient(mp)as slots:
  source_ir=ast.extract(source,old,key)[0];model=slots.predict(source,old,key);own=next(m for m in compile_file(source,w/'own/Fixture.dll')if m['stable_id']==key);goal_source=w/'Goal.cs';goal_source.write_text(text(BODY_GOAL),encoding='utf-8');goal=next(m for m in compile_file(goal_source,w/'goal/Fixture.dll')if m['stable_id']==key);out=calculate(source_ir,old,goal,model,plain_types(goal),own);assert out['eligible'],out;candidate=out['candidate_source'];prediction=slots.predict(source,candidate,key);assert prediction['complete']and prediction['predicted_types']==plain_types(goal);source.write_text(candidate,encoding='utf-8');actual=next(m for m in compile_file(source,w/'candidate/Fixture.dll')if m['stable_id']==key);assert body_cells(actual)==body_cells(goal)and actual['locals']==goal['locals']and actual['header']==goal['header']and actual['exception_regions']==goal['exception_regions']
  # A nested break belongs to a nested loop; never treat it as the switch exit.
  bad=text(BODY_OWN.replace('Use(1);break;','while(value>0){Use(1);break;}break;'));bad_ir=ast.extract(source,bad,key)[0];bad_model=slots.predict(source,bad,key);rejected=calculate(bad_ir,bad,goal,bad_model,plain_types(goal),own);assert not rejected['eligible']and rejected['reason']=='nested-transfer-or-switch-ownership'
  # Provenance is not chosen arbitrarily when original aliases have same case lists.
  # The same real original DTO is retained; duplicate source case provenance is refused.
  bad=text(BODY_OWN.replace('case 6:','case 0:').replace('case 8:','case 1:'));bad_ir=ast.extract(source,bad,key)[0];rejected2=calculate(bad_ir,bad,goal,model,plain_types(goal),own);assert not rejected2['eligible']
 result={'operation':'joint-selector-local-splitting-independent-smcs-fixture','game_compilations':0,'toy_compilations':3,'body_exact':True,'locals_exact':True,'header_exact':True,'EH_exact':True,'selector_calls_evaluated_once_each':True,'own_local_count':len(own['locals']),'goal_local_count':len(goal['locals']),'candidate_alias_slots':[(v['name'],v['slot'])for v in prediction['variables']if v['name'].startswith('recoveredSelector')],'negative_nested_break':rejected['reason'],'negative_nonbijective_provenance':rejected2['reason'],'source_AST_captured_before_toy_IL':True};(ROOT/'reports/round9-multi-selector-fixture.json').write_text(json.dumps(result,indent=2)+'\n',encoding='utf-8');print(json.dumps(result,indent=2))
if __name__=='__main__':main()
