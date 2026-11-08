
public class PlayerData{public int id_;}
public class LeagueData{public int value;}
public class QuestMatchData{public int state_;}
public class Bonus{public int flag_;}
public class Vector{public int Size(){return 3;}public object ElementAt(int n){return new PlayerData();}}
public class User{public Vector memberPDataist_=new Vector();public int selectLeagueId_,selectQmId_;public int GetMatchInfoJoin(){return selectLeagueId_;}}
public class App{public LeagueData[] leagueData_=new LeagueData[3];public QuestMatchData[] qmData_=new QuestMatchData[3];public Bonus[] fanBonus_=new Bonus[3];}
public static class TrainingData{public static int[] TRA_PLAYER=new int[3];}
public static class BitUtil{public static bool Check(int x,int y){return (x&y)!=0;}}
public class Fixture{
 int type_,value1_,num3,num7,num13,num18,num28;User udat_=new User();App apdat_=new App();QuestMatchData questMatchData3=new QuestMatchData();
 public void Touch(int value){type_=value;}public bool Pop(){return type_==0;}
 public bool Run(){
for(int i=0;i<TrainingData.TRA_PLAYER.Length;i++){
 if(TrainingData.TRA_PLAYER[i]!=-1){continue;}
 bool flag=false;
 for(int j=0;j<udat_.memberPDataist_.Size();j++){
  PlayerData playerData=(PlayerData)udat_.memberPDataist_.ElementAt(j);
  if(TrainingData.TRA_PLAYER[i]==playerData.id_){flag=true;break;}
 }
 if(!flag){TrainingData.TRA_PLAYER[i]=-1;}else{}
}
 return true;}
}
