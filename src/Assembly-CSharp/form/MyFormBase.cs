using System;
using data;
using game;
using java.lang;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;

namespace form
{
	public class MyFormBase : FormBase
	{
		internal IApplication app_;

		internal AppData apdat_;

		internal Canvas canvas_;

		internal FormManager formMan_;

		internal GameView view_;

		internal GamePad pad_;

		internal int update_;

		internal int draw_;

		internal SubForm dialog_;

		internal int result_;

		internal int arwFrame_;

		private int[] drawWindow_tmp;

		internal static readonly int WNDFLAG_LEFT_ARW = 1;

		internal static readonly int WNDFLAG_RIGHT_ARW = 2;

		internal static readonly int WNDFLAG_BOX = 4;

		internal static readonly int WNDRET_TOP;

		internal static readonly int WNDRET_RIGHT = 1;

		internal static readonly int WNDRET_BOTTOM = 2;

		internal static readonly int WNDRET_LEFT = 3;

		internal static readonly int WNDRET_BOX_TOP = 4;

		internal static readonly int WNDRET_BOX_RIGHT = 5;

		internal static readonly int WNDRET_BOX_BOTTOM = 6;

		internal static readonly int WNDRET_BOX_LEFT = 7;

		internal static readonly int WNDRET_BOX_END = 8;

		private static int fontSize_;

		internal MyFormBase(int id)
			: base(id)
		{
		}

		public override bool Init()
		{
			app_ = IApplication.GetCurrentApp();
			apdat_ = AppData.GetInstance();
			canvas_ = Canvas.GetInstance();
			formMan_ = FormManager.GetInstance();
			view_ = SurfaceManager.view_;
			pad_ = SurfaceManager.gpad_;
			result_ = -1;
			canvas_.DecideKeyState();
			TouchEffectManager.GetInstance().RemoveSurface(view_);
			return true;
		}

		public override bool Update()
		{
			update_ = (update_ + 1) % int.MaxValue;
			return true;
		}

		public override void Draw(Graphics g)
		{
			draw_ = (draw_ + 1) % int.MaxValue;
		}

		public override bool Resume()
		{
			return true;
		}

		public override bool Finish()
		{
			TouchEffectManager.GetInstance().RemoveSurface(view_);
			return true;
		}

		public static string Yen(long value)
		{
			return Yen(value, true);
		}

		public static string Yen(long value, bool cs)
		{
			return Unit(value, cs) + "円";
		}

		public static string Manyen(long value)
		{
			return Yen(value, true);
		}

		public static string Manyen(long value, bool omit)
		{
			return Yen(value, true);
		}

		public static string Manyen(long value, bool omit, bool cs)
		{
			if (omit)
			{
				return Unit(value, cs) + "万";
			}
			return Unit(value, cs) + "万円";
		}

		public static string Unit(long value, bool cs)
		{
			bool flag = value < 0;
			string text = JString.ValueOf(JMath.Abs(value));
			if (cs)
			{
				string str = text;
				text = string.Empty;
				int num = 0;
				int num2 = str.Length() - 1;
				while (num2 >= 0)
				{
					if (num > 0 && num % 3 == 0)
					{
						text = ',' + text;
					}
					text = str.CharAt(num2) + text;
					num2--;
					num++;
				}
			}
			if (flag)
			{
				text = "-" + text;
			}
			return text;
		}

		internal static string Comma(long value)
		{
			return Unit(value, true);
		}

		internal string Format(string str, int len)
		{
			return StringUtil.FormatString(str, len);
		}

		internal string Format(string str, int len, char padding)
		{
			return StringUtil.FormatString(str, len, padding);
		}

		internal string Format(int v, int len)
		{
			return StringUtil.FormatString(JString.ValueOf(v), len);
		}

		internal string Format(int v, int len, char padding)
		{
			return StringUtil.FormatString(JString.ValueOf(v), len, padding);
		}

		internal string Format(long v, int len)
		{
			return StringUtil.FormatString(JString.ValueOf(v), len);
		}

		internal string Format(long v, int len, char padding)
		{
			return StringUtil.FormatString(JString.ValueOf(v), len, padding);
		}

