using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Security;
using System.Xml;
using System.Xml.Serialization;

namespace Amazon.S3.Util
{
	[Serializable]
	public class S3PostUploadException : Exception
	{
		public string ErrorCode
		{
			get;
			set;
		}

		public string RequestId
		{
			get;
			set;
		}

		public string HostId
		{
			get;
			set;
		}

		public HttpStatusCode StatusCode
		{
			get;
			set;
		}

		public IDictionary<string, string> ExtraFields
		{
			get;
			set;
		}

		public S3PostUploadException(string message)
			: base(message)
		{
		}

		public S3PostUploadException(string errorCode, string message)
			: base(message)
		{
			ErrorCode = errorCode;
		}

		public static S3PostUploadException FromResponse(HttpWebResponse response)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(S3PostUploadError));
			S3PostUploadError s3PostUploadError = null;
			try
			{
				s3PostUploadError = (xmlSerializer.Deserialize(response.GetResponseStream()) as S3PostUploadError);
			}
			catch
			{
				return new S3PostUploadException("Unknown", "Unknown error");
			}
			S3PostUploadException ex = new S3PostUploadException(s3PostUploadError.ErrorCode, s3PostUploadError.ErrorMessage)
			{
				RequestId = s3PostUploadError.RequestId,
				HostId = s3PostUploadError.HostId
			};
			ex.StatusCode = response.StatusCode;
			ex.ExtraFields = new Dictionary<string, string>();
			if (s3PostUploadError.elements != null)
			{
				XmlElement[] elements = s3PostUploadError.elements;
				foreach (XmlElement xmlElement in elements)
				{
					ex.ExtraFields.Add(xmlElement.LocalName, xmlElement.InnerText);
				}
			}
			return ex;
		}

		protected S3PostUploadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				ErrorCode = info.GetString("ErrorCode");
				RequestId = info.GetString("RequestId");
				HostId = info.GetString("HostId");
				StatusCode = (HttpStatusCode)info.GetValue("StatusCode", typeof(HttpStatusCode));
				ExtraFields = (IDictionary<string, string>)info.GetValue("ExtraFields", typeof(IDictionary<string, string>));
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			if (info != null)
			{
				info.AddValue("ErrorCode", ErrorCode);
				info.AddValue("RequestId", RequestId);
				info.AddValue("HostId", HostId);
				info.AddValue("StatusCode", StatusCode);
				info.AddValue("ExtraFields", ExtraFields);
			}
		}
	}
}
