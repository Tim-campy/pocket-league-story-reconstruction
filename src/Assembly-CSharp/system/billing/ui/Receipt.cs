using System.Text;
using java.io;
using java.lang;
using kairo.common.cfg;
using kairo.unity.util;
using system.billing.util;
using UnityEngine.Purchasing;

namespace system.billing.ui
{
	public class Receipt
	{
		private enum State
		{
			None,
			Consume
		}

		private SecureString productID_;

		private SecureString receipt_;

		private SecureInt state_;

		private ProductType productType_;

		internal bool paymentComplete_;

		internal SecureLong purchaseTime_;

		private static byte[] PASSKEY = new byte[6] { 8, 64, 52, 1, 100, 3 };

		private int IsTicketProductTmp_ = -1;

		private Receipt()
		{
		}

		public Receipt(Product p, string receipt = null)
		{
		}

		public override bool Equals(object o)
		{
			if (!(o is Receipt))
			{
				return false;
			}
			if (o == this)
			{
				return true;
			}
			Receipt receipt = (Receipt)o;
			return productID_ == receipt.productID_ && receipt_ == receipt.receipt_;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Copy(Receipt r)
		{
			productID_ = r.productID_.Get();
			receipt_ = r.receipt_.Get();
			state_ = r.state_.Get();
			productType_ = r.productType_;
		}

		public Purchase GetPurchase()
		{
			string json = GetJson();
			return new Purchase(GetItemType(productType_), json, Json.ReadJSon(json, "signature"));
		}

		public void Consume()
		{
			if (productType_ == ProductType.Consumable)
			{
				state_ = 1;
			}
		}

		public bool IsConsume()
		{
			if (productType_ != ProductType.Consumable)
			{
				return false;
			}
			return (int)state_ == 1;
		}

		public string GetJson()
		{
			return Json.AdjustJson(GetReceipt(), GetProductID(), string.Empty + purchaseTime_);
		}

		public string GetJsonParam(string tag)
		{
			return Json.ReadJSon(GetJson(), tag);
		}

		public string GetReceipt()
		{
			return PasswordDecodeToString(receipt_);
		}

		public string GetProductID()
		{
			return PasswordDecodeToString(productID_);
		}

		public ProductType GetProductType()
		{
			return productType_;
		}

		public string GetOrderId()
		{
			return GetJsonParam("orderId");
		}

		public bool IsTicketProduct()
		{
			if (IsTicketProductTmp_ == -1)
			{
				IsTicketProductTmp_ = 0;
				if (GetProductID().Contains(StoreBase.PAYMENT_PRODUCT))
				{
					IsTicketProductTmp_ = 1;
				}
			}
			return IsTicketProductTmp_ == 1;
		}

		public void DebugVerificationReceipt()
		{
			DebugVerificationReceipt(Json.ReadJSon(GetJson(), "token"));
		}

		public static void DebugVerificationReceipt(string token)
		{
			if (Config.DEBUG)
			{
			}
		}

		public void Save(OutputStream stream)
		{
			StreamUtil.WriteString(stream, productID_);
			StreamUtil.WriteString(stream, receipt_);
			StreamUtil.WriteInt(stream, state_);
			StreamUtil.WriteInt(stream, (int)productType_);
			StreamUtil.WriteBoolean(stream, paymentComplete_);
			StreamUtil.WriteLong(stream, purchaseTime_);
		}

		public static Receipt Load(InputStream stream)
		{
			Receipt receipt = new Receipt();
			receipt.productID_ = StreamUtil.ReadString(stream);
			receipt.receipt_ = StreamUtil.ReadString(stream);
			receipt.state_ = StreamUtil.ReadInt(stream);
			receipt.productType_ = (ProductType)StreamUtil.ReadInt(stream);
			receipt.paymentComplete_ = StreamUtil.ReadBoolean(stream);
			receipt.purchaseTime_ = StreamUtil.ReadLong(stream);
			return receipt;
		}

		public static string GetItemType(Product p)
		{
			return null;
		}

		public static string GetItemType(ProductType type)
		{
			return (type != ProductType.Subscription) ? "inapp" : "subs";
		}

		public static string PasswordEncode(byte[] src)
		{
			bool pRINT = Config.PRINT;
			int num = src.Length;
			try
			{
				Config.PRINT = false;
				byte[] array = Compressor.Compress(src);
				src = new byte[array.Length + 8];
				JSystem.Arraycopy(EncodeUtil.LongToByteAry(CRC64.GetValue(array)), 0, src, 0, 8);
				JSystem.Arraycopy(array, 0, src, 8, array.Length);
				Encrypter.Encode(src, PASSKEY);
				return Base64.Encode(src);
			}
			finally
			{
				Config.PRINT = pRINT;
			}
		}

		public static string PasswordEncode(string src)
		{
			return PasswordEncode(src.GetBytes());
		}

		public static byte[] PasswordDecodeToBytes(string src)
		{
			bool pRINT = Config.PRINT;
			try
			{
				Config.PRINT = false;
				byte[] array = Base64.Decode(src);
				Encrypter.Decode(array, PASSKEY);
				long num = EncodeUtil.ByteAryToLong(array);
				byte[] array2 = new byte[array.Length - 8];
				JSystem.Arraycopy(array, 8, array2, 0, array2.Length);
				if (CRC64.GetValue(array2) != num)
				{
					return null;
				}
				return Compressor.Expand(array2);
			}
			finally
			{
				Config.PRINT = pRINT;
			}
		}

		public static string PasswordDecodeToString(string src)
		{
			return Encoding.UTF8.GetString(PasswordDecodeToBytes(src));
		}
	}
}
