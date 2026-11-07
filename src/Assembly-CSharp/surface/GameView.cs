using System;
using System.Collections.Generic;
using form;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;

namespace surface
{
	public class GameView : SurfaceBase
	{
		public const int TEX_LEFT_ARW = 0;

		public const int TEX_RIGHT_ARW = 1;

		public const int TEX_LEFT_ARW_L = 2;

		public const int TEX_RIGHT_ARW_L = 3;

		public const int TEX_MINUS = 4;

		public const int TEX_PLUS = 5;

		public const int TEX_MINUS_L = 6;

		public const int TEX_PLUS_L = 7;

		public const int TEX_MARKER_LEFT = 8;

		public const int TEX_MARKER_CENTER = 9;

		public const int TEX_MARKER_RIGHT = 10;

		public const int TEX_TRACK_BAR = 11;

		public const int TEX_NOTES = 12;

		public const int TEX_UP_ARW = 12;

		public const int TEX_DOWN_ARW = 13;

		public const int TEX_UP_ARW_L = 14;

		public const int TEX_DOWN_ARW_L = 15;

		internal const int T_IMG = 0;

		internal const int T_X = 1;

		internal const int T_Y = 2;

		internal const int T_U = 3;

		internal const int T_V = 4;

		internal const int T_W = 5;

		internal const int T_H = 6;

		public const int ID_TITLEMENU = 0;

		public const int ID_DLGPAGE = 1;

		public const int ID_DLGCMD = 2;

		public const int ID_DLGSEL = 3;

		public const int ID_KEYCLICK = 4;

		public const int ID_KEYPRESS = 5;

		public const int ID_KEYDOWN = 6;

		public const int ID_KEYUP = 7;

		public const int ID_MAINMENU = 8;

		public const int ID_SUBMENU = 9;

		public const int ID_LIST = 10;

		public const int ID_SLIST = 11;

		public const int ID_VSCROLLBAR = 12;

		public const int ID_VSCROLLBTN = 13;

		public const int ID_VSCROLLBAR2 = 14;

		public const int ID_VSCROLLBAR3 = 15;

		public const int ID_UNMARKER = 16;

		public const int ID_CAMERA = 17;

		public const int ID_ARROW = 18;

		public const int ID_TRACKBAR = 19;

		public const int ID_PLUS = 20;

		public const int ID_MINUS = 21;

		public const int ID_VSCROLLBAR4 = 22;

		public const int ID_SLIST2 = 23;

		public const int ID_ARROW_UD = 24;

		public const int ID_ARROW2 = 25;

		public const int ID_DLGPAGE2 = 26;

		public const int ID_RECT = 27;

		private const int TPARAM_LEFT = 0;

		private const int TPARAM_RIGHT = 1;

		private const int TPARAM_TOP = 2;

		private const int TPARAM_BOTTOM = 3;

		private const int TPARAM_COLLISION = 4;

		private const int TPARAM_SCREEN = 5;

		public const int VALUE_KEY = 65536;

		public const int VALUE_SELECT = 131072;

		public const int VALUE_SELMENU = 196608;

		public const int VALUE_SCROLL = 262144;

		public const int VALUE_BUILDCATEGORY = 327680;

		public const int VALUE_QUALITY = 393216;

		public const int VALUE_GAMESPEED = 458752;

		public const int VALUE_BGM_VOLUME = 524288;

		public const int VALUE_SE_VOLUME = 589824;

		public const int VALUE_CURRENCY = 655360;

		public const int VALUE_ORIENTATION = 720896;

		public const int VALUE_EDIT_MENU = 786432;

		public const int VALUE_EDIT_FORMENU = 851968;

		public const int VALUE_EDIT_FCHA = 917504;

		public const int VALUE_EDIT_FCHA2 = 983040;

		public const int VALUE_EDIT_FTYPE = 1048576;

		public const int VALUE_EDIT_MEMCHA = 1114112;

		public const int VALUE_EDIT_MEMCHA2 = 1179648;

		public const int VALUE_EDIT_FORMENU_SCROLL = 1245184;

		public const int VALUE_PAGE = 1310720;

		public const int VALUE_UDARROWCNT = 1376256;

		public const int VALUE_KAIROAD = 1441792;

		public const int VALUE_KAIROAD_CANCEL = 1507328;

		public const int KEYID_0 = 0;

		public const int KEYID_1 = 1;

		public const int KEYID_2 = 2;

		public const int KEYID_3 = 3;

		public const int KEYID_4 = 4;

		public const int KEYID_5 = 5;

		public const int KEYID_6 = 6;

		public const int KEYID_7 = 7;

		public const int KEYID_8 = 8;

		public const int KEYID_9 = 9;

		public const int KEYID_ASTERISK = 10;

		public const int KEYID_POUND = 11;

		public const int KEYID_LEFT = 16;

		public const int KEYID_UP = 17;

		public const int KEYID_RIGHT = 18;

		public const int KEYID_DOWN = 19;

		public const int KEYID_SELECT = 20;

		public const int KEYID_SOFT1 = 21;

		public const int KEYID_SOFT2 = 22;

		public const int KEYID_SOFT3 = 23;

		public const int KEYID_SOFT4 = 24;

		public const int KEYID_USER1 = 25;

		public const int KEYID_USER2 = 26;

		public const int KEYID_USER3 = 27;

		public const int KEYID_USER4 = 28;

		public const int KEYID_USER5 = 29;

		public const int KEYID_USER6 = 30;

		internal static GameView instance_;

		internal IApplication app_;

		internal Canvas canvas_;

		internal TouchEffectManager effMan_;

		internal FormManager formMan_;

		internal static Image[] images_;

		internal static readonly string[] FILENAMES = new string[2] { "buttoneffect.png", "notes.png" };

		internal static readonly int[] IMAGE_WIDTH = new int[2] { 128, 63 };

		public static readonly int[][] TEXTURES = new int[16][]
		{
			new int[7] { 0, -5, -8, 0, 57, 15, 20 },
			new int[7] { 0, -4, -8, 15, 57, 15, 20 },
			new int[7] { 0, -9, -12, 31, 57, 20, 26 },
			new int[7] { 0, -6, -12, 52, 57, 20, 26 },
			new int[7] { 0, -11, -11, 1, 85, 22, 21 },
			new int[7] { 0, -11, -11, 24, 85, 22, 21 },
			new int[7] { 0, -14, -14, 47, 85, 28, 28 },
			new int[7] { 0, -14, -14, 76, 85, 28, 28 },
			new int[7] { 0, 0, 0, 70, 1, 7, 26 },
			new int[7] { 0, 0, 0, 78, 1, 8, 26 },
			new int[7] { 0, 0, 0, 87, 1, 7, 26 },
			new int[7] { 0, -2, -13, 73, 57, 9, 23 },
			new int[7] { 0, -9, -7, 0, 3, 18, 14 },
			new int[7] { 0, -9, 15, 0, 18, 18, 14 },
			new int[7] { 0, -12, -10, 22, 0, 25, 17 },
			new int[7] { 0, -12, 15, 22, 21, 25, 17 }
		};

		internal Image render_;

		internal Graphics g_;

