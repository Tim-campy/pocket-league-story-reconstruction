using java.io;
using java.lang;
using java.util;
using kairo.unity.util;
using system.billing.util;

namespace system.billing.ui
{
	public class PurchaseRecord
	{
		public const int NONE = -1;

		public const int PURCHASE = 0;

		public const int CONSUME = 1;

		public const int PROVISION = 2;

		internal string productId_;

		internal string orderId_;

		internal int[] timestamp_;

		internal int state_;

		internal bool send_;

		public PurchaseRecord()
			: this(string.Empty, string.Empty)
		{
		}

		public PurchaseRecord(string productId, string orderId)
		{
			productId_ = productId;
			orderId_ = orderId;
			timestamp_ = new int[3];
			for (int i = 0; i < timestamp_.Length; i++)
			{
				timestamp_[i] = -1;
			}
			state_ = -1;
			send_ = false;
		}

		public override string ToString()
		{
			return GetProductId() + " (" + GetState() + ") [" + GetTimeStamp(0) + "][" + GetTimeStamp(1) + "][" + GetTimeStamp(2) + "]\n" + GetOrderId();
		}

		public string GetProductId()
		{
			return productId_;
		}

		public string GetOrderId()
		{
			return orderId_;
		}

		public string GetTimeStamp(int state)
		{
			if (timestamp_[state] <= 0)
			{
				return string.Empty;
			}
			JCalendar instance = JCalendar.GetInstance();
			instance.SetTimeInMillis((long)timestamp_[state] * 1000L);
			string text = JString.ValueOf(instance.Get(1));
			string text2 = JString.ValueOf(instance.Get(2) + 1);
			string text3 = JString.ValueOf(instance.Get(5));
			string text4 = JString.ValueOf(instance.Get(11));
			string text5 = JString.ValueOf(instance.Get(12));
			string text6 = JString.ValueOf(instance.Get(13));
			if (text2.Length() == 1)
			{
				text2 = "0" + text2;
			}
			if (text3.Length() == 1)
			{
				text3 = "0" + text3;
			}
			if (text4.Length() == 1)
			{
				text4 = "0" + text4;
			}
			if (text5.Length() == 1)
			{
				text5 = "0" + text5;
			}
			if (text6.Length() == 1)
			{
				text6 = "0" + text6;
			}
			return text + "-" + text2 + "-" + text3 + " " + text4 + ":" + text5 + ":" + text6;
		}

		public string GetTimeStampNengappi(int state)
		{
			if (timestamp_[state] <= 0)
			{
				return string.Empty;
			}
			JCalendar instance = JCalendar.GetInstance();
			instance.SetTimeInMillis((long)timestamp_[state] * 1000L);
			string text = JString.ValueOf(instance.Get(1));
			string text2 = JString.ValueOf(instance.Get(2) + 1);
			string text3 = JString.ValueOf(instance.Get(5));
			string text4 = JString.ValueOf(instance.Get(11));
			string text5 = JString.ValueOf(instance.Get(12));
			string text6 = JString.ValueOf(instance.Get(13));
			if (text2.Length() == 1)
			{
				text2 = "0" + text2;
			}
			if (text3.Length() == 1)
			{
				text3 = "0" + text3;
			}
			if (text4.Length() == 1)
			{
				text4 = "0" + text4;
			}
			if (text5.Length() == 1)
			{
				text5 = "0" + text5;
			}
			if (text6.Length() == 1)
			{
				text6 = "0" + text6;
			}
			return text + "-" + text2 + "-" + text3;
		}

		public int GetState()
		{
			return state_;
		}

		public void ChangeState(int state, Purchase purchase)
		{
			if (state_ != state)
			{
				long num = ClockTime.GetTime();
				if (state == 0 && purchase != null)
				{
					num = purchase.GetPurchaseTime();
				}
				int num2 = (int)(num / 1000);
				state_ = state;
				if (num2 != timestamp_[state])
				{
					send_ = true;
					timestamp_[state] = num2;
				}
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteString(os, productId_);
			StreamUtil.WriteString(os, orderId_);
			for (int i = 0; i < timestamp_.Length; i++)
			{
				StreamUtil.WriteInt(os, timestamp_[i]);
			}
			StreamUtil.WriteByte(os, (byte)state_);
			StreamUtil.WriteBoolean(os, send_);
		}

		public void Deserialize(InputStream stream)
		{
			productId_ = StreamUtil.ReadString(stream);
			orderId_ = StreamUtil.ReadString(stream);
			for (int i = 0; i < timestamp_.Length; i++)
			{
				timestamp_[i] = StreamUtil.ReadInt(stream);
			}
			state_ = StreamUtil.ReadByte(stream);
			send_ = StreamUtil.ReadBoolean(stream);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
