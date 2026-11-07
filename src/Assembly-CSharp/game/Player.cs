using data;
using form;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace game
{
	public class Player
	{
		public const int FLG_NOTDISP = 1;

		public const int FLG_ANIME = 2;

		public const int FLG_HAVEBALL = 4;

		public const int FLG_SAVING = 8;

		public const int FLG_SLIDING = 16;

		public const int FLG_SLIDING_HIT = 32;

		public const int FLG_HEAD = 64;

		public const int FLG_HEAD_HIT = 128;

		public const int FLG_THROWIN = 256;

		public const int FLG_GOALKICK = 512;

		public const int FLG_CONNERKICK = 1024;

		public const int FLG_GOAL_SCORER = 2048;

		public const int FLG_GOAL_SCORER_SUB = 4096;

		public const int FLG_GOAL_ENEMY = 8192;

		public const int FLG_DOWN = 16384;

		public const int FLG_VOLLEY = 32768;

		public const int FLG_VOLLEY_HIT = 65536;

		public const int FLG_TRA_LIFTING = 131072;

		public const int FLG_AURA = 262144;

		public const int DIRECTBOX_MAX = 4;

		public const int SEB_H_WALK = 0;

		public const int SEB_H_KICK = 1;

		public const int SEB_H_HEAD = 2;

		public const int SEB_H_GOAL_GLAD0 = 3;

		public const int SEB_H_THROWIN = 4;

		public const int SEB_H_SLIDING = 5;

		public const int SEB_H_TRAP1 = 6;

		public const int SEB_H_TRAP2 = 7;

		public const int SEB_H_GOAL_GLAD1 = 8;

		public const int SEB_H_GOAL_GLAD2 = 9;

		public const int SEB_H_RUN = 10;

		public const int SEB_H_GOAL_SAD = 11;

		public const int SEB_H_DOWN = 12;

		public const int SEB_H_VOLLEY = 13;

		public const int SEB_H_SAVING = 14;

		public const int SEB_H_FRONT = 15;

		public const int SEB_H_GOAL_GLAD0_SV = 16;

		public const int SEB_H_END = 17;

		public const float SEB_H_SAVING_DISY = 10f;

		public const int DRAW_HEAD_OX = 0;

		public const int DRAW_HEAD_OY = 1;

		public const int DRAW_HEAD_SEB = 2;

		public const int DRAW_HEAD_IMG = 3;

		public const int DRAW_HEAD_F = 4;

		public const int DRAW_BODY_OX = 5;

		public const int DRAW_BODY_OY = 6;

		public const int DRAW_BODY_SEB = 7;

		public const int DRAW_BODY_IMG = 8;

		public const int DRAW_BODY_F = 9;

		public const int DRAW_HAT_IMG = 10;

		public const int LAYER_HEAD_FRONT = 0;

		public const int LAYER_HEAD_BACK = 1;

		public const int IMG_W = 14;

		public const int IMG_H = 20;

		public const int SCR_MOVE = 0;

		public const int SCR_WAIT = 1;

		public const int SCR_CHANGE_STATE = 2;

		public const int SCR_CHANGE_SEB = 3;

		public const int SCR_CHANGE_DIRECT = 4;

		public const int SCR_FLAG = 5;

		public const int SCR_FLAG_ON = 6;

		public const int SCR_FLAG_OFF = 7;

		public const int SCR_KICK = 8;

		public const int SCR_SAVING = 9;

		public const int SCR_SLIDING = 10;

		public const int SCR_HEAD = 11;

		public const int SCR_VOLLEY = 12;

		public const int SCR_THROWIN = 13;

		public const int SCR_GOAL_GLAD = 14;

		public const int SCR_GOAL_SAD = 15;

		public const int SCR_DOWN = 16;

		public const int ACTID_FREERUN = 0;

		public const int ACTID_RECEIVEBALL = 1;

		public const int ACTID_CHARGE = 2;

		public const int ACTID_DRIBBLE = 3;

		public const int ACTID_GK = 4;

		public const int ACTID_STOP = 5;

		public const int ACTID_SEB = 6;

		public const int ACTID_KICKOFF_PASS = 7;

		public const int ACTID_THROWIN_PASS = 8;

		public const int ACTID_GOALKICK = 9;

		public const int ACTID_CONNERKICK = 10;

		public const int ACTID_GOAL_SCORER = 11;

		public const int ACTID_GOAL_SCORER_SUB = 12;

		public const int ACTID_GOAL_ENEMY = 13;

		public const int EVA_PASS_INFO_GROUND = 0;

		public const int EVA_PASS_INFO_JAMP = 1;

		public const int EVA_PASS_JAMP_RATE_MIN = 80;

		public const int EVA_PASS_JAMP_RATE_MAX = 95;

		public const int EVA_PASS_JAMP_RATE_SPACE_MIN = 80;

		public const int EVA_PASS_JAMP_RATE_SPACE_MAX = 95;

		public const int OBJTYPE_PLAYER = 0;

		public const int OBJTYPE_TPLAYER = 1;

		public const int OBJTYPE_SVISOR = 2;

		public const int MOVEDIRECTDATA_MAX = 15;

		private const float w_dis = 1f;

		private const float w_rad = 1f;

		private const float w_waitTime = 0.3f;

		private const float w_physicalHp = 0.3f;

		private const float w_direct = 0.6f;

		private const float w_teamRisk = 0.2f;

		private const float w_myPosEva = 0.3f;

		private const int EFCHARGE_W = 10;

		private const int EFCHARGE_H = 10;

		public static AppData apdat_;

		public static Canvas canvas_;

		public static UserData udat_;

		public int obj_type_;

		public int pData_id_;

		public int p_id_;

		public int team_id_;

		public int tData_id_;

		public int role_;

		public int frame_;

		public int direct_;

		public int seb_;

		public int sebCnt_;

		public int faceImg_;

		public int bodyImg_;

		public Vector script_;

		public Vector3D pos_;

		public Vector3D shadowPos_;

		public Vector3D goalPos_;

		public Vector3D formationGo_;

		public Vector3D v_;

		public int[] formation_;

		public int[][] formation_param_;

		public int[][] formation_address_;

		public int[] address_;

		public int[] mostEvaAddres_;

		public int[] screen_pos_;

		public int flag_;

		public int state_;

		public int stateCnt_;

		public int actId_;

		public int coliState_;

		public int coliBallState_;

		public int eva_pos_;

		public int eva_receivePos_;

		public int ballHaveTime_;

		public int ballDribbleTime_;

		public int notMoveCnt_;

		public int dribbleStopCnt_;

		public float speed_min;

		public float speed_max;

		public float pass_data;

		public float throuhPass_data;

		public float longPass_data;

		public float shoot_min;

		public float shoot_max;

		public float shootV0y_min;

		public float shootV0y_max;

		public int headShootUpRadRate;

		public int chargeDamage_body;

		public int chargeDamage_sliding;

		public int physicalHp_data;

		public float trapball_data;

		public int catch_data;

		public float keeperJmap_data;

		public int slidingRate_data;

		public float jamp_data;

		public float jamptop_data;

		public int shootTrap_data;

		public int physical_hp_;

		public int[] directBox_;

		public static readonly string[] SEB_NAME = new string[0];

		public static readonly int[] SEBH_TO_RESHUMANSEB = new int[17]
		{
			52, 24, 20, 4, 36, 32, 40, 44, 8, 12,
			28, 16, 0, 48, 56, 80, 81
		};

		public static int SEB_H_WALK_ASPEED = 4;

		public static int[] SEB_H_KICK_ANIME = new int[3] { 2, 6, 9 };

		public static int[] SEB_H_SAVING_ANIME = new int[2] { 22, 32 };

		public static int[] SEB_H_SLIDING_ANIME = new int[2] { 6, 12 };

		public static int SEB_H_HEAD_JAMPTIME = 14;

		public static int[] SEB_H_HEAD_ANIME = new int[2] { 6, SEB_H_HEAD_JAMPTIME };

		public static int SEB_H_VOLLEY_JAMPTIME = 16;

		public static int[] SEB_H_VOLLEY_ANIME = new int[2] { 6, SEB_H_VOLLEY_JAMPTIME };

		public static int[] SEB_H_THROWIN_ANIME = new int[2] { 0, 5 };

		public static int[] SEB_H_GOALGLAD_ANIME = new int[2] { 12, 22 };

		public static int SEB_H_DOWN_ANIME = 60;

		public static int[] SEB_H_TRAP1_ANIME = new int[2] { 4, 6 };

		public static int[] SEB_H_TRAP2_ANIME = new int[2] { 6, 8 };

		public static int[] SEB_H_GOALGLAD2_ANIME = new int[4] { 4, 8, 12, 16 };

		public static int[][] body_off = new int[18][]
		{
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
			new int[2],
			new int[2],
			new int[2],
			new int[2],
			new int[2]
		};

		public static int[][][][] head_off = new int[17][][][]
		{
			new int[4][][]
			{
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -5 },
					new int[2] { 0, -6 },
					new int[2] { 0, -5 }
				},
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -5 },
					new int[2] { 0, -6 },
					new int[2] { 0, -5 }
				},
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -5 },
					new int[2] { 0, -6 },
					new int[2] { 0, -5 }
				},
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -5 },
					new int[2] { 0, -6 },
					new int[2] { 0, -5 }
				}
			},
			new int[4][][]
			{
				new int[3][]
				{
					new int[2] { -1, -5 },
					new int[2] { 1, -4 },
					new int[2] { 0, -6 }
				},
				new int[3][]
				{
					new int[2] { 0, -5 },
					new int[2] { 1, -4 },
					new int[2] { 0, -6 }
				},
				new int[3][]
				{
					new int[2] { 0, -5 },
					new int[2] { -1, -4 },
					new int[2] { 0, -6 }
				},
				new int[3][]
				{
					new int[2] { 1, -5 },
					new int[2] { -1, -4 },
					new int[2] { 0, -6 }
				}
			},
			new int[4][][]
			{
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { 2, -7 }
				},
				new int[2][]
				{
					new int[2] { 1, -6 },
					new int[2] { 1, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { -1, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { -2, -7 }
				}
			},
			new int[4][][]
			{
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { -1, -6 },
					new int[2] { 0, -6 }
				}
			},
			new int[4][][]
			{
				new int[2][]
				{
					new int[2] { 1, -6 },
					new int[2] { 2, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { 3, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { -2, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { -1, -6 }
				}
			},
			new int[4][][]
			{
				new int[2][]
				{
					new int[2] { -1, -3 },
					new int[2] { -1, -1 }
				},
				new int[2][]
				{
					new int[2] { 0, -3 },
					new int[2] { 0, -1 }
				},
				new int[2][]
				{
					new int[2] { 0, -3 },
					new int[2] { 0, -1 }
				},
				new int[2][]
				{
					new int[2] { 1, -3 },
					new int[2] { 1, -1 }
				}
			},
			new int[4][][]
			{
				new int[2][]
				{
					new int[2] { 0, -5 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -5 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -5 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -5 },
					new int[2] { 0, -6 }
				}
			},
			new int[4][][]
			{
				new int[2][]
				{
					new int[2] { -1, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { -2, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 2, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 2, -6 },
					new int[2] { 0, -6 }
				}
			},
			new int[4][][]
			{
				new int[1][] { new int[2] { 0, -5 } },
				new int[1][] { new int[2] { 0, -5 } },
				new int[1][] { new int[2] { 0, -5 } },
				new int[1][] { new int[2] { 0, -5 } }
			},
			new int[4][][]
			{
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 1, -6 },
					new int[2] { 0, -6 },
					new int[2] { 0, -6 }
				},
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 1, -6 },
					new int[2] { 0, -6 },
					new int[2] { 1, -5 }
				},
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { -1, -6 },
					new int[2] { 0, -6 },
					new int[2] { -1, -5 }
				},
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { -1, -6 },
					new int[2] { 0, -6 },
					new int[2] { 0, -6 }
				}
			},
			new int[4][][]
			{
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -7 },
					new int[2] { 0, -6 },
					new int[2] { 0, -7 }
				},
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -7 },
					new int[2] { 0, -6 },
					new int[2] { 0, -7 }
				},
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -7 },
					new int[2] { 0, -6 },
					new int[2] { 0, -7 }
				},
				new int[4][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -7 },
					new int[2] { 0, -6 },
					new int[2] { 0, -7 }
				}
			},
			new int[4][][]
			{
				new int[1][] { new int[2] { 5, -2 } },
				new int[1][] { new int[2] { 6, -2 } },
				new int[1][] { new int[2] { -6, -2 } },
				new int[1][] { new int[2] { -5, -2 } }
			},
			new int[4][][]
			{
				new int[1][] { new int[2] { 2, -2 } },
				new int[1][] { new int[2] { 4, 1 } },
				new int[1][] { new int[2] { -4, 1 } },
				new int[1][] { new int[2] { -3, -2 } }
			},
			new int[4][][]
			{
				new int[1][] { new int[2] { -1, -6 } },
				new int[1][] { new int[2] { -1, -6 } },
				new int[1][] { new int[2] { 1, -6 } },
				new int[1][] { new int[2] { 1, -6 } }
			},
			new int[4][][]
			{
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 4, 1 },
					new int[2] { 4, -1 }
				},
				new int[2][]
				{
					new int[2] { -1, -6 },
					new int[2] { -1, -6 }
				},
				new int[2][]
				{
					new int[2] { -2, -5 },
					new int[2] { -1, -5 }
				}
			},
			new int[4][][]
			{
				new int[2][]
				{
					new int[2] { 1, -6 },
					new int[2] { 1, -6 }
				},
				new int[2][]
				{
					new int[2] { 1, -6 },
					new int[2] { 1, -6 }
				},
				new int[2][]
				{
					new int[2] { 1, -6 },
					new int[2] { 1, -6 }
				},
				new int[2][]
				{
					new int[2] { 1, -6 },
					new int[2] { 1, -6 }
				}
			},
			new int[4][][]
			{
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { 0, -6 },
					new int[2] { 0, -6 }
				},
				new int[2][]
				{
					new int[2] { -1, -6 },
					new int[2] { 0, -6 }
				}
			}
		};

		public int[] draw_param = new int[11];

		public int[] camera_pos = new int[2];

		public int insertScriptIndex_;

		public Vector3D old_pos = new Vector3D();

		public Vector3D temp_vec = new Vector3D();

		public Vector3D goalStatePos_ = new Vector3D();

		public float temp_len;

		private static Vector3D vec1 = new Vector3D();

		private static Vector3D vec2 = new Vector3D();

		public Vector3D seb_startPos = new Vector3D();

		public Vector3D seb_v = new Vector3D();

		public float seb_ay;

		public static readonly int[] PLAYER_EFLIST = new int[2] { 0, 6 };

		public static readonly string[] PLAYER_EFLIST_NAME = new string[0];

		public static readonly int[][] EF_SHOOT_OFF = new int[4][]
		{
			new int[2] { 9, 6 },
			new int[2] { 9, 6 },
			new int[2] { -11, 5 },
			new int[2] { -11, 6 }
		};

		public static readonly int[][][] EF_KEEPER_OFF = new int[2][][]
		{
			new int[4][]
			{
				new int[2] { 5, 15 },
				new int[2] { 10, 10 },
				new int[2] { -2, 15 },
				new int[2] { -2, 17 }
			},
			new int[4][]
			{
				new int[2] { 5, 15 },
				new int[2] { 6, 6 },
				new int[2] { -2, 15 },
				new int[2] { -7, 13 }
			}
		};

		public int[] moveDirectBox = new int[15];

		private int[] direct_dis_passer = new int[5];

		private int[] direct_dis_free = new int[5] { 2, 10, 15, 20, 20 };

		private static int[] temp_direct_s = new int[2];

		private static Vector3D temp_afr_mostEvaPos = new Vector3D();

		private static int[] temp_afr_mostEvaSPos = new int[2];

		private static int[] temp_address = new int[2];

		private static float w_freeRun_formation = 0.4f;

		private static float w_freeRun_afrMostEvaPos = 0.6f;

		private static float w_dribble = 0f;

		private static float w_pass = 0f;

		private static float w_pass_through = 0f;

		private static float w_clear = 0f;

		private static float w_shoot = 0f;

		private static float w_centering = 0f;

		private static float[] eva_dribble = new float[9];

		private static float[][] eva_pass = JTool.MakeArray<float[][]>(new int[2] { 11, 2 });

		private static int[][] eva_pass_info = JTool.MakeArray<int[][]>(new int[2] { 11, 2 });

		private static int[][] eva_pass_info_onEnemy = JTool.MakeArray<int[][]>(new int[2] { 11, 2 });

		private static float eva_clear;

		private static float eva_shoot;

		private static float eva_centering;

		private static float actDecide_max;

		private static int actDecide_index;

		internal int[][] DIRECT_RECT = new int[9][]
		{
			new int[2] { -1, 1 },
			new int[2] { 0, 1 },
			new int[2] { 1, 1 },
			new int[2] { -1, 0 },
			new int[2],
			new int[2] { 1, 0 },
			new int[2] { -1, -1 },
			new int[2] { 0, -1 },
			new int[2] { 1, -1 }
		};

		public static Vector3D chColPos = new Vector3D();

		private static Vector hitList = new Vector();

		private static Vector3D[] temp_moveAreaPos;

		private static int[] camera_pos_shadow = new int[2];

		private static int[] camera_pos_sliding = new int[2];

		private static Vector3D CH_BALLSUBPOS = new Vector3D();

		private static int[] CH_BALL_DIRECT = new int[2];

		private static Vector3D st_a_vec = new Vector3D();

		private static Vector3D st_b_vec = new Vector3D();

		public Player(int obj_type)
		{
			apdat_ = AppData.GetInstance();
			canvas_ = Canvas.GetInstance();
			obj_type_ = obj_type;
			pData_id_ = 1;
			bodyImg_ = 100;
		}

		public void Init(int pData_id, int formation_index, int teamId, int tDataId)
		{
			apdat_ = AppData.GetInstance();
			canvas_ = Canvas.GetInstance();
			udat_ = UserData.GetInstance();
			pData_id_ = pData_id;
			p_id_ = formation_index;
			team_id_ = teamId;
			tData_id_ = tDataId;
			role_ = 0;
			frame_ = 0;
			direct_ = 0;
			seb_ = 0;
			faceImg_ = 0;
			bodyImg_ = 100;
			script_ = new Vector();
			pos_ = new Vector3D();
			shadowPos_ = new Vector3D();
			goalPos_ = new Vector3D();
			formationGo_ = new Vector3D();
			v_ = new Vector3D();
			formation_ = new int[2];
			formation_param_ = JTool.MakeArray<int[][]>(new int[2] { 5, 2 });
			formation_address_ = JTool.MakeArray<int[][]>(new int[2] { 5, 2 });
			address_ = new int[2];
			mostEvaAddres_ = new int[2];
			screen_pos_ = new int[2];
			notMoveCnt_ = 0;
			flag_ = 0;
			state_ = 0;
			stateCnt_ = 0;
			actId_ = 0;
			coliState_ = 0;
			coliBallState_ = 0;
			flag_ = BitUtil.On(flag_, 2);
			physical_hp_ = PlayerData.GetPhysicalHpMax(this);
			directBox_ = new int[4];
			role_ = apdat_.pData_[pData_id].role_;
		}

		public void Clear()
		{
			pos_.Set(0f, 0f, 0f);
			frame_ = 0;
			seb_ = 0;
			if (script_.Size() > 0)
			{
				script_.RemoveAllElements();
			}
			flag_ = 0;
			state_ = 0;
			stateCnt_ = 0;
			flag_ = BitUtil.On(flag_, 2);
		}

		public bool Update()
		{
			frame_ = (frame_ + 1) % int.MaxValue;
			sebCnt_ = (sebCnt_ + 1) % int.MaxValue;
			int num = direct_;
			Player passer = udat_.teams_[0].GetPasser();
			if (passer == null)
			{
				passer = udat_.teams_[0].players_[udat_.teams_[0].players_.Length - 1];
			}
			Update_moveArea();
			if (script_.Size() > 0)
			{
				Script();
			}
			else
			{
				actId_ = DecideAction();
				Act(actId_);
			}
			if (role_ == 0)
			{
				if (team_id_ == 0)
				{
					if (pos_.z_ > Map.COAT_GOAL1_GLOBALPOS[1].z_)
					{
						pos_.z_ = Map.COAT_GOAL1_GLOBALPOS[1].z_;
					}
					if (pos_.z_ < -900f)
					{
						pos_.z_ = -900f;
					}
				}
				else
				{
					if (pos_.z_ < Map.COAT_GOAL2_GLOBALPOS[1].z_)
					{
						pos_.z_ = Map.COAT_GOAL2_GLOBALPOS[1].z_;
					}
					if (pos_.z_ > 900f)
					{
						pos_.z_ = 900f;
					}
				}
				if (pos_.x_ < Map.COAT_GOAL1_GLOBALPOS[0].x_)
				{
					pos_.x_ = Map.COAT_GOAL1_GLOBALPOS[0].x_;
				}
				if (pos_.x_ > Map.COAT_GOAL1_GLOBALPOS[1].x_)
				{
					pos_.x_ = Map.COAT_GOAL1_GLOBALPOS[1].x_;
				}
			}
			Camera.ConvertGlobalToAddress(pos_, address_);
			Camera.ConvertGlobalToScreen(pos_, screen_pos_);
			if (num != direct_)
			{
				ballDribbleTime_ = 0;
			}
			return false;
		}

		public int DecideAction()
		{
			int result = 0;
			Team team = udat_.teams_[team_id_];
			if (BitUtil.Check(flag_, 2048))
			{
				return 11;
			}
			if (BitUtil.Check(flag_, 4096))
			{
				return 12;
			}
			if (BitUtil.Check(flag_, 8192))
			{
				return 13;
			}
			if (udat_.gm_.state_ == 2 || udat_.gm_.state_ == 1)
			{
				if (udat_.gm_.stateCnt_ >= 50 && BitUtil.Check(flag_, 4))
				{
					return 7;
				}
				return 5;
			}
			if (udat_.gm_.state_ == 3 && BitUtil.Check(flag_, 4))
			{
				if (udat_.gm_.stateCnt_ >= GameManager.WAIT_THROWIN[1])
				{
					return 8;
				}
				return 5;
			}
			if (udat_.gm_.state_ == 4 && BitUtil.Check(flag_, 512))
			{
				if (udat_.gm_.stateCnt_ >= GameManager.WAIT_GK[1])
				{
					return 9;
				}
				return 5;
			}
			if (udat_.gm_.state_ == 5 && BitUtil.Check(flag_, 1024))
			{
				if (udat_.gm_.stateCnt_ >= GameManager.WAIT_CK[1])
				{
					return 10;
				}
				return 5;
			}
			if (role_ == 0)
			{
				result = 4;
			}
			else if (BitUtil.Check(flag_, 4))
			{
				result = 3;
			}
			else if (team.players_receive_.IndexOf(this) != -1)
			{
				result = 1;
			}
			else if (udat_.gm_.gameState_ == 1 - team_id_ && udat_.gm_.state_ == 0)
			{
				Player passer = udat_.teams_[1 - team_id_].GetPasser();
				if (passer != null && passer.role_ != 0 && JMath.Abs(passer.address_[0] - address_[0]) <= 2 && JMath.Abs(passer.address_[1] - address_[1]) <= 2)
				{
					float num = -900f + 1800f * (float)team_id_;
					float x = (float)JMath.Sqrt(passer.pos_.x_ * passer.pos_.x_ + (passer.pos_.z_ - num) * (passer.pos_.z_ - num));
					float num2 = GameUtil.RateConvert(x, 200f, 600f, 240f, 128f, true);
					if (JMath.Sqrt((passer.pos_.x_ - pos_.x_) * (passer.pos_.x_ - pos_.x_) + (passer.pos_.z_ - pos_.z_) * (passer.pos_.z_ - pos_.z_)) < (double)num2)
					{
						result = 2;
					}
				}
			}
			return result;
		}

		public int GetActIdFromSeb()
		{
			if (seb_ != 0)
			{
				if (seb_ == 5)
				{
					return 2;
				}
				if (seb_ == 14)
				{
					return 4;
				}
				if (seb_ == 1)
				{
					return 6;
				}
				if (seb_ == 2)
				{
					return 6;
				}
				if (seb_ == 4)
				{
					return 6;
				}
				if (seb_ == 3)
				{
					return 6;
				}
				if (seb_ == 11)
				{
					return 6;
				}
				if (seb_ == 12)
				{
					return 6;
				}
			}
			return 0;
		}

		public bool Act(int act_id)
		{
			v_.Set(0f, 0f, 0f);
			int physicalHpMax = PlayerData.GetPhysicalHpMax(this);
			if (physical_hp_ < physicalHpMax)
			{
				physical_hp_ = AppData.ClampMax(physical_hp_ + physicalHpMax / 100, physicalHpMax);
			}
			if (BitUtil.Check(flag_, 4))
			{
				ballHaveTime_++;
				ballDribbleTime_++;
			}
			if ((act_id == 0)) {
Act_freeRun();

} else if ((act_id == 1)) {
Act_receive();

} else if ((act_id == 2)) {
Act_charge();

} else if ((act_id == 3)) {
Act_dribble();

} else if ((act_id == 4)) {
Act_gk();

} else if ((act_id == 7)) {
udat_.ball_.SetPassData(this, 0, p_id_ - 1, 0, false);
				Proc_kick();
				udat_.ball_.receive_wait = 0;
				udat_.gm_.ChangeState(0);

} else if ((act_id == 8)) {
				int toThrowInPID = GetToThrowInPID();
				udat_.ball_.SetPassData(this, 5, toThrowInPID, 0, true);
				Proc_throwIn();
				if (udat_.ball_.lineOutPos_.x_ > 0f)
				{
					udat_.ball_.pos_.x_ = 450f;
				}
				else
				{
					udat_.ball_.pos_.x_ = -450f;
				}
				udat_.ball_.pos_.y_ = 20f;
				udat_.ball_.pos_.z_ = udat_.ball_.lineOutPos_.z_;
				udat_.ball_.receive_wait = 0;
				udat_.gm_.ChangeState(0);

			} else if ((act_id == 9)) {
				int[] collisionData5 = udat_.GetCollisionData(0, 0);
				int[] collisionData6 = udat_.GetCollisionData(1, 1);
				if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData5[0], pos_.z_ + (float)collisionData5[1], collisionData5[2], collisionData5[3], udat_.ball_.pos_.x_ + (float)collisionData6[0], udat_.ball_.pos_.z_ + (float)collisionData6[1], collisionData6[2], collisionData6[3], true))
				{
					udat_.ball_.ReceiveBall(this);
					udat_.ball_.SetPassData(this, 2, -1, 0, true);
					Proc_kick();
					flag_ = BitUtil.Off(flag_, 512);
					udat_.gm_.ChangeState(0);
				}
				else
				{
					Move(udat_.ball_.pos_, act_id);
				}

			} else if ((act_id == 10)) {
				int[] collisionData3 = udat_.GetCollisionData(0, 0);
				int[] collisionData4 = udat_.GetCollisionData(1, 1);
				if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData3[0], pos_.z_ + (float)collisionData3[1], collisionData3[2], collisionData3[3], udat_.ball_.pos_.x_ + (float)collisionData4[0], udat_.ball_.pos_.z_ + (float)collisionData4[1], collisionData4[2], collisionData4[3], true))
				{
					udat_.ball_.ReceiveBall(this, true);
					udat_.ball_.SetPassData(this, 4, 0, 0, true);
					Proc_kick();
					flag_ = BitUtil.Off(flag_, 1024);
					udat_.gm_.ChangeState(0);
				}
				else
				{
					Move(udat_.ball_.pos_, act_id);
				}

			} else if ((act_id == 6)) {
if (seb_ == 1)
				{
					return false;
				}

} else if ((act_id == 11)) {
				int[] collisionData7 = udat_.GetCollisionData(0, 0);
				int[] collisionData8 = udat_.GetCollisionData(1, 0);
				if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData7[0], pos_.z_ + (float)collisionData7[1], collisionData7[2], collisionData7[3], goalStatePos_.x_ + (float)collisionData8[0], goalStatePos_.z_ + (float)collisionData8[1], collisionData8[2], collisionData8[3], true))
				{
					AddCommandChangeSeb(3);
					AddCommandGoalGlad();
				}
				else
				{
					Move(goalStatePos_, act_id);
				}

			} else if ((act_id == 12)) {
				Player lastKicker = udat_.teams_[team_id_].GetLastKicker();
				if (lastKicker == null)
				{
					return true;
				}
				if (role_ == 0)
				{
					AddCommandChangeSeb(3);
					AddCommandGoalGlad();
					return true;
				}
				temp_vec.x_ = lastKicker.goalStatePos_.x_ + goalStatePos_.x_;
				temp_vec.z_ = lastKicker.goalStatePos_.z_ + goalStatePos_.z_;
				int[] collisionData = udat_.GetCollisionData(0, 0);
				int[] collisionData2 = udat_.GetCollisionData(1, 0);
				if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], temp_vec.x_ + (float)collisionData2[0], temp_vec.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true))
				{
					AddCommandChangeSeb(3);
					AddCommandGoalGlad();
				}
				else if (lastKicker.seb_ != 3)
				{
					Move(lastKicker.pos_, act_id);
				}
				else
				{
					Move(temp_vec, act_id);
				}

			} else if ((act_id == 13)) {
if (seb_ != 11)
				{
					AddCommandChangeSeb(11);
				}

}
			return true;
		}

		public void SetMoveDirectBox(int direct8)
		{
			for (int num = moveDirectBox.Length - 1; num >= 1; num--)
			{
				moveDirectBox[num] = moveDirectBox[num - 1];
			}
			moveDirectBox[0] = direct8;
		}

		public bool ChMoveDirect(int ch_direct, int act_id)
		{
			int num = 0;
			if (act_id == -1)
			{
				return true;
			}
			int num2;
int num3;
int[] array = null;
			if ((((BitUtil.Check(flag_, 4)) || (role_ == 0)) || (act_id == 1)) || (act_id == 2)) { array = direct_dis_passer; } else { array = direct_dis_free; }
			for (int i = 0; i < moveDirectBox.Length; i++)
			{
				num++;
				if (moveDirectBox[i] == -1) { continue; }
if (ch_direct != moveDirectBox[i]) {
if (ch_direct > moveDirectBox[i])
					{
						num2 = JMath.Abs(ch_direct - moveDirectBox[i]);
						num3 = JMath.Abs(ch_direct - (moveDirectBox[i] + 8));
					}
					else
					{
						num2 = JMath.Abs(moveDirectBox[i] - ch_direct);
						num3 = JMath.Abs(moveDirectBox[i] - (ch_direct + 8));
					}
if (num2 > num3)
					{
						num2 = num3;
					}
if (array[num2] <= num)
					{
						return true;
					}
return false;

}}
			return true;
		}

		public void Move(Vector3D go_pos, int act_id)
		{
			Move(go_pos, act_id, 50, false);
		}

		public void Move(Vector3D go_pos, int act_id, int speed_eva, bool isTrainingMove)
		{
			float num = (float)JMath.Sqrt((go_pos.x_ - pos_.x_) * (go_pos.x_ - pos_.x_) + (go_pos.z_ - pos_.z_) * (go_pos.z_ - pos_.z_));
			if (num == 0f)
			{
				return;
			}
			float num2 = (float)JMath.Sqrt((go_pos.x_ - pos_.x_) * (go_pos.x_ - pos_.x_));
			float num3 = (float)JMath.Sqrt((go_pos.x_ - pos_.x_) * (go_pos.x_ - pos_.x_) + (go_pos.z_ - pos_.z_) * (go_pos.z_ - pos_.z_));
			float num4 = (go_pos.x_ - pos_.x_) * (go_pos.x_ - pos_.x_);
			float num5 = num4 / (num2 * num3);
			int num6 = 0;
			if(num5>=GameUtil.Cos(22)){num6=0;}else if(num5>=GameUtil.Cos(67)){if(go_pos.z_>pos_.z_){num6=1;}else{num6=7;}}else if(num5>=GameUtil.Cos(112)){if(go_pos.z_>pos_.z_){num6=2;}else{num6=6;}}else if(num5>=GameUtil.Cos(157)){if(go_pos.z_>pos_.z_){num6=3;}else{num6=5;}}else{num6=4;}
			if (!ChMoveDirect(num6, act_id))
			{
				SetMoveDirectBox(-1);
				return;
			}
			SetMoveDirectBox(num6);
			float value = PlayerData.GetSpeed(this, speed_eva);
			if (isTrainingMove)
			{
				value = 10f;
			}
			value = PlayerData.AuraUpdate(this, value, 2);
			if (act_id == 3)
			{
				value *= 0.85f;
			}
			if (act_id == -1 && speed_eva == 50)
			{
				value *= 0.5f;
			}
			old_pos.Set(pos_);
			v_.x_ = (go_pos.x_ - pos_.x_) * value / num;
			v_.z_ = (go_pos.z_ - pos_.z_) * value / num;
			pos_.x_ += v_.x_;
			pos_.z_ += v_.z_;
			int num7 = 0;
			Camera.ConvertGlobalToScreen(v_, temp_direct_s);
			temp_direct_s[0] -= Camera.MAP_SCREEN_BASE_POS[0];
			temp_direct_s[1] -= Camera.MAP_SCREEN_BASE_POS[1];
			if(temp_direct_s[0]>0){if(temp_direct_s[1]>0){num7=0;}else{num7=1;}}else{if(temp_direct_s[1]>0){num7=3;}else{num7=2;}}
			for (int num8 = directBox_.Length - 1; num8 >= 1; num8--)
			{
				directBox_[num8] = directBox_[num8 - 1];
			}
			directBox_[0] = num7;
			int num9 = num7;
			for(int i=1;i<directBox_.Length;i++){if(num9!=directBox_[i]){break;}
				if (i == directBox_.Length - 1)
				{
					direct_ = num9;
				}
			}
		}

		public void Act_freeRun()
		{
			Player passer = udat_.teams_[team_id_].GetPasser();
			if (udat_.teams_[team_id_].ChWhichChargeTern() && passer != null)
			{
				float x = (float)JMath.Sqrt((pos_.x_ - passer.pos_.x_) * (pos_.x_ - passer.pos_.x_) + (pos_.z_ - passer.pos_.z_) * (pos_.z_ - passer.pos_.z_));
				w_freeRun_afrMostEvaPos = GameUtil.RateConvert(x, 160f, 80f, 0.6f, 0.9f, true);
				w_freeRun_formation = 1f - w_freeRun_afrMostEvaPos;
			}
			udat_.teams_[team_id_].SetMostEvaAddressFromMoveArea(this, mostEvaAddres_, false);
			Camera.ConvertAddressToGlobal(mostEvaAddres_[0], mostEvaAddres_[1], temp_afr_mostEvaPos);
			goalPos_.x_ = (w_freeRun_formation * formationGo_.x_ + w_freeRun_afrMostEvaPos * temp_afr_mostEvaPos.x_) / (w_freeRun_formation + w_freeRun_afrMostEvaPos);
			goalPos_.z_ = (w_freeRun_formation * formationGo_.z_ + w_freeRun_afrMostEvaPos * temp_afr_mostEvaPos.z_) / (w_freeRun_formation + w_freeRun_afrMostEvaPos);
			int[] collisionData = udat_.GetCollisionData(0, 0);
			int[] collisionData2 = udat_.GetCollisionData(1, 2);
			if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], goalPos_.x_ + (float)collisionData2[0], goalPos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true))
			{
				if (notMoveCnt_++ > 10)
				{
					SetPlayerDirect(udat_.ball_.pos_);
				}
				return;
			}
			notMoveCnt_ = 0;
			int[][] coatEva = GetCoatEva();
			int speed_eva = 0;
			if (formation_address_[4][1] >= 0 && formation_address_[4][1] < coatEva.Length && formation_address_[4][0] >= 0 && formation_address_[4][0] < coatEva[0].Length)
			{
				speed_eva = (int)((w_freeRun_formation * (float)coatEva[formation_address_[4][1]][formation_address_[4][0]] + w_freeRun_afrMostEvaPos * (float)coatEva[mostEvaAddres_[1]][mostEvaAddres_[0]]) / (w_freeRun_formation + w_freeRun_afrMostEvaPos));
			}
			else if (Config.DEBUG_CMD)
			{
				Log.Info("■Player.java act_freeRun() エラー");
			}
			Move(goalPos_, 0, speed_eva, false);
		}

		public void Act_receive()
		{
			if (ChHeading()) {
				Proc_head();
				float headJampDis = PlayerData.GetHeadJampDis(this);
				seb_startPos.Set(pos_);
				seb_v.y_ = GameUtil.SetVStop(headJampDis, SEB_H_HEAD_JAMPTIME / 2);
				seb_ay = GameUtil.SetAStop(headJampDis, SEB_H_HEAD_JAMPTIME / 2);

return;
}
if (BitUtil.Check(udat_.ball_.flag_, 16)) {
				goalPos_.Set(udat_.ball_.fallPos_);
				if (BitUtil.Check(udat_.ball_.flag_, 16) && udat_.ball_.boundNum_ == 0)
				{
					if (BitUtil.Check(udat_.ball_.flag_, 64))
					{
						goalPos_.Set(udat_.ball_.receiverPos_);
					}
					else if (udat_.ball_.boundNum_ > 0)
					{
						goalPos_.Set(udat_.ball_.pos_);
					}
					else if (udat_.ball_.a_.y_ == -0.25f)
					{
						if (udat_.ball_.fallTime_ - udat_.ball_.savePosFrame_ < SEB_H_HEAD_ANIME[1] / 2)
						{
							goalPos_.Set(udat_.ball_.pos_);
						}
						else
						{
							goalPos_.Set(udat_.ball_.receiverPos_);
						}
					}
					else
					{
						goalPos_.Set(udat_.ball_.fallPos_);
					}
				}
				if (goalPos_.x_ < -450f)
				{
					goalPos_.x_ = -450f;
				}
				if (goalPos_.x_ > 450f)
				{
					goalPos_.x_ = 450f;
				}
				if (goalPos_.z_ < -900f)
				{
					goalPos_.z_ = -900f;
				}
				if (goalPos_.z_ > 900f)
				{
					goalPos_.z_ = 900f;
				}
				int[] collisionData = udat_.GetCollisionData(0, 3);
				int[] collisionData2 = udat_.GetCollisionData(1, 3);
				if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], goalPos_.x_ + (float)collisionData2[0], goalPos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true)) {
					SetPlayerDirect(udat_.ball_.pos_);

return;
}
{
					Move(goalPos_, 1);
				}

