using System.Text.RegularExpressions;

namespace system.billing.util
{
	public class Purchase
	{
		internal string mItemType;

		internal string mOrderId;

		internal string mPackageName;

		internal string mSku;

		internal long mPurchaseTime;

		internal int mPurchaseState;

		internal string mDeveloperPayload;

		internal string mToken;

		internal string mOriginalJson;

		internal string mSignature;

		public Purchase()
		{
		}

		public Purchase(string itemType, string jsonPurchaseInfo, string signature)
		{
			mItemType = itemType;
			mOriginalJson = jsonPurchaseInfo;
			mOrderId = ReadJSon(mOriginalJson, "orderId");
			mPackageName = ReadJSon(mOriginalJson, "packageName");
			mSku = ReadJSon(mOriginalJson, "productId");
			mPurchaseTime = long.Parse(ReadJSon(mOriginalJson, "purchaseTime"));
			mPurchaseState = int.Parse(ReadJSon(mOriginalJson, "purchaseState"));
			mDeveloperPayload = ReadJSon(mOriginalJson, "developerPayload");
			mToken = ReadJSon(mOriginalJson, "token");
			if (mToken == null)
			{
				mToken = ReadJSon(mOriginalJson, "purchaseToken");
			}
			if (itemType == "subs" && (mOrderId == null || mOrderId.Length == 0) && mToken != null)
			{
				mOrderId = mToken;
				int num = mOrderId.IndexOf('.');
				if (num != -1)
				{
					mOrderId = mOrderId.Substring(0, num);
				}
			}
			mSignature = signature;
		}

		public string GetItemType()
		{
			return mItemType;
		}

		public string GetOrderId()
		{
			return mOrderId;
		}

		public string GetPackageName()
		{
			return mPackageName;
		}

		public string GetSku()
		{
			return mSku;
		}

		public long GetPurchaseTime()
		{
			return mPurchaseTime;
		}

		public int GetPurchaseState()
		{
			return mPurchaseState;
		}

		public string GetDeveloperPayload()
		{
			return mDeveloperPayload;
		}

		public string GetToken()
		{
			return mToken;
		}

		public string GetOriginalJson()
		{
			return mOriginalJson;
		}

		public string GetSignature()
		{
			return mSignature;
		}

		private static string ReadJSon(string json, string key)
		{
			if (key == null || json == null)
			{
				return null;
			}
			Match match = Regex.Match(json, "\"<key>\"\\s*:\\s*(\"[^\"]*\"|[0-9.]*)".Replace("<key>", key));
			if (match.Success)
			{
				return match.Groups[1].Value.Trim('"');
			}
			return null;
		}

		public override string ToString()
		{
			return "PurchaseInfo(type:" + mItemType + "):" + mOriginalJson;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
