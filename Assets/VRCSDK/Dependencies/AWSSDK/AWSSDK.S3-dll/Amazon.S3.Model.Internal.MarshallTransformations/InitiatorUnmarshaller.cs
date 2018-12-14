using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class InitiatorUnmarshaller : IUnmarshaller<Initiator, XmlUnmarshallerContext>, IUnmarshaller<Initiator, JsonUnmarshallerContext>
	{
		private static InitiatorUnmarshaller _instance;

		public static InitiatorUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InitiatorUnmarshaller();
				}
				return _instance;
			}
		}

		public Initiator Unmarshall(XmlUnmarshallerContext context)
		{
			Initiator initiator = new Initiator();
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
					if (context.TestExpression("DisplayName", num))
					{
						initiator.DisplayName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ID", num))
					{
						initiator.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return initiator;
				}
			}
			return initiator;
		}

		public Initiator Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
