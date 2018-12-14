using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketResponseUnmarshaller _instance;

		public static PutBucketResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			PutBucketResponse putBucketResponse = new PutBucketResponse();
			UnmarshallResult(context, putBucketResponse);
			return putBucketResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, PutBucketResponse response)
		{
		}
	}
}
