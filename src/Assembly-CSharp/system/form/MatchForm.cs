using System;
using form;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.io;
using kairo.unity.system;
using kairo.unity.ui;
using kairo.unity.util;
using main;

namespace system.form
{
	public class MatchForm : MyFormBase, JRunnable
	{
		private class UserRecord
		{
			public int rank_;

			public int friendId_;

			public int number_;

			public int visitor_;

			public string innName_;

			public int year_;

			public int month_;

			public int week_;

			public long money_;

			public string country_;

			public string name_;

			public Image image_;
		}

		public const int FORMID_MATCH = 904;

		public const int TYPE_MATCHING = 0;

		public const int TYPE_RANKING = 1;

		private const int THTYPE_MATCHING = 0;

		private const int THTYPE_RANKING = 1;

		private const int THTYPE_DOWNLOAD = 2;

		private const int THSTATE_NONE = 0;

		private const int THSTATE_RUN = 1;

		private const int THSTATE_SUCCESS = 2;

		private const int THSTATE_FAILURE = 3;

		public int type_;

		private static readonly int[][] configData_ = new int[2][]
		{
			new int[3] { 0, 2, 13 },
			new int[3] { 0, 2, 13 }
		};

		public bool untilActiveHide_;

		private int select_;

		private int command_;

		private int x_;

		private int y_;

		private int loopCounter_;

		private new bool draw_;

		private UserRecord[] userList_ = new UserRecord[0];

		private UserRecord dlUser_;

		internal int USER_MAX = 3;

		internal int TOPRANK_MAX = 7;

		private UserRecord[][] rankList_ = JTool.MakeArray<UserRecord[][]>(new int[2] { 2, 0 });

		private int threadType_;

		private int threadState_;

		private string error_;

		private JThread th_;

		private static byte[] upload_;

		public MatchForm()
			: base(904)
		{
			untilActiveHide_ = true;
		}

		public MatchForm(int type)
			: base(904)
		{
			type_ = type;
			select_ = 0;
			untilActiveHide_ = true;
		}

		public override string ToString()
		{
			return "MatchForm:" + type_;
		}

		public override bool Init()
		{
			base.Init();
			if (type_ != 0 && type_ == 1)
			{
				ThreadStart(1);
			}
			SetSoftlabel();
			return true;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			int num = x_;
			int num2 = y_;
			if (formMan_.GetForm(0) == apdat_.frmGame_)
			{
				num2 += configData_[type_][2];
			}
			if (view_.GetGameWidth() > 240 || view_.GetGameHeight() > 240)
			{
				num += (view_.GetGameWidth() - 240) / 2;
				num2 += (view_.GetGameHeight() - 240) / 2;
			}
			g.SetOrigin(num, num2);
			try
			{
				_draw(g);
			}
			catch (Exception)
			{
			}
			finally
			{
				g.SetOrigin(0, 0);
			}
		}

		private void _draw(Graphics g)
		{
			if (untilActiveHide_)
			{
				return;
			}
			if (!apdat_.IsTopForm(this))
			{
				FormBase topForm = formMan_.GetTopForm();
				if (topForm.GetId() != 4)
				{
					return;
				}
				SubForm subForm = (SubForm)topForm;
				if (!subForm.IsMenu() && subForm.type_ != type_)
				{
					return;
				}
				for (int num = formMan_.GetFormsNum() - 2; num >= 0; num--)
				{
					topForm = formMan_.GetForm(num);
					if (topForm == this)
					{
						break;
					}
					if (topForm.GetId() != 4 || !((SubForm)topForm).IsMenu())
					{
						return;
					}
				}
			}
			if (!Active())
			{
				return;
			}
			if (threadState_ != 0 && !draw_)
			{
				ConnectForm.DrawConnect(g);
				return;
			}
			draw_ = true;
			if (type_ != 0 && type_ != 1)
			{
			}
		}

		public override bool Finish()
		{
			base.Finish();
			return true;
		}

		public override bool Resume()
		{
			base.Resume();
			return true;
		}

		public override bool Update()
		{
			base.Update();
			bool result = _update();
			SetSoftlabel();
			return result;
		}

