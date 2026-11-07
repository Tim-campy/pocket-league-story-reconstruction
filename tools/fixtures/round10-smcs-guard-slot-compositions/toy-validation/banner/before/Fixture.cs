using System;
public class Jar{public long GetSize(string value){return value.Length;}public void Close(){}}
public class Fixture{
 static object image_;int current_;static string[] FILENAMES=new string[]{"x","y","z"};
 void Use(int n){} object Item(int n){return new object();}
 public void Run(){
 Jar jarInflater=null;
 try{
  jarInflater=new Jar();
  for(int i=0;i<FILENAMES.Length;i++){
   {bool recoveredCondition_4093=(image_ == null && i != 0) || i == current_ || jarInflater.GetSize(FILENAMES[i]) == -1;
    if(recoveredCondition_4093){continue;}}
   if(i==1){bool flag=true;object a=Item(i);object b=Item(i+1);
    for(int j=0;j<2;j++){object f=Item(j);if(f==a||f==b){flag=false;}}
    if(!flag){continue;}
   }
   Use(i);
  }
  int index=1;Use(index);
 }catch(Exception){Use(-1);}finally{if(jarInflater!=null){jarInflater.Close();}jarInflater=null;}
}
}