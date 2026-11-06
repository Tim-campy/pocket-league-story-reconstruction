using System;
using System.Threading;
using data;
using form;
using game;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.graph;
using kairo.unity.graphics;
using kairo.unity.io;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using surface;

namespace main
{
	public class AppData
	{
		public sealed class stringErrorCode
		{
			public const int app_name = 2131034112;

			public const int ERROR_MESSAGE_LICENSING_CHECK = 2131034113;

			public const int ERROR_MESSAGE_LICENSING_ERROR00 = 2131034114;

			public const int ERROR_MESSAGE_LICENSING_ERROR01 = 2131034115;

			public const int ERROR_MESSAGE_LICENSING_ERROR02 = 2131034116;

			public const int ERROR_MESSAGE_LICENSING_ERROR03 = 2131034117;

			public const int ERROR_MESSAGE_LICENSING_ERROR04 = 2131034118;

			public const int ERROR_MESSAGE_LICENSING_ERROR05 = 2131034119;

			public const int ERROR_MESSAGE_LICENSING_ERROR = 2131034120;
		}

		public const int APPLI_VERSION = 202;

		public const int REGIST_WAIT = 30;

		public const int PID = 55;

		public const int CGI_REGIST = 0;

		public const int CGI_RANKING = 1;

		public const int CGI_REGIST_SCORE = 2;

		public const int CGI_GET_SCORE = 3;

		public const int FORMID_BOOT = 0;

		public const int FORMID_TITLE = 1;

		public const int FORMID_SOCCER = 2;

		public const int FORMID_RANK = 3;

		public const int FORMID_SUB = 4;

		public const int FORMID_DEBUG = 5;

		public const int FORMID_TEST = 6;

		public const int FORMID_RACE = 7;

		public const int FORMID_PRINT = 8;

		public const int FORMID_GAME = 9;

		public const int RS_SYSTEM = 0;

		public const int RS_RESOURCE = 1;

		public const int RSSYS_SYSSAVE = 0;

		public const int RSSYS_SAVE1 = 1;

		public const int RSSYS_SAVE2 = 2;

		public const int RSSYS_AUTO1 = 3;

		public const int RSSYS_AUTO2 = 4;

		public const int RSRES_TEST = 0;

		public const int RSRES_COM2 = 1;

		public const int RSRES_MAP = 2;

		public const int RSRES_EVENT = 3;

		public const int RSRES_MAPCHIP = 4;

		public const int RSRES_MAPOBJECT = 5;

		public const int RSRES_XLS = 6;

		public const int RSRES_TITLE = 7;

		public const int RSRES_MAPCHIPMAIN = 8;

		public const int RSRES_HUMAN2 = 9;

		public const int RSRES_HUMAN_HEAD = 10;

		public const int RSRES_SOUND = 11;

		public const int RSRES_END = 12;

		public const int RSSYS_SIZE_SYSSAME = 8192;

		public const int RSSYS_SIZE_SAVE1 = 46080;

		public const int RESHUMAN_IMG_PLAYER00 = 0;

		public const int RESHUMAN_IMG_PLAYER01 = 1;

		public const int RESHUMAN_SEB_DOWN = 0;

		public const int RESHUMAN_SEB_GOALGLAD0 = 4;

		public const int RESHUMAN_SEB_GOALGLAD1 = 8;

		public const int RESHUMAN_SEB_GOALGLAD2 = 12;

		public const int RESHUMAN_SEB_GOALSAD = 16;

		public const int RESHUMAN_SEB_HEAD = 20;

		public const int RESHUMAN_SEB_KICK = 24;

		public const int RESHUMAN_SEB_RUN = 28;

		public const int RESHUMAN_SEB_SLIDING = 32;

		public const int RESHUMAN_SEB_THROWIN = 36;

		public const int RESHUMAN_SEB_TRAP1 = 40;

		public const int RESHUMAN_SEB_TRAP2 = 44;

		public const int RESHUMAN_SEB_VOLLEY = 48;

		public const int RESHUMAN_SEB_WALK = 52;

		public const int RESHUMAN_SEB_SAVING = 56;

		public const int RESHUMAN_SEB_GK_KICK = 60;

		public const int RESHUMAN_SEB_GK_GOALGLAD0 = 64;

		public const int RESHUMAN_SEB_GK_RUN = 68;

		public const int RESHUMAN_SEB_GK_GOALSAD = 72;

		public const int RESHUMAN_SEB_GK_DOWN = 76;

		public const int RESHUMAN_SEB_FRONT = 80;

		public const int RESHUMAN_SEB_GOALGLAD0_SV = 81;

		public const int RESHUMANHEAD_SEB_FACE = 0;

		public const int RESCOM_IMG_PLAYER = 0;

		public const int RESCOM_IMG_GROUND00 = 100;

		public const int RESCOM_IMG_GROUND01 = 101;

		public const int RESCOM_SEB_GROUND00 = 0;

		public const int RESCOM2_IMG_GRASS00 = 0;

		public const int RESCOM2_IMG_GRASS01 = 1;

		public const int RESCOM2_IMG_BALL00 = 2;

		public const int RESCOM2_IMG_GOAL00_0 = 3;

		public const int RESCOM2_IMG_GOAL00_1 = 4;

		public const int RESCOM2_IMG_GOAL00_2 = 5;

		public const int RESCOM2_IMG_GOAL01_0 = 6;

		public const int RESCOM2_IMG_GOAL01_1 = 7;

		public const int RESCOM2_IMG_EF_SHOOT = 9;

		public const int RESCOM2_IMG_EF_SLIDING = 10;

		public const int RESCOM2_IMG_L_CONNER00 = 11;

		public const int RESCOM2_IMG_L_CONNER01 = 12;

		public const int RESCOM2_IMG_L_CONNER02 = 13;

		public const int RESCOM2_IMG_L_CONNER03 = 14;

		public const int RESCOM2_IMG_L_CENTER00 = 15;

		public const int RESCOM2_IMG_L_CENTER01 = 16;

		public const int RESCOM2_IMG_L_CENTER02 = 17;

		public const int RESCOM2_IMG_L_CENTER03 = 18;

		public const int RESCOM2_IMG_L_BASE00 = 19;

		public const int RESCOM2_IMG_L_BASE01 = 20;

		public const int RESCOM2_IMG_L_GOAL1_00 = 21;

		public const int RESCOM2_IMG_L_GOAL1_01 = 22;

		public const int RESCOM2_IMG_L_GOAL2_00 = 23;

		public const int RESCOM2_IMG_L_GOAL2_01 = 24;

		public const int RESCOM2_IMG_MENU = 25;

		public const int RESCOM2_IMG_TOP_BAR = 26;

		public const int RESCOM2_IMG_ARROW00 = 27;

		public const int RESCOM2_IMG_WND_BACK = 28;

		public const int RESCOM2_IMG_WND_BAR = 29;

		public const int RESCOM2_IMG_WND_CONNER = 30;

		public const int RESCOM2_IMG_WND_POINT = 31;

		public const int RESCOM2_IMG_TOP_BAR_SOCCER = 32;

		public const int RESCOM2_IMG_UNIFORM = 33;

		public const int RESCOM2_IMG_TIME_PEOPLE = 34;

		public const int RESCOM2_IMG_TIME_GRASS = 35;

		public const int RESCOM2_IMG_TIME_JUDGE = 36;

		public const int RESCOM2_IMG_TIME_MESSAGE = 37;

		public const int RESCOM2_IMG_MATCHSTR = 38;

		public const int RESCOM2_IMG_CORN = 39;

		public const int RESCOM2_IMG_BUS = 42;

		public const int RESCOM2_IMG_MINIGOAL00_0 = 43;

		public const int RESCOM2_IMG_MINIGOAL00_1 = 44;

		public const int RESCOM2_IMG_MINIGOAL00_2 = 45;

		public const int RESCOM2_IMG_MINIGOAL01_0 = 46;

		public const int RESCOM2_IMG_MINIGOAL01_1 = 47;

		public const int RESCOM2_IMG_MINIGOAL01_2 = 48;

		public const int RESCOM2_IMG_EDIT_GRASS = 49;

		public const int RESCOM2_IMG_BUS01 = 50;

		public const int RESCOM2_IMG_ICON_POSITION = 51;

		public const int RESCOM2_IMG_GAME_STRHALF = 52;

		public const int RESCOM2_IMG_GAME_AURAEXPLAIN = 53;

		public const int RESCOM2_IMG_GAME_AURABAR = 54;

		public const int RESCOM2_IMG_GAME_AURAMAX = 55;

		public const int RESCOM2_IMG_GAME_PLAYERBACK = 56;

		public const int RESCOM2_IMG_MATCH_RANK = 57;

		public const int RESCOM2_IMG_MATCH_VS = 58;

		public const int RESCOM2_IMG_MATCH_FLAG = 59;

		public const int RESCOM2_IMG_MATCH_HOMEAWAY = 60;

		public const int RESCOM2_IMG_EDIT_BUTTON = 61;

		public const int RESCOM2_IMG_CURSOR_ARROW = 62;

		public const int RESCOM2_IMG_MOJI00 = 63;

		public const int RESCOM2_IMG_MOJI01 = 64;

		public const int RESCOM2_IMG_MOJI02 = 65;

		public const int RESCOM2_IMG_MOJI03 = 66;

		public const int RESCOM2_IMG_MOJI04 = 67;

		public const int RESCOM2_IMG_MOJI05 = 68;

		public const int RESCOM2_IMG_MOJI06 = 69;

		public const int RESCOM2_IMG_FINGER_R = 70;

		public const int RESCOM2_IMG_FINGER_D = 71;

		public const int RESCOM2_IMG_ARROW01 = 72;

		public const int RESCOM2_IMG_ARROW02 = 74;

		public const int RESCOM2_IMG_RANK_GRADE = 75;

		public const int RESCOM2_IMG_AURA00 = 76;

		public const int RESCOM2_IMG_HISHO_TALK = 77;

		public const int RESCOM2_IMG_CURSOR = 80;

		public const int RESCOM2_IMG_VICTORY_RIBBON = 81;

		public const int RESCOM2_IMG_VICTORY_FIREWORKS = 82;

		public const int RESCOM2_IMG_VICTORY_LAUREL = 83;

		public const int RESCOM2_IMG_VICTORY_STADIUM = 84;

		public const int RESCOM2_IMG_VICTORY_NAMEBACK = 85;

		public const int RESCOM2_IMG_VICTORY_BODY = 86;

		public const int RESCOM2_IMG_VICTORY_VICTORY = 87;

		public const int RESCOM2_IMG_CONFETTI = 88;

		public const int RESCOM2_IMG_FADEOUT = 89;

		public const int RESCOM2_IMG_EF_CHARGE = 90;

		public const int RESCOM2_IMG_GAME_HOWOFFENCE = 91;

		public const int RESCOM2_IMG_GAME_MINICOAT = 92;

		public const int RESCOM2_IMG_GAME_MINICOATOBJ = 93;

		public const int RESCOM2_IMG_ICON_RESEARCH = 94;

		public const int RESCOM2_IMG_ICON_SEASON = 95;

		public const int RESCOM2_IMG_EF_LVUP = 96;

		public const int RESCOM2_IMG_ICON_TROPHY = 97;

		public const int RESCOM2_IMG_ICON_TENANTBUILD = 98;

		public const int RESCOM2_IMG_NUMBER01 = 100;

		public const int RESCOM2_IMG_NUMBER02 = 101;

		public const int RESCOM2_IMG_NUMBER03 = 102;

		public const int RESCOM2_IMG_NUMBER05 = 103;

		public const int RESCOM2_IMG_NUMBER06 = 104;

		public const int RESCOM2_IMG_NUMBER08 = 105;

		public const int RESCOM2_IMG_NUMBER09 = 106;

		public const int RESCOM2_IMG_NUMBER10 = 107;

		public const int RESCOM2_IMG_NUMBER11 = 108;

		public const int RESCOM2_IMG_NUMBER12 = 109;

		public const int RESCOM2_IMG_WND_ATO = 115;

		public const int RESCOM2_IMG_WND_CATEGORYICON = 116;

		public const int RESCOM2_IMG_WND_EXP = 117;

		public const int RESCOM2_IMG_WND_EXPBAR = 118;

		public const int RESCOM2_IMG_WND_BAX = 119;

		public const int RESCOM2_IMG_WND_METER = 120;

		public const int RESCOM2_IMG_WND_PHYICON = 121;

		public const int RESCOM2_IMG_WND_PHYSTR = 122;

		public const int RESCOM2_IMG_WND_POSIBACK = 123;

		public const int RESCOM2_IMG_WND_STAMEM = 124;

		public const int RESCOM2_IMG_WND_STATUSBACK = 125;

		public const int RESCOM2_IMG_WND_LV = 126;

		public const int RESCOM2_IMG_WND_METER2 = 127;

		public const int RESCOM2_IMG_WND_TYPEBACK = 128;

		public const int RESCOM2_IMG_WND_ASSISTBAR = 129;

		public const int RESCOM2_IMG_WND_FUKIDASHI = 130;

		public const int RESCOM2_IMG_WND_PARSENT = 131;

		public const int RESCOM2_IMG_WND_METER01 = 132;

		public const int RESCOM2_IMG_WND_NEWSLOGO = 133;

		public const int RESCOM2_IMG_WND_MENUBTMBACK = 134;

		public const int RESCOM2_IMG_MAIN_NEXTMATCH = 135;

		public const int RESCOM2_IMG_MAIN_PLAYERBACK = 136;

		public const int RESCOM2_IMG_MAIN_PLAYERCURSOR00 = 137;

		public const int RESCOM2_IMG_MAIN_PLAYERCURSOR01 = 138;

		public const int RESCOM2_IMG_MAIN_PLAYERCURSOR02 = 139;

		public const int RESCOM2_IMG_MAIN_TENANTCURSOR = 140;

		public const int RESCOM2_IMG_MAIN_BTMBAR = 141;

		public const int RESCOM2_IMG_MAIN_TENANTCURSOR2 = 142;

		public const int RESCOM2_IMG_MAIN_RESEARCHPOINT = 143;

		public const int RESCOM2_IMG_MAIN_NEXTMATCHWEEK = 144;

		public const int RESCOM2_IMG_BUILDING00 = 155;

		public const int RESCOM2_IMG_BUILDING01 = 156;

		public const int RESCOM2_IMG_BUILDING02 = 157;

		public const int RESCOM2_IMG_DESTRUCT = 158;

		public const int RESCOM2_IMG_MATCHPRI_ASSISTBAR = 159;

		public const int RESCOM2_IMG_MATCHPRI_ASSISTUP = 160;

		public const int RESCOM2_IMG_MATCHPRI_ENEMY = 161;

		public const int RESCOM2_IMG_BACK_TRAINING = 162;

		public const int RESCOM2_IMG_WND_BONUS_GET = 163;

		public const int RESCOM2_IMG_WND_BONUS_NOTGET = 164;

		public const int RESCOM2_IMG_WND_BONUS_FUKIDASHI = 165;

		public const int RESCOM2_IMG_WND_FAN_CHARABACK = 166;

		public const int RESCOM2_IMG_WND_TRA_PBACK = 167;

		public const int RESCOM2_IMG_WND_MENUICON = 168;

		public const int RESCOM2_IMG_SPONSOR_CHARA = 169;

		public const int RESCOM2_IMG_WND_TENANTGETPT = 170;

		public const int RESCOM2_IMG_CHARA_PRESIDENT = 171;

		public const int RESCOM2_IMG_CHARA_HISHOKO = 172;

		public const int RESCOM2_IMG_WND_TRAININGICON = 173;

		public const int RESCOM2_IMG_WND_FANACTBACK = 174;

		public const int RESCOM2_IMG_WND_SPONSORRESULT = 175;

		public const int RESCOM2_IMG_WND_SVISOR_PT = 176;

		public const int RESCOM2_IMG_WND_PLAYERPOPULARUP = 177;

		public const int RESCOM2_IMG_WND_SPONSOR_CHECK = 178;

		public const int RESCOM2_IMG_WND_FORMATION_GET = 179;

		public const int RESCOM2_IMG_EDIT_MEMCHANGEHELP = 180;

		public const int RESCOM2_IMG_WND_NEW = 181;

		public const int RESCOM2_IMG_WND_MEMCHANGEHELP = 182;

		public const int RESCOM2_IMG_ICON_NATIONAL_FLAG = 183;

		public const int RESCOM2_IMG_WND_BATTLEBACK = 184;

		public const int RESCOM2_IMG_STADIUM00 = 185;

		public const int RESCOM2_IMG_STADIUM01 = 186;

		public const int RESCOM2_IMG_STADIUM02 = 187;

		public const int RESCOM2_IMG_WND_FINISH = 188;

		public const int RESCOM2_IMG_OFFENCE_DIRECT = 189;

		public const int RESCOM2_IMG_TROPHY = 190;

		public const int RESCOM2_IMG_VICTORY01 = 191;

		public const int RESCOM2_IMG_VICTORY02 = 192;

		public const int RESCOM2_IMG_SAVELOAD = 193;

		public const int RESCOM2_SEB_BALL00 = 0;

		public const int RESCOM2_SEB_GOAL00_0 = 1;

		public const int RESCOM2_SEB_GOAL00_1 = 2;

		public const int RESCOM2_SEB_GOAL00_2 = 3;

		public const int RESCOM2_SEB_GOAL01_0 = 4;

		public const int RESCOM2_SEB_GOAL01_1 = 5;

		public const int RESCOM2_SEB_EF_SHOOT = 6;

		public const int RESCOM2_SEB_EF_SLIDING = 7;

		public const int RESCOM2_SEB_L_CONNER00 = 8;

		public const int RESCOM2_SEB_L_CONNER01 = 9;

		public const int RESCOM2_SEB_L_CONNER02 = 10;

		public const int RESCOM2_SEB_L_CONNER03 = 11;

		public const int RESCOM2_SEB_L_CENTER00 = 12;

		public const int RESCOM2_SEB_L_CENTER01 = 13;

		public const int RESCOM2_SEB_L_CENTER02 = 14;

		public const int RESCOM2_SEB_L_CENTER03 = 15;

		public const int RESCOM2_SEB_L_BASE00 = 16;

		public const int RESCOM2_SEB_L_BASE01 = 17;

		public const int RESCOM2_SEB_L_GOAL1_00 = 18;

		public const int RESCOM2_SEB_L_GOAL1_01 = 19;

		public const int RESCOM2_SEB_L_GOAL2_00 = 20;

		public const int RESCOM2_SEB_L_GOAL2_01 = 21;

		public const int RESCOM2_SEB_MENU = 22;

		public const int RESCOM2_SEB_NUMBER01 = 23;

		public const int RESCOM2_SEB_ARROW00 = 24;

		public const int RESCOM2_SEB_WND_BACK = 25;

		public const int RESCOM2_SEB_WND_BAR = 26;

		public const int RESCOM2_SEB_WND_CONNER = 27;

		public const int RESCOM2_SEB_UNIFORM = 28;

		public const int RESCOM2_SEB_NUMBER02 = 29;

		public const int RESCOM2_SEB_NUMBER03 = 30;

		public const int RESCOM2_SEB_NUMBER04 = 31;

		public const int RESCOM2_SEB_TIME_PEOPLE = 32;

		public const int RESCOM2_SEB_TIME_GRASS = 33;

		public const int RESCOM2_SEB_TIME_JUDGE = 34;

		public const int RESCOM2_SEB_NUMBER05 = 35;

		public const int RESCOM2_SEB_MATCHSTR = 36;

		public const int RESCOM2_SEB_CORN = 37;

		public const int RESCOM2_SEB_BUS = 38;

		public const int RESCOM2_SEB_MINIGOAL00_0 = 39;

		public const int RESCOM2_SEB_MINIGOAL00_1 = 40;

		public const int RESCOM2_SEB_MINIGOAL00_2 = 41;

		public const int RESCOM2_SEB_MINIGOAL01_0 = 42;

		public const int RESCOM2_SEB_MINIGOAL01_1 = 43;

		public const int RESCOM2_SEB_MINIGOAL01_2 = 44;

		public const int RESCOM2_SEB_EDIT_GRASS = 45;

		public const int RESCOM2_SEB_BUS01 = 46;

		public const int RESCOM2_SEB_ICON_POSITION = 47;

		public const int RESCOM2_SEB_GAME_STRHALF = 48;

		public const int RESCOM2_SEB_GAME_AURAEXPLAIN = 49;

		public const int RESCOM2_SEB_MATCH_FLAG = 50;

		public const int RESCOM2_SEB_MATCH_HOMEAWAY = 51;

		public const int RESCOM2_SEB_CURSOR_ARROW = 52;

		public const int RESCOM2_SEB_MOJI00 = 53;

		public const int RESCOM2_SEB_MOJI01 = 54;

		public const int RESCOM2_SEB_MOJI02 = 55;

		public const int RESCOM2_SEB_MOJI03 = 56;

		public const int RESCOM2_SEB_MOJI04 = 57;

		public const int RESCOM2_SEB_MOJI05 = 58;

		public const int RESCOM2_SEB_MOJI06 = 59;

		public const int RESCOM2_SEB_FINGER_R = 60;

		public const int RESCOM2_SEB_FINGER_L = 61;

		public const int RESCOM2_SEB_FINGER_D = 62;

		public const int RESCOM2_SEB_ARROW01 = 63;

		public const int RESCOM2_SEB_ARROW02 = 64;

		public const int RESCOM2_SEB_RANK_GRADE = 65;

		public const int RESCOM2_SEB_NUMBER06 = 66;

		public const int RESCOM2_SEB_NUMBER07 = 67;

		public const int RESCOM2_SEB_AURA00 = 68;

		public const int RESCOM2_SEB_VICTORY_FIREWORKS = 69;

		public const int RESCOM2_SEB_VICTORY_BODY = 70;

		public const int RESCOM2_SEB_VICTORY_LAUREL = 71;

		public const int RESCOM2_SEB_CONFETTI = 72;

		public const int RESCOM2_SEB_WND_CATEGORYICON = 73;

		public const int RESCOM2_SEB_WND_PHYICON = 74;

		public const int RESCOM2_SEB_WND_PHYSTR = 75;

		public const int RESCOM2_SEB_WND_EXP = 76;

		public const int RESCOM2_SEB_MAIN_TENANTCURSOR = 77;

		public const int RESCOM2_SEB_NUMBER08 = 78;

		public const int RESCOM2_SEB_NUMBER09 = 79;

