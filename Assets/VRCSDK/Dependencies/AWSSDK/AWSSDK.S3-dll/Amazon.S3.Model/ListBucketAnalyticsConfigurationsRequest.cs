using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class ListBucketAnalyticsConfigurationsRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string token;

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

		public string ContinuationToken
		{
			get
			{
				return token;
			}
			set
			{
				token = value;
			}
		}

		internal bool IsSetBucket()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetContinuationToken()
		{
			return !string.IsNullOrEmpty(ContinuationToken);
		}

		public ListBucketAnalyticsConfigurationsRequest()
			: this()
		{
		}
	}
}
