using System.Security.Cryptography;
using System.Text;
using java.lang;

namespace kairo.unity.util
{
	public class MD5
	{
		private MD5()
		{
		}

		public static string GetValue(byte[] buf)
		{
			byte[] array = System.Security.Cryptography.MD5.Create().ComputeHash(buf);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				string text = JInteger.ToHexString(0xFF & array[i]);
				if (text.Length() == 1)
				{
					text = "0" + text;
				}
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}
	}
}
