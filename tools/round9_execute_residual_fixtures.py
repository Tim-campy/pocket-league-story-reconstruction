"""Historical Roslyn whole-composition fixtures: eager bool tail, null calls and owned dispatch."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import differences,sha
from round7_roslyn_for_declaration_fixture import compile_fixture
from round9_execute_residual_source import calculate_execute_residual,transform_execute_residual
HEADER="""using System; public enum AppEventType{Sleep} public class SaveRecorder{public int Count; public void Lock(){Count++;} public void Unlock(bool b){Count+=b?1:2;}public void Save(){Count++;}public void SaveAll(){Count+=2;}} public class Fixture{public bool lastKeyguard_;public SaveRecorder recorder;public int Reserve,Count;void ProcessEvent(AppEventType e){Count++;}
"""
BASE="""public bool Run(bool input)
{
 bool flag = input;
 SaveRecorder instance4 = recorder;
 SaveRecorder instance = recorder;
 int reserveSave = Reserve;
 try {
 if (instance4 != null && flag != lastKeyguard_) { if (flag) { ProcessEvent(AppEventType.Sleep); } }
 instance4?.Lock();
 instance4?.Unlock(true);
 for (int j = 0; j < 3; j++) {
 try {
 switch (reserveSave) {
 case 1: instance.Save(); break;
 case 2: instance.SaveAll(); break;
 }
 } catch (Exception) { Count++; continue; }
 break;
 }
 return flag;
 } finally {Count++;}
}"""
CONTROL="""public bool Run(bool input){bool flag=input;SaveRecorder instance4=recorder;SaveRecorder instance=recorder;int reserveSave=Reserve;try{if(instance4!=null && ((flag!=lastKeyguard_)&flag)){ProcessEvent(AppEventType.Sleep);}if((object)instance4!=null){instance4.Lock();}if((object)instance4!=null){instance4.Unlock(true);}for(int j=0;j<3;j++){try{if(reserveSave==1){instance.Save();}else if(reserveSave==2){instance.SaveAll();}}catch(Exception){Count++;continue;}break;}return flag;}finally{Count++;}}"""
def pick(rows):return next(v for k,v in rows.items()if k.startswith('Fixture::Run('))
def main():
 work=ROOT/'build/round9/execute-residual-fixtures';manifest=json.loads((ROOT/'build/round9/cycles/cycle19/after/KairoLibrary/build-manifest.json').read_text(encoding='utf-8-sig'));rows=[]
 for label,before,control in [('protected-return',BASE,CONTROL),('effects-preserved',BASE.replace('bool flag = input;','bool flag = input; Count++;').replace('return flag;','Count += reserveSave; return flag;'),CONTROL.replace('bool flag=input;','bool flag=input;Count++;').replace('return flag;','Count+=reserveSave;return flag;'))]:
  after=transform_execute_residual(before);old,_=compile_fixture(work/(label+'-before'),HEADER+before+'}',manifest);want,_=compile_fixture(work/(label+'-control'),HEADER+control+'}',manifest);actual,_=compile_fixture(work/(label+'-candidate'),HEADER+after+'}',manifest)
  a,b,c=pick(old),pick(want),pick(actual);assert differences(a,b)[0],label;assert not differences(b,c)[0],(label,differences(b,c)[0]);assert a['locals']==c['locals'];calls=[i for i in c['instructions']if isinstance(i.get('operand'),dict)and any('::'+name+'``0('in i['operand'].get('method','')for name in ('Lock','Unlock'))];assert len(calls)==2 and all(i['opcode']=='callvirt'for i in calls)
  rows.append({'case':label,'full_body_equal':True,'locals_unchanged':True,'Lock_and_Unlock_callvirt':True,'retry_loop_and_protected_return_retained':True,'before':before,'after':after,'manual_control':control})
 negatives=[]
 for label,bad in [('float-flag',BASE.replace('bool flag = input;','float flag = 1f;')),('enum-selector',BASE.replace('int reserveSave = Reserve;','AppEventType reserveSave = AppEventType.Sleep;')),('unstable-receiver',BASE.replace('SaveRecorder instance4 = recorder;','var instance4 = recorder;')),('extra-outer-effect',BASE.replace('if (flag) { ProcessEvent(AppEventType.Sleep); } }','if (flag) { ProcessEvent(AppEventType.Sleep); } Count++; }')),('nested-case-break',BASE.replace('case 1: instance.Save(); break;','case 1: while(input){break;} instance.Save(); break;')),('missing-loop-exit',BASE.replace(' break;\n }\n return flag;',' }\n return flag;'))]:
  try:calculate_execute_residual(bad)
  except(AssertionError,ValueError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'historical-csc-execute-four-reason-fixtures','compiler_sha256':manifest['compiler_sha256'],'producer_sha256':sha(ROOT/'tools/round9_execute_residual_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':6,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Control body and EH/slots compared exactly; whole game method remains unknown until oracle. Eager boolean changes only pure builtin local/field loads, preserves null short circuit and calls.'}
 (ROOT/'reports/round9-execute-residual-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()
