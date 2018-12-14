namespace Amazon.S3.Model
{
	public class GetObjectTorrentResponse : StreamResponse
	{
		private RequestCharged requestCharged;

		public RequestCharged RequestCharged
		{
			get
			{
				return requestCharged;
			}
			set
			{
				requestCharged = value;
			}
		}

		internal bool IsSetRequestCharged()
		{
			return requestCharged != null;
		}
	}
}
