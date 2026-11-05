using kairo.unity.util;

namespace kairo.unity.panel
{
	public class Button : Component
	{
		public string label_;

		public Button(string text)
		{
			label_ = Language.TranslateSoftLabel(Language.TranslateText(text));
		}

		public int GetWidth()
		{
			return 60;
		}

		public int GetHeight()
		{
			return 12;
		}

		public void SetLocation(int x, int y)
		{
		}

		public void SetEnabled(bool enable)
		{
		}
	}
}
