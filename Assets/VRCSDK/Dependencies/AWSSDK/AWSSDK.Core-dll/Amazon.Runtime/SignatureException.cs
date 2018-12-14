using Amazon.Runtime.Internal.Auth;
using System;
using System.Runtime.Serialization;

namespace Amazon.Runtime
{
	[Serializable]
	public class SignatureException : Amazon.Runtime.Internal.Auth.SignatureException
	{
		public SignatureException(string message)
			: base(message)
		{
		}

		public SignatureException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected SignatureException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
