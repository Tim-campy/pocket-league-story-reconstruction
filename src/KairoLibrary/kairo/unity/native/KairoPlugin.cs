using System;
using System.IO;
using java.lang;
using java.native;
using java.util;
using kairo.common.cfg;
using kairo.unity.native.util;
using kairo.unity.panel;
using kairo.unity.system;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.native
{
	public static class KairoPlugin
	{
		private static IApplication app_;

		private static AndroidJavaClass Config;

		private static AndroidJavaClass Utility;

		private static AndroidJavaClass VERSION;

		private static string fepTitle_;

		private static string fepText_;

		private static int fepMode_;

		private static string fepPositive_;

		private static string fepNegative_;

		private static int fepMaxLength_;

		private static string signature_;

		private static int sdk_ = -1;

		private static string release_;

		private static int isTablet_ = -1;

		private static int systemBarHeight_ = int.MaxValue;

		private static int statusBarHeight_ = int.MaxValue;

		private static int unityVarsion_ = int.MaxValue;

		private static int statusBarPosition_ = int.MaxValue;

		private static int lastCanvasWidth_;

		private static int lastCanvasHeight_;

		private static int appWidth_ = -1;

		private static int appHeight_ = -1;

		private static float scaleRatio_;

		private static int scaleRatioReqWidth_;

		private static int scaleRatioReqHeight_;

		private static int scaleRatioAppWidth_;

		private static int scaleRatioAppHeight_;

		private static float scaleRatioBase_;

		private static string packageName_;

		public static void Init(int language, bool log, string licenseKey)
		{
			app_ = IApplication.GetCurrentApp();
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				Config = new AndroidJavaClass("kairo.android.plugin.Config");
				Utility = new AndroidJavaClass("kairo.android.plugin.Utility");
				VERSION = new AndroidJavaClass("android.os.Build$VERSION");
			}
			JavaPlugin.Init();
			AdMobPlugin.Init();
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Config.CallStatic("init", language, log, licenseKey);
				}
			}, true);
		}

		public static void Finish()
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Config.CallStatic("finish");
				}
			}, true);
		}

		public static int LicenseCheck()
		{
			return (int)app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? ((object)Utility.CallStatic<int>("licenseCheck", new object[0])) : ((object)0), true);
		}

		public static string GetSignature()
		{
			if (signature_ == null)
			{
				if (app_.CheckUiThread())
				{
					signature_ = _getSignature();
				}
				else
				{
					signature_ = (string)app_.RunOnUiThread(() => _getSignature(), true);
				}
			}
			return signature_;
		}

		private static string _getSignature()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return Utility.CallStatic<string>("getSignature", new object[0]);
			}
			return "";
		}

		public static int GetSDK()
		{
			if (sdk_ == -1)
			{
				if (app_.CheckUiThread())
				{
					sdk_ = _getSDK();
				}
				else
				{
					sdk_ = (int)app_.RunOnUiThread(() => _getSDK(), true);
				}
			}
			return sdk_;
		}

		private static int _getSDK()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return VERSION.GetStatic<int>("SDK_INT");
			}
			return 21;
		}

		public static string GetReleaseNumber()
		{
			string text = GetRelease();
			for (int num = text.Length - 1; num >= 0; num--)
			{
				if (('0' > text[num] || text[num] >= '9') && text[num] != '.')
				{
					text = text.Remove(num);
				}
			}
			if (text.Length == 0)
			{
				text = "7.0";
			}
			return text;
		}

		public static string GetRelease()
		{
			if (release_ == null)
			{
				if (app_.CheckUiThread())
				{
					release_ = _getRelease();
				}
				else
				{
					release_ = (string)app_.RunOnUiThread(() => _getRelease(), true);
				}
			}
			return release_;
		}

		private static string _getRelease()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return VERSION.GetStatic<string>("RELEASE");
			}
			return "5.0";
		}

		public static bool IsTablet()
		{
			if (isTablet_ == -1)
			{
				if (app_.CheckUiThread())
				{
					isTablet_ = (_isTablet() ? 1 : 0);
				}
				else
				{
					isTablet_ = (((bool)app_.RunOnUiThread(() => _isTablet(), true)) ? 1 : 0);
				}
			}
			return isTablet_ == 1;
		}

		private static bool _isTablet()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return Utility.CallStatic<bool>("isTablet", new object[0]);
			}
			return false;
		}

		public static int GetSystemBarHeight()
		{
			if (systemBarHeight_ == int.MaxValue)
			{
				if (app_.CheckUiThread())
				{
					systemBarHeight_ = _getSystemBarHeight();
				}
				else
				{
					systemBarHeight_ = (int)app_.RunOnUiThread(() => _getSystemBarHeight(), true);
				}
			}
			return systemBarHeight_;
		}

		private static int _getSystemBarHeight()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return (int)((float)Utility.CallStatic<int>("getSystemBarHeight", new object[0]) * app_.GetResolutionScaleRatio() / 100f + 0.01f);
			}
			return 0;
		}

		public static int GetStatusBarHeight()
		{
			if (statusBarHeight_ != int.MaxValue)
			{
				int statusBarHeight_2 = statusBarHeight_;
				int num = 0;
			}
			if (app_.CheckUiThread())
			{
				statusBarHeight_ = _getStatusBarHeight();
			}
			else
			{
				statusBarHeight_ = (int)app_.RunOnUiThread(() => _getStatusBarHeight(), true);
			}
			return statusBarHeight_;
		}

		private static int _getStatusBarHeight()
		{
			if (GetUnityVarsion() < 530)
			{
				return 0;
			}
			int num = 0;
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				num = (int)((float)Utility.CallStatic<int>("getStatusBarHeight", new object[0]) * app_.GetResolutionScaleRatio() / 100f + 0.01f);
			}
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}

		public static int GetUnityVarsion()
		{
			if (unityVarsion_ == int.MaxValue)
			{
				string text = "";
				string unityVersion = Application.unityVersion;
				for (int i = 0; i < unityVersion.Length; i++)
				{
					if (unityVersion[i] != '.')
					{
						int result = 0;
						if (!int.TryParse(Application.unityVersion[i].ToString() ?? "", out result))
						{
							break;
						}
						text += result;
					}
				}
				unityVarsion_ = int.Parse(text);
			}
			return unityVarsion_;
		}

		public static int GetStatusBarPosition()
		{
			if (statusBarPosition_ == int.MaxValue)
			{
				if (app_.CheckUiThread())
				{
					statusBarPosition_ = _getStatusBarPosition();
				}
				else
				{
					statusBarPosition_ = (int)app_.RunOnUiThread(() => _getStatusBarPosition(), true);
				}
			}
			return statusBarPosition_;
		}

		private static int _getStatusBarPosition()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return Utility.CallStatic<int>("getStatusBarPosition", new object[0]);
			}
			return 0;
		}

		public static void Fullscreen(bool enable)
		{
			if (kairo.common.cfg.Config.FULLSCREEN)
			{
				return;
			}
			app_.RunOnUiThread(delegate
			{
				Screen.fullScreen = enable;
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("fullscreen", enable);
				}
			}, true);
		}

		public static bool IsFullscreen()
		{
			if (app_.CheckUiThread())
			{
				return _isFullscreen();
			}
			return (bool)app_.RunOnUiThread(() => _isFullscreen(), true);
		}

		private static bool _isFullscreen()
		{
			return Screen.fullScreen;
		}

		public static int GetWidth()
		{
			if (app_.CheckUiThread())
			{
				return _getWidth();
			}
			return (int)app_.RunOnUiThread(() => _getWidth(), true);
		}

		private static int _getWidth()
		{
			if (FepPanel.Current != null)
			{
				return lastCanvasWidth_;
			}
			lastCanvasWidth_ = Screen.width;
			if (kairo.common.cfg.Config.APPLI_TYPE == 2)
			{
				bool uNITY_EDITOR = kairo.common.cfg.Config.UNITY_EDITOR;
				return Screen.width;
			}
			return Screen.width;
		}

		public static int GetHeight()
		{
			if (app_.CheckUiThread())
			{
				return _getHeight();
			}
			return (int)app_.RunOnUiThread(() => _getHeight(), true);
		}

		private static int _getHeight()
		{
			if (FepPanel.Current != null)
			{
				return lastCanvasHeight_;
			}
			lastCanvasHeight_ = Screen.height;
			if (kairo.common.cfg.Config.APPLI_TYPE == 2)
			{
				bool uNITY_EDITOR = kairo.common.cfg.Config.UNITY_EDITOR;
				return Screen.height;
			}
			return Screen.height;
		}

		public static int GetAppWidth()
		{
			if (appWidth_ == -1 || kairo.common.cfg.Config.UNITY_EDITOR)
			{
				if (app_.CheckUiThread())
				{
					appWidth_ = _getAppWidth();
				}
				else
				{
					appWidth_ = (int)app_.RunOnUiThread(() => _getAppWidth(), true);
				}
			}
			return appWidth_;
		}

		private static int _getAppWidth()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return (int)((float)Utility.CallStatic<int>("getAppWidth", new object[0]) * app_.GetResolutionScaleRatio() / 100f + 0.01f);
			}
			return Screen.width;
		}

		public static int GetAppHeight()
		{
			if (appHeight_ == -1 || kairo.common.cfg.Config.UNITY_EDITOR)
			{
				if (app_.CheckUiThread())
				{
					appHeight_ = _getAppHeight();
				}
				else
				{
					appHeight_ = (int)app_.RunOnUiThread(() => _getAppHeight(), true);
				}
			}
			return appHeight_;
		}

		private static int _getAppHeight()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return (int)((float)Utility.CallStatic<int>("getAppHeight", new object[0]) * app_.GetResolutionScaleRatio() / 100f + 0.01f);
			}
			return Screen.height;
		}

		internal static void UpdateAppSize()
		{
			if (appWidth_ != -1 && appHeight_ != -1 && appWidth_ < appHeight_ != Screen.width < Screen.height)
			{
				int num = appWidth_;
				appWidth_ = appHeight_;
				appHeight_ = num;
			}
		}

		public static float GetScaleRatio(int width, int height)
		{
			if (scaleRatioReqWidth_ == width && scaleRatioReqHeight_ == height && scaleRatioAppWidth_ == GetAppWidth() && scaleRatioAppHeight_ == GetAppHeight())
			{
				return scaleRatio_;
			}
			if (FepPanel.Current != null && scaleRatioBase_ > 0f)
			{
				return (float)(width * 100) / scaleRatioBase_;
			}
			scaleRatioReqWidth_ = width;
			scaleRatioReqHeight_ = height;
			scaleRatioAppWidth_ = GetAppWidth();
			scaleRatioAppHeight_ = GetAppHeight();
			if (app_.CheckUiThread())
			{
				scaleRatio_ = _getScaleRatio(width, height);
			}
			else
			{
				scaleRatio_ = (float)app_.RunOnUiThread(() => _getScaleRatio(width, height), true);
			}
			scaleRatioBase_ = (float)width * 100f / scaleRatio_;
			return scaleRatio_;
		}

		private static float _getScaleRatio(int width, int height)
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return Utility.CallStatic<float>("getScaleRatio", new object[2] { width, height }) * app_.GetResolutionScaleRatio() / 100f;
			}
			float num = (float)GetAppWidth() * 100f / (float)width;
			float num2 = (float)GetAppHeight() * 100f / (float)height;
			if (!(num < num2))
			{
				return num2;
			}
			return num;
		}

		public static string GetPackageName()
		{
			if (packageName_ == null)
			{
				if (app_.CheckUiThread())
				{
					packageName_ = _getPackageName();
				}
				else
				{
					packageName_ = (string)app_.RunOnUiThread(() => _getPackageName(), true);
				}
			}
			return packageName_;
		}

		private static string _getPackageName()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return Utility.CallStatic<string>("getPackageName", new object[0]);
			}
			if (kairo.common.cfg.Config.APPLI_TYPE == 3 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return "";
			}
			Type type = Type.GetType("UnityEditor.PlayerSettings, UnityEditor.dll");
			if (type != null)
			{
				return (string)type.GetProperty("bundleIdentifier").GetValue(null, null);
			}
			return "";
		}

		public static int ShowDialog(int type, string title, string msg, string buttons = null)
		{
			return (int)app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					return Utility.CallStatic<int>("showDialog", new object[4] { type, title, msg, buttons });
				}
				try
				{
					return int.Parse(kairo.unity.native.util.Dialog.ShowUnityDialog("dialog", type, title, msg, buttons)[0]);
				}
				catch (NullReferenceException)
				{
					return int.Parse(kairo.unity.native.util.Dialog.ShowUnityDialog("dialog", type, title, msg)[0]);
				}
			}, true);
		}

		public static string ImeOnBlock(string text, int displayMode, int inputMode)
		{
			return (string)app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					return Utility.CallStatic<string>("imeOnBlock", new object[3] { text, displayMode, inputMode });
				}
				return (kairo.common.cfg.Config.APPLI_TYPE == 3 && !kairo.common.cfg.Config.UNITY_EDITOR) ? text : kairo.unity.native.util.Dialog.ShowUnityDialog("ime", text)[0];
			}, true);
		}

		public static void SetClockTime(long time, int precision)
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("setClockTime", time, precision);
				}
			}, true);
		}

		public static long GetClockTime()
		{
			if (app_.CheckUiThread())
			{
				return _getClockTime();
			}
			return (long)app_.RunOnUiThread(() => _getClockTime(), true);
		}

		private static long _getClockTime()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return Utility.CallStatic<long>("getClockTime", new object[0]);
			}
			return JSystem.CurrentTimeMillis();
		}

		public static int[] GetBatteryInfo()
		{
			if (app_.CheckUiThread())
			{
				return _getBatteryInfo();
			}
			return (int[])app_.RunOnUiThread(() => _getBatteryInfo(), true);
		}

		private static int[] _getBatteryInfo()
		{
			if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
			{
				return Utility.CallStatic<int[]>("getBatteryInfo", new object[0]);
			}
			return new int[2];
		}

		public static string[] ShowFepPanel(string title, string text, int mode, string positive, string negative, int maxLength = 0)
		{
			return (string[])app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					return kairo.unity.native.util.StringUtil.GetStrArray(Utility.CallStatic<string>("showInputPanel", new object[6] { title, text, mode, positive, negative, maxLength }));
				}
				return (kairo.common.cfg.Config.APPLI_TYPE == 3 && !kairo.common.cfg.Config.UNITY_EDITOR) ? new string[2] { text, text } : kairo.unity.native.util.Dialog.ShowUnityDialog("input", title, text, positive, negative);
			}, true);
		}

		public static void StartFepPanel(string title, string text, int mode, string positive, string negative, int maxLength = 0)
		{
			fepTitle_ = title;
			fepText_ = text;
			fepMode_ = mode;
			fepPositive_ = positive;
			fepNegative_ = negative;
			fepMaxLength_ = maxLength;
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("startInputPanel", title, text, mode, positive, negative, maxLength);
				}
			}, true);
		}

		public static bool IsFepPanelFinish()
		{
			return (bool)app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? ((object)Utility.CallStatic<bool>("isInputPanelFinish", new object[0])) : ((object)true), true);
		}

		public static string[] GetFepPanelResult()
		{
			return (string[])app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? kairo.unity.native.util.StringUtil.GetStrArray(Utility.CallStatic<string>("getInputPanelResult", new object[0])) : kairo.unity.native.util.Dialog.ShowUnityDialog("input", fepTitle_, fepText_, fepPositive_, fepNegative_), true);
		}

		public static void PutPreference(string key, byte[] data)
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("putPreference", key, data);
				}
			}, true);
		}

		public static byte[] GetPreference(string key)
		{
			return (byte[])app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? ((!Utility.CallStatic<bool>("existPreference", new object[1] { key })) ? null : Utility.CallStatic<byte[]>("getPreference", new object[1] { key })) : null, true);
		}

		public static void RemovePreference(string key)
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("removePreference", key);
				}
			}, true);
		}

		public static string[] GetPreferenceKeys()
		{
			return (string[])app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? kairo.unity.native.util.StringUtil.GetStrArray(Utility.CallStatic<string>("getPreferenceKeys", new object[0])) : new string[0], true);
		}

		public static void ShowToast(string text, int duration = 0)
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("showToast", text, duration);
				}
				else
				{
					Debug.Log(text);
				}
			}, true);
		}

		public static bool SaveToSd(string file, byte[] data)
		{
			string save;
			return SaveToSd(file, data, out save);
		}

		public static bool SaveToSd(string file, byte[] data, out string save)
		{
			save = (string)app_.RunOnUiThread(delegate
			{
				new JDate();
				JCalendar instance = JCalendar.GetInstance();
				string text = $"{instance.Get(1):D4}{instance.Get(2) + 1:D2}{instance.Get(5):D2}-{instance.Get(11):D2}{instance.Get(12):D2}{instance.Get(13):D2}";
				if (!file.Contains("."))
				{
					file = text + "." + file;
				}
				file = file.Replace("{date}", text);
				if (kairo.common.cfg.Config.UNITY_EDITOR)
				{
					string text2 = app_.GetDataPath() + "/sd";
					if (!Directory.Exists(text2))
					{
						Directory.CreateDirectory(text2);
					}
					File.WriteAllBytes(text2 + "/" + file, data);
					Log.Info(text2 + "/" + file);
				}
				else if (kairo.common.cfg.Config.APPLI_TYPE == 2)
				{
					if (Utility.CallStatic<bool>("saveToSd", new object[2] { file, data }))
					{
						return file;
					}
					return (object)null;
				}
				return (object)null;
			}, true);
			return save != null;
		}

		public static bool SaveToTmpCaptureDirectory(string file, byte[] data, out string save)
		{
			save = (string)app_.RunOnUiThread(delegate
			{
				new JDate();
				JCalendar instance = JCalendar.GetInstance();
				string text = $"{instance.Get(1):D4}{instance.Get(2) + 1:D2}{instance.Get(5):D2}-{instance.Get(11):D2}{instance.Get(12):D2}{instance.Get(13):D2}";
				if (!file.Contains("."))
				{
					file = text + "." + file;
				}
				file = file.Replace("{date}", text);
				if (!kairo.common.cfg.Config.UNITY_EDITOR)
				{
					int aPPLI_TYPE = kairo.common.cfg.Config.APPLI_TYPE;
					int num = 2;
					return (object)null;
				}
				string text2 = app_.GetDataPath() + "/tmp/capture";
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				File.WriteAllBytes(text2 + "/" + file, data);
				Log.Info(text2 + "/" + file);
				return (object)null;
			}, true);
			return save != null;
		}

		public static void DeleteTmpCaptureDirectory()
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.UNITY_EDITOR)
				{
					string path = app_.GetDataPath() + "/tmp/capture";
					if (Directory.Exists(path))
					{
						Directory.Delete(path, true);
					}
				}
			}, true);
		}

		public static bool CloseKairoClub()
		{
			return (bool)app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.UNITY_EDITOR)
				{
					return true;
				}
				return (kairo.common.cfg.Config.APPLI_TYPE == 2) ? ((object)Utility.CallStatic<bool>("closeKairoClub", new object[0])) : ((object)false);
			}, true);
		}

		public static void Vibrator(params long[] pattern)
		{
			app_.RunOnUiThread(delegate
			{
				if (!kairo.common.cfg.Config.UNITY_EDITOR && kairo.common.cfg.Config.APPLI_TYPE == 2)
				{
					Utility.CallStatic("vibrator", pattern);
				}
			}, true);
		}

		internal static bool SupportsDepth24()
		{
			return (bool)app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.UNITY_EDITOR)
				{
					return true;
				}
				if (kairo.common.cfg.Config.APPLI_TYPE == 2)
				{
					AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.opengl.GLES20");
					if (androidJavaClass.CallStatic<string>("glGetString", new object[1] { androidJavaClass.GetStatic<int>("GL_EXTENSIONS") }).IndexOf("GL_OES_depth24", StringComparison.CurrentCultureIgnoreCase) != -1)
					{
						return true;
					}
					return false;
				}
				return true;
			}, true);
		}

		public static bool IsInstalledApplication(string packageName)
		{
			return (bool)app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.UNITY_EDITOR)
				{
					return false;
				}
				return (kairo.common.cfg.Config.APPLI_TYPE == 2) ? ((object)Utility.CallStatic<bool>("isInstalledApplication", new object[1] { packageName })) : ((object)false);
			}, true);
		}

		public static string[] LaunchBonusFlow(string data, int requestCode)
		{
			return (string[])app_.RunOnUiThread(() => (kairo.common.cfg.Config.UNITY_EDITOR || kairo.common.cfg.Config.APPLI_TYPE != 2) ? new string[3] { "-2", "", null } : kairo.unity.native.util.StringUtil.GetStrArray(AsyncThread.CallStatic(Utility, "launchBonusFlowAsync", data, requestCode)), true);
		}

		public static void LaunchLobi(string data, int type, int number)
		{
			app_.RunOnUiThread(delegate
			{
				if (!kairo.common.cfg.Config.UNITY_EDITOR && kairo.common.cfg.Config.APPLI_TYPE == 2)
				{
					AndroidJavaObject androidJavaObject = CreateLobiIntent(data, type, number);
					AndroidJavaObject androidJavaObject2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
					if (androidJavaObject2.Call<AndroidJavaObject>("getPackageManager", new object[0]).Call<AndroidJavaObject>("queryIntentActivities", new object[2] { androidJavaObject, 0 }).Call<bool>("isEmpty", new object[0]))
					{
						kairo.unity.ui.Canvas.OpenBrowser(KairoClub.SITE_URL);
					}
					else
					{
						androidJavaObject2.Call("startActivity", androidJavaObject);
					}
				}
				else
				{
					kairo.unity.ui.Canvas.OpenBrowser(KairoClub.SITE_URL);
				}
			}, true);
		}

		private static AndroidJavaObject CreateLobiIntent(string data, int type, int number)
		{
			return (AndroidJavaObject)app_.RunOnUiThread(delegate
			{
				if (!kairo.common.cfg.Config.UNITY_EDITOR && kairo.common.cfg.Config.APPLI_TYPE == 2)
				{
					AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Intent");
					AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.support.v4.content.IntentCompat");
					AndroidJavaObject androidJavaObject = ((type == 0) ? new AndroidJavaObject("android.content.Intent", "net.kairosoft.android.LOBI_CHAT") : ((type != 1) ? new AndroidJavaObject("android.content.Intent", androidJavaClass.GetStatic<string>("ACTION_MAIN")) : new AndroidJavaObject("android.content.Intent", "net.kairosoft.android.LOBI_RANKING")));
					androidJavaObject.Call<AndroidJavaObject>("setClassName", new object[2] { "net.kairosoft.android.kairoclub", "net.kairosoft.android.kairoclub.LobiMain" });
					androidJavaObject.Call<AndroidJavaObject>("setFlags", new object[1] { androidJavaClass.GetStatic<int>("FLAG_ACTIVITY_NEW_TASK") | androidJavaClass2.GetStatic<int>("FLAG_ACTIVITY_CLEAR_TASK") });
					androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2] { "net.kairosoft.android.LobiDataVersion", 100 });
					androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2] { "net.kairosoft.android.LobiData", data });
					return androidJavaObject;
				}
				return (object)null;
			}, true);
		}

		public static string GetInstallReferrer(string key = null)
		{
			return (string)app_.RunOnUiThread(delegate
			{
				string text = null;
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					text = Utility.CallStatic<string>("getInstallReferrer", new object[0]);
				}
				if (text != null && key != null)
				{
					int num = 0;
					while (num < text.Length)
					{
						int num2 = text.IndexOf('=', num);
						if (num2 == -1)
						{
							break;
						}
						string text2 = text.Substring(num, num2 - num).Trim();
						num = num2 + 1;
						num2 = text.IndexOf('&', num);
						if (num2 == -1)
						{
							num2 = text.Length;
						}
						string result = text.Substring(num, num2 - num);
						if (text2 == key)
						{
							return result;
						}
						num = num2 + 1;
					}
					return (object)null;
				}
				return text;
			}, true);
		}

		public static void startInputPanel(string title, string text, int maxLength)
		{
		}

		public static bool isEndInputPanel()
		{
			return true;
		}

		public static string getResultInputPanel()
		{
			return null;
		}

		public static void jumpStore(string ID)
		{
		}

		public static void jumpStoreKairosoft()
		{
		}

		public static void SendNotification(long delay, string text, string option = "")
		{
			app_.RunOnUiThread(delegate
			{
				long num = JSystem.CurrentTimeMillis() + delay;
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("sendNotification", num, text, option);
				}
			}, true);
		}

		internal static void CancelNotifications(bool notify = true, bool ararm = true)
		{
			if (!notify && !ararm)
			{
				return;
			}
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("cancelNotifications", notify, ararm);
				}
			}, true);
		}

		internal static string GetGCMRegistrationId()
		{
			return (string)app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? Utility.CallStatic<string>("getGCMRegistrationId", new object[0]) : null, true);
		}

		internal static bool CheckPlayServices()
		{
			return (bool)app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? ((object)Utility.CallStatic<bool>("checkPlayServices", new object[0])) : ((object)false), true);
		}

		internal static string[] GetNotificationData()
		{
			return (string[])app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? Utility.CallStatic<string[]>("getNotificationData", new object[0]) : new string[0], true);
		}

		public static void RemoveNotificationData()
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("removeNotificationData");
				}
			}, true);
		}

		internal static void SetNotificationFilter(int filterLevel)
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("setNotificationFilter", filterLevel);
				}
			}, true);
		}

		internal static int GetNotificationFilter()
		{
			return (int)app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? ((object)Utility.CallStatic<int>("getNotificationFilter", null)) : ((object)0), true);
		}

		internal static void SetNotificationBackground(bool enable)
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("setNotificationBackground", enable);
				}
			}, true);
		}

		internal static void ResetResolution()
		{
			appHeight_ = -1;
			appWidth_ = -1;
			statusBarHeight_ = int.MaxValue;
		}

		public static int IsPB()
		{
			return 0;
		}

		public static void RequestPermission(string permissionName)
		{
			if (permissionName == null || permissionName.Length == 0)
			{
				return;
			}
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("requestPermission", permissionName);
				}
			}, true);
		}

		public static bool CheckPermissionConfirmAfter(string permissionName)
		{
			if (permissionName == null || permissionName.Length == 0)
			{
				return false;
			}
			return (bool)app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? ((object)Utility.CallStatic<bool>("checkPermissionConfirmAfter", new object[1] { permissionName })) : ((object)false), true);
		}

		public static void LaunchSetting()
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("launchSetting");
				}
			}, true);
		}

		public static void StartLanguagePanel(string[] languages, int index, string customPackLocation, int mode = 0)
		{
			app_.RunOnUiThread(delegate
			{
				if (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR)
				{
					Utility.CallStatic("startLanguagePanel", languages, index, customPackLocation, mode);
				}
			}, true);
		}

		public static bool IsLanguagePanelFinish()
		{
			return (bool)app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? ((object)Utility.CallStatic<bool>("isLanguagePanelFinish", new object[0])) : ((object)true), true);
		}

		public static string[] GetLanguagePanelResult()
		{
			return (string[])app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? kairo.unity.native.util.StringUtil.GetStrArray(Utility.CallStatic<string>("getLanguagePanelResult", new object[0])) : new string[2], true);
		}

		public static bool FileExists(string path)
		{
			return (bool)app_.RunOnUiThread(() => (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? ((object)Utility.CallStatic<bool>("fileExists", new object[1] { path })) : ((object)File.Exists(path)), true);
		}

		public static byte[] ReadFile(string path)
		{
			return (byte[])app_.RunOnUiThread(delegate
			{
				if (!FileExists(path))
				{
					return (object)null;
				}
				return (kairo.common.cfg.Config.APPLI_TYPE == 2 && !kairo.common.cfg.Config.UNITY_EDITOR) ? Utility.CallStatic<byte[]>("readFile", new object[1] { path }) : File.ReadAllBytes(path);
			}, true);
		}

		public static bool IsWeakDrawDevice()
		{
			return false;
		}

		public static bool IsWeakDrawDevice(string[] devNG)
		{
			return false;
		}

		public static bool IsTapjoyRestriction()
		{
			return false;
		}
	}
}
