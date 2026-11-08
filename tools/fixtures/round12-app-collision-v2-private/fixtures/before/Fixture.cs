using System;
public class BitUtil {public static int Reads;public static bool Check(int f,int mask){Reads++;return (f&mask)!=0;}}
public class JMath {public static int Reads;public static int Abs(int n){Reads++;return Math.Abs(n);}}
public class Point {public float x_,y_,z_;}
public class Player {public int[] address_=new int[2];public int flag_;}
public class Manager {public int state_,gameState_=2;}
public class Ball {public Point pos_=new Point();public int flag_=8;}
public class User {public Manager gm_=new Manager();public Ball ball_=new Ball();}
public class SoccerForm {public static int DEBUG_GK_MODE;}
public class GameUtil {public static bool Hit=true;public static int Reads;public static bool ChRectHitRect(float n){Reads++;return Hit;}}
public class Fixture {
 public int Trace,team_id_=1,p_id_=1,role_,flag_=8;public int[] address_=new int[2];public Point pos_=new Point();public User udat_=new User();public Player[] Players=new Player[]{new Player(),new Player(),new Player()};
public void RunLoop(){for(int i=0;i<2;i++){for(int j=0;j<Players.Length;j++){
 if(j == 0 || (i == team_id_ && j == p_id_)){continue;}
 Player player=Players[j];
 if(JMath.Abs(address_[0] - player.address_[0]) >= 2 || JMath.Abs(address_[1] - player.address_[1]) >= 2 || BitUtil.Check(player.flag_, 16408)){continue;}
 Trace++;
}}}
public void RunFloat(float num3,float num4){for(int n=0;n<1;n++){
 if(num3 < 0.001f || num4 < 0.001f){continue;}
 int num5=0;num5 = ((!(num4 > num3)) ? 1 : 0);
 Trace+=10+num5;
}}
public void RunRegions(){
 for(int j=0;j<2;j++){
  int[] collisionData=new int[1];
  if(!GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0])){continue;}
  Trace++;
 }
 if(udat_.gm_.state_ != 0 || udat_.gm_.gameState_ != 2 || !BitUtil.Check(udat_.ball_.flag_, 8)){return;}
 if(role_==0){
  if(!(udat_.ball_.pos_.y_ < 20f)){return;}
  int[] collisionData3=new int[1];
  if(!GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData3[0])){return;}
  if(BitUtil.Check(flag_,8)){
   if(SoccerForm.DEBUG_GK_MODE == 1 && team_id_ == 0){return;}
   Trace+=10;
  }else{Trace+=20;}
 }else{Trace+=30;}
}
public static int SemanticChecks(){
 Fixture f=new Fixture();BitUtil.Reads=0;JMath.Reads=0;f.RunLoop();if(f.Trace!=3||BitUtil.Reads!=3||JMath.Reads!=6)return 1;
 f.Trace=0;f.Players[1].address_[0]=3;BitUtil.Reads=0;JMath.Reads=0;f.RunLoop();if(f.Trace!=2||BitUtil.Reads!=2||JMath.Reads!=5)return 2;
 float[] vals=new float[]{float.NaN,float.NegativeInfinity,-0f,0.001f,1f,float.PositiveInfinity};
 foreach(float x in vals)foreach(float y in vals){f.Trace=0;f.RunFloat(x,y);int expected=(!(x<0.001f)&&!(y<0.001f))?(y>x?10:11):0;if(f.Trace!=expected)return 3;}
 foreach(float height in new float[]{float.NaN,10f,30f})for(int hit=0;hit<2;hit++)for(int state=0;state<2;state++)for(int role=0;role<2;role++)for(int dbg=0;dbg<2;dbg++)for(int team=0;team<2;team++){
  f.Trace=0;f.udat_.gm_.state_=state;f.role_=role;f.team_id_=team;SoccerForm.DEBUG_GK_MODE=dbg;GameUtil.Hit=hit!=0;f.udat_.ball_.pos_.y_=height;
  f.RunRegions();int expected=hit!=0?2:0;if(state==0){if(role==0){if(height<20f&&hit!=0&&!(dbg==1&&team==0))expected+=10;}else{expected+=30;}}if(f.Trace!=expected)return 4;
 }
 return 0;
}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}