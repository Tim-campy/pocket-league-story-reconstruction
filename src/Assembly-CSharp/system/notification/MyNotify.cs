using kairo.unity.system.notification.ui;

namespace system.notification
{
	public class MyNotify : NotifyManager
	{
		private MyNotify()
		{
		}

		public new static MyNotify GetInstance()
		{
			if (NotifyManager.instance_ == null)
			{
				NotifyManager.instance_ = new MyNotify();
			}
			return (MyNotify)NotifyManager.instance_;
		}

		public override void OnNotify()
		{
		}

		public override void OnReceiveValue()
		{
			for (int num = GetValuesNum() - 1; num >= 0; num--)
			{
				string value = GetValue(num);
				RemoveValues(num);
			}
		}
	}
}
