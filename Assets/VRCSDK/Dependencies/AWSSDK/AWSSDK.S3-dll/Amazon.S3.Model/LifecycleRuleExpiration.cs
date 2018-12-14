using System;

namespace Amazon.S3.Model
{
	public class LifecycleRuleExpiration
	{
		private DateTime? date;

		private int? days;

		private bool? expiredObjectDeleteMarker;

		public DateTime Date
		{
			get
			{
				return date ?? default(DateTime);
			}
			set
			{
				date = value;
			}
		}

		public int Days
		{
			get
			{
				return days ?? 0;
			}
			set
			{
				days = value;
			}
		}

		public bool ExpiredObjectDeleteMarker
		{
			get
			{
				return expiredObjectDeleteMarker.GetValueOrDefault();
			}
			set
			{
				expiredObjectDeleteMarker = value;
			}
		}

		internal bool IsSetDate()
		{
			return date.HasValue;
		}

		internal bool IsSetDays()
		{
			return days.HasValue;
		}

		internal bool IsSetExpiredObjectDeleteMarker()
		{
			return expiredObjectDeleteMarker.HasValue;
		}
	}
}
