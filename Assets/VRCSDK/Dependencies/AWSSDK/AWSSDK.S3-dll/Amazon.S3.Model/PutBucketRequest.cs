namespace Amazon.S3.Model
{
	public class PutBucketRequest : PutWithACLRequest
	{
		private string bucketName;

		private S3Region bucketRegion;

		private string bucketRegionName;

		private bool useClientRegion = true;

		private S3CannedACL cannedAcl;

		public S3CannedACL CannedACL
		{
			get
			{
				return cannedAcl;
			}
			set
			{
				cannedAcl = value;
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

		public string BucketRegionName
		{
			get
			{
				return bucketRegionName;
			}
			set
			{
				bucketRegionName = value;
			}
		}

		internal bool IsSetCannedACL()
		{
			if (cannedAcl != null)
			{
				return cannedAcl != S3CannedACL.NoACL;
			}
			return false;
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetBucketRegion()
		{
			return bucketRegion != null;
		}

		internal bool IsSetBucketRegionName()
		{
			return !string.IsNullOrEmpty(bucketRegionName);
		}
	}
}
