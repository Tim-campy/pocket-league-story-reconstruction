using System;
public struct GameAppli {public int Pid;public string Tag;}
public class Reader {
 public GameAppli[] Entries;
 public int Calls;
 public int gameId_;
 public Reader GetInstance(){return this;}
 public GameAppli[] GetAppList(){Calls++;return Entries;}
 public bool Run(){
  Reader instance=GetInstance();
  bool flag=false;
  GameAppli[] appList=instance.GetAppList();
for(int i=0;i<appList.Length;i++){
GameAppli recoveredApp=appList[i];
if(recoveredApp.Pid==gameId_){flag=true;}
}
  return flag;
 }
}
public static class Fixture {
 public static int SemanticChecks(){
  Reader r=new Reader();
  GameAppli[][] arrays={new GameAppli[0],new GameAppli[]{new GameAppli{Pid=1,Tag="a"}},new GameAppli[]{new GameAppli{Pid=2},new GameAppli{Pid=3},new GameAppli{Pid=2}}};
  for(int i=0;i<arrays.Length;i++)for(int target=0;target<5;target++){
   r.Entries=arrays[i];r.gameId_=target;r.Calls=0;
   bool expected=false;foreach(GameAppli entry in arrays[i])if(entry.Pid==target)expected=true;
   bool actual=r.Run();if(actual!=expected||r.Calls!=1)return 1;
   if(arrays[i].Length>0&&arrays[i][0].Pid!=(i==1?1:2))return 2;
  }
  r.Entries=null;r.Calls=0;bool threw=false;try{r.Run();}catch(NullReferenceException){threw=true;}
  if(!threw||r.Calls!=1)return 3;
  return 0;
 }
 public static float FloatNeg(float x){return -x;}
 public static float FloatZeroSub(float x){return 0f-x;}
}
