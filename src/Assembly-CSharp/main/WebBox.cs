using kairo.unity.ui;
using native;
using surface;

namespace main
{
	public class WebBox
	{
		public WebBox()
		{
			WebBoxPlugin.Create();
		}

		public static WebBox Create()
		{
			{ bool recoveredCondition_200 = WebBoxPlugin.Create();
if (!recoveredCondition_200)
			{
				return null;
			}
}
			return new WebBox();
		}

		public void SetViewRect(int x, int y, int width, int height)
		{
			SetViewRect(null, x, y, width, height);
		}

		public void SetViewRect(Graphics g, int x, int y, int width, int height)
		{
			x += g.GetOriginX();
			y += g.GetOriginY();
			GameView instance = GameView.GetInstance();
			x = instance.GetX() + (int)((float)x * instance.GetScaleRatio() / 100f + 0.01f);
			y = instance.GetY() + (int)((float)y * instance.GetScaleRatio() / 100f + 0.01f);
			width = (int)((float)width * instance.GetScaleRatio() / 100f + 0.01f);
			height = (int)((float)height * instance.GetScaleRatio() / 100f + 0.01f);
			WebBoxPlugin.SetViewRect(x, y, width, height);
		}

		public void LoadUrl(string url)
		{
			WebBoxPlugin.LoadUrl(url);
		}

		public void Dispose()
		{
			WebBoxPlugin.Dispose();
		}

		public bool Visible()
		{
			return WebBoxPlugin.Visible();
		}

		public void SetVisible(bool visible)
		{
			WebBoxPlugin.SetVisible(visible);
		}
	}
}
