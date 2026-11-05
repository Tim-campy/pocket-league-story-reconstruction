using java.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public class SoundPlayer
	{
		internal class Port
		{
			internal AudioSource component;

			internal SoundBase sound;
		}

		public const int MAX_VOLUME = 255;

		public const int MAX_PRIO = 255;

		public const int TYPE_BGM = 0;

		public const int TYPE_SE = 1;

		public const int TYPE_MAX = 2;

		public const int LOOP_NONE = 0;

		public const int LOOP_INFINITY = -1;

		public const int LOOP_MAX = 255;

		private static SoundPlayer instance_;

		private int[] volume_ = new int[2];

		private bool[] mute_ = new bool[2];

		private Vector playingList_;

		internal Port[] port_;

		private bool suspend_;

		private SoundBase[] suspendSoundList_;

		private bool suspend2_;

		private bool isSilentMode_;

		private bool isOtherAudioPlay_;

		private int[] masterVolume_ = new int[2];

		private SoundPlayer()
		{
			Init();
		}

		protected void Init()
		{
			GameObject gameObject = new GameObject("SoundPlayer");
			playingList_ = new Vector();
			volume_[0] = 127;
			volume_[1] = 127;
			mute_[0] = false;
			mute_[1] = false;
			masterVolume_[0] = 1;
			masterVolume_[1] = 1;
			port_ = new Port[4];
			for (int i = 0; i < port_.Length; i++)
			{
				port_[i] = new Port();
				port_[i].component = gameObject.AddComponent<AudioSource>();
				port_[i].component.playOnAwake = false;
			}
			isSilentMode_ = IsSilentMode();
			UpdateOtherAudioPlay();
		}

		internal void AddPlayingList(SoundBase sb)
		{
			if (!playingList_.Contains(sb))
			{
				playingList_.AddElement(sb);
			}
		}

		protected Vector GetPlayingList()
		{
			return playingList_;
		}

		internal void RemovePlayingList(SoundBase sb)
		{
			playingList_.RemoveElement(sb);
		}

		public void Dispose()
		{
			if (instance_ != null)
			{
				StopAll();
				instance_ = null;
			}
		}

		public static SoundPlayer GetInstance()
		{
			if (instance_ == null)
			{
				instance_ = new SoundPlayer();
			}
			return instance_;
		}

		public static void Init(bool usePort)
		{
			if (instance_ != null)
			{
				instance_.Dispose();
			}
			GetInstance();
		}

		public void SetVolume(int type, int volume)
		{
			volume_[type] = volume;
			for (int i = 0; i < playingList_.Size(); i++)
			{
				SoundBase soundBase = (SoundBase)playingList_.ElementAt(i);
				if (type == soundBase.type_)
				{
					soundBase.SetVolume(volume_[type] * masterVolume_[type], mute_[type]);
				}
			}
		}

		public void SetMute(int type, bool mute)
		{
			mute_[type] = mute;
			for (int i = 0; i < playingList_.Size(); i++)
			{
				SoundBase soundBase = (SoundBase)playingList_.ElementAt(i);
				if (type == soundBase.type_)
				{
					soundBase.SetVolume(volume_[type] * masterVolume_[type], mute_[type]);
				}
			}
		}

		public bool IsMute(int type)
		{
			return mute_[type];
		}

		public void ReflectVolumeAndMute()
		{
			SetVolume(0, volume_[0]);
			SetMute(0, mute_[0]);
			SetVolume(1, volume_[1]);
			SetMute(1, mute_[1]);
		}

		public void Play(SoundBase s)
		{
			if (!isSilentMode_)
			{
				s.Play(volume_[s.type_] * masterVolume_[s.type_], mute_[s.type_]);
			}
		}

		public void Stop(SoundBase s)
		{
			s.Stop();
		}

		public void StopAll()
		{
			while (!playingList_.IsEmpty())
			{
				((SoundBase)playingList_.FirstElement()).Stop();
			}
			playingList_.RemoveAllElements();
		}

		public void Resume()
		{
			if (suspend2_)
			{
				suspend2_ = false;
			}
		}

		public void Suspend()
		{
			if (!suspend2_)
			{
				suspend2_ = true;
			}
		}

		internal int GetPort(int priority)
		{
			for (int i = 0; i < port_.Length; i++)
			{
				if (port_[i].sound != null && !port_[i].component.isPlaying)
				{
					RemovePort(i);
				}
			}
			int result = -1;
			for (int j = 0; j < port_.Length; j++)
			{
				if (port_[j].sound == null)
				{
					return j;
				}
			}
			for (int k = 0; k <= priority; k++)
			{
				for (int l = 0; l < port_.Length; l++)
				{
					if (port_[l].sound.priority_ == k)
					{
						return result;
					}
				}
			}
			return -1;
		}

		internal AudioSource GetAudioSource(SoundBase sound)
		{
			Port[] array = port_;
			foreach (Port port in array)
			{
				if (port.sound == sound)
				{
					return port.component;
				}
			}
			return null;
		}

		internal void RemovePort(int port)
		{
			port_[port].component.clip = null;
			port_[port].sound = null;
		}

		internal void RemovePort(SoundBase sound)
		{
			for (int i = 0; i < port_.Length; i++)
			{
				if (port_[i].sound == sound)
				{
					RemovePort(i);
				}
			}
		}

		internal void SetPort(int port, SoundBase sound)
		{
			if (port_[port].sound != null)
			{
				RemovePort(port);
			}
			port_[port].component.clip = sound.data_;
			port_[port].sound = sound;
		}

		public bool IsSilentMode()
		{
			return false;
		}

		public void OnReceive(object context, object intent)
		{
		}

		private void SetMasterVolume(int type, int vol)
		{
			if (vol < 0)
			{
				vol = 0;
			}
			if (vol > 1)
			{
				vol = 1;
			}
			masterVolume_[type % 2] = vol;
			SetVolume(type, volume_[type]);
		}

		public bool IsOtherAudioPlay()
		{
			UpdateOtherAudioPlay();
			return isOtherAudioPlay_;
		}

		public void UpdateOtherAudioPlay()
		{
		}

		public void SetPortMax(int max)
		{
			if (max == port_.Length)
			{
				return;
			}
			GameObject gameObject = GameObject.Find("SoundPlayer");
			Port[] array = new Port[max];
			for (int i = 0; i < port_.Length; i++)
			{
				if (i < array.Length)
				{
					array[i] = port_[i];
					continue;
				}
				RemovePort(i);
				Object.Destroy(port_[i].component);
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] == null)
				{
					array[j] = new Port();
					array[j].component = gameObject.AddComponent<AudioSource>();
					array[j].component.playOnAwake = false;
				}
			}
			port_ = array;
		}

		public int GetPortMax()
		{
			return port_.Length;
		}
	}
}
