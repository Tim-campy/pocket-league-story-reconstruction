using game;
using java.io;
using java.lang;
using java.util;
using kairo.unity.util;
using main;

namespace data
{
	public class SupervisorData : BaseData
	{
		public const int FLAG_INIT_SUPERVISOR = 1;

		public const int FLAG_INIT = 2;

		public const int STATE_NOTFOUND = 0;

		public const int STATE_FOUND = 1;

		public const int MIN_LV = 1;

		public const int MAX_LV = 9;

		public const int TRAINING_POINT_MIN = 1000;

		public const int TRAINING_POINT_MAX = 4000;

		public const int TRAINING_POLICY_NORMAL = 0;

		public const int TRAINING_POLICY_TRAINING = 1;

		public const int TRAINING_POLICY_TENANT = 2;

		public const int TRAINING_POLICY_JOIN_NORMAL = 50;

		public const int TRAINING_POLICY_JOIN_TRAINING = 100;

		public const int TRAINING_POLICY_JOIN_TENANT = 20;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public string name2_;

		public string explain_;

		public int headImg_;

		public int bodyImg_;

		public int initLv_;

		public int[][] dataPhysical_;

		public int[][] dataResearch_;

		public int formation_;

		public int[] sallary_;

		public int winTotal_;

		public int state_;

		public int lv_;

		public int workMonth_;

		public int[] paramPhysical_ = new int[5];

		public int[] paramResearch_ = new int[4];

		public int rankPhysical_;

		public static readonly string[] TRAINING_POLICY_STR = new string[3] { "選手に任せる", "全員で練習", "施設利用をメイン" };

		public SupervisorData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public SupervisorData(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			winTotal_ = 0;
			state_ = 0;
			lv_ = initLv_;
			workMonth_ = 0;
			paramPhysical_ = new int[5];
			paramResearch_ = new int[4];
			rankPhysical_ = 0;
		}

		public void Load(string src)
		{
			int num = 0;
			int num2 = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			name2_ = array[num++];
			if (Language.English())
			{
				name2_ = " " + name2_;
			}
			explain_ = array[num++];
			headImg_ = JInteger.ParseInt(array[num++]);
			bodyImg_ = JInteger.ParseInt(array[num++]);
			initLv_ = JInteger.ParseInt(array[num++]);
			num2 = 0;
			string value = array[num++];
			string[] array2 = StringUtil.Split(value, "&");
			dataPhysical_ = JTool.MakeArray<int[][]>(new int[2] { 5, 2 });
			for (int i = 0; i < dataPhysical_.Length; i++)
			{
				for (int j = 0; j < dataPhysical_[i].Length; j++)
				{
					dataPhysical_[i][j] = JInteger.ParseInt(array2[num2++]);
				}
			}
			num2 = 0;
			value = array[num++];
			array2 = StringUtil.Split(value, "&");
			dataResearch_ = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			for (int k = 0; k < dataResearch_.Length; k++)
			{
				for (int l = 0; l < dataResearch_[k].Length; l++)
				{
					dataResearch_[k][l] = JInteger.ParseInt(array2[num2++]);
				}
			}
			formation_ = JInteger.ParseInt(array[num++]);
			sallary_ = new int[2];
			sallary_[0] = JInteger.ParseInt(array[num++]);
			sallary_[1] = JInteger.ParseInt(array[num++]);
			flag_ = JInteger.ParseInt(array[num++]);
			SetLv(initLv_);
		}

