public class Graphics{public static int GetColorOfRGB(int a,int b,int c){return a+b+c;}public void SetColor(int n){}public void DrawRect(float a,float b,float c,float d){}public void DrawLine(float a,float b,float c,float d,float lineWidth=1f){}public void FillRect(float a,float b,float c,float d){}}
public class Fixture{public void Run(Graphics g,int offset_x,int offset_y,int width,int height,int[][] colors){g.SetColor(Graphics.GetColorOfRGB(colors[0][0], colors[0][1], colors[0][2]));
g.DrawRect(offset_x, offset_y, width - 1, height - 1);
int num=offset_x+1;int num2=offset_y+1;int num3=width-2;int num4=height-2;
g.SetColor(Graphics.GetColorOfRGB(colors[2][0],colors[2][1],colors[2][2]));g.DrawLine(num,num2,num+num3-1,num2);g.DrawLine(num,num2+1,num,num2+num4-2);
g.SetColor(Graphics.GetColorOfRGB(colors[3][0],colors[3][1],colors[3][2]));g.DrawLine(num,num2+num4-1,num+num3-1,num2+num4-1);g.DrawLine(num+num3-1,num2+1,num+num3-1,num2+num4-2);
int num5=offset_x+2;int num6=offset_y+2;int num7=width-4;int num8=height-4;g.SetColor(Graphics.GetColorOfRGB(colors[1][0],colors[1][1],colors[1][2]));g.FillRect(num5,num6,num7,num8);}}
