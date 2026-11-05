using data;
using game;
using java.util;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;

namespace form
{
	public class RankForm : MyFormBase
	{
		private const int STATE_TOP = 0;

		private const int RESEVENT_SEB_BG = 0;

		private const int RESEVENT_SEB_CHARA = 1;

		private const int RESEVENT_SEB_BOX = 1;

		private const int RESEVENT_SEB_MINIHAPPY = 2;

		private int frame_;

		private int frame2_;

		private int page_;

		private int chara_;

		private Vector charaList_ = new Vector();

		private int state_;

		private static readonly int[][] softLabels_ = new int[1][] { new int[2] { 0, 2 } };

		private ResourceManager resEvent_;

		public RankForm()
			: base(3)
		{
		}

		public override string ToString()
		{
			return "RankForm:" + state_;
		}

		public override bool Init()
		{
			base.Init();
			if (charaList_.Size() > 0)
			{
				charaList_.RemoveAllElements();
			}
			Vector vector = new Vector();
			for (int i = 0; i < apdat_.pData_.Length; i++)
			{
				PlayerData playerData = apdat_.pData_[i];
				if (BitUtil.Check(playerData.flag_, 1))
				{
					vector.AddElement(playerData);
				}
			}
			for (int j = 0; j < vector.Size(); j++)
			{
				PlayerData obj = (PlayerData)vector.ElementAt(j);
				int num = GameUtil.Random(vector.Size());
				if (num != j)
				{
					vector.SetElementAt(vector.ElementAt(num), j);
					vector.SetElementAt(obj, num);
				}
			}
			for (int k = 0; k < 5; k++)
			{
				charaList_.AddElement(vector.ElementAt(k));
			}
			ChangeState(0);
			return true;
		}

		public override bool Update()
		{
			int num = 2;
			frame_ = (frame_ + 1) % int.MaxValue;
			if (state_ == 0)
			{
				if (canvas_.CheckKeyRepeat(65536))
				{
					page_ = (page_ - 1 + num) % num;
				}
				else if (canvas_.CheckKeyRepeat(262144))
				{
					page_ = (page_ + 1) % num;
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			return true;
		}

		public override void Draw(Graphics g)
		{
			int num = 0;
			int num2 = 13;
			num += (view_.GetGameWidth() - 240) / 2;
			num2 += (view_.GetGameHeight() - 240) / 2;
			g.SetOrigin(num, num2);
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
			int num = 2;
			Font font = g.GetFont(null);
			long netHighScore = apdat_.GetNetHighScore();
			long value = apdat_.GetHighScore();
			if (page_ == 1)
			{
				netHighScore = apdat_.GettMoneyNetHighScore();
				value = apdat_.GetMoneyHighScore();
			}
			if (state_ == 0)
			{
				if (MyFormBase.En())
				{
					apdat_.DrawWindow(g, 200, 170, 0, MyFormBase.LT("ハイスコア ") + " " + (page_ + 1) + "/" + num, -12);
				}
				else
				{
					apdat_.DrawWindow(g, 200, 170, 0, "ハイスコア " + (page_ + 1) + "/" + num, -12);
				}
				GameView.AddTouch(g, 1, 44 - frame_ % 20 / 5, 42, apdat_.resCom2_.seb[24], 0, 16);
				GameView.AddTouch(g, 1, 193 + frame_ % 20 / 5, 42, apdat_.resCom2_.seb[24], 1, 18);
				string text = "最高クリアポイント";
				if (page_ == 1)
				{
					text = MyFormBase.LT("<0>年までの最高資金", 8L);
				}
				int num2 = -9;
				g.SetColor(Graphics.GetColorOfRGB(60, 100, 200));
				g.DrawString(text, 120f - font.StringWidthF(text) / 2f, 68 + num2);
				num2 = 76;
				g.DrawImage(apdat_.resEvent_.img[11], 29f, num2 + 2);
				string text2 = apdat_.sysSave_.strs[0 + page_];
				int num3 = 0;
				int y = num2 + 51 + 20;
				int num4 = 26;
				UserData instance = UserData.GetInstance();
				for (int i = 0; i < charaList_.Size(); i++)
				{
					num3 = 120 - num4 * 2 + num4 * i;
					PlayerData playerData = (PlayerData)charaList_.ElementAt(i);
					int frame = frame_ % 20 / 5;
					instance.SetDispPlayerData(0, instance.selectUniform_, playerData.faceImg_, frame, 1);
					instance.DrawDispPlayer(g, num3, y);
				}
				num2 = 72;
				g.SetColor(Graphics.GetColorOfRGB(0, 0, 60));
				g.DrawString(text2, 120f - font.StringWidthF(text2) / 2f, 94 + num2);
				num2 = 68;
				g.SetColor(Graphics.GetColorOfRGB(60, 100, 200));
				g.DrawString("ﾊｲｽｺｱ", 58f, 118 + num2);
				string text3 = string.Empty;
				if (page_ == 0)
				{
					text3 = MyFormBase.UNI("P", value);
				}
				else if (page_ == 1)
				{
					text3 = MyFormBase.YEN("万円", value);
				}
				g.SetColor(Graphics.GetColorOfRGB(200, 100, 60));
				g.DrawString(text3, 186 - font.StringWidth(text3), 118 + num2);
			}
		}

		public override bool Resume()
		{
			return true;
		}

		public override bool Finish()
		{
			if (resEvent_ != null)
			{
				resEvent_.Dispose();
			}
			resEvent_ = null;
			return true;
		}

		private void ChangeState(int state)
		{
			state_ = state;
			SetSoftLabel();
		}

		private void SetSoftLabel()
		{
			int[] array = softLabels_[state_];
			if (array == null) { return; }
{
int num = array[0];
int num2 = array[1];
SetSoftLabel(AppData.softLabels_[num], AppData.softLabels_[num2]);
}
		}

		public override int GetTouchValue(int type, int index)
		{
			return 0;
		}

		public override void SetTouchValue(int type, int value)
		{
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
