using data;
using form;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;

namespace game
{
	public class GameManager
	{
		public const int TIME_CARD_ANIME_NORMAL = 0;

		public const int TIME_CARD_ANIME_DEPRESS = 1;

		public const int TIME_CARD_ANIME_EXCUSE = 2;

		public const int TIME_CARD_ANIME_PANIC = 3;

		public const int TIME_CARD_ANIME_END = 4;

		public const int STATE_MAIN = 0;

		public const int STATE_INITKICKOFF = 1;

		public const int STATE_KICKOFF = 2;

		public const int STATE_THROWIN = 3;

		public const int STATE_GOALKICK = 4;

		public const int STATE_CONNERKICK = 5;

		public const int STATE_TIME = 6;

		public const int STATE_FREEKICK = 7;

		public const int STATE_GOAL = 8;

		public const int STATE_TIMEUP = 9;

		public const int STATE_CARD = 10;

		public const int GAMESTATE_TEAM1_HAVE = 0;

		public const int GAMESTATE_TEAM2_HAVE = 1;

		public const int GAMESTATE_LOOSE_BALL = 2;

		public const int WAIT_KICKOFF = 50;

		public const int WAIT_FK = 50;

		public const int WAIT_TIMEUP = 30;

		public const int FADE_TIME_V = 6;

		public const int COLI_STATE_NOTHING = 0;

		public const int COLI_STATE_T0 = 1;

		public const int COLI_STATE_T1 = 2;

		public const int COLI_STATE_T0_DRIBBLE = 3;

		public const int COLI_STATE_T1_DRIBBLE = 4;

		public const int COLI_STATE_T0_SLIDING = 5;

		public const int COLI_STATE_T1_SLIDING = 6;

		public const int COLI_STATE_T0_RECEIVER = 7;

		public const int COLI_STATE_T1_RECEIVER = 8;

		public const int C_NO = 0;

		public const int C_PO = 1;

		public const int C_PC = 2;

		public const int C_PCB = 3;

		public const int C_SL = 4;

		public const int COLIBALL_STATE_SAVING = 0;

		public const int COLIBALL_STATE_HEADING = 1;

		public const int COLIBALL_STATE_VOLLEY = 2;

		public const int COLIBALL_STATE_TRAP = 3;

		public const int COLIBALL_STATE_NOTHING = 4;

		public const int GAME_HOME = 0;

		public const int GAME_AWAY = 1;

		private static AppData apdat_;

		private static Canvas canvas_;

		public static UserData udat_;

		public int frame_;

		public int flag_;

		public int state_;

		public int stateCnt_;

		public int gameState_;

		public int gameStateCnt_;

		public float gameTime_;

		public static int HALFTIME = 2700;

		public static float ADDTIME = (float)HALFTIME / 1200f;

		public int[] teamId_ = new int[2];

		public int[] score_ = new int[2];

		public bool halfTime;

		public int homeAway_;

		public int[] team_formationPage_ = new int[2];

		public int[] team_potential_ = new int[2];

		public Vector scorerListUserTeam = new Vector();

		public int time_card;

		public int time_team;

		public int time_pId;

		public int time_animePattern;

		private static GameManager instance;

		public static readonly int[] WAIT_THROWIN = new int[2] { 30, 90 };

		public static readonly int[] WAIT_CK = new int[2] { 30, 90 };

		public static readonly int[] WAIT_GK = new int[2] { 30, 90 };

		public static readonly int[] WAIT_GOAL = new int[3] { 30, 60, 180 };

		public static readonly int[] WAIT_CARD = new int[4] { 30, 60, 72, 117 };

		public static readonly int[][] COLI_PROC_DIAGRAM = new int[9][]
		{
			new int[9],
			new int[9] { 0, 1, 1, 1, 2, 0, 0, 1, 1 },
			new int[9] { 0, 1, 1, 2, 1, 0, 0, 1, 1 },
			new int[9] { 0, 1, 0, 0, 0, 0, 0, 0, 0 },
			new int[9] { 0, 0, 1, 0, 0, 0, 0, 0, 0 },
			new int[9] { 0, 0, 0, 0, 4, 0, 0, 0, 0 },
			new int[9] { 0, 0, 0, 4, 0, 0, 0, 0, 0 },
			new int[9] { 0, 1, 1, 1, 2, 0, 0, 1, 3 },
			new int[9] { 0, 1, 1, 2, 1, 0, 0, 3, 1 }
		};

		public static readonly string[] GAME_HOMEAWAY_STR = new string[2] { "ホーム", "アウェイ" };

		private static Vector cBallHitList = new Vector();

		public GameManager()
		{
			apdat_ = AppData.GetInstance();
		}

		public static GameManager GetInstance()
		{
			if (instance == null)
			{
				instance = new GameManager();
			}
			return instance;
		}

