namespace Amazon.Runtime.Internal.Transform
{
	public class DecimalUnmarshaller : IUnmarshaller<decimal, XmlUnmarshallerContext>, IUnmarshaller<decimal, JsonUnmarshallerContext>
	{
		private static DecimalUnmarshaller _instance = new DecimalUnmarshaller();

		public static DecimalUnmarshaller Instance => _instance;

		private DecimalUnmarshaller()
		{
		}

		public static DecimalUnmarshaller GetInstance()
		{
			return Instance;
		}

		public decimal Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<decimal>.Unmarshall(context);
		}

		public decimal Unmarshall(JsonUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<decimal>.Unmarshall(context);
		}
	}
}
