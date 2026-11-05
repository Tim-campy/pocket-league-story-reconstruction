using System;
using kairo.unity.system;
using kairo.unity.ui;
using kairo.unity.util;
using native;

namespace system.billing.ui
{
	public class PurchaseLogView
	{
		private string title_;

		private int num_;

		private string text_;

		private bool log_;

		public PurchaseLogView(string title)
		{
			title_ = title;
		}

		public void SetLog(int num, string text, bool log)
		{
			num_ = num;
			text_ = StringUtil.Replace(text, "<br>", "\n");
			log_ = log;
		}

		public void Show()
		{
			int num = AppPlugin.ShowPurchaseLog(title_, num_, text_, log_);
			if (num == 1)
			{
				IApplication currentApp = IApplication.GetCurrentApp();
				BillingManager instance = BillingManager.GetInstance();
				instance.ClearPurchaseLog();
				KairoBase instance2 = KairoBase.GetInstance();
				try
				{
					instance2.Save();
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
