using system.billing.util;using system.billing.ui;
namespace system.billing.util{public class Purchase{public string GetOrderId(){return null;}}}
namespace system.billing.ui{public class PurchaseRecord{public string productId_,orderId_;public PurchaseRecord(string id,string order){}public void ChangeState(int state,Purchase purchase){}}}
public class Vec{public int Size(){return 0;}public object ElementAt(int index){return null;}public void AddElement(object item){}}
public class Fixture{public Vec purchaseLog_;
public void Run(string productId,int state,Purchase purchase){
			PurchaseRecord purchaseRecord = null;
			string text = null;
			if (purchase != null)
			{
				text = purchase.GetOrderId();
			}
			for (int num = purchaseLog_.Size() - 1; num >= 0; num--)
			{
				PurchaseRecord purchaseRecord2 = (PurchaseRecord)purchaseLog_.ElementAt(num);
				if ((!(purchaseRecord2.productId_.Equals(productId))) || ((text != null) && (!(purchaseRecord2.orderId_.Equals(text))))) { continue; }
purchaseRecord = purchaseRecord2;
break;
			}
			if (purchaseRecord == null && text != null)
			{
				purchaseRecord = new PurchaseRecord(productId, text);
				purchaseLog_.AddElement(purchaseRecord);
			}
			if (purchaseRecord != null)
			{
				purchaseRecord.ChangeState(state, purchase);
			}
		}
}