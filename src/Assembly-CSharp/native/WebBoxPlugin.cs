using kairo.unity.ui;
using UnityEngine;

namespace native
{
	public static class WebBoxPlugin
	{
		private static AndroidJavaClass WebBox;

		static WebBoxPlugin()
		{
			WebBox = new AndroidJavaClass("plugin.WebBox");
		}

		public static bool Create()
		{
			return (bool)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				WebBox.CallStatic("create");
				return true;
			}, true);
		}

		public static void SetViewRect(int x, int y, int width, int height)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				WebBox.CallStatic("setViewRect", x, y, width, height);
			}, true);
		}

		public static void LoadUrl(string url)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				WebBox.CallStatic("loadUrl", url);
			}, true);
		}

		public static void Dispose()
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				WebBox.CallStatic("dispose");
			}, true);
		}

		public static bool Visible()
		{
			return (bool)IApplication.GetCurrentApp().RunOnUiThread(() => WebBox.CallStatic<bool>("visible", new object[0]), true);
		}

		public static void SetVisible(bool visible)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				WebBox.CallStatic("setVisible", visible);
			}, true);
		}
	}
}
