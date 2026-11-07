static class CharExt { public static char CharAt(this string s,int n){return s[n];} }
class Fixture {
 public static string Run(string text,int num,int num2) {
  string text2=text; bool flag=true; int num3=num2-1;
  while (num3 >= num && text.CharAt(num3) == '\\') { flag=!flag;num3--; }
  int i;
  for (i = 0; i < text2.Length && text2[i] <= ' '; i++) { }
  int num4=text2.Length-1;
  while (num4 >= 0 && text2[num4] <= ' ') { num4--; }
  text2 = ((num4 - i >= 0) ? text2.Substring(i, num4 - i + 1) : "");
  return flag ? text2 : "!"+text2;
 }
}
