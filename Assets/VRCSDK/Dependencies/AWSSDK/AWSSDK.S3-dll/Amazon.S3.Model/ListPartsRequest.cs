using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class ListPartsRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string key;

		private int? maxParts;

		private string partNumberMarker;

		private string uploadId;

		private EncodingType encoding;

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

		public int MaxParts
		{
			get
			{
				return maxParts ?? 0;
			}
			set
			{
				maxParts = value;
			}
		}

		public string PartNumberMarker
		{
			get
			{
				return partNumberMarker;
			}
			set
			{
				partNumberMarker = value;
			}
		}

		public string UploadId
		{
			get
			{
				return uploadId;
			}
			set
			{
				uploadId = value;
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

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetMaxParts()
		{
			return maxParts.HasValue;
		}

		internal bool IsSetPartNumberMarker()
		{
			return partNumberMarker != null;
		}

		internal bool IsSetUploadId()
		{
			return uploadId != null;
		}

		internal bool IsSetEncoding()
		{
			return encoding != null;
		}

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		public ListPartsRequest()
			: this()
		{
		}
	}
}
