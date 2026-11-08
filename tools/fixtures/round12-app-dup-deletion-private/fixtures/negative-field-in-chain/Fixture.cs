using System;
public class TeamData {public int Value=3;}
public class LeagueData {public int Value=4;}
public class QuestMatchData {public int Value=5;}
public class User {public static int Mode,Reads;public int GetMatchInfoJoin(){Reads++;return Mode;}}
public class GameView {public static int SideReads,HeightReads;public static GameView GetInstance(){return new GameView();}public int GetSideSoftWidth(){SideReads++;return 2;}public int GetGameHeight(){HeightReads++;return 240;}public int GetGameWidth(){return 240;}}
public class TPlayer {public int Value=2;}
public class Players {public int Count=3;public int Size(){return Count;}public object ElementAt(int n){return new TPlayer();}}
public class AppData {public static int ClampMax(int a,int b){return Math.Min(a,b);}}
public class Fixture {
 User udat_=new User();public Players useTPlayers_=new Players();public bool Enabled=true;public int draw_cnt;static int[] camera_pos=new int[2];public static int Sum;
 static void Touch(int n){Sum+=n;}static int Rate(int n){return n;}
public void RunMenu(){
 int num3, num;
num3 = num = 0;
int num7, num2;
num7 = num2 = 0;TeamData teamData=new TeamData();GameView instance=GameView.GetInstance();int matchInfoJoin=udat_.GetMatchInfoJoin();
 if(matchInfoJoin!=2){
  num3 = instance.GetSideSoftWidth() + 1;num=instance.GetGameHeight()-43-25;Touch(num3+num);
  int num4=0;
  if (matchInfoJoin == 0) {{LeagueData leagueData=new LeagueData();Touch(leagueData.Value+num3);int[] leagueData2=new int[]{1,2};Touch(leagueData2[1]);num4=leagueData.Value-2;}} else if (matchInfoJoin == 1) {{QuestMatchData questMatchData=new QuestMatchData();Touch(questMatchData.Value+num3);num4=questMatchData.Value-2;}}
  int num5=(8+(3+num4)/4)%12+1;int num6=(3+num4)%4+1;Touch(num5+num6);
 }
 num7 = instance.GetGameWidth() - 123 - instance.GetSideSoftWidth();num2=instance.GetGameHeight()-106;Touch(num7+num2+teamData.Value);
 int rankIndex=2;Touch(rankIndex);
 for(int i=0;i<3;i++){num7=240-i;num2=num+14*i;int fanEva=i+2;int fanDispParam=fanEva+3;
  if (i == 0) {Touch(fanDispParam+num7+num2);} else if (i == 1) {{int num8=52;int num9=0;num9=Rate(fanDispParam+num8-2);Touch(num9+num7+num2);}} else if (i == 2) {Touch(fanDispParam+num7+num2+1);}
 }
}
public void RunTenant(){TPlayer tPlayer=(TPlayer)useTPlayers_.ElementAt(0);Touch(tPlayer.Value);
 if(Enabled){camera_pos[1]+=40;int x, num;
x = num = draw_cnt = 0;int num2=AppData.ClampMax(4,useTPlayers_.Size()-1);
  for(int num3=num2;num3>=1;num3--){x = camera_pos[0] + (num3 - 1) * 8;num=camera_pos[1]+(num3-1)*-4;TPlayer tPlayer2=(TPlayer)useTPlayers_.ElementAt(num3);int layer=tPlayer2.Value;Touch(x+num+layer);draw_cnt++;if(draw_cnt>=4){break;}}
 }
}
public static int SemanticChecks(){Fixture f=new Fixture();for(int mode=-1;mode<3;mode++){User.Mode=mode;User.Reads=0;GameView.SideReads=0;GameView.HeightReads=0;Sum=0;f.RunMenu();if(User.Reads!=1||GameView.SideReads!=(mode==2?1:2)||GameView.HeightReads!=(mode==2?1:2)||Sum<=0)return 1;}
for(int size=1;size<7;size++){camera_pos=new int[2];f.useTPlayers_.Count=size;f.Enabled=true;f.RunTenant();if(f.draw_cnt!=Math.Min(4,size-1))return 2;}f.Enabled=false;f.draw_cnt=9;f.RunTenant();if(f.draw_cnt!=9)return 3;return 0;}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}