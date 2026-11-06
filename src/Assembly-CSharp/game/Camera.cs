using form;
using java.io;
using java.lang;
using java.util;
using kairo.unity.form;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;

namespace game
{
	public class Camera
	{
		public const int CAMERA_MODE_FREE = 0;

		public const int CAMERA_MODE_BALL = 1;

		public const int CAMERA_MODE_AUTO = 2;

		public const float MAP_GLOBAL_W = 900f;

		public const float MAP_GLOBAL_H = 1800f;

		public const float MAP_GLOBAL_LEFT = -450f;

		public const float MAP_GLOBAL_RIGHT = 450f;

		public const float MAP_GLOBAL_TOP = 900f;

		public const float MAP_GLOBAL_BOTTOM = -900f;

		public const int CURSOR_MAX = 80;

		public const int SUBCURSOR_MAX = 100;

		public AppData apdat_;

		public Canvas canvas_;

		public UserData udat_;

		private static Camera instance;

		public static bool CAMERA_LIMIT = true;

		public int mode_;

		public static Vector3D pos;

		public static Vector3D old_pos;

		public static Vector3D save_pos;

		public static Vector3D save_cursorPos;

		public static Vector3D cursor_pos;

		public static int[] camera_spos;

		public static Vector3D camera_gpos;

		public static int[] camera_address;

		public static Vector3D camera_goalGPos;

		public Vector3D v;

		public Vector3D a;

		public int[][] cameraLimitSPos;

		public static readonly Vector3D[] CAMERALIMIT_GPOS = new Vector3D[4]
		{
			new Vector3D(-405f, 0f, 810f),
			new Vector3D(405f, 0f, 810f),
			new Vector3D(405f, 0f, -810f),
			new Vector3D(-405f, 0f, -810f)
		};

		public static int MAP_SCREEN_W1 = 480;

		public static int MAP_SCREEN_W2 = 240;

		public static int MAP_SCREEN_W = MAP_SCREEN_W1 + MAP_SCREEN_W2;

		public static int MAP_SCREEN_H1 = 240;

		public static int MAP_SCREEN_H2 = 120;

		public static int MAP_SCREEN_H = MAP_SCREEN_H1 + MAP_SCREEN_H2;

		public static int MAP_SCREEN_LEFT = -MAP_SCREEN_W / 2;

		public static int MAP_SCREEN_RIGHT = MAP_SCREEN_LEFT + MAP_SCREEN_W;

		public static int MAP_SCREEN_TOP = MAP_SCREEN_H / 2;

		public static int MAP_SCREEN_BOTTOM = MAP_SCREEN_TOP - MAP_SCREEN_H;

		public static int[] MAP_SCREEN_BASE_POS = new int[2];

		public int cursorCnt_;

		public int subCursorCnt_;

		public TPlayer subPlayer_;

		public TPlayer subSvisor_;

		public Tenant subTenant_;

		public TPlayer old_subPlayer_;

		public TPlayer old_subSvisor_;

		public Tenant old_subTenant_;

		private int[] CURSOR_LIMIT_OFF = new int[4] { 0, 5, 0, -5 };

		public bool flg_disp = true;

		public int cnt_autoFocus;

		public int TIME_AUTOFOCUS = 12;

		public static int DRAW_CENTER_X = 0;

		public static int DRAW_CENTER_Y = 0;

		public static int DRAW_VIEW_X = 0;

		public static int DRAW_VIEW_Y = 0;

		public static int DRAW_VIEW_W = 0;

		public static int DRAW_VIEW_H = 0;

		public Vector3D limit_pos_lt = new Vector3D();

		public Vector3D limit_pos_rb = new Vector3D();

		public Vector3D limit_pos_base = new Vector3D();

		public static int CAMERA_R_W = 6;

		public static int FLG_DISPHITRECT = 0;

		public static float CONVERT_SG_A;

		public static float CONVERT_SG_B;

		public static float CONVERT_SG_C;

		public static float CONVERT_SG_D;

		public static Vector3D ch_global_pos = new Vector3D();

		public static int[] ch_screen_pos = new int[2];

		public static float UPX_FROMX;

		public static float UPX_FROMZ;

		public static float UPZ_FROMX;

