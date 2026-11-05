using java.io;
using java.lang;
using java.net;
using kairo.unity.util;

namespace system.friend.ui
{
	public class Friend
	{
		public const int STATE_APPLY = 1;

		public const int STATE_APPLY_WAIT = 2;

		public const int STATE_FRIEND = 3;

		public SecureString friendId_;

		public SecureString name_;

		public SecureInt state_ = new SecureInt();

		public SecureInt minute_ = new SecureInt();

		public SecureInt updated_ = new SecureInt();

		public Friend()
		{
			friendId_ = null;
			name_ = SecureString.ValueOf(string.Empty);
			state_ = new SecureInt(1);
			minute_ = new SecureInt(0);
			updated_ = new SecureInt(0);
		}

		public Friend(string friendId, string name, int state, int minute, int updated)
		{
			friendId_ = SecureString.ValueOf(friendId);
			name_ = SecureString.ValueOf(name);
			state_ = new SecureInt(state);
			minute_ = new SecureInt(minute);
			updated_ = new SecureInt(updated);
			if (!name_.EndsWith("\b"))
			{
				name_ = SecureString.ValueOf(name_.ToString() + "\b");
			}
		}

		public Friend(byte[] src)
			: this(JString.New(src))
		{
		}

		public Friend(string src)
			: this()
		{
			string[] array = StringUtil.Split(src, "|");
			if (0 < array.Length)
			{
				friendId_ = new SecureString(array[0]);
			}
			if (1 < array.Length)
			{
				name_ = new SecureString(URLDecoder.Decode(array[1]));
			}
			if (2 < array.Length)
			{
				state_ = new SecureInt(JInteger.ValueOf(array[2]));
			}
			if (3 < array.Length)
			{
				minute_ = new SecureInt(JInteger.ValueOf(array[3]));
			}
			if (4 < array.Length)
			{
				updated_ = new SecureInt(JInteger.ValueOf(array[4]));
			}
			if (!name_.EndsWith("\b"))
			{
				name_ = new SecureString(name_.ToString() + "\b");
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteString(os, SecureString.ToString(friendId_));
			StreamUtil.WriteInt(os, state_.Get());
			StreamUtil.WriteInt(os, minute_.Get());
			StreamUtil.WriteInt(os, updated_.Get());
			StreamUtil.WriteString(os, SecureString.ToString(name_));
		}

		public void Deserialize(InputStream stream)
		{
			friendId_ = new SecureString(StreamUtil.ReadString(stream));
			state_ = new SecureInt(StreamUtil.ReadInt(stream));
			minute_ = new SecureInt(StreamUtil.ReadInt(stream));
			updated_ = new SecureInt(StreamUtil.ReadInt(stream));
			name_ = new SecureString(StreamUtil.ReadString(stream));
		}
	}
}
