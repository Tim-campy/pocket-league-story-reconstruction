using System.Collections.Generic;
using System.Text.RegularExpressions;
using kairo.unity.util;
using UnityEngine.Purchasing;

namespace system.billing.util
{
	internal class Json
	{
		internal static class JsonParam
		{
			public const string ORDER_ID = "orderId";

			public const string PACKAGE_NAME = "packageName";

			public const string PRODUCT_ID = "productId";

			public const string PURCHASE_TIME = "purchaseTime";

			public const string PURCHASE_STATE = "purchaseState";

			public const string DEVELOPER_PAYLOAD = "developerPayload";

			public const string TOKEN = "token";

			public const string PURCHASE_TOKEN = "purchaseToken";

			public const string SIGNATURE = "signature";
		}

		public static string ReadJSon(string json, string key)
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
			match = Regex.Match(json, "\"<key>\"\\s*=\\s*(\"[^\"]*\"|[0-9.]*)".Replace("<key>", key));
			if (match.Success)
			{
				return match.Groups[1].Value.Trim('"');
			}
			return null;
		}

		public static string GetDetailJson(Product p)
		{
			return null;
		}

		private static string Record(string type, string value)
		{
			return "\"" + type + "\":\"" + StringUtil.Escape(value) + "\"";
		}

		private static string MakeJson(string[] records)
		{
			string text = "{";
			for (int i = 0; i < records.Length; i++)
			{
				text += records[i];
				if (i != records.Length - 1)
				{
					text += ",";
				}
			}
			return text + "}";
		}

		public static string AdjustJson(Product p)
		{
			return null;
		}

		public static string AdjustJson(string receipt, string productID, string iOSPurchaseTime = "0")
		{
			string json = receipt.Replace("\\", string.Empty);
			string[][] array = new string[9][]
			{
				new string[2]
				{
					"orderId",
					string.Empty
				},
				new string[2]
				{
					"packageName",
					string.Empty
				},
				new string[2]
				{
					"productId",
					string.Empty
				},
				new string[2]
				{
					"purchaseTime",
					string.Empty
				},
				new string[2]
				{
					"purchaseState",
					string.Empty
				},
				new string[2]
				{
					"developerPayload",
					string.Empty
				},
				new string[2]
				{
					"token",
					string.Empty
				},
				new string[2]
				{
					"purchaseToken",
					string.Empty
				},
				new string[2]
				{
					"signature",
					string.Empty
				}
			};
			foreach (string[] array2 in array)
			{
				array2[1] = ReadJSon(json, array2[0]);
			}
			List<string> list = new List<string>();
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j][1] != null && array[j][1].Length > 0)
				{
					list.Add(Record(array[j][0], array[j][1]));
				}
			}
			return MakeJson(list.ToArray());
		}
	}
}
