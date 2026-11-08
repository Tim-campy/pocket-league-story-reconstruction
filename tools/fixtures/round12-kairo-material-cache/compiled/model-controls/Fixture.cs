using System.Collections.Generic;
public class Holder {public int N;}
public struct Pair {public Holder Key {get;set;}}
public static class StructModelControls {
 public static int ListGetter(List<Pair> list,int i){return list[i].Key.N;}
 public static int ArrayGetter(Pair[] array,int i){return array[i].Key.N;}
 public static int LocalGetter(Pair value){return value.Key.N;}
 public static int ReferenceIndexer(List<Holder> list,int i){return list[i].N;}
 public static int ReusedReceiver(List<Pair> list,int i,int j){return list[i].Key.N+list[j].Key.N;}
}
