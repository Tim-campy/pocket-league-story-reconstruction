using System;
using form;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.ad;
using kairo.unity.form;
using kairo.unity.io;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;
using system.billing;
using system.form;

namespace system.surface
{
	public class GameBanner : SurfaceBase
	{
		internal static GameBanner instance_;

		internal IApplication app_;

		internal Canvas canvas_;

		internal TouchEffectManager effMan_;

		internal static Image image_;

		internal static readonly string[] FILENAMES = new string[7] { "friend_banner_00.png", "ad_off_banner_00.png", "park_banner_00.png", "park_banner_01.png", "park_banner_02.png", "park_banner_03.png", "park_banner_04.png" };

		internal int current_;

		internal long currentBeginTime_;

		internal long showBeginTime_;

		internal JRandom random_;

		internal bool useAdmob_;

		internal string friendBanner_;

		private GameBanner()
		{
			app_ = IApplication.GetCurrentApp();
			canvas_ = Canvas.GetInstance();
			effMan_ = TouchEffectManager.GetInstance();
			random_ = new JRandom();
			current_ = -1;
			showBeginTime_ = JSystem.CurrentTimeMillis();
			useAdmob_ = true;
		}

		public static GameBanner GetInstance()
		{
			if (instance_ == null)
			{
				instance_ = new GameBanner();
			}
			return instance_;
		}

		public override bool IsGameSurface()
		{
			return false;
		}

		protected override void OnTouchEvent(TouchEvent evt)
		{
			TouchComponent component = evt.component;
			int type = evt.type;
			if (type == 5)
			{
				if (component.value == 0)
				{
					string text = friendBanner_ + "\nhttp://kairopark.jp/friend/?gm=" + Config.PID + "&dl=store\n\n";
					IApplication currentApp = IApplication.GetCurrentApp();
				}
				else if (component.value == 1)
				{
					FormManager instance = FormManager.GetInstance();
					MyBilling instance2 = MyBilling.GetInstance();
					BillingForm billingForm = new BillingForm(3);
					billingForm.item_ = instance2.GetItem(0);
					instance.Push(billingForm);
				}
				else
				{
					Canvas.OpenBrowser(Config.URL_SITE);
				}
			}
		}

		public override void Draw(Graphics g)
		{
			SurfaceManagerBase surfaceManager = canvas_.GetSurfaceManager();
			if (surfaceManager != null && surfaceManager.IsRotating())
			{
				return;
			}
			if (Visible())
			{
				AdMobView adMobView = canvas_.GetAdMobView();
				if (useAdmob_ && adMobView != null && !adMobView.IsStart() && JSystem.CurrentTimeMillis() - showBeginTime_ >= 10000)
				{
					adMobView.Start();
				}
				if ((adMobView == null || !adMobView.IsStart()) && (image_ == null || JSystem.CurrentTimeMillis() - currentBeginTime_ > 20000))
				{
					Load();
				}
				g.SetColor(0, 0, 0);
				g.FillRect(GetX(), GetY(), GetWidth(), GetHeight());
				if (image_ != null)
				{
					float num = (float)GetHeight() / (float)image_.GetHeight();
					float num2 = (float)image_.GetWidth() * num;
					float num3 = (float)image_.GetHeight() * num;
					float num4 = ((float)GetWidth() - num2) / 2f;
					float num5 = GetY();
					if (CheckFirstTouch(0))
					{
						g.DrawScaledImage(image_, num4, num5, num2, num3, 0, 0, image_.GetWidth(), image_.GetHeight());
					}
					else
					{
						g.SetRenderMode(1, 240, 15);
						g.DrawScaledImage(image_, num4, num5, num2, num3, 0, 0, image_.GetWidth(), image_.GetHeight());
						g.SetRenderMode();
					}
					AddTouchComponent(0, (int)(num4 - (float)GetX()), (int)(num5 - (float)GetY()), (int)num2, (int)num3, current_, null);
				}
			}
			else
			{
				Unload();
			}
		}

		public override void RotateClear()
		{
		}

		internal void Load()
		{
			JarInflater jarInflater = null;
			try
			{
				jarInflater = new JarInflater(Storage.Read(3, false, "banner.dat"));
				friendBanner_ = JString.New(jarInflater.GetData("friend_banner.txt"));
				Vector vector = new Vector();
				for (int i = 0; i < FILENAMES.Length; i++)
				{
					if (image_ == null && i != 0) { continue; }
if (i == current_) { continue; }
if (jarInflater.GetSize(FILENAMES[i]) == -1) { continue; }
					if (i == 1)
					{
						bool flag = true;
						AppData instance = AppData.GetInstance();
						FormManager instance2 = FormManager.GetInstance();
						for (int j = 0; j < instance2.GetFormsNum(); j++)
						{
							FormBase form = instance2.GetForm(j);
							if (form == instance.frmBoot_ || form == instance.frmTitle_)
							{
								flag = false;
							}
						}
						if (!flag)
						{
							continue;
						}
					}
					vector.AddElement(new JInteger(i));
				}
				Unload();
				if (vector.Size() > 0)
				{
					int index = JMath.Abs(random_.NextInt() % vector.Size());
					current_ = ((JInteger)vector.ElementAt(index)).IntValue();
					image_ = Image.Load(jarInflater.GetData(FILENAMES[current_]));
					currentBeginTime_ = JSystem.CurrentTimeMillis();
				}
			}
			catch (Exception)
			{
				Unload();
			}
			finally
			{
				if (jarInflater != null)
				{
					jarInflater.Close();
				}
				jarInflater = null;
			}
		}

		internal void Unload()
		{
			if (image_ != null)
			{
				image_.Dispose();
			}
			image_ = null;
			current_ = -1;
		}

		internal bool Visible()
		{
			if (app_.GetHeight() <= GetY())
			{
				return false;
			}
			AdMobView adMobView = canvas_.GetAdMobView();
			if (adMobView != null && adMobView.IsLoad())
			{
				return false;
			}
			return true;
		}
	}
}
