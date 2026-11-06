using System;
using java.lang;
using kairo.unity.m3d4.util3d;

namespace kairo.unity.ui
{
	public class Matrix
	{
		private static readonly float[] identityMatrix_ = new float[9] { 1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f };

		private float[] mtx_ = new float[9];

		private float[] mul_ = new float[9];

		private float[] set_ = new float[9];

		private static Matrix RotateTempMatrix_ = new Matrix();

		private static Matrix ScaleTemporaryMatrix_ = new Matrix();

		private static Matrix TranslateTempMatrix_ = new Matrix();

		public Matrix()
		{
			Reset();
		}

		public Matrix(Matrix transform)
		{
			Set(transform);
		}

		public Matrix Reset()
		{
			JSystem.Arraycopy(identityMatrix_, 0, mtx_, 0, identityMatrix_.Length);
			return this;
		}

		public Matrix SetValues(float[] matrix)
		{
			JSystem.Arraycopy(matrix, 0, mtx_, 0, matrix.Length);
			return this;
		}

		public Matrix Set(Matrix transform)
		{
			JSystem.Arraycopy(transform.mtx_, 0, mtx_, 0, mtx_.Length);
			return this;
		}

		public void GetValues(float[] matrix)
		{
			JSystem.Arraycopy(mtx_, 0, matrix, 0, matrix.Length);
		}

		public bool IsIdentity()
		{
			for (int i = 0; i < identityMatrix_.Length; i++)
			{
				if (identityMatrix_[i] != mtx_[i])
				{
					return false;
				}
			}
			return true;
		}

		public Matrix PostConcat(float[] transform)
		{
			if (transform == null)
			{
				throw new NullReferenceException();
			}
			int j, k, l;
			for (int i = 0; i < mul_.Length; i++)
			{
				mul_[i] = 0f;
			}
			for (j = 0; j < 3; j++)
			{
				for (k = 0; k < 3; k++)
				{
					mul_[j * 3 + k] = 0f;
					for (l = 0; l < 3; l++)
					{
						mul_[j * 3 + k] += mtx_[j * 3 + l] * transform[l * 3 + k];
					}
				}
			}
			SetValues(mul_);
			return this;
		}

