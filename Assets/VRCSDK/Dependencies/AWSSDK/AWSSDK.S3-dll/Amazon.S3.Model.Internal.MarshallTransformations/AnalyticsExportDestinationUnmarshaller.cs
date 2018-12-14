using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class AnalyticsExportDestinationUnmarshaller : IUnmarshaller<AnalyticsExportDestination, XmlUnmarshallerContext>, IUnmarshaller<AnalyticsExportDestination, JsonUnmarshallerContext>
	{
		private static AnalyticsExportDestinationUnmarshaller _instance;

		public static AnalyticsExportDestinationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AnalyticsExportDestinationUnmarshaller();
				}
				return _instance;
			}
		}

		public AnalyticsExportDestination Unmarshall(XmlUnmarshallerContext context)
		{
			AnalyticsExportDestination analyticsExportDestination = new AnalyticsExportDestination();
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
					if (context.TestExpression("S3BucketDestination", num))
					{
						analyticsExportDestination.S3BucketDestination = AnalyticsS3BucketDestinationUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return analyticsExportDestination;
				}
			}
			return analyticsExportDestination;
		}

		public AnalyticsExportDestination Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