		private bool _update()
		{
			untilActiveHide_ = false;
			if (threadState_ != 0)
			{
				if (threadState_ == 1)
				{
					return true;
				}
				if (threadState_ == 2)
				{
					threadState_ = 0;
					if (threadType_ == 2)
					{
						apdat_.frmGame_.Init();
						return Pop();
					}
				}
				else if (threadState_ == 3)
				{
					threadState_ = 0;
					if (Config.PRINT)
					{
						Log.Info(error_);
					}
					Push(SubForm.MakeDialog(error_));
					if (type_ == 1)
					{
						return Pop();
					}
				}
			}
			loopCounter_ = (loopCounter_ + 1) % int.MaxValue;
			apdat_.resCom2_.seb[61].Frame();
			apdat_.resCom2_.seb[60].Frame();
			if (type_ == 0)
			{
				int num = USER_MAX + 1;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num) % num;
				}
				else if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1 + num) % num;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ == num - 1)
					{
						if (command_ == 0)
						{
							ThreadStart(0);
						}
						else if (command_ == 1)
						{
							return Push(new MatchForm(1));
						}
					}
					else if (select_ < userList_.Length)
					{
						dlUser_ = userList_[select_];
						ThreadStart(2);
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 1 && apdat_.IsPushSoftLabel(2))
			{
				return Pop();
			}
			return true;
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, int anchor, bool active, int margin, bool cursor)
		{
			return apdat_.DrawCommand(g, str, x, y, anchor, active, margin, cursor, this, loopCounter_);
		}

		public override int GetTouchValue(int type, int index)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 131072:
				return select_;
			case 196608:
				return command_;
			default:
				return base.GetTouchValue(type, index);
			}
		}

		public override void SetTouchValue(int type, int value)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 131072:
				select_ = value;
				break;
			case 196608:
				command_ = value;
				break;
			default:
				base.SetTouchValue(type, value);
				break;
			}
		}

		public override bool CheckMainWindow()
		{
			return false;
		}

		public void SetSoftlabel()
		{
			int num = configData_[type_][0];
			int num2 = configData_[type_][1];
			if (threadState_ != 0)
			{
				num = (num2 = 0);
			}
			SetSoftLabel(AppData.softLabels_[num], AppData.softLabels_[num2]);
		}

		public void ThreadStart(int type)
		{
			threadType_ = type;
			threadState_ = 1;
			error_ = null;
			th_ = new JThread(this);
			th_.Start();
		}

		public void Run()
		{
			try
			{
				KairoService instance = KairoService.GetInstance();
				if (!instance.IsLogin())
				{
					instance.ConnectLogin(true);
				}
				if (upload_ != null)
				{
					string text = apdat_.save_.ints[1] + "," + apdat_.save_.ints[2] + "," + apdat_.save_.ints[3];
					GameDataRecord gameDataRecord = new GameDataRecord(0);
					gameDataRecord.Add("v0", apdat_.save_.ints[1]);
					gameDataRecord.Add("s0", apdat_.save_.strs[0]);
					gameDataRecord.Add("s1", text + "," + apdat_.save_.lngs[0]);
					gameDataRecord.Add("d0", upload_);
					gameDataRecord.Add("d1", RecordStore.ReadRecord(0, 1 + AppData.currentSaveSlot_));
					gameDataRecord.Add("is_alive", 1L);
					instance.ConnectGameData(gameDataRecord);
					upload_ = null;
				}
				if (threadType_ == 0)
				{
					string get = "friendid, number, v0";
					string text2 = "pid=" + Config.PID + ", ban=0, is_alive=1, reg_friend=1";
					GameDataRecord[] array = instance.ConnectGameDataSearch(get, text2, null, "v0", apdat_.save_.ints[1], apdat_.save_.ints[1] - 5, apdat_.save_.ints[1] + 5, "50,50", 2);
					int num = JInteger.ParseInt(instance.GetFriendId());
					int[] array2 = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						int friendId = array[i].GetFriendId();
						if (friendId != num)
						{
							array2[i] = 100;
						}
					}
					Vector vector = new Vector();
					for (int j = 0; j < USER_MAX; j++)
					{
						int num2 = 0;
						for (int k = 0; k < array2.Length; k++)
						{
							num2 += array2[k];
						}
						if (num2 <= 0)
						{
							break;
						}
						int num3 = AppData.Random(num2);
						num2 = 0;
						for (int l = 0; l < array2.Length; l++)
						{
							num2 += array2[l];
							if (num3 < num2)
							{
								vector.AddElement(array[l]);
								array2[l] = 0;
								break;
							}
						}
					}
					GameDataRecord[] array3 = new GameDataRecord[vector.Size()];
					for (int m = 0; m < vector.Size(); m++)
					{
						GameDataRecord gameDataRecord2 = (GameDataRecord)vector.ElementAt(m);
						GameDataRecord gameDataRecord3 = new GameDataRecord(gameDataRecord2.GetFriendId(), gameDataRecord2.GetNumber());
						gameDataRecord3.Add("v1");
						gameDataRecord3.Add("s0");
						gameDataRecord3.Add("s1");
						gameDataRecord3.Add("d0");
						gameDataRecord3.Add("country");
						gameDataRecord3.Add("t_user.name");
						array3[m] = gameDataRecord3;
					}
					array3 = instance.ConnectGameData(array3);
					UserRecord[] array4 = new UserRecord[array3.Length];
					for (int n = 0; n < array4.Length; n++)
					{
						GameDataRecord gameDataRecord4 = array3[n];
						UserRecord userRecord = new UserRecord();
						userRecord.friendId_ = gameDataRecord4.GetFriendId();
						userRecord.number_ = gameDataRecord4.GetNumber();
						userRecord.visitor_ = (int)gameDataRecord4.GetValue("v1");
						userRecord.innName_ = gameDataRecord4.GetString("s0");
						userRecord.country_ = gameDataRecord4.GetString("country");
						userRecord.name_ = gameDataRecord4.GetString("t_user.name");
						userRecord.image_ = Image.Load(gameDataRecord4.GetData("d0"));
						string[] array5 = StringUtil.Split(gameDataRecord4.GetString("s1"), ",");
						userRecord.year_ = JInteger.ParseInt(array5[0]);
						userRecord.money_ = JInteger.ParseInt(array5[1]);
						userRecord.week_ = JInteger.ParseInt(array5[2]);
						userRecord.money_ = JLong.ParseLong(array5[3]);
						array4[n] = userRecord;
					}
					userList_ = array4;
				}
				else if (threadType_ == 1)
				{
					string get2 = "friendid, number, v0, t_user.name, s0";
					string text3 = "pid=" + Config.PID + ", ban=0, is_alive=1";
					GameDataRecord[][] array6 = instance.ConnectGameDataRanking(instance.GetFriendId(), get2, text3, "v0", JString.ValueOf(TOPRANK_MAX), true, null);
					for (int num4 = 0; num4 < array6.Length; num4++)
					{
						rankList_[num4] = new UserRecord[array6[num4].Length];
						for (int num5 = 0; num5 < rankList_[num4].Length; num5++)
						{
							GameDataRecord gameDataRecord5 = array6[num4][num5];
							UserRecord userRecord2 = (rankList_[num4][num5] = new UserRecord());
							userRecord2.friendId_ = gameDataRecord5.GetFriendId();
							userRecord2.number_ = gameDataRecord5.GetNumber();
							userRecord2.visitor_ = (int)gameDataRecord5.GetValue("v0");
							userRecord2.name_ = gameDataRecord5.GetString("t_user.name");
							userRecord2.innName_ = gameDataRecord5.GetString("s0");
							userRecord2.rank_ = (int)gameDataRecord5.GetValue("_rank");
							if (userRecord2.name_ == null)
							{
								userRecord2.name_ = "--";
							}
						}
					}
				}
				else if (threadType_ == 2)
				{
					GameDataRecord gameDataRecord6 = new GameDataRecord(dlUser_.friendId_, dlUser_.number_);
					gameDataRecord6.Add("v1", "v1+1");
					gameDataRecord6.Add("d1");
					gameDataRecord6.Add("v1");
					gameDataRecord6 = instance.ConnectGameData(gameDataRecord6);
					byte[] data = gameDataRecord6.GetData("d1");
					RecordStore.WriteRecord(0, 3 + AppData.currentSaveSlot_, data);
					apdat_.LoadGame(AppData.currentSaveSlot_, true);
				}
				threadState_ = 2;
			}
			catch (Exception)
			{
				error_ = Http.PopErrorText();
				threadState_ = 3;
				th_ = null;
			}
			finally
			{
				th_ = null;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
