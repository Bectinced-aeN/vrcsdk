using System;
using System.Runtime.Serialization;
using System.Security;

namespace Amazon.Runtime
{
	[Serializable]
	public class AmazonDateTimeUnmarshallingException : AmazonUnmarshallingException
	{
		public string InvalidDateTimeToken
		{
			get;
			private set;
		}

		public AmazonDateTimeUnmarshallingException(string requestId, string lastKnownLocation, string invalidDateTimeToken, Exception innerException)
			: base(requestId, lastKnownLocation, innerException)
		{
			InvalidDateTimeToken = invalidDateTimeToken;
		}

		public AmazonDateTimeUnmarshallingException(string requestId, string lastKnownLocation, string responseBody, string invalidDateTimeToken, Exception innerException)
			: base(requestId, lastKnownLocation, responseBody, innerException)
		{
			InvalidDateTimeToken = invalidDateTimeToken;
		}

		public AmazonDateTimeUnmarshallingException(string requestId, string lastKnownLocation, string responseBody, string invalidDateTimeToken, string message, Exception innerException)
			: base(requestId, lastKnownLocation, responseBody, message, innerException)
		{
			InvalidDateTimeToken = invalidDateTimeToken;
		}

		protected AmazonDateTimeUnmarshallingException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				InvalidDateTimeToken = info.GetString("InvalidDateTimeToken");
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info?.AddValue("InvalidDateTimeToken", InvalidDateTimeToken);
		}
	}
}
