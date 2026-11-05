using System;
using System.IO;
using System.Linq;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.io
{
	public class Storage
	{
		private const int FILENAME_MAX = 8;

		private const int FILEEXT_MAX = 3;

		public const int MEDIA_MEMORY = 0;

		public const int MEDIA_DATAFOLDER = 1;

		public const int MEDIA_HEAP = 2;

		public const int MEDIA_RESOURCE = 3;

		protected const int MEDIA_RECORDSTORE = 4;

		public const int ACCESS_PLATFORM = 2;

		public const int ACCESS_SERIES = 4;

		public const int ACCESS_UIM = 1;

		public const int SHARE_APPLICATION = 8;

		public const int SHARE_CONTENTS_PROVIDER = 16;

		protected static int access_ = 0;

		protected static int share_ = 0;

		private static string root_ = null;

		private string folder_;

		private int media_;

		private bool security_;

		private static Vector heapName_ = new Vector();

		private static Vector heapData_ = new Vector();

		public static void Setup(int access, int share, string root)
		{
			access_ = access;
			share_ = share;
			root_ = root;
			if (!root_.EndsWith("/"))
			{
				root_ += "/";
			}
		}

		protected Storage(string folder, int media, bool security)
		{
			folder_ = folder;
			media_ = media;
			security_ = security;
		}

		protected Storage(object Null, int media, bool security)
		{
			media_ = media;
			security_ = security;
		}

		public static Storage Open(int media, bool security)
		{
			if (media == 2)
			{
				if (Config.USE_HEAP_STORAGE)
				{
					return new Storage(null, media, security);
				}
				throw new IOException("media not found");
			}
			if (media == 3)
			{
				return new Storage(null, media, security);
			}
			if (media == 1 || media == 4)
			{
				return new Storage(GetFolder(media), media, security);
			}
			return null;
		}

		public static string GetFolder(int media)
		{
			if (media == 1 || media == 4)
			{
				string text = IApplication.GetCurrentApp().GetDataPath();
				if (Application.platform == RuntimePlatform.Android)
				{
					string text2 = text;
					int num = text2.LastIndexOf("data/") + 5;
					int length = text2.LastIndexOf("/files") - num;
					string text3 = text2.Substring(num, length);
					text = "data/data/" + text3;
				}
				if (!text.EndsWith("/"))
				{
					text += "/";
				}
				if (media == 1)
				{
					text += "files";
				}
				else if (media == 4)
				{
					text += "rs";
				}
				return text;
			}
			return null;
		}

		public void Close()
		{
			folder_ = null;
		}

		private static bool _filenameCheck(string filename)
		{
			int num = filename.Length();
			int num2 = 0;
			if (filename.LastIndexOf('.') != -1)
			{
				num = filename.LastIndexOf('.');
				num2 = filename.Length() - num - 1;
			}
			if (num > 0 && num <= 8)
			{
				return num2 <= 3;
			}
			return false;
		}

		public byte[] Read(string filename)
		{
			try
			{
				filename = Config.ConvertExtension(filename);
				if (Config.USE_HEAP_STORAGE && media_ == 2)
				{
					int num = _SearchHeapFile(filename);
					if (num != -1)
					{
						return (byte[])heapData_.ElementAt(num);
					}
					return null;
				}
				if (media_ == 3)
				{
					return AssetReader.GetData(filename);
				}
				if (media_ == 1 || media_ == 4)
				{
					if (filename.Contains(':'))
					{
						if (File.Exists(filename))
						{
							return File.ReadAllBytes(filename);
						}
					}
					else if (Exists(filename))
					{
						return File.ReadAllBytes(folder_ + "/" + filename);
					}
				}
				return null;
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new NotPermitException(NotPermitException.PermissionType.Storage, ex.Message, ex);
			}
		}

		public void Write(string filename, byte[] data)
		{
			try
			{
				filename = Config.ConvertExtension(filename);
				if (Config.USE_HEAP_STORAGE && media_ == 2)
				{
					int num = _SearchHeapFile(filename);
					if (num != -1)
					{
						heapData_.SetElementAt(data, num);
						return;
					}
					heapName_.AddElement(filename);
					heapData_.AddElement(data);
				}
				else if (media_ != 3 && (media_ == 1 || media_ == 4))
				{
					if (data == null)
					{
						data = new byte[0];
					}
					string path = folder_ + "/" + filename;
					if (filename.Contains(':'))
					{
						path = filename;
					}
					string directoryName = Path.GetDirectoryName(path);
					if (!Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
					File.WriteAllBytes(path, data);
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new NotPermitException(NotPermitException.PermissionType.Storage, ex.Message, ex);
			}
		}

		public void Delete(string filename)
		{
			try
			{
				filename = Config.ConvertExtension(filename);
				if (Config.USE_HEAP_STORAGE && media_ == 2)
				{
					int num = _SearchHeapFile(filename);
					if (num != -1)
					{
						heapData_.RemoveElementAt(num);
						heapName_.RemoveElementAt(num);
					}
				}
				else
				{
					if (media_ == 3 || (media_ != 1 && media_ != 4))
					{
						return;
					}
					if (filename.Contains(':'))
					{
						if (File.Exists(filename))
						{
							File.Delete(filename);
						}
					}
					else if (Exists(filename))
					{
						File.Delete(folder_ + "/" + filename);
					}
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new NotPermitException(NotPermitException.PermissionType.Storage, ex.Message, ex);
			}
		}

		public bool Exists(string filename)
		{
			try
			{
				filename = Config.ConvertExtension(filename);
				if (Config.USE_HEAP_STORAGE && media_ == 2)
				{
					return _SearchHeapFile(filename) != -1;
				}
				if (media_ == 3)
				{
					return AssetReader.Exist(filename);
				}
				if (media_ == 1 || media_ == 4)
				{
					string path = folder_ + "/" + filename;
					if (filename.Contains(':'))
					{
						path = filename;
					}
					if (!Directory.Exists(Path.GetDirectoryName(path)))
					{
						return false;
					}
					return File.Exists(path);
				}
				return false;
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new NotPermitException(NotPermitException.PermissionType.Storage, ex.Message, ex);
			}
		}

		public int GetSize(string filename)
		{
			try
			{
				filename = Config.ConvertExtension(filename);
				if (Config.USE_HEAP_STORAGE && media_ == 2)
				{
					int num = _SearchHeapFile(filename);
					if (num != -1)
					{
						return ((byte[])heapData_.ElementAt(num)).Length;
					}
				}
				if (media_ == 3)
				{
					byte[] data = AssetReader.GetData(filename);
					if (data != null)
					{
						return data.Length;
					}
				}
				if (media_ == 1 || media_ == 4)
				{
					if (filename.Contains(':'))
					{
						if (File.Exists(filename))
						{
							return (int)new FileInfo(filename).Length;
						}
					}
					else if (Exists(filename))
					{
						return (int)new FileInfo(folder_ + "/" + filename).Length;
					}
				}
				return -1;
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new NotPermitException(NotPermitException.PermissionType.Storage, ex.Message, ex);
			}
		}

		public string[] GetFiles()
		{
			try
			{
				if (Config.USE_HEAP_STORAGE && media_ == 2)
				{
					string[] array = new string[heapName_.Size()];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = (string)heapName_.ElementAt(i);
					}
					return array;
				}
				if (media_ == 3)
				{
					return null;
				}
				if (media_ == 1 || media_ == 4)
				{
					if (!Directory.Exists(folder_))
					{
						return new string[0];
					}
					string[] files = Directory.GetFiles(folder_, "*.*", SearchOption.AllDirectories);
					for (int j = 0; j < files.Length; j++)
					{
						int num = files[j].IndexOf(folder_);
						if (num != -1)
						{
							files[j] = files[j].Substring(num + folder_.Length + 1);
						}
					}
					return files;
				}
				return null;
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new NotPermitException(NotPermitException.PermissionType.Storage, ex.Message, ex);
			}
		}

		private int _SearchHeapFile(string filename)
		{
			filename = Config.ConvertExtension(filename);
			if (Config.USE_HEAP_STORAGE && media_ == 2)
			{
				for (int i = 0; i < heapName_.Size(); i++)
				{
					if (filename.CompareTo((string)heapName_.ElementAt(i)) == 0)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static byte[] Read(int media, bool security, string filename)
		{
			filename = Config.ConvertExtension(filename);
			Storage storage = null;
			try
			{
				storage = Open(media, security);
				return storage.Read(filename);
			}
			finally
			{
				if (storage != null)
				{
					storage.Close();
				}
				storage = null;
			}
		}

		public static void Write(int media, bool security, string filename, byte[] data)
		{
			filename = Config.ConvertExtension(filename);
			Storage storage = null;
			try
			{
				storage = Open(media, security);
				storage.Write(filename, data);
			}
			finally
			{
				if (storage != null)
				{
					storage.Close();
				}
				storage = null;
			}
		}

		public static bool Exists(int media, string filename)
		{
			filename = Config.ConvertExtension(filename);
			Storage storage = null;
			try
			{
				storage = Open(media, false);
				return storage.Exists(filename);
			}
			finally
			{
				if (storage != null)
				{
					storage.Close();
				}
				storage = null;
			}
		}
	}
}
