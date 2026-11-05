using System;

namespace kairo.unity.ui
{
	public class UIException : Exception
	{
		public const int UNDEFINED = 0;

		public const int ILLEGAL_STATE = 1;

		public const int NO_RESOURCES = 2;

		public const int BUSY_RESOURCE = 3;

		public const int UNSUPPORTED_FORMAT = 4;

		public int GetStatus()
		{
			return -1;
		}
	}
}
