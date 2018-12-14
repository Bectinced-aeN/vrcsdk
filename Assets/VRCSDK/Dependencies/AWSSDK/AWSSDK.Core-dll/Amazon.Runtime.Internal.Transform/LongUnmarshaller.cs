namespace Amazon.Runtime.Internal.Transform
{
	public class LongUnmarshaller : IUnmarshaller<long, XmlUnmarshallerContext>, IUnmarshaller<long, JsonUnmarshallerContext>
	{
		private static LongUnmarshaller _instance = new LongUnmarshaller();

		public static LongUnmarshaller Instance => _instance;

		private LongUnmarshaller()
		{
		}

		public static LongUnmarshaller GetInstance()
		{
			return Instance;
		}

		public long Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<long>.Unmarshall(context);
		}

		public long Unmarshall(JsonUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<long>.Unmarshall(context);
		}
	}
}
