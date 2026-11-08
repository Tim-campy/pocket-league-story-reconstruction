using System;
namespace kairo.unity.surface
{
	public class TouchEvent
	{
		public const int TYPE_DOWN = 0;

		public const int TYPE_UP = 1;

		public const int TYPE_ENTER = 2;

		public const int TYPE_LEAVE = 3;

		public const int TYPE_MOVE = 4;

		public const int TYPE_CLICK = 5;

		public const int TYPE_LOST = 6;

		public const int TYPE_DRAG = 7;

		public const int TYPE_GRID_DRAG = 8;

		public const int TYPE_CELL_DRAG = 9;

		public const int TYPE_CANCEL = 10;

		public const int TYPE_PINCH_START = 11;

		public const int TYPE_PINCH = 12;

		public const int TYPE_ON = 13;

		public int type;

		public int x;

		public int y;

		public int dx;

		public int dy;

		public float speedX;

		public float speedY;

		public int pinch;

		public int[][] pinchPointBegin_;

		public int[][] pinchPointNow_;

		public TouchComponent component;

		public TouchEvent()
		{
			Init();
		}

		public TouchEvent(int type, TouchComponent component)
		{
			Init();
			this.type = type;
			this.component = component;
		}

		public void Init()
		{
			this.component = null;
			this.type = 0;
			this.x = 0;
			this.y = 0;
			this.dx = 0;
			this.dy = 0;
			this.speedX = 0f;
			this.speedY = 0f;
			this.pinch = 0;
			if (this.pinchPointBegin_ != null)
			{
				for (int i = 0; i < (int)this.pinchPointBegin_.Length; i++)
				{
					for (int j = 0; j < (int)this.pinchPointBegin_[i].Length; j++)
					{
						this.pinchPointBegin_[i][j] = 0;
					}
				}
			}
			if (this.pinchPointNow_ != null)
			{
				for (int k = 0; k < (int)this.pinchPointNow_.Length; k++)
				{
					for (int l = 0; l < (int)this.pinchPointNow_[k].Length; l++)
					{
						this.pinchPointNow_[k][l] = 0;
					}
				}
			}
		}

		public override string ToString()
		{
			if (component == null)
			{
				return string.Concat(this, ":component=") ?? "";
			}
			string text = "";
			if (type == 0) {text="TYPE_DOWN";} else if (type == 1) {text="TYPE_UP";} else if (type == 2) {text="TYPE_ENTER";} else if (type == 3) {text="TYPE_LEAVE";} else if (type == 4) {text="TYPE_MOVE";} else if (type == 5) {text="TYPE_CLICK";} else if (type == 6) {text="TYPE_LOST";} else if (type == 7) {text="TYPE_DRAG";} else if (type == 8) {text="TYPE_GRID_DRAG";} else if (type == 9) {text="TYPE_CELL_DRAG";} else if (type == 10) {text="TYPE_CANCEL";} else if(type==13){text="TYPE_ON";} else{text="NONE["+type+"]";}
			return "TouchEvent:" + text + " (" + component.id + "," + component.value + ")";
		}

		public static bool IsPositive(int type)
		{
			switch (type)
			{
			case 0:
			case 2:
			case 4:
			case 7:
			case 8:
			case 9:
				return true;
			default:
				return false;
			}
		}

		public bool IsPositive()
		{
			return IsPositive(type);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
