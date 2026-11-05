using System;
using form;
using java.lang;
using kairo.unity.io;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace system.form
{
	public class ConnectForm : MyFormBase
	{
		public static int FORMID_CONNECT = 902;

		public static int frame_;

		public static bool close_;

		private static ConnectForm form_;

		internal static ResourceManager resConnect_;

		public ConnectForm()
			: base(FORMID_CONNECT)
		{
		}

		public override string ToString()
		{
			return "ConnectForm";
		}

		public override bool Init()
		{
			base.Init();
			apdat_ = AppData.GetInstance();
			canvas_ = Canvas.GetInstance();
			formMan_ = FormManager.GetInstance();
			if (resConnect_ == null)
			{
				resConnect_ = new ResourceManager(null, null, null, null, null, null, null).SetRecordNameBits(0);
				resConnect_.Load(Storage.Read(3, false, "connect.dat"));
			}
			frame_ = 0;
			close_ = false;
			SetSoftLabel(string.Empty, string.Empty);
			return true;
		}

		public override bool Update()
		{
			base.Update();
			frame_ = (frame_ + 1) % int.MaxValue;
			if (close_)
			{
				return Pop();
			}
			try
			{
				JThread.Yield();
				JThread.Sleep(50L);
			}
			catch (Exception)
			{
			}
			return true;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			g.SetOrigin((view_.GetGameWidth() - 240) / 2, (view_.GetGameHeight() - 240) / 2 + 13);
			try
			{
				_draw(g);
			}
			finally
			{
				g.SetOrigin(0, 0);
			}
		}

		private void _draw(Graphics g)
		{
			Log.Info("ConnectForm._draw()");
		}

		public static void DrawConnect(Graphics g)
		{
			try
			{
				if (form_ == null)
				{
					form_ = new ConnectForm();
					form_.Init();
				}
				form_.Draw(g);
				form_.Update();
			}
			catch (Exception)
			{
			}
		}

		public override bool Resume()
		{
			return base.Resume();
		}

		public override bool Finish()
		{
			return base.Finish();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
