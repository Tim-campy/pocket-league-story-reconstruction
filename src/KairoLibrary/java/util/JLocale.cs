using java.native;

namespace java.util
{
	public class JLocale
	{
		public static readonly JLocale CANADA = new JLocale("en", "CA");

		public static readonly JLocale CANADA_FRENCH = new JLocale("fr", "CA");

		public static readonly JLocale CHINA = new JLocale("zh", "CN");

		public static readonly JLocale CHINESE = new JLocale("zh", "");

		public static readonly JLocale ENGLISH = new JLocale("en", "");

		public static readonly JLocale FRANCE = new JLocale("fr", "FR");

		public static readonly JLocale FRENCH = new JLocale("fr", "");

		public static readonly JLocale GERMAN = new JLocale("de", "");

		public static readonly JLocale GERMANY = new JLocale("de", "DE");

		public static readonly JLocale ITALIAN = new JLocale("it", "");

		public static readonly JLocale ITALY = new JLocale("it", "IT");

		public static readonly JLocale JAPAN = new JLocale("ja", "JP");

		public static readonly JLocale JAPANESE = new JLocale("ja", "");

		public static readonly JLocale KOREA = new JLocale("ko", "KR");

		public static readonly JLocale KOREAN = new JLocale("ko", "");

		public static readonly JLocale PRC = new JLocale("zh", "CN");

		public static readonly JLocale ROOT = new JLocale("", "");

		public static readonly JLocale SIMPLIFIED_CHINESE = new JLocale("zh", "CN");

		public static readonly JLocale TAIWAN = new JLocale("zh", "TW");

		public static readonly JLocale TRADITIONAL_CHINESE = new JLocale("zh", "TW");

		public static readonly JLocale UK = new JLocale("en", "GB");

		public static readonly JLocale US = new JLocale("en", "US");

		private static JLocale defaultLocale;

		private string language_ = "";

		private string country_ = "";

		public static bool operator ==(JLocale obj1, JLocale obj2)
		{
			if ((object)obj1 == null || (object)obj2 == null)
			{
				return (object)obj1 == obj2;
			}
			return obj1.Equals(obj2);
		}

		public static bool operator !=(JLocale obj1, JLocale obj2)
		{
			return !(obj1 == obj2);
		}

		protected JLocale(string language)
			: this(language, "")
		{
		}

		protected JLocale(string language, string country)
		{
			language_ = language;
			country_ = country;
		}

		public static JLocale GetDefault()
		{
			if (defaultLocale == null)
			{
				string[] array = JavaPlugin.GetDefaultLocale();
				defaultLocale = new JLocale(array[0], array[1]);
			}
			return defaultLocale;
		}

		public string GetLanguage()
		{
			return language_;
		}

		public string GetCountry()
		{
			return country_;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is JLocale)
			{
				if (language_ == ((JLocale)obj).language_)
				{
					return country_ == ((JLocale)obj).country_;
				}
				return false;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
