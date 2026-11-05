using System;

namespace kairo.unity.util
{
	public static class EncodeUtil
	{
		public static byte[] DoubleToByteAry(double data, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 8);
			}
			return array;
		}

		public static byte[] DoubleToByteAry(double data, byte[] buffer, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 8);
			}
			for (int i = 0; i < array.Length; i++)
			{
				buffer[i] = array[i];
			}
			return buffer;
		}

		public static byte[] FloatToByteAry(float data, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 4);
			}
			return array;
		}

		public static byte[] FloatToByteAry(float data, byte[] buffer, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 4);
			}
			for (int i = 0; i < array.Length; i++)
			{
				buffer[i] = array[i];
			}
			return buffer;
		}

		public static byte[] LongToByteAry(long data, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 8);
			}
			return array;
		}

		public static byte[] LongToByteAry(ulong data, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 8);
			}
			return array;
		}

		public static byte[] LongToByteAry(long data, byte[] buffer, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 8);
			}
			for (int i = 0; i < array.Length; i++)
			{
				buffer[i] = array[i];
			}
			return buffer;
		}

		public static byte[] LongToByteAry(ulong data, byte[] buffer, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 8);
			}
			for (int i = 0; i < array.Length; i++)
			{
				buffer[i] = array[i];
			}
			return buffer;
		}

		public static byte[] IntToByteAry(int data, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 4);
			}
			return array;
		}

		public static byte[] IntToByteAry(uint data, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 4);
			}
			return array;
		}

		public static byte[] IntToByteAry(int data, byte[] buffer, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 4);
			}
			for (int i = 0; i < array.Length; i++)
			{
				buffer[i] = array[i];
			}
			return buffer;
		}

		public static byte[] IntToByteAry(uint data, byte[] buffer, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 4);
			}
			for (int i = 0; i < array.Length; i++)
			{
				buffer[i] = array[i];
			}
			return buffer;
		}

		public static byte[] ShortToByteAry(short data, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 2);
			}
			return array;
		}

		public static byte[] ShortToByteAry(ushort data, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 2);
			}
			return array;
		}

		public static byte[] ShortToByteAry(short data, byte[] buffer, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 2);
			}
			for (int i = 0; i < array.Length; i++)
			{
				buffer[i] = array[i];
			}
			return buffer;
		}

		public static byte[] ShortToByteAry(ushort data, byte[] buffer, bool endian = false)
		{
			byte[] array = BitConverter.GetBytes(data);
			if (endian)
			{
				array = ConvertEndian(array, 2);
			}
			for (int i = 0; i < array.Length; i++)
			{
				buffer[i] = array[i];
			}
			return buffer;
		}

		public static double ByteAryToDouble(byte[] data, bool endian = false)
		{
			if (endian)
			{
				data = ConvertEndian(data, 8);
			}
			return BitConverter.ToDouble(data, 0);
		}

		public static float ByteAryToFloat(byte[] data, bool endian = false)
		{
			if (endian)
			{
				data = ConvertEndian(data, 4);
			}
			return BitConverter.ToSingle(data, 0);
		}

		public static long ByteAryToLong(byte[] data, bool endian = false)
		{
			if (endian)
			{
				data = ConvertEndian(data, 8);
			}
			return BitConverter.ToInt64(data, 0);
		}

		public static ulong ByteAryToULong(byte[] data, bool endian = false)
		{
			if (endian)
			{
				data = ConvertEndian(data, 8);
			}
			return BitConverter.ToUInt64(data, 0);
		}

		public static int ByteAryToInt(byte[] data, bool endian = false)
		{
			if (endian)
			{
				data = ConvertEndian(data, 4);
			}
			return BitConverter.ToInt32(data, 0);
		}

		public static uint ByteAryToUInt(byte[] data, bool endian = false)
		{
			if (endian)
			{
				data = ConvertEndian(data, 4);
			}
			return BitConverter.ToUInt32(data, 0);
		}

		public static short ByteAryToShort(byte[] data, bool endian = false)
		{
			if (endian)
			{
				data = ConvertEndian(data, 2);
			}
			return BitConverter.ToInt16(data, 0);
		}

		public static ushort ByteAryToUShort(byte[] data, bool endian = false)
		{
			if (endian)
			{
				data = ConvertEndian(data, 2);
			}
			return BitConverter.ToUInt16(data, 0);
		}

		private static byte[] ConvertEndian(byte[] data, int length)
		{
			if (BitConverter.IsLittleEndian)
			{
				byte[] array = data;
				if (array.Length != length)
				{
					array = new byte[length];
					Array.Copy(data, array, length);
				}
				for (int num = array.Length / 2 - 1; num >= 0; num--)
				{
					byte b = array[num];
					array[num] = array[array.Length - num - 1];
					array[array.Length - num - 1] = b;
				}
				data = array;
			}
			return data;
		}
	}
}
