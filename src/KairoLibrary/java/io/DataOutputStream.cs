using System;
using System.IO;
using System.Net;
using System.Text;

namespace java.io
{
	public class DataOutputStream : OutputStream, IDisposable
	{
		private BinaryWriter bw_;

		public DataOutputStream(OutputStream stream)
		{
			Wrapped = stream;
			bw_ = new BinaryWriter(stream);
		}

		public void WriteBoolean(bool v)
		{
			bw_.Write(v);
		}

		public void WriteByte(int v)
		{
			bw_.Write((byte)v);
		}

		public void WriteChar(int v)
		{
			bw_.Write(IPAddress.HostToNetworkOrder((short)v));
		}

		public void WriteShort(int v)
		{
			bw_.Write(IPAddress.HostToNetworkOrder((short)v));
		}

		public void WriteUnsignedShort(int v)
		{
			bw_.Write(IPAddress.HostToNetworkOrder((short)v));
		}

		public void WriteInt(int v)
		{
			bw_.Write(IPAddress.HostToNetworkOrder(v));
		}

		public void WriteLong(long v)
		{
			bw_.Write(IPAddress.HostToNetworkOrder(v));
		}

		public void WriteFloat(float v)
		{
			byte[] bytes = BitConverter.GetBytes(v);
			if (BitConverter.IsLittleEndian)
			{
				Reverse(bytes);
			}
			bw_.Write(bytes, 0, bytes.Length);
		}

		public void WriteDouble(double v)
		{
			byte[] bytes = BitConverter.GetBytes(v);
			if (BitConverter.IsLittleEndian)
			{
				Reverse(bytes);
			}
			bw_.Write(bytes, 0, bytes.Length);
		}

		public void WriteUTF(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			WriteUnsignedShort(bytes.Length);
			bw_.Write(bytes, 0, bytes.Length);
		}

		public override void Close()
		{
			if (bw_ != null)
			{
				bw_.Close();
			}
			bw_ = null;
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
