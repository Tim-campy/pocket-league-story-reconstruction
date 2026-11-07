using System;
using form;
using java.io;
using java.lang;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.system;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using system.billing;
using system.billing.ui;
using system.form;
using system.friend;
using system.lineup;

namespace system
{
	public class KairoService : KairoBase
	{
		public const int FLAG_BILLING_LIST_ERR = 65536;

		public const int FLAG_BILLING_IN = 131072;

		public const int FLAG_BILLING_OUT = 262144;

		public const int FLAG_BILLING_NOTES = 524288;

		public const int FLAG_BILLING_CONFIRM = 1048576;

		private static AppData apdat_;

		private static MyBilling billing_;

		private static MyFriend friend_;

		private static MyLineup lineup_;

		private JThread provideThread_;

		private bool autoProvision_ = true;

		private int lastFormId_ = -1;

		private KairoService()
		{
			apdat_ = AppData.GetInstance();
			billing_ = MyBilling.GetInstance();
			friend_ = MyFriend.GetInstance();
			lineup_ = MyLineup.GetInstance();
		}

		public new static KairoService GetInstance()
		{
			if (KairoBase.instance_ == null)
			{
				KairoBase.instance_ = new KairoService();
			}
			return (KairoService)KairoBase.instance_;
		}

		protected override void OnInit()
		{
			billing_.Init();
			friend_.Init();
			lineup_.Init();
		}

		public override void OnStart()
		{
			FormManager instance = FormManager.GetInstance();
			if (appliVersion_ < GetLatestVersion())
			{
				SubForm subForm = SubForm.MakeDialog(KairoText.LT("ｱﾌﾟﾘのﾊﾞｰｼﾞｮﾝｱｯﾌﾟを行います"));
				instance.Push(subForm);
				while (subForm.CheckUsable())
				{
					try
					{
						JThread.Sleep(10L);
					}
					catch (Exception)
					{
					}
				}
				string packageName = IApplication.GetCurrentApp().GetPackageName();
				Canvas.OpenBrowser("market://details?id=" + packageName + "&referrer=utm_source%3Dappli%26utm_medium%3Dhouse_ad%26utm_content%3D" + packageName + "_lite%26utm_campaign%3Dbanner");
				instance.RemoveAllForms();
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				try
				{
					billing_.UpdateBillingItems(false);
					billing_.UpdateApplyTask(false);
break;				}
				catch (Exception)
				{
					JThread.Sleep(1000L);
				}
			}
			if (CheckUserId() && !friend_.CheckRegistFriend())
			{
				FriendForm friendForm = new FriendForm(0);
				instance.Push(friendForm);
				while (friendForm.CheckUsable())
				{
					try
					{
						JThread.Sleep(10L);
					}
					catch (Exception)
					{
					}
				}
				friend_.UpdateRandomFriend();
			}
			if (friend_.CheckRegistFriend())
			{
				try
				{
					friend_.CgiRegistFriend(SecureString.ToString(friend_.friSave_.strs[0]), friend_.friSave_.ints[1].Get());
					friend_.RefreshFriendList();
				}
				catch (Exception t)
				{
					if (Config.PRINT)
					{
						Log.Info(t);
					}
				}
				if (friend_.friendList_.Size() < friend_.FRIEND_MAX)
				{
					friend_.UpdateRandomFriend();
				}
			}
			BillingItem item = billing_.GetItem(0);
			if (item.GetState() == 0 && startCount_ > 1)
			{
				lineup_.Start();
			}
			MyLineup instance2 = MyLineup.GetInstance();
			LineupForm lineupForm;
if (instance2.CheckShowableLineup()) {
			lineupForm = new LineupForm(0);
			instance.Push(lineupForm);
			while (lineupForm.CheckUsable())
			{
				try
				{
					JThread.Sleep(10L);
				}
				catch (Exception)
				{
				}
			}
			try
			{
				JThread.Sleep(500L);
			}
			catch (Exception)
			{
			}

}
}

		protected override void Connected(string cgi, byte[] send, bool split)
		{
			base.Connected(cgi, send, split);
			if (login_)
			{
				try
				{
					BillingManager instance = BillingManager.GetInstance();
					instance.ConnectRegistMarketPurchase();
				}
				catch (Exception)
				{
				}
			}
		}

		public override void Update()
		{
			base.Update();
			if (!start_)
			{
				return;
			}
			FormManager instance = FormManager.GetInstance();
			FormBase form = instance.GetForm(0);
			if (autoProvision_ && provideThread_ == null && form.GetId() != lastFormId_ && form.GetId() != 0)
			{
				if (billing_.CheckNextConsume())
				{
					provideThread_ = new JThread(delegate
					{
						try
						{
							BillingManager instance2 = BillingManager.GetInstance();
							instance2.RegistApplyTask(true);
							instance2.UpdateApplyTask(true);
							if (instance2.CheckNextConsume())
							{
								autoProvision_ = false;
							}
						}
						catch (Exception t)
						{
							if (Config.PRINT)
							{
								Log.Info(t);
							}
							autoProvision_ = false;
						}
						finally
						{
							ConnectForm.close_ = true;
							provideThread_ = null;
						}
					});
					provideThread_.Start();
					try
					{
						JThread.Sleep(20L);
					}
					catch (Exception)
					{
					}
					if (provideThread_ != null)
					{
						instance.Push(new ConnectForm());
					}
				}
				else
				{
					billing_.UpdateApplyTask(true);
				}
			}
			lastFormId_ = form.GetId();
		}

		public override void Finish()
		{
			if (billing_ != null)
			{
				billing_.Finish();
			}
			base.Finish();
		}

		protected override void OnLoad(InputStream stream)
		{
			billing_.Load(StreamUtil.Read(stream, true));
			friend_.Load(StreamUtil.Read(stream, true));
			lineup_.Load(StreamUtil.Read(stream, true));
		}

		protected override void OnWrite(OutputStream os)
		{
			StreamUtil.Write(os, billing_.Write(), true);
			StreamUtil.Write(os, friend_.Write(), true);
			StreamUtil.Write(os, lineup_.Write(), true);
		}

		protected override void OnNewGame(bool take)
		{
			billing_.NewGame(take);
			friend_.NewGame(take);
		}

		protected override void OnLoadGame(InputStream stream)
		{
			billing_.LoadGame(StreamUtil.Read(stream, true));
			friend_.LoadGame(StreamUtil.Read(stream, true));
		}

		protected override void OnWriteGame(OutputStream os)
		{
			StreamUtil.Write(os, billing_.WriteGame(), true);
			StreamUtil.Write(os, friend_.WriteGame(), true);
		}

		public override void Save()
		{
			apdat_.SaveSystem();
		}

		public override void SaveAll()
		{
			if (apdat_.save_ != null)
			{
				apdat_.SaveGame(true);
				apdat_.SaveGame(false);
			}
			apdat_.SaveSystem();
		}
	}
}
