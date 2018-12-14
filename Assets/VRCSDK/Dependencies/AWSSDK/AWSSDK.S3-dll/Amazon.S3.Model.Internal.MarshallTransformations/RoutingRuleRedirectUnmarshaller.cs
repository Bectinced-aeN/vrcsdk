using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class RoutingRuleRedirectUnmarshaller : IUnmarshaller<RoutingRuleRedirect, XmlUnmarshallerContext>, IUnmarshaller<RoutingRuleRedirect, JsonUnmarshallerContext>
	{
		private static RoutingRuleRedirectUnmarshaller _instance;

		public static RoutingRuleRedirectUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RoutingRuleRedirectUnmarshaller();
				}
				return _instance;
			}
		}

		public RoutingRuleRedirect Unmarshall(XmlUnmarshallerContext context)
		{
			RoutingRuleRedirect routingRuleRedirect = new RoutingRuleRedirect();
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
					if (context.TestExpression("HostName", num))
					{
						routingRuleRedirect.HostName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("HttpRedirectCode", num))
					{
						routingRuleRedirect.HttpRedirectCode = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Protocol", num))
					{
						routingRuleRedirect.Protocol = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ReplaceKeyPrefixWith", num))
					{
						routingRuleRedirect.ReplaceKeyPrefixWith = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ReplaceKeyWith", num))
					{
						routingRuleRedirect.ReplaceKeyWith = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return routingRuleRedirect;
				}
			}
			return routingRuleRedirect;
		}

		public RoutingRuleRedirect Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
