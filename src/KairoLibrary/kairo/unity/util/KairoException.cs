using System;

namespace kairo.unity.util
{
	public class KairoException : Exception
	{
		protected string displayMessage_;

		public string DisplayMessage => displayMessage_;

		public KairoException(string message, string displayMessage, Exception exception)
			: base(message, exception)
		{
			displayMessage_ = displayMessage;
		}
	}
}
