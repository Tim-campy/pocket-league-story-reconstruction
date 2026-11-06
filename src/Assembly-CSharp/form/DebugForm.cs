using System;
using data;
using game;
using java.lang;
using java.util;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace form
{
	public class DebugForm : MyFormBase
	{
		private const int MODE_PARAM = 0;

		private const int MODE_GAMEPARAM = 1;

		private const int MODE_WINDOW = 2;

		public const int TPP_DISPMAX = 13;

		private const int PARAM_1 = 1;

		private const int PARAM_2 = 2;

		private const int PARAM_3 = 3;

		private const int PARAM_4 = 4;

		private const int PARAM_6 = 6;

		private const int PARAM_9 = 9;

		private const int PARAM_TEAMPLAYER = 11;

		private UserData udat_;

		public int mode;

		public int state_;

		public int select_;

		public int frame_;

		public static int COATEVA_TEAM_ID;

		public static int COATEVA_PLAYER_ID;

		public static int COATEVA_ROOT_INDEX;

		public static int COATEVA_WAKU;

		public static int CH_DATA_INDEX;

		public static int CH_DATA_INDEX2;

		public Vector3D[] formationRect_global_pos;

		public int[][] formationRect_camera_pos;

		public Player temp_player = new Player(0);

		public int PLAYER_FACE = 300;

		public int PLAYER_BODY;

		public int PLAYER_SEB;

		public int PLAYER_DIRECT = 1;

		public int PLAYER_FRAME;

		public int PLAYER_ANIME = 1;

		public int PLAYER_TEAM;

		public int BALL;

		public int PLAYER_EFINDEX;

		public int SHADOW;

		public int GOAL;

		public int GOAL_INDEX;

		public int TEST_P1_TEAM;

		public int TEST_P1_POTENTIAL;

		public int TEST_P1_FORMATION;

		public int TEST_P2_TEAM = 1;

		public int TEST_P2_POTENTIAL;

		public int TEST_P2_FORMATION;

		public int TEST_STADIUM;

		public int TEST_GROUND;

		public static int TEST_STATE = 0;

		public int ch_bgmIndex;

		public int ch_seIndex;

		public int ch_bgmVolumeIndex;

		public int ch_seVolumeIndex;

		public int[] ch_kickSound = new int[5];

		public int[] ch_shootSound = new int[2];

		public int ch_formationId;

		public int ch_ameftoId;

		public int ch_sChangeId;

		public static int max_fan;

		public static int max_assist;

		public static int max_nameValue;

		public static int flg_playerAll;

		public static int flg_playerLvMax;

		public static int flg_playerTPMax;

		public static int flg_formationMax;

		public static int flg_warPMax;

		public static int flg_researchPointMax;

		public static int flg_loadCheck;

		public static int flg_menuOpen;

		public static int flg_leagueMax;

		public static int flg_qmMax;

		public static int flg_trainingMax;

		public static int flg_fanPMax;

		public static int flg_tradeMax;

		public static int flg_tenantMax;

		public static int flg_sponsorMax;

		public static int flg_svisorMax;

		public static int flg_LVMAX_svisor;

		public static int flg_LVMAX_fanAct;

		public static int flg_LVMAX_sponsor;

		public static int flg_LVMAX_tenant;

		public static int tpp_index;

		public int tpp_scrollValue_;

		private int vCH_DATA_INDEX;

		public static int chFanBonusIndex;

		public static int ch_news;

		public static int ch_talks;

		public static int ch_eventMsgs;

		public static int ch_elements;

		public static int ch_popUp;

		private int fukidashiIndex_;

		private int fukidashiFeel_;

		public static int[] debug_testscore = new int[2];

		public static Vector debug_scorevec = new Vector();

		internal string[] on_off = new string[3] { "off", "on", "on2" };

		internal string[] str_direct = new string[4] { "↑", "→", "↓", "←" };

		internal string[] waku = new string[2] { "○", "×" };

		internal string[] player_param = new string[11];

		private Vector sortDraw = new Vector();

		private int[] camera_pos = new int[2];

		public DebugForm()
			: base(5)
		{
		}

		public override bool Init()
		{
			base.Init();
			udat_ = UserData.GetInstance();
			SetSoftLabel(AppData.softLabels_[6], AppData.softLabels_[2]);
			mode = 0;
			formationRect_global_pos = new Vector3D[4];
			for (int i = 0; i < formationRect_global_pos.Length; i++)
			{
				formationRect_global_pos[i] = new Vector3D();
			}
			formationRect_camera_pos = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			UpdateTestParam();
			ch_bgmVolumeIndex = apdat_.sysSave_.ints[3];
			ch_seVolumeIndex = apdat_.sysSave_.ints[4];
			ch_kickSound[0] = Ball.SOUND_PASS[0] - 8;
			ch_kickSound[1] = Ball.SOUND_PASS[2] - 8;
			ch_kickSound[2] = Ball.SOUND_PASS[3] - 8;
			ch_kickSound[3] = Ball.SOUND_PASS[5] - 8;
			ch_kickSound[4] = Ball.SOUND_HEAD - 8;
			state_ = 10;
			return true;
		}

		public override bool Finish()
		{
			return true;
		}

		public override bool Update()
		{
			frame_ = (frame_ + 1) % int.MaxValue;
			if (canvas_.CheckKeyPulse(1) || apdat_.IsPushSoftLabel(6))
			{
				mode = (mode + 1) % 3;
				state_ = 0;
				select_ = 0;
			}
			if (mode == 0)
			{
				Update_param();
			}
			else if (mode == 1)
			{
				Update_gameParam();
			}
			else if (mode == 2)
			{
				Update_window();
			}
			return true;
		}

		public bool Update_param()
		{
			int num = 12;
			if (state_ == 0)
			{
				int num2 = 4;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num2) % num2;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num2) % num2;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						COATEVA_TEAM_ID = 1 - COATEVA_TEAM_ID;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						COATEVA_TEAM_ID = 1 - COATEVA_TEAM_ID;
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						COATEVA_ROOT_INDEX = (COATEVA_ROOT_INDEX - 1 + 9) % 9;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						COATEVA_ROOT_INDEX = (COATEVA_ROOT_INDEX + 1 + 9) % 9;
					}
				}
				if (select_ == 3)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						COATEVA_WAKU = 1 - COATEVA_WAKU;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						COATEVA_WAKU = 1 - COATEVA_WAKU;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 1)
			{
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				for (int i = 0; i < Team.ch_ceva.Length; i++)
				{
					if (select_ == i + 1)
					{
						if (canvas_.CheckKeyRepeat(65536))
						{
							Team.ch_ceva[i] = 1 - Team.ch_ceva[i];
						}
						if (canvas_.CheckKeyRepeat(262144))
						{
							Team.ch_ceva[i] = 1 - Team.ch_ceva[i];
						}
					}
				}
				if ((canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144)) && udat_.teams_ != null)
				{
					udat_.teams_[0].Update_coatEva();
					udat_.teams_[1].Update_coatEva();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 2)
			{
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						CH_DATA_INDEX = 1 - CH_DATA_INDEX;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						CH_DATA_INDEX = 1 - CH_DATA_INDEX;
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						CH_DATA_INDEX2 = (CH_DATA_INDEX2 - 1 + 6) % 6;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						CH_DATA_INDEX2 = (CH_DATA_INDEX2 + 1 + 6) % 6;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 3)
			{
				int mAPCHIP_COAT_WNUM = Map.MAPCHIP_COAT_WNUM;
				int mAPCHIP_COAT_HNUM = Map.MAPCHIP_COAT_HNUM;
				int num3 = 0;
				if (apdat_.map_ != null)
				{
					num3 = apdat_.map_.goalBackNetNum_;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						Map.MAPCHIP_COAT_WNUM = AppData.ClampMin(Map.MAPCHIP_COAT_WNUM - 1, 1);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						Map.MAPCHIP_COAT_WNUM = AppData.ClampMax(Map.MAPCHIP_COAT_WNUM + 1, 100);
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						Map.MAPCHIP_COAT_HNUM = AppData.ClampMin(Map.MAPCHIP_COAT_HNUM - 1, 1);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						Map.MAPCHIP_COAT_HNUM = AppData.ClampMax(Map.MAPCHIP_COAT_HNUM + 1, 100);
					}
				}
				if (select_ == 3 && canvas_.CheckKeyPulse(1048576))
				{
					apdat_.camera.ChangeMode(0);
					return formMan_.Pop();
				}
				if (select_ == 4 && canvas_.CheckKeyPulse(1048576))
				{
					apdat_.camera.ChangeMode(1);
					return formMan_.Pop();
				}
				if (select_ == 5 && canvas_.CheckKeyPulse(1048576))
				{
					apdat_.camera.ChangeMode(2);
					return formMan_.Pop();
				}
				if (select_ == 6)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						apdat_.map_.goalBackNetNum_ = AppData.ClampMin(apdat_.map_.goalBackNetNum_ - 1, 2);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						apdat_.map_.goalBackNetNum_ = AppData.ClampMax(apdat_.map_.goalBackNetNum_ + 1, 3);
					}
				}
				if (apdat_.map_ != null && (mAPCHIP_COAT_WNUM != Map.MAPCHIP_COAT_WNUM || mAPCHIP_COAT_HNUM != Map.MAPCHIP_COAT_HNUM || num3 != apdat_.map_.goalBackNetNum_))
				{
					Camera.SetScreenData();
					apdat_.map_.SetCoatLine();
					for (int j = 0; j < udat_.teams_.Length; j++)
					{
						udat_.teams_[j].Init_coatEva();
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 4)
			{
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						COATEVA_TEAM_ID = 1 - COATEVA_TEAM_ID;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						COATEVA_TEAM_ID = 1 - COATEVA_TEAM_ID;
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						COATEVA_PLAYER_ID = (COATEVA_PLAYER_ID - 1 + 11) % 11;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						COATEVA_PLAYER_ID = (COATEVA_PLAYER_ID + 1 + 11) % 11;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 5)
			{
				int num4 = 3;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num4) % num4;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num4) % num4;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1 && canvas_.CheckKeyPulse(1048576))
				{
					udat_.League_update();
				}
				if (select_ == 2)
				{
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 6)
			{
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
				}
				if (select_ == 2)
				{
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 7)
			{
				int num5 = 6;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num5) % num5;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num5) % num5;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1 && canvas_.CheckKeyPulse(1048576))
				{
					formMan_.ChangeCurrentForm(apdat_.frmSoccer_);
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						udat_.gm_.team_potential_[0] = AppData.ClampMin(udat_.gm_.team_potential_[0] - 100, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						udat_.gm_.team_potential_[0] = AppData.ClampMax(udat_.gm_.team_potential_[0] + 100, 1000);
					}
				}
				if (select_ == 3)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						udat_.gm_.team_potential_[1] = AppData.ClampMin(udat_.gm_.team_potential_[1] - 100, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						udat_.gm_.team_potential_[1] = AppData.ClampMax(udat_.gm_.team_potential_[1] + 100, 1000);
					}
				}
				if (select_ == 4)
				{
				}
				if (select_ == 5)
				{
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 8)
			{
				int num6 = 6;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num6) % num6;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num6) % num6;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						GOAL = 1 - GOAL;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						GOAL = 1 - GOAL;
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						GOAL_INDEX = (GOAL_INDEX - 1 + Map.GOAL1_PARTS_POS.Length) % Map.GOAL1_PARTS_POS.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						GOAL_INDEX = (GOAL_INDEX + 1 + Map.GOAL1_PARTS_POS.Length) % Map.GOAL1_PARTS_POS.Length;
					}
				}
				if (select_ == 3)
				{
					int[] array = null;
					if (GOAL == 0)
					{
						array = Map.GOAL1_PARTS_POS[GOAL_INDEX];
					}
					if (GOAL == 1)
					{
						array = Map.GOAL2_PARTS_POS[GOAL_INDEX];
					}
					if (canvas_.CheckKeyRepeat(65536))
					{
						array[0] = AppData.ClampMin(array[0] - 1, -1000);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						array[0] = AppData.ClampMax(array[0] + 1, 1000);
					}
				}
				if (select_ == 4)
				{
					int[] array2 = null;
					if (GOAL == 0)
					{
						array2 = Map.GOAL1_PARTS_POS[GOAL_INDEX];
					}
					if (GOAL == 1)
					{
						array2 = Map.GOAL2_PARTS_POS[GOAL_INDEX];
					}
					if (canvas_.CheckKeyRepeat(65536))
					{
						array2[1] = AppData.ClampMin(array2[1] - 1, -1000);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						array2[1] = AppData.ClampMax(array2[1] + 1, 1000);
					}
				}
				if (select_ == 5)
				{
					int num7 = 0;
					if (GOAL == 0)
					{
						num7 = Map.GOAL1_PARTS_GRA[GOAL_INDEX][2];
					}
					if (GOAL == 1)
					{
						num7 = Map.GOAL2_PARTS_GRA[GOAL_INDEX][2];
					}
					if (canvas_.CheckKeyRepeat(65536))
					{
						num7 = AppData.ClampMin(num7 - 1, -1000);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						num7 = AppData.ClampMax(num7 + 1, 1000);
					}
					if (GOAL == 0)
					{
						Map.GOAL1_PARTS_GRA[GOAL_INDEX][2] = num7;
					}
					if (GOAL == 1)
					{
						Map.GOAL2_PARTS_GRA[GOAL_INDEX][2] = num7;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 9)
			{
				if (GOAL_INDEX >= MapMain.GOAL1_PARTS_POS.Length)
				{
					GOAL_INDEX = 0;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						GOAL = 1 - GOAL;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						GOAL = 1 - GOAL;
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						GOAL_INDEX = (GOAL_INDEX - 1 + MapMain.GOAL1_PARTS_POS.Length) % MapMain.GOAL1_PARTS_POS.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						GOAL_INDEX = (GOAL_INDEX + 1 + MapMain.GOAL1_PARTS_POS.Length) % MapMain.GOAL1_PARTS_POS.Length;
					}
				}
				if (select_ == 3)
				{
					int[] array3 = null;
					if (GOAL == 0)
					{
						array3 = MapMain.GOAL1_PARTS_POS[GOAL_INDEX];
					}
					if (GOAL == 1)
					{
						array3 = MapMain.GOAL2_PARTS_POS[GOAL_INDEX];
					}
					if (canvas_.CheckKeyRepeat(65536))
					{
						array3[0] = AppData.ClampMin(array3[0] - 1, -1000);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						array3[0] = AppData.ClampMax(array3[0] + 1, 1000);
					}
				}
				if (select_ == 4)
				{
					int[] array4 = null;
					if (GOAL == 0)
					{
						array4 = MapMain.GOAL1_PARTS_POS[GOAL_INDEX];
					}
					if (GOAL == 1)
					{
						array4 = MapMain.GOAL2_PARTS_POS[GOAL_INDEX];
					}
					if (canvas_.CheckKeyRepeat(65536))
					{
						array4[1] = AppData.ClampMin(array4[1] - 1, -1000);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						array4[1] = AppData.ClampMax(array4[1] + 1, 1000);
					}
				}
				if (select_ == 5)
				{
					int num8 = 0;
					if (GOAL == 0)
					{
						num8 = MapMain.GOAL1_PARTS_GRA[GOAL_INDEX][2];
					}
					if (GOAL == 1)
					{
						num8 = MapMain.GOAL2_PARTS_GRA[GOAL_INDEX][2];
					}
					if (canvas_.CheckKeyRepeat(65536))
					{
						num8 = AppData.ClampMin(num8 - 1, -1000);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						num8 = AppData.ClampMax(num8 + 1, 1000);
					}
					if (GOAL == 0)
					{
						MapMain.GOAL1_PARTS_GRA[GOAL_INDEX][2] = num8;
					}
					if (GOAL == 1)
					{
						MapMain.GOAL2_PARTS_GRA[GOAL_INDEX][2] = num8;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 10)
			{
				int num9 = 11;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num9) % num9;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num9) % num9;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					int num10 = flg_loadCheck;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_loadCheck = 1;
					}
					if (num10 != flg_loadCheck)
					{
						for (int k = 0; k < udat_.tenants_.Length; k++)
						{
							udat_.tenants_[k].Init(TenantData.TENANT_ROOT[k]);
							udat_.tenants_[k].BuildTenant(6);
							udat_.tenants_[k].ChangeState(1);
						}
						for (int l = 0; l < apdat_.pData_.Length; l++)
						{
							if (udat_.memberPDataist_.Size() >= 30)
							{
								break;
							}
							PlayerData playerData = apdat_.pData_[l];
							if (!BitUtil.Check(playerData.flag_, 12))
							{
								PlayerData.Employ(l);
							}
						}
						for (int m = 0; m < udat_.researchPoint_.Length; m++)
						{
							udat_.AddResearchPoint(m, 10000);
						}
						TeamData teamData = apdat_.tData_[0];
						teamData.stadium_ = 2;
						udat_.tAreaRootNum_ = AppData.ClampMax(udat_.tAreaRootNum_ + 1, 2);
						apdat_.mapMain_.UpdateTrainingCoatNum();
						udat_.tAreaLv_ = AppData.ClampMax(udat_.tAreaLv_ + 1, 2);
						apdat_.frmGame_.SetAllDraw();
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						UserData.DEBUG_DISPFPS = (UserData.DEBUG_DISPFPS - 1 + 2) % 2;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						UserData.DEBUG_DISPFPS = (UserData.DEBUG_DISPFPS + 1 + 2) % 2;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						UserData.DEBUG_DISPFPS = (UserData.DEBUG_DISPFPS + 1 + 2) % 2;
					}
				}
				if (select_ == 3)
				{
					int num11 = apdat_.newsTitles_.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_news = (ch_news - 1 + num11) % num11;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_news = (ch_news + 1 + num11) % num11;
					}
					if (canvas_.CheckKeyRepeat(1048576))
					{
						apdat_.Push(SubForm.MakeNews(ch_news, null));
					}
				}
				if (select_ == 4)
				{
					int num12 = AppData.talks_.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_talks = (ch_talks - 1 + num12) % num12;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_talks = (ch_talks + 1 + num12) % num12;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						SubForm form = SubForm.MakeTalkForm(ch_talks, "<x>");
						apdat_.Push(form);
					}
				}
				if (select_ == 5)
				{
					int num13 = apdat_.eventMsgs_.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_eventMsgs = (ch_eventMsgs - 1 + num13) % num13;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_eventMsgs = (ch_eventMsgs + 1 + num13) % num13;
					}
					if (canvas_.CheckKeyPulse(1048576) && ch_eventMsgs >= 3)
					{
						SubForm form2 = SubForm.MakeEvent(apdat_.eventMsgs_[ch_eventMsgs], "<x>", ch_eventMsgs);
						apdat_.Push(form2);
					}
				}
				if (select_ == 6)
				{
					int num14 = UserData.POPUP_MEM.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_popUp = (ch_popUp - 1 + num14) % num14;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_popUp = (ch_popUp + 1 + num14) % num14;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						apdat_.AddPopupMessage(ch_popUp, "<x>");
						Pop();
					}
				}
				if (select_ == 7)
				{
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						ch_elements = 1 - ch_elements;
						if (ch_elements == 1)
						{
							for (int n = 0; n < 6; n++)
							{
								udat_.OnEventFlg(1 << n);
							}
							apdat_.save_.ints[0] += 1000000;
							for (int num15 = 0; num15 < udat_.researchPoint_.Length; num15++)
							{
								udat_.AddResearchPoint(num15, 10000);
							}
							for (int num16 = 0; num16 < apdat_.formationData_.Length; num16++)
							{
								FormationData formationData = apdat_.formationData_[num16];
								formationData.state_ = 1;
							}
							for (int num17 = 0; num17 < apdat_.leagueData_.Length; num17++)
							{
								LeagueData leagueData = apdat_.leagueData_[num17];
								leagueData.ChangeState(1);
							}
							udat_.OnEventFlg(1);
							for (int num18 = 0; num18 < apdat_.qmData_.Length; num18++)
							{
								QuestMatchData questMatchData = apdat_.qmData_[num18];
								questMatchData.ChangeState(2);
							}
							udat_.OnEventFlg(1);
							for (int num19 = 0; num19 < apdat_.trainingData_.Length; num19++)
							{
								TrainingData trainingData = apdat_.trainingData_[num19];
								trainingData.ChangeState(1);
							}
							for (int num20 = 0; num20 < apdat_.fanActData_.Length; num20++)
							{
								FanActData fanActData = apdat_.fanActData_[num20];
								fanActData.ChangeState(1);
							}
							for (int num21 = 0; num21 < apdat_.tradeData_.Length; num21++)
							{
								TradeData tradeData = apdat_.tradeData_[num21];
								tradeData.ChangeState(1);
							}
							for (int num22 = 0; num22 < apdat_.tenantData_.Length; num22++)
							{
								TenantData tenantData = apdat_.tenantData_[num22];
								tenantData.ChangeState(1);
							}
							for (int num23 = 0; num23 < apdat_.sponsorData_.Length; num23++)
							{
								SponsorData sponsorData = apdat_.sponsorData_[num23];
								sponsorData.ChangeState(1);
							}
							for (int num24 = 0; num24 < apdat_.svisorData_.Length; num24++)
							{
								SupervisorData supervisorData = apdat_.svisorData_[num24];
								supervisorData.ChangeState(1);
							}
							for (int num25 = 0; num25 < apdat_.svisorData_.Length; num25++)
							{
								SupervisorData supervisorData2 = apdat_.svisorData_[num25];
								supervisorData2.SetLv(9);
							}
							for (int num26 = 0; num26 < apdat_.fanBonus_.Length; num26++)
							{
								apdat_.fanBonus_[num26].ChangeState(1);
							}
						}
					}
				}
				else if (select_ == 8)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						UserData.DEBUG_SALLARY_MAX = (UserData.DEBUG_SALLARY_MAX - 1 + 2) % 2;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						UserData.DEBUG_SALLARY_MAX = (UserData.DEBUG_SALLARY_MAX + 1 + 2) % 2;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						UserData.DEBUG_SALLARY_MAX = (UserData.DEBUG_SALLARY_MAX + 1 + 2) % 2;
					}
				}
				else if (select_ == 9)
				{
					int num27 = UserData.EF_FEELPT_COMMENT_[0].Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						fukidashiIndex_ = (fukidashiIndex_ - 1 + num27) % num27;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						fukidashiIndex_ = (fukidashiIndex_ + 1 + num27) % num27;
					}
				}
				else if (select_ == 10)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						fukidashiFeel_--;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						fukidashiFeel_++;
					}
					if (fukidashiFeel_ < 0)
					{
						fukidashiFeel_ = 0;
					}
					if (fukidashiFeel_ >= 100)
					{
						fukidashiFeel_ = 100;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 11)
			{
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						CH_DATA_INDEX = (CH_DATA_INDEX - 1) % 2;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						CH_DATA_INDEX = (CH_DATA_INDEX + 1) % 2;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						CH_DATA_INDEX = (CH_DATA_INDEX + 1) % 2;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			return true;
		}

		public bool Update_gameParam()
		{
			int num = 7;
			if (state_ == 0)
			{
				int num2 = 16;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num2) % num2;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num2) % num2;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						apdat_.save_.ints[1] = AppData.ClampMin(apdat_.save_.ints[1] - 1, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						apdat_.save_.ints[1] = AppData.ClampMax(apdat_.save_.ints[1] + 1, 100);
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						apdat_.save_.ints[2] = (apdat_.save_.ints[2] - 1 + 12) % 12;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						apdat_.save_.ints[2] = (apdat_.save_.ints[2] + 1 + 12) % 12;
					}
				}
				if (select_ == 3)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						apdat_.save_.ints[3] = (apdat_.save_.ints[3] - 1 + 4) % 4;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						apdat_.save_.ints[3] = (apdat_.save_.ints[3] + 1 + 4) % 4;
					}
				}
				if (select_ == 4)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						apdat_.save_.lngs[0] = AppData.ClampMin(apdat_.save_.lngs[0] - 10000, -10000000L);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						apdat_.save_.lngs[0] = AppData.ClampMax(apdat_.save_.lngs[0] + 10000, 1000000000L);
					}
					if (canvas_.CheckKeyRepeat(1048576))
					{
						apdat_.save_.lngs[0] = AppData.ClampMax(apdat_.save_.lngs[0] + 1000000, 1000000000L);
					}
				}
				if (select_ == 5 && (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576)))
				{
					SoccerForm.DEBUG_GAMETIMESTOP = 1;
				}
				if (select_ == 6 && (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576)))
				{
					SoccerForm.DEBUG_GAMEMODE_5KEY = 1;
				}
				if (select_ == 7)
				{
					int num3 = flg_playerAll;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_playerAll = 1;
					}
					if (num3 != flg_playerAll)
					{
						int num4 = apdat_.pData_.Length - 1;
						while (num4 >= 0 && udat_.memberPDataist_.Size() < 30)
						{
							PlayerData playerData = apdat_.pData_[num4];
							if (!BitUtil.Check(playerData.flag_, 12))
							{
								PlayerData.Employ(num4);
							}
							num4--;
						}
					}
				}
				if (select_ == 8)
				{
					int num5 = flg_playerLvMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_playerLvMax = 1;
					}
					if (num5 != flg_playerLvMax)
					{
						for (int i = 0; i < apdat_.pData_.Length; i++)
						{
							PlayerData playerData2 = apdat_.pData_[i];
							if (!BitUtil.Check(playerData2.flag_, 12))
							{
								playerData2.SetLv(30);
							}
						}
					}
				}
				if (select_ == 9)
				{
					int num6 = flg_playerTPMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_playerTPMax = 1;
					}
					if (num6 != flg_playerTPMax)
					{
						int[] array = new int[5];
						for (int j = 0; j < array.Length; j++)
						{
							array[j] = 1000;
						}
						for (int k = 0; k < apdat_.pData_.Length; k++)
						{
							PlayerData playerData3 = apdat_.pData_[k];
							if (!BitUtil.Check(playerData3.flag_, 12))
							{
								playerData3.AddPhysicalParam(array);
							}
						}
					}
				}
				if (select_ == 10)
				{
					int num7 = flg_formationMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_formationMax = 1;
					}
					if (num7 != flg_formationMax)
					{
						for (int l = 0; l < apdat_.formationData_.Length; l++)
						{
							FormationData formationData = apdat_.formationData_[l];
							formationData.state_ = 1;
						}
					}
				}
				if (select_ == 11)
				{
					int num8 = flg_warPMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_warPMax = 1;
					}
					if (num8 != flg_warPMax)
					{
						TeamData teamData = apdat_.tData_[0];
						teamData.AddTeamEva(1000);
					}
				}
				if (select_ == 12)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						flg_researchPointMax = 0;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						flg_researchPointMax = 1;
					}
					if (flg_researchPointMax == 1)
					{
						for (int m = 0; m < udat_.researchPoint_.Length; m++)
						{
							udat_.AddResearchPoint(m, 10000);
						}
					}
				}
				if (select_ == 13)
				{
					int num9 = flg_loadCheck;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_loadCheck = 1;
					}
					if (num9 != flg_loadCheck)
					{
						for (int n = 0; n < udat_.tenants_.Length; n++)
						{
							udat_.tenants_[n].Init(TenantData.TENANT_ROOT[n]);
							udat_.tenants_[n].BuildTenant(6);
							udat_.tenants_[n].ChangeState(1);
						}
						for (int num10 = 0; num10 < apdat_.pData_.Length; num10++)
						{
							if (udat_.memberPDataist_.Size() >= 30)
							{
								break;
							}
							PlayerData playerData4 = apdat_.pData_[num10];
							if (!BitUtil.Check(playerData4.flag_, 12))
							{
								PlayerData.Employ(num10);
							}
						}
						for (int num11 = 0; num11 < udat_.researchPoint_.Length; num11++)
						{
							udat_.AddResearchPoint(num11, 10000);
						}
						TeamData teamData2 = apdat_.tData_[0];
						teamData2.stadium_ = 2;
						udat_.tAreaRootNum_ = AppData.ClampMax(udat_.tAreaRootNum_ + 1, 2);
						apdat_.mapMain_.UpdateTrainingCoatNum();
						udat_.tAreaLv_ = AppData.ClampMax(udat_.tAreaLv_ + 1, 2);
						apdat_.frmGame_.SetAllDraw();
					}
				}
				if (select_ == 14 && (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576)))
				{
					flg_menuOpen = 1;
					for (int num12 = 0; num12 < 6; num12++)
					{
						udat_.OnEventFlg(1 << num12);
					}
				}
				if (select_ == 15)
				{
					int max = apdat_.formationData_.Length;
					int num13 = 0;
					int num14 = 0;
					for (int num15 = 0; num15 < apdat_.formationData_.Length; num15++)
					{
						FormationData formationData2 = apdat_.formationData_[num15];
						if (formationData2.state_ == 1)
						{
							num13++;
						}
					}
					num14 = num13;
					if (canvas_.CheckKeyRepeat(65536))
					{
						num14 = AppData.ClampMin(num14 - 1, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						num14 = AppData.ClampMax(num14 + 1, max);
					}
					if (num13 != num14)
					{
						for (int num16 = 0; num16 < apdat_.formationData_.Length; num16++)
						{
							FormationData formationData3 = apdat_.formationData_[num16];
							formationData3.state_ = 0;
							if (num16 < num14)
							{
								formationData3.state_ = 1;
							}
						}
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 1)
			{
				int num17 = 16;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num17) % num17;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num17) % num17;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1 && (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576)))
				{
					max_fan = 1;
					udat_.fanParam_[0] = 10000;
				}
				if (select_ == 2 && (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576)))
				{
					max_assist = 1;
					udat_.fanParam_[1] = 1000;
				}
				if (select_ == 3 && (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576)))
				{
					max_nameValue = 1;
					udat_.fanParam_[2] = 10000;
				}
				if (select_ == 4)
				{
					int num18 = flg_leagueMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_leagueMax = 1;
					}
					if (num18 != flg_leagueMax)
					{
						for (int num19 = 0; num19 < apdat_.leagueData_.Length; num19++)
						{
							LeagueData leagueData = apdat_.leagueData_[num19];
							leagueData.ChangeState(1);
						}
						udat_.OnEventFlg(1);
					}
				}
				if (select_ == 5)
				{
					int num20 = flg_qmMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_qmMax = 1;
					}
					if (num20 != flg_qmMax)
					{
						for (int num21 = 0; num21 < apdat_.qmData_.Length; num21++)
						{
							QuestMatchData questMatchData = apdat_.qmData_[num21];
							questMatchData.ChangeState(2);
						}
						udat_.OnEventFlg(1);
					}
				}
				if (select_ == 6)
				{
					int num22 = flg_trainingMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_trainingMax = 1;
					}
					if (num22 != flg_trainingMax)
					{
						for (int num23 = 0; num23 < apdat_.trainingData_.Length; num23++)
						{
							TrainingData trainingData = apdat_.trainingData_[num23];
							trainingData.ChangeState(1);
						}
					}
				}
				if (select_ == 7)
				{
					int num24 = flg_fanPMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_fanPMax = 1;
					}
					if (num24 != flg_fanPMax)
					{
						for (int num25 = 0; num25 < apdat_.fanActData_.Length; num25++)
						{
							FanActData fanActData = apdat_.fanActData_[num25];
							fanActData.ChangeState(1);
						}
					}
				}
				if (select_ == 8)
				{
					int num26 = flg_tradeMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_tradeMax = 1;
					}
					if (num26 != flg_tradeMax)
					{
						for (int num27 = 0; num27 < apdat_.tradeData_.Length; num27++)
						{
							TradeData tradeData = apdat_.tradeData_[num27];
							tradeData.ChangeState(1);
						}
					}
				}
				if (select_ == 9)
				{
					int num28 = flg_tenantMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_tenantMax = 1;
					}
					if (num28 != flg_tenantMax)
					{
						for (int num29 = 0; num29 < apdat_.tenantData_.Length; num29++)
						{
							TenantData tenantData = apdat_.tenantData_[num29];
							tenantData.ChangeState(1);
						}
					}
				}
				if (select_ == 10)
				{
					int num30 = flg_sponsorMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_sponsorMax = 1;
					}
					if (num30 != flg_sponsorMax)
					{
						for (int num31 = 0; num31 < apdat_.sponsorData_.Length; num31++)
						{
							SponsorData sponsorData = apdat_.sponsorData_[num31];
							sponsorData.ChangeState(1);
						}
					}
				}
				if (select_ == 11)
				{
					int num32 = flg_svisorMax;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_svisorMax = 1;
					}
					if (num32 != flg_svisorMax)
					{
						for (int num33 = 0; num33 < apdat_.svisorData_.Length; num33++)
						{
							SupervisorData supervisorData = apdat_.svisorData_[num33];
							supervisorData.ChangeState(1);
						}
					}
				}
				if (select_ == 12)
				{
					int num34 = flg_LVMAX_svisor;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_LVMAX_svisor = 1;
					}
					if (num34 != flg_LVMAX_svisor)
					{
						for (int num35 = 0; num35 < apdat_.svisorData_.Length; num35++)
						{
							SupervisorData supervisorData2 = apdat_.svisorData_[num35];
							supervisorData2.SetLv(9);
						}
					}
				}
				if (select_ == 13)
				{
					int num36 = flg_LVMAX_fanAct;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_LVMAX_fanAct = 1;
					}
					if (num36 != flg_LVMAX_fanAct)
					{
						for (int num37 = 0; num37 < apdat_.fanActData_.Length; num37++)
						{
							FanActData fa_data = apdat_.fanActData_[num37];
							for (int num38 = 0; num38 < 6; num38++)
							{
								FanActData.RankUp(fa_data);
							}
						}
					}
				}
				if (select_ == 14)
				{
					int num39 = flg_LVMAX_sponsor;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_LVMAX_sponsor = 1;
					}
					if (num39 != flg_LVMAX_sponsor)
					{
						for (int num40 = 0; num40 < apdat_.sponsorData_.Length; num40++)
						{
							SponsorData sponsorData2 = apdat_.sponsorData_[num40];
							sponsorData2.satisfactPoint_ = 100;
						}
					}
				}
				if (select_ == 15)
				{
					int num41 = flg_LVMAX_tenant;
					if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576))
					{
						flg_LVMAX_tenant = 1;
					}
					if (num41 != flg_LVMAX_tenant)
					{
						for (int num42 = 0; num42 < apdat_.tenantData_.Length; num42++)
						{
							TenantData tenantData2 = apdat_.tenantData_[num42];
							tenantData2.SetLv(5);
						}
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 2)
			{
				int num43 = 11;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num43) % num43;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num43) % num43;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1 && (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyPulse(1048576)))
				{
					for (int num44 = 0; num44 < udat_.memberPDataist_.Size(); num44++)
					{
						PlayerData playerData5 = (PlayerData)udat_.memberPDataist_.ElementAt(num44);
						playerData5.auraPoint_ = 10000;
					}
					return formMan_.Pop();
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_formationId = (ch_formationId - 1 + 3) % 3;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_formationId = (ch_formationId + 1 + 3) % 3;
					}
				}
				if (select_ == 3)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						Team.FORMATION_TYPE_RATE[ch_formationId] = AppData.ClampMin(Team.FORMATION_TYPE_RATE[ch_formationId] - 10, -1000);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						Team.FORMATION_TYPE_RATE[ch_formationId] = AppData.ClampMax(Team.FORMATION_TYPE_RATE[ch_formationId] + 10, 1000);
					}
				}
				if (select_ == 4)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						Team.FORMATION_RATE_DEFFECNE = AppData.ClampMin(Team.FORMATION_RATE_DEFFECNE - 10, -1000);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						Team.FORMATION_RATE_DEFFECNE = AppData.ClampMax(Team.FORMATION_RATE_DEFFECNE + 10, 1000);
					}
				}
				if (select_ == 5)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_ameftoId = (ch_ameftoId - 1 + 3) % 3;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_ameftoId = (ch_ameftoId + 1 + 3) % 3;
					}
				}
				if (select_ == 6)
				{
					int num45 = Team.AMEFTO_PZ[ch_ameftoId];
					if (canvas_.CheckKeyRepeat(65536))
					{
						Team.AMEFTO_PZ[ch_ameftoId] = AppData.ClampMin(Team.AMEFTO_PZ[ch_ameftoId] - 10, -1000);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						Team.AMEFTO_PZ[ch_ameftoId] = AppData.ClampMax(Team.AMEFTO_PZ[ch_ameftoId] + 10, 1000);
					}
					if (num45 != Team.AMEFTO_PZ[ch_ameftoId] && udat_.teams_ != null && ch_ameftoId == apdat_.frmSoccer_.statusAmeftoMode_)
					{
						Log.Info("■DebugForm.java update_gameParam() データ修正処理");
						if (!udat_.gm_.halfTime)
						{
							udat_.teams_[0].amefto_pz = Team.AMEFTO_PZ[ch_ameftoId];
						}
						else
						{
							udat_.teams_[1].amefto_pz = Team.AMEFTO_PZ[ch_ameftoId];
						}
					}
				}
				if (select_ == 7)
				{
					int aMEFTO_ONECHARGETIME_PLAYER = Team.AMEFTO_ONECHARGETIME_PLAYER;
					if (canvas_.CheckKeyRepeat(65536))
					{
						Team.AMEFTO_ONECHARGETIME_PLAYER = AppData.ClampMin(Team.AMEFTO_ONECHARGETIME_PLAYER - 10, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						Team.AMEFTO_ONECHARGETIME_PLAYER = AppData.ClampMax(Team.AMEFTO_ONECHARGETIME_PLAYER + 10, 1000);
					}
					if (aMEFTO_ONECHARGETIME_PLAYER != Team.AMEFTO_ONECHARGETIME_PLAYER && udat_.teams_ != null)
					{
						if (!udat_.gm_.halfTime)
						{
							udat_.teams_[0].amefto_oneChargeTime = Team.AMEFTO_ONECHARGETIME_PLAYER;
						}
						else
						{
							udat_.teams_[1].amefto_oneChargeTime = Team.AMEFTO_ONECHARGETIME_PLAYER;
						}
					}
				}
				if (select_ == 8 && (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576)))
				{
					for (int num46 = 0; num46 < apdat_.fanBonus_.Length; num46++)
					{
						apdat_.fanBonus_[num46].ChangeState(1);
					}
				}
				if (select_ == 9)
				{
					int max2 = apdat_.trainingData_.Length;
					int num47 = 0;
					int num48 = 0;
					for (int num49 = 0; num49 < apdat_.trainingData_.Length; num49++)
					{
						TrainingData trainingData2 = apdat_.trainingData_[num49];
						if (trainingData2.state_ == 1)
						{
							num47++;
						}
					}
					num48 = num47;
					if (canvas_.CheckKeyRepeat(65536))
					{
						num48 = AppData.ClampMin(num48 - 1, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						num48 = AppData.ClampMax(num48 + 1, max2);
					}
					if (num47 != num48)
					{
						for (int num50 = 0; num50 < apdat_.trainingData_.Length; num50++)
						{
							TrainingData trainingData3 = apdat_.trainingData_[num50];
							trainingData3.state_ = 0;
							if (num50 < num48)
							{
								trainingData3.state_ = 1;
							}
						}
					}
				}
				if (select_ == 10 && (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144) || canvas_.CheckKeyRepeat(1048576)))
				{
					apdat_.sysSave_.ints[14] = 1 - apdat_.sysSave_.ints[14];
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 3)
			{
				int num51 = 13;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num51) % num51;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num51) % num51;
				}
				SoundPlayer instance = SoundPlayer.GetInstance();
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_bgmIndex = (ch_bgmIndex - 1 + AppData.SOUND_FILENAME[0].Length) % AppData.SOUND_FILENAME[0].Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_bgmIndex = (ch_bgmIndex + 1 + AppData.SOUND_FILENAME[0].Length) % AppData.SOUND_FILENAME[0].Length;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						apdat_.PlayBgm(ch_bgmIndex);
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_seIndex = (ch_seIndex - 1 + AppData.SOUND_FILENAME[1].Length) % AppData.SOUND_FILENAME[1].Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_seIndex = (ch_seIndex + 1 + AppData.SOUND_FILENAME[1].Length) % AppData.SOUND_FILENAME[1].Length;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						apdat_.PlaySe(ch_seIndex + AppData.SOUND_FILENAME[0].Length);
					}
				}
				if (select_ == 3)
				{
					int num52 = ch_bgmVolumeIndex;
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_bgmVolumeIndex = (ch_bgmVolumeIndex - 1 + 6) % 6;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_bgmVolumeIndex = (ch_bgmVolumeIndex + 1 + 6) % 6;
					}
					if (num52 != ch_bgmVolumeIndex)
					{
						apdat_.sysSave_.ints[3] = ch_bgmVolumeIndex;
						instance.SetVolume(0, apdat_.sysSave_.ints[3] * 51);
						instance.SetMute(0, apdat_.sysSave_.ints[3] == 0);
					}
				}
				if (select_ == 4)
				{
					int num53 = ch_seVolumeIndex;
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_seVolumeIndex = (ch_seVolumeIndex - 1 + 6) % 6;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_seVolumeIndex = (ch_seVolumeIndex + 1 + 6) % 6;
					}
					if (num53 != ch_seVolumeIndex)
					{
						apdat_.sysSave_.ints[4] = ch_seVolumeIndex;
						instance.SetVolume(1, apdat_.sysSave_.ints[4] * 51);
						instance.SetMute(1, apdat_.sysSave_.ints[4] == 0);
						instance.Play(apdat_.resSound_.sound[5]);
					}
				}
				if (select_ == 5)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						Ball.SOUND_RANDOM = 1 - Ball.SOUND_RANDOM;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						Ball.SOUND_RANDOM = 1 - Ball.SOUND_RANDOM;
					}
				}
				if (select_ == 6)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_kickSound[0] = (ch_kickSound[0] - 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_kickSound[0] = (ch_kickSound[0] + 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
				}
				if (select_ == 7)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_kickSound[1] = (ch_kickSound[1] - 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_kickSound[1] = (ch_kickSound[1] + 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
				}
				if (select_ == 8)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_kickSound[2] = (ch_kickSound[2] - 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_kickSound[2] = (ch_kickSound[2] + 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
				}
				if (select_ == 9)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_kickSound[3] = (ch_kickSound[3] - 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_kickSound[3] = (ch_kickSound[3] + 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
				}
				if (select_ == 10)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_kickSound[4] = (ch_kickSound[4] - 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_kickSound[4] = (ch_kickSound[4] + 1 + Ball.SOUND_KICK.Length) % Ball.SOUND_KICK.Length;
					}
				}
				if (select_ == 11)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_shootSound[0] = (ch_shootSound[0] - 1 + Ball.SOUND_SHOOT.Length) % Ball.SOUND_SHOOT.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_shootSound[0] = (ch_shootSound[0] + 1 + Ball.SOUND_SHOOT.Length) % Ball.SOUND_SHOOT.Length;
					}
				}
				if (select_ == 12)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_shootSound[1] = (ch_shootSound[1] - 1 + Ball.SOUND_SHOOT.Length) % Ball.SOUND_SHOOT.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_shootSound[1] = (ch_shootSound[1] + 1 + Ball.SOUND_SHOOT.Length) % Ball.SOUND_SHOOT.Length;
					}
				}
				if (canvas_.CheckKeyRepeat(65536) || canvas_.CheckKeyRepeat(262144))
				{
					Ball.SOUND_PASS[0] = ch_kickSound[0] + 8;
					Ball.SOUND_PASS[1] = ch_kickSound[0] + 8;
					Ball.SOUND_PASS[2] = ch_kickSound[1] + 8;
					Ball.SOUND_PASS[3] = ch_kickSound[2] + 8;
					Ball.SOUND_PASS[4] = ch_kickSound[2] + 8;
					Ball.SOUND_PASS[5] = ch_kickSound[3] + 8;
					Ball.SOUND_HEAD = ch_kickSound[4] + 8;
					Ball.SOUND_SHOOT_KICK = ch_shootSound[0] + 15;
					Ball.SOUND_SHOOT_HEAD = ch_shootSound[1] + 15;
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 4)
			{
				int num54 = 13;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num54) % num54;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num54) % num54;
				}
				if (canvas_.CheckKeyRepeat(2))
				{
					TEST_P1_POTENTIAL = 30;
					TEST_P2_POTENTIAL = 30;
				}
				if (canvas_.CheckKeyRepeat(4))
				{
					TEST_P1_POTENTIAL = 1000;
					TEST_P2_POTENTIAL = 1000;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
				}
				if (select_ == 2)
				{
					int num55 = apdat_.tData_.Length;
					int tEST_P1_TEAM = TEST_P1_TEAM;
					if (canvas_.CheckKeyRepeat(65536))
					{
						TEST_P1_TEAM = (TEST_P1_TEAM - 1 + num55) % num55;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						TEST_P1_TEAM = (TEST_P1_TEAM + 1 + num55) % num55;
					}
					if (tEST_P1_TEAM != TEST_P1_TEAM)
					{
						UpdateTestParam();
					}
				}
				if (select_ == 3)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						TEST_P1_POTENTIAL = AppData.ClampMin(TEST_P1_POTENTIAL - 10, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						TEST_P1_POTENTIAL = AppData.ClampMax(TEST_P1_POTENTIAL + 10, 1000);
					}
				}
				if (select_ == 4)
				{
					int num56 = apdat_.formationData_.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						TEST_P1_FORMATION = (TEST_P1_FORMATION - 1 + num56) % num56;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						TEST_P1_FORMATION = (TEST_P1_FORMATION + 1 + num56) % num56;
					}
				}
				if (select_ == 5)
				{
					int num57 = apdat_.tData_.Length;
					int tEST_P2_TEAM = TEST_P2_TEAM;
					if (canvas_.CheckKeyRepeat(65536))
					{
						TEST_P2_TEAM = (TEST_P2_TEAM - 1 + num57) % num57;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						TEST_P2_TEAM = (TEST_P2_TEAM + 1 + num57) % num57;
					}
					if (tEST_P2_TEAM != TEST_P2_TEAM)
					{
						UpdateTestParam();
					}
				}
				if (select_ == 6)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						TEST_P2_POTENTIAL = AppData.ClampMin(TEST_P2_POTENTIAL - 10, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						TEST_P2_POTENTIAL = AppData.ClampMax(TEST_P2_POTENTIAL + 10, 1000);
					}
				}
				if (select_ == 7)
				{
					int num58 = apdat_.formationData_.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						TEST_P2_FORMATION = (TEST_P2_FORMATION - 1 + num58) % num58;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						TEST_P2_FORMATION = (TEST_P2_FORMATION + 1 + num58) % num58;
					}
				}
				if (select_ == 8)
				{
					int num59 = apdat_.stadiumData_.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						TEST_STADIUM = (TEST_STADIUM - 1 + num59) % num59;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						TEST_STADIUM = (TEST_STADIUM + 1 + num59) % num59;
					}
				}
				if (select_ == 9)
				{
					int num60 = Map.MAP_GROUND_NAME.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						TEST_GROUND = (TEST_GROUND - 1 + num60) % num60;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						TEST_GROUND = (TEST_GROUND + 1 + num60) % num60;
					}
				}
				if (select_ == 10)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						TEST_STATE = (TEST_STATE - 1 + SoccerForm.TEST_STATE.Length) % SoccerForm.TEST_STATE.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						TEST_STATE = (TEST_STATE + 1 + SoccerForm.TEST_STATE.Length) % SoccerForm.TEST_STATE.Length;
					}
				}
				if (select_ == 11 && canvas_.CheckKeyPulse(1048576))
				{
					TEST_P1_POTENTIAL = 500;
					TEST_P2_POTENTIAL = 500;
				}
				if (select_ == 12 && canvas_.CheckKeyPulse(1048576))
				{
					TEST_P1_POTENTIAL = 1000;
					TEST_P2_POTENTIAL = 1000;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					TeamData teamData3 = apdat_.tData_[TEST_P1_TEAM];
					TeamData teamData4 = apdat_.tData_[TEST_P2_TEAM];
					teamData3.warPotential_ = TEST_P1_POTENTIAL;
					teamData4.warPotential_ = TEST_P2_POTENTIAL;
					teamData3.formation_ = TEST_P1_FORMATION;
					teamData4.formation_ = TEST_P2_FORMATION;
					SoccerForm.DEBUG_START_P1_TEAMID = TEST_P1_TEAM;
					SoccerForm.DEBUG_START_P2_TEAMID = TEST_P2_TEAM;
					udat_.selectFormationId_ = TEST_P1_FORMATION;
					teamData3.SetWarParamPhysical();
					teamData4.SetWarParamPhysical();
					formMan_.ChangeCurrentForm(apdat_.frmSoccer_);
					SoccerForm.DEBUG_START_STATE = SoccerForm.TEST_STATE[TEST_STATE];
					apdat_.frmSoccer_.udat_ = udat_;
					apdat_.frmSoccer_.apdat_ = apdat_;
					apdat_.frmSoccer_.ChangeState(SoccerForm.TEST_STATE[TEST_STATE]);
					try
					{
						Map.CreateMap(TEST_STADIUM, apdat_.stadiumData_[TEST_STADIUM].fileName_);
						apdat_.frmSoccer_.SetAllDraw();
					}
					catch (Exception)
					{
					}
					apdat_.frmSoccer_.SetCustomerNum();
					Map.MAP_GROUND = TEST_GROUND;
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 5)
			{
				if (PLAYER_SEB == 14)
				{
					temp_player.team_id_ = PLAYER_FRAME;
				}
				int num61 = 16;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num61) % num61;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num61) % num61;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						do
						{
							PLAYER_FACE--;
							if (PLAYER_FACE < 300)
							{
								PLAYER_FACE = apdat_.resHumanHead_.img.Length - 1;
							}
						}
						while (apdat_.resHumanHead_.img[PLAYER_FACE] == null);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						do
						{
							PLAYER_FACE++;
							if (PLAYER_FACE >= apdat_.resHumanHead_.img.Length - 1)
							{
								PLAYER_FACE = 300;
							}
						}
						while (apdat_.resHumanHead_.img[PLAYER_FACE] == null);
					}
				}
				if (select_ == 2)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						do
						{
							PLAYER_BODY--;
							if (PLAYER_BODY < 0)
							{
								PLAYER_BODY = 99;
							}
						}
						while (apdat_.resHuman2_.img[PLAYER_BODY + 100] == null);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						do
						{
							PLAYER_BODY++;
							if (PLAYER_BODY >= 100)
							{
								PLAYER_BODY = 0;
							}
						}
						while (apdat_.resHuman2_.img[PLAYER_BODY + 100] == null);
					}
				}
				if (select_ == 3)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						PLAYER_SEB = (PLAYER_SEB - 1 + 17) % 17;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						PLAYER_SEB = (PLAYER_SEB + 1 + 17) % 17;
					}
				}
				if (select_ == 4)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						PLAYER_DIRECT = (PLAYER_DIRECT - 1 + 4) % 4;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						PLAYER_DIRECT = (PLAYER_DIRECT + 1 + 4) % 4;
					}
				}
				if (select_ == 5)
				{
					int num62 = Player.SEBH_TO_RESHUMANSEB[PLAYER_SEB];
					int maxFrame = apdat_.resHuman2_.seb[num62].GetMaxFrame();
					if (canvas_.CheckKeyRepeat(65536))
					{
						PLAYER_FRAME = AppData.ClampMin(PLAYER_FRAME - 1, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						PLAYER_FRAME = AppData.ClampMax(PLAYER_FRAME + 1, maxFrame - 1);
					}
				}
				if (select_ == 6)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						PLAYER_ANIME = 1 - PLAYER_ANIME;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						PLAYER_ANIME = 1 - PLAYER_ANIME;
					}
				}
				if (select_ == 7)
				{
					int[] array2 = Player.body_off[PLAYER_SEB];
					if (canvas_.CheckKeyRepeat(65536))
					{
						array2[0] = AppData.ClampMin(array2[0] - 1, -100);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						array2[0] = AppData.ClampMax(array2[0] + 1, 100);
					}
				}
				if (select_ == 8)
				{
					int[] array3 = Player.body_off[PLAYER_SEB];
					if (canvas_.CheckKeyRepeat(65536))
					{
						array3[1] = AppData.ClampMin(array3[1] - 1, -100);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						array3[1] = AppData.ClampMax(array3[1] + 1, 100);
					}
				}
				if (select_ == 9)
				{
					int[] array4 = Player.head_off[PLAYER_SEB][PLAYER_DIRECT][PLAYER_FRAME];
					if (canvas_.CheckKeyRepeat(65536))
					{
						array4[0] = AppData.ClampMin(array4[0] - 1, -100);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						array4[0] = AppData.ClampMax(array4[0] + 1, 100);
					}
				}
				if (select_ == 10)
				{
					int[] array5 = Player.head_off[PLAYER_SEB][PLAYER_DIRECT][PLAYER_FRAME];
					if (canvas_.CheckKeyRepeat(65536))
					{
						array5[1] = AppData.ClampMin(array5[1] - 1, -100);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						array5[1] = AppData.ClampMax(array5[1] + 1, 100);
					}
				}
				if (select_ == 11)
				{
				}
				if (select_ == 12)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						PLAYER_TEAM = 1 - PLAYER_TEAM;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						PLAYER_TEAM = 1 - PLAYER_TEAM;
					}
				}
				if (select_ == 13)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						BALL = 1 - BALL;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						BALL = 1 - BALL;
					}
				}
				if (select_ == 14)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						PLAYER_EFINDEX = (PLAYER_EFINDEX - 1 + Player.PLAYER_EFLIST.Length) % Player.PLAYER_EFLIST.Length;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						PLAYER_EFINDEX = (PLAYER_EFINDEX + 1 + Player.PLAYER_EFLIST.Length) % Player.PLAYER_EFLIST.Length;
					}
				}
				if (select_ == 15)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						SHADOW = 1 - SHADOW;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						SHADOW = 1 - SHADOW;
					}
				}
				int num63 = Player.SEBH_TO_RESHUMANSEB[PLAYER_SEB];
				if (PLAYER_FRAME >= apdat_.resHuman2_.seb[num63].GetMaxFrame())
				{
					PLAYER_FRAME = apdat_.resHuman2_.seb[num63].GetMaxFrame() - 1;
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 6)
			{
				int num64 = 3;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num64) % num64;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num64) % num64;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					int tAreaLv_ = udat_.tAreaLv_;
					if (canvas_.CheckKeyRepeat(65536))
					{
						udat_.tAreaLv_ = AppData.ClampMin(udat_.tAreaLv_ - 1, 0);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						udat_.tAreaLv_ = AppData.ClampMax(udat_.tAreaLv_ + 1, 2);
					}
					if (tAreaLv_ != udat_.tAreaLv_)
					{
						apdat_.frmGame_.SetAllDraw();
					}
				}
				if (select_ == 2)
				{
					int tAreaRootNum_ = udat_.tAreaRootNum_;
					if (canvas_.CheckKeyRepeat(65536))
					{
						udat_.tAreaRootNum_ = AppData.ClampMin(udat_.tAreaRootNum_ - 1, 1);
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						udat_.tAreaRootNum_ = AppData.ClampMax(udat_.tAreaRootNum_ + 1, 2);
					}
					if (tAreaRootNum_ != udat_.tAreaRootNum_)
					{
						apdat_.mapMain_.UpdateTrainingCoatNum();
						apdat_.frmGame_.SetAllDraw();
					}
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					return formMan_.Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			return true;
		}

		public bool Update_window()
		{
			int num = 6;
			if (state_ == 0)
			{
				int num2 = 2;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num2) % num2;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num2) % num2;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					int num3 = udat_.tpplay_stock.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						tpp_index = (tpp_index - 1 + num3) % num3;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						tpp_index = (tpp_index + 1 + num3) % num3;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 1)
			{
				if (canvas_.CheckKeyRepeat(65536))
				{
					state_ = (state_ - 1 + num) % num;
				}
				if (canvas_.CheckKeyRepeat(262144))
				{
					state_ = (state_ + 1) % num;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					state_ = (state_ + 1) % num;
				}
				int num4 = udat_.tpplay_battleRecode.Size() - 13 + 1;
				if (canvas_.CheckKeyRepeat(131072))
				{
					tpp_scrollValue_ = (tpp_scrollValue_ - 1 + num4) % num4;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					tpp_scrollValue_ = (tpp_scrollValue_ + 1) % num4;
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 2)
			{
				int num5 = 15;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num5) % num5;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num5) % num5;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1 && canvas_.CheckKeyPulse(1048576))
				{
					return formMan_.Pop();
				}
				if (select_ == 2 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(26));
				}
				if (select_ == 3 && canvas_.CheckKeyPulse(1048576))
				{
					Camera.pos.x_ = MapMain.DEFAULT_INIT_CAMERA_POS[0];
					Camera.pos.y_ = MapMain.DEFAULT_INIT_CAMERA_POS[1];
					apdat_.camera.ChangeMode(0);
					formMan_.ChangeCurrentForm(apdat_.frmGame_);
				}
				if (select_ == 4 && canvas_.CheckKeyPulse(1048576))
				{
					formMan_.ChangeCurrentForm(apdat_.frmTitle_);
				}
				if (select_ != 5 || canvas_.CheckKeyPulse(1048576))
				{
				}
				if (select_ == 6 && canvas_.CheckKeyPulse(1048576))
				{
					formMan_.Pop();
					return Push(new SubForm(37));
				}
				if (select_ == 7 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(39));
				}
				if (select_ == 8 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(50));
				}
				if (select_ == 9 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(53));
				}
				if (select_ == 10 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(29));
				}
				if (select_ == 11 && canvas_.CheckKeyPulse(1048576))
				{
					formMan_.ChangeCurrentForm(apdat_.frmSoccer_);
				}
				if (select_ == 12 && canvas_.CheckKeyPulse(1048576))
				{
					formMan_.ChangeCurrentForm(apdat_.frmSoccer_);
					SoccerForm.DEBUG_START_STATE = 9;
					if (apdat_.frmSoccer_.apdat_ != null)
					{
						apdat_.frmSoccer_.ChangeState(9);
					}
				}
				if (select_ == 13 && canvas_.CheckKeyPulse(1048576))
				{
					formMan_.ChangeCurrentForm(apdat_.frmSoccer_);
					SoccerForm.DEBUG_START_STATE = 10;
					if (apdat_.frmSoccer_.apdat_ != null)
					{
						apdat_.frmSoccer_.ChangeState(10);
					}
				}
				if (select_ == 14 && canvas_.CheckKeyPulse(1048576))
				{
					formMan_.ChangeCurrentForm(apdat_.frmSoccer_);
					SoccerForm.DEBUG_START_STATE = 14;
					if (apdat_.frmSoccer_.apdat_ != null)
					{
						apdat_.frmSoccer_.ChangeState(14);
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 3)
			{
				int num6 = 11;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num6) % num6;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num6) % num6;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(38));
				}
				if (select_ == 2)
				{
					int num7 = SubForm.SCENE_CHANGE_NAME.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						ch_sChangeId = (ch_sChangeId - 1 + num7) % num7;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						ch_sChangeId = (ch_sChangeId + 1) % num7;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						formMan_.Pop();
						SubForm subForm = new SubForm(69);
						subForm.value1_ = ch_sChangeId;
						return Push(subForm);
					}
				}
				if (select_ == 3 && canvas_.CheckKeyPulse(1048576))
				{
					formMan_.Pop();
					apdat_.ExecEvent(1, null, null);
				}
				if (select_ == 4 && canvas_.CheckKeyPulse(1048576))
				{
					formMan_.Pop();
					apdat_.ExecEvent(2, null, null);
				}
				if (select_ == 5 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(71));
				}
				if (select_ == 6 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(52));
				}
				if (select_ == 7 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(72));
				}
				if (select_ == 8 && canvas_.CheckKeyPulse(1048576))
				{
					TeamData teamData = apdat_.tData_[0];
					apdat_.ExecEvent(110, teamData.name_ + "\t" + udat_.memberPDataist_.Size(), null);
					Pop();
				}
				if (select_ == 9 && canvas_.CheckKeyPulse(1048576))
				{
					SubForm subForm2 = new SubForm(64);
					subForm2.tenantDataId_ = 5;
					return Push(subForm2);
				}
				if (select_ == 10 && canvas_.CheckKeyPulse(1048576))
				{
					return Push(new SubForm(74));
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 4)
			{
				int num8 = 2;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num8) % num8;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num8) % num8;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (select_ == 1)
				{
					int num9 = apdat_.fanBonus_.Length;
					if (canvas_.CheckKeyRepeat(65536))
					{
						chFanBonusIndex = (chFanBonusIndex - 1 + num9) % num9;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						chFanBonusIndex = (chFanBonusIndex + 1) % num9;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						Pop();
						FanBonusData fanBonusData = apdat_.fanBonus_[chFanBonusIndex];
						udat_.gm_.teamId_[0] = 0;
						udat_.gm_.teamId_[1] = 1;
						TeamData teamData2 = apdat_.tData_[udat_.gm_.teamId_[0]];
						TeamData teamData3 = apdat_.tData_[udat_.gm_.teamId_[1]];
						if (fanBonusData.termMode_ == 0)
						{
							debug_testscore[0] = 5;
							debug_testscore[1] = 0;
							teamData2.warPotential_ = 0;
							teamData3.warPotential_ = 50 + 100 * fanBonusData.termModeIndex_;
						}
						if (fanBonusData.termMode_ == 13 && fanBonusData.termModeIndex_ == 0)
						{
							debug_testscore[0] = 1;
							debug_testscore[1] = 1;
							teamData2.warPotential_ = 0;
							teamData3.warPotential_ = 150;
						}
						if (fanBonusData.termMode_ == 13 && fanBonusData.termModeIndex_ == 1)
						{
							debug_testscore[0] = 1;
							debug_testscore[1] = 1;
							teamData2.warPotential_ = 150;
							teamData3.warPotential_ = 0;
						}
						if (fanBonusData.termMode_ == 1)
						{
							debug_testscore[0] = 0;
							debug_testscore[1] = 5;
							teamData2.warPotential_ = 50 + 100 * fanBonusData.termModeIndex_;
							teamData3.warPotential_ = 0;
						}
						if (fanBonusData.termMode_ == 2)
						{
							debug_testscore[0] = 5;
							debug_testscore[1] = 0;
							udat_.game_winConsecutive_ = fanBonusData.termModeValue_;
						}
						if (fanBonusData.termMode_ == 15)
						{
							debug_testscore[0] = 5;
							debug_testscore[1] = 0;
						}
						if (fanBonusData.termMode_ == 4)
						{
							debug_testscore[0] = 5;
							debug_testscore[1] = 5;
						}
						if (fanBonusData.termMode_ == 5)
						{
							PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(7);
							debug_scorevec.AddElement(new JInteger(playerData.id_));
							debug_scorevec.AddElement(new JInteger(playerData.id_));
							debug_scorevec.AddElement(new JInteger(playerData.id_));
						}
						if (fanBonusData.termMode_ == 6)
						{
							PlayerData playerData2 = (PlayerData)udat_.memberPDataist_.ElementAt(7);
							playerData2.matchNum_ = fanBonusData.termModeValue_;
						}
						if (fanBonusData.termMode_ == 7)
						{
							PlayerData playerData3 = (PlayerData)udat_.memberPDataist_.ElementAt(7);
							playerData3.game_goalConsecutive_ = fanBonusData.termModeValue_;
						}
						if (fanBonusData.termMode_ == 8)
						{
							PlayerData playerData4 = (PlayerData)udat_.memberPDataist_.ElementAt(7);
							playerData4.game_goalTotal_ = fanBonusData.termModeValue_;
						}
						if (fanBonusData.termMode_ == 11)
						{
							SupervisorData supervisorData = apdat_.svisorData_[udat_.selectSupervisorId_];
							supervisorData.winTotal_ = fanBonusData.termModeValue_;
						}
						if (fanBonusData.termMode_ == 12)
						{
							udat_.customer_total_ = fanBonusData.termModeValue_;
						}
						formMan_.ChangeCurrentForm(apdat_.frmSoccer_);
						udat_.gm_.score_[0] = debug_testscore[0];
						udat_.gm_.score_[1] = debug_testscore[1];
						SoccerForm.DEBUG_START_STATE = 9;
						if (apdat_.frmSoccer_.apdat_ != null)
						{
							apdat_.frmSoccer_.ChangeState(9);
						}
						udat_.gm_.score_[0] = debug_testscore[0];
						udat_.gm_.score_[1] = debug_testscore[1];
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			else if (state_ == 5)
			{
				int num10 = 1;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num10) % num10;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num10) % num10;
				}
				if (select_ == 0)
				{
					if (canvas_.CheckKeyRepeat(65536))
					{
						state_ = (state_ - 1 + num) % num;
					}
					if (canvas_.CheckKeyRepeat(262144))
					{
						state_ = (state_ + 1) % num;
					}
					if (canvas_.CheckKeyPulse(1048576))
					{
						state_ = (state_ + 1) % num;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return formMan_.Pop();
				}
			}
			return true;
		}

		public override void Draw(Graphics g)
		{
			if (mode == 0)
			{
				Draw_param(g);
			}
			else if (mode == 1)
			{
				Draw_gameParam(g);
			}
			else if (mode == 2)
			{
				Draw_window(g);
			}
		}

		public void Draw_param(Graphics g)
		{
			g.SetColor(Graphics.GetColorOfRGB(64, 0, 0));
			g.FillRect(0f, 0f, 240f, 240f);
			if (state_ == 0)
			{
				string[] array = new string[9] { "new_offence", "new_deffence", "new_offence_Pass", "passer2", "freerun2", "守備用", "ルーズボール用", "パッサー用", "味方フリーラン用" };
				string[][] array2 = new string[4][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"チーム",
						string.Empty + COATEVA_TEAM_ID
					},
					new string[2]
					{
						"種類",
						string.Empty + array[COATEVA_ROOT_INDEX]
					},
					new string[2]
					{
						"枠" + waku[COATEVA_WAKU],
						string.Empty
					}
				};
				try
				{
					Draw_coatEva(g);
				}
				catch (Exception)
				{
				}
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int i = 0; i < array2.Length; i++)
				{
					g.DrawString(array2[i][0], 20f, 10 + i * 14);
					g.DrawString(array2[i][1], 120f, 10 + i * 14);
					if (select_ == i)
					{
						g.DrawString("○", 5f, 10 + i * 14);
					}
				}
			}
			else if (state_ == 1)
			{
				string[][] array3 = new string[2][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "このデバッグ機能は現在使えません", "\u3000\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int j = 0; j < array3.Length; j++)
				{
					g.DrawString(array3[j][0], 20f, 10 + j * 14);
					g.DrawString(array3[j][1], 120f, 10 + j * 14);
					if (select_ == j)
					{
						g.DrawString("○", 5f, 10 + j * 14);
					}
				}
			}
			else if (state_ == 2)
			{
				string[][] array4 = new string[2][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "このデバッグ機能は現在使えません", "\u3000\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int k = 0; k < array4.Length; k++)
				{
					g.DrawString(array4[k][0], 20f, 10 + k * 14);
					g.DrawString(array4[k][1], 120f, 10 + k * 14);
					if (select_ == k)
					{
						g.DrawString("○", 5f, 10 + k * 14);
					}
				}
			}
			else if (state_ == 3)
			{
				string[][] array5 = new string[2][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "このデバッグ機能は現在使えません", "\u3000\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int l = 0; l < array5.Length; l++)
				{
					g.DrawString(array5[l][0], 20f, 10 + l * 14);
					g.DrawString(array5[l][1], 120f, 10 + l * 14);
					if (select_ == l)
					{
						g.DrawString("○", 5f, 10 + l * 14);
					}
				}
			}
			else if (state_ == 4)
			{
				string[][] array6 = new string[2][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "このデバッグ機能は現在使えません", "\u3000\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int m = 0; m < array6.Length; m++)
				{
					g.DrawString(array6[m][0], 20f, 10 + m * 14);
					g.DrawString(array6[m][1], 120f, 10 + m * 14);
					if (select_ == m)
					{
						g.DrawString("○", 5f, 10 + m * 14);
					}
				}
			}
			else if (state_ == 5)
			{
				string[][] array7 = new string[3][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"リーグ更新",
						"\u3000\u3000\u3000第" + (udat_.league_setu_ + 1) + "節"
					},
					new string[2]
					{
						"チーム数",
						"\u3000\u3000\u3000" + udat_.league_teamNum_
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int n = 0; n < array7.Length; n++)
				{
					g.DrawString(array7[n][0], 20f, 10 + n * 14);
					g.DrawString(array7[n][1], 120f, 10 + n * 14);
					if (select_ == n)
					{
						g.DrawString("○", 5f, 10 + n * 14);
					}
				}
			}
			else if (state_ == 6)
			{
				string[][] array8 = new string[2][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "このデバッグ機能は現在使えません", "\u3000\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num = 0; num < array8.Length; num++)
				{
					g.DrawString(array8[num][0], 20f, 10 + num * 14);
					g.DrawString(array8[num][1], 120f, 10 + num * 14);
					if (select_ == num)
					{
						g.DrawString("○", 5f, 10 + num * 14);
					}
				}
			}
			else if (state_ == 7)
			{
				string[][] array9 = new string[6][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "試合シーン(決定)", "\u3000\u3000\u3000" },
					new string[2]
					{
						"戦力値_味方",
						"\u3000\u3000\u3000" + udat_.gm_.team_potential_[0]
					},
					new string[2]
					{
						"戦力値_敵",
						"\u3000\u3000\u3000" + udat_.gm_.team_potential_[1]
					},
					new string[2] { "ﾌｫｰﾒｰｼｮﾝ_味方", "\u3000\u3000\u3000" },
					new string[2] { "ﾌｫｰﾒｰｼｮﾝ_敵", "\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num2 = 0; num2 < array9.Length; num2++)
				{
					g.DrawString(array9[num2][0], 20f, 10 + num2 * 14);
					g.DrawString(array9[num2][1], 120f, 10 + num2 * 14);
					if (select_ == num2)
					{
						g.DrawString("○", 5f, 10 + num2 * 14);
					}
				}
			}
			else if (state_ == 8)
			{
				int[] array10 = null;
				if (GOAL == 0)
				{
					array10 = Map.GOAL1_PARTS_POS[GOAL_INDEX];
				}
				if (GOAL == 1)
				{
					array10 = Map.GOAL2_PARTS_POS[GOAL_INDEX];
				}
				int num3 = 0;
				if (GOAL == 0)
				{
					num3 = Map.GOAL1_PARTS_GRA[GOAL_INDEX][2];
				}
				if (GOAL == 1)
				{
					num3 = Map.GOAL2_PARTS_GRA[GOAL_INDEX][2];
				}
				string[][] array11 = new string[6][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"GOAL",
						"\u3000\u3000\u3000" + GOAL
					},
					new string[2]
					{
						"GOAL_INDEX",
						"\u3000\u3000\u3000" + Map.GOAL_PARTS_NAME[GOAL_INDEX]
					},
					new string[2]
					{
						"OX",
						"\u3000\u3000\u3000" + array10[0]
					},
					new string[2]
					{
						"OY",
						"\u3000\u3000\u3000" + array10[1]
					},
					new string[2]
					{
						"depth",
						"\u3000\u3000\u3000" + num3
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num4 = 0; num4 < array11.Length; num4++)
				{
					g.DrawString(array11[num4][0], 20f, 10 + num4 * 14);
					g.DrawString(array11[num4][1], 120f, 10 + num4 * 14);
					if (select_ == num4)
					{
						g.DrawString("○", 5f, 10 + num4 * 14);
					}
				}
				Draw_goal(g);
			}
			else if (state_ == 9)
			{
				string[][] array12 = new string[2][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "このデバッグ機能は現在使えません", "\u3000\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num5 = 0; num5 < array12.Length; num5++)
				{
					g.DrawString(array12[num5][0], 20f, 10 + num5 * 14);
					g.DrawString(array12[num5][1], 120f, 10 + num5 * 14);
					if (select_ == num5)
					{
						g.DrawString("○", 5f, 10 + num5 * 14);
					}
				}
				Draw_goalMini(g);
			}
			else if (state_ == 10)
			{
				string value = AppData.talks_[ch_talks];
				string[] array13 = StringUtil.Split(value, "\t");
				string text = array13[0];
				string[][] array14 = new string[11][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"負荷チェック",
						"\u3000\u3000\u3000" + on_off[flg_loadCheck]
					},
					new string[2]
					{
						"ＦＰＳ表示",
						"\u3000\u3000\u3000" + on_off[UserData.DEBUG_DISPFPS]
					},
					new string[2]
					{
						"新聞チェック",
						string.Empty + ch_news + "/" + (apdat_.newsTitles_.Length - 1) + ":" + apdat_.newsTitles_[ch_news]
					},
					new string[2]
					{
						"会話文チェック",
						string.Empty + ch_talks + "/" + (AppData.talks_.Length - 1)
					},
					new string[2]
					{
						"ｲﾍﾞﾝﾄﾒｯｾｰｼﾞﾁｪｯｸ",
						string.Empty + ch_eventMsgs + "/" + (apdat_.eventMsgs_.Length - 1)
					},
					new string[2]
					{
						"ﾎﾟｯﾌﾟｱｯﾌﾟﾁｪｯｸ",
						"\u3000\u3000" + (ch_popUp + 1) + ":" + UserData.POPUP_MEM[ch_popUp]
					},
					new string[2]
					{
						"ゲーム要素open",
						"\u3000\u3000" + on_off[ch_elements]
					},
					new string[2]
					{
						"給料MAX",
						"\u3000\u3000" + on_off[UserData.DEBUG_SALLARY_MAX]
					},
					new string[2]
					{
						"フキダシ(" + fukidashiIndex_ + "/" + (UserData.EF_FEELPT_COMMENT_[0].Length - 1) + ")",
						string.Empty
					},
					new string[2]
					{
						"気持ち" + fukidashiFeel_ + "%",
						string.Empty
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num6 = 0; num6 < array14.Length; num6++)
				{
					int num7 = 10 + num6 * 14;
					g.DrawString(array14[num6][0], 20f, num7);
					g.DrawString(array14[num6][1], 120f, num7);
					if (select_ == num6)
					{
						g.DrawString("○", 5f, num7);
					}
					if (num6 == 8)
					{
						string text2 = UserData.EF_FEELPT_COMMENT_[0][fukidashiIndex_];
						apdat_.DrawFukidashiBalloon(g, text2, 160, num7 + 20, SubForm.SC_WINDOW_BLUE);
					}
					if (num6 == 9)
					{
						apdat_.DrawFukidashiKimochi(g, 160, num7 + 20 + 8, fukidashiFeel_, 78);
					}
				}
			}
			else
			{
				if (state_ != 11)
				{
					return;
				}
				string[][] array15 = new string[2][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "このデバッグ機能は現在使えません", "\u3000\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num8 = 0; num8 < array15.Length; num8++)
				{
					g.DrawString(array15[num8][0], 20f, 10 + num8 * 14);
					g.DrawString(array15[num8][1], 120f, 10 + num8 * 14);
					if (select_ == num8)
					{
						g.DrawString("○", 5f, 10 + num8 * 14);
					}
				}
			}
		}

		public void Draw_gameParam(Graphics g)
		{
			g.SetColor(Graphics.GetColorOfRGB(0, 64, 0));
			g.FillRect(0f, 0f, 240f, 240f);
			if (state_ == 0)
			{
				int num = 0;
				for (int i = 0; i < apdat_.formationData_.Length; i++)
				{
					FormationData formationData = apdat_.formationData_[i];
					if (formationData.state_ == 1)
					{
						num++;
					}
				}
				string[][] array = new string[16][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"    0:window"
					},
					new string[2]
					{
						"年",
						"\u3000\u3000" + (apdat_.save_.ints[1] + 1)
					},
					new string[2]
					{
						"月",
						"\u3000\u3000" + (apdat_.save_.ints[2] + 1)
					},
					new string[2]
					{
						"週",
						"\u3000\u3000" + (apdat_.save_.ints[3] + 1)
					},
					new string[2]
					{
						"お金(決定+)",
						"\u3000\u3000" + AppData.CommaSeparate(apdat_.save_.lngs[0]) + "万"
					},
					new string[2]
					{
						"試合時間_STOP",
						"\u3000\u3000" + on_off[SoccerForm.DEBUG_GAMETIMESTOP]
					},
					new string[2]
					{
						"試合速度_5KEY",
						"\u3000\u3000" + on_off[SoccerForm.DEBUG_GAMEMODE_5KEY]
					},
					new string[2]
					{
						"f_選手全員出現",
						"\u3000\u3000\u3000\u3000" + on_off[flg_playerAll]
					},
					new string[2]
					{
						"f_選手全員LVMAX",
						"\u3000\u3000\u3000\u3000" + on_off[flg_playerLvMax]
					},
					new string[2]
					{
						"f_選手全員ﾄﾚｰﾆﾝｸﾞP_MAX",
						"\u3000\u3000\u3000\u3000" + on_off[flg_playerTPMax]
					},
					new string[2]
					{
						"f_ﾌｫｰﾒｰｼｮﾝMAX",
						"\u3000\u3000\u3000\u3000" + on_off[flg_formationMax]
					},
					new string[2]
					{
						"f_総合力MAX",
						"\u3000\u3000\u3000\u3000" + on_off[flg_warPMax]
					},
					new string[2]
					{
						"研究ﾎﾟｲﾝﾄMAX",
						"\u3000\u3000\u3000\u3000" + on_off[flg_researchPointMax]
					},
					new string[2]
					{
						"負荷ﾁｪｯｸﾓｰﾄﾞ",
						"\u3000\u3000\u3000\u3000" + on_off[flg_loadCheck]
					},
					new string[2]
					{
						"ﾒﾆｭｰ全部open",
						"\u3000\u3000\u3000\u3000" + on_off[flg_menuOpen]
					},
					new string[2]
					{
						"ﾌｫｰﾒｰｼｮﾝ数",
						"\u3000\u3000\u3000\u3000" + num
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int j = 0; j < array.Length; j++)
				{
					g.DrawString(array[j][0], 20f, 10 + j * 14);
					g.DrawString(array[j][1], 120f, 10 + j * 14);
					if (select_ == j)
					{
						g.DrawString("○", 5f, 10 + j * 14);
					}
				}
			}
			else if (state_ == 1)
			{
				string[][] array2 = new string[16][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"    0:window"
					},
					new string[2]
					{
						"max_ファン",
						"\u3000\u3000\u3000" + on_off[max_fan]
					},
					new string[2]
					{
						"max_応援",
						"\u3000\u3000\u3000" + on_off[max_assist]
					},
					new string[2]
					{
						"max_知名度",
						"\u3000\u3000\u3000" + on_off[max_nameValue]
					},
					new string[2]
					{
						"f_ﾘｰｸﾞ戦MAX",
						"\u3000\u3000\u3000" + on_off[flg_leagueMax]
					},
					new string[2]
					{
						"f_ｶｯﾌﾟ戦MAX",
						"\u3000\u3000\u3000" + on_off[flg_qmMax]
					},
					new string[2]
					{
						"f_ﾄﾚｰﾆﾝｸﾞMAX",
						"\u3000\u3000\u3000" + on_off[flg_trainingMax]
					},
					new string[2]
					{
						"f_地域活動MAX",
						"\u3000\u3000\u3000" + on_off[flg_fanPMax]
					},
					new string[2]
					{
						"f_ﾄﾚｰﾄﾞMAX",
						"\u3000\u3000\u3000" + on_off[flg_tradeMax]
					},
					new string[2]
					{
						"f_施設MAX",
						"\u3000\u3000\u3000" + on_off[flg_tenantMax]
					},
					new string[2]
					{
						"f_ｽﾎﾟﾝｻｰMAX",
						"\u3000\u3000\u3000" + on_off[flg_sponsorMax]
					},
					new string[2]
					{
						"f_監督MAX",
						"\u3000\u3000\u3000" + on_off[flg_svisorMax]
					},
					new string[2]
					{
						"監督LV_MAX",
						"\u3000\u3000\u3000" + on_off[flg_LVMAX_svisor]
					},
					new string[2]
					{
						"地域活動RANK_MAX",
						"\u3000\u3000\u3000" + on_off[flg_LVMAX_fanAct]
					},
					new string[2]
					{
						"ｽﾎﾟﾝｻｰ満足度_MAX",
						"\u3000\u3000\u3000" + on_off[flg_LVMAX_sponsor]
					},
					new string[2]
					{
						"施設LV_MAX",
						"\u3000\u3000\u3000" + on_off[flg_LVMAX_tenant]
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int k = 0; k < array2.Length; k++)
				{
					g.DrawString(array2[k][0], 20f, 10 + k * 14);
					g.DrawString(array2[k][1], 120f, 10 + k * 14);
					if (select_ == k)
					{
						g.DrawString("○", 5f, 10 + k * 14);
					}
				}
			}
			else if (state_ == 2)
			{
				string[] array3 = new string[3] { "通常", "攻め重視", "守り重視" };
				string[] array4 = new string[3] { "通常", "ﾄﾞﾘﾌﾞﾙ主体", "ﾛﾝｸﾞﾊﾟｽ主体" };
				int num2 = 0;
				for (int l = 0; l < apdat_.trainingData_.Length; l++)
				{
					TrainingData trainingData = apdat_.trainingData_[l];
					if (trainingData.state_ == 1)
					{
						num2++;
					}
				}
				string[][] array5 = new string[11][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"オーラMAX",
						string.Empty
					},
					new string[2]
					{
						"ﾌｫｰﾒｰｼｮﾝﾀｲﾌﾟ",
						string.Empty + array3[ch_formationId]
					},
					new string[2]
					{
						"ﾀｲﾌﾟ別の補正値",
						string.Empty + Team.FORMATION_TYPE_RATE[ch_formationId]
					},
					new string[2]
					{
						"守備の補正",
						string.Empty + Team.FORMATION_RATE_DEFFECNE
					},
					new string[2]
					{
						"攻めのﾀｲﾌﾟ",
						"\u3000\u3000\u3000\u3000" + array4[ch_ameftoId]
					},
					new string[2]
					{
						"攻め:距離test",
						"\u3000\u3000\u3000\u3000" + Team.AMEFTO_PZ[ch_ameftoId]
					},
					new string[2]
					{
						"攻め:TIMEtest",
						"\u3000\u3000\u3000\u3000" + Team.AMEFTO_ONECHARGETIME_PLAYER
					},
					new string[2] { "チームの歴史", "\u3000\u3000\u3000\u3000" },
					new string[2]
					{
						"ﾄﾚｰﾆﾝｸﾞ数",
						"\u3000\u3000\u3000\u3000" + num2
					},
					new string[2]
					{
						"通貨単位の切り替え",
						"\u3000\u3000\u3000\u3000" + apdat_.sysSave_.ints[14]
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int m = 0; m < array5.Length; m++)
				{
					g.DrawString(array5[m][0], 20f, 10 + m * 14);
					g.DrawString(array5[m][1], 120f, 10 + m * 14);
					if (select_ == m)
					{
						g.DrawString("○", 5f, 10 + m * 14);
					}
				}
			}
			else if (state_ == 3)
			{
				string[][] array6 = new string[13][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"BGM_テスト",
						string.Empty + AppData.SOUND_FILENAME[0][ch_bgmIndex]
					},
					new string[2]
					{
						"SE_テスト",
						string.Empty + AppData.SOUND_FILENAME[1][ch_seIndex]
					},
					new string[2]
					{
						"BGM音量",
						string.Empty + ch_bgmVolumeIndex
					},
					new string[2]
					{
						"SE音量",
						string.Empty + ch_seVolumeIndex
					},
					new string[2]
					{
						"ｷｯｸ音ﾗﾝﾀﾞﾑ",
						string.Empty + on_off[Ball.SOUND_RANDOM]
					},
					new string[2]
					{
						"ﾊﾟｽ",
						string.Empty + AppData.SOUND_FILENAME[1][ch_kickSound[0] + 8 - AppData.SOUND_FILENAME[0].Length]
					},
					new string[2]
					{
						"ｸﾘｱｰ",
						string.Empty + AppData.SOUND_FILENAME[1][ch_kickSound[1] + 8 - AppData.SOUND_FILENAME[0].Length]
					},
					new string[2]
					{
						"ｾﾝﾀﾘﾝｸﾞ",
						string.Empty + AppData.SOUND_FILENAME[1][ch_kickSound[2] + 8 - AppData.SOUND_FILENAME[0].Length]
					},
					new string[2]
					{
						"ｽﾛｰｲﾝ",
						string.Empty + AppData.SOUND_FILENAME[1][ch_kickSound[3] + 8 - AppData.SOUND_FILENAME[0].Length]
					},
					new string[2]
					{
						"ﾍﾃﾞｨﾝｸﾞ",
						string.Empty + AppData.SOUND_FILENAME[1][ch_kickSound[4] + 8 - AppData.SOUND_FILENAME[0].Length]
					},
					new string[2]
					{
						"ｼｭｰﾄ_kick",
						string.Empty + AppData.SOUND_FILENAME[1][ch_shootSound[0] + 15 - AppData.SOUND_FILENAME[0].Length]
					},
					new string[2]
					{
						"ｼｭｰﾄ_head",
						string.Empty + AppData.SOUND_FILENAME[1][ch_shootSound[1] + 15 - AppData.SOUND_FILENAME[0].Length]
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int n = 0; n < array6.Length; n++)
				{
					g.DrawString(array6[n][0], 20f, 10 + n * 14);
					g.DrawString(array6[n][1], 120f, 10 + n * 14);
					if (select_ == n)
					{
						g.DrawString("○", 5f, 10 + n * 14);
					}
				}
			}
			else if (state_ == 4)
			{
				string[][] array7 = new string[13][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "試合スタート", "\u3000\u3000\u3000" },
					new string[2]
					{
						"Ｐ１チーム",
						"\u3000\u3000\u3000" + apdat_.tData_[TEST_P1_TEAM].name_
					},
					new string[2]
					{
						"Ｐ１戦力値",
						"\u3000\u3000\u3000" + TEST_P1_POTENTIAL
					},
					new string[2]
					{
						"Ｐ１ﾌｫｰﾒｰｼｮﾝ",
						"\u3000\u3000\u3000" + apdat_.formationData_[TEST_P1_FORMATION].name_
					},
					new string[2]
					{
						"Ｐ２チーム",
						"\u3000\u3000\u3000" + apdat_.tData_[TEST_P2_TEAM].name_
					},
					new string[2]
					{
						"Ｐ２戦力値",
						"\u3000\u3000\u3000" + TEST_P2_POTENTIAL
					},
					new string[2]
					{
						"Ｐ２ﾌｫｰﾒｰｼｮﾝ",
						"\u3000\u3000\u3000" + apdat_.formationData_[TEST_P2_FORMATION].name_
					},
					new string[2]
					{
						"スタジアム",
						"\u3000\u3000\u3000" + apdat_.stadiumData_[TEST_STADIUM].name_
					},
					new string[2]
					{
						"スタジアムの土",
						"\u3000\u3000\u3000" + Map.MAP_GROUND_NAME[TEST_GROUND]
					},
					new string[2] { "test_state", "\u3000\u3000\u3000" },
					new string[2] { "両チーム戦力500(決定)", "\u3000\u3000\u3000" },
					new string[2] { "両チーム戦力1000(決定)", "\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num3 = 0; num3 < array7.Length; num3++)
				{
					g.DrawString(array7[num3][0], 20f, 10 + num3 * 14);
					g.DrawString(array7[num3][1], 120f, 10 + num3 * 14);
					if (select_ == num3)
					{
						g.DrawString("○", 5f, 10 + num3 * 14);
					}
				}
			}
			else if (state_ == 5)
			{
				int[] array8 = Player.body_off[PLAYER_SEB];
				int[] array9 = Player.head_off[PLAYER_SEB][PLAYER_DIRECT][PLAYER_FRAME];
				string[][] array10 = new string[16][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"face",
						string.Empty + (PLAYER_FACE - 300)
					},
					new string[2]
					{
						"body",
						string.Empty + PLAYER_BODY
					},
					new string[2]
					{
						"seb",
						string.Empty + Player.SEB_NAME[PLAYER_SEB]
					},
					new string[2]
					{
						"direct",
						string.Empty + PLAYER_DIRECT
					},
					new string[2]
					{
						"frame",
						string.Empty + PLAYER_FRAME
					},
					new string[2]
					{
						"anime",
						string.Empty + on_off[PLAYER_ANIME]
					},
					new string[2]
					{
						"body_ox",
						string.Empty + array8[0]
					},
					new string[2]
					{
						"body_oy",
						string.Empty + array8[1]
					},
					new string[2]
					{
						"head_ox",
						string.Empty + array9[0]
					},
					new string[2]
					{
						"head_oy",
						string.Empty + array9[1]
					},
					new string[2]
					{
						"head_f",
						string.Empty
					},
					new string[2]
					{
						"team",
						string.Empty + PLAYER_TEAM
					},
					new string[2]
					{
						"ball",
						string.Empty + on_off[BALL]
					},
					new string[2]
					{
						"eff",
						string.Empty + Player.PLAYER_EFLIST_NAME[PLAYER_EFINDEX]
					},
					new string[2]
					{
						"shadow",
						string.Empty + on_off[SHADOW]
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num4 = 0; num4 < array10.Length; num4++)
				{
					g.DrawString(array10[num4][0], 20f, 10 + num4 * 14);
					g.DrawString(array10[num4][1], 120f, 10 + num4 * 14);
					if (select_ == num4)
					{
						g.DrawString("○", 5f, 10 + num4 * 14);
					}
				}
				Draw_player(g);
			}
			else
			{
				if (state_ != 6)
				{
					return;
				}
				string[][] array11 = new string[3][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"メイン画面_練習環境Lv",
						"\u3000\u3000\u3000" + udat_.tAreaLv_
					},
					new string[2]
					{
						"メイン画面_コート数",
						"\u3000\u3000\u3000" + udat_.tAreaRootNum_
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num5 = 0; num5 < array11.Length; num5++)
				{
					g.DrawString(array11[num5][0], 20f, 10 + num5 * 14);
					g.DrawString(array11[num5][1], 120f, 10 + num5 * 14);
					if (select_ == num5)
					{
						g.DrawString("○", 5f, 10 + num5 * 14);
					}
				}
			}
		}

		public void Draw_window(Graphics g)
		{
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 64));
			g.FillRect(0f, 0f, 240f, 240f);
			if (state_ == 0)
			{
				int[] array = udat_.tpplay_stock[tpp_index];
				string[][] array2 = new string[11][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"チェック年数",
						"\u3000\u3000\u3000" + tpp_index + "年前"
					},
					new string[2]
					{
						"t_資金",
						"\u3000\u3000\u3000" + array[0]
					},
					new string[2]
					{
						"t_勝ち",
						"\u3000\u3000\u3000" + array[1]
					},
					new string[2]
					{
						"t_引分",
						"\u3000\u3000\u3000" + array[2]
					},
					new string[2]
					{
						"t_負け",
						"\u3000\u3000\u3000" + array[3]
					},
					new string[2]
					{
						"t_研究_ﾌｧﾝ",
						"\u3000\u3000\u3000" + array[4]
					},
					new string[2]
					{
						"t_研究_ｱｲﾃﾞｱ",
						"\u3000\u3000\u3000" + array[5]
					},
					new string[2]
					{
						"t_研究_ﾌｨｼﾞｶﾙ",
						"\u3000\u3000\u3000" + array[6]
					},
					new string[2]
					{
						"t_研究_戦術",
						"\u3000\u3000\u3000" + array[7]
					},
					new string[2]
					{
						"t_ﾁｰﾑ評価",
						"\u3000\u3000\u3000" + array[8]
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int i = 0; i < array2.Length; i++)
				{
					g.DrawString(array2[i][0], 20f, 10 + i * 14);
					g.DrawString(array2[i][1], 120f, 10 + i * 14);
					if (select_ == i)
					{
						g.DrawString("○", 5f, 10 + i * 14);
					}
				}
			}
			else if (state_ == 1)
			{
				string[][] array3 = new string[2][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "試合No  対戦相手      スコア  結果", "\u3000\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int j = 0; j < array3.Length; j++)
				{
					g.DrawString(array3[j][0], 20f, 10 + j * 14);
					g.DrawString(array3[j][1], 120f, 10 + j * 14);
				}
				int num = 38;
				int num2 = 0;
				for (int k = tpp_scrollValue_; k < udat_.tpplay_battleRecode.Size(); k++)
				{
					int num3 = k - tpp_scrollValue_;
					if (num3 >= 13)
					{
						break;
					}
					int[] array4 = (int[])udat_.tpplay_battleRecode.ElementAt(k);
					int num4 = array4[1] / 10000;
					int num5 = array4[1] / 100 % 100;
					int num6 = array4[1] % 100;
					num2 = num + num3 * 14;
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
					g.DrawString(array4[0] + string.Empty, 47f, num2, 4);
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
					TeamData teamData = apdat_.tData_[num4];
					g.DrawString(teamData.name_, 66f, num2);
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
					g.DrawString(num5 + "-" + num6, 166f, num2, 2);
					string empty = string.Empty;
					if (num5 > num6)
					{
						g.SetColor(SubForm.SC_WINDOW_BLUE);
						empty = "○";
					}
					else if (num5 == num6)
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
						empty = "△";
					}
					else
					{
						g.SetColor(SubForm.SC_WINDOW_RED);
						empty = "×";
					}
					g.DrawString(empty, 206f, num2);
				}
				apdat_.DrawVerticalScroll2(g, 226, 38, 3, 181, tpp_scrollValue_, udat_.tpplay_battleRecode.Size() - 1, 13);
			}
			else if (state_ == 2)
			{
				string[][] array5 = new string[15][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "試合の画面遷移テスト(決定)", "\u3000\u3000\u3000" },
					new string[2] { "新編集画面テスト(決定)", "\u3000\u3000\u3000" },
					new string[2] { "メイン画面テスト(決定)", "\u3000\u3000\u3000" },
					new string[2] { "タイトル(決定)", "\u3000\u3000\u3000" },
					new string[2] { "他の試合結果(決定)", "\u3000\u3000\u3000" },
					new string[2] { "試合終了(決定)", "\u3000\u3000\u3000" },
					new string[2] { "ファン活動(決定)", "\u3000\u3000\u3000" },
					new string[2] { "監督(決定)", "\u3000\u3000\u3000" },
					new string[2] { "ﾄﾚｰﾄﾞﾘｽﾄ(決定)", "\u3000\u3000\u3000" },
					new string[2] { "ﾒﾝﾊﾞｰ一覧(決定)", "\u3000\u3000\u3000" },
					new string[2] { "ﾎﾞｰﾅｽ画面のﾁｪｯｸ(決定)_選手", "\u3000\u3000\u3000" },
					new string[2] { "ﾎﾞｰﾅｽ画面のﾁｪｯｸ(決定)_ﾌｧﾝ", "\u3000\u3000\u3000" },
					new string[2] { "ﾎﾞｰﾅｽ画面のﾁｪｯｸ(決定)_収支情報", "\u3000\u3000\u3000" },
					new string[2] { "ﾎﾞｰﾅｽ画面のﾁｪｯｸ(決定)_選手new[", "\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int l = 0; l < array5.Length; l++)
				{
					g.DrawString(array5[l][0], 20f, 10 + l * 14);
					g.DrawString(array5[l][1], 120f, 10 + l * 14);
					if (select_ == l)
					{
						g.DrawString("○", 5f, 10 + l * 14);
					}
				}
			}
			else if (state_ == 3)
			{
				string[][] array6 = new string[11][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2] { "優勝シーン", "\u3000\u3000\u3000" },
					new string[2]
					{
						"シーン切り替え",
						"\u3000\u3000\u3000" + SubForm.SCENE_CHANGE_NAME[ch_sChangeId]
					},
					new string[2] { "イベント_喜び", "\u3000\u3000\u3000" },
					new string[2] { "イベント_悲しみ", "\u3000\u3000\u3000" },
					new string[2] { "ゲームスタート", "\u3000\u3000\u3000" },
					new string[2] { "年末人事_監督", "\u3000\u3000\u3000" },
					new string[2] { "年末人事_選手", "\u3000\u3000\u3000" },
					new string[2] { "年末人事", "\u3000\u3000\u3000" },
					new string[2] { "施設のLVUP", "\u3000\u3000\u3000" },
					new string[2] { "収支報告", "\u3000\u3000\u3000" }
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int m = 0; m < array6.Length; m++)
				{
					g.DrawString(array6[m][0], 20f, 10 + m * 14);
					g.DrawString(array6[m][1], 120f, 10 + m * 14);
					if (select_ == m)
					{
						g.DrawString("○", 5f, 10 + m * 14);
					}
				}
			}
			else if (state_ == 4)
			{
				string[][] array7 = new string[2][]
				{
					new string[2]
					{
						"<< 次のページへ" + state_ + " >> ",
						"\u3000\u3000\u3000\u3000"
					},
					new string[2]
					{
						"ﾌｧﾝﾎﾞｰﾅｽのﾁｪｯｸ",
						"\u3000\u3000\u3000" + apdat_.fanBonus_[chFanBonusIndex].message_
					}
				};
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int n = 0; n < array7.Length; n++)
				{
					g.DrawString(array7[n][0], 20f, 10 + n * 14);
					g.DrawString(array7[n][1], 120f, 10 + n * 14);
					if (select_ == n)
					{
						g.DrawString("○", 5f, 10 + n * 14);
					}
				}
			}
			else
			{
				if (state_ != 5)
				{
					return;
				}
				string[][] array8 = new string[1][] { new string[2]
				{
					"<< 次のページへ" + state_ + " >> ",
					"\u3000\u3000\u3000\u3000"
				} };
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num7 = 0; num7 < array8.Length; num7++)
				{
					g.DrawString(array8[num7][0], 20f, 10 + num7 * 14);
					g.DrawString(array8[num7][1], 120f, 10 + num7 * 14);
					if (select_ == num7)
					{
						g.DrawString("○", 5f, 10 + num7 * 14);
					}
				}
				string value = apdat_.sysSave_.strs[11];
				string[] array9 = StringUtil.Split(value, "\n");
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
				for (int num8 = 0; num8 < array9.Length; num8++)
				{
					g.DrawString(array9[num8], 20f, 10 + num8 * 14 + 14);
				}
			}
		}

		public override bool Resume()
		{
			return true;
		}

		public void Draw_coatEva(Graphics g)
		{
			int[][] array = null;
			Team team = udat_.teams_[COATEVA_TEAM_ID];
			Team team2 = udat_.teams_[1 - COATEVA_TEAM_ID];
			if (COATEVA_ROOT_INDEX == 0)
			{
				array = team.coatEva_new_offence;
			}
			if (COATEVA_ROOT_INDEX == 1)
			{
				array = team.coatEva_new_deffence;
			}
			if (COATEVA_ROOT_INDEX == 2)
			{
				array = team.coatEva_new_offence_passer;
			}
			if (COATEVA_ROOT_INDEX == 3)
			{
				array = team.coatEva_passer2;
			}
			if (COATEVA_ROOT_INDEX == 4)
			{
				array = team.coatEva_freeRun2;
			}
			if (COATEVA_ROOT_INDEX == 5)
			{
				array = team.coatEva_defence;
			}
			if (COATEVA_ROOT_INDEX == 6)
			{
				array = team.coatEva_looseBall;
			}
			if (COATEVA_ROOT_INDEX == 7)
			{
				array = team.coatEva_passer;
			}
			if (COATEVA_ROOT_INDEX == 8)
			{
				array = team.coatEva_freeRun;
			}
			int num = 10;
			int num2 = 120 - Map.MAPCHIP_COAT_WNUM / 2 * num;
			int num3 = 60;
			global::game.Player passer;
int num10;
int num4 = 0;
			for (int num5 = Map.MAPCHIP_COAT_HNUM - 1; num5 >= 0; num5--)
			{
				for (int i = 0; i < Map.MAPCHIP_COAT_WNUM; i++)
				{
					int num6 = array[num5][i];
					if (num6 < 20)
					{
						g.SetColor(Graphics.GetColorOfRGB(0, 0, 255));
					}
					else if (num6 < 40)
					{
						g.SetColor(Graphics.GetColorOfRGB(0, 255, 255));
					}
					else if (num6 < 60)
					{
						g.SetColor(Graphics.GetColorOfRGB(0, 255, 0));
					}
					else if (num6 < 80)
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
					}
					else
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 0, 0));
					}
					int num7 = num2 + i * num;
					int num8 = num3 + num4 * num;
					g.FillRect(num7, num8, num, num);
				}
				num4++;
			}
			if (COATEVA_WAKU == 0)
			{
				g.SetColor(Graphics.GetColorOfRGB(128, 128, 128));
				for (int num9 = Map.MAPCHIP_COAT_HNUM - 1; num9 >= 0; num9--)
				{
					for (int j = 0; j < Map.MAPCHIP_COAT_WNUM; j++)
					{
						int num7 = num2 + j * num;
						int num8 = num3 + num9 * num;
						g.DrawRect(num7, num8, num, num);
					}
				}
			}
			passer = team.GetPasser();
			if (passer == null)
			{
				passer = team2.GetPasser();
			}
			g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
			if (passer != null)
			{
				int num7 = num2 + passer.address_[0] * num;
				int num8 = num3 + (Map.MAPCHIP_COAT_HNUM - 1 - passer.address_[1]) * num;
				g.DrawRect(num7, num8, num, num);
			}
			num10 = -1;
			if (udat_.ball_.owner_ != null)
			{
				num10 = udat_.ball_.owner_.team_id_;
			}
			else if (udat_.ball_.lastOwner_ != null)
			{
				num10 = udat_.ball_.lastOwner_.team_id_;
			}
			if (num10 == -1)
			{
				return;
			}
			Team team3 = udat_.teams_[0];
			Team team4 = udat_.teams_[1];
			int num11 = num2;
			int num12 = num2 + Map.MAPCHIP_COAT_WNUM * num;
			int num13 = (int)GameUtil.RateConvert(team4.fPos_[1] + team4.fPos_[3] / 2f, 900f, -900f, num3, num3 + Map.MAPCHIP_COAT_HNUM * num, true);
			int num14 = (int)GameUtil.RateConvert(team4.fPos_[1] - team4.fPos_[3] / 2f, 900f, -900f, num3, num3 + Map.MAPCHIP_COAT_HNUM * num, true);
			num11 -= 2;
			num12 += 2;
			g.SetColor(Graphics.GetColorOfRGB(255, 168, 255));
			g.DrawRect(num11, num13, num12 - num11, num14 - num13);
			g.DrawRect(num11 + 1, num13 + 1, num12 - num11 - 2, num14 - num13 - 2);
			num11 = num2;
			num12 = num2 + Map.MAPCHIP_COAT_WNUM * num;
			num13 = (int)GameUtil.RateConvert(team3.fPos_[1] + team3.fPos_[3] / 2f, 900f, -900f, num3, num3 + Map.MAPCHIP_COAT_HNUM * num, true);
			num14 = (int)GameUtil.RateConvert(team3.fPos_[1] - team3.fPos_[3] / 2f, 900f, -900f, num3, num3 + Map.MAPCHIP_COAT_HNUM * num, true);
			g.SetColor(Graphics.GetColorOfRGB(255, 206, 16));
			g.DrawRect(num11, num13, num12 - num11, num14 - num13);
			g.DrawRect(num11 + 1, num13 + 1, num12 - num11 - 2, num14 - num13 - 2);
			int num15 = num2;
			int num16 = num2 + Map.MAPCHIP_COAT_WNUM * num;
			int num17 = (int)GameUtil.RateConvert(team.amefto_goalPos_[0].z_, 900f, -900f, num3, num3 + Map.MAPCHIP_COAT_HNUM * num, true);
			if (frame_ % 4 < 2)
			{
				if (team.team_id_ == 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
				}
				else
				{
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 255));
				}
			}
			else if (team.team_id_ == 0)
			{
				g.SetColor(Graphics.GetColorOfRGB(200, 255, 100));
			}
			else
			{
				g.SetColor(Graphics.GetColorOfRGB(100, 100, 255));
			}
			if (num10 != -1)
			{
				g.DrawLine(num15, num17, num16, num17);
				g.DrawLine(num15, num17 + 1, num16, num17 + 1);
			}
			int num18 = 6;
			for (int num19 = udat_.teams_.Length - 1; num19 >= 0; num19--)
			{
				for (int k = 0; k < udat_.teams_[num19].players_.Length; k++)
				{
					Player player = udat_.teams_[num19].players_[k];
					int num7 = (int)GameUtil.RateConvert(player.pos_.x_, -450f, 450f, num2, num2 + Map.MAPCHIP_COAT_WNUM * num, true);
					int num8 = (int)GameUtil.RateConvert(player.pos_.z_, 900f, -900f, num3, num3 + Map.MAPCHIP_COAT_HNUM * num, true);
					if (num19 == 1)
					{
						g.SetColor(Graphics.GetColorOfRGB(0, 128, 128));
					}
					else
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 122, 0));
					}
					g.FillArc(num7, num8, num18, num18, 0f, 359f);
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
					g.DrawArc(num7, num8, num18 - 1, num18 - 1, 0f, 359f);
					if (BitUtil.Check(player.flag_, 4))
					{
						g.SetColor(Graphics.GetColorOfRGB(255, 0, 255));
						g.FillArc(num7 - 2, num8 - 2, num18 + 4, num18 + 4, 0f, 359f);
					}
				}
			}
		}

		public void Draw_moveArea(Graphics g, int team_id, int p_id)
		{
			Team team = udat_.teams_[team_id];
			int num = 10;
			int num2 = 120 - Map.MAPCHIP_COAT_WNUM / 2 * num;
			int num3 = 60;
			global::game.Player player;
global::game.Player player2;
int num4 = 0;
			g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
			for (int num5 = Map.MAPCHIP_COAT_HNUM - 1; num5 >= 0; num5--)
			{
				for (int i = 0; i < Map.MAPCHIP_COAT_WNUM; i++)
				{
					int num6 = num2 + i * num;
					int num7 = num3 + num5 * num;
					g.DrawRect(num6, num7, num, num);
				}
			}
			formationRect_global_pos[0].x_ = team.fPos_[0] - team.fPos_[2] / 2f;
			formationRect_global_pos[0].z_ = team.fPos_[1] + team.fPos_[3] / 2f;
			formationRect_global_pos[1].x_ = team.fPos_[0] + team.fPos_[2] / 2f;
			formationRect_global_pos[1].z_ = team.fPos_[1] + team.fPos_[3] / 2f;
			formationRect_global_pos[2].x_ = team.fPos_[0] + team.fPos_[2] / 2f;
			formationRect_global_pos[2].z_ = team.fPos_[1] - team.fPos_[3] / 2f;
			formationRect_global_pos[3].x_ = team.fPos_[0] - team.fPos_[2] / 2f;
			formationRect_global_pos[3].z_ = team.fPos_[1] - team.fPos_[3] / 2f;
			for (int j = 0; j < formationRect_global_pos.Length; j++)
			{
				formationRect_camera_pos[j][0] = (int)GameUtil.RateConvert(formationRect_global_pos[j].x_, -450f, 450f, num2, num2 + Map.MAPCHIP_COAT_WNUM * num, true);
				formationRect_camera_pos[j][1] = (int)GameUtil.RateConvert(formationRect_global_pos[j].z_, 900f, -900f, num3, num3 + Map.MAPCHIP_COAT_HNUM * num, true);
			}
			g.SetColor(Graphics.GetColorOfRGB(255, 0, 0, 128));
			g.FillRect(formationRect_camera_pos[0][0], formationRect_camera_pos[0][1], formationRect_camera_pos[1][0] - formationRect_camera_pos[0][0], formationRect_camera_pos[2][1] - formationRect_camera_pos[0][1]);
			for (int k = 0; k < team.players_.Length; k++)
			{
				player = team.players_[k];
				if (k != p_id)
				{
					continue;
				}
				int num8 = 0;
				g.SetColor(Graphics.GetColorOfRGB(0, 255, 0, 220));
				for (int num9 = player.formation_address_[0][1]; num9 >= player.formation_address_[3][1]; num9--)
				{
					for (int l = player.formation_address_[0][0]; l <= player.formation_address_[1][0]; l++)
					{
						int num6 = num2 + l * num;
						int num7 = num3 + (Map.MAPCHIP_COAT_HNUM - num9 - 1) * num;
						g.FillRect(num6, num7, num, num);
					}
					num8++;
				}
			}
			int num10 = 6;
			for (int m = 0; m < team.players_.Length; m++)
			{
				player2 = team.players_[m];
				int num6 = (int)GameUtil.RateConvert(player2.pos_.x_, -450f, 450f, num2, num2 + Map.MAPCHIP_COAT_WNUM * num, true);
				int num7 = (int)GameUtil.RateConvert(player2.pos_.z_, 900f, -900f, num3, num3 + Map.MAPCHIP_COAT_HNUM * num, true);
				if (BitUtil.Check(player2.flag_, 4))
				{
					g.SetColor(Graphics.GetColorOfRGB(255, 0, 255));
					g.FillArc(num6 - 2, num7 - 2, num10 + 4, num10 + 4, 0f, 359f);
				}
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
				g.FillArc(num6, num7, num10, num10, 0f, 359f);
			}
		}

		public void Draw_player(Graphics g)
		{
			int pLAYER_SEB = PLAYER_SEB;
			int pLAYER_DIRECT = PLAYER_DIRECT;
			int head_id = PLAYER_FACE - 300;
			int num = PLAYER_BODY;
			if (pLAYER_SEB == 14)
			{
				num += 100;
			}
			temp_player.direct_ = pLAYER_DIRECT;
			temp_player.sebCnt_ = (temp_player.sebCnt_ + 1) % int.MaxValue;
			if (PLAYER_ANIME == 1)
			{
				temp_player.flag_ = BitUtil.On(temp_player.flag_, 2);
			}
			else
			{
				temp_player.flag_ = BitUtil.Off(temp_player.flag_, 2);
			}
			temp_player.team_id_ = PLAYER_TEAM;
			temp_player.role_ = 3;
			int num2 = 0;
			if (PLAYER_ANIME == 1) { num2 = temp_player.SetDrawParam(head_id, num, pLAYER_SEB, -1); } else { num2 = temp_player.SetDrawParam(head_id, num, pLAYER_SEB, PLAYER_FRAME); }
			int num3 = 194;
			int num4 = 58;
			int num5 = 50;
			g.SetColor(Graphics.GetColorOfRGB(200, 200, 200));
			g.FillRect(num3 - num5 / 2, num4 - num5 / 2, num5, num5);
			if (sortDraw.Size() > 0)
			{
				sortDraw.RemoveAllElements();
			}
			int num6 = num4 + temp_player.draw_param[6];
			sortDraw.AddElement(new int[2] { 0, num6 });
			int num7 = 0;
			int num8 = 0;
			if (BALL == 1)
			{
				int num9 = num4;
				num7 = Ball.SAVING_OFF[PLAYER_TEAM][PLAYER_FRAME][0];
				num8 = Ball.SAVING_OFF[PLAYER_TEAM][PLAYER_FRAME][1];
				sortDraw.AddElement(new int[2] { 1, num9 });
			}
			int num10 = 0;
			int num11 = 0;
			if (SHADOW == 1)
			{
				int num12 = -1000;
				UserData.Update_45v1();
				UserData.Update_78v2();
				UserData.PrintOut();
				num10 = UserData.TEST_VALUE1;
				num11 = UserData.TEST_VALUE2;
				sortDraw.AddElement(new int[2] { 2, num12 });
			}
			int num13 = 0;
			int num14 = 0;
			if (PLAYER_EFINDEX != 0)
			{
				int num15 = 1000000;
				UserData.Update_45v1();
				UserData.Update_78v2();
				UserData.PrintOut();
				num13 = UserData.TEST_VALUE1;
				num14 = UserData.TEST_VALUE2;
				sortDraw.AddElement(new int[2] { 3, num15 });
			}
			for (int i = 0; i < sortDraw.Size() - 1; i++)
			{
				for (int num16 = sortDraw.Size() - 1; num16 > i; num16--)
				{
					int[] array = (int[])sortDraw.ElementAt(i);
					int[] array2 = (int[])sortDraw.ElementAt(num16);
					if (array2[1] < array[1])
					{
						sortDraw.SetElementAt(array2, i);
						sortDraw.SetElementAt(array, num16);
					}
				}
			}
			for (int j = 0; j < sortDraw.Size(); j++)
			{
				int[] array3 = (int[])sortDraw.ElementAt(j);
				if (array3[0] == 0)
				{
					temp_player.Draw(g, num3, num4, num2, false);
				}
				else if (array3[0] == 1)
				{
					apdat_.DrawSeb(g, num3 + num7, num4 + num8, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 0);
				}
				else if (array3[0] == 2)
				{
					apdat_.DrawSeb(g, num3 + num10, num4 + num11, apdat_.resCom2_.seb[0], apdat_.resCom2_.img[2], 4);
				}
				else if (array3[0] == 3)
				{
					apdat_.DrawSeb(g, num3 + num13, num4 + num14, apdat_.resCom2_.seb[6], apdat_.resCom2_.img[9], 0);
				}
			}
		}

		public void Draw_goal(Graphics g)
		{
			apdat_.ClearZSortObjects();
			int num5;
int num;
int num2;
int num3;


			num5 = num = num2 = num3 = 0;
			int num4 = 0;
			num5 = -13;
			num = 17;
			camera_pos[0] = 120;
			camera_pos[1] = 160;
			if (GOAL == 0)
			{
				for (int i = 0; i < Map.GOAL1_PARTS_POS.Length; i++)
				{
					num2 = camera_pos[0] + Map.GOAL1_PARTS_POS[i][0] + num5;
					num3 = camera_pos[1] + Map.GOAL1_PARTS_POS[i][1] + num;
					num4 = camera_pos[1] + Map.GOAL1_PARTS_POS[i][1] + num + Map.GOAL1_PARTS_GRA[i][2];
					apdat_.InsertGameObject(8, 0, num4, Map.GOAL1_PARTS_GRA[i][0], Map.GOAL1_PARTS_GRA[i][1], 0, num2, num3, false, 0);
					if (i == GOAL_INDEX)
					{
						Image image = apdat_.resCom2_.img[Map.GOAL1_PARTS_GRA[i][0]];
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						g.DrawRect(num2 - image.GetWidth() / 2, num3 - image.GetHeight(), image.GetWidth(), image.GetHeight());
					}
				}
			}
			num5 = 12;
			num = 1;
			if (GOAL == 1)
			{
				for (int j = 0; j < Map.GOAL2_PARTS_POS.Length; j++)
				{
					num2 = camera_pos[0] + Map.GOAL2_PARTS_POS[j][0] + num5;
					num3 = camera_pos[1] + Map.GOAL2_PARTS_POS[j][1] + num;
					num4 = camera_pos[1] + Map.GOAL2_PARTS_POS[j][1] + num + Map.GOAL2_PARTS_GRA[j][2];
					apdat_.InsertGameObject(8, 0, num4, Map.GOAL2_PARTS_GRA[j][0], Map.GOAL2_PARTS_GRA[j][1], 0, num2, num3, false, 0);
					if (j == GOAL_INDEX)
					{
						Image image2 = apdat_.resCom2_.img[Map.GOAL1_PARTS_GRA[j][0]];
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						g.DrawRect(num2 - image2.GetWidth() / 2, num3 - image2.GetHeight(), image2.GetWidth(), image2.GetHeight());
					}
				}
			}
			apdat_.DrawZSortObjects(g, true);
		}

		public void Draw_goalMini(Graphics g)
		{
			apdat_.ClearZSortObjects();
			int num5;
int num;
int num2;
int num3;


			num5 = num = num2 = num3 = 0;
			int num4 = 0;
			num5 = -13;
			num = 17;
			camera_pos[0] = 120;
			camera_pos[1] = 160;
			if (GOAL == 0)
			{
				for (int i = 0; i < MapMain.GOAL1_PARTS_POS.Length; i++)
				{
					num2 = camera_pos[0] + MapMain.GOAL1_PARTS_POS[i][0] + num5;
					num3 = camera_pos[1] + MapMain.GOAL1_PARTS_POS[i][1] + num;
					num4 = camera_pos[1] + MapMain.GOAL1_PARTS_POS[i][1] + num + MapMain.GOAL1_PARTS_GRA[i][2];
					apdat_.InsertGameObject(8, 0, num4, MapMain.GOAL1_PARTS_GRA[i][0], MapMain.GOAL1_PARTS_GRA[i][1], 0, num2, num3, false, 0);
					if (i == GOAL_INDEX)
					{
						Image image = apdat_.resCom2_.img[MapMain.GOAL1_PARTS_GRA[i][0]];
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						g.DrawRect(num2 - image.GetWidth() / 2, num3 - image.GetHeight(), image.GetWidth(), image.GetHeight());
					}
				}
			}
			num5 = 12;
			num = 1;
			if (GOAL == 1)
			{
				for (int j = 0; j < MapMain.GOAL2_PARTS_POS.Length; j++)
				{
					num2 = camera_pos[0] + MapMain.GOAL2_PARTS_POS[j][0] + num5;
					num3 = camera_pos[1] + MapMain.GOAL2_PARTS_POS[j][1] + num;
					num4 = camera_pos[1] + MapMain.GOAL2_PARTS_POS[j][1] + num + MapMain.GOAL2_PARTS_GRA[j][2];
					apdat_.InsertGameObject(8, 0, num4, MapMain.GOAL2_PARTS_GRA[j][0], MapMain.GOAL2_PARTS_GRA[j][1], 0, num2, num3, false, 0);
					if (j == GOAL_INDEX)
					{
						Image image2 = apdat_.resCom2_.img[MapMain.GOAL1_PARTS_GRA[j][0]];
						g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
						g.DrawRect(num2 - image2.GetWidth() / 2, num3 - image2.GetHeight(), image2.GetWidth(), image2.GetHeight());
					}
				}
			}
			apdat_.DrawZSortObjects(g, true);
		}

		public void UpdateTestParam()
		{
			TeamData teamData = apdat_.tData_[TEST_P1_TEAM];
			TeamData teamData2 = apdat_.tData_[TEST_P2_TEAM];
			TEST_P1_POTENTIAL = teamData.warPotential_;
			TEST_P2_POTENTIAL = teamData2.warPotential_;
			TEST_P1_FORMATION = teamData.formation_;
			TEST_P2_FORMATION = teamData2.formation_;
		}

		public override int GetTouchValue(int type, int index)
		{
			return 0;
		}

		public override void SetTouchValue(int type, int value)
		{
		}
	}
}
