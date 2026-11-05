using System.IO;
using System.Text;

namespace java.io
{
	public class ByteArrayOutputStream : OutputStream
	{
		public ByteArrayOutputStream()
		{
			Wrapped = new MemoryStream();
		}

		public ByteArrayOutputStream(int bufferSize)
		{
			Wrapped = new MemoryStream(bufferSize);
		}

		public long Size()
		{
			return ((MemoryStream)Wrapped).Length;
		}

		public byte[] ToByteArray()
		{
			return ((MemoryStream)Wrapped).ToArray();
		}

		public override void Close()
		{
		}

		public override string ToString()
		{
			return Encoding.UTF8.GetString(ToByteArray());
		}

		public void Reset()
		{
			Wrapped = new MemoryStream();
		}
	}
}
