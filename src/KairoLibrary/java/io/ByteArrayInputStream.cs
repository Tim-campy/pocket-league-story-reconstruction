using System.IO;

namespace java.io
{
	public class ByteArrayInputStream : InputStream
	{
		public ByteArrayInputStream(byte[] data)
		{
			Wrapped = new MemoryStream(data);
		}

		public ByteArrayInputStream(byte[] data, int off, int len)
		{
			Wrapped = new MemoryStream(data, off, len);
		}

		public override int Available()
		{
			MemoryStream memoryStream = (MemoryStream)Wrapped;
			return (int)(memoryStream.Length - memoryStream.Position);
		}
	}
}
