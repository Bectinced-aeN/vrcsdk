using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.CognitoIdentity.Model
{
	[Serializable]
	public class ResourceConflictException : AmazonCognitoIdentityException
	{
		public ResourceConflictException(string message)
			: base(message)
		{
		}

		public ResourceConflictException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ResourceConflictException(Exception innerException)
			: base(innerException)
		{
		}

		public ResourceConflictException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		public ResourceConflictException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		protected ResourceConflictException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
