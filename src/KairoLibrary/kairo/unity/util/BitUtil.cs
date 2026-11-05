namespace kairo.unity.util
{
	public class BitUtil
	{
		private static sbyte[] split_tmpB = new sbyte[2];

		private static short[] split_tmpS = new short[2];

		private static int[] split_tmpI = new int[2];

		private BitUtil()
		{
		}

		public static bool Check(long v, long bit)
		{
			return (v & bit) != 0;
		}

		public static long On(long v, long bit)
		{
			return v | bit;
		}

		public static long Off(long v, long bit)
		{
			return v &= ~bit;
		}

		public static bool Check(int v, int bit)
		{
			return (v & bit) != 0;
		}

		public static int On(int v, int bit)
		{
			return v | bit;
		}

		public static int Off(int v, int bit)
		{
			return v &= ~bit;
		}

		public static int On(byte v, byte bit)
		{
			return v | bit;
		}

		public static byte Off(byte v, byte bit)
		{
			return v &= (byte)(~bit);
		}

		public static int On(sbyte v, sbyte bit)
		{
			return v | bit;
		}

		public static sbyte Off(sbyte v, sbyte bit)
		{
			return v &= (sbyte)(~bit);
		}

		public static short On(short v, short bit)
		{
			return (short)(v | bit);
		}

		public static short Off(short v, short bit)
		{
			return v &= (short)(~bit);
		}

		public static short Join(byte high, byte low)
		{
			return (short)(((high & 0xFF) << 8) | (low & 0xFF));
		}

		public static short Join(sbyte high, sbyte low)
		{
			return (short)(((high & 0xFF) << 8) | (low & 0xFF));
		}

		public static sbyte[] Split(short value)
		{
			split_tmpB[0] = (sbyte)((value & 0xFF00) >> 8);
			split_tmpB[1] = (sbyte)(value & 0xFF);
			return split_tmpB;
		}

		public static int Join(short high, short low)
		{
			return ((high & 0xFFFF) << 16) | (low & 0xFFFF);
		}

		public static short[] Split(int value)
		{
			split_tmpS[0] = (short)((value & 0xFFFF0000u) >> 16);
			split_tmpS[1] = (short)(value & 0xFFFF);
			return split_tmpS;
		}

		public static long Join(int high, int low)
		{
			return ((high & 0xFFFFFFFFu) << 32) | (low & 0xFFFFFFFFu);
		}

		public static int[] Split(long value)
		{
			ulong num = 18446744069414584320uL;
			split_tmpI[0] = (int)((long)((ulong)value & num) >> 32);
			split_tmpI[1] = (int)(value & 0xFFFFFFFFu);
			return split_tmpI;
		}
	}
}
