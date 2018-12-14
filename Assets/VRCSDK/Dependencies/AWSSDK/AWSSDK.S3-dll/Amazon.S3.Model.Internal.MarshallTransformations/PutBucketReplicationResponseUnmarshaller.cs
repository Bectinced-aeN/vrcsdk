using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketReplicationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketReplicationResponseUnmarshaller _instance;

		public static PutBucketReplicationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketReplicationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketReplicationResponse();
		}
	}
}
