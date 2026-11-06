using System;
using java.io;
using java.lang;
using kairo.unity.util;
using system.billing.util;

namespace system.billing.ui
{
	public class BillingItem
	{
		public const int TYPE_INAPP = 0;

		public const int TYPE_SUBSCRIPTION = 1;

		public const int STATE_SALE = 0;

		public const int STATE_PURCHASED = 1;

		public const int FLAG_CONSUME = 1;

		public const int FLAG_GAMEONLY = 2;

		public const int FLAG_NOTSALE = 4;

		public const int FLAG_NEW = 8;

		internal int id_;

		internal SecureString productId_;

		internal string name_;

		internal int type_;

		internal string price_;

		internal string explain_;

		internal SecureInt flag_ = new SecureInt();

		internal SecureInt state_ = new SecureInt();

		internal bool provision_;

		internal Purchase purchase_;

		internal bool purchaseNow_;

		internal SecureString provisionType_;

		internal SecureString provisionValueS_;

		internal SecureInt provisionValueI_ = new SecureInt();

		internal bool testProduct_;

		internal bool consuming_;

		internal bool recovery_;

		internal SecureString seriesName_;

		internal SecureInt seriesNumber_ = new SecureInt();

		public virtual void Init(int id, int flag, string productId, string name, string price, int type, string explain)
		{
			id_ = id;
			flag_ = new SecureInt(flag);
			productId_ = SecureString.ValueOf(productId);
			name_ = name;
			type_ = type;
			price_ = ((price == null) ? (string.Empty) : (price));
			explain_ = ((explain == null) ? (string.Empty) : (explain));
			state_ = new SecureInt(0);
			provision_ = false;
			consuming_ = false;
			seriesName_ = null;
			seriesNumber_ = new SecureInt(-1);
			productId_ = GetPlatformProductID(productId_);
			if (productId != null) {

			string[] array = StringUtil.Split(productId, "_");
			int num = array.Length;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].StartsWith("base") || array[i].StartsWith("case"))
				{
					testProduct_ = array[i].StartsWith("base");
					num = i;
					break;
				}
			}
			if (0 < array.Length)
			{
				provisionType_ = SecureString.ValueOf(array[0]);
			}
			if (num >= 2)
			{
				provisionValueS_ = SecureString.ValueOf(array[1]);
			}
			if (provisionValueS_ != null)
			{
				provisionValueI_ = new SecureInt(0);
				if (provisionValueS_.Equals("on"))
				{
					provisionValueI_ = new SecureInt(1);
				}
				else if (provisionValueS_.Equals("off"))
				{
					provisionValueI_ = new SecureInt(0);
				}
				else
				{
					try
					{
						provisionValueI_ = new SecureInt(JInteger.ParseInt(SecureString.ToString(provisionValueS_)));
					}
					catch (Exception)
					{
					}
				}
			}
			if (3 < array.Length)
			{
				seriesName_ = SecureString.ValueOf(array[0] + "_" + array[1] + "_" + array[2]);
				seriesNumber_ = new SecureInt(JInteger.ParseInt(array[3]));
			}
}
		}

		public int GetId()
		{
			return id_;
		}

		public string GetProductId()
		{
			return SecureString.ToString(productId_);
		}

		public string GetName()
		{
			return name_;
		}

		public int GetItemType()
		{
			return type_;
		}

		public string GetPrice()
		{
			return price_;
		}

		public string GetExplain()
		{
			return explain_;
		}

		public int GetState()
		{
			return state_.Get();
		}

		internal void ChangeState(int state)
		{
			state_ = new SecureInt(state);
		}

		public string GetProvisionType()
		{
			return SecureString.ToString(provisionType_);
		}

		public int GetProvisionValue()
		{
			return provisionValueI_.Get();
		}

		public string GetProvisionValueText()
		{
			return SecureString.ToString(provisionValueS_);
		}

		public string GetSeriesName()
		{
			return SecureString.ToString(seriesName_);
		}

		public int GetSeriesNumber()
		{
			return seriesNumber_.Get();
		}

		public bool CheckTestProduct()
		{
			return testProduct_;
		}

		public bool CheckProvision()
		{
			BillingManager instance = BillingManager.GetInstance();
			if (Check(2) && !instance.CheckPlayingGame())
			{
				return false;
			}
			return provision_;
		}

		public void SetProvision(bool provision)
		{
			provision_ = provision;
		}

		public Purchase GetPurchase()
		{
			return purchase_;
		}

		public void SetPurchase(Purchase purchase)
		{
			purchase_ = purchase;
		}

		public bool Check(int bit)
		{
			return (flag_.Get() & bit) != 0;
		}

		public void On(int bit)
		{
			flag_ = new SecureInt(flag_.Get() | bit);
		}

		public void Off(int bit)
		{
			flag_ = new SecureInt(flag_.Get() & ~bit);
		}

		public virtual void Serialize(OutputStream os)
		{
			StreamUtil.WriteByte(os, (byte)state_.Get());
			StreamUtil.WriteByte(os, (byte)flag_.Get());
			StreamUtil.WriteBoolean(os, provision_);
			StreamUtil.WriteBoolean(os, consuming_);
		}

		public virtual void Deserialize(InputStream stream)
		{
			state_ = new SecureInt((int)StreamUtil.ReadByte(stream));
			int num = StreamUtil.ReadByte(stream);
			provision_ = StreamUtil.ReadBoolean(stream);
			consuming_ = StreamUtil.ReadBoolean(stream);
			flag_ = new SecureInt(BitUtil.Off(flag_.Get(), 8) | (num & 8));
		}

		public static string GetPlatformProductID(string productID)
		{
			return productID;
		}
	}
}
