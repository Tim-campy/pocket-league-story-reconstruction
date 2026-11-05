using game;
using java.io;
using java.lang;
using kairo.unity.util;
using main;

namespace data
{
	public class StadiumData : BaseData
	{
		public const int FLAG_GROUND = 1;

		public static AppData apdat_;

		public static UserData udat_;

		public string name_;

		public string fileName_;

		public int img_out_;

		public int mapchip_oy_;

		public StadiumData()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public StadiumData(string src)
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
			fileName_ = array[num++];
			img_out_ = JInteger.ParseInt(array[num++]);
			mapchip_oy_ = JInteger.ParseInt(array[num++]);
			flag_ = JInteger.ParseInt(array[num++]);
		}

		public void NewGame()
		{
		}

		public void Serialize(OutputStream os)
		{
		}

		public void Deserialize(InputStream stream)
		{
		}
	}
}
