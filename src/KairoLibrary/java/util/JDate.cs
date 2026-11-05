using System;

namespace java.util
{
	public class JDate
	{
		public static DateTime UNIX_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		protected DateTime date_;

		public JDate()
		{
			date_ = DateTime.UtcNow;
		}

		public JDate(long date)
		{
			SetTime(date);
		}

		public JDate(DateTime date)
		{
			SetTime(date);
		}

		public override bool Equals(object obj)
		{
			if (obj is JDate)
			{
				return (long)date_.TimeOfDay.TotalMilliseconds == (long)((JDate)obj).date_.TimeOfDay.TotalMilliseconds;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public long GetTime()
		{
			return (long)(date_ - UNIX_TIME).TotalMilliseconds;
		}

		public void SetTime(long time)
		{
			date_ = new DateTime(UNIX_TIME.Year, UNIX_TIME.Month, UNIX_TIME.Day, UNIX_TIME.Hour, UNIX_TIME.Minute, UNIX_TIME.Second, UNIX_TIME.Kind);
			date_ = date_.AddMilliseconds(time);
		}

		public void SetTime(DateTime date)
		{
			date_ = date.ToUniversalTime();
		}

		public DateTime GetDateTime()
		{
			return date_;
		}

		public JDate Clone()
		{
			return new JDate
			{
				date_ = date_
			};
		}
	}
}