		public const int RESCOM2_SEB_MAIN_TENANTCURSOR2 = 80;

		public const int RESCOM2_SEB_EF_CHARGE = 81;

		public const int RESCOM2_SEB_NUMBER10 = 82;

		public const int RESCOM2_SEB_ICON_RESEARCH = 83;

		public const int RESCOM2_SEB_ICON_SEASON = 84;

		public const int RESCOM2_SEB_BUILDING00 = 85;

		public const int RESCOM2_SEB_BUILDING01 = 86;

		public const int RESCOM2_SEB_BUILDING02 = 87;

		public const int RESCOM2_SEB_DESTRUCT = 88;

		public const int RESCOM2_SEB_NUMBER11 = 89;

		public const int RESCOM2_SEB_WND_PARSENT = 90;

		public const int RESCOM2_SEB_NUMBER12 = 91;

		public const int RESCOM2_SEB_WND_TRA_PBACK = 92;

		public const int RESCOM2_SEB_WND_MENUICON = 93;

		public const int RESCOM2_SEB_EF_LVUP = 94;

		public const int RESCOM2_SEB_ICON_TROPHY = 95;

		public const int RESCOM2_SEB_CHARA = 96;

		public const int RESCOM2_SEB_WND_SPONSORRESULT = 97;

		public const int RESCOM2_SEB_WND_NEW = 98;

		public const int RESCOM2_SEB_WND_MEMCHANGEHELP = 99;

		public const int RESCOM2_SEB_ICON_NATIONAL_FLAG = 100;

		public const int RESCOM2_SEB_STADIUM00 = 101;

		public const int RESCOM2_SEB_STADIUM01 = 102;

		public const int RESCOM2_SEB_STADIUM02 = 103;

		public const int RESCOM2_SEB_NUMBER13 = 104;

		public const int RESCOM2_SEB_OFFENCE_DIRECT = 105;

		public const int RESCOM2_SEB_TROPHY = 106;

		public const int RESEVENT_IMG_BACK00 = 0;

		public const int RESEVENT_IMG_BACK01 = 1;

		public const int RESEVENT_IMG_BACK02 = 2;

		public const int RESEVENT_IMG_CHARA00 = 3;

		public const int RESEVENT_IMG_CHARA01 = 4;

		public const int RESEVENT_IMG_SURPORTER00 = 5;

		public const int RESEVENT_IMG_SURPORTER01 = 6;

		public const int RESEVENT_IMG_SPONSORBACK = 7;

		public const int RESEVENT_IMG_BACKMINI = 8;

		public const int RESEVENT_IMG_TRADEBACK = 9;

		public const int RESEVENT_IMG_FANBACK = 10;

		public const int RESEVENT_IMG_TRAININGBACK = 11;

		public const int RESEVENT_SEB_CHARA00 = 0;

		public const int RESEVENT_SEB_CHARA01 = 1;

		public const int RESEVENT_SEB_SURPORTER00 = 2;

		public const int RESEVENT_SEB_SURPORTER01 = 3;

		public const int RESSOUND_SND_BGM_MAIN1 = 0;

		public const int RESSOUND_SND_BGM_FORMATION = 1;

		public const int RESSOUND_SND_BGM_GAMEMAINA = 2;

		public const int RESSOUND_SND_BGM_GAMEMAINB = 3;

		public const int RESSOUND_SND_BGM_TITLE = 4;

		public const int RESSOUND_SND_06DECIDE = 5;

		public const int RESSOUND_SND_SE_CROSSBAR = 6;

		public const int RESSOUND_SND_SE_KANSEIA = 7;

		public const int RESSOUND_SND_SE_KICKA1 = 8;

		public const int RESSOUND_SND_SE_KICKA2 = 9;

		public const int RESSOUND_SND_SE_KICKA3 = 10;

		public const int RESSOUND_SND_SE_SLIDING = 11;

		public const int RESSOUND_SND_SE_WHISTLEA = 12;

		public const int RESSOUND_SND_SE_WHISTLEB = 13;

		public const int RESSOUND_SND_SE_WHISTLEC = 14;

		public const int RESSOUND_SND_SE_SHOOT = 15;

		public const int RESSOUND_SND_SE_SHOOT2 = 16;

		public const int RESSOUND_SND_SE_YOROKOBI = 17;

		public const int RESSOUND_SND_SE_GOAL = 18;

		public const int RESSOUND_SND_SE_GOAL2 = 19;

		public const int RESSOUND_SND_SE_FANFALE = 20;

		public const int RESSOUND_SND_SE_KANASHIMI = 21;

		public const int RESSOUND_SND_SE_KIRAN1 = 22;

		public const int RESSOUND_SND_SE_MISS = 23;

		public const int RESSOUND_SND_SE_BU = 24;

		public const int RESSOUND_SND_SE_LEVELUP = 25;

		public const int RESSOUND_SND_SE_01 = 26;

		public const int SOFT_NONE = 0;

		public const int SOFT_END = 1;

		public const int SOFT_MODORU = 2;

		public const int SOFT_SAVE = 3;

		public const int SOFT_MENU = 4;

		public const int SOFT_SITE = 5;

		public const int SOFT_CHANGE = 6;

		public const int SOFT_REMOVE = 7;

		public const int SOFT_EXPLAIN = 8;

		public const int SOFT_INFO = 9;

		public const int SOFT_DESTRUCT = 10;

		public const int SOFT_BENCH = 11;

		public const int SOFT_EDIT = 12;

		public const int SOFT_DECIDE = 13;

		public const int SYSSAVEP_END = 0;

		public const int SYSSAVEI_VER = 0;

		public const int SYSSAVEI_FLAG = 1;

		public const int SYSSAVEI_DLCNT = 2;

		public const int SYSSAVEI_BGM_LEVEL = 3;

		public const int SYSSAVEI_SE_LEVEL = 4;

		public const int SYSSAVEI_REGIST_WAIT = 5;

		public const int SYSSAVEI_SAVE1_DATE = 6;

		public const int SYSSAVEI_SAVE2_DATE = 7;

		public const int SYSSAVEI_CURRENT_SAVE = 8;

		public const int SYSSAVEI_OPEN_HIGHSPEED = 9;

		public const int SYSSAVEI_TROPHY = 10;

		public const int SYSSAVEI_AUTO1_DATE = 11;

		public const int SYSSAVEI_AUTO2_DATE = 12;

		public const int SYSSAVEI_SCREEN_ORIENTATION = 13;

		public const int SYSSAVEI_CURRENCY = 14;

		public const int SYSSAVEI_PADMODE = 15;

		public const int SYSSAVEI_RESTORATION_DATA = 16;

		public const int SYSSAVEI_END = 17;

		public const int SYSSAVEL_HIGHSCORE = 0;

		public const int SYSSAVEL_NET_HIGHSCORE = 1;

		public const int SYSSAVEL_MONEY_HIGHSCORE = 2;

		public const int SYSSAVEL_MONEY_NET_HIGHSCORE = 3;

		public const int SYSSAVEL_END = 4;

		public const int SYSSAVE_FLAG_RES_DOWNLOADED = 1;

		public const int SYSSAVES_HIGHSCORE_NAME = 0;

		public const int SYSSAVES_MONEY_HIGHSCORE_NAME = 1;

		public const int SYSSAVES_SAVE1_NAME = 2;

		public const int SYSSAVES_SAVE2_NAME = 3;

		public const int SYSSAVES_SAVE1_MONEY = 4;

		public const int SYSSAVES_SAVE2_MONEY = 5;

		public const int SYSSAVES_AUTO1_NAME = 6;

		public const int SYSSAVES_AUTO2_NAME = 7;

		public const int SYSSAVES_AUTO1_MONEY = 8;

		public const int SYSSAVES_AUTO2_MONEY = 9;

		public const int SYSSAVES_ERROR = 10;

		public const int SYSSAVES_ERROR_MATCH = 11;

		public const int SYSSAVES_END = 12;

		public const int SYSSAVEB_PLAYER_PARAM = 0;

		public const int SYSSAVEB_FORMATION = 1;

		public const int SYSSAVEB_TENANT_LV = 2;

		public const int SYSSAVEB_SVISOR_LV = 3;

		public const int SYSSAVEB_TRADE_LIST = 4;

		public const int SYSSAVEB_KAIROAD = 5;

		public const int SYSSAVEB_END = 6;

		public const int DATA_CHIP = 0;

		public const int DATA_EVENT = 1;

		public const int DATA_TALK = 2;

		public const int DATA_FURNITURE = 3;

		public const int DATA_FURNITURESET = 4;

		public const int SAVEP_EVENT_NUM = 0;

		public const int SAVEP_EVENT_NUM1 = 1;

		public const int SAVEP_EVENT_NUM2 = 2;

		public const int SAVEP_END = 3;

		public const int SAVEI_SAVEVER = 0;

		public const int SAVEI_YEAR = 1;

		public const int SAVEI_MONTH = 2;

		public const int SAVEI_WEEK = 3;

		public const int SAVEI_TIME = 4;

		public const int SAVEI_TIME_OLD = 5;

		public const int SAVEI_GAMESEC = 6;

		public const int SAVEI_CUSTOMER_INSTORE = 7;

		public const int SAVEI_FLAG = 8;

		public const int SAVEI_RIDE_TALK_CHARA = 9;

		public const int SAVEI_MAIN_BGM_GRADE = 10;

		public const int SAVEI_REPORT = 11;

		public const int SAVEI_QUALITY = 12;

		public const int SAVEI_GAME_SPEED = 13;

		public const int SAVEI_ENDING = 14;

		public const int SAVEI_SAVEDATA_VERSION = 15;

		public const int SAVEI_RESTORE_OLD_LOAD = 16;

		public const int SAVEI_END = 17;

		public const int FLAG_REPORTER = 1;

		public const int SAVEL_MONEY = 0;

		public const int SAVEL_PR = 1;

		public const int SAVEL_VTR = 2;

		public const int SAVEL_END = 3;

		public const int SAVES_NAME = 0;

		public const int SAVES_END = 1;

		public const int SAVEB_PLAYERDATA = 0;

		public const int SAVEB_QUESTMATCH = 1;

		public const int SAVEB_SPONSOR = 2;

		public const int SAVEB_FORMATION = 3;

		public const int SAVEB_STADIUM = 4;

		public const int SAVEB_SVISOR = 5;

		public const int SAVEB_TEAMDATA = 6;

		public const int SAVEB_TENANTDATA = 7;

		public const int SAVEB_FANACTDATA = 8;

		public const int SAVEB_FANBONUS = 9;

		public const int SAVEB_TRADE = 10;

		public const int SAVEB_TRAINING = 11;

		public const int SAVEB_LEAGUE = 12;

		public const int SAVEB_CAMERA = 13;

		public const int SAVEB_USERDATA = 14;

		public const int SAVEB_TENANT = 15;

		public const int SAVEB_DELAY_EVENTS = 16;

		public const int SAVEB_END = 17;

		public const int WEEK_SECOND = 10800;

		public const int ONE_DAY_WEEK = 4;

		public const int POPUP_ASSIST = 0;

		public const int SYSMSG_TABLESET = 0;

		public const int SYSMSG_TABLESET_SAME = 1;

		public const int SYSMSG_CLERK_TORESTROOM = 2;

		public const int SYSMSG_SET_GOING = 3;

		public const int MAP_MAX = 3;

		public const int CHIP_KOUZI = 0;

		public const int CHIP_GROUND = 4;

		public const int CHIP_LOBBY_OUTCOUNTER = 38;

		public const int CHIP_LOBBY_CORRIDOR = 39;

		public const int CHIP_LOBBY_INCOUNTER = 40;

		public const int CHIP_LOBBY_ENTRANCE = 41;

		public const int EFF_SHOOT = 0;

		public const int EFF_CHARGE = 1;

		public const int EFF_TRA_UP = 2;

		public const int EFF_TRA_LVUP = 3;

		public const int EFF_TRA_GETEXP = 4;

		public const int EFF_TRA_GETRP = 5;

		public const int EFF_TRA_GETMONEY = 6;

		public const int EFF_MAX = 7;

		public const int EVENT_NEWGAME = 0;

		public const int EVENT_TEST_EVENTMSG1 = 1;

		public const int EVENT_TEST_EVENTMSG2 = 2;

		public const int EVENT_ED_1 = 3;

		public const int EVENT_ED_HIGHSCORE_1 = 4;

		public const int EVENT_ED_HIGHSCORE_2 = 5;

		public const int EVENT_ED_2 = 6;

		public const int EVENT_TRA_NOTSETP = 10;

		public const int EVENT_MEM_NOTP = 11;

		public const int EVENT_TRA_NOTTRAP = 12;

		public const int EVENT_FAN_NOTFANP = 13;

		public const int EVENT_NOT_MONEY = 14;

		public const int EVENT_JOININNG_LEAGUE = 18;

		public const int EVENT_JOININIG_QUEST = 19;

		public const int EVENT_LEAGUE_AMA1 = 50;

		public const int EVENT_LEAGUE_REG1 = 51;

		public const int EVENT_LEAGUE_PRO1 = 52;

		public const int EVENT_LEAGUE_WOR1 = 53;

		public const int EVENT_LEAGUE_KAI1 = 54;

		public const int EVENT_LEAGUE_AMA2 = 55;

		public const int EVENT_LEAGUE_REG2 = 56;

		public const int EVENT_LEAGUE_PRO2 = 57;

		public const int EVENT_LEAGUE_WOR2 = 58;

		public const int EVENT_LEAGUE_KAI2 = 59;

		public const int EVENT_EDIT_F_COMMENT1 = 70;

		public const int EVENT_EDIT_F_COMMENT2 = 71;

		public const int EVENT_EDIT_F_COMMENT3 = 72;

		public const int EVENT_TRADE_COMMENT = 73;

		public const int EVENT_SPONSOR_SCCESS = 91;

		public const int EVENT_SPONSOR_FAIL = 92;

		public const int EVENT_SPONSOR_NOW = 93;

		public const int EVENT_TEST_SCRIPT = 100;

		public const int EVENT_TEST_RANK = 101;

		public const int EVENT_HUMAN_RESOURCE = 110;

		public const int EVENT_HR_SVISOR = 111;

		public const int EVENT_HR_SVISOR_SELECT = 112;

		public const int EVENT_HR_SVISOR_UPDATE = 113;

		public const int EVENT_HR_SVISOR_CONTINUE = 114;

		public const int EVENT_DIS_PLAYER_REPORT = 115;

		public const int EVENT_DIS_PLAYER_UPDATE = 116;

		public const int EVENT_DIS_PLAYER_SELECT = 117;

		public const int EVENT_DIS_PLAYER_M1 = 118;

		public const int EVENT_DIS_PLAYER_M2 = 119;

		public const int EVENT_DIS_PLAYER_M3 = 120;

		public const int EVENT_DIS_PLAYER_ELEVEN = 121;

		public const int EVENT_TRADE_FAIL = 122;

		public const int EVENT_TRADE_SELECT = 123;

		public const int EVENT_TRADE_LIST0 = 124;

		public const int EVENT_TENANT_BUILDLIST = 125;

		public const int EVENT_TENANT_STREN_ING = 126;

		public const int EVENT_FANACT_ACTIVE = 127;

		public const int EVENT_LEAGUE_SELECT = 128;

		public const int EVENT_LEAGUE_GO_NORMAL = 129;

		public const int EVENT_LEAGUE_GO_LAST = 130;

		public const int EVENT_LEAGUE_GO_LAST2 = 131;

		public const int EVENT_QUEST_SELECT = 132;

		public const int EVENT_QUEST_GO_NORMAL = 133;

		public const int EVENT_QUEST_WIN = 134;

		public const int EVENT_QUEST_DRAW = 135;

		public const int EVENT_QM_LOSE = 136;

		public const int EVENT_QM_LOSE2 = 137;

		public const int EVENT_GAME_INIT_GOAL = 138;

		public const int EVENT_GAME_INIT_MODELONGBALL = 139;

		public const int EVENT_GAME_INIT_MODEDRIBBLE = 140;

		public const int EVENT_CHANGE_TICKET = 141;

		public const int EVENT_SVISOR_LVUP = 142;

		public const int EVENT_EMPLOY_MAX = 143;

		public const int EVENT_SPONSOR_CONSTRUCT = 144;

		public const int EVENT_FADE_OUT = 145;

		public const int EVENT_FADE_IN = 146;

		public const int EVENT_TRAINING_INIT = 147;

		public const int EVENT_TRAINING_CHANGE = 148;

		public const int EVENT_TRA_CH_NORMAL = 149;

		public const int EVENT_TRA_CH_TRAINING = 150;

		public const int EVENT_TRA_CH_TENANT = 151;

		public const int EVENT_MATCH_NOTJOIN = 152;

		public const int EVENT_INIT_MATCH_LATE = 153;

		public const int EVENT_INIT_SELECT_TRADE = 154;

		public const int EVENT_INIT_SELECT_SPONSOR = 155;

		public const int EVENT_INIT_GO_MATCH = 156;

		public const int EVENT_INIT_SELECT_VACANT = 157;

		public const int EVENT_INIT_SELECT_TRA_TOP = 158;

		public const int EVENT_INIT_SELECT_TRAINING = 159;

		public const int EVENT_INIT_SELECT_TEAMEDIT = 160;

		public const int EVENT_INIT_SELECT_FORMATION = 161;

		public const int EVENT_INIT_SELECT_STAMEM = 162;

		public const int EVENT_INIT_SELECT_MEMINFO = 163;

		public const int EVENT_INIT_SELECT_TENANTSTREN = 164;

		public const int EVENT_INIT_SELECT_FANACT = 165;

		public const int EVENT_INIT_YEAREND_SVISOR = 166;

		public const int EVENT_INIT_LEAGUE_REGION = 167;

		public const int EVENT_INIT_LEAGUE_PRO = 168;

		public const int EVENT_INIT_LEAGUE_WORLD = 169;

		public const int EVENT_INIT_LEAGUE_KAIRO = 170;

		public const int EVENT_INIT_TENANT_GOODS = 171;

		public const int EVENT_INIT_AURAMAX = 172;

		public const int EVENT_INIT_TRADE_SUCCESS = 173;

		public const int EVENT_INIT_LEAGUE_SELECT_AMA = 174;

		public const int EVENT_NOT_SELECT_CLUB = 175;

		public const int EVENT_INIT_LEAGUEMATCH = 176;

		public const int EVENT_INIT_ASSISTMAX = 177;

		public const int EVENT_FANACT_HANDSHAKE = 178;

		public const int EVENT_FANACT_SIGN = 179;

		public const int EVENT_FANACT_SOCCER = 180;

		public const int EVENT_FANACT_COACH = 181;

		public const int EVENT_FANACT_TOURS = 182;

		public const int EVENT_FANACT_MAINTENANCE = 183;

		public const int EVENT_TSTREN_TAREA_LVUP1 = 184;

		public const int EVENT_TSTREN_TAREA_LVUP2 = 185;

		public const int EVENT_TSTREN_TAREA_SPRED = 186;

		public const int EVENT_TSTREN_STADIUM1 = 187;

		public const int EVENT_TSTREN_STADIUM2 = 188;

		public const int EVENT_INIT_PPARAM_1000 = 189;

		public const int EVENT_SELECT_HISTORY_NOTACHI = 190;

		public const int EVENT_163 = 191;

		public const int EVENT_164 = 184;

		public const int EVENT_165 = 184;

		public const int EVENT_166 = 184;

		public const int EVENT_167 = 184;

		public const int EVENT_168 = 184;

		public const int EVENT_169 = 184;

		public const int EVENT_170 = 184;

		public const int EVENT_171 = 184;

		public const int EVENT_172 = 184;

		public const int EVENT_173 = 184;

		public const int EVENT_174 = 184;

		public const int EVENT_175 = 184;

		public const int CTASK_CHARA = 0;

		public const int CTASK_REPEATER = 1;

		public const int CTASK_WAIT = 2;

		public const int CTASK_END = 3;

		public const int CHARA_MINAKO = 0;

		public const int CHARA_REPORTER = 1;

		public const int CHARA_WAITRESS = 2;

		public const int CHARA_ARCHITECT = 3;

		public const int CHARA_CUSTOMER_START = 4;

		public const int NODE_COST = 0;

		public const int NODE_HCOST = 1;

		public const int NODE_X = 2;

		public const int NODE_Y = 3;

		public const int NODE_FROM_X = 4;

		public const int NODE_FROM_Y = 5;

		public const int NODE_FIX = 6;

		public const int NODE_QUEUE = 7;

		public const int NODE_END = 8;

		public const int ENDING_YEAR = 8;

		public const int STAY_WEEK = 2;

		public const int EVA_MAX = 1000;

		public const int GAMEEVA_MAX = 100;

		public const int GOBJTYPE_FUKIDASHI = 0;

		public const int GOBJTYPE_PLAYER = 1;

		public const int GOBJTYPE_PLAYER_TRAINING = 2;

		public const int GOBJTYPE_PLAYER_SVISOR = 3;

		public const int GOBJTYPE_MAPCHIP = 4;

		public const int GOBJTYPE_MAPCHIPMAIN = 5;

		public const int GOBJTYPE_MAPCHIPMAIN_IMG = 6;

		public const int GOBJTYPE_MAPOBJECT = 7;

		public const int GOBJTYPE_COM2_SEB = 8;

		public const int GOBJTYPE_BALL = 9;

		public const int GOBJTYPE_TENANT_USE = 10;

		public const int GOBJTYPEBITS_ALL = 65535;

		public const int GOBJP_TYPE = 0;

		public const int GOBJP_ID = 1;

		public const int GOBJP_DEPTH = 2;

		public const int GOBJP_IMG = 3;

		public const int GOBJP_SEB = 4;

		public const int GOBJP_FRAME = 5;

		public const int GOBJP_X = 6;

		public const int GOBJP_Y = 7;

		public const int GOBJP_DARK = 8;

		public const int GOBJP_LAYER = 9;

		public const int GOBJP_END = 10;

		public const int ZSORT_PADDING = 360;

		public const int MAX_Z_PARALLEL = 30;

		public const int ERRMSG_LICENSE_00 = 0;

		public const int ERRMSG_LICENSE_01 = 1;

		public const int ERRMSG_LICENSE_02 = 2;

		public const int ERRMSG_LICENSE_03 = 3;

		public const int ERRMSG_LICENSE_04 = 4;

		public const int ERRMSG_LICENSE_05 = 5;

