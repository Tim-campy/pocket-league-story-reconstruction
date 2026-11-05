using kairo.unity.ui;
using UnityEngine;

namespace java.native
{
	public static class PluginBase
	{
		public static void CallStatic(AndroidJavaClass javaClass, string methodName, params object[] args)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				javaClass.CallStatic(methodName, args);
			}, true);
		}

		public static void Call(AndroidJavaObject javaObject, string methodName, params object[] args)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				javaObject.Call(methodName, args);
			}, true);
		}

		public static T CallStatic<T>(AndroidJavaClass javaClass, string methodName, params object[] args)
		{
			return (T)IApplication.GetCurrentApp().RunOnUiThread(() => javaClass.CallStatic<T>(methodName, args), true);
		}

		public static T Call<T>(AndroidJavaObject javaObject, string methodName, params object[] args)
		{
			return (T)IApplication.GetCurrentApp().RunOnUiThread(() => javaObject.Call<T>(methodName, args), true);
		}

		public static T GetStatic<T>(AndroidJavaClass javaClass, string fieldName)
		{
			return (T)IApplication.GetCurrentApp().RunOnUiThread(() => javaClass.GetStatic<T>(fieldName), true);
		}

		public static T Get<T>(AndroidJavaObject javaObject, string fieldName)
		{
			return (T)IApplication.GetCurrentApp().RunOnUiThread(() => javaObject.Get<T>(fieldName), true);
		}
	}
}
