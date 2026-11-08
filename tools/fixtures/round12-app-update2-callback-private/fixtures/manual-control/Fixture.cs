public delegate void NameChanged(string text);
public class TeamData{public string name_,name2_;}
public class TitleForm{public string newGameName_;}
public class AppData{public TitleForm frmTitle_;}
public static class JString{public static string New(string value){return value;}public static string ValueOf(char value){return value.ToString();}public static int Length(this string value){return value.Length;}public static char CharAt(this string value,int index){return value[index];}}
public static class MyFormBase{public static bool locale;public static bool Jp(){return locale;}}
public class Fixture{public AppData apdat_;public static int GetStringLength(string text){return text.Length;}
public NameChanged Run(){TeamData t_data=new TeamData();return delegate(string temp_name){
 if(temp_name!=null){
  t_data.name_=JString.New(temp_name);
  apdat_.frmTitle_.newGameName_=JString.New(temp_name);
  int unusedWidth;
  string shortName=string.Empty;
  int consumed=0;
  int limit=0;
  for(int index=0;index<temp_name.Length();index++){
   char value=temp_name.CharAt(index);
   int width=GetStringLength(JString.ValueOf(value));
   if(index==0){
    if(MyFormBase.Jp()){
     if(width==1){limit=5;}else{limit=4;}
    }else{limit=8;}
   }
   consumed+=width;
   if(consumed>limit){break;}
   shortName+=JString.ValueOf(value);
  }
  t_data.name2_=shortName;
 }
};}
}