using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetBucketWebsiteResponse : AmazonWebServiceResponse
	{
		private WebsiteConfiguration websiteConfiguration;

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

		public GetBucketWebsiteResponse()
			: this()
		{
		}
	}
}
