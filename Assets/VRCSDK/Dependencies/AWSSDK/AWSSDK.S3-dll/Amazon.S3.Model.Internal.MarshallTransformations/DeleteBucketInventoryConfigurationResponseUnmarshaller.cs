using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteBucketInventoryConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteBucketInventoryConfigurationResponseUnmarshaller _instance;

		public static DeleteBucketInventoryConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketInventoryConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteBucketInventoryConfigurationResponse();
		}
	}
}
