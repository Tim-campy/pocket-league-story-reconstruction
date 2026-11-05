using UnityEngine;

namespace kairo.unity.ui
{
	public class KeyEvent
	{
		public const KeyCode KEYCODE_HOME = KeyCode.Home;

		public const KeyCode KEYCODE_BACK = KeyCode.Escape;

		public const KeyCode KEYCODE_FOCUS = KeyCode.LeftWindows;

		public const int ACTION_DOWN = 0;

		public const int ACTION_UP = 1;

		private int action_;

		private KeyCode keyCode_;

		public KeyEvent(int action, KeyCode keyCode)
		{
			action_ = action;
			keyCode_ = keyCode;
		}

		public int GetAction()
		{
			return action_;
		}

		public KeyCode GetKeyCode()
		{
			return keyCode_;
		}

		public int GetRepeatCount()
		{
			return 0;
		}
	}
}
