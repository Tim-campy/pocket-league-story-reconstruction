using System;
using System.Globalization;

namespace java.lang
{
	public class JInteger
	{
		private int value_;

		public const int MAX_VALUE = int.MaxValue;

		public const int MIN_VALUE = int.MinValue;

		public static implicit operator int(JInteger s)
		{
			return s.IntValue();
		}

		public static implicit operator JInteger(int s)
		{
			return new JInteger(s);
		}

		public JInteger(int value)
		{
			value_ = value;
		}

		public int IntValue()
		{
			return value_;
		}

		public static JInteger ValueOf(string s)
		{
			return new JInteger(ParseInt(s));
		}

		public static JInteger ValueOf(string s, int radix)
		{
			return new JInteger(ParseInt(s, radix));
		}

		public override string ToString()
		{
			return value_.ToString();
		}

		public static string ToString(int i)
		{
			return i.ToString();
		}

		public static string ToString(int i, int radix)
		{
			if (i >= 0)
			{
				return i.ToString("x");
			}
			return "-" + Math.Abs(i).ToString("x");
		}

		public static string ToBinaryString(int i)
		{
			return Convert.ToString(i, 2);
		}

		public static string ToHexString(int i)
		{
			return Convert.ToString(i, 16);
		}

		public static string ToOctalString(int i)
		{
			return Convert.ToString(i, 8);
		}

		public static int ParseInt(string s)
		{
			return int.Parse(s);
		}

		public static int ParseInt(string s, int radix)
		{
			if (radix == 16)
			{
				return int.Parse(s, NumberStyles.AllowHexSpecifier);
			}
			return int.Parse(s);
		}

		public override bool Equals(object obj)
		{
			if (obj is JInteger)
			{
				return value_ == ((JInteger)obj).value_;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
