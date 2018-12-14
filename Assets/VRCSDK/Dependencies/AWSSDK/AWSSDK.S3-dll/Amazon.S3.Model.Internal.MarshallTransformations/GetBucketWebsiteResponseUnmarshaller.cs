using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketWebsiteResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketWebsiteResponseUnmarshaller _instance;

		public static GetBucketWebsiteResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketWebsiteResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketWebsiteResponse getBucketWebsiteResponse = new GetBucketWebsiteResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketWebsiteResponse);
				}
			}
			return getBucketWebsiteResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketWebsiteResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			response.WebsiteConfiguration = new WebsiteConfiguration();
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("RedirectAllRequestsTo", num))
					{
						response.WebsiteConfiguration.RedirectAllRequestsTo = RoutingRuleRedirectUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("IndexDocument/Suffix", num))
					{
						response.WebsiteConfiguration.IndexDocumentSuffix = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("ErrorDocument/Key", num))
					{
						response.WebsiteConfiguration.ErrorDocument = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("RoutingRule", num + 1))
					{
						response.WebsiteConfiguration.RoutingRules.Add(RoutingRuleUnmarshaller.Instance.Unmarshall(context));
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
