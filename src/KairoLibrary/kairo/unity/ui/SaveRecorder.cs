using System;
using System.Threading;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.form;
using kairo.unity.native;
using kairo.unity.system;
using kairo.unity.util;

namespace kairo.unity.ui
{
	public abstract class SaveRecorder
	{
		public delegate void PreSaveMethod();

		public delegate void PostSaveMethod(byte[] save);

		public delegate void PrePackMethod();

		public delegate void PostPackMethod(byte[][] data);

		public delegate byte[] PackMethod(int id);

		private struct AutoSaveLogData
		{
			public long LockTotalTime;

			public long LockTime;

			public int TryCount;

			public int[] DataIds;

			public int[] DataMis;
		}

		protected static SaveRecorder objectInstance_;

		private bool existAutoSave_;

		private long[] lastSaveTime_ = new long[2];

		private bool[] saveReserve_ = new bool[2];

		private JThread lockThread_;

		private int lockThreadCount_;

		private Property asProperty_;

		private PackMethod asPackMethod_;

		private PreSaveMethod asPreSaveMethod_;

		private PostSaveMethod asPostSaveMethod_;

		private PrePackMethod asPrePackMethod_;

		private PostPackMethod asPostPackMethod_;

		private bool asRedo_;

		private bool asCancel_;

		private bool asLog_;

		private JThread asThread_;

		private object SaveGame_lock = new object();

		protected SaveRecorder(bool existAutoSave)
		{
			objectInstance_ = this;
			existAutoSave_ = existAutoSave;
		}

		public static SaveRecorder GetInstance()
		{
			return objectInstance_;
		}

		public void SaveGame()
		{
			SaveGame(false, true);
		}

		public void SaveGame(bool auto)
		{
			SaveGame(auto, true);
		}

