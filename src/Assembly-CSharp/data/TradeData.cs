using game;
using java.io;
using java.lang;
using kairo.unity.util;
using main;

namespace data
{
	public class TradeData : BaseData
	{
		public const int FLAG_INIT = 1;

		public const int STATE_NOT_FOUND = 0;

		public const int STATE_FOUND = 1;

		public const int STATE_MEMBER = 2;

		public static AppData apdat_;

		public static UserData udat_;

		public int difficult_;

		public int feelPtInt_;

		public int getNameValue_;

		public int playerDataId_;

		public int[][] execsScript_;

		public int cost_;

		public int tradeGroup_;

		public int state_;

		public int feelPt_;

		public bool new_;

		public TradeData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public TradeData(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			state_ = 0;
			feelPt_ = 0;
			new_ = false;
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			string text = array[num++];
			difficult_ = JInteger.ParseInt(array[num++]);
			feelPtInt_ = JInteger.ParseInt(array[num++]);
			getNameValue_ = JInteger.ParseInt(array[num++]);
			playerDataId_ = JInteger.ParseInt(array[num++]);
			execsScript_ = EventData.ReadScript(array[num++]);
			cost_ = JInteger.ParseInt(array[num++]);
			tradeGroup_ = JInteger.ParseInt(array[num++]);
			flag_ = JInteger.ParseInt(array[num++]);
			feelPt_ = feelPtInt_;
		}

		public void NewGame()
		{
			if (state_ != 2)
			{
				state_ = 0;
				if (BitUtil.Check(flag_, 1))
				{
					state_ = 1;
				}
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteShort(os, (short)feelPt_);
			StreamUtil.WriteBoolean(os, new_);
		}

		public void Deserialize(InputStream stream)
		{
			state_ = StreamUtil.ReadShort(stream);
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

		public int GetSuccessRate()
		{
			TeamData teamData = AppData.GetInstance().tData_[0];
			int warPotential_ = teamData.warPotential_;
			int fanEvaToCommonValue = FanActData.GetFanEvaToCommonValue(2);
			int num = (int)((0.2f * (float)warPotential_ + 0.8f * (float)fanEvaToCommonValue) / 1f) / 10;
			int num2 = num - difficult_;
			int num3 = 0;
			if (num2 >= 0)
			{
				return GameUtil.RateConvert(num2, 0, 20, 50, 95, true);
			}
			if (num2 >= -20)
			{
				return GameUtil.RateConvert(num2, -20, 0, 5, 50, true);
			}
			return 5;
		}

		public static bool IsExistNew()
		{
			if (!UserData.GetInstance().ChEventFlg(16))
			{
				return false;
			}
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.tradeData_.Length; i++)
			{
				if (instance.tradeData_[i].IsNew())
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.tradeData_.Length; i++)
			{
				instance.tradeData_[i].new_ = false;
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

		public bool IsPlayerInMember()
		{
			PlayerData elem = apdat_.pData_[playerDataId_];
			int num = UserData.GetInstance().memberPDataist_.IndexOf(elem);
			if (num != -1)
			{
				return true;
			}
			return false;
		}
	}
}
