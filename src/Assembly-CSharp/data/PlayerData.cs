using form;
using game;
using java.io;
using java.lang;
using kairo.unity.util;
using main;

namespace data
{
	public class PlayerData : BaseData
	{
		public const int FLG_INIT = 1;

		public const int FLG_TRAINING_KEEPER = 2;

		public const int FLG_CPUDATA1 = 4;

		public const int FLG_CPUDATA2 = 8;

		public const int PARAM_MAX = 1000;

		public const int EVA_SPEED = 0;

		public const int EVA_PASS = 1;

		public const int EVA_SHOOT = 2;

		public const int EVA_SHOOTV0Y = 3;

		public const int EVA_CHARGE_BODY = 4;

		public const int EVA_CHARGE_SLIDING = 5;

		public const int EVA_PHYSICAL = 6;

		public const int EVA_TRAP = 7;

		public const int EVA_CATCH = 8;

		public const int EVA_KEEPERJAMP = 9;

		public const int EVA_JAMPDIS = 10;

		public const int EVA_JAMPDISTOP = 11;

		public const int ACTSCORE_SHOOT = 0;

		public const int ACTSCORE_PASS = 1;

		public const int ACTSCORE_CLEAR = 2;

		public const int ACTSCORE_CENTERING = 3;

		public const int ACTSCORE_DRIBBLE = 4;

		public const int ACTSCORE_SHOOT_SCCESS = 5;

		public const int ACTSCORE_PASS_SCCESS = 6;

		public const int ACTSCORE_RECEIVE_BALL = 7;

		public const int ACTSCORE_RE_TACLE = 8;

		public const int ACTSCORE_RE_TACLE_SLD = 9;

		public const int ACTSCORE_BALL_RELEASE = 10;

		public const int ACTSCORE_TACLE = 11;

		public const int ACTSCORE_TACLE_SLD = 12;

		public const int ACTSCORE_BALL_GET = 13;

		public const int ACTSCORE_HEAD = 14;

		public const int ACTSCORE_GK_SAVE = 15;

		public const int ACTSCORE_GK_SCORE0 = 16;

		public const int ACTSCORE_END = 17;

		public const int AURAID_SHOOT = 0;

		public const int AURAID_PATH = 1;

		public const int AURAID_SPEED = 2;

		public const int AURAID_PHY_OFF = 3;

		public const int AURAID_PHY_OFF_HITRATE = 4;

		public const int AURAID_PHY_DEF = 5;

		public const int AURAID_PHY_DEF_HITRATE = 6;

		public const int AURAID_GK = 7;

		public const int AURAID_RECEIVEPOS = 8;

		public const int AURAID_DOWNTIME = 9;

		public const int AURA_MAX = 10000;

		public const int AURA_ADD_MIN = 50;

		public const int AURA_ADD_MAX = 200;

		public const int AURA_PROGRESSTIME = 50;

		public const int AURA_ADD_DOWN = 500;

		public const int AURA_ADD_DOWN_MIN = 300;

		public const int AURA_ADD_DOWN_MAX = 600;

		public const int AURA_TIME = 160;

		public const int TID_KICK = 0;

		public const int TID_SPD = 1;

		public const int TID_TEQ = 2;

		public const int TID_BODY = 3;

		public const int TID_KEEPER = 4;

		public const int TID_END = 5;

		public const int TRAINING_TUP_MIN = 4000;

		public const int POSITIONID_GK = 0;

		public const int POSITIONID_DF = 1;

		public const int POSITIONID_MF = 2;

		public const int POSITIONID_FW = 3;

		public const int LV_MIN = 0;

		public const int LV_MAX = 30;

		public const int BONUS_R_ROOT = 0;

		public const int BONUS_R_POINT = 1;

		public const int CPU_OFF = 22;

		public const int EMPLOY_MAX = 30;

		public const int TAREA_EXP_BONUS = 4;

		public const int PARAMID_SPEED = 0;

		public const int PARAMID_PASS = 1;

		public const int PARAMID_THROUHPASS = 2;

		public const int PARAMID_LONGPASS = 3;

		public const int PARAMID_SHOOT = 4;

		public const int PARAMID_VOY = 5;

		public const int PARAMID_HSHOOTURR = 6;

