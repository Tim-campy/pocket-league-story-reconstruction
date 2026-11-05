using System;
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
			if (property == null)
			{
				return;
			}
			this.SetData(property.ints);
			this.SetData(property.lngs);
			this.SetData(property.strs);
			this.SetData(property.bins);
			this.SetData(property.pros);
		}

		public void SetData(SecureInt[] data)
		{
			this.ints = null;
			if (data == null)
			{
				return;
			}
			this.ints = new SecureInt[(int)data.Length];
			JSystem.Arraycopy(data, 0, this.ints, 0, (int)this.ints.Length);
		}

		public void SetData(int[] data)
		{
			this.ints = null;
			if (data == null)
			{
				return;
			}
			this.ints = new SecureInt[(int)data.Length];
			for (int i = 0; i < (int)data.Length; i++)
			{
				this.ints[i] = new SecureInt(data[i]);
			}
		}

		public void SetData(SecureLong[] data)
		{
			this.lngs = null;
			if (data == null)
			{
				return;
			}
			this.lngs = new SecureLong[(int)data.Length];
			JSystem.Arraycopy(data, 0, this.lngs, 0, (int)this.lngs.Length);
		}

		public void SetData(long[] data)
		{
			this.lngs = null;
			if (data == null)
			{
				return;
			}
			this.lngs = new SecureLong[(int)data.Length];
			for (int i = 0; i < (int)this.lngs.Length; i++)
			{
				this.lngs[i] = new SecureLong(data[i]);
			}
		}

		public void SetData(SecureString[] data)
		{
			this.strs = null;
			if (data == null)
			{
				return;
			}
			this.strs = new SecureString[(int)data.Length];
			SecureString.Arraycopy(data, 0, this.strs, 0, (int)this.strs.Length);
		}

		public void SetData(string[] data)
		{
			this.strs = null;
			if (data == null)
			{
				return;
			}
			this.strs = new SecureString[(int)data.Length];
			for (int i = 0; i < (int)this.strs.Length; i++)
			{
				this.strs[i] = SecureString.ValueOf(data[i]);
			}
		}

		public void SetData(byte[][] data)
		{
			this.bins = null;
			if (data == null)
			{
				return;
			}
			this.bins = new byte[(int)data.Length][];
			for (int i = 0; i < (int)this.bins.Length; i++)
			{
				this.bins[i] = new byte[(int)data[i].Length];
				JSystem.Arraycopy(data[i], 0, this.bins[i], 0, (int)this.bins[i].Length);
			}
		}

		public void SetData(SecureProperty[] data)
		{
			this.pros = null;
			if (data == null)
			{
				return;
			}
			this.pros = new SecureProperty[(int)data.Length];
			for (int i = 0; i < (int)this.pros.Length; i++)
			{
				this.pros[i] = new SecureProperty(data[i]);
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
			if (type == 0)
			{
				if (this.pros == null)
				{
					this.pros = new SecureProperty[size];
				}
				for (int i = 0; i < size; i++)
				{
					SecureProperty secureProperty = new SecureProperty(this.pros[i]);
					secureProperty.Load(StreamUtil.Read(stream, true));
					if (0 <= i && i < (int)this.pros.Length)
					{
						this.pros[i] = secureProperty;
					}
				}
				return;
			}
			if (type == 1)
			{
				if (this.ints == null)
				{
					this.ints = SecureInt.Array(size);
				}
				for (int j = 0; j < size; j++)
				{
					int num = StreamUtil.ReadInt(stream);
					if (0 <= j && j < (int)this.ints.Length)
					{
						this.ints[j].Set(num);
					}
				}
				return;
			}
			if (type == 2)
			{
				if (this.lngs == null)
				{
					this.lngs = SecureLong.Array(size);
				}
				for (int k = 0; k < size; k++)
				{
					long num1 = StreamUtil.ReadLong(stream);
					if (0 <= k && k < (int)this.lngs.Length)
					{
						this.lngs[k].Set(num1);
					}
				}
				return;
			}
			if (type == 3)
			{
				if (this.strs == null)
				{
					this.strs = new SecureString[size];
				}
				for (int l = 0; l < size; l++)
				{
					byte[] numArray = StreamUtil.Read(stream, true);
					if (0 <= l && l < (int)this.strs.Length)
					{
						this.strs[l] = new SecureString(StringUtil.GetString(numArray));
					}
				}
				return;
			}
			if (type == 4)
			{
				if (this.bins == null)
				{
					this.bins = new byte[size][];
				}
				for (int m = 0; m < size; m++)
				{
					byte[] numArray1 = StreamUtil.Read(stream, true);
					if (0 <= m && m < (int)this.bins.Length)
					{
						this.bins[m] = numArray1;
					}
				}
			}
		}

		protected void Write(OutputStream os, int type)
		{
			if (type == 0)
			{
				if (this.pros == null)
				{
					return;
				}
				for (int i = 0; i < (int)this.pros.Length; i++)
				{
					StreamUtil.Write(os, this.pros[i].ToByteArray(), true);
				}
				return;
			}
			if (type == 1)
			{
				if (this.ints == null)
				{
					return;
				}
				for (int j = 0; j < (int)this.ints.Length; j++)
				{
					StreamUtil.WriteInt(os, this.ints[j].Get());
				}
				return;
			}
			if (type == 2)
			{
				if (this.lngs == null)
				{
					return;
				}
				for (int k = 0; k < (int)this.lngs.Length; k++)
				{
					StreamUtil.WriteLong(os, this.lngs[k].Get());
				}
				return;
			}
			if (type == 3)
			{
				if (this.strs == null)
				{
					return;
				}
				for (int l = 0; l < (int)this.strs.Length; l++)
				{
					StreamUtil.Write(os, this.strs[l].GetBytes(), true);
				}
				return;
			}
			if (type == 4)
			{
				if (this.bins == null)
				{
					return;
				}
				for (int m = 0; m < (int)this.bins.Length; m++)
				{
					StreamUtil.Write(os, this.bins[m], true);
				}
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
			if (type == 0)
			{
				return "pro";
			}
			if (type == 1)
			{
				return "int";
			}
			if (type == 2)
			{
				return "lng";
			}
			if (type == 3)
			{
				return "str";
			}
			if (type == 4)
			{
				return "bin";
			}
			return null;
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
