using java.lang;
using java.util;
using kairo.common.cfg;
using kairo.unity.m3d4.util3d;

namespace game
{
	public class GameUtil
	{
		public const int UP = 0;

		public const int DOWN = 1;

		public const int LEFT = 2;

		public const int RIGHT = 3;

		public static readonly int[][] DRAW_DIRECT = new int[5][]
		{
			new int[2] { 0, -1 },
			new int[2] { 0, 1 },
			new int[2] { -1, 0 },
			new int[2] { 1, 0 },
			new int[2]
		};

		public static readonly int[][] DIRECT = new int[4][]
		{
			new int[2] { 0, 1 },
			new int[2] { 0, -1 },
			new int[2] { -1, 0 },
			new int[2] { 1, 0 }
		};

		public static int MAX_ANGLE = 23;

		public static float MAX_ANGLEDATA_SIN;

		public static float MAX_ANGLEDATA_COS;

		public static int MAX_ANGLE2 = 28;

		public static float MAX_ANGLEDATA_SIN2;

		public static float MAX_ANGLEDATA_COS2;

		public static int MAX_ANGLE3 = 70;

		public static float MAX_ANGLEDATA_SIN3;

		public static float MAX_ANGLEDATA_COS3;

		private static JRandom random = new JRandom();

		public static float[] SIN;

		private static Vector3D temp1 = new Vector3D();

		private static Vector3D temp2 = new Vector3D();

		private static Vector3D temp3 = new Vector3D();

		private static int[] vec1 = new int[2];

		private static int[] vec2 = new int[2];

		public GameUtil()
		{
			SIN = new float[91];
			for (int i = 0; i < SIN.Length; i++)
			{
				SIN[i] = (float)JMath.Sin(JMath.ToRadians(i));
			}
			MAX_ANGLEDATA_SIN = SIN[MAX_ANGLE];
			MAX_ANGLEDATA_COS = Cos(MAX_ANGLE);
			MAX_ANGLEDATA_SIN2 = SIN[MAX_ANGLE2];
			MAX_ANGLEDATA_COS2 = Cos(MAX_ANGLE2);
			MAX_ANGLEDATA_SIN3 = SIN[MAX_ANGLE3];
			MAX_ANGLEDATA_COS3 = Cos(MAX_ANGLE3);
			if (!Config.DEBUG_CMD)
			{
			}
		}

		public static int ConvertGameEva(int param)
		{
			if (param < 256)
			{
				return RateConvert(param, 0, 256, 0, 80, true);
			}
			return RateConvert(param, 256, 1000, 80, 100, true);
		}

		public static int RateConvert(int x, int c1min, int c1max, int c2min, int c2max, bool ch_limit)
		{
			if (ch_limit)
			{
				if ((x < c1min && c1min < c1max) || (x > c1min && c1min > c1max))
				{
					return c2min;
				}
				if ((x > c1max && c1min < c1max) || (x < c1max && c1min > c1max))
				{
					return c2max;
				}
			}
			if (c1max - c1min == 0)
			{
				return c2min;
			}
			{ int recoveredReturn = (x - c1min) * (c2max - c2min) / (c1max - c1min) + c2min; return recoveredReturn; }
		}

		public static long RateConvert(long x, long c1min, long c1max, long c2min, long c2max, bool ch_limit)
		{
			if (ch_limit)
			{
				if ((x < c1min && c1min < c1max) || (x > c1min && c1min > c1max))
				{
					return c2min;
				}
				if ((x > c1max && c1min < c1max) || (x < c1max && c1min > c1max))
				{
					return c2max;
				}
			}
			if (c1max - c1min == 0L)
			{
				return c2min;
			}
			{ long recoveredReturn = (x - c1min) * (c2max - c2min) / (c1max - c1min) + c2min; return recoveredReturn; }
		}

		public static float RateConvert(float x, float c1min, float c1max, float c2min, float c2max, bool ch_limit)
		{
			if (ch_limit)
			{
				if ((x < c1min && c1min < c1max) || (x > c1min && c1min > c1max))
				{
					return c2min;
				}
				if ((x > c1max && c1min < c1max) || (x < c1max && c1min > c1max))
				{
					return c2max;
				}
			}
			if (c1max - c1min == 0f)
			{
				return c2min;
			}
			{ float recoveredReturn = (x - c1min) * (c2max - c2min) / (c1max - c1min) + c2min; return recoveredReturn; }
		}