		public const int ERRMSG_LICENSE_CH = 6;

		public const int ERRMSG_LICENSE_ERROR = 7;

		public const int FILL_MARGIN_W = 4;

		public const int FILL_MARGIN_H = 1;

		public const int STEP_X = 30;

		public const int STEP_Y = 15;

		public const int SIZE_W = 60;

		public const int SIZE_H = 29;

		public const int POPUP_BASETIME = 60;

		private const int SENDSTATE_WAIT = 0;

		private const int SENDSTATE_CONNECTING = 1;

		private static AppData instance_ = null;

		private static Canvas canvas_ = null;

		private static UserData udat_ = null;

		public static readonly int[] APPLI_VERSION_RESOURCE = new int[6];

		public static string CGI_COMMON_QUERY = ((Config.APPLI_TYPE != 0) ? "uid=1&sid=PL91" : "uid=NULLGWDOCOMO");

		public static string URL = null;

		public static string HOST = null;

		public static string URL_CGI = null;

		public static string URL_DAT = null;

		public static string SITE_URL = null;

		public static string VERSIONUP_URL = null;

		public static string[] CGI = new string[4]
		{
			"regist.php?" + CGI_COMMON_QUERY + "&pid=" + 55 + "&ver=" + 202,
			"ranking.php?" + CGI_COMMON_QUERY + "&pid=" + 55 + "&ver=" + 202,
			"regist_score.php?" + CGI_COMMON_QUERY + "&pid=" + 55 + "&ver=" + 202,
			"get_score.php?" + CGI_COMMON_QUERY + "&pid=" + 55 + "&ver=" + 202
		};

		public BootForm frmBoot_;

		public TitleForm frmTitle_;

		public SoccerForm frmSoccer_;

		public GameForm frmGame_;

		public static string[] strDatFileName_ = new string[9] { "data1", "data2", "data3", "data4", "data5", "data6", "data7", "data8", "data9" };

		public static int[] dateFileHead_ = new int[9] { 0, 1, 2, 4, 5, 6, 7, 9, 10 };

		public static readonly string[][] RSFILENAMESS = new string[2][]
		{
			new string[3] { "system", "save1", "save2" },
			new string[12]
			{
				"test", "common2", "map", "event", "image", "object", "xls", "title", "imageMain", "human2",
				"human_head", "sound"
			}
		};

		public ResourceManager resCom2_;

		public ResourceManager resHuman2_;

		public ResourceManager resHumanHead_;

		public ResourceManager resMapChip_;

		public ResourceManager resMapObject_;

		public ResourceManager resMapChipMain_;

		public ResourceManager resEvent_;

		public static readonly string[][] SOUND_FILENAME = new string[2][]
		{
			new string[5] { "soccer_mainA.mld", "soccer_formation.mld", "soccer_gameA.mld", "soccer_gameB.mld", "soccer_title.mld" },
			new string[24]
			{
				"se_06_decide.mld", "soccer_crossbar.mld", "soccer_kanseiA.mld", "soccer_kickA-1.mld", "soccer_kickA-2.mld", "soccer_kickA-3.mld", "soccer_sliding.mld", "soccer_whistleA.mld", "soccer_whistleB.mld", "soccer_whistleC.mld",
				"soccer_kickShoot.mld", "soccer_kickShoot2.mld", "soccer_yorokobi.mld", "soccer_goal-mikata.mld", "soccer_goal-teki.mld", "soccer_fanfale.mld", "soccer_kanashimi.mld", "se_00_kiran1.mld", "se_03_miss.mld", "se_09_bu.mld",
				"department_LevelUp.mld", "se_04.mld", "se_05.mld", "se_01.mld"
			}
		};

		public ResourceManager resSound_;

		public static readonly int[] BGM = new int[5] { 0, 1, 2, 3, 4 };

		public static readonly int[] SE = new int[22]
		{
			5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
			25, 26
		};

		public static readonly string[] softLabels_ = new string[14]
		{
			string.Empty,
			"終了",
			"戻る",
			"セーブ",
			"ﾒﾆｭｰ",
			"ｻｲﾄ",
			"切替",
			"外す",
			"説明",
			"情報",
			"撤去",
			"ベンチ",
			"編成",
			"決定"
		};

		public static readonly Property[] defSysSaveP_ = new Property[0];

		public static readonly int[] defSysSaveI_ = new int[17]
		{
			202, 0, 0, 3, 3, 0, -1, -1, 0, 0,
			0, -1, -1, -1, 0, 1, 0
		};

		public static readonly long[] defSysSaveL_ = new long[4];

		public static readonly string[] defSysSaveS_ = new string[12]
		{
			"記録がありません",
			"記録がありません",
			string.Empty,
			string.Empty,
			"5000",
			"5000",
			string.Empty,
			string.Empty,
			"5000",
			"5000",
			string.Empty,
			string.Empty
		};

		public static readonly byte[][] defSysSaveB_ = JTool.MakeArray<byte[][]>(new int[2] { 6, 0 });

		public Property sysSave_;

		public static readonly Property[] defSaveP_ = new Property[1]
		{
			new Property()
		};

		public static readonly int[] defSaveI_ = new int[17]
		{
			0, 0, 3, 0, 0, 0, 0, 0, 255, -1,
			0, 0, 1, 0, 0, 0, 0
		};

		public static readonly long[] defSaveL_ = new long[3] { 5000L, 0L, 20L };

		public static readonly string[] defSaveS_ = new string[1] { string.Empty };

		public static readonly byte[][] defSaveB_ = JTool.MakeArray<byte[][]>(new int[2] { 17, 0 });

		public Property save_;

		public static int FRAME_TIME1 = 56;

		public Vector popupMessages_ = new Vector();

		public Vector popupScriptBuffer_;

		public static string[] talks_;

		private int NOWSTORE_INDEX;

		public int[] newsTypes_;

		public string[] newsTitles_;

		public string[] newsTexts_;

		public ResourceManager resChip_;

		public ResourceManager resChipSeason_;

		public int loadedChipResourceSeason_;

		public Vector effect_ = new Vector();

		public static int[][][] EFPOS_DATA = new int[7][][];

		public static int EFF_SHOOT_TIME = 6;

		public static int EFF_CHARGE_TIME = 3;

		public static int[] EFF_TUP_TIME = new int[2] { 6, 10 };

		public static int EFF_TLVUP_TIME = 30;

		public static int[] EFF_EXPUP_TIME = new int[2] { 6, 20 };

		public static int[] EFF_RPUP_TIME = new int[2] { 6, 30 };

		public static int[] EFF_MONEYUP_TIME = new int[2] { 6, 20 };

		public static int[] EFF_TUP_DY = new int[2] { -26, -36 };

		public static int[] EFF_EXPUP_DY = new int[2] { -16, -26 };

		public static int[] EFF_RPUP_DY = new int[2] { -16, -26 };

		public static int[] EFF_MONEYUP_DY = new int[2] { -16, -26 };

		public string[] manualTexts_;

		public int offSeason_;

		public int offYear_;

		public int offMonth_;

		public int offWeek_;

		public int offHour_;

		public int offSec_;

		public long offMoney_;

		public int offWeatherAlpha_;

		public int offScore1_;

		public int offScore2_;

		private int[] tmpInt4_ = new int[4];

		public EventData[] eventData_ = new EventData[0];

		public int[] eventNum_;

		public Vector delayEvents_;

		public string[] eventMsgs_;

		public string[] systemMsgs_;

		public static int currentSaveSlot_;

		public string[] characterSource_;

		public string platform_;

		public bool alphaRectPossible_;

		public int scriptArgCharacter_ = -1;

		public static readonly int[][] DIRECT1 = new int[4][]
		{
			new int[2] { 0, -1 },
			new int[2] { 1, 0 },
			new int[2] { 0, 1 },
			new int[2] { -1, 0 }
		};

		public static readonly int[][] DIRECT2 = new int[4][]
		{
			new int[2] { 1, 0 },
			new int[2] { 0, 1 },
			new int[2] { -1, 0 },
			new int[2] { 0, -1 }
		};

		public static readonly int[][] DIRECT3 = new int[4][]
		{
			new int[2] { 0, 1 },
			new int[2] { 0, -1 },
			new int[2] { 1, 0 },
			new int[2] { -1, 0 }
		};

		public int gameObjectPoolIndex_;

		public int[][] gameObjectPool_ = JTool.MakeArray<int[][]>(new int[2] { 768, 10 });

		public int zsortObjectStart_;

		public int zsortObjectLast_;

		public byte[] zsortObjectCount_ = new byte[960];

		public int[][] zsortObjects_ = JTool.MakeArray<int[][]>(new int[2] { 960, 30 });

		public static int FLG_FUKIDASHI_DISP = 1;

		public static int FLG_NYUUKIN = 1;

		public static int FLG_SHOPPING_BAG = 1;

		public static int NUM_MOVESEARCH = 0;

		public static long TIME_MOVESEARCH = 0L;

		public static int ROOPCNT_MOVESEARCH = 0;

		private int[] screen_pos = new int[2];

		private int[] camera_pos = new int[2];

		public Camera camera;

		public Map map_;

		public MapMain mapMain_;

		public PlayerData[] pData_;

		public TeamData[] tData_;

		public SupervisorData[] svisorData_;

		public FormationData[] formationData_;

		public MapchipMatch[] mapchipMatch_;

		public MapchipMain[] mapchipMain_;

		public StadiumData[] stadiumData_;

		public FanActData[] fanActData_;

		public SponsorData[] sponsorData_;

		public FanBonusData[] fanBonus_;

		public QuestMatchData[] qmData_;

		public TenantData[] tenantData_;

		public TradeData[] tradeData_;

		public TrainingData[] trainingData_;

		public LeagueData[] leagueData_;

		public static string[] resourceString;

		public int saveLine_;

		private int[] ef_cpos = new int[2];

		private int[] rotatePoint_tmp = new int[2];

		private Image imgOutline1_;

		private Graphics graOutline1_;

		private Image imgOutline2_;

		private Graphics graOutline2_;

		private int[] drawCommand_tmp = new int[4];

		private int[] getEvenlyDivisionArea_tmp_ = new int[2];

		public int settleX_;

		public int settleY_;

		private int[] drawPopupMessage_heights = new int[100];

		private string temp_str = "１２３４５６７８";

		private int[] draw_helpMessage_tmp1 = new int[0];

		public Vector debugLog_ = new Vector();

		private int sendState_;

		public void SetNowStoreIndex(int nowStore_index)
		{
			NOWSTORE_INDEX = nowStore_index;
		}

		public int GetNowStoreIndex()
		{
			return NOWSTORE_INDEX;
		}

		public static AppData GetInstance()
		{
			if (instance_ == null)
			{
				instance_ = new AppData();
				instance_.Init();
			}
			return instance_;
		}

		private void Init()
		{
			canvas_ = Canvas.GetInstance();
			udat_ = UserData.GetInstance();
			platform_ = IApplication.GetProperty("microedition.platform");
			Config.Init(IApplication.GetProperty("MIDlet-Jar-URL"));
			try
			{
				if (!RecordStore.Setup())
				{
					RecordStore.CreateRecordStore();
				}
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
			RecordStore.SetResourceFiles(RSFILENAMESS[1]);
			Storage.Setup(2, 8, "kairosoft/onsen");
			int[] array = new int[11]
			{
				-1387743643, 321849466, -380916995, 1114766278, 1209944503, 138008561, -893766998, -1242421477, -1230126924, 626883230,
				1684377624
			};
			byte[] array2 = new byte[array.Length * 4];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = (byte)((array[i / 4] >> i % 4 * 8) & 0xFF);
			}
			Encrypter.SetEncryptionKey(array2);
			RecordStore.PreferencesToFile();
			if (Config.DEBUG && !Config.PTW)
			{
				AssetReader.SetSecurity(null, true);
			}
			else
			{
				long[] crcTable = new long[28]
				{
					580282563L, 3896006224L, 1416911673L, 3042983061L, 3698152071L, 1699866191L, 529229099L, 3024621896L, 491867833L, 2309839040L,
					3418722154L, 1915009444L, 2928711961L, 1795527151L, 1348013988L, 534896488L, 199728330L, 1032406477L, 1673000315L, 2194497168L,
					1766122277L, 3102284349L, 1971932774L, 505776908L, 2612373199L, 117867316L, 3309258413L, 2511525444L
				};
				AssetReader.SetSecurity(crcTable, true);
			}
			SurfaceManager.Setup();
			Image image = null;
			Graphics graphics = null;
			try
			{
				image = Image.CreateImage(4, 4);
				graphics = image.GetGraphics();
				graphics.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
				graphics.FillRect(0f, 0f, 4f, 4f);
				graphics.SetRenderMode(1, 128, 128);
				graphics.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				graphics.FillRect(0f, 0f, 4f, 4f);
				graphics.SetRenderMode(0, 255, 0);
				alphaRectPossible_ = false;
			}
			finally
			{
				if (graphics != null)
				{
					graphics.Dispose(false);
				}
				if (image != null)
				{
					image.Dispose();
				}
			}
			frmBoot_ = new BootForm();
			frmTitle_ = new TitleForm();
			frmSoccer_ = new SoccerForm();
			frmGame_ = new GameForm();
			camera = Camera.GetInstance();
		}

		public byte[][] PhpDlFile(string dir, string file)
		{
			string url = Config.URL_DATA + ((dir == null) ? (string.Empty) : ((dir + "/"))) + file + ".dat";
			return Http.Connect(url, null, false, true);
		}

		public byte[] CgiRegist(string cm, string r)
		{
			if (!Config.REGIST_ENABLE)
			{
				return ("1\n" + 202).GetBytes();
			}
			string url = CgiRegistURL(cm, r);
			byte[] bytes = Http.Connect(url, null, true);
			return (JString.New(bytes) + "\n" + 202).GetBytes();
		}

		public string CgiRegistURL(string cm, string r)
		{
			cm = MakeQuery("&cm=", cm);
			r = MakeQuery("&r=", r);
			return Config.URL_REGIST + "&ver=" + 202 + cm + r;
		}

		public byte[] CgiRegistScore(string m, string s, string r)
		{
			string url = CgiRegistScoreURL(m, s, r);
			return Http.Connect(url, null, false);
		}

		public string CgiRegistScoreURL(string m, string s, string r)
		{
			m = MakeQuery("&m=", m);
			s = MakeQuery("&s=", s);
			r = MakeQuery("&r=", r);
			return Config.URL_REGIST_SCORE + "&ver=" + 202 + m + s + r;
		}

		public byte[] CgiGetScore(string m)
		{
			string url = CgiGetScoreURL(m);
			return Http.Connect(url, null, false);
		}

		public string CgiGetScoreURL(string m)
		{
			m = MakeQuery("&m=", m);
			return Config.URL_GET_SCORE + "&ver=" + 202 + m;
		}

		public byte[] CgiRanking(string m, string r, string p)
		{
			string url = CgiRankingURL(m, r, p);
			return Http.Connect(url, null, false);
		}

		public string CgiRankingURL(string m, string r, string p)
		{
			m = MakeQuery("&m=", m);
			r = MakeQuery("&r=", r);
			p = MakeQuery("&p=", p);
			return Config.URL_RANKING + "&ver=" + 202 + m + r + p;
		}

		public static string MakeQuery(string query, string param)
		{
			if (param == null)
			{
				return string.Empty;
			}
			return query + param;
		}

		public bool IsPushSoftLabel(int softlabel)
		{
			return IsPushSoftLabel(softlabel, 0);
		}

		public bool IsPushStateSoftLabel(int softlabel)
		{
			string softLabelL = canvas_.GetSoftLabelL();
			string softLabelR = canvas_.GetSoftLabelR();
			if (softLabelL.Equals(softLabels_[softlabel]) && canvas_.CheckKeyState(2097152))
			{
				return true;
			}
			if (softLabelR.Equals(softLabels_[softlabel]) && canvas_.CheckKeyState(4194304))
			{
				return true;
			}
			return false;
		}

		public bool IsPushSoftLabel(int softlabel, int key)
		{
			string softLabelL = canvas_.GetSoftLabelL();
			string softLabelR = canvas_.GetSoftLabelR();
			if ((softLabelL.Equals(softLabels_[softlabel]) && canvas_.CheckKeyPulse(2097152)) || canvas_.CheckKeyPulse(key))
			{
				return true;
			}
			if ((softLabelR.Equals(softLabels_[softlabel]) && canvas_.CheckKeyPulse(4194304)) || canvas_.CheckKeyPulse(key))
			{
				return true;
			}
			return false;
		}

		public static int Clamp(int val, int min, int max)
		{
			if (val > max)
			{
				val = max;
			}
			if (val < min)
			{
				val = min;
			}
			return val;
		}

		public static float Clamp(float val, float min, float max)
		{
			if (val > max)
			{
				val = max;
			}
			if (val < min)
			{
				val = min;
			}
			return val;
		}

		public static long Clamp(long val, long min, long max)
		{
			if (val > max)
			{
				val = max;
			}
			if (val < min)
			{
				val = min;
			}
			return val;
		}

		public static int ClampMin(int val, int min)
		{
			if (val < min)
			{
				return min;
			}
			return val;
		}

		public static int ClampMax(int val, int max)
		{
			if (val > max)
			{
				return max;
			}
			return val;
		}

		public static long ClampMin(long val, long min)
		{
			if (val < min)
			{
				return min;
			}
			return val;
		}

		public static long ClampMax(long val, long max)
		{
			if (val > max)
			{
				return max;
			}
			return val;
		}

		public static int Max(int v1, int v2)
		{
			if (v1 < v2)
			{
				return v2;
			}
			return v1;
		}

		public static int Min(int v1, int v2)
		{
			if (v1 < v2)
			{
				return v1;
			}
			return v2;
		}

		public void InitSystemSaveData()
		{
			sysSave_ = new Property(defSysSaveP_, defSysSaveI_, defSysSaveL_, defSysSaveS_, defSysSaveB_);
			if (Language.Japanese())
			{
				sysSave_.ints[14] = 0;
			}
			else
			{
				sysSave_.ints[14] = 1;
			}
			if (Config.MUTE_SE && platform_.StartsWith("ap"))
			{
				sysSave_.ints[4] = 0;
			}
		}

		public bool LoadSystem()
		{
			RecordStore recordStore = null;
			try
			{
				recordStore = RecordStore.OpenRecordStore(0);
				if (recordStore.GetNumRecords() == 0)
				{
					return false;
				}
				byte[] data = RecordStore.ReadRecord(0, 0);
				InitSystemSaveData();
				sysSave_.Load(data);
			}
			finally
			{
				if (recordStore != null)
				{
					recordStore.CloseRecordStore();
				}
				recordStore = null;
			}
			if (Config.MUTE_SE)
			{
				sysSave_.ints[4] = 0;
			}
			return true;
		}

