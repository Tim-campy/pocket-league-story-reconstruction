using System;
public class GameView {public static GameView GetInstance(){return new GameView();}}
public class AppData {public TeamData A=new TeamData(2),B=new TeamData(3);public static AppData GetInstance(){return new AppData();}}
public class UserData {public static int Mode,Reads;public static UserData GetInstance(){return new UserData();}public int GetMatchInfoJoin(){Reads++;return Mode;}}
public class TeamData {public int Value;public TeamData(int n){Value=n;}}
public class QuestMatchData {public int Value=9;}
public class Fixture {public static int Sum;static void Touch(int n){Sum+=n;}static int Rate(int frame){return frame+2;}
public static void Run(int frame){
 GameView instance=GameView.GetInstance();int originX=1;int originY=2;Touch(originX+originY+instance.GetHashCode()*0);
 AppData instance2=AppData.GetInstance();UserData instance3=UserData.GetInstance();
 int num4,num5,num,num2,num3;num5=num=num2=num3=num4=0;
 TeamData teamData=instance2.A;TeamData teamData2=instance2.B;
 int matchInfoJoin=instance3.GetMatchInfoJoin();
 if(matchInfoJoin==0){num=25;Touch(num);}else if(matchInfoJoin==1){{QuestMatchData questMatchData=new QuestMatchData();num=9;Touch(questMatchData.Value+num);num=32;Touch(num);}}
 num4=Rate(frame);num4=0;num5=4;num=61;Touch(num5+num+num4);
 int num6=0;if(frame==1){num6=1;}Touch(num6);
 for(int i=0;i<3;i++){Touch(i);}
 int num7=0;num2=0;if(frame==0){num2=57;num7=teamData.Value;}else{num2=183;num7=teamData2.Value;}Touch(num2+num7);
 int num8=frame%10/5;for(int j=0;j<3;j++){Touch(j+num8);}for(int k=0;k<3;k++){Touch(k+num8);}
 num5=4;num=174;
 for(int l=0;l<2;l++){
  num5=4;num=174;if(l==1){num5=129;}
  TeamData teamData3=null;
  if(l==0){teamData3=teamData;}else if(l==1){teamData3=teamData2;}
  Touch(teamData3.Value+num5+num);
  if(matchInfoJoin==0){int[] leagueData=new int[]{1,2};if(frame==0){num4=0;Touch(leagueData[0]+num4);}else{int num9=77;Touch(leagueData[1]+num9);}}
  else{int warPotential_=teamData3.Value;num4=0;Touch(warPotential_+num4);}
  int rankIndex=teamData3.Value;Touch(rankIndex+num5+num+num4);
 }
}
public static int SemanticChecks(){for(int mode=-1;mode<3;mode++){UserData.Mode=mode;for(int frame=-1;frame<4;frame++){UserData.Reads=0;Sum=0;Run(frame);if(UserData.Reads!=1||Sum<=0)return 1;}}return 0;}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}