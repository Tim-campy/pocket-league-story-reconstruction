using System;
public sealed class Classifier {
 public object component;
 public int type;
 public string Describe(){
 if(component==null){return string.Concat(this,":component=")??"";}
 string text="";
 if(type==0){text="TYPE_DOWN";}else if(type==1){text="TYPE_UP";}else if(type==2){text="TYPE_ENTER";}
 else if(type==3){text="TYPE_LEAVE";}else if(type==4){text="TYPE_MOVE";}else if(type==5){text="TYPE_CLICK";}
 else if(type==6){text="TYPE_LOST";}else if(type==7){text="TYPE_DRAG";}else if(type==8){text="TYPE_GRID_DRAG";}
 else if(type==9){text="TYPE_CELL_DRAG";}else if(type==10){text="TYPE_CANCEL";}else if(type==13){text="TYPE_ON";}
 else{text="NONE["+type+"]";}
 return "label:"+text;
}
}
public static class Parser {
 public static int Calls;
 static int[] getAreaSize_result=new int[2];
 public static string GetFileFormat(byte[] data){Calls++;return data[0]==1?"gif":(data[0]==2?"png":(data[0]==3?"jpg":null));}
 public static int[] Run(byte[] data){
 int[] array=getAreaSize_result;
 array[0]=(array[1]=0);
 string fileFormat=GetFileFormat(data);
 if(fileFormat=="gif"){array[0]=data[1];array[1]=data[2];}
 else if(fileFormat=="png"){array[0]=data[3];array[1]=data[4];}
 else if(fileFormat=="jpg"){{
  int num=2;
  while(true){
   int num2=(data[num+0]<<8)|data[num+1];num+=2;
   int num3=(data[num+0]<<8)|data[num+1];
   if(num2==65472){
    array[1]=(data[num+3]<<8)|data[num+4];
    array[0]=(data[num+5]<<8)|data[num+6];
    break;
   }
   num+=num3;
  }
 }}
 return array;
}
}
public static class Fixture {
 public static int SemanticChecks(){
  string[] expected={"TYPE_DOWN","TYPE_UP","TYPE_ENTER","TYPE_LEAVE","TYPE_MOVE","TYPE_CLICK","TYPE_LOST","TYPE_DRAG","TYPE_GRID_DRAG","TYPE_CELL_DRAG","TYPE_CANCEL","NONE[11]","NONE[12]","TYPE_ON","NONE[14]"};
  Classifier c=new Classifier();c.component=new object();
  for(int i=0;i<expected.Length;i++){c.type=i;if(c.Describe()!="label:"+expected[i])return 1;}
  c.type=-1;if(c.Describe()!="label:NONE[-1]")return 1;
  c.component=null;if(c.Describe()!="Classifier:component=")return 1;
  byte[][] data={new byte[]{1,7,8},new byte[]{2,0,0,9,10},new byte[]{0},new byte[]{3,0,255,192,0,11,0,0,9,0,12},new byte[]{3,0,0,1,0,4,0,0,255,192,0,7,0,0,10,0,13}};
  int[][] expectedSizes={new int[]{7,8},new int[]{9,10},new int[]{0,0},new int[]{12,9},new int[]{13,10}};
  Parser.Calls=0;
  for(int i=0;i<data.Length;i++){int[] result=Parser.Run(data[i]);if(result[0]!=expectedSizes[i][0]||result[1]!=expectedSizes[i][1])return 2;}
  if(Parser.Calls!=5)return 3;
  return 0;
 }
 public static float FloatNeg(float x){return -x;}
 public static float FloatZeroSub(float x){return 0f-x;}
}
