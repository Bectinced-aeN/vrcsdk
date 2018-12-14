using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.CognitoIdentity.Model
{
	[Serializable]
	public class ConcurrentModificationException : AmazonCognitoIdentityException
	{
		public ConcurrentModificationException(string message)
			: base(message)
		{
		}

		public ConcurrentModificationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ConcurrentModificationException(Exception innerException)
			: base(innerException)
		{
		}

		public ConcurrentModificationException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		public ConcurrentModificationException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		protected ConcurrentModificationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
