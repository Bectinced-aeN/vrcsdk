using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteBucketAnalyticsConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteBucketAnalyticsConfigurationResponseUnmarshaller _instance;

		public static DeleteBucketAnalyticsConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketAnalyticsConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteBucketAnalyticsConfigurationResponse();
		}
	}
}
