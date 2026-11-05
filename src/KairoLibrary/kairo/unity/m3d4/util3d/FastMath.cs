using System;
using java.lang;

namespace kairo.unity.m3d4.util3d
{
	public class FastMath
	{
		private FastMath()
		{
		}

		public static float Abs(float a)
		{
			if (!(a < 0f))
			{
				return a;
			}
			return 0f - a;
		}

		public static float Acos(float a)
		{
			return (float)JMath.ToDegrees(Math.Acos(a));
		}

		public static float Add(float x, float y)
		{
			return x + y;
		}

		public static float Asin(float a)
		{
			if (a < -1f || 1f < a)
			{
				throw new ArithmeticException("asin OutOfRange:" + a + " / [-1, 1]");
			}
			if (JFloat.IsNaN(a))
			{
				throw new ArgumentException();
			}
			return (float)JMath.ToDegrees(Math.Asin(a));
		}

		public static float Atan(float a)
		{
			return (float)JMath.ToDegrees(Math.Atan(a));
		}

		public static float Atan2(float a, float b)
		{
			float num = (float)JMath.ToDegrees(Math.Atan2(b, a));
			if (b < 0f)
			{
				num += 180f;
			}
			return num;
		}

		public static float Cos(float a)
		{
			return (float)Math.Cos(JMath.ToRadians(a));
		}

		public static float Div(float x, float y)
		{
			return x / y;
		}

		public static int FloatToInnerInt(float v)
		{
			if (v * 4096f > 2.1474836E+09f || v * 4096f < -2.1474836E+09f)
			{
				throw new ArgumentException();
			}
			if (v == float.NaN || v == float.PositiveInfinity || v == float.NegativeInfinity)
			{
				throw new ArgumentException();
			}
			return (int)(v * 4096f);
		}

		public static float InnerIntToFloat(int v)
		{
			return (float)((float)v / 4096f);
		}

		public static float Mul(float x, float y)
		{
			return x * y;
		}

		public static float Sin(float a)
		{
			return (float)Math.Sin(JMath.ToRadians(a));
		}

		public static float Sqrt(float x)
		{
			return (float)Math.Sqrt(x);
		}

		public static float Sub(float x, float y)
		{
			return x - y;
		}

		public static float Tan(float a)
		{
			if (JFloat.IsInfinite(a) || JFloat.IsNaN(a))
			{
				return 0f;
			}
			return (float)Math.Tan(JMath.ToRadians(a));
		}
	}
}
