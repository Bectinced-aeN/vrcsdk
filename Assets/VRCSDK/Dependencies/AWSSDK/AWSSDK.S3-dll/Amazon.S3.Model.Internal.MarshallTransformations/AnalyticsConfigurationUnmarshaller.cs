using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class AnalyticsConfigurationUnmarshaller : IUnmarshaller<AnalyticsConfiguration, XmlUnmarshallerContext>, IUnmarshaller<AnalyticsConfiguration, JsonUnmarshallerContext>
	{
		private static AnalyticsConfigurationUnmarshaller _instance;

		public static AnalyticsConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AnalyticsConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public AnalyticsConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			AnalyticsConfiguration analyticsConfiguration = new AnalyticsConfiguration();
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
					if (context.TestExpression("Id", num))
					{
						analyticsConfiguration.AnalyticsId = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						analyticsConfiguration.AnalyticsFilter = new AnalyticsFilter
						{
							AnalyticsFilterPredicate = AnalyticsPredicateListUnmarshaller.Instance.Unmarshall(context)[0]
						};
					}
					else if (context.TestExpression("StorageClassAnalysis", num))
					{
						analyticsConfiguration.StorageClassAnalysis = StorageClassAnalysisUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return analyticsConfiguration;
				}
			}
			return analyticsConfiguration;
		}

		public AnalyticsConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
