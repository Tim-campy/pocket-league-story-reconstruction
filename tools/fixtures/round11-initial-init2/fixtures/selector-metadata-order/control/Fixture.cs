
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
if(type_==58){
 int selectedMatch=udat_.GetMatchInfoJoin();
 if(selectedMatch==0){LeagueData leagueData=apdat_.leagueData_[udat_.selectLeagueId_];Touch(leagueData.value);return Pop();}
 else if(selectedMatch==1){QuestMatchData questMatchData=apdat_.qmData_[udat_.selectQmId_];Touch(questMatchData.state_);return Pop();}
 Vector vector=new Vector();Vector vector2=new Vector();Touch(vector.Size()+vector2.Size());
}
 return true;}
}
