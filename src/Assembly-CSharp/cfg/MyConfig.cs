using kairo.common.cfg;

namespace cfg
{
	public class MyConfig : Config
	{
		public new static int APPLI_TYPE = 2;

		public new static bool DEBUG = false;

		public new static bool PTW = false;

		public new static bool COUNT_ENABLE = ((!(DEBUG)) ? false : true);

		public new static bool PRINT = ((!(DEBUG)) ? false : true);

		public new static bool USE_UNUSE_CHECK = DEBUG && !PTW;

		public new static int LANGUAGE = 1;

		public new static bool LOWDEPTH_TEST = false;

		public new static bool FULLSCREEN = false;

		public new static int AD_SIZE = 0;

		public new static bool LOCALIZE_FONT_SCALE = false;

		public new static bool ICLOUD = false;

		public new static string BASE64_PUBLIC_KEY = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA6AQ0JoxDzeN7d5vGxuRtD/dpxGHRGEavR7lt5O2fKbtK3GmGaSS9/fHyc/M3w5HkwPFk65hqKlD5k5Zz9d8PZN4gp5eM/uOU/Z5k/Rra5BffxBWDgkMod5z7WV2hx8Ge2JtMDlTz2dip80mMdDtqbFR002FxAIIT4NaY9Im881Um/DT6LZmkMm8XrYx783QlarYq4QH43n3/9+xk+znGDNWjaP0q+f7//dHXSvFXC+oDci/YXKPT/tPl+/OP2YzwmJ+9Y8oRAPRTtUQPNCCLC0zWDIMUyYdcO7kRud0QkoP1cI55ZD2UZDvIL6Fua48OmL7yy6VBRY6EC6dkLEKsBQIDAQAB";

		public new static string AD_UNIT_ID = string.Empty;

		public new static bool TRACE_URL = true;

		public new static bool DEBUG_UID = true;

		public new static string UID = "NULLGWDOCOMO";

		public new static bool REGIST_ENABLE = true;

		public new static bool TRIAL_VERSION = false;

		public new static bool ADMOB_ENABLE = false;

		public new static bool DEBUG_CMD = (DEBUG ? true : false);

		public new static bool SHOW_DEBUG = DEBUG && !PTW;

		public new static bool SHOW_FPS = DEBUG && !PTW;

		public new static bool SHOW_MEM = DEBUG && false;

		public new static bool UNITY_EDITOR = false;

		public new static int PID = 55;

		public new static string APPLI_NAME = "Soccer_en";

		public new static string OUTPUT_CSV_NAME = "PL Story";

		public static bool GOOGLE_PLAY = false;

		public static bool AMAZON = false;

		public static bool SMART_PASS = false;
	}
}
