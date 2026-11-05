using kairo.unity.ui;
using UnityEngine;

namespace native
{
	public static class AppPlugin
	{
		private static AndroidJavaClass Config;

		private static AndroidJavaClass Utility;

		static AppPlugin()
		{
			Config = new AndroidJavaClass("plugin.Config");
			Utility = new AndroidJavaClass("plugin.Utility");
		}

		public static void Init(int language, bool log, string licenseKey)
		{
			Config.CallStatic("init", language, log, licenseKey);
		}

		public static void Finish()
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				Config.CallStatic("finish");
			}, true);
		}

		public static int ShowPurchaseLog(string title, int num, string text, bool log)
		{
			return (int)IApplication.GetCurrentApp().RunOnUiThread(() => Utility.CallStatic<int>("showPurchaseLog", new object[4] { title, num, text, log }), true);
		}

		public static void LaunchMailer(string subject, string text)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				Utility.CallStatic("launchMailer", subject, text);
			}, true);
		}
	}
}
