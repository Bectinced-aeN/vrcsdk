using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class ListVersionsRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string delimiter;

		private string keyMarker;

		private int? maxKeys;

		private string prefix;

		private string versionIdMarker;

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

		public int MaxKeys
		{
			get
			{
				return maxKeys ?? 0;
			}
			set
			{
				maxKeys = value;
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

		public string VersionIdMarker
		{
			get
			{
				return versionIdMarker;
			}
			set
			{
				versionIdMarker = value;
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

		internal bool IsSetMaxKeys()
		{
			return maxKeys.HasValue;
		}

		internal bool IsSetPrefix()
		{
			return prefix != null;
		}

		internal bool IsSetVersionIdMarker()
		{
			return versionIdMarker != null;
		}

		internal bool IsSetEncoding()
		{
			return encoding != null;
		}

		public ListVersionsRequest()
			: this()
		{
		}
	}
}
