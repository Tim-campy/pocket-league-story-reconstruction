using System;
using data;
using game;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.graphics;
using kairo.unity.io;
using kairo.unity.m3d4.util3d;
using kairo.unity.surface;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;

namespace form
{
	public class SoccerForm : MyFormBase
	{
		public const int STATUS_AMEFTO_NORMAL = 0;

		public const int STATUS_AMEFTO_DRIBBLE = 1;

		public const int STATUS_AMEFTO_LONGPASS = 2;

		public const int STATE_MAIN = 0;

		public const int STATE_PRI_MAIN = 1;

		public const int STATE_MATCHEND = 2;

		public const int STATE_MATCHEND2 = 3;

		public const int STATE_MATCHFINISH = 4;

		public const int STATE_MATCHFINISH2 = 5;

		public const int STATE_EDIT = 6;

		public const int STATE_STADIUM = 7;

		public const int STATE_STADIUM_FADEOUT = 8;

		public const int STATE_BONUS_FAN = 9;

		public const int STATE_BONUS_INCOME = 10;

		public const int STATE_BONUS_INCOME2 = 11;

		public const int STATE_BONUS_MATCH_ELSE = 12;

		public const int STATE_BONUS_LEAGUE = 13;

		public const int STATE_BONUS_NP_LV = 14;

		public const int STATE_BONUS_NP_POPULAR = 15;

		public const int STATE_BONUS_NP_TRAINING = 16;

		public const int PINCH_SACLE_PADDING = 40;

		public const int ECM_MAINMENU = 0;

		public const int ECM_MEMBER = 1;

		public const int ECM_MEMBER2 = 2;

		public const int ECM_FORMATION_T1 = 3;

		public const int ECM_FORMATION_T2 = 4;

		public const int ECM_COAT = 5;

		public const int ECM_COAT2 = 6;

		public const int ECM_END = 7;

		public const int E_F_W = 9;

		public const int E_F_H = 9;

		public const int E_F_OFFY = 10;

		public const int E_GRASS_W = 10;

		public const int E_GRASS_H = 10;

		public const int SCR_EVENT = 0;

		public const int SCR_WAIT = 1;

		public const int SCR_SCENE = 2;

		public const int SCR_SPECIAL = 3;

		public const int SCR_CARD = 4;

		public const int SCR_GM_CHANGESTATE = 5;

		public const int STADIUM_CAMERA_FADETIME = 25;

		public const int PBONUS_NP_LV_SANIME = 12;

		public const int TBARTYPE_BLACK = 0;

		public const int TBARTYPE_T1 = 1;

		public const int TBARTYPE_T1_NAME = 2;

		public const int TBARTYPE_CENTER = 3;

		public const int TBARTYPE_T2_NAME = 4;

		public const int TBARTYPE_T2 = 5;

		public const int TBARTYPE_SIDE = 6;

		public const int CUSTOMER_MIN = 0;

		public const int CUSTOMER_MAX = 100000;

		private const float w_fan = 0.5f;

		private const float w_assist_home = 0.15f;

		private const float w_assist_away = 0.05f;

		private const float w_nameValue = 0.1f;

		private const float w_enemyWP = 0.05f;

		private const float w_sumPopular = 0.1f;

		public UserData udat_;

		public int game_sec;

		public int game_sec_old;

		public bool flg_priCustomerInStore;

		public int stadiumId_;

		public Player statusPlayer_;

		public int statusPlayerChangeWait_;

		public int statusAmeftoMode_;

		public int statusAmeftoCnt_;

		public static readonly int[] STATUS_AMEFTO_ANIME = new int[2] { 20, 30 };

		public static readonly string[] STATUS_AMEFTO_NAME = new string[3] { "通常", "ﾄﾞﾘﾌﾞﾙ中心", "ﾛﾝｸﾞﾊﾟｽ中心" };

		public static readonly int[] STATUS_AMEFTO_COLOR = new int[3]
		{
			Graphics.GetColorOfRGB(0, 0, 0),
			Graphics.GetColorOfRGB(255, 255, 84),
			Graphics.GetColorOfRGB(31, 143, 255)
		};

		public int state_;

		public int stateCnt_;

