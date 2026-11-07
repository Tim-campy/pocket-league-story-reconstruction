using System.Text.RegularExpressions;
class Fixture {
 public static string Run(string text6) {
  while (true) {
   int length = text6.Length;
   text6 = Regex.Replace(text6, "^<po=[^>]+>|</po>$", "");
   if (length == text6.Length) { break; }
  }
  return text6;
 }
}
