using System;
public class FormManagerBase{public void UnlockUpdate(){}}
public static class JSystem{static long time;public static long CurrentTimeMillis(){return ++time;}}
public class Property{public byte[] ToByteArray(){return new byte[2];}}
public struct AutoSaveLogData{public int[] DataIds;public int TryCount;public long LockTotalTime;}
public class Fixture{
 bool asRedo_,asCancel_,asLog_;object asThread_;Property asProperty_;Action asPreSaveMethod_;Action<byte[]> asPostSaveMethod_;
 Property MakeSaveData(Property value,out AutoSaveLogData log){log=new AutoSaveLogData{DataIds=new int[2]};return value;}
 FormManagerBase GetManager(){return new FormManagerBase();}void Write(string s){asLog_=s.Length!=0;}
 public void Run(){
 FormManagerBase formManager=GetManager();
 try{do{
 asRedo_=false;
 long num2;
 long num=JSystem.CurrentTimeMillis();
 if(asPreSaveMethod_!=null){asPreSaveMethod_();}
 if(asCancel_){break;}
 if(asRedo_){continue;}
 num2=JSystem.CurrentTimeMillis();
 AutoSaveLogData log;
 Property property=MakeSaveData(asProperty_,out log);
 long num3=JSystem.CurrentTimeMillis()-num2;
 if(asCancel_){break;}
 if(asRedo_){continue;}
 if(asPostSaveMethod_!=null){asPostSaveMethod_(property.ToByteArray());}
 if(asCancel_){break;}
 if(asRedo_ || !asLog_){continue;}
 string text="";
 for(int i=0;i<log.DataIds.Length;i++){text+=log.DataIds[i];}
 string text2="time "+num3+" "+(JSystem.CurrentTimeMillis()-num)+" "+log.TryCount;
 if(text.Length>0){text2+=text;}
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);
 Write(text2);

 }while(asRedo_);}
 catch(Exception ex){Write(ex.ToString());}
 finally{formManager.UnlockUpdate();asThread_=null;}
 }
}
