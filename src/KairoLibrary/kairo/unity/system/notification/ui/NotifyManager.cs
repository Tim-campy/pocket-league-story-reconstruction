using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using kairo.unity.native;
using kairo.unity.util;
using UnityEngine;

namespace kairo.unity.system.notification.ui
{
	public class NotifyManager
	{
		public enum FilterLevel
		{
			None,
			Important,
			Middle,
			All
		}

		protected static NotifyManager instance_;

		protected List<string> values_;

		protected bool pushEnable_;

		private FilterLevel filterLevel_ = FilterLevel.All;

		private const string NOTIFY_DATA_PREFS = "_notification_data";

		protected NotifyManager()
		{
			values_ = new List<string>();
			string[] array = PlayerPrefs.GetString("_notification_data", "").Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string item in array)
			{
				values_.Add(item);
			}
			filterLevel_ = (FilterLevel)KairoPlugin.GetNotificationFilter();
		}

		public static NotifyManager GetInstance()
		{
			return instance_;
		}

		public void SendString(long delay, string text, string customField = "")
		{
			SendString(0, delay, text, customField);
		}

		public void SendString(int id, long delay, string text, string customField = "")
		{
			if (!Regex.IsMatch(customField, "\\b@level\\s"))
			{
				customField = "@level 0 " + customField;
			}
			customField = "@id " + id + " " + customField;
			KairoPlugin.SendNotification(delay, text, customField);
		}

		public void SendText(long delay, string text, string customField = "")
		{
			SendText(0, delay, text, customField);
		}

		public void SendText(int id, long delay, string text, string customField = "")
		{
			customField = "@id " + id + " @multiline on " + customField;
			SendString(delay, text, customField);
		}

		public void SendPushString(string friendId, string text, string customField = "")
		{
			SendPushString(100, friendId, text, customField);
		}

		public void SendPushString(int id, string friendId, string text, string customField = "")
		{
			if (!Regex.IsMatch(customField, "\\b@level\\s"))
			{
				customField = string.Concat("@level 1 ", customField);
			}
			KairoBase instance = KairoBase.GetInstance();
			if (instance != null)
			{
				instance.ConnectNotificationSend(friendId, string.Concat("@id ", id, "@text ", text, " ", customField), (id == -1) ? null : id.ToString());
			}
		}

		public void SendPushText(string friendId, string text, string customField = "")
		{
			SendPushText(100, friendId, text, customField);
		}

		public void SendPushText(int id, string friendId, string text, string customField = "")
		{
			customField = "@id " + id + " @multiline on " + customField;
			SendPushString(friendId, text, customField);
		}

		internal void DoNotify()
		{
			KairoBase instance = KairoBase.GetInstance();
			if (instance != null && instance.IsGamePlaying())
			{
				KairoPlugin.CancelNotifications(false);
				OnNotify();
			}
		}

		public virtual void OnNotify()
		{
		}

		public virtual void OnReceiveValue()
		{
		}

		internal void DoCancel()
		{
			KairoBase instance = KairoBase.GetInstance();
			if (instance != null && instance.IsGamePlaying())
			{
				KairoPlugin.CancelNotifications();
			}
			else
			{
				KairoPlugin.CancelNotifications(true, false);
			}
		}

		public int GetValuesNum()
		{
			return values_.Count;
		}

		public string GetValue(int index)
		{
			if (index < 0 || values_.Count <= index)
			{
				return null;
			}
			return values_[index];
		}

		public void RemoveValues(params int[] indicies)
		{
			bool[] array = new bool[values_.Count];
			foreach (int num in indicies)
			{
				if (0 <= num && num < array.Length)
				{
					array[num] = true;
				}
			}
			for (int num2 = array.Length - 1; num2 >= 0; num2--)
			{
				if (array[num2])
				{
					values_.RemoveAt(num2);
				}
			}
			SaveValues();
		}

		public void ClearValues()
		{
			values_.Clear();
			SaveValues();
		}

		internal void UpdateValues()
		{
			KairoBase instance = KairoBase.GetInstance();
			if (instance == null || !instance.IsStart())
			{
				return;
			}
			try
			{
				string[] notificationData = KairoPlugin.GetNotificationData();
				if (notificationData.Length == 0) { return; }
string[] array = notificationData;
foreach (string item in array)
					{
						values_.Add(item);
					}
SaveValues();
KairoPlugin.RemoveNotificationData();
OnReceiveValue();
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
		}

		internal void SaveValues()
		{
			try
			{
				string text = "";
				foreach (string item in values_)
				{
					if (text.Length > 0)
					{
						text += "\n";
					}
					text += item;
				}
				PlayerPrefs.SetString("_notification_data", text);
			}
			catch (Exception t)
			{
				Log.Info(t);
			}
		}

		public FilterLevel GetFilterLevel()
		{
			return filterLevel_;
		}

		public void SetFilterLevel(FilterLevel level)
		{
			filterLevel_ = level;
			KairoPlugin.SetNotificationFilter((int)level);
		}

		public void SetPushEnable(bool enable)
		{
			pushEnable_ = enable;
		}

		public bool IsPushEnable()
		{
			return pushEnable_;
		}
	}
}
