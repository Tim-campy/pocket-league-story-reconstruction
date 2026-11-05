using game;
using java.io;
using java.lang;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace data
{
	public class SponsorData : BaseData
	{
		public const int FLAG_GROUND = 1;

		public const int STATE_UNFINDED = 0;

		public const int STATE_FINDED = 1;

		public const int STATE_CONTRACT = 2;

		public const int SATISFACTEVA_MIN = 0;

		public const int SATISFACTEVA_MAX = 100;

		public const int EVA_DIFFICULT_MIN_MIN = 5;

		public const int EVA_DIFFICULT_MAX_MIN = 20;

		public const int EVA_DIFFICULT_MIN_MAX = 1;

		public const int EVA_DIFFICULT_MAX_MAX = 3;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public int charaIndex_;

		public int privilegeType_;

		public int privilegeValue_;

		public int costType_;

		public int cost_;

		public int[] income_ = new int[2];

		public int satisfactDifficult_;

		public int difficult_;

		public int feelPtInt_;

		public string explain_;

		public int[][] execsScript_;

		public int state_;

		public int satisfactPoint_;

		public int feelPt_;

		public bool new_;

		private static readonly string[] difficultStr = new string[3] { "簡単", "普通", "難しい" };

		public SponsorData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public SponsorData(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			state_ = 0;
			satisfactPoint_ = 0;
			feelPt_ = 0;
			new_ = false;
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			charaIndex_ = JInteger.ParseInt(array[num++]);
			privilegeType_ = JInteger.ParseInt(array[num++]);
			privilegeValue_ = JInteger.ParseInt(array[num++]);
			costType_ = JInteger.ParseInt(array[num++]);
			cost_ = JInteger.ParseInt(array[num++]);
			income_[0] = JInteger.ParseInt(array[num++]);
			income_[1] = JInteger.ParseInt(array[num++]);
			satisfactDifficult_ = JInteger.ParseInt(array[num++]);
			difficult_ = JInteger.ParseInt(array[num++]);
			feelPtInt_ = JInteger.ParseInt(array[num++]);
			explain_ = array[num++];
			execsScript_ = EventData.ReadScript(array[num++]);
			flag_ = JInteger.ParseInt(array[num++]);
			feelPt_ = feelPtInt_;
		}

		public void NewGame()
		{
			state_ = 0;
			if (BitUtil.Check(flag_, 1))
			{
				ChangeState(1);
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteInt(os, satisfactPoint_);
			StreamUtil.WriteShort(os, (short)feelPt_);
			StreamUtil.WriteBoolean(os, new_);
		}

		public void Deserialize(InputStream stream)
		{
			state_ = StreamUtil.ReadShort(stream);
			satisfactPoint_ = StreamUtil.ReadInt(stream);
			feelPt_ = StreamUtil.ReadShort(stream);
			new_ = StreamUtil.ReadBoolean(stream);
		}

		public void ChangeState(int state)
		{
			if (state_ == 0 && state == 1)
			{
				new_ = true;
			}
			state_ = state;
		}

		public static void Draw_sponsorChara(Graphics g, int charaId, int dx, int dy)
		{
			g.DrawImage(apdat_.resCom2_.img[169], dx - 10, dy - 30, 20 * (charaId % 12), 30 * (charaId / 12), 20, 30);
		}

		public int GetIncome()
		{
			return GameUtil.RateConvert(satisfactPoint_, 0, 100, income_[0], income_[1], true);
		}

		public static void UpdateSatisFactPoint()
		{
			TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[0]];
			TeamData teamData2 = apdat_.tData_[udat_.gm_.teamId_[1]];
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (udat_.gm_.score_[0] > udat_.gm_.score_[1])
			{
				flag = true;
			}
			else if (udat_.gm_.score_[0] == udat_.gm_.score_[1])
			{
				flag2 = true;
			}
			else
			{
				flag3 = true;
			}
			if (flag3)
			{
				return;
			}
			int x = teamData.warPotential_ - teamData2.warPotential_;
			int x2 = 0;
			if (flag)
			{
				x2 = GameUtil.RateConvert(x, -50, 50, 1000, 333, true);
			}
			else if (flag2)
			{
				x2 = GameUtil.RateConvert(x, -50, 50, 333, 0, true);
			}
			for (int i = 0; i < apdat_.sponsorData_.Length; i++)
			{
				SponsorData sponsorData = apdat_.sponsorData_[i];
				if (sponsorData.state_ == 2)
				{
					int x3 = sponsorData.satisfactDifficult_;
					int c2min = GameUtil.RateConvert(x3, 0, 1000, 5, 1, true);
					int c2max = GameUtil.RateConvert(x3, 0, 1000, 20, 3, true);
					int num = GameUtil.RateConvert(x2, 0, 1000, c2min, c2max, true);
					sponsorData.satisfactPoint_ = AppData.Clamp(sponsorData.satisfactPoint_ + num, 0, 100);
				}
			}
		}

		public string GetDifficultStr()
		{
			if (difficult_ < 50)
			{
				return difficultStr[0];
			}
			if (difficult_ < 100)
			{
				return difficultStr[1];
			}
			return difficultStr[2];
		}

		public static bool IsExistNew()
		{
			if (!UserData.GetInstance().ChEventFlg(32))
			{
				return false;
			}
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.sponsorData_.Length; i++)
			{
				if (instance.sponsorData_[i].IsNew())
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.sponsorData_.Length; i++)
			{
				instance.sponsorData_[i].new_ = false;
			}
		}

		public bool IsNew()
		{
			if (new_)
			{
				return true;
			}
			return false;
		}
	}
}
