using kairo.unity.native;
using kairo.unity.util;

namespace kairo.unity.ui
{
	public class Dialog
	{
		public const int BUTTON_CANCEL = 2;

		public const int BUTTON_NO = 8;

		public const int BUTTON_OK = 1;

		public const int BUTTON_YES = 4;

		public const int DIALOG_ERROR = 2;

		public const int DIALOG_INFO = 0;

		public const int DIALOG_WARNING = 1;

		public const int DIALOG_YESNO = 3;

		public const int DIALOG_YESNOCANCEL = 4;

		private Font font_;

		private string title_;

		private string msg_;

		private string[] cmds_;

		private int background_;

		private int type_;

		private string url_;

		public Dialog(int type, string title)
		{
			type_ = type;
			title_ = Language.TranslateText(title);
			font_ = new Font();
			msg_ = "";
			background_ = Graphics.GetColorOfName(-16777216);
		}

		public void SetFont(Font f)
		{
			font_ = f;
		}

		public void SetText(string msg)
		{
			string[] array = StringUtil.Split(msg, "\t");
			msg_ = Language.TranslateText(array[0]);
			msg_ = StringUtil.Replace(msg_, "<br>", "\n");
			url_ = null;
			if (1 < array.Length)
			{
				url_ = array[1];
			}
		}

		public void SetButtons(params string[] buttons)
		{
			cmds_ = buttons;
		}

		public void SetBackground(int c)
		{
			background_ = c;
		}

		public void SetSoftLabel(int key, string label)
		{
		}

		public int Show()
		{
			string text = null;
			if (cmds_ != null)
			{
				text = "";
				for (int i = 0; i < cmds_.Length; i++)
				{
					if (i != 0)
					{
						text += ",";
					}
					text += cmds_[i];
				}
			}
			int num = KairoPlugin.ShowDialog(type_, title_, msg_, text);
			if (url_ != null && (num == 1 || num == 4))
			{
				Canvas.OpenBrowser(url_);
			}
			return num;
		}
	}
}
