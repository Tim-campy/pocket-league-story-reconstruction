using System;
using form;
using java.lang;
using kairo.common.cfg;
using kairo.unity.ad;
using kairo.unity.form;
using kairo.unity.io;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using system;
using system.billing;
using system.surface;

namespace surface
{
	public class SurfaceManager : SurfaceManagerBase
	{
		internal static SurfaceManager instance_;

		public static GameView view_;

		public static GamePad gpad_;

		public static GameBanner gban_;

		internal bool wide_;

		internal bool layoutMode_;

		private SurfaceManager()
		{
		}

		public static void Setup()
		{
			if (instance_ != null)
			{
				return;
			}
			instance_ = new SurfaceManager();
			IApplication currentApp = IApplication.GetCurrentApp();
			if (!currentApp.IsSide())
			{
				instance_.wide_ = currentApp.GetWidth() > currentApp.GetHeight();
			}
			else
			{
				instance_.wide_ = currentApp.GetWidth() < currentApp.GetHeight();
			}
			Canvas instance = Canvas.GetInstance();
			instance.SetSurfaceManager(instance_);
			JarInflater jarInflater = null;
			try
			{
				jarInflater = new JarInflater(Storage.Read(3, false, "system.dat"));
				GameView.Load(jarInflater);
				GamePad.Load(jarInflater);
			}
			catch (Exception)
			{
			}
			finally
			{
				if (jarInflater != null)
				{
					jarInflater.Close();
				}
				jarInflater = null;
			}
			instance_.surfaceList_.Add(view_ = GameView.GetInstance());
			instance_.surfaceList_.Add(gpad_ = GamePad.GetInstance());
			instance_.surfaceList_.Add(gban_ = GameBanner.GetInstance());
		}

