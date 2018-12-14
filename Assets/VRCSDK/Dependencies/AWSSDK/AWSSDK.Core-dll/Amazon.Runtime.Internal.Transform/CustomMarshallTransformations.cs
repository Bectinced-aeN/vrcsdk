using Amazon.Util;
using System;

namespace Amazon.Runtime.Internal.Transform
{
	public static class CustomMarshallTransformations
	{
		public static long ConvertDateTimeToEpochMilliseconds(DateTime dateTime)
		{
			return (long)new TimeSpan(dateTime.ToUniversalTime().Ticks - AWSSDKUtils.EPOCH_START.Ticks).TotalMilliseconds;
		}
	}
}
