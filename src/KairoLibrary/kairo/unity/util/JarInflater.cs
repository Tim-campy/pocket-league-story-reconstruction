using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using java.io;
using java.lang;
using kairo.common.cfg;
using kairo.unity.io;

namespace kairo.unity.util
{
	public class JarInflater
	{
		public class FileInfo
		{
			public string Name;

			public int Offset;

			public int Size;

			public byte Flag;
		}

		private Dictionary<string, FileInfo> files_;

		private byte[] data_;

		private string directory_;

		private const sbyte FLAG_DEFLATE = 1;

		public JarInflater(byte[] data)
		{
			if (data == null)
			{
				throw new NullReferenceException();
			}
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(data);
				_load(byteArrayInputStream);
			}
			finally
			{
				if (byteArrayInputStream != null)
				{
					try
					{
						byteArrayInputStream.Close();
					}
					catch (Exception)
					{
					}
				}
				byteArrayInputStream = null;
			}
		}

		public JarInflater(InputStream stream)
		{
			if (stream == null)
			{
				throw new NullReferenceException();
			}
			_load(stream);
		}

		public JarInflater(string directory)
		{
			directory_ = directory;
			if (!directory_.EndsWith("/"))
			{
				directory_ += "/";
			}
			string text = directory_.Substring(0, directory_.Length() - 1) + ".dat";
			string[] array = new string[2]
			{
				text,
				directory_ + text
			};
			for (int i = 0; i < array.Length; i++)
			{
				InputStream resourceAsStream = GetResourceAsStream(array[i]);
				if (resourceAsStream != null)
				{
					try
					{
						_load(resourceAsStream);
						break;
					}
					finally
					{
						resourceAsStream?.Close();
						resourceAsStream = null;
					}
				}
			}
		}

		protected void _load(InputStream stream)
		{
			StreamUtil.ReadInt(stream);
			int size = StreamUtil.ReadInt(stream);
			int num = StreamUtil.ReadInt(stream);
			string[] array = new string[num];
			int[] array2 = new int[num];
			int[] array3 = new int[num];
			byte[] array4 = new byte[num];
			for (int i = 0; i < num; i++)
			{
				byte[] bytes = StreamUtil.Read(stream, StreamUtil.ReadInt(stream));
				array[i] = JString.New(bytes);
			}
			for (int j = 0; j < num; j++)
			{
				array2[j] = StreamUtil.ReadInt(stream);
			}
			for (int k = 0; k < num; k++)
			{
				array3[k] = StreamUtil.ReadInt(stream);
			}
			array4 = StreamUtil.Read(stream, array4.Length);
			files_ = new Dictionary<string, FileInfo>();
			for (int l = 0; l < array.Length; l++)
			{
				FileInfo fileInfo = new FileInfo();
				fileInfo.Name = array[l];
				fileInfo.Offset = array2[l];
				fileInfo.Size = array3[l];
				fileInfo.Flag = array4[l];
				string key = array[l].ToLower();
				files_[key] = fileInfo;
			}
			data_ = StreamUtil.Read(stream, size);
		}

		public void Close()
		{
			data_ = null;
			directory_ = null;
			if (files_ != null)
			{
				files_.Clear();
			}
			files_ = null;
		}

		public InputStream GetInputStream(string name)
		{
			name = ConvertExtension(name);
			if (directory_ != null)
			{
				InputStream resourceAsStream = GetResourceAsStream(directory_ + name);
				if (resourceAsStream != null)
				{
					return resourceAsStream;
				}
			}
			FileInfo fileInfo = _search(name);
			if (fileInfo == null)
			{
				return null;
			}
			byte[] array = null;
			DataInputStream dataInputStream = null;
			try
			{
				int num = ((0xFF & data_[fileInfo.Offset + 0]) << 24) | ((0xFF & data_[fileInfo.Offset + 1]) << 16) | ((0xFF & data_[fileInfo.Offset + 2]) << 8) | (0xFF & data_[fileInfo.Offset + 3]);
				byte[] array2 = new byte[num];
				JSystem.Arraycopy(data_, fileInfo.Offset + 4, array2, 0, num);
				if ((fileInfo.Flag & 1) != 0 && fileInfo.Size > 0)
				{
					throw new Exception("JarInflater.getInputStream() 圧縮には対応していません");
				}
				array = array2;
				array = Language.ConvertLanguagePack(name, array);
				return new ByteArrayInputStream(array);
			}
			finally
			{
				if (dataInputStream != null)
				{
					try
					{
						dataInputStream.Close();
					}
					catch (Exception)
					{
					}
				}
				dataInputStream = null;
			}
		}

