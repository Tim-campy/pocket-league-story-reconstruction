public static class Fixture {
 public sealed class Vector { public static int Count; public Vector(){Count++;} }
 public sealed class PlayerList { public int Number; public void AddElement(object value){Number++;} }
 public sealed class Group { public PlayerList tPlayerList_=new PlayerList(); }
 public sealed class Area { public Group[] tGroupAreas_=new Group[]{new Group()}; }
 public static class GameUtil { public static int Calls,Value; public static int Random(int limit){Calls++;return Value;} }
 public static Area[][] tAreas_;
 public static int Run(int num25,int num26) {
 int num23=0;
 Vector[] array = new Vector[2];
 array[0]=new Vector();
 array[1]=new Vector();
 int num24=0; object obj4=array[0];
 if(num25<num26){num23=0;}else{num23=GameUtil.Random(2);}
 if(num23==0){tAreas_[num24][0].tGroupAreas_[0].tPlayerList_.AddElement(obj4);}
 else if(num23==1){tAreas_[num24][1].tGroupAreas_[0].tPlayerList_.AddElement(obj4);}
 return tAreas_[0][0].tGroupAreas_[0].tPlayerList_.Number+10*tAreas_[0][1].tGroupAreas_[0].tPlayerList_.Number;
}

 public static int SemanticChecks(){
  int[] values=new int[]{-1,0,1,2};
  for(int i=0;i<values.Length;i++)for(int j=0;j<2;j++){
   tAreas_=new Area[][]{new Area[]{new Area(),new Area()}};
   Vector.Count=0;GameUtil.Calls=0;GameUtil.Value=values[i];
   int actual=Run(j,1);
   int desired=(j<1)?1:((values[i]==0)?1:((values[i]==1)?10:0));
   if(actual!=desired||GameUtil.Calls!=j||Vector.Count!=2)return 1;
  }
  return 0;
 }
 public static float FloatNeg(float a){return -a;}
 public static float FloatZeroSub(float a){return 0f-a;}
}
