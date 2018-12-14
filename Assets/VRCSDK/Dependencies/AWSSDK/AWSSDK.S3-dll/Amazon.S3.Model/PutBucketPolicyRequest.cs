using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutBucketPolicyRequest : AmazonWebServiceRequest
	{
		public string BucketName
		{
			get;
			set;
		}

		public string ContentMD5
		{
			get;
			set;
		}

		public string Policy
		{
			get;
			set;
		}

		protected override bool IncludeSHA256Header => false;

		internal bool IsSetBucket()
		{
			return BucketName != null;
		}

		internal bool IsSetContentMD5()
		{
			return ContentMD5 != null;
		}

		internal bool IsSetPolicy()
		{
			return Policy != null;
		}

		public PutBucketPolicyRequest()
			: this()
		{
		}
	}
}
