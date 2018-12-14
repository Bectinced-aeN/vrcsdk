using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutBucketWebsiteRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private WebsiteConfiguration websiteConfiguration;

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

		public WebsiteConfiguration WebsiteConfiguration
		{
			get
			{
				return websiteConfiguration;
			}
			set
			{
				websiteConfiguration = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetWebsiteConfiguration()
		{
			return websiteConfiguration != null;
		}

		public PutBucketWebsiteRequest()
			: this()
		{
		}
	}
}
