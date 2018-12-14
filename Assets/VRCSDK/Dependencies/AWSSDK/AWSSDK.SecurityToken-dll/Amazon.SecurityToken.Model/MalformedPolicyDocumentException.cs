using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.SecurityToken.Model
{
	[Serializable]
	public class MalformedPolicyDocumentException : AmazonSecurityTokenServiceException
	{
		public MalformedPolicyDocumentException(string message)
			: base(message)
		{
		}

		public MalformedPolicyDocumentException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public MalformedPolicyDocumentException(Exception innerException)
			: base(innerException)
		{
		}

		public MalformedPolicyDocumentException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0003: Unknown result type (might be due to invalid IL or missing references)


		public MalformedPolicyDocumentException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


		protected MalformedPolicyDocumentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
