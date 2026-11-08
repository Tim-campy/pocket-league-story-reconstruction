using System;
using data;
using form;
using java.io;
using java.lang;
using java.util;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace game
{
	public class TPlayer : Player
	{
		public new const int SCR_MOVE = 0;

		public new const int SCR_WAIT = 1;

		public new const int SCR_CHANGE_STATE = 2;

		public new const int SCR_CHANGE_SEB = 3;

		public new const int SCR_CHANGE_DIRECT = 4;

		public new const int SCR_FLAG = 5;

		public new const int SCR_FLAG_ON = 6;

		public new const int SCR_FLAG_OFF = 7;

		public new const int SCR_KICK = 8;

		public new const int SCR_SAVING = 9;

		public new const int SCR_SLIDING = 10;

		public new const int SCR_HEAD = 11;

		public new const int SCR_VOLLEY = 12;

		public new const int SCR_THROWIN = 13;

		public new const int SCR_GOAL_GLAD = 14;

		public new const int SCR_GOAL_SAD = 15;

		public new const int SCR_DOWN = 16;

		public const int SCR_TRA_MOVEGOALPOS = 17;

		public const int SCR_TRA_MOVEGOALPOS_SET = 18;

		public const int SCR_TRA_BALLPASS = 19;

		public const int SCR_TRA_BALLWAIT = 20;

		public const int SCR_TRA_BALLCHASE = 21;

		public const int SCR_TRA_LIFTING = 22;

		public const int SCR_TRA_LIFTING_BALLCHASE = 23;

		public const int SCR_TRA_DRIBBLE = 24;

		public const int SCR_TRA_DRIBBLE_OLDER = 25;

		public const int SCR_TRA_DRIBBLE_OLDERREGIST = 26;

		public const int SCR_TRA_SHOOT = 27;

		public const int SCR_TRA_SHOOT_OLDER = 28;

		public const int SCR_TRA_SHOOT_OLDERREGIST = 29;

		public const int SCR_TRA_SHOOT_GK = 30;

		public const int SCR_TRA_SHOOT_SAVING = 31;

		public const int SCR_TRA_RUNNING_REGIST = 32;

		public const int SCR_TRA_RUNNING = 33;

		public const int SCR_TRA_GET_TPOINT = 34;

		public const int SCR_TRA_TENANT_IN = 35;

		public const int SCR_TRA_SVISOR_ACT = 36;

		public const int TID_ID = 0;

		public const int TID_AREA = 1;

		public const int TID_ROOT = 2;

		public const int TID_GROUPAREA = 3;

		public const int TID_GROUP = 4;

		public const int TID_MAX = 5;

		public const int LIFT_LEFTKICK = 0;

		public const int LIFT_RIGHTKICK = 1;

		public const int LIFT_HEAD = 2;

		public const int LIFT_END = 3;

		public const int LIFT_FAIL = -1;

		public const float LIFT_AY = -0.45f;

		public const int CORN_NUM = 4;

		public const float CORN_LEN_X = 20f;

		public const float CORN_LEN_Z = 40f;

		public const float GOAL_W = 80f;

		public const float GOAL_H = 24f;

		public const int TRAININT_PRIORITY_STAMEM = 40;

		public const int TRAININT_PRIORITY_SUB = 100;

		public const int GB_TYPE_PASS = 0;

		public const int GB_TYPE_PASS_WAIT = 1;

		public const int GB_TYPE_PASS_FAIL = 2;

		public const int GB_TYPE_LIFTING = 3;

		public const int GB_TYPE_LIFTING_FAIL = 4;

		public const int GB_TYPE_DRIBBLE_END = 5;

		public const int GB_TYPE_DRIBBLE_OLDER = 6;

		public const int GB_TYPE_SHOOT = 7;

		public const int GB_TYPE_SHOOT_OLDER = 8;

		public TGroupArea tGroupArea_;

		public int[] trainingId_;

		public int[] oldTrainingId_;

		public int useTenantCnt_;

		public int trainingPriority_;

		public static readonly int[][] LIFTING_TIME = new int[3][]
		{
			new int[3] { 20, 24, 16 },
			new int[3] { 24, 20, 16 },
			new int[3] { 16, 16, 10 }
		};

		public static readonly Vector3D[] LIFTING_POS = new Vector3D[3]
		{
			new Vector3D(-10f, 7f, 0f),
			new Vector3D(10f, 7f, 0f),
			new Vector3D(0f, 22f, 0f)
		};

		public int dcorn;

		public int dpoint;

		public static readonly Vector3D[][] CORN_AROUND_POS = new Vector3D[4][]
		{
			new Vector3D[2]
			{
				new Vector3D(-20f, 0f, 0f),
				new Vector3D(-20f, 0f, 0f)
			},
			new Vector3D[2]
			{
				new Vector3D(-20f, 0f, -40f),
				new Vector3D(-20f * (float)JMath.Cos(Math.PI / 4.0), 0f, -40f * (float)JMath.Sin(Math.PI / 4.0) / 2f)
			},
			new Vector3D[2]
			{
				new Vector3D(0f, 0f, -40f),
				new Vector3D(0f, 0f, -20f)
			},
			new Vector3D[2]
			{
				new Vector3D(20f, 0f, -40f),
				new Vector3D(20f * (float)JMath.Cos(Math.PI / 4.0), 0f, -40f * (float)JMath.Sin(Math.PI / 4.0) / 2f)
			}
		};

		public static int[] TEMP_TPOINT_UP = new int[5];

		private static int[] camera_pos_shadow = new int[2];

		private static int[] camera_pos_sliding = new int[2];

		public TPlayer(int obj_type)
			: base(obj_type)
		{
		}

		public void Init_training(int pId, int pDataId)
		{
			Player.apdat_ = AppData.GetInstance();
			Player.canvas_ = Canvas.GetInstance();
			Player.udat_ = UserData.GetInstance();
			pData_id_ = pDataId;
			trainingId_ = new int[5];
			trainingId_[0] = pId;
			oldTrainingId_ = new int[5];
			p_id_ = pId;
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
			role_ = Player.apdat_.pData_[pDataId].role_;
		}

		public void SetTrainingId(int rootId, int areaId, int groupAreaId, int groupId)
		{
			oldTrainingId_[2] = trainingId_[2];
			oldTrainingId_[1] = trainingId_[1];
			oldTrainingId_[3] = trainingId_[3];
			oldTrainingId_[4] = trainingId_[4];
			trainingId_[2] = rootId;
			trainingId_[1] = areaId;
			trainingId_[3] = groupAreaId;
			trainingId_[4] = groupId;
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)obj_type_);
			StreamUtil.WriteShort(os, (short)pData_id_);
			StreamUtil.WriteShort(os, (short)p_id_);
			StreamUtil.WriteShort(os, (short)team_id_);
			StreamUtil.WriteShort(os, (short)tData_id_);
			StreamUtil.WriteShort(os, (short)role_);
			StreamUtil.WriteInt(os, frame_);
			StreamUtil.WriteShort(os, (short)direct_);
			StreamUtil.WriteShort(os, (short)seb_);
			StreamUtil.WriteInt(os, sebCnt_);
			StreamUtil.WriteShort(os, (short)faceImg_);
			StreamUtil.WriteShort(os, (short)bodyImg_);
			StreamUtil.WriteShort(os, (short)script_.Size());
			for (int i = 0; i < script_.Size(); i++)
			{
				int[] array = (int[])script_.ElementAt(i);
				StreamUtil.WriteShort(os, (short)array.Length);
				for (int j = 0; j < array.Length; j++)
				{
					StreamUtil.WriteInt(os, array[j]);
				}
			}
			StreamUtil.WriteVector3D(os, pos_);
			StreamUtil.WriteVector3D(os, shadowPos_);
			StreamUtil.WriteVector3D(os, goalPos_);
			StreamUtil.WriteVector3D(os, v_);
			StreamUtil.WriteShort(os, (short)address_[0]);
			StreamUtil.WriteShort(os, (short)address_[1]);
			StreamUtil.WriteInt(os, screen_pos_[0]);
			StreamUtil.WriteInt(os, screen_pos_[1]);
			StreamUtil.WriteInt(os, flag_);
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteInt(os, stateCnt_);
			StreamUtil.WriteShort(os, (short)dcorn);
			StreamUtil.WriteShort(os, (short)dpoint);
			StreamUtil.WriteVector3D(os, seb_startPos);
			StreamUtil.WriteVector3D(os, seb_v);
			StreamUtil.WriteFloat(os, seb_ay);
			for (int k = 0; k < trainingId_.Length; k++)
			{
				StreamUtil.WriteInt(os, trainingId_[k]);
			}
			for (int l = 0; l < oldTrainingId_.Length; l++)
			{
				StreamUtil.WriteInt(os, oldTrainingId_[l]);
			}
			StreamUtil.WriteInt(os, useTenantCnt_);
			StreamUtil.WriteInt(os, trainingPriority_);
		}

		public void Deserialize(InputStream stream)
		{
			obj_type_ = StreamUtil.ReadShort(stream);
			pData_id_ = StreamUtil.ReadShort(stream);
			p_id_ = StreamUtil.ReadShort(stream);
			team_id_ = StreamUtil.ReadShort(stream);
			tData_id_ = StreamUtil.ReadShort(stream);
			role_ = StreamUtil.ReadShort(stream);
			frame_ = StreamUtil.ReadInt(stream);
			direct_ = StreamUtil.ReadShort(stream);
			seb_ = StreamUtil.ReadShort(stream);
			sebCnt_ = StreamUtil.ReadInt(stream);
			faceImg_ = StreamUtil.ReadShort(stream);
			bodyImg_ = StreamUtil.ReadShort(stream);
			script_ = new Vector();
			int num = StreamUtil.ReadShort(stream);
			for (int i = 0; i < num; i++)
			{
				int num2 = StreamUtil.ReadShort(stream);
				int[] array = new int[num2];
				for (int j = 0; j < num2; j++)
				{
					array[j] = StreamUtil.ReadInt(stream);
				}
				script_.AddElement(array);
			}
			pos_ = StreamUtil.ReadVector3D(stream);
			shadowPos_ = StreamUtil.ReadVector3D(stream);
			goalPos_ = StreamUtil.ReadVector3D(stream);
			v_ = StreamUtil.ReadVector3D(stream);
			address_ = new int[2];
			screen_pos_ = new int[2];
			address_[0] = StreamUtil.ReadShort(stream);
			address_[1] = StreamUtil.ReadShort(stream);
			screen_pos_[0] = StreamUtil.ReadInt(stream);
			screen_pos_[1] = StreamUtil.ReadInt(stream);
			flag_ = StreamUtil.ReadInt(stream);
			state_ = StreamUtil.ReadShort(stream);
			stateCnt_ = StreamUtil.ReadInt(stream);
			dcorn = StreamUtil.ReadShort(stream);
			dpoint = StreamUtil.ReadShort(stream);
			seb_startPos = StreamUtil.ReadVector3D(stream);
			seb_v = StreamUtil.ReadVector3D(stream);
			seb_ay = StreamUtil.ReadFloat(stream);
			trainingId_ = new int[5];
			oldTrainingId_ = new int[5];
			for (int k = 0; k < trainingId_.Length; k++)
			{
				trainingId_[k] = StreamUtil.ReadInt(stream);
			}
			for (int l = 0; l < oldTrainingId_.Length; l++)
			{
				oldTrainingId_[l] = StreamUtil.ReadInt(stream);
			}
			useTenantCnt_ = StreamUtil.ReadInt(stream);
			trainingPriority_ = StreamUtil.ReadInt(stream);
			PlayerData playerData = Player.apdat_.pData_[pData_id_];
			playerData.SetGameData(this);
		}

		public void Deserialize2()
		{
		}

		public void AddCommandTrainigMoveGoalPos()
		{
			AddCommand(new int[1] { 17 });
		}

		public void AddCommandTrainigMoveGoalPosSet(int x, int z)
		{
			AddCommand(new int[3] { 18, x, z });
		}

		public void AddCommandTrainingBallKick()
		{
			AddCommand(new int[1] { 19 });
		}

		public void AddCommandTrainingBallWait()
		{
			AddCommand(new int[1] { 20 });
		}

		public void AddCommandTrainingBallChase()
		{
			AddCommand(new int[1] { 21 });
		}

		public void AddCommandTenantIn()
		{
			AddCommand(new int[1] { 35 });
		}

		public void AddCommandSvisorAct()
		{
			AddCommand(new int[1] { 36 });
		}

		public void AddCommandLifting(int lift_id)
		{
			AddCommand(new int[2] { 22, lift_id });
		}

		public void AddCommandLiftingBallChase()
		{
			AddCommand(new int[1] { 23 });
		}

		public void AddCommandDribble()
		{
			AddCommand(new int[1] { 24 });
		}

		public void AddCommandDribbleOlder()
		{
			AddCommand(new int[1] { 25 });
		}

		public void AddCommandDribbleOlderRegist()
		{
			AddCommand(new int[1] { 26 });
		}

		public void AddCommandShoot()
		{
			AddCommand(new int[1] { 27 });
		}

		public void AddCommandShootOlder()
		{
			AddCommand(new int[1] { 28 });
		}

		public void AddCommandShootOlderRegist()
		{
			AddCommand(new int[1] { 29 });
		}

		public void AddCommandShootGK()
		{
			AddCommand(new int[1] { 30 });
		}

		public void AddCommandTraSaving()
		{
			AddCommand(new int[1] { 31 });
		}

		public void AddCommandRunningRegist()
		{
			AddCommand(new int[1] { 32 });
		}

		public void AddCommandRunning(int next_point)
		{
			AddCommand(new int[2] { 33, next_point });
		}

		public void AddCommandGetTPoint()
		{
			AddCommand(new int[1] { 34 });
		}

		public override bool Script()
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
if (sebCnt_ < Player.SEB_H_SAVING_ANIME[0])
						{
							pos_.x_ = seb_startPos.x_ + seb_v.x_ * (float)sebCnt_;
							pos_.z_ = seb_startPos.z_ + seb_v.z_ * (float)sebCnt_;
							pos_.y_ = seb_v.y_ * (float)sebCnt_ + seb_ay * (float)sebCnt_ * (float)(sebCnt_ + 1) / 2f;
						}
						else
						{
							pos_.x_ = seb_startPos.x_ + seb_v.x_ * (float)Player.SEB_H_SAVING_ANIME[0] + seb_v.x_ / 3f * (float)(sebCnt_ - Player.SEB_H_SAVING_ANIME[0]);
							pos_.z_ = seb_startPos.z_ + seb_v.z_ * (float)Player.SEB_H_SAVING_ANIME[0] + seb_v.z_ / 3f * (float)(sebCnt_ - Player.SEB_H_SAVING_ANIME[0]);
							pos_.y_ = seb_v.y_ * (float)sebCnt_ + seb_ay * (float)sebCnt_ * (float)(sebCnt_ + 1) / 2f;
							if (pos_.y_ < 0f)
							{
								pos_.y_ = 0f;
							}
						}
						if (sebCnt_ >= Player.SEB_H_SAVING_ANIME[Player.SEB_H_SAVING_ANIME.Length - 1])
						{
							flag_ = BitUtil.Off(flag_, 8);
							script_.RemoveElementAt(0);
							continue;
						}

}
else if ((recoveredScriptCommand == 10)) {
if (sebCnt_ < Player.SEB_H_SLIDING_ANIME[0])
						{
							pos_.x_ += seb_v.x_;
							pos_.z_ += seb_v.z_;
						}
						if (sebCnt_ >= Player.SEB_H_SLIDING_ANIME[Player.SEB_H_SLIDING_ANIME.Length - 1])
						{
							flag_ = BitUtil.Off(flag_, 48);
							script_.RemoveElementAt(0);
							continue;
						}

}
else if ((recoveredScriptCommand == 11)) {
pos_.y_ = seb_v.y_ * (float)sebCnt_ + seb_ay * (float)sebCnt_ * (float)(sebCnt_ + 1) / 2f;
						if (sebCnt_ >= Player.SEB_H_HEAD_ANIME[Player.SEB_H_HEAD_ANIME.Length - 1])
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
						int num6 = sebCnt_ % Player.SEB_H_GOALGLAD_ANIME[1];
						float num7 = GameUtil.SetVStop(8f, Player.SEB_H_GOALGLAD_ANIME[0] / 2);
						float num8 = GameUtil.SetAStop(8f, Player.SEB_H_GOALGLAD_ANIME[0] / 2);
						pos_.y_ = num7 * (float)num6 + num8 * (float)num6 * (float)(num6 + 1) / 2f;
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
else if ((recoveredScriptCommand == 17)) {
{
						int[] collisionData21 = Player.udat_.GetCollisionData(0, 5);
						int[] collisionData22 = Player.udat_.GetCollisionData(1, 5);
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData21[0], pos_.z_ + (float)collisionData21[1], collisionData21[2], collisionData21[3], goalPos_.x_ + (float)collisionData22[0], goalPos_.z_ + (float)collisionData22[1], collisionData22[2], collisionData22[3], true))
						{
							script_.RemoveElementAt(0);
							continue;
						}
						int speed_eva2 = 50;
						bool isTrainingMove2 = false;
						if (tGroupArea_ != null && tGroupArea_.state_ != 12)
						{
							speed_eva2 = 60;
							isTrainingMove2 = true;
						}
						Move(goalPos_, -1, speed_eva2, isTrainingMove2);

					}
}
else if ((recoveredScriptCommand == 18)) {
{
						goalPos_.x_ = firstScript[1];
						goalPos_.z_ = firstScript[2];
						int[] collisionData5 = Player.udat_.GetCollisionData(0, 5);
						int[] collisionData6 = Player.udat_.GetCollisionData(1, 5);
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData5[0], pos_.z_ + (float)collisionData5[1], collisionData5[2], collisionData5[3], goalPos_.x_ + (float)collisionData6[0], goalPos_.z_ + (float)collisionData6[1], collisionData6[2], collisionData6[3], true))
						{
							script_.RemoveElementAt(0);
							continue;
						}
						int speed_eva = 50;
						bool isTrainingMove = false;
						if (tGroupArea_ != null && tGroupArea_.state_ != 12)
						{
							speed_eva = 60;
							isTrainingMove = true;
						}
						Move(goalPos_, -1, speed_eva, isTrainingMove);

					}
}
else if ((recoveredScriptCommand == 19)) {
{
						TBall ball2 = tGroupArea_.GetBall(this, 0);
						ball2.SetPassDataTraining(this, 0, false);
						ball2.ReleaseBall(this);
						AddCommandChangeSeb(1);
						if (GameUtil.Random(100) < 8)
						{
							AddCommandGetTPoint();
						}
						AddCommandWait(Player.SEB_H_KICK_ANIME[Player.SEB_H_KICK_ANIME.Length - 1]);
						AddCommandChangeSeb(0);
						AddCommandTrainingBallWait();
						script_.RemoveElementAt(0);
						continue;
					}
}
else if ((recoveredScriptCommand == 20)) {
{
						TBall ball9 = tGroupArea_.GetBall(this, 1);
						if (BitUtil.Check(ball9.flag_, 8)) {
						int[] collisionData19 = Player.udat_.GetCollisionData(0, 0);
						int[] collisionData20 = Player.udat_.GetCollisionData(1, 1);
float recoveredUnusedSingle17;
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData19[0], pos_.z_ + (float)collisionData19[1], collisionData19[2], collisionData19[3], ball9.pos_.x_ + (float)collisionData20[0], ball9.pos_.z_ + (float)collisionData20[1], collisionData20[2], collisionData20[3], true))
						{
							ball9.ReceiveBall(this);
							if (GameUtil.Random(100) < 10)
							{
								ball9.ReleaseBall(this);
								ball9.receive_wait *= 2;
								int angle3 = GameUtil.Random(360);
								int angle4 = GameUtil.Random(20) + 10;
								float y3 = 7f * GameUtil.Sin(angle4);
								float x4 = 7f * GameUtil.Cos(angle4) * GameUtil.Cos(angle3);
								float z3 = 7f * GameUtil.Cos(angle4) * GameUtil.Sin(angle3);
								ball9.v_.Set(x4, y3, z3);
								ball9.a_.Set(0f, -0.45f, 0f);
								AddCommandWait(14);
								AddCommandTrainingBallChase();
								int x5 = (int)GameUtil.RateConvert(10 + trainingId_[4] * 80, 0f, 99f, tGroupArea_.centerPos_.x_ - tGroupArea_.coatW_ / 2f, tGroupArea_.centerPos_.x_ + tGroupArea_.coatW_ / 2f, true);
								int z4 = (int)tGroupArea_.centerPos_.z_;
								AddCommandTrainigMoveGoalPosSet(x5, z4);
								AddCommandChangeDirect(1 + 2 * trainingId_[4]);
								AddCommandWait(GameUtil.Random(10) + 4);
								AddCommandTrainingBallKick();
							}
							else
							{
								AddCommandWait(GameUtil.Random(10) + 4);
								AddCommandTrainingBallKick();
							}
							script_.RemoveElementAt(0);
							continue;
						}

}

					}
}
else if ((recoveredScriptCommand == 21)) {
{
						TBall ball8 = tGroupArea_.GetBall(this, 2);
						int[] collisionData17 = Player.udat_.GetCollisionData(0, 0);
						int[] collisionData18 = Player.udat_.GetCollisionData(1, 1);
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData17[0], pos_.z_ + (float)collisionData17[1], collisionData17[2], collisionData17[3], ball8.pos_.x_ + (float)collisionData18[0], ball8.pos_.z_ + (float)collisionData18[1], collisionData18[2], collisionData18[3], true))
						{
							if (BitUtil.Check(ball8.flag_, 8))
							{
								ball8.ReceiveBall(this);
								script_.RemoveElementAt(0);
								continue;
							}
						}
						else
						{
							Move(ball8.pos_, -1);
						}

					}
}
else if ((recoveredScriptCommand == 22)) {
{
						TBall ball3 = tGroupArea_.GetBall(this, 3);
						int num9 = GameUtil.Random(3);
						if (GameUtil.Random(100) < 10)
						{
							num9 = -1;
						}
						else if (GameUtil.Random(100) < 8)
						{
							AddCommandGetTPoint();
						}
						int num10 = 10;
						if (num9 != -1)
						{
							num10 = LIFTING_TIME[firstScript[1]][num9];
							float y = (LIFTING_POS[num9].y_ - ball3.pos_.y_) / (float)num10 - -0.45f * (float)(num10 + 1) / 2f;
							float x = (pos_.x_ + LIFTING_POS[num9].x_ - ball3.pos_.x_) / (float)num10;
							ball3.v_.Set(x, y, 0f);
							ball3.a_.Set(0f, -0.45f, 0f);
						float recoveredUnusedSingle33;
}
						else
						{
							int angle = GameUtil.Random(360);
							int angle2 = GameUtil.Random(60) - 45;
							float y2 = 7f * GameUtil.Sin(angle2);
							float x2 = 7f * GameUtil.Cos(angle2) * GameUtil.Cos(angle);
							float z = 7f * GameUtil.Cos(angle2) * GameUtil.Sin(angle);
							ball3.v_.Set(x2, y2, z);
							ball3.a_.Set(0f, -0.45f, 0f);
						}
						if ((num9 == 0)) {
AddCommandWait(num10 - (Player.SEB_H_KICK_ANIME[Player.SEB_H_KICK_ANIME.Length - 1] - 1));
							AddCommandChangeDirect(2);
							AddCommandWait(5);
							AddCommandChangeSeb(1);
							AddCommandWait(Player.SEB_H_KICK_ANIME[Player.SEB_H_KICK_ANIME.Length - 1] - 1);
							AddCommandChangeSeb(0);
							AddCommandLifting(num9);

}
else if ((num9 == 1)) {
AddCommandWait(num10 - (Player.SEB_H_KICK_ANIME[Player.SEB_H_KICK_ANIME.Length - 1] - 1));
							AddCommandChangeDirect(1);
							AddCommandWait(5);
							AddCommandChangeSeb(1);
							AddCommandWait(Player.SEB_H_KICK_ANIME[Player.SEB_H_KICK_ANIME.Length - 1] - 1);
							AddCommandChangeSeb(0);
							AddCommandLifting(num9);

}
else if ((num9 == 2)) {
AddCommandWait(num10 - 14 + 5);
							AddCommandFlagOff(2);
							AddCommandWait(14);
							AddCommandFlagOn(2);
							AddCommandLifting(num9);

}
else if ((num9 == -1)) {
{
							flag_ = BitUtil.Off(flag_, 131072);
							ball3.ReleaseBall(this);
							ball3.receive_wait *= 2;
							AddCommandWait(14);
							AddCommandLiftingBallChase();
							int x3 = (int)GameUtil.RateConvert(30 + trainingId_[4] * 40, 0f, 99f, tGroupArea_.centerPos_.x_ - tGroupArea_.coatW_ / 2f, tGroupArea_.centerPos_.x_ + tGroupArea_.coatW_ / 2f, true);
							int z2 = (int)tGroupArea_.centerPos_.z_;
							AddCommandTrainigMoveGoalPosSet(x3, z2);
							AddCommandFlagOn(131072);
							AddCommandChangeDirect(2);
							AddCommandLifting(0);

						}
}
						script_.RemoveElementAt(0);
						continue;
					}
}
else if ((recoveredScriptCommand == 23)) {
{
						TBall ball5 = tGroupArea_.GetBall(this, 4);
						int[] collisionData7 = Player.udat_.GetCollisionData(0, 0);
						int[] collisionData8 = Player.udat_.GetCollisionData(1, 1);
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData7[0], pos_.z_ + (float)collisionData7[1], collisionData7[2], collisionData7[3], ball5.pos_.x_ + (float)collisionData8[0], ball5.pos_.z_ + (float)collisionData8[1], collisionData8[2], collisionData8[3], true))
						{
							if (BitUtil.Check(ball5.flag_, 8))
							{
								ball5.ReceiveBall(this);
								script_.RemoveElementAt(0);
								continue;
							}
						}
						else
						{
							Move(ball5.pos_, -1);
						}

					}
}
else if ((recoveredScriptCommand == 24)) {
{
						PlayerData playerData3 = Player.apdat_.pData_[pData_id_];
						int num19 = playerData3.paramPhysical_[2];
						float num20 = GameUtil.RateConvert(num19, 0f, 1000f, CORN_AROUND_POS[dpoint][0].x_, CORN_AROUND_POS[dpoint][1].x_, true);
						float num21 = GameUtil.RateConvert(num19, 0f, 1000f, CORN_AROUND_POS[dpoint][0].z_, CORN_AROUND_POS[dpoint][1].z_, true);
						goalPos_.Set(tGroupArea_.traPos_[dcorn]);
						goalPos_.x_ += num20;
						if (dcorn % 2 == 0)
						{
							goalPos_.z_ += num21;
						}
						else
						{
							goalPos_.z_ += -num21;
						}
						int[] collisionData15 = Player.udat_.GetCollisionData(0, 5);
						int[] collisionData16 = Player.udat_.GetCollisionData(1, 5);
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData15[0], pos_.z_ + (float)collisionData15[1], collisionData15[2], collisionData15[3], goalPos_.x_ + (float)collisionData16[0], goalPos_.z_ + (float)collisionData16[1], collisionData16[2], collisionData16[3], true))
						{
							dpoint++;
							if (dpoint == CORN_AROUND_POS.Length)
							{
								dpoint = 0;
								dcorn++;
								if (dcorn == 4)
								{
									TBall ball7 = tGroupArea_.GetBall(this, 5);
									ball7.ReleaseBall(this);
									ball7.flag_ = BitUtil.On(ball7.flag_, 1);
									AddCommandGetTPoint();
									AddCommandDribbleOlderRegist();
									AddCommandTrainigMoveGoalPosSet((int)(tGroupArea_.traPos_[tGroupArea_.traPos_.Length - 1].x_ + 20f), (int)(tGroupArea_.traPos_[0].z_ + 80f));
									AddCommandTrainigMoveGoalPosSet((int)(tGroupArea_.traPos_[0].x_ - 20f), (int)(tGroupArea_.traPos_[0].z_ + 80f));
									AddCommandDribbleOlder();
									script_.RemoveElementAt(0);
									continue;
								}
							}
						}
						else
						{
							Move(goalPos_, -1);
						}

					}
}
else if ((recoveredScriptCommand == 25)) {
{
						int num12 = tGroupArea_.older_vec.IndexOf(this);
						goalPos_.Set(tGroupArea_.traPos_[0]);
						goalPos_.x_ -= 20f;
						goalPos_.z_ += (float)num12 * 20f;
						int[] collisionData9 = Player.udat_.GetCollisionData(0, 5);
						int[] collisionData10 = Player.udat_.GetCollisionData(1, 5);
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData9[0], pos_.z_ + (float)collisionData9[1], collisionData9[2], collisionData9[3], goalPos_.x_ + (float)collisionData10[0], goalPos_.z_ + (float)collisionData10[1], collisionData10[2], collisionData10[3], true))
						{
							direct_ = 2;
							if (num12 == 0 && tGroupArea_.trainingWait_ <= 0)
							{
								TBall ball6 = tGroupArea_.GetBall(this, 6);
								if (ball6 == null)
								{
									return true;
								}
								ball6.ReceiveBall(this);
								ball6.flag_ = BitUtil.Off(ball6.flag_, 1);
								tGroupArea_.older_vec.RemoveElementAt(num12);
								tGroupArea_.trainingWait_ = 40;
								dcorn = 0;
								dpoint = 0;
								AddCommandDribble();
								script_.RemoveElementAt(0);
								continue;
							}
						}
						else
						{
							Move(goalPos_, -1);
						}

					}
}
else if ((recoveredScriptCommand == 26)) {
tGroupArea_.older_vec.AddElement(this);
						script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 27)) {
{
						PlayerData playerData2 = Player.apdat_.pData_[pData_id_];
						int num11 = 100;
						TBall ball4 = tGroupArea_.GetBall(this, 7);
						ball4.SetShoot(this, tGroupArea_.traPos_[2]);
						ball4.ReleaseBall(this);
						AddCommandChangeSeb(1);
						AddCommandWait(Player.SEB_H_KICK_ANIME[Player.SEB_H_KICK_ANIME.Length - 1]);
						AddCommandChangeSeb(0);
						AddCommandWait(6);
						AddCommandGetTPoint();
						AddCommandShootOlderRegist();
						AddCommandShootOlder();
						script_.RemoveElementAt(0);
						continue;
					}
}
else if ((recoveredScriptCommand == 28)) {
{
						int num4 = tGroupArea_.older_vec.IndexOf(this);
						goalPos_.Set(tGroupArea_.traPos_[0]);
						goalPos_.x_ -= (float)num4 * 20f;
						int[] collisionData = Player.udat_.GetCollisionData(0, 5);
						int[] collisionData2 = Player.udat_.GetCollisionData(1, 5);
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], goalPos_.x_ + (float)collisionData2[0], goalPos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true))
						{
							direct_ = 1;
							if (num4 == 0)
							{
								if (tGroupArea_.traPos_[1].z_ > tGroupArea_.traPos_[0].z_)
								{
									direct_ = 0;
								}
								else
								{
									direct_ = 2;
								}
							}
							if (num4 == 0 && tGroupArea_.trainingWait_ <= 0)
							{
								TBall ball = tGroupArea_.GetBall(this, 8);
								ball.ReceiveBall(this);
								ball.flag_ = BitUtil.Off(ball.flag_, 1);
								tGroupArea_.older_vec.RemoveElementAt(num4);
								tGroupArea_.trainingWait_ = 50;
								int direct = 0;
								if (trainingId_[1] % 2 == 1)
								{
									direct = 2;
								}
								AddCommandTrainigMoveGoalPosSet((int)tGroupArea_.traPos_[1].x_, (int)tGroupArea_.traPos_[1].z_);
								AddCommandChangeDirect(direct);
								AddCommandWait(12);
								AddCommandShoot();
								script_.RemoveElementAt(0);
								continue;
							}
						}
						else
						{
							Move(goalPos_, -1);
						}

					}
}
else if ((recoveredScriptCommand == 29)) {
tGroupArea_.older_vec.AddElement(this);
						script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 30)) {
{
						for (int k = 0; k < tGroupArea_.tBallList_.Size(); k++)
						{
							TBall tBall3 = (TBall)tGroupArea_.tBallList_.ElementAt(k);
							if (!tBall3.ChBallDirectHere(this))
							{
								continue;
							}
							int toGoalLineTime = tBall3.GetToGoalLineTime(this);
							if (toGoalLineTime > 0) {
 if (toGoalLineTime < Player.SEB_H_SAVING_ANIME[0]*5/10) {

							flag_ = BitUtil.On(flag_, 8);
							AddCommandChangeSeb(14);
							AddCommandTraSaving();
							seb_startPos.Set(pos_);
							seb_v.y_ = GameUtil.SetVStop(10f, Player.SEB_H_SAVING_ANIME[0] / 2);
							seb_ay = GameUtil.SetAStop(10f, Player.SEB_H_SAVING_ANIME[0] / 2);
							seb_v.x_ = (tBall3.g1.x_ - pos_.x_) / (float)(Player.SEB_H_SAVING_ANIME[0] / 2);
							seb_v.z_ = 0f;
							float num17 = (float)JMath.Sqrt(seb_v.x_ * seb_v.x_ + seb_v.z_ * seb_v.z_);
							float num18 = PlayerData.GetKeeperJampMax(this) / 2f;
							if (num17 > num18)
							{
								seb_v.x_ = seb_v.x_ * num18 / num17;
								seb_v.z_ = seb_v.z_ * num18 / num17;
							}
							if (num17 < 1.5f)
							{
								if (tGroupArea_.tArea_.pId_ == 2)
								{
									direct_ = 2;
								}
								else
								{
									direct_ = 0;
								}
							}
							else if (seb_v.x_ < 0f)
							{
								direct_ = 3;
							}
							else
							{
								direct_ = 1;
							}
							script_.RemoveElementAt(0);
							break;
}
}
						}
						if (!BitUtil.Check(flag_, 8)) {
						int[] collisionData13 = Player.udat_.GetCollisionData(0, 5);
						int[] collisionData14 = Player.udat_.GetCollisionData(1, 5);
						goalPos_.Set(tGroupArea_.traPos_[2]);
						if (tGroupArea_.tArea_.pId_ == 2)
						{
							goalPos_.z_ += -30f;
						}
						else
						{
							goalPos_.z_ += 30f;
						}
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData13[0], pos_.z_ + (float)collisionData13[1], collisionData13[2], collisionData13[3], goalPos_.x_ + (float)collisionData14[0], goalPos_.z_ + (float)collisionData14[1], collisionData14[2], collisionData14[3], true))
						{
							if (tGroupArea_.tArea_.pId_ == 2)
							{
								direct_ = 2;
							}
							else
							{
								direct_ = 0;
							}
						}
						else
						{
							Move(goalPos_, -1);
						}

}

					}
}
else if ((recoveredScriptCommand == 31)) {
{
						if (BitUtil.Check(flag_, 8) && !BitUtil.Check(flag_, 4))
						{
							for (int i = 0; i < tGroupArea_.tBallList_.Size(); i++)
							{
								TBall tBall = (TBall)tGroupArea_.tBallList_.ElementAt(i);
								if(!tBall.ChBallDirectHere(this)){continue;} CollisionSaving(tBall);
							}
						}
						if (sebCnt_ < Player.SEB_H_SAVING_ANIME[0])
						{
							pos_.x_ = seb_startPos.x_ + seb_v.x_ * (float)sebCnt_;
							pos_.z_ = seb_startPos.z_ + seb_v.z_ * (float)sebCnt_;
							pos_.y_ = seb_v.y_ * (float)sebCnt_ + seb_ay * (float)sebCnt_ * (float)(sebCnt_ + 1) / 2f;
						}
						else
						{
							pos_.x_ = seb_startPos.x_ + seb_v.x_ * (float)Player.SEB_H_SAVING_ANIME[0] + seb_v.x_ / 3f * (float)(sebCnt_ - Player.SEB_H_SAVING_ANIME[0]);
							pos_.z_ = seb_startPos.z_ + seb_v.z_ * (float)Player.SEB_H_SAVING_ANIME[0] + seb_v.z_ / 3f * (float)(sebCnt_ - Player.SEB_H_SAVING_ANIME[0]);
							pos_.y_ = seb_v.y_ * (float)sebCnt_ + seb_ay * (float)sebCnt_ * (float)(sebCnt_ + 1) / 2f;
							if (pos_.y_ < 0f)
							{
								pos_.y_ = 0f;
							}
						}
						TBall tBall2 = null;
						if (BitUtil.Check(flag_, 4))
						{
							for (int j = 0; j < tGroupArea_.tBallList_.Size(); j++)
							{
								tBall2 = (TBall)tGroupArea_.tBallList_.ElementAt(j);
								if (tBall2.t_owner_ != null && tBall2.t_owner_.trainingId_[0] == trainingId_[0])
								{
									break;
								}
							}
						}
						if (sebCnt_ >= Player.SEB_H_SAVING_ANIME[Player.SEB_H_SAVING_ANIME.Length - 1])
						{
							AddCommandGetTPoint();
							if (tBall2 != null)
							{
								tBall2.ReleaseBall(this);
							}
							flag_ = BitUtil.Off(flag_, 8);
							AddCommandChangeSeb(0);
							AddCommandShootGK();
							script_.RemoveElementAt(0);
							continue;
						}

					}
}
else if ((recoveredScriptCommand == 32)) {
{
						seb_ = 10;
						int num13 = 0;
						int num14 = 0;
						if(pos_.x_>=tGroupArea_.centerPos_.x_+tGroupArea_.coatW_/2f){num13=2;} else if(pos_.x_>tGroupArea_.centerPos_.x_-tGroupArea_.coatW_/2f){num13=1;} else{num13=0;}
						if(pos_.z_>=tGroupArea_.centerPos_.z_+tGroupArea_.coatH_/2f){num14=0;} else if(pos_.z_>tGroupArea_.centerPos_.z_-tGroupArea_.coatH_/2f){num14=1;} else{num14=2;}
						if ((num13 == 0)) {
goalPos_.x_ = tGroupArea_.centerPos_.x_ - tGroupArea_.coatW_ / 2f;

}
else if ((num13 == 1)) {
goalPos_.x_ = pos_.x_;

}
else {
goalPos_.x_ = tGroupArea_.centerPos_.x_ + tGroupArea_.coatW_ / 2f;

}
						if ((num14 == 0)) {
goalPos_.z_ = tGroupArea_.centerPos_.z_ + tGroupArea_.coatH_ / 2f;

}
else if ((num14 == 1)) {
goalPos_.z_ = pos_.z_;

}
else {
goalPos_.z_ = tGroupArea_.centerPos_.z_ - tGroupArea_.coatH_ / 2f;

}
						int[] collisionData11 = Player.udat_.GetCollisionData(0, 5);
						int[] collisionData12 = Player.udat_.GetCollisionData(1, 5);
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData11[0], pos_.z_ + (float)collisionData11[1], collisionData11[2], collisionData11[3], goalPos_.x_ + (float)collisionData12[0], goalPos_.z_ + (float)collisionData12[1], collisionData12[2], collisionData12[3], true))
						{
							int num15 = num13 + num14 * 3;
							int num16 = 0;
							if ((num15 == 1) || (num15 == 2)) {
num16 = 0;

}
else if ((num15 == 0) || (num15 == 3)) {
num16 = 3;

}
else if ((num15 == 6) || (num15 == 7)) {
num16 = 2;

}
else if ((num15 == 8) || (num15 == 5)) {
num16 = 1;

}
else {
num16 = GameUtil.Random(4);

}
							if ((num16 == 0)) {
AddCommandTrainigMoveGoalPosSet((int)pos_.x_, (int)(tGroupArea_.centerPos_.z_ + tGroupArea_.coatH_ / 2f));

}
else if ((num16 == 1)) {
AddCommandTrainigMoveGoalPosSet((int)(tGroupArea_.centerPos_.x_ + tGroupArea_.coatW_ / 2f), (int)pos_.z_);

}
else if ((num16 == 2)) {
AddCommandTrainigMoveGoalPosSet((int)pos_.x_, (int)(tGroupArea_.centerPos_.z_ - tGroupArea_.coatH_ / 2f));

}
else if ((num16 == 3)) {
AddCommandTrainigMoveGoalPosSet((int)(tGroupArea_.centerPos_.x_ - tGroupArea_.coatW_ / 2f), (int)pos_.z_);

}
							AddCommandRunning(num16);
							script_.RemoveElementAt(0);
							continue;
						}
						Move(goalPos_, -1);

					}
}
else if ((recoveredScriptCommand == 33)) {
{
						if (firstScript[1] == 0)
						{
							goalPos_.x_ = tGroupArea_.centerPos_.x_ - tGroupArea_.coatW_ / 2f;
							goalPos_.z_ = tGroupArea_.centerPos_.z_ + tGroupArea_.coatH_ / 2f;
						}
						else if (firstScript[1] == 1)
						{
							goalPos_.x_ = tGroupArea_.centerPos_.x_ + tGroupArea_.coatW_ / 2f;
							goalPos_.z_ = tGroupArea_.centerPos_.z_ + tGroupArea_.coatH_ / 2f;
						}
						else if (firstScript[1] == 2)
						{
							goalPos_.x_ = tGroupArea_.centerPos_.x_ + tGroupArea_.coatW_ / 2f;
							goalPos_.z_ = tGroupArea_.centerPos_.z_ - tGroupArea_.coatH_ / 2f;
						}
						else if (firstScript[1] == 3)
						{
							goalPos_.x_ = tGroupArea_.centerPos_.x_ - tGroupArea_.coatW_ / 2f;
							goalPos_.z_ = tGroupArea_.centerPos_.z_ - tGroupArea_.coatH_ / 2f;
						}
						int[] collisionData3 = Player.udat_.GetCollisionData(0, 5);
						int[] collisionData4 = Player.udat_.GetCollisionData(1, 5);
						if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData3[0], pos_.z_ + (float)collisionData3[1], collisionData3[2], collisionData3[3], goalPos_.x_ + (float)collisionData4[0], goalPos_.z_ + (float)collisionData4[1], collisionData4[2], collisionData4[3], true))
						{
							int next_point = (firstScript[1] - 1 + 4) % 4;
							AddCommandGetTPoint();
							AddCommandRunning(next_point);
							script_.RemoveElementAt(0);
							continue;
						}
						Move(goalPos_, -1);

					}
}
else if ((recoveredScriptCommand == 34)) {
{
						PlayerData playerData = Player.apdat_.pData_[pData_id_];
						int lv_ = playerData.lv_;
						int num5 = playerData.UpdateStockExp();
						if (num5 > 0)
						{
							Player.apdat_.AddEffectExpUp(trainingId_[0], num5);
						}
						if (lv_ != playerData.lv_)
						{
							Player.apdat_.AddEffectLvUp(AppData.EFF_EXPUP_TIME[1], trainingId_[0]);
						}
						script_.RemoveElementAt(0);
						continue;
					}
}
else if ((recoveredScriptCommand == 35)) {
flag_ = BitUtil.On(flag_, 1);
						Player.udat_.SetTenantIn(this);
						script_.RemoveElementAt(0);
						continue;
}
else if ((recoveredScriptCommand == 36)) {
{
						int num = GameUtil.Random(100);
						float num2 = GameUtil.RateConvert(num, 0f, 99f, MapMain.MOVE_POINT[8].x_, MapMain.MOVE_POINT[9].x_, true);
						float num3 = GameUtil.RateConvert(num, 0f, 99f, MapMain.MOVE_POINT[8].z_, MapMain.MOVE_POINT[9].z_, true);
						AddCommandTrainigMoveGoalPosSet((int)num2, (int)num3);
						AddCommandWait(25 + GameUtil.Random(30));
						if (GameUtil.Random(100) < 30)
						{
							AddCommandChangeDirect(GameUtil.Random(4));
							AddCommandWait(20 + GameUtil.Random(10));
						}
						AddCommandSvisorAct();
						script_.RemoveElementAt(0);
						continue;
					}
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

		public void Update_trainings()
		{
			frame_ = (frame_ + 1) % int.MaxValue;
			sebCnt_ = (sebCnt_ + 1) % int.MaxValue;
			Script();
			Camera.ConvertGlobalToScreen(pos_, screen_pos_);
		}

		public void Draw_training(Graphics g)
		{
			if (BitUtil.Check(flag_, 1))
			{
				return;
			}
			Camera.ConvertGlobalToCamera(pos_, camera_pos);
			shadowPos_.x_ = pos_.x_;
			shadowPos_.z_ = pos_.z_;
			Camera.ConvertGlobalToCamera(shadowPos_, camera_pos_shadow);
			if (!GameUtil.ChRectHitRect(camera_pos[0] - 7, camera_pos[1] - 20, 14, 20, GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))
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
			Player.apdat_.InsertGameObject(8, 0, depth, 2, 0, 4, camera_pos_shadow[0], camera_pos_shadow[1] + num2, dark, 0);
			if (seb_ == 5)
			{
				int num3 = sebCnt_ / 3;
				if (num3 <= 2)
				{
					Camera.ConvertGlobalToCamera(seb_startPos, camera_pos_sliding);
					Player.apdat_.InsertGameObject(8, 0, depth, 10, 7, num3, camera_pos_sliding[0], camera_pos_sliding[1] + num2, dark, 0);
				}
			}
			int layer = SetDrawParam(seb_, -1);
			Player.apdat_.InsertGameObject(2, trainingId_[0], depth, 0, 0, 0, camera_pos[0], camera_pos[1], dark, layer);
		}

		public void Draw_svisor(Graphics g)
		{
			if (BitUtil.Check(flag_, 1)) { return; }
{
Camera.ConvertGlobalToCamera(pos_, camera_pos);
shadowPos_.x_ = pos_.x_;
shadowPos_.z_ = pos_.z_;
Camera.ConvertGlobalToCamera(shadowPos_, camera_pos_shadow);
if (!(GameUtil.ChRectHitRect(camera_pos[0] - 7, camera_pos[1] - 20, 14, 20, GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))) { return; }
{
int depth = camera_pos_shadow[1] + 1;
bool dark = false;
int num = 1;
Player.apdat_.InsertGameObject(8, 0, depth, 2, 0, 4, camera_pos_shadow[0], camera_pos_shadow[1] + num, dark, 0);
int layer = SetDrawParam(faceImg_, bodyImg_, seb_, -1);
Player.apdat_.InsertGameObject(3, trainingId_[0], depth, 0, 0, 0, camera_pos[0], camera_pos[1], dark, layer);
}
}
		}

		public void CollisionSaving(TBall tball)
		{
			int[] collisionData = Player.udat_.GetCollisionData(0, 0);
			int[] collisionData2 = Player.udat_.GetCollisionData(1, 1);
			if (GameUtil.ChRectHitRect(pos_.x_ + (float)collisionData[0], pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], tball.pos_.x_ + (float)collisionData2[0], tball.pos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], true) && BitUtil.Check(flag_, 8))
			{
				int recoveredSavingChoice = GameUtil.Random(3);
if (recoveredSavingChoice == 2) {tball.v_.Set(0f, 0f, 0f);
					tball.ReceiveBall(this);
					} else if (recoveredSavingChoice == 1) {
					int angle2 = GameUtil.Random(180) + team_id_ * 180;
					float recoveredUnusedX2, recoveredUnusedY2;
float num2 = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 5f, 10f, true);
					int x2 = GameUtil.Random(100);
					float dis2 = GameUtil.RateConvert(x2, 0, 99, 5, 20, true);
					int t2 = GameUtil.RateConvert(x2, 0, 99, 5, 10, true);
					tball.ReceiveBall(this);
					tball.ReleaseBall(this);
					tball.v_.Set(num2 * GameUtil.Cos(angle2), 0f, num2 * GameUtil.Sin(angle2));
					tball.v_.y_ = GameUtil.SetVStop(dis2, t2);
					tball.a_.y_ = GameUtil.SetAStop(dis2, t2);
					} else {
					int angle = GameUtil.Random(90) + 45 + (1 - team_id_) * 180;
					float recoveredUnusedX, recoveredUnusedY;
float num = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, 2.5f, 5f, true);
					int x = GameUtil.Random(100);
					float dis = GameUtil.RateConvert(x, 0, 99, 2, 10, true);
					int t = GameUtil.RateConvert(x, 0, 99, 5, 10, true);
					tball.ReceiveBall(this);
					tball.ReleaseBall(this);
					tball.v_.Set(num * GameUtil.Cos(angle), 0f, num * GameUtil.Sin(angle));
					tball.v_.y_ = GameUtil.SetVStop(dis, t);
					tball.a_.y_ = GameUtil.SetAStop(dis, t);
					}
			}
		}

		public void EndTrainingScript()
		{
			int num = GameUtil.Random(100);
			int num2 = GameUtil.Random(100);
			if (tGroupArea_.tArea_.tMode_ == 4)
			{
				goalPos_.x_ = pos_.x_;
				goalPos_.z_ = pos_.z_;
			}
			else
			{
				goalPos_.x_ = GameUtil.RateConvert(num, 0f, 99f, tGroupArea_.centerPos_.x_ - tGroupArea_.coatW_ * 10f / 100f, tGroupArea_.centerPos_.x_ + tGroupArea_.coatW_ * 10f / 100f, true);
				goalPos_.z_ = GameUtil.RateConvert(num2, 0f, 99f, tGroupArea_.centerPos_.z_ + tGroupArea_.coatH_ * 5f / 100f, tGroupArea_.centerPos_.z_ - tGroupArea_.coatH_ * 5f / 100f, true);
			}
			ClearScript();
			AddCommandChangeSeb(0);
			AddCommandFlagOn(2);
			AddCommandTrainigMoveGoalPos();
		}

		public void StopTraining()
		{
			seb_ = 0;
			flag_ = BitUtil.Off(flag_, 12);
			flag_ = BitUtil.On(flag_, 2);
			if (pos_.y_ > 0f)
			{
				pos_.y_ = 0f;
			}
			ClearScript();
		}
	}
}
