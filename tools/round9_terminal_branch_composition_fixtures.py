"""Own smcs controls for the two calculated terminal branch compositions."""
import json,sys,math
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round9_terminal_branch_composition_source import transform_subform,transform_ball
HEADER_SUB='''public class TenantData {public const int TENANT_ID_STADIUM=1,TENANT_ID_TRAININGAREA=2,TENANT_ID_TRAININGAREA_SPRED=3;public int id_,state_;public int[] GetLvUpAddParam(){return new int[1];}}public class AppData{public TenantData[] tenantData_;}public class List{public int Count;public void AddElement(TenantData d){Count++;}}public class Fixture{public int type_,tenantDataId_,Count;public AppData apdat_;public List ary1;public bool Init2(){Count++;return false;}
'''
FILTER='(type_ != 61 || (tenantData.id_ != TenantData.TENANT_ID_STADIUM && tenantData.id_ != TenantData.TENANT_ID_TRAININGAREA && tenantData.id_ != TenantData.TENANT_ID_TRAININGAREA_SPRED)) && tenantData.state_ != 0'
SUB='''public bool Run(){if(type_==61){for(int i=0;i<apdat_.tenantData_.Length;i++){TenantData tenantData=apdat_.tenantData_[i];if ('''+FILTER+'''){ary1.AddElement(tenantData);}}}else if (type_ != 62 && type_ != 63){if (type_ != 64){return Init2();}TenantData tenantData3 = apdat_.tenantData_[tenantDataId_]; int[] lvUpAddParam = tenantData3.GetLvUpAddParam();Count+=lvUpAddParam[0];}return true;}'''
SUB_CONTROL='''public bool Run(){if(type_==61){for(int i=0;i<apdat_.tenantData_.Length;i++){TenantData tenantData=apdat_.tenantData_[i];if(type_==61){if(tenantData.id_==TenantData.TENANT_ID_STADIUM){continue;}if(tenantData.id_==TenantData.TENANT_ID_TRAININGAREA){continue;}if(tenantData.id_==TenantData.TENANT_ID_TRAININGAREA_SPRED){continue;}}if(tenantData.state_!=0){ary1.AddElement(tenantData);}}}else if(type_==62){}else if(type_==63){}else if(type_==64){TenantData tenantData3=apdat_.tenantData_[tenantDataId_];int[] lvUpAddParam=tenantData3.GetLvUpAddParam();Count+=lvUpAddParam[0];}else{return Init2();}return true;}'''
HEADER_BALL='''public class Position{public float z_;}public class SoccerForm{public static int DEBUG_BALLTEST_MODE,DEBUG_WALLREFRECT;}public class Game{public int State;public void ChangeState(int n){State=n;}}public class User{public Game gm_;}public class Fixture{public int Count;public Position lineOutPos_;public User udat_;
'''
BALL='''public bool Run(bool flag13,bool flag8){int num3=0;if (flag13 && !flag8 && SoccerForm.DEBUG_BALLTEST_MODE != 1){if (SoccerForm.DEBUG_WALLREFRECT == 1){Count++;}else if ((!(lineOutPos_.z_ < -900f)) && (!(lineOutPos_.z_ > 900f))){udat_.gm_.ChangeState(3);}else{if (!(lineOutPos_.z_ > 900f)){}else{num3 = 1;}Count+=num3;}}Count+=num3;return false;}'''
BALL_CONTROL='''public bool Run(bool flag13,bool flag8){int num3=0;if(flag13&&!flag8){if(SoccerForm.DEBUG_BALLTEST_MODE==1){}else if(SoccerForm.DEBUG_WALLREFRECT==1){Count++;}else if(lineOutPos_.z_<-900f||lineOutPos_.z_>900f){if(lineOutPos_.z_>900f){num3=1;}Count+=num3;}else{udat_.gm_.ChangeState(3);}}Count+=num3;return false;}'''
def main():
 compiler.HERE=ROOT/'build/round9/terminal-branch-composition-fixtures';compiler.HERE.mkdir(parents=True,exist_ok=True);rows=[]
 for label,before,expected,header,transform in [('tenant-continue-and-empty-chain',SUB,SUB_CONTROL,HEADER_SUB,transform_subform),('empty-debug-and-float-branches',BALL,BALL_CONTROL,HEADER_BALL,transform_ball)]:
  after=transform(before);control=compiler.compile_toy(header+expected+'}',label+'-control')['Run'];actual=compiler.compile_toy(header+after+'}',label+'-candidate')['Run'];assert not differences(control,actual)[0],(label,differences(control,actual)[0]);rows.append({'case':label,'full_body_equal':True,'locals_equal':control['locals']==actual['locals'],'before':before,'after':after,'manual_control':expected,'unordered_opcodes':[i['opcode']for i in actual['instructions']if '.un'in i['opcode']]})
 negative=[]
 tests=[('tenant-owner-is-while',SUB.replace('for(int i=0;i<apdat_.tenantData_.Length;i++)','while(Count<3)'),transform_subform),('tenant-extra-tail',SUB.replace('ary1.AddElement(tenantData);}}}','ary1.AddElement(tenantData);}Count++;}}'),transform_subform),('tenant-guard-effects',SUB.replace('return Init2();','Count++;return Init2();'),transform_subform),('tenant-final-return-other',SUB.replace('return true;','return false;'),transform_subform),('float-positive-other',BALL.replace('num3 = 1;','Count++;'),transform_ball),('debug-extra-tail',BALL.replace('Count+=num3;}}Count+=num3;','Count+=num3;}Count++;}Count+=num3;'),transform_ball),('float-comparison-other',BALL.replace('lineOutPos_.z_ > 900f','lineOutPos_.z_ >= 900f'),transform_ball)]
 for label,bad,transform in tests:
  try:transform(bad)
  except(ValueError,AssertionError):negative.append(label)
  else:raise AssertionError(label)
 cases=[]
 for z in [-math.inf,-901.,-900.,-0.,0.,900.,901.,math.inf,math.nan]:
  old_inside=(not(z<-900.))and(not(z>900.));new_outside=(z < -900.) or (z > 900.);assert old_inside==(not new_outside)
  old_assign=not(not(z>900.));new_assign=z>900.;assert old_assign==new_assign;cases.append({'z':str(z),'inside':old_inside,'assign_one':new_assign})
 report={'operation':'smcs-two-terminal-branch-composition-fixtures','compiler_sha256':sha(compiler.SMCS),'producer_sha256':sha(ROOT/'tools/round9_terminal_branch_composition_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':4,'positive_full_bodies':len(rows),'negative_controls':len(negative),'fixtures':rows,'negatives':negative,'float_boundary_and_NaN_cases':cases,'limitations':'Two exact source-control compositions, no emitter model changed. Nonfloating tenant fields stable; positive float operators preserved and ordered/unordered opcodes retained. Game bodies/signatures unknown before oracle.'}
 (ROOT/'reports/round9-terminal-branch-composition-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()
