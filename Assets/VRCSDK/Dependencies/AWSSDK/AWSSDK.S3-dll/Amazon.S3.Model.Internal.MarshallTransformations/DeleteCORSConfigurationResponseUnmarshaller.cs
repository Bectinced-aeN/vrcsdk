using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteCORSConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteCORSConfigurationResponseUnmarshaller _instance;

		public static DeleteCORSConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteCORSConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteCORSConfigurationResponse();
		}
	}
}
