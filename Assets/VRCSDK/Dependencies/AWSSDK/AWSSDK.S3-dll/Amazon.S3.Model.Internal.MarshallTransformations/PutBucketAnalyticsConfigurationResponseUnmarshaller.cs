using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class PutBucketAnalyticsConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketAnalyticsConfigurationResponseUnmarshaller _instance;

		public static PutBucketAnalyticsConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketAnalyticsConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketAnalyticsConfigurationResponse();
		}
	}
}
