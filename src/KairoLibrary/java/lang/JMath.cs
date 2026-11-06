using System;

namespace java.lang
{
	public static class JMath
	{
		public const double E = Math.E;

		public const double PI = Math.PI;

		public static double Abs(double a)
		{
			if (!(a >= 0.0))
			{
				return -(a);
			}
			return a;
		}

		public static float Abs(float a)
		{
			if (!(a >= 0f))
			{
				return -(a);
			}
			return a;
		}

		public static int Abs(int a)
		{
			if (a < 0)
			{
				return -a;
			}
			return a;
		}

		public static long Abs(long a)
		{
			if (a < 0)
			{
				return -a;
			}
			return a;
		}

		public static double Max(double a, double b)
		{
			if (!(a >= b))
			{
				return b;
			}
			return a;
		}

		public static float Max(float a, float b)
		{
			if (!(a >= b))
			{
				return b;
			}
			return a;
		}

		public static int Max(int a, int b)
		{
			if (a < b)
			{
				return b;
			}
			return a;
		}

		public static long Max(long a, long b)
		{
			if (a < b)
			{
				return b;
			}
			return a;
		}

		public static double Min(double a, double b)
		{
			if (!(a <= b))
			{
				return b;
			}
			return a;
		}

		public static float Min(float a, float b)
		{
			if (!(a <= b))
			{
				return b;
			}
			return a;
		}

		public static int Min(int a, int b)
		{
			if (a > b)
			{
				return b;
			}
			return a;
		}

		public static long Min(long a, long b)
		{
			if (a > b)
			{
				return b;
			}
			return a;
		}

		public static double Ceil(double a)
		{
			return Math.Ceiling(a);
		}

		public static double Floor(double a)
		{
			return Math.Floor(a);
		}

		public static double Cos(double a)
		{
			return Math.Cos(a);
		}

		public static double Sin(double a)
		{
			return Math.Sin(a);
		}

		public static double Tan(double a)
		{
			return Math.Tan(a);
		}

		public static double Sqrt(double a)
		{
			return Math.Sqrt(a);
		}

		public static double ToDegrees(double angrad)
		{
			return angrad * 180.0 / Math.PI;
		}

		public static double ToRadians(double angdeg)
		{
			return angdeg * Math.PI / 180.0;
		}
	}
}
