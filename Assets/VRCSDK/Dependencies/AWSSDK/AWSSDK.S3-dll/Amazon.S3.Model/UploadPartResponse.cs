using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class UploadPartResponse : AmazonWebServiceResponse
	{
		private ServerSideEncryptionMethod serverSideEncryption;

		private string eTag;

		private int partNumber;

		private RequestCharged requestCharged;

		public ServerSideEncryptionMethod ServerSideEncryptionMethod
		{
			get
			{
				return serverSideEncryption;
			}
			set
			{
				serverSideEncryption = value;
			}
		}

		public string ETag
		{
			get
			{
				return eTag;
			}
			set
			{
				eTag = value;
			}
		}

		public int PartNumber
		{
			get
			{
				return partNumber;
			}
			set
			{
				partNumber = value;
			}
		}

		public RequestCharged RequestCharged
		{
			get
			{
				return requestCharged;
			}
			set
			{
				requestCharged = value;
			}
		}

		internal bool IsSetETag()
		{
			return eTag != null;
		}

		internal bool IsSetRequestCharged()
		{
			return requestCharged != null;
		}

		public UploadPartResponse()
			: this()
		{
		}
	}
}
