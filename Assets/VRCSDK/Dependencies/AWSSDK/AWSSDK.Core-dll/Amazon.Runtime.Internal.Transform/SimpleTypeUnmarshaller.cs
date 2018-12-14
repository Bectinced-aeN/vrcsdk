using System;
using System.Globalization;

namespace Amazon.Runtime.Internal.Transform
{
	internal static class SimpleTypeUnmarshaller<T>
	{
		public static T Unmarshall(XmlUnmarshallerContext context)
		{
			return (T)Convert.ChangeType(context.ReadText(), typeof(T), CultureInfo.InvariantCulture);
		}

		public static T Unmarshall(JsonUnmarshallerContext context)
		{
			context.Read();
			string text = context.ReadText();
			if (text == null)
			{
				return default(T);
			}
			return (T)Convert.ChangeType(text, typeof(T), CultureInfo.InvariantCulture);
		}
	}
}
