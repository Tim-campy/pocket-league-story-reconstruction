using java.lang;
using kairo.unity.util;

namespace kairo.unity.ui
{
	public class TextFormat
	{
		protected static TextFormat textFormat_;

		protected int font_;

		protected int[] x_;

		protected int[] y_;

		protected int anchor_;

		public TextFormat()
		{
			Init(null);
		}

		public TextFormat(string format)
		{
			Init(format);
		}

		public static TextFormat Create(string format)
		{
			if (textFormat_ == null)
			{
				textFormat_ = new TextFormat();
			}
			textFormat_.Init(format);
			return textFormat_;
		}

		private void Init(string format)
		{
			font_ = -1;
			x_ = (y_ = null);
			anchor_ = -1;
			if (format == null)
			{
				return;
			}
			string[] array = StringUtil.Split(format, ",");
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.Length() == 0)
				{
					continue;
				}
				if ((i == 0)) { font_ = JInteger.ParseInt(text);
					 } else if ((i == 1) || (i == 2)) { {
					char c = text.CharAt(0);
					if (c == '+' || c == '-' || c == '*' || c == '/')
					{
						text = text.Substring(1);
					}
					int num = (int)(JFloat.ParseFloat(text) * 10f);
					int[] array2 = new int[2];
					if ((c == '+')) { array2[0] = 1;
						 } else if ((c == '-')) { array2[0] = 2;
						 } else if ((c == '*')) { array2[0] = 3;
						 } else if ((c == '/')) { array2[0] = 4;
						 }
					array2[1] = num;
					if (i == 1)
					{
						x_ = array2;
					}
					else
					{
						y_ = array2;
					}

				} } else if ((i == 3)) { anchor_ = JInteger.ParseInt(text);
					 }
			}
		}

		public int GetFont()
		{
			return font_;
		}

		public float GetX(float x)
		{
			return GetXY(x, x_);
		}

		public float GetY(float y)
		{
			return GetXY(y, y_);
		}

		private float GetXY(float basis, int[] elem)
		{
			if (elem == null)
			{
				return basis;
			}
			float num = (float)elem[1] / 10f;
			if (elem[0] == 0)
			{
				return num;
			}
			if (elem[0] == 1)
			{
				return basis + num;
			}
			if (elem[0] == 2)
			{
				return basis - num;
			}
			if (elem[0] == 3)
			{
				return basis * num;
			}
			return basis / num;
		}

		public int GetAnchor(int anchor)
		{
			if (anchor_ == -1)
			{
				return anchor;
			}
			if (BitUtil.Check(anchor_, 7))
			{
				anchor = BitUtil.Off(anchor, 7);
			}
			if (BitUtil.Check(anchor_, 112))
			{
				anchor = BitUtil.Off(anchor, 112);
			}
			return anchor | anchor_;
		}
	}
}
