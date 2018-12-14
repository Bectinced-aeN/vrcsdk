using System;
using System.IO;

namespace Amazon.Runtime.Internal.Transform
{
	public class MemoryStreamUnmarshaller : IUnmarshaller<MemoryStream, XmlUnmarshallerContext>, IUnmarshaller<MemoryStream, JsonUnmarshallerContext>
	{
		private static MemoryStreamUnmarshaller _instance = new MemoryStreamUnmarshaller();

		public static MemoryStreamUnmarshaller Instance => _instance;

		private MemoryStreamUnmarshaller()
		{
		}

		public static MemoryStreamUnmarshaller GetInstance()
		{
			return Instance;
		}

		public MemoryStream Unmarshall(XmlUnmarshallerContext context)
		{
			return new MemoryStream(Convert.FromBase64String(context.ReadText()));
		}

		public MemoryStream Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			return new MemoryStream(Convert.FromBase64String(context.ReadText()));
		}
	}
}
