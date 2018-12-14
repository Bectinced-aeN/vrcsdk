using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.SecurityToken.Model
{
	[Serializable]
	public class IDPRejectedClaimException : AmazonSecurityTokenServiceException
	{
		public IDPRejectedClaimException(string message)
			: base(message)
		{
		}

		public IDPRejectedClaimException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public IDPRejectedClaimException(Exception innerException)
			: base(innerException)
		{
		}

		public IDPRejectedClaimException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0003: Unknown result type (might be due to invalid IL or missing references)


		public IDPRejectedClaimException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


		protected IDPRejectedClaimException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