		public void NewGame()
		{
			state_ = 0;
			if (BitUtil.Check(flag_, 2))
			{
				ChangeState(1);
			}
			if (BitUtil.Check(flag_, 1))
			{
				ChangeState(1);
				ContractSupervisor(id_, false, false);
				FormationData formationData = apdat_.formationData_[formation_];
				formationData.new_ = false;
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteInt(os, winTotal_);
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteShort(os, (short)lv_);
			StreamUtil.WriteInt(os, workMonth_);
		}

		public void Deserialize(InputStream stream)
		{
			winTotal_ = StreamUtil.ReadInt(stream);
			state_ = StreamUtil.ReadShort(stream);
			lv_ = StreamUtil.ReadShort(stream);
			workMonth_ = StreamUtil.ReadInt(stream);
			SetLv(lv_);
		}

		public void ChangeState(int state)
		{
			state_ = state;
		}

		public bool LvUp()
		{
			if (lv_ == 9)
			{
				return false;
			}
			SetLv(lv_ + 1);
			return true;
		}

		public void SetLv(int lv)
		{
			lv_ = lv;
			int num = 0;
			for (int i = 0; i < paramPhysical_.Length; i++)
			{
				paramPhysical_[i] = GameUtil.RateConvert(lv_, 1, 9, dataPhysical_[i][0], dataPhysical_[i][1], true);
				num += paramPhysical_[i];
			}
			rankPhysical_ = PlayerData.GetParamEva(num / paramPhysical_.Length);
			for (int j = 0; j < paramResearch_.Length; j++)
			{
				paramResearch_[j] = GameUtil.RateConvert(lv_, 1, 9, dataResearch_[j][0], dataResearch_[j][1], true);
			}
		}

		public void SetResearchData(int gameResultEva, PlayerData p_data)
		{
			int num = 0;
			num = ((udat_.gm_.score_[0] > udat_.gm_.score_[1]) ? 10 : ((udat_.gm_.score_[0] != udat_.gm_.score_[1]) ? 50 : 30));
			if (GameUtil.Random(100) < num)
			{
				p_data.bonus_research[0] = -1;
				return;
			}
			int num2 = 0;
			for (int i = 0; i < paramResearch_.Length; i++)
			{
				num2 += paramResearch_[i];
			}
			int num3 = 0;
			int num4 = GameUtil.Random(num2);
			for (int j = 0; j < paramResearch_.Length; j++)
			{
				if (num4 < paramResearch_[j])
				{
					num3 = j;
					break;
				}
				num4 -= paramResearch_[j];
			}
			p_data.bonus_research[0] = num3;
			p_data.bonus_research[1] = paramResearch_[num3];
			p_data.bonus_research[1] += GameUtil.Random(3) - 1;
			if (udat_.gm_.score_[0] == udat_.gm_.score_[1])
			{
				p_data.bonus_research[1] = p_data.bonus_research[1] * 125 / 100;
			}
			else if (udat_.gm_.score_[0] < udat_.gm_.score_[1])
			{
				p_data.bonus_research[1] = p_data.bonus_research[1] * 150 / 100;
			}
			if (p_data.bonus_research[1] == 0)
			{
				p_data.bonus_research[1] = 1;
			}
		}

		public int GetTrainigGetExp()
		{
			int num = 0;
			for (int i = 0; i < paramPhysical_.Length; i++)
			{
				num += paramPhysical_[i];
			}
			return GameUtil.RateConvert(num / paramPhysical_.Length, 0, 1000, 1, 10, true);
		}

		public int GetSallay()
		{
			return GameUtil.RateConvert(lv_, 1, 9, sallary_[0], sallary_[1], true);
		}

		public static void ContractSupervisor(int svisorid, bool isContinue, bool formationAddPopUp)
		{
			UserData instance = UserData.GetInstance();
			instance.selectSupervisorId_ = svisorid;
			SupervisorData supervisorData = apdat_.svisorData_[svisorid];
			FormationData formationData = apdat_.formationData_[supervisorData.formation_];
			if (formationData.state_ == 0 && formationAddPopUp)
			{
				apdat_.AddPopupMessage(29, formationData.name_);
			}
			formationData.ChangeState(1);
			if (isContinue && supervisorData.LvUp())
			{
				apdat_.ExecEvent(142, null, null);
			}
			if (instance.tp_svisor_ != null)
			{
				instance.tp_svisor_.faceImg_ = supervisorData.headImg_;
				instance.tp_svisor_.bodyImg_ = supervisorData.bodyImg_;
			}
		}
	}
}
