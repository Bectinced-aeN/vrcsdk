using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteBucketPolicyResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteBucketPolicyResponseUnmarshaller _instance;

		public static DeleteBucketPolicyResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketPolicyResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteBucketPolicyResponse();
		}
	}
}
