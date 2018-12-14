using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class InventoryFilterUnmarshaller : IUnmarshaller<InventoryFilter, XmlUnmarshallerContext>, IUnmarshaller<InventoryFilter, JsonUnmarshallerContext>
	{
		private static InventoryFilterUnmarshaller _instance;

		public static InventoryFilterUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InventoryFilterUnmarshaller();
				}
				return _instance;
			}
		}

		public InventoryFilter Unmarshall(XmlUnmarshallerContext context)
		{
			InventoryFilter inventoryFilter = new InventoryFilter();
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
					if (context.TestExpression("Prefix", num))
					{
						inventoryFilter.InventoryFilterPredicate = new InventoryPrefixPredicate(StringUnmarshaller.get_Instance().Unmarshall(context));
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return inventoryFilter;
				}
			}
			return inventoryFilter;
		}

		public InventoryFilter Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
