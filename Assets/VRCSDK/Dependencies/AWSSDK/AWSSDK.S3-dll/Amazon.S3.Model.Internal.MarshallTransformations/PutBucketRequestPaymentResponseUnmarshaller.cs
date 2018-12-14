using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketRequestPaymentResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketRequestPaymentResponseUnmarshaller _instance;

		public static PutBucketRequestPaymentResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketRequestPaymentResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketRequestPaymentResponse();
		}
	}
}