		public long GetSize(string name)
		{
			name = ConvertExtension(name);
			if (directory_ != null)
			{
				byte[] array = Storage.Read(3, false, directory_ + name);
				if (array != null)
				{
					return array.Length;
				}
			}
			FileInfo fileInfo = _search(name);
			if (fileInfo == null)
			{
				return -1L;
			}
			return fileInfo.Size;
		}

		public bool Exist(string name)
		{
			name = ConvertExtension(name);
			if (directory_ != null)
			{
				return CheckResourceAsStream(directory_ + name);
			}
			return _search(name) != null;
		}

		public byte[] GetData(string name)
		{
			name = ConvertExtension(name);
			if (directory_ != null)
			{
				byte[] array = Storage.Read(3, false, directory_ + name);
				if (array != null)
				{
					return array;
				}
			}
			if ((int)GetSize(name) == -1)
			{
				if (Config.PRINT)
				{
					Log.Info("JarInflater.getData: [" + name + "]が見つかりません");
				}
				return null;
			}
			InputStream inputStream = GetInputStream(name);
			if (inputStream == null)
			{
				return null;
			}
			ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
			byte[] array2 = null;
			byte[] array3 = new byte[1024];
			try
			{
				int len;
				while ((len = inputStream.Read(array3)) >= 0)
				{
					byteArrayOutputStream.Write(array3, 0, len);
				}
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				if (byteArrayOutputStream != null)
				{
					try
					{
						byteArrayOutputStream.Close();
					}
					catch (Exception)
					{
					}
				}
				byteArrayOutputStream = null;
				if (inputStream != null)
				{
					try
					{
						inputStream.Close();
					}
					catch (Exception)
					{
					}
				}
				inputStream = null;
			}
		}

		public string GetDirectory()
		{
			return directory_;
		}

		private FileInfo _search(string name)
		{
			if (name == null || files_ == null)
			{
				return null;
			}
			name = ConvertExtension(name.Trim()).ToLower();
			string[] accessFilesList = AssetReader.GetAccessFilesList(name);
			for (int i = 0; i < accessFilesList.Length; i++)
			{
				FileInfo value;
				if (files_.TryGetValue(accessFilesList[i].ToLower(), out value))
				{
					return value;
				}
			}
			return null;
		}

		protected InputStream GetResourceAsStream(string file)
		{
			file = ConvertExtension(file);
			try
			{
				if (AssetReader.Exist(file))
				{
					return AssetReader.GetInputStream(file);
				}
				file += ".bytes";
				if (AssetReader.Exist(file))
				{
					return AssetReader.GetInputStream(file);
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		protected bool CheckResourceAsStream(string file)
		{
			file = ConvertExtension(file);
			try
			{
				return AssetReader.Exist(file) || AssetReader.Exist(file + ".bytes");
			}
			catch (Exception)
			{
			}
			return false;
		}

		public string ConvertExtension(string file)
		{
			if (file == null)
			{
				return file;
			}
			file = Config.ConvertExtension(file);
			if (directory_ != null)
			{
				string text = Path.GetExtension(file).ToLower();
				string text2 = text;
				if (text == ".inf" || text == ".ini")
				{
					text = ".txt";
				}
				if (text != text2)
				{
					file = Path.ChangeExtension(file, text);
				}
			}
			return file;
		}

		public string[] GetFiles()
		{
			if (directory_ != null)
			{
				return null;
			}
			return files_.Keys.ToArray();
		}
	}
}
