using System;
public class Item{public bool recovery_;public int value;}
public class Service{public static Service Get(){return new Service();} public void Save(){}}
public class Fixture {
 object gate=new object(); Item[] items=new Item[4]; bool broken;
 bool Consume(Item item){return item.value>0;} void Use(Item item){item.value++;}
 public bool Run(bool notice){
 lock(gate){int count=0;bool flag=false;bool flag2=notice;bool flag3=broken;
  for(int j=0;j<items.Length;j++){
   Item billingItem2=items[j];
   if(notice){
    try{Service instance2=Service.Get();instance2.Save();}catch(Exception){continue;}
    if (flag3) { continue; }
if (!billingItem2.recovery_) {
bool recoveredConsumeResult = Consume(billingItem2);
if (!recoveredConsumeResult) { continue; }
}
    billingItem2.recovery_=false;flag=true;
    Service instance3=Service.Get();instance3.Save();
   }
   Use(billingItem2);count++;
  }
  if(flag2){try{Service instance4=Service.Get();instance4.Save();}catch(Exception t2){Use(new Item());}}
  return flag && count!=0;
 }
}
}