		public void StartSoccerForm(bool isUserBattle)
		{
			TeamData[] array = new TeamData[2];
			if (!halfTime)
			{
				array[0] = apdat_.tData_[teamId_[0]];
				array[1] = apdat_.tData_[teamId_[1]];
			}
			else
			{
				array[0] = apdat_.tData_[teamId_[1]];
				array[1] = apdat_.tData_[teamId_[0]];
			}
			if (isUserBattle)
			{
				for (int i = 0; i < udat_.memberPDataist_.Size(); i++)
				{
					PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(i);
					playerData.SetGameParamData(playerData.paramPhysical_);
				}
				if (!halfTime)
				{
					for (int j = 0; j < udat_.cpuPlayerDatas_[1].Length; j++)
					{
						PlayerData playerData2 = udat_.cpuPlayerDatas_[1][j];
						playerData2.SetGameParamData(array[1].warP_paramPhysical_);
						playerData2.faceImg_ = array[1].memberFaceId_[j];
					}
				}
				else
				{
					for (int k = 0; k < udat_.cpuPlayerDatas_[0].Length; k++)
					{
						PlayerData playerData3 = udat_.cpuPlayerDatas_[0][k];
						playerData3.SetGameParamData(array[0].warP_paramPhysical_);
						playerData3.faceImg_ = array[0].memberFaceId_[k];
					}
				}
			}
			else
			{
				for (int l = 0; l < udat_.cpuPlayerDatas_[0].Length; l++)
				{
					PlayerData playerData4 = udat_.cpuPlayerDatas_[0][l];
					playerData4.SetGameParamData(array[0].warP_paramPhysical_);
					playerData4.faceImg_ = array[0].memberFaceId_[l];
				}
				for (int m = 0; m < udat_.cpuPlayerDatas_[1].Length; m++)
				{
					PlayerData playerData5 = udat_.cpuPlayerDatas_[1][m];
					playerData5.SetGameParamData(array[1].warP_paramPhysical_);
					playerData5.faceImg_ = array[1].memberFaceId_[m];
				}
			}
			udat_.teams_ = new Team[2];
			for (int n = 0; n < udat_.teams_.Length; n++)
			{
				udat_.teams_[n] = new Team();
				if (isUserBattle && ((!halfTime && n == 0) || (halfTime && n == 1)))
				{
					udat_.teams_[n].Init(true, n, array[n].id_);
				}
				else
				{
					udat_.teams_[n].Init(false, n, array[n].id_);
				}
			}
			for (int num = 0; num < udat_.teams_.Length; num++)
			{
				for (int num2 = 0; num2 < udat_.teams_[num].players_.Length; num2++)
				{
					Player player = udat_.teams_[num].players_[num2];
					PlayerData playerData6 = apdat_.pData_[player.pData_id_];
					playerData6.SetGameData(player);
				}
			}
			udat_.ball_ = new Ball();
			udat_.ball_.Init();
			if (SoccerForm.DEBUG_BALLTEST_MODE == 1 || SoccerForm.DEBUG_BALLPLAYER == 1)
			{
				ChangeState(0);
				ChangeGameState(2);
			}
			else
			{
				ChangeState(1);
				ChangeGameState(0);
			}
			apdat_.PlaySe(12);
		}

		public void StartSoccerMatch()
		{
			udat_.gm_.score_[0] = 0;
			udat_.gm_.score_[1] = 0;
			gameTime_ = 0f;
			halfTime = false;
			for (int i = 0; i < apdat_.pData_.Length; i++)
			{
				for (int j = 0; j < apdat_.pData_[i].actscoreNum_.Length; j++)
				{
					apdat_.pData_[i].actscoreNum_[j] = 0;
				}
				apdat_.pData_[i].actscoreSum_ = 0;
				apdat_.pData_[i].actscoreScore_ = 0;
			}
			for (int k = 0; k < udat_.memberPDataist_.Size(); k++)
			{
				PlayerData playerData = (PlayerData)udat_.memberPDataist_.ElementAt(k);
				playerData.pri_game_goalTotal_ = playerData.game_goalTotal_;
			}
			SetBallTeam(0);
		}

		public void StartSoccerInitKickOff()
		{
			gameTime_ = 0f;
			SetBallTeam(0);
			StartSoccerKickOff();
			apdat_.frmSoccer_.statusAmeftoCnt_ = SoccerForm.STATUS_AMEFTO_ANIME[1];
		}

		public void StartSoccerKickOff()
		{
			TeamData teamData = apdat_.tData_[teamId_[1]];
			if (!udat_.gm_.halfTime)
			{
				udat_.gm_.team_formationPage_[0] = udat_.selectFormationId_;
				udat_.gm_.team_formationPage_[1] = teamData.formation_;
			}
			else
			{
				udat_.gm_.team_formationPage_[0] = teamData.formation_;
				udat_.gm_.team_formationPage_[1] = udat_.selectFormationId_;
			}
			udat_.teams_[0].SetPositionKickOff();
			udat_.teams_[1].SetPositionKickOff();
		}

