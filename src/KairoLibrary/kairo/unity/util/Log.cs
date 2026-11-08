using System;
using java.lang;
using kairo.common.cfg;
using UnityEngine;

namespace kairo.unity.util
{
	public class Log
	{
		protected static LogListener listener_;

		private static string lastLog_;

		private Log()
		{
		}

		public static void Print(string text)
		{
			if (Config.PRINT)
			{
				DebugLog(text);
			}
		}

		public static void Println(string text)
		{
			if (Config.PRINT)
			{
				DebugLog(text);
			}
		}

		public static string Info(string text, Exception t = null)
		{
			text = GetThrowableLog(text, t);
			if (Config.PRINT)
			{
				if (text != null)
				{
					DebugLog(text);
				}
				if (t != null)
				{
					Debug.Log(t);
				}
			}
			if (text == null)
			{
				return "";
			}
			return text;
		}

		public static string Info(Exception t)
		{
			if (Config.PRINT && t != null)
			{
				Debug.Log(t);
			}
			return "";
		}

		public static string Error(string text, Exception t = null)
		{
			text = GetThrowableLog(text, t);
			if (Config.PRINT)
			{
				if (text != null)
				{
					DebugLog(text);
				}
				if (t != null)
				{
					Debug.Log(t);
				}
			}
			if (listener_ != null)
			{
				listener_.OnRecvLog(text);
			}
			if (text == null)
			{
				return "";
			}
			return text;
		}

		public static string Error(Exception t)
		{
			if (Config.PRINT && t != null)
			{
				Debug.Log(t);
			}
			if (listener_ != null)
			{
				listener_.OnRecvLog(t.ToString());
			}
			return "";
		}

		private static void DebugLog(string text)
		{
			if (text == lastLog_)
			{
				text += " ";
			}
			Debug.Log(text);
			lastLog_ = text;
		}

		public static void SetListener(LogListener listener)
		{
			listener_ = listener;
		}

		protected static string GetThrowableName(Exception t)
		{
			const string recoveredPdbConstString = "";
			if (t == null)
			{
				return "";
			}
			Type type = t.GetType();
			string text = type.Name;
			try
			{
				int num = text.LastIndexOf('.');
				if (num != -1)
				{
					text = text.Substring(num + 1);
				}
				if (text.EndsWith("Exception"))
				{
					text = text.Substring(0, text.Length() - "Exception".Length());
				}
				if (text.EndsWith("Error"))
				{
					text = text.Substring(0, text.Length() - "Error".Length());
				}
				return text;
			}
			catch (Exception)
			{
				return type.Name;
			}
		}

		protected static string GetThrowableLog(string text, Exception t)
		{
			if (t != null)
			{
				string throwableName = GetThrowableName(t);
				if (text == null) { text = throwableName; } else { text = (throwableName + ":" + text); }
			}
			return text;
		}
	}
}
