using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	internal class CommonPrefixesItemUnmarshaller : IUnmarshaller<string, XmlUnmarshallerContext>, IUnmarshaller<string, JsonUnmarshallerContext>
	{
		private static CommonPrefixesItemUnmarshaller _instance;

		public static CommonPrefixesItemUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CommonPrefixesItemUnmarshaller();
				}
				return _instance;
			}
		}

		public string Unmarshall(XmlUnmarshallerContext context)
		{
			int currentDepth = context.get_CurrentDepth();
			int num = currentDepth + 1;
			if (context.get_IsStartOfDocument())
			{
				num += 2;
			}
			string result = null;
			while (context.Read())
			{
				if (context.get_IsStartElement() || context.get_IsAttribute())
				{
					if (context.TestExpression("Prefix", num))
					{
						result = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.get_IsEndElement() && context.get_CurrentDepth() < currentDepth)
				{
					return result;
				}
			}
			return result;
		}

		public string Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
