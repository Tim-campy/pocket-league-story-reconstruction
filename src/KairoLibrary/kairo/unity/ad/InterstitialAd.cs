using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using java.lang;
using kairo.unity.native;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ad
{
	public class InterstitialAd : IDisposable
	{
		public enum State
		{
			None = -1,
			Load,
			Loaded,
			Show,
			Close
		}

		private string adUnitId_;

		private static bool showLoading_;

		private AndroidJavaObject object_;

		public void SetAdUnitId(string adUnitId)
		{
			if (adUnitId.Contains('@'))
			{
				if (Language.Japanese()) { adUnitId = Regex.Match(adUnitId, "@ja\\s+([^\\s@]+)").Groups[1].Value; } else { adUnitId = Regex.Match(adUnitId, "@en\\s+([^\\s@]+)").Groups[1].Value; }
			}
			adUnitId_ = adUnitId;
		}

		public bool Load()
		{
			if (adUnitId_ == null)
			{
				throw new ArgumentException("広告IDが未設定");
			}
			object_ = AdMobPlugin.LoadInterstitialAd(adUnitId_);
			return object_ != null;
		}

		public void Show()
		{
			if (GetState() == State.Load)
			{
				showLoading_ = true;
				long num = JSystem.CurrentTimeMillis();
				while (GetState() == State.Load)
				{
					JThread.Sleep(10L);
					long num2 = JSystem.CurrentTimeMillis() - num;
					if (num2 < 0 || num2 > 20000)
					{
						break;
					}
				}
				showLoading_ = false;
			}
			if (object_ == null) { return; }
AdMobPlugin.ShowInterstitialAd(object_);
while (GetState() == State.Show)
				{
					Thread.Sleep(10);
				}
Dispose();
		}

		public State GetState()
		{
			if (object_ == null)
			{
				return State.None;
			}
			return (State)AdMobPlugin.GetInterstitialAdState(object_);
		}

		public void Dispose()
		{
			if (object_ != null)
			{
				object_.Dispose();
			}
			object_ = null;
		}

		public bool IsLoaded()
		{
			if (GetState() == State.Loaded)
			{
				return true;
			}
			if (GetState() == State.Show)
			{
				return true;
			}
			return false;
		}

		public static void DrawLoading(kairo.unity.ui.Graphics g)
		{
			if (!showLoading_)
			{
				return;
			}
			float scaleRatio = IApplication.GetCurrentApp().GetScaleRatio();
			g.GetScale();
			g.Scale(scaleRatio);
			g.PushFont(10);
			try
			{
				string text = (Language.Japanese() ? "広告読み込み中" : "Loading Advert");
				int num = g.StringWidth(text + "...");
				int height = g.GetFont().GetHeight();
				float num2 = (float)(Screen.width * 100) / scaleRatio / 2f - (float)(num / 2);
				float num3 = (float)(Screen.height * 100) / scaleRatio / 2f - (float)(height / 2);
				int num4 = (int)(JSystem.CurrentTimeMillis() / 200 % 4);
				for (int i = 0; i < num4; i++)
				{
					text += ".";
				}
				g.SetColor(0, 0, 0, 128);
				g.FillRect(num2 - 12f, num3 - 2f, num + 24, height + 4);
				g.SetColor(255, 255, 255);
				g.DrawRect2(num2 - 13f, num3 - 3f, num + 26, height + 6);
				g.SetColor(255, 255, 255);
				g.DrawString(text, num2, num3);
			}
			catch
			{
			}
			g.PopFont();
			g.Scale(scaleRatio);
		}
	}
}
