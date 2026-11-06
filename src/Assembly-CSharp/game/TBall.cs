using data;
using form;
using java.io;
using java.lang;
using kairo.unity.m3d4.util3d;
using kairo.unity.ui;
using kairo.unity.util;

namespace game
{
	public class TBall : Ball
	{
		public int tId_;

		public TGroupArea tGroupArea_;

		private int ownerId_;

		private int lastOwnerId_;

		public TPlayer t_owner_;

		public TPlayer t_lastOwner_;

		private static int[] camera_pos_shadow = new int[2];

		private static int[] camera_pos_dribble = new int[2];

		public new Vector3D g1 = new Vector3D();

		public new Vector3D shoot_goalPos = new Vector3D();

		public TBall(int tId)
		{
			tId_ = tId;
		}

		public void Init_training(TGroupArea tGroupArea)
		{
			tGroupArea_ = tGroupArea;
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteShort(os, (short)tId_);
			StreamUtil.WriteInt(os, frame_);
			StreamUtil.WriteShort(os, (short)direct_);
			StreamUtil.WriteVector3D(os, pos_);
			StreamUtil.WriteVector3D(os, shadowPos_);
			StreamUtil.WriteVector3D(os, v_);
			StreamUtil.WriteVector3D(os, a_);
			StreamUtil.WriteInt(os, screen_pos_[0]);
			StreamUtil.WriteInt(os, screen_pos_[1]);
			StreamUtil.WriteInt(os, screen_shadowPos_[0]);
			StreamUtil.WriteInt(os, screen_shadowPos_[1]);
			StreamUtil.WriteInt(os, camera_pos_[0]);
			StreamUtil.WriteInt(os, camera_pos_[1]);
			StreamUtil.WriteFloat(os, v_len_);
			StreamUtil.WriteInt(os, flag_);
			StreamUtil.WriteShort(os, (short)state_);
			StreamUtil.WriteInt(os, stateCnt_);
			if (t_owner_ == null)
			{
				StreamUtil.WriteShort(os, -1);
			}
			else
			{
				StreamUtil.WriteShort(os, (short)t_owner_.trainingId_[0]);
			}
			if (t_lastOwner_ == null)
			{
				StreamUtil.WriteShort(os, -1);
			}
			else
			{
				StreamUtil.WriteShort(os, (short)t_lastOwner_.trainingId_[0]);
			}
		}

		public void Deserialize(InputStream stream)
		{
			tId_ = StreamUtil.ReadShort(stream);
			frame_ = StreamUtil.ReadInt(stream);
			direct_ = StreamUtil.ReadShort(stream);
			pos_ = StreamUtil.ReadVector3D(stream);
			shadowPos_ = StreamUtil.ReadVector3D(stream);
			v_ = StreamUtil.ReadVector3D(stream);
			a_ = StreamUtil.ReadVector3D(stream);
			screen_pos_[0] = StreamUtil.ReadInt(stream);
			screen_pos_[1] = StreamUtil.ReadInt(stream);
			screen_shadowPos_[0] = StreamUtil.ReadInt(stream);
			screen_shadowPos_[1] = StreamUtil.ReadInt(stream);
			camera_pos_[0] = StreamUtil.ReadInt(stream);
			camera_pos_[1] = StreamUtil.ReadInt(stream);
			v_len_ = StreamUtil.ReadFloat(stream);
			flag_ = StreamUtil.ReadInt(stream);
			state_ = StreamUtil.ReadShort(stream);
			stateCnt_ = StreamUtil.ReadInt(stream);
			ownerId_ = StreamUtil.ReadShort(stream);
			lastOwnerId_ = StreamUtil.ReadShort(stream);
			t_owner_ = null;
			t_lastOwner_ = null;
		}

		public void Deserialize2()
		{
			if (ownerId_ != -1)
			{
				t_owner_ = UserData.GetInstance().tPlayers_[ownerId_];
			}
			if (lastOwnerId_ != -1)
			{
				t_lastOwner_ = UserData.GetInstance().tPlayers_[lastOwnerId_];
			}
		}

