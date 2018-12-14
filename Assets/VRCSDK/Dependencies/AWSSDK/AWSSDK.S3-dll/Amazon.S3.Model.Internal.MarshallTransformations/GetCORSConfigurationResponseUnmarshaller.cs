using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetCORSConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetCORSConfigurationResponseUnmarshaller _instance;

		public static GetCORSConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetCORSConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetCORSConfigurationResponse getCORSConfigurationResponse = new GetCORSConfigurationResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getCORSConfigurationResponse);
				}
			}
			return getCORSConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetCORSConfigurationResponse response)
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
					if (context.TestExpression("CORSRule", num))
					{
						if (response.Configuration == null)
						{
							response.Configuration = new CORSConfiguration();
						}
						response.Configuration.Rules.Add(CORSRuleUnmarshaller.Instance.Unmarshall(context));
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
