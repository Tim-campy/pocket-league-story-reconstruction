using System;
using java.lang;
using kairo.unity.m3d4.util3d;

namespace kairo.unity.math
{
	public class KairoMath
	{
		public const float PI = (float)Math.PI;

		private static int[] checkHitLine_tmp = new int[2];

		private static float[] checkHitLineF_tmp = new float[2];

		private static float[] transPoint_tmp1 = new float[3];

		private static Vector3D transPoint_tmp2 = new Vector3D(0f, 0f, 0f);

		private static Vector3D transPoint_tmp3 = new Vector3D();

		private static Vector3D[] getNormalVector_tmp = new Vector3D[2]
		{
			new Vector3D(),
			new Vector3D()
		};

		private static float[] getNormalVector2D_tmp = new float[2];

		private KairoMath()
		{
		}

		public static bool CheckHitPointRect(int x, int y, int rcx, int rcy, int rcw, int rch)
		{
			if (rcx < x && x < rcx + rcw && rcy < y)
			{
				return y < rcy + rch;
			}
			return false;
		}

		public static bool CheckHitPointRect(float x, float y, float rcx, float rcy, float rcw, float rch)
		{
			if (rcx < x && x < rcx + rcw && rcy < y)
			{
				return y < rcy + rch;
			}
			return false;
		}

		public static bool CheckHitPointCircle(Vector3D pos1, Vector3D pos2, float r)
		{
			float num = pos2.GetX() - pos1.GetX();
			float num2 = pos2.GetZ() - pos1.GetZ();
			if (num * num + num2 * num2 <= r * r)
			{
				return true;
			}
			return false;
		}

		public static bool CheckHitCircle(Vector3D pos1, float rad1, Vector3D pos2, float rad2)
		{
			float num = pos2.GetX() - pos1.GetX();
			float num2 = pos2.GetZ() - pos1.GetZ();
			float num3 = rad1 + rad2;
			if (num * num + num2 * num2 <= num3 * num3)
			{
				return true;
			}
			return false;
		}

		public static bool CheckHitPointBox(Vector3D point, Vector3D min, Vector3D max)
		{
			if (point.GetX() < min.GetX() || max.GetX() < point.GetX() || point.GetY() < min.GetY() || max.GetY() < point.GetY() || point.GetZ() < min.GetZ() || max.GetZ() < point.GetZ())
			{
				return false;
			}
			return true;
		}

		public static bool CheckHitPointTriangle(Vector3D p, Vector3D v1, Vector3D v2, Vector3D v3)
		{
			Vector3D vector3D = new Vector3D(v1.GetX() - p.GetX(), 0f, v1.GetZ() - p.GetZ());
			Vector3D vector3D2 = new Vector3D(v2.GetX() - p.GetX(), 0f, v2.GetZ() - p.GetZ());
			Vector3D vector3D3 = new Vector3D(v3.GetX() - p.GetX(), 0f, v3.GetZ() - p.GetZ());
			float num = vector3D.GetX() * vector3D2.GetZ() - vector3D2.GetX() * vector3D.GetZ();
			float num2 = vector3D2.GetX() * vector3D3.GetZ() - vector3D3.GetX() * vector3D2.GetZ();
			float num3 = vector3D3.GetX() * vector3D.GetZ() - vector3D.GetX() * vector3D3.GetZ();
			if (!(num >= 0f) || !(num2 >= 0f) || !(num3 >= 0f))
			{
				if (num <= 0f && num2 <= 0f)
				{
					return num3 <= 0f;
				}
				return false;
			}
			return true;
		}

		public static bool CheckHitRect(int x1, int y1, int w1, int h1, int x2, int y2, int w2, int h2)
		{
			if (x1 <= x2 + w2 && y1 <= y2 + h2 && x1 + w1 >= x2)
			{
				return y1 + h1 >= y2;
			}
			return false;
		}

