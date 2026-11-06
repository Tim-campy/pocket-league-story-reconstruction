using System;
using data;
using form;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.m3d4.util3d;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;

namespace game
{
	public class UserData
	{
		public const int EVA_MAX = 1000;

		public const int EVA_MAX_FAN = 10000;

		public const int EVA_MAX_ASSIST = 1000;

		public const int EVA_MAX_NAMEVALUE = 10000;

		public const int DISP_MIN_FAN = 0;

		public const int DISP_MAX_FAN = 100000;

		public const int DISP_MAX_ASSIST = 100;

		public const int DISP_MAX_NAMEVALUE = 1000;

		public const int TAREA_PID_MAX = 4;

		public const int TAREA_PID_TENANT = 0;

		public const int TAREA_PID_RUNING = 1;

		public const int TAREA_PID_COAT1 = 2;

		public const int TAREA_PID_COAT2 = 3;

		public const int TMODE_PATH = 0;

		public const int TMODE_LIFTING = 1;

		public const int TMODE_DRIBBLE = 2;

		public const int TMODE_SHOOT = 3;

		public const int TMODE_RUNNING = 4;

		public const int TMODE_TENANT = 5;

		public const int RESEARCHP_FAN = 0;

		public const int RESEARCHP_IDEA = 1;

		public const int RESEARCHP_PHY = 2;

		public const int RESEARCHP_TAC = 3;

		public const int RESEARCHP_END = 4;

		public const int RESEARCHP_FAIL = -1;

		public const int RECORD_ID_MATCH = 0;

		public const int RECORD_ID_TENANT = 1;

		public const int RECORD_ID_SPONSOR = 2;

		public const int RECORD_ID_SALLARY = 3;

		public const int RECORD_ID_ELSE = 4;

		public const int RECORD_ID_END = 5;

		public const int BUSSTATE_MOVE = 0;

		public const int BUSSTATE_ARRIVE = 1;

		public const int COLLISIONID_PLAYER = 0;

		public const int COLLISIONID_BALL = 1;

		public const int COLLISIONID_MOVEPOINT = 2;

		public const int COLLISIONID_RECEIVE = 3;

		public const int COLLISIONID_HEADING = 4;

		public const int COLLISIONID_TRAINING = 5;

		public const int TENANT_MAX = 15;

		public const int LEAGUE_RESULT_WIN = 0;

		public const int LEAGUE_RESULT_DRAW = 1;

		public const int LEAGUE_RESULT_LOSE = 2;

		public const int LEAGUE_DATA_WINPOINT = 0;

		public const int LEAGUE_DATA_WIN = 1;

		public const int LEAGUE_DATA_DRAW = 2;

		public const int LEAGUE_DATA_LOSE = 3;

		public const int LEAGUE_DATA_GETSCORE = 4;

		public const int LEAGUE_DATA_LOSTSCORE = 5;

		public const int LEAGUE_DATA_SUBSCORE = 6;

		public const int LEAGUE_DATA_OLDER = 7;

		public const int LEAGUE_DATA_TEAMID = 8;

		public const int LEAGUE_DATA_END = 9;

		public const int LEAGUE_ID_AMATEUR = 0;

		public const int LEAGUE_ID_REGION = 1;

		public const int LEAGUE_ID_COUNTRY = 2;

		public const int LEAGUE_ID_WORLD = 3;

		public const int LEAGUE_ID_KAIRO = 4;

		public const int LEAGUE_ID_END = 5;

		public const int LEAGUE_PID = 0;

		public const int MATCH_WIN = 0;

		public const int MATCH_DRAW = 1;

		public const int MATCH_LOSE = 2;

		public const int ELEVEN = 11;

		public const int TPPLAY_ID_MONEY = 0;

		public const int TPPLAY_ID_WIN = 1;

		public const int TPPLAY_ID_DRAW = 2;

		public const int TPPLAY_ID_LOSE = 3;

		public const int TPPLAY_ID_RP_FAN = 4;

		public const int TPPLAY_ID_RP_IDEA = 5;

		public const int TPPLAY_ID_RP_PHY = 6;

		public const int TPPLAY_ID_RP_TAC = 7;

		public const int TPPLAY_ID_TEAMEVA = 8;

		public const int TPPLAY_ID_END = 9;

		public const int TPPLAY_STOCK_NUM = 15;

		public const int UNIFORM_SELECTNUM = 8;

		public const int UNIFORM_WHITE = 8;

		public const int TICKET_RANGE_MIN = 1000;

		public const int TICKET_RANGE_MAX = 30000;

		public const int TICKET_PRICE_MIN = 1000;

		public const int TICKET_PRICE_MAX = 10000;

		public const float TICKET_RATE_MIN = 3f;

		public const float TICKET_RATE_BASE = 1f;

		public const float TICKET_RATE_MAX = 1f / 3f;

		public const float TICKET_FB_RATE_MIN = 1.5f;

		public const float TICKET_FB_RATE_BASE = 1f;

		public const float TICKET_FB_RATE_MAX = 0.2f;

		public const int TICKET_EVA_BORDER = 700;

		public const int TICKET_IDEAL_MIN = 1000;

		public const int TICKET_IDEAL_BORDER = 3000;

		public const int TICKET_IDEAL_MAX = 6000;

		public const int TICKETSALSE_IDEAL_MIN = 65;

		public const int TICKETSALSE_IDEAL_BORDER = 12000;

		public const int TICKETSALSE_IDEAL_MAX = 50000;

		public const float TICKETSALSE_RATE_MIN = 0.8f;

		public const float TICKETSALSE_RATE_BORDER = 1f;

		public const float TICKETSALSE_RATE_MAX = 0.6f;

		public const int EVENTFLG_LEAGUE = 1;

		public const int EVENTFLG_FANACT = 2;

		public const int EVENTFLG_TENANTSTREN = 4;

		public const int EVENTFLG_QUESTMATCH = 8;

		public const int EVENTFLG_TRADE = 16;

		public const int EVENTFLG_SPONSOR = 32;

		public const int EVENTFLG_NUM = 6;

		private const int WARP_SUB_MIN = -300;

		private const int WARP_SUB_MAX = 300;

		public const int EDIT_PHASE_INIT = 0;

		public const int EDIT_PHASE_FORMATION = 1;

		public const int EDIT_PHASE_FCHANGE = 2;

		public const int EDIT_PHASE_FCHANGE2 = 3;

		public const int EDIT_PHASE_FMOVE = 4;

		public const int EDIT_PHASE_FTYPE = 5;

		public const int EDIT_PHASE_MEMCHANGE = 6;

		public const int EDIT_PHASE_MEMCHANGE2 = 7;

		public const int EDIT_PHASE_MEMMOVE = 8;

		public const int EDIT_PHASE_INIT_TO_F = 9;

		public const int EDIT_PHASE_INIT_TO_M = 10;

		public const int EDIT_PHASE_F_TO_INIT = 11;

		public const int EDIT_PHASE_F_TO_INIT2 = 12;

		public const int EDIT_PHASE_M_TO_INIT = 13;

		public const int EDIT_PHASE_ENEMY = 14;

		public const int EDIT_PHASE_INIT_TO_ENE = 15;

		public const int EDIT_PHASE_ENE_TO_INIT = 16;

		public const int EDIT_T1 = 4;

		public const int EDIT_T2 = 8;

		public const int EDIT_FOR_W = 214;

		public const int EDIT_FOR_W2 = 30;

		public const int EDIT_FOR_H = 94;

		public const int EDIT_FOR_RW = 144;

		public const int EDIT_FOR_RH = 90;

		public const int EDIT_MEM_W = 212;

		public const int EDIT_MEM_H = 100;

		public const int ECMTES_MENU = 0;

		public const int ECMTES_FORMENU = 1;

		public const int ECMTES_FCHA = 2;

		public const int ECMTES_FCHA2 = 3;

		public const int ECMTES_FTYPE = 4;

		public const int ECMTES_MEMCHA = 5;

		public const int ECMTES_MEMCHA2 = 6;

		public const int ECMTES_END = 7;

		public const int EMDM_NAME = 0;

		public const int EMDM_PARAM = 1;

		public const int EMDM_POSITION = 2;

		public const int EMDM_LV = 3;

		public const int EMDM_SALLARY = 4;

		public const int EMDM_END = 5;

		private const int E_F_W = 9;

		private const int E_F_H = 9;

		public const int MENUID_FORMATION = 0;

		public const int MENUID_STARTMEMBER = 1;

		public const int MENUID_PLAYERDATA = 2;

		public const int MENUID_COACH = 3;

		public const int MENUID_TRADE = 4;

		public const int MENUID_BATU = 5;

		private const int EDIT_MEMMOVE_TIME = 6;

		private const int P_W = 4;

		private const int N = 5;

		private const int MENUGRAPH_NUM = 5;

		public const float MENU_LEN = 16f;

		public const int MATCHINFOJOIN_LEAGUE = 0;

		public const int MATCHINFOJOIN_QUEST = 1;

		public const int MATCHINFOJOIN_NOTHING = 2;

		public const int POPUPMEM_ID_KEEPCOST = 0;

		public const int POPUPMEM_ID_E_MONEY = 1;

		public const int POPUPMEM_ID_E_TRADEG = 2;

		public const int POPUPMEM_ID_E_TRADE = 3;

		public const int POPUPMEM_ID_E_PLAYER = 4;

		public const int POPUPMEM_ID_E_LEAGUE = 5;

		public const int POPUPMEM_ID_E_QUEST = 6;

		public const int POPUPMEM_ID_E_EXP = 7;

		public const int POPUPMEM_ID_E_KICK = 8;

		public const int POPUPMEM_ID_E_SPD = 9;

		public const int POPUPMEM_ID_E_TEQ = 10;

		public const int POPUPMEM_ID_E_BODY = 11;

		public const int POPUPMEM_ID_E_GK = 12;

		public const int POPUPMEM_ID_E_AURA = 13;

		public const int POPUPMEM_ID_E_FAN = 14;

		public const int POPUPMEM_ID_E_FAN_DECRE = 15;

		public const int POPUPMEM_ID_E_ASSIST = 16;

		public const int POPUPMEM_ID_E_ASSIST_DECRE = 17;

		public const int POPUPMEM_ID_E_NVALUE = 18;

		public const int POPUPMEM_ID_E_NVALUE_DECRE = 19;

		public const int POPUPMEM_ID_E_TEVA = 20;

		public const int POPUPMEM_ID_E_TEVA_DECRE = 21;

		public const int POPUPMEM_ID_E_SPONSOR = 22;

		public const int POPUPMEM_ID_E_TRAINING = 23;

		public const int POPUPMEM_ID_E_TENANT = 24;

		public const int POPUPMEM_ID_E_FANACT = 25;

		public const int POPUPMEM_ID_E_SVISOR = 26;

		public const int POPUPMEM_ID_CON_SPONSOR = 27;

		public const int POPUPMEM_ID_LVUP_PLAYER = 28;

		public const int POPUPMEM_ID_FORMATION = 29;

		public const int POPUPMEM_ID_LVUP_TENANT = 30;

		public const int POPUPMEM_ID_BUILD_START = 31;

		public const int POPUPMEM_ID_STREN_START = 32;

		public const int POPUPMEM_ID_STREN_TAREA = 33;

		public const int POPUPMEM_ID_STREN_STADIUM = 34;

		public const int POPUPMEM_ID_FANACT_START = 35;

		public const int POPUPMEM_ID_REGARDS_YEAR = 36;

		public const int POPUPMEM_ID_CHANGE_POLICY = 37;

		public const int POPUPMEM_ID_SP_TENANT = 38;

		public const int FEELPT_MAX = 100;

		public const int FPT_UPRATE_MIN = 11;

		public const int FPT_UPRATE_CENTER = 60;

		public const int FPT_UPRATE_MAX = 90;

		public const int FPT_UP_CIR_MIN = 30;

		public const int FPT_UP_CIR_MAX = 45;

		public const int FPT_UP_RECT_MIN = 10;

		public const int FPT_UP_RECT_MAX = 20;

		public const int NEGOTIATE_RET_SUCCESS = 1;

		public const int NEGOTIATE_RET_FAIL = 0;

		public const int SPONSOR_EF_CNTANIME_TIME = 12;

		public const int SPONSOR_EF_ADDCNT_TIME = 20;

		public const int SPONSOR_EF_COMMENT_TIME = 20;

		public const int CPOINT_TOTALCUSTOMER = 0;

		public const int CPOINT_TOTAL_SCORE = 1;

		public const int CPOINT_ALL_PLAYERLV = 2;

		public const int CPOINT_WINNUM = 3;

		public const int CPOINT_TEAMEVA = 4;

		public const int CPOINT_SPONSOR_SATISFACT = 5;

		public const int CPOINT_FORMATIONNUM = 6;

		public const int CPOINT_MAX = 7;

		public const float MULTI_TOTALCUSTOMER = 0.01f;

		public const float MULTI_TOTAL_SCORE = 30f;

		public const float MULTI_ALL_PLAYERLV = 15f;

		public const float MULTI_WINNUM = 150f;

		public const float MULTI_TEAMEVA = 15f;

		public const float MULTI_SPONSOR_SATISFACT = 5f;

		public const float MULTI_FORMATIONNUM = 100f;

		private AppData apdat_;

		private Canvas canvas_;

		private FormManager formMan_;

		private static UserData instance;

		public GameManager gm_;

		public Team[] teams_;

		public Ball ball_;

		public PlayerData[][] cpuPlayerDatas_ = JTool.MakeArray<PlayerData[][]>(new int[2] { 2, 11 });

		public int teamEvaRank_;

		public int ticketPrice_;

		public int selectFormationId_;

		public int selectFormationType_;

		public int selectSupervisorId_;

		public int selectUniform_;

		public int selectLeagueId_;

		public int selectQmId_;

		public int selectTrainingPolicy_;

		public int battleQmIndex_;

		private int matchInfoJoin_;

		public int eventActFlg_;

		public int matchInfoWeek_;

		public int matchFlgVictory_;

		public Vector memberPDataist_ = new Vector();

		public int[] researchPoint_;

		public int[][] tpplay_stock = JTool.MakeArray<int[][]>(new int[2] { 15, 9 });

		public int tpplay_battleNum;

		public Vector tpplay_battleRecode = new Vector();

		public Tenant[] tenants_;

		public int league_setu_;

		public int league_teamNum_;

		public int[][] league_fightCombi_;

		public int[][] league_fightResult_;

		public int[][] league_data_;

		public int[][] league_dispOlder_;

		public int[] matchResult_ = new int[3];

		public int matchGetGoalTotal_;

		public int busState_;

		public Vector3D busPos_;

		public Vector3D busV_;

		public Vector3D busGoalPos_;

		public Vector busMovePoint_;

		public int busDirect_;

		public int[] busSDirect = new int[2];

		public int[] busCPos = new int[2];

		public int busCnt;

		public int game_winConsecutive_;

		public long customer_total_;

		public int[] customer_month_ = new int[12];

		public int game_matchEva_;

		public int[] fanParam_;

		public static readonly int[] EVA_INIT = new int[3] { 25, 500, 30 };

		public TPlayer tp_svisor_;

		public int[][][] records_ = JTool.MakeArray<int[][][]>(new int[3] { 12, 5, 2 });

		public Vector record_customer = new Vector();

		public TArea[][] tAreas_;

		public TPlayer[] tPlayers_;

		public TBall[] tBalls_;

		public int[] pDataIdOlderList_;

		public int save_tAreaRootNum_;

		public int[][] tModeList_;

		public int trainig_appearWait;

		public Vector restTPlayers_;

		public int tAreaRootNum_;

		public int tAreaLv_;

		public static readonly int[] TMODE_KEEPER = new int[4] { 0, 1, 2, 3 };

		public static readonly int[] TMODE_NONKEEPER = new int[3] { 0, 1, 2 };

		public bool isSoccerForm_;

		public static readonly string[] RECORD_NAME = new string[5] { "試合", "施設", "ｽﾎﾟﾝｻｰ", "給料", "その他" };

		public static readonly float[][] COLLISION_DATA = new float[6][]
		{
			new float[4] { -12f, 12f, 24f, 24f },
			new float[4] { -10f, 10f, 20f, 20f },
			new float[4] { -50f, 50f, 100f, 100f },
			new float[4] { -6f, 6f, 12f, 12f },
			new float[4] { -30f, 30f, 60f, 60f },
			new float[4] { -3f, 3f, 6f, 6f }
		};

		public static Vector3D[][] COLLISION_GPOS;

		public static int[][][] COLLISION_SPOS;

		private Player disp_player;

		public static int TEST_3KEYSTOP = 0;

		public static int HUMAN_DISP_MODE = 0;

		public Vector moveScreenPX = new Vector();

		public Vector moveScreenPY = new Vector();

		public Vector chMoveP_YOKO = new Vector();

		public Vector chMoveP_TATE = new Vector();

		private int[] temp_testTPlayersId_;

		public static readonly string[] RANK_NAME = new string[10] { "素人ﾁｰﾑ", "初級ﾁｰﾑ", "町内会ﾚﾍﾞﾙ", "ｱﾏﾁｭｱﾚﾍﾞﾙ", "地方ｸﾗﾌﾞ", "ﾌﾟﾛﾚﾍﾞﾙ", "強豪ｸﾗﾌﾞ", "世界ﾚﾍﾞﾙ", "名門ｸﾗﾌﾞ", "銀河系軍団" };

		public static readonly int[] RANK_VALUE = new int[10] { 25, 55, 90, 120, 200, 350, 450, 550, 650, 1010 };

		public static readonly int[] UNIFORM_KEEPER = new int[10] { 0, 2, 2, 0, 2, 0, 1, 1, 0, 0 };

		public int[] TALK_SCHARA_ID;

		public string[] TALK_SCHARA_NAME;

		public int[] TALK_SCHARA_IMGINDEX;

		public static int FLG_CONTROL_DISPPANEL = 0;

		public static int TEST_VALUE1 = 0;

		public static int TEST_VALUE2 = 0;

		public static int TEST_VALUE3 = 0;

		public static int DEBUG_MEMBERPARAM_DISP = 1;

		public static int DEBUG_DISPFPS = 0;

		public static int DEBUG_SALLARY_MAX = 0;

		private int[] base_spos = new int[2];

		internal long chTime1;

		internal long chTime2;

		private int[][] temp_collisionData = JTool.MakeArray<int[][]>(new int[2] { 10, 4 });

		public int[] retNextBattleData = new int[2];

		private static readonly int[][] WARPSUB_BATTLESCORE_PLUS = new int[6][]
		{
			new int[2] { 100, 200 },
			new int[2] { 300, 400 },
			new int[2] { 15, 10 },
			new int[2] { 10, 25 },
			new int[2] { 10, 25 },
			new int[2] { 15, 10 }
		};

		private static readonly int[][] WARPSUB_BATTLESCORE_MINUS = new int[6][]
		{
			new int[2] { 0, 100 },
			new int[2] { 200, 300 },
			new int[2] { 30, 10 },
			new int[2] { 10, 15 },
			new int[2] { 10, 15 },
			new int[2] { 30, 10 }
		};

		private static int[] temp_score = new int[10];

		private static int[] temp_scoreSum = new int[10];

		private static int[] temp_param = new int[6];

		public int phase_;

		public int phaseCnt_;

		public static readonly int[][] EDIT_POS_MENU = new int[2][]
		{
			new int[2] { 16, 41 },
			new int[2] { 16, -13 }
		};

		public static readonly int[][] EDIT_POS_MENU_SOC = new int[2][]
		{
			new int[2] { 16, 28 },
			new int[2] { 16, -53 }
		};

		public static readonly int[][] EDIT_POS_MENU_F = new int[2][]
		{
			new int[2] { 26, 36 },
			new int[2] { 26, -40 }
		};

		public static readonly int[][] EDIT_POS_MENU_M = new int[2][]
		{
			new int[2] { 14, 22 },
			new int[2] { 14, -90 }
		};

		public static readonly int[][] EDIT_POS_MENU_ENE = new int[2][]
		{
			new int[2] { 16, 34 },
			new int[2] { -204, 34 }
		};

		public static readonly int[][] EDIT_POS_FOR_F = new int[2][]
		{
			new int[2] { 13, 90 },
			new int[2] { 13, 107 }
		};

		public static readonly int[][] EDIT_POS_FOR_M = new int[2][]
		{
			new int[2] { 13, 90 },
			new int[2] { 13, 133 }
		};

		public static readonly int[][] EDIT_POS_FOR_ENE = new int[2][]
		{
			new int[2] { 13, 90 },
			new int[2] { -208, 90 }
		};

		public static readonly int[][] EDIT_POS_TOPHELP_F = new int[2][]
		{
			new int[2] { 16, 197 },
			new int[2] { 16, 208 }
		};

		public static readonly int[][] EDIT_POS_TOPHELP_M = new int[2][]
		{
			new int[2] { 16, 197 },
			new int[2] { 16, 238 }
		};

		public static readonly int[][] EDIT_POS_TOPHELP_ENE = new int[2][]
		{
			new int[2] { 16, 197 },
			new int[2] { -204, 197 }
		};

		public static readonly int[][] EDIT_POS_ENE_MENU = new int[2][]
		{
			new int[2] { 236, 28 },
			new int[2] { 16, 28 }
		};

		public static readonly int[][] EDIT_POS_ENE_FOR = new int[2][]
		{
			new int[2] { 233, 107 },
			new int[2] { 13, 107 }
		};

		public static readonly int[][] EDIT_POS_ENE_HELP = new int[2][]
		{
			new int[2] { 236, 208 },
			new int[2] { 16, 208 }
		};

		public int[] et_cur = new int[7];

		public int scrollValue_;

		public Vector editTest_fw = new Vector();

		public Vector editTest_mf = new Vector();

		public Vector editTest_df = new Vector();

		public Vector editTest_gk = new Vector();

		public Vector[] editTest_team;

		public Vector editTest_temp = new Vector();

		public Vector editTest_category = new Vector();

		public int[][] editForPos = JTool.MakeArray<int[][]>(new int[2] { 11, 3 });

		public int[][] editForPosEnemy = JTool.MakeArray<int[][]>(new int[2] { 11, 3 });

		public int editMemDispMode;

		public int emdm_cnt;

		public int[] EMDM_ANIME_TIME = new int[2] { 4, 49 };

		public static readonly int[] EDITMEM_PARAM_COLOR = new int[5]
		{
			Graphics.GetColorOfRGB(255, 84, 84),
			Graphics.GetColorOfRGB(0, 128, 255),
			Graphics.GetColorOfRGB(191, 255, 127),
			Graphics.GetColorOfRGB(207, 143, 79),
			Graphics.GetColorOfRGB(224, 224, 0)
		};

		internal string[] MENU_STR = new string[3] { "ﾌｫｰﾒｰｼｮﾝ", "試合ﾒﾝﾊﾞｰ", "ﾒﾝﾊﾞｰ情報" };

		internal string[] HELP_STR = new string[6]
		{
			"ﾌｫｰﾒｰｼｮﾝを変更します。",
			"試合のﾒﾝﾊﾞｰを変更します。",
			"選手情報です。",
			"監督情報です。",
			"選手を獲得します。",
			string.Empty
		};

		public Vector editTest_formation = new Vector();

		public static string[] FORMATIONTYPE_STR = new string[3] { "通常", "攻め重視", "守り重視" };

		public static readonly int[][] GRAPH_POS = new int[5][]
		{
			new int[2] { -7, -28 },
			new int[2] { 17, -15 },
			new int[2] { 10, 10 },
			new int[2] { -27, 10 },
			new int[2] { -37, -15 }
		};

		private int dx;

		private int dy;

		private int[] d1 = new int[2];

		private int[] d2 = new int[2];

		private int[] CR_COLOR = new int[4]
		{
			Graphics.GetColorOfRGB(255, 164, 0),
			Graphics.GetColorOfRGB(255, 206, 16),
			Graphics.GetColorOfRGB(255, 248, 64),
			Graphics.GetColorOfRGB(255, 206, 16)
		};

		private Vector tenantList = new Vector();

		private int[][] MENUGRAPH_POINTPOS = new int[6][]
		{
			new int[2] { 0, -16 },
			new int[2]
			{
				(int)(16.0 * JMath.Cos(0.3141592653589793)),
				-(int)(16.0 * JMath.Sin(0.3141592653589793))
			},
			new int[2]
			{
				(int)(16.0 * JMath.Cos(-0.9424777960769379)),
				-(int)(16.0 * JMath.Sin(-0.9424777960769379))
			},
			new int[2]
			{
				(int)(16.0 * JMath.Cos(-2.199114857512855)),
				-(int)(16.0 * JMath.Sin(-2.199114857512855))
			},
			new int[2]
			{
				(int)(16.0 * JMath.Cos(-3.455751918948773)),
				-(int)(16.0 * JMath.Sin(-3.455751918948773))
			},
			new int[2]
		};

		internal int[][] MENUGRAPH_OFF = new int[4][]
		{
			new int[2] { 0, -1 },
			new int[2] { 1, 0 },
			new int[2] { 0, 1 },
			new int[2] { -1, 0 }
		};

		public int[] playerParam1 = new int[5];

		public int[] playerParam2 = new int[5];

		public static readonly string[] POPUP_MEM = new string[39]
		{
			"維持費 <0>支払いました", "臨時収入<0>ｹﾞｯﾄ", "選手の獲得リストが追加!", "選手の獲得リストが追加!", "<0>選手が加入しました", "<0>に参戦可能です。", "参戦可能なｶｯﾌﾟ戦が増加!", "全選手の経験値が<0>増加!", "全選手のｷｯｸ力が<0>増加!", "全選手のｽﾋﾟｰﾄﾞが<0>増加!",
			"全選手のﾃｸﾆｯｸが<0>増加!", "全選手のﾎﾞﾃﾞｨが<0>増加!", "全選手のｷｰﾊﾟｰ力が<0>増加!", "全選手のｵｰﾗが<0>増加!", "ﾌｧﾝが<0>人増えました", "ﾌｧﾝが<0>人減りました…", "応援熱が<0>増加!", "応援熱が<0>減少…", "知名度が<0>上がりました", "知名度が<0>下がりました",
			"ﾁｰﾑ評価が<0>上がりました", "ﾁｰﾑ評価が<0>下がりました", "スポンサー候補が追加!", "特訓 <0>が追加!", "<0>が建設可能です。", "地域活動 <co=0064FF><0></co>を発見", "監督候補が追加!", "<0>と正式契約", "<0>さんLv<1>にｱｯﾌﾟ!", "ﾌｫｰﾒｰｼｮﾝ <0>が追加",
			"<0>がLv<1>にｱｯﾌﾟ!", "<0>の建設開始!", "<0>の研究開始!", "<0>を実行しました!!", "<0>を実行しました!", "<0>の活動開始!", "今年もよろしくお願いします。", "練習方針を切り替えました。", "施設の強化 <0>が追加!"
		};

