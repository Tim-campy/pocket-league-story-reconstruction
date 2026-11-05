using kairo.unity.util;

namespace kairo.unity.panel
{
	public class Label : Component
	{
		public string text_;

		public Label()
		{
			text_ = "";
		}

		public Label(string text)
		{
			text_ = Language.TranslateText(text);
		}

		public int GetWidth()
		{
			return 240;
		}

		public int GetHeight()
		{
			return 12;
		}

		public void SetLocation(int x, int y)
		{
		}

		public void SetText(string text)
		{
		}
	}
}
