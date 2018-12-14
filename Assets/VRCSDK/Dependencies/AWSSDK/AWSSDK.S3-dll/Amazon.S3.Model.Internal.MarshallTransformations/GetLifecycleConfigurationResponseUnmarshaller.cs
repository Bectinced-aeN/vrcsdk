using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetLifecycleConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetLifecycleConfigurationResponseUnmarshaller _instance;

		public static GetLifecycleConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetLifecycleConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetLifecycleConfigurationResponse getLifecycleConfigurationResponse = new GetLifecycleConfigurationResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getLifecycleConfigurationResponse);
				}
			}
			return getLifecycleConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetLifecycleConfigurationResponse response)
		{
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
					if (context.TestExpression("Rule", num))
					{
						response.Configuration.Rules.Add(RulesItemUnmarshaller.Instance.Unmarshall(context));
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					break;
				}
			}
		}
	}
}
