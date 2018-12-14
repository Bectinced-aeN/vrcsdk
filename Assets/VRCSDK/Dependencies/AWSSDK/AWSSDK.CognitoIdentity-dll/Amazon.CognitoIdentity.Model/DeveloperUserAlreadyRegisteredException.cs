using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.CognitoIdentity.Model
{
	[Serializable]
	public class DeveloperUserAlreadyRegisteredException : AmazonCognitoIdentityException
	{
		public DeveloperUserAlreadyRegisteredException(string message)
			: base(message)
		{
		}

		public DeveloperUserAlreadyRegisteredException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public DeveloperUserAlreadyRegisteredException(Exception innerException)
			: base(innerException)
		{
		}

		public DeveloperUserAlreadyRegisteredException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		public DeveloperUserAlreadyRegisteredException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		protected DeveloperUserAlreadyRegisteredException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
