using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class RestoreObjectResponse : AmazonWebServiceResponse
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

		public RestoreObjectResponse()
			: this()
		{
		}
	}
}
