namespace java.lang
{
	public class JFloat
	{
		private float value_;

		public const float MAX_VALUE = float.MaxValue;

		public const float MIN_VALUE = float.MinValue;

		public const float NaN = float.NaN;

		public const float NEGATIVE_INFINITY = float.NegativeInfinity;

		public const float POSITIVE_INFINITY = float.PositiveInfinity;

		public static implicit operator float(JFloat s)
		{
			return s.FloatValue();
		}

		public static implicit operator JFloat(float s)
		{
			return new JFloat(s);
		}

		public JFloat(float value)
		{
			value_ = value;
		}

		public JFloat(double value)
		{
			value_ = (float)value;
		}

		public float FloatValue()
		{
			return value_;
		}

		public static JFloat ValueOf(string s)
		{
			return new JFloat(ParseFloat(s));
		}

		public override string ToString()
		{
			return value_.ToString();
		}

		public static string ToString(float f)
		{
			return f.ToString();
		}

		public static float ParseFloat(string s)
		{
			return float.Parse(s);
		}

		public bool IsInfinite()
		{
			return float.IsInfinity(value_);
		}

		public static bool IsInfinite(float v)
		{
			return float.IsInfinity(v);
		}

		public bool IsNaN()
		{
			return float.IsNaN(value_);
		}

		public static bool IsNaN(float v)
		{
			return float.IsNaN(v);
		}

		public override bool Equals(object obj)
		{
			if (obj is JFloat)
			{
				return value_ == ((JFloat)obj).value_;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
