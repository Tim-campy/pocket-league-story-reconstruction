using game;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace data
{
	public class FanBonusData : BaseData
	{
		public const int FLAG_GROUND = 1;

		public const int FLAG_POPULARUP = 2;

		public const int FLAG_HISTORY_DISP = 4;

		public const int FLAG_LEAGUE_AMA = 8;

		public const int FLAG_LEAGUE_REGI = 16;

		public const int FLAG_LEAGUE_ALL = 32;

		public const int FLAG_LEAGUE_WORLD = 64;

		public const int FLAG_LEAGUE_KAIRO = 128;

		public const int FLAG_GOAL_50 = 256;

		public const int FLAG_GOAL_100 = 512;

		public const int BPF_UP_FAN = 0;

		public const int BPF_UP_ASSIST = 1;

		public const int BPF_UP_NAMEVALUE = 2;

		public const int BPF_HNUM = 3;

		public const int BPF_HACT = 4;

		public const int BPF_END = 5;

		public const int BP_HACT_JAMPGLAD = 0;

		public const int BP_HACT_SAD = 1;

		public const int BP_HACT_KICK = 2;

		public const int TERM_MODE_WIN = 0;

		public const int TERM_MODE_DRAW = 13;

		public const int TERM_MODE_LOSE = 1;

		public const int TERM_MODE_WIN_CONSECUTIVE = 2;

		public const int TERM_MODE_VICTORY_QUEST_1M = 15;

		public const int TERM_MODE_VICTORY_QUEST_T = 16;

		public const int TERM_MODE_VICTORY_LEAGUE = 3;

		public const int TERM_MODE_SCORERANKING = 14;

		public const int TERM_MODE_SCORE3 = 4;

		public const int TERM_MODE_HATTRICK = 5;

		public const int TERM_MODE_GAMEPLAYNUM = 6;

		public const int TERM_MODE_GOAL_CONSECUTIVE = 7;

		public const int TERM_MODE_GOAL_TOTAL = 8;

		public const int TERM_MODE_DEBUT_YOUTH = 9;

		public const int TERM_MODE_DEBUT_TRADE = 10;

		public const int TERM_MODE_SVISOR_WINNUM = 11;

		public const int TERM_MODE_CUSTOMER_TOTAL = 12;

		public const int BFD_TOPSTR = 0;

		public const int BFD_TOPSTRLEN = 1;

		public const int BFD_PARAM = 2;

		public const int BFD_DISPTYPE = 3;

		public const int BFD_CHARAINDEXARY = 4;

		public const int BFD_POPULAR = 5;

		public const int DISPCHARATYPE_GENERIC = 0;

		public const int DISPCHARATYPE_PLAYER = 1;

		public const int DISPCHARATYPE_SVISOR = 2;

		public const int ADDPOPULAR_GENERIC = 1;

		public const int ADDPOPULAR_PLAYER = 3;

		public const int ACHIVE_YEAR = 0;

		public const int ACHIVE_MONTH = 1;

		public const int ACHIVE_WEEK = 2;

		public const int STATE_NOT_FOUND = 0;

		public const int STATE_FOUND = 1;

		public static AppData apdat_;

		public static UserData udat_;

		public string message_;

		public string info_name_;

		public int termMode_;

		public int termModeIndex_;

		public int termModeValue_;

		public int dispCharaType_;

		public int[] param = new int[5];

		public int state_;

		public int[] achiveDay_ = new int[3];

		public static readonly int[] TERA_MODE_LIST = new int[17]
		{
			0, 13, 1, 2, 15, 16, 3, 14, 4, 5,
			6, 7, 8, 9, 10, 11, 12
		};

		public static Vector bonus_f_data1 = new Vector();

		public static int FB_LEAGUE_AMA;

		public static int FB_LEAGUE_REGI;

		public static int FB_LEAGUE_ALL;

		public static int FB_LEAGUE_WORLD;

		public static int FB_LEAGUE_KAIRO;

		public static int FB_GOAL_50;

		public static int FB_GOAL_100;

		private static Vector fb_list = new Vector();

		public FanBonusData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public FanBonusData(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			state_ = 0;
			achiveDay_ = new int[3];
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			message_ = array[num++];
			info_name_ = array[num++];
			termMode_ = JInteger.ParseInt(array[num++]);
			termModeIndex_ = JInteger.ParseInt(array[num++]);
			termModeValue_ = JInteger.ParseInt(array[num++]);
			dispCharaType_ = JInteger.ParseInt(array[num++]);
			for (int i = 0; i < param.Length; i++)
			{
				param[i] = JInteger.ParseInt(array[num++]);
			}
			flag_ = JInteger.ParseInt(array[num++]);
		}

		public static void Load2()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.fanBonus_.Length; i++)
			{
				FanBonusData fanBonusData = instance.fanBonus_[i];
				if (BitUtil.Check(fanBonusData.flag_, 8))
				{
					FB_LEAGUE_AMA = i;
				}
				else if (BitUtil.Check(fanBonusData.flag_, 16))
				{
					FB_LEAGUE_REGI = i;
				}
				else if (BitUtil.Check(fanBonusData.flag_, 32))
				{
					FB_LEAGUE_ALL = i;
				}
				else if (BitUtil.Check(fanBonusData.flag_, 64))
				{
					FB_LEAGUE_WORLD = i;
				}
				else if (BitUtil.Check(fanBonusData.flag_, 128))
				{
					FB_LEAGUE_KAIRO = i;
				}
				else if (BitUtil.Check(fanBonusData.flag_, 256))
				{
					FB_GOAL_50 = i;
				}
				else if (BitUtil.Check(fanBonusData.flag_, 512))
				{
					FB_GOAL_100 = i;
				}
			}
			if (Config.DEBUG_CMD)
			{
				Log.Info("■FanBonusData.java ch " + FB_LEAGUE_AMA + ", " + FB_LEAGUE_REGI + ", " + FB_LEAGUE_ALL + ", " + FB_LEAGUE_WORLD + ", " + FB_LEAGUE_KAIRO);
			}
			FB_GOAL_50 = 41;
			FB_GOAL_100 = 42;
		}

		public void NewGame()
		{
			state_ = 0;
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)state_);
			for (int i = 0; i < achiveDay_.Length; i++)
			{
				StreamUtil.WriteShort(os, (short)achiveDay_[i]);
			}
		}

		public void Deserialize(InputStream stream)
		{
			state_ = StreamUtil.ReadShort(stream);
			for (int i = 0; i < achiveDay_.Length; i++)
			{
				achiveDay_[i] = StreamUtil.ReadShort(stream);
			}
		}

		public void ChangeState(int state)
		{
			state_ = state;
		}

		public static void SetBPFData(FanBonusData fb_data, string topMessage, int[] param)
		{
			SetBPFData(fb_data, topMessage, param, -1);
		}

		public static void SetBPFData(FanBonusData fb_data, string topMessage, int[] param, int charaDataId)
		{
			int[] array = null;
			if (fb_data.dispCharaType_ == 0)
			{
				array = new int[fb_data.param[3]];
				for (int i = 0; i < array.Length; i++)
				{
					PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(i);
					array[i] = playerData.id_;
				}
			}
			else if (fb_data.dispCharaType_ == 1)
			{
				array = new int[1] { charaDataId };
			}
			else if (fb_data.dispCharaType_ == 2)
			{
				array = new int[1] { udat_.selectSupervisorId_ };
			}
			int[] array2 = new int[param.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = param[j];
			}
			float ticketPriceFanBonusRate = udat_.GetTicketPriceFanBonusRate(apdat_.frmSoccer_.GetEvaCumstomer(true));
			for (int k = 0; k < 3; k++)
			{
				if (k != 1 || array2[k] >= 0)
				{
					array2[k] = (int)((float)array2[k] * ticketPriceFanBonusRate);
				}
			}
			int value = 0;
			if (BitUtil.Check(fb_data.flag_, 2))
			{
				value = 1;
			}
			Font font = new Font();
			bonus_f_data1.AddElement(new object[6]
			{
				topMessage,
				new JInteger(font.StringWidth(topMessage) + 20),
				array2,
				new JInteger(fb_data.dispCharaType_),
				array,
				new JInteger(value)
			});
			if (fb_data.state_ == 0)
			{
				fb_data.ChangeState(1);
				fb_data.achiveDay_[0] = apdat_.save_.ints[1];
				fb_data.achiveDay_[1] = apdat_.save_.ints[2];
				fb_data.achiveDay_[2] = apdat_.save_.ints[3];
			}
		}

		public static FanBonusData GetFanBonusData(int term_mode, int param)
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.fanBonus_.Length; i++)
			{
				FanBonusData fanBonusData = instance.fanBonus_[i];
				if (fanBonusData.termMode_ == term_mode && fanBonusData.termModeIndex_ == param)
				{
					return fanBonusData;
				}
			}
			return null;
		}

		public static Vector GetFanBonusDataList(int term_mode)
		{
			if (fb_list.Size() > 0)
			{
				fb_list.RemoveAllElements();
			}
			for (int i = 0; i < apdat_.fanBonus_.Length; i++)
			{
				FanBonusData fanBonusData = apdat_.fanBonus_[i];
				if (fanBonusData.termMode_ != term_mode) { continue; }
fb_list.AddElement(fanBonusData);
			}
			return fb_list;
		}

		public static void ChBonusEvent()
		{
			AppData instance = AppData.GetInstance();
			UserData instance2 = UserData.GetInstance();
			TeamData teamData = instance.tData_[instance2.gm_.teamId_[0]];
			TeamData teamData2 = instance.tData_[instance2.gm_.teamId_[1]];
			FanBonusData fanBonusData = null;
			FanBonusData fanBonusData2 = null;
			FanBonusData fanBonusData3 = null;
			for (int i = 0; i < TERA_MODE_LIST.Length; i++)
			{
				int num = TERA_MODE_LIST[i];
				switch (num)
				{
				case 0:
					if (instance2.gm_.score_[0] > instance2.gm_.score_[1])
					{
						int warPotential_5 = teamData.warPotential_;
						int warPotential_6 = teamData2.warPotential_;
						int num12 = warPotential_6 - warPotential_5;
						if (num12 < 100)
						{
							fanBonusData = GetFanBonusData(num, 0);
							SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
						}
						else if (num12 < 200)
						{
							fanBonusData = GetFanBonusData(num, 1);
							SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
						}
						else
						{
							fanBonusData = GetFanBonusData(num, 2);
							SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
						}
					}
					break;
				case 13:
					if (instance2.gm_.score_[0] == instance2.gm_.score_[1])
					{
						int warPotential_ = teamData.warPotential_;
						int warPotential_2 = teamData2.warPotential_;
						int num2 = warPotential_2 - warPotential_;
						if (num2 > 100)
						{
							fanBonusData = GetFanBonusData(num, 0);
							SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
						}
						else if (num2 < -100)
						{
							fanBonusData = GetFanBonusData(num, 1);
							SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
						}
					}
					break;
				case 1:
					if (instance2.gm_.score_[0] < instance2.gm_.score_[1])
					{
						int warPotential_3 = teamData.warPotential_;
						int warPotential_4 = teamData2.warPotential_;
						int num11 = warPotential_4 - warPotential_3;
						if (num11 >= -100)
						{
							fanBonusData = GetFanBonusData(num, 0);
							SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
						}
						else if (num11 >= -200)
						{
							fanBonusData = GetFanBonusData(num, 1);
							SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
						}
						else
						{
							fanBonusData = GetFanBonusData(num, 2);
							SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
						}
					}
					break;
				case 2:
				{
					if (instance2.gm_.score_[0] <= instance2.gm_.score_[1])
					{
						break;
					}
					Vector fanBonusDataList4 = GetFanBonusDataList(num);
					for (int n = 0; n < fanBonusDataList4.Size(); n++)
					{
						FanBonusData fanBonusData7 = (FanBonusData)fanBonusDataList4.ElementAt(n);
						if (instance2.game_winConsecutive_ == fanBonusData7.termModeValue_)
						{
							SetBPFData(fanBonusData7, fanBonusData7.message_, fanBonusData7.param);
							break;
						}
					}
					break;
				}
				case 15:
				case 16:
				{
					if (instance2.gm_.score_[0] <= instance2.gm_.score_[1])
					{
						break;
					}
					int matchInfoJoin = instance2.GetMatchInfoJoin();
					if (matchInfoJoin == 1)
					{
						QuestMatchData questMatchData = instance.qmData_[instance2.selectQmId_];
						if (instance2.battleQmIndex_ == questMatchData.matchEnemy_.Length - 1 && ((questMatchData.matchForm_ == 2 && num == 15) || (questMatchData.matchForm_ == 1 && num == 16)) && questMatchData.rank_ >= 0 && questMatchData.rank_ <= 6)
						{
							fanBonusData = GetFanBonusData(num, questMatchData.rank_);
							SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
						}
					}
					break;
				}
				case 3:
					if (instance2.GetMatchInfoJoin() == 0 && LeagueData.IsVictoryLeague())
					{
						LeagueData leagueData = instance.leagueData_[instance2.selectLeagueId_];
						fanBonusData = GetFanBonusData(num, leagueData.id_);
						SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
					}
					break;
				case 14:
				{
					if (!LeagueData.IsLastMatch())
					{
						break;
					}
					LeagueData league_data = instance.leagueData_[instance2.selectLeagueId_];
					Vector out_ary = new Vector();
					Vector vector = new Vector();
					TeamData.SetScoreRanking(league_data, out_ary, vector);
					if (vector.Size() > 0)
					{
						int[] array = (int[])vector.ElementAt(0);
						if (array[1] == 0)
						{
							PlayerData playerData3 = instance.pData_[array[2]];
							fanBonusData = GetFanBonusData(num, 0);
							SetBPFData(fanBonusData, StringUtil.Replace(fanBonusData.message_, playerData3.name_), fanBonusData.param, playerData3.id_);
						}
					}
					break;
				}
				case 4:
					if (instance2.gm_.score_[0] >= 3 && instance2.gm_.score_[1] >= 3)
					{
						fanBonusData = GetFanBonusData(num, 0);
						SetBPFData(fanBonusData, fanBonusData.message_, fanBonusData.param);
					}
					break;
				case 5:
				{
					Vector vector2 = new Vector();
					bool flag = false;
					for (int num7 = 0; num7 < instance2.gm_.scorerListUserTeam.Size(); num7++)
					{
						int num8 = ((JInteger)instance2.gm_.scorerListUserTeam.ElementAt(num7)).IntValue();
						flag = false;
						for (int num9 = 0; num9 < vector2.Size(); num9++)
						{
							int[] array2 = (int[])vector2.ElementAt(num9);
							if (array2[0] == num8)
							{
								flag = true;
								array2[1]++;
								break;
							}
						}
						if (!flag)
						{
							vector2.AddElement(new int[2] { num8, 1 });
						}
					}
					for (int num10 = 0; num10 < vector2.Size(); num10++)
					{
						int[] array3 = (int[])vector2.ElementAt(num10);
						if (array3[1] >= 3)
						{
							PlayerData playerData5 = instance.pData_[array3[0]];
							fanBonusData = GetFanBonusData(num, 0);
							SetBPFData(fanBonusData, StringUtil.Replace(fanBonusData.message_, playerData5.name_), fanBonusData.param, playerData5.id_);
						}
					}
					break;
				}
				case 6:
				{
					Vector fanBonusDataList6 = GetFanBonusDataList(num);
					for (int num5 = 0; num5 < fanBonusDataList6.Size(); num5++)
					{
						FanBonusData fanBonusData9 = (FanBonusData)fanBonusDataList6.ElementAt(num5);
						for (int num6 = 0; num6 < 11; num6++)
						{
							PlayerData playerData4 = (PlayerData)instance2.memberPDataist_.ElementAt(num6);
							if (playerData4.matchNum_ == fanBonusData9.termModeValue_)
							{
								string topMessage4 = StringUtil.Replace(fanBonusData9.message_, playerData4.name_ + "\t" + playerData4.matchNum_);
								SetBPFData(fanBonusData9, topMessage4, fanBonusData9.param, playerData4.id_);
							}
						}
					}
					break;
				}
				case 7:
				{
					Vector fanBonusDataList5 = GetFanBonusDataList(num);
					for (int num3 = 0; num3 < fanBonusDataList5.Size(); num3++)
					{
						FanBonusData fanBonusData8 = (FanBonusData)fanBonusDataList5.ElementAt(num3);
						for (int num4 = 0; num4 < 11; num4++)
						{
							PlayerData playerData2 = (PlayerData)instance2.memberPDataist_.ElementAt(num4);
							if (playerData2.game_goalConsecutive_ == fanBonusData8.termModeValue_)
							{
								string topMessage3 = StringUtil.Replace(fanBonusData8.message_, playerData2.name_ + "\t" + playerData2.game_goalConsecutive_);
								SetBPFData(fanBonusData8, topMessage3, fanBonusData8.param, playerData2.id_);
							}
						}
					}
					break;
				}
				case 8:
				{
					Vector fanBonusDataList3 = GetFanBonusDataList(num);
					for (int l = 0; l < fanBonusDataList3.Size(); l++)
					{
						FanBonusData fanBonusData6 = (FanBonusData)fanBonusDataList3.ElementAt(l);
						for (int m = 0; m < instance2.memberPDataist_.Size(); m++)
						{
							PlayerData playerData = (PlayerData)instance2.memberPDataist_.ElementAt(m);
							if (playerData.game_goalTotal_ >= fanBonusData6.termModeValue_ && playerData.pri_game_goalTotal_ < fanBonusData6.termModeValue_)
							{
								string topMessage2 = StringUtil.Replace(fanBonusData6.message_, playerData.name_ + "\t" + playerData.game_goalTotal_);
								SetBPFData(fanBonusData6, topMessage2, fanBonusData6.param, playerData.id_);
							}
						}
					}
					break;
				}
				case 11:
				{
					if (instance2.gm_.score_[0] <= instance2.gm_.score_[1])
					{
						break;
					}
					SupervisorData supervisorData = instance.svisorData_[instance2.selectSupervisorId_];
					Vector fanBonusDataList2 = GetFanBonusDataList(num);
					for (int k = 0; k < fanBonusDataList2.Size(); k++)
					{
						FanBonusData fanBonusData5 = (FanBonusData)fanBonusDataList2.ElementAt(k);
						if (supervisorData.winTotal_ == fanBonusData5.termModeValue_)
						{
							string topMessage = StringUtil.Replace(fanBonusData5.message_, supervisorData.name_ + "\t" + supervisorData.winTotal_);
							SetBPFData(fanBonusData5, topMessage, fanBonusData5.param, supervisorData.id_);
							break;
						}
					}
					break;
				}
				case 12:
				{
					Vector fanBonusDataList = GetFanBonusDataList(num);
					for (int j = 0; j < fanBonusDataList.Size(); j++)
					{
						FanBonusData fanBonusData4 = (FanBonusData)fanBonusDataList.ElementAt(j);
						if (fanBonusData4.state_ != 1 && instance2.customer_total_ >= fanBonusData4.termModeValue_ * 10000)
						{
							SetBPFData(fanBonusData4, fanBonusData4.message_, fanBonusData4.param);
							break;
						}
					}
					break;
				}
				}
			}
		}
	}
}