		internal static readonly int[][] TOUCHPARAMS = new int[28][]
		{
			new int[6] { 10, 10, 10, 10, 1, 1 },
			new int[6] { 120, 120, 120, 20, 1, 1 },
			new int[6] { 31, 31, 18, 18, 1, 1 },
			new int[6] { 16, 16, 8, 8, 1, 1 },
			new int[6] { 0, 0, 0, 0, 0, 1 },
			new int[6] { 0, 0, 0, 0, 1, 1 },
			new int[6] { 0, 0, 0, 0, 1, 1 },
			new int[6] { 0, 0, 0, 0, 1, 1 },
			new int[6] { 40, 40, 10, 10, 1, 1 },
			new int[6] { 40, 40, 10, 10, 1, 1 },
			new int[6] { 240, 240, 10, 10, 1, 1 },
			new int[6] { 80, -10, 3, 3, 1, 1 },
			new int[6] { 20, 40, 0, 0, 0, 1 },
			new int[6],
			new int[6] { 20, 40, 0, 0, 0, 1 },
			new int[6] { 20, 40, 0, 0, 0, 1 },
			new int[6] { 0, 0, 0, 0, 0, 1 },
			new int[6] { 0, 0, 0, 0, 0, 1 },
			new int[6] { 0, 0, 0, 0, 1, 1 },
			new int[6],
			new int[6] { 0, 0, 0, 0, 1, 1 },
			new int[6] { 0, 0, 0, 0, 1, 1 },
			new int[6] { 20, 20, 0, 0, 0, 1 },
			new int[6] { 0, 0, 0, 0, 1, 1 },
			new int[6] { 0, 0, 0, 0, 1, 1 },
			new int[6] { 0, 0, 0, 0, 1, 1 },
			new int[6] { 120, 120, 120, 20, 1, 1 },
			new int[6] { 0, 0, 0, 0, 1, 1 }
		};

		internal Font font_;

		internal long markComponentKey_;

		private int markComponentFrame_;

		private bool capture_;

		private string captureFile_;

		private int captureScale_;

		private string toastText_;

		private bool addScrollMargin_;

		private int notesFrame_;

		public long notesEffect_;

		internal Vector<int[]> delayPaintBuffer_ = new Vector<int[]>();

		private int loopFrame_;

		private static float[] GetImageRect_result = new float[4];

		private int[] drawVerticalScroll_tmp = new int[2];

		private GameView()
		{
			app_ = IApplication.GetCurrentApp();
			canvas_ = Canvas.GetInstance();
			effMan_ = TouchEffectManager.GetInstance();
			formMan_ = FormManager.GetInstance();
			IApplication currentApp = IApplication.GetCurrentApp();
			SetScaleRatio(currentApp.GetScaleRatio(false));
			font_ = new Font();
			markComponentKey_ = -1L;
			markComponentFrame_ = 0;
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
			}
			catch (Exception)
			{
			}
		}

		public static GameView GetInstance()
		{
			if (instance_ == null)
			{
				instance_ = new GameView();
			}
			return instance_;
		}

		public int GetSideSoftWidth()
		{
			GamePad instance = GamePad.GetInstance();
			if (instance.GetMode() == 2)
			{
				int sideSize = instance.GetSideSize();
				return (int)((float)sideSize * instance.GetScaleRatio() / GetScaleRatio() + 0.01f);
			}
			return 0;
		}

		public override bool IsGameSurface()
		{
			return true;
		}

		public override void BeginTouchRegist(Graphics g)
		{
			base.BeginTouchRegist(g);
		}

