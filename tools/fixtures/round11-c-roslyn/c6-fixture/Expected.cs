public class Image {}
public class Graphics {
 public int value;public void SetFlipMode(int v){value+=v;}
 public void DrawImage(Image image,float x,float y,int sx,int sy,int width,int height){value+=(int)(x+y)+sx+sy+width+height;}
 public void FillRect(float x,float y,int w,int h){value+=(int)(x+y)+w+h;}
}
public class Fixture {
 static int value;static void Consume(){value++;}static void Observe(ref bool x){if(value!=0)x=false;}
 public static void NegativeLiteral(){if(true){Consume();}}
 public static void NegativeConst(){const bool local=true;if(local){Consume();}}
 public static void NegativeAddress(){bool local=true;Observe(ref local);if(local){Consume();}}
 public static void PositiveShort(){bool local=true;if(local){Consume();}}
 public static void Run(Graphics g,float x,float y,Image[] img,int[] sp){
 if(sp==null)return;
 if(sp[1]>-1){int sx=sp[2];int sy=sp[3];int width=sp[4];int height=sp[5];
 bool manuallyEnabled=true;
 if(manuallyEnabled) {
 int num4 = sp[8] * 1 + sp[9] * 2;
 if(num4!=0){g.SetFlipMode(num4);}
 g.DrawImage(img[sp[1]], x, y, sx, sy, width, height);
 g.SetFlipMode(0);
 g.SetFlipMode(1);
 g.SetFlipMode(2);
 g.SetFlipMode(3);
 g.SetFlipMode(4);
 g.SetFlipMode(5);
 g.SetFlipMode(6);
 g.SetFlipMode(7);
 g.SetFlipMode(8);
 g.SetFlipMode(9);
 g.SetFlipMode(10);
 g.SetFlipMode(11);
 g.SetFlipMode(12);
 g.SetFlipMode(13);
 g.SetFlipMode(14);
 g.SetFlipMode(15);
 g.SetFlipMode(16);
 g.SetFlipMode(17);
 g.SetFlipMode(18);
 g.SetFlipMode(19);
 g.SetFlipMode(20);
 g.SetFlipMode(21);
 g.SetFlipMode(22);
 g.SetFlipMode(23);
 g.SetFlipMode(24);
 g.SetFlipMode(25);
 g.SetFlipMode(26);
 g.SetFlipMode(27);
 g.SetFlipMode(28);
 g.SetFlipMode(29);
 g.SetFlipMode(30);
 g.SetFlipMode(31);
 if(num4!=0){g.SetFlipMode(0);}
}
 }else if(sp[1]==-2){g.FillRect(x,y,sp[4],sp[5]);}
 g.SetFlipMode(0);
 }
}
