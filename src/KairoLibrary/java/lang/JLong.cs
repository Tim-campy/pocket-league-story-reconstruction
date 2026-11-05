using System;
using System.Globalization;

namespace java.lang
{
	public class JLong
	{
		private long value_;

		public const long MAX_VALUE = long.MaxValue;

		public const long MIN_VALUE = long.MinValue;

		public static implicit operator long(JLong s)
		{
			return s.LongValue();
		}

		public static implicit operator JLong(long s)
		{
			return new JLong(s);
		}

		public JLong(long value)
		{
			value_ = value;
		}

		public long LongValue()
		{
			return value_;
		}

		public override string ToString()
		{
			return value_.ToString();
		}

		public static string ToString(long i)
		{
			return i.ToString();
		}

		public static string ToString(long i, int radix)
		{
			if (i >= 0)
			{
				return i.ToString("x");
			}
			return "-" + Math.Abs(i).ToString("x");
		}

		public static long ParseLong(string s)
		{
			return long.Parse(s);
		}

		public static long ParseLong(string s, int radix)
		{
			if (radix == 16)
			{
				return long.Parse(s, NumberStyles.AllowHexSpecifier);
			}
			return long.Parse(s);
		}

		public override bool Equals(object obj)
		{
			if (obj is JLong)
			{
				return value_ == ((JLong)obj).value_;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
