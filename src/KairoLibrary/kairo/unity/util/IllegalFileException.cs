using System.IO;

namespace kairo.unity.util
{
	public class IllegalFileException : IOException
	{
		public IllegalFileException()
		{
		}

		public IllegalFileException(string msg)
			: base(msg)
		{
		}
	}
}
