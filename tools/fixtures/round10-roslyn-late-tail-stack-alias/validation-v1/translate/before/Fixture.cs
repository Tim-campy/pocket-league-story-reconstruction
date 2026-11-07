using System.Text;using System.Text.RegularExpressions;
class Fixture {
 static string Replace(string s,string a,string b){return s.Replace(a,b);}static string _translateText(string s){return s;}
 static string[] Split(string s,string sep){return s.Split(new string[]{sep},System.StringSplitOptions.None);}
 public static string Run(string text,bool tabCheck=true) {
  text=Replace(text,"\\","¥");if(text.IndexOf('\b')!=-1){text=Replace(text,"\b","");return text;}
  if(text.IndexOf("<")!=-1){MatchCollection matchCollection=Regex.Matches(text,"<co=[0-9A-Z]+>");if(matchCollection.Count>0){StringBuilder stringBuilder=new StringBuilder();for(int i=0;i<matchCollection.Count;i++){Match match=matchCollection[i];if(i==0){stringBuilder.Append(text.Substring(0,match.Index));}stringBuilder.Append(match.Value.ToLower());int num=text.Length;if(i+1<matchCollection.Count){num=matchCollection[i+1].Index;}stringBuilder.Append(text.Substring(match.Index+match.Length,num-(match.Index+match.Length)));}text=stringBuilder.ToString();}}
  if(text.Contains("\t")) {
   string text2=Replace(text,"\t","¥t");
   string text3 = _translateText(text2);
   if (text2 != text3) {return Replace(text3,"¥t","\t");}
   string text4=Replace(text2,"\n","<br>");text3=_translateText(text4);if(text4!=text3){return Replace(text3,"¥t","\t");}
   if(tabCheck){string[] array=Split(text,"\t");text="";for(int j=0;j<array.Length;j++){if(j!=0){text+="\t";}text+=_translateText(array[j]);}return text;}
   return text;
  }
  string result = text;
  string text5=_translateText(text);if(text5!=text){return text5;}
  text=Replace(text,"\r\n","<br>");text=Replace(text,"\n","<br>");text5=_translateText(text);if(text5!=text){return text5;}return result;
 }
}