		public static string Replace(string text, string arg)
		{
			return StringUtil.Replace(text, arg);
		}

		public string Replace(string text, string oldStr, string newStr)
		{
			return StringUtil.Replace(text, oldStr, newStr);
		}

		internal bool Active()
		{
			FormManager instance = FormManager.GetInstance();
			return this == instance.GetTopForm() && app_.Focus();
		}

		internal bool Push(string text)
		{
			return Push(SubForm.MakeDialog(text));
		}

		internal bool Push(string text, string cmds)
		{
			dialog_ = SubForm.MakeDialog(text, cmds);
			return Push(dialog_);
		}

		internal bool Push(string text, string cmds, int defaultSelect)
		{
			dialog_ = SubForm.MakeDialog(text, cmds, defaultSelect);
			return Push(dialog_);
		}

		internal bool Pop(int result)
		{
			result_ = result;
			return Pop();
		}

		internal bool Check(int flag, int bit)
		{
			return (flag & bit) != 0;
		}

		internal static bool Jp()
		{
			return Language.Japanese();
		}

		internal static bool En()
		{
			return Language.English();
		}

		public override int GetTouchValue(int type, int index)
		{
			UserData instance = UserData.GetInstance();
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 65536:
				return 0;
			case 393216:
				return apdat_.save_.ints[12];
			case 458752:
				return apdat_.save_.ints[13];
			case 524288:
				return apdat_.sysSave_.ints[3];
			case 589824:
				return apdat_.sysSave_.ints[4];
			case 655360:
				return apdat_.sysSave_.ints[14];
			case 720896:
				return apdat_.sysSave_.ints[13];
			case 131072:
				return apdat_.frmSoccer_.select_;
			case 262144:
				return apdat_.frmSoccer_.scrollValue_;
			case 786432:
				return instance.et_cur[0];
			case 851968:
				return instance.et_cur[1];
			case 917504:
				return instance.et_cur[2];
			case 983040:
				return instance.et_cur[3];
			case 1048576:
				return instance.et_cur[4];
			case 1114112:
				return instance.et_cur[5];
			case 1179648:
				return instance.et_cur[6];
			case 1441792:
				return 0;
			case 1507328:
				return 0;
			default:
				if (Config.PRINT)
				{
					throw new ArgumentException("getTouchValue() : not found value type (" + type + ")");
				}
				return 0;
			}
		}

