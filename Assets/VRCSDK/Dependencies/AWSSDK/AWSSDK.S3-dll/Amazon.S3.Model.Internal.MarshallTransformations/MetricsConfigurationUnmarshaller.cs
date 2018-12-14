using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class MetricsConfigurationUnmarshaller : IUnmarshaller<MetricsConfiguration, XmlUnmarshallerContext>, IUnmarshaller<MetricsConfiguration, JsonUnmarshallerContext>
	{
		private static MetricsConfigurationUnmarshaller _instance;

		public static MetricsConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MetricsConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public MetricsConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			MetricsConfiguration metricsConfiguration = new MetricsConfiguration();
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
					if (context.TestExpression("Filter", num))
					{
						metricsConfiguration.MetricsFilter = new MetricsFilter
						{
							MetricsFilterPredicate = MetricsPredicateListFilterUnmarshaller.Instance.Unmarshall(context)[0]
						};
					}
					else if (context.TestExpression("Id", num))
					{
						metricsConfiguration.MetricsId = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return metricsConfiguration;
				}
			}
			return metricsConfiguration;
		}

		public MetricsConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
