namespace kairo.unity.graphics
{
	public class Point
	{
		public int x;

		public int y;

		public Point()
		{
		}

		public Point(int x, int y)
		{
			Set(x, y);
		}

		public Point(Point pt)
		{
			Set(pt);
		}

		public void Set(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public void Set(Point pt)
		{
			x = pt.x;
			y = pt.y;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Point)
			{
				Point point = (Point)obj;
				if (x == point.x)
				{
					return y == point.y;
				}
				return false;
			}
			return this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
