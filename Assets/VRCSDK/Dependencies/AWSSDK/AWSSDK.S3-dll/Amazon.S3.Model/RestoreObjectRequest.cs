using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class RestoreObjectRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string key;

		private int? days;

		private string versionId;

		private RequestPayer requestPayer;

		private GlacierJobTier tier;

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

		public string Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		public int Days
		{
			get
			{
				return days ?? 0;
			}
			set
			{
				days = value;
			}
		}

		public string VersionId
		{
			get
			{
				return versionId;
			}
			set
			{
				versionId = value;
			}
		}

		public RequestPayer RequestPayer
		{
			get
			{
				return requestPayer;
			}
			set
			{
				requestPayer = value;
			}
		}

		public GlacierJobTier Tier
		{
			get
			{
				return tier;
			}
			set
			{
				tier = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetDays()
		{
			return days.HasValue;
		}

		internal bool IsSetVersionId()
		{
			return versionId != null;
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		internal bool IsSetTier()
		{
			return tier != null;
		}

		public RestoreObjectRequest()
			: this()
		{
		}
	}
}
