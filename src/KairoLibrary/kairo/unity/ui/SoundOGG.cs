using System;
using java.io;
using kairo.common.cfg;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.ui
{
	public class SoundOGG : SoundBase
	{
		private SoundOGG(AudioClip p, int type, int loop)
			: base(p, type, loop)
		{
		}

		public static SoundOGG Load(InputStream stream)
		{
			throw new Exception("load:ストリームからのロードは対応していません。");
		}

		public static SoundOGG Load(byte[] data)
		{
			throw new Exception("load:バイナリからのロードは対応していません。");
		}

		public static SoundOGG Load(string file)
		{
			int num = file.LastIndexOf('.');
			if (num != -1)
			{
				file = file.Substring(0, num);
			}
			return (SoundOGG)IApplication.GetCurrentApp().RunOnUiThread(() => new SoundOGG(Resources.Load<AudioClip>(file), 1, 0), true);
		}

		public override void Play(int volume, bool mute)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				SoundPlayer instance = SoundPlayer.GetInstance();
				try
				{
					if (base.track_ != null && base.state_ == 2 && base.track_.isPlaying)
					{
						SetVolume(volume, mute);
						base.track_.time = 0f;
					}
					else
					{
						Stop();
						if (!(type_ == 1 && mute))
						{
							if (loop_ != 0)
							{
								priority_ = 2;
							}
							else if (type_ == 0)
							{
								priority_ = 1;
							}
							else
							{
								priority_ = 0;
							}
							int port = instance.GetPort(priority_);
							if (port != -1)
							{
								instance.RemovePort(port);
								instance.SetPort(port, this);
								SetVolume(volume, mute);
								base.track_.loop = loop_ != 0;
								if (loop_ != 0)
								{
									instance.AddPlayingList(this);
								}
								base.track_.Play();
							}
						}
					}
				}
				catch (Exception t)
				{
					if (Config.PRINT)
					{
						Log.Info(t);
					}
					Stop();
				}
			}, true);
		}

		public override void SetVolume(int volume, bool mute)
		{
			if (base.track_ == null)
			{
				return;
			}
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				if (Config.UNITY_EDITOR)
				{
					volume = volume * 10 / 100;
				}
				try
				{
					if (mute)
					{
						base.track_.volume = 0f;
					}
					else
					{
						base.track_.volume = (float)volume / 255f;
					}
				}
				catch (Exception t)
				{
					if (Config.PRINT)
					{
						Log.Info(t);
					}
					Stop();
				}
			}, true);
		}

		public override void Stop()
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				try
				{
					SoundPlayer instance = SoundPlayer.GetInstance();
					if (base.track_ != null)
					{
						base.track_.Stop();
					}
					instance.RemovePort(this);
					if (loop_ != 0)
					{
						instance.RemovePlayingList(this);
					}
				}
				catch (Exception t)
				{
					if (Config.PRINT)
					{
						Log.Info(t);
					}
				}
			}, true);
		}

		public override void Dispose()
		{
			Stop();
			base.Dispose();
		}

		public override int GetState()
		{
			return base.state_;
		}
	}
}
