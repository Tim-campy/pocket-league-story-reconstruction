public class Vec {public float x_,y_,z_;public void Set(float x,float y,float z){x_=x;y_=y;z_=z;} }
public class TBall {public Vec pos_,v_,a_;public void ReceiveBall(Fixture p){}public void ReleaseBall(Fixture p){} }
public class Data {public int[] GetCollisionData(int a,int b){return new int[4];} }
public class Player {public static Data udat_;}
public static class BitUtil {public static bool Check(int flags,int mask){return(flags&mask)!=0;} }
public static class GameUtil {
public static bool ChRectHitRect(float a,float b,float c,float d,float e,float f,float g,float h,bool z){return a>e;}
public static int Random(int max){return max;}
public static int RateConvert(int x,int a,int b,int c,int d,bool e){return x;}
public static float RateConvert(float x,float a,float b,float c,float d,bool e){return x;}
public static float Cos(int x){return x;}public static float Sin(int x){return x;}
public static float SetVStop(float distance,int time){return distance;}public static float SetAStop(float distance,int time){return distance;}
}
public class Fixture {public Vec pos_;public int flag_,team_id_;
public void Run(TBall tball){int[] first=Player.udat_.GetCollisionData(0,0);
int[] second=Player.udat_.GetCollisionData(1,1);
if(GameUtil.ChRectHitRect(pos_.x_+(float)first[0],pos_.z_+(float)first[1],first[2],first[3],tball.pos_.x_+(float)second[0],tball.pos_.z_+(float)second[1],second[2],second[3],true)&&BitUtil.Check(flag_,8)){
 int choice=GameUtil.Random(3);
 if(choice==2){tball.v_.Set(0f,0f,0f);tball.ReceiveBall(this);}
 else if(choice==1){
  int direction=GameUtil.Random(180)+team_id_*180;
  float reservedA,reservedB;
  float speed=GameUtil.RateConvert(GameUtil.Random(100),0f,99f,5f,10f,true);
  int sample=GameUtil.Random(100);
  float distance=GameUtil.RateConvert(sample,0,99,5,20,true);
  int time=GameUtil.RateConvert(sample,0,99,5,10,true);
  tball.ReceiveBall(this);tball.ReleaseBall(this);
  tball.v_.Set(speed*GameUtil.Cos(direction),0f,speed*GameUtil.Sin(direction));
  tball.v_.y_=GameUtil.SetVStop(distance,time);tball.a_.y_=GameUtil.SetAStop(distance,time);
 }else{
  int direction2=GameUtil.Random(90)+45+(1-team_id_)*180;
  float reservedC,reservedD;
  float speed2=GameUtil.RateConvert(GameUtil.Random(100),0f,99f,2.5f,5f,true);
  int sample2=GameUtil.Random(100);
  float distance2=GameUtil.RateConvert(sample2,0,99,2,10,true);
  int time2=GameUtil.RateConvert(sample2,0,99,5,10,true);
  tball.ReceiveBall(this);tball.ReleaseBall(this);
  tball.v_.Set(speed2*GameUtil.Cos(direction2),0f,speed2*GameUtil.Sin(direction2));
  tball.v_.y_=GameUtil.SetVStop(distance2,time2);tball.a_.y_=GameUtil.SetAStop(distance2,time2);
 }
}}
}