		public const int PARAMID_CHARGE_BODY = 7;

		public const int PARAMID_CHARGE_SLD = 8;

		public const int PARAMID_PHYSICAL = 9;

		public const int PARAMID_CATCH = 10;

		public const int PARAMID_KEEPERJAMP = 11;

		public const int PARAMID_SLIDINGRATE = 12;

		public const int PARAMID_JAMPDIS = 13;

		public const int PARAMID_JAMPDISSTOP = 14;

		public const int PARAMID_SHOOTTRAP = 15;

		public const float W_BALLHAVETIME = 0.3f;

		public const float W_PHYSICALHP = 0.7f;

		public const float W_CATCH = 0.4f;

		public const float W_V_LEN = 0.5f;

		public const float W_FRONT = 0.1f;

		private const int GPPBAR_DIV = 500;

		private const int GPPBAR_RATE = 70;

		private const int LVUP_RATE = 80;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public string name2_;

		public int category_;

		public int faceImg_;

		public int initLv_;

		public int[] paramPhysical_ = new int[5];

		public int[] paramPhysicalMax_ = new int[5];

		public int[] salary_ = new int[2];

		public int popularP_;

		public int[] trainingPoint_;

		public int role_;

		public int auraPoint_;

		public int matchNum_;

		public int[] actscoreNum_ = new int[17];

		public int actscoreSum_;

		public int actscoreScore_;

		public int game_goalConsecutive_;

		public int game_goalTotal_;

		public int game_goalInLeague_;

		public int exp_;

		public int expStock_;

		public int lv_;

		public bool new_;

		public int onceMember_;

		public static float SPEEDMIN_MIN = 8f;

		public static float SPEEDMAX_MIN = 9f;

		public static float SPEEDMIN_MAX = 6f;

		public static float SPEEDMAX_MAX = 12f;

		public int param_speed;

		public static float PASSSPEED_MIN = 18f;

		public static float PASSSPEED_MAX = 23f;

		public int param_pass;

		public static float PASSSPEED_THROUH_MIN = 12f;

		public static float PASSSPEED_THROUH_MAX = 17f;

		public int param_throuhPass;

		public static float LONGPASSSPEED_MIN = 19f;

		public static float LONGPASSSPEED_MAX = 24f;

		public int param_longPass;

		public static float SHOOTMIN_MIN = 23f;

		public static float SHOOTMIN_MAX = 23f;

		public static float SHOOTMAX_MIN = 48f;

		public static float SHOOTMAX_MAX = 48f;

		public int param_shoot;

		public static float V0YMIN_MIN = 3f;

		public static float V0YMIN_MAX = 1f;

		public static float V0YMAX_MIN = 1.5f;

		public static float V0YMAX_MAX = 0.5f;

		public int param_v0y;

		public static int HSHOOT_URR_MIN = -10;

		public static int HSHOOT_URR_MAX = -30;

		public int param_hShootURR;

		public static int CHARGE_RATE_BODY = 200;

		public static int CHARGEDAMAGE_BODY_MIN = 4000;

		public static int CHARGEDAMAGE_BODY_MAX = 12000;

		public int param_charge_body;

		public static int CHARGEDAMAGE_SLIDING_MIN = 9000;

		public static int CHARGEDAMAGE_SLIDING_MAX = 40000;

		public int param_charge_sliding;

		public static int PHYSICALHP_MIN = 10000;

		public static int PHYSICALHP_MAX = 50000;

		public int param_physical;

		public static float TRAPBALL_MIN = 6f;

		public static float TRAPBALL_MAX = 18f;

		public int param_trap;

		public int param_catch;

		public static float KEEPERJAMP_MIN = 6f;

		public static float KEEPERJAMP_MAX = 10f;

		public int param_keeperJamp;

		public static float SLIDING_SPEED = 10f;

		public static int SLIDING_RATE_MIN = 980;

		public static int SLIDING_RATE_MAX = 850;

		public int param_slidingRate;

		public static float JAMPDIS_MIN = 10f;

		public static float JAMPDIS_MAX = 20f;

		public int param_jampdis;

		public static float JAMPDISTOP_MIN = 30f;

		public static float JAMPDISTOP_MAX = 40f;

