using System;
public class Fixture {
 long[] values=new long[4];string[] labels=new string[2];
 byte[] Fetch(){return new byte[2];} string Decode(byte[] input){return "1,2";}
 string[] Split(string text){return text.Split(',');} long Parse(string text){return long.Parse(text);}
 void Save(){}
 public void Run(){
 string unused;byte[] bytes=Fetch();string text=Decode(bytes);string[] array=Split(text);
 for(int i=0;i<array.Length;i++){
  if(array[i].Length==0||Parse(array[i])==0L){
   if(i==0){values[0]=0L;values[1]=0L;}else if(i==1){values[2]=5000L;values[3]=0L;}
   continue;
  }
  long num2=Parse(array[i]);
  if(i==0){values[0]=num2;values[1]=num2;labels[0]="old";}
  else if(i==1){values[2]=num2;values[3]=num2;labels[1]="old";}
 }
 Save();
}
}