		public void Update()
		{
			stateCnt_ = (stateCnt_ + 1) % int.MaxValue;
			gameStateCnt_ = (gameStateCnt_ + 1) % int.MaxValue;
			frame_ = (frame_ + 1) % int.MaxValue;
			if (state_ == 3)
			{
				if (stateCnt_ == WAIT_THROWIN[0])
				{
					SetBallTeam(1 - udat_.ball_.lastOwner_.team_id_);
					udat_.teams_[0].SetPositionThrowIn();
					udat_.teams_[1].SetPositionThrowIn();
				}
			}
			else if (state_ == 4)
			{
				if (stateCnt_ == WAIT_GK[0])
				{
					if (udat_.ball_.owner_ != null)
					{
						udat_.ball_.ReleaseBall(udat_.ball_.owner_);
						if (udat_.ball_.lineOutPos_.z_ > 0f)
						{
							SetBallTeam(1);
						}
						else
						{
							SetBallTeam(0);
						}
					}
					else
					{
						SetBallTeam(1 - udat_.ball_.lastOwner_.team_id_);
					}
					udat_.teams_[0].SetPositionGoalKick();
					udat_.teams_[1].SetPositionGoalKick();
				}
			}
			else if (state_ == 5)
			{
				if (stateCnt_ == WAIT_CK[0])
				{
					SetBallTeam(1 - udat_.ball_.lastOwner_.team_id_);
					udat_.teams_[0].SetPositionConnerKick();
					udat_.teams_[1].SetPositionConnerKick();
				}
			}
			else if (state_ == 8)
			{
				if (Canvas.GetInstance().CheckKeyRepeat(1048576) && stateCnt_ > WAIT_GOAL[1])
				{
					stateCnt_ = WAIT_GOAL[2] - 5;
				}
				if (stateCnt_ == WAIT_GOAL[2] - 5 && ((!udat_.gm_.halfTime && udat_.gm_.gameState_ == 1) || (udat_.gm_.halfTime && udat_.gm_.gameState_ == 0)) && !apdat_.CheckAlreadyEvent(138))
				{
					apdat_.ExecEvent(138, null, null);
				}
				if (stateCnt_ >= WAIT_GOAL[2])
				{
					ChangeState(2);
				}
			}
			else if (state_ == 1)
			{
				if (stateCnt_ == 1 || stateCnt_ == 2)
				{
					apdat_.frmSoccer_.SetAllDraw();
				}
				if (stateCnt_ >= 40 && udat_.gm_.halfTime && !apdat_.CheckAlreadyEvent(153))
				{
					apdat_.ExecEvent(153, null, null);
				}
			}
			else if (state_ == 9 && stateCnt_ >= 30)
			{
				bool flag = halfTime;
				if (!halfTime)
				{
					halfTime = true;
					apdat_.frmSoccer_.ChangeState(6);
					Vector3D global_pos = new Vector3D();
					int[] array = new int[2];
					Camera.ConvertGlobalToScreen(global_pos, array);
					Camera.pos.x_ = array[0];
					Camera.pos.y_ = array[1];
				}
				else
				{
					apdat_.frmSoccer_.ChangeState(2);
				}
				if ((!halfTime && score_[1] == 0) || (halfTime && score_[0] == 0))
				{
					Player player = null;
					player = (halfTime ? udat_.teams_[1].players_[1] : udat_.teams_[0].players_[0]);
					apdat_.pData_[player.pData_id_].SetActScore(16);
				}
				if (!flag)
				{
					ChangeState(1);
				}
			}
			bool flag2 = true;
			if (SoccerForm.DEBUG_BALLTEST_MODE == 1)
			{
				flag2 = false;
			}
			if (state_ != 0 || !flag2)
			{
				return;
			}
			if (SoccerForm.DEBUG_GAMETIMESTOP == 0)
			{
				float num = gameTime_;
				gameTime_ += ADDTIME;
				if ((num - (float)(HALFTIME / 2)) * (gameTime_ - (float)(HALFTIME / 2)) < 0f)
				{
					JSystem.Gc();
				}
			}
			if (Config.DEBUG_CMD && SoccerForm.DEBUG_8KEY_FAST == 1 && Canvas.GetInstance().CheckKeyPulse(256))
			{
				gameTime_ += HALFTIME;
				halfTime = true;
			}
			if (gameTime_ >= (float)HALFTIME)
			{
				gameTime_ = HALFTIME;
				ChangeState(9);
			}
		}

		public void ChangeState(int next_state)
		{
			state_ = next_state;
			stateCnt_ = 0;
			frame_ = 0;
			if (state_ == 1)
			{
				apdat_.frmSoccer_.SetAllDraw();
				StartSoccerInitKickOff();
			}
			else if (state_ == 2)
			{
				apdat_.frmSoccer_.SetAllDraw();
				StartSoccerKickOff();
				apdat_.PlaySe(12);
			}
			else if (state_ == 3)
			{
				apdat_.PlaySe(12);
			}
			else if (state_ == 4)
			{
				apdat_.PlaySe(12);
			}
			else if (state_ == 5)
			{
				apdat_.PlaySe(12);
			}
			else if (state_ == 8)
			{
				SetBallTeam(udat_.ball_.chInGoal_);
				udat_.teams_[0].SetGoalState();
				udat_.teams_[1].SetGoalState();
				if ((!udat_.gm_.halfTime && udat_.gm_.gameState_ == 1) || (udat_.gm_.halfTime && udat_.gm_.gameState_ == 0))
				{
					apdat_.PlaySe(18);
				}
				else
				{
					apdat_.PlaySe(19);
				}
			}
			else if (state_ == 0)
			{
				apdat_.frmSoccer_.SetAllDraw();
				JSystem.Gc();
				apdat_.frmSoccer_.SetAllDraw();
				udat_.teams_[0].Amefto_start();
				udat_.teams_[1].Amefto_start();
			}
			else if (state_ == 9)
			{
				if (!halfTime)
				{
					apdat_.PlaySe(13);
				}
				else
				{
					apdat_.PlaySe(14);
				}
			}
		}

