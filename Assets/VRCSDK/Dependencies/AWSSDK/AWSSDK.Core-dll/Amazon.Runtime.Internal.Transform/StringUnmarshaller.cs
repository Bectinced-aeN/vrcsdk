namespace Amazon.Runtime.Internal.Transform
{
	public class StringUnmarshaller : IUnmarshaller<string, XmlUnmarshallerContext>, IUnmarshaller<string, JsonUnmarshallerContext>
	{
		private static StringUnmarshaller _instance = new StringUnmarshaller();

		public static StringUnmarshaller Instance => _instance;

		private StringUnmarshaller()
		{
		}

		public static StringUnmarshaller GetInstance()
		{
			return Instance;
		}

		public string Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<string>.Unmarshall(context);
		}

		public string Unmarshall(JsonUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<string>.Unmarshall(context);
		}
	}
}
