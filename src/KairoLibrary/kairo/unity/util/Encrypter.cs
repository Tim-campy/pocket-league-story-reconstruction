namespace kairo.unity.util
{
	public class Encrypter
	{
		private static byte[] key_ = new byte[1];

		private Encrypter()
		{
		}

		public static void SetEncryptionKey(byte[] key)
		{
			key_ = key;
		}

		public static void SetEncryptionKey(sbyte[] key)
		{
			key_ = new byte[key.Length];
			for (int i = 0; i < key_.Length; i++)
			{
				key_[i] = (byte)key[i];
			}
		}

		public static void Encode(byte[] src)
		{
			Encode(src, key_);
		}

		public static void Encode(byte[] src, byte[] key)
		{
			for (int i = 0; i < src.Length; i++)
			{
				src[i] ^= key[i % key.Length];
			}
		}

		public static void Decode(byte[] src)
		{
			Decode(src, key_);
		}

		public static void Decode(byte[] src, byte[] key)
		{
			Encode(src, key);
		}

		public static void Encode(byte[] src, sbyte[] key)
		{
			for (int i = 0; i < src.Length; i++)
			{
				src[i] = (byte)(src[i] ^ key[i % key.Length]);
			}
		}

		public static void Decode(byte[] src, sbyte[] key)
		{
			Encode(src, key);
		}
	}
}