		public static int RateConvertArea(int x, int c1min, int c1max, int c2min, int c2max, bool ch_limit)
		{
			if (ch_limit)
			{
				if ((x < c1min && c1min < c1max) || (x > c1min && c1min > c1max))
				{
					return c2min;
				}
				if ((x > c1max && c1min < c1max) || (x < c1max && c1min > c1max))
				{
					return c2max;
				}
			}
			if (c1max - c1min == 0)
			{
				return c2min;
			}
			int num = c2max;
			c2max++;
			int num2 = (x - c1min) * (c2max - c2min) / (c1max - c1min) + c2min;
			if (num2 > num)
			{
				num2 = num;
			}
			return num2;
		}

		public static int AddRand(int value, int rand_min, int rand_max, int ret_min, int ret_max)
		{
			value += Random(rand_max - rand_min + 1) + rand_min;
			if (value < ret_min)
			{
				value = ret_min;
			}
			else if (value > ret_max)
			{
				value = ret_max;
			}
			return value;
		}

		public static float RateChA(float a2, float b2, float X)
		{
			if (a2 + b2 == 0f)
			{
				return 0f;
			}
			return X * a2 / (a2 + b2);
		}

		public static float RateChB(float a2, float b2, float X)
		{
			if (a2 + b2 == 0f)
			{
				return 0f;
			}
			return X * b2 / (a2 + b2);
		}

		public int[] Bezier(long x1, long y1, long x2, long y2, long x3, long y3, int t, int[] result)
		{
			if (result == null)
			{
				result = new int[2];
			}
			long num = x1 * (4096 - t) * (4096 - t) + 2 * x2 * t * (4096 - t) + x3 * t * t;
			long num2 = y1 * (4096 - t) * (4096 - t) + 2 * y2 * t * (4096 - t) + y3 * t * t;
			result[0] = (int)(num / 16777216);
			result[1] = (int)(num2 / 16777216);
			return result;
		}

		public static bool ChRectHitRect(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2, bool directUp)
		{
			if (x1 + w1 < x2)
			{
				return false;
			}
			if (x1 > x2 + w2)
			{
				return false;
			}
			if (directUp)
			{
				if (y1 < y2 - h2)
				{
					return false;
				}
				if (y1 - h1 > y2)
				{
					return false;
				}
			}
			else
			{
				if (y1 + h1 < y2)
				{
					return false;
				}
				if (y1 > y2 + h2)
				{
					return false;
				}
			}
			return true;
		}

		public static bool ChRectHitRect(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2, bool directUp)
		{
			if (x1 + w1 < x2)
			{
				return false;
			}
			if (x1 > x2 + w2)
			{
				return false;
			}
			if (directUp)
			{
				if (y1 < y2 - h2)
				{
					return false;
				}
				if (y1 - h1 > y2)
				{
					return false;
				}
			}
			else
			{
				if (y1 + h1 < y2)
				{
					return false;
				}
				if (y1 > y2 + h2)
				{
					return false;
				}
			}
			return true;
		}

		public static bool ChRectHitParallel(int leftTopX, int leftTopY, int w, int h, int p1X, int p1Y, int p2X, int p2Y)
		{
			if (p1X < leftTopX)
			{
				return false;
			}
			if (p2X > leftTopX + w)
			{
				return false;
			}
			if (p2Y > leftTopY)
			{
				return false;
			}
			if (p1Y < leftTopY - h)
			{
				return false;
			}
			return true;
		}

		public int Pow(int x, int y)
		{
			int num = 1;
			if (y > 0)
			{
				num = x;
			}
			for (int i = 1; i < y; i++)
			{
				num *= x;
			}
			return num;
		}

		public static int Random(int n)
		{
			return JMath.Abs(random.NextInt() % n);
		}

		public static int Clamp(int val, int min, int max)
		{
			if (val > max)
			{
				val = max;
			}
			if (val < min)
			{
				val = min;
			}
			return val;
		}

		public static long Clamp(long val, long min, long max)
		{
			if (val > max)
			{
				val = max;
			}
			if (val < min)
			{
				val = min;
			}
			return val;
		}

		public static int ClampMin(int val, int min)
		{
			if (val < min)
			{
				return min;
			}
			return val;
		}

		public static int ClampMax(int val, int max)
		{
			if (val > max)
			{
				return max;
			}
			return val;
		}

		public static long ClampMin(long val, long min)
		{
			if (val < min)
			{
				return min;
			}
			return val;
		}

		public static long ClampMax(long val, long max)
		{
			if (val > max)
			{
				return max;
			}
			return val;
		}

		public static int ConvertKeyToGameKey(int input_key)
		{
			int[] array = new int[5] { 131072, 524288, 65536, 262144, 1048576 };
			int num = -1;
			for (int i = 0; i < array.Length; i++)
			{
				if (input_key == array[i])
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return -1;
			}
			return num;
		}

