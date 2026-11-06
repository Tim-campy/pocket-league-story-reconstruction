using System;
using java.lang;
using kairo.unity.panel;
using kairo.unity.ui;
using kairo.unity.util;

namespace kfw.panel
{
	public class MainPanel : FepPanel
	{
		public delegate void OnFinishMethod(string result);

		private Label lbl1;

		private TextBox txtBox;

		private Button btn;

		private Button btn2;

		private Canvas bkCanvas_;

		private int maxLength_;

		private int maxWidth_;

		private char[] prohibition_ = new char[4] { '/', '|', '<', '>' };

		private string itemName_ = "名前";

		private bool endMark_ = true;

		private bool trim_;

		private int inputMode_ = 2;

		private bool end_;

		public string result_;

		public string defaultText_;

		public string originalText_;

		private string[] bkSoftLabels_ = new string[4];

		private OnFinishMethod onFinishMethod_;

		public MainPanel()
		{
		}

		public MainPanel(string label, string defaultText, int maxLength = 14, int inputMode = 2, string itemName = null)
		{
			Init(label, defaultText, maxLength, inputMode, itemName);
		}

		private void Init(string label, string defaultText, int maxLength, int inputMode, string itemName)
		{
			originalText_ = defaultText;
			maxLength_ = maxLength;
			inputMode_ = inputMode;
			if (itemName != null)
			{
				itemName_ = itemName;
			}
			if (inputMode != 2)
			{
				endMark_ = false;
				trim_ = true;
			}
			string text = "";
			for (int i = 0; i < maxLength_; i++)
			{
				text += "w";
			}
			maxWidth_ = GetStringWidth(text);
			if (!Language.Japanese())
			{
				string text2 = "";
				for (int j = 0; j < defaultText.Length; j++)
				{
					text = text2 + defaultText[j];
					if (GetStringWidth(text) > maxWidth_)
					{
						break;
					}
					text2 = text;
				}
				defaultText = text2;
			}
			lbl1 = new Label(label);
			txtBox = new TextBox(defaultText, 20, 1, 0);
			txtBox.SetInputMode(inputMode);
			txtBox.SetMaxLength(maxLength);
			btn = new Button("決定");
			btn2 = new Button("戻る");
			defaultText_ = txtBox.GetText();
			SetLayoutManager(null);
			lbl1.SetLocation(GetWidth() / 2 - lbl1.GetWidth() / 2, GetHeight() / 2 - lbl1.GetHeight() - 16);
			txtBox.SetLocation(GetWidth() / 2 - txtBox.GetWidth() / 2, GetHeight() / 2 - 16 + 4);
			btn.SetLocation(GetWidth() / 2 - btn.GetWidth() - 4, GetHeight() / 2 + txtBox.GetHeight() - 16 + 8);
			btn2.SetLocation(GetWidth() / 2 + 4, GetHeight() / 2 + txtBox.GetHeight() - 16 + 8);
			Add(lbl1);
			Add(txtBox);
			Add(btn);
			Add(btn2);
			SetComponentListener(this);
		}

		[Obsolete("入力パネルはiPhone対応に向けて処理をブロックさせず、第二引数にイベントハンドラを指定する方式に書き換えてください。")]
		public virtual string Show(Canvas canvas)
		{
			bkCanvas_ = canvas;
			Display.SetCurrent(this);
			SetSoftLabel(0, "");
			SetSoftLabel(1, "");
			end_ = false;
			while (!end_)
			{
				Show();
				IApplication.Sleep(1);
			}
			return result_;
		}

		public virtual void Show(Canvas canvas, OnFinishMethod onFinishMethod)
		{
			bkCanvas_ = canvas;
			bkSoftLabels_[0] = canvas.GetSoftLabel(0);
			bkSoftLabels_[1] = canvas.GetSoftLabel(1);
			bkSoftLabels_[2] = canvas.GetSoftLabel(2);
			bkSoftLabels_[3] = canvas.GetSoftLabel(3);
			canvas.SetSoftLabel("", "");
			canvas.UpdateSoftLabel();
			Display.SetCurrent(this);
			end_ = false;
			onFinishMethod_ = onFinishMethod;
			Start();
		}

