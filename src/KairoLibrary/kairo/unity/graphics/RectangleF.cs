using UnityEngine;

namespace kairo.unity.graphics
{
	public class RectangleF
	{
		public float bottom;

		public float left;

		public float right;

		public float top;

		public RectangleF()
		{
		}

		public RectangleF(float left, float top, float right, float bottom)
		{
			Set(left, top, right, bottom);
		}

		public RectangleF(RectangleF rc)
		{
			Set(rc);
		}

		public RectangleF(Rectangle rc)
		{
			Set(rc);
		}

		public void Set(float left, float top, float right, float bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public void Set(RectangleF rc)
		{
			left = rc.left;
			top = rc.top;
			right = rc.right;
			bottom = rc.bottom;
		}

		public void Set(Rectangle rc)
		{
			left = rc.left;
			top = rc.top;
			right = rc.right;
			bottom = rc.bottom;
		}

		public float Width()
		{
			return right - left;
		}

		public float Height()
		{
			return bottom - top;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is RectangleF)
			{
				RectangleF rectangleF = (RectangleF)obj;
				if (left == rectangleF.left && top == rectangleF.top && right == rectangleF.right)
				{
					return bottom == rectangleF.bottom;
				}
				return false;
			}
			return this == obj;
		}

		public Rect ToRect()
		{
			return new Rect(left, top, Width(), Height());
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
