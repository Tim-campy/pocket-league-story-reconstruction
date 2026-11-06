using System;
using System.Text;
using java.io;
using java.lang;
using java.util;
using kairo.unity.util;

namespace kairo.unity.system
{
	public class GameDataRecord
	{
		public class Data
		{
			internal SecureString name_;

			internal SecureByteArray value_;

			internal bool toset_;

			internal bool acquired_;

			public Data()
			{
			}

			public Data(string name, byte[] value, bool toset)
			{
				name_ = SecureString.ValueOf(name);
				value_ = SecureByteArray.ValueOf(value);
				toset_ = toset;
			}

			public Data Clone()
			{
				return new Data(SecureString.ToString(name_), SecureByteArray.ToArray(value_), toset_)
				{
					acquired_ = acquired_
				};
			}

			public void Serialize(OutputStream os)
			{
				if (name_ == null)
				{
					StreamUtil.WriteInt(os, -1);
				}
				else
				{
					StreamUtil.Write(os, name_.GetBytes(), true);
				}
				if (value_ == null)
				{
					StreamUtil.WriteInt(os, -1);
				}
				else
				{
					StreamUtil.Write(os, value_.ToArray(), true);
				}
				StreamUtil.WriteBoolean(os, toset_);
				StreamUtil.WriteBoolean(os, acquired_);
			}

			public void Deserialize(InputStream stream)
			{
				int num = StreamUtil.ReadInt(stream);
				if (num == -1)
				{
					name_ = null;
				}
				else
				{
					name_ = new SecureString(StreamUtil.Read(stream, num));
				}
				num = StreamUtil.ReadInt(stream);
				if (num == -1)
				{
					value_ = null;
				}
				else
				{
					value_ = SecureByteArray.ValueOf(StreamUtil.Read(stream, num));
				}
				toset_ = StreamUtil.ReadBoolean(stream);
				acquired_ = StreamUtil.ReadBoolean(stream);
			}
		}

		internal SecureInt userId_ = new SecureInt();

		internal SecureInt friendId_ = new SecureInt();

		internal SecureInt number_ = new SecureInt();

		internal Vector dataSet_;

		internal bool exist_;

		internal bool isInsert_;

		public GameDataRecord()
			: this(-1, 0)
		{
		}

		public GameDataRecord(int number, bool insert = true)
			: this(JInteger.ParseInt(KairoBase.GetInstance().GetFriendId()), number, insert)
		{
		}

		public GameDataRecord(int friendId, int number, bool insert = true)
		{
			userId_ = new SecureInt(-1);
			friendId_ = new SecureInt(friendId);
			number_ = new SecureInt(number);
			dataSet_ = new Vector();
			exist_ = true;
			isInsert_ = insert;
		}

		public GameDataRecord(int number, string list, bool insert = true)
			: this(JInteger.ParseInt(KairoBase.GetInstance().GetFriendId()), number, list, insert)
		{
		}

		public GameDataRecord(int friendId, int number, string list, bool insert = true)
			: this(friendId, number, insert)
		{
			AddList(list);
		}

		public void Add(string name)
		{
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data = (Data)dataSet_.ElementAt(i);
				if (!data.toset_ && name.Equals(SecureString.ToString(data.name_)))
				{
					return;
				}
			}
			dataSet_.AddElement(new Data(name, null, false));
		}

		public void Add(string name, string value)
		{
			Add(name, (value == null) ? null : value.GetBytes());
		}

		public void Add(string name, long value)
		{
			Add(name, JString.ValueOf(value).GetBytes());
		}

