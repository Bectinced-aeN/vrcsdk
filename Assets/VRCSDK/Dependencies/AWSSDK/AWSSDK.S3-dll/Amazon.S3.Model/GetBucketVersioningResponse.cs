using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketVersioningResponse : AmazonWebServiceResponse
	{
		private S3BucketVersioningConfig config;

		public S3BucketVersioningConfig VersioningConfig
		{
			get
			{
				if (config == null)
				{
					config = new S3BucketVersioningConfig();
				}
				return config;
			}
			set
			{
				config = value;
			}
		}

		public GetBucketVersioningResponse()
			: this()
		{
		}
	}
}