		public void Update_training()
		{
			frame_ = (frame_ + 1) % int.MaxValue;
			savePosFrame_ = (savePosFrame_ + 1) % int.MaxValue;
			bool flag = BitUtil.Check(flag_, 64);
			old_pos.Set(pos_);
			if (t_owner_ != null)
			{
				if (BitUtil.Check(t_owner_.flag_, 131072))
				{
					v_.Add(a_);
					pos_.Add(v_);
				}
				else
				{
					pos_.Set(t_owner_.pos_);
				}
			}
			else
			{
				if (a_.y_ == 0f || v_.y_ == 0f || pos_.y_ <= 0f)
				{
					float num = GameUtil.RateConvert(savePosFrame_, 0f, 60f, 0.99f, 0.93f, true);
					float num2 = GameUtil.RateConvert(savePosFrame_, 0f, 60f, 0.99f, 0.93f, true);
					v_.x_ *= num;
					v_.z_ *= num2;
				}
				else if (!flag)
				{
					float num;
					float num2;
					if (a_.y_ == -0.25f)
					{
						num = GameUtil.RateConvert(savePosFrame_, 0f, fallTime_ * 100 / 100, 0.995f, 0.96f, true);
						num2 = GameUtil.RateConvert(savePosFrame_, 0f, fallTime_ * 100 / 100, 0.995f, 0.96f, true);
					}
					else
					{
						num = GameUtil.RateConvert(savePosFrame_, 0f, fallTime_ * 100 / 100, 0.995f, 0.97f, true);
						num2 = GameUtil.RateConvert(savePosFrame_, 0f, fallTime_ * 100 / 100, 0.995f, 0.97f, true);
					}
					v_.x_ *= num;
					v_.y_ *= 0.995f;
					v_.z_ *= num2;
				}
				v_.Add(a_);
				pos_.Add(v_);
				v_len_ = (float)JMath.Sqrt(v_.x_ * v_.x_ + v_.z_ * v_.z_);
				if (tGroupArea_.tArea_.tMode_ == 3)
				{
					bool flag2 = false;
					int[] collisionData = udat_.GetCollisionData(0, 1);
					Vector3D[] goal_gpos = tGroupArea_.goal_gpos;
					if (tGroupArea_.tArea_.pId_ == 2)
					{
						if (GameUtil.ChRectHitRect(old_pos.x_ + (float)collisionData[0], old_pos.z_ + (float)collisionData[1], collisionData[2], collisionData[3], goal_gpos[3].x_, goal_gpos[3].z_, 80f, 24f, true) && old_pos.y_ < 15f)
						{
							flag2 = true;
						}
					}
					else if (GameUtil.ChRectHitRect(old_pos.x_ + (float)collisionData[0], old_pos.z_ + (float)collisionData[1], collisionData[2], collisionData[3], goal_gpos[0].x_, goal_gpos[0].z_, 80f, 24f, true) && old_pos.y_ < 15f)
					{
						flag2 = true;
					}
					if (flag2 && old_pos.x_ >= goal_gpos[0].x_ && old_pos.x_ <= goal_gpos[1].x_ && (pos_.x_ < goal_gpos[0].x_ || pos_.x_ > goal_gpos[1].x_))
					{
						if (pos_.x_ < goal_gpos[0].x_)
						{
							pos_.x_ = goal_gpos[0].x_;
						}
						if (pos_.x_ > goal_gpos[1].x_)
						{
							pos_.x_ = goal_gpos[1].x_;
						}
					}
					if (flag2 && tGroupArea_.tArea_.pId_ == 2)
					{
						if (old_pos.z_ <= goal_gpos[3].z_ && pos_.z_ > goal_gpos[3].z_)
						{
							pos_.z_ = goal_gpos[3].z_;
						}
					}
					else if (old_pos.z_ >= goal_gpos[3].z_ && pos_.z_ < goal_gpos[3].z_)
					{
						pos_.z_ = goal_gpos[3].z_;
					}
					if (flag2 && pos_.y_ > 15f)
					{
						pos_.y_ = 15f;
					}
				}
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
				if (v_.y_ < 0.3f)
				{
					SetStop();
				}
				else
				{
					SetSavePos(pos_, v_, a_);
				}
				targetPId = -1;
			}
			Camera.ConvertGlobalToAddress(pos_, address_);
			Camera.ConvertGlobalToScreen(pos_, screen_pos_);
			shadowPos_.x_ = pos_.x_;
			shadowPos_.z_ = pos_.z_;
			Camera.ConvertGlobalToScreen(shadowPos_, screen_shadowPos_);
		}

