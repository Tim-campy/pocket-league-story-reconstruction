using java.lang;
using java.util;

namespace kairo.unity.util
{
	public abstract class SecureNumber
	{
		protected static JRandom random_;

		protected static long hacking_;

		protected long bitmask_;

		protected long original_;

		protected long value_;

		public SecureNumber()
		{
		}

		public SecureNumber(long value)
		{
			SetLong(value);
		}

		public SecureNumber(SecureNumber src)
		{
			SetLong(src.GetLong());
		}

		protected SecureNumber SetLong(long value)
		{
			if (random_ == null)
			{
				random_ = new JRandom();
			}
			bitmask_ = 0x22 | random_.NextInt();
			original_ = value;
			value_ = Generate(value);
			return this;
		}

		protected byte GetByte()
		{
			return (byte)GetLong();
		}

		protected short GetShort()
		{
			return (short)GetLong();
		}

		protected int GetInt()
		{
			return (int)GetLong();
		}

		protected long GetLong()
		{
			long num = Generate(value_);
			if (num != original_)
			{
				hacking_++;
				original_ = num;
			}
			return num;
		}

		private long Generate(long value)
		{
			return ~(value | ~bitmask_) | (value & ~bitmask_);
		}

		public override string ToString()
		{
			return JString.ValueOf(GetLong());
		}

		public static long GetHackingCount()
		{
			return hacking_;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
