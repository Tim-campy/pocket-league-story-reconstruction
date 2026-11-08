public struct Converted {
 public int value;public static int trace;
 public static implicit operator int(Converted a){trace=trace*10+a.value;return a.value;}
}
public struct Overloaded {
 public static bool operator <(Overloaded a,Overloaded b){return true;}
 public static bool operator >(Overloaded a,Overloaded b){return false;}
}
public static class Fixture {
 public static bool Pair(Converted a,Converted b){return a<b;}
 public static bool Literal(Converted a){return a<0;}
 public static bool Operator(Overloaded a,Overloaded b){return a<b;}
 public static int SemanticChecks(){Converted a=new Converted();a.value=1;Converted b=new Converted();b.value=2;Converted.trace=0;bool yes=Pair(a,b);return yes&&Converted.trace==12?0:1;}
}
