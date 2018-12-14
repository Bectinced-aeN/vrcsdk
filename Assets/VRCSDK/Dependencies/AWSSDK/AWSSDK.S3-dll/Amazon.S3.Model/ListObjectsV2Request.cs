using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class ListObjectsV2Request : AmazonWebServiceRequest
	{
		private string bucketName;

		private string delimiter;

		private EncodingType encoding;

		private int? maxKeys;

		private string prefix;

		private string continuationToken;

		private bool? fetchOwner;

		private string startAfter;

		private RequestPayer requestPayer;

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

		public string StartAfter
		{
			get
			{
				return startAfter;
			}
			set
			{
				startAfter = value;
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

		public bool FetchOwner
		{
			get
			{
				return fetchOwner.GetValueOrDefault();
			}
			set
			{
				fetchOwner = value;
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

		internal bool IsSetBucketName()
		{
			return bucketName != null;
		}

		internal bool IsSetDelimiter()
		{
			return delimiter != null;
		}

		internal bool IsSetMaxKeys()
		{
			return maxKeys.HasValue;
		}

		internal bool IsSetPrefix()
		{
			return prefix != null;
		}

		internal bool IsSetEncoding()
		{
			return encoding != null;
		}

		internal bool IsSetStartAfter()
		{
			return StartAfter != null;
		}

		internal bool IsSetContinuationToken()
		{
			return continuationToken != null;
		}

		internal bool IsSetFetchOwner()
		{
			return fetchOwner.HasValue;
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		public ListObjectsV2Request()
			: this()
		{
		}
	}
}
