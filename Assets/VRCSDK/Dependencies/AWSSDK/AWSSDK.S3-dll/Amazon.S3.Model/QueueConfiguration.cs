namespace Amazon.S3.Model
{
	public class QueueConfiguration : NotificationConfiguration
	{
		public string Id
		{
			get;
			set;
		}

		public string Queue
		{
			get;
			set;
		}

		internal bool IsSetId()
		{
			return Id != null;
		}

		internal bool IsSetQueue()
		{
			return Queue != null;
		}
	}
}
