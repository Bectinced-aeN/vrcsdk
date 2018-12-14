using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class QueueConfigurationUnmarshaller : IUnmarshaller<QueueConfiguration, XmlUnmarshallerContext>, IUnmarshaller<QueueConfiguration, JsonUnmarshallerContext>
	{
		private static QueueConfigurationUnmarshaller _instance;

		public static QueueConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new QueueConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public QueueConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			QueueConfiguration queueConfiguration = new QueueConfiguration();
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
						queueConfiguration.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Event", num))
					{
						queueConfiguration.Events.Add(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("Queue", num))
					{
						queueConfiguration.Queue = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						queueConfiguration.Filter = FilterUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return queueConfiguration;
				}
			}
			return queueConfiguration;
		}

		public QueueConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
