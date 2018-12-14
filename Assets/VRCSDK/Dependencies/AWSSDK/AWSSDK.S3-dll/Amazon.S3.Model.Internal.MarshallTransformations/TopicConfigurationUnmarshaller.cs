using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class TopicConfigurationUnmarshaller : IUnmarshaller<TopicConfiguration, XmlUnmarshallerContext>, IUnmarshaller<TopicConfiguration, JsonUnmarshallerContext>
	{
		private static TopicConfigurationUnmarshaller _instance;

		public static TopicConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new TopicConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public TopicConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			TopicConfiguration topicConfiguration = new TopicConfiguration();
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
						topicConfiguration.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Event", num))
					{
						topicConfiguration.Events.Add(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("Topic", num))
					{
						topicConfiguration.Topic = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						topicConfiguration.Filter = FilterUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return topicConfiguration;
				}
			}
			return topicConfiguration;
		}

		public TopicConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
