namespace Amazon.Runtime.Internal.Transform
{
	public class FloatUnmarshaller : IUnmarshaller<float, XmlUnmarshallerContext>, IUnmarshaller<float, JsonUnmarshallerContext>
	{
		private static FloatUnmarshaller _instance = new FloatUnmarshaller();

		public static FloatUnmarshaller Instance => _instance;

		private FloatUnmarshaller()
		{
		}

		public static FloatUnmarshaller GetInstance()
		{
			return Instance;
		}

		public float Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<float>.Unmarshall(context);
		}

		public float Unmarshall(JsonUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<float>.Unmarshall(context);
		}
	}
}
