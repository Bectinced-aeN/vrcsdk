using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteBucketReplicationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteBucketReplicationResponseUnmarshaller _instance;

		public static DeleteBucketReplicationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketReplicationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteBucketReplicationResponse();
		}
	}
}
