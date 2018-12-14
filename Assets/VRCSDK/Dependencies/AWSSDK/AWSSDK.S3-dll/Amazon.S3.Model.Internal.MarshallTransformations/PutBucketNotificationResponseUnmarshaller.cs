using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketNotificationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketNotificationResponseUnmarshaller _instance;

		public static PutBucketNotificationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketNotificationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketNotificationResponse();
		}
	}
}
