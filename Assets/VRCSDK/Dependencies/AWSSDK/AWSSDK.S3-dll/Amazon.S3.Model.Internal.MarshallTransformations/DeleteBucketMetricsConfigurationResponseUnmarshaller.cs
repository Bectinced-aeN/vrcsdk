using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class DeleteBucketMetricsConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteBucketMetricsConfigurationResponseUnmarshaller _instance;

		public static DeleteBucketMetricsConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketMetricsConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteBucketMetricsConfigurationResponse();
		}
	}
}
