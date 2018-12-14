using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketReplicationResponse : AmazonWebServiceResponse
	{
		private ReplicationConfiguration configuration;

		public ReplicationConfiguration Configuration
		{
			get
			{
				if (configuration == null)
				{
					configuration = new ReplicationConfiguration();
				}
				return configuration;
			}
			set
			{
				configuration = value;
			}
		}

		public GetBucketReplicationResponse()
			: this()
		{
		}
	}
}
