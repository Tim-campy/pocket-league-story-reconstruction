using game;
using java.io;
using java.lang;
using kairo.unity.util;
using main;

namespace data
{
	public class QuestMatchData : BaseData
	{
		public const int FLAG_INIT = 1;

		public const int FLAG_VICTORY = 2;

		public const int MATCHFORM_RANDOM = 0;

		public const int MATCHFORM_TOURNAMENT = 1;

		public const int MATCHFORM_1MATCH = 2;

		public const int APPOINT_LEAGUE_NOTHING = -1;

		public const int APPOINT_LEAGUE_EVEN = 0;

		public const int APPOINT_LEAGUE_DOWN = 1;

		public const int APPOINT_LEAGUE_UP = 2;

		public const int STATE_NOT_FOUND = 0;

		public const int STATE_FOUND_INIT = 1;

		public const int STATE_FOUND = 2;

		public const int TROPHY_INIT = 0;

		public const int TROPHY_JOIN = 1;

		public const int TROPHY_COPPER = 2;

		public const int TROPHY_SILVER = 3;

		public const int TROPHY_GOLD = 4;

		public const int RANK_MAX = 6;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public int matchForm_;

		public int[] matchEnemy_;

		public int matchLeague_;

		public int[][] execsScript_;

		public int[][] execsScript2_;

		public int victoryPrize_;

		public int matchPrize_;

		public int joinCost_;

		public int schedule_;

		public int rank_;

		public string explain_;

		public int state_;

		public int trophy_;

		public bool new_;

		public QuestMatchData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public QuestMatchData(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			state_ = 0;
			trophy_ = 0;
			new_ = false;
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			matchForm_ = JInteger.ParseInt(array[num++]);
			matchEnemy_ = StringUtil.SplitIntegers(array[num++], ",");
			matchLeague_ = JInteger.ParseInt(array[num++]);
			execsScript_ = EventData.ReadScript(array[num++]);
			execsScript2_ = EventData.ReadScript(array[num++]);
			victoryPrize_ = JInteger.ParseInt(array[num++]);
			matchPrize_ = JInteger.ParseInt(array[num++]);
			joinCost_ = JInteger.ParseInt(array[num++]);
			schedule_ = JInteger.ParseInt(array[num++]);
			rank_ = JInteger.ParseInt(array[num++]);
			explain_ = array[num++];
			flag_ = JInteger.ParseInt(array[num++]);
		}

		public void NewGame()
		{
			state_ = 0;
			trophy_ = 0;
			if (BitUtil.Check(flag_, 1))
			{
				ChangeState(1);
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteShort(os, (short)trophy_);
			StreamUtil.WriteBoolean(os, new_);
			StreamUtil.WriteShort(os, (short)matchEnemy_.Length);
			for (int i = 0; i < matchEnemy_.Length; i++)
			{
				StreamUtil.WriteShort(os, (short)matchEnemy_[i]);
			}
		}

		public void Deserialize(InputStream stream)
		{
			state_ = StreamUtil.ReadShort(stream);
			trophy_ = StreamUtil.ReadShort(stream);
			new_ = StreamUtil.ReadBoolean(stream);
			int num = StreamUtil.ReadShort(stream);
			matchEnemy_ = new int[num];
			for (int i = 0; i < matchEnemy_.Length; i++)
			{
				matchEnemy_[i] = StreamUtil.ReadShort(stream);
			}
		}

		public void ChangeState(int next_state)
		{
			if (state_ == 0 && next_state != 0)
			{
				new_ = true;
			}
			state_ = next_state;
		}

		public void SetGenericData()
		{
			int num = 0;
			for (int i = 0; i < matchEnemy_.Length; i++)
			{
				TeamData teamData = apdat_.tData_[matchEnemy_[i]];
				num += teamData.warPotential_;
			}
			float num2 = GameUtil.RateConvert(matchEnemy_.Length, 1f, 10f, 1f, 5f, true);
			float num3 = GameUtil.RateConvert(matchEnemy_.Length, 1f, 10f, 1f, 2f, true);
			int x = (int)((float)(num / matchEnemy_.Length) * num2);
			int x2 = (int)((float)(num / matchEnemy_.Length) * num3);
			joinCost_ = GameUtil.RateConvert(x, 0, 1000, 50, 2000, false);
			joinCost_ = AppData.ClampMin(joinCost_ - joinCost_ % 100, 100);
			rank_ = GameUtil.RateConvert(x2, 0, 1300, 0, 6, true);
		}

		public static void RegistQuestMatch(int qmId)
		{
			UserData instance = UserData.GetInstance();
			instance.SetMatchInfoJoin(1);
			instance.selectQmId_ = qmId;
			instance.battleQmIndex_ = 0;
			instance.matchFlgVictory_ = 0;
		}

		public void SetGetTrophy(int trophy)
		{
			if (trophy_ < trophy)
			{
				trophy_ = trophy;
			}
		}

		public static bool IsVictoryQuestMatch()
		{
			int matchInfoJoin = udat_.GetMatchInfoJoin();
			if (matchInfoJoin != 1)
			{
				return false;
			}
			if (udat_.matchFlgVictory_ == 1)
			{
				return false;
			}
			QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
			if (udat_.battleQmIndex_ == questMatchData.matchEnemy_.Length - 1 && udat_.gm_.score_[0] > udat_.gm_.score_[1])
			{
				return true;
			}
			return false;
		}

		public static bool IsVictoryMatch()
		{
			int matchInfoJoin = udat_.GetMatchInfoJoin();
			if (matchInfoJoin != 1)
			{
				return false;
			}
			if (udat_.matchFlgVictory_ == 1)
			{
				return false;
			}
			QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
			if (questMatchData.matchEnemy_.Length == 1)
			{
				return false;
			}
			if (udat_.battleQmIndex_ == questMatchData.matchEnemy_.Length - 1)
			{
				return true;
			}
			return false;
		}

		public static bool IsExistNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.qmData_.Length; i++)
			{
				if (instance.qmData_[i].IsNew())
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.qmData_.Length; i++)
			{
				instance.qmData_[i].new_ = false;
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
