using data;
using form;
using java.io;
using java.util;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace game
{
	public class TGroupArea
	{
		public const int STATE_NEW_STANDBY = 10;

		public const int STATE_NEW_GATHER = 11;

		public const int STATE_NEW_MAIN = 12;

		public const int STATE_NEW_TRAINING_STOP = 13;

		public const int STATE_NEW_TRAINING_CHANGE = 14;

		public AppData apdat_;

		public UserData udat_;

		public int id_;

		public Vector3D centerPos_;

		public float coatW_;

		public float coatH_;

		public TArea tArea_;

		public Vector tPlayerList_;

		public Vector tBallList_;

		public int state_;

		public int stateCnt_;

		public Vector older_vec;

		public Vector3D[] traPos_;

		public int ballIndex_;

		public int trainingWait_;

		public Vector3D[] filed_gpos = new Vector3D[4];

		public int[][] filed_cpos = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		public int[] cx = new int[4];

		public int[] cy = new int[4];

		public Vector3D[] goal_gpos = new Vector3D[4];

		public int[][] goal_cpos = JTool.MakeArray<int[][]>(new int[2] { 4, 2 });

		public int[] temp_olderVecId_;

		public int[] temp_tPlayerListId_;

		public int[] temp_tBallListId_;

		private static int[] camera_pos = new int[2];

		public TGroupArea(int id, TArea tArea)
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			tPlayerList_ = new Vector();
			tBallList_ = new Vector();
			id_ = id;
			tArea_ = tArea;
			older_vec = new Vector();
			Init();
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteInt(os, stateCnt_);
			StreamUtil.WriteShort(os, (short)older_vec.Size());
			for (int i = 0; i < older_vec.Size(); i++)
			{
				TPlayer tPlayer = (TPlayer)older_vec.ElementAt(i);
				StreamUtil.WriteShort(os, (short)tPlayer.trainingId_[0]);
			}
			StreamUtil.WriteShort(os, (short)ballIndex_);
			StreamUtil.WriteInt(os, trainingWait_);
			StreamUtil.WriteInt(os, tPlayerList_.Size());
			for (int j = 0; j < tPlayerList_.Size(); j++)
			{
				TPlayer tPlayer2 = (TPlayer)tPlayerList_.ElementAt(j);
				StreamUtil.WriteShort(os, (short)tPlayer2.trainingId_[0]);
			}
			StreamUtil.WriteInt(os, tBallList_.Size());
			for (int k = 0; k < tBallList_.Size(); k++)
			{
				TBall tBall = (TBall)tBallList_.ElementAt(k);
				StreamUtil.WriteShort(os, (short)tBall.tId_);
			}
		}

		public void Deserialize(InputStream stream, TArea tArea)
		{
			tArea_ = tArea;
			state_ = StreamUtil.ReadShort(stream);
			stateCnt_ = StreamUtil.ReadInt(stream);
			int num = StreamUtil.ReadShort(stream);
			temp_olderVecId_ = new int[num];
			for (int i = 0; i < temp_olderVecId_.Length; i++)
			{
				temp_olderVecId_[i] = StreamUtil.ReadShort(stream);
			}
			ballIndex_ = StreamUtil.ReadShort(stream);
			trainingWait_ = StreamUtil.ReadInt(stream);
			int num2 = StreamUtil.ReadInt(stream);
			temp_tPlayerListId_ = new int[num2];
			for (int j = 0; j < temp_tPlayerListId_.Length; j++)
			{
				temp_tPlayerListId_[j] = StreamUtil.ReadShort(stream);
			}
			num2 = StreamUtil.ReadInt(stream);
			temp_tBallListId_ = new int[num2];
			for (int k = 0; k < temp_tBallListId_.Length; k++)
			{
				temp_tBallListId_[k] = StreamUtil.ReadShort(stream);
			}
		}

		public void Deserialize2()
		{
			UserData instance = UserData.GetInstance();
			older_vec = new Vector();
			tPlayerList_ = new Vector();
			tBallList_ = new Vector();
			for (int i = 0; i < temp_olderVecId_.Length; i++)
			{
				TPlayer obj = instance.tPlayers_[temp_olderVecId_[i]];
				older_vec.AddElement(obj);
			}
			for (int j = 0; j < temp_tPlayerListId_.Length; j++)
			{
				TPlayer tPlayer = instance.tPlayers_[temp_tPlayerListId_[j]];
				tPlayerList_.AddElement(tPlayer);
				tPlayer.tGroupArea_ = this;
			}
			for (int k = 0; k < temp_tBallListId_.Length; k++)
			{
				TBall tBall = instance.tBalls_[temp_tBallListId_[k]];
				tBallList_.AddElement(tBall);
				tBall.tGroupArea_ = this;
			}
		}

		public void Init()
		{
			centerPos_ = new Vector3D();
			tPlayerList_ = new Vector();
			tBallList_ = new Vector();
			state_ = 10;
		}

		public void Update()
		{
			stateCnt_ = (stateCnt_ + 1) % int.MaxValue;
			if (state_ == 11)
			{
				if (tArea_.tMode_ == 0 || tArea_.tMode_ == 1)
				{
					bool flag = true;
					for (int i = 0; i < tPlayerList_.Size(); i++)
					{
						TPlayer tPlayer = (TPlayer)tPlayerList_.ElementAt(i);
						if (tPlayer.script_.Size() > 0)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						ChangeState(12);
					}
				}
				else
				{
					ChangeState(12);
				}
			}
			else
			{
				if (state_ != 12)
				{
					return;
				}
				if (tArea_.tMode_ == 2 && trainingWait_ > 0)
				{
					trainingWait_--;
				}
				if (tArea_.tMode_ == 3 && trainingWait_ > 0)
				{
					trainingWait_--;
				}
				if (stateCnt_ % GameForm.TRAINING_GETEXP_TIME == GameForm.TRAINING_GETEXP_TIME - 1)
				{
					SupervisorData supervisorData = apdat_.svisorData_[udat_.selectSupervisorId_];
					int trainigGetExp = supervisorData.GetTrainigGetExp();
					for (int j = 0; j < tPlayerList_.Size(); j++)
					{
						TPlayer tPlayer2 = (TPlayer)tPlayerList_.ElementAt(j);
						PlayerData playerData = apdat_.pData_[tPlayer2.pData_id_];
						playerData.StockExp(trainigGetExp);
					}
				}
			}
		}

		public void Draw_obj(Graphics g)
		{
			if (tArea_.tMode_ == 2 && state_ == 12)
			{
				for (int i = 0; i < traPos_.Length; i++)
				{
					Camera.ConvertGlobalToCamera(traPos_[i], camera_pos);
					if (GameUtil.ChRectHitRect(camera_pos[0] - 3, camera_pos[1] - 4, 6, 4, GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))
					{
						apdat_.InsertGameObject(8, 0, camera_pos[1], 39, 37, 0, camera_pos[0], camera_pos[1], false, 0);
					}
				}
			}
			else
			{
				if (tArea_.tMode_ != 3 || state_ != 12)
				{
					return;
				}
				int num2;
				int num3;
				int num4;
				int num = (num2 = (num3 = (num4 = 0)));
				int num5 = 0;
				if (tArea_.pId_ == 3)
				{
					Camera.ConvertGlobalToCamera(traPos_[2], camera_pos);
					for (int j = 0; j < MapMain.GOAL1_PARTS_POS.Length; j++)
					{
						num3 = camera_pos[0] + MapMain.GOAL1_PARTS_POS[j][0] + num;
						num4 = camera_pos[1] + MapMain.GOAL1_PARTS_POS[j][1] + num2;
						num5 = camera_pos[1] + MapMain.GOAL1_PARTS_POS[j][1] + num2 + MapMain.GOAL1_PARTS_GRA[j][2];
						apdat_.InsertGameObject(8, 0, num5, MapMain.GOAL1_PARTS_GRA[j][0], MapMain.GOAL1_PARTS_GRA[j][1], 0, num3, num4, false, 0);
					}
					return;
				}
				num = 0;
				num2 = 0;
				Camera.ConvertGlobalToCamera(traPos_[2], camera_pos);
				for (int k = 0; k < MapMain.GOAL2_PARTS_POS.Length; k++)
				{
					num3 = camera_pos[0] + MapMain.GOAL2_PARTS_POS[k][0] + num;
					num4 = camera_pos[1] + MapMain.GOAL2_PARTS_POS[k][1] + num2;
					num5 = camera_pos[1] + MapMain.GOAL2_PARTS_POS[k][1] + num2 + MapMain.GOAL2_PARTS_GRA[k][2];
					apdat_.InsertGameObject(8, 0, num5, MapMain.GOAL2_PARTS_GRA[k][0], MapMain.GOAL2_PARTS_GRA[k][1], 0, num3, num4, false, 0);
				}
			}
		}

		public void ChangeState(int next_state)
		{
			state_ = next_state;
			stateCnt_ = 0;
			if (state_ == 11)
			{
				if (tArea_.tMode_ == 0)
				{
					for (int i = 0; i < tPlayerList_.Size(); i++)
					{
						TPlayer tPlayer = (TPlayer)tPlayerList_.ElementAt(i);
						tPlayer.ClearScript();
						tPlayer.goalPos_.x_ = GameUtil.RateConvert(10 + 80 * i, 0f, 99f, centerPos_.x_ - coatW_ / 2f, centerPos_.x_ + coatW_ / 2f, true);
						tPlayer.goalPos_.z_ = centerPos_.z_;
						tPlayer.AddCommandWait(GameUtil.Random(14));
						tPlayer.AddCommandTrainigMoveGoalPos();
						tPlayer.AddCommandChangeDirect(1 + i * 2);
					}
				}
				else if (tArea_.tMode_ == 1)
				{
					for (int j = 0; j < tPlayerList_.Size(); j++)
					{
						TPlayer tPlayer2 = (TPlayer)tPlayerList_.ElementAt(j);
						tPlayer2.ClearScript();
						tPlayer2.goalPos_.x_ = GameUtil.RateConvert(30 + 40 * j, 0f, 99f, centerPos_.x_ - coatW_ / 2f, centerPos_.x_ + coatW_ / 2f, true);
						tPlayer2.goalPos_.z_ = centerPos_.z_;
						tPlayer2.AddCommandWait(GameUtil.Random(14));
						tPlayer2.AddCommandTrainigMoveGoalPos();
						tPlayer2.AddCommandChangeDirect(2);
						tPlayer2.AddCommandFlagOn(131072);
					}
				}
				else if (tArea_.tMode_ == 2)
				{
					for (int k = 0; k < tPlayerList_.Size(); k++)
					{
						TPlayer tPlayer3 = (TPlayer)tPlayerList_.ElementAt(k);
						tPlayer3.ClearScript();
						tPlayer3.AddCommandDribbleOlderRegist();
						tPlayer3.AddCommandDribbleOlder();
					}
				}
				else if (tArea_.tMode_ == 4)
				{
					int num = 15;
					for (int l = 0; l < tPlayerList_.Size(); l++)
					{
						TPlayer tPlayer4 = (TPlayer)tPlayerList_.ElementAt(l);
						tPlayer4.ClearScript();
						tPlayer4.AddCommandWait(num);
						num += 15;
						tPlayer4.AddCommandRunningRegist();
					}
				}
				else
				{
					if (tArea_.tMode_ != 3)
					{
						return;
					}
					for (int m = 0; m < tPlayerList_.Size(); m++)
					{
						TPlayer tPlayer5 = (TPlayer)tPlayerList_.ElementAt(m);
						tPlayer5.ClearScript();
						if (m == 0)
						{
							tPlayer5.AddCommandShootGK();
							continue;
						}
						tPlayer5.AddCommandShootOlderRegist();
						tPlayer5.AddCommandShootOlder();
					}
				}
			}
			else if (state_ == 12)
			{
				if (tArea_.tMode_ == 0)
				{
					for (int n = 0; n < tPlayerList_.Size(); n++)
					{
						TPlayer tPlayer6 = (TPlayer)tPlayerList_.ElementAt(n);
						if (n == 0)
						{
							TBall tBall = (TBall)tBallList_.ElementAt(0);
							tBall.ReceiveBall(tPlayer6);
							tPlayer6.AddCommandWait(10);
							tPlayer6.AddCommandTrainingBallKick();
						}
						else
						{
							tPlayer6.AddCommandTrainingBallWait();
						}
					}
				}
				else if (tArea_.tMode_ == 1)
				{
					for (int num2 = 0; num2 < tPlayerList_.Size(); num2++)
					{
						TPlayer tPlayer7 = (TPlayer)tPlayerList_.ElementAt(num2);
						TBall tBall2 = (TBall)tBallList_.ElementAt(num2);
						tBall2.ReceiveBall(tPlayer7);
						tPlayer7.AddCommandLifting(0);
					}
				}
				else if (tArea_.tMode_ != 2 && tArea_.tMode_ != 4 && tArea_.tMode_ != 3)
				{
				}
			}
			else if (state_ == 13)
			{
				for (int num3 = 0; num3 < tPlayerList_.Size(); num3++)
				{
					TPlayer tPlayer8 = (TPlayer)tPlayerList_.ElementAt(num3);
					tPlayer8.StopTraining();
					tPlayer8.AddCommandWait(10);
				}
			}
			else
			{
				if (state_ != 14)
				{
					return;
				}
				int num4 = 0;
				for (int num5 = 0; num5 < tPlayerList_.Size(); num5++)
				{
					TPlayer tPlayer9 = (TPlayer)tPlayerList_.ElementAt(num5);
					if (tPlayer9.oldTrainingId_[1] == 0)
					{
						if (tPlayer9.trainingId_[1] == 0)
						{
							tPlayer9.AddCommandTenantIn();
							continue;
						}
						tPlayer9.pos_.Set(MapMain.MOVE_POINT[7]);
						tPlayer9.AddCommandWait(num4);
						tPlayer9.AddCommandFlagOff(1);
						tPlayer9.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[3].x_, (int)MapMain.MOVE_POINT[3].z_);
						if (tPlayer9.trainingId_[2] == 0)
						{
							tPlayer9.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[5].x_, (int)MapMain.MOVE_POINT[5].z_);
						}
						else
						{
							tPlayer9.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[6].x_, (int)MapMain.MOVE_POINT[6].z_);
						}
						Vector3D startTrainingGatherPos = tArea_.GetStartTrainingGatherPos(this, tPlayer9);
						tPlayer9.AddCommandTrainigMoveGoalPosSet((int)startTrainingGatherPos.x_, (int)startTrainingGatherPos.z_);
						num4 += 6;
					}
					else if (tPlayer9.trainingId_[1] == 0)
					{
						if (tPlayer9.trainingId_[2] == 0)
						{
							tPlayer9.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[5].x_, (int)MapMain.MOVE_POINT[5].z_);
						}
						else
						{
							tPlayer9.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[6].x_, (int)MapMain.MOVE_POINT[6].z_);
						}
						tPlayer9.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[3].x_, (int)MapMain.MOVE_POINT[3].z_);
						tPlayer9.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[7].x_, (int)MapMain.MOVE_POINT[7].z_);
						tPlayer9.AddCommandTenantIn();
					}
					else
					{
						Vector3D startTrainingGatherPos2 = tArea_.GetStartTrainingGatherPos(this, tPlayer9);
						tPlayer9.AddCommandTrainigMoveGoalPosSet((int)startTrainingGatherPos2.x_, (int)startTrainingGatherPos2.z_);
					}
				}
			}
		}

		public void SetCoatData()
		{
			if (tArea_.tMode_ == 0)
			{
				int num = 3;
				coatW_ = tArea_.coatW_;
				coatH_ = tArea_.coatH_ / (float)num;
				centerPos_.x_ = tArea_.centerPos_.x_;
				centerPos_.z_ = tArea_.centerPos_.z_ + tArea_.coatH_ / 2f + (0f - coatH_) / 2f - coatH_ * (float)id_;
			}
			else if (tArea_.tMode_ == 1)
			{
				int num2 = 3;
				coatW_ = tArea_.coatW_;
				coatH_ = tArea_.coatH_ / (float)num2;
				centerPos_.x_ = tArea_.centerPos_.x_;
				centerPos_.z_ = tArea_.centerPos_.z_ + tArea_.coatH_ / 2f + (0f - coatH_) / 2f - coatH_ * (float)id_;
			}
			else if (tArea_.tMode_ == 2)
			{
				int num3 = 2;
				coatW_ = tArea_.coatW_;
				coatH_ = tArea_.coatH_ / (float)num3;
				centerPos_.x_ = tArea_.centerPos_.x_;
				centerPos_.z_ = tArea_.centerPos_.z_ + tArea_.coatH_ / 2f + (0f - coatH_) / 2f - coatH_ * (float)id_;
			}
			else if (tArea_.tMode_ == 4)
			{
				int num4 = 1;
				coatW_ = tArea_.coatW_;
				coatH_ = tArea_.coatH_ / (float)num4;
				centerPos_.x_ = tArea_.centerPos_.x_;
				centerPos_.z_ = tArea_.centerPos_.z_ + tArea_.coatH_ / 2f + (0f - coatH_) / 2f - coatH_ * (float)id_;
			}
			else if (tArea_.tMode_ == 3)
			{
				int num5 = 1;
				coatW_ = tArea_.coatW_ / (float)num5;
				coatH_ = tArea_.coatH_;
				centerPos_.x_ = tArea_.centerPos_.x_ + tArea_.coatW_ / 2f + (0f - coatW_) / 2f - coatW_ * (float)id_;
				centerPos_.z_ = tArea_.centerPos_.z_;
			}
		}

		public void SetPosData()
		{
			if (tArea_.tMode_ == 0)
			{
				Vector3D vector3D = centerPos_;
				float num = coatW_;
				float num2 = coatH_;
				filed_gpos[0] = new Vector3D(vector3D.x_ - num / 2f, 0f, vector3D.z_ + num2 / 2f);
				filed_gpos[1] = new Vector3D(vector3D.x_ + num / 2f, 0f, vector3D.z_ + num2 / 2f);
				filed_gpos[2] = new Vector3D(vector3D.x_ + num / 2f, 0f, vector3D.z_ - num2 / 2f);
				filed_gpos[3] = new Vector3D(vector3D.x_ - num / 2f, 0f, vector3D.z_ - num2 / 2f);
			}
			else if (tArea_.tMode_ == 1)
			{
				Vector3D vector3D2 = centerPos_;
				float num3 = coatW_;
				float num4 = coatH_;
				filed_gpos[0] = new Vector3D(vector3D2.x_ - num3 / 2f, 0f, vector3D2.z_ + num4 / 2f);
				filed_gpos[1] = new Vector3D(vector3D2.x_ + num3 / 2f, 0f, vector3D2.z_ + num4 / 2f);
				filed_gpos[2] = new Vector3D(vector3D2.x_ + num3 / 2f, 0f, vector3D2.z_ - num4 / 2f);
				filed_gpos[3] = new Vector3D(vector3D2.x_ - num3 / 2f, 0f, vector3D2.z_ - num4 / 2f);
			}
			else if (tArea_.tMode_ == 2)
			{
				traPos_ = new Vector3D[4];
				for (int i = 0; i < traPos_.Length; i++)
				{
					traPos_[i] = new Vector3D();
					traPos_[i].x_ = GameUtil.RateConvert(i, 0f, traPos_.Length - 1, centerPos_.x_ - coatW_ * 30f / 100f, centerPos_.x_ + coatW_ * 30f / 100f, true);
					traPos_[i].z_ = centerPos_.z_;
				}
				Vector3D vector3D3 = centerPos_;
				float num5 = coatW_;
				float num6 = coatH_;
				filed_gpos[0] = new Vector3D(vector3D3.x_ - num5 / 2f, 0f, vector3D3.z_ + num6 / 2f);
				filed_gpos[1] = new Vector3D(vector3D3.x_ + num5 / 2f, 0f, vector3D3.z_ + num6 / 2f);
				filed_gpos[2] = new Vector3D(vector3D3.x_ + num5 / 2f, 0f, vector3D3.z_ - num6 / 2f);
				filed_gpos[3] = new Vector3D(vector3D3.x_ - num5 / 2f, 0f, vector3D3.z_ - num6 / 2f);
			}
			else if (tArea_.tMode_ == 4)
			{
				Vector3D vector3D4 = centerPos_;
				float num7 = coatW_;
				float num8 = coatH_;
				filed_gpos[0] = new Vector3D(vector3D4.x_ - num7 / 2f, 0f, vector3D4.z_ + num8 / 2f);
				filed_gpos[1] = new Vector3D(vector3D4.x_ + num7 / 2f, 0f, vector3D4.z_ + num8 / 2f);
				filed_gpos[2] = new Vector3D(vector3D4.x_ + num7 / 2f, 0f, vector3D4.z_ - num8 / 2f);
				filed_gpos[3] = new Vector3D(vector3D4.x_ - num7 / 2f, 0f, vector3D4.z_ - num8 / 2f);
			}
			else
			{
				if (tArea_.tMode_ != 3)
				{
					return;
				}
				traPos_ = new Vector3D[3];
				for (int j = 0; j < traPos_.Length; j++)
				{
					traPos_[j] = new Vector3D();
					traPos_[j].x_ = centerPos_.x_;
					int num9 = 90;
					switch (j)
					{
					case 1:
						num9 = 70;
						break;
					case 2:
						num9 = 15;
						break;
					}
					if (tArea_.pId_ == 3)
					{
						num9 = 100 - num9;
					}
					traPos_[j].z_ = GameUtil.RateConvert(num9, 0f, 100f, centerPos_.z_ + coatH_ / 2f, centerPos_.z_ - coatH_ / 2f, true);
				}
				Vector3D vector3D5 = centerPos_;
				float num10 = coatW_;
				float num11 = coatH_;
				filed_gpos[0] = new Vector3D(vector3D5.x_ - num10 / 2f, 0f, vector3D5.z_ + num11 / 2f);
				filed_gpos[1] = new Vector3D(vector3D5.x_ + num10 / 2f, 0f, vector3D5.z_ + num11 / 2f);
				filed_gpos[2] = new Vector3D(vector3D5.x_ + num10 / 2f, 0f, vector3D5.z_ - num11 / 2f);
				filed_gpos[3] = new Vector3D(vector3D5.x_ - num10 / 2f, 0f, vector3D5.z_ - num11 / 2f);
				Vector3D vector3D6 = traPos_[2];
				float num12 = 80f;
				float num13 = 24f;
				if (tArea_.pId_ == 3)
				{
					num13 = 0f - num13;
				}
				goal_gpos[0] = new Vector3D(vector3D6.x_ - num12 / 2f, 0f, vector3D6.z_);
				goal_gpos[1] = new Vector3D(vector3D6.x_ + num12 / 2f, 0f, vector3D6.z_);
				goal_gpos[2] = new Vector3D(vector3D6.x_ + num12 / 2f, 0f, vector3D6.z_ + num13);
				goal_gpos[3] = new Vector3D(vector3D6.x_ - num12 / 2f, 0f, vector3D6.z_ + num13);
			}
		}

		public TBall GetBall(TPlayer player, int type)
		{
			if (tArea_.tMode_ == 0)
			{
				return (TBall)tBallList_.ElementAt(0);
			}
			if (tArea_.tMode_ == 1)
			{
				return (TBall)tBallList_.ElementAt(player.trainingId_[4]);
			}
			if (tArea_.tMode_ == 2)
			{
				if (!BitUtil.Check(player.flag_, 4))
				{
					for (int i = 0; i < tBallList_.Size(); i++)
					{
						TBall tBall = (TBall)tBallList_.ElementAt(i);
						if (!BitUtil.Check(tBall.flag_, 4))
						{
							return tBall;
						}
					}
				}
				else
				{
					for (int j = 0; j < tBallList_.Size(); j++)
					{
						TBall tBall2 = (TBall)tBallList_.ElementAt(j);
						if (tBall2.t_owner_ != null && tBall2.t_owner_.trainingId_[0] == player.trainingId_[0])
						{
							return tBall2;
						}
					}
				}
			}
			else if (tArea_.tMode_ != 4 && tArea_.tMode_ == 3)
			{
				if (!BitUtil.Check(player.flag_, 4))
				{
					TBall result = (TBall)tBallList_.ElementAt(ballIndex_);
					ballIndex_ = (ballIndex_ + 1 + tBallList_.Size()) % tBallList_.Size();
					return result;
				}
				for (int k = 0; k < tBallList_.Size(); k++)
				{
					TBall tBall3 = (TBall)tBallList_.ElementAt(k);
					if (tBall3.t_owner_ != null && tBall3.t_owner_.trainingId_[0] == player.trainingId_[0])
					{
						return tBall3;
					}
				}
			}
			return null;
		}

		public void SetPlayerGatherPos()
		{
			if (tArea_.tMode_ == 0)
			{
				for (int i = 0; i < tPlayerList_.Size(); i++)
				{
					TPlayer tPlayer = (TPlayer)tPlayerList_.ElementAt(i);
					tPlayer.goalPos_.x_ = GameUtil.RateConvert(10 + 80 * i, 0f, 99f, centerPos_.x_ - coatW_ / 2f, centerPos_.x_ + coatW_ / 2f, true);
					tPlayer.goalPos_.z_ = centerPos_.z_;
					tPlayer.AddCommandWait(GameUtil.Random(14));
					tPlayer.AddCommandTrainigMoveGoalPos();
					tPlayer.AddCommandChangeDirect(1 + i * 2);
				}
			}
			else if (tArea_.tMode_ == 1)
			{
				for (int j = 0; j < tPlayerList_.Size(); j++)
				{
					TPlayer tPlayer2 = (TPlayer)tPlayerList_.ElementAt(j);
					tPlayer2.goalPos_.x_ = GameUtil.RateConvert(30 + 40 * j, 0f, 99f, centerPos_.x_ - coatW_ / 2f, centerPos_.x_ + coatW_ / 2f, true);
					tPlayer2.goalPos_.z_ = centerPos_.z_;
					tPlayer2.AddCommandWait(GameUtil.Random(14));
					tPlayer2.AddCommandTrainigMoveGoalPos();
					tPlayer2.AddCommandChangeDirect(2);
					tPlayer2.AddCommandFlagOn(131072);
				}
			}
			else if (tArea_.tMode_ == 2)
			{
				for (int k = 0; k < tPlayerList_.Size(); k++)
				{
					TPlayer tPlayer3 = (TPlayer)tPlayerList_.ElementAt(k);
					tPlayer3.AddCommandDribbleOlderRegist();
					tPlayer3.AddCommandDribbleOlder();
				}
			}
			else if (tArea_.tMode_ == 4)
			{
				int num = 0;
				for (int l = 0; l < tPlayerList_.Size(); l++)
				{
					TPlayer tPlayer4 = (TPlayer)tPlayerList_.ElementAt(l);
					tPlayer4.AddCommandWait(num);
					num += 6;
					tPlayer4.AddCommandRunningRegist();
				}
			}
			else
			{
				if (tArea_.tMode_ != 3)
				{
					return;
				}
				for (int m = 0; m < tPlayerList_.Size(); m++)
				{
					TPlayer tPlayer5 = (TPlayer)tPlayerList_.ElementAt(m);
					if (m == 0)
					{
						tPlayer5.AddCommandShootGK();
						continue;
					}
					tPlayer5.AddCommandShootOlderRegist();
					tPlayer5.AddCommandShootOlder();
				}
			}
		}

		public static int GetAreaMaxNum(int trainingMode)
		{
			if ((trainingMode == 0)) { return 2; } else if ((trainingMode == 1)) { return 2; } else if ((trainingMode == 2)) { return 5; } else if ((trainingMode == 4)) { return 100; } else if ((trainingMode == 3)) { return 5; } else { return 0; }
		}
	}
}
