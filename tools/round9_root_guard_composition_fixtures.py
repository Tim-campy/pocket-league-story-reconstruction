"""Independent smcs controls: preserve root declarations while restoring guard."""
import hashlib,json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round9_root_guard_composition_source import transform_root_guard
WORK=ROOT/'build/round9/root-guard-composition-fixtures';WORK.mkdir(parents=True,exist_ok=True);compiler.HERE=WORK
HEADER='''using System;public class LineupForm{public int Value;public LineupForm(int n){Value=n;}}public class SupervisorData{public int Value;public SupervisorData(int n){Value=n;}}public class PlayerData{public int Value;public PlayerData(int n){Value=n;}}public class Fixture{public int Count;
'''
BEFORE1='''public void Run(bool guard,int n){Count++;if(guard){}else{return;}LineupForm lineupForm=new LineupForm(Count++);while(n>0){try{Count+=lineupForm.Value;}catch(Exception){Count--;}n--;}}'''
EXPECTED1='''public void Run(bool guard,int n){LineupForm lineupForm;Count++;if(guard){lineupForm=new LineupForm(Count++);while(n>0){try{Count+=lineupForm.Value;}catch(Exception){Count--;}n--;}}}'''
BEFORE2='''public void Run(int first,int last,int n){Count+=3;if(first!=last){}else{return;}SupervisorData supervisorData2=new SupervisorData(Count++);for(int l=0;l<n;l++){PlayerData playerData=new PlayerData(l);for(int m=0;m<playerData.Value;m++){Count+=supervisorData2.Value;}}}'''
EXPECTED2='''public void Run(int first,int last,int n){SupervisorData supervisorData2;Count+=3;if(first!=last){supervisorData2=new SupervisorData(Count++);for(int l=0;l<n;l++){PlayerData playerData=new PlayerData(l);for(int m=0;m<playerData.Value;m++){Count+=supervisorData2.Value;}}}}'''
BEFORE3='''public void Run(bool guard){if(guard){}else{return;}LineupForm first=new LineupForm(Count++);SupervisorData second=new SupervisorData(Count++);Count+=first.Value+second.Value;}'''
EXPECTED3='''public void Run(bool guard){LineupForm first;SupervisorData second;if(guard){first=new LineupForm(Count++);second=new SupervisorData(Count++);Count+=first.Value+second.Value;}}'''
def main():
 rows=[]
 for label,before,expected in [('protected-tail',BEFORE1,EXPECTED1),('nested-for-locals',BEFORE2,EXPECTED2),('two-root-declarations',BEFORE3,EXPECTED3)]:
  after=transform_root_guard(before);control=compiler.compile_toy(HEADER+expected+'}',label+'-expected')['Run'];actual=compiler.compile_toy(HEADER+after+'}',label+'-candidate')['Run'];delta=differences(control,actual)[0];assert not delta,(label,delta)
  rows.append({'case':label,'before':before,'after':after,'expected_manual':expected,'full_body_equal':True,'locals_equal':control['locals']==actual['locals'],'initializer_evaluation_remains_after_condition':True})
 negatives=[]
 cases=[('var',BEFORE1.replace('LineupForm lineupForm=','var lineupForm=')),('condition-uses-local',BEFORE1.replace('if(guard)','if(lineupForm!=null)')),('capture',BEFORE1.replace('Count+=lineupForm.Value;','System.Threading.ThreadStart a=delegate{Count+=lineupForm.Value;};a();')),('nonvoid',BEFORE1.replace('public void Run','public int Run')),('tail-transfer',BEFORE2.replace('Count+=supervisorData2.Value;','if(m==1)break;Count+=supervisorData2.Value;')),('missing-else-return',BEFORE1.replace('else{return;}','else{Count++;}')),('nonleading-root-declaration',BEFORE1.replace('while(n>0)','Count++;SupervisorData later=new SupervisorData(1);while(n>0)'))]
 for name,bad in cases:
  try:transform_root_guard(bad)
  except ValueError:negatives.append(name)
  else:raise AssertionError(name)
 report={'operation':'smcs-root-declaration-plus-positive-tail-composition-fixtures','compiler_sha256':sha(compiler.SMCS),'producer_sha256':sha(ROOT/'tools/round9_root_guard_composition_source.py'),'driver_sha256':sha(Path(__file__)),
  'game_compilations':0,'toy_compilations':6,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'fixtures':rows,'negatives':negatives,
  'child_loop_variables_not_hoisted':True,'no_compiler_generated_class_authored':True,'limitations':'Explicit root declarations at leading tail only; no captures or unsupported transfers.'}
 (ROOT/'reports/round9-root-guard-composition-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items() if k!='fixtures'}))
if __name__=='__main__':main()
