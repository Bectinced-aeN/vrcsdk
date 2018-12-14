using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetACLResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetACLResponseUnmarshaller _instance;

		public static GetACLResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetACLResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetACLResponse getACLResponse = new GetACLResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getACLResponse);
				}
			}
			return getACLResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetACLResponse response)
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
					if (context.TestExpression("Owner", num))
					{
						if (response.AccessControlList == null)
						{
							response.AccessControlList = new S3AccessControlList();
						}
						response.AccessControlList.Owner = OwnerUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Grant", num + 1))
					{
						if (response.AccessControlList == null)
						{
							response.AccessControlList = new S3AccessControlList();
						}
						response.AccessControlList.Grants.Add(GrantUnmarshaller.Instance.Unmarshall(context));
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
