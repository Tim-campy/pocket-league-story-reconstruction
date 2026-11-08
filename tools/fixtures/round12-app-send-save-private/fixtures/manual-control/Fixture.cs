using System;using System.Threading;
public class Save {public string[] strs=new string[11];}
public class Dialog {public static int Reply=8;public Dialog(int kind,string title){}public void SetText(string text){}public int Show(){return Reply;}}
public class Config {public static bool PRINT=true;}
public class Log {public static string Info(Exception e){return "log";}}
public class JThread {public static int Sleeps;public static void Sleep(long n){Sleeps++;}}
public class Canvas {public static int Clears;public int GetKeypadState(){return 0;}public void ClearKeyBuffer(){Clears++;}}
public class Fixture {
 public int sendState_=1,Draws,PostCalls,Done,Failures,ThrowDraw;public Save save_=new Save();
 void Draw(string s){Draws++;if(ThrowDraw>0){ThrowDraw--;throw new Exception();}}
 void Post(string url,string a,string b,string date,byte[] data){PostCalls++;if(Failures>0){Failures--;throw new Exception();}}
public void Run(){
 int rounds=0;Canvas instance=new Canvas();string text8="payload";
 while(true){
  if(rounds++>8)return;
  try {
   if(sendState_==0){Draw(text8);sendState_=1;}
   else if(sendState_==1){
    Draw(text8);byte[][] array4=new byte[2][];string[] array5=new string[2];string[] array6=new string[2];string date="date";string url="url";
    string str=((save_!=null)?save_.strs[0]:string.Empty)+"first";
    array4[0]=new byte[str.Length];
    str=((save_!=null)?save_.strs[0]:string.Empty)+"second";array4[1]=new byte[str.Length];
    for(int num7=0;num7<array4.Length;num7++){
     retryManual:
     Draw(text8);
     try{Post(url,array5[num7],array6[num7],date,array4[num7]);continue;}
     catch(Exception t){
      if(Config.PRINT){Log.Info(t);}
      Dialog dialog=new Dialog(3,"err");dialog.SetText("retry");
      if(dialog.Show()==8){
       while((instance.GetKeypadState()&0x100000)!=0){try{JThread.Sleep(100L);}catch(ThreadAbortException){}}
       sendState_=0;instance.ClearKeyBuffer();goto nextRoundManual;
      }
     }
     goto retryManual;
    }
    Done++;return;
   }
   JThread.Sleep(100L);
  }
  catch(Exception t2){
   if(Config.PRINT){Log.Info(t2);}
   Dialog dialog3=new Dialog(3,"outer");dialog3.SetText("retry");
   if(dialog3.Show()==8){
    while((instance.GetKeypadState()&0x100000)!=0){try{JThread.Sleep(100L);}catch(ThreadAbortException){}}
    sendState_=0;instance.ClearKeyBuffer();
   }
  }
  nextRoundManual:;
 }
}
 public static int SemanticChecks(){
  for(int mode=0;mode<5;mode++){
   JThread.Sleeps=0;Canvas.Clears=0;Dialog.Reply=mode==0?0:8;Fixture f=new Fixture();f.Failures=2;
   if(mode==2)f.ThrowDraw=1;if(mode==3)f.sendState_=2;if(mode==4)f.save_=null;
   f.Run();
   if(mode==0&&(f.Done!=1||f.PostCalls!=4||Canvas.Clears!=0||JThread.Sleeps!=0))return 1;
   if(mode==1&&(f.Done!=1||f.PostCalls!=4||Canvas.Clears!=2||JThread.Sleeps!=2))return 2;
   if(mode==2&&(f.Done!=1||Canvas.Clears!=3||JThread.Sleeps!=3))return 3;
   if(mode==3&&(f.Done!=0||f.PostCalls!=0||JThread.Sleeps!=9))return 4;
   if(mode==4&&(f.Done!=1||f.PostCalls!=4))return 5;
  }
  return 0;
 }
 public static float FloatNeg(float v){return -v;}public static float FloatZeroSub(float v){return 0f-v;}
}