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
int firstAnchor,lastAnchor,secondAnchor,thirdAnchor;
TeamData left=apdat_.tData_[udat_.gm_.teamId_[0]];
TeamData right=apdat_.tData_[udat_.gm_.teamId_[1]];
GameView view=GameView.GetInstance();
int offset=GetTopBarOffN();
int cursor=AppData.ClampMin((view.GetGameWidth()-(240+offset*2))/2,0);
firstAnchor=cursor;Use(cursor,1);cursor+=20;
secondAnchor=cursor;Use(cursor,2);cursor+=20;
thirdAnchor=cursor;Use(cursor,3);cursor+=20;
lastAnchor=cursor;Use(cursor,4);cursor+=20;
Use(firstAnchor,left.uniform_);Use(lastAnchor,right.uniform_);
Use(secondAnchor+offset,1);Use(thirdAnchor+offset,2);
int scoreX,scoreY;
scoreX=scoreY=0;
if(all_update){scoreX=view.GetGameWidth()/2-13;scoreY=4;Use(scoreX,scoreY);}
if(!all_update){scoreX=view.GetGameWidth()/2+13;scoreY=4;Use(scoreX,scoreY);}}
}