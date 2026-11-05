using java.util;

namespace kairo.unity.util
{
	public class SecureChar : SecureNumber
	{
		public static implicit operator char(SecureChar s)
		{
			return s.Get();
		}

		public static implicit operator SecureChar(char s)
		{
			return new SecureChar(s);
		}

		public SecureChar()
		{
		}

		public SecureChar(char value)
			: base(value)
		{
		}

		public SecureChar(SecureChar src)
		{
			SetLong(src.GetLong());
		}

		internal SecureChar Set(char value)
		{
			SetLong(value);
			return this;
		}

		internal SecureChar Set(SecureChar value)
		{
			SetLong(value.GetLong());
			return this;
		}

		public char Get()
		{
			return (char)GetLong();
		}

		public static SecureChar[] Array(int n)
		{
			SecureChar[] array = new SecureChar[n];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SecureChar();
			}
			return array;
		}

		public static SecureChar[][] Array(int n1, int n2)
		{
			SecureChar[][] array = JTool.MakeArray<SecureChar[][]>(new int[2] { n1, n2 });
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[i][j] = new SecureChar();
				}
			}
			return array;
		}

		public static void Arraycopy(SecureChar[] srcArray, int srcIndex, SecureChar[] dstArray, int dstIndex, int length)
		{
			for (int i = 0; i < dstArray.Length; i++)
			{
				if (dstArray[i] == null)
				{
					dstArray[i] = new SecureChar();
				}
			}
			for (int j = 0; j < length; j++)
			{
				dstArray[dstIndex + j].Set(srcArray[srcIndex + j]);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is SecureChar)
			{
				return Get() == ((SecureChar)obj).Get();
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
