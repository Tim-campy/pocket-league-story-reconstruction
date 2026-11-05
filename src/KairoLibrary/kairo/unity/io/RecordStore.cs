using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using java.lang;
using kairo.common.cfg;
using kairo.unity.native;
using kairo.unity.ui;
using kairo.unity.util;

namespace kairo.unity.io
{
	public class RecordStore
	{
		private const int ADDR_EXIST_FLAG = -4;

		private const int ADDR_NUM_RECORDS = -8;

		private const int RSTORE_MAX = 5;

		private const int RINFO_SIZE = 25;

		private const int RSINFO_SIZE = 8;

		private const int CRC_SIZE = 8;

		private static RecordStore[] recordStores_;

		private static RecordStoreListener listener_;

		private int rsId_;

		private static string[] resourceFiles_;

		private static Storage storage_;

		private static bool usePreference_;

		public RecordStore(int rsId)
		{
			rsId_ = rsId;
		}

		public static void SetPreferenceMode(bool use)
		{
			usePreference_ = Config.APPLI_TYPE == 2 && !Config.UNITY_EDITOR && use;
		}

		public static bool Setup()
		{
			storage_ = Storage.Open(4, true);
			return true;
		}

		public static void PreferencesToFile()
		{
			if (usePreference_)
			{
				return;
			}
			string[] preferenceKeys = KairoPlugin.GetPreferenceKeys();
			List<object[]> list = new List<object[]>();
			for (int i = 0; i < preferenceKeys.Length; i++)
			{
				int result;
				if (preferenceKeys.Length >= 4 && int.TryParse(preferenceKeys[i], out result))
				{
					byte[] preference = KairoPlugin.GetPreference(preferenceKeys[i]);
					list.Add(new object[2]
					{
						preferenceKeys[i],
						preference
					});
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				string text = (string)list[j][0];
				byte[] array = (byte[])list[j][1];
				int num = int.Parse(text);
				int rsId = num / 1000;
				int rcId = num % 1000;
				RecordStore recordStore = null;
				try
				{
					recordStore = new RecordStore(rsId);
					if (!recordStore.ExistRecord(rcId) && array.Length != 0)
					{
						string fileName = GetFileName(rsId, rcId);
						storage_.Write(fileName, array);
					}
					KairoPlugin.RemovePreference(text);
				}
				catch (Exception t)
				{
					Log.Info(t);
				}
				finally
				{
					if (recordStore != null)
					{
						recordStore.CloseRecordStore();
					}
					recordStore = null;
				}
			}
		}

		public static void FileToPreferences()
		{
			if (!usePreference_)
			{
				return;
			}
			string[] files = storage_.GetFiles();
			List<object[]> list = new List<object[]>();
			for (int i = 0; i < files.Length; i++)
			{
				int result;
				if (files.Length >= 4 && int.TryParse(files[i], out result))
				{
					byte[] array = storage_.Read(files[i]);
					list.Add(new object[2]
					{
						files[i],
						array
					});
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				string text = (string)list[j][0];
				byte[] array2 = (byte[])list[j][1];
				int num = int.Parse(text);
				int rsId = num / 1000;
				int rcId = num % 1000;
				RecordStore recordStore = null;
				try
				{
					recordStore = new RecordStore(rsId);
					if (!recordStore.ExistRecord(rcId) && array2.Length != 0)
					{
						KairoPlugin.PutPreference(GetFileName(rsId, rcId), array2);
					}
					storage_.Delete(text);
				}
				catch (Exception t)
				{
					Log.Info(t);
				}
				finally
				{
					if (recordStore != null)
					{
						recordStore.CloseRecordStore();
					}
					recordStore = null;
				}
			}
		}

		public static void CreateRecordStore()
		{
			Setup();
		}

		public int AddRecord(int size)
		{
			if (rsId_ == 1 && resourceFiles_ != null)
			{
				return -1;
			}
			int numRecords = GetNumRecords();
			string fileName = GetFileName(rsId_, numRecords);
			if (!usePreference_)
			{
				storage_.Write(fileName, new byte[0]);
			}
			else
			{
				KairoPlugin.PutPreference(fileName, new byte[0]);
			}
			return numRecords;
		}

		public int AddRecord(byte[] data)
		{
			if (rsId_ == 1 && resourceFiles_ != null)
			{
				return -1;
			}
			int num = AddRecord(data.Length);
			WriteRecord(num, data);
			return num;
		}

		public void WriteRecord(int rcId, byte[] data)
		{
			WriteRecord(rcId, new byte[1][] { data });
		}

		public void WriteRecord(int rcId, byte[][] data)
		{
			if (rsId_ != 1 || resourceFiles_ == null)
			{
				string fileName = GetFileName(rsId_, rcId);
				byte[] array = data[0];
				data[0] = null;
				data = null;
				byte[] array2 = new byte[array.Length + 8];
				JSystem.Arraycopy(EncodeUtil.LongToByteAry(CRC64.GetValue(array), true), 0, array2, 0, 8);
				JSystem.Arraycopy(array, 0, array2, 8, array.Length);
				Encrypter.Encode(array2);
				if (!usePreference_)
				{
					storage_.Write(fileName, array2);
				}
				else
				{
					KairoPlugin.PutPreference(fileName, array2);
				}
			}
		}

		public bool ExistRecord(int rcId)
		{
			if (rsId_ == 1 && resourceFiles_ != null)
			{
				return true;
			}
			IApplication.GetCurrentApp();
			string fileName = GetFileName(rsId_, rcId);
			if (!usePreference_)
			{
				return storage_.Exists(fileName);
			}
			return KairoPlugin.GetPreferenceKeys().Contains(fileName);
		}

		public byte[] ReadRecord(int rcId)
		{
			if (rsId_ == 1 && resourceFiles_ != null)
			{
				return Storage.Read(3, false, resourceFiles_[rcId] + (resourceFiles_[rcId].EndsWith(".dat") ? "" : ".dat"));
			}
			IApplication.GetCurrentApp();
			string fileName = GetFileName(rsId_, rcId);
			byte[] array = null;
			array = (usePreference_ ? KairoPlugin.GetPreference(fileName) : storage_.Read(fileName));
			if (array == null)
			{
				array = new byte[0];
			}
			if (array.Length == 0)
			{
				return array;
			}
			Encrypter.Decode(array);
			long num = EncodeUtil.ByteAryToLong(array, true);
			byte[] array2 = new byte[array.Length - 8];
			JSystem.Arraycopy(array, 8, array2, 0, array2.Length);
			array = null;
			if (CRC64.GetValue(array2) != num)
			{
				throw new IllegalFileException();
			}
			return array2;
		}

		public void RemoveLastRecord()
		{
			if (rsId_ == 1 && resourceFiles_ != null)
			{
				return;
			}
			int numRecords = GetNumRecords();
			if (numRecords != 0)
			{
				string fileName = GetFileName(rsId_, numRecords - 1);
				if (!usePreference_)
				{
					storage_.Delete(fileName);
				}
				else
				{
					KairoPlugin.RemovePreference(fileName);
				}
			}
		}

		public void RemoveAllRecord()
		{
			if (rsId_ == 1 && resourceFiles_ != null)
			{
				return;
			}
			if (!usePreference_)
			{
				string[] files = storage_.GetFiles();
				foreach (string filename in files)
				{
					storage_.Delete(filename);
				}
			}
			else
			{
				string[] files = KairoPlugin.GetPreferenceKeys();
				for (int i = 0; i < files.Length; i++)
				{
					KairoPlugin.RemovePreference(files[i]);
				}
			}
		}

		public int GetRecordSize(int rcId)
		{
			if (rsId_ == 1 && resourceFiles_ != null)
			{
				return 0;
			}
			string fileName = GetFileName(rsId_, rcId);
			if (!usePreference_)
			{
				return storage_.GetSize(fileName);
			}
			byte[] preference = KairoPlugin.GetPreference(fileName);
			if (preference == null)
			{
				return -1;
			}
			return preference.Length;
		}

		public int GetNumRecords()
		{
			if (rsId_ == 1 && resourceFiles_ != null)
			{
				return 0;
			}
			IApplication.GetCurrentApp();
			int num = 0;
			string[] array = null;
			array = (usePreference_ ? KairoPlugin.GetPreferenceKeys() : storage_.GetFiles());
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text != null && text.CharAt(0) == 48 + rsId_)
				{
					int num2 = JInteger.ParseInt(Path.GetFileNameWithoutExtension(text)) % 1000;
					if (num <= num2)
					{
						num = num2 + 1;
					}
				}
			}
			return num;
		}

