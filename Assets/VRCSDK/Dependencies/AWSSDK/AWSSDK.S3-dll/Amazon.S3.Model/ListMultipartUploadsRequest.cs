using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class ListMultipartUploadsRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string delimiter;

		private string keyMarker;

		private int? maxUploads;

		private string prefix;

		private string uploadIdMarker;

		private EncodingType encoding;

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

		public EncodingType Encoding
		{
			get
			{
				return encoding;
			}
			set
			{
				encoding = value;
			}
		}

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetDelimiter()
		{
			return delimiter != null;
		}

		internal bool IsSetKeyMarker()
		{
			return keyMarker != null;
		}

		internal bool IsSetMaxUploads()
		{
			return maxUploads.HasValue;
		}

		internal bool IsSetPrefix()
		{
			return prefix != null;
		}

		internal bool IsSetUploadIdMarker()
		{
			return uploadIdMarker != null;
		}

		internal bool IsSetEncoding()
		{
			return encoding != null;
		}

		public ListMultipartUploadsRequest()
			: this()
		{
		}
	}
}
