using System;
using System.Reflection;
using java.lang;
using kairo.unity.ui;
using kairo.unity.util;

namespace kairo.common.cfg
{
	public class Config
	{
		public const int APPLITYPE_DOCOMO = 0;

		public const int APPLITYPE_SOFTBANK = 1;

		public const int APPLITYPE_ANDROID = 2;

		public const int APPLITYPE_IPHONE = 3;

		public static int APPLI_TYPE = 2;

		public const int BUILDTYPE_NORMAL = 0;

		public const int BUILDTYPE_AMAZON = 1;

		public const int BUILDTYPE_AU = 2;

		public const int BUILDTYPE_SUGO = 3;

		public static int BUILD_TYPE = 0;

		public static bool DEBUG = true;

		public static bool PTW = false;

		public static bool POST_MARK = true;

		public static bool USE_HEAP_STORAGE = true;

		public static bool USE_MEMORY = true;

		public static bool USE_MEXA_UNIT1 = true;

		public static bool COUNT_ENABLE = (!Config.DEBUG ? false : true);

		public static bool PRINT = (!Config.DEBUG ? false : true);

		public static bool USE_UNUSE_CHECK = (!Config.DEBUG ? false : !Config.PTW);

		public static bool NUMPADCONTROL = (!Config.DEBUG ? true : false);

		public static bool FAST_BRIGHTNESS = true;

		public static int LANGUAGE = -1;

		public static string BASE64_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmp4nkTi9cXIfx3bxAtIiFKYtNPnXzkx01PkfcYaRHKsZYxsOfGwxoJtlZfgovAgn5m+OsX7d0xrGON+4/fYDgqw4Pfz8Gg//g0DV3JJ202wh07OykycemKTR7DXipiXnehEIajW8hX5gIFqFX730RBcDlF0aQAEMM7mYQEw/ZMxXh0dAwC3j1AWfgAzbx1gym86Mj1tTc2p0L/GO6WFKV5NY4cIQ45AX5Z85GELL52vlpFZ4nTy90Hsy3yguAVQ9SoXbE/cEu8139Cn2SjNoykiL3EKqUVx3ux///s0e1kqJcpWkm5mV/8VlsEdFJWOv8QY3lWU3bKLp7KdeYkbDQQIDAQAB";

		public static bool UNITY_EDITOR = true;

		public static string AD_UNIT_ID = "ca-app-pub-4399146668561085/7311196364";

		public static int AD_SIZE = 0;

		public static bool FULLSCREEN = false;

		public static bool ICLOUD = false;

		public static bool LOCALIZE_FONT_SCALE = false;

		public static bool LOWDEPTH_TEST = false;

		public static bool LANDSCAPE_GAME = false;

		public static bool TRACE_URL = true;

		public static bool DEBUG_UID = true;

		public static string UID = "01NCzWojPdaL";

		public static bool REGIST_ENABLE = false;

		public static bool TRIAL_VERSION = false;

		public static bool ADMOB_ENABLE = false;

		public static bool DEBUG_CMD = (!Config.DEBUG ? false : true);

		public static bool DEBUG_SEND_SC = (!Config.DEBUG ? false : false);

		public static bool DEBUG_CMD_DEV = (!Config.DEBUG ? false : true);

		public static bool SHOW_DEBUG = (!Config.DEBUG ? false : !Config.PTW);

		public static bool SHOW_FPS = (!Config.DEBUG ? false : !Config.PTW);

		public static bool SHOW_MEM = (!Config.DEBUG ? false : false);

		public static bool MUTE_SE = (!Config.DEBUG ? false : !Config.PTW);

		public static bool FPS_UNLIMITED = (!Config.DEBUG ? false : false);

		public static bool SHOW_GUIDEPOINT = (!Config.DEBUG ? false : !Config.PTW);

		public static bool DEBUG_SLOW = (!Config.DEBUG ? false : !Config.PTW);

		public static bool DEBUG_FAST = (!Config.DEBUG ? false : true);

		public static bool DEBUG_MONEYUP = (!Config.DEBUG ? false : !Config.PTW);

		public static int drawImageCount_;

		public static int drawStringCount_;

		public static int drawShapeCount_;

		public static int drawImageArea_;

		public static int drawSourceImageArea_;

		public static int createImageArea_;

		public static int checkCount_;

		public static int PID = 25;

		public static string PID_NAME = "cGlk";

		private static string CGI_COMMON_QUERY;

		public static string APPLI_NAME = "ks_android_ja";

		public static string OUTPUT_CSV_NAME = "output";

		public static string URL_REGIST;

		public static string URL_RANKING;

		public static string URL_REGIST_SCORE;

		public static string URL_GET_SCORE;

		public static string URL_DATA;

		public static string URL_SITE;

		public static string URL_VERSIONUP;

		public static string URL_SP;

		public static string URL_PACKAGE;

		public static string URL_PHP;

		private static bool[] checkVRAM_result = null;

		private static string platform_;

		protected Config()
		{
		}