		public int param_jampdistop;

		public int param_shootTrap;

		public static readonly string[] POSITION_NAME = new string[4] { "GK", "DF", "MF", "FW" };

		public static readonly int[][] DEFAULT_DATA = new int[13][]
		{
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 },
			new int[2] { 500, 500 }
		};

		public static readonly string[] CATEGORY_STR = new string[24]
		{
			"平凡", "代表ｸﾗｽ", "ﾜｰﾙﾄﾞｸﾗｽ", "ｽｰﾊﾟｰｽﾀｰ", "ｽﾄﾗｲｶｰ", "ﾎﾞﾃﾞｨﾋﾞﾙ", "なでしこ", "お子様", "ﾃｸﾆｼｬﾝ", "ﾌｧﾝﾀｼﾞｽﾀ",
			"自由人", "ｽﾋﾟｰﾄﾞｽﾀｰ", "守備職人", "平凡GK", "代表GK", "ｽﾊﾟｲﾀﾞｰ", "世界GK", "ｽｰﾊﾟｰGK", "ｶﾘｽﾏ", "ﾘｰﾀﾞｰ",
			"ご年配", "勉強家", "長距離砲", "熱血漢"
		};

		public static readonly int[] ACTSCORE_SCORE = new int[17]
		{
			200, 60, 150, 250, 2, 500, 150, 150, 10, 30,
			-150, 30, 100, 200, 100, 200, 300
		};

		public static readonly float[] AURA_RATE = new float[10] { 1.3f, 1.3f, 1.3f, 1.3f, 2f, 0.7692308f, 0.5f, 1.3f, 1.3f, 0.77f };

		public static readonly string[] PARAM_NAME = new string[5] { "ｷｯｸ", "ｽﾋﾟｰﾄﾞ", "ﾃｸﾆｯｸ", "ﾎﾞﾃﾞｨ", "ｷｰﾊﾟｰ" };

		public static readonly int[][] TRAINING_TUP_GRAPH = new int[6][]
		{
			new int[5] { 1, 1, 1, 0, 0 },
			new int[5] { 1, 0, 1, 0, 0 },
			new int[5] { 0, 1, 1, 1, 0 },
			new int[5] { 1, 1, 0, 1, 0 },
			new int[5] { 1, 0, 1, 0, 0 },
			new int[5] { 0, 0, 0, 0, 1 }
		};

		public static readonly float[][] POSIEVA_RATE = new float[4][]
		{
			new float[5] { 0f, 0f, 0f, 0f, 1f },
			new float[5] { 0f, 0f, 0f, 1f, 0f },
			new float[5] { 0f, 0f, 1f, 0f, 0f },
			new float[5] { 1f, 0f, 0f, 0f, 0f }
		};

		public int temp_lv;

		public int temp_exp;

		public int temp_anime;

		public bool temp_lvUp;

		public int[] bonus_research = new int[2];

		public int auraCnt_;

		public int pri_game_goalTotal_;

		public static readonly float[][] PARAM_RATE = new float[16][]
		{
			new float[5] { 0f, 0.9f, 0.1f, 0f, 0f },
			new float[5] { 0.2f, 0f, 0.8f, 0f, 0f },
			new float[5] { 0.2f, 0f, 0.8f, 0f, 0f },
			new float[5] { 0.2f, 0f, 0.8f, 0f, 0f },
			new float[5] { 0.9f, 0f, 0.1f, 0f, 0f },
			new float[5] { 0.7f, 0f, 0.3f, 0f, 0f },
			new float[5] { 0.7f, 0f, 0.3f, 0f, 0f },
			new float[5] { 0f, 0f, 0.2f, 0.8f, 0f },
			new float[5] { 0.2f, 0f, 0.2f, 0.6f, 0f },
			new float[5] { 0f, 0f, 0.1f, 0.9f, 0f },
			new float[5] { 0f, 0f, 0f, 0f, 1f },
			new float[5] { 0f, 0f, 0f, 0f, 1f },
			new float[5] { 0.1f, 0.1f, 0.6f, 0.2f, 0f },
			new float[5] { 0.1f, 0f, 0.3f, 0.3f, 0f },
			new float[5] { 0.1f, 0f, 0.3f, 0.3f, 0f },
			new float[5] { 0f, 0f, 0.5f, 0.5f, 0f }
		};

