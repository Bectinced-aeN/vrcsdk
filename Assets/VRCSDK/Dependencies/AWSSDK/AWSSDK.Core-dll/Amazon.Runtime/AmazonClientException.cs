using System;
using System.Runtime.Serialization;

namespace Amazon.Runtime
{
	[Serializable]
	public class AmazonClientException : Exception
	{
		public AmazonClientException(string message)
			: base(message)
		{
		}

		public AmazonClientException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected AmazonClientException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
