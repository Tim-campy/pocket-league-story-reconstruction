using System;
using System.Text;

namespace kairo.unity.util
{
	public class Base64
	{
		private Base64()
		{
		}

		public static byte[] Decode(byte[] bytes)
		{
			return Decode(Encoding.UTF8.GetString(bytes));
		}

		public static byte[] Decode(string str)
		{
			try
			{
				return Convert.FromBase64String(str);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static string Encode(byte[] bytes)
		{
			return Convert.ToBase64String(bytes);
		}

		public static string Encode(string str)
		{
			return Encode(Encoding.UTF8.GetBytes(str));
		}
	}
}