return;
}
{
				Ball ball_ = udat_.ball_;
				if (JMath.Abs(ball_.v_.x_) < 0.01f || JMath.Abs(ball_.v_.z_) < 0.01f) {
					Move(ball_.pos_, 1);

} else {
				float num = ball_.v_.z_ * ball_.pos_.x_ / ball_.v_.x_ - ball_.pos_.z_;
				float num2 = ball_.v_.x_ * pos_.x_ / ball_.v_.z_ + pos_.z_;
				temp_vec.x_ = (num + num2) / (ball_.v_.x_ / ball_.v_.z_ + ball_.v_.z_ / ball_.v_.x_);
				temp_vec.z_ = ball_.v_.z_ * temp_vec.x_ / ball_.v_.x_ - num;
				udat_.ball_.temp_pos.x_ = udat_.ball_.pos_.x_ + udat_.ball_.v_.x_ * 3f;
				udat_.ball_.temp_pos.z_ = udat_.ball_.pos_.z_ + udat_.ball_.v_.z_ * 3f;
				udat_.ball_.temp_pos.x_ = (udat_.ball_.temp_pos.x_ + temp_vec.x_) / 2f;
				udat_.ball_.temp_pos.z_ = (udat_.ball_.temp_pos.z_ + temp_vec.z_) / 2f;
				Move(udat_.ball_.temp_pos, 1);
}
			}
		}

		public void Act_charge()
		{
Player passer = udat_.teams_[1 - team_id_].GetPasser();
old_pos.Set(pos_);
if (passer == null) {
return;
}
if (ChSlidingTackleEva(passer)) {
GameUtil.Sub(passer.pos_, pos_, temp_vec);
float num = (float)JMath.Sqrt(temp_vec.x_ * temp_vec.x_ + temp_vec.z_ * temp_vec.z_);
if (num < 0.1f) {
return;
}
seb_startPos.Set(pos_);
seb_v.x_ = PlayerData.SLIDING_SPEED * temp_vec.x_ / num;
seb_v.z_ = PlayerData.SLIDING_SPEED * temp_vec.z_ / num;
Proc_sliding(passer);
apdat_.PlaySe(11);
return;
}
Move(udat_.ball_.pos_, 2);
return;
}

		public void Act_dribble()
		{
			bool flag = udat_.teams_[team_id_].ChPAreaLineOver();
			w_dribble = 1f;
			w_pass = 1f;
			w_clear = 0.65000004f;
			w_shoot = 1.2f;
			w_centering = 2f;
			int max_ballHaveTime = 120;
			if (flag)
			{
				max_ballHaveTime = 10;
			}
			int playerBodyPoseEva = PlayerData.GetPlayerBodyPoseEva(this, max_ballHaveTime);
			w_pass = GameUtil.RateConvert(playerBodyPoseEva, 0f, 100f, 0.4f, 1f, true);
			w_pass_through = w_pass;
			w_dribble = GameUtil.RateConvert(playerBodyPoseEva, 0f, 100f, 1f, 0.4f, true);
			eva_shoot = 0f;
			if (dribbleStopCnt_ >= 3 && flag)
			{
				w_dribble = 0f;
			}
			else if (dribbleStopCnt_ >= 10)
			{
				w_dribble = 0f;
				w_pass_through = 0f;
			}
			int num = GameUtil.RateConvert(playerBodyPoseEva, 0, 100, 8, 2, true);
			eva_clear = 100f;
			if (apdat_.frmSoccer_.statusAmeftoMode_ == 2)
			{
				if (team_id_ == 0 && pos_.z_ > -450f)
				{
					eva_clear = 0f;
				}
				else if (team_id_ == 1 && pos_.z_ < 450f)
				{
					eva_clear = 0f;
				}
			}
			else if (team_id_ == 0 && pos_.z_ > -300f)
			{
				eva_clear = 0f;
			}
			else if (team_id_ == 1 && pos_.z_ < 600f)
			{
				eva_clear = 0f;
			}
			eva_centering = 0f;
			if (flag)
			{
				int num2 = (int)GameUtil.RateConvert(pos_.x_, -450f, 450f, 0f, 100f, true);
				if (num2 < 25 || num2 > 75)
				{
					int num3 = 0;
					for (int i = 1; i < udat_.teams_[team_id_].players_.Length; i++)
					{
						if (i == p_id_)
						{
							continue;
						}
						Player player = udat_.teams_[team_id_].players_[i];
						if ((player.pos_.x_ * pos_.x_ > 0f)) { continue; }
float num4 = 0f;
if (team_id_ == 0) { num4 = ((float)JMath.Sqrt((player.pos_.z_ - 900f) * (player.pos_.z_ - 900f))); } else { num4 = ((float)JMath.Sqrt((player.pos_.z_ - -900f) * (player.pos_.z_ - -900f))); }
if (num4 < 400f)
							{
								num3++;
							}
					}
					if (num3 > 0)
					{
						eva_centering = GameUtil.RateConvert(num3, 1, 3, 70, 100, true);
					}
				}
			}
			float num5 = GameUtil.RateConvert(udat_.teams_[team_id_].eva_risk_, 0, 100, 80, 95, true);
			float num6 = GameUtil.RateConvert(udat_.teams_[team_id_].eva_risk_, 0, 100, 80, 95, true);
			for (int j = 0; j < DIRECT_RECT.Length; j++)
			{
				if (address_[0] + DIRECT_RECT[j][0] >= 0 && address_[0] + DIRECT_RECT[j][0] < Map.MAPCHIP_COAT_WNUM && address_[1] + DIRECT_RECT[j][1] >= 0 && address_[1] + DIRECT_RECT[j][1] < Map.MAPCHIP_COAT_HNUM)
				{
					eva_dribble[j] = udat_.teams_[team_id_].Get_playerReceivePosEva(this, address_[0] + DIRECT_RECT[j][0], address_[1] + DIRECT_RECT[j][1]);
					if (address_[1] == udat_.teams_[team_id_].amefto_address_[0][1] && ((team_id_ == 0 && j == 1) || (team_id_ == 1 && j == 7)))
					{
						eva_dribble[j] = eva_dribble[j] * 120f / 100f;
					}
				}
				else
				{
					eva_dribble[j] = -1f;
				}
			}
			for (int k = 1; k < udat_.teams_[team_id_].players_.Length; k++)
			{
				if (k == p_id_)
				{
					eva_pass[k][0] = 0f;
					continue;
				}
				eva_pass_info[k][0] = 0;
				eva_pass_info_onEnemy[k][0] = 0;
				Player player2 = udat_.teams_[team_id_].players_[k];
				int num7 = player2.eva_receivePos_;
				float num8 = (float)JMath.Sqrt((player2.pos_.x_ - pos_.x_) * (player2.pos_.x_ - pos_.x_) + (player2.pos_.z_ - pos_.z_) * (player2.pos_.z_ - pos_.z_));
				if (num8 < 100f)
				{
					eva_pass[k][0] = 0f;
					continue;
				}
				if (num8 >= 270f)
				{
					eva_pass_info[k][0] = 1;
					num7 = (int)((float)num7 * num5 / 100f);
				}
				bool flag2 = false;
				for (int l = 0; l < udat_.teams_[1 - team_id_].players_.Length; l++)
				{
					Player player3 = udat_.teams_[1 - team_id_].players_[l];
					if ((address_[0] == player3.address_[0]) && (address_[1] == player3.address_[1])) { continue; }
if (GameUtil.ChOnLine(screen_pos_, player2.screen_pos_, 12, player3.screen_pos_)) {
flag2 = true;
eva_pass_info_onEnemy[k][0] = 1;
break;
				}}
				if (flag2) { if (eva_pass_info[k][0] == 1) { } else {
					eva_pass_info[k][0] = 1;
					num7 = num7 * 70 / 100;
				} }
				eva_pass[k][0] = num7 + GameUtil.Random(num) - num / 2;
			}
			bool flag3 = true;
			for (int m = 0; m < eva_pass.Length; m++)
			{
				if (eva_pass[m][0] != 0f)
				{
					flag3 = false;
					break;
				}
			}
			if (flag3)
			{
				for (int n = 0; n < eva_pass.Length; n++)
				{
					if (n == p_id_) { continue; }
						eva_pass[n][0] = 1f;
						break;

				}
			}
			for (int num9 = 1; num9 < udat_.teams_[team_id_].players_.Length; num9++)
			{
				eva_pass[num9][1] = 0f;
				if (num9 == p_id_)
				{
					continue;
				}
				if (address_[0] == 0 || address_[0] == Map.MAPCHIP_COAT_WNUM - 1)
				{
					eva_pass[num9][1] = 0f;
					continue;
				}
				Player player4 = udat_.teams_[team_id_].players_[num9];
				if (team_id_ == 0) { if (pos_.z_ > player4.pos_.z_) { continue; } }
				if (team_id_ == 1) { if (pos_.z_ < player4.pos_.z_) { continue; } }
				if (player4.address_[0] == player4.mostEvaAddres_[0] && player4.address_[1] == player4.mostEvaAddres_[1]) { continue; }
				eva_pass_info[num9][1] = 0;
				eva_pass_info_onEnemy[num9][1] = 0;
				int num10 = 0;
				udat_.teams_[team_id_].SetMostEvaAddressFromMoveArea_thrughPass(player4, mostEvaAddres_, true);
				num10 = udat_.teams_[player4.team_id_].Get_playerReceivePosEva(player4, player4.mostEvaAddres_[0], player4.mostEvaAddres_[1]);
				Camera.ConvertAddressToGlobal(mostEvaAddres_[0], mostEvaAddres_[1], temp_afr_mostEvaPos);
				Camera.ConvertGlobalToScreen(temp_afr_mostEvaPos, temp_afr_mostEvaSPos);
				float num11 = (float)JMath.Sqrt((temp_afr_mostEvaPos.x_ - pos_.x_) * (temp_afr_mostEvaPos.x_ - pos_.x_) + (temp_afr_mostEvaPos.z_ - pos_.z_) * (temp_afr_mostEvaPos.z_ - pos_.z_));
				if (num11 < 100f)
				{
					eva_pass[num9][1] = 0f;
					continue;
				}
				if (num11 >= 270f)
				{
					eva_pass_info[num9][1] = 1;
					num10 = (int)((float)num10 * num6 / 100f);
				}
				bool flag4 = false;
				for (int num12 = 0; num12 < udat_.teams_[1 - team_id_].players_.Length; num12++)
				{
					Player player5 = udat_.teams_[1 - team_id_].players_[num12];
					if (address_[0] == player5.address_[0] && address_[1] == player5.address_[1]) { continue; }
					if (GameUtil.ChOnLine(screen_pos_, temp_afr_mostEvaSPos, 12, player5.screen_pos_)) {
						flag4 = true;
						eva_pass_info_onEnemy[num9][1] = 1;
						break;
					}
				}
				if (flag4)
				{
					eva_pass_info[num9][1] = 1;
				}
				eva_pass[num9][1] = num10 + GameUtil.Random(num) - num / 2;
			}
			actDecide_max = 0f;
			actDecide_index = 0;
			for (int num13 = 0; num13 < eva_dribble.Length; num13++)
			{
				eva_dribble[num13] *= w_dribble;
			}
			for (int num14 = 0; num14 < eva_pass.Length; num14++)
			{
				eva_pass[num14][0] *= w_pass;
				eva_pass[num14][1] *= w_pass_through;
			}
			eva_clear *= w_clear;
			if (team_id_ == 0)
			{
				if (address_[0] >= 2 && address_[0] <= 7 && address_[1] >= 12)
				{
					eva_shoot = GetShootEva();
					eva_shoot *= w_shoot;
				}
			}
			else if (address_[0] >= 2 && address_[0] <= 7 && address_[1] <= 3)
			{
				eva_shoot = GetShootEva();
				eva_shoot *= w_shoot;
			}
			for (int num15 = 0; num15 < eva_dribble.Length; num15++)
			{
				if (actDecide_max < eva_dribble[num15])
				{
					actDecide_max = eva_dribble[num15];
					actDecide_index = num15;
				}
			}
			for (int num16 = 0; num16 < eva_pass.Length; num16++)
			{
				if (actDecide_max < eva_pass[num16][0])
				{
					actDecide_max = eva_pass[num16][0];
					actDecide_index = 100 + num16;
				}
				if (actDecide_max < eva_pass[num16][1])
				{
					actDecide_max = eva_pass[num16][1];
					actDecide_index = 200 + num16;
				}
			}
			if (actDecide_max < eva_clear)
			{
				actDecide_max = eva_clear;
				actDecide_index = 300;
			}
			if (actDecide_max < eva_centering)
			{
				actDecide_max = eva_centering;
				actDecide_index = 400;
			}
			if (actDecide_max < eva_shoot)
			{
				actDecide_max = eva_shoot;
				actDecide_index = 500;
			}
			if (actDecide_index != 4)
			{
				dribbleStopCnt_ = 0;
			}
			if (actDecide_index < 100)
			{
				int num17 = actDecide_index % 100;
				if (num17 % 2 == 1)
				{
					goalPos_.Set(pos_);
					goalPos_.x_ += 900f / (float)Map.MAPCHIP_COAT_WNUM * (float)DIRECT_RECT[num17][0];
					goalPos_.z_ += 1800f / (float)Map.MAPCHIP_COAT_HNUM * (float)DIRECT_RECT[num17][1];
				}
				else if (num17 == 4)
				{
					goalPos_.Set(pos_);
				}
				else
				{
					Camera.ConvertAddressToGlobal(address_[0] + DIRECT_RECT[num17][0], address_[1] + DIRECT_RECT[num17][1], goalPos_);
				}
				int[] collisionData = udat_.GetCollisionData(0, 0);
				int[] collisionData2 = udat_.GetCollisionData(1, 0);
				if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], goalPos_.x_ + (float)collisionData2[0], goalPos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true))
				{
					dribbleStopCnt_++;
					return;
				}
				Move(goalPos_, 3, 60, false);
				apdat_.pData_[pData_id_].SetActScore(4);
			}
			else if (actDecide_index < 200)
			{
				int num18 = actDecide_index % 100;
				bool jamp = false;
				if (eva_pass_info[num18][0] == 1)
				{
					jamp = true;
				}
				udat_.ball_.SetPassData(this, 0, num18, 0, jamp);
				Proc_kick();
				apdat_.pData_[pData_id_].SetActScore(1);
			}
			else if (actDecide_index < 300)
			{
				int num19 = actDecide_index % 100;
				bool jamp2 = false;
				if (eva_pass_info[num19][1] == 1)
				{
					jamp2 = true;
				}
				Player player6 = udat_.teams_[team_id_].players_[num19];
				Camera.ConvertAddressToGlobal(player6.mostEvaAddres_[0], player6.mostEvaAddres_[1], temp_afr_mostEvaPos);
				udat_.ball_.SetPassData(player6, 1, player6.mostEvaAddres_[0], player6.mostEvaAddres_[1], jamp2);
				Proc_kick();
				apdat_.pData_[pData_id_].SetActScore(1);
			}
			else if (actDecide_index < 400)
			{
				udat_.ball_.SetPassData(this, 2, -1, 0, true);
				Proc_kick();
				apdat_.pData_[pData_id_].SetActScore(2);
			}
			else if (actDecide_index < 500)
			{
				udat_.ball_.SetPassData(this, 3, -1, 0, true);
				Proc_kick();
				apdat_.pData_[pData_id_].SetActScore(3);
			}
			else if (actDecide_index < 600)
			{
				udat_.ball_.SetShoot(this, 0);
				Proc_kick();
				apdat_.AddEffectShoot(screen_pos_[0] + EF_SHOOT_OFF[direct_][0], screen_pos_[1] + EF_SHOOT_OFF[direct_][1]);
				apdat_.pData_[pData_id_].SetActScore(0);
			}
		}

		public void DebugLogOut()
		{
		}

		public static void Debug_1keyStop()
		{
		}

		public void Act_gk()
		{
			Ball ball_ = udat_.ball_;
			if (BitUtil.Check(flag_, 4))
			{
				if (ballHaveTime_ >= 35)
				{
					udat_.ball_.SetPassData(this, 2, -1, 0, true);
					Proc_kick();
				}
				else
				{
					if (((team_id_ == 0) && ((pos_.z_ < Map.COAT_GOAL1_IN_GLOBALPOS[1].z_))) || ((team_id_ == 1) && ((pos_.z_ > Map.COAT_GOAL2_IN_GLOBALPOS[1].z_)))) {

					goalPos_.x_ = pos_.x_;
					if (team_id_ == 0)
					{
						goalPos_.z_ = Map.COAT_GOAL1_IN_GLOBALPOS[1].z_;
					}
					else
					{
						goalPos_.z_ = Map.COAT_GOAL2_IN_GLOBALPOS[1].z_;
					}
					int[] collisionData = udat_.GetCollisionData(0, 0);
					int[] collisionData2 = udat_.GetCollisionData(1, 0);
					if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], goalPos_.x_ + (float)collisionData2[0], goalPos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true))
					{
						if (notMoveCnt_++ > 3)
						{
							if (team_id_ == 0)
							{
								direct_ = 0;
							}
							else
							{
								direct_ = 2;
							}
						}
					}
					else
					{
						notMoveCnt_ = 0;
						Move(goalPos_, 4);
					}
}
				}
			}
			else
			{
				if (BitUtil.Check(flag_, 8)) { } else {

				bool flag = ball_.ChBallDirectHere(this);
				int toGoalLineTime = ball_.GetToGoalLineTime(this);
				bool flag2 = false;
				if (BitUtil.Check(ball_.flag_, 32))
				{
					flag2 = true;
				}
				if (flag && toGoalLineTime > 0 && toGoalLineTime < SEB_H_SAVING_ANIME[0] * 5 / 10 && flag2) {
					Proc_saving();
					seb_startPos.Set(pos_);
					seb_v.y_ = GameUtil.SetVStop(10f, SEB_H_SAVING_ANIME[0] / 2);
					seb_ay = GameUtil.SetAStop(10f, SEB_H_SAVING_ANIME[0] / 2);
					seb_v.x_ = (ball_.g1.x_ - pos_.x_) / (float)(SEB_H_SAVING_ANIME[0] / 2);
					seb_v.z_ = (ball_.g1.z_ - pos_.z_) / (float)(SEB_H_SAVING_ANIME[0] / 2);
					float num = (float)JMath.Sqrt(seb_v.x_ * seb_v.x_ + seb_v.z_ * seb_v.z_);
					float keeperJampMax = PlayerData.GetKeeperJampMax(this);
					if (num > keeperJampMax)
					{
						seb_v.x_ = seb_v.x_ * keeperJampMax / num;
						seb_v.z_ = seb_v.z_ * keeperJampMax / num;
					}
					if (num < 4.5f)
					{
						direct_ = 0 + 2 * team_id_;
					}
					else if (seb_v.x_ < 0f)
					{
						direct_ = 3;
					}
					else
					{
						direct_ = 1;
					}

} else {
				float num2 = 0f;
				num2 = (float)JMath.Sqrt((ball_.pos_.x_ - pos_.x_) * (ball_.pos_.x_ - pos_.x_) + (ball_.pos_.z_ - pos_.z_) * (ball_.pos_.z_ - pos_.z_));
				if (num2 < 180f)
				{
					goalPos_.Set(ball_.pos_);
				}
				else
				{
					int num3 = 0;
					if (team_id_ == 0)
					{
						num3 = (int)GameUtil.RateConvert(ball_.pos_.z_, -900f, 900f, 0f, 100f, true);
						goalPos_.z_ = GameUtil.RateConvert(num3, 0f, 100f, -900f, Map.COAT_GOAL1_GLOBALPOS[1].z_, true);
					}
					else
					{
						num3 = (int)GameUtil.RateConvert(ball_.pos_.z_, 900f, -900f, 0f, 100f, true);
						goalPos_.z_ = GameUtil.RateConvert(num3, 0f, 100f, 900f, Map.COAT_GOAL2_GLOBALPOS[1].z_, true);
					}
					if (pos_.x_ == ball_.pos_.x_)
					{
						goalPos_.x_ = 0f;
					}
					else
					{
						temp_vec.Set(udat_.ball_.pos_);
						if (temp_vec.x_ < Map.COAT_GOAL1_GLOBALPOS[0].x_)
						{
							temp_vec.x_ = Map.COAT_GOAL1_GLOBALPOS[0].x_;
						}
						if (temp_vec.x_ > Map.COAT_GOAL1_GLOBALPOS[1].x_)
						{
							temp_vec.x_ = Map.COAT_GOAL1_GLOBALPOS[1].x_;
						}
						goalPos_.x_ = temp_vec.x_ / 2f + Map.COAT_GOAL1_GLOBALPOS[0].x_ / 4f + Map.COAT_GOAL1_GLOBALPOS[1].x_ / 4f;
					}
				}
				old_pos.Set(pos_);
				int[] collisionData3 = udat_.GetCollisionData(0, 0);
				int[] collisionData4 = udat_.GetCollisionData(1, 1);
				if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData3[0], pos_.z_ + (float)collisionData3[1], collisionData3[2], collisionData3[3], goalPos_.x_ + (float)collisionData4[0], goalPos_.z_ + (float)collisionData4[1], collisionData4[2], collisionData4[3], true))
				{
					if (notMoveCnt_++ > 3)
					{
						SetPlayerDirect(udat_.ball_.pos_);
					}
				}
				else
				{
					notMoveCnt_ = 0;
					Move(goalPos_, 4);
				}
				int[] collisionData5 = udat_.GetCollisionData(0, 0);
				int[] collisionData6 = udat_.GetCollisionData(1, 0);
				Player passer = udat_.teams_[1 - team_id_].GetPasser();
				if (passer != null && GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData5[0], pos_.z_ + (float)collisionData5[1], collisionData5[2], collisionData5[3], passer.pos_.x_ + (float)collisionData6[0], passer.pos_.z_ + (float)collisionData6[1], collisionData6[2], collisionData6[3], true))
				{
					pos_.Set(old_pos);
					int chargeDamage = PlayerData.GetChargeDamage(this, false);
					passer.physical_hp_ = AppData.ClampMin(passer.physical_hp_ - chargeDamage, 0);
					if (passer.physical_hp_ <= 0)
					{
						udat_.ball_.ReleaseBall(passer);
						udat_.ball_.ReceiveBall(this);
						passer.physical_hp_ = PlayerData.GetPhysicalHpMax(passer);
					}
				}
}
}
			}
		}

		public int GetToThrowInPID()
		{
			int num = 0;
			for (int i = 0; i < udat_.teams_[team_id_].players_.Length; i++)
			{
				if (i == p_id_)
				{
					eva_pass[i][0] = 0f;
					continue;
				}
				Player player = udat_.teams_[team_id_].players_[i];
				int num2 = player.eva_receivePos_;
				int num3 = num2;
				float num4 = (float)JMath.Sqrt((player.pos_.x_ - pos_.x_) * (player.pos_.x_ - pos_.x_) + (player.pos_.z_ - pos_.z_) * (player.pos_.z_ - pos_.z_));
				if (num4 > 500f)
				{
					eva_pass[i][0] = 0f;
					continue;
				}
				eva_pass_info[i][0] = 1;
				num3 = num3 * (int)GameUtil.RateConvert(num4, 50f, 300f, 100f, 50f, true) / 100;
				bool flag = false;
				for (int j = 0; j < udat_.teams_[1 - team_id_].players_.Length; j++)
				{
					Player player2 = udat_.teams_[1 - team_id_].players_[j];
					if (address_[0] == player2.address_[0] && address_[1] == player2.address_[1]) { continue; }
if (GameUtil.ChOnLine(screen_pos_,player.screen_pos_,6,player2.screen_pos_)) {
flag = true;
break;

}}
				if (flag && eva_pass_info[i][0] == 1)
				{
					num3 = num3 * 90 / 100;
				}
				eva_pass[i][0] = num3;
				num++;
			}
			float num5 = 0f;
			int num6 = -1;
			for (int k = 0; k < eva_pass.Length; k++)
			{
				if (num5 < eva_pass[k][0])
				{
					num5 = eva_pass[k][0];
					num6 = k;
				}
			}
			if (num6 == -1)
			{
				num5 = 0f;
				for (int l = 1; l < udat_.teams_[team_id_].players_.Length; l++)
				{
					if (l == p_id_) { continue; }
						Player player3 = udat_.teams_[team_id_].players_[l];
						float num7 = (float)JMath.Sqrt((player3.pos_.x_ - pos_.x_) * (player3.pos_.x_ - pos_.x_) + (player3.pos_.z_ - pos_.z_) * (player3.pos_.z_ - pos_.z_));
						if (num5 < num7)
						{
							num5 = num7;
							num6 = l;
						}

				}
			}
			return num6;
		}

		public int[][] GetCoatEva()
		{

			Team team = udat_.teams_[team_id_];
			int[][] array = null;
			if (team.ChWhichChargeTern()) { array = team.coatEva_new_offence; } else { array = team.coatEva_new_deffence; }
return array;
}

		public void Collision()
		{
			bool flag = true;
			if (BitUtil.Check(flag_, 16408))
			{
				flag = false;
			}
			bool flag2 = false;
			bool flag3 = false;
			if (actId_ == 1)
			{
				flag2 = true;
			}
			{ bool recoveredCondition_54620 = udat_.gm_.state_ == 0 && flag;
if (recoveredCondition_54620)
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < udat_.teams_[i].players_.Length; j++)
					{
						if (j == 0 || (i == team_id_ && j == p_id_))
						{
							continue;
						}
						Player player = udat_.teams_[i].players_[j];
						if (JMath.Abs(address_[0] - player.address_[0]) >= 2 || JMath.Abs(address_[1] - player.address_[1]) >= 2 || BitUtil.Check(player.flag_, 16408))
						{
							continue;
						}
						int[] collisionData = udat_.GetCollisionData(0, 0);
						int[] collisionData2 = udat_.GetCollisionData(1, 0);
						if (!GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], player.pos_.x_ + (float)collisionData2[0], player.pos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true))
						{
							continue;
						}
						float num = collisionData2[2] / 2;
						float num2 = collisionData2[3] / 2;
						float num3 = JMath.Abs(player.pos_.x_ - pos_.x_);
						float num4 = JMath.Abs(player.pos_.z_ - pos_.z_);
						if (num3 < 0.001f || num4 < 0.001f)
						{
							continue;
						}
						int num5 = 0;
						num5 = ((!(num4 > num3)) ? 1 : 0);
						chColPos.Set(pos_);
						if (num5 == 0)
						{
							if (pos_.x_ < player.pos_.x_)
							{
								pos_.x_ = player.pos_.x_ - 2f * num2 * num3 / num4;
							}
							else
							{
								pos_.x_ = player.pos_.x_ + 2f * num2 * num3 / num4;
							}
							if (pos_.z_ < player.pos_.z_)
							{
								pos_.z_ = player.pos_.z_ - 2f * num2;
							}
							else
							{
								pos_.z_ = player.pos_.z_ + 2f * num2;
							}
						}
						else
						{
							if (pos_.z_ < player.pos_.z_)
							{
								pos_.z_ = player.pos_.z_ - 2f * num * num4 / num3;
							}
							else
							{
								pos_.z_ = player.pos_.z_ + 2f * num * num4 / num3;
							}
							if (pos_.x_ < player.pos_.x_)
							{
								pos_.x_ = player.pos_.x_ - 2f * num;
							}
							else
							{
								pos_.x_ = player.pos_.x_ + 2f * num;
							}
						}
						if (flag2 && team_id_ == 1 - i)
						{
							flag3 = false;
							if (player.actId_ == 1)
							{
								flag3 = true;
							}
							if (flag3 && !BitUtil.Check(player.flag_, 16384) && ChBodyHitBody(this, player) && player.physical_hp_ <= 0)
							{
								player.Proc_down();
							}
						}
					}
				}
			}
}
			if (udat_.gm_.state_ != 0 || udat_.gm_.gameState_ != 2 || !BitUtil.Check(udat_.ball_.flag_, 8))
			{
				return;
			}
			if (role_ == 0)
			{
				if (!(udat_.ball_.pos_.y_ < 20f))
				{
					return;
				}
				int[] collisionData3 = udat_.GetCollisionData(0, 0);
				int[] collisionData4 = udat_.GetCollisionData(1, 1);
				if (!GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData3[0], pos_.z_ + (float)collisionData3[1], collisionData3[2], collisionData3[3], udat_.ball_.pos_.x_ + (float)collisionData4[0], udat_.ball_.pos_.z_ + (float)collisionData4[1], collisionData4[2], collisionData4[3], true))
				{
					return;
				}
				if (BitUtil.Check(flag_, 8))
				{
					if (SoccerForm.DEBUG_GK_MODE == 1 && team_id_ == 0)
					{
						return;
					}
					bool front = false;
					if (team_id_ == 0)
					{
						if (pos_.z_ < udat_.ball_.pos_.z_)
						{
							front = true;
						}
					}
					else if (pos_.z_ > udat_.ball_.pos_.z_)
					{
						front = true;
					}
					switch (PlayerData.GetCatchProc(this, udat_.ball_.v_len_, front))
					{
					case 2:
						udat_.ball_.ReceiveBall(this);
						break;
					case 1:
					{
						int angle2 = GameUtil.Random(180) + team_id_ * 180;
						float num7 = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 5f, 10f, true);
						int x2 = GameUtil.Random(100);
						float dis2 = GameUtil.RateConvert(x2, 0, 99, 5, 20, true);
						int t2 = GameUtil.RateConvert(x2, 0, 99, 5, 10, true);
						udat_.ball_.ReceiveBall(this);
						udat_.ball_.ReleaseBall(this);
						udat_.ball_.v_.Set(num7 * GameUtil.Cos(angle2), 0f, num7 * GameUtil.Sin(angle2));
						udat_.ball_.v_.y_ = GameUtil.SetVStop(dis2, t2);
						udat_.ball_.a_.y_ = GameUtil.SetAStop(dis2, t2);
						break;
					}
					default:
					{
						int angle = GameUtil.Random(180) + (1 - team_id_) * 180;
						float num6 = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 2.5f, 5f, true);
						int x = GameUtil.Random(100);
						float dis = GameUtil.RateConvert(x, 0, 99, 5, 20, true);
						int t = GameUtil.RateConvert(x, 0, 99, 10, 20, true);
						udat_.ball_.ReceiveBall(this);
						udat_.ball_.ReleaseBall(this);
						udat_.ball_.v_.Set(num6 * GameUtil.Cos(angle), 0f, num6 * GameUtil.Sin(angle));
						udat_.ball_.v_.y_ = GameUtil.SetVStop(dis, t);
						udat_.ball_.a_.y_ = GameUtil.SetAStop(dis, t);
						break;
					}
					}
				}
				else
				{
					udat_.ball_.ReceiveBall(this);
				}
			}
			else if ((!BitUtil.Check(udat_.ball_.flag_, 16) || !BitUtil.Check(udat_.ball_.flag_, 32) || udat_.ball_.boundNum_ != 0) && ChTrapOrHeading())
			{
				ChRepetition();
			}
		}

		public static bool ChBodyHitBody(Player charge_p, Player receive_p)
		{
			bool flag = BitUtil.Check(charge_p.flag_, 16);
			float value = PlayerData.CHARGE_RATE_BODY;
			value = PlayerData.AuraUpdate(charge_p, value, 4);
			value = PlayerData.AuraUpdate(receive_p, value, 6);
			if ((float)GameUtil.Random(1000) < value || flag)
			{
				int chargeDamage = PlayerData.GetChargeDamage(charge_p, flag);
				chargeDamage = (int)PlayerData.AuraUpdate(charge_p, chargeDamage, 3);
				chargeDamage = (int)PlayerData.AuraUpdate(receive_p, chargeDamage, 5);
				receive_p.physical_hp_ = AppData.ClampMin(receive_p.physical_hp_ - chargeDamage, 0);
				if (flag)
				{
					charge_p.flag_ = BitUtil.Off(charge_p.flag_, 32);
				}
				return true;
			}
			return false;
		}

		public bool ChTrapOrHeading()
		{
			bool flag = BitUtil.Check(flag_, 64);
			if (flag && !BitUtil.Check(flag_, 128))
			{
				return false;
			}
			float num = 20f;
			if (flag)
			{
				num = 40f;
			}
			if (udat_.ball_.pos_.y_ < num)
			{
				int[] collisionData = udat_.GetCollisionData(0, 0);
				int[] collisionData2 = udat_.GetCollisionData(1, 1);
				if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], udat_.ball_.pos_.x_ + (float)collisionData2[0], udat_.ball_.pos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true))
				{
					return true;
				}
			}
			return false;
		}

		public void ChRepetition()
		{
			Ball ball_ = udat_.ball_;
			if (hitList.Size() > 0)
			{
				hitList.RemoveAllElements();
			}
			hitList.AddElement(this);
			for (int i = 1; i < udat_.teams_[team_id_].players_.Length; i++)
			{
				if (i == p_id_) { continue; }
Player player = udat_.teams_[team_id_].players_[i];
if ((((player.address_[0] - ball_.address_[0] < -2) || (player.address_[0] - ball_.address_[0] > 2)) || (player.address_[1] - ball_.address_[1] < -2)) || (player.address_[1] - ball_.address_[1] > 2)) { continue; }
if (!(player.ChTrapOrHeading())) { continue; }
hitList.AddElement(player);
			}
			for (int j = 1; j < udat_.teams_[1 - team_id_].players_.Length; j++)
			{
				Player player2 = udat_.teams_[1 - team_id_].players_[j];
				if (player2.ChTrapOrHeading())
				{
					hitList.AddElement(player2);
				}
			}
			bool flag = false;
			for (int k = 0; k < hitList.Size(); k++)
			{
				Player player3 = (Player)hitList.ElementAt(k);
				if (BitUtil.Check(player3.flag_, 64))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				for (int num = hitList.Size() - 1; num >= 0; num--)
				{
					Player player4 = (Player)hitList.ElementAt(num);
					if (!BitUtil.Check(player4.flag_, 64))
					{
						hitList.RemoveElementAt(num);
					}
				}
			}
			Player player5 = null;
			if (hitList.Size() > 1)
			{
				for (int l = 0; l < hitList.Size(); l++)
				{
					Player player6 = (Player)hitList.ElementAt(l);
					player6.temp_len = (float)JMath.Sqrt((player6.pos_.x_ - ball_.pos_.x_) * (player6.pos_.x_ - ball_.pos_.x_) + (player6.pos_.z_ - ball_.pos_.z_) * (player6.pos_.z_ - ball_.pos_.z_));
				}
				float num2 = 2.1474836E+09f;
				int index = 0;
				for (int m = 0; m < hitList.Size(); m++)
				{
					Player player7 = (Player)hitList.ElementAt(m);
					if (player7.temp_len < num2)
					{
						num2 = player7.temp_len;
						index = m;
					}
				}
				player5 = (Player)hitList.ElementAt(index);
			}
			else
			{
				player5 = (Player)hitList.FirstElement();
			}
			if (BitUtil.Check(player5.flag_, 64))
			{
				udat_.ball_.ReceiveBall(player5);
				udat_.ball_.ReleaseBall(player5);
				udat_.ball_.SetHeadingData(player5);
				player5.flag_ = BitUtil.Off(flag_, 128);
			}
			else
			{
				udat_.ball_.ReceiveBall(player5);
			}
		}

		public void Update_moveArea()
		{
			if (temp_moveAreaPos == null)
			{
				temp_moveAreaPos = new Vector3D[formation_param_.Length];
				for (int i = 0; i < temp_moveAreaPos.Length; i++)
				{
					temp_moveAreaPos[i] = new Vector3D();
				}
			}
			float[] fPos_ = udat_.teams_[team_id_].fPos_;
			if (team_id_ == 0)
			{
				for (int j = 0; j < temp_moveAreaPos.Length; j++)
				{
					temp_moveAreaPos[j].x_ = GameUtil.RateConvert(formation_param_[j][0], 0f, 100f, fPos_[0] - fPos_[2] / 2f, fPos_[0] + fPos_[2] / 2f, true);
					temp_moveAreaPos[j].z_ = GameUtil.RateConvert(formation_param_[j][1], 0f, 100f, fPos_[1] - fPos_[3] / 2f, fPos_[1] + fPos_[3] / 2f, true);
				}
			}
			else
			{
				for (int k = 0; k < temp_moveAreaPos.Length; k++)
				{
					temp_moveAreaPos[k].x_ = GameUtil.RateConvert(formation_param_[k][0], 0f, 100f, fPos_[0] + fPos_[2] / 2f, fPos_[0] - fPos_[2] / 2f, true);
					temp_moveAreaPos[k].z_ = GameUtil.RateConvert(formation_param_[k][1], 0f, 100f, fPos_[1] + fPos_[3] / 2f, fPos_[1] - fPos_[3] / 2f, true);
				}
			}
			for (int l = 0; l < temp_moveAreaPos.Length; l++)
			{
				Camera.ConvertGlobalToAddress(temp_moveAreaPos[l], formation_address_[l]);
			}
		}

		public virtual bool Script()
		{
			while (script_.Size() > 0)
			{
				insertScriptIndex_ = 1;
				try
				{
					int[] firstScript = GetFirstScript();
int recoveredScriptCommand = firstScript[0];
					if ((recoveredScriptCommand == 1)) {
if (firstScript[2] == 1)
						{
							flag_ = BitUtil.On(flag_, 1);
						}
						firstScript[1]--;
						if (firstScript[1] <= 0)
						{
							script_.RemoveElementAt(0);
							continue;
						}

}
else if ((recoveredScriptCommand == 2)) {
ChangeState(firstScript[1]);
						script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 3)) {
seb_ = firstScript[1];
						sebCnt_ = 0;
						frame_ = 0;
						flag_ = BitUtil.On(flag_, 2);
						script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 4)) {
direct_ = firstScript[1];
						script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 5)) {
flag_ = firstScript[1];
						script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 6)) {
flag_ = BitUtil.On(flag_, firstScript[1]);
						script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 7)) {
flag_ = BitUtil.Off(flag_, firstScript[1]);
						script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 9)) {
if (sebCnt_ < SEB_H_SAVING_ANIME[0])
						{
							pos_.x_ = seb_startPos.x_ + seb_v.x_ * (float)sebCnt_;
							pos_.z_ = seb_startPos.z_ + seb_v.z_ * (float)sebCnt_;
							pos_.y_ = seb_v.y_ * (float)sebCnt_ + seb_ay * (float)sebCnt_ * (float)(sebCnt_ + 1) / 2f;
						}
						else
						{
							pos_.x_ = seb_startPos.x_ + seb_v.x_ * (float)SEB_H_SAVING_ANIME[0] + seb_v.x_ / 3f * (float)(sebCnt_ - SEB_H_SAVING_ANIME[0]);
							pos_.z_ = seb_startPos.z_ + seb_v.z_ * (float)SEB_H_SAVING_ANIME[0] + seb_v.z_ / 3f * (float)(sebCnt_ - SEB_H_SAVING_ANIME[0]);
							pos_.y_ = seb_v.y_ * (float)sebCnt_ + seb_ay * (float)sebCnt_ * (float)(sebCnt_ + 1) / 2f;
							if (pos_.y_ < 0f)
							{
								pos_.y_ = 0f;
							}
						}
						if (sebCnt_ >= SEB_H_SAVING_ANIME[SEB_H_SAVING_ANIME.Length - 1])
						{
							flag_ = BitUtil.Off(flag_, 8);
							script_.RemoveElementAt(0);
							continue;
						}

}
else if ((recoveredScriptCommand == 10)) {
if (sebCnt_ < SEB_H_SLIDING_ANIME[0])
						{
							pos_.x_ += seb_v.x_;
							pos_.z_ += seb_v.z_;
						}
						if (sebCnt_ >= SEB_H_SLIDING_ANIME[SEB_H_SLIDING_ANIME.Length - 1])
						{
							flag_ = BitUtil.Off(flag_, 48);
							script_.RemoveElementAt(0);
							continue;
						}

}
else if ((recoveredScriptCommand == 11)) {
pos_.y_ = seb_v.y_ * (float)sebCnt_ + seb_ay * (float)sebCnt_ * (float)(sebCnt_ + 1) / 2f;
						if (sebCnt_ >= SEB_H_HEAD_ANIME[SEB_H_HEAD_ANIME.Length - 1])
						{
							flag_ = BitUtil.Off(flag_, 64);
							if (pos_.y_ < 0f)
							{
								pos_.y_ = 0f;
							}
							script_.RemoveElementAt(0);
							continue;
						}

}
else if ((recoveredScriptCommand == 12)) {
script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 13)) {
script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 14)) {
{
						int num = sebCnt_ % SEB_H_GOALGLAD_ANIME[1];
						float num2 = GameUtil.SetVStop(8f, SEB_H_GOALGLAD_ANIME[0] / 2);
						float num3 = GameUtil.SetAStop(8f, SEB_H_GOALGLAD_ANIME[0] / 2);
						pos_.y_ = num2 * (float)num + num3 * (float)num * (float)(num + 1) / 2f;
						if (pos_.y_ < 0f)
						{
							pos_.y_ = 0f;
						}

					}
}
else if ((recoveredScriptCommand == 15)) {
script_.RemoveElementAt(0);
						continue;
}
				break;
}
				finally
				{
					insertScriptIndex_ = -1;
				}

			}
			return false;
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
			if (insertScriptIndex_ != -1)
			{
				insertScriptIndex_ = 0;
			}
		}

		public void AddCommandMove(int moveNode_id)
		{
			AddCommand(new int[2] { 0, moveNode_id });
		}

		public void AddCommandWait(int wait, bool flg_disp)
		{
			AddCommand(new int[3]
			{
				1,
				wait,
				(flg_disp) ? (0) : (1)
			});
		}

		public void AddCommandWait(int wait)
		{
			AddCommandWait(wait, true);
		}

		public void AddCommandWaitStop(int wait)
		{
			AddCommandFlagOff(2);
			AddCommandWait(wait, true);
			AddCommandFlagOn(2);
		}

		public void AddCommandChangeState(int state)
		{
			AddCommand(new int[2] { 2, state });
		}

		public void AddCommandChangeSeb(int seb)
		{
			AddCommand(new int[2] { 3, seb });
		}

		public void AddCommandChangeDirect(int direct)
		{
			AddCommand(new int[2] { 4, direct });
		}

		public void AddCommandFlag(int flag)
		{
			AddCommand(new int[2] { 5, flag });
		}

		public void AddCommandFlagOn(int bits)
		{
			AddCommand(new int[2] { 6, bits });
		}

		public void AddCommandFlagOff(int bits)
		{
			AddCommand(new int[2] { 7, bits });
		}

		public void AddCommandSaving()
		{
			AddCommand(new int[1] { 9 });
		}

		public void AddCommandSliding()
		{
			AddCommand(new int[1] { 10 });
		}

		public void AddCommandHead()
		{
			AddCommand(new int[1] { 11 });
		}

		public void AddCommandVolley()
		{
			AddCommand(new int[1] { 12 });
		}

		public void AddCommandThrowIn()
		{
			AddCommand(new int[1] { 13 });
		}

		public void AddCommandGoalGlad()
		{
			AddCommand(new int[1] { 14 });
		}

		public void AddCommandGoalSad()
		{
			AddCommand(new int[1] { 15 });
		}

		public void AddCommand(int[] script)
		{
			script_.AddElement(script);
		}

		public void ChangeState(int next_state)
		{
			state_ = next_state;
			stateCnt_ = 0;
			frame_ = 0;
		}

		public int SetDrawParam(int seb, int frame)
		{
			int num = 0;
			int num2 = 0;
			PlayerData playerData = apdat_.pData_[pData_id_];
			num = playerData.faceImg_;
			TeamData teamData = apdat_.tData_[tData_id_];
			num2 = teamData.bodyImg_;
			return SetDrawParam(num, num2, seb, frame);
		}

		public int SetDrawParam(int head_id, int body_id, int seb, int frame)
		{
			head_id += 300;
			body_id += 100;
			if (role_ == 0)
			{
				if (tData_id_ != -1)
				{
					body_id = apdat_.tData_[tData_id_].keeperImg_ + 200;
				}
				else
				{
					if (udat_ == null)
					{
						udat_ = UserData.GetInstance();
					}
					body_id = 200 + UserData.UNIFORM_KEEPER[udat_.selectUniform_];
				}
			}
			bool flag = false;
			if (body_id >= 200)
			{
				flag = true;
			}
			if (flag && (seb == 2 || seb == 4 || seb == 5 || seb == 6 || seb == 7 || seb == 8 || seb == 9 || seb == 13))
			{
				return 0;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = body_id;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = head_id;
			int num10 = 0;
			int num11 = sebCnt_;
			if (!BitUtil.Check(flag_, 2))
			{
				num11 = 0;
			}
			bool flag2 = true;
			if (direct_ == 0 || direct_ == 3)
			{
				flag2 = false;
			}
			if ((seb == 0)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 52 + direct_;
				num5 = num11 % (SEB_H_WALK_ASPEED * 4) / SEB_H_WALK_ASPEED;
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];
				if (BitUtil.Check(flag_, 256))
				{
					num3 = 36 + direct_;
					num5 = 0;
					num = body_off[4][0];
					num2 = body_off[4][1];
					num6 = num + head_off[4][direct_][num5][0];
					num7 = num2 + head_off[4][direct_][num5][1];
					flag2 = false;
				}

} else if ((seb == 1)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 24 + direct_;
				if (flag)
				{
					num3 = 60 + direct_;
				}
				if (num11 % SEB_H_KICK_ANIME[SEB_H_KICK_ANIME.Length - 1] < SEB_H_KICK_ANIME[0])
				{
					num5 = 0;
				}
				else if (num11 % SEB_H_KICK_ANIME[SEB_H_KICK_ANIME.Length - 1] < SEB_H_KICK_ANIME[1])
				{
					num5 = 1;
				}
				else if (num11 % SEB_H_KICK_ANIME[SEB_H_KICK_ANIME.Length - 1] < SEB_H_KICK_ANIME[2])
				{
					num5 = 2;
				}
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 14)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 56 + direct_;
				num5 = team_id_;
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];
				if ((direct_ == 0) || (direct_ == 2)) { } else {
					if ((direct_ == 1)) { num10 = ((8 + team_id_)); } else { num10 = ((11 - team_id_)); }
					flag2 = false;
				}
				if (num10 == 2)
				{
					flag2 = false;
				}

} else if ((seb == 5)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 32 + direct_;
				if (num11 % SEB_H_SLIDING_ANIME[SEB_H_SLIDING_ANIME.Length - 1] < SEB_H_SLIDING_ANIME[0])
				{
					num5 = 0;
				}
				else if (num11 % SEB_H_SLIDING_ANIME[SEB_H_SLIDING_ANIME.Length - 1] < SEB_H_SLIDING_ANIME[1])
				{
					num5 = 1;
				}
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 2)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 20 + direct_;
				if (num11 % SEB_H_HEAD_ANIME[SEB_H_HEAD_ANIME.Length - 1] < SEB_H_HEAD_ANIME[0])
				{
					num5 = 0;
				}
				else if (num11 % SEB_H_HEAD_ANIME[SEB_H_HEAD_ANIME.Length - 1] < SEB_H_HEAD_ANIME[1])
				{
					num5 = 1;
				}
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];
				if (num5 == 1)
				{
					num10 += 12;
				}

} else if ((seb == 4)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 36 + direct_;
				if (num11 % SEB_H_THROWIN_ANIME[SEB_H_THROWIN_ANIME.Length - 1] < SEB_H_THROWIN_ANIME[0])
				{
					num5 = 0;
				}
				else if (num11 % SEB_H_THROWIN_ANIME[SEB_H_THROWIN_ANIME.Length - 1] < SEB_H_THROWIN_ANIME[1])
				{
					num5 = 1;
				}
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];
				if (num5 == 0 && (direct_ == 0 || direct_ == 3))
				{
					flag2 = false;
				}

} else if ((seb == 3)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 4 + direct_;
				if (flag)
				{
					num3 = 64 + direct_;
				}
				if (num11 % SEB_H_GOALGLAD_ANIME[SEB_H_GOALGLAD_ANIME.Length - 1] < SEB_H_GOALGLAD_ANIME[0])
				{
					num5 = 0;
				}
				else if (num11 % SEB_H_GOALGLAD_ANIME[SEB_H_GOALGLAD_ANIME.Length - 1] < SEB_H_GOALGLAD_ANIME[1])
				{
					num5 = 1;
				}
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				if (num5 == 0) { num10 = (direct_ + 4); } else { num10 = direct_; }
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];
				if (num5 == 0 && (direct_ == 0 || direct_ == 3))
				{
					flag2 = false;
				}

} else if ((seb == 11)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 16 + direct_;
				if (flag)
				{
					num3 = 72 + direct_;
				}
				num5 = 0;
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_ + 16;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 12)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 0 + direct_;
				if (flag)
				{
					num3 = 76 + direct_;
				}
				num5 = 0;
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 6)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 40 + direct_;
				if (num11 % SEB_H_TRAP1_ANIME[SEB_H_TRAP1_ANIME.Length - 1] < SEB_H_TRAP1_ANIME[0])
				{
					num5 = 0;
				}
				else if (num11 % SEB_H_TRAP1_ANIME[SEB_H_TRAP1_ANIME.Length - 1] < SEB_H_TRAP1_ANIME[1])
				{
					num5 = 1;
				}
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 7)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 44 + direct_;
				if (num11 % SEB_H_TRAP2_ANIME[SEB_H_TRAP2_ANIME.Length - 1] < SEB_H_TRAP2_ANIME[0])
				{
					num5 = 0;
				}
				else if (num11 % SEB_H_TRAP2_ANIME[SEB_H_TRAP2_ANIME.Length - 1] < SEB_H_TRAP2_ANIME[1])
				{
					num5 = 1;
				}
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 8)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 8 + direct_;
				num5 = 0;
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_ + 4;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];
				flag2 = false;

} else if ((seb == 9)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 12 + direct_;
				if (num11 % SEB_H_GOALGLAD2_ANIME[SEB_H_GOALGLAD2_ANIME.Length - 1] < SEB_H_GOALGLAD2_ANIME[0])
				{
					num5 = 0;
				}
				else if (num11 % SEB_H_GOALGLAD2_ANIME[SEB_H_GOALGLAD2_ANIME.Length - 1] < SEB_H_GOALGLAD2_ANIME[1])
				{
					num5 = 1;
				}
				else if (num11 % SEB_H_GOALGLAD2_ANIME[SEB_H_GOALGLAD2_ANIME.Length - 1] < SEB_H_GOALGLAD2_ANIME[2])
				{
					num5 = 2;
				}
				else if (num11 % SEB_H_GOALGLAD2_ANIME[SEB_H_GOALGLAD2_ANIME.Length - 1] < SEB_H_GOALGLAD2_ANIME[3])
				{
					num5 = 3;
				}
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_ + 4;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 10)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 28 + direct_;
				if (flag)
				{
					num3 = 68 + direct_;
				}
				num5 = num11 % (SEB_H_WALK_ASPEED * 4) / SEB_H_WALK_ASPEED;
				if (frame != -1)
				{
					num5 = frame;
				}
				if (num5 % 2 == 1)
				{
					num2--;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 13)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 48 + direct_;
				num5 = 0;
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				num10 = direct_;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 15)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 80;
				num5 = 0;
				if (frame_ == 1)
				{
					num5 = 1;
				}
				num8 = 0;
				num10 = 20;
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];

} else if ((seb == 16)) {
num = body_off[seb][0];
				num2 = body_off[seb][1];
				num4 = body_id;
				num3 = 81 + direct_;
				if (num11 % SEB_H_GOALGLAD_ANIME[SEB_H_GOALGLAD_ANIME.Length - 1] < SEB_H_GOALGLAD_ANIME[0])
				{
					num5 = 0;
				}
				else if (num11 % SEB_H_GOALGLAD_ANIME[SEB_H_GOALGLAD_ANIME.Length - 1] < SEB_H_GOALGLAD_ANIME[1])
				{
					num5 = 1;
				}
				if (frame != -1)
				{
					num5 = frame;
				}
				num8 = 0;
				if (num5 == 0) { num10 = (direct_ + 4); } else { num10 = direct_; }
				num6 = num + head_off[seb][direct_][num5][0];
				num7 = num2 + head_off[seb][direct_][num5][1];
				if (num5 == 0 && (direct_ == 0 || direct_ == 3))
				{
					flag2 = false;
				}

}
			draw_param[0] = num6;
			draw_param[1] = num7;
			draw_param[2] = num8;
			draw_param[3] = num9;
			draw_param[4] = num10;
			draw_param[5] = num;
			draw_param[6] = num2;
			draw_param[7] = num3;
			draw_param[8] = num4;
			draw_param[9] = num5;
			draw_param[10] = 0;
			if (flag2)
			{
				return 0;
			}
			return 1;
		}

		public void Draw(Graphics g, int x, int y, int layer_id, bool isSoccerForm)
		{
			if ((apdat_.camera.subPlayer_ != null && pData_id_ == apdat_.camera.subPlayer_.pData_id_ && obj_type_ == 1) || (apdat_.camera.subSvisor_ != null && obj_type_ == 2))
			{
				g.DrawImage(apdat_.resCom2_.img[139], x + draw_param[5] - 5, y + draw_param[6] - 28);
				g.DrawImage(apdat_.resCom2_.img[138], x + draw_param[5] - 8, y + draw_param[6] - 4);
			}
			if (BitUtil.Check(flag_, 262144))
			{
				int frame = frame_ % 12 / 3;
				apdat_.DrawSeb(g, x + draw_param[5], y + draw_param[6], apdat_.resCom2_.seb[68], apdat_.resCom2_.img[76], frame);
			}
			if (layer_id == 0)
			{
				apdat_.DrawSeb(g, x + draw_param[5], y + draw_param[6], apdat_.resHuman2_.seb[draw_param[7]], apdat_.resHuman2_.img[draw_param[8]], draw_param[9]);
				apdat_.DrawSeb(g, x + draw_param[0], y + draw_param[1], apdat_.resHumanHead_.seb[draw_param[2]], apdat_.resHumanHead_.img[draw_param[3]], draw_param[4]);
			}
			else
			{
				apdat_.DrawSeb(g, x + draw_param[0], y + draw_param[1], apdat_.resHumanHead_.seb[draw_param[2]], apdat_.resHumanHead_.img[draw_param[3]], draw_param[4]);
				apdat_.DrawSeb(g, x + draw_param[5], y + draw_param[6], apdat_.resHuman2_.seb[draw_param[7]], apdat_.resHuman2_.img[draw_param[8]], draw_param[9]);
			}
			if (isSoccerForm && (BitUtil.Check(flag_, 4) || actId_ == 1) && udat_.gm_.state_ == 0)
			{
				int physicalHpMax = PlayerData.GetPhysicalHpMax(this);
				if (physical_hp_ < physicalHpMax)
				{
					int num = -27;
					int num2 = GameUtil.RateConvert(physicalHpMax, PlayerData.PHYSICALHP_MIN, PlayerData.PHYSICALHP_MAX, 8, 26, true);
					int num3 = 4;
					g.SetColor(Graphics.GetColorOfRGB(41, 14, 0));
					g.DrawRect(x + draw_param[5] - num2 / 2, y + draw_param[6] + num, num2 - 1, num3 - 1);
					g.SetColor(Graphics.GetColorOfRGB(117, 21, 0));
					g.FillRect(x + draw_param[5] - num2 / 2 + 1, y + draw_param[6] + num + 1, num2 - 2, num3 - 2);
					int num4 = GameUtil.RateConvert(physical_hp_, physicalHpMax, 0, num2 - 2, 0, true);
					g.SetColor(Graphics.GetColorOfRGB(255, GameUtil.RateConvert(physical_hp_, physicalHpMax, 0, 255, 0, true), 0));
					g.FillRect(x + draw_param[5] - num2 / 2 + 1, y + draw_param[6] + num + 1, num4, num3 - 2);
				}
			}
			if (isSoccerForm && SoccerForm.SHOW_DEBUGDRAW2 == 1)
			{
				if (team_id_ == 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 0));
				}
				else
				{
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 255));
				}
				g.DrawString(string.Empty + coliState_, x + draw_param[5], y + draw_param[6] - 40);
			}
		}

		public void Draw(Graphics g, int index)
		{
			if (BitUtil.Check(flag_, 1))
			{
				return;
			}
			Camera.ConvertGlobalToCamera(pos_, camera_pos);
			shadowPos_.x_ = pos_.x_;
			shadowPos_.z_ = pos_.z_;
			Camera.ConvertGlobalToCamera(shadowPos_, camera_pos_shadow);
			if (!GameUtil.ChRectHitRect(camera_pos[0] - 7, camera_pos[1] - 20, 14, 20, SoccerForm.VIEW_X, SoccerForm.VIEW_Y, SoccerForm.VIEW_W, SoccerForm.VIEW_H, false))
			{
				return;
			}
			int depth = camera_pos_shadow[1] + 1;
			bool dark = false;
			int num = flag_;
			int num2 = 1;
			if (seb_ == 11)
			{
				num2 = 0;
			}
			apdat_.InsertGameObject(8, 0, depth, 2, 0, 4, camera_pos_shadow[0], camera_pos_shadow[1] + num2, dark, 0);
			if (seb_ == 5)
			{
				int num3 = sebCnt_ / 3;
				if (num3 <= 2)
				{
					Camera.ConvertGlobalToCamera(seb_startPos, camera_pos_sliding);
					apdat_.InsertGameObject(8, 0, depth, 10, 7, num3, camera_pos_sliding[0], camera_pos_sliding[1] + num2, dark, 0);
				}
			}
			int layer = SetDrawParam(seb_, -1);
			apdat_.InsertGameObject(1, index + team_id_ * 100, depth, 0, 0, 0, camera_pos[0], camera_pos[1], dark, layer);
		}

		public void Proc_kick()
		{
			udat_.ball_.ReleaseBall(this);
			AddCommandChangeSeb(1);
			AddCommandWait(SEB_H_KICK_ANIME[SEB_H_KICK_ANIME.Length - 1]);
			AddCommandChangeSeb(0);
			temp_vec.x_ = udat_.ball_.pos_.x_ + udat_.ball_.v_.x_;
			temp_vec.y_ = 0f;
			temp_vec.z_ = udat_.ball_.pos_.z_ + udat_.ball_.v_.z_;
			Camera.ConvertGlobalToScreen(temp_vec, CH_BALL_DIRECT);
			CH_BALL_DIRECT[0] = CH_BALL_DIRECT[0] - screen_pos_[0];
			CH_BALL_DIRECT[1] = CH_BALL_DIRECT[1] - screen_pos_[1];
			if (CH_BALL_DIRECT[0] > 0)
			{
				if (CH_BALL_DIRECT[1] > 0)
				{
					direct_ = 0;
				}
				else
				{
					direct_ = 1;
				}
			}
			else if (CH_BALL_DIRECT[1] > 0)
			{
				direct_ = 3;
			}
			else
			{
				direct_ = 2;
			}
		}

		public void Proc_throwIn()
		{
			udat_.ball_.ReleaseBall(this);
			flag_ = BitUtil.Off(flag_, 16384);
			seb_ = 0;
			AddCommandChangeSeb(4);
			AddCommandWait(SEB_H_THROWIN_ANIME[SEB_H_THROWIN_ANIME.Length - 1]);
			AddCommandFlagOff(256);
			AddCommandChangeSeb(0);
			if (udat_.ball_.lineOutPos_.x_ > 0f)
			{
				direct_ = 3;
			}
			else
			{
				direct_ = 1;
			}
		}

		public void Proc_saving()
		{
			flag_ = BitUtil.On(flag_, 8);
			AddCommandChangeSeb(14);
			AddCommandSaving();
			AddCommandChangeSeb(0);
		}

		public void Proc_sliding(Player enemy_passerPlayer)
		{
			flag_ = BitUtil.On(flag_, 48);
			AddCommandChangeSeb(5);
			AddCommandSliding();
			AddCommandFlagOff(48);
			AddCommandChangeSeb(0);
			SetPlayerDirect(enemy_passerPlayer.pos_);
		}

		public void Proc_head()
		{
			flag_ = BitUtil.On(flag_, 192);
			AddCommandChangeSeb(2);
			AddCommandHead();
			AddCommandChangeSeb(0);
			SetPlayerDirect(udat_.ball_.pos_);
		}

		public void Proc_down()
		{
			flag_ = BitUtil.On(flag_, 16384);
			AddCommandChangeSeb(12);
			int num = SEB_H_DOWN_ANIME + 1;
			num = (int)PlayerData.AuraUpdate(this, num, 9);
			AddCommandWait(num);
			AddCommandFlagOff(16384);
			AddCommandChangeSeb(0);
		}

		public void SetPlayerDirect(Vector3D goal_pos)
		{
			CH_BALLSUBPOS.x_ = goal_pos.x_ - pos_.x_;
			CH_BALLSUBPOS.z_ = goal_pos.z_ - pos_.z_;
			Camera.ConvertGlobalToScreen(CH_BALLSUBPOS, CH_BALL_DIRECT);
			CH_BALL_DIRECT[0] = CH_BALL_DIRECT[0] - Camera.MAP_SCREEN_BASE_POS[0];
			CH_BALL_DIRECT[1] = CH_BALL_DIRECT[1] - Camera.MAP_SCREEN_BASE_POS[1];
			if (CH_BALL_DIRECT[0] > 0)
			{
				if (CH_BALL_DIRECT[1] > 0)
				{
					direct_ = 0;
				}
				else
				{
					direct_ = 1;
				}
			}
			else if (CH_BALL_DIRECT[1] > 0)
			{
				direct_ = 3;
			}
			else
			{
				direct_ = 2;
			}
		}

		private int GetShootEva()
		{
			Vector3D[] array = null;
			float num = 0f;
			if (team_id_ == 0)
			{
				array = Map.GOAL2_GLOBAL_POS;
				num = (float)JMath.Sqrt((0f - pos_.x_) * (0f - pos_.x_) + (900f - pos_.z_) * (900f - pos_.z_));
			}
			else
			{
				array = Map.GOAL1_GLOBAL_POS;
				num = (float)JMath.Sqrt((0f - pos_.x_) * (0f - pos_.x_) + (-900f - pos_.z_) * (-900f - pos_.z_));
			}
			if (num > 600f)
			{
				return 0;
			}
			int num2 = (int)GameUtil.RateConvert(num, 900f, 0f, 0f, 100f, true);
			GameUtil.Sub(array[0], pos_, vec1);
			GameUtil.Sub(array[1], pos_, vec2);
			float num3 = (float)JMath.Sqrt(vec1.x_ * vec1.x_ + vec1.z_ * vec1.z_);
			float num4 = (float)JMath.Sqrt(vec2.x_ * vec2.x_ + vec2.z_ * vec2.z_);
			float num5 = 0f;
			if (num3 * num4 == 0f) { num5 = 0f; } else { num5 = ((vec1.x_ * vec2.x_ + vec1.z_ * vec2.z_) / (num3 * num4)); }
			int num6 = (int)GameUtil.RateConvert(num5, 1f, 0.5f, 0f, 100f, true);
			int num7 = GameUtil.RateConvert(ballHaveTime_, 0, 10, 0, 100, true);
			int physicalHpMax = PlayerData.GetPhysicalHpMax(this);
			int num8 = GameUtil.RateConvert(physical_hp_, 0, physicalHpMax, 0, 100, true);
			{ int recoveredReturn_89626 = (int)((1f * (float)num2 + 1f * (float)num6 + 0.3f * (float)num7 + 0.3f * (float)num8) / 2.6f); return recoveredReturn_89626; }
		}

		private bool ChSlidingTackleEva(Player target_player)
		{
			int slidingRate = PlayerData.GetSlidingRate(this);
			if (GameUtil.Random(1000) < slidingRate)
			{
				return false;
			}
			float num = (float)JMath.Sqrt((target_player.pos_.x_ - pos_.x_) * (target_player.pos_.x_ - pos_.x_) + (target_player.pos_.z_ - pos_.z_) * (target_player.pos_.z_ - pos_.z_));
			if (!(num >= 30f) || !(num < 70f))
			{
				return false;
			}
			st_a_vec = Map.DIRECT_VEC[target_player.direct_];
			st_b_vec.x_ = target_player.pos_.x_ - pos_.x_;
			st_b_vec.z_ = target_player.pos_.z_ - pos_.z_;
			float num2 = (float)JMath.Sqrt(st_a_vec.x_ * st_a_vec.x_ + st_a_vec.z_ * st_a_vec.z_);
			float num3 = (float)JMath.Sqrt(st_b_vec.x_ * st_b_vec.x_ + st_b_vec.z_ * st_b_vec.z_);
			if (num2 * num3 == 0f)
			{
				return false;
			}
			float x = (st_a_vec.x_ * st_b_vec.x_ + st_a_vec.z_ * st_b_vec.z_) / (num2 * num3);
			int num4 = (int)GameUtil.RateConvert(x, 0f, -1f, 0f, 100f, true);
			int num5 = 0;
			int num6 = eva_pos_;
			int num7 = udat_.teams_[team_id_].Get_playerPosEva(this, address_[0], address_[1], false);
			int num8 = GameUtil.RateConvert(num7 - num6, 20, 0, 0, 100, true);
			int num9 = (int)((0.6f * (float)num4 + 0.2f * (float)num5 + 0.3f * (float)num8) / 1.1f);
			if (num9 > 50)
			{
				return true;
			}
			return false;
		}

		public bool ChHeading()
		{
			Ball ball_ = udat_.ball_;
			if (BitUtil.Check(ball_.flag_, 16) && udat_.gm_.gameState_ != 2)
			{
				return false;
			}
			if (ball_.boundNum_ > 0)
			{
				return false;
			}
			if (JMath.Abs(ball_.address_[0] - address_[0]) + JMath.Abs(ball_.address_[1] - address_[1]) > 6)
			{
				return false;
			}
			int num = 0;
			for (int i = 1; i < udat_.teams_[1 - team_id_].players_.Length; i++)
			{
				Player player = udat_.teams_[1 - team_id_].players_[i];
				if (player.address_[0] == address_[0] && player.address_[1] == address_[1])
				{
					num++;
					break;
				}
			}
			if (num == 0)
			{
				return false;
			}
			int headingStartFrame = GameUtil.Random(2) - 1 + SEB_H_HEAD_ANIME[1] / 2;
			int headIndexFormSavePos = ball_.GetHeadIndexFormSavePos2(headingStartFrame, PlayerData.GetHeadJampDisTop(this));
			if (headIndexFormSavePos == -1)
			{
				return false;
			}
			Vector3D vector3D = ball_.savePos_[headIndexFormSavePos];
			int[] collisionData = udat_.GetCollisionData(0, 4);
			int[] collisionData2 = udat_.GetCollisionData(1, 1);
			if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], vector3D.x_ + (float)collisionData2[0], vector3D.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true))
			{
				return true;
			}
			return false;
		}

		public static void SetEfCharge(Player charger, Player receiver, bool sliding)
		{
			int sx = (charger.screen_pos_[0] + receiver.screen_pos_[0]) / 2 + GameUtil.Random(10) - 5;
			int num = (charger.screen_pos_[1] + receiver.screen_pos_[1]) / 2 + GameUtil.Random(10) - 5 + 5;
			if (sliding)
			{
				num -= 4;
			}
			apdat_.AddEffectCharge(sx, num);
		}
	}
}
