using System.Collections.Generic;
public class Fixture {
 public bool invalidTouch_;
 public Dictionary<int,float[]> onTouchPointer_=new Dictionary<int,float[]>();
 public struct Touch { public int fingerId; }
 public static class Input { public static int touchCount=2; public static Touch GetTouch(int i) { Touch t; t.fingerId=i; return t; } }
 public class IApplication { public static IApplication GetCurrentApp(){return new IApplication();} public int GetTotalFrames(){return 20;} }
 public int result;
 void Consume(float[] values,int n){result+=values.Length+n;}
 public void Run() {
  if (invalidTouch_) { return; }
  if (Input.touchCount > 0 && IApplication.GetCurrentApp().GetTotalFrames() > 10) {
   List<int[]> list = new List<int[]>();
   for(int i=0;i<Input.touchCount;i++) {
    Touch touch=Input.GetTouch(i);
    float[] value=null;
    onTouchPointer_.TryGetValue(touch.fingerId,out value);
    if(value==null) { value = new float[4]; onTouchPointer_[touch.fingerId] = value; }
    list.Add(new int[]{touch.fingerId}); Consume(value,i);
   }
   if(list.Count>0) { int total=0; for(int j=0;j<list.Count;j++) { total+=list[j][0]; } result+=total; }
   {
    int num3 = 0;
    if(list.Count==1) { num3=list[0][0]; }
    if(list.Count>num3) { int width=6; for(int m=0;m<list.Count-num3;m++){ result+=list[m][0]+width; } }
   }
  }
 }
}
