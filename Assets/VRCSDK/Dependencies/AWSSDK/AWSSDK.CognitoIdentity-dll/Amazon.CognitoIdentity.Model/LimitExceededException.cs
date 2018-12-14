using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.CognitoIdentity.Model
{
	[Serializable]
	public class LimitExceededException : AmazonCognitoIdentityException
	{
		public LimitExceededException(string message)
			: base(message)
		{
		}

		public LimitExceededException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public LimitExceededException(Exception innerException)
			: base(innerException)
		{
		}

		public LimitExceededException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		public LimitExceededException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		protected LimitExceededException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
