using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class OwnerUnmarshaller : IUnmarshaller<Owner, XmlUnmarshallerContext>, IUnmarshaller<Owner, JsonUnmarshallerContext>
	{
		private static OwnerUnmarshaller _instance;

		public static OwnerUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new OwnerUnmarshaller();
				}
				return _instance;
			}
		}

		public Owner Unmarshall(XmlUnmarshallerContext context)
		{
			Owner owner = new Owner();
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
						owner.DisplayName = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("ID", num))
					{
						owner.Id = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return owner;
				}
			}
			return owner;
		}

		public Owner Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
