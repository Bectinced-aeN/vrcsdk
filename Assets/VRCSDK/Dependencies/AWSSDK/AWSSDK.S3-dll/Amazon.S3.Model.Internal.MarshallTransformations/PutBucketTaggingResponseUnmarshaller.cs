using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketTaggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketTaggingResponseUnmarshaller _instance;

		public static PutBucketTaggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketTaggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketTaggingResponse();
		}
	}
}
