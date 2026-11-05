using kairo.unity.ui;
using kfw.panel;

namespace system.panel
{
	public class InputPanel : MainPanel
	{
		public InputPanel()
		{
		}

		public InputPanel(string label, string defaultText, int maxLength = 14, int inputMode = 2, string itemName = null)
			: base(label, defaultText, maxLength, inputMode, itemName)
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
