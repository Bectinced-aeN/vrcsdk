using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketAnalyticsConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketAnalyticsConfigurationResponseUnmarshaller _instance;

		public static GetBucketAnalyticsConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketAnalyticsConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketAnalyticsConfigurationResponse getBucketAnalyticsConfigurationResponse = new GetBucketAnalyticsConfigurationResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketAnalyticsConfigurationResponse);
				}
			}
			return getBucketAnalyticsConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketAnalyticsConfigurationResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			response.AnalyticsConfiguration = new AnalyticsConfiguration();
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("Id", num))
					{
						response.AnalyticsConfiguration.AnalyticsId = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						response.AnalyticsConfiguration.AnalyticsFilter = new AnalyticsFilter
						{
							AnalyticsFilterPredicate = AnalyticsPredicateListUnmarshaller.Instance.Unmarshall(context)[0]
						};
					}
					else if (context.TestExpression("StorageClassAnalysis", num))
					{
						response.AnalyticsConfiguration.StorageClassAnalysis = StorageClassAnalysisUnmarshaller.Instance.Unmarshall(context);
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
