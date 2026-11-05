using System;
using System.Collections.Generic;
using System.Linq;
using java.io;
using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.ui;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.io
{
	public class AssetReader
	{
		protected static string country_;

		protected static string language_;

		protected static int dpi_;

		private static long[] crcTable_;

		private static bool encrypt_;

		protected static readonly string[] DPI_FOLDERS = new string[4] { "drawable-ldpi", "drawable-mdpi", "drawable-hdpi", "drawable-xhdpi" };

		protected const int DPI_LDPI = 0;

		protected const int DPI_MDPI = 1;

		protected const int DPI_HDPI = 2;

		protected const int DPI_XHDPI = 3;

		protected static Dictionary<string, bool> filelist_;

		private AssetReader()
		{
		}

		private static void _init()
		{
			int num = (int)(160f * DisplayMetrics.Density + 0.5f);
			if (0 <= num && num < 140)
			{
				dpi_ = 0;
			}
			else if (140 <= num && num < 180)
			{
				dpi_ = 1;
			}
			else if (190 <= num && num < 250)
			{
				dpi_ = 2;
			}
			else if (250 <= num)
			{
				dpi_ = 3;
			}
			JLocale jLocale = JLocale.GetDefault();
			language_ = jLocale.GetLanguage().ToLowerCase();
			country_ = jLocale.GetCountry().ToLowerCase();
		}

		public static InputStream GetInputStream(string file)
		{
			if (!Exist(file))
			{
				return null;
			}
			return new ByteArrayInputStream(GetData(file));
		}

		private static InputStream _getInputStream(string file)
		{
			return (InputStream)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				file = Config.ConvertExtension(file);
				string[] accessFilesList = GetAccessFilesList(file);
				for (int i = 0; i < accessFilesList.Length; i++)
				{
					InputStream inputStream = _open(accessFilesList[i]);
					if (inputStream != null)
					{
						return inputStream;
					}
				}
				return (object)null;
			}, true);
		}

		public static string[] GetAccessFilesList(string file)
		{
			if (country_ == null)
			{
				_init();
			}
			file = Config.ConvertExtension(file);
			string[] array = StringUtil.Split(file, "/");
			file = array[array.Length - 1];
			string text = "";
			for (int i = 0; i < array.Length - 1; i++)
			{
				text = text + array[i] + "/";
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			if (Language.GetLanguagePackFolders() != null)
			{
				string[] languagePackFolders = Language.GetLanguagePackFolders();
				foreach (string text2 in languagePackFolders)
				{
					list2.Add(text2 + "/");
				}
			}
			list2.Add(Language.GetFolder() + "/");
			list2.Add("");
			for (int k = dpi_; k < DPI_FOLDERS.Length; k++)
			{
				foreach (string item in list2)
				{
					list.Add(text + DPI_FOLDERS[k] + "/" + item + file);
				}
			}
			for (int num = dpi_ - 1; num >= 0; num--)
			{
				foreach (string item2 in list2)
				{
					list.Add(text + DPI_FOLDERS[num] + "/" + item2 + file);
				}
			}
			foreach (string item3 in list2)
			{
				list.Add(text + item3 + file);
			}
			return list.ToArray();
		}

		private static InputStream _open(string file)
		{
			byte[] array = _getData(file);
			if (array == null)
			{
				return null;
			}
			return new ByteArrayInputStream(array);
		}

		private static byte[] _getData(string file)
		{
			return (byte[])IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				TextAsset textAsset = _getTextAsset(file);
				if (textAsset == null)
				{
					return (object)null;
				}
				try
				{
					return textAsset.bytes;
				}
				finally
				{
					Resources.UnloadAsset(textAsset);
				}
			}, true);
		}

		private static TextAsset _getTextAsset(string file)
		{
			return (TextAsset)IApplication.GetCurrentApp().RunOnUiThread(delegate
			{
				string text = "Data/";
				file = Config.ConvertExtension(file).Trim();
				TextAsset textAsset = null;
				try
				{
					textAsset = Resources.Load(text + file) as TextAsset;
					if (textAsset != null)
					{
						return textAsset;
					}
				}
				catch (Exception)
				{
				}
				int num = file.LastIndexOf('.');
				if (num != -1)
				{
					file = file.Substring(0, num);
				}
				try
				{
					textAsset = Resources.Load(text + file) as TextAsset;
					if (textAsset != null)
					{
						return textAsset;
					}
				}
				catch (Exception)
				{
				}
				return (object)null;
			}, true);
		}

		public static byte[] GetData(string file)
		{
			if (country_ == null)
			{
				_init();
			}
			byte[] array = null;
			array = _getData(file);
			if (array == null)
			{
				return null;
			}
			if (Config.DEBUG && file.Equals("save.bytes"))
			{
				return array;
			}
			if (file.Contains('/') || (!file.EndsWith(".dat") && !file.EndsWith(".bytes")))
			{
				return array;
			}
			if (encrypt_)
			{
				Encrypter.Decode(array);
			}
			return array;
		}

		public static bool Exist(string file)
		{
			if (filelist_ == null)
			{
				filelist_ = new Dictionary<string, bool>();
				try
				{
					IApplication.GetCurrentApp().RunOnUiThread(delegate
					{
						TextAsset textAsset = Resources.Load("Data/filelist") as TextAsset;
						if (textAsset != null)
						{
							string[] array = StringUtil.Split(textAsset.text, "\n");
							for (int i = 0; i < array.Length; i++)
							{
								string text = array[i].Trim();
								if (text.Length > 0)
								{
									int num2 = text.LastIndexOf('.');
									if (num2 != -1)
									{
										text = text.Substring(0, num2);
									}
									filelist_[text] = true;
								}
							}
							Resources.UnloadAsset(textAsset);
						}
					}, true);
				}
				catch (Exception)
				{
				}
			}
			file = Config.ConvertExtension(file).Trim();
			if (country_ == null)
			{
				_init();
			}
			if (filelist_.Count == 0)
			{
				return _getData(file) != null;
			}
			if (filelist_.ContainsKey(file))
			{
				return true;
			}
			int num = file.LastIndexOf('.');
			if (num != -1)
			{
				file = file.Substring(0, num);
				if (filelist_.ContainsKey(file))
				{
					return true;
				}
			}
			return false;
		}

		public static void SetSecurity(long[] crcTable, bool encrypt)
		{
			crcTable_ = crcTable;
			encrypt_ = encrypt;
		}
	}
}
