using java.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public abstract class SoundBase
	{
		protected const int MAX_VOLUME_SPF = 127;

		protected const int EVENT_END = -1;

		protected const int EVENT_LOOP = -2;

		protected const int EVENT_PAUSE = -3;

		public const int NO_DATA = 0;

		public const int READY = 1;

		public const int PLAYING = 2;

		public const int PAUSED = 3;

		protected static Vector soundList_ = new Vector();

		protected AudioSource track_ => SoundPlayer.GetInstance().GetAudioSource(this);

		protected int state_
		{
			get
			{
				if ((object)data_ == null)
				{
					return 0;
				}
				if ((object)track_ == null)
				{
					return 3;
				}
				if ((bool)IApplication.GetCurrentApp().RunOnUiThread(() => track_.isPlaying, true))
				{
					return 2;
				}
				return 3;
			}
		}

		internal string name_ = "";

		internal AudioClip data_;

		internal int type_;

		internal int loop_;

		internal int priority_;

		public const int MIN_PRIORITY = 0;

		public const int NORM_PRIORITY = 1;

		public const int MAX_PRIORITY = 2;

		public SoundBase(AudioClip data, int type, int loop)
		{
			data_ = data;
			type_ = type;
			loop_ = loop;
			priority_ = 1;
			soundList_.AddElement(this);
		}

		public void SetName(string name)
		{
			name_ = name;
		}

		public string GetName()
		{
			return name_;
		}

		public abstract void Play(int volume, bool mute);

		public abstract void SetVolume(int volume, bool mute);

		public abstract void Stop();

		public virtual void Dispose()
		{
			if ((object)data_ != null)
			{
				IApplication.GetCurrentApp().RunOnUiThread(delegate
				{
					Resources.UnloadAsset(data_);
				}, true);
			}
			data_ = null;
			type_ = 0;
			soundList_.RemoveElement(this);
		}

		public abstract int GetState();

		public int GetSoundType()
		{
			return type_;
		}

		public void SetSoundType(int type)
		{
			type_ = type;
		}

		public int GetLoop()
		{
			return loop_;
		}

		public void SetLoop(int loop)
		{
			loop_ = loop;
			if ((object)track_ != null)
			{
				track_.loop = loop == -1;
			}
		}

		public object GetData()
		{
			return data_;
		}

		public override string ToString()
		{
			return "[" + name_ + "] " + base.ToString();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