		public override void EndTouchRegist(Graphics g)
		{
			TouchComponent touchComponent = null;
			TouchComponent touchComponent2 = null;
			TouchComponent touchComponent3 = null;
			TouchComponent touchComponent4 = null;
			int componentLength = GetComponentLength();
			for (int i = 0; i < componentLength; i++)
			{
				TouchComponent component = GetComponent(i);
				if (component.id == 12 || component.id == 15)
				{
					touchComponent3 = component;
				}
				else if (component.id == 11)
				{
					int num = BitUtil.Split(component.value)[1];
					if (touchComponent == null || BitUtil.Split(touchComponent.value)[1] > num)
					{
						touchComponent = component;
					}
					if (touchComponent2 == null || BitUtil.Split(touchComponent2.value)[1] < num)
					{
						touchComponent2 = component;
					}
				}
				if (component.Check(16))
				{
					touchComponent4 = component;
				}
			}
			List<TouchComponent> list = new List<TouchComponent>();
			List<TouchComponent> list2 = new List<TouchComponent>();
			for (int j = 0; j < componentLength; j++)
			{
				TouchComponent component2 = GetComponent(j);
				if (component2.id == 11)
				{
					if (touchComponent != null && touchComponent.y == component2.y)
					{
						list.Add(component2);
					}
					if (touchComponent2 != null && touchComponent2.y + touchComponent2.height == component2.y + component2.height)
					{
						list2.Add(component2);
					}
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				list[k].y -= 10;
				list[k].height += 10;
			}
			for (int l = 0; l < list2.Count; l++)
			{
				list2[l].height += 10;
			}
			int num2 = int.MaxValue;
			int num3 = int.MinValue;
			for (int m = 0; m < list.Count; m++)
			{
				if (list[m].x < num2)
				{
					num2 = list[m].x;
				}
				if (list[m].x + list[m].width > num3)
				{
					num3 = list[m].x + list[m].width;
				}
			}
			for (int n = 0; n < list2.Count; n++)
			{
				if (list2[n].x < num2)
				{
					num2 = list2[n].x;
				}
				if (list2[n].x + list2[n].width > num3)
				{
					num3 = list2[n].x + list2[n].width;
				}
			}
			if (touchComponent != null && touchComponent3 != null)
			{
				int[] array = new int[touchComponent3.option.args.Length];
				JSystem.Arraycopy(touchComponent3.option.args, 0, array, 0, array.Length);
				bool flag = false;
				bool flag2 = false;
				for (int num4 = 0; num4 < list.Count; num4++)
				{
					if (CheckTouch(11, list[num4].value))
					{
						flag = true;
					}
				}
				for (int num5 = 0; num5 < list2.Count; num5++)
				{
					if (CheckTouch(11, list2[num5].value))
					{
						flag2 = true;
					}
				}
				TouchOption option = TouchOption.Create(array);
				if (flag || CheckTouch(13, touchComponent3.value | 0))
				{
					AddTouchComponent(13, num2, 0, num3 - num2, touchComponent.y, touchComponent3.value | 0, option);
				}
				if (flag2 || CheckTouch(13, touchComponent3.value | 1))
				{
					AddTouchComponent(13, num2, touchComponent2.y + touchComponent2.height, num3 - num2, 240, touchComponent3.value | 1, option);
				}
			}
			if (touchComponent3 != null)
			{
				int num6 = touchComponent3.option.args[0];
				int num7 = touchComponent3.option.args[1];
				if (num6 <= num7)
				{
					touchComponent3.option.flag |= 2;
					touchComponent3.x += 20;
					for (int num8 = 0; num8 < componentLength; num8++)
					{
						TouchComponent component3 = GetComponent(num8);
						if (component3.id == 11)
						{
							component3.width += 20;
						}
					}
				}
			}
			if (touchComponent4 != null)
			{
				AddTouchComponent(16, 0, 0, GetGameWidth(), GetGameHeight(), 0, TouchOption.Create(1));
			}
			int originX = g.GetOriginX();
			int originY = g.GetOriginY();
			float scale = g.GetScale();
			g.SetOrigin((int)((float)(GetX() * 100) / app_.GetScaleRatio(false) + 0.01f), (int)((float)(GetY() * 100) / app_.GetScaleRatio(false) + 0.01f));
			g.Scale(GetScaleRatio() + 1f);
			for (int num9 = 0; num9 < delayPaintBuffer_.Size(); num9++)
			{
				int[] array2 = delayPaintBuffer_.ElementAt(num9);
				g.Scale((float)array2[array2.Length - 1] / 100f);
				if (array2[0] == 0)
				{
					int num10 = 6;
					int num11 = 2;
					int num12 = 2;
					int num13 = 3;
					int num14 = array2[1];
					int num15 = array2[2];
					int num16 = array2[3];
					int num17 = array2[4];
					if (array2[3] > 30 && array2[4] > 30)
					{
						num12 = 2;
						num13 = 3;
						num14 += 4;
						num15 += 3;
						num16 -= 8;
						num17 -= 6;
						num11 = 3;
					}
					int num18 = markComponentFrame_ / num11 % num10;
					int num19;
					if ((num18 == 0)) { num19 = 255; } else if ((num18 == 1)) { num19 = 224; } else if ((num18 == 2)) { num19 = 192; } else if ((num18 == 3)) { num19 = 160; } else if ((num18 == 4)) { num19 = 128; } else { num19 = 96; }
					int num20 = num14 + num12;
					int num21 = num15 - num13;
					int num22 = num16 - num12 * 2;
					int num23 = num17 + num13 * 2;
					g.SetRenderMode(1, num19, 255 - num19);
					DrawScaledImage(g, num20 - 5, num21, 5f, num23, 8);
					DrawScaledImage(g, num20, num21, num22, num23, 9);
					DrawScaledImage(g, num20 + num22, num21, 5f, num23, 10);
					if (num18 > 0)
					{
						DrawScaledImage(g, num20 - 5 - num18, num21 - num18, 5f, num23 + num18 * 2, 8);
						DrawScaledImage(g, num20 - num18, num21 - num18, num22 + num18 * 2, num23 + num18 * 2, 9);
						DrawScaledImage(g, num20 + num22 + num18, num21 - num18, 5f, num23 + num18 * 2, 10);
					}
					g.SetRenderMode(0, 255, 0);
					markComponentFrame_ = (markComponentFrame_ + 1) % int.MaxValue;
				}
			}
			delayPaintBuffer_.RemoveAllElements();
			loopFrame_ = (loopFrame_ + 1) % int.MaxValue;
			g.Scale(scale);
			g.SetOrigin(originX, originY);
			base.EndTouchRegist(g);
		}

		public override void DoEnd()
		{
			base.DoEnd();
		}

		public static TouchComponent AddTouch(int id)
		{
			return instance_._addTouch(null, id, 0, 0, instance_.GetGameWidth(), instance_.GetGameHeight(), 0, null);
		}

		public static TouchComponent AddTouch(int id, int value)
		{
			return instance_._addTouch(null, id, 0, 0, instance_.GetGameWidth(), instance_.GetGameHeight(), value, null);
		}

		public static TouchComponent AddTouch(int id, TouchOption opt)
		{
			return instance_._addTouch(null, id, 0, 0, instance_.GetGameWidth(), instance_.GetGameHeight(), 0, opt);
		}

		public static TouchComponent AddTouch(int id, int value, TouchOption opt)
		{
			return instance_._addTouch(null, id, 0, 0, instance_.GetGameWidth(), instance_.GetGameHeight(), value, opt);
		}

		public static TouchComponent AddTouch(Graphics g, int id, int x, int y, int w, int h)
		{
			return instance_._addTouch(g, id, x, y, w, h, 0, null);
		}

		public static TouchComponent AddTouch(Graphics g, int id, int x, int y, int w, int h, int value)
		{
			return instance_._addTouch(g, id, x, y, w, h, value, null);
		}

		public static TouchComponent AddTouch(Graphics g, int id, int x, int y, int w, int h, int value, TouchOption opt)
		{
			return instance_._addTouch(g, id, x, y, w, h, value, opt);
		}

		public static TouchComponent AddTouch(Graphics g, int id, Image img, int x, int y, int u, int v, int w, int h)
		{
			return AddTouch(g, id, img, x, y, u, v, w, h, 0);
		}

		public static TouchComponent AddTouch(Graphics g, int id, Image img, int x, int y, int u, int v, int w, int h, int value)
		{
			return AddTouch(g, id, img, x, y, u, v, w, h, value);
		}

		public static TouchComponent AddTouch(Graphics g, int id, Image img, int x, int y, int u, int v, int w, int h, int value, TouchOption opt)
		{
			g.DrawImage(img, x, y, u, v, w, h);
			TouchComponent touchComponent = instance_._addTouch(g, id, x, y, w, h, value, opt);
			if (touchComponent == null)
			{
				return null;
			}
			if (g == null)
			{
				touchComponent.SetPaint(x, y);
			}
			else
			{
				touchComponent.SetPaint(g.GetOriginX() + x, g.GetOriginY() + y);
			}
			return touchComponent;
		}

		public static TouchComponent AddTouch(Graphics g, int id, int x, int y, Seb seb, int frame)
		{
			return AddTouch(g, id, x, y, seb, frame, 0, null);
		}

		public static TouchComponent AddTouch(Graphics g, int id, int x, int y, Seb seb, int frame, int value)
		{
			return AddTouch(g, id, x, y, seb, frame, value, null);
		}

		public static TouchComponent AddTouch(Graphics g, int id, int x, int y, Seb seb, int frame, int value, TouchOption opt)
		{
			short[] boundingRect = seb.GetBoundingRect(frame);
			int num = x + boundingRect[0];
			int num2 = y + boundingRect[1];
			int num3 = boundingRect[2];
			int num4 = boundingRect[3];
			if (g != null)
			{
				switch(id){case 1:case 18:case 26:{
					int num5 = -1;
					int num6 = ((frame==0 || frame==3)?3:1);
					if(instance_.CheckTouch(id,value)){num5=((frame==0 || frame==3)?2:3);}else{num5=((frame==0 || frame==3)?0:1);}
					float[] imageRect = GetImageRect(num5);
					x += (int)((float)(boundingRect[0] + boundingRect[2] / 2) - (imageRect[0] + imageRect[2] / 2f));
					y += (int)((float)(boundingRect[1] + boundingRect[3] / 2) - (imageRect[1] + imageRect[3] / 2f));
					num = (int)((float)x + imageRect[0]);
					num2 = (int)((float)y + imageRect[1]);
					num3 = (int)imageRect[2];
					num4 = (int)imageRect[3];
					if (id == 18)
					{
						if ((num6 == 3)) {
num -= 30;
							num2 -= 10;
							num3 += 40;
							num4 += 20;

}
else if ((num6 == 1)) {
num -= 10;
							num2 -= 10;
							num3 += 40;
							num4 += 20;

}
					}
					instance_.DrawImage(g, x, y, num5);
				break;}}
			}
			TouchComponent touchComponent = instance_._addTouch(g, id, num, num2, num3, num4, value, opt);
			if (touchComponent == null)
			{
				return null;
			}
			if (g == null)
			{
				touchComponent.SetPaint(x, y, frame);
			}
			else
			{
				touchComponent.SetPaint(g.GetOriginX() + x, g.GetOriginY() + y, frame);
			}
			return touchComponent;
		}

		internal TouchComponent _addTouch(Graphics g, int id, int x, int y, int w, int h, int value, TouchOption opt)
		{
			if (id == 2)
			{
				if (opt == null)
				{
					opt = new TouchOption();
				}
				opt.Flag(opt.flag | 0x400);
			}
			if (g != null)
			{
				{
if ((id == 21)) {
if (CheckTouch(id, value))
					{
						DrawImage(g, x + w - 15, y + h / 2, 6);
					}
					else
					{
						DrawImage(g, x + w - 15, y + h / 2, 4);
					}

} else if ((id == 20)) {
if (CheckTouch(id, value))
					{
						DrawImage(g, x + 15, y + h / 2, 7);
					}
					else
					{
						DrawImage(g, x + 15, y + h / 2, 5);
					}

} else if ((id == 19)) {
					int num12 = y + (h - 4) / 2;
					g.SetColor(30, 30, 30);
					g.FillRect(x + 1, num12, w - 2, 4f);
					g.FillRect(x, num12 + 1, w, 2f);
					int num13 = (w - 2) / (opt.dragCell[4] - 1);
					DrawImage(g, x + Get(value) * num13, num12 + 2, 11);
					x = x + Get(value) * num13 - 15;
					w = 32;

				} else if ((id == 12) || (id == 14) || (id == 15)) {
					int num = opt.args[0];
					int num2 = opt.args[1];
					bool flag = CheckFirstTouch(id, value);
					int[] array = null;
					if (num <= num2)
					{
						flag = false;
					}
					if (flag)
					{
						ClearMarker();
					}
					addScrollMargin_ = opt != null && (opt.flag & 0x200) != 0;
					if (id == 12 || id == 14)
					{
						array = DrawVerticalScroll(g, x, y, w, h, Get(value), num - 1, num2, flag, (id == 12) ? (0) : (1));
					}
					if (array != null) {
					if (flag && num - num2 > 0)
					{
						opt.flag |= 36;
						if (id == 15)
						{
							int num3 = x;
							int num4 = y;
							int num5 = w;
							int num6 = h;
							int num7 = 0;
							int num8 = 0;
							if (g != null)
							{
								num7 = g.GetOriginX();
								num8 = g.GetOriginY();
								num3 += num7;
								num4 += num8;
							}
							if (g != null && g.GetScale() != GetScaleRatio())
							{
								double num9 = (double)g.GetScale() / (double)GetScaleRatio();
								num3 = num7 + (int)((double)(num3 - num7) * num9);
								num4 = num8 + (int)((double)(num4 - num8) * num9);
								num5 = (int)((double)num5 * num9);
								num6 = (int)((double)num6 * num9);
								num3 += (int)((float)(g.GetRenderRect()[0] * 100) / GetScaleRatio() + 0.01f);
								num4 += (int)((float)(g.GetRenderRect()[1] * 100) / GetScaleRatio() + 0.01f);
							}
							opt.dragCell = new int[8]
							{
								num3,
								num4,
								num5,
								num6,
								0,
								num,
								Get(value),
								num2
							};
						}
						else
						{
							int num10 = x;
							int num11 = y;
							if (g != null)
							{
								num10 += g.GetOriginX();
								num11 += g.GetOriginY();
							}
							if (num2 == 1)
							{
								opt.dragCell = new int[6] { num10, num11, w, h, 0, num };
							}
							else
							{
								opt.dragGridH = (float)(h - array[1]) / (float)(num - num2);
								if (opt.dragGridH <= 0f)
								{
									opt.dragGridH = 1f;
								}
							}
						}
						x -= 30;
						y = -1000;
						w = 125;
						h = 2000;
					}
					else
					{
						y = array[0];
						w = 5;
						h = array[1];
					}

				}}
dispatchExit: ;
}
			}
			if (formMan_.GetCurrentForm() != formMan_.GetTopForm())
			{
				FormBase topForm = formMan_.GetTopForm();
				FormBase currentForm = formMan_.GetCurrentForm();
				if (topForm.GetId() != currentForm.GetId() || topForm.GetId() != 4 || !((SubForm)topForm).IsMenu() || !((SubForm)currentForm).IsMenu())
				{
					return null;
				}
			}
			if (opt != null && (opt.flag & 0x40) != 0)
			{
				return null;
			}
			if (value < 0 && Config.PRINT)
			{
				Dialog dialog = new Dialog(1, "GameTouch._addTouch");
				dialog.SetText("valueの値が不正です(value=" + value + ")");
				dialog.Show();
				app_.Terminate();
				return null;
			}
			int num14 = 0;
			int num15 = 0;
			if (g != null)
			{
				num14 = g.GetOriginX();
				num15 = g.GetOriginY();
				x += num14;
				y += num15;
				g.SetOrigin(0, 0);
			}
			int x2 = x;
			int y2 = y;
			try
			{
				if (g != null && opt != null)
				{
					if ((opt.flag & 0x10) != 0 && CheckMarker(id, value) && !IsTouch())
					{
						AddDelayPaintBuffer(g, new int[5]
						{
							0,
							x + g.GetOriginX(),
							y + g.GetOriginY(),
							w,
							h
						});
					}
					int color = g.GetColor();
					int colorOfRGB = Graphics.GetColorOfRGB(239, 154, 22);
					if ((opt.flag & 0x100) != 0 && (value == 0 || Get(value) != (value & 0xFFFF)))
					{
						g.SetColor(colorOfRGB);
						int num16 = (int)(96.0 * JMath.Sin(JMath.ToRadians(loopFrame_ % 45 * 4)));
						g.SetRenderMode(1, num16, 255 - num16);
						if (opt.args != null)
						{
							g.FillRect(opt.args[0] + num14, opt.args[1] + num15, opt.args[2], opt.args[3]);
						}
						else
						{
							g.FillRect(x, y, w, h);
						}
						g.SetRenderMode();
					}
					if ((opt.flag & 0x80) != 0)
					{
						g.SetColor(colorOfRGB);
						int num17 = loopFrame_ / 12 % 2;
						int num18 = x + num17;
						int num19 = y + num17;
						int num20 = w - num17 * 2;
						int num21 = h - num17 * 2;
						g.DrawRect(num18, num19, num20, num21);
					}
					if ((opt.flag & 0x800) != 0 && CheckTouch(id, value))
					{
						g.SetColor(255, 255, 255);
						g.PushRenderMode(1, 64, 255);
						if (opt != null && opt.args != null)
						{
							g.FillRect(opt.args[0] + num14, opt.args[1] + num15, opt.args[2], opt.args[3]);
						}
						else
						{
							g.FillRect(x, y, w, h);
						}
						g.PopRenderMode();
					}
					g.SetColor(color);
				}
				if (!base.registTouch)
				{
					return null;
				}
				int[] array2 = TOUCHPARAMS[id];
				if (opt != null && opt.margin != null)
				{
					array2 = opt.margin;
				}
				int num22 = x - array2[0];
				int num23 = x + w + array2[1];
				int num24 = y - array2[2];
				int num25 = y + h + array2[3];
				switch (id)
				{
				case 8:
					if (formMan_.GetCurrentForm() != formMan_.GetTopForm())
					{
						num23 = x + w;
					}
					break;
				case 9:
					if (GetComponentId(8) != null)
					{
						num22 = x;
					}
					break;
				case 13:
				{
					int componentLength = GetComponentLength();
					for (int i = 0; i < componentLength; i++)
					{
						TouchComponent component = GetComponent(i);
						if (component.id != 2 && component.id != 3) { continue; } {
							int num26 = value & 0xFFFF;
							if (num26 == 0 && CheckHitRect(num22, num24, num23 - num22, num25 - num24, component.x, component.y, component.width, component.height))
							{
								num24 = component.y + component.height;
							}
							if (num26 == 1 && CheckHitRect(num22, num24, num23 - num22, num25 - num24, component.x, component.y, component.width, component.height))
							{
								num25 = component.y;
							}
						}
					}
					break;
				}
				case 12:
				case 14:
				case 15:
				case 22:
					if (CheckFirstTouch(id, value))
					{
						if (opt == null)
						{
							opt = new TouchOption();
						}
						opt.flag |= 8;
					}
					else
					{
						num24 -= 20;
						num25 += 20;
					}
					break;
				case 19:
					if (CheckFirstTouch(id, value))
					{
						opt.flag |= 8;
						num22 = 0;
						num24 = 0;
						num23 = GetGameWidth();
						num25 = GetGameHeight();
					}
					break;
				case 11:
					if (addScrollMargin_)
					{
						num23 += 120;
					}
					break;
				}
				if (g != null)
				{
				}
				if (TOUCHPARAMS[id][5] == 1 && (opt == null || (opt.flag & 0x20) == 0))
				{
					float num27 = ((g != null) ? g.GetScale() : GetScaleRatio());
					int num28 = (int)((float)(GetWidth() * 100) / num27 + 0.01f);
					int num29 = (int)((float)(GetHeight() * 100) / num27 + 0.01f);
					if (num22 < 0)
					{
						num22 = 0;
					}
					if (num24 < 0)
					{
						num24 = 0;
					}
					if (num23 > num28)
					{
						num23 = num28;
					}
					if (num25 > num29)
					{
						num25 = num29;
					}
				}
				x = num22;
				y = num24;
				w = num23 - num22;
				h = num25 - num24;
				if (g != null && g.GetScale() != GetScaleRatio())
				{
					double num30 = (double)g.GetScale() / (double)GetScaleRatio();
					x = num14 + (int)((double)(x - num14) * num30);
					y = num15 + (int)((double)(y - num15) * num30);
					w = (int)((double)w * num30);
					h = (int)((double)h * num30);
				}
				TouchComponent touchComponent = AddTouchComponent(id, x, y, w, h, value, opt);
				touchComponent.SetPaint(x2, y2);
				int num31 = 0;
				if (TOUCHPARAMS[id][4] == 1)
				{
					num31 |= 1 << id;
				}
				if (opt != null)
				{
					num31 |= opt.collisionBits;
				}
				int componentLength2 = GetComponentLength();
				for (int j = 0; j < componentLength2; j++)
				{
					TouchComponent component2 = GetComponent(j);
					if (component2 == touchComponent)
					{
						continue;
					}
					int num32 = 0;
					if (TOUCHPARAMS[component2.id][4] == 1)
					{
						num32 |= 1 << component2.id;
					}
					if (component2.option != null)
					{
						num32 |= component2.option.collisionBits;
					}
					if (((num32 & (1 << touchComponent.id)) == 0 && (num31 & (1 << component2.id)) == 0)) { continue; } if (touchComponent.GetPriority() != component2.GetPriority() || !CheckHitRect(touchComponent.x, touchComponent.y, touchComponent.width, touchComponent.height, component2.x, component2.y, component2.width, component2.height)) { continue; }
					int num33 = touchComponent.width - (component2.x - touchComponent.x);
					if (component2.x < touchComponent.x)
					{
						num33 = component2.width - (touchComponent.x - component2.x);
					}
					int num34 = touchComponent.height - (component2.y - touchComponent.y);
					if (component2.y < touchComponent.y)
					{
						num34 = component2.height - (touchComponent.y - component2.y);
					}
					bool flag2 = num33 < num34;
					if (touchComponent.x == component2.x && touchComponent.width == component2.width)
					{
						flag2 = false;
					}
					if (touchComponent.y == component2.y && touchComponent.height == component2.height)
					{
						flag2 = true;
					}
					TouchComponent touchComponent2 = touchComponent;
					TouchComponent touchComponent3 = component2;
					if (flag2)
					{
						if (touchComponent3.x < touchComponent2.x)
						{
							touchComponent2 = component2;
							touchComponent3 = touchComponent;
						}
						touchComponent2.width -= num33 / 2;
						num33 -= num33 / 2;
						touchComponent3.x += num33;
						touchComponent3.width -= num33;
					}
					else
					{
						if (touchComponent3.y < touchComponent2.y)
						{
							touchComponent2 = component2;
							touchComponent3 = touchComponent;
						}
						touchComponent2.height -= num34 / 2;
						num34 -= num34 / 2;
						touchComponent3.y += num34;
						touchComponent3.height -= num34;
					}
				}
				return touchComponent;
			}
			finally
			{
				if (g != null)
				{
					g.SetOrigin(num14, num15);
				}
			}
		}

		public static bool CheckMarker(long key)
		{
			if (instance_ == null)
			{
				return false;
			}
			return instance_.markComponentKey_ == key;
		}

		public static bool CheckMarker(int id, int value)
		{
			if (instance_ == null)
			{
				return false;
			}
			return instance_.markComponentKey_ == TouchComponent.GetKey(id, value);
		}

		public static void ClearMarker()
		{
			if (instance_ == null) { return; }
{
instance_.markComponentKey_ = -1L;
instance_.markComponentFrame_ = 0;
}
		}

		public override void OnKeyDown(int key)
		{
			ClearMarker();
		}

		internal void Set(int value, TouchComponent c)
		{
			Set(GetTouchValueAccessor(), (int)(value & 0xFFFF0000u), BitUtil.Split(value)[1], c);
		}

		internal void Set(TouchValueAccessor va, int value, TouchComponent c)
		{
			Set(va, (int)(value & 0xFFFF0000u), BitUtil.Split(value)[1], c);
		}

		internal void Set(int type, int value, TouchComponent c)
		{
			Set(GetTouchValueAccessor(), type, value, c);
		}

		internal void Set(TouchValueAccessor va, int type, int value, TouchComponent c)
		{
			type = (int)(type & 0xFFFF0000u);
			if (type != 0)
			{
				va.SetTouchValue(type, value);
			}
			if (c != null && c.option != null && c.option.extendValues != null)
			{
				int[] extendValues = c.option.extendValues;
				for (int i = 0; i < extendValues.Length; i++)
				{
					va.SetTouchValue((int)(extendValues[i] & 0xFFFF0000u), BitUtil.Split(extendValues[i])[1]);
				}
			}
		}

		internal int Get(int type)
		{
			return Get(GetTouchValueAccessor(), type);
		}

		internal int Get(TouchValueAccessor va, int type)
		{
			type = (int)(type & 0xFFFF0000u);
			return va.GetTouchValue(type, 0);
		}

		internal bool CheckHitRect(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2)
		{
			return x1 <= x2 + w2 && y1 <= y2 + h2 && x1 + w1 >= x2 && y1 + h1 >= y2;
		}

		internal int Join(int type, int v)
		{
			short high = (short)type;
			if (type < -32768 || 32767 < type)
			{
				high = (short)(type >> 16);
			}
			return BitUtil.Join(high, (short)v);
		}

		internal TouchValueAccessor GetTouchValueAccessor()
		{
			return formMan_.GetTopForm();
		}

		protected override void OnTouchEvent(TouchEvent e)
		{
TouchComponent component = e.component;
if (formMan_.GetTopForm() is MyFormBase) {
MyFormBase myFormBase = (MyFormBase)formMan_.GetTopForm();
if (!myFormBase.OnTouchEvent(e)) {
return;
}
}
switch (component.id) {
case 0:
OnTouchTitleMenu(e.type, component.value, e);
break;
case 1:
case 18:
OnTouchDlgPage(e.type, component.value, e);
break;
case 26:
OnTouchDlgPage2(e.type, component.value, e);
break;
case 24:
OnTouchDlgPage(e.type, component.value, e);
break;
case 2:
OnTouchDlgCmd(e.type, component.value, e);
break;
case 3:
OnTouchDlgSel(e.type, component.value, e);
break;
case 4:
OnTouchKeyClick(e.type, component.value, e);
break;
case 5:
OnTouchKeyPress(e.type, component.value, e);
break;
case 6:
OnTouchKeyDown(e.type, component.value, e);
break;
case 7:
OnTouchKeyUp(e.type, component.value, e);
break;
case 8:
OnTouchMainMenu(e.type, component.value, e);
break;
case 9:
OnTouchSubMenu(e.type, component.value, e);
break;
case 10:
OnTouchList(e.type, component.value, e);
break;
case 11:
case 23:
OnTouchScrollList(e.type, component.value, e);
break;
case 12:
case 14:
case 15:
case 22:
OnTouchVScrollBar(e.type, component.value, e);
break;
case 13:
OnTouchVScrollButton(e.type, component.value, e);
break;
case 16:
OnTouchUnMarker(e.type, component.value, e);
break;
case 17:
OnTouchCamera(e.type, component.value, e);
break;
case 19:
OnTouchTrackBar(e.type, component.value, e);
break;
case 20:
OnTouchKeyClick(e.type, component.value, e);
break;
case 21:
OnTouchKeyClick(e.type, component.value, e);
break;
case 27:
OnTouchRect(e.type, component.value, e);
break;
}
return;
}

		internal void OnTouchTitleMenu(int type, int v, TouchEvent e)
		{
			switch (type)
			{
			case 2:
				Set(196608, v, e.component);
				break;
			case 1:
				canvas_.KeyClick(GetKey(e.component, 1048576));
				break;
			}
		}

		internal void OnTouchDlgPage(int type, int v, TouchEvent e)
		{
			TouchComponent component = e.component;
			switch (type)
			{
			case 0:
				canvas_.KeyDown(GetKey(e.component, 1 << v));
				switch (v)
				{
				case 16:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 2, component);
					touchEffect.vx_ = -10f;
					break;
				}
				case 18:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 3, component);
					touchEffect.vx_ = 10f;
					break;
				}
				case 17:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 14, component);
					touchEffect.vy_ = -10f;
					break;
				}
				case 19:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 15, component);
					touchEffect.vy_ = 10f;
					break;
				}
				}
				break;
			case 6:
			case 10:
				canvas_.KeyUp(GetKey(e.component, 1 << v));
				break;
			}
		}

		protected void OnTouchDlgPage2(int type, int v, TouchEvent e)
		{
			TouchComponent component = e.component;
			switch (type)
			{
			case 5:
				canvas_.KeyClick(1 << v);
				switch (v)
				{
				case 16:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 2, component);
					touchEffect.vx_ = -10f;
					break;
				}
				case 18:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 3, component);
					touchEffect.vx_ = 10f;
					break;
				}
				case 17:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 14, component);
					touchEffect.vy_ = -10f;
					break;
				}
				case 19:
				{
					TouchEffect touchEffect = AddEffect(0, component.paintX, component.paintY, 15, component);
					touchEffect.vy_ = 10f;
					break;
				}
				}
				break;
			case 1:
			case 2:
				Set(v, e.component);
				if (type == 1)
				{
					if (component.Check(16) && markComponentKey_ != component.GetKey())
					{
						markComponentKey_ = e.component.GetKey();
					}
					else
					{
						canvas_.KeyClick(1048576);
					}
				}
				break;
			case 6:
			case 10:
				canvas_.KeyUp(GetKey(e.component, 1 << v));
				break;
			case 3:
			case 4:
			case 7:
			case 8:
			case 9:
				break;
			}
		}

		internal void OnTouchDlgCmd(int type, int v, TouchEvent e)
		{
			switch (type) {
case 1:
case 2:
{ Set(v, e.component);
if (type == 1)
				{
					canvas_.KeyClick(GetKey(e.component, 1048576));
				}
break; }
}
		}

		internal void OnTouchDlgSel(int type, int v, TouchEvent e)
		{
			TouchComponent component = e.component;
			switch (type) {
case 1:
case 2:

			Set(v, e.component);
			if (type == 1)
			{
				if (component.Check(16) && markComponentKey_ != component.GetKey())
				{
					markComponentKey_ = e.component.GetKey();
				}
				else
				{
					canvas_.KeyClick(GetKey(e.component, 1048576));
				}
			}
break;
}
		}

		internal void OnTouchKeyClick(int type, int v, TouchEvent e)
		{
			if (v == 0)
			{
				v = 20;
			}
			bool flag = BitUtil.Split(v)[0] == 0;
			switch (type) {
case 5:

if (flag)
				{
					canvas_.KeyClick(GetKey(e.component, 1 << v));
				}
				else
				{
					Set(v, e.component);
				}
break;
}
		}

		internal void OnTouchKeyPress(int type, int v, TouchEvent e)
		{
			if (v == 0)
			{
				v = 20;
			}
			bool flag = BitUtil.Split(v)[0] == 0;
			switch (type)
			{
			case 0:
				if (flag)
				{
					canvas_.KeyDown(GetKey(e.component, 1 << v), false);
				}
				else
				{
					Set(v, e.component);
				}
				break;
			case 6:
			case 10:
				if (flag)
				{
					canvas_.KeyUp(GetKey(e.component, 1 << v));
				}
				break;
			}
		}

		internal void OnTouchKeyDown(int type, int v, TouchEvent e)
		{
			if (v == 0)
			{
				v = 20;
			}
			bool flag = BitUtil.Split(v)[0] == 0;
			switch (type) {
case 0:

if (flag)
				{
					canvas_.KeyDown(GetKey(e.component, 1 << v));
				}
				else
				{
					Set(v, e.component);
				}
break;
}
		}

		internal void OnTouchKeyUp(int type, int v, TouchEvent e)
		{
			if (v == 0)
			{
				v = 20;
			}
			bool flag = BitUtil.Split(v)[0] == 0;
			switch (type) {
case 1:

if (flag)
				{
					canvas_.KeyDown(GetKey(e.component, 1 << v));
				}
				else
				{
					Set(v, e.component);
				}
break;
}
		}

		internal void OnTouchMainMenu(int type, int v, TouchEvent e)
		{
			FormBase va = null;
			for (int i = 0; i < formMan_.GetFormsNum(); i++)
			{
				FormBase form = formMan_.GetForm(i);
				if (form.GetId() == 4 && ((SubForm)form).type_ == 3)
				{
					va = form;
					break;
				}
			}
			switch (type) {
case 1:
case 2:

			if (GetComponentId(9) == null)
			{
				Set(v, e.component);
				if (type == 1)
				{
					canvas_.KeyClick(1048576);
				}
			}
			else if (Get(va, v) != (v & 0xFFFF))
			{
				formMan_.Pop(formMan_.GetTopForm());
				Set(va, v, e.component);
			}
break;
}
		}

		internal void OnTouchSubMenu(int type, int v, TouchEvent e)
		{
			switch (type) {
case 1:
case 2:
{ Set(v, e.component);
if (type == 1)
				{
					canvas_.KeyClick(GetKey(e.component, 1048576));
				}
break; }
}
		}

		internal void OnTouchList(int type, int v, TouchEvent e)
		{
			TouchComponent component = e.component;
			switch (type) {
case 1:
case 2:

			Set(v, e.component);
			if (type == 1)
			{
				if (component.Check(16) && markComponentKey_ != component.GetKey())
				{
					markComponentKey_ = e.component.GetKey();
				}
				else
				{
					canvas_.KeyClick(GetKey(e.component, 1048576));
				}
			}
break;
}
		}

		internal void OnTouchScrollList(int type, int v, TouchEvent e)
		{
			TouchComponent component = e.component;
			switch (type) {
case 1:
case 2:

			Set(v, e.component);
			if (type == 1)
			{
				if (component.Check(16) && markComponentKey_ != component.GetKey())
				{
					markComponentKey_ = e.component.GetKey();
				}
				else
				{
					canvas_.KeyClick(GetKey(e.component, 1048576));
				}
			}
break;
}
		}

		internal void OnTouchVScrollBar(int type, int v, TouchEvent e)
		{
			TouchComponent component = e.component;
			int[] args = e.component.option.args;
			int num = args[0];
			int num2 = args[1];
			int num3;
if (2 < args.Length) { num3 = args[2]; } else { num3 = (-1); }
			int num4 = int.MaxValue;
			switch (type)
			{
			case 8:
				num4 = Get(v) + e.dy;
				break;
			case 9:
				num4 = e.dy;
				break;
			}
			if (num4 == int.MaxValue)
			{
				return;
			}
			if (num4 > num - num2)
			{
				num4 = num - num2;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (Get(v) == num4)
			{
				return;
			}
			int num5 = num4 - Get(v);
			Set(v, num4, e.component);
			if (num3 == -1)
			{
				return;
			}
			int num6 = Get(num3);
			if (component.id == 14)
			{
				num6 += num5 * 2;
				if (num6 < 0)
				{
					num6 = 0;
				}
			}
			else
			{
				num6 += num5;
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num6 >= num)
				{
					num6 = num - 1;
				}
			}
			Set(num3, num6, e.component);
		}

		internal void OnTouchVScrollButton(int type, int v, TouchEvent e)
		{
			TouchOption option = e.component.option;
			int num = option.args[0];
			int num2 = option.args[1];
			int num3;
if (2 < option.args.Length) { num3 = option.args[2]; } else { num3 = (-1); }
			if (num3 == -1)
			{
				return;
			}
			int num4 = v & 0xFFFF;
			if (type == 13)
			{
				if (num4 == 0 && Get(num3) > 0)
				{
					Set(num3, Get(num3) - 1, e.component);
				}
				if (num4 == 1 && Get(num3) < num - 1)
				{
					Set(num3, Get(num3) + 1, e.component);
				}
				if (Get(num3) >= Get(v) + num2)
				{
					Set(v, Get(num3) - (num2 - 1), e.component);
				}
				if (Get(num3) < Get(v))
				{
					Set(v, Get(num3), e.component);
				}
				while (Get(v) > 0 && num < Get(v) + num2)
				{
					Set(v, Get(v) - 1, e.component);
				}
			}
		}

		internal void OnTouchUnMarker(int type, int v, TouchEvent e)
		{
			TouchEvent touchEvent = null;
			switch (type)
			{
			case 0:
				markComponentFrame_ = 0;
				if (markComponentKey_ != -1)
				{
					touchEvent = GetTouchEventKey(markComponentKey_, 0);
					if (touchEvent != null)
					{
						break;
					}
				}
				ClearMarker();
				break;
			case 7:
				if(markComponentKey_==-1){break;}
					touchEvent = GetTouchEventKey(markComponentKey_, 7);
					if (touchEvent == null)
					{
						ClearMarker();
					}

				break;
			case 1:
				if (markComponentKey_ == -1)
				{
					break;
				}
				touchEvent = GetTouchEventId(12, 1);
				if (touchEvent != null)
				{
					break;
				}
				touchEvent = GetTouchEventId(14, 1);
				if (touchEvent != null)
				{
					break;
				}
				touchEvent = GetTouchEventId(15, 1);
				if (touchEvent != null)
				{
					break;
				}
				touchEvent=GetTouchEventId(22,1);if(touchEvent!=null){break;}touchEvent=GetTouchEventKey(markComponentKey_,1);if(touchEvent!=null){break;}ClearMarker();
				break;
			}
		}

		internal void OnTouchCamera(int type, int v, TouchEvent e)
		{
		}

		internal void OnTouchTrackBar(int type, int v, TouchEvent e)
		{
			int typeAlias = type;
if ((typeAlias == 0) || (typeAlias != 9)) { } else {

Set(v, e.dx, e.component);
}
		}

		protected void OnTouchRect(int type, int v, TouchEvent e)
		{
			switch (type) {
case 1:
case 2:
{ Set(v, e.component);
if (type == 1)
				{
					canvas_.KeyClick(1048576);
				}
break; }
}
		}

		public override void Draw(Graphics g)
		{
			g.SetRenderRect(GetX(), GetY(), GetWidth(), GetHeight());
			g.Scale(GetScaleRatio());
			Font font = g.GetFont(null);
			try
			{
				g.SetFont(font_);
				formMan_.Draw(g);
			}
			finally
			{
				g.SetFont(font);
				g.Scale(100f);
				g.ClearRenderRect();
			}
			if (notesFrame_ != 0)
			{
				notesFrame_++;
			}
			if (!formMan_.IsPause() && !GamePad.GetInstance().hide_)
			{
				if (GamePad.GetInstance().visibleTouchRange_)
				{
					g.SetColor(255, 0, 0);
					g.DrawLine(GetX(), (float)GetY() + (float)lastTouchY * GetScaleRatio() / 100f, GetX() + GetWidth(), (float)GetY() + (float)lastTouchY * GetScaleRatio() / 100f);
					g.DrawLine((float)GetX() + (float)lastTouchX * GetScaleRatio() / 100f, GetY(), (float)GetX() + (float)lastTouchX * GetScaleRatio() / 100f, GetY() + GetHeight());
				}
				if (Config.SHOW_FPS)
				{
					g.SetColor(0, 0, 0);
					g.SetRenderMode(1, 128, 128);
					g.FillRect(GetX(), GetY(), 14f, 14f);
					g.SetRenderMode();
					g.SetColor(255, 255, 255);
					g.DrawString(formMan_.GetFps(), GetX() + 1, GetY() + 1);
					string text = JRuntime.GetRuntime().FreeMemory().ToString();
					int num = g.GetFont(null).StringWidth(text);
					g.SetColor(0, 0, 0);
					g.SetRenderMode(1, 128, 128);
					g.FillRect(GetX() + 15, GetY(), num + 2, 14f);
					g.SetRenderMode();
					g.SetColor(255, 255, 255);
					g.DrawString(text, GetX() + 15 + 1, GetY() + 1);
				}
			}
		}

		private void DrawImage(Graphics g, float x, float y, int imgId)
		{
			int[] array = TEXTURES[imgId];
			float num = 0.75f;
			DrawScaledImage(g, x, y, (float)array[5] * num, (float)array[6] * num, imgId);
		}

		private void DrawScaledImage(Graphics g, float x, float y, float w, float h, int imgId)
		{
			int[] array = TEXTURES[imgId];
			Image image = images_[array[0]];
			float num = (float)image.GetWidth() / (float)IMAGE_WIDTH[array[0]];
			float num2 = 0.75f;
			g.DrawScaledImage(image, x + (float)array[1] * num2, y + (float)array[2] * num2, w, h, (int)((float)array[3] * num + 0.5f), (int)((float)array[4] * num + 0.5f), (int)((float)array[5] * num + 0.5f), (int)((float)array[6] * num + 0.5f));
		}

		private static float[] GetImageRect(int imgId)
		{
			int[] array = TEXTURES[imgId];
			float num = 0.75f;
			float[] getImageRect_result = GetImageRect_result;
			getImageRect_result[0] = (float)array[1] * num;
			getImageRect_result[1] = (float)array[2] * num;
			getImageRect_result[2] = (float)array[5] * num;
			getImageRect_result[3] = (float)array[6] * num;
			return getImageRect_result;
		}

		public int[] DrawVerticalScroll(Graphics g, int x, int y, int w, int h, int value, int maximum, int largeChange, bool touch, int type)
		{
			if (maximum < largeChange - 1)
			{
				maximum = largeChange - 1;
			}
			if (type == 0)
			{
				x--;
				w += 2;
			}
			if (type == 0)
			{
				g.SetColor(Graphics.GetColorOfRGB(7, 5, 78));
				g.FillRect(x, y, w, h);
			}
			else
			{
				g.SetColor(Graphics.GetColorOfRGB(168, 95, 80));
				g.FillRect(x, y - 1, w + 2, h + 2);
				g.SetColor(Graphics.GetColorOfRGB(202, 150, 66));
				g.FillRect(x + w / 2, y, 1f, h - 2);
				g.SetColor(Graphics.GetColorOfRGB(202, 150, 66));
				g.FillRect(x, y, w, 1f);
				g.SetColor(Graphics.GetColorOfRGB(202, 150, 66));
				g.FillRect(x, y - 1 + h - 2, w, 1f);
			}
			if (maximum < 0)
			{
				maximum = 0;
			}
			int num = y + value * h / (maximum + 1);
			int num2 = largeChange * h / (maximum + 1);
			if (num + num2 > y + h)
			{
				num2 = h - num;
			}
			if (num2 <= 0)
			{
				return null;
			}
			int color = 0;
			int color2 = 0;
			if ((type == 0)) { if (touch)
				{
					color = Graphics.GetColorOfRGB(246, 129, 0);
					color2 = Graphics.GetColorOfRGB(246, 129, 0);
				}
				else
				{
					color = Graphics.GetColorOfRGB(48, 160, 255);
					color2 = Graphics.GetColorOfRGB(48, 160, 255);
				} } else if ((type == 1)) { if (touch)
				{
					color = Graphics.GetColorOfRGB(240, 130, 0);
					color2 = Graphics.GetColorOfRGB(240, 130, 0);
				}
				else
				{
					color = Graphics.GetColorOfRGB(243, 210, 114);
					color2 = Graphics.GetColorOfRGB(202, 150, 66);
				} }
			g.SetColor(color);
			g.FillRect(x, num, w, num2);
			g.SetColor(color2);
			g.FillRect(x, num, 1f, 1f);
			g.FillRect(x + w - 1, num, 1f, 1f);
			g.FillRect(x, num + num2 - 1, 1f, 1f);
			g.FillRect(x + w - 1, num + num2 - 1, 1f, 1f);
			drawVerticalScroll_tmp[0] = num;
			drawVerticalScroll_tmp[1] = num2;
			return drawVerticalScroll_tmp;
		}

		public TouchEffect AddEffect(int id, int x, int y, int imgId, TouchComponent c)
		{
			if (imgId == -1)
			{
				{ global::surface.TouchEffect recoveredReturn_49580 = effMan_.AddEffect(this, id, x, y, c, null); return recoveredReturn_49580; }
			}
			int[] array = TEXTURES[imgId];
			Image image = images_[array[0]];
			float num = (float)image.GetWidth() / (float)IMAGE_WIDTH[array[0]];
			float num2 = 0.75f;
			object[] arg = new object[2]
			{
				images_[array[0]],
				new float[6]
				{
					(float)array[5] * num2,
					(float)array[6] * num2,
					(float)array[3] * num,
					(float)array[4] * num,
					(float)array[5] * num,
					(float)array[6] * num
				}
			};
			{ global::surface.TouchEffect recoveredReturn_50165 = effMan_.AddEffect(this, id, (float)x + (float)array[1] * num2, (float)y + (float)array[2] * num2, c, arg); return recoveredReturn_50165; }
		}

		public override void RotateClear()
		{
			ClearMarker();
		}

		public override void OnReceiveTouchEvent(MotionEvent evt)
		{
			base.OnReceiveTouchEvent(evt);
		}

		private int GetKey(TouchComponent c, int defaultKey)
		{
			if (c.option != null && c.option.keys != null && c.option.keys.Length > 0)
			{
				return c.option.keys[0];
			}
			return defaultKey;
		}

		private void AddDelayPaintBuffer(Graphics g, int[] paint)
		{
			int[] array = new int[paint.Length + 1];
			Array.Copy(paint, 0, array, 0, paint.Length);
			array[array.Length - 1] = (int)(g.GetScale() * 100f);
			delayPaintBuffer_.Add(array);
		}
	}
}
