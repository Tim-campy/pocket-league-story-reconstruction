using System.Runtime.CompilerServices;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.ad;
using kairo.unity.form;
using kairo.unity.graphics;
using kairo.unity.native;
using kairo.unity.surface;
using kairo.unity.system;
using kairo.unity.system.appli;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public class Canvas
	{
		private const int KEYID_0 = 0;

		private const int KEYID_1 = 1;

		private const int KEYID_2 = 2;

		private const int KEYID_3 = 3;

		private const int KEYID_4 = 4;

		private const int KEYID_5 = 5;

		private const int KEYID_6 = 6;

		private const int KEYID_7 = 7;

		private const int KEYID_8 = 8;

		private const int KEYID_9 = 9;

		private const int KEYID_ASTERISK = 10;

		private const int KEYID_POUND = 11;

		private const int KEYID_LEFT = 16;

		private const int KEYID_UP = 17;

		private const int KEYID_RIGHT = 18;

		private const int KEYID_DOWN = 19;

		private const int KEYID_SELECT = 20;

		private const int KEYID_SOFT1 = 21;

		private const int KEYID_SOFT2 = 22;

		private const int KEYID_SOFT3 = 23;

		private const int KEYID_SOFT4 = 24;

		private const int KEYID_USER1 = 25;

		private const int KEYID_USER2 = 26;

		private const int KEYID_USER3 = 27;

		private const int KEYID_USER4 = 28;

		private const int KEYID_USER5 = 29;

		private const int KEYID_USER6 = 30;

		private static KeyCode[][] VALID_KEYS;

		private static bool[] onValidKeys_;

		public const int KEY_LEFT = 65536;

		public const int KEY_RIGHT = 262144;

		public const int KEY_UP = 131072;

		public const int KEY_DOWN = 524288;

		public const int KEY_SELECT = 1048576;

		public const int KEY_0 = 1;

		public const int KEY_1 = 2;

		public const int KEY_2 = 4;

		public const int KEY_3 = 8;

		public const int KEY_4 = 16;

		public const int KEY_5 = 32;

		public const int KEY_6 = 64;

		public const int KEY_7 = 128;

		public const int KEY_8 = 256;

		public const int KEY_9 = 512;

		public const int KEY_POUND = 2048;

		public const int KEY_ASTERISK = 1024;

		public const int KEY_SOFT1 = 2097152;

		public const int KEY_SOFT2 = 4194304;

		public const int KEY_SOFT3 = 8388608;

		public const int KEY_SOFT4 = 16777216;

		public const int KEY_USER1 = 33554432;

		public const int KEY_USER2 = 67108864;

		public const int KEY_USER3 = 134217728;

		public const int KEY_USER4 = 268435456;

		public const int KEY_USER5 = 536870912;

		public const int KEY_USER6 = 1073741824;

		public const int OP_REPL = 0;

		public const int OP_ADD = 1;

		public const int OP_SUB = 2;

		public const int SOFT_KEY_1 = 0;

		public const int SOFT_KEY_2 = 1;

		public const int SOFT_KEY_3 = 2;

		public const int SOFT_KEY_4 = 3;

		public const int TIMER_EXPIRED_EVENT = 0;

		public const int RESUME_VM_EVENT = 1;

		public const int KEY_RELEASED_EVENT = 2;

		public const int KEY_PRESSED_EVENT = 3;

		protected static Canvas instance_ = null;

		protected Graphics graphics_;

		protected int background_;

		internal Font defaultFont_;

		protected SurfaceManagerBase surfaceManager_;

		protected int statusBar_ = -1;

		protected int statusBarPosition_ = -1;

		protected AdMobView admobView_;

		protected int disableKeyBits_;

		protected Dictionary<int, float[]> onTouchPointer_ = new Dictionary<int, float[]>();

		protected bool onTouchPinch_;

		protected bool invalidTouch_;

		protected int onMouseButton_ = -1;

		protected Vector3 onMousePosition_;

		protected Vector3 onMouseBeginPosition_;

		protected bool onMouseRight_;

		protected bool onMousePinch_;

		protected float[] onMousePinchPointer_;

		private long onMousePinchTime_;

		protected string[] softLabel_ = new string[4] { "", "", null, null };

		protected string[] showSoftLabel_ = new string[4] { "", "", null, null };

		protected const int KEYBUFFER_SIZE = 60;

		protected int keyBufIndex_;

		protected int[] keyBuffer_ = new int[60];

		protected int keyStateWork_;

		protected int keyState_;

		protected int keyPulse_;

		protected int keyPulseInvalid_;

		protected int keyEventState_;

		protected int defaultKeyRepeatInterval_ = 10;

		protected int defaultKeyDoubleInterval_ = 4;

		protected bool fullscreen_;

		protected bool fullscreenChanging_;

		protected int linearFilterQuality_ = -1;

		private static DisplayMetrics displayMetrics_ = new DisplayMetrics();

		private List<string> backSoftLabels_ = new List<string>();

		private float aspectRatio_ = float.NaN;

		internal static int lastOpenBrowserFrame_ = -1;

		internal bool isAlterStaticRendering_;

		private Canvas()
		{
			Init();
		}

		private void Init()
		{
			if (Config.DEBUG)
			{
				VALID_KEYS = new KeyCode[23][]
				{
					new KeyCode[1] { KeyCode.Keypad0 },
					new KeyCode[1] { KeyCode.Keypad1 },
					new KeyCode[1] { KeyCode.Keypad2 },
					new KeyCode[1] { KeyCode.Keypad3 },
					new KeyCode[3]
					{
						KeyCode.Keypad4,
						KeyCode.LeftControl,
						KeyCode.RightControl
					},
					new KeyCode[1] { KeyCode.Keypad5 },
					new KeyCode[3]
					{
						KeyCode.Keypad6,
						KeyCode.LeftShift,
						KeyCode.RightShift
					},
					new KeyCode[1] { KeyCode.Keypad7 },
					new KeyCode[1] { KeyCode.Keypad8 },
					new KeyCode[1] { KeyCode.Keypad9 },
					new KeyCode[1] { KeyCode.Asterisk },
					new KeyCode[1] { KeyCode.Hash },
					new KeyCode[1] { KeyCode.KeypadMultiply },
					new KeyCode[1] { KeyCode.KeypadDivide },
					new KeyCode[1] { KeyCode.UpArrow },
					new KeyCode[1] { KeyCode.RightArrow },
					new KeyCode[1] { KeyCode.DownArrow },
					new KeyCode[1] { KeyCode.LeftArrow },
					new KeyCode[4]
					{
						KeyCode.KeypadEnter,
						KeyCode.Space,
						KeyCode.Z,
						KeyCode.Return
					},
					new KeyCode[1] { KeyCode.A },
					new KeyCode[2]
					{
						KeyCode.S,
						KeyCode.X
					},
					new KeyCode[1] { KeyCode.Escape },
					new KeyCode[1] { KeyCode.Home }
				};
			}
			else
			{
				VALID_KEYS = new KeyCode[23][]
				{
					new KeyCode[1] { KeyCode.Keypad0 },
					new KeyCode[1] { KeyCode.Keypad1 },
					new KeyCode[1] { KeyCode.Keypad2 },
					new KeyCode[1] { KeyCode.Keypad3 },
					new KeyCode[1] { KeyCode.Keypad4 },
					new KeyCode[1] { KeyCode.Keypad5 },
					new KeyCode[1] { KeyCode.Keypad6 },
					new KeyCode[1] { KeyCode.Keypad7 },
					new KeyCode[1] { KeyCode.Keypad8 },
					new KeyCode[1] { KeyCode.Keypad9 },
					new KeyCode[1] { KeyCode.Asterisk },
					new KeyCode[1] { KeyCode.Hash },
					new KeyCode[1] { KeyCode.KeypadMultiply },
					new KeyCode[1] { KeyCode.KeypadDivide },
					new KeyCode[1] { KeyCode.UpArrow },
					new KeyCode[1] { KeyCode.RightArrow },
					new KeyCode[1] { KeyCode.DownArrow },
					new KeyCode[1] { KeyCode.LeftArrow },
					new KeyCode[4]
					{
						KeyCode.KeypadEnter,
						KeyCode.Space,
						KeyCode.Z,
						KeyCode.Return
					},
					new KeyCode[2]
					{
						KeyCode.A,
						KeyCode.LeftShift
					},
					new KeyCode[3]
					{
						KeyCode.S,
						KeyCode.X,
						KeyCode.RightShift
					},
					new KeyCode[1] { KeyCode.Escape },
					new KeyCode[1] { KeyCode.Home }
				};
			}
			onValidKeys_ = new bool[VALID_KEYS.Length];
			defaultFont_ = new Font();
			RegistBackSoftLabel("戻る");
			RegistBackSoftLabel("Back");
			if (linearFilterQuality_ == -1)
			{
				linearFilterQuality_ = QualitySettings.antiAliasing;
				if (linearFilterQuality_ <= 0)
				{
					linearFilterQuality_ = 2;
				}
			}
			UpdateAdMobView();
			IApplication.GetCurrentApp().RunOnUpdate(delegate
			{
				if (KairoPlugin.SupportsDepth24() && !Config.LOWDEPTH_TEST)
				{
					Offscreen.DepthBits = 24;
					Graphics.DepthBits = 24;
					Image tmpImg = Image.CreateImage(10, 10);
					Image[] imgs = new Image[2];
					for (int i = 0; i < imgs.Length; i++)
					{
						imgs[i] = Image.CreateImage(tmpImg.GetWidth(), tmpImg.GetHeight());
						imgs[i].Clear((i == 0) ? Graphics.GetColorOfRGB(255, 0, 0) : Graphics.GetColorOfRGB(0, 255, 0));
					}
					Offscreen.Render(tmpImg, delegate(Graphics g, Offscreen off)
					{
						off.Clear(0, 0, 0);
						for (int j = 0; j < tmpImg.GetHeight(); j++)
						{
							g.DrawImage(imgs[j % imgs.Length], 0f, j);
						}
						return true;
					});
					Color color = default(Color);
					for (int num = 0; num < tmpImg.GetHeight(); num++)
					{
						Color pixel = tmpImg.GetPixel(tmpImg.GetWidth() / 2, num);
						if (num != 0 && color.r == pixel.r && color.g == pixel.g && color.b == pixel.b)
						{
							Offscreen.DepthBits = 16;
							Graphics.DepthBits = 16;
							break;
						}
						color = pixel;
					}
					for (int num2 = imgs.Length - 1; num2 >= 0; num2--)
					{
						imgs[num2].Dispose();
					}
					tmpImg.Dispose();
				}
				else
				{
					Offscreen.DepthBits = 16;
					Graphics.DepthBits = 16;
				}
			});
		}

		private static Graphics graphicsBuffer_;

		public static Canvas GetInstance()
		{
			return GetInstance(true);
		}

		public static Canvas GetInstance(bool make)
		{
			if (instance_ == null && make)
			{
				instance_ = new Canvas();
			}
			return instance_;
		}

		public void SetDisplayCurrent(IApplication app)
		{
		}

		public Graphics GetGraphicsInstance()
		{
			return graphics_;
		}

		public Graphics BeginPaint()
		{
			return BeginPaint(graphics_);
		}

		internal Graphics BeginPaint(Graphics g, int meshId = -1)
		{
			if (g == null)
			{
				if (graphics_ != null)
				{
					return null;
				}
				if (graphics_ == null)
				{
					if (graphicsBuffer_ == null)
					{
						graphicsBuffer_ = new Graphics();
					}
					graphics_ = graphicsBuffer_;
					graphics_.Init(null, null);
				}
				g = graphics_;
			}
			if (!g.IsRenderGUIMode())
			{
				if (meshId != -1)
				{
					if (g.meshManList_[meshId] != null)
					{
						g.meshManList_[meshId].BeginMesh();
					}
				}
				else
				{
					for (int i = 0; i < g.meshManList_.Length; i++)
					{
						if (g.meshManList_[i] != null)
						{
							g.meshManList_[i].BeginMesh();
						}
					}
				}
			}
			g.ResetRender();
			g.SetFont(defaultFont_);
			if (!g.IsRenderGUIMode() && meshId != -1)
			{
				g.SetCurrentMeshManager(meshId);
			}
			Graphics.current_ = g;
			return g;
		}

		internal Graphics BeginAlterStaticRender(Graphics g, int meshId = -1)
		{
			if (g == null)
			{
				if (graphics_ != null)
				{
					return null;
				}
				if (graphics_ == null)
				{
					if (graphicsBuffer_ == null)
					{
						graphicsBuffer_ = new Graphics();
					}
					graphics_ = graphicsBuffer_;
					graphics_.Init(null, null);
				}
				g = graphics_;
			}
			g.ResetRender();
			g.SetFont(defaultFont_);
			if (!g.IsRenderGUIMode() && meshId != -1)
			{
				g.SetCurrentMeshManager(meshId);
			}
			isAlterStaticRendering_ = true;
			Graphics.current_ = g;
			return g;
		}

		public void LockAndUnlock()
		{
		}

		public void EndPaint()
		{
			EndPaint(graphics_);
		}

		internal void EndPaint(Graphics g, int meshId = -1)
		{
			if (!g.IsRenderGUIMode())
			{
				int i;
				if (meshId != -1)
				{
					if (g.meshManList_[meshId] != null)
					{
						g.meshManList_[meshId].EndMesh();
					}
				}
				else
				{
					for (i = 0; i < g.meshManList_.Length; i++)
					{
						if (g.meshManList_[i] != null)
						{
							g.meshManList_[i].EndMesh();
						}
					}
				}
				MeshManager[] meshManList_ = g.meshManList_;
				for (int j = 0; j < meshManList_.Length; j++)
				{
					foreach (MeshBuilder value in meshManList_[j].meshList_.Values)
					{
						foreach (MeshBuilder.Layer value2 in value.layers.Values)
						{
							foreach (MeshBuilder.Group group in value2.groups)
							{
								group.change = false;
							}
						}
					}
				}
			}
			g.EndGLRender(true);
			if (g == graphics_ && graphics_ != null)
			{
				graphics_.Dispose(true);
				graphics_ = null;
			}
			isAlterStaticRendering_ = false;
		}

		internal void EndAlterStaticRender(Graphics g, int meshId = -1)
		{
			if (!g.IsRenderGUIMode())
			{
				if (meshId != -1)
				{
					if (g.meshManList_[meshId] != null)
					{
						g.meshManList_[meshId].EndAlterRender();
					}
				}
				else
				{
					for (int i = 0; i < g.meshManList_.Length; i++)
					{
						if (g.meshManList_[i] != null)
						{
							g.meshManList_[i].EndAlterRender();
						}
					}
				}
				MeshManager[] meshManList_ = g.meshManList_;
				for (int j = 0; j < meshManList_.Length; j++)
				{
					foreach (MeshBuilder value in meshManList_[j].meshList_.Values)
					{
						foreach (MeshBuilder.Layer value2 in value.layers.Values)
						{
							foreach (MeshBuilder.Group group in value2.groups)
							{
								group.change = false;
							}
						}
					}
				}
			}
			g.EndGLRender(true);
			if (g == graphics_ && graphics_ != null)
			{
				graphics_.Dispose(true);
				graphics_ = null;
			}
		}

		public void Paint(object arg0)
		{
		}

		public void SetSoftLabel(string sk1, string sk2, string sk3 = null, string sk4 = null)
		{
			softLabel_[0] = sk1;
			softLabel_[1] = sk2;
			softLabel_[2] = sk3;
			softLabel_[3] = sk4;
		}

		public void SetSoftLabel(int type, string label)
		{
			softLabel_[type] = label;
			UpdateSoftLabel();
		}

		public string GetSoftLabelL()
		{
			return showSoftLabel_[0];
		}

		public string GetSoftLabelR()
		{
			return showSoftLabel_[1];
		}

		public string GetSoftLabel(int id)
		{
			return showSoftLabel_[id];
		}

		public void UpdateSoftLabel()
		{
			for (int i = 0; i < showSoftLabel_.Length; i++)
			{
				if (GetSoftLabel(i) != softLabel_[i])
				{
					showSoftLabel_[i] = softLabel_[i];
				}
			}
		}

		public void SetKeyRepeatInterval(int frame)
		{
			defaultKeyRepeatInterval_ = frame;
		}

		public int GetKeyRepeatInterval()
		{
			return defaultKeyRepeatInterval_;
		}

		public void SetKeyDoubleInterval(int frame)
		{
			defaultKeyDoubleInterval_ = frame;
		}

		public int GetKeyDoubleInterval()
		{
			return defaultKeyDoubleInterval_;
		}

		public void OnInputUpdate()
		{
			if (invalidTouch_ || Input.touchCount <= 0 || IApplication.GetCurrentApp().GetTotalFrames() <= 10)
			{
				return;
			}
			List<int[]> list = new List<int[]>();
			List<int[]> list2 = new List<int[]>();
			List<int[]> list3 = new List<int[]>();
			List<int[]> list4 = new List<int[]>();
			List<float[]> list5 = new List<float[]>();
			List<float[]> list6 = new List<float[]>();
			List<float[]> list7 = new List<float[]>();
			List<float[]> list8 = new List<float[]>();
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				Vector2 position = touch.position;
				if (KairoPlugin.GetUnityVarsion() >= 540 && !IsFullscreen() && GetStatusBarPosition() == 0)
				{
					position.y -= 25f * DisplayMetrics.Density;
				}
				int[] item = new int[3]
				{
					touch.fingerId,
					(int)position.x,
					Screen.height - (int)position.y
				};
				float[] array = new float[3]
				{
					touch.deltaPosition.x,
					touch.deltaPosition.y,
					touch.deltaTime
				};
				float[] value = null;
				onTouchPointer_.TryGetValue(touch.fingerId, out value);
				if (touch.phase == TouchPhase.Began)
				{
					value = (onTouchPointer_[touch.fingerId] = new float[4]);
				}
				if (value != null)
				{
					if (touch.deltaPosition.x * touch.deltaPosition.x + touch.deltaPosition.y * touch.deltaPosition.y >= 1f)
					{
						value[2] = touch.deltaPosition.x;
						value[3] = touch.deltaPosition.y;
					}
					else
					{
						value[2] = (position.x - value[0]) * touch.deltaTime;
						value[3] = (position.y - value[1]) * touch.deltaTime;
					}
					value[0] = position.x;
					value[1] = position.y;
					array[0] = value[2];
					array[1] = value[3];
				}
				if (touch.phase == TouchPhase.Began)
				{
					list.Add(item);
					list5.Add(array);
				}
				if (touch.phase == TouchPhase.Moved)
				{
					list2.Add(item);
					list6.Add(array);
				}
				if (touch.phase == TouchPhase.Ended)
				{
					list3.Add(item);
					list7.Add(array);
					if (value != null)
					{
						onTouchPointer_.Remove(touch.fingerId);
					}
				}
				if (touch.phase == TouchPhase.Canceled)
				{
					list4.Add(item);
					list8.Add(array);
					if (value != null)
					{
						onTouchPointer_.Remove(touch.fingerId);
					}
				}
			}
			if (list.Count > 0)
			{
				int num = 0;
				if (list2.Count == 0)
				{
					OnTouchEvent(new MotionEvent(0, new int[1] { list[0][0] }, new float[2]
					{
						list[0][1],
						list[0][2]
					}, new float[3]
					{
						list5[0][0],
						list5[0][1],
						list5[0][2]
					}));
					num = 1;
				}
				if (list.Count - num > 0)
				{
					int[] array3 = new int[list.Count - num];
					float[] array4 = new float[(list.Count - num) * 2];
					float[] array5 = new float[(list.Count - num) * 3];
					int num2 = 5;
					for (int j = 0; j < list.Count - num; j++)
					{
						array3[j] = list[j + num][0];
						array4[j * 2 + 0] = list[j + num][1];
						array4[j * 2 + 1] = list[j + num][2];
						array5[j * 3 + 0] = list5[j + num][0];
						array5[j * 3 + 1] = list5[j + num][1];
						array5[j * 3 + 2] = list5[j + num][2];
						num2 |= 1 << array3[j] << 8;
					}
					OnTouchEvent(new MotionEvent(num2, array3, array4, array5));
				}
			}
			if (list2.Count > 0)
			{
				int[] array6 = new int[list2.Count];
				float[] array7 = new float[list2.Count * 2];
				float[] array8 = new float[list2.Count * 3];
				for (int k = 0; k < list2.Count; k++)
				{
					array6[k] = list2[k][0];
					array7[k * 2 + 0] = list2[k][1];
					array7[k * 2 + 1] = list2[k][2];
					array8[k * 3 + 0] = list6[k][0];
					array8[k * 3 + 1] = list6[k][1];
					array8[k * 3 + 2] = list6[k][2];
				}
				OnTouchEvent(new MotionEvent(2, array6, array7, array8));
			}
			if (list4.Count > 0)
			{
				int[] array9 = new int[list4.Count];
				float[] array10 = new float[list4.Count * 2];
				float[] array11 = new float[list4.Count * 3];
				for (int l = 0; l < list4.Count; l++)
				{
					array9[l] = list4[l][0];
					array10[l * 2 + 0] = list4[l][1];
					array10[l * 2 + 1] = list4[l][2];
					array11[l * 3 + 0] = list8[l][0];
					array11[l * 3 + 1] = list8[l][1];
					array11[l * 3 + 2] = list8[l][2];
				}
				OnTouchEvent(new MotionEvent(3, array9, array10, array11));
			}
			if (list3.Count <= 0)
			{
				return;
			}
			int num3 = 0;
			if (list2.Count == 0)
			{
				num3 = 1;
			}
			if (list3.Count - num3 > 0)
			{
				int[] array12 = new int[list3.Count - num3];
				float[] array13 = new float[(list3.Count - num3) * 2];
				float[] array14 = new float[(list3.Count - num3) * 3];
				int num4 = 6;
				for (int m = 0; m < list3.Count - num3; m++)
				{
					array12[m] = list3[m + num3][0];
					array13[m * 2 + 0] = list3[m + num3][1];
					array13[m * 2 + 1] = list3[m + num3][2];
					array14[m * 3 + 0] = list7[m + num3][0];
					array14[m * 3 + 1] = list7[m + num3][1];
					array14[m * 3 + 2] = list7[m + num3][2];
					num4 |= 1 << array12[m] << 8;
				}
				OnTouchEvent(new MotionEvent(num4, array12, array13, array14));
			}
			if (num3 > 0)
			{
				OnTouchEvent(new MotionEvent(1, new int[1] { list3[0][0] }, new float[2]
				{
					list3[0][1],
					list3[0][2]
				}, new float[3]
				{
					list7[0][0],
					list7[0][1],
					list7[0][2]
				}));
			}
		}

		public void DecideKeyState()
		{
			if (Input.touchCount == 0 && invalidTouch_)
			{
				invalidTouch_ = false;
			}
			for (int i = 0; i < VALID_KEYS.Length; i++)
			{
				KeyCode[] array = VALID_KEYS[i];
				GetKeyID(array[0]);
				bool flag = false;
				KeyCode[] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					if (Input.GetKey(array2[j]))
					{
						flag = true;
						break;
					}
				}
				if (!onValidKeys_[i] && flag)
				{
					onValidKeys_[i] = true;
					OnKeyDown(array[0], new KeyEvent(0, array[0]));
				}
				else if (onValidKeys_[i] && !flag)
				{
					onValidKeys_[i] = false;
					OnKeyUp(array[0], new KeyEvent(1, array[0]));
				}
			}
			if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
			{
				bool flag2 = true;
				if (Config.DEBUG && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
				{
					flag2 = false;
				}
				if (flag2)
				{
					bool mouseButton = Input.GetMouseButton(0);
					int num = -1;
					if (mouseButton)
					{
						onMousePinch_ = false;
						if (onMouseButton_ == -1)
						{
							num = 0;
							onMouseButton_ = 0;
							onMousePosition_ = (onMouseBeginPosition_ = Input.mousePosition);
						}
						else
						{
							int num2 = (int)(Input.mousePosition.x - onMousePosition_.x);
							int num3 = (int)(Input.mousePosition.y - onMousePosition_.y);
							if (num2 * num2 + num3 * num3 > 0)
							{
								num = 2;
								onMousePosition_ = Input.mousePosition;
							}
						}
					}
					else if (onMouseButton_ != -1)
					{
						num = 1;
						onMouseButton_ = -1;
						onMousePosition_ = Input.mousePosition;
					}
					if (num != -1)
					{
						MotionEvent motionEvent = null;
						motionEvent = new MotionEvent(num, new int[1], new float[2]
						{
							Input.mousePosition.x,
							(float)Screen.height - Input.mousePosition.y
						}, null, true);
						OnTouchEvent(motionEvent);
					}
					mouseButton = Input.GetMouseButton(1);
					if (mouseButton && !onMouseRight_)
					{
						OnKeyDown(KeyCode.Keypad4, new KeyEvent(0, KeyCode.Keypad4));
					}
					else if (!mouseButton && onMouseRight_)
					{
						OnKeyUp(KeyCode.Keypad4, new KeyEvent(0, KeyCode.Keypad4));
					}
					onMouseRight_ = mouseButton;
					if (!mouseButton && onMouseButton_ == -1)
					{
						float axis = Input.GetAxis("Mouse ScrollWheel");
						if (axis != 0f)
						{
							float x = Input.mousePosition.x;
							float num4 = (float)Screen.height - Input.mousePosition.y;
							if (!onMousePinch_)
							{
								onMousePinch_ = true;
								onMousePinchPointer_ = new float[4]
								{
									x,
									num4,
									x + 500f,
									num4
								};
								OnTouchEvent(new MotionEvent(0, new int[1], new float[2]
								{
									onMousePinchPointer_[0],
									onMousePinchPointer_[1]
								}, null, true));
								OnTouchEvent(new MotionEvent(261, new int[2] { 0, 1 }, onMousePinchPointer_, null, true));
							}
							onMousePinchPointer_[2] += axis * 200f;
							OnTouchEvent(new MotionEvent(2, new int[2] { 0, 1 }, onMousePinchPointer_, null, true));
							onMousePinchTime_ = JSystem.CurrentTimeMillis();
						}
						else if (onMousePinch_ && JSystem.CurrentTimeMillis() - onMousePinchTime_ >= 500)
						{
							OnTouchEvent(new MotionEvent(262, new int[2] { 0, 1 }, onMousePinchPointer_, null, true));
							onMousePinch_ = false;
						}
					}
				}
			}
			int num5 = GetKeypadState() | keyEventState_;
			keyEventState_ = 0;
			keyPulse_ = (num5 ^ keyState_) & num5 & ~keyPulseInvalid_;
			keyState_ = num5;
			keyPulseInvalid_ = 0;
			keyStateWork_ &= -2097153;
			keyStateWork_ &= -4194305;
			keyBuffer_[keyBufIndex_] = num5;
			keyBufIndex_ = (keyBufIndex_ + 1) % keyBuffer_.Length;
		}

		public int GetKeyState()
		{
			return keyState_;
		}

		public bool CheckKeyState(int key)
		{
			if ((keyState_ & key) == 0)
			{
				return false;
			}
			return true;
		}

		public bool CheckKeyPulse(int key)
		{
			int num = keyPulse_;
			keyPulse_ = num & ~key;
			if ((num & key) == 0)
			{
				return false;
			}
			return true;
		}

		public bool CheckKeyRepeat(int key)
		{
			return CheckKeyRepeat(key, defaultKeyRepeatInterval_);
		}

		public bool CheckKeyRepeat(int key, int interval)
		{
			if ((keyBuffer_[(keyBufIndex_ + keyBuffer_.Length - 1) % keyBuffer_.Length] & key) == 0)
			{
				return false;
			}
			if ((keyBuffer_[(keyBufIndex_ + keyBuffer_.Length - 2) % keyBuffer_.Length] & key) == 0)
			{
				return true;
			}
			for (int i = 1; i < interval; i++)
			{
				int num = keyBuffer_[(keyBufIndex_ + keyBuffer_.Length - 2 - i) % keyBuffer_.Length];
				int num2 = num & key;
				if (num - num2 != 0)
				{
					return false;
				}
				if (num2 == 0)
				{
					return false;
				}
			}
			return true;
		}

		public bool CheckKeyDouble(int key)
		{
			return CheckKeyDouble(key, defaultKeyDoubleInterval_);
		}

		public bool CheckKeyDouble(int key, int interval)
		{
			if ((keyBuffer_[(keyBufIndex_ + keyBuffer_.Length - 1) % keyBuffer_.Length] & key) == 0)
			{
				return false;
			}
			if ((keyBuffer_[(keyBufIndex_ + keyBuffer_.Length - 2) % keyBuffer_.Length] & key) != 0)
			{
				return false;
			}
			for (int i = 3; i < interval + 3; i++)
			{
				int num = keyBuffer_[(keyBufIndex_ + keyBuffer_.Length - i) % keyBuffer_.Length];
				int num2 = num & key;
				if (num - num2 != 0)
				{
					return false;
				}
				if (num2 != 0)
				{
					return true;
				}
			}
			return false;
		}

		public int GetKeypadState()
		{
			return keyStateWork_;
		}

		public void ClearKeyBuffer()
		{
			for (int i = 0; i < keyBuffer_.Length; i++)
			{
				keyBuffer_[i] = 0;
			}
			keyBufIndex_ = 0;
			keyPulse_ = 0;
			keyState_ = 0;
			keyEventState_ = 0;
			keyStateWork_ = 0;
			invalidTouch_ = true;
			if (Input.touchCount <= 0)
			{
				return;
			}
			List<int[]> list = new List<int[]>();
			for (int j = 0; j < Input.touchCount; j++)
			{
				Touch touch = Input.GetTouch(j);
				int[] item = new int[3]
				{
					touch.fingerId,
					(int)touch.position.x,
					Screen.height - (int)touch.position.y
				};
				float[] array3 = new float[3]
				{
					touch.deltaPosition.x,
					touch.deltaPosition.y,
					touch.deltaTime
				};
				float[] value = null;
				onTouchPointer_.TryGetValue(touch.fingerId, out value);
				if (touch.phase == TouchPhase.Canceled)
				{
					list.Add(item);
					if (value != null)
					{
						onTouchPointer_.Remove(touch.fingerId);
					}
				}
			}
			if (list.Count > 0)
			{
				int[] array = new int[list.Count];
				float[] array2 = new float[list.Count * 2];
				float[] pointerDelta = new float[list.Count * 3];
				for (int k = 0; k < list.Count; k++)
				{
					array[k] = list[k][0];
					array2[k * 2 + 0] = list[k][1];
					array2[k * 2 + 1] = list[k][2];
				}
				OnTouchEvent(new MotionEvent(3, array, array2, pointerDelta));
			}
		}

		public bool OnKeyDown(KeyCode keyCode, KeyEvent evt)
		{
			if (surfaceManager_ != null)
			{
				surfaceManager_.OnKeyDown((int)keyCode, evt);
			}
			if (evt.GetRepeatCount() == 0)
			{
				int keyID = GetKeyID(keyCode);
				if (keyID != -1 && (disableKeyBits_ & (1 << keyID)) == 0)
				{
					keyStateWork_ |= 1 << keyID;
					if (surfaceManager_ != null)
					{
						surfaceManager_.OnKeyDown(keyID);
					}
				}
				IApplication currentApp = IApplication.GetCurrentApp();
				if (keyCode == KeyCode.Escape)
				{
					if (currentApp.IsTermination())
					{
						return true;
					}
					string text = GetSoftLabelL();
					string text2 = GetSoftLabelR();
					string text3 = GetSoftLabel(2);
					string text4 = GetSoftLabel(3);
					if (text != null)
					{
						text = text.ToLower();
					}
					if (text2 != null)
					{
						text2 = text2.ToLower();
					}
					if (text3 != null)
					{
						text3 = text3.ToLower();
					}
					if (text4 != null)
					{
						text4 = text4.ToLower();
					}
					int num = -1;
					foreach (string item in backSoftLabels_)
					{
						if (text == item)
						{
							num = 2097152;
						}
						else if (text2 == item)
						{
							num = 4194304;
						}
						else if (text3 == item)
						{
							num = 8388608;
						}
						else if (text4 == item)
						{
							num = 16777216;
						}
						if (num != -1)
						{
							break;
						}
					}
					if (num != -1)
					{
						KeyDown(num);
					}
					else if (currentApp.GetFormManager() != null)
					{
						currentApp.GetFormManager().ProcessBackKey();
					}
				}
			}
			return true;
		}

		public bool OnKeyUp(KeyCode keyCode, KeyEvent evt)
		{
			if (evt.GetRepeatCount() == 0)
			{
				int keyID = GetKeyID(keyCode);
				if (keyID != -1 && (disableKeyBits_ & (1 << keyID)) == 0)
				{
					keyStateWork_ &= ~(1 << keyID);
					if (surfaceManager_ != null)
					{
						surfaceManager_.OnKeyUp(keyID);
					}
				}
			}
			return true;
		}

		public void KeyDown(int key, bool pulse = true)
		{
			if (key == -1) { return; }
keyEventState_ |= key;
keyStateWork_ |= key;
if (pulse)
				{
					keyPulseInvalid_ &= ~key;
				}
				else
				{
					keyPulseInvalid_ |= key;
				}
if (surfaceManager_ != null)
				{
					surfaceManager_.OnKeyDown(key);
				}
		}

		public void KeyUp(int key)
		{
			if (key == -1)
			{
				return;
			}
			this.keyStateWork_ &= ~key;
			if (this.surfaceManager_ != null)
			{
				this.surfaceManager_.OnKeyUp(key);
			}
		}

		public void KeyClick(int key)
		{
			if (key == -1)
			{
				return;
			}
			this.keyEventState_ |= key;
			if (this.surfaceManager_ != null)
			{
				this.surfaceManager_.OnKeyDown(key);
				this.surfaceManager_.OnKeyUp(key);
			}
		}

		protected int GetKeyID(KeyCode keyCode)
		{
			switch (keyCode)
			{
			case KeyCode.Keypad0:
				return 0;
			case KeyCode.Keypad1:
				return 1;
			case KeyCode.Keypad2:
				return 2;
			case KeyCode.Keypad3:
				return 3;
			case KeyCode.Keypad4:
				return 4;
			case KeyCode.Keypad5:
				return 5;
			case KeyCode.Keypad6:
				return 6;
			case KeyCode.Keypad7:
				return 7;
			case KeyCode.Keypad8:
				return 8;
			case KeyCode.Keypad9:
				return 9;
			case KeyCode.Asterisk:
			case KeyCode.KeypadMultiply:
				return 10;
			case KeyCode.Hash:
			case KeyCode.KeypadDivide:
				return 11;
			case KeyCode.UpArrow:
				return 17;
			case KeyCode.RightArrow:
				return 18;
			case KeyCode.DownArrow:
				return 19;
			case KeyCode.LeftArrow:
				return 16;
			case KeyCode.Return:
			case KeyCode.Space:
			case KeyCode.Z:
			case KeyCode.KeypadEnter:
				return 20;
			case KeyCode.A:
				return 21;
			case KeyCode.S:
			case KeyCode.X:
				return 22;
			case KeyCode.LeftShift:
				if (Config.DEBUG)
				{
					return 6;
				}
				return 21;
			case KeyCode.RightShift:
				if (Config.DEBUG)
				{
					return 6;
				}
				return 22;
			case KeyCode.RightControl:
			case KeyCode.LeftControl:
				if (Config.DEBUG)
				{
					return 4;
				}
				break;
			}
			return -1;
		}

		public static void Vibrator(bool flag)
		{
			if (flag)
			{
				Vibrator(100L);
			}
		}

		public static void Vibrator(long ms)
		{
			KairoPlugin.Vibrator(0L, ms);
		}

		public static void Vibrator(long[] pattern)
		{
			KairoPlugin.Vibrator(pattern);
		}

		public static void BackLight(bool on)
		{
		}

		public static bool OpenBrowser(string url)
		{
			try
			{
				if (Config.APPLI_TYPE == 3)
				{
					KairoBase instance = KairoBase.GetInstance();
					if (instance != null)
					{
						Match match = Regex.Match(url, "market://details\\?id=([\\w.]+)");
						if (match.Success)
						{
							string value = match.Groups[1].Value;
							GameAppli gameAppli = default(GameAppli);
							int num = 0;
							GameAppli[] appList = instance.GetAppList(false, false);
							for (int i = 0; i < appList.Length; i++)
							{
								GameAppli gameAppli2 = appList[i];
								if (gameAppli2.IOSId != null && gameAppli2.IOSId.Length > 0 && (gameAppli2.AndPkg.Equals(value, StringComparison.CurrentCultureIgnoreCase) || gameAppli2.IOSPkg.Equals(value, StringComparison.CurrentCultureIgnoreCase)))
								{
									int num2 = 1;
									if (gameAppli2.Language == "ja" == Language.Japanese())
									{
										num2++;
									}
									if (num2 > num)
									{
										num = num2;
										gameAppli = gameAppli2;
									}
								}
							}
							if (num == 0 && Config.DEBUG)
							{
								Dialog dialog = new Dialog(2, "OpenBrowser");
								dialog.SetText("ﾊﾟｯｹｰｼﾞ「" + value + "」からAppleIdへの変換に失敗しました。\n共有DBに登録されているか確認してください。");
								dialog.Show();
								return false;
							}
							if (gameAppli.Language == "ja")
							{
								url = "https://itunes.apple.com/jp/app/id" + gameAppli.IOSId + "?mt=8";
							}
							else
							{
								url = "https://itunes.apple.com/app/id" + gameAppli.IOSId + "?mt=8";
							}
						}
					}
				}
				IApplication.GetCurrentApp().RunOnUiThread(delegate
				{
					FormManagerBase.ProcessEvent(AppEventType.Browser, url);
					lastOpenBrowserFrame_ = IApplication.GetCurrentApp().GetTotalFrames();
					string text = "_lh=" + Language.GetLanguageCode();
					if (url.Contains("?")) { text = ("&" + text); } else { text = ("?" + text); }
					int num3 = url.LastIndexOf('#');
					if (num3 != -1)
					{
						url = url.Insert(num3, text);
					}
					else
					{
						url += text;
					}
					Application.OpenURL(url);
				}, true);
				return true;
			}
			catch (Exception)
			{
				if (Config.PRINT)
				{
					Log.Info("ブラウザの起動に失敗しました。" + url);
				}
			}
			return false;
		}

		public static bool JumpStore(long ID)
		{
			return JumpStore(string.Concat(ID));
		}

		public static bool JumpStore(string ID)
		{
			return false;
		}

		public static bool JumpStoreKairosoft()
		{
			return false;
		}

		public static void VersionUp(string url)
		{
		}

		public string ImeOnBlock(string text, int displayMode, int inputMode)
		{
			return KairoPlugin.ImeOnBlock(text, displayMode, inputMode);
		}

		public void SetBackground(int c)
		{
			SetBackColor(c);
		}

		public void SetBackColor(int c)
		{
			background_ = c;
		}

		public int GetBackColor()
		{
			return background_;
		}

		public int GetSyncMillSecound(int framerate)
		{
			return 0;
		}

		public void Fullscreen(bool enable)
		{
			if (IApplication.GetCurrentApp().GetFormManager().IsCapturing())
			{
				return;
			}
			KairoPlugin.Fullscreen(enable);
		}

		public bool IsFullscreen()
		{
			return KairoPlugin.IsFullscreen();
		}

		public void SetSurfaceManager(SurfaceManagerBase surfaceManager)
		{
			surfaceManager_ = surfaceManager;
		}

		public SurfaceManagerBase GetSurfaceManager()
		{
			return surfaceManager_;
		}

		public void RemoveView()
		{
		}

		public int GetWidth()
		{
			return KairoPlugin.GetWidth();
		}

		public int GetHeight()
		{
			if (float.IsNaN(aspectRatio_))
			{
				return KairoPlugin.GetHeight();
			}
			return (int)((float)GetWidth() * aspectRatio_ / 100f + 0.01f);
		}

		public int GetWidth2()
		{
			return KairoPlugin.GetWidth();
		}

		public int GetHeight2()
		{
			if (float.IsNaN(aspectRatio_))
			{
				return KairoPlugin.GetHeight();
			}
			return (int)((float)GetWidth2() * aspectRatio_ / 100f + 0.01f);
		}

		public int GetStatusBarHeight()
		{
			return KairoPlugin.GetStatusBarHeight();
		}

		public bool IsGetStatusBarHeight()
		{
			return true;
		}

		public bool OnTouchEvent(MotionEvent evt)
		{
			if (surfaceManager_ != null)
			{
				Vector<SurfaceBase> surfaceList = surfaceManager_.GetSurfaceList();
				for (int num = surfaceList.Size() - 1; num >= 0; num--)
				{
					surfaceList.ElementAt(num).OnReceiveTouchEvent(evt);
				}
			}
			return true;
		}

		public bool OnTrackballEvent(MotionEvent evt)
		{
			if (surfaceManager_ != null)
			{
				Vector<SurfaceBase> surfaceList = surfaceManager_.GetSurfaceList();
				for (int num = surfaceList.Size() - 1; num >= 0; num--)
				{
					surfaceList.ElementAt(num).OnReceiveTrackballEvent(evt);
				}
			}
			return true;
		}

		public void SetLinearFilterEnable(bool enable)
		{
		}

		public bool IsLinearFilterEnable()
		{
			return QualitySettings.antiAliasing != 0;
		}

		public int GetStatusBarPosition()
		{
			return KairoPlugin.GetStatusBarPosition();
		}

		public bool IsGetStatusBarPosition()
		{
			return statusBarPosition_ != -1;
		}

		public bool UpdateAdMobView()
		{
			if (updateAdMobViewRun_)
			{
				return false;
			}
			IApplication.GetCurrentApp();
			if (Config.ADMOB_ENABLE && admobView_ == null)
			{
				updateAdMobViewRun_ = true;
				IApplication.GetCurrentApp().RunOnUiThread(delegate
				{
					string text = Config.AD_UNIT_ID;
					if (text.Contains('@'))
					{
						if (Language.Japanese()) { text = Regex.Match(text, "@ja\\s+([^\\s@]+)").Groups[1].Value; } else { text = Regex.Match(text, "@en\\s+([^\\s@]+)").Groups[1].Value; }
					}
					AndroidJavaObject androidJavaObject = AdMobPlugin.CreateAdMobView(Config.AD_SIZE, text);
					if (androidJavaObject != null)
					{
						admobView_ = new AdMobView(androidJavaObject);
					}
					updateAdMobViewRun_ = false;
				});
				return true;
			}
			if (!Config.ADMOB_ENABLE && admobView_ != null)
			{
				updateAdMobViewRun_ = true;
				IApplication.GetCurrentApp().RunOnUiThread(delegate
				{
					if (admobView_ != null)
					{
						admobView_.Dispose();
					}
					admobView_ = null;
					updateAdMobViewRun_ = false;
				});
				return true;
			}
			return false;
		}

		private bool updateAdMobViewRun_;

		public AdMobView GetAdMobView()
		{
			return admobView_;
		}

		public void SetDisableKeyBits(int keyBits)
		{
			disableKeyBits_ = keyBits;
		}

		public void RegistBackSoftLabel(string label)
		{
			label = label.ToLower();
			if (!backSoftLabels_.Contains(label))
			{
				backSoftLabels_.Add(label);
			}
		}

		public void UnregistBackSoftLabel(string label)
		{
			label = label.ToLower();
			backSoftLabels_.Remove(label);
		}

		public void SetAspectRatio(int width, int height)
		{
			aspectRatio_ = (float)height * 100f / (float)width;
		}

		public void SetAspectRatio()
		{
			aspectRatio_ = float.NaN;
		}
	}
}
