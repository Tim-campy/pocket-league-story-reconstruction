using System;
using System.IO;

namespace java.io
{
	public class InputStream : IDisposable
	{
		private long mark;

		protected Stream Wrapped;

		protected Stream BaseStream;

		internal long Position
		{
			get
			{
				if (Wrapped != null)
				{
					return Wrapped.Position;
				}
				throw new NotSupportedException();
			}
			set
			{
				if (Wrapped != null)
				{
					Wrapped.Position = value;
					return;
				}
				throw new NotSupportedException();
			}
		}

		public static implicit operator InputStream(Stream s)
		{
			return Wrap(s);
		}

		public static implicit operator Stream(InputStream s)
		{
			return s.GetWrappedStream();
		}

		public virtual int Available()
		{
			if (Wrapped is WrappedSystemStream)
			{
				return ((WrappedSystemStream)Wrapped).InputStream.Available();
			}
			return 0;
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

		public virtual void Mark(int readlimit)
		{
			if (Wrapped is WrappedSystemStream)
			{
				((WrappedSystemStream)Wrapped).InputStream.Mark(readlimit);
				return;
			}
			if (BaseStream is WrappedSystemStream)
			{
				((WrappedSystemStream)BaseStream).OnMark(readlimit);
			}
			if (Wrapped != null)
			{
				mark = Wrapped.Position;
			}
		}

		public virtual bool MarkSupported()
		{
			if (Wrapped is WrappedSystemStream)
			{
				return ((WrappedSystemStream)Wrapped).InputStream.MarkSupported();
			}
			if (Wrapped != null)
			{
				return Wrapped.CanSeek;
			}
			return false;
		}

		public virtual int Read()
		{
			if (Wrapped == null)
			{
				throw new NotImplementedException();
			}
			return Wrapped.ReadByte();
		}

		public virtual int Read(byte[] buf)
		{
			return Read(buf, 0, buf.Length);
		}

		public virtual int Read(byte[] b, int off, int len)
		{
			if (Wrapped is WrappedSystemStream)
			{
				return ((WrappedSystemStream)Wrapped).InputStream.Read(b, off, len);
			}
			if (Wrapped != null)
			{
				int num = Wrapped.Read(b, off, len);
				if (num > 0)
				{
					return num;
				}
				return -1;
			}
			int i;
			for (i = 0; i < len; i++)
			{
				int num2 = Read();
				if (num2 == -1)
				{
					return -1;
				}
				b[off + i] = (byte)num2;
			}
			return i;
		}

		public virtual void Reset()
		{
			if (Wrapped is WrappedSystemStream)
			{
				((WrappedSystemStream)Wrapped).InputStream.Reset();
				return;
			}
			if (Wrapped == null)
			{
				throw new IOException();
			}
			Wrapped.Position = mark;
		}

		public virtual long Skip(long cnt)
		{
			if (Wrapped is WrappedSystemStream)
			{
				return ((WrappedSystemStream)Wrapped).InputStream.Skip(cnt);
			}
			long num;
			for (num = cnt; num > 0; num--)
			{
				if (Read() == -1)
				{
					return cnt - num;
				}
			}
			return cnt - num;
		}

		internal Stream GetWrappedStream()
		{
			return new WrappedSystemStream(this);
		}

		internal bool CanSeek()
		{
			if (Wrapped != null)
			{
				return Wrapped.CanSeek;
			}
			return false;
		}

		internal static InputStream Wrap(Stream s)
		{
			return new InputStream
			{
				Wrapped = s
			};
		}
	}
}
