public static class GameUtil{public static float RateConvert(float a,float b,float c,float d,float e,bool f){return a;}public static int RateConvert(int a,int b,int c,int d,int e,bool f){return a;}}
public class Fixture{public float distance;public float axisA,axisB;public int previous,current;public bool Run(){int slidingRate=1;float num=distance;if (!(num >= 30f) || !(num < 70f)){return false;}float num2=axisA;float num3=axisB;if(num2*num3==0f){return false;}float x=(num2+num3)/(num2*num3);
int num4 = (int)GameUtil.RateConvert(x, 0f, -1f, 0f, 100f, true);
int num5 = 0;int num6=previous;int num7=current;
int num8 = GameUtil.RateConvert(num7 - num6, 20, 0, 0, 100, true);
int num9=(int)((0.6f*(float)num4+0.2f*(float)num5+0.3f*(float)num8)/1.1f);return num9>50;
}}
