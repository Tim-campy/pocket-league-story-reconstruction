using System.Text.RegularExpressions;

namespace system.billing.util
{
	public class SkuDetails
	{
		internal string mItemType;

		internal string mSku;

		internal string mType;

		internal string mPrice;

		internal string mTitle;

		internal string mDescription;

		internal string mJson;

		public SkuDetails()
		{
		}

		public SkuDetails(string jsonSkuDetails)
			: this("inapp", jsonSkuDetails)
		{
		}

		public SkuDetails(string itemType, string jsonSkuDetails)
		{
			mItemType = itemType;
			mJson = jsonSkuDetails;
			mSku = ReadJSon(mJson, "productId");
			mType = ReadJSon(mJson, "type");
			mPrice = ReadJSon(mJson, "price");
			mTitle = ReadJSon(mJson, "title");
			mDescription = ReadJSon(mJson, "description");
		}

		public string GetSku()
		{
			return mSku;
		}

		public string GetTypeA()
		{
			return mType;
		}

		public string GetPrice()
		{
			return mPrice;
		}

		public string GetTitle()
		{
			return mTitle;
		}

		public string GetDescription()
		{
			return mDescription;
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
			return "SkuDetails:" + mJson;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
