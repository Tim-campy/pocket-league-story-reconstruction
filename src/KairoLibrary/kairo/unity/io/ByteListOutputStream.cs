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
			for (int i = 0; i < list_.Length; i++)
			{
				if (num >= array.Length)
				{
					break;
				}
				byte[] array2 = list_[i];
				int num2 = array.Length - num;
				if (num2 > array2.Length)
				{
					num2 = array2.Length;
				}
				JSystem.Arraycopy(array2, 0, array, num, num2);
				num += num2;
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
			for (int i = num; i < list_.Length; i++)
			{
				if (num3 >= length)
				{
					break;
				}
				byte[] array = list_[i];
				int num4 = length - num3;
				if (num4 > array.Length - num2)
				{
					num4 = array.Length - num2;
				}
				JSystem.Arraycopy(array, num2, dst, num3, num4);
				num3 += num4;
				num2 = 0;
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
