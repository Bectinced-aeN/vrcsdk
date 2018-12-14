using Amazon.Runtime;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security;

namespace Amazon.S3
{
	[Serializable]
	public class AmazonS3Exception : AmazonServiceException
	{
		public string AmazonId2
		{
			get;
			protected set;
		}

		public string AmazonCloudFrontId
		{
			get;
			protected set;
		}

		public string ResponseBody
		{
			get;
			internal set;
		}

		internal string Region
		{
			get;
			set;
		}

		public override string Message
		{
			get
			{
				if (string.IsNullOrEmpty(ResponseBody))
				{
					return base.Message;
				}
				return base.Message + " Response Body: " + ResponseBody;
			}
		}

		public AmazonS3Exception(string message)
			: this(message)
		{
		}

		public AmazonS3Exception(string message, Exception innerException)
			: this(message, innerException)
		{
		}

		public AmazonS3Exception(Exception innerException)
			: this(innerException.Message, innerException)
		{
		}

		public AmazonS3Exception(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: this(message, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


		public AmazonS3Exception(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
			: this(message, innerException, errorType, errorCode, requestId, statusCode)
		{
		}//IL_0003: Unknown result type (might be due to invalid IL or missing references)


		public AmazonS3Exception(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode, string amazonId2)
			: this(message, innerException, errorType, errorCode, requestId, statusCode)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			AmazonId2 = amazonId2;
		}

		public AmazonS3Exception(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode, string amazonId2, string amazonCfId)
			: this(message, innerException, errorType, errorCode, requestId, statusCode)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			AmazonId2 = amazonId2;
			AmazonCloudFrontId = amazonCfId;
		}

		protected AmazonS3Exception(SerializationInfo info, StreamingContext context)
			: this(info, context)
		{
			if (info != null)
			{
				AmazonId2 = info.GetString("AmazonId2");
				ResponseBody = info.GetString("ResponseBody");
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("AmazonId2", AmazonId2);
				info.AddValue("ResponseBody", ResponseBody);
			}
		}
	}
}
