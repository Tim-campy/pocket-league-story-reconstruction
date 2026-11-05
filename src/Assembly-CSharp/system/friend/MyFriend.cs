using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.system;
using kairo.unity.util;
using system.friend.ui;

namespace system.friend
{
	public class MyFriend : FriendData
	{
		internal MyFriend()
		{
			FRIEND_MAX = 5;
			if (Language.Japanese())
			{
				INTRODUCTION_APPLY_SUBJECT = (INTRODUCTION_SUBJECT = KairoText.LT("フレンド申請 お住まい夢物語"));
				INTRODUCTION_APPLY_TEXT = KairoText.LT("お住まい夢物語for Androidで\nフレンド申請したよ!\n承認してね。");
				INTRODUCTION_TEXT = KairoText.LT("お住まい夢物語for Androidをインストールして\nフレンドで\nID：「<0>」\nを探してね。\n\nDLはこっちだよ\n") + "http://kairopark.jp/friend/?gm=" + Config.PID + "&dl=store";
			}
			else
			{
				INTRODUCTION_APPLY_SUBJECT = (INTRODUCTION_SUBJECT = KairoText.LT("Friend Request (Dream House Days)"));
				INTRODUCTION_APPLY_TEXT = KairoText.LT("I'd like to be friends in Dream House Days for Android. Please approve my request!");
				INTRODUCTION_TEXT = KairoText.LT("Try Dream House Days for Android and search for the ID \"<0>\" to find me.<br><br>You can download the app here:<br>") + "http://kairopark.jp/friend/?gm=" + Config.PID + "&dl=store";
			}
			PLAY_BONUS_INTERVAL_MINUTE = 40;
			PLAY_BONUS_MAX = 20;
			PLAY_NEW_FRIEND_BONUS = 7;
		}

		public override void PlayBonus(int value)
		{
		}

		public new static MyFriend GetInstance()
		{
			if (FriendData.instance_ == null)
			{
				FriendData.instance_ = new MyFriend();
			}
			return (MyFriend)FriendData.instance_;
		}

		public override Vector CgiGetFriendList()
		{
			if (0 == 0)
			{
				return base.CgiGetFriendList();
			}
			Vector vector = new Vector();
			Vector vector2 = new Vector();
			byte[] array = KairoBase.Connect("friend/friend_get.php");
			if (array.Length == 0)
			{
				return friendList_;
			}
			string[] array2 = StringUtil.Split(JString.New(array), ",");
			int num = 0;
			while (num < array2.Length)
			{
				Friend friend = new Friend(array2[num]);
				if (friend.state_.Get() == 1)
				{
					vector2.AddElement(friend);
				}
				else if (friend.state_.Get() == 3)
				{
					vector.AddElement(friend);
				}
				num++;
			}
			friendList_.RemoveAllElements();
			int num2 = 0;
			while (num2 < vector.Size())
			{
				friendList_.AddElement(vector.ElementAt(num2));
				num2++;
			}
			applyList_.RemoveAllElements();
			int num3 = 0;
			while (num3 < vector2.Size())
			{
				applyList_.AddElement(vector2.ElementAt(num3));
				num3++;
			}
			return friendList_;
		}
	}
}
