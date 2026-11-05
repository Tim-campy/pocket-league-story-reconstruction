using System;
using java.lang;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.surface;
using kairo.unity.ui;
using main;

namespace form
{
	public class FormManager : FormManagerBase
	{
		private static FormManager instance_ = null;

		private long oldTime_;

		private long fpsTime_;

		private bool fpsUnlimited_;

		private int tagFps_;

		private int fps_;

		private int frameCount_;

		private int vibeTime_;

		private int vibeFlag_;

		private int lastTopFormId_;

		private int lastTopFormType_;

		private int lastTopFormChara_;

		public int loopCounter_;

		internal FormManager()
		{
			oldTime_ = JSystem.CurrentTimeMillis();
			SetTargetFps(20);
			fps_ = 1;
			lastTopFormId_ = -1;
			lastTopFormType_ = -1;
		}

		public static FormManager GetInstance()
		{
			if (instance_ == null)
			{
				instance_ = new FormManager();
			}
			return instance_;
		}

		public override void Resume()
		{
			base.Resume();
		}

		protected override void PreUpdate()
		{
			base.PreUpdate();
			FormBase topForm = GetTopForm();
			if (topForm.GetId() == 4)
			{
				SubForm subForm = (SubForm)topForm;
				if (topForm.GetId() == lastTopFormId_ && subForm.type_ == 0 && subForm.type_ == lastTopFormType_ && subForm.humanCharaIndex_ != lastTopFormChara_)
				{
					subForm.wait_ = 5;
				}
				lastTopFormType_ = subForm.type_;
				lastTopFormChara_ = subForm.humanCharaIndex_;
			}
			lastTopFormId_ = topForm.GetId();
		}

		protected override void Updated()
		{
			AppData instance = AppData.GetInstance();
			int num = tagFps_;
			if (Config.DEBUG_SLOW && Canvas.GetInstance().CheckKeyState(1024))
			{
				num = 1;
			}
			int num2 = 1000 / num;
			if (!fpsUnlimited_)
			{
				try
				{
					Canvas instance2 = Canvas.GetInstance();
					SurfaceManagerBase surfaceManager = instance2.GetSurfaceManager();
					while (0 <= JSystem.CurrentTimeMillis() - oldTime_ && JSystem.CurrentTimeMillis() - oldTime_ < num2)
					{
						JThread.Sleep(0L);
						surfaceManager.CheckScreenRotation();
					}
				}
				catch (Exception)
				{
				}
			}
			oldTime_ = JSystem.CurrentTimeMillis();
			long num3 = JSystem.CurrentTimeMillis() - oldTime_;
			if (num3 < 0)
			{
				num3 = num2;
			}
			if (JSystem.CurrentTimeMillis() - fpsTime_ >= 1000)
			{
				fps_ = frameCount_;
				if (fps_ <= 0)
				{
					fps_ = 1;
				}
				frameCount_ = 0;
				fpsTime_ = JSystem.CurrentTimeMillis();
			}
			else
			{
				frameCount_++;
			}
			loopCounter_ = (loopCounter_ + 1) % int.MaxValue;
			base.Updated();
		}

		protected override void PreDraw()
		{
			base.PreDraw();
		}

		protected override void Drawn()
		{
			base.Drawn();
		}

		public int GetFps()
		{
			return fps_;
		}

		public void StartVibrator(int ms)
		{
			Canvas.Vibrator(ms);
		}

		public void StopVibrator()
		{
			vibeFlag_ = 0;
		}

		public void SetTargetFps(int fps)
		{
			tagFps_ = fps + 1;
		}

		public int GetTargetFps()
		{
			return tagFps_ - 1;
		}
	}
}
