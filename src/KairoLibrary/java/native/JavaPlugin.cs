using System.Collections.Generic;
using kairo.common.cfg;
using kairo.unity.ui;
using UnityEngine;

namespace java.native
{
	public static class JavaPlugin
	{
		private static AndroidJavaClass Locale;

		private static AndroidJavaClass TimeZone;

		private static AndroidJavaClass Runtime;

		private static bool init_;

		private static Dictionary<string, int> utcOffsetTable_ = new Dictionary<string, int>();

		public static void Init()
		{
			if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
			{
				Locale = new AndroidJavaClass("java.util.Locale");
				TimeZone = new AndroidJavaClass("java.util.TimeZone");
				Runtime = new AndroidJavaClass("java.lang.Runtime");
			}
			init_ = true;
		}

		public static string[] GetDefaultLocale()
		{
			return (string[])IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (!init_)
				{
					Init();
				}
				string text = "ja";
				string text2 = "JP";
				if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
				{
					using (AndroidJavaObject androidJavaObject = Locale.CallStatic<AndroidJavaObject>("getDefault", new object[0]))
					{
						text = androidJavaObject.Call<string>("getLanguage", new object[0]);
						text2 = androidJavaObject.Call<string>("getCountry", new object[0]);
					}
				}
				return new string[2] { text, text2 };
			}, true);
		}

		public static string GetDefaultTimeZone()
		{
			return (string)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (!init_)
				{
					Init();
				}
				if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
				{
					using (AndroidJavaObject androidJavaObject = TimeZone.CallStatic<AndroidJavaObject>("getDefault", new object[0]))
					{
						return androidJavaObject.Call<string>("getID", new object[0]);
					}
				}
				return "Asia/Tokyo";
			}, true);
		}

		public static int GetUtcOffset(string timezone)
		{
			return (int)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (!init_)
				{
					Init();
				}
				if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
				{
					using (AndroidJavaObject androidJavaObject = TimeZone.CallStatic<AndroidJavaObject>("getTimeZone", new object[1] { timezone }))
					{
						return androidJavaObject.Call<int>("getRawOffset", new object[0]);
					}
				}
				if (utcOffsetTable_.Count == 0)
				{
					CreateUtcOffsetTable();
				}
				int value = 0;
				utcOffsetTable_.TryGetValue(timezone, out value);
				return value;
			}, true);
		}

		public static long FreeMemory()
		{
			return (long)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
				{
					using (AndroidJavaObject androidJavaObject = Runtime.CallStatic<AndroidJavaObject>("getRuntime", new object[0]))
					{
						return androidJavaObject.Call<long>("freeMemory", new object[0]);
					}
				}
				return (long)(Profiler.GetMonoHeapSize() - Profiler.GetMonoUsedSize());
			}, true);
		}

		public static long TotalMemory()
		{
			return (long)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR)
				{
					using (AndroidJavaObject androidJavaObject = Runtime.CallStatic<AndroidJavaObject>("getRuntime", new object[0]))
					{
						return androidJavaObject.Call<long>("totalMemory", new object[0]);
					}
				}
				return (long)Profiler.GetMonoHeapSize();
			}, true);
		}

		private static void CreateUtcOffsetTable()
		{
			utcOffsetTable_.Add("Etc/GMT+12", -43200000);
			utcOffsetTable_.Add("Etc/GMT+11", -39600000);
			utcOffsetTable_.Add("Pacific/Midway", -39600000);
			utcOffsetTable_.Add("Pacific/Niue", -39600000);
			utcOffsetTable_.Add("Pacific/Pago_Pago", -39600000);
			utcOffsetTable_.Add("Pacific/Samoa", -39600000);
			utcOffsetTable_.Add("US/Samoa", -39600000);
			utcOffsetTable_.Add("America/Adak", -36000000);
			utcOffsetTable_.Add("America/Atka", -36000000);
			utcOffsetTable_.Add("Etc/GMT+10", -36000000);
			utcOffsetTable_.Add("HST", -36000000);
			utcOffsetTable_.Add("Pacific/Honolulu", -36000000);
			utcOffsetTable_.Add("Pacific/Johnston", -36000000);
			utcOffsetTable_.Add("Pacific/Rarotonga", -36000000);
			utcOffsetTable_.Add("Pacific/Tahiti", -36000000);
			utcOffsetTable_.Add("SystemV/HST10", -36000000);
			utcOffsetTable_.Add("US/Aleutian", -36000000);
			utcOffsetTable_.Add("US/Hawaii", -36000000);
			utcOffsetTable_.Add("Pacific/Marquesas", -34200000);
			utcOffsetTable_.Add("AST", -32400000);
			utcOffsetTable_.Add("America/Anchorage", -32400000);
			utcOffsetTable_.Add("America/Juneau", -32400000);
			utcOffsetTable_.Add("America/Nome", -32400000);
			utcOffsetTable_.Add("America/Sitka", -32400000);
			utcOffsetTable_.Add("America/Yakutat", -32400000);
			utcOffsetTable_.Add("Etc/GMT+9", -32400000);
			utcOffsetTable_.Add("Pacific/Gambier", -32400000);
			utcOffsetTable_.Add("SystemV/YST9", -32400000);
			utcOffsetTable_.Add("SystemV/YST9YDT", -32400000);
			utcOffsetTable_.Add("US/Alaska", -32400000);
			utcOffsetTable_.Add("America/Dawson", -28800000);
			utcOffsetTable_.Add("America/Ensenada", -28800000);
			utcOffsetTable_.Add("America/Los_Angeles", -28800000);
			utcOffsetTable_.Add("America/Metlakatla", -28800000);
			utcOffsetTable_.Add("America/Santa_Isabel", -28800000);
			utcOffsetTable_.Add("America/Tijuana", -28800000);
			utcOffsetTable_.Add("America/Vancouver", -28800000);
			utcOffsetTable_.Add("America/Whitehorse", -28800000);
			utcOffsetTable_.Add("Canada/Pacific", -28800000);
			utcOffsetTable_.Add("Canada/Yukon", -28800000);
			utcOffsetTable_.Add("Etc/GMT+8", -28800000);
			utcOffsetTable_.Add("Mexico/BajaNorte", -28800000);
			utcOffsetTable_.Add("PST", -28800000);
			utcOffsetTable_.Add("PST8PDT", -28800000);
			utcOffsetTable_.Add("Pacific/Pitcairn", -28800000);
			utcOffsetTable_.Add("SystemV/PST8", -28800000);
			utcOffsetTable_.Add("SystemV/PST8PDT", -28800000);
			utcOffsetTable_.Add("US/Pacific", -28800000);
			utcOffsetTable_.Add("US/Pacific-New", -28800000);
			utcOffsetTable_.Add("America/Boise", -25200000);
			utcOffsetTable_.Add("America/Cambridge_Bay", -25200000);
			utcOffsetTable_.Add("America/Chihuahua", -25200000);
			utcOffsetTable_.Add("America/Creston", -25200000);
			utcOffsetTable_.Add("America/Dawson_Creek", -25200000);
			utcOffsetTable_.Add("America/Denver", -25200000);
			utcOffsetTable_.Add("America/Edmonton", -25200000);
			utcOffsetTable_.Add("America/Hermosillo", -25200000);
			utcOffsetTable_.Add("America/Inuvik", -25200000);
			utcOffsetTable_.Add("America/Mazatlan", -25200000);
			utcOffsetTable_.Add("America/Ojinaga", -25200000);
			utcOffsetTable_.Add("America/Phoenix", -25200000);
			utcOffsetTable_.Add("America/Shiprock", -25200000);
			utcOffsetTable_.Add("America/Yellowknife", -25200000);
			utcOffsetTable_.Add("Canada/Mountain", -25200000);
			utcOffsetTable_.Add("Etc/GMT+7", -25200000);
			utcOffsetTable_.Add("MST", -25200000);
			utcOffsetTable_.Add("MST7MDT", -25200000);
			utcOffsetTable_.Add("Mexico/BajaSur", -25200000);
			utcOffsetTable_.Add("Navajo", -25200000);
			utcOffsetTable_.Add("PNT", -25200000);
			utcOffsetTable_.Add("SystemV/MST7", -25200000);
			utcOffsetTable_.Add("SystemV/MST7MDT", -25200000);
			utcOffsetTable_.Add("US/Arizona", -25200000);
			utcOffsetTable_.Add("US/Mountain", -25200000);
			utcOffsetTable_.Add("America/Bahia_Banderas", -21600000);
			utcOffsetTable_.Add("America/Belize", -21600000);
			utcOffsetTable_.Add("America/Chicago", -21600000);
			utcOffsetTable_.Add("America/Costa_Rica", -21600000);
			utcOffsetTable_.Add("America/El_Salvador", -21600000);
			utcOffsetTable_.Add("America/Guatemala", -21600000);
			utcOffsetTable_.Add("America/Indiana/Knox", -21600000);
			utcOffsetTable_.Add("America/Indiana/Tell_City", -21600000);
			utcOffsetTable_.Add("America/Knox_IN", -21600000);
			utcOffsetTable_.Add("America/Managua", -21600000);
			utcOffsetTable_.Add("America/Matamoros", -21600000);
			utcOffsetTable_.Add("America/Menominee", -21600000);
			utcOffsetTable_.Add("America/Merida", -21600000);
			utcOffsetTable_.Add("America/Mexico_City", -21600000);
			utcOffsetTable_.Add("America/Monterrey", -21600000);
			utcOffsetTable_.Add("America/North_Dakota/Beulah", -21600000);
			utcOffsetTable_.Add("America/North_Dakota/Center", -21600000);
			utcOffsetTable_.Add("America/North_Dakota/New_Salem", -21600000);
			utcOffsetTable_.Add("America/Rainy_River", -21600000);
			utcOffsetTable_.Add("America/Rankin_Inlet", -21600000);
			utcOffsetTable_.Add("America/Regina", -21600000);
			utcOffsetTable_.Add("America/Resolute", -21600000);
			utcOffsetTable_.Add("America/Swift_Current", -21600000);
			utcOffsetTable_.Add("America/Tegucigalpa", -21600000);
			utcOffsetTable_.Add("America/Winnipeg", -21600000);
			utcOffsetTable_.Add("CST", -21600000);
			utcOffsetTable_.Add("CST6CDT", -21600000);
			utcOffsetTable_.Add("Canada/Central", -21600000);
			utcOffsetTable_.Add("Canada/East-Saskatchewan", -21600000);
			utcOffsetTable_.Add("Canada/Saskatchewan", -21600000);
			utcOffsetTable_.Add("Etc/GMT+6", -21600000);
			utcOffsetTable_.Add("Mexico/General", -21600000);
			utcOffsetTable_.Add("Pacific/Galapagos", -21600000);
			utcOffsetTable_.Add("SystemV/CST6", -21600000);
			utcOffsetTable_.Add("SystemV/CST6CDT", -21600000);
			utcOffsetTable_.Add("US/Central", -21600000);
			utcOffsetTable_.Add("US/Indiana-Starke", -21600000);
			utcOffsetTable_.Add("America/Atikokan", -18000000);
			utcOffsetTable_.Add("America/Bogota", -18000000);
			utcOffsetTable_.Add("America/Cancun", -18000000);
			utcOffsetTable_.Add("America/Cayman", -18000000);
			utcOffsetTable_.Add("America/Coral_Harbour", -18000000);
			utcOffsetTable_.Add("America/Detroit", -18000000);
			utcOffsetTable_.Add("America/Eirunepe", -18000000);
			utcOffsetTable_.Add("America/Fort_Wayne", -18000000);
			utcOffsetTable_.Add("America/Guayaquil", -18000000);
			utcOffsetTable_.Add("America/Havana", -18000000);
			utcOffsetTable_.Add("America/Indiana/Indianapolis", -18000000);
			utcOffsetTable_.Add("America/Indiana/Marengo", -18000000);
			utcOffsetTable_.Add("America/Indiana/Petersburg", -18000000);
			utcOffsetTable_.Add("America/Indiana/Vevay", -18000000);
			utcOffsetTable_.Add("America/Indiana/Vincennes", -18000000);
			utcOffsetTable_.Add("America/Indiana/Winamac", -18000000);
			utcOffsetTable_.Add("America/Indianapolis", -18000000);
			utcOffsetTable_.Add("America/Iqaluit", -18000000);
			utcOffsetTable_.Add("America/Jamaica", -18000000);
			utcOffsetTable_.Add("America/Kentucky/Louisville", -18000000);
			utcOffsetTable_.Add("America/Kentucky/Monticello", -18000000);
			utcOffsetTable_.Add("America/Lima", -18000000);
			utcOffsetTable_.Add("America/Louisville", -18000000);
			utcOffsetTable_.Add("America/Montreal", -18000000);
			utcOffsetTable_.Add("America/Nassau", -18000000);
			utcOffsetTable_.Add("America/New_York", -18000000);
			utcOffsetTable_.Add("America/Nipigon", -18000000);
			utcOffsetTable_.Add("America/Panama", -18000000);
			utcOffsetTable_.Add("America/Pangnirtung", -18000000);
			utcOffsetTable_.Add("America/Port-au-Prince", -18000000);
			utcOffsetTable_.Add("America/Porto_Acre", -18000000);
			utcOffsetTable_.Add("America/Rio_Branco", -18000000);
			utcOffsetTable_.Add("America/Thunder_Bay", -18000000);
			utcOffsetTable_.Add("America/Toronto", -18000000);
			utcOffsetTable_.Add("Brazil/Acre", -18000000);
			utcOffsetTable_.Add("Canada/Eastern", -18000000);
			utcOffsetTable_.Add("Chile/EasterIsland", -18000000);
			utcOffsetTable_.Add("Cuba", -18000000);
			utcOffsetTable_.Add("EST", -18000000);
			utcOffsetTable_.Add("EST5EDT", -18000000);
			utcOffsetTable_.Add("Etc/GMT+5", -18000000);
			utcOffsetTable_.Add("IET", -18000000);
			utcOffsetTable_.Add("Jamaica", -18000000);
			utcOffsetTable_.Add("Pacific/Easter", -18000000);
			utcOffsetTable_.Add("SystemV/EST5", -18000000);
			utcOffsetTable_.Add("SystemV/EST5EDT", -18000000);
			utcOffsetTable_.Add("US/East-Indiana", -18000000);
			utcOffsetTable_.Add("US/Eastern", -18000000);
			utcOffsetTable_.Add("US/Michigan", -18000000);
			utcOffsetTable_.Add("America/Caracas", -16200000);
			utcOffsetTable_.Add("America/Anguilla", -14400000);
			utcOffsetTable_.Add("America/Antigua", -14400000);
			utcOffsetTable_.Add("America/Aruba", -14400000);
			utcOffsetTable_.Add("America/Asuncion", -14400000);
			utcOffsetTable_.Add("America/Barbados", -14400000);
			utcOffsetTable_.Add("America/Blanc-Sablon", -14400000);
			utcOffsetTable_.Add("America/Boa_Vista", -14400000);
			utcOffsetTable_.Add("America/Campo_Grande", -14400000);
			utcOffsetTable_.Add("America/Cuiaba", -14400000);
			utcOffsetTable_.Add("America/Curacao", -14400000);
			utcOffsetTable_.Add("America/Dominica", -14400000);
			utcOffsetTable_.Add("America/Glace_Bay", -14400000);
			utcOffsetTable_.Add("America/Goose_Bay", -14400000);
			utcOffsetTable_.Add("America/Grand_Turk", -14400000);
			utcOffsetTable_.Add("America/Grenada", -14400000);
			utcOffsetTable_.Add("America/Guadeloupe", -14400000);
			utcOffsetTable_.Add("America/Guyana", -14400000);
			utcOffsetTable_.Add("America/Halifax", -14400000);
			utcOffsetTable_.Add("America/Kralendijk", -14400000);
			utcOffsetTable_.Add("America/La_Paz", -14400000);
			utcOffsetTable_.Add("America/Lower_Princes", -14400000);
			utcOffsetTable_.Add("America/Manaus", -14400000);
			utcOffsetTable_.Add("America/Marigot", -14400000);
			utcOffsetTable_.Add("America/Martinique", -14400000);
			utcOffsetTable_.Add("America/Moncton", -14400000);
			utcOffsetTable_.Add("America/Montserrat", -14400000);
			utcOffsetTable_.Add("America/Port_of_Spain", -14400000);
			utcOffsetTable_.Add("America/Porto_Velho", -14400000);
			utcOffsetTable_.Add("America/Puerto_Rico", -14400000);
			utcOffsetTable_.Add("America/Santo_Domingo", -14400000);
			utcOffsetTable_.Add("America/St_Barthelemy", -14400000);
			utcOffsetTable_.Add("America/St_Kitts", -14400000);
			utcOffsetTable_.Add("America/St_Lucia", -14400000);
			utcOffsetTable_.Add("America/St_Thomas", -14400000);
			utcOffsetTable_.Add("America/St_Vincent", -14400000);
			utcOffsetTable_.Add("America/Thule", -14400000);
			utcOffsetTable_.Add("America/Tortola", -14400000);
			utcOffsetTable_.Add("America/Virgin", -14400000);
			utcOffsetTable_.Add("Atlantic/Bermuda", -14400000);
			utcOffsetTable_.Add("Brazil/West", -14400000);
			utcOffsetTable_.Add("Canada/Atlantic", -14400000);
			utcOffsetTable_.Add("Etc/GMT+4", -14400000);
			utcOffsetTable_.Add("PRT", -14400000);
			utcOffsetTable_.Add("SystemV/AST4", -14400000);
			utcOffsetTable_.Add("SystemV/AST4ADT", -14400000);
			utcOffsetTable_.Add("America/St_Johns", -12600000);
			utcOffsetTable_.Add("CNT", -12600000);
			utcOffsetTable_.Add("Canada/Newfoundland", -12600000);
			utcOffsetTable_.Add("AGT", -10800000);
			utcOffsetTable_.Add("America/Araguaina", -10800000);
			utcOffsetTable_.Add("America/Argentina/Buenos_Aires", -10800000);
			utcOffsetTable_.Add("America/Argentina/Catamarca", -10800000);
			utcOffsetTable_.Add("America/Argentina/ComodRivadavia", -10800000);
			utcOffsetTable_.Add("America/Argentina/Cordoba", -10800000);
			utcOffsetTable_.Add("America/Argentina/Jujuy", -10800000);
			utcOffsetTable_.Add("America/Argentina/La_Rioja", -10800000);
			utcOffsetTable_.Add("America/Argentina/Mendoza", -10800000);
			utcOffsetTable_.Add("America/Argentina/Rio_Gallegos", -10800000);
			utcOffsetTable_.Add("America/Argentina/Salta", -10800000);
			utcOffsetTable_.Add("America/Argentina/San_Juan", -10800000);
			utcOffsetTable_.Add("America/Argentina/San_Luis", -10800000);
			utcOffsetTable_.Add("America/Argentina/Tucuman", -10800000);
			utcOffsetTable_.Add("America/Argentina/Ushuaia", -10800000);
			utcOffsetTable_.Add("America/Bahia", -10800000);
			utcOffsetTable_.Add("America/Belem", -10800000);
			utcOffsetTable_.Add("America/Buenos_Aires", -10800000);
			utcOffsetTable_.Add("America/Catamarca", -10800000);
			utcOffsetTable_.Add("America/Cayenne", -10800000);
			utcOffsetTable_.Add("America/Cordoba", -10800000);
			utcOffsetTable_.Add("America/Fortaleza", -10800000);
			utcOffsetTable_.Add("America/Godthab", -10800000);
			utcOffsetTable_.Add("America/Jujuy", -10800000);
			utcOffsetTable_.Add("America/Maceio", -10800000);
			utcOffsetTable_.Add("America/Mendoza", -10800000);
			utcOffsetTable_.Add("America/Miquelon", -10800000);
			utcOffsetTable_.Add("America/Montevideo", -10800000);
			utcOffsetTable_.Add("America/Paramaribo", -10800000);
			utcOffsetTable_.Add("America/Recife", -10800000);
			utcOffsetTable_.Add("America/Rosario", -10800000);
			utcOffsetTable_.Add("America/Santarem", -10800000);
			utcOffsetTable_.Add("America/Santiago", -10800000);
			utcOffsetTable_.Add("America/Sao_Paulo", -10800000);
			utcOffsetTable_.Add("Antarctica/Palmer", -10800000);
			utcOffsetTable_.Add("Antarctica/Rothera", -10800000);
			utcOffsetTable_.Add("Atlantic/Stanley", -10800000);
			utcOffsetTable_.Add("BET", -10800000);
			utcOffsetTable_.Add("Brazil/East", -10800000);
			utcOffsetTable_.Add("Chile/Continental", -10800000);
			utcOffsetTable_.Add("Etc/GMT+3", -10800000);
			utcOffsetTable_.Add("America/Noronha", -7200000);
			utcOffsetTable_.Add("Atlantic/South_Georgia", -7200000);
			utcOffsetTable_.Add("Brazil/DeNoronha", -7200000);
			utcOffsetTable_.Add("Etc/GMT+2", -7200000);
			utcOffsetTable_.Add("America/Scoresbysund", -3600000);
			utcOffsetTable_.Add("Atlantic/Azores", -3600000);
			utcOffsetTable_.Add("Atlantic/Cape_Verde", -3600000);
			utcOffsetTable_.Add("Etc/GMT+1", -3600000);
			utcOffsetTable_.Add("Africa/Abidjan", 0);
			utcOffsetTable_.Add("Africa/Accra", 0);
			utcOffsetTable_.Add("Africa/Bamako", 0);
			utcOffsetTable_.Add("Africa/Banjul", 0);
			utcOffsetTable_.Add("Africa/Bissau", 0);
			utcOffsetTable_.Add("Africa/Casablanca", 0);
			utcOffsetTable_.Add("Africa/Conakry", 0);
			utcOffsetTable_.Add("Africa/Dakar", 0);
			utcOffsetTable_.Add("Africa/El_Aaiun", 0);
			utcOffsetTable_.Add("Africa/Freetown", 0);
			utcOffsetTable_.Add("Africa/Lome", 0);
			utcOffsetTable_.Add("Africa/Monrovia", 0);
			utcOffsetTable_.Add("Africa/Nouakchott", 0);
			utcOffsetTable_.Add("Africa/Ouagadougou", 0);
			utcOffsetTable_.Add("Africa/Sao_Tome", 0);
			utcOffsetTable_.Add("Africa/Timbuktu", 0);
			utcOffsetTable_.Add("America/Danmarkshavn", 0);
			utcOffsetTable_.Add("Antarctica/Troll", 0);
			utcOffsetTable_.Add("Atlantic/Canary", 0);
			utcOffsetTable_.Add("Atlantic/Faeroe", 0);
			utcOffsetTable_.Add("Atlantic/Faroe", 0);
			utcOffsetTable_.Add("Atlantic/Madeira", 0);
			utcOffsetTable_.Add("Atlantic/Reykjavik", 0);
			utcOffsetTable_.Add("Atlantic/St_Helena", 0);
			utcOffsetTable_.Add("Eire", 0);
			utcOffsetTable_.Add("Etc/GMT", 0);
			utcOffsetTable_.Add("Etc/GMT+0", 0);
			utcOffsetTable_.Add("Etc/GMT-0", 0);
			utcOffsetTable_.Add("Etc/GMT0", 0);
			utcOffsetTable_.Add("Etc/Greenwich", 0);
			utcOffsetTable_.Add("Etc/UCT", 0);
			utcOffsetTable_.Add("Etc/UTC", 0);
			utcOffsetTable_.Add("Etc/Universal", 0);
			utcOffsetTable_.Add("Etc/Zulu", 0);
			utcOffsetTable_.Add("Europe/Belfast", 0);
			utcOffsetTable_.Add("Europe/Dublin", 0);
			utcOffsetTable_.Add("Europe/Guernsey", 0);
			utcOffsetTable_.Add("Europe/Isle_of_Man", 0);
			utcOffsetTable_.Add("Europe/Jersey", 0);
			utcOffsetTable_.Add("Europe/Lisbon", 0);
			utcOffsetTable_.Add("Europe/London", 0);
			utcOffsetTable_.Add("GB", 0);
			utcOffsetTable_.Add("GB-Eire", 0);
			utcOffsetTable_.Add("GMT", 0);
			utcOffsetTable_.Add("GMT0", 0);
			utcOffsetTable_.Add("Greenwich", 0);
			utcOffsetTable_.Add("Iceland", 0);
			utcOffsetTable_.Add("Portugal", 0);
			utcOffsetTable_.Add("UCT", 0);
			utcOffsetTable_.Add("UTC", 0);
			utcOffsetTable_.Add("Universal", 0);
			utcOffsetTable_.Add("WET", 0);
			utcOffsetTable_.Add("Zulu", 0);
			utcOffsetTable_.Add("Africa/Algiers", 3600000);
			utcOffsetTable_.Add("Africa/Bangui", 3600000);
			utcOffsetTable_.Add("Africa/Brazzaville", 3600000);
			utcOffsetTable_.Add("Africa/Ceuta", 3600000);
			utcOffsetTable_.Add("Africa/Douala", 3600000);
			utcOffsetTable_.Add("Africa/Kinshasa", 3600000);
			utcOffsetTable_.Add("Africa/Lagos", 3600000);
			utcOffsetTable_.Add("Africa/Libreville", 3600000);
			utcOffsetTable_.Add("Africa/Luanda", 3600000);
			utcOffsetTable_.Add("Africa/Malabo", 3600000);
			utcOffsetTable_.Add("Africa/Ndjamena", 3600000);
			utcOffsetTable_.Add("Africa/Niamey", 3600000);
			utcOffsetTable_.Add("Africa/Porto-Novo", 3600000);
			utcOffsetTable_.Add("Africa/Tunis", 3600000);
			utcOffsetTable_.Add("Africa/Windhoek", 3600000);
			utcOffsetTable_.Add("Arctic/Longyearbyen", 3600000);
			utcOffsetTable_.Add("Atlantic/Jan_Mayen", 3600000);
			utcOffsetTable_.Add("CET", 3600000);
			utcOffsetTable_.Add("ECT", 3600000);
			utcOffsetTable_.Add("Etc/GMT-1", 3600000);
			utcOffsetTable_.Add("Europe/Amsterdam", 3600000);
			utcOffsetTable_.Add("Europe/Andorra", 3600000);
			utcOffsetTable_.Add("Europe/Belgrade", 3600000);
			utcOffsetTable_.Add("Europe/Berlin", 3600000);
			utcOffsetTable_.Add("Europe/Bratislava", 3600000);
			utcOffsetTable_.Add("Europe/Brussels", 3600000);
			utcOffsetTable_.Add("Europe/Budapest", 3600000);
			utcOffsetTable_.Add("Europe/Busingen", 3600000);
			utcOffsetTable_.Add("Europe/Copenhagen", 3600000);
			utcOffsetTable_.Add("Europe/Gibraltar", 3600000);
			utcOffsetTable_.Add("Europe/Ljubljana", 3600000);
			utcOffsetTable_.Add("Europe/Luxembourg", 3600000);
			utcOffsetTable_.Add("Europe/Madrid", 3600000);
			utcOffsetTable_.Add("Europe/Malta", 3600000);
			utcOffsetTable_.Add("Europe/Monaco", 3600000);
			utcOffsetTable_.Add("Europe/Oslo", 3600000);
			utcOffsetTable_.Add("Europe/Paris", 3600000);
			utcOffsetTable_.Add("Europe/Podgorica", 3600000);
			utcOffsetTable_.Add("Europe/Prague", 3600000);
			utcOffsetTable_.Add("Europe/Rome", 3600000);
			utcOffsetTable_.Add("Europe/San_Marino", 3600000);
			utcOffsetTable_.Add("Europe/Sarajevo", 3600000);
			utcOffsetTable_.Add("Europe/Skopje", 3600000);
			utcOffsetTable_.Add("Europe/Stockholm", 3600000);
			utcOffsetTable_.Add("Europe/Tirane", 3600000);
			utcOffsetTable_.Add("Europe/Vaduz", 3600000);
			utcOffsetTable_.Add("Europe/Vatican", 3600000);
			utcOffsetTable_.Add("Europe/Vienna", 3600000);
			utcOffsetTable_.Add("Europe/Warsaw", 3600000);
			utcOffsetTable_.Add("Europe/Zagreb", 3600000);
			utcOffsetTable_.Add("Europe/Zurich", 3600000);
			utcOffsetTable_.Add("MET", 3600000);
			utcOffsetTable_.Add("Poland", 3600000);
			utcOffsetTable_.Add("ART", 7200000);
			utcOffsetTable_.Add("Africa/Blantyre", 7200000);
			utcOffsetTable_.Add("Africa/Bujumbura", 7200000);
			utcOffsetTable_.Add("Africa/Cairo", 7200000);
			utcOffsetTable_.Add("Africa/Gaborone", 7200000);
			utcOffsetTable_.Add("Africa/Harare", 7200000);
			utcOffsetTable_.Add("Africa/Johannesburg", 7200000);
			utcOffsetTable_.Add("Africa/Kigali", 7200000);
			utcOffsetTable_.Add("Africa/Lubumbashi", 7200000);
			utcOffsetTable_.Add("Africa/Lusaka", 7200000);
			utcOffsetTable_.Add("Africa/Maputo", 7200000);
			utcOffsetTable_.Add("Africa/Maseru", 7200000);
			utcOffsetTable_.Add("Africa/Mbabane", 7200000);
			utcOffsetTable_.Add("Africa/Tripoli", 7200000);
			utcOffsetTable_.Add("Asia/Amman", 7200000);
			utcOffsetTable_.Add("Asia/Beirut", 7200000);
			utcOffsetTable_.Add("Asia/Damascus", 7200000);
			utcOffsetTable_.Add("Asia/Gaza", 7200000);
			utcOffsetTable_.Add("Asia/Hebron", 7200000);
			utcOffsetTable_.Add("Asia/Istanbul", 7200000);
			utcOffsetTable_.Add("Asia/Jerusalem", 7200000);
			utcOffsetTable_.Add("Asia/Nicosia", 7200000);
			utcOffsetTable_.Add("Asia/Tel_Aviv", 7200000);
			utcOffsetTable_.Add("CAT", 7200000);
			utcOffsetTable_.Add("EET", 7200000);
			utcOffsetTable_.Add("Egypt", 7200000);
			utcOffsetTable_.Add("Etc/GMT-2", 7200000);
			utcOffsetTable_.Add("Europe/Athens", 7200000);
			utcOffsetTable_.Add("Europe/Bucharest", 7200000);
			utcOffsetTable_.Add("Europe/Chisinau", 7200000);
			utcOffsetTable_.Add("Europe/Helsinki", 7200000);
			utcOffsetTable_.Add("Europe/Istanbul", 7200000);
			utcOffsetTable_.Add("Europe/Kaliningrad", 7200000);
			utcOffsetTable_.Add("Europe/Kiev", 7200000);
			utcOffsetTable_.Add("Europe/Mariehamn", 7200000);
			utcOffsetTable_.Add("Europe/Nicosia", 7200000);
			utcOffsetTable_.Add("Europe/Riga", 7200000);
			utcOffsetTable_.Add("Europe/Sofia", 7200000);
			utcOffsetTable_.Add("Europe/Tallinn", 7200000);
			utcOffsetTable_.Add("Europe/Tiraspol", 7200000);
			utcOffsetTable_.Add("Europe/Uzhgorod", 7200000);
			utcOffsetTable_.Add("Europe/Vilnius", 7200000);
			utcOffsetTable_.Add("Europe/Zaporozhye", 7200000);
			utcOffsetTable_.Add("Israel", 7200000);
			utcOffsetTable_.Add("Libya", 7200000);
			utcOffsetTable_.Add("Turkey", 7200000);
			utcOffsetTable_.Add("Africa/Addis_Ababa", 10800000);
			utcOffsetTable_.Add("Africa/Asmara", 10800000);
			utcOffsetTable_.Add("Africa/Asmera", 10800000);
			utcOffsetTable_.Add("Africa/Dar_es_Salaam", 10800000);
			utcOffsetTable_.Add("Africa/Djibouti", 10800000);
			utcOffsetTable_.Add("Africa/Juba", 10800000);
			utcOffsetTable_.Add("Africa/Kampala", 10800000);
			utcOffsetTable_.Add("Africa/Khartoum", 10800000);
			utcOffsetTable_.Add("Africa/Mogadishu", 10800000);
			utcOffsetTable_.Add("Africa/Nairobi", 10800000);
			utcOffsetTable_.Add("Antarctica/Syowa", 10800000);
			utcOffsetTable_.Add("Asia/Aden", 10800000);
			utcOffsetTable_.Add("Asia/Baghdad", 10800000);
			utcOffsetTable_.Add("Asia/Bahrain", 10800000);
			utcOffsetTable_.Add("Asia/Kuwait", 10800000);
			utcOffsetTable_.Add("Asia/Qatar", 10800000);
			utcOffsetTable_.Add("Asia/Riyadh", 10800000);
			utcOffsetTable_.Add("EAT", 10800000);
			utcOffsetTable_.Add("Etc/GMT-3", 10800000);
			utcOffsetTable_.Add("Europe/Minsk", 10800000);
			utcOffsetTable_.Add("Europe/Moscow", 10800000);
			utcOffsetTable_.Add("Europe/Simferopol", 10800000);
			utcOffsetTable_.Add("Europe/Volgograd", 10800000);
			utcOffsetTable_.Add("Indian/Antananarivo", 10800000);
			utcOffsetTable_.Add("Indian/Comoro", 10800000);
			utcOffsetTable_.Add("Indian/Mayotte", 10800000);
			utcOffsetTable_.Add("W-SU", 10800000);
			utcOffsetTable_.Add("Asia/Riyadh87", 11220000);
			utcOffsetTable_.Add("Asia/Riyadh88", 11220000);
			utcOffsetTable_.Add("Asia/Riyadh89", 11220000);
			utcOffsetTable_.Add("Mideast/Riyadh87", 11220000);
			utcOffsetTable_.Add("Mideast/Riyadh88", 11220000);
			utcOffsetTable_.Add("Mideast/Riyadh89", 11220000);
			utcOffsetTable_.Add("Asia/Tehran", 12600000);
			utcOffsetTable_.Add("Iran", 12600000);
			utcOffsetTable_.Add("Asia/Baku", 14400000);
			utcOffsetTable_.Add("Asia/Dubai", 14400000);
			utcOffsetTable_.Add("Asia/Muscat", 14400000);
			utcOffsetTable_.Add("Asia/Tbilisi", 14400000);
			utcOffsetTable_.Add("Asia/Yerevan", 14400000);
			utcOffsetTable_.Add("Etc/GMT-4", 14400000);
			utcOffsetTable_.Add("Europe/Samara", 14400000);
			utcOffsetTable_.Add("Indian/Mahe", 14400000);
			utcOffsetTable_.Add("Indian/Mauritius", 14400000);
			utcOffsetTable_.Add("Indian/Reunion", 14400000);
			utcOffsetTable_.Add("NET", 14400000);
			utcOffsetTable_.Add("Asia/Kabul", 16200000);
			utcOffsetTable_.Add("Antarctica/Mawson", 18000000);
			utcOffsetTable_.Add("Asia/Aqtau", 18000000);
			utcOffsetTable_.Add("Asia/Aqtobe", 18000000);
			utcOffsetTable_.Add("Asia/Ashgabat", 18000000);
			utcOffsetTable_.Add("Asia/Ashkhabad", 18000000);
			utcOffsetTable_.Add("Asia/Dushanbe", 18000000);
			utcOffsetTable_.Add("Asia/Karachi", 18000000);
			utcOffsetTable_.Add("Asia/Oral", 18000000);
			utcOffsetTable_.Add("Asia/Samarkand", 18000000);
			utcOffsetTable_.Add("Asia/Tashkent", 18000000);
			utcOffsetTable_.Add("Asia/Yekaterinburg", 18000000);
			utcOffsetTable_.Add("Etc/GMT-5", 18000000);
			utcOffsetTable_.Add("Indian/Kerguelen", 18000000);
			utcOffsetTable_.Add("Indian/Maldives", 18000000);
			utcOffsetTable_.Add("PLT", 18000000);
			utcOffsetTable_.Add("Asia/Calcutta", 19800000);
			utcOffsetTable_.Add("Asia/Colombo", 19800000);
			utcOffsetTable_.Add("Asia/Kolkata", 19800000);
			utcOffsetTable_.Add("IST", 19800000);
			utcOffsetTable_.Add("Asia/Kathmandu", 20700000);
			utcOffsetTable_.Add("Asia/Katmandu", 20700000);
			utcOffsetTable_.Add("Antarctica/Vostok", 21600000);
			utcOffsetTable_.Add("Asia/Almaty", 21600000);
			utcOffsetTable_.Add("Asia/Bishkek", 21600000);
			utcOffsetTable_.Add("Asia/Dacca", 21600000);
			utcOffsetTable_.Add("Asia/Dhaka", 21600000);
			utcOffsetTable_.Add("Asia/Kashgar", 21600000);
			utcOffsetTable_.Add("Asia/Novosibirsk", 21600000);
			utcOffsetTable_.Add("Asia/Omsk", 21600000);
			utcOffsetTable_.Add("Asia/Qyzylorda", 21600000);
			utcOffsetTable_.Add("Asia/Thimbu", 21600000);
			utcOffsetTable_.Add("Asia/Thimphu", 21600000);
			utcOffsetTable_.Add("Asia/Urumqi", 21600000);
			utcOffsetTable_.Add("BST", 21600000);
			utcOffsetTable_.Add("Etc/GMT-6", 21600000);
			utcOffsetTable_.Add("Indian/Chagos", 21600000);
			utcOffsetTable_.Add("Asia/Rangoon", 23400000);
			utcOffsetTable_.Add("Indian/Cocos", 23400000);
			utcOffsetTable_.Add("Antarctica/Davis", 25200000);
			utcOffsetTable_.Add("Asia/Bangkok", 25200000);
			utcOffsetTable_.Add("Asia/Ho_Chi_Minh", 25200000);
			utcOffsetTable_.Add("Asia/Hovd", 25200000);
			utcOffsetTable_.Add("Asia/Jakarta", 25200000);
			utcOffsetTable_.Add("Asia/Krasnoyarsk", 25200000);
			utcOffsetTable_.Add("Asia/Novokuznetsk", 25200000);
			utcOffsetTable_.Add("Asia/Phnom_Penh", 25200000);
			utcOffsetTable_.Add("Asia/Pontianak", 25200000);
			utcOffsetTable_.Add("Asia/Saigon", 25200000);
			utcOffsetTable_.Add("Asia/Vientiane", 25200000);
			utcOffsetTable_.Add("Etc/GMT-7", 25200000);
			utcOffsetTable_.Add("Indian/Christmas", 25200000);
			utcOffsetTable_.Add("VST", 25200000);
			utcOffsetTable_.Add("Antarctica/Casey", 28800000);
			utcOffsetTable_.Add("Asia/Brunei", 28800000);
			utcOffsetTable_.Add("Asia/Chita", 28800000);
			utcOffsetTable_.Add("Asia/Choibalsan", 28800000);
			utcOffsetTable_.Add("Asia/Chongqing", 28800000);
			utcOffsetTable_.Add("Asia/Chungking", 28800000);
			utcOffsetTable_.Add("Asia/Harbin", 28800000);
			utcOffsetTable_.Add("Asia/Hong_Kong", 28800000);
			utcOffsetTable_.Add("Asia/Irkutsk", 28800000);
			utcOffsetTable_.Add("Asia/Kuala_Lumpur", 28800000);
			utcOffsetTable_.Add("Asia/Kuching", 28800000);
			utcOffsetTable_.Add("Asia/Macao", 28800000);
			utcOffsetTable_.Add("Asia/Macau", 28800000);
			utcOffsetTable_.Add("Asia/Makassar", 28800000);
			utcOffsetTable_.Add("Asia/Manila", 28800000);
			utcOffsetTable_.Add("Asia/Shanghai", 28800000);
			utcOffsetTable_.Add("Asia/Singapore", 28800000);
			utcOffsetTable_.Add("Asia/Taipei", 28800000);
			utcOffsetTable_.Add("Asia/Ujung_Pandang", 28800000);
			utcOffsetTable_.Add("Asia/Ulaanbaatar", 28800000);
			utcOffsetTable_.Add("Asia/Ulan_Bator", 28800000);
			utcOffsetTable_.Add("Australia/Perth", 28800000);
			utcOffsetTable_.Add("Australia/West", 28800000);
			utcOffsetTable_.Add("CTT", 28800000);
			utcOffsetTable_.Add("Etc/GMT-8", 28800000);
			utcOffsetTable_.Add("Hongkong", 28800000);
			utcOffsetTable_.Add("PRC", 28800000);
			utcOffsetTable_.Add("Singapore", 28800000);
			utcOffsetTable_.Add("Australia/Eucla", 31500000);
			utcOffsetTable_.Add("Asia/Dili", 32400000);
			utcOffsetTable_.Add("Asia/Jayapura", 32400000);
			utcOffsetTable_.Add("Asia/Khandyga", 32400000);
			utcOffsetTable_.Add("Asia/Pyongyang", 32400000);
			utcOffsetTable_.Add("Asia/Seoul", 32400000);
			utcOffsetTable_.Add("Asia/Tokyo", 32400000);
			utcOffsetTable_.Add("Asia/Yakutsk", 32400000);
			utcOffsetTable_.Add("Etc/GMT-9", 32400000);
			utcOffsetTable_.Add("JST", 32400000);
			utcOffsetTable_.Add("Japan", 32400000);
			utcOffsetTable_.Add("Pacific/Palau", 32400000);
			utcOffsetTable_.Add("ROK", 32400000);
			utcOffsetTable_.Add("ACT", 34200000);
			utcOffsetTable_.Add("Australia/Adelaide", 34200000);
			utcOffsetTable_.Add("Australia/Broken_Hill", 34200000);
			utcOffsetTable_.Add("Australia/Darwin", 34200000);
			utcOffsetTable_.Add("Australia/North", 34200000);
			utcOffsetTable_.Add("Australia/South", 34200000);
			utcOffsetTable_.Add("Australia/Yancowinna", 34200000);
			utcOffsetTable_.Add("AET", 36000000);
			utcOffsetTable_.Add("Antarctica/DumontDUrville", 36000000);
			utcOffsetTable_.Add("Asia/Magadan", 36000000);
			utcOffsetTable_.Add("Asia/Sakhalin", 36000000);
			utcOffsetTable_.Add("Asia/Ust-Nera", 36000000);
			utcOffsetTable_.Add("Asia/Vladivostok", 36000000);
			utcOffsetTable_.Add("Australia/ACT", 36000000);
			utcOffsetTable_.Add("Australia/Brisbane", 36000000);
			utcOffsetTable_.Add("Australia/Canberra", 36000000);
			utcOffsetTable_.Add("Australia/Currie", 36000000);
			utcOffsetTable_.Add("Australia/Hobart", 36000000);
			utcOffsetTable_.Add("Australia/Lindeman", 36000000);
			utcOffsetTable_.Add("Australia/Melbourne", 36000000);
			utcOffsetTable_.Add("Australia/NSW", 36000000);
			utcOffsetTable_.Add("Australia/Queensland", 36000000);
			utcOffsetTable_.Add("Australia/Sydney", 36000000);
			utcOffsetTable_.Add("Australia/Tasmania", 36000000);
			utcOffsetTable_.Add("Australia/Victoria", 36000000);
			utcOffsetTable_.Add("Etc/GMT-10", 36000000);
			utcOffsetTable_.Add("Pacific/Chuuk", 36000000);
			utcOffsetTable_.Add("Pacific/Guam", 36000000);
			utcOffsetTable_.Add("Pacific/Port_Moresby", 36000000);
			utcOffsetTable_.Add("Pacific/Saipan", 36000000);
			utcOffsetTable_.Add("Pacific/Truk", 36000000);
			utcOffsetTable_.Add("Pacific/Yap", 36000000);
			utcOffsetTable_.Add("Australia/LHI", 37800000);
			utcOffsetTable_.Add("Australia/Lord_Howe", 37800000);
			utcOffsetTable_.Add("Antarctica/Macquarie", 39600000);
			utcOffsetTable_.Add("Asia/Srednekolymsk", 39600000);
			utcOffsetTable_.Add("Etc/GMT-11", 39600000);
			utcOffsetTable_.Add("Pacific/Bougainville", 39600000);
			utcOffsetTable_.Add("Pacific/Efate", 39600000);
			utcOffsetTable_.Add("Pacific/Guadalcanal", 39600000);
			utcOffsetTable_.Add("Pacific/Kosrae", 39600000);
			utcOffsetTable_.Add("Pacific/Noumea", 39600000);
			utcOffsetTable_.Add("Pacific/Pohnpei", 39600000);
			utcOffsetTable_.Add("Pacific/Ponape", 39600000);
			utcOffsetTable_.Add("SST", 39600000);
			utcOffsetTable_.Add("Pacific/Norfolk", 41400000);
			utcOffsetTable_.Add("Antarctica/McMurdo", 43200000);
			utcOffsetTable_.Add("Antarctica/South_Pole", 43200000);
			utcOffsetTable_.Add("Asia/Anadyr", 43200000);
			utcOffsetTable_.Add("Asia/Kamchatka", 43200000);
			utcOffsetTable_.Add("Etc/GMT-12", 43200000);
			utcOffsetTable_.Add("Kwajalein", 43200000);
			utcOffsetTable_.Add("NST", 43200000);
			utcOffsetTable_.Add("NZ", 43200000);
			utcOffsetTable_.Add("Pacific/Auckland", 43200000);
			utcOffsetTable_.Add("Pacific/Fiji", 43200000);
			utcOffsetTable_.Add("Pacific/Funafuti", 43200000);
			utcOffsetTable_.Add("Pacific/Kwajalein", 43200000);
			utcOffsetTable_.Add("Pacific/Majuro", 43200000);
			utcOffsetTable_.Add("Pacific/Nauru", 43200000);
			utcOffsetTable_.Add("Pacific/Tarawa", 43200000);
			utcOffsetTable_.Add("Pacific/Wake", 43200000);
			utcOffsetTable_.Add("Pacific/Wallis", 43200000);
			utcOffsetTable_.Add("NZ-CHAT", 45900000);
			utcOffsetTable_.Add("Pacific/Chatham", 45900000);
			utcOffsetTable_.Add("Etc/GMT-13", 46800000);
			utcOffsetTable_.Add("MIT", 46800000);
			utcOffsetTable_.Add("Pacific/Apia", 46800000);
			utcOffsetTable_.Add("Pacific/Enderbury", 46800000);
			utcOffsetTable_.Add("Pacific/Fakaofo", 46800000);
			utcOffsetTable_.Add("Pacific/Tongatapu", 46800000);
			utcOffsetTable_.Add("Etc/GMT-14", 50400000);
			utcOffsetTable_.Add("Pacific/Kiritimati", 50400000);
		}
	}
}
