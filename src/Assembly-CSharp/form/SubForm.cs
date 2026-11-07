using System;
using data;
using game;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.m3d4.util3d;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using panel;
using surface;

namespace form
{
	public class SubForm : MyFormBase
	{
		public const int TYPE_TALK = 0;

		public const int TYPE_DIALOG = 1;

		public const int TYPE_DIALOG_CHARA = 2;

		public const int TYPE_MAIN_MENU = 3;

		public const int TYPE_MAIN_MENU_SOCCER = 4;

		public const int TYPE_GAME_MENU = 5;

		public const int TYPE_TEAM_MENU = 6;

		public const int TYPE_CLUB_MANAGE = 7;

		public const int TYPE_SETTING_MENU = 8;

		public const int TYPE_ACTION_MENU = 9;

		public const int TYPE_LIST_MENU = 10;

		public const int TYPE_INFO_MENU = 11;

		public const int TYPE_SYSTEM_MENU = 12;

		public const int TYPE_EVENT = 13;

		public const int TYPE_CONFIG = 14;

		public const int TYPE_MANUAL = 15;

		public const int TYPE_SAVE = 16;

		public const int TYPE_NEWS = 17;

		public const int TYPE_WAIT = 18;

		public const int TYPE_CLEARPOINT = 19;

		public const int TYPE_ENDING = 20;

		public const int TYPE_NO_MYMENU = 21;

		public const int TYPE_SAVE_MENU = 22;

		public const int TYPE_DIALOG_SYSTEM = 23;

		public const int TYPE_FADEIN = 24;

		public const int TYPE_FADEOUT = 25;

		public const int TYPE_TEAMEDIT = 26;

		public const int TYPE_MEMBERINFO = 27;

		public const int TYPE_MEMBERINFO_BENCH = 28;

		public const int TYPE_MEMBERINFO2 = 29;

		public const int TYPE_MEMBERINFO_MINI = 30;

		public const int TYPE_MEMBERINFO_DISMISSAL = 31;

		public const int TYPE_MEMBERINFO_EXPLAIN_P = 32;

		public const int TYPE_LEAGUEINFO = 33;

		public const int TYPE_LEAGUESELECT2 = 34;

		public const int TYPE_LEAGUESELECT3 = 35;

		public const int TYPE_MATCHNEXT = 36;

		public const int TYPE_MATCHEND = 37;

		public const int TYPE_MATCH_VICTORY = 38;

		public const int TYPE_FAN_FANACT = 39;

		public const int TYPE_FAN_FANACT2 = 40;

		public const int TYPE_FAN_FANACT_EF2 = 41;

		public const int TYPE_FAN_SPONSOR = 42;

		public const int TYPE_FAN_SPONSOR2 = 43;

		public const int TYPE_FAN_SPONSOR_EF2 = 44;

		public const int TYPE_FAN_TRAINING_TOP = 45;

		public const int TYPE_FAN_TRAINING_T = 46;

		public const int TYPE_FAN_TRAINING_P = 47;

		public const int TYPE_FAN_TRAINING_EF2 = 48;

		public const int TYPE_FAN_TRAINING_EF3 = 49;

		public const int TYPE_SUPERVISOR_DATA = 50;

		public const int TYPE_SUPERVISOR_CHANGE = 51;

		public const int TYPE_SUPERVISOR_YEAREND = 52;

		public const int TYPE_TRADELIST = 53;

		public const int TYPE_TRADELIST_CHECK = 54;

		public const int TYPE_TRADELIST_EF = 55;

		public const int TYPE_TRADELIST_SUCCESS = 56;

		public const int TYPE_EDITFORMATION_PRINT = 57;

		public const int TYPE_GAME_QUESTMATCH_LIST = 58;

		public const int TYPE_GAME_QUESTMATCH_LIST2 = 59;

		public const int TYPE_TENANT_STRENGTH = 60;

		public const int TYPE_TENANT_VAC = 61;

		public const int TYPE_TENANT_VAC2 = 62;

		public const int TYPE_TENANT_STRENGTH2 = 63;

		public const int TYPE_TENANT_LVUP = 64;

		public const int TYPE_TENANT_INFO = 65;

		public const int TYPE_TENANT_DESTRUCT = 66;

		public const int TYPE_TEAM_INFO = 67;

		public const int TYPE_TICKET_PRICE = 68;

		public const int TYPE_FADE_OUT = 69;

		public const int TYPE_FADE_IN = 70;

		public const int TYPE_NEWGAME = 71;

		public const int TYPE_HUMANR_PLAYER = 72;

		public const int TYPE_TRAINING_INIT = 73;

		public const int TYPE_INCOME = 74;

		public const int TYPE_FADE_OUT2 = 75;

		public const int TYPE_FADE_IN2 = 76;

		public const int TYPE_FANBONUS_HISTORY = 77;

		public const int TYPE_GAME_BUS = 78;

		public const int TYPE_GAME_TOMATCH = 79;

		public const int TYPE_GAME_CHANGESTATE = 80;

		private const int MENU_GAME = 0;

		private const int MENU_TRAINING = 1;

		private const int MENU_MANAGE = 2;

		private const int MENU_TEAM = 3;

		private const int MENU_INFO = 4;

		private const int MENU_SYSTEM = 5;

		private const int MENU_LEAGUE = 6;

		private const int MENU_EXHIBITION_MATCH = 7;

		private const int MENU_QUEST_MATCH = 8;

		private const int MENU_ABSTAIN = 9;

		private const int MENU_NEXTMATCH = 10;

		private const int MENU_MEMBER = 11;

		private const int MENU_TRADE = 12;

		private const int MENU_SVISOR = 13;

		private const int MENU_FAN_ACT = 14;

		private const int MENU_TENANT = 15;

		private const int MENU_SPONSOR = 16;

		private const int MENU_TEAMINFO = 17;

		private const int MENU_FANBONUS_HISTORY = 18;

		private const int MENU_INCOME = 19;

		private const int MENU_SAVE = 20;

		private const int MENU_CONFIG = 21;

		private const int MENU_TUTORIAL = 22;

		private const int MENU_RANKING = 23;

		private const int MENU_EXIT = 24;

		private const int MENU_SOCCEREND = 25;

		private const int MENU_CONTINUE = 26;

		private const int MENU_NEWGAME = 27;

		private const int MENU_DELETE = 28;

		private const int MENU_TITLE = 29;

		public const string BUTTONSTR_HAI_IIE = "はい\tいいえ";

		public const int EVENT_HAPPY = 0;

		public const int EVENT_UNHAPPY = 1;

		public const int EVENT_SPECIAL = 2;

		public const int EVENT_END = 3;

		private const int NEWS_LINES = 6;

		public const int TALK_CHARATYPE_CHARA = 0;

		public const int TALK_CHARATYPE_PLAYER = 1;

		public const int TALK_CHARATYPE_SPONSOR = 2;

		public const int TALK_CHARATYPE_SVISOR = 3;

		public const int TALK_CHARATYPE_SPCHARA = 4;

		public const int DEF_WSIZE_W1 = 228;

		public const int DEF_WSIZE_H1 = 204;

		public const int DEF_WSIZE_W2 = 228;

		public const int DEF_WSIZE_H2 = 176;

		public const int DEF_WSIZE_W3 = 228;

		public const int DEF_WSIZE_H3 = 176;

		public const int DEF_WSIZE_W4 = 231;

		public const int DEF_WSIZE_H4 = 185;

		public const int DEF_WSIZE_W5 = 188;

		public const int DEF_WSIZE_H5 = 110;

		public const int DEF_WSIZE_W6 = 223;

		public const int DEF_WSIZE_H6 = 199;

		public const int DEF_WSIZE_W7 = 210;

		public const int DEF_WSIZE_H7 = 110;

		private const int EVENTMSG_ANIME_TIME = 60;

		private const int FA_ANIME_JTIME = 12;

		public const int TARADE_ANIME_TIME = 50;

		private const int TRA_END = 25;

		public const int SP_NEGORESULT_ANIME = 10;

		private new const int WNDFLAG_LEFT_ARW = 1;

		private new const int WNDFLAG_RIGHT_ARW = 2;

		private new const int WNDFLAG_BOX = 4;

		private new const int WNDRET_TOP = 0;

		private new const int WNDRET_RIGHT = 1;

		private new const int WNDRET_BOTTOM = 2;

		private new const int WNDRET_LEFT = 3;

		private new const int WNDRET_BOX_TOP = 4;

		private new const int WNDRET_BOX_RIGHT = 5;

		private new const int WNDRET_BOX_BOTTOM = 6;

		private new const int WNDRET_BOX_LEFT = 7;

		private new const int WNDRET_BOX_END = 8;

		private const int TITLE_HEIGHT = 17;

		public const int CP2_MENU = 0;

		public const int CP2_KIT = 1;

		public const int CP2_HALL = 2;

		public const int CP2_SELE = 3;

		public const int CP2_STBODY = 5;

		public const string HISHOKO_NAME = "姫川チア子";

		public const string PUMPKIN_NAME = "ﾊﾟﾝﾌﾟｷﾝ商会";

		public const int MEMBERINFO_PAGE_POS = 0;

		public const int MEMBERINFO_PAGE_PHY = 999;

		public const int MEMBERINFO_PAGE_CATE = 1;

		public const int MEMBERINFO_PAGE_SCORE = 2;

		public const int MEMBERINFO_PAGE_END = 3;

		public const int LEAGUE_FROM_MAIN = 0;

		public const int LEAGUE_FROM_SOCCER = 1;

		private const int WVIC_PDATAID = 0;

		private const int WVIC_WAIT = 1;

		private const int WVIC_END = 2;

		public const int FW_CWAIT_MAX = 35;

		public const int FW_CWAIT_MIN = 5;

		public const int FW_W = 192;

		public const int FW_H = 63;

		public const int FW_LEFT_X = 24;

		public const int FW_RIGHT_X = 216;

		public const int FW_TOP_Y = 28;

		public const int FW_BOTTOM_Y = 91;

		private const int SID_TA = 0;

		private const int SID_YO = 1;

		private const int SID_NA = 2;

		private const int ANIME_LEFT_X = 73;

		private const int ANIME_RIGHT_X = 168;

		private const int ANIME_Y = 146;

		private const int ANIME_ONEWAY_TIME = 28;

		private const int ANIME_KICK_TIME = 6;

		public const int ADD_TSIZE = 30;

		public const int UPARROW_OY = 13;

		private const int MOVETOBUS_START = 25;

		private const int MOVETOBUS_END = 165;

		private const int WC_SEB = 0;

		private const int WC_OY = 1;

		private const int WC_F = 2;

		private const int WC_END = 3;

		private const int JAMP_TIME = 14;

		private const int JAMP_WAIT = 6;

		public const int MP_TIME = 0;

		public const int MP_VALUE = 1;

		public const int MP_VALUEMAX = 2;

		public const int MP_RANK = 3;

		public const int MP_RANKMAX = 4;

		public const int MP_RANKUP = 5;

		public const int MP_ADD = 6;

		public const int MP_END_VALUE = 7;

		public const int MP_END_RANK = 8;

		public const int MP_DISP_ADD = 9;

		public const int MP_END = 10;

		public const int CHARA_SEB_TATI = 0;

		public const int CHARA_SEB_SAD = 1;

		public const int CHARA_SEB_APPLAUSE = 2;

		public const int C_WAIT = 15;

		public const int CLEARPOINT_MODE_INIT = 0;

		public const int CLEARPOINT_MODE_NAME = 1;

		public const int CLEARPOINT_MODE_CPOINT = 2;

		public const int CLEARPOINT_MODE_SCORE = 3;

		public const int CLEARPOINT_MODE_HIGHSCORE = 4;

		public const int CLEARPOINT_MODE_NEXT = 5;

		public const int CLEARPOINT_MODE_RESULT = 6;

		public const int CLEARPOINT_MODE_END = 7;

		private string err_method = string.Empty;

		private int err_line;

		private UserData udat_;

		public int type_;

		private static readonly int[][] softLabels_ = new int[83][]
		{
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 3, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 5, 2 },
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 8, 2 },
			new int[2] { 8, 2 },
			new int[2] { 8, 2 },
			new int[2] { 8, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2] { 7, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2] { 10, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 6, 2 }
		};

		private static readonly string[] MENU_STR = new string[30]
		{
			"試合", "特別な練習", "クラブ運営", "チーム編成", "情報を見る", "ｼｽﾃﾑを設定", "リーグ戦", "ｴｷｼﾋﾞｼﾞｮﾝﾏｯﾁ", "カップ戦", "棄権する",
			"(test)次の対戦", "作戦を設定", "選手の獲得", "監督の情報", "地域の活動", "施設の強化", "スポンサー", "チーム情報", "ｸﾗﾌﾞの歴史", "収支情報",
			"セーブ", "設定変更", "遊び方", "ハイスコア", "ｹﾞｰﾑ終了", "ﾀｲﾑｱｯﾌﾟ", "続きから", "最初から", "削除する", "ﾀｲﾄﾙへ"
		};

		private int scrollValue_;

		public bool untilActiveHide_;

		public int select_;

		private int x_;

		private int y_;

		private int loopCounter_;

		public int frame_;

		public int frame2_;

		private string name_;

		private TextLayout textLay_;

		private int[][] menus_;

		private string[] selectButtons_;

		public int value1_;

		public int value2_;

		public int training_cnt;

		private string text_;

		public string[] textAry_;

		private int textIdx_;

		public int eventPage_;

		public int eventId_;

		private int pagesNum_;

		public int page_;

		public int page2_;

		public int eventRoot_;

		public int humanCharaType_;

		public int humanCharaIndex_;

		private new SubForm dialog_;

		private SubForm subForm_;

		public DelayEvent delayEvent_;

		private string talkArg_;

		private int nextIndex_;

		private int[] eventIds_;

		private string[] eventTexts_;

		private int jingle_;

		private int[] EVENT_HAPPY_ANIME_OY = new int[27]
		{
			13, 11, 10, 9, 7, 6, 5, 4, 3, 2,
			1, 0, 0, 0, 0, 0, 1, 1, 2, 3,
			3, 2, 2, 1, 1, 0, 0
		};

		private int[] EVENT_SAD_ANIME_OY = new int[25]
		{
			0, 0, 1, 1, 2, 2, 3, 4, 4, 5,
			5, 6, 6, 7, 7, 6, 5, 4, 3, 2,
			1, 1, 1, 2, 3
		};

		private int saveState_;

		public int dialogId_ = -1;

		public new int result_ = -1;

		public int arg1_;

		public int arg2_;

		public int wait_;

		private int inputFig_;

		private static readonly int SC_MAINMENU_SELECT = Graphics.GetColorOfRGB(30, 30, 30);

		private static readonly int SC_MAINMENU_NOTSELECT = Graphics.GetColorOfRGB(204, 220, 228);

		public static readonly int SC_WINDOW_TITLE = Graphics.GetColorOfRGB(247, 253, 247);

		public static readonly int SC_WINDOW_TITLE2 = Graphics.GetColorOfRGB(44, 54, 105);

		public static readonly int SC_WINDOW_BLACK = Graphics.GetColorOfRGB(92, 51, 31);

		public static readonly int SC_WINDOW_WHITE = Graphics.GetColorOfRGB(255, 227, 217);

		public static readonly int SC_WINDOW_RED = Graphics.GetColorOfRGB(255, 100, 25);

		public static readonly int SC_WINDOW_BLUE = Graphics.GetColorOfRGB(0, 100, 255);

		public static readonly int SC_WINDOW_SELECT = Graphics.GetColorOfRGB(255, 187, 76);

		public static readonly int SC_WINDOW_GRAY = Graphics.GetColorOfRGB(156, 155, 155);

		public static readonly string[] TROPHY_NAME = new string[5]
		{
			string.Empty,
			"銅の",
			"銀の",
			"金の",
			"金のｶｲﾛ"
		};

		private static readonly int[] FA_ANIME_TIME2 = new int[4] { 12, 40, 66, 76 };

		private static readonly int[] FA_ANIME_TIME3 = new int[3] { 12, 22, 48 };

		public static readonly int[] S_ANIME_TIME2 = new int[4] { 12, 16, 24, 40 };

		public static readonly int[] S_SNIME_ST = new int[5]
		{
			10,
			10 + S_ANIME_TIME2[3],
			10 + S_ANIME_TIME2[3] * 2,
			10 + S_ANIME_TIME2[3] * 3,
			20 + S_ANIME_TIME2[3] * 3
		};

		public static readonly int[] S_ANIME_TIME = new int[7]
		{
			10,
			10 + S_SNIME_ST[4],
			15 + S_SNIME_ST[4],
			35 + S_SNIME_ST[4],
			40 + S_SNIME_ST[4],
			40 + S_SNIME_ST[4] + FA_ANIME_TIME2[3],
			40 + S_SNIME_ST[4] + FA_ANIME_TIME2[3] + FA_ANIME_TIME3[2]
		};

		private int[] ANIME_MATCH_RESULT = new int[2] { 5, 17 };

		public static readonly int[] FA2_ANIME_TIME = new int[7] { 0, 0, 10, 60, 70, 130, 170 };

		private static readonly int[] TRA_AT = new int[3] { 7, 13, 25 };

		private static readonly int[] TRA_AT2 = new int[2] { 5, 25 };

		public static readonly int[] TRA_ANIME_TIME2 = new int[6] { 5, 130, 145, 150, 170, 175 };

		public static readonly int[] SP_ANIME_TIME = new int[5] { 5, 109, 119, 131, 141 };

		public static readonly int[] TLUP_ANIME_TIME = new int[6] { 5, 29, 44, 74, 84, 96 };

		public static readonly int[] TLUP_ANIME_TIME2 = new int[4] { 5, 55, 60, 72 };

		public static readonly int[] VICTORY_ANIME_TIME = new int[4] { 60, 75, 95, 115 };

		internal static readonly int CONFIG_QUALITY = 0;

		internal static readonly int CONFIG_SPEED = 1;

		internal static readonly int CONFIG_BGM = 2;

		internal static readonly int CONFIG_SE = 3;

		internal static readonly int CONFIG_ORIENTATION = 4;

		internal static readonly string[] CONFIG_NAMES = new string[5] { "画面", "ｽﾋﾟｰﾄﾞ", "BGM", "効果音", "画面回転" };

		internal static readonly string[][] CONFIG_VALUE_NAMES = new string[5][]
		{
			new string[2] { "描画優先", "速度優先" },
			new string[2] { "高速", "普通" },
			new string[0],
			new string[0],
			new string[2] { "自動", "固定" }
		};

		internal static readonly int[] CONFIG_VALUES = new int[5] { 393216, 458752, 524288, 589824, 720896 };

		internal static readonly bool[] CONFIG_VALUE_REVERSE = new bool[6] { true, true, false, false, false, false };

		internal static readonly int[] CONFIG_VALUE_MAX = new int[5] { 2, 2, 6, 6, 2 };

		internal static readonly int CONFIG_EN_QUALITY = 0;

		internal static readonly int CONFIG_EN_SPEED = 1;

		internal static readonly int CONFIG_EN_BGM = 2;

		internal static readonly int CONFIG_EN_SE = 3;

		internal static readonly int CONFIG_EN_CURRENCY = 4;

		internal static readonly int CONFIG_EN_ORIENTATION = 5;

		internal static readonly string[] CONFIG_EN_NAMES = new string[6] { "画面", "ｽﾋﾟｰﾄﾞ", "BGM", "効果音", "通貨単位", "画面回転" };

		internal static readonly string[][] CONFIG_EN_VALUE_NAMES = new string[6][]
		{
			new string[2] { "描画優先", "速度優先" },
			new string[2] { "高速", "普通" },
			new string[0],
			new string[0],
			new string[2] { "円", "ドル" },
			new string[2] { "自動", "固定" }
		};

		internal static readonly int[] CONFIG_EN_VALUES = new int[6] { 393216, 458752, 524288, 589824, 655360, 720896 };

		internal static readonly bool[] CONFIG_EN_VALUE_REVERSE = new bool[6] { true, true, false, false, false, false };

		internal static readonly int[] CONFIG_EN_VALUE_MAX = new int[6] { 2, 2, 6, 6, 2, 2 };

		public int[] temp_paramArray;

		private Vector ary1;

		private Vector ary2;

		public FanActData fanActData_;

		private SponsorData sponsorData_;

		public PlayerData playerData_;

		private TradeData tradeData_;

		public int tenantId_;

		public int tenantDataId_;

		private QuestMatchData qmData_;

		private LeagueData leagueData_;

		private int sponsor_income_;

		private int sponsor_num_;

		public static string ticketPriceMessage;

		internal int[][] MENUGRAPH_OFF = new int[4][]
		{
			new int[2] { 0, -1 },
			new int[2] { 1, 0 },
			new int[2] { 0, 1 },
			new int[2] { -1, 0 }
		};

		public static int FADE_TIME_V = 12;

		public int[][] zsort_mhobj;

		public Vector3D[] w_vic_pos;

		public int[][] w_vic_data;

		public int[] fw_goRate;

		public int[] fw_cnt;

		public int[][] fw_pos;

		public int[][] fw_zsort;

		public int fw_commonWait;

		public static readonly int[] FW_ANIME = new int[4] { 3, 6, 9, 12 };

		private static readonly int[] SCENE_CHANGE_TIME = new int[3] { 60, 60, 90 };

		private static readonly int[] SCENE_IMG_DIV_X = new int[3] { 8, 20, 8 };

		private static readonly int[] SCENE_IMG_DIV_Y = new int[3] { 8, 6, 8 };

		private static readonly int[] SCENE_CHANGE_ONE = new int[3] { 12, 35, 8 };

		private static readonly int[] SCENE_CHANGE_WAIT = new int[3] { 4, 1, 2 };

		public static readonly string[] SCENE_CHANGE_NAME = new string[3] { "縦", "横", "斜め" };

		private static readonly string[] UNIFORM_NAME = new string[9] { "ｸﾞﾘｰﾝ", "ﾚｯﾄﾞ", "ﾋﾟﾝｸ", "ｲｴﾛｰ", "ﾊﾟｰﾌﾟﾙ", "ﾗｲﾄﾌﾞﾙｰ", "ﾌﾞﾙｰ", "ﾌﾞﾗｯｸ", "ﾎﾜｲﾄ" };

		private static readonly string[] PARAM_EXPLAIN1 = new string[5] { "…ｼｭｰﾄの成功率・パスの強さ", "…走るときの早さ", "…多くの場面でのﾌﾟﾚｰの精度", "…転倒しにくさ", "…ｺﾞｰﾙｷｰﾊﾟｰの能力" };

		private static readonly string[] PARAM_EXPLAIN2 = new string[5] { "選手の能力に応じてポジションの適正", "が<co=FF1800>FEDCBAのランク</co>で評価されます。", "<co=0A44FF>Aランク</co>に近いポジションほど実力が", "発揮されやすいです。", "<co=0FA5FB>ポジション配置の参考にしましょう。</co>" };

		private static int anime_mode = 0;

		private static int anime_ballY = 0;

		private static int anime_cnt = 0;

		public static readonly int[][] DLGSEL_TRECT = new int[2][]
		{
			new int[4] { -20, -20, 40, 40 },
			new int[4] { -20, -20, 40, 40 }
		};

		public int udArrowCnt;

		private static int[] SP_MONEY = new int[4] { 1500, 1000, 1000, 1500 };

		private Vector3D update_tmp1 = new Vector3D();

		private static int[][] WIDTH_DATA = new int[2][]
		{
			new int[3] { 105, 85, 42 },
			new int[3] { 125, 91, 42 }
		};

		private static int[] WIDTH = new int[3];

		private int[] drawWindow_tmp;

		private static int[] WIN_CHA = new int[3];

		private static int[][] meterParam = JTool.MakeArray<int[][]>(new int[2] { 10, 10 });

		public int clearPoint_mode;

		public int clearPoint_cnt;

		public int clearPoint_index;

		public static readonly int[] CLEARPOINT_ANIMETIME = new int[8] { 75, 75, 45, 45, 65, 45, 65, 9999 };

		public long clearPoint_score;

		public long clearPoint_highScore;

		private static int fontSize = 12;

		public SubForm()
			: base(4)
		{
			untilActiveHide_ = true;
		}

		public SubForm(int type)
			: base(4)
		{
			type_ = type;
			select_ = 0;
			untilActiveHide_ = true;
			udArrowCnt = 200;
		}

		public SubForm(int type, int value1)
			: base(4)
		{
			type_ = type;
			select_ = 0;
			untilActiveHide_ = true;
			value1_ = value1;
		}

		public SubForm(int type, int x, int y)
			: base(4)
		{
			type_ = type;
			x_ = x;
			y_ = y;
			select_ = 0;
			untilActiveHide_ = true;
		}

		public override string ToString()
		{
			return "SubForm:" + type_ + "," + err_method + "," + err_line;
		}

		public void InitDialog(string text, string buttons)
		{
			x_ = 0;
			y_ = 0;
			select_ = 0;
			text_ = text;
			if (buttons.Length() == 0)
			{
				buttons = "了解";
			}
			selectButtons_ = StringUtil.Split(buttons, "\t");
		}

		public static SubForm MakeDialog(string text, string buttons)
		{
			SubForm subForm = new SubForm();
			subForm.type_ = 1;
			subForm.InitDialog(text, buttons);
			return subForm;
		}

		public static SubForm MakeDialog(string text)
		{
			{ global::form.SubForm recoveredReturn = MakeDialog(text, string.Empty); return recoveredReturn; }
		}

		public static SubForm MakeDialog(string text, string buttons, int defaultSelect)
		{
			SubForm subForm = MakeDialog(text, buttons);
			subForm.select_ = defaultSelect;
			return subForm;
		}

		public static SubForm MakeNoMymenu(string text)
		{
			SubForm subForm = MakeDialog(text, "はい\tいいえ");
			subForm.type_ = 21;
			return subForm;
		}

		public static SubForm MakeWait(int wait)
		{
			SubForm subForm = new SubForm(18);
			subForm.wait_ = wait;
			return subForm;
		}

		public static SubForm MakeSystemDialog(string text)
		{
			SubForm subForm = new SubForm(23);
			subForm.type_ = 23;
			subForm.x_ = 0;
			subForm.y_ = 0;
			subForm.select_ = 0;
			subForm.textLay_ = new TextLayout();
			subForm.textLay_.SetAnchor(18);
			subForm.textLay_.SetSpeed(3000);
			subForm.textLay_.SetColor(SC_WINDOW_WHITE);
			subForm.textLay_.SetLineSpace(4);
			subForm.textLay_.SetText(text);
			subForm.textLay_.Flush();
			return subForm;
		}

		public static SubForm MakeTalkForm(int msgId, string arg)
		{
			UserData instance = UserData.GetInstance();
			string value = AppData.talks_[msgId];
			if (arg != null)
			{
				value = StringUtil.Replace(value, arg);
			}
			string[] array = StringUtil.Split(value, "\t");
			int num = JInteger.ParseInt(array[0]);
			int num2 = JInteger.ParseInt(array[1]);
			if (num2 == -1 && num == 3)
			{
				num2 = instance.selectSupervisorId_;
			}
			SubForm subForm = new SubForm();
			subForm.talkArg_ = arg;
			subForm.type_ = 0;
			subForm.x_ = 0;
			subForm.y_ = 0;
			subForm.select_ = 0;
			subForm.humanCharaType_ = num;
			subForm.humanCharaIndex_ = num2;
			subForm.nextIndex_ = JInteger.ParseInt(array[2]);
			subForm.textAry_ = new string[array.Length - 3];
			JSystem.Arraycopy(array, 3, subForm.textAry_, 0, array.Length - 3);
			subForm.textIdx_ = 0;
			subForm.textLay_ = new TextLayout(false);
			subForm.textLay_.SetSize(172f, 60f);
			subForm.textLay_.SetSpeed(3000);
			subForm.textLay_.SetColor(Graphics.GetColorOfRGB(37, 35, 54));
			subForm.textLay_.SetLineSpace(4);
			subForm.textLay_.SetText(subForm.textAry_[subForm.textIdx_]);
			return subForm;
		}

		public static SubForm MakeTalkForm(int characterIndex, string text, string arg)
		{
			AppData instance = AppData.GetInstance();
			SubForm subForm = new SubForm();
			subForm.talkArg_ = arg;
			subForm.type_ = 0;
			subForm.x_ = 0;
			subForm.y_ = 0;
			subForm.select_ = 0;
			subForm.humanCharaIndex_ = characterIndex;
			subForm.nextIndex_ = -1;
			subForm.textAry_ = StringUtil.Split(text, "\t");
			subForm.textIdx_ = 0;
			subForm.textLay_ = new TextLayout();
			subForm.textLay_.SetSize(172f, 60f);
			subForm.textLay_.SetSpeed(3000);
			subForm.textLay_.SetColor(Graphics.GetColorOfRGB(7, 7, 7));
			subForm.textLay_.SetLineSpace(4);
			subForm.textLay_.SetText(text);
			return subForm;
		}

		public static SubForm MakeEvent(string var_event, string arg, int eventMsgId)
		{
			if ((((eventMsgId == 49) || (eventMsgId == 50)) || (eventMsgId == 51)) || (eventMsgId == 52)) { var_event = StringUtil.Replace(var_event, MyFormBase.YEN("万円", SP_MONEY[eventMsgId - 49])); } else { var_event = StringUtil.Replace(var_event, arg); }
			string[] array = StringUtil.Split(var_event, "\t");
			SubForm subForm = new SubForm();
			subForm.eventIds_ = new int[array.Length];
			subForm.eventTexts_ = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = StringUtil.Split(array[i], "&", false, false, false);
				subForm.eventIds_[i] = JInteger.ParseInt(array2[0]);
				subForm.eventTexts_[i] = array2[1];
			}
			subForm.type_ = 13;
			subForm.x_ = 0;
			subForm.y_ = 0;
			subForm.select_ = 0;
			return subForm;
		}

		public static SubForm MakeNews(int id, string arg)
		{
			AppData instance = AppData.GetInstance();
			if (arg == null)
			{
				arg = instance.save_.strs[0];
			}
			string text = StringUtil.Replace(instance.newsTexts_[id], arg);
			SubForm subForm = new SubForm();
			subForm.type_ = 17;
			subForm.x_ = 0;
			subForm.y_ = 0;
			subForm.select_ = 0;
			subForm.name_ = instance.newsTitles_[id];
			subForm.text_ = text;
			subForm.textLay_ = new TextLayout(false);
			subForm.textLay_.SetSize(159f, 106f);
			subForm.textLay_.SetColor(Graphics.GetColorOfRGB(37, 35, 54));
			subForm.textLay_.SetLineSpace(6);
			subForm.textLay_.SetText(text);
			subForm.textLay_.Flush();
			subForm.page_ = 0;
			subForm.pagesNum_ = ((subForm.textLay_.GetHeight() + 6) / 18 + 5) / 6;
			return subForm;
		}

		public override bool Init()
		{
			base.Init();
			err_method = "init()";
			udat_ = UserData.GetInstance();
			jingle_ = -1;
			frame_ = 0;
			int num = softLabels_[type_][0];
			int num2 = softLabels_[type_][1];
			SetSoftLabel(AppData.softLabels_[num], AppData.softLabels_[num2]);
			if (type_ == 3)
			{
				menus_ = null;
				AddMenu(0, 1);
				AddMenu(1, 1);
				AddMenu(2, 1);
				AddMenu(3, 1);
				AddMenu(4, 1);
				AddMenu(5, 1);
			}
			else if (type_ == 4)
			{
				menus_ = null;
				AddMenu(21, 1);
				AddMenu(22, 1);
				if (Config.DEBUG_CMD)
				{
					AddMenu(25, 1);
				}
			}
			else if (type_ == 5)
			{
				int matchInfoJoin = udat_.GetMatchInfoJoin();
				if (udat_.ChEventFlg(1))
				{
					AddMenu(6, 1);
				}
				if (udat_.ChEventFlg(8))
				{
					AddMenu(8, 1);
				}
				if (matchInfoJoin != 2)
				{
					AddMenu(9, 1);
				}
			}
			else if (type_ == 6) { } else {
				if (type_ == 7)
				{
					if (udat_.ChEventFlg(16))
					{
						AddMenu(12, 1);
					}
					if (udat_.ChEventFlg(2))
					{
						AddMenu(14, 1);
					}
					if (udat_.ChEventFlg(4))
					{
						AddMenu(15, 1);
					}
					if (udat_.ChEventFlg(32))
					{
						AddMenu(16, 1);
					}
				}
				else if (type_ == 11)
				{
					AddMenu(17, 1);
					AddMenu(13, 1);
					AddMenu(18, 1);
					AddMenu(19, 1);
				}
				else if (type_ == 12)
				{
					AddMenu(20, 1);
					AddMenu(21, 1);
					AddMenu(22, 1);
					AddMenu(23, 1);
					AddMenu(24, 1);
				}
				else if (type_ == 15)
				{
					textLay_ = new TextLayout(false);
					if (MyFormBase.Jp())
					{
						textLay_.SetSize(146f, 124f);
						textLay_.SetLineSpace(6);
					}
					else
					{
						textLay_.SetSize(186f, 124f);
						textLay_.SetLineSpace(8);
						textLay_.SetFontSize(10);
					}
					textLay_.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
					textLay_.SetText(apdat_.manualTexts_[page_]);
					textLay_.Flush();
				}
				else if (type_ == 16)
				{
					InitDialog("ゲームセーブ中", string.Empty);
				}
				else if (type_ == 17) { } else {
					if (type_ == 19)
					{
						apdat_.PlayBgm(4);
						clearPoint_highScore = apdat_.GetHighScore();
						udat_.SetClearPoint();
					}
					else if (type_ == 13) { } else {
						if (type_ == 14)
						{
							value1_ = apdat_.save_.ints[12];
						}
						else if (type_ == 22)
						{
							AddMenu(26, 1);
							AddMenu(27, 1);
							AddMenu(28, 1);
						}
						else if (type_ == 23) { } else if ((type_ == 24) || (type_ == 25)) { } else {
							if (type_ == 26)
							{
								if (udat_.editTest_formation.Size() > 0)
								{
									udat_.editTest_formation.RemoveAllElements();
								}
								for (int i = 0; i < apdat_.formationData_.Length; i++)
								{
									FormationData formationData = apdat_.formationData_[i];
									if (formationData.state_ == 1)
									{
										udat_.editTest_formation.AddElement(apdat_.formationData_[i]);
									}
								}
								FormationData elem = apdat_.formationData_[udat_.selectFormationId_];
								int num3 = udat_.editTest_formation.IndexOf(elem);
								if (num3 != -1)
								{
									udat_.et_cur[1] = num3;
								}
								udat_.SetEditForPos(false);
							}
							else if (type_ == 27 || type_ == 28 || type_ == 29 || type_ == 31)
							{
								ary1 = new Vector();
								for (int j = 0; j < udat_.memberPDataist_.Size(); j++)
								{
									PlayerData obj = (PlayerData)udat_.memberPDataist_.ElementAt(j);
									ary1.AddElement(obj);
								}
								if (type_ == 28)
								{
									if (playerData_ == null)
									{
										return Pop();
									}
									int num4 = playerData_.id_;
									for (int k = 0; k < 11; k++)
									{
										PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(k);
										if (playerData.id_ == num4) { continue; }
ary1.RemoveElement(playerData);
									}
									if (ary1.Size() == 0)
									{
										apdat_.ExecEvent(11, null, null);
										return Pop();
									}
								}
							}
							else if (type_ == 30) { } else if (type_ == 32) { } else {
								if (type_ == 33)
								{
									int testedMatchInfoJoin1 = udat_.GetMatchInfoJoin();
									if (testedMatchInfoJoin1 == 1)
									{
										{
										QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
										apdat_.ExecEvent(19, questMatchData.name_, null);
										return Pop();
									}
									}
									else if (testedMatchInfoJoin1 == 2)
									{
										formMan_.Push(new SubForm(34));
										return Pop();
									}
									LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
									if (udat_.league_setu_ == leagueData.matchEnemy_.Length - 1)
									{
										formMan_.Push(new SubForm(34));
										return Pop();
									}
									ary1 = new Vector();
									ary2 = new Vector();
									TeamData.SetScoreRanking(leagueData, ary1, ary2);
								}
								else if (type_ == 34)
								{
									ary1 = new Vector();
									for (int num5 = apdat_.leagueData_.Length - 1; num5 >= 0; num5--)
									{
										LeagueData leagueData2 = apdat_.leagueData_[num5];
										if (leagueData2.state_ == 1)
										{
											ary1.AddElement(leagueData2);
										}
									}
									if (ary1.Size() == 0)
									{
										Push(MakeDialog("リーグが存在しません"));
										return Pop();
									}
									for (int l = 0; l < ary1.Size(); l++)
									{
										LeagueData leagueData3 = (LeagueData)ary1.ElementAt(l);
										if (leagueData3.id_ == 1)
										{
											if (!apdat_.CheckAlreadyEvent(167))
											{
												apdat_.ExecEvent(167, null, null);
											}
										}
										else if (leagueData3.id_ == 2)
										{
											if (!apdat_.CheckAlreadyEvent(168))
											{
												apdat_.ExecEvent(168, null, null);
											}
										}
										else if (leagueData3.id_ == 3)
										{
											if (!apdat_.CheckAlreadyEvent(169))
											{
												apdat_.ExecEvent(169, null, null);
											}
										}
										else if (leagueData3.id_ == 4 && !apdat_.CheckAlreadyEvent(170))
										{
											apdat_.ExecEvent(170, null, null);
										}
									}
									LeagueData leagueData4 = (LeagueData)ary1.ElementAt(0);
									textLay_ = new TextLayout(false);
									leagueData4.SetLeagueInfoExplain(textLay_);
								}
								else if (type_ == 35) { } else {
									if (type_ == 36)
									{
										int testedMatchInfoJoin2 = udat_.GetMatchInfoJoin();
										if (testedMatchInfoJoin2 == 0)
										{
											{
											LeagueData leagueData5 = apdat_.leagueData_[udat_.selectLeagueId_];
											if (udat_.league_setu_ == leagueData5.matchEnemy_.Length - 1) { } else {
												apdat_.frmGame_.state_ = 2;
												apdat_.frmGame_.SetBattleData();
											}
											return Pop();
										}
										}
										else if (testedMatchInfoJoin2 == 1)
										{
											apdat_.frmGame_.state_ = 2;
											apdat_.frmGame_.SetBattleData();
											return Pop();
										}
										else
										{
											formMan_.Push(MakeDialog("種目を決定してください"));
											return Pop();
										}
									}
									if (type_ == 37) { } else {
										if (type_ == 38)
										{
											int num6 = 40;
											int num7 = 200;
											int num8 = 170;
											int num9 = 210;
											int num10 = (num7 - num6) / 3;
											int num11 = (num9 - num8) / 3;
											int num12 = udat_.memberPDataist_.Size();
											zsort_mhobj = JTool.MakeArray<int[][]>(new int[2] { num12, 2 });
											w_vic_pos = new Vector3D[num12];
											w_vic_data = JTool.MakeArray<int[][]>(new int[2] { num12, 2 });
											for (int m = 0; m < w_vic_pos.Length; m++)
											{
												int num13 = m % 9 % 3 * num10 + num6;
												int num14 = num9 - m % 9 / 3 * num11;
												w_vic_pos[m] = new Vector3D();
												w_vic_pos[m].x_ = GameUtil.RateConvert(GameUtil.Random(100), 0, 99, num13, num13 + num10, true);
												w_vic_pos[m].z_ = GameUtil.RateConvert(GameUtil.Random(100), 0, 99, num14, num14 - num11, true);
												PlayerData playerData2 = (PlayerData)udat_.memberPDataist_.ElementAt(m);
												w_vic_data[m][0] = playerData2.id_;
												w_vic_data[m][1] = GameUtil.Random(20) + 5;
											}
											for (int n = 0; n < w_vic_pos.Length; n++)
											{
												zsort_mhobj[n][0] = n;
												zsort_mhobj[n][1] = (int)w_vic_pos[n].z_;
											}
											for (int num15 = 0; num15 < zsort_mhobj.Length - 1; num15++)
											{
												for (int num16 = zsort_mhobj.Length - 1; num16 > num15; num16--)
												{
													if (zsort_mhobj[num15][1] > zsort_mhobj[num16][1])
													{
														int num17 = zsort_mhobj[num16][1];
														zsort_mhobj[num16][1] = zsort_mhobj[num15][1];
														zsort_mhobj[num15][1] = num17;
														num17 = zsort_mhobj[num16][0];
														zsort_mhobj[num16][0] = zsort_mhobj[num15][0];
														zsort_mhobj[num15][0] = num17;
													}
												}
											}
											fw_goRate = new int[5] { 0, 200, 200, 200, 0 };
											fw_cnt = new int[5];
											fw_pos = JTool.MakeArray<int[][]>(new int[2] { 5, 2 });
											fw_zsort = JTool.MakeArray<int[][]>(new int[2] { 5, 2 });
											fw_commonWait = 5;
										}
										else if (type_ == 39)
										{
											ary1 = new Vector();
											for (int num18 = 0; num18 < apdat_.fanActData_.Length; num18++)
											{
												FanActData fanActData = apdat_.fanActData_[num18];
												if (fanActData.state_ != 0)
												{
													ary1.AddElement(fanActData);
												}
											}
											if (ary1.Size() == 0)
											{
												Push(MakeDialog("項目がありません。"));
												return Pop();
											}
										}
										else if (type_ == 40)
										{
											FanActData fanActData2 = fanActData_;
											textLay_ = new TextLayout(false);
											textLay_.SetSize(124f, 47f);
											textLay_.SetColor(SC_WINDOW_BLACK);
											textLay_.SetLineSpace(5);
											textLay_.SetText(fanActData2.explain_);
											textLay_.Flush();
										}
										else if (type_ == 41)
										{
											FanActData fanActData3 = fanActData_;
											int num19 = GameUtil.Random(FanActData.FARESULT_RATE[FanActData.FARESULT_RATE.Length - 1]);
											for (int num20 = 0; num20 < FanActData.FARESULT_RATE.Length; num20++)
											{
												if (num19 < FanActData.FARESULT_RATE[num20])
												{
													FanActData.fa_result = num20;
													break;
												}
											}
											FanActData.fa_id[0] = 3;
											FanActData.fa_id[1] = 0;
											FanActData.fa_id[2] = 1;
											FanActData.fa_id[3] = 2;
											int limit = GameUtil.RateConvert(fanActData3.rank_, 0, 6, fanActData3.effLimit_[0], fanActData3.effLimit_[1], true);
											for (int num21 = 0; num21 < FanActData.fa_id.Length; num21++)
											{
												int num22 = FanActData.fa_id[num21];
												if (num22 == 3)
												{
													int add = fanActData3.difficult_;
													if (fanActData3.rank_ == 6)
													{
														add = 0;
													}
													SetMeterParamRank(num21, FA2_ANIME_TIME[6] - FA2_ANIME_TIME[5], fanActData3.rankPoint_, 100, fanActData3.rank_, 6, add);
													SetMeterParamDispAdd(num21, num22, fanActData3.rankPoint_);
													continue;
												}
												int fanEva = FanActData.GetFanEva(FanActData.fa_id[num21]);
												int num23 = GameUtil.RateConvert(fanActData3.rank_, 0, 6, fanActData3.eff_[num22][0], fanActData3.eff_[num22][1], true);
												if (FanActData.fa_id[num21] != 1)
												{
													num23 = FanActData.GetAddEvaLimitOver(FanActData.fa_id[num21], fanEva, num23, limit);
												}
												SetMeterParam(num21, FA2_ANIME_TIME[6] - FA2_ANIME_TIME[5], fanEva, 10000, num23);
												SetMeterParamDispAdd(num21, num22, FanActData.GetFanDispParam(fanEva, num22));
											}
											ary1 = new Vector();
											for (int num24 = 0; num24 < 11; num24++)
											{
												ary1.AddElement(udat_.memberPDataist_.ElementAt(num24));
											}
											for (int num25 = 0; num25 < ary1.Size(); num25++)
											{
												PlayerData obj2 = (PlayerData)ary1.ElementAt(num25);
												int num26 = GameUtil.Random(ary1.Size());
												if (num26 == num25) { } else {
													ary1.SetElementAt(ary1.ElementAt(num26), num25);
													ary1.SetElementAt(obj2, num26);
												}
											}
										}
										else if (type_ == 42)
										{
											ary1 = new Vector();
											for (int num27 = 0; num27 < apdat_.sponsorData_.Length; num27++)
											{
												SponsorData sponsorData = apdat_.sponsorData_[num27];
												if (sponsorData.state_ == 0) { } else if ((sponsorData.state_ == 1)) {
													ary1.AddElement(apdat_.sponsorData_[num27]);
												}
											}
											for (int num28 = 0; num28 < apdat_.sponsorData_.Length; num28++)
											{
												SponsorData sponsorData2 = apdat_.sponsorData_[num28];
												if (sponsorData2.state_ == 0) { } else if ((sponsorData2.state_ == 2)) {
													ary1.AddElement(apdat_.sponsorData_[num28]);
												}
											}
										}
										else if (type_ == 43)
										{
											SponsorData sponsorData3 = sponsorData_;
											textLay_ = new TextLayout(false);
											textLay_.SetSize(124f, 47f);
											textLay_.SetColor(SC_WINDOW_BLACK);
											textLay_.SetLineSpace(5);
											textLay_.SetText(sponsorData3.explain_);
											textLay_.Flush();
										}
										else if (type_ == 44)
										{
											SponsorData sponsorData4 = sponsorData_;
											udat_.SetNegotiateResult(sponsorData4.feelPt_, sponsorData4.difficult_);
											apdat_.PlayBgm(4);
										}
										else if (type_ == 60 || type_ == 61)
										{
											ary1 = new Vector();
											for (int num29 = 0; num29 < apdat_.tenantData_.Length; num29++)
											{
												TenantData tenantData = apdat_.tenantData_[num29];
												if (type_ == 61) {
													if (tenantData.id_ == TenantData.TENANT_ID_STADIUM) { continue; }
													if (tenantData.id_ == TenantData.TENANT_ID_TRAININGAREA) { continue; }
													if (tenantData.id_ == TenantData.TENANT_ID_TRAININGAREA_SPRED) { continue; }
												}
												if (tenantData.state_ != 0) {
													ary1.AddElement(tenantData);
												}
											}
											if (ary1.Size() == 0)
											{
												Push(MakeDialog("施設がありません。"));
												return Pop();
											}
											TenantData tenantData2 = (TenantData)ary1.ElementAt(0);
											if (tenantData2.IsTAreaOrStadiumTenant())
											{
												textLay_ = new TextLayout(false);
												textLay_.SetSize(192f, 33f);
												textLay_.SetColor(SC_WINDOW_BLACK);
												textLay_.SetLineSpace(5);
												textLay_.SetText(tenantData2.explain_);
												textLay_.Flush();
											}
										}
										else if (type_ == 62) { }
										else if (type_ == 63) { }
										else if (type_ == 64) {
											TenantData tenantData3 = apdat_.tenantData_[tenantDataId_];
											int[] lvUpAddParam = tenantData3.GetLvUpAddParam();
											SetMeterParam(0, TLUP_ANIME_TIME[5] - TLUP_ANIME_TIME[4], tenantData3.paramUsePrice_, int.MaxValue, lvUpAddParam[0]);
											SetMeterParam(1, TLUP_ANIME_TIME[5] - TLUP_ANIME_TIME[4], tenantData3.paramKeepCost_, int.MaxValue, lvUpAddParam[1]);
											for (int num30 = 0; num30 < tenantData3.paramResearch_.Length; num30++)
											{
												SetMeterParam(2 + num30, TLUP_ANIME_TIME2[3] - TLUP_ANIME_TIME2[2], tenantData3.paramResearch_[num30], int.MaxValue, lvUpAddParam[2 + num30]);
											}
										}
										else {
												return Init2();
											}
									}
								}
							}
						}
					}
				}
			}
			return true;
		}

		public bool Init2()
		{
			if (type_ == 45)
			{
				for (int i = 0; i < TrainingData.TRA_PLAYER.Length; i++)
				{
					if (TrainingData.TRA_PLAYER[i] != -1)
					{
						continue;
					}
					bool flag = false;
					for (int j = 0; j < udat_.memberPDataist_.Size(); j++)
					{
						PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(j);
						if (TrainingData.TRA_PLAYER[i] != playerData.id_) { continue; }
flag = true;
break;
					}
					if (!flag)
					{
						TrainingData.TRA_PLAYER[i] = -1;
					}
				}
				bool flag2 = false;
				for (int k = 0; k < TrainingData.TRA_PLAYER.Length; k++)
				{
					if (TrainingData.TRA_PLAYER[k] != -1)
					{
						continue;
					}
					for (int l = 0; l < udat_.memberPDataist_.Size(); l++)
					{
						PlayerData playerData2 = (PlayerData)udat_.memberPDataist_.ElementAt(l);
						flag2 = false;
						for (int m = 0; m < TrainingData.TRA_PLAYER.Length; m++)
						{
							if (playerData2.id_ == TrainingData.TRA_PLAYER[m])
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							TrainingData.TRA_PLAYER[k] = playerData2.id_;
							break;
						}
					}
				}
			}
			else if (type_ == 46)
			{
				ary1 = new Vector();
				for (int n = 0; n < apdat_.trainingData_.Length; n++)
				{
					TrainingData trainingData = apdat_.trainingData_[n];
					if (trainingData.state_ == 1)
					{
						ary1.AddElement(trainingData);
					}
				}
				if (TrainingData.TRA_SELECT_TRA != -1)
				{
					int num = ary1.IndexOf(apdat_.trainingData_[TrainingData.TRA_SELECT_TRA]);
					select_ = num % 5;
					page_ = num / 5;
				}
			}
			else if (type_ == 47)
			{
				ary1 = new Vector();
				for (int num2 = 0; num2 < udat_.memberPDataist_.Size(); num2++)
				{
					ary1.AddElement(udat_.memberPDataist_.ElementAt(num2));
				}
				for (int num3 = 0; num3 < TrainingData.TRA_PLAYER.Length; num3++)
				{
					if (TrainingData.TRA_PLAYER[num3] != -1 && value1_ != num3)
					{
						PlayerData obj = apdat_.pData_[TrainingData.TRA_PLAYER[num3]];
						ary1.RemoveElement(obj);
					}
				}
				if (TrainingData.TRA_PLAYER[value1_] != -1)
				{
					PlayerData elem = apdat_.pData_[TrainingData.TRA_PLAYER[value1_]];
					select_ = ary1.IndexOf(elem);
					if (select_ == -1)
					{
						select_ = 0;
					}
					int num4 = 8;
					if (scrollValue_ > select_)
					{
						scrollValue_ = select_;
					}
					if (scrollValue_ + num4 <= select_)
					{
						scrollValue_ = select_ - num4 + 1;
					}
				}
			}
			else if (type_ == 48 || type_ == 49)
			{
				int num5 = GameUtil.Random(TrainingData.FARESULT_RATE[TrainingData.FARESULT_RATE.Length - 1]);
				for (int num6 = 0; num6 < TrainingData.FARESULT_RATE.Length; num6++)
				{
					if (num5 < TrainingData.FARESULT_RATE[num6])
					{
						TrainingData.fa_result = num6;
						break;
					}
				}
				TrainingData trainingData2 = apdat_.trainingData_[TrainingData.TRA_SELECT_TRA];
				for (int num7 = 0; num7 < TrainingData.TRA_PLAYER.Length; num7++)
				{
					if (TrainingData.TRA_PLAYER[num7] != -1)
					{
						PlayerData playerData3 = apdat_.pData_[TrainingData.TRA_PLAYER[num7]];
						for (int num8 = 0; num8 < trainingData2.eff_.Length; num8++)
						{
							TrainingData.TRA_ADD[num7][num8] = GameUtil.RateConvert(playerData3.paramPhysicalMax_[1], 200, 700, trainingData2.eff_[num8][0], trainingData2.eff_[num8][1], true);
						}
						playerData3.UpdatePhysicalParam();
						playerData3.SetTrainingAddParam(TrainingData.TRA_ADD[num7], TrainingData.TRA_UPPARAM[num7]);
						for (int num9 = 0; num9 < playerData3.paramPhysical_.Length; num9++)
						{
							TrainingData.TRA_UPPARAM_DISP[num7][num9] = TrainingData.TRA_UPPARAM[num7][num9] - playerData3.paramPhysical_[num9];
						}
					}
				}
			}
			else if (type_ == 50)
			{
				SupervisorData supervisorData = apdat_.svisorData_[udat_.selectSupervisorId_];
				udat_.SetPlayerParam(supervisorData.paramPhysical_, supervisorData.paramPhysical_);
				select_ = udat_.selectTrainingPolicy_;
			}
			else if (type_ == 51)
			{
				ary1 = new Vector();
				for (int num10 = 0; num10 < apdat_.svisorData_.Length; num10++)
				{
					if (apdat_.svisorData_[num10].state_ == 1)
					{
						ary1.AddElement(apdat_.svisorData_[num10]);
					}
				}
				SupervisorData supervisorData2 = (SupervisorData)ary1.FirstElement();
				udat_.SetPlayerParam(supervisorData2.paramPhysical_, supervisorData2.paramPhysical_);
			}
			else if (type_ != 52)
			{
				if (type_ == 53)
				{
					ary1 = new Vector();
					for (int num11 = 0; num11 < apdat_.tradeData_.Length; num11++)
					{
						TradeData tradeData = apdat_.tradeData_[num11];
						if (tradeData.state_ == 1)
						{
							PlayerData elem2 = apdat_.pData_[tradeData.playerDataId_];
							int num12 = udat_.memberPDataist_.IndexOf(elem2);
							if (num12 != -1)
							{
								tradeData.ChangeState(2);
							}
							else
							{
								ary1.AddElement(tradeData);
							}
						}
					}
					if (ary1.Size() == 0)
					{
						apdat_.ExecEvent(124, null, null);
						return Pop();
					}
				}
				else if (type_ != 54)
				{
					if (type_ == 55)
					{
						TradeData tradeData2 = tradeData_;
						udat_.SetNegotiateResult(tradeData2.feelPt_, tradeData2.difficult_);
						apdat_.PlayBgm(4);
					}
					else if (type_ != 56 && type_ != 57)
					{
						if (type_ == 58)
						{
							switch (udat_.GetMatchInfoJoin())
							{
							case 0:
							{
								LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
								apdat_.ExecEvent(18, leagueData.name_, null);
								return Pop();
							}
							case 1:
							{
								QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
								apdat_.ExecEvent(19, questMatchData.name_, null);
								return Pop();
							}
							}
							Vector vector = new Vector();
							Vector vector2 = new Vector();
							for (int num13 = 0; num13 < apdat_.tData_.Length; num13++)
							{
								if (num13 != 0)
								{
									vector.AddElement(apdat_.tData_[num13]);
								}
							}
							for (int num14 = 0; num14 < vector.Size() - 1; num14++)
							{
								for (int num15 = vector.Size() - 1; num15 > num14; num15--)
								{
									TeamData teamData = (TeamData)vector.ElementAt(num14);
									TeamData teamData2 = (TeamData)vector.ElementAt(num15);
									if (teamData2.warPotential_ > teamData.warPotential_)
									{
										vector.SetElementAt(teamData2, num14);
										vector.SetElementAt(teamData, num15);
									}
								}
							}
							int[][] array = new int[3][]
							{
								new int[2] { -20, 5 },
								new int[2] { -120, 20 },
								new int[2] { 0, 200 }
							};
							int warPotential_ = apdat_.tData_[0].warPotential_;
							for (int num16 = 0; num16 < array.Length; num16++)
							{
								for (int num17 = 0; num17 < apdat_.qmData_.Length; num17++)
								{
									QuestMatchData questMatchData2 = apdat_.qmData_[num17];
									if (questMatchData2.matchLeague_ != num16)
									{
										continue;
									}
									if (vector2.Size() > 0)
									{
										vector2.RemoveAllElements();
									}
									for (int num18 = 0; num18 < apdat_.tData_.Length; num18++)
									{
										if (num18 != 0)
										{
											TeamData teamData3 = apdat_.tData_[num18];
											if (teamData3.warPotential_ >= warPotential_ + array[num16][0] && teamData3.warPotential_ <= warPotential_ + array[num16][1])
											{
												vector2.AddElement(teamData3);
											}
										}
									}
									for (int num19 = 0; num19 < vector2.Size() - 1; num19++)
									{
										for (int num20 = vector2.Size() - 1; num20 > num16; num20--)
										{
											TeamData teamData4 = (TeamData)vector2.ElementAt(num19);
											TeamData teamData5 = (TeamData)vector2.ElementAt(num20);
											if (teamData5.warPotential_ > teamData4.warPotential_)
											{
												vector2.SetElementAt(teamData5, num19);
												vector2.SetElementAt(teamData4, num20);
											}
										}
									}
									if (num16 == 0 && questMatchData2.trophy_ == 0)
									{
										questMatchData2.matchEnemy_ = new int[1];
										questMatchData2.matchEnemy_[0] = 1;
									}
									else if (vector2.Size() == 0)
									{
										questMatchData2.matchEnemy_ = new int[1];
										int num21 = -1;
										for (int num22 = 0; num22 < vector.Size(); num22++)
										{
											TeamData teamData6 = (TeamData)vector.ElementAt(num22);
											if (teamData6.warPotential_ < warPotential_)
											{
												num21 = teamData6.id_;
												break;
											}
										}
										if (num21 != -1)
										{
											questMatchData2.matchEnemy_[0] = num21;
										}
										else
										{
											questMatchData2.matchEnemy_[0] = 1;
										}
									}
									else
									{
										questMatchData2.matchEnemy_ = new int[1];
										questMatchData2.matchEnemy_[0] = ((TeamData)vector2.ElementAt(GameUtil.Random(vector2.Size()))).id_;
									}
								}
							}
							ary1 = new Vector();
							for (int num23 = 0; num23 < apdat_.qmData_.Length; num23++)
							{
								QuestMatchData questMatchData3 = apdat_.qmData_[num23];
								if (questMatchData3.state_ != 0)
								{
									ary1.AddElement(questMatchData3);
								}
							}
							if (ary1.Size() == 0)
							{
								Push(MakeDialog("カップ戦が存在しません"));
								return Pop();
							}
						}
						else if (type_ == 59)
						{
							ary1 = new Vector();
							for (int num24 = 0; num24 < qmData_.matchEnemy_.Length; num24++)
							{
								ary1.AddElement(apdat_.tData_[qmData_.matchEnemy_[num24]]);
							}
							QuestMatchData questMatchData4 = qmData_;
							textLay_ = new TextLayout(false);
							if (En())
							{
								textLay_.SetAnchor(2);
							}
							textLay_.SetSize(194f, 32f);
							textLay_.SetColor(SC_WINDOW_BLACK);
							textLay_.SetLineSpace(5);
							textLay_.SetText(questMatchData4.explain_);
							textLay_.Flush();
						}
						else if (type_ == 65)
						{
							TenantData tenantData = apdat_.tenantData_[tenantDataId_];
							if (!BitUtil.Check(tenantData.flag_, 2))
							{
								SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[2]);
							}
						}
						else if (type_ != 66)
						{
							if (type_ == 67)
							{
								TeamData teamData7 = apdat_.tData_[0];
								int num25 = 0;
								for (int num26 = 0; num26 < teamData7.warP_paramPhysical_.Length; num26++)
								{
									num25 = 0;
									if (num26 == 4)
									{
										PlayerData playerData4 = (PlayerData)udat_.memberPDataist_.ElementAt(0);
										teamData7.warP_paramPhysical_[num26] = playerData4.paramPhysical_[num26];
										continue;
									}
									for (int num27 = 1; num27 < 11; num27++)
									{
										PlayerData playerData5 = (PlayerData)udat_.memberPDataist_.ElementAt(num27);
										num25 += playerData5.paramPhysical_[num26];
									}
									teamData7.warP_paramPhysical_[num26] = num25 / 10;
								}
								udat_.SetPlayerParam(teamData7.warP_paramPhysical_, teamData7.warP_paramPhysical_);
								sponsor_income_ = apdat_.frmSoccer_.GetSponsorPrice();
								sponsor_num_ = apdat_.frmSoccer_.GetSponsorNum();
							}
							else if (type_ == 68)
							{
								SetTicketWindowBtmMsg("左右ｷｰで設定します。");
								value1_ = udat_.ticketPrice_;
							}
							else if (type_ != 69 && type_ != 71 && type_ != 72 && type_ != 74 && type_ == 77)
							{
								ary1 = new Vector();
								for (int num28 = 0; num28 < apdat_.fanBonus_.Length; num28++)
								{
									if (BitUtil.Check(apdat_.fanBonus_[num28].flag_, 4))
									{
										ary1.AddElement(apdat_.fanBonus_[num28]);
									}
								}
							}
						}
					}
				}
			}
			return true;
		}

		public override void Draw(Graphics g)
		{
			int num = x_;
			int num2 = y_;
			if (view_.GetGameWidth() > 240 || view_.GetGameHeight() > 240)
			{
				FormBase form = formMan_.GetForm(0);
				int id = form.GetId();
				if ((id == 9) && (IsMenu())) { } else if ((id == 2) && (IsMenu())) { } else {
					num += (view_.GetGameWidth() - 240) / 2;
					num2 += (view_.GetGameHeight() - 240) / 2;
				}
			}
			g.SetOrigin(num, num2);
			try
			{
				_draw(g);
			}
			catch (Exception ex)
			{
				if (Config.DEBUG_CMD)
				{
					Log.Info("■SubForm.java draw()でエラー発生 type_ = " + type_ + ", toString() = " + ToString());
				}
				throw ex;
			}
			finally
			{
				g.SetOrigin(0, 0);
			}
		}

		private void _draw(Graphics g)
		{
			err_method = "_draw()";
			if (untilActiveHide_ || wait_ > 0)
			{
				return;
			}
			int num24;
int num;
int num2;
int num3;


			num24 = num = num2 = num3 = 0;
			Font font = g.GetFont(null);
			bool flag = false;
			FormBase topForm = formMan_.GetTopForm();
			if (topForm.GetId() == 4)
			{
			}
			if (IsDrawWindow()) { } else {
				if (!IsMenu() && !flag && !apdat_.IsTopForm(this))
				{
					return;
				}
				if (IsMenu() && !apdat_.IsTopForm(this))
				{
					FormBase topForm2 = formMan_.GetTopForm();
					if (topForm2.GetId() == 4 && !((SubForm)topForm2).IsMenu())
					{
						return;
					}
				}
			}
			if (type_ == 1 || type_ == 16 || type_ == 21)
			{
				err_line = 205;
				apdat_.DrawWindow(g, 210, 110, 0, "メッセージ");
				apdat_.DrawBox(g, 26, 90, 213, 148);
				if (text_ != null)
				{
					TextLayout.Draw(g, text_, 26f, 115f, 187f, 32f, SC_WINDOW_BLACK, 6, 34);
				}
				err_line = 206;
				if (type_ != 16 || (saveState_ == 2 && selectButtons_ != null && font != null))
				{
					float num4 = 0f;
					err_line = 2061;
					for (int i = 0; i < selectButtons_.Length; i++)
					{
						if (selectButtons_[i] == null) { continue; }
							float num5 = font.StringWidthF(selectButtons_[i]);
							if (num4 < num5)
							{
								num4 = num5;
							}

					}
					num4 += 2f;
					err_line = 2062;
					int num6 = 72;
					int num7 = 200;
					int num8 = 200;
					for (int j = 0; j < selectButtons_.Length; j++)
					{
						if (selectButtons_[j] == null) { continue; }
							int num9 = 120 - (selectButtons_.Length - 1) * num6 / 2 + j * num6;
							if (j == select_)
							{
								g.SetColor(SC_WINDOW_SELECT);
								g.FillRect((float)num9 - num4 / 2f - 5f, 164f, num4 + 10f, 17f);
							}
							g.SetColor(SC_WINDOW_BLACK);
							g.DrawString(selectButtons_[j], num9, 167f, 2);
							GameView.AddTouch(g, 3, (int)((float)num9 - num4 / 2f - 5f - (float)(num7 / 2)), 164 - num8 / 2, (int)(num4 + 10f + (float)num7), 17 + num8, 0x20000 | j);

					}
				}
				err_line = 207;
			}
			else
			{
				if (type_ == 2) { } else if (type_ == 3)
				{
					DrawMenu_btmInfo(g);
					DrawMenu(g, 0);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 4)
				{
					DrawMenu(g, 0);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 8)
				{
					DrawMenu(g, 1);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 5)
				{
					DrawMenu(g, 1);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 7)
				{
					DrawMenu(g, 1);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 6)
				{
					DrawMenu(g, 1);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 9)
				{
					DrawMenu(g, 1);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 10)
				{
					DrawMenu(g, 1);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 11)
				{
					DrawMenu(g, 1);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 12)
				{
					DrawMenu(g, 1);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 22)
				{
					DrawMenu(g, 1);
					GameView.AddTouch(4, 22, TouchOption.Create(2));
				}
				else if (type_ == 0)
				{
					apdat_.DrawWindow(g, 202, 122, 0, "会話ウィンドウ");
					apdat_.DrawBox(g, 27, 82, 214, 159);
					g.SetColor(SC_WINDOW_BLACK);
					textLay_.Draw(g, 37f, 100f);
					if (humanCharaType_ == 0)
					{
						Draw_chara(g, 172, 0, 1, frame_, 193, 189);
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString("姫川チア子", 84f, 174f);
					}
					else if (humanCharaType_ == 1)
					{
						if (humanCharaIndex_ != -1)
						{
							PlayerData playerData = apdat_.pData_[humanCharaIndex_];
							udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, 0, 2);
							udat_.DrawDispPlayer(g, 193, 189);
							g.SetColor(SC_WINDOW_BLACK);
							if (MyFormBase.Jp())
							{
								g.DrawString(playerData.name_ + playerData.name2_, 84f, 174f);
							}
							else
							{
								g.DrawString(playerData.name_ + " " + playerData.name2_, 84f, 174f);
							}
						}
					}
					else if (humanCharaType_ == 2)
					{
						if (humanCharaIndex_ != -1)
						{
							SponsorData sponsorData = apdat_.sponsorData_[humanCharaIndex_];
							SponsorData.Draw_sponsorChara(g, sponsorData.charaIndex_, 193, 189);
							g.SetColor(SC_WINDOW_BLACK);
							g.DrawString(sponsorData.name_ + string.Empty, 84f, 174f);
						}
					}
					else if (humanCharaType_ == 3)
					{
						if (humanCharaIndex_ != -1)
						{
							SupervisorData supervisorData = apdat_.svisorData_[humanCharaIndex_];
							g.SetColor(SC_WINDOW_BLACK);
							string text = MyFormBase.LT(supervisorData.name_);
							string text2 = MyFormBase.LT(supervisorData.name2_);
							string text3 = MyFormBase.LT("監督");
							if (MyFormBase.Jp())
							{
								g.DrawString(text + text2 + text3, 60f, 174f);
							}
							else
							{
								g.DrawString(text + " " + text2 + text3, 60f, 174f);
							}
							udat_.SetDispPlayerData(0, supervisorData.bodyImg_, supervisorData.headImg_, 0, 2);
							udat_.DrawDispPlayer(g, 193, 189);
						}
					}
					else if (humanCharaType_ == 4 && humanCharaIndex_ != -1)
					{
						SponsorData.Draw_sponsorChara(g, udat_.TALK_SCHARA_IMGINDEX[humanCharaIndex_], 193, 189);
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString(udat_.TALK_SCHARA_NAME[humanCharaIndex_] + string.Empty, 84f, 174f);
					}
					GameView.AddTouch(2);
				}
				else if (type_ == 13)
				{
					int num10 = eventIds_[page_];
					apdat_.DrawWindow(g, 222, 170, 0, "イベント");
					apdat_.DrawBox(g, 15, 139, 224, 202);
					num = 67;
					if (num10 == 0)
					{
						g.DrawImage(apdat_.resEvent_.img[0], 30f, num);
					}
					else
					{
						g.DrawImage(apdat_.resEvent_.img[1], 30f, num);
					}
					num = 93;
					g.DrawImage(apdat_.resEvent_.img[2], 30f, num);
					num = 159;
					int num11 = GameUtil.RateConvert(frame_, 5, 60, 0, 48, true);
					g.ClipRect(0f, num, 240f, num11);
					if (MyFormBase.Jp())
					{
						TextLayout.Draw(g, eventTexts_[page_], 28f, num, 182f, 49f, SC_WINDOW_BLACK, 6, 2);
					}
					else
					{
						TextLayout.Draw(g, eventTexts_[page_], 28f, num, 182f, 46f, SC_WINDOW_BLACK, 6, 34);
					}
					g.PopClip();
					int num12 = 30;
					int num13 = 16;
					int num14 = 6;
					int num15 = num13 + num14 * 3 + num12 + num13;
					int num16 = 0;
					int num17 = 0;
					float num18 = GameUtil.SetVStop(-6f, num12 / 2);
					float num19 = GameUtil.SetAStop(-6f, num12 / 2);
					g.ClipRect(30f, 67f, 180f, 58f);
					int num20 = frame_ % num15;
					for (int k = 0; k < 4; k++)
					{
						num2 = 56 + 42 * k;
						num16 = 129;
						if (num10 == 0)
						{
							int num21 = num20 - (num13 + num14 * k);
							if (num21 >= 0 && num21 < num12)
							{
								num17 = 1;
								num16 += (int)(num18 * (float)num21 + num19 * (float)num21 * (float)(num21 + 1) / 2f);
								num16 = AppData.ClampMax(num16, 129);
							}
							else
							{
								num17 = 0;
							}
							apdat_.DrawSeb(g, num2, num16, apdat_.resEvent_.seb[2], apdat_.resEvent_.img[5], num17);
						}
						else
						{
							num16 = 125;
							num17 = frame_ % 44 / 22;
							apdat_.DrawSeb(g, num2 + num17, num16, apdat_.resEvent_.seb[3], apdat_.resEvent_.img[6], num17);
						}
					}
					g.PopClip();
					num = 67;
					g.ClipRect(0f, num, 240f, 79f);
					int num22 = frame_ / 2;
					if (num10 == 0) {if (num22 > EVENT_HAPPY_ANIME_OY.Length - 1)
						{
							num22 = EVENT_HAPPY_ANIME_OY.Length - 1;
						}
						num3 = EVENT_HAPPY_ANIME_OY[num22] + 148;
						apdat_.DrawSeb(g, 95, num3, apdat_.resEvent_.seb[0], apdat_.resEvent_.img[3], 0);
						apdat_.DrawSeb(g, 143, num3, apdat_.resEvent_.seb[0], apdat_.resEvent_.img[3], 1);
						} else if (num10 == 1) {if (num22 > EVENT_SAD_ANIME_OY.Length - 1)
						{
							num22 = EVENT_SAD_ANIME_OY.Length - 1;
						}
						num3 = EVENT_SAD_ANIME_OY[num22] + 148 - 3;
						apdat_.DrawSeb(g, 95, num3, apdat_.resEvent_.seb[1], apdat_.resEvent_.img[4], 0);
						apdat_.DrawSeb(g, 142, num3, apdat_.resEvent_.seb[1], apdat_.resEvent_.img[4], 1);
						}
					g.PopClip();
					GameView.AddTouch(2);
				}
				else if (type_ == 14)
				{
					bool flag2 = MyFormBase.Jp();
					if (Language.GetLanguageCode() != "ja-JP" && Language.GetLanguageCode() != "en-US" && !Language.GetLanguageCode().StartsWith("*"))
					{
						flag2 = true;
					}
					if (flag2)
					{
						DrawConfig_jp(g);
					}
					else if (En())
					{
						DrawConfig_en(g);
					}
				}
				else if (type_ == 15)
				{
					int num23 = 0;
					if (!MyFormBase.Jp())
					{
						num23 = 40;
					}
					if (MyFormBase.Jp())
					{
						apdat_.DrawWindow(g, 180 + num23, 166, 0, "遊び方 " + (page_ + 1) + "/" + apdat_.manualTexts_.Length);
					}
					else if (En())
					{
						apdat_.DrawWindow(g, 180 + num23, 166, 0, MyFormBase.LT("遊び方") + " " + (page_ + 1) + "/" + apdat_.manualTexts_.Length);
					}
					GameView.AddTouch(g, 1, 44 - frame_ % 20 / 5 - num23 / 2, 56, apdat_.resCom2_.seb[24], 0, 16);
					GameView.AddTouch(g, 1, 193 + frame_ % 20 / 5 + num23 / 2, 56, apdat_.resCom2_.seb[24], 1, 18);
					apdat_.DrawBox(g, 37 - num23 / 2, 60, 203 + num23 / 2, 197);
					if (page_ == apdat_.manualTexts_.Length - 1)
					{
						g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
						g.DrawString("このゲームについて", 120f, 80f, 2);
						if (!Config.TRIAL_VERSION)
						{
							g.DrawString("サッカークラブ物語", 120f, 166f, 2);
						}
						else
						{
							g.DrawString(MyFormBase.LT("サッカークラブ物語") + "Lite", 120f, 166f, 2);
						}
						g.DrawString("for カイロパーク", 120f, 182f, 2);
						num24 = 43;
						num = 101;
						int num25 = 154;
						int num26 = 56;
						g.DrawImage(apdat_.resCom2_.img[162], num24, num, 13, 18, num25, num26);
						g.SetColor(Graphics.GetColorOfRGB(43, 116, 190));
						g.DrawRect(num24, num, num25 - 1, num26 - 1);
						g.ClipRect(num24, num, num25, num26);
						int num27 = anime_cnt % 56;
						PlayerData playerData2 = apdat_.pData_[111];
						PlayerData playerData3 = apdat_.pData_[112];
						int num28 = 0;
						int num29 = 0;
						if (num27 < 6)
						{
							num28 = 1;
							num29 = 1;
						}
						else
						{
							num28 = 0;
							num29 = 0;
						}
						udat_.SetDispPlayerData(num28, 2, playerData3.faceImg_, num29, 1);
						udat_.DrawDispPlayer(g, 73, 146);
						if (num27 >= 28 && num27 < 34)
						{
							num28 = 1;
							num29 = 1;
						}
						else
						{
							num28 = 0;
							num29 = 0;
						}
						udat_.SetDispPlayerData(num28, 1, playerData2.faceImg_, num29, 2);
						udat_.DrawDispPlayer(g, 168, 146);
						int x = GameUtil.RateConvert(JMath.Abs(num27 - 28), 28, 0, 80, 161, true);
						num = 146;
						int num30 = num;
						if (anime_mode == 1)
						{
							float num31 = GameUtil.SetVStop(anime_ballY, 14);
							float num32 = GameUtil.SetAStop(anime_ballY, 14);
							int num33 = JMath.Abs(num27 - 28);
							num30 = (int)(num31 * (float)num33 + num32 * (float)num33 * (float)(num33 + 1) / 2f) + num;
							if (num30 > num)
							{
								num30 = num;
							}
						}
						int num34 = num27 % 28;
						num29 = num34 % 12 / 3;
						if (num27 < 28)
						{
							num29 = 3 - num29;
						}
						apdat_.DrawSeb(g, x, num30, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], num29);
						g.PopClip();
					}
					else
					{
						textLay_.Draw(g, 48 - num23 / 2, 80f);
					}
					GameView.AddTouch(2, TouchOption.Create(2));
				}
				else if (type_ == 17)
				{
					if (MyFormBase.Jp())
					{
						apdat_.DrawWindow(g, 194, 186, 0, "開幕ｷｯｸｵﾌ通信 " + (page_ + 1) + "/" + pagesNum_ + string.Empty);
					}
					else if (En())
					{
						apdat_.DrawWindow(g, 194, 186, 0, MyFormBase.LT("開幕ｷｯｸｵﾌ通信 ") + " " + (page_ + 1) + "/" + pagesNum_ + string.Empty);
					}
					g.DrawImage(apdat_.resCom2_.img[133], 29f, 62f);
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString(name_, 120f, 69f, 2);
					if (pagesNum_ > 1)
					{
						GameView.AddTouch(g, 1, 44 - frame_ % 20 / 5, 46, apdat_.resCom2_.seb[24], 0, 16);
						GameView.AddTouch(g, 1, 193 + frame_ % 20 / 5, 46, apdat_.resCom2_.seb[24], 1, 18);
					}
					apdat_.DrawBox(g, 31, 82, 210, 209);
					textLay_.Draw(g, 41f, 106f, page_ * 18 * 6);
					GameView.AddTouch(2, TouchOption.Create(2));
				}
				else
				{
					if (type_ == 18) { } else if (type_ == 19)
					{
						Draw_clearPoint(g);
						GameView.AddTouch(2, TouchOption.Create(2));
					}
					else
					{
						if (type_ == 20) { } else if (type_ == 23)
						{
							int num35 = GameUtil.RateConvert(frame_, 1, 3, 6, 232, true);
							int num36 = textLay_.GetHeight() + 7 + 8;
							if (num36 < 59)
							{
								num36 = 59;
							}
							num24 = 4;
							num = 120 - num36 / 2;
							num24 = 120 - num35 / 2;
							g.SetColor(Graphics.GetColorOfRGB(43, 37, 32));
							g.DrawRect(num24, num, num35 - 1, num36 - 1);
							g.DrawLine(num24, num + num36 - 2, num24 + num35 - 1, num + num36 - 2);
							g.SetColor(Graphics.GetColorOfRGB(199, 184, 175));
							g.DrawRect(num24 + 1, num + 1, num35 - 3, num36 - 4);
							g.SetColor(Graphics.GetColorOfRGB(163, 150, 141));
							g.DrawRect(num24 + 2, num + 2, num35 - 5, num36 - 6);
							g.SetColor(Graphics.GetColorOfRGB(72, 61, 55));
							g.FillRect(num24 + 3, num + 3, num35 - 6, num36 - 7);
							if (frame_ >= 3)
							{
								g.SetColor(Graphics.GetColorOfRGB(241, 241, 241));
								g.SetColor(SC_WINDOW_WHITE);
								textLay_.Draw(g, 120f, 120 - textLay_.GetHeight() / 2);
								if (frame_ % 50 < 30)
								{
									apdat_.DrawSeb(g, 223, 120 + num36 / 2 - 8, apdat_.resCom2_.seb[63], apdat_.resCom2_.img[72], 0);
								}
							}
						}
						else if (type_ == 24 || type_ == 25)
						{
							if (type_ == 24)
							{
								int num37 = GameUtil.RateConvert(frame_, 0, FADE_TIME_V, 0, 255, true);
								if (num37 > 0)
								{
									g.SetRenderMode(2, num37, 255);
									g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
									g.FillRect(0f, SoccerForm.VIEW_Y, 240f, SoccerForm.VIEW_H);
									g.SetRenderMode(0, 255, 0);
								}
							}
							else if (type_ == 25)
							{
								int num38 = GameUtil.RateConvert(frame_, 0, FADE_TIME_V, 255, 0, true);
								if (num38 > 0)
								{
									g.SetRenderMode(2, num38, 255);
									g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
									g.FillRect(0f, SoccerForm.VIEW_Y, 240f, SoccerForm.VIEW_H);
									g.SetRenderMode(0, 255, 0);
								}
							}
						}
						else if (type_ == 26)
						{
							if (apdat_.IsTopForm(this))
							{
								udat_.Draw_edit(g, false);
								udat_.Draw_edit2(g, false);
							}
						}
						else if (type_ == 27 || type_ == 28 || type_ == 47 || type_ == 31)
						{
							if (type_ == 47)
							{
								apdat_.DrawWindow(g, 230, 200, 0, "選手選択");
							}
							else if (type_ == 31)
							{
								apdat_.DrawWindow(g, 230, 200, 0, "選手解雇");
							}
							else
							{
								apdat_.DrawWindow(g, 230, 200, 0, "選手情報");
							}
							bool flag3 = false;
							num24 = 11;
							num = 67;
							int num39 = 63;
							int num40 = 18;
							int num41 = 0;
							for (int l = scrollValue_; l < ary1.Size(); l++)
							{
								int num42 = l - scrollValue_;
								if (num42 >= 8)
								{
									break;
								}
								flag3 = false;
								PlayerData playerData4 = (PlayerData)ary1.ElementAt(l);
								int num43 = udat_.memberPDataist_.IndexOf(playerData4);
								if (num43 < 11)
								{
									flag3 = true;
								}
								num2 = num24;
								num3 = num + (num40 + num41) * num42;
								if (flag3)
								{
									g.SetColor(Graphics.GetColorOfRGB(255, 230, 81));
								}
								else
								{
									g.SetColor(Graphics.GetColorOfRGB(197, 197, 197));
								}
								g.FillRect(num2, num3, num39, num40);
								g.SetColor(Graphics.GetColorOfRGB(180, 221, 210));
								g.DrawRect(num2, num3, num39 - 1, num40 - 1);
								GameView.AddTouch(g, 11, num2, num3, num39, num40, 0x20000 | l, TouchOption.Create(16).Margin(30, 0, 0, 0));
								g.SetColor(Graphics.GetColorOfRGB(212, 255, 243));
								g.FillRect(num2 + 1, num3 + 1, 20f, num40 - 2);
								g.SetColor(Graphics.GetColorOfRGB(180, 221, 210));
								g.DrawLine(num2 + 14 + 7, num3, num2 + 14 + 7, num3 + num40 - 1);
								g.SetColor(SC_WINDOW_BLACK);
								if (flag3)
								{
									apdat_.DrawSeb(g, num2 + 1, num3 + 3, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], playerData4.role_);
								}
								else
								{
									apdat_.DrawSeb(g, num2 + 1, num3 + 3, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], 4);
								}
								g.SetColor(SC_WINDOW_BLACK);
								SetFontSize(g, 9);
								if (MyFormBase.Jp())
								{
									g.DrawString(playerData4.name_, num2 + num39 / 2 + 10, num3 + 4, 2);
								}
								else
								{
									g.DrawString(playerData4.name_, num2 + num39 / 2 + 11, num3 + 4, 2);
								}
								ResetFontSize(g);
								if (playerData4.IsNew())
								{
									Draw_newMark(g, num2, num3 + 3, frame_);
								}
								if (l == select_)
								{
									apdat_.resCom2_.seb[60].Draw(g, num2 - 4 + 5, num3 + 9, apdat_.resCom2_.img);
								}
							}
							TouchOption touchOption = TouchOption.Create(2);
							TouchOption touchOption2 = TouchOption.Create(2);
							GameView.AddTouch(g, 24, num24 + num39 / 2, num - 13, apdat_.resCom2_.seb[24], 0, 17, touchOption.Margin(30, 30, 60, 5));
							GameView.AddTouch(g, 24, num24 + num39 / 2, num + num40 * 7 + 13, apdat_.resCom2_.seb[24], 1, 19, touchOption2.Margin(30, 30, 5, 60));
							apdat_.DrawVerticalScroll2(g, num24 + 56 + 7 + 1 + 2, num, 3, 145, scrollValue_, ary1.Size() - 1, 8);
							PlayerData playerData5 = (PlayerData)ary1.ElementAt(select_);
							flag3 = false;
							int num44 = udat_.memberPDataist_.IndexOf(playerData5);
							if (num44 < 11)
							{
								flag3 = true;
							}
							num24 = 81;
							num = 47;
							apdat_.DrawBox(g, num24, num, num24 + 148, num + 61);
							num24 = 89;
							num = 44;
							g.DrawImage(apdat_.resCom2_.img[125], num24 + 5, num + 17);
							if (playerData5.IsNew())
							{
								Draw_newMark(g, num24 + 6 + 3, num + 8, frame_);
							}
							else if (flag3)
							{
								g.DrawImage(apdat_.resCom2_.img[124], num24 + 6, num + 8);
							}
							int frame = frame_ % 16 / 4;
							udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData5.faceImg_, frame, 1);
							udat_.DrawDispPlayer(g, num24 + 22, num + 44);
							num2 = num24 + 31;
							num3 = num + 46;
							g.SetColor(Graphics.GetColorOfRGB(29, 108, 1));
							g.DrawLine(num2, num3, num2, num3 - 16);
							g.DrawLine(num2 + 3, num3, num2 + 3, num3 - 16);
							int num45 = GameUtil.RateConvert(playerData5.auraPoint_, 0, 10000, 0, 17, true);
							g.SetColor(Graphics.GetColorOfRGB(244, 216, 93));
							g.FillRect(num2 + 1, num3 - (num45 - 1), 2f, num45);
							g.SetColor(Graphics.GetColorOfRGB(60, 42, 16));
							g.FillRect(num2 + 1, num3 - 16, 2f, 17 - num45);
							num24 = 138;
							num = 65;
							string text4 = playerData5.name_ + " " + playerData5.name2_;
							int num46 = font.StringWidth(text4) + 6;
							g.SetColor(SC_WINDOW_SELECT);
							g.FillRect(num24 - 3, num - 1, num46, 14f);
							GameView.AddTouch(g, 4, view_.GetGameWidth() / 2 + 18 - 20, view_.GetGameHeight() / 2 - 55 - 10, 110, 70, 20, TouchOption.Create(2));
							g.SetColor(SC_WINDOW_BLACK);
							g.DrawString(playerData5.name_ + " " + playerData5.name2_, num24, num);
							num24 = 142;
							num = 79;
							if (MyFormBase.Jp())
							{
								Draw_categoryIcon(g, playerData5.category_, num24, num);
								g.SetColor(SC_WINDOW_BLACK);
								g.DrawString(PlayerData.CATEGORY_STR[playerData5.category_], num24 + 20, num + 2);
							}
							else
							{
								Draw_categoryIcon(g, playerData5.category_, num24 - 4, num);
								g.SetColor(SC_WINDOW_BLACK);
								SetFontSize(g, 10);
								g.DrawString(PlayerData.CATEGORY_STR[playerData5.category_], num24 - 4 + 20 - 2, num + 2 + 1);
								ResetFontSize(g);
							}
							num2 = 85;
							num3 = 96;
							g.SetColor(Graphics.GetColorOfRGB(221, 251, 219));
							g.FillRect(num2, num3, 54f, 21f);
							g.DrawImage(apdat_.resCom2_.img[126], num2 + 10, num3 + 3);
							apdat_.DrawNumImage(g, 79, playerData5.lv_, num2 + 38, num3 + 1, 0, 2);
							g.DrawImage(apdat_.resCom2_.img[118], num2 + 1, num3 + 14, 1, 0, 52, 5);
							int c1min = 0;
							int nextLvUpExp = PlayerData.GetNextLvUpExp(playerData5.lv_);
							int width = GameUtil.RateConvert(playerData5.exp_, c1min, nextLvUpExp, 0, 50, true);
							g.DrawImage(apdat_.resCom2_.img[118], num2 + 1 + 1, num3 + 14 + 1, 2, 5, width, 3);
							g.SetColor(SC_WINDOW_BLUE);
							SetFontSize(g, 8);
							if (MyFormBase.Jp())
							{
								g.DrawString("給料", 146f, 101f);
							}
							else
							{
								g.DrawString("給料", 141f, 104f);
							}
							ResetFontSize(g);
							if (MyFormBase.Jp())
							{
								apdat_.DrawNumImageComma(g, 35, playerData5.GetSallay(), 212, 102, 0, 0, 4);
								apdat_.DrawSeb(g, 213, 102, apdat_.resCom2_.seb[35], apdat_.resCom2_.img[103], 17);
							}
							else
							{
								apdat_.DrawMoney(g, 35, playerData5.GetSallay(), 223, 102, 0, 0, 4);
							}
							num24 = 81;
							num = 110;
							apdat_.DrawBox(g, num24, num, num24 + 148, num + 104);
							num2 = num24 + 11;
							num3 = num + 15;
							for (int m = 0; m < 4; m++)
							{
								g.DrawImage(apdat_.resCom2_.img[123], num2 + 31 * m, num3);
								g.SetColor(SC_WINDOW_BLACK);
								int positionRank = PlayerData.GetPositionRank(m, playerData5);
								apdat_.DrawPositionStatus(g, m, positionRank, num2 + 31 * m + 4, num3 + 4);
							}
							num += 36;
							num2 = num24 + 11;
							for (int n = 0; n < PlayerData.PARAM_NAME.Length; n++)
							{
								num3 = num + 15 * n;
								apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[74], apdat_.resCom2_.img[121], n);
								apdat_.DrawSeb(g, num2 + 16, num3, apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], n);
								g.DrawImage(apdat_.resCom2_.img[120], num2 + 43, num3, 0, 0, 82, 15);
								int phyParamBarW = PlayerData.GetPhyParamBarW(playerData5.paramPhysical_[n], 80);
								g.DrawImage(apdat_.resCom2_.img[120], num2 + 43 + 1, num3 + 3, 1, 16 + 10 * n, phyParamBarW, 9);
								if (UserData.DEBUG_MEMBERPARAM_DISP == 1)
								{
									apdat_.DrawNumImage(g, 89, playerData5.paramPhysical_[n], num2 + 125 + 5, num3 + 5, 0, 4);
								}
							}
						}
						else if (type_ == 29) { } else {
							_draw2(g);
						}
					}
				}
			}
		}

		private void _draw2(Graphics g)
		{
			int num4, num, num2, num3;


			num4 = num = num2 = num3 = 0;
			Font font = g.GetFont(null);
			if (type_ == 30)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "選手情報");
				apdat_.DrawBox(g, 12, 69, 227, 190);
				num4 = 20;
				num = 63;
				PlayerData playerData = playerData_;
				num3 = num + 2;
				string text = playerData.name_ + " " + playerData.name2_;
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(text, 120f, num3, 2);
				int num5 = g.GetFont(null).StringWidth(text);
				apdat_.DrawSeb(g, 120 - num5 / 2 - 27, num3, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], playerData.role_);
				g.DrawImage(apdat_.resCom2_.img[162], 16f, 85f, 0, 16, 122, 64);
				g.SetColor(Graphics.GetColorOfRGB(144, 165, 163));
				g.DrawRect(16f, 85f, 121f, 63f);
				num4 = 16;
				num = 83;
				int num6 = udat_.memberPDataist_.IndexOf(playerData);
				if (num6 < 11)
				{
					g.DrawImage(apdat_.resCom2_.img[124], num4, num);
				}
				num4 = 77;
				num = 125;
				int frame = frame_ % 16 / 4;
				udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, frame, 1);
				udat_.DrawDispPlayer(g, num4, num);
				num2 = 86;
				num3 = 124;
				g.SetColor(Graphics.GetColorOfRGB(29, 108, 1));
				g.DrawLine(num2, num3, num2, num3 - 16);
				g.DrawLine(num2 + 3, num3, num2 + 3, num3 - 16);
				int num7 = GameUtil.RateConvert(playerData.auraPoint_, 0, 10000, 0, 17, true);
				g.SetColor(Graphics.GetColorOfRGB(244, 216, 93));
				g.FillRect(num2 + 1, num3 - (num7 - 1), 2f, num7);
				g.SetColor(Graphics.GetColorOfRGB(60, 42, 16));
				g.FillRect(num2 + 1, num3 - 16, 2f, 17 - num7);
				num2 = 149;
				num3 = 85;
				if (MyFormBase.Jp())
				{
					Draw_categoryIcon(g, playerData.category_, num2, num3);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(PlayerData.CATEGORY_STR[playerData.category_], num2 + 72, num3 + 2, 4);
				}
				else
				{
					Draw_categoryIcon(g, playerData.category_, num2 - 3, num3);
					g.SetColor(SC_WINDOW_BLACK);
					SetFontSize(g, 10);
					g.DrawString(PlayerData.CATEGORY_STR[playerData.category_], num2 - 3 + 16, num3 + 2 + 1, 1);
					ResetFontSize(g);
				}
				num2 = 143;
				num3 = 105;
				g.SetColor(SC_WINDOW_BLUE);
				SetFontSize(g, 10);
				g.DrawString("給料", num2, num3);
				ResetFontSize(g);
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(MyFormBase.YEN("万", playerData.GetSallay()), num2 + 78, num3, 4);
				num2 = 143;
				num3 = 123;
				g.SetColor(SC_WINDOW_BLUE);
				SetFontSize(g, 10);
				g.DrawString("人気", num2, num3);
				ResetFontSize(g);
				g.SetColor(SC_WINDOW_BLACK);
				if (En())
				{
					g.DrawString(playerData.popularP_ + string.Empty, num2 + 78, num3, 4);
				}
				else
				{
					g.DrawString(playerData.popularP_ + string.Empty, num2 + 78 - 12, num3, 4);
				}
				num2 = 16;
				num3 = 155;
				g.SetColor(Graphics.GetColorOfRGB(221, 251, 219));
				g.FillRect(num2, num3, 124f, 23f);
				g.DrawImage(apdat_.resCom2_.img[126], num2 + 15, num3 + 5);
				apdat_.DrawNumImage(g, 79, playerData.lv_, num2 + 43, num3 + 2, 0, 2);
				g.DrawImage(apdat_.resCom2_.img[118], num2 + 6, num3 + 17, 1, 0, 52, 5);
				int c1min = 0;
				int nextLvUpExp = PlayerData.GetNextLvUpExp(playerData.lv_);
				int width = GameUtil.RateConvert(playerData.exp_, c1min, nextLvUpExp, 0, 50, true);
				g.DrawImage(apdat_.resCom2_.img[118], num2 + 6 + 1, num3 + 17 + 1, 2, 5, width, 3);
				if (En())
				{
					g.DrawImage(apdat_.resCom2_.img[115], num2 + 87 + 4, num3 + 2);
				}
				else
				{
					g.DrawImage(apdat_.resCom2_.img[115], num2 + 87, num3 + 2);
				}
				apdat_.DrawExp(g, num2 + 69, num3 + 2);
				apdat_.DrawNumImage(g, 78, nextLvUpExp - playerData.exp_, num2 + 110, num3 + 11, 0, 4);
				num2 = 15;
				num3 = 180;
				for (int i = 0; i < 4; i++)
				{
					g.DrawImage(apdat_.resCom2_.img[123], num2 + 31 * i, num3);
					g.SetColor(SC_WINDOW_BLACK);
					int positionRank = PlayerData.GetPositionRank(i, playerData);
					apdat_.DrawPositionStatus(g, i, positionRank, num2 + 31 * i + 4, num3 + 4);
				}
				num4 = 141;
				num = 140;
				g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
				g.DrawRect(num4, num, 83f, 56f);
				int[] array = new int[5] { 8, 11, 13, 9, 11 };
				for (int j = 0; j < PlayerData.PARAM_NAME.Length; j++)
				{
					num3 = num + 11 * j;
					int num8 = 12;
					if (j == PlayerData.PARAM_NAME.Length - 1)
					{
						num8 = 13;
					}
					g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
					g.DrawRect(num4, num3, 30f, num8 - 1);
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 223));
					g.FillRect(num4 + 1, num3 + 1, 29f, num8 - 2);
					int num9 = array[j];
					if (AppData.IsLanguageJpBase(false))
					{
						num9 = 12;
					}
					apdat_.DrawSeb(g, num4 + 15 - num9, num3 - 1, apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], j);
					g.SetColor(Graphics.GetColorOfRGB(108, 94, 86));
					g.DrawRect(num4 + 31, num3 + 1, 51f, 10f);
					g.SetColor(Graphics.GetColorOfRGB(57, 57, 70));
					g.FillRect(num4 + 31 + 1, num3 + 1 + 1, 50f, 9f);
					int phyParamBarW = PlayerData.GetPhyParamBarW(playerData.paramPhysical_[j], 50);
					if ((j == 0)) { g.SetColor(Graphics.GetColorOfRGB(252, 54, 0)); } else if ((j == 1)) { g.SetColor(Graphics.GetColorOfRGB(84, 188, 255)); } else if ((j == 2)) { g.SetColor(Graphics.GetColorOfRGB(103, 255, 121)); } else if ((j == 3)) { g.SetColor(Graphics.GetColorOfRGB(248, 248, 248)); } else if ((j == 4)) { g.SetColor(Graphics.GetColorOfRGB(255, 251, 0)); }
					g.FillRect(num4 + 32, num3 + 2, phyParamBarW, 9f);
				}
				num = 204;
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(MyFormBase.LT("出場\u3000<0>試合", playerData.matchNum_), 120f, num, 2);
				GameView.AddTouch(2);
			}
			else if (type_ == 32)
			{
				apdat_.DrawWindow(g, 228, 176, 0, MyFormBase.LT("説明 <0>/2", select_ + 1));
				apdat_.DrawBox(g, 12, 69, 227, 190);
				GameView.AddTouch(g, 1, 44 - frame_ % 20 / 5, 51, apdat_.resCom2_.seb[24], 0, 16);
				GameView.AddTouch(g, 1, 193 + frame_ % 20 / 5, 51, apdat_.resCom2_.seb[24], 1, 18);
				num4 = 20;
				num = 63;
				num = 64;
				g.SetColor(SC_WINDOW_BLACK);
				if (select_ == 0)
				{
					g.DrawString("選手のパラメータについて", 120f, num, 2);
				}
				else if (select_ == 1)
				{
					g.DrawString("ポジション評価について", 120f, num, 2);
				}
				if (select_ == 0)
				{
					num2 = 16;
					num = 84;
					for (int k = 0; k < PARAM_EXPLAIN1.Length; k++)
					{
						num3 = num + 24 * k;
						apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[74], apdat_.resCom2_.img[121], k);
						apdat_.DrawSeb(g, num2 + 16, num3, apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], k);
						g.SetColor(SC_WINDOW_BLACK);
						SetFontSize(g, 10);
						if (En())
						{
							g.DrawString(PARAM_EXPLAIN1[k], 66f, num3 + 2 + 1);
						}
						else
						{
							g.DrawString(PARAM_EXPLAIN1[k], 66f, num3 + 2);
						}
						ResetFontSize(g);
						if (k != PARAM_EXPLAIN1.Length - 1)
						{
							g.SetColor(Graphics.GetColorOfRGB(222, 234, 225));
							g.DrawLine(num2 - 3, num3 + 20, 213f, num3 + 20);
						}
					}
				}
				else
				{
					num2 = 57;
					num3 = 88;
					int[] array2 = new int[4] { 9, 4, 1, 0 };
					for (int l = 0; l < 4; l++)
					{
						g.DrawImage(apdat_.resCom2_.img[123], num2 + 31 * l, num3);
						g.SetColor(SC_WINDOW_BLACK);
						int rank = array2[l];
						apdat_.DrawPositionStatus(g, l, rank, num2 + 31 * l + 4, num3 + 4);
					}
					num2 = 19;
					num = 114;
					for (int m = 0; m < 5; m++)
					{
						num3 = num + 16 * m;
						g.SetColor(SC_WINDOW_BLACK);
						if (MyFormBase.Jp())
						{
							g.DrawString2(PARAM_EXPLAIN2[m], num2, num3);
						}
						else if (En())
						{
							g.DrawString2(PARAM_EXPLAIN2[m], 120f, num3, 18);
						}
					}
				}
				num = 202;
				g.SetColor(SC_WINDOW_SELECT);
				g.FillRect(96f, num, 48f, 16f);
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString("閉じる", 120f, num + 2, 2);
				apdat_.resCom2_.seb[60].Draw(g, 92f, num + 9, apdat_.resCom2_.img);
				int num10 = 68;
				GameView.AddTouch(g, 2, 120 - num10 / 2, num + 2 - 10, num10, 36, 131072);
			}
			else if (type_ == 33)
			{
				LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
				int num11 = udat_.league_setu_ + value1_;
				if (page_ == 0 || page_ == 1)
				{
					apdat_.DrawWindow(g, 228, 204, 0, MyFormBase.LT("<0> <1>/3", MyFormBase.LT(leagueData.name2_) + "\t" + (page_ + 1)));
				}
				else
				{
					apdat_.DrawWindow(g, 228, 204, 0, MyFormBase.LT("得点ﾗﾝｷﾝｸﾞ <0>/3", page_ + 1));
				}
				apdat_.DrawBox(g, 15, 55, 225, 201);
				GameView.AddTouch(g, 1, 44 - frame_ % 20 / 5, 37, apdat_.resCom2_.seb[24], 0, 16);
				GameView.AddTouch(g, 1, 193 + frame_ % 20 / 5, 37, apdat_.resCom2_.seb[24], 1, 18);
				num4 = 20;
				num = 63;
				num3 = num - 12;
				if (page_ == 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(152, 152, 152));
					if (num11 + 1 == leagueData.matchEnemy_.Length)
					{
						g.DrawString("終節", 29f, 52f);
					}
					else
					{
						g.DrawString(MyFormBase.LT("<0>試合目", num11 + 1), 29f, 52f);
					}
					apdat_.DrawSeb(g, 142, num3, apdat_.resCom2_.seb[36], apdat_.resCom2_.img[38], 6);
					apdat_.DrawSeb(g, 182, num3, apdat_.resCom2_.seb[36], apdat_.resCom2_.img[38], 0);
					apdat_.DrawSeb(g, 194, num3, apdat_.resCom2_.seb[36], apdat_.resCom2_.img[38], 2);
					apdat_.DrawSeb(g, 206, num3, apdat_.resCom2_.seb[36], apdat_.resCom2_.img[38], 1);
				}
				else if (page_ == 1)
				{
					g.SetColor(Graphics.GetColorOfRGB(152, 152, 152));
					if (num11 + 1 == leagueData.matchEnemy_.Length)
					{
						g.DrawString("終節", 29f, 52f);
					}
					else
					{
						g.DrawString(MyFormBase.LT("<0>試合目", num11 + 1), 29f, 52f);
					}
					SetFontSize(g, 6);
					g.SetColor(SC_WINDOW_BLACK);
					if (En())
					{
						g.DrawString("得点", 137f, num3 + 5, 1);
						g.DrawString("失点", 172f, num3 + 5, 1);
						g.DrawString("差", 205f, num3 + 5, 1);
					}
					else
					{
						g.DrawString("得点", 139f, num3, 1);
						g.DrawString("失点", 170f, num3, 1);
						g.DrawString("差", 205f, num3, 1);
					}
					ResetFontSize(g);
				}
				else if (page_ == 2)
				{
					SetFontSize(g, 8);
					g.SetColor(SC_WINDOW_BLACK);
					if (En())
					{
						g.DrawString("得点者", 68f, num3 + 2);
						g.DrawString("ﾁｰﾑ名", 127f, num3 + 2);
						g.DrawString("得点", 181f, num3 + 2);
					}
					else
					{
						g.DrawString("得点者", 68f, num3);
						g.DrawString("ﾁｰﾑ名", 127f, num3);
						g.DrawString("得点", 190f, num3);
					}
					ResetFontSize(g);
				}
				int num12 = 0;
				int num13 = 23;
				int num14 = 200;
				g.ClipRect(20f, 68f, 201f, 141f);
				if (page_ == 0 || page_ == 1)
				{
					for (int n = scrollValue_; n < udat_.league_dispOlder_.Length; n++)
					{
						int num15 = n - scrollValue_;
						if (num15 > 6)
						{
							break;
						}
						int[] leagueData2 = udat_.GetLeagueData(udat_.league_dispOlder_[n][0]);
						TeamData teamData = apdat_.tData_[leagueData2[8]];
						num2 = num4;
						num3 = 69 + num15 * num13;
						g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
						g.DrawRect(num2, num3, num14, num13);
						if (teamData.id_ == 0)
						{
							g.SetColor(Graphics.GetColorOfRGB(210, 255, 158));
							g.FillRect(num2 + 2, num3 + 3, 197f, 18f);
						}
						if (MyFormBase.Jp())
						{
							g.SetColor(Graphics.GetColorOfRGB(139, 139, 139));
							num12 = 19;
							if (num11 == 0)
							{
								apdat_.DrawSeb(g, num2 + num12 - 8, num3 + 7, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 15);
							}
							else if (n == 0)
							{
								apdat_.DrawNumImage(g, 35, n + 1, num2 + num12, num3 + 7, 0, 4);
							}
							else
							{
								apdat_.DrawNumImage(g, 78, n + 1, num2 + num12, num3 + 7, 0, 4);
							}
							SetFontSize(g, 8);
							if (En())
							{
								g.DrawString("位", num2 + num12 + 2, num3 + 6 + 2);
							}
							else
							{
								g.DrawString("位", num2 + num12 + 2, num3 + 6);
							}
							ResetFontSize(g);
							num12 += 20;
							if (teamData.id_ == 0)
							{
								g.SetColor(Graphics.GetColorOfRGB(67, 72, 255));
							}
							else
							{
								g.SetColor(SC_WINDOW_BLACK);
							}
							if (En())
							{
								g.DrawString(MyFormBase.LT(teamData.name2_), num2 + num12 + 3, num3 + 6);
							}
							else
							{
								g.DrawString(MyFormBase.LT(teamData.name2_), num2 + num12, num3 + 6);
							}
							num12 += 80;
						}
						else
						{
							g.SetColor(Graphics.GetColorOfRGB(139, 139, 139));
							num12 = 19;
							SetFontSize(g, 8);
							if (En())
							{
								g.DrawString("位", num2 + 21 - 16, num3 + 6 + 2);
							}
							else
							{
								g.DrawString("位", num2 + 21 - 16, num3 + 6);
							}
							ResetFontSize(g);
							if (num11 == 0)
							{
								apdat_.DrawSeb(g, num2 + 11 + 21, num3 + 7, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 15);
							}
							else if (n == 0)
							{
								apdat_.DrawNumImage(g, 35, n + 1, num2 + 19 + 21, num3 + 7, 0, 4);
							}
							else
							{
								apdat_.DrawNumImage(g, 78, n + 1, num2 + 19 + 21, num3 + 7, 0, 4);
							}
							num12 += 20;
							if (teamData.id_ == 0)
							{
								g.SetColor(Graphics.GetColorOfRGB(67, 72, 255));
							}
							else
							{
								g.SetColor(SC_WINDOW_BLACK);
							}
							if (En())
							{
								g.DrawString(MyFormBase.LT(teamData.name2_), num2 + 39 + 8, num3 + 6);
							}
							else
							{
								g.DrawString(MyFormBase.LT(teamData.name2_), num2 + 39, num3 + 6);
							}
							num12 += 80;
						}
						g.SetColor(SC_WINDOW_BLACK);
						if (page_ == 0)
						{
							apdat_.DrawNumImage(g, 35, leagueData2[0], num2 + 133, num3 + 7, 0, 2);
							if (teamData.id_ == 0)
							{
								g.SetColor(Graphics.GetColorOfRGB(67, 72, 255));
							}
							else
							{
								g.SetColor(SC_WINDOW_BLACK);
							}
							g.DrawString(leagueData2[1] + "-" + leagueData2[2] + "-" + leagueData2[3], num2 + 196, num3 + 6, 4);
						}
						else
						{
							g.DrawString(string.Empty + leagueData2[4], num2 + 135, num3 + 6, 4);
							g.DrawString(string.Empty + leagueData2[5], num2 + 165, num3 + 6, 4);
							g.DrawString(string.Empty + leagueData2[6], num2 + 195, num3 + 6, 4);
						}
					}
				}
				else if (page_ == 2)
				{
					for (int num16 = scrollValue_; num16 < ary1.Size(); num16++)
					{
						int num17 = num16 - scrollValue_;
						if (num17 > 6)
						{
							break;
						}
						string str = (string)ary1.ElementAt(num16);
						int[] array3 = (int[])ary2.ElementAt(num16);
						TeamData teamData2 = apdat_.tData_[array3[1]];
						num2 = num4;
						num3 = 69 + num17 * num13;
						g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
						g.DrawRect(num2, num3, num14, num13);
						if (teamData2.id_ == 0)
						{
							g.SetColor(Graphics.GetColorOfRGB(210, 255, 158));
							g.FillRect(num2 + 2, num3 + 3, 197f, 18f);
						}
						g.SetColor(SC_WINDOW_BLACK);
						num12 = 24;
						if (MyFormBase.Jp())
						{
							if (num16 == 0)
							{
								apdat_.DrawNumImage(g, 35, num16 + 1, 39, num3 + 7, 0, 4);
							}
							else
							{
								apdat_.DrawNumImage(g, 78, num16 + 1, 39, num3 + 7, 0, 4);
							}
							g.SetColor(Graphics.GetColorOfRGB(139, 139, 139));
							SetFontSize(g, 8);
							if (En())
							{
								g.DrawString("位", num2 + 21, num3 + 6 + 2);
							}
							else
							{
								g.DrawString("位", num2 + 21, num3 + 6);
							}
							ResetFontSize(g);
						}
						else
						{
							g.SetColor(Graphics.GetColorOfRGB(139, 139, 139));
							SetFontSize(g, 8);
							if (En())
							{
								g.DrawString("位", num2 + 21 - 16, num3 + 6 + 2);
							}
							else
							{
								g.DrawString("位", num2 + 21 - 16, num3 + 6);
							}
							ResetFontSize(g);
							if (num16 == 0)
							{
								apdat_.DrawNumImage(g, 35, num16 + 1, 63, num3 + 7, 0, 4);
							}
							else
							{
								apdat_.DrawNumImage(g, 78, num16 + 1, 63, num3 + 7, 0, 4);
							}
						}
						if (teamData2.id_ == 0)
						{
							g.SetColor(Graphics.GetColorOfRGB(92, 51, 31));
						}
						else
						{
							g.SetColor(Graphics.GetColorOfRGB(146, 146, 146));
						}
						if (En())
						{
							g.DrawString(str, num2 + 51 - 3, num3 + 6);
						}
						else
						{
							g.DrawString(str, num2 + 51, num3 + 6);
						}
						num12 += 80;
						if (teamData2.id_ == 0)
						{
							g.SetColor(Graphics.GetColorOfRGB(92, 51, 31));
						}
						else
						{
							g.SetColor(Graphics.GetColorOfRGB(146, 146, 146));
						}
						SetFontSize(g, 10);
						if (En())
						{
							g.DrawString(teamData2.name2_, num2 + 108, num3 + 6 + 1);
						}
						else
						{
							g.DrawString(teamData2.name2_, num2 + 108, num3 + 6);
						}
						ResetFontSize(g);
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString(MyFormBase.LT("<0>点", array3[0]), num2 + 195, num3 + 6, 4);
					}
				}
				g.PopClip();
				num3 = 215;
				if (value1_ == 0)
				{
					int[] nextBattleLeagueIndex = udat_.GetNextBattleLeagueIndex(udat_.league_setu_);
					if (nextBattleLeagueIndex != null)
					{
						apdat_.DrawSeb(g, 24, 226, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 0);
						g.SetColor(Graphics.GetColorOfRGB(68, 58, 234));
						g.DrawString("次の対戦チーム", 34f, num3);
						g.SetColor(SC_WINDOW_BLACK);
						TeamData teamData3 = apdat_.tData_[leagueData.matchEnemy_[nextBattleLeagueIndex[0]]];
						g.DrawString(teamData3.name2_, 176f, num3, 2);
					}
					else
					{
						apdat_.DrawSeb(g, 93, 226, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 0);
						apdat_.DrawSeb(g, 147, 226, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 0);
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString("終節", 120f, num3, 2);
					}
				}
				else
				{
					if (select_ == 0)
					{
						g.SetColor(SC_WINDOW_SELECT);
						g.FillRect(100f, num3 - 2, 40f, 16f);
					}
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString("次へ", 120f, num3, 2);
					apdat_.resCom2_.seb[60].Draw(g, 96f, num3 + 7, apdat_.resCom2_.img);
				}
				if (page_ == 0 || page_ == 1)
				{
					apdat_.DrawVerticalScroll2(g, num4 + 207, num + 3, 3, 146, scrollValue_, udat_.league_dispOlder_.Length - 1, 6);
				}
				else
				{
					apdat_.DrawVerticalScroll2(g, num4 + 207, num + 3, 3, 146, scrollValue_, ary1.Size() - 1, 6);
				}
				GameView.AddTouch(2, TouchOption.Create(2));
			}
			else if (type_ == 34)
			{
				if (ary1.Size() > 1)
				{
					apdat_.DrawWindow(g, 228, 204, 0, MyFormBase.LT("リーグ戦 <0>/<1>", page_ + 1 + "\t" + ary1.Size()));
				}
				else
				{
					apdat_.DrawWindow(g, 228, 204, 0, "リーグ戦");
				}
				apdat_.DrawBox(g, 15, 55, 225, 201);
				if (ary1.Size() > 1)
				{
					GameView.AddTouch(g, 1, 44 - frame_ % 20 / 5, 37, apdat_.resCom2_.seb[24], 0, 16);
					GameView.AddTouch(g, 1, 193 + frame_ % 20 / 5, 37, apdat_.resCom2_.seb[24], 1, 18);
				}
				num4 = 20;
				num = 63;
				LeagueData leagueData3 = (LeagueData)ary1.ElementAt(page_);
				if (scrollValue_ == 0)
				{
					num2 = 20;
					num3 = 47;
					apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[95], apdat_.resCom2_.img[97], leagueData3.trophy_);
					num2 = 40;
					num3 = 51;
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString(leagueData3.name2_, num2, num3);
					num2 = 164;
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("前回", num2, num3);
					g.SetColor(SC_WINDOW_BLACK);
					if (leagueData3.userPriOlder_ == -1)
					{
						g.DrawString("--位", 222f, num3, 4);
					}
					else
					{
						g.DrawString(MyFormBase.LT("<0>位", leagueData3.userPriOlder_), 222f, num3, 4);
					}
				}
				else
				{
					num3 = 51;
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("参戦チーム一覧", 120f, num3, 2);
				}
				if (scrollValue_ == 0)
				{
					apdat_.DrawBox(g, 20, 138, 221, 180);
					num2 = 57;
					if (En())
					{
						num2 += -10;
					}
					num3 = 82;
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("優勝賞金", num2, num3);
					if (MyFormBase.Jp())
					{
						apdat_.DrawNumImageComma_en(g, 35, leagueData3.victoryPrize_, 180, num3 + 1, 0, 0, 4);
						apdat_.DrawSeb(g, 181, num3 + 1, apdat_.resCom2_.seb[35], apdat_.resCom2_.img[103], 13);
					}
					else if (En())
					{
						apdat_.DrawMoney(g, 35, leagueData3.victoryPrize_, 194, num3 + 1, 0, 0, 4);
					}
					num4 = 24;
					num = 108;
					string[] array4 = new string[4] { "難度", "費用", "対戦", "日程" };
					for (int num18 = 0; num18 < 4; num18++)
					{
						num2 = num4 + num18 / 2 * 97;
						num3 = num + num18 % 2 * 19;
						g.SetColor(Graphics.GetColorOfRGB(222, 234, 225));
						g.DrawRect(num2, num3, 96f, 19f);
						g.SetColor(Graphics.GetColorOfRGB(151, 151, 151));
						SetFontSize(g, 10);
						if (En())
						{
							g.DrawString(array4[num18], num2 + 8 - 4, num3 + 5 + 1);
						}
						else
						{
							g.DrawString(array4[num18], num2 + 8, num3 + 5);
						}
						ResetFontSize(g);
						if ((num18 == 0)) { if (En())
							{
								apdat_.DrawSeb(g, num2 + 72 + 3, num3 + 17, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], leagueData3.rank_);
							}
							else
							{
								apdat_.DrawSeb(g, num2 + 62, num3 + 17, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], leagueData3.rank_);
							} } else if ((num18 == 1)) { g.SetColor(SC_WINDOW_BLACK);
g.DrawString(MyFormBase.YEN("万", leagueData3.joinCost_), num2 + 93, num3 + 5, 4); } else if ((num18 == 2)) { SetFontSize(g, 6);
g.SetColor(SC_WINDOW_BLACK);
if (En())
							{
								g.DrawString("全", num2 + 38 - 5, num3 + 4 + 6);
							}
							else
							{
								g.DrawString("全", num2 + 38, num3 + 4);
								g.DrawString("試合", num2 + 66, num3 + 4);
							}
ResetFontSize(g);
if (En())
							{
								apdat_.DrawNumImage(g, 35, leagueData3.matchEnemy_.Length - 1, num2 + 64 - 5 + 28, num3 + 5, 0, 4);
							}
							else
							{
								apdat_.DrawNumImage(g, 35, leagueData3.matchEnemy_.Length - 1, num2 + 64, num3 + 5, 0, 4);
							} } else if ((num18 == 3)) { SetFontSize(g, 10);
g.SetColor(SC_WINDOW_BLACK);
if (En())
							{
								g.DrawString("週後", num2 + 68 - 12 - 6, num3 + 5 + 6 - 4);
								apdat_.DrawNumImage(g, 35, leagueData3.schedule_, num2 + 66 + 6, num3 + 6, 0, 4);
							}
							else
							{
								g.DrawString("週後", num2 + 68, num3 + 5);
								apdat_.DrawNumImage(g, 35, leagueData3.schedule_, num2 + 66, num3 + 6, 0, 4);
							}
ResetFontSize(g); }
					}
					if (En())
					{
						textLay_.SetAnchor(2);
					}
					textLay_.Draw(g, 24f, 155f, 0);
					Draw_chara(g, 172, 0, 0, frame_, 32, 209);
					Draw_chara(g, 171, 0, 1, frame_, 209, 209);
					num3 = 195;
					if (leagueData3.trophy_ == 4)
					{
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString("リーグ制覇済み", 120f, num3, 2);
					}
					else
					{
						g.SetColor(SC_WINDOW_GRAY);
						g.DrawString("リーグ未制覇", 120f, num3, 2);
					}
				}
				else
				{
					num4 = 19;
					num = 74;
					int num19 = 0;
					for (int num20 = 0; num20 < 16; num20++)
					{
						num2 = num4 + num20 % 2 * 101;
						num3 = num + num20 / 2 * 16;
						g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
						g.DrawRect(num2, num3, 74f, 16f);
						g.DrawRect(num2 + 75, num3, 26f, 16f);
						if (num20 >= leagueData3.matchEnemy_.Length)
						{
							continue;
						}
						TeamData teamData4 = null;
						if (num19 < leagueData3.matchEnemy_.Length)
						{
							if (leagueData3.matchEnemy_[num19] == 0)
							{
								num19++;
							}
							if (num19 < leagueData3.matchEnemy_.Length)
							{
								teamData4 = apdat_.tData_[leagueData3.matchEnemy_[num19]];
							}
							num19++;
						}
						if (teamData4 == null) { continue; }
g.SetColor(SC_WINDOW_BLACK);
string str2 = MyFormBase.LT(teamData4.name2_);
g.DrawString(str2, num2 + 8, num3 + 3);
apdat_.DrawNumImage(g, 35, udat_.GetRankIndex(teamData4.warPotential_) + 1, num2 + 89, num3 + 4, 0, 2);
					}
					g.DrawImage(apdat_.resCom2_.img[57], 100f, 65f);
					g.DrawImage(apdat_.resCom2_.img[57], 201f, 65f);
				}
				num = 214;
				g.SetColor(SC_WINDOW_SELECT);
				g.FillRect(90f, num, 60f, 16f);
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString("参加する", 120f, num + 2, 2);
				apdat_.resCom2_.seb[60].Draw(g, 86f, num + 9, apdat_.resCom2_.img);
				apdat_.DrawVerticalScroll2(g, 228, 66, 3, 147, select_, 1, 1);
				GameView.AddTouch(2, TouchOption.Create(2));
			}
			else if (type_ == 35)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "リーグ戦");
				apdat_.DrawBox(g, 12, 136, 227, 205);
				LeagueData leagueData4 = leagueData_;
				g.DrawImage(apdat_.resCom2_.img[162], 30f, 64f);
				num = 129;
				for (int num21 = 0; num21 < 3; num21++)
				{
					PlayerData playerData2 = (PlayerData)udat_.memberPDataist_.ElementAt(num21);
					udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData2.faceImg_, frame_ % 20 / 5, 1);
					udat_.DrawDispPlayer(g, 100 + num21 * 20, num);
				}
				Draw_chara(g, 172, 0, 0, frame_, 44, 140);
				Draw_chara(g, 171, 0, 1, frame_, 196, 140);
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(MyFormBase.LT("<0>に参戦しますか", leagueData4.name2_), 120f, 164f, 2);
				num3 = 195;
				num = 195;
				int num22 = 40;
				if (select_ == 0)
				{
					Draw_btmMsg(g, "参戦する", 120 - num22, num, true, true);
					Draw_btmMsg(g, "考え直す", 120 + num22, num, false, false);
				}
				else if (select_ == 1)
				{
					Draw_btmMsg(g, "参戦する", 120 - num22, num, false, false);
					Draw_btmMsg(g, "考え直す", 120 + num22, num, true, true);
				}
				num22 = 58;
				for (int num23 = 0; num23 < 2; num23++)
				{
					num2 = 75 - num22 / 2 + 90 * num23;
					GameView.AddTouch(g, 3, num2 + DLGSEL_TRECT[num23][0], num3 - 2 + DLGSEL_TRECT[num23][1], num22 + DLGSEL_TRECT[num23][2], 16 + DLGSEL_TRECT[num23][3], 0x20000 | num23);
				}
			}
			else if (type_ == 36) { } else {
				_draw3(g);
			}
		}

		private void _draw3(Graphics g)
		{
int coalescedValue, num, num2, num3;
coalescedValue = num = num2 = num3 = 0;




			Font font = g.GetFont(null);
			if (type_ == 37)
			{
				Draw_backBlackAlpha(g);
				LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
				g.SetColor(Graphics.GetColorOfRGB(162, 204, 153, 128));
				g.FillRect(5f, 34f, 231f, 199f);
				g.SetColor(Graphics.GetColorOfRGB(106, 129, 78, 128));
				g.DrawRect(7f, 36f, 226f, 194f);
				g.SetColor(Graphics.GetColorOfRGB(108, 158, 107, 128));
				g.FillRect(8f, 37f, 225f, 193f);
				g.SetColor(Graphics.GetColorOfRGB(178, 236, 179, 128));
				g.FillRect(10f, 94f, 220f, 80f);
				for (int i = 0; i < 3; i++)
				{
					udat_.gm_.Draw_timeMessageWindow(g, 40 + 17 * i, 222, i % 2, 120);
				}
				for (int j = 0; j < 3; j++)
				{
					udat_.gm_.Draw_timeMessageWindow(g, 176 + 17 * j, 222, 1 - j % 2, 120);
				}
				TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[0]];
				TeamData teamData2 = apdat_.tData_[udat_.gm_.teamId_[1]];
				int num4 = 0;
				if ((udat_.gm_.score_[0] > udat_.gm_.score_[1])) { num4 = (0); } else { if ((udat_.gm_.score_[0] < udat_.gm_.score_[1])) { num4 = 1; } else { num4 = 2; } }
				num3 = 43;
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				g.DrawString("結果", 120f, num3, 2);
				num3 += 17;
				g.DrawString(teamData.name_, 60f, num3, 2);
				g.DrawString(teamData2.name_, 180f, num3, 2);
				num3 += 17;
				apdat_.DrawNumImage(g, 30, udat_.gm_.score_[0], 60, num3, 0, 2);
				apdat_.DrawNumImage(g, 30, udat_.gm_.score_[1], 180, num3, 0, 2);
				apdat_.DrawSeb(g, 116, num3, apdat_.resCom2_.seb[35], apdat_.resCom2_.img[103], 15);
				num = 98;
				int matchPrice = apdat_.frmSoccer_.GetMatchPrice();
				if (num4 == 0)
				{
					SetWinCharaJamp(frame_ - ANIME_MATCH_RESULT[0]);
				}
				apdat_.DrawSeb(g, 68, num - 1, apdat_.resCom2_.seb[36], apdat_.resCom2_.img[38], 7);
				if (MyFormBase.Jp())
				{
					apdat_.DrawSeb(g, 144, num - 1, apdat_.resCom2_.seb[36], apdat_.resCom2_.img[38], 8);
				}
				if (num4 == 0)
				{
					if (frame_ >= ANIME_MATCH_RESULT[0])
					{
						int winCharaJamp = AppData.ClampMax(frame_ - ANIME_MATCH_RESULT[0], 17);
						SetWinCharaJamp(winCharaJamp);
						if (!En())
						{
							apdat_.DrawNumImageComma(g, 35, matchPrice, 142, num + WIN_CHA[1], 0, 0, 4);
							apdat_.DrawSeb(g, 144, num - 1, apdat_.resCom2_.seb[36], apdat_.resCom2_.img[38], 8);
						}
						else
						{
							apdat_.DrawMoney(g, 35, matchPrice, 168, num + WIN_CHA[1], 0, 0, 4);
						}
					}
				}
				else
				{
					g.ClipRect(0f, 98f, 240f, 12f);
					int num5 = GameUtil.RateConvert(frame_, ANIME_MATCH_RESULT[0], ANIME_MATCH_RESULT[1], -12, 0, true);
					if (!En())
					{
						apdat_.DrawNumImageComma(g, 35, matchPrice, 142, num + num5, 0, 0, 4);
						apdat_.DrawSeb(g, 144, num - 1, apdat_.resCom2_.seb[36], apdat_.resCom2_.img[38], 8);
					}
					else
					{
						apdat_.DrawMoney(g, 35, matchPrice, 168, num + num5, 0, 0, 4);
					}
					g.PopClip();
				}
				int num6 = frame_ % 10 / 5;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				for (int k = 0; k < 6; k++)
				{
					int num10 = k / 3;
					num8 = 0;
					num9 = 0;
					if (frame_ < ANIME_MATCH_RESULT[1])
					{
						num6 = 0;
						num7 = 0;
					}
					else if (num4 == num10)
					{
						SetWinCharaJamp(frame_ - ANIME_MATCH_RESULT[1]);
						num7 = WIN_CHA[0];
						num8 = WIN_CHA[1];
						num6 = 0;
					}
					else if (num4 == 1 - num10)
					{
						num6 = 0;
						num7 = 11;
						num9 = -1;
					}
					else
					{
						num6 = 0;
						num7 = 0;
					}
					if (num10 == 0)
					{
						apdat_.DrawSeb(g, 44 + k * 18 - 1, 163 + num9, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 4);
						udat_.SetDispPlayerData(num7, teamData.bodyImg_, teamData.memberFaceId_[k], num6, 1);
						udat_.DrawDispPlayer(g, 44 + k * 18, 162 + num8);
					}
					else
					{
						apdat_.DrawSeb(g, 162 + (k - 3) * 18, 163 + num9, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 4);
						udat_.SetDispPlayerData(num7, teamData2.bodyImg_, teamData2.memberFaceId_[k], num6, 2);
						udat_.DrawDispPlayer(g, 162 + (k - 3) * 18, 162 + num8);
					}
				}
				num3 = 179;
				SetFontSize(g, 10);
				int testedValue = udat_.GetMatchInfoJoin();
if (testedValue == 0)
				{
					int[] nextBattleLeagueIndex = udat_.GetNextBattleLeagueIndex(udat_.league_setu_);
					int[] leagueData2 = udat_.GetLeagueData(0);
					int[] leagueData3 = udat_.GetLeagueData(leagueData.matchEnemy_[nextBattleLeagueIndex[0]]);
					if (leagueData2 == null || leagueData3 == null)
					{
						return;
					}
					num = 174;
					coalescedValue = 4;
					if (MyFormBase.Jp())
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾘｰｸﾞ", coalescedValue + 14, num + 5);
						apdat_.DrawNumImage(g, 30, leagueData2[7], coalescedValue + 85, num + 5, 0, 4);
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						g.DrawString("位", coalescedValue + 90, num + 5);
					}
					else
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾘｰｸﾞ", coalescedValue + 14 + 3, num + 5 + 1);
						int num12 = 85;
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						g.DrawString("位", coalescedValue + num12 - 30 + 3, num + 5 + 1);
						apdat_.DrawNumImage(g, 30, leagueData2[7], coalescedValue + num12 + 9 + 1, num + 5, 0, 2);
					}
					coalescedValue = 129;
					if (MyFormBase.Jp())
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾘｰｸﾞ", coalescedValue + 7, num + 5);
						apdat_.DrawNumImage(g, 30, leagueData3[7], coalescedValue + 78, num + 5, 0, 4);
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						g.DrawString("位", coalescedValue + 83, num + 5);
					}
					else
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾘｰｸﾞ", coalescedValue + 7 + 3, num + 5 + 1);
						int num13 = 78;
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						g.DrawString("位", coalescedValue + num13 - 30 + 3, num + 5 + 1);
						apdat_.DrawNumImage(g, 30, leagueData3[7], coalescedValue + num13 + 9 + 1, num + 5, 0, 2);
					}
				}
				else
				{
					num = 174;
					coalescedValue = 4;
					if (MyFormBase.Jp())
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾁｰﾑ評価", coalescedValue + 14, num + 5);
						apdat_.DrawNumImage(g, 30, teamData.warPotential_, coalescedValue + 93, num + 5, 0, 4);
						coalescedValue = 129;
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾁｰﾑ評価", coalescedValue + 7, num + 5);
						apdat_.DrawNumImage(g, 30, teamData2.warPotential_, coalescedValue + 78, num + 5, 0, 4);
					}
					else
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾁｰﾑ評価", coalescedValue + 14, num + 5 + 2);
						apdat_.DrawNumImage(g, 30, teamData.warPotential_, coalescedValue + 93 + 6, num + 5, 0, 4);
						coalescedValue = 129;
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾁｰﾑ評価", coalescedValue + 7, num + 5 + 2);
						apdat_.DrawNumImage(g, 30, teamData2.warPotential_, coalescedValue + 78 + 12, num + 5, 0, 4);
					}
				}
				num3 = 196;
				num2 = 106;
				if (En())
				{
					num2 = 110;
				}
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				g.DrawString("観客", num2 - 90, num3);
				apdat_.DrawNumImageComma(g, 35, apdat_.frmSoccer_.customerNum[0], num2 - 12 - 2, num3 + 1, 0, 0, 4);
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
				if (MyFormBase.Jp())
				{
					g.DrawString("人", num2 - 12, num3);
				}
				num2 = 224;
				if (En())
				{
					num2 = 228;
				}
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				g.DrawString("観客", num2 - 90, num3);
				apdat_.DrawNumImageComma(g, 35, apdat_.frmSoccer_.customerNum[1], num2 - 12 - 2, num3 + 1, 0, 0, 4);
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
				if (MyFormBase.Jp())
				{
					g.DrawString("人", num2 - 12, num3);
				}
				ResetFontSize(g);
				num = 206;
				g.DrawImage(apdat_.resCom2_.img[172], 21f, num, 0, 0, 16, 21);
				if (frame_ >= ANIME_MATCH_RESULT[1])
				{
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
					if (num4 == 0) {
g.DrawString2("我々の勝利です!!", 120f, num + 7, 2);
} else if (num4 == 1) {
g.DrawString2("惜しくも負けてしまいました", 120f, num + 7, 2);
} else {
g.DrawString2("引き分けでした", 120f, num + 7, 2);
}
coalescedSwitchExit114919: ;
				}
				GameView.AddTouch(2);
			}
			else if (type_ == 38)
			{
				g.SetOrigin(0, 0);
				GameView instance = GameView.GetInstance();
				int gameWidth = instance.GetGameWidth();
				int gameHeight = instance.GetGameHeight();
				int num14 = GameUtil.RateConvert(frame_, 0, VICTORY_ANIME_TIME[0], VICTORY_ANIME_TIME[0] * 3, 0, true);
				g.SetColor(Graphics.GetColorOfRGB(23, 32, 58));
				g.FillRect(0f, 0f, gameWidth, gameHeight);
				int num15 = 2;
				int num16 = 0;
				int r;
int num17;
int num18;

				r = num17 = num18 = 0;
				int num19 = 0;
				while (num16 < gameHeight - 100)
				{
					r = GameUtil.RateConvert(num16, 0, 140, 23, 75, true);
					num17 = GameUtil.RateConvert(num16, 0, 140, 32, 123, true);
					num18 = GameUtil.RateConvert(num16, 0, 140, 58, 168, true);
					g.SetColor(Graphics.GetColorOfRGB(r, num17, num18));
					g.FillRect(0f, num16 + num14, gameWidth, num15);
					num16 += num15;
					if (num19 >= 300)
					{
						break;
					}
				}
				if (gameWidth > 240)
				{
					g.DrawImage(apdat_.resCom2_.img[84], 0f, gameHeight - 130 + num14);
					g.DrawImage(apdat_.resCom2_.img[84], gameWidth - 240, gameHeight - 130 + num14);
				}
				g.DrawImage(apdat_.resCom2_.img[84], gameWidth / 2 - 120, gameHeight - 130 + num14);
				g.DrawImage(apdat_.resCom2_.img[192], gameWidth / 2 - 120 - 40, gameHeight - 44 + num14);
				g.DrawImage(apdat_.resCom2_.img[192], gameWidth / 2 + 120 - 40, gameHeight - 44 + num14);
				g.DrawImage(apdat_.resCom2_.img[191], gameWidth / 2 - 96, gameHeight - 31 + num14);
				int num20 = gameWidth / 2 - 120;
				int num21 = gameHeight - 240;
				int num22 = 14;
				int num23 = 10;
				int num24 = 0;
				int num25 = 0;
				int num26 = 0;
				for (int l = 0; l < zsort_mhobj.Length; l++)
				{
					int num27 = zsort_mhobj[l][0];
					coalescedValue = (int)w_vic_pos[num27].x_ + num20;
					num = (int)w_vic_pos[num27].z_ + num14 + num21;
					num24 = num;
					num25 = 0;
					num26 = 15;
					if (frame_ >= VICTORY_ANIME_TIME[2])
					{
						int num28 = frame_ - VICTORY_ANIME_TIME[2];
						int num29 = (num28 - w_vic_data[num27][1]) % (num22 + num23);
						num29 -= num23 / 2;
						if (num29 >= 0)
						{
							float num30 = GameUtil.SetVStop(-6f, num22 / 2);
							float num31 = GameUtil.SetAStop(-6f, num22 / 2);
							num24 = (int)((float)num + num30 * (float)num29 + num31 * (float)num29 * (float)(num29 + 1) / 2f);
							if (num24 > num)
							{
								num24 = num;
							}
							if ((num24 < num)) { num25 = 1; } else { num25 = 0; }
						}
					}
					apdat_.DrawSeb(g, coalescedValue, num, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 4);
					PlayerData playerData = apdat_.pData_[w_vic_data[num27][0]];
					udat_.SetDispPlayerData(num26, udat_.selectUniform_, playerData.faceImg_, num25, 1);
					udat_.DrawDispPlayer(g, coalescedValue, num24);
				}
				if (frame_ >= VICTORY_ANIME_TIME[2])
				{
					coalescedValue = AppData.ClampMin((gameWidth - 360) / 2, 0);
					num = 0;
					int frame = frame_ % 20 / 10;
					apdat_.DrawSeb(g, coalescedValue, num + num14, apdat_.resCom2_.seb[72], apdat_.resCom2_.img[88], frame);
					apdat_.DrawSeb(g, coalescedValue + 180, num + num14, apdat_.resCom2_.seb[72], apdat_.resCom2_.img[88], frame);
					apdat_.DrawSeb(g, coalescedValue, num + num14 + 40, apdat_.resCom2_.seb[72], apdat_.resCom2_.img[88], frame);
					apdat_.DrawSeb(g, coalescedValue + 180, num + num14 + 40, apdat_.resCom2_.seb[72], apdat_.resCom2_.img[88], frame);
					apdat_.DrawSeb(g, coalescedValue, num + num14 + 80, apdat_.resCom2_.seb[72], apdat_.resCom2_.img[88], frame);
					apdat_.DrawSeb(g, coalescedValue + 180, num + num14 + 80, apdat_.resCom2_.seb[72], apdat_.resCom2_.img[88], frame);
				}
				if (frame_ >= VICTORY_ANIME_TIME[2])
				{
					for (int m = 0; m < fw_zsort.Length; m++)
					{
						fw_zsort[m][0] = m;
						fw_zsort[m][1] = fw_cnt[m];
					}
					for (int n = 0; n < fw_zsort.Length - 1; n++)
					{
						for (int num32 = fw_zsort.Length - 1; num32 > n; num32--)
						{
							if (fw_zsort[n][1] < fw_zsort[num32][1])
							{
								int num33 = fw_zsort[num32][1];
								fw_zsort[num32][1] = fw_zsort[n][1];
								fw_zsort[n][1] = num33;
								num33 = fw_zsort[num32][0];
								fw_zsort[num32][0] = fw_zsort[n][0];
								fw_zsort[n][0] = num33;
							}
						}
					}
					int num34 = gameWidth / 2 - 120;
					g.SetRenderMode(1, 255, 255);
					for (int num35 = 0; num35 < fw_zsort.Length; num35++)
					{
						if (fw_zsort[num35][1] == 0) { continue; }
int num36 = fw_zsort[num35][0];
int num37 = FW_ANIME[3] - fw_cnt[num36];
int num38 = 0;
if ((num37 < FW_ANIME[0])) { num38 = (0); } else { if ((num37 < FW_ANIME[1])) { num38 = 1; } else { if ((num37 < FW_ANIME[2])) { num38 = (2); } else { num38 = (3); } } }
apdat_.DrawSeb(g, fw_pos[num36][0] + num34, fw_pos[num36][1] + num14, apdat_.resCom2_.seb[69], apdat_.resCom2_.img[82], num38);
					}
					g.SetRenderMode(0, 255, 0);
				}
				if (frame_ >= VICTORY_ANIME_TIME[1])
				{
					if (En())
					{
						g.DrawImage(apdat_.resCom2_.img[85], gameWidth / 2 - 109, gameHeight - 169 + 10 + num14);
					}
					else
					{
						g.DrawImage(apdat_.resCom2_.img[85], gameWidth / 2 - 109, gameHeight - 169 + num14);
					}
					TeamData teamData3 = apdat_.tData_[0];
					g.SetColor(SC_WINDOW_WHITE);
					if (En())
					{
						g.DrawString(teamData3.name_, gameWidth / 2, gameHeight - 169 + 10 + 7 + num14, 2);
					}
					else
					{
						g.DrawString(teamData3.name_, gameWidth / 2, gameHeight - 169 + 7 + num14, 2);
					}
					if (MyFormBase.Jp())
					{
						g.DrawImage(apdat_.resCom2_.img[87], gameWidth / 2 - 48, gameHeight - 223 + num14);
					}
					else if (En())
					{
						g.DrawImage(apdat_.resCom2_.img[87], gameWidth / 2 - 94, gameHeight - 223 - 7 + num14);
					}
					num = gameHeight - 25;
					g.SetColor(SC_WINDOW_BLACK);
					int switchValue = udat_.GetMatchInfoJoin();
if (switchValue == 0) {
LeagueData leagueData4 = apdat_.leagueData_[udat_.selectLeagueId_];
g.DrawString(leagueData4.name_, gameWidth / 2, num, 2);
} else if (switchValue == 1) {
QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
g.DrawString(questMatchData.name_, gameWidth / 2, num, 2);
}
coalescedSwitchExit121148: ;
				}
				GameView.AddTouch(2);
			}
			else if (type_ == 39)
			{
				apdat_.DrawWindow(g, 228, 204, 0, "地域の活動");
				num = 49;
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("活動の内容", 25f, num);
				g.DrawString("ﾗﾝｸ", 131f, num);
				g.DrawString("費用", 187f, num);
				num = 53;
				apdat_.DrawBox(g, 15, num, 224, num + 92);
				g.SetColor(SC_WINDOW_BLACK);
				num2 = 24;
				for (int num39 = scrollValue_; num39 < ary1.Size(); num39++)
				{
					int num40 = num39 - scrollValue_;
					if (num40 >= 5)
					{
						break;
					}
					FanActData fanActData = (FanActData)ary1.ElementAt(num39);
					num3 = num + 15 + num40 * 17;
					if (num39 == select_)
					{
						g.SetColor(SC_WINDOW_SELECT);
						g.FillRect(num2 + 1, num3, 194f, 16f);
					}
					GameView.AddTouch(g, 11, num2 + 1, num3, 194, 16, 0x20000 | num39, TouchOption.Create(16).Margin(30, 0, 0, 0));
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(fanActData.name_, num2 + 3, num3 + 2);
					if (fanActData.state_ == 2)
					{
						int num41 = 0;
						if (En())
						{
							num41 = 3;
						}
						g.SetColor(SC_WINDOW_BLACK);
						SetFontSize(g, 8);
						if (En())
						{
							g.DrawString("活動中", 139 - num41, num3 + 2 + 2);
						}
						else
						{
							g.DrawString("活動中", 139 - num41, num3 + 2);
						}
						ResetFontSize(g);
						int num42 = GameUtil.RateConvert(fanActData.stateCnt_, 0, 200, 0, 100, true);
						apdat_.DrawNumImage(g, 35, num42, num2 + 175 + num41, num3 + 3, 0, 4);
						apdat_.DrawSeb(g, num2 + 175 + 1 + num41, num3 + 3, apdat_.resCom2_.seb[90], apdat_.resCom2_.img[131], 1);
					}
					else if (fanActData.type_ == 0)
					{
						apdat_.DrawSeb(g, num2 + 114 + 2, num3 + 14, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], 9);
					}
					else
					{
						apdat_.DrawSeb(g, num2 + 114 + 2, num3 + 14, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], fanActData.rank_);
					}
					if (fanActData.state_ == 2) { } else {
						int num43 = GameUtil.RateConvert(fanActData.rank_, 0, 6, fanActData.cost_[0], fanActData.cost_[1], true);
						apdat_.DrawSeb(g, num2 + 150, num3 + 1, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], fanActData.costType_);
						if (udat_.ChResearchPoin(fanActData.costType_, num43))
						{
							apdat_.DrawNumImage(g, 35, num43, num2 + 192, num3 + 3, 0, 4);
						}
						else
						{
							apdat_.DrawNumImage(g, 91, num43, num2 + 192, num3 + 3, 0, 4);
						}
					}
					if (fanActData.IsNew())
					{
						Draw_newMark(g, num2 + 79, num3 + 2, frame_);
					}
					if (num39 == select_)
					{
						apdat_.resCom2_.seb[60].Draw(g, num2 - 6, num3 + 9, apdat_.resCom2_.img);
					}
				}
				apdat_.DrawVerticalScroll2(g, 226, 64, 3, 93, scrollValue_, ary1.Size() - 1, 5);
				FanActData fanActData2 = (FanActData)ary1.ElementAt(select_);
				num = 151;
				apdat_.DrawBox(g, 15, num, 224, num + 63);
				Draw_fanActBack(g, fanActData2.icon_, 18, 166);
				coalescedValue = 59;
				num = 165;
				int num44 = 0;
				for (int num45 = 0; num45 < 3; num45++)
				{
					num2 = coalescedValue;
					num3 = num + 20 * num45;
					g.SetColor(Graphics.GetColorOfRGB(207, 245, 229));
					g.FillRect(num2, num3, 106f, 18f);
					g.SetColor(Graphics.GetColorOfRGB(232, 249, 227));
					g.FillRect(num2 + 107, num3, 54f, 18f);
					num2 = coalescedValue;
					g.SetColor(SC_WINDOW_BLUE);
					SetFontSize(g, 10);
					if (En())
					{
						g.DrawString(FanActData.element_name[num45], num2 + 6, num3 + 4 + 1);
					}
					else
					{
						g.DrawString(FanActData.element_name[num45], num2 + 6, num3 + 4);
					}
					ResetFontSize(g);
					int fanEva = FanActData.GetFanEva(num45);
					int fanDispParam = FanActData.GetFanDispParam(fanEva, num45);
					if (num45 == 0) {
apdat_.DrawNumImageComma(g, 78, fanDispParam, num2 + 88, num3 + 5, 0, 0, 4);
apdat_.DrawSeb(g, num2 + 90, num3 + 6, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 18);
} else if (num45 == 1) {
int num46 = 52;
int num47 = 0;
num47 = GameUtil.RateConvert(fanDispParam, 0, 100, 0, num46 - 2, true);
g.DrawImage(apdat_.resCom2_.img[129], num2 + 48, num3 + 4, 0, 0, 52, 10);
g.DrawImage(apdat_.resCom2_.img[129], num2 + 48 + 1, num3 + 5, 1, 10, num47, 8);
} else if (num45 == 2) {
apdat_.DrawNumImage(g, 78, fanDispParam, num2 + 88, num3 + 5, 0, 4);
}
coalescedSwitchExit125387: ;
					int eva = GameUtil.RateConvert(fanActData2.rank_, 0, 6, fanActData2.eff_[num45][0], fanActData2.eff_[num45][1], true);
					int fanAddParamNum = FanActData.GetFanAddParamNum(eva, num45);
					Draw_up(g, num2 + 122, num3 + 4, fanAddParamNum, 1);
				}
				if (udArrowCnt > 0)
				{
					udArrowCnt--;
				}
				coalescedValue = 120;
				num = 70;
				if (udArrowCnt == 0) {
					GameView.AddTouch(g, 2, coalescedValue - 25 - 5, num - 50, 70, 30, 1376456, TouchOption.Create(2));
					GameView.AddTouch(g, 2, coalescedValue - 25 - 5, num + 97, 70, 30, 1376456, TouchOption.Create(2));

} else {
				TouchOption touchOption = TouchOption.Create(2);
				TouchOption touchOption2 = TouchOption.Create(2);
				GameView.AddTouch(g, 24, coalescedValue, num - 13, apdat_.resCom2_.seb[24], 0, 17, touchOption.Margin(40, 40, 45, 0));
				GameView.AddTouch(g, 24, coalescedValue, num + 61 + 13, apdat_.resCom2_.seb[24], 1, 19, touchOption2.Margin(40, 40, -15, 60));
}
			}
			else if (type_ == 40)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "地域の活動");
				apdat_.DrawBox(g, 83, 52, 227, 134);
				apdat_.DrawBox(g, 12, 136, 227, 205);
				FanActData fanActData3 = fanActData_;
				coalescedValue = 30;
				num = 74;
				Draw_fanActBack(g, fanActData3.icon_, coalescedValue, num);
				if (fanActData3.type_ == 0)
				{
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(fanActData3.name_, 155f, 71f, 2);
				}
				else
				{
					apdat_.DrawSeb(g, 116, 83, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], fanActData3.rank_);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(fanActData3.name_, 132f, 71f);
				}
				textLay_.Draw(g, 95f, 88f, 0);
				coalescedValue = 15;
				num = 150;
				for (int num48 = 0; num48 < 3; num48++)
				{
					num2 = coalescedValue + 70 * num48;
					num3 = num;
					g.SetColor(Graphics.GetColorOfRGB(207, 245, 229));
					g.FillRect(num2, num3, 42f, 17f);
					g.SetColor(Graphics.GetColorOfRGB(232, 249, 227));
					g.FillRect(num2 + 42, num3, 28f, 17f);
					g.SetColor(SC_WINDOW_BLUE);
					SetFontSize(g, 10);
					if (En())
					{
						g.DrawString(FanActData.element_name[num48], num2 + 3, num3 + 2 + 1);
					}
					else
					{
						g.DrawString(FanActData.element_name[num48], num2 + 3, num3 + 2);
					}
					ResetFontSize(g);
					int eva2 = GameUtil.RateConvert(fanActData3.rank_, 0, 6, fanActData3.eff_[num48][0], fanActData3.eff_[num48][1], true);
					int fanAddParamNum2 = FanActData.GetFanAddParamNum(eva2, num48);
					Draw_up(g, num2 + 43, num3 + 3, fanAddParamNum2, 1);
				}
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString2("<co=4B8FFF>費用</co>", 80f, 175f, 1);
				num2 = -25;
				num3 = 173;
				int num49 = GameUtil.RateConvert(fanActData3.rank_, 0, 6, fanActData3.cost_[0], fanActData3.cost_[1], true);
				apdat_.DrawSeb(g, num2 + 150, num3, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], fanActData3.costType_);
				if (udat_.ChResearchPoin(fanActData3.costType_, num49))
				{
					apdat_.DrawNumImage(g, 35, num49, num2 + 169, num3 + 3, 0, 1);
				}
				else
				{
					apdat_.DrawNumImage(g, 91, num49, num2 + 169, num3 + 3, 0, 1);
				}
				num = 192;
				int num50 = 40;
				if (select_ == 0)
				{
					Draw_btmMsg(g, "いざ実行", 120 - num50, num, true, true);
					Draw_btmMsg(g, "考え直す", 120 + num50, num, false, false);
				}
				else if (select_ == 1)
				{
					Draw_btmMsg(g, "いざ実行", 120 - num50, num, false, false);
					Draw_btmMsg(g, "考え直す", 120 + num50, num, true, true);
				}
				num50 = 58;
				for (int num51 = 0; num51 < 2; num51++)
				{
					num2 = 75 - num50 / 2 + 90 * num51;
					GameView.AddTouch(g, 3, num2 + DLGSEL_TRECT[num51][0], num3 - 2 + DLGSEL_TRECT[num51][1], num50 + DLGSEL_TRECT[num51][2], 16 + DLGSEL_TRECT[num51][3], 0x20000 | num51);
				}
			}
			else if (type_ == 41)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "地域の活動");
				apdat_.DrawBox(g, 12, 136, 227, 205);
				FanActData fanActData4 = fanActData_;
				coalescedValue = 30;
				num = 64;
				g.DrawImage(apdat_.resEvent_.img[10], coalescedValue, num);
				coalescedValue = 85;
				num = 70;
				g.DrawImage(apdat_.resEvent_.img[8], coalescedValue, num);
				coalescedValue = 100;
				num = 78;
				Draw_fanActBack(g, fanActData4.icon_, coalescedValue, num);
				for (int num52 = 0; num52 < 6; num52++)
				{
					PlayerData playerData2 = (PlayerData)ary1.ElementAt(num52);
					num2 = 41 + 17 * num52;
					if (num52 / 3 == 1)
					{
						num2 = 165 + 17 * (num52 - 3);
					}
					num3 = 142;
					udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData2.faceImg_, 0, 1 + num52 / 3);
					udat_.DrawDispPlayer(g, num2, num3);
				}
				if (frame_ < FA2_ANIME_TIME[3])
				{
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.LT("<0> の", fanActData4.name_), 120f, 163f, 2);
					num = 186;
					int num53 = GameUtil.RateConvert(frame_, FA2_ANIME_TIME[2], FA2_ANIME_TIME[3] - 30, 0, 12, true);
					g.ClipRect(0f, num, 240f, num53);
					g.SetColor(Graphics.GetColorOfRGB(0, 84, 255));
					g.DrawString("活動完了！", 120f, num, 2);
					g.PopClip();
				}
				else
				{
					coalescedValue = 35;
					num = 148;
					for (int num54 = 0; num54 < FanActData.fa_id.Length; num54++)
					{
						num2 = coalescedValue + 7;
						num3 = num + 16 * num54;
						int num55 = FanActData.fa_id[num54];
						int[] array = GetMeterParam(num54);
						g.SetColor(SC_WINDOW_BLUE);
						SetFontSize(g, 10);
						if (En())
						{
							g.DrawString(FanActData.element_name[num55], num2 + 6 - 21 - 3, num3 + 4 + 1);
						}
						else
						{
							g.DrawString(FanActData.element_name[num55], num2 + 6 - 21, num3 + 4);
						}
						ResetFontSize(g);
						int fanEva2 = FanActData.GetFanEva(num55);
						int fanDispParam2 = FanActData.GetFanDispParam(array[1], num55);
						if (num55 == 3) {
apdat_.DrawSeb(g, num2 + 37 + 93, num3 + 16, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], array[3]);
int num57 = 0;
num57 = GameUtil.RateConvert(array[1], 0, 100, 0, 64, true);
if (array[3] == 6)
							{
								num57 = 0;
							}
g.SetColor(Graphics.GetColorOfRGB(160, 160, 160));
g.DrawRect(num2 + 56, num3 + 5, 65f, 9f);
g.SetColor(Graphics.GetColorOfRGB(0, 254, 0));
g.FillRect(num2 + 56 + 1, num3 + 5 + 1, num57, 8f);
g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
g.FillRect(num2 + 56 + 1 + num57, num3 + 5 + 1, 64 - num57, 8f);
} else if (num55 == 0) {
apdat_.DrawNumImageComma(g, 78, fanDispParam2, num2 + 100 + 10, num3 + 5, 0, 0, 4);
apdat_.DrawSeb(g, num2 + 102 + 10, num3 + 6, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 18);
} else if (num55 == 1) {
int num56 = 0;
num56 = GameUtil.RateConvert(fanDispParam2, 0, 100, 0, 50, true);
g.DrawImage(apdat_.resCom2_.img[129], num2 + 49 + 10, num3 + 4, 0, 0, 52, 10);
g.DrawImage(apdat_.resCom2_.img[129], num2 + 49 + 1 + 10, num3 + 5, 1, 10, num56, 8);
apdat_.DrawNumImageComma(g, 78, fanDispParam2, num2 + 102 + 10, num3 + 4, 0, 0, 1);
} else if (num55 == 2) {
apdat_.DrawNumImage(g, 78, fanDispParam2, num2 + 100 + 10, num3 + 5, 0, 4);
}
coalescedSwitchExit131982: ;
					}
					num = 153;
					int num58 = 0;
					int num59 = 0;
					for (int num60 = 0; num60 < FanActData.fa_id.Length; num60++)
					{
						num2 = 214;
						num3 = num + 16 * num60;
						int[] array2 = GetMeterParam(num60);
						num58 = num60 * 6;
						num59 = frame_ - FA2_ANIME_TIME[4] - num58;
						if (num59 < 0) { } else {
							num59 = AppData.ClampMax(num59, 12);
							SetWinCharaJamp(num59, 0, -6, 12);
							int cnt = array2[9];
							Draw_upParam(g, 35, num2, num3 + WIN_CHA[1], cnt);
						}
					}
				}
				GameView.AddTouch(2);
			}
			else if (type_ == 42)
			{
				apdat_.DrawWindow(g, 228, 185, 0, "スポンサー");
				num = 59;
				SetFontSize(g, 10);
				g.SetColor(SC_WINDOW_BLUE);
				if (En())
				{
					g.DrawString("契約できる会社", 25f, num);
					g.DrawString("満足", 134f, num);
					g.DrawString("費用", 197f, num);
				}
				else
				{
					g.DrawString("契約できる会社", 25f, num);
					g.DrawString("満足", 141f, num);
					g.DrawString("費用", 187f, num);
				}
				ResetFontSize(g);
				num = 62;
				apdat_.DrawBox(g, 15, num, 224, num + 109);
				g.SetColor(SC_WINDOW_BLACK);
				num2 = 24;
				for (int num61 = scrollValue_; num61 < ary1.Size(); num61++)
				{
					int num62 = num61 - scrollValue_;
					if (num62 >= 6)
					{
						break;
					}
					SponsorData sponsorData = (SponsorData)ary1.ElementAt(num61);
					num3 = num + 15 + num62 * 17;
					if (num61 == select_)
					{
						g.SetColor(SC_WINDOW_SELECT);
						g.FillRect(24f, num3, 195f, 16f);
					}
					GameView.AddTouch(g, 11, 24, num3, 195, 16, 0x20000 | num61, TouchOption.Create(16).Margin(30, 0, 0, 0));
					if (sponsorData.state_ == 2)
					{
						g.DrawImage(apdat_.resCom2_.img[178], num2 - 3, num3);
					}
					if (sponsorData.IsNew())
					{
						Draw_newMark(g, num2 - 8, num3 + 2, frame_);
					}
					if (sponsorData.state_ == 2)
					{
						g.SetColor(Graphics.GetColorOfRGB(183, 165, 156));
					}
					else
					{
						g.SetColor(SC_WINDOW_BLACK);
					}
					SetFontSize(g, 10);
					if (En())
					{
						g.DrawString(sponsorData.name_, num2 + 15, num3 + 2 + 1);
					}
					else
					{
						g.DrawString(sponsorData.name_, num2 + 15, num3 + 2);
					}
					ResetFontSize(g);
					if (sponsorData.state_ == 2)
					{
						apdat_.DrawNumImage(g, 78, sponsorData.satisfactPoint_, num2 + 132, num3 + 3, 0, 4);
						apdat_.DrawSeb(g, num2 + 132 + 1, num3 + 3, apdat_.resCom2_.seb[90], apdat_.resCom2_.img[131], 1);
					}
					if (sponsorData.state_ == 2)
					{
						SetFontSize(g, 8);
						g.SetColor(Graphics.GetColorOfRGB(183, 165, 156));
						if (En())
						{
							g.DrawString("契約中", 173f, num3 + 2 + 2 + 1);
						}
						else
						{
							g.DrawString("契約中", 177f, num3 + 2);
						}
						ResetFontSize(g);
					}
					else
					{
						apdat_.DrawSeb(g, num2 + 150, num3 + 1, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], sponsorData.costType_);
						if (udat_.ChResearchPoin(sponsorData.costType_, sponsorData.cost_))
						{
							apdat_.DrawNumImage(g, 35, sponsorData.cost_, num2 + 192, num3 + 3, 0, 4);
						}
						else
						{
							apdat_.DrawNumImage(g, 91, sponsorData.cost_, num2 + 192, num3 + 3, 0, 4);
						}
					}
					if (num61 == select_)
					{
						apdat_.resCom2_.seb[60].Draw(g, num2 - 6, num3 + 9, apdat_.resCom2_.img);
					}
				}
				apdat_.DrawVerticalScroll2(g, 226, 70, 3, 109, scrollValue_, ary1.Size() - 1, 6);
				coalescedValue = 33;
				if (En())
				{
					coalescedValue += -4;
				}
				num = 171;
				apdat_.DrawBox(g, 15, num + 3, 224, num + 3 + 40 - 3);
				SponsorData sponsorData2 = (SponsorData)ary1.ElementAt(select_);
				g.SetColor(SC_WINDOW_BLUE);
				if (En())
				{
					g.DrawString("特典", coalescedValue + 15 - 5, num + 18);
				}
				else
				{
					g.DrawString("特典", coalescedValue + 15, num + 18);
				}
				g.SetColor(SC_WINDOW_BLACK);
				if (En())
				{
					g.DrawString("毎試合", coalescedValue + 38 + 20, num + 18, 1);
					g.DrawString(FanActData.element_name[sponsorData2.privilegeType_], coalescedValue + 38 + 20, num + 18 + 16, 1);
				}
				else
				{
					g.DrawString("毎試合", 111f, num + 18, 2);
					g.DrawString(FanActData.element_name[sponsorData2.privilegeType_], 111f, num + 18 + 16, 2);
				}
				g.SetColor(SC_WINDOW_BLACK);
				int income = sponsorData2.GetIncome();
				if (En())
				{
					g.DrawString("+" + MyFormBase.YEN("万円", income), coalescedValue + 151 + 20, num + 18, 4);
				}
				else
				{
					g.DrawString("+" + MyFormBase.YEN("万円", income), coalescedValue + 184, num + 18, 4);
				}
				int fanAddParamNum3 = FanActData.GetFanAddParamNum(sponsorData2.privilegeValue_, sponsorData2.privilegeType_);
				if (En())
				{
					Draw_up(g, coalescedValue + 115 + 20, num + 18 + 16, fanAddParamNum3, 1);
				}
				else
				{
					Draw_up(g, coalescedValue + 148, num + 18 + 16, fanAddParamNum3, 1);
				}
				if (udArrowCnt > 0)
				{
					udArrowCnt--;
				}
				coalescedValue = 120;
				num = 80;
				if (udArrowCnt == 0) {
					GameView.AddTouch(g, 2, coalescedValue - 25 - 5, num - 51, 70, 30, 1376456, TouchOption.Create(2));
					GameView.AddTouch(g, 2, coalescedValue - 25 - 5, num + 113, 70, 30, 1376456, TouchOption.Create(2));

} else {
				TouchOption touchOption3 = TouchOption.Create(2);
				TouchOption touchOption4 = TouchOption.Create(2);
				GameView.AddTouch(g, 24, coalescedValue, num - 13 - 1, apdat_.resCom2_.seb[24], 0, 17, touchOption3.Margin(40, 40, 45, 0));
				GameView.AddTouch(g, 24, coalescedValue, num + 77 + 13, apdat_.resCom2_.seb[24], 1, 19, touchOption4.Margin(40, 40, -15, 60));
}
			}
			else if (type_ == 43)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "スポンサー");
				apdat_.DrawBox(g, 83, 52, 227, 134);
				apdat_.DrawBox(g, 12, 136, 227, 205);
				SponsorData sponsorData3 = sponsorData_;
				coalescedValue = 11;
				num = 73;
				g.DrawImage(apdat_.resEvent_.img[8], coalescedValue, num);
				SponsorData.Draw_sponsorChara(g, sponsorData3.charaIndex_, coalescedValue + 36, num + 63);
				apdat_.DrawFukidashiKimochi(g, 45, 95, sponsorData3.feelPt_, 78);
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(sponsorData3.name_, 155f, 71f, 2);
				textLay_.Draw(g, 95f, 88f, 0);
				g.SetColor(SC_WINDOW_BLACK);
				SetFontSize(g, 10);
				if (En())
				{
					g.DrawString(MyFormBase.LT("<0>と交渉に向かいます", sponsorData3.name_), 120f, 158f, 2);
				}
				else
				{
					g.DrawString(MyFormBase.LT("<0>と交渉に向かいます", sponsorData3.name_), 120f, 157f, 2);
				}
				ResetFontSize(g);
				num2 = 37;
				num3 = 177;
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("費用", num2, num3);
				num2 = 69;
				num3 = 175;
				apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], sponsorData3.costType_);
				if (udat_.ChResearchPoin(sponsorData3.costType_, sponsorData3.cost_))
				{
					apdat_.DrawNumImage(g, 35, sponsorData3.cost_, num2 + 30, num3 + 3, 0, 2);
				}
				else
				{
					apdat_.DrawNumImage(g, 91, sponsorData3.cost_, num2 + 30, num3 + 3, 0, 2);
				}
				g.SetColor(SC_WINDOW_BLACK);
				string[] array3 = new string[3] { "簡単", "普通", "難しい" };
				string empty = string.Empty;
				if ((sponsorData3.difficult_ < 50)) { empty = array3[0]; } else { if ((sponsorData3.difficult_ < 100)) { empty = (array3[1]); } else { empty = (array3[2]); } }
				string arg = MyFormBase.LT(empty);
				g.DrawString2(MyFormBase.LT("<co=4B8FFF>難度</co>  <0>", arg), 170f, 177f, 2);
				int num63 = 3;
				num3 = 195;
				num = 193;
				int num64 = 40;
				if (select_ == 0)
				{
					Draw_btmMsg(g, "いざ出陣", 120 - num64, num, true, true);
					Draw_btmMsg(g, "考え直す", 120 + num64, num, false, false);
				}
				else if (select_ == 1)
				{
					Draw_btmMsg(g, "いざ出陣", 120 - num64, num, false, false);
					Draw_btmMsg(g, "考え直す", 120 + num64, num, true, true);
				}
				num64 = 58;
				for (int num65 = 0; num65 < 2; num65++)
				{
					num2 = 75 - num64 / 2 + 90 * num65;
					GameView.AddTouch(g, 3, num2 + DLGSEL_TRECT[num65][0], num3 - 2 + DLGSEL_TRECT[num65][1], num64 + DLGSEL_TRECT[num65][2], 16 + DLGSEL_TRECT[num65][3], 0x20000 | num65);
				}
				g.ClipRect(0f, 194f, 240f, 20f);
				Draw_chara(g, 172, 0, 0, frame_, 26, 222);
				Draw_chara(g, 171, 0, 1, frame_, 212, 222);
				g.PopClip();
			}
			else
			{
				_draw4(g);
			}
		}

		private void _draw4(Graphics g)
		{
			int num4;
int num;
int num2;
int num3;


			num4 = num = num2 = num3 = 0;
			Font font = g.GetFont(null);
			if (type_ == 44)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "スポンサー交渉");
				apdat_.DrawBox(g, 12, 136, 227, 205);
				SponsorData sponsorData = sponsorData_;
				num4 = 30;
				num = 64;
				g.DrawImage(apdat_.resEvent_.img[7], num4, num);
				num4 = 16;
				num = 70;
				g.DrawImage(apdat_.resEvent_.img[8], num4, num);
				Draw_chara(g, 172, 0, 0, frame_, num4 + 26, num + 63);
				Draw_chara(g, 171, 0, 0, frame_, num4 + 44, num + 63);
				int[] feelPtEfCommentParam = UserData.GetFeelPtEfCommentParam(frame_ - SP_ANIME_TIME[0], SP_ANIME_TIME[1] - SP_ANIME_TIME[0]);
				if (feelPtEfCommentParam[0] == 1)
				{
					string text = UserData.EF_FEELPT_COMMENT_[feelPtEfCommentParam[1]][feelPtEfCommentParam[2]];
					apdat_.DrawFukidashiBalloon(g, text, num4 + 33, num + 24, SC_WINDOW_BLUE);
				}
				num4 = 155;
				num = 70;
				g.DrawImage(apdat_.resEvent_.img[8], num4, num);
				SponsorData.Draw_sponsorChara(g, sponsorData.charaIndex_, num4 + 35, num + 63);
				int feelPtEf = UserData.GetFeelPtEf(frame_ - SP_ANIME_TIME[0], SP_ANIME_TIME[1] - SP_ANIME_TIME[0]);
				apdat_.DrawFukidashiKimochi(g, 189, 94, feelPtEf, 78);
				int[] addFeelPtEf = UserData.GetAddFeelPtEf(frame_ - SP_ANIME_TIME[0], SP_ANIME_TIME[1] - SP_ANIME_TIME[0]);
				if (addFeelPtEf[0] >= 1)
				{
					int num5 = 0;
					int num6 = 78;
					int num7 = 105;
					if (addFeelPtEf[0] == 2)
					{
						num5 = frame_ % 8 / 4 * -2;
						num6 = 35;
						num7 = 103;
					}
					int num8 = AppData.GetFig(addFeelPtEf[1]) * 8 + 8 + 2;
					num3 = 76;
					num2 = 208;
					apdat_.DrawNumImage(g, num6, addFeelPtEf[1], num2, num3 + num5, 0, 4);
					apdat_.DrawSeb(g, num2 - num8, num3 + num5, apdat_.resCom2_.seb[num6], apdat_.resCom2_.img[num7], 14);
				}
				num = 153;
				if (frame_ < SP_ANIME_TIME[1])
				{
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.LT("<0>と交渉中", sponsorData.name_), 120f, num, 2);
				}
				else
				{
					int num9 = GameUtil.RateConvert(frame_, SP_ANIME_TIME[2], SP_ANIME_TIME[3], 0, 14, true);
					g.ClipRect(0f, num, 240f, num9);
					string feelPtEfRetStr = UserData.GetFeelPtEfRetStr(UserData.NEGOTIATE_RESULT_DISPVALUE[3]);
					if (UserData.NEGOTIATE_RESULT_DISPVALUE[3] < 100)
					{
						g.SetColor(SC_WINDOW_BLACK);
					}
					else
					{
						g.SetColor(SC_WINDOW_BLUE);
					}
					g.DrawString(feelPtEfRetStr, 120f, num, 2);
					g.PopClip();
				}
				num2 = 41;
				num3 = 169;
				int num10 = GameUtil.RateConvert(frame_, SP_ANIME_TIME[0], SP_ANIME_TIME[1], 0, 156, true);
				g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
				g.FillRect(num2, num3 + 1, 158f, 10f);
				g.SetColor(Graphics.GetColorOfRGB(115, 175, 188));
				g.DrawRect(num2, num3 + 1, 157f, 9f);
				g.SetColor(Graphics.GetColorOfRGB(0, 254, 0));
				g.FillRect(num2 + 1, num3 + 2, num10, 8f);
				num3 = 206;
				g.ClipRect(0f, num3 - 20, 240f, 20f);
				for (int i = 0; i < UserData.NEGOTIATE_RESULT_TH.Length; i++)
				{
					num2 = 70 + 50 * i;
					int num11 = 0;
					int frame = UserData.NEGOTIATE_RESULT_TH[i];
					int num12 = (SP_ANIME_TIME[1] - SP_ANIME_TIME[0]) * UserData.WAIT_TIME[i] / 100 + SP_ANIME_TIME[0] - 5;
					num11 = GameUtil.RateConvert(frame_ - num12, 0, 10, -20, 0, true);
					if (num11 <= -20) { continue; }
apdat_.DrawSeb(g, num2, num3 + num11, apdat_.resCom2_.seb[97], apdat_.resCom2_.img[175], frame);
				}
				g.PopClip();
				GameView.AddTouch(2);
			}
			else if (type_ == 45)
			{
				apdat_.DrawWindow(g, 228, 191, 0, "特別な練習");
				apdat_.DrawBox(g, 12, 129, 227, 213);
				num4 = 30;
				num = 57;
				g.DrawImage(apdat_.resEvent_.img[11], num4, num);
				num3 = 126;
				bool flag = false;
				for (int j = 0; j < TrainingData.TRA_PLAYER.Length; j++)
				{
					num2 = 66 + 54 * j;
					if (TrainingData.TRA_PLAYER[j] == -1)
					{
						apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[92], apdat_.resCom2_.img[167], 1);
					}
					else
					{
						PlayerData playerData = apdat_.pData_[TrainingData.TRA_PLAYER[j]];
						apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[92], apdat_.resCom2_.img[167], 0);
						int frame2 = frame_ % 20 / 5;
						udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, frame2, 1);
						udat_.DrawDispPlayer(g, num2, num3 - 4);
						flag = false;
						if (udat_.memberPDataist_.IndexOf(playerData) < 11)
						{
							flag = true;
						}
						if (flag)
						{
							apdat_.DrawSeb(g, num2 - 10, num3 - 1, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], playerData.role_);
						}
						else
						{
							apdat_.DrawSeb(g, num2 - 10, num3 - 1, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], 4);
						}
					}
					if (select_ == j)
					{
						udat_.Draw_colorRect(g, num2 - 16, num3 - 32, frame_, 31, 31);
					}
					GameView.AddTouch(g, 3, num2 - 16 - 30, num3 - 32 - 15, 92, 62, 0x20000 | j);
				}
				g.SetColor(Graphics.GetColorOfRGB(222, 234, 225));
				g.DrawLine(14f, 170f, 224f, 170f);
				g.DrawLine(14f, 195f, 224f, 195f);
				num3 = 148;
				for (int k = 0; k < TrainingData.TRA_PLAYER.Length; k++)
				{
					num2 = 66 + 54 * k;
					if (select_ == k)
					{
						g.SetColor(SC_WINDOW_SELECT);
						g.FillRect(num2 - 20, num3, 40f, 17f);
						apdat_.DrawSeb(g, num2 - 20 - 5, num3 + 8, apdat_.resCom2_.seb[24], apdat_.resCom2_.img[27], 0);
						apdat_.DrawSeb(g, num2 + 20 + 5, num3 + 8, apdat_.resCom2_.seb[24], apdat_.resCom2_.img[27], 1);
					}
					GameView.AddTouch(g, 3, num2 - 20, num3 - 3, 40, 23, 0x20000 | k);
					g.SetColor(SC_WINDOW_BLACK);
					if (TrainingData.TRA_PLAYER[k] == -1)
					{
						g.DrawString("---", num2, num3 + 2, 2);
						continue;
					}
					PlayerData playerData2 = apdat_.pData_[TrainingData.TRA_PLAYER[k]];
					SetFontSize(g, 10);
					if (En())
					{
						g.DrawString(playerData2.name_, num2, num3 + 2 + 1, 2);
					}
					else
					{
						g.DrawString(playerData2.name_, num2, num3 + 2, 2);
					}
					ResetFontSize(g);
				}
				num = 175;
				if (select_ == 3)
				{
					g.SetColor(SC_WINDOW_SELECT);
					if (En())
					{
						g.FillRect(63f, num, 93f, 16f);
					}
					else
					{
						g.FillRect(63f, num, 85f, 16f);
					}
				}
				GameView.AddTouch(g, 2, 40, num, 160, 16, 131075, TouchOption.Create().Margin(32, 32, 0, 8));
				TrainingData trainingData = apdat_.trainingData_[TrainingData.TRA_SELECT_TRA];
				g.SetColor(SC_WINDOW_BLUE);
				SetFontSize(g, 10);
				g.DrawString("ﾄﾚｰﾆﾝｸﾞ", 20f, 177f);
				ResetFontSize(g);
				g.SetColor(SC_WINDOW_BLACK);
				if (En())
				{
					g.DrawString(trainingData.name_, 110f, 177f, 2);
				}
				else
				{
					g.DrawString(trainingData.name_, 106f, 177f, 2);
				}
				num2 = 152;
				if (En())
				{
					num2 += 7;
				}
				num3 = 175;
				apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], trainingData.costType_);
				if (udat_.ChResearchPoin(trainingData.costType_, trainingData.cost_))
				{
					apdat_.DrawNumImage(g, 35, trainingData.cost_, num2 + 41, num3 + 3, 0, 4);
				}
				else
				{
					apdat_.DrawNumImage(g, 91, trainingData.cost_, num2 + 41, num3 + 3, 0, 4);
				}
				g.SetColor(Graphics.GetColorOfRGB(151, 151, 152));
				SetFontSize(g, 10);
				if (En())
				{
					g.DrawString("消費", num2 + 43, 179f);
				}
				else
				{
					g.DrawString("消費", num2 + 43, 177f);
				}
				ResetFontSize(g);
				num = 200;
				if (select_ == 4)
				{
					g.SetColor(SC_WINDOW_SELECT);
					g.FillRect(90f, num, 60f, 16f);
				}
				GameView.AddTouch(g, 2, 40, num, 160, 16, 131076, TouchOption.Create().Margin(32, 32, 0, 12));
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString("実行する", 120f, num + 2, 2);
				if (select_ == 4)
				{
					apdat_.resCom2_.seb[60].Draw(g, 86f, num + 9, apdat_.resCom2_.img);
				}
			}
			else if (type_ == 46)
			{
				apdat_.DrawWindow(g, 223, 182, 0, MyFormBase.LT("練習リスト") + " " + (page_ + 1) + "/" + ((ary1.Size() - 1) / 5 + 1));
				if (page_ > 0)
				{
					GameView.AddTouch(g, 1, 44 - frame_ % 20 / 5, 47, apdat_.resCom2_.seb[24], 0, 16);
				}
				if (page_ < (ary1.Size() - 1) / 5)
				{
					GameView.AddTouch(g, 1, 193 + frame_ % 20 / 5, 47, apdat_.resCom2_.seb[24], 1, 18);
				}
				num = 60;
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("名前", 52f, num);
				g.DrawString("消費ﾎﾟｲﾝﾄ", 208f, num, 4);
				num = 63;
				apdat_.DrawBox(g, 14, num, 222, num + 92);
				g.SetColor(SC_WINDOW_BLACK);
				num2 = 23;
				for (int l = 5 * page_; l < ary1.Size(); l++)
				{
					int num13 = l - 5 * page_;
					if (num13 >= 5)
					{
						break;
					}
					TrainingData trainingData2 = (TrainingData)ary1.ElementAt(l);
					num3 = num + 15 + num13 * 17;
					Draw_trainingIcon(g, trainingData2.icon_, num2, num3);
					if (l == select_ + page_ * 5)
					{
						g.SetColor(Graphics.GetColorOfRGB(247, 203, 125));
						g.FillRect(num2 + 16 + 2, num3, 176f, 16f);
					}
					GameView.AddTouch(g, 11, num2 + 16 - 16, num3, 192, 16, 0x20000 | num13, TouchOption.Create(16).Margin(30, 0, 0, 0));
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(trainingData2.name_, num2 + 18, num3 + 2);
					apdat_.DrawSeb(g, num2 + 138, num3 + 1, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], trainingData2.costType_);
					if (udat_.ChResearchPoin(trainingData2.costType_, trainingData2.cost_))
					{
						apdat_.DrawNumImage(g, 35, trainingData2.cost_, num2 + 184, num3 + 4, 0, 4);
					}
					else
					{
						apdat_.DrawNumImage(g, 91, trainingData2.cost_, num2 + 184, num3 + 4, 0, 4);
					}
					if (trainingData2.IsNew())
					{
						Draw_newMark(g, num2 - 5, num3 + 2, frame_);
					}
					if (l == select_ + page_ * 5)
					{
						apdat_.resCom2_.seb[60].Draw(g, num2 - 6, num3 + 9, apdat_.resCom2_.img);
					}
				}
				num4 = 33;
				num = 159;
				apdat_.DrawBox(g, 14, num, 222, num + 45);
				TrainingData trainingData3 = (TrainingData)ary1.ElementAt(select_ + page_ * 5);
				for (int m = 0; m < 5; m++)
				{
					num2 = 49 + 67 * (m / 2);
					num3 = num + 14 + 21 * (m % 2);
					g.SetColor(Graphics.GetColorOfRGB(197, 255, 183));
					g.FillRect(num2, num3, 34f, 18f);
					int num14 = (trainingData3.eff_[m][0] + trainingData3.eff_[m][1]) / 2;
					if (num14 > 0)
					{
						int trainingAddParamNum = TrainingData.GetTrainingAddParamNum(num14);
						Draw_up(g, num2 + 4, num3 + 5, trainingAddParamNum, 1);
					}
					apdat_.DrawSeb(g, num2 - 28, num3 + 3, apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], m);
				}
			}
			else
			{
				if (type_ == 48) { } else if (type_ == 49)
				{
					apdat_.DrawWindow(g, 228, 176, 0, "特別な練習");
					apdat_.DrawBox(g, 12, 136, 227, 205);
					TrainingData trainingData4 = apdat_.trainingData_[TrainingData.TRA_SELECT_TRA];
					int[] array = new int[5] { 8, 11, 13, 9, 11 };
					num4 = 30;
					num = 64;
					g.DrawImage(apdat_.resEvent_.img[11], num4, num);
					int num15 = 0;
					for (int n = 0; n < TrainingData.TRA_PLAYER.Length; n++)
					{
						num2 = 66 + 54 * n;
						num3 = 133;
						if (TrainingData.TRA_PLAYER[n] == -1)
						{
							continue;
						}
						PlayerData playerData3 = apdat_.pData_[TrainingData.TRA_PLAYER[n]];
						int num16 = 0;
						int num17 = 0;
						int num18 = 0;
						if (training_cnt < TRA_ANIME_TIME2[2])
						{
							SetWinCharaTraining(training_cnt, trainingData4.tMode_);
							num17 = WIN_CHA[0];
							num16 = WIN_CHA[2];
							num18 = WIN_CHA[1];
						}
						else
						{
							num17 = 0;
							num16 = 0;
						}
						apdat_.DrawSeb(g, num2, num3 - 4 + 1, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 4);
						udat_.SetDispPlayerData(num17, udat_.selectUniform_, playerData3.faceImg_, num16, 1);
						udat_.DrawDispPlayer(g, num2, num3 - 4 + num18);
						if (training_cnt >= TRA_ANIME_TIME2[0] && training_cnt < TRA_ANIME_TIME2[1]) {
						int num19 = training_cnt - TRA_ANIME_TIME2[0];
						int num20 = num19 / 25;
						int num21 = num19 % 25;
						int num22 = num2 - 13;
						int num23 = 87;
						if (num21 >= TRA_AT2[0])
						{
							g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
							g.DrawRect(num22, num23, 30f, 12f);
							g.SetColor(Graphics.GetColorOfRGB(255, 255, 223));
							g.FillRect(num22 + 1, num23 + 1, 29f, 11f);
							int num24 = array[num20];
							if (AppData.IsLanguageJpBase(false))
							{
								num24 = 12;
							}
							apdat_.DrawSeb(g, num22 + 15 - num24, num23 - 1, apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], num20);
						}
						num15 = TrainingData.TRA_UPPARAM_DISP[n][num20];
						int num25 = GameUtil.RateConvert(num21, TRA_AT[0], TRA_AT[1], 12, 0, true);
						g.ClipRect(num22, num23 + 15, 51f, 12f);
						if (num15 < 10)
						{
							Draw_upParam(g, 78, num22 + 6, num23 + 15 + num25, num15, 1);
						}
						else
						{
							Draw_upParam(g, 35, num22 + 6, num23 + 15 + num25, num15, 1);
						}
						g.PopClip();

}}
					num2 = 94;
					num3 = 132;
					int num26 = GameUtil.RateConvert(training_cnt, TRA_ANIME_TIME2[0], TRA_ANIME_TIME2[1], 0, 50, true);
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
					g.FillRect(num2, num3 + 1, 52f, 10f);
					g.SetColor(Graphics.GetColorOfRGB(115, 175, 188));
					g.DrawRect(num2, num3 + 1, 51f, 9f);
					g.SetColor(Graphics.GetColorOfRGB(0, 254, 0));
					g.FillRect(num2 + 1, num3 + 2, num26, 8f);
					if (training_cnt >= TRA_ANIME_TIME2[1])
					{
						if (En())
						{
							g.DrawImage(apdat_.resCom2_.img[188], 101f, 131f);
						}
						else
						{
							g.DrawImage(apdat_.resCom2_.img[188], 107f, 131f);
						}
					}
					num4 = 20;
					num = 149;
					g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
					g.DrawRect(num4, num, 30f, 56f);
					int num27 = 0;
					for (int num28 = 0; num28 < PlayerData.PARAM_NAME.Length; num28++)
					{
						num3 = num + 13 * num28;
						int num29 = 12;
						if (num28 == PlayerData.PARAM_NAME.Length - 1)
						{
							num29 = 13;
						}
						g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
						g.DrawRect(num4, num3, 30f, num29 - 1);
						g.DrawLine(num4, num3 + num29, num4 + 186 - 1, num3 + num29);
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 223));
						g.FillRect(num4 + 1, num3 + 1, 29f, num29 - 2);
						int num30 = array[num28];
						if (AppData.IsLanguageJpBase(false))
						{
							num30 = 12;
						}
						apdat_.DrawSeb(g, num4 + 15 - num30, num3 - 1, apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], num28);
						for (int num31 = 0; num31 < TrainingData.TRA_PLAYER.Length; num31++)
						{
							num2 = num4 + 30 + 52 * num31;
							g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
							g.DrawRect(num2, num3, 51f, num29 - 1);
							if (TrainingData.TRA_PLAYER[num31] == -1) { continue; }
{
								PlayerData playerData4 = apdat_.pData_[TrainingData.TRA_PLAYER[num31]];
								num27 = TrainingData.TRA_UPPARAM_DISP[num31][num28];
								int num32 = 0;
								num32 = GameUtil.RateConvert(training_cnt, TRA_ANIME_TIME2[3], TRA_ANIME_TIME2[4], playerData4.paramPhysical_[num28], TrainingData.TRA_UPPARAM[num31][num28], true);
								int seb = 89;
								if (playerData4.paramPhysical_[num28] != num32)
								{
									seb = 104;
								}
								g.SetColor(Graphics.GetColorOfRGB(57, 57, 70));
								g.FillRect(num2 + 1, num3 + 1, 50f, num29 - 2);
								int phyParamBarW = PlayerData.GetPhyParamBarW(num32, 50);
								if (num28 == 0) { g.SetColor(Graphics.GetColorOfRGB(252, 54, 0)); }
else if (num28 == 1) { g.SetColor(Graphics.GetColorOfRGB(84, 188, 255)); }
else if (num28 == 2) { g.SetColor(Graphics.GetColorOfRGB(103, 255, 121)); }
else if (num28 == 3) { g.SetColor(Graphics.GetColorOfRGB(248, 248, 248)); }
else if (num28 == 4) { g.SetColor(Graphics.GetColorOfRGB(255, 251, 0)); }
								g.FillRect(num2 + 1, num3 + 1, phyParamBarW, num29 - 2);
								apdat_.DrawNumImage(g, seb, num32, num2 + 26, num3 + 2, 0, 2);
							}
						}
					}
					GameView.AddTouch(2, TouchOption.Create(2));
				}
				else if (type_ == 50 || type_ == 51)
				{
					if (type_ == 50)
					{
						apdat_.DrawWindow(g, 228, 204, 0, "監督");
					}
					else if (type_ == 51)
					{
						apdat_.DrawWindow(g, 228, 204, 0, MyFormBase.LT("監督 <0>/<1>", select_ + 1 + "\t" + ary1.Size()));
						GameView.AddTouch(g, 1, 44 - frame_ % 20 / 5, 37, apdat_.resCom2_.seb[24], 0, 16);
						GameView.AddTouch(g, 1, 193 + frame_ % 20 / 5, 37, apdat_.resCom2_.seb[24], 1, 18);
					}
					apdat_.DrawBox(g, 18, 44, 223, 203);
					SupervisorData supervisorData = null;
					if (type_ == 50)
					{
						supervisorData = apdat_.svisorData_[udat_.selectSupervisorId_];
					}
					else if (type_ == 51)
					{
						supervisorData = (SupervisorData)ary1.ElementAt(select_);
					}
					num4 = 23;
					num = 62;
					g.DrawImage(apdat_.resCom2_.img[125], num4, num);
					int frame3 = frame_ % 16 / 4;
					udat_.SetDispPlayerData(0, supervisorData.bodyImg_, supervisorData.headImg_, frame3, 1);
					udat_.DrawDispPlayer(g, num4 + 17, num + 27);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.LT("<0> <1>", supervisorData.name_ + "\t" + supervisorData.name2_), num4 + 38 - 2, num + 3);
					g.DrawImage(apdat_.resCom2_.img[126], num4 + 10 + 38, num + 3 + 18);
					apdat_.DrawNumImage(g, 79, supervisorData.lv_, num4 + 40 + 38, num + 18, 0, 2);
					num4 = 138;
					num = 63;
					for (int num33 = 0; num33 < 4; num33++)
					{
						num2 = num4 + 38 * (num33 % 2);
						num3 = num + 18 * (num33 / 2);
						apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], num33);
						apdat_.DrawNumImage(g, 35, supervisorData.paramResearch_[num33], num2 + 32, num3 + 3, 0, 4);
					}
					num4 = 70;
					num = 151;
					apdat_.DrawBox(g, num4 - 45, num - 63, num4 - 45 + 94, num - 63 + 84);
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("指導", num4 - 29, num - 46);
					apdat_.DrawSeb(g, num4 + 24, num - 35, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], supervisorData.rankPhysical_);
					g.SetColor(Graphics.GetColorOfRGB(204, 224, 255));
					g.DrawLine(num4 - 41, num - 33, num4 + 46, num - 33);
					num2 = 71;
					num3 = 151;
					udat_.DrawGraph_menuElement(g, num2, num3, udat_.playerParam1, udat_.playerParam2, 100);
					for (int num34 = 0; num34 < UserData.GRAPH_POS.Length; num34++)
					{
						apdat_.DrawSeb(g, num2 + UserData.GRAPH_POS[num34][0], num3 + UserData.GRAPH_POS[num34][1], apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], num34);
					}
					num4 = 141;
					num = 122;
					apdat_.DrawBox(g, num4 - 18, num - 34, num4 - 18 + 94, num - 34 + 84);
					FormationData formationData = apdat_.formationData_[supervisorData.formation_];
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("ﾌｫｰﾒｰｼｮﾝ", num4 + 2, num - 18);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(formationData.name_, num4 + 29, num + 4, 2);
					g.SetColor(Graphics.GetColorOfRGB(204, 224, 255));
					g.DrawLine(num4 - 16, num - 4, num4 + 73, num - 4);
					int num35 = 75;
					int num36 = 34;
					num2 = num4 - 8;
					num3 = num + 20;
					g.SetColor(Graphics.GetColorOfRGB(179, 255, 77));
					g.FillRect(num2, num3, num35, num36);
					g.SetColor(Graphics.GetColorOfRGB(142, 215, 45));
					g.DrawRect(num2, num3, num35 - 1, num36 - 1);
					int num39, num37;
num39 = num37 = 0;
					for (int num38 = 0; num38 < formationData.formation_.Length; num38++)
					{
						 num39 = GameUtil.RateConvert(formationData.formation_[num38][1], 0, 8, num2 + num35 - 10, num2 + 10, true);
						num37 = GameUtil.RateConvert(formationData.formation_[num38][0], 0, 8, num3 + 1 + 1, num3 + num36 - 1 - 2, true);
						if (num38 == 0)
						{
							num39 = num2 + 3;
							g.SetColor(Graphics.GetColorOfRGB(18, 211, 86));
						}
						else
						{
							g.SetColor(Graphics.GetColorOfRGB(6, 166, 254));
						}
						g.FillRect(num39 - 1, num37 - 1, 3f, 3f);
					}
					if (formationData.state_ == 1)
					{
						g.DrawImage(apdat_.resCom2_.img[179], 150f, 171f);
					}
					num4 = 24;
					num = 187;
					apdat_.DrawBox(g, num4, num - 11, num4 + 193, num - 11 + 22);
					if (type_ == 50)
					{
						g.SetColor(SC_WINDOW_BLUE);
						SetFontSize(g, 10);
						if (!En())
						{
							g.DrawString("方針", 36f, 193f);
						}
						else
						{
							g.DrawString("方針", 36f, 192f);
						}
						ResetFontSize(g);
						num2 = 140;
						if (En())
						{
							num2 += 6;
						}
						num3 = 190;
						int num40 = 124;
						if (En())
						{
							num40 += -16;
						}
						g.SetColor(SC_WINDOW_SELECT);
						g.FillRect(num2 - num40 / 2 + 3, num3, num40 - 2, 16f);
						GameView.AddTouch(g, 18, num2 - num40 / 2 - 5, num3 + 8, apdat_.resCom2_.seb[24], 0, 16, TouchOption.Create().Margin(60, 5, 20, 20));
						GameView.AddTouch(g, 18, num2 + num40 / 2 + 5, num3 + 8, apdat_.resCom2_.seb[24], 1, 18, TouchOption.Create().Margin(5, 50, 20, 20));
						GameView.AddTouch(g, 2, num2 - num40 / 2 + 20, num3 - 10, num40 - 40, 36, 0x20000 | select_);
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString(SupervisorData.TRAINING_POLICY_STR[select_], num2, num3 + 2, 2);
					}
					else if (type_ == 51)
					{
						g.ClipRect(25f, 188f, 20f, 19f);
						udat_.SetDispPlayerData(0, supervisorData.bodyImg_, supervisorData.headImg_, 0, 1);
						udat_.DrawDispPlayer(g, 36, 211);
						g.PopClip();
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString2(supervisorData.explain_, num4 + 25, num + 6);
					}
					num = 215;
					if (type_ == 50)
					{
						SetFontSize(g, 10);
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString("月給", 28f, 218f);
						if (En())
						{
							g.DrawString("就任", 116f, 218f);
						}
						else
						{
							g.DrawString("就任", 133f, 218f);
						}
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString(MyFormBase.YEN("万", supervisorData.GetSallay()), 105f, 218f, 4);
						if (En())
						{
							g.DrawString(MyFormBase.LT("<0>ヵ月", AppData.CommaSeparate(supervisorData.workMonth_)), 214f, 218f, 4);
						}
						else
						{
							g.DrawString(MyFormBase.LT("<0>ヵ月", AppData.CommaSeparate(supervisorData.workMonth_)), 212f, 218f, 4);
						}
						ResetFontSize(g);
					}
					else if (type_ == 51)
					{
						SetFontSize(g, 10);
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString("月給", 28f, 218f);
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString(MyFormBase.YEN("万", supervisorData.GetSallay()), 105f, 218f, 4);
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString("タッチで決定", 139f, 218f);
						ResetFontSize(g);
					}
					if (type_ == 51)
					{
						GameView.AddTouch(2, TouchOption.Create(2));
					}
				}
				else if (type_ == 52)
				{
					apdat_.DrawWindow(g, 228, 176, 0, "監督");
					apdat_.DrawBox(g, 12, 53, 227, 134);
					apdat_.DrawBox(g, 12, 136, 227, 205);
					SupervisorData supervisorData2 = apdat_.svisorData_[udat_.selectSupervisorId_];
					num4 = 25;
					num = 77;
					g.DrawImage(apdat_.resCom2_.img[125], num4, num);
					int frame4 = frame_ % 16 / 4;
					udat_.SetDispPlayerData(0, supervisorData2.bodyImg_, supervisorData2.headImg_, frame4, 1);
					udat_.DrawDispPlayer(g, num4 + 17, num + 27);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.LT("<0> <1>", supervisorData2.name_ + "\t" + supervisorData2.name2_), num4 + 38 - 2, num + 3);
					g.DrawImage(apdat_.resCom2_.img[126], num4 + 10 + 38, num + 3 + 18);
					apdat_.DrawNumImage(g, 79, supervisorData2.lv_, num4 + 40 + 38, num + 18, 0, 2);
					num4 = 140;
					num = 68;
					g.SetColor(Graphics.GetColorOfRGB(221, 251, 219));
					g.FillRect(num4, num, 80f, 52f);
					g.DrawImage(apdat_.resCom2_.img[176], num4 + 12, num + 3);
					num4 = 143;
					num = 83;
					for (int num41 = 0; num41 < 4; num41++)
					{
						num2 = num4 + 38 * (num41 % 2);
						num3 = num + 18 * (num41 / 2);
						apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], num41);
						apdat_.DrawNumImage(g, 35, supervisorData2.paramResearch_[num41], num2 + 32, num3 + 3, 0, 4);
					}
					g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
					g.DrawLine(14f, 123f, 224f, 123f);
					num4 = 28;
					num = 128;
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("指導", num4, num);
					apdat_.DrawSeb(g, num4 + 53, num + 12, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], supervisorData2.rankPhysical_);
					num4 = 115;
					num = 127;
					FormationData formationData2 = apdat_.formationData_[supervisorData2.formation_];
					g.SetColor(SC_WINDOW_BLUE);
					SetFontSize(g, 10);
					if (!En())
					{
						g.DrawString("ﾌｫｰﾒｰｼｮﾝ", num4, num);
					}
					else
					{
						g.DrawString("ﾌｫｰﾒｰｼｮﾝ", num4, num + 1);
					}
					ResetFontSize(g);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(formationData2.name_, num4 + 80, num, 2);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.LT("<0>さんと契約延長しますか", supervisorData2.name_), 120f, 157f, 2);
					g.DrawString2(MyFormBase.LT("<co=4B8FFF>月給</co> <0>", MyFormBase.YEN("万", supervisorData2.GetSallay())), 120f, 177f, 2);
					num3 = 195;
					num = 193;
					int num42 = 40;
					if (select_ == 0)
					{
						Draw_btmMsg(g, "入替え", 120 - num42, num, true, true);
						Draw_btmMsg(g, "延長する", 120 + num42, num, false, false);
					}
					else if (select_ == 1)
					{
						Draw_btmMsg(g, "入替え", 120 - num42, num, false, false);
						Draw_btmMsg(g, "延長する", 120 + num42, num, true, true);
					}
					num42 = 58;
					int num43 = 100;
					int num44 = 160;
					GameView.AddTouch(g, 3, 75 - num42 / 2 - num43 / 2, num3 - 2 - num44 / 2, 68 + num43, 16 + num44, 131072);
					GameView.AddTouch(g, 3, 75 - num42 / 2 + 90 - num43 / 2, num3 - 2 - num44 / 2, 68 + num43, 16 + num44, 131073);
				}
				else if (type_ == 53)
				{
					apdat_.DrawWindow(g, 228, 204, 0, "選手の獲得");
					if (MyFormBase.Jp()) { num = 50; } else { num = 51; }
					SetFontSize(g, 10);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString("名前", 44f, num);
					if (En())
					{
						g.DrawString("交渉費用", 142f, num);
					}
					else
					{
						g.DrawString("交渉費用", 162f, num);
					}
					ResetFontSize(g);
					num = 69;
					apdat_.DrawBox(g, 16, num - 16, 223, num - 16 + 95);
					g.SetColor(SC_WINDOW_BLACK);
					for (int num45 = scrollValue_; num45 < ary1.Size(); num45++)
					{
						int num46 = num45 - scrollValue_;
						if (num46 >= 5)
						{
							break;
						}
						TradeData tradeData = (TradeData)ary1.ElementAt(num45);
						PlayerData playerData5 = apdat_.pData_[tradeData.playerDataId_];
						num2 = 26;
						num3 = num + num46 * 17;
						g.SetColor(Graphics.GetColorOfRGB(196, 236, 169));
						g.FillRect(num2, num3, 16f, 16f);
						g.SetColor(Graphics.GetColorOfRGB(204, 204, 204));
						g.DrawRect(num2, num3, 15f, 15f);
						if (num45 == select_)
						{
							g.SetColor(SC_WINDOW_SELECT);
							g.FillRect(num2 + 16, num3, 175f, 16f);
						}
						GameView.AddTouch(g, 11, num2 + 16 - 16, num3, 191, 16, 0x20000 | num45, TouchOption.Create(16).Margin(30, 0, 0, 0));
						g.ClipRect(num2, num3, 13f, 16f);
						udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData5.faceImg_, 0, 1);
						udat_.DrawDispPlayer(g, num2 + 8, num3 + 16 + 5);
						g.PopClip();
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString(MyFormBase.LT("<0> <1>", playerData5.name_ + "\t" + playerData5.name2_), num2 + 18, num3 + 2);
						if (apdat_.ChPayMoney(tradeData.cost_))
						{
							g.SetColor(SC_WINDOW_BLACK);
						}
						else
						{
							g.SetColor(SC_WINDOW_RED);
						}
						g.DrawString(MyFormBase.YEN("万", tradeData.cost_), 214f, num3 + 2, 4);
						num2 = 142;
						if (En())
						{
							num2 += -10;
						}
						num3++;
						int mostEvaPosition = PlayerData.GetMostEvaPosition(playerData5.paramPhysical_);
						apdat_.DrawSeb(g, num2, num3 + 1, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], mostEvaPosition);
						num2 = 26;
						num3 = num + num46 * 17;
						if (tradeData.IsNew())
						{
							Draw_newMark(g, num2 - 20, num3 + 2, frame_);
						}
						if (num45 == select_)
						{
							apdat_.resCom2_.seb[60].Draw(g, num2 - 8, num3 + 9, apdat_.resCom2_.img);
						}
					}
					apdat_.DrawVerticalScroll2(g, 225, 64, 3, 96, scrollValue_, ary1.Size() - 1, 5);
					num4 = 29;
					num = 168;
					apdat_.DrawBox(g, 16, num - 16, 223, num - 16 + 66);
					TradeData tradeData2 = (TradeData)ary1.ElementAt(select_);
					PlayerData playerData6 = apdat_.pData_[tradeData2.playerDataId_];
					g.SetColor(Graphics.GetColorOfRGB(223, 248, 200));
					g.FillRect(20f, 170f, 111f, 16f);
					g.DrawImage(apdat_.resCom2_.img[126], num4 - 2, num + 6);
					apdat_.DrawNumImage(g, 79, playerData6.lv_, num4 + 25, num + 4, 0, 2);
					num2 = 80;
					if (En())
					{
						num2 = 72;
					}
					g.SetColor(SC_WINDOW_BLUE);
					SetFontSize(g, 10);
					if (En())
					{
						g.DrawString("人気", num2, num + 5 + 1);
					}
					else
					{
						g.DrawString("人気", num2, num + 5);
					}
					ResetFontSize(g);
					g.SetColor(SC_WINDOW_BLACK);
					if (En())
					{
						g.DrawString(playerData6.popularP_ + string.Empty, num2 + 57, num + 5, 4);
					}
					else
					{
						g.DrawString(playerData6.popularP_ + string.Empty, num2 + 45, num + 5, 4);
					}
					num2 = 39;
					if (En())
					{
						num2 = 33;
					}
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("気持", num2, num + 22);
					num2 = 89;
					if (En())
					{
						num2 = 106;
					}
					int feelPt_ = tradeData2.feelPt_;
					if (feelPt_ == 100)
					{
						apdat_.DrawNumImage(g, 35, feelPt_, num2, num + 23, 0, 4);
					}
					else
					{
						apdat_.DrawNumImage(g, 78, feelPt_, num2, num + 23, 0, 4);
					}
					apdat_.DrawSeb(g, num2 + 2, num + 23, apdat_.resCom2_.seb[90], apdat_.resCom2_.img[131], 1);
					g.SetColor(Graphics.GetColorOfRGB(223, 248, 200));
					g.FillRect(20f, 206f, 111f, 16f);
					num2 = num4 + 9;
					if (En())
					{
						num2 += -6;
					}
					Draw_categoryIcon(g, playerData6.category_, num2, num + 39);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(PlayerData.CATEGORY_STR[playerData6.category_], num2 + 20, num + 39 + 2);
					num4 = 134;
					num = 168;
					g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
					g.DrawRect(num4, num, 83f, 56f);
					int[] array2 = new int[5] { 8, 11, 13, 9, 11 };
					for (int num47 = 0; num47 < PlayerData.PARAM_NAME.Length; num47++)
					{
						num3 = num + 11 * num47;
						int num48 = 12;
						if (num47 == PlayerData.PARAM_NAME.Length - 1)
						{
							num48 = 13;
						}
						g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
						g.DrawRect(num4, num3, 30f, num48 - 1);
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 223));
						g.FillRect(num4 + 1, num3 + 1, 29f, num48 - 2);
						int num49 = array2[num47];
						if (AppData.IsLanguageJpBase(false))
						{
							num49 = 12;
						}
						apdat_.DrawSeb(g, num4 + 15 - num49, num3 - 1, apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], num47);
						g.SetColor(Graphics.GetColorOfRGB(108, 94, 86));
						g.DrawRect(num4 + 31, num3 + 1, 51f, 10f);
						g.SetColor(Graphics.GetColorOfRGB(57, 57, 70));
						g.FillRect(num4 + 31 + 1, num3 + 1 + 1, 50f, 9f);
						int phyParamBarW2 = PlayerData.GetPhyParamBarW(playerData6.paramPhysical_[num47], 50);
						if (num47 == 0) { g.SetColor(Graphics.GetColorOfRGB(252, 54, 0)); }
else if (num47 == 1) { g.SetColor(Graphics.GetColorOfRGB(84, 188, 255)); }
else if (num47 == 2) { g.SetColor(Graphics.GetColorOfRGB(103, 255, 121)); }
else if (num47 == 3) { g.SetColor(Graphics.GetColorOfRGB(248, 248, 248)); }
else if (num47 == 4) { g.SetColor(Graphics.GetColorOfRGB(255, 251, 0)); }
						g.FillRect(num4 + 32, num3 + 2, phyParamBarW2, 9f);
					}
					if (udArrowCnt > 0)
					{
						udArrowCnt--;
					}
					num4 = 120;
					num = 70;
					if (udArrowCnt == 0)
					{
						GameView.AddTouch(g, 2, num4 - 25 - 5, num - 50, 70, 30, 1376456, TouchOption.Create(2));
						GameView.AddTouch(g, 2, num4 - 25 - 5, num + 100, 70, 30, 1376456, TouchOption.Create(2));

} else {
					TouchOption touchOption = TouchOption.Create(2);
					TouchOption touchOption2 = TouchOption.Create(2);
					GameView.AddTouch(g, 24, num4, num - 13, apdat_.resCom2_.seb[24], 0, 17, touchOption.Margin(40, 40, 45, 0));
					GameView.AddTouch(g, 24, num4, num + 64 + 13, apdat_.resCom2_.seb[24], 1, 19, touchOption2.Margin(40, 40, -15, 60));

}}
				else
				{
					_draw5(g);
				}
			}
		}

		private void _draw5(Graphics g)
		{
int coalescedValue, num, num2, num3;
coalescedValue = num = num2 = num3 = 0;




			Font font = g.GetFont(null);
			if (type_ == 54)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "選手の獲得");
				apdat_.DrawBox(g, 12, 136, 227, 205);
				apdat_.DrawBox(g, 97, 53, 227, 134);
				TradeData tradeData = tradeData_;
				PlayerData playerData = apdat_.pData_[tradeData.playerDataId_];
				g.DrawImage(apdat_.resEvent_.img[8], 19f, 73f);
				udat_.SetDispPlayerData(0, 0, playerData.faceImg_, 0, 1);
				udat_.DrawDispPlayer(g, 55, 128);
				coalescedValue = 53;
				num = 95;
				int num4 = 0;
				if (En())
				{
					num4 = 6;
				}
				int feelPt_ = tradeData.feelPt_;
				apdat_.DrawFukidashiKimochi(g, coalescedValue, num, feelPt_, (feelPt_ == 100) ? (35) : (78));
				coalescedValue = 31;
				num = 133;
				g.DrawImage(apdat_.resCom2_.img[126], coalescedValue, num);
				apdat_.DrawNumImage(g, 79, playerData.lv_, coalescedValue + 28, num - 2, 0, 2);
				coalescedValue = 11;
				num = 66;
				g.SetColor(Graphics.GetColorOfRGB(197, 220, 224));
				g.DrawRect(coalescedValue, num, 83f, 16f);
				g.SetColor(Graphics.GetColorOfRGB(253, 255, 255));
				g.FillRect(coalescedValue + 1, num + 1, 82f, 15f);
				g.SetColor(SC_WINDOW_BLUE);
				SetFontSize(g, 10);
				if (En())
				{
					g.DrawString("給料", coalescedValue + 4, num + 3 + 1);
				}
				else
				{
					g.DrawString("給料", coalescedValue + 4, num + 3);
				}
				ResetFontSize(g);
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(MyFormBase.YEN("万", playerData.GetSallay()), coalescedValue + 80, num + 3, 4);
				num2 = 100;
				num3 = 66;
				for (int i = 0; i < 4; i++)
				{
					g.DrawImage(apdat_.resCom2_.img[123], num2 + 31 * i, num3);
					g.SetColor(SC_WINDOW_BLACK);
					int positionRank = PlayerData.GetPositionRank(i, playerData);
					apdat_.DrawPositionStatus(g, i, positionRank, num2 + 31 * i + 4, num3 + 4);
				}
				coalescedValue = 120;
				num = 85;
				g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
				g.DrawRect(coalescedValue, num, 83f, 56f);
				int[] array = new int[5] { 8, 11, 13, 9, 11 };
				for (int j = 0; j < PlayerData.PARAM_NAME.Length; j++)
				{
					num3 = num + 11 * j;
					int num5 = 12;
					if (j == PlayerData.PARAM_NAME.Length - 1)
					{
						num5 = 13;
					}
					g.SetColor(Graphics.GetColorOfRGB(214, 236, 244));
					g.DrawRect(coalescedValue, num3, 30f, num5 - 1);
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 223));
					g.FillRect(coalescedValue + 1, num3 + 1, 29f, num5 - 2);
					int num6 = array[j];
					if (AppData.IsLanguageJpBase(false))
					{
						num6 = 12;
					}
					apdat_.DrawSeb(g, coalescedValue + 15 - num6, num3 - 1, apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], j);
					g.SetColor(Graphics.GetColorOfRGB(108, 94, 86));
					g.DrawRect(coalescedValue + 31, num3 + 1, 51f, 10f);
					g.SetColor(Graphics.GetColorOfRGB(57, 57, 70));
					g.FillRect(coalescedValue + 31 + 1, num3 + 1 + 1, 50f, 9f);
					int phyParamBarW = PlayerData.GetPhyParamBarW(playerData.paramPhysical_[j], 50);
					if (j == 0) {
g.SetColor(Graphics.GetColorOfRGB(252, 54, 0));
} else if (j == 1) {
g.SetColor(Graphics.GetColorOfRGB(84, 188, 255));
} else if (j == 2) {
g.SetColor(Graphics.GetColorOfRGB(103, 255, 121));
} else if (j == 3) {
g.SetColor(Graphics.GetColorOfRGB(248, 248, 248));
} else if (j == 4) {
g.SetColor(Graphics.GetColorOfRGB(255, 251, 0));
}
coalescedSwitchExit177847: ;
					g.FillRect(coalescedValue + 32, num3 + 2, phyParamBarW, 9f);
				}
				g.SetColor(SC_WINDOW_BLACK);
				if (MyFormBase.Jp())
				{
					g.DrawString(MyFormBase.LT("<0><1>さんの交渉に向かいます", playerData.name_ + "\t" + playerData.name2_), 120f, 157f, 2);
				}
				else
				{
					g.DrawString(MyFormBase.LT("<0><1>さんの交渉に向かいます", global::System.String.Concat(playerData.name_, "\t", " ", playerData.name2_)), 120f, 157f, 2);
				}
				string text = MyFormBase.YEN("万", tradeData.cost_);
				if (!apdat_.ChPayMoney(tradeData.cost_))
				{
					text = "<co=FF6419>" + text + "</co>";
				}
				g.DrawString2(MyFormBase.LT("<co=4B8FFF>費用</co> <0>", text), 120f, 177f, 2);
				num3 = 195;
				num = 193;
				int num7 = 40;
				if (select_ == 0)
				{
					Draw_btmMsg(g, "いざ出陣", 120 - num7, num, true, true);
					Draw_btmMsg(g, "考え直す", 120 + num7, num, false, false);
				}
				else if (select_ == 1)
				{
					Draw_btmMsg(g, "いざ出陣", 120 - num7, num, false, false);
					Draw_btmMsg(g, "考え直す", 120 + num7, num, true, true);
				}
				num7 = 58;
				for (int k = 0; k < 2; k++)
				{
					num2 = 75 - num7 / 2 + 90 * k;
					GameView.AddTouch(g, 3, num2 + DLGSEL_TRECT[k][0], num3 - 2 + DLGSEL_TRECT[k][1], num7 + DLGSEL_TRECT[k][2], 16 + DLGSEL_TRECT[k][3], 0x20000 | k);
				}
			}
			else if (type_ == 55)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "選手獲得交渉");
				apdat_.DrawBox(g, 12, 136, 227, 205);
				TradeData tradeData2 = tradeData_;
				PlayerData playerData2 = apdat_.pData_[tradeData2.playerDataId_];
				coalescedValue = 30;
				num = 64;
				g.DrawImage(apdat_.resEvent_.img[7], coalescedValue, num);
				coalescedValue = 16;
				num = 70;
				g.DrawImage(apdat_.resEvent_.img[8], coalescedValue, num);
				Draw_chara(g, 172, 0, 0, frame_, coalescedValue + 26, num + 63);
				Draw_chara(g, 171, 0, 0, frame_, coalescedValue + 44, num + 63);
				int[] feelPtEfCommentParam = UserData.GetFeelPtEfCommentParam(frame_ - SP_ANIME_TIME[0], SP_ANIME_TIME[1] - SP_ANIME_TIME[0]);
				if (feelPtEfCommentParam[0] == 1)
				{
					string text2 = UserData.EF_FEELPT_COMMENT_[feelPtEfCommentParam[1]][feelPtEfCommentParam[2]];
					apdat_.DrawFukidashiBalloon(g, text2, coalescedValue + 33, num + 24, SC_WINDOW_BLUE);
				}
				coalescedValue = 155;
				num = 70;
				g.DrawImage(apdat_.resEvent_.img[8], coalescedValue, num);
				udat_.SetDispPlayerData(0, 0, playerData2.faceImg_, 0, 2);
				udat_.DrawDispPlayer(g, coalescedValue + 35, num + 63);
				int feelPtEf = UserData.GetFeelPtEf(frame_ - SP_ANIME_TIME[0], SP_ANIME_TIME[1] - SP_ANIME_TIME[0]);
				apdat_.DrawFukidashiKimochi(g, 189, 94, feelPtEf, 78);
				int[] addFeelPtEf = UserData.GetAddFeelPtEf(frame_ - SP_ANIME_TIME[0], SP_ANIME_TIME[1] - SP_ANIME_TIME[0]);
				if (addFeelPtEf[0] >= 1)
				{
					int num8 = 0;
					int num9 = 78;
					int num10 = 105;
					if (addFeelPtEf[0] == 2)
					{
						num8 = frame_ % 8 / 4 * -2;
						num9 = 35;
						num10 = 103;
					}
					int num11 = AppData.GetFig(addFeelPtEf[1]) * 8 + 8 + 2;
					num3 = 76;
					num2 = 208;
					apdat_.DrawNumImage(g, num9, addFeelPtEf[1], num2, num3 + num8, 0, 4);
					apdat_.DrawSeb(g, num2 - num11, num3 + num8, apdat_.resCom2_.seb[num9], apdat_.resCom2_.img[num10], 14);
				}
				num = 153;
				if (frame_ < SP_ANIME_TIME[1])
				{
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.LT("<0>と交渉中", playerData2.name_), 120f, num, 2);
				}
				else
				{
					int num12 = GameUtil.RateConvert(frame_, SP_ANIME_TIME[2], SP_ANIME_TIME[3], 0, 14, true);
					g.ClipRect(0f, num, 240f, num12);
					string feelPtEfRetStr = UserData.GetFeelPtEfRetStr(UserData.NEGOTIATE_RESULT_DISPVALUE[3]);
					if (UserData.NEGOTIATE_RESULT_DISPVALUE[3] < 100)
					{
						g.SetColor(SC_WINDOW_BLACK);
					}
					else
					{
						g.SetColor(SC_WINDOW_BLUE);
					}
					g.DrawString(feelPtEfRetStr, 120f, num, 2);
					g.PopClip();
				}
				num2 = 41;
				num3 = 169;
				int num13 = GameUtil.RateConvert(frame_, SP_ANIME_TIME[0], SP_ANIME_TIME[1], 0, 156, true);
				g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
				g.FillRect(num2, num3 + 1, 158f, 10f);
				g.SetColor(Graphics.GetColorOfRGB(115, 175, 188));
				g.DrawRect(num2, num3 + 1, 157f, 9f);
				g.SetColor(Graphics.GetColorOfRGB(0, 254, 0));
				g.FillRect(num2 + 1, num3 + 2, num13, 8f);
				num3 = 206;
				g.ClipRect(0f, num3 - 20, 240f, 20f);
				for (int l = 0; l < UserData.NEGOTIATE_RESULT_TH.Length; l++)
				{
					num2 = 70 + 50 * l;
					int num14 = 0;
					int frame = UserData.NEGOTIATE_RESULT_TH[l];
					int num15 = (SP_ANIME_TIME[1] - SP_ANIME_TIME[0]) * UserData.WAIT_TIME[l] / 100 + SP_ANIME_TIME[0] - 5;
					num14 = GameUtil.RateConvert(frame_ - num15, 0, 10, -20, 0, true);
					if (num14 <= -20) { } else {
						apdat_.DrawSeb(g, num2, num3 + num14, apdat_.resCom2_.seb[97], apdat_.resCom2_.img[175], frame);
					}
				}
				g.PopClip();
				GameView.AddTouch(2);
			}
			else if (type_ == 56)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "イベント");
				apdat_.DrawBox(g, 12, 136, 227, 205);
				TradeData tradeData3 = tradeData_;
				PlayerData playerData3 = apdat_.pData_[tradeData3.playerDataId_];
				g.DrawImage(apdat_.resEvent_.img[9], 30f, 64f);
				udat_.SetDispPlayerData(15, udat_.selectUniform_, playerData3.faceImg_, 0, 1);
				udat_.DrawDispPlayer(g, 120, 123);
				g.DrawImage(apdat_.resEvent_.img[9], 111f, 120f, 81, 56, 20, 7);
				int num16 = GameUtil.RateConvert(frame_, 0, 50, 0, 50, true);
				g.ClipRect(0f, 157f, 240f, num16);
				g.SetColor(Graphics.GetColorOfRGB(0, 114, 255));
				if (MyFormBase.Jp())
				{
					g.DrawString(MyFormBase.LT("<0><1>さんの獲得を発表!!", playerData3.name_ + "\t" + playerData3.name2_), 120f, 166f, 2);
				}
				else
				{
					g.DrawString(MyFormBase.LT("<0><1>さんの獲得を発表!!", global::System.String.Concat(playerData3.name_, "\t", " ", playerData3.name2_)), 120f, 166f, 2);
				}
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(MyFormBase.LT("知名度が<0>ｱｯﾌﾟしました!", tradeData3.getNameValue_), 120f, 187f, 2);
				g.PopClip();
				GameView.AddTouch(2);
			}
			else if (type_ == 57)
			{
				int num17 = 192;
				num = 120 - num17 / 2;
				int num18 = 60;
				coalescedValue = 120 - num18 / 2;
				g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
				g.FillRect(coalescedValue, num, num18, num17);
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				int[][] array2 = null;
				array2 = apdat_.formationData_[udat_.et_cur[1]].formation_;
				for (int m = 0; m < array2.Length; m++)
				{
					num3 = num + 5 + 17 * m;
					g.DrawString(array2[m][0] + ", " + array2[m][1], 120f, num3, 2);
				}
			}
			else if (type_ == 58)
			{
				apdat_.DrawWindow(g, 228, 204, 0, "カップ戦");
				apdat_.DrawBox(g, 15, 55, 225, 201);
				coalescedValue = 20;
				num = 63;
				num3 = num - 12;
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("大会の名前", 36f, num3);
				if (En())
				{
					g.DrawString("ﾗﾝｸ", 135f, num3);
					g.DrawString("費用", 187f, num3);
				}
				else
				{
					g.DrawString("ﾗﾝｸ", 133f, num3);
					g.DrawString("費用", 183f, num3);
				}
				int num19 = 23;
				for (int n = scrollValue_; n < ary1.Size(); n++)
				{
					int num20 = n - scrollValue_;
					if (num20 >= 6)
					{
						break;
					}
					QuestMatchData questMatchData = (QuestMatchData)ary1.ElementAt(n);
					num2 = coalescedValue;
					num3 = 69 + num20 * num19;
					int trophy_ = questMatchData.trophy_;
					apdat_.DrawSeb(g, num2 - 3, num3 + 3, apdat_.resCom2_.seb[95], apdat_.resCom2_.img[97], trophy_);
					if (n == select_)
					{
						g.SetColor(SC_WINDOW_SELECT);
						g.FillRect(num2 + 14, num3 + 4, 188f, 16f);
						apdat_.resCom2_.seb[60].Draw(g, num2 - 7, num3 + 13, apdat_.resCom2_.img);
					}
					GameView.AddTouch(g, 11, num2 + 14 - 24, num3 + 4, 236, 16, 0x20000 | n);
					g.SetColor(SC_WINDOW_BLACK);
					SetFontSize(g, 10);
					g.DrawString(questMatchData.name_, num2 + 16, num3 + 6);
					ResetFontSize(g);
					if (En())
					{
						apdat_.DrawSeb(g, num2 + 122 + 5 - 1, num3 + 18, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], questMatchData.rank_);
					}
					else
					{
						apdat_.DrawSeb(g, num2 + 122, num3 + 18, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], questMatchData.rank_);
					}
					int num21 = 198;
					if (apdat_.ChPayMoney(questMatchData.joinCost_) || questMatchData.joinCost_ == 0)
					{
						if (MyFormBase.Jp())
						{
							apdat_.DrawNumImageComma(g, 78, questMatchData.joinCost_, num2 + 195 - 16 - 1, num3 + 7, 0, 0, 4);
							apdat_.DrawSeb(g, num2 + 195 - 16, num3 + 7, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 13);
						}
						else
						{
							apdat_.DrawMoney(g, 78, questMatchData.joinCost_, num2 + num21, num3 + 7, 0, 0, 4);
						}
					}
					else if (MyFormBase.Jp())
					{
						apdat_.DrawNumImageComma(g, 91, questMatchData.joinCost_, num2 + 195 - 16 - 1, num3 + 7, 0, 0, 4);
						apdat_.DrawSeb(g, num2 + 195 - 16, num3 + 7, apdat_.resCom2_.seb[91], apdat_.resCom2_.img[109], 13);
					}
					else
					{
						apdat_.DrawMoney(g, 91, questMatchData.joinCost_, num2 + num21, num3 + 7, 0, 0, 4);
					}
				}
				num3 = 216;
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("参加する大会を選んでください", 120f, num3, 2);
				apdat_.DrawVerticalScroll2(g, coalescedValue + 207, num + 3, 3, 146, scrollValue_, ary1.Size() - 1, 6);
			}
			else if (type_ == 59)
			{
				apdat_.DrawWindow(g, 228, 204, 0, "カップ戦");
				apdat_.DrawBox(g, 15, 55, 225, 201);
				coalescedValue = 20;
				num = 63;
				QuestMatchData questMatchData2 = qmData_;
				if (scrollValue_ == 0)
				{
					if (MyFormBase.Jp())
					{
						num2 = 60;
						num3 = 47;
						apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[95], apdat_.resCom2_.img[97], questMatchData2.trophy_);
						num2 = 120;
						num3 = 51;
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString(questMatchData2.name_, num2, num3, 2);
						apdat_.DrawSeb(g, 172, 63, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], questMatchData2.rank_);
					}
					else
					{
						int num22 = 26 + font.StringWidth(questMatchData2.name_) + 5 + 10;
						num2 = 120 - num22 / 2;
						num3 = 47;
						apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[95], apdat_.resCom2_.img[97], questMatchData2.trophy_);
						num2 = 120;
						num3 = 51;
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString(questMatchData2.name_, num2, num3, 2);
						num2 = 120 + num22 / 2 - 10;
						apdat_.DrawSeb(g, num2, 63, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], questMatchData2.rank_);
					}
				}
				else
				{
					num3 = 51;
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("参戦チーム一覧", 120f, num3, 2);
				}
				if (scrollValue_ == 0)
				{
					apdat_.DrawBox(g, 20, 138, 221, 180);
					num2 = 57;
					if (En())
					{
						num2 += -10;
					}
					num3 = 82;
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("優勝賞金", num2, num3);
					if (MyFormBase.Jp())
					{
						apdat_.DrawNumImageComma(g, 35, questMatchData2.victoryPrize_, 172, num3 + 1, 0, 0, 4);
						apdat_.DrawSeb(g, 173, num3 + 1, apdat_.resCom2_.seb[35], apdat_.resCom2_.img[103], 13);
					}
					else
					{
						apdat_.DrawMoney(g, 35, questMatchData2.victoryPrize_, 194, num3 + 1, 0, 0, 4);
					}
					coalescedValue = 24;
					num = 108;
					string[] array3 = new string[4] { "難度", "費用", "対戦", "日程" };
					for (int num23 = 0; num23 < 4; num23++)
					{
						num2 = coalescedValue + num23 / 2 * 97;
						num3 = num + num23 % 2 * 19;
						g.SetColor(Graphics.GetColorOfRGB(222, 234, 225));
						g.DrawRect(num2, num3, 96f, 19f);
						g.SetColor(Graphics.GetColorOfRGB(151, 151, 151));
						SetFontSize(g, 10);
						if (En())
						{
							g.DrawString(array3[num23], num2 + 8 - 4, num3 + 5 + 1);
						}
						else
						{
							g.DrawString(array3[num23], num2 + 8, num3 + 5);
						}
						ResetFontSize(g);
						if (num23 == 0) {
if (En())
							{
								apdat_.DrawSeb(g, num2 + 75, num3 + 17, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], questMatchData2.rank_);
							}
							else
							{
								apdat_.DrawSeb(g, num2 + 62, num3 + 17, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], questMatchData2.rank_);
							}
} else if (num23 == 1) {
g.SetColor(SC_WINDOW_BLACK);
g.DrawString(MyFormBase.YEN("万", questMatchData2.joinCost_), num2 + 93, num3 + 5, 4);
} else if (num23 == 2) {
g.SetColor(SC_WINDOW_BLACK);
if (AppData.IsLanguageJpBase(true))
							{
								g.DrawString("全", num2 + 38 + 10, num3 + 4, 4);
								g.DrawString("試合", num2 + 68, num3 + 4);
								apdat_.DrawNumImage(g, 35, questMatchData2.matchEnemy_.Length, num2 + 66, num3 + 5, 0, 4);
							}
							else
							{
								SetFontSize(g, 6);
								g.DrawString("全", num2 + 38 - 5, num3 + 4 + 6);
								ResetFontSize(g);
								apdat_.DrawNumImage(g, 35, questMatchData2.matchEnemy_.Length, num2 + 64 - 5 + 28, num3 + 5, 0, 4);
							}
} else if (num23 == 3) {
g.SetColor(SC_WINDOW_BLACK);
if (AppData.IsLanguageJpBase(true))
							{
								g.DrawString("週後", num2 + 68, num3 + 5);
								apdat_.DrawNumImage(g, 35, questMatchData2.schedule_, num2 + 66, num3 + 6, 0, 4);
								goto coalescedSwitchExit190333;
							}
SetFontSize(g, 10);
g.DrawString("週後", num2 + 68 - 12 - 6, num3 + 5 + 6 - 4);
ResetFontSize(g);
apdat_.DrawNumImage(g, 35, questMatchData2.schedule_, num2 + 66 + 6, num3 + 6, 0, 4);
}
coalescedSwitchExit190333: ;
					}
					textLay_.Draw(g, 24f, 155f, 0);
					Draw_chara(g, 172, 0, 0, frame_, 32, 209);
					Draw_chara(g, 171, 0, 1, frame_, 209, 209);
					num3 = 195;
					if (questMatchData2.trophy_ == 4)
					{
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString("制覇済み", 120f, num3, 2);
					}
					else
					{
						g.SetColor(SC_WINDOW_GRAY);
						g.DrawString("未制覇", 120f, num3, 2);
					}
				}
				else
				{
					coalescedValue = 19;
					num = 74;
					for (int num24 = 0; num24 < 16; num24++)
					{
						num2 = coalescedValue + num24 % 2 * 101;
						num3 = num + num24 / 2 * 16;
						g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
						g.DrawRect(num2, num3, 74f, 16f);
						g.DrawRect(num2 + 75, num3, 26f, 16f);
						if (num24 >= questMatchData2.matchEnemy_.Length) { } else {
							TeamData teamData = apdat_.tData_[questMatchData2.matchEnemy_[num24]];
							g.SetColor(SC_WINDOW_BLACK);
							SetFontSize(g, 11);
							if (MyFormBase.Jp())
							{
								g.DrawString(MyFormBase.LT(teamData.name2_), num2 + 8, num3 + 3);
							}
							else
							{
								g.DrawString(MyFormBase.LT(teamData.name2_), num2 + 8 - 1, num3 + 3);
							}
							ResetFontSize(g);
							apdat_.DrawNumImage(g, 35, udat_.GetRankIndex(teamData.warPotential_) + 1, num2 + 89, num3 + 4, 0, 2);
						}
					}
					g.DrawImage(apdat_.resCom2_.img[57], 100f, 65f);
					g.DrawImage(apdat_.resCom2_.img[57], 201f, 65f);
				}
				num3 = 216;
				num = 214;
				int num25 = 40;
				if (page_ == 0)
				{
					Draw_btmMsg(g, "参戦する", 120 - num25, num, true, true);
					Draw_btmMsg(g, "考え直す", 120 + num25, num, false, false);
				}
				else if (page_ == 1)
				{
					Draw_btmMsg(g, "参戦する", 120 - num25, num, false, false);
					Draw_btmMsg(g, "考え直す", 120 + num25, num, true, true);
				}
				num25 = 58;
				for (int num26 = 0; num26 < 2; num26++)
				{
					num2 = 75 - num25 / 2 + 90 * num26;
					GameView.AddTouch(g, 3, num2 + DLGSEL_TRECT[num26][0], num3 - 2 + DLGSEL_TRECT[num26][1], num25 + DLGSEL_TRECT[num26][2], 16 + DLGSEL_TRECT[num26][3], 0x140000 | num26);
				}
				apdat_.DrawVerticalScroll2(g, 228, 66, 3, 147, scrollValue_, 1, 1);
			}
			else if (type_ == 60 || type_ == 61)
			{
				if (type_ == 60)
				{
					apdat_.DrawWindow(g, 228, 204, 0, "施設の強化");
				}
				else
				{
					apdat_.DrawWindow(g, 228, 204, 0, "施設の建設");
				}
				num = 50;
				if (En())
				{
					num++;
				}
				SetFontSize(g, 10);
				g.SetColor(SC_WINDOW_BLUE);
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("建設できる建物", 21f, num);
				if (type_ == 60)
				{
					g.DrawString("強化費用", 220f, num, 4);
				}
				else
				{
					g.DrawString("建設費用", 220f, num, 4);
				}
				ResetFontSize(g);
				g.DrawImage(apdat_.resCom2_.img[126], 131f, num + 1);
				num = 53;
				apdat_.DrawBox(g, 15, num, 224, num + 109);
				g.SetColor(SC_WINDOW_BLACK);
				num2 = 24;
				for (int num27 = scrollValue_; num27 < ary1.Size(); num27++)
				{
					int num28 = num27 - scrollValue_;
					if (num28 >= 6)
					{
						break;
					}
					num3 = num + 15 + num28 * 17;
					TenantData tenantData = (TenantData)ary1.ElementAt(num27);
					Draw_tenantBuildIcon(g, tenantData.icon_, num2, num3);
					if (num27 == select_)
					{
						g.SetColor(SC_WINDOW_SELECT);
						g.FillRect(num2 + 16 + 1, num3, 179f, 16f);
					}
					GameView.AddTouch(g, 11, num2 + 16 + 1 - 16, num3, 196, 16, 0x20000 | num27, TouchOption.Create(16).Margin(30, 0, 0, 0));
					if (tenantData.IsNew())
					{
						Draw_newMark(g, num2 - 7, num3 + 2, frame_);
					}
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(tenantData.name_, num2 + 18, num3 + 2);
					if (type_ == 60)
					{
						if (tenantData.state_ == 2)
						{
							g.SetColor(SC_WINDOW_BLACK);
							SetFontSize(g, 10);
							if (En())
							{
								g.DrawString("強化中", 139f, num3 + 2 + 1);
							}
							else
							{
								g.DrawString("強化中", 139f, num3 + 2);
							}
							ResetFontSize(g);
							int num29 = GameUtil.RateConvert(tenantData.stateCnt_, 0, 200, 0, 100, true);
							if (En())
							{
								apdat_.DrawNumImage(g, 35, num29, num2 + 175 + 8, num3 + 3, 0, 4);
								apdat_.DrawSeb(g, num2 + 175 + 1 + 8, num3 + 3, apdat_.resCom2_.seb[90], apdat_.resCom2_.img[131], 1);
							}
							else
							{
								apdat_.DrawNumImage(g, 35, num29, num2 + 175, num3 + 3, 0, 4);
								apdat_.DrawSeb(g, num2 + 175 + 1, num3 + 3, apdat_.resCom2_.seb[90], apdat_.resCom2_.img[131], 1);
							}
						}
						else
						{
							apdat_.DrawNumImage(g, 79, tenantData.lv_, num2 + 93 + 22, num3 + 4 - 2, 0, 2);
							if (tenantData.IsMaxLv()) { } else {
								if (apdat_.ChPayMoney(tenantData.paramStrengthCost_))
								{
									if (MyFormBase.Jp())
									{
										apdat_.DrawNumImageComma(g, 78, tenantData.paramStrengthCost_, num2 + 195 - 12 - 1, num3 + 3, 0, 0, 4);
										apdat_.DrawSeb(g, num2 + 195 - 12, num3 + 3, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 17);
									}
									else
									{
										apdat_.DrawMoney(g, 78, tenantData.paramStrengthCost_, num2 + 192, num3 + 3, 0, 0, 4);
									}
								}
								else if (MyFormBase.Jp())
								{
									apdat_.DrawNumImageComma(g, 91, tenantData.paramStrengthCost_, num2 + 195 - 12 - 1, num3 + 3, 0, 0, 4);
									apdat_.DrawSeb(g, num2 + 195 - 12, num3 + 3, apdat_.resCom2_.seb[91], apdat_.resCom2_.img[109], 17);
								}
								else
								{
									apdat_.DrawMoney(g, 91, tenantData.paramStrengthCost_, num2 + 192, num3 + 3, 0, 0, 4);
								}
							}
						}
					}
					else
					{
						apdat_.DrawNumImage(g, 79, tenantData.lv_, num2 + 93 + 22, num3 + 4 - 2, 0, 2);
						if (apdat_.ChPayMoney(tenantData.buildCost_))
						{
							if (MyFormBase.Jp())
							{
								apdat_.DrawNumImageComma(g, 78, tenantData.buildCost_, num2 + 195 - 12 - 1, num3 + 3, 0, 0, 4);
								apdat_.DrawSeb(g, num2 + 195 - 12, num3 + 3, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 17);
							}
							else
							{
								apdat_.DrawMoney(g, 78, tenantData.buildCost_, num2 + 192, num3 + 3, 0, 0, 4);
							}
						}
						else if (MyFormBase.Jp())
						{
							apdat_.DrawNumImageComma(g, 91, tenantData.buildCost_, num2 + 195 - 12 - 1, num3 + 3, 0, 0, 4);
							apdat_.DrawSeb(g, num2 + 195 - 12, num3 + 3, apdat_.resCom2_.seb[91], apdat_.resCom2_.img[109], 17);
						}
						else
						{
							apdat_.DrawMoney(g, 91, tenantData.buildCost_, num2 + 192, num3 + 3, 0, 0, 4);
						}
					}
					if (num27 == select_)
					{
						apdat_.resCom2_.seb[60].Draw(g, num2 - 6, num3 + 9, apdat_.resCom2_.img);
					}
				}
				apdat_.DrawVerticalScroll2(g, 226, 64, 3, 110, scrollValue_, ary1.Size() - 1, 6);
				coalescedValue = 33;
				num = 168;
				apdat_.DrawBox(g, 15, num, 224, num + 46);
				TenantData tenantData2 = (TenantData)ary1.ElementAt(select_);
				if (!tenantData2.IsTAreaOrStadiumTenant())
				{
					num2 = 24;
					num3 = 190;
					g.SetColor(SC_WINDOW_BLUE);
					SetFontSize(g, 10);
					if (En())
					{
						g.DrawString("選手への影響", num2, num3 + 1);
						g.DrawString("一回の使用料", num2, num3 + 16 + 1);
					}
					else
					{
						g.DrawString("選手への影響", num2, num3);
						g.DrawString("一回の使用料", num2, num3 + 16);
					}
					ResetFontSize(g);
					g.SetColor(SC_WINDOW_BLACK);
					int fanToEf = TenantData.GetFanToEf(tenantData2.efToFan_[1]);
					if (En())
					{
						g.DrawString(TenantData.EFTOFAN_STR[fanToEf], num2 + 112, num3, 4);
						g.DrawString(MyFormBase.YEN("万", tenantData2.paramUsePrice_), num2 + 112, num3 + 16, 4);
					}
					else
					{
						g.DrawString(TenantData.EFTOFAN_STR[fanToEf], num2 + 96, num3, 2);
						g.DrawString(MyFormBase.YEN("万", tenantData2.paramUsePrice_), num2 + 108, num3 + 16, 4);
					}
					coalescedValue = 141;
					num = 182;
					g.SetColor(Graphics.GetColorOfRGB(255, 229, 149));
					g.DrawRect(coalescedValue, num, 79f, 39f);
					g.SetColor(Graphics.GetColorOfRGB(253, 252, 202));
					g.FillRect(coalescedValue + 1, num + 1, 78f, 38f);
					g.DrawImage(apdat_.resCom2_.img[170], 153f, 174f);
					coalescedValue += 5;
					num += 4;
					for (int num30 = 0; num30 < 4; num30++)
					{
						num2 = coalescedValue + 37 * (num30 / 2);
						num3 = num + 18 * (num30 % 2);
						apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], num30);
						apdat_.DrawNumImage(g, 78, tenantData2.paramResearch_[num30], num2 + 26, num3 + 3, 0, 2);
					}
				}
				else
				{
					textLay_.Draw(g, 25f, 185f, 0);
				}
				if (udArrowCnt > 0)
				{
					udArrowCnt--;
				}
				coalescedValue = 120;
				num = 70;
				if (udArrowCnt == 0) {
					GameView.AddTouch(g, 2, coalescedValue - 25 - 5, num - 50, 70, 30, 1376456, TouchOption.Create(2));
					GameView.AddTouch(g, 2, coalescedValue - 25 - 5, num + 114, 70, 30, 1376456, TouchOption.Create(2));

} else {
				TouchOption touchOption = TouchOption.Create(2);
				TouchOption touchOption2 = TouchOption.Create(2);
				GameView.AddTouch(g, 24, coalescedValue, num - 13, apdat_.resCom2_.seb[24], 0, 17, touchOption.Margin(40, 40, 45, 0));
				GameView.AddTouch(g, 24, coalescedValue, num + 78 + 13, apdat_.resCom2_.seb[24], 1, 19, touchOption2.Margin(40, 40, -15, 60));
}
			}
			else if (type_ == 62 || type_ == 63)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "確認");
				apdat_.DrawBox(g, 12, 69, 230, 188);
				TenantData tenantData3 = apdat_.tenantData_[tenantDataId_];
				coalescedValue = 17;
				num = 66;
				g.DrawImage(apdat_.resCom2_.img[126], coalescedValue, num);
				apdat_.DrawNumImage(g, 79, tenantData3.lv_, coalescedValue + 24, num - 2, 0, 2);
				coalescedValue = 52;
				num = 65;
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(tenantData3.name_, coalescedValue, num);
				if (!tenantData3.IsTAreaOrStadiumTenant())
				{
					g.SetColor(SC_WINDOW_BLUE);
					SetFontSize(g, 10);
					if (En())
					{
						g.DrawString("維持費", 149f, num + 1);
					}
					else
					{
						g.DrawString("維持費", 149f, num);
					}
					ResetFontSize(g);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.YEN("万", tenantData3.paramKeepCost_), 225f, num, 4);
				}
				coalescedValue = 59;
				num = 83;
				g.SetColor(Graphics.GetColorOfRGB(181, 219, 166));
				g.DrawRect(coalescedValue, num, 121f, 75f);
				g.SetColor(Graphics.GetColorOfRGB(219, 249, 207));
				g.FillRect(coalescedValue + 1, num + 1, 120f, 74f);
				g.ClipRect(coalescedValue, num, 122f, 76f);
				coalescedValue += 31;
				num += 16;
				if (!tenantData3.IsTAreaOrStadiumTenant())
				{
					for (int num31 = 0; num31 < 4; num31++)
					{
						num2 = coalescedValue + num31 / 2 * 30 - num31 % 2 * 30;
						num3 = num + num31 / 2 * 15 + num31 % 2 * 15;
						MapchipMain mapchipMain = null;
						if (tenantData3 != null) { mapchipMain = apdat_.mapchipMain_[tenantData3.img_[num31]]; } else { mapchipMain = apdat_.mapchipMain_[MapMain.mapchip_vacant.id_ + num31]; }
						apdat_.DrawSeb(g, num2, num3, apdat_.resMapChipMain_.seb[mapchipMain.seb_], apdat_.resMapChipMain_.img[mapchipMain.img_], 0);
					}
				}
				else if (tenantData3.id_ == TenantData.TENANT_ID_STADIUM)
				{
					num2 = 120;
					num3 = 140;
					int num32 = 0;
					TeamData teamData2 = apdat_.tData_[0];
					if ((teamData2.stadium_ == 1)) { num32 = (0); } else { if (teamData2.stadium_ == 0) { num32 = 2; } else { num32 = 2; } }
					apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[101 + num32], apdat_.resCom2_.img[185 + num32], 0);
				}
				else if (tenantData3.id_ == TenantData.TENANT_ID_TRAININGAREA || tenantData3.id_ == TenantData.TENANT_ID_TRAININGAREA_SPRED)
				{
					num2 = 59;
					num3 = 89;
					g.DrawImage(apdat_.resCom2_.img[162], num2, num3, 0, 16, 122, 64);
					g.SetColor(Graphics.GetColorOfRGB(144, 165, 163));
					g.DrawRect(num2, num3, 121f, 63f);
				}
				g.PopClip();
				if (type_ == 62)
				{
					int num33 = 0;
					for (int num34 = 0; num34 < udat_.tenants_.Length; num34++)
					{
						Tenant tenant = udat_.tenants_[num34];
						if (tenant.tenantDataId_ == tenantData3.id_)
						{
							num33++;
						}
					}
					if (num33 == 0)
					{
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString("敷地内にまだありません", 120f, 164f, 2);
					}
					else
					{
						g.SetColor(SC_WINDOW_GRAY);
						g.DrawString(MyFormBase.LT("敷地内に<0>軒建設済み", num33), 120f, 164f, 2);
					}
					g.SetColor(SC_WINDOW_BLACK);
					string text3 = MyFormBase.YEN("万円", tenantData3.buildCost_);
					if (!apdat_.ChPayMoney(tenantData3.buildCost_))
					{
						text3 = "<co=FF6419>" + text3 + "</co>";
					}
					g.DrawString2(MyFormBase.LT("<co=4B8FFF>費用</co>  <0>", text3), 120f, 182f, 2);
				}
				else if (!tenantData3.IsMaxLv())
				{
					coalescedValue = 71;
					num = 165;
					g.DrawImage(apdat_.resCom2_.img[126], coalescedValue, num);
					apdat_.DrawNumImage(g, 79, tenantData3.lv_, coalescedValue + 24, num - 2, 0, 2);
					coalescedValue = 110;
					num = 169;
					apdat_.DrawSeb(g, coalescedValue, num, apdat_.resCom2_.seb[24], apdat_.resCom2_.img[27], 1);
					apdat_.DrawSeb(g, coalescedValue + 9, num, apdat_.resCom2_.seb[24], apdat_.resCom2_.img[27], 1);
					apdat_.DrawSeb(g, coalescedValue + 18, num, apdat_.resCom2_.seb[24], apdat_.resCom2_.img[27], 1);
					coalescedValue = 138;
					num = 165;
					g.DrawImage(apdat_.resCom2_.img[126], coalescedValue, num);
					apdat_.DrawNumImage(g, 79, tenantData3.lv_ + 1, coalescedValue + 24, num - 2, 0, 2);
					g.SetColor(SC_WINDOW_BLACK);
					string text4 = MyFormBase.YEN("万円", tenantData3.paramStrengthCost_);
					if (!apdat_.ChPayMoney(tenantData3.paramStrengthCost_))
					{
						text4 = "<co=FF6419>" + text4 + "</co>";
					}
					g.DrawString2(MyFormBase.LT("<co=4B8FFF>費用</co>  <0>", text4), 120f, 182f, 2);
				}
				else
				{
					coalescedValue = 105;
					num = 165;
					g.DrawImage(apdat_.resCom2_.img[126], coalescedValue, num);
					apdat_.DrawNumImage(g, 79, tenantData3.lv_, coalescedValue + 24, num - 2, 0, 2);
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("最大LVです", 120f, 182f, 2);
				}
				num3 = 203;
				if (!tenantData3.IsMaxLv())
				{
					string empty = string.Empty;
					if ((type_ == 62)) { empty = ("建設する"); } else { empty = ("投資する"); }
					num = 201;
					int num35 = 40;
					if (select_ == 0)
					{
						Draw_btmMsg(g, empty, 120 - num35, num, true, true);
						Draw_btmMsg(g, "やめておく", 120 + num35, num, false, false);
					}
					else if (select_ == 1)
					{
						Draw_btmMsg(g, empty, 120 - num35, num, false, false);
						Draw_btmMsg(g, "やめておく", 120 + num35, num, true, true);
					}
					num35 = 58;
					for (int num36 = 0; num36 < 2; num36++)
					{
						num2 = 75 - num35 / 2 + 90 * num36;
						GameView.AddTouch(g, 3, num2 + DLGSEL_TRECT[num36][0], num3 - 2 + DLGSEL_TRECT[num36][1], num35 + DLGSEL_TRECT[num36][2], 16 + DLGSEL_TRECT[num36][3], 0x20000 | num36);
					}
				}
				else
				{
					num = num3 - 2;
					g.SetColor(SC_WINDOW_SELECT);
					g.FillRect(100f, num, 40f, 16f);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString("了解", 120f, num + 2, 2);
					apdat_.resCom2_.seb[60].Draw(g, 96f, num + 9, apdat_.resCom2_.img);
					GameView.AddTouch(2);
				}
			}
			else
			{
				_draw6(g);
			}
		}

		private void _draw6(Graphics g)
		{
			int num4;
int num;
int num2;
int num3;


			num4 = num = num2 = num3 = 0;
			Font font = g.GetFont(null);
			if (type_ == 64)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "施設のLVUP");
				apdat_.DrawBox(g, 12, 136, 227, 205);
				TenantData tenantData = apdat_.tenantData_[tenantDataId_];
				num4 = 60;
				num = 66;
				g.SetColor(Graphics.GetColorOfRGB(181, 219, 166));
				g.DrawRect(num4, num, 121f, 75f);
				g.SetColor(Graphics.GetColorOfRGB(219, 249, 207));
				g.FillRect(num4 + 1, num + 1, 120f, 74f);
				g.ClipRect(num4, num, 122f, 76f);
				num4 += 31;
				num += 16;
				for (int i = 0; i < 4; i++)
				{
					num2 = num4 + i / 2 * 30 - i % 2 * 30;
					num3 = num + i / 2 * 15 + i % 2 * 15;
					MapchipMain mapchipMain = null;
					if (tenantData != null) { mapchipMain = apdat_.mapchipMain_[tenantData.img_[i]]; } else { mapchipMain = apdat_.mapchipMain_[MapMain.mapchip_vacant.id_ + i]; }
					apdat_.DrawSeb(g, num2, num3, apdat_.resMapChipMain_.seb[mapchipMain.seb_], apdat_.resMapChipMain_.img[mapchipMain.img_], 0);
				}
				g.PopClip();
				if (page_ == 0)
				{
					num = 156;
					int num5 = GameUtil.RateConvert(frame_, TLUP_ANIME_TIME[0], TLUP_ANIME_TIME[1], 0, 12, true);
					g.ClipRect(0f, num, 240f, num5);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.LT("<0>のレベルが<1>になった!", tenantData.name_ + "\t" + (tenantData.lv_ + 1)), 120f, num, 2);
					g.PopClip();
					int num6 = 0;
					int num7 = 0;
					if (En())
					{
						num6 = -10;
						num7 = 10;
					}
					num4 = 53;
					num = 178;
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString("商品価格", num4 + num6, num);
					g.DrawString("維持費/月", num4 + num6, num + 16);
					int[] array = GetMeterParam(0);
					int[] array2 = GetMeterParam(1);
					int num8 = array[1];
					int num9 = array2[1];
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.YEN("万", num8), num4 + 105, num, 4);
					g.DrawString(MyFormBase.YEN("万", num9), num4 + 105, num + 16, 4);
					num = 178;
					int num10 = 0;
					int num11 = 0;
					for (int j = 0; j < 2; j++)
					{
						num2 = 188;
						num3 = num + 16 * j;
						num10 = j * 6;
						num11 = frame_ - TLUP_ANIME_TIME[2] - num10;
						if (num11 < 0) { } else {
							num11 = AppData.ClampMax(num11, 12);
							SetWinCharaJamp(num11, 0, -6, 12);
							int num12 = 0;
							if (j == 0) { num12 = array[6]; } else { num12 = array2[6]; }
							Draw_upParam(g, 35, num2 + num7, num3 + WIN_CHA[1], num12);
						}
					}
					if (frame_ >= TLUP_ANIME_TIME[5] && frame_ % 40 > 20)
					{
						apdat_.DrawSeb(g, 215, 207, apdat_.resCom2_.seb[63], apdat_.resCom2_.img[72], 0);
					}
				}
				else
				{
					num = 156;
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString("入手可能ﾎﾟｲﾝﾄ", 120f, num, 2);
					num4 = 45;
					num = 175;
					int num13 = 0;
					int num14 = 0;
					for (int k = 0; k < 4; k++)
					{
						num2 = num4 + 91 * (k / 2);
						num3 = num + 18 * (k % 2);
						int[] array3 = GetMeterParam(k + 2);
						int num15 = array3[1];
						apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], k);
						apdat_.DrawNumImage(g, 78, num15, num2 + 26, num3 + 3, 0, 2);
						num2 += 58;
						num3 += 3;
						num13 = k * 6;
						num14 = frame_ - TLUP_ANIME_TIME2[0] - num13;
						if (num14 < 0) { } else {
							num14 = AppData.ClampMax(num14, 12);
							SetWinCharaJamp(num14, 0, -6, 12);
							int cnt = array3[6];
							Draw_upParam(g, 35, num2, num3 + WIN_CHA[1], cnt);
						}
					}
					if (frame_ >= TLUP_ANIME_TIME2[3] && frame_ % 40 > 20)
					{
						apdat_.DrawSeb(g, 215, 207, apdat_.resCom2_.seb[63], apdat_.resCom2_.img[72], 0);
					}
				}
				GameView.AddTouch(2);
			}
			else if (type_ == 65)
			{
				int[] array4 = DrawWindow2(g, 228, 176, "施設情報", 5, 20, 20, 0, 4, 12);
				TenantData tenantData2 = null;
				if (tenantDataId_ != -1)
				{
					tenantData2 = apdat_.tenantData_[tenantDataId_];
				}
				num2 = 66;
				num3 = 64;
				g.DrawImage(apdat_.resCom2_.img[126], num2, num3 + 2);
				apdat_.DrawNumImage(g, 79, tenantData2.lv_, num2 + 20, num3, 0, 1);
				num2 = 121;
				num3 = 64;
				g.SetColor(SC_WINDOW_BLACK);
				if (tenantData2 != null)
				{
					g.DrawString(tenantData2.name_, num2, num3);
				}
				else
				{
					g.DrawString("空き地", num2, num3);
				}
				num2 = 21;
				num3 = 164;
				g.SetColor(SC_WINDOW_BLUE);
				SetFontSize(g, 10);
				if (En())
				{
					g.DrawString("商品価格", num2, num3 + 1);
				}
				else
				{
					g.DrawString("商品価格", num2, num3);
				}
				ResetFontSize(g);
				g.SetColor(SC_WINDOW_BLACK);
				if (En())
				{
					g.DrawString(MyFormBase.YEN("万", tenantData2.paramUsePrice_), num2 + 106 + 3, num3, 4);
				}
				else
				{
					g.DrawString(MyFormBase.YEN("万", tenantData2.paramUsePrice_), num2 + 106, num3, 4);
				}
				num2 = 21;
				num3 = 180;
				g.SetColor(SC_WINDOW_BLUE);
				SetFontSize(g, 10);
				if (En())
				{
					g.DrawString("維持費/月", num2, num3 + 1);
				}
				else
				{
					g.DrawString("維持費/月", num2, num3);
				}
				ResetFontSize(g);
				g.SetColor(SC_WINDOW_BLACK);
				if (En())
				{
					g.DrawString(MyFormBase.YEN("万", tenantData2.paramKeepCost_), num2 + 106 + 3, num3, 4);
				}
				else
				{
					g.DrawString(MyFormBase.YEN("万", tenantData2.paramKeepCost_), num2 + 106, num3, 4);
				}
				num4 = 14;
				num = 83;
				g.SetColor(Graphics.GetColorOfRGB(181, 219, 166));
				g.DrawRect(num4, num, 121f, 75f);
				g.SetColor(Graphics.GetColorOfRGB(219, 249, 207));
				g.FillRect(num4 + 1, num + 1, 120f, 74f);
				g.ClipRect(num4, num, 122f, 76f);
				num4 = 45;
				num = 99;
				for (int l = 0; l < 4; l++)
				{
					num2 = num4 + l / 2 * 30 - l % 2 * 30;
					num3 = num + l / 2 * 15 + l % 2 * 15;
					MapchipMain mapchipMain2 = null;
					if (tenantData2 != null) { mapchipMain2 = apdat_.mapchipMain_[tenantData2.img_[l]]; } else { mapchipMain2 = apdat_.mapchipMain_[MapMain.mapchip_vacant.id_ + l]; }
					apdat_.DrawSeb(g, num2, num3, apdat_.resMapChipMain_.seb[mapchipMain2.seb_], apdat_.resMapChipMain_.img[mapchipMain2.img_], 0);
				}
				g.PopClip();
				num4 = 138;
				num = 83;
				g.SetColor(Graphics.GetColorOfRGB(255, 229, 149));
				g.DrawRect(num4, num, 86f, 47f);
				g.SetColor(Graphics.GetColorOfRGB(253, 252, 202));
				g.FillRect(num4 + 1, num + 1, 85f, 46f);
				g.SetColor(SC_WINDOW_BLACK);
				SetFontSize(g, 10);
				if (En())
				{
					g.DrawString("選手への影響", num4 + 2 + 42, num + 5 + 1, 2);
				}
				else
				{
					g.DrawString("選手への影響", num4 + 2 + 42, num + 5, 2);
				}
				ResetFontSize(g);
				g.SetColor(SC_WINDOW_BLUE);
				int fanToEf = TenantData.GetFanToEf(tenantData2.paramEfToFan_);
				g.DrawString(TenantData.EFTOFAN_STR[fanToEf], num4 + 45, num + 20, 2);
				int width = GameUtil.RateConvert(tenantData2.paramEfToFan_, 0, 10, 0, 80, true);
				g.DrawImage(apdat_.resCom2_.img[132], num4 + 3, num + 35, 0, 0, 82, 10);
				g.DrawImage(apdat_.resCom2_.img[132], num4 + 3 + 1, num + 35 + 1, 1, 10, width, 8);
				num4 = 138;
				num = 132;
				g.SetColor(Graphics.GetColorOfRGB(255, 229, 149));
				g.DrawRect(num4, num, 86f, 61f);
				g.SetColor(Graphics.GetColorOfRGB(253, 252, 202));
				g.FillRect(num4 + 1, num + 1, 85f, 60f);
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString("入手可能ﾎﾟｲﾝﾄ", num4 + 4, num + 6);
				num4 = 145;
				num = 156;
				for (int m = 0; m < 4; m++)
				{
					num2 = num4 + 37 * (m / 2);
					num3 = num + 18 * (m % 2);
					apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], m);
					apdat_.DrawNumImage(g, 78, tenantData2.paramResearch_[m], num2 + 26, num3 + 3, 0, 2);
				}
				num4 = array4[7] + 5;
				if (En()) { num = 203; } else { num = 208; }
				g.SetColor(SC_WINDOW_BLACK);
				if (tenantData2 != null)
				{
					g.DrawString2(tenantData2.explain_, 120f, num, 2);
				}
				else
				{
					g.DrawString("空き地です", 120f, num, 2);
				}
			}
			else if (type_ == 66)
			{
				int[] array5 = DrawWindow2(g, 228, 176, "施設情報", 5, 20, 20, 0, 4, 12);
				TenantData tenantData3 = null;
				if (tenantDataId_ != -1)
				{
					tenantData3 = apdat_.tenantData_[tenantDataId_];
				}
				num2 = 17;
				num3 = 64;
				g.DrawImage(apdat_.resCom2_.img[126], num2, num3 + 2);
				apdat_.DrawNumImage(g, 79, tenantData3.lv_, num2 + 19, num3, 0, 1);
				num2 = 52;
				num3 = 65;
				g.SetColor(SC_WINDOW_BLACK);
				if (tenantData3 != null)
				{
					g.DrawString(tenantData3.name_, num2, num3);
				}
				else
				{
					g.DrawString("空き地", num2, num3);
				}
				num2 = 149;
				num3 = 65;
				g.SetColor(SC_WINDOW_BLUE);
				SetFontSize(g, 10);
				if (En())
				{
					g.DrawString("維持費", num2, num3 + 1);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.YEN("万", tenantData3.paramKeepCost_), num2 + 76, num3 + 1, 4);
				}
				else
				{
					g.DrawString("維持費", num2, num3);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.YEN("万", tenantData3.paramKeepCost_), num2 + 76, num3, 4);
				}
				ResetFontSize(g);
				num4 = 59;
				num = 83;
				g.SetColor(Graphics.GetColorOfRGB(181, 219, 166));
				g.DrawRect(num4, num, 121f, 75f);
				g.SetColor(Graphics.GetColorOfRGB(219, 249, 207));
				g.FillRect(num4 + 1, num + 1, 120f, 74f);
				g.ClipRect(num4, num, 122f, 76f);
				num4 = 90;
				num = 99;
				for (int n = 0; n < 4; n++)
				{
					num2 = num4 + n / 2 * 30 - n % 2 * 30;
					num3 = num + n / 2 * 15 + n % 2 * 15;
					MapchipMain mapchipMain3 = null;
					if (tenantData3 != null) { mapchipMain3 = apdat_.mapchipMain_[tenantData3.img_[n]]; } else { mapchipMain3 = apdat_.mapchipMain_[MapMain.mapchip_vacant.id_ + n]; }
					apdat_.DrawSeb(g, num2, num3, apdat_.resMapChipMain_.seb[mapchipMain3.seb_], apdat_.resMapChipMain_.img[mapchipMain3.img_], 0);
				}
				g.PopClip();
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(MyFormBase.LT("<0>を撤去しますか", tenantData3.name_), 120f, 172f, 2);
				num3 = 202;
				num = 200;
				int num16 = 40;
				if (select_ == 0)
				{
					Draw_btmMsg(g, "撤去する", 120 - num16, num, true, true);
					Draw_btmMsg(g, "やめておく", 120 + num16, num, false, false);
				}
				else if (select_ == 1)
				{
					Draw_btmMsg(g, "撤去する", 120 - num16, num, false, false);
					Draw_btmMsg(g, "やめておく", 120 + num16, num, true, true);
				}
				num16 = 58;
				GameView.AddTouch(g, 2, 75 - num16 / 2, num3 - 2, 68, 16, 131072);
				GameView.AddTouch(g, 2, 75 - num16 / 2 + 90, num3 - 2, 68, 16, 131073);
			}
			else if (type_ == 67)
			{
				TeamData teamData = apdat_.tData_[0];
				apdat_.DrawWindow(g, 231, 185, 0, teamData.name_);
				apdat_.DrawBox(g, 12, 64, 230, 192);
				int[] leagueData = udat_.GetLeagueData(0);
				num2 = 31;
				num3 = 60;
				g.DrawImage(apdat_.resCom2_.img[57], num2, num3);
				int rankIndex = udat_.GetRankIndex();
				if (MyFormBase.Jp())
				{
					apdat_.DrawNumImage(g, 35, rankIndex + 1, num2 + 16, num3, 0, 1);
				}
				else
				{
					apdat_.DrawNumImage(g, 35, rankIndex + 1, num2 + 16 + 3, num3, 0, 1);
				}
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString(UserData.RANK_NAME[rankIndex], 120f, num3, 2);
				g.DrawString(MyFormBase.LT("<0>年目", apdat_.save_.ints[1] + 1), 210f, num3, 4);
				SupervisorData supervisorData = apdat_.svisorData_[udat_.selectSupervisorId_];
				num2 = 12;
				num3 = 79;
				g.SetColor(Graphics.GetColorOfRGB(255, 251, 207));
				g.FillRect(16f, 79f, 99f, 42f);
				g.SetColor(Graphics.GetColorOfRGB(228, 255, 207));
				g.FillRect(16f, 151f, 99f, 47f);
				g.SetColor(SC_WINDOW_BLUE);
				SetFontSize(g, 10);
				g.DrawString("監督", num2 + 7, num3 + 5);
				g.DrawString("選手数", num2 + 7, num3 + 5 + 19);
				g.SetColor(SC_WINDOW_BLACK);
				if (MyFormBase.Jp())
				{
					g.DrawString(MyFormBase.LT("<0><1>", supervisorData.name_ + "\t" + supervisorData.name2_), num2 + 100 + 3, num3 + 5, 4);
				}
				else
				{
					string empty = string.Empty;
					empty += MyFormBase.LT(supervisorData.name_);
					empty += " ";
					empty += MyFormBase.LT(supervisorData.name2_);
					g.DrawString(empty, num2 + 100 + 3 - 7, num3 + 5, 4);
				}
				apdat_.DrawNumImage(g, 35, udat_.memberPDataist_.Size(), num2 + 100 - 12 - 2, num3 + 5 + 21, 0, 4);
				if (MyFormBase.Jp())
				{
					g.DrawString("人", num2 + 100, num3 + 5 + 21, 4);
				}
				ResetFontSize(g);
				SetFontSize(g, 10);
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("ｽﾎﾟﾝｻｰ", num2 + 8, num3 + 44);
				if (MyFormBase.Jp())
				{
					g.DrawString(MyFormBase.LT("<0>社", sponsor_num_), num2 + 53, num3 + 44);
				}
				else
				{
					g.DrawString(MyFormBase.LT("<0>社", sponsor_num_), num2 + 53 + 46, num3 + 44, 4);
				}
				if (MyFormBase.Jp())
				{
					apdat_.DrawNumImageComma(g, 35, sponsor_income_, num2 + 59, num3 + 59, 0, 0, 4);
					apdat_.DrawSeb(g, num2 + 60, num3 + 59, apdat_.resCom2_.seb[35], apdat_.resCom2_.img[103], 17);
				}
				else
				{
					apdat_.DrawMoney(g, 35, sponsor_income_, num2 + 59, num3 + 59, 0, 0, 4);
				}
				g.DrawString("/試合", num2 + 70, num3 + 58);
				ResetFontSize(g);
				num4 = 17;
				num = 153;
				int[] array6 = new int[3] { 2, 0, 1 };
				for (int num17 = 0; num17 < 3; num17++)
				{
					int num18 = array6[num17];
					num3 = num + 16 * num17;
					num2 = num4;
					g.SetColor(SC_WINDOW_BLUE);
					SetFontSize(g, 10);
					g.DrawString(FanActData.element_name[num18], num2 - 2 + 4, num3);
					ResetFontSize(g);
					int fanEva = FanActData.GetFanEva(num18);
					int fanDispParam = FanActData.GetFanDispParam(fanEva, num18);
					num2 = 112;
					if (num18 == 0) {
apdat_.DrawNumImageComma(g, 35, fanDispParam, num2 - 14, num3 + 2, 0, 0, 4);
g.SetColor(SC_WINDOW_BLACK);
if (MyFormBase.Jp())
						{
							g.DrawString("人", num2, num3 - 2 + 4, 4);
						}
} else if (num18 == 1) {
int num19 = 46;
int num20 = 0;
num20 = GameUtil.RateConvert(fanDispParam, 0, 100, 0, num19 - 2, true);
g.DrawImage(apdat_.resCom2_.img[129], num2 - 52, num3 + 1, 0, 0, 52, 10);
g.DrawImage(apdat_.resCom2_.img[129], num2 - 52 + 1, num3 + 2, 1, 10, num20, 8);
} else if (num18 == 2) {
apdat_.DrawNumImage(g, 35, fanDispParam, num2 - 14, num3 + 1, 0, 4);
}
coalescedSwitchExit221580: ;
				}
				int switchValue = udat_.GetMatchInfoJoin();
if (switchValue == 0) {
LeagueData leagueData2 = apdat_.leagueData_[udat_.selectLeagueId_];
num2 = 120;
num3 = 79;
g.SetColor(SC_WINDOW_BLUE);
if (MyFormBase.Jp())
					{
						g.DrawString("所属", num2 + 3, num3 + 5);
						g.DrawString("リーグ", num2 + 3, num3 + 5 + 19);
					}
					else
					{
						g.DrawString("所属", num2 + 3 - 3, num3 + 5);
						g.DrawString("リーグ", num2 + 3 - 3, num3 + 5 + 19);
					}
g.SetColor(SC_WINDOW_BLACK);
SetFontSize(g, 10);
if (MyFormBase.Jp())
					{
						g.DrawString(leagueData2.name_, num2 + 73, num3 + 5, 2);
					}
					else
					{
						g.DrawString(leagueData2.name_, num2 + 73 - 6, num3 + 5 + 1, 2);
					}
ResetFontSize(g);
if (MyFormBase.Jp())
					{
						int num21 = 14 + AppData.GetFig(leagueData[7]) * 8;
						apdat_.DrawNumImageComma(g, 35, leagueData[7], num2 + 73 + num21 / 2 - 14, num3 + 1 + 5 + 19, 0, 0, 4);
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString("位", num2 + 73 + num21 / 2 - 12, num3 + 5 + 19);
					}
					else
					{
						int num22 = 52;
						int num23 = AppData.GetFig(leagueData[7]) * 8 + 4 + font.StringWidth("位");
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString("位", num2 + num22, num3 + 5 + 19);
						apdat_.DrawNumImageComma(g, 35, leagueData[7], num2 + num22 + num23, num3 + 1 + 5 + 19, 0, 0, 4);
					}
} else if (switchValue == 1) {
QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
num2 = 171;
num3 = 79;
g.SetColor(SC_WINDOW_BLUE);
if (MyFormBase.Jp())
					{
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString("回戦", num2 - 7, num3 + 5 + 19);
						apdat_.DrawNumImageComma(g, 35, udat_.battleQmIndex_ + 1, num2 - 9, num3 + 1 + 5 + 19, 0, 0, 4);
					}
					else
					{
						g.SetColor(SC_WINDOW_BLACK);
						g.DrawString("回戦", num2 - 7 - 23, num3 + 5 + 19);
						apdat_.DrawNumImageComma(g, 35, udat_.battleQmIndex_ + 1, num2 - 9 + 41, num3 + 1 + 5 + 19, 0, 0, 4);
					}
SetFontSize(g, 10);
if (MyFormBase.Jp())
					{
						g.DrawString(questMatchData.name_, num2, num3 + 5, 2);
					}
					else
					{
						g.DrawString(questMatchData.name_, num2, num3 + 5 + 1, 2);
					}
ResetFontSize(g);
}
coalescedSwitchExit222308: ;
				num2 = 119;
				num3 = 125;
				g.SetColor(Graphics.GetColorOfRGB(255, 251, 207));
				g.FillRect(num2, num3, 107f, 54f);
				num2 = 174;
				num3 = 154;
				udat_.DrawGraph_menuElement(g, num2, num3, udat_.playerParam1, udat_.playerParam2, 100);
				for (int num24 = 0; num24 < UserData.GRAPH_POS.Length; num24++)
				{
					apdat_.DrawSeb(g, num2 + UserData.GRAPH_POS[num24][0], num3 + UserData.GRAPH_POS[num24][1], apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], num24);
				}
				num2 = 119;
				num3 = 181;
				g.SetColor(Graphics.GetColorOfRGB(255, 251, 207));
				g.FillRect(num2, num3, 107f, 20f);
				num2 = 133;
				num3 = 185;
				g.SetColor(SC_WINDOW_BLUE);
				SetFontSize(g, 10);
				if (MyFormBase.Jp())
				{
					g.DrawString("ﾁｰﾑ評価", num2, num3);
				}
				else
				{
					g.DrawString("ﾁｰﾑ評価", num2 - 4, num3 + 1);
				}
				ResetFontSize(g);
				if (MyFormBase.Jp())
				{
					apdat_.DrawNumImage(g, 30, teamData.warPotential_, num2 + 79, num3, 0, 4);
				}
				else
				{
					apdat_.DrawNumImage(g, 30, teamData.warPotential_, num2 + 79 + 2, num3, 0, 4);
				}
				num = 208;
				int num25 = udat_.matchResult_[0] + udat_.matchResult_[1] + udat_.matchResult_[2];
				g.SetColor(SC_WINDOW_BLACK);
				SetFontSize(g, 10);
				if (En())
				{
					g.DrawString(MyFormBase.LT("<0>戦", num25), 76f, num, 4);
					g.DrawString(MyFormBase.LT("<0>勝", udat_.matchResult_[0]), 123f, num, 4);
					g.DrawString(MyFormBase.LT("<0>敗", udat_.matchResult_[2]), 174f, num, 4);
					g.DrawString(MyFormBase.LT("<0>分", udat_.matchResult_[1]), 217f, num, 4);
				}
				else
				{
					g.DrawString(MyFormBase.LT("<0>戦", num25), 91f, num, 4);
					g.DrawString(MyFormBase.LT("<0>勝", udat_.matchResult_[0]), 123f, num, 4);
					g.DrawString(MyFormBase.LT("<0>敗", udat_.matchResult_[2]), 155f, num, 4);
					g.DrawString(MyFormBase.LT("<0>分", udat_.matchResult_[1]), 187f, num, 4);
				}
				ResetFontSize(g);
				GameView.AddTouch(2);
			}
			else if (type_ == 68)
			{
				int[] array7 = DrawWindow(g, 188, 110, "ﾁｹｯﾄ代の変更", 5, 5, 20, 0, 4);

				if (En())
				{
					num4 = 123;
					num = 103;
					apdat_.DrawMoney(g, 35, udat_.ticketPrice_ / 100, 120, num, 0, 0, 2, false);
				}
				else
				{
					num4 = 145;
					num = 101;
					apdat_.DrawNumImageComma(g, 31, udat_.ticketPrice_, num4, num, 4, 0, 4);
					apdat_.DrawSeb(g, num4 + 2, num + 2, apdat_.resCom2_.seb[35], apdat_.resCom2_.img[103], 12);
				}
				num4 = 120;
				num = 101;
				int num26 = 1000;
				int val = udat_.teamEvaRank_ * 2000 + 3000;
				val = AppData.ClampMax(val, 20000);
				val -= val % 1000;
				if (udat_.ticketPrice_ < val)
				{
					GameView.AddTouch(g, 18, num4 - 40 - 5, num + 6, apdat_.resCom2_.seb[24], 0, 16, TouchOption.Create().Margin(60, 5, 30, 30));
				}
				if (udat_.ticketPrice_ > num26)
				{
					GameView.AddTouch(g, 18, num4 + 40 + 7, num + 6, apdat_.resCom2_.seb[24], 1, 18, TouchOption.Create().Margin(5, 60, 30, 30));
				}
				num4 = 37;
				num = 128;
				apdat_.DrawBox(g, num4, num - 11, num4 + 167, num - 11 + 22);
				g.DrawImage(apdat_.resCom2_.img[172], num4, num, 0, 0, 16, 21);
				g.SetColor(SC_WINDOW_BLACK);
				SetFontSize(g, 10);
				if (En())
				{
					g.DrawString2(ticketPriceMessage, num4 + 23, num + 6 + 1);
				}
				else
				{
					g.DrawString2(ticketPriceMessage, num4 + 23, num + 6);
				}
				ResetFontSize(g);
				num = 157;
				Draw_btmMsg(g, "決定", num);
				int num27 = 50;
				GameView.AddTouch(g, 2, 120 - num27 / 2, num - 2, num27, 16, 131072);
			}
			else if (type_ == 69)
			{
				int coalescedWidth, num28;
coalescedWidth = num28 = 0;
				GameView instance = GameView.GetInstance();
				int gameWidth = instance.GetGameWidth();
				int gameHeight = instance.GetGameHeight();
				int originX = g.GetOriginX();
				int originY = g.GetOriginY();
				g.SetOrigin(0, 0);
				if (value1_ == 0)
				{
					coalescedWidth = gameWidth / SCENE_IMG_DIV_X[0];
					for (int num29 = 0; num29 < SCENE_IMG_DIV_Y[0]; num29++)
					{
						num = 0 + gameHeight * num29 / SCENE_IMG_DIV_Y[0];
						num28 = GameUtil.RateConvert(frame_ - num29 * SCENE_CHANGE_WAIT[0], 0, SCENE_CHANGE_ONE[0], 0, gameHeight / SCENE_IMG_DIV_Y[0], true);
						for (int num30 = 0; num30 < SCENE_IMG_DIV_X[0]; num30++)
						{
							num4 = 0 + gameWidth * num30 / SCENE_IMG_DIV_X[0];
							g.DrawImage(apdat_.resCom2_.img[89], num4, num, num4 % 120, 0, coalescedWidth, num28);
						}
					}
				}
				if (value1_ == 1)
				{
					num28 = 40;
					int num31 = gameHeight / num28 + 1;
					for (int num32 = 0; num32 < num31; num32++)
					{
						num = num28 * num32;
						int height = AppData.ClampMax(num28, gameHeight - num);
						for (int num33 = 0; num33 < SCENE_IMG_DIV_X[1]; num33++)
						{
							num4 = 0 + gameWidth * num33 / SCENE_IMG_DIV_X[1];
							coalescedWidth = GameUtil.RateConvert(frame_ - num33 * SCENE_CHANGE_WAIT[1], 0, SCENE_CHANGE_ONE[1], 0, gameWidth / SCENE_IMG_DIV_X[1], true);
							g.DrawImage(apdat_.resCom2_.img[89], num4, num, num4 % 120, 0, coalescedWidth, height);
						}
					}
				}
				if (value1_ == 2)
				{
					coalescedWidth = 20;
					num28 = 20;
					int num34 = instance.GetGameWidth() / coalescedWidth + 1;
					int num35 = instance.GetGameHeight() / num28 + 1;
					for (int num36 = 0; num36 < num35; num36++)
					{
						num = num28 * num36;
						for (int num37 = 0; num37 < num34; num37++)
						{
							num4 = coalescedWidth * num37;
							int width3 = GameUtil.RateConvert(frame_ - (num36 * SCENE_CHANGE_WAIT[2] + num37 * SCENE_CHANGE_WAIT[2]), 0, SCENE_CHANGE_ONE[2], 0, coalescedWidth, true);
							int height2 = GameUtil.RateConvert(frame_ - (num36 * SCENE_CHANGE_WAIT[2] + num37 * SCENE_CHANGE_WAIT[2]), 0, SCENE_CHANGE_ONE[2], 0, num28, true);
							g.DrawImage(apdat_.resCom2_.img[89], num4, num, num4 % 120, num % 40, width3, height2);
						}
					}
				}
				g.SetOrigin(originX, originY);
			}
			else if (type_ == 71)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "新チーム情報登録");
				apdat_.DrawBox(g, 12, 52, 227, 185);
				TeamData teamData2 = apdat_.tData_[0];
				int num38 = 76;
				int num39 = 16;
				num4 = 22;
				num = 73;
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("チームの名前", num4, num);
				g.DrawString("短縮名(4文字)", num4, num + 20);
				g.DrawString("ﾕﾆﾌｫｰﾑｶﾗｰ", num4, num + 40);
				g.SetColor(SC_WINDOW_SELECT);
				int num40 = g.GetFont(null).StringWidth(teamData2.name_);
				int num41 = g.GetFont(null).StringWidth(teamData2.name2_);
				if (select_ == 0)
				{
					g.FillRect(169 - num40 / 2 - 1, 72 + 20 * select_, num40 + 1, num39);
				}
				else if (select_ == 1)
				{
					g.FillRect(169 - num41 / 2 - 1, 72 + 20 * select_, num41 + 1, num39);
				}
				else if (select_ == 2)
				{
					g.FillRect(135f, 72 + 20 * select_, num38, num39);
				}
				GameView.AddTouch(g, 11, 125, 72, num38 + 20, num39, 131072);
				GameView.AddTouch(g, 11, 125, 92, num38 + 20, num39, 131073);
				GameView.AddTouch(g, 11, 125, 112, num38 + 20, num39, 131074);
				num4 = 169;
				GameView.AddTouch(g, 18, num4 - 40 - 5, num + 46, apdat_.resCom2_.seb[24], 0, 16, TouchOption.Create().Margin(30, 5, 20, 20));
				GameView.AddTouch(g, 18, num4 + 40 + 7, num + 46, apdat_.resCom2_.seb[24], 1, 18, TouchOption.Create().Margin(5, 20, 20, 20));
				if (select_ == 0)
				{
					apdat_.resCom2_.seb[60].Draw(g, 169 - num40 / 2 - 1 - 8, num + 9 + 20 * select_, apdat_.resCom2_.img);
				}
				else if (select_ == 1)
				{
					apdat_.resCom2_.seb[60].Draw(g, 169 - num41 / 2 - 1 - 8, num + 9 + 20 * select_, apdat_.resCom2_.img);
				}
				else if (select_ == 2)
				{
					apdat_.resCom2_.seb[60].Draw(g, 131f, num + 9 + 20 * select_, apdat_.resCom2_.img);
				}
				g.SetColor(SC_WINDOW_BLACK);
				num4 = 169;
				g.DrawString(teamData2.name_, num4, num, 2);
				g.DrawString(teamData2.name2_, num4, num + 20, 2);
				int num42 = 15 + g.GetFont(null).StringWidth(UNIFORM_NAME[udat_.selectUniform_]);
				apdat_.DrawSeb(g, num4 - num42 / 2, num + 40 + 1, apdat_.resCom2_.seb[28], apdat_.resCom2_.img[33], udat_.selectUniform_);
				g.DrawString(UNIFORM_NAME[udat_.selectUniform_], num4 - num42 / 2 + 15, num + 40);
				num = 134;
				g.ClipRect(14f, num, 211f, 60f);
				apdat_.DrawSeb(g, 0, num, apdat_.resCom2_.seb[32], apdat_.resCom2_.img[34], 0);
				apdat_.DrawSeb(g, 120, num, apdat_.resCom2_.seb[32], apdat_.resCom2_.img[34], 0);
				apdat_.DrawSeb(g, 0, num + 27, apdat_.resCom2_.seb[33], apdat_.resCom2_.img[35], 0);
				apdat_.DrawSeb(g, 120, num + 27, apdat_.resCom2_.seb[33], apdat_.resCom2_.img[35], 0);
				g.PopClip();
				num2 = 14;
				num3 = 134;
				g.SetColor(Graphics.GetColorOfRGB(213, 232, 220));
				g.DrawRect(num2, num3, 210f, 59f);
				g.DrawRect(num2 + 1, num3 + 1, 208f, 57f);
				num3 = 184;
				for (int num43 = 0; num43 < 5; num43++)
				{
					num2 = 82 + 19 * num43;
					PlayerData playerData = apdat_.pData_[num43 + 22];
					apdat_.DrawSeb(g, num2 - 1, num3 + 1, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 4);
					if (num43 == 0)
					{
						udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, frame_ % 20 / 5, 1, 0, -1);
					}
					else
					{
						udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, frame_ % 20 / 5, 1);
					}
					udat_.DrawDispPlayer(g, num2, num3);
				}
				num = 200;
				if (select_ == 3)
				{
					g.SetColor(SC_WINDOW_SELECT);
					g.FillRect(78f, num, 84f, 16f);
				}
				GameView.AddTouch(g, 11, 68, num, 104, 16, 131075);
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("加盟申請する", 120f, num + 2, 2);
				if (select_ == 3)
				{
					apdat_.resCom2_.seb[60].Draw(g, 74f, num + 9, apdat_.resCom2_.img);
				}
				g.ClipRect(0f, 198f, 240f, 20f);
				Draw_chara(g, 172, 0, 0, frame_, 26, 226);
				Draw_chara(g, 171, 0, 1, frame_, 212, 226);
				g.PopClip();
			}
			else if (type_ == 72)
			{
				apdat_.DrawWindow(g, 210, 110, 0, "選手の解雇");
				apdat_.DrawBox(g, 26, 90, 213, 148);
				g.SetColor(SC_WINDOW_BLACK);
				num = 116;
				g.DrawString("選手を解雇しますか", 120f, num, 2);
				num = 132;
				g.DrawString(MyFormBase.LT("現在  <0> /30名", udat_.memberPDataist_.Size()), 120f, num, 2);
				num3 = 164;
				if (select_ == 0)
				{
					g.SetColor(SC_WINDOW_SELECT);
					g.FillRect(54f, num3 - 2, 58f, 16f);
					apdat_.resCom2_.seb[60].Draw(g, 50f, num3 + 7, apdat_.resCom2_.img);
				}
				else
				{
					g.SetColor(SC_WINDOW_SELECT);
					g.FillRect(127f, num3 - 2, 58f, 16f);
					apdat_.resCom2_.seb[60].Draw(g, 123f, num3 + 7, apdat_.resCom2_.img);
				}
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString("はい", 71f, num3);
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString("いいえ", 138f, num3);
				int num44 = 58;
				int num45 = 100;
				int num46 = 160;
				GameView.AddTouch(g, 3, 75 - num44 / 2 - num45 / 2, num3 - 2 - num46 / 2, 68 + num45, 16 + num46, 131072);
				GameView.AddTouch(g, 3, 75 - num44 / 2 + 90 - num45 / 2, num3 - 2 - num46 / 2, 68 + num45, 16 + num46, 131073);
			}
			else if (type_ == 74)
			{
				apdat_.DrawWindow(g, 228, 176, 0, MyFormBase.LT("収支情報 <0>/2", page_ + 1));
				apdat_.DrawBox(g, 12, 69, 227, 190);
				GameView.AddTouch(g, 1, 44 - frame_ % 20 / 5, 51, apdat_.resCom2_.seb[24], 0, 16);
				GameView.AddTouch(g, 1, 193 + frame_ % 20 / 5, 51, apdat_.resCom2_.seb[24], 1, 18);
				num = 65;
				g.SetColor(SC_WINDOW_BLACK);
				if (page_ == 0)
				{
					g.DrawString("月間", 28f, num);
				}
				else
				{
					g.DrawString("年間", 28f, num);
				}
				g.SetColor(SC_WINDOW_GRAY);
				if (En())
				{
					g.DrawString("収入", 102f, num);
					g.DrawString("支出", 168f, num);
				}
				else
				{
					g.DrawString("収入", 121f, num);
					g.DrawString("支出", 194f, num);
				}
				num4 = 22;
				num = 90;
				int num47 = 0;
				int num48 = 0;
				int num49 = 0;
				for (int num50 = 0; num50 < UserData.RECORD_NAME.Length; num50++)
				{
					num3 = num + 18 * num50;
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(UserData.RECORD_NAME[num50], num4, num3);
					if (num50 != 4)
					{
						g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
						g.DrawLine(num4, num3 + 12 + 2, num4 + 210 - 12, num3 + 12 + 2);
					}
					g.SetColor(SC_WINDOW_BLUE);
					if (page_ == 0)
					{
						num48 = udat_.records_[apdat_.save_.ints[2]][num50][0];
					}
					else
					{
						num48 = 0;
						for (int num51 = 0; num51 < 12; num51++)
						{
							num48 += udat_.records_[num51][num50][0];
						}
					}
					g.DrawString(MyFormBase.YEN("万", num48), num4 + 121, num3, 4);
					g.SetColor(SC_WINDOW_RED);
					if (page_ == 0)
					{
						num49 = udat_.records_[apdat_.save_.ints[2]][num50][1];
					}
					else
					{
						num49 = 0;
						for (int num52 = 0; num52 < 12; num52++)
						{
							num49 += udat_.records_[num52][num50][1];
						}
					}
					g.DrawString(MyFormBase.YEN("万", num49), num4 + 194, num3, 4);
					num47 += num48 - num49;
				}
				g.SetColor(Graphics.GetColorOfRGB(146, 184, 247));
				g.DrawLine(22f, 180f, 220f, 180f);
				g.SetColor(SC_WINDOW_BLACK);
				g.DrawString("利益", num4, 184f);
				if (num47 >= 0)
				{
					g.SetColor(SC_WINDOW_BLUE);
				}
				else
				{
					g.SetColor(SC_WINDOW_RED);
				}
				g.DrawString(MyFormBase.YEN("万", num47), num4 + 194, 184f, 4);
				num = 201;
				g.ClipRect(0f, num, 240f, 17f);
				Draw_chara(g, 172, 0, 0, frame_, 30, num + 28);
				Draw_chara(g, 171, 0, 1, frame_, 209, num + 28);
				g.PopClip();
				num = 204;
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("観客数", 55f, 204f);
				int num53 = 0;
				if (page_ == 0)
				{
					num53 = udat_.customer_month_[apdat_.save_.ints[2]];
				}
				else
				{
					num53 = 0;
					for (int num54 = 0; num54 < udat_.customer_month_.Length; num54++)
					{
						num53 += udat_.customer_month_[num54];
					}
				}
				apdat_.DrawNumImageComma(g, 78, num53, 179, num + 1, 0, 0, 4);
				apdat_.DrawSeb(g, 181, num + 2, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 18);
				GameView.AddTouch(2, TouchOption.Create(2));
			}
			else if (type_ == 75 || type_ == 76)
			{
				int num55 = 0;
				if (type_ == 75)
				{
					num55 = GameUtil.RateConvert(frame_, 0, FADE_TIME_V, 0, 255, true);
				}
				else if (type_ == 76)
				{
					num55 = GameUtil.RateConvert(frame_, 0, FADE_TIME_V, 255, 0, true);
				}
				if (num55 > 0)
				{
					GameView instance2 = GameView.GetInstance();
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, num55));
					g.FillRect(0f, 0f, instance2.GetGameWidth(), instance2.GetGameHeight());
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
				}
			}
			else if (type_ == 77)
			{
				apdat_.DrawWindow(g, 228, 176, 0, "クラブの歴史");
				apdat_.DrawBox(g, 12, 51, 227, 205);
				num4 = 14;
				num = 67;
				for (int num56 = scrollValue_; num56 < ary1.Size(); num56++)
				{
					int num57 = num56 - scrollValue_;
					if (num57 >= 9)
					{
						break;
					}
					num2 = num4;
					num3 = num + 16 * num57;
					FanBonusData fanBonusData = (FanBonusData)ary1.ElementAt(num56);
					g.SetColor(Graphics.GetColorOfRGB(222, 234, 225));
					g.DrawLine(num2, num3, num2 + 211 - 1, num3);
					g.DrawLine(num2, num3 + 16, num2 + 211 - 1, num3 + 16);
					if (num56 == select_)
					{
						g.SetColor(SC_WINDOW_SELECT);
						g.FillRect(num2 + 1, num3 + 1, 209f, 15f);
					}
					GameView.AddTouch(g, 11, num2, num3, 209, 15, 0x20000 | num56, TouchOption.Create(16).Margin(30, 0, 0, 0));
					if (fanBonusData.state_ == 1)
					{
						g.DrawImage(apdat_.resCom2_.img[178], num2 + 2, num3 + 1);
						string empty2 = string.Empty;
						if (MyFormBase.Jp()) { empty2 = (fanBonusData.achiveDay_[0] + 1 + MyFormBase.LT("年") + (fanBonusData.achiveDay_[1] + 1) + "/" + (fanBonusData.achiveDay_[2] + 1)); } else { empty2 = (MyFormBase.LT("年") + (fanBonusData.achiveDay_[0] + 1) + "/" + (fanBonusData.achiveDay_[1] + 1) + "/" + (fanBonusData.achiveDay_[2] + 1)); }
						g.SetColor(SC_WINDOW_BLUE);
						g.DrawString(empty2, num2 + 71, num3 + 3, 4);
						g.SetColor(SC_WINDOW_BLACK);
						SetFontSize(g, 10);
						if (En())
						{
							g.DrawString(fanBonusData.info_name_, num2 + 84 - 4, num3 + 3 + 1);
						}
						else
						{
							g.DrawString(fanBonusData.info_name_, num2 + 84 - 4, num3 + 3);
						}
						ResetFontSize(g);
						if (MyFormBase.Jp())
						{
							g.SetColor(SC_WINDOW_BLUE);
							g.DrawString("達成", num2 + 190, num3 + 3, 2);
						}
					}
					else
					{
						g.SetColor(SC_WINDOW_GRAY);
						g.DrawString("--年--/-", num2 + 71, num3 + 3, 4);
						g.DrawString("？？？？", num2 + 84, num3 + 3);
						SetFontSize(g, 10);
						if (En()) { } else {
							g.DrawString("未達成", num2 + 190 - 2, num3 + 3, 2);
						}
						ResetFontSize(g);
					}
					if (num56 == select_)
					{
						apdat_.resCom2_.seb[60].Draw(g, num2 - 4 + 5, num3 + 9, apdat_.resCom2_.img);
					}
				}
				g.SetColor(Graphics.GetColorOfRGB(222, 234, 225));
				g.DrawLine(90f, 68f, 90f, 210f);
				apdat_.DrawVerticalScroll2(g, num4 + 214, num - 5, 3, 154, scrollValue_, ary1.Size() - 1, 9);
			}
			else if (type_ == 78)
			{
				Draw_moveToBus(g, frame_);
				GameView.AddTouch(2);
			}
			else if (type_ == 79)
			{
				Draw_battlePic(g, frame_);
				GameView.AddTouch(2);
			}
		}

		public override bool Finish()
		{
			JSystem.Gc();
			return true;
		}

		public override bool Resume()
		{
			return true;
		}

		public override bool Update()
		{
			err_method = "update()";
			frame_ = (frame_ + 1) % int.MaxValue;
			frame2_ = (frame2_ + 1) % int.MaxValue;
			FormManager instance = FormManager.GetInstance();
			untilActiveHide_ = false;
			if (wait_ > 0)
			{
				formMan_.DrawAllForms();
				wait_--;
				if (wait_ > 0)
				{
					return true;
				}
			}
			int num = 0;
			if (canvas_.CheckKeyState(131072))
			{
				num++;
			}
			if (canvas_.CheckKeyState(524288))
			{
				num++;
			}
			if (canvas_.CheckKeyState(65536))
			{
				num++;
			}
			if (canvas_.CheckKeyState(262144))
			{
				num++;
			}
			if (num > 1)
			{
				canvas_.ClearKeyBuffer();
			}
			loopCounter_ = (loopCounter_ + 1) % int.MaxValue;
			if (jingle_ != -1)
			{
				apdat_.PlayJingle(jingle_);
			}
			jingle_ = -1;
			apdat_.resCom2_.seb[61].Frame();
			apdat_.resCom2_.seb[60].Frame();
			apdat_.resCom2_.seb[62].Frame();
			if (type_ == 1 || type_ == 21 || type_ == 2)
			{
				if (canvas_.CheckKeyRepeat(262144))
				{
					select_ = (select_ + 1) % selectButtons_.Length;
				}
				else if (canvas_.CheckKeyRepeat(65536))
				{
					select_ = (select_ - 1 + selectButtons_.Length) % selectButtons_.Length;
				}
				else if (canvas_.CheckKeyPulse(1048576) || (Config.DEBUG_CMD && canvas_.CheckKeyState(64)))
				{
					result_ = select_;
					instance.Pop();
					if (type_ == 21 && result_ == 0)
					{
						Canvas.OpenBrowser(AppData.SITE_URL);
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					instance.Pop();
				}
			}
			else if (type_ == 3)
			{
				if (Config.TRIAL_VERSION && GetLeftSoftLabel() == AppData.softLabels_[3])
				{
					SetSoftLabel(AppData.softLabels_[5], AppData.softLabels_[2]);
				}
				if (dialog_ != null)
				{
					int num2 = dialog_.result_;
					dialog_ = null;
					if (num2 == -1)
					{
						return apdat_.RemoveAllMenuForms();
					}
				}
				int num3 = 0;
				num3 = ((!MyFormBase.Jp()) ? 124 : 104);
				int[] array = FrameMenu();
				if (array != null)
				{
					int num4 = array[0];
					int num5 = array[1];
					int num6 = 29;
					switch (num4)
					{
					case 0:
						if (!udat_.ChEventFlg(8) && !udat_.ChEventFlg(8))
						{
							apdat_.ExecEvent(152, null, null);
							break;
						}
						return Push(new SubForm(5, x_ + num3, y_ + num6 * select_));
					case 1:
						if (!apdat_.CheckAlreadyEvent(158))
						{
							apdat_.ExecEvent(158, null, null);
						}
						Push(new SubForm(45));
						break;
					case 2:
						if (!udat_.ChEventFlg(2) && !udat_.ChEventFlg(16) && !udat_.ChEventFlg(4) && !udat_.ChEventFlg(32))
						{
							apdat_.ExecEvent(175, null, null);
							break;
						}
						return Push(new SubForm(7, x_ + num3, y_ + num6 * select_));
					case 3:
						Push(new SubForm(26));
						break;
					case 4:
						return Push(new SubForm(11, x_ + num3, y_ + num6 * select_));
					case 5:
						return Push(new SubForm(12, x_ + num3, y_ + num6 * select_));
					}
					return apdat_.RemoveAllMenuForms();
				}
				if (apdat_.IsPushSoftLabel(3))
				{
					instance.Pop();
					return Push(new SubForm(16));
				}
				if (apdat_.IsPushSoftLabel(5))
				{
					Canvas.OpenBrowser(Config.URL_SITE);
				}
			}
			else if (type_ == 4)
			{
				frame2_ = (frame2_ + 1) % int.MaxValue;
				if (dialog_ != null)
				{
					int num7 = dialog_.result_;
					dialog_ = null;
					if (num7 == -1)
					{
						return apdat_.RemoveAllMenuForms();
					}
				}
				int num8 = 104;
				int[] array2 = FrameMenu();
				if (array2 != null)
				{
					int num9 = array2[0];
					int num10 = array2[1];
					int num11 = 24;
					switch (num9)
					{
					case 21:
						Push(new SubForm(14));
						break;
					case 22:
						Push(new SubForm(15));
						break;
					case 25:
						udat_.gm_.gameTime_ = GameManager.HALFTIME;
						udat_.gm_.halfTime = true;
						break;
					}
					return apdat_.RemoveAllMenuForms();
				}
			}
			else if (type_ == 8)
			{
				if (dialog_ != null)
				{
					if (dialog_.result_ == 0)
					{
						instance.Pop();
					}
					dialog_ = null;
					return false;
				}
				int[] array3 = FrameMenu();
				if (array3 != null)
				{
					int num12 = array3[0];
					int num13 = array3[1];
					return apdat_.RemoveAllMenuForms();
				}
			}
			else if (type_ == 5)
			{
				if (dialog_ != null)
				{
					if (dialog_.result_ == 0)
					{
						dialog_ = null;
						if (udat_.GetMatchInfoJoin() == 0)
						{
							udat_.League_resetScore();
							LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
							leagueData.SetGetTrophy(1);
						}
						udat_.SetMatchInfoJoin(2);
						return Pop();
					}
					dialog_ = null;
				}
				int[] array4 = FrameMenu();
				if (array4 != null)
				{
					int num14 = array4[0];
					int num15 = array4[1];
					switch (num14)
					{
					case 6:
						Push(new SubForm(33, 0));
						break;
					case 8:
						Push(new SubForm(58));
						break;
					case 9:
						dialog_ = MakeDialog("参加中の大会を棄権します", "はい\tいいえ", 1);
						return Push(dialog_);
					}
					return apdat_.RemoveAllMenuForms();
				}
			}
			else if (type_ == 6)
			{
				int[] array5 = FrameMenu();
				if (array5 != null)
				{
					int num16 = array5[0];
					int num17 = array5[1];
					if (num16 == 11)
					{
						Push(new SubForm(26));
					}
					return apdat_.RemoveAllMenuForms();
				}
			}
			else if (type_ == 7)
			{
				int[] array6 = FrameMenu();
				if (array6 != null)
				{
					int num18 = array6[0];
					int num19 = array6[1];
					switch (num18)
					{
					case 14:
						if (!apdat_.CheckAlreadyEvent(165))
						{
							apdat_.ExecEvent(165, null, null);
						}
						Push(new SubForm(39));
						break;
					case 12:
					{
						if (udat_.memberPDataist_.Size() >= 30)
						{
							apdat_.ExecEvent(143, null, null);
							return true;
						}
						if (!apdat_.CheckAlreadyEvent(154))
						{
							apdat_.ExecEvent(154, null, null);
						}
						int num20 = 0;
						for (int i = 0; i < apdat_.tradeData_.Length; i++)
						{
							TradeData tradeData = apdat_.tradeData_[i];
							if (tradeData.state_ == 1)
							{
								num20++;
							}
						}
						if (num20 == 0)
						{
							apdat_.ExecEvent(124, null, null);
							return Pop();
						}
						apdat_.ExecEvent(123, num20 + string.Empty, null);
						Push(new SubForm(53));
						break;
					}
					case 15:
						if (!apdat_.CheckAlreadyEvent(164))
						{
							apdat_.ExecEvent(164, null, null);
						}
						Push(new SubForm(60));
						break;
					case 16:
						if (!apdat_.CheckAlreadyEvent(155))
						{
							apdat_.ExecEvent(155, null, null);
						}
						Push(new SubForm(42));
						break;
					}
					return apdat_.RemoveAllMenuForms();
				}
			}
			else if (type_ == 11)
			{
				int[] array7 = FrameMenu();
				if (array7 != null)
				{
					int num21 = array7[0];
					int num22 = array7[1];
					switch (num21)
					{
					case 17:
						Push(new SubForm(67));
						break;
					case 13:
						Push(new SubForm(50));
						break;
					case 18:
						Push(new SubForm(77));
						break;
					case 19:
						Push(new SubForm(74));
						break;
					}
					return apdat_.RemoveAllMenuForms();
				}
			}
			else if (type_ == 12)
			{
				if (dialog_ != null)
				{
					if (dialog_.result_ == 0)
					{
						if (dialog_.dialogId_ != 99)
						{
							return formMan_.RemoveAllForms();
						}
						dialog_ = null;
						Canvas.OpenBrowser(AppData.SITE_URL);
					}
					dialog_ = null;
				}
				int[] array8 = FrameMenu();
				if (array8 != null)
				{
					int num23 = array8[0];
					int num24 = array8[1];
					switch (num23)
					{
					case 20:
						Push(new SubForm(16));
						break;
					case 21:
						Push(new SubForm(14));
						break;
					case 22:
						Push(new SubForm(15));
						break;
					case 23:
						if (!Config.TRIAL_VERSION)
						{
							Push(new RankForm());
							break;
						}
						return Push(MakeTalkForm(0, MyFormBase.LT("ハイスコアは<br>正式版でご利用頂けます!!"), null));
					case 24:
						dialog_ = MakeDialog("ゲームを終了します", "はい\tいいえ", 1);
						return Push(dialog_);
					case 29:
						return formMan_.ChangeCurrentForm(apdat_.frmTitle_);
					}
					return apdat_.RemoveAllMenuForms();
				}
				if (apdat_.IsPushSoftLabel(5))
				{
					Canvas.OpenBrowser(Config.URL_SITE);
				}
			}
			else if (type_ == 22)
			{
				if (dialog_ != null)
				{
					if (dialog_.result_ == 0)
					{
						return Pop();
					}
					dialog_ = null;
				}
				int[] array9 = FrameMenu();
				if (array9 != null)
				{
					int num25 = array9[0];
					int num26 = array9[1];
					switch (num25)
					{
					case 26:
						result_ = 0;
						return Pop();
					case 27:
						result_ = 1;
						dialog_ = MakeDialog("今のセーブデータに\n上書きされます", "はい\tいいえ", 1);
						return Push(dialog_);
					case 28:
						result_ = 2;
						dialog_ = MakeDialog("今のセーブデータは\n消えてしまいます", "はい\tいいえ", 1);
						return Push(dialog_);
					}
				}
			}
			else if (type_ == 0)
			{
				formMan_.DrawAllForms();
				textLay_.Frame();
				if (canvas_.CheckKeyPulse(1048576) || (Config.DEBUG_CMD && canvas_.CheckKeyState(64)))
				{
					if (textLay_.IsEnd())
					{
						textIdx_++;
						if (textIdx_ < textAry_.Length)
						{
							textLay_.SetText(textAry_[textIdx_]);
						}
						else
						{
							if (nextIndex_ == -1)
							{
								return instance.Pop();
							}
							string value = AppData.talks_[nextIndex_];
							if (talkArg_ != null)
							{
								value = StringUtil.Replace(value, talkArg_);
							}
							string[] array10 = StringUtil.Split(value, "\t");
							nextIndex_ = JInteger.ParseInt(array10[1]);
							textAry_ = new string[array10.Length - 2];
							JSystem.Arraycopy(array10, 2, textAry_, 0, array10.Length - 2);
							textIdx_ = 0;
							textLay_.SetText(textAry_[textIdx_]);
						}
					}
					else
					{
						textLay_.Flush();
					}
				}
			}
			else if (type_ == 13)
			{
				int num27 = eventIds_[page_];
				if (frame_ == 1)
				{
					switch (num27)
					{
					case 0:
						apdat_.PlayJingle(17);
						break;
					case 1:
						apdat_.PlayJingle(21);
						break;
					}
				}
				if (canvas_.CheckKeyPulse(1048576) || (Config.DEBUG_CMD && canvas_.CheckKeyState(64)))
				{
					if (frame_ < 60)
					{
						frame_ = 60;
					}
					else if (frame_ >= 60)
					{
						instance.Pop();
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					instance.Pop();
				}
			}
			else if (type_ == 14)
			{
				frame_ = (frame_ + 1) % 20;
				bool flag = MyFormBase.Jp();
				if (Language.GetLanguageCode() != "ja-JP" && Language.GetLanguageCode() != "en-US" && !Language.GetLanguageCode().StartsWith("*"))
				{
					flag = true;
				}
				if (flag)
				{
					if (canvas_.CheckKeyRepeat(131072))
					{
						select_ = (select_ - 1 + CONFIG_NAMES.Length) % CONFIG_NAMES.Length;
					}
					if (canvas_.CheckKeyRepeat(524288))
					{
						select_ = (select_ + 1) % CONFIG_NAMES.Length;
					}
					int num28 = GetTouchValue(CONFIG_VALUES[select_], 0);
					if (CONFIG_VALUE_REVERSE[select_])
					{
						num28 = (num28 + 1) % 2;
					}
					if (select_ == CONFIG_ORIENTATION)
					{
						num28 = ((num28 != -1) ? 1 : 0);
					}
					int num29 = num28;
					if (select_ != CONFIG_SPEED || apdat_.sysSave_.ints[9] != 0)
					{
						if (canvas_.CheckKeyRepeat(65536))
						{
							num28 = (num28 - 1 + CONFIG_VALUE_MAX[select_]) % CONFIG_VALUE_MAX[select_];
						}
						if (canvas_.CheckKeyRepeat(262144))
						{
							num28 = (num28 + 1) % CONFIG_VALUE_MAX[select_];
						}
					}
					if (num29 != num28)
					{
						if (CONFIG_VALUE_REVERSE[select_])
						{
							num28 = (num28 + 1) % 2;
						}
						if (select_ == CONFIG_ORIENTATION)
						{
							num28 = ((num28 != 0) ? app_.GetOrientation() : (-1));
						}
						SetTouchValue(CONFIG_VALUES[select_], num28);
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						try
						{
							apdat_.SaveSystem();
						}
						catch (Exception t)
						{
							if (Config.PRINT)
							{
								Log.Info(t);
							}
						}
						return Pop();
					}
				}
				else
				{
					if (canvas_.CheckKeyRepeat(131072))
					{
						select_ = (select_ - 1 + CONFIG_EN_NAMES.Length) % CONFIG_EN_NAMES.Length;
					}
					if (canvas_.CheckKeyRepeat(524288))
					{
						select_ = (select_ + 1) % CONFIG_EN_NAMES.Length;
					}
					int num30 = GetTouchValue(CONFIG_EN_VALUES[select_], 0);
					if (CONFIG_EN_VALUE_REVERSE[select_])
					{
						num30 = (num30 + 1) % 2;
					}
					if (select_ == CONFIG_EN_ORIENTATION)
					{
						num30 = ((num30 != -1) ? 1 : 0);
					}
					int num31 = num30;
					if (select_ != CONFIG_EN_SPEED || apdat_.sysSave_.ints[9] != 0)
					{
						if (canvas_.CheckKeyRepeat(65536))
						{
							num30 = (num30 - 1 + CONFIG_EN_VALUE_MAX[select_]) % CONFIG_EN_VALUE_MAX[select_];
						}
						if (canvas_.CheckKeyRepeat(262144))
						{
							num30 = (num30 + 1) % CONFIG_EN_VALUE_MAX[select_];
						}
					}
					if (num31 != num30)
					{
						if (CONFIG_EN_VALUE_REVERSE[select_])
						{
							num30 = (num30 + 1) % 2;
						}
						if (select_ == CONFIG_EN_ORIENTATION)
						{
							num30 = ((num30 != 0) ? app_.GetOrientation() : (-1));
						}
						SetTouchValue(CONFIG_EN_VALUES[select_], num30);
						if (select_ == CONFIG_EN_CURRENCY)
						{
							apdat_.frmGame_.SetAllDraw();
						}
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						try
						{
							apdat_.SaveSystem();
							apdat_.frmGame_.SetAllDraw();
						}
						catch (Exception t2)
						{
							if (Config.PRINT)
							{
								Log.Info(t2);
							}
						}
						return Pop();
					}
				}
			}
			else if (type_ == 15)
			{
				frame_ = (frame_ + 1) % int.MaxValue;
				int num32 = page_;
				if (canvas_.CheckKeyPulse(1048576) || canvas_.CheckKeyRepeat(262144))
				{
					page_ = (page_ + 1) % apdat_.manualTexts_.Length;
				}
				if (canvas_.CheckKeyRepeat(65536))
				{
					page_ = (page_ - 1 + apdat_.manualTexts_.Length) % apdat_.manualTexts_.Length;
				}
				if (num32 != page_)
				{
					textLay_.SetText(apdat_.manualTexts_[page_]);
					textLay_.Flush();
				}
				if (page_ == apdat_.manualTexts_.Length - 1)
				{
					anime_cnt = (anime_cnt + 1) % int.MaxValue;
					int num33 = anime_cnt % 56;
					if (num33 % 28 == 0 && anime_cnt != 0)
					{
						anime_mode = GameUtil.Random(2);
						anime_ballY = -(GameUtil.Random(25) + 5);
					}
				}
				if (apdat_.IsPushSoftLabel(2, 1048576))
				{
					return instance.Pop();
				}
			}
			else if (type_ == 16)
			{
				if (saveState_ == 0)
				{
					saveState_ = 1;
				}
				else if (saveState_ == 1)
				{
					try
					{
						apdat_.SaveGame();
						text_ = "ゲームセーブ完了";
					}
					catch (Exception ex)
					{
						if (Config.PRINT)
						{
							Log.Info(ex);
						}
						text_ = MyFormBase.LT("セーブに失敗しました【<0>】", apdat_.saveLine_) + ex;
						apdat_.sysSave_.strs[10] += ex.ToString() + ":" + text_;
						if (apdat_.sysSave_.strs[10].Length() > 1024)
						{
							apdat_.sysSave_.strs[10] = apdat_.sysSave_.strs[10].SubstringJ(apdat_.sysSave_.strs[10].Length() - 1024);
						}
						try
						{
							apdat_.SaveSystem();
						}
						catch (Exception)
						{
						}
					}
					saveState_ = 2;
				}
				else if (saveState_ == 2 && apdat_.IsPushSoftLabel(2, 1048576))
				{
					return instance.Pop();
				}
			}
			else if (type_ == 17)
			{
				frame_ = (frame_ + 1) % 20;
				if (canvas_.CheckKeyRepeat(65536))
				{
					page_ = (page_ - 1 + pagesNum_) % pagesNum_;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					page_ = (page_ + 1 + pagesNum_) % pagesNum_;
				}
				if (canvas_.CheckKeyPulse(1048576) || (Config.DEBUG_CMD && canvas_.CheckKeyState(64)))
				{
					page_++;
					if (page_ >= pagesNum_)
					{
						return instance.Pop();
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return instance.Pop();
				}
			}
			else if (type_ == 18)
			{
				value1_--;
				if (value1_ <= 0)
				{
					return instance.Pop();
				}
			}
			else if (type_ == 19)
			{
				Update_clearPoint();
			}
			else if (type_ == 23)
			{
				frame_++;
				formMan_.DrawAllForms();
				if (frame_ > 3)
				{
					if (canvas_.CheckKeyPulse(1048576) || (Config.DEBUG_CMD && canvas_.CheckKeyState(64)))
					{
						instance.Pop();
					}
					else if (apdat_.IsPushSoftLabel(2))
					{
						instance.Pop();
					}
				}
			}
			else if (type_ == 24 || type_ == 25)
			{
				frame_++;
				formMan_.DrawAllForms();
				if (frame_ >= FADE_TIME_V)
				{
					Pop();
				}
			}
			else if (type_ == 26)
			{
				if (frame_ == 1 && !apdat_.CheckAlreadyEvent(160))
				{
					apdat_.ExecEvent(160, null, null);
				}
				if (udat_.Update_edit(false))
				{
					return Pop();
				}
			}
			else if (type_ == 27 || type_ == 28 || type_ == 31)
			{
				if (dialog_ != null)
				{
					if (dialog_.result_ == 0)
					{
						PlayerData playerData = (PlayerData)ary1.ElementAt(select_);
						PlayerData.Dismissal(playerData.id_);
						dialog_ = null;
						SubForm subForm = apdat_.ExecEvent(118 + GameUtil.Random(3), null, null);
						subForm.humanCharaType_ = 1;
						subForm.humanCharaIndex_ = playerData.id_;
						return Pop();
					}
					dialog_ = null;
					return true;
				}
				int num34 = ary1.Size();
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num34) % num34;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num34;
				}
				int num35 = 8;
				if (scrollValue_ > select_)
				{
					scrollValue_ = select_;
				}
				if (scrollValue_ + num35 <= select_)
				{
					scrollValue_ = select_ - num35 + 1;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					PlayerData playerData2 = (PlayerData)ary1.ElementAt(select_);
					PlayerData.RemoveNew();
					if (type_ == 27)
					{
						SubForm subForm2 = new SubForm(30);
						subForm2.playerData_ = playerData2;
						return Push(subForm2);
					}
					if (type_ == 31)
					{
						if (MyFormBase.Jp())
						{
							dialog_ = MakeDialog(MyFormBase.LT(playerData2.name_) + MyFormBase.LT("選手を解雇しますか"), "はい\tいいえ", 1);
						}
						else
						{
							dialog_ = MakeDialog(MyFormBase.LT("選手を解雇しますか"), "はい\tいいえ", 1);
						}
						return Push(dialog_);
					}
					if (type_ == 28)
					{
						int num36 = playerData2.id_;
						int[] array11 = (int[])udat_.editTest_team[udat_.et_cur[5] / 10].ElementAt(udat_.et_cur[5] % 10);
						int index = array11[2];
						PlayerData obj = (PlayerData)udat_.memberPDataist_.ElementAt(index);
						int index2 = udat_.memberPDataist_.IndexOf(playerData2);
						udat_.memberPDataist_.SetElementAt(playerData2, index);
						udat_.memberPDataist_.SetElementAt(obj, index2);
						Pop();
						udat_.ChangeEditPhase(6, false);
						return true;
					}
				}
				else
				{
					if (apdat_.IsPushSoftLabel(2))
					{
						PlayerData.RemoveNew();
						return Pop();
					}
					if (apdat_.IsPushSoftLabel(8))
					{
						return Push(new SubForm(32));
					}
				}
			}
			else if (type_ == 29)
			{
				int num37 = ary1.Size();
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num37) % num37;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num37;
				}
				if (canvas_.CheckKeyRepeat(65536))
				{
					page_ = (page_ - 1 + 3) % 3;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					page_ = (page_ + 1) % 3;
				}
				int num38 = 9;
				if (scrollValue_ > select_)
				{
					scrollValue_ = select_;
				}
				if (scrollValue_ + num38 <= select_)
				{
					scrollValue_ = select_ - num38 + 1;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (type_ == 29)
					{
						return Pop();
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 30)
			{
				if (canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(32));
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(8))
				{
					return Push(new SubForm(32));
				}
			}
			else if (type_ == 32)
			{
				if (canvas_.CheckKeyRepeat(65536))
				{
					select_ = (select_ - 1 + 2) % 2;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					select_ = (select_ + 1) % 2;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 33)
			{
				formMan_.DrawAllForms();
				int num39 = page_;
				if (canvas_.CheckKeyRepeat(65536))
				{
					page_ = (page_ - 1 + 3) % 3;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					page_ = (page_ + 1 + 3) % 3;
				}
				if (num39 != page_)
				{
					if ((num39 == 0 || num39 == 1) && page_ == 2)
					{
						scrollValue_ = 0;
					}
					if ((page_ == 0 || page_ == 1) && num39 == 2)
					{
						scrollValue_ = 0;
					}
				}
				if (page_ == 0 || page_ == 1)
				{
					int num40 = udat_.league_dispOlder_.Length;
					int num41 = 6;
					int num42 = num40 - num41 + 1;
					if (num42 > 0)
					{
						if (canvas_.CheckKeyRepeat(131072))
						{
							scrollValue_ = (scrollValue_ - 1 + num42) % num42;
						}
						if (canvas_.CheckKeyRepeat(524288))
						{
							scrollValue_ = (scrollValue_ + 1) % num42;
						}
					}
				}
				else if (page_ == 2)
				{
					int num43 = ary1.Size();
					int num44 = 6;
					int num45 = num43 - num44 + 1;
					if (num45 > 0)
					{
						if (canvas_.CheckKeyRepeat(131072))
						{
							scrollValue_ = (scrollValue_ - 1 + num45) % num45;
						}
						if (canvas_.CheckKeyRepeat(524288))
						{
							scrollValue_ = (scrollValue_ + 1) % num45;
						}
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
				if (Config.DEBUG_CMD)
				{
					bool flag2 = canvas_.CheckKeyPulse(2);
					bool flag3 = canvas_.CheckKeyPulse(4);
					bool flag4 = canvas_.CheckKeyPulse(8);
					if (flag2 || flag3 || flag4)
					{
						udat_.gm_.score_[0] = 0;
						udat_.gm_.score_[1] = 0;
						if (flag2)
						{
							udat_.gm_.score_[0] = 1;
						}
						else if (!flag3)
						{
							udat_.gm_.score_[1] = 1;
						}
						if (!udat_.League_update())
						{
						}
					}
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 34)
			{
				if (canvas_.CheckKeyRepeat(131072))
				{
					scrollValue_ = AppData.ClampMin(scrollValue_ - 1, 0);
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					scrollValue_ = AppData.ClampMax(scrollValue_ + 1, 1);
				}
				int num46 = page_;
				int num47 = ary1.Size();
				if (canvas_.CheckKeyRepeat(65536))
				{
					page_ = (page_ - 1 + num47) % num47;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					page_ = (page_ + 1) % num47;
				}
				if (num46 != page_)
				{
					LeagueData leagueData2 = (LeagueData)ary1.ElementAt(page_);
					leagueData2.SetLeagueInfoExplain(textLay_);
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					SubForm subForm3 = new SubForm(35);
					LeagueData leagueData3 = (LeagueData)ary1.ElementAt(page_);
					subForm3.leagueData_ = leagueData3;
					LeagueData.RemoveNew();
					return Push(subForm3);
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					LeagueData.RemoveNew();
					return Pop();
				}
			}
			else if (type_ == 35)
			{
				if (canvas_.CheckKeyRepeat(65536))
				{
					select_ = (select_ - 1 + 2) % 2;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					select_ = (select_ + 1) % 2;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ != 0)
					{
						return Pop();
					}
					LeagueData leagueData4 = leagueData_;
					if (apdat_.ChPayMoney(leagueData4.joinCost_))
					{
						apdat_.SubMoney(leagueData4.joinCost_, 4);
						LeagueData.RegistLeague(leagueData_.id_);
						formMan_.ChangeCurrentForm(apdat_.frmGame_);
						apdat_.ExecEvent(128, leagueData4.name_ + "\t" + leagueData4.schedule_, null);
						if (leagueData4.id_ == 0 && !apdat_.CheckAlreadyEvent(174))
						{
							apdat_.ExecEvent(174, null, null);
						}
					}
					else
					{
						apdat_.ExecEvent(14, null, null);
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 36)
			{
				if (canvas_.CheckKeyPulse(1048576))
				{
					apdat_.frmGame_.SetBattleData();
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 37)
			{
				formMan_.DrawAllForms();
				if (frame_ >= ANIME_MATCH_RESULT[1] && canvas_.CheckKeyPulse(1048576))
				{
					return Pop();
				}
			}
			else if (type_ == 38)
			{
				if (frame_ == 1)
				{
					apdat_.PlayJingle(20);
				}
				if (frame_ >= VICTORY_ANIME_TIME[2])
				{
					for (int j = 0; j < fw_cnt.Length; j++)
					{
						if (fw_cnt[j] > 0)
						{
							fw_cnt[j]--;
						}
					}
					if (fw_commonWait > 0)
					{
						fw_commonWait--;
					}
					if (fw_commonWait <= 0)
					{
						int num48 = 0;
						for (int k = 0; k < fw_goRate.Length; k++)
						{
							num48 += fw_goRate[k];
						}
						int num49 = GameUtil.Random(num48);
						for (int l = 0; l < fw_goRate.Length; l++)
						{
							if (num49 < fw_goRate[l])
							{
								num49 = l;
								break;
							}
							num49 -= fw_goRate[l];
						}
						int num50 = 0;
						for (int m = 0; m < fw_cnt.Length * 2; m++)
						{
							if (fw_cnt[m % fw_cnt.Length] <= 0)
							{
								if (num50 == num49)
								{
									fw_cnt[num50] = FW_ANIME[3];
									int c2min = GameUtil.RateConvert(m % fw_cnt.Length, 0, fw_cnt.Length - 1, 24, 216 - 192 / fw_cnt.Length, true);
									int c2max = GameUtil.RateConvert(m % fw_cnt.Length, 0, fw_cnt.Length - 1, 24 + 192 / fw_cnt.Length, 216, true);
									fw_pos[num50][0] = GameUtil.RateConvert(GameUtil.Random(100), 0, 99, c2min, c2max, true);
									fw_pos[num50][1] = GameUtil.RateConvert(GameUtil.Random(100), 0, 99, 28, 91, true);
									break;
								}
								num50++;
							}
						}
						fw_commonWait = GameUtil.Random(30) + 5;
						for (int n = 0; n < fw_goRate.Length; n++)
						{
							if (n == num50)
							{
								fw_goRate[n] = 0;
							}
							else
							{
								fw_goRate[n] += 100;
							}
						}
					}
				}
				if (canvas_.CheckKeyPulse(1048576) && frame_ >= VICTORY_ANIME_TIME[3])
				{
					return Pop();
				}
			}
			else if (type_ == 39)
			{
				int num51 = ary1.Size();
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num51) % num51;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num51;
				}
				int num52 = 5;
				if (scrollValue_ > select_)
				{
					scrollValue_ = select_;
				}
				if (scrollValue_ + num52 <= select_)
				{
					scrollValue_ = select_ - num52 + 1;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					FanActData.RemoveNew();
					FanActData fanActData = (FanActData)ary1.ElementAt(select_);
					if (fanActData.state_ == 2)
					{
						apdat_.ExecEvent(127, fanActData.name_, null);
						return true;
					}
					SubForm subForm4 = new SubForm(40);
					subForm4.fanActData_ = fanActData;
					return Push(subForm4);
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					FanActData.RemoveNew();
					return Pop();
				}
			}
			else if (type_ == 40)
			{
				if (canvas_.CheckKeyRepeat(65536))
				{
					select_ = (select_ - 1 + 2) % 2;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					select_ = (select_ + 1) % 2;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					FanActData fanActData2 = fanActData_;
					if (select_ != 0)
					{
						return Pop();
					}
					int num53 = GameUtil.RateConvert(fanActData2.rank_, 0, 6, fanActData2.cost_[0], fanActData2.cost_[1], true);
					if (udat_.ChResearchPoin(fanActData2.costType_, num53))
					{
						if (fanActData2.type_ == 0)
						{
							SubForm subForm5 = new SubForm(68);
							subForm5.fanActData_ = fanActData2;
							Push(subForm5);
						}
						else
						{
							udat_.SubResearchPoint(fanActData2.costType_, num53);
							fanActData2.ChangeState(2);
							formMan_.ChangeCurrentForm(apdat_.frmGame_);
							apdat_.AddPopupMessage(35, fanActData2.name_);
							if (fanActData2.type_ == 1)
							{
								apdat_.ExecEvent(178, null, null);
							}
							else if (fanActData2.type_ == 2)
							{
								apdat_.ExecEvent(179, null, null);
							}
							else if (fanActData2.type_ == 3)
							{
								apdat_.ExecEvent(180, null, null);
							}
							else if (fanActData2.type_ == 4)
							{
								apdat_.ExecEvent(181, null, null);
							}
							else if (fanActData2.type_ == 5)
							{
								apdat_.ExecEvent(182, null, null);
							}
							else if (fanActData2.type_ == 6)
							{
								apdat_.ExecEvent(183, null, null);
							}
						}
					}
					else
					{
						apdat_.ExecEvent(13, null, null);
					}
					Pop();
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else
			{
				if (type_ != 41)
				{
					return Update2();
				}
				if (frame_ == FA2_ANIME_TIME[2] + 1)
				{
					apdat_.PlaySe(25);
				}
				if (frame_ >= FA2_ANIME_TIME[5] && frame_ <= FA2_ANIME_TIME[6])
				{
					for (int num54 = 0; num54 < 4; num54++)
					{
						UpdateMeterParam(frame_ - FA2_ANIME_TIME[5], num54);
					}
				}
				if (frame_ == FA2_ANIME_TIME[6])
				{
					for (int num55 = 0; num55 < 4; num55++)
					{
						FinishMeterParam(num55);
					}
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					FanActData fanActData3 = fanActData_;
					if (frame_ < FA2_ANIME_TIME[4])
					{
						frame_ = FA2_ANIME_TIME[4];
					}
					else
					{
						if (frame_ >= FA2_ANIME_TIME[6])
						{
							int rank_ = fanActData3.rank_;
							for (int num56 = 0; num56 < 4; num56++)
							{
								int[] array12 = GetMeterParam(num56);
								int num57 = FanActData.fa_id[num56];
								if (num57 == 3)
								{
									FanActData.AddRankUpParam(array12[6], fanActData3);
								}
								else
								{
									FanActData.AddFanParam(array12[6], num57);
								}
							}
							if (fanActData3.rank_ != rank_)
							{
								apdat_.ExecuteScript(fanActData3.execsScript_[fanActData3.rank_ - 1], true, null, null, 0, 0, -1);
							}
							return Pop();
						}
						frame_ = FA2_ANIME_TIME[6] - 1;
					}
				}
			}
			return true;
		}

		private bool Update2()
		{
			if (type_ == 42)
			{
				int num = ary1.Size();
				{ bool recoveredCondition_273013 = canvas_.CheckKeyRepeat(131072);
if (recoveredCondition_273013)
				{
					select_ = (select_ - 1 + num) % num;
				}
}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num;
				}
				int num2 = 6;
				if (scrollValue_ > select_)
				{
					scrollValue_ = select_;
				}
				if (scrollValue_ + num2 <= select_)
				{
					scrollValue_ = select_ - num2 + 1;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					SponsorData.RemoveNew();
					SponsorData sponsorData = (SponsorData)ary1.ElementAt(select_);
					if (sponsorData.state_ != 2)
					{
						SubForm subForm = new SubForm(43);
						subForm.sponsorData_ = sponsorData;
						return Push(subForm);
					}
					apdat_.ExecEvent(144, sponsorData.name_ + "\t" + MyFormBase.YEN("万", sponsorData.GetIncome()) + "\t" + sponsorData.satisfactPoint_, null);
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					SponsorData.RemoveNew();
					return Pop();
				}
			}
			else if (type_ == 43)
			{
				if (canvas_.CheckKeyRepeat(65536))
				{
					select_ = (select_ - 1 + 2) % 2;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					select_ = (select_ + 1) % 2;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					SponsorData sponsorData2 = sponsorData_;
					if (select_ != 0)
					{
						return Pop();
					}
					if (udat_.ChResearchPoin(sponsorData2.costType_, sponsorData2.cost_))
					{
						udat_.SubResearchPoint(sponsorData2.costType_, sponsorData2.cost_);
						SubForm subForm2 = new SubForm(44);
						subForm2.sponsorData_ = sponsorData_;
						Push(subForm2);
					}
					else
					{
						apdat_.ExecEvent(13, null, null);
					}
					Pop();
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 44)
			{
				for (int i = 0; i < UserData.NEGOTIATE_RESULT_TH.Length; i++)
				{
					int num3 = UserData.NEGOTIATE_RESULT_TH[i];
					int num4 = (SP_ANIME_TIME[1] - SP_ANIME_TIME[0]) * UserData.WAIT_TIME[i] / 100 + SP_ANIME_TIME[0] - 5;
					if (num3 == 1 && frame_ - num4 == 1)
					{
						apdat_.PlaySe(26);
					}
				}
				if ((Config.DEBUG_CMD || Config.PTW) && (canvas_.CheckKeyState(16) || canvas_.CheckKeyState(32)))
				{
					frame_++;
				}
				if (canvas_.CheckKeyPulse(1048576) || (Config.DEBUG_CMD && canvas_.CheckKeyState(64)))
				{
					int feelPtEf = UserData.GetFeelPtEf(frame_ - SP_ANIME_TIME[0], SP_ANIME_TIME[1] - SP_ANIME_TIME[0]);
					if (feelPtEf >= 100)
					{
						for (int j = 0; j < UserData.NEGOTIATE_RESULT_TH.Length; j++)
						{
							int num5 = UserData.NEGOTIATE_RESULT_TH[j];
							int num6 = (SP_ANIME_TIME[1] - SP_ANIME_TIME[0]) * UserData.WAIT_TIME[j] / 100 + SP_ANIME_TIME[0] - 5;
							if (frame_ < num6)
							{
								frame_ = num6;
								break;
							}
							if (j == UserData.NEGOTIATE_RESULT_TH.Length - 1 && frame_ < SP_ANIME_TIME[3])
							{
								frame_ = SP_ANIME_TIME[3];
							}
						}
					}
					if (frame_ >= SP_ANIME_TIME[4])
					{
						SponsorData sponsorData3 = sponsorData_;
						sponsorData3.feelPt_ = UserData.NEGOTIATE_RESULT_DISPVALUE[3];
						if (sponsorData3.feelPt_ >= 100)
						{
							TeamData teamData = apdat_.tData_[0];
							if (MyFormBase.Jp())
							{
								apdat_.ExecuteScript(sponsorData3.execsScript_, true, MyFormBase.LT(teamData.name_) + "\t" + sponsorData3.income_[0] + "万円", null, 0, 0, -1);
							}
							else
							{
								apdat_.ExecuteScript(sponsorData3.execsScript_, true, MyFormBase.LT(teamData.name_) + "\t" + MyFormBase.YEN("万円", sponsorData3.income_[0]), null, 0, 0, -1);
							}
							sponsorData3.ChangeState(2);
							apdat_.AddPopupMessage(27, sponsorData3.name_);
						}
						else
						{
							SubForm subForm3 = apdat_.ExecEvent(92, null, null);
							subForm3.humanCharaIndex_ = sponsorData3.id_;
						}
						Pop();
						apdat_.PlayBgm(0);
					}
				}
			}
			else if (type_ == 45)
			{
				if (canvas_.CheckKeyRepeat(131072))
				{
					if (select_ <= 2)
					{
						select_ = 4;
					}
					else if (select_ == 3)
					{
						select_ = page_;
					}
					else if (select_ == 4)
					{
						select_ = 3;
					}
				}
				else if (canvas_.CheckKeyRepeat(524288))
				{
					if (select_ <= 2)
					{
						select_ = 3;
					}
					else if (select_ == 3)
					{
						select_ = 4;
					}
					else if (select_ == 4)
					{
						select_ = page_;
					}
				}
				else if (canvas_.CheckKeyRepeat(65536))
				{
					if (select_ == 0)
					{
						select_ = 2;
					}
					else if (select_ == 1 || select_ == 2)
					{
						select_--;
					}
				}
				else if (canvas_.CheckKeyRepeat(262144))
				{
					if (select_ == 2)
					{
						select_ = 0;
					}
					else if (select_ == 0 || select_ == 1)
					{
						select_++;
					}
				}
				if (select_ < 3)
				{
					page_ = select_;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ <= 2)
					{
						SubForm subForm4 = new SubForm(47);
						subForm4.value1_ = select_;
						return Push(subForm4);
					}
					if (select_ == 3)
					{
						select_ = 4;
						if (!apdat_.CheckAlreadyEvent(159))
						{
							apdat_.ExecEvent(159, null, null);
						}
						SubForm form = new SubForm(46);
						return Push(form);
					}
					bool flag = false;
					for (int k = 0; k < TrainingData.TRA_PLAYER.Length; k++)
					{
						if (TrainingData.TRA_PLAYER[k] != -1)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						TrainingData trainingData = apdat_.trainingData_[TrainingData.TRA_SELECT_TRA];
						if (udat_.ChResearchPoin(trainingData.costType_, trainingData.cost_))
						{
							udat_.SubResearchPoint(trainingData.costType_, trainingData.cost_);
							SubForm form2 = new SubForm(49);
							return Push(form2);
						}
						apdat_.ExecEvent(12, null, null);
					}
					else
					{
						apdat_.ExecEvent(10, null, null);
					}
				}
				else
				{
					if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
					if (apdat_.IsPushSoftLabel(7) && select_ <= 2)
					{
						TrainingData.TRA_PLAYER[select_] = -1;
					}
				}
			}
			else if (type_ == 46)
			{
				int num7 = (AppData.ClampMax((page_ + 1) * 5, ary1.Size()) - 1) % 5 + 1;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num7) % num7;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num7;
				}
				int max = (ary1.Size() - 1) / 5;
				if (canvas_.CheckKeyRepeat(65536))
				{
					page_ = AppData.ClampMin(page_ - 1, 0);
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					page_ = AppData.ClampMax(page_ + 1, max);
				}
				if (select_ + page_ * num7 >= ary1.Size())
				{
					select_ = (ary1.Size() - 1) % num7;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					TrainingData.RemoveNew();
					TrainingData trainingData2 = (TrainingData)ary1.ElementAt(select_ + page_ * 5);
					if (udat_.ChResearchPoin(trainingData2.costType_, trainingData2.cost_))
					{
						TrainingData.TRA_SELECT_TRA = trainingData2.id_;
						return Pop();
					}
					apdat_.ExecEvent(12, null, null);
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					TrainingData.RemoveNew();
					return Pop();
				}
			}
			else if (type_ == 47)
			{
				int num8 = ary1.Size();
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num8) % num8;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num8;
				}
				int num9 = 8;
				if (scrollValue_ > select_)
				{
					scrollValue_ = select_;
				}
				if (scrollValue_ + num9 <= select_)
				{
					scrollValue_ = select_ - num9 + 1;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					PlayerData playerData = (PlayerData)ary1.ElementAt(select_);
					TrainingData.TRA_PLAYER[value1_] = playerData.id_;
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ != 48)
			{
				if (type_ == 49)
				{
					training_cnt++;
					if (training_cnt >= TRA_ANIME_TIME2[0] && training_cnt < TRA_ANIME_TIME2[1])
					{
						int num10 = training_cnt - TRA_ANIME_TIME2[0];
						int num11 = num10 / 25;
						int num12 = num10 % 25;
						if (num12 == TRA_AT[0])
						{
							apdat_.PlaySe(26);
						}
					}
					if (training_cnt == TRA_ANIME_TIME2[3] + 1)
					{
						apdat_.PlaySe(22);
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						if (training_cnt >= TRA_ANIME_TIME2[0] && training_cnt < TRA_ANIME_TIME2[1])
						{
							int num13 = training_cnt - TRA_ANIME_TIME2[0];
							int num14 = num13 / 25;
							int num15 = num13 % 25;
							if (num15 < TRA_AT[1])
							{
								training_cnt = num14 * 25 + TRA_AT[1] + TRA_ANIME_TIME2[0];
							}
							else
							{
								training_cnt = (num14 + 1) * 25 + TRA_AT[1] + TRA_ANIME_TIME2[0];
							}
							if (training_cnt >= TRA_ANIME_TIME2[1])
							{
								training_cnt = TRA_ANIME_TIME2[1];
							}
						}
						if (training_cnt >= TRA_ANIME_TIME2[TRA_ANIME_TIME2.Length - 1])
						{
							for (int l = 0; l < TrainingData.TRA_PLAYER.Length; l++)
							{
								if (TrainingData.TRA_PLAYER[l] == -1)
								{
									continue;
								}
								PlayerData playerData2 = apdat_.pData_[TrainingData.TRA_PLAYER[l]];
								playerData2.AddPhysicalParam(TrainingData.TRA_ADD[l]);
								if (apdat_.CheckAlreadyEvent(189))
								{
									continue;
								}
								for (int m = 0; m < playerData2.paramPhysical_.Length; m++)
								{
									if (playerData2.paramPhysical_[m] >= 1000)
									{
										apdat_.ExecEvent(189, playerData2.name_ + "\t" + MyFormBase.LT(PlayerData.PARAM_NAME[m]), null);
										break;
									}
								}
							}
							return Pop();
						}
					}
				}
				else if (type_ == 50)
				{
					int num16 = SupervisorData.TRAINING_POLICY_STR.Length;
					int num17 = select_;
					if (canvas_.CheckKeyRepeat(65536))
					{
						select_ = (select_ - 1 + num16) % num16;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						select_ = (select_ + 1) % num16;
					}
					if (num17 != select_)
					{
						if (select_ != udat_.selectTrainingPolicy_)
						{
							SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[13]);
						}
						else
						{
							SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[2]);
						}
					}
					if (canvas_.CheckKeyPulse(1048576) || apdat_.IsPushSoftLabel(13))
					{
						if (select_ != udat_.selectTrainingPolicy_)
						{
							udat_.selectTrainingPolicy_ = select_;
							SubForm subForm5 = null;
							if (udat_.selectTrainingPolicy_ == 0)
							{
								subForm5 = apdat_.ExecEvent(149, null, null);
							}
							else if (udat_.selectTrainingPolicy_ == 1)
							{
								subForm5 = apdat_.ExecEvent(150, null, null);
							}
							else if (udat_.selectTrainingPolicy_ == 2)
							{
								subForm5 = apdat_.ExecEvent(151, null, null);
							}
							if (subForm5 != null)
							{
								subForm5.humanCharaIndex_ = udat_.selectSupervisorId_;
							}
							udat_.ChangeTraining();
							apdat_.AddPopupMessage(37, null);
							Log.Info("■SubForm.java update2() UserData.POPUPMEM_ID_CHANGE_POLICY = " + 37);
						}
						return Pop();
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
				}
				else if (type_ == 51)
				{
					int num18 = select_;
					int num19 = ary1.Size();
					if (canvas_.CheckKeyRepeat(65536))
					{
						select_ = (select_ - 1 + num19) % num19;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						select_ = (select_ + 1) % num19;
					}
					if (num18 != select_)
					{
						SupervisorData supervisorData = (SupervisorData)ary1.ElementAt(select_);
						udat_.SetPlayerParam(supervisorData.paramPhysical_, supervisorData.paramPhysical_);
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						int selectSupervisorId_ = udat_.selectSupervisorId_;
						SupervisorData supervisorData2 = (SupervisorData)ary1.ElementAt(select_);
						if (selectSupervisorId_ != supervisorData2.id_)
						{
							SupervisorData.ContractSupervisor(supervisorData2.id_, false, true);
							SubForm subForm6 = apdat_.ExecEvent(113, null, null);
							subForm6.humanCharaType_ = 3;
							subForm6.humanCharaIndex_ = supervisorData2.id_;
						}
						else
						{
							SupervisorData.ContractSupervisor(supervisorData2.id_, true, true);
							SubForm subForm7 = apdat_.ExecEvent(114, null, null);
							subForm7.humanCharaType_ = 3;
							subForm7.humanCharaIndex_ = supervisorData2.id_;
						}
						result_ = 1;
						return Pop();
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						result_ = 0;
						return Pop();
					}
				}
				else if (type_ == 52)
				{
					if (subForm_ != null)
					{
						if (subForm_.result_ == 1)
						{
							subForm_ = null;
							return Pop();
						}
						subForm_ = null;
					}
					if (canvas_.CheckKeyRepeat(65536))
					{
						select_ = (select_ - 1 + 2) % 2;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						select_ = (select_ + 1) % 2;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						SupervisorData supervisorData3 = apdat_.svisorData_[udat_.selectSupervisorId_];
						if (select_ == 0)
						{
							apdat_.ExecEvent(112, null, null);
							subForm_ = new SubForm(51);
							return Push(subForm_);
						}
						SupervisorData.ContractSupervisor(supervisorData3.id_, true, true);
						SubForm subForm8 = apdat_.ExecEvent(114, null, null);
						subForm8.humanCharaType_ = 3;
						subForm8.humanCharaIndex_ = supervisorData3.id_;
						return Pop();
					}
				}
				else if (type_ == 53)
				{
					if (subForm_ != null)
					{
						if (subForm_.result_ == 0)
						{
							subForm_ = null;
							return Pop();
						}
						subForm_ = null;
					}
					int num20 = select_;
					int num21 = ary1.Size();
					if (canvas_.CheckKeyRepeat(131072))
					{
						select_ = (select_ - 1 + num21) % num21;
					}
					if (canvas_.CheckKeyRepeat(524288))
					{
						select_ = (select_ + 1) % num21;
					}
					int num22 = 5;
					if (scrollValue_ > select_)
					{
						scrollValue_ = select_;
					}
					if (scrollValue_ + num22 <= select_)
					{
						scrollValue_ = select_ - num22 + 1;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						TradeData.RemoveNew();
						TradeData tradeData = (TradeData)ary1.ElementAt(select_);
						PlayerData playerData3 = apdat_.pData_[tradeData.playerDataId_];
						apdat_.ExecEvent(73 + playerData3.category_, null, null);
						subForm_ = new SubForm(54);
						subForm_.tradeData_ = tradeData;
						return Push(subForm_);
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						TradeData.RemoveNew();
						return Pop();
					}
					if (apdat_.IsPushSoftLabel(8))
					{
						TradeData.RemoveNew();
						TradeData tradeData2 = (TradeData)ary1.ElementAt(select_);
						PlayerData playerData4 = apdat_.pData_[tradeData2.playerDataId_];
						apdat_.ExecEvent(73 + playerData4.category_, null, null);
					}
				}
				else if (type_ == 54)
				{
					if (subForm_ != null)
					{
						result_ = subForm_.result_;
						subForm_ = null;
						return Pop();
					}
					if (canvas_.CheckKeyRepeat(65536))
					{
						select_ = (select_ - 1 + 2) % 2;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						select_ = (select_ + 1) % 2;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						if (select_ != 0)
						{
							return Pop();
						}
						TradeData tradeData3 = tradeData_;
						if (apdat_.ChPayMoney(tradeData3.cost_))
						{
							apdat_.SubMoney(tradeData3.cost_, 4);
							subForm_ = new SubForm(55);
							subForm_.tradeData_ = tradeData_;
							formMan_.Push(subForm_);
						}
						else
						{
							apdat_.ExecEvent(14, null, null);
							result_ = 1;
							Pop();
						}
					}
					else if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
				}
				else if (type_ == 55)
				{
					for (int n = 0; n < UserData.NEGOTIATE_RESULT_TH.Length; n++)
					{
						int num23 = UserData.NEGOTIATE_RESULT_TH[n];
						int num24 = (SP_ANIME_TIME[1] - SP_ANIME_TIME[0]) * UserData.WAIT_TIME[n] / 100 + SP_ANIME_TIME[0] - 5;
						if (num23 == 1 && frame_ - num24 == 1)
						{
							apdat_.PlaySe(26);
						}
					}
					if ((Config.DEBUG_CMD || Config.PTW) && (canvas_.CheckKeyState(16) || canvas_.CheckKeyState(32)))
					{
						frame_++;
					}
					if (canvas_.CheckKeyPulse(1048576) || (Config.DEBUG_CMD && canvas_.CheckKeyState(64)))
					{
						int feelPtEf2 = UserData.GetFeelPtEf(frame_ - SP_ANIME_TIME[0], SP_ANIME_TIME[1] - SP_ANIME_TIME[0]);
						if (feelPtEf2 >= 100)
						{
							for (int num25 = 0; num25 < UserData.NEGOTIATE_RESULT_TH.Length; num25++)
							{
								int num26 = UserData.NEGOTIATE_RESULT_TH[num25];
								int num27 = (SP_ANIME_TIME[1] - SP_ANIME_TIME[0]) * UserData.WAIT_TIME[num25] / 100 + SP_ANIME_TIME[0] - 5;
								if (frame_ < num27)
								{
									frame_ = num27;
									break;
								}
								if (num25 == UserData.NEGOTIATE_RESULT_TH.Length - 1 && frame_ < SP_ANIME_TIME[3])
								{
									frame_ = SP_ANIME_TIME[3];
								}
							}
						}
						if (frame_ >= SP_ANIME_TIME[4])
						{
							TradeData tradeData4 = tradeData_;
							PlayerData playerData5 = apdat_.pData_[tradeData4.playerDataId_];
							tradeData4.feelPt_ = UserData.NEGOTIATE_RESULT_DISPVALUE[3];
							if (tradeData4.feelPt_ >= 100)
							{
								apdat_.ExecEvent(145, null, null);
								SubForm subForm9 = new SubForm(56);
								subForm9.tradeData_ = tradeData_;
								Push(subForm9);
								result_ = 0;
							}
							else
							{
								apdat_.ExecEvent(122, null, null);
								result_ = 1;
							}
							Pop();
							apdat_.PlayBgm(0);
						}
					}
				}
				else if (type_ == 56)
				{
					formMan_.DrawAllForms();
					if (frame_ == 1)
					{
						apdat_.PlayJingle(17);
					}
					if (frame_ >= 50 && canvas_.CheckKeyPulse(1048576))
					{
						TradeData tradeData5 = tradeData_;
						PlayerData playerData6 = apdat_.pData_[tradeData5.playerDataId_];
						int dispParamToEva = FanActData.GetDispParamToEva(tradeData5.getNameValue_, 2);
						FanActData.AddFanParam(dispParamToEva, 2);
						PlayerData.Employ(playerData6.id_);
						apdat_.ExecuteScript(tradeData5.execsScript_, true, null, null, 0, 1, playerData6.id_);
						apdat_.ExecEvent(146, null, null);
						apdat_.ExecEvent(147, null, null);
						if (!apdat_.CheckAlreadyEvent(173))
						{
							apdat_.ExecEvent(173, null, null);
						}
						return Pop();
					}
				}
				else if (type_ == 57)
				{
					if (canvas_.CheckKeyPulse(1048576))
					{
						return Pop();
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
				}
				else if (type_ == 58)
				{
					int num28 = ary1.Size();
					if (canvas_.CheckKeyRepeat(131072))
					{
						select_ = (select_ - 1 + num28) % num28;
					}
					if (canvas_.CheckKeyRepeat(524288))
					{
						select_ = (select_ + 1) % num28;
					}
					int num29 = 6;
					if (scrollValue_ > select_)
					{
						scrollValue_ = select_;
					}
					if (scrollValue_ + num29 <= select_)
					{
						scrollValue_ = select_ - num29 + 1;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						SubForm subForm10 = new SubForm(59);
						subForm10.qmData_ = (QuestMatchData)ary1.ElementAt(select_);
						QuestMatchData.RemoveNew();
						return Push(subForm10);
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						QuestMatchData.RemoveNew();
						return Pop();
					}
				}
				else if (type_ == 59)
				{
					if (canvas_.CheckKeyRepeat(131072))
					{
						scrollValue_ = AppData.ClampMin(scrollValue_ - 1, 0);
					}
					if (canvas_.CheckKeyRepeat(524288))
					{
						scrollValue_ = AppData.ClampMax(scrollValue_ + 1, 1);
					}
					if (canvas_.CheckKeyRepeat(65536))
					{
						page_ = (page_ - 1 + 2) % 2;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						page_ = (page_ + 1) % 2;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						if (page_ != 0)
						{
							return Pop();
						}
						QuestMatchData questMatchData = qmData_;
						if (apdat_.ChPayMoney(questMatchData.joinCost_) || questMatchData.joinCost_ == 0)
						{
							apdat_.SubMoney(questMatchData.joinCost_, 4);
							QuestMatchData.RegistQuestMatch(qmData_.id_);
							formMan_.ChangeCurrentForm(apdat_.frmGame_);
							apdat_.ExecEvent(132, questMatchData.name_ + "\t" + questMatchData.schedule_, null);
						}
						else
						{
							apdat_.ExecEvent(14, null, null);
						}
					}
					else if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
				}
				else if (type_ == 60 || type_ == 61)
				{
					int num30 = select_;
					int num31 = ary1.Size();
					if (canvas_.CheckKeyRepeat(131072))
					{
						select_ = (select_ - 1 + num31) % num31;
					}
					if (canvas_.CheckKeyRepeat(524288))
					{
						select_ = (select_ + 1) % num31;
					}
					int num32 = 6;
					if (scrollValue_ > select_)
					{
						scrollValue_ = select_;
					}
					if (scrollValue_ + num32 <= select_)
					{
						scrollValue_ = select_ - num32 + 1;
					}
					if (num30 != select_)
					{
						TenantData tenantData = (TenantData)ary1.ElementAt(select_);
						if (tenantData.IsTAreaOrStadiumTenant())
						{
							textLay_ = new TextLayout(false);
							textLay_.SetSize(192f, 33f);
							textLay_.SetColor(SC_WINDOW_BLACK);
							textLay_.SetLineSpace(5);
							textLay_.SetText(tenantData.explain_);
							textLay_.Flush();
						}
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						TenantData.RemoveNew();
						TenantData tenantData2 = (TenantData)ary1.ElementAt(select_);
						if (type_ == 60)
						{
							if (tenantData2.state_ == 2)
							{
								apdat_.ExecEvent(126, tenantData2.name_, null);
								return true;
							}
							SubForm subForm11 = new SubForm(63);
							subForm11.tenantId_ = tenantId_;
							subForm11.tenantDataId_ = tenantData2.id_;
							return Push(subForm11);
						}
						if (type_ == 61)
						{
							if (apdat_.ChPayMoney(tenantData2.buildCost_))
							{
								SubForm subForm12 = new SubForm(62);
								subForm12.tenantId_ = tenantId_;
								subForm12.tenantDataId_ = tenantData2.id_;
								return Push(subForm12);
							}
							apdat_.ExecEvent(14, null, null);
						}
					}
					else if (apdat_.IsPushSoftLabel(2))
					{
						TenantData.RemoveNew();
						return Pop();
					}
				}
				else if (type_ == 62)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						select_ = (select_ - 1 + 2) % 2;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						select_ = (select_ + 1) % 2;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						if (select_ != 0)
						{
							return Pop();
						}
						Tenant tenant = udat_.tenants_[tenantId_];
						TenantData tenantData3 = apdat_.tenantData_[tenantDataId_];
						if (!apdat_.ChPayMoney(tenantData3.buildCost_))
						{
							apdat_.ExecEvent(14, null, null);
							return Pop();
						}
						apdat_.SubMoney(tenantData3.buildCost_, 1);
						tenant.BuildTenant(tenantData3.id_);
						apdat_.mapMain_.UpdateTenantMapcip();
						formMan_.ChangeCurrentForm(apdat_.frmGame_);
						apdat_.AddPopupMessage(31, tenantData3.name_);
					}
					else if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
				}
				else if (type_ == 63)
				{
					TenantData tenantData4 = apdat_.tenantData_[tenantDataId_];
					if (tenantData4.lv_ < 5)
					{
						if (canvas_.CheckKeyRepeat(65536))
						{
							select_ = (select_ - 1 + 2) % 2;
						}
						if (canvas_.CheckKeyRepeat(262144))
						{
							select_ = (select_ + 1) % 2;
						}
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						if (select_ != 0)
						{
							return Pop();
						}
						if (!tenantData4.IsStreangth())
						{
							return Pop();
						}
						if (!apdat_.ChPayMoney(tenantData4.paramStrengthCost_))
						{
							apdat_.ExecEvent(14, null, null);
							return Pop();
						}
						apdat_.SubMoney(tenantData4.paramStrengthCost_, 1);
						if (!tenantData4.IsTAreaOrStadiumTenant())
						{
							tenantData4.StrengthTenant();
							formMan_.ChangeCurrentForm(apdat_.frmGame_);
							apdat_.AddPopupMessage(32, tenantData4.name_);
						}
						else
						{
							if (!tenantData4.IsStreangth())
							{
								return Pop();
							}
							formMan_.ChangeCurrentForm(apdat_.frmGame_);
							TenantData.ProcStrenSpecialTenant(tenantData4);
							if (tenantData4.id_ == TenantData.TENANT_ID_STADIUM)
							{
								apdat_.AddPopupMessage(34, tenantData4.name_);
							}
							else
							{
								apdat_.AddPopupMessage(33, tenantData4.name_);
							}
						}
					}
					else if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
				}
				else if (type_ == 64)
				{
					if (frame_ == 3 && page_ == 0)
					{
						apdat_.PlaySe(25);
					}
					if (page_ == 0)
					{
						if (frame_ >= TLUP_ANIME_TIME[4] && frame_ <= TLUP_ANIME_TIME[5])
						{
							for (int num33 = 0; num33 < 2; num33++)
							{
								UpdateMeterParam(frame_ - TLUP_ANIME_TIME[4], num33);
							}
						}
					}
					else if (frame_ >= TLUP_ANIME_TIME2[2] && frame_ <= TLUP_ANIME_TIME2[3])
					{
						for (int num34 = 0; num34 < 4; num34++)
						{
							UpdateMeterParam(frame_ - TLUP_ANIME_TIME2[2], num34 + 2);
						}
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						if (page_ == 0)
						{
							if (frame_ >= TLUP_ANIME_TIME[5])
							{
								page_ = 1;
								frame_ = 0;
							}
						}
						else if (frame_ >= TLUP_ANIME_TIME2[3])
						{
							TenantData tenantData5 = apdat_.tenantData_[tenantDataId_];
							tenantData5.LvUp();
							apdat_.AddPopupMessage(30, tenantData5.name_ + "\t" + tenantData5.lv_);
							return Pop();
						}
					}
				}
				else if (type_ == 65)
				{
					if (canvas_.CheckKeyPulse(1048576))
					{
						return Pop();
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
					if (apdat_.IsPushSoftLabel(10))
					{
						Pop();
						SubForm subForm13 = new SubForm(66);
						subForm13.tenantId_ = tenantId_;
						subForm13.tenantDataId_ = tenantDataId_;
						return formMan_.Push(subForm13);
					}
				}
				else if (type_ == 66)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						select_ = (select_ - 1 + 2) % 2;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						select_ = (select_ + 1) % 2;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						Pop();
						if (select_ == 0 && tenantId_ != -1)
						{
							Tenant tenant2 = udat_.tenants_[tenantId_];
							tenant2.DestructTenant();
						}
					}
					else if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
				}
				else if (type_ == 67)
				{
					if (canvas_.CheckKeyPulse(1048576))
					{
						return Pop();
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						return Pop();
					}
				}
				else if (type_ == 68)
				{
					if (dialog_ != null)
					{
						if (dialog_.result_ == 0)
						{
							dialog_ = null;
							int sub = GameUtil.RateConvert(fanActData_.rank_, 0, 6, fanActData_.cost_[0], fanActData_.cost_[1], true);
							udat_.SubResearchPoint(fanActData_.costType_, sub);
							apdat_.ExecEvent(141, null, null);
							return Pop();
						}
						dialog_ = null;
					}
					int standartTicketPrice = udat_.GetStandartTicketPrice(apdat_.frmSoccer_.GetEvaCumstomer(false));
					int min = 1000;
					int val = udat_.teamEvaRank_ * 2000 + 3000;
					val = AppData.ClampMax(val, 20000);
					val -= val % 1000;
					int ticketPrice_ = udat_.ticketPrice_;
					if (canvas_.CheckKeyRepeat(65536))
					{
						udat_.ticketPrice_ = AppData.ClampMax(udat_.ticketPrice_ + 1000, val);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						udat_.ticketPrice_ = AppData.ClampMin(udat_.ticketPrice_ - 1000, min);
					}
					if (ticketPrice_ != udat_.ticketPrice_)
					{
						int num35 = standartTicketPrice - udat_.ticketPrice_;
						if (num35 >= 2400)
						{
							SetTicketWindowBtmMsg("安すぎでないでしょうか。");
						}
						else if (num35 >= 800)
						{
							SetTicketWindowBtmMsg("お買い得な値段ですね。");
						}
						else if (num35 >= -800)
						{
							SetTicketWindowBtmMsg("適正価格だと思います。");
						}
						else if (num35 >= -2400)
						{
							SetTicketWindowBtmMsg("少し高い気がします…。");
						}
						else
						{
							SetTicketWindowBtmMsg("高すぎです！");
						}
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						dialog_ = MakeDialog(MyFormBase.LT("チケットの価格は\n<0>でよろしいですか", MyFormBase.YEN("円", udat_.ticketPrice_)), "はい\tいいえ");
						return Push(dialog_);
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						udat_.ticketPrice_ = value1_;
						return Pop();
					}
				}
				else if (type_ == 69)
				{
					if (frame_ >= SCENE_CHANGE_TIME[value1_])
					{
						apdat_.frmGame_.ChangeState(3);
						return Pop();
					}
				}
				else
				{
					if (type_ == 70)
					{
						apdat_.frmGame_.ChangeState(0);
						apdat_.frmGame_.SetAllDraw();
						return Pop();
					}
					if (type_ == 71)
					{
						if (canvas_.CheckKeyRepeat(131072))
						{
							select_ = (select_ - 1 + 4) % 4;
						}
						if (canvas_.CheckKeyRepeat(524288))
						{
							select_ = (select_ + 1) % 4;
						}
						if (canvas_.CheckKeyRepeat(65536))
						{
							udat_.selectUniform_ = (udat_.selectUniform_ - 1 + 8) % 8;
							select_ = 2;
						}
						if (canvas_.CheckKeyRepeat(262144))
						{
							udat_.selectUniform_ = (udat_.selectUniform_ + 1) % 8;
							select_ = 2;
						}
						if (canvas_.CheckKeyPulse(1048576))
						{
							TeamData t_data = apdat_.tData_[0];
							if (select_ == 0)
							{
								try
								{
									MainPanel mainPanel = new MainPanel("チーム名(全角７文字)", t_data.name_, 14);
									mainPanel.Show(canvas_, delegate(string temp_name)
									{
										if (temp_name != null)
										{
											t_data.name_ = JString.New(temp_name);
											apdat_.frmTitle_.newGameName_ = JString.New(temp_name);
											string text = string.Empty;
											int num38 = 0;
											int num39 = 0;
											for (int num40 = 0; num40 < temp_name.Length(); num40++)
											{
												char c = temp_name.CharAt(num40);
												int stringLength = GetStringLength(JString.ValueOf(c));
												if (num40 == 0)
												{
													num39 = ((!MyFormBase.Jp()) ? 8 : ((stringLength != 1) ? 4 : 5));
												}
												num38 += stringLength;
												if (num38 > num39)
												{
													break;
												}
												text += JString.ValueOf(c);
											}
											t_data.name2_ = text;
										}
									});
								}
								finally
								{
									formMan_.DrawAllForms();
								}
							}
							else if (select_ == 1)
							{
								try
								{
									MainPanel mainPanel2 = new MainPanel("短縮名(全角４文字)", t_data.name2_, 8);
									mainPanel2.Show(canvas_, delegate(string temp_name)
									{
										if (temp_name != null)
										{
											t_data.name2_ = JString.New(temp_name);
										}
									});
								}
								finally
								{
									formMan_.DrawAllForms();
								}
							}
							else if (select_ != 2 && select_ == 3)
							{
								try
								{
									apdat_.NewGame(apdat_.frmTitle_.selectSave_, apdat_.frmTitle_.newGameName_);
									udat_.NewGame();
									formMan_.ChangeCurrentForm(apdat_.frmGame_);
								}
								catch (Exception t)
								{
									apdat_.save_ = null;
									if (Config.PRINT)
									{
										Log.Info(t);
									}
									apdat_.save_ = null;
									Push(MakeDialog("ゲーム開始に失敗しました"));
								}
							}
						}
						else if (apdat_.IsPushSoftLabel(2))
						{
							return Pop();
						}
					}
					else if (type_ == 72)
					{
						formMan_.DrawAllForms();
						if (canvas_.CheckKeyRepeat(65536))
						{
							select_ = (select_ - 1 + 2) % 2;
						}
						if (canvas_.CheckKeyRepeat(262144))
						{
							select_ = (select_ + 1) % 2;
						}
						if (canvas_.CheckKeyPulse(1048576))
						{
							if (select_ != 0)
							{
								apdat_.ExecEvent(116, null, null);
								return Pop();
							}
							if (udat_.memberPDataist_.Size() > 11)
							{
								apdat_.ExecEvent(117, null, null);
								return Push(new SubForm(31));
							}
							apdat_.ExecEvent(121, null, null);
						}
						else if (apdat_.IsPushSoftLabel(2))
						{
							return Pop();
						}
					}
					else
					{
						if (type_ == 73)
						{
							udat_.Training_init();
							udat_.Init_bus();
							return Pop();
						}
						if (type_ == 74)
						{
							if (canvas_.CheckKeyRepeat(65536))
							{
								page_ = (page_ - 1 + 2) % 2;
							}
							if (canvas_.CheckKeyRepeat(262144))
							{
								page_ = (page_ + 1) % 2;
							}
							if (canvas_.CheckKeyPulse(1048576))
							{
								return Pop();
							}
							if (apdat_.IsPushSoftLabel(2))
							{
								return Pop();
							}
						}
						else if (type_ == 75 || type_ == 76)
						{
							if (frame_ >= FADE_TIME_V)
							{
								return Pop();
							}
						}
						else if (type_ == 77)
						{
							int num36 = ary1.Size();
							if (canvas_.CheckKeyRepeat(131072))
							{
								select_ = (select_ - 1 + num36) % num36;
							}
							if (canvas_.CheckKeyRepeat(524288))
							{
								select_ = (select_ + 1) % num36;
							}
							int num37 = 9;
							if (scrollValue_ > select_)
							{
								scrollValue_ = select_;
							}
							if (scrollValue_ + num37 <= select_)
							{
								scrollValue_ = select_ - num37 + 1;
							}
							if (canvas_.CheckKeyPulse(1048576))
							{
								FanBonusData fanBonusData = (FanBonusData)ary1.ElementAt(select_);
								if (fanBonusData.state_ != 0)
								{
									return Pop();
								}
								apdat_.ExecEvent(190, null, null);
							}
							else if (apdat_.IsPushSoftLabel(2))
							{
								return Pop();
							}
						}
						else if (type_ == 78)
						{
							formMan_.DrawAllForms();
							apdat_.resCom2_.seb[60].Frame();
							if (frame_ >= udat_.memberPDataist_.Size() * 12 + 20 || canvas_.CheckKeyPulse(1048576))
							{
								apdat_.frmGame_.SetBattleData();
								if (udat_.GetMatchInfoJoin() == 0 && !apdat_.CheckAlreadyEvent(176))
								{
									apdat_.ExecEvent(176, null, null);
								}
								Push(new SubForm(79));
								Pop();
							}
						}
						else if (type_ == 79)
						{
							formMan_.DrawAllForms();
							if (canvas_.CheckKeyPulse(1048576) && frame_ >= 5)
							{
								apdat_.camera.SaveMapMainBasePos();
								formMan_.ChangeCurrentForm(apdat_.frmSoccer_);
							}
						}
						else if (type_ == 80)
						{
							apdat_.frmGame_.ChangeState(value1_);
							Pop();
						}
					}
				}
			}
			return true;
		}

		private void ScrollControl(int num, int showMax)
		{
			if (num <= 0) { return; }
{
if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num) % num;
				}
				else if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num;
				}
ScrollUpdate(num, showMax);
}
		}

		public void ScrollUpdate(int num, int showMax)
		{
			ScrollUpdate(num, showMax, select_);
		}

		public void ScrollUpdate(int num, int showMax, int value)
		{
			if (num <= 0) { return; }
if (value >= scrollValue_ + showMax)
				{
					scrollValue_ = value - (showMax - 1);
				}
if (value < scrollValue_)
				{
					scrollValue_ = value;
				}
		}

		private void AddMenu(int menuId, int state)
		{
			if (menus_ == null)
			{
				menus_ = new int[0][];
			}
			int[][] array = new int[menus_.Length + 1][];
			if (menus_.Length > 0)
			{
				JSystem.Arraycopy(menus_, 0, array, 0, menus_.Length);
			}
			array[array.Length - 1] = new int[2] { menuId, state };
			menus_ = array;
		}

		private void DrawMenu(Graphics g, int type)
		{
			int num = 3;
			int num2 = 29;
			int num3 = num2 * menus_.Length + 5;
			if (MyFormBase.Jp())
			{
				for (int i = 0; i < WIDTH.Length; i++)
				{
					WIDTH[i] = WIDTH_DATA[0][i];
				}
			}
			else
			{
				for (int j = 0; j < WIDTH.Length; j++)
				{
					WIDTH[j] = WIDTH_DATA[1][j];
				}
			}
			int[] array = new int[3] { 20, 6, 0 };
			int num4 = WIDTH[type];
			int num5 = array[type];
			if (MyFormBase.Jp())
			{
				num5 = 0;
			}
			GameView instance = GameView.GetInstance();
			int num6 = 0;
			int num7 = 0;
			if (g.GetOriginX() + num4 > instance.GetGameWidth())
			{
				num6 = instance.GetGameWidth() - num4 - g.GetOriginX();
			}
			if (g.GetOriginY() + num3 > instance.GetGameHeight())
			{
				num7 = instance.GetGameHeight() - num3 - g.GetOriginY();
			}
			for (int k = 0; k < menus_.Length; k++)
			{
				int[] array2 = menus_[k];
				int num8 = 1000 * frame_ / num;
				int num9 = type * 2 + 1;
				if (k == select_)
				{
					num9--;
				}
				if (MyFormBase.Jp())
				{
					apdat_.DrawSeb(g, num6 + 1, num7 + 1 + num2 * k, apdat_.resCom2_.seb[22], apdat_.resCom2_.img, num9, num8, num8);
				}
				else
				{
					int num10 = num6 + 1;
					int num11 = num7 + 1 + num2 * k;
					int num12 = 0;
					if (type == 0)
					{
						if (k != select_)
						{
							num12 += 30;
						}
						int num13 = num4 - 34 - 72;
						g.DrawImage(apdat_.resCom2_.img[25], num10, num11, 0, num12, 24, 30);
						num10 += 24;
						g.DrawImage(apdat_.resCom2_.img[25], num10, num11, 24, num12, 72, 30);
						num10 += 72;
						g.DrawImage(apdat_.resCom2_.img[25], num10, num11, 24, num12, num13, 30);
						num10 += num13;
						g.DrawImage(apdat_.resCom2_.img[25], num10, num11, 96, num12, 10, 30);
					}
					else
					{
						if (k != select_)
						{
							num12 += 30;
						}
						int num14 = num4 - 15 - 70;
						g.DrawImage(apdat_.resCom2_.img[25], num10, num11, 106, num12, 4, 30);
						num10 += 4;
						g.DrawImage(apdat_.resCom2_.img[25], num10, num11, 110, num12, 70, 30);
						num10 += 70;
						g.DrawImage(apdat_.resCom2_.img[25], num10, num11, 110, num12, num14, 30);
						num10 += num14;
						g.DrawImage(apdat_.resCom2_.img[25], num10, num11, 180, num12, 11, 30);
					}
				}
				int num15 = num6 + 10;
				if (type == 0)
				{
					num15 += 10;
				}
				int num16 = num7 + 7 + num2 * k;
				int num17 = num15;
				int num18 = num16;
				if (type_ == 3)
				{
					int frame = 0;
					int num19 = array2[0];
					if (num19 == 0)
					{
						frame = 0;
					}
					if (num19 == 1)
					{
						frame = 1;
					}
					if (num19 == 3)
					{
						frame = 2;
					}
					if (num19 == 2)
					{
						frame = 3;
					}
					if (num19 == 4)
					{
						frame = 4;
					}
					if (num19 == 5)
					{
						frame = 5;
					}
					apdat_.resCom2_.DrawSeb(g, num15 - 17, num16 - 2, 93, frame);
					num17 += 42 + num5 / 2;
					GameView.AddTouch(g, 8, num6, num7 + num2 * k, num4, num2, 0x20000 | k);
				}
				else if (type_ == 4)
				{
					int frame2 = 0;
					int num20 = array2[0];
					if (num20 == 21)
					{
						frame2 = 4;
					}
					if (num20 == 22)
					{
						frame2 = 5;
					}
					apdat_.resCom2_.DrawSeb(g, num15 - 17, num16 - 2, 93, frame2);
					num17 += 42 + num5 / 2;
					GameView.AddTouch(g, 8, num6, num7 + num2 * k, num4, num2, 0x20000 | k);
				}
				else
				{
					num17 += 34 + num5 / 2;
					GameView.AddTouch(g, 9, num6, num7 + num2 * k, num4, num2, 0x20000 | k);
				}
				if (k == select_)
				{
					g.SetColor(SC_MAINMENU_SELECT);
				}
				else
				{
					g.SetColor(SC_MAINMENU_NOTSELECT);
				}
				if (frame_ >= num)
				{
					g.DrawString(MENU_STR[array2[0]], num17, num18 + 2, 2);
				}
				if (frame_ < num)
				{
					continue;
				}
				if (type_ == 3)
				{
					int recoveredSelector1 = array2[0];
if (recoveredSelector1 == 0) {
if (LeagueData.IsExistNew() || QuestMatchData.IsExistNew())
						{
							Draw_newMark(g, num15 - 18, num16 - 5, frame2_);
						}
} else if (recoveredSelector1 == 1) {
if (TrainingData.IsExistNew())
						{
							Draw_newMark(g, num15 - 18, num16 - 5, frame2_);
						}
} else if (recoveredSelector1 == 2) {
if (TradeData.IsExistNew() || FanActData.IsExistNew() || TenantData.IsExistNew() || SponsorData.IsExistNew())
						{
							Draw_newMark(g, num15 - 18, num16 - 5, frame2_);
						}
} else if (recoveredSelector1 == 3) {
if (FormationData.IsExistNew() || PlayerData.IsExistNew())
						{
							Draw_newMark(g, num15 - 18, num16 - 5, frame2_);
						}
}
				}
				else if (type_ == 5)
				{
					int num21 = -12;
					int recoveredSelector2 = array2[0];
if (recoveredSelector2 == 6) {
if (LeagueData.IsExistNew())
						{
							Draw_newMark(g, num15 - 7 + num21, num16 - 5, frame2_);
						}
} else if (recoveredSelector2 == 8) {
if (QuestMatchData.IsExistNew())
						{
							Draw_newMark(g, num15 - 7 + num21, num16 - 5, frame2_);
						}
}
				}
				else if (type_ == 7)
				{
					int num22 = -12;
					int recoveredSelector3 = array2[0];
if (recoveredSelector3 == 12) {
if (TradeData.IsExistNew())
						{
							Draw_newMark(g, num15 - 7 + num22, num16 - 5, frame2_);
						}
} else if (recoveredSelector3 == 14) {
if (FanActData.IsExistNew())
						{
							Draw_newMark(g, num15 - 7 + num22, num16 - 5, frame2_);
						}
} else if (recoveredSelector3 == 15) {
if (TenantData.IsExistNew())
						{
							Draw_newMark(g, num15 - 7 + num22, num16 - 5, frame2_);
						}
} else if (recoveredSelector3 == 16) {
if (SponsorData.IsExistNew())
						{
							Draw_newMark(g, num15 - 7 + num22, num16 - 5, frame2_);
						}
}
				}
				if (k == select_ && apdat_.IsTopForm(this))
				{
					int num23 = 0;
					if (type == 2)
					{
						num23 = 3;
					}
					if (num6 < 120)
					{
						apdat_.resCom2_.seb[61].Draw(g, num6 + num4 + num23 + 2, num7 + 5 + num2 * k + 8, apdat_.resCom2_.img);
					}
					else
					{
						apdat_.resCom2_.seb[60].Draw(g, num6 - 2, num7 + 5 + num2 * k + 8, apdat_.resCom2_.img);
					}
				}
			}
		}

		public void DrawMenu_btmInfo(Graphics g)
		{
			int num = 0;
			int num2 = 0;
			TeamData teamData = apdat_.tData_[0];
			GameView instance = GameView.GetInstance();
			int matchInfoJoin = udat_.GetMatchInfoJoin();
			if (matchInfoJoin != 2)
			{
				int num3 = instance.GetSideSoftWidth() + 1;
				num = instance.GetGameHeight() - 43 - 25;
				g.SetColor(Graphics.GetColorOfRGB(46, 209, 80));
				g.DrawRect(num3, num, 111f, 41f);
				g.SetColor(Graphics.GetColorOfRGB(34, 154, 59));
				g.DrawRect(num3 + 1, num + 1, 109f, 19f);
				g.DrawRect(num3 + 1, num + 1 + 20, 109f, 19f);
				g.SetColor(Graphics.GetColorOfRGB(22, 96, 37));
				g.FillRect(num3 + 2, num + 2, 108f, 18f);
				g.FillRect(num3 + 2, num + 2 + 20, 108f, 18f);
				SetFontSize(g, 10);
				int num4 = 0;
				switch (matchInfoJoin)
				{
				case 0:
				{
					LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
					g.SetColor(Graphics.GetColorOfRGB(255, 236, 238));
					g.DrawString(leagueData.name_, num3 + 5, num + 5);
					int[] leagueData2 = udat_.GetLeagueData(0);
					g.DrawString(MyFormBase.LT("<0>位", leagueData2[7]), num3 + 105, num + 5, 4);
					num4 = leagueData.schedule_ - udat_.matchInfoWeek_;
					break;
				}
				case 1:
				{
					QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
					g.SetColor(Graphics.GetColorOfRGB(255, 236, 238));
					g.DrawString(questMatchData.name_, num3 + 5, num + 5);
					num4 = questMatchData.schedule_ - udat_.matchInfoWeek_;
					break;
				}
				}
				g.DrawString("次の試合", num3 + 5, num + 5 + 20);
				int num5 = (apdat_.save_.ints[2] + (apdat_.save_.ints[3] + num4) / 4) % 12 + 1;
				int num6 = (apdat_.save_.ints[3] + num4) % 4 + 1;
				g.DrawString(MyFormBase.LT("<0>月<1>週", num5 + "\t" + num6), num3 + 105, num + 5 + 20, 4);
				ResetFontSize(g);
			}
			int num7 = instance.GetGameWidth() - 123 - instance.GetSideSoftWidth();
			num2 = instance.GetGameHeight() - 106;
			g.DrawImage(apdat_.resCom2_.img[134], num7, num2);
			num7 = instance.GetGameWidth() - 91 - instance.GetSideSoftWidth();
			num2 = instance.GetGameHeight() - 99;
			int rankIndex = udat_.GetRankIndex();
			if (En())
			{
				apdat_.DrawNumImage(g, 35, rankIndex + 1, num7 + 6, num2, 0, 2);
			}
			else
			{
				apdat_.DrawNumImage(g, 35, rankIndex + 1, num7, num2, 0, 2);
			}
			g.SetColor(SC_WINDOW_BLACK);
			SetFontSize(g, 10);
			if (En())
			{
				g.DrawString(UserData.RANK_NAME[rankIndex], num7 + 51, num2, 2);
			}
			else
			{
				g.DrawString(UserData.RANK_NAME[rankIndex], num7 + 51, num2 - 2, 2);
			}
			ResetFontSize(g);
			num7 = instance.GetGameWidth() - 2 - instance.GetSideSoftWidth();
			num2 = instance.GetGameHeight() - 84;
			apdat_.DrawNumImageComma(g, 78, teamData.warPotential_, num7 - 14, num2, 0, 0, 4);
			num = instance.GetGameHeight() - 70;
			for (int i = 0; i < 3; i++)
			{
				num7 = instance.GetGameWidth() - 2 - instance.GetSideSoftWidth();
				num2 = num + 14 * i;
				int fanEva = FanActData.GetFanEva(i);
				int fanDispParam = FanActData.GetFanDispParam(fanEva, i);
				switch (i)
				{
				case 0:
					apdat_.DrawNumImageComma(g, 78, fanDispParam, num7 - 14, num2, 0, 0, 4);
					break;
				case 1:
				{
					int num8 = 52;
					int num9 = 0;
					num9 = GameUtil.RateConvert(fanDispParam, 0, 100, 0, num8 - 2, true);
					g.DrawImage(apdat_.resCom2_.img[129], num7 - 52 + 1 - 13, num2 + 2, 1, 10, num9, 8);
					break;
				}
				case 2:
					apdat_.DrawNumImage(g, 78, fanDispParam, num7 - 14, num2 + 1, 0, 4);
					break;
				}
			}
		}

		private int[] FrameMenu()
		{
			return FrameMenu(-1);
		}

		private int[] FrameMenu(int backResult)
		{
			int num = 3;
			frame_++;
			if (frame_ > num)
			{
				frame_ = num;
			}
			formMan_.DrawAllForms();
			if (canvas_.CheckKeyRepeat(131072))
			{
				select_ = (select_ + menus_.Length - 1) % menus_.Length;
			}
			if (canvas_.CheckKeyRepeat(524288))
			{
				select_ = (select_ + 1) % menus_.Length;
			}
			if (canvas_.CheckKeyPulse(1048576) || ((type_ == 3 || type_ == 4 || type_ == 5 || type_ == 6 || type_ == 7 || type_ == 12 || type_ == 11 || type_ == 9 || type_ == 10 || type_ == 8) && canvas_.CheckKeyPulse(262144)))
			{
				frame_ = num;
				formMan_.DrawAllForms();
				return menus_[select_];
			}
			if (apdat_.IsPushSoftLabel(2) || ((type_ == 3 || type_ == 4 || type_ == 5 || type_ == 6 || type_ == 7 || type_ == 12 || type_ == 11 || type_ == 9 || type_ == 10 || type_ == 8) && canvas_.CheckKeyPulse(65536)))
			{
				result_ = backResult;
				formMan_.Pop();
			}
			return null;
		}

		private void SetEvent(int index)
		{
			int num = eventIds_[index];
		}

		public void DrawInputValue(Graphics g, int right, int top, int value, bool inputMode, bool active, int color, int fig)
		{
			g.SetColor(Graphics.GetColorOfRGB(200, 200, 255));
			int num = 6 * fig + 2;
			if (active)
			{
				if (!inputMode)
				{
					g.FillRect(right - num - 1, top - 1, num + 2, 14f);
				}
				else
				{
					g.SetColor(Graphics.GetColorOfRGB(205, 235, 111));
					g.FillRect(right - 6 - inputFig_ * 6 - 1, top, 6f, 12f);
					if (apdat_.IsTopForm(this))
					{
					}
				}
			}
			g.SetColor(color);
			g.DrawString(JString.ValueOf(value), right, top, 4);
		}

		public bool IsMenu()
		{
			return type_ == 3 || type_ == 4 || type_ == 5 || type_ == 6 || type_ == 7 || type_ == 12 || type_ == 22 || type_ == 11 || type_ == 8 || type_ == 9 || type_ == 10;
		}

		public bool IsDrawWindow()
		{
			return type_ == 26 || type_ == 69;
		}

		public new static int GetBoxHeaderY(int boxTop)
		{
			return boxTop - 16;
		}

		public new static int GetBoxFooterY(int boxBottom)
		{
			return boxBottom + 4;
		}

		public void DrawCommand(Graphics g, string str, int x, int y, bool active, bool cursor)
		{
			DrawCommand(g, str, x, y, 0, active, 1, cursor);
		}

		public void DrawCommand(Graphics g, string str, int x, int y, int anchor, bool active, bool cursor)
		{
			DrawCommand(g, str, x, y, anchor, active, 1, cursor);
		}

		public void DrawCommand(Graphics g, string str, int x, int y, int anchor, bool active, int margin, bool cursor)
		{
			apdat_.DrawCommand(g, str, x, y, anchor, active, margin, cursor, this, loopCounter_);
		}

		private void DrawMeter(Graphics g, int x, int y, int value, int max)
		{
			DrawMeter(g, x, y, value, max, 40, false);
		}

		private void DrawMeter(Graphics g, int x, int y, int value, int max, int scaleNum)
		{
			DrawMeter(g, x, y, value, max, scaleNum, false);
		}

		private void DrawMeter(Graphics g, int x, int y, int value, int max, int scaleNum, bool rev)
		{
			value = AppData.Clamp(value, 0, max);
			x -= 28;
			y -= 2;
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
			g.FillRect(x + 28, y + 2, scaleNum * 2 + 3, 8f);
			g.SetColor(Graphics.GetColorOfRGB(64, 64, 64));
			g.FillRect(x + 29, y + 3, scaleNum * 2 + 1, 6f);
			int num = max / scaleNum;
			int num2 = value;
			int num3 = x + 30;
			if (rev)
			{
				num3 += (scaleNum - 1) * 2;
			}
			for (int i = 0; i < scaleNum; i++)
			{
				int num4 = 4;
				g.SetColor(Graphics.GetColorOfRGB(32, 32, 32));
				g.FillRect(num3, y + 4, 1f, num4);
				int num5 = AppData.ClampMax(num2 * num4 / num, num4);
				if (num5 > 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(64, 255, 128));
					g.FillRect(num3, y + 4 + num4 - num5, 1f, num5);
				}
				num2 -= num;
				if (rev) { num3 = (num3 - 2); } else { num3 = (num3 + 2); }
			}
		}

		public static void Draw_moveToBus(Graphics g, int frame)
		{
			int gWidth = 190;
			int gHeight = 106;
			GameView instance = GameView.GetInstance();
			int a = GameUtil.RateConvert(frame, 0, 100, 0, 180, true);
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, a));
			int originX = g.GetOriginX();
			int originY = g.GetOriginY();
			g.SetOrigin(0, 0);
			g.FillRect(0f, 0f, instance.GetGameWidth(), instance.GetGameHeight());
			g.SetOrigin(originX, originY);
			AppData instance2 = AppData.GetInstance();
			UserData instance3 = UserData.GetInstance();
			int num = 4;
			instance2.DrawWindow(g, gWidth, gHeight, -15, "バスに乗り込み中", 3 + num);
			instance2.DrawBox(g, 30, 80 + num, 209, 80 + num + 71);
			int num2 = 32;
			int num3 = 91;
			int num4 = 176;
			g.ClipRect(32f, 91f, 175f, 72f);
			num3 = 93 + num;
			for (int i = 0; i < 3; i++)
			{
				g.DrawImage(instance2.resCom2_.img[184], 0 + 80 * i, num3);
			}
			g.PopClip();
			int num5 = 0;
			int num6 = 158 + num;
			int frame2 = frame % 16 / 4;
			instance2.DrawSeb(g, 165, num6 - 4, instance2.resCom2_.seb[46], instance2.resCom2_.img[50], 1);
			g.ClipRect(32f, 70 + num, 179f, 126f);
			for (int j = 0; j < instance3.memberPDataist_.Size(); j++)
			{
				PlayerData playerData = (PlayerData)instance3.memberPDataist_.ElementAt(j);
				num5 = 25 - j * 15 + 2 * frame;
				if (num5 >= 25 && num5 <= 165)
				{
					instance3.SetDispPlayerData(0, instance3.selectUniform_, playerData.faceImg_, frame2, 1);
					instance3.DrawDispPlayer(g, num5, num6);
				}
			}
			g.PopClip();
		}

		public static void Draw_battlePic(Graphics g, int frame)
		{
			GameView instance = GameView.GetInstance();
			int originX = g.GetOriginX();
			int originY = g.GetOriginY();
			g.SetOrigin(0, 0);
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, 180));
			g.FillRect(GameForm.VIEW_X, 0f, GameForm.VIEW_W, instance.GetGameHeight());
			g.SetOrigin(originX, originY);
			AppData instance2 = AppData.GetInstance();
			UserData instance3 = UserData.GetInstance();
			int num3;
			int num2;
			int num4;
			int num = (num2 = (num3 = (num4 = 0)));
			TeamData teamData = instance2.tData_[instance3.gm_.teamId_[0]];
			TeamData teamData2 = instance2.tData_[instance3.gm_.teamId_[1]];
			int matchInfoJoin = instance3.GetMatchInfoJoin();
			switch (matchInfoJoin)
			{
			case 0:
				num = 25;
				instance2.frmGame_.Draw_stadiumName(g, num, 120);
				break;
			case 1:
			{
				QuestMatchData questMatchData = instance2.qmData_[instance3.selectQmId_];
				num = 9;
				instance3.gm_.Draw_timeMessageWindow(g, num, 222, 1, 120);
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				g.DrawString(MyFormBase.LT("<0>  <1>回戦", questMatchData.name_ + "\t" + (instance3.battleQmIndex_ + 1)), 120f, num + 3, 2);
				num = 32;
				instance2.frmGame_.Draw_stadiumName(g, num, 120);
				break;
			}
			}
			num4 = GameUtil.RateConvert(frame, 10, 18, 120, 0, true);
			num4 = 0;
			int num5 = 4;
			num = 61;
			g.SetColor(Graphics.GetColorOfRGB(0, 162, 35));
			g.FillRect(num5, num, 232f, 17f);
			g.SetColor(Graphics.GetColorOfRGB(3, 59, 63));
			g.DrawLine(num5 - 1, num, num5 - 1, num + 16);
			g.DrawLine(num5 + 232, num, num5 + 232, num + 16);
			g.DrawLine(num5 - 1, num - 1, num5 + 232, num - 1);
			instance2.DrawSeb(g, 7, num + 4, instance2.resCom2_.seb[28], instance2.resCom2_.img[33], teamData.uniform_);
			instance2.DrawSeb(g, 221 + num4, num + 4, instance2.resCom2_.seb[28], instance2.resCom2_.img[33], teamData2.uniform_);
			g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
			g.DrawString(teamData.name_, 65f, num + 3, 2);
			g.DrawString(teamData2.name_, 177 + num4, num + 3, 2);
			g.DrawImage(instance2.resCom2_.img[58], 112f, 65f);
			int num6 = 0;
			if (instance3.gm_.homeAway_ == 1)
			{
				num6 = 1;
			}
			instance2.DrawSeb(g, 2, num - 10, instance2.resCom2_.seb[51], instance2.resCom2_.img[60], num6);
			instance2.DrawSeb(g, 215, num - 10, instance2.resCom2_.seb[51], instance2.resCom2_.img[60], 1 - num6);
			num = 93;
			for (int i = 0; i < 3; i++)
			{
				g.DrawImage(instance2.resCom2_.img[184], 0 + 80 * i, num);
			}
			int num7 = 0;
			num2 = 0;
			if (instance3.gm_.homeAway_ == 0)
			{
				num2 = 57;
				num7 = teamData.stadium_;
			}
			else
			{
				num2 = 183;
				num7 = teamData2.stadium_;
			}
			instance2.DrawSeb(g, num2, 147, instance2.resCom2_.seb[101 + num7], instance2.resCom2_.img[185 + num7], 0);
			int num8 = frame % 10 / 5;
			for (int j = 0; j < 3; j++)
			{
				if (num8 == 0)
				{
					instance3.SetDispPlayerData(0, teamData.bodyImg_, teamData.memberFaceId_[j], num8, 1);
				}
				else
				{
					instance3.SetDispPlayerData(4, teamData.bodyImg_, teamData.memberFaceId_[j], 0, 1);
				}
				instance3.DrawDispPlayer(g, 37 + j * 20, num + 63);
			}
			for (int k = 0; k < 3; k++)
			{
				if (num8 == 0)
				{
					instance3.SetDispPlayerData(0, teamData2.bodyImg_, teamData2.memberFaceId_[k], num8, 2);
				}
				else
				{
					instance3.SetDispPlayerData(4, teamData2.bodyImg_, teamData2.memberFaceId_[k], 0, 2);
				}
				instance3.DrawDispPlayer(g, 163 + k * 20, num + 63);
			}
			num5 = 4;
			num = 174;
			for (int l = 0; l < 2; l++)
			{
				num5 = 4;
				num = 174;
				if (l == 1)
				{
					num5 = 129;
				}
				TeamData teamData3 = null;
				switch (l)
				{
				case 0:
					teamData3 = teamData;
					break;
				case 1:
					teamData3 = teamData2;
					break;
				}
				g.SetColor(Graphics.GetColorOfRGB(0, 124, 27));
				g.FillRect(num5, num, 107f, 17f);
				g.SetColor(Graphics.GetColorOfRGB(0, 162, 35));
				g.FillRect(num5, num + 17, 107f, 17f);
				g.SetColor(Graphics.GetColorOfRGB(0, 124, 27));
				g.FillRect(num5, num + 34, 107f, 17f);
				g.SetColor(Graphics.GetColorOfRGB(3, 59, 63));
				g.DrawLine(num5 - 1, num, num5 - 1, num + 51);
				g.DrawLine(num5 + 107, num, num5 + 107, num + 51);
				g.DrawLine(num5 - 1, num + 51, num5 + 107, num + 51);
				if (matchInfoJoin == 0)
				{
					int[] leagueData = instance3.GetLeagueData(teamData3.id_);
					if (MyFormBase.Jp())
					{
						num4 = 0;
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾘｰｸﾞ", num5 + 19 + num4, num + 3);
						instance2.DrawNumImage(g, 30, leagueData[7], num5 + 77 + num4, num + 3, 0, 4);
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						g.DrawString("位", num5 + 79 + num4, num + 3);
					}
					else
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						g.DrawString("ﾘｰｸﾞ", num5 + 9, num + 3);
						int num9 = 77;
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						SetFontSize(g, 10);
						g.DrawString("位", num5 + num9 - 30 + 6, num + 3 + 2);
						ResetFontSize(g);
						instance2.DrawNumImage(g, 30, leagueData[7], num5 + num9 + 9 + 3, num + 3, 0, 2);
					}
				}
				else
				{
					int warPotential_ = teamData3.warPotential_;
					num4 = 0;
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
					SetFontSize(g, 10);
					if (MyFormBase.Jp())
					{
						g.DrawString("ﾁｰﾑ評価", num5 + 19 + num4, num + 3);
					}
					else
					{
						g.DrawString("ﾁｰﾑ評価", num5 + 19 + num4 - 6, num + 3 + 1);
					}
					ResetFontSize(g);
					if (MyFormBase.Jp())
					{
						instance2.DrawNumImage(g, 30, warPotential_, num5 + 77 + num4 + 8 - 8 + 2, num + 3, 0, 2);
					}
					else
					{
						instance2.DrawNumImage(g, 30, warPotential_, num5 + 77 + num4 + 8 - 8 + 2 + 1, num + 3, 0, 2);
					}
				}
				num4 = 0;
				g.DrawImage(instance2.resCom2_.img[57], num5 + 37 + num4, num + 17 + 4);
				int rankIndex = instance3.GetRankIndex(teamData3.warPotential_);
				instance2.DrawNumImage(g, 35, rankIndex + 1, num5 + 53 + num4 + 2, num + 17 + 4 + 1, 0, 1);
				num4 = 0;
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				g.DrawString(UserData.RANK_NAME[rankIndex], num5 + 53 + num4, num + 34 + 3, 2);
			}
		}

		public static void Draw_upParam(Graphics g, int seb, int dx, int dy, int cnt)
		{
			Draw_upParam(g, seb, dx, dy, cnt, 4);
		}

		public static void Draw_upParam(Graphics g, int seb, int dx, int dy, int cnt, int anchor)
		{
			AppData instance = AppData.GetInstance();
			int num = AppData.GetFig(cnt) * 8 + 8 + 2;
			if ((anchor == 1)) { if (cnt >= 0)
				{
					instance.DrawNumImage(g, seb, cnt, dx + 10, dy, 0, 1);
					instance.DrawSeb(g, dx, dy, instance.resCom2_.seb[35], instance.resCom2_.img[103], 14);
				}
				else
				{
					instance.DrawNumImage(g, 91, -cnt, dx + 10, dy, 0, 1);
					instance.DrawSeb(g, dx, dy, instance.resCom2_.seb[91], instance.resCom2_.img[109], 15);
				} } else if ((anchor == 2)) { if (cnt >= 0)
				{
					instance.DrawNumImage(g, seb, cnt, dx - num / 2 + 10, dy, 0, 1);
					instance.DrawSeb(g, dx - num / 2, dy, instance.resCom2_.seb[35], instance.resCom2_.img[103], 14);
				}
				else
				{
					instance.DrawNumImage(g, 91, -cnt, dx - num / 2 + 10, dy, 0, 1);
					instance.DrawSeb(g, dx - num / 2, dy, instance.resCom2_.seb[91], instance.resCom2_.img[109], 15);
				} } else if ((anchor == 4)) { if (cnt >= 0)
				{
					instance.DrawNumImage(g, seb, cnt, dx, dy, 0, 4);
					instance.DrawSeb(g, dx - num, dy, instance.resCom2_.seb[35], instance.resCom2_.img[103], 14);
				}
				else
				{
					instance.DrawNumImage(g, 91, -cnt, dx, dy, 0, 4);
					instance.DrawSeb(g, dx - num, dy, instance.resCom2_.seb[91], instance.resCom2_.img[109], 15);
				} }
		}

		public static void Draw_multiParam(Graphics g, int seb, int dx, int dy, int cnt)
		{
			Draw_multiParam(g, seb, dx, dy, cnt, 4);
		}

		public static void Draw_multiParam(Graphics g, int seb, int dx, int dy, int cnt, int anchor)
		{
			AppData instance = AppData.GetInstance();
			int num = AppData.GetFig(cnt) * 8 + 8 + 5;
			if ((anchor == 1)) { instance.DrawNumImageComma(g, seb, cnt, dx + 8 + 5, dy, 0, 0, 1);
instance.DrawSeb(g, dx, dy, instance.resCom2_.seb[35], instance.resCom2_.img[103], 16); } else if ((anchor == 2)) { instance.DrawNumImageComma(g, seb, cnt, dx - num / 2 + 8 + 5, dy, 0, 0, 1);
instance.DrawSeb(g, dx - num / 2, dy, instance.resCom2_.seb[35], instance.resCom2_.img[103], 16); } else if ((anchor == 4)) { instance.DrawNumImageComma(g, seb, cnt, dx - num + 8 + 5, dy, 0, 0, 1);
instance.DrawSeb(g, dx - num, dy, instance.resCom2_.seb[35], instance.resCom2_.img[103], 16); }
		}

		public static void Draw_up(Graphics g, int dx, int dy, int cnt, int padding)
		{
if (cnt == 0) {
return;
}
AppData instance = AppData.GetInstance();
if (cnt >= 0) {
int i = 0;
while (i < cnt) {
instance.DrawSeb(g, dx + i * (8 + padding), dy, instance.resCom2_.seb[35], instance.resCom2_.img[103], 14);
i++;
}
} else {
cnt = -cnt;
int j = 0;
while (j < cnt) {
instance.DrawSeb(g, dx + j * (8 + padding), dy, instance.resCom2_.seb[35], instance.resCom2_.img[103], 15);
j++;
}
}
return;
}

		private int[] DrawWindow2(Graphics g, int width, int height, string title, int sideSpace, int topSpace, int bottomSpace, int barArwSpace, int flag)
		{
			return DrawWindow(g, width, height, title, sideSpace, topSpace, bottomSpace, barArwSpace, flag, 0, -1);
		}

		private int[] DrawWindow(Graphics g, int width, int height, string title, int sideSpace, int topSpace, int bottomSpace, int barArwSpace, int flag, int page, int pageMax)
		{
			if (drawWindow_tmp == null)
			{
				drawWindow_tmp = new int[8];
			}
			int num;
if (pageMax != -1)
			{
				string text = title;
				title = text + " " + (page + 1) + "/" + pageMax;
			}
			if (pageMax > 1)
			{
				flag |= 3;
			}
			apdat_.DrawWindow(g, width, height, 0, title);
			if (apdat_.IsTopForm(this))
			{
			}
			drawWindow_tmp[0] = 120 - height / 2 + 17;
			drawWindow_tmp[3] = 120 - width / 2;
			drawWindow_tmp[1] = drawWindow_tmp[3] + width;
			drawWindow_tmp[2] = drawWindow_tmp[0] + height - 17;
			drawWindow_tmp[7] = drawWindow_tmp[3] + sideSpace;
			drawWindow_tmp[5] = drawWindow_tmp[1] - sideSpace;
			drawWindow_tmp[4] = drawWindow_tmp[0] + topSpace;
			drawWindow_tmp[6] = drawWindow_tmp[2] - bottomSpace;
			if (BitUtil.Check(flag, 4))
			{
				apdat_.DrawBox(g, drawWindow_tmp[7], drawWindow_tmp[4], drawWindow_tmp[5], drawWindow_tmp[6]);
			}
			num = GameForm.VIEW_Y / 2;
			drawWindow_tmp[0] += num;
			drawWindow_tmp[2] += num;
			drawWindow_tmp[4] += num;
			drawWindow_tmp[6] += num;
			return drawWindow_tmp;
		}

		public void SetWinCharaJamp(int time)
		{
			SetWinCharaJamp(time, 6, -10, 14);
		}

		public void SetWinCharaJamp(int time, int wait, int JAMP_DIS, int JAMP_TIME)
		{
			int num = time % (JAMP_TIME + wait);
			num -= wait / 2;
			float num2 = GameUtil.SetVStop(JAMP_DIS, JAMP_TIME / 2);
			float num3 = GameUtil.SetAStop(JAMP_DIS, JAMP_TIME / 2);
			WIN_CHA[1] = (int)(num2 * (float)num + num3 * (float)num * (float)(num + 1) / 2f);
			if (WIN_CHA[1] > 0)
			{
				WIN_CHA[1] = 0;
			}
			if (WIN_CHA[1] < 0)
			{
				WIN_CHA[0] = 3;
			}
			else
			{
				WIN_CHA[0] = 0;
			}
		}

		public void SetWinCharaTraining(int time, int tMode)
		{
			if (tMode == 2)
			{
				int num = 6;
				int num2 = time % (14 + num);
				num2 -= num / 2;
				float num3 = GameUtil.SetVStop(-10f, 7);
				float num4 = GameUtil.SetAStop(-10f, 7);
				WIN_CHA[1] = (int)(num3 * (float)num2 + num4 * (float)num2 * (float)(num2 + 1) / 2f);
				if (WIN_CHA[1] > 0)
				{
					WIN_CHA[1] = 0;
				}
				if (WIN_CHA[1] < 0)
				{
					WIN_CHA[0] = 2;
				}
				else
				{
					WIN_CHA[0] = 0;
				}
				WIN_CHA[2] = 0;
				if (WIN_CHA[0] == 2 && num2 >= (14 + num) / 2 - 3)
				{
					WIN_CHA[2] = 1;
				}
				} else {
			WIN_CHA[1] = 0;
			int num5 = 0;
			if (tMode == 0) {WIN_CHA[0] = 1;
				num5 = time % Player.SEB_H_KICK_ANIME[Player.SEB_H_KICK_ANIME.Length - 1];
				if (num5 < Player.SEB_H_KICK_ANIME[0])
				{
					WIN_CHA[2] = 0;
				}
				else if (num5 < Player.SEB_H_KICK_ANIME[1])
				{
					WIN_CHA[2] = 1;
				}
				else if (num5 < Player.SEB_H_KICK_ANIME[2])
				{
					WIN_CHA[2] = 2;
				}
				} else if (tMode == 1) {WIN_CHA[0] = 10;
				num5 = time % Player.SEB_H_HEAD_ANIME[Player.SEB_H_HEAD_ANIME.Length - 1];
				WIN_CHA[2] = time % (Player.SEB_H_WALK_ASPEED * 4) / Player.SEB_H_WALK_ASPEED;
				} else if (tMode == 3) {WIN_CHA[0] = 4;
				num5 = time % Player.SEB_H_THROWIN_ANIME[Player.SEB_H_THROWIN_ANIME.Length - 1];
				WIN_CHA[2] = time % 12 / 6;
				}
		}}

		public void SetMeterParamRank(int line, int TIME, int value, int valueMax, int rank, int rankMax, int add)
		{
			meterParam[line][0] = TIME;
			meterParam[line][1] = value;
			meterParam[line][2] = valueMax;
			meterParam[line][3] = rank;
			meterParam[line][4] = rankMax;
			meterParam[line][6] = add;
			meterParam[line][5] = 1;
			meterParam[line][7] = meterParam[line][1] + meterParam[line][6];
			meterParam[line][8] = meterParam[line][3];
			if (meterParam[line][7] >= meterParam[line][2] && meterParam[line][3] < meterParam[line][4] && meterParam[line][5] == 1)
			{
				meterParam[line][7] -= meterParam[line][2];
				meterParam[line][8]++;
			}
		}

		public void SetMeterParam(int line, int TIME, int value, int valueMax, int add)
		{
			meterParam[line][0] = TIME;
			meterParam[line][1] = value;
			meterParam[line][2] = valueMax;
			meterParam[line][6] = add;
			meterParam[line][5] = 0;
			meterParam[line][7] = AppData.ClampMax(meterParam[line][1] + meterParam[line][6], meterParam[line][2]);
		}

		public void SetMeterParamDispAdd(int line, int faId, int nowValue)
		{
			int num = 0;
			int num2 = 0;
			num = nowValue;
			if (faId == 3)
			{
				num2 = meterParam[line][1] + meterParam[line][6];
			}
			else
			{
				num2 = meterParam[line][1] + meterParam[line][6];
				num2 = FanActData.GetFanDispParam(num2, faId);
			}
			meterParam[line][9] = num2 - num;
		}

		public void UpdateMeterParam(int frame, int line)
		{
			int num = GameUtil.RateConvert(frame - 1, 0, meterParam[line][0], 0, meterParam[line][6], true);
			int num2 = GameUtil.RateConvert(frame, 0, meterParam[line][0], 0, meterParam[line][6], true) - num;
			meterParam[line][1] += num2;
			if (meterParam[line][5] == 1)
			{
				if (meterParam[line][1] >= meterParam[line][2] && meterParam[line][3] < meterParam[line][4])
				{
					meterParam[line][1] -= meterParam[line][2];
					meterParam[line][3]++;
				}
			}
			else if (meterParam[line][5] == 0)
			{
				meterParam[line][1] = AppData.ClampMax(meterParam[line][1], meterParam[line][2]);
			}
		}

		public void FinishMeterParam(int line)
		{
			meterParam[line][1] = meterParam[line][7];
			if (meterParam[line][5] == 1)
			{
				meterParam[line][3] = meterParam[line][8];
			}
		}

		public int[] GetMeterParam(int line)
		{
			return meterParam[line];
		}

		public static void Draw_chara(Graphics g, int charaImg, int charaSeb, int direct, int cnt, int dx, int dy)
		{
			int frame = 0;
			if ((charaSeb == 0)) {
frame = 0 + direct;

} else if ((charaSeb == 1)) {
frame = 2 + direct;

} else if ((charaSeb == 2)) {
				int num = cnt % 12 / 6;
				frame = num * 2 + 4 + direct;

			}
			AppData instance = AppData.GetInstance();
			instance.DrawSeb(g, dx, dy, instance.resCom2_.seb[96], instance.resCom2_.img[charaImg], frame);
		}

		public void Draw_categoryIcon(Graphics g, int iconId, int dx, int dy)
		{
			g.DrawImage(apdat_.resCom2_.img[116], dx, dy, iconId % 9 * 14, iconId / 9 * 14, 14, 14);
		}

		public void Draw_tenantBuildIcon(Graphics g, int iconId, int dx, int dy)
		{
			g.DrawImage(apdat_.resCom2_.img[98], dx, dy, iconId % 10 * 16, iconId / 10 * 16, 16, 16);
		}

		public void Draw_trainingIcon(Graphics g, int iconId, int dx, int dy)
		{
			g.DrawImage(apdat_.resCom2_.img[173], dx, dy, iconId % 10 * 16, iconId / 10 * 16, 16, 16);
		}

		public void Draw_fanActBack(Graphics g, int backId, int dx, int dy)
		{
			g.DrawImage(apdat_.resCom2_.img[174], dx, dy, backId % 3 * 40, backId / 3 * 55, 40, 55);
		}

		public void SetTicketWindowBtmMsg(string msg)
		{
			ticketPriceMessage = msg;
		}

		public static void Draw_newMark(Graphics g, int dx, int dy, int frame_)
		{
			AppData instance = AppData.GetInstance();
			instance.DrawSeb(g, dx, dy, instance.resCom2_.seb[98], instance.resCom2_.img[181], frame_ % 12 / 6);
		}

		public override bool CheckMainWindow()
		{
			return false;
		}

		public void Update_clearPoint()
		{
			frame_ = (frame_ + 1) % int.MaxValue;
			if (clearPoint_cnt < CLEARPOINT_ANIMETIME[clearPoint_mode])
			{
				clearPoint_cnt++;
				if (Config.DEBUG_CMD && canvas_.CheckKeyState(64))
				{
					clearPoint_cnt = CLEARPOINT_ANIMETIME[clearPoint_mode];
				}
				if (clearPoint_cnt == CLEARPOINT_ANIMETIME[clearPoint_mode])
				{
					if (clearPoint_mode == 3)
					{
					}
					if (clearPoint_mode == 3)
					{
						clearPoint_score += udat_.clear_point[clearPoint_index][1];
					}
				}
			}
			if (clearPoint_cnt >= CLEARPOINT_ANIMETIME[clearPoint_mode]) {

			if (clearPoint_mode == 0)
			{
				if (canvas_.CheckKeyPulse(1048576))
				{
					clearPoint_cnt = 0;
					clearPoint_mode = 1;
				}
			}
			else if (clearPoint_mode == 1)
			{
				clearPoint_cnt = 0;
				clearPoint_mode = 2;
			}
			else if (clearPoint_mode == 2)
			{
				clearPoint_cnt = 0;
				clearPoint_mode = 3;
			}
			else if (clearPoint_mode == 3)
			{
				if (canvas_.CheckKeyPulse(1048576) || (Config.DEBUG_CMD && canvas_.CheckKeyState(64)))
				{
					clearPoint_cnt = 0;
					clearPoint_index++;
					clearPoint_mode = 5;
					if (clearPoint_index < 7)
					{
						clearPoint_mode = 5;
					}
					else if (clearPoint_score > clearPoint_highScore)
					{
						clearPoint_mode = 4;
					}
					else
					{
						clearPoint_mode = 6;
					}
					Log.Info("■SubForm.java update_clearPoint() clearPoint_index = " + clearPoint_index + ", clearPoint_mode = " + clearPoint_mode);
				}
			}
			else if (clearPoint_mode == 4)
			{
				clearPoint_cnt = 0;
				clearPoint_mode = 6;
			}
			else if (clearPoint_mode == 5)
			{
				clearPoint_cnt = 0;
				clearPoint_mode = 1;
			}
			else
			{
				if (clearPoint_mode == 6) {

				formMan_.Pop();
				apdat_.PlayBgm(0);
				apdat_.ExecEvent(6, null, null);
				if (clearPoint_score > clearPoint_highScore)
				{
					if (clearPoint_score >= 100000)
					{
						apdat_.sysSave_.ints[10] = 4;
					}
					else if (clearPoint_score >= 75000)
					{
						apdat_.sysSave_.ints[10] = 3;
					}
					else if (clearPoint_score >= 50000)
					{
						apdat_.sysSave_.ints[10] = 2;
					}
					else
					{
						apdat_.sysSave_.ints[10] = 1;
					}
					apdat_.ExecEvent(4, AppData.CommaSeparate(clearPoint_score) + "\t" + MyFormBase.LT(TROPHY_NAME[apdat_.sysSave_.ints[10]]), null);
					apdat_.sysSave_.lngs[0] = clearPoint_score;
					apdat_.sysSave_.strs[0] = apdat_.save_.strs[0];
				}
				else
				{
					apdat_.ExecEvent(5, AppData.CommaSeparate(clearPoint_score), null);
				}
				clearPoint_cnt = 0;
				clearPoint_mode = 7;
				try
				{
					apdat_.SaveSystem();
				}
				catch (Exception t)
				{
					Log.Info(t);
				}
}
			}
}
		}

		public void Draw_clearPoint(Graphics g)
		{
			Font font = g.GetFont(null);
			apdat_.DrawWindow(g, 222, 155, 0, "ｸﾘｱﾎﾟｲﾝﾄ発表");
			apdat_.DrawBox(g, 17, 81, 224, 177);
			int num2;
int num3;
int num = 105;
			num2 = 30;
			num = 105;
			g.DrawImage(apdat_.resEvent_.img[11], num2, num);
			apdat_.DrawBox(g, 21, 85, 218, 137);
			num = 181;
			if (clearPoint_mode == 4)
			{
				num3 = 0;
				if (frame_ % 6 * 2 < 6)
				{
					num3 = 1;
				}
				Draw_chara(g, 171, 2, 1, frame_, 190, num);
				Draw_chara(g, 172, 2, 0, frame_, 51, num);
			}
			else
			{
				Draw_chara(g, 171, 0, 1, frame_, 190, num);
				Draw_chara(g, 172, 0, 0, frame_, 51, num);
			}
			if (clearPoint_mode == 0)
			{
				if (clearPoint_cnt >= 15)
				{
					num = 105;
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString("それではさっそく", 120f, num, 2);
					if (clearPoint_cnt >= (CLEARPOINT_ANIMETIME[clearPoint_mode] + 15) / 2)
					{
						num += 22;
						g.DrawString("ｸﾘｱﾎﾟｲﾝﾄの計算に入ります。", 120f, num, 2);
					}
				}
				if (clearPoint_cnt == CLEARPOINT_ANIMETIME[clearPoint_mode] && frame_ % 40 > 20)
				{
					apdat_.DrawSeb(g, 213, 179, apdat_.resCom2_.seb[63], apdat_.resCom2_.img[72], 0);
				}
			}
			else if (clearPoint_mode == 1)
			{
				if (clearPoint_cnt >= 15)
				{
					num = 105;
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(UserData.CPOINT_STR[clearPoint_index][0], 120f, num, 2);
					if (clearPoint_cnt >= (CLEARPOINT_ANIMETIME[clearPoint_mode] + 15) / 2)
					{
						num += 22;
						g.DrawString(MyFormBase.LT("<0><1>", AppData.CommaSeparate(udat_.clear_point[clearPoint_index][0]) + "\t" + MyFormBase.LT(UserData.CPOINT_STR[clearPoint_index][1])), 120f, num, 2);
					}
				}
			}
			else if (clearPoint_mode == 2)
			{
				if (clearPoint_cnt >= 15)
				{
					num = 117;
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString("ｸﾘｱﾎﾟｲﾝﾄに…", 120f, num, 2);
				}
			}
			else if (clearPoint_mode == 3)
			{
				if (clearPoint_cnt >= 15)
				{
					string empty = string.Empty;
					empty = AppData.CommaSeparate(udat_.clear_point[clearPoint_index][1]);
					string text = "点追加!!";
					num2 = 120 - (font.StringWidth(empty) + font.StringWidth(text)) / 2;
					num = 117;
					g.SetColor(SC_WINDOW_BLUE);
					g.DrawString(empty, num2, num);
					num2 += font.StringWidth(empty);
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString(text, num2, num);
				}
				if (clearPoint_cnt == CLEARPOINT_ANIMETIME[clearPoint_mode] && frame_ % 40 > 20)
				{
					apdat_.DrawSeb(g, 213, 179, apdat_.resCom2_.seb[63], apdat_.resCom2_.img[72], 0);
				}
			}
			else if (clearPoint_mode == 4)
			{
				num = 117;
				g.SetColor(SC_WINDOW_BLUE);
				g.DrawString("ハイスコア更新！", 120f, num, 2);
			}
			else if (clearPoint_mode == 5)
			{
				if (clearPoint_cnt >= 15)
				{
					num = 117;
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString("つづきまして…", 120f, num, 2);
				}
			}
			else if (clearPoint_mode == 6)
			{
				if (clearPoint_cnt >= 15)
				{
					num = 105;
					g.SetColor(SC_WINDOW_BLACK);
					g.DrawString("ｸﾘｱﾎﾟｲﾝﾄの計算は", 120f, num, 2);
					if (clearPoint_cnt >= (CLEARPOINT_ANIMETIME[clearPoint_mode] + 15) / 2)
					{
						num += 22;
						g.DrawString("以上となります。", 120f, num, 2);
					}
				}
				if (clearPoint_cnt == CLEARPOINT_ANIMETIME[clearPoint_mode] && frame_ % 40 > 20)
				{
					apdat_.DrawSeb(g, 213, 167, apdat_.resCom2_.seb[63], apdat_.resCom2_.img[72], 0);
				}
			}
			num = 78;
			g.SetColor(SC_WINDOW_BLACK);
			if (!En())
			{
				g.DrawString("現在ﾎﾟｲﾝﾄ:", 93f, num, 4);
			}
			else
			{
				g.DrawString("現在ﾎﾟｲﾝﾄ:", 33f, num, 1);
			}
			g.DrawString(MyFormBase.LT("<0>点", AppData.CommaSeparate(clearPoint_score)), 213f, num, 4);
			num = 192;
			g.SetColor(SC_WINDOW_BLUE);
			g.DrawString("ﾊｲｽｺｱ  :", 93f, num, 4);
			g.DrawString(MyFormBase.LT("<0>点", AppData.CommaSeparate(clearPoint_highScore)), 213f, num, 4);
		}

		public override int GetTouchValue(int type, int index)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 131072:
				return select_;
			case 1310720:
				return page_;
			case 262144:
				if (type_ == 26)
				{
					return udat_.scrollValue_;
				}
				return scrollValue_;
			case 786432:
				return udat_.et_cur[0];
			case 851968:
				return udat_.et_cur[1];
			case 917504:
				return udat_.et_cur[2];
			case 983040:
				return udat_.et_cur[3];
			case 1048576:
				return udat_.et_cur[4];
			case 1114112:
				return udat_.et_cur[5];
			case 1179648:
				return udat_.et_cur[6];
			default:
				return base.GetTouchValue(type, index);
			}
		}

		public override void SetTouchValue(int type, int value)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 131072:
				select_ = value;
				break;
			case 1310720:
				page_ = value;
				break;
			case 262144:
				scrollValue_ = value;
				udat_.scrollValue_ = value;
				break;
			case 1376256:
				udArrowCnt = value;
				break;
			default:
				base.SetTouchValue(type, value);
				break;
			}
		}

		public override bool OnTouchEvent(TouchEvent e)
		{
			TouchComponent component = e.component;
			return true;
		}

		public static void Draw_backBlackAlpha(Graphics g)
		{
			GameView instance = GameView.GetInstance();
			int originX = g.GetOriginX();
			int originY = g.GetOriginY();
			g.SetOrigin(0, 0);
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, 180));
			g.FillRect(0f, 0f, instance.GetGameWidth(), instance.GetGameHeight());
			g.SetOrigin(originX, originY);
		}

		public void Draw_udArrow(Graphics g)
		{
		}

		private void DrawConfig_jp(Graphics g)
		{
			int[] array = DrawWindow(g, 210, 186, "設定", 4, 4, 39, 80, 4);
			int num = array[7] + 10;
			int num2 = array[4] + 10;
			int num3 = 24;
			int num4 = num + 65;
			int num5 = array[5] - 15;
			int num6 = 0;
			while (num6 < CONFIG_NAMES.Length)
			{
				int num7 = 36;
				if (num6 == CONFIG_BGM || num6 == CONFIG_SE)
				{
					num7 = 54;
				}
				else if (num6 == CONFIG_ORIENTATION)
				{
					num7 = 48;
				}
				if (select_ == num6)
				{
					g.SetColor(159, 189, 255);
					if (MyFormBase.Jp())
					{
						g.FillRect(num - 1, num2 - 1, num7 + 2, 14f);
					}
					else
					{
						g.FillRect(num - 1, num2 - 1, 56f, 14f);
					}
				}
				int num8 = num + 30;
				GameView.AddTouch(g, 6, -120, num2 - 6, 120 + num8, num3, 0x20000 | num6);
				g.SetColor(30, 30, 30);
				g.DrawString(CONFIG_NAMES[num6], num, num2);
				int num9 = GetTouchValue(CONFIG_VALUES[num6], 0);
				if (CONFIG_VALUE_REVERSE[num6])
				{
					num9 = (num9 + 1) % 2;
				}
				if (num6 == CONFIG_BGM || num6 == CONFIG_SE)
				{
					g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
					g.DrawString(num9, num4 - 12, num2, 4);
					GameView.AddTouch(g, 21, num8, num2 - 6, num4 + 20 - num8, num3, 0x20000 | num6, TouchOption.Create().ExtendValues(65552));
					GameView.AddTouch(g, 20, num5 - 20, num2 - 6, 240, num3, 0x20000 | num6, TouchOption.Create().ExtendValues(65554));
					GameView.AddTouch(g, 19, num4 + 25, num2 - 6, num5 - num4 - 50, num3, CONFIG_VALUES[num6], TouchOption.Drag(num4 + 25, 0, num5 - num4 - 50, 0, 6, 0).Flag(4).ExtendValues(0x20000 | num6));
				}
				else
				{
					for (int i = 0; i < CONFIG_VALUE_NAMES[num6].Length; i++)
					{
						int[] evenlyDivisionArea = apdat_.GetEvenlyDivisionArea(num4, num5, 2, i, 2);
						bool flag = false;
						if (num6 == CONFIG_ORIENTATION)
						{
							if (i == 0 && num9 == -1)
							{
								flag = true;
							}
							if (i == 1 && num9 != -1)
							{
								flag = true;
							}
						}
						else if (num9 == i)
						{
							flag = true;
						}
						if (flag)
						{
							g.SetColor(159, 189, 255);
							g.FillRect(evenlyDivisionArea[0] + (evenlyDivisionArea[1] - evenlyDivisionArea[0]) / 2 - 26 - 1, num2 - 1, 54f, 14f);
						}
						g.SetColor(30, 30, 30);
						if (num6 == CONFIG_SPEED && i == 0 && apdat_.sysSave_.ints[9] == 0)
						{
							g.SetColor(128, 128, 128);
						}
						if (Config.ADMOB_ENABLE && num6 == CONFIG_ORIENTATION && i == 0)
						{
							g.SetColor(128, 128, 128);
						}
						g.DrawString(CONFIG_VALUE_NAMES[num6][i], evenlyDivisionArea[0] + (evenlyDivisionArea[1] - evenlyDivisionArea[0]) / 2, num2, 2);
						if (num6 == CONFIG_SPEED && i == 0 && apdat_.sysSave_.ints[9] == 0)
						{
							continue;
						}
						int num10;
						if (num6 == CONFIG_ORIENTATION)
						{
							if (i == 0) { num10 = (-1); } else { num10 = app_.GetOrientation(); }
						}
						else
						{
							num10 = i;
							if (CONFIG_VALUE_REVERSE[num6])
							{
								num10 = (num10 + 1) % 2;
							}
						}
						if (i == 0)
						{
							GameView.AddTouch(g, 6, num8, num2 - 6, evenlyDivisionArea[1] + 2 - num8, num3, Tv(CONFIG_VALUES[num6], num10), TouchOption.Create().ExtendValues(0x20000 | num6));
						}
						else
						{
							GameView.AddTouch(g, 6, evenlyDivisionArea[0] - 2, num2 - 6, 240, num3, Tv(CONFIG_VALUES[num6], num10), TouchOption.Create().ExtendValues(0x20000 | num6));
						}
					}
				}
				num6++;
				num2 += num3;
			}
			num2 = GetBoxFooterY(array[6]);
			if (select_ == CONFIG_QUALITY)
			{
				g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
				g.DrawString("速度優先にすると一部の演出や", array[7] + 15, num2, 17);
				g.DrawString("表示が削除されます", array[7] + 15, num2 + 18, 17);
			}
			else if (select_ == CONFIG_SPEED)
			{
				g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
				g.DrawString("ゲームの動作速度を変更します", array[7] + 15, num2, 17);
				g.SetColor(Graphics.GetColorOfRGB(200, 0, 0));
				g.DrawString("ｹﾞｰﾑをｸﾘｱすると変更できます", array[7] + 15, num2 + 18);
			}
			else if (select_ == CONFIG_BGM || select_ == CONFIG_SE)
			{
				g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
				g.DrawString("ゲーム中のボリュームを", array[7] + 15, num2, 17);
				g.DrawString("設定します", array[7] + 15, num2 + 18, 17);
			}
			else if (select_ == CONFIG_ORIENTATION)
			{
				if (Config.ADMOB_ENABLE)
				{
					g.SetColor(Graphics.GetColorOfRGB(200, 0, 0));
					g.DrawString("横画面モードは", array[7] + 15, num2, 17);
					g.DrawString("正式版でご利用頂けます!!", array[7] + 15, num2 + 18, 17);
				}
				else
				{
					g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
					g.DrawString("本体の向きに合わせて", array[7] + 15, num2, 17);
					g.DrawString("画面を回転するか設定します", array[7] + 15, num2 + 18, 17);
				}
			}
		}

		private void DrawConfig_en(Graphics g)
		{
			int[] array = DrawWindow(g, 210, 210, MyFormBase.LT("設定"), 4, 4, 39, 80, 4);
			int num = array[7] + 10;
			int num2 = array[4] + 10;
			int num3 = 24;
			int num4 = num + 65;
			int num5 = array[5] - 15;
			int num6 = 0;
			while (num6 < 6)
			{
				int num7 = 36;
				if (num6 == CONFIG_EN_BGM || num6 == CONFIG_EN_SE)
				{
					num7 = 54;
				}
				else if (num6 == CONFIG_EN_CURRENCY || num6 == CONFIG_EN_ORIENTATION)
				{
					num7 = 48;
				}
				if (select_ == num6)
				{
					g.SetColor(159, 189, 255);
					if (MyFormBase.Jp())
					{
						g.FillRect(num - 1, num2 - 1, num7 + 2, 14f);
					}
					else
					{
						g.FillRect(num - 1, num2 - 1, 56f, 14f);
					}
				}
				int num8 = num + 30;
				GameView.AddTouch(g, 6, -120, num2 - 6, 120 + num8, num3, 0x20000 | num6);
				g.SetColor(30, 30, 30);
				g.DrawString(CONFIG_EN_NAMES[num6], num, num2);
				int num9 = GetTouchValue(CONFIG_EN_VALUES[num6], 0);
				if (CONFIG_EN_VALUE_REVERSE[num6])
				{
					num9 = (num9 + 1) % 2;
				}
				if (num6 == CONFIG_EN_BGM || num6 == CONFIG_EN_SE)
				{
					g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
					g.DrawString(num9, num4 - 12, num2, 4);
					GameView.AddTouch(g, 21, num8, num2 - 6, num4 + 20 - num8, num3, 0x20000 | num6, TouchOption.Create().ExtendValues(65552));
					GameView.AddTouch(g, 20, num5 - 20, num2 - 6, 240, num3, 0x20000 | num6, TouchOption.Create().ExtendValues(65554));
					GameView.AddTouch(g, 19, num4 + 25, num2 - 6, num5 - num4 - 50, num3, CONFIG_VALUES[num6], TouchOption.Drag(num4 + 25, 0, num5 - num4 - 50, 0, 6, 0).Flag(4).ExtendValues(0x20000 | num6));
				}
				else
				{
					for (int i = 0; i < CONFIG_EN_VALUE_NAMES[num6].Length; i++)
					{
						int[] evenlyDivisionArea = apdat_.GetEvenlyDivisionArea(num4, num5, 2, i, 2);
						bool flag = false;
						if (num6 == CONFIG_EN_ORIENTATION)
						{
							if (i == 0 && num9 == -1)
							{
								flag = true;
							}
							if (i == 1 && num9 != -1)
							{
								flag = true;
							}
						}
						else if (num9 == i)
						{
							flag = true;
						}
						if (flag)
						{
							g.SetColor(159, 189, 255);
							g.FillRect(evenlyDivisionArea[0] + (evenlyDivisionArea[1] - evenlyDivisionArea[0]) / 2 - 26 - 1, num2 - 1, 54f, 14f);
						}
						g.SetColor(30, 30, 30);
						if (num6 == CONFIG_EN_SPEED && i == 0 && apdat_.sysSave_.ints[9] == 0)
						{
							g.SetColor(128, 128, 128);
						}
						if (Config.ADMOB_ENABLE && num6 == CONFIG_ORIENTATION && i == 0)
						{
							g.SetColor(128, 128, 128);
						}
						g.DrawString(CONFIG_EN_VALUE_NAMES[num6][i], evenlyDivisionArea[0] + (evenlyDivisionArea[1] - evenlyDivisionArea[0]) / 2, num2, 2);
						if (num6 == CONFIG_EN_SPEED && i == 0 && apdat_.sysSave_.ints[9] == 0)
						{
							continue;
						}
						int num10;
						if (num6 == CONFIG_EN_ORIENTATION)
						{
							if (i == 0) { num10 = (-1); } else { num10 = app_.GetOrientation(); }
						}
						else
						{
							num10 = i;
							if (CONFIG_EN_VALUE_REVERSE[num6])
							{
								num10 = (num10 + 1) % 2;
							}
						}
						if (i == 0)
						{
							GameView.AddTouch(g, 6, num8, num2 - 6, evenlyDivisionArea[1] + 2 - num8, num3, Tv(CONFIG_EN_VALUES[num6], num10), TouchOption.Create().ExtendValues(0x20000 | num6));
						}
						else
						{
							GameView.AddTouch(g, 6, evenlyDivisionArea[0] - 2, num2 - 6, 240, num3, Tv(CONFIG_EN_VALUES[num6], num10), TouchOption.Create().ExtendValues(0x20000 | num6));
						}
					}
				}
				num6++;
				num2 += num3;
			}
			num2 = GetBoxFooterY(array[6]);
			if (select_ == CONFIG_EN_QUALITY)
			{
				g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
				g.DrawString("速度優先にすると一部の演出や", array[7] + 15, num2, 17);
				g.DrawString("表示が削除されます", array[7] + 15, num2 + 18, 17);
			}
			else if (select_ == CONFIG_EN_SPEED)
			{
				g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
				g.DrawString("ゲームの動作速度を変更します", array[7] + 15, num2, 17);
				g.SetColor(Graphics.GetColorOfRGB(200, 0, 0));
				g.DrawString("ｹﾞｰﾑをｸﾘｱすると変更できます", array[7] + 15, num2 + 18);
			}
			else if (select_ == CONFIG_EN_BGM || select_ == CONFIG_EN_SE)
			{
				g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
				g.DrawString("ゲーム中のボリュームを", array[7] + 15, num2, 17);
				g.DrawString("設定します", array[7] + 15, num2 + 18, 17);
			}
			else if (select_ == CONFIG_EN_CURRENCY)
			{
				g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
				g.DrawString("お金の表記を変更できます", array[7] + 15, num2, 17);
				g.DrawString("\\100 = $1というレートです", array[7] + 15, num2 + 18, 17);
			}
			else if (select_ == CONFIG_EN_ORIENTATION)
			{
				if (Config.ADMOB_ENABLE)
				{
					g.SetColor(Graphics.GetColorOfRGB(200, 0, 0));
					g.DrawString("Landscape mode available", array[7] + 15, num2, 17);
					g.DrawString("in the Full Version!", array[7] + 15, num2 + 18, 17);
				}
				else
				{
					g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
					g.DrawString("本体の向きに合わせて", array[7] + 15, num2, 17);
					g.DrawString("画面を回転するか設定します", array[7] + 15, num2 + 18, 17);
				}
			}
		}

		public static void SetFontSize(Graphics g, int size)
		{
			if (!(En())) { return; }
{
Font font = g.GetFont(null);
fontSize = font.GetSize();
font.SetSize(size);
g.SetFont(font);
}
		}

		public static void ResetFontSize(Graphics g)
		{
			if (!(En())) { return; }
{
Font font = g.GetFont(null);
font.SetSize(fontSize);
g.SetFont(font);
}
		}

		internal new static bool En()
		{
			return Language.English();
		}

		public static void Draw_btmMsg(Graphics g, string str, int dy)
		{
			Draw_btmMsg(g, str, dy, true, true);
		}

		public static void Draw_btmMsg(Graphics g, string str, int dy, bool flgSelect, bool flgCursor)
		{
			int num = g.GetFont(null).StringWidth(str);
			AppData instance = AppData.GetInstance();
			if (flgSelect)
			{
				g.SetColor(SC_WINDOW_SELECT);
				g.FillRect(120 - (num + 16) / 2, dy, num + 16, 16f);
			}
			g.SetColor(SC_WINDOW_BLACK);
			g.DrawString(str, 120f, dy + 2, 2);
			if (flgCursor)
			{
				instance.resCom2_.seb[60].Draw(g, 120 - (num + 16) / 2 - 4, dy + 9, instance.resCom2_.img);
			}
		}

		public static void Draw_btmMsg(Graphics g, string str, int dx, int dy)
		{
			Draw_btmMsg(g, str, dx, dy, true, true);
		}

		public static void Draw_btmMsg(Graphics g, string str, int dx, int dy, bool flgSelect, bool flgCursor)
		{
			int num = g.GetFont(null).StringWidth(str);
			AppData instance = AppData.GetInstance();
			if (flgSelect)
			{
				g.SetColor(SC_WINDOW_SELECT);
				g.FillRect(dx - (num + 16) / 2, dy, num + 16, 16f);
			}
			g.SetColor(SC_WINDOW_BLACK);
			g.DrawString(str, dx, dy + 2, 2);
			if (flgCursor)
			{
				instance.resCom2_.seb[60].Draw(g, dx - (num + 16) / 2 - 4, dy + 9, instance.resCom2_.img);
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static int GetStringLength(string s)
		{
			int num = 0;
			for (int i = 0; i < s.Length(); i++)
			{
				if (IsSimpleChara(s.CharAt(i))) { num = (num + 1); } else { num = (num + 2); }
			}
			return num;
		}

		private static bool IsSimpleChara(char c)
		{
			return (c < '\u007f' && c >= ' ') || (c > '｠' && c <= 'ﾟ');
		}
	}
}
