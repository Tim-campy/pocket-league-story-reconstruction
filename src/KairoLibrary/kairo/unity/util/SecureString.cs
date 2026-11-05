using System;
using System.Text;
using java.lang;
using java.util;

namespace kairo.unity.util
{
	public class SecureString : SecureNumber
	{
		private new string original_;

		private new char[] value_;

		private char[] buffer_;

		private int hashCode_;

		public char this[int index] => CharAt(index);

		public static implicit operator string(SecureString s)
		{
			return s?.ToString();
		}

		public static implicit operator SecureString(string s)
		{
			if (s == null)
			{
				return null;
			}
			return new SecureString(s);
		}

		public static SecureString operator +(SecureString z, string w)
		{
			return new SecureString(string.Concat(z, w));
		}

		public static SecureString operator +(string z, SecureString w)
		{
			return new SecureString(z + w);
		}

		public static SecureString operator +(SecureString z, SecureString w)
		{
			return new SecureString(string.Concat(z, w));
		}

		public SecureString()
		{
			Set("");
		}

		public SecureString(string value)
		{
			Set(value);
		}

		public SecureString(char[] value)
		{
			Set(new string(value));
		}

		public SecureString(char[] value, int offset, int count)
		{
			Set(new string(value, offset, count));
		}

		public SecureString(byte[] bytes, int offset, int count, string enc)
		{
			Set(JString.New(bytes, offset, count, enc));
		}

		public SecureString(byte[] bytes, string enc)
		{
			Set(JString.New(bytes, enc));
		}

		public SecureString(byte[] bytes, int offset, int count)
		{
			Set(JString.New(bytes, offset, count));
		}

		public SecureString(byte[] bytes)
		{
			Set(JString.New(bytes));
		}

		public SecureString(StringBuilder buffer)
		{
			Set(buffer.ToString());
		}

		public SecureString(SecureString src)
		{
			Set(src.Get());
		}

		internal SecureString Set(string value)
		{
			if (SecureNumber.random_ == null)
			{
				SecureNumber.random_ = new JRandom();
			}
			bitmask_ = 0x22 | SecureNumber.random_.NextInt();
			original_ = value;
			value_ = original_.ToCharArray();
			Generate(value_, value_);
			hashCode_ = 0;
			for (int i = 0; i < value_.Length; i++)
			{
				hashCode_ ^= value_[i];
			}
			return this;
		}

		internal SecureString Set(SecureString value)
		{
			Set(value.Get());
			return this;
		}

		public string Get()
		{
			if (buffer_ == null || buffer_.Length != value_.Length)
			{
				buffer_ = new char[value_.Length];
			}
			Generate(value_, buffer_);
			string text = new string(buffer_);
			if (!text.Equals(original_))
			{
				SecureNumber.hacking_++;
				original_ = text;
			}
			return text;
		}

		private void Generate(char[] from, char[] to)
		{
			for (int i = 0; i < from.Length; i++)
			{
				to[i] = Generate(from[i]);
			}
		}

		private char Generate(char value)
		{
			return (char)(~(value | ~bitmask_) | (value & ~bitmask_));
		}

		public override string ToString()
		{
			return Get();
		}

		public static SecureString[] Array(int n)
		{
			SecureString[] array = new SecureString[n];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new SecureString();
			}
			return array;
		}

