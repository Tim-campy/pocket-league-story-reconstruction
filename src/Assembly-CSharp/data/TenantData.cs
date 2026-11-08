using form;
using game;
using java.io;
using java.lang;
using java.util;
using kairo.unity.util;
using main;

namespace data
{
	public class TenantData : BaseData
	{
		public const int FLAG_INIT = 1;

		public const int FLAG_BUILD = 2;

		public const int FLAG_TENANT_GOODS = 4;

		public const int FLAG_TENANT_TAREA_LVUP = 8;

		public const int FLAG_TENANT_TAREA_SPRED = 16;

		public const int FLAG_TENANT_STADIUM = 32;

		public const int TENANTDATAID_VACANT = -1;

		public const int ROOT_R = 0;

		public const int ROOT_L = 1;

		public const int STATE_NOT_FOUND = 0;

		public const int STATE_FOUND = 1;

		public const int STATE_STRENGTH = 2;

		public const int MIN_LV = 1;

		public const int MAX_LV = 5;

		public const int TENANT_STRENGH_TIME = 200;

		public const int TENANTUSE_AURA_MAX = 10;

		public const int TLUP_ID_PRICE = 0;

		public const int TLUP_ID_KEEPCOST = 1;

		public const int TLUP_ID_GP_FAN = 2;

		public const int TLUP_ID_GP_IDEA = 3;

		public const int TLUP_ID_GP_PHY = 4;

		public const int TLUP_ID_GP_TAC = 5;

		public const int TLUP_ID_GP_END = 6;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public int root_;

		public int icon_;

		public int[] img_;

		public int charm_;

		public int[] usePrice_;

		public int[] keepCost_;

		public int[][] dataResearch_;

		public int[] efToFan_;

		public int buildTime_;

		public int buildCost_;

		public int[] investCost_ = new int[2];

		public string explain_;

		public int state_;

		public int stateCnt_;

		public int lv_;

		public bool new_;

		public int[] paramResearch_ = new int[4];

		public int paramUsePrice_;

		public int paramKeepCost_;

		public int paramEfToFan_;

		public int paramStrengthCost_;

		public int paramTenantUseAura_;

		public static readonly int[] INIT_TENANTID = new int[15]
		{
			-1, -1, -1, -1, -1, -1, -1, 0, -1, -1,
			-1, -1, -1, -1, -1
		};

		public static readonly int[] TENANT_ROOT = new int[15]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1, 1, 1, 1
		};

		public static readonly string[] EFTOFAN_STR = new string[3] { "低い", "普通", "高い" };

		public static int TENANT_ID_GOODS;

		public static int TENANT_ID_TRAININGAREA;

		public static int TENANT_ID_TRAININGAREA_SPRED;

		public static int TENANT_ID_STADIUM;

		private int[] TUSEPOINT = new int[2];

		public static int[] TLUP_PARAM = new int[6];

		public TenantData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public TenantData(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			state_ = 0;
			stateCnt_ = 0;
			new_ = false;
			lv_ = 1;
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			root_ = JInteger.ParseInt(array[num++]);
			icon_ = JInteger.ParseInt(array[num++]);
			img_ = new int[4];
			for (int i = 0; i < img_.Length; i++)
			{
				img_[i] = JInteger.ParseInt(array[num++]);
			}
			charm_ = JInteger.ParseInt(array[num++]);
			usePrice_ = new int[2];
			keepCost_ = new int[2];
			dataResearch_ = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			efToFan_ = new int[2];
			for (int j = 0; j < usePrice_.Length; j++)
			{
				usePrice_[j] = JInteger.ParseInt(array[num++]);
			}
			for (int k = 0; k < keepCost_.Length; k++)
			{
				keepCost_[k] = JInteger.ParseInt(array[num++]);
			}
			int num2 = 0;
			string value = array[num++];
			string[] array2 = StringUtil.Split(value, "&");
			dataResearch_ = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			for (int l = 0; l < dataResearch_.Length; l++)
			{
				for (int m = 0; m < dataResearch_[l].Length; m++)
				{
					dataResearch_[l][m] = JInteger.ParseInt(array2[num2++]);
				}
			}
			for (int n = 0; n < efToFan_.Length; n++)
			{
				efToFan_[n] = JInteger.ParseInt(array[num++]);
			}
			buildTime_ = JInteger.ParseInt(array[num++]);
			buildCost_ = JInteger.ParseInt(array[num++]);
			for (int num3 = 0; num3 < investCost_.Length; num3++)
			{
				investCost_[num3] = JInteger.ParseInt(array[num++]);
			}
			explain_ = array[num++];
			flag_ = JInteger.ParseInt(array[num++]);
			SetLv(1);
		}

