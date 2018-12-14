using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class DeleteObjectTaggingResponse : AmazonWebServiceResponse
	{
		private string versionId;

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

		internal bool IsSetVersionId()
		{
			return !string.IsNullOrEmpty(versionId);
		}

		public DeleteObjectTaggingResponse()
			: this()
		{
		}
	}
}
