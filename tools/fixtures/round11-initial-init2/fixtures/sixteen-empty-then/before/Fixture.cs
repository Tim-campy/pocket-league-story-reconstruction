
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
if (TrainingData.TRA_PLAYER[num3] != -1 && value1_ != num3){Touch(1);}
if (TrainingData.TRA_PLAYER[num7] != -1){Touch(2);}
if(type_==51){Touch(3);}
else if (type_ != 52){
 if(type_==53){Touch(4);}
 else if (type_ != 54){
  if(type_==55){Touch(5);}
  else if (type_ != 56 && type_ != 57){
   if(type_==58){Touch(6);}
   else if (type_ != 66){
    if(type_==67){Touch(7);}
    else if (type_ != 69 && type_ != 71 && type_ != 72 && type_ != 74 && type_ == 77){Touch(8);}
   }
  }
 }
}
if (num13 != 0){Touch(9);}
if (num18 != 0){Touch(10);}
if (questMatchData3.state_ != 0){Touch(11);}
if (BitUtil.Check(apdat_.fanBonus_[num28].flag_, 4)){Touch(12);}
 return true;}
}
