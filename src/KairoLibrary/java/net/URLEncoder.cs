using System.Collections.Generic;
using System.Text;

namespace java.net
{
	public static class URLEncoder
	{
		private static Dictionary<char, string> convertTable_;

		static URLEncoder()
		{
			convertTable_ = new Dictionary<char, string>();
			convertTable_.Clear();
			convertTable_.Add(' ', "+");
			convertTable_.Add('*', "*");
			convertTable_.Add('-', "-");
			convertTable_.Add('.', ".");
			convertTable_.Add('_', "_");
		}

		public static string Encode(string s)
		{
			return Encode(s, "UTF-8");
		}

		public static string Encode(string s, string enc)
		{
			Encoding encoding = Encoding.GetEncoding(enc);
			StringBuilder stringBuilder = new StringBuilder();
			char[] array = s.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				string value = null;
				if (convertTable_.TryGetValue(c, out value))
				{
					stringBuilder.Append(c);
					continue;
				}
				byte[] bytes = encoding.GetBytes(c.ToString());
				for (int j = 0; j < bytes.Length; j++)
				{
					stringBuilder.Append("%" + bytes[j].ToString("X2"));
				}
			}
			return stringBuilder.ToString();
		}
	}
}
