using form;
using game;
using java.io;
using java.lang;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace data
{
	public class LeagueData : BaseData
	{
		public const int FLAG_INIT = 1;

		public const int STATE_NOT_FOUND = 0;

		public const int STATE_FOUND = 1;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public string name2_;

		public int[] matchEnemy_;

		public int[] matchEnemyNext_;

		public int[][] execsScript_;

		public int[][] execsScript2_;

		public int[][] execsScript3_;

		public int victoryPrize_;

		public int matchPrize_;

		public int joinCost_;

		public int schedule_;

		public int rank_;

		public string explain_;

		public int state_;

		public int trophy_;

		public int userPriOlder_;

		public bool new_;

		public LeagueData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public LeagueData(string src)
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
			userPriOlder_ = 0;
			new_ = false;
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			name2_ = array[num++];
			matchEnemy_ = StringUtil.SplitIntegers(array[num++], ",");
			execsScript_ = EventData.ReadScript(array[num++]);
			execsScript2_ = EventData.ReadScript(array[num++]);
			execsScript3_ = EventData.ReadScript(array[num++]);
			victoryPrize_ = JInteger.ParseInt(array[num++]);
			matchPrize_ = JInteger.ParseInt(array[num++]);
			joinCost_ = JInteger.ParseInt(array[num++]);
			schedule_ = JInteger.ParseInt(array[num++]);
			rank_ = JInteger.ParseInt(array[num++]);
			explain_ = array[num++];
			flag_ = JInteger.ParseInt(array[num++]);
			matchEnemyNext_ = new int[matchEnemy_.Length];
			for (int i = 0; i < matchEnemy_.Length; i++)
			{
				matchEnemyNext_[i] = matchEnemy_[i];
			}
		}

		public void NewGame()
		{
			state_ = 0;
			if (BitUtil.Check(flag_, 1))
			{
				ChangeState(1);
			}
			userPriOlder_ = -1;
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteShort(os, (short)trophy_);
			StreamUtil.WriteShort(os, (short)userPriOlder_);
			StreamUtil.WriteBoolean(os, new_);
			for (int i = 0; i < matchEnemy_.Length; i++)
			{
				StreamUtil.WriteShort(os, (short)matchEnemy_[i]);
			}
			for (int j = 0; j < matchEnemyNext_.Length; j++)
			{
				StreamUtil.WriteShort(os, (short)matchEnemyNext_[j]);
			}
		}

		public void Deserialize(InputStream stream)
		{
			state_ = StreamUtil.ReadShort(stream);
			trophy_ = StreamUtil.ReadShort(stream);
			userPriOlder_ = StreamUtil.ReadShort(stream);
			new_ = StreamUtil.ReadBoolean(stream);
			for (int i = 0; i < matchEnemy_.Length; i++)
			{
				matchEnemy_[i] = StreamUtil.ReadShort(stream);
			}
			for (int j = 0; j < matchEnemyNext_.Length; j++)
			{
				matchEnemyNext_[j] = StreamUtil.ReadShort(stream);
			}
		}

		public void ChangeState(int state)
		{
			if (state_ == 0 && state == 1)
			{
				new_ = true;
			}
			state_ = state;
		}

		public static void RegistLeague(int leagueId)
		{
			UserData instance = UserData.GetInstance();
			instance.SetMatchInfoJoin(0);
			instance.selectLeagueId_ = leagueId;
			instance.League_init();
			instance.matchFlgVictory_ = 0;
		}

		public void SetGetTrophy(int trophy)
		{
			if (trophy_ < trophy)
			{
				trophy_ = trophy;
			}
		}

		public static bool IsLastMatch()
		{
			UserData instance = UserData.GetInstance();
			LeagueData leagueData = AppData.GetInstance().leagueData_[instance.selectLeagueId_];
			if (instance.GetMatchInfoJoin() != 0)
			{
				return false;
			}
			if (instance.league_setu_ == leagueData.matchEnemy_.Length - 2)
			{
				return true;
			}
			return false;
		}

		public static bool IsVictoryLeague()
		{
			if (udat_.GetMatchInfoJoin() != 0)
			{
				return false;
			}
			if (udat_.matchFlgVictory_ == 1)
			{
				return false;
			}
			int[] leagueData = udat_.GetLeagueData(0);
			if (leagueData[7] == 1)
			{
				LeagueData leagueData2 = apdat_.leagueData_[udat_.selectLeagueId_];
				int[] leagueData3 = udat_.GetLeagueData(udat_.league_dispOlder_[1][0]);
				int num = (leagueData2.matchEnemy_.Length - 1 - (udat_.league_setu_ + 1)) * 3;
				if (leagueData[0] > leagueData3[0] + num)
				{
					return true;
				}
				if (leagueData[0] == leagueData3[0] && udat_.league_setu_ == leagueData2.matchEnemy_.Length - 2 && leagueData[6] >= leagueData3[6])
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsVictoryMatch()
		{
			if (udat_.GetMatchInfoJoin() != 0)
			{
				return false;
			}
			int[] leagueData = udat_.GetLeagueData(0);
			if (leagueData[7] == 1 && udat_.matchFlgVictory_ == 0)
			{
				LeagueData leagueData2 = apdat_.leagueData_[udat_.selectLeagueId_];
				int[] leagueData3 = udat_.GetLeagueData(udat_.league_dispOlder_[1][0]);
				int num = leagueData[0] - leagueData3[0];
				int num2 = (leagueData2.matchEnemy_.Length - 1 - (udat_.league_setu_ + 1)) * 3;
				if (num > num2 - 3)
				{
					return true;
				}
			}
			return false;
		}

		public void SetLeagueInfoExplain(TextLayout var_out)
		{
			var_out.SetAnchor(2);
			var_out.SetSize(194f, 32f);
			var_out.SetColor(SubForm.SC_WINDOW_BLACK);
			var_out.SetLineSpace(5);
			var_out.SetText(explain_);
			var_out.Flush();
		}

		public static bool IsExistNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.leagueData_.Length; i++)
			{
				if (instance.leagueData_[i].IsNew())
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.leagueData_.Length; i++)
			{
				instance.leagueData_[i].new_ = false;
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
