using form;
using kairo.unity.io;
using kairo.unity.surface;
using kairo.unity.system.lineup.ui;
using kairo.unity.ui;
using main;
using surface;
using system.lineup;

namespace system.form
{
	public class LineupForm : MyFormBase
	{
		public const int FORMID_LINEUP = 903;

		public const int TYPE_MAIN = 0;

		internal const int RESLINEUPLAY_SEB_BG = 0;

		public int type_;

		private static readonly int[][] configData_ = new int[1][] { new int[2] };

		private int select_;

		private int loopCounter_;

		private int frame_;

		private LineupData[] linups_;

		private TextLayout lay_;

		private int[] clip_;

		private bool push_;

		internal static ResourceManager resLineupLayout_;

		public LineupForm()
			: base(903)
		{
		}

		public LineupForm(int type)
			: base(903)
		{
			type_ = type;
			select_ = 0;
		}

		public override string ToString()
		{
			return "LineupForm:" + type_;
		}

		public override bool Init()
		{
			base.Init();
			MyLineup instance = MyLineup.GetInstance();
			resLineupLayout_ = new ResourceManager(null, null, null, null, null, null, null).SetRecordNameBits(0);
			resLineupLayout_.Load(Storage.Read(3, false, "lineup_layout.dat"));
			linups_ = instance.GetShowableLineups();
			select_ = linups_.Length;
			apdat_.SaveSystem();
			SetSoftlabel();
			return true;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			Font.PushLocalizeSetting(false);
			int[] sprite = resLineupLayout_.seb[0].GetSprite(0, 0);
			int num = (view_.GetGameWidth() - 240) / 2;
			int num2 = (view_.GetGameHeight() - (sprite[5] + 30)) / 2;
			g.SetColor(255, 255, 255);
			g.DrawString(KairoText.LT("オススメアプリ情報"), 120f, num2 - 31, 2);
			g.DrawScaledImage(resLineupLayout_.img[sprite[1]], 0f, num2, view_.GetGameWidth(), sprite[5], sprite[2], sprite[3], sprite[4], sprite[5]);
			LineupData lineupData = linups_[0];
			g.DrawImage(lineupData.GetImage(), num + 5, num2 + 9);
			int num3 = num + 40 + 33;
			int num4 = num2 + 12;
			if (select_ < linups_.Length && push_)
			{
				g.SetColor(Graphics.GetColorOfRGB(255, 187, 76));
				g.FillRect(num3, num4 + 22 - 26, 160f, 61f);
			}
			if (lay_ == null)
			{
				string explain = lineupData.GetExplain();
				int num5 = 58;
				lay_ = new TextLayout();
				lay_.SetSize(160f, num5);
				lay_.SetAnchor(34);
				lay_.SetColor(Graphics.GetColorOfRGB(255, 255, 254));
				lay_.SetLineSpace(8);
				lay_.SetText(explain);
				lay_.Flush();
				int height = lay_.GetHeight();
				if (height <= num5 / 2 + 6 + 1)
				{
					clip_ = new int[3] { 19, 38, 58 };
				}
				else if (height <= num5 / 2 + 16 + 1)
				{
					clip_ = new int[2] { 29, 58 };
				}
				else
				{
					clip_ = new int[3] { 19, 38, 58 };
				}
			}
			int num6 = frame_ - 10;
			if (num6 >= 0 && lay_ != null)
			{
				int num7 = num6 / 10;
				if (!push_ && num7 >= clip_.Length)
				{
					push_ = true;
				}
				if (num7 >= clip_.Length)
				{
					num7 = clip_.Length - 1;
				}
				g.ClipRect(0f, num4 - 2, view_.GetGameWidth(), clip_[num7]);
				lay_.Draw(g, num3, num4 - 2);
				g.PopClip();
			}
			if (push_)
			{
				GameView.AddTouch(g, 2, 0, num2, view_.GetGameWidth(), 77, 131072);
			}
			int num8 = num2 + 109;
			if (push_)
			{
				g.SetColor(79, 79, 79);
				g.FillRect(num + 49, num8, 148f, 37f);
				g.SetColor(217, 237, 255);
				g.FillRect(num + 49 + 1, num8 + 1, 146f, 1f);
				g.SetColor(108, 167, 255);
				g.FillRect(num + 49 + 1, num8 + 2, 146f, 34f);
				g.SetColor(255, 255, 254);
				int[] array = DrawCommand(g, KairoText.LT("タイトル画面へ"), 120, num8 + 14, 2, select_ == linups_.Length, 5, false);
				TouchOption opt = TouchOption.Create(256).Margin(50, 50, 10, 20);
				GameView.AddTouch(g, 2, array[0], array[1], array[2], array[3], 0x20000 | linups_.Length, opt);
			}
			Font.PopLocalizeSetting();
		}

		public override bool Finish()
		{
			base.Finish();
			if (resLineupLayout_ != null)
			{
				resLineupLayout_.Dispose();
			}
			resLineupLayout_ = null;
			MyLineup instance = MyLineup.GetInstance();
			instance.UnloadImage();
			return true;
		}

		public override bool Resume()
		{
			base.Resume();
			return true;
		}

		public override bool Update()
		{
			base.Update();
			frame_ = (frame_ + 1) % int.MaxValue;
			if (push_)
			{
				int num = linups_.Length + 1;
				if (canvas_.CheckKeyPulse(131072))
				{
					select_ = (select_ - 1 + num) % num;
				}
				if (canvas_.CheckKeyPulse(524288))
				{
					select_ = (select_ + 1 + num) % num;
				}
				if ((canvas_.CheckKeyPulse(1048576))) {
					if (select_ < linups_.Length) {
					Canvas.OpenBrowser(linups_[select_].GetURL());
				} else {
						return Pop();
					}}
			}
			formMan_.DrawAllForms();
			SetSoftlabel();
			return true;
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, int anchor, bool active, int margin, bool cursor)
		{
			return apdat_.DrawCommand(g, str, x, y, anchor, active, margin, cursor, this, loopCounter_);
		}

		public override int GetTouchValue(int type, int index)
		{
			type = (int)(type & 0xFFFF0000u);
			int num = type;
			if (num != 131072) { } else {
				return select_;
			}
			return base.GetTouchValue(type, index);
		}

		public override void SetTouchValue(int type, int value)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type) {case 131072: select_ = value; break; default: base.SetTouchValue(type,value); break;}
		}

		public override bool CheckMainWindow()
		{
			return false;
		}

		public void SetSoftlabel()
		{
			int num = configData_[type_][0];
			int num2 = configData_[type_][1];
			SetSoftLabel(AppData.softLabels_[num], AppData.softLabels_[num2]);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
