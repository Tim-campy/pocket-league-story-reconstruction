using System.Text.RegularExpressions;
class Fixture {
 public static string Run(string text6) {
  int length;
  do {
   length = text6.Length;
   text6 = Regex.Replace(text6, "^<po=[^>]+>|</po>$", "");
  } while (length != text6.Length);
  return text6;
 }
}
