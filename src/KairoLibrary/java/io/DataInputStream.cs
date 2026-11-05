using System;
using System.IO;
using System.Net;
using System.Text;

namespace java.io
{
	public class DataInputStream : InputStream, IDisposable
	{
		private BinaryReader br_;

		public DataInputStream(InputStream stream)
		{
			Wrapped = stream;
			br_ = new BinaryReader(stream);
		}

		public bool ReadBoolean()
		{
			return br_.ReadBoolean();
		}

		public sbyte ReadByte()
		{
			return br_.ReadSByte();
		}

		public byte ReadUnsignedByte()
		{
			return br_.ReadByte();
		}

		public char ReadChar()
		{
			return (char)IPAddress.NetworkToHostOrder(br_.ReadInt16());
		}

		public short ReadShort()
		{
			return IPAddress.NetworkToHostOrder(br_.ReadInt16());
		}

		public int ReadUnsignedShort()
		{
			return (ushort)IPAddress.NetworkToHostOrder(br_.ReadInt16());
		}

		public int ReadInt()
		{
			return IPAddress.NetworkToHostOrder(br_.ReadInt32());
		}

		public long ReadLong()
		{
			return IPAddress.NetworkToHostOrder(br_.ReadInt64());
		}

		public float ReadFloat()
		{
			byte[] array = new byte[4];
			br_.Read(array, 0, array.Length);
			if (BitConverter.IsLittleEndian)
			{
				Reverse(array);
			}
			return BitConverter.ToSingle(array, 0);
		}

		public double ReadDouble()
		{
			byte[] array = new byte[8];
			br_.Read(array, 0, array.Length);
			if (BitConverter.IsLittleEndian)
			{
				Reverse(array);
			}
			return BitConverter.ToDouble(array, 0);
		}

		public string ReadUTF()
		{
			byte[] array = new byte[ReadUnsignedShort()];
			br_.Read(array, 0, array.Length);
			return Encoding.UTF8.GetString(array);
		}

		public int SkipBytes(int n)
		{
			return (int)Skip(n);
		}

		public void ReadFully(byte[] b)
		{
			Read(b);
		}

		public void ReadFully(byte[] b, int off, int len)
		{
			Read(b, off, len);
		}

		public override void Close()
		{
			if (br_ != null)
			{
				br_.Close();
			}
			br_ = null;
			base.Close();
		}

		protected void Reverse(byte[] data)
		{
			for (int num = data.Length / 2 - 1; num >= 0; num--)
			{
				byte b = data[num];
				data[num] = data[data.Length - num - 1];
				data[data.Length - num - 1] = b;
			}
		}
	}
}
