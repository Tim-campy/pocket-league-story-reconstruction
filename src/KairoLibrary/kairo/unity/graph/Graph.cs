using System;
using kairo.unity.m3d4.util3d;

namespace kairo.unity.graph
{
	public class Graph
	{
		private Graph()
		{
		}

		public static int[] Bezier(int x1, int y1, int x2, int y2, int x3, int y3, int t, int[] result = null)
		{
			if (result == null)
			{
				result = new int[2];
			}
			long num = (long)x1 * (long)(4096 - t) * (4096 - t) + 2L * (long)x2 * t * (4096 - t) + (long)x3 * (long)t * t;
			long num2 = (long)y1 * (long)(4096 - t) * (4096 - t) + 2L * (long)y2 * t * (4096 - t) + (long)y3 * (long)t * t;
			result[0] = (int)(num / 16777216);
			result[1] = (int)(num2 / 16777216);
			return result;
		}

		public static float[] BezierF(float x1, float y1, float x2, float y2, float x3, float y3, int t, float[] result = null)
		{
			if (result == null)
			{
				result = new float[2];
			}
			float num = x1 * (float)(4096 - t) * (float)(4096 - t) + 2f * x2 * (float)t * (float)(4096 - t) + x3 * (float)t * (float)t;
			float num2 = y1 * (float)(4096 - t) * (float)(4096 - t) + 2f * y2 * (float)t * (float)(4096 - t) + y3 * (float)t * (float)t;
			result[0] = num / 16777216f;
			result[1] = num2 / 16777216f;
			return result;
		}

		public static int Lerp(int v1, int v2, int length, int frame)
		{
			if (frame >= length)
			{
				frame = length - 1;
			}
			long num = (long)v1 * 100000000L;
			long num2 = ((long)v2 * 100000000L - num) / ((length + 1) * length / 2);
			long num3 = num2 * length;
			long num4 = num;
			for (int i = 0; i < frame; i++)
			{
				num4 += num3;
				num3 -= num2;
			}
			return (int)(num4 / 100000000);
		}

		public static int Easing(int v0, int v1, int length, int frame, int edging)
		{
			if (frame < 0)
			{
				return v0;
			}
			if (frame >= length)
			{
				return v1;
			}
			float num = (float)frame / (float)length;
			float num2 = FastMath.Sin((float)Math.PI * num * 180f / (float)Math.PI);
			int num3 = (int)((float)v0 + (float)(v1 - v0) * (num + (float)edging / ((float)Math.PI * 100f) * num2));
			if (v0 < v1)
			{
				if (num3 < v0)
				{
					num3 = v0;
				}
				if (num3 > v1)
				{
					num3 = v1;
				}
			}
			else
			{
				if (num3 > v0)
				{
					num3 = v0;
				}
				if (num3 < v1)
				{
					num3 = v1;
				}
			}
			return num3;
		}

		public static long Easing(long v0, long v1, long length, long frame, int edging)
		{
			if (frame < 0)
			{
				return v0;
			}
			if (frame >= length)
			{
				return v1;
			}
			float num = (float)frame / (float)length;
			float num2 = FastMath.Sin((float)Math.PI * num * 180f / (float)Math.PI);
			long num3 = (long)((float)v0 + (float)(v1 - v0) * (num + (float)edging / ((float)Math.PI * 100f) * num2));
			if (v0 < v1)
			{
				if (num3 < v0)
				{
					num3 = v0;
				}
				if (num3 > v1)
				{
					num3 = v1;
				}
			}
			else
			{
				if (num3 > v0)
				{
					num3 = v0;
				}
				if (num3 < v1)
				{
					num3 = v1;
				}
			}
			return num3;
		}

		public static float Easing(float v0, float v1, float length, float frame, float edging)
		{
			if (frame < 0f)
			{
				return v0;
			}
			if (frame >= length)
			{
				return v1;
			}
			float num = frame / length;
			float num2 = FastMath.Sin((float)Math.PI * num * 180f / (float)Math.PI);
			float num3 = v0 + (v1 - v0) * (num + edging / ((float)Math.PI * 100f) * num2);
			if (v0 < v1)
			{
				if (num3 < v0)
				{
					num3 = v0;
				}
				if (num3 > v1)
				{
					num3 = v1;
				}
			}
			else
			{
				if (num3 > v0)
				{
					num3 = v0;
				}
				if (num3 < v1)
				{
					num3 = v1;
				}
			}
			return num3;
		}
	}
}