		public override void SetTouchValue(int type, int value)
		{
			SoundPlayer instance = SoundPlayer.GetInstance();
			UserData instance2 = UserData.GetInstance();
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 65536:
				canvas_.KeyClick(1 << value);
				break;
			case 393216:
				apdat_.save_.ints[12] = value;
				canvas_.SetLinearFilterEnable(value == 1);
				formMan_.DrawAllForms();
				break;
			case 458752:
				apdat_.save_.ints[13] = value;
				break;
			case 524288:
				if (apdat_.sysSave_.ints[3] != value)
				{
					apdat_.sysSave_.ints[3] = value;
					instance.SetVolume(0, apdat_.sysSave_.ints[3] * 51);
					instance.SetMute(0, apdat_.sysSave_.ints[3] == 0);
					instance.ReflectVolumeAndMute();
				}
				break;
			case 589824:
				if (apdat_.sysSave_.ints[4] != value)
				{
					apdat_.sysSave_.ints[4] = value;
					instance.SetVolume(1, apdat_.sysSave_.ints[4] * 51);
					instance.SetMute(1, apdat_.sysSave_.ints[4] == 0);
					instance.ReflectVolumeAndMute();
				}
				break;
			case 655360:
				apdat_.sysSave_.ints[14] = value;
				apdat_.frmGame_.offYear_ = -1;
				apdat_.frmGame_.offMonth_ = -1;
				apdat_.frmGame_.offHour_ = -1;
				apdat_.frmGame_.offMoney_ = -1L;
				apdat_.frmGame_.offSeason_ = -1;
				formMan_.DrawAllForms();
				apdat_.frmGame_.SetAllDraw();
				break;
			case 720896:
				if (!Config.ADMOB_ENABLE)
				{
					apdat_.sysSave_.ints[13] = value;
					if (value == -1)
					{
						app_.SetOrientation(-1);
					}
					else
					{
						app_.SetOrientation(app_.GetOrientation());
					}
					formMan_.DrawAllForms();
					if (canvas_.GetSurfaceManager() != null)
					{
						canvas_.GetSurfaceManager().CheckScreenRotation(true);
					}
				}
				break;
			case 786432:
				instance2.et_cur[0] = value;
				break;
			case 851968:
			{
				int num = instance2.et_cur[1];
				instance2.et_cur[1] = value;
				if (instance2.et_cur[1] >= instance2.editTest_formation.Size())
				{
					instance2.et_cur[1] = instance2.editTest_formation.Size() - 1;
				}
				if (num != instance2.et_cur[1])
				{
					instance2.SetEditForPos(instance2.isSoccerForm_);
					FormationData formationData = (FormationData)instance2.editTest_formation.ElementAt(instance2.et_cur[1]);
					if (BitUtil.Check(formationData.flag_, 2))
					{
						FormBase topForm = formMan_.GetTopForm();
						topForm.SetSoftLabel(AppData.softLabels_[12], AppData.softLabels_[2]);
					}
					else
					{
						FormBase topForm2 = formMan_.GetTopForm();
						topForm2.SetSoftLabel(AppData.softLabels_[13], AppData.softLabels_[2]);
					}
				}
				break;
			}
			case 917504:
				instance2.et_cur[2] = value;
				break;
			case 983040:
				instance2.et_cur[3] = value;
				break;
			case 1048576:
				instance2.et_cur[4] = value;
				break;
			case 1114112:
				instance2.et_cur[5] = value;
				break;
			case 1179648:
				instance2.et_cur[6] = value;
				break;
			case 131072:
				apdat_.frmSoccer_.select_ = value;
				break;
			case 262144:
				apdat_.frmSoccer_.scrollValue_ = value;
				break;
			case 1441792:
			case 1507328:
				break;
			default:
				if (Config.PRINT)
				{
					throw new ArgumentException("setTouchValue() : not found value type (" + type + ")");
				}
				break;
			}
		}

		public virtual bool OnTouchEvent(TouchEvent e)
		{
			return true;
		}

		public int Tv(int type, int value)
		{
			return type | ((short)value & 0xFFFF);
		}

		internal int[] DrawWindow(Graphics g, int width, int height, string title, int sideSpace, int topSpace, int bottomSpace, int barArwSpace, int flag)
		{
			return DrawWindow2(g, width, height, title, sideSpace, topSpace, bottomSpace, barArwSpace, flag, 0);
		}

		internal int[] DrawWindow2(Graphics g, int width, int height, string title, int sideSpace, int topSpace, int bottomSpace, int barArwSpace, int flag, int ay)
		{
			int num = 16;
			if (drawWindow_tmp == null)
			{
				drawWindow_tmp = new int[WNDRET_BOX_END];
			}
			apdat_.DrawWindow(g, width, height, 0, title, -12 + ay);
			if (apdat_.IsTopForm(this))
			{
				arwFrame_ = (arwFrame_ + 1) % int.MaxValue;
			}
			drawWindow_tmp[WNDRET_TOP] = 120 - height / 2 + num;
			drawWindow_tmp[WNDRET_LEFT] = 120 - width / 2;
			drawWindow_tmp[WNDRET_RIGHT] = drawWindow_tmp[WNDRET_LEFT] + width;
			drawWindow_tmp[WNDRET_BOTTOM] = drawWindow_tmp[WNDRET_TOP] + height - num;
			drawWindow_tmp[WNDRET_BOX_LEFT] = drawWindow_tmp[WNDRET_LEFT] + sideSpace;
			drawWindow_tmp[WNDRET_BOX_RIGHT] = drawWindow_tmp[WNDRET_RIGHT] - sideSpace;
			drawWindow_tmp[WNDRET_BOX_TOP] = drawWindow_tmp[WNDRET_TOP] + topSpace;
			drawWindow_tmp[WNDRET_BOX_BOTTOM] = drawWindow_tmp[WNDRET_BOTTOM] - bottomSpace;
			if (BitUtil.Check(flag, WNDFLAG_BOX))
			{
				apdat_.DrawBox(g, drawWindow_tmp[WNDRET_BOX_LEFT], drawWindow_tmp[WNDRET_BOX_TOP] - 12 + ay, drawWindow_tmp[WNDRET_BOX_RIGHT], drawWindow_tmp[WNDRET_BOX_BOTTOM] - 12 + ay);
			}
			return drawWindow_tmp;
		}

		public static int GetBoxHeaderY(int boxTop)
		{
			return boxTop - 16;
		}

		public static int GetBoxFooterY(int boxBottom)
		{
			return boxBottom + 4;
		}

		public static string LT(string text)
		{
			return Language.TranslateText(text);
		}

		public static string LT(string text, string arg)
		{
			return Replace(Language.TranslateText(text), arg);
		}

		public static string LT(string text, long arg)
		{
			return Replace(Language.TranslateText(text), JString.ValueOf(arg));
		}

		public TextFormat LF(string format)
		{
			return TextFormat.Create(format);
		}

		public static string YEN(string text, long value)
		{
			return YEN(text, value, true);
		}

		public static string YEN(string text, long value, bool comma)
		{
			if (Language.GetLanguageCode() != "ja-JP" && Language.GetLanguageCode() != "en-US" && !Language.GetLanguageCode().StartsWith("*"))
			{
				value *= 10;
				return ((comma) ? (Comma(value)) : ((string.Empty + value))) + "G";
			}
			bool flag = AppData.GetInstance().sysSave_.ints[14] == 1;
			if (text.Equals("万円") || text.Equals("万"))
			{
				if (flag)
				{
					return "$" + ((comma) ? (Comma(value / 10)) : ((string.Empty + value / 10))) + "." + JMath.Abs(value) % 10 + "K";
				}
				if (!Jp())
				{
					return "¥" + ((comma) ? (Comma(value * 10)) : ((string.Empty + value * 10))) + "K";
				}
			}
			else if (text.Equals("円"))
			{
				if (flag)
				{
					return "$" + ((comma) ? (Comma(value / 100)) : ((string.Empty + value / 100)));
				}
				if (!Jp())
				{
					return "¥" + ((comma) ? (Comma(value)) : ((string.Empty + value)));
				}
			}
			else if (text.Equals("\\"))
			{
				if (flag)
				{
					return "$" + ((comma) ? (Comma(value / 100)) : ((string.Empty + value / 100)));
				}
				if (!Jp())
				{
					return "¥" + ((comma) ? (Comma(value)) : ((string.Empty + value)));
				}
				return "\\" + ((comma) ? (Comma(value)) : ((string.Empty + value)));
			}
			if (flag)
			{
				return ((comma) ? (Comma(value / 100)) : ((string.Empty + value / 100))) + text;
			}
			return ((comma) ? (Comma(value)) : ((string.Empty + value))) + text;
		}

		public static string UNI(string text, long value)
		{
			return UNI(text, value, true);
		}

		public static string UNI(string text, long value, bool comma)
		{
			string text2 = "K";
			if (Jp())
			{
				return JString.ValueOf((comma) ? (Comma(value)) : ((string.Empty + value))) + text;
			}
			if (text.Contains("万"))
			{
				return ((comma) ? (Comma(value * 10)) : ((string.Empty + value * 10))) + "k";
			}
			return JString.ValueOf((comma) ? (Comma(value)) : ((string.Empty + value)));
		}

		public static void PushFontSize(Graphics g, int size)
		{
			if (En())
			{
				Font font = g.GetFont(null);
				fontSize_ = font.GetSize();
				font.SetSize(size);
				g.SetFont(font);
			}
		}

		public static void BackFontSize(Graphics g)
		{
			if (En())
			{
				Font font = g.GetFont(null);
				font.SetSize(fontSize_);
				g.SetFont(font);
			}
		}
	}
}
