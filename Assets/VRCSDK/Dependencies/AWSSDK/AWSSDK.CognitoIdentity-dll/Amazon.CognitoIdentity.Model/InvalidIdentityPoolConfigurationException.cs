using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.CognitoIdentity.Model
{
	[Serializable]
	public class InvalidIdentityPoolConfigurationException : AmazonCognitoIdentityException
	{
		public InvalidIdentityPoolConfigurationException(string message)
			: base(message)
		{
		}

		public InvalidIdentityPoolConfigurationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public InvalidIdentityPoolConfigurationException(Exception innerException)
			: base(innerException)
		{
		}

		public InvalidIdentityPoolConfigurationException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}

		public InvalidIdentityPoolConfigurationException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}

		protected InvalidIdentityPoolConfigurationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
