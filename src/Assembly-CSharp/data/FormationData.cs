using game;
using java.io;
using java.lang;
using java.util;
using kairo.unity.util;
using main;

namespace data
{
	public class FormationData : BaseData
	{
		public const int FLAG_INIT = 1;

		public const int FLAG_EDIT = 2;

		public const int STATE_NOT_FOUND = 0;

		public const int STATE_FOUND = 1;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public int[][] formation_;

		public string explain_;

		public int state_;

		public bool new_;

		public FormationData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public FormationData(string src)
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
			string src2 = array[num++];
			int[] array2 = StringUtil.SplitIntegers(src2, ",");
			formation_ = JTool.MakeArray<int[][]>(new int[2] { 11, 2 });
			for (int i = 0; i < formation_.Length; i++)
			{
				formation_[i][0] = array2[i * 2];
				formation_[i][1] = array2[i * 2 + 1];
			}
			explain_ = array[num++];
			flag_ = JInteger.ParseInt(array[num++]);
		}

		public void NewGame()
		{
			state_ = 0;
			if (BitUtil.Check(flag_, 1))
			{
				state_ = 1;
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteBoolean(os, new_);
			for (int i = 0; i < formation_.Length; i++)
			{
				for (int j = 0; j < formation_[i].Length; j++)
				{
					StreamUtil.WriteShort(os, (short)formation_[i][j]);
				}
			}
		}

		public void Deserialize(InputStream stream)
		{
			state_ = StreamUtil.ReadShort(stream);
			new_ = StreamUtil.ReadBoolean(stream);
			for (int i = 0; i < formation_.Length; i++)
			{
				for (int j = 0; j < formation_[i].Length; j++)
				{
					formation_[i][j] = StreamUtil.ReadShort(stream);
				}
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

		public static void UpdateStamemRole()
		{
			UserData instance = UserData.GetInstance();
			FormationData formationData = AppData.GetInstance().formationData_[instance.selectFormationId_];
			for (int i = 0; i < 11; i++)
			{
				PlayerData playerData = (PlayerData)instance.memberPDataist_.ElementAt(i);
				if (i == 0)
				{
					playerData.role_ = 0;
				}
				else if (formationData.formation_[i][1] <= 2)
				{
					playerData.role_ = 3;
				}
				else if (formationData.formation_[i][1] <= 5)
				{
					playerData.role_ = 2;
				}
				else if (formationData.formation_[i][1] <= 8)
				{
					playerData.role_ = 1;
				}
			}
		}

		public static bool IsExistNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.formationData_.Length; i++)
			{
				if (instance.formationData_[i].IsNew())
				{
					return true;
				}
			}
			return false;
		}

		public static void RemoveNew()
		{
			AppData instance = AppData.GetInstance();
			for (int i = 0; i < instance.formationData_.Length; i++)
			{
				instance.formationData_[i].new_ = false;
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