		public static float UPZ_FROMZ;

		public static float OFF_SX;

		public static float OFF_SY;

		public static Vector3D CAMERA_BASE_GPOS = new Vector3D();

		public static Vector3D CAMERA_ADS_GPOS = new Vector3D();

		public Vector3D[] chPos;

		public Vector3D camera_pos1 = new Vector3D();

		private static int[] temp_c = new int[2];

		private static int[] temp_s = new int[2];

		private static int[] temp_screen_pos = new int[2];

		public static Vector3D COATADDRESS_POS = new Vector3D();

		private int[] temp_cpos = new int[2];

		private static int[] temp_cursorSPos = new int[2];

		internal int[][] CAMERA_RECT = new int[2][]
		{
			new int[2] { 300, 200 },
			new int[2] { 1200, -350 }
		};

		public int cursorPressCnt_;

		private static int[] temp_spos = new int[2];

		private static Vector3D temp_gpos = new Vector3D();

		private static int[] temp_address = new int[2];

		private Camera()
		{
			apdat_ = AppData.GetInstance();
			canvas_ = Canvas.GetInstance();
			udat_ = UserData.GetInstance();
			pos = new Vector3D();
			old_pos = new Vector3D();
			save_pos = new Vector3D();
			save_cursorPos = new Vector3D();
			cursor_pos = new Vector3D();
			v = new Vector3D();
			a = new Vector3D();
			camera_spos = new int[2];
			camera_gpos = new Vector3D();
			camera_address = new int[2];
			camera_goalGPos = new Vector3D();
			cameraLimitSPos = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });
			for (int i = 0; i < cameraLimitSPos.Length; i++)
			{
				ConvertGlobalToScreen(CAMERALIMIT_GPOS[i], cameraLimitSPos[i]);
			}
			SetScreenData();
			pos.Set(0f, 0f, 0f);
			SetCameraLimitData();
			mode_ = 0;
			int[] screen_pos = new int[2];
			ConvertScreenToGlobal(screen_pos, CAMERA_BASE_GPOS);
			chPos = new Vector3D[4];
			for (int j = 0; j < chPos.Length; j++)
			{
				chPos[j] = new Vector3D();
			}
		}

		public static Camera GetInstance()
		{
			if (instance == null)
			{
				instance = new Camera();
			}
			return instance;
		}

		public void SetMapMainBasePos()
		{
			pos.x_ = save_pos.x_;
			pos.y_ = save_pos.y_;
			cursor_pos.x_ = save_cursorPos.x_;
			cursor_pos.y_ = save_cursorPos.y_;
			old_pos.Set(pos);
		}

		public void SaveMapMainBasePos()
		{
			save_pos.x_ = pos.x_;
			save_pos.y_ = pos.y_;
			save_cursorPos.x_ = cursor_pos.x_;
			save_cursorPos.y_ = cursor_pos.y_;
		}

		public static void SetScreenData()
		{
			MAP_SCREEN_W1 = 30 * Map.MAPCHIP_COAT_HNUM;
			MAP_SCREEN_W2 = 30 * Map.MAPCHIP_COAT_WNUM;
			MAP_SCREEN_W = MAP_SCREEN_W1 + MAP_SCREEN_W2;
			MAP_SCREEN_H1 = 15 * Map.MAPCHIP_COAT_HNUM;
			MAP_SCREEN_H2 = 15 * Map.MAPCHIP_COAT_WNUM;
			MAP_SCREEN_H = MAP_SCREEN_H1 + MAP_SCREEN_H2;
			MAP_SCREEN_LEFT = -MAP_SCREEN_W / 2;
			MAP_SCREEN_RIGHT = MAP_SCREEN_LEFT + MAP_SCREEN_W;
			MAP_SCREEN_TOP = MAP_SCREEN_H / 2;
			MAP_SCREEN_BOTTOM = MAP_SCREEN_TOP - MAP_SCREEN_H;
			int num = Map.MAPCHIP_COAT_HNUM * 80 / 100;
			if (num > Map.MAPCHIP_COAT_HNUM - 1)
			{
				num = Map.MAPCHIP_COAT_HNUM - 1;
			}
			ConvertGlobalToScreen(new Vector3D(), MAP_SCREEN_BASE_POS);
		}

		public void Update_pos()
		{
			bool flag = canvas_.CheckKeyState(131072);
			bool flag2 = canvas_.CheckKeyState(524288);
			bool flag3 = canvas_.CheckKeyState(65536);
			bool flag4 = canvas_.CheckKeyState(262144);
			if (SoccerForm.DEBUG_PLAYER_NOTACT == 1)
			{
				Player player = udat_.GetPlayer(0, 0);
				player.v_.Set(0f, 0f, 0f);
				if (flag)
				{
					player.v_.z_ = 35f;
					player.direct_ = 0;
				}
				if (flag2)
				{
					player.v_.z_ = -35f;
					player.direct_ = 2;
				}
				if (flag3)
				{
					player.v_.x_ = -35f;
					player.direct_ = 3;
				}
				if (flag4)
				{
					player.v_.x_ = 35f;
					player.direct_ = 1;
				}
				player.pos_.x_ += player.v_.x_;
				player.pos_.z_ += player.v_.z_;
				ConvertGlobalToScreen(player.pos_, player.screen_pos_);
				pos.x_ = player.screen_pos_[0];
				pos.y_ = player.screen_pos_[1];
			}
			else if (udat_.gm_.state_ == 3 && (udat_.gm_.stateCnt_ >= GameManager.WAIT_THROWIN[0] || udat_.gm_.stateCnt_ >= GameManager.WAIT_GK[0] || udat_.gm_.stateCnt_ >= GameManager.WAIT_CK[0]))
			{
				camera_gpos.x_ = (int)udat_.ball_.shadowPos_.x_;
				camera_gpos.z_ = (int)udat_.ball_.shadowPos_.z_;
				if (camera_gpos.x_ > CAMERALIMIT_GPOS[1].x_)
				{
					camera_gpos.x_ = CAMERALIMIT_GPOS[1].x_;
				}
				if (camera_gpos.x_ < CAMERALIMIT_GPOS[0].x_)
				{
					camera_gpos.x_ = CAMERALIMIT_GPOS[0].x_;
				}
				if (camera_gpos.z_ > CAMERALIMIT_GPOS[0].z_)
				{
					camera_gpos.z_ = CAMERALIMIT_GPOS[0].z_;
				}
				if (camera_gpos.z_ < CAMERALIMIT_GPOS[2].z_)
				{
					camera_gpos.z_ = CAMERALIMIT_GPOS[2].z_;
				}
				ConvertGlobalToScreen(camera_gpos, camera_spos);
				pos.x_ = camera_spos[0];
				pos.y_ = camera_spos[1];
			}
			else if (mode_ == 0)
			{
				switch (0)
				{
				case 0:
					Update_normal();
					SetCameraRect();
					camera_spos[0] = (int)cursor_pos.x_;
					camera_spos[1] = (int)cursor_pos.y_;
					ConvertScreenToGlobal(camera_spos, camera_gpos);
					CAMERA_ADS_GPOS.Set(camera_gpos.x_ - CAMERA_BASE_GPOS.x_, 0f, camera_gpos.z_ - CAMERA_BASE_GPOS.z_);
					ConvertGlobalToAddressMain(CAMERA_ADS_GPOS, camera_address);
					if (cursorCnt_ < 80)
					{
						cursorCnt_++;
					}
					if (flag || flag2 || flag3 || flag4)
					{
						subPlayer_ = null;
						subSvisor_ = null;
						cursorCnt_ = 0;
					}
					if (subCursorCnt_ < 100 && subPlayer_ != null)
					{
						subCursorCnt_++;
					}
					if (subCursorCnt_ >= 100)
					{
						subCursorCnt_ = 0;
						subPlayer_ = null;
						subSvisor_ = null;
					}
					if (flg_disp)
					{
						Collision((int)cursor_pos.x_, (int)cursor_pos.y_);
					}
					break;
				case 1:
					if (flag)
					{
						ch_global_pos.z_ += 10f;
					}
					if (flag2)
					{
						ch_global_pos.z_ -= 10f;
					}
					if (flag3)
					{
						ch_global_pos.x_ -= 10f;
					}
					if (flag4)
					{
						ch_global_pos.x_ += 10f;
					}
					ConvertGlobalToScreen(ch_global_pos, ch_screen_pos);
					pos.x_ = ch_screen_pos[0];
					pos.y_ = ch_screen_pos[1];
					break;
				}
			}
			else if (mode_ == 1)
			{
				pos.x_ = udat_.ball_.screen_shadowPos_[0];
				pos.y_ = udat_.ball_.screen_shadowPos_[1];
			}
			else
			{
				if (mode_ != 2 || apdat_.frmSoccer_.state_ == 8)
				{
					return;
				}
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				if (udat_.gm_.state_ == 8 && udat_.gm_.stateCnt_ >= GameManager.WAIT_GOAL[0])
				{
					if (udat_.ball_.chInGoal_ != 2)
					{
						Team team = udat_.teams_[1 - udat_.ball_.chInGoal_];
						if (team.lastKickeer_ != -1)
						{
							Player player2 = team.players_[team.lastKickeer_];
							num = (int)JMath.Abs((float)player2.screen_pos_[0] - pos.x_);
							num2 = (int)JMath.Abs((float)player2.screen_pos_[1] - pos.y_);
							num3 = GameUtil.RateConvert(num, 20, 90, 0, 10, true);
							if ((float)player2.screen_pos_[0] < pos.x_)
							{
								num3 *= -1;
							}
							num4 = GameUtil.RateConvert(num2, 20, 90, 0, 10, true);
							if ((float)player2.screen_pos_[1] < pos.y_)
							{
								num4 *= -1;
							}
						}
					}
				}
				else
				{
					Ball ball_ = udat_.ball_;
					if (ball_ != null)
					{
						num = (int)JMath.Abs((float)ball_.screen_shadowPos_[0] - pos.x_);
						num2 = (int)JMath.Abs((float)ball_.screen_shadowPos_[1] - pos.y_);
						num3 = GameUtil.RateConvert(num, 20, 90, 0, 13, true);
						if ((float)ball_.screen_shadowPos_[0] < pos.x_)
						{
							num3 *= -1;
						}
						num4 = GameUtil.RateConvert(num2, 20, 90, 0, 13, true);
						if ((float)ball_.screen_shadowPos_[1] < pos.y_)
						{
							num4 *= -1;
						}
					}
				}
				if (udat_.gm_.state_ == 8 && udat_.gm_.stateCnt_ >= GameManager.WAIT_GOAL[0])
				{
					pos.x_ += num3;
					pos.y_ += num4;
					return;
				}
				chPos[0].x_ = pos.x_;
				chPos[0].z_ = pos.y_;
				chPos[1].x_ = pos.x_ + (float)num3;
				chPos[1].z_ = pos.y_ + (float)num4;
				camera_spos[0] = (int)pos.x_ + num3;
				camera_spos[1] = (int)pos.y_ + num4;
				ConvertScreenToGlobal(camera_spos, camera_gpos);
				bool flag5 = false;
				for (int i = 0; i < 4; i++)
				{
					chPos[2].x_ = cameraLimitSPos[i][0];
					chPos[2].z_ = cameraLimitSPos[i][1];
					chPos[3].x_ = cameraLimitSPos[(i + 1) % 4][0];
					chPos[3].z_ = cameraLimitSPos[(i + 1) % 4][1];
					if (GameUtil.ChCrossLine(chPos[0], chPos[1], chPos[2], chPos[3]))
					{
						Vector3D crossPoint = GameUtil.GetCrossPoint(chPos[2], chPos[3], chPos[1], true);
						pos.x_ = crossPoint.x_;
						pos.y_ = crossPoint.z_;
						flag5 = true;
						break;
					}
				}
				if (!flag5)
				{
					pos.x_ += num3;
					pos.y_ += num4;
				}
			}
		}

		public static void ConvertGlobalToScreen(Vector3D global_pos, int[] screen_pos)
		{
			float num = (global_pos.x_ - -450f) * (float)MAP_SCREEN_W2 / 900f + (global_pos.z_ - 900f) * (float)(-MAP_SCREEN_W1) / -1800f;
			float num2 = (global_pos.z_ - 900f) * (float)(-MAP_SCREEN_H1) / -1800f + (global_pos.x_ - -450f) * (float)(-MAP_SCREEN_H2) / 900f;
			num2 += global_pos.y_;
			screen_pos[0] = (int)num;
			screen_pos[1] = (int)num2;
		}

		public static void ConvertScreenToCamera(Vector3D screen_pos, Vector3D camera_pos)
		{
			int num = (int)screen_pos.x_ + DRAW_CENTER_X - (int)pos.x_;
			int num2 = (int)screen_pos.y_ + DRAW_CENTER_Y - (int)pos.y_;
			num2 = DRAW_VIEW_Y + DRAW_VIEW_H - num2;
			camera_pos.x_ = num;
			camera_pos.y_ = num2;
		}

		public static void ConvertScreenToCamera(Vector3D screen_pos, int[] camera_pos)
		{
			int num = (int)screen_pos.x_ + DRAW_CENTER_X - (int)pos.x_;
			int num2 = (int)screen_pos.y_ + DRAW_CENTER_Y - (int)pos.y_;
			num2 = DRAW_VIEW_Y + DRAW_VIEW_H - num2;
			camera_pos[0] = num;
			camera_pos[1] = num2;
		}

		public static void ConvertScreenToCamera(int sx, int sy, int[] camera_pos)
		{
			int num = sx + DRAW_CENTER_X - (int)pos.x_;
			int num2 = sy + DRAW_CENTER_Y - (int)pos.y_;
			num2 = DRAW_VIEW_Y + DRAW_VIEW_H - num2;
			camera_pos[0] = num;
			camera_pos[1] = num2;
		}

		public static void ConvertScreenToCamera(int[] screen_pos, int[] camera_pos)
		{
			int num = screen_pos[0] + DRAW_CENTER_X - (int)pos.x_;
			int num2 = screen_pos[1] + DRAW_CENTER_Y - (int)pos.y_;
			num2 = DRAW_VIEW_Y + DRAW_VIEW_H - num2;
			camera_pos[0] = num;
			camera_pos[1] = num2;
		}

		public static void ConvertGlobalToCamera(Vector3D global_pos, int[] var_out)
		{
			ConvertGlobalToScreen(global_pos, temp_s);
			ConvertScreenToCamera(temp_s, var_out);
		}

		public static void ConvertAddressToScreen(int xi, int yi, int[] screen_pos)
		{
			screen_pos[0] = xi * 60 / 2 + yi * 60 / 2 - MAP_SCREEN_W1;
			screen_pos[1] = xi * 60 / 2 - yi * 30 / 2 - MAP_SCREEN_H1;
		}

		public static void ConvertAddressToGlobal(int xi, int yi, Vector3D global_pos)
		{
			global_pos.x_ = (float)xi * (900f / (float)Map.MAPCHIP_COAT_WNUM) + 900f / (float)Map.MAPCHIP_COAT_WNUM / 2f - 450f;
			global_pos.y_ = 0f;
			global_pos.z_ = (float)yi * (1800f / (float)Map.MAPCHIP_COAT_HNUM) + 1800f / (float)Map.MAPCHIP_COAT_HNUM / 2f - 900f;
		}

		public static void ConvertGlobalToAddress(Vector3D global_pos, int[] out_address)
		{
			out_address[0] = (int)((global_pos.x_ - 1f + 450f) / (900f / (float)Map.MAPCHIP_COAT_WNUM));
			out_address[1] = (int)((global_pos.z_ - 1f + 900f) / (1800f / (float)Map.MAPCHIP_COAT_HNUM));
		}

		public static void ConvertGlobalToAddressMain(Vector3D global_pos, int[] out_address)
		{
			out_address[0] = (int)(global_pos.x_ / (900f / (float)Map.MAPCHIP_COAT_WNUM));
			out_address[1] = (int)(global_pos.z_ / (1800f / (float)Map.MAPCHIP_COAT_HNUM));
		}

		public static void ConvertScreenToGlobal(int[] screen_pos, Vector3D var_out)
		{
			float num = screen_pos[0] - 480;
			float num2 = screen_pos[1] - 480;
			float num3 = (float)MAP_SCREEN_W2 / 900f;
			float num4 = (float)(-MAP_SCREEN_W1) / -1800f;
			float num5 = (float)(-MAP_SCREEN_H1) / -1800f;
			float num6 = (float)(-MAP_SCREEN_H2) / 900f;
			float num7 = 1f / (num3 * num5 - num4 * num6);
			float num8 = (-(num6)) * num7 * (float)screen_pos[0] + num3 * num7 * (float)screen_pos[1];
			float num9 = (float)screen_pos[0] / num3 - num4 * num8 / num3;
			var_out.z_ = num8 + 900f;
			var_out.x_ = num9 + -450f;
		}

		public static bool ChDisplayable(int c_x, int c_y, int w, int h)
		{
			if (c_x + w < 0)
			{
				return false;
			}
			if (c_x - w > 240)
			{
				return false;
			}
			if (c_y + h < SoccerForm.VIEW_Y)
			{
				return false;
			}
			if (c_y - h > SoccerForm.VIEW_Y + SoccerForm.VIEW_H)
			{
				return false;
			}
			return true;
		}

		public static void SetMapPosData()
		{
			MAP_SCREEN_LEFT = -MAP_SCREEN_W / 2;
			MAP_SCREEN_RIGHT = MAP_SCREEN_LEFT + MAP_SCREEN_W;
			MAP_SCREEN_TOP = MAP_SCREEN_H / 2;
			MAP_SCREEN_BOTTOM = MAP_SCREEN_TOP - MAP_SCREEN_H;
		}

		public void SetCameraLimitData()
		{
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteVector3D(os, pos);
			StreamUtil.WriteVector3D(os, old_pos);
			StreamUtil.WriteVector3D(os, save_pos);
			StreamUtil.WriteVector3D(os, save_cursorPos);
			StreamUtil.WriteVector3D(os, cursor_pos);
		}

		public void Deserialize(InputStream stream)
		{
			pos = StreamUtil.ReadVector3D(stream);
			old_pos = StreamUtil.ReadVector3D(stream);
			save_pos = StreamUtil.ReadVector3D(stream);
			save_cursorPos = StreamUtil.ReadVector3D(stream);
			cursor_pos = StreamUtil.ReadVector3D(stream);
		}

		public void ChangeMode(int next_mode)
		{
			mode_ = next_mode;
		}

		public static Vector3D GetPosFromAddress(int xi, int yi)
		{
			COATADDRESS_POS.x_ = -450f + (float)xi * 900f / (float)Map.MAPCHIP_COAT_WNUM;
			COATADDRESS_POS.y_ = 0f;
			COATADDRESS_POS.z_ = -900f + (float)yi * 1800f / (float)Map.MAPCHIP_COAT_HNUM;
			return COATADDRESS_POS;
		}

		public void DrawTest(Graphics g)
		{
		}

		public void DrawCursor(Graphics g)
		{
			if (!(flg_disp)) { } else if ((cursorCnt_ < 80)) {
				GameView gameView = GameView.GetInstance();
				int scale_ = apdat_.frmGame_.scale_;
				temp_cursorSPos[0] = (int)(pos.x_ + (cursor_pos.x_ - pos.x_) * (float)scale_ / 100f);
				temp_cursorSPos[1] = (int)(pos.y_ + (cursor_pos.y_ - pos.y_) * (float)scale_ / 100f);
				ConvertScreenToCamera(temp_cursorSPos, temp_cpos);
				g.DrawImage(apdat_.resCom2_.img[80], temp_cpos[0], temp_cpos[1]);
			}
		}

		public void SetCameraRect()
		{
			if (!(CAMERA_LIMIT)) { } else {

if (pos.x_ < (float)(CAMERA_RECT[0][0] + 120))
				{
					pos.x_ = CAMERA_RECT[0][0] + 120;
				}
if (pos.x_ > (float)(CAMERA_RECT[1][0] - 120))
				{
					pos.x_ = CAMERA_RECT[1][0] - 120;
				}
if (pos.y_ < (float)(CAMERA_RECT[1][1] + 120))
				{
					pos.y_ = CAMERA_RECT[1][1] + 120;
				}
if (pos.y_ > (float)(CAMERA_RECT[0][1] - 120))
				{
					pos.y_ = CAMERA_RECT[0][1] - 120;
				}
GameView gameView = GameView.GetInstance();
int gameWidth = gameView.GetGameWidth();
int gameHeight = gameView.GetGameHeight();
if (cursor_pos.x_ < pos.x_ - (float)(gameWidth / 2))
				{
					cursor_pos.x_ = pos.x_ - (float)(gameWidth / 2);
				}
if (cursor_pos.x_ > pos.x_ + (float)(gameWidth / 2))
				{
					cursor_pos.x_ = pos.x_ + (float)(gameWidth / 2);
				}
if (cursor_pos.y_ < pos.y_ - (float)(gameHeight / 2))
				{
					cursor_pos.y_ = pos.y_ - (float)(gameHeight / 2);
				}
if (cursor_pos.y_ > pos.y_ + (float)(gameHeight / 2 - GameForm.VIEW_Y))
				{
					cursor_pos.y_ = pos.y_ + (float)(gameHeight / 2 - GameForm.VIEW_Y);
				}
if (cursor_pos.x_ < (float)(CAMERA_RECT[0][0] + CURSOR_LIMIT_OFF[2]))
				{
					cursor_pos.x_ = CAMERA_RECT[0][0] + CURSOR_LIMIT_OFF[2];
				}
if (cursor_pos.x_ > (float)(CAMERA_RECT[1][0] + CURSOR_LIMIT_OFF[3]))
				{
					cursor_pos.x_ = CAMERA_RECT[1][0] + CURSOR_LIMIT_OFF[3];
				}
if (cursor_pos.y_ < (float)(CAMERA_RECT[1][1] + CURSOR_LIMIT_OFF[1]))
				{
					cursor_pos.y_ = CAMERA_RECT[1][1] + CURSOR_LIMIT_OFF[1];
				}
if (cursor_pos.y_ > (float)(CAMERA_RECT[0][1] + CURSOR_LIMIT_OFF[0] - GameForm.VIEW_Y))
				{
					cursor_pos.y_ = CAMERA_RECT[0][1] + CURSOR_LIMIT_OFF[0] - GameForm.VIEW_Y;
				}
}
		}

		public void Update_normal()
		{
			v.Set(0f, 0f, 0f);
			int num = 0;
			int num2 = 0;
			int num3;
			int num4;
			if (cursorPressCnt_ <= 6)
			{
				num3 = 3;
				num4 = 4;
			}
			else if (cursorPressCnt_ <= 20)
			{
				num3 = 6;
				num4 = 8;
			}
			else
			{
				num3 = 8;
				num4 = 10;
			}
			bool flag = canvas_.CheckKeyState(131072);
			bool flag2 = canvas_.CheckKeyState(524288);
			bool flag3 = canvas_.CheckKeyState(65536);
			bool flag4 = canvas_.CheckKeyState(262144);
			if (flag4 && flag)
			{
				num = num4 / 1;
				num2 = num4 / 2;
			}
			else if (flag4 && flag2)
			{
				num = num4 / 1;
				num2 = -num4 / 2;
			}
			else if (flag3 && flag)
			{
				num = -num4 / 1;
				num2 = num4 / 2;
			}
			else if (flag3 && flag2)
			{
				num = -num4 / 1;
				num2 = -num4 / 2;
			}
			else if (flag)
			{
				num2 = num3;
			}
			else if (flag2)
			{
				num2 = -num3;
			}
			else if (flag4)
			{
				num = num3;
			}
			else if (flag3)
			{
				num = -num3;
			}
			v.Set(num, num2, 0f);
			cursor_pos.Add(v);
			if (num == 0 && num2 == 0)
			{
				cursorPressCnt_ = 0;
				Vector3D touchCameraSpeed_ = apdat_.frmGame_.touchCameraSpeed_;
				Vector3D touchCameraDecimal_ = apdat_.frmGame_.touchCameraDecimal_;
				FastVector touchCameraPoint_ = apdat_.frmGame_.touchCameraPoint_;
				bool touchDragCamera_ = apdat_.frmGame_.touchDragCamera_;
				bool touchBuildDrag_ = apdat_.frmGame_.touchBuildDrag_;
				if (touchCameraSpeed_.x_ != 0f || touchCameraSpeed_.z_ != 0f)
				{
					touchCameraDecimal_.Add(touchCameraSpeed_);
					touchCameraSpeed_.Set(touchCameraSpeed_.x_ / 1.75f, touchCameraSpeed_.y_ / 1.75f, touchCameraSpeed_.z_ / 1.75f);
					if (JMath.Abs(touchCameraSpeed_.x_) < 0.5f)
					{
						touchCameraSpeed_.x_ = 0f;
					}
					if (JMath.Abs(touchCameraSpeed_.z_) < 0.5f)
					{
						touchCameraSpeed_.z_ = 0f;
					}
					num = (int)touchCameraDecimal_.x_;
					num2 = (int)touchCameraDecimal_.z_;
					touchCameraDecimal_.Add(-num, 0f, -num2);
					cursor_pos.Add(num, -num2, 0f);
					cursorPressCnt_++;
					flg_disp = false;
					pos.Set(cursor_pos);
				}
			}
			else
			{
				cursorPressCnt_++;
				flg_disp = true;
				pos.Set(cursor_pos);
			}
		}

		public static void SetDrawCenter(int viewX, int viewY, int viewW, int viewH)
		{
			DRAW_CENTER_X = (viewX + viewW) / 2;
			DRAW_CENTER_Y = (viewY + viewH) / 2;
			DRAW_VIEW_X = viewX;
			DRAW_VIEW_Y = viewY;
			DRAW_VIEW_W = viewW;
			DRAW_VIEW_H = viewH;
		}

		public void Collision(int cursor_sx, int cursor_sy)
		{
			temp_spos[0] = cursor_sx;
			temp_spos[1] = cursor_sy;
			ConvertScreenToGlobal(temp_spos, temp_gpos);
			CAMERA_ADS_GPOS.Set(temp_gpos.x_ - CAMERA_BASE_GPOS.x_, 0f, temp_gpos.z_ - CAMERA_BASE_GPOS.z_);
			ConvertGlobalToAddressMain(CAMERA_ADS_GPOS, temp_address);
			FormManager formManager = FormManager.GetInstance();
			FormBase currentForm = formManager.GetCurrentForm();
			if (currentForm.GetId() == 9) {

			bool flag = false;
			for (int i = 0; i < udat_.tAreas_.Length; i++)
			{
				if (flag)
				{
					break;
				}
				for (int j = 0; j < udat_.tAreas_[i].Length; j++)
				{
					if (flag)
					{
						break;
					}
					TArea tArea = udat_.tAreas_[i][j];
					for (int k = 0; k < tArea.tGroupAreas_.Length; k++)
					{
						if (flag)
						{
							break;
						}
						TGroupArea tGroupArea = tArea.tGroupAreas_[k];
						for (int l = 0; l < tGroupArea.tPlayerList_.Size(); l++)
						{
							TPlayer tPlayer = (TPlayer)tGroupArea.tPlayerList_.ElementAt(l);
							if (!BitUtil.Check(tPlayer.flag_, 1) && GameUtil.ChRectHitRect(cursor_sx - 4, cursor_sy + 4, 8, 8, tPlayer.screen_pos_[0] - 7, tPlayer.screen_pos_[1] + 28, 14, 28, true))
							{
								flag = true;
								subPlayer_ = tPlayer;
								break;
							}
						}
					}
				}
				if (!flag && GameUtil.ChRectHitRect(cursor_sx - 4, cursor_sy + 4, 8, 8, udat_.tp_svisor_.screen_pos_[0] - 7, udat_.tp_svisor_.screen_pos_[1] + 28, 14, 28, true))
				{
					flag = true;
					subSvisor_ = udat_.tp_svisor_;
				}
			}
			subTenant_ = null;
			for (int m = 0; m < udat_.tenants_.Length; m++)
			{
				Tenant tenant = udat_.tenants_[m];
				if ((tenant.root_ != 1 || udat_.tAreaRootNum_ != 1) && (temp_address[0] == tenant.address_[0] || temp_address[0] == tenant.address_[0] + 1) && (temp_address[1] == tenant.address_[1] || temp_address[1] == tenant.address_[1] - 1))
				{
					subTenant_ = tenant;
				}
			}
}
		}
	}
}
