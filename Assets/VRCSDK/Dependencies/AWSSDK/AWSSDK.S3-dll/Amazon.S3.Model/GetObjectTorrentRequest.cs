using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class GetObjectTorrentRequest : AmazonWebServiceRequest
	{
		private string bucketName;

		private string key;

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

		internal bool IsSetRequestPayer()
		{
			return requestPayer != null;
		}

		public GetObjectTorrentRequest()
			: this()
		{
		}
	}
}
