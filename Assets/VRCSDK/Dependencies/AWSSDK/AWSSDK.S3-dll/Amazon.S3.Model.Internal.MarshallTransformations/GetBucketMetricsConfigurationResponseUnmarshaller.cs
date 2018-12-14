using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketMetricsConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketMetricsConfigurationResponseUnmarshaller _instance;

		public static GetBucketMetricsConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketMetricsConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketMetricsConfigurationResponse getBucketMetricsConfigurationResponse = new GetBucketMetricsConfigurationResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketMetricsConfigurationResponse);
				}
			}
			return getBucketMetricsConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketMetricsConfigurationResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			response.MetricsConfiguration = new MetricsConfiguration();
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("Filter", num))
					{
						response.MetricsConfiguration.MetricsFilter = new MetricsFilter
						{
							MetricsFilterPredicate = MetricsPredicateListFilterUnmarshaller.Instance.Unmarshall(context)[0]
						};
					}
					else if (context.TestExpression("Id", num))
					{
						response.MetricsConfiguration.MetricsId = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					break;
				}
			}
		}
	}
}
