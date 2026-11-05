using game;
using java.lang;
using kairo.unity.util;
using main;

namespace data
{
	public class MapchipMain : BaseData
	{
		public const int FLAG_GROUND = 1;

		public const int FLAG_OUT_MAP = 2;

		public const int FLAG_GROUND_SEASON_CHANGE = 4;

		public const int FLAG_GROUND_GROUND = 8;

		public const int FLAG_GROUND_SNOW = 16;

		public const int FLAG_GROUND_GRAVEL = 32;

		public const int FLAG_GROUND_GRASS2 = 64;

		public const int FLAG_VACANT = 128;

		public const int FLAG_TREE = 256;

		public const int FLAG_FENCE_GRASS = 512;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public int img_;

		public int seb_;

		public int fenceId_;

		public MapchipMain()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public MapchipMain(string src)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Load(src);
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			img_ = JInteger.ParseInt(array[num++]);
			seb_ = JInteger.ParseInt(array[num++]);
			fenceId_ = JInteger.ParseInt(array[num++]);
			flag_ = JInteger.ParseInt(array[num++]);
		}
	}
}
