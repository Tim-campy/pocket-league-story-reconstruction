using java.util;

namespace kairo.unity.util
{
	public class SecureLong : SecureNumber
	{
		public static implicit operator long(SecureLong s)
		{
			return s.Get();
		}

		public static implicit operator SecureLong(long s)
		{
			return new SecureLong(s);
		}

		public static implicit operator SecureLong(int s)
		{
			return new SecureLong((long)s);
		}

		public static implicit operator SecureLong(short s)
		{
			return new SecureLong((long)s);
		}

		public static implicit operator SecureLong(sbyte s)
		{
			return new SecureLong((long)s);
		}

		public static implicit operator SecureLong(SecureInt s)
		{
			return new SecureLong((long)s.Get());
		}

		public static implicit operator SecureLong(SecureShort s)
		{
			return new SecureLong((long)s.Get());
		}

		public static implicit operator SecureLong(SecureByte s)
		{
			return new SecureLong((long)s.Get());
		}

		public static SecureLong operator ++(SecureLong z)
		{
			return new SecureLong(z.Get() + 1);
		}

		public static SecureLong operator --(SecureLong z)
		{
			return new SecureLong(z.Get() - 1);
		}

		public SecureLong()
		{
		}

		public SecureLong(long value)
			: base(value)
		{
		}

		public SecureLong(SecureLong src)
		{
			SetLong(src.GetLong());
		}

		internal SecureLong Set(long value)
		{
			SetLong(value);
			return this;
		}

		internal SecureLong Set(SecureLong value)
		{
			SetLong(value.GetLong());
			return this;
		}

		public long Get()
		{
			return GetLong();
		}

		public static SecureLong[] Array(int n)
		{
			SecureLong[] array = new SecureLong[n];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SecureLong();
			}
			return array;
		}

		public static SecureLong[][] Array(int n1, int n2)
		{
			SecureLong[][] array = JTool.MakeArray<SecureLong[][]>(new int[2] { n1, n2 });
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[i][j] = new SecureLong();
				}
			}
			return array;
		}

		public static void Arraycopy(SecureLong[] srcArray, int srcIndex, SecureLong[] dstArray, int dstIndex, int length)
		{
			for (int i = 0; i < dstArray.Length; i++)
			{
				if (dstArray[i] == null)
				{
					dstArray[i] = new SecureLong();
				}
			}
			for (int j = 0; j < length; j++)
			{
				dstArray[dstIndex + j].Set(srcArray[srcIndex + j]);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is SecureLong)
			{
				return Get() == ((SecureLong)obj).Get();
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
