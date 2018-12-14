using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.CognitoIdentity
{
	[Serializable]
	public class AmazonCognitoIdentityException : AmazonServiceException
	{
		public AmazonCognitoIdentityException(string message)
			: base(message)
		{
		}

		public AmazonCognitoIdentityException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public AmazonCognitoIdentityException(Exception innerException)
			: base(innerException.Message, innerException)
		{
		}

		public AmazonCognitoIdentityException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		public AmazonCognitoIdentityException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		protected AmazonCognitoIdentityException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
