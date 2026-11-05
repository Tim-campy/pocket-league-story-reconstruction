using kairo.unity.util;

namespace kairo.unity.surface
{
	public class TouchComponent
	{
		public SurfaceBase surface_;

		public int id;

		public int x;

		public int y;

		public int width;

		public int height;

		public int value;

		public TouchOption option;

		public int paintX;

		public int paintY;

		public int paintFrame;

		internal bool isLock;

		internal int hasCount;

		public TouchComponent()
		{
			Init();
		}

		public TouchComponent(TouchComponent copy)
		{
			Init();
			paintX = copy.paintX;
			paintY = copy.paintY;
			surface_ = copy.surface_;
			id = copy.id;
			x = copy.x;
			y = copy.y;
			width = copy.width;
			height = copy.height;
			value = copy.value;
			if (copy.option != null)
			{
				option = new TouchOption(copy.option);
			}
			paintFrame = copy.paintFrame;
			isLock = true;
		}

		public void Init()
		{
			paintX = (paintY = int.MaxValue);
			surface_ = null;
			id = -1;
			x = 0;
			y = 0;
			width = 0;
			height = 0;
			value = 0;
			option = null;
			paintFrame = 0;
			isLock = false;
			hasCount = 0;
		}

		public void SetPaint(int x, int y)
		{
			paintX = x;
			paintY = y;
		}

		public void SetPaint(int x, int y, int frame)
		{
			paintX = x;
			paintY = y;
			paintFrame = frame;
		}

		public long GetKey()
		{
			return GetKey(id, value);
		}

		public static long GetKey(int id, int value)
		{
			return BitUtil.Join(id, value);
		}

		public static int GetId(long key)
		{
			return BitUtil.Split(key)[0];
		}

		public static int GetValue(long key)
		{
			return BitUtil.Split(key)[1];
		}

		public bool Check(int bit)
		{
			int num = 0;
			if (option != null)
			{
				num = option.flag;
			}
			return (num & bit) != 0;
		}

		public int GetPriority()
		{
			int result = 1;
			if (hasCount > 0)
			{
				result = -10;
			}
			if (option != null)
			{
				if ((option.flag & 2) != 0)
				{
					result = 0;
				}
				if ((option.flag & 4) != 0)
				{
					result = 2;
				}
				if ((option.flag & 8) != 0)
				{
					result = 3;
				}
			}
			return result;
		}
	}
}
