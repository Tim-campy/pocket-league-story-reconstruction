namespace java.lang
{
	public class JDouble
	{
		private double value_;

		public const double MAX_VALUE = double.MaxValue;

		public const double MIN_VALUE = double.MinValue;

		public const double NaN = double.NaN;

		public const double NEGATIVE_INFINITY = double.NegativeInfinity;

		public const double POSITIVE_INFINITY = double.PositiveInfinity;

		public static implicit operator double(JDouble s)
		{
			return s.DoubleValue();
		}

		public static implicit operator JDouble(double s)
		{
			return new JDouble(s);
		}

		public JDouble(double value)
		{
			value_ = value;
		}

		public double DoubleValue()
		{
			return value_;
		}

		public static JDouble ValueOf(string s)
		{
			return new JDouble(ParseDouble(s));
		}

		public override string ToString()
		{
			return value_.ToString();
		}

		public static string ToString(double d)
		{
			return d.ToString();
		}

		public static double ParseDouble(string s)
		{
			return double.Parse(s);
		}

		public bool IsInfinite()
		{
			return double.IsInfinity(value_);
		}

		public static bool IsInfinite(double v)
		{
			return double.IsInfinity(v);
		}

		public bool IsNaN()
		{
			return double.IsNaN(value_);
		}

		public static bool IsNaN(double v)
		{
			return double.IsNaN(v);
		}

		public override bool Equals(object obj)
		{
			if (obj is JDouble)
			{
				return value_ == ((JDouble)obj).value_;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
