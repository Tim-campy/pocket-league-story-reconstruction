using System;
public class Graphics{}
public class MapchipMain{public int fenceId_,img_,flag_;}
public class Image{public static int Reads;public int GetHeight(){Reads++;return 12;}}
public class Resource{public Image[] img=new Image[]{new Image(),new Image(),new Image(),new Image(),new Image(),new Image(),new Image(),new Image()};}
public class User{public int tAreaLv_;}
public class App{public MapchipMain[] mapchipMain_=new MapchipMain[8];public Resource resMapChipMain_=new Resource();public int Trace,Calls;public App(){for(int i=0;i<8;i++){mapchipMain_[i]=new MapchipMain();mapchipMain_[i].img_=i;}}public void InsertGameObject(int kind,int tag){Calls++;Trace=Trace*37+kind*10+tag;}}
public class GameUtil{public static bool Hit=true;public static int Calls;public static bool ChRectHitRect(int x,int y,int w,int h){Calls++;return Hit;}}
public class Fixture{public App apdat_=new App();public User udat_=new User();public static int MAP_GROUND,MAPCHIP_FIELD_HNUM=1,MAPCHIP_FIELD_WNUM=2;public int[][] MAPCHIP_IMG=new int[][]{new int[]{2,3}};public int[] camera_pos=new int[]{5,6};public MapchipMain[] mapchipGrounds=new MapchipMain[]{new MapchipMain()};
public void Run(Graphics g,bool offscreen){MapchipMain mapchipMain=null;if(MAP_GROUND==0){mapchipMain=null;}else{mapchipMain=mapchipGrounds[MAP_GROUND-1];}int num2;
for(int num=MAPCHIP_FIELD_HNUM-1;num>=0;num--){for(int i=0;i<MAPCHIP_FIELD_WNUM;i++){
 if(offscreen&&i==1){continue;}
 num2=-1;MapchipMain mapchipMain2=apdat_.mapchipMain_[MAPCHIP_IMG[num][i]];num2=mapchipMain2.fenceId_;
 if(udat_.tAreaLv_<2){if (num2 == 0) {mapchipMain2=apdat_.mapchipMain_[MAPCHIP_IMG[num][i]+1];} else if (num2 == 3) {mapchipMain2=apdat_.mapchipMain_[MAPCHIP_IMG[num][i]-1];} else if (num2 == 6) {num2=5;}}if(mapchipMain!=null&&num2==5){mapchipMain2=mapchipMain;}Image image=apdat_.resMapChipMain_.img[mapchipMain2.img_];if (!(GameUtil.ChRectHitRect(camera_pos[0],camera_pos[1]+29-image.GetHeight(),60,image.GetHeight()))) {continue;}
apdat_.InsertGameObject(5,mapchipMain2.img_);if (num2 < 5) {continue;}
apdat_.InsertGameObject(6,num2);}}}
public static int SemanticChecks(){Fixture f=new Fixture();Graphics g=new Graphics();f.mapchipGrounds[0].img_=7;
foreach(int fence in new int[]{-2,-1,0,1,3,6,7})for(int area=1;area<=2;area++)for(int ground=0;ground<2;ground++)for(int hit=0;hit<2;hit++)for(int off=0;off<2;off++){
 for(int i=0;i<8;i++){f.apdat_.mapchipMain_[i].fenceId_=fence;}f.udat_.tAreaLv_=area;MAP_GROUND=ground;GameUtil.Hit=hit!=0;GameUtil.Calls=0;Image.Reads=0;f.apdat_.Trace=0;f.apdat_.Calls=0;f.Run(g,off!=0);
 int cells=off!=0?1:2;if(GameUtil.Calls!=cells||Image.Reads!=2*cells)return 1;
 int trace=0,calls=0;for(int i=0;i<cells;i++){int part=2+i;int value=fence;if(area<2){if(value==0){part++;}else if(value==3){part--;}else if(value==6){value=5;}}if(ground==1&&value==5){part=7;}if(hit!=0){calls++;trace=trace*37+50+part;if(value>=5){calls++;trace=trace*37+60+value;}}}
 if(trace!=f.apdat_.Trace||calls!=f.apdat_.Calls)return 2;
}return 0;}
public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}