		public static bool CheckHitRect(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
		{
			if (x1 <= x2 + w2 && y1 <= y2 + h2 && x1 + w1 >= x2)
			{
				return y1 + h1 >= y2;
			}
			return false;
		}

		public static bool CheckHitRect(int[] rect1, int[] rect2)
		{
			if (rect1[0] <= rect2[0] + rect2[2] && rect1[1] <= rect2[1] + rect2[3] && rect1[0] + rect1[2] >= rect2[0])
			{
				return rect1[1] + rect1[3] >= rect2[1];
			}
			return false;
		}

		public static bool CheckHitLineCircle(Vector3D pos1, Vector3D vec1, Vector3D pos2, float r, Vector3D cross)
		{
			if (vec1.GetX() == 0f && vec1.GetZ() == 0f)
			{
				return false;
			}
			float num;
float num2;
float num3;
float num4;
float num5;
Vector3D vector3D = new Vector3D(pos1.GetX() - pos2.GetX(), 0f, pos1.GetZ() - pos2.GetZ());
			num = vec1.GetX() * vec1.GetX() + vec1.GetZ() * vec1.GetZ();
			num2 = 2f * (vector3D.GetX() * vec1.GetX() + vector3D.GetZ() * vec1.GetZ());
			num3 = vector3D.GetX() * vector3D.GetX() + vector3D.GetZ() * vector3D.GetZ() - r * r;
			num4 = num2 * num2 - 4f * num * num3;
			if (num4 < 0f || FastMath.FloatToInnerInt(num4) < 0)
			{
				return false;
			}
			num5 = (-(num2) - FastMath.Sqrt(num4)) / (2f * num);
			if (num5 < 0f || 1f < num5)
			{
				return false;
			}
			if (((object)(cross) != null)) { (cross).Set(pos1.GetX() + vec1.GetX() * num5, 0f, pos1.GetZ() + vec1.GetZ() * num5); }
			return true;
		}

		public static bool CheckHitLine(int ax, int ay, int bx, int by, int cx, int cy, int dx, int dy, int[] cross)
		{
			float num = (bx - ax) * (dy - cy) - (by - ay) * (dx - cx);
			if (0f == num)
			{
				return false;
			}
			int num2 = cx - ax;
			int num3 = cy - ay;
			float num4 = (float)((dy - cy) * num2 - (dx - cx) * num3) / num;
			float num5 = (float)((by - ay) * num2 - (bx - ax) * num3) / num;
			if (num4 < 0f || 1f < num4 || num5 < 0f || 1f < num5)
			{
				return false;
			}
			float num6 = (float)ax + num4 * (float)(bx - ax);
			float num7 = (float)ay + num4 * (float)(by - ay);
			if (ax == bx)
			{
				num6 = ax;
			}
			if (cx == dx)
			{
				num6 = cx;
			}
			if (ay == by)
			{
				num7 = ay;
			}
			if (cy == dy)
			{
				num7 = cy;
			}
			checkHitLine_tmp[0] = (int)num6;
			checkHitLine_tmp[1] = (int)num7;
			if (cross != null)
			{
				JSystem.Arraycopy(checkHitLine_tmp, 0, cross, 0, cross.Length);
			}
			return true;
		}

		public static bool CheckHitLine(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy, float[] cross)
		{
			float num = (bx - ax) * (dy - cy) - (by - ay) * (dx - cx);
			if (0f == num)
			{
				return false;
			}
			float num2 = cx - ax;
			float num3 = cy - ay;
			float num4 = ((dy - cy) * num2 - (dx - cx) * num3) / num;
			float num5 = ((by - ay) * num2 - (bx - ax) * num3) / num;
			if (num4 < 0f || 1f < num4 || num5 < 0f || 1f < num5)
			{
				return false;
			}
			float num6 = ax + num4 * (bx - ax);
			float num7 = ay + num4 * (by - ay);
			if (ax == bx)
			{
				num6 = ax;
			}
			if (cx == dx)
			{
				num6 = cx;
			}
			if (ay == by)
			{
				num7 = ay;
			}
			if (cy == dy)
			{
				num7 = cy;
			}
			checkHitLineF_tmp[0] = num6;
			checkHitLineF_tmp[1] = num7;
			if (cross != null)
			{
				JSystem.Arraycopy(checkHitLineF_tmp, 0, cross, 0, cross.Length);
			}
			return true;
		}

		public static bool CheckHitLineRect(Vector3D p1, Vector3D p2, float rcx, float rcy, float rcw, float rch)
		{
			float x = rcx + rcw;
			float z = rcy + rch;
			if (CheckHitPointRect(p1.GetX(), p1.GetZ(), rcx, rcy, rcw, rch) || CheckHitPointRect(p2.GetX(), p2.GetZ(), rcx, rcy, rcw, rch))
			{
				return true;
			}
			if (CheckHitLinePlane(p1, p2, new Vector3D(rcx, 0f, rcy), new Vector3D(x, 0f, rcy)))
			{
				return true;
			}
			if (CheckHitLinePlane(p1, p2, new Vector3D(rcx, 0f, z), new Vector3D(x, 0f, z)))
			{
				return true;
			}
			if (CheckHitLinePlane(p1, p2, new Vector3D(rcx, 0f, rcy), new Vector3D(rcx, 0f, z)))
			{
				return true;
			}
			if (CheckHitLinePlane(p1, p2, new Vector3D(x, 0f, rcy), new Vector3D(x, 0f, z)))
			{
				return true;
			}
			return false;
		}

		private static bool CheckLineCrossRough(Vector3D a1, Vector3D a2, Vector3D b1, Vector3D b2)
		{
			if (a1.GetX() >= a2.GetX())
			{
				if (a1.GetX() < b1.GetX() && a1.GetX() < b2.GetX())
				{
					return false;
				}
				if (a2.GetX() > b1.GetX() && a2.GetX() > b2.GetX())
				{
					return false;
				}
			}
			else
			{
				if (a2.GetX() < b1.GetX() && a2.GetX() < b2.GetX())
				{
					return false;
				}
				if (a1.GetX() > b1.GetX() && a1.GetX() > b2.GetX())
				{
					return false;
				}
			}
			if (a1.GetZ() >= a2.GetZ())
			{
				if (a1.GetZ() < b1.GetZ() && a1.GetZ() < b2.GetZ())
				{
					return false;
				}
				if (a2.GetZ() > b1.GetZ() && a2.GetZ() > b2.GetZ())
				{
					return false;
				}
			}
			else
			{
				if (a2.GetZ() < b1.GetZ() && a2.GetZ() < b2.GetZ())
				{
					return false;
				}
				if (a1.GetZ() > b1.GetZ() && a1.GetZ() > b2.GetZ())
				{
					return false;
				}
			}
			return true;
		}

		private static bool CheckLineCrossLeftRight(Vector3D a1, Vector3D a2, Vector3D b1, Vector3D b2)
		{
			float num = (a1.GetX() - a2.GetX()) * (b1.GetZ() - a1.GetZ()) + (a1.GetZ() - a2.GetZ()) * (a1.GetX() - b1.GetX());
			float num2 = (a1.GetX() - a2.GetX()) * (b2.GetZ() - a1.GetZ()) + (a1.GetZ() - a2.GetZ()) * (a1.GetX() - b2.GetX());
			if (num * num2 > 0f)
			{
				return false;
			}
			float num3 = (b1.GetX() - b2.GetX()) * (a1.GetZ() - b1.GetZ()) + (b1.GetZ() - b2.GetZ()) * (b1.GetX() - a1.GetX());
			float num4 = (b1.GetX() - b2.GetX()) * (a2.GetZ() - b1.GetZ()) + (b1.GetZ() - b2.GetZ()) * (b1.GetX() - a2.GetX());
			if (num3 * num4 > 0f)
			{
				return false;
			}
			return true;
		}

		private static bool CheckLineCrossRough(float a1x, float a1y, float a2x, float a2y, float b1x, float b1y, float b2x, float b2y)
		{
			if (a1x >= a2x)
			{
				if (a1x < b1x && a1x < b2x)
				{
					return false;
				}
				if (a2x > b1x && a2x > b2x)
				{
					return false;
				}
			}
			else
			{
				if (a2x < b1x && a2x < b2x)
				{
					return false;
				}
				if (a1x > b1x && a1x > b2x)
				{
					return false;
				}
			}
			if (a1y >= a2y)
			{
				if (a1y < b1y && a1y < b2y)
				{
					return false;
				}
				if (a2y > b1y && a2y > b2y)
				{
					return false;
				}
			}
			else
			{
				if (a2y < b1y && a2y < b2y)
				{
					return false;
				}
				if (a1y > b1y && a1y > b2y)
				{
					return false;
				}
			}
			return true;
		}

		private static bool CheckLineCrossLeftRight(float a1x, float a1y, float a2x, float a2y, float b1x, float b1y, float b2x, float b2y)
		{
			float num = (a1x - a2x) * (b1y - a1y) + (a1y - a2y) * (a1x - b1x);
			float num2 = (a1x - a2x) * (b2y - a1y) + (a1y - a2y) * (a1x - b2x);
			if (num * num2 > 0f)
			{
				return false;
			}
			float num3 = (b1x - b2x) * (a1y - b1y) + (b1y - b2y) * (b1x - a1x);
			float num4 = (b1x - b2x) * (a2y - b1y) + (b1y - b2y) * (b1x - a2x);
			if (num3 * num4 > 0f)
			{
				return false;
			}
			return true;
		}

		public static bool CheckHitLinePlane(Vector3D a1, Vector3D a2, Vector3D b1, Vector3D b2)
		{
			if (!CheckLineCrossRough(a1, a2, b1, b2))
			{
				return false;
			}
			if (!CheckLineCrossLeftRight(a1, a2, b1, b2))
			{
				return false;
			}
			return true;
		}

		public static bool CheckHitLinePlane(float a1x, float a1y, float a2x, float a2y, float b1x, float b1y, float b2x, float b2y)
		{
			if (!CheckLineCrossRough(a1x, a1y, a2x, a2y, b1x, b1y, b2x, b2y))
			{
				return false;
			}
			if (!CheckLineCrossLeftRight(a1x, a1y, a2x, a2y, b1x, b1y, b2x, b2y))
			{
				return false;
			}
			return true;
		}

		public static Vector3D GetCrossPlane(Vector3D a1, Vector3D a2, Vector3D p)
		{
			Vector3D vector3D = new Vector3D();
			Vector3D vector3D2 = new Vector3D(a2.GetX() - a1.GetX(), 0f, a2.GetZ() - a1.GetZ());
			Vector3D vector3D3 = new Vector3D(p.GetX() - a1.GetX(), 0f, p.GetZ() - a1.GetZ());
			float num = vector3D2.GetX() * vector3D3.GetX() + vector3D2.GetZ() * vector3D3.GetZ();
			float num2 = vector3D2.GetX() * vector3D2.GetX() + vector3D2.GetZ() * vector3D2.GetZ();
			if (num2 == 0f)
			{
				return null;
			}
			float num3 = num / num2;
			Vector3D vector3D4 = new Vector3D(vector3D2.GetX() * num3, 0f, vector3D2.GetZ() * num3);
			vector3D.Set(a1.GetX() + vector3D4.GetX(), 0f, a1.GetZ() + vector3D4.GetZ());
			return vector3D;
		}

		public static Vector3D GetCrossLine(Vector3D v1, Vector3D v2, Vector3D v3, Vector3D v4)
		{
			return GetCrossLine(v1.GetX(), v1.GetZ(), v2.GetX(), v2.GetZ(), v3.GetX(), v3.GetZ(), v4.GetX(), v4.GetZ());
		}

		public static Vector3D GetCrossLine(float v1x, float v1y, float v2x, float v2y, float v3x, float v3y, float v4x, float v4y)
		{
			float single = v2y - v1y;
			float single1 = v1x - v2x;
			float single2 = -single * v1x - single1 * v1y;
			float single3 = v4y - v3y;
			float single4 = v3x - v4x;
			float single5 = -single3 * v3x - single4 * v3y;
			float single6 = single1 * single3 - single4 * single;
			float single7 = (single2 * single4 - single5 * single1) / single6;
			float single8 = (single * single5 - single3 * single2) / single6;
			return new Vector3D(single7, 0f, single8);
		}

		public static bool IsContainPoint(Vector3D[] v, float x, float z)
		{
			Vector3D[] array = new Vector3D[v.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Vector3D(x - v[i].GetX(), 0f, z - v[i].GetZ());
			}
			Vector3D[] array2 = new Vector3D[v.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = new Vector3D(array[(j + 1) % array.Length].GetX() - array[j].GetX(), 0f, array[(j + 1) % array.Length].GetZ() - array[j].GetZ());
			}
			float num = 0f;
			for (int k = 0; k < v.Length; k++)
			{
				float num2 = array[k].GetX() * array2[k].GetZ() - array[k].GetZ() * array2[k].GetX();
				if (k == 0)
				{
					num = num2;
					continue;
				}
				if (num >= 0f && num2 < 0f)
				{
					return false;
				}
				if (num <= 0f && num2 > 0f)
				{
					return false;
				}
			}
			return true;
		}

		public static float VecToAngle(float x, float z)
		{
			if (x == 0f && z == 0f)
			{
				return 0f;
			}
			float num = FastMath.Atan2(z, x);
			if (x < 0f)
			{
				num = (num + 180f) % 360f;
			}
			if (x == 0f && z < 0f)
			{
				return 180f;
			}
			return num;
		}

		public static float VecToAngle(float v1x, float v1y, float v2x, float v2y)
		{
			float num = v1x * v1x + v1y * v1y;
			if (num <= 0.9999f || 1.0001f <= num)
			{
				float num2 = FastMath.Sqrt(num);
				v1x /= num2;
				v1y /= num2;
			}
			num = v2x * v2x + v2y * v2y;
			if (num <= 0.9999f || 1.0001f <= num)
			{
				float num3 = FastMath.Sqrt(num);
				v2x /= num3;
				v2y /= num3;
			}
			float num4 = v2x * v1x + v2y * v1y;
			if (num4 < -1f)
			{
				num4 = -1f;
			}
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			if (JFloat.IsNaN(num4))
			{
				return float.NaN;
			}
			float num5 = FastMath.Acos(num4);
			num = v2x * v1y - v2y * v1x;
			if (num > 0f)
			{
				num5 = 360f - num5;
			}
			return num5;
		}

		public static float[] TransPoint(float x, float y, float z, Transform viewMatrix, int viewAngle)
		{
			float num = 1f / FastMath.Tan(viewAngle / 2) * 120f;
			viewMatrix.Translate(x, y, z);
			viewMatrix.TransVector(transPoint_tmp2, transPoint_tmp3);
			transPoint_tmp3.SetX(transPoint_tmp3.GetX() * num / transPoint_tmp3.GetZ());
			transPoint_tmp3.SetY(transPoint_tmp3.GetY() * num / transPoint_tmp3.GetZ());
			transPoint_tmp1[0] = 120f + transPoint_tmp3.GetX();
			transPoint_tmp1[1] = 120f + transPoint_tmp3.GetY();
			transPoint_tmp1[2] = transPoint_tmp3.GetZ();
			return transPoint_tmp1;
		}

		public static int DotProduct(int ax, int ay, int bx, int by, int cx, int cy, int dx, int dy)
		{
			int num = bx - ax;
			int num2 = by - ay;
			int num3 = dx - cx;
			int num4 = dy - cy;
			return num * num3 + num2 * num4;
		}

		public static int DotProduct(int ax, int ay, int bx, int by, int cx, int cy)
		{
			int num = bx - ax;
			int num2 = by - ay;
			int num3 = cx - ax;
			int num4 = cy - ay;
			return num * num3 + num2 * num4;
		}

		public static float DotProduct(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy)
		{
			float num = bx - ax;
			float num2 = by - ay;
			float num3 = dx - cx;
			float num4 = dy - cy;
			return num * num3 + num2 * num4;
		}

		public static float DotProduct(float ax, float ay, float bx, float by, float cx, float cy)
		{
			float num = bx - ax;
			float num2 = by - ay;
			float num3 = cx - ax;
			float num4 = cy - ay;
			return num * num3 + num2 * num4;
		}

		public static float DotProduct(float v1x, float v1y, float v2x, float v2y)
		{
			return v1x * v2x + v1y * v2y;
		}

		public static int Pow(int x, int y)
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

		public static long Sqrt(long c)
		{
			if (c == 0L)
			{
				return 0L;
			}
			int num = 10;
			long num2 = 0L;
			do
			{
				num2++;
			}
			while (num2 * num2 < c);
			for (int i = 0; i < num; i++)
			{
				num2 = (num2 + c / num2) / 2;
			}
			return num2;
		}

		public static float Cbrt(float c)
		{
			double num = 1E-11;
			double num2 = c;
			double num4;
double num3 = 1.0;

			do
			{
				num4 = num3;
				num3 = (2.0 * num4 + num2 / (num4 * num4)) / 3.0;
			}
			while (JMath.Abs(num3 - num4) > num);
			return (float)num3;
		}

		public static void Sort(int[] list)
		{
			for (int i = 0; i < list.Length - 1; i++)
			{
				for (int num = list.Length - 1; num > i; num--)
				{
					if (list[num - 1] > list[num])
					{
						int num2 = list[num];
						list[num] = list[num - 1];
						list[num - 1] = num2;
					}
				}
			}
		}

		public static int[][] QSort(int[][] data, int target, bool upOrDown)
		{
			int[][] array = null;
			QuickSort(data, 0, data.Length - 1, target);
			if (!upOrDown)
			{
				array = new int[data.Length][];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = data[array.Length - i - 1];
				}
			}
			else
			{
				array = data;
			}
			return array;
		}