		public static void Load2()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.tenantData_.Length; i++)
			{
				TenantData tenantData = instance.tenantData_[i];
				if (BitUtil.Check(tenantData.flag_, 4))
				{
					TENANT_ID_GOODS = i;
				}
				else if (BitUtil.Check(tenantData.flag_, 8))
				{
					TENANT_ID_TRAININGAREA = i;
				}
				else if (BitUtil.Check(tenantData.flag_, 16))
				{
					TENANT_ID_TRAININGAREA_SPRED = i;
				}
				else if (BitUtil.Check(tenantData.flag_, 32))
				{
					TENANT_ID_STADIUM = i;
				}
			}
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
			StreamUtil.WriteInt(os, stateCnt_);
			StreamUtil.WriteShort(os, (short)lv_);
			StreamUtil.WriteBoolean(os, new_);
		}

		public void Deserialize(InputStream stream)
		{
			state_ = StreamUtil.ReadShort(stream);
			stateCnt_ = StreamUtil.ReadInt(stream);
			lv_ = StreamUtil.ReadShort(stream);
			new_ = StreamUtil.ReadBoolean(stream);
			SetLv(lv_);
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

		public void LvUp()
		{
			if (lv_ == 5) { return; }
{

SetLv(lv_ + 1);
}
		}

		public void SetLv(int lv)
		{
			lv_ = lv;
			for (int i = 0; i < paramResearch_.Length; i++)
			{
				paramResearch_[i] = GameUtil.RateConvert(lv_, 1, 5, dataResearch_[i][0], dataResearch_[i][1], true);
			}
			paramUsePrice_ = GameUtil.RateConvert(lv_, 1, 5, usePrice_[0], usePrice_[1], true);
			paramKeepCost_ = GameUtil.RateConvert(lv_, 1, 5, keepCost_[0], keepCost_[1], true);
			paramEfToFan_ = GameUtil.RateConvert(lv_, 1, 5, efToFan_[0], efToFan_[1], true);
			paramStrengthCost_ = GameUtil.RateConvert(lv_, 1, 5, investCost_[0], investCost_[1], true);
			paramTenantUseAura_ = GameUtil.RateConvert(lv_, 1, 5, efToFan_[0], efToFan_[1], true);
		}

		public static int GetFanToEf(int eva)
		{
			if (eva <= 3)
			{
				return 0;
			}
			if (eva <= 7)
			{
				return 1;
			}
			return 2;
		}

		public int GetUserPrice()
		{
			return GameUtil.RateConvert(lv_, 1, 5, usePrice_[0], usePrice_[1], true);
		}

		public int[] GetTenantUseResearchPoint()
		{
			int num = 0;
			for (int i = 0; i < paramResearch_.Length; i++)
			{
				num += paramResearch_[i];
			}
			int num2 = 0;
			int num3 = GameUtil.Random(num);
			for (int j = 0; j < paramResearch_.Length; j++)
			{
				if (num3 < paramResearch_[j])
				{
					num2 = j;
					break;
				}
				num3 -= paramResearch_[j];
			}
			TUSEPOINT[0] = num2;
			TUSEPOINT[1] = paramResearch_[num2];
			return TUSEPOINT;
		}

		public float GetGoodsIncomeRate()
		{
			return GameUtil.RateConvert(lv_, 1, 5, 100, 500, true);
		}

		public void StrengthTenant()
		{
			if (lv_ == 5)
			{
				return;
			}
			for (int i = 0; i < udat_.tenants_.Length; i++)
			{
				Tenant tenant = udat_.tenants_[i];
				if (tenant.state_ == 0) { continue; }
if (tenant.tenantDataId_ == id_) { tenant.StrenghTenant(); }
			}
			ChangeState(2);
		}

		public void Update()
		{
			if (state_ != 2) { return; }
stateCnt_++;
if (stateCnt_ >= 200)
				{
					SubForm subForm = new SubForm(64);
					subForm.tenantDataId_ = id_;
					apdat_.Push(subForm);
					ChangeState(1);
				}
		}

		public int[] GetLvUpAddParam()
		{
			TLUP_PARAM[0] = GameUtil.RateConvert(lv_ + 1, 1, 5, usePrice_[0], usePrice_[1], true) - paramUsePrice_;
			TLUP_PARAM[1] = GameUtil.RateConvert(lv_ + 1, 1, 5, keepCost_[0], keepCost_[1], true) - paramKeepCost_;
			for (int i = 0; i < paramResearch_.Length; i++)
			{
				TLUP_PARAM[2 + i] = GameUtil.RateConvert(lv_ + 1, 1, 5, dataResearch_[i][0], dataResearch_[i][1], true) - paramResearch_[i];
			}
			return TLUP_PARAM;
		}

		public static void ProcStrenSpecialTenant(TenantData tenant_data)
		{
			AppData instance = AppData.GetInstance();
			UserData instance2 = UserData.GetInstance();
			if (tenant_data.id_ == TENANT_ID_STADIUM)
			{
				TeamData teamData = instance.tData_[0];
				if (teamData.stadium_ == 1)
				{
					teamData.stadium_ = 0;
				}
				else if (teamData.stadium_ == 0)
				{
					teamData.stadium_ = 2;
				}
				tenant_data.LvUp();
				if (teamData.stadium_ == 0)
				{
					instance.ExecEvent(187, null, null);
				}
				else if (teamData.stadium_ == 2)
				{
					instance.ExecEvent(188, null, null);
				}
			}
			else if (tenant_data.id_ == TENANT_ID_TRAININGAREA)
			{
				instance2.tAreaLv_ = AppData.ClampMax(instance2.tAreaLv_ + 1, 2);
				instance.frmGame_.SetAllDraw();
				tenant_data.LvUp();
				if (instance2.tAreaLv_ == 1)
				{
					instance.ExecEvent(184, null, null);
				}
				else if (instance2.tAreaLv_ == 2)
				{
					instance.ExecEvent(185, null, null);
				}
			}
			else if (tenant_data.id_ == TENANT_ID_TRAININGAREA_SPRED)
			{
				instance2.tAreaRootNum_ = AppData.ClampMax(instance2.tAreaRootNum_ + 1, 2);
				instance.mapMain_.UpdateTrainingCoatNum();
				instance.frmGame_.SetAllDraw();
				tenant_data.LvUp();
				instance.ExecEvent(186, null, null);
			}
		}

		public bool IsStreangth()
		{
			if (!IsMaxLv())
			{
				return true;
			}
			return false;
		}

		public bool IsMaxLv()
		{
			if (id_ == TENANT_ID_STADIUM)
			{
				if (lv_ >= 3)
				{
					return true;
				}
			}
			else if (id_ == TENANT_ID_TRAININGAREA)
			{
				if (lv_ >= 3)
				{
					return true;
				}
			}
			else if (id_ == TENANT_ID_TRAININGAREA_SPRED)
			{
				if (lv_ >= 2)
				{
					return true;
				}
			}
			else if (lv_ >= 5)
			{
				return true;
			}
			return false;
		}

		public bool IsTAreaOrStadiumTenant()
		{
			if (id_ == TENANT_ID_STADIUM || id_ == TENANT_ID_TRAININGAREA || id_ == TENANT_ID_TRAININGAREA_SPRED)
			{
				return true;
			}
			return false;
		}

		public static bool IsExistNew()
		{
			if (!UserData.GetInstance().ChEventFlg(4))
			{
				return false;
			}
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.tenantData_.Length; i++)
			{
				if (instance.tenantData_[i].IsNew())
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.tenantData_.Length; i++)
			{
				instance.tenantData_[i].new_ = false;
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
