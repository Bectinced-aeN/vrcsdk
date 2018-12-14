using System.Runtime.Serialization;

namespace System.IO
{
	[Serializable]
	public sealed class InvalidDataException : SystemException
	{
		public InvalidDataException()
		{
		}

		public InvalidDataException(string message)
			: base(message)
		{
		}

		public InvalidDataException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		internal InvalidDataException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