		private static int Pivot(int[][] a, int i, int j, int target)
		{
			int k;
			for (k = i + 1; k <= j && a[i][target] == a[k][target]; k++)
			{
			}
			if (k > j)
			{
				return -1;
			}
			if (a[i][target] >= a[k][target])
			{
				return i;
			}
			return k;
		}

		private static int Partition(int[][] a, int i, int j, int x, int target)
		{
			int k = i;
			int num = j;
			while (k <= num)
			{
				for (; k <= j && a[k][target] < x; k++)
				{
				}
				while (num >= i && a[num][target] >= x)
				{
					num--;
				}
				if (k > num)
				{
					break;
				}
				int[] array = a[k];
				a[k] = a[num];
				a[num] = array;
				k++;
				num--;
			}
			return k;
		}

		private static void QuickSort(int[][] a, int i, int j, int target)
		{
			if (i == j)
			{
				return;
			}
			int num = KairoMath.Pivot(a, i, j, target);
			if (num != -1)
			{
				int num1 = KairoMath.Partition(a, i, j, a[num][target], target);
				KairoMath.QuickSort(a, i, num1 - 1, target);
				KairoMath.QuickSort(a, num1, j, target);
			}
		}

		public static float[] GetInscribedCircle(float ax, float ay, float bx, float by, float cx, float cy)
		{
			return GetInscribedCircle(ax, ay, bx, by, cx, cy, null);
		}

