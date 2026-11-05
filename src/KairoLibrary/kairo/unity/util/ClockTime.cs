using java.lang;
using kairo.unity.native;

namespace kairo.unity.util
{
	public class ClockTime
	{
		private static bool isSetTime_;

		public static void SetTime(long time, int precision = 10)
		{
			KairoPlugin.SetClockTime(time, precision);
			isSetTime_ = true;
		}

		public static long GetTime()
		{
			if (!isSetTime_)
			{
				return JSystem.CurrentTimeMillis();
			}
			return KairoPlugin.GetClockTime();
		}

		public static void Destroy()
		{
		}
	}
}
