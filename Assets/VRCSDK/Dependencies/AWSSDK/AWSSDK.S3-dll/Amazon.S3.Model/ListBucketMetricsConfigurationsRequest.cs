using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class ListBucketMetricsConfigurationsRequest : AmazonWebServiceRequest
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

		internal bool IsSetBucketName()
		{
			return !string.IsNullOrEmpty(bucketName);
		}

		internal bool IsSetContinuationToken()
		{
			return !string.IsNullOrEmpty(ContinuationToken);
		}

		public ListBucketMetricsConfigurationsRequest()
			: this()
		{
		}
	}
}
