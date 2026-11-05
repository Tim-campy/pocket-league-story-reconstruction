using java.io;
using java.lang;

namespace kairo.unity.util
{
	public class SecureProperty
	{
		public const int TYPE_PRO = 0;

		public const int TYPE_INT = 1;

		public const int TYPE_LNG = 2;

		public const int TYPE_STR = 3;

		public const int TYPE_BIN = 4;

		private const int TYPE_END = 5;

		public SecureProperty[] pros;

		public SecureInt[] ints;

		public SecureLong[] lngs;

		public SecureString[] strs;

		public byte[][] bins;

		public SecureProperty()
		{
		}

		public SecureProperty(SecureProperty[] pros, int[] ints, long[] lngs, string[] strs, byte[][] bins)
		{
			SetData(ints);
			SetData(lngs);
			SetData(strs);
			SetData(bins);
			SetData(pros);
		}

		public SecureProperty(SecureProperty[] pros, SecureInt[] ints, SecureLong[] lngs, SecureString[] strs, byte[][] bins)
		{
			SetData(ints);
			SetData(lngs);
			SetData(strs);
			SetData(bins);
			SetData(pros);
		}

		public SecureProperty(SecureProperty property)
		{
			if (property != null)
			{
				SetData(property.ints);
				SetData(property.lngs);
				SetData(property.strs);
				SetData(property.bins);
				SetData(property.pros);
			}
		}

		public void SetData(SecureInt[] data)
		{
			ints = null;
			if (data != null)
			{
				ints = new SecureInt[data.Length];
				JSystem.Arraycopy(data, 0, ints, 0, ints.Length);
			}
		}

		public void SetData(int[] data)
		{
			ints = null;
			if (data != null)
			{
				ints = new SecureInt[data.Length];
				for (int i = 0; i < data.Length; i++)
				{
					ints[i] = new SecureInt(data[i]);
				}
			}
		}

		public void SetData(SecureLong[] data)
		{
			lngs = null;
			if (data != null)
			{
				lngs = new SecureLong[data.Length];
				JSystem.Arraycopy(data, 0, lngs, 0, lngs.Length);
			}
		}

		public void SetData(long[] data)
		{
			lngs = null;
			if (data != null)
			{
				lngs = new SecureLong[data.Length];
				for (int i = 0; i < lngs.Length; i++)
				{
					lngs[i] = new SecureLong(data[i]);
				}
			}
		}

		public void SetData(SecureString[] data)
		{
			strs = null;
			if (data != null)
			{
				strs = new SecureString[data.Length];
				SecureString.Arraycopy(data, 0, strs, 0, strs.Length);
			}
		}

		public void SetData(string[] data)
		{
			strs = null;
			if (data != null)
			{
				strs = new SecureString[data.Length];
				for (int i = 0; i < strs.Length; i++)
				{
					strs[i] = SecureString.ValueOf(data[i]);
				}
			}
		}

		public void SetData(byte[][] data)
		{
			bins = null;
			if (data != null)
			{
				bins = new byte[data.Length][];
				for (int i = 0; i < bins.Length; i++)
				{
					bins[i] = new byte[data[i].Length];
					JSystem.Arraycopy(data[i], 0, bins[i], 0, bins[i].Length);
				}
			}
		}

		public void SetData(SecureProperty[] data)
		{
			pros = null;
			if (data != null)
			{
				pros = new SecureProperty[data.Length];
				for (int i = 0; i < pros.Length; i++)
				{
					pros[i] = new SecureProperty(data[i]);
				}
			}
		}

		public void Load(byte[] data)
		{
			ByteArrayInputStream byteArrayInputStream = null;
			try
			{
				byteArrayInputStream = new ByteArrayInputStream(data);
				Load(byteArrayInputStream);
			}
			finally
			{
				byteArrayInputStream?.Close();
				byteArrayInputStream = null;
			}
		}

		public void Load(InputStream stream)
		{
			int num = StreamUtil.ReadInt(stream);
			for (int i = 0; i < num; i++)
			{
				byte[] ids = null;
				int type = StreamUtil.ReadInt(stream);
				int size = StreamUtil.ReadInt(stream);
				Read(stream, type, size, ids);
			}
		}

		public byte[] ToByteArray()
		{
			ByteArrayOutputStream byteArrayOutputStream = null;
			try
			{
				byteArrayOutputStream = new ByteArrayOutputStream();
				ToByteArray(byteArrayOutputStream);
				return byteArrayOutputStream.ToByteArray();
			}
			finally
			{
				byteArrayOutputStream?.Close();
				byteArrayOutputStream = null;
			}
		}

