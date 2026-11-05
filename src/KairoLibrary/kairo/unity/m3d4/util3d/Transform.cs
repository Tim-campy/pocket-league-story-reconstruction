using System;
using java.lang;

namespace kairo.unity.m3d4.util3d
{
	public class Transform
	{
		private static readonly float[] identityMatrix_ = new float[16]
		{
			1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f,
			1f, 0f, 0f, 0f, 0f, 1f
		};

		private float[] mtx_ = new float[16];

		public Transform()
		{
			SetIdentity();
		}

		public Transform(Transform transform)
		{
			Set(transform);
		}

		public void Get(float[] matrix)
		{
			JSystem.Arraycopy(mtx_, 0, matrix, 0, matrix.Length);
		}

		public float Get(int index)
		{
			return mtx_[index];
		}

		public void Invert()
		{
		}

		public void LookAt(Vector3D position, Vector3D look, Vector3D up)
		{
			Vector3D vector3D = new Vector3D(look.GetX() - position.GetX(), look.GetY() - position.GetY(), look.GetZ() - position.GetZ());
			vector3D.Normalize();
			up.Normalize();
			Vector3D vector3D1 = new Vector3D();
			vector3D1.Cross(vector3D, up);
			Vector3D vector3D2 = new Vector3D();
			vector3D2.Cross(vector3D1, vector3D);
			this.mtx_[0] = vector3D1.GetX();
			this.mtx_[1] = vector3D1.GetY();
			this.mtx_[2] = vector3D1.GetZ();
			this.mtx_[3] = 0f;
			this.mtx_[4] = -vector3D2.GetX();
			this.mtx_[5] = -vector3D2.GetY();
			this.mtx_[6] = -vector3D2.GetZ();
			this.mtx_[7] = 0f;
			this.mtx_[8] = vector3D.GetX();
			this.mtx_[9] = vector3D.GetY();
			this.mtx_[10] = vector3D.GetZ();
			this.mtx_[11] = 0f;
			this.mtx_[12] = 0f;
			this.mtx_[13] = 0f;
			this.mtx_[14] = 0f;
			this.mtx_[15] = 1f;
			this.Translate(-position.GetX(), -position.GetY(), -position.GetZ());
		}

		public void Multiply(Transform transform)
		{
			if (transform == null)
			{
				throw new NullReferenceException();
			}
			float[] array = new float[16];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[i * 4 + j] = 0f;
					for (int k = 0; k < 4; k++)
					{
						array[i * 4 + j] += mtx_[i * 4 + k] * transform.mtx_[k * 4 + j];
					}
				}
			}
			Set(array);
		}

		public void Rotate(float x, float y, float z, float angle)
		{
			if (_checkArgumentException(x, y, z) || _checkArgumentException(angle) || (angle != 0f && x == 0f && y == 0f && z == 0f))
			{
				throw new ArgumentException();
			}
			Vector3D axis = new Vector3D(x, y, z);
			axis.Normalize();
			float num = FastMath.Sin(angle);
			float num2 = FastMath.Cos(angle);
			Transform transform = new Transform();
			transform.Set(new float[16]
			{
				x * x * (1f - num2) + num2,
				x * y * (1f - num2) - z * num,
				x * z * (1f - num2) + y * num,
				0f,
				y * x * (1f - num2) + z * num,
				y * y * (1f - num2) + num2,
				y * z * (1f - num2) - x * num,
				0f,
				x * z * (1f - num2) - y * num,
				y * z * (1f - num2) + x * num,
				z * z * (1f - num2) + num2,
				0f,
				0f,
				0f,
				0f,
				1f
			});
			Multiply(transform);
		}

		public void Rotate(Vector3D v, float angle)
		{
			Rotate(v.GetX(), v.GetY(), v.GetZ(), angle);
		}

		public void RotateQuat(float x, float y, float z, float w)
		{
		}

		public void RotateQuat(Vector3D v, float w)
		{
		}

		public void Scale(float x, float y, float z)
		{
			if (_checkArgumentException(x, y, z))
			{
				throw new ArgumentException();
			}
			Transform transform = new Transform();
			transform.Set(new float[16]
			{
				x, 0f, 0f, 0f, 0f, y, 0f, 0f, 0f, 0f,
				z, 0f, 0f, 0f, 0f, 1f
			});
			Multiply(transform);
		}

		public void Scale(Vector3D v)
		{
			Scale(v.GetX(), v.GetY(), v.GetZ());
		}

		public void Set(float[] matrix)
		{
			JSystem.Arraycopy(matrix, 0, mtx_, 0, matrix.Length);
		}

		public void Set(int index, float value)
		{
			mtx_[index] = value;
		}

		public void Set(Transform transform)
		{
			JSystem.Arraycopy(transform.mtx_, 0, mtx_, 0, mtx_.Length);
		}

		public void SetIdentity()
		{
			JSystem.Arraycopy(identityMatrix_, 0, mtx_, 0, identityMatrix_.Length);
		}

		public void Translate(float x, float y, float z)
		{
			if (_checkArgumentException(x, y, z))
			{
				throw new ArgumentException();
			}
			Transform transform = new Transform();
			float[] obj = new float[16]
			{
				1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f,
				1f, 0f, 0f, 0f, 0f, 1f
			};
			obj[3] = x;
			obj[7] = y;
			obj[11] = z;
			transform.Set(obj);
			Multiply(transform);
		}

		public void Translate(Vector3D v)
		{
			Translate(v.GetX(), v.GetY(), v.GetZ());
		}

		public void Transpose()
		{
			float[] array = new float[16]
			{
				mtx_[0],
				mtx_[4],
				mtx_[8],
				mtx_[12],
				mtx_[1],
				mtx_[5],
				mtx_[9],
				mtx_[13],
				mtx_[2],
				mtx_[6],
				mtx_[10],
				mtx_[14],
				mtx_[3],
				mtx_[7],
				mtx_[11],
				mtx_[15]
			};
		}

		public void TransVector(Vector3D v, Vector3D result)
		{
			if (v == null || result == null)
			{
				throw new NullReferenceException();
			}
			result.Set(v);
			TransVector(ref result.x_, ref result.y_, ref result.z_);
		}

		public void TransVector(ref float x, ref float y, ref float z)
		{
			float num = x;
			float num2 = y;
			float num3 = z;
			x = mtx_[0] * num + mtx_[1] * num2 + mtx_[2] * num3 + mtx_[3];
			y = mtx_[4] * num + mtx_[5] * num2 + mtx_[6] * num3 + mtx_[7];
			z = mtx_[8] * num + mtx_[9] * num2 + mtx_[10] * num3 + mtx_[11];
		}

		public void TransVectorXY(ref float x, ref float y)
		{
			float num = x;
			float num2 = y;
			x = mtx_[0] * num + mtx_[1] * num2 + mtx_[3];
			y = mtx_[4] * num + mtx_[5] * num2 + mtx_[7];
		}

		public void TransVectorXZ(ref float x, ref float z)
		{
			float num = x;
			float num2 = z;
			x = mtx_[0] * num + mtx_[2] * num2 + mtx_[3];
			z = mtx_[8] * num + mtx_[10] * num2 + mtx_[11];
		}

		private static bool _checkArgumentException(float x, float y, float z)
		{
			if (!_checkArgumentException(x) && !_checkArgumentException(y))
			{
				return _checkArgumentException(z);
			}
			return true;
		}

		private static bool _checkArgumentException(float v)
		{
			if (v != float.NaN && v != float.PositiveInfinity)
			{
				return v == float.NegativeInfinity;
			}
			return true;
		}
	}
}
