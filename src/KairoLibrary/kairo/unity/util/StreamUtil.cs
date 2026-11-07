using System.Collections;
using System;
using System.Text;
using java.io;
using kairo.unity.m3d4.util3d;

namespace kairo.unity.util
{
	public class StreamUtil
	{
		private StreamUtil()
		{
		}

		public static byte[] Read(InputStream stream, int size)
		{
			if (size == -1)
			{
				return null;
			}
			ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
			byte[] b = new byte[size];
			int num = 0;
			int num2 = 0;
			while (num2 < size)
			{
				num = stream.Read(b, 0, size - num2);
				if (num < 0)
				{
					break;
				}
				num2 += num;
				if (num > 0)
				{
					byteArrayOutputStream.Write(b, 0, num);
				}
			}
			if (num2 < size)
			{
				throw new EOFException("request[" + size + "] read[" + num2 + "]");
			}
			b = null;
			return byteArrayOutputStream.ToByteArray();
		}

		public static byte[] Read(InputStream stream, byte[] buf, int size)
		{
			if (size == -1)
			{
				return null;
			}
			int num = 0;
			int i;
			for (i = 0; i < size; i += num)
			{
				num = stream.Read(buf, 0, size - i);
				if (num < 0)
				{
					break;
				}
			}
			if (i < size)
			{
				throw new EOFException("request[" + size + "] read[" + i + "]");
			}
			return buf;
		}

		public static byte[] Read(InputStream stream, bool size)
		{
			if (size)
			{
				return Read(stream, ReadInt(stream));
			}
			return Read(stream);
		}

		public static byte[] Read(InputStream stream)
		{
			ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
			byte[] array = new byte[1024];
			while (true)
			{
				int num = stream.Read(array, 0, array.Length);
				if (num < 0)
				{
					break;
				}
				if (num > 0)
				{
					byteArrayOutputStream.Write(array, 0, num);
				}
			}
			return byteArrayOutputStream.ToByteArray();
		}

		public static sbyte ReadByte(InputStream stream)
		{
			return (sbyte)Read(stream, 1)[0];
		}

		public static byte ReadUnsignedByte(InputStream stream)
		{
			return Read(stream, 1)[0];
		}

		public static short ReadShort(InputStream stream)
		{
			return EncodeUtil.ByteAryToShort(Read(stream, 2), true);
		}

		public static short ReadShortLE(InputStream stream)
		{
			return EncodeUtil.ByteAryToShort(Read(stream, 2));
		}

		public static ushort ReadUnsignedShort(InputStream stream)
		{
			return EncodeUtil.ByteAryToUShort(Read(stream, 2), true);
		}

		public static int ReadInt(InputStream stream)
		{
			return EncodeUtil.ByteAryToInt(Read(stream, 4), true);
		}

		public static int ReadIntLE(InputStream stream)
		{
			return EncodeUtil.ByteAryToInt(Read(stream, 4));
		}

		public static uint ReadUnsignedInt(InputStream stream)
		{
			return EncodeUtil.ByteAryToUInt(Read(stream, 4), true);
		}

		public static long ReadLong(InputStream stream)
		{
			return EncodeUtil.ByteAryToLong(Read(stream, 8), true);
		}

		public static long ReadLongLE(InputStream stream)
		{
			return EncodeUtil.ByteAryToLong(Read(stream, 8));
		}

		public static ulong ReadUnsignedLong(InputStream stream)
		{
			return EncodeUtil.ByteAryToULong(Read(stream, 8), true);
		}

		public static bool ReadBoolean(InputStream stream)
		{
			return Read(stream, 1)[0] == 1;
		}

		public static float ReadFloat(InputStream stream)
		{
			return EncodeUtil.ByteAryToFloat(Read(stream, 4), true);
		}

		public static float ReadFloat_iOS_OLD(InputStream stream)
		{
			return (float)EncodeUtil.ByteAryToInt(Read(stream, 4), true) / 10000f;
		}

		public static double ReadDouble(InputStream stream)
		{
			return EncodeUtil.ByteAryToDouble(Read(stream, 8), true);
		}

		public static Vector3D ReadVector3D(InputStream stream)
		{
			Vector3D vector3D = new Vector3D();
			vector3D.SetX(ReadFloat(stream));
			vector3D.SetY(ReadFloat(stream));
			vector3D.SetZ(ReadFloat(stream));
			return vector3D;
		}

		public static Vector3D ReadVector3D_iOS_OLD(InputStream stream)
		{
			Vector3D vector3D = new Vector3D();
			vector3D.SetX(ReadFloat_iOS_OLD(stream));
			vector3D.SetY(ReadFloat_iOS_OLD(stream));
			vector3D.SetZ(ReadFloat_iOS_OLD(stream));
			return vector3D;
		}

