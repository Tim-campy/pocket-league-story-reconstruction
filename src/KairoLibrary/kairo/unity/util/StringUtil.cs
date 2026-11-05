using System;
using System.Text;
using java.io;
using java.lang;
using kairo.common.cfg;

namespace kairo.unity.util
{
	public class StringUtil
	{
		private StringUtil()
		{
		}

		public static string Replace(string value, string args)
		{
			string[] array = Split(args, "\t", false, false, false);
			for (int i = 0; i < array.Length; i++)
			{
				value = Replace(value, "<" + i + ">", Language.TranslateText(array[i], false));
			}
			return value;
		}

		public static string Replace(string value, string oldStr, string newStr)
		{
			while (true)
			{
				int num = value.IndexOf(oldStr);
				if (num == -1)
				{
					break;
				}
				value = value.SubstringJ(0, num) + newStr + value.SubstringJ(num + oldStr.Length());
			}
			return value;
		}

		public static string[] Split(string value, string sep)
		{
			return Split(value, sep, true, false, false);
		}

		public static string[] Split(string value, string sep, bool zeroSkip)
		{
			return Split(value, sep, true, zeroSkip, false);
		}

		public static string[] Split(string text, string sep, bool trim, bool excludeZeroLength, bool escape)
		{
			if (text == null || text.Length() == 0)
			{
				return new string[0];
			}
			FastVector fastVector = new FastVector();
			int num = 0;
			string text2 = "";
			int num2;
			do
			{
				IL_0020:
				num2 = text.IndexOf(sep, num);
				if (num2 == -1)
				{
					text2 += text.Substring(num);
				}
				else
				{
					if (escape)
					{
						bool flag = true;
						int num3 = num2 - 1;
						while (num3 >= num && text.CharAt(num3) == '\\')
						{
							flag = !flag;
							num3--;
						}
						if (!flag)
						{
							text2 = text2 + text.SubstringJ(num, num2 - 1) + sep;
							num = num2 + sep.Length();
							goto IL_0020;
						}
					}
					text2 += text.SubstringJ(num, num2);
					num = num2 + sep.Length();
				}
				if (escape)
				{
					text2 = Escape(text2);
				}
				if (trim)
				{
					int i;
					for (i = 0; i < text2.Length && text2[i] <= ' '; i++)
					{
					}
					int num4 = text2.Length - 1;
					while (num4 >= 0 && text2[num4] <= ' ')
					{
						num4--;
					}
					text2 = ((num4 - i >= 0) ? text2.Substring(i, num4 - i + 1) : "");
				}
				if (!excludeZeroLength || text2.Length() > 0)
				{
					fastVector.AddElement(text2);
				}
				text2 = "";
			}
			while (num2 != -1);
			string[] array = new string[fastVector.Size()];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = (string)fastVector.ElementAt(j);
			}
			return array;
		}

		public static string Join(string sep, params object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Join(stringBuilder, sep, args);
			return stringBuilder.ToString();
		}