		public static float SetAMSet(float dis, int t1, int t2)
		{
			if (t1 * (t1 + t2) == 0)
			{
				return 0f;
			}
			return dis / (float)(t1 * (t1 + t2));
		}

		public static float SetVStop(float dis, int t)
		{
			if (t == 1)
			{
				return 0f;
			}
			return 2f * dis / (float)(t - 1);
		}

		public static float SetAStop(float dis, int t)
		{
			if (t == 0 || t == 1)
			{
				return 0f;
			}
			return -2f * dis / (float)(t * (t - 1));
		}

		public static float SetVReturn(float dis, int t)
		{
			return 2f * dis / (float)(t + 2);
		}

		public static float SetAReturn(float dis, int t)
		{
			return -4f * dis / (float)(t * (t + 2));
		}

		public static float SetVVelocity(float dis, int t)
		{
			return dis / (float)t;
		}

		public static float SetAVelocity(float dis, int t)
		{
			return 0f;
		}

		public static float Convert_return(float time, int TIME, float DIS)
		{
			if (time <= 0f)
			{
				return 0f;
			}
			if (time > (float)TIME)
			{
				return 0f;
			}
			float num = (0f - 8f * DIS) / (float)(TIME * TIME);
			float num2 = 2f * DIS / (float)TIME - num * (float)(TIME + 2) / 4f;
			return num2 * time + time * (time + 1f) * num / 2f;
		}

		public static float SetVdecreV(float decre_rate, float DIS, int TIME)
		{
			return 2f * DIS / ((float)(2 * TIME) + (float)(TIME + 1) * (decre_rate - 1f));
		}

		public static float SetAdecreV(float decre_rate, float DIS, int TIME)
		{
			float num = 2f * DIS / ((float)(2 * TIME) + (float)(TIME + 1) * (decre_rate - 1f));
			return (decre_rate - 1f) * num / (float)TIME;
		}

		public static float SetV(float a, float DIS, int TIME)
		{
			return DIS / (float)TIME - a * (float)(TIME + 1) / 2f;
		}

		public static float GetA_fixV0DIS(float v0, float DIS, int TIME)
		{
			{ float recoveredReturn = 2f * (DIS - v0 * (float)TIME) / (float)(TIME * (TIME + 1)); return recoveredReturn; }
		}

		public static float Sin(int angle)
		{
			angle %= 360;
			if (angle < 0)
			{
				angle = 360 + angle;
			}
			if (angle <= 90)
			{
				return SIN[angle];
			}
			if (angle <= 180)
			{
				return SIN[180 - angle];
			}
			if (angle <= 270)
			{
				return 0f - SIN[angle - 180];
			}
			return 0f - SIN[360 - angle];
		}

		public static float Cos(int angle)
		{
			angle %= 360;
			if (angle < 0)
			{
				angle = 360 + angle;
			}
			if (angle <= 90)
			{
				return SIN[90 - angle];
			}
			if (angle <= 180)
			{
				return 0f - SIN[angle - 90];
			}
			if (angle <= 270)
			{
				return 0f - SIN[270 - angle];
			}
			return SIN[angle - 270];
		}

		public static float GetDistanceFromPos(Vector3D a, Vector3D b, Vector3D c)
		{
			Sub(b, a, temp1);
			Sub(c, a, temp2);
			if (Dot(temp1, temp2) < 0f)
			{
				return Abs(c, a);
			}
			Sub(a, b, temp1);
			Sub(c, b, temp2);
			if (Dot(temp1, temp2) < 0f)
			{
				return Abs(c, b);
			}
			Sub(b, a, temp1);
			Sub(c, a, temp2);
			return JMath.Abs(Cross(temp1, temp2)) / Abs(b, a);
		}

		public static float Abs(Vector3D a, Vector3D b)
		{
			return JMath.Abs((a.x_ - b.x_) * (a.x_ - b.x_) + (a.z_ - b.z_) * (a.z_ - b.z_));
		}

		public static float Dot(Vector3D a, Vector3D b)
		{
			return a.x_ * b.x_ + a.z_ * b.z_;
		}

		public static float Cross(Vector3D a, Vector3D b)
		{
			return a.x_ * b.z_ - a.z_ * b.x_;
		}

