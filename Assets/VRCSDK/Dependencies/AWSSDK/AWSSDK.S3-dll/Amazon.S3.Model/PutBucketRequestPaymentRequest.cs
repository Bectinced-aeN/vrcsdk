using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutBucketRequestPaymentRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private RequestPaymentConfiguration requestPaymentConfiguration;

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

		public RequestPaymentConfiguration RequestPaymentConfiguration
		{
			get
			{
				return requestPaymentConfiguration;
			}
			set
			{
				requestPaymentConfiguration = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetRequestPaymentConfiguration()
		{
			return requestPaymentConfiguration != null;
		}

		public PutBucketRequestPaymentRequest()
			: this()
		{
		}
	}
}
