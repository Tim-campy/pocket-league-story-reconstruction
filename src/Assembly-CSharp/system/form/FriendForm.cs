using System;
using form;
using java.lang;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.graph;
using kairo.unity.io;
using kairo.unity.surface;
using kairo.unity.system;
using kairo.unity.ui;
using kairo.unity.util;
using main;
using surface;
using system.friend;
using system.friend.ui;
using system.panel;

namespace system.form
{
	public class FriendForm : MyFormBase
	{
		public const int FORMID_FRIEND = 901;

		public const int TYPE_INPUT_NAME = 0;

		public const int TYPE_RENAME = 1;

		public const int TYPE_FRIEND_LIST = 2;

		public const int TYPE_SEARCH = 3;

		public const int TYPE_APPLY = 4;

		public const int TYPE_APPLY_CONFIRM = 5;

		public const int TYPE_APPROVE = 6;

		public const int TYPE_APPROVE2 = 7;

		public const int TYPE_REFUSE = 8;

		public const int TYPE_REFUSE2 = 9;

		public const int TYPE_ERROR_MSG = 10;

		public const int TYPE_ABOUT_FRIEND = 11;

		public const int TYPE_PLAY_BONUS1 = 12;

		public const int TYPE_PLAY_BONUS2 = 13;

		public const int TYPE_NEW_FRIEND_BONUS1 = 14;

		public const int TYPE_NEW_FRIEND_BONUS2 = 15;

		private const int PLAY_BONUS_END_FRAME = 24;

		private const int PLAY_BONUS2_END_FRAME = 50;

		private const int FRIEND_LIST_MAX = 5;

		public const int RESFRIEND_SEB_TRIAL = 0;

		public const int RESFRIEND_SEB_STUDENT = 1;

		public const int RESFRIEND_SEB_OBJ01 = 2;

		public const int RESFRIEND_SEB_EXCLAMATION_MARK00 = 3;

		public const int RESFRIEND_SEB_EFFECT00 = 4;

		public const int RESFRIEND_SEB_FOUNDATION = 5;

		public const int RESFRIEND_SEB_JUMP_ASSIST00 = 6;

		public const int RESFRIEND_SEB_JUMP_ASSIST01 = 7;

		public const int RESFRIEND_SEB_TALK_ASSIST00D = 8;

		public const int RESFRIEND_SEB_TALK_ASSIST00R = 9;

		public const int RESFRIEND_SEB_TALK_ASSIST01D = 10;

		public const int RESFRIEND_SEB_TALK_ASSIST01R = 11;

		public const int RESFRIEND_SEB_CLAP_ASSIST00D = 12;

		public const int RESFRIEND_SEB_CLAP_ASSIST00R = 13;

		public const int RESFRIEND_SEB_CLAP_ASSIST01D = 14;

		public const int RESFRIEND_SEB_CLAP_ASSIST01R = 15;

		public const int RESFRIEND_SEB_TICKET = 16;

		public const int RESFRIEND_SEB_NUM_TICKET = 17;

		public const int RESFRIEND_SEB_GET = 18;

		public int type_;

		private static readonly int[][] softLabels_ = new int[16][]
		{
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2] { 0, 2 },
			new int[2],
			new int[2],
			new int[2],
			new int[2]
		};

		private int frame_;

		private int select_;

		private int select2_;

		private int page_;

		private int value1_;

		private int loopCounter_;

		private FriendForm next_;

		private string text_;

		private string text2_;

		private TextLayout textLay_;

		private FriendData friendData_;

		public Friend friend_;

		private JThread th_;

		public static ResourceManager resFriend_;

		public static int COLOR_WHITE = Graphics.GetColorOfRGB(255, 255, 255);

		public static int COLOR_BLACK = Graphics.GetColorOfRGB(0, 0, 0);

		public static int COLOR_RED = Graphics.GetColorOfRGB(255, 0, 0);

		public static int COLOR_GREEN = Graphics.GetColorOfRGB(0, 255, 0);

		public static int COLOR_BLUE = Graphics.GetColorOfRGB(0, 0, 255);

		public static int COLOR_AQUA = Graphics.GetColorOfRGB(0, 128, 255);

		private static int[] drawRandomFriend_result = new int[4];

		public FriendForm()
			: base(901)
		{
		}

		public FriendForm(int type)
			: base(901)
		{
			type_ = type;
			select_ = 0;
		}

		public override string ToString()
		{
			return "FriendForm:" + type_;
		}

		public static FriendForm MakeErrorMsgForm(string msg, string msg_sub)
		{
			FriendForm friendForm = new FriendForm();
			friendForm.type_ = 10;
			friendForm.textLay_ = new TextLayout();
			friendForm.textLay_.SetSize(140f, 44f);
			friendForm.textLay_.SetColor(COLOR_BLACK);
			friendForm.textLay_.SetAnchor(34);
			friendForm.textLay_.SetLineSpace(4);
			friendForm.textLay_.SetText(msg);
			friendForm.textLay_.Flush();
			friendForm.text_ = msg_sub;
			return friendForm;
		}

		internal static void LoadResource()
		{
			if (resFriend_ != null)
			{
				return;
			}
			try
			{
				resFriend_ = new ResourceManager(null, null, null, null, null, null, null).SetRecordNameBits(0);
				resFriend_.Load(Storage.Read(3, false, "friend.dat"));
			}
			catch (Exception)
			{
				resFriend_ = null;
			}
		}