		public static string ReadString(InputStream stream)
		{
			byte[] array = Read(stream, true);
			if (array == null)
			{
				return null;
			}
			return StringUtil.GetString(array);
		}

		public static T ReadObject<T>(InputStream stream)
		{
			if (!ReadBoolean(stream))
			{
				return default(T);
			}
			ISerializable obj = (ISerializable)Activator.CreateInstance(typeof(T));
			obj.Deserialize(stream);
			return (T)obj;
		}

		public static ISerializable ReadObject(InputStream stream, ISerializable obj)
		{
			if (stream.Available() <= 0)
			{
				return obj;
			}
			if (ReadBoolean(stream))
			{
				obj.Deserialize(stream);
				return obj;
			}
			return null;
		}

		public static void Write(OutputStream os, byte[] data)
		{
			Write(os, data, false);
		}

		public static void Write(OutputStream os, byte[] data, bool size)
		{
			if (size)
			{
				WriteInt(os, (data == null) ? (-1) : data.Length);
				if (data == null)
				{
					return;
				}
			}
			if (data.Length != 0)
			{
				os.Write(data);
			}
		}

		public static void WriteByte(OutputStream os, sbyte data)
		{
			os.Write(data);
		}

		public static void WriteByte(OutputStream os, byte data)
		{
			os.Write(data);
		}

		public static void WriteShort(OutputStream os, short data)
		{
			byte[] array = EncodeUtil.ShortToByteAry(data, true);
			for (int i = 0; i < array.Length; i++)
			{
				sbyte b = (sbyte)array[i];
				os.Write(b);
			}
		}

		public static void WriteShort(OutputStream os, ushort data)
		{
			byte[] array = EncodeUtil.ShortToByteAry(data, true);
			for (int i = 0; i < array.Length; i++)
			{
				sbyte b = (sbyte)array[i];
				os.Write(b);
			}
		}

		public static void WriteInt(OutputStream os, int data)
		{
			byte[] array = EncodeUtil.IntToByteAry(data, true);
			for (int i = 0; i < array.Length; i++)
			{
				sbyte b = (sbyte)array[i];
				os.Write(b);
			}
		}

		public static void WriteInt(OutputStream os, uint data)
		{
			byte[] array = EncodeUtil.IntToByteAry(data, true);
			for (int i = 0; i < array.Length; i++)
			{
				sbyte b = (sbyte)array[i];
				os.Write(b);
			}
		}

		public static void WriteLong(OutputStream os, long data)
		{
			byte[] array = EncodeUtil.LongToByteAry(data, true);
			for (int i = 0; i < array.Length; i++)
			{
				sbyte b = (sbyte)array[i];
				os.Write(b);
			}
		}

		public static void WriteLong(OutputStream os, ulong data)
		{
			byte[] array = EncodeUtil.LongToByteAry(data, true);
			for (int i = 0; i < array.Length; i++)
			{
				sbyte b = (sbyte)array[i];
				os.Write(b);
			}
		}

		public static void WriteBoolean(OutputStream os, bool data)
		{
			os.Write((sbyte)(data ? 1 : 0));
		}

		public static void WriteFloat(OutputStream os, float data)
		{
			byte[] array = EncodeUtil.FloatToByteAry(data, true);
			for (int i = 0; i < array.Length; i++)
			{
				sbyte b = (sbyte)array[i];
				os.Write(b);
			}
		}

		public static void WriteDouble(OutputStream os, double data)
		{
			byte[] array = EncodeUtil.DoubleToByteAry(data, true);
			for (int i = 0; i < array.Length; i++)
			{
				sbyte b = (sbyte)array[i];
				os.Write(b);
			}
		}

		public static void WriteVector3D(OutputStream os, Vector3D value)
		{
			WriteFloat(os, value.GetX());
			WriteFloat(os, value.GetY());
			WriteFloat(os, value.GetZ());
		}

		public static void WriteString(OutputStream os, SecureString data)
		{
			Write(os, (data == null) ? null : Encoding.UTF8.GetBytes(data.Get()), true);
		}

		public static void WriteString(OutputStream os, string data)
		{
			Write(os, (data == null) ? null : Encoding.UTF8.GetBytes(data), true);
		}

		public static void WriteObject(OutputStream os, ISerializable obj)
		{
			WriteBoolean(os, obj != null);
			obj?.Serialize(os);
		}

