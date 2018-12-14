using Amazon.Runtime;

namespace Amazon.S3.Model
{
	public class PutObjectTaggingResponse : AmazonWebServiceResponse
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
			return versionId != null;
		}

		public PutObjectTaggingResponse()
			: this()
		{
		}
	}
}
