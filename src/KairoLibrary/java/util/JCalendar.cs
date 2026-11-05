using System;

namespace java.util
{
	public class JCalendar
	{
		public const int YEAR = 1;

		public const int MONTH = 2;

		public const int DATE = 5;

		public const int DAY_OF_MONTH = 5;

		public const int DAY_OF_YEAR = 6;

		public const int DAY_OF_WEEK = 7;

		public const int AM_PM = 9;

		public const int HOUR = 10;

		public const int HOUR_OF_DAY = 11;

		public const int MINUTE = 12;

		public const int SECOND = 13;

		public const int MILLISECOND = 14;

		public const int SUNDAY = 1;

		public const int MONDAY = 2;

		public const int TUESDAY = 3;

		public const int WEDNESDAY = 4;

		public const int THURSDAY = 5;

		public const int FRIDAY = 6;

		public const int SATURDAY = 7;

		public const int JANUARY = 0;

		public const int FEBRUARY = 1;

		public const int MARCH = 2;

		public const int APRIL = 3;

		public const int MAY = 4;

		public const int JUNE = 5;

		public const int JULY = 6;

		public const int AUGUST = 7;

		public const int SEPTEMBER = 8;

		public const int OCTOBER = 9;

		public const int NOVEMBER = 10;

		public const int DECEMBER = 11;

		public const int AM = 0;

		public const int PM = 1;

		protected JDate date_;

		protected JTimeZone timezone_;

		protected JCalendar()
		{
			date_ = new JDate();
			timezone_ = JTimeZone.GetDefault();
		}

		protected JCalendar(JTimeZone zone)
		{
			date_ = new JDate();
			timezone_ = zone;
		}

		public static JCalendar GetInstance()
		{
			return new JCalendar();
		}

		public static JCalendar GetInstance(JTimeZone zone)
		{
			return new JCalendar(zone);
		}

		public bool After(object when)
		{
			if (when is JCalendar)
			{
				return ((JCalendar)when).GetTimeInMillis() < GetTimeInMillis();
			}
			return false;
		}

		public bool Before(object when)
		{
			if (when is JCalendar)
			{
				return GetTimeInMillis() < ((JCalendar)when).GetTimeInMillis();
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is JCalendar)
			{
				return ((JCalendar)obj).GetTimeInMillis() == GetTimeInMillis();
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Add(int field, int value)
		{
			Set(field, Get(field) + 1);
		}

		public int Get(int field)
		{
			TimeZoneInfo timeZoneInfo = timezone_.GetTimeZoneInfo();
			DateTime dateTime = new DateTime(date_.GetDateTime().ToUniversalTime().Ticks + timeZoneInfo.BaseUtcOffset.Ticks, DateTimeKind.Local);
			switch (field)
			{
			case 1:
				return dateTime.Year;
			case 2:
				return 0 + (dateTime.Month - 1);
			case 5:
				return dateTime.Day;
			case 7:
				return (int)(1 + (dateTime.DayOfWeek - 0));
			case 9:
				if (dateTime.Hour < 12)
				{
					return 0;
				}
				return 1;
			case 10:
				return dateTime.Hour % 12;
			case 11:
				return dateTime.Hour;
			case 12:
				return dateTime.Minute;
			case 13:
				return dateTime.Second;
			case 14:
				return dateTime.Millisecond;
			case 6:
				return dateTime.DayOfYear;
			default:
				throw new NotSupportedException();
			}
		}

		public void Set(int field, int value)
		{
			int num = Get(field);
			int num2 = value - num;
			TimeZoneInfo timeZoneInfo = timezone_.GetTimeZoneInfo();
			DateTime dateTime = new DateTime(date_.GetDateTime().ToUniversalTime().Ticks + timeZoneInfo.BaseUtcOffset.Ticks, DateTimeKind.Utc);
			switch (field)
			{
			case 1:
				dateTime = dateTime.AddYears(num2);
				break;
			case 2:
				dateTime = dateTime.AddMonths(num2);
				break;
			case 5:
				dateTime = dateTime.AddDays(num2);
				break;
			case 7:
				dateTime = dateTime.AddDays(num2);
				break;
			case 9:
				dateTime = dateTime.AddHours(num2 * 12);
				break;
			case 10:
				dateTime = dateTime.AddHours(num2);
				break;
			case 11:
				dateTime = dateTime.AddHours(num2);
				break;
			case 12:
				dateTime = dateTime.AddMinutes(num2);
				break;
			case 13:
				dateTime = dateTime.AddSeconds(num2);
				break;
			case 14:
				dateTime = dateTime.AddMilliseconds(num2);
				break;
			case 6:
				dateTime.AddDays(num2 - dateTime.DayOfYear);
				break;
			default:
				throw new NotSupportedException();
			}
			dateTime = new DateTime(dateTime.ToUniversalTime().Ticks - timeZoneInfo.BaseUtcOffset.Ticks, DateTimeKind.Utc);
			date_.SetTime((long)(TimeZoneInfo.ConvertTimeToUtc(dateTime) - JDate.UNIX_TIME).TotalMilliseconds);
		}

		public void Set(int year, int month, int date)
		{
			Set(1, year);
			Set(2, month);
			Set(5, date);
		}

		public void Set(int year, int month, int date, int hourOfDay, int minute)
		{
			Set(1, year);
			Set(2, month);
			Set(5, date);
			Set(11, hourOfDay);
			Set(12, minute);
		}

		public void Set(int year, int month, int date, int hourOfDay, int minute, int second)
		{
			Set(1, year);
			Set(2, month);
			Set(5, date);
			Set(11, hourOfDay);
			Set(12, minute);
			Set(13, second);
		}

		public void Clear()
		{
			TimeZoneInfo timeZoneInfo = timezone_.GetTimeZoneInfo();
			DateTime dateTime = new DateTime(timeZoneInfo.BaseUtcOffset.Ticks, DateTimeKind.Local);
			date_.SetTime((long)(TimeZoneInfo.ConvertTimeToUtc(dateTime) - JDate.UNIX_TIME).TotalMilliseconds);
		}

		public JDate GetTime()
		{
			return date_.Clone();
		}

		public void SetTime(JDate date)
		{
			date_ = date;
		}

		public JTimeZone GetTimeZone()
		{
			return timezone_.Clone();
		}

		public void SetTimeZone(JTimeZone value)
		{
			timezone_ = value;
		}

		public void SetTimeInMillis(long millis)
		{
			date_.SetTime(millis);
		}

		public long GetTimeInMillis()
		{
			return date_.GetTime();
		}
	}
}
