using System;
using java.io;
using java.lang;
using kairo.unity.util;

namespace system.billing.ui
{
	public class ApplyTask
	{
		public const int TYPE_PROVISION = 0;

		public const int TYPE_CANCEL = 1;

		public BillingItem item_;

		public SecureString gift_;

		public bool notice_;

		public bool lock_;

		public int type_;

		public SecureString name_;

		public SecureString productId_;

		public SecureString provisionType_;

		public SecureString provisionValueS_;

		public SecureInt provisionValueI_ = new SecureInt();

		public int talkCharaId_;

		public SecureString talkText_;

		public ApplyTask()
		{
		}

		public ApplyTask(BillingItem item, int type, bool notice)
		{
			Init(item, string.Empty, type, notice);
		}

		public ApplyTask(string gift)
		{
			Init(null, gift, 0, true);
		}

		private void Init(BillingItem item, string gift, int type, bool notice)
		{
			item_ = item;
			gift_ = new SecureString((gift == null) ? string.Empty : gift);
			type_ = type;
			notice_ = notice;
			lock_ = false;
			name_ = new SecureString(string.Empty);
			productId_ = new SecureString(string.Empty);
			provisionType_ = new SecureString(string.Empty);
			provisionValueS_ = new SecureString(string.Empty);
			provisionValueI_ = new SecureInt(0);
			if (item_ != null)
			{
				name_ = new SecureString(item_.GetName());
				productId_ = new SecureString(item_.GetProductId());
				provisionType_ = new SecureString(item_.GetProvisionType());
				provisionValueS_ = new SecureString(item_.GetProvisionValueText());
				provisionValueI_ = new SecureInt(item_.GetProvisionValue());
			}
			else
			{
				if (gift_.Length() <= 0)
				{
					return;
				}
				if (gift_.IndexOf(9) != -1)
				{
					string[] array = StringUtil.Split(gift_.ToString(), "\t");
					talkCharaId_ = JInteger.ParseInt(array[0]);
					talkText_ = new SecureString(array[1]);
				}
				else
				{
					if (gift_.StartsWith("@"))
					{
						return;
					}
					string[] array2 = StringUtil.Split(gift_.ToString(), ",");
					productId_ = new SecureString(array2[0]);
					if (1 < array2.Length)
					{
						name_ = new SecureString(array2[1]);
					}
					array2 = StringUtil.Split(productId_.Get(), "_");
					if (0 < array2.Length)
					{
						provisionType_ = new SecureString(array2[0]);
					}
					if (1 < array2.Length)
					{
						provisionValueS_ = new SecureString(array2[1]);
					}
					try
					{
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
							provisionValueI_ = new SecureInt(JInteger.ParseInt(provisionValueS_.Get()));
						}
					}
					catch (Exception)
					{
					}
					if (name_.Length() != 0)
					{
						return;
					}
					BillingManager instance = BillingManager.GetInstance();
					BillingItem[] items = instance.GetItems();
					BillingItem billingItem = null;
					BillingItem billingItem2 = null;
					BillingItem billingItem3 = null;
					for (int i = 0; i < items.Length; i++)
					{
						if (items[i] != null && items[i].GetProvisionType().Equals(SecureString.ToString(provisionType_)))
						{
							if (items[i].GetProvisionValueText().Equals(SecureString.ToString(provisionValueS_)))
							{
								billingItem = items[i];
							}
							else if (items[i].GetProvisionValue() > 1 && provisionValueI_.Get() > 1)
							{
								billingItem2 = items[i];
							}
							else
							{
								billingItem3 = items[i];
							}
						}
					}
					if (billingItem != null)
					{
						name_ = new SecureString(billingItem.GetName());
					}
					else if (billingItem2 != null)
					{
						name_ = new SecureString(billingItem2.GetName());
					}
					else if (billingItem3 != null)
					{
						name_ = new SecureString(billingItem3.GetName());
					}
				}
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteInt(os, (item_ == null) ? (-1) : item_.id_);
			StreamUtil.WriteString(os, (gift_ == null) ? string.Empty : gift_.Get());
			StreamUtil.WriteByte(os, (byte)type_);
			StreamUtil.WriteBoolean(os, notice_);
		}

		public void Deserialize(InputStream stream)
		{
			BillingManager instance = BillingManager.GetInstance();
			BillingItem[] items = instance.GetItems();
			item_ = null;
			int num = StreamUtil.ReadInt(stream);
			if (0 <= num && num < items.Length)
			{
				item_ = items[num];
			}
			gift_ = new SecureString(StreamUtil.ReadString(stream));
			type_ = StreamUtil.ReadByte(stream);
			notice_ = StreamUtil.ReadBoolean(stream);
			Init(item_, SecureString.ToString(gift_), type_, notice_);
		}

		public bool Enable()
		{
			return IsItem() || IsGift();
		}

		public bool IsItem()
		{
			return item_ != null;
		}

		public bool IsGift()
		{
			return gift_ != null && gift_.Length() > 0;
		}

		public bool IsGiftScript()
		{
			return gift_ != null && gift_.Length() > 0 && gift_.StartsWith("@");
		}

		public bool IsGameApply()
		{
			return IsGift() || (IsItem() && item_.Check(2));
		}

		public bool Equals(ApplyTask obj)
		{
			if (IsItem() && obj.IsItem())
			{
				return item_ == obj.item_ && type_ == obj.type_;
			}
			if (IsGift() && obj.IsGift())
			{
				return gift_.Equals(obj.gift_);
			}
			if (!IsGift() && !IsItem() && !obj.IsGift() && !obj.IsItem())
			{
				return true;
			}
			return false;
		}
	}
}