		public void Draw_training(Graphics g)
		{
			if (BitUtil.Check(flag_, 1))
			{
				return;
			}
			Camera.ConvertGlobalToCamera(pos_, camera_pos_);
			Camera.ConvertGlobalToCamera(shadowPos_, camera_pos_shadow);
			if (!GameUtil.ChRectHitRect(camera_pos_shadow[0] - 5, camera_pos_shadow[1] - 11, 11, 11, GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H, false))
			{
				return;
			}
			int num = camera_pos_shadow[1];
			bool dark = false;
			int frame = frame_ % 12 / 6;
			if (t_owner_ != null)
			{
				if (BitUtil.Check(t_owner_.flag_, 131072))
				{
					num += 3;
				}
				else if (t_owner_.seb_ == 14)
				{
					pos_.Set(t_owner_.pos_);
					direct_ = t_owner_.direct_;
					num += 3;
				}
				else
				{
					pos_.Set(t_owner_.pos_);
					direct_ = t_owner_.direct_;
					float num2 = 50f;
					float x = (float)JMath.Sqrt(t_owner_.v_.x_ * t_owner_.v_.x_ + t_owner_.v_.z_ * t_owner_.v_.z_);
					num2 = GameUtil.RateConvert(x, 0f, 10f, 0f, 100f, true);
					int c2min = (int)GameUtil.RateConvert(num2, 0f, 100f, Ball.DRIBBLE_DIRECT_DIS[direct_][0][0], Ball.DRIBBLE_DIRECT_DIS[direct_][1][0], true);
					int c2max = (int)GameUtil.RateConvert(num2, 0f, 100f, Ball.DRIBBLE_DIRECT_DIS[direct_][2][0], Ball.DRIBBLE_DIRECT_DIS[direct_][3][0], true);
					int c2min2 = (int)GameUtil.RateConvert(num2, 0f, 100f, Ball.DRIBBLE_DIRECT_DIS[direct_][0][1], Ball.DRIBBLE_DIRECT_DIS[direct_][1][1], true);
					int c2max2 = (int)GameUtil.RateConvert(num2, 0f, 100f, Ball.DRIBBLE_DIRECT_DIS[direct_][2][1], Ball.DRIBBLE_DIRECT_DIS[direct_][3][1], true);
					int num3 = frame_ % 16;
					int num4 = GameUtil.RateConvert(JMath.Abs(num3 - 8), 8, 0, c2min, c2max, true);
					int num5 = GameUtil.RateConvert(JMath.Abs(num3 - 8), 8, 0, c2min2, c2max2, true);
					float num6 = (float)JMath.Sqrt((camera_pos_dribble[0] - num4) * (camera_pos_dribble[0] - num4) + (camera_pos_dribble[1] - num5) * (camera_pos_dribble[1] - num5));
					int num7 = num4 * 3 / 4;
					int num8 = num5 * 3 / 4;
					camera_pos_dribble[0] = num7;
					camera_pos_dribble[1] = num8;
					camera_pos_[0] += num7;
					camera_pos_[1] += num8;
					camera_pos_shadow[0] += num7;
					camera_pos_shadow[1] += num8;
					if (rotate_ == 0 || num3 == 0)
					{
						rotate_ = (int)GameUtil.RateConvert(num2, 0f, 100f, 8f, 2f, true);
					}
					frame = frame_ % (rotate_ * 4) / rotate_;
					num = camera_pos_[1];
				}
			}
			int num10;
int num9 = 0;
			num10 = 0;
			if (t_owner_ != null)
			{
				if (t_owner_.seb_ != 14)
				{
					num = ((t_owner_.direct_ != 0 && t_owner_.direct_ != 3) ? (num + 1) : (num - 1));
				}
				else
				{
					num = ((t_owner_.team_id_ != 0) ? (num + 1) : (num + -1));
					int num11 = 0;
					if (t_owner_.team_id_ == 0)
					{
						if (t_owner_.direct_ == 3)
						{
							num11 = 0;
						}
						else if (t_owner_.direct_ == 0)
						{
							num11 = 1;
						}
						else if (t_owner_.direct_ == 1)
						{
							num11 = 2;
						}
					}
					else if (t_owner_.direct_ == 3)
					{
						num11 = 0;
					}
					else if (t_owner_.direct_ == 2)
					{
						num11 = 1;
					}
					else if (t_owner_.direct_ == 1)
					{
						num11 = 2;
					}
					num9 = Ball.SAVING_OFF[t_owner_.team_id_][num11][0];
					num10 = Ball.SAVING_OFF[t_owner_.team_id_][num11][1];
				}
			}
			if (t_owner_ == null)
			{
				apdat_.InsertGameObject(8, 0, num, 2, 0, 4, camera_pos_shadow[0], camera_pos_shadow[1] + 1, dark, 0);
			}
			apdat_.InsertGameObject(9, 0, num, 2, 0, frame, camera_pos_[0] + num9, camera_pos_[1] + num10, dark, 0);
		}

