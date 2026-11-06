using java.io;
using java.lang;
using kairo.unity.util;

namespace game
{
	public class DelayEvent
	{
		public int delayId_;

		public int line_;

		public int wait_;

		public int hour_;

		public int rideChara_;

		public string args_;

		public int[] argi_;

		public DelayEvent()
		{
			delayId_ = -1;
			line_ = 0;
			wait_ = 0;
			hour_ = -1;
			rideChara_ = -1;
			args_ = string.Empty;
		}

		public DelayEvent(int delayId, int line, int wait, int hour, int rideChara, string args, int[] argi)
		{
			delayId_ = delayId;
			line_ = line;
			wait_ = wait;
			hour_ = hour;
			rideChara_ = rideChara;
			args_ = args;
			argi_ = null;
			if (argi != null)
			{
				argi_ = new int[argi.Length];
				JSystem.Arraycopy(argi, 0, argi_, 0, argi_.Length);
			}
		}

		public void Serialize(OutputStream os)
		{
			StreamUtil.WriteInt(os, delayId_);
			StreamUtil.WriteInt(os, line_);
			StreamUtil.WriteInt(os, wait_);
			StreamUtil.WriteInt(os, hour_);
			StreamUtil.WriteInt(os, rideChara_);
			StreamUtil.WriteString(os, (args_ == null) ? (string.Empty) : (args_));
			if (argi_ == null)
			{
				StreamUtil.WriteInt(os, 0);
				return;
			}
			StreamUtil.WriteInt(os, argi_.Length);
			for (int i = 0; i < argi_.Length; i++)
			{
				StreamUtil.WriteInt(os, argi_[i]);
			}
		}

		public void Deserialize(InputStream stream)
		{
			delayId_ = StreamUtil.ReadInt(stream);
			line_ = StreamUtil.ReadInt(stream);
			wait_ = StreamUtil.ReadInt(stream);
			hour_ = StreamUtil.ReadInt(stream);
			rideChara_ = StreamUtil.ReadInt(stream);
			args_ = StreamUtil.ReadString(stream);
			argi_ = new int[StreamUtil.ReadInt(stream)];
			for (int i = 0; i < argi_.Length; i++)
			{
				argi_[i] = StreamUtil.ReadInt(stream);
			}
		}
	}
}
