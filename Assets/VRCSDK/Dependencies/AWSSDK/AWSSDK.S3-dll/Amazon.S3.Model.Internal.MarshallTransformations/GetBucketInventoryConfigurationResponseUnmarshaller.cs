using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class GetBucketInventoryConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketInventoryConfigurationResponseUnmarshaller _instance;

		public static GetBucketInventoryConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketInventoryConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketInventoryConfigurationResponse getBucketInventoryConfigurationResponse = new GetBucketInventoryConfigurationResponse();
			while (context.Read())
			{
				if (context.get_IsStartElement())
				{
					UnmarshallResult(context, getBucketInventoryConfigurationResponse);
				}
			}
			return getBucketInventoryConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketInventoryConfigurationResponse response)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			response.InventoryConfiguration = new InventoryConfiguration();
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("Destination", num))
					{
						response.InventoryConfiguration.Destination = InventoryDestinationUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("IsEnabled", num))
					{
						response.InventoryConfiguration.IsEnabled = BoolUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						response.InventoryConfiguration.InventoryFilter = InventoryFilterUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Id", num))
					{
						response.InventoryConfiguration.InventoryId = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("IncludedObjectVersions", num))
					{
						response.InventoryConfiguration.IncludedObjectVersions = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("Field", num + 1))
					{
						response.InventoryConfiguration.InventoryOptionalFields.Add(StringUnmarshaller.get_Instance().Unmarshall(context));
					}
					else if (context.TestExpression("Schedule", num))
					{
						response.InventoryConfiguration.Schedule = InventoryScheduleUnmarshaller.Instance.Unmarshall(context);
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
