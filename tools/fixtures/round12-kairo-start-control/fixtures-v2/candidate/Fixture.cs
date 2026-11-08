using System;
public enum NetworkReachability {NotReachable=0,Reachable=1}
public static class Application {static NetworkReachability state;public static NetworkReachability internetReachability{get{return state;}}}
public static class Language {static bool mode;public static bool Japanese(){return mode;}}
public static class JThread {public static void Sleep(long value){}}
public class Dialog {public Dialog(int kind,string title){}public void SetText(string text){}public void SetButtons(string[] text){}public int Show(){return 4;}}
public class Fixture {
 Func<object> action;public void Save(Func<object> value){action=value;}
 public void Run(){Save(delegate{
 int num=0;while (true){try{
  if(Application.internetReachability!=NetworkReachability.NotReachable){return true;}
  string title=Language.Japanese()?"JP-title":"EN-title";
  string text=Language.Japanese()?"JP-text":"EN-text";
  string text2=Language.Japanese()?"JP-retry,cancel":"EN-retry,cancel";
  if(num>0){JThread.Sleep(500L);}
  Dialog dialog=new Dialog(3,title);dialog.SetText(text);dialog.SetButtons(text2.Split(','));
  if (dialog.Show() == 4) { num++; continue; } return false;
 }catch{return true;}}
 });}
}