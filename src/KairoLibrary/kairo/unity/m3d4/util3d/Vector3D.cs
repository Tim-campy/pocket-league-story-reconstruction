using System;
using java.io;
using kairo.unity.util;

namespace kairo.unity.m3d4.util3d
{
	public class Vector3D : ISerializable
	{
		public float x_;

		public float y_;

		public float z_;

		public Vector3D()
		{
			x_ = (y_ = (z_ = 0f));
		}

		public Vector3D(float x, float y, float z)
		{
			Set(x, y, z);
		}

		public Vector3D(Vector3D v)
		{
			Set(v);
		}

		public void Add(float x, float y, float z)
		{
			x_ += x;
			y_ += y;
			z_ += z;
		}

		public void Add(Vector3D v)
		{
			Add(v.x_, v.y_, v.z_);
		}

		public void Cross(Vector3D v)
		{
			Cross(this, v);
		}

		public void Cross(Vector3D u, Vector3D v)
		{
			Set(u.y_ * v.z_ - u.z_ * v.y_, u.z_ * v.x_ - u.x_ * v.z_, u.x_ * v.y_ - u.y_ * v.x_);
		}

		public float Dot(Vector3D v)
		{
			return x_ * v.x_ + y_ * v.y_ + z_ * v.z_;
		}

		public static float Dot(Vector3D v1, Vector3D v2)
		{
			return v1.x_ * v2.x_ + v1.y_ * v2.y_ + v1.z_ * v2.z_;
		}

		public float GetX()
		{
			return x_;
		}

		public float GetY()
		{
			return y_;
		}

		public float GetZ()
		{
			return z_;
		}

		public void Normalize()
		{
			if (this.x_ == 0f && this.y_ == 0f && this.z_ == 0f)
			{
				throw new ArithmeticException();
			}
			float single = FastMath.Sqrt(this.x_ * this.x_ + this.y_ * this.y_ + this.z_ * this.z_);
			if (single == 0f)
			{
				return;
			}
			this.x_ /= single;
			this.y_ /= single;
			this.z_ /= single;
		}

		public void Set(float x, float y, float z)
		{
			x_ = x;
			y_ = y;
			z_ = z;
		}

		public void Set(Vector3D v)
		{
			Set(v.x_, v.y_, v.z_);
		}

		public void SetX(float x)
		{
			x_ = x;
		}

		public void SetY(float y)
		{
			y_ = y;
		}

		public void SetZ(float z)
		{
			z_ = z;
		}

		public void Serialize(OutputStream stream)
		{
			StreamUtil.WriteFloat(stream, x_);
			StreamUtil.WriteFloat(stream, y_);
			StreamUtil.WriteFloat(stream, z_);
		}

		public void Deserialize(InputStream stream)
		{
			x_ = StreamUtil.ReadFloat(stream);
			y_ = StreamUtil.ReadFloat(stream);
			z_ = StreamUtil.ReadFloat(stream);
		}

		public static Vector3D operator +(Vector3D z, Vector3D w)
		{
			return new Vector3D(z.x_ + w.x_, z.y_ + w.y_, z.z_ + w.z_);
		}

		public static Vector3D operator -(Vector3D z, Vector3D w)
		{
			return new Vector3D(z.x_ - w.x_, z.y_ - w.y_, z.z_ - w.z_);
		}

		public static Vector3D operator *(Vector3D z, float w)
		{
			return new Vector3D(z.x_ * w, z.y_ * w, z.z_ * w);
		}

		public static Vector3D operator /(Vector3D z, float w)
		{
			return new Vector3D(z.x_ / w, z.y_ / w, z.z_ / w);
		}
	}
}
