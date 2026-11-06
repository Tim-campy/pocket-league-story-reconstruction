using System;
using data;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace game
{
	public class Team
	{
		public const int F_X = 0;

		public const int F_Y = 1;

		public const int F_W = 2;

		public const int F_H = 3;

		public const int F_END = 4;

		public const int ROLE_GK = 0;

		public const int ROLE_DF = 1;

		public const int ROLE_MF = 2;

		public const int ROLE_FW = 3;

		public const int ROLE_END = 4;

		public const float TEAM_FRECT_X_MIN = 0f;

		public const float TEAM_FRECT_X_MAX = 0f;

		public const float TEAM_FRECT_Y_MIN = -1530f;

		public const float TEAM_FRECT_Y_MAX = 360f;

		public const float TEAM_FRECT_W_MIN = 450f;

		public const float TEAM_FRECT_W_MAX = 450f;

		public const float TEAM_FRECT_H_MIN = 720f;

		public const float TEAM_FRECT_H_MAX = 1260f;

		public const float TEAM_FRECT_Y_ADD_CHARGE_MIN = 90f;

		public const float TEAM_FRECT_Y_ADD_CHARGE_MAX = 90f;

		public const float TEAM_FRECT_Y_ADD_DEFFENCE_MIN = -270f;

		public const float TEAM_FRECT_Y_ADD_DEFFENCE_MAX = -270f;

		public const float FRECT_FRONT_START_H = 450f;

		public const float FRECT_BACK_START_H = 540f;

		public const float FRECT_BACK_DEFENCE_H = 450f;

		public const int AMEFTO_ONECHARGETIME_MIN = 50;

		public const int AMEFTO_ONECHARGETIME_MAX = 10;

		public const int AMEFTO_PZID_NORMAL = 0;

		public const int AMEFTO_PZID_DRIBBLE = 1;

		public const int AMEFTO_PZID_LONGPASS = 2;

		public const int AMEFTO_PZID_END = 3;

		public const int RECIVERPIDBOX_MAX = 5;

		private const float CPM1 = 0.2f;

		private const float CPM2 = 0.4f;

		private const float CPM3 = 0.7f;

		public const int COATEVA_ROOT_NUM = 9;

		public const int COATEVA_PASSER_BASE = 30;

		public const int DEF1 = 100;

		public const int DEF2 = 80;

		public const int DEF3 = 60;

		public const int DEF4 = 40;

		public const int DEF5 = 20;

		public const int DEF6 = 0;

		public const int CALMODE_PYRAMID = 0;

		public const int CALMODE_RECT = 1;

		public const int CH_CEVA_PASSER_DEFAULT = 0;

		public const int CH_CEVA_PASSER_FRINDADD = 1;

		public const int CH_CEVA_PASSER_ENDDIV = 2;

		public const int CH_CEVA_FREERUN_DEFAULT = 3;

		public const int CH_CEVA_FREERUN_ENDDIV = 4;

		public const int CH_CEVA_FREERUN_PASSSUPORT = 5;

		public const int CH_CEVA_DEFENCE_DEFAULT = 6;

		public const int CH_CEVA_DEFENCE_PASSER = 7;

		public const int CH_CEVA_DEFENCE_ALLYDIV = 8;

		public const int CH_CEVA_LOOSEBALL_DEFAULT = 9;

		public const int CH_CEVA_LOOSEBALL_BALLPOS = 10;

		public const int CH_CEVA_LOOSEBALL_ALLYDIV = 11;

		public const int CH_CEVA_END = 12;

		public const int CH_DATA_POSEVA = 0;

		public const int CH_DATA_RECEIVEEVA = 1;

		public const int CH_DATA_ADDRESS = 2;

		public const int CH_DATA_POS = 3;

		public const int CH_DATA_FLAG = 4;

		public const int CH_DATA_COLISTATE = 5;

		public const int CH_DATA_END = 6;

		private const float w_coatEvaSum = 0.7f;

		private const float w_ballLine = 0.3f;

		public AppData apdat_;

		private UserData udat_;

		public int team_id_;

		public int tDataId_;

		public int formationType_;

		public Player[] players_;

		public int lastKickeer_;

		public int amefto_pz;

		public int amefto_oneChargeTime;

		public float[] fPos_;

		public int[] CORRECT_LINE;

		public int[] NOW_LINE;

		public static readonly Vector3D[] FORMATION_KICKOFF = new Vector3D[2]
		{
			new Vector3D(-30f, 0f, -20f),
			new Vector3D(-5f, 0f, -30f)
		};

		public int[] team_policyEva;

		public int[][] team_eva;

		public float[][] team_frect;

		public Vector3D[] frect_pos;

		public int[][] frect_cpos;

		public static int AMEFTO_ONECHARGETIME_PLAYER = 40;

		public static int[] AMEFTO_PZ = new int[3] { 180, 90, 360 };

		public float startZ_;

		public float goalZ_;

		public int chargeNum_;

		public int chargeCnt_;

		public int eva_risk_;

		public Vector3D[] amefto_goalPos_;

		public int[][] amefto_cameraPos_;

		public int[][] amefto_address_;

		public static readonly int[] AMEFTO_PASSER_COATEVA = new int[4] { 10, 25, 50, 100 };

		public static readonly int[] AMEFTO_FRECT_GOALZ = new int[4] { 60, 50, 40, 30 };

		public int[] receiverPIdBox_;

		internal float[] CEVA_PASSER_MP2 = new float[9] { 0.7f, 0.4f, 0.7f, 0.4f, 0.2f, 0.4f, 0.7f, 0.4f, 0.7f };

		internal float[] CEVA_PASSER_MP2_DIV = new float[9] { 2.857143f, 5f, 2.857143f, 5f, 10f, 5f, 2.857143f, 5f, 2.857143f };

		internal float[] CEVA_PASSER_MP3 = new float[9] { 1f, 0.99f, 1f, 0.99f, 0.95f, 0.99f, 1f, 0.99f, 1f };

		internal int[] CEVA_PASSER_AP = new int[6] { 50, 20, 0, 0, 0, 0 };

		internal int[] CEVA_PASSER_ENEAP = new int[6] { -50, -20, 0, 0, 0, 0 };

		public int[][] coatEva_passer;

		public int[][] coatEva_freeRun;

		public int[][] coatEva_defence;

		public int[][] coatEva_looseBall;

		public int[][] coatEva_passer2;

		public int[][] coatEva_freeRun2;

		public int[][] coatEva_new_offence;

		public int[][] coatEva_new_offence_passer;

		public int[][] coatEva_new_deffence;

		public static readonly int[][] PANEL_DIRECT_1 = new int[1][] { new int[2] };

		public static readonly int[][] PANEL_DIRECT_2 = new int[4][]
		{
			new int[2] { 0, -1 },
			new int[2] { -1, 0 },
			new int[2] { 1, 0 },
			new int[2] { 0, 1 }
		};

		public static readonly int[][] PANEL_DIRECT_3 = new int[8][]
		{
			new int[2] { 0, -2 },
			new int[2] { -1, -1 },
			new int[2] { 1, -1 },
			new int[2] { -2, 0 },
			new int[2] { 2, 0 },
			new int[2] { -1, 1 },
			new int[2] { 1, 1 },
			new int[2] { 0, 2 }
		};

		public static readonly int[][] PANEL_DIRECT_4 = new int[12][]
		{
			new int[2] { 0, -3 },
			new int[2] { -1, -2 },
			new int[2] { 1, -2 },
			new int[2] { -2, -1 },
			new int[2] { 2, -1 },
			new int[2] { -3, 0 },
			new int[2] { 3, 0 },
			new int[2] { -2, 1 },
			new int[2] { 2, 1 },
			new int[2] { -1, 2 },
			new int[2] { 1, 2 },
			new int[2] { 0, 3 }
		};

		public static readonly int[][] PANEL_DIRECT_5 = new int[16][]
		{
			new int[2] { 0, -4 },
			new int[2] { -1, -3 },
			new int[2] { 1, -3 },
			new int[2] { -2, -2 },
			new int[2] { 2, -2 },
			new int[2] { -3, -1 },
			new int[2] { 3, -1 },
			new int[2] { -4, 0 },
			new int[2] { 4, 0 },
			new int[2] { -3, 1 },
			new int[2] { 3, 1 },
			new int[2] { -2, 2 },
			new int[2] { 2, 2 },
			new int[2] { -1, 3 },
			new int[2] { 1, 3 },
			new int[2] { 0, 4 }
		};

		public static readonly int[][] PANEL_DIRECT_6 = new int[20][]
		{
			new int[2] { 0, -5 },
			new int[2] { -1, -4 },
			new int[2] { 1, -4 },
			new int[2] { -2, -3 },
			new int[2] { 2, -3 },
			new int[2] { -3, -2 },
			new int[2] { 3, -2 },
			new int[2] { -4, -1 },
			new int[2] { 4, -1 },
			new int[2] { -5, 0 },
			new int[2] { 5, 0 },
			new int[2] { -4, 1 },
			new int[2] { 4, 1 },
			new int[2] { -3, 2 },
			new int[2] { 3, 2 },
			new int[2] { -2, 3 },
			new int[2] { 2, 3 },
			new int[2] { -1, 4 },
			new int[2] { 1, 4 },
			new int[2] { 0, 5 }
		};

		public static readonly int[][][] PANEL_DIRECT = new int[6][][] { PANEL_DIRECT_1, PANEL_DIRECT_2, PANEL_DIRECT_3, PANEL_DIRECT_4, PANEL_DIRECT_5, PANEL_DIRECT_6 };

		public static readonly int[][] PANEL_DIRECT_R_1 = new int[1][] { new int[2] };

		public static readonly int[][] PANEL_DIRECT_R_2 = new int[8][]
		{
			new int[2] { -1, -1 },
			new int[2] { 0, -1 },
			new int[2] { 1, -1 },
			new int[2] { -1, 0 },
			new int[2] { 1, 0 },
			new int[2] { -1, 1 },
			new int[2] { 0, 1 },
			new int[2] { 1, 1 }
		};

		public static readonly int[][] PANEL_DIRECT_R_3 = new int[16][]
		{
			new int[2] { -2, -2 },
			new int[2] { -1, -2 },
			new int[2] { 0, -2 },
			new int[2] { 1, -2 },
			new int[2] { 2, -2 },
			new int[2] { -2, -1 },
			new int[2] { 2, -1 },
			new int[2] { -2, 0 },
			new int[2] { 2, 0 },
			new int[2] { -2, 1 },
			new int[2] { 2, 1 },
			new int[2] { -2, 2 },
			new int[2] { -1, 2 },
			new int[2] { 0, 2 },
			new int[2] { 1, 2 },
			new int[2] { 2, 2 }
		};

		public static readonly int[][] PANEL_DIRECT_R_4 = new int[24][]
		{
			new int[2] { -3, -3 },
			new int[2] { -2, -3 },
			new int[2] { -1, -3 },
			new int[2] { 0, -3 },
			new int[2] { 1, -3 },
			new int[2] { 2, -3 },
			new int[2] { 3, -3 },
			new int[2] { -3, -2 },
			new int[2] { 3, -2 },
			new int[2] { -3, -1 },
			new int[2] { 3, -1 },
			new int[2] { -3, 0 },
			new int[2] { 3, 0 },
			new int[2] { -3, 1 },
			new int[2] { 3, 1 },
			new int[2] { -3, 2 },
			new int[2] { 3, 2 },
			new int[2] { -3, 3 },
			new int[2] { -2, 3 },
			new int[2] { -1, 3 },
			new int[2] { 0, 3 },
			new int[2] { 1, 3 },
			new int[2] { 2, 3 },
			new int[2] { 3, 3 }
		};

		public static readonly int[][][] PANEL_DIRECT_R = new int[4][][] { PANEL_DIRECT_R_1, PANEL_DIRECT_R_2, PANEL_DIRECT_R_3, PANEL_DIRECT_R_4 };

		public static readonly int[][] PANEL_DIRECT_R_9BLOCK = new int[9][]
		{
			new int[2] { -1, -1 },
			new int[2] { 0, -1 },
			new int[2] { 1, -1 },
			new int[2] { -1, 0 },
			new int[2],
			new int[2] { 1, 0 },
			new int[2] { -1, 1 },
			new int[2] { 0, 1 },
			new int[2] { 1, 1 }
		};

		public static int[][] DEAFULT_FORMATION_EVA;

		public static int[][] DEAFULT_FORMATION_EVA_PENALTYAREA;

		public static int[] FORMATION_TYPE_RATE = new int[3] { 0, 50, -50 };

		public static int FORMATION_RATE_DEFFECNE = -70;

		public Vector ballNearPlayer_ = new Vector();

		public Vector players_receive_ = new Vector();

		public bool ternCharge;

		public static readonly string[] CH_COATEVA_STR = new string[0];

		public static int[] ch_ceva = new int[12];

		public static readonly string[] CH_DATA_STR = new string[0];

		private float w_disBoal = 0.5f;

		private float w_formation = 0.5f;

		private static int[] mostEva_address = new int[2];

		private static float RPP_W_COATEVASUM = 0.5f;

		private static float RPP_W_DISTOGOAL_NORMAL = 0.6f;

		private static float RPP_W_DISTOGOAL_PAREA = 3f;

		private static float RPP_W_AROUNDENEMY = 0.6f;

		private static float PRP_W_DIS = 1.4f;

		private static Vector3D team_formationCenterPos = new Vector3D();

		private static readonly int[][][] GOAL_GLAD_AROUND_POS = new int[2][][]
		{
			new int[5][]
			{
				new int[4] { -60, 60, 60, 60 },
				new int[4] { -30, 70, 70, 70 },
				new int[4] { -70, 30, 70, 70 },
				new int[4] { -30, 70, 30, -70 },
				new int[4] { -70, 30, 30, -70 }
			},
			new int[5][]
			{
				new int[4] { -60, 60, 60, 60 },
				new int[4] { -30, 70, 70, 70 },
				new int[4] { -70, 30, 70, 70 },
				new int[4] { -30, 70, 70, -30 },
				new int[4] { -70, 30, 70, -30 }
			}
		};

		public Team()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
		}

		public void Init(bool isUserTeam, int team_id, int tDataId)
		{
			team_id_ = team_id;
			tDataId_ = tDataId;
			TeamData teamData = apdat_.tData_[tDataId];
			if (isUserTeam)
			{
				formationType_ = udat_.selectFormationType_;
			}
			else
			{
				formationType_ = teamData.formationType_;
			}
			fPos_ = new float[4];
			Init_coatEva();
			fPos_[0] = 0f;
			fPos_[1] = (int)(-450f + 900f * (float)team_id);
			fPos_[2] = 900f;
			fPos_[3] = 900f;
			int[] array = new int[11];
			if (isUserTeam)
			{
				for (int i = 0; i < array.Length; i++)
				{
					PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(i);
					array[i] = playerData.id_;
				}
			}
			else
			{
				for (int j = 0; j < array.Length; j++)
				{
					PlayerData playerData2 = udat_.cpuPlayerDatas_[team_id][j];
					array[j] = playerData2.id_;
				}
			}
			players_ = new Player[11];
			for (int k = 0; k < players_.Length; k++)
			{
				players_[k] = new Player(0);
				players_[k].Init(array[k], k, team_id_, tDataId);
			}
			for (int l = 0; l < players_.Length; l++)
			{
				SetPosFPos(players_[l], players_[l].pos_);
				SetFormationParam(players_[l]);
				if (team_id == 1)
				{
					players_[l].direct_ = 2;
				}
			}
			SetFormationRectParam();
			CORRECT_LINE = new int[4];
			NOW_LINE = new int[4];
			team_policyEva = new int[4];
			team_eva = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			team_frect = JTool.MakeArray<float[][]>(new int[2] { 4, 2 });
			frect_pos = new Vector3D[4];
			for (int m = 0; m < frect_pos.Length; m++)
			{
				frect_pos[m] = new Vector3D();
				if (m == 1 || m == 2)
				{
					frect_pos[m].x_ = 450f;
				}
				else
				{
					frect_pos[m].x_ = -450f;
				}
			}
			frect_cpos = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			amefto_goalPos_ = new Vector3D[2];
			for (int n = 0; n < amefto_goalPos_.Length; n++)
			{
				amefto_goalPos_[n] = new Vector3D();
			}
			amefto_goalPos_[0].x_ = -450f;
			amefto_goalPos_[1].x_ = 450f;
			amefto_cameraPos_ = JTool.MakeArray<int[][]>(new int[2] { 2, 2 });
			amefto_address_ = JTool.MakeArray<int[][]>(new int[2] { 2, 2 });
			receiverPIdBox_ = new int[5];
			if (isUserTeam)
			{
				amefto_pz = AMEFTO_PZ[0];
				amefto_oneChargeTime = AMEFTO_ONECHARGETIME_PLAYER;
			}
			else
			{
				amefto_pz = teamData.amefto_pz_;
				amefto_oneChargeTime = GameUtil.RateConvert(teamData.warP_paramPhysical_[2], 0, 1000, 50, 10, true);
			}
			for (int num = 0; num < ch_ceva.Length; num++)
			{
				ch_ceva[num] = 1;
			}
		}

		public void Update()
		{
			if (udat_.gm_.state_ == 0)
			{
				if (players_receive_.Size() > 0)
				{
					players_receive_.RemoveAllElements();
				}
				Amefto();
				Update_formationRect2();
				Update_coatEva();
				for (int i = 0; i < players_.Length; i++)
				{
					players_[i].eva_pos_ = Get_playerPosEva(players_[i], players_[i].address_[0], players_[i].address_[1]);
					players_[i].eva_receivePos_ = Get_playerReceivePosEva(players_[i], players_[i].address_[0], players_[i].address_[1]);
					players_[i].eva_receivePos_ = (int)PlayerData.AuraUpdate(players_[i], players_[i].eva_receivePos_, 8);
				}
				if (udat_.gm_.gameState_ == 2)
				{
					if (!BitUtil.Check(udat_.ball_.flag_, 16))
					{
						SetSortGroundBallNear();
						Player player = null;
						if ((((udat_.ball_.lastOwner_ != null) && (udat_.ball_.lastOwner_.team_id_ == team_id_)) && (udat_.ball_.targetPId != -1))) { player = (players_[udat_.ball_.targetPId]); } else { player = (((Player)ballNearPlayer_.FirstElement())); }
						for (int num = receiverPIdBox_.Length - 1; num >= 1; num--)
						{
							receiverPIdBox_[num] = receiverPIdBox_[num - 1];
						}
						receiverPIdBox_[0] = player.p_id_;
						int p_id_ = player.p_id_;
						for (int j = 1; (j < receiverPIdBox_.Length); j++)
{
if ((p_id_ != receiverPIdBox_[j])) { break; }
if (j == receiverPIdBox_.Length - 1)
							{
								players_receive_.AddElement(player);
							}
}
					}
					else
					{
						SetSortJampBallNear();
						Player player2 = null;
						if (udat_.ball_.lastOwner_ != null && udat_.ball_.lastOwner_.team_id_ == team_id_ && udat_.ball_.targetPId != -1)
						{
							player2 = players_[udat_.ball_.targetPId];
							players_receive_.AddElement(player2);
						}
						else
						{
							player2 = (Player)ballNearPlayer_.FirstElement();
							int num2 = (int)player2.temp_len;
							players_receive_.AddElement(player2);
							for (int k = 0; k < ballNearPlayer_.Size(); k++)
							{
								Player player3 = (Player)ballNearPlayer_.ElementAt(k);
								if (player3.temp_len > (float)(num2 - 2))
								{
									players_receive_.AddElement(player3);
									if (players_receive_.Size() >= 3)
									{
										break;
									}
								}
							}
						}
					}
				}
				else if ((udat_.gm_.gameState_ == team_id_) || (udat_.gm_.gameState_ == 1 - team_id_)) { } else {
				}
				for (int l = 0; l < players_.Length; l++)
				{
					players_[l].Update();
				}
			}
			else if (udat_.gm_.state_ == 2 || udat_.gm_.state_ == 1)
			{
				for (int m = 0; m < players_.Length; m++)
				{
					players_[m].Update();
				}
			}
			else if (udat_.gm_.state_ == 3 || udat_.gm_.state_ == 4 || udat_.gm_.state_ == 5)
			{
				if (players_receive_.Size() > 0)
				{
					players_receive_.RemoveAllElements();
				}
				Update_coatEva();
				for (int n = 0; n < players_.Length; n++)
				{
					players_[n].eva_pos_ = Get_playerPosEva(players_[n], players_[n].address_[0], players_[n].address_[1]);
					players_[n].eva_receivePos_ = Get_playerReceivePosEva(players_[n], players_[n].address_[0], players_[n].address_[1]);
				}
				Amefto();
				Update_formationRect2();
				for (int num3 = 0; num3 < players_.Length; num3++)
				{
					players_[num3].Update();
				}
			}
			else if (udat_.gm_.state_ != 7 && udat_.gm_.state_ != 6 && udat_.gm_.state_ == 8)
			{
				for (int num4 = 0; num4 < players_.Length; num4++)
				{
					players_[num4].Update();
				}
			}
		}

		public void Collision()
		{
			if (udat_.gm_.state_ == 0)
			{
				for (int i = 0; i < players_.Length; i++)
				{
					players_[i].Collision();
				}
			}
		}

		public void Draw(Graphics g, bool offscreen)
		{
			for (int i = 0; i < players_.Length; i++)
			{
				players_[i].Draw(g, i);
			}
		}

		public void SetPosFPos(Player player, Vector3D var_out)
		{
			int[][] formation_ = apdat_.formationData_[udat_.gm_.team_formationPage_[player.team_id_]].formation_;
			int p_id_ = player.p_id_;
			if (player.team_id_ == 0)
			{
				var_out.x_ = GameUtil.RateConvert(formation_[p_id_][0], 0f, 8f, fPos_[0] - fPos_[2] / 2f + fPos_[2] / 9f / 2f, fPos_[0] + fPos_[2] / 2f - fPos_[2] / 9f / 2f, true);
				var_out.y_ = 0f;
				var_out.z_ = GameUtil.RateConvert(formation_[p_id_][1], 8f, 0f, fPos_[1] - fPos_[3] / 2f + fPos_[3] / 9f / 2f, fPos_[1] + fPos_[3] / 2f - fPos_[3] / 9f / 2f, true);
			}
			else
			{
				var_out.x_ = GameUtil.RateConvert(formation_[p_id_][0], 0f, 8f, fPos_[0] + fPos_[2] / 2f - fPos_[2] / 9f / 2f, fPos_[0] - fPos_[2] / 2f + fPos_[2] / 9f / 2f, true);
				var_out.y_ = 0f;
				var_out.z_ = GameUtil.RateConvert(formation_[p_id_][1], 8f, 0f, fPos_[1] + fPos_[3] / 2f - fPos_[3] / 9f / 2f, fPos_[1] - fPos_[3] / 2f + fPos_[3] / 9f / 2f, true);
			}
			if (player.p_id_ == 0)
			{
				player.role_ = 0;
			}
			else if (formation_[p_id_][1] <= 2)
			{
				player.role_ = 3;
			}
			else if (formation_[p_id_][1] <= 5)
			{
				player.role_ = 2;
			}
			else if (formation_[p_id_][1] <= 8)
			{
				player.role_ = 1;
			}
			if (player.pData_id_ > 0)
			{
				apdat_.pData_[player.pData_id_].role_ = player.role_;
			}
		}

		public void SetFormationParam(Player player)
		{
			if (player.team_id_ == 0)
			{
				player.formation_[0] = (int)GameUtil.RateConvert(player.pos_.x_, fPos_[0] - fPos_[2] / 2f, fPos_[0] + fPos_[2] / 2f, 0f, 100f, true);
				player.formation_[1] = (int)GameUtil.RateConvert(player.pos_.z_, fPos_[1] - fPos_[3] / 2f, fPos_[1] + fPos_[3] / 2f, 0f, 100f, true);
			}
			else
			{
				player.formation_[0] = (int)GameUtil.RateConvert(player.pos_.x_, fPos_[0] + fPos_[2] / 2f, fPos_[0] - fPos_[2] / 2f, 0f, 100f, true);
				player.formation_[1] = (int)GameUtil.RateConvert(player.pos_.z_, fPos_[1] + fPos_[3] / 2f, fPos_[1] - fPos_[3] / 2f, 0f, 100f, true);
			}
		}

		public void SetFormationRectParam()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 3;
			int num6 = 10;
			for (int i = 0; i < players_.Length; i++)
			{
				Player player = players_[i];
				num = 100 - player.formation_[1];
				for (int j = 0; j < players_.Length; j++)
				{
					if (j == i) { continue; }
Player player2 = players_[j];
int num7 = player2.formation_[1] - player.formation_[1];
if (num > num7 && num7 > num5)
						{
							num = num7;
						}
				}
				if (num != 100 - player.formation_[1])
				{
					num /= 2;
				}
				num2 = 0 + player.formation_[1];
				for (int k = 0; k < players_.Length; k++)
				{
					if (k == i) { continue; }
Player player3 = players_[k];
int num8 = player.formation_[1] - player3.formation_[1];
if (num2 > num8 && num8 > num5)
						{
							num2 = num8;
						}
				}
				if (num2 != 0 + player.formation_[1])
				{
					num2 /= 2;
				}
				num3 = 0 + player.formation_[0];
				for (int l = 0; l < players_.Length; l++)
				{
					if (l == i) { continue; }
Player player4 = players_[l];
int num9 = player.formation_[0] - player4.formation_[0];
if (num3 > num9 && num9 > num5 && JMath.Abs(player.formation_[1] - player4.formation_[1]) < num6)
						{
							num3 = num9;
						}
				}
				if (num3 != 0 + player.formation_[0])
				{
					num3 /= 2;
				}
				num4 = 100 - player.formation_[0];
				for (int m = 0; m < players_.Length; m++)
				{
					if (m == i) { continue; }
Player player5 = players_[m];
int num10 = player5.formation_[0] - player.formation_[0];
if (num4 > num10 && num10 > num5 && JMath.Abs(player.formation_[1] - player5.formation_[1]) < num6)
						{
							num4 = num10;
						}
				}
				if (num4 != 100 - player.formation_[0])
				{
					num4 /= 2;
				}
				if (team_id_ == 0)
				{
					player.formation_param_[0][0] = player.formation_[0] - num3;
					player.formation_param_[0][1] = player.formation_[1] + num;
					player.formation_param_[1][0] = player.formation_[0] + num4;
					player.formation_param_[1][1] = player.formation_[1] + num;
					player.formation_param_[2][0] = player.formation_[0] + num4;
					player.formation_param_[2][1] = player.formation_[1] - num2;
					player.formation_param_[3][0] = player.formation_[0] - num3;
					player.formation_param_[3][1] = player.formation_[1] - num2;
					player.formation_param_[4][0] = player.formation_[0];
					player.formation_param_[4][1] = player.formation_[1];
				}
				else
				{
					player.formation_param_[0][0] = player.formation_[0] + num4;
					player.formation_param_[0][1] = player.formation_[1] - num2;
					player.formation_param_[1][0] = player.formation_[0] - num3;
					player.formation_param_[1][1] = player.formation_[1] - num2;
					player.formation_param_[2][0] = player.formation_[0] - num3;
					player.formation_param_[2][1] = player.formation_[1] + num;
					player.formation_param_[3][0] = player.formation_[0] + num4;
					player.formation_param_[3][1] = player.formation_[1] + num;
					player.formation_param_[4][0] = player.formation_[0];
					player.formation_param_[4][1] = player.formation_[1];
				}
			}
		}

		public void SetSortGroundBallNear()
		{
			if (ballNearPlayer_.Size() > 0)
			{
				ballNearPlayer_.RemoveAllElements();
			}
			Ball ball_ = udat_.ball_;
			ball_.temp_pos.x_ = (ball_.pos_.x_ * 0.5f + (ball_.pos_.x_ + ball_.v_.x_ * 5f) * 0.5f) / 1f;
			ball_.temp_pos.z_ = (ball_.pos_.z_ * 0.5f + (ball_.pos_.z_ + ball_.v_.z_ * 5f) * 0.5f) / 1f;
			for (int i = 0; i < players_.Length; i++)
			{
				if (players_[i].role_ == 0) { continue; }
Player player = players_[i];
if (player.seb_ == 12) { continue; }
player.temp_len = (float)JMath.Sqrt((ball_.temp_pos.x_ - player.pos_.x_) * (ball_.temp_pos.x_ - player.pos_.x_) + (ball_.temp_pos.z_ - player.pos_.z_) * (ball_.temp_pos.z_ - player.pos_.z_));
ballNearPlayer_.AddElement(player);
			}
			for (int j = 0; j < ballNearPlayer_.Size() - 1; j++)
			{
				for (int num = ballNearPlayer_.Size() - 1; num > j; num--)
				{
					Player player2 = (Player)ballNearPlayer_.ElementAt(j);
					Player player3 = (Player)ballNearPlayer_.ElementAt(num);
					if (player3.temp_len < player2.temp_len)
					{
						ballNearPlayer_.SetElementAt(player3, j);
						ballNearPlayer_.SetElementAt(player2, num);
					}
				}
			}
		}

		public void SetSortJampBallNear()
		{
			if (ballNearPlayer_.Size() > 0)
			{
				ballNearPlayer_.RemoveAllElements();
			}
			Ball ball_ = udat_.ball_;
			ball_.temp_pos.Set(ball_.fallPos_);
			w_disBoal = GameUtil.RateConvert(ball_.savePosFrame_ * 100 / ball_.fallTime_, 0f, 100f, 0.7f, 1f, true);
			w_formation = 1f - w_disBoal;
			float num;
float num2;
for (int i = 0; i < players_.Length; i++)
			{
				{ bool recoveredCondition_24616 = players_[i].role_ != 0;
if (recoveredCondition_24616)
				{
					Player player = players_[i];
					if (player.seb_ != 12)
					{
						num = (float)JMath.Sqrt((ball_.temp_pos.x_ - player.pos_.x_) * (ball_.temp_pos.x_ - player.pos_.x_) + (ball_.temp_pos.z_ - player.pos_.z_) * (ball_.temp_pos.z_ - player.pos_.z_));
						num2 = (float)JMath.Sqrt((player.formationGo_.x_ - player.pos_.x_) * (player.formationGo_.x_ - player.pos_.x_) + (player.formationGo_.z_ - player.pos_.z_) * (player.formationGo_.z_ - player.pos_.z_));
						player.temp_len = (w_disBoal * num + w_formation * num2) / (w_disBoal + w_formation);
						player.temp_len = GameUtil.RateConvert(player.temp_len, 225f, 0f, 0f, 100f, true);
						ballNearPlayer_.AddElement(player);
					}
				}
}
			}
			for (int j = 0; j < ballNearPlayer_.Size() - 1; j++)
			{
				for (int num3 = ballNearPlayer_.Size() - 1; num3 > j; num3--)
				{
					Player player2 = (Player)ballNearPlayer_.ElementAt(j);
					Player player3 = (Player)ballNearPlayer_.ElementAt(num3);
					if (player3.temp_len > player2.temp_len)
					{
						ballNearPlayer_.SetElementAt(player3, j);
						ballNearPlayer_.SetElementAt(player2, num3);
					}
				}
			}
		}

		public void Init_coatEva()
		{
			coatEva_passer = JTool.MakeArray<int[][]>(new int[2]
			{
				Map.MAPCHIP_COAT_HNUM,
				Map.MAPCHIP_COAT_WNUM
			});
			coatEva_freeRun = JTool.MakeArray<int[][]>(new int[2]
			{
				Map.MAPCHIP_COAT_HNUM,
				Map.MAPCHIP_COAT_WNUM
			});
			coatEva_defence = JTool.MakeArray<int[][]>(new int[2]
			{
				Map.MAPCHIP_COAT_HNUM,
				Map.MAPCHIP_COAT_WNUM
			});
			coatEva_looseBall = JTool.MakeArray<int[][]>(new int[2]
			{
				Map.MAPCHIP_COAT_HNUM,
				Map.MAPCHIP_COAT_WNUM
			});
			coatEva_passer2 = JTool.MakeArray<int[][]>(new int[2]
			{
				Map.MAPCHIP_COAT_HNUM,
				Map.MAPCHIP_COAT_WNUM
			});
			coatEva_freeRun2 = JTool.MakeArray<int[][]>(new int[2]
			{
				Map.MAPCHIP_COAT_HNUM,
				Map.MAPCHIP_COAT_WNUM
			});
			coatEva_new_offence = JTool.MakeArray<int[][]>(new int[2]
			{
				Map.MAPCHIP_COAT_HNUM,
				Map.MAPCHIP_COAT_WNUM
			});
			coatEva_new_deffence = JTool.MakeArray<int[][]>(new int[2]
			{
				Map.MAPCHIP_COAT_HNUM,
				Map.MAPCHIP_COAT_WNUM
			});
			coatEva_new_offence_passer = JTool.MakeArray<int[][]>(new int[2]
			{
				Map.MAPCHIP_COAT_HNUM,
				Map.MAPCHIP_COAT_WNUM
			});
		}

		public void Update_coatEva()
		{
			if (ChWhichChargeTern())
			{
				Update_coatEvaNewOffence();
			}
			else
			{
				Update_coatEvaNewDeffence();
			}
		}

		public void Update_coatEvaNewOffence()
		{
			for (int i = 0; i < Map.MAPCHIP_COAT_HNUM; i++)
			{
				for (int j = 0; j < Map.MAPCHIP_COAT_WNUM; j++)
				{
					coatEva_new_offence[i][j] = 0;
					coatEva_new_offence_passer[i][j] = 0;
				}
			}
			if (ch_ceva[0] == 1)
			{
				{ bool recoveredCondition_27548 = team_id_ == 0;
if (recoveredCondition_27548)
				{
					for (int k = 0; k < Map.MAPCHIP_COAT_HNUM; k++)
					{
						float num = GameUtil.RateConvert(k, amefto_address_[0][1] - 4, amefto_address_[0][1] + 1, 0, AMEFTO_PASSER_COATEVA[chargeNum_], true) + 30;
						num = AppData.ClampMax((int)num, 100);
						for (int l = 0; l < Map.MAPCHIP_COAT_WNUM; l++)
						{
							coatEva_new_offence[k][l] = (int)num;
							coatEva_new_offence_passer[k][l] = (int)num;
						}
					}
					if (ChPAreaLineOver())
					{
						for (int m = 0; m < Map.MAPCHIP_COAT_HNUM; m++)
						{
							for (int n = 0; n < Map.MAPCHIP_COAT_WNUM; n++)
							{
								coatEva_new_offence[m][n] = DEAFULT_FORMATION_EVA_PENALTYAREA[m][n];
							}
						}
					}
				}
				else
				{
					for (int num2 = 0; num2 < Map.MAPCHIP_COAT_HNUM; num2++)
					{
						float num3 = GameUtil.RateConvert(num2, amefto_address_[0][1] + 4, amefto_address_[0][1] - 1, 0, AMEFTO_PASSER_COATEVA[chargeNum_], true) + 30;
						num3 = AppData.ClampMax((int)num3, 100);
						for (int num4 = 0; num4 < Map.MAPCHIP_COAT_WNUM; num4++)
						{
							coatEva_new_offence[num2][num4] = (int)num3;
							coatEva_new_offence_passer[num2][num4] = (int)num3;
						}
					}
					if (ChPAreaLineOver())
					{
						for (int num5 = 0; num5 < Map.MAPCHIP_COAT_HNUM; num5++)
						{
							for (int num6 = 0; num6 < Map.MAPCHIP_COAT_WNUM; num6++)
							{
								coatEva_new_offence[num5][num6] = DEAFULT_FORMATION_EVA_PENALTYAREA[Map.MAPCHIP_COAT_HNUM - 1 - num5][Map.MAPCHIP_COAT_WNUM - 1 - num6];
							}
						}
					}
				}
}
			}
			if (ch_ceva[2] == 1)
			{
				Team team = udat_.teams_[1 - team_id_];
				float[] cEVA_PASSER_MP = CEVA_PASSER_MP2;
				for (int num7 = 0; num7 < team.players_.Length; num7++)
				{
					Player player = team.players_[num7];
					Cal_evaCoatMulti9block(coatEva_new_offence, player.address_[0], player.address_[1], cEVA_PASSER_MP);
					Cal_evaCoatMulti9block(coatEva_new_offence_passer, player.address_[0], player.address_[1], cEVA_PASSER_MP);
				}
			}
		}

		public void Update_coatEvaNewDeffence()
		{
			for (int i = 0; i < Map.MAPCHIP_COAT_HNUM; i++)
			{
				for (int j = 0; j < Map.MAPCHIP_COAT_WNUM; j++)
				{
					coatEva_new_deffence[i][j] = 0;
				}
			}
			if (ch_ceva[6] == 1)
			{
				Team team = udat_.teams_[1 - team_id_];
				if (team_id_ == 0)
				{
					if (!team.ChPAreaLineOver())
					{
						for (int k = 0; k < Map.MAPCHIP_COAT_HNUM; k++)
						{
							for (int l = 0; l < Map.MAPCHIP_COAT_WNUM; l++)
							{
								coatEva_new_deffence[k][l] = DEAFULT_FORMATION_EVA[Map.MAPCHIP_COAT_HNUM - 1 - k][Map.MAPCHIP_COAT_WNUM - 1 - l];
							}
						}
					}
					else
					{
						for (int m = 0; m < Map.MAPCHIP_COAT_HNUM; m++)
						{
							for (int n = 0; n < Map.MAPCHIP_COAT_WNUM; n++)
							{
								coatEva_new_deffence[m][n] = DEAFULT_FORMATION_EVA_PENALTYAREA[Map.MAPCHIP_COAT_HNUM - 1 - m][Map.MAPCHIP_COAT_WNUM - 1 - n];
							}
						}
					}
				}
				else if (!team.ChPAreaLineOver())
				{
					for (int num = 0; num < Map.MAPCHIP_COAT_HNUM; num++)
					{
						for (int num2 = 0; num2 < Map.MAPCHIP_COAT_WNUM; num2++)
						{
							coatEva_new_deffence[num][num2] = DEAFULT_FORMATION_EVA[num][num2];
						}
					}
				}
				else
				{
					for (int num3 = 0; num3 < Map.MAPCHIP_COAT_HNUM; num3++)
					{
						for (int num4 = 0; num4 < Map.MAPCHIP_COAT_WNUM; num4++)
						{
							coatEva_new_deffence[num3][num4] = DEAFULT_FORMATION_EVA_PENALTYAREA[num3][num4];
						}
					}
				}
			}
			if (ch_ceva[7] == 1)
			{

int cx, cy;
Player passer;
passer = udat_.teams_[1 - team_id_].GetPasser();


				if (passer != null)
				{
					cx = passer.address_[0];
					cy = passer.address_[1];
				}
				else
				{
					cx = udat_.ball_.address_[0];
					cy = udat_.ball_.address_[1];
				}
				if (udat_.gm_.state_ == 5) { return; }
Cal_evaCoatMulti9block(coatEva_new_deffence, cx, cy, CEVA_PASSER_MP2_DIV);
			}
			if (ch_ceva[8] == 1)
			{
				for (int num5 = 0; num5 < players_.Length; num5++)
				{
					Player player = players_[num5];
					Cal_evaCoatMulti9block(coatEva_new_deffence, player.address_[0], player.address_[1], CEVA_PASSER_MP2);
				}
			}
		}

		public void Update_coatEvaPasser()
		{
			for (int i = 0; i < Map.MAPCHIP_COAT_HNUM; i++)
			{
				for (int j = 0; j < Map.MAPCHIP_COAT_WNUM; j++)
				{
					coatEva_passer[i][j] = 0;
					coatEva_passer2[i][j] = 0;
				}
			}
			if (ch_ceva[0] == 1)
			{
				if (team_id_ == 0)
				{
					for (int k = 0; k < Map.MAPCHIP_COAT_HNUM; k++)
					{
						float num = GameUtil.RateConvert(k, amefto_address_[0][1] - 4, amefto_address_[0][1] + 1, 0, AMEFTO_PASSER_COATEVA[chargeNum_], false) + 30;
						num = AppData.ClampMax((int)num, 100);
						for (int l = 0; l < Map.MAPCHIP_COAT_WNUM; l++)
						{
							coatEva_passer2[k][l] = (int)num;
						}
					}
				}
				else
				{
					for (int m = 0; m < Map.MAPCHIP_COAT_HNUM; m++)
					{
						float num2 = GameUtil.RateConvert(m, amefto_address_[0][1] + 4, amefto_address_[0][1] - 1, 0, AMEFTO_PASSER_COATEVA[chargeNum_], false) + 30;
						num2 = AppData.ClampMax((int)num2, 100);
						for (int n = 0; n < Map.MAPCHIP_COAT_WNUM; n++)
						{
							coatEva_passer2[m][n] = (int)num2;
						}
					}
				}
			}
			if (ch_ceva[1] == 1)
			{
			}
			if (ch_ceva[2] == 1)
			{
				Team team = udat_.teams_[1 - team_id_];
				for (int num3 = 0; num3 < team.players_.Length; num3++)
				{
					Player player = team.players_[num3];
					Cal_evaCoatMulti9block(coatEva_passer2, player.address_[0], player.address_[1], CEVA_PASSER_MP2);
				}
			}
		}

		public void Update_coatEvaFreeRun()
		{
			for (int i = 0; i < Map.MAPCHIP_COAT_HNUM; i++)
			{
				for (int j = 0; j < Map.MAPCHIP_COAT_WNUM; j++)
				{
					coatEva_freeRun[i][j] = 0;
					coatEva_freeRun2[i][j] = 0;
				}
			}
			if (ch_ceva[3] == 1)
			{
				if (team_id_ == 0)
				{
					for (int k = 0; k < Map.MAPCHIP_COAT_HNUM; k++)
					{
						int val = GameUtil.RateConvert(k, amefto_address_[0][1] - 4, amefto_address_[0][1] - 1, 0, AMEFTO_PASSER_COATEVA[chargeNum_], false) + 30;
						val = AppData.ClampMax(val, 100);
						for (int l = 0; l < Map.MAPCHIP_COAT_WNUM; l++)
						{
							coatEva_freeRun2[k][l] = val;
						}
					}
				}
				else
				{
					for (int m = 0; m < Map.MAPCHIP_COAT_HNUM; m++)
					{
						int val2 = GameUtil.RateConvert(m, amefto_address_[0][1] + 4, amefto_address_[0][1] + 1, 0, AMEFTO_PASSER_COATEVA[chargeNum_], false) + 30;
						val2 = AppData.ClampMax(val2, 100);
						for (int n = 0; n < Map.MAPCHIP_COAT_WNUM; n++)
						{
							coatEva_freeRun2[m][n] = val2;
						}
					}
				}
			}
			Player passer = GetPasser();
			if (passer == null)
			{
				return;
			}
			passer.eva_pos_ = Get_playerPosEva(passer, passer.address_[0], passer.address_[1]);
			if (ch_ceva[4] == 1)
			{
				Team team = udat_.teams_[1 - team_id_];
				for (int num = 0; num < team.players_.Length; num++)
				{
					Player player = team.players_[num];
					Cal_evaCoatMulti9block(coatEva_freeRun2, player.address_[0], player.address_[1], CEVA_PASSER_MP2);
				}
			}
			if (ch_ceva[5] == 1)
			{
				int[] array = new int[4]
				{
					0,
					0,
					GameUtil.RateConvert(passer.eva_pos_, 0, 50, 0, 50, true),
					0
				};
			}
		}

		public void Update_coatEvaDefence()
		{
			for (int i = 0; i < Map.MAPCHIP_COAT_HNUM; i++)
			{
				for (int j = 0; j < Map.MAPCHIP_COAT_WNUM; j++)
				{
					coatEva_defence[i][j] = 0;
				}
			}
			if (ch_ceva[6] == 1)
			{
				if (team_id_ == 0)
				{
					for (int k = 0; k < Map.MAPCHIP_COAT_HNUM; k++)
					{
						for (int l = 0; l < Map.MAPCHIP_COAT_WNUM; l++)
						{
							coatEva_defence[k][l] = DEAFULT_FORMATION_EVA[Map.MAPCHIP_COAT_HNUM - 1 - k][Map.MAPCHIP_COAT_WNUM - 1 - l];
						}
					}
				}
				else
				{
					for (int m = 0; m < Map.MAPCHIP_COAT_HNUM; m++)
					{
						for (int n = 0; n < Map.MAPCHIP_COAT_WNUM; n++)
						{
							coatEva_defence[m][n] = DEAFULT_FORMATION_EVA[m][n];
						}
					}
				}
			}
			if (ch_ceva[7] == 1)
			{
				Player passer = udat_.teams_[1 - team_id_].GetPasser();
				if (passer == null)
				{
					return;
				}
				passer.eva_pos_ = Get_playerPosEva(passer, passer.address_[0], passer.address_[1]);
				int[] add_param = new int[6]
				{
					GameUtil.RateConvert(passer.eva_pos_, 0, 50, 0, 50, true),
					GameUtil.RateConvert(passer.eva_pos_, 0, 50, 0, 50, true),
					GameUtil.RateConvert(passer.eva_pos_, 0, 50, 0, 50, true),
					0,
					0,
					0
				};
				Cal_evaCoatAdd(0, coatEva_defence, passer.address_[0], passer.address_[1], add_param);
			}
			if (ch_ceva[8] == 1)
			{
				for (int num = 0; num < players_.Length; num++)
				{
					Player player = players_[num];
					Cal_evaCoatMulti9block(coatEva_defence, player.address_[0], player.address_[1], CEVA_PASSER_MP2);
				}
			}
		}

		public void Update_coatEvaLooseBall()
		{
			Ball ball_ = udat_.ball_;
			int num = 0;
			if (ball_.pos_.z_ > 0f)
			{
				num = 1;
			}
			for (int i = 0; i < Map.MAPCHIP_COAT_HNUM; i++)
			{
				for (int j = 0; j < Map.MAPCHIP_COAT_WNUM; j++)
				{
					coatEva_looseBall[i][j] = 0;
				}
			}
			if (ch_ceva[9] == 1)
			{
				if ((team_id_ == 0 && num == 1) || (team_id_ == 1 && num == 1))
				{
					for (int k = 0; k < Map.MAPCHIP_COAT_HNUM; k++)
					{
						for (int l = 0; l < Map.MAPCHIP_COAT_WNUM; l++)
						{
							coatEva_looseBall[k][l] = DEAFULT_FORMATION_EVA[k][l];
						}
					}
				}
				else
				{
					for (int m = 0; m < Map.MAPCHIP_COAT_HNUM; m++)
					{
						for (int n = 0; n < Map.MAPCHIP_COAT_WNUM; n++)
						{
							coatEva_looseBall[m][n] = DEAFULT_FORMATION_EVA[Map.MAPCHIP_COAT_HNUM - 1 - m][Map.MAPCHIP_COAT_WNUM - 1 - n];
						}
					}
				}
			}
			if (ch_ceva[10] == 1)
			{
				int[] add_param = new int[6] { 50, 50, 50, 0, 0, 0 };
				Cal_evaCoatAdd(0, coatEva_looseBall, ball_.address_[0], ball_.address_[1], add_param);
			}
			if (ch_ceva[11] == 1)
			{
				for (int num2 = 0; num2 < players_.Length; num2++)
				{
					Player player = players_[num2];
					Cal_evaCoatMulti9block(coatEva_looseBall, player.address_[0], player.address_[1], CEVA_PASSER_MP2);
				}
			}
		}

		public Player GetPasser()
		{
			for (int i = 0; i < players_.Length; i++)
			{
				if (BitUtil.Check(players_[i].flag_, 4))
				{
					return players_[i];
				}
			}
			return null;
		}

		public Player GetLastKicker()
		{
			if (lastKickeer_ >= 0 && lastKickeer_ < players_.Length)
			{
				return players_[lastKickeer_];
			}
			return null;
		}

		public int GetSum(int[][] data, int cx, int cy, bool div)
		{
			int num = 0;
			int num3;
int num4;
for (int i = 0; i < CEVA_PASSER_MP2.Length; i++)
			{
				float num2 = 1f;
				if (!div)
				{
					num2 = CEVA_PASSER_MP2[i];
				}
				num3 = cx + PANEL_DIRECT_R_9BLOCK[i][0];
				num4 = cy + PANEL_DIRECT_R_9BLOCK[i][1];
				if (num3 >= 0 && num3 < Map.MAPCHIP_COAT_WNUM && num4 >= 0 && num4 < Map.MAPCHIP_COAT_HNUM)
				{
					num += (int)((float)data[num4][num3] / num2);
				}
			}
			return num;
		}

		public int[] GetMostEvaAddress(int[][] data, int cx, int cy, int num)
		{
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < PANEL_DIRECT_R[i].Length; j++)
				{
					if (cx + PANEL_DIRECT_R[i][j][0] >= 0 && cx + PANEL_DIRECT_R[i][j][0] < Map.MAPCHIP_COAT_WNUM && cy + PANEL_DIRECT_R[i][j][1] >= 0 && cy + PANEL_DIRECT_R[i][j][1] < Map.MAPCHIP_COAT_HNUM)
					{
						int num5 = data[cy + PANEL_DIRECT_R[i][j][1]][cx + PANEL_DIRECT_R[i][j][0]];
						if (num2 < num5)
						{
							num2 = num5;
							num3 = cx + PANEL_DIRECT_R[i][j][0];
							num4 = cy + PANEL_DIRECT_R[i][j][1];
						}
					}
				}
			}
			mostEva_address[0] = num3;
			mostEva_address[1] = num4;
			return mostEva_address;
		}

		public void SetMostEvaAddressFromMoveArea(Player player, int[] var_out, bool isThroughPass)
		{
			int[][] coatEva = player.GetCoatEva();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			try
			{
				int num4 = player.formation_address_[0][0];
				int num5 = player.formation_address_[1][0];
				if (isThroughPass)
				{
					num4 = AppData.Clamp(player.formation_address_[0][0], 1, Map.MAPCHIP_COAT_WNUM - 2);
					num5 = AppData.Clamp(player.formation_address_[1][0], 1, Map.MAPCHIP_COAT_WNUM - 2);
				}
				for (int num6 = player.formation_address_[0][1]; num6 >= player.formation_address_[3][1]; num6--)
				{
					for (int i = num4; i <= num5; i++)
					{
						if (num6 < 0 || num6 >= coatEva.Length || i < 0 || i >= coatEva[0].Length)
						{
							if (Config.DEBUG_CMD)
							{
								Log.Info("■Team.java setMostEvaAddressFromMoveArea() エラー");
							}
							continue;
						}
						int num7 = coatEva[num6][i];
						if (num < num7)
						{
							num = num7;
							num2 = i;
							num3 = num6;
						}
					}
				}
				var_out[0] = num2;
				var_out[1] = num3;
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
		}

		public void SetMostEvaAddressFromMoveArea_thrughPass(Player player, int[] var_out, bool isThroughPass)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = player.formation_address_[0][0];
			int num5 = player.formation_address_[1][0];
			if (isThroughPass)
			{
				num4 = AppData.Clamp(player.formation_address_[0][0], 1, Map.MAPCHIP_COAT_WNUM - 2);
				num5 = AppData.Clamp(player.formation_address_[1][0], 1, Map.MAPCHIP_COAT_WNUM - 2);
			}
			for (int num6 = player.formation_address_[0][1]; num6 >= player.formation_address_[3][1]; num6--)
			{
				for (int i = num4; i <= num5; i++)
				{
					int num7 = Get_playerReceivePosEva(player, i, num6);
					if (num < num7)
					{
						num = num7;
						num2 = i;
						num3 = num6;
					}
				}
			}
			var_out[0] = num2;
			var_out[1] = num3;
		}

		public int Get_playerPosEva(Player player, int xi, int yi)
		{
			return Get_playerPosEva(player, xi, yi, true);
		}

		public int Get_playerPosEva(Player player, int xi, int yi, bool div)
		{
			int[][] coatEva = player.GetCoatEva();
			int sum = GetSum(coatEva, xi, yi, div);
			int num = GameUtil.RateConvert(sum, 0, 700, 0, 100, true);
			Vector3D posFromAddress = Camera.GetPosFromAddress(xi, yi);
			int num2 = 0;
			if (team_id_ == 0) { num2 = ((int)GameUtil.RateConvert(posFromAddress.z_, -900f, 900f, 0f, 100f, true)); } else { num2 = ((int)GameUtil.RateConvert(posFromAddress.z_, 900f, -900f, 0f, 100f, true)); }
			{ int recoveredReturn_42482 = (int)((0.7f * (float)num + 0.3f * (float)num2) / 1f); return recoveredReturn_42482; }
		}

		public int Get_playerReceivePosEva(Player player, int xi, int yi)
		{
			int[][] array = null;
			array = coatEva_new_offence;
			int sum = GetSum(array, xi, yi, true);
			int num = GameUtil.RateConvert(sum, 0, 270, 0, 100, true);
			Camera.ConvertAddressToGlobal(xi, yi, player.temp_vec);
			float x = amefto_goalPos_[0].z_ - player.temp_vec.z_;
			int num2 = 0;
			float num3 = RPP_W_DISTOGOAL_NORMAL;
			if (team_id_ == 0)
			{
				if (!ChPAreaLineOver())
				{
					num2 = (int)GameUtil.RateConvert(x, 270f, 0f, 0f, 100f, true);
				}
				else
				{
					float x2 = (float)JMath.Sqrt(player.pos_.x_ * player.pos_.x_ + (900f - player.pos_.z_) * (900f - player.pos_.z_));
					num2 = (int)GameUtil.RateConvert(x2, 900f, 0f, 0f, 100f, true);
					num3 = RPP_W_DISTOGOAL_PAREA;
				}
			}
			else if (!ChPAreaLineOver())
			{
				num2 = (int)GameUtil.RateConvert(x, -270f, 0f, 0f, 100f, true);
			}
			else
			{
				float x3 = (float)JMath.Sqrt(player.pos_.x_ * player.pos_.x_ + (-900f - player.pos_.z_) * (-900f - player.pos_.z_));
				num2 = (int)GameUtil.RateConvert(x3, 900f, 0f, 0f, 100f, true);
				num3 = RPP_W_DISTOGOAL_PAREA;
			}
			int num4 = 0;
			for (int i = 0; i < udat_.teams_[1 - team_id_].players_.Length; i++)
			{
				Player player2 = udat_.teams_[1 - team_id_].players_[i];
				if (JMath.Abs(player.address_[0] - player2.address_[0]) <= 1 && JMath.Abs(player.address_[1] - player2.address_[1]) <= 1)
				{
					num4++;
				}
			}
			int num5 = GameUtil.RateConvert(num4, 4, 0, 0, 100, true);
			Player passer = GetPasser();
			if (passer == null)
			{
				return (int)((RPP_W_COATEVASUM * (float)num + num3 * (float)num2 + RPP_W_AROUNDENEMY * (float)num5) / (RPP_W_COATEVASUM + num3 + RPP_W_AROUNDENEMY));
			}
			float x4 = (float)JMath.Sqrt((passer.pos_.x_ - player.temp_vec.x_) * (passer.pos_.x_ - player.temp_vec.x_) + (passer.pos_.z_ - player.temp_vec.z_) * (passer.pos_.z_ - player.temp_vec.z_));
			int num6 = (int)GameUtil.RateConvert(x4, 900f, 0f, 0f, 100f, true);
			{ int recoveredReturn = (int)((RPP_W_COATEVASUM * (float)num + num3 * (float)num2 + RPP_W_AROUNDENEMY * (float)num5 + PRP_W_DIS * (float)num6) / (RPP_W_COATEVASUM + num3 + RPP_W_AROUNDENEMY + PRP_W_DIS)); return recoveredReturn; }
		}

		public void Cal_evaCoatMulti(int mode, int[][] data, int cx, int cy, float[] muliti_param)
		{
			int[][][] array = null;
			if ((mode == 0)) { array = PANEL_DIRECT; } else if ((mode == 1)) { array = PANEL_DIRECT_R; }
			for (int i = 0; i < array.Length; i++)
			{
				if (muliti_param[i] == 0f)
				{
					continue;
				}
				for (int j = 0; j < array[i].Length; j++)
				{
					if (cx + array[i][j][0] >= 0 && cx + array[i][j][0] < Map.MAPCHIP_COAT_WNUM && cy + array[i][j][1] >= 0 && cy + array[i][j][1] < Map.MAPCHIP_COAT_HNUM)
					{
						int num = cx + array[i][j][0];
						int num2 = cy + array[i][j][1];
						data[num2][num] = (int)((float)data[num2][num] * muliti_param[i]);
					}
				}
			}
		}

		public void Cal_evaCoatMulti9block(int[][] data, int cx, int cy, float[] muliti_param)
		{
			int num;
int num2;
for (int i = 0; i < PANEL_DIRECT_R_9BLOCK.Length; i++)
			{
				num = cx + PANEL_DIRECT_R_9BLOCK[i][0];
				num2 = cy + PANEL_DIRECT_R_9BLOCK[i][1];
				if (num >= 0 && num < Map.MAPCHIP_COAT_WNUM && num2 >= 0 && num2 < Map.MAPCHIP_COAT_HNUM)
				{
					data[num2][num] = (int)((float)data[num2][num] * muliti_param[i]);
				}
			}
		}

		public void Cal_evaCoatAdd(int mode, int[][] data, int cx, int cy, int[] add_param)
		{
			int[][][] array = null;
			if ((mode == 0)) { array = PANEL_DIRECT; } else if ((mode == 1)) { array = PANEL_DIRECT_R; }
			for (int i = 0; i < array.Length; i++)
			{
				if (add_param[i] == 0)
				{
					continue;
				}
				for (int j = 0; j < array[i].Length; j++)
				{
					if (cx + array[i][j][0] >= 0 && cx + array[i][j][0] < Map.MAPCHIP_COAT_WNUM && cy + array[i][j][1] >= 0 && cy + array[i][j][1] < Map.MAPCHIP_COAT_HNUM)
					{
						int num = cx + array[i][j][0];
						int num2 = cy + array[i][j][1];
						data[num2][num] = data[num2][num] + add_param[i];
					}
				}
			}
		}

		public void Cal_evaCoatAdd9block(int[][] data, int cx, int cy, int[] add_param)
		{
			int num;
int num2;
for (int i = 0; i < PANEL_DIRECT_R_9BLOCK.Length; i++)
			{
				num = cx + PANEL_DIRECT_R_9BLOCK[i][0];
				num2 = cy + PANEL_DIRECT_R_9BLOCK[i][1];
				if (num >= 0 && num < Map.MAPCHIP_COAT_WNUM && num2 >= 0 && num2 < Map.MAPCHIP_COAT_HNUM)
				{
					data[num2][num] = data[num2][num] + add_param[i];
				}
			}
		}

		public void Amefto()
		{
			if (udat_.ball_.lastOwner_ == null)
			{
				return;
			}
			if (udat_.ball_.owner_ == null)
			{
				if (udat_.ball_.lastOwner_.team_id_ != team_id_)
				{
					return;
				}
			}
			else if (udat_.ball_.owner_.team_id_ != team_id_)
			{
				return;
			}
			Player passer = GetPasser();
			if (passer != null)
			{
				if (team_id_ == 0)
				{
					if (passer.pos_.z_ >= goalZ_)
					{
						Amefto_start();
						return;
					}
				}
				else if (passer.pos_.z_ <= goalZ_)
				{
					Amefto_start();
					return;
				}
			}
			chargeCnt_++;
			if (chargeCnt_ >= amefto_oneChargeTime)
			{
				if (udat_.gm_.state_ == 0 && ((team_id_ == 0 && udat_.ball_.pos_.z_ >= Map.COAT_GOAL2_GLOBALPOS[1].z_) || (team_id_ == 1 && udat_.ball_.pos_.z_ <= Map.COAT_GOAL1_GLOBALPOS[1].z_)))
				{
					apdat_.PlaySe(7);
				}
				chargeCnt_ = 0;
				chargeNum_ = AppData.ClampMax(chargeNum_ + 1, 3);
				eva_risk_ = 0;
				if (chargeNum_ == 1)
				{
					eva_risk_ = 25;
				}
				else if (chargeNum_ == 2)
				{
					eva_risk_ = 50;
				}
				else if (chargeNum_ == 3)
				{
					eva_risk_ = 100;
				}
				if (chargeNum_ >= 4)
				{
				}
			}
		}

		public void Amefto_start()
		{
			startZ_ = udat_.ball_.pos_.z_;
			if (team_id_ == 0)
			{
				goalZ_ = startZ_ + (float)amefto_pz;
			}
			else
			{
				goalZ_ = startZ_ - (float)amefto_pz;
			}
			amefto_goalPos_[0].z_ = goalZ_;
			amefto_goalPos_[1].z_ = goalZ_;
			Camera.ConvertGlobalToAddress(amefto_goalPos_[0], amefto_address_[0]);
			Camera.ConvertGlobalToAddress(amefto_goalPos_[1], amefto_address_[1]);
			chargeNum_ = 0;
			chargeCnt_ = 0;
			eva_risk_ = 0;
			if (ChPAreaLineOver())
			{
				Player passer = GetPasser();
				if (passer != null)
				{
					apdat_.PlaySe(7);
				}
			}
		}

		public void Update_formationRect2()
		{
			ternCharge = true;
			if (udat_.ball_.lastOwner_ == null)
			{
				ternCharge = false;
			}
			else if (udat_.ball_.owner_ == null)
			{
				if (udat_.ball_.lastOwner_.team_id_ != team_id_)
				{
					ternCharge = false;
				}
			}
			else if (udat_.ball_.owner_.team_id_ != team_id_)
			{
				ternCharge = false;
			}
			if (udat_.gm_.state_ == 4 || udat_.gm_.state_ == 5)
			{
				if (udat_.gm_.gameState_ == team_id_)
				{
					ternCharge = true;
				}
				else
				{
					ternCharge = false;
				}
			}
			if (ternCharge)
			{
				Player passer = GetPasser();
				float num = 0f;
				float num2 = 0f;
				float num3 = 990f;
				team_formationCenterPos.x_ = udat_.ball_.pos_.x_;
				if (team_id_ == 0)
				{
					team_formationCenterPos.z_ = (float)(50 - AMEFTO_FRECT_GOALZ[chargeNum_]) * num3 / 100f + amefto_goalPos_[0].z_;
					if (udat_.gm_.state_ == 4 || (passer != null && passer.p_id_ == 0))
					{
						team_formationCenterPos.z_ = -180f;
					}
					if (BitUtil.Check(udat_.ball_.flag_, 16))
					{
						team_formationCenterPos.x_ = udat_.ball_.fallPos_.x_;
						team_formationCenterPos.z_ = udat_.ball_.fallPos_.z_;
					}
					num = team_formationCenterPos.z_ + num3 / 2f;
					num2 = team_formationCenterPos.z_ - num3 / 2f;
				}
				else
				{
					team_formationCenterPos.z_ = (float)(-50 + AMEFTO_FRECT_GOALZ[chargeNum_]) * num3 / 100f + amefto_goalPos_[0].z_;
					if (udat_.gm_.state_ == 4 || (passer != null && passer.p_id_ == 0))
					{
						team_formationCenterPos.z_ = 180f;
					}
					if (BitUtil.Check(udat_.ball_.flag_, 16))
					{
						team_formationCenterPos.x_ = udat_.ball_.fallPos_.x_;
						team_formationCenterPos.z_ = udat_.ball_.fallPos_.z_;
					}
					num = team_formationCenterPos.z_ - num3 / 2f;
					num2 = team_formationCenterPos.z_ + num3 / 2f;
				}
				num += (float)(FORMATION_TYPE_RATE[formationType_] - 2 * FORMATION_TYPE_RATE[formationType_] * team_id_);
				num2 += (float)(FORMATION_TYPE_RATE[formationType_] - 2 * FORMATION_TYPE_RATE[formationType_] * team_id_);
				if (num < -900f)
				{
					num = -900f;
				}
				if (num2 < -900f)
				{
					num2 = -900f;
				}
				if (num > 900f)
				{
					num = 900f;
				}
				if (num2 > 900f)
				{
					num2 = 900f;
				}
				fPos_[3] = JMath.Abs(num - num2);
				fPos_[1] = (num + num2) / 2f;
			}
			else
			{
				Player passer2 = udat_.teams_[1 - team_id_].GetPasser();
				int num4 = 0;
				int num5 = 0;
				float num6 = 0.5f;
				float num7 = 0.5f;
				if (passer2 != null)
				{
					num4 = passer2.eva_pos_;
				}
				else
				{
					num6 = 0f;
				}
				int num8 = 0;
				for (int i = 1; i < players_.Length; i++)
				{
					if (passer2 != null && passer2.p_id_ == i)
					{
						continue;
					}
					if (passer2 != null)
					{
						if ((team_id_ == 0 && passer2.pos_.z_ < players_[i].pos_.z_) || (team_id_ == 1 && passer2.pos_.z_ > players_[i].pos_.z_))
						{
							num8++;
						}
					}
					else if ((team_id_ == 0 && udat_.ball_.pos_.z_ < players_[i].pos_.z_) || (team_id_ == 1 && udat_.ball_.pos_.z_ > players_[i].pos_.z_))
					{
						num8++;
					}
				}
				num5 = GameUtil.RateConvert(num8, 0, 5, 0, 100, true);
				team_formationCenterPos.x_ = udat_.ball_.pos_.x_;
				team_formationCenterPos.z_ = udat_.ball_.pos_.z_;
				if (BitUtil.Check(udat_.ball_.flag_, 16))
				{
					team_formationCenterPos.z_ = udat_.ball_.fallPos_.z_;
				}
				team_formationCenterPos.z_ += FORMATION_RATE_DEFFECNE + FORMATION_TYPE_RATE[formationType_] - 2 * (FORMATION_RATE_DEFFECNE + FORMATION_TYPE_RATE[formationType_]) * team_id_;
				if (udat_.gm_.state_ == 4 || (passer2 != null && passer2.p_id_ == 0))
				{
					if (team_id_ == 0)
					{
						team_formationCenterPos.z_ = -180f;
					}
					else
					{
						team_formationCenterPos.z_ = 180f;
					}
				}
				float num9 = 0f;
				float num10 = 0f;
				if (team_id_ == 0)
				{
					if (team_formationCenterPos.z_ < Map.COAT_GOAL1_GLOBALPOS[1].z_)
					{
						num10 = team_formationCenterPos.z_ - 450f;
						if (num10 < -900f)
						{
							num10 = -900f;
						}
						num9 = team_formationCenterPos.z_ + 450f;
						num10 = (team_formationCenterPos.z_ + num10) / 2f;
						num9 = (team_formationCenterPos.z_ + num9) / 2f;
					}
					else
					{
						num10 = team_formationCenterPos.z_ - 450f;
						if (num10 < -900f)
						{
							num10 = -900f;
						}
						num9 = team_formationCenterPos.z_ + 450f;
					}
				}
				else if (team_formationCenterPos.z_ > Map.COAT_GOAL2_GLOBALPOS[1].z_)
				{
					num10 = team_formationCenterPos.z_ + 450f;
					if (num10 > 900f)
					{
						num10 = 900f;
					}
					num9 = team_formationCenterPos.z_ - 450f;
					num10 = (team_formationCenterPos.z_ + num10) / 2f;
					num9 = (team_formationCenterPos.z_ + num9) / 2f;
				}
				else
				{
					num10 = team_formationCenterPos.z_ + 450f;
					if (num10 > 900f)
					{
						num10 = 900f;
					}
					num9 = team_formationCenterPos.z_ - 450f;
				}
				Team team = udat_.teams_[1 - team_id_];
				if (team_id_ == 0)
				{
					float num11 = team.fPos_[1] - team.fPos_[3] / 2f;
					if (num10 > num11)
					{
						num10 = num11;
					}
				}
				else
				{
					float num12 = team.fPos_[1] + team.fPos_[3] / 2f;
					if (num10 < num12)
					{
						num10 = num12;
					}
				}
				fPos_[3] = JMath.Abs(num9 - num10);
				fPos_[1] = (num9 + num10) / 2f;
			}
			int num13 = 30;
			if (team_id_ == 0)
			{
				if (fPos_[1] - fPos_[3] / 2f < -900f)
				{
					fPos_[1] = -900f + fPos_[3] / 2f;
				}
				if (fPos_[1] + fPos_[3] / 2f + (float)num13 > 900f)
				{
					fPos_[1] = 900f - fPos_[3] / 2f - (float)num13;
				}
			}
			else
			{
				if (fPos_[1] - fPos_[3] / 2f - (float)num13 < -900f)
				{
					fPos_[1] = -900f + fPos_[3] / 2f + (float)num13;
				}
				if (fPos_[1] + fPos_[3] / 2f > 900f)
				{
					fPos_[1] = 900f - fPos_[3] / 2f;
				}
			}
			float num14 = 0f;
			num14 = GameUtil.RateConvert(team_formationCenterPos.z_, -900f, 900f, 0f, 100f, true);
			float num15 = 0f;
			float num16 = JMath.Abs(Map.COAT_GOAL1_GLOBALPOS[1].x_ - Map.COAT_GOAL1_GLOBALPOS[0].x_) * 90f / 100f;
			if (ChWhichChargeTern())
			{
				num16 = JMath.Abs(Map.COAT_GOAL1_GLOBALPOS[1].x_ - Map.COAT_GOAL1_GLOBALPOS[0].x_) * 120f / 100f;
			}
			if ((num14 < 50f)) { num15 = GameUtil.RateConvert(num14, 5f, 30f, num16, 900f, true); } else { num15 = GameUtil.RateConvert(num14, 70f, 95f, 900f, num16, true); }
			fPos_[0] = 0f;
			fPos_[2] = num15;
			for (int j = 0; j < players_.Length; j++)
			{
				SetPosFPos(players_[j], players_[j].formationGo_);
			}
			frect_pos[0].z_ = fPos_[1] + fPos_[3] / 2f;
			frect_pos[1].z_ = fPos_[1] + fPos_[3] / 2f;
			frect_pos[2].z_ = fPos_[1] - fPos_[3] / 2f;
			frect_pos[3].z_ = fPos_[1] - fPos_[3] / 2f;
		}

		public void SetPositionKickOff()
		{
			fPos_[0] = 0f;
			fPos_[1] = (int)(-450f + 900f * (float)team_id_);
			fPos_[2] = 900f;
			fPos_[3] = 720f;
			for (int i = 0; i < players_.Length; i++)
			{
				players_[i].Clear();
				SetPosFPos(players_[i], players_[i].pos_);
				if (team_id_ == 0)
				{
					players_[i].direct_ = 0;
				}
				else
				{
					players_[i].direct_ = 2;
				}
			}
			udat_.ball_.SetStop();
			udat_.ball_.pos_.Set(0f, 0f, 0f);
			udat_.ball_.flag_ = BitUtil.Off(udat_.ball_.flag_, 1);
			Camera.ConvertGlobalToScreen(udat_.ball_.pos_, udat_.ball_.screen_pos_);
			Camera.pos.x_ = udat_.ball_.screen_pos_[0];
			Camera.pos.y_ = udat_.ball_.screen_pos_[1];
			if (udat_.gm_.gameState_ == team_id_) {

			if (team_id_ == 0)
			{
				players_[players_.Length - 1].pos_.Set(FORMATION_KICKOFF[0].x_, 0f, FORMATION_KICKOFF[0].z_);
				if (players_[players_.Length - 2].formation_[0] >= 50)
				{
					players_[players_.Length - 2].pos_.Set(FORMATION_KICKOFF[1].x_, 0f, FORMATION_KICKOFF[1].z_);
				}
				else
				{
					players_[players_.Length - 2].pos_.Set(-(FORMATION_KICKOFF[1].x_), 0f, FORMATION_KICKOFF[1].z_);
				}
			}
			else
			{
				players_[players_.Length - 1].pos_.Set(FORMATION_KICKOFF[0].x_, 0f, -(FORMATION_KICKOFF[0].z_));
				if (players_[players_.Length - 2].formation_[0] <= 50)
				{
					players_[players_.Length - 2].pos_.Set(FORMATION_KICKOFF[1].x_, 0f, -(FORMATION_KICKOFF[1].z_));
				}
				else
				{
					players_[players_.Length - 2].pos_.Set(-(FORMATION_KICKOFF[1].x_), 0f, 0f - FORMATION_KICKOFF[1].z_);
				}
			}
			udat_.ball_.ReceiveBall(players_[players_.Length - 1]);
}
		}

		public void SetPositionThrowIn()
		{
			if (udat_.gm_.gameState_ == team_id_) {

			int num;
			int num2;
			if (team_id_ == 0)
			{
				if (((udat_.ball_.lineOutPos_.x_ < 0f))) { num = (0); } else { num = (100); }
				num2 = (int)GameUtil.RateConvert(udat_.ball_.lineOutPos_.z_, -900f, 900f, 0f, 100f, true);
			}
			else
			{
				if (((udat_.ball_.lineOutPos_.x_ > 0f))) { num = (0); } else { num = (100); }
				num2 = (int)GameUtil.RateConvert(udat_.ball_.lineOutPos_.z_, 900f, -900f, 0f, 100f, true);
			}
			int num3 = int.MaxValue;
			int num4 = 1;
			for (int i = 1; i < players_.Length; i++)
			{
				Player player = players_[i];
				int num5 = (num - player.formation_[0]) * (num - player.formation_[0]) + (num2 - player.formation_[1]) * (num2 - player.formation_[1]);
				if (num3 > num5)
				{
					num3 = num5;
					num4 = i;
				}
			}
			players_[num4].flag_ = BitUtil.On(players_[num4].flag_, 256);
			if (udat_.ball_.lineOutPos_.x_ < 0f)
			{
				players_[num4].pos_.Set(-470f, 0f, udat_.ball_.lineOutPos_.z_);
			}
			else
			{
				players_[num4].pos_.Set(470f, 0f, udat_.ball_.lineOutPos_.z_);
			}
			udat_.ball_.flag_ = BitUtil.Off(udat_.ball_.flag_, 1);
			if (udat_.ball_.lineOutPos_.x_ > 0f)
			{
				players_[num4].direct_ = 3;
			}
			else
			{
				players_[num4].direct_ = 1;
			}
			udat_.ball_.ReceiveBall(players_[num4], true);
			udat_.ball_.SetStop();
}
		}

		public void SetPositionGoalKick()
		{
			if (udat_.gm_.gameState_ == team_id_)
			{
				players_[0].flag_ = BitUtil.On(players_[0].flag_, 512);
				if (team_id_ == 0)
				{
					players_[0].pos_.Set(Map.COAT_GOAL1_IN_GLOBALPOS[0]);
				}
				else
				{
					players_[0].pos_.Set(Map.COAT_GOAL2_IN_GLOBALPOS[0]);
				}
				if (team_id_ == 0)
				{
					players_[0].direct_ = 0;
				}
				else
				{
					players_[0].direct_ = 2;
				}
				udat_.ball_.SetStop();
				if (team_id_ == 0)
				{
					udat_.ball_.pos_.Set(Map.COAT_GOAL1_IN_GLOBALPOS[0].x_, 0f, Map.COAT_GOAL1_IN_GLOBALPOS[1].z_);
				}
				else
				{
					udat_.ball_.pos_.Set(Map.COAT_GOAL2_IN_GLOBALPOS[0].x_, 0f, Map.COAT_GOAL2_IN_GLOBALPOS[1].z_);
				}
				udat_.ball_.flag_ = BitUtil.Off(udat_.ball_.flag_, 1);
			}
		}

		public void SetPositionConnerKick()
		{
			if (udat_.gm_.gameState_ == team_id_) {

			int num = 8;
			players_[num].flag_ = BitUtil.On(players_[num].flag_, 1024);
			if (team_id_ == 0)
			{
				if (udat_.ball_.lineOutPos_.x_ < 0f)
				{
					players_[num].pos_.Set(-480f, 0f, 930f);
				}
				else
				{
					players_[num].pos_.Set(480f, 0f, 930f);
				}
			}
			else if (udat_.ball_.lineOutPos_.x_ < 0f)
			{
				players_[num].pos_.Set(-480f, 0f, -930f);
			}
			else
			{
				players_[num].pos_.Set(480f, 0f, -930f);
			}
			if (team_id_ == 0)
			{
				if (udat_.ball_.lineOutPos_.x_ < 0f)
				{
					players_[num].direct_ = 1;
				}
				else
				{
					players_[num].direct_ = 3;
				}
			}
			else if (udat_.ball_.lineOutPos_.x_ < 0f)
			{
				players_[num].direct_ = 1;
			}
			else
			{
				players_[num].direct_ = 3;
			}
			udat_.ball_.SetStop();
			if (team_id_ == 0)
			{
				if (udat_.ball_.lineOutPos_.x_ < 0f)
				{
					udat_.ball_.pos_.Set(-450f, 0f, 900f);
				}
				else
				{
					udat_.ball_.pos_.Set(450f, 0f, 900f);
				}
			}
			else if (udat_.ball_.lineOutPos_.x_ < 0f)
			{
				udat_.ball_.pos_.Set(-450f, 0f, -900f);
			}
			else
			{
				udat_.ball_.pos_.Set(450f, 0f, -900f);
			}
			udat_.ball_.flag_ = BitUtil.Off(udat_.ball_.flag_, 1);
			Amefto_start();
}
		}

		public void SetGoalState()
		{
			if (udat_.gm_.gameState_ == 1 - team_id_)
			{
				if ((!udat_.gm_.halfTime && team_id_ == 0) || (udat_.gm_.halfTime && team_id_ == 1))
				{
					udat_.gm_.scorerListUserTeam.AddElement(new JInteger(players_[lastKickeer_].pData_id_));
					apdat_.pData_[players_[lastKickeer_].pData_id_].SetActScore(5);
				}
				if (!udat_.gm_.halfTime)
				{
					udat_.gm_.score_[team_id_]++;
				}
				else
				{
					udat_.gm_.score_[1 - team_id_]++;
				}
				int num = GameUtil.Random(4) + 1;
				for (int i = 0; i < players_.Length; i++)
				{
					Player player = players_[i];
					if (i == lastKickeer_)
					{
						player.flag_ = BitUtil.On(player.flag_, 2048);
						switch (num)
						{
						case 1:
							player.goalStatePos_.x_ = -450f;
							player.goalStatePos_.z_ = player.pos_.z_;
							break;
						case 2:
							player.goalStatePos_.x_ = 450f;
							player.goalStatePos_.z_ = player.pos_.z_;
							break;
						case 3:
							player.goalStatePos_.x_ = -450f;
							player.goalStatePos_.z_ = 900f - 1800f * (float)team_id_;
							break;
						case 4:
							player.goalStatePos_.x_ = 450f;
							player.goalStatePos_.z_ = 900f - 1800f * (float)team_id_;
							break;
						}
					}
					else
					{
						player.flag_ = BitUtil.On(player.flag_, 4096);
						player.goalStatePos_.x_ = GameUtil.RateConvert(GameUtil.Random(100), 0, 99, GOAL_GLAD_AROUND_POS[team_id_][num][0], GOAL_GLAD_AROUND_POS[team_id_][num][1], true);
						player.goalStatePos_.z_ = GameUtil.RateConvert(GameUtil.Random(100), 0, 99, GOAL_GLAD_AROUND_POS[team_id_][num][2], GOAL_GLAD_AROUND_POS[team_id_][num][3], true);
					}
				}
			}
			else
			{
				for (int j = 0; j < players_.Length; j++)
				{
					Player player2 = players_[j];
					player2.flag_ = BitUtil.On(player2.flag_, 8192);
					player2.AddCommandChangeSeb(12);
				}
			}
		}

		public bool ChWhichChargeTern()
		{
			bool result = true;
			if (udat_.ball_.lastOwner_ == null)
			{
				result = false;
			}
			else if (udat_.ball_.owner_ == null)
			{
				if (udat_.ball_.lastOwner_.team_id_ != team_id_)
				{
					result = false;
				}
			}
			else if (udat_.ball_.owner_.team_id_ != team_id_)
			{
				result = false;
			}
			if (udat_.gm_.state_ == 4)
			{
				if ((udat_.gm_.gameState_ == team_id_)) { result = true; } else { result = false; }
			}
			return result;
		}

		public bool ChPAreaLineOver()
		{
			if (team_id_ == 0)
			{
				if (amefto_goalPos_[0].z_ >= Map.COAT_GOAL2_GLOBALPOS[1].z_)
				{
					return true;
				}
			}
			else if (amefto_goalPos_[0].z_ <= Map.COAT_GOAL1_GLOBALPOS[1].z_)
			{
				return true;
			}
			return false;
		}
	}
}
