public class Vec{public int Size(){return 0;}public void RemoveElementAt(int i){}}
public class Property{public string[] strs;}
public class AppData{public Property save_;public SubForm ExecEvent(int x,string y,int[] z){return null;}}
public class FormBase{public bool Push(FormBase x){return true;}}
public class SubForm:FormBase{public static SubForm MakeWait(int x){return null;}}
public class GameManager{public static int[] WAIT_CARD;public int time_card,time_team,time_pId,time_animePattern;public void ChangeState(int x){}}
public class UserData{public GameManager gm_;}
public static class GameUtil{public static int Random(int x){return x;}}
public class Fixture:FormBase{public Vec script_;public AppData apdat_;public UserData udat_;public int[] commands;
public int[] GetFirstScript(){return commands;}
public void Run(){while(script_.Size()>0){
 int[] command=GetFirstScript();
 int operation=command[0];
 if(operation==1){Push(SubForm.MakeWait(command[1]));script_.RemoveElementAt(0);}
 else if(operation==0){script_.RemoveElementAt(0);}
 else if(operation==2){script_.RemoveElementAt(0);continue;}
 else if(operation==3){script_.RemoveElementAt(0);}
 else if(operation==4){
  if(command[1]==0){
   udat_.gm_.ChangeState(10);
   udat_.gm_.time_card=command[2];
   udat_.gm_.time_team=command[3];
   udat_.gm_.time_pId=command[4];
   udat_.gm_.time_animePattern=GameUtil.Random(4);
  }
  command[1]++;
  if(command[1]>=GameManager.WAIT_CARD[GameManager.WAIT_CARD.Length-1]){
   udat_.gm_.ChangeState(0);
   script_.RemoveElementAt(0);
   continue;
  }
 }
 else if(operation==5){udat_.gm_.ChangeState(command[1]);script_.RemoveElementAt(0);continue;}
 break;
}}
}