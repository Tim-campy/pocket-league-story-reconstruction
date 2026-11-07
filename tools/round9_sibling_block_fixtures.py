"""Own smcs sibling-block joins and integer ternary orientation controls."""
import json,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1];sys.path.insert(0,str(ROOT/'tools'))
import test_round9_scoped_exit as compiler
from compare_kairo import differences,sha
from round9_sibling_block_source import transform_title,transform_team
TITLE_HEADER='''using System.Threading;public class Config{public static bool TRIAL_VERSION;}public class Language{public static int Calls;public static bool English(){Calls++;return Calls>0;}}public class App{public int Calls;public void DrawSeb(int a,int b){Calls+=a+b;}}public class Fixture{public int Count,selMenu_;public App apdat_;
'''
TITLE='''public void Run(){ThreadStart action=delegate{Count++;};action();for(int i=0;i<3;i++){if(i==selMenu_){Count++;if ((Config.TRIAL_VERSION) && (Language.English())){return;}apdat_.DrawSeb(1,2);apdat_.DrawSeb(3,4);}if(i==selMenu_){Count+=2;}for(int j=0;j<2;j++){apdat_.DrawSeb((j != 0) ? 6 : 11,0);apdat_.DrawSeb((j != 0) ? 2 : 6,0);apdat_.DrawSeb((j != 0) ? 4 : 8,0);}}}'''
TITLE_CONTROL='''public void Run(){ThreadStart action=delegate{Count++;};action();for(int i=0;i<3;i++){if(i==selMenu_){Count++;if((Config.TRIAL_VERSION)&&(Language.English())){}else{apdat_.DrawSeb(1,2);apdat_.DrawSeb(3,4);}}if(i==selMenu_){Count+=2;}for(int j=0;j<2;j++){apdat_.DrawSeb((j == 0) ? 11 : 6,0);apdat_.DrawSeb((j == 0) ? 6 : 2,0);apdat_.DrawSeb((j == 0) ? 8 : 4,0);}}}'''
TEAM_HEADER='''public class Game{public int state_;}public class World{public Game gm_;}public class Fixture{public int Count;public int[]ch_ceva;public World udat_;public object coatEva_new_deffence;public const int CEVA_PASSER_MP2_DIV=2;void Cal_evaCoatMulti9block(object arr,int x,int y,int d){Count+=x+y+d;}
'''
TEAM='''public void Run(){if(ch_ceva[7]==1){int cx=Count;int cy=2;if(udat_.gm_.state_==5){return;}Cal_evaCoatMulti9block(coatEva_new_deffence,cx,cy,CEVA_PASSER_MP2_DIV);}if(ch_ceva[8]==1){Count+=8;}}'''
TEAM_CONTROL='''public void Run(){if(ch_ceva[7]==1){int cx=Count;int cy=2;if(udat_.gm_.state_==5){}else{Cal_evaCoatMulti9block(coatEva_new_deffence,cx,cy,CEVA_PASSER_MP2_DIV);}}if(ch_ceva[8]==1){Count+=8;}}'''
def main():
 compiler.HERE=ROOT/'build/round9/sibling-block-fixtures';compiler.HERE.mkdir(parents=True,exist_ok=True);rows=[]
 for label,before,expected,header,transform in [('highlight-sibling-with-three-ternaries',TITLE,TITLE_CONTROL,TITLE_HEADER,transform_title),('passer-seven-to-eight-sibling',TEAM,TEAM_CONTROL,TEAM_HEADER,transform_team)]:
  after=transform(before);old=compiler.compile_toy(header+before+'}',label+'-before');want=compiler.compile_toy(header+expected+'}',label+'-control');actual=compiler.compile_toy(header+after+'}',label+'-candidate');assert differences(old['Run'],want['Run'])[0];assert set(want)==set(actual)
  for key in want:assert not differences(want[key],actual[key])[0],(label,key,differences(want[key],actual[key])[0])
  assert old['Run']['locals']==actual['Run']['locals'];rows.append({'case':label,'full_body_equal':True,'all_fixture_method_bodies_equal':sorted(actual),'before_body_different':True,'locals_unchanged':True,'before':before,'after':after,'manual_control':expected})
 negatives=[]
 for label,bad,transform in [('highlight-for-initial-other',TITLE.replace('int i=0','int i=1'),transform_title),('highlight-guard-effect',TITLE.replace('{return;}','{Count++;return;}'),transform_title),('highlight-next-sibling-other',TITLE.replace('}if(i==selMenu_)','}if(i!=selMenu_)'),transform_title),('highlight-tail-declaration',TITLE.replace('apdat_.DrawSeb(1,2);','int local=Count;apdat_.DrawSeb(local,2);'),transform_title),('integer-ternary-values-other',TITLE.replace('? 6 : 11','? 7 : 11'),transform_title),('passer-call-arguments-other',TEAM.replace('cx,cy,CEVA_PASSER_MP2_DIV','cy,cx,CEVA_PASSER_MP2_DIV'),transform_team),('passer-next-section-other',TEAM.replace('ch_ceva[8]','ch_ceva[9]'),transform_team),('passer-tail-extra-effect',TEAM.replace('Cal_evaCoatMulti9block(coatEva_new_deffence','Count++;Cal_evaCoatMulti9block(coatEva_new_deffence'),transform_team)]:
  try:transform(bad)
  except(ValueError,AssertionError):negatives.append(label)
  else:raise AssertionError(label)
 report={'operation':'smcs-sibling-block-exit-own-fixtures','compiler_sha256':sha(compiler.SMCS),'producer_sha256':sha(ROOT/'tools/round9_sibling_block_source.py'),'driver_sha256':sha(Path(__file__)),'game_compilations':0,'toy_compilations':6,'positive_full_bodies':len(rows),'negative_controls':len(negatives),'negatives':negatives,'fixtures':rows,'limitations':'Named scopes and sibling nodes proven lexically; no break/continue/return invented for method or loop. Compiler creates delegate helper itself; every Fixture method compared, no generated class manually written. Game original signatures/body mandatory.'}
 (ROOT/'reports/round9-sibling-block-fixtures.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8');print(json.dumps({k:v for k,v in report.items()if k!='fixtures'}))
if __name__=='__main__':main()

