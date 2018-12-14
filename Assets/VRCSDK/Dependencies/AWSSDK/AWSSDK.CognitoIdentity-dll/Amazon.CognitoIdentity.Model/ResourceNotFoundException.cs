using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.CognitoIdentity.Model
{
	[Serializable]
	public class ResourceNotFoundException : AmazonCognitoIdentityException
	{
		public ResourceNotFoundException(string message)
			: base(message)
		{
		}

		public ResourceNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ResourceNotFoundException(Exception innerException)
			: base(innerException)
		{
		}

		public ResourceNotFoundException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		public ResourceNotFoundException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		protected ResourceNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
