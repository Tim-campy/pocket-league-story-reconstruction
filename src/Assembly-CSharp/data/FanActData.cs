using form;
using game;
using java.io;
using java.lang;
using java.util;
using kairo.unity.util;
using main;

namespace data
{
	public class FanActData : BaseData
	{
		public const int FLAG_INIT = 1;

		public const int STATE_NOT_FOUND = 0;

		public const int STATE_FOUND = 1;

		public const int STATE_ACTIVE = 2;

		public const int RANK_MAX = 6;

		public const int FATYPE_TICKET = 0;

		public const int FATYPE_HANDSHAKE = 1;

		public const int FATYPE_SIGN = 2;

		public const int FATYPE_SOCCER = 3;

		public const int FATYPE_COACH = 4;

		public const int FATYPE_TOURS = 5;

		public const int FATYPE_MAINTENANCE = 6;

		public const int FAID_FAN = 0;

		public const int FAID_ASSIST = 1;

		public const int FAID_NAMEVALUE = 2;

		public const int FAID_RANKP = 3;

		public const int FAID_NUM = 3;

		public const int FARESULT_SUB_OK = 1;

		public const int FARESULT_SUB_NO = 0;

		public const int COSTTYPE_FANSERVICE = 0;

		public const int COSTTYPE_IDEA = 1;

		public const int COSTTYPE_PHYSICAL = 2;

		public const int COSTTYPE_TACTICS = 3;

		public const int COSTTYPE_MONEY = 4;

		public const int FANACT_ACTIVE_TIME = 200;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public int type_;

		public int icon_;

		public int[] effLimit_ = new int[2];

		public int[][] eff_ = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		public int costType_;

		public int[] cost_ = new int[2];

		public int[] benefit_ = new int[2];

		public int difficult_;

		public int[][][] execsScript_ = new int[6][][];

		public string explain_;

		public int rank_;

		public int rankPoint_;

		public int state_;

		public int stateCnt_;

		public bool new_;

		public static int[] fa_value = new int[4];

		public static int[] fa_add = new int[4];

		public static int[] fa_addRUp = new int[4];

		public static int[] fa_id = new int[4];

		public static int fa_rank = 0;

		public static bool fa_rankUp = false;

		public static int fa_result = 0;

		public static int[] fa_resultSub = new int[4];

		public static readonly int[] FARESULT_RATE = new int[3] { 10, 100, 100 };

		public static readonly string[] element_name = new string[4] { "ファン", "応援熱", "知名度", "次のﾗﾝｸまで" };

		public static readonly string[] element_name2 = new string[3] { "ファン", "応援熱", "知名度" };

		public FanActData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public FanActData(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			rank_ = 0;
			rankPoint_ = 0;
			state_ = 0;
			stateCnt_ = 0;
			new_ = false;
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			type_ = JInteger.ParseInt(array[num++]);
			icon_ = JInteger.ParseInt(array[num++]);
			for (int i = 0; i < effLimit_.Length; i++)
			{
				effLimit_[i] = JInteger.ParseInt(array[num++]);
			}
			for (int j = 0; j < eff_.Length; j++)
			{
				for (int k = 0; k < eff_[j].Length; k++)
				{
					eff_[j][k] = JInteger.ParseInt(array[num++]);
				}
			}
			costType_ = JInteger.ParseInt(array[num++]);
			for (int l = 0; l < cost_.Length; l++)
			{
				cost_[l] = JInteger.ParseInt(array[num++]);
			}
			for (int m = 0; m < benefit_.Length; m++)
			{
				benefit_[m] = JInteger.ParseInt(array[num++]);
			}
			difficult_ = JInteger.ParseInt(array[num++]);
			for (int n = 0; n < execsScript_.Length; n++)
			{
				execsScript_[n] = EventData.ReadScript(array[num++]);
			}
			explain_ = array[num++];
			flag_ = JInteger.ParseInt(array[num++]);
		}

		public void NewGame()
		{
			state_ = 0;
			if (BitUtil.Check(flag_, 1))
			{
				ChangeState(1);
				new_ = false;
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)rank_);
			StreamUtil.WriteInt(os, rankPoint_);
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteInt(os, stateCnt_);
			StreamUtil.WriteBoolean(os, new_);
		}

		public void Deserialize(InputStream stream)
		{
			rank_ = StreamUtil.ReadShort(stream);
			rankPoint_ = StreamUtil.ReadInt(stream);
			state_ = StreamUtil.ReadShort(stream);
			stateCnt_ = StreamUtil.ReadInt(stream);
			new_ = StreamUtil.ReadBoolean(stream);
		}

		public void ChangeState(int state)
		{
			if (state_ == 0 && state == 1)
			{
				new_ = true;
			}
			state_ = state;
			stateCnt_ = 0;
		}

		public void Update()
		{
			if (state_ == 2)
			{
				stateCnt_++;
				if (stateCnt_ >= 200)
				{
					SubForm subForm = new SubForm(41);
					subForm.fanActData_ = this;
					apdat_.Push(subForm);
					ChangeState(1);
				}
			}
		}

