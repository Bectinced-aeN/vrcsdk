using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetACLRequest : AmazonWebServiceRequest
	{
		public string BucketName
		{
			get;
			set;
		}

		public string Key
		{
			get;
			set;
		}

		public string VersionId
		{
			get;
			set;
		}

		internal bool IsSetBucket()
		{
			return BucketName != null;
		}

		internal bool IsSetKey()
		{
			return Key != null;
		}

		internal bool IsSetVersionId()
		{
			return VersionId != null;
		}

		public GetACLRequest()
			: this()
		{
		}
	}
}