		public static readonly int[] PARAM_EVA = new int[7] { 50, 100, 150, 250, 400, 600, 1000 };

		private static int[] eva_position = new int[4];

		public PlayerData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
		}

		public PlayerData(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			trainingPoint_ = new int[5];
			if (paramPhysicalMax_ != null)
			{
				role_ = GetMostEvaPosition(paramPhysicalMax_);
			}
			auraPoint_ = 5000;
			matchNum_ = 0;
			actscoreNum_ = new int[17];
			actscoreSum_ = 0;
			actscoreScore_ = 0;
			game_goalConsecutive_ = 0;
			game_goalTotal_ = 0;
			game_goalInLeague_ = 0;
			exp_ = 0;
			expStock_ = 0;
			lv_ = 0;
			new_ = false;
			onceMember_ = 0;
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			num++;
			name_ = array[num++];
			name2_ = array[num++];
			category_ = JInteger.ParseInt(array[num++]);
			faceImg_ = JInteger.ParseInt(array[num++]);
			initLv_ = JInteger.ParseInt(array[num++]);
			string value = array[num++];
			string[] array2 = StringUtil.Split(value, "&");
			paramPhysicalMax_ = new int[array2.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				paramPhysicalMax_[i] = JInteger.ParseInt(array2[i]);
			}
			salary_[0] = JInteger.ParseInt(array[num++]);
			salary_[1] = JInteger.ParseInt(array[num++]);
			popularP_ = JInteger.ParseInt(array[num++]);
			flag_ = JInteger.ParseInt(array[num++]);
			SetLv(initLv_);
			exp_ = 0;
			auraPoint_ = 5000;
			role_ = GetMostEvaPosition(paramPhysicalMax_);
		}

		public void NewGame()
		{
		}

		public void Serialize(OutputStream os)
		{
			for (int i = 0; i < paramPhysical_.Length; i++)
			{
				StreamUtil.WriteShort(os, (short)paramPhysical_[i]);
			}
			for (int j = 0; j < trainingPoint_.Length; j++)
			{
				StreamUtil.WriteInt(os, trainingPoint_[j]);
			}
			StreamUtil.WriteShort(os, (short)role_);
			StreamUtil.WriteShort(os, (short)auraPoint_);
			StreamUtil.WriteInt(os, matchNum_);
			StreamUtil.WriteInt(os, game_goalConsecutive_);
			StreamUtil.WriteInt(os, game_goalTotal_);
			StreamUtil.WriteShort(os, (short)game_goalInLeague_);
			StreamUtil.WriteInt(os, exp_);
			StreamUtil.WriteInt(os, expStock_);
			StreamUtil.WriteInt(os, lv_);
			StreamUtil.WriteBoolean(os, new_);
			StreamUtil.WriteShort(os, (short)onceMember_);
			StreamUtil.WriteInt(os, popularP_);
		}

		public void Deserialize(InputStream stream)
		{
			for (int i = 0; i < paramPhysical_.Length; i++)
			{
				paramPhysical_[i] = StreamUtil.ReadShort(stream);
			}
			for (int j = 0; j < trainingPoint_.Length; j++)
			{
				trainingPoint_[j] = StreamUtil.ReadInt(stream);
			}
			role_ = StreamUtil.ReadShort(stream);
			auraPoint_ = StreamUtil.ReadShort(stream);
			matchNum_ = StreamUtil.ReadInt(stream);
			game_goalConsecutive_ = StreamUtil.ReadInt(stream);
			game_goalTotal_ = StreamUtil.ReadInt(stream);
			game_goalInLeague_ = StreamUtil.ReadShort(stream);
			exp_ = StreamUtil.ReadInt(stream);
			expStock_ = StreamUtil.ReadInt(stream);
			lv_ = StreamUtil.ReadInt(stream);
			new_ = StreamUtil.ReadBoolean(stream);
			onceMember_ = StreamUtil.ReadShort(stream);
			popularP_ = StreamUtil.ReadInt(stream);
		}

		public void SetGameParamData(int[] paramPhysical)
		{
			float num = 0f;
			int num2 = 0;
			for (int i = 0; i < PARAM_RATE.Length; i++)
			{
				num = 0f;
				for (int j = 0; j < PARAM_RATE[i].Length; j++)
				{
					num += PARAM_RATE[i][j];
				}
				num2 = (int)((PARAM_RATE[i][0] * (float)paramPhysical[0] + PARAM_RATE[i][1] * (float)paramPhysical[1] + PARAM_RATE[i][2] * (float)paramPhysical[2] + PARAM_RATE[i][3] * (float)paramPhysical[3] + PARAM_RATE[i][4] * (float)paramPhysical[4]) / num);
				switch (i)
				{
				case 0:
					param_speed = num2;
					break;
				case 1:
					param_pass = num2;
					break;
				case 2:
					param_throuhPass = num2;
					break;
				case 3:
					param_longPass = num2;
					break;
				case 4:
					param_shoot = num2;
					break;
				case 5:
					param_v0y = num2;
					break;
				case 6:
					param_hShootURR = num2;
					break;
				case 7:
					param_charge_body = num2;
					break;
				case 8:
					param_charge_sliding = num2;
					break;
				case 9:
					param_physical = num2;
					break;
				case 10:
					param_catch = num2;
					break;
				case 11:
					param_keeperJamp = num2;
					break;
				case 12:
					param_slidingRate = num2;
					break;
				case 13:
					param_jampdis = num2;
					break;
				case 14:
					param_jampdistop = num2;
					break;
				case 15:
					param_shootTrap = num2;
					break;
				}
			}
		}

		public void SetGameData(Player player)
		{
			player.speed_min = GameUtil.RateConvert(param_speed, 0f, 1000f, SPEEDMIN_MIN, SPEEDMAX_MIN, true);
			player.speed_max = GameUtil.RateConvert(param_speed, 0f, 1000f, SPEEDMIN_MAX, SPEEDMAX_MAX, true);
			player.pass_data = GameUtil.RateConvert(param_pass, 0f, 1000f, PASSSPEED_MIN, PASSSPEED_MAX, true);
			player.throuhPass_data = GameUtil.RateConvert(param_throuhPass, 0f, 1000f, PASSSPEED_THROUH_MIN, PASSSPEED_THROUH_MAX, true);
			player.longPass_data = GameUtil.RateConvert(param_longPass, 0f, 1000f, LONGPASSSPEED_MIN, LONGPASSSPEED_MAX, true);
			player.shoot_min = GameUtil.RateConvert(param_shoot, 0f, 1000f, SHOOTMIN_MIN, SHOOTMAX_MIN, true);
			player.shoot_max = GameUtil.RateConvert(param_shoot, 0f, 1000f, SHOOTMIN_MAX, SHOOTMAX_MAX, true);
			player.shootV0y_min = GameUtil.RateConvert(param_v0y, 0f, 1000f, V0YMIN_MIN, V0YMAX_MIN, true);
			player.shootV0y_max = GameUtil.RateConvert(param_v0y, 0f, 1000f, V0YMIN_MAX, V0YMAX_MAX, true);
			player.headShootUpRadRate = GameUtil.RateConvert(param_hShootURR, 500, 1000, HSHOOT_URR_MIN, HSHOOT_URR_MAX, true);
			player.chargeDamage_body = GameUtil.RateConvert(param_charge_body, 0, 1000, CHARGEDAMAGE_BODY_MIN, CHARGEDAMAGE_BODY_MAX, true);
			player.chargeDamage_sliding = GameUtil.RateConvert(param_charge_sliding, 0, 1000, CHARGEDAMAGE_SLIDING_MIN, CHARGEDAMAGE_SLIDING_MAX, true);
			player.physicalHp_data = GameUtil.RateConvert(param_physical, 0, 1000, PHYSICALHP_MIN, PHYSICALHP_MAX, true);
			player.trapball_data = GameUtil.RateConvert(param_trap, 0f, 1000f, TRAPBALL_MIN, TRAPBALL_MAX, true);
			player.catch_data = GameUtil.RateConvert(param_catch, 0, 1000, 0, 100, true);
			player.keeperJmap_data = GameUtil.RateConvert(param_keeperJamp, 0f, 1000f, KEEPERJAMP_MIN, KEEPERJAMP_MAX, true);
			player.slidingRate_data = GameUtil.RateConvert(param_slidingRate, 0, 1000, SLIDING_RATE_MIN, SLIDING_RATE_MAX, true);
			player.jamp_data = GameUtil.RateConvert(param_jampdis, 0f, 1000f, JAMPDIS_MIN, JAMPDIS_MAX, true);
			player.jamptop_data = GameUtil.RateConvert(param_jampdistop, 0f, 1000f, JAMPDISTOP_MIN, JAMPDISTOP_MAX, true);
			player.shootTrap_data = GameUtil.RateConvert(param_shootTrap, 0, 1000, 0, 1000, true);
		}

		public static float GetSpeed(Player player, int eva)
		{
			return GameUtil.RateConvert(eva, 0f, 100f, player.speed_min, player.speed_max, true);
		}

		public static float GetPassSpeed(Player player)
		{
			return player.pass_data;
		}

		public static float GetThroughPassSpeed(Player player)
		{
			return player.throuhPass_data;
		}

		public static float GetLongPassSpeed(Player player)
		{
			return player.longPass_data;
		}

		public static float GetClearSpeed(Player player)
		{
			return player.pass_data * 7f / 10f;
		}

		public static float GetShootSpeed(Player player, int eva)
		{
			return GameUtil.RateConvert(eva, 0f, 100f, player.shoot_min, player.shoot_max, true);
		}

		public static float GetShootV0y(Player player, int eva)
		{
			return GameUtil.RateConvert(eva, 0f, 100f, player.shootV0y_min, player.shootV0y_max, true);
		}

		public static int GetHeadShootUpRadRate(Player player)
		{
			return player.headShootUpRadRate;
		}

		public static int GetChargeDamage(Player player, bool flg_sliding)
		{
			if (flg_sliding)
			{
				return player.chargeDamage_sliding;
			}
			return player.chargeDamage_body;
		}

		public static int GetPhysicalHpMax(Player player)
		{
			return player.physicalHp_data;
		}

		public static int GetBallShedRate(Player dribbler_player, Player charge_player)
		{
			return GameUtil.RateConvert(apdat_.pData_[dribbler_player.pData_id_].param_physical - apdat_.pData_[charge_player.pData_id_].param_physical, 0, 300, 5, 40, true);
		}

		public static int GetPlayerBodyPoseEva(Player player, int max_ballHaveTime)
		{
			int num = GameUtil.RateConvert(player.ballHaveTime_, 0, max_ballHaveTime, 0, 100, true);
			int num2 = GameUtil.RateConvert(player.physical_hp_, player.physicalHp_data, 0, 0, 100, true);
			return (int)((0.3f * (float)num + 0.7f * (float)num2) / 1f);
		}

		public static float GetTrapBallV(Player player)
		{
			return player.trapball_data;
		}

		public static int GetCatchProc(Player player, float v_len, bool front)
		{
			int catch_data = player.catch_data;
			int num = (int)GameUtil.RateConvert(v_len, 45f, 10f, 0f, 100f, true);
			int num2 = 0;
			if (front)
			{
				num2 = 100;
			}
			int num3 = (int)((0.4f * (float)catch_data + 0.5f * (float)num + 0.1f * (float)num2) / 1f);
			num3 = (int)AuraUpdate(player, num3, 7);
			num3 = num3 + GameUtil.Random(40) - 20;
			if (num3 < 40)
			{
				return 0;
			}
			if (num3 < 70)
			{
				return 1;
			}
			return 2;
		}

		public static float GetKeeperJampMax(Player player)
		{
			return player.keeperJmap_data;
		}

		public static int GetSlidingRate(Player player)
		{
			return player.slidingRate_data;
		}

		public static float GetHeadJampDis(Player player)
		{
			return player.jamp_data;
		}

		public static float GetHeadJampDisTop(Player player)
		{
			return player.jamptop_data;
		}

		public static int GetParamEva(int param)
		{
			for (int i = 0; i < PARAM_EVA.Length; i++)
			{
				if (param < PARAM_EVA[i])
				{
					return i;
				}
			}
			return PARAM_EVA.Length - 1;
		}

		public static int GetNextLvUpExp(int nowLv)
		{
			if (nowLv < 1)
			{
				return 0;
			}
			return 18 * nowLv * nowLv + 0 * nowLv + 100;
		}

		public void SetActScore(int actscore_id)
		{
			actscoreNum_[actscore_id]++;
			actscoreSum_ += ACTSCORE_SCORE[actscore_id];
		}

		public static float AuraUpdate(Player player, float value, int auraActId)
		{
			if (!BitUtil.Check(player.flag_, 262144))
			{
				return value;
			}
			return value * AURA_RATE[auraActId];
		}

		public static int GetPhyParamBarW(int param, int BAR_W_MAX)
		{
			int num = 0;
			if (param < 500)
			{
				return GameUtil.RateConvert(param, 0, 500, BAR_W_MAX * 2 / 100, BAR_W_MAX * 70 / 100, true);
			}
			return GameUtil.RateConvert(param, 500, 1000, BAR_W_MAX * 70 / 100, BAR_W_MAX * 100 / 100, true);
		}

		public static int GetPositionEva(int positionId, int[] phyParam)
		{
			return (int)((POSIEVA_RATE[positionId][0] * (float)phyParam[0] + POSIEVA_RATE[positionId][1] * (float)phyParam[1] + POSIEVA_RATE[positionId][2] * (float)phyParam[2] + POSIEVA_RATE[positionId][3] * (float)phyParam[3] + POSIEVA_RATE[positionId][4] * (float)phyParam[4]) / (POSIEVA_RATE[positionId][0] + POSIEVA_RATE[positionId][1] + POSIEVA_RATE[positionId][2] + POSIEVA_RATE[positionId][3] + POSIEVA_RATE[positionId][4]));
		}

		public static int GetPositionRank(int positionId, PlayerData p_data)
		{
			if (positionId == 0 && p_data.paramPhysicalMax_[4] < 100)
			{
				return 9;
			}
			int positionEva = GetPositionEva(positionId, p_data.paramPhysical_);
			return GetParamEva(positionEva);
		}

		public static int GetMostEvaPosition(int[] phyParam)
		{
			for (int i = 0; i < 4; i++)
			{
				eva_position[i] = GetPositionEva(i, phyParam);
			}
			int num = 0;
			int result = 0;
			for (int j = 0; j < 4; j++)
			{
				if (num < eva_position[j])
				{
					num = eva_position[j];
					result = j;
				}
			}
			return result;
		}

		public void UpdatePhysicalParam()
		{
			int num = 0;
			for (int i = 0; i < paramPhysical_.Length; i++)
			{
				paramPhysical_[i] = GameUtil.RateConvert(lv_, 0, 30, 0, paramPhysicalMax_[i] * 80 / 100, true);
				num = trainingPoint_[i];
				int num2 = paramPhysical_[i] + num - paramPhysicalMax_[i];
				if (num2 > 0)
				{
					num2 /= 20;
					paramPhysical_[i] = paramPhysicalMax_[i] + num2;
				}
				else
				{
					paramPhysical_[i] += num;
				}
				paramPhysical_[i] = AppData.ClampMax(paramPhysical_[i], 1000);
			}
		}

		public void SetTrainingAddParam(int[] trainingAddEva, int[] var_out)
		{
			int num = 0;
			for (int i = 0; i < paramPhysicalMax_.Length; i++)
			{
				var_out[i] = GameUtil.RateConvert(lv_, 0, 30, 0, paramPhysicalMax_[i] * 80 / 100, true);
				num = trainingPoint_[i] + trainingAddEva[i];
				int num2 = var_out[i] + num - paramPhysicalMax_[i];
				if (num2 > 0)
				{
					num2 /= 20;
					var_out[i] = paramPhysicalMax_[i] + num2;
				}
				else
				{
					var_out[i] += num;
				}
				var_out[i] = AppData.ClampMax(var_out[i], 1000);
			}
		}

		public void SetLv(int lv)
		{
			lv_ = lv;
			UpdatePhysicalParam();
		}

		public void LvUp()
		{
			if (lv_ != 30)
			{
				lv_ = AppData.ClampMax(lv_ + 1, 30);
				UpdatePhysicalParam();
			}
		}

		public void AddExp(int add_exp, bool addLvUpPopUp)
		{
			exp_ += add_exp;
			int num = 0;
			do
			{
				int nextLvUpExp = GetNextLvUpExp(lv_);
				if (exp_ < nextLvUpExp)
				{
					break;
				}
				exp_ -= nextLvUpExp;
				LvUp();
				if (addLvUpPopUp)
				{
					string empty = string.Empty;
					empty += MyFormBase.LT(name_);
					if (Language.English())
					{
						empty += " ";
					}
					empty += MyFormBase.LT(name2_);
					empty = empty + "\t" + lv_;
					apdat_.AddPopupMessage(28, empty);
				}
				if (lv_ == 30)
				{
					break;
				}
				num++;
			}
			while (num < 10);
		}

		public void StockExp(int addStockExp)
		{
			expStock_ += addStockExp;
		}

		public int UpdateStockExp()
		{
			int num = expStock_;
			num += udat_.tAreaLv_ * 4;
			AddExp(num, true);
			expStock_ = 0;
			return num;
		}

		public void AddPhysicalParam(int eva, int TID)
		{
			trainingPoint_[TID] += eva;
			UpdatePhysicalParam();
		}

		public void AddPhysicalParam(int[] add_eva)
		{
			for (int i = 0; i < trainingPoint_.Length; i++)
			{
				trainingPoint_[i] += add_eva[i];
			}
			UpdatePhysicalParam();
		}

		public int GetSallay()
		{
			if (UserData.DEBUG_SALLARY_MAX == 1)
			{
				return salary_[1];
			}
			return GameUtil.RateConvert(matchNum_, 0, 150, salary_[0], salary_[1], true);
		}

		public void AddAuraPoint(int add_point, bool isSoccerForm)
		{
			int num = GameUtil.RateConvert(udat_.fanParam_[1], 0, 100, 300, 600, true);
			auraPoint_ = AppData.ClampMax(auraPoint_ + num, 10000);
			if (auraPoint_ == 10000 && isSoccerForm && id_ >= 22 && !apdat_.CheckAlreadyEvent(172))
			{
				apdat_.ExecEvent(172, name_, null);
			}
		}

		public void Update_aura(Player player)
		{
			if (auraCnt_ > 0)
			{
				auraCnt_--;
				if (auraCnt_ == 0)
				{
					flag_ = BitUtil.Off(flag_, 262144);
				}
			}
		}

		public void Start_aura(Player player)
		{
			player.flag_ = BitUtil.On(player.flag_, 262144);
			auraPoint_ = 0;
			auraCnt_ = 160;
		}

		public void AddPopular(int add_popular)
		{
			popularP_ = AppData.ClampMax(popularP_ + add_popular, 100);
		}

		public static void Employ(int pDataId)
		{
			UserData instance = UserData.GetInstance();
			PlayerData playerData = AppData.GetInstance().pData_[pDataId];
			if (instance.memberPDataist_.IndexOf(playerData) == -1)
			{
				instance.memberPDataist_.AddElement(playerData);
				playerData.new_ = true;
				playerData.onceMember_ = 1;
			}
			int num = playerData.id_ - 22;
			if (num >= 0 && num < apdat_.tradeData_.Length)
			{
				TradeData tradeData = apdat_.tradeData_[num];
				tradeData.ChangeState(2);
				tradeData.feelPt_ /= 2;
			}
		}

		public static void Dismissal(int pDataId)
		{
			UserData instance = UserData.GetInstance();
			PlayerData playerData = AppData.GetInstance().pData_[pDataId];
			int num = instance.memberPDataist_.IndexOf(playerData);
			if (num == -1)
			{
				return;
			}
			instance.memberPDataist_.RemoveElementAt(num);
			int num2 = playerData.id_ - 22;
			if (num2 >= 0 && num2 < apdat_.tradeData_.Length)
			{
				TradeData tradeData = apdat_.tradeData_[num2];
				tradeData.ChangeState(1);
				if (num < 11)
				{
					FormationData.UpdateStamemRole();
				}
			}
		}

		public static bool IsExistNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.pData_.Length; i++)
			{
				if (instance.pData_[i].IsNew())
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.pData_.Length; i++)
			{
				instance.pData_[i].new_ = false;
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
