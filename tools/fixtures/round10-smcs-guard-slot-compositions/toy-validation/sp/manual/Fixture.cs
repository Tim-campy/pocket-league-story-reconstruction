using System;
public class Store {
 public static Store Open(){return new Store();} public int Size(int n){return n;}
 public int Count(){return 3;} public void Close(){}
}
public class Commands {public void AddElement(string[] data){}}
public class Fixture {
 string[] strings = new string[30]; int[] ints = new int[30]; Commands commands_=new Commands(); int mode;
 static string V(int x){return x.ToString();} static int Read(int n){return n+4;}
 public void Run(int id, string str) {
 Store recordStore=null; int num=0;
 if(id>=0) {
  bool flag=id/10==1; int num2=id%10;
  string text=strings[(flag ? 6 : 2)+num2];
  string text2=strings[(flag ? 8 : 4)+num2];
  int num3=ints[(flag ? 11 : 6)+num2];
  if(num3==-1) { commands_.AddElement(new string[]{V(id),str}); }
  else {
   try { recordStore=Store.Open(); num=recordStore.Size((flag ? 3 : 1)+num2); }
   catch(Exception){} finally {if(recordStore!=null){try{recordStore.Close();}catch(Exception){}}}
   commands_.AddElement(new string[]{V(id),str,V(num),text,V(num3),text2});
  }
 } else {
  if(id==-1) {
   try { if(mode==0){num=Read(0);} else { recordStore=Store.Open(); int numRecords=recordStore.Count();
    for(int i=0;i<numRecords;i++){num+=recordStore.Size(i);} } }
   catch(Exception){} finally{if(recordStore!=null){try{recordStore.Close();}catch(Exception){}}}
   commands_.AddElement(new string[]{V(id),str,V(num)});
  } else if(id==-2) {
   try{num=Read(0)+Read(1);}catch(Exception){}
   commands_.AddElement(new string[]{V(id),str,V(num)});
  } else {commands_.AddElement(new string[]{V(id),str,V(-1)});}
 }
}
}