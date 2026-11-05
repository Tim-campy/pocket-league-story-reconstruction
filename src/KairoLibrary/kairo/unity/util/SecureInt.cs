using java.util;

namespace kairo.unity.util
{
	public class SecureInt : SecureNumber
	{
		public static implicit operator int(SecureInt s)
		{
			return s.Get();
		}

		public static implicit operator SecureInt(int s)
		{
			return new SecureInt(s);
		}

		public static implicit operator SecureInt(short s)
		{
			return new SecureInt((int)s);
		}

		public static implicit operator SecureInt(sbyte s)
		{
			return new SecureInt((int)s);
		}

		public static implicit operator SecureInt(SecureShort s)
		{
			return new SecureInt((int)s.Get());
		}

		public static implicit operator SecureInt(SecureByte s)
		{
			return new SecureInt((int)s.Get());
		}

		public static SecureInt operator ++(SecureInt z)
		{
			return new SecureInt(z.Get() + 1);
		}

		public static SecureInt operator --(SecureInt z)
		{
			return new SecureInt(z.Get() - 1);
		}

		public SecureInt()
		{
		}

		public SecureInt(int value)
			: base(value)
		{
		}

		public SecureInt(SecureInt src)
		{
			SetLong(src.GetLong());
		}

		internal SecureInt Set(int value)
		{
			SetLong(value);
			return this;
		}

		internal SecureInt Set(SecureInt value)
		{
			SetLong(value.GetLong());
			return this;
		}

		public int Get()
		{
			return (int)GetLong();
		}

		public static SecureInt[] Array(int n)
		{
			SecureInt[] array = new SecureInt[n];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SecureInt();
			}
			return array;
		}

		public static SecureInt[][] Array(int n1, int n2)
		{
			SecureInt[][] array = JTool.MakeArray<SecureInt[][]>(new int[2] { n1, n2 });
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[i][j] = new SecureInt();
				}
			}
			return array;
		}

		public static void Arraycopy(SecureInt[] srcArray, int srcIndex, SecureInt[] dstArray, int dstIndex, int length)
		{
			for (int i = 0; i < dstArray.Length; i++)
			{
				if (dstArray[i] == null)
				{
					dstArray[i] = new SecureInt();
				}
			}
			for (int j = 0; j < length; j++)
			{
				dstArray[dstIndex + j].Set(srcArray[srcIndex + j]);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is SecureInt)
			{
				return Get() == ((SecureInt)obj).Get();
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
