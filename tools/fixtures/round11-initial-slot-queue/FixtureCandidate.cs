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
  string[] array; JarInflater jarInflater = null;
  try{RecordStore recordStore = RecordStore.OpenRecordStore(1);byte[] data=recordStore.ReadRecord(6);recordStore.CloseRecordStore();jarInflater=new JarInflater(data);
   array = StringUtil.LoadStringArray(jarInflater,"player.txt");
   for(int i=0;i<array.Length;i++)Trace+=array[i];
  }finally{if(jarInflater!=null)Calls++;}
 }
 public static void RunDraw(int state){
  string text; Canvas instance = Canvas.GetInstance(); Graphics graphics = instance.BeginPaint();
  if(state==0){text="A"; graphics.Use(text);}
  else if(state==1){text="B"; graphics.Use(text);text="C";graphics.Use(text);}
 }
 public class Purchase {public string productId_="p",orderId_="o"; public int state_=7;}
 public static void RunBilling(Purchase purchaseRecord){
  string text="s";
  text += purchaseRecord.productId_ + "," + purchaseRecord.orderId_ + "," + purchaseRecord.state_;
  for(int j=0;j<1;j++)Trace+=text+j;
  try{byte[] data=new byte[1];Trace+=data.Length;}catch(Exception ex){Trace+=ex.Message;}
 }
 public static class Http{public static int GetErrorCode(){Calls++;return 9;}}
 public static void RunSave(int type){
  string text8="s",text9="p";
  text8 += text9 + "," + Http.GetErrorCode() + "\n";
  string text="q"; byte[] array=null;
  if (type == 1) {text+="1";array=new byte[1];} else if (type == 2) {{byte[] array2=new byte[2];byte[] array3=new byte[3];array=new byte[array2.Length+array3.Length];}} else if (type == 0) {try{array=new byte[0];}catch(Exception ex){text+=ex.Message;}}
  if (type == 1) {text+="-sys";} else if (type == 2) {text+="-all";}
  Append(text8);Append(text);Append((array==null?-1:array.Length).ToString());
 }
 public static void RunPad(int id){
  string text="x"; CanvasLabels canvas_=new CanvasLabels();
  if (id == 0 || id == 5) {text="a";} else if (id == 1) {text="b";} else if (id == 2) {text="c";} else if (id == 3) {text="d";} else if (id == 11) {text="e";} else if (id == 12) {text="f";} else if (id == 13) {text="g";} else if (id == 14) {text="h";} else if (id == 15) {text="i";} else if (id == 16) {text="j";} else if (id == 17) {text="k";} else if (id == 18) {text="l";} else if (id == 19) {text="m";} else if (id == 20) {text="n";} else if (id == 4) {if(text == null){text=canvas_.GetSoftLabel(3);}}
  if (id == 21) {Trace+="u";} else if (id == 22) {Trace+="v";}
  if (id == 2) {Trace+="2";} else if (id == 3) {Trace+="3";} else if (id == 4) {Trace+="4";}
  Trace+=text;
 }
 public class CanvasLabels{public string GetSoftLabel(int i){Calls++;return "label";}}
 public static int SemanticChecks(){Trace="";Calls=0;RunBoot();RunDraw(0);RunDraw(1);RunBilling(new Purchase());RunSave(2);RunPad(2);return 0;}
 public static float FloatNeg(float a){return -a;}
 public static float FloatZeroSub(float a){return 0f-a;}
}
namespace kairo.unity.io {public class RecordStore {public static RecordStore OpenRecordStore(int n){return new RecordStore();} public byte[] ReadRecord(int n){return new byte[n];} public void CloseRecordStore(){} }}
namespace kairo.unity.ui {public class Graphics { public void Use(string s){Fixture.Trace+=s;} }}
