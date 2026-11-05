using java.io;
using java.util;
using kairo.unity.util;

namespace kairo.unity.system.payment
{
	public class PaymentDataRecord : ISerializable
	{
		internal string productId_;

		internal string orderId_;

		internal string purchaseToken_;

		internal long timestamp_;

		internal bool send_;

		internal int amount_;

		internal int actCode_;

		public const int ACT_NONE = 0;

		public const int ACT_NORMAL_BUY = 1;

		public const int ACT_NORMAL_USE = 2;

		public PaymentDataRecord()
			: this("", "", 0, 0)
		{
		}

		public PaymentDataRecord(string productId, string orderId, int amount, int actCode)
		{
			productId_ = productId;
			orderId_ = orderId;
			amount_ = amount;
			actCode_ = actCode;
			send_ = false;
			purchaseToken_ = " ";
			timestamp_ = -1L;
		}

		public override string ToString()
		{
			return GetProductId() + "[" + timestamp_ + "]\n" + GetOrderId();
		}

		public string GetProductId()
		{
			return productId_;
		}

		public string GetOrderId()
		{
			return orderId_;
		}

		public int GetAmount()
		{
			return amount_;
		}

		public int GetActionCode()
		{
			return actCode_;
		}

		public void SetTimeStamp(long time)
		{
			timestamp_ = time;
		}

		public void SetSendFlags(bool send)
		{
			send_ = send;
		}

		public void SetPurchaseToken(string token)
		{
			purchaseToken_ = token;
		}

		public string GetTimeStamp(bool dayling = false)
		{
			if (timestamp_ <= 0)
			{
				return "";
			}
			JCalendar instance = JCalendar.GetInstance();
			instance.SetTimeInMillis(timestamp_);
			string text = string.Concat(instance.Get(1));
			string text2 = string.Concat(instance.Get(2) + 1);
			string text3 = string.Concat(instance.Get(5));
			string text4 = string.Concat(instance.Get(11));
			string text5 = string.Concat(instance.Get(12));
			string text6 = string.Concat(instance.Get(13));
			if (text2.Length == 1)
			{
				text2 = "0" + text2;
			}
			if (text3.Length == 1)
			{
				text3 = "0" + text3;
			}
			if (text4.Length == 1)
			{
				text4 = "0" + text4;
			}
			if (text5.Length == 1)
			{
				text5 = "0" + text5;
			}
			if (text6.Length == 1)
			{
				text6 = "0" + text6;
			}
			if (dayling)
			{
				return text + "-" + text2 + "-" + text3;
			}
			return text + "-" + text2 + "-" + text3 + " " + text4 + ":" + text5 + ":" + text6;
		}

		public void Serialize(OutputStream stream)
		{
			StreamUtil.WriteString(stream, productId_);
			StreamUtil.WriteString(stream, orderId_);
			StreamUtil.WriteString(stream, purchaseToken_);
			StreamUtil.WriteLong(stream, timestamp_);
			StreamUtil.WriteBoolean(stream, send_);
			StreamUtil.WriteInt(stream, amount_);
			StreamUtil.WriteInt(stream, actCode_);
		}

		public void Deserialize(InputStream stream)
		{
			productId_ = StreamUtil.ReadString(stream);
			orderId_ = StreamUtil.ReadString(stream);
			purchaseToken_ = StreamUtil.ReadString(stream);
			timestamp_ = StreamUtil.ReadLong(stream);
			send_ = StreamUtil.ReadBoolean(stream);
			amount_ = StreamUtil.ReadInt(stream);
			actCode_ = StreamUtil.ReadInt(stream);
		}
	}
}