		public override void ComponentAction(Component c, int type, int param)
		{
			int num = 0;
			if (type == 1)
			{
				if (c == btn)
				{
					string text = txtBox.GetText().Replace('\n', ' ');
					if (trim_)
					{
						text = text.Trim();
					}
					num = 0;
					if (Language.Japanese())
					{
						if (GetStringLength(text) > maxLength_)
						{
							num = 1;
						}
					}
					else if (GetStringWidth(text) > maxWidth_)
					{
						num = 1;
					}
					for (int i = 0; i < prohibition_.Length; i++)
					{
						if (text.IndexOf(prohibition_[i]) != -1)
						{
							num = 2;
							break;
						}
					}
					if (text.Trim('\b').Trim().Length() == 0)
					{
						num = 3;
					}
					if ((num == 0)) { result_ = text;
						if (result_.Equals(defaultText_))
						{
							result_ = originalText_;
						}
						else
						{
							result_ += "\b";
						}
						while (!endMark_ && result_.EndsWith("\b"))
						{
							result_ = result_.SubstringJ(0, result_.Length() - 1);
						}
						Display.SetCurrent(bkCanvas_);
						end_ = true;
						 } else if ((num == 1)) { {
						int num2 = maxLength_;
						if (Language.Japanese())
						{
							num2 /= 2;
						}
						string text3 = "全角<0>文字以内にして下さい";
						if (inputMode_ != 2)
						{
							text3 = "<0>文字以内にして下さい";
							num2 = maxLength_;
						}
						if (!Language.Japanese())
						{
							text3 = "内容を短くして下さい";
						}
						Dialog dialog3 = new Dialog(1, "ﾒｯｾｰｼﾞ");
						dialog3.SetText(StringUtil.Replace(Language.TranslateText(text3), JString.ValueOf(num2)));
						dialog3.Show();

					} } else if ((num == 2)) { {
						Dialog dialog2 = new Dialog(1, "ﾒｯｾｰｼﾞ");
						string text2 = "";
						for (int j = 0; j < prohibition_.Length; j++)
						{
							if (text2.Length() > 0)
							{
								text2 += " ";
							}
							text2 += prohibition_[j];
						}
						dialog2.SetText(StringUtil.Replace(Language.TranslateText("システムの都合上 <0> は使えません"), text2));
						dialog2.Show();

					} } else if ((num == 3)) { {
						Dialog dialog = new Dialog(1, "ﾒｯｾｰｼﾞ");
						dialog.SetText(StringUtil.Replace(Language.TranslateText("<0>を入力して下さい"), itemName_));
						dialog.Show();

					} }
				}
				else
				{
					result_ = null;
					Display.SetCurrent(bkCanvas_);
					end_ = true;
				}
			}
			if (end_)
			{
				if (onFinishMethod_ != null)
				{
					onFinishMethod_(result_);
				}
				bkCanvas_.SetSoftLabel(bkSoftLabels_[0], bkSoftLabels_[1], bkSoftLabels_[2], bkSoftLabels_[3]);
			}
			else
			{
				Start();
			}
		}

		public void SetProhibition(char[] words)
		{
			if (words == null)
			{
				prohibition_ = new char[0];
				return;
			}
			prohibition_ = new char[words.Length];
			JSystem.Arraycopy(words, 0, prohibition_, 0, words.Length);
		}

		public void SetItemName(string name)
		{
			itemName_ = name;
		}

		public void SetEndMark(bool enable)
		{
			endMark_ = enable;
		}

		public void SetTrim(bool enable)
		{
			trim_ = enable;
		}

		private int GetStringWidth(string str)
		{
			Font font = new Font();
			int result = font.StringWidth(str);
			font.Dispose();
			return result;
		}
	}
}