		public Matrix PreConcat(float[] transform)
		{
			if (transform == null)
			{
				throw new NullReferenceException();
			}
			for (int i = 0; i < mul_.Length; i++)
			{
				mul_[i] = 0f;
			}
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					mul_[j * 3 + k] = 0f;
					for (int l = 0; l < 3; l++)
					{
						mul_[j * 3 + k] += transform[j * 3 + l] * mtx_[l * 3 + k];
					}
				}
			}
			SetValues(mul_);
			return this;
		}

		public Matrix SetConcat(float[] a, float[] b)
		{
			JSystem.Arraycopy(a, 0, mtx_, 0, a.Length);
			PostConcat(b);
			return this;
		}

		public Matrix PostConcat(Matrix transform)
		{
			PostConcat(transform.mtx_);
			return this;
		}

		public Matrix PreConcat(Matrix transform)
		{
			PreConcat(transform.mtx_);
			return this;
		}

		public Matrix SetConcat(Matrix a, Matrix b)
		{
			SetConcat(a.mtx_, b.mtx_);
			return this;
		}

		public Matrix PostRotate(float angle, float px = 0f, float py = 0f)
		{
			if (float.IsNaN(angle))
			{
				throw new ArgumentException();
			}
			if (px != 0f || py != 0f)
			{
				this.PostTranslate(-px, -py);
			}
			float single = FastMath.Sin(angle);
			float single1 = FastMath.Cos(angle);
			this.set_[0] = single1;
			this.set_[1] = single;
			this.set_[2] = 0f;
			this.set_[3] = -single;
			this.set_[4] = single1;
			this.set_[5] = 0f;
			this.set_[6] = 0f;
			this.set_[7] = 0f;
			this.set_[8] = 1f;
			this.PostConcat(this.set_);
			if (px != 0f || py != 0f)
			{
				this.PostTranslate(px, py);
			}
			return this;
		}

		public Matrix PreRotate(float angle, float px = 0f, float py = 0f)
		{
			if (float.IsNaN(angle))
			{
				throw new ArgumentException();
			}
			if (px != 0f || py != 0f)
			{
				this.PreTranslate(px, py);
			}
			float single = FastMath.Sin(angle);
			float single1 = FastMath.Cos(angle);
			this.set_[0] = single1;
			this.set_[1] = single;
			this.set_[2] = 0f;
			this.set_[3] = -single;
			this.set_[4] = single1;
			this.set_[5] = 0f;
			this.set_[6] = 0f;
			this.set_[7] = 0f;
			this.set_[8] = 1f;
			this.PreConcat(this.set_);
			if (px != 0f || py != 0f)
			{
				this.PreTranslate(-px, -py);
			}
			return this;
		}

		public Matrix SetRotate(float angle, float px = 0f, float py = 0f)
		{
			Reset();
			PostRotate(angle, px, py);
			return this;
		}

		public static Matrix Rotate(float angle, float px = 0f, float py = 0f)
		{
			Matrix matrix = new Matrix();
			matrix.SetRotate(angle, px, py);
			return matrix;
		}

		public static Matrix RotateTemporary(float angle, float px = 0f, float py = 0f)
		{
			return RotateTempMatrix_.SetRotate(angle, px, py);
		}

		public Matrix PostScale(float x, float y, float px = 0f, float py = 0f)
		{
			if (float.IsNaN(x) || float.IsNaN(y))
			{
				throw new ArgumentException();
			}
			if (px != 0f || py != 0f)
			{
				this.PostTranslate(-px, -py);
			}
			this.mtx_[0] *= x;
			this.mtx_[1] *= y;
			this.mtx_[3] *= x;
			this.mtx_[4] *= y;
			this.mtx_[6] *= x;
			this.mtx_[7] *= y;
			if (px != 0f || py != 0f)
			{
				this.PostTranslate(px, py);
			}
			return this;
		}

		public Matrix PreScale(float x, float y, float px = 0f, float py = 0f)
		{
			if (float.IsNaN(x) || float.IsNaN(y))
			{
				throw new ArgumentException();
			}
			if (px != 0f || py != 0f)
			{
				this.PreTranslate(px, py);
			}
			this.mtx_[0] *= x;
			this.mtx_[1] *= x;
			this.mtx_[2] *= x;
			this.mtx_[3] *= y;
			this.mtx_[4] *= y;
			this.mtx_[5] *= y;
			if (px != 0f || py != 0f)
			{
				this.PreTranslate(-px, -py);
			}
			return this;
		}

		public Matrix SetScale(float x, float y, float px = 0f, float py = 0f)
		{
			Reset();
			PostScale(x, y, px, py);
			return this;
		}

		public static Matrix Scale(float x, float y, float px = 0f, float py = 0f)
		{
			Matrix matrix = new Matrix();
			matrix.SetScale(x, y, px, py);
			return matrix;
		}

		public static Matrix ScaleTemporary(float x, float y, float px = 0f, float py = 0f)
		{
			return ScaleTemporaryMatrix_.SetScale(x, y, px, py);
		}

		public Matrix PostTranslate(float x, float y)
		{
			if (float.IsNaN(x) || float.IsNaN(y))
			{
				throw new ArgumentException();
			}
			mtx_[0] += mtx_[2] * x;
			mtx_[1] += mtx_[2] * y;
			mtx_[3] += mtx_[5] * x;
			mtx_[4] += mtx_[5] * y;
			mtx_[6] += mtx_[8] * x;
			mtx_[7] += mtx_[8] * y;
			return this;
		}

		public Matrix PreTranslate(float x, float y)
		{
			if (float.IsNaN(x) || float.IsNaN(y))
			{
				throw new ArgumentException();
			}
			mtx_[6] += mtx_[0] * x + mtx_[3] * y;
			mtx_[7] += mtx_[1] * x + mtx_[4] * y;
			mtx_[8] += mtx_[2] * x + mtx_[5] * y;
			return this;
		}

		public Matrix SetTranslate(float x, float y)
		{
			Reset();
			PostTranslate(x, y);
			return this;
		}

		public static Matrix Translate(float x, float y)
		{
			Matrix matrix = new Matrix();
			matrix.SetTranslate(x, y);
			return matrix;
		}

		public static Matrix TranslateTemporary(float x, float y)
		{
			return TranslateTempMatrix_.SetTranslate(x, y);
		}

		public void MapPoints(ref float x, ref float y)
		{
			float num = x;
			float num2 = y;
			x = mtx_[0] * num + mtx_[3] * num2 + mtx_[6];
			y = mtx_[1] * num + mtx_[4] * num2 + mtx_[7];
		}

		public void MapPoints(float[] pts)
		{
			MapPoints(pts, pts);
		}

		public void MapPoints(float[] src, float[] dst)
		{
			for (int i = 0; i < src.Length; i += 2)
			{
				float num = src[i + 0];
				float num2 = src[i + 1];
				dst[i + 0] = mtx_[0] * num + mtx_[3] * num2 + mtx_[6];
				dst[i + 1] = mtx_[1] * num + mtx_[4] * num2 + mtx_[7];
			}
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Matrix)
			{
				Matrix matrix = (Matrix)obj;
				for (int i = 0; i < mtx_.Length; i++)
				{
					if (mtx_[i] != matrix.mtx_[i])
					{
						return false;
					}
				}
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
