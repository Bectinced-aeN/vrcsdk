using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListObjectsByTagsResponse : AmazonWebServiceResponse
	{
		private string bucketName;

		private string continuationToken;

		private string nextContinuationToken;

		private int? maxKeys;

		private bool isTruncated;

		private EncodingType encodingType;

		private List<TaggedResource> contents = new List<TaggedResource>();

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
				return continuationToken;
			}
			set
			{
				continuationToken = value;
			}
		}

		public string NextContinuationToken
		{
			get
			{
				return nextContinuationToken;
			}
			set
			{
				nextContinuationToken = value;
			}
		}

		public int? MaxKeys
		{
			get
			{
				return maxKeys;
			}
			set
			{
				maxKeys = value;
			}
		}

		public bool IsTruncated
		{
			get
			{
				return isTruncated;
			}
			set
			{
				isTruncated = value;
			}
		}

		public EncodingType EncodingType
		{
			get
			{
				return encodingType;
			}
			set
			{
				encodingType = value;
			}
		}

		public List<TaggedResource> Contents
		{
			get
			{
				return contents;
			}
			set
			{
				contents = value;
			}
		}

		public ListObjectsByTagsResponse()
			: this()
		{
		}
	}
}
