namespace java.lang
{
	public class JCharacter
	{
		private char value_;

		public const char MAX_VALUE = '\uffff';

		public const char MIN_VALUE = '\0';

		public static implicit operator char(JCharacter s)
		{
			return s.CharValue();
		}

		public static implicit operator JCharacter(char s)
		{
			return new JCharacter(s);
		}

		public JCharacter(char value)
		{
			value_ = value;
		}

		public char CharValue()
		{
			return value_;
		}

		public override string ToString()
		{
			return value_.ToString();
		}

		public static bool IsDigit(char ch)
		{
			return char.IsDigit(ch);
		}

		public static bool IsLowerCase(char ch)
		{
			return char.IsLower(ch);
		}

		public static bool IsUpperCase(char ch)
		{
			return char.IsUpper(ch);
		}

		public static char ToLowerCase(char ch)
		{
			return char.ToLower(ch);
		}

		public static char ToUpperCase(char ch)
		{
			return char.ToUpper(ch);
		}

		public override bool Equals(object obj)
		{
			if (obj is JCharacter)
			{
				return value_ == ((JCharacter)obj).value_;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
