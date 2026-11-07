"""Independent smcs loop targets, section joins and preserved child blocks."""
import json,sys,re
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round9_loop_target_source import transform_execute_script,transform_coat,transform_zsort
EXEC_HEADER='''public class TradeData{public int Value;}public class Fixture{public int Count;public TradeData[]tradeData_;void SubMoney(int a,int b){Count-=a+b;}void AddMoney(int a,int b){Count+=a+b;}void AddPopupMessage(string text){Count+=text.Length;}'''
EXEC='''public void Run(int[] scr){int num=scr[0];switch(num){case 0:if(scr[1]<0){SubMoney(-scr[1],4);break;}AddMoney(scr[1],4);AddPopupMessage("money");break;case 17:{if(scr[1]==-1){break;}TradeData tradeData2 = tradeData_[scr[1]];Count+=tradeData2.Value;break;}default:Count++;break;}}'''
EXEC_CONTROL='''public void Run(int[] scr){int num=scr[0];switch(num){case 0:if(scr[1]<0){SubMoney(-scr[1],4);}else{AddMoney(scr[1],4);AddPopupMessage("money");}break;case 17:{if(scr[1]!=-1){TradeData tradeData2=tradeData_[scr[1]];Count+=tradeData2.Value;}break;}default:Count++;break;}}'''
COAT_HEADER='''public class GameUtil{public static int Calls;public static bool ChRectHitRect(int a,int b){Calls++;return a>b;}}public class Paint{public int Calls;public void InsertGameObject(int a,int b){Calls+=a+b;}}public class Fixture{public int Count;public Paint apdat_;void Draw_coatLine(int a,int b){Count+=a+b;}'''
COAT='''public void Run(){for(int num9=0;num9<2;num9++){for(int j=0;j<3;j++){if(Count==1){if (!(GameUtil.ChRectHitRect(j,num9))){}else{apdat_.InsertGameObject(j,num9);}continue;}if(Count==2){if(GameUtil.ChRectHitRect(j+1,num9)){Draw_coatLine(j,num9);}continue;}if(GameUtil.ChRectHitRect(j+2,num9)){int width=j+num9;Count+=width;apdat_.InsertGameObject(j,width);}}}}'''
COAT_CONTROL='''public void Run(){for(int num9=0;num9<2;num9++){for(int j=0;j<3;j++){if(Count==1){if(!(GameUtil.ChRectHitRect(j,num9))){continue;}{apdat_.InsertGameObject(j,num9);}continue;}if(Count==2){if(!(GameUtil.ChRectHitRect(j+1,num9))){continue;}{Draw_coatLine(j,num9);}continue;}if(!(GameUtil.ChRectHitRect(j+2,num9))){continue;}{int width=j+num9;Count+=width;apdat_.InsertGameObject(j,width);}}}}'''
ZSORT_HEADER='''public class BitUtil{public static bool Check(int flags,int mask){return(flags&mask)!=0;}}public class Fixture{public int Count;'''
def zsource(control=False):
 cases=[]
 for number in [1,2,3,8,9,4,5,6,7,10]:
  payload='{int q=Count+'+str(number)+';Count+=q;}'
  if number==1:
   code='if (!(BitUtil.Check(typeBits, 1 << num2)))'+('{continue;}'+payload if control else '{}else'+payload)
  elif number==9:
   code='if (!BitUtil.Check(typeBits, 1 << num2)){break;}Count+=9;'
  else:
   code=('if (!BitUtil.Check(typeBits, 1 << num2)){continue;}'+payload if control else 'if (BitUtil.Check(typeBits, 1 << num2))'+payload)
  cases.append('case '+str(number)+':'+code+'break;')
 return 'public void Run(int typeBits){for(int j=0;j<12;j++){int num2=j;switch(num2){'+''.join(cases)+'}}}'
ZSORT=zsource();ZSORT_CONTROL=zsource(True)
def main():
 compiler.HERE=ROOT/'build/round9/loop-target-fixtures';compiler.HERE.mkdir(parents=True,exist_ok=True);rows=[]
 for label,before,expected,header,transform in [('execute-section-local-joins',EXEC,EXEC_CONTROL,EXEC_HEADER,transform_execute_script),('coat-explicit-loop-continues',COAT,COAT_CONTROL,COAT_HEADER,transform_coat),('zsort-direct-loop-targets',ZSORT,ZSORT_CONTROL,ZSORT_HEADER,transform_zsort)]:
  after=transform(before);old=compiler.compile_toy(header+before+'}',label+'-before')['Run'];want=compiler.compile_toy(header+expected+'}',label+'-control')['Run'];actual=compiler.compile_toy(header+after+'}',label+'-candidate')['Run'];assert differences(old,want)[0],label;assert not differences(want,actual)[0],(label,differences(want,actual)[0]);assert old['locals']==actual['locals'];rows.append({'case':label,'full_body_equal':True,'before_body_different':True,'local_signature_unchanged':True,'before':before,'after':after,'manual_control':expected})
 negatives=[]
 tests=[('execute-money-early-guard-effects',EXEC.replace('SubMoney(-scr[1],4);break;','SubMoney(-scr[1],4);Count++;break;'),transform_execute_script),('execute-money-false-extra-effect',EXEC.replace('AddMoney(scr[1],4);','AddMoney(scr[1],4);Count++;'),transform_execute_script),('execute-trade-guard-effect',EXEC.replace('if(scr[1]==-1){break;}','if(scr[1]==-1){Count++;break;}'),transform_execute_script),('coat-owner-other-variable',re.sub(r'\bj\b','k',COAT),transform_coat),('coat-missing-owned-tail-continue',COAT.replace('InsertGameObject(j,num9);}continue;','InsertGameObject(j,num9);}'),transform_coat),('coat-extra-positive-guard',COAT.replace('if(Count==2)','if(GameUtil.ChRectHitRect(0,0)){Count++;}if(Count==2)'),transform_coat),('zsort-first-guard-not-empty',ZSORT.replace('))){}else', '))){Count++;}else',1),transform_zsort),('zsort-extra-after-payload',ZSORT.replace('Count+=q;}break;case 3:','Count+=q;}Count++;break;case 3:'),transform_zsort),('zsort-case9-must-remain-distinct',ZSORT.replace('if (!BitUtil.Check(typeBits, 1 << num2)){break;}','if (BitUtil.Check(typeBits, 1 << num2)){Count++;}'),transform_zsort),('zsort-loop-sibling-after-switch',ZSORT[:-3]+'}Count++;}}',transform_zsort)]
 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError,KeyError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'smcs-intermediate-section-joins-versus-direct-loop-targets-own-fixtures','compiler_sha256':sha(compiler.SMCS),'producer_sha256':sha(ROOT/'tools/round9_loop_target_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':9,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Exact manual source controls preserve standalone declaration blocks. Loop continue never used when a sibling remains after switch. Case9 distinct break retained. Compiled body/signature/header/EH of game remain sole judge.'}
 (ROOT/'reports/round9-loop-target-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

