using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class DeleteObjectResponse : AmazonWebServiceResponse
	{
		private string deleteMarker;

		private string versionId;

		private RequestCharged requestCharged;

		public string DeleteMarker
		{
			get
			{
				return deleteMarker;
			}
			set
			{
				deleteMarker = value;
			}
		}

		public string VersionId
		{
			get
			{
				return versionId;
			}
			set
			{
				versionId = value;
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

		internal bool IsSetDeleteMarker()
		{
			return deleteMarker != null;
		}

		internal bool IsSetVersionId()
		{
			return versionId != null;
		}

		internal bool IsSetRequestCharged()
		{
			return requestCharged != null;
		}

		public DeleteObjectResponse()
			: this()
		{
		}
	}
}
