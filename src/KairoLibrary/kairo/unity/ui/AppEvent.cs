namespace kairo.unity.ui
{
	public class AppEvent
	{
		public AppEventType Type;

		public object[] Args;

		internal AppEvent(AppEventType type, params object[] args)
		{
			Type = type;
			Args = args;
			if (Args == null)
			{
				Args = new object[0];
			}
		}
	}
}
