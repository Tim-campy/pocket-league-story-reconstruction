using System;
using System.Text;

namespace java.lang
{
	public static class StringEx
	{
		public static char CharAt(this string str, int index)
		{
			return str[index];
		}

		public static bool EqualsIgnoreCase(this string str, string anotherString)
		{
			if (str.Length != anotherString.Length)
			{
				return false;
			}
			return str.Equals(anotherString, StringComparison.CurrentCultureIgnoreCase);
		}

		public static byte[] GetBytes(this string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		public static byte[] GetBytes(this string str, string enc)
		{
			return Encoding.GetEncoding(enc).GetBytes(str);
		}

		public static int IndexOf(this string str, int ch)
		{
			return str.IndexOf((char)ch);
		}

		public static int IndexOf(this string str, int ch, int fromIndex)
		{
			return str.IndexOf((char)ch, fromIndex);
		}

		public static int LastIndexOf(this string str, int ch)
		{
			return str.LastIndexOf((char)ch);
		}

		public static int LastIndexOf(this string str, int ch, int fromIndex)
		{
			return str.LastIndexOf((char)ch, fromIndex);
		}

		public static int Length(this string str)
		{
			return str.Length;
		}

		public static bool StartsWith(this string str, string prefix, int toffset)
		{
			for (int i = 0; i < prefix.Length; i++)
			{
				if (i + toffset >= str.Length || str[i + toffset] != prefix[i])
				{
					return false;
				}
			}
			return true;
		}

		public static string ToLowerCase(this string str)
		{
			return str.ToLower();
		}

		public static string ToUpperCase(this string str)
		{
			new char[10].ToString();
			return str.ToUpper();
		}

		public static string SubstringJ(this string str, int beginIndex, int endIndex)
		{
			return str.Substring(beginIndex, endIndex - beginIndex);
		}

		public static string SubstringJ(this string str, int beginIndex)
		{
			return str.Substring(beginIndex);
		}
	}
}
