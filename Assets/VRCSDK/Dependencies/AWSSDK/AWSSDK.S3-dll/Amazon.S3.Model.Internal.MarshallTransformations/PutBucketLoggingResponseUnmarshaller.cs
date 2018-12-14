using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketLoggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketLoggingResponseUnmarshaller _instance;

		public static PutBucketLoggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketLoggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketLoggingResponse();
		}
	}
}
