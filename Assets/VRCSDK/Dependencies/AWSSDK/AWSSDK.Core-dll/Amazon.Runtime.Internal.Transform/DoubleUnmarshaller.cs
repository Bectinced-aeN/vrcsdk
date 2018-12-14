namespace Amazon.Runtime.Internal.Transform
{
	public class DoubleUnmarshaller : IUnmarshaller<double, XmlUnmarshallerContext>, IUnmarshaller<double, JsonUnmarshallerContext>
	{
		private static DoubleUnmarshaller _instance = new DoubleUnmarshaller();

		public static DoubleUnmarshaller Instance => _instance;

		private DoubleUnmarshaller()
		{
		}

		public static DoubleUnmarshaller GetInstance()
		{
			return Instance;
		}

		public double Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<double>.Unmarshall(context);
		}

		public double Unmarshall(JsonUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<double>.Unmarshall(context);
		}
	}
}
