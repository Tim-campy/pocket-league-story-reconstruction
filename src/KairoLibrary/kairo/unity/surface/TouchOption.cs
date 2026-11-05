using System;
using kairo.unity.util;

namespace kairo.unity.surface
{
	public class TouchOption
	{
		public const int FLAG_THROUGH = 1;

		public const int FLAG_MIN_PRIORITY = 2;

		public const int FLAG_MAX_PRIORITY = 4;

		public const int FLAG_TOP_PRIORITY = 8;

		public const int FLAG_MARKER = 16;

		public const int FLAG_CANCEL_SCREEN_LIMIT = 32;

		public const int FLAG_CANCEL_COMPONENT = 64;

		public const int FLAG_TOUCH_RECT = 128;

		public const int FLAG_TOUCH_F_RECT = 256;

		public const int FLAG_SCROLL_BAR_ADD_MARGIN = 512;

		public const int FLAG_SAME_ID = 1024;

		public const int FLAG_TOUCH_LIGHT = 2048;

		public int flag;

		public float dragGridW;

		public float dragGridH;

		public int[] dragCell;

		public int[] args;

		public int[] margin;

		public int collisionBits;

		public int[] extendValues;

		public int[] keys;

		private int[] keysBuf_ = new int[1];

		internal bool Lock;

		internal int hasCount;

		public static FastVector<TouchOption> buffer = new FastVector<TouchOption>();

		public TouchOption()
		{
			Init();
		}

		public TouchOption(TouchOption copy)
		{
			Init();
			flag = copy.flag;
			dragGridW = copy.dragGridW;
			dragGridH = copy.dragGridH;
			collisionBits = copy.collisionBits;
			Lock = true;
			if (copy.dragCell != null)
			{
				dragCell = new int[copy.dragCell.Length];
				Array.Copy(copy.dragCell, dragCell, dragCell.Length);
			}
			if (copy.args != null)
			{
				args = new int[copy.args.Length];
				Array.Copy(copy.args, args, args.Length);
			}
			if (copy.margin != null)
			{
				margin = new int[copy.margin.Length];
				Array.Copy(copy.margin, margin, margin.Length);
			}
			if (copy.extendValues != null)
			{
				extendValues = new int[copy.extendValues.Length];
				Array.Copy(copy.extendValues, extendValues, extendValues.Length);
			}
			if (copy.keys != null)
			{
				keys = new int[copy.keys.Length];
				Array.Copy(copy.keys, keys, keys.Length);
			}
		}

		public void Init()
		{
			flag = 0;
			dragGridW = 0f;
			dragGridH = 0f;
			dragCell = null;
			args = null;
			margin = null;
			collisionBits = 0;
			extendValues = null;
			keys = null;
			Lock = false;
			hasCount = 0;
		}

		public static TouchOption Create()
		{
			TouchOption touchOption = null;
			for (int i = 0; i < buffer.Size(); i++)
			{
				if (!buffer.ElementAt(i).Lock)
				{
					touchOption = buffer.ElementAt(i);
					touchOption.Init();
					break;
				}
			}
			if (touchOption == null)
			{
				buffer.Add(touchOption = new TouchOption());
			}
			touchOption.Lock = true;
			return touchOption;
		}

		public static TouchOption Create(int[] args)
		{
			TouchOption touchOption = Create();
			touchOption.args = args;
			return touchOption;
		}

		public static TouchOption Create(int[] args, int flag)
		{
			TouchOption touchOption = Create();
			touchOption.args = args;
			touchOption.flag = flag;
			return touchOption;
		}

		public static TouchOption Create(int flag)
		{
			TouchOption touchOption = Create();
			touchOption.flag = flag;
			return touchOption;
		}

		public static TouchOption Drag(int gridX, int gridY)
		{
			return Drag(gridX, gridY, 0);
		}

		public static TouchOption Drag(int gridX, int gridY, int flag)
		{
			TouchOption touchOption = Create();
			touchOption.dragGridW = gridX;
			touchOption.dragGridH = gridY;
			touchOption.flag = flag;
			return touchOption;
		}

		public static TouchOption Drag(int x, int y, int w, int h, int xcells, int ycells)
		{
			return Drag(x, y, w, h, xcells, ycells, 0);
		}

		public static TouchOption Drag(int x, int y, int w, int h, int xcells, int ycells, int flag)
		{
			TouchOption touchOption = Create();
			touchOption.dragCell = new int[6] { x, y, w, h, xcells, ycells };
			touchOption.flag = flag;
			return touchOption;
		}

		public TouchOption Arg(params int[] args)
		{
			this.args = args;
			return this;
		}

		public TouchOption Flag(int flag)
		{
			this.flag = flag;
			return this;
		}

		public TouchOption Margin(int left, int right, int top, int bottom)
		{
			margin = new int[4] { left, right, top, bottom };
			return this;
		}

		public TouchOption Collision(int bits)
		{
			collisionBits |= bits;
			return this;
		}

		public TouchOption ExtendValues(int[] values)
		{
			extendValues = values;
			return this;
		}

		public TouchOption ExtendValues(int value)
		{
			extendValues = new int[1] { value };
			return this;
		}

		public TouchOption Keys(params int[] keys)
		{
			this.keys = keys;
			return this;
		}

		public TouchOption Keys(int key)
		{
			keysBuf_[0] = key;
			keys = keysBuf_;
			return this;
		}

		public void Dispose()
		{
			dragCell = null;
			args = null;
			margin = null;
		}
	}
}
