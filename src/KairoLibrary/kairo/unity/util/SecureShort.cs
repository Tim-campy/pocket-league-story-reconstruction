using java.util;

namespace kairo.unity.util
{
	public class SecureShort : SecureNumber
	{
		public static implicit operator short(SecureShort s)
		{
			return s.Get();
		}

		public static implicit operator SecureShort(short s)
		{
			return new SecureShort(s);
		}

		public static implicit operator SecureShort(sbyte s)
		{
			return new SecureShort((short)s);
		}

		public static implicit operator SecureShort(SecureByte s)
		{
			return new SecureShort((short)s.Get());
		}

		public static SecureShort operator ++(SecureShort z)
		{
			return new SecureShort((short)(z.Get() + 1));
		}

		public static SecureShort operator --(SecureShort z)
		{
			return new SecureShort((short)(z.Get() - 1));
		}

		public SecureShort()
		{
		}

		public SecureShort(short value)
			: base(value)
		{
		}

		public SecureShort(SecureShort src)
		{
			SetLong(src.GetLong());
		}

		internal SecureShort Set(short value)
		{
			SetLong(value);
			return this;
		}

		internal SecureShort Set(SecureShort value)
		{
			SetLong(value.GetLong());
			return this;
		}

		public short Get()
		{
			return (short)GetLong();
		}

		public static SecureShort[] Array(int n)
		{
			SecureShort[] array = new SecureShort[n];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SecureShort();
			}
			return array;
		}

		public static SecureShort[][] Array(int n1, int n2)
		{
			SecureShort[][] array = JTool.MakeArray<SecureShort[][]>(new int[2] { n1, n2 });
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[i][j] = new SecureShort();
				}
			}
			return array;
		}

		public static void Arraycopy(SecureShort[] srcArray, int srcIndex, SecureShort[] dstArray, int dstIndex, int length)
		{
			for (int i = 0; i < dstArray.Length; i++)
			{
				if (dstArray[i] == null)
				{
					dstArray[i] = new SecureShort();
				}
			}
			for (int j = 0; j < length; j++)
			{
				dstArray[dstIndex + j].Set(srcArray[srcIndex + j]);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is SecureShort)
			{
				return Get() == ((SecureShort)obj).Get();
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
