using java.util;

namespace kairo.unity.util
{
	public class SecureByte : SecureNumber
	{
		public static implicit operator sbyte(SecureByte s)
		{
			return s.Get();
		}

		public static implicit operator SecureByte(sbyte s)
		{
			return new SecureByte(s);
		}

		public static SecureByte operator ++(SecureByte z)
		{
			return new SecureByte((sbyte)(z.Get() + 1));
		}

		public static SecureByte operator --(SecureByte z)
		{
			return new SecureByte((sbyte)(z.Get() - 1));
		}

		public SecureByte()
		{
		}

		public SecureByte(sbyte value)
			: base(value)
		{
		}

		public SecureByte(SecureByte src)
		{
			SetLong(src.GetLong());
		}

		internal SecureByte Set(sbyte value)
		{
			SetLong(value);
			return this;
		}

		internal SecureByte Set(SecureByte value)
		{
			SetLong(value.GetLong());
			return this;
		}

		public sbyte Get()
		{
			return (sbyte)GetLong();
		}

		public static SecureByte[] Array(int n)
		{
			SecureByte[] array = new SecureByte[n];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SecureByte();
			}
			return array;
		}

		public static SecureByte[][] Array(int n1, int n2)
		{
			SecureByte[][] array = JTool.MakeArray<SecureByte[][]>(new int[2] { n1, n2 });
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[i][j] = new SecureByte();
				}
			}
			return array;
		}

		public static void Arraycopy(SecureByte[] srcArray, int srcIndex, SecureByte[] dstArray, int dstIndex, int length)
		{
			for (int i = 0; i < dstArray.Length; i++)
			{
				if (dstArray[i] == null)
				{
					dstArray[i] = new SecureByte();
				}
			}
			for (int j = 0; j < length; j++)
			{
				dstArray[dstIndex + j].Set(srcArray[srcIndex + j]);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is SecureByte)
			{
				return Get() == ((SecureByte)obj).Get();
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return value_.GetHashCode();
		}
	}
}
