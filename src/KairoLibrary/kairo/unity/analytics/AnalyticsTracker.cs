using System.Runtime.CompilerServices;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using java.lang;
using kairo.unity.io;
using kairo.unity.native;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.analytics
{
	public class AnalyticsTracker
	{
		private enum HitType
		{
			PageView,
			ScreenView,
			Event,
			Transaction,
			Item,
			Social,
			Exception,
			Timing
		}

		private class AnalyticsData
		{
			public string Url;

			public int Retry;

			public long Date;

			public AnalyticsData()
			{
			}

			public AnalyticsData(string url)
			{
				Url = url;
				Retry = 0;
				Date = ClockTime.GetTime();
			}
		}

		private static readonly string[] HIT_TYPES = new string[8] { "pageview", "screenview", "event", "transaction", "item", "social", "exception", "timing" };

		private const string ANALYTICS_KEY = "_AnalyticsTracker";

		private static List<AnalyticsData> analyticsBuffer_ = new List<AnalyticsData>();

		private static object lock_ = new object();

		private static JThread thread_;

		private static bool isSetup_;

		public static void TrackPageView(bool packageName, string id, params string[] p)
		{
			if (!isSetup_)
			{
				return;
			}
			string text = "";
			for (int i = 0; i < p.Length; i++)
			{
				if (i != 0)
				{
					text += "/";
				}
				text += p[i];
			}
			if (!text.StartsWith("/"))
			{
				text = "/" + text;
			}
			if (packageName)
			{
				text = "/" + IApplication.GetCurrentApp().GetPackageName() + text;
			}
			string text2 = "https://ssl.google-analytics.com/collect?&v=" + 1 + "&tid=" + id + "&cid=" + MakeClientId() + "&t=" + HIT_TYPES[0] + "&dl=" + Encode("http://kairopark.jp") + "&an=" + GetApplicationName() + "&dp=" + Encode(text);
			text2 = text2 + "&z=" + new System.Random((int)JSystem.CurrentTimeMillis()).Next();
			lock (lock_)
			{
				analyticsBuffer_.Add(new AnalyticsData(text2));
			}
			Save();
		}

		public static void DispatchPageView()
		{
			if (!AnalyticsTracker.isSetup_)
			{
				return;
			}
			if (AnalyticsTracker.analyticsBuffer_.Count <= 0 || AnalyticsTracker.thread_ != null)
			{
				return;
			}
			AnalyticsTracker.thread_ = new JThread(new ThreadStart(AnalyticsTracker.Run));
			AnalyticsTracker.thread_.Start();
		}

		protected static void Run()
		{
			try
			{
				while (analyticsBuffer_.Count > 0)
				{
					AnalyticsData analyticsData = null;
					HttpObject httpObject;
					lock (lock_)
					{
						if (analyticsBuffer_.Count > 0)
						{
							analyticsData = analyticsBuffer_[0];
						}
					}
					if (analyticsData != null)
					{
						httpObject = new HttpObject();
						try
						{
							httpObject.Connect(analyticsData.Url, null, false);
						}
						catch
						{
							analyticsData.Retry++;
							Save();
							throw;
						}
						analyticsBuffer_.Remove(analyticsData);
						Save();
						continue;
					}
					break;
				}
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
			finally
			{
				thread_ = null;
			}
		}

		private static string Encode(string url)
		{
			return Uri.EscapeDataString(url);
		}

		private static string MakeClientId()
		{
			return Guid.NewGuid().ToString();
		}

		private static string GetApplicationName()
		{
			string packageName = IApplication.GetCurrentApp().GetPackageName();
			return packageName.Substring(packageName.LastIndexOf(".") + 1);
		}

		internal static void Save()
		{
			if (!isSetup_)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				try
				{
					StringBuilder stringBuilder = new StringBuilder();
					lock (lock_)
					{
						stringBuilder.Append('1');
						for (int i = 0; i < analyticsBuffer_.Count; i++)
						{
							AnalyticsData analyticsData = analyticsBuffer_[i];
							if (analyticsData != null)
							{
								if (i != 0)
								{
									stringBuilder.Append('\n');
								}
								stringBuilder.Append(analyticsData.Url + "\t" + analyticsData.Retry + "\t" + analyticsData.Date);
							}
						}
					}
					PlayerPrefs.SetString("_AnalyticsTracker", stringBuilder.ToString());
					PlayerPrefs.Save();
				}
				catch
				{
				}
			}, true);
		}

		internal static void Setup()
		{
			if (isSetup_)
			{
				return;
			}
			isSetup_ = true;
			try
			{
				string text = PlayerPrefs.GetString("_AnalyticsTracker", null);
				if (text != null && text.Length > 0)
				{
					lock (lock_)
					{
						analyticsBuffer_ = new List<AnalyticsData>();
						text = text.Substring(1);
						if (text.Length > 0)
						{
							string[] array = text.Split('\n');
							for (int i = 0; i < array.Length; i++)
							{
								string[] array2 = array[i].Split('\t');
								AnalyticsData analyticsData = new AnalyticsData();
								analyticsData.Url = array2[0];
								analyticsData.Retry = int.Parse(array2[1]);
								analyticsData.Date = long.Parse(array2[2]);
								analyticsBuffer_.Add(analyticsData);
							}
						}
					}
				}
				else
				{
					string installReferrer = KairoPlugin.GetInstallReferrer();
					lock (lock_)
					{
						analyticsBuffer_ = new List<AnalyticsData>();
						if (installReferrer != null && installReferrer.Length > 0)
						{
							TrackPageView(true, "UA-35228877-1", "referrer", installReferrer);
						}
						Save();
					}
				}
				DispatchPageView();
			}
			catch
			{
			}
		}
	}
}
