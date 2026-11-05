using java.io;
using kairo.unity.util;

namespace system.friend.ui
{
	public class FriendPlayTimeData
	{
		public SecureString friendId_;

		public SecureInt minute_ = new SecureInt();

		public SecureInt state_ = new SecureInt();

		public FriendPlayTimeData()
		{
		}

		public FriendPlayTimeData(string friendId, int minute, int state)
		{
			friendId_ = SecureString.ValueOf(friendId);
			minute_ = new SecureInt(minute);
			state_ = new SecureInt(state);
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteString(os, SecureString.ToString(friendId_));
			StreamUtil.WriteInt(os, minute_.Get());
			StreamUtil.WriteInt(os, state_.Get());
		}

		public void Deserialize(InputStream stream)
		{
			friendId_ = SecureString.ValueOf(StreamUtil.ReadString(stream));
			minute_ = new SecureInt(StreamUtil.ReadInt(stream));
			state_ = new SecureInt(StreamUtil.ReadInt(stream));
		}
	}
}
