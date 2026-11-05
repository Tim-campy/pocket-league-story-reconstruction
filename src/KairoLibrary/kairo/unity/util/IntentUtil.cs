using System;
using java.lang;

namespace kairo.unity.util
{
	public class IntentUtil
	{
		private static byte[] AddCrcTo(byte[] sendData)
		{
			byte[] array = new byte[sendData.Length + 8];
			JSystem.Arraycopy(EncodeUtil.LongToByteAry(CRC64.GetValue(sendData), true), 0, array, 0, 8);
			JSystem.Arraycopy(sendData, 0, array, 8, sendData.Length);
			return array;
		}

		private static SecureString EncodeSendData(byte[] sendData)
		{
			int[] array = new int[11]
			{
				-101254619, 1955144635, -50531940, -1372136611, -877512591, 553577529, -463354533, 515797882, 101671734, -16938526,
				1680794419
			};
			byte[] array2 = new byte[array.Length * 4];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = (byte)((array[i / 4] >> i % 4 * 8) & 0xFF);
			}
			Encrypter.Encode(sendData, array2);
			return new SecureString(Base64.Encode(sendData));
		}

		public static SecureString PackSendData(byte[] sendData)
		{
			sendData = Compressor.Compress(sendData);
			sendData = AddCrcTo(sendData);
			return EncodeSendData(sendData);
		}

		private static byte[] DecodeReceivedData(string receivedData)
		{
			byte[] array = Base64.Decode(receivedData);
			int[] array2 = new int[11]
			{
				-101254619, 1955144635, -50531940, -1372136611, -877512591, 553577529, -463354533, 515797882, 101671734, -16938526,
				1680794419
			};
			byte[] array3 = new byte[array2.Length * 4];
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i] = (byte)((array2[i / 4] >> i % 4 * 8) & 0xFF);
			}
			Encrypter.Decode(array, array3);
			return array;
		}

		private static byte[] RemoveCrcFrom(byte[] receivedData)
		{
			long num = EncodeUtil.ByteAryToLong(receivedData, true);
			byte[] array = new byte[receivedData.Length - 8];
			JSystem.Arraycopy(receivedData, 8, array, 0, array.Length);
			if (CRC64.GetValue(array) != num)
			{
				throw new Exception();
			}
			return array;
		}

		public static byte[] UnpackReceivedData(string receivedData)
		{
			return Compressor.Expand(RemoveCrcFrom(DecodeReceivedData(receivedData)));
		}
	}
}
