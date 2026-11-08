using kairo.unity.ui;
namespace kairo.unity.ui{public class Graphics{public static int GetColorOfRGB(int a,int b,int c){return a;}public void SetColor(int c){}public void DrawString(string s,float x,float y,int align){}}}
public class TeamData{public string name_;}
public class AppData{public TeamData[] tData_;}
public class GameManager{public int[] teamId_;}
public class UserData{public GameManager gm_;public int phase_;}
public class Fixture{public AppData apdat_;public UserData udat_;public int state_;
public int GetDrawTBAROff(int x){return x;}
public void UpdateOffscreenTopBar(Graphics g,bool flag){}
public void Run(Graphics g){
			TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[0]];
			TeamData teamData2 = apdat_.tData_[udat_.gm_.teamId_[1]];
			int num = 4;
			int drawTBAROff = GetDrawTBAROff(2);
			int drawTBAROff2 = GetDrawTBAROff(4);
			global::kairo.unity.ui.Graphics gAlias = g;
if (state_ == 0 || state_ == 1 || state_ == 7 || state_ == 8)
			{
				gAlias.SetColor(Graphics.GetColorOfRGB(214, 241, 255));
				gAlias.DrawString(teamData.name_, drawTBAROff, num, 2);
				gAlias.DrawString(teamData2.name_, drawTBAROff2, num, 2);
			}
			else if (udat_.phase_ == 14 || udat_.phase_ == 15)
			{
				gAlias.SetColor(Graphics.GetColorOfRGB(128, 128, 128));
				gAlias.DrawString(teamData.name_, drawTBAROff, num, 2);
				gAlias.SetColor(Graphics.GetColorOfRGB(214, 241, 255));
				gAlias.DrawString(teamData2.name_, drawTBAROff2, num, 2);
			}
			else
			{
				gAlias.SetColor(Graphics.GetColorOfRGB(214, 241, 255));
				gAlias.DrawString(teamData.name_, drawTBAROff, num, 2);
				gAlias.SetColor(Graphics.GetColorOfRGB(128, 128, 128));
				gAlias.DrawString(teamData2.name_, drawTBAROff2, num, 2);
			}
			UpdateOffscreenTopBar(gAlias, false);
		}
}