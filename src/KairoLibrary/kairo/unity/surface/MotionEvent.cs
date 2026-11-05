namespace kairo.unity.surface
{
	public class MotionEvent
	{
		public const int ACTION_DOWN = 0;

		public const int ACTION_UP = 1;

		public const int ACTION_MOVE = 2;

		public const int ACTION_CANCEL = 3;

		public const int ACTION_OUTSIDE = 4;

		public const int ACTION_POINTER_DOWN = 5;

		public const int ACTION_POINTER_UP = 6;

		public const int ACTION_POINTER_1_DOWN = 5;

		public const int ACTION_POINTER_2_DOWN = 261;

		public const int ACTION_POINTER_3_DOWN = 517;

		public const int ACTION_POINTER_1_UP = 6;

		public const int ACTION_POINTER_2_UP = 262;

		public const int ACTION_POINTER_3_UP = 518;

		protected int action;

		protected int[] pointerIds;

		protected float[] pointerPos;

		protected float[] pointerDelta;

		protected bool isMousePointer;

		public MotionEvent(int action, int[] pointerIds, float[] pointerPos, float[] pointerDelta, bool isMousePointer = false)
		{
			this.action = action;
			this.pointerIds = pointerIds;
			this.pointerPos = pointerPos;
			this.pointerDelta = pointerDelta;
			this.isMousePointer = isMousePointer;
		}

		public int GetAction()
		{
			return action;
		}

		public float GetX()
		{
			return pointerPos[0];
		}

		public float GetY()
		{
			return pointerPos[1];
		}

		public float GetDeltaX()
		{
			if (pointerDelta == null)
			{
				return 0f;
			}
			return pointerDelta[0];
		}

		public float GetDeltaY()
		{
			if (pointerDelta == null)
			{
				return 0f;
			}
			return pointerDelta[1];
		}

		public float GetDeltaTime()
		{
			if (pointerDelta == null)
			{
				return 0f;
			}
			return pointerDelta[2];
		}

		public float GetX(int index)
		{
			return pointerPos[index * 2];
		}

		public float GetY(int index)
		{
			return pointerPos[index * 2 + 1];
		}

		public float GetDeltaX(int index)
		{
			if (pointerDelta == null)
			{
				return 0f;
			}
			return pointerDelta[index * 3 + 0];
		}

		public float GetDeltaY(int index)
		{
			if (pointerDelta == null)
			{
				return 0f;
			}
			return pointerDelta[index * 3 + 1];
		}

		public float GetDeltaTime(int index)
		{
			if (pointerDelta == null)
			{
				return 0f;
			}
			return pointerDelta[index * 3 + 2];
		}

		public bool IsMousePointer()
		{
			return isMousePointer;
		}

		public int GetPointerCount()
		{
			return pointerIds.Length;
		}

		public int GetPointerId(int index)
		{
			return pointerIds[index];
		}
	}
}
