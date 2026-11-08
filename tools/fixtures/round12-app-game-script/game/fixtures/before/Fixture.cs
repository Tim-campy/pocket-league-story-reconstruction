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
public void Run(){
			while (script_.Size() > 0)
			{
				int[] firstScript = GetFirstScript();
				switch (firstScript[0])
				{
				case 1:
					Push(SubForm.MakeWait(firstScript[1]));
					script_.RemoveElementAt(0);
					return;
				case 0:
					apdat_.ExecEvent(firstScript[1], apdat_.save_.strs[0], null);
					script_.RemoveElementAt(0);
					return;
				case 2:
					break;
				case 3:
					script_.RemoveElementAt(0);
					return;
				default:
					return;
				}
				script_.RemoveElementAt(0);
			}
		}
}