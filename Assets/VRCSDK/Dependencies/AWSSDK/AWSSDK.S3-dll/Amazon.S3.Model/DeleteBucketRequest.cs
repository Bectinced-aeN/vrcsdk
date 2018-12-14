using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class DeleteBucketRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private S3Region bucketRegion;

		private bool useClientRegion = true;

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

		public S3Region BucketRegion
		{
			get
			{
				return bucketRegion;
			}
			set
			{
				bucketRegion = value;
			}
		}

		public bool UseClientRegion
		{
			get
			{
				return useClientRegion;
			}
			set
			{
				useClientRegion = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetBucketRegion()
		{
			return bucketRegion != null;
		}

		public DeleteBucketRequest()
			: this()
		{
		}
	}
}
