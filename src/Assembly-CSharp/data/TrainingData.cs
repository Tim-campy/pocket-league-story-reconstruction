using game;
using java.io;
using java.lang;
using java.util;
using kairo.unity.util;
using main;

namespace data
{
	public class TrainingData : BaseData
	{
		public const int FLAG_INIT = 1;

		public const int STATE_NOT_FOUND = 0;

		public const int STATE_FOUND = 1;

		public const int PUP_KICK = 0;

		public const int PUP_SPEED = 1;

		public const int PUP_TEQNIC = 2;

		public const int PUP_BODY = 3;

		public const int PUP_KEEPER = 4;

		public const int PUP_GK = 5;

		public const int PUP_END = 6;

		public const int COSTTYPE_FANSERVICE = 0;

		public const int COSTTYPE_IDEA = 1;

		public const int COSTTYPE_PHYSICAL = 2;

		public const int COSTTYPE_TACTICS = 3;

		public const int COSTTYPE_MONEY = 4;

		public const int TRA_NOTSET = -1;

		public const int TRAMODE_KICK = 0;

		public const int TRAMODE_RUN = 1;

		public const int TRAMODE_HEAD = 2;

		public const int TRAMODE_THROWIN = 3;

		public const int FARESULT_SCCESS = 0;

		public const int FARESULT_NORMAL = 1;

		public const int FARESULT_FAIL = 2;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public int icon_;

		public int tMode_;

		public int[][] eff_ = JTool.MakeArray<int[][]>(new int[2] { 6, 2 });

		public int costType_;

		public int cost_;

		public string explain_;

		public int state_;

		public bool new_;

		public static int[] TRA_PLAYER = new int[3];

		public static int TRA_SELECT_TRA = 0;

		public static int[][] TRA_UPPARAM = JTool.MakeArray<int[][]>(new int[2] { 3, 6 });

		public static int[][] TRA_ADD = JTool.MakeArray<int[][]>(new int[2] { 3, 6 });

		public static int[][] TRA_UPPARAM_DISP = JTool.MakeArray<int[][]>(new int[2] { 3, 6 });

		public static readonly int[] FARESULT_RATE = new int[3] { 8, 92, 100 };

		public static int fa_result = 0;

		public TrainingData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public TrainingData(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
			Load(src);
		}

		public void Init()
		{
			state_ = 0;
			new_ = false;
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			icon_ = JInteger.ParseInt(array[num++]);
			tMode_ = JInteger.ParseInt(array[num++]);
			for (int i = 0; i < eff_.Length; i++)
			{
				eff_[i][0] = JInteger.ParseInt(array[num++]);
				eff_[i][1] = JInteger.ParseInt(array[num++]);
			}
			costType_ = JInteger.ParseInt(array[num++]);
			cost_ = JInteger.ParseInt(array[num++]);
			explain_ = array[num++];
			flag_ = JInteger.ParseInt(array[num++]);
		}

		public static void Load2()
		{
			for (int i = 0; i < TRA_PLAYER.Length; i++)
			{
				TRA_PLAYER[i] = -1;
			}
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
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteBoolean(os, new_);
		}

		public void Deserialize(InputStream stream)
		{
			state_ = StreamUtil.ReadShort(stream);
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

		public static int GetTrainingAddParamNum(int eva)
		{
			if (eva <= 5)
			{
				return 1;
			}
			if (eva <= 10)
			{
				return 2;
			}
			return 3;
		}

		public static bool IsExistNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.trainingData_.Length; i++)
			{
				if (instance.trainingData_[i].IsNew())
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.trainingData_.Length; i++)
			{
				instance.trainingData_[i].new_ = false;
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
