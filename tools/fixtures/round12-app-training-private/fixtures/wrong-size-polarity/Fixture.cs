public class Vector{int count;public int Size(){return count;}public void AddElement(object value){count++;}public object FirstElement(){return null;}public void RemoveElementAt(int index){count--;}}
public class Cell{public Vector tPlayerList_=new Vector();}
public class Area{public Cell[] tGroupAreas_=new Cell[3];}
public static class TGroupArea{public static int GetAreaMaxNum(int mode){return mode;}}
public class Fixture{int[][] tModeList_;Area[][] tAreas_;int result;void Sink(int n){result=n;}
public void Run(Vector[] array,int num14,int num17,int areaMaxNum,int num16){int num15=0;for(int iteration=0;iteration<areaMaxNum&&array[num14].Size()>=0;iteration++){
 int maximum=TGroupArea.GetAreaMaxNum(tModeList_[num14][num17]);
 for(int group=0;group<tAreas_[num14][num17].tGroupAreas_.Length;group++){
  if(tAreas_[num14][num17].tGroupAreas_[group].tPlayerList_.Size()<maximum){
   tAreas_[num14][num17].tGroupAreas_[group].tPlayerList_.AddElement(array[num14].FirstElement());
   array[num14].RemoveElementAt(0);num15++;break;
  }
 }
 if(num15>=num16){break;}
} Sink(num15);}}