		public void SaveGame(bool auto, bool immediate)
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				lock (SaveGame_lock)
				{
					if (!immediate)
					{
						saveReserve_[auto ? 1 : 0] = true;
						return;
					}
					Lock();
					KairoBase instance = KairoBase.GetInstance();
					try
					{
						if (!auto)
						{
							CancelAutoSave();
						}
						instance.OnSaveGameBegin();
						OnSaveGame(auto);
						instance.OnSaveGameEnd();
					}
					finally
					{
						Unlock();
					}
				}
			}, true);
		}

		public void SaveSystem()
		{
			IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				lock (SaveGame_lock)
				{
					OnSaveSystem();
				}
			}, true);
		}

		public abstract void OnSaveGame(bool auto);

		public abstract void OnSaveSystem();

		public void Lock()
		{
			JThread jThread = JThread.CurrentThread();
			while (lockThreadCount_ > 0 && lockThread_ != jThread)
			{
				try
				{
					JThread.Sleep(10L);
				}
				catch (ThreadAbortException)
				{
				}
			}
			lockThread_ = JThread.CurrentThread();
			lockThreadCount_++;
		}

		public void Unlock()
		{
			Unlock(false);
		}

		public void Unlock(bool forced)
		{
			lockThreadCount_--;
			if (lockThreadCount_ <= 0 || forced)
			{
				lockThreadCount_ = 0;
				lockThread_ = null;
			}
			if (!forced)
			{
				return;
			}
			for (int i = 0; i < saveReserve_.Length; i++)
			{
				if (!saveReserve_[i])
				{
					continue;
				}
				try
				{
					saveReserve_[i] = false;
					SaveGame(i == 1, true);
				}
				catch (Exception t)
				{
					if (Config.PRINT)
					{
						Log.Info(t);
					}
				}
			}
		}

		protected void SaveGame(bool auto, Property property, PackMethod packMethod, PreSaveMethod preSaveMethod, PostSaveMethod postSaveMethod, PrePackMethod prePackMethod, PostPackMethod postPackMethod, bool log)
		{
			if (IApplication.GetCurrentApp().IsTermination() || !auto)
			{
				CancelAutoSave();
				byte[][] array = new byte[property.bins.Length][];
				Array.Copy(property.bins, array, array.Length);
				preSaveMethod?.Invoke();
				prePackMethod?.Invoke();
				for (int i = 0; i < property.bins.Length; i++)
				{
					property.bins[i] = packMethod(i);
				}
				postPackMethod?.Invoke(property.bins);
				postSaveMethod?.Invoke(property.ToByteArray());
				Array.Copy(array, property.bins, array.Length);
			}
			else if (asThread_ != null)
			{
				asRedo_ = true;
			}
			else
			{
				asProperty_ = property;
				asPackMethod_ = packMethod;
				asPreSaveMethod_ = preSaveMethod;
				asPostSaveMethod_ = postSaveMethod;
				asPrePackMethod_ = prePackMethod;
				asPostPackMethod_ = postPackMethod;
				asLog_ = log;
				asRedo_ = false;
				asThread_ = new JThread(AutoSaveRun);
				asThread_.Start();
			}
		}

		private void AutoSaveRun()
		{
			FormManagerBase formManager = IApplication.GetCurrentApp().GetFormManager();
			try
			{
				do
				{
					asRedo_ = false;
					long num = JSystem.CurrentTimeMillis();
					if (asPreSaveMethod_ != null)
					{
						asPreSaveMethod_();
					}
					if (asCancel_)
					{
						break;
					}
					if (asRedo_)
					{
						continue;
					}
					long num2 = JSystem.CurrentTimeMillis();
					AutoSaveLogData log;
					Property property = MakeSaveData(asProperty_, out log);
					long num3 = JSystem.CurrentTimeMillis() - num2;
					if (asCancel_)
					{
						break;
					}
					if (asRedo_)
					{
						continue;
					}
					if (asPostSaveMethod_ != null)
					{
						asPostSaveMethod_(property.ToByteArray());
					}
					if (asCancel_)
					{
						break;
					}
					if (asRedo_ || !asLog_)
					{
						continue;
					}
					string text = "";
					for (int i = 0; i < log.DataIds.Length; i++)
					{
						if (log.DataMis[i] != 0)
						{
							if (text.Length != 0)
							{
								text += ",";
							}
							text += log.DataIds[i];
						}
					}
					string text2 = "try : " + log.TryCount + "\npack : " + num3 + " ms\ntotal : " + (JSystem.CurrentTimeMillis() - num) + " ms";
					if (text.Length > 0)
					{
						text2 = text2 + "\nmiss : " + text;
					}
					if (log.LockTotalTime > 0)
					{
						text2 = text2 + "\nlock : " + log.LockTime + " / " + log.LockTotalTime + " ms";
					}
					KairoPlugin.ShowToast(text2);
				}
				while (asRedo_);
			}
			catch (Exception ex)
			{
				if (Config.PRINT)
				{
					Log.Info(ex);
				}
				if (asLog_)
				{
					KairoPlugin.ShowToast(ex.ToString());
				}
			}
			finally
			{
				formManager.UnlockUpdate();
				asThread_ = null;
			}
		}

		private Property MakeSaveData(Property src, out AutoSaveLogData log)
		{
			FormManagerBase formManager = IApplication.GetCurrentApp().GetFormManager();
			Property dst = new Property(src);
			log = default(AutoSaveLogData);
			int num = src.bins.Length;
			byte[][][] array = JTool.MakeArray<byte[][][]>(new int[3] { 2, num, 0 });
			int[] array2 = new int[num];
			int[] array3 = new int[num];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = i;
			}
			log.DataIds = array2;
			log.DataMis = array3;
			int num2 = 0;
			bool flag;
			do
			{
				if (IsBreak())
				{
					return null;
				}
				log.TryCount++;
				flag = log.TryCount >= 5;
				if (flag)
				{
					for (int j = 0; j < array3.Length; j++)
					{
						array3[j]++;
					}
				}
				switch (num2)
				{
				case 0:
				{
					for (int n = 0; n < array.Length; n++)
					{
						formManager.WaitLateUpdate();
						long updateStartTime = formManager.GetUpdateStartTime();
						if (IsBreak())
						{
							return null;
						}
						if (asPrePackMethod_ != null)
						{
							asPrePackMethod_();
						}
						if (IsBreak())
						{
							return null;
						}
						CopyProperty(dst, src);
						for (int num5 = 0; num5 < array2.Length; num5++)
						{
							if (IsBreak())
							{
								return null;
							}
							try
							{
								array[n][array2[num5]] = asPackMethod_(array2[num5]);
							}
							catch
							{
								array[n][array2[num5]] = null;
							}
							if (IsBreak())
							{
								return null;
							}
						}
						if (IsBreak())
						{
							return null;
						}
						if (asPostPackMethod_ != null)
						{
							asPostPackMethod_(array[n]);
						}
						if (IsBreak())
						{
							return null;
						}
						if (updateStartTime == formManager.GetUpdateStartTime())
						{
							return SetBin(dst, array[n]);
						}
					}
					num2 = 1;
					break;
				}
				case 1:
				{
					int num3 = 0;
					for (int k = 0; k < array3.Length; k++)
					{
						if (array3[k] > 0)
						{
							num3++;
						}
					}
					formManager.WaitLateUpdate();
					formManager.LockUpdate();
					if (IsBreak())
					{
						return null;
					}
					if (asPrePackMethod_ != null)
					{
						asPrePackMethod_();
					}
					if (IsBreak())
					{
						return null;
					}
					CopyProperty(dst, src);
					for (int l = 0; l < num3; l++)
					{
						if (IsBreak())
						{
							return null;
						}
						try
						{
							array[0][array2[l]] = (array[1][array2[l]] = asPackMethod_(array2[l]));
						}
						catch
						{
							array[1][array2[l]] = null;
						}
						if (IsBreak())
						{
							return null;
						}
					}
					long num4 = formManager.UnlockUpdate();
					log.LockTotalTime += num4;
					log.LockTime = num4;
					for (int m = num3; m < array2.Length; m++)
					{
						if (IsBreak())
						{
							return null;
						}
						array[0][array2[m]] = array[1][array2[m]];
						try
						{
							array[1][array2[m]] = asPackMethod_(array2[m]);
						}
						catch
						{
							array[1][array2[m]] = null;
						}
						if (IsBreak())
						{
							return null;
						}
					}
					if (IsBreak())
					{
						return null;
					}
					if (asPostPackMethod_ != null)
					{
						asPostPackMethod_(array[1]);
					}
					if (IsBreak())
					{
						return null;
					}
					break;
				}
				}
				if (IsBreak())
				{
					return null;
				}
				bool flag2 = true;
				for (int num6 = 0; num6 < array2.Length; num6++)
				{
					int num7 = array2[num6];
					bool flag3 = array[0][num7] != null && array[1][num7] != null && array[0][num7].Length == array[1][num7].Length;
					if (flag3)
					{
						for (int num8 = 0; num8 < array[0][num7].Length; num8++)
						{
							if (array[0][num7][num8] != array[1][num7][num8])
							{
								flag3 = false;
								break;
							}
						}
					}
					if (!flag3)
					{
						array3[num6]++;
						flag2 = false;
					}
				}
				if (IsBreak())
				{
					return null;
				}
				if (flag2)
				{
					return SetBin(dst, array[1]);
				}
				for (int num9 = 0; num9 < array2.Length - 1; num9++)
				{
					for (int num10 = array2.Length - 1; num10 > num9; num10--)
					{
						if (array3[num10 - 1] < array3[num10 + 0])
						{
							int num11 = array2[num10];
							array2[num10] = array2[num10 - 1];
							array2[num10 - 1] = num11;
							num11 = array3[num10];
							array3[num10] = array3[num10 - 1];
							array3[num10 - 1] = num11;
						}
					}
				}
			}
			while (!flag);
			throw new Exception("serialize failure.");
		}

		private bool IsBreak()
		{
			if (!asCancel_)
			{
				return asRedo_;
			}
			return true;
		}

		private void CopyProperty(Property dst, Property src)
		{
			if (dst == null && src == null)
			{
				return;
			}
			if (src.ints == null)
			{
				dst.ints = null;
			}
			else if (dst.ints == null || src.ints.Length != dst.ints.Length)
			{
				dst.ints = new int[src.ints.Length];
			}
			if (src.ints != null)
			{
				Array.Copy(src.ints, dst.ints, src.ints.Length);
			}
			if (src.lngs == null)
			{
				dst.lngs = null;
			}
			else if (dst.lngs == null || src.lngs.Length != dst.lngs.Length)
			{
				dst.lngs = new long[src.lngs.Length];
			}
			if (src.lngs != null)
			{
				Array.Copy(src.lngs, dst.lngs, src.lngs.Length);
			}
			if (src.strs == null)
			{
				dst.strs = null;
			}
			else if (dst.strs == null || src.strs.Length != dst.strs.Length)
			{
				dst.strs = new string[src.strs.Length];
			}
			if (src.strs != null)
			{
				Array.Copy(src.strs, dst.strs, src.strs.Length);
			}
			if (src.pros == null)
			{
				return;
			}
			for (int i = 0; i < src.pros.Length; i++)
			{
				if (src.pros[i] == null)
				{
					dst.pros[i] = null;
				}
				else if (dst.pros[i] == null)
				{
					dst.pros[i] = new Property();
				}
				CopyProperty(dst.pros[i], src.pros[i]);
			}
		}

		private Property SetBin(Property dst, byte[][] src)
		{
			Array.Copy(src, dst.bins, src.Length);
			return dst;
		}

		public bool IsAutoSaving()
		{
			return asThread_ != null;
		}

		public void CancelAutoSave()
		{
			asCancel_ = true;
			while (asThread_ != null)
			{
				JThread.Sleep(1L);
			}
			asCancel_ = false;
		}
	}
}
