using System;

namespace kairo.unity.util
{
	public class NotPermitException : KairoException
	{
		public enum PermissionType
		{
			Storage
		}

		private PermissionType type_;

		public PermissionType Type => type_;

		public NotPermitException(PermissionType type, string message, string displayMessage, Exception exception)
			: base(message, displayMessage, exception)
		{
			type_ = type;
		}

		public NotPermitException(PermissionType type, string message, Exception exception)
			: base(message, "", exception)
		{
			type_ = type;
			if (type == PermissionType.Storage)
			{
				displayMessage_ = "権限：ストレージを許可して起動し直してください。<br>※ゲームの保存に必要となります。";
			}
			else
			{
				displayMessage_ = "アプリの設定画面から「アプリの権限」をすべてオンにして再起動して下さい。";
			}
			displayMessage_ = Language.LT(displayMessage_) + "<br>" + message;
		}

		public override string ToString()
		{
			if (base.InnerException != null)
			{
				return base.InnerException.ToString();
			}
			return base.ToString();
		}
	}
}