		public static float[] GetInscribedCircle(float ax, float ay, float bx, float by, float cx, float cy, float[] vout)
		{
			if (vout == null)
			{
				vout = new float[3];
			}
			float num = bx - ax;
			float num2 = by - ay;
			float num3 = cx - bx;
			float num4 = cy - by;
			float num5 = ax - cx;
			float num6 = ay - cy;
			float num7 = FastMath.Sqrt(num3 * num3 + num4 * num4);
			float num8 = FastMath.Sqrt(num5 * num5 + num6 * num6);
			float num9 = FastMath.Sqrt(num * num + num2 * num2);
			float num10 = (num7 * ax + num8 * bx + num9 * cx) / (num7 + num8 + num9);
			float num11 = (num7 * ay + num8 * by + num9 * cy) / (num7 + num8 + num9);
			float num12 = (num7 + num8 + num9) / 2f;
			float num13 = GetTriangleArea(ax, ay, bx, by, cx, cy) / num12;
			vout[0] = num10;
			vout[1] = num11;
			vout[2] = num13;
			return vout;
		}

		public static float[] GetCircumscribedCircle(float ax, float ay, float bx, float by, float cx, float cy)
		{
			return GetCircumscribedCircle(ax, ay, bx, by, cx, cy, null);
		}

		public static float[] GetCircumscribedCircle(float ax, float ay, float bx, float by, float cx, float cy, float[] vout)
		{
			if (vout == null)
			{
				vout = new float[3];
			}
			float num = bx - ax;
			float num2 = by - ay;
			float num3 = cx - bx;
			float num4 = cy - by;
			float num5 = ax - cx;
			float num6 = ay - cy;
			float num7 = FastMath.Sqrt(num3 * num3 + num4 * num4);
			float num8 = FastMath.Sqrt(num5 * num5 + num6 * num6);
			float num9 = FastMath.Sqrt(num * num + num2 * num2);
			float triangleArea = GetTriangleArea(ax, ay, bx, by, cx, cy);
			float num10 = num7 * num7;
			float num11 = num8 * num8;
			float num12 = num9 * num9;
			float num13 = (num10 * (num11 + num12 - num10) * ax + num11 * (num12 + num10 - num11) * bx + num12 * (num10 + num11 - num12) * cx) / (16f * triangleArea * triangleArea);
			float num14 = (num10 * (num11 + num12 - num10) * ay + num11 * (num12 + num10 - num11) * by + num12 * (num10 + num11 - num12) * cy) / (16f * triangleArea * triangleArea);
			float num15 = num7 * num8 * num9 / (4f * triangleArea);
			vout[0] = num13;
			vout[1] = num14;
			vout[2] = num15;
			return vout;
		}

