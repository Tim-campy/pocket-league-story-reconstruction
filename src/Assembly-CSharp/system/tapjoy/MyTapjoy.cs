using system.tapjoy.ui;

namespace system.tapjoy
{
	public class MyTapjoy : Tapjoy
	{
		private const string SDK_KEY = "";

		private const string OFFEREALL_NAME = "GetFreeTicket";

		public MyTapjoy()
			: base(string.Empty, "GetFreeTicket")
		{
		}

		public new static MyTapjoy GetInstance()
		{
			if (Tapjoy.instance_ == null)
			{
				Tapjoy.instance_ = new MyTapjoy();
			}
			return (MyTapjoy)Tapjoy.instance_;
		}
	}
}
