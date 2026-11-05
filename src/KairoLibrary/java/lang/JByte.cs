using System.Globalization;

namespace java.lang
{
	public class JByte
	{
		private sbyte value_;

		public const sbyte MAX_VALUE = sbyte.MaxValue;

		public const sbyte MIN_VALUE = sbyte.MinValue;

		public static implicit operator sbyte(JByte s)
		{
			return s.ByteValue();
		}

		public static implicit operator JByte(sbyte s)
		{
			return new JByte(s);
		}

		public JByte(sbyte value)
		{
			value_ = value;
		}

		public sbyte ByteValue()
		{
			return value_;
		}

		public override string ToString()
		{
			return value_.ToString();
		}

		public static sbyte ParseByte(string s)
		{
			return sbyte.Parse(s);
		}

		public static sbyte ParseByte(string s, int radix)
		{
			if (radix == 16)
			{
				return sbyte.Parse(s, NumberStyles.AllowHexSpecifier);
			}
			return sbyte.Parse(s);
		}

		public override bool Equals(object obj)
		{
			if (obj is JByte)
			{
				return value_ == ((JByte)obj).value_;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
