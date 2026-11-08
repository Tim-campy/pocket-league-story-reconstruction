using System;
public static class Application { public static int reads;public static string input="1.2.3";public static string unityVersion{get{reads++;return input;}} }
public static class Fixture {
 static int unityVarsion_=int.MaxValue;
 public static int Run() {
  if(unityVarsion_==int.MaxValue) {
   string text="";
   string unityVersion=Application.unityVersion;
   for(int i=0;i<unityVersion.Length;i++) {
    if(unityVersion[i]!='.') {
     int result=0;
     if(!int.TryParse((Application.unityVersion[i] + ""),out result)) {break;}
     text+=result;
    }
   }
   unityVarsion_=int.Parse(text);
  }
  return unityVarsion_;
 }
 public static int SemanticChecks() {
  unityVarsion_=int.MaxValue;Application.input="1.2.3";Application.reads=0;
  if(Run()!=123 || Application.reads!=4)return 1;
  if(Run()!=123 || Application.reads!=4)return 2;
  unityVarsion_=int.MaxValue;Application.input="7.x";Application.reads=0;
  if(Run()!=7 || Application.reads!=3)return 3;
  return 0;
 }
 public static float FloatNeg(float v){return -v;} public static float FloatZeroSub(float v){return 0f-v;}
}