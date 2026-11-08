public class Font { public int StringWidth(string s) { return s.Length; } }
public class Graphics { public Font GetFont(Font input) { return input; } }
public class Fixture { public int clearPoint_mode,frame_,sink;
void Use(int a,int b) { sink+=a+b; }
public void Run(Graphics g) { Font font = g.GetFont(null);
Use(222,155);
int num2;
int recoveredUnusedLayout; int num3;
int num = 105;
num2=30;num=105;Use(num2,num);
num=181;
if (clearPoint_mode == 4) {
 num3 = 0;
 if(frame_ % 6 * 2 < 6) { num3=1; }
 Use(190,num); Use(51,num);
} else { Use(190,num); Use(51,num); }
if(clearPoint_mode == 3) {
 string empty=string.Empty;
 empty="total";
 string text="points";
 num2=120-(font.StringWidth(empty)+font.StringWidth(text))/2;
 num=117;Use(num2,num);num2+=font.StringWidth(empty);Use(num2,num);
}
num=192;Use(93,num); }
}