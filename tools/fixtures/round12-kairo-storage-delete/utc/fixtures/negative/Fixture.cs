using System;using System.Collections.Generic;using UnityEngine;
public static class Config{public static int APPLI_TYPE;public static bool UNITY_EDITOR;}
public class Ui{public static Ui GetCurrentApp(){return null;}public object RunOnUiThread(Func<object> action,bool wait){return null;}}
public static class Fixture{static AndroidJavaClass TimeZone;static bool init_;static Dictionary<string,int> utcOffsetTable_=new Dictionary<string,int>();
static void Init(){init_=true;}static void CreateUtcOffsetTable(){}
public static int Run(string timezone){return(int)Ui.GetCurrentApp().RunOnUiThread(delegate{int misplaced=0;
if(!init_){Init();}
if(Config.APPLI_TYPE==2&&!Config.UNITY_EDITOR){
 using (AndroidJavaObject androidJavaObject=TimeZone.CallStatic<AndroidJavaObject>("getTimeZone",new object[1]{timezone})){
  return androidJavaObject.Call<int>("getRawOffset",new object[0]);
 }
}
{
 if(utcOffsetTable_.Count==0){CreateUtcOffsetTable();}
 utcOffsetTable_.TryGetValue(timezone,out misplaced);return misplaced;
}},true);}}
