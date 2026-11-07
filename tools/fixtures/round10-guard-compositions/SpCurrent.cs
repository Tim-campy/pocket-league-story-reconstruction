using System;
public class Fixture {
 string text_;
 public static class Config { public static bool PTW; }
 public static class Http { public static string GetErrorUrl(){return "a.b";} public static int GetErrorCode(){return 7;} }
 public Fixture(Exception th) {
  text_=null;
  if (th == null) { return; }
  if (Config.PTW) { text_=string.Empty; }
  if(Http.GetErrorUrl()!=null) {
   string text=Http.GetErrorUrl(); int index=text.Length;
   if(index>0) { text=text.Substring(0,index); }
   string text2 = text_;
   text_ = text2 + text + "," + Http.GetErrorCode() + "\n";
  }
  string full=string.Empty+th; string name=th.GetType().Name; int position=name.Length;
  if(position>0) { full=full+name; } text_+=full;
 }
}
