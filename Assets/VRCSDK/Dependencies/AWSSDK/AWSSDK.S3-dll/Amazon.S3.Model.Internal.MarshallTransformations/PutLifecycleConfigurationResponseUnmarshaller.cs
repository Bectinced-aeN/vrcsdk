using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutLifecycleConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutLifecycleConfigurationResponseUnmarshaller _instance;

		public static PutLifecycleConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutLifecycleConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutLifecycleConfigurationResponse();
		}
	}
}
