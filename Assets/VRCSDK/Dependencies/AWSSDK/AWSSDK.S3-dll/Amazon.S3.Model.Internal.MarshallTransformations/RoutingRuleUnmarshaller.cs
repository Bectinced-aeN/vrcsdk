using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class RoutingRuleUnmarshaller : IUnmarshaller<RoutingRule, XmlUnmarshallerContext>, IUnmarshaller<RoutingRule, JsonUnmarshallerContext>
	{
		private static RoutingRuleUnmarshaller _instance;

		public static RoutingRuleUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RoutingRuleUnmarshaller();
				}
				return _instance;
			}
		}

		public RoutingRule Unmarshall(XmlUnmarshallerContext context)
		{
			RoutingRule routingRule = new RoutingRule();
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
					if (context.TestExpression("Condition", num))
					{
						routingRule.Condition = RoutingRuleConditionUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Redirect", num))
					{
						routingRule.Redirect = RoutingRuleRedirectUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return routingRule;
				}
			}
			return routingRule;
		}

		public RoutingRule Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
