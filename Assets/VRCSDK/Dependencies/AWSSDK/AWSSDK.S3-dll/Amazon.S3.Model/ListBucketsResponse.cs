using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListBucketsResponse : AmazonWebServiceResponse
	{
		private List<S3Bucket> buckets = new List<S3Bucket>();

		private Owner owner;

		public List<S3Bucket> Buckets
		{
			get
			{
				return buckets;
			}
			set
			{
				buckets = value;
			}
		}

		public Owner Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
			}
		}

		internal bool IsSetBuckets()
		{
			return buckets.Count > 0;
		}

		internal bool IsSetOwner()
		{
			return owner != null;
		}

		public ListBucketsResponse()
			: this()
		{
		}
	}
}
