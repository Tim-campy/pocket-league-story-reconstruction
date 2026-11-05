using System.Globalization;

namespace java.lang
{
	public class JShort
	{
		private short value_;

		public const short MAX_VALUE = short.MaxValue;

		public const short MIN_VALUE = short.MinValue;

		public static implicit operator short(JShort s)
		{
			return s.ShortValue();
		}

		public static implicit operator JShort(short s)
		{
			return new JShort(s);
		}

		public JShort(short value)
		{
			value_ = value;
		}

		public short ShortValue()
		{
			return value_;
		}

		public override string ToString()
		{
			return value_.ToString();
		}

		public static short ParseShort(string s)
		{
			return short.Parse(s);
		}

		public static short ParseShort(string s, int radix)
		{
			if (radix == 16)
			{
				return short.Parse(s, NumberStyles.AllowHexSpecifier);
			}
			return short.Parse(s);
		}

		public override bool Equals(object obj)
		{
			if (obj is JShort)
			{
				return value_ == ((JShort)obj).value_;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
