using System.Globalization;
using System.IO;
using System.Text;

namespace java.net
{
	public static class URLDecoder
	{
		public static string Decode(string s)
		{
			return Decode(s, "UTF-8");
		}

		public static string Decode(string s, string enc)
		{
			Encoding encoding = Encoding.GetEncoding(enc);
			string text = "";
			byte[] bytes = encoding.GetBytes(" ");
			for (int i = 0; i < bytes.Length; i++)
			{
				text = text + "%" + bytes[i].ToString("X2");
			}
			s = s.Replace("+", text);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = 0;
				while (num < s.Length)
				{
					if (s[num] == '%')
					{
						memoryStream.WriteByte(byte.Parse(s.Substring(num + 1, 2), NumberStyles.HexNumber));
						num += 3;
					}
					else
					{
						bytes = encoding.GetBytes(s[num].ToString());
						memoryStream.Write(bytes, 0, bytes.Length);
						num++;
					}
				}
				return encoding.GetString(memoryStream.ToArray());
			}
		}
	}
}
