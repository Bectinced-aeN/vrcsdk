using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteBucketResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteBucketResponseUnmarshaller _instance;

		public static DeleteBucketResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteBucketResponse();
		}
	}
}
