using System.IO;

namespace java.io
{
	public class EOFException : EndOfStreamException
	{
		public EOFException(string s)
			: base(s)
		{
		}
	}
}
