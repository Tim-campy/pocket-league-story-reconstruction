using game;
using java.lang;
using kairo.unity.util;
using main;

namespace data
{
	public class MapchipMatch : BaseData
	{
		public const int FLAG_GROUND = 1;

		public const int FLAG_GROUND_CHANGE = 2;

		public const int FLAG_GROUND_SNOW = 4;

		public const int FLAG_GROUND_DESERT = 8;

		public const int FLAG_GROUND_GRAVEL = 16;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public int img_;

		public int seb_;

		public MapchipMatch()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public MapchipMatch(string src)
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
			flag_ = JInteger.ParseInt(array[num++]);
		}
	}
}
