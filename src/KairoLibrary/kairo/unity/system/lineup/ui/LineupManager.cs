using System;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.io;
using kairo.unity.ui;
using kairo.unity.util;

namespace kairo.unity.system.lineup.ui
{
	public class LineupManager
	{
		protected static LineupManager instance_;

		private string appliLinkUrl_;

		private LineupData[] lineupList_ = new LineupData[0];

		private int[] showCount_ = new int[0];

		protected LineupData[] showLineups_ = new LineupData[0];

		protected LineupManager()
		{
		}

		public static LineupManager GetInstance()
		{
			return instance_;
		}

		public void Init()
		{
		}

		public void Load(byte[] data)
		{
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(data);
				int num = StreamUtil.ReadShort(byteArrayInputStream);
				showCount_ = new int[num];
				for (int i = 0; i < showCount_.Length; i++)
				{
					showCount_[i] = StreamUtil.ReadByte(byteArrayInputStream);
				}
			}
			finally
			{
				byteArrayInputStream?.Close();
				byteArrayInputStream = null;
			}
		}

		public byte[] Write()
		{
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				StreamUtil.WriteShort(byteArrayOutputStream, (short)showCount_.Length);
				for (int i = 0; i < showCount_.Length; i++)
				{
					StreamUtil.WriteByte(byteArrayOutputStream, (byte)showCount_[i]);
				}
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				byteArrayOutputStream?.Close();
				byteArrayOutputStream = null;
			}
		}

		private void LoadLineupList(byte[] data)
		{
			try
			{
				data = Compressor.Expand(data);
				Vector vector = new Vector();
				string[] array = StringUtil.Split(JString.New(data, "UTF-8"), "\n");
				LineupData[][] array2 = new LineupData[array.Length][];
				for (int i = 0; i < array.Length; i++)
				{
					string[] array3 = StringUtil.Split(array[i], "\t");
					int gameId = JInteger.ParseInt(array3[0]);
					LineupData[] array4 = new LineupData[array3.Length - 1];
					for (int j = 1; j < array3.Length; j++)
					{
						LineupData lineupData = new LineupData();
						lineupData.Load(gameId, array3[j]);
						if (lineupData.url_.IndexOf("http://") == -1)
						{
							lineupData.url_ = StringUtil.Replace(appliLinkUrl_, lineupData.url_);
						}
						if (lineupData.GetRatio() > 0)
						{
							array4[j - 1] = lineupData;
						}
					}
					array2[i] = array4;
				}
				for (int num = Language.Get(); num >= 0; num--)
				{
					for (int k = 0; k < array2.Length; k++)
					{
						if (array2[k] != null && num < array2[k].Length && array2[k][num] != null)
						{
							vector.AddElement(array2[k][num]);
						}
					}
					if (vector.Size() > 0)
					{
						break;
					}
				}
				lineupList_ = new LineupData[vector.Size()];
				for (int l = 0; l < lineupList_.Length; l++)
				{
					lineupList_[l] = (LineupData)vector.ElementAt(l);
				}
			}
			catch (Exception t)
			{
				if (Config.PRINT)
				{
					Log.Info(t);
				}
			}
		}

		public void Start(int num)
		{
			KairoBase instance = KairoBase.GetInstance();
			string text = IApplication.GetCurrentApp().GetPackageName();
			int num2 = text.LastIndexOf('.');
			if (num2 != -1)
			{
				text = text.SubstringJ(num2 + 1);
			}
			appliLinkUrl_ = "market://details?id=<0>&referrer=utm_source%3Dappli%26utm_medium%3Dhouse_ad%26utm_content%3D" + text + "_title%26utm_campaign%3Dbanner";
			string key = "update.txt";
			string key2 = "list.bin";
			JarInflater jarInflater = null;
			try
			{
				byte[] array = Storage.Read(3, false, "lineup.dat");
				if (array == null)
				{
					return;
				}
				jarInflater = new JarInflater(array);
				string time = "";
				try
				{
					byte[] preferences = GetPreferences(key);
					if (preferences != null)
					{
						time = JString.New(preferences);
					}
				}
				catch (Exception t)
				{
					if (Config.PRINT)
					{
						Log.Info(t);
					}
				}
				try
				{
					byte[][] array2 = instance.ConnectLineupList(time);
					if (array2 != null)
					{
						PutPreferences(key2, array2[0]);
						PutPreferences(key, array2[1]);
					}
				}
				catch (Exception t2)
				{
					if (Config.PRINT)
					{
						Log.Info(t2);
					}
				}
				byte[] array3 = null;
				try
				{
					array3 = GetPreferences(key2);
				}
				catch (Exception t3)
				{
					if (Config.PRINT)
					{
						Log.Info(t3);
					}
				}
				if (array3 == null)
				{
					try
					{
						array3 = jarInflater.GetData("list.bin");
					}
					catch (Exception t4)
					{
						if (Config.PRINT)
						{
							Log.Info(t4);
						}
					}
				}
				if (array3 == null)
				{
					return;
				}
				LoadLineupList(array3);
				int num3 = 0;
				for (int i = 0; i < lineupList_.Length; i++)
				{
					if (num3 < lineupList_[i].gameId_)
					{
						num3 = lineupList_[i].gameId_;
					}
				}
				int num4 = num3 + 1;
				if (num4 < showCount_.Length)
				{
					num4 = showCount_.Length;
				}
				int[] dst = new int[num4];
				if (showCount_.Length != 0)
				{
					JSystem.Arraycopy(showCount_, 0, dst, 0, showCount_.Length);
				}
				showCount_ = dst;
				int[] array4 = new int[lineupList_.Length];
				for (int j = 0; j < lineupList_.Length; j++)
				{
					LineupData lineupData = lineupList_[j];
					array4[j] = lineupData.GetRatio();
					if (showCount_[lineupData.gameId_] == 0 && lineupData.GetPriority() > 0)
					{
						array4[j] += 10000000;
					}
					if (lineupData.gameId_ == Config.PID && !Config.TRIAL_VERSION)
					{
						array4[j] = 0;
					}
				}
				JRandom jRandom = new JRandom();
				jRandom.SetSeed(JSystem.CurrentTimeMillis());
				Vector vector = new Vector();
				while (vector.Size() < num)
				{
					LineupData lineupData2 = null;
					int num5 = 0;
					for (int k = 0; k < array4.Length; k++)
					{
						num5 += array4[k];
					}
					if (num5 <= 0)
					{
						break;
					}
					int num6 = jRandom.NextInt(num5);
					num2 = -1;
					num5 = 0;
					for (int l = 0; l < array4.Length; l++)
					{
						num5 += array4[l];
						if (num6 < num5)
						{
							num2 = l;
							break;
						}
					}
					if (num2 == -1)
					{
						break;
					}
					lineupData2 = lineupList_[num2];
					try
					{
						string fileName = lineupData2.GetFileName();
						if (jarInflater.Exist(fileName))
						{
							lineupData2.image_ = Image.Load(jarInflater.GetData(fileName));
						}
						else if (ExistPreferences(fileName))
						{
							lineupData2.image_ = Image.Load(GetPreferences(fileName));
						}
						else
						{
							byte[][] array5 = instance.ConnectLineupImage(fileName);
							if (array5.Length != 0)
							{
								PutPreferences(fileName, array5[0]);
								lineupData2.image_ = Image.Load(array5[0]);
							}
						}
					}
					catch (Exception t5)
					{
						if (Config.PRINT)
						{
							Log.Info(t5);
						}
					}
					if (lineupData2.image_ != null)
					{
						vector.AddElement(lineupData2);
					}
					array4[num2] = 0;
				}
				if (vector.Size() < num)
				{
					UnloadImage();
					return;
				}
				showLineups_ = new LineupData[num];
				for (int m = 0; m < showLineups_.Length; m++)
				{
					showLineups_[m] = (LineupData)vector.ElementAt(m);
					int gameId_ = showLineups_[m].gameId_;
					if (gameId_ < showCount_.Length)
					{
						showCount_[gameId_]++;
					}
				}
			}
			catch (Exception t6)
			{
				if (Config.PRINT)
				{
					Log.Info(t6);
				}
			}
			finally
			{
				jarInflater?.Close();
				jarInflater = null;
			}
		}

		public void UnloadImage()
		{
			if (lineupList_ == null)
			{
				return;
			}
			for (int i = 0; i < lineupList_.Length; i++)
			{
				if (lineupList_[i] != null && lineupList_[i].image_ != null)
				{
					lineupList_[i].image_.Dispose();
					lineupList_[i].image_ = null;
				}
			}
		}

		public bool CheckShowableLineup()
		{
			return showLineups_.Length != 0;
		}

		public LineupData[] GetShowableLineups()
		{
			return showLineups_;
		}

		public LineupData[] GetLineups()
		{
			if (lineupList_ != null && lineupList_.Length != 0)
			{
				return lineupList_;
			}
			KairoBase.GetInstance();
			string text = IApplication.GetCurrentApp().GetPackageName();
			int num = text.LastIndexOf('.');
			if (num != -1)
			{
				text = text.SubstringJ(num + 1);
			}
			appliLinkUrl_ = "market://details?id=<0>&referrer=utm_source%3Dappli%26utm_medium%3Dhouse_ad%26utm_content%3D" + text + "_title%26utm_campaign%3Dbanner";
			string key = "list.bin";
			JarInflater jarInflater = null;
			try
			{
				byte[] array = Storage.Read(3, false, "lineup.dat");
				if (array != null)
				{
					jarInflater = new JarInflater(array);
					byte[] array2 = null;
					try
					{
						array2 = GetPreferences(key);
					}
					catch (Exception t)
					{
						if (Config.PRINT)
						{
							Log.Info(t);
						}
					}
					if (array2 == null)
					{
						try
						{
							array2 = jarInflater.GetData("list.bin");
						}
						catch (Exception t2)
						{
							if (Config.PRINT)
							{
								Log.Info(t2);
							}
						}
					}
					if (array2 == null)
					{
						return null;
					}
					LoadLineupList(array2);
				}
			}
			catch (Exception t3)
			{
				if (Config.PRINT)
				{
					Log.Info(t3);
				}
			}
			finally
			{
				if (jarInflater != null)
				{
					jarInflater.Close();
				}
				jarInflater = null;
			}
			return lineupList_;
		}

		private static void PutPreferences(string key, byte[] data)
		{
			IApplication.GetCurrentApp();
			key = "lineup/" + key;
			if (data == null)
			{
				data = new byte[0];
			}
			Storage.Write(1, true, key, data);
		}

		private static byte[] GetPreferences(string key)
		{
			IApplication.GetCurrentApp();
			key = "lineup/" + key;
			return Storage.Read(1, true, key);
		}

		private static bool ExistPreferences(string key)
		{
			IApplication.GetCurrentApp();
			key = "lineup/" + key;
			Storage storage = Storage.Open(1, true);
			try
			{
				return storage.Exists(key);
			}
			finally
			{
				storage.Close();
			}
		}
	}
}
