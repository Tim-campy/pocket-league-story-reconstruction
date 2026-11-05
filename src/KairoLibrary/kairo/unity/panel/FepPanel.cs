using System;
using java.lang;
using kairo.common.cfg;
using kairo.unity.native;
using kairo.unity.ui;
using kairo.unity.util;

namespace kairo.unity.panel
{
	public abstract class FepPanel
	{
		public class Display
		{
			public static void SetCurrent(FepPanel panel)
			{
			}

			public static void SetCurrent(Canvas canvas)
			{
			}
		}

		public const int SOFT_KEY_1 = 0;

		public const int SOFT_KEY_2 = 1;

		public const int BUTTON_PRESSED = 1;

		public const int SELECTION_CHANGED = 2;

		public const int TEXT_CHANGED = 3;

		private static FepPanel current_;

		private Label label_;

		private TextBox text_;

		private Button btn1_;

		private Button btn2_;

		private int delay_;

		public static FepPanel Current => current_;

		public FepPanel()
		{
		}

		public void SetLayoutManager(object obj)
		{
		}

		public void SetComponentListener(object obj)
		{
		}

		public void SetSoftLabel(int key, string text)
		{
		}

		public int GetWidth()
		{
			return 240;
		}

		public int GetHeight()
		{
			return 240;
		}

		public void Add(Component component)
		{
			if (component is Label)
			{
				label_ = (Label)component;
			}
			if (component is TextBox)
			{
				text_ = (TextBox)component;
			}
			if (component is Button)
			{
				if (btn1_ == null)
				{
					btn1_ = (Button)component;
				}
				else
				{
					btn2_ = (Button)component;
				}
			}
		}

		[Obsolete("kfw.paenl.MainPanel.Show()を使用してください")]
		public void Show()
		{
			if (Config.PRINT)
			{
				Log.Info("入力パネルはiPhone対応に向けて処理をブロックさせず、kfw.paenl.MainPanel.Show() の第二引数にイベントハンドラを指定する方式に書き換えてください。");
			}
			string[] array = KairoPlugin.ShowFepPanel(label_.text_, text_.text_, text_.mode_, btn1_.label_, btn2_.label_, text_.maxLength_);
			text_.text_ = array[1];
			if (array[0] != null)
			{
				ComponentAction(btn1_, 1, 0);
			}
			else
			{
				ComponentAction(btn2_, 1, 0);
			}
		}

		public void Start()
		{
			current_ = this;
			delay_ = 2;
			KairoPlugin.StartFepPanel(label_.text_, text_.text_, text_.mode_, btn1_.label_, btn2_.label_, text_.maxLength_);
		}

		public void Start_iOS(int maxLen)
		{
			current_ = this;
			delay_ = 2;
			KairoPlugin.StartFepPanel(label_.text_, text_.text_, text_.mode_, btn1_.label_, btn2_.label_, maxLen);
		}

		internal bool Update()
		{
			if (Config.UNITY_EDITOR && --delay_ > 0)
			{
				return false;
			}
			if (!KairoPlugin.IsFepPanelFinish())
			{
				return false;
			}
			current_ = null;
			string[] fepPanelResult = KairoPlugin.GetFepPanelResult();
			text_.text_ = fepPanelResult[1];
			if (fepPanelResult[0] != null)
			{
				ComponentAction(btn1_, 1, 0);
			}
			else
			{
				ComponentAction(btn2_, 1, 0);
			}
			return true;
		}

		public abstract void ComponentAction(Component c, int type, int param);

		protected int GetStringLength(string s)
		{
			int num = 0;
			for (int i = 0; i < s.Length(); i++)
			{
				num = ((!IsSimpleChara(s.CharAt(i))) ? (num + 2) : (num + 1));
			}
			return num;
		}

		private bool IsSimpleChara(char c)
		{
			if (c >= '\u007f' || c < ' ')
			{
				if (c > '｠')
				{
					return c <= 'ﾟ';
				}
				return false;
			}
			return true;
		}
	}
}
