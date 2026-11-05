using System;
using System.Text;
using form;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.ad;
using kairo.unity.form;
using kairo.unity.native;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace surface
{
	public class GamePad : SurfaceBase
	{
		internal const int TEX_BG_SHOW = 0;

		internal const int TEX_BG_HIDE1 = 1;

		internal const int TEX_BG_HIDE2_TOP = 2;

		internal const int TEX_BG_HIDE2_BOTTOM = 3;

		internal const int TEX_BUTTON = 4;

		internal const int TEX_BUTTON_L = 5;

		internal const int TEX_BUTTON_H = 6;

		internal const int TEX_SBUTTON = 7;

		internal const int TEX_SBUTTON_L = 8;

		internal const int TEX_SBUTTON_H = 9;

		internal const int TEX_UP_L = 10;

		internal const int TEX_DOWN_L = 11;

		internal const int TEX_LEFT_L = 12;

		internal const int TEX_RIGHT_L = 13;

		internal const int TEX_SELECT_L = 14;

		internal const int TEX_KAIRO1 = 15;

		internal const int TEX_KAIRO2 = 16;

		internal const int TEX_COPYLIGHT = 17;

		internal const int TEX_COPYLIGHT_EN = 18;

		internal const int TEX_PAD_ON = 19;

		internal const int TEX_PAD_OFF = 20;

		internal const int TEX_SOFT1_FRAME = 21;

		internal const int TEX_SOFT2_FRAME = 22;

		internal const int TEX_AM = 23;

		internal const int TEX_PM = 24;

		internal const int TEX_0 = 25;

		internal const int TEX_1 = 26;

		internal const int TEX_2 = 27;

		internal const int TEX_3 = 28;

		internal const int TEX_4 = 29;

		internal const int TEX_5 = 30;

		internal const int TEX_6 = 31;

		internal const int TEX_7 = 32;

		internal const int TEX_8 = 33;

		internal const int TEX_9 = 34;

		internal const int TEX_COLON = 35;

		internal const int TEX_BATTERY0 = 36;

		internal const int TEX_BATTERY1 = 37;

		internal const int TEX_BATTERY2 = 38;

		internal const int TEX_BATTERY3 = 39;

		internal const int TEX_BATTERY4 = 40;

		internal const int TEX_BATTERY5 = 41;

		internal const int TEX_BATTERY6 = 42;

		internal const int TEX_BATTERY7 = 43;

		internal const int TEX_BATTERY8 = 44;

		internal const int TEX_BATTERY9 = 45;

		internal const int TEX_BATTERY10 = 46;

		internal const int TEX_BATTERY11 = 47;

		internal const int TEX_BATTERY12 = 48;

		internal const int TEX_BATTERY13 = 49;

		internal const int TEX_BATTERY_CHARGE = 50;

		internal const int T_IMG = 0;

		internal const int T_X = 1;

		internal const int T_Y = 2;

		internal const int T_U = 3;

		internal const int T_V = 4;

		internal const int T_W = 5;

		internal const int T_H = 6;

		internal const int ID_SOFT1 = 0;

		internal const int ID_SOFT2 = 1;

		internal const int ID_SOFT3 = 2;

		internal const int ID_SOFT4 = 3;

		internal const int ID_SOFTM = 4;

		internal const int ID_WEB = 5;

		internal const int ID_UP = 6;

		internal const int ID_DOWN = 7;

		internal const int ID_LEFT = 8;

		internal const int ID_RIGHT = 9;

		internal const int ID_SELECT = 10;

		internal const int ID_DEBUG = 11;

		internal const int ID_SKIP = 12;

		internal const int ID_DIALOG = 13;

		internal const int ID_HITTEST = 14;

		internal const int ID_SLOW = 15;

		internal const int ID_OPTION = 16;

		internal const int ID_LAYOUT = 17;

		internal const int ID_PAUSE = 18;

		internal const int ID_CAPTCHA = 19;

		internal const int ID_HIDE = 20;

		internal const int ID_PAD_ON = 21;

		internal const int ID_PAD_OFF = 22;

		internal const int C_L = 0;

		internal const int C_R = 1;

		internal const int C_T = 2;

		internal const int C_B = 3;

		internal const int C_TEX = 4;

		internal const int C_TEX_ON = 5;

		internal const int C_TEX_HIDE = 6;

		public const int MODE_FULL = 0;

		public const int MODE_PLAIN = 1;

		public const int MODE_OVERLAP = 2;

		public const int SOFT_W = 82;

		internal static GamePad instance_;

		internal IApplication app_;

		internal Canvas canvas_;

		internal TouchEffectManager effMan_;

		internal static Image[] images_;

		internal static readonly string[] FILENAMES = new string[3] { "keypad01.png", "keypad02.png", "battery.png" };

		internal static readonly int[] IMAGE_WIDTH = new int[3] { 512, 512, 128 };

		internal static readonly int[][] TEXTURES = new int[51][]
		{
			new int[7] { 0, 0, 0, 0, 0, 320, 160 },
			new int[7] { 1, 0, 0, 0, 0, 320, 65 },
			new int[7] { 1, 0, 0, 0, 67, 320, 50 },
			new int[7] { 1, 0, 0, 0, 118, 320, 90 },
			new int[7] { 0, 0, 0, 1, 161, 73, 37 },
			new int[7] { 0, -5, -7, 75, 161, 81, 47 },
			new int[7] { 0, -1, 1, 237, 161, 73, 34 },
			new int[7] { 0, 0, 0, 0, 198, 75, 20 },
			new int[7] { 0, -5, -2, 320, 230, 82, 22 },
			new int[7] { 0, -1, 0, 237, 196, 73, 17 },
			new int[7] { 0, 0, 0, 322, 1, 109, 57 },
			new int[7] { 0, 0, 0, 322, 59, 109, 57 },
			new int[7] { 0, 0, 0, 322, 117, 56, 109 },
			new int[7] { 0, 0, 0, 379, 117, 56, 109 },
			new int[7] { 0, 0, 0, 157, 161, 78, 78 },
			new int[7] { 0, 0, 0, 432, 13, 21, 19 },
			new int[7] { 0, 0, 0, 454, 13, 20, 19 },
			new int[7] { 0, 0, 0, 432, 0, 52, 12 },
			new int[7] { 0, 0, 0, 432, 53, 52, 12 },
			new int[7] { 0, 0, 0, 432, 65, 22, 12 },
			new int[7] { 0, 0, 0, 453, 65, 22, 12 },
			new int[7] { 1, -1, 0, 320, 0, 85, 46 },
			new int[7] { 1, 1, 0, 320, 46, 85, 46 },
			new int[7] { 2, 0, 0, 106, 1, 10, 5 },
			new int[7] { 2, 0, 0, 106, 7, 10, 5 },
			new int[7] { 2, 0, 0, 2, 1, 8, 10 },
			new int[7] { 2, 0, 0, 12, 1, 8, 10 },
			new int[7] { 2, 0, 0, 22, 1, 8, 10 },
			new int[7] { 2, 0, 0, 32, 1, 8, 10 },
			new int[7] { 2, 0, 0, 42, 1, 8, 10 },
			new int[7] { 2, 0, 0, 52, 1, 8, 10 },
			new int[7] { 2, 0, 0, 62, 1, 8, 10 },
			new int[7] { 2, 0, 0, 72, 1, 8, 10 },
			new int[7] { 2, 0, 0, 82, 1, 8, 10 },
			new int[7] { 2, 0, 0, 92, 1, 8, 10 },
			new int[7] { 2, 0, 0, 102, 1, 3, 10 },
			new int[7] { 2, 0, 0, 0, 15, 21, 11 },
			new int[7] { 2, 0, 0, 21, 15, 21, 11 },
			new int[7] { 2, 0, 0, 42, 15, 21, 11 },
			new int[7] { 2, 0, 0, 63, 15, 21, 11 },
			new int[7] { 2, 0, 0, 84, 15, 21, 11 },
			new int[7] { 2, 0, 0, 0, 27, 21, 11 },
			new int[7] { 2, 0, 0, 21, 27, 21, 11 },
			new int[7] { 2, 0, 0, 42, 27, 21, 11 },
			new int[7] { 2, 0, 0, 63, 27, 21, 11 },
			new int[7] { 2, 0, 0, 84, 27, 21, 11 },
			new int[7] { 2, 0, 0, 0, 39, 21, 11 },
			new int[7] { 2, 0, 0, 21, 39, 21, 11 },
			new int[7] { 2, 0, 0, 42, 39, 21, 11 },
			new int[7] { 2, 0, 0, 63, 39, 21, 11 },
			new int[7] { 2, 0, 0, 84, 39, 21, 11 }
		};

		internal static readonly int[][] COMPONENTS = new int[23][]
		{
			new int[7] { 10, -7, 10, 10, 4, 5, 6 },
			new int[7] { -6, 10, 10, 10, 4, 5, 6 },
			new int[7] { 10, -7, 10, 10, 4, 5, 6 },
			new int[7] { -6, 10, 10, 10, 4, 5, 6 },
			new int[7] { 10, 10, 10, 10, 4, 5, 6 },
			new int[7] { 10, -7, 10, 10, 4, 5, 6 },
			new int[7] { 25, 29, 0, -9, 10, 10, -1 },
			new int[7] { 27, 27, -5, 20, 11, 11, -1 },
			new int[7] { 30, -8, 27, 27, 12, 12, -1 },
			new int[7] { -7, 30, 25, 26, 13, 13, -1 },
			new int[7] { -16, -15, -16, -15, 14, 14, -1 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 1, 1, 0, 0, 7, 8, 9 },
			new int[7] { 10, 10, 10, 10, 7, 8, 9 },
			new int[7] { 10, 10, 10, 10, 7, 8, 9 }
		};

		internal Font font_;

		internal bool visibleTouchRange_;

		internal long webTouchBegin_;

		internal bool hideDebugButton_;

		internal long padTouchBegin_;

		internal int optionPage_;

		internal int optionPageMax_ = 2;

		internal int layoutMode_;

		internal bool hide_;

		public static int SIZE_FULL = 160;

		public static int SIZE_PLAIN = 65;

		public static int SIZE_PLAIN_MIN = 40;

		internal static readonly string[] WEB_NAMES = new string[3] { "ｻｲﾄ", "site", "web" };

		internal int mode_;

		public static bool versionVisible_ = true;

		private static JCalendar drawWatch_calendar;

		private static long drawWatch_rangeMin;

		private static long drawWatch_rangeMax;

		private float[] getEvenlyDivisionArea_tmp_ = new float[3];

		private GamePad()
		{
			app_ = IApplication.GetCurrentApp();
			canvas_ = Canvas.GetInstance();
			effMan_ = TouchEffectManager.GetInstance();
			float scaleRatio = app_.GetScaleRatio(true);
			float num = 240f * scaleRatio / 100f;
			float num2 = 320f;
			SetScaleRatio(num * 100f / num2);
			font_ = new Font();
			font_.SetSize(14);
			mode_ = 1;
			int num3 = JInteger.ParseInt(KairoPlugin.GetReleaseNumber().SubstringJ(0, 1));
			if (num3 == 3)
			{
				COMPONENTS[8][0] = 0;
				COMPONENTS[9][1] = 0;
			}
		}

		public static void Load(JarInflater jar)
		{
			try
			{
				images_ = new Image[FILENAMES.Length];
				for (int i = 0; i < images_.Length; i++)
				{
					if (jar.GetSize(FILENAMES[i]) != -1)
					{
						images_[i] = Image.Load(jar.GetData(FILENAMES[i]));
					}
				}
				int width = images_[0].GetWidth();
				if (width != 384 && width != 512 && width != 576 && width != 768)
				{
					IMAGE_WIDTH[0] = 484;
					IMAGE_WIDTH[1] = 405;
				}
			}
			catch (Exception)
			{
			}
		}

		public static GamePad GetInstance()
		{
			if (instance_ == null)
			{
				instance_ = new GamePad();
			}
			return instance_;
		}

		public int GetMode()
		{
			return mode_;
		}

		public void SetMode(int mode)
		{
			mode_ = mode;
		}

		public override bool IsGameSurface()
		{
			return false;
		}

		private void AddTouch(Graphics g, float x, float y, int id)
		{
			AddTouch(g, x, y, id, 0, true);
		}

		private void AddTouch(Graphics g, float x, float y, int id, int value)
		{
			AddTouch(g, x, y, id, value, true);
		}

		private void AddTouch(Graphics g, float x, float y, int id, int value, bool enable)
		{
			int[] array = COMPONENTS[id];
			int[] array2 = TEXTURES[array[4]];
			int imgId = array[4];
			int num = 0;
			int num2 = 0;
			if (g != null)
			{
				num = g.GetOriginX();
				num2 = g.GetOriginY();
				x += (float)num;
				y += (float)num2;
				g.SetOrigin(0, 0);
			}
			try
			{
				if (g != null)
				{
					if (DrawSoftLabel(g, x, y, id, value, enable))
					{
						return;
					}
					switch (id)
					{
					case 6:
					case 7:
					case 8:
					case 9:
					case 10:
						if (enable && CheckFirstTouch(id) && array[5] != -1)
						{
							DrawImage(g, x, y, array[5]);
						}
						break;
					default:
						if (enable)
						{
							DrawImage(g, x, y, imgId);
							if (CheckFirstTouch(id) && array[5] != -1)
							{
								DrawImage(g, x, y, array[5]);
							}
							string text = null;
							switch (id)
							{
							case 0:
							case 5:
								text = canvas_.GetSoftLabelL();
								break;
							case 1:
								text = canvas_.GetSoftLabelR();
								break;
							case 2:
								text = canvas_.GetSoftLabel(2);
								break;
							case 3:
								text = canvas_.GetSoftLabel(3);
								break;
							case 11:
								text = "debug";
								break;
							case 12:
								text = "skip";
								break;
							case 13:
								text = "dialog";
								break;
							case 14:
								text = "hittest";
								break;
							case 15:
								text = "slow";
								break;
							case 16:
								text = optionPage_ + 1 + " / " + optionPageMax_;
								break;
							case 17:
								text = "layout";
								break;
							case 18:
								text = ((!FormManager.GetInstance().IsPause()) ? "pause" : "play");
								break;
							case 19:
								text = "captcha";
								break;
							case 20:
								text = "hide";
								break;
							case 4:
								text = canvas_.GetSoftLabel(2);
								if (text == null)
								{
									text = canvas_.GetSoftLabel(3);
								}
								break;
							}
							switch (id)
							{
							case 21:
								DrawImage(g, x + 26f, y + 4f, 19);
								break;
							case 22:
								DrawImage(g, x + 26f, y + 4f, 20);
								break;
							}
							if (text != null)
							{
								float num3 = x + (float)(array2[5] + array2[1]) / 2f - 1f;
								float num4 = y + (float)(array2[6] + array2[2]) / 2f - 1f;
								if (id == 5)
								{
									bool linearFilterEnable = canvas_.IsLinearFilterEnable();
									canvas_.SetLinearFilterEnable(true);
									DrawImage(g, num3 - 27f, num4 - 10f, (JSystem.CurrentTimeMillis() % 800 >= 400) ? 16 : 15);
									canvas_.SetLinearFilterEnable(linearFilterEnable);
									num3 += 12f;
								}
								if (array[4] == 7)
								{
									Font font = g.GetFont(null);
									font.SetSize(8);
									g.PushFont(font);
								}
								g.SetColor(0, 0, 0);
								g.DrawText(Language.TranslateSoftLabel(text), num3 - 30f, num4, 60f, 34);
								if (array[4] == 7)
								{
									g.PopFont();
								}
							}
						}
						else
						{
							if (id != 0 && id != 1 && id != 2 && id != 3 && id == 4 && id != 21 && id != 22 && mode_ == 0)
							{
								break;
							}
							bool flag = array[6] != -1;
							if (flag)
							{
								switch (id)
								{
								case 2:
									flag = canvas_.GetSoftLabel(2) != null;
									break;
								case 3:
									flag = canvas_.GetSoftLabel(3) != null;
									break;
								case 4:
									flag = canvas_.GetSoftLabel(2) != null || canvas_.GetSoftLabel(3) != null;
									break;
								}
							}
							if (flag)
							{
								DrawImage(g, x, y, array[6]);
							}
						}
						break;
					}
				}
				if (enable && array[5] != -1)
				{
					array2 = TEXTURES[array[5]];
					float num5 = array2[5] + array[0] + array[1] - array2[1];
					float num6 = array2[6] + array[2] + array[3] - array2[2];
					float num7 = x - (float)array[0] + (float)array2[1];
					float num8 = y - (float)array[2] + (float)array2[2];
					TouchOption touchOption = null;
					if (id == 6 || id == 7 || id == 8 || id == 9 || id == 13)
					{
						touchOption = TouchOption.Create(1);
					}
					if (id == 12 && mode_ == 0)
					{
						touchOption = TouchOption.Create(1);
					}
					if (touchOption == null)
					{
						touchOption = TouchOption.Create();
					}
					if (mode_ != 2)
					{
						touchOption.Flag(touchOption.flag | 2);
					}
					string softLabelR = canvas_.GetSoftLabelR();
					FormBase topForm = FormManager.GetInstance().GetTopForm();
					if (mode_ == 2 && id == 1 && softLabelR.Equals(AppData.softLabels_[2]) && (topForm.GetId() == 4 || topForm.GetId() == 3))
					{
						num7 -= 10f;
						num5 += 10f;
						num8 -= 30f;
						num6 += 30f;
					}
					if (id == 1 && softLabelR.Equals(AppData.softLabels_[2]) && topForm.GetId() == 5)
					{
						num7 += 30f;
						num5 -= 30f;
					}
					TouchComponent touchComponent = AddTouchComponent(id, (int)num7, (int)num8, (int)num5, (int)num6, value, touchOption);
					if (touchComponent != null)
					{
						touchComponent.SetPaint((int)x, (int)y);
					}
				}
			}
			finally
			{
				if (g != null)
				{
					g.SetOrigin(num, num2);
				}
			}
		}

		protected override void OnTouchEvent(TouchEvent evt)
		{
			TouchComponent component = evt.component;
			FormManager instance = FormManager.GetInstance();
			int id = evt.component.id;
			int type = evt.type;
			switch (id)
			{
			case 6:
			case 7:
			case 8:
			case 9:
				if (type == 0)
				{
					canvas_.KeyDown(component.value);
				}
				if (type == 6 || type == 10)
				{
					canvas_.KeyUp(component.value);
				}
				break;
			case 12:
				if (type == 0 || (type == 13 && !canvas_.CheckKeyState(component.value) && CheckFirstTouch(id)))
				{
					canvas_.KeyDown(component.value);
				}
				if (type == 6 || type == 10)
				{
					canvas_.KeyUp(component.value);
				}
				break;
			case 13:
				if (type == 0 || (type == 13 && !canvas_.CheckKeyState(component.value) && CheckFirstTouch(id)))
				{
					canvas_.KeyDown(component.value);
				}
				if (type == 6 || type == 10)
				{
					canvas_.KeyUp(component.value);
				}
				break;
			case 14:
				if (type == 5)
				{
					visibleTouchRange_ = !visibleTouchRange_;
				}
				break;
			case 15:
				if (type == 0 || (type == 13 && !canvas_.CheckKeyState(component.value) && CheckFirstTouch(id)))
				{
					canvas_.KeyDown(component.value);
				}
				if (type == 6 || type == 10)
				{
					canvas_.KeyUp(component.value);
				}
				break;
			case 21:
			case 22:
				if (Config.DEBUG && !Config.PTW)
				{
					if (type == 0)
					{
						padTouchBegin_ = JSystem.CurrentTimeMillis();
					}
					if (type == 13 && padTouchBegin_ > 0 && CheckFirstTouch(id) && JSystem.CurrentTimeMillis() - padTouchBegin_ > 1000)
					{
						padTouchBegin_ = 0L;
						hideDebugButton_ = !hideDebugButton_;
						ClearTouchComponent();
						break;
					}
					if (type == 6)
					{
						padTouchBegin_ = 0L;
					}
				}
				if (type == 5)
				{
					AppData instance2 = AppData.GetInstance();
					int num = ((id != 21) ? 1 : 0);
					SetMode(num);
					instance2.sysSave_.ints[15] = num;
					try
					{
						instance2.SaveSystem();
					}
					catch (Exception)
					{
					}
				}
				break;
			case 5:
			{
				if (type == 5)
				{
					canvas_.KeyClick(component.value);
				}
				FormBase topForm = instance.GetTopForm();
				if (type == 0)
				{
					webTouchBegin_ = JSystem.CurrentTimeMillis();
				}
				if (type == 13 && webTouchBegin_ > 0 && CheckFirstTouch(id) && topForm.GetId() == 1 && JSystem.CurrentTimeMillis() - webTouchBegin_ > 5000)
				{
					webTouchBegin_ = 0L;
					instance.Push(new SpForm());
				}
				if (type == 10 || type == 6)
				{
					webTouchBegin_ = 0L;
				}
				break;
			}
			case 16:
				if (type == 5)
				{
					optionPage_ = (optionPage_ + 1) % optionPageMax_;
				}
				break;
			case 17:
				if (type == 5)
				{
					layoutMode_ = (layoutMode_ + 1) % 2;
					SetLayoutMode(layoutMode_);
				}
				break;
			case 18:
				if (type == 0)
				{
					instance.SetPause(!instance.IsPause());
					TouchEffectManager.GetInstance().Clear();
				}
				break;
			case 19:
				if (type == 5)
				{
					instance.Capture(GameView.GetInstance());
				}
				break;
			case 20:
				if (type == 5)
				{
					hide_ = !hide_;
				}
				break;
			default:
				if (type == 5)
				{
					canvas_.KeyClick(component.value);
				}
				break;
			}
			if (type == 5 && component.paintX != int.MaxValue && COMPONENTS[id][5] != -1 && id != 21 && id != 22)
			{
				int[] array = TEXTURES[COMPONENTS[id][5]];
				Image image = images_[array[0]];
				float num2 = (float)image.GetWidth() / (float)IMAGE_WIDTH[array[0]];
				object[] arg = new object[2]
				{
					images_[array[0]],
					new float[6]
					{
						array[5],
						array[6],
						(float)array[3] * num2,
						(float)array[4] * num2,
						(float)array[5] * num2,
						(float)array[6] * num2
					}
				};
				effMan_.AddEffect(this, 0, component.paintX + array[1], component.paintY + array[2], component, arg);
			}
		}

		public override void Draw(Graphics g)
		{
			SurfaceBase view_ = SurfaceManager.view_;
			FormManager instance = FormManager.GetInstance();
			Font font = g.GetFont(null);
			try
			{
				Font.PushLocalizeSetting(false);
				g.SetRenderRect(GetX(), GetY(), GetWidth(), GetHeight());
				g.Scale(100f);
				g.SetFont(font_);
				bool flag = true;
				if (Config.ADMOB_ENABLE)
				{
					Canvas instance2 = Canvas.GetInstance();
					IApplication currentApp = IApplication.GetCurrentApp();
					int num = instance2.GetHeight() - AdMobView.GetAdHeight();
					float scaleRatio = currentApp.GetScaleRatio(false);
					int num2 = (int)(240f * scaleRatio / 100f);
					flag = (float)(num - num2) >= (float)SIZE_FULL * GetScaleRatio() / 100f;
				}
				bool flag2 = false;
				string softLabelL = canvas_.GetSoftLabelL();
				for (int i = 0; i < WEB_NAMES.Length; i++)
				{
					if (WEB_NAMES[i].EqualsIgnoreCase(softLabelL))
					{
						flag2 = true;
						break;
					}
				}
				int num3 = (int)((float)(GetWidth() * 100) / GetScaleRatio());
				int num4 = (int)((float)(GetHeight() * 100) / GetScaleRatio());
				int num5 = view_.GetY() - GetY();
				int num6 = GetY() + GetHeight() - (view_.GetY() + view_.GetHeight());
				if (num5 > 0)
				{
					int num7 = TEXTURES[2][6];
					if (num7 <= num5)
					{
						DrawScaledImage(g, 0f, 0f, GetWidth(), num5, 2);
					}
					else
					{
						DrawScaledImage(g, 0f, num5 - num7, GetWidth(), num7, 2);
					}
				}
				if (mode_ == 0)
				{
					DrawScaledImage(g, 0f, (float)(GetHeight() - num6) - 0.5f, GetWidth(), num6, 0);
				}
				else if (mode_ == 1 && num6 > 0)
				{
					int num8 = TEXTURES[3][6];
					int num9 = GetHeight() - num6;
					if (num8 <= num6)
					{
						DrawScaledImage(g, 0f, num9 - 1, GetWidth(), num6 + 1, 3);
					}
					else
					{
						DrawScaledImage(g, 0f, num9 - 1, GetWidth(), num8 + 1, 3);
					}
				}
				g.Scale(GetScaleRatio());
				num5 = (int)((float)(num5 * 100) / GetScaleRatio());
				num6 = (int)((float)(num6 * 100) / GetScaleRatio());
				if (mode_ == 0)
				{
					g.SetOrigin(0, num4 - num6);
					AddTouch(g, 104f, 0f, 6, 131072);
					AddTouch(g, 106f, 107f, 7, 524288);
					AddTouch(g, 79f, 27f, 8, 65536);
					AddTouch(g, 186f, 25f, 9, 262144);
					AddTouch(g, 121f, 41f, 10, 1048576);
					if (Config.DEBUG && !instance.IsPause() && !hideDebugButton_ && !instance.IsCapturing())
					{
						if (Config.DEBUG)
						{
							AddTouch(g, 5f, 64f, 11, 1);
						}
						if (Config.DEBUG)
						{
							AddTouch(g, 5f, 90f, 19);
						}
						if (Config.DEBUG)
						{
							AddTouch(g, 243f, 110f, 12, 16);
						}
						if (!Config.PTW)
						{
							AddTouch(g, 243f, 90f, 13, 64);
						}
						if (!Config.PTW)
						{
							AddTouch(g, 243f, 64f, 14);
						}
					}
					if (flag2)
					{
						AddTouch(g, 5f, 7f, 5, 2097152);
						AddTouch(g, 243f, 7f, 1, 4194304, canvas_.GetSoftLabelR().Length() > 0);
						if (Language.Japanese())
						{
							DrawImage(g, 15f, 48f, 17);
						}
						else
						{
							DrawImage(g, 15f, 48f, 18);
						}
					}
					else
					{
						AddTouch(g, 5f, 7f, 0, 2097152, canvas_.GetSoftLabelL().Length() > 0);
						AddTouch(g, 243f, 7f, 1, 4194304, canvas_.GetSoftLabelR().Length() > 0);
					}
					DrawWatch(g, 320f, 143f);
					DrawBattery(g, 292f, 142f);
					AddTouch(g, 5f, num6 - 28, 22);
					DrawVersion(g);
				}
				else if (mode_ == 1)
				{
					g.SetOrigin(0, num4 - num6);
					int num10 = 7;
					if (Config.DEBUG)
					{
						num10 = 4;
					}
					if (flag2)
					{
						AddTouch(g, 5f, 7f, 5, 2097152);
						AddTouch(g, 243f, 7f, 1, 4194304, canvas_.GetSoftLabelR().Length() > 0);
						if (Language.Japanese())
						{
							DrawImage(g, 15f, 48f, 17);
						}
						else
						{
							DrawImage(g, 15f, 48f, 18);
						}
					}
					else
					{
						AddTouch(g, 5f, 7f, 0, 2097152, canvas_.GetSoftLabelL().Length() > 0);
						AddTouch(g, 243f, 7f, 1, 4194304, canvas_.GetSoftLabelR().Length() > 0);
					}
					if (Config.DEBUG && !instance.IsPause() && !hideDebugButton_ && !instance.IsCapturing())
					{
						AddTouch(g, 4f, num6 - 21, 19);
						AddTouch(g, 242f, num6 - 21, 12, 16);
					}
					float num11 = (float)Canvas.GetInstance().GetHeight() - 240f * view_.GetScaleRatio() / 100f - (float)(Config.ADMOB_ENABLE ? AdMobView.GetAdHeight() : 0);
					if (num11 >= (float)SIZE_FULL * GetScaleRatio() / 100f)
					{
						AddTouch(g, 122f, num6 - 28, 21);
					}
					DrawVersion(g);
				}
				else
				{
					if (mode_ != 2)
					{
						return;
					}
					int num12 = 0;
					int num13 = num3 - 85;
					int num14 = num4 - 46;
					if (IsSmallSideView())
					{
						num13 = num3 - 83;
					}
					else
					{
						DrawImage(g, num12, num14, 21);
						DrawImage(g, num13, num14, 22);
					}
					num12 += 3;
					num13 += 9;
					num14 += 7;
					if (flag2)
					{
						AddTouch(g, num12, num14, 5, 2097152);
						AddTouch(g, num13, num14, 1, 4194304, canvas_.GetSoftLabelR().Length() > 0);
						int num15 = 10;
						int num16 = -20;
						if (IsSmallSideView())
						{
							num15 = -2;
							num16 = -6;
						}
						if (Language.Japanese())
						{
							DrawImage(g, num12 + num15, num14 + num16, 17);
						}
						else
						{
							DrawImage(g, num12 + num15, num14 + num16, 18);
						}
					}
					else
					{
						AddTouch(g, num12, num14, 0, 2097152, canvas_.GetSoftLabelL().Length() > 0);
						AddTouch(g, num13, num14, 1, 4194304, canvas_.GetSoftLabelR().Length() > 0);
					}
					if (Config.DEBUG && !instance.IsPause() && !instance.IsCapturing())
					{
						AddTouch(g, num13, num14 - 30, 12, 16);
						AddTouch(g, num12, num14 - 30, 19);
					}
					FormBase topForm = instance.GetTopForm();
					if (topForm.GetId() == 4 && ((SubForm)topForm).IsMenu())
					{
						int num17 = 2;
						int num18 = -19;
						if (IsSmallSideView())
						{
							num17 = 24;
							num18 = -4;
						}
						DrawWatch(g, num3 + num17, num14 + num18);
						DrawBattery(g, num3 + 2 - 28, num14 - 20);
					}
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				g.Scale(100f);
				g.ClearRenderRect();
				Font.PopLocalizeSetting();
				g.SetFont(font);
			}
		}

		public bool IsSmallSideView()
		{
			return 0f < GetSideViewRatio();
		}

		public float GetSideViewRatio()
		{
			GameView instance = GameView.GetInstance();
			float num = (float)(instance.GetGameWidth() - 164) / 240f;
			if (GetMode() == 2 && num < 0.9f)
			{
				return num;
			}
			return 0f;
		}

		public int GetSideSize()
		{
			int num = 82;
			float sideViewRatio = GetSideViewRatio();
			if (0f < sideViewRatio)
			{
				num = (int)((float)num * sideViewRatio);
			}
			return num;
		}

		private bool DrawSoftLabel(Graphics g, float x, float y, int id, int value, bool enable)
		{
			if (!IsSmallSideView())
			{
				return false;
			}
			bool flag = x < (float)GameView.GetInstance().GetGameWidth();
			int[] array = COMPONENTS[id];
			int[] array2 = TEXTURES[array[4]];
			int imgId = array[4];
			string text = null;
			switch (id)
			{
			case 0:
			case 5:
				text = canvas_.GetSoftLabelL();
				break;
			case 1:
				text = canvas_.GetSoftLabelR();
				break;
			case 2:
				text = canvas_.GetSoftLabel(2);
				break;
			case 3:
				text = canvas_.GetSoftLabel(3);
				break;
			}
			if (text == null)
			{
				return false;
			}
			float num = x;
			float num2 = y;
			float sideViewRatio = GetSideViewRatio();
			int num3 = (int)((float)array2[5] * sideViewRatio);
			int num4 = (int)((float)array2[6] * sideViewRatio);
			if (!flag)
			{
				x += (float)(array2[5] - num3);
			}
			y += (float)(array2[6] - num4);
			switch (id)
			{
			case 0:
			case 5:
				DrawSoftLabelImage(g, x - 2f, y - 3f, 21);
				break;
			case 1:
				DrawSoftLabelImage(g, x - 6f, y - 3f, 22);
				break;
			}
			if (!enable && array[6] != -1)
			{
				DrawSoftLabelImage(g, x, y, array[6]);
				return true;
			}
			DrawSoftLabelImage(g, x, y, imgId);
			if (CheckFirstTouch(id) && array[5] != -1)
			{
				int[] array3 = TEXTURES[array[5]];
				int num5 = (int)((float)array3[5] * sideViewRatio);
				int num6 = (int)((float)array3[6] * sideViewRatio);
				if (!flag)
				{
					num += (float)(array3[5] - num5);
				}
				num2 += (float)(array3[6] - num6);
				DrawSoftLabelImage(g, num, num2, array[5]);
			}
			float num7 = x + (float)(array2[5] + array2[1]) * sideViewRatio / 2f - 1f;
			float num8 = y + (float)(array2[6] + array2[2]) * sideViewRatio / 2f - 1f;
			if (id == 5)
			{
				bool linearFilterEnable = canvas_.IsLinearFilterEnable();
				canvas_.SetLinearFilterEnable(true);
				DrawSoftLabelImage(g, num7 - 27f * sideViewRatio, num8 - 10f * sideViewRatio, (JSystem.CurrentTimeMillis() % 800 >= 400) ? 16 : 15);
				canvas_.SetLinearFilterEnable(linearFilterEnable);
				num7 += 12f * sideViewRatio;
			}
			g.PushFont((int)((float)g.GetFont(null).GetSize() * sideViewRatio));
			g.SetColor(0, 0, 0);
			g.DrawString(Language.TranslateSoftLabel(text), num7, num8, 34);
			g.PopFont();
			if (enable && array[5] != -1)
			{
				array2 = TEXTURES[array[5]];
				float num9 = array2[5] + array[0] + array[1] - array2[1];
				float num10 = array2[6] + array[2] + array[3] - array2[2];
				float num11 = x - (float)array[0] + (float)array2[1];
				float num12 = y - (float)array[2] + (float)array2[2];
				num9 *= sideViewRatio;
				float num13 = (float)array2[6] * (1f - sideViewRatio);
				num12 -= num13 / 2f;
				num10 += num13 / 2f;
				TouchComponent touchComponent = AddTouchComponent(id, (int)num11, (int)num12, (int)num9, (int)num10, value, null);
				if (touchComponent != null)
				{
					touchComponent.SetPaint((int)x, (int)y);
				}
			}
			return true;
		}

		private void DrawSoftLabelImage(Graphics g, float x, float y, int imgId)
		{
			int[] array = TEXTURES[imgId];
			Image image = images_[array[0]];
			float sideViewRatio = GetSideViewRatio();
			int num = (int)((float)array[5] * sideViewRatio);
			int num2 = (int)((float)array[6] * sideViewRatio);
			float num3 = (float)image.GetWidth() / (float)IMAGE_WIDTH[array[0]];
			g.DrawScaledImage(image, x + (float)array[1], y + (float)array[2], num, num2, (int)((float)array[3] * num3 + 0.5f), (int)((float)array[4] * num3 + 0.5f), (int)((float)array[5] * num3 + 0.5f), (int)((float)array[6] * num3 + 0.5f));
		}

		private bool IsSoftLabelID(int id)
		{
			if (id == 0 || id == 5 || id == 1 || id == 2 || id == 3)
			{
				return true;
			}
			return false;
		}

		public void DrawVersion(Graphics g)
		{
			if ((!Config.DEBUG || !FormManager.GetInstance().IsCapturing()) && versionVisible_)
			{
				int num = 7;
				int originX = g.GetOriginX();
				int originY = g.GetOriginY();
				string value = 202 + string.Empty;
				StringBuilder stringBuilder = new StringBuilder(value);
				stringBuilder.Insert(1, ".");
				string str = "ver " + stringBuilder.ToString();
				Font font = g.GetFont(null);
				g.SetFont(font);
				g.SetColor(255, 255, 255);
				g.DrawString(str, 277f, num + 38, 2);
				font.SetSize(0);
				g.SetFont(font);
			}
		}

		internal void DrawWatch(Graphics g, float x, float y)
		{
			if (drawWatch_calendar == null)
			{
				drawWatch_calendar = JCalendar.GetInstance();
			}
			JCalendar jCalendar = drawWatch_calendar;
			long num = JSystem.CurrentTimeMillis();
			if (num < drawWatch_rangeMin || drawWatch_rangeMax <= num)
			{
				jCalendar.SetTimeInMillis(num);
				int num2 = jCalendar.Get(13);
				int num3 = jCalendar.Get(14);
				drawWatch_rangeMin = num - num2 * 1000 - num3;
				drawWatch_rangeMax = drawWatch_rangeMin + 60000;
			}
			int num4 = jCalendar.Get(11);
			int num5 = jCalendar.Get(12);
			JCalendar instance = JCalendar.GetInstance();
			instance.SetTime(new JDate());
			string text = instance.Get(11).ToString();
			if (text == null || text.Equals("12"))
			{
				if (num4 < 12)
				{
					DrawImage(g, x - 81f, y, 23);
				}
				else
				{
					DrawImage(g, x - 81f, y + 5f, 24);
				}
				num4 %= 12;
			}
			for (int i = 0; i < 2; i++)
			{
				int num6 = num4 % 10;
				DrawImage(g, x - 63f + 2f - (float)(9 * i), y, 25 + num6);
				num4 /= 10;
				if (num4 <= 0)
				{
					break;
				}
			}
			DrawImage(g, x - 54f + 2f, y, 35);
			for (int j = 0; j < 2; j++)
			{
				int num7 = num5 % 10;
				DrawImage(g, x - 40f + 2f - (float)(9 * j), y, 25 + num7);
				num5 /= 10;
			}
		}

		internal void DrawBattery(Graphics g, float x, float y)
		{
			int num = 13;
			if (app_.IsBatteryCharging())
			{
				DrawImage(g, x, y, 50);
				return;
			}
			int num2 = app_.GetBattery() * num / 100;
			DrawImage(g, x, y, 36 + num2);
		}

		private void DrawImage(Graphics g, float x, float y, int imgId)
		{
			int[] array = TEXTURES[imgId];
			DrawScaledImage(g, x, y, array[5], array[6], imgId);
		}

		private void DrawScaledImage(Graphics g, float x, float y, float w, float h, int imgId)
		{
			int[] array = TEXTURES[imgId];
			Image image = images_[array[0]];
			float num = (float)image.GetWidth() / (float)IMAGE_WIDTH[array[0]];
			g.DrawScaledImage(image, x + (float)array[1], y + (float)array[2], w, h, (int)((float)array[3] * num + 0.5f), (int)((float)array[4] * num + 0.5f), (int)((float)array[5] * num + 0.5f), (int)((float)array[6] * num + 0.5f));
		}

		public override void RotateClear()
		{
		}

		private float[] GetDivisionArea(float left, float right, float padding, int index, int max)
		{
			index = AppData.ClampMax(index, max - 1);
			if (max == 1)
			{
				getEvenlyDivisionArea_tmp_[0] = left;
				getEvenlyDivisionArea_tmp_[1] = right;
			}
			else
			{
				float num = (right - left - padding * (float)(max - 1)) / (float)max;
				getEvenlyDivisionArea_tmp_[0] = left + (float)(int)((float)index * (num + padding) + 0.1f);
				getEvenlyDivisionArea_tmp_[1] = left + (float)(int)((float)index * (num + padding) + num + 0.1f);
			}
			getEvenlyDivisionArea_tmp_[2] = getEvenlyDivisionArea_tmp_[0] + (getEvenlyDivisionArea_tmp_[1] - getEvenlyDivisionArea_tmp_[0]) / 2f;
			return getEvenlyDivisionArea_tmp_;
		}

		public void SetLayoutMode(int mode)
		{
			SurfaceManager surfaceManager = (SurfaceManager)Canvas.GetInstance().GetSurfaceManager();
			switch (mode)
			{
			case 0:
				surfaceManager.SetLayoutMode(false);
				app_.SetScaleRatio(float.NaN);
				break;
			case 1:
				surfaceManager.SetLayoutMode(true);
				break;
			}
		}

		public int GetLayoutMode()
		{
			return layoutMode_;
		}

		public void SetOptionPage(int page)
		{
			optionPage_ = page;
		}

		public int GetOptionPage()
		{
			return optionPage_;
		}
	}
}