		public void ReceiveBall(TPlayer player)
		{
			a_.x_ = 0f;
			a_.z_ = 0f;
			player.flag_ = BitUtil.On(player.flag_, 4);
			player.ballHaveTime_ = 0;
			player.ballDribbleTime_ = 0;
			flag_ = BitUtil.On(flag_, 4);
			t_owner_ = player;
			pos_.Set(t_owner_.pos_);
			receive_wait = 0;
			flag_ = BitUtil.On(flag_, 8);
			boundNum_ = 0;
			flag_ = BitUtil.Off(flag_, 112);
			targetPId = -1;
		}

		public void ReleaseBall(TPlayer player)
		{
			if (!(BitUtil.Check(flag_, 4))) { return; }
{
player.flag_ = BitUtil.Off(player.flag_, 4);
player.ballHaveTime_ = 0;
flag_ = BitUtil.Off(flag_, 4);
t_lastOwner_ = t_owner_;
t_owner_ = null;
receive_wait = Ball.RECEIVE_WAIT_MIN;
flag_ = BitUtil.Off(flag_, 8);
boundNum_ = 0;
}
		}

		public void SetPassDataTraining(TPlayer player, int pass_mode, bool jamp)
		{
			v_.Set(0f, 0f, 0f);
			a_.Set(0f, 0f, 0f);
			TPlayer tPlayer = (TPlayer)player.tGroupArea_.tPlayerList_.ElementAt(1 - player.trainingId_[4]);
			temp_pos.x_ = tPlayer.pos_.x_;
			temp_pos.z_ = tPlayer.pos_.z_;
			float num = (float)JMath.Sqrt((temp_pos.x_ - pos_.x_) * (temp_pos.x_ - pos_.x_) + (temp_pos.z_ - pos_.z_) * (temp_pos.z_ - pos_.z_));
			float passSpeed = PlayerData.GetPassSpeed(player);
			v_.x_ = passSpeed * (temp_pos.x_ - pos_.x_) / num;
			v_.y_ = 0f;
			v_.z_ = passSpeed * (temp_pos.z_ - pos_.z_) / num;
			SetSavePos(pos_, v_, a_);
			receive_wait = Ball.RECEIVE_WAIT_MIN;
		}

		public void SetShoot(Player player, Vector3D goal_center)
		{
			v_.Set(0f, 0f, 0f);
			a_.Set(0f, 0f, 0f);
			int num = GameUtil.Random(100);
			g1.x_ = GameUtil.RateConvert(num, 0f, 99f, goal_center.x_ - 40f, goal_center.x_ + 40f, true);
			g1.z_ = goal_center.z_;
			int playerBodyPoseEva = PlayerData.GetPlayerBodyPoseEva(player, 10);
			float shootSpeed = PlayerData.GetShootSpeed(player, playerBodyPoseEva);
			float num2 = (float)JMath.Sqrt((g1.x_ - pos_.x_) * (g1.x_ - pos_.x_) + (g1.z_ - pos_.z_) * (g1.z_ - pos_.z_));
			v_.x_ = shootSpeed * (g1.x_ - pos_.x_) / num2;
			v_.z_ = shootSpeed * (g1.z_ - pos_.z_) / num2;
			int eva = playerBodyPoseEva;
			float shootV0y = PlayerData.GetShootV0y(player, eva);
			v_.y_ = shootV0y;
			a_.y_ = -0.1f;
			shoot_goalPos.Set(g1);
			flag_ = BitUtil.On(flag_, 48);
			SetSavePos(pos_, v_, a_);
		}

		public bool ChBallDirectHere(TPlayer tPlayer)
		{
			if (BitUtil.Check(flag_, 32))
			{
				if (tPlayer.tGroupArea_.tArea_.pId_ == 2)
				{
					if (pos_.z_ < tPlayer.pos_.z_ && v_.z_ > 0f)
					{
						return true;
					}
				}
				else if (pos_.z_ > tPlayer.pos_.z_ && v_.z_ < 0f)
				{
					return true;
				}
			}
			return false;
		}

		public override int GetToGoalLineTime(Player player)
		{
			if (v_.z_ == 0f)
			{
				return -1;
			}
			return (int)((g1.z_ - pos_.z_) / v_.z_);
		}
	}
}
