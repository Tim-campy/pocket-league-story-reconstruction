using System;
using java.native;

namespace java.lang
{
	public class JRuntime
	{
		private static JRuntime instance;

		public static JRuntime GetRuntime()
		{
			if (instance == null)
			{
				instance = new JRuntime();
			}
			return instance;
		}

		public void Exit(int status)
		{
			Environment.Exit(status);
		}

		public long FreeMemory()
		{
			return JavaPlugin.FreeMemory();
		}

		public long TotalMemory()
		{
			return JavaPlugin.TotalMemory();
		}

		public void Gc()
		{
			GC.Collect(0);
		}
	}
}
