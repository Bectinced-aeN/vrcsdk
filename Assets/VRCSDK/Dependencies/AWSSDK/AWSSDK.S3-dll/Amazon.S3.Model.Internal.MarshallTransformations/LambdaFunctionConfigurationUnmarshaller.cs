using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class LambdaFunctionConfigurationUnmarshaller : IUnmarshaller<LambdaFunctionConfiguration, XmlUnmarshallerContext>, IUnmarshaller<LambdaFunctionConfiguration, JsonUnmarshallerContext>
	{
		private static LambdaFunctionConfigurationUnmarshaller _instance;

		public static LambdaFunctionConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LambdaFunctionConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public LambdaFunctionConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			LambdaFunctionConfiguration lambdaFunctionConfiguration = new LambdaFunctionConfiguration();
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
						lambdaFunctionConfiguration.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Event", num))
					{
						lambdaFunctionConfiguration.Events.Add(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("CloudFunction", num))
					{
						lambdaFunctionConfiguration.FunctionArn = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						lambdaFunctionConfiguration.Filter = FilterUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return lambdaFunctionConfiguration;
				}
			}
			return lambdaFunctionConfiguration;
		}

		public LambdaFunctionConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
