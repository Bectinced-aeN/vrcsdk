using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;

namespace Amazon.SecurityToken.Model
{
	[Serializable]
	public class IDPCommunicationErrorException : AmazonSecurityTokenServiceException
	{
		public IDPCommunicationErrorException(string message)
			: base(message)
		{
		}

		public IDPCommunicationErrorException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public IDPCommunicationErrorException(Exception innerException)
			: base(innerException)
		{
		}

		public IDPCommunicationErrorException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0003: Unknown result type (might be due to invalid IL or missing references)


		public IDPCommunicationErrorException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


		protected IDPCommunicationErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
