using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.SecurityToken
{
	[Serializable]
	public class AmazonSecurityTokenServiceException : AmazonServiceException
	{
		public AmazonSecurityTokenServiceException(string message)
			: this(message)
		{
		}

		public AmazonSecurityTokenServiceException(string message, Exception innerException)
			: this(message, innerException)
		{
		}

		public AmazonSecurityTokenServiceException(Exception innerException)
			: this(innerException.Message, innerException)
		{
		}

		public AmazonSecurityTokenServiceException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: this(message, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


		public AmazonSecurityTokenServiceException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: this(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0003: Unknown result type (might be due to invalid IL or missing references)


		protected AmazonSecurityTokenServiceException(SerializationInfo info, StreamingContext context)
			: this(info, context)
		{
		}
	}
}
