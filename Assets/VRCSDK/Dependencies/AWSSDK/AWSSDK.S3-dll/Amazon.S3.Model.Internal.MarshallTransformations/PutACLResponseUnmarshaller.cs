using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutACLResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutACLResponseUnmarshaller _instance;

		public static PutACLResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutACLResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutACLResponse();
		}
	}
}
