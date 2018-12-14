using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketAccelerateConfigurationResponse : AmazonWebServiceResponse
	{
		private BucketAccelerateStatus status;

		public BucketAccelerateStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;
			}
		}

		public GetBucketAccelerateConfigurationResponse()
			: this()
		{
		}
	}
}
