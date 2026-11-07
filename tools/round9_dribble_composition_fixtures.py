"""Independent smcs whole six-hunk dribble control fixture."""
import json,sys,math
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round9_dribble_composition_source import transform_dribble
HEADER='''public class Position{public float z_;}public class Player{public int[]address_,mostEvaAddres_,screen_pos_;public Position pos_;}public class GameUtil{public static int Calls;public static bool ChOnLine(int[]a,int[]b,int c,int[]d){Calls++;return Calls>0;}}public class Fixture{public int Count,p_id_,team_id_;public int[]address_,screen_pos_,temp_afr_mostEvaSPos;public int[][]eva_pass_info_onEnemy,eva_pass_info;public float[][]eva_pass;public Position pos_;public Player Other;
'''
BEFORE='''public void Run(){int num7=100;for(int k=1;k<3;k++){Player player2=Other;bool flag2=false;
for(int l=0;l<3;l++){Player player3=Other;
if((address_[0]==player3.address_[0])&&(address_[1]==player3.address_[1])){continue;}
if(!(GameUtil.ChOnLine(screen_pos_,player2.screen_pos_,12,player3.screen_pos_))){continue;}
flag2=true;eva_pass_info_onEnemy[k][0]=1;break;
}
if(flag2&&eva_pass_info[k][0]!=1){eva_pass_info[k][0]=1;num7=num7*70/100;}}
for(int n=0;n<3;n++){if(n!=p_id_){eva_pass[n][0]=1f;break;}}
for(int num9=1;num9<3;num9++){Player player4=Other;
if((team_id_==0&&pos_.z_>player4.pos_.z_)||(team_id_==1&&pos_.z_<player4.pos_.z_)||(player4.address_[0]==player4.mostEvaAddres_[0]&&player4.address_[1]==player4.mostEvaAddres_[1])){continue;}
bool flag4=false;
for(int num12=0;num12<3;num12++){Player player5=Other;
if((address_[0]!=player5.address_[0]||address_[1]!=player5.address_[1])&&GameUtil.ChOnLine(screen_pos_,temp_afr_mostEvaSPos,12,player5.screen_pos_)){flag4=true;eva_pass_info_onEnemy[num9][1]=1;break;}}
if(flag4){Count++;}
}
Count+=num7;
}'''
CONTROL='''public void Run(){int num7=100;for(int k=1;k<3;k++){Player player2=Other;bool flag2=false;
for(int l=0;l<3;l++){Player player3=Other;
if((address_[0]==player3.address_[0])&&(address_[1]==player3.address_[1])){continue;}
if(GameUtil.ChOnLine(screen_pos_,player2.screen_pos_,12,player3.screen_pos_)){flag2=true;eva_pass_info_onEnemy[k][0]=1;break;}}
if(flag2){if(eva_pass_info[k][0]==1){}else{eva_pass_info[k][0]=1;num7=num7*70/100;}}}
for(int n=0;n<3;n++){if(n==p_id_){continue;}eva_pass[n][0]=1f;break;}
for(int num9=1;num9<3;num9++){Player player4=Other;
if(team_id_==0){if(pos_.z_>player4.pos_.z_){continue;}}
if(team_id_==1){if(pos_.z_<player4.pos_.z_){continue;}}
if(player4.address_[0]==player4.mostEvaAddres_[0]&&player4.address_[1]==player4.mostEvaAddres_[1]){continue;}
bool flag4=false;
for(int num12=0;num12<3;num12++){Player player5=Other;
if(address_[0]==player5.address_[0]&&address_[1]==player5.address_[1]){continue;}
if(GameUtil.ChOnLine(screen_pos_,temp_afr_mostEvaSPos,12,player5.screen_pos_)){flag4=true;eva_pass_info_onEnemy[num9][1]=1;break;}}
if(flag4){Count++;}
}
Count+=num7;
}'''
def main():
 compiler.HERE=ROOT/'build/round9/dribble-composition-fixtures';compiler.HERE.mkdir(parents=True,exist_ok=True)
 after=transform_dribble(BEFORE);old=compiler.compile_toy(HEADER+BEFORE+'}','before')['Run'];want=compiler.compile_toy(HEADER+CONTROL+'}','control')['Run'];actual=compiler.compile_toy(HEADER+after+'}','candidate')['Run'];assert differences(old,want)[0];assert not differences(want,actual)[0],differences(want,actual)[0];assert old['locals']==actual['locals']
 negatives=[]
 for label,bad in [('line-guard-effect',BEFORE.replace('{continue;}\nflag2=true;','{Count++;continue;}\nflag2=true;')),('guard-empty-value-other',BEFORE.replace('eva_pass_info[k][0]!=1','eva_pass_info[k][0]!=2')),('wrong-loop-owner',BEFORE.replace('for(int n=0;n<3;n++)','while(Count<3)')),('wrong-coordinate-index',BEFORE.replace('player5.address_[1]','player5.address_[2]')),('float-other-relation',BEFORE.replace('pos_.z_>player4.pos_.z_','pos_.z_>=player4.pos_.z_')),('address-guard-effect',BEFORE.replace('{flag4=true;eva_pass_info_onEnemy[num9][1]=1;break;}','{Count++;flag4=true;eva_pass_info_onEnemy[num9][1]=1;break;}'))]:
  try:transform_dribble(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 # Semantic truth table for positive ordered comparisons: no >= or <= replacement.
 float_cases=[]
 for a,b in [(0.,0.),(-0.,0.),(math.nan,0.),(0.,math.nan),(math.inf,0.),(-math.inf,0.),(1.,2.),(2.,1.)]:
  for team in [0,1,2]:
   for same in [False,True]:
    old_guard=(team==0 and a>b)or(team==1 and a<b)or same
    new_guard=False
    if team==0:
     if a>b:new_guard=True
    if not new_guard and team==1:
     if a<b:new_guard=True
    if not new_guard and same:new_guard=True
    assert old_guard==new_guard
    float_cases.append({'a':str(a),'b':str(b),'team':team,'address_equal':same,'outcome':old_guard})
 report={'operation':'smcs-whole-six-hunk-dribble-composition-fixture','compiler_sha256':sha(compiler.SMCS),'producer_sha256':sha(ROOT/'tools/round9_dribble_composition_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':3,'positive_full_bodies':1,'negative_controls':len(negatives),'negatives':negatives,'fixtures':[{'before':BEFORE,'after':after,'manual_control':CONTROL,'full_body_equal':True,'before_body_different':True,'locals_unchanged':True,'unordered_opcodes':[i['opcode']for i in actual['instructions']if '.un'in i['opcode']]}],'float_cases':float_cases,'limitations':'One joint calculated game target. All six hunk shapes in independent toy; does not replay isolated C17 candidate. Runtime equivalence or role mapping never promotes a method.'}
 (ROOT/'reports/round9-dribble-composition-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k not in ('fixtures','float_cases')}))
if __name__=='__main__':main()