		public override void AdjustSurfaceLayout()
		{
			AppData instance = AppData.GetInstance();
			Canvas instance2 = Canvas.GetInstance();
			TouchEffectManager instance3 = TouchEffectManager.GetInstance();
			FormManager instance4 = FormManager.GetInstance();
			if (instance4.GetFormsNum() == 0)
			{
				return;
			}
			FormBase form = instance4.GetForm(0);
			bool flag = gpad_.GetHeight() == 0;
			UpdateAdMobView();
			if (!instance2.IsGetStatusBarHeight())
			{
				instance2.GetStatusBarHeight();
			}
			if (!instance2.IsGetStatusBarPosition())
			{
				instance2.GetStatusBarPosition();
			}
			IApplication currentApp = IApplication.GetCurrentApp();
			int num = currentApp.GetWidth();
			int num2 = currentApp.GetHeight();
			int width = instance2.GetWidth2();
			int num3 = instance2.GetHeight2();
			if ((!wide_ && (currentApp.IsSide() ? (num < num2) : (num > num2))) || (wide_ && (currentApp.IsSide() ? (num > num2) : (num < num2))))
			{
				int num4 = num;
				num = num2;
				num2 = num4;
				num4 = width;
				width = num3;
				num3 = num4;
			}
			if ((Config.AD_SIZE == 4 || num2 > num) && Config.ADMOB_ENABLE)
			{
				num2 -= AdMobView.GetAdHeight();
				num3 -= AdMobView.GetAdHeight();
			}
			float scaleRatio = currentApp.GetScaleRatio(true);
			float num5 = 240f * scaleRatio / 100f;
			float num6 = 320f;
			view_.SetScaleRatio(currentApp.GetScaleRatio(false));
			gpad_.SetScaleRatio(num5 * 100f / num6);
			int width2 = view_.GetWidth();
			int height = view_.GetHeight();
			int width3 = gpad_.GetWidth();
			int height2 = gpad_.GetHeight();
			float scaleRatio2 = view_.GetScaleRatio();
			float scaleRatio3 = gpad_.GetScaleRatio();
			view_.SetSize((int)(240f * scaleRatio2 / 100f + 0.01f), (int)(240f * scaleRatio2 / 100f + 0.01f));
			gpad_.SetSize(view_.GetWidth(), num3);
			int num7 = (int)((float)GamePad.SIZE_FULL * scaleRatio3 / 100f + 0.01f);
			int num8 = (int)((float)GamePad.SIZE_PLAIN * scaleRatio3 / 100f + 0.01f);
			int num9 = (int)((float)GamePad.SIZE_PLAIN_MIN * scaleRatio3 / 100f + 0.01f);
			int num10 = num2 - view_.GetHeight();
			int mode = gpad_.GetMode();
			mode = ((num10 < num9) ? 2 : ((num10 < num7) ? 1 : ((instance.sysSave_ == null || instance.sysSave_.ints == null) ? 1 : instance.sysSave_.ints[15])));
			if (mode != gpad_.GetMode())
			{
				gpad_.SetMode(mode);
			}
			int num11 = 0;
			switch (mode)
			{
			case 0:
				num11 = num7;
				break;
			case 1:
				num10 -= instance2.GetStatusBarHeight();
				num11 = ((num10 >= num8) ? num8 : num10);
				break;
			}
			gpad_.SetHeight(num11);
			bool flag2 = false;
			if (mode == 2)
			{
				flag2 = true;
			}
			if (flag2 != instance2.IsFullscreen())
			{
				instance2.Fullscreen(flag2);
				if (flag)
				{
					try
					{
						JThread.Sleep(100L);
					}
					catch (Exception)
					{
					}
				}
			}
			width = instance2.GetWidth2();
			num3 = instance2.GetHeight2();
			if ((Config.AD_SIZE == 4 || num2 > num) && Config.ADMOB_ENABLE)
			{
				num3 -= AdMobView.GetAdHeight();
			}
			if (mode == 2)
			{
				view_.SetSize(width, num3);
				view_.SetLocation(0, 0);
			}
			else
			{
				int num12 = 0;
				if (layoutMode_)
				{
					int[] viewport = currentApp.GetViewport();
					num12 = (int)((float)viewport[1] * scaleRatio2 / 100f + 0.01f);
				}
				view_.SetHeight(num3 - num11);
				if (num12 > 0 && view_.GetHeight() > num12)
				{
					view_.SetHeight(num12);
				}
				view_.SetLocation((width - view_.GetWidth()) / 2, num3 - (view_.GetHeight() + num11));
			}
			gpad_.SetSize(width, num3);
			gpad_.SetLocation((width - gpad_.GetWidth()) / 2, 0);
			if (view_.GetY() < 0)
			{
				view_.SetLocation(view_.GetX(), 0);
			}
			if (width3 != gpad_.GetWidth() || height2 != gpad_.GetHeight())
			{
				instance3.RemoveSurface(gpad_);
			}
			if (width2 != view_.GetWidth() || height != view_.GetHeight())
			{
				instance3.RemoveSurface(view_);
			}
			gban_.SetLocation(0, num3);
			gban_.SetSize(num, AdMobView.GetAdHeight());
		}

		public override void Draw(Graphics g)
		{
			Canvas instance = Canvas.GetInstance();
			TouchEffectManager instance2 = TouchEffectManager.GetInstance();
			base.Draw(g);
			instance2.Draw(g);
			instance2.Frame();
		}

		public override void DrawComponents(Graphics g)
		{
			if (gpad_.visibleTouchRange_ && (gpad_.IsTouch() || view_.IsTouch()))
			{
				base.DrawComponents(g);
			}
		}

		public void UpdateAdMobView()
		{
			Canvas instance = Canvas.GetInstance();
			KairoService instance2 = KairoService.GetInstance();
			MyBilling instance3 = MyBilling.GetInstance();
			if (instance2.IsInit())
			{
				Config.ADMOB_ENABLE = !Config.UNITY_EDITOR && !instance3.GetItem(0).CheckProvision();
			}
			instance.UpdateAdMobView();
			GamePad.SIZE_PLAIN = 65;
		}

		public void SetLayoutMode(bool enable)
		{
			layoutMode_ = enable;
		}

		public bool IsLayoutMode()
		{
			return layoutMode_;
		}
	}
}
