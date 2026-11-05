using main;

namespace game
{
	public class MapObject
	{
		private static AppData apdat_;

		private static UserData udat_;

		public int img_;

		public int[] spos_;

		public MapObject()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			Init();
		}

		public void Init()
		{
			spos_ = new int[2];
		}
	}
}
