using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class CORSRuleUnmarshaller : IUnmarshaller<CORSRule, XmlUnmarshallerContext>, IUnmarshaller<CORSRule, JsonUnmarshallerContext>
	{
		private static CORSRuleUnmarshaller _instance;

		public static CORSRuleUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CORSRuleUnmarshaller();
				}
				return _instance;
			}
		}

		public CORSRule Unmarshall(XmlUnmarshallerContext context)
		{
			CORSRule cORSRule = new CORSRule();
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
					if (context.TestExpression("AllowedMethod", num))
					{
						cORSRule.AllowedMethods.Add(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("AllowedOrigin", num))
					{
						cORSRule.AllowedOrigins.Add(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("ExposeHeader", num))
					{
						cORSRule.ExposeHeaders.Add(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("AllowedHeader", num))
					{
						cORSRule.AllowedHeaders.Add(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("MaxAgeSeconds", num))
					{
						cORSRule.MaxAgeSeconds = IntUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ID", num))
					{
						cORSRule.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return cORSRule;
				}
			}
			return cORSRule;
		}

		public CORSRule Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