		public static bool ChCrossLine(Vector3D a1, Vector3D a2, Vector3D b1, Vector3D b2)
		{
			Sub(a2, a1, temp1);
			Sub(b1, a1, temp2);
			float num = Cross(temp1, temp2);
			Sub(a2, a1, temp1);
			Sub(b2, a1, temp2);
			float num2 = Cross(temp1, temp2);
			Sub(b2, b1, temp1);
			Sub(a1, b1, temp2);
			float num3 = Cross(temp1, temp2);
			Sub(b2, b1, temp1);
			Sub(a2, b1, temp2);
			float num4 = Cross(temp1, temp2);
			return num * num2 < 0f && num3 * num4 < 0f;
		}

		public static Vector3D GetCrossPoint(Vector3D a1, Vector3D a2, Vector3D b1, Vector3D b2)
		{
			Sub(a1, b1, temp1);
			Sub(a2, b1, temp2);
			Sub(b2, b1, temp3);
			float num = JMath.Abs(Cross(temp3, temp1));
			float num2 = JMath.Abs(Cross(temp3, temp2));
			float num3 = num / (num + num2);
			Sub(a2, a1, temp1);
			temp3.Set(a1.x_ + temp1.x_ * num3, 0f, a1.z_ + temp1.z_ * num3);
			return temp3;
		}

		public static Vector3D GetCrossPoint(Vector3D a, Vector3D b, Vector3D c, bool limit)
		{
			Sub(b, a, temp1);
			Sub(c, a, temp2);
			float num = (temp1.x_ * temp2.x_ + temp1.z_ * temp2.z_) / (temp1.x_ * temp1.x_ + temp1.z_ * temp1.z_);
			if (limit)
			{
				if (num <= 0f)
				{
					return temp1;
				}
				if (num >= 1f)
				{
					return temp2;
				}
				temp3.x_ = a.x_ + num * temp1.x_;
				temp3.z_ = a.z_ + num * temp1.z_;
				return temp3;
			}
			temp3.x_ = a.x_ + num * temp1.x_;
			temp3.z_ = a.z_ + num * temp1.z_;
			return temp3;
		}

		public static bool ChOnLine(Vector3D p1, Vector3D p2, float h, Vector3D target_pos)
		{
			if (p2.x_ == p1.x_)
			{
				return false;
			}
			float num = (p2.z_ - p1.z_) / (p2.x_ - p1.x_);
			float num2 = p1.z_ - num * p1.x_;
			float num3 = num * target_pos.x_ + num2 + h / 2f;
			float num4 = num * target_pos.x_ + num2 - h / 2f;
			if (target_pos.z_ <= num3 && target_pos.z_ >= num4)
			{
				return true;
			}
			return false;
		}

		public static bool ChOnLine(int[] p1, int[] p2, int h, int[] target_pos)
		{
			if (p2[0] == p1[0])
			{
				int num = ((p1[1] <= p2[1]) ? p2[1] : p1[1]);
				num += h / 2;
				if (target_pos[1] > num)
				{
					return false;
				}
				int num2 = ((p1[1] >= p2[1]) ? p2[1] : p1[1]);
				num2 -= h / 2;
				if (target_pos[1] < num2)
				{
					return false;
				}
				if (p1[0] < target_pos[0] - h / 2 || p1[0] > target_pos[0] + h / 2)
				{
					return false;
				}
				return true;
			}
			int num3 = ((p1[0] >= p2[0]) ? p2[0] : p1[0]);
			if (target_pos[0] < num3 - h / 2)
			{
				return false;
			}
			int num4 = ((p1[0] <= p2[0]) ? p2[0] : p1[0]);
			if (target_pos[0] > num4 + h / 2)
			{
				return false;
			}
			int num5 = ((p1[1] <= p2[1]) ? p2[1] : p1[1]);
			if (target_pos[1] > num5 + h / 2)
			{
				return false;
			}
			int num6 = ((p1[1] >= p2[1]) ? p2[1] : p1[1]);
			if (target_pos[1] < num6 - h / 2)
			{
				return false;
			}
			vec1[0] = target_pos[0] - p1[0];
			vec1[1] = target_pos[1] - p1[1];
			vec2[0] = p2[0] - p1[0];
			vec2[1] = p2[1] - p1[1];
			float num7 = (float)((double)(vec1[0] * vec2[1] - vec1[1] * vec2[0]) / JMath.Sqrt(vec2[0] * vec2[0] + vec2[1] * vec2[1]));
			if (num7 < (float)(h / 2))
			{
				return true;
			}
			return false;
		}

		public static void Sub(Vector3D a, Vector3D b, Vector3D var_out)
		{
			var_out.x_ = a.x_ - b.x_;
			var_out.y_ = a.y_ - b.y_;
			var_out.z_ = a.z_ - b.z_;
		}
	}
}
