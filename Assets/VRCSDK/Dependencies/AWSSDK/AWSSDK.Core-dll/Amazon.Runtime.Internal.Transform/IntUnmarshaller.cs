namespace Amazon.Runtime.Internal.Transform
{
	public class IntUnmarshaller : IUnmarshaller<int, XmlUnmarshallerContext>, IUnmarshaller<int, JsonUnmarshallerContext>
	{
		private static IntUnmarshaller _instance = new IntUnmarshaller();

		public static IntUnmarshaller Instance => _instance;

		private IntUnmarshaller()
		{
		}

		public static IntUnmarshaller GetInstance()
		{
			return Instance;
		}

		public int Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<int>.Unmarshall(context);
		}

		public int Unmarshall(JsonUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<int>.Unmarshall(context);
		}
	}
}
