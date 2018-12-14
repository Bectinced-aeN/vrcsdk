using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class InventoryS3BucketDestinationUnmarshaller : IUnmarshaller<InventoryS3BucketDestination, XmlUnmarshallerContext>, IUnmarshaller<InventoryS3BucketDestination, JsonUnmarshallerContext>
	{
		private static InventoryS3BucketDestinationUnmarshaller _instance;

		public static InventoryS3BucketDestinationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InventoryS3BucketDestinationUnmarshaller();
				}
				return _instance;
			}
		}

		public InventoryS3BucketDestination Unmarshall(XmlUnmarshallerContext context)
		{
			InventoryS3BucketDestination inventoryS3BucketDestination = new InventoryS3BucketDestination();
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
					if (context.TestExpression("AccountId", num))
					{
						inventoryS3BucketDestination.AccountId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Bucket", num))
					{
						inventoryS3BucketDestination.BucketName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Format", num))
					{
						inventoryS3BucketDestination.InventoryFormat = InventoryFormat.FindValue(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
					else if (context.TestExpression("Prefix", num))
					{
						inventoryS3BucketDestination.Prefix = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return inventoryS3BucketDestination;
				}
			}
			return inventoryS3BucketDestination;
		}

		public InventoryS3BucketDestination Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
