using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class FilterRuleUnmarshaller : IUnmarshaller<FilterRule, XmlUnmarshallerContext>, IUnmarshaller<FilterRule, JsonUnmarshallerContext>
	{
		private static FilterRuleUnmarshaller _instance;

		public static FilterRuleUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new FilterRuleUnmarshaller();
				}
				return _instance;
			}
		}

		public FilterRule Unmarshall(XmlUnmarshallerContext context)
		{
			FilterRule filterRule = new FilterRule();
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
					if (context.TestExpression("Name", num))
					{
						filterRule.Name = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Value", num))
					{
						filterRule.Value = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return filterRule;
				}
			}
			return filterRule;
		}

		public FilterRule Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
