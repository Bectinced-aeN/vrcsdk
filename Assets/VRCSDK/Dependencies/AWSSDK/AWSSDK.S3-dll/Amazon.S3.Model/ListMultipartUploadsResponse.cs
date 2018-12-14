using Amazon.Runtime;
using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ListMultipartUploadsResponse : AmazonWebServiceResponse
	{
		private string bucketName;

		private string keyMarker;

		private string uploadIdMarker;

		private string nextKeyMarker;

		private string nextUploadIdMarker;

		private int? maxUploads;

		private bool? isTruncated;

		private List<MultipartUpload> multipartUploads;

		private string delimiter;

		private List<string> commonPrefixes;

		private string prefix;

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

		public string KeyMarker
		{
			get
			{
				return keyMarker;
			}
			set
			{
				keyMarker = value;
			}
		}

		public string UploadIdMarker
		{
			get
			{
				return uploadIdMarker;
			}
			set
			{
				uploadIdMarker = value;
			}
		}

		public string NextKeyMarker
		{
			get
			{
				return nextKeyMarker;
			}
			set
			{
				nextKeyMarker = value;
			}
		}

		public string NextUploadIdMarker
		{
			get
			{
				return nextUploadIdMarker;
			}
			set
			{
				nextUploadIdMarker = value;
			}
		}

		public int MaxUploads
		{
			get
			{
				return maxUploads ?? 0;
			}
			set
			{
				maxUploads = value;
			}
		}

		public bool IsTruncated
		{
			get
			{
				return isTruncated ?? false;
			}
			set
			{
				isTruncated = value;
			}
		}

		public List<MultipartUpload> MultipartUploads
		{
			get
			{
				if (multipartUploads == null)
				{
					multipartUploads = new List<MultipartUpload>();
				}
				return multipartUploads;
			}
			set
			{
				multipartUploads = value;
			}
		}

		public string Prefix
		{
			get
			{
				return prefix;
			}
			set
			{
				prefix = value;
			}
		}

		public string Delimiter
		{
			get
			{
				return delimiter;
			}
			set
			{
				delimiter = value;
			}
		}

		public List<string> CommonPrefixes
		{
			get
			{
				if (commonPrefixes == null)
				{
					commonPrefixes = new List<string>();
				}
				return commonPrefixes;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetKeyMarker()
		{
			return keyMarker != null;
		}

		internal bool IsSetUploadIdMarker()
		{
			return uploadIdMarker != null;
		}

		internal bool IsSetNextKeyMarker()
		{
			return nextKeyMarker != null;
		}

		internal bool IsSetNextUploadIdMarker()
		{
			return nextUploadIdMarker != null;
		}

		internal bool IsSetMaxUploads()
		{
			return maxUploads.HasValue;
		}

		internal bool IsSetIsTruncated()
		{
			return isTruncated.HasValue;
		}

		public ListMultipartUploadsResponse()
			: this()
		{
		}
	}
}
