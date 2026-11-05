using java.io;
using java.lang;
using java.util;

namespace kairo.unity.io
{
	public class ByteListOutputStream : OutputStream
	{
		protected const int BUFFER_SIZE = 256;

		protected byte[][] list_;

		protected int count_;

		protected int clst_;

		protected int cidx_;

		public ByteListOutputStream()
		{
			_init(1);
		}

		public ByteListOutputStream(int size)
		{
			int listNum = (size + 256 - 1) / 256;
			_init(listNum);
		}

		protected void _init(int listNum)
		{
			list_ = JTool.MakeArray<byte[][]>(new int[2] { listNum, 256 });
			count_ = 0;
			clst_ = (cidx_ = 0);
		}

		public override void Close()
		{
			for (int num = list_.Length - 1; num >= 1; num--)
			{
				list_[num] = null;
			}
			list_ = null;
			count_ = 0;
			clst_ = (cidx_ = 0);
		}

		public void Reset()
		{
			for (int num = list_.Length - 1; num >= 1; num--)
			{
				list_[num] = null;
			}
			list_ = new byte[1][] { list_[0] };
			count_ = 0;
			clst_ = (cidx_ = 0);
		}

		public int Size()
		{
			return count_;
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[count_];
			int num = 0;
			int num2 = 0;
			while (num2 < list_.Length && num < array.Length)
			{
				byte[] array2 = list_[num2];
				int num3 = array.Length - num;
				if (num3 > array2.Length)
				{
					num3 = array2.Length;
				}
				JSystem.Arraycopy(array2, 0, array, num, num3);
				num += num3;
				num2++;
			}
			return array;
		}

		public byte[] ToByteArray(int index, int length)
		{
			return ToByteArray(index, length, null);
		}

		public byte[] ToByteArray(int index, int length, byte[] dst)
		{
			if (dst == null)
			{
				dst = new byte[length];
			}
			int num = index / 256;
			int num2 = index % 256;
			int num3 = 0;
			int num4 = num;
			while (num4 < list_.Length && num3 < length)
			{
				byte[] array = list_[num4];
				int num5 = length - num3;
				if (num5 > array.Length - num2)
				{
					num5 = array.Length - num2;
				}
				JSystem.Arraycopy(array, num2, dst, num3, num5);
				num3 += num5;
				num2 = 0;
				num4++;
			}
			return dst;
		}

		public override void Write(byte[] b, int off, int len)
		{
			int num = off;
			while (len > 0)
			{
				if (cidx_ >= 256)
				{
					clst_++;
					cidx_ = 0;
					if (clst_ >= list_.Length)
					{
						_expand();
					}
				}
				int num2 = 256 - cidx_;
				if (num2 > len)
				{
					num2 = len;
				}
				JSystem.Arraycopy(b, num, list_[clst_], cidx_, num2);
				cidx_ += num2;
				count_ += num2;
				len -= num2;
				num += num2;
			}
		}

		public override void Write(byte[] b)
		{
			Write(b, 0, b.Length);
		}

		public override void Write(int b)
		{
			if (cidx_ >= 256)
			{
				clst_++;
				cidx_ = 0;
				if (clst_ >= list_.Length)
				{
					_expand();
				}
			}
			list_[clst_][cidx_] = (byte)(b & 0xFF);
			cidx_++;
			count_++;
		}

		public override void Flush()
		{
			base.Flush();
		}

		protected void _expand()
		{
			byte[][] array = new byte[list_.Length + 1][];
			JSystem.Arraycopy(list_, 0, array, 0, list_.Length);
			array[array.Length - 1] = new byte[256];
			list_ = array;
		}
	}
}
