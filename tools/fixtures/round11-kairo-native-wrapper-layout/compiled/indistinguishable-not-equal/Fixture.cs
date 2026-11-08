using System;
public static class Fixture {
 public delegate object UiJob();
 public sealed class App {public int Calls;public object RunOnUiThread(UiJob job,bool wait){Calls++;return job();}public string GetDataPath(){return "/data";}}
 public static App app_=new App();
 public sealed class AndroidJavaClass {
  public static int StringReads;
  public AndroidJavaClass(string name){}
  public T GetStatic<T>(string name){if(typeof(T)==typeof(string)){StringReads++;return (T)(object)"MAIN";}return (T)(object)(name=="FLAG_ACTIVITY_NEW_TASK"?1:2);}
 }
 public sealed class AndroidJavaObject {
  public string Action;public int Calls;
  public AndroidJavaObject(string name,params object[] args){Action=(string)args[0];}
  public T Call<T>(string name,params object[] args){Calls++;return (T)(object)this;}
 }
 public sealed class JDate {public static int Count;public JDate(){Count++;}}
 public sealed class JCalendar {public static JCalendar GetInstance(){return new JCalendar();}public int Get(int n){return n==1?2026:(n==2?0:(n==5?7:n-10));}}
 public static class Directory {public static int Creates;public static bool Exists(string p){return false;}public static object CreateDirectory(string p){Creates++;return new object();}}
 public static class File {public static int Writes;public static string Last;public static void WriteAllBytes(string p,byte[] data){Writes++;Last=p;}}
 public static class Log {public static int Calls;public static object Info(string s){Calls++;return null;}}
 public static AndroidJavaObject BuildLobi(string data,int type,int number){
  return (AndroidJavaObject)app_.RunOnUiThread(delegate {
   if(!kairo.common.cfg.Config.UNITY_EDITOR&&kairo.common.cfg.Config.APPLI_TYPE==2){
    AndroidJavaClass androidJavaClass=new AndroidJavaClass("android.content.Intent");
    AndroidJavaClass androidJavaClass2=new AndroidJavaClass("android.support.v4.content.IntentCompat");
    AndroidJavaObject androidJavaObject;
if(type==0){androidJavaObject=new AndroidJavaObject("android.content.Intent","net.kairosoft.android.LOBI_CHAT");}
else if(type==1){androidJavaObject=new AndroidJavaObject("android.content.Intent","net.kairosoft.android.LOBI_RANKING");}
else{androidJavaObject=new AndroidJavaObject("android.content.Intent",androidJavaClass.GetStatic<string>("ACTION_MAIN"));}
    androidJavaObject.Call<AndroidJavaObject>("setClassName",new object[2]{"net.kairosoft.android.kairoclub","net.kairosoft.android.kairoclub.LobiMain"});
    androidJavaObject.Call<AndroidJavaObject>("setFlags",new object[1]{androidJavaClass.GetStatic<int>("FLAG_ACTIVITY_NEW_TASK")|androidJavaClass2.GetStatic<int>("FLAG_ACTIVITY_CLEAR_TASK")});
    androidJavaObject.Call<AndroidJavaObject>("putExtra",new object[2]{"net.kairosoft.android.LobiDataVersion",100});
    androidJavaObject.Call<AndroidJavaObject>("putExtra",new object[2]{"net.kairosoft.android.LobiData",data});
    return androidJavaObject;
   }
   return (object)null;
  },true);
 }
 public static bool SaveOwn(string file,byte[] data,out string save){
  save=(string)app_.RunOnUiThread(delegate {
   new JDate();JCalendar instance=JCalendar.GetInstance();
   string text=$"{instance.Get(1):D4}{instance.Get(2)+1:D2}{instance.Get(5):D2}-{instance.Get(11):D2}{instance.Get(12):D2}{instance.Get(13):D2}";
   if(!file.Contains(".")){file=text+"."+file;}
   file=file.Replace("{date}",text);
   if (kairo.common.cfg.Config.UNITY_EDITOR) {
string text2 = app_.GetDataPath() + "/tmp/capture";
if (!Directory.Exists(text2)) { Directory.CreateDirectory(text2); }
File.WriteAllBytes(text2 + "/" + file, data);
Log.Info(text2 + "/" + file);
} else { if(kairo.common.cfg.Config.APPLI_TYPE!=2){} return (object)null; }
return (object)null;
  },true);
  return save!=null;
 }
 public static int SemanticChecks(){
  for(int editor=0;editor<2;editor++)for(int platform=0;platform<3;platform++){
   kairo.common.cfg.Config.UNITY_EDITOR=editor==1;kairo.common.cfg.Config.APPLI_TYPE=platform;
   for(int type=-1;type<=2;type++){
    AndroidJavaClass.StringReads=0;int before=app_.Calls;AndroidJavaObject result=BuildLobi("payload",type,123);
    if(app_.Calls!=before+1)return 1;
    if(editor==0&&platform==2){string action=type==0?"net.kairosoft.android.LOBI_CHAT":(type==1?"net.kairosoft.android.LOBI_RANKING":"MAIN");if(result==null||result.Action!=action||result.Calls!=4||AndroidJavaClass.StringReads!=((type==0||type==1)?0:1))return 1;}
    else if(result!=null)return 1;
   }
   string save;File.Writes=0;Directory.Creates=0;Log.Calls=0;JDate.Count=0;
   if(SaveOwn("png",new byte[]{1,2},out save)||save!=null)return 2;
   if(JDate.Count!=1||File.Writes!=editor||Directory.Creates!=editor||Log.Calls!=editor)return 2;
   if(editor==1&&File.Last!="/data/tmp/capture/20260107-010203.png")return 2;
  }
  return 0;
 }
 public static float FloatNeg(float x){return -x;}
 public static float FloatZeroSub(float x){return 0f-x;}
}
namespace kairo.common.cfg {public static class Config {public static bool UNITY_EDITOR;public static int APPLI_TYPE;}}
