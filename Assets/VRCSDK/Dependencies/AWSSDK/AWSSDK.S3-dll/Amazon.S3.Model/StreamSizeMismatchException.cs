using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security;

namespace Amazon.S3.Model
{
	[Serializable]
	public class StreamSizeMismatchException : AmazonS3Exception
	{
		public long ExpectedSize
		{
			get;
			set;
		}

		public long ActualSize
		{
			get;
			set;
		}

		public StreamSizeMismatchException(string message)
			: base(message)
		{
		}

		public StreamSizeMismatchException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public StreamSizeMismatchException(string message, long expectedSize, long actualSize, string requestId, string amazonId2)
			: base(message)
		{
			ExpectedSize = expectedSize;
			ActualSize = actualSize;
			this.set_RequestId(requestId);
			base.AmazonId2 = amazonId2;
		}

		public StreamSizeMismatchException(string message, long expectedSize, long actualSize, string requestId, string amazonId2, string amazonCfId)
			: base(message)
		{
			ExpectedSize = expectedSize;
			ActualSize = actualSize;
			this.set_RequestId(requestId);
			base.AmazonId2 = amazonId2;
			base.AmazonCloudFrontId = amazonCfId;
		}

		public StreamSizeMismatchException(string message, Exception innerException, long expectedSize, long actualSize, string requestId, string amazonId2)
			: base(message, innerException)
		{
			ExpectedSize = expectedSize;
			ActualSize = actualSize;
			this.set_RequestId(requestId);
			base.AmazonId2 = amazonId2;
		}

		public StreamSizeMismatchException(Exception innerException)
			: base(innerException.Message, innerException)
		{
		}

		public StreamSizeMismatchException(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


		public StreamSizeMismatchException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: base(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0003: Unknown result type (might be due to invalid IL or missing references)


		public StreamSizeMismatchException(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode, string amazonId2)
			: base(message, innerException, errorType, errorCode, requestId, statusCode, amazonId2)
		{
		}//IL_0003: Unknown result type (might be due to invalid IL or missing references)


		protected StreamSizeMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				ExpectedSize = info.GetInt64("ExpectedSize");
				ActualSize = info.GetInt64("ActualSize");
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("ExpectedSize", ExpectedSize);
				info.AddValue("ActualSize", ActualSize);
			}
		}
	}
}
