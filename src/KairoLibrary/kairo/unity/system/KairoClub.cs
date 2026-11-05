using kairo.unity.native;
using kairo.unity.util;

namespace kairo.unity.system
{
	public class KairoClub
	{
		public const string APP_NAME = "カイロクラブ";

		public static readonly SecureString PACKAGE_NAME = new SecureString("net.kairosoft.android.kairoclub");

		public static readonly SecureString CLASS_NAME = new SecureString(PACKAGE_NAME.Get() + ".Main");

		public static readonly SecureString LOBI_CLASS_NAME = new SecureString(PACKAGE_NAME.Get() + ".LobiMain");

		public static readonly string SITE_URL = (Language.Japanese() ? "http://kairopark.jp/android/kairoclub/guide.html" : "http://kairopark.jp/android/kairoclub/guide_en.html");

		public static bool FinishActivity()
		{
			return KairoPlugin.CloseKairoClub();
		}
	}
}
