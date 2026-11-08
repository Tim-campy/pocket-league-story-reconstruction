public class Graphics { public static int GetColorOfRGB(int r,int g,int b){return r+g+b;} public void SetColor(int color){} public void DrawLine(float a,float b,float c,float d,float width=1f){} }
public static class GameUtil { public static int RateConvert(int x,int low,int high,int start,int end,bool clamp){return start+x*(end-start)/(high-low);} }
public class Fixture { public int[][] MENUGRAPH_POINTPOS,MENUGRAPH_OFF;
public void Run(Graphics g,int ox,int oy,int[] param1,int[] param2,int frame){for (int i = 0; i < MENUGRAPH_POINTPOS.Length; i++) { }
g.SetColor(Graphics.GetColorOfRGB(214,214,214));
for(int j=0;j<5;j++) {
 int num=ox+MENUGRAPH_POINTPOS[5][0];int num2=oy+MENUGRAPH_POINTPOS[5][1];
 int num3=ox+MENUGRAPH_POINTPOS[j][0];int num4=oy+MENUGRAPH_POINTPOS[j][1];
 g.DrawLine(num,num2,num3,num4);
}
g.SetColor(Graphics.GetColorOfRGB(160,160,160));
for(int k=0;k<5;k++) {
 int num=ox+MENUGRAPH_POINTPOS[k][0];int num2=oy+MENUGRAPH_POINTPOS[k][1];
 int num3=ox+MENUGRAPH_POINTPOS[(k+1)%5][0];int num4=oy+MENUGRAPH_POINTPOS[(k+1)%5][1];
 g.DrawLine(num,num2,num3,num4);
}
int c1max = 25;
for(int l=0;l<5;l++) {
 int x = GameUtil.RateConvert(frame,0,c1max,param1[l],param2[l],true);
 int x2 = GameUtil.RateConvert(frame,0,c1max,param1[(l+1)%5],param2[(l+1)%5],true);
 int num=ox+GameUtil.RateConvert(x,0,1000,MENUGRAPH_POINTPOS[l][0]/5,MENUGRAPH_POINTPOS[l][0],true);
 int num2=oy+GameUtil.RateConvert(x,0,1000,MENUGRAPH_POINTPOS[l][1]/5,MENUGRAPH_POINTPOS[l][1],true);
 int num3=ox+GameUtil.RateConvert(x2,0,1000,MENUGRAPH_POINTPOS[(l+1)%5][0]/5,MENUGRAPH_POINTPOS[(l+1)%5][0],true);
 int num4=oy+GameUtil.RateConvert(x2,0,1000,MENUGRAPH_POINTPOS[(l+1)%5][1]/5,MENUGRAPH_POINTPOS[(l+1)%5][1],true);
 for(int num5=4;num5>=0;num5--) {
  if(num5>0){num+=MENUGRAPH_OFF[num5-1][0];num2+=MENUGRAPH_OFF[num5-1][1];num3+=MENUGRAPH_OFF[num5-1][0];num4+=MENUGRAPH_OFF[num5-1][1];}
  if(num5==0){g.SetColor(Graphics.GetColorOfRGB(199,0,46));}else{g.SetColor(Graphics.GetColorOfRGB(255,128,100));}
  g.DrawLine(num,num2,num3,num4);
 }
}}
}