		public static void WriteArray(OutputStream os, Array array, int arrayLengthBytes = 4)
		{
			long num = (((object)(array) == null) ? ((-1)) : array.GetLength(0));
			long num2 = 2147483647L;
			if ((arrayLengthBytes == 1)) { num2 = 127L;
				 } else if ((arrayLengthBytes == 2)) { num2 = 32767L;
				 } else if ((arrayLengthBytes == 8)) { num2 = long.MaxValue;
				 }
			if (num2 <= num)
			{
				throw new ArgumentException("StreamUtil.WriteArray : out of range " + num + " >= " + num2);
			}
			if ((arrayLengthBytes == 1)) { WriteByte(os, (sbyte)num);
				 } else if ((arrayLengthBytes == 2)) { WriteShort(os, (short)num);
				 } else if ((arrayLengthBytes == 8)) { WriteLong(os, num);
				 } else { WriteInt(os, (int)num);
				 }
			if (num == -1)
			{
				return;
			}
			Type elementType = array.GetType().GetElementType();
			if (elementType.IsArray)
			{
				foreach (object item in array)
				{
					WriteArray(os, (Array)item, arrayLengthBytes);
				}

			}
			else if (elementType == typeof(bool))
			{
				foreach (object item2 in array)
				{
					WriteBoolean(os, (bool)item2);
				}

			}
			else if (elementType == typeof(byte))
			{
				foreach (object item3 in array)
				{
					WriteByte(os, (byte)item3);
				}

			}
			else if (elementType == typeof(sbyte))
			{
				foreach (object item4 in array)
				{
					WriteByte(os, (sbyte)item4);
				}

			}
			else if (elementType == typeof(short))
			{
				foreach (object item5 in array)
				{
					WriteShort(os, (short)item5);
				}

			}
			else if (elementType == typeof(ushort))
			{
				foreach (object item6 in array)
				{
					WriteShort(os, (ushort)item6);
				}

			}
			else if (elementType == typeof(int))
			{
				foreach (object item7 in array)
				{
					WriteInt(os, (int)item7);
				}

			}
			else if (elementType == typeof(uint))
			{
				foreach (object item8 in array)
				{
					WriteInt(os, (uint)item8);
				}

			}
			else if (elementType == typeof(long))
			{
				foreach (object item9 in array)
				{
					WriteLong(os, (long)item9);
				}

			}
			else if (elementType == typeof(ulong))
			{
				foreach (object item10 in array)
				{
					WriteLong(os, (ulong)item10);
				}

			}
			else if (elementType == typeof(float))
			{
				foreach (object item11 in array)
				{
					WriteFloat(os, (float)item11);
				}

			}
			else if (elementType == typeof(double))
			{
				foreach (object item12 in array)
				{
					WriteDouble(os, (double)item12);
				}

			}
			else if (elementType == typeof(string))
			{
				foreach (object item13 in array)
				{
					WriteString(os, (string)item13);
				}

			}
			else if (elementType == typeof(SecureByte))
			{
				foreach (object item14 in array)
				{
					WriteByte(os, (SecureByte)item14);
				}

			}
			else if (elementType == typeof(SecureShort))
			{
				foreach (object item15 in array)
				{
					WriteShort(os, (SecureShort)item15);
				}

			}
			else if (elementType == typeof(SecureInt))
			{
				foreach (object item16 in array)
				{
					WriteInt(os, (SecureInt)item16);
				}

			}
			else if (elementType == typeof(SecureLong))
			{
				foreach (object item17 in array)
				{
					WriteLong(os, (SecureLong)item17);
				}

			}
			else if (elementType == typeof(SecureString))
			{
				foreach (object item18 in array)
				{
					WriteString(os, (SecureString)item18);
				}

			}
			else if (elementType.GetInterface(typeof(ISerializable).Name) != null)
			{
				foreach (ISerializable item19 in array)
				{
					WriteBoolean(os, item19 != null);
					item19?.Serialize(os);
				}

			}
			else { throw new ArgumentException("StreamUtil.WriteArray : unsupported by " + elementType.FullName); }
		}

		public static T ReadArray<T>(InputStream stream, int arrayLengthBytes = 4)
		{
			return (T)ReadArray(stream, null, typeof(T));
		}

		public static Array ReadArray(InputStream stream, Array array, int arrayLengthBytes = 4)
		{
			return (Array)ReadArray(stream, array, array.GetType());
		}

