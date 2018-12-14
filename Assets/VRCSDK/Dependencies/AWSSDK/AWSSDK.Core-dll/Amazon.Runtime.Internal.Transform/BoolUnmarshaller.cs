namespace Amazon.Runtime.Internal.Transform
{
	public class BoolUnmarshaller : IUnmarshaller<bool, XmlUnmarshallerContext>, IUnmarshaller<bool, JsonUnmarshallerContext>
	{
		private static BoolUnmarshaller _instance = new BoolUnmarshaller();

		public static BoolUnmarshaller Instance => _instance;

		private BoolUnmarshaller()
		{
		}

		public static BoolUnmarshaller GetInstance()
		{
			return Instance;
		}

		public bool Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<bool>.Unmarshall(context);
		}

		public bool Unmarshall(JsonUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<bool>.Unmarshall(context);
		}
	}
}