		public static SecureString[][] Array(int n1, int n2)
		{
			SecureString[][] array = JTool.MakeArray<SecureString[][]>(new int[2] { n1, n2 });
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					array[i][j] = new SecureString();
				}
			}
			return array;
		}

		public static void Arraycopy(SecureString[] srcArray, int srcIndex, SecureString[] dstArray, int dstIndex, int length)
		{
			for (int i = 0; i < length; i++)
			{
				if (srcArray[srcIndex + i] == null)
				{
					dstArray[dstIndex + i] = null;
				}
				else
				{
					dstArray[dstIndex + i] = new SecureString(srcArray[srcIndex + i]);
				}
			}
		}

		public int Length()
		{
			return value_.Length;
		}

		public char CharAt(int index)
		{
			return Generate(value_[index]);
		}

		public void GetChars(int srcBegin, int srcEnd, char[] dst, int dstBegin)
		{
			int num = srcEnd - srcBegin;
			for (int i = 0; i < num; i++)
			{
				dst[dstBegin + i] = Generate(value_[srcBegin + i]);
			}
		}

		public byte[] GetBytes(string enc)
		{
			return Get().GetBytes(enc);
		}

		public byte[] GetBytes()
		{
			return Get().GetBytes();
		}

		public bool Equals(string anObject)
		{
			return Get().Equals(anObject);
		}

		public bool Equals(SecureString anObject)
		{
			return Get().Equals(anObject.Get());
		}

		public int CompareTo(string anotherString)
		{
			return Get().CompareTo(anotherString);
		}

		public bool RegionMatches(bool ignoreCase, int toffset, string other, int ooffset, int len)
		{
			string text = Substring(toffset, len).ToString();
			string value = other.Substring(ooffset, len);
			return text.Equals(value, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);
		}

		public bool StartsWith(string prefix, int toffset)
		{
			return Get().StartsWith(prefix, toffset);
		}

		public bool StartsWith(string prefix)
		{
			return Get().StartsWith(prefix);
		}

		public bool EndsWith(string suffix)
		{
			return Get().EndsWith(suffix);
		}

		public int IndexOf(int ch)
		{
			return Get().IndexOf(ch);
		}

		public int IndexOf(int ch, int fromIndex)
		{
			return Get().IndexOf(ch, fromIndex);
		}

		public int LastIndexOf(int ch)
		{
			return Get().LastIndexOf(ch);
		}

		public int LastIndexOf(int ch, int fromIndex)
		{
			return Get().LastIndexOf(ch, fromIndex);
		}

		public int IndexOf(string str)
		{
			return Get().IndexOf(str);
		}

		public int IndexOf(string str, int fromIndex)
		{
			return Get().IndexOf(str, fromIndex);
		}

		public SecureString Substring(int beginIndex)
		{
			return new SecureString(Get().Substring(beginIndex));
		}

		public SecureString Substring(int beginIndex, int length)
		{
			return new SecureString(Get().Substring(beginIndex, length));
		}

		public SecureString SubstringJ(int beginIndex)
		{
			return Substring(beginIndex);
		}

		public SecureString SubstringJ(int beginIndex, int endIndex)
		{
			return new SecureString(Get().SubstringJ(beginIndex, endIndex));
		}

		public SecureString Concat(string str)
		{
			return new SecureString(Get() + str);
		}

		public SecureString Replace(char oldChar, char newChar)
		{
			return new SecureString(Get().Replace(oldChar, newChar));
		}

		public SecureString ToLowerCase()
		{
			return new SecureString(Get().ToLowerCase());
		}

		public SecureString ToUpperCase()
		{
			return new SecureString(Get().ToUpperCase());
		}

		public SecureString Trim()
		{
			return new SecureString(Get().Trim());
		}

		public char[] ToCharArray()
		{
			return Get().ToCharArray();
		}

		public static SecureString ValueOf(object obj)
		{
			return new SecureString(JString.ValueOf(obj));
		}

		public static SecureString ValueOf(char[] data)
		{
			return new SecureString(JString.ValueOf(data));
		}

		public static SecureString ValueOf(char[] data, int offset, int count)
		{
			return new SecureString(JString.ValueOf(data, offset, count));
		}

		public static SecureString ValueOf(bool b)
		{
			return new SecureString(JString.ValueOf(b));
		}

		public static SecureString ValueOf(char c)
		{
			return new SecureString(JString.ValueOf(c));
		}

		public static SecureString ValueOf(int i)
		{
			return new SecureString(JString.ValueOf(i));
		}

		public static SecureString ValueOf(long l)
		{
			return new SecureString(JString.ValueOf(l));
		}

		public static SecureString ValueOf(string str)
		{
			if (str == null)
			{
				return null;
			}
			return new SecureString(str);
		}

		public static SecureString ValueOf(byte[] bytes)
		{
			if (bytes == null)
			{
				return null;
			}
			return new SecureString(bytes);
		}

		public static string ToString(SecureString str)
		{
			return str?.ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is SecureString)
			{
				return Get().Equals(((SecureString)obj).Get());
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return hashCode_;
		}
	}
}
