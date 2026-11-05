using java.io;
using java.lang;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace game
{
	public class TArea
	{
		public const int STATE_NEW_STANDBY = 10;

		public const int STATE_NEW_GATHER = 11;

		public const int STATE_NEW_MAIN = 12;

		public const int STATE_NEW_TRAINING_STOP = 13;

		public const int STATE_NEW_TRAINING_CHANGE = 14;

		public AppData apdat_;

		public UserData udat_;

		public int rootId_;

		public int pId_;

		public int tMode_;

		public Vector3D centerPos_;

		public float coatW_;

		public float coatH_;

		public TGroupArea[] tGroupAreas_;

		public int state_;

		public int stateCnt_;

		private static Vector3D st_gatherPos = new Vector3D();

		public TArea()
		{
			apdat_ = AppData.GetInstance();
			udat_ = UserData.GetInstance();
			centerPos_ = new Vector3D();
		}

		public void Init(int rootId, int pId)
		{
			rootId_ = rootId;
			pId_ = pId;
			state_ = 10;
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteInt(os, stateCnt_);
			StreamUtil.WriteShort(os, (short)tGroupAreas_.Length);
			for (int i = 0; i < tGroupAreas_.Length; i++)
			{
				tGroupAreas_[i].Serialize(os);
			}
		}

		public void Deserialize(InputStream stream)
		{
			state_ = StreamUtil.ReadShort(stream);
			stateCnt_ = StreamUtil.ReadInt(stream);
			int num = StreamUtil.ReadShort(stream);
			for (int i = 0; i < tGroupAreas_.Length; i++)
			{
				tGroupAreas_[i].Deserialize(stream, this);
			}
		}

		public void Deserialize2()
		{
			for (int i = 0; i < tGroupAreas_.Length; i++)
			{
				tGroupAreas_[i].Deserialize2();
			}
		}

		public void Update()
		{
			stateCnt_ = (stateCnt_ + 1) % int.MaxValue;
			if (state_ == 11)
			{
				for (int i = 0; i < tGroupAreas_.Length; i++)
				{
					for (int j = 0; j < tGroupAreas_[i].tPlayerList_.Size(); j++)
					{
						TPlayer tPlayer = (TPlayer)tGroupAreas_[i].tPlayerList_.ElementAt(j);
						tPlayer.Update_trainings();
					}
				}
				bool flag = true;
				for (int k = 0; k < tGroupAreas_.Length; k++)
				{
					if (!flag)
					{
						break;
					}
					for (int l = 0; l < tGroupAreas_[k].tPlayerList_.Size(); l++)
					{
						TPlayer tPlayer2 = (TPlayer)tGroupAreas_[k].tPlayerList_.ElementAt(l);
						int[] collisionData = udat_.GetCollisionData(0, 0);
						if (!GameUtil.ChRectHitRect(tPlayer2.pos_.x_ + (float)collisionData[0], tPlayer2.pos_.z_ + (float)collisionData[1], collisionData[2], collisionData[3], tGroupAreas_[k].centerPos_.x_ - tGroupAreas_[k].coatW_ / 2f, tGroupAreas_[k].centerPos_.z_ + tGroupAreas_[k].coatH_ / 2f, tGroupAreas_[k].coatW_, tGroupAreas_[k].coatH_, true))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					ChangeState(12);
				}
			}
			else if (state_ == 12)
			{
				for (int m = 0; m < tGroupAreas_.Length; m++)
				{
					for (int n = 0; n < tGroupAreas_[m].tPlayerList_.Size(); n++)
					{
						TPlayer tPlayer3 = (TPlayer)tGroupAreas_[m].tPlayerList_.ElementAt(n);
						tPlayer3.Update_trainings();
					}
					for (int num = 0; num < tGroupAreas_[m].tBallList_.Size(); num++)
					{
						TBall tBall = (TBall)tGroupAreas_[m].tBallList_.ElementAt(num);
						tBall.Update_training();
					}
				}
				for (int num2 = 0; num2 < tGroupAreas_.Length; num2++)
				{
					tGroupAreas_[num2].Update();
				}
			}
			else
			{
				if (state_ != 14)
				{
					return;
				}
				for (int num3 = 0; num3 < tGroupAreas_.Length; num3++)
				{
					for (int num4 = 0; num4 < tGroupAreas_[num3].tPlayerList_.Size(); num4++)
					{
						TPlayer tPlayer4 = (TPlayer)tGroupAreas_[num3].tPlayerList_.ElementAt(num4);
						tPlayer4.Update_trainings();
					}
					for (int num5 = 0; num5 < tGroupAreas_[num3].tBallList_.Size(); num5++)
					{
						TBall tBall2 = (TBall)tGroupAreas_[num3].tBallList_.ElementAt(num5);
						tBall2.Update_training();
					}
				}
				bool flag2 = true;
				for (int num6 = 0; num6 < tGroupAreas_.Length; num6++)
				{
					if (!flag2)
					{
						break;
					}
					for (int num7 = 0; num7 < tGroupAreas_[num6].tPlayerList_.Size(); num7++)
					{
						TPlayer tPlayer5 = (TPlayer)tGroupAreas_[num6].tPlayerList_.ElementAt(num7);
						int[] collisionData2 = udat_.GetCollisionData(0, 0);
						if (!GameUtil.ChRectHitRect(tPlayer5.pos_.x_ + (float)collisionData2[0], tPlayer5.pos_.z_ + (float)collisionData2[1], collisionData2[2], collisionData2[3], tGroupAreas_[num6].centerPos_.x_ - tGroupAreas_[num6].coatW_ / 2f, tGroupAreas_[num6].centerPos_.z_ + tGroupAreas_[num6].coatH_ / 2f, tGroupAreas_[num6].coatW_, tGroupAreas_[num6].coatH_, true))
						{
							flag2 = false;
							break;
						}
					}
				}
				if (flag2)
				{
					ChangeState(12);
				}
			}
		}

		public void Draw(Graphics g)
		{
			int num = rootId_;
			int num2 = 0;
			if (pId_ == 3)
			{
				num2 = 1;
			}
			for (int i = 0; i < tGroupAreas_.Length; i++)
			{
				for (int j = 0; j < tGroupAreas_[i].tPlayerList_.Size(); j++)
				{
					TPlayer tPlayer = (TPlayer)tGroupAreas_[i].tPlayerList_.ElementAt(j);
					tPlayer.Draw_training(g);
				}
				for (int k = 0; k < tGroupAreas_[i].tBallList_.Size(); k++)
				{
					TBall tBall = (TBall)tGroupAreas_[i].tBallList_.ElementAt(k);
					tBall.Draw_training(g);
				}
				tGroupAreas_[i].Draw_obj(g);
			}
		}

		public void ChangeState(int next_state)
		{
			state_ = next_state;
			stateCnt_ = 0;
			if (state_ == 10)
			{
				return;
			}
			if (state_ == 11)
			{
				for (int i = 0; i < tGroupAreas_.Length; i++)
				{
					tGroupAreas_[i].ChangeState(10);
				}
				for (int j = 0; j < tGroupAreas_.Length; j++)
				{
					for (int k = 0; k < tGroupAreas_[j].tPlayerList_.Size(); k++)
					{
						TPlayer tPlayer = (TPlayer)tGroupAreas_[j].tPlayerList_.ElementAt(k);
						int num = (j + k) % 2;
						tPlayer.pos_.Set(MapMain.MOVE_POINT[1 + num]);
						tPlayer.flag_ = BitUtil.Off(tPlayer.flag_, 1);
						tPlayer.AddCommandWait(udat_.trainig_appearWait);
						tPlayer.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[3 + num].x_, (int)MapMain.MOVE_POINT[3 + num].z_);
						if (pId_ == 0)
						{
							tPlayer.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[7].x_, (int)MapMain.MOVE_POINT[7].z_);
							tPlayer.AddCommandTenantIn();
						}
						else
						{
							if (rootId_ == 0)
							{
								tPlayer.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[5].x_, (int)MapMain.MOVE_POINT[5].z_);
							}
							else
							{
								tPlayer.AddCommandTrainigMoveGoalPosSet((int)MapMain.MOVE_POINT[6].x_, (int)MapMain.MOVE_POINT[6].z_);
							}
							Vector3D startTrainingGatherPos = GetStartTrainingGatherPos(tGroupAreas_[j], tPlayer);
							tPlayer.AddCommandTrainigMoveGoalPosSet((int)startTrainingGatherPos.x_, (int)startTrainingGatherPos.z_);
						}
						if (num == 1)
						{
							udat_.trainig_appearWait += 7;
						}
					}
				}
			}
			else if (state_ == 12)
			{
				for (int l = 0; l < tGroupAreas_.Length; l++)
				{
					tGroupAreas_[l].ChangeState(11);
				}
			}
			else if (state_ == 13)
			{
				for (int m = 0; m < tGroupAreas_.Length; m++)
				{
					tGroupAreas_[m].ChangeState(13);
				}
			}
			else if (state_ == 14)
			{
				for (int n = 0; n < tGroupAreas_.Length; n++)
				{
					tGroupAreas_[n].ChangeState(14);
				}
			}
		}

		public int GetAreaMaxNum(int tMode)
		{
			if ((tMode == 0)) { return 6; } else if ((tMode == 1)) { return 6; } else if ((tMode == 2)) { return 10; } else if ((tMode == 4)) { return 100; } else if ((tMode == 3)) { return 5; } else { return 0; }
		}

		public int GetAreaBallNum(int tMode)
		{
			if ((tMode == 0)) { return 1; } else if ((tMode == 1)) { return 2; } else if ((tMode == 2)) { return 3; } else if ((tMode == 3)) { return 3; } else { return 0; }
		}

		public void SetPlayerGatherPos()
		{
			int num = 0;
			for (int i = 0; i < tGroupAreas_.Length; i++)
			{
				for (int j = 0; j < tGroupAreas_[i].tPlayerList_.Size(); j++)
				{
					TPlayer tPlayer = (TPlayer)tGroupAreas_[i].tPlayerList_.ElementAt(j);
					int num2 = GameUtil.Random(100);
					int num3 = GameUtil.Random(100);
					if (tMode_ == 4)
					{
						tPlayer.goalPos_.x_ = tPlayer.pos_.x_;
						tPlayer.goalPos_.z_ = tPlayer.pos_.z_;
					}
					else
					{
						tPlayer.goalPos_.x_ = GameUtil.RateConvert(num2, 0f, 99f, tGroupAreas_[i].centerPos_.x_ - tGroupAreas_[i].coatW_ * 10f / 100f, tGroupAreas_[i].centerPos_.x_ + tGroupAreas_[i].coatW_ * 10f / 100f, true);
						tPlayer.goalPos_.z_ = GameUtil.RateConvert(num3, 0f, 99f, tGroupAreas_[i].centerPos_.z_ + tGroupAreas_[i].coatH_ * 5f / 100f, tGroupAreas_[i].centerPos_.z_ - tGroupAreas_[i].coatH_ * 5f / 100f, true);
					}
					num = udat_.trainig_appearWait;
					udat_.trainig_appearWait += 4;
					tPlayer.AddCommandWait(num);
					tPlayer.AddCommandTrainigMoveGoalPos();
				}
			}
		}

		public void SetTrainingArea(int tMode)
		{
			tMode_ = tMode;
			int num = rootId_ * 2;
			if (pId_ == 3)
			{
				num++;
			}
			if (tMode_ == 4)
			{
				centerPos_.x_ = (MapMain.TRAIN_COAT_BLOCK[num][0].x_ + MapMain.TRAIN_COAT_BLOCK[num][1].x_) / 2f;
				centerPos_.y_ = 0f;
				centerPos_.z_ = (MapMain.TRAIN_COAT_BLOCK[num][0].z_ + MapMain.TRAIN_COAT_BLOCK[num + 1][1].z_) / 2f;
				coatW_ = JMath.Abs(MapMain.TRAIN_COAT_BLOCK[num][0].x_ - MapMain.TRAIN_COAT_BLOCK[num][1].x_) + 100f;
				coatH_ = JMath.Abs(MapMain.TRAIN_COAT_BLOCK[num][0].z_ - MapMain.TRAIN_COAT_BLOCK[num + 1][1].z_) + 100f;
			}
			else if (tMode_ != 5)
			{
				centerPos_.x_ = (MapMain.TRAIN_COAT_BLOCK[num][0].x_ + MapMain.TRAIN_COAT_BLOCK[num][1].x_) / 2f;
				centerPos_.y_ = 0f;
				centerPos_.z_ = (MapMain.TRAIN_COAT_BLOCK[num][0].z_ + MapMain.TRAIN_COAT_BLOCK[num][1].z_) / 2f;
				coatW_ = JMath.Abs(MapMain.TRAIN_COAT_BLOCK[num][0].x_ - MapMain.TRAIN_COAT_BLOCK[num][1].x_);
				coatH_ = JMath.Abs(MapMain.TRAIN_COAT_BLOCK[num][0].z_ - MapMain.TRAIN_COAT_BLOCK[num][1].z_);
			}
			if (tMode_ == 0)
			{
				int num2 = 3;
				tGroupAreas_ = new TGroupArea[num2];
				for (int i = 0; i < tGroupAreas_.Length; i++)
				{
					tGroupAreas_[i] = new TGroupArea(i, this);
					tGroupAreas_[i].SetCoatData();
					tGroupAreas_[i].SetPosData();
				}
			}
			else if (tMode_ == 1)
			{
				int num3 = 3;
				tGroupAreas_ = new TGroupArea[num3];
				for (int j = 0; j < tGroupAreas_.Length; j++)
				{
					tGroupAreas_[j] = new TGroupArea(j, this);
					tGroupAreas_[j].SetCoatData();
					tGroupAreas_[j].SetPosData();
				}
			}
			else if (tMode_ == 2)
			{
				int num4 = 2;
				tGroupAreas_ = new TGroupArea[num4];
				for (int k = 0; k < tGroupAreas_.Length; k++)
				{
					tGroupAreas_[k] = new TGroupArea(k, this);
					tGroupAreas_[k].SetCoatData();
					tGroupAreas_[k].SetPosData();
				}
			}
			else if (tMode_ == 3)
			{
				int num5 = 1;
				tGroupAreas_ = new TGroupArea[num5];
				for (int l = 0; l < tGroupAreas_.Length; l++)
				{
					tGroupAreas_[l] = new TGroupArea(l, this);
					tGroupAreas_[l].SetCoatData();
					tGroupAreas_[l].SetPosData();
				}
			}
			else if (tMode_ == 4)
			{
				int num6 = 1;
				tGroupAreas_ = new TGroupArea[num6];
				for (int m = 0; m < tGroupAreas_.Length; m++)
				{
					tGroupAreas_[m] = new TGroupArea(m, this);
					tGroupAreas_[m].SetCoatData();
					tGroupAreas_[m].SetPosData();
				}
			}
			else if (tMode_ == 5)
			{
				int num7 = 1;
				tGroupAreas_ = new TGroupArea[num7];
				for (int n = 0; n < tGroupAreas_.Length; n++)
				{
					tGroupAreas_[n] = new TGroupArea(n, this);
				}
			}
		}

		public Vector3D GetStartTrainingGatherPos(TGroupArea tGroupArea, TPlayer tPlayer)
		{
			st_gatherPos.x_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, (0f - tGroupArea.coatW_) / 5f, tGroupArea.coatW_ / 5f, true) + tGroupArea.centerPos_.x_;
			st_gatherPos.y_ = 0f;
			st_gatherPos.z_ = GameUtil.RateConvert(GameUtil.Random(100), 0f, 99f, (0f - tGroupArea.coatH_) / 5f, tGroupArea.coatH_ / 5f, true) + tGroupArea.centerPos_.z_;
			return st_gatherPos;
		}
	}
}
