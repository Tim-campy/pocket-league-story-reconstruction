using System.Collections.Generic;
using System.Linq;

namespace system.billing.util
{
	public class Inventory
	{
		internal Dictionary<string, SkuDetails> mSkuMap = new Dictionary<string, SkuDetails>();

		internal Dictionary<string, Purchase> mPurchaseMap = new Dictionary<string, Purchase>();

		internal Inventory()
		{
		}

		public SkuDetails GetSkuDetails(string sku)
		{
			SkuDetails value;
			mSkuMap.TryGetValue(sku, out value);
			return value;
		}

		public Purchase GetPurchase(string sku)
		{
			Purchase value;
			mPurchaseMap.TryGetValue(sku, out value);
			return value;
		}

		public bool HasPurchase(string sku)
		{
			return mPurchaseMap.ContainsKey(sku);
		}

		public bool HasDetails(string sku)
		{
			return mSkuMap.ContainsKey(sku);
		}

		public void ErasePurchase(string sku)
		{
			if (mPurchaseMap.ContainsKey(sku))
			{
				mPurchaseMap.Remove(sku);
			}
		}

		internal List<string> GetAllOwnedSkus()
		{
			return mPurchaseMap.Keys.ToList();
		}

		internal List<string> GetAllOwnedSkus(string itemType)
		{
			List<string> list = new List<string>();
			foreach (Purchase value in mPurchaseMap.Values)
			{
				if (value.GetItemType().Equals(itemType))
				{
					list.Add(value.GetSku());
				}
			}
			return list;
		}

		internal List<Purchase> GetAllPurchases()
		{
			return mPurchaseMap.Values.ToList();
		}

		internal void AddSkuDetails(SkuDetails d)
		{
			mSkuMap[d.GetSku()] = d;
		}

		internal void AddPurchase(Purchase p)
		{
			mPurchaseMap[p.GetSku()] = p;
		}
	}
}
