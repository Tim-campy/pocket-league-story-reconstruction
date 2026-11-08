using data; using surface;
namespace data { public class TeamData { public int uniform_; } }
namespace surface { public class GameView { public static GameView GetInstance(){return new GameView();} public int GetGameWidth(){return 320;} } }
public class GM { public int[] teamId_; }
public class UserData { public GM gm_; }
public class AppData { public TeamData[] tData_;public static int ClampMin(int v,int min){return v<min?min:v;} }
public class Graphics { }
public class Fixture { public AppData apdat_;public UserData udat_;int sink;
public int GetTopBarOffN(){return 3;} public void Use(int x,int y){sink=x+y;}
public void Run(Graphics g,bool all_update){if(all_update){Use(1,2);}
global::data.TeamData teamData2;
global::surface.GameView instance;
int topBarOffN;
int num;
TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[0]];
teamData2 = apdat_.tData_[udat_.gm_.teamId_[1]];
instance = GameView.GetInstance();
topBarOffN = GetTopBarOffN();
num = AppData.ClampMin((instance.GetGameWidth() - (240 + topBarOffN * 2)) / 2, 0);
int num2 = num;Use(num,1);num+=20;
int num3 = num;Use(num,2);num+=20;
int num4 = num;Use(num,3);num+=20;
int num5 = num;Use(num,4);num+=20;
Use(num2,teamData.uniform_);Use(num5,teamData2.uniform_);
Use(num3+topBarOffN,1);Use(num4+topBarOffN,2);
int num6 = 0;
if(all_update){
 int x = instance.GetGameWidth() / 2 - 13;
 num6=4;Use(x,num6);
}
if(!all_update){
 int x = instance.GetGameWidth() / 2 + 13;
 num6=4;Use(x,num6);
}}
}