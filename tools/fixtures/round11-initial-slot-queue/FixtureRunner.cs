using System;
using System.Reflection;
public static class FixtureRunner {
 public static int Main(string[] args){
  Type type=Assembly.LoadFile(System.IO.Path.GetFullPath(args[0])).GetType("Fixture");
  type.GetField("Trace").SetValue(null,"");type.GetField("Calls").SetValue(null,0);
  type.GetMethod("RunBoot").Invoke(null,null);
  for(int i=-1;i<=2;i++)type.GetMethod("RunDraw").Invoke(null,new object[]{i});
  Type purchase=type.GetNestedType("Purchase");type.GetMethod("RunBilling").Invoke(null,new object[]{Activator.CreateInstance(purchase)});
  for(int i=-1;i<=3;i++)type.GetMethod("RunSave").Invoke(null,new object[]{i});
  for(int i=-1;i<=23;i++)type.GetMethod("RunPad").Invoke(null,new object[]{i});
  Console.WriteLine(type.GetField("Calls").GetValue(null));
  Console.WriteLine(type.GetField("Trace").GetValue(null));return 0;
 }
}
