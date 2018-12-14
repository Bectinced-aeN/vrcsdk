using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteBucketTaggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteBucketTaggingResponseUnmarshaller _instance;

		public static DeleteBucketTaggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketTaggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteBucketTaggingResponse();
		}
	}
}
