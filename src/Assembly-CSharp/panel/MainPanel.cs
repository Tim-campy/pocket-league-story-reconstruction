using kairo.unity.ui;
using kfw.panel;

namespace panel
{
	public class MainPanel : kfw.panel.MainPanel
	{
		public MainPanel()
		{
		}

		public MainPanel(string label, string defaultText)
			: base(label, defaultText)
		{
		}

		public MainPanel(string label, string defaultText, int maxLength)
			: base(label, defaultText, maxLength)
		{
		}

		public override string Show(Canvas canvas)
		{
			return base.Show(canvas);
		}

		public override void Show(Canvas canvas, OnFinishMethod onFinishMethod)
		{
			base.Show(canvas, onFinishMethod);
		}
	}
}
