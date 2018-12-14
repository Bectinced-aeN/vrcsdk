using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutBucketReplicationRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private ReplicationConfiguration configuration;

		public string BucketName
		{
			get
			{
				return bucketName;
			}
			set
			{
				bucketName = value;
			}
		}

		public ReplicationConfiguration Configuration
		{
			get
			{
				return configuration;
			}
			set
			{
				configuration = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetConfiguration()
		{
			return configuration != null;
		}

		public PutBucketReplicationRequest()
			: this()
		{
		}
	}
}