		public static readonly int[][] softLabels_ = new int[22][]
		{
			new int[2] { 0, 4 },
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2] { 6, 0 },
			new int[2] { 6, 0 },
			new int[2] { 6, 0 },
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2]
		};

		private new SubForm dialog_;

		private SubForm menu_;

		private int frame_;

		public long proc_time;

		public long draw_time;

		public static int BOTTOM_INFO_BAR = 0;

		public static int VIEW_X = 0;

		public static int VIEW_Y = 21;

		public static int VIEW_W = 240;

		public static int VIEW_H = 240 - VIEW_Y - BOTTOM_INFO_BAR;

		public int scale_;

		public int pinchScaleValue_;

		public static Offscreen offGameScreen_;

		private static bool resetOffscreen_;

		public bool buildTrackX_;

		public bool buildTrachY_;

		public bool isBuildTrack_;

		public int buildDownX_;

		public int buildDownY_;

		public bool buildLock_;

		public int touchBuildDragFrame_;

		public bool touchSkip_;

		public bool autoSave_;

		public int[] editCursor = new int[7];

		public int[] editStock = new int[7];

		public int editCursorMode;

		public int editTeam;

		public Vector script_;

		public static readonly int[] BONUS_P_CYCLE = new int[4] { 20, 50, 70, 95 };

		public Vector bonus_p_list = new Vector();

		public int bonus_p_cnt;

		public int bonus_p_index;

		public int select_;

		public int select2_;

		public int scrollValue_;

		public static readonly int[] BONUS_F_CYCLE = new int[5] { 30, 42, 102, 114, 144 };

		public static readonly int[] BONUS_F_CYCLE_MAIN = new int[4] { 25, 40, 55, 60 };

		public int bonus_f_cnt;

		public int bonus_f_index;

		public int temp_fan;

		public int temp_assist;

		public int temp_nameValue;

		public int temp_addFan;

		public int temp_addAssist;

		public int temp_addNameValue;

		public static readonly int[] BONUS_IN_CYCLE = new int[6] { 22, 44, 66, 88, 110, 130 };

		public int[][] bonus_in_param = JTool.MakeArray<int[][]>(new int[2] { 6, 2 });

		public int bonus_in_cnt;

		public int bonus_in_index;

		public int bonus_softKey;

		public int bonus_sponsorNum;

		public static readonly int[] STADIUM_CAMERA_TIME = new int[3] { 20, 185, 205 };

		public static readonly Vector3D[][] STADIUM_CAERA_BASE_POS = new Vector3D[3][]
		{
			new Vector3D[2]
			{
				new Vector3D(-480f, -171f, 0f),
				new Vector3D(0f, 69f, 0f)
			},
			new Vector3D[2]
			{
				new Vector3D(-480f, -210f, 0f),
				new Vector3D(0f, 30f, 0f)
			},
			new Vector3D[2]
			{
				new Vector3D(-480f, -143f, 0f),
				new Vector3D(0f, 97f, 0f)
			}
		};

		public static readonly int[] STADIUM_BTMINFO_TIME = new int[4] { 20, 65, 95, 115 };

		public static readonly int[] PBONUS_NP_LV_TIME = new int[4] { 0, 40, 55, 95 };

		public int PBONUS_NP_LV_ADDEXP;

		public int PBONUS_NP_LV_GETPOPULAR;

		public int PBONUS_NP_LV_GETTRAINING;

		public static readonly int[] PBONUS_NP_LV_POPULAR = new int[4] { 10, 22, 47, 77 };

		public static readonly int[] PBONUS_NP_LV_TRAINING1 = new int[2] { 10, 232 };

		public static readonly int[] PBONUS_NP_LV_T_GET = new int[2] { 42, 62 };

		public static int DISP_COPAREA = 1;

		public static int FLG_PROCESSTIME = 0;

		public static int SHOW_FPS = 1;

		public static int SHOW_MEMORY = 0;

		public static int SHOW_PDTIME = 1;

		public static int SHOW_DRAWNUM = 0;

		public static int SHOW_DEBUGDRAW = 0;

		public static int SHOW_DEBUGDRAW2 = 0;

		public static int DEBUG_GK_MODE = 0;

		public static int DEBUG_PLAYER_NOTACT = ((DEBUG_GK_MODE == 1) ? 1 : 0);

		public static int DEBUG_PASSSTOP_MODE = 0;

		public static int DEBUG_1KEYSTOP_MODE = 0;

		public static int DEBUG_WALLREFRECT = 0;

		public static int DEBUG_3KEY_KICKTEST = 1;

		public static int DEBUG_BALLTEST_MODE = 0;

		public static int DEBUG_BALLPLAYER = 0;

		public static int DEBUG_1KEYSTOP_MODE2 = 1;

		public static int DEBUG_8KEY_FAST = 1;

		public static int DEBUG_SCREENTRAVEL = 0;

		public static int DEBUG_START_STATE = -1;

		public static int DEBUG_START_P1_TEAMID = -1;

		public static int DEBUG_START_P2_TEAMID = -1;

		public static int DEBUG_GAMETIMESTOP = 0;

		public static int DEBUG_GAMEMODE_5KEY = 0;

		public static long CH_MEMORY_SUM;

		public static long CH_MEMORY_OLD;

		public static readonly int[] TEST_STATE = new int[3] { 0, 6, 7 };

		public int stadiumMoveCnt_;

		private bool drawTopBar;

		public static readonly int[] TBAR_SX = new int[7] { 99, 0, 14, 99, 141, 226, 1 };

		public static readonly int[] TBAR_W = new int[7] { 17, 14, 85, 42, 85, 14, 12 };

		private bool debugProStop_;

		private int offscreenDayTime_ = -1;

		private string offscreenDayTimeString_ = string.Empty;

		private long offscreenMoney_;

		private string offscreenMoneyString_ = "0";

		private int[] offUpdateSRect = new int[4];

		private bool flg_allDraw;

		internal int COAT_X = 131;

		internal int COAT_Y = 10;

		public int[] customerNum = new int[2];

		internal Vector3D touchCameraSpeed_ = new Vector3D();

		internal Vector3D touchCameraDecimal_ = new Vector3D();

		internal FastVector touchCameraPoint_ = new FastVector();

		internal bool touchDragCamera_;

		internal bool touchBuildDrag_;

		private int[] getScaleRange_tmp = new int[2];

		public SoccerForm()
			: base(2)
		{
		}

		public override string ToString()
		{
			return "SoccerForm:" + state_;
		}

		public override bool Init()
		{
			base.Init();
			udat_ = UserData.GetInstance();
			menu_ = new SubForm(4, 1, 24);
			script_ = new Vector();
			if (DEBUG_START_P1_TEAMID != -1 || DEBUG_START_P2_TEAMID != -1)
			{
				int[] nextBattleLeagueIndex = udat_.GetNextBattleLeagueIndex(udat_.league_setu_);
				udat_.gm_.SetBattleTeamId(DEBUG_START_P1_TEAMID, DEBUG_START_P2_TEAMID, nextBattleLeagueIndex[1]);
			}
			apdat_.camera.ChangeMode(2);
			udat_.gm_.StartSoccerMatch();
			if (udat_.gm_.scorerListUserTeam.Size() > 0)
			{
				udat_.gm_.scorerListUserTeam.RemoveAllElements();
			}
			int state = 7;
			if (DEBUG_START_STATE == -1)
			{
				ChangeState(state);
			}
			else
			{
				udat_.gm_.score_[0] = DebugForm.debug_testscore[0];
				udat_.gm_.score_[1] = DebugForm.debug_testscore[1];
				for (int i = 0; i < DebugForm.debug_scorevec.Size(); i++)
				{
					JInteger obj = (JInteger)DebugForm.debug_scorevec.ElementAt(i);
					udat_.gm_.scorerListUserTeam.AddElement(obj);
				}
				ChangeState(DEBUG_START_STATE);
				DEBUG_START_STATE = -1;
			}
			if (apdat_.resMapChip_ != null)
			{
				apdat_.resMapChip_.Dispose();
			}
			apdat_.resMapChip_ = new ResourceManager(null, null, null, null, null, null, null);
			apdat_.resMapChip_.Load(RecordStore.ReadRecord(1, 4));
			if (apdat_.resMapObject_ != null)
			{
				apdat_.resMapObject_.Dispose();
			}
			apdat_.resMapObject_ = new ResourceManager(null, null, null, null, null, null, null);
			apdat_.resMapObject_.Load(RecordStore.ReadRecord(1, 5));
			try
			{
				TeamData teamData = null;
				if (udat_.gm_.homeAway_ == 0) { teamData = apdat_.tData_[udat_.gm_.teamId_[0]]; } else { teamData = apdat_.tData_[udat_.gm_.teamId_[1]]; }
				stadiumId_ = teamData.stadium_;
				Map.CreateMap(stadiumId_, apdat_.stadiumData_[stadiumId_].fileName_);
				SetAllDraw();
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
			SetCustomerNum();
			scale_ = 100;
			pinchScaleValue_ = 120;
			InitOffscreen();
			SoundPlayer instance = SoundPlayer.GetInstance();
			instance.SetVolume(1, apdat_.sysSave_.ints[4] * 51);
			int num = 0;
			if (num == 1)
			{
				udat_.ball_.pos_.z_ = 0f;
				DEBUG_GK_MODE = 0;
				DEBUG_PLAYER_NOTACT = ((DEBUG_GK_MODE == 1) ? 1 : 0);
				DEBUG_PASSSTOP_MODE = 0;
				DEBUG_1KEYSTOP_MODE = 0;
				DEBUG_WALLREFRECT = 0;
				DEBUG_3KEY_KICKTEST = 1;
				int num2 = 0;
				int num3 = 0;
				int num4 = 1;
				if (num2 == 1)
				{
					apdat_.camera.ChangeMode(0);
				}
				if (num3 == 1)
				{
					apdat_.camera.ChangeMode(1);
				}
				if (num4 == 1)
				{
					apdat_.camera.ChangeMode(2);
				}
				if (DEBUG_BALLTEST_MODE == 1)
				{
					apdat_.camera.ChangeMode(1);
				}
				if (DEBUG_BALLPLAYER == 1)
				{
					apdat_.camera.ChangeMode(1);
				}
			}
			return true;
		}

		public void GameScreenLayout(out int width, out int height, out Image.Format format, out Image.Filter filterMode)
		{
			width = view_.GetGameWidth() * 100 / 60 + 1;
			height = view_.GetGameHeight() * 100 / 60 + 1;
			format = Image.Format.ARGB32;
			filterMode = Image.Filter.Point;
		}

		public bool StaticRender(Graphics g, Offscreen offscreen)
		{
			apdat_.ClearZSortObjects();
			resetOffscreen_ = false;
			return true;
		}

		public bool RenderGameScreen(Graphics g, Offscreen offscreen)
		{
			VIEW_W = GetGameWidth();
			VIEW_H = GetGameHeight();
			Camera.SetDrawCenter(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			UpdateOffscreen(g);
			bool flag = true;
			if (state_ == 6 || state_ == 7 || state_ == 8)
			{
				flag = false;
			}
			g.SetClip(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			Seb.SetClip(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			if (flag)
			{
				apdat_.ClearZSortObjects();
				apdat_.map_.Draw(g, false);
				for (int i = 0; i < udat_.teams_.Length; i++)
				{
					udat_.teams_[i].Draw(g, false);
				}
				udat_.ball_.Draw(g);
				apdat_.DrawZSortObjects(g, false);
				apdat_.DrawEffect(g);
			}
			else
			{
				apdat_.ClearZSortObjects();
				apdat_.map_.Draw(g, false);
				apdat_.DrawZSortObjects(g, false);
			}
			Seb.ClearClip();
			g.ClearClip();
			return true;
		}

		public void ResetOffscreen()
		{
			SetAllDraw();
		}

		private void InitOffscreen()
		{
			offGameScreen_ = Offscreen.CreateOffscreen(RenderGameScreen, StaticRender, GameScreenLayout);
			apdat_.offSeason_ = -1;
			apdat_.offYear_ = -1;
			apdat_.offMonth_ = -1;
			apdat_.offWeek_ = -1;
			apdat_.offHour_ = -1;
			apdat_.offSec_ = -1;
			apdat_.offMoney_ = -2147483648L;
			apdat_.offScore1_ = -1;
			apdat_.offScore2_ = -1;
		}

		public int GetTopBarOffN()
		{
			int result = 0;
			if (GameView.GetInstance().GetGameWidth() > 240)
			{
				result = 30;
			}
			return result;
		}

		public int GetDrawTBAROff(int type)
		{
			int topBarOffN = GetTopBarOffN();
			int num = AppData.ClampMin((view_.GetGameWidth() - (240 + topBarOffN * 2)) / 2, 0);
			if (type == 1)
			{
				return num;
			}
			num += TBAR_W[1];
			if (type == 2)
			{
				return num + topBarOffN / 2 + 43;
			}
			num += TBAR_W[2] + topBarOffN;
			num += TBAR_W[3];
			if (type == 4)
			{
				return num + topBarOffN / 2 + 43;
			}
			num += TBAR_W[4] + topBarOffN;
			if (type == 5)
			{
				return num;
			}
			return num;
		}

		public void Draw_topBarParts(Graphics g, int type, int dx)
		{
			Draw_topBarParts(g, type, dx, TBAR_W[type]);
		}

		public void Draw_topBarParts(Graphics g, int type, int dx, int w)
		{
			int height = VIEW_Y - 1;
			switch (type)
			{
			case 0:
			{
				if (w <= TBAR_W[type])
				{
					g.DrawImage(apdat_.resCom2_.img[32], dx, 0f, TBAR_SX[type], 0, w, height);
					break;
				}
				int num7 = w;
				int num8 = 0;
				while (num7 > 0)
				{
					int num9 = AppData.Clamp(num7, 1, TBAR_W[type]);
					g.DrawImage(apdat_.resCom2_.img[32], dx + num8, 0f, TBAR_SX[type], 0, num9, height);
					num7 -= num9;
					num8 += num9;
				}
				break;
			}
			case 1:
			case 3:
			case 5:
				g.DrawImage(apdat_.resCom2_.img[32], dx, 0f, TBAR_SX[type], 0, w, height);
				switch (type)
				{
				case 1:
					g.DrawImage(apdat_.resCom2_.img[32], dx - 1, 0f, TBAR_SX[type] + 1, 0, 3, height);
					break;
				case 5:
					g.DrawImage(apdat_.resCom2_.img[32], dx - 1 + w, 0f, TBAR_SX[type] + 1, 0, 3, height);
					break;
				}
				break;
			case 2:
			case 4:
			{
				if (w <= TBAR_W[type])
				{
					g.DrawImage(apdat_.resCom2_.img[32], dx, 0f, TBAR_SX[type], 0, w, height);
					break;
				}
				int num4 = w;
				int num5 = 0;
				while (num4 > 0)
				{
					int num6 = AppData.Clamp(num4, 1, TBAR_W[type]);
					g.DrawImage(apdat_.resCom2_.img[32], dx + num5, 0f, TBAR_SX[type], 0, num6, height);
					num4 -= num6;
					num5 += num6;
				}
				break;
			}
			case 6:
			{
				if (w <= TBAR_W[type])
				{
					g.DrawImage(apdat_.resCom2_.img[32], dx, 0f, TBAR_SX[type], 0, w, height);
					break;
				}
				int num = w;
				int num2 = 0;
				while (num > 0)
				{
					int num3 = AppData.Clamp(num, 1, TBAR_W[type]);
					g.DrawImage(apdat_.resCom2_.img[32], dx + num2, 0f, TBAR_SX[type], 0, num3, height);
					num -= num3;
					num2 += num3;
				}
				break;
			}
			}
		}

		public int[] GetFirstScript()
		{
			if (script_.Size() == 0)
			{
				return null;
			}
			return (int[])script_.FirstElement();
		}

		public void ClearScript()
		{
			script_.RemoveAllElements();
		}

		public void AddCommandWait(int wait)
		{
			script_.AddElement(new int[2] { 1, wait });
		}

		public void AddCommandEvent(int evtId)
		{
			script_.AddElement(new int[2] { 0, evtId });
		}

		public void AddCommandScene(int on)
		{
			script_.AddElement(new int[2] { 2, on });
		}

		public void AddCommandSpecial(int[] arg)
		{
			if (arg == null)
			{
				arg = new int[0];
			}
			int[] array = new int[arg.Length + 1];
			array[0] = 3;
			if (arg.Length > 0)
			{
				JSystem.Arraycopy(arg, 0, array, 1, arg.Length);
			}
			script_.AddElement(array);
		}

		public void AddCommandCard(int card, int team, int p_id)
		{
			script_.AddElement(new int[5] { 4, 0, card, team, p_id });
		}

		public void AddCommandGMChangeState(int gm_state)
		{
			script_.AddElement(new int[2] { 5, gm_state });
		}

		public void Script()
		{
			while (script_.Size() > 0)
			{
				int[] firstScript = GetFirstScript();
				switch (firstScript[0])
				{
				case 1:
					Push(SubForm.MakeWait(firstScript[1]));
					script_.RemoveElementAt(0);
					return;
				case 0:
					script_.RemoveElementAt(0);
					return;
				case 2:
					script_.RemoveElementAt(0);
					break;
				case 3:
					script_.RemoveElementAt(0);
					return;
				case 4:
					if (firstScript[1] == 0)
					{
						udat_.gm_.ChangeState(10);
						udat_.gm_.time_card = firstScript[2];
						udat_.gm_.time_team = firstScript[3];
						udat_.gm_.time_pId = firstScript[4];
						udat_.gm_.time_animePattern = GameUtil.Random(4);
					}
					firstScript[1]++;
					if (firstScript[1] >= GameManager.WAIT_CARD[GameManager.WAIT_CARD.Length - 1])
					{
						udat_.gm_.ChangeState(0);
						script_.RemoveElementAt(0);
						break;
					}
					return;
				case 5:
					udat_.gm_.ChangeState(firstScript[1]);
					script_.RemoveElementAt(0);
					break;
				default:
					return;
				}
			}
		}

		public override bool Resume()
		{
			return true;
		}

		public override bool Finish()
		{
			if (apdat_.resMapChip_ != null)
			{
				apdat_.resMapChip_.Dispose();
			}
			apdat_.resMapChip_ = null;
			if (apdat_.resMapObject_ != null)
			{
				apdat_.resMapObject_.Dispose();
			}
			apdat_.resMapObject_ = null;
			if (offGameScreen_ != null)
			{
				offGameScreen_.Dispose();
				offGameScreen_ = null;
			}
			apdat_.map_ = null;
			udat_.teams_ = null;
			JSystem.Gc();
			return true;
		}

		public void ChangeState(int state)
		{
			int[] array = softLabels_[state];
			if (array != null)
			{
				SetSoftLabel(AppData.softLabels_[array[0]], AppData.softLabels_[array[1]]);
			}
			state_ = state;
			stateCnt_ = 0;
			if (state_ == 0)
			{
				if (DEBUG_START_P1_TEAMID == -1 && DEBUG_START_P2_TEAMID == -1)
				{
					udat_.gm_.StartSoccerForm(true);
				}
				else
				{
					udat_.gm_.StartSoccerForm(false);
				}
				SetAllDraw();
				int matchInfoJoin = udat_.GetMatchInfoJoin();
				int bgm = 3;
				switch (matchInfoJoin)
				{
				case 0:
					if (LeagueData.IsVictoryMatch())
					{
						bgm = 2;
					}
					break;
				case 1:
				{
					QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
					if (BitUtil.Check(questMatchData.flag_, 2) && QuestMatchData.IsVictoryQuestMatch())
					{
						bgm = 2;
					}
					break;
				}
				}
				apdat_.PlayBgm(bgm);
				drawTopBar = false;
			}
			else if (state_ == 6)
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
				int num = udat_.editTest_formation.IndexOf(elem);
				if (num != -1)
				{
					udat_.et_cur[1] = num;
				}
				for (int j = 0; j < editCursor.Length; j++)
				{
					editCursor[j] = 0;
				}
				udat_.SetEditForPos(true);
				udat_.et_cur[0] = 1;
				apdat_.PlayBgm(1);
				TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[1]];
				udat_.SetPlayerParam(teamData.warP_paramPhysical_, teamData.warP_paramPhysical_);
				SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[0]);
			}
			else if (state_ == 7)
			{
				stadiumMoveCnt_ = 0;
				SetAllDraw();
			}
			else if (state_ == 9)
			{
				if (FanBonusData.bonus_f_data1.Size() > 0)
				{
					FanBonusData.bonus_f_data1.RemoveAllElements();
				}
				bonus_f_cnt = 0;
				bonus_f_index = 0;
				FanBonusData.ChBonusEvent();
				if (FanBonusData.bonus_f_data1.Size() == 0)
				{
					ChangeState(14);
					return;
				}
				temp_fan = FanActData.GetFanEva(0);
				temp_assist = FanActData.GetFanEva(1);
				temp_nameValue = FanActData.GetFanEva(2);
				object[] array2 = (object[])FanBonusData.bonus_f_data1.ElementAt(0);
				int[] array3 = (int[])array2[2];
				temp_addFan = array3[0];
				temp_addAssist = array3[1];
				temp_addNameValue = array3[2];
			}
			else if (state_ == 10)
			{
				bonus_in_param[0][0] = udat_.ticketPrice_ * customerNum[0] / 10000;
				bonus_in_param[0][1] = 0;
				bonus_in_param[1][0] = GetMatchPrice();
				bonus_in_param[1][1] = 0;
				bonus_in_param[2][0] = GetGoodsPrice();
				bonus_in_param[2][1] = 0;
				bonus_in_param[3][0] = GetSponsorPrice();
				bonus_in_param[3][1] = 0;
				bonus_in_param[4][0] = 0;
				bonus_in_param[4][1] = GetPlayersSalary();
				bonus_in_cnt = 0;
				bonus_in_index = 0;
				bonus_softKey = 0;
				bonus_sponsorNum = GetSponsorNum();
			}
			else
			{
				if (state_ == 2)
				{
					return;
				}
				if (state_ == 14)
				{
					for (int k = 0; k < udat_.memberPDataist_.Size(); k++)
					{
						PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(k);
						playerData.temp_lv = playerData.lv_;
						playerData.temp_exp = playerData.exp_;
						playerData.temp_anime = 0;
						playerData.temp_lvUp = false;
					}
					TeamData teamData2 = apdat_.tData_[udat_.gm_.teamId_[0]];
					TeamData teamData3 = apdat_.tData_[udat_.gm_.teamId_[1]];
					PBONUS_NP_LV_ADDEXP = GameUtil.RateConvert(teamData3.warPotential_, 50, 700, 150, 2200, false);
					PBONUS_NP_LV_ADDEXP = AppData.Clamp(PBONUS_NP_LV_ADDEXP, 150, 5000);
					if (udat_.gm_.score_[0] == udat_.gm_.score_[1])
					{
						PBONUS_NP_LV_ADDEXP /= 2;
					}
					else if (udat_.gm_.score_[0] < udat_.gm_.score_[1])
					{
						PBONUS_NP_LV_ADDEXP /= 4;
					}
					PBONUS_NP_LV_GETPOPULAR = GameUtil.RateConvert(teamData3.warPotential_, 50, 700, 0, 15, false);
					PBONUS_NP_LV_GETTRAINING = GameUtil.RateConvert(teamData3.warPotential_, 50, 700, 5, 15, false);
					int num2 = int.MaxValue;
					int num3 = 0;
					int num4 = 0;
					int num5 = 0;
					for (int l = 0; l < udat_.memberPDataist_.Size(); l++)
					{
						PlayerData playerData2 = (PlayerData)udat_.memberPDataist_.ElementAt(l);
						if (num2 > playerData2.actscoreSum_)
						{
							num2 = playerData2.actscoreSum_;
						}
						else if (num3 < playerData2.actscoreSum_)
						{
							num3 = playerData2.actscoreSum_;
						}
						num5 += playerData2.actscoreSum_;
					}
					num4 = num5 / udat_.memberPDataist_.Size();
					int num6 = 0;
					num6 = ((udat_.gm_.score_[0] <= udat_.gm_.score_[1]) ? ((udat_.gm_.score_[0] == udat_.gm_.score_[1]) ? 1 : 2) : 0);
					int num7 = 100 - num6 * 20;
					int x = teamData2.warPotential_ - teamData3.warPotential_;
					int num8 = GameUtil.RateConvert(x, 100, -100, 80, 120, true);
					int x2 = num7 / 2 + num8 / 2 + GameUtil.Random(10) - 5;
					int c2max = GameUtil.RateConvert(x2, 0, 100, 0, 100, true);
					int c2min = GameUtil.RateConvert(x2, 0, 100, 0, 30, true);
					for (int m = 0; m < udat_.memberPDataist_.Size(); m++)
					{
						PlayerData playerData3 = (PlayerData)udat_.memberPDataist_.ElementAt(m);
						playerData3.actscoreScore_ = GameUtil.RateConvert(playerData3.actscoreSum_, num2, num3, c2min, c2max, true);
					}
				}
				else if (state_ == 16)
				{
					SupervisorData supervisorData = apdat_.svisorData_[udat_.selectSupervisorId_];
					for (int n = 0; n < 11; n++)
					{
						PlayerData p_data = (PlayerData)udat_.memberPDataist_.ElementAt(n);
						supervisorData.SetResearchData(udat_.game_matchEva_, p_data);
					}
				}
			}
		}

		public override bool Update()
		{
			long num = JSystem.CurrentTimeMillis();
			frame_ = (frame_ + 1) % int.MaxValue;
			int num2 = 1;
			if (apdat_.save_.ints[13] == 1)
			{
				num2 = 2;
			}
			if (state_ != 0)
			{
				num2 = 1;
			}
			if (Config.DEBUG_CMD || Config.PTW)
			{
				if (canvas_.CheckKeyState(16))
				{
					num2 = 2;
				}
				else if (canvas_.CheckKeyState(32))
				{
					num2 = 20;
				}
				else if (canvas_.CheckKeyState(64))
				{
					num2 = 10;
				}
				if (!canvas_.CheckKeyRepeat(16) || canvas_.CheckKeyRepeat(32))
				{
				}
				if (DEBUG_GAMEMODE_5KEY == 1)
				{
					num2 = 20;
				}
			}
			if (Config.DEBUG_CMD)
			{
				if (canvas_.CheckKeyRepeat(2))
				{
					SHOW_DEBUGDRAW = 1 - SHOW_DEBUGDRAW;
				}
				if (canvas_.CheckKeyRepeat(4))
				{
					SHOW_DEBUGDRAW2 = 1 - SHOW_DEBUGDRAW2;
				}
			}
			if (state_ == 0)
			{
				apdat_.camera.Update_pos();
			}
			for (int i = 0; i < num2 && !_update(i); i++)
			{
			}
			proc_time = JSystem.CurrentTimeMillis() - num;
			return true;
		}

		private bool _update(int frameCount)
		{
			if (Config.DEBUG_CMD && canvas_.CheckKeyPulse(1))
			{
				return formMan_.Push(new DebugForm());
			}
			stateCnt_ = (stateCnt_ + 1) % int.MaxValue;
			Script();
			apdat_.UpdateEffect();
			int num = 5;
			if (state_ == 0)
			{
				Update_main();
				SetStatusPlayer();
				if (statusPlayer_ != null && canvas_.CheckKeyRepeat(1048576))
				{
					PlayerData playerData = apdat_.pData_[statusPlayer_.pData_id_];
					if (playerData.auraPoint_ == 10000 && !BitUtil.Check(statusPlayer_.flag_, 262144) && udat_.gm_.state_ == 0)
					{
						playerData.Start_aura(statusPlayer_);
					}
				}
				for (int i = 0; i < udat_.teams_[0].players_.Length; i++)
				{
					Player player = udat_.teams_[0].players_[i];
					PlayerData playerData2 = apdat_.pData_[player.pData_id_];
					playerData2.Update_aura(player);
				}
				if (udat_.gm_.state_ != 1 && statusAmeftoCnt_ > 0)
				{
					statusAmeftoCnt_--;
				}
				int num2 = statusAmeftoMode_;
				if (statusAmeftoCnt_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144))
					{
						statusAmeftoCnt_ = STATUS_AMEFTO_ANIME[1];
					}
				}
				else
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						statusAmeftoMode_ = (statusAmeftoMode_ - 1 + 3) % 3;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						statusAmeftoMode_ = (statusAmeftoMode_ + 1 + 3) % 3;
					}
				}
				if (num2 != statusAmeftoMode_)
				{
					if (!udat_.gm_.halfTime)
					{
						udat_.teams_[0].amefto_pz = Team.AMEFTO_PZ[statusAmeftoMode_];
					}
					else
					{
						udat_.teams_[1].amefto_pz = Team.AMEFTO_PZ[statusAmeftoMode_];
					}
					statusAmeftoCnt_ = STATUS_AMEFTO_ANIME[1];
					if (statusAmeftoMode_ == 2)
					{
						if (!apdat_.CheckAlreadyEvent(139))
						{
							apdat_.ExecEvent(139, null, null);
						}
					}
					else if (statusAmeftoMode_ == 1 && !apdat_.CheckAlreadyEvent(140))
					{
						apdat_.ExecEvent(140, null, null);
					}
				}
				if (Config.DEBUG_CMD && canvas_.CheckKeyPulse(128))
				{
					AddCommandCard(GameUtil.Random(2), 0, 0);
				}
				if (apdat_.IsPushSoftLabel(4))
				{
					return Push(menu_);
				}
			}
			else if (state_ == 1)
			{
				if (stateCnt_ >= 2)
				{
					ChangeState(0);
				}
			}
			else if (state_ == 2)
			{
				if (udat_.gm_.score_[0] > udat_.gm_.score_[1])
				{
					udat_.matchResult_[0]++;
				}
				else if (udat_.gm_.score_[0] < udat_.gm_.score_[1])
				{
					udat_.matchResult_[2]++;
				}
				else
				{
					udat_.matchResult_[1]++;
				}
				udat_.matchGetGoalTotal_ += udat_.gm_.score_[0];
				int matchInfoJoin = udat_.GetMatchInfoJoin();
				if (matchInfoJoin == 0)
				{
					udat_.Update_league();
				}
				udat_.Update_gameParam();
				ChangeState(3);
				switch (matchInfoJoin)
				{
				case 0:
					if (LeagueData.IsVictoryLeague())
					{
						Push(new SubForm(75));
						Push(new SubForm(38));
					}
					break;
				case 1:
				{
					QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
					if (BitUtil.Check(questMatchData.flag_, 2) && QuestMatchData.IsVictoryQuestMatch())
					{
						Push(new SubForm(75));
						Push(new SubForm(38));
					}
					break;
				}
				}
				Push(new SubForm(37));
			}
			else if (state_ == 3)
			{
				ChangeState(9);
			}
			else if (state_ == 4)
			{
				udat_.matchInfoWeek_ = 0;
				udat_.Init_bus();
				apdat_.camera.SetMapMainBasePos();
				if (udat_.GetMatchInfoJoin() == 0)
				{
					if (LeagueData.IsVictoryLeague())
					{
						udat_.matchFlgVictory_ = 1;
					}
					udat_.league_setu_++;
					udat_.League_finish();
				}
				else
				{
					if (QuestMatchData.IsVictoryQuestMatch())
					{
						udat_.matchFlgVictory_ = 1;
					}
					udat_.Qm_update();
				}
				ChangeState(5);
			}
			else if (state_ == 5)
			{
				formMan_.ChangeCurrentForm(apdat_.frmGame_);
				apdat_.frmGame_.ChangeState(0);
			}
			else if (state_ == 6)
			{
				apdat_.resCom2_.seb[61].Frame();
				apdat_.resCom2_.seb[60].Frame();
				apdat_.resCom2_.seb[62].Frame();
				if (udat_.Update_edit(true))
				{
					ChangeState(0);
				}
			}
			else if (state_ == 7)
			{
				stadiumMoveCnt_++;
				SetAllDraw();
				if (stateCnt_ < STADIUM_CAMERA_TIME[0])
				{
					Camera.pos.Set(STADIUM_CAERA_BASE_POS[stadiumId_][0]);
				}
				else if (stateCnt_ < STADIUM_CAMERA_TIME[1])
				{
					float x_ = Camera.pos.x_;
					float y_ = Camera.pos.y_;
					Camera.pos.x_ = GameUtil.RateConvert(stateCnt_, STADIUM_CAMERA_TIME[0], STADIUM_CAMERA_TIME[1], STADIUM_CAERA_BASE_POS[stadiumId_][0].x_, STADIUM_CAERA_BASE_POS[stadiumId_][1].x_, true);
					Camera.pos.y_ = GameUtil.RateConvert(stateCnt_, STADIUM_CAMERA_TIME[0], STADIUM_CAMERA_TIME[1], STADIUM_CAERA_BASE_POS[stadiumId_][0].y_, STADIUM_CAERA_BASE_POS[stadiumId_][1].y_, true);
				}
				else
				{
					Camera.pos.Set(STADIUM_CAERA_BASE_POS[stadiumId_][1]);
				}
				if (canvas_.CheckKeyPulse(1048576) && stateCnt_ >= STADIUM_BTMINFO_TIME[1])
				{
					stateCnt_ = STADIUM_CAMERA_TIME[2];
				}
				if (stateCnt_ >= STADIUM_CAMERA_TIME[2])
				{
					ChangeState(8);
				}
			}
			else if (state_ == 8)
			{
				if (stateCnt_ >= 25)
				{
					ChangeState(6);
					Vector3D global_pos = new Vector3D();
					int[] array = new int[2];
					Camera.ConvertGlobalToScreen(global_pos, array);
					Camera.pos.x_ = array[0];
					Camera.pos.y_ = array[1];
				}
			}
			else if (state_ == 9)
			{
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (bonus_f_cnt < BONUS_F_CYCLE[1] + BONUS_F_CYCLE_MAIN[0])
					{
						bonus_f_cnt = BONUS_F_CYCLE[1] + BONUS_F_CYCLE_MAIN[0];
					}
					else
					{
						bonus_f_cnt = BONUS_F_CYCLE[BONUS_F_CYCLE.Length - 1];
					}
				}
				bonus_f_cnt++;
				if (bonus_f_cnt >= BONUS_F_CYCLE[BONUS_F_CYCLE.Length - 1])
				{
					bonus_f_cnt = 0;
					bonus_f_index++;
					if (bonus_f_index < FanBonusData.bonus_f_data1.Size())
					{
						temp_fan += temp_addFan;
						temp_assist += temp_addAssist;
						temp_nameValue += temp_addNameValue;
						object[] array2 = (object[])FanBonusData.bonus_f_data1.ElementAt(bonus_f_index);
						int[] array3 = (int[])array2[2];
						temp_addFan = array3[0];
						temp_addAssist = array3[1];
						temp_addNameValue = array3[2];
					}
					if (bonus_f_index >= FanBonusData.bonus_f_data1.Size())
					{
						ChangeState(14);
						for (int j = 0; j < FanBonusData.bonus_f_data1.Size(); j++)
						{
							object[] array4 = (object[])FanBonusData.bonus_f_data1.ElementAt(j);
							int[] array5 = (int[])array4[2];
							int num3 = ((JInteger)array4[3]).IntValue();
							int[] array6 = (int[])array4[4];
							int num4 = ((JInteger)array4[5]).IntValue();
							FanActData.AddFanParam(array5[0], 0);
							FanActData.AddFanParam(array5[1], 1);
							FanActData.AddFanParam(array5[2], 2);
							int fanEva = FanActData.GetFanEva(0);
							if (num4 != 1)
							{
								continue;
							}
							switch (num3)
							{
							case 0:
							{
								for (int l = 0; l < array6.Length; l++)
								{
									PlayerData playerData4 = apdat_.pData_[array6[l]];
									playerData4.AddPopular(1);
								}
								break;
							}
							case 1:
							{
								for (int k = 0; k < array6.Length; k++)
								{
									PlayerData playerData3 = apdat_.pData_[array6[k]];
									playerData3.AddPopular(3);
								}
								break;
							}
							}
						}
					}
				}
			}
			else if (state_ == 10)
			{
				if (canvas_.CheckKeyPulse(1048576))
				{
					bonus_in_cnt = BONUS_IN_CYCLE[BONUS_IN_CYCLE.Length - 1];
				}
				for (int m = 0; m < BONUS_IN_CYCLE.Length; m++)
				{
					if (bonus_in_cnt < BONUS_IN_CYCLE[m])
					{
						if (bonus_in_param[m][0] == 0 && bonus_in_param[m][1] == 0)
						{
							bonus_in_cnt = BONUS_IN_CYCLE[m];
						}
						break;
					}
				}
				bonus_in_cnt++;
				if (bonus_in_cnt >= BONUS_IN_CYCLE[BONUS_IN_CYCLE.Length - 1])
				{
					bonus_in_index++;
					if (bonus_in_index >= 1)
					{
						ChangeState(11);
						for (int n = 0; n < bonus_in_param.Length; n++)
						{
							switch (n)
							{
							case 0:
							case 1:
							case 2:
								apdat_.AddMoney(bonus_in_param[n][0], 0);
								apdat_.SubMoney(bonus_in_param[n][1], 0);
								break;
							case 3:
								apdat_.AddMoney(bonus_in_param[n][0], 2);
								apdat_.SubMoney(bonus_in_param[n][1], 2);
								break;
							case 4:
								apdat_.AddMoney(bonus_in_param[n][0], 3);
								apdat_.SubMoney(bonus_in_param[n][1], 3);
								break;
							default:
								apdat_.AddMoney(bonus_in_param[n][0], 4);
								apdat_.SubMoney(bonus_in_param[n][1], 4);
								break;
							}
						}
					}
				}
			}
			else if (state_ == 11)
			{
				apdat_.resCom2_.seb[60].Frame();
				if (canvas_.CheckKeyPulse(1048576) && select_ == 0)
				{
					ChangeState(12);
					if (udat_.GetMatchInfoJoin() == 0)
					{
						ChangeState(12);
					}
					else
					{
						ChangeState(4);
					}
				}
			}
			else if (state_ == 12)
			{
				apdat_.resCom2_.seb[60].Frame();
				if (canvas_.CheckKeyPulse(1048576) && select_ == 0)
				{
					ChangeState(4);
					Push(new SubForm(33, 1));
				}
			}
			else if (state_ == 14)
			{
				apdat_.resCom2_.seb[60].Frame();
				for (int num5 = 0; num5 < udat_.memberPDataist_.Size(); num5++)
				{
					PlayerData playerData5 = (PlayerData)udat_.memberPDataist_.ElementAt(num5);
					if (playerData5.temp_anime > 0)
					{
						playerData5.temp_anime--;
					}
				}
				if (stateCnt_ >= PBONUS_NP_LV_TIME[2] && stateCnt_ < PBONUS_NP_LV_TIME[3])
				{
					for (int num6 = 0; num6 < udat_.memberPDataist_.Size(); num6++)
					{
						PlayerData playerData6 = (PlayerData)udat_.memberPDataist_.ElementAt(num6);
						int num7 = GameUtil.RateConvert(stateCnt_ - 1, PBONUS_NP_LV_TIME[2], PBONUS_NP_LV_TIME[3], 0, PBONUS_NP_LV_ADDEXP, true);
						int num8 = GameUtil.RateConvert(stateCnt_, PBONUS_NP_LV_TIME[2], PBONUS_NP_LV_TIME[3], 0, PBONUS_NP_LV_ADDEXP, true);
						int num9 = num8 - num7;
						playerData6.temp_exp += num9;
						int nextLvUpExp = PlayerData.GetNextLvUpExp(playerData6.temp_lv);
						if (playerData6.temp_exp > nextLvUpExp)
						{
							playerData6.temp_exp -= nextLvUpExp;
							playerData6.temp_lv = AppData.ClampMax(playerData6.temp_lv + 1, 30);
							playerData6.temp_lvUp = true;
							playerData6.temp_anime = 12;
						}
					}
				}
				if (stateCnt_ >= PBONUS_NP_LV_TIME[3])
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
				if (stateCnt_ == PBONUS_NP_LV_TIME[3])
				{
					for (int num10 = 0; num10 < 11; num10++)
					{
						PlayerData playerData7 = (PlayerData)udat_.memberPDataist_.ElementAt(num10);
						playerData7.AddExp(PBONUS_NP_LV_ADDEXP, false);
						playerData7.temp_exp = playerData7.exp_;
						if (playerData7.temp_lv != playerData7.lv_)
						{
							playerData7.temp_lvUp = true;
						}
						playerData7.temp_lv = playerData7.lv_;
					}
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (stateCnt_ < PBONUS_NP_LV_TIME[3])
					{
						stateCnt_ = PBONUS_NP_LV_TIME[3] - 1;
					}
					else if (stateCnt_ >= PBONUS_NP_LV_TIME[3] && select_ == 0)
					{
						ChangeState(16);
					}
				}
				else if (apdat_.IsPushSoftLabel(6))
				{
					bonus_softKey = 1 - bonus_softKey;
				}
			}
			else if (state_ == 15)
			{
				apdat_.resCom2_.seb[60].Frame();
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (stateCnt_ < PBONUS_NP_LV_POPULAR[3])
					{
						stateCnt_ = PBONUS_NP_LV_POPULAR[3] - 1;
					}
					else if (select_ == 0)
					{
						for (int num11 = 0; num11 < udat_.memberPDataist_.Size(); num11++)
						{
							PlayerData playerData8 = (PlayerData)udat_.memberPDataist_.ElementAt(num11);
							playerData8.AddPopular(PBONUS_NP_LV_GETPOPULAR);
						}
						ChangeState(16);
					}
				}
				else if (apdat_.IsPushSoftLabel(6))
				{
					bonus_softKey = 1 - bonus_softKey;
				}
			}
			else if (state_ == 16)
			{
				apdat_.resCom2_.seb[60].Frame();
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (stateCnt_ < PBONUS_NP_LV_TRAINING1[1])
					{
						stateCnt_ = PBONUS_NP_LV_TRAINING1[1];
					}
					else if (select_ == 0)
					{
						for (int num12 = 0; num12 < 11; num12++)
						{
							PlayerData playerData9 = (PlayerData)udat_.memberPDataist_.ElementAt(num12);
							if (playerData9.bonus_research[0] != -1)
							{
								udat_.AddResearchPoint(playerData9.bonus_research[0], playerData9.bonus_research[1]);
							}
						}
						ChangeState(10);
					}
				}
				else if (apdat_.IsPushSoftLabel(6))
				{
					bonus_softKey = 1 - bonus_softKey;
				}
			}
			if (state_ == 0)
			{
			}
			return false;
		}

		public bool Update_main()
		{
			udat_.ball_.Update();
			for (int i = 0; i < udat_.teams_.Length; i++)
			{
				udat_.teams_[i].Update();
			}
			udat_.gm_.Collision();
			udat_.gm_.Update();
			return false;
		}

		public void UpdateOffscreenTopBar(Graphics g, bool all_update)
		{
			if (all_update)
			{
				apdat_.offYear_ = -1;
				apdat_.offMonth_ = -1;
				apdat_.offHour_ = -1;
				apdat_.offSec_ = -1;
				apdat_.offMoney_ = -1L;
				apdat_.offWeatherAlpha_ = -1;
				apdat_.offScore1_ = -1;
				apdat_.offScore2_ = -1;
			}
			global::data.TeamData teamData2;
global::surface.GameView instance;
int topBarOffN;
int num;
TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[0]];
			teamData2 = apdat_.tData_[udat_.gm_.teamId_[1]];
			instance = GameView.GetInstance();
			topBarOffN = GetTopBarOffN();
			num = AppData.ClampMin((instance.GetGameWidth() - (240 + topBarOffN * 2)) / 2, 0);
			if (num > 0)
			{
				Draw_topBarParts(g, 6, 0, num);
				Draw_topBarParts(g, 6, instance.GetGameWidth() - num, num);
			}
			int num2 = num;
			Draw_topBarParts(g, 1, num, TBAR_W[1]);
			num += TBAR_W[1];
			int num3 = num;
			Draw_topBarParts(g, 2, num, TBAR_W[2] + topBarOffN);
			num += TBAR_W[2] + topBarOffN;
			Draw_topBarParts(g, 3, num, TBAR_W[3]);
			num += TBAR_W[3];
			int num4 = num;
			Draw_topBarParts(g, 4, num, TBAR_W[4] + topBarOffN);
			num += TBAR_W[4] + topBarOffN;
			int num5 = num;
			Draw_topBarParts(g, 5, num, TBAR_W[5]);
			num += TBAR_W[5];
			apdat_.DrawSeb(g, num2 + 1, 4, apdat_.resCom2_.seb[28], apdat_.resCom2_.img[33], udat_.selectUniform_);
			apdat_.DrawSeb(g, num5 + 1, 4, apdat_.resCom2_.seb[28], apdat_.resCom2_.img[33], teamData2.uniform_);
			g.SetColor(Graphics.GetColorOfRGB(214, 241, 255));
			g.DrawString(teamData.name_, num3 + topBarOffN / 2 + 43, 4f, 2);
			g.DrawString(teamData2.name_, num4 + topBarOffN / 2 + 43, 4f, 2);
			int num6 = 0;
			if (apdat_.offScore1_ != udat_.gm_.score_[0] || apdat_.offScore2_ != udat_.gm_.score_[1])
			{
				apdat_.offScore1_ = udat_.gm_.score_[0];
				int x = instance.GetGameWidth() / 2 - 13;
				num6 = 4;
				g.DrawImage(apdat_.resCom2_.img[32], instance.GetGameWidth() / 2 - 21, 3f, 99, 3, 17, 16);
				if (udat_.gm_.score_[0] > udat_.gm_.score_[1])
				{
					apdat_.DrawNumImage(g, 31, udat_.gm_.score_[0], x, num6, 0, 2);
				}
				else
				{
					apdat_.DrawNumImage(g, 30, udat_.gm_.score_[0], x, num6, 0, 2);
				}
			}
			if (apdat_.offScore2_ != udat_.gm_.score_[1])
			{
				apdat_.offScore2_ = udat_.gm_.score_[1];
				int x = instance.GetGameWidth() / 2 + 13;
				num6 = 4;
				g.DrawImage(apdat_.resCom2_.img[32], instance.GetGameWidth() / 2 + 4, 3f, 124, 3, 17, 16);
				apdat_.DrawNumImage(g, 30, udat_.gm_.score_[1], x, num6, 0, 2);
			}
		}

		private void Draw_watch(Graphics g, int cx, int cy, double len, int angle)
		{
			float num = GameUtil.Cos(angle);
			float num2 = GameUtil.Sin(angle);
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
			g.DrawLine(cx, cy, cx + (int)(len * (double)num), cy - (int)(len * (double)num2));
		}

		public void UpdateOffscreen(Graphics g)
		{
			int num = 0;
			if (Camera.old_pos.x_ != Camera.pos.x_ || Camera.old_pos.y_ != Camera.pos.y_)
			{
				num = 1;
				if (DISP_COPAREA == 0)
				{
					num = 2;
				}
			}
			if (flg_allDraw)
			{
				flg_allDraw = false;
				num = 2;
			}
			int vIEW_W = VIEW_W;
			int vIEW_H = VIEW_H;
			VIEW_W = GetGameWidth();
			VIEW_H = GetGameHeight();
			Camera.SetDrawCenter(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			g.ClipRect(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			Seb.SetClip(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			apdat_.ClearZSortObjects();
			num = 2;
			if (num == 2)
			{
				g.SetColor(Graphics.GetColorOfRGB(0, 0, 255));
				g.FillRect(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
				apdat_.map_.Draw(g, true);
				Camera.old_pos.x_ = Camera.pos.x_;
				Camera.old_pos.y_ = Camera.pos.y_;
			}
			else
			{
				g.SetColor(Graphics.GetColorOfRGB(0, 0, 255));
				int num2 = (int)Camera.pos.x_;
				int num3 = (int)Camera.old_pos.x_;
				int num4 = -(num2 - num3);
				num2 = (int)Camera.pos.y_;
				num3 = (int)Camera.old_pos.y_;
				int num5 = num2 - num3;
				Camera.old_pos.x_ = Camera.pos.x_;
				Camera.old_pos.y_ = Camera.pos.y_;
				g.CopyArea(VIEW_X, VIEW_Y, VIEW_W, VIEW_H, num4, num5);
				for (int i = 0; i < offUpdateSRect.Length; i++)
				{
					offUpdateSRect[i] = 0;
				}
				if (num4 > 0)
				{
					offUpdateSRect[3] = num4;
				}
				else
				{
					offUpdateSRect[1] = -num4;
				}
				if (num5 > 0)
				{
					offUpdateSRect[0] = num5;
				}
				else
				{
					offUpdateSRect[2] = -num5;
				}
				int colorOfRGB = Graphics.GetColorOfRGB(128, 210, 255);
				if (offUpdateSRect[0] > 0)
				{
					g.SetColor(colorOfRGB);
					g.FillRect(0f, VIEW_Y, 240f, offUpdateSRect[0]);
				}
				if (offUpdateSRect[1] > 0)
				{
					g.SetColor(colorOfRGB);
					g.FillRect(VIEW_W - offUpdateSRect[1], VIEW_Y, offUpdateSRect[1], VIEW_H);
				}
				if (offUpdateSRect[2] > 0)
				{
					g.SetColor(colorOfRGB);
					g.FillRect(0f, VIEW_Y + VIEW_H - offUpdateSRect[2], 240f, offUpdateSRect[2]);
				}
				if (offUpdateSRect[3] > 0)
				{
					g.SetColor(colorOfRGB);
					g.FillRect(VIEW_X, VIEW_Y, offUpdateSRect[3], VIEW_H);
				}
				apdat_.map_.Draw_copyArea(g, offUpdateSRect);
			}
			apdat_.DrawZSortObjects(g, true);
			Seb.ClearClip();
			g.PopClip();
			VIEW_W = vIEW_W;
			VIEW_H = vIEW_H;
		}

		public void SetAllDraw()
		{
			flg_allDraw = true;
			Camera.old_pos.Set(Camera.pos);
		}

		public override void Draw(Graphics g)
		{
			VIEW_W = view_.GetGameWidth() - VIEW_X;
			VIEW_H = view_.GetGameHeight() - VIEW_Y;
			int num = AppData.Clamp(scale_, GetScaleMin(), GetScaleMax() + 40);
			if (num != scale_)
			{
				SetScale(num);
			}
			try
			{
				_draw(g);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public void _draw(Graphics g)
		{
			GameView instance = GameView.GetInstance();
			Camera.SetDrawCenter(VIEW_X, VIEW_Y, VIEW_W, VIEW_H);
			if (state_ != 7 && state_ != 8 && state_ != 6 && state_ != 4 && state_ != 5)
			{
				Draw_main(g);
			}
			int originX = g.GetOriginX();
			int originY = g.GetOriginY();
			if (state_ == 9 || state_ == 10 || state_ == 11 || state_ == 12 || state_ == 13 || state_ == 14 || state_ == 15 || state_ == 16)
			{
				int x = (instance.GetGameWidth() - 240) / 2;
				int y = (instance.GetGameHeight() - 240) / 2;
				g.SetOrigin(x, y);
				SubForm.Draw_backBlackAlpha(g);
			}
			if (state_ == 0)
			{
				if (instance.CheckFirstTouch(17))
				{
					GameView.AddTouch(g, 17, -1000, -1000, 2000, 2000, 0, TouchOption.Drag(1, 1).Flag(40));
				}
				else
				{
					GameView.AddTouch(g, 17, VIEW_X, VIEW_Y, VIEW_W, VIEW_H, 0, TouchOption.Drag(1, 1));
				}
				if (statusAmeftoCnt_ > 0)
				{
					int x2 = STATUS_AMEFTO_ANIME[1] - statusAmeftoCnt_;
					int num = GameUtil.RateConvert(x2, STATUS_AMEFTO_ANIME[0], STATUS_AMEFTO_ANIME[1], 0, 110, true);
					g.DrawImage(apdat_.resCom2_.img[91], instance.GetGameWidth() - 103 + num, 22f);
					GameView.AddTouch(g, 1, instance.GetGameWidth() - 99 + num, 33, apdat_.resCom2_.seb[24], 0, 16);
					GameView.AddTouch(g, 1, instance.GetGameWidth() - 9 + num, 33, apdat_.resCom2_.seb[24], 1, 18);
					g.SetColor(Graphics.GetColorOfRGB(245, 255, 139));
					g.DrawString(STATUS_AMEFTO_NAME[statusAmeftoMode_], instance.GetGameWidth() - 52 + num, 27f, 2);
				}
				else
				{
					GameView.AddTouch(g, 2, instance.GetGameWidth() - 120, 0, 120, 60, 65554);
				}
				if (statusPlayer_ != null)
				{
					int sideSoftWidth = instance.GetSideSoftWidth();
					PlayerData playerData = apdat_.pData_[statusPlayer_.pData_id_];
					bool flag = false;
					if (playerData.auraPoint_ == 10000)
					{
						flag = true;
					}
					if (flag && udat_.gm_.state_ == 0)
					{
						g.DrawImage(apdat_.resCom2_.img[55], 2 + sideSoftWidth, VIEW_Y + VIEW_H - 29 - statusPlayer_.frame_ % 10 / 5);
						apdat_.DrawSeb(g, 87 + sideSoftWidth, VIEW_Y + VIEW_H - 23, apdat_.resCom2_.seb[49], apdat_.resCom2_.img[53], 0);
						if (statusPlayer_.frame_ % 10 / 5 == 1)
						{
							apdat_.DrawSeb(g, 94 + sideSoftWidth, VIEW_Y + VIEW_H - 17, apdat_.resCom2_.seb[49], apdat_.resCom2_.img[53], 1);
						}
						GameView.AddTouch(g, 2, 0, instance.GetGameHeight() / 2, instance.GetGameWidth(), instance.GetGameHeight() / 2, 131072);
					}
				}
				if (stateCnt_ < 50)
				{
					if (!udat_.gm_.halfTime)
					{
						apdat_.DrawSeb(g, 77, 21, apdat_.resCom2_.seb[105], apdat_.resCom2_.img[189], 0);
					}
					else
					{
						apdat_.DrawSeb(g, 77, 21, apdat_.resCom2_.seb[105], apdat_.resCom2_.img[189], 1);
					}
				}
				if (udat_.gm_.state_ == 8)
				{
					GameView.AddTouch(2);
				}
			}
			else if (state_ == 1)
			{
				Draw_topBar(g);
			}
			else if (state_ == 6)
			{
				g.DrawScaledImage(offGameScreen_.Image, VIEW_X, VIEW_Y, VIEW_W, VIEW_H, VIEW_X, VIEW_Y, GetGameWidth(), GetGameHeight());
				g.DrawImage(apdat_.resCom2_.img[32], 0f, 20f, 0, 20, 81, 13);
				if (!udat_.gm_.halfTime)
				{
					apdat_.DrawSeb(g, 4, 20, apdat_.resCom2_.seb[48], apdat_.resCom2_.img[52], 0);
				}
				else
				{
					apdat_.DrawSeb(g, 4, 20, apdat_.resCom2_.seb[48], apdat_.resCom2_.img[52], 1);
				}
				int num2 = (int)udat_.gm_.gameTime_;
				int num3 = 41;
				int y2 = 21;
				apdat_.DrawNumImage(g, 29, num2 / 60, num3, y2, 0, 4);
				apdat_.DrawSeb(g, num3, y2, apdat_.resCom2_.seb[29], apdat_.resCom2_.img[101], 10);
				apdat_.DrawNumImage(g, 29, num2 % 60 / 10, num3 + 13, y2, 0, 4);
				apdat_.DrawNumImage(g, 29, num2 % 60 % 10, num3 + 21, y2, 0, 4);
				int x3 = (instance.GetGameWidth() - 240) / 2;
				int y3 = (instance.GetGameHeight() - 240) / 2;
				g.SetOrigin(x3, y3);
				udat_.Draw_edit(g, true);
				udat_.Draw_edit2(g, true);
				g.SetOrigin(originX, originY);
			}
			else if (state_ == 7 || state_ == 8)
			{
				g.DrawScaledImage(offGameScreen_.Image, VIEW_X, VIEW_Y, VIEW_W, VIEW_H, VIEW_X, VIEW_Y, GetGameWidth(), GetGameHeight());
				g.DrawImage(apdat_.resCom2_.img[32], 0f, 20f, 0, 20, 81, 13);
				if (!udat_.gm_.halfTime)
				{
					apdat_.DrawSeb(g, 4, 20, apdat_.resCom2_.seb[48], apdat_.resCom2_.img[52], 0);
				}
				else
				{
					apdat_.DrawSeb(g, 4, 20, apdat_.resCom2_.seb[48], apdat_.resCom2_.img[52], 1);
				}
				int num4 = (int)udat_.gm_.gameTime_;
				int num5 = 41;
				int y4 = 21;
				apdat_.DrawNumImage(g, 29, num4 / 60, num5, y4, 0, 4);
				apdat_.DrawSeb(g, num5, y4, apdat_.resCom2_.seb[29], apdat_.resCom2_.img[101], 10);
				apdat_.DrawNumImage(g, 29, num4 % 60 / 10, num5 + 13, y4, 0, 4);
				apdat_.DrawNumImage(g, 29, num4 % 60 % 10, num5 + 21, y4, 0, 4);
				Draw_stadiumCamera(g);
				if (state_ == 8)
				{
					int num6 = stateCnt_;
					int num7 = GameUtil.RateConvert(stateCnt_, 0, 17, 0, 255, true);
					if (num7 >= 0 && num6 >= 0)
					{
						int num8 = VIEW_Y - 3;
						g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, num7));
						g.FillRect(0f, num8, instance.GetGameWidth(), instance.GetGameHeight() - num8);
						g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
					}
				}
				GameView.AddTouch(2, TouchOption.Create(2));
			}
			else if (state_ == 9)
			{
				Draw_bonusFan(g);
			}
			else if (state_ == 10 || state_ == 11)
			{
				Draw_bonusIncome(g);
			}
			else if (state_ == 12)
			{
				Draw_matchElse(g);
			}
			else if (state_ != 13)
			{
				if (state_ == 14)
				{
					Draw_newPlayerLv(g);
				}
				else if (state_ == 15)
				{
					Draw_newPlayerPopular(g);
				}
				else if (state_ == 16)
				{
					Draw_newPlayerReserach(g);
				}
			}
			if (state_ == 9 || state_ == 10 || state_ == 11 || state_ == 12 || state_ == 13 || state_ == 14 || state_ == 15 || state_ == 16)
			{
				g.SetOrigin(originX, originY);
			}
			UpdateOffscreenTopBar(g, true);
		}

		public void Draw_main(Graphics g)
		{
			long num = JSystem.CurrentTimeMillis();
			g.DrawScaledImage(offGameScreen_.Image, VIEW_X, VIEW_Y, VIEW_W, VIEW_H, VIEW_X, VIEW_Y, GetGameWidth(), GetGameHeight());
			if (apdat_.save_.ints[12] == 1)
			{
				apdat_.map_.Draw_miniMap(g);
			}
			if (statusPlayer_ != null)
			{
				int sideSoftWidth = view_.GetSideSoftWidth();
				PlayerData playerData = apdat_.pData_[statusPlayer_.pData_id_];
				if (MyFormBase.Jp())
				{
					g.DrawImage(apdat_.resCom2_.img[56], 0 + sideSoftWidth, VIEW_Y + VIEW_H - 20);
				}
				else
				{
					int num2 = 15;
					g.DrawImage(apdat_.resCom2_.img[56], 0 + sideSoftWidth, VIEW_Y + VIEW_H - 20, 0, 0, 80, 20);
					g.DrawImage(apdat_.resCom2_.img[56], 0 + sideSoftWidth + 80, VIEW_Y + VIEW_H - 20, 102 - (22 + num2), 0, 22 + num2, 20);
				}
				g.ClipRect(0 + sideSoftWidth, VIEW_Y + VIEW_H - 20, 20f, 19f);
				udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, 0, 1);
				udat_.DrawDispPlayer(g, 10 + sideSoftWidth, VIEW_Y + VIEW_H + 3);
				g.PopClip();
				int num3 = GameUtil.RateConvert(playerData.auraPoint_, 0, 10000, 0, 17, true);
				g.DrawImage(apdat_.resCom2_.img[54], 18 + sideSoftWidth, VIEW_Y + VIEW_H - 1 - num3, 0, 17 - num3, 2, num3);
				apdat_.DrawSeb(g, 23 + sideSoftWidth, VIEW_Y + VIEW_H - 15, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], statusPlayer_.role_);
				PlayerData playerData2 = apdat_.pData_[statusPlayer_.pData_id_];
				g.SetColor(Graphics.GetColorOfRGB(246, 255, 225));
				g.DrawString(playerData2.name_, 47 + sideSoftWidth, VIEW_Y + VIEW_H - 15);
			}
			udat_.gm_.Draw(g);
			int num4 = 21;
			g.DrawImage(apdat_.resCom2_.img[32], 0f, num4, 0, 20, 81, 13);
			if (!udat_.gm_.halfTime)
			{
				apdat_.DrawSeb(g, 4, num4, apdat_.resCom2_.seb[48], apdat_.resCom2_.img[52], 0);
			}
			else
			{
				apdat_.DrawSeb(g, 4, num4, apdat_.resCom2_.seb[48], apdat_.resCom2_.img[52], 1);
			}
			int num5 = (int)udat_.gm_.gameTime_;
			int num6 = 41;
			apdat_.DrawNumImage(g, 29, num5 / 60, num6, num4 + 1, 0, 4);
			apdat_.DrawSeb(g, num6, num4 + 1, apdat_.resCom2_.seb[29], apdat_.resCom2_.img[101], 10);
			apdat_.DrawNumImage(g, 29, num5 % 60 / 10, num6 + 13, num4 + 1, 0, 4);
			apdat_.DrawNumImage(g, 29, num5 % 60 % 10, num6 + 21, num4 + 1, 0, 4);
			apdat_.camera.DrawTest(g);
			draw_time = JSystem.CurrentTimeMillis() - num;
			if (Config.DEBUG_CMD && UserData.DEBUG_DISPFPS == 1)
			{
				apdat_.DrawNumImage(g, 23, string.Empty, FormManager.GetInstance().GetFps(), string.Empty, 5, 25, 0);
			}
			if (Config.DEBUG_CMD && SHOW_DEBUGDRAW2 == 1)
			{
				if (SHOW_FPS == 1)
				{
					apdat_.DrawNumImage(g, 23, string.Empty, FormManager.GetInstance().GetFps(), string.Empty, 5, 25, 0);
				}
				if (SHOW_PDTIME == 1)
				{
					apdat_.DrawNumImage(g, 23, string.Empty, proc_time, string.Empty, 5, 40, 0);
					apdat_.DrawNumImage(g, 23, string.Empty, draw_time, string.Empty, 5, 55, 0);
				}
			}
			if ((!(Config.DEBUG_CMD)) || (SHOW_DRAWNUM == 1)) { return; }

		}

		public void SetStatusPlayer()
		{
			if (statusPlayerChangeWait_ > 0)
			{
				statusPlayerChangeWait_--;
				return;
			}
			statusPlayer_ = null;
			if (udat_.gm_.state_ == 8)
			{
				if (!udat_.gm_.halfTime && udat_.gm_.gameState_ == 1)
				{
					statusPlayer_ = udat_.teams_[0].players_[udat_.teams_[0].lastKickeer_];
				}
				else if (udat_.gm_.halfTime && udat_.gm_.gameState_ == 0)
				{
					statusPlayer_ = udat_.teams_[1].players_[udat_.teams_[1].lastKickeer_];
				}
				return;
			}
			if (!udat_.gm_.halfTime)
			{
				if (udat_.gm_.gameState_ == 0)
				{
					statusPlayer_ = udat_.teams_[0].GetPasser();
				}
				else if (udat_.gm_.gameState_ == 1)
				{
					for (int i = 0; i < udat_.teams_[0].players_.Length; i++)
					{
						if (udat_.teams_[0].players_[i].actId_ == 2)
						{
							statusPlayer_ = udat_.teams_[0].players_[i];
							break;
						}
					}
				}
				else if (udat_.teams_[0].players_receive_.Size() > 0)
				{
					statusPlayer_ = (Player)udat_.teams_[0].players_receive_.FirstElement();
				}
			}
			else if (udat_.gm_.gameState_ == 0)
			{
				for (int j = 0; j < udat_.teams_[1].players_.Length; j++)
				{
					if (udat_.teams_[1].players_[j].actId_ == 2)
					{
						statusPlayer_ = udat_.teams_[1].players_[j];
						break;
					}
				}
			}
			else if (udat_.gm_.gameState_ == 1)
			{
				statusPlayer_ = udat_.teams_[1].GetPasser();
			}
			else if (udat_.teams_[1].players_receive_.Size() > 0)
			{
				statusPlayer_ = (Player)udat_.teams_[1].players_receive_.FirstElement();
			}
			if (statusPlayer_ != null)
			{
				statusPlayerChangeWait_ = 5;
			}
			if (statusPlayer_ != null)
			{
				int pData_id_ = statusPlayer_.pData_id_;
				if (pData_id_ < 22 || pData_id_ >= apdat_.pData_.Length)
				{
					statusPlayer_ = null;
				}
			}
		}

		public void Draw_stadiumCamera(Graphics g)
		{
			GameView instance = GameView.GetInstance();
			int num3;
			int num2;
			int num = (num2 = (num3 = 0));
			int num4 = instance.GetGameWidth() / 2 - 87;
			num = instance.GetGameHeight() - 78;
			g.SetColor(Graphics.GetColorOfRGB(46, 168, 59));
			g.FillRect(num4, num, 174f, 72f);
			TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[0]];
			TeamData teamData2 = apdat_.tData_[udat_.gm_.teamId_[1]];
			num3 = 39;
			apdat_.frmGame_.Draw_stadiumName(g, num3, instance.GetGameWidth() / 2);
			g.DrawImage(apdat_.resCom2_.img[159], num4 + 5 - 1, num + 3 - 1, 0, 0, 166, 12);
			if (stadiumMoveCnt_ < STADIUM_BTMINFO_TIME[2])
			{
				g.DrawImage(apdat_.resCom2_.img[159], num4 + 5, num + 3, 1, 12, 82, 10);
			}
			else
			{
				int c2min = 82;
				int num5 = 0;
				int num6 = 0;
				num5 = GameUtil.RateConvert(customerNum[0], 0, customerNum[0] + customerNum[1], 0, 164, true);
				num6 = GameUtil.RateConvert(stadiumMoveCnt_, STADIUM_BTMINFO_TIME[2], STADIUM_BTMINFO_TIME[3], c2min, num5, true);
				g.DrawImage(apdat_.resCom2_.img[159], num4 + 5, num + 3, 1, 12, num6, 10);
			}
			g.SetColor(Graphics.GetColorOfRGB(251, 251, 97));
			g.DrawString2("観客数", instance.GetGameWidth() / 2, num + 17, 2);
			g.DrawImage(apdat_.resCom2_.img[161], instance.GetGameWidth() / 2 + 50, num + 15);
			int num7 = stateCnt_ % 10 / 5;
			if (stadiumMoveCnt_ < STADIUM_BTMINFO_TIME[1] || customerNum[0] < customerNum[1])
			{
				num7 = 0;
			}
			for (int i = 0; i < 2; i++)
			{
				if (num7 == 0)
				{
					udat_.SetDispPlayerData(0, teamData.bodyImg_, teamData.memberFaceId_[i], num7, 1);
				}
				else
				{
					udat_.SetDispPlayerData(4, teamData.bodyImg_, teamData.memberFaceId_[i], 0, 1);
				}
				udat_.DrawDispPlayer(g, instance.GetGameWidth() / 2 - 54 + i * 20, num + 52);
			}
			num7 = stadiumMoveCnt_ % 10 / 5;
			if (stadiumMoveCnt_ < STADIUM_BTMINFO_TIME[1] || customerNum[1] < customerNum[0])
			{
				num7 = 0;
			}
			for (int j = 0; j < 2; j++)
			{
				if (num7 == 0)
				{
					udat_.SetDispPlayerData(0, teamData2.bodyImg_, teamData2.memberFaceId_[j], num7, 2);
				}
				else
				{
					udat_.SetDispPlayerData(4, teamData2.bodyImg_, teamData2.memberFaceId_[j], 0, 2);
				}
				udat_.DrawDispPlayer(g, instance.GetGameWidth() / 2 + 23 + j * 20, num + 52);
			}
			if (stadiumMoveCnt_ >= STADIUM_BTMINFO_TIME[1] && customerNum[0] >= customerNum[1])
			{
				g.DrawImage(apdat_.resCom2_.img[160], instance.GetGameWidth() / 2 - 66, num + 20);
			}
			num3 = num + 58;
			if (stadiumMoveCnt_ < STADIUM_BTMINFO_TIME[0])
			{
				num2 = instance.GetGameWidth() / 2 - 45;
				apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 15);
				num2 = instance.GetGameWidth() / 2 + 41;
				apdat_.DrawSeb(g, num2, num3, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 15);
			}
			else if (stadiumMoveCnt_ < STADIUM_BTMINFO_TIME[1])
			{
				int num8 = GameUtil.Random(1000000);
				num2 = instance.GetGameWidth() / 2 - 8;
				apdat_.DrawNumImageComma(g, 78, num8, num2 - 12, num3, 0, 0, 4);
				num8 = GameUtil.Random(1000000);
				num2 = instance.GetGameWidth() / 2 + 74;
				apdat_.DrawNumImageComma(g, 78, num8, num2 - 12, num3, 0, 0, 4);
			}
			else
			{
				num2 = instance.GetGameWidth() / 2 - 8;
				apdat_.DrawNumImageComma(g, 78, customerNum[0], num2 - 12, num3, 0, 0, 4);
				num2 = instance.GetGameWidth() / 2 + 74;
				apdat_.DrawNumImageComma(g, 78, customerNum[1], num2 - 12, num3, 0, 0, 4);
			}
			if (stadiumMoveCnt_ >= STADIUM_BTMINFO_TIME[0])
			{
				num2 = instance.GetGameWidth() / 2 - 8;
				g.SetColor(SubForm.SC_WINDOW_BLACK);
				apdat_.DrawSeb(g, num2 - 11, num3, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 18);
				num2 = instance.GetGameWidth() / 2 + 74;
				g.SetColor(SubForm.SC_WINDOW_BLACK);
				apdat_.DrawSeb(g, num2 - 11, num3, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 18);
			}
		}

		public void Draw_bonusFan(Graphics g)
		{
			if (bonus_f_index >= FanBonusData.bonus_f_data1.Size())
			{
				return;
			}
			object[] array = (object[])FanBonusData.bonus_f_data1.ElementAt(bonus_f_index);
			string str = (string)array[0];
			int num = ((JInteger)array[1]).IntValue();
			int[] array2 = (int[])array[2];
			int num2 = ((JInteger)array[3]).IntValue();
			int[] array3 = (int[])array[4];
			int num3 = ((JInteger)array[5]).IntValue();
			int num5;
			int num6;
			int num7;
			int num8;
			int num9;
			int num12;
			int num11;
			int num10;
			int num4 = (num5 = (num6 = (num7 = (num8 = (num9 = (num10 = (num11 = (num12 = 0))))))));
			int num13 = 20;
			num4 = 72;
			int num14 = 43;
			int num15 = 7;
			int num16 = 120;
			int num17 = 232;
			int num18 = 0;
			int frame = 0;
			g.SetColor(Graphics.GetColorOfRGB(162, 204, 153, 128));
			g.FillRect(5f, 34f, 231f, 199f);
			g.SetColor(Graphics.GetColorOfRGB(106, 129, 78, 128));
			g.DrawRect(7f, 36f, 226f, 194f);
			g.SetColor(Graphics.GetColorOfRGB(108, 158, 107, 128));
			g.FillRect(8f, 37f, 225f, 193f);
			g.SetColor(Graphics.GetColorOfRGB(178, 236, 179, 128));
			g.FillRect(10f, 94f, 220f, 80f);
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
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
			num12 = 43;
			g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
			g.DrawString("試合ボーナス", 120f, num12, 2);
			num12 += 17;
			g.DrawString(teamData.name_, 60f, num12, 2);
			g.DrawString(teamData2.name_, 180f, num12, 2);
			num12 += 17;
			apdat_.DrawNumImage(g, 30, udat_.gm_.score_[0], 60, num12, 0, 2);
			apdat_.DrawNumImage(g, 30, udat_.gm_.score_[1], 180, num12, 0, 2);
			apdat_.DrawSeb(g, 116, num12, apdat_.resCom2_.seb[35], apdat_.resCom2_.img[103], 15);
			num13 = 120;
			num4 = 97;
			int num19 = num;
			g.SetColor(Graphics.GetColorOfRGB(223, 223, 220));
			g.DrawRect(num13 - num19 / 2, num4, num19 - 1, 19f);
			g.SetColor(Graphics.GetColorOfRGB(153, 152, 145));
			g.DrawRect(num13 - num19 / 2 + 1, num4 + 1, num19 - 1 - 2, 17f);
			g.SetColor(Graphics.GetColorOfRGB(224, 255, 255));
			g.FillRect(num13 - num19 / 2 + 2, num4 + 2, num19 - 1 - 3, 16f);
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			g.DrawString(str, 120f, num4 + 4, 2);
			if (num3 == 1)
			{
				g.DrawImage(apdat_.resCom2_.img[177], 88f, 119f);
			}
			g.ClipRect(14f, 84f, 211f, 94f);
			int num20 = array2[3];
			int num21 = GameUtil.RateConvert(num20, 3, 7, 5, 1, true);
			int num22 = num20 * 14 + (num20 - 1) * num21;
			int num23 = 0;
			num4 = 163;
			if (bonus_f_cnt < BONUS_F_CYCLE[0])
			{
				num5 = GameUtil.RateConvert(bonus_f_cnt, 0, BONUS_F_CYCLE[0], num15 - num22 / 2, num16, true);
				num6 = num4;
				num18 = 10;
				frame = bonus_f_cnt % 16 / 4;
			}
			else if (bonus_f_cnt < BONUS_F_CYCLE[1])
			{
				num5 = num16;
				num6 = num4;
				num18 = 0;
				frame = 0;
			}
			else if (bonus_f_cnt < BONUS_F_CYCLE[2])
			{
				num5 = num16;
				switch (array2[4])
				{
				case 0:
				{
					int num25 = 14;
					int num26 = 6;
					int num27 = (bonus_f_cnt - BONUS_F_CYCLE[1]) % (num25 + num26);
					num27 -= num26 / 2;
					float num28 = GameUtil.SetVStop(-10f, num25 / 2);
					float num29 = GameUtil.SetAStop(-10f, num25 / 2);
					num6 = (int)((float)num4 + num28 * (float)num27 + num29 * (float)num27 * (float)(num27 + 1) / 2f);
					if (num6 > num4)
					{
						num6 = num4;
					}
					num18 = ((num6 < num4) ? 3 : 0);
					frame = 0;
					break;
				}
				case 1:
					num6 = num4;
					num18 = 11;
					frame = 0;
					num23 = -1;
					break;
				case 2:
				{
					num6 = num4;
					num18 = 1;
					int num24 = (bonus_f_cnt - BONUS_F_CYCLE[1]) % Player.SEB_H_KICK_ANIME[Player.SEB_H_KICK_ANIME.Length - 1];
					if (num24 < Player.SEB_H_KICK_ANIME[0])
					{
						frame = 0;
					}
					else if (num24 < Player.SEB_H_KICK_ANIME[1])
					{
						frame = 1;
					}
					else if (num24 < Player.SEB_H_KICK_ANIME[2])
					{
						frame = 2;
					}
					break;
				}
				}
			}
			else if (bonus_f_cnt < BONUS_F_CYCLE[3])
			{
				num5 = num16;
				num6 = num4;
				num18 = 0;
				frame = 0;
			}
			else if (bonus_f_cnt < BONUS_F_CYCLE[4])
			{
				num5 = GameUtil.RateConvert(bonus_f_cnt, BONUS_F_CYCLE[3], BONUS_F_CYCLE[4], num16, num17 + num22 / 2, true);
				num6 = num4;
				num18 = 10;
				frame = bonus_f_cnt % 16 / 4;
			}
			int faceImg = 0;
			int bodyImg = 0;
			int num30 = num18;
			for (int k = 0; k < num20; k++)
			{
				num30 = num18;
				switch (num2)
				{
				case 0:
				case 1:
				{
					PlayerData playerData = apdat_.pData_[array3[k]];
					faceImg = playerData.faceImg_;
					bodyImg = udat_.selectUniform_;
					break;
				}
				case 2:
				{
					SupervisorData supervisorData = apdat_.svisorData_[udat_.selectSupervisorId_];
					faceImg = supervisorData.headImg_;
					bodyImg = supervisorData.bodyImg_;
					switch (num18)
					{
					case 10:
						num30 = 0;
						break;
					case 3:
						num30 = 16;
						break;
					}
					break;
				}
				}
				apdat_.DrawSeb(g, num5 - num22 / 2 + 7 + k * (14 + num21) - 1, num4 + 1 + num23, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 4);
				udat_.SetDispPlayerData(num30, bodyImg, faceImg, frame, 1);
				udat_.DrawDispPlayer(g, num5 - num22 / 2 + 7 + k * (14 + num21), num6);
			}
			g.PopClip();
			num4 = 174;
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			num13 = 18;
			for (int l = 0; l < FanActData.element_name2.Length; l++)
			{
				num12 = num4 + 4 + l * 17;
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				if (!MyFormBase.En())
				{
					g.DrawString(FanActData.element_name[l], num13 + 18, num12 + 2);
				}
				else
				{
					g.DrawString(FanActData.element_name[l], num13 + 18 - 12, num12 + 2);
				}
				g.SetColor(Graphics.GetColorOfRGB(37, 193, 173));
				g.FillRect(num13 + 64, num12, 87f, 14f);
				int num31 = 0;
				int num32 = 0;
				num11 = 141;
				switch (l)
				{
				case 0:
				{
					num31 = FanActData.GetFanDispParam(temp_fan, 0);
					num32 = FanActData.GetFanDispParam(temp_fan + temp_addFan, 0);
					int num34 = 0;
					num34 = ((bonus_f_cnt < BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[1]) ? num31 : ((bonus_f_cnt < BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[1] || bonus_f_cnt >= BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[2]) ? num32 : GameUtil.RateConvert(bonus_f_cnt - BONUS_F_CYCLE[0], BONUS_F_CYCLE_MAIN[1], BONUS_F_CYCLE_MAIN[2], num31, num32, true)));
					if (MyFormBase.Jp())
					{
						apdat_.DrawNumImageComma(g, 78, num34, num11 + 14 - 2, num12 + 2, 0, 0, 4);
						apdat_.DrawSeb(g, num11 + 15, num12 + 3, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 18);
					}
					else
					{
						apdat_.DrawNumImageComma(g, 78, num34, num11 + 14 - 2 + 10, num12 + 2, 0, 0, 4);
						apdat_.DrawSeb(g, num11 + 15 + 10, num12 + 3, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 18);
					}
					break;
				}
				case 1:
				{
					num31 = FanActData.GetFanDispParam(temp_assist, 1);
					num32 = FanActData.GetFanDispParam(temp_assist + temp_addAssist, 1);
					int num35 = 52;
					int num36 = 0;
					int num37 = 0;
					num37 = ((bonus_f_cnt < BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[1]) ? num31 : ((bonus_f_cnt < BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[1] || bonus_f_cnt >= BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[2]) ? num32 : GameUtil.RateConvert(bonus_f_cnt - BONUS_F_CYCLE[0], BONUS_F_CYCLE_MAIN[1], BONUS_F_CYCLE_MAIN[2], num31, num32, true)));
					apdat_.DrawNumImage(g, 78, num37, num11 + 22, num12 + 2, 0, 4);
					num36 = GameUtil.RateConvert(num37, 0, 100, 0, num35 - 2, true);
					g.DrawImage(apdat_.resCom2_.img[129], num11 + 48 - 101, num12 + 4 - 2, 0, 0, 52, 10);
					g.DrawImage(apdat_.resCom2_.img[129], num11 + 48 + 1 - 101, num12 + 5 - 2, 1, 10, num36, 8);
					break;
				}
				case 2:
				{
					num31 = FanActData.GetFanDispParam(temp_nameValue, 2);
					num32 = FanActData.GetFanDispParam(temp_nameValue + temp_addNameValue, 2);
					int num33 = 0;
					num33 = ((bonus_f_cnt < BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[1]) ? num31 : ((bonus_f_cnt < BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[1] || bonus_f_cnt >= BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[2]) ? num32 : GameUtil.RateConvert(bonus_f_cnt - BONUS_F_CYCLE[0], BONUS_F_CYCLE_MAIN[1], BONUS_F_CYCLE_MAIN[2], num31, num32, true)));
					if (MyFormBase.Jp())
					{
						apdat_.DrawNumImage(g, 78, num33, 123, num12 + 2, 0, 2);
					}
					else
					{
						apdat_.DrawNumImage(g, 78, num33, 163, num12 + 2, 0, 4);
					}
					break;
				}
				}
				if (bonus_f_cnt < BONUS_F_CYCLE[0])
				{
					continue;
				}
				int num38 = num32 - num31;
				if (num38 == 0)
				{
					continue;
				}
				if (bonus_f_cnt < BONUS_F_CYCLE[0] + BONUS_F_CYCLE_MAIN[0])
				{
					int num39 = 12;
					int num40 = 10 - 5 * l;
					int num41 = bonus_f_cnt - BONUS_F_CYCLE[0] - (BONUS_F_CYCLE_MAIN[0] - num40 - num39);
					if (num41 >= 0)
					{
						float num42 = GameUtil.SetVStop(-6f, num39 / 2);
						float num43 = GameUtil.SetAStop(-6f, num39 / 2);
						num10 = (int)((float)(num12 + 2) + num42 * (float)num41 + num43 * (float)num41 * (float)(num41 + 1) / 2f);
						if (num10 >= num12 + 2)
						{
							num10 = num12 + 2;
						}
						SubForm.Draw_upParam(g, 35, num13 + 201, num10, num38);
					}
				}
				else
				{
					SubForm.Draw_upParam(g, 35, num13 + 201, num12 + 2, num38);
				}
			}
			GameView.AddTouch(2);
		}

		public void Draw_bonusIncome(Graphics g)
		{
			apdat_.DrawWindow(g, 228, 204, 0, "収支情報");
			apdat_.DrawBox(g, 15, 55, 225, 201);
			int num5;
int num;
int num2;
int num3;


			num5 = num = num2 = num3 = 0;
			Font font = g.GetFont(null);
			string text = "観客数";
			int num4 = font.StringWidth(text) + 10 + AppData.GetFig(customerNum[0]) * 8 + 10 + 8;
			if (MyFormBase.En())
			{
				num4 -= 18;
			}
			num = 51;
			g.SetColor(SubForm.SC_WINDOW_BLUE);
			g.DrawString(text, 120 - num4 / 2, num);
			num2 = 158;
			num3 = num;
			apdat_.DrawNumImageComma(g, 78, customerNum[0], 120 - num4 / 2 + (font.StringWidth(text) + 10), num3 + 1, 0, 0, 1);
			apdat_.DrawSeb(g, 120 - num4 / 2 + 46 + (AppData.GetFig(customerNum[0]) * 8 + 10), num3 + 2, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[105], 18);
			num5 = 20;
			num = 72;
			g.SetColor(SubForm.SC_WINDOW_BLUE);
			g.DrawString("項目一覧", 42f, num);
			g.DrawString("収支", 191f, num, 2);
			g.SetColor(Graphics.GetColorOfRGB(146, 184, 247));
			g.DrawLine(num5, num + 12 + 2, num5 + 210 - 12, num + 12 + 2);
			num5 = 20;
			num = 95;
			string[] array = new string[5] { "チケット", "賞金", "ｸﾞｯｽﾞ販売", "ｽﾎﾟﾝｻｰ", "選手給料" };
			int num6 = 216;
			string empty = string.Empty;
			for (int i = 0; i < 5; i++)
			{
				num3 = num + 18 * i;
				empty = MyFormBase.LT(array[i]);
				if (i == 3)
				{
					empty += MyFormBase.LT("(<0>社)", bonus_sponsorNum);
				}
				g.SetColor(SubForm.SC_WINDOW_BLACK);
				g.DrawString(empty, num5, num3);
				if (i == 0)
				{
					if (!MyFormBase.En())
					{
						SubForm.Draw_multiParam(g, 35, num5 + 51, num3 + 1, udat_.ticketPrice_, 1);
						int num7 = AppData.GetFig(udat_.ticketPrice_) * 8 + 8 + 5;
						apdat_.DrawSeb(g, num5 + 51 + num7, num3 + 1, apdat_.resCom2_.seb[35], apdat_.resCom2_.img[103], 12);
					}
					else
					{
						apdat_.DrawSeb(g, num5 + 51, num3 + 1, apdat_.resCom2_.seb[35], apdat_.resCom2_.img[103], 16);
						apdat_.DrawMoney(g, 35, udat_.ticketPrice_ / 100, num5 + 51 + 10 + 4, num3 + 1, 0, 0, 1, false);
					}
				}
				if (i != 4)
				{
					g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
					g.DrawLine(num5, num3 + 12 + 2, num5 + 210 - 12, num3 + 12 + 2);
				}
				g.SetColor(SubForm.SC_WINDOW_BLACK);
				if (i == 0)
				{
					if (bonus_in_cnt < BONUS_IN_CYCLE[i])
					{
						int num8 = GameUtil.RateConvert(bonus_in_cnt, 0, BONUS_IN_CYCLE[i], 0, bonus_in_param[i][0], true);
						int num9 = GameUtil.RateConvert(bonus_in_cnt, 0, BONUS_IN_CYCLE[i], 0, bonus_in_param[i][1], true);
						g.SetColor(SubForm.SC_WINDOW_BLUE);
						g.DrawString(MyFormBase.YEN("万", num8), num6, num3, 4);
					}
					else
					{
						g.SetColor(SubForm.SC_WINDOW_BLUE);
						g.DrawString(MyFormBase.YEN("万", bonus_in_param[i][0]), num6, num3, 4);
					}
				}
				else if (bonus_in_cnt < BONUS_IN_CYCLE[i - 1])
				{
					if (i == 4)
					{
						g.SetColor(SubForm.SC_WINDOW_RED);
					}
					else
					{
						g.SetColor(SubForm.SC_WINDOW_BLUE);
					}
					g.DrawString(MyFormBase.YEN("万", 0L), num6, num3, 4);
				}
				else if (bonus_in_cnt >= BONUS_IN_CYCLE[i - 1] && bonus_in_cnt < BONUS_IN_CYCLE[i])
				{
					int num10 = GameUtil.RateConvert(bonus_in_cnt, BONUS_IN_CYCLE[i - 1], BONUS_IN_CYCLE[i], 0, bonus_in_param[i][0], true);
					int num11 = GameUtil.RateConvert(bonus_in_cnt, BONUS_IN_CYCLE[i - 1], BONUS_IN_CYCLE[i], 0, bonus_in_param[i][1], true);
					if (i == 4)
					{
						g.SetColor(SubForm.SC_WINDOW_RED);
						g.DrawString(MyFormBase.YEN("万", num11), num6, num3, 4);
					}
					else
					{
						g.SetColor(SubForm.SC_WINDOW_BLUE);
						g.DrawString(MyFormBase.YEN("万", num10), num6, num3, 4);
					}
				}
				else if (i == 4)
				{
					g.SetColor(SubForm.SC_WINDOW_RED);
					g.DrawString(MyFormBase.YEN("万", bonus_in_param[i][1]), num6, num3, 4);
				}
				else
				{
					g.SetColor(SubForm.SC_WINDOW_BLUE);
					g.DrawString(MyFormBase.YEN("万", bonus_in_param[i][0]), num6, num3, 4);
				}
			}
			num5 = 20;
			num = 189;
			g.SetColor(Graphics.GetColorOfRGB(146, 184, 247));
			g.DrawLine(num5, num - 4, num5 + 210 - 12, num - 4);
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			g.DrawString("合計", num5, num);
			int num12 = 0;
			for (int j = 0; j < BONUS_IN_CYCLE.Length; j++)
			{
				if (bonus_in_cnt < BONUS_IN_CYCLE[j] || j == BONUS_IN_CYCLE.Length - 1)
				{
					num12 = j;
					break;
				}
			}
			int num13 = 0;
			for (int k = 0; k < num12; k++)
			{
				if (num12 >= bonus_in_param.Length)
				{
					break;
				}
				num13 += bonus_in_param[k][0];
				num13 -= bonus_in_param[k][1];
			}
			if (num13 >= 0)
			{
				g.SetColor(SubForm.SC_WINDOW_BLUE);
			}
			else
			{
				g.SetColor(SubForm.SC_WINDOW_RED);
			}
			g.DrawString(MyFormBase.YEN("万", num13), num6, num, 4);
			if (state_ == 11)
			{
				num5 = 120;
				num = 214;
				if (select_ == 0)
				{
					g.SetColor(SubForm.SC_WINDOW_SELECT);
					g.FillRect(num5 - 20, num, 40f, 16f);
				}
				g.SetColor(SubForm.SC_WINDOW_BLACK);
				g.DrawString("次へ", num5, num + 2, 2);
				if (select_ == 0 && state_ == 11)
				{
					apdat_.resCom2_.seb[60].Draw(g, num5 - 20 - 4, num + 9, apdat_.resCom2_.img);
				}
			}
			GameView.AddTouch(2);
		}

		private void Draw_topBar(Graphics g)
		{
			TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[0]];
			TeamData teamData2 = apdat_.tData_[udat_.gm_.teamId_[1]];
			int num = 4;
			int drawTBAROff = GetDrawTBAROff(2);
			int drawTBAROff2 = GetDrawTBAROff(4);
			if (state_ == 0 || state_ == 1 || state_ == 7 || state_ == 8)
			{
				g.SetColor(Graphics.GetColorOfRGB(214, 241, 255));
				g.DrawString(teamData.name_, drawTBAROff, num, 2);
				g.DrawString(teamData2.name_, drawTBAROff2, num, 2);
			}
			else if (udat_.phase_ == 14 || udat_.phase_ == 15)
			{
				g.SetColor(Graphics.GetColorOfRGB(128, 128, 128));
				g.DrawString(teamData.name_, drawTBAROff, num, 2);
				g.SetColor(Graphics.GetColorOfRGB(214, 241, 255));
				g.DrawString(teamData2.name_, drawTBAROff2, num, 2);
			}
			else
			{
				g.SetColor(Graphics.GetColorOfRGB(214, 241, 255));
				g.DrawString(teamData.name_, drawTBAROff, num, 2);
				g.SetColor(Graphics.GetColorOfRGB(128, 128, 128));
				g.DrawString(teamData2.name_, drawTBAROff2, num, 2);
			}
			UpdateOffscreenTopBar(g, false);
		}

		public void Draw_matchElse(Graphics g)
		{
			apdat_.DrawWindow(g, 228, 204, 0, "試合結果");
			apdat_.DrawBox(g, 15, 55, 225, 201);
			int num8;
int num;
int num2;
int num3;


			num8 = num = num2 = num3 = 0;
			int[] nextBattleLeagueIndex = udat_.GetNextBattleLeagueIndex(udat_.league_setu_);
			if (nextBattleLeagueIndex == null)
			{
				num = 50;
				g.SetColor(SubForm.SC_WINDOW_BLACK);
				g.DrawString("終節", 51f, num, 2);
				return;
			}
			num = 51;
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			g.DrawString(MyFormBase.LT("第<0>節", udat_.league_setu_ + 1), 120f, num, 2);
			num = 72;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < udat_.league_fightCombi_.Length; i++)
			{
				flag = false;
				flag2 = false;
				if (udat_.league_fightResult_[i][0] > udat_.league_fightResult_[i][1])
				{
					flag = true;
				}
				if (udat_.league_fightResult_[i][0] < udat_.league_fightResult_[i][1])
				{
					flag2 = true;
				}
				num3 = num + 20 * i;
				LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
				int num4 = udat_.league_fightCombi_[i][0];
				int num5 = udat_.league_fightCombi_[i][1];
				int num6 = leagueData.matchEnemy_[num4];
				int num7 = leagueData.matchEnemy_[num5];
				TeamData teamData = apdat_.tData_[num6];
				TeamData teamData2 = apdat_.tData_[num7];
				apdat_.DrawSeb(g, 20, num3, apdat_.resCom2_.seb[28], apdat_.resCom2_.img[33], teamData.uniform_);
				apdat_.DrawSeb(g, 209, num3, apdat_.resCom2_.seb[28], apdat_.resCom2_.img[33], teamData2.uniform_);
				if (num6 == 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(210, 255, 158));
					g.FillRect(32f, num3 - 2, 60f, 16f);
					g.SetColor(Graphics.GetColorOfRGB(67, 72, 255));
				}
				else if (flag)
				{
					g.SetColor(Graphics.GetColorOfRGB(92, 51, 31));
				}
				else
				{
					g.SetColor(Graphics.GetColorOfRGB(144, 144, 144));
				}
				g.DrawString(teamData.name2_, 62f, num3, 2);
				if (num7 == 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(210, 255, 158));
					g.FillRect(148f, num3 - 2, 60f, 16f);
					g.SetColor(Graphics.GetColorOfRGB(67, 72, 255));
				}
				else if (flag2)
				{
					g.SetColor(Graphics.GetColorOfRGB(92, 51, 31));
				}
				else
				{
					g.SetColor(Graphics.GetColorOfRGB(144, 144, 144));
				}
				g.DrawString(teamData2.name2_, 178f, num3, 2);
				if (flag)
				{
					apdat_.DrawNumImage(g, 35, udat_.league_fightResult_[i][0], 106, num3 + 2, 0, 2);
				}
				else
				{
					apdat_.DrawNumImage(g, 78, udat_.league_fightResult_[i][0], 106, num3 + 2, 0, 2);
				}
				if (flag2)
				{
					apdat_.DrawNumImage(g, 35, udat_.league_fightResult_[i][1], 135, num3 + 2, 0, 2);
				}
				else
				{
					apdat_.DrawNumImage(g, 78, udat_.league_fightResult_[i][1], 135, num3 + 2, 0, 2);
				}
				apdat_.DrawSeb(g, 116, num3 + 2, apdat_.resCom2_.seb[78], apdat_.resCom2_.img[103], 15);
			}
			num8 = 120;
			num = 214;
			if (select_ == 0)
			{
				g.SetColor(SubForm.SC_WINDOW_SELECT);
				g.FillRect(num8 - 20, num, 40f, 16f);
			}
			GameView.AddTouch(2);
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			g.DrawString("次へ", num8, num + 2, 2);
			if (select_ == 0)
			{
				apdat_.resCom2_.seb[60].Draw(g, num8 - 20 - 4, num + 9, apdat_.resCom2_.img);
			}
		}

		public void Draw_newPlayerLv(Graphics g)
		{
			apdat_.DrawWindow(g, 228, 204, 0, "ボーナス");
			apdat_.DrawBox(g, 15, 55, 225, 201);
			int num3;
			int num2;
			int num = (num2 = (num3 = 0));
			num = 51;
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			apdat_.DrawExp(g, 81, num + 2);
			g.DrawString("獲得", 140f, num);
			int num4 = GameUtil.RateConvert(stateCnt_, PBONUS_NP_LV_TIME[0], PBONUS_NP_LV_TIME[1], 0, PBONUS_NP_LV_ADDEXP, true);
			apdat_.DrawNumImage(g, 35, string.Empty, num4, string.Empty, 120, num + 1, 0, 2);
			int num5 = 19;
			num = 70;
			for (int i = 0; i < 11; i++)
			{
				num2 = num5 + i / 6 * 100;
				num3 = num + i % 6 * 23;
				PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(i);
				g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
				g.DrawRect(num2, num3, 100f, 23f);
				if (bonus_softKey == 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(192, 212, 153));
					g.FillRect(num2 + 5, num3 + 4, 16f, 16f);
					g.SetColor(Graphics.GetColorOfRGB(233, 240, 219));
					g.DrawRect(num2 + 5, num3 + 4, 15f, 15f);
					g.ClipRect(num2 + 5, num3 + 4, 13f, 15f);
					udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, 0, 1);
					udat_.DrawDispPlayer(g, num2 + 13, num3 + 25);
					g.PopClip();
					apdat_.DrawSeb(g, num2 + 21, num3 + 6, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], playerData.role_);
				}
				else
				{
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					SubForm.SetFontSize(g, 10);
					if (!MyFormBase.En())
					{
						g.DrawString(playerData.name_, num2 + 5 + 1, num3 + 5 + 1);
					}
					else
					{
						g.DrawString(playerData.name_, num2 + 5 + 1 - 2, num3 + 5 + 1 + 1);
					}
					SubForm.ResetFontSize(g);
				}
				if (select_ == 0)
				{
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					apdat_.DrawLv(g, MyFormBase.LT("Lv") + ":", num2 + 50, num3 + 4);
					if (playerData.temp_anime > 0)
					{
						int num6 = GameUtil.RateConvert(playerData.temp_anime, 12, 0, -12, 0, true);
						if (playerData.temp_lv % 10 == 0)
						{
							g.ClipRect(num2 + 89 - 20, num3 + 4, 20f, 12f);
							apdat_.DrawNumImage(g, 79, string.Empty, playerData.temp_lv, string.Empty, num2 + 89, num3 + 4 + num6, 0, 4);
							g.PopClip();
						}
						else
						{
							if (playerData.temp_lv / 10 > 0)
							{
								apdat_.DrawNumImage(g, 79, string.Empty, playerData.temp_lv / 10, string.Empty, num2 + 89 - 10, num3 + 4, 0, 4);
							}
							g.ClipRect(num2 + 89 - 10, num3 + 4, 10f, 12f);
							apdat_.DrawNumImage(g, 79, string.Empty, playerData.temp_lv, string.Empty, num2 + 89, num3 + 4 + num6, 0, 4);
							g.PopClip();
						}
					}
					else
					{
						apdat_.DrawNumImage(g, 79, string.Empty, playerData.temp_lv, string.Empty, num2 + 89, num3 + 4, 0, 4);
					}
					int c1min = 0;
					int nextLvUpExp = PlayerData.GetNextLvUpExp(playerData.temp_lv);
					int num7 = 0;
					num7 = GameUtil.RateConvert(playerData.temp_exp, c1min, nextLvUpExp, 0, 42, true);
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
					g.FillRect(num2 + 50, num3 + 15 + 2, 44f, 4f);
					g.SetColor(Graphics.GetColorOfRGB(10, 149, 46));
					g.DrawRect(num2 + 50, num3 + 15 + 2, 43f, 3f);
					g.SetColor(Graphics.GetColorOfRGB(92, 255, 7));
					g.FillRect(num2 + 50 + 1, num3 + 15 + 1 + 2, num7, 2f);
				}
				else
				{
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					if (!MyFormBase.En())
					{
						apdat_.DrawNumImage(g, 79, string.Empty, playerData.actscoreScore_, string.Empty, num2 + 62 + 2 + 16, num3 + 6, 0, 4);
						g.DrawString("点", num2 + 62 + 2 + 16 + 2, num3 + 6);
					}
					else
					{
						apdat_.DrawNumImage(g, 79, string.Empty, playerData.actscoreScore_, string.Empty, num2 + 62 + 2 + 16 - 2, num3 + 6, 0, 4);
						g.DrawString("点", num2 + 62 + 2 + 16 + 2 - 2, num3 + 6);
					}
				}
				if (playerData.temp_lvUp && stateCnt_ % 20 / 10 == 0)
				{
					apdat_.resCom2_.DrawSeb(g, num2 + 23, num3 + 25 - 1, 94, 0);
				}
			}
			num5 = 80;
			num = 214;
			if (select_ == 0)
			{
				g.SetColor(SubForm.SC_WINDOW_SELECT);
				g.FillRect(num5 - 20, num, 40f, 16f);
			}
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			g.DrawString("次へ", num5, num + 2, 2);
			if (select_ == 0 && stateCnt_ >= PBONUS_NP_LV_TIME[3])
			{
				apdat_.resCom2_.seb[60].Draw(g, num5 - 20 - 4, num + 9, apdat_.resCom2_.img);
			}
			if (!MyFormBase.En())
			{
				num5 = 160;
				if (select_ == 1)
				{
					g.SetColor(SubForm.SC_WINDOW_SELECT);
					g.FillRect(num5 - 20, num, 40f, 16f);
				}
				g.SetColor(SubForm.SC_WINDOW_BLACK);
				g.DrawString("採点", num5, num + 2, 2);
			}
			else
			{
				num5 = 170;
				if (select_ == 1)
				{
					g.SetColor(SubForm.SC_WINDOW_SELECT);
					g.FillRect(num5 - 25, num, 50f, 16f);
				}
				g.SetColor(SubForm.SC_WINDOW_BLACK);
				g.DrawString("採点", num5, num + 2, 2);
			}
			if (select_ == 1 && stateCnt_ >= PBONUS_NP_LV_TIME[3])
			{
				apdat_.resCom2_.seb[60].Draw(g, num5 - 20 - 4, num + 9, apdat_.resCom2_.img);
			}
			if (stateCnt_ < PBONUS_NP_LV_TIME[3])
			{
				GameView.AddTouch(2);
				return;
			}
			int num8 = 50;
			for (int j = 0; j < 2; j++)
			{
				num2 = 80 - num8 / 2 + 80 * j;
				GameView.AddTouch(g, 3, num2 + SubForm.DLGSEL_TRECT[j][0], num + SubForm.DLGSEL_TRECT[j][1], num8 + SubForm.DLGSEL_TRECT[j][2], 16 + SubForm.DLGSEL_TRECT[j][3], 0x20000 | j);
			}
			GameView.AddTouch(2, TouchOption.Create(2));
		}

		public void Draw_newPlayerPopular(Graphics g)
		{
			apdat_.DrawWindow(g, 228, 204, 0, "ボーナス");
			apdat_.DrawBox(g, 15, 55, 225, 201);
			int num4;
int num;
int num2;
int num3;


			num4 = num = num2 = num3 = 0;
			num = 51;
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			g.DrawString("人気UP", 120f, num, 2);
			num4 = 19;
			num = 70;
			for (int i = 0; i < 11; i++)
			{
				num2 = num4 + i / 6 * 100;
				num3 = num + i % 6 * 23;
				PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(i);
				g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
				g.DrawRect(num2, num3, 100f, 23f);
				if (bonus_softKey == 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(192, 212, 153));
					g.FillRect(num2 + 5, num3 + 4, 16f, 16f);
					g.SetColor(Graphics.GetColorOfRGB(233, 240, 219));
					g.DrawRect(num2 + 5, num3 + 4, 15f, 15f);
					g.ClipRect(num2 + 5, num3 + 4, 13f, 15f);
					udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, 0, 1);
					udat_.DrawDispPlayer(g, num2 + 13, num3 + 25);
					g.PopClip();
					apdat_.DrawSeb(g, num2 + 21, num3 + 6, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], playerData.role_);
				}
				else
				{
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					g.DrawString(playerData.name_, num2 + 5 + 1, num3 + 5 + 1);
				}
				if (select_ == 0)
				{
					if (stateCnt_ >= PBONUS_NP_LV_POPULAR[0])
					{
						if (stateCnt_ >= PBONUS_NP_LV_POPULAR[0] && stateCnt_ < PBONUS_NP_LV_POPULAR[2])
						{
							int num5 = GameUtil.RateConvert(stateCnt_, PBONUS_NP_LV_POPULAR[0], PBONUS_NP_LV_POPULAR[1], -12, 0, true);
							g.ClipRect(num2 + 52, num3 + 5, 40f, 16f);
							SubForm.Draw_upParam(g, 35, num2 + 89, num3 + 7 + num5, PBONUS_NP_LV_GETPOPULAR);
							g.PopClip();
						}
						else
						{
							int num6 = GameUtil.RateConvert(stateCnt_, PBONUS_NP_LV_POPULAR[2], PBONUS_NP_LV_POPULAR[3], playerData.popularP_, playerData.popularP_ + PBONUS_NP_LV_GETPOPULAR, true);
							apdat_.DrawNumImage(g, 35, string.Empty, num6, string.Empty, num2 + 89, num3 + 7, 0, 4);
						}
					}
				}
				else
				{
					apdat_.DrawNumImage(g, 30, string.Empty, playerData.actscoreScore_, string.Empty, num2 + 62 + 2 + 16, num3 + 6, 0, 4);
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					g.DrawString("点", num2 + 62 + 2 + 16 + 2, num3 + 6);
				}
			}
			num4 = 120;
			num = 214;
			if (select_ == 0)
			{
				g.SetColor(SubForm.SC_WINDOW_SELECT);
				g.FillRect(num4 - 20, num, 40f, 16f);
			}
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			g.DrawString("次へ", num4, num + 2, 2);
			if (select_ == 0)
			{
				apdat_.resCom2_.seb[60].Draw(g, num4 - 20 - 4, num + 9, apdat_.resCom2_.img);
			}
		}

		public void Draw_newPlayerReserach(Graphics g)
		{
			apdat_.DrawWindow(g, 228, 204, 0, "ボーナス");
			apdat_.DrawBox(g, 15, 55, 225, 201);
			int num4;
int num;
int num2;
int num3;


			num4 = num = num2 = num3 = 0;
			num = 51;
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			g.DrawString("研究ﾎﾟｲﾝﾄの獲得", 120f, num, 2);
			num4 = 19;
			num = 70;
			for (int i = 0; i < 11; i++)
			{
				num2 = num4 + i / 6 * 100;
				num3 = num + i % 6 * 23;
				PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(i);
				g.SetColor(Graphics.GetColorOfRGB(199, 223, 148));
				g.DrawRect(num2, num3, 100f, 23f);
				if (bonus_softKey == 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(192, 212, 153));
					g.FillRect(num2 + 5, num3 + 4, 16f, 16f);
					g.SetColor(Graphics.GetColorOfRGB(233, 240, 219));
					g.DrawRect(num2 + 5, num3 + 4, 15f, 15f);
					g.ClipRect(num2 + 5, num3 + 4, 13f, 15f);
					udat_.SetDispPlayerData(0, udat_.selectUniform_, playerData.faceImg_, 0, 1);
					udat_.DrawDispPlayer(g, num2 + 13, num3 + 25);
					g.PopClip();
					apdat_.DrawSeb(g, num2 + 21, num3 + 6, apdat_.resCom2_.seb[47], apdat_.resCom2_.img[51], playerData.role_);
				}
				else
				{
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					SubForm.SetFontSize(g, 9);
					if (!MyFormBase.En())
					{
						g.DrawString(playerData.name_, num2 + 5 + 1, num3 + 5 + 1);
					}
					else
					{
						g.DrawString(playerData.name_, num2 + 5 + 1 - 4, num3 + 5 + 1 + 1);
					}
					SubForm.ResetFontSize(g);
				}
				if (select_ == 0)
				{
					g.DrawImage(apdat_.resCom2_.img[165], num2 + 43, num3 + 2);
					int num5 = stateCnt_ - PBONUS_NP_LV_TRAINING1[0] - i * 16;
					if (num5 < 0)
					{
						continue;
					}
					if (num5 < PBONUS_NP_LV_T_GET[0])
					{
						int num6 = GameUtil.RateConvert(num5, 0, PBONUS_NP_LV_T_GET[0], 0, 4, true);
						if (num6 >= 4)
						{
							num6 = 3;
						}
						g.SetColor(SubForm.SC_WINDOW_BLACK);
						for (int j = 0; j < num6; j++)
						{
							g.DrawString("・", num2 + 56 + j * 12, num3 + 7);
						}
					}
					else if (playerData.bonus_research[0] != -1)
					{
						int num7 = GameUtil.RateConvert(num5, PBONUS_NP_LV_T_GET[0], PBONUS_NP_LV_T_GET[1], -20, 0, true);
						g.ClipRect(num2 + 43, num3 + 2, 56f, 20f);
						apdat_.DrawSeb(g, num2 + 52, num3 + 4 + num7, apdat_.resCom2_.seb[83], apdat_.resCom2_.img[94], playerData.bonus_research[0]);
						apdat_.DrawNumImage(g, 78, playerData.bonus_research[1], num2 + 82, num3 + 7 + num7, 0, 2);
						g.PopClip();
						g.DrawImage(apdat_.resCom2_.img[163], num2 + 62, num3 + 16);
					}
					else
					{
						g.DrawImage(apdat_.resCom2_.img[164], num2 + 47, num3 + 6);
					}
				}
				else
				{
					apdat_.DrawNumImage(g, 79, string.Empty, playerData.actscoreScore_, string.Empty, num2 + 62 + 2 + 16, num3 + 6, 0, 4);
					g.SetColor(SubForm.SC_WINDOW_BLACK);
					g.DrawString("点", num2 + 62 + 2 + 16 + 2, num3 + 6);
				}
			}
			num4 = 120;
			num = 214;
			if (select_ == 0)
			{
				g.SetColor(SubForm.SC_WINDOW_SELECT);
				g.FillRect(num4 - 20, num, 40f, 16f);
			}
			g.SetColor(SubForm.SC_WINDOW_BLACK);
			g.DrawString("次へ", num4, num + 2, 2);
			if (select_ == 0)
			{
				apdat_.resCom2_.seb[60].Draw(g, num4 - 20 - 4, num + 9, apdat_.resCom2_.img);
			}
			GameView.AddTouch(2);
		}

		public void SetCustomerNum()
		{
			int evaCumstomer = GetEvaCumstomer(true);
			int idealSalse = udat_.GetIdealSalse(evaCumstomer);
			float ticketSalseRate = udat_.GetTicketSalseRate(evaCumstomer);
			int num = (int)((float)idealSalse * ticketSalseRate);
			long num2 = (long)num * 10000L / udat_.ticketPrice_;
			customerNum[0] = (int)num2;
			TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[1]];
			customerNum[1] = GameUtil.RateConvert(teamData.fan_, 0, 1000, 0, 100000, true);
			customerNum[0] = customerNum[0] - customerNum[0] * 10 / 100 + customerNum[0] * GameUtil.Random(20) / 100;
			customerNum[1] = customerNum[1] - customerNum[1] * 10 / 100 + customerNum[1] * GameUtil.Random(20) / 100;
			int num3 = 0;
			if ((stadiumId_ == 0)) { num3 = 40000; } else { if (stadiumId_ == 1) { num3 = 20000; } else { num3 = int.MaxValue; } }
			customerNum[0] = AppData.ClampMax(customerNum[0], num3);
			customerNum[1] = AppData.ClampMax(customerNum[1], num3);
			customerNum[0] -= customerNum[0] % 10;
			customerNum[1] -= customerNum[1] % 10;
		}

		public int GetEvaCumstomer(bool isMatch)
		{
			if (udat_ == null)
			{
				udat_ = UserData.GetInstance();
			}
			if (apdat_ == null)
			{
				apdat_ = AppData.GetInstance();
			}
			TeamData teamData = apdat_.tData_[udat_.gm_.teamId_[1]];
			int num = 0;
			for (int i = 0; i < 11; i++)
			{
				PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(i);
				num += playerData.popularP_;
			}
			int fanEvaToCommonValue = FanActData.GetFanEvaToCommonValue(0);
			int fanEvaToCommonValue2 = FanActData.GetFanEvaToCommonValue(1);
			int fanEvaToCommonValue3 = FanActData.GetFanEvaToCommonValue(2);
			int num2 = 0;
			int num3 = num / 11;
			if (isMatch)
			{
				num2 = teamData.warPotential_;
			}
			float num4 = 0f;
			if (isMatch && udat_.gm_.homeAway_ == 0)
			{
				num4 = GameUtil.RateConvert(fanEvaToCommonValue2, 0f, 1000f, 0.75f, 1f, true);
				fanEvaToCommonValue = (int)((float)fanEvaToCommonValue * num4);
			}
			else
			{
				num4 = GameUtil.RateConvert(fanEvaToCommonValue2, 0f, 1000f, 0.5f, 1f, true);
				fanEvaToCommonValue = (int)((float)fanEvaToCommonValue * num4);
			}
			{ int recoveredReturn = (int)((0.5f * (float)fanEvaToCommonValue + 0.1f * (float)fanEvaToCommonValue3 + 0.05f * (float)num2 + 0.1f * (float)num3) / 0.75000006f); return recoveredReturn; }
		}

		public int GetMatchPrice()
		{
			int matchInfoJoin = udat_.GetMatchInfoJoin();
			int num = 0;
			if ((matchInfoJoin == 0)) { {
				LeagueData leagueData = apdat_.leagueData_[udat_.selectLeagueId_];
				if (udat_.gm_.score_[0] > udat_.gm_.score_[1])
				{
					num += leagueData.matchPrize_;
				}
				if (LeagueData.IsVictoryLeague())
				{
					num += leagueData.victoryPrize_;
				}
				return num;
			} } else if ((matchInfoJoin == 1)) { {
				QuestMatchData questMatchData = apdat_.qmData_[udat_.selectQmId_];
				if (udat_.gm_.score_[0] > udat_.gm_.score_[1])
				{
					num += questMatchData.matchPrize_;
				}
				if (QuestMatchData.IsVictoryQuestMatch())
				{
					num += questMatchData.victoryPrize_;
				}
				return num;
			} } else { return 0; }
		}

		public int GetGoodsPrice()
		{
			bool flag = false;
			for (int i = 0; i < udat_.tenants_.Length; i++)
			{
				Tenant tenant = udat_.tenants_[i];
				if (tenant.tenantDataId_ == TenantData.TENANT_ID_GOODS)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return 0;
			}
			TenantData tenantData = apdat_.tenantData_[TenantData.TENANT_ID_GOODS];
			float goodsIncomeRate = tenantData.GetGoodsIncomeRate();
			{ int recoveredReturn = (int)((float)customerNum[0] * goodsIncomeRate / 10000f); return recoveredReturn; }
		}

		public int GetSponsorPrice()
		{
			if (apdat_ == null)
			{
				apdat_ = AppData.GetInstance();
			}
			int num = 0;
			for (int i = 0; i < apdat_.sponsorData_.Length; i++)
			{
				SponsorData sponsorData = apdat_.sponsorData_[i];
				if (sponsorData.state_ == 2)
				{
					num += sponsorData.GetIncome();
				}
			}
			return num;
		}

		public int GetSponsorNum()
		{
			int num = 0;
			for (int i = 0; i < apdat_.sponsorData_.Length; i++)
			{
				SponsorData sponsorData = apdat_.sponsorData_[i];
				if (sponsorData.state_ == 2)
				{
					num++;
				}
			}
			return num;
		}

		public int GetPlayersSalary()
		{
			int num = 0;
			for (int i = 0; i < 11; i++)
			{
				PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(i);
				num += playerData.GetSallay();
			}
			return num;
		}

		private int[] GetScaleRange()
		{
			getScaleRange_tmp[0] = GetScaleMin();
			getScaleRange_tmp[1] = GetScaleMax();
			return getScaleRange_tmp;
		}

		private int GetScaleMin()
		{
			if (view_.GetGameWidth() > 240)
			{
				return 75;
			}
			return 60;
		}

		private int GetScaleMax()
		{
			return 200;
		}

		private void SetScale(int scale)
		{
			int[] scaleRange = GetScaleRange();
			scale = AppData.Clamp(scale, scaleRange[0], scaleRange[1]);
			if (scale == scale_) { return; }
{
int gameWidth = GetGameWidth();
int gameHeight = GetGameHeight();
scale_ = scale;
ResetOffscreen();
}
		}

		public int GetGameWidth()
		{
			return (view_.GetGameWidth() - VIEW_X) * 100 / scale_;
		}

		public int GetGameHeight()
		{
			return (view_.GetGameHeight() - VIEW_Y) * 100 / scale_;
		}

		public override int GetTouchValue(int type, int index)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 131072:
				return select_;
			case 262144:
				if (state_ == 6)
				{
					return udat_.scrollValue_;
				}
				return scrollValue_;
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
			case 262144:
				scrollValue_ = value;
				break;
			default:
				base.SetTouchValue(type, value);
				break;
			}
		}

		public override bool OnTouchEvent(TouchEvent e)
		{
			if (e.component.id == 17)
			{
				OnTouchCamera(e.type, e.component.value, e);
				return false;
			}
			return true;
		}

		public void OnTouchCamera(int type, int v, TouchEvent e)
		{
			float num = e.dx * 100 / scale_;
			float num2 = e.dy * 100 / scale_;
			switch (type)
			{
			case 5:
				if (touchCameraPoint_.Size() != 0)
				{
				}
				break;
			case 1:
				if (touchCameraPoint_.Size() > 0 && touchDragCamera_)
				{
					int[] array = (int[])touchCameraPoint_.FirstElement();
					touchCameraSpeed_.Set((int)num - array[0], 0f, (int)num2 - array[1]);
					touchCameraDecimal_.Set(0f, 0f, 0f);
				}
				break;
			case 12:
			{
				touchBuildDrag_ = false;
				pinchScaleValue_ = AppData.Clamp(pinchScaleValue_ + e.pinch, GetScaleMin(), GetScaleMax() + 40);
				int num3 = pinchScaleValue_;
				if (num3 > 100)
				{
					num3 = AppData.ClampMin(num3 - 40, 100);
				}
				SetScale(num3);
				touchDragCamera_ = false;
				break;
			}
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
