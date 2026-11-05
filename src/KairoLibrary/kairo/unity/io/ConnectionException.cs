using System;

namespace kairo.unity.io
{
	public class ConnectionException : Exception
	{
		public int GetStatus()
		{
			return -1;
		}
	}
}