		public void SaveSystem()
		{
			RecordStore recordStore = null;
			try
			{
				recordStore = RecordStore.OpenRecordStore(0);
				recordStore.WriteRecord(0, sysSave_.ToByteArray());
				if (Config.DEBUG_CMD && Config.PRINT)
				{
					Log.Info("■AppData.java saveSystem() sysSave_.toByteArray().length = " + sysSave_.ToByteArray().Length);
				}
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
			finally
			{
				if (recordStore != null)
				{
					recordStore.CloseRecordStore();
				}
				recordStore = null;
			}
		}

		public void NewGame(int slot, string name)
		{
			currentSaveSlot_ = slot;
			sysSave_.ints[8] = slot;
			SaveSystem();
			save_ = new Property(defSaveP_, defSaveI_, defSaveL_, defSaveS_, defSaveB_);
			save_.strs[0] = name;
			save_.ints[4] = GameUtil.RateConvert(0, 0, 144, 0, 10800, true);
			eventNum_ = new int[eventData_.Length];
			delayEvents_ = new Vector();
			IApplication currentApp = IApplication.GetCurrentApp();
			if (currentApp.GetScaleRatio(false) % 100f != 0f && currentApp.GetWidth() > 480 && currentApp.GetHeight() > 480)
			{
				save_.ints[12] = 0;
				canvas_.SetLinearFilterEnable(false);
			}
			canvas_.SetLinearFilterEnable(save_.ints[12] == 1);
			try
			{
				SaveSystem();
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
		}

		public void SaveGame()
		{
			SaveGame(false);
		}

		public void SaveGame(bool auto)
		{
			saveLine_ = 0;
			RecordStore recordStore = null;
			ByteArrayOutputStream byteArrayOutputStream = null;
			if (Config.PRINT && save_ == null)
			{
				Log.Info("■AppData.java saveGame() save_ == null");
			}
			try
			{
				save_.ints[15] = 1;
				saveLine_ = 0;
				save_.pros[0].ints = eventNum_;
				saveLine_ = 1;
				byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.WriteInt(byteArrayOutputStream, delayEvents_.Size());
				for (int i = 0; i < delayEvents_.Size(); i++)
				{
					saveLine_ = 2;
					DelayEvent delayEvent = (DelayEvent)delayEvents_.ElementAt(i);
					saveLine_ = 3;
					delayEvent.Serialize(byteArrayOutputStream);
				}
				save_.bins[16] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 4;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int j = 0; j < pData_.Length; j++)
				{
					pData_[j].Serialize(byteArrayOutputStream);
				}
				save_.bins[0] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 5;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int k = 0; k < qmData_.Length; k++)
				{
					qmData_[k].Serialize(byteArrayOutputStream);
				}
				save_.bins[1] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 6;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int l = 0; l < sponsorData_.Length; l++)
				{
					sponsorData_[l].Serialize(byteArrayOutputStream);
				}
				save_.bins[2] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 7;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int m = 0; m < formationData_.Length; m++)
				{
					formationData_[m].Serialize(byteArrayOutputStream);
				}
				save_.bins[3] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 8;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int n = 0; n < stadiumData_.Length; n++)
				{
					stadiumData_[n].Serialize(byteArrayOutputStream);
				}
				save_.bins[4] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 9;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num = 0; num < svisorData_.Length; num++)
				{
					svisorData_[num].Serialize(byteArrayOutputStream);
				}
				save_.bins[5] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 10;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num2 = 0; num2 < tData_.Length; num2++)
				{
					tData_[num2].Serialize(byteArrayOutputStream);
				}
				save_.bins[6] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 11;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num3 = 0; num3 < tenantData_.Length; num3++)
				{
					tenantData_[num3].Serialize(byteArrayOutputStream);
				}
				save_.bins[7] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 12;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num4 = 0; num4 < fanActData_.Length; num4++)
				{
					fanActData_[num4].Serialize(byteArrayOutputStream);
				}
				save_.bins[8] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 13;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num5 = 0; num5 < fanBonus_.Length; num5++)
				{
					fanBonus_[num5].Serialize(byteArrayOutputStream);
				}
				save_.bins[9] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 14;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num6 = 0; num6 < tradeData_.Length; num6++)
				{
					tradeData_[num6].Serialize(byteArrayOutputStream);
				}
				save_.bins[10] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 15;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num7 = 0; num7 < trainingData_.Length; num7++)
				{
					trainingData_[num7].Serialize(byteArrayOutputStream);
				}
				save_.bins[11] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 16;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num8 = 0; num8 < trainingData_.Length; num8++)
				{
					trainingData_[num8].Serialize(byteArrayOutputStream);
				}
				save_.bins[11] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 17;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num9 = 0; num9 < leagueData_.Length; num9++)
				{
					leagueData_[num9].Serialize(byteArrayOutputStream);
				}
				save_.bins[12] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 18;
				byteArrayOutputStream = new ByteArrayOutputStream();
				camera.Serialize(byteArrayOutputStream);
				save_.bins[13] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 19;
				byteArrayOutputStream = new ByteArrayOutputStream();
				udat_.Serialize(byteArrayOutputStream);
				save_.bins[14] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 20;
				byteArrayOutputStream = new ByteArrayOutputStream();
				for (int num10 = 0; num10 < udat_.tenants_.Length; num10++)
				{
					udat_.tenants_[num10].Serialize(byteArrayOutputStream);
				}
				save_.bins[15] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				saveLine_ = 21;
				recordStore = RecordStore.OpenRecordStore(0);
				recordStore.WriteRecord(((auto) ? (3) : (1)) + currentSaveSlot_, save_.ToByteArray());
				saveLine_ = 31;
				sysSave_.ints[((auto) ? (11) : (6)) + currentSaveSlot_] = save_.ints[1] * 100 * 100 + save_.ints[2] * 100 + save_.ints[3];
				sysSave_.strs[((auto) ? (8) : (4)) + currentSaveSlot_] = JString.ValueOf(save_.lngs[0]);
				sysSave_.strs[((auto) ? (6) : (2)) + currentSaveSlot_] = save_.strs[0];
				saveLine_ = 32;
				SaveSystem();
				if (Config.PRINT)
				{
					int num11 = save_.ToByteArray().Length;
					Log.Info("save:" + num11 + "byte (" + num11 / 1024 + "." + num11 % 1024 * 100 / 1024 + "kb)");
				}
			}
			finally
			{
				if (byteArrayOutputStream != null)
				{
					byteArrayOutputStream.Close();
				}
				byteArrayOutputStream = null;
				if (recordStore != null)
				{
					recordStore.CloseRecordStore();
				}
				recordStore = null;
			}
		}

		public void LoadGame(int slot)
		{
			LoadGame(slot, false);
		}

		public void LoadGame(int slot, bool auto)
		{
			RecordStore recordStore = null;
			ByteArrayInputStream byteArrayInputStream = null;
			currentSaveSlot_ = slot;
			sysSave_.ints[8] = slot;
			SaveSystem();
			try
			{
				recordStore = RecordStore.OpenRecordStore(0);
				int num;
byte[] data = RecordStore.ReadRecord(0, ((auto) ? (3) : (1)) + currentSaveSlot_);
				save_ = new Property(defSaveP_, defSaveI_, defSaveL_, defSaveS_, defSaveB_);
				save_.Load(data);
				eventNum_ = new int[eventData_.Length];
				JSystem.Arraycopy(save_.pros[0].ints, 0, eventNum_, 0, save_.pros[0].ints.Length);
				delayEvents_ = new Vector();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[16]);
				num = StreamUtil.ReadInt(byteArrayInputStream);
				for (int i = 0; i < num; i++)
				{
					DelayEvent delayEvent = new DelayEvent();
					delayEvent.Deserialize(byteArrayInputStream);
					delayEvents_.AddElement(delayEvent);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[0]);
				for (int j = 0; j < pData_.Length; j++)
				{
					pData_[j].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[1]);
				for (int k = 0; k < qmData_.Length; k++)
				{
					qmData_[k].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[2]);
				for (int l = 0; l < sponsorData_.Length; l++)
				{
					sponsorData_[l].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[3]);
				for (int m = 0; m < formationData_.Length; m++)
				{
					formationData_[m].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[4]);
				for (int n = 0; n < stadiumData_.Length; n++)
				{
					stadiumData_[n].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[5]);
				for (int num2 = 0; num2 < svisorData_.Length; num2++)
				{
					svisorData_[num2].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[6]);
				for (int num3 = 0; num3 < tData_.Length; num3++)
				{
					tData_[num3].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[7]);
				for (int num4 = 0; num4 < tenantData_.Length; num4++)
				{
					tenantData_[num4].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[8]);
				for (int num5 = 0; num5 < fanActData_.Length; num5++)
				{
					fanActData_[num5].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[9]);
				for (int num6 = 0; num6 < fanBonus_.Length; num6++)
				{
					fanBonus_[num6].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[10]);
				for (int num7 = 0; num7 < tradeData_.Length; num7++)
				{
					tradeData_[num7].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[11]);
				for (int num8 = 0; num8 < trainingData_.Length; num8++)
				{
					trainingData_[num8].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[12]);
				for (int num9 = 0; num9 < leagueData_.Length; num9++)
				{
					leagueData_[num9].Deserialize(byteArrayInputStream);
				}
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[13]);
				camera.Deserialize(byteArrayInputStream);
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[14]);
				udat_.Deserialize(byteArrayInputStream);
				byteArrayInputStream.Close();
				byteArrayInputStream = new ByteArrayInputStream(save_.bins[15]);
				for (int num10 = 0; num10 < udat_.tenants_.Length; num10++)
				{
					udat_.tenants_[num10].Deserialize(byteArrayInputStream, num10);
				}
				byteArrayInputStream.Close();
				udat_.Deserialize2();
				for (int num11 = 0; num11 < udat_.tenants_.Length; num11++)
				{
					udat_.tenants_[num11].Deserialize2();
				}
			}
			finally
			{
				if (byteArrayInputStream != null)
				{
					byteArrayInputStream.Close();
				}
				byteArrayInputStream = null;
				if (recordStore != null)
				{
					recordStore.CloseRecordStore();
				}
				recordStore = null;
			}
			mapMain_.UpdateTenantMapcip();
			canvas_.SetLinearFilterEnable(save_.ints[12] == 1);
			frmGame_.SetAllDraw();
		}

		public void DrawWindow(Graphics g, int GWidth, int GHeight, int GTop, string T_S)
		{
			DrawWindow(g, GWidth, GHeight, GTop, T_S, 0);
		}

		public void DrawWindow(Graphics g, int GWidth, int GHeight, int GTop, string T_S, int off_y)
		{
			Font font = g.GetFont(null);
			int num = 240;
			int num2 = 240;
			int num3 = GameForm.VIEW_Y + off_y * 2;
			if (GHeight > 216)
			{
				num3 = 0;
			}
			g.SetColor(Graphics.GetColorOfRGB(89, 103, 91));
			g.DrawRect(num / 2 - GWidth / 2 - 1, (num3 + num2) / 2 - GHeight / 2 - 1 + GTop - 1, GWidth + 1, GHeight + 2);
			g.SetColor(Graphics.GetColorOfRGB(239, 239, 221));
			g.DrawRect(num / 2 - GWidth / 2, (num3 + num2) / 2 - GHeight / 2 - 1 + GTop, GWidth - 1, GHeight);
			int num4 = GWidth / 40 + 1;
			for (int i = 0; i < num4; i++)
			{
				int num5 = num / 2 - GWidth / 2 + i * 40;
				int width = ClampMax(GWidth - i * 40, 40);
				g.DrawImage(resCom2_.img[28], num5, (num3 + num2) / 2 - GHeight / 2 + GTop, 0, 0, width, GHeight);
			}
			g.DrawImage(resCom2_.img[29], 120 - GWidth / 2 + 1, (num3 + num2) / 2 - GHeight / 2 + GTop, 120 - GWidth / 2 + 1, 0, GWidth - 2, 17);
			if (T_S == null) { return; }
{
g.SetColor(SubForm.SC_WINDOW_TITLE2);
g.DrawString(T_S, 120 - font.StringWidth(T_S) / 2, (num3 + num2) / 2 - GHeight / 2 - 1 + GTop + 3 + 1);
g.SetColor(SubForm.SC_WINDOW_TITLE);
g.DrawString(T_S, 120 - font.StringWidth(T_S) / 2, (num3 + num2) / 2 - GHeight / 2 - 1 + GTop + 3);
}
		}

		public void DrawWindow2(Graphics g, int GWidth, int GHeight, int GTop, int oy)
		{
			oy--;
			int originX = g.GetOriginX();
			int originY = g.GetOriginY();
			g.SetOrigin(0, 0);
			GameView instance = GameView.GetInstance();
			Image image = resCom2_.img[28];
			g.DrawScaledImage(image, 0f, oy, instance.GetGameWidth(), instance.GetGameHeight() - oy, 0, 0, image.GetWidth(), image.GetHeight());
			g.SetOrigin(originX, originY);
		}

		public void DrawBox(Graphics g, int BoxX1, int BoxY1, int BoxX2, int BoxY2)
		{
			int num = BoxX2 - BoxX1;
			int num2 = BoxY2 - BoxY1;
			BoxY1 += SoccerForm.VIEW_Y / 2;
			BoxY2 += SoccerForm.VIEW_Y / 2;
			g.SetColor(Graphics.GetColorOfRGB(247, 253, 247));
			g.FillRect(BoxX1, BoxY1, num, num2);
			g.SetColor(Graphics.GetColorOfRGB(172, 202, 179));
			g.DrawRect(BoxX1, BoxY1, num - 1, num2 - 1);
			g.SetColor(Graphics.GetColorOfRGB(222, 234, 225));
			g.DrawRect(BoxX1 + 1, BoxY1 + 1, num - 2 - 1, num2 - 2 - 1);
			resCom2_.seb[27].DrawFrame(g, BoxX1, BoxY1, resCom2_.img, 0);
			resCom2_.seb[27].DrawFrame(g, BoxX2, BoxY1, resCom2_.img, 1);
			resCom2_.seb[27].DrawFrame(g, BoxX2, BoxY2, resCom2_.img, 2);
			resCom2_.seb[27].DrawFrame(g, BoxX1, BoxY2, resCom2_.img, 3);
		}

		public void DrawArcRect(Graphics g, int x, int y, int w, int h, int r)
		{
			g.DrawLine(x, y + r, x, y + h - r);
			g.DrawLine(x + w, y + r, x + w, y + h - r);
			g.DrawLine(x + r, y, x + w - r, y);
			g.DrawLine(x + r, y + h, x + w - r, y + h);
			g.DrawArc(x, y, r * 2, r * 2, 90f, 90f);
			g.DrawArc(x, y + h - r * 2, r * 2, r * 2, 180f, 90f);
			g.DrawArc(x + w - r * 2, y, r * 2, r * 2, 0f, 90f);
			g.DrawArc(x + w - r * 2, y + h - r * 2, r * 2, r * 2, 270f, 90f);
		}

		public void FillArcRect(Graphics g, int x, int y, int w, int h, int r)
		{
			int num = r * 2;
			g.FillRect(x, y + r, r, h - num);
			g.FillRect(x + w - r, y + r, r, h - num);
			g.FillRect(x + r, y, w - num, r);
			g.FillRect(x + r, y + h - r, w - num, r);
			g.FillRect(x + r, y + r, w - num, h - num);
			g.FillArc(x, y, r * 2, r * 2, 90f, 90f);
			g.FillArc(x, y + h - r * 2, r * 2, r * 2, 180f, 90f);
			g.FillArc(x + w - r * 2, y, r * 2, r * 2, 0f, 90f);
			g.FillArc(x + w - r * 2, y + h - r * 2, r * 2, r * 2, 270f, 90f);
		}

		public void DrawFillArcRect(Graphics g, int x, int y, int w, int h, int r, int r0, int g0, int b0, int r1, int g1, int b1)
		{
			g.SetColor(Graphics.GetColorOfRGB(r0, g0, b0));
			FillArcRect(g, x + 1, y + 1, w - 2, h - 2, r);
			g.SetColor(Graphics.GetColorOfRGB(r1, g1, b1));
			DrawArcRect(g, x, y, w - 1, h - 1, r);
		}

		public void Draw3DBox(Graphics g, int offset_x, int offset_y, int width, int height, int[][] colors)
		{
			g.SetColor(Graphics.GetColorOfRGB(colors[0][0], colors[0][1], colors[0][2]));
			g.DrawRect(offset_x, offset_y, width - 1, height - 1);
			int num = offset_x + 1;
			int num2 = offset_y + 1;
			int num3 = width - 2;
			int num4 = height - 2;
			g.SetColor(Graphics.GetColorOfRGB(colors[2][0], colors[2][1], colors[2][2]));
			g.DrawLine(num, num2, num + num3 - 1, num2);
			g.DrawLine(num, num2 + 1, num, num2 + num4 - 2);
			g.SetColor(Graphics.GetColorOfRGB(colors[3][0], colors[3][1], colors[3][2]));
			g.DrawLine(num, num2 + num4 - 1, num + num3 - 1, num2 + num4 - 1);
			g.DrawLine(num + num3 - 1, num2 + 1, num + num3 - 1, num2 + num4 - 2);
			int num5 = offset_x + 2;
			int num6 = offset_y + 2;
			int num7 = width - 4;
			int num8 = height - 4;
			g.SetColor(Graphics.GetColorOfRGB(colors[1][0], colors[1][1], colors[1][2]));
			g.FillRect(num5, num6, num7, num8);
		}

		public bool IsTopForm(FormBase formBase)
		{
			FormManager instance = FormManager.GetInstance();
			return formBase == instance.GetTopForm();
		}

		public int DrawSeb(Graphics g, int x, int y, Seb seb, Image[] imgs, int frame, int w1000, int h1000)
		{
			return DrawSeb(g, x, y, seb, imgs, frame, w1000, h1000, -1, -1);
		}

		public int DrawSeb(Graphics g, int x, int y, Seb seb, Image[] imgs, int frame, int w1000, int h1000, int maxWidth, int maxHeight)
		{
			int num = x;
			w1000 = Clamp(w1000, 0, 1000);
			h1000 = Clamp(h1000, 0, 1000);
			if (w1000 <= 0 || h1000 <= 0)
			{
				return num;
			}
			int[][] sprites = seb.GetSprites(frame);
			for (int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++) {
int[] array = sprites[spriteIndex];
int num2 = maxWidth;
int num3 = maxHeight;
if (num2 == -1)
				{
					num2 = array[4];
				}
if (num3 == -1)
				{
					num3 = array[5];
				}
Image image = imgs[array[1]];
g.DrawImage(image, x + array[6], y + array[6], array[2], array[3], num2 * w1000 / 1000, num3 * h1000 / 1000);
int num4 = x + array[6] + num2 * w1000 / 1000;
if (num < num4)
				{
					num = num4;
				}
}
			return num;
		}

		public void DrawSeb(Graphics g, int x, int y, Seb seb, Image[] imgs, int frame, int ucnt)
		{
			int[][] sprites = seb.GetSprites(frame);
			for (int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++) {
int[] array = sprites[spriteIndex];
Image image = imgs[array[1]];
if (array[2] + ucnt * array[4] + array[4] < image.GetWidth())
				{
					g.DrawImage(image, x + array[6], y + array[7], array[2] + ucnt * array[4], array[3], array[4], array[5]);
				}
				else
				{
					g.DrawImage(image, x + array[6], y + array[7], array[2] + ucnt * array[4], array[3], array[4], array[5]);
				}
}
		}

		public void DrawSebV(Graphics g, int x, int y, Seb seb, Image[] imgs, int frame, int vcnt)
		{
			int[][] sprites = seb.GetSprites(frame);
			for (int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++) {
int[] array = sprites[spriteIndex];
Image image = imgs[array[1]];
if (array[3] + vcnt * array[5] + array[5] < image.GetHeight())
				{
					g.DrawImage(image, x + array[6], y + array[7], array[2], array[3] + vcnt * array[5], array[4], array[5]);
				}
				else
				{
					g.DrawImage(image, x + array[6], y + array[7], array[2], array[3] + vcnt * array[5], array[4], array[5]);
				}
}
		}

		public void DrawSeb(Graphics g, int x, int y, Seb seb, Image img, int frame)
		{
			int[][] sprites = seb.GetSprites(frame);
			for (int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++) {
int[] array = sprites[spriteIndex];
if (array[1] < 0) { continue; }
g.DrawImage(img, x + array[6], y + array[7], array[2], array[3], array[4], array[5]);
}
		}

		public void DrawSebReverse(Graphics g, int x, int y, Seb seb, int frame, Image[] imgs)
		{
			int[][] sprites = seb.GetSprites(frame);
			for (int spriteIndex = 0; spriteIndex < sprites.Length; spriteIndex++) {
int[] array = sprites[spriteIndex];
if (array[1] < 0) { continue; }
g.DrawImage(imgs[array[1]], x - array[6] - array[4], y + array[7], array[2], array[3], array[4], array[5]);
}
		}

		public static bool ColRect(int x, int y, int left, int top, int right, int bottom)
		{
			return left < x && x < right && top < y && y < bottom;
		}

		public static bool ColRect(int left1, int top1, int right1, int bottom1, int left2, int top2, int right2, int bottom2)
		{
			if (right1 < left2 || bottom1 < top2 || left1 > right2 || top1 > bottom2)
			{
				return false;
			}
			return true;
		}

		public static bool ColRect(float left1, float top1, float right1, float bottom1, float left2, float top2, float right2, float bottom2)
		{
			if (right1 < left2)
			{
				return false;
			}
			if (bottom1 < top2)
			{
				return false;
			}
			if (left1 > right2)
			{
				return false;
			}
			if (top1 > bottom2)
			{
				return false;
			}
			return true;
		}

		public static string CommaSeparate(long value)
		{
			bool flag = value < 0;
			if (flag)
			{
				value = -value;
			}
			string str = string.Empty + value;
			string text = string.Empty;
			int num = 0;
			int num2 = str.Length() - 1;
			while (num2 >= 0)
			{
				if (num > 0 && num % 3 == 0)
				{
					text = ',' + text;
				}
				text = str.CharAt(num2) + text;
				num2--;
				num++;
			}
			if (flag)
			{
				text = "-" + text;
			}
			return text;
		}

		public static string CommaSeparate_en(long value)
		{
			bool flag = GetInstance().sysSave_.ints[14] == 1;
			bool flag2 = value < 0;
			if (flag2)
			{
				value = -value;
			}
			string str = string.Empty + value;
			string text = string.Empty;
			int num = 0;
			if (flag)
			{
				int num2 = str.Length() - 1;
				while (num2 >= 0)
				{
					if (num == 1)
					{
						text = "." + text;
					}
					if (num > 1 && (num - 1) % 3 == 0)
					{
						text = ',' + text;
					}
					text = str.CharAt(num2) + text;
					num2--;
					num++;
				}
			}
			else if (Language.English())
			{
				str = string.Empty + value * 10;
				int num3 = str.Length() - 1;
				while (num3 >= 0)
				{
					if (num > 0 && num % 3 == 0)
					{
						text = ',' + text;
					}
					text = str.CharAt(num3) + text;
					num3--;
					num++;
				}
			}
			else
			{
				int num4 = str.Length() - 1;
				while (num4 >= 0)
				{
					if (num > 0 && num % 3 == 0)
					{
						text = ',' + text;
					}
					text = str.CharAt(num4) + text;
					num4--;
					num++;
				}
			}
			if (flag2)
			{
				text = "-" + text;
			}
			return text;
		}

		public int[] GetBRectSeb(Seb seb, int x, int y, int frame)
		{
			int[] boundingRect = seb.GetBoundingRect();
			tmpInt4_[0] = x + boundingRect[0];
			tmpInt4_[1] = y + boundingRect[1];
			tmpInt4_[2] = boundingRect[2];
			tmpInt4_[3] = boundingRect[3];
			return tmpInt4_;
		}

		public void DrawVerticalScroll2(Graphics g, int x, int y, int w, int h, int value, int maximum, int largeChange)
		{
			if (maximum < largeChange - 1)
			{
				maximum = largeChange - 1;
			}
			GameView.AddTouch(g, 12, x, y, w - 1, h, 262144, TouchOption.Create(new int[3]
			{
				maximum + 1,
				largeChange,
				131072
			}));
		}

		public void DrawVerticalScroll3(Graphics g, int x, int y, int w, int h, int value, int maximum, int largeChange)
		{
			if (maximum < largeChange - 1)
			{
				maximum = largeChange - 1;
			}
			GameView.AddTouch(g, 22, x, y, w - 1, h, 262144, TouchOption.Create(new int[3] { maximum, largeChange, 851968 }));
		}

		public void DrawNumImage(Graphics g, int seb, string header, long value, string footer, int x, int y)
		{
			DrawNumImage(g, seb, header, value, footer, x, y, 0, 1);
		}

		public void DrawNumImage(Graphics g, int seb, string header, long value, string footer, int x, int y, int padding)
		{
			DrawNumImage(g, seb, header, value, footer, x, y, padding, 1);
		}

		public void DrawNumImage(Graphics g, int seb, long value, int x, int y, int padding, int anchor)
		{
			DrawNumImage(g, seb, string.Empty, value, string.Empty, x, y, padding, anchor);
		}

		public void DrawNumImage(Graphics g, int seb, long value, int x, int y)
		{
			DrawNumImage(g, seb, string.Empty, value, string.Empty, x, y, 0, 1);
		}

		public void DrawNumImage(Graphics g, int seb, string header, long value, string footer, int x, int y, int padding, int anchor)
		{
			ResourceManager resourceManager = resCom2_;
			int num = resourceManager.seb[seb].GetSprite(0, 0)[4];
			long num2 = 1L;
			long num3 = 1L;
			long num4 = value;
			while (num4 / 10 > 0)
			{
				num4 /= 10;
				num3 *= 10;
				num2++;
			}
			long num5 = num2 + header.Length() + footer.Length();
			num5 = num5 * (num + padding) - padding;
			if (BitUtil.Check(anchor, 2))
			{
				x -= (int)(num5 / 2);
			}
			else if (BitUtil.Check(anchor, 4))
			{
				x -= (int)num5;
			}
			for (int i = 0; i < header.Length(); i++)
			{
				char c = header.CharAt(i);
				if (c != ' ')
				{
					resourceManager.seb[seb].DrawFrame(g, x, y, resourceManager.img, c - 48);
				}
				x += num + padding;
			}
			num4 = value;
			for (int j = 0; j < num2; j++)
			{
				resourceManager.seb[seb].DrawFrame(g, x, y, resourceManager.img, (int)(num4 / num3));
				num4 %= num3;
				num3 /= 10;
				x += num + padding;
			}
			for (int k = 0; k < footer.Length(); k++)
			{
				char c2 = footer.CharAt(k);
				if (c2 != ' ')
				{
					resourceManager.seb[seb].DrawFrame(g, x, y, resourceManager.img, c2 - 48);
				}
				x += num + padding;
			}
		}

		public void DrawNumImageComma(Graphics g, int seb, long in_value, int x, int y, int padding, int comma_padding, int anchor)
		{
			string str = CommaSeparate(in_value);
			int num = 0;
			num += JString.ValueOf(in_value).Length() * (8 + padding) - padding;
			if ((anchor == 2)) { x -= num / 2; } else if ((anchor == 4)) { x -= num; }
			bool flag = false;
			for (int i = 0; i < str.Length(); i++)
			{
				char c = str.CharAt(i);
				if (c == ',')
				{
					flag = true;
					continue;
				}
				resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, c - 48);
				if (flag)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y, resCom2_.img, 10);
					flag = false;
				}
				x += 8 + padding;
			}
		}

		public void DrawNumImageComma_en(Graphics g, int seb, long in_value, int x, int y, int padding, int comma_padding, int anchor)
		{
			string str = CommaSeparate_en(in_value);
			int num = 0;
			num += JString.ValueOf(in_value).Length() * (8 + padding) - padding;
			if ((anchor == 2)) { x -= num / 2; } else if ((anchor == 4)) { x -= num; }
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < str.Length(); i++)
			{
				char c = str.CharAt(i);
				if ((c == ',')) { flag = true;
continue; } else if ((c == '.')) { flag2 = true;
continue; }
				resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, c - 48);
				if (flag)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y, resCom2_.img, 10);
					flag = false;
				}
				else if (flag2)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y, resCom2_.img, 10);
					flag2 = false;
				}
				x += 8 + padding;
			}
		}

		public void DrawMoney(Graphics g, int seb, long in_value, int x, int y, int padding, int comma_padding, int anchor)
		{
			DrawMoney(g, seb, in_value, x, y, padding, comma_padding, anchor, true);
		}

		public void DrawMoney(Graphics g, int seb, long in_value, int x, int y, int padding, int comma_padding, int anchor, bool kmark)
		{
			bool flag = false;
			if (IsLanguageOther())
			{
				flag = true;
				in_value *= 10;
				x -= 8;
			}
			string str = CommaSeparate_en(in_value);
			bool flag2 = GetInstance().sysSave_.ints[14] == 1;
			if (!kmark)
			{
				str = CommaSeparate(in_value);
				if (!flag2)
				{
					str = CommaSeparate(in_value * 100);
				}
			}
			if (flag)
			{
				str = CommaSeparate((kmark) ? (in_value) : ((in_value / 100)));
				kmark = false;
			}
			int num = 0;
			num += JString.ValueOf(in_value).Length() * (8 + padding) - padding + 17;
			if (!flag2)
			{
				num += 8;
			}
			if (!kmark)
			{
				num -= 7;
			}
			if ((anchor == 2)) { x -= num / 2; } else if ((anchor == 4)) { x -= num; }
			if (!flag)
			{
				if (flag2)
				{
					resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, 11);
				}
				else
				{
					resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, 17);
				}
			}
			x += 10 + padding;
			bool flag3 = false;
			bool flag4 = false;
			for (int i = 0; i < str.Length(); i++)
			{
				char c = str.CharAt(i);
				if ((c == ',')) { flag3 = true;
continue; } else if ((c == '.')) { flag4 = true;
continue; }
				if (flag3)
				{
					x += comma_padding;
				}
				if (flag4)
				{
					x++;
				}
				resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, c - 48);
				if (flag3)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y, resCom2_.img, 10);
					flag3 = false;
				}
				else if (flag4)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y + 1, resCom2_.img, 19);
					flag4 = false;
				}
				x += 8 + padding;
			}
			if (kmark)
			{
				resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, 12);
			}
			if (flag)
			{
				g.DrawImage(resCom2_.img[193], x, y, 37, 54, 9, 10);
			}
		}

		public void DrawMoney2(Graphics g, int seb, long in_value, int x, int y, int padding, int comma_padding, int anchor)
		{
			bool flag = false;
			if (IsLanguageOther())
			{
				flag = true;
				in_value *= 10;
				x += 8;
			}
			string str = CommaSeparate_en(in_value);
			if (flag)
			{
				str = CommaSeparate(in_value);
			}
			int num = 0;
			num += JString.ValueOf(in_value).Length() * (8 + padding) - padding + 7;
			if ((anchor == 2)) { x -= num / 2; } else if ((anchor == 4)) { x -= num; }
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < str.Length(); i++)
			{
				char c = str.CharAt(i);
				if ((c == ',')) { flag2 = true;
continue; } else if ((c == '.')) { flag3 = true;
continue; }
				if (flag2)
				{
					x += comma_padding;
				}
				if (flag3)
				{
					x += comma_padding;
				}
				resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, c - 48);
				if (flag2)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y, resCom2_.img, 10);
					flag2 = false;
				}
				else if (flag3)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y, resCom2_.img, 10);
					flag3 = false;
				}
				x += 8 + padding;
			}
			if (flag) { return; }
resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, 12);
		}

		public void DrawMoney3(Graphics g, int seb, long in_value, int x, int y, int padding, int comma_padding, int anchor)
		{
			string str = CommaSeparate_en(in_value);
			bool flag = GetInstance().sysSave_.ints[14] == 1;
			int num = 0;
			num += JString.ValueOf(in_value).Length() * (8 + padding) - padding + 7;
			if (!flag)
			{
				num += 8;
			}
			if ((anchor == 2)) { x -= num / 2; } else if ((anchor == 4)) { x -= num; }
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < str.Length(); i++)
			{
				char c = str.CharAt(i);
				if ((c == ',')) { flag2 = true;
continue; } else if ((c == '.')) { flag3 = true;
continue; }
				if (flag2)
				{
					x += comma_padding;
				}
				if (flag3)
				{
					x++;
				}
				resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, c - 48);
				if (flag2)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y, resCom2_.img, 10);
					flag2 = false;
				}
				else if (flag3)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y + 1, resCom2_.img, 19);
					flag3 = false;
				}
				x += 8 + padding;
			}
			resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, 12);
		}

		public void DrawMoney4(Graphics g, int seb, long in_value, int x, int y, int padding, int comma_padding, int anchor)
		{
			string str = CommaSeparate_en(in_value);
			int num = 0;
			num += JString.ValueOf(in_value).Length() * (8 + padding) - padding;
			if ((anchor == 2)) { x -= num / 2; } else if ((anchor == 4)) { x -= num; }
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < str.Length(); i++)
			{
				char c = str.CharAt(i);
				if ((c == ',')) { flag = true;
continue; } else if ((c == '.')) { flag2 = true;
continue; }
				if (flag)
				{
					x += comma_padding;
				}
				if (flag2)
				{
					x += comma_padding;
				}
				resCom2_.seb[seb].DrawFrame(g, x, y, resCom2_.img, c - 48);
				if (flag)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y, resCom2_.img, 10);
					flag = false;
				}
				else if (flag2)
				{
					resCom2_.seb[seb].DrawFrame(g, x - 2, y, resCom2_.img, 10);
					flag2 = false;
				}
				x += 8 + padding;
			}
		}

		public bool Push(FormBase form)
		{
			FormManager instance = FormManager.GetInstance();
			return instance.Push(instance.GetCurrentForm(), form);
		}

		public void AddEffectShoot(int sx, int sy)
		{
			effect_.AddElement(new int[4] { 0, 0, sx, sy });
		}

		public void AddEffectCharge(int sx, int sy)
		{
			effect_.AddElement(new int[4] { 1, 0, sx, sy });
		}

		public void AddEffectTUp(int TAreaCoatNo, int pId, int t_index)
		{
			float num = GameUtil.SetVStop((EFF_TUP_DY[1] - EFF_TUP_DY[0]) * 1000, EFF_TUP_TIME[0]);
			float num2 = GameUtil.SetAStop((EFF_TUP_DY[1] - EFF_TUP_DY[0]) * 1000, EFF_TUP_TIME[0]);
			effect_.AddElement(new int[6]
			{
				2,
				0,
				pId,
				t_index,
				(int)num,
				(int)num2
			});
		}

		public void AddEffectLvUp(int wait, int pId)
		{
			effect_.AddElement(new int[3]
			{
				3,
				-wait,
				pId
			});
		}

		public void AddEffectExpUp(int pId, int value)
		{
			float num = GameUtil.SetVStop((EFF_EXPUP_DY[1] - EFF_EXPUP_DY[0]) * 1000, EFF_EXPUP_TIME[0]);
			float num2 = GameUtil.SetAStop((EFF_EXPUP_DY[1] - EFF_EXPUP_DY[0]) * 1000, EFF_EXPUP_TIME[0]);
			effect_.AddElement(new int[6]
			{
				4,
				0,
				pId,
				value,
				(int)num,
				(int)num2
			});
		}

		public void AddEffectRpUp(int sx, int sy, int rId, int value)
		{
			float num = GameUtil.SetVStop((EFF_RPUP_DY[1] - EFF_RPUP_DY[0]) * 1000, EFF_RPUP_TIME[0]);
			float num2 = GameUtil.SetAStop((EFF_RPUP_DY[1] - EFF_RPUP_DY[0]) * 1000, EFF_RPUP_TIME[0]);
			effect_.AddElement(new int[8]
			{
				5,
				0,
				sx,
				sy,
				rId,
				value,
				(int)num,
				(int)num2
			});
		}

		public void AddEffectMoneyUp(int sx, int sy, int value)
		{
			float num = GameUtil.SetVStop((EFF_MONEYUP_DY[1] - EFF_MONEYUP_DY[0]) * 1000, EFF_MONEYUP_TIME[0]);
			float num2 = GameUtil.SetAStop((EFF_MONEYUP_DY[1] - EFF_MONEYUP_DY[0]) * 1000, EFF_MONEYUP_TIME[0]);
			effect_.AddElement(new int[7]
			{
				6,
				0,
				sx,
				sy,
				value,
				(int)num,
				(int)num2
			});
		}

		public void DrawEffect(Graphics g)
		{
			for (int i = 0; i < effect_.Size(); i++)
			{
				int[] array = (int[])effect_.ElementAt(i);
				if (array[0] == 0)
				{
					Camera.ConvertScreenToCamera(array[2], array[3], ef_cpos);
					DrawSeb(g, ef_cpos[0], ef_cpos[1], resCom2_.seb[6], resCom2_.img[9], 0);
				}
				else if (array[0] == 1)
				{
					Camera.ConvertScreenToCamera(array[2], array[3], ef_cpos);
					DrawSeb(g, ef_cpos[0], ef_cpos[1], resCom2_.seb[81], resCom2_.img[90], 0);
				}
				else if (array[0] == 2)
				{
					TPlayer tPlayer = udat_.tPlayers_[array[2]];
					Camera.ConvertScreenToCamera(tPlayer.screen_pos_, ef_cpos);
					if (array[1] >= EFF_TUP_TIME[0])
					{
						ef_cpos[1] += EFF_TUP_DY[1];
					}
					else
					{
						ef_cpos[1] += EFF_TUP_DY[0] + (array[4] * array[1] + array[5] * array[1] * (array[1] + 1) / 2) / 1000;
					}
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 168));
					g.FillRect(ef_cpos[0] - 26, ef_cpos[1], 52f, 17f);
					g.SetColor(Graphics.GetColorOfRGB(255, 199, 147));
					g.DrawRect(ef_cpos[0] - 26, ef_cpos[1], 51f, 16f);
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					g.DrawString(PlayerData.PARAM_NAME[array[3]], ef_cpos[0] - 26 + 2 + 10 + 9, ef_cpos[1] + 2, 2);
					resCom2_.DrawSeb(g, ef_cpos[0] + 14, ef_cpos[1] + 3, 52, 0);
				}
				else if (array[0] == 3)
				{
					if (array[1] >= 0)
					{
						TPlayer tPlayer2 = udat_.tPlayers_[array[2]];
						Camera.ConvertScreenToCamera(tPlayer2.screen_pos_, ef_cpos);
						resCom2_.DrawSeb(g, ef_cpos[0], ef_cpos[1] - 16, 94, array[1] % 12 / 6);
					}
				}
				else if (array[0] == 4)
				{
					TPlayer tPlayer3 = udat_.tPlayers_[array[2]];
					Camera.ConvertScreenToCamera(tPlayer3.screen_pos_, ef_cpos);
					if (array[1] >= EFF_EXPUP_TIME[0])
					{
						ef_cpos[1] += EFF_EXPUP_DY[1];
					}
					else
					{
						ef_cpos[1] += EFF_EXPUP_DY[0] + (array[4] * array[1] + array[5] * array[1] * (array[1] + 1) / 2) / 1000;
					}
					DrawExp(g, ef_cpos[0] - 18, ef_cpos[1] - 6);
					DrawNumImage(g, 35, array[3], ef_cpos[0] + 12, ef_cpos[1] - 7, 0, 2);
				}
				else if (array[0] == 5)
				{
					Camera.ConvertScreenToCamera(array[2], array[3], ef_cpos);
					if (array[1] >= EFF_RPUP_TIME[0])
					{
						ef_cpos[1] += EFF_RPUP_DY[1];
					}
					else
					{
						ef_cpos[1] += EFF_RPUP_DY[0] + (array[6] * array[1] + array[7] * array[1] * (array[1] + 1) / 2) / 1000;
					}
					DrawSeb(g, ef_cpos[0] - 19 + 35, ef_cpos[1] - 46 + 33, resCom2_.seb[83], resCom2_.img[94], array[4]);
					DrawNumImage(g, 35, array[5], ef_cpos[0] + 7 + 35, ef_cpos[1] - 44 + 33, 0, 2);
				}
				else
				{
					if (array[0] != 6) {
						continue;
					}
if (array[1] < 0) {
						continue;
					}
					Camera.ConvertScreenToCamera(array[2], array[3], ef_cpos);
					if (array[1] >= EFF_MONEYUP_TIME[0])
					{
						ef_cpos[1] += EFF_MONEYUP_DY[1];
					}
					else
					{
						ef_cpos[1] += EFF_MONEYUP_DY[0] + (array[5] * array[1] + array[6] * array[1] * (array[1] + 1) / 2) / 1000;
					}
					if (Language.English())
					{
						int x = ef_cpos[0] + 33;
						int y = ef_cpos[1] - 15 + 5;
						DrawMoney(g, 35, array[4] * 100, x, y, 0, 0, 2, false);
						continue;
					}
					int num = 0;
					int num2 = 18;
					num2 += JString.ValueOf(array[4]).Length() * 8;
					int num3 = ef_cpos[0] - num2 / 2 + 33;
					int num4 = ef_cpos[1] - 15 + 5;
					string str = CommaSeparate(array[4]);
					str = str.SubstringJ(0, str.Length() - num);
					resCom2_.seb[35].DrawFrame(g, num3, num4, resCom2_.img, 11);
					num3 += 9;
					bool flag = false;
					for (int j = 0; j < str.Length(); j++)
					{
						char c = str.CharAt(j);
						if (c == ',')
						{
							flag = true;
							continue;
						}
						resCom2_.seb[35].DrawFrame(g, num3, num4, resCom2_.img, c - 48);
						if (flag)
						{
							resCom2_.seb[35].DrawFrame(g, num3 - 2, num4, resCom2_.img, 10);
							flag = false;
						}
						num3 += 8;
					}
					resCom2_.seb[35].DrawFrame(g, num3, num4, resCom2_.img, 17);
				}
			}
		}

		public void UpdateEffect()
		{
			for (int i = 0; i < effect_.Size(); i++)
			{
				int[] array = (int[])effect_.ElementAt(i);
				if (array[0] == 0)
				{
					array[1]++;
					if (array[1] >= EFF_SHOOT_TIME)
					{
						effect_.RemoveElementAt(i);
					}
				}
				else if (array[0] == 1)
				{
					array[1]++;
					if (array[1] >= EFF_CHARGE_TIME)
					{
						effect_.RemoveElementAt(i);
					}
				}
				else if (array[0] == 2)
				{
					array[1]++;
					if (array[1] >= EFF_TUP_TIME[1])
					{
						effect_.RemoveElementAt(i);
					}
				}
				else if (array[0] == 3)
				{
					array[1]++;
					if (array[1] >= EFF_TLVUP_TIME)
					{
						effect_.RemoveElementAt(i);
					}
				}
				else if (array[0] == 4)
				{
					array[1]++;
					if (array[1] >= EFF_EXPUP_TIME[1])
					{
						effect_.RemoveElementAt(i);
					}
				}
				else if (array[0] == 5)
				{
					array[1]++;
					if (array[1] >= EFF_RPUP_TIME[1])
					{
						effect_.RemoveElementAt(i);
					}
				}
				else if (array[0] == 6)
				{
					array[1]++;
					if (array[1] >= EFF_MONEYUP_TIME[1])
					{
						effect_.RemoveElementAt(i);
					}
				}
			}
		}

		public void RemoveeEffect()
		{
			for (int num = effect_.Size() - 1; num >= 0; num--)
			{
				effect_.RemoveElementAt(num);
			}
		}

		public int GetSeason(int month)
		{
			if (3 <= month + 1 && month + 1 <= 5)
			{
				return 0;
			}
			if (6 <= month + 1 && month + 1 <= 8)
			{
				return 1;
			}
			if (9 <= month + 1 && month + 1 <= 11)
			{
				return 2;
			}
			return 3;
		}

		public int[] IntegerVectorToIntAry(Vector ary)
		{
			int[] array = new int[ary.Size()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((JInteger)ary.ElementAt(i)).IntValue();
			}
			return array;
		}

		public void PlayBgm(int bgm)
		{
			if (GetInstance().platform_.StartsWith("ap")) {
				return;
			}
if (resSound_.sound[bgm].GetState() == 2) {
				return;
			}
			for (int i = 0; i < BGM.Length; i++)
			{
				if (resSound_.sound[BGM[i]].GetState() == 2)
				{
					resSound_.sound[BGM[i]].Stop();
				}
			}
			SoundPlayer instance = SoundPlayer.GetInstance();
			instance.Play(resSound_.sound[bgm]);
		}

		public void PlaySe(int se)
		{
			if (GetInstance().platform_.StartsWith("ap")) { return; }
{
SoundPlayer instance = SoundPlayer.GetInstance();
instance.Play(resSound_.sound[se]);
}
		}

		public void PlayJingle(int jingle)
		{
			Main.SetJingle(resSound_.sound[jingle]);
			PlaySe(jingle);
		}

		public string GetHttpErrorText()
		{
			string result = "通信を許可し電波状況\nを確認して下さい";
			if (Http.GetErrorText() != null)
			{
				result = Http.GetErrorText();
			}
			return result;
		}

		public bool ShowNews(int id, string arg)
		{
			FormManager instance = FormManager.GetInstance();
			instance.Push(SubForm.MakeNews(id, arg));
			return instance.Push(SubForm.MakeDialog("開幕ｷｯｸｵﾌ通信\nが発行されました"));
		}

		public long GetHighScore()
		{
			return sysSave_.lngs[0];
		}

		public void SetHighScore(long score)
		{
			sysSave_.lngs[0] = score;
		}

		public long GetNetHighScore()
		{
			return sysSave_.lngs[1];
		}

		public void SetNetHighScore(long score)
		{
			sysSave_.lngs[1] = score;
		}

		public long GetMoneyHighScore()
		{
			return sysSave_.lngs[2];
		}

		public void SetMoneyHighScore(long score)
		{
			sysSave_.lngs[2] = score;
		}

		public long GettMoneyNetHighScore()
		{
			return sysSave_.lngs[3];
		}

		public void SettMoneyNetHighScore(long score)
		{
			sysSave_.lngs[3] = score;
		}

		public int DateToTime(int year, int month, int day, int time)
		{
			return DateToDays(year, month, day) * 24 * 60 * 60 + time;
		}

		public int DateToDays(int year, int month, int week)
		{
			return year * 336 + month * 28 + week * 7;
		}

		public void DrawIcon(Graphics g, int x, int y, int icon)
		{
		}

		public long AddMoney(int add, int recordId)
		{
			save_.lngs[0] = Clamp(save_.lngs[0] + add, long.MinValue, long.MaxValue);
			udat_.RecordAddMoney(recordId, add);
			if (save_.ints[14] == 0 && save_.lngs[0] > GetMoneyHighScore())
			{
				sysSave_.strs[1] = save_.strs[0];
				SetMoneyHighScore(save_.lngs[0]);
			}
			return save_.lngs[0];
		}

		public long SubMoney(int sub, int recordId)
		{
			save_.lngs[0] = Clamp(save_.lngs[0] - sub, long.MinValue, long.MaxValue);
			udat_.RecordSubMoney(recordId, sub);
			return save_.lngs[0];
		}

		public bool ChPayMoney(int cost)
		{
			if (save_.lngs[0] >= cost)
			{
				return true;
			}
			return false;
		}

		public int[] RotatePoint(int x, int y, int dir)
		{
			if ((dir == 0)) { rotatePoint_tmp[0] = x;
rotatePoint_tmp[1] = y; } else if ((dir == 1)) { rotatePoint_tmp[0] = -y;
rotatePoint_tmp[1] = x; } else if ((dir == 2)) { rotatePoint_tmp[0] = -x;
rotatePoint_tmp[1] = -y; } else { rotatePoint_tmp[0] = y;
rotatePoint_tmp[1] = -x; }
			return rotatePoint_tmp;
		}

		public void UpdatePassiveScript()
		{
		}

		public bool RemoveAllMenuForms()
		{
			FormManager instance = FormManager.GetInstance();
			int formsNum = instance.GetFormsNum();
			for (int num = formsNum - 1; num >= 0; num--)
			{
				FormBase form = instance.GetForm(num);
				if (form.GetId() != 4) { continue; }
SubForm subForm = (SubForm)form;
if (subForm.IsMenu())
					{
						instance.Pop(subForm);
					}
			}
			return true;
		}

		public int[] GetNearNeighborNode(int[][][] nodeMap, int x, int y)
		{
			return GetNearNeighborNode(nodeMap, x, y, null);
		}

		public int[] GetNearNeighborNode(int[][][] nodeMap, int x, int y, int[] priority)
		{
			int num = int.MaxValue;
			int[] result = null;
			for (int i = 0; i < DIRECT1.Length; i++)
			{
				int num2 = x + DIRECT1[i][0];
				int num3 = y + DIRECT1[i][1];
				if ((((num3 < 0) || (nodeMap.Length <= num3)) || (num2 < 0)) || (nodeMap[0].Length <= num2)) { continue; }
int[] array = nodeMap[num3][num2];
if (priority != null && array == priority)
					{
						return priority;
					}
int num4 = array[0];
if (num4 < num)
					{
						num = num4;
						result = array;
					}
			}
			return result;
		}

		public int[][] GetNeighborNodeList(int[][][] nodeMap, int x, int y)
		{
			Vector vector = new Vector();
			for (int i = 0; i < DIRECT1.Length; i++)
			{
				int num = x + DIRECT1[i][0];
				int num2 = y + DIRECT1[i][1];
				if ((((num2 < 0) || (nodeMap.Length <= num2)) || (num < 0)) || (nodeMap[0].Length <= num)) { continue; }
int[] array = nodeMap[num2][num];
if (array[0] < int.MaxValue)
					{
						vector.AddElement(array);
					}
			}
			int[][] array2 = new int[vector.Size()][];
			for (int j = 0; j < vector.Size(); j++)
			{
				array2[j] = (int[])vector.ElementAt(j);
			}
			for (int k = 0; k < array2.Length - 1; k++)
			{
				for (int num3 = array2.Length - 1; num3 > k; num3--)
				{
					if (array2[num3][0] < array2[num3 - 1][0])
					{
						int[] array3 = array2[num3];
						array2[num3] = array2[num3 - 1];
						array2[num3 - 1] = array3;
					}
				}
			}
			return array2;
		}

		public static int[] GetIntArray(string src)
		{
			string[] array = StringUtil.Split(src, ",");
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = JInteger.ParseInt(array[i]);
			}
			return array2;
		}

		public static int[][] GetInt2Array(string src)
		{
			string[] array = StringUtil.Split(src, "&");
			int[][] array2 = new int[array.Length][];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = GetIntArray(array[i]);
			}
			return array2;
		}

		public byte[] BooleanToByteArray(bool[] bools)
		{
			ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
			StreamUtil.WriteInt(byteArrayOutputStream, bools.Length);
			for (int i = 0; i < bools.Length; i++)
			{
				StreamUtil.WriteByte(byteArrayOutputStream, (sbyte)(bools[i] ? 1 : 0));
			}
			byte[] result = byteArrayOutputStream.ToByteArray();
			byteArrayOutputStream.Close();
			return result;
		}

		public bool[] SbyteArrayToBoolean(byte[] src)
		{
			return SbyteArrayToBoolean(src, -1);
		}

		public bool[] SbyteArrayToBoolean(byte[] src, int size)
		{
			ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(src);
			int num = StreamUtil.ReadInt(byteArrayInputStream);
			if (size == -1)
			{
				size = num;
			}
			bool[] array = new bool[size];
			for (int i = 0; i < num && i < array.Length; i++)
			{
				array[i] = StreamUtil.ReadByte(byteArrayInputStream) == 1;
			}
			byteArrayInputStream.Close();
			return array;
		}

		public byte[] IntToByteArray(int[] ints)
		{
			ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
			StreamUtil.WriteInt(byteArrayOutputStream, ints.Length);
			for (int i = 0; i < ints.Length; i++)
			{
				StreamUtil.WriteInt(byteArrayOutputStream, ints[i]);
			}
			byte[] result = byteArrayOutputStream.ToByteArray();
			byteArrayOutputStream.Close();
			return result;
		}

		public int[] SbyteArrayToInt(byte[] src)
		{
			return SbyteArrayToInt(src, -1);
		}

		public int[] SbyteArrayToInt(byte[] src, int size)
		{
			ByteArrayInputStream byteArrayInputStream = new ByteArrayInputStream(src);
			int num = StreamUtil.ReadInt(byteArrayInputStream);
			if (size == -1)
			{
				size = num;
			}
			int[] array = new int[size];
			for (int i = 0; i < num && i < array.Length; i++)
			{
				array[i] = StreamUtil.ReadInt(byteArrayInputStream);
			}
			byteArrayInputStream.Close();
			return array;
		}

		public int FrameToMilliSecond(int frame)
		{
			return 1000 * frame / 20;
		}

		public void DrawOutline(Graphics g, int x, int y, Seb seb, Image[] images, int frame, int lineWidth, int color)
		{
			int[] boundingRect = seb.GetBoundingRect();
			int num = boundingRect[2] - boundingRect[0] + lineWidth * 2;
			int num2 = boundingRect[3] - boundingRect[1] + lineWidth * 2;
			if (imgOutline1_ != null && (imgOutline1_.GetWidth() < num || imgOutline1_.GetHeight() < num2))
			{
				graOutline1_.Dispose(false);
				graOutline1_ = null;
				imgOutline1_.Dispose();
				imgOutline1_ = null;
				graOutline2_.Dispose(false);
				graOutline2_ = null;
				imgOutline2_.Dispose();
				imgOutline2_ = null;
			}
			if (imgOutline1_ == null)
			{
				imgOutline1_ = Image.CreateImage(num, num2);
				graOutline1_ = imgOutline1_.GetGraphics();
				imgOutline2_ = Image.CreateImage(num, num2);
				graOutline2_ = imgOutline2_.GetGraphics();
			}
			graOutline1_.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
			graOutline1_.FillRect(0f, 0f, imgOutline1_.GetWidth(), imgOutline1_.GetHeight());
			graOutline1_.SetRenderMode(0, 0, 0);
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if ((i == 1) && (j == 1)) { continue; }
seb.DrawFrame(graOutline1_, j * lineWidth - boundingRect[0], i * lineWidth - boundingRect[1], images, frame);
				}
			}
			graOutline1_.SetRenderMode(0, 255, 0);
			graOutline2_.SetColor(color);
			graOutline2_.FillRect(0f, 0f, imgOutline2_.GetWidth(), imgOutline2_.GetHeight());
			graOutline2_.SetRenderMode(2, 255, 255);
			graOutline2_.DrawImage(imgOutline1_, 0f, 0f, 0, 0, num, num2);
			graOutline2_.SetRenderMode(0, 0, 0);
			seb.DrawFrame(graOutline2_, lineWidth - boundingRect[0], lineWidth - boundingRect[1], images, frame);
			graOutline2_.SetRenderMode(0, 255, 0);
			imgOutline2_.SetTransparentColor(Graphics.GetColorOfRGB(0, 0, 0));
			imgOutline2_.SetTransparentEnabled(true);
			g.DrawImage(imgOutline2_, x + boundingRect[0] - lineWidth, y + boundingRect[1] - lineWidth, 0, 0, num, num2);
		}

		public void DrawOutline(Graphics g, int x, int y, Image img, int u, int v, int w, int h, int lineWidth, int color)
		{
			Image image = Image.CreateImage(w + lineWidth * 2, h + lineWidth * 2);
			global::kairo.unity.ui.Image image2;
Graphics graphics = image.GetGraphics();
			image2 = Image.CreateImage(image.GetWidth(), image.GetHeight());
			Graphics graphics2 = image2.GetGraphics();
			graphics.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
			graphics.FillRect(0f, 0f, image.GetWidth(), image.GetHeight());
			graphics.SetRenderMode(0, 0, 0);
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if ((i == 1) && (j == 1)) { continue; }
graphics.DrawImage(img, j * lineWidth, i * lineWidth, u, v, w, h);
				}
			}
			graphics.SetRenderMode(0, 255, 0);
			graphics2.SetColor(color);
			graphics2.FillRect(0f, 0f, image2.GetWidth(), image2.GetHeight());
			graphics2.SetRenderMode(2, 255, 255);
			graphics2.DrawImage(image, 0f, 0f);
			graphics2.SetRenderMode(0, 0, 0);
			graphics2.DrawImage(img, lineWidth, lineWidth, u, v, w, h);
			graphics2.SetRenderMode(0, 255, 0);
			image2.SetTransparentColor(Graphics.GetColorOfRGB(0, 0, 0));
			image2.SetTransparentEnabled(true);
			g.DrawImage(image2, x - lineWidth, y - lineWidth);
			graphics.Dispose(false);
			image.Dispose();
			graphics2.Dispose(false);
			image2.Dispose();
		}

		public static bool CheckTimeZone(int time, int start, int end)
		{
			if (start > end)
			{
				return start <= time || time < end;
			}
			return start <= time && time < end;
		}

		public void ResourceReDownload()
		{
		}

		public void DrawMeter(Graphics g, int x, int y, int value, int max, int scaleNum, bool reverse, int anchor)
		{
			DrawMeter(g, x, y, value, max, scaleNum, reverse, anchor, 12);
		}

		public void DrawMeter(Graphics g, int x, int y, int value, int max, int scaleNum, bool reverse, int anchor, int height)
		{
			y -= 2;
			if ((anchor & 4) != 0)
			{
				x -= scaleNum * 2 + 3;
			}
			else if ((anchor & 2) != 0)
			{
				x -= (scaleNum * 2 + 3) / 2;
			}
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
			g.FillRect(x, y + 2, scaleNum * 2 + 3, height - 4);
			if (height > 8)
			{
				g.SetColor(Graphics.GetColorOfRGB(64, 64, 64));
				g.FillRect(x + 1, y + 3, scaleNum * 2 + 1, height - 6);
			}
			else
			{
				y--;
				height += 2;
			}
			int num = 0;
			int num2 = scaleNum;
			int num3 = 1;
			if (reverse)
			{
				num = scaleNum - 1;
				num2 = -1;
				num3 = -1;
			}
			int num4 = max / scaleNum;
			int num5 = value;
			for (int i = num; i != num2; i += num3)
			{
				int num6 = height - 8;
				g.SetColor(Graphics.GetColorOfRGB(32, 32, 32));
				g.FillRect(x + 2 + i * 2, y + 4, 1f, num6);
				int num7 = ClampMax(num5 * num6 / num4, num6);
				if (num7 > 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(64, 255, 128));
					g.FillRect(x + 2 + i * 2, y + 4 + num6 - num7, 1f, num7);
				}
				num5 -= num4;
			}
		}

		public int GetAllStock(int itemId)
		{
			{ int recoveredReturn = 0; return recoveredReturn; }
		}

		public int GetStockTotal(int[] stock)
		{
			if (stock == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < stock.Length; i++)
			{
				num += stock[i];
			}
			return num;
		}

		public static int SearchInteger(int[] array, int key)
		{
			if (array == null)
			{
				return -1;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == key)
				{
					return i;
				}
			}
			return -1;
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, int anchor, bool active, int margin, bool cursor, FormBase form, int frame)
		{
			Font font = g.GetFont(null);
			int num = font.StringWidth(str);
			int height = font.GetHeight();
			int[] anchorPosition = Graphics.GetAnchorPosition(x, y, num, height, anchor);
			int color = g.GetColor();
			if (active)
			{
				g.SetColor(Graphics.GetColorOfRGB(205, 235, 111));
				g.FillRect(anchorPosition[0] - margin, anchorPosition[1] - 1, num + margin * 2, height + 2);
			}
			g.SetColor(color);
			g.DrawString(str, anchorPosition[0], anchorPosition[1]);
			if (cursor)
			{
			}
			drawCommand_tmp[0] = anchorPosition[0] - margin - 4;
			drawCommand_tmp[1] = anchorPosition[1] - 1 - 1;
			drawCommand_tmp[2] = num + margin * 2 + 8;
			drawCommand_tmp[3] = height + 2 + 2;
			return drawCommand_tmp;
		}

		public void DrawCommand(Graphics g, string str, int x, int y, int w, int anchor, bool active, bool cursor, FormBase form, int frame)
		{
			Font font = g.GetFont(null);
			int height = font.GetHeight();
			int width = font.StringWidth(str);
			int[] anchorPosition = Graphics.GetAnchorPosition(x, y, width, height, anchor);
			int color = g.GetColor();
			if (active)
			{
				g.SetColor(Graphics.GetColorOfRGB(205, 235, 111));
				g.FillRect(x - w / 2, anchorPosition[1] - 1, w, height + 2);
			}
			g.SetColor(color);
			g.DrawString(str, anchorPosition[0], anchorPosition[1]);
			if (cursor) { return; }

		}

		public int GetMainBGM()
		{
			return 0;
		}

		public static void Gc()
		{
			JSystem.Gc();
			JThread.Yield();
			try
			{
				JThread.Sleep(100L);
			}
			catch (Exception)
			{
			}
		}

		public static void Sort(int[] list)
		{
			for (int i = 0; i < list.Length - 1; i++)
			{
				for (int num = list.Length - 1; num > i; num--)
				{
					if (list[num - 1] > list[num])
					{
						int num2 = list[num];
						list[num] = list[num - 1];
						list[num - 1] = num2;
					}
				}
			}
		}

		public int GetTime(int week, int time)
		{
			int num = 0;
			num += week % 4 * 21600;
			num += time / 4;
			return (num + 43200) % 86400;
		}

		public bool CheckNightTime(int time)
		{
			int num = time / 3600;
			return num < 6 || 20 <= num;
		}

		public bool CheckSleepingTime(int time)
		{
			int num = time / 3600;
			return num < 5 || 22 <= num;
		}

		internal bool CheckPointInPolygon(int x, int y, int[][] aPoint)
		{
			int num = 0;
			int[] array = aPoint[0];
			bool flag = x <= array[0];
			bool flag2 = y <= array[1];
			for (int i = 1; i < aPoint.Length + 1; i++)
			{
				int[] array2 = aPoint[i % aPoint.Length];
				bool flag3 = x <= array2[0];
				bool flag4 = y <= array2[1];
				if (flag2 != flag4)
				{
					if (flag == flag3)
					{
						if (flag)
						{
							num += ((flag2) ? ((-1)) : (1));
						}
					}
					else if (x <= array[0] + (array2[0] - array[0]) * (y - array[1]) / (array2[1] - array[1]))
					{
						num += ((flag2) ? ((-1)) : (1));
					}
				}
				array = array2;
				flag = flag3;
				flag2 = flag4;
			}
			return 0 != num;
		}

		public void DrawFukidashiBalloon(Graphics g, string text, int dx, int dy, int color)
		{
			dy += 12;
			int color2 = g.GetColor();
			Balloon balloon = new Balloon(text, color);
			balloon.Draw(g, dx, dy);
			g.SetColor(color2);
		}

		public void DrawFukidashiKimochi(Graphics g, int dx, int dy, int numValue, int numSeb)
		{
			dy += 12;
			int color = g.GetColor();
			Balloon balloon = new Balloon(MyFormBase.LT("気持ち"), SubForm.SC_WINDOW_BLUE);
			int keta = 3;
			balloon.SetSpace(0, 12 + 8 * keta, 0, 0, delegate(Graphics gra, float fy, RectangleF left, RectangleF right)
			{
				int num = (int)right.left + 8 * keta;
				int y = -16;
				DrawNumImage(g, numSeb, numValue, num, y, 0, 4);
				DrawSeb(g, num + 2, y, resCom2_.seb[90], resCom2_.img[131], 1);
			});
			balloon.Draw(g, dx, dy);
			g.SetColor(color);
		}

		public void DrawFukidashiKimochiOLD(Graphics g, int dx, int dy, int numValue, int numSeb)
		{
			int color = g.GetColor();
			if (MyFormBase.Jp())
			{
				DrawFukidashiOLD(g, string.Empty, dx, dy, -1);
				DrawSeb(g, dx - 28, dy - 5, resCom2_.seb[90], resCom2_.img[131], 0);
				DrawSeb(g, dx + 19, dy - 5, resCom2_.seb[90], resCom2_.img[131], 1);
				DrawNumImage(g, 78, numValue, dx + 17, dy - 5, 0, 4);
			}
			else if (MyFormBase.En())
			{
				int num = 8;
				DrawFukidashiOLD(g, string.Empty, dx, dy, 70);
				DrawSeb(g, dx - 28 - num, dy - 5, resCom2_.seb[90], resCom2_.img[131], 0);
				DrawSeb(g, dx + 19 + num, dy - 5, resCom2_.seb[90], resCom2_.img[131], 1);
				DrawNumImage(g, 78, numValue, dx + 17 + num, dy - 5, 0, 4);
			}
			g.SetColor(color);
		}

		public void DrawFukidashiOLD(Graphics g, string str, int dx, int dy)
		{
			SubForm.SetFontSize(g, 10);
			int len = g.GetFont(null).StringWidth(str);
			DrawFukidashiOLD(g, str, dx, dy, len, SubForm.SC_WINDOW_BLUE);
			SubForm.ResetFontSize(g);
		}

		public void DrawFukidashiOLD(Graphics g, string str, int dx, int dy, int len)
		{
			DrawFukidashiOLD(g, str, dx, dy, len, SubForm.SC_WINDOW_BLUE);
		}

		public void DrawFukidashiOLD(Graphics g, string str, int dx, int dy, int len, int color)
		{
			int color2 = g.GetColor();
			g.SetColor(color);
			if (len != -1)
			{
				len += 12;
				if (len < 72)
				{
					g.DrawImage(resCom2_.img[130], dx - len / 2 + 6, dy - 9, 6, 0, len - 12, 18);
				}
				else
				{
					g.DrawImage(resCom2_.img[130], dx - len / 2 + 6, dy - 9, 6, 0, 60, 18);
					g.DrawImage(resCom2_.img[130], dx - len / 2 + 6 + 60, dy - 9, 6, 0, len - 12 - 60, 18);
				}
				g.DrawImage(resCom2_.img[130], dx - len / 2, dy - 9, 0, 0, 6, 18);
				g.DrawImage(resCom2_.img[130], dx + len / 2 - 6, dy - 9, 66, 0, 6, 18);
				g.DrawImage(resCom2_.img[130], dx - 1, dy + 7, 72, 0, 5, 5);
				g.DrawString(str, dx, dy, 34);
			}
			else
			{
				g.DrawImage(resCom2_.img[130], dx - 36, dy - 9, 0, 0, 72, 18);
				g.DrawString2(str, dx, dy, 34);
				g.DrawImage(resCom2_.img[130], dx - 1, dy + 7, 72, 0, 5, 5);
			}
			g.SetColor(color2);
		}

		public int[] GetEvenlyDivisionArea(int left, int right, int padding, int index, int max)
		{
			int num = (right - left - padding * (max - 1)) / max;
			getEvenlyDivisionArea_tmp_[0] = left + index * (num + padding);
			getEvenlyDivisionArea_tmp_[1] = left + index * (num + padding) + num;
			return getEvenlyDivisionArea_tmp_;
		}

		public void BubbleSort(int[] list)
		{
			for (int i = 0; i < list.Length - 1; i++)
			{
				for (int num = list.Length - 1; num > i; num--)
				{
					if (list[num] > list[num - 1])
					{
						int num2 = list[num];
						list[num] = list[num - 1];
						list[num - 1] = num2;
					}
				}
			}
		}

		public int GetIsometricX(int xi, int yi)
		{
			return xi * 30 - yi * 30;
		}

		public int GetIsometricY(int xi, int yi)
		{
			return yi * 15 + xi * 15;
		}

		public BaseData[] AdjustDataList(BaseData[] list, int id)
		{
			int num = 0;
			for (int i = 0; i < list.Length; i++)
			{
				if (num < list[i].id_)
				{
					num = list[i].id_;
				}
			}
			num++;
			BaseData[] array = null;
			if (id == 1)
			{
				array = new EventData[num];
			}
			for (int j = 0; j < list.Length; j++)
			{
				array[list[j].id_] = list[j];
			}
			return array;
		}

		public void SerializeScripts(OutputStream os, Vector scripts)
		{
			StreamUtil.WriteInt(os, scripts.Size());
			for (int i = 0; i < scripts.Size(); i++)
			{
				int[] array = (int[])scripts.ElementAt(i);
				StreamUtil.WriteInt(os, array.Length);
				for (int j = 0; j < array.Length; j++)
				{
					StreamUtil.WriteInt(os, array[j]);
				}
			}
		}

		public Vector DeserializeScripts(InputStream stream)
		{
			Vector vector = new Vector();
			int num = StreamUtil.ReadInt(stream);
			for (int i = 0; i < num; i++)
			{
				int[] array = new int[StreamUtil.ReadInt(stream)];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = StreamUtil.ReadInt(stream);
				}
				vector.AddElement(array);
			}
			return vector;
		}

		public SubForm ExecEvent(int id, string args, int[] argi)
		{
			if (id < 0 || eventData_.Length <= id)
			{
				return null;
			}
			if (args == null)
			{
				args = save_.strs[0];
			}
			EventData eventData = eventData_[id];
			eventNum_[id] = ClampMax(eventNum_[id] + 1, int.MaxValue);
			if (Config.PRINT && (eventData.execsScript_ == null || eventData.execsScript_.Length == 0))
			{
				Log.Info("ID[" + id + "] のイベントは存在しません");
			}
			return ExecuteScript(eventData.execsScript_, true, args, argi, 0);
		}

		public bool CheckAlreadyEvent(int id)
		{
			if (id < 0 || eventData_.Length <= id)
			{
				return false;
			}
			return eventNum_[id] > 0;
		}

		public SubForm ExecuteScript(int[][] script, bool popup, string args, int[] argi, int line)
		{
			return ExecuteScript(script, popup, args, argi, line, 0, -1);
		}

		public SubForm ExecuteScript(int[][] script, bool popup, string args, int[] argi, int line, int talkCharaType, int talkCharaIndex)
		{
			if (script == null)
			{
				return null;
			}
			SubForm subForm = null;
			DelayEvent delayEvent = null;
			for (int i = line; i < script.Length; i++)
			{
				int[] array = script[i];
				int num = array[0];
				int num2 = num;
				if (num2 == 5)
				{
					delayEvent = new DelayEvent(GetDelayId(script), i + 1, array[1], -1, save_.ints[9], args, argi);
					if (2 < array.Length)
					{
						delayEvent.hour_ = array[2];
					}
					delayEvents_.AddElement(delayEvent);
					return subForm;
				}
				subForm = ExecuteScript(array, popup, args, argi, script);
				if ((talkCharaIndex == -1) || (subForm == null)) { continue; }
if (subForm.humanCharaIndex_ != -1) { continue; }
subForm.humanCharaType_ = talkCharaType;
subForm.humanCharaIndex_ = talkCharaIndex;
			}
			return subForm;
		}

		public SubForm ExecuteScript(int[] scr, bool popup, string args, int[] argi)
		{
			return ExecuteScript(scr, popup, args, argi, null);
		}

		public SubForm ExecuteScript(int[] scr, bool popup, string args, int[] argi, int[][] script)
		{
			if (scr == null || scr.Length == 0)
			{
				return null;
			}
			SubForm subForm = null;
			int num = scr[0];
			int num2 = 0;
			TeamData teamData = null;
			TenantData tenantData = null;
			int[] array = new int[scr.Length];
			JSystem.Arraycopy(scr, 0, array, 0, array.Length);
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i] > -2147483549)
				{
					continue;
				}
				int num3 = array[i] - int.MinValue;
				if (Config.DEBUG)
				{
					if (argi == null)
					{
						Dialog dialog = new Dialog(0, "ｴﾗｰ");
						dialog.SetText("スクリプトの実行に失敗しました。整数引数への参照が null です。");
						dialog.Show();
					}
					else if (argi.Length <= num3)
					{
						Dialog dialog2 = new Dialog(0, "ｴﾗｰ");
						dialog2.SetText("スクリプトの実行に失敗しました。整数引数への参照番号が範囲外です。" + num3 + "/" + argi.Length);
						dialog2.Show();
					}
				}
				array[i] = argi[num3];
			}
			scr = array;
			switch (num)
			{
			case 0:
				if (scr[1] < 0)
				{
					SubMoney(-scr[1], 4);
					break;
				}
				AddMoney(scr[1], 4);
				AddPopupMessage(1, MyFormBase.YEN("万", scr[1]));
				break;
			case 1:
				subForm = ExecEvent(scr[1], args, argi);
				break;
			case 2:
				if (scr.Length > 2)
				{
					string text = string.Empty;
					for (int n = 2; n < scr.Length; n++)
					{
						if (n != 2)
						{
							text += "\t";
						}
						string value = EventData.stringTable_[scr[n]];
						text += StringUtil.Replace(value, args);
					}
					args = text;
				}
				subForm = SubForm.MakeTalkForm(scr[1], args);
				Push(subForm);
				break;
			case 3:
				Push(SubForm.MakeDialog(MyFormBase.LT("開幕ｷｯｸｵﾌ通信\nが発行されました")));
				subForm = SubForm.MakeNews(scr[1], args);
				Push(subForm);
				break;
			case 4:
				if (scr.Length > 2)
				{
					string text2 = string.Empty;
					for (int num4 = 2; num4 < scr.Length; num4++)
					{
						if (num4 != 2)
						{
							text2 += "\t";
						}
						string value2 = EventData.stringTable_[scr[num4]];
						text2 += StringUtil.Replace(value2, args);
					}
					args = text2;
				}
				subForm = SubForm.MakeEvent(eventMsgs_[scr[1]], args, scr[1]);
				Push(subForm);
				break;
			case 6:
				subForm = SubForm.MakeWait(scr[1]);
				Push(subForm);
				break;
			case 7:
				save_.ints[8] |= scr[1];
				break;
			case 8:
				save_.ints[8] = BitUtil.Off(save_.ints[8], scr[1]);
				break;
			case 9:
				if (scr[1] != -1)
				{
					PlayerData playerData5 = pData_[scr[1]];
					if (udat_.memberPDataist_.IndexOf(playerData5) == -1)
					{
						udat_.memberPDataist_.AddElement(playerData5);
					}
					AddPopupMessage(4, playerData5.name_ + string.Empty);
				}
				break;
			case 10:
				if (scr[1] != -1)
				{
					SupervisorData supervisorData = svisorData_[scr[1]];
					supervisorData.ChangeState(1);
					if (!ChExistPopup(26))
					{
						AddPopupMessage(26, null);
					}
				}
				break;
			case 11:
				if (scr[1] != -1)
				{
					FormationData formationData = formationData_[scr[1]];
					formationData.ChangeState(1);
				}
				break;
			case 12:
				if (scr[1] != -1)
				{
					tenantData = tenantData_[scr[1]];
					tenantData.ChangeState(1);
					AddPopupMessage(24, tenantData.name_);
					if (!udat_.ChEventFlg(4))
					{
						udat_.OnEventFlg(4);
					}
				}
				break;
			case 13:
				if (scr[1] != -1)
				{
					FanActData fanActData = fanActData_[scr[1]];
					fanActData.ChangeState(1);
					AddPopupMessage(25, fanActData.name_);
					if (!udat_.ChEventFlg(2))
					{
						udat_.OnEventFlg(2);
					}
				}
				break;
			case 14:
				if (scr[1] != -1)
				{
					TrainingData trainingData = trainingData_[scr[1]];
					trainingData.ChangeState(1);
					AddPopupMessage(23, trainingData.name_ + string.Empty);
				}
				break;
			case 15:
				if (scr[1] != -1)
				{
					SponsorData sponsorData = sponsorData_[scr[1]];
					if (sponsorData.state_ == 0)
					{
						sponsorData.ChangeState(1);
					}
					if (!ChExistPopup(22))
					{
						AddPopupMessage(22, null);
					}
					if (!udat_.ChEventFlg(32))
					{
						udat_.OnEventFlg(32);
					}
				}
				break;
			case 16:
				if (scr[1] != -1)
				{
					QuestMatchData questMatchData = qmData_[scr[1]];
					if (questMatchData.state_ == 0)
					{
						questMatchData.ChangeState(1);
					}
					if (!ChExistPopup(6))
					{
						AddPopupMessage(6, null);
					}
					if (!udat_.ChEventFlg(8))
					{
						udat_.OnEventFlg(8);
					}
				}
				break;
			case 17:
			{
				if (scr[1] == -1)
				{
					break;
				}
				TradeData tradeData2 = tradeData_[scr[1]];
				if (tradeData2.state_ != 2)
				{
					tradeData2.ChangeState(1);
					if (!ChExistPopup(3))
					{
						AddPopupMessage(3, null);
					}
					if (!udat_.ChEventFlg(16))
					{
						udat_.OnEventFlg(16);
					}
				}
				break;
			}
			case 18:
			{
				for (int k = 0; k < tradeData_.Length; k++)
				{
					TradeData tradeData = tradeData_[k];
					if (tradeData.tradeGroup_ == scr[1])
					{
						tradeData.ChangeState(1);
						if (!ChExistPopup(2))
						{
							AddPopupMessage(2, null);
						}
					}
				}
				break;
			}
			case 19:
				if (scr[1] != -1)
				{
					LeagueData leagueData = leagueData_[scr[1]];
					leagueData.ChangeState(1);
					AddPopupMessage(5, leagueData.name_);
					if (!udat_.ChEventFlg(1))
					{
						udat_.OnEventFlg(1);
					}
				}
				break;
			case 20:
				tenantData = tenantData_[TenantData.TENANT_ID_STADIUM];
				tenantData.ChangeState(1);
				AddPopupMessage(38, tenantData.name_);
				if (!udat_.ChEventFlg(4))
				{
					udat_.OnEventFlg(4);
				}
				break;
			case 21:
				tenantData = tenantData_[TenantData.TENANT_ID_TRAININGAREA_SPRED];
				tenantData.ChangeState(1);
				AddPopupMessage(38, tenantData.name_);
				if (!udat_.ChEventFlg(4))
				{
					udat_.OnEventFlg(4);
				}
				break;
			case 22:
				tenantData = tenantData_[TenantData.TENANT_ID_TRAININGAREA];
				tenantData.ChangeState(1);
				AddPopupMessage(38, tenantData.name_);
				if (!udat_.ChEventFlg(4))
				{
					udat_.OnEventFlg(4);
				}
				break;
			case 23:
				udat_.AddResearchPoint(0, scr[1]);
				break;
			case 24:
				udat_.AddResearchPoint(1, scr[1]);
				break;
			case 25:
				udat_.AddResearchPoint(2, scr[1]);
				break;
			case 26:
				udat_.AddResearchPoint(3, scr[1]);
				break;
			case 27:
			{
				for (int num8 = 0; num8 < udat_.memberPDataist_.Size(); num8++)
				{
					PlayerData playerData8 = (PlayerData)udat_.memberPDataist_.ElementAt(num8);
					playerData8.AddExp(scr[1], true);
				}
				AddPopupMessage(7, scr[1] + string.Empty);
				break;
			}
			case 28:
			{
				for (int num7 = 0; num7 < udat_.memberPDataist_.Size(); num7++)
				{
					PlayerData playerData7 = (PlayerData)udat_.memberPDataist_.ElementAt(num7);
					playerData7.AddPhysicalParam(scr[1], 0);
				}
				AddPopupMessage(8, scr[1] + string.Empty);
				break;
			}
			case 29:
			{
				for (int num6 = 0; num6 < udat_.memberPDataist_.Size(); num6++)
				{
					PlayerData playerData6 = (PlayerData)udat_.memberPDataist_.ElementAt(num6);
					playerData6.AddPhysicalParam(scr[1], 1);
				}
				AddPopupMessage(9, scr[1] + string.Empty);
				break;
			}
			case 30:
			{
				for (int num5 = 0; num5 < udat_.memberPDataist_.Size(); num5++)
				{
					PlayerData playerData4 = (PlayerData)udat_.memberPDataist_.ElementAt(num5);
					playerData4.AddPhysicalParam(scr[1], 2);
				}
				AddPopupMessage(10, scr[1] + string.Empty);
				break;
			}
			case 31:
			{
				for (int m = 0; m < udat_.memberPDataist_.Size(); m++)
				{
					PlayerData playerData3 = (PlayerData)udat_.memberPDataist_.ElementAt(m);
					playerData3.AddPhysicalParam(scr[1], 3);
				}
				AddPopupMessage(11, scr[1] + string.Empty);
				break;
			}
			case 32:
			{
				for (int l = 0; l < udat_.memberPDataist_.Size(); l++)
				{
					PlayerData playerData2 = (PlayerData)udat_.memberPDataist_.ElementAt(l);
					playerData2.AddPhysicalParam(scr[1], 4);
				}
				AddPopupMessage(12, scr[1] + string.Empty);
				break;
			}
			case 33:
			{
				for (int j = 0; j < udat_.memberPDataist_.Size(); j++)
				{
					PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(j);
					playerData.AddAuraPoint(scr[1], false);
				}
				AddPopupMessage(13, scr[1] + string.Empty);
				break;
			}
			case 34:
				num2 = FanActData.GetDispParamToEva(scr[1], 0);
				FanActData.AddFanParam(num2, 0);
				if (num2 >= 0)
				{
					AddPopupMessage(14, scr[1] + string.Empty);
				}
				else
				{
					AddPopupMessage(15, scr[1] + string.Empty);
				}
				break;
			case 35:
				num2 = FanActData.GetDispParamToEva(scr[1], 1);
				FanActData.AddFanParam(num2, 1);
				if (num2 >= 0)
				{
					AddPopupMessage(16, scr[1] + string.Empty);
				}
				else
				{
					AddPopupMessage(17, scr[1] + string.Empty);
				}
				break;
			case 36:
				num2 = FanActData.GetDispParamToEva(scr[1], 2);
				FanActData.AddFanParam(num2, 2);
				if (num2 >= 0)
				{
					AddPopupMessage(18, scr[1] + string.Empty);
				}
				else
				{
					AddPopupMessage(19, scr[1] + string.Empty);
				}
				break;
			case 37:
				subForm = new SubForm(72);
				Push(subForm);
				break;
			case 38:
				subForm = new SubForm(52);
				Push(subForm);
				break;
			case 39:
				subForm = new SubForm(70);
				Push(subForm);
				break;
			case 40:
				subForm = new SubForm(69);
				subForm.value1_ = scr[1];
				Push(subForm);
				break;
			case 41:
				teamData = tData_[0];
				teamData.AddTeamEva(scr[1]);
				if (scr[1] >= 0)
				{
					AddPopupMessage(20, scr[1] + string.Empty);
				}
				else
				{
					AddPopupMessage(21, scr[1] + string.Empty);
				}
				break;
			case 42:
				subForm = new SubForm(73);
				Push(subForm);
				break;
			}
			return subForm;
		}

		public bool CheckTermScript(int[][] terms)
		{
			bool flag = true;
			for (int i = 0; i < terms.Length; i++)
			{
				flag = CheckTermScript(terms[i]);
				if (!flag)
				{
					break;
				}
			}
			return flag;
		}

		public bool CheckTermScript(int[] terms)
		{
			long num = 0L;
			num = 0L;
			int[] termsAlias = terms;
int num2 = termsAlias[1];
			int num3 = 2;
			bool flag = true;
			switch (num2)
			{
			case 0:
				flag = false;
				break;
			case 1:
				num = save_.lngs[0];
				break;
			case 2:
				flag = CheckAlreadyEvent(termsAlias[num3++]);
				break;
			case 3:
				num = save_.ints[1] + 1;
				break;
			case 4:
				num = save_.ints[2] + 1;
				break;
			case 5:
				num = save_.ints[3] + 1;
				break;
			case 6:
				if (frmSoccer_.game_sec / 6 == termsAlias[2])
				{
					return true;
				}
				return false;
			case 7:
			{
				int rankIndex = udat_.GetRankIndex();
				if (rankIndex + 1 >= termsAlias[2])
				{
					return true;
				}
				return false;
			}
			case 8:
			{
				int num7 = udat_.matchResult_[0] + udat_.matchResult_[1] + udat_.matchResult_[2];
				if (num7 >= termsAlias[2])
				{
					return true;
				}
				return false;
			}
			case 9:
			{
				int num6 = udat_.matchResult_[0];
				if (num6 >= termsAlias[2])
				{
					return true;
				}
				return false;
			}
			case 10:
			{
				int num5 = udat_.matchResult_[1];
				if (num5 >= termsAlias[2])
				{
					return true;
				}
				return false;
			}
			case 11:
			{
				int num4 = udat_.matchResult_[2];
				if (num4 >= termsAlias[2])
				{
					return true;
				}
				return false;
			}
			case 12:
			{
				int fanDispParam = FanActData.GetFanDispParam(FanActData.GetFanEva(0), 0);
				if (fanDispParam >= termsAlias[2])
				{
					return true;
				}
				return false;
			}
			}
			if (flag && num3 < termsAlias.Length)
			{
				flag = termsAlias[num3] <= num && (termsAlias.Length <= num3 + 1 || num <= termsAlias[num3 + 1]);
			}
			if (termsAlias[0] == 1)
			{
				flag = !flag;
			}
			if (!flag)
			{
				return false;
			}
			return true;
		}

		public int GetDelayId(int[][] script)
		{
			for (int i = 0; i < eventData_.Length; i++)
			{
				if (eventData_[i] != null && script == eventData_[i].execsScript_)
				{
					return 0 + i;
				}
			}
			if (Config.PRINT)
			{
				Log.Info("getDelayId:-1");
			}
			return -1;
		}

		public int[][] GetDelayScript(int delayId)
		{
			int num = delayId % 1000;
			if (delayId < 1000)
			{
				return eventData_[num].execsScript_;
			}
			return null;
		}

		public static void Print(string str)
		{
			if (Config.PRINT)
			{
				Log.Info("■" + str);
			}
		}

		public void ClearZSortObjects()
		{
			gameObjectPoolIndex_ = 0;
			zsortObjectStart_ = zsortObjectCount_.Length;
			zsortObjectLast_ = 0;
			for (int i = 0; i < zsortObjectCount_.Length; i++)
			{
				zsortObjectCount_[i] = 0;
			}
		}

		public void InsertGameObject(int type, int id, int depth, int seb, int frame, int x, int y)
		{
			InsertGameObject(type, id, depth, -1, seb, frame, x, y, false, -1);
		}

		public void InsertGameObject(int type, int id, int depth, int img, int seb, int frame, int x, int y)
		{
			InsertGameObject(type, id, depth, img, seb, frame, x, y, false, -1);
		}

		public void InsertGameObject(int type, int id, int depth, int seb, int frame, int x, int y, bool dark)
		{
			InsertGameObject(type, id, depth, -1, seb, frame, x, y, dark, -1);
		}

		public void InsertGameObject(int type, int id, int depth, int img, int seb, int frame, int x, int y, bool dark)
		{
			InsertGameObject(type, id, depth, img, seb, frame, x, y, dark, -1);
		}

		public void InsertGameObject(int type, int id, int depth, int img, int seb, int frame, int x, int y, bool dark, int layer)
		{
			if (gameObjectPoolIndex_ >= gameObjectPool_.Length)
			{
				return;
			}
			int num;
			while (true)
			{
				num = depth + 360;
				if (num < 0 || zsortObjectCount_.Length <= num)
				{
					return;
				}
				if (zsortObjectCount_[num] < 30)
				{
					break;
				}
				depth++;
			}
			zsortObjects_[num][zsortObjectCount_[num]] = gameObjectPoolIndex_;
			zsortObjectCount_[num]++;
			if (zsortObjectStart_ > num)
			{
				zsortObjectStart_ = num;
			}
			if (zsortObjectLast_ < num)
			{
				zsortObjectLast_ = num;
			}
			int[] array = gameObjectPool_[gameObjectPoolIndex_];
			array[0] = type;
			array[1] = id;
			array[2] = depth;
			array[3] = img;
			array[4] = seb;
			array[5] = frame;
			array[6] = x;
			array[7] = y;
			array[8] = (dark ? 1 : 0);
			array[9] = layer;
			gameObjectPoolIndex_++;
		}

		public void DrawZSortObjects(Graphics g)
		{
			DrawZSortObjects(g, false, 65535);
		}

		public void DrawZSortObjects(Graphics g, bool offscreen)
		{
			DrawZSortObjects(g, offscreen, 65535);
		}

		public void DrawZSortObjects(Graphics g, bool offscreen, int typeBits)
		{
			Player player = null;
			int num = 0;
			MeshManager meshManager = g.GetMeshManager();
			for (int i = zsortObjectStart_; i <= zsortObjectLast_; i++)
			{
				for (int j = 0; j < zsortObjectCount_[i]; j++)
				{
					int[] array = gameObjectPool_[zsortObjects_[i][j]];
					bool flag = array[8] == 1;
					int num2 = array[0];
					if (!offscreen)
					{
						meshManager.SetDepth(array[2] * 1024);
					}
					switch (num2)
					{
					case 1:
						if (BitUtil.Check(typeBits, 1 << num2))
						{
							player = udat_.GetPlayer(array[1] / 100, array[1] % 100);
							player.Draw(g, array[6], array[7], array[9], true);
						}
						break;
					case 2:
						if (BitUtil.Check(typeBits, 1 << num2))
						{
							TPlayer tPlayer2 = udat_.GetTPlayer(array[1]);
							tPlayer2.Draw(g, array[6], array[7], array[9], false);
						}
						break;
					case 3:
						if (BitUtil.Check(typeBits, 1 << num2))
						{
							udat_.tp_svisor_.Draw(g, array[6], array[7], array[9], false);
						}
						break;
					case 8:
						if (BitUtil.Check(typeBits, 1 << num2))
						{
							DrawSeb(g, array[6], array[7], resCom2_.seb[array[4]], resCom2_.img[array[3]], array[5]);
						}
						break;
					case 9:
						if (!BitUtil.Check(typeBits, 1 << num2))
						{
							break;
						}
						DrawSeb(g, array[6], array[7], resCom2_.seb[array[4]], resCom2_.img[array[3]], array[5]);
						if (SoccerForm.SHOW_DEBUGDRAW == 1)
						{
							int[][] array2 = UserData.COLLISION_SPOS[1];
							g.SetColor(Graphics.GetColorOfRGB(255, 0, 0));
							for (int k = 0; k < array2.Length; k++)
							{
								g.DrawLine(array[6] + array2[k][0], array[7] - array2[k][1], array[6] + array2[(k + 1) % 4][0], array[7] - array2[(k + 1) % 4][1]);
							}
						}
						break;
					case 4:
						if (BitUtil.Check(typeBits, 1 << num2))
						{
							DrawSeb(g, array[6], array[7], resMapChip_.seb[array[4]], resMapChip_.img[array[3]], array[5]);
						}
						break;
					case 5:
						if (BitUtil.Check(typeBits, 1 << num2))
						{
							DrawSeb(g, array[6], array[7], resMapChipMain_.seb[array[4]], resMapChipMain_.img[array[3]], array[5]);
						}
						break;
					case 6:
						if (BitUtil.Check(typeBits, 1 << num2))
						{
							g.DrawImage(resMapChipMain_.img[array[3]], array[6], array[7]);
						}
						break;
					case 7:
						if (BitUtil.Check(typeBits, 1 << num2))
						{
							g.DrawImage(resMapObject_.img[array[3]], array[6], array[7]);
						}
						break;
					case 10:
						if (BitUtil.Check(typeBits, 1 << num2))
						{
							TPlayer tPlayer = udat_.tPlayers_[array[1]];
							PlayerData playerData = pData_[tPlayer.pData_id_];
							int num3 = array[6] - 25;
							num = array[7] - 18;
							g.DrawImage(resCom2_.img[136], num3, num, 0, 0, 51, 18);
							g.ClipRect(num3 + 1, num + 2, 16f, 13f);
							g.SetImageClip(num3 + 1, num + 2, 16f, 13f);
							udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, 0, 1);
							udat_.DrawDispPlayer(g, num3 + 10, num + 16 + 6);
							g.ClearImageClip();
							g.PopClip();
							int width = GameUtil.RateConvert(tPlayer.useTenantCnt_, 0, Tenant.USETENANT_TIME[0], 0, 30, true);
							g.DrawImage(resCom2_.img[136], num3 + 19, num + 10, 19, 18, width, 4);
						}
						break;
					}
				}
			}
			meshManager.SetDepthFront();
		}

		public void AddPopupScriptBuffer(int[] script, int popId, string popMsg)
		{
			if (script == null)
			{
				script = new int[0];
			}
			if (popMsg == null)
			{
				popMsg = string.Empty;
			}
			popupScriptBuffer_.AddElement(new object[3]
			{
				script,
				new JInteger(popId),
				popMsg
			});
		}

		public void ChangeMap(int next_mapId)
		{
			SetNowStoreIndex(next_mapId);
		}

		public int GetSeason()
		{
			if (save_.ints[2] >= 2 && save_.ints[2] <= 4)
			{
				return 0;
			}
			if (save_.ints[2] >= 5 && save_.ints[2] <= 7)
			{
				return 1;
			}
			if (save_.ints[2] >= 8 && save_.ints[2] <= 10)
			{
				return 2;
			}
			return 3;
		}

		public void ClearPopupMessages()
		{
			popupMessages_.RemoveAllElements();
		}

		public void AddPopupMessage(int id, string args)
		{
			int existPopupNum = GetExistPopupNum(id);
			if (existPopupNum >= 2) { return; }
{
string value = MyFormBase.LT(UserData.POPUP_MEM[id]);
value = StringUtil.Replace(value, args);
AddPopupMessage(id, value, -1, 60);
}
		}

		public void AddPopupMessage(int id, string msg, int wait, int time)
		{
			TextLayout textLayout = new TextLayout(false);
			textLayout.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
			textLayout.SetText(msg);
			textLayout.Flush();
			popupMessages_.AddElement(new object[2]
			{
				textLayout,
				new int[3] { id, wait, time }
			});
		}

		public void RemovePopupMessages(int id)
		{
			for (int num = popupMessages_.Size() - 1; num >= 0; num--)
			{
				object[] array = (object[])popupMessages_.ElementAt(num);
				if (((int[])array[1])[0] == id)
				{
					popupMessages_.RemoveElementAt(num);
				}
			}
		}

		public void DrawPopupMessage(Graphics g, int bottom)
		{
			GameView instance = GameView.GetInstance();
			int num = 8;
			int num2 = bottom;
			for (int i = 0; i < popupMessages_.Size(); i++)
			{
				object[] array = (object[])popupMessages_.ElementAt(i);
				int[] array2 = (int[])array[1];
				if (array2[1] >= 0)
				{
					int num3 = array2[1];
					int num4 = 19;
					if (i == 0)
					{
						num4 = 21;
					}
					int num5 = 0;
					num5 = ((num3 <= num) ? (num4 * num3 / num) : ((num3 < array2[2] + num) ? num4 : (num4 - num4 * (num3 - (array2[2] + num)) / num)));
					drawPopupMessage_heights[i] = num5;
					if (num5 > 0)
					{
						num2 -= num5;
					}
				}
			}
			int num6 = ClampMin(instance.GetGameWidth(), 240 + instance.GetSideSoftWidth() * 2);
			int num7 = bottom - num2 + 20;
			int num8 = 0;
			if (GamePad.GetInstance().GetMode() == 2)
			{
				num8 += instance.GetSideSoftWidth();
			}
			int num9 = num6 - instance.GetSideSoftWidth();
			double num10 = (double)(num6 - instance.GetSideSoftWidth() * 2) / 240.0;
			int num11 = 0;
			if (!Language.Japanese())
			{
				num11 = 1;
			}
			g.ClipRect(0f, SoccerForm.VIEW_Y, instance.GetWidth(), bottom - SoccerForm.VIEW_Y);
			for (int num12 = popupMessages_.Size() - 1; num12 >= 0; num12--)
			{
				object[] array3 = (object[])popupMessages_.ElementAt(num12);
				int[] array4 = (int[])array3[1];
				if (array4[1] >= 0)
				{
					TextLayout textLayout = (TextLayout)array3[0];
					if (drawPopupMessage_heights[num12] > 0)
					{
						Draw_helpMessage(g, textLayout.GetText(), num8, num2);
						num2 += drawPopupMessage_heights[num12];
					}
				}
			}
			g.PopClip();
		}

		public void Draw_helpMessage(Graphics g, string text, int x, int y)
		{
			Font font = g.GetFont(null);
			g.DrawImage(resCom2_.img[77], x, y, 0, 0, 32, 22);
			g.DrawImage(resCom2_.img[172], x + 2, y + 1, 0, 0, 16, 19);
			int num = 0;
			string text2 = text;
			if (text.IndexOf("<co=", 0) != -1)
			{
				text2 = text2.Replace("<co=0064FF>", string.Empty);
				text2 = text2.Replace("</co>", string.Empty);
			}
			int num2 = font.StringWidth(text2) + 16;
			int num3 = 32;
			GameView instance = GameView.GetInstance();
			int num4 = instance.GetGameWidth() - instance.GetSideSoftWidth();
			int num5 = x + 32;
			int num6 = 0;
			int num7 = num2;
			num = 0;
			do
			{
				num6 = ClampMax(108, num7);
				if (num5 + num6 > num4)
				{
					num6 = num4 - num5;
				}
				g.DrawImage(resCom2_.img[77], num5, y, 32, 0, num6, 22);
				num5 += num6;
				num7 -= num6;
				if (num5 >= num4) {
					break;
				}
if (num7 == 0) {
					break;
				}
				num++;
			}
			while (num < 5);
			g.SetColor(Graphics.GetColorOfRGB(30, 30, 30));
			g.DrawString2(text, x + 32, y + 6);
			g.DrawImage(resCom2_.img[77], num5 - 16, y, 160, 0, 16, 22);
		}

		public void UpdatePopupMessage()
		{
			for (int num = popupMessages_.Size() - 1; num >= 0; num--)
			{
				object[] array = (object[])popupMessages_.ElementAt(num);
				int[] array2 = (int[])array[1];
				if (++array2[1] < 0) { continue; }
int num2 = array2[1];
if (num2 == 1)
					{
						PlaySe(24);
					}
int num3 = 8;
int num4 = num3 * 2 + array2[2];
if (num2 > num4)
					{
						popupMessages_.RemoveElementAt(num);
					}
			}
		}

		public object[] GetPopupMessage(int key)
		{
			for (int num = popupMessages_.Size() - 1; num >= 0; num--)
			{
				object[] array = (object[])popupMessages_.ElementAt(num);
				int[] array2 = (int[])array[1];
				if (array2[3] == key)
				{
					return array;
				}
			}
			return null;
		}

		public bool ChExistPopup(int id)
		{
			for (int num = popupMessages_.Size() - 1; num >= 0; num--)
			{
				object[] array = (object[])popupMessages_.ElementAt(num);
				int[] array2 = (int[])array[1];
				if (array2[0] == id)
				{
					return true;
				}
			}
			return false;
		}

		public int GetExistPopupNum(int id)
		{
			int num = 0;
			for (int num2 = popupMessages_.Size() - 1; num2 >= 0; num2--)
			{
				object[] array = (object[])popupMessages_.ElementAt(num2);
				int[] array2 = (int[])array[1];
				if (array2[0] == id)
				{
					num++;
				}
			}
			return num;
		}

		public void AddDebugLog(string debug_log)
		{
			debugLog_.AddElement(debug_log);
			if (debugLog_.Size() > 50)
			{
				debugLog_.RemoveElementAt(0);
			}
		}

		public void SendSaveData(string text)
		{
			SendSaveData(text, false);
		}

		public void SendSaveData(string text, bool scratchpad)
		{
			SendSaveData(text, scratchpad, null);
		}

		public void SendSaveData(string text, bool scratchpad, Exception th)
		{
			SendSaveData(text, scratchpad ? 1 : 0, th);
		}

		public void SendSaveData(string text, int type)
		{
			SendSaveData(text, type, null);
		}

		public void SendSaveData(string text, int type, Exception th)
		{
			text = "dev=" + platform_ + "\nver" + 202 + "\n" + text;
			Canvas instance = Canvas.GetInstance();
			instance.SetSoftLabel(0, string.Empty);
			instance.SetSoftLabel(1, string.Empty);
			JCalendar instance2 = JCalendar.GetInstance();
			instance2.SetTime(new JDate(JSystem.CurrentTimeMillis()));
			string text2 = string.Empty + instance2.Get(1);
			string text3 = string.Empty + (instance2.Get(2) + 1);
			if (text3.Length() == 1)
			{
				text3 = "0" + text3;
			}
			string text4 = string.Empty + instance2.Get(5);
			if (text4.Length() == 1)
			{
				text4 = "0" + text4;
			}
			string text5 = string.Empty + instance2.Get(11);
			if (text5.Length() == 1)
			{
				text5 = "0" + text5;
			}
			string text6 = string.Empty + instance2.Get(12);
			if (text6.Length() == 1)
			{
				text6 = "0" + text6;
			}
			string text7 = string.Empty + instance2.Get(13);
			if (text7.Length() == 1)
			{
				text7 = "0" + text7;
			}
			string date = string.Empty + text2 + "." + text3 + "." + text4 + "_" + text5 + "." + text6 + "." + text7 + string.Empty;
			bool flag = false;
			sendState_ = 0;
			instance.ClearKeyBuffer();
			string text8 = null;
			if (th != null)
			{
				text8 = string.Empty;
				if (Http.GetErrorUrl() != null)
				{
					string text9 = Http.GetErrorUrl();
					int num = text9.LastIndexOf('/');
					if (num != -1)
					{
						text9 = text9.SubstringJ(num + 1);
					}
					num = text9.LastIndexOf('.');
					if (num != -1)
					{
						text9 = text9.SubstringJ(0, num);
					}
					string text10 = text8;
					text8 = text10 + text9 + "," + Http.GetErrorCode() + "\n";
				}
				string text11 = string.Empty + th;
				string text12 = th.GetType().Name;
				int num2 = text12.LastIndexOf('.');
				if (num2 != -1)
				{
					text12 = text12.SubstringJ(num2 + 1);
				}
				num2 = text11.IndexOf(text12);
				if (num2 != -1)
				{
					text11 = text11.SubstringJ(num2);
				}
				if (th is ConnectionException)
				{
					text11 = text11 + "," + ((ConnectionException)th).GetStatus();
				}
				if (th is UIException)
				{
					text11 = text11 + "," + ((UIException)th).GetStatus();
				}
				text8 += text11;
			}
			if (Config.APPLI_TYPE == 0)
			{
				Draw(text8);
				while ((instance.GetKeypadState() & 0x100000) != 0)
				{
					try
					{
						JThread.Sleep(100L);
					}
					catch (ThreadAbortException)
					{
					}
				}
			}
			while (true)
			{
				instance.DecideKeyState();
				try
				{
					if (sendState_ == 0)
					{
						Draw(text8, true);
						if (instance.CheckKeyPulse(1024))
						{
							instance.ImeOnBlock(text8, 0, 0);
						}
						else if (instance.CheckKeyPulse(1048576))
						{
							sendState_ = 1;
						}
						goto IL_0817;
					}
					if (sendState_ != 1)
					{
						goto IL_0817;
					}
					Draw(text8);
					byte[] array = null;
					if (Config.APPLI_TYPE == 0)
					{
						switch (type)
						{
						case 1:
							text += "\nscratchpad";
							array = ScratchPad.ReadByteArray(0, 0, ScratchPad.GetSPSize(0));
							break;
						case 2:
						{
							text += "\nall-scratchpad";
							byte[] array2 = ScratchPad.ReadByteArray(0, 0, ScratchPad.GetSPSize(0));
							byte[] array3 = ScratchPad.ReadByteArray(1, 0, ScratchPad.GetSPSize(1));
							array = new byte[array2.Length + array3.Length];
							JSystem.Arraycopy(array2, 0, array, 0, array2.Length);
							JSystem.Arraycopy(array3, 0, array, array2.Length, array3.Length);
							break;
						}
						case 0:
							try
							{
								RecordStore recordStore = RecordStore.OpenRecordStore(0);
								array = RecordStore.ReadRecord(0, 1 + currentSaveSlot_);
								recordStore.CloseRecordStore();
							}
							catch (Exception ex2)
							{
								if (!flag)
								{
									text = text + "\nRecordStore[" + ex2.ToString() + "]";
								}
								array = ScratchPad.ReadByteArray(0, 0, ScratchPad.GetSPSize(0));
								flag = true;
							}
							break;
						}
					}
					else if (Config.APPLI_TYPE == 1)
					{
						try
						{
							RecordStore recordStore2 = RecordStore.OpenRecordStore(0);
							array = RecordStore.ReadRecord(0, 1 + currentSaveSlot_);
							recordStore2.CloseRecordStore();
						}
						catch (Exception)
						{
							flag = true;
						}
					}
					string url = Config.URL_SP + "&ver=" + 202;
					int num3 = 51200;
					byte[][] array4 = new byte[(array.Length + num3 - 1) / num3 + 1][];
					string[] array5 = new string[array4.Length];
					string[] array6 = new string[array5.Length];
					if (Config.DEBUG)
					{
						string str = ((save_ == null) ? string.Empty : save_.strs[0]) + "\n" + text + "\n";
						try
						{
							RecordStore recordStore3 = RecordStore.OpenRecordStore(0);
							byte[] data = RecordStore.ReadRecord(0, 1 + currentSaveSlot_);
							Property property = new Property(defSaveP_, defSaveI_, defSaveL_, defSaveS_, defSaveB_);
							property.Load(data);
							recordStore3.CloseRecordStore();
						}
						catch (Exception)
						{
						}
						array4[0] = str.GetBytes();
					}
					else
					{
						array4[0] = (((save_ == null) ? string.Empty : save_.strs[0]) + "\n" + text).GetBytes();
					}
					array5[0] = "info";
					array6[0] = "0";
					int num4 = 0;
					for (int i = 0; i < array4.Length - 1; i++)
					{
						int num5 = array.Length - num4;
						if (num5 > num3)
						{
							num5 = num3;
						}
						int num6 = i + 1;
						array4[num6] = new byte[num5];
						JSystem.Arraycopy(array, num4, array4[num6], 0, num5);
						array5[num6] = "save";
						switch (type)
						{
						case 1:
							array5[num6] += "-sys";
							break;
						case 2:
							array5[num6] += "-all";
							break;
						}
						array6[num6] = JString.ValueOf(i);
						num4 += num5;
					}
					int num7 = 0;
					while (true)
					{
						IL_07cd:
						if (num7 < array4.Length)
						{
							while (true)
							{
								Draw(text8);
								try
								{
									Post(url, array5[num7], array6[num7], date, array4[num7]);
								}
								catch (Exception t)
								{
									if (Config.PRINT)
									{
										Log.Info(t);
									}
									Dialog dialog = new Dialog(3, "ｴﾗｰ");
									dialog.SetText("エラーが発生しました。リトライしますか？");
									if (dialog.Show() != 8)
									{
										continue;
									}
									while ((instance.GetKeypadState() & 0x100000) != 0)
									{
										try
										{
											JThread.Sleep(100L);
										}
										catch (ThreadAbortException)
										{
										}
									}
									sendState_ = 0;
									instance.ClearKeyBuffer();
									break;
								}
								num7++;
								goto IL_07cd;
							}
							break;
						}
						sysSave_.strs[10] = string.Empty;
						SaveSystem();
						Dialog dialog2 = new Dialog(0, "完了");
						dialog2.SetText("送信を完了しました。\nご協力ありがとうございます。\n今後もカイロパークを宜しくお願い致します。");
						dialog2.Show();
						return;
					}
					goto end_IL_038e;
					IL_0817:
					JThread.Sleep(100L);
					end_IL_038e:;
				}
				catch (Exception t2)
				{
					if (Config.PRINT)
					{
						Log.Info(t2);
					}
					Dialog dialog3 = new Dialog(3, "ｴﾗｰ");
					dialog3.SetText("エラーが発生しました。リトライしますか？");
					if (dialog3.Show() != 8)
					{
						continue;
					}
					while ((instance.GetKeypadState() & 0x100000) != 0)
					{
						try
						{
							JThread.Sleep(100L);
						}
						catch (ThreadAbortException)
						{
						}
					}
					sendState_ = 0;
					instance.ClearKeyBuffer();
				}
			}
		}

		private void Draw(string err)
		{
			Draw(err, false);
		}

		private void Draw(string err, bool copy)
		{
			global::kairo.unity.ui.Graphics graphics;
Canvas instance = Canvas.GetInstance();
			graphics = instance.BeginPaint();
			Font font = graphics.GetFont(null);
			graphics.SetColor(Graphics.GetColorOfRGB(0, 0, 64));
			graphics.FillRect(0f, 0f, 240f, 240f);
			if (err != null)
			{
				graphics.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				graphics.DrawRect(1f, 1f, 237f, 147f);
				graphics.DrawRect(1f, 150f, 237f, 88f);
				graphics.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				graphics.DrawString("【エラー情報】", 120f, 5f, 2);
				TextLayout.Draw(graphics, err, 5f, 20f, 230f, 100f, Graphics.GetColorOfRGB(255, 255, 255), 2);
				if (copy)
				{
					graphics.SetColor(Graphics.GetColorOfRGB(0, 255, 255));
					graphics.DrawString("[＊]キーでコピー出来ます", 120f, 133f, 2);
				}
			}
			int num = 106;
			if (err != null)
			{
				num = 186;
			}
			if (sendState_ == 0)
			{
				graphics.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				string text = "決定キーを押すと送信を開始します。";
				graphics.DrawString(text, 120 - font.StringWidth(text) / 2, num);
				if (instance.CheckKeyPulse(1048576))
				{
					sendState_ = 1;
				}
			}
			else if (sendState_ == 1)
			{
				graphics.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				string text = "セーブデータの送信中...";
				graphics.DrawString(text, 120 - font.StringWidth(text) / 2, num);
				text = "電源キーを押さないでください。";
				graphics.DrawString(text, 120 - font.StringWidth(text) / 2, num + 16);
			}
			instance.EndPaint();
		}

		private void Post(string url, string file, string index, string date, byte[] data)
		{
			Http.Connect(url + "&file=" + file + "&index=" + index + "&date=(" + date + ")", data, true);
		}

		public void Draw_trophy(Graphics g, int frame)
		{
			if (sysSave_.ints[10] > 0)
			{
				int px = 118;
				int py = GameView.GetInstance().GetGameHeight() - 90;
				DrawTrophyEffect(g, sysSave_.ints[10] - 1, frame, px, py);
			}
		}

		public void DrawTrophyEffect(Graphics g, int trophy_id, int subform_frame, int px, int py)
		{
			DrawSeb(g, px, py, resCom2_.seb[106], resCom2_.img[190], sysSave_.ints[10] - 1);
			int num = (subform_frame - 20) % 66;
			int[] array = null;
			array = resCom2_.seb[106].GetSprite(trophy_id, 0);
			int num2 = array[5];
			int[][] array2 = new int[2][]
			{
				new int[4] { 1, 4, 1, 128 },
				new int[4] { 1, 2, 5, 255 }
			};
			int num3 = 4;
			int num4 = num - 2;
			for (int i = 0; i < array2.Length; i++)
			{
				if (i == 1)
				{
					continue;
				}
				int[] array3 = array2[i];
				int num5 = num4 * num3;
				int num6 = 0;
				if (num5 <= array[4] / 2)
				{
					num6 = Graph.Easing(array3[0], array3[1], array[4] / 2, num5, 0);
				}
				if (num5 >= array[4] / 2)
				{
					num6 = Graph.Easing(array3[1], array3[0], array[4] / 2, num5 - array[4] / 2, 0);
				}
				int num7 = px + array[6] + num5 - num6 / 2;
				int num8 = py + array[7];
				int num9 = array[2] + num5 - num6 / 2;
				int sy = array[3];
				int num10 = num6;
				int height = num2;
				if (num9 < array[2])
				{
					int num11 = array[2] - num9;
					num9 += num11;
					num10 -= num11;
					num7 += num11;
				}
				if (num9 + num10 > array[2] + array[4])
				{
					num10 = array[2] + array[4] - num9;
				}
				if (num10 > 0)
				{
					g.SetRenderMode(1, array3[3], 255);
					for (int j = 0; j < array3[2]; j++)
					{
						g.DrawImage(resCom2_.img[array[1]], num7, num8, num9, sy, num10, height);
					}
					g.SetRenderMode(0, 255, 255);
				}
			}
		}

		public static int Random(int total)
		{
			return GameUtil.Random(total);
		}

		public static bool IsLanguageOther()
		{
			return Language.GetLanguageCode() != "ja-JP" && Language.GetLanguageCode() != "en-US" && !Language.GetLanguageCode().StartsWith("*");
		}

		public static bool IsLanguageJpBase(bool includeJP)
		{
			if (includeJP && MyFormBase.Jp())
			{
				return true;
			}
			string languageCode = Language.GetLanguageCode();
			if ("ko".Equals(languageCode))
			{
				return true;
			}
			if ("zh".Equals(languageCode))
			{
				return true;
			}
			if ("zh-CN".Equals(languageCode))
			{
				return true;
			}
			return false;
		}

		public static int GetFig(long value)
		{
			return (int)Math.Log10(Math.Abs(value)) + 1;
		}

		public void DrawExp(Graphics g, int x, int y)
		{
			if (IsSC_LangCode())
			{
				g.DrawImage(resCom2_.img[117], x - 11, y - 2);
			}
			else
			{
				DrawSeb(g, x, y, resCom2_.seb[76], resCom2_.img[117], 0);
			}
		}

		public void DrawLv(Graphics g, string text, int x, int y)
		{
			if (IsSC_LangCode())
			{
				SubForm.SetFontSize(g, 8);
				g.DrawString(text, x, y + 3);
				SubForm.ResetFontSize(g);
			}
			else
			{
				g.DrawString(text, x, y);
			}
		}

		public void DrawPositionStatus(Graphics g, int index, int rank, int x, int y)
		{
			int num = 18;
			if (IsSC_LangCode())
			{
				SubForm.SetFontSize(g, 6);
				g.DrawString(PlayerData.POSITION_NAME[index], x + 6, y + 1, 2);
				num++;
			}
			else
			{
				SubForm.SetFontSize(g, 8);
				g.DrawString(PlayerData.POSITION_NAME[index], x, y + (MyFormBase.En() ? 2 : 0));
			}
			SubForm.ResetFontSize(g);
			if (rank < 0) { return; }
{

DrawSeb(g, x + num, y + 11, resCom2_.seb[65], resCom2_.img[75], rank);
}
		}

		public static bool IsSC_LangCode()
		{
			string languageCode = Language.GetLanguageCode();
			if (languageCode != null && languageCode.Equals("zh-CN"))
			{
				return true;
			}
			return false;
		}
	}
}
