using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketAccelerateConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketAccelerateConfigurationResponseUnmarshaller _instance;

		public static PutBucketAccelerateConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketAccelerateConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketAccelerateConfigurationResponse();
		}
	}
}
