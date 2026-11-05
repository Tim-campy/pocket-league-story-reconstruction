namespace java.lang
{
	public class JBoolean
	{
		private bool value_;

		public JBoolean(bool value)
		{
			value_ = value;
		}

		public bool BooleanValue()
		{
			return value_;
		}

		public override string ToString()
		{
			return value_.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is JBoolean)
			{
				return value_ == ((JBoolean)obj).value_;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
