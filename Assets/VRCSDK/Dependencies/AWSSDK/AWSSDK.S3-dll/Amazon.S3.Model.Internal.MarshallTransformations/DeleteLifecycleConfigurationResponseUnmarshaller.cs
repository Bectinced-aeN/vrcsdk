using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteLifecycleConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteLifecycleConfigurationResponseUnmarshaller _instance;

		public static DeleteLifecycleConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteLifecycleConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteLifecycleConfigurationResponse();
		}
	}
}
