using System;
using System.Globalization;

namespace Amazon.Runtime.Internal.Transform
{
	public class DateTimeUnmarshaller : IUnmarshaller<DateTime, XmlUnmarshallerContext>, IUnmarshaller<DateTime, JsonUnmarshallerContext>
	{
		private static DateTimeUnmarshaller _instance = new DateTimeUnmarshaller();

		public static DateTimeUnmarshaller Instance => _instance;

		private DateTimeUnmarshaller()
		{
		}

		public static DateTimeUnmarshaller GetInstance()
		{
			return Instance;
		}

		public DateTime Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<DateTime>.Unmarshall(context);
		}

		public DateTime Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			string text = context.ReadText();
			if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
			{
				return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(result);
			}
			if (text == null)
			{
				return default(DateTime);
			}
			return (DateTime)Convert.ChangeType(text, typeof(DateTime), CultureInfo.InvariantCulture);
		}
	}
}
