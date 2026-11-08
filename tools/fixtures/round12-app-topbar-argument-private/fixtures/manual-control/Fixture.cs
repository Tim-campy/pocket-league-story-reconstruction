using kairo.unity.ui;
namespace kairo.unity.ui{public class Graphics{public static int GetColorOfRGB(int a,int b,int c){return a;}public void SetColor(int c){}public void DrawString(string s,float x,float y,int align){}}}
public class TeamData{public string name_;}
public class AppData{public TeamData[] tData_;}
public class GameManager{public int[] teamId_;}
public class UserData{public GameManager gm_;public int phase_;}
public class Fixture{public AppData apdat_;public UserData udat_;public int state_;
public int GetDrawTBAROff(int x){return x;}
public void UpdateOffscreenTopBar(Graphics g,bool flag){}
public void Run(Graphics g){TeamData first=apdat_.tData_[udat_.gm_.teamId_[0]];
TeamData second=apdat_.tData_[udat_.gm_.teamId_[1]];
int top=4;
int left=GetDrawTBAROff(2);
int right=GetDrawTBAROff(4);
Graphics cached=g;
if(state_==0||state_==1||state_==7||state_==8){
 cached.SetColor(Graphics.GetColorOfRGB(214,241,255));
 cached.DrawString(first.name_,left,top,2);
 cached.DrawString(second.name_,right,top,2);
}else if(udat_.phase_==14||udat_.phase_==15){
 cached.SetColor(Graphics.GetColorOfRGB(128,128,128));
 cached.DrawString(first.name_,left,top,2);
 cached.SetColor(Graphics.GetColorOfRGB(214,241,255));
 cached.DrawString(second.name_,right,top,2);
}else{
 cached.SetColor(Graphics.GetColorOfRGB(214,241,255));
 cached.DrawString(first.name_,left,top,2);
 cached.SetColor(Graphics.GetColorOfRGB(128,128,128));
 cached.DrawString(second.name_,right,top,2);
}
UpdateOffscreenTopBar(g,false);}
}