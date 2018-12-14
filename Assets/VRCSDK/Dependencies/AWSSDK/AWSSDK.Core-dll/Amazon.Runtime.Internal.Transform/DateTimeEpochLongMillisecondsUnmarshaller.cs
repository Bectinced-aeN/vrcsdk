using Amazon.Util;
using System;

namespace Amazon.Runtime.Internal.Transform
{
	public class DateTimeEpochLongMillisecondsUnmarshaller : IUnmarshaller<DateTime, XmlUnmarshallerContext>, IUnmarshaller<DateTime, JsonUnmarshallerContext>
	{
		private static DateTimeEpochLongMillisecondsUnmarshaller _instance = new DateTimeEpochLongMillisecondsUnmarshaller();

		public static DateTimeEpochLongMillisecondsUnmarshaller Instance => _instance;

		private DateTimeEpochLongMillisecondsUnmarshaller()
		{
		}

		public static DateTimeEpochLongMillisecondsUnmarshaller GetInstance()
		{
			return Instance;
		}

		public DateTime Unmarshall(XmlUnmarshallerContext context)
		{
			return SimpleTypeUnmarshaller<DateTime>.Unmarshall(context);
		}

		public DateTime Unmarshall(JsonUnmarshallerContext context)
		{
			long num = LongUnmarshaller.Instance.Unmarshall(context);
			return AWSSDKUtils.EPOCH_START.AddMilliseconds((double)num);
		}
	}
}
