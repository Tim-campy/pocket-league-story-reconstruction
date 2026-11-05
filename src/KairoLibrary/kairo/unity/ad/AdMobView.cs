using System;
using kairo.common.cfg;
using kairo.unity.native;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ad
{
	public class AdMobView : IDisposable
	{
		private static AndroidJavaObject admob_;

		private static int adHeight_ = -1;

		public AdMobView(AndroidJavaObject admob)
		{
			admob_ = admob;
		}

		public void Start()
		{
			AdMobPlugin.Start(admob_);
		}

		public bool IsStart()
		{
			return AdMobPlugin.IsStart(admob_);
		}

		public bool IsLoad()
		{
			return AdMobPlugin.IsLoad(admob_);
		}

		public void Dispose()
		{
			AdMobPlugin.RemoveAdMobView(admob_);
			admob_ = null;
		}

		public static int GetAdHeight()
		{
			IApplication currentApp = IApplication.GetCurrentApp();
			if (Config.AD_SIZE == 4)
			{
				if (admob_ == null)
				{
					adHeight_ = 0;
				}
				else
				{
					adHeight_ = admob_.Call<int>("getAdHeight", new object[0]);
				}
			}
			else if (adHeight_ == -1)
			{
				adHeight_ = (int)(50f * DisplayMetrics.Density);
			}
			return (int)((float)adHeight_ * currentApp.GetResolutionScaleRatio() / 100f + 0.01f);
		}
	}
}