		public void Add(string name, byte[] data)
		{
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data2 = (Data)dataSet_.ElementAt(i);
				if (data2.toset_ && name.Equals(SecureString.ToString(data2.name_)))
				{
					data2.value_ = SecureByteArray.ValueOf(data);
					return;
				}
			}
			dataSet_.AddElement(new Data(name, data, true));
		}

		internal void Acquire(string name, byte[] data)
		{
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data2 = (Data)dataSet_.ElementAt(i);
				if (!data2.toset_ && name.Equals(SecureString.ToString(data2.name_)))
				{
					data2.value_ = SecureByteArray.ValueOf(data);
					data2.acquired_ = true;
					break;
				}
			}
		}

		public void AddList(string list)
		{
			string[] array = StringUtil.Split(list, ",");
			for (int i = 0; i < array.Length; i++)
			{
				int num = array[i].IndexOf('=');
				if (num == -1)
				{
					string name = array[i].Trim();
					Add(name);
				}
				else
				{
					string name2 = array[i].SubstringJ(0, num).Trim();
					string value = array[i].SubstringJ(num + 1);
					Add(name2, value);
				}
			}
		}

		public bool Contains(string name)
		{
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data = (Data)dataSet_.ElementAt(i);
				if (!data.toset_ && name.Equals(SecureString.ToString(data.name_)) && data.acquired_)
				{
					return true;
				}
			}
			for (int j = 0; j < dataSet_.Size(); j++)
			{
				Data data2 = (Data)dataSet_.ElementAt(j);
				if (data2.toset_ && name.Equals(SecureString.ToString(data2.name_)))
				{
					return true;
				}
			}
			return false;
		}

		public bool TryGetInteger(string name, out int to, int def = 0)
		{
			to = def;
			byte[] to2;
			if (!TryGetData(name, out to2))
			{
				return false;
			}
			to = JInteger.ParseInt(Encoding.UTF8.GetString(to2));
			return true;
		}

		public bool TryGetValue(string name, out long to, long def = 0L)
		{
			to = def;
			byte[] to2;
			if (!TryGetData(name, out to2))
			{
				return false;
			}
			to = JLong.ParseLong(Encoding.UTF8.GetString(to2));
			return true;
		}

		public bool TryGetString(string name, out string to, string def = null)
		{
			to = def;
			byte[] to2;
			if (!TryGetData(name, out to2))
			{
				return false;
			}
			to = Encoding.UTF8.GetString(to2);
			return true;
		}

		public bool TryGetData(string name, out byte[] to, byte[] def = null)
		{
			to = def;
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data = (Data)dataSet_.ElementAt(i);
				if (!data.toset_ && name.Equals(SecureString.ToString(data.name_)) && data.acquired_)
				{
					to = SecureByteArray.ToArray(data.value_);
					return true;
				}
			}
			for (int j = 0; j < dataSet_.Size(); j++)
			{
				Data data2 = (Data)dataSet_.ElementAt(j);
				if (data2.toset_ && name.Equals(SecureString.ToString(data2.name_)))
				{
					to = SecureByteArray.ToArray(data2.value_);
					return true;
				}
			}
			return false;
		}

		public int GetInteger(string name)
		{
			byte[] data = GetData(name);
			if (data == null)
			{
				return int.MinValue;
			}
			return JInteger.ParseInt(Encoding.UTF8.GetString(data));
		}

		public long GetValue(string name)
		{
			byte[] data = GetData(name);
			if (data == null)
			{
				return long.MinValue;
			}
			return JLong.ParseLong(Encoding.UTF8.GetString(data));
		}

		public string GetString(string name)
		{
			byte[] data = GetData(name);
			if (data == null)
			{
				return null;
			}
			return Encoding.UTF8.GetString(data);
		}

		public byte[] GetData(string name)
		{
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data = (Data)dataSet_.ElementAt(i);
				if (!data.toset_ && name.Equals(SecureString.ToString(data.name_)) && data.acquired_)
				{
					return SecureByteArray.ToArray(data.value_);
				}
			}
			for (int j = 0; j < dataSet_.Size(); j++)
			{
				Data data2 = (Data)dataSet_.ElementAt(j);
				if (data2.toset_ && name.Equals(SecureString.ToString(data2.name_)))
				{
					return SecureByteArray.ToArray(data2.value_);
				}
			}
			return null;
		}

		public void Pack(OutputStream os)
		{
			KairoBase instance = KairoBase.GetInstance();
			StreamUtil.WriteInt(os, userId_.Get());
			StreamUtil.WriteInt(os, friendId_.Get());
			StreamUtil.WriteInt(os, number_.Get());
			StreamUtil.WriteInt(os, dataSet_.Size());
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data = (Data)dataSet_.ElementAt(i);
				string text = SecureString.ToString(data.name_);
				byte[] array = instance.ToDBValue("t_gamedata", text, SecureByteArray.ToArray(data.value_));
				StreamUtil.WriteString(os, text);
				StreamUtil.WriteInt(os, (array == null) ? (-1) : array.Length);
				if (array != null)
				{
					StreamUtil.Write(os, array);
				}
			}
		}

		public GameDataRecord Copy(bool to_set)
		{
			GameDataRecord gameDataRecord = new GameDataRecord(friendId_.Get(), number_.Get(), isInsert_);
			gameDataRecord.friendId_ = new SecureInt(friendId_);
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data = (Data)dataSet_.ElementAt(i);
				if (data.toset_ == to_set && (!to_set || data.name_.IndexOf(46) == -1))
				{
					gameDataRecord.dataSet_.AddElement(data.Clone());
				}
			}
			return gameDataRecord;
		}

		public GameDataRecord Copy()
		{
			GameDataRecord gameDataRecord = new GameDataRecord(friendId_.Get(), number_.Get(), isInsert_);
			gameDataRecord.userId_ = new SecureInt(userId_);
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data = (Data)dataSet_.ElementAt(i);
				gameDataRecord.dataSet_.AddElement(data.Clone());
			}
			return gameDataRecord;
		}

		public override string ToString()
		{
			string text = "userId [" + userId_.Get() + "] friendId [" + friendId_.Get() + "] number [" + number_.Get() + "]";
			try
			{
				if (dataSet_ != null)
				{
					for (int i = 0; i < dataSet_.Size(); i++)
					{
						Data data = (Data)dataSet_.ElementAt(i);
						text += "\n" + (data.toset_ ? "set" : "get") + ":" + data.name_ + ":";
						if (data.value_ == null) { text = (text + "null"); } else { text = (text + Encoding.UTF8.GetString(data.value_.ToArray())); }
					}
				}
			}
			catch (Exception)
			{
			}
			return text;
		}

		public GameDataRecord Clone()
		{
			GameDataRecord gameDataRecord = new GameDataRecord(friendId_.Get(), number_.Get(), isInsert_);
			gameDataRecord.userId_ = new SecureInt(userId_);
			gameDataRecord.dataSet_ = new Vector();
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				Data data = (Data)dataSet_.ElementAt(i);
				gameDataRecord.dataSet_.AddElement(data.Clone());
			}
			return gameDataRecord;
		}

		public int GetUserId()
		{
			return userId_.Get();
		}

		public int GetFriendId()
		{
			return friendId_.Get();
		}

		public int GetNumber()
		{
			return number_.Get();
		}

		public bool Exist()
		{
			return exist_;
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteInt(os, userId_.Get());
			StreamUtil.WriteInt(os, friendId_.Get());
			StreamUtil.WriteInt(os, number_.Get());
			StreamUtil.WriteBoolean(os, exist_);
			StreamUtil.WriteInt(os, dataSet_.Size());
			for (int i = 0; i < dataSet_.Size(); i++)
			{
				((Data)dataSet_.ElementAt(i)).Serialize(os);
			}
		}

		public void Deserialize(InputStream stream)
		{
			userId_ = new SecureInt(StreamUtil.ReadInt(stream));
			friendId_ = new SecureInt(StreamUtil.ReadInt(stream));
			number_ = new SecureInt(StreamUtil.ReadInt(stream));
			exist_ = StreamUtil.ReadBoolean(stream);
			int num = StreamUtil.ReadInt(stream);
			dataSet_ = new Vector();
			for (int i = 0; i < num; i++)
			{
				Data data = new Data();
				data.Deserialize(stream);
				dataSet_.AddElement(data);
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
