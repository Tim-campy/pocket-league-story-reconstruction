using System;
public class Store { public int[] ints=new int[20];public void SaveSystem(){}public static Store GetInstance(){return new Store();} }
public class Fixture {public bool visibleTouchRange_,hide_;public int sink;void Use(int n){sink=n;}
public void Run(int id,int type){switch(id){
case 14:
 if(type==5){visibleTouchRange_ = !visibleTouchRange_;}break;
case 15:
 if(type==0){Use(15);}break;
case 21:case 22:
 if(type==5){
  Store instance2=Store.GetInstance();
  int num = ((id == 21) ? 0 : 1);
  Use(num);instance2.ints[15]=num;
  try{instance2.SaveSystem();}catch(Exception){}
 }break;
case 5:
 hide_=!hide_;break;
}
Use(100);}
}