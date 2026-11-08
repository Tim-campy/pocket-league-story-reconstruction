public class Graphics { public static int GetColorOfRGB(int r,int g,int b){return r+g+b;} public void SetColor(int color){} public void DrawLine(float a,float b,float c,float d,float width=1f){} }
public static class GameUtil { public static int RateConvert(int x,int low,int high,int start,int end,bool clamp){return start+x*(end-start)/(high-low);} }
public class Fixture { public int[][] MENUGRAPH_POINTPOS,MENUGRAPH_OFF;
public void Run(Graphics g,int ox,int oy,int[] param1,int[] param2,int frame){int ax,ay,bx,by;
for(int initial=0;initial<MENUGRAPH_POINTPOS.Length;initial++){}
g.SetColor(Graphics.GetColorOfRGB(214,214,214));
for(int first=0;first<5;first++){
 ax=ox+MENUGRAPH_POINTPOS[5][0];ay=oy+MENUGRAPH_POINTPOS[5][1];
 bx=ox+MENUGRAPH_POINTPOS[first][0];by=oy+MENUGRAPH_POINTPOS[first][1];g.DrawLine(ax,ay,bx,by);
}
g.SetColor(Graphics.GetColorOfRGB(160,160,160));
for(int second=0;second<5;second++){
 ax=ox+MENUGRAPH_POINTPOS[second][0];ay=oy+MENUGRAPH_POINTPOS[second][1];
 bx=ox+MENUGRAPH_POINTPOS[(second+1)%5][0];by=oy+MENUGRAPH_POINTPOS[(second+1)%5][1];g.DrawLine(ax,ay,bx,by);
}
int value,nextValue;
int limit=25;
for(int segment=0;segment<5;segment++){
 value=GameUtil.RateConvert(frame,0,limit,param1[segment],param2[segment],true);
 nextValue=GameUtil.RateConvert(frame,0,limit,param1[(segment+1)%5],param2[(segment+1)%5],true);
 ax=ox+GameUtil.RateConvert(value,0,1000,MENUGRAPH_POINTPOS[segment][0]/5,MENUGRAPH_POINTPOS[segment][0],true);
 ay=oy+GameUtil.RateConvert(value,0,1000,MENUGRAPH_POINTPOS[segment][1]/5,MENUGRAPH_POINTPOS[segment][1],true);
 bx=ox+GameUtil.RateConvert(nextValue,0,1000,MENUGRAPH_POINTPOS[(segment+1)%5][0]/5,MENUGRAPH_POINTPOS[(segment+1)%5][0],true);
 by=oy+GameUtil.RateConvert(nextValue,0,1000,MENUGRAPH_POINTPOS[(segment+1)%5][1]/5,MENUGRAPH_POINTPOS[(segment+1)%5][1],true);
 for(int thickness=4;thickness>=0;thickness--){
  if(thickness>0){ax+=MENUGRAPH_OFF[thickness-1][0];ay+=MENUGRAPH_OFF[thickness-1][1];bx+=MENUGRAPH_OFF[thickness-1][0];by+=MENUGRAPH_OFF[thickness-1][1];}
  if(thickness==0){g.SetColor(Graphics.GetColorOfRGB(199,0,46));}else{g.SetColor(Graphics.GetColorOfRGB(255,128,100));}
  g.DrawLine(ax,ay,bx,by);
 }
}}
}