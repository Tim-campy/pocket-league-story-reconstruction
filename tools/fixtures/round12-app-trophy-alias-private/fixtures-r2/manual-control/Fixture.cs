public class Image{}
public class Graphics{public void SetRenderMode(int a,int b,int c){}public void DrawImage(Image i,float x,float y,int sx,int sy,int width,int height){}}
public class Seb{public int[] GetSprite(int a,int b){return null;}}
public class Res{public Seb[] seb;public Image[] img;}
public class Property{public int[] ints;}
public static class Graph{public static int Easing(int a,int b,int c,int d,int e){return a;}}
public class Fixture{public Res resCom2_;public Property sysSave_;
public void DrawSeb(Graphics g,int x,int y,Seb seb,Image img,int id){}
public void Run(Graphics g,int trophy_id,int subform_frame,int px,int py){DrawSeb(g,px,py,resCom2_.seb[106],resCom2_.img[190],sysSave_.ints[10]-1);
int originX=px;
int originY=py;
int frame=(subform_frame-20)%66;
int[] sprite=null;
sprite=resCom2_.seb[106].GetSprite(trophy_id,0);
int spriteHeight=sprite[5];
int[][] modes=new int[2][]{new int[4]{1,4,1,128},new int[4]{1,2,5,255}};
int speed=4;
int adjusted=frame-2;
for(int mode=0;mode<modes.Length;mode++){
 if(mode==1){continue;}
 int[] config=modes[mode];
 int position=adjusted*speed;
 int ease=0;
 if(position<=sprite[4]/2){ease=Graph.Easing(config[0],config[1],sprite[4]/2,position,0);}
 if(position>=sprite[4]/2){ease=Graph.Easing(config[1],config[0],sprite[4]/2,position-sprite[4]/2,0);}
 int x=originX+sprite[6]+position-ease/2;
 int y=originY+sprite[7];
 int sx=sprite[2]+position-ease/2;
 int sy=sprite[3];
 int width=ease;
 int height=spriteHeight;
 if(sx<sprite[2]){int clip=sprite[2]-sx;sx+=clip;width-=clip;x+=clip;}
 if(sx+width>sprite[2]+sprite[4]){width=sprite[2]+sprite[4]-sx;}
 if(width>0){
  g.SetRenderMode(1,config[3],255);
  for(int repeat=0;repeat<config[2];repeat++){g.DrawImage(resCom2_.img[sprite[1]],x,y,sx,sy,width,height);}
  g.SetRenderMode(0,255,255);
 }
}}
}