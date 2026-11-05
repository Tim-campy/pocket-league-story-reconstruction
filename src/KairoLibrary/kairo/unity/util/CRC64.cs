namespace kairo.unity.util
{
	public class CRC64
	{
		public const int SIZE = 8;

		private static long[] crcTable_ = null;

		private CRC64()
		{
		}

		private static void MakeCrcTable()
		{
			crcTable_ = new long[256];
			for (int i = 0; i < 256; i++)
			{
				long num = i;
				for (int j = 0; j < 8; j++)
				{
					num = (((num & 1) != 0L) ? (num >> 1) : (0xEDB88320u ^ (num >> 1)));
				}
				crcTable_[i] = num;
			}
		}

		private static long Update(long crc, byte[] buf, int len)
		{
			long num = crc;
			if (crcTable_ == null)
			{
				MakeCrcTable();
			}
			for (int i = 0; i < len; i++)
			{
				num = crcTable_[((int)num ^ buf[i]) & 0xFF] ^ (num >> 8);
			}
			return num;
		}

		public static long GetValue(byte[] buf)
		{
			return Update(4294967295L, buf, buf.Length) ^ 0xFFFFFFFFu;
		}
	}
}
