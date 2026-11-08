using System;
public class Config{public static bool PRINT;}
public class Point{public float z_;}
public class Ball{public Point pos_=new Point();public int[] camera_pos_=new int[]{3,21};}
public class UserData{public static int Reads;public Ball ball_=new Ball();public int[] et_cur=new int[]{7,8};public static UserData GetInstance(){Reads++;return new UserData();}}
public class TouchComponent{public int id=4,value=17;}
public class TouchEvent{public int type,dx=3;public TouchComponent component=new TouchComponent();}
public class Canvas{public int Clicks,Key;public void KeyClick(int key){Clicks++;Key=key;}}
public class FixtureBase{public int BaseCalls,LastType,LastValue;public void SetTouchValue(int type,int value){BaseCalls++;LastType=type;LastValue=value;}}
public class Fixture:FixtureBase{public int select_,select2_,type_=2,TrackCalls,TrackValue,TrackDx;public TouchComponent TrackComponent;public UserData udat_=new UserData();public Canvas canvas_=new Canvas();public static Point[] GOAL1_GLOBAL_POS=new Point[]{new Point(),new Point(),new Point()};
void Set(int v,int dx,TouchComponent component){TrackCalls++;TrackValue=v;TrackDx=dx;TrackComponent=component;}
public int RunGet(int type,int index){UserData instance=UserData.GetInstance();type=(int)(type&0xFFFF0000u);switch(type){case 65536:return 0;case 393216:return instance.et_cur[0];default:if(Config.PRINT){throw new ArgumentException("bad "+type);}break;}
return 0;}
public void RunSp(int type,int value){type=(int)(type&0xFFFF0000u);switch (type) {case 131072: select_ = value; break; default: base.SetTouchValue(type,value); break;}}
public int RunGoal(int parts_id){if((parts_id==3)){if(udat_.ball_.pos_.z_<GOAL1_GLOBAL_POS[2].z_){return udat_.ball_.camera_pos_[1]-1;}} else if (parts_id == 4) {} else if (parts_id == 5) {} else if (parts_id == 6) {} else if (parts_id == 7) {}return 0;}
public void RunTrack(int type,int v,TouchEvent e){switch (type) {case 0: break; case 9: Set(v,e.dx,e.component); break;}}
public bool RunBilling(TouchEvent e){TouchComponent component=e.component;if(type_==2&&component.id==4){switch (e.type) {case 2: case 1: {select2_=component.value;if(e.type==1){canvas_.KeyClick(2048);}break;}}return false;}return true;}
public void RunLineup(int type,int value){type=(int)(type&0xFFFF0000u);switch (type) {case 131072: select_ = value; break; default: base.SetTouchValue(type,value); break;}}
public static int SemanticChecks(){Fixture f=new Fixture();TouchEvent e=new TouchEvent();
 foreach(int type in new int[]{0,65536,65537,131072,131073,393216,-1,int.MaxValue}){
  UserData.Reads=0;Config.PRINT=false;int value=f.RunGet(type,0);int masked=(int)(type&0xFFFF0000u);if(value!=(masked==393216?7:0)||UserData.Reads!=1)return 1;
  Config.PRINT=true;bool thrown=false;try{value=f.RunGet(type,0);}catch(ArgumentException){thrown=true;}if(thrown!=(masked!=65536&&masked!=393216))return 2;
  f.BaseCalls=0;f.select_=-1;f.RunSp(type,17);if(f.BaseCalls!=(masked==131072?0:1)||f.select_!=(masked==131072?17:-1))return 3;
  f.BaseCalls=0;f.select_=-1;f.RunLineup(type,19);if(f.BaseCalls!=(masked==131072?0:1)||f.select_!=(masked==131072?19:-1))return 4;
 }
 foreach(int type in new int[]{-1,0,1,2,8,9,10}){f.TrackCalls=0;f.RunTrack(type,7,e);if(f.TrackCalls!=(type==9?1:0)||(type==9&&(f.TrackValue!=7||f.TrackDx!=e.dx||f.TrackComponent!=e.component)))return 5;}
 for(int owner=1;owner<=3;owner++)for(int id=3;id<=5;id++)for(int type=-1;type<=3;type++){
  f.type_=owner;e.component.id=id;e.type=type;f.select2_=-1;f.canvas_.Clicks=0;bool returned=f.RunBilling(e);bool active=owner==2&&id==4;bool assign=active&&(type==1||type==2);if(returned==active||f.select2_!=(assign?e.component.value:-1)||f.canvas_.Clicks!=(active&&type==1?1:0))return 6;
 }
 foreach(float z in new float[]{float.NaN,float.NegativeInfinity,-0f,0f,1f,float.PositiveInfinity})for(int part=-1;part<=8;part++){
  f.udat_.ball_.pos_.z_=z;GOAL1_GLOBAL_POS[2].z_=0f;int result=f.RunGoal(part);if(result!=(part==3&&z<0f?20:0))return 7;
 }
 return 0;}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}