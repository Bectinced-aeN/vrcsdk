using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteBucketWebsiteResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteBucketWebsiteResponseUnmarshaller _instance;

		public static DeleteBucketWebsiteResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketWebsiteResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteBucketWebsiteResponse();
		}
	}
}
