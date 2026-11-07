using System.Collections.Generic;
enum TouchPhase { Began, Other }
struct Touch { public int id; public TouchPhase kind; public int fingerId {get{return id;}} public TouchPhase phase {get{return kind;}} }
class Fixture {
 bool invalidTouch_; int count,frames;
 Dictionary<int,float[]> onTouchPointer_=new Dictionary<int,float[]>();
 void Send(int a,int[] b,float[] c){}
 public void Run(Touch touch) {
  if (invalidTouch_) { return; }
  if (count>0 && frames>10) {
   List<int[]> list2=new List<int[]>();List<int[]> list3=new List<int[]>();
   float[] value=null;onTouchPointer_.TryGetValue(touch.fingerId,out value);
   if (touch.phase == TouchPhase.Began) { onTouchPointer_[touch.fingerId] = (value = new float[4]); }
   if(value!=null){value[0]=touch.fingerId;list3.Add(new int[]{touch.fingerId});}
   if (list3.Count > 0) {
    int num3 = 0;
    if(list2.Count==0){num3=1;}
    if(list3.Count-num3>0) {
     int[] array12=new int[list3.Count-num3];float[] array13=new float[(list3.Count-num3)*2];
     int num4=6;
     for(int m=0;m<list3.Count-num3;m++){array12[m]=list3[m+num3][0];array13[m*2]=value[0];num4|=1<<array12[m]<<8;}
     Send(num4,array12,array13);
    }
    if(num3>0){Send(1,new int[]{list3[0][0]},new float[]{value[0],value[1]});}
   }
  }
 }
}
