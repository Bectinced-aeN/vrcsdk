using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketPolicyResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketPolicyResponseUnmarshaller _instance;

		public static PutBucketPolicyResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketPolicyResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketPolicyResponse();
		}
	}
}