		public static float[] GetEscribedCircle(float ax, float ay, float bx, float by, float cx, float cy)
		{
			return GetEscribedCircle(ax, ay, bx, by, cx, cy, 0, null);
		}

		public static float[] GetEscribedCircle(float ax, float ay, float bx, float by, float cx, float cy, float[] vout)
		{
			return GetEscribedCircle(ax, ay, bx, by, cx, cy, 0, vout);
		}

		public static float[] GetEscribedCircle(float ax, float ay, float bx, float by, float cx, float cy, int side)
		{
			return GetEscribedCircle(ax, ay, bx, by, cx, cy, side, null);
		}

		public static float[] GetEscribedCircle(float ax, float ay, float bx, float by, float cx, float cy, int side, float[] vout)
		{
			if (vout == null)
			{
				vout = new float[3];
			}
			float num = bx - ax;
			float num2 = by - ay;
			float num3 = cx - bx;
			float num4 = cy - by;
			float num5 = ax - cx;
			float num6 = ay - cy;
			float num7 = FastMath.Sqrt(num3 * num3 + num4 * num4);
			float num8 = FastMath.Sqrt(num5 * num5 + num6 * num6);
			float num9 = FastMath.Sqrt(num * num + num2 * num2);
			float triangleArea = GetTriangleArea(ax, ay, bx, by, cx, cy);
			if (side == 0)
			{
				num9 *= -1f;
			}
			if (side == 1)
			{
				num7 *= -1f;
			}
			if (side == 2)
			{
				num8 *= -1f;
			}
			float num10 = (num7 * ax + num8 * bx + num9 * cx) / (num7 + num8 + num9);
			float num11 = (num7 * ay + num8 * by + num9 * cy) / (num7 + num8 + num9);
			float num12 = 2f * triangleArea / (num9 + num7 + num8);
			vout[0] = num10;
			vout[1] = num11;
			vout[2] = num12;
			return vout;
		}