		public void ChangeGameState(int next_state)
		{
			gameState_ = next_state;
			gameStateCnt_ = 0;
			frame_ = 0;
		}

		public void Draw(Graphics g)
		{
			GameView gameView = GameView.GetInstance();
			if (state_ == 3 || state_ == 4 || state_ == 5 || state_ == 2 || state_ == 1)
			{
				int num = 0;
				if (state_ == 3)
				{
					num = WAIT_THROWIN[0];
				}
				else if (state_ == 4)
				{
					num = WAIT_GK[0];
				}
				else if (state_ == 5)
				{
					num = WAIT_CK[0];
				}
				int num2 = stateCnt_ - num;
				int num3 = GameUtil.RateConvert(num2, 0, 6, 255, 0, true);
				if (num3 > 0 && num2 >= 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, num3));
					g.FillRect(0f, 0f, gameView.GetGameWidth(), gameView.GetGameHeight());
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, 255));
				}
				if (num2 <= 6 || num2 > 36)
				{
					return;
				}
				int num4 = 0;
				if (state_ == 5)
				{
					num4 = 1;
				}
				else if (state_ == 4)
				{
					num4 = 2;
				}
				else if (state_ == 3)
				{
					num4 = 3;
				}
				else if (state_ == 2 || state_ == 1)
				{
					num4 = 0;
				}
				if (num4 == 0)
				{
					g.SetColor(Graphics.GetColorOfRGB(78, 137, 255, 194));
					g.FillRect(0f, 73f, gameView.GetGameWidth(), 13f);
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, 255));
					int num5 = 0;
					num5 = ((!Language.English()) ? GameUtil.RateConvert(num2, 6, 14, gameView.GetGameWidth(), gameView.GetGameWidth() / 2 - 42, true) : GameUtil.RateConvert(num2, 6, 14, gameView.GetGameWidth(), gameView.GetGameWidth() / 2 - 49, true));
					apdat_.DrawSeb(g, num5, 68, apdat_.resCom2_.seb[53], apdat_.resCom2_.img[63], 0);
					if (num2 >= 14)
					{
						int num6 = num2 - 14 - 5;
						float num7 = GameUtil.SetVStop(-6000f, 5);
						float num8 = GameUtil.SetAStop(-6000f, 5);
						int num9 = (int)((num7 * (float)num6 + num8 * (float)num6 * (float)(num6 + 1) / 2f) / 1000f);
						if (num9 > 0)
						{
							num9 = 0;
						}
						int num10 = 51;
						if (Language.English())
						{
							num10 += 10;
						}
						apdat_.DrawSeb(g, gameView.GetGameWidth() / 2 - num10 - 7, num9 + 87, apdat_.resCom2_.seb[53], apdat_.resCom2_.img[63], 1);
						apdat_.DrawSeb(g, gameView.GetGameWidth() / 2 + num10, num9 + 87, apdat_.resCom2_.seb[53], apdat_.resCom2_.img[63], 1);
					}
				}
				else
				{
					apdat_.DrawSeb(g, gameView.GetGameWidth() / 2, 80, apdat_.resCom2_.seb[53 + num4], apdat_.resCom2_.img[63 + num4], 0);
				}
			}
			else if (state_ == 8)
			{
				int num11 = stateCnt_ % 8 / 2;
				if (num11 == 3)
				{
					num11 = 1;
				}
				apdat_.DrawSeb(g, gameView.GetGameWidth() / 2, gameView.GetGameHeight() / 2 - 40, apdat_.resCom2_.seb[59], apdat_.resCom2_.img[69], num11);
			}
			else if (state_ == 9)
			{
				if (!halfTime)
				{
					apdat_.DrawSeb(g, gameView.GetGameWidth() / 2, gameView.GetGameHeight() / 2, apdat_.resCom2_.seb[57], apdat_.resCom2_.img[67], 0);
				}
				else
				{
					apdat_.DrawSeb(g, gameView.GetGameWidth() / 2, gameView.GetGameHeight() / 2, apdat_.resCom2_.seb[58], apdat_.resCom2_.img[68], 0);
				}
			}
			else
			{
				if (state_ != 10)
				{
					return;
				}
				g.SetColor(Graphics.GetColorOfRGB(0, 0, 0, 128));
				g.FillRect(0f, SoccerForm.VIEW_Y, 240f, SoccerForm.VIEW_H);
				apdat_.DrawSeb(g, 0, 118, apdat_.resCom2_.seb[33], apdat_.resCom2_.img[35], 0);
				apdat_.DrawSeb(g, 120, 118, apdat_.resCom2_.seb[33], apdat_.resCom2_.img[35], 0);
				apdat_.DrawSeb(g, 0, 91, apdat_.resCom2_.seb[32], apdat_.resCom2_.img[34], 0);
				apdat_.DrawSeb(g, 120, 91, apdat_.resCom2_.seb[32], apdat_.resCom2_.img[34], 0);
				if (stateCnt_ < WAIT_CARD[0])
				{
					int num12 = stateCnt_ % 16 / 4;
					int num13 = 0;
					switch (num12)
					{
					case 0:
					case 2:
						num13 = 0;
						break;
					case 1:
						num13 = 1;
						break;
					default:
						num13 = 2;
						break;
					}
					int x = GameUtil.RateConvert(stateCnt_, 0, WAIT_CARD[0], 68, 101, true);
					apdat_.DrawSeb(g, x, 141, apdat_.resCom2_.seb[34], apdat_.resCom2_.img[36], num13);
				}
				else if (stateCnt_ < WAIT_CARD[1])
				{
					apdat_.DrawSeb(g, 101, 141, apdat_.resCom2_.seb[34], apdat_.resCom2_.img[36], 0);
				}
				else
				{
					apdat_.DrawSeb(g, 101, 141, apdat_.resCom2_.seb[34], apdat_.resCom2_.img[36], 3 + time_card);
				}
				Player player = udat_.teams_[time_team].players_[time_pId];
				PlayerData playerData = apdat_.pData_[player.pData_id_];
				Team team = udat_.teams_[player.team_id_];
				TeamData teamData = apdat_.tData_[team.tDataId_];
				if (stateCnt_ < WAIT_CARD[2])
				{
					udat_.SetDispPlayerData(0, teamData.bodyImg_, playerData.faceImg_, 0, 2);
					udat_.DrawDispPlayer(g, 137, 141);
				}
				else
				{
					int num14 = 0;
					if (time_animePattern == 0 || time_card == 0)
					{
						udat_.SetDispPlayerData(0, teamData.bodyImg_, playerData.faceImg_, 0, 2);
					}
					else if (time_animePattern == 1)
					{
						udat_.SetDispPlayerData(11, teamData.bodyImg_, playerData.faceImg_, 0, 2);
					}
					else if (time_animePattern == 2)
					{
						int num15 = (stateCnt_ - WAIT_CARD[2]) % 12;
						if (num15 < 4)
						{
							udat_.SetDispPlayerData(0, teamData.bodyImg_, playerData.faceImg_, 0, 2);
						}
						else
						{
							udat_.SetDispPlayerData(4, teamData.bodyImg_, playerData.faceImg_, 1, 2);
						}
					}
					else if (time_animePattern == 3)
					{
						if ((stateCnt_ - WAIT_CARD[2]) % 12 / 6 == 0)
						{
							udat_.SetDispPlayerData(4, teamData.bodyImg_, playerData.faceImg_, 0, 1);
						}
						else
						{
							udat_.SetDispPlayerData(4, teamData.bodyImg_, playerData.faceImg_, 0, 2);
							num14 = -1;
						}
					}
					udat_.DrawDispPlayer(g, 137 + num14, 141);
				}
				g.SetColor(Graphics.GetColorOfRGB(255, 255, 255, 128));
				g.FillRect(5f, 169f, 230f, 61f);
				for (int i = 0; i < 3; i++)
				{
					Draw_timeMessageWindow(g, 172 + 18 * i, 222, i % 2, 120);
				}
			}
		}

		public void Draw_timeMessageWindow(Graphics g, int oy, int W, int index, int centerX)
		{
			int num = centerX - W / 2;
			int num2 = 224;
			g.SetRenderMode(1, num2, 255 - num2);
			g.DrawImage(apdat_.resCom2_.img[37], num, oy, 0, index * 18, 5, 18);
			g.DrawImage(apdat_.resCom2_.img[37], num + W - 5, oy, 115, index * 18, 5, 18);
			if (W - 10 <= 110)
			{
				g.DrawImage(apdat_.resCom2_.img[37], num + 5, oy, 5, index * 18, W - 10, 18);
			}
			else
			{
				int num3 = W - 10;
				int num4 = 0;
				while (num3 > 0)
				{
					int num5 = AppData.ClampMax(num3, 110);
					g.DrawImage(apdat_.resCom2_.img[37], num + 5 + num4, oy, 5, index * 18, num5, 18);
					num3 -= num5;
					num4 += num5;
				}
			}
			g.SetRenderMode(0, 255, 0);
		}

		public void SetBallTeam(int team_id)
		{
			switch (team_id)
			{
			case -1:
				ChangeGameState(2);
				break;
			case 0:
				ChangeGameState(0);
				break;
			default:
				ChangeGameState(1);
				break;
			}
		}

		public void SetBattleTeamId(int tId1, int tId2, int homeAway)
		{
			teamId_[0] = tId1;
			teamId_[1] = tId2;
			homeAway_ = homeAway;
		}

		public void Collision()
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < udat_.teams_[i].players_.Length; j++)
				{
					Collision_updateCollisionState(udat_.teams_[i].players_[j]);
				}
			}
			int num = GameUtil.Random(2);
			for (int k = 0; k < 2; k++)
			{
				int num2 = 0;
				num2 = ((k != 0) ? (1 - num) : num);
				for (int l = 0; l < udat_.teams_[num2].players_.Length; l++)
				{
					Player p = udat_.teams_[num2].players_[l];
					for (int m = 0; m < udat_.teams_[0].players_.Length; m++)
					{
						if (num2 != 0 || l != m)
						{
							Player p2 = udat_.teams_[0].players_[m];
							Collision_proc(p, p2);
						}
					}
					for (int n = 0; n < udat_.teams_[1].players_.Length; n++)
					{
						if (num2 != 1 || l != n)
						{
							Player p3 = udat_.teams_[1].players_[n];
							Collision_proc(p, p3);
						}
					}
				}
			}
			if (state_ != 0 || gameState_ != 2 || !BitUtil.Check(udat_.ball_.flag_, 8))
			{
				return;
			}
			for (int num3 = 0; num3 < 2; num3++)
			{
				for (int num4 = 0; num4 < udat_.teams_[num3].players_.Length; num4++)
				{
					Collision_updateCollisionBallState(udat_.teams_[num3].players_[num4]);
				}
			}
			CollisionBall_proc();
		}

		public void Collision_updateCollisionState(Player player)
		{
			player.coliState_ = 1 + player.team_id_;
			if (BitUtil.Check(player.flag_, 16384))
			{
				player.coliState_ = 0;
			}
			else if (BitUtil.Check(player.flag_, 4))
			{
				player.coliState_ = 3 + player.team_id_;
			}
			else if (BitUtil.Check(player.flag_, 16))
			{
				if (BitUtil.Check(player.flag_, 32))
				{
					player.coliState_ = 5 + player.team_id_;
				}
				else
				{
					player.coliState_ = 0;
				}
			}
			else if (player.actId_ == 1)
			{
				player.coliState_ = 7 + player.team_id_;
			}
			else if (player.p_id_ == 0)
			{
				player.coliState_ = 0;
			}
			else if (BitUtil.Check(player.flag_, 14400))
			{
				player.coliState_ = 0;
			}
		}

		public void Collision_updateCollisionBallState(Player player)
		{
			player.coliBallState_ = 3;
			if (BitUtil.Check(player.flag_, 8))
			{
				player.coliBallState_ = 0;
			}
			else if (BitUtil.Check(player.flag_, 16384))
			{
				player.coliBallState_ = 4;
			}
			else if (BitUtil.Check(player.flag_, 64))
			{
				if (BitUtil.Check(player.flag_, 128))
				{
					player.coliBallState_ = 1;
				}
				else
				{
					player.coliBallState_ = 4;
				}
			}
			else if (udat_.ball_.lastOwner_ != null && udat_.ball_.lastOwner_.p_id_ == player.p_id_ && udat_.ball_.lastOwner_.team_id_ == player.team_id_)
			{
				player.coliBallState_ = 4;
			}
		}

		public void Collision_proc(Player p1, Player p2)
		{
			int num = COLI_PROC_DIAGRAM[p1.coliState_][p2.coliState_];
			if (num == 0 || p1.address_[0] - p2.address_[0] < -1 || p1.address_[0] - p2.address_[0] > 1 || p1.address_[1] - p2.address_[1] < -1 || p1.address_[1] - p2.address_[1] > 1)
			{
				return;
			}
			int[] collisionData = udat_.GetCollisionData(0, 0);
			int[] collisionData2 = udat_.GetCollisionData(1, 0);
			if (!GameUtil.ChRectHitRect(p1.pos_.x_ + (float)collisionData[0], p1.pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], p2.pos_.x_ + (float)collisionData2[0], p2.pos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true))
			{
				return;
			}
			if (num != 4)
			{
				float num2 = collisionData2[2] / 2;
				float num3 = collisionData2[3] / 2;
				float num4 = JMath.Abs(p2.pos_.x_ - p1.pos_.x_);
				float num5 = JMath.Abs(p2.pos_.z_ - p1.pos_.z_);
				if (num4 < 0.001f || num5 < 0.001f)
				{
					p1.pos_.Set(p1.old_pos);
				}
				else
				{
					int num6 = 0;
					num6 = ((!(num5 > num4)) ? 1 : 0);
					Player.chColPos.Set(p1.pos_);
					if (num6 == 0)
					{
						if (p1.pos_.x_ < p2.pos_.x_)
						{
							p1.pos_.x_ = p2.pos_.x_ - 2f * num3 * num4 / num5;
						}
						else
						{
							p1.pos_.x_ = p2.pos_.x_ + 2f * num3 * num4 / num5;
						}
						if (p1.pos_.z_ < p2.pos_.z_)
						{
							p1.pos_.z_ = p2.pos_.z_ - 2f * num3;
						}
						else
						{
							p1.pos_.z_ = p2.pos_.z_ + 2f * num3;
						}
					}
					else
					{
						if (p1.pos_.z_ < p2.pos_.z_)
						{
							p1.pos_.z_ = p2.pos_.z_ - 2f * num2 * num5 / num4;
						}
						else
						{
							p1.pos_.z_ = p2.pos_.z_ + 2f * num2 * num5 / num4;
						}
						if (p1.pos_.x_ < p2.pos_.x_)
						{
							p1.pos_.x_ = p2.pos_.x_ - 2f * num2;
						}
						else
						{
							p1.pos_.x_ = p2.pos_.x_ + 2f * num2;
						}
					}
				}
			}
			if (state_ != 0 || (num != 2 && num != 3 && num != 4))
			{
				return;
			}
			if (Player.ChBodyHitBody(p1, p2))
			{
				if (num == 4)
				{
					apdat_.pData_[p1.pData_id_].SetActScore(12);
					apdat_.pData_[p2.pData_id_].SetActScore(9);
				}
				else
				{
					apdat_.pData_[p1.pData_id_].SetActScore(11);
					apdat_.pData_[p2.pData_id_].SetActScore(8);
				}
				if (p2.physical_hp_ <= 0)
				{
					if (BitUtil.Check(p2.flag_, 4))
					{
						udat_.ball_.ReleaseBall(p2);
						udat_.ball_.ReceiveBall(p1);
						apdat_.pData_[p1.pData_id_].SetActScore(13);
						apdat_.pData_[p2.pData_id_].SetActScore(10);
					}
					p2.Proc_down();
					p2.physical_hp_ = PlayerData.GetPhysicalHpMax(p2);
					PlayerData playerData = apdat_.pData_[p2.pData_id_];
					playerData.AddAuraPoint(500, true);
					Collision_updateCollisionState(p2);
				}
				if (num == 4)
				{
					Player.SetEfCharge(p1, p2, true);
				}
				else
				{
					Player.SetEfCharge(p1, p2, false);
				}
			}
			if (num == 3 && !BitUtil.Check(p2.flag_, 16384) && Player.ChBodyHitBody(p2, p1))
			{
				if (num == 4)
				{
					apdat_.pData_[p2.pData_id_].SetActScore(12);
					apdat_.pData_[p1.pData_id_].SetActScore(9);
				}
				else
				{
					apdat_.pData_[p2.pData_id_].SetActScore(11);
					apdat_.pData_[p1.pData_id_].SetActScore(8);
				}
				if (p1.physical_hp_ <= 0)
				{
					p1.Proc_down();
					p1.physical_hp_ = PlayerData.GetPhysicalHpMax(p1);
					PlayerData playerData2 = apdat_.pData_[p1.pData_id_];
					playerData2.AddAuraPoint(500, true);
				}
				if (num == 4)
				{
					Player.SetEfCharge(p2, p1, true);
				}
				else
				{
					Player.SetEfCharge(p2, p1, false);
				}
			}
		}

		public void CollisionBall_proc()
		{
			Ball ball_ = udat_.ball_;
			if (cBallHitList.Size() > 0)
			{
				cBallHitList.RemoveAllElements();
			}
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < udat_.teams_[i].players_.Length; j++)
				{
					Player player = udat_.teams_[i].players_[j];
					if (player.coliBallState_ != 4 && player.address_[0] - ball_.address_[0] >= -1 && player.address_[0] - ball_.address_[0] <= 1 && player.address_[1] - ball_.address_[1] >= -1 && player.address_[1] - ball_.address_[1] <= 1)
					{
						int[] collisionData = udat_.GetCollisionData(0, 1);
						int[] array = null;
						float num = 0f;
						if (player.coliBallState_ == 0)
						{
							array = udat_.GetCollisionData(0, 0);
							num = PlayerData.GetHeadJampDisTop(player);
						}
						else if (player.coliBallState_ == 3)
						{
							array = udat_.GetCollisionData(0, 0);
							num = 20f;
						}
						else if (player.coliBallState_ == 1)
						{
							array = udat_.GetCollisionData(0, 0);
							num = PlayerData.GetHeadJampDisTop(player);
						}
						else
						{
							array = udat_.GetCollisionData(0, 0);
							num = 20f;
						}
						if (!(ball_.pos_.y_ >= num) && GameUtil.ChRectHitRect(ball_.pos_.x_ + (float)collisionData[0], ball_.pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], player.pos_.x_ + (float)array[0], player.pos_.z_ + (float)array[1], array[2], array[3], true))
						{
							cBallHitList.AddElement(player);
						}
					}
				}
			}
			if (cBallHitList.Size() == 0)
			{
				return;
			}
			float num2 = 2.1474836E+09f;
			int index = 0;
			for (int k = 0; k < cBallHitList.Size(); k++)
			{
				Player player2 = (Player)cBallHitList.ElementAt(k);
				player2.temp_len = (float)JMath.Sqrt((ball_.pos_.x_ - player2.pos_.x_) * (ball_.pos_.x_ - player2.pos_.x_) + (ball_.pos_.z_ - player2.pos_.z_) * (ball_.pos_.z_ - player2.pos_.z_)) + (float)(player2.coliBallState_ * 10000);
				if (player2.temp_len < num2)
				{
					num2 = player2.temp_len;
					index = k;
				}
			}
			Player player3 = (Player)cBallHitList.ElementAt(index);
			bool flag = false;
			if (BitUtil.Check(ball_.flag_, 32))
			{
				flag = true;
			}
			if (player3.coliBallState_ == 0)
			{
				bool front = false;
				if (player3.team_id_ == 0)
				{
					if (player3.pos_.z_ < udat_.ball_.pos_.z_)
					{
						front = true;
					}
				}
				else if (player3.pos_.z_ > udat_.ball_.pos_.z_)
				{
					front = true;
				}
				int num3 = PlayerData.GetCatchProc(player3, udat_.ball_.v_len_, front);
				apdat_.pData_[player3.pData_id_].SetActScore(15);
				apdat_.AddEffectShoot(player3.screen_pos_[0] + Player.EF_KEEPER_OFF[player3.team_id_][player3.direct_][0], player3.screen_pos_[1] + Player.EF_KEEPER_OFF[player3.team_id_][player3.direct_][1]);
				if (!flag)
				{
					num3 = 2;
				}
				ball_.flag_ = BitUtil.Off(ball_.flag_, 96);
				ball_.receive_wait = Ball.RECEIVE_WAIT_MIN * 3;
				ball_.flag_ = BitUtil.Off(ball_.flag_, 8);
				switch (num3)
				{
				case 2:
					udat_.ball_.ReceiveBall(player3);
					break;
				case 0:
				case 1:
				{
					udat_.ball_.ReceiveBall(player3);
					udat_.ball_.ReleaseBall(player3);
					int num4 = 30;
					int n = 120;
					float num5 = (float)JMath.Sqrt(udat_.ball_.v_.x_ * udat_.ball_.v_.x_ + udat_.ball_.v_.z_ * udat_.ball_.v_.z_);
					float num6 = 0f;
					switch (num3)
					{
					case 1:
						num6 = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 0.5f, 0.35f, true);
						break;
					case 0:
						num6 = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 0.5f, 0.35f, true);
						break;
					}
					num5 *= num6;
					int angle = 0;
					if (player3.team_id_ == 0)
					{
						switch (num3)
						{
						case 1:
							angle = num4 + GameUtil.Random(n);
							break;
						case 0:
							angle = ((player3.direct_ != 0) ? ((player3.direct_ != 1) ? (190 + GameUtil.Random(70)) : (280 + GameUtil.Random(70))) : (190 + GameUtil.Random(160)));
							break;
						}
					}
					else
					{
						switch (num3)
						{
						case 1:
							angle = 180 + num4 + GameUtil.Random(n);
							break;
						case 0:
							angle = ((player3.direct_ != 2) ? ((player3.direct_ != 1) ? (100 + GameUtil.Random(70)) : (10 + GameUtil.Random(70))) : (10 + GameUtil.Random(160)));
							break;
						}
					}
					udat_.ball_.v_.Set(num5 * GameUtil.Cos(angle), udat_.ball_.v_.y_ * 0.5f, num5 * GameUtil.Sin(angle));
					udat_.ball_.a_.y_ = -0.6f;
					break;
				}
				}
				apdat_.PlaySe(Ball.SOUND_HEAD);
			}
			else if (player3.coliBallState_ == 3)
			{
				if (flag && ball_.lastOwner_ != null && ball_.lastOwner_.team_id_ != player3.team_id_)
				{
					HitShootPlayer(ball_, player3);
				}
				else
				{
					udat_.ball_.ReceiveBall(player3);
				}
			}
			else if (player3.coliBallState_ == 1)
			{
				udat_.ball_.ReceiveBall(player3);
				udat_.ball_.ReleaseBall(player3);
				udat_.ball_.SetHeadingData(player3);
				player3.flag_ = BitUtil.Off(flag_, 128);
				apdat_.pData_[player3.pData_id_].SetActScore(14);
			}
			else
			{
				udat_.ball_.ReceiveBall(player3);
			}
		}

		public void HitShootPlayer(Ball ball, Player hiter)
		{
			int num = (int)GameUtil.RateConvert(ball.v_len_, PlayerData.SHOOTMIN_MAX * 80f / 100f, PlayerData.SHOOTMAX_MAX * 80f / 100f, 0f, 1000f, true);
			int num2 = hiter.shootTrap_data + GameUtil.Random(100) - 50;
			if (Config.DEBUG_CMD)
			{
				Log.Info("■GameManager.java hitShootPlayer() 修正前 ball.v_ = " + ball.v_.x_ + ", " + ball.v_.y_ + ", " + ball.v_.z_);
			}
			if (num2 >= num)
			{
				ball.ReceiveBall(hiter);
			}
			else if (num2 >= num / 2)
			{
				udat_.ball_.ReceiveBall(hiter);
				udat_.ball_.ReleaseBall(hiter);
				int num3 = GameUtil.Random(200) - 100;
				int num4 = GameUtil.Random(180) - 90;
				int num5 = GameUtil.Random(40) + 10;
				ball.v_.y_ = ball.v_.y_ * (float)num3 / 100f;
				float num6 = 0f - ball.v_.x_;
				float num7 = 0f - ball.v_.z_;
				ball.v_.x_ = (float)((double)num6 * JMath.Cos(num4) - (double)num7 * JMath.Sin(num4));
				ball.v_.z_ = (float)((double)num6 * JMath.Sin(num4) + (double)num7 * JMath.Cos(num4));
				ball.v_.x_ = ball.v_.x_ * (float)num5 / 100f;
				ball.v_.z_ = ball.v_.z_ * (float)num5 / 100f;
			}
			else
			{
				udat_.ball_.ReceiveBall(hiter);
				udat_.ball_.ReleaseBall(hiter);
				int num8 = GameUtil.Random(100) - 50;
				int num9 = GameUtil.Random(60) - 30;
				int num10 = GameUtil.Random(30) + 60;
				ball.v_.y_ = ball.v_.y_ * (float)num8 / 100f;
				float x_ = ball.v_.x_;
				float z_ = ball.v_.z_;
				ball.v_.x_ = (float)((double)x_ * JMath.Cos(num9) - (double)z_ * JMath.Sin(num9));
				ball.v_.z_ = (float)((double)x_ * JMath.Sin(num9) + (double)z_ * JMath.Cos(num9));
				ball.v_.x_ = ball.v_.x_ * (float)num10 / 100f;
				ball.v_.z_ = ball.v_.z_ * (float)num10 / 100f;
			}
			if (Config.DEBUG_CMD)
			{
				Log.Info("■GameManager.java hitShootPlayer() 修正後 ball.v_ = " + ball.v_.x_ + ", " + ball.v_.y_ + ", " + ball.v_.z_);
			}
		}
	}
}
