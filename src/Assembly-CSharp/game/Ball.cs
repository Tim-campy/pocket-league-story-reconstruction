using data;
using form;
using java.lang;
using kairo.common.cfg;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace game
{
	public class Ball
	{
		public const int FLG_NOTDISP = 1;

		public const int FLG_ANIME = 2;

		public const int FLG_HAVEBALL = 4;

		public const int FLG_RECEIVABLE = 8;

		public const int FLG_JAMP = 16;

		public const int FLG_SHOOT = 32;

		public const int FLG_CENTERING = 64;

		public const float TEMP_R11 = 1f;

		public const float TEMP_R12 = 2f;

		public const float TEMP_R21 = 3f;

		public const float TEMP_R22 = 12f;

		public const int DRIBBLE_DIRECT_CYCLE = 16;

		public const int DRIBBLE_DIRECT_ROTATETIME_MIN = 8;

		public const int DRIBBLE_DIRECT_ROTATETIME_MAX = 2;

		public const int PASSMODE_ALLY = 0;

		public const int PASSMODE_SPACE = 1;

		public const int PASSMODE_CLEAR = 2;

		public const int PASSMODE_CENTERING = 3;

		public const int PASSMODE_CENTERINGAREA = 4;

		public const int PASSMODE_THROWIN = 5;

		public const float CLEAR_DISY = 60f;

		public const float PASS_DISY_MIN = 5f;

		public const float PASS_DISY_MAX = 30f;

		public const float PASS_GRAVITY_NORMAL = -0.6f;

		public const float PASS_GRAVITY_LONG = -0.25f;

		public const float PASS_GRAVITY_LONG_PASS = -0.35f;

		public const float PASS_GRAVITY_CLEAR = -0.1f;

		public const float PASS_GRAVITY_SHOOT = -0.1f;

		public const float PASS_DIS_RATE = 1.12f;

		public const float PASS_DIS_RATE_LONG = 1.27f;

		public const float CENTERING_DECRERATE = 0.2f;

		public const float CENTERING_HEIGHT = 68f;

		public const int CENTERING_TIME = 36;

		public const int SHOOTMODE_KICK = 0;

		public const int SHOOTMODE_HEAD = 1;

		public const float FRICTION_START_VX = 0.99f;

		public const float FRICTION_START_VZ = 0.99f;

		public const float FRICTION_END_VX = 0.93f;

		public const float FRICTION_END_VZ = 0.93f;

		public const int FRICTION_GROUND_TIME = 60;

		public const float FRICTION_AIR_VX = 0.995f;

		public const float FRICTION_AIR_VY = 0.995f;

		public const float FRICTION_AIR_VZ = 0.995f;

		public const float FRICTION_AIR_VX2 = 0.97f;

		public const float FRICTION_AIR_VZ2 = 0.97f;

		public const float FRICTION_AIR_VX_LONG = 0.96f;

		public const float FRICTION_AIR_VZ_LONG = 0.96f;

		public const int FRICTION_AIR_F2TIME = 100;

		public const int FRICTION_AIR_F2TIME_LONG = 100;

		public const int SAVENUM_MAX = 300;

		public const int SAVEPOS_RATE = 4;

		public const int TARGETPID_NONE = -1;

		public const float TRAP_ABLE_H = 20f;

		public const float DISPOUT_LEN = 80f;

		private const float CENTERING_AREA_LEFT = -128.57143f;

		private const float CENTERING_AREA_RIGHT = 128.57143f;

		private const float CENTERING_AREA_G2_TOP = 846f;

		private const float CENTERING_AREA_G2_BOTTOM = 720f;

		private const float CENTERING_AREA_G1_TOP = -720f;

		private const float CENTERING_AREA_G1_BOTTOM = -846f;

		public const float HEADING_H_MAX = 40f;

		public AppData apdat_;

		private Canvas canvas_;

		public UserData udat_;

		public int teamId_;

		public int frame_;

		public int direct_;

		public Vector3D pos_;

		public Vector3D shadowPos_;

		public Vector3D fallPos_;

		public Vector3D receiverPos_;

		public Vector3D lineOutPos_;

		public Vector3D v_;

		public Vector3D a_;

		public int[] address_;

		public int[] screen_pos_;

		public int[] screen_shadowPos_;

		public int[] camera_pos_;

		public float v_len_;

		public int flag_;

		public int state_;

		public int stateCnt_;

		public Player owner_;

		public Player lastOwner_;

		public static int RECEIVE_WAIT_MIN = 5;

		public int receive_wait;

		public int boundNum_;

		public int chInGoal_;

		public int rotate_;

		public static readonly float[][][] DRIBBLE_DIRECT_DIS = new float[4][][]
		{
			new float[4][]
			{
				new float[2] { 1f, -1f },
				new float[2] { 2f, -2f },
				new float[2] { 3f, -3f },
				new float[2] { 12f, -12f }
			},
			new float[4][]
			{
				new float[2] { 1f, 1f },
				new float[2] { 2f, 2f },
				new float[2] { 3f, 3f },
				new float[2] { 12f, 12f }
			},
			new float[4][]
			{
				new float[2] { -1f, 1f },
				new float[2] { -2f, 2f },
				new float[2] { -3f, 3f },
				new float[2] { -12f, 12f }
			},
			new float[4][]
			{
				new float[2] { -1f, -1f },
				new float[2] { -2f, -2f },
				new float[2] { -3f, -3f },
				new float[2] { -12f, -12f }
			}
		};

		public static readonly int[][] THROWIN_OFF = new int[4][]
		{
			new int[2] { 0, -15 },
			new int[2] { 0, -15 },
			new int[2] { -1, -15 },
			new int[2] { -1, -15 }
		};

		public int frameNum_;

		public Vector3D[] savePos_ = new Vector3D[300];

		public Vector3D saveV_ = new Vector3D();

		public Vector3D saveA_ = new Vector3D();

		public int savePosFrame_;

		public int fallTime_;

		public int targetPId;

		public static readonly int[][][] SAVING_OFF = new int[2][][]
		{
			new int[3][]
			{
				new int[2] { -5, -9 },
				new int[2] { 2, -12 },
				new int[2] { 7, 0 }
			},
			new int[3][]
			{
				new int[2] { -8, -8 },
				new int[2] { -1, -12 },
				new int[2] { 7, 0 }
			}
		};

		public static int[] SOUND_PASS = new int[6] { 8, 8, 10, 9, 9, 8 };

		public static int SOUND_SHOOT_KICK = 15;

		public static int SOUND_SHOOT_HEAD = 16;

		public static int SOUND_HEAD = 10;

		public static readonly string[] SOUND_NAME = new string[0];

		public static readonly int[] SOUND_KICK = new int[3] { 8, 9, 10 };

		public static readonly int[] SOUND_SHOOT = new int[2] { 15, 16 };

		public static int SOUND_RANDOM = 1;

		public Vector3D old_pos = new Vector3D();

		public Vector3D temp_pos = new Vector3D();

		public int[] temp_spos = new int[2];

		internal float MIN_AY = -0.2f;

		private int[] camera_pos_shadow = new int[2];

		private int[] camera_pos_dribble = new int[2];

		private static Vector3D vec1 = new Vector3D();

		private static Vector3D vec2 = new Vector3D();

		private static Vector3D vec3 = new Vector3D();

		private static Vector3D vec_GK = new Vector3D();

		private int[] rand_cos = new int[3];

		public Vector3D g1 = new Vector3D();

		public Vector3D shoot_goalPos = new Vector3D();

		private int[] headableIndex = new int[2];

		public void Init()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			canvas_ = Canvas.GetInstance();
			frame_ = 0;
			direct_ = 0;
			pos_ = new Vector3D();
			shadowPos_ = new Vector3D();
			fallPos_ = new Vector3D();
			receiverPos_ = new Vector3D();
			lineOutPos_ = new Vector3D();
			v_ = new Vector3D();
			a_ = new Vector3D();
			address_ = new int[2];
			screen_pos_ = new int[2];
			screen_shadowPos_ = new int[2];
			camera_pos_ = new int[2];
			flag_ = 0;
			state_ = 0;
			stateCnt_ = 0;
			owner_ = null;
			lastOwner_ = null;
			flag_ = BitUtil.On(flag_, 2);
			flag_ = BitUtil.On(flag_, 8);
			rotate_ = 0;
			boundNum_ = 0;
			savePos_ = new Vector3D[300];
			for (int i = 0; i < savePos_.Length; i++)
			{
				savePos_[i] = new Vector3D();
			}
			saveV_ = new Vector3D();
			saveA_ = new Vector3D();
			savePosFrame_ = 0;
			targetPId = -1;
		}

		public bool Update()
		{
			frame_ = (frame_ + 1) % int.MaxValue;
			savePosFrame_ = (savePosFrame_ + 1) % int.MaxValue;
			bool flag = BitUtil.Check(flag_, 64);
			old_pos.Set(pos_);
			if (SoccerForm.DEBUG_BALLTEST_MODE == 1)
			{
				bool flag2 = canvas_.CheckKeyState(131072);
				bool flag3 = canvas_.CheckKeyState(524288);
				bool flag4 = canvas_.CheckKeyState(65536);
				bool flag5 = canvas_.CheckKeyState(262144);
				bool flag6 = canvas_.CheckKeyState(128);
				bool flag7 = canvas_.CheckKeyState(256);
				if (flag2)
				{
					pos_.z_ += 10f;
				}
				if (flag3)
				{
					pos_.z_ += -10f;
				}
				if (flag4)
				{
					pos_.x_ += -10f;
				}
				if (flag5)
				{
					pos_.x_ += 10f;
				}
				if (flag6)
				{
					pos_.y_ += 3f;
				}
				if (flag7)
				{
					pos_.y_ += -3f;
				}
				v_.Set(0f, 0f, 0f);
				a_.Set(0f, 0f, 0f);
			}
			if (owner_ != null)
			{
				pos_.Set(owner_.pos_);
			}
			else
			{
float unusedOriginalSlot7;
float dragX;
float dragZ;


				if (a_.y_ == 0f || v_.y_ == 0f || pos_.y_ <= 0f)
				{
					dragX = GameUtil.RateConvert(savePosFrame_, 0f, 60f, 0.99f, 0.93f, true);
					dragZ = GameUtil.RateConvert(savePosFrame_, 0f, 60f, 0.99f, 0.93f, true);
					v_.x_ *= dragX;
					v_.z_ *= dragZ;
				}
				else if (!flag)
				{


					if (a_.y_ == -0.25f || a_.y_ == -0.35f)
					{
						dragX = GameUtil.RateConvert(savePosFrame_, 0f, fallTime_ * 100 / 100, 0.995f, 0.96f, true);
						dragZ = GameUtil.RateConvert(savePosFrame_, 0f, fallTime_ * 100 / 100, 0.995f, 0.96f, true);
					}
					else
					{
						dragX = GameUtil.RateConvert(savePosFrame_, 0f, fallTime_ * 100 / 100, 0.995f, 0.97f, true);
						dragZ = GameUtil.RateConvert(savePosFrame_, 0f, fallTime_ * 100 / 100, 0.995f, 0.97f, true);
					}
					v_.x_ *= dragX;
					v_.y_ *= 0.995f;
					v_.z_ *= dragZ;
				}
				v_.Add(a_);
				pos_.Add(v_);
				v_len_ = (float)JMath.Sqrt(v_.x_ * v_.x_ + v_.z_ * v_.z_);
			}
			if (receive_wait >= 0)
			{
				receive_wait--;
				if (receive_wait <= 0)
				{
					flag_ = BitUtil.On(flag_, 8);
				}
			}
			if (pos_.y_ < 0f)
			{
				boundNum_++;
				pos_.y_ *= -1f;
				v_.y_ = (0f - v_.y_) / 2f;
				v_.x_ *= 0.9f;
				v_.z_ *= 0.9f;
				a_.x_ = 0f;
				a_.z_ = 0f;
				if (v_.y_ < 0.3f)
				{
					SetStop();
				}
				else
				{
					SetSavePos(pos_, v_, a_);
				}
				targetPId = -1;
				if (pos_.x_ < -530f || pos_.x_ > 530f || pos_.z_ > 980f || pos_.z_ < -980f)
				{
					flag_ = BitUtil.On(flag_, 1);
					v_.Set(0f, 0f, 0f);
					a_.Set(0f, 0f, 0f);
				}
			}
			if (!BitUtil.Check(flag_, 16) && (pos_.x_ < -530f || pos_.x_ > 530f || pos_.z_ > 980f || pos_.z_ < -980f))
			{
				flag_ = BitUtil.On(flag_, 1);
				v_.Set(0f, 0f, 0f);
				a_.Set(0f, 0f, 0f);
			}
			bool flag8 = false;
			bool flag9 = false;
float unusedOriginalSlot12;
			chInGoal_ = 2;
			if (pos_.z_ > 900f || pos_.z_ < -900f)
			{
				bool flag10 = false;
				bool flag11 = false;
				bool flag12 = false;
				if (pos_.y_ < 26f)
				{
					if ((Map.GOAL1_GLOBAL_POS[0].x_ + 11f - old_pos.x_) * (Map.GOAL1_GLOBAL_POS[0].x_ + 11f - pos_.x_) <= 0f)
					{
						if (old_pos.x_ < Map.GOAL1_GLOBAL_POS[0].x_ + 11f)
						{
							pos_.x_ = Map.GOAL1_GLOBAL_POS[0].x_ + 11f - 1f;
						}
						else
						{
							pos_.x_ = Map.GOAL1_GLOBAL_POS[0].x_ + 11f + 1f;
						}
						flag9 = true;
					}
					if ((Map.GOAL1_GLOBAL_POS[2].x_ - 11f - old_pos.x_) * (Map.GOAL1_GLOBAL_POS[2].x_ - 11f - pos_.x_) <= 0f)
					{
						if (old_pos.x_ > Map.GOAL1_GLOBAL_POS[2].x_ - 11f)
						{
							pos_.x_ = Map.GOAL1_GLOBAL_POS[2].x_ - 11f + 1f;
						}
						else
						{
							pos_.x_ = Map.GOAL1_GLOBAL_POS[2].x_ - 11f - 1f;
						}
						flag9 = true;
					}
				}
				if (pos_.x_ >= Map.GOAL1_GLOBAL_POS[0].x_ + 11f && pos_.x_ <= Map.GOAL1_GLOBAL_POS[2].x_ - 11f)
				{
					flag10 = true;
				}
				if (flag10)
				{
					if (pos_.y_ < 26f)
					{
						if ((Map.GOAL2_GLOBAL_POS[2].z_ - 11f - old_pos.z_) * (Map.GOAL2_GLOBAL_POS[2].z_ - 11f - pos_.z_) <= 0f)
						{
							pos_.z_ = Map.GOAL2_GLOBAL_POS[2].z_ - 11f - 1f;
							flag9 = true;
						}
						if ((Map.GOAL1_GLOBAL_POS[2].z_ + 11f - old_pos.z_) * (Map.GOAL1_GLOBAL_POS[2].z_ + 11f - pos_.z_) <= 0f)
						{
							pos_.z_ = Map.GOAL1_GLOBAL_POS[2].z_ + 11f + 1f;
							flag9 = true;
						}
					}
					if (pos_.z_ >= 900f && pos_.z_ <= Map.GOAL2_GLOBAL_POS[2].z_ - 11f)
					{
						flag12 = true;
					}
					if (pos_.z_ <= -900f && pos_.z_ >= Map.GOAL1_GLOBAL_POS[2].z_ + 11f)
					{
						flag12 = true;
					}
					if (flag12)
					{
						if ((21f - old_pos.y_) * (21f - pos_.y_) <= 0f)
						{
							if (old_pos.y_ > 21f)
							{
								pos_.y_ = 22f;
							}
							else
							{
								pos_.y_ = 20f;
							}
						}
						if (pos_.y_ < 21f)
						{
							if (pos_.z_ < 0f)
							{
								chInGoal_ = 0;
							}
							else
							{
								chInGoal_ = 1;
							}
						}
						if (udat_.gm_.state_ == 0 && pos_.y_ < 21f && SoccerForm.DEBUG_BALLTEST_MODE == 0)
						{
							udat_.gm_.ChangeState(8);
						}
					}
				}
				if (pos_.y_ < 21f)
				{
					if ((Map.GOAL1_GLOBAL_POS[0].x_ - 11f - old_pos.x_) * (Map.GOAL1_GLOBAL_POS[0].x_ - 11f - pos_.x_) <= 0f)
					{
						if (old_pos.x_ < Map.GOAL1_GLOBAL_POS[0].x_ - 11f)
						{
							pos_.x_ = Map.GOAL1_GLOBAL_POS[0].x_ - 11f - 1f;
						}
						else
						{
							pos_.x_ = Map.GOAL1_GLOBAL_POS[0].x_ - 11f + 1f;
						}
					}
					if ((Map.GOAL1_GLOBAL_POS[2].x_ + 11f - old_pos.x_) * (Map.GOAL1_GLOBAL_POS[2].x_ + 11f - pos_.x_) <= 0f)
					{
						if (old_pos.x_ > Map.GOAL1_GLOBAL_POS[2].x_ + 11f)
						{
							pos_.x_ = Map.GOAL1_GLOBAL_POS[2].x_ + 11f + 1f;
						}
						else
						{
							pos_.x_ = Map.GOAL1_GLOBAL_POS[2].x_ + 11f - 1f;
						}
					}
				}
				if (pos_.x_ <= Map.GOAL1_GLOBAL_POS[0].x_ - 11f || pos_.x_ >= Map.GOAL1_GLOBAL_POS[2].x_ + 11f)
				{
					flag11 = true;
				}
				if ((old_pos.z_ <= 900f || old_pos.z_ >= -900f) && ((!flag10 && !flag11 && pos_.y_ < 21f) || (flag10 && pos_.y_ >= 21f && pos_.y_ < 32f)) && udat_.gm_.state_ == 0)
				{
					flag8 = true;
					if (pos_.z_ > 0f)
					{
						pos_.z_ = 900f + (900f - pos_.z_);
						pos_.z_ = AppData.ClampMax((int)pos_.z_, 899);
					}
					else
					{
						pos_.z_ = -900f + (-900f - pos_.z_);
						pos_.z_ = AppData.ClampMin((int)pos_.z_, -899);
					}
					v_.x_ = v_.x_ * 25f / 100f;
					v_.z_ = (0f - v_.z_) * 25f / 100f;
					ChGravity();
					SetSavePos(pos_, v_, a_);
					targetPId = -1;
					flag_ = BitUtil.Off(flag_, 96);
					apdat_.PlaySe(6);
				}
			}
			if (flag9)
			{
				v_.y_ *= 0.9f;
				a_.y_ *= 0.9f;
				if (a_.y_ > -0.1f)
				{
					a_.y_ = -0.1f;
				}
			}
			bool flag13 = false;
			if (udat_.gm_.state_ == 0 && SoccerForm.DEBUG_BALLTEST_MODE == 0)
			{
				if (pos_.x_ < -450f)
				{
					if (SoccerForm.DEBUG_WALLREFRECT == 1)
					{
						pos_.x_ = -450f + (-450f - pos_.x_);
						v_.x_ *= -1f;
					}
					else
					{
						lineOutPos_.Set(pos_);
					}
					flag13 = true;
				}
				if (pos_.x_ > 450f)
				{
					if (SoccerForm.DEBUG_WALLREFRECT == 1)
					{
						pos_.x_ = 450f + (450f - pos_.x_);
						v_.x_ *= -1f;
					}
					else
					{
						lineOutPos_.Set(pos_);
					}
					flag13 = true;
				}
				if (pos_.z_ > 900f)
				{
					if (SoccerForm.DEBUG_WALLREFRECT == 1)
					{
						pos_.z_ = 900f + (900f - pos_.z_);
						v_.z_ *= -1f;
					}
					else
					{
						lineOutPos_.Set(pos_);
					}
					flag13 = true;
				}
				if (pos_.z_ < -900f)
				{
					if (SoccerForm.DEBUG_WALLREFRECT == 1)
					{
						pos_.z_ = -900f + (-900f - pos_.z_);
						v_.z_ *= -1f;
					}
					else
					{
						lineOutPos_.Set(pos_);
					}
					flag13 = true;
				}
				if (flag13 && !flag8 && SoccerForm.DEBUG_BALLTEST_MODE != 1)
				{
					if (SoccerForm.DEBUG_WALLREFRECT == 1)
					{
						ChGravity();
						SetSavePos(pos_, v_, a_);
						targetPId = -1;
					}
					else if (lineOutPos_.z_ < -900f || lineOutPos_.z_ > 900f)
					{
						int num3 = -1;
						if (lineOutPos_.z_ > 900f)
						{
							num3 = 1;
						}
						int num4 = -1;
						if (lastOwner_ != null && lastOwner_.team_id_ == 1)
						{
							num4 = 1;
						}
						if (num3 * num4 == -1)
						{
							udat_.gm_.ChangeState(4);
						}
						else
						{
							udat_.gm_.ChangeState(5);
						}
					}
					else
					{
						udat_.gm_.ChangeState(3);
					}
				}
			}
			Camera.ConvertGlobalToAddress(pos_, address_);
			Camera.ConvertGlobalToScreen(pos_, screen_pos_);
			shadowPos_.x_ = pos_.x_;
			shadowPos_.z_ = pos_.z_;
			Camera.ConvertGlobalToScreen(shadowPos_, screen_shadowPos_);
			return false;
		}

		public void ChGravity()
		{
			if (a_.y_ != 0f && pos_.y_ > 0f && a_.y_ >= MIN_AY)
			{
				a_.y_ = MIN_AY;
			}
		}

		public void ChangeState(int next_state)
		{
			state_ = next_state;
			stateCnt_ = 0;
			frame_ = 0;
		}

		public void Draw(Graphics g)
		{
			if (BitUtil.Check(flag_, 1))
			{
				return;
			}
			Camera.ConvertGlobalToCamera(pos_, camera_pos_);
			Camera.ConvertGlobalToCamera(shadowPos_, camera_pos_shadow);
			int num = camera_pos_shadow[1];
			bool dark = false;
			int num2 = flag_;
			int frame = frame_ % 12 / 6;
			if (owner_ != null && udat_.gm_.state_ == 0)
			{
				pos_.Set(owner_.pos_);
				direct_ = owner_.direct_;
				float num3 = 50f;
				float x = (float)JMath.Sqrt(owner_.v_.x_ * owner_.v_.x_ + owner_.v_.z_ * owner_.v_.z_);
				num3 = GameUtil.RateConvert(x, 0f, 10f, 0f, 100f, true);
				int c2min = (int)GameUtil.RateConvert(num3, 0f, 100f, DRIBBLE_DIRECT_DIS[direct_][0][0], DRIBBLE_DIRECT_DIS[direct_][1][0], true);
				int c2max = (int)GameUtil.RateConvert(num3, 0f, 100f, DRIBBLE_DIRECT_DIS[direct_][2][0], DRIBBLE_DIRECT_DIS[direct_][3][0], true);
				int c2min2 = (int)GameUtil.RateConvert(num3, 0f, 100f, DRIBBLE_DIRECT_DIS[direct_][0][1], DRIBBLE_DIRECT_DIS[direct_][1][1], true);
				int c2max2 = (int)GameUtil.RateConvert(num3, 0f, 100f, DRIBBLE_DIRECT_DIS[direct_][2][1], DRIBBLE_DIRECT_DIS[direct_][3][1], true);
				int num4 = owner_.ballDribbleTime_ % 16;
				int num5 = GameUtil.RateConvert(JMath.Abs(num4 - 8), 8, 0, c2min, c2max, true);
				int num6 = GameUtil.RateConvert(JMath.Abs(num4 - 8), 8, 0, c2min2, c2max2, true);
				float num7 = (float)JMath.Sqrt((camera_pos_dribble[0] - num5) * (camera_pos_dribble[0] - num5) + (camera_pos_dribble[1] - num6) * (camera_pos_dribble[1] - num6));
				int num8 = num5;
				int num9 = num6;
				camera_pos_dribble[0] = num8;
				camera_pos_dribble[1] = num9;
				camera_pos_[0] += num8;
				camera_pos_[1] += num9;
				camera_pos_shadow[0] += num8;
				camera_pos_shadow[1] += num9;
				if (rotate_ == 0 || num4 == 0)
				{
					rotate_ = (int)GameUtil.RateConvert(num3, 0f, 100f, 8f, 2f, true);
				}
				frame = frame_ % (rotate_ * 4) / rotate_;
				num = camera_pos_[1];
			}
			if (udat_.gm_.state_ == 1 || udat_.gm_.state_ == 2 || udat_.gm_.state_ == 3 || udat_.gm_.state_ == 4 || udat_.gm_.state_ == 5)
			{
				frame = 0;
			}
			if (udat_.gm_.state_ == 3 && owner_ != null)
			{
				camera_pos_[0] += THROWIN_OFF[owner_.direct_][0];
				camera_pos_[1] += THROWIN_OFF[owner_.direct_][1];
				num += 3;
			}
			global::System.Single originalUnusedSlot16;

int num10 = 0;
			int num11 = 0;
			if (owner_ != null)
			{
				if (owner_.seb_ == 14) {
					if (owner_.team_id_ == 0) { num = (num + -1); } else { num = (num + 1); }
					int num12 = 0;
					if (owner_.team_id_ == 0)
					{
						if (owner_.direct_ == 3)
						{
							num12 = 0;
						}
						else if (owner_.direct_ == 0)
						{
							num12 = 1;
						}
						else if (owner_.direct_ == 1)
						{
							num12 = 2;
						}
					}
					else if (owner_.direct_ == 3)
					{
						num12 = 0;
					}
					else if (owner_.direct_ == 2)
					{
						num12 = 1;
					}
					else if (owner_.direct_ == 1)
					{
						num12 = 2;
					}
					num10 = SAVING_OFF[owner_.team_id_][num12][0];
					num11 = SAVING_OFF[owner_.team_id_][num12][1];
				} else {
					if (((owner_.direct_ == 0) || (owner_.direct_ == 3))) { num = ((num - 1)); } else { num = ((num + 1)); }
				}
			}
			if (udat_.gm_.state_ != 3 || owner_ == null)
			{
				apdat_.InsertGameObject(8, 0, num, 2, 0, 4, camera_pos_shadow[0], camera_pos_shadow[1] + 1, dark, 0);
			}
			apdat_.InsertGameObject(9, 0, num, 2, 0, frame, camera_pos_[0] + num10, camera_pos_[1] + num11, dark, 0);
		}

		public void ReceiveBall(Player player)
		{
			ReceiveBall(player, false);
		}

		public void ReceiveBall(Player player, bool coatOut_ok)
		{
			if ((udat_.gm_.state_ != 0 && coatOut_ok) || (!(pos_.z_ >= 900f) && !(pos_.z_ <= -900f) && !(pos_.x_ <= -450f) && !(pos_.x_ >= 450f) && !(player.pos_.z_ >= 900f) && !(player.pos_.z_ <= -900f) && !(player.pos_.x_ <= -450f) && !(player.pos_.x_ >= 450f)))
			{
				flag_ = BitUtil.Off(flag_, 1);
				a_.x_ = 0f;
				a_.z_ = 0f;
				player.flag_ = BitUtil.On(player.flag_, 4);
				player.ballHaveTime_ = 0;
				player.ballDribbleTime_ = 0;
				flag_ = BitUtil.On(flag_, 4);
				owner_ = player;
				if (lastOwner_ != null && lastOwner_.team_id_ == owner_.team_id_ && lastOwner_.pData_id_ >= 0)
				{
					apdat_.pData_[lastOwner_.pData_id_].SetActScore(6);
				}
				if (player.pData_id_ >= 0)
				{
					apdat_.pData_[player.pData_id_].SetActScore(7);
				}
				receive_wait = 0;
				flag_ = BitUtil.On(flag_, 8);
				boundNum_ = 0;
				flag_ = BitUtil.Off(flag_, 112);
				targetPId = -1;
				udat_.gm_.SetBallTeam(player.team_id_);
				if (lastOwner_ == null)
				{
					udat_.teams_[player.team_id_].Amefto_start();
				}
				else if (owner_.team_id_ != lastOwner_.team_id_)
				{
					udat_.teams_[player.team_id_].Amefto_start();
				}
			}
		}

		public void ReleaseBall(Player player)
		{
			if (!(BitUtil.Check(flag_, 4))) { return; }
{
player.flag_ = BitUtil.Off(player.flag_, 4);
player.ballHaveTime_ = 0;
flag_ = BitUtil.Off(flag_, 4);
lastOwner_ = owner_;
owner_ = null;
receive_wait = RECEIVE_WAIT_MIN;
flag_ = BitUtil.Off(flag_, 8);
boundNum_ = 0;
udat_.teams_[player.team_id_].lastKickeer_ = player.p_id_;
udat_.gm_.SetBallTeam(-1);
}
		}

		public void SetPassData(Player player, int pass_mode, int data1, int data2, bool jamp)
		{
			v_.Set(0f, 0f, 0f);
			a_.Set(0f, 0f, 0f);
			switch (pass_mode)
			{
			case 0:
			case 5:
			{
				Player player2 = udat_.teams_[player.team_id_].players_[data1];
				temp_pos.Set(player2.pos_);
				break;
			}
			case 1:
				temp_pos.Set(Camera.GetPosFromAddress(data1, data2));
				temp_pos.x_ += 900f / (float)Map.MAPCHIP_COAT_WNUM * 1f / 2f;
				temp_pos.z_ += 1800f / (float)Map.MAPCHIP_COAT_HNUM * 1f / 2f;
				break;
			case 2:
				temp_pos.x_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, -337.5f, 337.5f, true);
				temp_pos.y_ = 0f;
				if (player.team_id_ == 0)
				{
					temp_pos.z_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 0f, 300f, true);
				}
				else
				{
					temp_pos.z_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 0f, -300f, true);
				}
				break;
			case 3:
			{
				float num = 2.1474836E+09f;
				int num2 = 0;
				for (int i = 1; i < udat_.teams_[player.team_id_].players_.Length; i++)
				{
					if (i != player.p_id_)
					{
						Player player3 = udat_.teams_[player.team_id_].players_[i];
						float num3 = 0f;
						num3 = ((player3.team_id_ != 0) ? (player3.pos_.x_ * player3.pos_.x_ + (player3.pos_.z_ - -900f) * (player3.pos_.z_ - -900f)) : (player3.pos_.x_ * player3.pos_.x_ + (player3.pos_.z_ - 900f) * (player3.pos_.z_ - 900f)));
						if (player3.pos_.x_ * player.pos_.x_ > 0f)
						{
							num += 180f;
						}
						if (num3 < num)
						{
							num = num3;
							num2 = i;
						}
					}
				}
				data1 = num2;
				Player player4 = udat_.teams_[player.team_id_].players_[num2];
				temp_pos.x_ = player4.pos_.x_;
				if (temp_pos.x_ < -128.57143f)
				{
					temp_pos.x_ = -128.57143f;
				}
				if (temp_pos.x_ > 128.57143f)
				{
					temp_pos.x_ = 128.57143f;
				}
				temp_pos.z_ = player4.pos_.z_;
				if (player.team_id_ == 0)
				{
					if (temp_pos.z_ < 720f)
					{
						temp_pos.z_ = 720f;
					}
					if (temp_pos.z_ > 846f)
					{
						temp_pos.z_ = 846f;
					}
				}
				else
				{
					if (temp_pos.z_ < -846f)
					{
						temp_pos.z_ = -846f;
					}
					if (temp_pos.z_ > -720f)
					{
						temp_pos.z_ = -720f;
					}
				}
				break;
			}
			case 4:
				temp_pos.y_ = 0f;
				if (player.team_id_ == 0)
				{
					temp_pos.x_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, Map.COAT_GOAL2_GPOS_CAREA[0].x_, Map.COAT_GOAL2_GPOS_CAREA[1].x_, true);
					temp_pos.z_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, Map.COAT_GOAL2_GPOS_CAREA[0].z_, Map.COAT_GOAL2_GPOS_CAREA[1].z_, true);
				}
				else
				{
					temp_pos.x_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, Map.COAT_GOAL1_GPOS_CAREA[0].x_, Map.COAT_GOAL1_GPOS_CAREA[1].x_, true);
					temp_pos.z_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, Map.COAT_GOAL1_GPOS_CAREA[0].z_, Map.COAT_GOAL1_GPOS_CAREA[1].z_, true);
				}
				break;
			}
			int num4 = 2;
			float num5 = 1f;
			float num6 = 1f;
			float num7 = (float)JMath.Sqrt((temp_pos.x_ - pos_.x_) * (temp_pos.x_ - pos_.x_) + (temp_pos.z_ - pos_.z_) * (temp_pos.z_ - pos_.z_));
			if (num7 < 0.001f)
			{
				return;
			}
			if (pass_mode == 5)
			{
				float num8 = num7 / 400f;
				int num9 = (int)GameUtil.RateConvert(num7, 100f, 400f, 8f, 30f, true);
				a_.y_ = -0.48000002f;
				v_.y_ = -20f / (float)num9 - a_.y_ * (float)(num9 + 1) / 2f;
				v_.x_ = (temp_pos.x_ - pos_.x_) / (float)num9;
				v_.z_ = (temp_pos.z_ - pos_.z_) / (float)num9;
				if (num8 >= 1f)
				{
					v_.x_ /= num8;
					v_.z_ /= num8;
				}
			}
			else
			{
				if (pass_mode == 3 || pass_mode == 4)
				{
					num5 = PlayerData.GetLongPassSpeed(player);
					num6 = -0.25f;
				}
				else if (num7 > 594f)
				{
					num5 = PlayerData.GetLongPassSpeed(player);
					num6 = -0.35f;
				}
				else if (pass_mode == 1)
				{
					num5 = PlayerData.GetThroughPassSpeed(player);
					num6 = -0.6f;
				}
				else
				{
					num5 = PlayerData.GetPassSpeed(player);
					num6 = -0.6f;
				}
				num5 = PlayerData.AuraUpdate(player, num5, 1);
				int num10 = 0;
				float num11 = 0f;
				int num12 = 0;
				int num13 = 0;
				if (num6 != -0.25f && num6 != -0.35f)
				{
					num10 = (int)JMath.Abs(num5 * GameUtil.MAX_ANGLEDATA_SIN / num6) * 2;
					num11 = num5 * GameUtil.MAX_ANGLEDATA_COS * (float)num10;
					num13 = GameUtil.MAX_ANGLE;
				}
				else
				{
					num10 = (int)JMath.Abs(num5 * GameUtil.MAX_ANGLEDATA_SIN2 / num6) * 2;
					num11 = num5 * GameUtil.MAX_ANGLEDATA_COS2 * (float)num10;
					num13 = GameUtil.MAX_ANGLE2;
				}
				if (num11 < 0.001f)
				{
					num11 = 0.001f;
				}
				if (num10 < 5)
				{
					jamp = false;
				}
				if (jamp)
				{
					if (pass_mode == 3 || pass_mode == 4)
					{
						v_.y_ = GameUtil.SetVStop(68f, 18);
						a_.y_ = GameUtil.SetAStop(68f, 18);
						v_.x_ = GameUtil.SetVdecreV(0.2f, temp_pos.x_ - pos_.x_, 36);
						a_.x_ = GameUtil.SetAdecreV(0.2f, temp_pos.x_ - pos_.x_, 36);
						v_.z_ = GameUtil.SetVdecreV(0.2f, temp_pos.z_ - pos_.z_, 36);
						a_.z_ = GameUtil.SetAdecreV(0.2f, temp_pos.z_ - pos_.z_, 36);
					}
					else
					{
						if (num6 == -0.25f || num6 == -0.35f)
						{
							temp_pos.x_ = pos_.x_ + (temp_pos.x_ - pos_.x_) * 1.27f;
							temp_pos.z_ = pos_.z_ + (temp_pos.z_ - pos_.z_) * 1.27f;
						}
						else if (num6 == -0.6f)
						{
							temp_pos.x_ = pos_.x_ + (temp_pos.x_ - pos_.x_) * 1.12f;
							temp_pos.z_ = pos_.z_ + (temp_pos.z_ - pos_.z_) * 1.12f;
						}
						if (num11 < num7)
						{
							num12 = num13;
						}
						else
						{
							float x = num7 / num11;
							num12 = (int)GameUtil.RateConvert(x, 0.1f, 1f, 12f, num13, true);
						}
						v_.x_ = num5 * GameUtil.Cos(num12) * (temp_pos.x_ - pos_.x_) / num7;
						v_.z_ = num5 * GameUtil.Cos(num12) * (temp_pos.z_ - pos_.z_) / num7;
						v_.y_ = num5 * GameUtil.Sin(num12);
						a_.y_ = num6;
					}
				}
				else
				{
					v_.x_ = num5 * (temp_pos.x_ - pos_.x_) / num7;
					v_.y_ = 0f;
					v_.z_ = num5 * (temp_pos.z_ - pos_.z_) / num7;
				}
			}
			receive_wait = RECEIVE_WAIT_MIN;
			if (jamp)
			{
				flag_ = BitUtil.On(flag_, 16);
			}
			if (Config.DEBUG_CMD && SoccerForm.DEBUG_3KEY_KICKTEST == 1 && canvas_.CheckKeyState(8))
			{
				v_.x_ *= 10f;
			}
			switch (pass_mode)
			{
			case 0:
			case 5:
			{
				int num15 = (targetPId = data1);
				for (int k = 0; k < udat_.teams_[player.team_id_].receiverPIdBox_.Length; k++)
				{
					udat_.teams_[player.team_id_].receiverPIdBox_[k] = num15;
				}
				break;
			}
			case 1:
			{
				int num16 = (targetPId = player.p_id_);
				for (int l = 0; l < udat_.teams_[player.team_id_].receiverPIdBox_.Length; l++)
				{
					udat_.teams_[player.team_id_].receiverPIdBox_[l] = num16;
				}
				break;
			}
			case 3:
			{
				flag_ = BitUtil.On(flag_, 64);
				int num14 = (targetPId = data1);
				for (int j = 0; j < udat_.teams_[player.team_id_].receiverPIdBox_.Length; j++)
				{
					udat_.teams_[player.team_id_].receiverPIdBox_[j] = num14;
				}
				break;
			}
			case 4:
				flag_ = BitUtil.On(flag_, 64);
				break;
			}
			SetSavePos(pos_, v_, a_);
			if (SOUND_RANDOM == 1)
			{
				apdat_.PlaySe(SOUND_KICK[GameUtil.Random(SOUND_KICK.Length)]);
			}
			else
			{
				apdat_.PlaySe(SOUND_PASS[pass_mode]);
			}
		}

		public void SetShoot(Player player, int shoot_mode)
		{
			v_.Set(0f, 0f, 0f);
			a_.Set(0f, 0f, 0f);
			Vector3D[] array = null;
			if (player.team_id_ == 0) { array = Map.GOAL2_GLOBAL_POS; } else { array = Map.GOAL1_GLOBAL_POS; }
			GameUtil.Sub(array[0], pos_, vec1);
			GameUtil.Sub(array[1], pos_, vec2);
			vec3.x_ = pos_.x_;
			vec3.z_ = array[0].z_;
			GameUtil.Sub(vec3, pos_, vec3);
			Player player2 = udat_.teams_[1 - player.team_id_].players_[0];
			GameUtil.Sub(player2.pos_, pos_, vec_GK);
			float num = (float)JMath.Sqrt(vec1.x_ * vec1.x_ + vec1.z_ * vec1.z_);
			float num2 = (float)JMath.Sqrt(vec2.x_ * vec2.x_ + vec2.z_ * vec2.z_);
			float num3 = (float)JMath.Sqrt(vec3.x_ * vec3.x_ + vec3.z_ * vec3.z_);
			float num4 = (float)JMath.Sqrt(vec_GK.x_ * vec_GK.x_ + vec_GK.z_ * vec_GK.z_);
			if (num3 > num4)
			{
				float num5 = 0f;
				if (num * num3 != 0f)
				{
					num5 = (vec1.x_ * vec3.x_ + vec1.z_ * vec3.z_) / (num * num3);
				}
				float num6 = 0f;
				if (num2 * num3 != 0f)
				{
					num6 = (vec2.x_ * vec3.x_ + vec2.z_ * vec3.z_) / (num2 * num3);
				}
				rand_cos[0] = (int)(num5 * 100f);
				rand_cos[1] = rand_cos[0] + (int)(num6 * 100f);
				rand_cos[2] = rand_cos[1] + 30;
				int num7 = 0;
				int num8 = GameUtil.Random(rand_cos[2]);
				num7 = ((num8 > rand_cos[1]) ? 2 : ((num8 > rand_cos[0]) ? 1 : 0));
				float num9 = 0f;
				float num10 = 0f;
				switch (num7)
				{
				case 0:
					num9 = array[0].x_;
					break;
				case 1:
					num9 = array[1].x_;
					break;
				default:
					num9 = 0f;
					break;
				}
				num10 = GameUtil.RateConvert(GameUtil.Random(130) - 30, 0f, 100f, array[0].x_, array[1].x_, false);
				float num11 = 0.7f;
				float num12 = 1f - num11;
				g1.x_ = (num11 * num9 + num12 * num10) / (num11 + num12);
				g1.z_ = array[0].z_;
			}
			else
			{
				float num13 = array[1].x_ - array[0].x_;
				float num14 = GameUtil.RateConvert(pos_.x_, array[0].x_, array[1].x_, 0f - num13 / 4f, 0f + num13 / 4f, true);
				g1.x_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, (num14 + array[0].x_) / 2f, (num14 + array[1].x_) / 2f, true);
				g1.z_ = array[0].z_;
			}
			switch (shoot_mode)
			{
			case 0:
			{
				int playerBodyPoseEva = PlayerData.GetPlayerBodyPoseEva(player, 10);
				float shootSpeed = PlayerData.GetShootSpeed(player, playerBodyPoseEva);
				shootSpeed = PlayerData.AuraUpdate(player, shootSpeed, 0);
				float num17 = (float)JMath.Sqrt((g1.x_ - pos_.x_) * (g1.x_ - pos_.x_) + (g1.z_ - pos_.z_) * (g1.z_ - pos_.z_));
				v_.x_ = shootSpeed * (g1.x_ - pos_.x_) / num17;
				v_.z_ = shootSpeed * (g1.z_ - pos_.z_) / num17;
				int eva = playerBodyPoseEva;
				float shootV0y2 = PlayerData.GetShootV0y(player, eva);
				v_.y_ = shootV0y2;
				a_.y_ = -0.1f;
				break;
			}
			case 1:
			{
				float value = PlayerData.GetShootSpeed(player, 50) * 80f / 100f;
				value = PlayerData.AuraUpdate(player, value, 0);
				float num15 = (float)JMath.Sqrt((g1.x_ - pos_.x_) * (g1.x_ - pos_.x_) + (g1.z_ - pos_.z_) * (g1.z_ - pos_.z_));
				v_.x_ = value * (g1.x_ - pos_.x_) / num15;
				v_.z_ = value * (g1.z_ - pos_.z_) / num15;
				float shootV0y = PlayerData.GetShootV0y(player, 50);
				int headShootUpRadRate = PlayerData.GetHeadShootUpRadRate(player);
				int num16 = GameUtil.Random(100);
				float x = (float)JMath.Sqrt((g1.z_ - pos_.z_) * (g1.z_ - pos_.z_));
				num16 += (int)GameUtil.RateConvert(x, 200f, 60f, -20f, -70f, true);
				v_.y_ = GameUtil.RateConvert(num16, 0f, 99f, (0f - shootV0y) * 350f / 100f, shootV0y * (float)headShootUpRadRate / 100f, true);
				a_.y_ = -0.6f;
				break;
			}
			}
			shoot_goalPos.Set(g1);
			flag_ = BitUtil.On(flag_, 48);
			SetSavePos(pos_, v_, a_);
			switch (shoot_mode)
			{
			case 0:
				apdat_.PlaySe(SOUND_SHOOT_KICK);
				break;
			case 1:
				apdat_.PlaySe(SOUND_SHOOT_HEAD);
				break;
			}
		}

		public void SetHeadingData(Player player)
		{
			float num = 0f;
			if (player.team_id_ == 0) { num = ((float)JMath.Sqrt(player.pos_.x_ * player.pos_.x_ + (player.pos_.z_ - 900f) * (player.pos_.z_ - 900f))); } else { num = ((float)JMath.Sqrt(player.pos_.x_ * player.pos_.x_ + (player.pos_.z_ - -900f) * (player.pos_.z_ - -900f))); }
			if (num < 360f)
			{
				SetShoot(player, 1);
				apdat_.AddEffectShoot((player.screen_pos_[0] + screen_pos_[0]) / 2, (player.screen_pos_[1] + screen_pos_[1]) / 2);
			}
			else
			{
				float num2 = JMath.Abs(v_.x_ * v_.x_ + v_.y_ * v_.y_ + v_.z_ * v_.z_) / 3f;
				if (num2 > 15f)
				{
					num2 = 15f;
				}
				a_.y_ = -0.6f;
				int num3 = GameUtil.Random(80) + 50;
				int angle = GameUtil.Random(30) + 10;
				float num4 = num2 * GameUtil.Cos(angle);
				if (player.team_id_ == 0)
				{
					v_.x_ = num4 * GameUtil.Cos(num3);
					v_.y_ = num2 * GameUtil.Sin(angle);
					v_.z_ = num4 * GameUtil.Sin(num3);
				}
				else
				{
					num3 = -num3 + 360;
					v_.x_ = num4 * GameUtil.Cos(num3);
					v_.y_ = num2 * GameUtil.Sin(angle);
					v_.z_ = num4 * GameUtil.Sin(num3);
				}
				SetSavePos(pos_, v_, a_);
			}
			flag_ = BitUtil.On(flag_, 16);
			if (SOUND_RANDOM == 1)
			{
				apdat_.PlaySe(SOUND_KICK[GameUtil.Random(SOUND_KICK.Length)]);
			}
			else
			{
				apdat_.PlaySe(SOUND_HEAD);
			}
		}

		public bool ChBallDirectHere(Player player)
		{
			if (player.team_id_ == 0)
			{
				if (pos_.y_ > 0f && v_.z_ < 0f)
				{
					return true;
				}
			}
			else if (pos_.y_ > 0f && v_.z_ > 0f)
			{
				return true;
			}
			return false;
		}

		public virtual int GetToGoalLineTime(Player player)
		{
			if (v_.z_ == 0f)
			{
				return -1;
			}
			if (player.team_id_ == 0)
			{
				return (int)((Map.GOAL1_GLOBAL_POS[0].z_ - pos_.z_) / v_.z_);
			}
			return (int)((Map.GOAL2_GLOBAL_POS[0].z_ - pos_.z_) / v_.z_);
		}

		public void SetSavePos(Vector3D init_pos, Vector3D init_v, Vector3D init_a)
		{
			int num = 0;
			savePos_[0].Set(init_pos);
			saveV_.Set(init_v);
			saveA_.y_ = init_a.y_;
			float num2 = init_pos.y_;
			float num3 = init_v.y_;
			float y_ = init_a.y_;
			int num4 = 0;
			if (init_a.y_ == 0f || saveV_.y_ == 0f)
			{
				num4 = 100;
			}
			else
			{
				while (num4 < 300)
				{
					num4++;
					num3 += y_;
					num2 += num3;
					if (num2 < 0f)
					{
						break;
					}
				}
			}
			int num5 = 0;
			if ((num2 >= 300f)) { num5 = JMath.Abs((int)(init_v.y_ / init_a.y_) * 2); } else { num5 = num4; }
			int num6 = num5 * 100 / 100;
			int num7 = num5 * 100 / 100;
			bool flag = BitUtil.Check(flag_, 64);
			for (int i = 1; i < savePos_.Length; i++)
			{
				if (flag)
				{
					saveV_.x_ += init_a.x_;
					saveV_.y_ += init_a.y_;
					saveV_.z_ += init_a.z_;
					savePos_[i].x_ = savePos_[i - 1].x_ + saveV_.x_;
					savePos_[i].y_ = savePos_[i - 1].y_ + saveV_.y_;
					savePos_[i].z_ = savePos_[i - 1].z_ + saveV_.z_;
				}
				else
				{
					if (init_a.y_ == 0f || saveV_.y_ == 0f)
					{
						float num8 = GameUtil.RateConvert(i, 0f, 60f, 0.99f, 0.93f, true);
						float num9 = GameUtil.RateConvert(i, 0f, 60f, 0.99f, 0.93f, true);
						saveV_.x_ *= num8;
						saveV_.z_ *= num9;
					}
					else
					{
						float num8;
						float num9;
						if (init_a.y_ == -0.25f || init_a.y_ == -0.35f)
						{
							num8 = GameUtil.RateConvert(num, 0f, num7, 0.995f, 0.96f, true);
							num9 = GameUtil.RateConvert(num, 0f, num7, 0.995f, 0.96f, true);
						}
						else
						{
							num8 = GameUtil.RateConvert(num, 0f, num6, 0.995f, 0.97f, true);
							num9 = GameUtil.RateConvert(num, 0f, num6, 0.995f, 0.97f, true);
						}
						saveV_.x_ *= num8;
						saveV_.y_ *= 0.995f;
						saveV_.z_ *= num9;
					}
					saveV_.y_ += saveA_.y_;
					savePos_[i].x_ = savePos_[i - 1].x_ + saveV_.x_;
					savePos_[i].y_ = savePos_[i - 1].y_ + saveV_.y_;
					savePos_[i].z_ = savePos_[i - 1].z_ + saveV_.z_;
				}
				num++;
				if (savePos_[i].y_ <= 0f)
				{
					fallPos_.Set(savePos_[i]);
					fallTime_ = i;
					break;
				}
			}
			receiverPos_.x_ = fallPos_.x_;
			receiverPos_.z_ = fallPos_.z_;
			int num10 = -1;
			for (int num11 = fallTime_ - 1; num11 >= fallTime_ / 2; num11--)
			{
				if (savePos_[num11].y_ <= 40f)
				{
					num10 = num11;
				}
				else if (num10 != -1)
				{
					break;
				}
			}
			if (num10 != -1)
			{
				receiverPos_.Set(savePos_[num10]);
			}
			frameNum_ = num;
			savePosFrame_ = 0;
		}

		public int GetHeadIndexFormSavePos2(int headingStartFrame, float headDisTop)
		{
			int num = headingStartFrame + savePosFrame_;
			if (num >= frameNum_)
			{
				return -1;
			}
			if (savePos_[num].y_ >= headDisTop - 5f && savePos_[num].y_ <= headDisTop)
			{
				return num;
			}
			return -1;
		}

		public void SetStop()
		{
			v_.Set(0f, 0f, 0f);
			a_.Set(0f, 0f, 0f);
			flag_ = BitUtil.Off(flag_, 112);
			targetPId = -1;
		}
	}
}
