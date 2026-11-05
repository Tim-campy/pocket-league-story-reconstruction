using UnityEngine;

namespace kairo.unity.graphics
{
	public class Rectangle
	{
		public int bottom;

		public int left;

		public int right;

		public int top;

		public Rectangle()
		{
		}

		public Rectangle(int left, int top, int right, int bottom)
		{
			Set(left, top, right, bottom);
		}

		public Rectangle(Rectangle rc)
		{
			Set(rc);
		}

		public void Set(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public void Set(Rectangle rc)
		{
			left = rc.left;
			top = rc.top;
			right = rc.right;
			bottom = rc.bottom;
		}

		public int Width()
		{
			return right - left;
		}

		public int Height()
		{
			return bottom - top;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Rectangle)
			{
				Rectangle rectangle = (Rectangle)obj;
				if (left == rectangle.left && top == rectangle.top && right == rectangle.right)
				{
					return bottom == rectangle.bottom;
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
