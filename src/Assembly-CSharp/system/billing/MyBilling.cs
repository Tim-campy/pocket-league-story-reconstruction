using System;
using form;
using kairo.unity.form;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using system.billing.ui;
using system.form;

namespace system.billing
{
	public class MyBilling : BillingManager
	{
		public const int ITEM_AD_OFF = 0;

		public const int TICKET_MAX = 999;

		public const string TICKET_PRODUCT = "ticket";

		public const int SHOP_OWNER = 4;

		internal static IApplication app_;

		internal static FormManager fm_;

		internal static AppData apdat_;

		private MyBilling()
		{
			app_ = IApplication.GetCurrentApp();
			fm_ = (FormManager)app_.GetFormManager();
			apdat_ = AppData.GetInstance();
			BillingManager.BASE64_PUBLIC_KEY = new SecureString("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmp4nkTi9cXIfx3bxAtIiFKYtNPnXzkx01PkfcYaRHKsZYxsOfGwxoJtlZfgovAgn5m+OsX7d0xrGON+4/fYDgqw4Pfz8Gg//g0DV3JJ202wh07OykycemKTR7DXipiXnehEIajW8hX5gIFqFX730RBcDlF0aQAEMM7mYQEw/ZMxXh0dAwC3j1AWfgAzbx1gym86Mj1tTc2p0L/GO6WFKV5NY4cIQ45AX5Z85GELL52vlpFZ4nTy90Hsy3yguAVQ9SoXbE/cEu8139Cn2SjNoykiL3EKqUVx3ux///s0e1kqJcpWkm5mV/8VlsEdFJWOv8QY3lWU3bKLp7KdeYkbDQQIDAQAB");
		}

		private string[][] GetBillingTable()
		{
			bool flag = Language.Japanese();
			return new string[4][]
			{
				new string[7]
				{
					"00",
					"0000",
					"ad_off_case0",
					(flag) ? ("広告OFF+横回転") : ("Remove ads"),
					string.Empty,
					"0",
					string.Empty
				},
				new string[7]
				{
					"10",
					"1100",
					"ticket_1_case0",
					(flag) ? ("ﾁｹｯﾄ|枚|枚") : ("Ticket||Ticket"),
					string.Empty,
					"0",
					string.Empty
				},
				new string[7]
				{
					"20",
					"0100",
					"ticket_10_case0_00",
					(flag) ? ("ﾁｹｯﾄ|枚|枚ｾｯﾄ") : ("Tickets||Tickets"),
					string.Empty,
					"0",
					string.Empty
				},
				new string[7]
				{
					"21",
					"0100",
					"ticket_10_case0_01",
					(flag) ? ("ﾁｹｯﾄ|枚|枚ｾｯﾄ") : ("Tickets||Tickets"),
					string.Empty,
					"0",
					string.Empty
				}
			};
		}

		public new static MyBilling GetInstance()
		{
			if (BillingManager.instance_ == null)
			{
				BillingManager.instance_ = new MyBilling();
			}
			return (MyBilling)BillingManager.instance_;
		}

		public void Init()
		{
			Init(GetBillingTable());
		}

		public override bool ExecuteApplyTask(ApplyTask task)
		{
			if (task.IsGift() && task.talkText_ != null)
			{
				fm_.Push(SubForm.MakeTalkForm(task.talkCharaId_, SecureString.ToString(task.talkText_), string.Empty));
				Provisioning(task);
				return true;
			}
			if (task.type_ == 0)
			{
				if (!task.IsItem() || task.provisionType_.Equals("ticket"))
				{
				}
				if (task.notice_)
				{
					fm_.Push(new BillingForm(4, task));
				}
				else
				{
					Provisioning(task);
				}
			}
			else if (task.type_ == 1)
			{
				Cancel(task);
			}
			return true;
		}

		public override void Provisioning(ApplyTask task)
		{
			string text = SecureString.ToString(task.provisionType_);
			int num = task.provisionValueI_.Get();
			if (text.Equals("ad"))
			{
				FormBase topForm = fm_.GetTopForm();
				topForm.SetTouchValue(720896, -1);
				try
				{
					apdat_.SaveSystem();
				}
				catch (Exception)
				{
				}
			}
			else if (!text.Equals("ticket"))
			{
			}
			base.Provisioning(task);
		}

		public override void Cancel(ApplyTask task)
		{
			string text = SecureString.ToString(task.provisionType_);
			int num = task.provisionValueI_.Get();
			if (text.Equals("ad"))
			{
				int num2 = app_.GetOrientation();
				if (app_.GetWidth() > app_.GetHeight())
				{
					num2 = (num2 + 1) % 4;
				}
				FormBase topForm = fm_.GetTopForm();
				topForm.SetTouchValue(720896, num2);
				try
				{
					apdat_.SaveSystem();
				}
				catch (Exception)
				{
				}
			}
			else if (!text.Equals("ticket"))
			{
			}
			base.Cancel(task);
		}

		public override bool CheckPlayingGame()
		{
			if (base.CheckPlayingGame())
			{
				return true;
			}
			FormBase form = fm_.GetForm(0);
			if (form != null && form.IsAlive())
			{
				int id = form.GetId();
				if (id != 0 && id != 1)
				{
					return true;
				}
			}
			return false;
		}
	}
}
