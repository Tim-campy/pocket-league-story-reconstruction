namespace kairo.unity.ui
{
	public class DepthInfo
	{
		private Seb seb_;

		public int depth;

		public int scale = 1;

		public int start;

		public int length;

		private long bound;

		public const int DLINE_X1 = 0;

		public const int DLINE_Y1 = 1;

		public const int DLINE_X2 = 2;

		public const int DLINE_Y2 = 3;

		public const int DLINE_SIZE = 4;

		public int[] depthLines;

		internal DepthInfo(Seb seb)
		{
			seb_ = seb;
		}

		internal void CreateDepthLines(int[] sp, int scale)
		{
			int num = 0;
			if (((sp[2] != 0) || (sp[3] != 0)) && ((sp[4] != 0) || (sp[5] != 0))) { num = 8; } else { num = 4; }
			if (depthLines == null || depthLines.Length != num)
			{
				depthLines = new int[num];
			}
			int num2 = 0;
			if (sp[2] != 0 || sp[3] != 0)
			{
				depthLines[num2 + 0] = sp[6];
				depthLines[num2 + 1] = sp[7];
				depthLines[num2 + 2] = sp[6] + sp[2];
				depthLines[num2 + 3] = sp[7] + sp[3];
				num2 += 4;
			}
			if (sp[4] != 0 || sp[5] != 0)
			{
				depthLines[num2 + 0] = sp[6];
				depthLines[num2 + 1] = sp[7];
				depthLines[num2 + 2] = sp[6] + sp[4];
				depthLines[num2 + 3] = sp[7] + sp[5];
				num2 += 4;
			}
			for (int i = 1; i < depthLines.Length; i += 2)
			{
				depthLines[i] *= scale;
			}
		}

		public int[] GetBoundingRect(int x = 0, int y = 0)
		{
			int[] array = new int[4];
			ulong num = (ulong)(bound & -281474976710656L) >> 48;
			array[0] = (short)num;
			array[1] = (short)((bound & 0xFFFF00000000L) >> 32);
			array[2] = (short)((bound & 0xFFFF0000u) >> 16);
			array[3] = (short)(bound & 0xFFFF);
			if (seb_.IsFlipHorizontal())
			{
				array[0] = -array[2] / 2 + (-array[2] / 2 - array[0]);
			}
			if (seb_.IsFlipVertical())
			{
				array[1] = -array[3] / 2 + (-array[3] / 2 - array[1]);
			}
			array[0] += x;
			array[1] += y;
			return array;
		}

		public void SetBoundingRect(int x, int y, int width, int height)
		{
			bound = (long)((((ulong)x & 0xFFFFuL) << 48) | (((ulong)y & 0xFFFFuL) << 32) | (((ulong)width & 0xFFFFuL) << 16) | ((ulong)height & 0xFFFFuL));
		}

		public int GetLast()
		{
			return start + length - 1;
		}
	}
}
