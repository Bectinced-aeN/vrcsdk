namespace Amazon.Runtime.Internal.Transform
{
	public class ByteUnmarshaller : IUnmarshaller<byte, XmlUnmarshallerContext>, IUnmarshaller<byte, JsonUnmarshallerContext>
	{
		private static ByteUnmarshaller _instance = new ByteUnmarshaller();

		public static ByteUnmarshaller Instance => _instance;

		private ByteUnmarshaller()
		{
		}

		public static ByteUnmarshaller GetInstance()
		{
			return Instance;
		}

		public byte Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<byte>.Unmarshall(context);
		}

		public byte Unmarshall(JsonUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<byte>.Unmarshall(context);
		}
	}
}