		public static int[] NEGOTIATE_RESULT_TH = new int[3];

		public static int[] NEGOTIATE_RESULT_VALUE = new int[3];

		public static int[] NEGOTIATE_RESULT_DISPVALUE = new int[4];

		public static int NEGOTIATE_RESULT;

		public static readonly int[] WAIT_TIME = new int[3] { 15, 50, 85 };

		public static int[] EF_VALUE = new int[2];

		private static readonly string[] FPER_STR = new string[5] { "成功!", "あともう少し!", "手ごたえあり", "まだまだ", "かなり厳しい…。" };

		public static int[] EF_COMMENT = new int[3];

		public static int[][] EF_COMMENT_SETTING = JTool.MakeArray<int[][]>(new int[2] { 3, 2 });

		public static readonly string[][] EF_FEELPT_COMMENT_ = new string[2][]
		{
			new string[3] { "お願いです!", "いかがですか", "どうでしょう" },
			new string[3] { "お願いです!", "いかがですか", "どうでしょう" }
		};

		public static readonly string[][] CPOINT_STR = new string[7][]
		{
			new string[2] { "累計観客数は…", "人" },
			new string[2] { "全得点数は…", "点" },
			new string[2] { "獲得した全選手のレベル", "ﾚﾍﾞﾙ" },
			new string[2] { "累計勝利数は…", "勝" },
			new string[2]
			{
				"ﾁｰﾑ評価は…",
				string.Empty
			},
			new string[2]
			{
				"全ｽﾎﾟﾝｻｰ満足度は…",
				string.Empty
			},
			new string[2] { "発見したﾌｫｰﾒｰｼｮﾝは…", "個" }
		};

		public long[][] clear_point = JTool.MakeArray<long[][]>(new int[2] { 7, 2 });

		public long ranking_point;

		private UserData()
		{
			Init();
		}

		public static UserData GetInstance()
		{
			if (instance == null)
			{
				instance = new UserData();
			}
			return instance;
		}

		public static void PrintOut()
		{
			Log.Info("VALUE1=" + TEST_VALUE1 + ", VALUE2=" + TEST_VALUE2 + ", VALUE3 = " + TEST_VALUE3);
		}

		public static void Update_45v1()
		{
			if (Canvas.GetInstance().CheckKeyRepeat(16))
			{
				TEST_VALUE1--;
			}
			if (Canvas.GetInstance().CheckKeyRepeat(32))
			{
				TEST_VALUE1++;
			}
		}

		public static void Update_78v2()
		{
			if (Canvas.GetInstance().CheckKeyRepeat(128))
			{
				TEST_VALUE2--;
			}
			if (Canvas.GetInstance().CheckKeyRepeat(256))
			{
				TEST_VALUE2++;
			}
		}

		public static void Update_12v3()
		{
			if (Canvas.GetInstance().CheckKeyRepeat(2))
			{
				TEST_VALUE3--;
			}
			if (Canvas.GetInstance().CheckKeyRepeat(4))
			{
				TEST_VALUE3++;
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)teamEvaRank_);
			StreamUtil.WriteInt(os, ticketPrice_);
			StreamUtil.WriteShort(os, (short)selectFormationId_);
			StreamUtil.WriteShort(os, (short)selectFormationType_);
			StreamUtil.WriteShort(os, (short)selectSupervisorId_);
			StreamUtil.WriteShort(os, (short)selectUniform_);
			StreamUtil.WriteShort(os, (short)selectLeagueId_);
			StreamUtil.WriteShort(os, (short)selectQmId_);
			StreamUtil.WriteShort(os, (short)selectTrainingPolicy_);
			StreamUtil.WriteShort(os, (short)battleQmIndex_);
			StreamUtil.WriteShort(os, (short)matchInfoJoin_);
			StreamUtil.WriteInt(os, eventActFlg_);
			for (int i = 0; i < TrainingData.TRA_PLAYER.Length; i++)
			{
				StreamUtil.WriteShort(os, (short)TrainingData.TRA_PLAYER[i]);
			}
			StreamUtil.WriteShort(os, (short)TrainingData.TRA_SELECT_TRA);
			for (int j = 0; j < researchPoint_.Length; j++)
			{
				StreamUtil.WriteShort(os, (short)researchPoint_[j]);
			}
			StreamUtil.WriteShort(os, (short)memberPDataist_.Size());
			for (int k = 0; k < memberPDataist_.Size(); k++)
			{
				PlayerData playerData = (PlayerData)memberPDataist_.ElementAt(k);
				StreamUtil.WriteShort(os, (short)playerData.id_);
			}
			for (int l = 0; l < tpplay_stock.Length; l++)
			{
				for (int m = 0; m < tpplay_stock[l].Length; m++)
				{
					StreamUtil.WriteInt(os, tpplay_stock[l][m]);
				}
			}
			StreamUtil.WriteInt(os, tpplay_battleNum);
			StreamUtil.WriteInt(os, tpplay_battleRecode.Size());
			for (int n = 0; n < tpplay_battleRecode.Size(); n++)
			{
				int[] array = (int[])tpplay_battleRecode.ElementAt(n);
				StreamUtil.WriteInt(os, array[0]);
				StreamUtil.WriteInt(os, array[1]);
			}
			StreamUtil.WriteShort(os, (short)matchInfoWeek_);
			StreamUtil.WriteShort(os, (short)matchFlgVictory_);
			StreamUtil.WriteShort(os, (short)league_setu_);
			StreamUtil.WriteShort(os, (short)league_teamNum_);
			StreamUtil.WriteShort(os, (short)league_data_.Length);
			StreamUtil.WriteShort(os, (short)league_data_[0].Length);
			for (int num = 0; num < league_data_.Length; num++)
			{
				for (int num2 = 0; num2 < league_data_[num].Length; num2++)
				{
					StreamUtil.WriteInt(os, league_data_[num][num2]);
				}
			}
			for (int num3 = 0; num3 < league_dispOlder_.Length; num3++)
			{
				for (int num4 = 0; num4 < league_dispOlder_[num3].Length; num4++)
				{
					StreamUtil.WriteInt(os, league_dispOlder_[num3][num4]);
				}
			}
			for (int num5 = 0; num5 < matchResult_.Length; num5++)
			{
				StreamUtil.WriteInt(os, matchResult_[num5]);
			}
			StreamUtil.WriteInt(os, matchGetGoalTotal_);
			StreamUtil.WriteInt(os, busState_);
			StreamUtil.WriteVector3D(os, busPos_);
			StreamUtil.WriteVector3D(os, busV_);
			StreamUtil.WriteVector3D(os, busGoalPos_);
			StreamUtil.WriteShort(os, (short)busMovePoint_.Size());
			for (int num6 = 0; num6 < busMovePoint_.Size(); num6++)
			{
				Vector3D value = (Vector3D)busMovePoint_.ElementAt(num6);
				StreamUtil.WriteVector3D(os, value);
			}
			StreamUtil.WriteShort(os, (short)busDirect_);
			StreamUtil.WriteInt(os, busSDirect[0]);
			StreamUtil.WriteInt(os, busSDirect[1]);
			StreamUtil.WriteInt(os, busCPos[0]);
			StreamUtil.WriteInt(os, busCPos[1]);
			StreamUtil.WriteInt(os, busCnt);
			for (int num7 = 0; num7 < fanParam_.Length; num7++)
			{
				StreamUtil.WriteInt(os, fanParam_[num7]);
			}
			StreamUtil.WriteInt(os, game_winConsecutive_);
			StreamUtil.WriteLong(os, customer_total_);
			for (int num8 = 0; num8 < customer_month_.Length; num8++)
			{
				StreamUtil.WriteInt(os, customer_month_[num8]);
			}
			tp_svisor_.Serialize(os);
			for (int num9 = 0; num9 < records_.Length; num9++)
			{
				for (int num10 = 0; num10 < records_[num9].Length; num10++)
				{
					StreamUtil.WriteInt(os, records_[num9][num10][0]);
					StreamUtil.WriteInt(os, records_[num9][num10][1]);
				}
			}
			StreamUtil.WriteInt(os, record_customer.Size());
			for (int num11 = 0; num11 < record_customer.Size(); num11++)
			{
				JInteger jInteger = (JInteger)record_customer.ElementAt(num11);
				StreamUtil.WriteInt(os, jInteger.IntValue());
			}
			StreamUtil.WriteShort(os, (short)tAreaRootNum_);
			StreamUtil.WriteShort(os, (short)tAreaLv_);
			StreamUtil.WriteInt(os, tModeList_.Length);
			if (tModeList_.Length > 0)
			{
				StreamUtil.WriteInt(os, tModeList_[0].Length);
			}
			else
			{
				StreamUtil.WriteInt(os, 0);
			}
			for (int num12 = 0; num12 < tModeList_.Length; num12++)
			{
				for (int num13 = 0; num13 < tModeList_[num12].Length; num13++)
				{
					StreamUtil.WriteInt(os, tModeList_[num12][num13]);
				}
			}
			StreamUtil.WriteInt(os, tAreas_.Length);
			if (tModeList_.Length > 0)
			{
				StreamUtil.WriteInt(os, tAreas_[0].Length);
			}
			else
			{
				StreamUtil.WriteInt(os, 0);
			}
			for (int num14 = 0; num14 < tAreas_.Length; num14++)
			{
				for (int num15 = 0; num15 < tAreas_[num14].Length; num15++)
				{
					tAreas_[num14][num15].Serialize(os);
				}
			}
			StreamUtil.WriteInt(os, tPlayers_.Length);
			for (int num16 = 0; num16 < tPlayers_.Length; num16++)
			{
				tPlayers_[num16].Serialize(os);
			}
			StreamUtil.WriteInt(os, tBalls_.Length);
			for (int num17 = 0; num17 < tBalls_.Length; num17++)
			{
				tBalls_[num17].Serialize(os);
			}
			StreamUtil.WriteInt(os, pDataIdOlderList_.Length);
			for (int num18 = 0; num18 < pDataIdOlderList_.Length; num18++)
			{
				StreamUtil.WriteInt(os, pDataIdOlderList_[num18]);
			}
			StreamUtil.WriteInt(os, save_tAreaRootNum_);
			StreamUtil.WriteInt(os, trainig_appearWait);
			StreamUtil.WriteInt(os, restTPlayers_.Size());
			for (int num19 = 0; num19 < restTPlayers_.Size(); num19++)
			{
				TPlayer tPlayer = (TPlayer)restTPlayers_.ElementAt(num19);
				StreamUtil.WriteInt(os, tPlayer.trainingId_[0]);
			}
		}

		public void Deserialize(InputStream stream)
		{
			teamEvaRank_ = StreamUtil.ReadShort(stream);
			ticketPrice_ = StreamUtil.ReadInt(stream);
			selectFormationId_ = StreamUtil.ReadShort(stream);
			selectFormationType_ = StreamUtil.ReadShort(stream);
			selectSupervisorId_ = StreamUtil.ReadShort(stream);
			selectUniform_ = StreamUtil.ReadShort(stream);
			selectLeagueId_ = StreamUtil.ReadShort(stream);
			selectQmId_ = StreamUtil.ReadShort(stream);
			selectTrainingPolicy_ = StreamUtil.ReadShort(stream);
			battleQmIndex_ = StreamUtil.ReadShort(stream);
			matchInfoJoin_ = StreamUtil.ReadShort(stream);
			eventActFlg_ = StreamUtil.ReadInt(stream);
			for (int i = 0; i < TrainingData.TRA_PLAYER.Length; i++)
			{
				TrainingData.TRA_PLAYER[i] = StreamUtil.ReadShort(stream);
			}
			TrainingData.TRA_SELECT_TRA = StreamUtil.ReadShort(stream);
			for (int j = 0; j < researchPoint_.Length; j++)
			{
				researchPoint_[j] = StreamUtil.ReadShort(stream);
			}
			memberPDataist_ = new Vector();
			int num = StreamUtil.ReadShort(stream);
			for (int k = 0; k < num; k++)
			{
				PlayerData obj = apdat_.pData_[StreamUtil.ReadShort(stream)];
				memberPDataist_.AddElement(obj);
			}
			for (int l = 0; l < tpplay_stock.Length; l++)
			{
				for (int m = 0; m < tpplay_stock[l].Length; m++)
				{
					tpplay_stock[l][m] = StreamUtil.ReadInt(stream);
				}
			}
			tpplay_battleNum = StreamUtil.ReadInt(stream);
			tpplay_battleRecode = new Vector();
			int num2 = StreamUtil.ReadInt(stream);
			for (int n = 0; n < num2; n++)
			{
				int[] obj2 = new int[2]
				{
					StreamUtil.ReadInt(stream),
					StreamUtil.ReadInt(stream)
				};
				tpplay_battleRecode.AddElement(obj2);
			}
			League_init();
			matchInfoWeek_ = StreamUtil.ReadShort(stream);
			matchFlgVictory_ = StreamUtil.ReadShort(stream);
			league_setu_ = StreamUtil.ReadShort(stream);
			league_teamNum_ = StreamUtil.ReadShort(stream);
			int num3 = StreamUtil.ReadShort(stream);
			int num4 = StreamUtil.ReadShort(stream);
			league_data_ = JTool.MakeArray<int[][]>(new int[2] { num3, num4 });
			for (int num5 = 0; num5 < league_data_.Length; num5++)
			{
				for (int num6 = 0; num6 < league_data_[num5].Length; num6++)
				{
					league_data_[num5][num6] = StreamUtil.ReadInt(stream);
				}
			}
			league_dispOlder_ = JTool.MakeArray<int[][]>(new int[2] { num3, 3 });
			for (int num7 = 0; num7 < league_dispOlder_.Length; num7++)
			{
				for (int num8 = 0; num8 < league_dispOlder_[num7].Length; num8++)
				{
					league_dispOlder_[num7][num8] = StreamUtil.ReadInt(stream);
				}
			}
			for (int num9 = 0; num9 < matchResult_.Length; num9++)
			{
				matchResult_[num9] = StreamUtil.ReadInt(stream);
			}
			matchGetGoalTotal_ = StreamUtil.ReadInt(stream);
			busState_ = StreamUtil.ReadInt(stream);
			busPos_ = StreamUtil.ReadVector3D(stream);
			busV_ = StreamUtil.ReadVector3D(stream);
			busGoalPos_ = StreamUtil.ReadVector3D(stream);
			busMovePoint_ = new Vector();
			int num10 = StreamUtil.ReadShort(stream);
			for (int num11 = 0; num11 < num10; num11++)
			{
				Vector3D obj3 = StreamUtil.ReadVector3D(stream);
				busMovePoint_.AddElement(obj3);
			}
			busDirect_ = StreamUtil.ReadShort(stream);
			busSDirect[0] = StreamUtil.ReadInt(stream);
			busSDirect[1] = StreamUtil.ReadInt(stream);
			busCPos[0] = StreamUtil.ReadInt(stream);
			busCPos[1] = StreamUtil.ReadInt(stream);
			busCnt = StreamUtil.ReadInt(stream);
			fanParam_ = new int[3];
			for (int num12 = 0; num12 < fanParam_.Length; num12++)
			{
				fanParam_[num12] = StreamUtil.ReadInt(stream);
			}
			game_winConsecutive_ = StreamUtil.ReadInt(stream);
			customer_total_ = StreamUtil.ReadLong(stream);
			for (int num13 = 0; num13 < customer_month_.Length; num13++)
			{
				customer_month_[num13] = StreamUtil.ReadInt(stream);
			}
			tp_svisor_ = new TPlayer(2);
			tp_svisor_.Init_training(0, 0);
			tp_svisor_.Deserialize(stream);
			apdat_.pData_[0].SetGameData(tp_svisor_);
			for (int num14 = 0; num14 < records_.Length; num14++)
			{
				for (int num15 = 0; num15 < records_[num14].Length; num15++)
				{
					records_[num14][num15][0] = StreamUtil.ReadInt(stream);
					records_[num14][num15][1] = StreamUtil.ReadInt(stream);
				}
			}
			int num16 = StreamUtil.ReadInt(stream);
			record_customer = new Vector();
			for (int num17 = 0; num17 < num16; num17++)
			{
				int value = StreamUtil.ReadInt(stream);
				record_customer.AddElement(new JInteger(value));
			}
			tAreaRootNum_ = StreamUtil.ReadShort(stream);
			tAreaLv_ = StreamUtil.ReadShort(stream);
			int num18 = StreamUtil.ReadInt(stream);
			int num19 = StreamUtil.ReadInt(stream);
			tModeList_ = JTool.MakeArray<int[][]>(new int[2] { num18, num19 });
			for (int num20 = 0; num20 < tModeList_.Length; num20++)
			{
				for (int num21 = 0; num21 < tModeList_[num20].Length; num21++)
				{
					tModeList_[num20][num21] = StreamUtil.ReadInt(stream);
				}
			}
			int num22 = StreamUtil.ReadInt(stream);
			int num23 = StreamUtil.ReadInt(stream);
			tAreas_ = JTool.MakeArray<TArea[][]>(new int[2] { num22, num23 });
			for (int num24 = 0; num24 < tAreas_.Length; num24++)
			{
				for (int num25 = 0; num25 < tAreas_[num24].Length; num25++)
				{
					tAreas_[num24][num25] = new TArea();
					tAreas_[num24][num25].Init(num24, num25);
					tAreas_[num24][num25].SetTrainingArea(tModeList_[num24][num25]);
				}
			}
			for (int num26 = 0; num26 < tAreas_.Length; num26++)
			{
				for (int num27 = 0; num27 < tAreas_[num26].Length; num27++)
				{
					tAreas_[num26][num27].Deserialize(stream);
				}
			}
			int num28 = StreamUtil.ReadInt(stream);
			tPlayers_ = new TPlayer[num28];
			for (int num29 = 0; num29 < tPlayers_.Length; num29++)
			{
				tPlayers_[num29] = new TPlayer(1);
				tPlayers_[num29].Init_training(0, 0);
				tPlayers_[num29].Deserialize(stream);
			}
			int num30 = StreamUtil.ReadInt(stream);
			tBalls_ = new TBall[num30];
			for (int num31 = 0; num31 < tBalls_.Length; num31++)
			{
				tBalls_[num31] = new TBall(num31);
				tBalls_[num31].Init();
				tBalls_[num31].Deserialize(stream);
			}
			int num32 = StreamUtil.ReadInt(stream);
			pDataIdOlderList_ = new int[num32];
			for (int num33 = 0; num33 < pDataIdOlderList_.Length; num33++)
			{
				pDataIdOlderList_[num33] = StreamUtil.ReadInt(stream);
			}
			save_tAreaRootNum_ = StreamUtil.ReadInt(stream);
			trainig_appearWait = StreamUtil.ReadInt(stream);
			num32 = StreamUtil.ReadInt(stream);
			temp_testTPlayersId_ = new int[num32];
			for (int num34 = 0; num34 < temp_testTPlayersId_.Length; num34++)
			{
				temp_testTPlayersId_[num34] = StreamUtil.ReadInt(stream);
			}
			apdat_.mapMain_.UpdateTenantMapcip();
		}

		public void Deserialize2()
		{
			if (restTPlayers_.Size() > 0)
			{
				restTPlayers_.RemoveAllElements();
			}
			for (int i = 0; i < temp_testTPlayersId_.Length; i++)
			{
				TPlayer obj = tPlayers_[temp_testTPlayersId_[i]];
				restTPlayers_.AddElement(obj);
			}
			for (int j = 0; j < tAreas_.Length; j++)
			{
				for (int k = 0; k < tAreas_[j].Length; k++)
				{
					tAreas_[j][k].Deserialize2();
				}
			}
			for (int l = 0; l < tPlayers_.Length; l++)
			{
				tPlayers_[l].Deserialize2();
			}
			for (int m = 0; m < tBalls_.Length; m++)
			{
				tBalls_[m].Deserialize2();
			}
		}

		public void Init()
		{
			apdat_ = AppData.GetInstance();
			canvas_ = Canvas.GetInstance();
			formMan_ = FormManager.GetInstance();
			gm_ = GameManager.GetInstance();
			GameManager.udat_ = this;
			disp_player = new Player(0);
			researchPoint_ = new int[4];
			tenants_ = new Tenant[15];
			for (int i = 0; i < tenants_.Length; i++)
			{
				tenants_[i] = new Tenant(i);
			}
			busPos_ = new Vector3D();
			busV_ = new Vector3D();
			busGoalPos_ = new Vector3D();
			busMovePoint_ = new Vector();
			GameUtil gameUtil = new GameUtil();
			SetCollisionPos();
			Training_init();
		}

