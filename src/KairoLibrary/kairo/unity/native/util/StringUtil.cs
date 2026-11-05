using System.Collections.Generic;
using System.Text;

namespace kairo.unity.native.util
{
	public static class StringUtil
	{
		private const string ESCAPES = ",&@\\";

		private static string Escape(string str)
		{
			if (str == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder(str.Length);
			foreach (char value in str)
			{
				if (",&@\\".IndexOf(value) != -1)
				{
					stringBuilder.Append('\\');
				}
				stringBuilder.Append(value);
			}
			return "\"" + stringBuilder.ToString() + "\"";
		}

		private static string Unescape(string str)
		{
			if (str.Length == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(str.Length);
			for (int i = 1; i < str.Length - 1; i++)
			{
				char value = str[i];
				if (",&@\\".IndexOf(value) != -1)
				{
					value = str[++i];
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		private static string[] Split(string src, char sep)
		{
			List<string> list = new List<string>();
			int num = 0;
			for (int i = 0; i < src.Length; i++)
			{
				char c = src[i];
				if (c == '\\')
				{
					i++;
				}
				else if (c == sep)
				{
					list.Add(src.Substring(num, i - num));
					num = i + 1;
				}
			}
			if (num <= src.Length)
			{
				list.Add(src.Substring(num));
			}
			string[] array = new string[list.Count];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = list[j];
			}
			return array;
		}

		public static string GetString(string[] ary)
		{
			if (ary == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < ary.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(Escape(ary[i]));
			}
			return stringBuilder.ToString();
		}

		public static string GetString(string[][] ary)
		{
			if (ary == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < ary.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(',');
				}
				for (int j = 0; j < ary[i].Length; j++)
				{
					if (j != 0)
					{
						stringBuilder.Append('&');
					}
					stringBuilder.Append(Escape(ary[i][j]));
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetString(string[][][] ary)
		{
			if (ary == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < ary.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(',');
				}
				for (int j = 0; j < ary[i].Length; j++)
				{
					if (j != 0)
					{
						stringBuilder.Append('&');
					}
					for (int k = 0; k < ary[i][j].Length; k++)
					{
						if (k != 0)
						{
							stringBuilder.Append('@');
						}
						stringBuilder.Append(Escape(ary[i][j][k]));
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string[] GetStrArray(string src)
		{
			if (src == null)
			{
				return null;
			}
			string[] array = Split(src, ',');
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = Unescape(array[i]);
			}
			return array2;
		}

		public static string[][] GetStrArray2(string src)
		{
			if (src == null)
			{
				return null;
			}
			string[] array = Split(src, '&');
			string[][] array2 = new string[array.Length][];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = GetStrArray(array[i]);
			}
			return array2;
		}

		public static string[][][] GetStrArray3(string src)
		{
			if (src == null)
			{
				return null;
			}
			string[] array = Split(src, '@');
			string[][][] array2 = new string[array.Length][][];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = GetStrArray2(array[i]);
			}
			return array2;
		}
	}
}