		public void ToByteArray(OutputStream os)
		{
			int typeNum = GetTypeNum();
			StreamUtil.WriteInt(os, typeNum);
			for (int i = 0; i < typeNum; i++)
			{
				StreamUtil.WriteInt(os, i);
				StreamUtil.WriteInt(os, Size(i));
				Write(os, i);
			}
		}

		protected void Read(InputStream stream, int type, int size, byte[] ids)
		{
			switch (type)
			{
			case 0:
			{
				if (pros == null)
				{
					pros = new SecureProperty[size];
				}
				for (int l = 0; l < size; l++)
				{
					SecureProperty secureProperty = new SecureProperty(pros[l]);
					secureProperty.Load(StreamUtil.Read(stream, true));
					if (0 <= l && l < pros.Length)
					{
						pros[l] = secureProperty;
					}
				}
				break;
			}
			case 1:
			{
				if (ints == null)
				{
					ints = SecureInt.Array(size);
				}
				for (int k = 0; k < size; k++)
				{
					int value2 = StreamUtil.ReadInt(stream);
					if (0 <= k && k < ints.Length)
					{
						ints[k].Set(value2);
					}
				}
				break;
			}
			case 2:
			{
				if (lngs == null)
				{
					lngs = SecureLong.Array(size);
				}
				for (int j = 0; j < size; j++)
				{
					long value = StreamUtil.ReadLong(stream);
					if (0 <= j && j < lngs.Length)
					{
						lngs[j].Set(value);
					}
				}
				break;
			}
			case 3:
			{
				if (strs == null)
				{
					strs = new SecureString[size];
				}
				for (int m = 0; m < size; m++)
				{
					byte[] bytes = StreamUtil.Read(stream, true);
					if (0 <= m && m < strs.Length)
					{
						strs[m] = new SecureString(StringUtil.GetString(bytes));
					}
				}
				break;
			}
			case 4:
			{
				if (bins == null)
				{
					bins = new byte[size][];
				}
				for (int i = 0; i < size; i++)
				{
					byte[] array = StreamUtil.Read(stream, true);
					if (0 <= i && i < bins.Length)
					{
						bins[i] = array;
					}
				}
				break;
			}
			}
		}

		protected void Write(OutputStream os, int type)
		{
			switch (type)
			{
			case 0:
				if (pros != null)
				{
					for (int j = 0; j < pros.Length; j++)
					{
						StreamUtil.Write(os, pros[j].ToByteArray(), true);
					}
				}
				break;
			case 1:
				if (ints != null)
				{
					for (int m = 0; m < ints.Length; m++)
					{
						StreamUtil.WriteInt(os, ints[m].Get());
					}
				}
				break;
			case 2:
				if (lngs != null)
				{
					for (int k = 0; k < lngs.Length; k++)
					{
						StreamUtil.WriteLong(os, lngs[k].Get());
					}
				}
				break;
			case 3:
				if (strs != null)
				{
					for (int l = 0; l < strs.Length; l++)
					{
						StreamUtil.Write(os, strs[l].GetBytes(), true);
					}
				}
				break;
			case 4:
				if (bins != null)
				{
					for (int i = 0; i < bins.Length; i++)
					{
						StreamUtil.Write(os, bins[i], true);
					}
				}
				break;
			}
		}

		public int Size(int type)
		{
			if (type == 0 && pros != null)
			{
				return pros.Length;
			}
			if (type == 1 && ints != null)
			{
				return ints.Length;
			}
			if (type == 2 && lngs != null)
			{
				return lngs.Length;
			}
			if (type == 3 && strs != null)
			{
				return strs.Length;
			}
			if (type == 4 && bins != null)
			{
				return bins.Length;
			}
			return 0;
		}

		public int GetTypeNum()
		{
			return 5;
		}

		public string GetTypeTag(int type)
		{
			switch (type)
			{
			case 0:
				return "pro";
			case 1:
				return "int";
			case 2:
				return "lng";
			case 3:
				return "str";
			case 4:
				return "bin";
			default:
				return null;
			}
		}

		public void DisposeBin()
		{
			if (bins != null)
			{
				for (int i = 0; i < bins.Length; i++)
				{
					bins[i] = new byte[0];
				}
			}
		}
	}
}