		public override bool Init()
		{
			base.Init();
			friendData_ = FriendData.GetInstance();
			SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[0]);
			LoadResource();
			int num = softLabels_[type_][0];
			int num2 = softLabels_[type_][1];
			if (type_ == 0)
			{
				text_ = string.Empty;
			}
			else if (type_ == 1)
			{
				text_ = SecureString.ToString(MyFriend.GetInstance().friSave_.strs[0]);
			}
			else if (type_ == 2)
			{
				th_ = new JThread(delegate
				{
					try
					{
						KairoBase instance = KairoBase.GetInstance();
						instance.Save();
						if (friendData_.previousUpdateMinute_.Get() != friendData_.friSave_.ints[1].Get())
						{
							friendData_.CgiRegistFriend(null, friendData_.friSave_.ints[1].Get());
							friendData_.previousUpdateMinute_ = new SecureInt(friendData_.friSave_.ints[1]);
						}
						friendData_.RefreshFriendList();
					}
					catch (Exception t)
					{
						if (Config.PRINT)
						{
							Log.Info(t);
						}
						Push(MakeErrorDialog());
						Pop();
					}
					finally
					{
						th_ = null;
					}
				});
				th_.Start();
				if (!BitUtil.Check(friendData_.friSave_.ints[0].Get(), 1))
				{
					friendData_.friSave_.ints[0] = new SecureInt(BitUtil.On(friendData_.friSave_.ints[0].Get(), 1));
					Push(new FriendForm(11));
				}
			}
			else if (type_ == 3)
			{
				text_ = string.Empty;
			}
			else if (type_ == 4) { } else if (type_ == 5) { } else if (type_ == 6) { } else if (type_ == 7) { } else if (type_ == 8) { } else if (type_ == 9) { } else if (type_ == 10) { } else {
				if (type_ == 11)
				{
					string value = KairoText.LT("全国に散らばるユーザーを<br>フレンドとして最大<0>人まで<br>登録できます。<br>フレンドの合計ﾌﾟﾚｲ時間に<br>よって起動時にボーナスが<br>入ります。IDを聞いて友達<br>に申請してみましょう。");
					textLay_ = new TextLayout();
					textLay_.SetSize(204f, 128f);
					textLay_.SetColor(COLOR_BLACK);
					textLay_.SetAnchor(34);
					textLay_.SetLineSpace(4);
					textLay_.SetText(StringUtil.Replace(value, JString.ValueOf(friendData_.FRIEND_MAX)));
					textLay_.Flush();
				}
				else if (type_ == 12) { } else if ((type_ == 13) || (type_ == 15)) { } else {
				}
			}
			if (th_ != null)
			{
				SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[0]);
			}
			else
			{
				SetSoftLabel(AppData.softLabels_[num], AppData.softLabels_[num2]);
			}
			return true;
		}

		public override bool Update()
		{
			try
			{
				return _update();
			}
			finally
			{
				if (th_ != null)
				{
					SetSoftLabel(AppData.softLabels_[0], AppData.softLabels_[0]);
				}
				else
				{
					SetSoftLabel(AppData.softLabels_[softLabels_[type_][0]], AppData.softLabels_[softLabels_[type_][1]]);
				}
			}
		}

		private bool _update()
		{
			base.Update();
			FormManager instance = FormManager.GetInstance();
			loopCounter_ = (loopCounter_ + 1) % int.MaxValue;
			if (th_ != null)
			{
				return true;
			}
			if (type_ == 0)
			{
				int num = 2;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num) % num;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ == 0)
					{
						SetSoftLabel(string.Empty, string.Empty);
						canvas_.UpdateSoftLabel();
						try
						{
							InputPanel inputPanel = new InputPanel(KairoText.LT("ﾆｯｸﾈｰﾑ"), text_, 12);
							inputPanel.SetItemName(KairoText.LT("ﾆｯｸﾈｰﾑ ").Trim());
							inputPanel.Show(canvas_, delegate(string ret)
							{
								if (ret != null && ret.Length() > 0)
								{
									text_ = ret;
								}
							});
						}
						finally
						{
							SetSoftLabel(softLabels_[type_][0], softLabels_[type_][1]);
							formMan_.DrawAllForms();
						}
					}
					else if (select_ == 1)
					{
						if (text_.Length() == 0)
						{
							return Push(SubForm.MakeDialog(KairoText.LT("ﾆｯｸﾈｰﾑを入力してください")));
						}
						if (text_.Trim().Length() == 0)
						{
							return Push(SubForm.MakeDialog(KairoText.LT("空白以外で入力してください")));
						}
						if (!FriendData.IsEffectiveRegistName(text_))
						{
							return Push(SubForm.MakeDialog(KairoText.LT("利用できない文字があります")));
						}
						th_ = new JThread(delegate
						{
							try
							{
								friendData_.CgiRegistFriend(text_, friendData_.friSave_.ints[1].Get());
								friendData_.SetFriendName(text_);
								KairoBase instance7 = KairoBase.GetInstance();
								instance7.Save();
								bool flag9 = false;
								FormManager instance8 = FormManager.GetInstance();
								for (int i = 0; i < instance8.GetFormsNum(); i++)
								{
									if (instance8.GetForm(i).GetId() == 0)
									{
										flag9 = true;
										break;
									}
								}
								if (!flag9)
								{
									Push(new FriendForm(2));
								}
								Pop();
							}
							catch (Exception t2)
							{
								if (Config.PRINT)
								{
									Log.Info(t2);
								}
								Push(MakeErrorDialog());
							}
							finally
							{
								th_ = null;
							}
						});
						th_.Start();
						return true;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 1)
			{
				int num2 = 2;
				if (canvas_.CheckKeyRepeat(131072))
				{
					select_ = (select_ - 1 + num2) % num2;
				}
				if (canvas_.CheckKeyRepeat(524288))
				{
					select_ = (select_ + 1) % num2;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ == 0)
					{
						SetSoftLabel(string.Empty, string.Empty);
						canvas_.UpdateSoftLabel();
						try
						{
							InputPanel inputPanel2 = new InputPanel(KairoText.LT("ﾆｯｸﾈｰﾑ"), text_, 12);
							inputPanel2.SetItemName(KairoText.LT("ﾆｯｸﾈｰﾑ ").Trim());
							inputPanel2.Show(canvas_, delegate(string ret)
							{
								if (ret != null && ret.Length() > 0)
								{
									text_ = ret;
								}
							});
						}
						finally
						{
							SetSoftLabel(softLabels_[type_][0], softLabels_[type_][1]);
							formMan_.DrawAllForms();
						}
					}
					else if (select_ == 1)
					{
						if (text_.Length() == 0)
						{
							return Push(SubForm.MakeDialog(KairoText.LT("ﾆｯｸﾈｰﾑを入力してください")));
						}
						if (text_.Trim().Length() == 0)
						{
							return Push(SubForm.MakeDialog(KairoText.LT("空白以外で入力してください")));
						}
						if (!FriendData.IsEffectiveRegistName(text_))
						{
							return Push(SubForm.MakeDialog(KairoText.LT("利用できない文字があります")));
						}
						if (text_.Equals(SecureString.ToString(friendData_.friSave_.strs[0])))
						{
							return Pop();
						}
						th_ = new JThread(delegate
						{
							try
							{
								friendData_.CgiRegistFriend(text_, -1L);
								friendData_.friSave_.strs[0] = SecureString.ValueOf(text_);
								KairoBase instance7 = KairoBase.GetInstance();
								instance7.Save();
								Pop();
							}
							catch (Exception t2)
							{
								if (Config.PRINT)
								{
									Log.Info(t2);
								}
								Push(MakeErrorDialog());
							}
							finally
							{
								th_ = null;
							}
						});
						th_.Start();
						return true;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 2)
			{
				int num3 = 6;
				if (canvas_.CheckKeyPulse(131072))
				{
					select_ = (select_ - 1 + num3) % num3;
					if (select_ < 5 && page_ * 5 + select_ >= friendData_.FRIEND_MAX)
					{
						select_ = (friendData_.FRIEND_MAX - 1) % 5;
					}
				}
				if (canvas_.CheckKeyPulse(524288))
				{
					select_ = (select_ + 1 + num3) % num3;
					if (select_ < 5 && page_ * 5 + select_ >= friendData_.FRIEND_MAX)
					{
						select_ = 5;
					}
				}
				if (select_ < 5)
				{
					int num4 = (friendData_.FRIEND_MAX + 5 - 1) / 5;
					if (canvas_.CheckKeyPulse(65536))
					{
						page_ = (page_ - 1 + num4) % num4;
					}
					if (canvas_.CheckKeyPulse(262144))
					{
						page_ = (page_ + 1) % num4;
					}
					if (select_ < 5 && page_ * 5 + select_ >= friendData_.FRIEND_MAX)
					{
						select_ = (friendData_.FRIEND_MAX - 1) % 5;
					}
				}
				else
				{
					int num5 = 2;
					if (canvas_.CheckKeyPulse(65536))
					{
						select2_ = (select2_ - 1 + num5) % num5;
					}
					if (canvas_.CheckKeyPulse(262144))
					{
						select2_ = (select2_ + 1) % num5;
					}
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ < 5)
					{
						int num6 = page_ * 5 + select_;
						Friend friend = null;
						bool flag = false;
						if (friendData_.applyList_.Size() > 0 && friendData_.friendList_.Size() < friendData_.FRIEND_MAX)
						{
							if (friendData_.FRIEND_MAX <= 5)
							{
								if (num6 == friendData_.FRIEND_MAX - 1)
								{
									flag = true;
								}
								else if (num6 < friendData_.friendList_.Size())
								{
									friend = (Friend)friendData_.friendList_.ElementAt(num6);
								}
							}
							else if (num6 == 0)
							{
								flag = true;
							}
							else if (num6 - 1 < friendData_.friendList_.Size())
							{
								friend = (Friend)friendData_.friendList_.ElementAt(num6 - 1);
							}
						}
						else if (num6 < friendData_.friendList_.Size())
						{
							friend = (Friend)friendData_.friendList_.ElementAt(num6);
						}
						if (friend != null)
						{
							FriendForm friendForm = new FriendForm(8);
							friendForm.friend_ = friend;
							return Push(friendForm);
						}
						if (flag)
						{
							FriendForm friendForm2 = new FriendForm(6);
							friendForm2.friend_ = (Friend)friendData_.applyList_.FirstElement();
							return Push(friendForm2);
						}
						if (num6 < friendData_.FRIEND_MAX)
						{
							return Push(new FriendForm(3));
						}
					}
					else
					{
						if (select2_ == 0)
						{
							return Push(new FriendForm(11));
						}
						if (select2_ == 1)
						{
							try
							{
								FriendData.ActionSendMail(friendData_.INTRODUCTION_SUBJECT, StringUtil.Replace(friendData_.INTRODUCTION_TEXT, friendData_.GetFriendID()));
							}
							catch (Exception)
							{
								return Push(MakeErrorDialog());
							}
						}
					}
				}
				else if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 3)
			{
				int num7 = 2;
				if (canvas_.CheckKeyPulse(131072))
				{
					select_ = (select_ - 1 + num7) % num7;
				}
				if (canvas_.CheckKeyPulse(524288))
				{
					select_ = (select_ + 1) % num7;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select2_ != 0)
					{
						select2_ = 0;
						try
						{
							string friendID = friendData_.GetFriendID();
							FriendData.ActionSendMail(friendData_.INTRODUCTION_SUBJECT, StringUtil.Replace(friendData_.INTRODUCTION_TEXT, friendID));
						}
						catch (Exception)
						{
							return Push(MakeErrorDialog());
						}
					}
					else if (select_ == 0)
					{
						SetSoftLabel(string.Empty, string.Empty);
						canvas_.UpdateSoftLabel();
						try
						{
							InputPanel inputPanel3 = new InputPanel(KairoText.LT("フレンドのID"), text_, 9, 0, KairoText.LT("フレンドのID ").Trim());
							inputPanel3.Show(canvas_, delegate(string ret)
							{
								if (ret != null && ret.Length() > 0)
								{
									text_ = ret;
								}
							});
						}
						finally
						{
							SetSoftLabel(softLabels_[type_][0], softLabels_[type_][1]);
							formMan_.DrawAllForms();
						}
					}
					else if (select_ == 1)
					{
						text2_ = text_;
						text2_ = StringUtil.Replace(text2_, ",", string.Empty);
						if (text2_.Length() == 0)
						{
							return Push(SubForm.MakeDialog(KairoText.LT("フレンドIDを入力してください")));
						}
						th_ = new JThread(delegate
						{
							try
							{
								Friend friend2 = friendData_.CgiSearchFriend(text2_);
								global::system.form.FriendForm createdValue = new FriendForm(4);
createdValue.friend_ = friend2;
Push(createdValue);
							}
							catch (Exception t2)
							{
								if (Config.PRINT)
								{
									Log.Info(t2);
								}
								Push(MakeErrorDialog(Http.PopErrorText(), null));
							}
							finally
							{
								th_ = null;
							}
						});
						th_.Start();
						return true;
					}
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 4)
			{
				int num8 = 2;
				if (canvas_.CheckKeyPulse(65536))
				{
					select_ = (select_ - 1 + num8) % num8;
				}
				if (canvas_.CheckKeyPulse(262144))
				{
					select_ = (select_ + 1) % num8;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ == 0)
					{
						th_ = new JThread(delegate
						{
							try
							{
								friendData_.CgiApplyFriend(SecureString.ToString(friend_.friendId_));
								global::system.form.FriendForm createdValue = new FriendForm(5);
createdValue.friend_ = friend_;
Push(createdValue);
								Pop();
							}
							catch (Exception t2)
							{
								if (Config.PRINT)
								{
									Log.Info(t2);
								}
								Push(MakeErrorDialog(Http.PopErrorText(), SecureString.ToString(friend_.name_)));
							}
							finally
							{
								th_ = null;
							}
						});
						th_.Start();
						return true;
					}
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 5)
			{
				int num9 = 2;
				if (canvas_.CheckKeyPulse(65536))
				{
					select_ = (select_ - 1 + num9) % num9;
				}
				if (canvas_.CheckKeyPulse(262144))
				{
					select_ = (select_ + 1) % num9;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ == 0) {FriendData.ActionSendMail(friendData_.INTRODUCTION_APPLY_SUBJECT, friendData_.INTRODUCTION_APPLY_TEXT);} else { return Pop(); }
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 6)
			{
				if (next_ != null)
				{
					if (next_.type_ == 8)
					{
						if (friendData_.applyList_.Size() > 0) {
						friend_ = (Friend)friendData_.applyList_.FirstElement();
					} else { return Pop(); }}
					else
					{
						th_ = new JThread(delegate
						{
							try
							{
								friendData_.RefreshFriendList();
								if (friendData_.applyList_.Size() > 0)
								{
									friend_ = (Friend)friendData_.applyList_.FirstElement();
								}
								else
								{
									Pop();
								}
							}
							catch (Exception t2)
							{
								if (Config.PRINT)
								{
									Log.Info(t2);
								}
								Push(MakeErrorDialog(Http.PopErrorText(), null));
							}
							finally
							{
								th_ = null;
							}
						});
						th_.Start();
						if (friendData_.applyList_.Size() == 0)
						{
							return Pop();
						}
					}
					next_ = null;
					return true;
				}
				int num10 = 2;
				if (canvas_.CheckKeyPulse(65536))
				{
					select_ = (select_ - 1 + num10) % num10;
				}
				if (canvas_.CheckKeyPulse(262144))
				{
					select_ = (select_ + 1) % num10;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ == 0)
					{
						th_ = new JThread(delegate
						{
							try
							{
								friendData_.CgiApproveFriend(SecureString.ToString(friend_.friendId_));
								next_ = new FriendForm(7);
								next_.friend_ = friend_;
								Push(next_);
							}
							catch (Exception t2)
							{
								if (Config.PRINT)
								{
									Log.Info(t2);
								}
								Push(MakeErrorDialog(Http.PopErrorText(), SecureString.ToString(friend_.name_)));
							}
							finally
							{
								th_ = null;
							}
						});
						th_.Start();
						return true;
					}
					next_ = new FriendForm(8);
					next_.friend_ = friend_;
					return Push(next_);
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 7)
			{
				frame_ = (frame_ + 1) % int.MaxValue;
				if (frame_ >= 24)
				{
					bool flag2 = false;
					if (canvas_.CheckKeyPulse(1048576))
					{
						flag2 = true;
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						flag2 = true;
					}
					if (flag2)
					{
						return Pop();
					}
				}
			}
			else if (type_ == 8)
			{
				int num11 = 2;
				if (canvas_.CheckKeyPulse(65536))
				{
					select_ = (select_ - 1 + num11) % num11;
				}
				if (canvas_.CheckKeyPulse(262144))
				{
					select_ = (select_ + 1) % num11;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ == 0)
					{
						if (friend_.state_.Get() == 3) {
						FriendForm friendForm3 = new FriendForm(9);
						friendForm3.friend_ = friend_;
						Push(friendForm3);
					} else {
							th_ = new JThread(delegate
							{
								try
								{
									friendData_.CgiRefuseFriend(SecureString.ToString(friend_.friendId_));
									friendData_.RefreshFriendList();
									Pop();
								}
								catch (Exception t2)
								{
									if (Config.PRINT)
									{
										Log.Info(t2);
									}
									Push(MakeErrorDialog(Http.PopErrorText(), SecureString.ToString(friend_.name_)));
								}
								finally
								{
									th_ = null;
								}
							});
							th_.Start();
							return true;
						}}
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 9)
			{
				int num12 = 2;
				if (canvas_.CheckKeyPulse(65536))
				{
					select_ = (select_ - 1 + num12) % num12;
				}
				if (canvas_.CheckKeyPulse(262144))
				{
					select_ = (select_ + 1) % num12;
				}
				if (canvas_.CheckKeyPulse(1048576))
				{
					if (select_ == 0)
					{
						th_ = new JThread(delegate
						{
							try
							{
								friendData_.CgiRefuseFriend(SecureString.ToString(friend_.friendId_));
								friendData_.RefreshFriendList();
								Pop();
							}
							catch (Exception t2)
							{
								if (Config.PRINT)
								{
									Log.Info(t2);
								}
								Push(MakeErrorDialog(Http.PopErrorText(), SecureString.ToString(friend_.name_)));
							}
							finally
							{
								th_ = null;
							}
						});
						th_.Start();
						return true;
					}
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 10)
			{
				if (canvas_.CheckKeyPulse(1048576))
				{
					return Pop();
				}
				if (apdat_.IsPushSoftLabel(2))
				{
					return Pop();
				}
			}
			else if (type_ == 11)
			{
				if (apdat_.IsPushSoftLabel(2, 1048576))
				{
					return Pop();
				}
			}
			else if (type_ == 12)
			{
				frame_ = (frame_ + 1) % int.MaxValue;
				formMan_.DrawAllForms();
				if (frame_ >= 24)
				{
					bool flag3 = false;
					bool flag4 = false;
					if (canvas_.CheckKeyRepeat(65536))
					{
						select_ = (select_ - 1 + 2) % 2;
					}
					else if (canvas_.CheckKeyRepeat(262144))
					{
						select_ = (select_ + 1 + 2) % 2;
					}
					else if (canvas_.CheckKeyPulse(1048576))
					{
						if (select_ == 0)
						{
							flag4 = true;
						}
						flag3 = true;
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						flag3 = true;
					}
					if (flag3)
					{
						if (flag4)
						{
							FriendForm friendForm4 = new FriendForm(13);
							friendForm4.value1_ = friendData_.GetPlayBonus();
							friendData_.friSave_.ints[3] = new SecureInt(0);
							try
							{
								KairoBase instance2 = KairoBase.GetInstance();
								instance2.SaveAll();
							}
							catch (Exception)
							{
							}
							Push(friendForm4);
						}
						else
						{
							friendData_.alreadyCheckPlayTimeBonus_ = true;
						}
						return Pop();
					}
				}
			}
			else if (type_ == 13)
			{
				frame_ = (frame_ + 1) % int.MaxValue;
				formMan_.DrawAllForms();
				if (frame_ >= 50)
				{
					bool flag5 = false;
					if (canvas_.CheckKeyPulse(1048576))
					{
						flag5 = true;
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						flag5 = true;
					}
					if (flag5)
					{
						MyFriend instance3 = MyFriend.GetInstance();
						instance3.PlayBonus(value1_);
						KairoBase instance4 = KairoBase.GetInstance();
						instance4.SaveAll();
						return Pop();
					}
				}
				else if (canvas_.CheckKeyPulse(1048576))
				{
					frame_ = 50;
				}
			}
			else if (type_ == 14)
			{
				frame_ = (frame_ + 1) % int.MaxValue;
				formMan_.DrawAllForms();
				if (frame_ >= 24)
				{
					bool flag6 = false;
					bool flag7 = false;
					if (canvas_.CheckKeyRepeat(65536))
					{
						select_ = (select_ - 1 + 2) % 2;
					}
					else if (canvas_.CheckKeyRepeat(262144))
					{
						select_ = (select_ + 1 + 2) % 2;
					}
					else if (canvas_.CheckKeyPulse(1048576))
					{
						if (select_ == 0)
						{
							flag7 = true;
						}
						flag6 = true;
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						flag6 = true;
					}
					if (flag6)
					{
						if (flag7)
						{
							Push(SubForm.MakeTalkForm(0, KairoText.LT("おめでとうございます!<br>新しい<co=60,100,200>フレンド</co>が登録されボーナスが入りました!"), string.Empty));
							Push(SubForm.MakeTalkForm(0, KairoText.LT("一緒にプレイしていれば今後もボーナスが入っていきますよ!"), string.Empty));
							Push(SubForm.MakeTalkForm(0, KairoText.LT("ちなみに<co=255,0,0>アプリは削除しないで</co>下さいね。削除するとフレンドが全部消えて戻せなくなってしまいます。"), string.Empty));
							for (int num13 = 0; num13 < friendData_.newFriendList_.Size(); num13++)
							{
								FriendForm friendForm5 = new FriendForm(15);
								friendForm5.friend_ = (Friend)friendData_.newFriendList_.ElementAt(num13);
								Push(friendForm5);
							}
						}
						else
						{
							friendData_.alreadyCheckNewFriendBonus_ = true;
						}
						return Pop();
					}
				}
			}
			else if (type_ == 15)
			{
				frame_ = (frame_ + 1) % int.MaxValue;
				formMan_.DrawAllForms();
				if (frame_ >= 50)
				{
					bool flag8 = false;
					if (canvas_.CheckKeyPulse(1048576))
					{
						flag8 = true;
					}
					if (apdat_.IsPushSoftLabel(2))
					{
						flag8 = true;
					}
					if (flag8)
					{
						MyFriend instance5 = MyFriend.GetInstance();
						instance5.PlayBonus(friendData_.PLAY_NEW_FRIEND_BONUS);
						friendData_.newFriendList_.RemoveElement(friend_);
						try
						{
							KairoBase instance6 = KairoBase.GetInstance();
							instance6.SaveAll();
						}
						catch (Exception t)
						{
							if (Config.PRINT)
							{
								Log.Info(t);
							}
						}
						return Pop();
					}
				}
				else if (canvas_.CheckKeyPulse(1048576))
				{
					frame_ = 50;
				}
			}
			return true;
		}

		public override void Draw(Graphics g)
		{
			base.Draw(g);
			g.SetOrigin((view_.GetGameWidth() - 240) / 2, (view_.GetGameHeight() - 240) / 2);
			try
			{
				_draw(g);
			}
			catch (Exception t)
			{
				if (Config.PRINT)
				{
					Log.Info(t);
				}
				if (Config.PRINT)
				{
					Log.Info("type_" + type_);
				}
			}
			finally
			{
				g.SetOrigin(0, 0);
			}
		}

		public void _draw(Graphics g)
		{
			bool flag = false;
			if (apdat_.save_ != null)
			{
				flag = apdat_.save_.ints[12] != 0;
			}
			ResourceManager resourceManager = resFriend_;
			if (th_ != null) {
				ConnectForm.DrawConnect(g);

return;
}
if (type_ == 0)
			{
				int num = 12;
				int num2 = 60;
				int num3 = 216;
				int num4 = 128;
				FillRect2(g, num, num2, num3, num4, Graphics.GetColorOfRGB(149, 149, 171), Graphics.GetColorOfRGB(204, 206, 241));
				DrawGradation(g, num + 2, num2 + 2, 0, 0, num3 - 4, num4 - 4, 1, 125, 255, 107, 220, 255);
				g.SetColor(Graphics.GetColorOfRGB(164, 168, 178));
				g.DrawString(KairoText.LT("ゲームの前に"), 120f, num2 + 10, 2);
				g.SetColor(Graphics.GetColorOfRGB(207, 245, 255));
				g.DrawString(KairoText.LT("あなたのﾆｯｸﾈｰﾑを入力してね"), 120f, num2 + 28, 2);
				FillRect2(g, num + 24, num2 + 47, 171, 24, Graphics.GetColorOfRGB(184, 184, 184), Graphics.GetColorOfRGB(255, 255, 255));
				g.SetColor(166, 167, 179);
				string str = KairoText.LT("ここをタッチ");
				if (text_ != null && text_.Length() > 0)
				{
					str = text_;
				}
				g.DrawString(str, 120f, num2 + 53, 2);
				GameView.AddTouch(g, 3, num + 24, num2 + 47, 171, 24, 131072, TouchOption.Create().Margin(5, 5, 10, 10));
				g.SetColor(Graphics.GetColorOfRGB(207, 245, 255));
				g.DrawString(KairoText.LT("※あとで変更できます※"), 120f, num2 + 77, 2);
				g.SetColor(Graphics.GetColorOfRGB(70, 70, 70));
				int[] array = DrawCommand(g, KairoText.LT("OK"), 120, num2 + num4 - 25, 2, true, 20, select_ == 1);
				GameView.AddTouch(g, 3, array[0], array[1], array[2], array[3], 131073, TouchOption.Create().Margin(40, 40, 10, 20));
				resFriend_.seb[2].DrawFrame(g, num + 142, num2 + 109, resFriend_.img, 0);
			}
			else if (type_ == 1)
			{
				int num5 = 12;
				int num6 = 60;
				int num7 = 216;
				int num8 = 128;
				FillRect2(g, num5, num6, num7, num8, Graphics.GetColorOfRGB(149, 149, 171), Graphics.GetColorOfRGB(204, 206, 241));
				DrawGradation(g, num5 + 2, num6 + 2, 0, 0, num7 - 4, num8 - 4, 1, 125, 255, 107, 220, 255);
				g.SetColor(Graphics.GetColorOfRGB(207, 245, 255));
				g.DrawString(KairoText.LT("新しいﾆｯｸﾈｰﾑを入力してね"), 120f, num6 + 28, 2);
				FillRect2(g, num5 + 24, num6 + 47, 171, 24, Graphics.GetColorOfRGB(184, 184, 184), Graphics.GetColorOfRGB(255, 255, 255));
				g.SetColor(166, 167, 179);
				string str2 = KairoText.LT("ここをタッチ");
				if (text_ != null && text_.Length() > 0)
				{
					str2 = text_;
				}
				g.DrawString(str2, 120f, num6 + 53, 2);
				GameView.AddTouch(g, 3, num5 + 24, num6 + 47, 171, 24, 131072, TouchOption.Create().Margin(5, 5, 10, 10));
				g.SetColor(Graphics.GetColorOfRGB(207, 245, 255));
				g.DrawString(KairoText.LT("※あとで変更できます※"), 120f, num6 + 77, 2);
				g.SetColor(Graphics.GetColorOfRGB(70, 70, 70));
				int[] array2 = DrawCommand(g, KairoText.LT("OK"), 120, num6 + num8 - 25, 2, true, 20, select_ == 1);
				GameView.AddTouch(g, 3, array2[0], array2[1], array2[2], array2[3], 131073, TouchOption.Create().Margin(40, 40, 10, 20));
				resFriend_.seb[2].DrawFrame(g, num5 + 142, num6 + 109, resFriend_.img, 0);
			}
			else if (type_ == 2)
			{
				int num9 = 224;
				int num10 = 224;
				int num11 = (friendData_.FRIEND_MAX + 5 - 1) / 5;
				string text = KairoText.LT("<0>  <1>時間 <2>分", (string)(friendData_.friSave_.strs[0] + "\t") + friendData_.friSave_.ints[1].Get() / 60 + "\t" + friendData_.friSave_.ints[1].Get() % 60);
				int[] array3 = null;
				int num12 = array3[MyFormBase.WNDRET_BOX_TOP] + 5;
				int num13 = array3[MyFormBase.WNDRET_BOX_LEFT] + 6;
				g.SetColor(COLOR_BLACK);
				g.DrawString(LF("10,,+1"), KairoText.LT("前回起動時"), num13, num12);
				g.SetColor(COLOR_BLUE);
				g.DrawString(LF("10,,+1"), KairoText.LT("累計"), array3[MyFormBase.WNDRET_BOX_LEFT] + 91, num12);
				int num14 = friendData_.friSave_.ints[2].Get();
				g.DrawString(LF("10,,+1"), KairoText.LT("<0>時間<1>分", num14 / 60 + "\t" + num14 % 60), array3[MyFormBase.WNDRET_BOX_RIGHT] - 5, num12, 4);
				int num15 = array3[MyFormBase.WNDRET_BOX_LEFT] + 10;
				int num16 = 120;
				int num17 = array3[MyFormBase.WNDRET_BOX_RIGHT] - 10;
				num12 = array3[MyFormBase.WNDRET_BOX_TOP] + 20;
				for (int i = 0; i < 5; i++)
				{
					int num18 = page_ * 5 + i;
					if (num18 < friendData_.FRIEND_MAX)
					{
						Friend friend = null;
						bool flag2 = false;
						if (friendData_.applyList_.Size() > 0 && friendData_.friendList_.Size() < friendData_.FRIEND_MAX)
						{
							if (friendData_.FRIEND_MAX <= 5)
							{
								if (num18 == friendData_.FRIEND_MAX - 1)
								{
									flag2 = true;
								}
								else if (num18 < friendData_.friendList_.Size())
								{
									friend = (Friend)friendData_.friendList_.ElementAt(num18);
								}
							}
							else if (num18 == 0)
							{
								flag2 = true;
							}
							else if (num18 - 1 < friendData_.friendList_.Size())
							{
								friend = (Friend)friendData_.friendList_.ElementAt(num18 - 1);
							}
						}
						else if (num18 < friendData_.friendList_.Size())
						{
							friend = (Friend)friendData_.friendList_.ElementAt(num18);
						}
						int num19 = num12 + 2;
						int num20 = num12 + 18;
						int num21 = num12 + 10;
						if (friend != null)
						{
							FillRect2(g, array3[MyFormBase.WNDRET_BOX_LEFT] + 3, num12, 196, 32, Graphics.GetColorOfRGB(31, 80, 75), Graphics.GetColorOfRGB(255, 255, 255));
							g.SetColor(COLOR_BLACK);
							g.DrawString(LF("10,,+1"), KairoText.LT("<0>さん", SecureString.ToString(friend.name_)), num15, num21, 1);
							g.SetColor(COLOR_RED);
							g.DrawString(LF("10,,+1"), KairoText.LT("<0>時間<1>分", friend.minute_.Get() / 60 + "\t" + friend.minute_.Get() % 60), num17, num21, 4);
						}
						else if (flag2)
						{
							FillRect2(g, array3[MyFormBase.WNDRET_BOX_LEFT] + 3, num12, 196, 32, Graphics.GetColorOfRGB(31, 80, 75), Graphics.GetColorOfRGB(255, 255, 255));
							friend = (Friend)friendData_.applyList_.FirstElement();
							g.SetColor(COLOR_BLACK);
							g.DrawString(LF("10,,+1"), KairoText.LT("<0>さん", SecureString.ToString(friend.name_)), num15, num19, 1);
							g.DrawString(LF("10,,+1"), KairoText.LT("<0>時間<1>分", friend.minute_.Get() / 60 + "\t" + friend.minute_.Get() % 60), num17, num19, 4);
							g.DrawString(LF("10,,+1"), KairoText.LT("から申請があります。"), num15, num20, 1);
							if (friendData_.applyList_.Size() - 1 > 0)
							{
								g.DrawString(LF("10,,+1"), KairoText.LT("他<0>件", friendData_.applyList_.Size() - 1), num17, num20, 4);
							}
						}
						else
						{
							FillRect2(g, array3[MyFormBase.WNDRET_BOX_LEFT] + 3, num12, 196, 32, Graphics.GetColorOfRGB(31, 80, 75), Graphics.GetColorOfRGB(89, 167, 153));
							g.SetColor(COLOR_WHITE);
							g.DrawString(LF("10,,+1"), KairoText.LT("フレンドを探す"), num16, num21, 2);
						}
						GameView.AddTouch(g, 3, array3[MyFormBase.WNDRET_BOX_LEFT] + 3, num12, 196, 32, 0x20000 | i, TouchOption.Create().Margin(5, 5, 0, 0));
					}
					num12 += 32;
				}
			}
			else if (type_ == 3)
			{
				int num22 = 12;
				int num23 = 60;
				int num24 = 216;
				int num25 = 128;
				FillRect2(g, num22, num23, num24, num25, Graphics.GetColorOfRGB(149, 149, 171), Graphics.GetColorOfRGB(204, 206, 241));
				DrawGradation(g, num22 + 2, num23 + 2, 0, 0, num24 - 4, num25 - 4, 1, 125, 255, 107, 220, 255);
				g.SetColor(Graphics.GetColorOfRGB(207, 245, 255));
				g.DrawString(KairoText.LT("あなたのIDはこちら(ﾀｯﾁしてね)"), 120f, num23 + 10, 2);
				FillRect2(g, num22 + 24, num23 + 25, 171, 24, Graphics.GetColorOfRGB(106, 106, 106), Graphics.GetColorOfRGB(152, 152, 152));
				g.SetColor(COLOR_WHITE);
				string friendID = friendData_.GetFriendID();
				string text2 = string.Empty;
				try
				{
					for (int num26 = friendID.Length() - 1; num26 >= 0; num26--)
					{
						text2 = friendID.SubstringJ(num26, num26 + 1) + text2;
						int num27 = friendID.Length() - 1 - num26;
						if (num26 != 0 && num27 % 3 == 2)
						{
							text2 = "," + text2;
						}
					}
				}
				catch (Exception)
				{
					text2 = string.Empty;
				}
				g.DrawString(text2, 120f, num23 + 32, 2);
				GameView.AddTouch(g, 3, num22 + 24, num23 + 25, 171, 24, 65536001, TouchOption.Create().Margin(40, 40, 10, 10));
				g.SetColor(Graphics.GetColorOfRGB(207, 245, 255));
				g.DrawString(KairoText.LT("検索はこちら"), 120f, num23 + 56, 2);
				FillRect2(g, num22 + 24, num23 + 71, 171, 24, Graphics.GetColorOfRGB(184, 184, 184), Graphics.GetColorOfRGB(255, 255, 255));
				g.SetColor(166, 167, 179);
				string str3 = KairoText.LT("ここに入力");
				if (text_ != null && text_.Length() > 0)
				{
					str3 = text_;
				}
				g.DrawString(str3, 120f, num23 + 77, 2);
				GameView.AddTouch(g, 3, num22 + 24, num23 + 71, 171, 24, 131072, TouchOption.Create().Margin(5, 5, 20, 5));
				g.SetColor(Graphics.GetColorOfRGB(70, 70, 70));
				int[] array4 = DrawCommand(g, KairoText.LT("検索"), 120, num23 + num25 - 25, 2, true, 10, select_ == 1);
				GameView.AddTouch(g, 3, array4[0], array4[1], array4[2], array4[3], 131073, TouchOption.Create().Margin(40, 40, 10, 20));
			}
			else
			{
				if (type_ == 4) { }
				else if (type_ == 5) { }
				else if (type_ == 6) { }
				else if (type_ == 7) { }
				else if (type_ == 8) { }
				else if (type_ == 9) { }
				else if (type_ == 10) { }
				else if (type_ == 11) { }
				else if (type_ == 12)
				{
					if (flag)
					{
						g.SetRenderMode(1, 48, 207);
						g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
						int originX = g.GetOriginX();
						int originY = g.GetOriginY();
						g.SetOrigin(0, 0);
						g.FillRect(GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H);
						g.SetOrigin(originX, originY);
						g.SetRenderMode(0, 255, 0);
					}
					int originX2 = g.GetOriginX();
					int originY2 = g.GetOriginY();
					g.SetOrigin(0, originY2);
					int num28 = GameForm.VIEW_W / 2;
					int num29 = (240 - g.GetOriginY()) / 2;
					int num30 = 4;
					int num31 = 8;
					int length = 8;
					int num32 = 16;
					int num33 = 8;
					int vIEW_W = GameForm.VIEW_W;
					int num34 = 66;
					int num35 = 0;
					int num36 = 0;
					int num37 = 60;
					int num38 = 12;
					num37 = frame_ * ((vIEW_W - 60) / num30);
					if (num37 > vIEW_W)
					{
						num37 = vIEW_W;
					}
					num38 = frame_ * ((num34 - 12) / num30);
					if (num38 > num34)
					{
						num38 = num34;
					}
					num35 = 0;
					num36 = num29 - num38 / 2;
					g.SetRenderMode(1, 192, 63);
					DrawBonusObi(g, num35, num36, num37, num38, Graphics.GetColorOfRGB(241, 178, 50), Graphics.GetColorOfRGB(234, 215, 12), Graphics.GetColorOfRGB(246, 222, 12));
					g.SetRenderMode(0, 255, 0);
					if (frame_ >= num31)
					{
						num37 = 153;
						num38 = 19;
						num35 = Graph.Easing(-num37, (GameForm.VIEW_W - num37) / 2, length, frame_ - num31, 80);
						num36 = num29 - 39;
						FillRect3(g, num35, num36, num37, num38, Graphics.GetColorOfRGB(255, 210, 25), Graphics.GetColorOfRGB(255, 224, 72), Graphics.GetColorOfRGB(255, 250, 226));
						g.SetColor(COLOR_BLACK);
						string str4 = KairoText.LT("ﾌﾚﾝﾄﾞﾎﾞｰﾅｽ(<0>分毎)", JString.ValueOf(friendData_.PLAY_BONUS_INTERVAL_MINUTE));
						g.DrawString(LF("10,,+1"), str4, num35 + num37 / 2, num36 + 3, 2);
					}
					if (frame_ >= num32)
					{
						num37 = 204;
						num38 = 54;
						num35 = Graph.Easing(-num37, (GameForm.VIEW_W - num37) / 2, num33, frame_ - num32, 80);
						num36 = num29 - 16;
						FillRect2(g, num35, num36, num37, num38, Graphics.GetColorOfRGB(7, 56, 158), Graphics.GetColorOfRGB(206, 217, 152));
						DrawGradation(g, num35 + 2, num36 + 2, 0, 0, num37 - 4, num38 - 4, 114, 142, 176, 80, 86, 155);
						int num39 = num32 + num33;
						g.SetClip(num35 + 2, num36 + 2, num37 - 4, num38 - 4);
						if (frame_ < num39)
						{
							int maxFrame = resourceManager.seb[9].GetMaxFrame();
							resourceManager.seb[11].DrawFrame(g, num35 + 16, num36 + 57, resourceManager.img, frame_ % maxFrame);
							resourceManager.seb[8].DrawFrame(g, num35 + 190, num36 + 57, resourceManager.img, frame_ % maxFrame);
						}
						else
						{
							int maxFrame2 = resourceManager.seb[13].GetMaxFrame();
							resourceManager.seb[15].DrawFrame(g, num35 + 16, num36 + 57, resourceManager.img, (frame_ - num39) % maxFrame2);
							resourceManager.seb[12].DrawFrame(g, num35 + 190, num36 + 57, resourceManager.img, (frame_ - num39) % maxFrame2);
						}
						g.ClearClip();
						g.SetColor(COLOR_WHITE);
						string text3 = KairoText.LT("ﾌﾚﾝﾄﾞから <co=254,233,1><0></co>時間<co=254,233,1><1></co>分 取得", friendData_.friSave_.ints[3].Get() / 60 + "\t" + friendData_.friSave_.ints[3].Get() % 60);
						TextLayout.Draw(LF("10,,+1"), g, text3, num35 + num37 / 2, num36 + 5, COLOR_WHITE, 2);
						resourceManager.DrawSeb(g, num35 + 59, num36 + 9 + 9, 16);
						int playBonus = friendData_.GetPlayBonus();
						apdat_.DrawNumImage(g, 17, string.Empty, playBonus, string.Empty, num35 + 80 + 7, num36 + 12 + 9, 0, 2);
						resourceManager.seb[18].DrawFrame(g, num35 + 98, num36 + 11 + 9, resourceManager.img, 0);
						g.SetColor(COLOR_WHITE);
						if (friendData_.GetPlayBonus() >= friendData_.PLAY_BONUS_MAX)
						{
							g.DrawString(LF("10,,+1"), KairoText.LT("(最大)"), num35 + 133, num36 + 12 + 9, 1);
						}
					}
					if (frame_ >= 24)
					{
						g.SetColor(COLOR_WHITE);
						int[] array5 = DrawCommand(g, KairoText.LT("受け取る"), num35 + 67, num36 + 37, 2, select_ == 0, 10, select_ == 0);
						GameView.AddTouch(g, 3, array5[0], array5[1], array5[2], array5[3], 131072, TouchOption.Create().Margin(20, 10, 15, 15));
						array5 = DrawCommand(g, KairoText.LT("また次回"), num35 + 139, num36 + 37, 2, select_ == 1, 10, select_ == 1);
						GameView.AddTouch(g, 3, array5[0], array5[1], array5[2], array5[3], 131073, TouchOption.Create().Margin(10, 20, 15, 15));
					}
				}
				else if (type_ == 13)
				{
					if (flag)
					{
						g.SetRenderMode(1, 48, 207);
						g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
						int originX3 = g.GetOriginX();
						int originY3 = g.GetOriginY();
						g.SetOrigin(0, 0);
						g.FillRect(GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H);
						g.SetOrigin(originX3, originY3);
						g.SetRenderMode(0, 255, 0);
					}
				}
				else if (type_ == 14)
				{
					if (flag)
					{
						g.SetRenderMode(1, 48, 207);
						g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
						int originX4 = g.GetOriginX();
						int originY4 = g.GetOriginY();
						g.SetOrigin(0, 0);
						g.FillRect(GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H);
						g.SetOrigin(originX4, originY4);
						g.SetRenderMode(0, 255, 0);
					}
					int originX5 = g.GetOriginX();
					int originY5 = g.GetOriginY();
					g.SetOrigin(0, originY5);
					int num40 = GameForm.VIEW_W / 2;
					int num41 = (240 - g.GetOriginY()) / 2;
					int num42 = 4;
					int num43 = 8;
					int length2 = 8;
					int num44 = 16;
					int num45 = 8;
					int vIEW_W2 = GameForm.VIEW_W;
					int num46 = 66;
					int num47 = 0;
					int num48 = 0;
					int num49 = 60;
					int num50 = 12;
					num49 = frame_ * ((vIEW_W2 - 60) / num42);
					if (num49 > vIEW_W2)
					{
						num49 = vIEW_W2;
					}
					num50 = frame_ * ((num46 - 12) / num42);
					if (num50 > num46)
					{
						num50 = num46;
					}
					num47 = 0;
					num48 = num41 - num50 / 2;
					g.SetRenderMode(1, 192, 63);
					DrawBonusObi(g, num47, num48, num49, num50, Graphics.GetColorOfRGB(241, 178, 50), Graphics.GetColorOfRGB(234, 215, 12), Graphics.GetColorOfRGB(246, 222, 12));
					g.SetRenderMode(0, 255, 0);
					if (frame_ >= num43)
					{
						num49 = 153;
						num50 = 19;
						num47 = Graph.Easing(-num49, (GameForm.VIEW_W - num49) / 2, length2, frame_ - num43, 80);
						num48 = num41 - 39;
						FillRect3(g, num47, num48, num49, num50, Graphics.GetColorOfRGB(255, 210, 25), Graphics.GetColorOfRGB(255, 224, 72), Graphics.GetColorOfRGB(255, 250, 226));
						g.SetColor(COLOR_BLACK);
						g.DrawString(KairoText.LT("ﾌﾚﾝﾄﾞﾎﾞｰﾅｽ"), num47 + num49 / 2, num48 + 3, 2);
					}
					if (frame_ >= num44)
					{
						num49 = 204;
						num50 = 54;
						num47 = Graph.Easing(-num49, (GameForm.VIEW_W - num49) / 2, num45, frame_ - num44, 80);
						num48 = num41 - 16;
						FillRect2(g, num47, num48, num49, num50, Graphics.GetColorOfRGB(7, 56, 158), Graphics.GetColorOfRGB(206, 217, 152));
						DrawGradation(g, num47 + 2, num48 + 2, 0, 0, num49 - 4, num50 - 4, 114, 142, 176, 80, 86, 155);
						int num51 = num44 + num45;
						g.SetClip(num47 + 2, num48 + 2, num49 - 4, num50 - 4);
						if (frame_ < num51)
						{
							int maxFrame3 = resourceManager.seb[9].GetMaxFrame();
							resourceManager.seb[11].DrawFrame(g, num47 + 16, num48 + 57, resourceManager.img, frame_ % maxFrame3);
							resourceManager.seb[8].DrawFrame(g, num47 + 190, num48 + 57, resourceManager.img, frame_ % maxFrame3);
						}
						else
						{
							int maxFrame4 = resourceManager.seb[13].GetMaxFrame();
							resourceManager.seb[15].DrawFrame(g, num47 + 16, num48 + 57, resourceManager.img, (frame_ - num51) % maxFrame4);
							resourceManager.seb[12].DrawFrame(g, num47 + 190, num48 + 57, resourceManager.img, (frame_ - num51) % maxFrame4);
						}
						g.ClearClip();
						g.SetColor(COLOR_WHITE);
						string text4 = KairoText.LT("フレンドが<co=254,233,1><0>人</co>増えました!", JString.ValueOf(friendData_.newFriendList_.Size()));
						TextLayout.Draw(g, text4, num47 + num49 / 2, num48 + 5, COLOR_WHITE, 2);
						resourceManager.DrawSeb(g, num47 + 59, num48 + 9 + 9, 16);
						int num52 = friendData_.PLAY_NEW_FRIEND_BONUS * friendData_.newFriendList_.Size();
						DrawNumImage(g, 17, num52, num47 + 80 + 7, num48 + 12 + 9, 0, 2);
						resourceManager.seb[18].DrawFrame(g, num47 + 98, num48 + 11 + 9, resourceManager.img, 0);
					}
					if (frame_ >= 24)
					{
						g.SetColor(COLOR_WHITE);
						int[] array6 = DrawCommand(g, KairoText.LT("受け取る"), num47 + 67, num48 + 37, 2, select_ == 0, 10, select_ == 0);
						GameView.AddTouch(g, 3, array6[0], array6[1], array6[2], array6[3], 131072, TouchOption.Create().Margin(20, 10, 15, 15));
						array6 = DrawCommand(g, KairoText.LT("また次回"), num47 + 139, num48 + 37, 2, select_ == 1, 10, select_ == 1);
						GameView.AddTouch(g, 3, array6[0], array6[1], array6[2], array6[3], 131073, TouchOption.Create().Margin(10, 20, 15, 15));
					}
				}
				else if (type_ == 15 && flag)
				{
					g.SetRenderMode(1, 48, 207);
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
					int originX6 = g.GetOriginX();
					int originY6 = g.GetOriginY();
					g.SetOrigin(0, 0);
					g.FillRect(GameForm.VIEW_X, GameForm.VIEW_Y, GameForm.VIEW_W, GameForm.VIEW_H);
					g.SetOrigin(originX6, originY6);
					g.SetRenderMode(0, 255, 0);
				}
			}
		}

		public override bool Resume()
		{
			{ bool recoveredReturn = base.Resume(); return recoveredReturn; }
		}

		public override bool Finish()
		{
			base.Finish();
			return true;
		}

		public override int GetTouchValue(int type, int index)
		{
			type = (int)(type & 0xFFFF0000u);
			switch (type)
			{
			case 131072:
				return select_;
			case 65536000:
				return select2_;
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
				if (type_ == 2 && value >= 5)
				{
					select2_ = value - 5;
					select_ = 5;
				}
				break;
			case 65536000:
				select2_ = value;
				break;
			default:
				base.SetTouchValue(type, value);
				break;
			}
		}

		public override bool OnTouchEvent(TouchEvent e)
		{
			TouchComponent component = e.component;
			return true;
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, bool active, bool cursor)
		{
			return DrawCommand(g, str, x, y, 0, active, 1, cursor);
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, int anchor, bool active, bool cursor)
		{
			return DrawCommand(g, str, x, y, anchor, active, 1, cursor);
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, int anchor, bool active, int margin, bool cursor)
		{
			return null;
		}

		public int[] DrawCommand(Graphics g, string str, int x, int y, int w, int anchor, bool active, bool cursor)
		{
			return null;
		}

		public void SetSoftLabel(int left, int right)
		{
			SetSoftLabel(AppData.softLabels_[left], AppData.softLabels_[right]);
		}

		public static void DrawLine(Graphics g, int x1, int y1, int x2, int y2, int color)
		{
			g.SetColor(color);
			g.DrawLine(x1, y1, x2, y2);
		}

		public static void DrawDotLine(Graphics g, int x1, int y1, int x2, int y2, int color)
		{
			g.SetColor(color);
			for (int i = x1; i < x2; i += 3)
			{
				g.DrawLine(i, y1, i + 1, y2);
			}
		}

		public static void DrawRect(Graphics g, int x, int y, int width, int height, int color)
		{
			g.SetColor(color);
			g.DrawRect(x, y, width, height);
		}

		public static void DrawRect2(Graphics g, int x, int y, int width, int height, int color1, int color2)
		{
			g.SetColor(color1);
			g.DrawRect(x, y, width, height);
			g.SetColor(color2);
			g.DrawRect(x + 1, y + 1, width - 2, height - 2);
		}

		public static void DrawRect3(Graphics g, int x, int y, int width, int height, int color1, int color2, int color3)
		{
			g.SetColor(color1);
			g.DrawRect(x, y, width, height);
			g.SetColor(color2);
			g.DrawRect(x + 1, y + 1, width - 2, height - 2);
			g.SetColor(color3);
			g.DrawRect(x + 2, y + 2, width - 4, height - 4);
		}

		public static void FillRect(Graphics g, int x, int y, int width, int height, int color)
		{
			g.SetColor(color);
			g.FillRect(x, y, width, height);
		}

		public static void FillRect2(Graphics g, int x, int y, int width, int height, int color1, int color2)
		{
			DrawRect(g, x, y, width - 1, height - 1, color1);
			FillRect(g, x + 1, y + 1, width - 2, height - 2, color2);
		}

		public static void FillRect3(Graphics g, int x, int y, int width, int height, int color1, int color2, int color3)
		{
			DrawRect(g, x, y, width - 1, height - 1, color1);
			DrawRect(g, x + 1, y + 1, width - 3, height - 3, color2);
			FillRect(g, x + 2, y + 2, width - 4, height - 4, color3);
		}

		public static void FillRect4(Graphics g, int x, int y, int width, int height, int color1, int color2, int color3, int color4)
		{
			DrawRect(g, x, y, width - 1, height - 1, color1);
			DrawRect(g, x + 1, y + 1, width - 3, height - 3, color2);
			DrawRect(g, x + 2, y + 2, width - 5, height - 5, color3);
			FillRect(g, x + 3, y + 3, width - 6, height - 6, color4);
		}

		public static void FillRectShadow(Graphics g, int x, int y, int width, int height, int color1, int color2, int color3)
		{
			DrawRect(g, x, y, width - 1, height - 1, color1);
			DrawLine(g, x + 1, y + (height - 2), x + (width - 2), y + (height - 2), color2);
			DrawLine(g, x + (width - 2), y + 1, x + (width - 2), y + (height - 2), color2);
			FillRect(g, x + 1, y + 1, width - 3, height - 3, color3);
		}

		public static void DrawObi(Graphics g, int x, int y, int width, int height, int color1, int color2)
		{
			FillRect(g, x, y, width, 1, color1);
			FillRect(g, x, y + 1, width, height - 2, color2);
			FillRect(g, x, y + height - 1, width, 1, color1);
		}

		public static void DrawGradation(Graphics g, int dx, int dy, int sx, int sy, int w, int h, int sr, int sg, int sb, int dr, int dg, int db)
		{
			if ((w <= 0) || (h <= 0)) { return; }
int dxAlias = dx;
int num = dy;
int num2 = dx + w;
int num3 = dy + h;
double num4 = (double)(dr - sr) / (double)(h / 4 + 1);
double num5 = (double)(dg - sg) / (double)(h / 4 + 1);
double num6 = (double)(db - sb) / (double)(h / 4 + 1);
double num7 = (double)sr + (double)(sy / 4) * num4;
double num8 = (double)sg + (double)(sy / 4) * num5;
double num9 = (double)sb + (double)(sy / 4) * num6;
while (num < num3)
				{
					num7 = (((num7 > 255.0)) ? (255.0) : (num7));
					num7 = (((num7 < 0.0)) ? (0.0) : (num7));
					num8 = (((num8 > 255.0)) ? (255.0) : (num8));
					num8 = (((num8 < 0.0)) ? (0.0) : (num8));
					num9 = (((num9 > 255.0)) ? (255.0) : (num9));
					num9 = (((num9 < 0.0)) ? (0.0) : (num9));
					g.SetColor(Graphics.GetColorOfRGB((int)num7, (int)num8, (int)num9));
					int num10 = 0;
					if ((num == dy && sy % 4 != 0)) { num10 = (sy % 4); } else { if ((num + 4 > num3)) { num10 = ((num3 - num)); } else { num10 = (4); } }
					g.FillRect(dxAlias, num, w, num10);
					num += num10;
					num7 += num4;
					num8 += num5;
					num9 += num6;
				}
		}

		public static void DrawBonusObi(Graphics g, int x, int y, int width, int height, int color1, int color2, int color3)
		{
			if (height > 5)
			{
				FillRect(g, x, y, width, 5, color1);
			}
			if (height > 6)
			{
				FillRect(g, x, y + 5, width, 1, color2);
			}
			if (height > 12)
			{
				FillRect(g, x, y + 6, width, height - 12, color3);
			}
			if (height > 6)
			{
				FillRect(g, x, y + height - 6, width, 1, color2);
			}
			if (height > 5)
			{
				FillRect(g, x, y + height - 5, width, 5, color1);
			}
		}

		public void DrawNumImage(Graphics g, int seb, long value, int x, int y)
		{
			DrawNumImage(g, seb, value, x, y, 0, 1);
		}

		public void DrawNumImage(Graphics g, int seb, long value, int x, int y, int padding)
		{
			DrawNumImage(g, seb, value, x, y, padding, 1);
		}

		public void DrawNumImage(Graphics g, int seb, long value, int x, int y, int padding, int anchor)
		{
			ResourceManager resourceManager = resFriend_;
			int[] sprite = resourceManager.seb[seb].GetSprite(0, 0);
			int num = sprite[4];
			int num2 = sprite[5];
			bool flag = value < 0;
			if (value < 0)
			{
				value *= -1;
			}
			long num3 = 1L;
			long num4 = 1L;
			long num5 = value;
			while (num5 / 10 > 0)
			{
				num5 /= 10;
				num4 *= 10;
				num3++;
			}
			long num6 = num3;
			if (flag)
			{
				num6++;
			}
			num6 = num6 * (num + padding) - padding;
			if ((anchor & 2) != 0)
			{
				x -= (int)num6 / 2;
			}
			else if ((anchor & 4) != 0)
			{
				x -= (int)num6;
			}
			if ((anchor & 0x20) != 0)
			{
				y -= num2 / 2;
			}
			else if ((anchor & 0x40) != 0)
			{
				y -= num2;
			}
			if (flag)
			{
				if (45 < resourceManager.seb[seb].GetMaxFrame())
				{
					resourceManager.seb[seb].DrawFrame(g, x, y, resourceManager.img, 45);
				}
				else
				{
					g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
					g.DrawString("-", x + 1, y + 1);
					g.SetColor(Graphics.GetColorOfRGB(0, 0, 0));
					g.DrawString("-", x, y);
				}
				x += num + padding;
			}
			num5 = value;
			for (int i = 0; i < num3; i++)
			{
				resourceManager.seb[seb].DrawFrame(g, x, y, resourceManager.img, (int)(num5 / num4));
				num5 %= num4;
				num4 /= 10;
				x += num + padding;
			}
		}

		public FormBase MakeErrorDialog()
		{
			return MakeErrorDialog(null, null);
		}

		public FormBase MakeErrorDialog(string msg, string args)
		{
			if (msg == null || msg.Length() == 0)
			{
				msg = KairoText.LT("通信できませんでした。|設定を確認し時間を|おいて挑戦して下さい。&ﾌﾚﾝﾄﾞは<friend_max>人まで登録できます");
			}
			msg = StringUtil.Replace(msg, "<friend_max>", JString.ValueOf(friendData_.FRIEND_MAX));
			if (args != null)
			{
				msg = StringUtil.Replace(msg, args);
			}
			msg = StringUtil.Replace(msg, "|", string.Empty);
			string[] array = StringUtil.Split(msg, "&");
			if (array.Length == 1)
			{
				return SubForm.MakeDialog(array[0]);
			}
			return MakeErrorMsgForm(array[0], array[1]);
		}

		public static int[] DrawRandomFriend(Graphics g, int center, int top)
		{
			LoadResource();
			FriendData instance = MyFriend.GetInstance();
			if (resFriend_ == null || !instance.ExistRandomFriend())
			{
				return null;
			}
			Friend randomFriend = instance.GetRandomFriend();
			int num = 150;
			int num2 = 53;
			int num3 = center - num / 2;
			int topAlias = top;
g.SetRenderMode(1, 200, 55);
			DrawGradation(g, num3 + 1, topAlias + 1, 0, 0, num - 2, num2 - 2, 99, 84, 149, 159, 128, 186);
			g.SetRenderMode(0, 255, 0);
			g.SetColor(92, 50, 104);
			g.DrawRect(num3, topAlias, num - 2, num2 - 2);
			g.SetColor(Graphics.GetColorOfRGB(255, 255, 255));
			string str = KairoText.LT("<0>さん", SecureString.ToString(randomFriend.name_));
			g.DrawString(str, center, topAlias + 4, 2);
			string str2 = KairoText.LT("ﾌﾟﾚｲ <0>時間<1>分", randomFriend.minute_.Get() / 60 + "\t" + randomFriend.minute_.Get() % 60);
			g.DrawString(str2, center, topAlias + 20, 2);
			g.SetColor(Graphics.GetColorOfRGB(85, 50, 16));
			g.DrawStringBold(KairoText.LT("申請してみる"), center, topAlias + 37, Graphics.GetColorOfRGB(255, 246, 222), 2);
			resFriend_.seb[1].DrawFrame(g, num3 + 4, topAlias + 50, resFriend_.img, 2);
			resFriend_.seb[1].DrawFrame(g, num3 + 129, topAlias + 50, resFriend_.img, 1);
			resFriend_.seb[0].DrawFrame(g, num3 + 42, topAlias - 11, resFriend_.img, 0);
			drawRandomFriend_result[0] = num3;
			drawRandomFriend_result[1] = topAlias;
			drawRandomFriend_result[2] = num;
			drawRandomFriend_result[3] = num2;
			return drawRandomFriend_result;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