		public void NewGame()
		{
			if (memberPDataist_.Size() > 0)
			{
				memberPDataist_.RemoveAllElements();
			}
			for (int i = 0; i < apdat_.pData_.Length; i++)
			{
				PlayerData playerData = apdat_.pData_[i];
				if (BitUtil.Check(playerData.flag_, 1))
				{
					PlayerData.Employ(playerData.id_);
					playerData.new_ = false;
				}
			}
			TeamData teamData = apdat_.tData_[0];
			teamData.AddTeamEva(0);
			ticketPrice_ = 1000;
			selectTrainingPolicy_ = 0;
			apdat_.tData_[0].bodyImg_ = selectUniform_;
			apdat_.tData_[0].keeperImg_ = UNIFORM_KEEPER[selectUniform_];
			apdat_.tData_[0].uniform_ = selectUniform_;
			for (int j = 0; j < apdat_.tData_.Length; j++)
			{
				if (j == 0) { continue; }
TeamData teamData2 = apdat_.tData_[j];
if (teamData2.uniform_ == selectUniform_)
					{
						teamData2.bodyImg_ = 8;
						teamData2.keeperImg_ = UNIFORM_KEEPER[8];
						teamData2.uniform_ = 8;
					}
			}
			matchInfoJoin_ = 2;
			selectLeagueId_ = 0;
			for (int k = 0; k < apdat_.leagueData_.Length; k++)
			{
				LeagueData leagueData = apdat_.leagueData_[k];
				if (BitUtil.Check(leagueData.flag_, 1))
				{
					selectLeagueId_ = k;
					break;
				}
			}
			League_init();
			fanParam_ = new int[3];
			for (int l = 0; l < fanParam_.Length; l++)
			{
				fanParam_[l] = EVA_INIT[l];
			}
			for (int m = 0; m < researchPoint_.Length; m++)
			{
				AddResearchPoint(m, 10);
			}
			tAreaRootNum_ = 1;
			tAreaLv_ = 0;
			selectQmId_ = -1;
			battleQmIndex_ = 0;
			for (int n = 0; n < tenants_.Length; n++)
			{
				tenants_[n].Init(TenantData.TENANT_ROOT[n]);
				if (TenantData.INIT_TENANTID[n] != -1)
				{
					tenants_[n].BuildTenant(TenantData.INIT_TENANTID[n]);
					tenants_[n].ChangeState(1);
				}
			}
			for (int num = 0; num < apdat_.formationData_.Length; num++)
			{
				apdat_.formationData_[num].NewGame();
			}
			for (int num2 = 0; num2 < apdat_.tradeData_.Length; num2++)
			{
				apdat_.tradeData_[num2].NewGame();
			}
			for (int num3 = 0; num3 < apdat_.sponsorData_.Length; num3++)
			{
				apdat_.sponsorData_[num3].NewGame();
			}
			for (int num4 = 0; num4 < apdat_.svisorData_.Length; num4++)
			{
				apdat_.svisorData_[num4].NewGame();
			}
			for (int num5 = 0; num5 < apdat_.qmData_.Length; num5++)
			{
				apdat_.qmData_[num5].NewGame();
			}
			for (int num6 = 0; num6 < apdat_.trainingData_.Length; num6++)
			{
				apdat_.trainingData_[num6].NewGame();
			}
			for (int num7 = 0; num7 < apdat_.fanActData_.Length; num7++)
			{
				apdat_.fanActData_[num7].NewGame();
			}
			for (int num8 = 0; num8 < apdat_.fanBonus_.Length; num8++)
			{
				apdat_.fanBonus_[num8].NewGame();
			}
			for (int num9 = 0; num9 < apdat_.tenantData_.Length; num9++)
			{
				apdat_.tenantData_[num9].NewGame();
			}
			for (int num10 = 0; num10 < apdat_.leagueData_.Length; num10++)
			{
				apdat_.leagueData_[num10].NewGame();
			}
			try
			{
				ReflectContinueGameData();
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
			FormationData.UpdateStamemRole();
			apdat_.frmGame_.ChangeState(0);
			Init_bus();
			apdat_.mapMain_.UpdateTenantMapcip();
			Camera.ConvertGlobalToScreen(MapMain.MOVE_POINT[3], base_spos);
			Camera.save_pos.x_ = base_spos[0];
			Camera.save_pos.y_ = base_spos[1];
			Camera.save_cursorPos.x_ = base_spos[0];
			Camera.save_cursorPos.y_ = base_spos[1];
			apdat_.camera.SetMapMainBasePos();
			SupervisorData supervisorData = apdat_.svisorData_[selectSupervisorId_];
			tp_svisor_ = new TPlayer(2);
			tp_svisor_.Init_training(0, 0);
			tp_svisor_.faceImg_ = supervisorData.headImg_;
			tp_svisor_.bodyImg_ = supervisorData.bodyImg_;
			tp_svisor_.tData_id_ = 0;
			tp_svisor_.pos_.Set(MapMain.MOVE_POINT[8]);
			tp_svisor_.direct_ = 2;
			tp_svisor_.role_ = 3;
			tp_svisor_.AddCommandSvisorAct();
			tp_svisor_.pData_id_ = 0;
			apdat_.pData_[0].SetGameData(tp_svisor_);
		}

		public void ProcCommonMethod()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < apdat_.pData_.Length; i++)
			{
				PlayerData playerData = apdat_.pData_[i];
				if (BitUtil.Check(playerData.flag_, 4))
				{
					cpuPlayerDatas_[0][num++] = playerData;
				}
				else if (BitUtil.Check(playerData.flag_, 8))
				{
					cpuPlayerDatas_[1][num2++] = playerData;
				}
			}
		}

		public Player GetPlayer(int teamId, int id)
		{
			return teams_[teamId].players_[id];
		}

		public TPlayer GetTPlayer(int id)
		{
			return tPlayers_[id];
		}

		public Player GetDispPlayer()
		{
			return disp_player;
		}

		public void SetDispPlayerData(int seb, int bodyImg, int faceImg, int frame, int direct)
		{
			SetDispPlayerData(seb, bodyImg, faceImg, frame, direct, 3, 0);
		}

		public void SetDispPlayerData(int seb, int bodyImg, int faceImg, int frame, int direct, int role, int tData_id)
		{
			disp_player.seb_ = seb;
			disp_player.bodyImg_ = bodyImg;
			disp_player.faceImg_ = faceImg;
			disp_player.frame_ = frame;
			disp_player.direct_ = direct;
			disp_player.role_ = role;
			disp_player.tData_id_ = tData_id;
		}

		public void DrawDispPlayer(Graphics g, int x, int y)
		{
			int layer_id = disp_player.SetDrawParam(disp_player.faceImg_, disp_player.bodyImg_, disp_player.seb_, disp_player.frame_);
			disp_player.Draw(g, x, y, layer_id, false);
		}

		public void ChTime1()
		{
			chTime1 = JSystem.CurrentTimeMillis();
		}

		public void ChTime2()
		{
			chTime2 = JSystem.CurrentTimeMillis();
		}

		public void SetCollisionPos()
		{
			COLLISION_GPOS = new Vector3D[COLLISION_DATA.Length][];
			COLLISION_SPOS = new int[COLLISION_DATA.Length][][];
			for (int i = 0; i < COLLISION_DATA.Length; i++)
			{
				COLLISION_GPOS[i] = new Vector3D[4]
				{
					new Vector3D(COLLISION_DATA[i][0] - COLLISION_DATA[i][2] / 2f, 0f, COLLISION_DATA[i][1] + COLLISION_DATA[i][3] / 2f),
					new Vector3D(COLLISION_DATA[i][0] + COLLISION_DATA[i][2] / 2f, 0f, COLLISION_DATA[i][1] + COLLISION_DATA[i][3] / 2f),
					new Vector3D(COLLISION_DATA[i][0] + COLLISION_DATA[i][2] / 2f, 0f, COLLISION_DATA[i][1] - COLLISION_DATA[i][3] / 2f),
					new Vector3D(COLLISION_DATA[i][0] - COLLISION_DATA[i][2] / 2f, 0f, COLLISION_DATA[i][1] - COLLISION_DATA[i][3] / 2f)
				};
				COLLISION_SPOS[i] = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
				for (int j = 0; j < COLLISION_SPOS[i].Length; j++)
				{
					Camera.ConvertGlobalToScreen(COLLISION_GPOS[i][j], COLLISION_SPOS[i][j]);
				}
			}
		}

		public int[] GetCollisionData(int retIndex, int collisionId)
		{
			temp_collisionData[retIndex][0] = (int)COLLISION_GPOS[collisionId][0].x_;
			temp_collisionData[retIndex][1] = (int)COLLISION_GPOS[collisionId][0].z_;
			temp_collisionData[retIndex][2] = (int)COLLISION_DATA[collisionId][2];
			temp_collisionData[retIndex][3] = (int)COLLISION_DATA[collisionId][3];
			return temp_collisionData[retIndex];
		}

		public void League_init()
		{
			LeagueData leagueData = apdat_.leagueData_[selectLeagueId_];
			matchInfoWeek_ = 0;
			league_setu_ = 0;
			league_teamNum_ = leagueData.matchEnemy_.Length;
			league_fightCombi_ = JTool.MakeArray<int[][]>(new int[2]
			{
				league_teamNum_ / 2,
				2
			});
			league_fightResult_ = JTool.MakeArray<int[][]>(new int[2]
			{
				league_teamNum_ / 2,
				2
			});
			league_data_ = JTool.MakeArray<int[][]>(new int[2] { league_teamNum_, 9 });
			league_dispOlder_ = JTool.MakeArray<int[][]>(new int[2] { league_teamNum_, 3 });
			for (int i = 0; i < league_dispOlder_.Length; i++)
			{
				league_dispOlder_[i][0] = leagueData.matchEnemyNext_[i];
			}
			for (int j = 0; j < league_data_.Length; j++)
			{
				TeamData teamData = apdat_.tData_[leagueData.matchEnemy_[j]];
				league_data_[j][8] = leagueData.matchEnemy_[j];
				league_data_[j][7] = j + 1;
			}
			SetFightCombi(league_setu_);
		}

		private void SetFightCombi(int setu)
		{
			for (int i = 0; i < league_fightCombi_.Length; i++)
			{
				league_fightCombi_[i][0] = i;
				league_fightCombi_[i][1] = league_teamNum_ - 1 - i;
			}
			int num;
for (int j = 0; j < league_fightCombi_.Length; j++)
			{

				if (j != 0)
				{
					num = league_fightCombi_[j][0] - setu;
					if (num <= 0)
					{
						num += league_teamNum_ - 1;
					}
					league_fightCombi_[j][0] = num;
				}
				num = league_fightCombi_[j][1] - setu;
				if (num <= 0)
				{
					num += league_teamNum_ - 1;
				}
				league_fightCombi_[j][1] = num;
			}
		}

		public int[] GetNextBattleLeagueIndex(int setu)
		{
			if (setu == league_teamNum_ - 1)
			{
				return null;
			}
			LeagueData leagueData = apdat_.leagueData_[selectLeagueId_];
			SetFightCombi(setu);
			for (int i = 0; i < league_fightCombi_.Length; i++)
			{
				if (leagueData.matchEnemy_[league_fightCombi_[i][0]] == 0)
				{
					retNextBattleData[0] = league_fightCombi_[i][1];
					retNextBattleData[1] = 0;
					return retNextBattleData;
				}
				if (leagueData.matchEnemy_[league_fightCombi_[i][1]] == 0)
				{
					retNextBattleData[0] = league_fightCombi_[i][0];
					retNextBattleData[1] = 1;
					return retNextBattleData;
				}
			}
			return null;
		}

		public int[] GetLeagueData(int teamId)
		{
			LeagueData leagueData = apdat_.leagueData_[selectLeagueId_];
			for (int i = 0; i < leagueData.matchEnemy_.Length; i++)
			{
				if (leagueData.matchEnemy_[i] == teamId)
				{
					return league_data_[i];
				}
			}
			return null;
		}

		public int GetLeagueDataIndex(int teamId)
		{
			LeagueData leagueData = apdat_.leagueData_[selectLeagueId_];
			for (int i = 0; i < leagueData.matchEnemy_.Length; i++)
			{
				if (leagueData.matchEnemy_[i] == teamId)
				{
					return i;
				}
			}
			return -1;
		}

		public bool League_update()
		{
			Update_league();
			league_setu_++;
			if (league_setu_ == league_teamNum_ - 1)
			{
				League_finish();
				return true;
			}
			return false;
		}

		public void League_resetScore()
		{
			for (int i = 0; i < memberPDataist_.Size(); i++)
			{
				PlayerData playerData = (PlayerData)memberPDataist_.ElementAt(i);
				playerData.game_goalInLeague_ = 0;
			}
			for (int j = 0; j < apdat_.tData_.Length; j++)
			{
				TeamData teamData = apdat_.tData_[j];
				for (int k = 0; k < teamData.goalScore_.Length; k++)
				{
					teamData.goalScore_[k] = 0;
				}
			}
		}

		public void League_finish()
		{
			LeagueData leagueData = apdat_.leagueData_[selectLeagueId_];
			if (league_setu_ == league_teamNum_ - 1) {

			League_resetScore();
			int[] leagueData2 = GetLeagueData(0);
			if (leagueData2[7] == 1)
			{
				if (leagueData.trophy_ != 4)
				{
					apdat_.ExecuteScript(leagueData.execsScript_, true, null, null, 0, 0, -1);
				}
				else
				{
					apdat_.ExecuteScript(leagueData.execsScript2_, true, null, null, 0, 0, -1);
				}
				if (Config.DEBUG_CMD)
				{
					Log.Info("■UserData.java league_finish() league_data.trophy_ = " + leagueData.trophy_);
				}
				FanBonusData fanBonusData = null;
				if (leagueData.id_ == 0)
				{
					fanBonusData = apdat_.fanBonus_[FanBonusData.FB_LEAGUE_AMA];
				}
				else if (leagueData.id_ == 1)
				{
					fanBonusData = apdat_.fanBonus_[FanBonusData.FB_LEAGUE_REGI];
				}
				else if (leagueData.id_ == 2)
				{
					fanBonusData = apdat_.fanBonus_[FanBonusData.FB_LEAGUE_ALL];
				}
				else if (leagueData.id_ == 3)
				{
					fanBonusData = apdat_.fanBonus_[FanBonusData.FB_LEAGUE_WORLD];
				}
				else if (leagueData.id_ == 4)
				{
					fanBonusData = apdat_.fanBonus_[FanBonusData.FB_LEAGUE_KAIRO];
				}
				if (fanBonusData != null && fanBonusData.state_ == 0)
				{
					fanBonusData.ChangeState(1);
					fanBonusData.achiveDay_[0] = apdat_.save_.ints[1];
					fanBonusData.achiveDay_[1] = apdat_.save_.ints[2];
					fanBonusData.achiveDay_[2] = apdat_.save_.ints[3];
				}
			}
			else
			{
				apdat_.ExecuteScript(leagueData.execsScript3_, true, leagueData2[7] + string.Empty, null, 0, 0, -1);
			}
			leagueData.SetGetTrophy(1);
			if (leagueData2[7] >= 1 && leagueData2[7] <= 3)
			{
				if (leagueData2[7] == 1)
				{
					leagueData.SetGetTrophy(4);
				}
				else if (leagueData2[7] == 2)
				{
					leagueData.SetGetTrophy(3);
				}
				else if (leagueData2[7] == 3)
				{
					leagueData.SetGetTrophy(2);
				}
			}
			if (Config.DEBUG_CMD)
			{
				Log.Info("■UserData.java league_finish() 最終順位 = " + leagueData2[7]);
			}
			leagueData.userPriOlder_ = leagueData2[7];
			for (int i = 0; i < league_data_.Length; i++)
			{
				int num = league_data_[i][8];
				int num2 = league_data_[i][7];
				leagueData.matchEnemyNext_[num2 - 1] = num;
			}
			SetMatchInfoJoin(2);
}
		}

		public void Qm_update()
		{
			battleQmIndex_++;
			QuestMatchData questMatchData = apdat_.qmData_[selectQmId_];
			questMatchData.SetGetTrophy(1);
			if (gm_.score_[0] > gm_.score_[1])
			{
				if (battleQmIndex_ == questMatchData.matchEnemy_.Length)
				{
					selectQmId_ = -1;
					apdat_.frmGame_.ChangeState(0);
					if (gm_.score_[0] > gm_.score_[1])
					{
						if (questMatchData.trophy_ != 4)
						{
							apdat_.ExecuteScript(questMatchData.execsScript_, true, null, null, 0, 0, -1);
						}
						else
						{
							apdat_.ExecuteScript(questMatchData.execsScript2_, true, null, null, 0, 0, -1);
						}
						questMatchData.SetGetTrophy(4);
					}
					SetMatchInfoJoin(2);
				}
				else
				{
					apdat_.ExecEvent(134, questMatchData.name_, null);
				}
			}
			else if (gm_.score_[0] == gm_.score_[1])
			{
				apdat_.ExecEvent(135, null, null);
				SetMatchInfoJoin(2);
			}
			else
			{
				if (!apdat_.CheckAlreadyEvent(136))
				{
					apdat_.ExecEvent(136, null, null);
				}
				else
				{
					apdat_.ExecEvent(137, null, null);
				}
				SetMatchInfoJoin(2);
			}
		}

		public void Update_league()
		{
			LeagueData leagueData = apdat_.leagueData_[selectLeagueId_];
			SetFightCombi(league_setu_);
			int num;
int num2;
for (int i = 0; i < league_fightCombi_.Length; i++)
			{
				num = league_fightCombi_[i][0];
				num2 = league_fightCombi_[i][1];
				int num3 = leagueData.matchEnemy_[num];
				int num4 = leagueData.matchEnemy_[num2];
				TeamData teamData = apdat_.tData_[num3];
				TeamData teamData2 = apdat_.tData_[num4];
				int num5 = GetTeamScore(teamData.war_offence_, teamData2.war_deffence_);
				int num6 = GetTeamScore(teamData2.war_offence_, teamData.war_deffence_);
				if (leagueData.matchEnemy_[num] == 0 || leagueData.matchEnemy_[num2] == 0)
				{
					if (leagueData.matchEnemy_[num] == 0)
					{
						num5 = gm_.score_[0];
						num6 = gm_.score_[1];
					}
					else
					{
						num5 = gm_.score_[1];
						num6 = gm_.score_[0];
					}
				}
				league_fightResult_[i][0] = num5;
				league_fightResult_[i][1] = num6;
				league_data_[num][4] += num5;
				league_data_[num][5] += num6;
				league_data_[num][6] = league_data_[num][4] - league_data_[num][5];
				league_data_[num2][4] += num6;
				league_data_[num2][5] += num5;
				league_data_[num2][6] = league_data_[num2][4] - league_data_[num2][5];
				if (num5 > num6)
				{
					League_setFightResult(0, num);
					League_setFightResult(2, num2);
				}
				else if (num5 == num6)
				{
					League_setFightResult(1, num);
					League_setFightResult(1, num2);
				}
				else
				{
					League_setFightResult(0, num2);
					League_setFightResult(2, num);
				}
				if (num3 == 0)
				{
					teamData.SetScore(num5, true);
				}
				else
				{
					teamData.SetScore(num5, false);
				}
				if (num4 == 0)
				{
					teamData2.SetScore(num6, true);
				}
				else
				{
					teamData2.SetScore(num6, false);
				}
			}
			for (int j = 0; j < league_dispOlder_.Length; j++)
			{
				league_dispOlder_[j][0] = leagueData.matchEnemy_[j];
				league_dispOlder_[j][1] = league_data_[j][0];
				league_dispOlder_[j][2] = league_data_[j][6];
			}
			for (int k = 0; k < league_dispOlder_.Length - 1; k++)
			{
				for (int num7 = league_dispOlder_.Length - 1; num7 > k; num7--)
				{
					if (league_dispOlder_[num7][1] > league_dispOlder_[k][1] || (league_dispOlder_[num7][1] == league_dispOlder_[k][1] && league_dispOlder_[num7][2] > league_dispOlder_[k][2]))
					{
						int num8 = league_dispOlder_[k][0];
						league_dispOlder_[k][0] = league_dispOlder_[num7][0];
						league_dispOlder_[num7][0] = num8;
						num8 = league_dispOlder_[k][1];
						league_dispOlder_[k][1] = league_dispOlder_[num7][1];
						league_dispOlder_[num7][1] = num8;
						num8 = league_dispOlder_[k][2];
						league_dispOlder_[k][2] = league_dispOlder_[num7][2];
						league_dispOlder_[num7][2] = num8;
					}
				}
			}
			for (int l = 0; l < league_dispOlder_.Length; l++)
			{
				int teamId = league_dispOlder_[l][0];
				int[] leagueData2 = GetLeagueData(teamId);
				leagueData2[7] = l + 1;
			}
		}

		public void League_setFightResult(int league_result, int team_id)
		{
			if ((league_result == 0)) { league_data_[team_id][0] += 3;
league_data_[team_id][1]++; } else if ((league_result == 1)) { league_data_[team_id][0]++;
league_data_[team_id][2]++; } else if ((league_result == 2)) { league_data_[team_id][3]++; }
		}

		public int GetTeamScore(int t1_offence, int t2_deffence)
		{
			int num = t1_offence - t2_deffence;
			int num2 = 0;
			for (int i = 0; i < temp_param.Length; i++)
			{
				if (num >= 0)
				{
					temp_param[i] = GameUtil.RateConvert(num, 0, 300, WARPSUB_BATTLESCORE_PLUS[i][0], WARPSUB_BATTLESCORE_PLUS[i][1], true);
				}
				else
				{
					temp_param[i] = GameUtil.RateConvert(num, -300, 0, WARPSUB_BATTLESCORE_MINUS[i][0], WARPSUB_BATTLESCORE_MINUS[i][1], true);
				}
			}
			num2 = GameUtil.Random(temp_param[1] - temp_param[0]) + temp_param[0];
			if (GameUtil.Random(100) < temp_param[2])
			{
				num2 /= 2;
			}
			if (GameUtil.Random(100) < temp_param[3])
			{
				num2 *= 2;
			}
			if (GameUtil.Random(100) < temp_param[4])
			{
				num2 += 100;
			}
			if (GameUtil.Random(100) < temp_param[5])
			{
				num2 -= 100;
			}
			num2 = AppData.ClampMin(num2 / 100, 0);
			if (num2 >= 8)
			{
				num2 = 7;
			}
			return num2;
		}

		public void Update_gameParam()
		{
			TeamData teamData = apdat_.tData_[gm_.teamId_[0]];
			TeamData teamData2 = apdat_.tData_[gm_.teamId_[1]];
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (gm_.score_[0] > gm_.score_[1])
			{
				flag = true;
			}
			else if (gm_.score_[0] == gm_.score_[1])
			{
				flag2 = true;
			}
			else
			{
				flag3 = true;
			}
			int matchInfoJoin = GetMatchInfoJoin();
			int num = teamData.warPotential_ - teamData2.warPotential_;
			int warPotential_ = teamData.warPotential_;
			int num2 = 0;
			num2 = (flag ? ((num >= 0) ? ((num2 + GameUtil.RateConvert(num, 0, 50, 5, 1, true))) : ((num2 + GameUtil.RateConvert(num, 0, -50, 5, 30, true)))) : ((!flag2) ? (num2 + GameUtil.RateConvert(num, -50, 50, 0, -6, true)) : (num2 + GameUtil.RateConvert(num, -50, 50, 6, -3, true))));
			teamData.AddTeamEva(num2);
			game_matchEva_ = GameUtil.RateConvert(teamData.warPotential_ - warPotential_, -20, 20, 0, 1000, true);
			if (flag)
			{
				game_winConsecutive_++;
			}
			else
			{
				game_winConsecutive_ = 0;
			}
			customer_total_ += apdat_.frmSoccer_.customerNum[0];
			customer_month_[apdat_.save_.ints[2]] += apdat_.frmSoccer_.customerNum[0];
			for (int i = 0; i < 11; i++)
			{
				PlayerData playerData = (PlayerData)memberPDataist_.ElementAt(i);
				playerData.matchNum_++;
				int num3 = 0;
				for (int j = 0; j < gm_.scorerListUserTeam.Size(); j++)
				{
					int num4 = ((JInteger)gm_.scorerListUserTeam.ElementAt(j)).IntValue();
					if (num4 == playerData.id_)
					{
						num3++;
					}
				}
				if (num3 > 0)
				{
					playerData.game_goalConsecutive_++;
				}
				else
				{
					playerData.game_goalConsecutive_ = 0;
				}
				if (matchInfoJoin == 0)
				{
					playerData.game_goalInLeague_ += num3;
				}
				playerData.game_goalTotal_ += num3;
			}
			if (flag)
			{
				apdat_.svisorData_[selectSupervisorId_].winTotal_++;
			}
			SponsorData.UpdateSatisFactPoint();
			if (flag)
			{
				AddTestPlayerParam(1, 1);
			}
			else if (flag2)
			{
				AddTestPlayerParam(2, 1);
			}
			else
			{
				AddTestPlayerParam(3, 1);
			}
			AddTPlayBattleRecode(gm_.teamId_[1], gm_.score_[0], gm_.score_[1]);
			RecordAddCustomerNum(apdat_.frmSoccer_.customerNum[0]);
		}

		public void ChangeEditPhase(int next_phase, bool isSoccerForm)
		{
			phase_ = next_phase;
			phaseCnt_ = 0;
			if (phase_ == 10)
			{
				SetMemberChangeList();
				editMemDispMode = 0;
			}
			if (phase_ == 0)
			{
				FormationData.UpdateStamemRole();
				if (!isSoccerForm)
				{
					FormBase topForm = formMan_.GetTopForm();
					topForm.SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[2]);
				}
				else
				{
					FormBase topForm2 = formMan_.GetTopForm();
					topForm2.SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[0]);
				}
			}
			else if (phase_ == 6)
			{
				FormBase topForm3 = formMan_.GetTopForm();
				topForm3.SetSoftLabel(AppData.softLabels_[6], AppData.softLabels_[2]);
				emdm_cnt = 0;
			}
			else if (phase_ == 7)
			{
				FormBase topForm4 = formMan_.GetTopForm();
				topForm4.SetSoftLabel(AppData.softLabels_[11], AppData.softLabels_[2]);
			}
			else if (phase_ == 1)
			{
				FormBase topForm5 = formMan_.GetTopForm();
				topForm5.SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[2]);
			}
			if (phase_ == 0)
			{
				FormationData elem = apdat_.formationData_[selectFormationId_];
				int num = editTest_formation.IndexOf(elem);
				if (num != -1)
				{
					et_cur[1] = num;
					int num2 = 3;
					if (scrollValue_ > et_cur[1] / 3)
					{
						scrollValue_ = et_cur[1] / 3;
					}
					if (scrollValue_ + num2 <= et_cur[1] / 3)
					{
						scrollValue_ = et_cur[1] / 3 - num2 + 1;
					}
				}
			}
			else if (phase_ == 9)
			{
				FormationData elem2 = apdat_.formationData_[selectFormationId_];
				int num3 = editTest_formation.IndexOf(elem2);
				if (num3 != -1)
				{
					et_cur[1] = num3;
					int num4 = 3;
					if (scrollValue_ > et_cur[1] / 3)
					{
						scrollValue_ = et_cur[1] / 3;
					}
					if (scrollValue_ + num4 <= et_cur[1] / 3)
					{
						scrollValue_ = et_cur[1] / 3 - num4 + 1;
					}
				}
			}
			if (phase_ == 1 && et_cur[1] < editTest_formation.Size())
			{
				FormationData formationData = (FormationData)editTest_formation.ElementAt(et_cur[1]);
				if (BitUtil.Check(formationData.flag_, 2))
				{
					FormBase topForm6 = formMan_.GetTopForm();
					topForm6.SetSoftLabel(AppData.softLabels_[12], AppData.softLabels_[2]);
				}
				else
				{
					FormBase topForm7 = formMan_.GetTopForm();
					topForm7.SetSoftLabel(AppData.softLabels_[13], AppData.softLabels_[2]);
				}
			}
		}

		public void SetMemberChangeList()
		{
			if (editTest_fw.Size() > 0)
			{
				editTest_fw.RemoveAllElements();
			}
			if (editTest_mf.Size() > 0)
			{
				editTest_mf.RemoveAllElements();
			}
			if (editTest_df.Size() > 0)
			{
				editTest_df.RemoveAllElements();
			}
			if (editTest_gk.Size() > 0)
			{
				editTest_gk.RemoveAllElements();
			}
			if (editTest_temp.Size() > 0)
			{
				editTest_temp.RemoveAllElements();
			}
			if (editTest_category.Size() > 0)
			{
				editTest_category.RemoveAllElements();
			}
			int[][] formation_ = apdat_.formationData_[selectFormationId_].formation_;
			editTest_gk.AddElement(new int[3]
			{
				formation_[0][0],
				formation_[0][1],
				0
			});
			for (int i = 1; i < formation_.Length; i++)
			{
				if (formation_[i][1] >= 6 && formation_[i][1] <= 8)
				{
					editTest_df.AddElement(new int[3]
					{
						formation_[i][0],
						formation_[i][1],
						i
					});
				}
				if (formation_[i][1] >= 3 && formation_[i][1] <= 5)
				{
					editTest_mf.AddElement(new int[3]
					{
						formation_[i][0],
						formation_[i][1],
						i
					});
				}
				if (formation_[i][1] >= 0 && formation_[i][1] <= 2)
				{
					editTest_fw.AddElement(new int[3]
					{
						formation_[i][0],
						formation_[i][1],
						i
					});
				}
			}
			if (editTest_df.Size() == 0)
			{
				editTest_df.AddElement(new int[3] { 0, 0, -1 });
			}
			if (editTest_mf.Size() == 0)
			{
				editTest_mf.AddElement(new int[3] { 0, 0, -1 });
			}
			if (editTest_fw.Size() == 0)
			{
				editTest_fw.AddElement(new int[3] { 0, 0, -1 });
			}
			int num = 4;
			for (int j = 0; j < editTest_df.Size() - 1; j++)
			{
				for (int num2 = editTest_df.Size() - 1; num2 > j; num2--)
				{
					int[] array = (int[])editTest_df.ElementAt(j);
					int[] array2 = (int[])editTest_df.ElementAt(num2);
					if (editTest_df.Size() <= num)
					{
						if (array2[0] < array[0] || (array2[0] == array[0] && array2[1] < array[1]))
						{
							editTest_df.SetElementAt(array2, j);
							editTest_df.SetElementAt(array, num2);
						}
					}
					else if (array2[1] < array[1] || (array2[1] == array[1] && array2[0] < array[0]))
					{
						editTest_df.SetElementAt(array2, j);
						editTest_df.SetElementAt(array, num2);
					}
				}
			}
			for (int k = 0; k < editTest_mf.Size() - 1; k++)
			{
				for (int num3 = editTest_mf.Size() - 1; num3 > k; num3--)
				{
					int[] array3 = (int[])editTest_mf.ElementAt(k);
					int[] array4 = (int[])editTest_mf.ElementAt(num3);
					if (editTest_mf.Size() <= num)
					{
						if (array4[0] < array3[0] || (array4[0] == array3[0] && array4[1] < array3[1]))
						{
							editTest_mf.SetElementAt(array4, k);
							editTest_mf.SetElementAt(array3, num3);
						}
					}
					else if (array4[1] < array3[1] || (array4[1] == array3[1] && array4[0] < array3[0]))
					{
						editTest_mf.SetElementAt(array4, k);
						editTest_mf.SetElementAt(array3, num3);
					}
				}
			}
			for (int l = 0; l < editTest_fw.Size() - 1; l++)
			{
				for (int num4 = editTest_fw.Size() - 1; num4 > l; num4--)
				{
					int[] array5 = (int[])editTest_fw.ElementAt(l);
					int[] array6 = (int[])editTest_fw.ElementAt(num4);
					if (editTest_fw.Size() <= num)
					{
						if (array6[0] < array5[0] || (array6[0] == array5[0] && array6[1] < array5[1]))
						{
							editTest_fw.SetElementAt(array6, l);
							editTest_fw.SetElementAt(array5, num4);
						}
					}
					else if (array6[1] < array5[1] || (array6[1] == array5[1] && array6[0] < array5[0]))
					{
						editTest_fw.SetElementAt(array6, l);
						editTest_fw.SetElementAt(array5, num4);
					}
				}
			}
			bool flag = true;
			int num5 = 4;
			if (editTest_df.Size() > num || editTest_mf.Size() > num || editTest_fw.Size() > num)
			{
				num5 = 5;
				flag = false;
			}
			editTest_team = new Vector[num5];
			int num6 = 0;
			editTest_category.AddElement(new JInteger(0));
			editTest_team[num6++] = editTest_gk;
			editTest_category.AddElement(new JInteger(1));
			if (editTest_df.Size() > num && !flag)
			{
				for (int m = 0; m < 3; m++)
				{
					for (int num7 = editTest_df.Size() - 1; num7 >= 0; num7--)
					{
						int[] array7 = (int[])editTest_df.ElementAt(num7);
						if (array7[1] == 8 - m)
						{
							editTest_temp.InsertElementAt(editTest_df.ElementAt(num7), 0);
							editTest_df.RemoveElementAt(num7);
						}
						if (editTest_temp.Size() == 5) {
							break;
						}
if (editTest_df.Size() == 1) {
							break;
						}
					}
					if (editTest_temp.Size() == 5) {
						break;
					}
if ((editTest_temp.Size() > 0 && editTest_df.Size() <= 5)) {
						break;
					}
				}
				editTest_category.AddElement(new JInteger(1));
				if (editTest_temp.Size() > 0)
				{
					editTest_team[num6++] = editTest_temp;
				}
				editTest_team[num6++] = editTest_df;
				flag = true;
			}
			else
			{
				editTest_team[num6++] = editTest_df;
			}
			editTest_category.AddElement(new JInteger(2));
			if (editTest_mf.Size() > num && !flag)
			{
				for (int n = 0; n < 3; n++)
				{
					for (int num8 = editTest_mf.Size() - 1; num8 >= 0; num8--)
					{
						int[] array8 = (int[])editTest_mf.ElementAt(num8);
						if (array8[1] == 5 - n)
						{
							editTest_temp.InsertElementAt(editTest_mf.ElementAt(num8), 0);
							editTest_mf.RemoveElementAt(num8);
						}
						if (editTest_temp.Size() == 5) {
							break;
						}
if (editTest_mf.Size() == 1) {
							break;
						}
					}
					if (editTest_temp.Size() == 5) {
						break;
					}
if ((editTest_temp.Size() > 0 && editTest_mf.Size() <= 5)) {
						break;
					}
				}
				editTest_category.AddElement(new JInteger(2));
				if (editTest_temp.Size() > 0)
				{
					editTest_team[num6++] = editTest_temp;
				}
				editTest_team[num6++] = editTest_mf;
				flag = true;
			}
			else
			{
				editTest_team[num6++] = editTest_mf;
			}
			editTest_category.AddElement(new JInteger(3));
			if (editTest_fw.Size() > num && !flag)
			{
				for (int num9 = 0; num9 < 3; num9++)
				{
					for (int num10 = editTest_fw.Size() - 1; num10 >= 0; num10--)
					{
						int[] array9 = (int[])editTest_fw.ElementAt(num10);
						if (array9[1] == 2 - num9)
						{
							editTest_temp.InsertElementAt(editTest_fw.ElementAt(num10), 0);
							editTest_fw.RemoveElementAt(num10);
						}
						if (editTest_temp.Size() == 5) {
							break;
						}
if (editTest_fw.Size() == 1) {
							break;
						}
					}
					if (editTest_temp.Size() == 5) {
						break;
					}
if ((editTest_temp.Size() > 0 && editTest_fw.Size() <= 5)) {
						break;
					}
				}
				editTest_category.AddElement(new JInteger(3));
				if (editTest_temp.Size() > 0)
				{
					editTest_team[num6++] = editTest_temp;
				}
				editTest_team[num6++] = editTest_fw;
				flag = true;
			}
			else
			{
				editTest_team[num6++] = editTest_fw;
			}
		}

		public bool Update_edit(bool isSoccerForm)
		{
			isSoccerForm_ = isSoccerForm;
			phaseCnt_ = (phaseCnt_ + 1) % int.MaxValue;
			if (phase_ == 0)
			{
				if (!isSoccerForm)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						{ bool recoveredCondition_68130 = et_cur[0] % 3 == 0;
if (recoveredCondition_68130)
						{
							et_cur[0] += 2;
						}
						else
						{
							et_cur[0] = (et_cur[0] - 1 + MENU_STR.Length) % MENU_STR.Length;
						}
}
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						if (et_cur[0] % 3 == 2)
						{
							et_cur[0] -= 2;
						}
						else
						{
							et_cur[0] = (et_cur[0] + 1 + MENU_STR.Length) % MENU_STR.Length;
						}
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						if (et_cur[0] == 4)
						{
							FormBase topForm = formMan_.GetTopForm();
							topForm.Push(new SubForm(53));
							return false;
						}
						if (et_cur[0] == 2)
						{
							if (!apdat_.CheckAlreadyEvent(163))
							{
								apdat_.ExecEvent(163, null, null);
							}
							apdat_.Push(new SubForm(27));
							return false;
						}
						if (et_cur[0] != 5)
						{
							if (et_cur[0] == 0)
							{
								if (!apdat_.CheckAlreadyEvent(161))
								{
									apdat_.ExecEvent(161, null, null);
								}
								ChangeEditPhase(9, isSoccerForm);
							}
							else if (et_cur[0] == 1)
							{
								if (!apdat_.CheckAlreadyEvent(162))
								{
									apdat_.ExecEvent(162, null, null);
								}
								ChangeEditPhase(10, isSoccerForm);
							}
							else if (et_cur[0] == 3)
							{
								FormBase topForm2 = formMan_.GetTopForm();
								topForm2.Push(new SubForm(50));
								return false;
							}
						}
					}
					else if (apdat_.IsPushSoftLabel(2))
					{
						FormationData.RemoveNew();
						return true;
					}
				}
				else
				{
					if (et_cur[0] == 0)
					{
						et_cur[0] = 1;
					}
					if (canvas_.CheckKeyRepeat(131072) && et_cur[0] >= 2)
					{
						et_cur[0] = 1;
					}
					if (canvas_.CheckKeyRepeat(524288))
					{
						if (et_cur[0] == 0)
						{
							et_cur[0] = 1;
						}
						else if (et_cur[0] == 1)
						{
							et_cur[0] = 2;
						}
					}
					if (canvas_.CheckKeyRepeat(65536) && et_cur[0] >= 3)
					{
						et_cur[0]--;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						if (et_cur[0] == 1)
						{
							ChangeEditPhase(15, isSoccerForm);
						}
						else if (et_cur[0] == 2 || et_cur[0] == 3)
						{
							et_cur[0]++;
						}
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						if (et_cur[0] == 0)
						{
							ChangeEditPhase(15, isSoccerForm);
						}
						else
						{
							if (et_cur[0] == 1)
							{
								FormationData.RemoveNew();
								return true;
							}
							if (et_cur[0] == 2)
							{
								ChangeEditPhase(9, isSoccerForm);
							}
							else if (et_cur[0] == 3)
							{
								ChangeEditPhase(10, isSoccerForm);
							}
							else if (et_cur[0] == 4)
							{
								FormBase topForm3 = formMan_.GetTopForm();
								topForm3.Push(new SubForm(27));
								return false;
							}
						}
					}
				}
			}
			else if (phase_ == 1)
			{
				int num = editTest_formation.Size();
				int num2 = et_cur[1];
				if (canvas_.CheckKeyRepeat(131072) && et_cur[1] / 3 > 0)
				{
					et_cur[1] -= 3;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					int num3 = et_cur[1] / 3;
					int num4 = et_cur[1];
					et_cur[1] += 3;
					if (et_cur[1] >= num)
					{
						et_cur[1] = num - 1;
					}
					if (et_cur[1] / 3 == num3)
					{
						et_cur[1] = num4;
					}
				}
				if (canvas_.CheckKeyRepeat(65536) && et_cur[1] % 3 != 0)
				{
					et_cur[1]--;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					et_cur[1]++;
					if (et_cur[1] >= num)
					{
						et_cur[1] = num - 1;
					}
				}
				int num5 = 3;
				if (scrollValue_ > et_cur[1] / 3)
				{
					scrollValue_ = et_cur[1] / 3;
				}
				if (scrollValue_ + num5 <= et_cur[1] / 3)
				{
					scrollValue_ = et_cur[1] / 3 - num5 + 1;
				}
				if (num2 != et_cur[1])
				{
					SetEditForPos(isSoccerForm);
					FormationData formationData = (FormationData)editTest_formation.ElementAt(et_cur[1]);
					if (BitUtil.Check(formationData.flag_, 2))
					{
						FormBase topForm4 = formMan_.GetTopForm();
						topForm4.SetSoftLabel(AppData.softLabels_[12], AppData.softLabels_[2]);
					}
					else
					{
						FormBase topForm5 = formMan_.GetTopForm();
						topForm5.SetSoftLabel(AppData.softLabels_[13], AppData.softLabels_[2]);
					}
				}
				if (canvas_.CheckKeyPulse(1048576) || apdat_.IsPushSoftLabel(13))
				{
					ChangeEditPhase(5, isSoccerForm);
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					ChangeEditPhase(11, isSoccerForm);
				}
				else if (apdat_.IsPushSoftLabel(12))
				{
					FormationData formationData2 = (FormationData)editTest_formation.ElementAt(et_cur[1]);
					selectFormationId_ = formationData2.id_;
					ChangeEditPhase(2, isSoccerForm);
				}
			}
			else if (phase_ == 2)
			{
				if (canvas_.CheckKeyRepeat(131072) && et_cur[2] / 10 > 0)
				{
					et_cur[2] -= 10;
				}
				if (canvas_.CheckKeyRepeat(524288) && et_cur[2] + 10 < 90)
				{
					et_cur[2] += 10;
				}
				if (canvas_.CheckKeyRepeat(65536) && et_cur[2] % 10 != 8)
				{
					et_cur[2]++;
				}
				if (canvas_.CheckKeyRepeat(262144) && et_cur[2] % 10 != 0)
				{
					et_cur[2]--;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					et_cur[3] = et_cur[2];
					ChangeEditPhase(3, isSoccerForm);
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					ChangeEditPhase(1, isSoccerForm);
				}
			}
			else if (phase_ == 3)
			{
				if (canvas_.CheckKeyRepeat(131072) && et_cur[3] / 10 > 0)
				{
					et_cur[3] -= 10;
				}
				if (canvas_.CheckKeyRepeat(524288) && et_cur[3] + 10 < 90)
				{
					et_cur[3] += 10;
				}
				if (canvas_.CheckKeyRepeat(65536) && et_cur[3] % 10 != 8)
				{
					et_cur[3]++;
				}
				if (canvas_.CheckKeyRepeat(262144) && et_cur[3] % 10 != 0)
				{
					et_cur[3]--;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					FormationData formationData3 = (FormationData)editTest_formation.ElementAt(et_cur[1]);
					int[][] formation_ = formationData3.formation_;
					int[] array = null;
					int[] array2 = null;
					for (int i = 1; i < formation_.Length; i++)
					{
						if (formation_[i][0] == et_cur[2] / 10 && formation_[i][1] == et_cur[2] % 10)
						{
							array = formation_[i];
						}
						if (formation_[i][0] == et_cur[3] / 10 && formation_[i][1] == et_cur[3] % 10)
						{
							array2 = formation_[i];
						}
					}
					if (array != null || array2 != null)
					{
						ChangeEditPhase(4, isSoccerForm);
					}
					else
					{
						et_cur[2] = et_cur[3];
						ChangeEditPhase(2, isSoccerForm);
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					et_cur[2] = et_cur[3];
					ChangeEditPhase(2, isSoccerForm);
				}
			}
			else if (phase_ == 4)
			{
				SetEditForPos(isSoccerForm);
				if (phaseCnt_ >= 6)
				{
					FormationData formationData4 = (FormationData)editTest_formation.ElementAt(et_cur[1]);
					int[][] formation_2 = formationData4.formation_;
					int[] array3 = null;
					int[] array4 = null;
					for (int j = 1; j < formation_2.Length; j++)
					{
						if (formation_2[j][0] == et_cur[2] / 10 && formation_2[j][1] == et_cur[2] % 10)
						{
							array3 = formation_2[j];
						}
						if (formation_2[j][0] == et_cur[3] / 10 && formation_2[j][1] == et_cur[3] % 10)
						{
							array4 = formation_2[j];
						}
					}
					if (array3 != null && array4 != null)
					{
						int num6 = array3[0];
						array3[0] = array4[0];
						array4[0] = num6;
						num6 = array3[1];
						array3[1] = array4[1];
						array4[1] = num6;
					}
					else if (array3 != null)
					{
						array3[0] = et_cur[3] / 10;
						array3[1] = et_cur[3] % 10;
					}
					else if (array4 != null)
					{
						array4[0] = et_cur[2] / 10;
						array4[1] = et_cur[2] % 10;
					}
					et_cur[2] = et_cur[3];
					ChangeEditPhase(2, isSoccerForm);
				}
			}
			else if (phase_ == 5)
			{
				int num7 = 3;
				if (canvas_.CheckKeyRepeat(131072))
				{
					et_cur[4] = (et_cur[4] - 1 + 3) % 3;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					et_cur[4] = (et_cur[4] + 1 + 3) % 3;
				}
				if (canvas_.CheckKeyPulse(1048576) || apdat_.IsPushSoftLabel(13))
				{
					FormationData formationData5 = (FormationData)editTest_formation.ElementAt(et_cur[1]);
					selectFormationId_ = formationData5.id_;
					selectFormationType_ = et_cur[4];
					SetEditForPos(isSoccerForm);
					ChangeEditPhase(12, isSoccerForm);
					apdat_.ExecEvent(70 + selectFormationType_, formationData5.name_ + "\t" + FORMATIONTYPE_STR[selectFormationType_], null);
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					ChangeEditPhase(1, isSoccerForm);
				}
			}
			else if (phase_ == 6)
			{
				emdm_cnt++;
				FormBase topForm6 = formMan_.GetTopForm();
				if (topForm6.GetLeftSoftLabel() == "ベンチ")
				{
					topForm6.SetSoftLabel(AppData.softLabels_[6], AppData.softLabels_[2]);
				}
				int num8 = et_cur[5] / 10;
				if (num8 >= editTest_team.Length)
				{
					num8 = editTest_team.Length - 1;
				}
				et_cur[5] = et_cur[5] % 10 + num8 * 10;
				num8 = et_cur[5] % 10;
				if (num8 >= editTest_team[et_cur[5] / 10].Size())
				{
					num8 = editTest_team[et_cur[5] / 10].Size() - 1;
					et_cur[5] = et_cur[5] / 10 * 10 + num8;
				}
				if (canvas_.CheckKeyRepeat(131072))
				{
					int num9 = et_cur[5] / 10;
					et_cur[5] = et_cur[5] % 10;
					et_cur[5] = (et_cur[5] - 1 + editTest_team[num9].Size()) % editTest_team[num9].Size();
					et_cur[5] += num9 * 10;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					int num10 = et_cur[5] / 10;
					et_cur[5] = et_cur[5] % 10;
					et_cur[5] = (et_cur[5] + 1 + editTest_team[num10].Size()) % editTest_team[num10].Size();
					et_cur[5] += num10 * 10;
				}
				if (canvas_.CheckKeyRepeat(65536))
				{
					int num11 = et_cur[5] % 10;
					et_cur[5] = et_cur[5] / 10;
					et_cur[5] = (et_cur[5] - 1 + editTest_team.Length) % editTest_team.Length;
					et_cur[5] = et_cur[5] * 10;
					if (num11 >= editTest_team[et_cur[5] / 10].Size())
					{
						num11 = editTest_team[et_cur[5] / 10].Size() - 1;
					}
					et_cur[5] += num11;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					int num12 = et_cur[5] % 10;
					et_cur[5] = et_cur[5] / 10;
					et_cur[5] = (et_cur[5] + 1 + editTest_team.Length) % editTest_team.Length;
					et_cur[5] = et_cur[5] * 10;
					if (num12 >= editTest_team[et_cur[5] / 10].Size())
					{
						num12 = editTest_team[et_cur[5] / 10].Size() - 1;
					}
					et_cur[5] += num12;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					int[] array5 = (int[])editTest_team[et_cur[5] / 10].ElementAt(et_cur[5] % 10);
					if (array5[2] != -1)
					{
						ChangeEditPhase(7, isSoccerForm);
						et_cur[6] = et_cur[5];
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					ChangeEditPhase(13, isSoccerForm);
				}
				else if (apdat_.IsPushSoftLabel(6))
				{
					editMemDispMode = (editMemDispMode + 1) % 5;
					emdm_cnt = 0;
				}
			}
			else if (phase_ == 7)
			{
				if (canvas_.CheckKeyRepeat(131072))
				{
					int num13 = et_cur[6] / 10;
					et_cur[6] = et_cur[6] % 10;
					et_cur[6] = (et_cur[6] - 1 + editTest_team[num13].Size()) % editTest_team[num13].Size();
					et_cur[6] += num13 * 10;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					int num14 = et_cur[6] / 10;
					et_cur[6] = et_cur[6] % 10;
					et_cur[6] = (et_cur[6] + 1 + editTest_team[num14].Size()) % editTest_team[num14].Size();
					et_cur[6] += num14 * 10;
				}
				if (canvas_.CheckKeyRepeat(65536))
				{
					int num15 = et_cur[6] % 10;
					et_cur[6] = et_cur[6] / 10;
					et_cur[6] = (et_cur[6] - 1 + editTest_team.Length) % editTest_team.Length;
					et_cur[6] = et_cur[6] * 10;
					if (num15 >= editTest_team[et_cur[6] / 10].Size())
					{
						num15 = editTest_team[et_cur[6] / 10].Size() - 1;
					}
					et_cur[6] += num15;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					int num16 = et_cur[6] % 10;
					et_cur[6] = et_cur[6] / 10;
					et_cur[6] = (et_cur[6] + 1 + editTest_team.Length) % editTest_team.Length;
					et_cur[6] = et_cur[6] * 10;
					if (num16 >= editTest_team[et_cur[6] / 10].Size())
					{
						num16 = editTest_team[et_cur[6] / 10].Size() - 1;
					}
					et_cur[6] += num16;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					int[] array6 = (int[])editTest_team[et_cur[6] / 10].ElementAt(et_cur[6] % 10);
					if (array6[2] != -1)
					{
						ChangeEditPhase(8, isSoccerForm);
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					ChangeEditPhase(6, isSoccerForm);
				}
				else if (apdat_.IsPushSoftLabel(11))
				{
					FormBase topForm7 = formMan_.GetTopForm();
					SubForm subForm = new SubForm(28);
					int num17 = et_cur[5] / 10;
					int index = et_cur[5] % 10;
					Vector vector = editTest_team[num17];
					int[] array7 = (int[])vector.ElementAt(index);
					if (array7[2] != -1)
					{
						int index2 = array7[2];
						PlayerData playerData_ = (PlayerData)memberPDataist_.ElementAt(index2);
						subForm.playerData_ = playerData_;
					}
					topForm7.Push(subForm);
				}
			}
			else if (phase_ == 8)
			{
				SetEditForPos(isSoccerForm);
				if (phaseCnt_ >= 6)
				{
					int[] array8 = (int[])editTest_team[et_cur[6] / 10].ElementAt(et_cur[6] % 10);
					if (array8[2] != -1)
					{
						int[] array9 = (int[])editTest_team[et_cur[6] / 10].ElementAt(et_cur[6] % 10);
						int index3 = array9[2];
						array9 = (int[])editTest_team[et_cur[5] / 10].ElementAt(et_cur[5] % 10);
						int index4 = array9[2];
						PlayerData obj = (PlayerData)memberPDataist_.ElementAt(index4);
						memberPDataist_.SetElementAt(memberPDataist_.ElementAt(index3), index4);
						memberPDataist_.SetElementAt(obj, index3);
						SetMemberChangeList();
						ChangeEditPhase(6, isSoccerForm);
						SetEditForPos(isSoccerForm);
						et_cur[5] = et_cur[6];
					}
				}
			}
			else if (phase_ == 14)
			{
				if (canvas_.CheckKeyRepeat(65536))
				{
					ChangeEditPhase(16, isSoccerForm);
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					ChangeEditPhase(16, isSoccerForm);
				}
			}
			if (phase_ == 9 || phase_ == 10 || phase_ == 11 || phase_ == 12 || phase_ == 13 || phase_ == 15 || phase_ == 16)
			{
				SetEditForPos(isSoccerForm);
				if (phaseCnt_ > 8)
				{
					if (phase_ == 9)
					{
						ChangeEditPhase(1, isSoccerForm);
					}
					if (phase_ == 10)
					{
						ChangeEditPhase(6, isSoccerForm);
					}
					if (phase_ == 11)
					{
						ChangeEditPhase(0, isSoccerForm);
					}
					if (phase_ == 12)
					{
						ChangeEditPhase(0, isSoccerForm);
					}
					if (phase_ == 13)
					{
						ChangeEditPhase(0, isSoccerForm);
					}
					if (phase_ == 15)
					{
						ChangeEditPhase(14, isSoccerForm);
					}
					if (phase_ == 16)
					{
						ChangeEditPhase(0, isSoccerForm);
					}
				}
			}
			return false;
		}

		public void Draw_edit(Graphics g, bool isSoccerForm)
		{
			TeamData teamData = apdat_.tData_[gm_.teamId_[0]];
			int num2;
			int num3;
			int num = (num2 = (num3 = 0));
			int num4 = 0;
			if (isSoccerForm)
			{
				num4 = SoccerForm.VIEW_Y;
				apdat_.DrawWindow2(g, 240, 240 - SoccerForm.VIEW_Y + 2 + 2, 9, num4);
			}
			else
			{
				num4 = GameForm.VIEW_Y;
				apdat_.DrawWindow2(g, 240, 240 - GameForm.VIEW_Y, 11, num4);
			}
			g.ClipRect(4f, num4, 232f, 236 - num4);
			if (phase_ == 0 || phase_ == 9 || phase_ == 10 || phase_ == 11 || phase_ == 12 || phase_ == 13 || phase_ == 14 || phase_ == 15 || phase_ == 16)
			{
				int num5 = 64;
				int num6 = 8;
				int num7 = 20;
				int num8 = 2;
				if (!isSoccerForm)
				{
					int num9 = EDIT_POS_MENU[0][0];
					num = EDIT_POS_MENU[0][1];
					if (phase_ == 9 || phase_ == 10)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_MENU[0][1], EDIT_POS_MENU[1][1], true);
					}
					else if (phase_ == 11 || phase_ == 12 || phase_ == 13)
					{
						num = GameUtil.RateConvert(phaseCnt_, 4, 8, EDIT_POS_MENU[1][1], EDIT_POS_MENU[0][1], true);
					}
					for (int i = 0; i < MENU_STR.Length; i++)
					{
						num2 = num9 + (num5 + num6) * (i % 3);
						num3 = num + (num7 + num8) * (i / 3);
						if (i == et_cur[0])
						{
							Draw_editButton(g, num2, num3, num5, num7, true, 0, i, true, false);
						}
						else
						{
							Draw_editButton(g, num2, num3, num5, num7, false, 0, i, true, false);
						}
						g.SetColor(SubForm.SC_WINDOW_BLACK);
						SubForm.SetFontSize(g, 10);
						if (Language.English())
						{
							g.DrawString(MENU_STR[i], num2 + num5 / 2, num3 + 4 + 1, 2);
						}
						else
						{
							g.DrawString(MENU_STR[i], num2 + num5 / 2, num3 + 4, 2);
						}
						SubForm.ResetFontSize(g);
						if (i == 0 && FormationData.IsExistNew())
						{
							SubForm.Draw_newMark(g, num2 + 20, num3 - 5, phaseCnt_);
						}
						else if (i == 2 && PlayerData.IsExistNew())
						{
							SubForm.Draw_newMark(g, num2 + 20, num3 - 5, phaseCnt_);
						}
						if (i == et_cur[0])
						{
							apdat_.resCom2_.seb[60].Draw(g, num2 - 1, num3 + 10, apdat_.resCom2_.img);
						}
					}
				}
				else
				{
					int num9 = EDIT_POS_MENU_SOC[0][0];
					num = EDIT_POS_MENU_SOC[0][1];
					if (phase_ == 9 || phase_ == 10)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_MENU_SOC[0][1], EDIT_POS_MENU_SOC[1][1], true);
					}
					else if (phase_ == 11 || phase_ == 12 || phase_ == 13)
					{
						num = GameUtil.RateConvert(phaseCnt_, 4, 8, EDIT_POS_MENU_SOC[1][1], EDIT_POS_MENU_SOC[0][1], true);
					}
					else if (phase_ == 14)
					{
						num9 = EDIT_POS_MENU_ENE[1][0];
					}
					else if (phase_ == 15)
					{
						num9 = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_MENU_ENE[0][0], EDIT_POS_MENU_ENE[1][0], true);
					}
					else if (phase_ == 16)
					{
						num9 = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_MENU_ENE[1][0], EDIT_POS_MENU_ENE[0][0], true);
					}
					num2 = num9 + 26;
					num3 = num;
					if (et_cur[0] == 0)
					{
						g.SetColor(SubForm.SC_WINDOW_SELECT);
						g.FillRect(num2, num3 + 2, 164f, 17f);
						apdat_.resCom2_.seb[60].Draw(g, num2 - 1, num3 + 10, apdat_.resCom2_.img);
					}
					TeamData teamData2 = apdat_.tData_[gm_.teamId_[1]];
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					g.DrawString(teamData.name_, num2 + 81, num3 + 4, 2);
					if (phase_ == 0)
					{
						if (et_cur[0] == 1)
						{
							GameView.AddTouch(g, 1, num2 + 171 + phaseCnt_ % 16 / 8 * 4, num3 + 10 + 26, apdat_.resCom2_.seb[24], 1, 18);
						}
						else
						{
							GameView.AddTouch(g, 26, num2 + 171 + phaseCnt_ % 16 / 8 * 4, num3 + 10 + 26, apdat_.resCom2_.seb[24], 1, 786433, TouchOption.Create().ExtendValues(65554));
						}
					}
					num2 = num9 + 26;
					num3 = num + 26;
					if (et_cur[0] == 1)
					{
						Draw_editButton(g, num2, num3, 164, 21, true, 0, 1, true, false);
						apdat_.resCom2_.seb[60].Draw(g, num2 - 1, num3 + 10, apdat_.resCom2_.img);
					}
					else
					{
						Draw_editButton(g, num2, num3, 164, 21, false, 0, 1, true, false);
					}
					g.SetColor(Graphics.GetColorOfRGB(0, 60, 183));
					if (!gm_.halfTime)
					{
						g.DrawString("前半キックオフ", num2 + 81, num3 + 4, 2);
					}
					else
					{
						g.DrawString("後半キックオフ", num2 + 81, num3 + 4, 2);
					}
					apdat_.DrawSeb(g, num2 + 19, num3 + 16, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 0);
					apdat_.DrawSeb(g, num2 + 144, num3 + 16, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 0);
					num3 = num + 52;
					for (int j = 0; j < 3; j++)
					{
						num2 = num9 + (num5 + num6) * (j % 3);
						if (j + 2 == et_cur[0])
						{
							Draw_editButton(g, num2, num3, num5, num7, true, 0, j + 2, true, false);
						}
						else
						{
							Draw_editButton(g, num2, num3, num5, num7, false, 0, j + 2, true, false);
						}
						g.SetColor(SubForm.SC_WINDOW_BLACK);
						SubForm.SetFontSize(g, 10);
						if (Language.English())
						{
							g.DrawString(MENU_STR[j], num2 + num5 / 2, num3 + 4 + 1, 2);
						}
						else
						{
							g.DrawString(MENU_STR[j], num2 + num5 / 2, num3 + 4, 2);
						}
						SubForm.ResetFontSize(g);
						if (j == 0 && FormationData.IsExistNew())
						{
							SubForm.Draw_newMark(g, num2 + 20, num3 - 5, phaseCnt_);
						}
						else if (j == 2 && PlayerData.IsExistNew())
						{
							SubForm.Draw_newMark(g, num2 + 20, num3 - 5, phaseCnt_);
						}
						if (j + 2 == et_cur[0])
						{
							apdat_.resCom2_.seb[60].Draw(g, num2 - 1, num3 + 10, apdat_.resCom2_.img);
						}
					}
				}
				if (phase_ == 14)
				{
					GameView.AddTouch(2, TouchOption.Create(2));
				}
			}
			if (phase_ == 0 || phase_ == 1 || phase_ == 9 || phase_ == 10 || phase_ == 11 || phase_ == 12 || phase_ == 13 || phase_ == 2 || phase_ == 3 || phase_ == 4 || phase_ == 5 || phase_ == 14 || phase_ == 16 || phase_ == 15)
			{
				int num9;
				if (!isSoccerForm)
				{
					num9 = EDIT_POS_TOPHELP_F[0][0];
					num = EDIT_POS_TOPHELP_F[0][1];
					if (phase_ == 1 || phase_ == 2 || phase_ == 3 || phase_ == 4 || phase_ == 5)
					{
						num9 = EDIT_POS_TOPHELP_F[1][0];
						num = EDIT_POS_TOPHELP_F[1][1];
					}
					else if (phase_ == 9)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_TOPHELP_F[0][1], EDIT_POS_TOPHELP_F[1][1], true);
					}
					else if (phase_ == 11 || phase_ == 12)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_TOPHELP_F[1][1], EDIT_POS_TOPHELP_F[0][1], true);
					}
					else if (phase_ == 10)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_TOPHELP_M[0][1], EDIT_POS_TOPHELP_M[1][1], true);
					}
					else if (phase_ == 13)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_TOPHELP_M[1][1], EDIT_POS_TOPHELP_M[0][1], true);
					}
				}
				else
				{
					num9 = EDIT_POS_TOPHELP_F[1][0];
					num = EDIT_POS_TOPHELP_F[1][1];
					if (phase_ == 10)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_TOPHELP_F[1][1], EDIT_POS_TOPHELP_M[1][1], true);
					}
					else if (phase_ == 13)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_TOPHELP_M[1][1], EDIT_POS_TOPHELP_F[1][1], true);
					}
				}
				apdat_.DrawBox(g, num9 - 2, num - 11, num9 - 2 + 213, num - 11 + 22);
				g.DrawImage(apdat_.resCom2_.img[172], num9 + 5, num, 0, 0, 16, 21);
				g.SetColor(SubForm.SC_WINDOW_BLACK);
				if (phase_ == 1 || phase_ == 2 || phase_ == 3 || phase_ == 4 || phase_ == 5 || phase_ == 12)
				{
					if (phase_ == 5 || phase_ == 12)
					{
						string empty = string.Empty;
						empty = ((et_cur[4] == 0) ? "ﾊﾞﾗﾝｽ重視で試合に臨みます。" : ((et_cur[4] != 1) ? "下がり目でﾃﾞｨﾌｪﾝｽします。" : "攻める意識がやや高くなります。"));
						g.DrawString2(empty, num9 + 28, num + 6);
					}
					else if (phase_ == 1)
					{
						FormationData formationData = (FormationData)editTest_formation.ElementAt(et_cur[1]);
						if (BitUtil.Check(formationData.flag_, 2))
						{
							g.DrawString2("左ｿﾌﾄｷｰで編集できます。", num9 + 28, num + 6);
						}
						else
						{
							g.DrawString2(formationData.explain_, num9 + 28, num + 6);
						}
					}
					else if (phase_ == 2)
					{
						g.DrawString2("移動元を選択してください。", num9 + 28, num + 6);
					}
					else
					{
						g.DrawString2("移動先を選択してください。", num9 + 28, num + 6);
					}
				}
				else if (!isSoccerForm)
				{
					g.DrawString2(HELP_STR[et_cur[0]], num9 + 28, num + 6);
				}
				else if (phase_ == 14 || phase_ == 16 || phase_ == 15)
				{
					g.DrawString2("相手ﾁｰﾑの情報です。", num9 + 28, num + 6);
				}
				else if (et_cur[0] == 0)
				{
					g.DrawString2("相手ﾁｰﾑの情報です。", num9 + 28, num + 6);
				}
				else if (et_cur[0] == 1)
				{
					if (!gm_.halfTime)
					{
						g.DrawString2("試合開始です!", num9 + 28, num + 6);
					}
					else
					{
						g.DrawString2("後半キックオフです!", num9 + 28, num + 6);
					}
				}
				else
				{
					g.DrawString2(HELP_STR[et_cur[0] - 2], num9 + 28, num + 6);
				}
			}
			if (phase_ == 1 || phase_ == 9 || phase_ == 11 || phase_ == 12 || phase_ == 2 || phase_ == 3 || phase_ == 4 || phase_ == 5)
			{
				int num10 = 60;
				int num11 = 4;
				int num12 = 18;
				int num13 = 2;
				int num9 = EDIT_POS_MENU_F[0][0];
				num = EDIT_POS_MENU_F[0][1];
				if (phase_ == 9)
				{
					num = GameUtil.RateConvert(phaseCnt_, 4, 8, EDIT_POS_MENU_F[1][1], EDIT_POS_MENU_F[0][1], true);
				}
				else if (phase_ == 11 || phase_ == 12)
				{
					num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_MENU_F[0][1], EDIT_POS_MENU_F[1][1], true);
				}
				bool isTouch = false;
				if (phase_ == 1)
				{
					isTouch = true;
				}
				for (int k = scrollValue_ * 3; k < editTest_formation.Size(); k++)
				{
					int num14 = k - scrollValue_ * 3;
					if (num14 / 3 >= 3)
					{
						break;
					}
					FormationData formationData2 = (FormationData)editTest_formation.ElementAt(k);
					num2 = num9 + (num10 + num11) * (num14 % 3);
					num3 = num + (num12 + num13) * (num14 / 3);
					if (k == et_cur[1])
					{
						Draw_editButton(g, num2, num3, num10, num12, true, 1, k, isTouch, true);
					}
					else
					{
						Draw_editButton(g, num2, num3, num10, num12, false, 1, k, isTouch, true);
					}
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					g.DrawString(formationData2.name_, num2 + num10 / 2, num3 + 4, 2);
					if (formationData2.IsNew())
					{
						SubForm.Draw_newMark(g, num2 + 20, num3 - 5, phaseCnt_);
					}
					if (k == et_cur[1])
					{
						if (phase_ == 2 || phase_ == 3 || phase_ == 4 || phase_ == 5)
						{
							apdat_.DrawSeb(g, num2 - 4, num3 + 9, apdat_.resCom2_.seb[60], apdat_.resCom2_.img[70], 0);
						}
						else
						{
							apdat_.resCom2_.seb[60].Draw(g, num2 - 4, num3 + 9, apdat_.resCom2_.img);
						}
					}
				}
				if (phase_ == 1)
				{
					apdat_.DrawVerticalScroll3(g, 221, num, 3, 59, scrollValue_, (editTest_formation.Size() + 2) / 3, 3);
				}
			}
			if (phase_ == 6 || phase_ == 7 || phase_ == 8 || phase_ == 10 || phase_ == 13)
			{
				int num9 = EDIT_POS_MENU_M[0][0];
				num = EDIT_POS_MENU_M[0][1];
				if (phase_ == 10)
				{
					num = GameUtil.RateConvert(phaseCnt_, 4, 8, EDIT_POS_MENU_M[1][1], EDIT_POS_MENU_M[0][1], true);
				}
				else if (phase_ == 13)
				{
					num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_MENU_M[0][1], EDIT_POS_MENU_M[1][1], true);
				}
				int num15 = 18;
				int num16 = 0;
				int num17 = 0;
				int num18 = 0;
				for (int l = 0; l < editTest_team.Length; l++)
				{
					int num19 = GameUtil.RateConvert(l, 0, editTest_team.Length - 1, num9 + 212 / editTest_team.Length / 2, num9 + 212 - 212 / editTest_team.Length / 2, true);
					Vector vector = editTest_team[l];
					num17 = ((JInteger)editTest_category.ElementAt(l)).IntValue();
					apdat_.DrawSeb(g, num19 - 12, num + 3, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], num17);
					int num20 = 212 / editTest_team.Length - 2;
					for (int m = 0; m < vector.Size(); m++)
					{
						int[] array = (int[])vector.ElementAt(m);
						num2 = num19 - num20 / 2;
						num3 = num + (num15 + num16) * (m + 1) - 2;
						if (array[2] != -1)
						{
							int num21 = array[2];
							if (phase_ == 8)
							{
								int[] array2 = (int[])editTest_team[et_cur[5] / 10].ElementAt(et_cur[5] % 10);
								int[] array3 = (int[])editTest_team[et_cur[6] / 10].ElementAt(et_cur[6] % 10);
								if (num21 == array2[2])
								{
									num21 = array3[2];
								}
								else if (num21 == array3[2])
								{
									num21 = array2[2];
								}
							}
							PlayerData playerData = (PlayerData)memberPDataist_.ElementAt(num21);
							bool flag = false;
							if (editMemDispMode == 0)
							{
								if (phase_ == 6 && l == et_cur[5] / 10 && m == et_cur[5] % 10)
								{
									Draw_editButton(g, num2, num3, num20, num15, true, 5, l * 10 + m, true, flag);
								}
								else if (phase_ == 7 && l == et_cur[6] / 10 && m == et_cur[6] % 10)
								{
									Draw_editButton(g, num2, num3, num20, num15, true, 6, l * 10 + m, true, flag);
								}
								else
								{
									int type = 5;
									if (phase_ == 7)
									{
										type = 6;
									}
									Draw_editButton(g, num2, num3, num20, num15, false, type, l * 10 + m, true, flag);
								}
								g.SetColor(SubForm.SC_WINDOW_BLACK);
								if (Language.Japanese())
								{
									g.DrawString(playerData.name_, num2 + num20 / 2, num3 + 3, 2);
								}
								else
								{
									if (editTest_team.Length == 4)
									{
										SubForm.SetFontSize(g, 10);
										g.DrawString(playerData.name_, num2 + num20 / 2, num3 + 3 + 1, 2);
									}
									else
									{
										SubForm.SetFontSize(g, 8);
										g.DrawString(playerData.name_, num2 + num20 / 2, num3 + 3 + 1 + 1, 2);
									}
									SubForm.ResetFontSize(g);
								}
							}
							else if (editMemDispMode == 1)
							{
								if (editTest_team.Length == 4)
								{
									g.DrawImage(apdat_.resCom2_.img[127], num2, num3, 0, 0, 51, 18);
								}
								else
								{
									g.DrawImage(apdat_.resCom2_.img[127], num2, num3, 51, 0, 40, 18);
								}
								g.ClipRect(num2, num3, 13f, 17f);
								SetDispPlayerData(0, selectUniform_, playerData.faceImg_, 0, 1, playerData.role_, 0);
								DrawDispPlayer(g, num2 + 7, num3 + 16 + 5);
								g.PopClip();
								if (num21 == 0)
								{
									num18 = PlayerData.GetPhyParamBarW(playerData.paramPhysical_[4], num20 - 11);
									if (editTest_team.Length == 4)
									{
										g.DrawImage(apdat_.resCom2_.img[127], num2 + 13, num3 + 1 + 5, 13, 52, num18, 6);
									}
									else
									{
										g.DrawImage(apdat_.resCom2_.img[127], num2 + 13, num3 + 1 + 5, 64, 52, num18, 6);
									}
								}
								else
								{
									num18 = 0;
									for (int n = 0; n < 4; n++)
									{
										num18 = PlayerData.GetPhyParamBarW(playerData.paramPhysical_[n], num20 - 11);
										if (editTest_team.Length == 4)
										{
											g.DrawImage(apdat_.resCom2_.img[127], num2 + 13, num3 + 1 + n * 4, 13, 36 + n * 4, num18, 4);
										}
										else
										{
											g.DrawImage(apdat_.resCom2_.img[127], num2 + 13, num3 + 1 + n * 4, 64, 36 + n * 4, num18, 4);
										}
									}
								}
							}
							else if (editMemDispMode == 2)
							{
								if (editTest_team.Length == 4)
								{
									g.DrawImage(apdat_.resCom2_.img[127], num2, num3, 0, 18, 51, 18);
								}
								else if (Language.English())
								{
									g.DrawImage(apdat_.resCom2_.img[127], num2, num3, 51, 18, 40, 18);
								}
								else
								{
									g.DrawImage(apdat_.resCom2_.img[127], num2, num3, 51, 18, 51, 18);
								}
								g.ClipRect(num2, num3, 13f, 17f);
								SetDispPlayerData(0, selectUniform_, playerData.faceImg_, 0, 1, playerData.role_, 0);
								DrawDispPlayer(g, num2 + 7, num3 + 16 + 5);
								g.PopClip();
								g.SetColor(Graphics.GetColorOfRGB(208, 237, 246));
								apdat_.DrawPositionStatus(g, num17, -1, num2 + 13 + 2, num3 + 3);
								int positionRank = PlayerData.GetPositionRank(num17, playerData);
								if (editTest_team.Length == 4)
								{
									apdat_.DrawSeb(g, num2 + num20 - 8 - 6, num3 + 15, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], positionRank);
								}
								else
								{
									apdat_.DrawSeb(g, num2 + num20 - 8, num3 + 15, apdat_.resCom2_.seb[65], apdat_.resCom2_.img[75], positionRank);
								}
							}
							else if (editMemDispMode == 3)
							{
								if (editTest_team.Length == 4)
								{
									g.DrawImage(apdat_.resCom2_.img[127], num2, num3, 0, 18, 51, 18);
								}
								else if (Language.English())
								{
									g.DrawImage(apdat_.resCom2_.img[127], num2, num3, 51, 18, 40, 18);
								}
								else
								{
									g.DrawImage(apdat_.resCom2_.img[127], num2, num3, 51, 18, 51, 18);
								}
								g.ClipRect(num2, num3, 13f, 17f);
								SetDispPlayerData(0, selectUniform_, playerData.faceImg_, 0, 1, playerData.role_, 0);
								DrawDispPlayer(g, num2 + 7, num3 + 16 + 5);
								g.PopClip();
								g.SetColor(Graphics.GetColorOfRGB(208, 237, 246));
								if (editTest_team.Length == 4)
								{
									apdat_.DrawLv(g, MyFormBase.LT("Lv"), num2 + 13 + 2 + 2, num3 + 3);
									g.DrawString(playerData.lv_ + string.Empty, num2 + num20 - 1 - 3, num3 + 3, 4);
								}
								else
								{
									apdat_.DrawLv(g, MyFormBase.LT("Lv"), num2 + 15 - (Language.English() ? 2 : 0), num3 + 3);
									g.DrawString(playerData.lv_ + string.Empty, num2 + num20 - 1, num3 + 3, 4);
								}
							}
							else if (editMemDispMode == 4)
							{
								Draw_editButton(g, num2, num3, num20, num15, false, 0, m, false, false);
								int sallay = playerData.GetSallay();
								if (editTest_team.Length == 4)
								{
									if (Language.Japanese())
									{
										apdat_.DrawNumImage(g, 78, sallay, num2 + num20 - 2 - 9 - 2, num3 + 4, 0, 4);
										apdat_.DrawSeb(g, num2 + num20 - 2 - 9 + 1 - 2, num3 + 4, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 17);
									}
									else if (AppData.IsLanguageOther())
									{
										apdat_.DrawMoney(g, 78, sallay, num2 + num20 - 2 - 9 - 2 + 10, num3 + 4, 0, 0, 4);
									}
									else
									{
										bool flag2 = AppData.GetInstance().sysSave_.ints[14] == 1;
										int num22 = 0;
										if (!flag2)
										{
											num22 = 1;
										}
										if (sallay >= 1000 && !flag2)
										{
											apdat_.DrawMoney3(g, 78, sallay, num2 + num20 - 2 - 9 - 2 + 10 + num22, num3 + 4, 0, 0, 4);
										}
										else
										{
											apdat_.DrawMoney(g, 78, sallay, num2 + num20 - 2 - 9 - 2 + 10 + num22, num3 + 4, 0, 0, 4);
										}
									}
								}
								else if (Language.Japanese())
								{
									apdat_.DrawNumImage(g, 78, sallay, num2 + num20 - 2 - 2, num3 + 4, 0, 4);
								}
								else if (AppData.IsLanguageOther())
								{
									apdat_.DrawMoney2(g, 78, sallay, num2 + num20 - 2 - 2, num3 + 4, -1, 0, 4);
								}
								else if (AppData.GetInstance().sysSave_.ints[14] == 1)
								{
									apdat_.DrawMoney2(g, 78, sallay, num2 + num20 - 2 - 2 + 2, num3 + 4, 0, 0, 4);
								}
								else if (sallay >= 1000)
								{
									apdat_.DrawMoney2(g, 78, sallay, num2 + num20 - 2 - 2 - 8 + 5, num3 + 4, -1, 0, 4);
								}
								else
								{
									apdat_.DrawMoney2(g, 78, sallay, num2 + num20 - 2 - 2 - 8 + 3, num3 + 4, -1, 0, 4);
								}
							}
							if (editMemDispMode == 1 || editMemDispMode == 2 || editMemDispMode == 3 || editMemDispMode == 4)
							{
								int num23 = 1114112;
								if (phase_ == 7)
								{
									num23 = 1179648;
								}
								if (flag)
								{
									GameView.AddTouch(g, 23, num2, num3, num20, num15, num23 | (l * 10 + m), TouchOption.Create(16).Margin(5, 5, 5, 5));
								}
								else
								{
									GameView.AddTouch(g, 23, num2, num3, num20, num15, num23 | (l * 10 + m));
								}
							}
						}
						if (phase_ == 7 && l == et_cur[5] / 10 && m == et_cur[5] % 10)
						{
							Draw_colorRect(g, num2, num3, phaseCnt_, num20 - 1, num15 - 1);
						}
						if (phase_ == 6 && l == et_cur[5] / 10 && m == et_cur[5] % 10)
						{
							apdat_.resCom2_.seb[60].Draw(g, num2 - 3, num3 + 9, apdat_.resCom2_.img);
						}
						else if (phase_ == 7 && l == et_cur[6] / 10 && m == et_cur[6] % 10)
						{
							apdat_.resCom2_.seb[60].Draw(g, num2 - 3, num3 + 9, apdat_.resCom2_.img);
						}
					}
				}
			}
			g.PopClip();
		}

		public void Draw_edit2(Graphics g, bool isSoccerForm)
		{
int coalescedValue, num, num2, num3;
coalescedValue = num = num2 = num3 = 0;




			int num4 = 0;
			if (isSoccerForm) { num4 = SoccerForm.VIEW_Y; } else { num4 = GameForm.VIEW_Y; }
			g.ClipRect(13f, num4, 221f, 236 - num4);
			if (phase_ == 0 || phase_ == 1 || phase_ == 9 || phase_ == 11 || phase_ == 12 || phase_ == 6 || phase_ == 10 || phase_ == 13 || phase_ == 7 || phase_ == 8 || phase_ == 2 || phase_ == 3 || phase_ == 4 || phase_ == 5 || phase_ == 14 || phase_ == 16 || phase_ == 15)
			{

				if (!isSoccerForm)
				{
					coalescedValue = EDIT_POS_FOR_F[0][0];
					num = EDIT_POS_FOR_F[0][1];
					if (phase_ == 1)
					{
						num = EDIT_POS_FOR_F[1][1];
					}
					else if (phase_ == 9)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_F[0][1], EDIT_POS_FOR_F[1][1], true);
					}
					else if (phase_ == 11)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_F[1][1], EDIT_POS_FOR_F[0][1], true);
					}
					else if (phase_ == 12)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_F[1][1], EDIT_POS_FOR_F[0][1], true);
					}
					else if (phase_ == 2)
					{
						num = EDIT_POS_FOR_F[1][1];
					}
					else if (phase_ == 3)
					{
						num = EDIT_POS_FOR_F[1][1];
					}
					else if (phase_ == 4)
					{
						num = EDIT_POS_FOR_F[1][1];
					}
					else if (phase_ == 5)
					{
						num = EDIT_POS_FOR_F[1][1];
					}
					else if (phase_ == 6)
					{
						num = EDIT_POS_FOR_M[1][1];
					}
					else if (phase_ == 7)
					{
						num = EDIT_POS_FOR_M[1][1];
					}
					else if (phase_ == 8)
					{
						num = EDIT_POS_FOR_M[1][1];
					}
					else if (phase_ == 10)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_M[0][1], EDIT_POS_FOR_M[1][1], true);
					}
					else if (phase_ == 13)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_M[1][1], EDIT_POS_FOR_M[0][1], true);
					}
				}
				else
				{
					coalescedValue = EDIT_POS_FOR_F[1][0];
					num = EDIT_POS_FOR_F[1][1];
					if (phase_ == 6)
					{
						num = EDIT_POS_FOR_M[1][1];
					}
					else if (phase_ == 7)
					{
						num = EDIT_POS_FOR_M[1][1];
					}
					else if (phase_ == 8)
					{
						num = EDIT_POS_FOR_M[1][1];
					}
					else if (phase_ == 14)
					{
						coalescedValue = EDIT_POS_FOR_ENE[1][0];
					}
					else if (phase_ == 10)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_F[1][1], EDIT_POS_FOR_M[1][1], true);
					}
					else if (phase_ == 13)
					{
						num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_M[1][1], EDIT_POS_FOR_F[1][1], true);
					}
					else if (phase_ == 15)
					{
						coalescedValue = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_F[1][0], EDIT_POS_FOR_ENE[1][0], true);
					}
					else if (phase_ == 16)
					{
						coalescedValue = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_ENE[1][0], EDIT_POS_FOR_F[1][0], true);
					}
				}
				for (int i = 0; i < 6; i++)
				{
					apdat_.DrawSeb(g, coalescedValue + 107 - 107 + 33 * i, num, apdat_.resCom2_.seb[45], apdat_.resCom2_.img[49], 0);
				}
				apdat_.DrawSeb(g, coalescedValue + 107 - 107 + 198, num, apdat_.resCom2_.seb[45], apdat_.resCom2_.img[49], 1);
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				int num6 = 16;
				g.DrawArc(coalescedValue + 107 - 1 - num6 / 2 - 1, num + 47 - num6 / 2 - 1, num6 + 2, num6 + 2, 0f, 360f);
				g.DrawLine(coalescedValue + 107 - 1, num, coalescedValue + 107 - 1, num + 94 - 1);
				int num7 = 35;
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				g.DrawRect(coalescedValue, num + 47 - 32, num7, 65f);
				g.DrawRect(coalescedValue, num + 47 - 16, num7 / 2, 32f);
				g.DrawRect(coalescedValue + 214 - num7, num + 47 - 32, num7 - 1, 65f);
				g.DrawRect(coalescedValue + 214 - num7 / 2, num + 47 - 16, num7 / 2 - 1, 32f);
				if (phase_ == 2 || phase_ == 3 || phase_ == 4)
				{
					g.SetColor(Graphics.GetColorOfRGB(219, 255, 109));
					g.FillRect(coalescedValue + 35, num + 2, 48f, 90f);
					g.SetColor(Graphics.GetColorOfRGB(255, 156, 40));
					g.FillRect(coalescedValue + 35 + 48, num + 2, 48f, 90f);
					g.SetColor(Graphics.GetColorOfRGB(255, 56, 49));
					g.FillRect(coalescedValue + 35 + 96, num + 2, 48f, 90f);
					g.SetColor(Graphics.GetColorOfRGB(85, 85, 85));
					for (int unusedOriginalSlot8, j = 0; j <= 9; j++)
					{
						num3 = GameUtil.RateConvert(j, 0, 9, num + 2, num + 94 - 1 - 2, true);
						g.DrawLine(coalescedValue + 35, num3, coalescedValue + 214 - 1 - 35, num3);
					}
					for (int k = 0; k <= 9; k++)
					{
						num2 = GameUtil.RateConvert(k, 0, 9, coalescedValue + 214 - 1 - 35, coalescedValue + 35, true);
						if (k % 3 == 0 && k != 0 && k != 9)
						{
							g.SetColor(Graphics.GetColorOfRGB(59, 228, 255));
							g.FillRect(coalescedValue - 13 + num2 - 1, num + 2, 3f, 90f);
						}
						else
						{
							g.SetColor(Graphics.GetColorOfRGB(85, 85, 85));
							g.DrawLine(coalescedValue - 13 + num2, num + 2, num2, num + 94 - 1 - 2);
						}
					}
				}
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				g.DrawRect(coalescedValue, num, 213f, 93f);
				int[] array = null;
				int[] array2 = null;
				if (phase_ == 6 && et_cur[5] / 10 < editTest_team.Length && et_cur[5] % 10 < editTest_team[et_cur[5] / 10].Size())
				{
					array = (int[])editTest_team[et_cur[5] / 10].ElementAt(et_cur[5] % 10);
				}
				if (phase_ == 7)
				{
					if (et_cur[5] / 10 < editTest_team.Length && et_cur[5] % 10 < editTest_team[et_cur[5] / 10].Size())
					{
						array2 = (int[])editTest_team[et_cur[5] / 10].ElementAt(et_cur[5] % 10);
					}
					if (et_cur[6] / 10 < editTest_team.Length && et_cur[6] % 10 < editTest_team[et_cur[6] / 10].Size())
					{
						array = (int[])editTest_team[et_cur[6] / 10].ElementAt(et_cur[6] % 10);
					}
				}
				if (phase_ == 2 || phase_ == 3)
				{
					num2 = GameUtil.RateConvert(et_cur[2] % 10, 0, 9, coalescedValue + 214 - 1 - 35, coalescedValue + 35, true);
					num3 = GameUtil.RateConvert(et_cur[2] / 10, 0, 9, num + 2, num + 94 - 1 - 2, true);
					num2 += coalescedValue - 13;
					int num8 = 15;
					int num9 = 9;
					int num10 = 200;
					if (phase_ == 3)
					{
						num10 = 100;
						Draw_colorRect(g, num2 - num8, num3, phaseCnt_, num8 - 1, num9 - 1, num10);
					}
					else
					{
						g.SetColor(Graphics.GetColorOfRGB(154, 234, 255));
						g.FillRect(num2 - num8, num3 + 1, num8, num9);
					}
					if (phase_ == 3 && et_cur[2] != et_cur[3])
					{
						num2 = GameUtil.RateConvert(et_cur[3] % 10, 0, 9, coalescedValue + 214 - 1 - 35, coalescedValue + 35, true);
						num3 = GameUtil.RateConvert(et_cur[3] / 10, 0, 9, num + 2, num + 94 - 1 - 2, true);
						g.SetColor(Graphics.GetColorOfRGB(154, 234, 255));
						g.FillRect(num2 - num8, num3 + 1, num8, num9);
					}
					int num11 = 0;
					int num12 = 0;
					int num13 = 0;
					int num14 = 0;
					for (int l = 0; l < 9; l++)
					{
						for (int m = 0; m < 9; m++)
						{
							num2 = GameUtil.RateConvert(l, 0, 9, coalescedValue + 214 - 1 - 35, coalescedValue + 35, true);
							num3 = GameUtil.RateConvert(m, 0, 9, num + 2, num + 94 - 1 - 2, true);
							num2 += coalescedValue - 13;
							num11 = num2 - num8;
							num12 = num3 + 1;
							num13 = num8;
							num14 = num9;
							if (phase_ == 2)
							{
								GameView.AddTouch(g, 23, num11, num12, num13, num14, 0xE0000 | (m * 10 + l), TouchOption.Create().Margin(5, 5, 5, 5));
							}
							else
							{
								GameView.AddTouch(g, 23, num11, num12, num13, num14, 0xF0000 | (m * 10 + l), TouchOption.Create().Margin(5, 5, 5, 5));
							}
						}
					}
				}
				if (phase_ != 2 && phase_ != 3 && phase_ != 4 && phase_ != 14)
				{
					num2 = coalescedValue + 161;
					num3 = num + 84;
					g.DrawImage(apdat_.resCom2_.img[128], num2, num3 - 2);
					string[] array3 = new string[3] { "通常", "攻め", "守り" };
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					SubForm.SetFontSize(g, 10);
					g.DrawString(MyFormBase.LT("ﾀｲﾌﾟ") + MyFormBase.LT(array3[selectFormationType_]), num2 + 5 + 24, num3 + 2, 2);
					SubForm.ResetFontSize(g);
				}
				int seb = 0;
				int num15 = 0;
				for (int n = 0; n < editForPos.Length; n++)
				{
					PlayerData playerData = (PlayerData)memberPDataist_.ElementAt(editForPos[n][2]);
					num2 = editForPos[n][0];
					num3 = editForPos[n][1];
					num2 += coalescedValue - 13;
					num15 = phaseCnt_ % 16 / 4;
					SetDispPlayerData(seb, selectUniform_, playerData.faceImg_, num15, 1, playerData.role_, 0);
					DrawDispPlayer(g, num2, num3 + 2);
					if (array != null && array[2] == editForPos[n][2])
					{
						apdat_.DrawSeb(g, num2, num3 - 24, apdat_.resCom2_.seb[62], apdat_.resCom2_.img[71], 0);
					}
					if (array2 != null && array2[2] == editForPos[n][2])
					{
						int num16 = 16;
						int num17 = 21;
						Draw_colorRect(g, num2 - num16 / 2, num3 - num17 + 2, phaseCnt_, num16 - 1, num17 - 1);
					}
				}
				if (phase_ == 5)
				{
					apdat_.DrawWindow(g, 140, 100, 0, "ﾀｲﾌﾟ選択");
					apdat_.DrawBox(g, 60, 94, 180, 165);
					num = 115;
					for (int num18 = 0; num18 < FORMATIONTYPE_STR.Length; num18++)
					{
						num3 = num + num18 * 18;
						if (num18 == et_cur[4])
						{
							g.SetColor(SubForm.SC_WINDOW_SELECT);
							g.FillRect(70f, num3 - 2, 100f, 17f);
							apdat_.resCom2_.seb[60].Draw(g, 70f, num3 + 8, apdat_.resCom2_.img);
						}
						GameView.AddTouch(g, 11, 60, num3 - 2, 120, 17, 0x100000 | num18);
						g.SetColor(SubForm.SC_WINDOW_BLACK);
						g.DrawString(FORMATIONTYPE_STR[num18], 120f, num3, 2);
					}
				}
				if (phase_ == 14 || phase_ == 15 || phase_ == 16)
				{
					TeamData teamData = apdat_.tData_[gm_.teamId_[1]];
					int[] array4 = null;
					if (matchInfoJoin_ == 0)
					{
						array4 = GetLeagueData(teamData.id_);
					}
					coalescedValue = EDIT_POS_ENE_MENU[1][0];
					num = EDIT_POS_ENE_MENU[1][1];
					if (phase_ == 15)
					{
						coalescedValue = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_ENE_MENU[0][0], EDIT_POS_ENE_MENU[1][0], true);
					}
					else if (phase_ == 16)
					{
						coalescedValue = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_ENE_MENU[1][0], EDIT_POS_ENE_MENU[0][0], true);
					}
					apdat_.DrawBox(g, coalescedValue - 2, num - 13, coalescedValue - 2 + 213, num - 13 + 74);
					apdat_.DrawBox(g, coalescedValue + 2, num - 9, coalescedValue + 2 + 204, num - 9 + 66);
					g.SetColor(Graphics.GetColorOfRGB(246, 249, 187));
					g.FillRect(coalescedValue + 6, num + 5, 90f, 60f);
					num2 = coalescedValue + 49;
					num3 = num + 36;
					DrawGraph_menuElement(g, num2, num3, playerParam1, playerParam2, 100);
					for (int num19 = 0; num19 < GRAPH_POS.Length; num19++)
					{
						apdat_.DrawSeb(g, num2 + GRAPH_POS[num19][0], num3 + GRAPH_POS[num19][1], apdat_.resCom2_.seb[75], apdat_.resCom2_.img[122], num19);
					}
					num2 = coalescedValue + 108;
					num3 = num + 12;
					g.SetColor(SubForm.SC_WINDOW_BLUE);
					if (matchInfoJoin_ == 0)
					{
						g.DrawString("リーグ", num2 - 1, num3 + 7);
					}
					else
					{
						g.DrawString("ﾁｰﾑ評価", num2 - 1, num3 + 7);
					}
					g.DrawString("観客数", num2 - 1, num3 + 3 + 26);
					if (matchInfoJoin_ == 0)
					{
						if (Language.Japanese())
						{
							apdat_.DrawNumImageComma(g, 35, array4[7], num2 + 75, num3 + 1 + 7, 0, 0, 4);
							g.SetColor(SubForm.SC_WINDOW_BLACK);
							g.DrawString("位", num2 + 75 + 2, num3 + 7);
						}
						else
						{
							int num20 = 78;
							g.SetColor(SubForm.SC_WINDOW_BLACK);
							g.DrawString("位", num2 + num20 - 30, num3 + 7);
							apdat_.DrawNumImageComma(g, 35, array4[7], num2 + num20 + 9, num3 + 7 + 1, 0, 0, 2);
						}
					}
					else if (!SubForm.En())
					{
						apdat_.DrawNumImageComma(g, 35, teamData.warPotential_, num2 + 75, num3 + 1 + 7, 0, 0, 4);
					}
					else
					{
						apdat_.DrawNumImageComma(g, 35, teamData.warPotential_, num2 + 75 + 14, num3 + 1 + 7, 0, 0, 4);
					}
					if (Language.Japanese())
					{
						g.DrawString(MyFormBase.LT("<0>人", AppData.CommaSeparate(apdat_.frmSoccer_.customerNum[1])), num2 + 76 + 14, num3 + 3 + 26, 4);
					}
					else
					{
						g.DrawString(MyFormBase.LT("<0>", AppData.CommaSeparate(apdat_.frmSoccer_.customerNum[1])), num2 + 76 + 14 - 2, num3 + 3 + 26, 4);
					}
				}
				if (phase_ == 14 || phase_ == 16 || phase_ == 15)
				{
					TeamData teamData2 = apdat_.tData_[gm_.teamId_[1]];
					coalescedValue = EDIT_POS_ENE_FOR[1][0];
					num = EDIT_POS_ENE_FOR[1][1];
					if (phase_ == 15)
					{
						coalescedValue = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_ENE_FOR[0][0], EDIT_POS_ENE_FOR[1][0], true);
					}
					else if (phase_ == 16)
					{
						coalescedValue = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_ENE_FOR[1][0], EDIT_POS_ENE_FOR[0][0], true);
					}
					for (int num21 = 0; num21 < 6; num21++)
					{
						apdat_.DrawSeb(g, coalescedValue + 107 - 107 + 33 * num21, num, apdat_.resCom2_.seb[45], apdat_.resCom2_.img[49], 0);
					}
					apdat_.DrawSeb(g, coalescedValue + 107 - 107 + 198, num, apdat_.resCom2_.seb[45], apdat_.resCom2_.img[49], 1);
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
					int num22 = 16;
					g.DrawArc(coalescedValue + 107 - 1 - num22 / 2 - 1, num + 47 - num22 / 2 - 1, num22 + 2, num22 + 2, 0f, 360f);
					g.DrawLine(coalescedValue + 107 - 1, num, coalescedValue + 107 - 1, num + 94 - 1);
					int num23 = 35;
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
					g.DrawRect(coalescedValue, num + 47 - 32, num23, 65f);
					g.DrawRect(coalescedValue, num + 47 - 16, num23 / 2, 32f);
					g.DrawRect(coalescedValue + 214 - num23, num + 47 - 32, num23 - 1, 65f);
					g.DrawRect(coalescedValue + 214 - num23 / 2, num + 47 - 16, num23 / 2 - 1, 32f);
					if (phase_ == 2 || phase_ == 3 || phase_ == 4)
					{
						g.SetColor(Graphics.GetColorOfRGB(219, 255, 109));
						g.FillRect(coalescedValue + 35, num + 2, 48f, 90f);
						g.SetColor(Graphics.GetColorOfRGB(255, 156, 40));
						g.FillRect(coalescedValue + 35 + 48, num + 2, 48f, 90f);
						g.SetColor(Graphics.GetColorOfRGB(255, 56, 49));
						g.FillRect(coalescedValue + 35 + 96, num + 2, 48f, 90f);
						g.SetColor(Graphics.GetColorOfRGB(85, 85, 85));
						for (int unusedOriginalSlot38, num24 = 0; num24 <= 9; num24++)
						{
							num3 = GameUtil.RateConvert(num24, 0, 9, num + 2, num + 94 - 1 - 2, true);
							g.DrawLine(coalescedValue + 35, num3, coalescedValue + 214 - 1 - 35, num3);
						}
						for (int num25 = 0; num25 <= 9; num25++)
						{
							num2 = GameUtil.RateConvert(num25, 0, 9, coalescedValue + 214 - 1 - 35, coalescedValue + 35, true);
							if (num25 % 3 == 0 && num25 != 0 && num25 != 9)
							{
								g.SetColor(Graphics.GetColorOfRGB(59, 228, 255));
								g.FillRect(coalescedValue - 13 + num2 - 1, num + 2, 3f, 90f);
							}
							else
							{
								g.SetColor(Graphics.GetColorOfRGB(85, 85, 85));
								g.DrawLine(coalescedValue - 13 + num2, num + 2, num2, num + 94 - 1 - 2);
							}
						}
					}
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
					g.DrawRect(coalescedValue, num, 213f, 93f);
					num2 = coalescedValue + 161;
					num3 = num + 84;
					g.DrawImage(apdat_.resCom2_.img[128], num2, num3 - 2);
					string[] array5 = new string[3] { "通常", "攻め", "守り" };
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					g.DrawString(MyFormBase.LT("ﾀｲﾌﾟ") + MyFormBase.LT(array5[teamData2.formationType_]), num2 + 5 + 24, num3 + 2, 2);
					num2 = coalescedValue + 1;
					num3 = num + 84;
					FormationData formationData = apdat_.formationData_[teamData2.formation_];
					g.DrawImage(apdat_.resCom2_.img[128], num2, num3 - 2);
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					g.DrawString(formationData.name_, num2 + 36, num3 + 2, 2);
					int seb2 = 0;
					int num26 = 0;
					for (int num27 = 0; num27 < editForPosEnemy.Length; num27++)
					{
						num2 = editForPosEnemy[num27][0];
						num3 = editForPosEnemy[num27][1];
						num2 += coalescedValue - 13;
						num26 = phaseCnt_ % 16 / 4;
						if (editForPosEnemy[num27][2] == 0)
						{
							SetDispPlayerData(seb2, teamData2.bodyImg_, teamData2.memberFaceId_[editForPosEnemy[num27][2]], num26, 2, 0, teamData2.id_);
						}
						else
						{
							SetDispPlayerData(seb2, teamData2.bodyImg_, teamData2.memberFaceId_[editForPosEnemy[num27][2]], num26, 2, 3, teamData2.id_);
						}
						DrawDispPlayer(g, num2, num3 + 2);
					}
				}
			}
			g.PopClip();
			if (phase_ == 7)
			{
				int num28 = GameUtil.RateConvert(phaseCnt_, 0, 3, 19, 0, true);
				g.SetClip(2f, 220f, 126f, 19f);
				SubForm.Draw_chara(g, 172, 0, 0, 0, 11, 247 + num28);
				g.DrawImage(apdat_.resCom2_.img[180], 17f, 223 + num28);
				g.ClearClip();
			}
			else if (phase_ == 6 && editMemDispMode != 0 && emdm_cnt < EMDM_ANIME_TIME[1])
			{
				int frame = editMemDispMode - 1;
				coalescedValue = GameUtil.RateConvert(emdm_cnt, 0, EMDM_ANIME_TIME[0], -60, 0, true);
				g.SetColor(Graphics.GetColorOfRGB(11, 192, 245));
				g.DrawRect(-1 + coalescedValue, 121f, 56f, 16f);
				g.SetColor(Graphics.GetColorOfRGB(15, 154, 255));
				g.FillRect(-1 + coalescedValue + 1, 122f, 55f, 15f);
				apdat_.DrawSeb(g, -1 + coalescedValue + 2, 123, apdat_.resCom2_.seb[99], apdat_.resCom2_.img[182], frame);
			}
		}

		public void Draw_editButton(Graphics g, int dx, int dy, int W, int H, bool select, int type, int index)
		{
			Draw_editButton(g, dx, dy, W, H, select, type, index, true, false);
		}

		public void Draw_editButton(Graphics g, int dx, int dy, int W, int H, bool select, int type, int index, bool isTouch, bool isMarker)
		{
			g.SetColor(Graphics.GetColorOfRGB(223, 223, 220));
			g.DrawRect(dx, dy, W - 1, H - 1);
			g.SetColor(Graphics.GetColorOfRGB(153, 152, 145));
			g.DrawRect(dx + 1, dy + 1, W - 2 - 1, H - 2 - 1);
			if (select)
			{
				g.DrawImage(apdat_.resCom2_.img[61], dx + 2, dy + 2, 0, (19 - (H - 4)) / 2 + 19, W - 4, H - 4);
			}
			else
			{
				g.DrawImage(apdat_.resCom2_.img[61], dx + 2, dy + 2, 0, (19 - (H - 4)) / 2, W - 4, H - 4);
			}
			if (isTouch)
			{
				int num = 786432;
				switch (type)
				{
				case 0:
					num = 786432;
					break;
				case 1:
					num = 851968;
					break;
				case 2:
					num = 917504;
					break;
				case 3:
					num = 983040;
					break;
				case 4:
					num = 1048576;
					break;
				case 5:
					num = 1114112;
					break;
				case 6:
					num = 1179648;
					break;
				}
				if (isMarker)
				{
					GameView.AddTouch(g, 23, dx, dy, W, H, num | index, TouchOption.Create(16).Margin(5, 5, 5, 5));
				}
				else
				{
					GameView.AddTouch(g, 23, dx, dy, W, H, num | index, TouchOption.Create().Margin(5, 5, 5, 5));
				}
			}
		}

		public void Draw_colorRect(Graphics g, int ox, int oy, int cnt, int W, int H)
		{
			Draw_colorRect(g, ox, oy, cnt, W, H, 200);
		}

		public void Draw_colorRect(Graphics g, int ox, int oy, int cnt, int W, int H, int speed)
		{
			int num = W * 2 + H * 2;
			int num2 = num / 16 + 1;
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
			g.DrawRect(ox, oy, W, H);
			for (int i = 0; i < num2; i++)
			{
				for (int j = i * 4; j < 4 + i * 4; j++)
				{
					GetPoint(ox, oy, (cnt + 4 * j) * speed / 100, d1, W, H);
					GetPoint(ox, oy, (cnt + 4 * (j + 1)) * speed / 100, d2, W, H);
					g.SetColor(CR_COLOR[j % 4]);
					if (d1[0] != d2[0] && d1[1] != d2[1])
					{
						if (d2[1] == oy || d2[1] == oy + H)
						{
							g.DrawLine(d1[0], d1[1], d1[0], d2[1]);
							g.DrawLine(d2[0], d2[1], d1[0], d2[1]);
							if (d2[1] == oy)
							{
								g.DrawLine(d1[0] + 1, d1[1], d1[0] + 1, d2[1]);
								g.DrawLine(d1[0], d2[1] + 1, d2[0], d2[1] + 1);
							}
							else
							{
								g.DrawLine(d1[0] - 1, d1[1], d1[0] - 1, d2[1]);
								g.DrawLine(d1[0], d2[1] - 1, d2[0], d2[1] - 1);
							}
						}
						else
						{
							g.DrawLine(d1[0], d1[1], d2[0], d1[1]);
							g.DrawLine(d2[0], d2[1], d2[0], d1[1]);
							if (d1[1] == oy)
							{
								g.DrawLine(d2[0] - 1, d1[1], d2[0] - 1, d2[1]);
								g.DrawLine(d1[0], d1[1] + 1, d2[0], d1[1] + 1);
							}
							else
							{
								g.DrawLine(d2[0] + 1, d1[1], d2[0] + 1, d2[1]);
								g.DrawLine(d1[0], d1[1] - 1, d2[0], d1[1] - 1);
							}
						}
						continue;
					}
					g.DrawLine(d1[0], d1[1], d2[0], d2[1]);
					if (d1[0] == d2[0])
					{
						if (d1[0] == ox)
						{
							g.DrawLine(d1[0] + 1, d1[1], d2[0] + 1, d2[1]);
						}
						else
						{
							g.DrawLine(d1[0] - 1, d1[1], d2[0] - 1, d2[1]);
						}
					}
					else if (d1[1] == d2[1])
					{
						if (d1[1] == oy)
						{
							g.DrawLine(d1[0], d1[1] + 1, d2[0], d2[1] + 1);
						}
						else
						{
							g.DrawLine(d1[0], d1[1] - 1, d2[0], d2[1] - 1);
						}
					}
				}
			}
		}

		public void GetPoint(int ox, int oy, int cnt, int[] d, int W, int H)
		{
			int num = (4 + cnt) % (W * 2 + H * 2);
			if (num < W)
			{
				d[0] = ox + num;
				d[1] = oy;
			}
			else if (num < W + H)
			{
				d[0] = ox + W;
				d[1] = oy + num - W;
			}
			else if (num < W * 2 + H)
			{
				d[0] = ox + W - (num - (W + H));
				d[1] = oy + H;
			}
			else
			{
				d[0] = ox;
				d[1] = oy + H - (num - (W * 2 + H));
			}
		}

		public void SetEditForPos(bool isSoccerForm)
		{
			isSoccerForm_ = isSoccerForm;

TeamData teamData, teamData2;
int[][] array, array2;
FormationData formationData, formationData2;
int num6, num, num2, num3, num4, num5;
teamData = apdat_.tData_[gm_.teamId_[0]];
			teamData2 = apdat_.tData_[gm_.teamId_[1]];
			array = null;
			array2 = null;
			formationData = (FormationData)editTest_formation.ElementAt(et_cur[1]);
			array = formationData.formation_;
			formationData2 = apdat_.formationData_[teamData2.formation_];
			array2 = formationData2.formation_;
			if (phase_ == 11 || phase_ == 12)
			{
				array = apdat_.formationData_[selectFormationId_].formation_;
			}




			num6 = num = num2 = num3 = num4 = num5 = 0;
			if (phase_ == 14 || phase_ == 15 || phase_ == 16)
			{
				SetPlayerParam(teamData2.warP_paramPhysical_, teamData2.warP_paramPhysical_);
			}

			if (!isSoccerForm)
			{
				num6 = EDIT_POS_FOR_F[0][0];
				num = EDIT_POS_FOR_F[0][1];
				if (phase_ == 1)
				{
					num = EDIT_POS_FOR_F[1][1];
				}
				if (phase_ == 9)
				{
					num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_F[0][1], EDIT_POS_FOR_F[1][1], true);
				}
				if (phase_ == 11)
				{
					num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_F[1][1], EDIT_POS_FOR_F[0][1], true);
				}
				if (phase_ == 12)
				{
					num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_F[1][1], EDIT_POS_FOR_F[0][1], true);
				}
				if (phase_ == 4)
				{
					num = EDIT_POS_FOR_F[1][1];
				}
				if (phase_ == 6)
				{
					num = EDIT_POS_FOR_M[1][1];
				}
				if (phase_ == 7)
				{
					num = EDIT_POS_FOR_M[1][1];
				}
				if (phase_ == 8)
				{
					num = EDIT_POS_FOR_M[1][1];
				}
				if (phase_ == 10)
				{
					num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_M[0][1], EDIT_POS_FOR_M[1][1], true);
				}
				if (phase_ == 13)
				{
					num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_M[1][1], EDIT_POS_FOR_M[0][1], true);
				}
			}
			else
			{
				num6 = EDIT_POS_FOR_F[1][0];
				num = EDIT_POS_FOR_F[1][1];
				num5 = EDIT_POS_FOR_F[1][1];
				if (phase_ == 6)
				{
					num = EDIT_POS_FOR_M[1][1];
				}
				else if (phase_ == 7)
				{
					num = EDIT_POS_FOR_M[1][1];
				}
				else if (phase_ == 8)
				{
					num = EDIT_POS_FOR_M[1][1];
				}
				else if (phase_ == 10)
				{
					num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_F[1][1], EDIT_POS_FOR_M[1][1], true);
				}
				else if (phase_ == 13)
				{
					num = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_M[1][1], EDIT_POS_FOR_F[1][1], true);
				}
				if (phase_ == 14)
				{
					num6 = EDIT_POS_FOR_ENE[1][0];
					num = EDIT_POS_FOR_ENE[1][1];
					num4 = EDIT_POS_ENE_FOR[1][0];
					num5 = EDIT_POS_ENE_FOR[1][1];
				}
				else if (phase_ == 15)
				{
					num6 = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_ENE[0][0], EDIT_POS_FOR_ENE[1][0], true);
					num4 = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_ENE_FOR[0][0], EDIT_POS_ENE_FOR[1][0], true);
				}
				else if (phase_ == 16)
				{
					num6 = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_FOR_ENE[1][0], EDIT_POS_FOR_ENE[0][0], true);
					num4 = GameUtil.RateConvert(phaseCnt_, 0, 4, EDIT_POS_ENE_FOR[1][0], EDIT_POS_ENE_FOR[0][0], true);
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (i == 0)
				{
					editForPos[i][0] = -1;
					editForPos[i][1] = -1;
					editForPos[i][2] = i;
				}
				else
				{
					editForPos[i][0] = array[i][0];
					editForPos[i][1] = array[i][1];
					editForPos[i][2] = i;
				}
				if (i == 0)
				{
					num2 = num6 + 8;
					num3 = num + 47;
				}
				else
				{
					num2 = GameUtil.RateConvert(editForPos[i][1], 0, 9, num6 + 214 - 1 - 35, num6 + 35, true);
					num3 = GameUtil.RateConvert(editForPos[i][0], 0, 9, num + 2, num + 94 - 1 - 2, true);
					num2 -= 8;
					num3 += 5;
				}
				editForPos[i][0] = num2;
				editForPos[i][1] = num3;
			}
			for (int j = 0; j < array2.Length; j++)
			{
				if (j == 0)
				{
					editForPosEnemy[j][0] = -1;
					editForPosEnemy[j][1] = -1;
					editForPosEnemy[j][2] = j;
				}
				else
				{
					editForPosEnemy[j][0] = array2[j][0];
					editForPosEnemy[j][1] = array2[j][1];
					editForPosEnemy[j][2] = j;
				}
				if (j == 0)
				{
					num2 = num4 + 214 - 8;
					num3 = num5 + 47;
				}
				else
				{
					num2 = GameUtil.RateConvert(editForPosEnemy[j][1], 0, 9, num4 + 35, num4 + 214 - 1 - 35, true);
					num3 = GameUtil.RateConvert(editForPosEnemy[j][0], 0, 9, num5 + 94 - 1 - 2, num5 + 2, true);
					num2 += 8;
					num3 -= 5;
				}
				editForPosEnemy[j][0] = num2;
				editForPosEnemy[j][1] = num3;
			}
			if (phase_ == 8)
			{
				int[] array3 = (int[])editTest_team[et_cur[5] / 10].ElementAt(et_cur[5] % 10);
				int[] array4 = (int[])editTest_team[et_cur[6] / 10].ElementAt(et_cur[6] % 10);
				int num7 = array3[2];
				int num8 = array4[2];
				int num9 = editForPos[num7][0];
				int num10 = editForPos[num8][0];
				int num11 = editForPos[num7][1];
				int num12 = editForPos[num8][1];
				editForPos[num7][0] = GameUtil.RateConvert(phaseCnt_, 0, 6, num9, num10, true);
				editForPos[num7][1] = GameUtil.RateConvert(phaseCnt_, 0, 6, num11, num12, true);
				editForPos[num8][0] = GameUtil.RateConvert(phaseCnt_, 0, 6, num10, num9, true);
				editForPos[num8][1] = GameUtil.RateConvert(phaseCnt_, 0, 6, num12, num11, true);
			}
			else if (phase_ == 4)
			{
				int num13 = -1;
				int num14 = -1;
				for (int k = 1; k < array.Length; k++)
				{
					if (array[k][0] == et_cur[2] / 10 && array[k][1] == et_cur[2] % 10)
					{
						num13 = k;
					}
					if (array[k][0] == et_cur[3] / 10 && array[k][1] == et_cur[3] % 10)
					{
						num14 = k;
					}
				}

int num18, num15, num16, num17;


				num18 = num15 = num16 = num17 = 0;

				if (num13 != -1)
				{
					num18 = editForPos[num13][0];
					num16 = editForPos[num13][1];
				}
				else
				{
					num2 = GameUtil.RateConvert(et_cur[2] % 10, 0, 9, num6 + 214 - 1 - 35, num6 + 35, true);
					num3 = GameUtil.RateConvert(et_cur[2] / 10, 0, 9, num + 2, num + 94 - 1 - 2, true);
					num2 -= 8;
					num3 += 5;
					num18 = num2;
					num16 = num3;
				}
				if (num14 != -1)
				{
					num15 = editForPos[num14][0];
					num17 = editForPos[num14][1];
				}
				else
				{
					num2 = GameUtil.RateConvert(et_cur[3] % 10, 0, 9, num6 + 214 - 1 - 35, num6 + 35, true);
					num3 = GameUtil.RateConvert(et_cur[3] / 10, 0, 9, num + 2, num + 94 - 1 - 2, true);
					num2 -= 8;
					num3 += 5;
					num15 = num2;
					num17 = num3;
				}
				if (num13 != -1)
				{
					editForPos[num13][0] = GameUtil.RateConvert(phaseCnt_, 0, 6, num18, num15, true);
					editForPos[num13][1] = GameUtil.RateConvert(phaseCnt_, 0, 6, num16, num17, true);
				}
				if (num14 != -1)
				{
					editForPos[num14][0] = GameUtil.RateConvert(phaseCnt_, 0, 6, num15, num18, true);
					editForPos[num14][1] = GameUtil.RateConvert(phaseCnt_, 0, 6, num17, num16, true);
				}
			}
			for (int l = 0; l < editForPos.Length - 1; l++)
			{
				for (int num19 = editForPos.Length - 1; num19 > l; num19--)
				{
					if (editForPos[l][1] > editForPos[num19][1])
					{
						int num20 = editForPos[num19][0];
						editForPos[num19][0] = editForPos[l][0];
						editForPos[l][0] = num20;
						num20 = editForPos[num19][1];
						editForPos[num19][1] = editForPos[l][1];
						editForPos[l][1] = num20;
						num20 = editForPos[num19][2];
						editForPos[num19][2] = editForPos[l][2];
						editForPos[l][2] = num20;
					}
				}
			}
			for (int m = 0; m < editForPosEnemy.Length - 1; m++)
			{
				for (int num21 = editForPosEnemy.Length - 1; num21 > m; num21--)
				{
					if (editForPosEnemy[m][1] > editForPosEnemy[num21][1])
					{
						int num22 = editForPosEnemy[num21][0];
						editForPosEnemy[num21][0] = editForPosEnemy[m][0];
						editForPosEnemy[m][0] = num22;
						num22 = editForPosEnemy[num21][1];
						editForPosEnemy[num21][1] = editForPosEnemy[m][1];
						editForPosEnemy[m][1] = num22;
						num22 = editForPosEnemy[num21][2];
						editForPosEnemy[num21][2] = editForPosEnemy[m][2];
						editForPosEnemy[m][2] = num22;
					}
				}
			}
		}

		public void SetTrainingData3()
		{
			pDataIdOlderList_ = new int[memberPDataist_.Size()];
			for (int i = 0; i < pDataIdOlderList_.Length; i++)
			{
				PlayerData playerData = (PlayerData)memberPDataist_.ElementAt(i);
				pDataIdOlderList_[i] = playerData.id_;
			}
			save_tAreaRootNum_ = tAreaRootNum_;
			tAreas_ = JTool.MakeArray<TArea[][]>(new int[2] { save_tAreaRootNum_, 4 });
			tModeList_ = JTool.MakeArray<int[][]>(new int[2] { save_tAreaRootNum_, 4 });
			for (int j = 0; j < tAreas_.Length; j++)
			{
				for (int k = 0; k < tAreas_[j].Length; k++)
				{
					tAreas_[j][k] = new TArea();
					tAreas_[j][k].Init(j, k);
				}
			}
			SetTrainingCommonMethod(true);
			trainig_appearWait = 0;
			for (int l = 0; l < tAreas_.Length; l++)
			{
				for (int m = 0; m < tAreas_[l].Length; m++)
				{
					tAreas_[l][m].ChangeState(11);
				}
			}
		}

		public void SetTrainingCommonMethod(bool isInit)
		{
			for (int i = 0; i < tenants_.Length; i++)
			{
				if (tenants_[i].useTPlayers_.Size() > 0)
				{
					tenants_[i].useTPlayers_.RemoveAllElements();
				}
			}
			if (isInit)
			{
				tPlayers_ = new TPlayer[pDataIdOlderList_.Length];
				for (int j = 0; j < tPlayers_.Length; j++)
				{
					PlayerData playerData = apdat_.pData_[pDataIdOlderList_[j]];
					tPlayers_[j] = new TPlayer(1);
					tPlayers_[j].Init_training(j, playerData.id_);
					int num = memberPDataist_.IndexOf(playerData);
					if (num < 11)
					{
						tPlayers_[j].trainingPriority_ = GameUtil.Random(40);
					}
					else
					{
						tPlayers_[j].trainingPriority_ = GameUtil.Random(100);
					}
					playerData.SetGameData(tPlayers_[j]);
					if (BitUtil.Check(playerData.flag_, 2))
					{
						tPlayers_[j].role_ = 0;
					}
					else
					{
						tPlayers_[j].role_ = 3;
					}
				}
				if (restTPlayers_.Size() > 0)
				{
					restTPlayers_.RemoveAllElements();
				}
			}
			int num2 = 0;
			int num3 = 0;
			for (int k = 0; k < pDataIdOlderList_.Length; k++)
			{
				PlayerData playerData2 = apdat_.pData_[pDataIdOlderList_[k]];
				if (BitUtil.Check(playerData2.flag_, 2))
				{
					if (k < pDataIdOlderList_.Length / 2 || save_tAreaRootNum_ == 1)
					{
						num2++;
					}
					else
					{
						num3++;
					}
				}
			}
			for (int l = 0; l < tModeList_.Length; l++)
			{
				for (int m = 0; m < tModeList_[l].Length; m++)
				{
					if ((m == 1)) { tModeList_[l][m] = 4;
continue; } else if ((m == 0)) { tModeList_[l][m] = 5;
continue; }
					int num4 = num2;
					if (l == 1)
					{
						num4 = num3;
					}
					int num5 = 0;
					if (num4 > 0)
					{
						num5 = GameUtil.Random(TMODE_KEEPER.Length);
						tModeList_[l][m] = TMODE_KEEPER[num5];
						if (tModeList_[l][m] == 3)
						{
							if (l == 0)
							{
								num2--;
							}
							else
							{
								num3--;
							}
						}
					}
					else
					{
						num5 = GameUtil.Random(TMODE_NONKEEPER.Length);
						tModeList_[l][m] = TMODE_NONKEEPER[num5];
					}
				}
			}
			for (int n = 0; n < tAreas_.Length; n++)
			{
				for (int num6 = 0; num6 < tAreas_[n].Length; num6++)
				{
					tAreas_[n][num6].SetTrainingArea(tModeList_[n][num6]);
				}
			}
			Vector[] array = new Vector[2]
			{
				new Vector(),
				new Vector()
			};
			int[] array2 = new int[2];
			for (int num7 = 0; num7 < tPlayers_.Length; num7++)
			{
				TPlayer obj = tPlayers_[num7];
				if (num7 < tPlayers_.Length / 2 || save_tAreaRootNum_ == 1)
				{
					array[0].AddElement(obj);
				}
				else
				{
					array[1].AddElement(obj);
				}
			}
			array2[0] = array[0].Size();
			array2[1] = array[1].Size();
			for (int num8 = 0; num8 < tModeList_.Length; num8++)
			{
				for (int num9 = 0; num9 < tModeList_[num8].Length; num9++)
				{
					if (tModeList_[num8][num9] != 3)
					{
						continue;
					}
					for (int num10 = array[num8].Size() - 1; num10 >= 0; num10--)
					{
						TPlayer tPlayer = (TPlayer)array[num8].ElementAt(num10);
						if (tPlayer.role_ == 0)
						{
							tAreas_[num8][num9].tGroupAreas_[0].tPlayerList_.AddElement(tPlayer);
							array[num8].RemoveElementAt(num10);
							break;
						}
					}
				}
			}
			for (int num11 = 0; num11 < array.Length; num11++)
			{
				for (int num12 = 0; num12 < array[num11].Size() - 1; num12++)
				{
					for (int num13 = array[num11].Size() - 1; num13 > num12; num13--)
					{
						TPlayer tPlayer2 = (TPlayer)array[num11].ElementAt(num12);
						TPlayer tPlayer3 = (TPlayer)array[num11].ElementAt(num13);
						if (tPlayer3.trainingPriority_ < tPlayer2.trainingPriority_)
						{
							array[num11].SetElementAt(tPlayer3, num12);
							array[num11].SetElementAt(tPlayer2, num13);
						}
					}
				}
			}
			for (int num14 = 0; num14 < tModeList_.Length; num14++)
			{
				int num15 = 0;
				int num16 = 0;
				if (selectTrainingPolicy_ == 0)
				{
					num16 = array[num14].Size() * 50 / 100;
				}
				else if (selectTrainingPolicy_ == 1)
				{
					num16 = array[num14].Size() * 100 / 100;
				}
				else if (selectTrainingPolicy_ == 2)
				{
					num16 = array[num14].Size() * 20 / 100;
				}
				for (int num17 = 0; num17 < tModeList_[num14].Length; num17++)
				{
					if (tModeList_[num14][num17] == 4 || tModeList_[num14][num17] == 5)
					{
						continue;
					}
					int areaMaxNum = tAreas_[num14][num17].GetAreaMaxNum(tModeList_[num14][num17]);
					for (int num18 = 0; num18 < areaMaxNum; num18++)
					{
						if (array[num14].Size() <= 0)
						{
							break;
						}
						int areaMaxNum2 = TGroupArea.GetAreaMaxNum(tModeList_[num14][num17]);
						for (int num19 = 0; num19 < tAreas_[num14][num17].tGroupAreas_.Length; num19++)
						{
							if (tAreas_[num14][num17].tGroupAreas_[num19].tPlayerList_.Size() < areaMaxNum2)
							{
								tAreas_[num14][num17].tGroupAreas_[num19].tPlayerList_.AddElement(array[num14].FirstElement());
								array[num14].RemoveElementAt(0);
								num15++;
								break;
							}
						}
						if (num15 >= num16)
						{
							break;
						}
					}
					if (num15 >= num16)
					{
						break;
					}
				}
			}
			for (int num20 = 0; num20 < tModeList_.Length; num20++)
			{
				for (int num21 = 0; num21 < tModeList_[num20].Length; num21++)
				{
					for (int num22 = 0; num22 < tAreas_[num20][num21].tGroupAreas_.Length; num22++)
					{
						if ((tModeList_[num20][num21] == 0 || tModeList_[num20][num21] == 1) && tAreas_[num20][num21].tGroupAreas_[num22].tPlayerList_.Size() % 2 == 1)
						{
							TPlayer obj2 = (TPlayer)tAreas_[num20][num21].tGroupAreas_[num22].tPlayerList_.FirstElement();
							tAreas_[num20][num21].tGroupAreas_[num22].tPlayerList_.RemoveElementAt(0);
							array[num20].AddElement(obj2);
						}
						if (tModeList_[num20][num21] == 3 && tAreas_[num20][num21].tGroupAreas_[num22].tPlayerList_.Size() == 1)
						{
							TPlayer obj3 = (TPlayer)tAreas_[num20][num21].tGroupAreas_[num22].tPlayerList_.FirstElement();
							tAreas_[num20][num21].tGroupAreas_[num22].tPlayerList_.RemoveElementAt(0);
							array[num20].AddElement(obj3);
						}
					}
				}
			}
			int num23 = 0;
			for (int num24 = 0; num24 < array.Length; num24++)
			{
				int num25 = 0;
				int num26 = 0;
				if (selectTrainingPolicy_ == 0)
				{
					num26 = array2[num24] * 5 / 10;
				}
				else if (selectTrainingPolicy_ == 2)
				{
					num26 = array2[num24] * 8 / 10;
				}
				if (selectTrainingPolicy_ == 0)
				{
					num26 = array2[num24] * 50 / 100;
				}
				else if (selectTrainingPolicy_ == 1)
				{
					num26 = array2[num24] * 0 / 100;
				}
				else if (selectTrainingPolicy_ == 2)
				{
					num26 = array2[num24] * 80 / 100;
				}
				for (int num27 = array[num24].Size() - 1; num27 >= 0; num27--)
				{
					TPlayer obj4 = (TPlayer)array[num24].ElementAt(num27);
					switch ((num25 >= num26) ? GameUtil.Random(2) : 0)
					{
					case 0:
						tAreas_[num24][0].tGroupAreas_[0].tPlayerList_.AddElement(obj4);
						break;
					case 1:
						tAreas_[num24][1].tGroupAreas_[0].tPlayerList_.AddElement(obj4);
						break;
					}
					array[num24].RemoveElementAt(num27);
				}
			}
			for (int num28 = 0; num28 < tAreas_.Length; num28++)
			{
				for (int num29 = 0; num29 < tAreas_[num28].Length; num29++)
				{
					int num30 = 0;
					for (int num31 = 0; num31 < tAreas_[num28][num29].tGroupAreas_.Length; num31++)
					{
						num30 += tAreas_[num28][num29].tGroupAreas_[num31].tPlayerList_.Size();
						for (int num32 = 0; num32 < tAreas_[num28][num29].tGroupAreas_[num31].tPlayerList_.Size(); num32++)
						{
							TPlayer tPlayer4 = (TPlayer)tAreas_[num28][num29].tGroupAreas_[num31].tPlayerList_.ElementAt(num32);
							tPlayer4.SetTrainingId(num28, num29, num31, num32);
							tPlayer4.tGroupArea_ = tAreas_[num28][num29].tGroupAreas_[num31];
							tPlayer4.team_id_ = 1;
							if (tAreas_[num28][num29].pId_ == 3)
							{
								tPlayer4.team_id_ = 0;
							}
						}
					}
				}
			}
			int num33 = 0;
			for (int num34 = 0; num34 < tAreas_.Length; num34++)
			{
				for (int num35 = 0; num35 < tAreas_[num34].Length; num35++)
				{
					for (int num36 = 0; num36 < tAreas_[num34][num35].tGroupAreas_.Length; num36++)
					{
						num33 += tAreas_[num34][num35].GetAreaBallNum(tAreas_[num34][num35].tMode_);
					}
				}
			}
			tBalls_ = new TBall[num33];
			for (int num37 = 0; num37 < tBalls_.Length; num37++)
			{
				tBalls_[num37] = new TBall(num37);
				tBalls_[num37].Init();
			}
			int num38 = 0;
			for (int num39 = 0; num39 < tAreas_.Length; num39++)
			{
				for (int num40 = 0; num40 < tAreas_[num39].Length; num40++)
				{
					for (int num41 = 0; num41 < tAreas_[num39][num40].tGroupAreas_.Length; num41++)
					{
						int areaBallNum = tAreas_[num39][num40].GetAreaBallNum(tAreas_[num39][num40].tMode_);
						for (int num42 = 0; num42 < areaBallNum; num42++)
						{
							tAreas_[num39][num40].tGroupAreas_[num41].tBallList_.AddElement(tBalls_[num38]);
							tBalls_[num38].tGroupArea_ = tAreas_[num39][num40].tGroupAreas_[num41];
							num38++;
						}
					}
				}
			}
		}

		public bool SetTenantIn(TPlayer tPlayer)
		{
			tPlayer.useTenantCnt_ = 0;
			if (tenantList.Size() > 0)
			{
				tenantList.RemoveAllElements();
			}
			for (int i = 0; i < tenants_.Length; i++)
			{
				if (tenants_[i].state_ != 1) { continue; }
if (tenants_[i].useTPlayers_.Size() > 3) { continue; }
tenantList.AddElement(tenants_[i]);
			}
			if (tenantList.Size() == 0)
			{
				for (int j = 0; j < tenants_.Length; j++)
				{
					if (tenants_[j].state_ != 1) { continue; }
tenantList.AddElement(tenants_[j]);
				}
				if (tenantList.Size() == 0)
				{
					if (restTPlayers_.IndexOf(tPlayer) == -1)
					{
						restTPlayers_.AddElement(tPlayer);
					}
					return false;
				}
			}
			for (int k = 0; k < tenantList.Size() - 1; k++)
			{
				for (int num = tenantList.Size() - 1; num > k; num--)
				{
					Tenant tenant = (Tenant)tenantList.ElementAt(k);
					Tenant tenant2 = (Tenant)tenantList.ElementAt(num);
					TenantData tenantData = apdat_.tenantData_[tenant.tenantDataId_];
					TenantData tenantData2 = apdat_.tenantData_[tenant2.tenantDataId_];
					if (tenantData2.charm_ > tenantData.charm_)
					{
						tenantList.SetElementAt(tenant2, k);
						tenantList.SetElementAt(tenant, num);
					}
				}
			}
			int[] array = new int[tenantList.Size()];
			int num2 = 0;
			for (int l = 0; l < tenantList.Size(); l++)
			{
				Tenant tenant3 = (Tenant)tenantList.ElementAt(l);
				num2 = (array[l] = num2 + apdat_.tenantData_[tenant3.tenantDataId_].charm_);
			}
			int num3 = GameUtil.Random(num2);
			for (int m = 0; m < tenantList.Size(); m++)
			{
				Tenant tenant4 = (Tenant)tenantList.ElementAt(m);
				if (num3 < array[m])
				{
					tenant4.useTPlayers_.AddElement(tPlayer);
					break;
				}
			}
			return true;
		}

		public void ChangeTraining()
		{
			for (int i = 0; i < tAreas_.Length; i++)
			{
				for (int j = 0; j < tAreas_[i].Length; j++)
				{
					tAreas_[i][j].ChangeState(13);
				}
			}
			for (int k = 0; k < tPlayers_.Length; k++)
			{
				tPlayers_[k].flag_ = BitUtil.Off(tPlayers_[k].flag_, 131072);
			}
			SetTrainingCommonMethod(false);
			for (int l = 0; l < tAreas_.Length; l++)
			{
				for (int m = 0; m < tAreas_[l].Length; m++)
				{
					tAreas_[l][m].ChangeState(14);
				}
			}
		}

		public void Update_training()
		{
			for (int i = 0; i < tAreas_.Length; i++)
			{
				for (int j = 0; j < tAreas_[i].Length; j++)
				{
					tAreas_[i][j].Update();
				}
			}
			tp_svisor_.Update_trainings();
		}

		public void Draw_training(Graphics g)
		{
			for (int i = 0; i < tAreas_.Length; i++)
			{
				for (int j = 0; j < tAreas_[i].Length; j++)
				{
					tAreas_[i][j].Draw(g);
				}
			}
			tp_svisor_.Draw_svisor(g);
		}

		public void Init_bus()
		{
			busPos_.Set(MapMain.MOVE_POINT[0]);
			busState_ = 0;
			busMovePoint_.AddElement(MapMain.MOVE_POINT[1]);
			busCnt = 0;
			GameForm.busgather_ph_cnt = 0;
		}

		public void Update_bus()
		{
if (busState_ == 0) {
busCnt = (busCnt + 1) % int.MaxValue;
int[] collisionData = GetCollisionData(0, 5);
int[] collisionData2 = GetCollisionData(1, 5);
busGoalPos_.Set((Vector3D)busMovePoint_.FirstElement());
if (GameUtil.ChRectHitRect(busPos_.x_ + (float)collisionData[0], busPos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], busGoalPos_.x_ + (float)collisionData2[0], busGoalPos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true)) {
busMovePoint_.RemoveElementAt(0);
if (busMovePoint_.Size() == 0) {
busState_ = 1;
SetTrainingData3();
}
} else {
float num = 10f;
float num2 = (float)JMath.Sqrt((busGoalPos_.x_ - busPos_.x_) * (busGoalPos_.x_ - busPos_.x_) + (busGoalPos_.z_ - busPos_.z_) * (busGoalPos_.z_ - busPos_.z_));
busV_.x_ = (busGoalPos_.x_ - busPos_.x_) * num / num2;
busV_.z_ = (busGoalPos_.z_ - busPos_.z_) * num / num2;
busPos_.x_ += busV_.x_;
busPos_.z_ += busV_.z_;
Camera.ConvertGlobalToScreen(busV_, busSDirect);
busSDirect[0] -= Camera.MAP_SCREEN_BASE_POS[0];
busSDirect[1] -= Camera.MAP_SCREEN_BASE_POS[1];
if (busSDirect[0] > 0) {
if (busSDirect[1] > 0) {
busDirect_ = 0;
} else {
busDirect_ = 1;
}
} else {
if (busSDirect[1] > 0) {
busDirect_ = 3;
} else {
busDirect_ = 2;
}
}
}
}
return;
}

		public void Draw_bus(Graphics g)
		{
			Camera.ConvertGlobalToCamera(busPos_, busCPos);
			apdat_.InsertGameObject(8, 0, busCPos[1] + 10, 42, 38, busDirect_, busCPos[0], busCPos[1], false, 0);
			int num = busCnt % 12 / 6;
			if (num == 1)
			{
				int frame = 4;
				if (busDirect_ == 0 || busDirect_ == 1)
				{
					frame = 5;
				}
				apdat_.InsertGameObject(8, 0, busCPos[1] + 10 + 1, 42, 38, frame, busCPos[0], busCPos[1], false, 0);
			}
		}

		public void SetPlayerParam(int[] p1, int[] p2)
		{
			for (int i = 0; i < playerParam1.Length; i++)
			{
				playerParam1[i] = p1[i];
				playerParam2[i] = p2[i];
			}
		}

		public void DrawGraph_menuElement(Graphics g, int ox, int oy, int[] param1, int[] param2, int frame)
		{
			for (int i = 0; i < MENUGRAPH_POINTPOS.Length; i++)
			{
			}
			g.SetColor(Graphics.GetColorOfRGB(214, 214, 214));
			for (int j = 0; j < 5; j++)
			{
				int num = ox + MENUGRAPH_POINTPOS[5][0];
				int num2 = oy + MENUGRAPH_POINTPOS[5][1];
				int num3 = ox + MENUGRAPH_POINTPOS[j][0];
				int num4 = oy + MENUGRAPH_POINTPOS[j][1];
				g.DrawLine(num, num2, num3, num4);
			}
			g.SetColor(Graphics.GetColorOfRGB(160, 160, 160));
			for (int k = 0; k < 5; k++)
			{
				int num = ox + MENUGRAPH_POINTPOS[k][0];
				int num2 = oy + MENUGRAPH_POINTPOS[k][1];
				int num3 = ox + MENUGRAPH_POINTPOS[(k + 1) % 5][0];
				int num4 = oy + MENUGRAPH_POINTPOS[(k + 1) % 5][1];
				g.DrawLine(num, num2, num3, num4);
			}
			int c1max = 25;
			for (int l = 0; l < 5; l++)
			{
				int x = GameUtil.RateConvert(frame, 0, c1max, param1[l], param2[l], true);
				int x2 = GameUtil.RateConvert(frame, 0, c1max, param1[(l + 1) % 5], param2[(l + 1) % 5], true);
				int num = ox + GameUtil.RateConvert(x, 0, 1000, MENUGRAPH_POINTPOS[l][0] / 5, MENUGRAPH_POINTPOS[l][0], true);
				int num2 = oy + GameUtil.RateConvert(x, 0, 1000, MENUGRAPH_POINTPOS[l][1] / 5, MENUGRAPH_POINTPOS[l][1], true);
				int num3 = ox + GameUtil.RateConvert(x2, 0, 1000, MENUGRAPH_POINTPOS[(l + 1) % 5][0] / 5, MENUGRAPH_POINTPOS[(l + 1) % 5][0], true);
				int num4 = oy + GameUtil.RateConvert(x2, 0, 1000, MENUGRAPH_POINTPOS[(l + 1) % 5][1] / 5, MENUGRAPH_POINTPOS[(l + 1) % 5][1], true);
				for (int num5 = 4; num5 >= 0; num5--)
				{
					if (num5 > 0)
					{
						num += MENUGRAPH_OFF[num5 - 1][0];
						num2 += MENUGRAPH_OFF[num5 - 1][1];
						num3 += MENUGRAPH_OFF[num5 - 1][0];
						num4 += MENUGRAPH_OFF[num5 - 1][1];
					}
					if (num5 == 0)
					{
						g.SetColor(Graphics.GetColorOfRGB(199, 0, 46));
					}
					else
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 128, 100));
					}
					g.DrawLine(num, num2, num3, num4);
				}
			}
		}

		public int GetParamPlusNum(int param_now, int param_max)
		{
			if (param_max - param_now >= 400)
			{
				return 3;
			}
			if (param_max - param_now >= 150)
			{
				return 2;
			}
			if (param_max - param_now >= 30)
			{
				return 1;
			}
			return 0;
		}

		public int GetRankIndex()
		{
			return GetInstance().teamEvaRank_;
		}

		public int GetRankIndex(int value)
		{
			for (int i = 0; i < RANK_VALUE.Length; i++)
			{
				if (value < RANK_VALUE[i])
				{
					return i;
				}
			}
			return RANK_VALUE.Length - 1;
		}

		public void AddResearchPoint(int researchId, int add)
		{
			researchPoint_[researchId] = AppData.Clamp(researchPoint_[researchId] + add, 0, 999);
		}

		public void SubResearchPoint(int researchId, int sub)
		{
			researchPoint_[researchId] = AppData.Clamp(researchPoint_[researchId] - sub, 0, 999);
		}

		public bool ChResearchPoin(int researchId, int cost)
		{
			if (researchPoint_[researchId] >= cost)
			{
				return true;
			}
			return false;
		}

		public void Training_init()
		{
			for (int i = 0; i < tenants_.Length; i++)
			{
				if (tenants_[i].useTPlayers_.Size() > 0)
				{
					tenants_[i].useTPlayers_.RemoveAllElements();
				}
			}
			tAreas_ = JTool.MakeArray<TArea[][]>(new int[2]);
			tPlayers_ = new TPlayer[0];
			tBalls_ = new TBall[0];
			pDataIdOlderList_ = new int[0];
			tModeList_ = JTool.MakeArray<int[][]>(new int[2]);
			restTPlayers_ = new Vector();
			apdat_.RemoveeEffect();
		}

		public void SetMatchInfoJoin(int matchInfoJoin)
		{
			matchInfoJoin_ = matchInfoJoin;
			switch (matchInfoJoin)
			{
			}
		}

		public int GetMatchInfoJoin()
		{
			return matchInfoJoin_;
		}

		public int GetEvaWarPNameValue(float w_warP, float w_nameValue)
		{
			TeamData teamData = AppData.GetInstance().tData_[0];
			int warPotential_ = teamData.warPotential_;
			int fanEvaToCommonValue = FanActData.GetFanEvaToCommonValue(2);
			{ int recoveredReturn = (int)((w_warP * (float)warPotential_ + w_nameValue * (float)fanEvaToCommonValue) / (w_warP + w_nameValue)); return recoveredReturn; }
		}

		public float GetTicketPriceRate(int eva)
		{
			int standartTicketPrice = GetStandartTicketPrice(eva);
			{ float recoveredReturn = (float)standartTicketPrice / (float)ticketPrice_; return recoveredReturn; }
		}

		public int GetStandartTicketPrice(int eva)
		{

			int num = 0;
			if (eva < 700) { num = GameUtil.RateConvert(eva, 0, 700, 1000, 3000, true); } else { num = GameUtil.RateConvert(eva, 700, 1000, 3000, 6000, true); }
return num;
}

		public float GetTicketPriceFanBonusRate(int eva)
		{

			int standartTicketPrice = GetStandartTicketPrice(eva);
			float num = (float)standartTicketPrice / (float)ticketPrice_;
			float num2 = 1f;
			if (num >= 1f) { num2 = GameUtil.RateConvert(num, 1f, 3f, 1f, 1.5f, true); } else { num2 = GameUtil.RateConvert(num, 1f, 1f / 3f, 1f, 0.2f, true); }
return num2;
}

		public int GetIdealSalse(int eva)
		{

			int num = 0;
			if (eva < 700) { num = GameUtil.RateConvert(eva, 0, 700, 65, 12000, true); } else { num = GameUtil.RateConvert(eva, 700, 1000, 12000, 50000, true); }
return num;
}

		public float GetTicketSalseRate(int eva)
		{

			int standartTicketPrice = GetStandartTicketPrice(eva);
			float num = (float)standartTicketPrice / (float)ticketPrice_;
			float num2 = 1f;
			if (num >= 1f) { num2 = GameUtil.RateConvert(num, 1f, 3f, 1f, 0.8f, true); } else { num2 = GameUtil.RateConvert(num, 1f, 1f / 3f, 1f, 0.6f, true); }
return num2;
}

		public void RotateTestPlayParam()
		{
			for (int i = 0; i < 9; i++)
			{
				if ((i == 0)) { AddTestPlayerParam(i, (int)apdat_.save_.lngs[0]);
continue; } else if ((i == 4) || (i == 5) || (i == 6) || (i == 7)) { AddTestPlayerParam(i, researchPoint_[i - 4]);
continue; } else if ((i == 1) || (i == 2) || (i == 3)) { continue; }
				if (i == 8)
				{
					TeamData teamData = apdat_.tData_[0];
					AddTestPlayerParam(i, teamData.warPotential_);
				}
			}
			for (int num = tpplay_stock.Length - 1; num >= 1; num--)
			{
				for (int j = 0; j < tpplay_stock[num].Length; j++)
				{
					tpplay_stock[num][j] = tpplay_stock[num - 1][j];
				}
			}
			for (int k = 0; k < tpplay_stock[0].Length; k++)
			{
				tpplay_stock[0][k] = 0;
			}
		}

		public void AddTestPlayerParam(int tppId, int addValue)
		{
			tpplay_stock[0][tppId] += addValue;
		}

		public void AddTPlayBattleRecode(int teamId, int user_score, int enemy_score)
		{
			tpplay_battleNum++;
			int num = teamId * 10000 + user_score * 100 + enemy_score;
			int[] obj = new int[2] { tpplay_battleNum, num };
			tpplay_battleRecode.AddElement(obj);
			if (tpplay_battleRecode.Size() >= 50)
			{
				tpplay_battleRecode.RemoveElementAt(0);
			}
		}

		public void OnEventFlg(int id)
		{
			eventActFlg_ = BitUtil.On(eventActFlg_, id);
		}

		public bool ChEventFlg(int id)
		{
			if (BitUtil.Check(eventActFlg_, id))
			{
				return true;
			}
			return false;
		}

		public void SetNegotiateResult(int init_value, int difficult)
		{
			int evaWarPNameValue = GetEvaWarPNameValue(0.2f, 0.8f);
			int num = evaWarPNameValue / 10 - difficult;
			int num2 = 0;
			if (num >= 0) { num2 = GameUtil.RateConvert(num, 0, 20, 60, 90, true); } else { num2 = GameUtil.RateConvert(num, -20, 0, 11, 60, true); }
			int num3 = GameUtil.RateConvert(num, -20, 20, 30, 45, true);
			int num4 = GameUtil.RateConvert(num, -20, 20, 10, 20, true);
			int num5 = 0;
			for (int i = 0; i < NEGOTIATE_RESULT_TH.Length; i++)
			{
				if (GameUtil.Random(100) < num2)
				{
					NEGOTIATE_RESULT_TH[i] = 1;
					NEGOTIATE_RESULT_VALUE[i] = num3;
					num5++;
				}
				else
				{
					NEGOTIATE_RESULT_TH[i] = 0;
					NEGOTIATE_RESULT_VALUE[i] = num4;
				}
			}
			NEGOTIATE_RESULT_DISPVALUE[0] = init_value;
			NEGOTIATE_RESULT_DISPVALUE[1] = NEGOTIATE_RESULT_DISPVALUE[0] + NEGOTIATE_RESULT_VALUE[0];
			NEGOTIATE_RESULT_DISPVALUE[2] = NEGOTIATE_RESULT_DISPVALUE[1] + NEGOTIATE_RESULT_VALUE[1];
			NEGOTIATE_RESULT_DISPVALUE[3] = NEGOTIATE_RESULT_DISPVALUE[2] + NEGOTIATE_RESULT_VALUE[2];
			for (int j = 0; j < NEGOTIATE_RESULT_DISPVALUE.Length; j++)
			{
				NEGOTIATE_RESULT_DISPVALUE[j] = AppData.ClampMax(NEGOTIATE_RESULT_DISPVALUE[j], 100);
			}
			if (NEGOTIATE_RESULT_DISPVALUE[3] >= 100)
			{
				NEGOTIATE_RESULT = 1;
			}
			for (int k = 0; k < EF_COMMENT_SETTING.Length; k++)
			{
				if (k == 0)
				{
					EF_COMMENT_SETTING[k][0] = 0;
				}
				else
				{
					EF_COMMENT_SETTING[k][0] = GameUtil.Random(2);
				}
				EF_COMMENT_SETTING[k][1] = GameUtil.Random(EF_FEELPT_COMMENT_[EF_COMMENT_SETTING[k][0]].Length);
			}
		}

		public static int GetFeelPtEf(int frame, int TIME)
		{

			int num = TIME * WAIT_TIME[0] / 100 + 4;
			int num2 = TIME * WAIT_TIME[1] / 100 + 4;
			int num3 = TIME * WAIT_TIME[2] / 100 + 4;
			int num4 = 0;
			if (frame < num + 12)
			{
				{ num4 = GameUtil.RateConvert(frame, num, num + 12, NEGOTIATE_RESULT_DISPVALUE[0], NEGOTIATE_RESULT_DISPVALUE[1], true); goto resultExit; }
			}
			if (frame < num2 + 12)
			{
				{ num4 = GameUtil.RateConvert(frame, num2, num2 + 12, NEGOTIATE_RESULT_DISPVALUE[1], NEGOTIATE_RESULT_DISPVALUE[2], true); goto resultExit; }
			}
			if (frame < num3 + 12)
			{
				{ num4 = GameUtil.RateConvert(frame, num3, num3 + 12, NEGOTIATE_RESULT_DISPVALUE[2], NEGOTIATE_RESULT_DISPVALUE[3], true); goto resultExit; }
			}
			{ num4 = NEGOTIATE_RESULT_DISPVALUE[3];  }

resultExit: return num4;
}

		public static int[] GetAddFeelPtEf(int frame, int TIME)
		{
			int num = TIME * WAIT_TIME[0] / 100 - 1;
			int num2 = TIME * WAIT_TIME[1] / 100 - 1;
			int num3 = TIME * WAIT_TIME[2] / 100 - 1;
			if (frame < num + 20)
			{
				if (frame < num)
				{
					EF_VALUE[0] = 0;
				}
				else if (NEGOTIATE_RESULT_TH[0] == 1)
				{
					EF_VALUE[0] = 2;
				}
				else
				{
					EF_VALUE[0] = 1;
				}
				EF_VALUE[1] = NEGOTIATE_RESULT_VALUE[0];
			}
			else if (frame < num2 + 20)
			{
				if (frame < num2)
				{
					EF_VALUE[0] = 0;
				}
				else if (NEGOTIATE_RESULT_TH[1] == 1)
				{
					EF_VALUE[0] = 2;
				}
				else
				{
					EF_VALUE[0] = 1;
				}
				EF_VALUE[1] = NEGOTIATE_RESULT_VALUE[1];
			}
			else if (frame < num3 + 20)
			{
				if (frame < num3)
				{
					EF_VALUE[0] = 0;
				}
				else if (NEGOTIATE_RESULT_TH[2] == 1)
				{
					EF_VALUE[0] = 2;
				}
				else
				{
					EF_VALUE[0] = 1;
				}
				EF_VALUE[1] = NEGOTIATE_RESULT_VALUE[2];
			}
			else
			{
				EF_VALUE[0] = 0;
			}
			return EF_VALUE;
		}

		public static string GetFeelPtEfRetStr(int retFeelPt)
		{
			if (retFeelPt >= 100)
			{
				return FPER_STR[0];
			}
			if (retFeelPt >= 75)
			{
				return FPER_STR[1];
			}
			if (retFeelPt >= 50)
			{
				return FPER_STR[2];
			}
			if (retFeelPt >= 25)
			{
				return FPER_STR[3];
			}
			return FPER_STR[4];
		}

		public static int[] GetFeelPtEfCommentParam(int frame, int TIME)
		{
			int num = TIME * 20 / 100 - 10;
			int num2 = TIME * 70 / 100 - 10;
			if (frame < num + 20)
			{
				EF_COMMENT[0] = 1;
				EF_COMMENT[1] = EF_COMMENT_SETTING[0][0];
				EF_COMMENT[2] = EF_COMMENT_SETTING[0][1];
			}
			else if (frame < num2 + 20)
			{
				EF_COMMENT[0] = 1;
				EF_COMMENT[1] = EF_COMMENT_SETTING[1][0];
				EF_COMMENT[2] = EF_COMMENT_SETTING[1][1];
			}
			else
			{
				EF_COMMENT[0] = 1;
				EF_COMMENT[1] = EF_COMMENT_SETTING[2][0];
				EF_COMMENT[2] = EF_COMMENT_SETTING[2][1];
			}
			return EF_COMMENT;
		}

		public static bool ChGetFeelPtEfCommentParam_dispRefresh(int frame, int TIME)
		{
			int num = TIME * 20 / 100 - 10;
			int num2 = TIME * 70 / 100 - 10;
			if (frame == num + 20 || frame == num + 20 + 1 || frame == num + 20 - 1)
			{
				return true;
			}
			if (frame == num2 + 20 || frame == num2 + 20 + 1 || frame == num2 + 20 - 1)
			{
				return true;
			}
			return false;
		}

		public void RecordAddMoney(int recordId, int value)
		{
			records_[apdat_.save_.ints[2]][recordId][0] += value;
		}

		public void RecordSubMoney(int recordId, int value)
		{
			records_[apdat_.save_.ints[2]][recordId][1] += value;
		}

		public void RecordReset()
		{
			for (int i = 0; i < records_.Length; i++)
			{
				for (int j = 0; j < records_[i].Length; j++)
				{
					records_[i][j][0] = 0;
					records_[i][j][1] = 0;
				}
			}
		}

		public void RecordAddCustomerNum(int value)
		{
			record_customer.AddElement(new JInteger(value));
			if (record_customer.Size() > 10)
			{
				record_customer.RemoveElementAt(0);
			}
		}

		public void CustomerNumReset()
		{
			for (int i = 0; i < customer_month_.Length; i++)
			{
				customer_month_[i] = 0;
			}
		}

		public void SetClearPoint()
		{
			clear_point[0][0] = customer_total_;
			clear_point[0][1] = (long)((float)clear_point[0][0] * 0.01f);
			clear_point[1][0] = matchGetGoalTotal_;
			clear_point[1][1] = (long)((float)clear_point[1][0] * 30f);
			int num = 0;
			for (int i = 0; i < apdat_.pData_.Length; i++)
			{
				PlayerData playerData = apdat_.pData_[i];
				if (playerData.onceMember_ == 0) { continue; }
num += playerData.lv_;
			}
			clear_point[2][0] = num;
			clear_point[2][1] = (long)((float)clear_point[2][0] * 15f);
			clear_point[3][0] = matchResult_[0];
			clear_point[3][1] = (long)((float)clear_point[3][0] * 150f);
			TeamData teamData = apdat_.tData_[0];
			clear_point[4][0] = teamData.warPotential_;
			clear_point[4][1] = (long)((float)clear_point[4][0] * 15f);
			int num2 = 0;
			for (int j = 0; j < apdat_.sponsorData_.Length; j++)
			{
				if (apdat_.sponsorData_[j].state_ != 2) { continue; }
num2 += apdat_.sponsorData_[j].satisfactPoint_;
			}
			clear_point[5][0] = num2;
			clear_point[5][1] = (long)((float)clear_point[5][0] * 5f);
			int num3 = 0;
			for (int k = 0; k < apdat_.formationData_.Length; k++)
			{
				if (apdat_.formationData_[k].state_ == 0) { continue; }
num3++;
			}
			clear_point[6][0] = num3;
			clear_point[6][1] = (long)((float)clear_point[6][0] * 100f);
		}

		public void SetContinueGameData()
		{
			int[] array = null;
			int[][] array2 = null;
			if (apdat_.sysSave_.bins[0].Length > 0)
			{
				array = new int[apdat_.pData_.Length];
				array2 = JTool.MakeArray<int[][]>(new int[2]
				{
					apdat_.pData_.Length,
					apdat_.pData_[0].trainingPoint_.Length
				});
				ByteArrayInputStream byteArrayInputStream = null;
				try
				{
					byteArrayInputStream = new ByteArrayInputStream(apdat_.sysSave_.bins[0]);
					for (int i = 0; i < apdat_.pData_.Length; i++)
					{
						array[i] = StreamUtil.ReadInt(byteArrayInputStream);
						for (int j = 0; j < array2[i].Length; j++)
						{
							array2[i][j] = StreamUtil.ReadInt(byteArrayInputStream);
						}
					}
				}
				finally
				{
					if (byteArrayInputStream != null)
					{
						byteArrayInputStream.Close();
					}
					byteArrayInputStream = null;
				}
			}
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int k = 0; k < apdat_.pData_.Length; k++)
				{
					PlayerData playerData = apdat_.pData_[k];
					if (array == null)
					{
						StreamUtil.WriteInt(byteArrayOutputStream, playerData.lv_);
						for (int l = 0; l < playerData.trainingPoint_.Length; l++)
						{
							StreamUtil.WriteInt(byteArrayOutputStream, playerData.trainingPoint_[l]);
						}
						continue;
					}
					int num = 0;
					if ((playerData.lv_ > array[k])) { num = (playerData.lv_); } else { num = (array[k]); }
					StreamUtil.WriteInt(byteArrayOutputStream, num);
					for (int m = 0; m < playerData.trainingPoint_.Length; m++)
					{
						if ((playerData.trainingPoint_[m] > array2[k][m])) { num = (playerData.trainingPoint_[m]); } else { num = (array2[k][m]); }
						StreamUtil.WriteInt(byteArrayOutputStream, num);
					}
				}
				apdat_.sysSave_.bins[0] = byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream != null)
				{
					byteArrayOutputStream.Close();
				}
				byteArrayOutputStream = null;
				array = null;
				array2 = null;
			}
			bool[] array3 = null;
			if (apdat_.sysSave_.bins[1].Length > 0)
			{
				ByteArrayInputStream byteArrayInputStream2 = null;
				try
				{
					byteArrayInputStream2 = new ByteArrayInputStream(apdat_.sysSave_.bins[1]);
					array3 = new bool[apdat_.formationData_.Length];
					for (int n = 0; n < apdat_.formationData_.Length; n++)
					{
						array3[n] = StreamUtil.ReadBoolean(byteArrayInputStream2);
					}
				}
				finally
				{
					if (byteArrayInputStream2 != null)
					{
						byteArrayInputStream2.Close();
					}
					byteArrayInputStream2 = null;
				}
			}
			ByteArrayOutputStream byteArrayOutputStream2 = null;
			try
			{
				byteArrayOutputStream2 = new ByteArrayOutputStream();
				for (int num2 = 0; num2 < apdat_.formationData_.Length; num2++)
				{
					FormationData formationData = apdat_.formationData_[num2];
					if (array3 == null)
					{
						if (formationData.state_ == 1)
						{
							StreamUtil.WriteBoolean(byteArrayOutputStream2, true);
						}
						else
						{
							StreamUtil.WriteBoolean(byteArrayOutputStream2, false);
						}
					}
					else if (formationData.state_ == 1 || array3[num2])
					{
						StreamUtil.WriteBoolean(byteArrayOutputStream2, true);
					}
					else
					{
						StreamUtil.WriteBoolean(byteArrayOutputStream2, false);
					}
				}
				apdat_.sysSave_.bins[1] = byteArrayOutputStream2.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream2 != null)
				{
					byteArrayOutputStream2.Close();
				}
				byteArrayOutputStream2 = null;
				array3 = null;
			}
			int[] array4 = null;
			if (apdat_.sysSave_.bins[2].Length > 0)
			{
				ByteArrayInputStream byteArrayInputStream3 = null;
				try
				{
					byteArrayInputStream3 = new ByteArrayInputStream(apdat_.sysSave_.bins[2]);
					array4 = new int[apdat_.tenantData_.Length];
					for (int num3 = 0; num3 < apdat_.tenantData_.Length; num3++)
					{
						array4[num3] = StreamUtil.ReadInt(byteArrayInputStream3);
					}
				}
				finally
				{
					if (byteArrayInputStream3 != null)
					{
						byteArrayInputStream3.Close();
					}
					byteArrayInputStream3 = null;
				}
			}
			ByteArrayOutputStream byteArrayOutputStream3 = null;
			try
			{
				byteArrayOutputStream3 = new ByteArrayOutputStream();
				for (int num4 = 0; num4 < apdat_.tenantData_.Length; num4++)
				{
					TenantData tenantData = apdat_.tenantData_[num4];
					if (tenantData.id_ == TenantData.TENANT_ID_TRAININGAREA || tenantData.id_ == TenantData.TENANT_ID_TRAININGAREA_SPRED || tenantData.id_ == TenantData.TENANT_ID_STADIUM)
					{
						StreamUtil.WriteInt(byteArrayOutputStream3, 1);
						continue;
					}
					if (array4 == null)
					{
						StreamUtil.WriteInt(byteArrayOutputStream3, tenantData.lv_);
						continue;
					}
					int num5 = 0;
					if ((tenantData.lv_ > array4[num4])) { num5 = (tenantData.lv_); } else { num5 = (array4[num4]); }
					StreamUtil.WriteInt(byteArrayOutputStream3, num5);
				}
				apdat_.sysSave_.bins[2] = byteArrayOutputStream3.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream3 != null)
				{
					byteArrayOutputStream3.Close();
				}
				byteArrayOutputStream3 = null;
				array4 = null;
			}
			int[] array5 = null;
			if (apdat_.sysSave_.bins[3].Length > 0)
			{
				ByteArrayInputStream byteArrayInputStream4 = null;
				try
				{
					byteArrayInputStream4 = new ByteArrayInputStream(apdat_.sysSave_.bins[3]);
					array5 = new int[apdat_.svisorData_.Length];
					for (int num6 = 0; num6 < apdat_.svisorData_.Length; num6++)
					{
						array5[num6] = StreamUtil.ReadInt(byteArrayInputStream4);
					}
				}
				finally
				{
					if (byteArrayInputStream4 != null)
					{
						byteArrayInputStream4.Close();
					}
					byteArrayInputStream4 = null;
				}
			}
			ByteArrayOutputStream byteArrayOutputStream4 = null;
			try
			{
				byteArrayOutputStream4 = new ByteArrayOutputStream();
				for (int num7 = 0; num7 < apdat_.svisorData_.Length; num7++)
				{
					SupervisorData supervisorData = apdat_.svisorData_[num7];
					if (array5 == null)
					{
						StreamUtil.WriteInt(byteArrayOutputStream4, supervisorData.lv_);
						continue;
					}
					int num8 = 0;
					if ((supervisorData.lv_ > array5[num7])) { num8 = (supervisorData.lv_); } else { num8 = (array5[num7]); }
					StreamUtil.WriteInt(byteArrayOutputStream4, num8);
				}
				apdat_.sysSave_.bins[3] = byteArrayOutputStream4.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream4 != null)
				{
					byteArrayOutputStream4.Close();
				}
				byteArrayOutputStream4 = null;
				array5 = null;
			}
			bool[] array6 = null;
			if (apdat_.sysSave_.bins[4].Length > 0)
			{
				ByteArrayInputStream byteArrayInputStream5 = null;
				try
				{
					byteArrayInputStream5 = new ByteArrayInputStream(apdat_.sysSave_.bins[4]);
					array6 = new bool[apdat_.tradeData_.Length];
					for (int num9 = 0; num9 < apdat_.tradeData_.Length; num9++)
					{
						TradeData tradeData = apdat_.tradeData_[num9];
						array6[num9] = StreamUtil.ReadBoolean(byteArrayInputStream5);
					}
				}
				finally
				{
					if (byteArrayInputStream5 != null)
					{
						byteArrayInputStream5.Close();
					}
					byteArrayInputStream5 = null;
				}
			}
			ByteArrayOutputStream byteArrayOutputStream5 = null;
			try
			{
				byteArrayOutputStream5 = new ByteArrayOutputStream();
				for (int num10 = 0; num10 < apdat_.tradeData_.Length; num10++)
				{
					TradeData tradeData2 = apdat_.tradeData_[num10];
					if (array6 == null)
					{
						if (tradeData2.state_ != 0)
						{
							StreamUtil.WriteBoolean(byteArrayOutputStream5, true);
						}
						else
						{
							StreamUtil.WriteBoolean(byteArrayOutputStream5, false);
						}
					}
					else if (tradeData2.state_ != 0 || array6[num10])
					{
						StreamUtil.WriteBoolean(byteArrayOutputStream5, true);
					}
					else
					{
						StreamUtil.WriteBoolean(byteArrayOutputStream5, false);
					}
				}
				apdat_.sysSave_.bins[4] = byteArrayOutputStream5.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream5 != null)
				{
					byteArrayOutputStream5.Close();
				}
				byteArrayOutputStream5 = null;
			}
			apdat_.SaveSystem();
		}

		public void ReflectContinueGameData()
		{
			if (apdat_.sysSave_.bins[0].Length > 0)
			{
				ByteArrayInputStream byteArrayInputStream = null;
				try
				{
					byteArrayInputStream = new ByteArrayInputStream(apdat_.sysSave_.bins[0]);
					for (int i = 0; i < apdat_.pData_.Length; i++)
					{
						PlayerData playerData = apdat_.pData_[i];
						playerData.lv_ = StreamUtil.ReadInt(byteArrayInputStream);
						for (int j = 0; j < playerData.trainingPoint_.Length; j++)
						{
							playerData.trainingPoint_[j] = StreamUtil.ReadInt(byteArrayInputStream);
						}
						playerData.SetLv(playerData.lv_);
					}
				}
				finally
				{
					if (byteArrayInputStream != null)
					{
						byteArrayInputStream.Close();
					}
					byteArrayInputStream = null;
				}
			}
			if (apdat_.sysSave_.bins[1].Length > 0)
			{
				ByteArrayInputStream byteArrayInputStream2 = null;
				try
				{
					byteArrayInputStream2 = new ByteArrayInputStream(apdat_.sysSave_.bins[1]);
					for (int k = 0; k < apdat_.formationData_.Length; k++)
					{
						FormationData formationData = apdat_.formationData_[k];
						{ bool recoveredCondition_173183 = StreamUtil.ReadBoolean(byteArrayInputStream2);
if (recoveredCondition_173183)
						{
							formationData.ChangeState(1);
							formationData.new_ = false;
						}
}
					}
				}
				finally
				{
					if (byteArrayInputStream2 != null)
					{
						byteArrayInputStream2.Close();
					}
					byteArrayInputStream2 = null;
				}
			}
			if (apdat_.sysSave_.bins[2].Length > 0)
			{
				ByteArrayInputStream byteArrayInputStream3 = null;
				try
				{
					byteArrayInputStream3 = new ByteArrayInputStream(apdat_.sysSave_.bins[2]);
					for (int l = 0; l < apdat_.tenantData_.Length; l++)
					{
						TenantData tenantData = apdat_.tenantData_[l];
						int lv = StreamUtil.ReadInt(byteArrayInputStream3);
						tenantData.SetLv(lv);
					}
				}
				finally
				{
					if (byteArrayInputStream3 != null)
					{
						byteArrayInputStream3.Close();
					}
					byteArrayInputStream3 = null;
				}
			}
			if (apdat_.sysSave_.bins[3].Length > 0)
			{
				ByteArrayInputStream byteArrayInputStream4 = null;
				try
				{
					byteArrayInputStream4 = new ByteArrayInputStream(apdat_.sysSave_.bins[3]);
					for (int m = 0; m < apdat_.svisorData_.Length; m++)
					{
						SupervisorData supervisorData = apdat_.svisorData_[m];
						int lv2 = StreamUtil.ReadInt(byteArrayInputStream4);
						supervisorData.SetLv(lv2);
					}
				}
				finally
				{
					if (byteArrayInputStream4 != null)
					{
						byteArrayInputStream4.Close();
					}
					byteArrayInputStream4 = null;
				}
			}
			if (apdat_.sysSave_.bins[4].Length <= 0)
			{
				return;
			}
			ByteArrayInputStream byteArrayInputStream5 = null;
			try
			{
				byteArrayInputStream5 = new ByteArrayInputStream(apdat_.sysSave_.bins[4]);
				for (int n = 0; n < apdat_.tradeData_.Length; n++)
				{
					TradeData tradeData = apdat_.tradeData_[n];
					if (StreamUtil.ReadBoolean(byteArrayInputStream5))
					{
						if (tradeData.IsPlayerInMember())
						{
							tradeData.ChangeState(2);
							tradeData.new_ = false;
						}
						else
						{
							tradeData.ChangeState(1);
							tradeData.new_ = false;
						}
					}
				}
			}
			finally
			{
				if (byteArrayInputStream5 != null)
				{
					byteArrayInputStream5.Close();
				}
				byteArrayInputStream5 = null;
			}
		}
	}
}