		public static float GetTriangleArea(float ax, float ay, float bx, float by, float cx, float cy)
		{
			return 0.5f * JMath.Abs((bx - ax) * (cy - ay) - (cx - ax) * (by - ay));
		}

		public static int GetLineSide(float px, float py, float x1, float y1, float x2, float y2)
		{
			float num = px * (y1 - y2) + x1 * (y2 - py) + x2 * (py - y1);
			if (num > 0f)
			{
				return 1;
			}
			if (num < 0f)
			{
				return -1;
			}
			return 0;
		}

		public static float GetDifferenceAngle(float angle1, float angle2)
		{
			float num = JMath.Abs(angle2 - angle1);
			if (num > 180f)
			{
				if ((angle2 < angle1)) { angle1 = (angle1 - 360f); } else { angle1 = (angle1 + 360f); }
				num = JMath.Abs(angle2 - angle1);
			}
			return num;
		}

		public static Vector3D GetNormalVector(Vector3D p1, Vector3D p2, Vector3D p3)
		{
			Vector3D vector3D = getNormalVector_tmp[0];
			Vector3D vector3D2 = getNormalVector_tmp[1];
			vector3D.Set(p2.GetX() - p1.GetX(), p2.GetY() - p1.GetY(), p2.GetZ() - p1.GetZ());
			vector3D2.Set(p3.GetX() - p1.GetX(), p3.GetY() - p1.GetY(), p3.GetZ() - p1.GetZ());
			Vector3D vector3D3 = new Vector3D();
			vector3D3.Cross(vector3D, vector3D2);
			vector3D3.Normalize();
			return vector3D3;
		}

		public static float[] GetNormalVector2D(float x1, float y1, float x2, float y2)
		{
			float single = y2 - y1;
			float single1 = -(x2 - x1);
			if (single * single + single1 * single1 != 0f)
			{
				float single2 = FastMath.Sqrt(single * single + single1 * single1);
				single /= single2;
				single1 /= single2;
			}
			KairoMath.getNormalVector2D_tmp[0] = single;
			KairoMath.getNormalVector2D_tmp[1] = single1;
			return KairoMath.getNormalVector2D_tmp;
		}

		public static float GetArrivalAngle(float speed, float gravity, float distance)
		{
			float num = distance * gravity / (speed * speed);
			if (num > 1f)
			{
				return float.NaN;
			}
			return FastMath.Asin(num) / 2f;
		}

		public static float GetArrivalDistance(float speed, float gravity, float angle)
		{
			float num = FastMath.Cos(angle) * speed;
			float num2 = FastMath.Sin(angle) * speed;
			return 2f * (num * num2) / gravity;
		}

		public static float GetArrivalSpeed(float gravity, float angle, float distance)
		{
			return FastMath.Sqrt(distance / (FastMath.Sin(2f * angle) / gravity));
		}
	}
}
