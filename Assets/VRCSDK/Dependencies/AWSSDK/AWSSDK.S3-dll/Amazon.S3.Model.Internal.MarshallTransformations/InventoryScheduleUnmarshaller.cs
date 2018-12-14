using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class InventoryScheduleUnmarshaller : IUnmarshaller<InventorySchedule, XmlUnmarshallerContext>, IUnmarshaller<InventorySchedule, JsonUnmarshallerContext>
	{
		private static InventoryScheduleUnmarshaller _instance;

		public static InventoryScheduleUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InventoryScheduleUnmarshaller();
				}
				return _instance;
			}
		}

		public InventorySchedule Unmarshall(XmlUnmarshallerContext context)
		{
			InventorySchedule inventorySchedule = new InventorySchedule();
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
					if (context.TestExpression("Frequency", num))
					{
						inventorySchedule.Frequency = InventoryFrequency.FindValue(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return inventorySchedule;
				}
			}
			return inventorySchedule;
		}

		public InventorySchedule Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
