using java.lang;
using kairo.unity.util;

namespace data
{
	public class EventData : BaseData
	{
		public const int TYPE_SYSTEM = 0;

		public const int TERM_NG = 0;

		public const int TERM_MONEY = 1;

		public const int TERM_EVENT = 2;

		public const int TERM_YEAR = 3;

		public const int TERM_MONTH = 4;

		public const int TERM_WEEK = 5;

		public const int TERM_HOUR = 6;

		public const int TERM_RANK = 7;

		public const int TERM_NUM_MATCH = 8;

		public const int TERM_NUM_WIN = 9;

		public const int TERM_NUM_DRAW = 10;

		public const int TERM_NUM_LOSE = 11;

		public const int TERM_NUM_FAN = 12;

		public const int SCR_MONEY = 0;

		public const int SCR_EVENT = 1;

		public const int SCR_TALK = 2;

		public const int SCR_NEWS = 3;

		public const int SCR_EVTMSG = 4;

		public const int SCR_DELAY = 5;

		public const int SCR_WAIT = 6;

		public const int SCR_FLAG_ON = 7;

		public const int SCR_FLAG_OFF = 8;

		public const int SCR_PLAYER = 9;

		public const int SCR_SVISOR = 10;

		public const int SCR_FORMATION = 11;

		public const int SCR_TENANT = 12;

		public const int SCR_FANACT = 13;

		public const int SCR_TRAINING = 14;

		public const int SCR_SPONSOR = 15;

		public const int SCR_CUP = 16;

		public const int SCR_TRADE = 17;

		public const int SCR_TRADEGROUP = 18;

		public const int SCR_LEAGUE = 19;

		public const int SCR_STADIUM_LVUP = 20;

		public const int SCR_TAREA_NUM_UP = 21;

		public const int SCR_TAREA_LV_UP = 22;

		public const int SCR_RP_FANSERVICE = 23;

		public const int SCR_RP_IDEA = 24;

		public const int SCR_RP_PHYSICAL = 25;

		public const int SCR_RP_TACTICS = 26;

		public const int SCR_PP_EXP = 27;

		public const int SCR_PP_KICK = 28;

		public const int SCR_PP_SPEED = 29;

		public const int SCR_PP_TEQNIC = 30;

		public const int SCR_PP_BODY = 31;

		public const int SCR_PP_KEEPER = 32;

		public const int SCR_PP_AURA = 33;

		public const int SCR_FAN_FAN = 34;

		public const int SCR_FAN_ASSIST = 35;

		public const int SCR_FAN_NAMEVALUE = 36;

		public const int SCR_HUMANR_PLAYER = 37;

		public const int SCR_HUMANR_SVISOR = 38;

		public const int SCR_FADEIN = 39;

		public const int SCR_FADEOUT = 40;

		public const int SCR_TEAMEVA = 41;

		public const int SCR_TRAINING_INIT = 42;

		public string name_;

		public int type_;

		public int[][] termsScript_;

		public int[][] execsScript_;

		public static string[] stringTable_;

		public EventData()
		{
		}

		public EventData(string src)
		{
			Load(src);
		}

		public void Load(string src)
		{
			int num = 0;
			string[] array = StringUtil.Split(src, "\t");
			id_ = JInteger.ParseInt(array[num++]);
			name_ = array[num++];
			type_ = JInteger.ParseInt(array[num++]);
			termsScript_ = ReadScript(array[num++]);
			execsScript_ = ReadScript(array[num++]);
		}

		public static int[][] ReadScript(string source)
		{
			if (source == null || source.Length() == 0)
			{
				return new int[0][];
			}
			string[] array = StringUtil.Split(source, "&");
			int[][] array2 = new int[array.Length][];
			for (int i = 0; i < array.Length; i++)
			{
				string[] array3 = StringUtil.Split(array[i], ",");
				array2[i] = new int[array3.Length];
				for (int j = 0; j < array3.Length; j++)
				{
					array2[i][j] = JInteger.ParseInt(array3[j]);
				}
			}
			return array2;
		}

		public static bool CheckScript(int[][] script, int scrId)
		{
			for (int i = 0; i < script.Length; i++)
			{
				if (script[i][0] == scrId)
				{
					return true;
				}
			}
			return false;
		}

		public static int Search(EventData[] events, int id)
		{
			for (int i = 0; i < events.Length; i++)
			{
				if (events[i].id_ == id)
				{
					return i;
				}
			}
			return -1;
		}

		public static void LoadStringTable(string src)
		{
			stringTable_ = StringUtil.Split(src, "\n");
		}
	}
}
