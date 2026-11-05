using System;
using System.IO;

namespace java.io
{
	internal class WrappedSystemStream : Stream
	{
		private InputStream ist;

		private OutputStream ost;

		private int position;

		private int markedPosition;

		public WrappedSystemStream(InputStream ist)
		{
			this.ist = ist;
		}

		public WrappedSystemStream(OutputStream ost)
		{
			this.ost = ost;
		}

		public InputStream InputStream => ist;

		public OutputStream OutputStream => ost;

		public override void Close()
		{
			if (ist != null)
			{
				ist.Close();
			}
			if (ost != null)
			{
				ost.Close();
			}
		}

		public override void Flush()
		{
			ost.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = ist.Read(buffer, offset, count);
			if (num != -1)
			{
				position += num;
				return num;
			}
			return 0;
		}

		public override int ReadByte()
		{
			int num = ist.Read();
			if (num != -1)
			{
				position++;
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin == SeekOrigin.Begin)
			{
				Position = offset;
			}
			else if (origin == SeekOrigin.Current)
			{
				Position += offset;
			}
			else if (origin == SeekOrigin.End)
			{
				Position = Length + offset;
			}
			return Position;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			ost.Write(buffer, offset, count);
			position += count;
		}

		public override void WriteByte(byte value)
		{
			ost.Write(value);
			position++;
		}

		public override bool CanRead => ist != null;

		public override bool CanSeek => true;

		public override bool CanWrite => ost != null;

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal void OnMark(int nb)
		{
			markedPosition = position;
			ist.Mark(nb);
		}

		public override long Position
		{
			get
			{
				if (ist != null && ist.CanSeek())
				{
					return ist.Position;
				}
				return position;
			}
			set
			{
				if (value == position)
				{
					return;
				}
				if (value == markedPosition)
				{
					ist.Reset();
					return;
				}
				if (ist != null && ist.CanSeek())
				{
					ist.Position = value;
					return;
				}
				throw new NotSupportedException();
			}
		}
	}
}
