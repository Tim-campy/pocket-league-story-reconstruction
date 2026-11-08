using system.billing.util;using system.billing.ui;
namespace system.billing.util{public class Purchase{public string GetOrderId(){return null;}}}
namespace system.billing.ui{public class PurchaseRecord{public string productId_,orderId_;public PurchaseRecord(string id,string order){}public void ChangeState(int state,Purchase purchase){}}}
public class Vec{public int Size(){return 0;}public object ElementAt(int index){return null;}public void AddElement(object item){}}
public class Fixture{public Vec purchaseLog_;
public void Run(string productId,int state,Purchase purchase){PurchaseRecord found=null;
string order=null;
if(purchase!=null){order=purchase.GetOrderId();}
for(int entry=purchaseLog_.Size()-1;entry>=0;entry--){
 PurchaseRecord record=(PurchaseRecord)purchaseLog_.ElementAt(entry);
 if(!record.productId_.Equals(productId)){continue;}
 if(order==null||record.orderId_.Equals(order)){found=record;break;}
}
if(found==null&&order!=null){found=new PurchaseRecord(productId,order);purchaseLog_.AddElement(found);}
if(found!=null){found.ChangeState(state,purchase);}}
}