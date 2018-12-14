using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketInventoryConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketInventoryConfigurationResponseUnmarshaller _instance;

		public static PutBucketInventoryConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketInventoryConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketInventoryConfigurationResponse();
		}
	}
}
