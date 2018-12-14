using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class ListBucketAnalyticsConfigurationsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListBucketAnalyticsConfigurationsResponseUnmarshaller _instance = new ListBucketAnalyticsConfigurationsResponseUnmarshaller();

		public static ListBucketAnalyticsConfigurationsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListBucketAnalyticsConfigurationsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListBucketAnalyticsConfigurationsResponse listBucketAnalyticsConfigurationsResponse = new ListBucketAnalyticsConfigurationsResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, listBucketAnalyticsConfigurationsResponse);
				}
			}
			return listBucketAnalyticsConfigurationsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListBucketAnalyticsConfigurationsResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("ContinuationToken", num))
					{
						response.ContinuationToken = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("AnalyticsConfiguration", num))
					{
						response.AnalyticsConfigurationList.Add(AnalyticsConfigurationUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("IsTruncated", num))
					{
						response.IsTruncated = BoolUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("NextContinuationToken", num))
					{
						response.NextContinuationToken = StringUnmarshaller.get_Instance().Unmarshall(context);
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
