namespace kairo.unity.io
{
	public class BitArrayInputStream
	{
		protected byte[] buf_;

		protected int index_;

		protected int bit_;

		public BitArrayInputStream(byte[] buf)
		{
			buf_ = buf;
			bit_ = 0;
		}

		public BitArrayInputStream(byte[] buf, int offset)
		{
			buf_ = buf;
			index_ = offset;
			bit_ = 0;
		}

		public int Read()
		{
			if (index_ >= buf_.Length)
			{
				return -1;
			}
			int result = (buf_[index_] >> bit_) & 1;
			if (++bit_ >= 8)
			{
				index_++;
				bit_ = 0;
			}
			return result;
		}

		public void Close()
		{
			buf_ = null;
		}

		public long Skip(long n)
		{
			long num = 0L;
			long num2 = 0L;
			while (num2 < n) {
if (Read() == -1) { break; }

				num2++;
				num++;
			}
			return num;
		}
	}
}
