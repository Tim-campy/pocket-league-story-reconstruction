using System;
using form;
using java.io;
using java.lang;
using java.net;
using java.util;
using kairo.common.cfg;
using kairo.unity.system;
using kairo.unity.ui;
using kairo.unity.util;
using native;
using system.form;

namespace system.friend.ui
{
	public abstract class FriendData
	{
		public const int FRISAVEP_END = 0;

		public const int FRISAVEI_FLAG = 0;

		public const int FRISAVEI_MINUTE_MY = 1;

		public const int FRISAVEI_MINUTE_PREVIOUS = 2;

		public const int FRISAVEI_MINUTE_DIFF = 3;

		public const int FRISAVEI_MINUTE_TOTAL = 4;

		public const int FRISAVEI_SAVE_VERSION = 5;

		public const int FRISAVEI_END = 6;

		public const int FRISAVE_FLAG_ABOUT_FRIEND = 1;

		public const int FRISAVEL_END = 0;

		public const int FRISAVES_FRIEND_NAME = 0;

		public const int FRISAVES_END = 1;

		public const int FRISAVEB_FRIEND_HISTORY = 0;

		public const int FRISAVEB_NEW_FRIEND = 1;

		public const int FRISAVEB_END = 2;

		internal static FriendData instance_;

		public int FRIEND_MAX = 5;

		public string INTRODUCTION_APPLY_SUBJECT = KairoText.LT("フレンド申請 お住まい夢物語");

		public string INTRODUCTION_APPLY_TEXT = KairoText.LT("お住まい夢物語for Androidで\nフレンド申請したよ!\n承認してね。");

		public string INTRODUCTION_SUBJECT = KairoText.LT("フレンド申請 お住まい夢物語");

		public string INTRODUCTION_TEXT = KairoText.LT("お住まい夢物語for Androidをインストールして\nフレンドで\nID：「<0>」\nを探してね。\n\nDLはこっちだよ\n") + "http://kairopark.jp/friend/?gm=" + Config.PID + "&dl=store";

		public SecureProperty friSave_;

		public static readonly SecureProperty[] defFriSaveP_ = new SecureProperty[0];

		public static readonly int[] defFriSaveI_ = new int[6];

		public static readonly long[] defFriSaveL_ = new long[0];

		public static readonly string[] defFriSaveS_ = new string[1] { string.Empty };

		public static readonly byte[][] defFriSaveB_ = JTool.MakeArray<byte[][]>(new int[2] { 2, 0 });

		public int PLAY_BONUS_INTERVAL_MINUTE = 40;

		public int PLAY_BONUS_MAX = 20;

		public int PLAY_NEW_FRIEND_BONUS = 7;

		public Vector friendList_ = new Vector();

		public Vector applyList_ = new Vector();

		public Vector newFriendList_ = new Vector();

		private Friend randomFriend_;

		private bool friendDataUpdated_;

		public bool alreadyCheckPlayTimeBonus_;

		public bool alreadyCheckNewFriendBonus_;

		public SecureInt previousUpdateMinute_ = new SecureInt(-1);

		private Vector friendHistory_ = new Vector();

		private long playTimeBegin_ = -1L;

		private long playTimeSec_;

		internal FriendData()
		{
			friSave_ = new SecureProperty(defFriSaveP_, defFriSaveI_, defFriSaveL_, defFriSaveS_, defFriSaveB_);
		}

		public static FriendData GetInstance()
		{
			return instance_;
		}

		public void Init()
		{
		}

		public void Load(byte[] data)
		{
			friSave_ = new SecureProperty(defFriSaveP_, defFriSaveI_, defFriSaveL_, defFriSaveS_, defFriSaveB_);
			friSave_.Load(data);
			UnpackFriendBinalyData();
			friSave_.ints[2] = new SecureInt(friSave_.ints[4]);
		}

		public byte[] Write()
		{
			PackFriendBinalyData();
			return friSave_.ToByteArray();
		}

		public void NewGame(bool take)
		{
			InitializeData();
		}

		public void LoadGame(byte[] data)
		{
			InitializeData();
		}

