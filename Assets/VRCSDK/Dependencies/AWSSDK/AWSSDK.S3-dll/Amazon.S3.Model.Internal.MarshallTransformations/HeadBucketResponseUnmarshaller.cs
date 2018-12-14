using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class HeadBucketResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static HeadBucketResponseUnmarshaller _instance;

		public static HeadBucketResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new HeadBucketResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new HeadBucketResponse();
		}
	}
}