		private static void Join(StringBuilder sb, string sep, params object[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] != null && args[i].GetType().IsArray)
				{
					Array array = (Array)args[i];
					for (int j = 0; j < array.Length; j++)
					{
						Join(sb, sep, array.GetValue(j));
					}
					continue;
				}
				if (sb.Length > 0)
				{
					sb.Append(sep);
				}
				if (args[i] != null)
				{
					sb.Append(args[i]);
				}
			}
		}

		public static string Escape(string text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (num < text.Length())
			{
				int num2 = text.IndexOf('\\', num);
				if (num2 == -1)
				{
					stringBuilder.Append(text.Substring(num));
					break;
				}
				stringBuilder.Append(text.SubstringJ(num, num2));
				if (num2 + 1 < text.Length())
				{
					char c = text.CharAt(num2 + 1);
					if (c == '\\')
					{
						stringBuilder.Append('\\');
					}
					else if (c == 't')
					{
						stringBuilder.Append('\t');
					}
					else if (c == 'n')
					{
						stringBuilder.Append('\n');
					}
				}
				num = num2 + 2;
			}
			return stringBuilder.ToString();
		}

		public static string FormatString(string value, int len)
		{
			return FormatString(value, len, ' ', false);
		}

		public static string FormatString(string value, int len, char padding)
		{
			return FormatString(value, len, padding, false);
		}

		public static string FormatString(string value, int len, char padding, bool left)
		{
			string text = "";
			int num = JString.ValueOf(padding).GetBytes().Length;
			int num2 = len - value.GetBytes().Length;
			for (int i = 0; i < num2; i += num)
			{
				text += padding;
			}
			if (left)
			{
				return value + text;
			}
			return text + value;
		}

		public static string[] LoadStringArray(JarInflater jar, string fileName)
		{
			return LoadStringArray(jar, fileName, false);
		}

		public static string[] LoadStringArray(JarInflater jar, string fileName, bool zeroSkip)
		{
			InputStream inputStream = null;
			try
			{
				inputStream = jar.GetInputStream(fileName);
				if (inputStream == null)
				{
					return null;
				}
				return LoadStringArray(inputStream, zeroSkip);
			}
			finally
			{
				inputStream?.Close();
				inputStream = null;
			}
		}

		public static string[] LoadStringArray(InputStream stream)
		{
			return LoadStringArray(stream, false);
		}

		public static string[] LoadStringArray(InputStream stream, bool zeroSkip)
		{
			return Split(JString.New(StreamUtil.Read(stream)), "\n", false, zeroSkip, false);
		}

		public static string ToWideString(string text)
		{
			string text2 = "";
			for (int i = 0; i < text.Length(); i++)
			{
				char c = text.CharAt(i);
				if ('!' <= c && c <= '~')
				{
					c = (char)(65345 + (c - 97));
				}
				text2 += c;
			}
			return text2;
		}

		public static int[] SplitIntegers(string src, string sep)
		{
			string[] array = Split(src, sep, true, true, false);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = JInteger.ParseInt(array[i]);
			}
			return array2;
		}

		public static string GetString(byte[] bytes)
		{
			string code = GetCode(bytes);
			if (code != null)
			{
				try
				{
					return JString.New(bytes, code);
				}
				catch (Exception)
				{
				}
			}
			return JString.New(bytes);
		}

		public static string GetCode(byte[] bytes)
		{
			string[] array = new string[2] { "UTF-8", "Shift_JIS" };
			if (bytes.Length == 0)
			{
				return array[0];
			}
			for (int i = 0; i < array.Length; i++)
			{
				string str = JString.New(bytes, array[i]);
				if (str.Length() == 0)
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < str.Length(); j++)
				{
					if (str.CharAt(j) == '\ufffd')
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return array[i];
				}
			}
			int num = bytes.Length;
			bool flag2 = false;
			for (int k = 0; k < num; k++)
			{
				int num2 = bytes[k] & 0xFF;
				if (num2 <= 6 || num2 == 127 || num2 == 255)
				{
					flag2 = true;
					if (num2 == 0 && k < num - 1 && (bytes[k + 1] & 0xFF) <= 127)
					{
						return "UTF-16";
					}
				}
			}
			if (flag2)
			{
				return null;
			}
			bool flag3 = true;
			for (int l = 0; l < num; l++)
			{
				int num2 = bytes[l] & 0xFF;
				if (num2 == 27 || 128 <= num2)
				{
					flag3 = false;
					break;
				}
			}
			if (flag3)
			{
				return "ASCII";
			}
			for (int m = 0; m < num - 2; m++)
			{
				int num2 = bytes[m] & 0xFF;
				int num3 = bytes[m + 1] & 0xFF;
				int num4 = bytes[m + 2] & 0xFF;
				if (num2 != 27)
				{
					continue;
				}
				if (num3 == 36 && num4 == 64)
				{
					return "JIS";
				}
				if (num3 == 36 && num4 == 66)
				{
					return "JIS";
				}
				if (num3 == 40 && (num4 == 66 || num4 == 74))
				{
					return "JIS";
				}
				if (num3 == 40 && num4 == 73)
				{
					return "JIS";
				}
				if (m < num - 3)
				{
					int num5 = bytes[m + 3] & 0xFF;
					if (num3 == 36 && num4 == 40 && num5 == 68)
					{
						return "JIS";
					}
					if (m < num - 5 && num3 == 38 && num4 == 64 && num5 == 27 && (bytes[m + 4] & 0xFF) == 36 && (bytes[m + 5] & 0xFF) == 66)
					{
						return "JIS";
					}
				}
			}
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			for (int n = 0; n < num - 1; n++)
			{
				int num2 = bytes[n] & 0xFF;
				int num3 = bytes[n + 1] & 0xFF;
				if (((129 <= num2 && num2 <= 159) || (224 <= num2 && num2 <= 252)) && ((64 <= num3 && num3 <= 126) || (128 <= num3 && num3 <= 252)))
				{
					num6 += 2;
					n++;
				}
			}
			for (int num9 = 0; num9 < num - 1; num9++)
			{
				int num2 = bytes[num9] & 0xFF;
				int num3 = bytes[num9 + 1] & 0xFF;
				if ((161 <= num2 && num2 <= 254 && 161 <= num3 && num3 <= 254) || (num2 == 142 && 161 <= num3 && num3 <= 223))
				{
					num7 += 2;
					num9++;
				}
				else if (num9 < num - 2)
				{
					int num4 = bytes[num9 + 2] & 0xFF;
					if (num2 == 143 && 161 <= num3 && num3 <= 254 && 161 <= num4 && num4 <= 254)
					{
						num7 += 3;
						num9 += 2;
					}
				}
			}
			for (int num10 = 0; num10 < num - 1; num10++)
			{
				int num2 = bytes[num10] & 0xFF;
				int num3 = bytes[num10 + 1] & 0xFF;
				if (192 <= num2 && num2 <= 223 && 128 <= num3 && num3 <= 191)
				{
					num8 += 2;
					num10++;
				}
				else if (num10 < num - 2)
				{
					int num4 = bytes[num10 + 2] & 0xFF;
					if (224 <= num2 && num2 <= 239 && 128 <= num3 && num3 <= 191 && 128 <= num4 && num4 <= 191)
					{
						num8 += 3;
						num10 += 2;
					}
				}
			}
			int[] array2 = new int[2]
			{
				num8 * 10,
				num6 * 10
			};
			string[] array3 = new string[2] { "UTF-8", "Shift_JIS" };
			if (Config.APPLI_TYPE == 2)
			{
				array2[0]++;
			}
			else if (Config.APPLI_TYPE == 3)
			{
				array2[0]++;
			}
			else
			{
				array2[1]++;
			}
			for (int num11 = 0; num11 < array2.Length - 1; num11++)
			{
				for (int num12 = array2.Length - 1; num12 > num11; num12--)
				{
					if (array2[num12 - 1] < array2[num12])
					{
						int num13 = array2[num12];
						array2[num12] = array2[num12 - 1];
						array2[num12 - 1] = num13;
						string text = array3[num12];
						array3[num12] = array3[num12 - 1];
						array3[num12 - 1] = text;
					}
				}
			}
			for (int num14 = 0; num14 < array3.Length; num14++)
			{
				bool flag4 = false;
				string str2 = JString.New(bytes, array3[num14]);
				for (int num15 = 0; num15 < str2.Length(); num15++)
				{
					if (str2.CharAt(num15) == '\ufffd')
					{
						flag4 = true;
						break;
					}
				}
				if (!flag4)
				{
					return array3[num14];
				}
			}
			return null;
		}
	}
}
