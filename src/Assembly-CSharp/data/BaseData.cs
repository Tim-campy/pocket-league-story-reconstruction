namespace data
{
	public class BaseData
	{
		public int id_;

		public int flag_;

		public bool Check(int bit)
		{
			return (flag_ & bit) != 0;
		}
	}
}
