using kairo.common.cfg;
using kairo.unity.ui;
using UnityEngine;

namespace kairo.unity.native.util
{
	public static class AsyncThread
	{
		public static string CallStatic(AndroidJavaClass javaClass, string methodName, params object[] args)
		{
			AndroidJavaObject asyncThread = (AndroidJavaObject)IApplication.GetCurrentApp().RunOnUiThread(() => ((Config.APPLI_TYPE != 2) || (Config.UNITY_EDITOR)) ? (null) : (javaClass.CallStatic<AndroidJavaObject>(methodName, args)), true);
			if (asyncThread == null)
			{
				return null;
			}
			string result = null;
			while ((bool)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				bool flag = false;
				if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
				{
					flag = asyncThread.Call<bool>("isRunning", new object[0]);
					if (!flag)
					{
						result = asyncThread.Call<string>("getResult", new object[0]);
					}
				}
				return flag;
			}, true))
			{
			}
			return result;
		}
	}
}
