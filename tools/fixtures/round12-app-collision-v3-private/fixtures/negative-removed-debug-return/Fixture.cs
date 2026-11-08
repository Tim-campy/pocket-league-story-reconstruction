using System;
public class Point {public float y_;}
public class Ball {public int flag_=8,boundNum_;public Point pos_=new Point();}
public class Manager {public int state_,gameState_=2;}
public class User {public Ball ball_=new Ball();public Manager gm_=new Manager();}
public class SoccerForm {public static int DEBUG_GK_MODE;}
public class BitUtil {public static int Reads;public static bool Check(int flags,int mask){Reads++;return (flags&mask)!=0;}}
public class Fixture {
 public User udat_=new User();public int role_,team_id_,Trace,TrapReads,RepeatReads;public bool Hit=true,Trap=true;
 bool ChTrapOrHeading(){TrapReads++;return Trap;}void ChRepetition(){RepeatReads++;Trace+=30;}
public void RunHeight(){if(role_==0){
 if (udat_.ball_.pos_.y_ < 20f) {
 int[] collisionData3=new int[1];int[] collisionData4=new int[1];
 if(Hit){if(SoccerForm.DEBUG_GK_MODE==1&&team_id_==0){}Trace+=10+collisionData3[0]+collisionData4[0];}
}
}else{Trace+=1;}}
public void RunTail(){if(role_==0){Trace+=1;}
 else {
if (BitUtil.Check(udat_.ball_.flag_, 16) && BitUtil.Check(udat_.ball_.flag_, 32) && udat_.ball_.boundNum_ == 0) {return;}
if (ChTrapOrHeading()) {ChRepetition();}
}
}
public void RunCombined(){
 if(udat_.gm_.state_==0&&udat_.gm_.gameState_==2&&BitUtil.Check(udat_.ball_.flag_,8)){
 if(role_==0){
  if (udat_.ball_.pos_.y_ < 20f) {
  int[] collisionData3=new int[1];int[] collisionData4=new int[1];
  if(Hit){if(SoccerForm.DEBUG_GK_MODE==1&&team_id_==0){}Trace+=10+collisionData3[0]+collisionData4[0];}
 }
}else {
if (BitUtil.Check(udat_.ball_.flag_, 16) && BitUtil.Check(udat_.ball_.flag_, 32) && udat_.ball_.boundNum_ == 0) {return;}
if (ChTrapOrHeading()) {ChRepetition();}
}
 }
}
public static int SemanticChecks(){Fixture f=new Fixture();
 float[] heights=new float[]{float.NaN,float.NegativeInfinity,-0f,0f,19.99f,20f,float.PositiveInfinity};
 foreach(float height in heights)for(int role=0;role<2;role++)for(int state=0;state<2;state++)for(int gm=1;gm<=2;gm++)for(int mask=0;mask<4;mask++)for(int bound=-1;bound<=1;bound++)for(int trap=0;trap<2;trap++)for(int hit=0;hit<2;hit++)for(int dbg=0;dbg<2;dbg++)for(int team=0;team<2;team++){
  f.udat_.ball_.pos_.y_=height;f.role_=role;f.udat_.gm_.state_=state;f.udat_.gm_.gameState_=gm;f.udat_.ball_.flag_=8+(mask%2)*16+(mask/2)*32;f.udat_.ball_.boundNum_=bound;f.Trap=trap!=0;f.Hit=hit!=0;SoccerForm.DEBUG_GK_MODE=dbg;f.team_id_=team;
  f.Trace=0;f.TrapReads=0;f.RepeatReads=0;BitUtil.Reads=0;f.RunCombined();
  int trace=0,reads=(state==0&&gm==2)?1:0,traps=0,repeats=0;
  if(state==0&&gm==2){if(role==0){if(height<20f&&hit!=0&&!(dbg==1&&team==0))trace=10;}else{reads++;if(mask%2!=0)reads++;if(!(mask==3&&bound==0)){traps=1;if(trap!=0){trace=30;repeats=1;}}}}
  if(f.Trace!=trace||BitUtil.Reads!=reads||f.TrapReads!=traps||f.RepeatReads!=repeats)return 1;
 }
 return 0;}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}