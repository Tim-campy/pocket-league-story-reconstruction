using System;
using System.IO;

namespace java.io
{
	public class OutputStream : IDisposable
	{
		protected Stream Wrapped;

		public static implicit operator OutputStream(Stream s)
		{
			return Wrap(s);
		}

		public static implicit operator Stream(OutputStream s)
		{
			return s.GetWrappedStream();
		}

		public virtual void Close()
		{
			if (Wrapped != null)
			{
				Wrapped.Close();
			}
		}

		public void Dispose()
		{
			Close();
		}

		public virtual void Flush()
		{
			if (Wrapped != null)
			{
				Wrapped.Flush();
			}
		}

		public virtual void Write(int b)
		{
			if (Wrapped is WrappedSystemStream)
			{
				((WrappedSystemStream)Wrapped).OutputStream.Write(b);
				return;
			}
			if (Wrapped == null)
			{
				throw new NotImplementedException();
			}
			Wrapped.WriteByte((byte)b);
		}

		public virtual void Write(byte[] b)
		{
			Write(b, 0, b.Length);
		}

		public virtual void Write(byte[] b, int offset, int len)
		{
			if (Wrapped is WrappedSystemStream)
			{
				((WrappedSystemStream)Wrapped).OutputStream.Write(b, offset, len);
				return;
			}
			if (Wrapped != null)
			{
				Wrapped.Write(b, offset, len);
				return;
			}
			for (int i = 0; i < len; i++)
			{
				Write(b[i + offset]);
			}
		}

		internal Stream GetWrappedStream()
		{
			return new WrappedSystemStream(this);
		}

		internal static OutputStream Wrap(Stream s)
		{
			return new OutputStream
			{
				Wrapped = s
			};
		}
	}
}
