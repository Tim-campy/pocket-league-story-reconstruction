using System;
using kairo.unity.io;
using kairo.unity.ui;
public static class Fixture {
 public class JarInflater {public JarInflater(byte[] data){} }
 public class StringUtil {public static string[] LoadStringArray(JarInflater j,string s){return new[]{s};}}
 public class Canvas {public static Canvas GetInstance(){return new Canvas();}public Graphics BeginPaint(){return new Graphics();}}
 public static void Append(string s){Trace+=s;}
 public static string Trace; public static int Calls;
 public static void RunBoot(){
  global::kairo.unity.io.RecordStore recordStore; JarInflater jarInflater = null;
  try{recordStore = RecordStore.OpenRecordStore(1);byte[] data=recordStore.ReadRecord(6);recordStore.CloseRecordStore();jarInflater=new JarInflater(data);
   string[] array = StringUtil.LoadStringArray(jarInflater,"player.txt");
   for(int i=0;i<array.Length;i++)Trace+=array[i];
  }finally{if(jarInflater!=null)Calls++;}
 }
 public static void RunDraw(int state){
  global::kairo.unity.ui.Graphics graphics; Canvas instance = Canvas.GetInstance(); graphics = instance.BeginPaint();
  if(state==0){string text="A"; graphics.Use(text);}
  else if(state==1){string text="B"; graphics.Use(text);text="C";graphics.Use(text);}
 }
 public class Purchase {public string productId_="p",orderId_="o"; public int state_=7;}
 public static void RunBilling(Purchase purchaseRecord){
  string text="s";
  string text2 = text; text = text2 + purchaseRecord.productId_ + "," + purchaseRecord.orderId_ + "," + purchaseRecord.state_;
  for(int j=0;j<1;j++)Trace+=text+j;
  try{byte[] data=new byte[1];Trace+=data.Length;}catch(Exception ex){Trace+=ex.Message;}
 }
 public static class Http{public static int GetErrorCode(){Calls++;return 9;}}
 public static void RunSave(int type){
  string text8="s",text9="p";
  string text10 = text8; text8 = text10 + text9 + "," + Http.GetErrorCode() + "\n";
  string text="q"; byte[] array=null;
  switch(type){case 1:text+="1";array=new byte[1];break;case 2:{byte[] array2=new byte[2];byte[] array3=new byte[3];array=new byte[array2.Length+array3.Length];break;}case 0:try{array=new byte[0];}catch(Exception ex){text+=ex.Message;}break;}
  switch(type){case 1:text+="-sys";break;case 2:text+="-all";break;}
  Append(text8);Append(text);Append((array==null?-1:array.Length).ToString());
 }
 public static void RunPad(int id){
  string text="x"; CanvasLabels canvas_=new CanvasLabels();
  switch(id){case 0:case 5:text="a";break;case 1:text="b";break;case 2:text="c";break;case 3:text="d";break;
   case 11:text="e";break;case 12:text="f";break;case 13:text="g";break;case 14:text="h";break;case 15:text="i";break;case 16:text="j";break;case 17:text="k";break;case 18:text="l";break;case 19:text="m";break;case 20:text="n";break;
   case 4:{ bool recoveredCondition_12162 = text == null; if (recoveredCondition_12162) { text = canvas_.GetSoftLabel(3); } }break;}
  switch(id){case 21:Trace+="u";break;case 22:Trace+="v";break;}
  switch(id){case 2:Trace+="2";break;case 3:Trace+="3";break;case 4:Trace+="4";break;}
  Trace+=text;
 }
 public class CanvasLabels{public string GetSoftLabel(int i){Calls++;return "label";}}
 public static int SemanticChecks(){Trace="";Calls=0;RunBoot();RunDraw(0);RunDraw(1);RunBilling(new Purchase());RunSave(2);RunPad(2);return 0;}
 public static float FloatNeg(float a){return -a;}
 public static float FloatZeroSub(float a){return 0f-a;}
}
namespace kairo.unity.io {public class RecordStore {public static RecordStore OpenRecordStore(int n){return new RecordStore();} public byte[] ReadRecord(int n){return new byte[n];} public void CloseRecordStore(){} }}
namespace kairo.unity.ui {public class Graphics { public void Use(string s){Fixture.Trace+=s;} }}
