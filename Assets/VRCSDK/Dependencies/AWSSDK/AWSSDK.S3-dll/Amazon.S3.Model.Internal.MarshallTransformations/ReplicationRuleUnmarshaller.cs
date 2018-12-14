using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class ReplicationRuleUnmarshaller : IUnmarshaller<ReplicationRule, XmlUnmarshallerContext>, IUnmarshaller<ReplicationRule, JsonUnmarshallerContext>
	{
		private static ReplicationRuleUnmarshaller _instance;

		public static ReplicationRuleUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ReplicationRuleUnmarshaller();
				}
				return _instance;
			}
		}

		public ReplicationRule Unmarshall(XmlUnmarshallerContext context)
		{
			ReplicationRule replicationRule = new ReplicationRule();
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
					if (context.TestExpression("ID", num))
					{
						replicationRule.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Prefix", num))
					{
						replicationRule.Prefix = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Status", num))
					{
						replicationRule.Status = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Destination", num))
					{
						replicationRule.Destination = ReplicationDestinationUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return replicationRule;
				}
			}
			return replicationRule;
		}

		public ReplicationRule Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
