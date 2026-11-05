using System;
using kairo.common.cfg;
using kairo.unity.ui;
using UnityEngine;

namespace kairo.unity.native
{
	public class AdMobPlugin
	{
		private static IApplication app_ = null;

		private static AndroidJavaClass AdMobView;

		private static AndroidJavaClass Interstitial;

		internal static int AdSize;

		internal static bool weakOmit_;

		public static void SetWeakOmit(bool flag)
		{
			weakOmit_ = flag;
		}

		public static void Init()
		{
			app_ = IApplication.GetCurrentApp();
			if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
			{
				AdMobView = new AndroidJavaClass("kairo.android.plugin.ad.AdMobView");
				Interstitial = new AndroidJavaClass("kairo.android.plugin.ad.Interstitial");
			}
		}

		public static AndroidJavaObject CreateAdMobView(int adSize, string adUnitId)
		{
			return (AndroidJavaObject)app_.RunOnUiThread(delegate
			{
				AdSize = adSize;
				return ((Config.APPLI_TYPE != 2) || (Config.UNITY_EDITOR)) ? (null) : (AdMobView.CallStatic<AndroidJavaObject>("createAdMobView", new object[2] { adSize, adUnitId }));
			}, true);
		}

		public static void RemoveAdMobView(AndroidJavaObject obj)
		{
			app_.RunOnUiThread(delegate
			{
				if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
				{
					AdMobView.CallStatic("removeAdMobView");
					if (obj != null)
					{
						obj.Dispose();
					}
					obj = null;
				}
			}, true);
		}

		public static void Start(AndroidJavaObject obj)
		{
			app_.RunOnUiThread(delegate
			{
				if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
				{
					obj.Call("start");
				}
			}, true);
		}

		public static bool IsStart(AndroidJavaObject obj)
		{
			if (app_.CheckUiThread())
			{
				return _isStart(obj);
			}
			return (bool)app_.RunOnUiThread(() => _isStart(obj), true);
		}

		private static bool _isStart(AndroidJavaObject obj)
		{
			if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
			{
				return obj.Call<bool>("isStart", new object[0]);
			}
			return false;
		}

		public static bool IsLoad(AndroidJavaObject obj)
		{
			if (app_.CheckUiThread())
			{
				return _isLoad(obj);
			}
			return (bool)app_.RunOnUiThread(() => _isLoad(obj), true);
		}

		private static bool _isLoad(AndroidJavaObject obj)
		{
			if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
			{
				return obj.Call<bool>("isLoad", new object[0]);
			}
			return false;
		}

		internal static AndroidJavaObject LoadInterstitialAd(string adUnitId)
		{
			if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
			{
				return (AndroidJavaObject)app_.RunOnUiThread(delegate
				{
					try
					{
						return Interstitial.CallStatic<AndroidJavaObject>("startInterstitialAd", new object[1] { adUnitId });
					}
					catch (Exception)
					{
						return (object)null;
					}
				}, true);
			}
			return null;
		}

		internal static int GetInterstitialAdState(AndroidJavaObject obj)
		{
			if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
			{
				return (int)app_.RunOnUiThread(() => obj.Call<int>("getState", new object[0]), true);
			}
			return -1;
		}

		internal static void ShowInterstitialAd(AndroidJavaObject obj)
		{
			if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
			{
				app_.RunOnUiThread(delegate
				{
					obj.Call("show");
				}, true);
			}
		}
	}
}
