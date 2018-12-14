using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class ListBucketInventoryConfigurationsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListBucketInventoryConfigurationsResponseUnmarshaller _instance = new ListBucketInventoryConfigurationsResponseUnmarshaller();

		public static ListBucketInventoryConfigurationsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListBucketInventoryConfigurationsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListBucketInventoryConfigurationsResponse listBucketInventoryConfigurationsResponse = new ListBucketInventoryConfigurationsResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, listBucketInventoryConfigurationsResponse);
				}
			}
			return listBucketInventoryConfigurationsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListBucketInventoryConfigurationsResponse response)
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
					if (context.TestExpression("ContinuationToken", num))
					{
						response.Token = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("InventoryConfiguration", num))
					{
						response.InventoryConfigurationList.Add(InventoryConfigurationUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("IsTruncated", num))
					{
						response.IsTruncated = BoolUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("NextContinuationToken", num))
					{
						response.NextToken = StringUnmarshaller.get_Instance().Unmarshall(context);
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
