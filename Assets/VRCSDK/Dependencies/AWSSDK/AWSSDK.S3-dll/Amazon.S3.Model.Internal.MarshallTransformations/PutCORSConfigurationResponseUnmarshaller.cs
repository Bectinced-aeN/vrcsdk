using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutCORSConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutCORSConfigurationResponseUnmarshaller _instance;

		public static PutCORSConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutCORSConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutCORSConfigurationResponse();
		}
	}
}
