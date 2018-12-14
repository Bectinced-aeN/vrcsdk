using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class InventoryConfigurationUnmarshaller : IUnmarshaller<InventoryConfiguration, XmlUnmarshallerContext>, IUnmarshaller<InventoryConfiguration, JsonUnmarshallerContext>
	{
		private static InventoryConfigurationUnmarshaller _instance;

		public static InventoryConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InventoryConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public InventoryConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			InventoryConfiguration inventoryConfiguration = new InventoryConfiguration();
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
					if (context.TestExpression("Destination", num))
					{
						inventoryConfiguration.Destination = InventoryDestinationUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("IsEnabled", num))
					{
						inventoryConfiguration.IsEnabled = BoolUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						inventoryConfiguration.InventoryFilter = InventoryFilterUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Id", num))
					{
						inventoryConfiguration.InventoryId = StringUnmarshaller.get_Instance().Unmarshall(context);
					}
					else if (context.TestExpression("IncludedObjectVersions", num))
					{
						inventoryConfiguration.IncludedObjectVersions = InventoryIncludedObjectVersions.FindValue(StringUnmarshaller.get_Instance().Unmarshall(context));
					}
					else if (context.TestExpression("Field", num + 1))
					{
						inventoryConfiguration.InventoryOptionalFields.Add(InventoryOptionalField.FindValue(StringUnmarshaller.get_Instance().Unmarshall(context)));
					}
					else if (context.TestExpression("Schedule", num))
					{
						inventoryConfiguration.Schedule = InventoryScheduleUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return inventoryConfiguration;
				}
			}
			return inventoryConfiguration;
		}

		public InventoryConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
