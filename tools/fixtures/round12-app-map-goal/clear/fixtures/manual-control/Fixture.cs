public class Font { public int StringWidth(string s) { return s.Length; } }
public class Graphics { public Font GetFont(Font input) { return input; } }
public class Fixture { public int clearPoint_mode,frame_,sink;
void Use(int a,int b) { sink+=a+b; }
public void Run(Graphics g) { Font f = g.GetFont(null);
Use(222,155);
int x;
int reserved;
int y=105;
x=30;y=105;Use(x,y);
y=181;
if(clearPoint_mode==4) {
 int counter=0;
 if(frame_%6*2<6) { counter=1; }
 Use(190,y);Use(51,y);
} else { Use(190,y);Use(51,y); }
if(clearPoint_mode==3) {
 string first=string.Empty;
 first="total";
 string second="points";
 x=120-(f.StringWidth(first)+f.StringWidth(second))/2;
 y=117;Use(x,y);x+=f.StringWidth(first);Use(x,y);
}
y=192;Use(93,y); }
}