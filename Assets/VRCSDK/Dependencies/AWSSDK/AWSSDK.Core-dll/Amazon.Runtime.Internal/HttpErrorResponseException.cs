using Amazon.Runtime.Internal.Transform;
using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace Amazon.Runtime.Internal
{
	[Serializable]
	public class HttpErrorResponseException : Exception
	{
		public IWebResponseData Response
		{
			get;
			private set;
		}

		public HttpErrorResponseException(IWebResponseData response)
		{
			Response = response;
		}

		public HttpErrorResponseException(string message, IWebResponseData response)
			: base(message)
		{
			Response = response;
		}

		public HttpErrorResponseException(string message, Exception innerException, IWebResponseData response)
			: base(message, innerException)
		{
			Response = response;
		}

		protected HttpErrorResponseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				Response = (IWebResponseData)info.GetValue("Response", typeof(IWebResponseData));
			}
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info?.AddValue("Response", Response);
		}
	}
}
