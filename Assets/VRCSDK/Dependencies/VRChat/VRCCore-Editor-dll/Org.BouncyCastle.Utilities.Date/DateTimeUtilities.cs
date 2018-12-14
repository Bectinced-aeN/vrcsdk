using System;

namespace Org.BouncyCastle.Utilities.Date
{
	internal class DateTimeUtilities
	{
		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

		private DateTimeUtilities()
		{
		}

		public static long DateTimeToUnixMs(DateTime dateTime)
		{
			if (dateTime.CompareTo(UnixEpoch) < 0)
			{
				throw new ArgumentException("DateTime value may not be before the epoch", "dateTime");
			}
			return (dateTime.Ticks - UnixEpoch.Ticks) / 10000;
		}

		public static DateTime UnixMsToDateTime(long unixMs)
		{
			return new DateTime(unixMs * 10000 + UnixEpoch.Ticks);
		}

		public static long CurrentUnixMs()
		{
			return DateTimeToUnixMs(DateTime.UtcNow);
		}
	}
}
