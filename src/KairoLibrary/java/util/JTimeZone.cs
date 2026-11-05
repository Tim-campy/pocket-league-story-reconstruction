using System;
using java.native;

namespace java.util
{
	public class JTimeZone
	{
		private string id_;

		private long utcOffset_;

		private TimeZoneInfo timezone_;

		private JTimeZone(string id)
		{
			id_ = id;
			utcOffset_ = JavaPlugin.GetUtcOffset(id_);
			int milliseconds = (int)(utcOffset_ % 1000);
			utcOffset_ /= 1000L;
			int seconds = (int)(utcOffset_ % 60);
			utcOffset_ /= 60L;
			int minutes = (int)(utcOffset_ % 60);
			utcOffset_ /= 60L;
			int hours = (int)(utcOffset_ % 24);
			utcOffset_ /= 24L;
			int days = (int)utcOffset_;
			timezone_ = TimeZoneInfo.CreateCustomTimeZone(id_, new TimeSpan(days, hours, minutes, seconds, milliseconds), id_, id_);
		}

		public static JTimeZone GetDefault()
		{
			return new JTimeZone(JavaPlugin.GetDefaultTimeZone());
		}

		public string GetID()
		{
			return id_;
		}

		public static JTimeZone GetTimeZone(string ID)
		{
			try
			{
				return new JTimeZone(ID);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public JTimeZone Clone()
		{
			return new JTimeZone(id_);
		}

		public TimeZoneInfo GetTimeZoneInfo()
		{
			return timezone_;
		}
	}
}