		public byte[] WriteGame()
		{
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream != null)
				{
					byteArrayOutputStream.Close();
				}
				byteArrayOutputStream = null;
			}
		}

		private void PackFriendBinalyData()
		{
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.WriteInt(byteArrayOutputStream, friendHistory_.Size());
				for (int i = 0; i < friendHistory_.Size(); i++)
				{
					FriendPlayTimeData friendPlayTimeData = (FriendPlayTimeData)friendHistory_.ElementAt(i);
					friendPlayTimeData.Serialize(byteArrayOutputStream);
				}
				friSave_.bins[0] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
				byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.WriteInt(byteArrayOutputStream, newFriendList_.Size());
				for (int j = 0; j < newFriendList_.Size(); j++)
				{
					Friend friend = (Friend)newFriendList_.ElementAt(j);
					friend.Serialize(byteArrayOutputStream);
				}
				friSave_.bins[1] = byteArrayOutputStream.ToByteArray();
				byteArrayOutputStream.Close();
			}
			finally
			{
				if (byteArrayOutputStream != null)
				{
					byteArrayOutputStream.Close();
				}
				byteArrayOutputStream = null;
			}
		}

		private void UnpackFriendBinalyData()
		{
			ByteArrayInputStream byteArrayInputStream = null;
			if (friSave_.bins[0] == null || friSave_.bins[0].Length == 0)
			{
				return;
			}
			try
			{
				friendHistory_.Clear();
				byteArrayInputStream = new ByteArrayInputStream(friSave_.bins[0]);
				if (byteArrayInputStream.Available() > 0)
				{
					int num = StreamUtil.ReadInt(byteArrayInputStream);
					for (int i = 0; i < num; i++)
					{
						FriendPlayTimeData friendPlayTimeData = new FriendPlayTimeData();
						friendPlayTimeData.Deserialize(byteArrayInputStream);
						friendHistory_.AddElement(friendPlayTimeData);
					}
				}
				byteArrayInputStream.Close();
				newFriendList_.Clear();
				byteArrayInputStream = new ByteArrayInputStream(friSave_.bins[1]);
				if (byteArrayInputStream.Available() > 0)
				{
					int num2 = StreamUtil.ReadInt(byteArrayInputStream);
					for (int j = 0; j < num2; j++)
					{
						Friend friend = new Friend();
						friend.Deserialize(byteArrayInputStream);
						newFriendList_.AddElement(friend);
					}
				}
				byteArrayInputStream.Close();
			}
			finally
			{
			}
		}

		public void CgiRegistFriend(string name, long minute)
		{
			string text = KairoBase.MakeQuery("name", name);
			string text2 = KairoBase.MakeQuery("m", (minute == -1) ? (string.Empty) : (JString.ValueOf(minute)));
			KairoBase.Connect("friend/regist_friend.php" + text + text2);
		}

		public virtual Vector CgiGetFriendList()
		{
			friendList_.RemoveAllElements();
			applyList_.RemoveAllElements();
			try
			{
				byte[] array = KairoBase.Connect("friend/friend_get.php");
				if (array.Length == 0)
				{
					return friendList_;
				}
				string[] array2 = StringUtil.Split(JString.New(array), ",");
				for (int i = 0; i < array2.Length; i++)
				{
					Friend friend = new Friend(array2[i]);
					if (friend.state_.Get() == 1)
					{
						applyList_.AddElement(friend);
					}
					else if (friend.state_.Get() == 3)
					{
						friendList_.AddElement(friend);
					}
				}
				return friendList_;
			}
			catch (Exception ex)
			{
				friendList_.RemoveAllElements();
				applyList_.RemoveAllElements();
				throw ex;
			}
		}

		public Friend CgiSearchFriend(string friendId)
		{
			if (!CheckRegistFriend())
			{
				throw new Exception();
			}
			string text = KairoBase.MakeQuery("friendid", friendId.Trim());
			byte[] array = KairoBase.Connect("friend/friend_search.php" + text);
			if (array.Length == 0)
			{
				return null;
			}
			return new Friend(array);
		}

		public void CgiApplyFriend(string friendId)
		{
			if (!CheckRegistFriend())
			{
				throw new Exception();
			}
			string text = KairoBase.MakeQuery("friendid", friendId);
			string text2 = KairoBase.MakeQuery("max", JString.ValueOf(FRIEND_MAX));
			KairoBase.Connect("friend/friend_apply.php" + text + text2);
		}

		public void CgiApproveFriend(string friendId)
		{
			if (!CheckRegistFriend())
			{
				throw new Exception();
			}
			string text = KairoBase.MakeQuery("friendid=", friendId);
			string text2 = KairoBase.MakeQuery("max", JString.ValueOf(FRIEND_MAX));
			KairoBase.Connect("friend/friend_approve.php" + text + text2);
		}

		public void CgiRefuseFriend(string friendId)
		{
			if (!CheckRegistFriend())
			{
				throw new Exception();
			}
			string text = KairoBase.MakeQuery("friendid=", friendId);
			KairoBase.Connect("friend/friend_refuse.php" + text);
		}

		public Friend CgiRandomFriend()
		{
			if (!CheckRegistFriend())
			{
				return null;
			}
			byte[] array = KairoBase.Connect("friend/friend_random.php");
			if (array.Length == 0)
			{
				return null;
			}
			return new Friend(array);
		}

		public string GetFriendName()
		{
			return SecureString.ToString(friSave_.strs[0]);
		}

		public string GetFriendID()
		{
			KairoBase instance = KairoBase.GetInstance();
			if (instance == null)
			{
				return string.Empty;
			}
			return instance.GetFriendId();
		}

		public void SetFriendName(string name)
		{
			friSave_.strs[0] = SecureString.ValueOf(name);
		}

		public bool CheckRegistFriend()
		{
			return friSave_.strs[0] != null && friSave_.strs[0].Length() > 0;
		}

		public int GetPlayBonus()
		{
			if (PLAY_BONUS_MAX == 0 || PLAY_BONUS_INTERVAL_MINUTE == 0)
			{
				return 0;
			}
			int num = friSave_.ints[3].Get() / PLAY_BONUS_INTERVAL_MINUTE;
			if (num > PLAY_BONUS_MAX)
			{
				num = PLAY_BONUS_MAX;
			}
			return num;
		}

		public void RefreshFriendList()
		{
			if (!CheckRegistFriend())
			{
				return;
			}
			try
			{
				friendDataUpdated_ = false;
				CgiGetFriendList();
				int num = 0;
				Vector vector = new Vector();
				for (int i = 0; i < friendList_.Size(); i++)
				{
					vector.AddElement(friendList_.ElementAt(i));
				}
				for (int j = 0; j < applyList_.Size(); j++)
				{
					vector.AddElement(applyList_.ElementAt(j));
				}
				for (int k = 0; k < vector.Size(); k++)
				{
					Friend friend = (Friend)vector.ElementAt(k);
					bool flag = false;
					for (int l = 0; l < friendHistory_.Size(); l++)
					{
						FriendPlayTimeData friendPlayTimeData = (FriendPlayTimeData)friendHistory_.ElementAt(l);
						if (!friend.friendId_.Equals(friendPlayTimeData.friendId_))
						{
							continue;
						}
						flag = true;
						if (friend.state_.Get() == 3)
						{
							int num2 = friend.minute_.Get() - friendPlayTimeData.minute_.Get();
							if (num2 > 0)
							{
								num += num2;
							}
							friendPlayTimeData.minute_ = new SecureInt(friend.minute_);
						}
						friendPlayTimeData.state_ = new SecureInt(friend.state_);
						break;
					}
					if (!flag)
					{
						if (friend.state_.Get() == 3)
						{
							FriendPlayTimeData obj = new FriendPlayTimeData(SecureString.ToString(friend.friendId_), friend.minute_.Get(), friend.state_.Get());
							friendHistory_.AddElement(obj);
							num += friend.minute_.Get();
							newFriendList_.AddElement(friend);
						}
						friendDataUpdated_ = true;
					}
				}
				friSave_.ints[3] = new SecureInt(friSave_.ints[3].Get() + num);
				friSave_.ints[4] = new SecureInt(friSave_.ints[4].Get() + num);
				for (int m = 0; m < friendHistory_.Size(); m++)
				{
					FriendPlayTimeData friendPlayTimeData2 = (FriendPlayTimeData)friendHistory_.ElementAt(m);
					if (friendPlayTimeData2.state_.Get() != 3)
					{
						continue;
					}
					bool flag2 = false;
					for (int n = 0; n < friendList_.Size(); n++)
					{
						Friend friend2 = (Friend)friendList_.ElementAt(n);
						if (friendPlayTimeData2.friendId_.Equals(friend2.friendId_))
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						friendPlayTimeData2.state_ = new SecureInt(-1);
					}
				}
				KairoBase instance = KairoBase.GetInstance();
				instance.Save();
			}
			catch (Exception ex)
			{
				if (Config.PRINT)
				{
					Log.Info(ex);
				}
				friendDataUpdated_ = false;
				throw ex;
			}
		}

		public bool UpdateRandomFriend()
		{
			Friend friend = null;
			try
			{
				friend = CgiRandomFriend();
			}
			catch (Exception t)
			{
				if (Config.PRINT)
				{
					Log.Info(t);
				}
			}
			randomFriend_ = friend;
			return randomFriend_ != null;
		}

		public bool ExistRandomFriend()
		{
			return randomFriend_ != null;
		}

		public Friend GetRandomFriend()
		{
			return randomFriend_;
		}

		private static void DrawGradation(Graphics g, int dx, int dy, int sx, int sy, int w, int h, int sr, int sg, int sb, int dr, int dg, int db)
		{
			if ((w <= 0) || (h <= 0)) { return; }
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
					num7 = ((!(num7 > 255.0)) ? num7 : 255.0);
					num7 = ((!(num7 < 0.0)) ? num7 : 0.0);
					num8 = ((!(num8 > 255.0)) ? num8 : 255.0);
					num8 = ((!(num8 < 0.0)) ? num8 : 0.0);
					num9 = ((!(num9 > 255.0)) ? num9 : 255.0);
					num9 = ((!(num9 < 0.0)) ? num9 : 0.0);
					g.SetColor(Graphics.GetColorOfRGB((int)num7, (int)num8, (int)num9));
					int num10 = 0;
					num10 = ((num == dy && sy % 4 != 0) ? (sy % 4) : ((num + 4 <= num3) ? 4 : (num3 - num)));
					g.FillRect(dx, num, w, num10);
					num += num10;
					num7 += num4;
					num8 += num5;
					num9 += num6;
				}
		}

		public static void ActionSendMail(string subject, string text)
		{
			AppPlugin.LaunchMailer(subject, text);
		}

		public bool IsFriendDataUpdated()
		{
			return friendDataUpdated_;
		}

		public void InitializeData()
		{
			alreadyCheckPlayTimeBonus_ = false;
			alreadyCheckNewFriendBonus_ = false;
		}

		public static bool IsEffectiveRegistName(string name)
		{
			return IsEffectiveRegistName(name, -1);
		}

		public static bool IsEffectiveRegistName(string name, int max)
		{
			if (name == null)
			{
				return false;
			}
			if (name.Length() == 0)
			{
				return false;
			}
			if (name.Trim().Length() == 0)
			{
				return false;
			}
			if (max != -1 && name.Length() > max)
			{
				return false;
			}
			try
			{
				string text = URLEncoder.Encode(name, "UTF-8");
				if (text == null)
				{
					return false;
				}
				if (text.Length() == 0)
				{
					return false;
				}
				if (text.Trim().Length() == 0)
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public FriendForm GetCurrentForm()
		{
			return GetCurrentForm(null);
		}

		public FriendForm GetCurrentForm(Friend friend)
		{
			if (!CheckRegistFriend())
			{
				FriendForm friendForm = new FriendForm(0);
				friendForm.friend_ = friend;
				return friendForm;
			}
			if (friend == null)
			{
				return new FriendForm(2);
			}
			FriendForm friendForm2 = new FriendForm(4);
			friendForm2.friend_ = friend;
			return friendForm2;
		}

		public void UpdatePlayTime()
		{
			long num = JSystem.CurrentTimeMillis();
			if (playTimeBegin_ == -1)
			{
				playTimeSec_ = 0L;
				playTimeBegin_ = num;
			}
			long num2 = num - playTimeBegin_;
			if (num2 > 1000)
			{
				playTimeSec_++;
				playTimeBegin_ = num;
			}
			if (playTimeSec_ >= 60)
			{
				friSave_.ints[1] = new SecureInt(friSave_.ints[1].Get() + 1);
				playTimeSec_ = 0L;
			}
			FormManager instance = FormManager.GetInstance();
			if (!alreadyCheckNewFriendBonus_ && newFriendList_.Size() > 0 && PLAY_NEW_FRIEND_BONUS > 0)
			{
				instance.Push(new FriendForm(14));
			}
			else if (!alreadyCheckPlayTimeBonus_ && friSave_.ints[3].Get() >= PLAY_BONUS_INTERVAL_MINUTE && PLAY_BONUS_INTERVAL_MINUTE > 0)
			{
				instance.Push(new FriendForm(12));
			}
		}

		public abstract void PlayBonus(int value);
	}
}
