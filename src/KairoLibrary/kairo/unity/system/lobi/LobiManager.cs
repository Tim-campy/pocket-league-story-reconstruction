using System;
using java.io;
using java.util;
using kairo.common.cfg;
using kairo.unity.native;
using kairo.unity.util;

namespace kairo.unity.system.lobi
{
	public abstract class LobiManager
	{
		protected static LobiManager instance_;

		public const int TYPE_CHAT = 0;

		public const int TYPE_RANKING = 1;

		private SecureString country_;

		protected LobiManager()
		{
			country_ = new SecureString(JLocale.GetDefault().GetCountry());
		}

		public static LobiManager GetInstance()
		{
			return instance_;
		}

		protected abstract long[][] GetScoreList();

		private byte[] WriteSendData(int type, int number)
		{
			ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
			StreamUtil.WriteInt(byteArrayOutputStream, Config.PID);
			StreamUtil.WriteLong(byteArrayOutputStream, number);
			if (type == 1)
			{
				long[][] scoreList = GetScoreList();
				StreamUtil.WriteInt(byteArrayOutputStream, scoreList.Length);
				foreach (long[] array in scoreList)
				{
					StreamUtil.WriteInt(byteArrayOutputStream, array.Length);
					for (int j = 0; j < array.Length; j++)
					{
						StreamUtil.WriteLong(byteArrayOutputStream, array[j]);
					}
				}
			}
			return byteArrayOutputStream.ToByteArray();
		}

		public void Open(int type, int number)
		{
			if (!IsEnabled())
			{
				return;
			}
			KairoBase.GetInstance().SaveAll();
			SecureString secureString = IntentUtil.PackSendData(WriteSendData(type, number));
			try
			{
				KairoPlugin.LaunchLobi(secureString, type, number);
			}
			catch (Exception t)
			{
				Log.Info("LobiManager: Open Lobi failed.");
				if (Config.PRINT)
				{
					Log.Info(t);
				}
			}
		}

		protected abstract bool IsGlobal();

		public bool IsEnabled()
		{
			if (!IsGlobal() && !country_.Equals("JP"))
			{
				return false;
			}
			return true;
		}
	}
}
