using System;using System.Collections.Generic;
public class Seb {}
public class Matrix {}
public class Pictograph {
 public string Key;public Seb Seb{get;set;}public int Frame{get;set;}public int Wait{get;set;}public int X{get;set;}public int Y{get;set;}public int Margin{get;set;}public Matrix Transform{get;set;}
 public static int creates;
 public Pictograph(string key,Seb seb,int frame,int wait,int x,int y,int margin){creates++;Key=key;Seb=seb;Frame=frame;Wait=wait;X=x;Y=y;Margin=margin;}
}
public static class Fixture {
 static Dictionary<string,Pictograph> pictographList_=new Dictionary<string,Pictograph>();
 static Pictograph GetPictograph(string key){Pictograph p;return pictographList_.TryGetValue(key,out p)?p:null;}
 public static Pictograph Run(string key,Seb seb,int frame=-1,int wait=1,int x=0,int y=0,int margin=0) {
  if(wait<1){wait=1;}
  Pictograph pictograph=GetPictograph(key);
  if(pictograph!=null) {
   pictograph.Seb=seb;pictograph.Frame=frame;pictograph.Wait=wait;pictograph.X=x;pictograph.Y=y;pictograph.Margin=margin;pictograph.Transform=new Matrix();
  } else { pictographList_[key] = (pictograph = new Pictograph(key, seb, frame, wait, x, y, margin)); }
  return pictograph;
 }
 public static int SemanticChecks() {
  pictographList_.Clear();Pictograph.creates=0;Seb a=new Seb();
  Pictograph first=Run("a",a,3,0,4,5,6);
  if(Pictograph.creates!=1||first.Wait!=1||first!=pictographList_["a"])return 1;
  Pictograph second=Run("a",a,9,2,7,8,10);
  if(first!=second||Pictograph.creates!=1||second.Frame!=9||second.Wait!=2||second.X!=7||second.Transform==null)return 2;
  bool threw=false;try{Run(null,a);}catch(ArgumentNullException){threw=true;}
  return threw?0:3;
 }
 public static float FloatNeg(float v){return -v;} public static float FloatZeroSub(float v){return 0f-v;}
}