namespace kairo.unity.io
{
	public class BitArrayOutputStream
	{
		protected ByteListOutputStream blos_;

		protected int buf_;

		protected int len_;

		protected int count_;

		public BitArrayOutputStream()
		{
			blos_ = new ByteListOutputStream();
		}

		public void Close()
		{
			if (blos_ != null)
			{
				blos_.Close();
			}
			blos_ = null;
		}

		public void Reset()
		{
			blos_.Reset();
		}

		public int Size()
		{
			return blos_.Size();
		}

		public int BitCount()
		{
			return count_;
		}

		public byte[] ToByteArray()
		{
			if (len_ >= 0)
			{
				blos_.Write(buf_);
				buf_ = (len_ = 0);
			}
			return blos_.ToByteArray();
		}

		public byte[] ToByteArray(int index, int length)
		{
			return ToByteArray(index, length, null);
		}

		public byte[] ToByteArray(int index, int length, byte[] dst)
		{
			if (len_ >= 0)
			{
				blos_.Write(buf_);
				buf_ = (len_ = 0);
			}
			return blos_.ToByteArray(index, length, dst);
		}

		public void Write(int b)
		{
			buf_ |= (b & 1) << len_;
			if (++len_ >= 8)
			{
				blos_.Write(buf_);
				buf_ = (len_ = 0);
			}
		}

		public void Flush()
		{
		}
	}
}
