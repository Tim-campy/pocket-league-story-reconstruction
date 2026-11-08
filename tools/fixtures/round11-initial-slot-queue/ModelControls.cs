public static class ModelControls {
 public static string FourStrings(string x,string y){string s="base";s+=x+"a"+y;return s;}
 public static string FiveStrings(string x,string y,string z){string s="base";s+=x+"a"+y+z;return s;}
 public static string FourObjects(string x,int number){string s="base";s+=x+"a"+number;return s;}
 public static string AdjacentConstantFold(string x){string s="base";s+="a"+"b"+x+"c";return s;}
 public static string NestedParentheses(string x,string y){string s="base";s+=(x+"a")+(y+"b");return s;}
}
