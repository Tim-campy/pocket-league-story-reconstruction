public static class GameUtil{public static float RateConvert(float a,float b,float c,float d,float e,bool f){return a;}public static int RateConvert(int a,int b,int c,int d,int e,bool f){return a;}}
public class Semantics{public float distance;public float axisA,axisB;public int previous,current;public bool Before(){int slidingRate=1;float num=distance;if (!(num >= 30f) || !(num < 70f)){return false;}float num2=axisA;float num3=axisB;if(num2*num3==0f){return false;}float x=(num2+num3)/(num2*num3);
int num4 = (int)GameUtil.RateConvert(x, 0f, -1f, 0f, 100f, true);
int num5 = 0;int num6=previous;int num7=current;
int num8 = GameUtil.RateConvert(num7 - num6, 20, 0, 0, 100, true);
int num9=(int)((0.6f*(float)num4+0.2f*(float)num5+0.3f*(float)num8)/1.1f);return num9>50;
}public bool Candidate(){int slidingRate=1;float num=distance;int num4;
int num5;
int num8;
if (num >= 30f && num < 70f) { } else { return false; }float num2=axisA;float num3=axisB;if(num2*num3==0f){return false;}float x=(num2+num3)/(num2*num3);
num4 = (int)GameUtil.RateConvert(x, 0f, -1f, 0f, 100f, true);
num5 = 0;int num6=previous;int num7=current;
num8 = GameUtil.RateConvert(num7 - num6, 20, 0, 0, 100, true);
int num9=(int)((0.6f*(float)num4+0.2f*(float)num5+0.3f*(float)num8)/1.1f);return num9>50;
}public bool Control(){int slidingRate=1;float num=distance;int a;int b;int c;if(num>=30f&&num<70f){}else{return false;}float first=axisA;float second=axisB;if(first*second==0f){return false;}float dot=(first+second)/(first*second);a=(int)GameUtil.RateConvert(dot,0f,-1f,0f,100f,true);b=0;int oldValue=previous;int newValue=current;c=GameUtil.RateConvert(newValue-oldValue,20,0,0,100,true);int final=(int)((0.6f*(float)a+0.2f*(float)b+0.3f*(float)c)/1.1f);return final>50;}public bool Wrong(){int slidingRate=1;float num=distance;int a;int b;int c;if(num<30f||num>=70f){return false;}float first=axisA;float second=axisB;if(first*second==0f){return false;}float dot=(first+second)/(first*second);a=(int)GameUtil.RateConvert(dot,0f,-1f,0f,100f,true);b=0;int oldValue=previous;int newValue=current;c=GameUtil.RateConvert(newValue-oldValue,20,0,0,100,true);int final=(int)((0.6f*(float)a+0.2f*(float)b+0.3f*(float)c)/1.1f);return final>50;}}public class Runner{public static int Main(){float[] distances={float.NaN,float.NegativeInfinity,-1f,0f,29f,30f,31f,69f,70f,71f,float.PositiveInfinity};int bad=0,wrong=0,count=0;foreach(float d in distances)for(int score=0;score<300;score++){Semantics f=new Semantics();f.distance=d;f.axisA=1f;f.axisB=2f;f.previous=0;f.current=score;bool a=f.Before(),b=f.Candidate(),c=f.Control(),w=f.Wrong();if(a!=b||b!=c)bad++;if(a!=w)wrong++;count++;}System.Console.WriteLine(count+":"+bad+":"+wrong);return bad==0&&wrong>0?0:1;}}