public class G {public void Draw(int x,int y,int w,int h){}}
public class Fixture{
 static int VIEW_Y=32;static int[] TBAR_W=new int[]{5,6,7,8,9,10,11};
 static int[] TBAR_SX=new int[]{1,2,3,4,5,6,7};
 static int Clamp(int value,int lo,int hi){return value<lo?lo:(value>hi?hi:value);}
 public void Run(G g,int type,int dx,int w){
 int height=VIEW_Y-1;
 switch(type){
 case 0:{
  if(w<=TBAR_W[type]){g.Draw(dx,TBAR_SX[type],w,height);break;}
  int num7=w;int num8=0;
  while(num7>0){int num9=Clamp(num7,1,TBAR_W[type]);g.Draw(dx+num8,TBAR_SX[type],num9,height);num7-=num9;num8+=num9;}
  break;
 }
 case 1:case 3:case 5:
  g.Draw(dx,TBAR_SX[type],w,height);
  switch(type){case 1:g.Draw(dx-1,TBAR_SX[type]+1,3,height);break;case 5:g.Draw(dx-1+w,TBAR_SX[type]+1,3,height);break;}
  break;
 case 2:case 4:{
  if(w<=TBAR_W[type]){g.Draw(dx,TBAR_SX[type],w,height);break;}
  int num4=w;int num5=0;
  while(num4>0){int num6=Clamp(num4,1,TBAR_W[type]);g.Draw(dx+num5,TBAR_SX[type],num6,height);num4-=num6;num5+=num6;}
  break;
 }
 case 6:{
  if(w<=TBAR_W[type]){g.Draw(dx,TBAR_SX[type],w,height);break;}
  int num=w;int num2=0;
  while(num>0){int num3=Clamp(num,1,TBAR_W[type]);g.Draw(dx+num2,TBAR_SX[type],num3,height);num-=num3;num2+=num3;}
  break;
 }
 }
}
}