		private static object ReadArray(InputStream stream, Array dst, Type type, int arrayLengthBytes = 4)
		{
			if (stream.Available() <= 0)
			{
				return dst;
			}
			long num = 0L;
			if (arrayLengthBytes == 1)
			{
				num = ReadByte(stream);
			}
			else if (arrayLengthBytes == 2)
			{
				num = ReadShort(stream);
			}
			else if (arrayLengthBytes == 8)
			{
				num = ReadLong(stream);
			}
			else
			{
				num = ReadInt(stream);
			}
			if (num == -1)
			{
				return null;
			}
			type = type.GetElementType();
			Array array = Array.CreateInstance(type, num);
			if (type.IsArray)
			{
				for (int i = 0; i < num; i++)
				{
					array.SetValue(ReadArray(stream, null, type), i);
				}
			}
			else if (type == typeof(bool))
			{
				for (int j = 0; j < num; j++)
				{
					array.SetValue(ReadBoolean(stream), j);
				}
			}
			else if (type == typeof(byte))
			{
				for (int k = 0; k < num; k++)
				{
					array.SetValue(ReadUnsignedByte(stream), k);
				}
			}
			else if (type == typeof(sbyte))
			{
				for (int l = 0; l < num; l++)
				{
					array.SetValue(ReadByte(stream), l);
				}
			}
			else if (type == typeof(short))
			{
				for (int m = 0; m < num; m++)
				{
					array.SetValue(ReadShort(stream), m);
				}
			}
			else if (type == typeof(ushort))
			{
				for (int n = 0; n < num; n++)
				{
					array.SetValue(ReadUnsignedShort(stream), n);
				}
			}
			else if (type == typeof(int))
			{
				for (int num2 = 0; num2 < num; num2++)
				{
					array.SetValue(ReadInt(stream), num2);
				}
			}
			else if (type == typeof(uint))
			{
				for (int num3 = 0; num3 < num; num3++)
				{
					array.SetValue(ReadUnsignedInt(stream), num3);
				}
			}
			else if (type == typeof(long))
			{
				for (int num4 = 0; num4 < num; num4++)
				{
					array.SetValue(ReadLong(stream), num4);
				}
			}
			else if (type == typeof(ulong))
			{
				for (int num5 = 0; num5 < num; num5++)
				{
					array.SetValue(ReadUnsignedLong(stream), num5);
				}
			}
			else if (type == typeof(float))
			{
				for (int num6 = 0; num6 < num; num6++)
				{
					array.SetValue(ReadFloat(stream), num6);
				}
			}
			else if (type == typeof(double))
			{
				for (int num7 = 0; num7 < num; num7++)
				{
					array.SetValue(ReadDouble(stream), num7);
				}
			}
			else if (type == typeof(string))
			{
				for (int num8 = 0; num8 < num; num8++)
				{
					array.SetValue(ReadString(stream), num8);
				}
			}
			else if (type == typeof(SecureByte))
			{
				for (int num9 = 0; num9 < num; num9++)
				{
					array.SetValue(new SecureByte(ReadByte(stream)), num9);
				}
			}
			else if (type == typeof(SecureShort))
			{
				for (int num10 = 0; num10 < num; num10++)
				{
					array.SetValue(new SecureShort(ReadShort(stream)), num10);
				}
			}
			else if (type == typeof(SecureInt))
			{
				for (int num11 = 0; num11 < num; num11++)
				{
					array.SetValue(new SecureInt(ReadInt(stream)), num11);
				}
			}
			else if (type == typeof(SecureLong))
			{
				for (int num12 = 0; num12 < num; num12++)
				{
					array.SetValue(new SecureLong(ReadLong(stream)), num12);
				}
			}
			else if (type == typeof(SecureString))
			{
				for (int num13 = 0; num13 < num; num13++)
				{
					array.SetValue(SecureString.ValueOf(ReadString(stream)), num13);
				}
			}
			else if (type.GetInterface(typeof(ISerializable).Name) != null)
			{
				for (int num14 = 0; num14 < num; num14++)
				{
					ISerializable serializable = null;
					if (ReadBoolean(stream))
					{
						serializable = (ISerializable)Activator.CreateInstance(type);
						serializable.Deserialize(stream);
					}
					array.SetValue(serializable, num14);
				}
			}
			else
			{
				throw new ArgumentException("StreamUtil.ReadArray : unsupported by " + type.FullName);
			}
			if (dst != null)
			{
				long num15 = dst.GetLength(0);
				if (num < num15)
				{
					num15 = num;
				}
				for (int num16 = 0; num16 < num15; num16++)
				{
					dst.SetValue(array.GetValue(num16), num16);
				}
				array = dst;
			}
			return array;
		}

		public static void WriteDoubleIntArray(OutputStream os, int[][] data)
		{
			if (data != null)
			{
				WriteByte(os, (byte)0);
				WriteInt(os, data.Length);
				int num = 0;
				while (num < data.Length)
				{
					int[] array = data[num];
					WriteInt(os, array.Length);
					int num2 = 0;
					while (num2 < array.Length)
					{
						WriteInt(os, array[num2]);
						num2++;
					}
					num++;
				}
			}
			else
			{
				WriteByte(os, (byte)1);
			}
		}
	}
}
