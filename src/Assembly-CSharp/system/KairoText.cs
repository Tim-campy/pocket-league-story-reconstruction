using java.lang;
using kairo.unity.util;

namespace system
{
	public class KairoText
	{
		private static readonly string[][] TRANSLATE_TABLE = new string[0][];

		private static bool convertTranslateTable_;

		public static string LT(string text)
		{
			string text2 = Language.TranslateText(text);
			if (text2 != text)
			{
				return text2;
			}
			if (Language.Japanese())
			{
				return text;
			}
			if (!convertTranslateTable_)
			{
				for (int i = 0; i < TRANSLATE_TABLE.Length; i++)
				{
					for (int j = 0; j < TRANSLATE_TABLE[i].Length; j++)
					{
						TRANSLATE_TABLE[i][j] = StringUtil.Replace(TRANSLATE_TABLE[i][j], "\n", "<br>");
					}
				}
				convertTranslateTable_ = true;
			}
			string value = StringUtil.Replace(text, "\n", "<br>");
			int num = 1;
			for (int k = 0; k < TRANSLATE_TABLE.Length; k++)
			{
				if (num < TRANSLATE_TABLE[k].Length && TRANSLATE_TABLE[k][0].Equals(value))
				{
					return TRANSLATE_TABLE[k][num];
				}
			}
			return text;
		}

		public static string LT(string text, string arg)
		{
			return StringUtil.Replace(LT(text), arg);
		}

		public static string LT(string text, int arg)
		{
			return StringUtil.Replace(LT(text), JString.ValueOf(arg));
		}
	}
}