		public static RecordStore OpenRecordStore(int rsId)
		{
			return new RecordStore(rsId);
		}

		public static void WriteRecord(int rsId, int rcId, byte[] data)
		{
			RecordStore recordStore = null;
			try
			{
				recordStore = OpenRecordStore(rsId);
				recordStore.WriteRecord(rcId, data);
			}
			finally
			{
				if (recordStore != null)
				{
					recordStore.CloseRecordStore();
				}
				recordStore = null;
			}
		}

		public static byte[] ReadRecord(int rsId, int rcId)
		{
			RecordStore recordStore = null;
			try
			{
				recordStore = OpenRecordStore(rsId);
				return recordStore.ReadRecord(rcId);
			}
			finally
			{
				if (recordStore != null)
				{
					recordStore.CloseRecordStore();
				}
				recordStore = null;
			}
		}

		public void CloseRecordStore()
		{
		}

		public int GetSizeAvailable()
		{
			return 1048576;
		}

		public int GetCapacity()
		{
			return GetSizeAvailable() + GetSize();
		}

		public int GetSize()
		{
			return 0;
		}

		public static int GetRecordInfoSize()
		{
			return 33;
		}

		private int GetDataWriteSize(int size)
		{
			return 9 + size;
		}

		public static void SetListener(RecordStoreListener listener)
		{
			listener_ = listener;
		}

		private static string GetFileName(int rsId, int rcId)
		{
			return rsId + StringUtil.FormatString(JString.ValueOf(rcId), 3, '0', false);
		}

		public static void SetResourceFiles(string[] files)
		{
			resourceFiles_ = files;
		}
	}
}
