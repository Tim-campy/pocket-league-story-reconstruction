using java.lang;
using java.util;

namespace kairo.unity.util
{
	public sealed class SecureByteArray : SecureNumber
	{
		private new byte[] original_;

		private new byte[] value_;

		private byte[] buffer_;

		public int length;

		public byte this[int index] => Generate(value_[index]);

		public static implicit operator byte[](SecureByteArray s)
		{
			return s.ToArray();
		}

		public static implicit operator SecureByteArray(byte[] s)
		{
			return new SecureByteArray(s);
		}

		public SecureByteArray()
		{
			Set(new byte[0]);
		}

		public SecureByteArray(byte[] ary)
		{
			Set(ary);
		}

		public SecureByteArray(SecureByteArray src)
		{
			Set(src.Get());
		}

		public SecureByteArray Set(byte[] ary)
		{
			if (SecureNumber.random_ == null)
			{
				SecureNumber.random_ = new JRandom();
			}
			bitmask_ = 0x22 | SecureNumber.random_.NextInt();
			original_ = ary;
			value_ = new byte[ary.Length];
			Generate(original_, value_);
			length = value_.Length;
			return this;
		}

		public byte[] Get()
		{
			if (buffer_ == null || buffer_.Length != value_.Length)
			{
				buffer_ = new byte[value_.Length];
			}
			Generate(value_, buffer_);
			for (int i = 0; i < buffer_.Length; i++)
			{
				if (buffer_[i] != original_[i])
				{
					SecureNumber.hacking_++;
					JSystem.Arraycopy(buffer_, 0, original_, 0, buffer_.Length);
					break;
				}
			}
			return buffer_;
		}

		public byte[] ToArray()
		{
			return Get();
		}

		private void Generate(byte[] from, byte[] to)
		{
			for (int i = 0; i < from.Length; i++)
			{
				to[i] = Generate(from[i]);
			}
		}

		private byte Generate(byte value)
		{
			return (byte)(~(value | ~bitmask_) | (value & ~bitmask_));
		}

		public static void Arraycopy(SecureByteArray srcArray, int srcIndex, SecureByteArray dstArray, int dstIndex, int length)
		{
			byte[] array = dstArray.ToArray();
			JSystem.Arraycopy(srcArray.ToArray(), srcIndex, array, dstIndex, length);
			dstArray.Set(array);
		}

		public static SecureByteArray ValueOf(byte[] ary)
		{
			if (ary == null)
			{
				return null;
			}
			return new SecureByteArray(ary);
		}

		public static byte[] ToArray(SecureByteArray ary)
		{
			return ary?.Get();
		}
	}
}
