static class CharExt { public static char CharAt(this string s,int n){return s[n];} }
class Fixture {
 public static string Run(string text,int num,int num2) {
  string text2=text; bool flag=true; int num3=num2-1;
  while (num3 >= num) { if (text.CharAt(num3) != '\\') { break; } flag=!flag;num3--; }
  int i;
  for (i = 0; i < text2.Length; i++) { if (text2[i] > ' ') { break; } }
  int num4=text2.Length-1;
  while (num4 >= 0) { if (text2[num4] > ' ') { break; } num4--; }
  if (num4 - i < 0) { text2 = ""; } else { text2 = text2.Substring(i, num4 - i + 1); }
  return flag ? text2 : "!"+text2;
 }
}
