using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class InventoryDestinationUnmarshaller : IUnmarshaller<InventoryDestination, XmlUnmarshallerContext>, IUnmarshaller<InventoryDestination, JsonUnmarshallerContext>
	{
		private static InventoryDestinationUnmarshaller _instance;

		public static InventoryDestinationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InventoryDestinationUnmarshaller();
				}
				return _instance;
			}
		}

		public InventoryDestination Unmarshall(XmlUnmarshallerContext context)
		{
			InventoryDestination inventoryDestination = new InventoryDestination();
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
					if (context.TestExpression("S3BucketDestination", num))
					{
						inventoryDestination.S3BucketDestination = InventoryS3BucketDestinationUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return inventoryDestination;
				}
			}
			return inventoryDestination;
		}

		public InventoryDestination Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