		public static void Create(Type config)
		{
			if (config == null)
			{
				return;
			}
			Type typeFromHandle = typeof(Config);
			FieldInfo[] fields = config.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!(fieldInfo.Name == "APPLI_TYPE"))
				{
					FieldInfo field = typeFromHandle.GetField(fieldInfo.Name);
					if (field != null)
					{
						field.SetValue(null, fieldInfo.GetValue(null));
					}
				}
			}
		}

		public static void Init(string sourceUrl)
		{
			if (sourceUrl == null)
			{
				sourceUrl = "";
			}
			string text = APPLI_NAME;
			int num = APPLI_NAME.IndexOf('_');
			if (num != -1)
			{
				text = text.SubstringJ(0, num);
			}
			int num2 = -1;
			if (APPLI_TYPE == 1)
			{
				if (sourceUrl.StartsWith("http://macslab"))
				{
					num2 = 0;
					URL_PACKAGE = "http://kaipakaipa.fam.cx/SB/game/gm" + PID + "/bin/" + APPLI_NAME + ".jar";
				}
				else if (sourceUrl.StartsWith("http://macs"))
				{
					num2 = 1;
					URL_PACKAGE = "http://kaipa.jp/SB/game/gm" + PID + "/bin/" + APPLI_NAME + ".jar";
				}
				else
				{
					num2 = 2;
					URL_PACKAGE = "http://kairosoft.net/appli/" + text + "/bin/" + APPLI_NAME + ".jar";
				}
				sourceUrl = URL_PACKAGE;
			}
			if (APPLI_TYPE == 2 || APPLI_TYPE == 3)
			{
				URL_PACKAGE = "http://kairopark.jp/j7hjT4ua/gm" + PID + "/" + APPLI_NAME + ".jar";
				sourceUrl = URL_PACKAGE;
			}
			string text2 = sourceUrl.SubstringJ(0, sourceUrl.LastIndexOf('/') + 1);
			string text3 = text2.SubstringJ(0, text2.IndexOf("/", 7)) + "/";
			string text4 = text2 + ((APPLI_TYPE == 2) ? "" : "../../cgi/");
			URL_REGIST = text4 + "regist.php?" + CGI_COMMON_QUERY + "&pid=" + PID;
			URL_RANKING = text4 + "ranking.php?" + CGI_COMMON_QUERY + "&pid=" + PID;
			URL_REGIST_SCORE = text4 + "regist_score.php?" + CGI_COMMON_QUERY + "&pid=" + PID;
			URL_GET_SCORE = text4 + "get_score.php?" + CGI_COMMON_QUERY + "&pid=" + PID;
			URL_DATA = text2 + ((num2 != -1) ? "sb/" : "903/");
			URL_SITE = text3 + "?" + CGI_COMMON_QUERY + "&from=gm" + PID;
			URL_SP = text2 + "save.php?" + CGI_COMMON_QUERY + "&pid=" + PID;
			URL_PACKAGE = sourceUrl;
			URL_PHP = text2 + "php/";
			if (APPLI_TYPE == 2)
			{
				URL_SITE = "http://kairopark.jp/android/";
				if (BUILD_TYPE == 1)
				{
					URL_SITE += "amazon/";
				}
				if (!Language.Japanese())
				{
					URL_SITE += "en/";
				}
				URL_SITE = URL_SITE + "?pid=" + PID + "&from=on50";
			}
			else if (APPLI_TYPE == 3)
			{
				URL_SITE = (Language.Japanese() ? ("http://kairopark.jp/iphone/?pid=" + PID + "&from=on50") : ("http://kairopark.jp/iphone/en/?pid=" + PID + "&from=on50"));
			}
			if (APPLI_TYPE == 1)
			{
				if (num2 == 0)
				{
					URL_SP = "http://kaipakaipa.fam.cx/SB/game/2Eo9mAkM/spout/gm" + PID + "/";
					URL_SITE = "http://kaipakaipa.fam.cx/?uid=1&sid=PL91&from=gm" + PID;
					URL_VERSIONUP = "http://kaipakaipa.fam.cx/i/f/game.php?cmd=download&" + PID_NAME + "=" + PID + "&v=1";
				}
				else if (num2 == 1)
				{
					URL_SP = "http://kaipa.jp/SB/game/2Eo9mAkM/spout/gm" + PID + "/";
					URL_SITE = "http://kaipa.jp/?uid=1&sid=PL91&from=gm" + PID;
					URL_VERSIONUP = "http://kaipa.jp/i/f/game.php?cmd=download&" + PID_NAME + "=" + PID + "&v=1";
				}
				else if (num2 == 2)
				{
					URL_SP = "http://kairosoft.net/appli/" + text + "/bin/SB/";
					URL_SITE = "http://vf-dev.appget.com/download/htm/AC20389/" + APPLI_NAME + ".htm";
					URL_VERSIONUP = "http://vf-dev.appget.com/download/htm/AC20389/" + APPLI_NAME + ".htm";
				}
			}
		}

		public static bool CheckVRAM()
		{
			if (checkVRAM_result == null)
			{
				checkVRAM_result = new bool[1];
				if (APPLI_TYPE == 0)
				{
					checkVRAM_result[0] = false;
				}
				else
				{
					int num = JSystem.GetProperty("Platform").IndexOf("SH");
					checkVRAM_result[0] = num != -1 && num > 0;
				}
			}
			return checkVRAM_result[0];
		}

		public static string GetPlatform()
		{
			if (platform_ == null)
			{
				platform_ = IApplication.GetProperty("microedition.platform");
			}
			return platform_;
		}

		public static string ConvertExtension(string file)
		{
			if (APPLI_TYPE == 1)
			{
				file = StringUtil.Replace(file, ".gif", ".png");
				file = StringUtil.Replace(file, ".mld", ".spf");
			}
			if (APPLI_TYPE == 2 || APPLI_TYPE == 3)
			{
				file = StringUtil.Replace(file, ".gif", ".png");
				file = StringUtil.Replace(file, ".mld", ".ogg");
			}
			return file;
		}
	}
}