		public static int GetFanDispParam(int eva, int F_ID)
		{
			if ((F_ID == 0)) { return GameUtil.RateConvert(eva, 0, 10000, 0, 100000, true); } else if ((F_ID == 1)) { return GameUtil.RateConvert(eva, 0, 1000, 0, 100, true); } else if ((F_ID == 2)) { return GameUtil.RateConvert(eva, 0, 10000, 0, 1000, true); } else { return 0; }
		}

		public static int GetDispParamToEva(int dispParam, int F_ID)
		{
			switch (F_ID)
			{
			case 0:
			{
				int num = GameUtil.RateConvert(dispParam, 0, 100000, 0, 10000, true);
				return GameUtil.RateConvert(dispParam, 0, 100000, 0, 10000, true);
			}
			case 1:
				return GameUtil.RateConvert(dispParam, 0, 100, 0, 1000, true);
			case 2:
				return GameUtil.RateConvert(dispParam, 0, 1000, 0, 10000, true);
			default:
				return 0;
			}
		}

		public static int GetFanAddParamNum(int eva, int F_ID)
		{
			if (eva == 0)
			{
				return 0;
			}
			if ((F_ID == 0)) { if (eva <= 60)
				{
					return 1;
				}
if (eva < 200)
				{
					return 2;
				}
return 3; } else if ((F_ID == 1)) { if (eva < 200)
				{
					return 1;
				}
if (eva < 800)
				{
					return 2;
				}
return 3; } else if ((F_ID == 2)) { if (eva <= 60)
				{
					return 1;
				}
if (eva < 120)
				{
					return 2;
				}
return 3; } else { return 0; }
		}

		public static int GetFanEva(int F_ID)
		{
			UserData instance = UserData.GetInstance();
			if ((F_ID == 0)) { return instance.fanParam_[0]; } else if ((F_ID == 1)) { return instance.fanParam_[1]; } else if ((F_ID == 2)) { return instance.fanParam_[2]; } else { return 0; }
		}

		public static int GetAddEvaLimitOver(int F_ID, int eva, int add_eva, int limit)
		{
			int num = eva + add_eva;
			int num2 = 0;
			if ((F_ID == 0)) { if (num > 10000 * limit / 100)
				{
					num2 = num - 10000 * limit / 100;
					add_eva = AppData.ClampMin(add_eva - num2, 0);
				} } else if ((F_ID == 1)) { if (num > 1000 * limit / 100)
				{
					num2 = num - 1000 * limit / 100;
					add_eva = AppData.ClampMin(add_eva - num2, 0);
				} } else if ((F_ID == 2)) { if (num > 10000 * limit / 100)
				{
					num2 = num - 10000 * limit / 100;
					add_eva = AppData.ClampMin(add_eva - num2, 0);
				} }
			return add_eva;
		}

		public static int GetFanEvaToCommonValue(int F_ID)
		{
			if ((F_ID == 0)) { return GameUtil.RateConvert(udat_.fanParam_[0], 0, 10000, 0, 1000, true); } else if ((F_ID == 1)) { return GameUtil.RateConvert(udat_.fanParam_[1], 0, 1000, 0, 1000, true); } else if ((F_ID == 2)) { return GameUtil.RateConvert(udat_.fanParam_[2], 0, 10000, 0, 1000, true); } else { return 0; }
		}

		public static void AddFanParam(int add_eva, int F_ID)
		{
			UserData instance = UserData.GetInstance();
			if ((F_ID == 0)) { instance.fanParam_[0] = AppData.ClampMax(instance.fanParam_[0] + add_eva, 10000); } else if ((F_ID == 1)) { instance.fanParam_[1] = AppData.ClampMax(instance.fanParam_[1] + add_eva, 1000); } else if ((F_ID == 2)) { instance.fanParam_[2] = AppData.ClampMax(instance.fanParam_[2] + add_eva, 10000); }
		}

		public static void AddRankUpParam(int add_eva, FanActData fa_data)
		{
			fa_data.rankPoint_ += add_eva;
			if (fa_data.rankPoint_ >= 100 && fa_data.rank_ < 6)
			{
				fa_data.rankPoint_ -= 100;
				RankUp(fa_data);
			}
		}

		public static void RankUp(FanActData fa_data)
		{
			if (fa_data.rank_ < 6)
			{
				fa_data.rank_++;
			}
		}

		public static int ChangeAssistAddEva(int add_eva)
		{
			UserData instance = UserData.GetInstance();
			int num = instance.fanParam_[1] - add_eva;
			int result = GameUtil.RateConvert(num, -500, 500, 300, 10, true);
			if (num >= 500)
			{
				result = 0;
			}
			return result;
		}

		public static bool IsExistNew()
		{
			if (!UserData.GetInstance().ChEventFlg(2))
			{
				return false;
			}
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.fanActData_.Length; i++)
			{
				if (instance.fanActData_[i].IsNew())
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.fanActData_.Length; i++)
			{
				instance.fanActData_[i].new_ = false;
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
